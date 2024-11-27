using System;
using System.Collections;
using ABH.GameDatas;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class PvPSeasonStateMgr : MonoBehaviourContainerBase
{
	public bool IsInitialized { get; private set; }

	private IEnumerator Start()
	{
		while (!DIContainerInfrastructure.GetCoreStateMgr().m_isInitialized)
		{
			yield return new WaitForEndOfFrame();
		}
		IInventoryItemGameData unlockItem = null;
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		player.InventoryGameData.StoryItemGained -= OnStoryItemAdded;
		if (!DIContainerLogic.InventoryService.TryGetItemGameData(player.InventoryGameData, "unlock_pvp", out unlockItem) || unlockItem.ItemValue <= 0)
		{
			player.InventoryGameData.StoryItemGained += OnStoryItemAdded;
			yield break;
		}
		if (unlockItem.ItemData.IsNew)
		{
			unlockItem.ItemData.IsNew = false;
			if (player.BannerGameData != null)
			{
				BannerGameData banner = player.BannerGameData;
				banner.Data.Level = player.Data.Level;
				if (banner.BannerTip != null)
				{
					banner.BannerTip.Data.Level = banner.Data.Level;
				}
				if (banner.BannerCenter != null)
				{
					banner.BannerCenter.Data.Level = banner.Data.Level;
				}
				if (banner.BannerEmblem != null)
				{
					banner.BannerEmblem.Data.Level = banner.Data.Level;
				}
			}
			player.Data.PvPTutorialDisplayState = 1u;
			DIContainerInfrastructure.TutorialMgr.StartTutorial("tutorial_pvp_first_fight");
			player.SavePlayerData();
		}
		DebugLog.Log("[PvPSeasonStateMgr] Begin Load Event Balancing!");
		while (DIContainerBalancing.EventBalancingLoadingPending)
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerBalancing.GetEventBalancingDataPoviderAsynch(OnBalancingDataProviderReceived);
	}

	private void OnStoryItemAdded(IInventoryItemGameData obj)
	{
		if (obj.ItemBalancing.NameId == "unlock_pvp")
		{
			StartCoroutine("Start");
		}
	}

	private void OnBalancingDataProviderReceived(IBalancingDataLoaderService balancing)
	{
		DebugLog.Log("[PvPSeasonStateMgr] Event Balancing loaded Begin initialize Event System!");
		InitializePvPSystem();
	}

	private void InitializePvPSystem()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		currentPlayer.GeneratePvPManagerFromProfile();
		InvokeRepeating("UpdatePvPSeason", 0.1f, 5f);
		InvokeRepeating("UpdateMatchmakingScores", 10f, 10f);
		DIContainerLogic.GetPvpObjectivesService().SetPersistedPvPObjectives(currentPlayer);
		if (currentPlayer.CurrentPvPSeasonGameData != null && currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn != null)
		{
			DIContainerLogic.PvPSeasonService.SubmitOfflineMatchmakingAttributes(currentPlayer, currentPlayer.CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.NameId, false);
		}
		IsInitialized = true;
	}

	public void ResetPvPSystem()
	{
		CancelInvoke("UpdatePvPSeason");
		IsInitialized = false;
		DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData = null;
		StartCoroutine(Start());
	}

	private void UpdatePvPSeason()
	{
		DIContainerLogic.GetServerOnlyTimingService().GetTrustedTimeEx(OnTrustedTimeReceivedUpdatePvpSeasonState);
	}

	private void OnTrustedTimeReceivedUpdatePvpSeasonState(DateTime trustedTime)
	{
		if (DIContainerBalancing.EventBalancingService == null)
		{
			return;
		}
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		PvPSeasonManagerGameData pvPSeasonManagerGameData = currentPlayer.CurrentPvPSeasonGameData;
		if (pvPSeasonManagerGameData == null)
		{
			pvPSeasonManagerGameData = DIContainerLogic.PvPSeasonService.StartNewSeason();
			DIContainerLogic.PvPSeasonService.StartNewPvPTurn(pvPSeasonManagerGameData, currentPlayer);
			return;
		}
		PvPSeasonState currentPvPSeasonState = pvPSeasonManagerGameData.CurrentPvPSeasonState;
		PvPSeasonState seasonState = GetSeasonState(pvPSeasonManagerGameData);
		EventManagerState eventManagerState = ((pvPSeasonManagerGameData.CurrentSeasonTurn == null) ? EventManagerState.Invalid : pvPSeasonManagerGameData.CurrentSeasonTurn.Data.CurrentState);
		EventManagerState turnState = GetTurnState(pvPSeasonManagerGameData);
		if (seasonState == PvPSeasonState.Invalid)
		{
			DebugLog.Error(GetType(), "OnTrustedTimeReceivedUpdatePvpSeasonState: State for this season is invalid! Clearing it out...");
			return;
		}
		if (eventManagerState == EventManagerState.Invalid)
		{
			foreach (PvPSeasonManagerBalancingData balancingData in DIContainerBalancing.EventBalancingService.GetBalancingDataList<PvPSeasonManagerBalancingData>())
			{
				if (DIContainerLogic.PvPSeasonService.IsSeasonRunning(balancingData) && (pvPSeasonManagerGameData == null || pvPSeasonManagerGameData.Balancing.NameId != balancingData.NameId))
				{
					pvPSeasonManagerGameData = DIContainerLogic.PvPSeasonService.StartNewSeason(balancingData, DIContainerInfrastructure.GetCurrentPlayer());
				}
			}
		}
		if (currentPvPSeasonState == PvPSeasonState.Pending && (seasonState == PvPSeasonState.Running || seasonState == PvPSeasonState.FinishedWithoutPoints))
		{
			pvPSeasonManagerGameData.CurrentPvPSeasonState = PvPSeasonState.Running;
		}
		if (currentPvPSeasonState >= PvPSeasonState.Running)
		{
			if (eventManagerState <= EventManagerState.Teasing)
			{
				DIContainerLogic.PvPSeasonService.StartNewPvPTurn(pvPSeasonManagerGameData, currentPlayer);
			}
			if (eventManagerState == EventManagerState.Running && turnState >= EventManagerState.Finished)
			{
				DIContainerLogic.PvPSeasonService.FinishCurrentPvPTurn(pvPSeasonManagerGameData);
			}
			if (eventManagerState == EventManagerState.Finished && DIContainerLogic.PvPSeasonService.IsSeasonOver(pvPSeasonManagerGameData.Balancing))
			{
				DIContainerLogic.PvPSeasonService.TriggerSeasonEnd();
			}
		}
		if (seasonState == PvPSeasonState.Running && turnState == EventManagerState.Running)
		{
			DIContainerLogic.PvPSeasonService.StartPvPTurn(pvPSeasonManagerGameData);
		}
	}

	private EventManagerState GetTurnState(PvPSeasonManagerGameData seasonGameData)
	{
		if (seasonGameData == null)
		{
			return EventManagerState.Invalid;
		}
		int num = ((seasonGameData.CurrentSeasonTurn != null) ? seasonGameData.CurrentSeasonTurn.Data.CurrentSeason : 0);
		int currentSeasonTurn = DIContainerLogic.PvPSeasonService.GetCurrentSeasonTurn(seasonGameData.Balancing);
		if (num == 0)
		{
			return (currentSeasonTurn <= seasonGameData.Balancing.SeasonTurnAmount) ? EventManagerState.Running : EventManagerState.FinishedWithoutPoints;
		}
		if (currentSeasonTurn > seasonGameData.CurrentSeasonTurn.Data.CurrentSeason)
		{
			return (seasonGameData.CurrentSeasonTurn.Data.CurrentScore != 0) ? EventManagerState.Finished : EventManagerState.FinishedWithoutPoints;
		}
		if (currentSeasonTurn == seasonGameData.CurrentSeasonTurn.Data.CurrentSeason)
		{
			return EventManagerState.Running;
		}
		return EventManagerState.Invalid;
	}

	private PvPSeasonState GetSeasonState(PvPSeasonManagerGameData seasonGameData)
	{
		if (seasonGameData == null || !seasonGameData.IsValid)
		{
			return PvPSeasonState.Invalid;
		}
		if (DIContainerLogic.PvPSeasonService.IsSeasonRunning(seasonGameData.Balancing))
		{
			return PvPSeasonState.Running;
		}
		if (DIContainerLogic.PvPSeasonService.IsSeasonOver(seasonGameData.Balancing))
		{
			return (seasonGameData.Data.HighestLeagueRecord <= 0) ? PvPSeasonState.FinishedWithoutPoints : PvPSeasonState.Finished;
		}
		return PvPSeasonState.Invalid;
	}

	public void ShowEventResult()
	{
	}

	private void UpdateMatchmakingScores()
	{
		if (IsInitialized && DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && (DIContainerLogic.PvPSeasonService.IsPvPTurnRunning(DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData) || DIContainerLogic.PvPSeasonService.IsWaitingForConfirmation(DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData)) && DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.CurrentSeasonTurn.Data.MatchmakingScoreSubmitted)
		{
			DIContainerLogic.PvPSeasonService.UpdateMatchmakingScoreLeaderboard(DIContainerInfrastructure.GetCurrentPlayer());
		}
	}
}
