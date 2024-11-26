using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationPopupController : MonoBehaviour
{
	[SerializeField]
	private UILabel m_Text;

	[SerializeField]
	private List<UIPanel> m_Panels;

	[SerializeField]
	public UIInputTrigger m_AbortButton;

	[SerializeField]
	private UIInputTrigger m_ConfirmButton;

	[SerializeField]
	private UISprite m_ConfirmIcon;

	private Action m_AbortAction;

	private Action m_ConfirmAction;

	private bool m_autoDestroy;

	private string m_atlasName;

	private string m_spriteName;

	public bool m_IsShowing;

	private GameObject m_cachedGameObject;

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		if (m_AbortAction != null)
		{
			AbortButtonClicked();
		}
		else if (m_ConfirmAction != null)
		{
			ConfirmButtonClicked();
		}
		else
		{
			StartCoroutine(LeaveCoroutine(null));
		}
	}

	private void Awake()
	{
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		foreach (UIPanel panel in m_Panels)
		{
			panel.enabled = false;
		}
		m_cachedGameObject = base.gameObject;
	}

	public ConfirmationPopupController SetMessage(string message, bool autoDestroy = true)
	{
		m_Text.text = message;
		m_autoDestroy = autoDestroy;
		return this;
	}

	public ConfirmationPopupController SetConfirmIcon(string atlasName, string spriteName)
	{
		m_atlasName = atlasName;
		m_spriteName = spriteName;
		return this;
	}

	public ConfirmationPopupController SetActions(Action actionOnConfirm, Action actionOnAbort)
	{
		m_ConfirmAction = actionOnConfirm;
		m_AbortAction = actionOnAbort;
		return this;
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(8, HandleBackButton);
		m_AbortButton.Clicked += AbortButtonClicked;
		m_ConfirmButton.Clicked += ConfirmButtonClicked;
	}

	private void DeRegisterEventHandlers()
	{
		if ((bool)DIContainerInfrastructure.BackButtonMgr)
		{
			DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(8);
		}
		if ((bool)m_AbortButton)
		{
			m_AbortButton.Clicked -= AbortButtonClicked;
		}
		if ((bool)m_ConfirmButton)
		{
			m_ConfirmButton.Clicked -= ConfirmButtonClicked;
		}
	}

	private void ConfirmButtonClicked()
	{
		DeRegisterEventHandlers();
		StartCoroutine(LeaveCoroutine(m_ConfirmAction));
	}

	private void AbortButtonClicked()
	{
		DebugLog.Log("[ConfirmationPopupController] Abort Clicked");
		DeRegisterEventHandlers();
		StartCoroutine(LeaveCoroutine(m_AbortAction));
	}

	public void Enter()
	{
		DebugLog.Log("[ConfirmationPopupController] Enter");
		if (m_IsShowing)
		{
			DebugLog.Warn("[ConfirmationPopupController] Enter: already visible!");
		}
		m_IsShowing = true;
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_confirmation_enter");
		foreach (UIPanel panel in m_Panels)
		{
			panel.enabled = true;
		}
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(true);
		m_AbortButton.gameObject.SetActive(m_AbortAction != null);
		m_ConfirmButton.gameObject.SetActive(m_ConfirmAction != null);
		if (!string.IsNullOrEmpty(m_atlasName) && !string.IsNullOrEmpty(m_spriteName))
		{
			GameObject atlasGob = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(m_atlasName) as GameObject;
			if (atlasGob != null)
			{
				m_ConfirmIcon.atlas = atlasGob.GetComponent<UIAtlas>();
				m_ConfirmIcon.spriteName = m_spriteName;
				m_ConfirmIcon.MakePixelPerfect();
			}
			else
			{
				DebugLog.Warn("[ConfirmationPopupController] SetIconForConfirm: can not set atlas to " + m_atlasName + " and sprite to " + m_spriteName + " because atlas is null");
			}
		}
		SetDragControllerActive(false);
		GetComponent<Animation>().Play("Popup_Confirmation_Enter");
		if (DIContainerInfrastructure.GetCoreStateMgr().m_GameIsPaused)
		{
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(GetComponent<Animation>()["Popup_Confirmation_Enter"].length + 0.5f));
		}
		else
		{
			yield return new WaitForSeconds(GetComponent<Animation>()["Popup_Confirmation_Enter"].length + 0.5f);
		}
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_confirmation_enter");
	}

	private IEnumerator LeaveCoroutine(Action actionToInvoke)
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_confirmation_leave");
		DebugLog.Log("[ConfirmationPopupController] LeaveCoroutine, autoDestroy = " + m_autoDestroy + ", actionToInvoke: " + ((actionToInvoke == null) ? "null" : "not null"));
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(false);
		if (m_autoDestroy || actionToInvoke == null)
		{
			yield return StartCoroutine(Shutdown(actionToInvoke));
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_confirmation_leave");
			m_autoDestroy = false;
		}
		else
		{
			m_autoDestroy = false;
			actionToInvoke();
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_confirmation_leave");
		}
	}

	private IEnumerator Shutdown(Action actionToInvoke)
	{
		DebugLog.Log("[ConfirmationPopupController] Shutdown started");
		DeRegisterEventHandlers();
		SetDragControllerActive(true);
		GetComponent<Animation>().Play("Popup_Confirmation_Leave");
		float timeLeft = GetComponent<Animation>()["Popup_Confirmation_Leave"].length;
		while (timeLeft > 0f)
		{
			timeLeft -= Time.fixedDeltaTime;
			yield return new WaitForEndOfFrame();
		}
		m_IsShowing = false;
		m_atlasName = null;
		m_spriteName = null;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_confirmation_leave");
		if (actionToInvoke != null)
		{
			actionToInvoke();
		}
		UnityEngine.Object.Destroy(base.gameObject);
		base.gameObject.SetActive(false);
		DebugLog.Log("[ConfirmationPopupController] Shutdown finished");
	}

	public void Shutdown()
	{
		StartCoroutine(Shutdown(null));
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	public void OnDestroy()
	{
		DeRegisterEventHandlers();
		SetDragControllerActive(true);
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			RegisterEventHandlers();	
		}
	}
}
