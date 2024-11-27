using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class PopupRankUpStateMgr : MonoBehaviour
{
	[SerializeField]
	private GameObject m_ClassAmountRoot;

	[SerializeField]
	private UILabel m_ClassAmountLabel;

	[SerializeField]
	private UILabel m_RankUpDescLabel;

	[SerializeField]
	private UIInputTrigger m_AbortButton;

	[SerializeField]
	private float m_FirstTransitionTime = 2f;

	[SerializeField]
	private float m_AdditionalTransitionTime = 1.5f;

	[SerializeField]
	private UIGrid m_ItemGrid;

	[SerializeField]
	private CHMotionTween m_MotionTween;

	[SerializeField]
	private RankUpClassBlind m_RankUpClassBlindPrefab;

	[SerializeField]
	private float m_MaximumShowTime = 4.5f;

	private WaitTimeOrAbort m_AsyncOperation;

	public bool m_IsShowing;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_RankUpPopup = this;
	}

	public WaitTimeOrAbort ShowRankUpPopup(Dictionary<string, int> classesWithOldLevels)
	{
		m_IsShowing = true;
		if (classesWithOldLevels == null || classesWithOldLevels.Count == 0)
		{
			m_IsShowing = false;
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
			m_AsyncOperation.Abort();
			return m_AsyncOperation;
		}
		m_ClassAmountLabel.text = "x" + classesWithOldLevels.Count.ToString("0");
		m_ItemGrid.transform.localPosition = Vector3.zero;
		foreach (Transform item in m_ItemGrid.transform)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (string key in classesWithOldLevels.Keys)
		{
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, key, out data))
			{
				RankUpClassBlind rankUpClassBlind = Object.Instantiate(m_RankUpClassBlindPrefab, Vector3.zero, Quaternion.identity) as RankUpClassBlind;
				rankUpClassBlind.transform.parent = m_ItemGrid.transform;
				rankUpClassBlind.transform.localPosition = Vector3.zero;
				rankUpClassBlind.transform.localScale = Vector3.one;
				rankUpClassBlind.SetModel(data as ClassItemGameData, classesWithOldLevels[key]);
			}
		}
		base.gameObject.SetActive(true);
		m_ClassAmountRoot.gameObject.SetActive(classesWithOldLevels.Count > 1);
		m_ItemGrid.Reposition();
		StartCoroutine("EnterCoroutine");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, true);
		m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		return m_AsyncOperation;
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_unlock_enter");
		SetDragControllerActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u
		}, true);
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_MasteryUp_Enter"));
		StartCoroutine(SwapBetweenClasses(0));
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_unlock_enter");
	}

	private IEnumerator SwapBetweenClasses(int index)
	{
		if (index == 0)
		{
			yield return new WaitForSeconds(m_FirstTransitionTime);
		}
		else
		{
			yield return new WaitForSeconds(m_AdditionalTransitionTime);
		}
		if (index < m_ItemGrid.transform.childCount - 1)
		{
			m_MotionTween.m_EndOffset = new Vector3(0f - m_ItemGrid.cellWidth, 0f, 0f);
			m_MotionTween.Play();
			yield return new WaitForSeconds(m_MotionTween.m_DurationInSeconds);
			StartCoroutine(SwapBetweenClasses(index + 1));
		}
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, AbortButtonClicked);
		m_AbortButton.Clicked += AbortButtonClicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_AbortButton.Clicked -= AbortButtonClicked;
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_unlock_leave");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_MasteryUp_Leave"));
		foreach (Transform child in m_ItemGrid.transform)
		{
			Object.Destroy(child.gameObject);
		}
		m_ItemGrid.Reposition();
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

	private void AbortButtonClicked()
	{
		DeRegisterEventHandlers();
		StartCoroutine("LeaveCoroutine");
	}
}
