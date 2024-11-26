using System;
using System.Collections;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class PvpBannerUI : MonoBehaviour
{
	[SerializeField]
	private UILabel m_TimeLabel;

	[SerializeField]
	private UIInputTrigger m_EventMenuButton;

	[SerializeField]
	private GameObject m_HighlightObject;

	private bool m_Entering;

	private bool m_Leaving;

	private bool m_Entered;

	private bool m_EventHasChanged;

	private ArenaCampStateMgr m_arenaStatemgr;

	private PvPSeasonManagerGameData m_model;

	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	private IEnumerator CountDownTimer()
	{
		PvPSeasonManagerGameData pvPSeasonManager = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		DateTime trustedTime;
		while (!DIContainerLogic.GetServerOnlyTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		DateTime targetTime = DIContainerLogic.PvPSeasonService.GetPvpTurnEndTime(pvPSeasonManager);
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetServerOnlyTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_TimeLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(DIContainerLogic.GetServerOnlyTimingService().TimeLeftUntil(targetTime));
			}
			yield return new WaitForSeconds(1f);
		}
	}

	public IEnumerator EnterCoroutine(ArenaCampStateMgr statemgr)
	{
		m_arenaStatemgr = statemgr;
		if (m_Entered)
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
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("SeasonBanner_Enter"));
		m_Entering = false;
		m_Entered = true;
		RegisterEventHandler();
	}

	private void HandleBannerContent()
	{
		StopCoroutine("CountDownTimer");
		if (!DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			m_TimeLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_calculating", "Calculating!");
			return;
		}
		PvPSeasonManagerGameData currentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		if (currentPvPSeasonGameData == null)
		{
			return;
		}
		if (currentPvPSeasonGameData.CurrentSeasonTurn.IsResultValid)
		{
			m_TimeLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_finished", "Finished!");
			m_HighlightObject.SetActive(true);
			return;
		}
		switch (currentPvPSeasonGameData.CurrentSeasonTurn.CurrentPvPTurnManagerState)
		{
		case EventManagerState.Running:
			m_HighlightObject.SetActive(false);
			StartCoroutine("CountDownTimer");
			break;
		case EventManagerState.Finished:
			m_TimeLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_calculating", "Calculating!");
			m_HighlightObject.SetActive(true);
			break;
		case EventManagerState.FinishedWithoutPoints:
			m_TimeLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_finished", "Finished!");
			m_HighlightObject.SetActive(false);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		m_EventMenuButton.Clicked += EventMenuButtonClicked;
		DIContainerInfrastructure.GetCurrentPlayer().GlobalPvPStateChanged += GlobalPvPStateChanged;
	}

	private void GlobalPvPStateChanged(CurrentGlobalEventState arg1, CurrentGlobalEventState arg2)
	{
		HandleBannerContent();
	}

	private void DeRegisterEventHandler()
	{
		m_EventMenuButton.Clicked -= EventMenuButtonClicked;
		DIContainerInfrastructure.GetCurrentPlayer().GlobalPvPStateChanged -= GlobalPvPStateChanged;
	}

	private void EventMenuButtonClicked()
	{
		PvPSeasonManagerGameData currentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		if (currentPvPSeasonGameData != null)
		{
			if (DIContainerLogic.PvPSeasonService.IsPvPTurnRunning(currentPvPSeasonGameData))
			{
				m_arenaStatemgr.ShowPvpInfoScreen();
			}
			else if (DIContainerLogic.PvPSeasonService.IsWaitingForConfirmation(currentPvPSeasonGameData) && currentPvPSeasonGameData.CurrentSeasonTurn.IsResultValid)
			{
				m_arenaStatemgr.ShowPvPTurnResultScreen();
			}
		}
	}

	public void WaitThenLeave()
	{
		StartCoroutine(WaitThenLeaveCoroutine());
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator WaitThenLeaveCoroutine()
	{
		while (!m_Entered)
		{
			yield return new WaitForEndOfFrame();
		}
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		if (m_Entered)
		{
			while (m_Entering)
			{
				yield return new WaitForEndOfFrame();
			}
			DeRegisterEventHandler();
			m_Leaving = true;
			yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("SeasonBanner_Leave"));
			m_Leaving = false;
			m_Entered = false;
			base.gameObject.SetActive(false);
		}
	}

	private void OnDisable()
	{
		m_Leaving = false;
		base.gameObject.SetActive(false);
		DeRegisterEventHandler();
	}
}
