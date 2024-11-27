using System;
using System.Collections;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class EventBannerUI : MonoBehaviour
{
	[SerializeField]
	private Transform m_EventIconRoot;

	[SerializeField]
	private GameObject m_InfoSpriteRoot;

	[SerializeField]
	private GameObject m_LoadingSpinnerRoot;

	[SerializeField]
	private UILabel m_TimeLabel;

	[SerializeField]
	private UIInputTrigger m_EventMenuButton;

	[SerializeField]
	private GameObject m_BossUi;

	[SerializeField]
	private UILabel m_BossLabel;

	[SerializeField]
	public CharacterHealthBar m_BossHP;

	private EventManagerGameData m_Model;

	private bool m_Entering;

	private bool m_Leaving;

	private bool m_Entered;

	private bool m_oldLabelSet;

	private bool m_EventHasChanged;

	private UILabel m_oldLabel;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged -= GlobalEventStateChanged;
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged += GlobalEventStateChanged;
		m_BossHP.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged -= GlobalEventStateChanged;
	}

	public bool SetModel(EventManagerGameData eventManager)
	{
		if (eventManager == null)
		{
			return false;
		}
		m_EventHasChanged = m_Model != eventManager;
		if (m_EventHasChanged)
		{
			foreach (Transform item in m_EventIconRoot)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		if (eventManager.IsBossEvent)
		{
			m_BossUi.SetActive(true);
			m_TimeLabel.gameObject.SetActive(false);
			if (!m_oldLabelSet)
			{
				m_oldLabel = m_TimeLabel;
				m_oldLabelSet = true;
			}
			m_TimeLabel = m_BossLabel;
			m_TimeLabel.gameObject.SetActive(true);
		}
		else
		{
			if (m_oldLabel != null)
			{
				m_TimeLabel = m_oldLabel;
			}
			m_TimeLabel.gameObject.SetActive(true);
			m_BossUi.SetActive(false);
		}
		m_Model = eventManager;
		return true;
	}

	private IEnumerator CountDownTimer()
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		DateTime targetTime = DIContainerLogic.EventSystemService.GetEventEndTime(m_Model.Balancing);
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_TimeLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(DIContainerLogic.GetTimingService().TimeLeftUntil(targetTime));
			}
			yield return new WaitForSeconds(1f);
		}
	}

	public void Enter()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	public IEnumerator EnterCoroutine()
	{
		if (m_Entered || DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData != m_Model)
		{
			HandleBannerContent();
			yield break;
		}
		while (m_Leaving)
		{
			yield return new WaitForEndOfFrame();
		}
		m_Entering = true;
		HandleBannerContent();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("EventBanner_Enter"));
		m_Entering = false;
		m_Entered = true;
		RegisterEventHandler();
	}

	private void HandleBannerContent()
	{
		StopCoroutine("CountDownTimer");
		if (m_Model != null)
		{
			if (m_Model.IsResultValid)
			{
				m_TimeLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_finished", "Finished!");
				m_InfoSpriteRoot.SetActive(true);
			}
			else
			{
				switch (m_Model.CurrentEventManagerState)
				{
				case EventManagerState.Teasing:
					m_TimeLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_teasing", "Coming Soon!");
					m_InfoSpriteRoot.SetActive(true);
					break;
				case EventManagerState.Running:
					StartCoroutine("CountDownTimer");
					m_InfoSpriteRoot.SetActive(true);
					break;
				case EventManagerState.Finished:
					m_TimeLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_calculating", "Calculating!");
					m_InfoSpriteRoot.SetActive(false);
					break;
				case EventManagerState.FinishedWithoutPoints:
					m_TimeLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_finished", "Finished!");
					m_InfoSpriteRoot.SetActive(true);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}
		StopCoroutine("HandleIconAsset");
		if (base.gameObject.activeSelf)
		{
			StartCoroutine("HandleIconAsset");
		}
	}

	private IEnumerator HandleIconAsset()
	{
		while (m_Model == null || (!m_Model.IsAssetValid && m_EventIconRoot.childCount == 0))
		{
			m_LoadingSpinnerRoot.SetActive(true);
			yield return new WaitForEndOfFrame();
		}
		m_LoadingSpinnerRoot.SetActive(false);
		if (m_EventIconRoot.childCount == 0)
		{
			GameObject eventIcon = DIContainerInfrastructure.EventSystemStateManager.InstantiateEventObject("Icon", m_EventIconRoot);
			if ((bool)eventIcon)
			{
				eventIcon.transform.localScale = Vector3.one;
			}
		}
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		m_EventMenuButton.Clicked += EventMenuButtonClicked;
	}

	private void GlobalEventStateChanged(CurrentGlobalEventState arg1, CurrentGlobalEventState arg2)
	{
		HandleBannerContent();
	}

	private void DeRegisterEventHandler()
	{
		m_EventMenuButton.Clicked -= EventMenuButtonClicked;
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged -= GlobalEventStateChanged;
	}

	private void EventMenuButtonClicked()
	{
		if (m_Model != null)
		{
			if (DIContainerLogic.EventSystemService.IsEventRunning(m_Model.Balancing))
			{
				DIContainerInfrastructure.LocationStateMgr.ShowEventDetailScreen(m_Model);
			}
			else if (DIContainerLogic.EventSystemService.IsEventTeasing(m_Model.Balancing))
			{
				DIContainerInfrastructure.LocationStateMgr.ShowEventPreviewScreen(m_Model);
			}
			else if (DIContainerLogic.EventSystemService.IsWaitingForConfirmation(m_Model))
			{
				DIContainerInfrastructure.LocationStateMgr.ShowEventResultPopup();
			}
		}
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine());
	}

	public IEnumerator LeaveCoroutine()
	{
		if (m_Entered)
		{
			while (m_Entering)
			{
				yield return new WaitForEndOfFrame();
			}
			DeRegisterEventHandler();
			m_Leaving = true;
			yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("EventBanner_Leave"));
			m_Leaving = false;
			m_Entered = false;
			base.gameObject.SetActive(false);
		}
	}

	private void OnDisable()
	{
		m_Leaving = false;
		base.gameObject.SetActive(false);
	}
}
