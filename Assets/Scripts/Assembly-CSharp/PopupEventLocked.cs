using System;
using System.Collections;
using UnityEngine;

public class PopupEventLocked : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_OkButton;

	[SerializeField]
	private UIInputTrigger m_Background;

	[NonSerialized]
	public bool m_IsShowing;

	[SerializeField]
	public Transform m_AssetParent;

	private WaitTimeOrAbort m_AsyncOperation;

	private GameObject m_instantiatedAsset;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_EventLockedPopup = this;
	}

	public void LeavePopup()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine("LeaveCoroutine");
		}
	}

	public WaitTimeOrAbort ShowSonicEventLockedPopup()
	{
		m_IsShowing = true;
		base.gameObject.SetActive(true);
		m_instantiatedAsset = DIContainerInfrastructure.GetCharacterAssetProvider(false).InstantiateObject("SonicDash", m_AssetParent, Vector3.zero, Quaternion.identity);
		if (m_instantiatedAsset == null)
		{
			Debug.LogError("Could not find SonicDash in event asset provider!");
			return m_AsyncOperation;
		}
		m_instantiatedAsset.SetActive(true);
		StartCoroutine("EnterCoroutine");
		m_AsyncOperation = new WaitTimeOrAbort(0f);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		return m_AsyncOperation;
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(true);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_event_locked_enter");
		SetDragControllerActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showSnoutlings = false
		}, true);
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_ArenaLocked_Enter"));
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_event_locked_enter");
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, m_OkButton_Clicked);
		m_OkButton.Clicked += m_OkButton_Clicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_OkButton.Clicked -= m_OkButton_Clicked;
	}

	private IEnumerator LeaveCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(false);
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_event_locked_enter");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_ArenaLocked_Leave"));
		UnityEngine.Object.Destroy(m_instantiatedAsset);
		m_IsShowing = false;
		m_AsyncOperation.Abort();
		m_AsyncOperation = null;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_event_locked_enter");
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
