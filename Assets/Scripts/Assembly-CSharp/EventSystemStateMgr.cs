using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class EventSystemStateMgr : MonoBehaviourContainerBase
{
	private const string ChestRewardIdCampaign = "collection_elite_chest_campaign";

	private const string ChestRewardIdBoss = "collection_elite_chest_worldboss";

	private const string EventCampaignAtlasPrefix = "EventImage_Campaign_";

	private DateTime m_firstSuccesfullBalancingFetchTime;

	private float m_timeTillReloadOfBalancingInSeconds = 120f;

	private PlayerGameData m_currentPlayer;

	[SerializeField]
	private GenericAssetProvider m_AllEventsAssetProvider;

	private GameObject m_EventsAssetProvidersRoot;

	private Dictionary<string, GenericAssetProvider> m_LoadedEventAssetProviders;

	private Dictionary<string, EventManagerGameData> m_CachedEventManagers;

	public EventItemGameData lastRemovedEventItem;

	private bool m_eventFixed;

	public bool IsInitialized { get; private set; }

	public bool assetProviderLoading { get; set; }

	public EventManagerGameData GetCachedEventManager(string eventNameId)
	{
		if (m_CachedEventManagers == null)
		{
			m_CachedEventManagers = new Dictionary<string, EventManagerGameData>();
		}
		if (!m_CachedEventManagers.ContainsKey(eventNameId))
		{
			m_CachedEventManagers.Add(eventNameId, new EventManagerGameData().CreateNewInstance(eventNameId));
		}
		return m_CachedEventManagers[eventNameId];
	}

	private IEnumerator Start()
	{
		while (!DIContainerInfrastructure.GetCoreStateMgr().m_isInitialized)
		{
			yield return new WaitForEndOfFrame();
		}
		IInventoryItemGameData unlockItem = null;
		m_LoadedEventAssetProviders = new Dictionary<string, GenericAssetProvider>();
		DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.StoryItemGained -= OnStoryItemAdded;
		if (!DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_events", out unlockItem))
		{
			DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.StoryItemGained += OnStoryItemAdded;
			yield break;
		}
		if (unlockItem.ItemData.IsNew)
		{
			unlockItem.ItemData.IsNew = false;
			DIContainerInfrastructure.TutorialMgr.StartTutorial("tutorial_battle_rule");
		}
		DebugLog.Log("[EventSystemStateManager] Begin Load Event Balancing!");
		while (DIContainerBalancing.EventBalancingLoadingPending)
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerBalancing.GetEventBalancingDataPoviderAsynch(OnBalancingDataProviderReceived);
	}

	private IEnumerator DownloadEventAssets()
	{
		string locaFilename = DIContainerInfrastructure.GetTargetBuildGroup() + "_" + m_AllEventsAssetProvider.name.ToLower() + ".assetbundle";
		if (DIContainerInfrastructure.GetAssetsService().NeedToDownloadAsset(locaFilename))
		{
			assetProviderLoading = true;
			DIContainerInfrastructure.GetAssetsService().Load(locaFilename, delegate(string result)
			{
				if (result != null)
				{
					DebugLog.Log("[ContentLoader] EventAssetProvider: " + m_AllEventsAssetProvider.name + " succesfully downloaded!");
					assetProviderLoading = false;
				}
				else
				{
					DebugLog.Error("[ContentLoader] EventAssetProvider: " + m_AllEventsAssetProvider.name + " failed to downloaded!");
					assetProviderLoading = false;
				}
			}, delegate
			{
			}, delegate
			{
			});
		}
		while (assetProviderLoading)
		{
			yield return new WaitForEndOfFrame();
		}
		yield return StartCoroutine(m_AllEventsAssetProvider.InitializeCoroutine());
		if (!m_EventsAssetProvidersRoot)
		{
			m_EventsAssetProvidersRoot = m_AllEventsAssetProvider.InstantiateObject("EventAssetProvidersRoot", m_AllEventsAssetProvider.transform.parent, Vector3.zero, Quaternion.identity);
		}
	}

	private void OnStoryItemAdded(IInventoryItemGameData obj)
	{
		if (obj.ItemBalancing.NameId == "unlock_events")
		{
			StartCoroutine("Start");
		}
	}

	private void OnBalancingDataProviderReceived(IBalancingDataLoaderService balancing)
	{
		DebugLog.Log("[EventSystemStateManager] Event Balancing loaded Begin initialize Event System!");
		InitializeEventSystem();
		StartCoroutine(DownloadEventAssets());
	}

	private void InitializeEventSystem()
	{
		m_firstSuccesfullBalancingFetchTime = DIContainerLogic.GetDeviceTimingService().GetPresentTime();
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		currentPlayer.GenerateEventManagerFromProfile();
		InvokeRepeating("UpdateEvents", 0.1f, 4f);
		if (!DIContainerConfig.GetClientConfig().UseChimeraLeaderboards)
		{
			InvokeRepeating("UpdateMatchmakingScores", 10f, 10f);
		}
		IsInitialized = true;
	}

	private void CheckBrokenBoss()
	{
		DebugLog.Error("DEPRECATED: CheckBrokenBoss");
	}

	public void ResetEventManager()
	{
		CancelInvoke("UpdateEvents");
		CancelInvoke("UpdateMatchmakingScores");
		IsInitialized = false;
		DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData = null;
		m_LoadedEventAssetProviders = new Dictionary<string, GenericAssetProvider>();
		m_CachedEventManagers = new Dictionary<string, EventManagerGameData>();
		StartCoroutine(Start());
	}

	public void UpdateEvents()
	{
		DIContainerLogic.GetServerOnlyTimingService().GetTrustedTimeEx(OnTrustedTimeReceivedAndUpdateEvents);
	}

	private void UpdateMatchmakingScores()
	{
		if (IsInitialized && DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && (DIContainerLogic.EventSystemService.IsEventRunning(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Balancing) || DIContainerLogic.EventSystemService.IsWaitingForConfirmation(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData)) && DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.MatchmakingScoreSubmitted)
		{
			DIContainerLogic.EventSystemService.UpdateMatchmakingScore(DIContainerInfrastructure.GetCurrentPlayer());
		}
	}

	private void OnTrustedTimeReceivedAndUpdateEvents(DateTime trustedDateTime)
	{
		if (DIContainerBalancing.EventBalancingService == null || (DIContainerInfrastructure.LocationStateMgr == null && IsInitialized))
		{
			return;
		}
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (!DIContainerLogic.EventSystemService.IsCurrentEventAvailable(currentPlayer))
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			EventManagerGameData currentEventManagerGameData = currentPlayer.CurrentEventManagerGameData;
			if (currentEventManagerGameData != null && !string.IsNullOrEmpty(currentEventManagerGameData.Data.NameId))
			{
				dictionary.Add("EventNameId", currentEventManagerGameData.Data.NameId);
			}
			else
			{
				dictionary.Add("EventNameId", "Unnamed");
			}
			dictionary.Add("CurrentScore", "-1");
			dictionary.Add("CurrentState", EventManagerState.Invalid.ToString());
			DIContainerLogic.EventSystemService.ClearEvent(currentPlayer, string.Empty);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("EventStateChanged", dictionary);
			OnCurrentEventUnavailable();
			return;
		}
		EventManagerGameData currentEventManagerGameData2 = currentPlayer.CurrentEventManagerGameData;
		if ((uint)currentEventManagerGameData2.CurrentEventManagerState < 1u && !m_eventFixed)
		{
			DIContainerLogic.EventSystemService.ClearEvent(currentPlayer, string.Empty);
			OnCurrentEventUnavailable();
			return;
		}
		if (!IsEventStateValid(currentEventManagerGameData2))
		{
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2.Add("EventNameId", currentEventManagerGameData2.Data.NameId);
			dictionary2.Add("CurrentScore", currentEventManagerGameData2.Data.CurrentScore.ToString("0"));
			dictionary2.Add("CurrentState", EventManagerState.Invalid.ToString());
			dictionary2.Add("MatchmakingScore", currentEventManagerGameData2.Data.MatchmakingScore.ToString("0"));
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("EventStateChanged", dictionary2);
			DIContainerLogic.EventSystemService.ClearEvent(currentPlayer, currentEventManagerGameData2.Balancing.NameId);
			if (DIContainerInfrastructure.LocationStateMgr != null)
			{
				EventSystemWorldMapStateMgr eventsWorldMapStateMgr = DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr;
				if (eventsWorldMapStateMgr != null)
				{
					eventsWorldMapStateMgr.Restart();
				}
			}
		}
		if (DIContainerLogic.EventSystemService.IsWaitingForConfirmation(currentEventManagerGameData2))
		{
			CheckAndLoadEventAssets(currentEventManagerGameData2);
			return;
		}
		EventManagerState currentStateForEventManager = GetCurrentStateForEventManager(currentEventManagerGameData2);
		DebugLog.Log(GetType(), string.Concat("UpdateEvents: current event state is ", currentEventManagerGameData2.CurrentEventManagerState, ". Expected state for current event is ", currentStateForEventManager));
		switch (currentStateForEventManager)
		{
		case EventManagerState.Teasing:
			DIContainerLogic.EventSystemService.TeaseEvent(currentEventManagerGameData2);
			CheckAndLoadEventAssets(currentEventManagerGameData2);
			break;
		case EventManagerState.Running:
			if (DIContainerLogic.EventSystemService.StartEvent(currentEventManagerGameData2) && (bool)DIContainerInfrastructure.LocationStateMgr && DIContainerInfrastructure.LocationStateMgr != null)
			{
				EventSystemWorldMapStateMgr eventsWorldMapStateMgr2 = DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr;
				if (eventsWorldMapStateMgr2 != null)
				{
					eventsWorldMapStateMgr2.Restart();
				}
			}
			CheckAndLoadEventAssets(currentEventManagerGameData2);
			break;
		case EventManagerState.Finished:
		case EventManagerState.FinishedWithoutPoints:
			DIContainerLogic.EventSystemService.FinishCurrentEvent(currentPlayer.InventoryGameData, currentEventManagerGameData2);
			CheckAndLoadEventAssets(currentEventManagerGameData2);
			break;
		case EventManagerState.FinishedAndResultIsValid:
		case EventManagerState.FinishedAndConfirmed:
			break;
		}
	}

	public bool CheckAndLoadEventAssets(EventManagerGameData eventManagerGameData)
	{
		if (eventManagerGameData != null && !eventManagerGameData.IsAssetValid)
		{
			StartCoroutine(LoadAndSetEventAssetBundle(eventManagerGameData));
			return true;
		}
		return false;
	}

	public EventManagerState GetCurrentStateForEventManager(EventManagerGameData currentEventManagerGameData)
	{
		if (DIContainerLogic.EventSystemService.IsEventOverNow(currentEventManagerGameData))
		{
			return EventManagerState.Finished;
		}
		if (DIContainerLogic.EventSystemService.IsEventOverNow(currentEventManagerGameData) && currentEventManagerGameData.Data.CurrentScore == 0)
		{
			return EventManagerState.FinishedWithoutPoints;
		}
		if (DIContainerLogic.EventSystemService.IsEventTeasing(currentEventManagerGameData.Balancing))
		{
			return EventManagerState.Teasing;
		}
		if (DIContainerLogic.EventSystemService.IsEventRunning(currentEventManagerGameData.Balancing))
		{
			return EventManagerState.Running;
		}
		DebugLog.Error(GetType(), "GetCurrentStateForEventManager: No valid state found! Event is neither teasing nor running or anything else.");
		return EventManagerState.Invalid;
	}

	private void OnCurrentEventUnavailable()
	{
		IList<EventManagerBalancingData> balancingDataList = DIContainerBalancing.EventBalancingService.GetBalancingDataList<EventManagerBalancingData>();
		if (balancingDataList == null)
		{
			DebugLog.Error(GetType(), "OnCurrentEventUnavailable: No EventBalancing found in EventManagerBalancingData");
			return;
		}
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		foreach (EventManagerBalancingData item in balancingDataList)
		{
			if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(currentPlayer) && (uint)currentPlayer.CurrentEventManagerGameData.CurrentEventManagerState > 1u)
			{
				break;
			}
			if (DIContainerLogic.EventSystemService.IsEventTeasing(item))
			{
				DebugLog.Log("OnCurrentEventUnavailable: event teasing = " + item.NameId);
				DIContainerLogic.EventSystemService.TeaseNewEvent(item, currentPlayer);
				CheckAndLoadEventAssets(currentPlayer.CurrentEventManagerGameData);
				m_eventFixed = true;
			}
			else
			{
				if (!DIContainerLogic.EventSystemService.IsEventRunning(item))
				{
					continue;
				}
				DIContainerLogic.EventSystemService.StartNewEvent(item, currentPlayer);
				if (DIContainerInfrastructure.LocationStateMgr != null)
				{
					EventSystemWorldMapStateMgr eventsWorldMapStateMgr = DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr;
					if (eventsWorldMapStateMgr != null)
					{
						eventsWorldMapStateMgr.Restart();
					}
				}
				CheckAndLoadEventAssets(currentPlayer.CurrentEventManagerGameData);
				break;
			}
		}
	}

	public bool IsEventStateValid(EventManagerGameData currentEventManagerGameData)
	{
		uint currentTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		switch (currentEventManagerGameData.CurrentEventManagerState)
		{
		case EventManagerState.Teasing:
			if (currentTimestamp < currentEventManagerGameData.Balancing.EventTeaserStartTimeStamp)
			{
				DebugLog.Error("[EventSystemStateMgr] Event is invalid - state = teasing. Removing it!");
				return false;
			}
			break;
		case EventManagerState.Running:
			if (currentTimestamp < currentEventManagerGameData.Balancing.EventStartTimeStamp)
			{
				DebugLog.Error("[EventSystemStateMgr] Event is invalid - state = running. Removing it!");
				return false;
			}
			break;
		case EventManagerState.Finished:
		case EventManagerState.FinishedWithoutPoints:
		case EventManagerState.FinishedAndResultIsValid:
		case EventManagerState.FinishedAndConfirmed:
			if (currentTimestamp < currentEventManagerGameData.Balancing.EventStartTimeStamp)
			{
				DebugLog.Error("[EventSystemStateMgr] Event is invalid - state = finished or higher. Removing it!");
				return false;
			}
			if (DIContainerLogic.EventSystemService.IsEventRunning(currentEventManagerGameData))
			{
				return true;
			}
			break;
		}
		return true;
	}

	private IEnumerator LoadAndSetEventAssetBundle(EventManagerGameData evtMgr)
	{
		if (m_LoadedEventAssetProviders.ContainsKey(evtMgr.EventBalancing.NameId))
		{
			evtMgr.IsAssetValid = true;
		}
		if (!m_EventsAssetProvidersRoot || !evtMgr.IsValid || evtMgr.IsAssetValid)
		{
			yield break;
		}
		evtMgr.IsAssetValid = false;
		if (m_EventsAssetProvidersRoot != null)
		{
			GenericAssetProvider currentAssetProvider = m_EventsAssetProvidersRoot.GetComponentsInChildren<GenericAssetProvider>().FirstOrDefault((GenericAssetProvider p) => p.name == evtMgr.EventBalancing.AssetBaseId + "AssetProvider");
			if ((bool)currentAssetProvider)
			{
				string locaFilename = DIContainerInfrastructure.GetTargetBuildGroup() + "_" + currentAssetProvider.name.ToLower() + ".assetbundle";
				if (assetProviderLoading)
				{
					yield break;
				}
				if (DIContainerInfrastructure.GetAssetsService().NeedToDownloadAsset(locaFilename))
				{
					assetProviderLoading = true;
					DIContainerInfrastructure.GetAssetsService().Load(locaFilename, delegate(string result)
					{
						if (result != null)
						{
							DebugLog.Log("[ContentLoader] EventAssetProvider: " + currentAssetProvider.name + " succesfully downloaded!");
							assetProviderLoading = false;
						}
						else
						{
							DebugLog.Error("[ContentLoader] EventAssetProvider: " + currentAssetProvider.name + " failed to downloaded!");
							assetProviderLoading = false;
						}
					}, delegate
					{
					}, delegate
					{
					});
				}
				while (assetProviderLoading)
				{
					yield return new WaitForEndOfFrame();
				}
				yield return StartCoroutine(currentAssetProvider.InitializeCoroutine());
			}
			else
			{
				DebugLog.Error("[EventSystemStateMgr] Could not load current EventAssetProvider: " + evtMgr.EventBalancing.AssetBaseId + "AssetProvider");
			}
			if (!m_LoadedEventAssetProviders.ContainsKey(evtMgr.EventBalancing.NameId))
			{
				m_LoadedEventAssetProviders.Add(evtMgr.EventBalancing.NameId, currentAssetProvider);
			}
			if (m_CachedEventManagers == null)
			{
				m_CachedEventManagers = new Dictionary<string, EventManagerGameData>();
			}
			m_CachedEventManagers[evtMgr.Balancing.NameId] = evtMgr;
		}
		if (m_LoadedEventAssetProviders[evtMgr.EventBalancing.NameId] != null)
		{
			evtMgr.IsAssetValid = true;
			DebugLog.Log("[EventSystemStateMgr] Current Event is Asset valid now!");
		}
	}

	public void RemoveEventItemFromLocation(string locationId)
	{
		List<IInventoryItemGameData> list = new List<IInventoryItemGameData>(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.EventBattleItem]);
		for (int i = 0; i < list.Count; i++)
		{
			IInventoryItemGameData inventoryItemGameData = list[i];
			EventItemGameData eventItemGameData = inventoryItemGameData as EventItemGameData;
			if (eventItemGameData != null && eventItemGameData.Data.PositionId == locationId)
			{
				DIContainerLogic.InventoryService.RemoveItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, inventoryItemGameData, inventoryItemGameData.ItemValue, "finished_event_battle", true);
				lastRemovedEventItem = eventItemGameData;
			}
		}
	}

	public void RemoveEventCollectibleFromLocation(string locationId)
	{
		List<IInventoryItemGameData> list = new List<IInventoryItemGameData>(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.EventCollectible]);
		if (list.Count == DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.MaxNumberOfCollectibles)
		{
			DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.LastCollectibleSpawnTime = DIContainerLogic.GetServerOnlyTimingService().GetCurrentTimestamp();
		}
		for (int i = 0; i < list.Count; i++)
		{
			IInventoryItemGameData inventoryItemGameData = list[i];
			EventItemGameData eventItemGameData = inventoryItemGameData as EventItemGameData;
			if (eventItemGameData != null && eventItemGameData.Data.PositionId == locationId)
			{
				DIContainerLogic.InventoryService.RemoveItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, inventoryItemGameData, inventoryItemGameData.ItemValue, "event_item_collected", true);
				lastRemovedEventItem = eventItemGameData;
			}
		}
	}

	public int GetNumberOfCampaignGateUnlocks()
	{
		EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		float num = DIContainerLogic.GetServerOnlyTimingService().GetCurrentTimestamp();
		float num2 = currentEventManagerGameData.Balancing.EventStartTimeStamp;
		float num3 = num - num2;
		int num4 = (int)(num3 / currentEventManagerGameData.EventBalancing.MiniCampaignUnlockDelay);
		IInventoryItemGameData data = null;
		if (!DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "event_gate_unlock", out data))
		{
			DIContainerLogic.InventoryService.AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "event_gate_unlock", Math.Max(num4, 0), "event_gate_open");
		}
		else
		{
			DIContainerLogic.InventoryService.SetItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "event_gate_unlock", Math.Max(num4, 0), "event_gate_open");
		}
		return num4;
	}

	public TimeSpan GetCampaignGateOpenTimeLeft(int gateIndex)
	{
		DateTime targetServerTime = DIContainerLogic.GetServerOnlyTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Balancing.EventStartTimeStamp).AddSeconds((float)gateIndex * DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.MiniCampaignUnlockDelay);
		return DIContainerLogic.GetServerOnlyTimingService().TimeLeftUntil(targetServerTime);
	}

	public void UpdateEventStars()
	{
		int num = 0;
		foreach (HotspotGameData value in DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentMiniCampaign.HotspotGameDatas.Values)
		{
			if (value.WorldMapView != null && !value.WorldMapView.IsDungeonHotSpot() && !(value.WorldMapView is HotSpotWorldMapViewPortalNode))
			{
				num += value.Data.StarCount;
			}
		}
		DIContainerLogic.InventoryService.SetItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "collection_event_stars", num, "EventStageResolved");
	}

	public void UpdateEventRewardStatus()
	{
		EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		if (currentEventManagerGameData == null)
		{
			DebugLog.Warn(GetType(), "No EventManagerGameData found!");
		}
		else if (currentEventManagerGameData.IsBossEvent)
		{
			UpdateBossRewardState();
		}
		else if (currentEventManagerGameData.IsCampaignEvent)
		{
			UpdateCampaignCollectionStatus();
		}
		else
		{
			DebugLog.Log(GetType(), "No valid event type for rewards!");
		}
	}

	private void UpdateBossRewardState()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss != null && DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss.RewardStatus < EventCampaignRewardStatus.unlocked_new && IsCollectionComplete())
		{
			DebugLog.Log("Collection assembled! Processing reward...");
			CollectionGroupBalancingData collectionGroupBalancing = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentEventBoss.CollectionGroupBalancing;
			Dictionary<string, int> dictionary = ((!DIContainerInfrastructure.EventSystemStateManager.UseCollectionFallbackReward() && DIContainerLogic.EventSystemService.GetAvailableEliteChestReward(DIContainerInfrastructure.GetCurrentPlayer()).Count != 0) ? collectionGroupBalancing.Reward : collectionGroupBalancing.FallbackReward);
			if (DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, dictionary.Keys.FirstOrDefault()))
			{
				dictionary = collectionGroupBalancing.FallbackReward;
			}
			int level = 1;
			List<IInventoryItemGameData> list = DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(dictionary, level), "CollectionReward");
			DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss.RewardStatus = ((!UseCollectionFallbackReward() && DIContainerLogic.EventSystemService.GetAvailableEliteChestReward(DIContainerInfrastructure.GetCurrentPlayer()).Count != 0) ? EventCampaignRewardStatus.unlocked_new : EventCampaignRewardStatus.unlocked_new_fallback);
		}
	}

	public void UpdateCampaignCollectionStatus()
	{
		EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		if (currentEventManagerGameData != null && currentEventManagerGameData.CurrentMiniCampaign != null && currentEventManagerGameData.CurrentMiniCampaign != null && currentEventManagerGameData.CurrentMiniCampaign.Data.RewardStatus < EventCampaignRewardStatus.unlocked_new && IsCollectionComplete())
		{
			DebugLog.Log("Collection assembled! Processing reward...");
			bool flag = UseCollectionFallbackReward();
			Dictionary<string, int> loot = ((!flag) ? currentEventManagerGameData.CurrentMiniCampaign.CollectionGroupBalancing.Reward : currentEventManagerGameData.CurrentMiniCampaign.CollectionGroupBalancing.FallbackReward);
			int level = 1;
			List<IInventoryItemGameData> list = DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(loot, level), "CollectionReward");
			currentEventManagerGameData.CurrentMiniCampaign.Data.RewardStatus = ((!flag) ? EventCampaignRewardStatus.unlocked_new : EventCampaignRewardStatus.unlocked_new_fallback);
		}
	}

	public bool UseCollectionFallbackReward(EventManagerGameData evtMgr = null)
	{
		if (evtMgr == null)
		{
			evtMgr = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		}
		if (evtMgr == null || (!evtMgr.IsBossEvent && !evtMgr.IsCampaignEvent))
		{
			return false;
		}
		CollectionGroupBalancingData collectionGroupBalancingData = null;
		if (DIContainerLogic.EventSystemService.IsEventTeasing(evtMgr.Balancing) && evtMgr.IsCampaignEvent)
		{
			EventItemGameData eventItemGameData = DIContainerLogic.EventSystemService.GenerateEventMiniCampaignPortal(evtMgr.EventBalancing, DIContainerInfrastructure.GetCurrentPlayer());
			string nameId = eventItemGameData.BalancingData.EventParameters[0];
			string collectionGroupId = DIContainerBalancing.Service.GetBalancingData<MiniCampaignBalancingData>(nameId).CollectionGroupId;
			collectionGroupBalancingData = DIContainerBalancing.Service.GetBalancingData<CollectionGroupBalancingData>(collectionGroupId);
		}
		else
		{
			if (DIContainerLogic.EventSystemService.IsEventTeasing(evtMgr.Balancing) && evtMgr.IsBossEvent)
			{
				return false;
			}
			collectionGroupBalancingData = ((!evtMgr.IsBossEvent) ? evtMgr.CurrentMiniCampaign.CollectionGroupBalancing : evtMgr.CurrentEventBoss.CollectionGroupBalancing);
		}
		return collectionGroupBalancingData != null && collectionGroupBalancingData.FallbackRewardRequirements != null && collectionGroupBalancingData.FallbackRewardRequirements.Count != 0 && (DIContainerLogic.EventSystemService.GetCurrentCollectionRewardStatus() >= EventCampaignRewardStatus.unlocked_new_fallback || (DIContainerLogic.EventSystemService.GetCurrentCollectionRewardStatus() < EventCampaignRewardStatus.unlocked_new && DIContainerLogic.RequirementService.CheckGenericRequirements(DIContainerInfrastructure.GetCurrentPlayer(), collectionGroupBalancingData.FallbackRewardRequirements)));
	}

	public bool IsCollectionComplete()
	{
		EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		CollectionGroupBalancingData collectionGroupBalancingData = ((!currentEventManagerGameData.IsBossEvent) ? currentEventManagerGameData.CurrentMiniCampaign.CollectionGroupBalancing : currentEventManagerGameData.CurrentEventBoss.CollectionGroupBalancing);
		if (collectionGroupBalancingData == null)
		{
			DebugLog.Warn(GetType(), "IsCollectionComplete: No CollectionBalancingData found!");
			return false;
		}
		for (int i = 0; i < collectionGroupBalancingData.ComponentRequirements.Count; i++)
		{
			Requirement req = collectionGroupBalancingData.ComponentRequirements[i];
			if (!DIContainerLogic.RequirementService.CheckRequirement(DIContainerInfrastructure.GetCurrentPlayer(), req))
			{
				return false;
			}
		}
		return true;
	}

	public GenericAssetProvider GetCurrentEventAssetProvider()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData != null && DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.IsValid)
		{
			return m_LoadedEventAssetProviders[DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.NameId];
		}
		return null;
	}

	public GameObject InstantiateEventObject(string assetName, Transform root, EventManagerGameData customMgr = null)
	{
		if (customMgr == null)
		{
			if (!DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
			{
				return null;
			}
			customMgr = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		}
		if (customMgr.IsAssetValid)
		{
			GenericAssetProvider genericAssetProvider = m_LoadedEventAssetProviders[customMgr.EventBalancing.NameId];
			if ((bool)genericAssetProvider)
			{
				return genericAssetProvider.InstantiateObject(customMgr.EventBalancing.AssetBaseId + "_" + assetName, root, Vector3.zero, Quaternion.identity);
			}
		}
		DebugLog.Error(GetType(), $"InstantiateEventObject returning null for object {customMgr.EventBalancing.AssetBaseId + "_" + assetName}");
		return null;
	}

	public List<IInventoryItemGameData> GetCollectionComponentFallbackItemGameData(IInventoryItemGameData item, bool isSecondaryHard = false)
	{
		if (item == null)
		{
			DebugLog.Log(GetType(), "GetCollectionComponentFallbackItemGameData: Item parameter was null....");
			return null;
		}
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (!DIContainerLogic.EventSystemService.IsCurrentEventAvailable(currentPlayer))
		{
			DebugLog.Log(GetType(), "GetCollectionComponentFallbackItemGameData: No current even found! Stepping out.");
			return null;
		}
		if (currentPlayer.CurrentEventManagerGameData.CurrentMiniCampaign == null || currentPlayer.CurrentEventManagerGameData.CurrentMiniCampaign.CollectionGroupBalancing == null)
		{
			DebugLog.Log(GetType(), "GetCollectionComponentFallbackItemGameData: No Campaign or Collectiongroup available to search through! Stepping out.");
			return null;
		}
		CollectionGroupBalancingData collectionGroupBalancing = currentPlayer.CurrentEventManagerGameData.CurrentMiniCampaign.CollectionGroupBalancing;
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Dictionary<string, int> dictionary2 = ((!isSecondaryHard) ? DIContainerLogic.EventSystemService.m_CachedFallBackLoot : DIContainerLogic.EventSystemService.m_CachedSecondaryFallBackLoot);
		if (dictionary2 != null && dictionary2.Count > 0)
		{
			for (int i = 0; i < dictionary2.Count; i++)
			{
				string key = dictionary2.Keys.ToList()[i];
				int num = dictionary2.Values.ToList()[i];
				dictionary[key] = num * item.ItemValue;
			}
		}
		else
		{
			for (int j = 0; j < collectionGroupBalancing.ComponentFallbackLoot.Count; j++)
			{
				string key2 = collectionGroupBalancing.ComponentFallbackLoot.Keys.ToList()[j];
				int num2 = collectionGroupBalancing.ComponentFallbackLoot.Values.ToList()[j];
				dictionary[key2] = num2 * item.ItemValue;
			}
		}
		return DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(dictionary, DIContainerInfrastructure.GetCurrentPlayer().Data.Level));
	}

	public bool IsCollectionComponentFull(IInventoryItemGameData pItem)
	{
		BasicItemGameData basicItemGameData = pItem as BasicItemGameData;
		if (basicItemGameData == null)
		{
			DebugLog.Log(GetType(), "IsCollectionComponentFull: Item parameter was null....");
			return false;
		}
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (!DIContainerLogic.EventSystemService.IsCurrentEventAvailable(currentPlayer))
		{
			DebugLog.Log(GetType(), "IsCollectionComponentFull: No current even found! Stepping out.");
			return false;
		}
		CollectionGroupBalancingData collectionGroupBalancing = currentPlayer.CurrentEventManagerGameData.CurrentMiniCampaign.CollectionGroupBalancing;
		if (collectionGroupBalancing == null)
		{
			DebugLog.Log(GetType(), "IsCollectionComponentFull: No Collectiongroup available to search through! Stepping out.");
			return false;
		}
		List<Requirement> componentRequirements = collectionGroupBalancing.ComponentRequirements;
		for (int i = 0; i < componentRequirements.Count; i++)
		{
			if (componentRequirements[i].NameId == basicItemGameData.BalancingData.NameId)
			{
				bool flag = DIContainerLogic.RequirementService.CheckRequirement(currentPlayer, componentRequirements[i]);
				DebugLog.Log(GetType(), "IsCollectionComponentFull: collection component found for current event: " + basicItemGameData.BalancingData.NameId + " requirement met? => " + flag);
				return flag;
			}
		}
		DebugLog.Log(GetType(), "IsCollectionComponentFull: Item " + basicItemGameData.BalancingData.NameId + " not found in current collection group.");
		return false;
	}

	public void ShowEventResult()
	{
	}

	internal Dictionary<string, string> GetCollectionRewardReplacementDict(EventManagerGameData evtMgr = null)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		EventManagerGameData eventManagerGameData = evtMgr ?? currentPlayer.CurrentEventManagerGameData;
		if (eventManagerGameData == null || (!eventManagerGameData.IsBossEvent && !eventManagerGameData.IsCampaignEvent))
		{
			return dictionary;
		}
		CollectionGroupBalancingData collectionGroupBalancingData = ((!eventManagerGameData.IsBossEvent) ? eventManagerGameData.CurrentMiniCampaign.CollectionGroupBalancing : eventManagerGameData.CurrentEventBoss.CollectionGroupBalancing);
		if (collectionGroupBalancingData == null)
		{
			return dictionary;
		}
		string text = ((!UseCollectionFallbackReward(eventManagerGameData)) ? collectionGroupBalancingData.Reward.FirstOrDefault().Key : collectionGroupBalancingData.FallbackReward.FirstOrDefault().Key);
		ClassSkinBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<ClassSkinBalancingData>(text);
		string nameId = ((balancingData != null) ? balancingData.OriginalClass : text);
		ClassItemBalancingData balancingData2 = DIContainerBalancing.Service.GetBalancingData<ClassItemBalancingData>(nameId);
		if (balancingData2 == null)
		{
			return dictionary;
		}
		string value = DIContainerInfrastructure.GetLocaService().Tr(balancingData2.RestrictedBirdId + "_name");
		string value2 = ((balancingData == null) ? DIContainerInfrastructure.GetLocaService().Tr(balancingData2.LocaBaseId + "_name") : DIContainerInfrastructure.GetLocaService().Tr(balancingData.LocaBaseId + "_name"));
		dictionary.Add("{value_1}", value2);
		dictionary.Add("{value_2}", value);
		return dictionary;
	}

	internal BasicItemGameData CreateChestRewardPopupItem(EventManagerGameData evtmgr)
	{
		if (evtmgr == null || !evtmgr.IsValid || !DIContainerLogic.EventSystemService.IsChestCollectionReward(evtmgr))
		{
			return null;
		}
		string nameId = ((!evtmgr.IsCampaignEvent) ? "collection_elite_chest_worldboss" : "collection_elite_chest_campaign");
		return DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, nameId, 1) as BasicItemGameData;
	}

	public UIAtlas GetCurrentEventUiAtlas()
	{
		GenericAssetProvider currentEventAssetProvider = DIContainerInfrastructure.EventSystemStateManager.GetCurrentEventAssetProvider();
		if (currentEventAssetProvider != null)
		{
			string nameId = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Balancing.NameId;
			nameId = nameId.Remove(nameId.IndexOf('_'));
			string nameId2 = ("EventImage_Campaign_" + nameId).ToLower();
			GameObject gameObject = currentEventAssetProvider.GetObject(nameId2) as GameObject;
			if (gameObject != null)
			{
				return gameObject.GetComponent<UIAtlas>();
			}
		}
		DebugLog.Error(GetType(), "No current event ui atlas found!");
		return null;
	}
}
