using System.Collections;
using UnityEngine;

public class PopupShowNotificationStateMgr : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_YesButton;

	[SerializeField]
	private UIInputTrigger m_NoButton;

	[SerializeField]
	private UILabel m_HeaderText;

	[SerializeField]
	private UILabel m_DescriptionText;

	private WaitTimeOrAbort m_AsyncOperation;

	public bool m_IsShowing;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_NotificationPopup = this;
	}

	public WaitTimeOrAbort ShowNotificationPopup()
	{
		m_IsShowing = true;
		m_HeaderText.text = DIContainerInfrastructure.GetLocaService().Tr("popup_notify_header");
		m_DescriptionText.text = DIContainerInfrastructure.GetLocaService().Tr("popup_notify_desc");
		base.gameObject.SetActive(true);
		StartCoroutine("EnterCoroutine");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		DIContainerLogic.NotificationPopupController.m_notificationRequestReasons.Clear();
		m_AsyncOperation = new WaitTimeOrAbort(4.5f);
		return m_AsyncOperation;
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_notification_enter");
		SetDragControllerActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		GetComponent<Animation>().Play("Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["Enter"].length);
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_notification_enter");
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(5, NoButtonClicked);
		m_NoButton.Clicked += NoButtonClicked;
		m_YesButton.Clicked += YesButtonClicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(5);
		m_NoButton.Clicked -= NoButtonClicked;
		m_YesButton.Clicked -= YesButtonClicked;
	}

	private IEnumerator LeaveCoroutine()
	{
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_notification_leave");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Leave"));
		m_IsShowing = false;
		m_AsyncOperation.Abort();
		m_AsyncOperation = null;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_notification_leave");
		base.gameObject.SetActive(false);
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	private void NoButtonClicked()
	{
		DIContainerInfrastructure.GetCurrentPlayer().Data.NotificationUsageState = 1;
		DeRegisterEventHandlers();
		StartCoroutine(LeaveCoroutine());
	}

	private void YesButtonClicked()
	{
		DIContainerInfrastructure.GetCurrentPlayer().Data.NotificationUsageState = 2;
		DeRegisterEventHandlers();
		StartCoroutine(LeaveCoroutine());
	}
}
