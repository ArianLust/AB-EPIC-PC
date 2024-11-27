using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using UnityEngine;

public class EventSystemWorldMapStateMgr : MonoBehaviour
{
	private Dictionary<string, EventPositionNode> m_PossiblePositionNodes = new Dictionary<string, EventPositionNode>();

	private Dictionary<string, EventPositionNode> m_PossibleCollectibleNodes = new Dictionary<string, EventPositionNode>();

	private Dictionary<string, EventPositionNode> m_EntranceNodes = new Dictionary<string, EventPositionNode>();

	private Dictionary<string, EventPositionNode> m_BossNodes = new Dictionary<string, EventPositionNode>();

	private EventPositionNode m_nearestNode;

	private float distance;

	private EventPositionNode m_currentBossNode;

	private static bool m_attackInfosFromOtherPlayersFetched;

	private int m_cachedNumberOfAttacks;

	public BossCombatant m_WorldMapBossCombatant;

	public bool m_BossInitialized;

	public bool m_BossDefeatAniShown;

	private bool m_BossHpUpdateRunning;

	public int m_MaxAttacksOnFirstSpawn = 5;

	private EventManagerGameData m_Model;

	static EventSystemWorldMapStateMgr()
	{
	}

	private IEnumerator Start()
	{
		while (DIContainerInfrastructure.EventSystemStateManager == null || !DIContainerInfrastructure.EventSystemStateManager.IsInitialized)
		{
			yield return new WaitForSeconds(1f);
		}
		m_PossiblePositionNodes.Clear();
		m_PossibleCollectibleNodes.Clear();
		m_EntranceNodes.Clear();
		m_BossNodes.Clear();
		m_BossHpUpdateRunning = false;
		EventPositionNode[] allHotspotPositionNodes = DIContainerInfrastructure.LocationStateMgr.gameObject.GetComponentsInChildren<EventPositionNode>(true);
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		for (int i = 0; i < allHotspotPositionNodes.Length; i++)
		{
			if (allHotspotPositionNodes[i].SynchBalancing() && DIContainerLogic.RequirementService.CheckGenericRequirements(player, allHotspotPositionNodes[i].GetModel().SpawnAbleRequirements))
			{
				if (allHotspotPositionNodes[i].m_IsCollectible)
				{
					m_PossibleCollectibleNodes.Add(allHotspotPositionNodes[i].GetModel().NameId, allHotspotPositionNodes[i]);
				}
				else if (allHotspotPositionNodes[i].m_IsEntrance)
				{
					m_EntranceNodes.Add(allHotspotPositionNodes[i].GetModel().NameId, allHotspotPositionNodes[i]);
				}
				else if (allHotspotPositionNodes[i].m_IsBossLocation)
				{
					m_BossNodes.Add(allHotspotPositionNodes[i].GetModel().NameId, allHotspotPositionNodes[i]);
				}
				else
				{
					m_PossiblePositionNodes.Add(allHotspotPositionNodes[i].GetModel().NameId, allHotspotPositionNodes[i]);
				}
			}
		}
		if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(player) && DIContainerLogic.EventSystemService.IsEventRunning(player.CurrentEventManagerGameData.Balancing))
		{
			if (player.CurrentEventManagerGameData.Data.CurrentState != EventManagerState.Running)
			{
				player.CurrentEventManagerGameData.Data.CurrentState = EventManagerState.Running;
			}
			DebugLog.Log(GetType(), "[EPIC SERVER] Start: pulling leaderboard update!");
			DIContainerLogic.EventSystemService.GetLeaderboardScores(DIContainerInfrastructure.GetCurrentPlayer(), player.CurrentEventManagerGameData.Data.CurrentOpponents, player.CurrentEventManagerGameData, SpawnEventItems, delegate
			{
				SpawnEventItems();
			});
			RegisterEventHandlers();
			yield return new WaitForSeconds(5f);
			/*Error: Unable to find new state assignment for yield return*/;
		}
	}

	private void SpawnEventItems()
	{
		EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		if (currentEventManagerGameData.IsCampaignEvent)
		{
			StartCoroutine(SpawnMiniCampaignPortal());
		}
		StartCoroutine(SpawnEncounters());
		StartCoroutine(SpawnCollectibles());
		if (currentEventManagerGameData.IsBossEvent)
		{
			StartCoroutine(SpawnBoss());
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			m_attackInfosFromOtherPlayersFetched = false;
		}
		else if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData != null && DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.IsBossEvent)
		{
			DIContainerLogic.EventSystemService.GetEventBossDefeatLog(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData, delegate(RESTResultEnum result)
			{
				OnGetBossDefeatLogCallback(result, false);
			});
		}
	}

	public void SpawnEncountersNow()
	{
		StartCoroutine(SpawnEncounters());
		StartCoroutine(SpawnCollectibles());
	}

	private void RegisterEventHandlers()
	{
		if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			DeregisterEventHandlers();
			DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.StateChanged += CurrentEventManagerGameDataStateChanged;
		}
	}

	private void DeregisterEventHandlers()
	{
		if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.StateChanged -= CurrentEventManagerGameDataStateChanged;
		}
	}

	private void OnDestroy()
	{
		m_BossHpUpdateRunning = false;
		DeregisterEventHandlers();
	}

	private void CurrentEventManagerGameDataStateChanged(EventManagerState oldState, EventManagerState newState)
	{
		if (newState > EventManagerState.Running)
		{
			DebugLog.Log("[EventSystemWorldMapStateMgr] Despawn Encounters");
			DeregisterEventHandlers();
			StartCoroutine(DespawnEncountersAndRestart());
		}
	}

	private IEnumerator DespawnEncountersAndRestart()
	{
		yield return StartCoroutine(DespawnAllEventItems());
		StopCoroutine("Start");
		StartCoroutine("Start");
	}

	private IEnumerator DespawnAllEventItems()
	{
		m_PossiblePositionNodes.Clear();
		m_PossibleCollectibleNodes.Clear();
		EventPositionNode[] allHotspotPositionNodes = DIContainerInfrastructure.LocationStateMgr.gameObject.GetComponentsInChildren<EventPositionNode>(true);
		for (int i = 0; i < allHotspotPositionNodes.Length; i++)
		{
			if (allHotspotPositionNodes[i].SynchBalancing() && DIContainerLogic.RequirementService.CheckGenericRequirements(DIContainerInfrastructure.GetCurrentPlayer(), allHotspotPositionNodes[i].GetModel().SpawnAbleRequirements))
			{
				if (allHotspotPositionNodes[i].m_IsCollectible)
				{
					m_PossibleCollectibleNodes.Add(allHotspotPositionNodes[i].GetModel().NameId, allHotspotPositionNodes[i]);
				}
				else
				{
					m_PossiblePositionNodes.Add(allHotspotPositionNodes[i].GetModel().NameId, allHotspotPositionNodes[i]);
				}
				if (allHotspotPositionNodes[i].HasItem())
				{
					allHotspotPositionNodes[i].DespawnItem();
				}
			}
		}
		yield break;
	}

	private IEnumerator SpawnMiniCampaignPortal()
	{
		if (!DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.IsCampaignEvent)
		{
			yield break;
		}
		while (!DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.IsAssetValid)
		{
			yield return new WaitForEndOfFrame();
		}
		EventItemGameData mcItem = null;
		List<IInventoryItemGameData> eventCampaignItems = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.EventCampaignItem];
		if (eventCampaignItems.Count > 0)
		{
			for (int i = 0; i < eventCampaignItems.Count; i++)
			{
				IInventoryItemGameData item = eventCampaignItems[i];
				mcItem = item as EventItemGameData;
			}
		}
		else
		{
			mcItem = DIContainerLogic.EventSystemService.GenerateEventMiniCampaignPortal(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing, DIContainerInfrastructure.GetCurrentPlayer());
		}
		string miniCampName = mcItem.BalancingData.EventParameters[0];
		EventCampaignData cMiniMapData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.EventCampaignData;
		EventCampaignGameData cMiniMap = ((cMiniMapData == null) ? new EventCampaignGameData(miniCampName) : new EventCampaignGameData(cMiniMapData));
		DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentMiniCampaign = cMiniMap;
		foreach (EventPositionNode node in m_EntranceNodes.Values)
		{
			DebugLog.Log(GetType(), "SpawnMiniCampaignPortal: Spawning portal for " + mcItem.BalancingData.NameId);
			node.SetItem(mcItem);
		}
	}

	private IEnumerator SpawnEncounters()
	{
		if (!DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()) || !DIContainerLogic.EventSystemService.IsEventRunning(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Balancing))
		{
			yield break;
		}
		DebugLog.Log("Start spawn Encounters");
		if (DIContainerInfrastructure.EventSystemStateManager.lastRemovedEventItem != null)
		{
			EventPositionNode possiblePositionNode = null;
			if (m_PossiblePositionNodes.TryGetValue(DIContainerInfrastructure.EventSystemStateManager.lastRemovedEventItem.Data.PositionId, out possiblePositionNode))
			{
				possiblePositionNode.SetItem(DIContainerInfrastructure.EventSystemStateManager.lastRemovedEventItem, true);
				yield return new WaitForSeconds(DIContainerInfrastructure.LocationStateMgr.TweenCameraToTransform(possiblePositionNode.transform));
				while (!possiblePositionNode.IsNodeSpawned)
				{
					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForSeconds(possiblePositionNode.AccomplishItem());
			}
			DIContainerInfrastructure.EventSystemStateManager.lastRemovedEventItem = null;
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.LastEncounterSpawnTime != 0 && DIContainerLogic.GetTimingService().IsAfter(DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.LastEncounterSpawnTime).AddSeconds(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.TimeForEncounterRespawnInSec)))
		{
			DIContainerLogic.EventSystemService.RemoveEncounters(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData);
		}
		List<IInventoryItemGameData> battleEventItemGameDatas = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.EventBattleItem];
		List<EventPositionNode> orderedByDistance2 = m_PossiblePositionNodes.Values.ToList();
		for (int j = 0; j < battleEventItemGameDatas.Count; j++)
		{
			IInventoryItemGameData inventoryItemGameData = battleEventItemGameDatas[j];
			EventItemGameData battleEventItemGameData = (EventItemGameData)inventoryItemGameData;
			if (!string.IsNullOrEmpty(battleEventItemGameData.Data.PositionId))
			{
				EventPositionNode possiblePositionNode2 = null;
				if (m_PossiblePositionNodes.TryGetValue(battleEventItemGameData.Data.PositionId, out possiblePositionNode2))
				{
					possiblePositionNode2.SetItem(battleEventItemGameData);
					m_PossiblePositionNodes.Remove(battleEventItemGameData.Data.PositionId);
				}
			}
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.MaxAmountOfEventItems > battleEventItemGameDatas.Count)
		{
			EventPositionNode nearestEventPositionNode = null;
			bool takeNearest = false;
			if ((bool)DIContainerInfrastructure.LocationStateMgr.SceneryCamera && battleEventItemGameDatas.Count == 0)
			{
				orderedByDistance2 = orderedByDistance2.OrderBy((EventPositionNode node) => Vector3.Distance(node.transform.position, DIContainerInfrastructure.LocationStateMgr.SceneryCamera.transform.position)).ToList();
				nearestEventPositionNode = orderedByDistance2.FirstOrDefault();
				if ((bool)nearestEventPositionNode && !nearestEventPositionNode.HasItem())
				{
					takeNearest = true;
				}
			}
			int eventItemsCountOld = battleEventItemGameDatas.Count;
			for (int i = 0; i < DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.MaxAmountOfEventItems - eventItemsCountOld; i++)
			{
				if (takeNearest)
				{
					nearestEventPositionNode.SetItem(DIContainerLogic.EventSystemService.GenerateEventItem(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing, DIContainerInfrastructure.GetCurrentPlayer()));
					m_PossiblePositionNodes.Remove(nearestEventPositionNode.GetModel().NameId);
					takeNearest = false;
					continue;
				}
				List<EventPositionNode> randomNodes = m_PossiblePositionNodes.Values.ToList();
				if (randomNodes.Count <= 0)
				{
					DebugLog.Error("[EventSystemWorldMapStateMgr] No further spawn locations available!");
					break;
				}
				EventPositionNode randomEventPositionNode = randomNodes[Random.Range(0, randomNodes.Count)];
				randomEventPositionNode.SetItem(DIContainerLogic.EventSystemService.GenerateEventItem(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing, DIContainerInfrastructure.GetCurrentPlayer()));
				m_PossiblePositionNodes.Remove(randomEventPositionNode.GetModel().NameId);
			}
			DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.LastEncounterSpawnTime = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		}
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
	}

	private IEnumerator SpawnCollectibles()
	{
		if (!DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()) || !DIContainerLogic.EventSystemService.IsEventRunning(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Balancing))
		{
			yield break;
		}
		if (DIContainerInfrastructure.EventSystemStateManager.lastRemovedEventItem != null)
		{
			EventPositionNode possiblePositionNode = null;
			if (m_PossibleCollectibleNodes.TryGetValue(DIContainerInfrastructure.EventSystemStateManager.lastRemovedEventItem.Data.PositionId, out possiblePositionNode))
			{
				possiblePositionNode.SetItem(DIContainerInfrastructure.EventSystemStateManager.lastRemovedEventItem, true);
				yield return new WaitForSeconds(DIContainerInfrastructure.LocationStateMgr.TweenCameraToTransform(possiblePositionNode.transform));
				while (!possiblePositionNode.IsNodeSpawned)
				{
					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForSeconds(possiblePositionNode.AccomplishItem());
			}
			DIContainerInfrastructure.EventSystemStateManager.lastRemovedEventItem = null;
		}
		List<IInventoryItemGameData> collectibleEventItemGameDatas = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.EventCollectible];
		for (int j = 0; j < collectibleEventItemGameDatas.Count; j++)
		{
			IInventoryItemGameData inventoryItemGameData = collectibleEventItemGameDatas[j];
			EventItemGameData collectibleEventItemGameData = (EventItemGameData)inventoryItemGameData;
			if (collectibleEventItemGameData.Data.PositionId == null)
			{
				DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.RemoveItemFromInventory(collectibleEventItemGameData);
				continue;
			}
			EventPositionNode possiblePositionNode2 = null;
			if (m_PossibleCollectibleNodes.TryGetValue(collectibleEventItemGameData.Data.PositionId, out possiblePositionNode2))
			{
				possiblePositionNode2.SetItem(collectibleEventItemGameData);
				m_PossibleCollectibleNodes.Remove(collectibleEventItemGameData.Data.PositionId);
			}
		}
		uint lastSpawn = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.LastCollectibleSpawnTime;
		float respawnTime = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.TimeForCollectibleRespawnInSec;
		int numberOfCollectiblesRemaining = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.MaxNumberOfCollectibles - DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.EventCollectible].Count;
		int numberOfItemsToSpawn = ((lastSpawn == 0) ? DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing.MaxNumberOfCollectibles : 0);
		if (lastSpawn != 0 && numberOfCollectiblesRemaining > 0 && DIContainerLogic.GetTimingService().IsAfter(DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(lastSpawn).AddSeconds(respawnTime)))
		{
			int k = (int)((float)(DIContainerLogic.GetTimingService().GetCurrentTimestamp() - lastSpawn) / respawnTime);
			numberOfItemsToSpawn += Mathf.Min(k, numberOfCollectiblesRemaining);
		}
		if (numberOfItemsToSpawn > 0)
		{
			for (int i = 0; i < numberOfItemsToSpawn; i++)
			{
				EventItemGameData eventItem = DIContainerLogic.EventSystemService.GenerateEventCollectible(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.EventBalancing, DIContainerInfrastructure.GetCurrentPlayer());
				List<EventPositionNode> randomNodes = (from node in m_PossibleCollectibleNodes.Values.ToList()
					where eventItem.BalancingData.SpawnCategories.Contains(node.SpawnCategory)
					select node).ToList();
				if (randomNodes.Count <= 0)
				{
					DebugLog.Error("[EventSystemWorldMapStateMgr] No further spawn locations available!");
					break;
				}
				EventPositionNode randomEventPositionNode = randomNodes[Random.Range(0, randomNodes.Count)];
				randomEventPositionNode.SetItem(eventItem);
				m_PossibleCollectibleNodes.Remove(randomEventPositionNode.GetModel().NameId);
			}
			DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.LastCollectibleSpawnTime = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		}
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
	}

	public void StartSpawnBossCoroutine()
	{
		StartCoroutine(SpawnBoss());
	}

	private IEnumerator SpawnBoss()
	{
		m_BossInitialized = false;
		if (m_currentBossNode != null)
		{
			yield return new WaitForSeconds(m_currentBossNode.DespawnItem());
		}
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		EventManagerGameData emgr = player.CurrentEventManagerGameData;
		if (emgr == null || !emgr.IsBossEvent)
		{
			yield break;
		}
		EventItemGameData eItem = null;
		List<IInventoryItemGameData> eventBossItems = player.InventoryGameData.Items[InventoryItemType.EventBossItem];
		if (eventBossItems.Count > 0)
		{
			eItem = eventBossItems[0] as EventItemGameData;
		}
		else
		{
			eItem = DIContainerLogic.EventSystemService.GenerateEventBoss(emgr.EventBalancing, player);
		}
		string currentBossNodeId = emgr.CurrentEventBoss.EventNodeId;
		if (currentBossNodeId == null)
		{
			string bossNodeId = GetBossNodeId(Random.Range(0, this.m_BossNodes.Count - 1), 0, 10);
			emgr.CurrentEventBoss.EventNodeId = bossNodeId;
			currentBossNodeId = bossNodeId;
		}
		if (emgr.CurrentEventBoss.Data.LastPositionSwapOnDefeat < player.Data.WorldBoss.DeathCount && !DIContainerLogic.EventSystemService.IsBossOnCooldown())
		{
			currentBossNodeId = SetNewRandomBossPosition(emgr.CurrentEventBoss);
		}
		if (m_BossNodes.TryGetValue(currentBossNodeId, out this.m_currentBossNode))
		{
			m_currentBossNode.SetItem(eItem, false);
			if (string.IsNullOrEmpty(player.m_LastBattle) || player.m_LastBattle.Contains("boss"))
			{
				DIContainerInfrastructure.LocationStateMgr.TweenCameraToTransform(this.m_currentBossNode.transform);
			}
		}
		if (string.IsNullOrEmpty(emgr.Data.LeaderboardId))
		{
			DebugLog.Log(base.GetType(), "SpawnBoss: No leaderboard yet, therefore not initializing boss combatant!!");
			yield break;
		}
		DIContainerLogic.EventSystemService.GetEventBossDefeatLog(emgr, delegate(RESTResultEnum result)
		{
			OnGetBossDefeatLogCallback(result, true);
		});
		m_BossInitialized = true;
	}

	public EventPositionNode GetBossNode()
	{
		if (m_currentBossNode == null && DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData != null)
		{
			string eventNodeId = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentEventBoss.EventNodeId;
			m_BossNodes.TryGetValue(eventNodeId, out m_currentBossNode);
		}
		return m_currentBossNode;
	}

	public IEnumerator PlayBossDefeatAnimation()
	{
		if (!m_currentBossNode)
		{
			yield break;
		}
		BossAssetController bossAssetController = m_currentBossNode.GetComponentInChildren<BossAssetController>();
		float timeout = 0f;
		while (bossAssetController == null && timeout < 7.5f)
		{
			yield return new WaitForSeconds(0.5f);
			bossAssetController = m_currentBossNode.GetComponentInChildren<BossAssetController>();
			timeout += 0.5f;
		}
		if (!(bossAssetController == null))
		{
			if (m_BossDefeatAniShown)
			{
				bossAssetController.SetDefeatState();
				yield return new WaitForSeconds(bossAssetController.GetDefeatAnimationLength());
			}
			else
			{
				bossAssetController.PlayDefeatAnim();
				m_BossDefeatAniShown = true;
			}
			m_cachedNumberOfAttacks = 0;
		}
	}

	private IEnumerator WorldBossWasAttackedVisuals(PublicPlayerData attackingPlayer)
	{
		m_cachedNumberOfAttacks++;
		yield return StartCoroutine(base.transform.GetComponent<WorldMapStateMgr>().ShowPlayerAttacksBossAnim(attackingPlayer));
		BossBalancingData balancing = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentEventBoss.BalancingData;
		m_WorldMapBossCombatant.CurrentHealth = m_WorldMapBossCombatant.ModifiedHealth - m_WorldMapBossCombatant.ModifiedHealth * ((float)m_cachedNumberOfAttacks / (float)balancing.AttacksNeeded);
		if (m_cachedNumberOfAttacks >= DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentEventBoss.BalancingData.AttacksNeeded)
		{
			StartCoroutine(PlayBossDefeatAnimation());
			m_BossHpUpdateRunning = false;
		}
		else if (m_currentBossNode.GetComponentInChildren<BossAssetController>() != null)
		{
			m_currentBossNode.GetComponentInChildren<BossAssetController>().PlayHitAnim();
		}
	}

	public string GetBossNodeId(int index, int recursionCount = 0, int maxDepth = 10)
	{
		foreach (EventPositionNode value in m_BossNodes.Values)
		{
			if (value.m_balancingName.Contains(index.ToString()))
			{
				return value.m_balancingName;
			}
		}
		if (recursionCount > maxDepth)
		{
			DebugLog.Log(GetType(), "GetBossNodeID: Max recursion count reached. Breaking recursive call. No boss node found for index " + index);
			return string.Empty;
		}
		return GetBossNodeId(index + 1, recursionCount + 1);
	}

	private void OnGetBossDefeatLogCallback(RESTResultEnum result, bool firstSpawn = false)
	{
		if (result != RESTResultEnum.Success)
		{
			DebugLog.Error(GetType(), "OnGetBossDefeatLogCallback: Service call failed: result=" + result);
			return;
		}
		DebugLog.Log(GetType(), string.Concat("[BOSS] OnGetBossDefeatLogCallback: result = ", result, ". Now initalizing healthbar and updating hp!"));
		WorldEventBossData worldBoss = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss;
		if (worldBoss != null && firstSpawn)
		{
			while (worldBoss.DefeatsToProcess != null && worldBoss.DefeatsToProcess.Count > m_MaxAttacksOnFirstSpawn && DIContainerLogic.EventSystemService.ProcessPendingAttack())
			{
			}
		}
		if (DIContainerLogic.EventSystemService.IsBossOnCooldown() && firstSpawn)
		{
			m_BossDefeatAniShown = true;
		}
		UpdateWorldBossHP();
	}

	public void UpdateWorldBossHP()
	{
		StartCoroutine(UpdateWorldBossHPCoroutine());
	}

	private IEnumerator UpdateWorldBossHPCoroutine()
	{
		DebugLog.Log("[EPIC SERVER] UpdateWorldBossHPCoroutineFromServer");
		if (m_BossHpUpdateRunning)
		{
			yield break;
		}
		m_BossHpUpdateRunning = true;
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		WorldEventBossData bossData = player.Data.WorldBoss;
		if (DIContainerLogic.EventSystemService.IsBossOnCooldown() && bossData != null)
		{
			StartCoroutine(PlayBossDefeatAnimation());
			DebugLog.Log(GetType(), "[EPIC SERVER] UpdateWorldBossHPCoroutineFromServer: worldboss is broken. Needs time to repair. Not updating health!");
			m_BossHpUpdateRunning = false;
			yield break;
		}
		WorldBossTeamData ownTeam = ((bossData.OwnTeamId != 1) ? bossData.Team2 : bossData.Team1);
		BossGameData currentEventBoss = player.CurrentEventManagerGameData.CurrentEventBoss;
		while (player.CurrentEventManagerGameData == null || currentEventBoss == null)
		{
			yield return new WaitForSeconds(0.5f);
		}
		m_cachedNumberOfAttacks = bossData.NumberOfAttacks % currentEventBoss.BalancingData.AttacksNeeded;
		DebugLog.Log(GetType(), "[EPIC SERVER] UpdateWorldBossHPCoroutineFromServer: cached # of attacks = " + m_cachedNumberOfAttacks);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		float timeout = 0f;
		while (!m_currentBossNode.GetComponentInChildren<CharacterHealthBar>())
		{
			yield return new WaitForSeconds(0.5f);
			timeout += 0.5f;
			if (timeout > 7.5f)
			{
				yield break;
			}
		}
		if (m_WorldMapBossCombatant == null)
		{
			m_WorldMapBossCombatant = new BossCombatant(currentEventBoss);
		}
		CharacterChunkHealthBar healthBar = m_currentBossNode.GetComponentInChildren<CharacterChunkHealthBar>();
		if (healthBar != null && m_WorldMapBossCombatant != null)
		{
			healthBar.SetModel(m_WorldMapBossCombatant);
			m_WorldMapBossCombatant.CurrentHealth = m_WorldMapBossCombatant.ModifiedHealth - m_WorldMapBossCombatant.ModifiedHealth * ((float)m_cachedNumberOfAttacks / (float)currentEventBoss.BalancingData.AttacksNeeded);
			healthBar.SetHealthInstant();
		}
		if (bossData.DefeatsToProcess == null)
		{
			yield break;
		}
		for (int i = 0; i < bossData.DefeatsToProcess.Count; i++)
		{
			if (!m_BossHpUpdateRunning)
			{
				DebugLog.Warn(GetType(), "UpdateWorldBossHpCoroutine: The Boss Update was aborted! stopping attack display spawn");
				break;
			}
			string teamMateId = bossData.DefeatsToProcess[i].Key;
			PublicPlayerData attackingPlayer = null;
			if (teamMateId == DIContainerInfrastructure.IdentityService.SharedId)
			{
				attackingPlayer = DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer;
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.PublicOpponentDatas.TryGetValue(teamMateId, out attackingPlayer);
			}
			if (attackingPlayer != null)
			{
				DIContainerLogic.EventSystemService.ProcessPendingAttack();
				i--;
				yield return StartCoroutine(WorldBossWasAttackedVisuals(attackingPlayer));
			}
		}
		m_BossHpUpdateRunning = false;
	}

	public void Restart()
	{
		StartCoroutine(DespawnEncountersAndRestart());
		WorldMapStateMgr worldMapStateMgr = DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr;
		if (worldMapStateMgr != null && worldMapStateMgr.m_WorldMenuUI.gameObject.activeInHierarchy)
		{
			worldMapStateMgr.m_WorldMenuUI.RecheckHotlinkButtons();
		}
	}

	public bool ToggleBossIdleAnimation(bool active = true)
	{
		if ((bool)m_currentBossNode && !DIContainerLogic.EventSystemService.IsBossOnCooldown())
		{
			BossAssetController componentInChildren = m_currentBossNode.GetComponentInChildren<BossAssetController>();
			if (componentInChildren == null)
			{
				return false;
			}
			if (active)
			{
				componentInChildren.PlayIdleAnimation();
			}
			else
			{
				componentInChildren.gameObject.PlayAnimationOrAnimatorState("Silent_Idle");
			}
			return true;
		}
		return false;
	}

	public string SetNewRandomBossPosition(BossGameData boss)
	{
		int index = Random.Range(0, m_BossNodes.Count - 1);
		for (int i = 0; i < 20; i++)
		{
			if (!string.IsNullOrEmpty(boss.EventNodeId) && !boss.EventNodeId.Contains(index.ToString("00")))
			{
				break;
			}
			index = Random.Range(0, m_BossNodes.Count - 1);
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss != null)
		{
			boss.Data.LastPositionSwapOnDefeat = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss.DeathCount;
		}
		return boss.EventNodeId = GetBossNodeId(index);
	}
}
