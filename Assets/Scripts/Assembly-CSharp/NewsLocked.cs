using System.Collections;
using UnityEngine;

public class NewsLocked : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_LeaveButton;

	private void Awake()
	{
		base.transform.position += DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.transform.position;
		UIPanel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = componentsInChildren;
		foreach (UIPanel uIPanel in array)
		{
			uIPanel.enabled = false;
		}
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, HandleBackButton);
		m_LeaveButton.Clicked += LeaveButtonClicked;
	}

	private void HandleBackButton()
	{
		LeaveButtonClicked();
	}

	private void DeRegisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
		m_LeaveButton.Clicked -= LeaveButtonClicked;
	}

	public void LeaveButtonClicked()
	{
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("event_newslocked_animate");
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_ArenaLocked_Leave"));
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("event_newslocked_animate");
		base.gameObject.SetActive(false);
	}

	public void Enter()
	{
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		}
		base.gameObject.SetActive(true);
		UIPanel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = componentsInChildren;
		foreach (UIPanel uIPanel in array)
		{
			uIPanel.enabled = true;
		}
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("event_newslocked_animate");
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_ArenaLocked_Enter"));
		RegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("event_newslocked_animate");
	}
}
