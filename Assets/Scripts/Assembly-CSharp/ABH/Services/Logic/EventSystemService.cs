using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.DTOs;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Chimera.Library.Components.Interfaces;
using Rcs;
using UnityEngine;

namespace ABH.Services.Logic
{
	public class EventSystemService
	{

		private ITimingService m_timingService;

		private bool m_FirstBossBattleHelperFlag;

		public Dictionary<string, int> m_CachedFallBackLoot;

		public Dictionary<string, int> m_CachedSecondaryFallBackLoot;

		private EventDetailUI m_EventDetailUI;

		public uint OldEventScore;

		public long ServerEventScore;

		public List<string> PreviousCurrentOpponents;

		public EventSystemService SetTimingService(ITimingService timingService)
		{
			m_timingService = timingService;
			return this;
		}

		public EventSystemService SetDebugLog(Action<string> debugLog)
		{
			return this;
		}

		public EventSystemService SetErrorLog(Action<string> errorLog)
		{
			return this;
		}

		private void LogDebug(string message)
		{
			DebugLog.Log(GetType(), message);
		}

		private void LogError(string message)
		{
			DebugLog.Error(GetType(), message);
		}

		public bool IsCollectionGroupAvailable()
		{
			if (!IsCurrentEventAvailable())
			{
				return false;
			}
			EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
			return (currentEventManagerGameData.IsCampaignEvent && currentEventManagerGameData.CurrentMiniCampaign != null && currentEventManagerGameData.CurrentMiniCampaign.CollectionGroupBalancing != null) || (currentEventManagerGameData.IsBossEvent && currentEventManagerGameData.CurrentEventBoss != null && currentEventManagerGameData.CurrentEventBoss.CollectionGroupBalancing != null);
		}

		public CollectionGroupBalancingData GetCollectionGroupBalancingForEvent(EventManagerGameData emgr = null)
		{
			if (!IsCollectionGroupAvailable())
			{
				return null;
			}
			if (emgr == null)
			{
				emgr = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
			}
			if (emgr.IsBossEvent)
			{
				return emgr.CurrentEventBoss.CollectionGroupBalancing;
			}
			if (emgr.IsCampaignEvent)
			{
				return emgr.CurrentMiniCampaign.CollectionGroupBalancing;
			}
			return null;
		}

		public bool IsCurrentEventAvailable(PlayerGameData currentPlayer = null)
		{
			if (currentPlayer == null)
			{
				currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			}
			return currentPlayer.CurrentEventManagerGameData != null && currentPlayer.CurrentEventManagerGameData.IsValid;
		}

		public bool IsEventOverNow(EventManagerGameData currentEventManagerGameData)
		{
			return currentEventManagerGameData.Balancing.EventEndTimeStamp < m_timingService.GetCurrentTimestamp() && !IsWaitingForConfirmation(currentEventManagerGameData);
		}

		public bool IsWaitingForConfirmation(EventManagerGameData currentEventManagerGameData)
		{
			return currentEventManagerGameData.CurrentEventManagerState >= EventManagerState.Finished;
		}

		public EventManagerState FinishCurrentEvent(InventoryGameData inventory, EventManagerGameData currentEventManagerGameData)
		{
			RemoveEncounters(inventory);
			RemoveCollectibles(inventory);
			RemoveCampaign(inventory, currentEventManagerGameData.Balancing.NameId);
			ClearBossIntros();
			EventManagerState eventManagerState2 = (currentEventManagerGameData.CurrentEventManagerState = ((currentEventManagerGameData.Data.CurrentScore == 0) ? EventManagerState.FinishedWithoutPoints : EventManagerState.Finished));
			EventManagerState eventManagerState3 = eventManagerState2;
			if (currentEventManagerGameData.CurrentEventManagerState == EventManagerState.Finished)
			{
				LogDebug(string.Concat("Finish Current Event: ", currentEventManagerGameData.Balancing.NameId, " with state: ", eventManagerState3, " and Score: ", currentEventManagerGameData.Data.CurrentScore));
				DIContainerInfrastructure.GetCurrentPlayer().RegisterGlobalEventStateChanged(CurrentGlobalEventState.RunningEvent, CurrentGlobalEventState.FinishedEvent);
				DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("EventNameId", currentEventManagerGameData.Balancing.NameId);
				dictionary.Add("CurrentScore", currentEventManagerGameData.Data.CurrentScore.ToString("0"));
				dictionary.Add("CurrentState", currentEventManagerGameData.CurrentEventManagerState.ToString());
				dictionary.Add("MatchmakingScore", currentEventManagerGameData.Data.MatchmakingScore.ToString("0"));
				DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("EventStateChanged", dictionary);
			}
			else
			{
				LogDebug(string.Concat("Finish Current Event without points: ", currentEventManagerGameData.Balancing.NameId, " with state: ", eventManagerState3, " and Score: ", currentEventManagerGameData.Data.CurrentScore));
				if (DIContainerInfrastructure.LocationStateMgr != null && DIContainerInfrastructure.LocationStateMgr is EventCampaignStateMgr)
				{
					DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
				}
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				dictionary2.Add("EventNameId", currentEventManagerGameData.Balancing.NameId);
				dictionary2.Add("CurrentScore", currentEventManagerGameData.Data.CurrentScore.ToString("0"));
				dictionary2.Add("CurrentState", currentEventManagerGameData.CurrentEventManagerState.ToString());
				dictionary2.Add("MatchmakingScore", currentEventManagerGameData.Data.MatchmakingScore.ToString("0"));
				DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("EventStateChanged", dictionary2);
				DIContainerInfrastructure.GetCurrentPlayer().RemoveEventManager();
				RemoveEncounters(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData);
				RemoveCollectibles(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData);
				DIContainerInfrastructure.GetCurrentPlayer().RegisterGlobalEventStateChanged(CurrentGlobalEventState.FinishedEvent, CurrentGlobalEventState.NoEvent);
			}
			if ((bool)DIContainerInfrastructure.EventSystemStateManager)
			{
				DIContainerInfrastructure.EventSystemStateManager.ResetEventManager();
				if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData == null)
				{
					DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData = currentEventManagerGameData;
				}
			}
			return eventManagerState3;
		}

		public void SetEventResultValid(PlayerGameData currentPlayer, EventManagerGameData currentEventManagerGameData)
		{
			currentEventManagerGameData.ResultRank = currentEventManagerGameData.GetCurrentRank;
			int wheelIndex = 0;
			if (currentEventManagerGameData.ResultStars > 0)
			{
				if (currentEventManagerGameData.Data.CachedRolledResultWheelIndex > -1)
				{
					wheelIndex = currentEventManagerGameData.Data.CachedRolledResultWheelIndex;
					currentEventManagerGameData.RolledResultLoot = DIContainerLogic.GetLootOperationService().GenerateLootForcedWheelIndex(currentEventManagerGameData.EventBalancing.EventRewardLootTableWheel, currentPlayer.Data.Level, currentEventManagerGameData.ResultStars, ref wheelIndex);
				}
				else
				{
					currentEventManagerGameData.RolledResultLoot = DIContainerLogic.GetLootOperationService().GenerateLoot(currentEventManagerGameData.EventBalancing.EventRewardLootTableWheel, currentPlayer.Data.Level, currentEventManagerGameData.ResultStars, ref wheelIndex);
				}
				currentEventManagerGameData.RolledResultLoot = CheckForMasteryReplacement(currentEventManagerGameData.RolledResultLoot);
			}
			currentEventManagerGameData.Data.CachedRolledResultWheelIndex = wheelIndex;
			if (currentEventManagerGameData.EventBalancing.EventBonusLootTablesPerRank.Count >= currentEventManagerGameData.ResultRank)
			{
				currentEventManagerGameData.FinalRankBonusLoot = DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { 
				{
					currentEventManagerGameData.GetScalingRankRewardLootTable(),
					1
				} }, currentPlayer.Data.Level);
			}
			currentEventManagerGameData.IsResultValid = true;
			currentPlayer.SavePlayerData();
			DIContainerInfrastructure.GetCurrentPlayer().RegisterGlobalEventStateChanged(CurrentGlobalEventState.FinishedEvent, CurrentGlobalEventState.FinishedEventAndResultValid);
		}

		private Dictionary<string, LootInfoData> CheckForMasteryReplacement(Dictionary<string, LootInfoData> RolledResultLoot)
		{
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			Dictionary<string, LootInfoData> dictionary = new Dictionary<string, LootInfoData>();
			foreach (string key2 in RolledResultLoot.Keys)
			{
				if (key2.Contains("mastery"))
				{
					MasteryItemBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<MasteryItemBalancingData>(key2);
					if (!DIContainerLogic.InventoryService.IsAddMasteryPossible(balancingData, currentPlayer))
					{
						Dictionary<string, LootInfoData> dictionary2 = DIContainerLogic.GetLootOperationService().GenerateLoot(balancingData.FallbackLootTable, currentPlayer.Data.Level);
						LootInfoData lootInfoData = dictionary2.Values.FirstOrDefault();
						string key = dictionary2.Keys.FirstOrDefault();
						if (dictionary.ContainsKey(key))
						{
							dictionary[key].Value += lootInfoData.Value;
						}
						else
						{
							dictionary.Add(key, lootInfoData);
						}
					}
					else
					{
						dictionary.Add(key2, RolledResultLoot[key2]);
					}
				}
				else
				{
					dictionary.Add(key2, RolledResultLoot[key2]);
				}
			}
			return dictionary;
		}

		public bool ConfirmCurrentEvent(PlayerGameData currentPlayer)
		{
			if (!IsCurrentEventAvailable(currentPlayer) || !IsWaitingForConfirmation(currentPlayer.CurrentEventManagerGameData))
			{
				LogError("There is no finished event to confirm");
				return false;
			}
			string nameId = currentPlayer.CurrentEventManagerGameData.Balancing.NameId;
			LogDebug("Confirmed Current Event: " + nameId);
			if (currentPlayer.CurrentEventManagerGameData.RolledResultLoot != null && currentPlayer.CurrentEventManagerGameData.RolledResultLoot.Count > 0)
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(currentPlayer.InventoryGameData, 1, currentPlayer.CurrentEventManagerGameData.RolledResultLoot, new Dictionary<string, string>
				{
					{ "EventName", nameId },
					{ "LootType", "Wheel" }
				});
			}
			if (currentPlayer.CurrentEventManagerGameData.FinalRankBonusLoot.Count > 0)
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(currentPlayer.InventoryGameData, 1, currentPlayer.CurrentEventManagerGameData.FinalRankBonusLoot, new Dictionary<string, string>
				{
					{ "EventName", nameId },
					{ "LootType", "RankBonus" }
				});
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("EventNameId", nameId);
			dictionary.Add("CurrentScore", currentPlayer.CurrentEventManagerGameData.Data.CurrentScore.ToString("0"));
			dictionary.Add("CurrentState", EventManagerState.FinishedAndConfirmed.ToString());
			dictionary.Add("MatchmakingScore", currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore.ToString("0"));
			if (currentPlayer.CurrentEventManagerGameData.Data.CurrentOpponents != null)
			{
				for (int i = 0; i < currentPlayer.CurrentEventManagerGameData.Data.CurrentOpponents.Count; i++)
				{
					string value = currentPlayer.CurrentEventManagerGameData.Data.CurrentOpponents[i];
					if (!string.IsNullOrEmpty(value))
					{
						dictionary.Add("Opponent" + i.ToString("00"), value);
					}
				}
			}
			dictionary.Add("Rank", currentPlayer.CurrentEventManagerGameData.ResultRank.ToString("0"));
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("EventStateChanged", dictionary);
			ClearEvent(currentPlayer, nameId);
			return true;
		}

		public void ClearEvent(PlayerGameData currentPlayer, string eventName)
		{
			currentPlayer.RemoveEventManager();
			RemoveEncounters(currentPlayer.InventoryGameData);
			RemoveCollectibles(currentPlayer.InventoryGameData);
			RemoveCampaign(currentPlayer.InventoryGameData, eventName);
			currentPlayer.RegisterGlobalEventStateChanged(CurrentGlobalEventState.FinishedEvent, CurrentGlobalEventState.NoEvent);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}

		public bool IsEventTeasing(EventManagerBalancingData eventManagerBalancing)
		{
			return eventManagerBalancing != null && eventManagerBalancing.EventTeaserStartTimeStamp != 0 && m_timingService.IsAfter(m_timingService.GetDateTimeFromTimestamp(eventManagerBalancing.EventTeaserStartTimeStamp)) && m_timingService.IsBefore(m_timingService.GetDateTimeFromTimestamp(eventManagerBalancing.EventStartTimeStamp));
		}

		public bool IsEventRunning(EventManagerGameData eventManager)
		{
			return IsEventRunning(eventManager.Balancing);
		}

		public bool IsEventRunning(EventManagerBalancingData eventManagerBalancing)
		{
			return eventManagerBalancing != null && m_timingService.IsAfter(m_timingService.GetDateTimeFromTimestamp(eventManagerBalancing.EventStartTimeStamp)) && m_timingService.IsBefore(m_timingService.GetDateTimeFromTimestamp(eventManagerBalancing.EventEndTimeStamp));
		}

		public TimeSpan GetTimeTillEventEnd(EventManagerBalancingData eventManagerBalancing)
		{
			return m_timingService.TimeLeftUntil(m_timingService.GetDateTimeFromTimestamp(eventManagerBalancing.EventEndTimeStamp));
		}

		public DateTime GetEventEndTime(EventManagerBalancingData eventManagerBalancing)
		{
			return m_timingService.GetDateTimeFromTimestamp(eventManagerBalancing.EventEndTimeStamp);
		}

		public DateTime GetTeasingEndTime(EventManagerBalancingData eventManagerBalancing)
		{
			return m_timingService.GetDateTimeFromTimestamp(eventManagerBalancing.EventStartTimeStamp);
		}

		public void StartNewEvent(EventManagerBalancingData eventManagerBalancing, PlayerGameData playerGameData)
		{
			LogDebug(string.Concat("Start New Event: ", eventManagerBalancing.NameId, " with start Time: ", m_timingService.GetDateTimeFromTimestamp(eventManagerBalancing.EventStartTimeStamp), " at time: ", m_timingService.GetPresentTime()));
			ResetBoss();
			EventManagerGameData eventManagerGameData = new EventManagerGameData().SetBalancingData(eventManagerBalancing).CreateNewInstance();
			playerGameData.SetEventManager(eventManagerGameData);
			playerGameData.CurrentEventManagerGameData.CurrentEventManagerState = EventManagerState.Running;
			playerGameData.CurrentEventManagerGameData.Data.StartingPlayerLevel = playerGameData.Data.Level;
			if (eventManagerGameData.IsBossEvent)
			{
				playerGameData.Data.WorldBoss = new WorldEventBossData();
				WorldEventBossData worldBoss = playerGameData.Data.WorldBoss;
				worldBoss.DeathCount = 0;
				worldBoss.DefeatedTimestamp = new List<uint>();
				worldBoss.NameId = playerGameData.CurrentEventManagerGameData.EventBalancing.BossId;
				worldBoss.NumberOfAttacks = 0;
				worldBoss.OwnTeamId = 0;
				worldBoss.Team1 = null;
				worldBoss.Team2 = null;
				worldBoss.VictoryCount = 0;
			}
			DIContainerInfrastructure.GetCurrentPlayer().RegisterGlobalEventStateChanged(CurrentGlobalEventState.NoEvent, CurrentGlobalEventState.RunningEvent);
			playerGameData.CurrentEventManagerGameData.Data.CurrentScore = 0u;
			if (!playerGameData.CurrentEventManagerGameData.Data.MatchmakingScoreSubmitted)
			{
				SubmitMatchmakingScore(playerGameData, eventManagerBalancing.NameId);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("EventNameId", eventManagerBalancing.NameId);
			dictionary.Add("CurrentScore", playerGameData.CurrentEventManagerGameData.Data.CurrentScore.ToString("0"));
			dictionary.Add("CurrentState", playerGameData.CurrentEventManagerGameData.CurrentEventManagerState.ToString());
			dictionary.Add("MatchmakingScore", playerGameData.CurrentEventManagerGameData.Data.MatchmakingScore.ToString("0"));
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("EventStateChanged", dictionary);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}

		public void TeaseNewEvent(EventManagerBalancingData eventManager, PlayerGameData playerGameData)
		{
			LogDebug(string.Concat("Tease New Event: ", eventManager.NameId, " with tease start Time: ", m_timingService.GetDateTimeFromTimestamp(eventManager.EventTeaserStartTimeStamp), " and start Time: ", m_timingService.GetDateTimeFromTimestamp(eventManager.EventStartTimeStamp), " at time: ", m_timingService.GetPresentTime()));
			playerGameData.SetEventManager(new EventManagerGameData().SetBalancingData(eventManager).CreateNewInstance());
			playerGameData.CurrentEventManagerGameData.CurrentEventManagerState = EventManagerState.Teasing;
			playerGameData.CurrentEventManagerGameData.Data.CurrentScore = 0u;
			playerGameData.RegisterGlobalEventStateChanged(CurrentGlobalEventState.NoEvent, CurrentGlobalEventState.TeasingEvent);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("EventNameId", eventManager.NameId);
			dictionary.Add("CurrentScore", playerGameData.CurrentEventManagerGameData.Data.CurrentScore.ToString("0"));
			dictionary.Add("CurrentState", playerGameData.CurrentEventManagerGameData.CurrentEventManagerState.ToString());
			dictionary.Add("MatchmakingScore", playerGameData.CurrentEventManagerGameData.Data.MatchmakingScore.ToString("0"));
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("EventStateChanged", dictionary);
			if (playerGameData.CurrentEventManagerGameData.Data.CurrentOpponents == null && DIContainerLogic.EventSystemService.PreviousCurrentOpponents != null)
			{
				playerGameData.CurrentEventManagerGameData.Data.CurrentOpponents = DIContainerLogic.EventSystemService.PreviousCurrentOpponents;
			}
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}

		public void TeaseEvent(EventManagerGameData currentEventManagerGameData)
		{
			if (currentEventManagerGameData != null && currentEventManagerGameData.CurrentEventManagerState != 0)
			{
				LogDebug(string.Concat("Tease Event: ", currentEventManagerGameData.Balancing.NameId, " with tease start Time: ", m_timingService.GetDateTimeFromTimestamp(currentEventManagerGameData.Balancing.EventTeaserStartTimeStamp), " and start Time: ", m_timingService.GetDateTimeFromTimestamp(currentEventManagerGameData.Balancing.EventStartTimeStamp), " at time: ", m_timingService.GetPresentTime()));
				currentEventManagerGameData.CurrentEventManagerState = EventManagerState.Teasing;
				currentEventManagerGameData.Data.CurrentScore = 0u;
				DIContainerInfrastructure.GetCurrentPlayer().RegisterGlobalEventStateChanged(CurrentGlobalEventState.NoEvent, CurrentGlobalEventState.RunningEvent);
				DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			}
		}

		public bool StartEvent(EventManagerGameData currentEventManagerGameData)
		{
			if (currentEventManagerGameData == null)
			{
				global::DebugLog.Warn(base.GetType(), "StartEvent: eventManagerGameData is null. Cannot start null!");
				return false;
			}

			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (currentEventManagerGameData.CurrentEventManagerState < EventManagerState.Running)
			{
				this.LogDebug(string.Concat(new object[]
				{
					"Start Event: ",
					currentEventManagerGameData.Balancing.NameId,
					" with start Time: ",
					this.m_timingService.GetDateTimeFromTimestamp(currentEventManagerGameData.Balancing
						.EventStartTimeStamp),
					" at time: ",
					this.m_timingService.GetPresentTime()
				}));
				currentEventManagerGameData.CurrentEventManagerState = EventManagerState.Running;
				currentEventManagerGameData.Data.StartingPlayerLevel = currentPlayer.Data.Level;
				if (currentEventManagerGameData.IsBossEvent)
				{
					currentPlayer.Data.WorldBoss = new WorldEventBossData();
					WorldEventBossData worldBoss = currentPlayer.Data.WorldBoss;
					worldBoss.DeathCount = 0;
					worldBoss.DefeatedTimestamp = new List<uint>();
					worldBoss.NameId = currentPlayer.CurrentEventManagerGameData.EventBalancing.BossId;
					worldBoss.NumberOfAttacks = 0;
					worldBoss.OwnTeamId = 0;
					worldBoss.Team1 = null;
					worldBoss.Team2 = null;
					worldBoss.VictoryCount = 0;
				}

				currentPlayer.RegisterGlobalEventStateChanged(CurrentGlobalEventState.NoEvent, CurrentGlobalEventState.RunningEvent);
				if (!currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScoreSubmitted &&
				    !DIContainerConfig.GetClientConfig().UseChimeraLeaderboards)
				{
					SubmitMatchmakingScore(currentPlayer, currentEventManagerGameData.Balancing.NameId);
				}

				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("EventNameId", currentEventManagerGameData.Balancing.NameId);
				dictionary.Add("CurrentScore", currentEventManagerGameData.Data.CurrentScore.ToString("0"));
				dictionary.Add("CurrentState", currentEventManagerGameData.CurrentEventManagerState.ToString());
				dictionary.Add("MatchmakingScore", currentEventManagerGameData.Data.MatchmakingScore.ToString("0"));
				DIContainerInfrastructure.GetAnalyticsSystem(false).LogEventWithParameters("EventStateChanged", dictionary, false);
				currentPlayer.SavePlayerData();
				return true;
			}

			if (!DIContainerConfig.GetClientConfig().UseChimeraLeaderboards &&
			    !currentEventManagerGameData.Data.MatchmakingScoreSubmitted &&
			    !currentEventManagerGameData.CalledMatchmakeOnce)
			{
				currentEventManagerGameData.CalledMatchmakeOnce = true;
				SubmitMatchmakingScore(currentPlayer, currentEventManagerGameData.Balancing.NameId);
			}

			return false;
		}

		public EventItemGameData GenerateEventItem(EventBalancingData eventBalancingData, PlayerGameData player)
		{
			List<IInventoryItemGameData> source = DIContainerLogic.GetLootOperationService().RewardLoot(player.InventoryGameData, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(eventBalancingData.EventGeneratorItemLootTable, player.Data.Level), new Dictionary<string, string> { { "EventName", eventBalancingData.NameId } });
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			return source.FirstOrDefault() as EventItemGameData;
		}

		public EventItemGameData GenerateEventCollectible(EventBalancingData eventBalancingData, PlayerGameData player)
		{
			List<IInventoryItemGameData> source = DIContainerLogic.GetLootOperationService().RewardLoot(player.InventoryGameData, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(eventBalancingData.EventCollectibleGeneratorItemLootTable, player.Data.Level), new Dictionary<string, string> { { "EventName", eventBalancingData.NameId } });
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			return source.FirstOrDefault() as EventItemGameData;
		}

		public EventItemGameData GenerateEventMiniCampaignPortal(EventBalancingData eventBalancingData, PlayerGameData player)
		{
			List<IInventoryItemGameData> source = DIContainerLogic.GetLootOperationService().RewardLoot(player.InventoryGameData, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(eventBalancingData.EventMiniCampaignItemLootTable, player.Data.Level), new Dictionary<string, string> { { "EventName", eventBalancingData.NameId } });
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			return source.FirstOrDefault() as EventItemGameData;
		}

		public void RemoveEncounters(InventoryGameData inventoryGameData)
		{
			inventoryGameData.ClearInventoryOfType(InventoryItemType.EventBattleItem);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}

		public void RemoveCollectibles(InventoryGameData inventoryGameData)
		{
			inventoryGameData.ClearInventoryOfType(InventoryItemType.EventCollectible);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}

		public void RemoveCampaign(InventoryGameData inventory, string campaignName)
		{
			inventory.ClearInventoryOfType(InventoryItemType.EventCampaignItem);
			int itemValue = DIContainerLogic.InventoryService.GetItemValue(inventory, "unlock_eventcampaign_dungeon");
			if (itemValue > 0)
			{
				DIContainerLogic.InventoryService.RemoveItem(inventory, "unlock_eventcampaign_dungeon", itemValue, "event campaign finished");
			}
			itemValue = DIContainerLogic.InventoryService.GetItemValue(inventory, "unlock_eventcampaign_moviedungeon");
			if (itemValue > 0)
			{
				DIContainerLogic.InventoryService.RemoveItem(inventory, "unlock_eventcampaign_moviedungeon", itemValue, "event campaign finished");
			}
			RemoveCollectionComponents(inventory, campaignName);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}

		private void ClearBossIntros()
		{
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (currentPlayer.Data.BossIntrosPlayed == null)
			{
				currentPlayer.Data.BossIntrosPlayed = new List<string>();
			}
			currentPlayer.Data.BossIntrosPlayed.Clear();
			currentPlayer.InventoryGameData.Items[InventoryItemType.EventBossItem].Clear();
			currentPlayer.InventoryGameData.Data.EventItems.Clear();
		}

		public void ResetBoss()
		{
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			currentPlayer.Data.WorldBoss = null;
			currentPlayer.Data.WorldBossPlayersAttacksTimestamps = new Dictionary<string, List<uint>>();
		}

		public void RemoveCollectionComponents(InventoryGameData inventory, string eventName)
		{
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (currentPlayer.Data.CollectiblesPerEvent == null)
			{
				currentPlayer.Data.CollectiblesPerEvent = new Dictionary<string, int>();
			}
			int num = 0;
			foreach (IInventoryItemGameData item in inventory.Items[InventoryItemType.CollectionComponent])
			{
				if (item.Name != "collection_event_stars")
				{
					num += item.ItemValue;
				}
			}
			if (!string.IsNullOrEmpty(eventName))
			{
				if (currentPlayer.Data.CollectiblesPerEvent.ContainsKey(eventName))
				{
					currentPlayer.Data.CollectiblesPerEvent[eventName] = Mathf.Max(num, currentPlayer.Data.CollectiblesPerEvent[eventName]);
				}
				else
				{
					currentPlayer.Data.CollectiblesPerEvent.Add(eventName, num);
				}
			}
			inventory.ClearInventoryOfType(InventoryItemType.CollectionComponent);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}

		public void RemoveCollectibleFromLocation(string locationId)
		{
			DIContainerInfrastructure.EventSystemStateManager.RemoveEventCollectibleFromLocation(locationId);
		}

		// public void SubmitEventScore(PlayerGameData player, EventManagerGameData eventManager, Action<RESTResultEnum> uiCallback = null)
		// {
		// 	if (eventManager == null || !IsSubmitAllowed(player))
		// 	{
		// 		return;
		// 	}
		// 	RemoveCheatingEventScoreFix();
		// 	Leaderboard.Score score = new Leaderboard.Score(eventManager.Balancing.NameId);
		// 	score.SetPoints(eventManager.Data.CurrentScore);
		// 	LogDebug("Final Score: " + eventManager.Data.CurrentScore);
		// 	DIContainerInfrastructure.ScoringService.SubmitScore(eventManager.Balancing.NameId, eventManager.Data.CurrentScore, delegate
		// 	{
		// 		if (uiCallback != null)
		// 		{
		// 			uiCallback(RESTResultEnum.Success);
		// 		}
		// 		OnSuccessfullySubmittedEventScore();
		// 		global::DebugLog.Log("HatchScore Submit Successful!");
		// 	}, delegate(int errorCode)
		// 	{
		// 		if (uiCallback != null)
		// 		{
		// 			uiCallback(RESTResultEnum.Fail);
		// 		}
		// 		OnErrorSubmitingEventScore(errorCode);
		// 	});
		// }
		// above is igtbap's hacked version of submiteventscore to use rovio leaderboard
		
		public void SubmitEventScore(PlayerGameData player, EventManagerGameData eventManager, Action<RESTResultEnum> uiCallback = null)
		{
			if (eventManager == null || !this.IsSubmitAllowed(player))
			{
				return;
			}
			if (DIContainerConfig.GetClientConfig().UseChimeraLeaderboards && eventManager.IsQualifiedForLeaderboard)
			{
				int level = player.Data.Level;
				if (eventManager.IsBossEvent && string.IsNullOrEmpty(eventManager.Data.LeaderboardId))
				{
					this.m_FirstBossBattleHelperFlag = true;
				}
				global::DebugLog.Log(base.GetType(), "SubmitEventScore: POSTing AddEventScore to server with score = " + eventManager.Data.CurrentScore);
				DIContainerLogic.BackendService.AddEventScore(eventManager.Balancing.NameId, (long)((ulong)eventManager.Data.CurrentScore), level, eventManager.IsBossEvent, eventManager.Balancing.EventEndTimeStamp, delegate(AddEventScoreResponseDto response)
				{
					this.OnSuccessfullySubmittedEventScore(response, uiCallback);
				}, delegate(int errorcode)
				{
					if (uiCallback != null)
					{
						uiCallback((RESTResultEnum)errorcode);
					}
					this.OnErrorSubmitingEventScore(errorcode);
				});
			}
			else
			{
				DIContainerInfrastructure.ScoringService.SubmitScore(eventManager.Balancing.NameId, (long)((ulong)eventManager.Data.CurrentScore), delegate
				{
					global::DebugLog.Log("HatchScore Submit Successful!");
				}, new Action<int>(this.OnErrorSubmitingEventScore));
			}
		}

		public void UpdateEventLeaderboard(string lbId, Dictionary<string, bool> lbPlayerIds, Action successUiCallback = null, Action<int> errorUiCallback = null)
		{
			LogDebug("[EPIC SERVER] UpdateEventLeaderboard: GOGOGO");
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (string.IsNullOrEmpty(lbId) || lbPlayerIds == null || lbPlayerIds.Count <= 1)
			{
				global::DebugLog.Error(GetType(), "[EPIC SERVER] UpdateEventLeaderboard: Got false parameters: id=" + lbId + " and playerIds=" + lbPlayerIds);
				if (successUiCallback != null)
				{
					successUiCallback();
				}
				return;
			}
			EventManagerGameData currentEventManagerGameData = currentPlayer.CurrentEventManagerGameData;
			if (currentEventManagerGameData.Data.CurrentOpponents == null)
			{
				currentEventManagerGameData.Data.CurrentOpponents = new List<string>();
			}
			if (currentEventManagerGameData.Data.CheatingOpponents == null)
			{
				currentEventManagerGameData.Data.CheatingOpponents = new List<string>();
			}
			global::DebugLog.Log(GetType(), "[EPIC SERVER] UpdateEventLeaderboard: setting new leaderboardID: " + lbId);
			currentEventManagerGameData.Data.LeaderboardId = lbId;
			currentEventManagerGameData.Data.CurrentOpponents.Clear();
			currentEventManagerGameData.Data.CheatingOpponents.Clear();
			currentEventManagerGameData.ScoresByPlayer.Clear();
			foreach (KeyValuePair<string, bool> lbPlayerId in lbPlayerIds)
			{
				string key = lbPlayerId.Key;
				if (lbPlayerId.Value && !currentEventManagerGameData.Data.CheatingOpponents.Contains(key))
				{
					currentEventManagerGameData.Data.CheatingOpponents.Add(key);
				}
				if (key == DIContainerInfrastructure.IdentityService.SharedId || currentEventManagerGameData.Data.CurrentOpponents.Contains(key))
				{
					LogDebug("[EPIC SERVER] UpdateEventLeaderboard: ignore own sharedId in the idList");
					continue;
				}
				DebugLog.Log(GetType(), "[EPIC SERVER] SetEventLeaderboard: Added new player to leaderboard: " + key);
				currentEventManagerGameData.Data.CurrentOpponents.Add(key);
				if (!currentEventManagerGameData.ScoresByPlayer.ContainsKey(key))
				{
					currentEventManagerGameData.ScoresByPlayer.Add(key, new Leaderboard.Score(currentEventManagerGameData.Data.NameId, key));
				}
			}
			DebugLog.Log(GetType(), "[EPIC SERVER] SetEventLeaderboard: Getting leaderboard scores for LB " + lbId);
			if (currentPlayer.CurrentEventManagerGameData.IsBossEvent)
			{
				UpdateBossLeaderboardEpicServer(Enumerable.ToList(lbPlayerIds.Keys));
			}
			GetLeaderboardScores(currentPlayer, currentEventManagerGameData.Data.CurrentOpponents, currentEventManagerGameData, successUiCallback, errorUiCallback);
		}

		private void OnSuccessfullySubmittedEventScore(AddEventScoreResponseDto response, Action<RESTResultEnum> uiCallback = null)
		{
			LogDebug("[EPIC SERVER] OnSuccessfullSubmittedEventScore: Succesfully submitted event score. FirstBossBattle = " + m_FirstBossBattleHelperFlag);
			UpdateEventLeaderboard(response.LeaderboardId, response.Leaderboard, delegate
			{
				if (uiCallback != null)
				{
					uiCallback(RESTResultEnum.Success);
				}
			}, delegate(int errorcode)
			{
				if (uiCallback != null)
				{
					uiCallback((RESTResultEnum)errorcode);
				}
			});
			if (m_FirstBossBattleHelperFlag)
			{
				m_FirstBossBattleHelperFlag = false;
				DIContainerLogic.BackendService.TrackBossDefeat(response.LeaderboardId, null, delegate(int errorcode)
				{
					global::DebugLog.Error(GetType(), "BossDefeatTracking failed with errorcode " + errorcode + ". Enum equivalent: " + (RESTResultEnum)errorcode);
				});
			}
		}

		private void OnErrorSubmitingEventScore(int errorCode)
		{
			LogError("Error submitting event score: " + errorCode);
			if (m_EventDetailUI.m_WaitingForServerLabel.text != null)
			{
				m_EventDetailUI.m_WaitingForServerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_leaderboard_error");
			}
		}

		public bool IsSubmitAllowed(PlayerGameData player)
		{
			return player.CurrentEventManagerGameData != null && player.CurrentEventManagerGameData.Data.CurrentState == EventManagerState.Running;
		}

		public void UpdateMatchmakingScore(PlayerGameData player)
		{
			if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(player) && player.CurrentEventManagerGameData.Data.MatchmakingScoreSubmitted)
			{
				long num = player.CurrentEventManagerGameData.Data.MatchmakingScore + player.CurrentEventManagerGameData.Data.MatchmakingScoreOffset;
				DIContainerInfrastructure.ScoringService.SubmitScore(player.CurrentEventManagerGameData.Balancing.NameId + "_matchmaking", (int)num, OnSuccessfullUpdatedMatchmakingScore, OnErrorUpdatingMatchmakingScore);
			}
		}

		private void OnErrorUpdatingMatchmakingScore(int errorcode)
		{
			LogError("Error updating matchmaking score: " + errorcode);
		}

		private void OnSuccessfullUpdatedMatchmakingScore()
		{
			LogDebug("Succesfull updated matchmaking score");
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(currentPlayer) && currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScoreSubmitted)
			{
				LogDebug("Increasing offset for matchmaking score update");
				currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScoreOffset = Mathf.Min(currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScoreOffset + 1, 99);
			}
		}

		public void SubmitMatchmakingScore(PlayerGameData player, string eventManagerId)
		{
			if (player.CurrentEventManagerGameData == null || player.CurrentEventManagerGameData.IsQualifiedForLeaderboard || player.CurrentEventManagerGameData.Data.CurrentScore > 1000000000)
			{
				global::DebugLog.Log(GetType(), "SubmitMatchmakingScore: User has not yet qualified for Leaderboard!");
				return;
			}
			byte b = (byte)(TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours + 13.0);
			long num = 0L;
			num += (long)(Math.Pow(10.0, 13.0) * (double)(int)b);
			LogDebug("Timezone Indicator: " + b);
			int num2 = (int)((DIContainerLogic.GetDeviceTimingService().GetPresentTime() - new DateTime(2014, 1, 1)).TotalDays + (double)player.Data.ActivityIndicator);
			LogDebug("Activity Indicator: " + num2);
			num += (long)(Math.Pow(10.0, 9.0) * (double)num2);
			ExperienceLevelBalancingData balancing;
			if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + player.Data.Level.ToString("00"), out balancing) || DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + (player.Data.Level - 1).ToString("00"), out balancing))
			{
				int matchmakingRangeIndex = balancing.MatchmakingRangeIndex;
				num += (long)(Math.Pow(10.0, 6.0) * (double)matchmakingRangeIndex);
				LogDebug("Level Range Indicator: " + matchmakingRangeIndex);
				int num3 = 0;
				if (player.Data.EventFinishStatistic != null && player.Data.EventFinishStatistic.Count > 0)
				{
					num3 = Mathf.Min(99, Mathf.FloorToInt((float)player.Data.EventFinishStatistic.Sum((int s) => s) / (float)player.Data.EventFinishStatistic.Count) * 100);
					LogDebug("Average Event Group Indicator: " + num3);
					num += (long)(Math.Pow(10.0, 3.0) * (double)num3);
				}
			}
			else
			{
				LogError("Seems to be a cheater!");
				num = 0L;
			}
			System.Random random = new System.Random((int)player.Data.LastSaveTimestamp);
			num += (long)(Math.Pow(10.0, 1.0) * (double)Mathf.FloorToInt((float)random.NextDouble() * 10f - 1f));
			player.CurrentEventManagerGameData.Data.MatchmakingScore = num;
			player.CurrentEventManagerGameData.Data.MatchmakingScoreOffset = 1;
			DIContainerInfrastructure.ScoringService.SubmitScore(eventManagerId + "_matchmaking", num, new Action(this.OnSuccessfullSubmittedMatchmakingScore), new Action<int>(this.OnErrorSubmitingMatchmakingScore));
		}

		private void OnErrorSubmitingMatchmakingScore(int errorCode)
		{
			// if (errorCode < 2)
			// {
			// 	LogError("No such level or invalid parameter while submitting matchmaking score, submitting offline matchmaking as fallback instead");
			// 	SubmitOfflineMatchmakingAttributes(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Balancing.NameId, true);
			// }
			// else
			// {
			// 	LogError("Error submitting matchmaking score: " + errorCode);
			// }
			LogError("Error submitting matchmaking score: " + errorCode);
		}

		private void OnSuccessfullSubmittedMatchmakingScore()
		{
			LogDebug("Succesfull submited matchmaking score");
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(currentPlayer))
			{
				LogDebug("Flagging event as succesful matchmaking score submit");
				currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScoreSubmitted = true;
				Matchmake(currentPlayer, currentPlayer.CurrentEventManagerGameData);
			}
		}

		public bool Matchmake(PlayerGameData player, EventManagerGameData eventManagerGameData)
		{
			if (eventManagerGameData == null)
			{
				LogDebug("Matchmake: eventManager = null!");
				return false;
			}
			LogDebug(" Matchmake: Tries = " + eventManagerGameData.FailedOnlineMatchmakeCount);
			if (player.CurrentEventManagerGameData.Data.CurrentOpponents == null)
			{
				player.CurrentEventManagerGameData.Data.CurrentOpponents = new List<string>();
			}
			if (IsCurrentEventAvailable(player) && eventManagerGameData.Data.MatchmakingScoreSubmitted && eventManagerGameData.Data.CurrentOpponents.Count < eventManagerGameData.Balancing.MaximumMatchmakingPlayers)
			{
				switch (eventManagerGameData.Balancing.MatchmakingStrategy)
				{
				default:
				{
					int num = 1;
					if (num == 1)
					{
						MatchmakingStrategyOffline(player, eventManagerGameData, false);
					}
					else
					{
						MatchmakingStrategyLeaderboard(player, eventManagerGameData);
					}
					break;
				}
				case "offline":
					MatchmakingStrategyOnline(player, eventManagerGameData);
					break;
				}
				return true;
			}
			return false;
		}

		private void MatchmakingStrategyOnline(PlayerGameData player, EventManagerGameData eventManagerGameData)
		{
			LogDebug("[EventSystemService] MatchmakingStrategyOnline: Start Online event Matchmaking via lobbies! Retries used: " + eventManagerGameData.FailedOnlineMatchmakeCount);
			if (IsEventRunning(eventManagerGameData.Balancing) && eventManagerGameData.Balancing.FailedWithNoPlayersCountTillFallback > 0 && eventManagerGameData.FailedOnlineMatchmakeCount >= eventManagerGameData.Balancing.FailedWithNoPlayersCountTillFallback)
			{
				LogError("[EventSystemService] MatchmakingStrategyOnline: Online Matchmaking not successful after " + eventManagerGameData.FailedOnlineMatchmakeCount + " tries fallback to " + eventManagerGameData.Balancing.OnlineFallbackMatchmakingStrategy);
				switch (eventManagerGameData.Balancing.OnlineFallbackMatchmakingStrategy)
				{
				case "offline":
					MatchmakingStrategyOffline(player, eventManagerGameData, true);
					return;
				}
				int num = 1;
				if (num != 1)
				{
					MatchmakingStrategyLeaderboard(player, eventManagerGameData);
					return;
				}
			}
			int num2 = 0;
			ExperienceLevelBalancingData balancing;
			if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + player.Data.Level.ToString("00"), out balancing) || DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + (player.Data.Level - 1).ToString("00"), out balancing))
			{
				num2 = balancing.MatchmakingRangeIndex;
			}
			string text = eventManagerGameData.Balancing.LobbyPrefix.Replace("{value_1}", num2.ToString("00"));
			LogDebug("Start Online event Matchmaking with lobby: " + text);
			DIContainerInfrastructure.MatchmakingService.JoinLobby(text, eventManagerGameData.Balancing.OnlineMatchmakeTimeoutInSec, (int)eventManagerGameData.Balancing.MaximumMatchmakingPlayers, delegate(OnlineMatchmaker.Response result, List<string> ids, string flow)
			{
				OnMatchedOnline(player, result, ids, flow);
			});
		}

		private void MatchmakingStrategyOffline(PlayerGameData player, EventManagerGameData eventManagerGameData, bool asFallback)
		{
			LogDebug("Start Offline Matchmaking!");
			Dictionary<string, object> offlineMatchmakingAttributes = GetOfflineMatchmakingAttributes(player, eventManagerGameData.Data.NameId);
			int maxResults = (int)eventManagerGameData.Balancing.MaximumMatchmakingPlayers;
			if (eventManagerGameData.IsBossEvent)
			{
				maxResults = 71;
			}
			DIContainerInfrastructure.MatchmakingService.MatchOfflineUsers((!asFallback) ? eventManagerGameData.Balancing.OfflineGetCompetitorsFunction : eventManagerGameData.Balancing.OfflineGetCompetitorsFallbackFunction, offlineMatchmakingAttributes, delegate(OfflineMatchmaker.ResultCode result, List<string> ids)
			{
				OnMatchedOffline(player, result, ids);
			}, maxResults);
		}

		private void OnMatchedOffline(PlayerGameData player, OfflineMatchmaker.ResultCode result, List<string> ids)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("AsyncMatchedEvent", dictionary);
			switch (result)
			{
			case OfflineMatchmaker.ResultCode.Success:
				OnSuccessfullMatchedOnline(player, ids, false);
				break;
			case OfflineMatchmaker.ResultCode.ErrorNetworkFailure:
			case OfflineMatchmaker.ResultCode.ErrorOtherReason:
				OnErrorMatchmakingOffline(result.ToString());
				break;
			default:
				throw new ArgumentOutOfRangeException("result");
			}
		}

		private void OnErrorMatchmakingOffline(string error)
		{
			LogError("Error matching with other players: " + error);
		}

		private void OnMatchedOnline(PlayerGameData player, OnlineMatchmaker.Response result, List<string> ids, string flow)
		{
			LogDebug("OnMatchedOnline GO:");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("SyncMatchedEvent", dictionary);
			switch (result.Result)
			{
			case OnlineMatchmaker.Response.ResultType.Success:
				OnSuccessfullMatchedOnline(player, ids, true);
				break;
			case OnlineMatchmaker.Response.ResultType.Cancelled:
			case OnlineMatchmaker.Response.ResultType.ErrorInvalidLobby:
			case OnlineMatchmaker.Response.ResultType.ErrorTimeoutNoOtherPlayers:
			case OnlineMatchmaker.Response.ResultType.ErrorTimeoutServerUnreachable:
			case OnlineMatchmaker.Response.ResultType.ErrorInvalidTimeout:
			case OnlineMatchmaker.Response.ResultType.ErrorInUse:
			case OnlineMatchmaker.Response.ResultType.ErrorOtherReason:
				OnErrorMatchmakingOnline(player, result.Result);
				break;
			}
		}

		private void OnErrorMatchmakingOnline(PlayerGameData player, OnlineMatchmaker.Response.ResultType resultType)
		{
			if (resultType == OnlineMatchmaker.Response.ResultType.ErrorTimeoutNoOtherPlayers && IsEventRunning(player.CurrentEventManagerGameData.Balancing))
			{
				player.CurrentEventManagerGameData.FailedOnlineMatchmakeCount++;
			}
			LogError("Error online event matching with other players: " + resultType);
		}

		private void OnSuccessfullMatchedOnline(PlayerGameData player, List<string> ids, bool bossLbSync)
		{
			if (ids == null)
			{
				global::DebugLog.Warn(GetType(), "OnSuccessfullMatchedOnline: ids is empty!");
				return;
			}
			EventManagerGameData currentEventManagerGameData = player.CurrentEventManagerGameData;
			LogDebug("successful online event matched to other players count: " + ids.Count);
			if (currentEventManagerGameData == null || !IsEventRunning(currentEventManagerGameData.Balancing))
			{
				return;
			}
			if (currentEventManagerGameData.Data.CurrentOpponents == null)
			{
				currentEventManagerGameData.Data.CurrentOpponents = new List<string>();
			}
			for (int i = 0; i < ids.Count; i++)
			{
				if (currentEventManagerGameData.Data.CurrentOpponents.Count >= currentEventManagerGameData.Balancing.MaximumMatchmakingPlayers)
				{
					break;
				}
				string text = ids[i];
				if (!(text == DIContainerInfrastructure.IdentityService.SharedId) && !currentEventManagerGameData.Data.CurrentOpponents.Contains(text))
				{
					currentEventManagerGameData.Data.CurrentOpponents.Add(text);
					global::DebugLog.Log(GetType(), "OnSuccessfullMatchedOnline: Added new Player Score: " + text);
					if (!currentEventManagerGameData.ScoresByPlayer.ContainsKey(text))
					{
						//currentEventManagerGameData.ScoresByPlayer.Add(text, new Leaderboard.Score(currentEventManagerGameData.Data.NameId, text));
					}
				}
			}
			GetLeaderboardScores(player, currentEventManagerGameData.Data.CurrentOpponents, currentEventManagerGameData);
			if (currentEventManagerGameData.IsBossEvent)
			{
				if (DIContainerConfig.GetClientConfig().UseChimeraLeaderboards)
				{
					global::DebugLog.Error(GetType(), "OnSuccessfullMatchedOnline: DEPRECATED entry point!");
				}
				else
				{
					SetupBossLeaderboard(bossLbSync);
				}
			}
		}

		public EventItemGameData GenerateEventBoss(EventBalancingData eventBalancingData, PlayerGameData player)
		{
			List<IInventoryItemGameData> source = DIContainerLogic.GetLootOperationService().RewardLoot(player.InventoryGameData, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(eventBalancingData.EventBossItemLootTable, player.Data.Level), new Dictionary<string, string> { { "EventName", eventBalancingData.NameId } });
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			return source.FirstOrDefault() as EventItemGameData;
		}

		public void SetupBossLeaderboard(bool sync)
		{
			DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(SetupBossLeaderboardCoroutine(sync));
		}

		public void UpdateBossLeaderboardEpicServer(List<string> allIds)
		{
			LogDebug("[EPIC SERVER] UpdateBossLeaderboardEpicServer gogogo");
			if (allIds == null)
			{
				global::DebugLog.Log(GetType(), "[EPIC SERVER] UpdateBossLeaderboardEpicServer: no IDs in parameter! quitting boss group setup...");
				return;
			}
			WorldEventBossData worldBoss = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss;
			if (worldBoss == null)
			{
				global::DebugLog.Warn(GetType(), "UpdateBossLeaderboardEpicServer: bossData == null!");
				return;
			}
			if (string.IsNullOrEmpty(worldBoss.NameId))
			{
				worldBoss.NameId = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.BossId;
			}
			if (worldBoss.Team1 == null)
			{
				worldBoss.Team1 = new WorldBossTeamData();
			}
			if (worldBoss.Team1.TeamPlayerIds == null)
			{
				worldBoss.Team1.TeamPlayerIds = new List<string>();
			}
			if (worldBoss.Team2 == null)
			{
				worldBoss.Team2 = new WorldBossTeamData();
			}
			if (worldBoss.Team2.TeamPlayerIds == null)
			{
				worldBoss.Team2.TeamPlayerIds = new List<string>();
			}
			if (string.IsNullOrEmpty(worldBoss.Team1.NameId) && !string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentEventManager.LeaderboardId))
			{
				ParseAndSetTeamNames(worldBoss);
			}
			LogDebug("[EPIC SERVER] UpdateBossLeaderboardEpicServer: Assigning " + allIds.Count + " players to teams... OWN TEAM = " + worldBoss.OwnTeamId);
			for (int i = 0; i < allIds.Count; i++)
			{
				if (i % 2 == 0)
				{
					if (!worldBoss.Team1.TeamPlayerIds.Contains(allIds[i]))
					{
						worldBoss.Team1.TeamPlayerIds.Add(allIds[i]);
					}
					if (allIds[i] == DIContainerInfrastructure.IdentityService.SharedId)
					{
						worldBoss.OwnTeamId = 1;
					}
				}
				else
				{
					if (!worldBoss.Team2.TeamPlayerIds.Contains(allIds[i]))
					{
						worldBoss.Team2.TeamPlayerIds.Add(allIds[i]);
					}
					if (allIds[i] == DIContainerInfrastructure.IdentityService.SharedId)
					{
						worldBoss.OwnTeamId = 2;
					}
				}
			}
			LogDebug("[EPIC SERVER] UpdateBossLeaderboardEpicServer OWN TEAM = " + worldBoss.OwnTeamId);
		}

		private void ParseAndSetTeamNames(WorldEventBossData worldBossData)
		{
			string leaderboardId = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.LeaderboardId;
			if (!leaderboardId.Contains("names_"))
			{
				global::DebugLog.Error(GetType(), "ParseAndSetTeamNames: Invalid parameter! LBID does not contain team names: " + leaderboardId);
				return;
			}
			string text = leaderboardId.Substring(leaderboardId.IndexOf("names_") + 6);
			global::DebugLog.Log(GetType(), "ParseAndSetTeamNames: lb suffix for naming = " + text);
			string[] array = text.Split(':');
			if (text.Count() < 2)
			{
				global::DebugLog.Error(GetType(), "ParseAndSetTeamNames: Invalid parameter! LBID format unknown: " + leaderboardId);
				return;
			}
			string[] array2 = array[0].Split('_');
			worldBossData.Team1.TeamColor = int.Parse(array2[0]);
			string text2 = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBossData.Team1.TeamColor.ToString("00") + "_name");
			text2 = text2.Replace("{value_1}", array2[1]);
			worldBossData.Team1.NameId = text2;
			string[] array3 = array[1].Split('_');
			worldBossData.Team2.TeamColor = int.Parse(array3[0]);
			string text3 = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBossData.Team2.TeamColor.ToString("00") + "_name");
			text3 = text3.Replace("{value_1}", array3[1]);
			worldBossData.Team2.NameId = text3;
		}

		private IEnumerator SetupBossLeaderboardCoroutine(bool syncronous)
		{
			Dictionary<string, Leaderboard.Score> playerScores = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.ScoresByPlayer;
			LogDebug("[BossSystem] Waiting to fill PPD...");
			while (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.PublicOpponentDatas.Count == 0)
			{
				yield return new WaitForSeconds(0.5f);
			}
			Dictionary<string, PublicPlayerData> playerProfiles = new Dictionary<string, PublicPlayerData>(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.PublicOpponentDatas);
			if (!playerProfiles.ContainsKey(DIContainerInfrastructure.IdentityService.SharedId))
			{
				playerProfiles.Add(DIContainerInfrastructure.IdentityService.SharedId, DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer);
			}
			PublicPlayerData chosenPlayer = null;
			string chosenPlayerId = string.Empty;
			float lowestRandomSeed = float.MaxValue;
			foreach (string id in playerProfiles.Keys)
			{
				PublicPlayerData ppd = playerProfiles[id];
				if (ppd.RandomDecisionSeed < lowestRandomSeed)
				{
					lowestRandomSeed = ppd.RandomDecisionSeed;
					chosenPlayer = ppd;
					chosenPlayerId = id;
				}
			}
			if (chosenPlayerId == DIContainerInfrastructure.IdentityService.SharedId || !syncronous)
			{
				LogDebug("[EventSystemService][BossSystem] You are the chosen one to setup the teams");
				yield break;
			}
			Leaderboard.Score chosenOne = playerScores[chosenPlayerId];
			LogDebug("[EventSystemService][BossSystem] We will wait for player " + chosenPlayer.RandomDecisionSeed + " to setup the teams");
		}

		private ABH.Shared.Models.PlayerData OnProfileSyncError(ABH.Shared.Models.PlayerData current, ABH.Shared.Models.PlayerData remotePlayerData)
		{
			LogError("Error syncinc profile after creating the boss");
			return current;
		}

		private void SetupRandomColors(WorldBossTeamData wbtData)
		{
			ABHLocaService locaService = DIContainerInfrastructure.GetLocaService();
			int num = UnityEngine.Random.Range(0, 5);
			string text = locaService.Tr("worldboss_teamname_" + num.ToString("00") + "_name");
			text = text.Replace("{value_1}", UnityEngine.Random.Range(0, 100).ToString("000"));
			wbtData.TeamColor = num;
			wbtData.NameId = text;
			LogDebug("[EventSystemService][BossSystem] Set up team color " + num + " and teamname: " + text);
		}

		public void HandleBossDeath(uint timeOfDeath)
		{
			ABH.Shared.Models.PlayerData data = DIContainerInfrastructure.GetCurrentPlayer().Data;
			data.WorldBoss.DeathCount++;
			data.BossStartTime = timeOfDeath;
			data.WorldBoss.NumberOfAttacks = 0;
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}

		public void HandleBossBattleWon(PlayerGameData player)
		{
			EventManagerGameData currentEventManagerGameData = player.CurrentEventManagerGameData;
			if (!IsCurrentEventAvailable(player))
			{
				LogError("HandleBossBattle: no event available. This should not be possible!");
				return;
			}
			LogDebug("[EPIC SERVER] HandleBossBattleWon: Do we have a LB ID? " + !string.IsNullOrEmpty(currentEventManagerGameData.Data.LeaderboardId));
			if (DIContainerConfig.GetClientConfig().UseChimeraLeaderboards && !string.IsNullOrEmpty(currentEventManagerGameData.Data.LeaderboardId))
			{
				LogDebug("[EPIC SERVER] HandleBossBattleWon: Sending out trackbossdefeat for lbID: " + currentEventManagerGameData.Data.LeaderboardId);
				DIContainerLogic.BackendService.TrackBossDefeat(currentEventManagerGameData.Data.LeaderboardId, delegate(TrackBossDefeatResponseDto dto)
				{
					global::DebugLog.Log(GetType(), "[EPIC SERVER] TrackBossDefeat Callback received! ServerTime= " + dto.ServerTimeUtc);
				}, delegate(int errorcode)
				{
					global::DebugLog.Error(GetType(), "[EPIC SERVER] TrackBossDefeat request failed with error code " + errorcode);
				});
				return;
			}
			WorldEventBossData worldBoss = player.Data.WorldBoss;
			worldBoss.NumberOfAttacks++;
			if (worldBoss.DefeatedTimestamp == null)
			{
				worldBoss.DefeatedTimestamp = new List<uint>();
			}
			worldBoss.DefeatedTimestamp.Add(DIContainerLogic.GetTimingService().GetCurrentTimestamp());
			if (worldBoss.NumberOfAttacks >= DIContainerBalancing.Service.GetBalancingData<BossBalancingData>(worldBoss.NameId).AttacksNeeded)
			{
				DIContainerLogic.EventSystemService.HandleBossDeath(DIContainerLogic.GetTimingService().GetCurrentTimestamp());
			}
		}

		public bool IsBossOnCooldown()
		{
			EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
			if (currentEventManagerGameData == null || !currentEventManagerGameData.IsBossEvent)
			{
				return false;
			}
			if (currentEventManagerGameData.IsBossEvent)
			{
				long num = DIContainerInfrastructure.GetCurrentPlayer().Data.BossStartTime + currentEventManagerGameData.CurrentEventBoss.BalancingData.TimeToReactivate;
				return DIContainerLogic.GetTimingService().GetCurrentTimestamp() < num;
			}
			return false;
		}

		public string GetFormattedBossCooldown()
		{
			EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
			if (currentEventManagerGameData == null || !currentEventManagerGameData.IsBossEvent)
			{
				return "n/a";
			}
			if (currentEventManagerGameData.IsBossEvent)
			{
				uint num = DIContainerInfrastructure.GetCurrentPlayer().Data.BossStartTime + (uint)currentEventManagerGameData.CurrentEventBoss.BalancingData.TimeToReactivate;
				int num2 = (int)(num - DIContainerLogic.GetTimingService().GetCurrentTimestamp());
				int num3 = num2 % 3600 / 60;
				int num4 = num2 % 60;
				return num3.ToString("D2") + ":" + num4.ToString("D2");
			}
			return "n/a";
		}

		private void MatchmakingStrategyLeaderboard(PlayerGameData player, EventManagerGameData pvpManagerGameData)
		{
			string text = pvpManagerGameData.Data.NameId + "_matchmaking";
			LogDebug("[EventSystemService] MatchmakingStrategyLeaderboard: Establish Leaderboard matchmaking on table: " + text + " with maximum " + pvpManagerGameData.Balancing.MaximumMatchmakingPlayers + " and a offset of " + -((int)pvpManagerGameData.Balancing.MaximumMatchmakingPlayers / 2));
			DIContainerInfrastructure.ScoringService.Matchmake(text, 0, pvpManagerGameData.Balancing.MaximumMatchmakingPlayers, delegate(List<Leaderboard.Result> scores)
			{
				OnSuccessfullMatched(player, scores);
			}, OnErrorMatchmaking);
		}

		private Dictionary<string, object> GetOfflineMatchmakingAttributes(PlayerGameData player, string pvpTurnManagerId)
		{
			int level = player.Data.Level;
			LogDebug("[EventSystemService] GetOfflineMatchmakingAttributes: Level Param: " + level);
			int num = (int)(TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours + 13.0);
			LogDebug("[EventSystemService] GetOfflineMatchmakingAttributes: Timezone Indicator: " + num);
			int num2 = (int)((DIContainerLogic.GetDeviceTimingService().GetPresentTime() - new DateTime(2014, 1, 1)).TotalDays + (double)player.Data.ActivityIndicator);
			LogDebug("[EventSystemService] GetOfflineMatchmakingAttributes: Activity Indicator: " + num2);
			return AddOfflineMatchmakingAttributes(pvpTurnManagerId, level, num, num2);
		}

		private Dictionary<string, object> AddOfflineMatchmakingAttributes(string eventManagerId, int levelParameter, int timeZoneIndicator, int activity)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("timezone", timeZoneIndicator);
			dictionary.Add("activity", activity);
			dictionary.Add("variance", UnityEngine.Random.Range(0, 1000));
			dictionary.Add("level", levelParameter);
			return dictionary;
		}

		private void OnErrorMatchmaking(Leaderboard.ErrorCode errorCode)
		{
			if (errorCode == Leaderboard.ErrorCode.ErrorNoSuchLevel)
			{
				LogError("No such level while matching with other players, trying matching offline users");
				MatchmakingStrategyOffline(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData, true);
			}
			else
			{
				LogError("Error matching with other players: " + errorCode);
			}
		}

		private void OnSuccessfullMatched(PlayerGameData player, List<Leaderboard.Result> scores)
		{
			LogDebug("[EventSystemService] OnSuccessfullMatched: Succesfully matched to other players count: " + scores.Count);
			EventManagerGameData currentEventManagerGameData = player.CurrentEventManagerGameData;
			if (currentEventManagerGameData.Data.CurrentOpponents == null)
			{
				currentEventManagerGameData.Data.CurrentOpponents = new List<string>();
			}
			foreach (Leaderboard.Result score in scores)
			{
				LogDebug("[EventSystemService] OnSuccessfullMatched: Matched Player Score: " + score.GetScore().GetAccountId() + "with score: " + score.GetScore().GetPoints());
			}
			for (int i = 0; i < scores.Count; i++)
			{
				if (currentEventManagerGameData.Data.CurrentOpponents.Count >= currentEventManagerGameData.Balancing.MaximumMatchmakingPlayers)
				{
					break;
				}
				if (!(scores[i].GetScore().GetAccountId() == DIContainerInfrastructure.IdentityService.SharedId) && !currentEventManagerGameData.Data.CurrentOpponents.Contains(scores[i].GetScore().GetAccountId()))
				{
					currentEventManagerGameData.Data.CurrentOpponents.Add(scores[i].GetScore().GetAccountId());
					LogDebug("[EventSystemService] OnSuccessfullMatched: Added new Player Score: " + scores[i].GetScore().GetAccountId() + "with score: " + scores[i].GetScore().GetPoints());
					if (!currentEventManagerGameData.ScoresByPlayer.ContainsKey(scores[i].GetScore().GetAccountId()))
					{
						currentEventManagerGameData.ScoresByPlayer.Add(scores[i].GetScore().GetAccountId(), new Leaderboard.Score(currentEventManagerGameData.Balancing.NameId, scores[i].GetScore().GetAccountId()));
					}
				}
			}
			GetLeaderboardScores(player, currentEventManagerGameData.Data.CurrentOpponents, currentEventManagerGameData);
		}

		public void GetLeaderboardScores(PlayerGameData player, List<string> ids, EventManagerGameData eventManagerGameData, Action successUiCallback = null, Action<int> errorUiCallback = null)
		{
			LogDebug("[EPIC SERVER] GetLeaderboardScores!");
			List<string> list = ids ?? new List<string>();
			if (eventManagerGameData == null || list == null)
			{
				return;
			}
			LogDebug("GetLeaderboardScores: Start fetch scores:  Number of IDs = " + list.Count);
			RemoveCheatingEventScoreFix();
			DIContainerInfrastructure.RemoteStorageService.GetPublicPlayerDatas(list.ToArray(), delegate(Dictionary<string, PublicPlayerData> publicPlayers)
			{
				OnSuccessfullGotPublicPlayers(player, publicPlayers);
			}, OnErrorGetPublicPlayers);
			DIContainerInfrastructure.ScoringService.FetchScores(list.ToArray(), eventManagerGameData.Balancing.NameId, delegate(Dictionary<string, int> scores)
			{
				OnSuccessfullFetchedScore(player, scores);
				if (successUiCallback != null)
				{
					successUiCallback();
				}
			}, delegate(int errorCode)
			{
				OnErrorFetchingScores(errorCode);
				if (errorUiCallback != null)
				{
					errorUiCallback(errorCode);
				}
			});
		}

		private void OnSuccessfullGotPublicPlayers(PlayerGameData player, Dictionary<string, PublicPlayerData> playerDatas)
		{
			LogDebug("OnSuccessfullGotPublicPlayers: Succesfully got profiles of other players");
			player.CurrentEventManagerGameData.UpdateOpponents(playerDatas);
		}

		private void OnErrorGetPublicPlayers(string error)
		{
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (currentPlayer.CurrentEventManagerGameData.Data.CurrentOpponents == null)
			{
				currentPlayer.CurrentEventManagerGameData.Data.CurrentOpponents = new List<string>();
			}
			if (currentPlayer.CurrentEventManagerGameData.Data.CurrentOpponents.Count < 1)
			{
				LogDebug("[EventSystemService] OnErrorGetPublicPlayers: Currently no opponents found, matchmaking");
				if (!currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScoreSubmitted)
				{
					SubmitMatchmakingScore(currentPlayer, currentPlayer.CurrentEventManagerGameData.Data.NameId);
				}
				else
				{
					MatchmakingStrategyLeaderboard(currentPlayer, currentPlayer.CurrentEventManagerGameData);
				}
				if (currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore != 0L)
				{
					if (currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore.ToString().Length >= 9)
					{
						OldEventScore = currentPlayer.CurrentEventManagerGameData.Data.CurrentScore ^ (Convert.ToUInt32(currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore.ToString().Substring(0, 9)) + 195225786);
						ServerEventScore = currentPlayer.CurrentEventManagerGameData.Data.CurrentScore ^ (Convert.ToInt64(currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore.ToString().Substring(0, 8)) + 1856747893);
					}
					else
					{
						OldEventScore = currentPlayer.CurrentEventManagerGameData.Data.CurrentScore ^ (Convert.ToUInt32(currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore) + 195225786);
						ServerEventScore = currentPlayer.CurrentEventManagerGameData.Data.CurrentScore ^ (Convert.ToInt64(currentPlayer.CurrentEventManagerGameData.Data.MatchmakingScore) + 1856747893);
					}
				}
			}
			else
			{
				LogError("[EventSystemService] OnErrorGetPublicPlayers: Error getting profiles of other players: " + error);
			}
		}

		private void OnSuccessfullFetchedScore(PlayerGameData player, Dictionary<string, int> scores)
		{
			// LogDebug("Successful got scores of other players");
			// // List<Leaderboard.Result> list = new List<Leaderboard.Result>(scores.Count);
			// // foreach (KeyValuePair<string, int> score2 in scores)
			// // {
			// // 	Leaderboard.Score score = new Leaderboard.Score(string.Empty, score2.Key);
			// // 	score.SetPoints(score2.Value);
			// // 	Leaderboard.Result item = new Leaderboard.Result(0L, score);
			// // 	list.Add(item);
			// // }
			// LogDebug(string.Concat("EventSystemService: This mess is gonna make me throw up! Results for scorefetching in Hatch-format = ", list, " and they count ", list.Count));
			// player.CurrentEventManagerGameData.UpdateOpponentScores(list);
			if (IsWaitingForConfirmation(player.CurrentEventManagerGameData) && !player.CurrentEventManagerGameData.IsResultValid)
			{
				LogDebug("Event is over and valid to show result!");
				SetEventResultValid(player, player.CurrentEventManagerGameData);
				player.RegisterShowEventResult(player.CurrentEventManagerGameData);
			}
		}

		private void OnErrorFetchingScores(int errorCode)
		{
			LogError("Error getting scores of other players: " + errorCode);
			if (IsWaitingForConfirmation(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData) && !DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.IsResultValid)
			{
				LogDebug("Event is over and valid to show result!");
				SetEventResultValid(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData);
				DIContainerInfrastructure.GetCurrentPlayer().RegisterShowEventResult(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData);
			}
		}

		public bool IsResultRerollPossible(EventManagerGameData eventManagerGameData, InventoryGameData inventoryGameData)
		{
			return eventManagerGameData.EventBalancing.RerollResultRequirement == null || (float)DIContainerLogic.InventoryService.GetItemValue(inventoryGameData, eventManagerGameData.EventBalancing.RerollResultRequirement.NameId) >= eventManagerGameData.EventBalancing.RerollResultRequirement.Value;
		}

		public bool IsChestRerollPossible(InventoryGameData inventory)
		{
			WorldBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island");
			Requirement rerollChestRequirement = balancingData.RerollChestRequirement;
			return rerollChestRequirement == null || (float)DIContainerLogic.InventoryService.GetItemValue(inventory, rerollChestRequirement.NameId) >= rerollChestRequirement.Value;
		}

		public bool ExecuteResultRerollCost(EventManagerGameData eventManagerGameData, InventoryGameData inventoryGameData)
		{
			return eventManagerGameData.EventBalancing.RerollResultRequirement == null || DIContainerLogic.InventoryService.RemoveItem(inventoryGameData, eventManagerGameData.EventBalancing.RerollResultRequirement.NameId, (int)eventManagerGameData.EventBalancing.RerollResultRequirement.Value, "event_reroll");
		}

		public void RerollEventResultLoot(EventManagerGameData eventManagerGameData, PlayerGameData playerGameData)
		{
			LootTableBalancingData balancing = null;
			if (DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(eventManagerGameData.EventBalancing.EventRewardLootTableWheel.Keys.FirstOrDefault(), out balancing))
			{
				if (balancing.LootTableEntries.Count != 8)
				{
					LogError("Wheel LootTable for Battles does not contains 8 entrys instead it has " + balancing.LootTableEntries.Count);
					return;
				}
				int wheelIndex = eventManagerGameData.Data.CachedRolledResultWheelIndex;
				eventManagerGameData.RolledResultLoot = DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { { balancing.NameId, 1 } }, playerGameData.Data.Level, eventManagerGameData.ResultStars, ref wheelIndex);
				eventManagerGameData.Data.CachedRolledResultWheelIndex = wheelIndex;
				eventManagerGameData.RolledResultLoot = CheckForMasteryReplacement(eventManagerGameData.RolledResultLoot);
			}
			else
			{
				LogError("No Wheel LootTable set for battle ");
			}
		}

		public Requirement GetRerollRequirement(EventBalancingData eventBalancingData)
		{
			return eventBalancingData.RerollResultRequirement;
		}

		public bool HasMaximumEnergy(PlayerGameData playerGameData)
		{
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "event_energy", out data) && data.ItemBalancing.NameId == "event_energy" && DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ItemMaxCaps.ContainsKey(data.ItemBalancing.NameId) && data.ItemValue >= DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ItemMaxCaps["event_energy"] + DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, data.ItemBalancing.NameId + "_cap_extension"))
			{
				return true;
			}
			return false;
		}

		public void TravelToMiniCampaignHotspot(PlayerGameData playerGameData, HotspotGameData target)
		{
			playerGameData.CurrentEventManagerGameData.CurrentMiniCampaign.CurrentHotspotGameData = target;
			playerGameData.SavePlayerData();
		}

		public bool ForceResolveEvent()
		{
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			EventManagerGameData currentEventManagerGameData = currentPlayer.CurrentEventManagerGameData;
			if (currentEventManagerGameData == null)
			{
				return false;
			}
			if (IsEventOverNow(currentEventManagerGameData) && currentEventManagerGameData.Data.CurrentState < EventManagerState.Finished)
			{
				FinishCurrentEvent(currentPlayer.InventoryGameData, currentEventManagerGameData);
				return true;
			}
			return false;
		}

		public void CheckoutClicked(EventManagerGameData eMgr, string origin = null)
		{
			if (eMgr != null)
			{
				if (!eMgr.IsAssetValid) return;
				EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
				if (eMgr != currentEventManagerGameData && IsWaitingForConfirmation(currentEventManagerGameData))
				{
					DIContainerInfrastructure.LocationStateMgr.ShowEventResultPopup();
				}
				else if (IsEventRunning(eMgr.Balancing))
				{
					DIContainerInfrastructure.LocationStateMgr.ShowEventDetailScreen(eMgr);
				}
				else if (IsEventTeasing(eMgr.Balancing))
				{
					DIContainerInfrastructure.LocationStateMgr.ShowEventPreviewScreen(eMgr, false, origin);
				}
				else if (IsWaitingForConfirmation(eMgr))
				{
					DIContainerInfrastructure.LocationStateMgr.ShowEventResultPopup();
				}
			}
		}

		public bool AllowCheckout()
		{
			EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
			if (currentEventManagerGameData != null)
			{
				if (DIContainerLogic.EventSystemService.IsEventRunning(currentEventManagerGameData.Balancing))
				{
					return true;
				}
				if (DIContainerLogic.EventSystemService.IsEventTeasing(currentEventManagerGameData.Balancing))
				{
					return true;
				}
				if (DIContainerLogic.EventSystemService.IsWaitingForConfirmation(currentEventManagerGameData))
				{
					return true;
				}
			}
			return false;
		}

		public EventCampaignRewardStatus GetCurrentCollectionRewardStatus()
		{
			EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
			if (currentEventManagerGameData.IsCampaignEvent && IsCollectionGroupAvailable() && currentEventManagerGameData.CurrentMiniCampaign != null)
			{
				return currentEventManagerGameData.CurrentMiniCampaign.Data.RewardStatus;
			}
			if (currentEventManagerGameData.IsBossEvent && IsCollectionGroupAvailable() && DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss != null)
			{
				return DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss.RewardStatus;
			}
			return EventCampaignRewardStatus.locked;
		}

		public void GetEventBossDefeatLog(EventManagerGameData currentEventManagerGameData, Action<RESTResultEnum> uiCallback = null)
		{
			if (string.IsNullOrEmpty(currentEventManagerGameData.Data.LeaderboardId) || !currentEventManagerGameData.IsBossEvent || !IsEventRunning(currentEventManagerGameData))
			{
				return;
			}
			DIContainerLogic.BackendService.GetBossDefeatLog(currentEventManagerGameData.Data.LeaderboardId, delegate(GetBossDefeatLogResponseDto response)
			{
				OnGetBossDefeatLogSuccess(response);
				if (uiCallback != null)
				{
					uiCallback(response.Result);
				}
			}, delegate(int errorcode)
			{
				OnGetBossDefeatLogError(errorcode);
				if (uiCallback != null)
				{
					uiCallback((RESTResultEnum)errorcode);
				}
			});
		}

		private void OnGetBossDefeatLogSuccess(GetBossDefeatLogResponseDto response)
		{
			WorldEventBossData worldBoss = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss;
			if (worldBoss == null || worldBoss.OwnTeamId == 0 || response.BossDefeatLog == null)
			{
				global::DebugLog.Warn(GetType(), string.Concat("[EPIC SERVER] OnGetBossDefeatLogSucces: One of these is null: bossdata = ", worldBoss, " ownTeamId = ", worldBoss.OwnTeamId, " defeatLog = ", response.BossDefeatLog));
				return;
			}
			WorldBossTeamData worldBossTeamData = ((worldBoss.OwnTeamId != 1) ? worldBoss.Team2 : worldBoss.Team1);
			if (worldBossTeamData.TeamPlayerIds == null)
			{
				global::DebugLog.Error(GetType(), "[EPIC SERVER] OnGetBossDefeatLogSuccess: NO TEAM PLAYER IDS AVAILABLE!");
				return;
			}
			int numberOfAttacks = worldBoss.NumberOfAttacks;
			for (int i = numberOfAttacks; i < response.BossDefeatLog.Count; i++)
			{
				KeyValuePair<string, uint> keyValuePair = response.BossDefeatLog[i];
				if ((DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").OneWorldBoss || worldBossTeamData.TeamPlayerIds.Contains(keyValuePair.Key)) && keyValuePair.Value >= worldBossTeamData.LastProcessedBossDefeat)
				{
					WorldBossWasAttackedLogic(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		public void WorldBossWasAttackedLogic(string playerId, uint attackStamp)
		{
			WorldEventBossData worldBoss = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss;
			WorldBossTeamData worldBossTeamData = ((worldBoss.OwnTeamId != 1) ? worldBoss.Team2 : worldBoss.Team1);
			worldBossTeamData.LastProcessedBossDefeat = attackStamp;
			if (worldBoss.DefeatsToProcess == null)
			{
				worldBoss.DefeatsToProcess = new List<KeyValuePair<string, uint>>();
			}
			worldBoss.DefeatsToProcess.Add(new KeyValuePair<string, uint>(playerId, attackStamp));
		}

		public bool ProcessPendingAttack()
		{
			WorldEventBossData worldBoss = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss;
			if (worldBoss == null || (worldBoss.DefeatsToProcess != null && worldBoss.DefeatsToProcess.Count == 0))
			{
				return false;
			}
			worldBoss.NumberOfAttacks++;
			KeyValuePair<string, uint> keyValuePair = worldBoss.DefeatsToProcess[0];
			uint value = keyValuePair.Value;
			worldBoss.DefeatsToProcess.RemoveAt(0);
			int attacksNeeded = DIContainerBalancing.Service.GetBalancingData<BossBalancingData>(worldBoss.NameId).AttacksNeeded;
			if (worldBoss.NumberOfAttacks % attacksNeeded == 0)
			{
				global::DebugLog.Log("[EPIC SERVER] WorldBossWasAttackedLogic: Boss was DESTROYED at: " + value);
				HandleBossDeath(value);
			}
			return true;
		}

		private void OnGetBossDefeatLogError(int errorCode)
		{
			global::DebugLog.Error(GetType(), "GetBossDefeatLog failed with errorcode = " + errorCode);
		}

		public void PullLeaderboardUpdate(EventManagerGameData evtManager, Action successUiCallback = null, Action<int> errorUiCallback = null)
		{
			if (DIContainerConfig.GetClientConfig().UseChimeraLeaderboards && !string.IsNullOrEmpty(evtManager.Data.LeaderboardId))
			{
				DIContainerLogic.BackendService.GetEventLeaderboard(evtManager.Data.LeaderboardId, delegate(GetLeaderboardResponseDto response)
				{
					UpdateEventLeaderboard(evtManager.Data.LeaderboardId, response.Leaderboard, successUiCallback, errorUiCallback);
				}, delegate(int errorCode)
				{
					LogDebug("GetEventLeaderboard for id " + evtManager.Data.LeaderboardId + " failed with errorCode = " + errorCode);
					PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
					if (currentPlayer == null || currentPlayer.CurrentEventManagerGameData == null || currentPlayer.CurrentEventManagerGameData.Data.CurrentOpponents == null)
					{
						if (errorUiCallback != null)
						{
							errorUiCallback(errorCode);
						}
					}
					else
					{
						GetLeaderboardScores(currentPlayer, currentPlayer.CurrentEventManagerGameData.Data.CurrentOpponents, currentPlayer.CurrentEventManagerGameData, successUiCallback, errorUiCallback);
					}
				});
			}
			else
			{
				global::DebugLog.Warn(GetType(), "PullLeaderboardUpdate: invalid eventManagerGameData LeaderboardId!");
				if (errorUiCallback != null)
				{
					errorUiCallback(-1);
				}
			}
		}

		public void UpdateCachedFallbackLoot(HotspotBalancingData hotspotBal)
		{
			if (hotspotBal == null)
			{
				LogDebug("UpdateCachedFallbackLoot: battle balancing is null!");
				return;
			}
			EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
			if (currentEventManagerGameData != null && currentEventManagerGameData.CurrentMiniCampaign != null)
			{
				CollectionGroupBalancingData collectionGroupBalancing = currentEventManagerGameData.CurrentMiniCampaign.CollectionGroupBalancing;
				if (hotspotBal.ZoneLocaIdent.Contains("easy"))
				{
					m_CachedFallBackLoot = collectionGroupBalancing.EasyBattleFallbackLoot;
					m_CachedSecondaryFallBackLoot = collectionGroupBalancing.EasyBattleSecondaryFallbackLoot;
				}
				else if (hotspotBal.ZoneLocaIdent.Contains("medium"))
				{
					m_CachedFallBackLoot = collectionGroupBalancing.MediumBattleFallbackLoot;
					m_CachedSecondaryFallBackLoot = collectionGroupBalancing.MediumBattleSecondaryFallbackLoot;
				}
				else if (hotspotBal.ZoneLocaIdent.Contains("hard"))
				{
					m_CachedFallBackLoot = collectionGroupBalancing.HardBattleFallbackLoot;
					m_CachedSecondaryFallBackLoot = collectionGroupBalancing.HardBattleSecondaryFallbackLoot;
				}
				else
				{
					LogDebug("No fallback for collections in hotspot " + hotspotBal.NameId + " found!");
				}
			}
		}

		public List<IInventoryItemGameData> GetAvailableEliteChestReward(PlayerGameData player)
		{
			List<IInventoryItemGameData> list = EliteChestSkinReward(player);
			if (list.Count == 0)
			{
				list = EliteChestMasteryReward(player);
			}
			if (list.Count == 0)
			{
				list = EliteChestCurrencyReward(player);
			}
			return list;
		}

		private List<IInventoryItemGameData> EliteChestSkinReward(PlayerGameData player)
		{
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			string text = "loot_elitechest_skins";
			LootTableBalancingData balancing = null;
			if (!DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(text, out balancing))
			{
				LogDebug("EliteChestSkinReward: No Balancing entry found for '" + text + "'");
				return list;
			}
			for (int i = 0; i < balancing.LootTableEntries.Count; i++)
			{
				LootTableEntry lootTableEntry = balancing.LootTableEntries[i];
				SkinItemGameData skinItemGameData = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, lootTableEntry.NameId, 1) as SkinItemGameData;
				if (skinItemGameData != null && player.OwnsClass(skinItemGameData.BalancingData.OriginalClass) && !player.OwnsClass(skinItemGameData.BalancingData.NameId))
				{
					list.Add(skinItemGameData);
				}
			}
			return list;
		}

		private List<IInventoryItemGameData> EliteChestMasteryReward(PlayerGameData player)
		{
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			string text = "loot_elitechest_mastery";
			LootTableBalancingData balancing = null;
			if (!DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(text, out balancing))
			{
				LogDebug("EliteChestMasteryReward: No Balancing entry found for '" + text + "'");
				return list;
			}
			for (int i = 0; i < balancing.LootTableEntries.Count; i++)
			{
				LootTableEntry lootTableEntry = balancing.LootTableEntries[i];
				MasteryItemGameData masteryItemGameData = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(player.InventoryGameData, 1, 1, lootTableEntry.NameId, lootTableEntry.BaseValue) as MasteryItemGameData;
				if (masteryItemGameData != null && player.CanAddMastery(masteryItemGameData))
				{
					list.Add(masteryItemGameData);
				}
			}
			return list;
		}

		private List<IInventoryItemGameData> EliteChestCurrencyReward(PlayerGameData player)
		{
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			string text = "loot_elitechest_currency";
			LootTableBalancingData balancing = null;
			if (!DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(text, out balancing))
			{
				LogDebug("EliteChestCurrencyReward: No Balancing entry found for '" + text + "'");
				return list;
			}
			for (int i = 0; i < balancing.LootTableEntries.Count; i++)
			{
				LootTableEntry lootTableEntry = balancing.LootTableEntries[i];
				list.Add(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(player.InventoryGameData, 1, 1, lootTableEntry.NameId, lootTableEntry.BaseValue));
			}
			return list;
		}

		public bool IsChestCollectionReward(EventManagerGameData mgr)
		{
			CollectionGroupBalancingData collectionBalancing = mgr.GetCollectionBalancing();
			if (collectionBalancing != null)
			{
				foreach (string key in collectionBalancing.Reward.Keys)
				{
					if (key.Contains("elite_chest"))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool ConfirmEliteChestReward(PlayerGameData player)
		{
			if (player.Data.CachedChestRewardItem == null || player.RolledChestReward == null)
			{
				return false;
			}
			IInventoryItemGameData rolledChestReward = player.RolledChestReward;
			DIContainerLogic.InventoryService.AddItem(player.InventoryGameData, rolledChestReward.ItemData.Level, rolledChestReward.ItemData.Quality, rolledChestReward.ItemBalancing.NameId, rolledChestReward.ItemValue, "Elite_Chest_Reward");
			player.CurrentEventManagerGameData.Data.ConfirmedChestLootId = rolledChestReward.ItemBalancing.NameId;
			player.Data.CachedChestRewardItem = null;
			player.RolledChestReward = null;
			if (player.CurrentEventManagerGameData != null && IsChestCollectionReward(player.CurrentEventManagerGameData))
			{
				if (player.CurrentEventManagerGameData.IsBossEvent && player.Data.WorldBoss != null)
				{
					player.Data.WorldBoss.RewardStatus = EventCampaignRewardStatus.chest_claimed;
				}
				else if (player.CurrentEventManagerGameData.IsCampaignEvent && player.CurrentEventManagerGameData.CurrentMiniCampaign != null)
				{
					player.CurrentEventManagerGameData.CurrentMiniCampaign.Data.RewardStatus = EventCampaignRewardStatus.chest_claimed;
				}
			}
			player.SavePlayerData();
			return true;
		}

		internal bool IsChestRewardPending(EventManagerGameData eventManagerGameData)
		{
			if (eventManagerGameData != null && IsChestCollectionReward(eventManagerGameData))
			{
				EventCampaignRewardStatus eventCampaignRewardStatus = ((!eventManagerGameData.IsBossEvent) ? eventManagerGameData.CurrentMiniCampaign.Data.RewardStatus : DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss.RewardStatus);
				return eventCampaignRewardStatus >= EventCampaignRewardStatus.unlocked_new && eventCampaignRewardStatus < EventCampaignRewardStatus.chest_claimed;
			}
			return false;
		}

		public void RemoveCheatingEventScoreFix()
		{
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (currentPlayer == null || currentPlayer.Data == null || currentPlayer.Data.CurrentEventManager == null || currentPlayer.Data.CurrentEventManager.MatchmakingScore <= 0 || currentPlayer.CurrentEventManagerGameData == null || currentPlayer.CurrentEventManagerGameData.Data == null || !currentPlayer.CurrentEventManagerGameData.IsQualifiedForLeaderboard || !string.IsNullOrEmpty(currentPlayer.Data.CurrentEventManager.LeaderboardId))
			{
				return;
			}
			uint currentScore = currentPlayer.Data.CurrentEventManager.CurrentScore;
			try
			{
				uint num;
				uint num2;
				long num3;
				try
				{
					if (currentPlayer.Data.CurrentEventManager.MatchmakingScore.ToString().Length >= 9)
					{
						num = Convert.ToUInt32(currentPlayer.Data.CurrentEventManager.MatchmakingScore.ToString().Substring(0, 9)) + 195225786;
						num2 = OldEventScore ^ num;
						num3 = ServerEventScore ^ (Convert.ToInt64(currentPlayer.Data.CurrentEventManager.MatchmakingScore.ToString().Substring(0, 8)) + 1856747893);
					}
					else
					{
						num = Convert.ToUInt32(currentPlayer.Data.CurrentEventManager.MatchmakingScore) + 195225786;
						num2 = OldEventScore ^ num;
						num3 = ServerEventScore ^ (Convert.ToInt64(currentPlayer.Data.CurrentEventManager.MatchmakingScore) + 1856747893);
					}
				}
				catch
				{
					return;
				}
				if (OldEventScore != 0 && currentScore != num2)
				{
					currentPlayer.Data.CurrentEventManager.CurrentScore = num2;
					currentPlayer.CurrentEventManagerGameData.Data.CurrentScore = num2;
					OldEventScore = currentPlayer.Data.CurrentEventManager.CurrentScore ^ num;
				}
				else if (num3 <= uint.MaxValue && num3 >= 0 && ServerEventScore != 0L && currentScore != Convert.ToUInt32(num3))
				{
					currentPlayer.Data.CurrentEventManager.CurrentScore = Convert.ToUInt32(num3);
					currentPlayer.CurrentEventManagerGameData.Data.CurrentScore = Convert.ToUInt32(num3);
					OldEventScore = currentPlayer.Data.CurrentEventManager.CurrentScore ^ num;
				}
			}
			catch
			{
				if (currentPlayer.Data.CurrentEventManager.CurrentScore != currentScore)
				{
					currentPlayer.Data.CurrentEventManager.CurrentScore = currentScore;
					currentPlayer.CurrentEventManagerGameData.Data.CurrentScore = currentScore;
				}
				if (currentPlayer.Data.CurrentEventManager.CurrentScore < 0)
				{
					currentPlayer.Data.CurrentEventManager.CurrentScore = 0u;
					currentPlayer.CurrentEventManagerGameData.Data.CurrentScore = 0u;
				}
			}
		}

		public List<Leaderboard.Result> GetTeamScores(List<Leaderboard.Result> scores)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.IsBossEvent)
			{
				return scores;
			}
			uint currentScore = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.CurrentScore;
			WorldEventBossData worldBoss = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss;
			Leaderboard.Result item = null;
			for (int i = 0; i < scores.Count; i++)
			{
				if (scores[i].GetScore().GetAccountId() == DIContainerInfrastructure.IdentityService.SharedId)
				{
					item = scores[i];
					break;
				}
			}
			int num = scores.Count;
			int num2 = scores.Count;
			if (worldBoss.OwnTeamId != 1)
			{
				num -= 15;
			}
			else if (worldBoss.OwnTeamId != 2)
			{
				num2 -= 15;
			}
			if (scores.Count > 71 && (ulong)currentScore < (ulong)scores[57].GetScore().GetPoints())
			{
				worldBoss.Team1.TeamColor = 4;
				string text = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBoss.Team1.TeamColor.ToString("00") + "_name");
				text = text.Replace("{value_1}", Math.Max(num, 0).ToString());
				worldBoss.Team1.NameId = text;
				worldBoss.Team2.TeamColor = 0;
				string text2 = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBoss.Team2.TeamColor.ToString("00") + "_name");
				text2 = text2.Replace("{value_1}", Math.Max(num2, 0).ToString());
				worldBoss.Team2.NameId = text2;
				if (scores.Count > 16)
				{
					scores.RemoveRange(0, 58);
					scores.Add(item);
				}
				return scores;
			}
			if (scores.Count > 57 && (ulong)currentScore < (ulong)scores[43].GetScore().GetPoints())
			{
				worldBoss.Team1.TeamColor = 3;
				string text3 = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBoss.Team1.TeamColor.ToString("00") + "_name");
				text3 = text3.Replace("{value_1}", Math.Max(num, 0).ToString());
				worldBoss.Team1.NameId = text3;
				worldBoss.Team2.TeamColor = 4;
				string text4 = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBoss.Team2.TeamColor.ToString("00") + "_name");
				text4 = text4.Replace("{value_1}", Math.Max(num2, 0).ToString());
				worldBoss.Team2.NameId = text4;
				if (scores.Count > 16)
				{
					scores.RemoveRange(0, 44);
					if (scores.Count > 16)
					{
						scores.RemoveRange(15, scores.Count - 1);
					}
					scores.Add(item);
				}
				return scores;
			}
			if (scores.Count > 43 && (ulong)currentScore < (ulong)scores[29].GetScore().GetPoints())
			{
				worldBoss.Team1.TeamColor = 2;
				string text5 = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBoss.Team1.TeamColor.ToString("00") + "_name");
				text5 = text5.Replace("{value_1}", Math.Max(num, 0).ToString());
				worldBoss.Team1.NameId = text5;
				worldBoss.Team2.TeamColor = 3;
				string text6 = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBoss.Team2.TeamColor.ToString("00") + "_name");
				text6 = text6.Replace("{value_1}", Math.Max(num2, 0).ToString());
				worldBoss.Team2.NameId = text6;
				if (scores.Count > 16)
				{
					scores.RemoveRange(0, 30);
					if (scores.Count > 16)
					{
						scores.RemoveRange(15, scores.Count - 1);
					}
					scores.Add(item);
				}
				return scores;
			}
			if (scores.Count > 29 && (ulong)currentScore < (ulong)scores[15].GetScore().GetPoints())
			{
				worldBoss.Team1.TeamColor = 1;
				string text7 = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBoss.Team1.TeamColor.ToString("00") + "_name");
				text7 = text7.Replace("{value_1}", Math.Max(num, 0).ToString());
				worldBoss.Team1.NameId = text7;
				worldBoss.Team2.TeamColor = 2;
				string text8 = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBoss.Team2.TeamColor.ToString("00") + "_name");
				text8 = text8.Replace("{value_1}", Math.Max(num2, 0).ToString());
				worldBoss.Team2.NameId = text8;
				if (scores.Count > 16)
				{
					scores.RemoveRange(0, 15);
					if (scores.Count > 16)
					{
						scores.RemoveRange(15, scores.Count - 1);
					}
					scores.Add(item);
				}
				return scores;
			}
			worldBoss.Team1.TeamColor = 0;
			string text9 = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBoss.Team1.TeamColor.ToString("00") + "_name");
			text9 = text9.Replace("{value_1}", Math.Max(num, 0).ToString());
			worldBoss.Team1.NameId = text9;
			worldBoss.Team2.TeamColor = 1;
			string text10 = DIContainerInfrastructure.GetLocaService().Tr("worldboss_teamname_" + worldBoss.Team2.TeamColor.ToString("00") + "_name");
			text10 = text10.Replace("{value_1}", Math.Max(num2, 0).ToString());
			worldBoss.Team2.NameId = text10;
			return scores;
		}

		public void SubmitOfflineMatchmakingAttributes(PlayerGameData player, string pvpTurnManagerId, bool asFallback)
		{
			Dictionary<string, object> offlineMatchmakingAttributes = GetOfflineMatchmakingAttributes(player, pvpTurnManagerId);
			DIContainerInfrastructure.MatchmakingService.SetOfflineAttributes(offlineMatchmakingAttributes, delegate(OfflineMatchmaker.ResultCode result)
			{
				if (result == OfflineMatchmaker.ResultCode.Success && IsCurrentEventAvailable(player))
				{
					MatchmakingStrategyOffline(player, player.CurrentEventManagerGameData, asFallback);
				}
			});
		}
	}
}
