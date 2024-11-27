using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using UnityEngine;

public class EventPreviewUI : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_LeaveButton;

	[SerializeField]
	private Transform m_ContentRoot;

	[SerializeField]
	private GameObject m_InjectedContent;

	private BaseLocationStateManager m_StateMgr;

	private EventManagerGameData m_Model;

	private bool m_EventHasChanged;

	[SerializeField]
	private List<UIFont> m_FontsToReplace = new List<UIFont>();

	private EventPreviewContent m_Content;

	private string m_returnToScene;

	private void Awake()
	{
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot)
		{
			base.transform.position += DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.transform.position;
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_eventTeaserScreen = this;
		UIPanel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = componentsInChildren;
		foreach (UIPanel uIPanel in array)
		{
			uIPanel.enabled = false;
		}
	}

	public void SetStateMgr(BaseLocationStateManager locationStateMgr)
	{
		m_StateMgr = locationStateMgr;
	}

	public void SetModel(EventManagerGameData eventManagerGameData)
	{
		m_EventHasChanged = m_Model != eventManagerGameData;
		m_Model = eventManagerGameData;
	}

	public void SetHasChanged()
	{
		m_EventHasChanged = true;
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		m_LeaveButton.Clicked += LeaveButtonClicked;
	}

	private void HandleBackButton()
	{
		LeaveButtonClicked();
	}

	private void DeRegisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		m_LeaveButton.Clicked -= LeaveButtonClicked;
	}

	private void LeaveButtonClicked()
	{
		Leave();
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("event_preview_animate");
		if (string.IsNullOrEmpty(m_returnToScene) || m_returnToScene == "WorldMap")
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
			m_StateMgr.WorldMenuUI.Enter();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 0u,
				showFriendshipEssence = true,
				showLuckyCoins = true,
				showSnoutlings = true
			}, true);
		}
		else if (m_returnToScene == "NewsUI")
		{
			m_StateMgr.ShowNewsUi();
		}
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("EventPreviewScreen_Leave"));
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("event_preview_animate");
		base.gameObject.SetActive(false);
	}

	public void Enter(bool showStarting = false, string origin = null)
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveAllBars(true);
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		}
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
		}
		if (origin != null)
		{
			m_returnToScene = origin;
		}
		base.gameObject.SetActive(true);
		UIPanel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPanel>();
		UIPanel[] array = componentsInChildren;
		foreach (UIPanel uIPanel in array)
		{
			uIPanel.enabled = true;
		}
		if (m_StateMgr != null)
		{
			m_StateMgr.WorldMenuUI.Leave();
		}
		StartCoroutine(EnterCoroutine(showStarting));
	}

	private IEnumerator EnterCoroutine(bool showStarting)
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("event_preview_animate");
		if (m_EventHasChanged || m_Model.IsBossEvent || m_Model.IsCampaignEvent)
		{
			if ((bool)m_Content)
			{
				Object.Destroy(m_Content.gameObject);
			}
			GameObject contentGameObject2 = null;
			if (m_InjectedContent == null)
			{
				contentGameObject2 = ((!showStarting) ? DIContainerInfrastructure.EventSystemStateManager.InstantiateEventObject("Image", m_ContentRoot, m_Model) : DIContainerInfrastructure.EventSystemStateManager.InstantiateEventObject("StartingImage", m_ContentRoot, m_Model));
			}
			else
			{
				contentGameObject2 = Object.Instantiate(m_InjectedContent);
				contentGameObject2.transform.parent = m_ContentRoot;
				contentGameObject2.transform.localPosition = Vector3.zero;
			}
			if ((bool)contentGameObject2)
			{
				m_Content = contentGameObject2.GetComponent<EventPreviewContent>();
				if ((bool)m_Content)
				{
					m_Content.SetModel(m_Model);
					UILabel[] allLabels = m_Content.GetComponentsInChildren<UILabel>();
					UILabel[] array = allLabels;
					foreach (UILabel label in array)
					{
						label.font = m_FontsToReplace.FirstOrDefault((UIFont f) => f.name == label.font.name);
					}
				}
			}
		}
		else if ((bool)m_Content)
		{
			m_Content.Refresh();
		}
		RegisterEventHandler();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("EventPreviewScreen_Enter"));
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("event_preview_animate");
	}
}
