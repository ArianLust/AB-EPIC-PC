using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class EventPositionNode : MonoBehaviour
{
	private EventPlacementBalancingData m_model;

	public string m_balancingName;

	public bool m_IsCollectible;

	public bool m_IsEntrance;

	public bool m_IsBossLocation;

	[SerializeField]
	private EventPositionAsset m_SpawnAdditionPrefab;

	private EventPositionAsset m_SpawnAddition;

	[SerializeField]
	private Transform m_SpawnAdditionRoot;

	private bool m_instant;

	[SerializeField]
	private LootDisplayContoller m_ExplodeLootItems;

	private bool m_IsExploding;

	private EventItemGameData m_placedItem;

	private BattleBalancingData m_AssociatedBattle;

	private HotspotGameData m_AssociatedTempHotspotGameData;

	public bool IsNodeSpawned
	{
		get
		{
			return (bool)m_SpawnAddition && m_SpawnAddition.m_IsSpawned;
		}
	}

	public string SpawnCategory
	{
		get
		{
			return m_model.Category;
		}
	}

	public bool SynchBalancing()
	{
		if (DIContainerBalancing.EventBalancingService == null)
		{
			return false;
		}
		return DIContainerBalancing.EventBalancingService.TryGetBalancingData<EventPlacementBalancingData>(m_balancingName, out m_model);
	}

	public void SetItem(EventItemGameData eventItem, bool instant = false)
	{
		if (m_placedItem != null)
		{
			DebugLog.Warn(GetType(), "SetItem: m_placedItem != null");
			return;
		}
		m_instant = instant;
		m_placedItem = eventItem;
		m_placedItem.Data.PositionId = m_model.NameId;
		GetComponent<BoxCollider>().enabled = true;
		SpawnAdditionalPrefab(m_placedItem);
	}

	private void SpawnAdditionalPrefab(EventItemGameData item)
	{
		if ((bool)m_SpawnAdditionPrefab && (bool)m_SpawnAdditionRoot)
		{
			m_SpawnAddition = Object.Instantiate(m_SpawnAdditionPrefab);
			if (!(m_SpawnAddition == null))
			{
				m_SpawnAddition.transform.parent = m_SpawnAdditionRoot;
				m_SpawnAddition.transform.localPosition = Vector3.zero;
				m_SpawnAddition.SetItem(item, m_instant);
			}
		}
	}

	private void OnTouchClicked()
	{
		HandleClicked();
	}

	private void HandleClicked()
	{
		if (m_placedItem != null && !m_IsExploding && !DIContainerInfrastructure.CurrentDragController.m_dragging && DIContainerInfrastructure.LocationStateMgr.IsInitialized && !DIContainerInfrastructure.LocationStateMgr.IsBirdWalking())
		{
			HandleMouseButtonUp();
		}
	}

	public void HandleMouseButtonUp()
	{
		DebugLog.Log("[EventPositionNode] Clicked event node" + base.name);
		if (m_IsCollectible)
		{
			m_IsExploding = true;
		}
		if (m_placedItem.BalancingData.ItemType == InventoryItemType.EventBattleItem || m_placedItem.BalancingData.ItemType == InventoryItemType.EventBossItem)
		{
			WorldMapStateMgr worldMapStateMgr = DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr;
			EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
			if (worldMapStateMgr != null && !currentEventManagerGameData.IsBossEvent)
			{
				worldMapStateMgr.ShowBattlePreperationScreenForEvent(m_placedItem, m_model);
			}
			else if (worldMapStateMgr != null && currentEventManagerGameData.IsBossEvent && !DIContainerLogic.EventSystemService.IsBossOnCooldown())
			{
				worldMapStateMgr.ShowBattlePreperationScreenForEvent(m_placedItem, m_model);
			}
		}
		else if (m_placedItem.BalancingData.ItemType == InventoryItemType.EventCollectible)
		{
			HandleCollectibleClicked();
		}
		else if (m_placedItem.BalancingData.ItemType == InventoryItemType.EventCampaignItem)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.IsAssetValid)
			{
				DIContainerInfrastructure.GetCoreStateMgr().GoToMiniCampaign();
			}
			else
			{
				DebugLog.Warn("EventCampaign assets not loaded yet, blocking entrance!");
			}
		}
	}

	private void HandleCollectibleClicked()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add(m_placedItem.BalancingData.EventParameters[0], 1);
		Dictionary<string, LootInfoData> loot = DIContainerLogic.GetLootOperationService().GenerateLoot(dictionary, 1);
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(loot);
		LootDisplayContoller lootDisplayContoller = Object.Instantiate(m_ExplodeLootItems, m_SpawnAdditionRoot.transform.position, Quaternion.identity) as LootDisplayContoller;
		lootDisplayContoller.SetModel(null, itemsFromLoot, LootDisplayType.None);
		lootDisplayContoller.gameObject.SetActive(false);
		StartCoroutine(ExplodeLootAndDespawn(itemsFromLoot, lootDisplayContoller));
		DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 2, loot, "hotspot_chest");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		DIContainerLogic.EventSystemService.RemoveCollectibleFromLocation(m_balancingName);
	}

	private IEnumerator ExplodeLootAndDespawn(List<IInventoryItemGameData> lootItems, LootDisplayContoller explodingLoot)
	{
		float openDuration = 1f;
		if ((bool)m_SpawnAddition)
		{
			openDuration = m_SpawnAddition.OpenCollectible();
		}
		yield return new WaitForSeconds(openDuration * 0.3f);
		if (explodingLoot != null)
		{
			explodingLoot.transform.parent = base.transform;
		}
		explodingLoot.gameObject.SetActive(true);
		List<LootDisplayContoller> explodedLoot = explodingLoot.Explode(true, false, 0.5f, false, 0f, 0f);
		explodingLoot.gameObject.SetActive(false);
		yield return new WaitForSeconds(openDuration * 0.7f);
		Object.Destroy(explodingLoot);
		foreach (LootDisplayContoller explodedItem in explodedLoot)
		{
			WorldMapStateMgr worldMapStateMgr = DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr;
			Object.Destroy(explodedItem.gameObject, explodedItem.gameObject.GetComponent<Animation>().clip.length);
		}
		DespawnItem();
	}

	public EventPlacementBalancingData GetModel()
	{
		return m_model;
	}

	public bool HasItem()
	{
		return m_placedItem != null;
	}

	public float AccomplishItem()
	{
		m_placedItem = null;
		CharacterControllerWorldMap[] componentsInChildren = GetComponentsInChildren<CharacterControllerWorldMap>();
		if (m_SpawnAddition != null)
		{
			return m_SpawnAddition.AccomplishItem();
		}
		return 0f;
	}

	public float DespawnItem()
	{
		m_placedItem = null;
		m_AssociatedBattle = null;
		m_AssociatedTempHotspotGameData = null;
		CharacterControllerWorldMap[] componentsInChildren = GetComponentsInChildren<CharacterControllerWorldMap>();
		float result = 0f;
		if (m_SpawnAddition != null)
		{
			result = m_SpawnAddition.DespawnItem();
		}
		return result;
	}

	public void ShowTooltip()
	{
		if (m_placedItem == null)
		{
			DebugLog.Warn(GetType(), "ShowTooltip: No PlacedItem found in " + base.name + " (" + m_model.NameId + ")");
			return;
		}
		if (m_IsCollectible || m_IsEntrance)
		{
			string localizedText = DIContainerInfrastructure.GetLocaService().Tr(m_placedItem.BalancingData.LocaBaseId + "_tt");
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(base.transform, localizedText, false);
			return;
		}
		if (m_AssociatedBattle == null)
		{
			DebugLog.Warn(GetType(), "ShowTooltip: No AssociatedBattle found!");
			string firstPossibleBattle = DIContainerLogic.GetBattleService().GetFirstPossibleBattle(m_placedItem.BalancingData.EventParameters, DIContainerInfrastructure.GetCurrentPlayer());
			m_AssociatedBattle = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(firstPossibleBattle);
			m_AssociatedTempHotspotGameData = new HotspotGameData(new HotspotBalancingData
			{
				BattleId = m_placedItem.BalancingData.EventParameters,
				NameId = m_model.NameId + "_battleground",
				ZoneLocaIdent = m_placedItem.BalancingData.LocaBaseId + "_battleground",
				Type = HotspotType.Battle,
				ZoneStageIndex = 0,
				IsSpawnEventPossible = true
			});
		}
		if (m_AssociatedBattle != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowBattleOverlay(base.transform, m_AssociatedTempHotspotGameData, m_AssociatedBattle, m_placedItem.BalancingData.LocaBaseId + "_tt", false);
		}
	}

	public Bounds GetBoundingBox()
	{
		Collider component = GetComponent<Collider>();
		if (component != null)
		{
			return component.bounds;
		}
		return new Bounds(new Vector3(10000000f, 10000000f), Vector3.zero);
	}
}
