using System.Collections;
using ABH.GameDatas;
using UnityEngine;

public class PopupRateAppStateMgr : MonoBehaviour
{
	[Header("State: Rate App")]
	[SerializeField]
	private GameObject m_GetItButtonRoot;

	[SerializeField]
	private UIInputTrigger m_GetItButton;

	[SerializeField]
	private UIInputTrigger m_AbortButton;

	private RatePopupState m_currentState;

	private BasicItemGameData m_BasicItemGameData;

	[SerializeField]
	[Header("Settings")]
	private float m_MaximumShowTime = 4.5f;

	private WaitTimeOrAbort m_AsyncOperation;

	public bool m_IsShowing;

	private string shopCategory = "shop_premium";

	private bool m_waitingForResume;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_RateAppPopup = this;
	}

	public WaitTimeOrAbort ShowRatingPopup()
	{
		m_IsShowing = true;
		m_waitingForResume = false;
		base.gameObject.SetActive(true);
		m_currentState = RatePopupState.RateApp;
		StartCoroutine("EnterCoroutine");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		DIContainerLogic.RateAppController.m_rateRequestReasons.Clear();
		m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		return m_AsyncOperation;
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_unlock_enter");
		SetDragControllerActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		GetComponent<Animation>().Play("Popup_RateApp_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_RateApp_Leave"].length);
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_unlock_enter");
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(5, m_AbortButton_Clicked);
		m_AbortButton.Clicked += m_AbortButton_Clicked;
		m_GetItButton.Clicked += GetItButtonClicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(5);
		m_AbortButton.Clicked -= m_AbortButton_Clicked;
		m_GetItButton.Clicked -= GetItButtonClicked;
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_unlock_leave");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		if (m_currentState == RatePopupState.Feedback)
		{
			yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_Feedback_Leave"));
		}
		else if (m_currentState == RatePopupState.RateApp)
		{
			yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_RateApp_Leave"));
		}
		m_IsShowing = false;
		m_AsyncOperation.Abort();
		m_AsyncOperation = null;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_unlock_leave");
		base.gameObject.SetActive(false);
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	private void m_AbortButton_Clicked()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.GetCurrentPlayer().Data.LastRatingFailTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		StartCoroutine("LeaveCoroutine");
	}

	private void GetItButtonClicked()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.GetClientUpdateService().OpenAppropriateReviewPage();
		DIContainerLogic.RateAppController.SetRatedVersion();
		StartCoroutine("LeaveCoroutine");
	}

	private void SendFeedbackClicked()
	{
		m_waitingForResume = true;
		DIContainerLogic.RateAppController.InitiateFeedbackEmail();
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused && m_waitingForResume)
		{
			DeRegisterEventHandlers();
			StartCoroutine(LeaveCoroutine());
		}
	}

	private void NoThanksButtonClicked()
	{
		DIContainerLogic.RateAppController.SetRatedVersion();
		StartCoroutine(EnterFeedbackState());
	}

	private IEnumerator EnterFeedbackState()
	{
		DeRegisterEventHandlers();
		yield return base.gameObject.PlayAnimationOrAnimatorState("Popup_Feedback_Enter");
		m_currentState = RatePopupState.Feedback;
		RegisterEventHandlers();
	}
}
