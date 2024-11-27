using System;
using System.Collections;
using UnityEngine;

public class PopupArenaLocked : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_OkButton;

	[SerializeField]
	private UIInputTrigger m_Background;

	[SerializeField]
	private UILabel m_LockedReasonLabel;

	[NonSerialized]
	public bool m_IsShowing;

	private WaitTimeOrAbort m_AsyncOperation;

	private bool m_NoShipLocked;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_ArenaLockedPopup = this;
	}

	public void LeavePopup()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine("LeaveCoroutine");
		}
	}

	public WaitTimeOrAbort ShowArenaLockedPopup(bool noShipLocked)
	{
		m_IsShowing = true;
		m_NoShipLocked = noShipLocked;
		base.gameObject.SetActive(true);
		StartCoroutine("EnterCoroutine");
		m_AsyncOperation = new WaitTimeOrAbort(0f);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		return m_AsyncOperation;
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(true);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_arena_locked_enter");
		SetDragControllerActive(false);
		m_LockedReasonLabel.text = DIContainerInfrastructure.GetLocaService().Tr((!m_NoShipLocked) ? "locked_pvp_gen_desc" : "locked_pvp_desc");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showSnoutlings = false
		}, true);
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_ArenaLocked_Enter"));
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_arena_locked_enter");
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, m_OkButton_Clicked);
		m_OkButton.Clicked += m_OkButton_Clicked;
		m_Background.Clicked += m_OkButton_Clicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_OkButton.Clicked -= m_OkButton_Clicked;
		m_Background.Clicked -= m_OkButton_Clicked;
	}

	private IEnumerator LeaveCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(false);
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_arena_locked_enter");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_ArenaLocked_Leave"));
		m_IsShowing = false;
		m_AsyncOperation.Abort();
		m_AsyncOperation = null;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_arena_locked_enter");
		base.gameObject.SetActive(false);
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	private void m_OkButton_Clicked()
	{
		DeRegisterEventHandlers();
		StartCoroutine("LeaveCoroutine");
	}
}
