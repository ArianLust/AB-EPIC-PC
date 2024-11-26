using System;
using System.Collections;
using UnityEngine;

public class DungeonsLockedPopup : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_OkButton;

	[SerializeField]
	private UIInputTrigger m_Background;

	[NonSerialized]
	public bool m_IsShowing;

	private WaitTimeOrAbort m_AsyncOperation;

	private void Awake()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_DungeonsLockedPopup = this;
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
	}

	public void LeavePopup()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine("LeaveCoroutine");
		}
	}

	public WaitTimeOrAbort ShowDungeonLockedPopup()
	{
		m_IsShowing = true;
		base.gameObject.SetActive(true);
		StartCoroutine("EnterCoroutine");
		m_AsyncOperation = new WaitTimeOrAbort(0f);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		return m_AsyncOperation;
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(true);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_dungeons_locked_enter");
		SetDragControllerActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showSnoutlings = false
		}, true);
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_DungeonsLocked_Enter"));
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_dungeons_locked_enter");
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
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_dungeons_locked_enter");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_DungeonsLocked_Leave"));
		m_IsShowing = false;
		m_AsyncOperation.Abort();
		m_AsyncOperation = null;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_dungeons_locked_enter");
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
