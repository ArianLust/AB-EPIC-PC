using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class HotSpotWorldMapViewBattle : HotSpotWorldMapViewBase
{
	protected HotSpotWorldMapAssetBase m_CurrentAssetBase;

	[SerializeField]
	protected HotSpotWorldMapAssetBase m_FullAssetBasePrefab;

	private HotSpotWorldMapAssetBase m_FullAssetBase;

	[SerializeField]
	protected HotSpotWorldMapAssetBase m_ProxyAssetBase;

	private int m_Performance;

	[SerializeField]
	private string m_OverrideLocaIdent;

	[SerializeField]
	private CharacterControllerWorldMap m_CharacterControllerWorldMapPrefab;

	private CharacterControllerWorldMap m_GoldenPig;

	private ChestPositioning m_ChestPositioning;

	public bool HasGoldenPig;

	[SerializeField]
	private bool ShowTooltipIfNotActive;

	private BattleBalancingData m_AssociatedBattle;

	[SerializeField]
	private bool m_IsDungeonHotspot;

	public override void Complete(HotSpotState state, bool startUp)
	{
	}

	public override void Initialize()
	{
		SetProxyOrFullAsset();
		base.Initialize();
	}

	private void SetProxyOrFullAsset()
	{
		if ((bool)m_FullAssetBasePrefab && (base.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew || base.Model.Data.UnlockState == HotspotUnlockState.Active))
		{
			if (!m_FullAssetBase)
			{
				m_CurrentAssetBase = Object.Instantiate(m_FullAssetBasePrefab);
				m_CurrentAssetBase.transform.parent = base.transform;
				m_CurrentAssetBase.transform.localPosition = Vector3.zero;
				m_FullAssetBase = m_CurrentAssetBase;
			}
			m_ProxyAssetBase.gameObject.SetActive(false);
		}
		else if ((bool)m_FullAssetBasePrefab && base.Model.Data.UnlockState == HotspotUnlockState.Resolved && IsDungeonHotSpot())
		{
			if (!m_FullAssetBase)
			{
				m_CurrentAssetBase = Object.Instantiate(m_FullAssetBasePrefab);
				m_CurrentAssetBase.transform.parent = base.transform;
				m_CurrentAssetBase.transform.localPosition = Vector3.zero;
				m_FullAssetBase = m_CurrentAssetBase;
			}
		}
		else
		{
			if (!m_ProxyAssetBase)
			{
				DebugLog.Log("No Battle Hotspot Asset assigned! This hotspot won't have a flag or animations.");
				return;
			}
			if (base.Model.Data.UnlockState == HotspotUnlockState.Hidden)
			{
				m_CurrentAssetBase = m_ProxyAssetBase;
				m_CurrentAssetBase.gameObject.SetActive(!m_CurrentAssetBase.HasActivateObjects());
			}
			else
			{
				m_CurrentAssetBase = m_ProxyAssetBase;
				m_ProxyAssetBase.gameObject.SetActive(true);
			}
		}
		m_CurrentAssetBase.SetModel(base.Model);
	}

	public override void ActivateAsset(bool activate)
	{
		if ((bool)m_CurrentAssetBase)
		{
			m_CurrentAssetBase.ActivateContainingAssets(activate);
		}
	}

	public void ShowTooltip()
	{
		if (ShowTooltipIfNotActive || base.Model.Data.UnlockState >= HotspotUnlockState.Active)
		{
			HotspotGameData currentHotspotGameData = DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.CurrentHotspotGameData;
			DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.CurrentHotspotGameData = base.Model;
			if (m_AssociatedBattle == null)
			{
				string firstPossibleBattle = DIContainerLogic.GetBattleService().GetFirstPossibleBattle(base.Model.BalancingData.BattleId, DIContainerInfrastructure.GetCurrentPlayer(), m_ChronicleCaveHotspot);
				m_AssociatedBattle = ((!m_ChronicleCaveHotspot) ? DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(firstPossibleBattle) : DIContainerBalancing.Service.GetBalancingData<ChronicleCaveBattleBalancingData>(firstPossibleBattle));
			}
			if (m_AssociatedBattle != null)
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowBattleOverlay(base.transform, base.Model, m_AssociatedBattle, m_OverrideLocaIdent, false);
			}
			if (m_MiniCampaignHotspot)
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowBattleOverlay(base.transform, base.Model, m_AssociatedBattle, base.Model.BalancingData.ZoneLocaIdent + "_tt", false);
			}
			DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.CurrentHotspotGameData = currentHotspotGameData;
		}
	}

	private IEnumerator PlayCompleteAnimAsync(bool startUp)
	{
		HotspotUnlockState unlockStateAtStart = base.Model.Data.UnlockState;
		if (unlockStateAtStart == HotspotUnlockState.ResolvedNew)
		{
			ExecuteActionTree execute2 = GetComponent<ExecuteActionTree>();
			if (execute2 != null)
			{
				execute2.SetStateMgr(DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr);
				if (execute2.m_executeBeforeUnlock)
				{
					DebugLog.Warn("StartAction'Tree before stage unlock");
					yield return StartCoroutine(PlayActionTree(execute2));
				}
			}
		}
		yield return StartCoroutine(ActivateFollowUpStagesAsync(GetPreviousHotspot(), null));
		if (unlockStateAtStart != HotspotUnlockState.ResolvedNew || base.Model.Data.UnlockState != HotspotUnlockState.Resolved)
		{
			yield break;
		}
		ExecuteActionTree execute = GetComponent<ExecuteActionTree>();
		if (execute != null)
		{
			execute.SetStateMgr(DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr);
			if (!execute.m_executeBeforeUnlock)
			{
				DebugLog.Warn("StartAction'Tree after stage unlock");
				yield return StartCoroutine(PlayActionTree(execute));
			}
		}
	}

	public override IEnumerator ActivateFollowUpStagesAsync(HotSpotWorldMapViewBase parentHotSpot, HotSpotWorldMapViewBase activateTo, bool instant = false)
	{
		if (base.Model == null)
		{
			DebugLog.Log("Model is null from " + m_nameId);
		}
		else
		{
			if (!DIContainerLogic.WorldMapService.IsHotspotVisible(DIContainerInfrastructure.GetCurrentPlayer(), base.Model))
			{
				yield break;
			}
			HotspotUnlockState unlockStateAtStart = base.Model.Data.UnlockState;
			if (unlockStateAtStart == HotspotUnlockState.ResolvedNew && !instant)
			{
				ExecuteActionTree execute2 = GetComponent<ExecuteActionTree>();
				if (execute2 != null)
				{
					execute2.SetStateMgr(DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr);
					if (execute2.m_executeBeforeUnlock)
					{
						DebugLog.Warn("StartAction'Tree before stage unlock");
						yield return StartCoroutine(PlayActionTree(execute2));
					}
				}
				StartCoroutine(DIContainerInfrastructure.LocationStateMgr.StoppablePopupCoroutine());
				while (DIContainerInfrastructure.LocationStateMgr.FeatureUnlocksRunning)
				{
					yield return new WaitForSeconds(0.1f);
				}
			}
			bool isAlreadyActive = false;
			switch (base.Model.Data.UnlockState)
			{
			case HotspotUnlockState.Resolved:
				if ((bool)m_ChestPositioning)
				{
					m_ChestPositioning.SetActive();
					m_ChestPositioning.SetOpenable();
				}
				break;
			case HotspotUnlockState.ResolvedBetter:
				if ((bool)m_ChestPositioning)
				{
					m_ChestPositioning.SetActive();
					m_ChestPositioning.SetOpenable();
				}
				break;
			case HotspotUnlockState.ResolvedNew:
				if ((bool)m_ChestPositioning)
				{
					m_ChestPositioning.SetActive();
					m_ChestPositioning.SetOpenable();
				}
				break;
			case HotspotUnlockState.Active:
				isAlreadyActive = true;
				if ((bool)m_ChestPositioning)
				{
					m_ChestPositioning.SetActive();
				}
				break;
			}
			if (m_CurrentAssetBase == null)
			{
				Debug.Log("Hotspot Asset wasn't set: " + base.Model.BalancingData.NameId);
			}
			else
			{
				#if ENABLE_IL2CPP
				m_CurrentAssetBase.HandleFlagBeforeUnlockOnState(base.Model.Data.UnlockState);
                #else
				yield return new WaitForEndOfFrame(); // janky fix for stack issues (for some reason the og game doesnt need this)
				m_CurrentAssetBase.HandleFlagBeforeUnlockOnState(base.Model.Data.UnlockState);
				#endif
			}
			yield return StartCoroutine(base.ActivateFollowUpStagesAsync(parentHotSpot, activateTo, instant));
			if (unlockStateAtStart == HotspotUnlockState.ResolvedNew && base.Model.Data.UnlockState == HotspotUnlockState.Resolved && !instant)
			{
				ExecuteActionTree execute = GetComponent<ExecuteActionTree>();
				if (execute != null)
				{
					execute.SetStateMgr(DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr);
					if (!execute.m_executeBeforeUnlock)
					{
						DebugLog.Warn("StartAction'Tree after stage unlock");
						yield return StartCoroutine(PlayActionTree(execute));
					}
				}
			}
			if (base.Model.Data.UnlockState == HotspotUnlockState.Active)
			{
				SetProxyOrFullAsset();
				if (DIContainerLogic.WorldMapService.HasHotspotLootChest(base.Model))
				{
					m_ChestPositioning = GetComponent<ChestPositioning>();
					if ((bool)m_ChestPositioning && m_ChestPositioning.SpawnChest() && base.Model.IsCompleted())
					{
						m_ChestPositioning.OnChestClicked -= m_ChestPositioning_OnChestClicked;
						m_ChestPositioning.OnChestClicked += m_ChestPositioning_OnChestClicked;
					}
				}
				if ((bool)m_CurrentAssetBase)
				{
					m_CurrentAssetBase.ActivateContainingAssets(true);
				}
				if (!isAlreadyActive)
				{
					if ((bool)m_ChestPositioning)
					{
						m_ChestPositioning.SetActive();
					}
					if ((bool)m_CurrentAssetBase)
					{
						m_CurrentAssetBase.PlaySetActiveAnimation();
					}
					Dictionary<string, string> values = new Dictionary<string, string> { { "HotspotName", m_nameId } };
					DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("HotspotActivated", values);
					if ((bool)m_CurrentAssetBase)
					{
						m_CurrentAssetBase.CreateInitialFlag();
					}
				}
			}
			HandleChains();
			DIContainerInfrastructure.LocationStateMgr.ResolveCutsceneError();
		}
	}

	private void HandleChains()
	{
		if (!HasChains())
		{
			return;
		}
		Requirement firstFailedReq = new Requirement();
		bool flag = DIContainerLogic.WorldMapService.IsHotspotEnterable(DIContainerInfrastructure.GetCurrentPlayer(), base.Model, out firstFailedReq);
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.TemporaryOpenHotspots != null && !DIContainerInfrastructure.GetCurrentPlayer().Data.TemporaryOpenHotspots.Contains(m_nameId))
		{
			if (!flag && firstFailedReq.RequirementType == RequirementType.IsSpecificWeekday)
			{
				SetChainsEnabled(true);
				return;
			}
			if (!flag && firstFailedReq.RequirementType == RequirementType.CooldownFinished)
			{
				SetChainsEnabled(true);
				return;
			}
		}
		SetChainsEnabled(false);
	}

	protected override void RemoveHandlers()
	{
		base.RemoveHandlers();
		if ((bool)m_ChestPositioning)
		{
			m_ChestPositioning.OnChestClicked -= m_ChestPositioning_OnChestClicked;
		}
	}

	protected override void InitialSetupHotspot()
	{
		base.InitialSetupHotspot();
		if (base.Model.Data.UnlockState > HotspotUnlockState.Hidden)
		{
			base.gameObject.SetActive(true);
			if (DIContainerLogic.WorldMapService.HasHotspotLootChest(base.Model))
			{
				m_ChestPositioning = GetComponent<ChestPositioning>();
				if ((bool)m_ChestPositioning && m_ChestPositioning.SpawnChest() && base.Model.IsCompleted())
				{
					m_ChestPositioning.OnChestClicked -= m_ChestPositioning_OnChestClicked;
					m_ChestPositioning.OnChestClicked += m_ChestPositioning_OnChestClicked;
				}
			}
			if (base.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew)
			{
				if ((bool)m_ChestPositioning)
				{
					m_ChestPositioning.SetActive();
					m_ChestPositioning.SetOpenable();
				}
			}
			else if (base.Model.Data.UnlockState == HotspotUnlockState.Resolved)
			{
				if ((bool)m_ChestPositioning)
				{
					m_ChestPositioning.SetActive();
					m_ChestPositioning.SetOpenable();
				}
				HandleChains();
			}
			else if (base.Model.Data.UnlockState == HotspotUnlockState.ResolvedBetter)
			{
				HandleChains();
				if ((bool)m_ChestPositioning)
				{
					m_ChestPositioning.SetActive();
					m_ChestPositioning.SetOpenable();
				}
			}
			else if (base.Model.Data.UnlockState == HotspotUnlockState.Active)
			{
				if ((bool)m_CurrentAssetBase)
				{
					m_CurrentAssetBase.ActivateContainingAssets(true);
				}
				if ((bool)m_CurrentAssetBase)
				{
					m_CurrentAssetBase.PlayActiveIdleAnimation();
				}
				if ((bool)m_CurrentAssetBase)
				{
					m_CurrentAssetBase.CreateInitialFlag();
				}
				if ((bool)m_ChestPositioning)
				{
					m_ChestPositioning.SetActive();
				}
				HandleChains();
				DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("hotspot_available", m_nameId);
			}
			else if ((base.Model.Data.UnlockState == HotspotUnlockState.Hidden || base.Model.Data.UnlockState == HotspotUnlockState.Unknown) && (bool)m_CurrentAssetBase)
			{
				m_CurrentAssetBase.ActivateContainingAssets(false);
			}
			if ((bool)m_CurrentAssetBase)
			{
				m_CurrentAssetBase.HandleFlagAfterUnlockOnState(base.Model.Data.UnlockState);
			}
			m_Performance = base.Model.Data.StarCount;
		}
		else if (DIContainerLogic.WorldMapService.HasHotspotLootChest(base.Model))
		{
			base.gameObject.SetActive(true);
			m_ChestPositioning = GetComponent<ChestPositioning>();
			if ((bool)m_ChestPositioning && m_ChestPositioning.SpawnChest() && base.Model.IsCompleted())
			{
				m_ChestPositioning.OnChestClicked -= m_ChestPositioning_OnChestClicked;
				m_ChestPositioning.OnChestClicked += m_ChestPositioning_OnChestClicked;
			}
			if ((bool)m_CurrentAssetBase)
			{
				m_CurrentAssetBase.ActivateContainingAssets(false);
			}
		}
	}

	private void m_ChestPositioning_OnChestClicked()
	{
		if (DIContainerLogic.WorldMapService.HasHotspotLootChest(base.Model) && base.Model.IsCompleted())
		{
			Dictionary<string, LootInfoData> loot = DIContainerLogic.GetLootOperationService().GenerateLoot(base.Model.BalancingData.HotspotContents, 1);
			List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(loot, EquipmentSource.LootBird, true);
			DIContainerLogic.WorldMapService.LootedHotspotChest(base.Model);
			if ((bool)m_ChestPositioning && m_ChestPositioning.RemoveChest(itemsFromLoot))
			{
				DebugLog.Log("Hotspot Chest Looted");
			}
			DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 2, loot, "hotspot_chest");
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		}
	}

	private IEnumerator GoldenPigCheer(CharacterControllerWorldMap goldenPig, int p)
	{
		yield return new WaitForSeconds(p);
		if (!goldenPig.m_AssetController.GetComponent<Animation>().IsPlaying("Surprised"))
		{
			goldenPig.PlayCheerCharacter();
		}
		StartCoroutine(GoldenPigCheer(goldenPig, p));
	}

	protected override void HotSpotChanged(bool startUp)
	{
		base.HotSpotChanged(startUp);
		CompleteHotspot();
	}

	private void CompleteStartUpHotspot()
	{
		if (base.Model.IsCompleted() && base.Model.GetStarCount() > m_Performance)
		{
			m_Performance = base.Model.GetStarCount();
			Complete(m_state, true);
		}
	}

	private void CompleteHotspot()
	{
		if (base.Model.IsCompleted())
		{
			m_Performance = base.Model.GetStarCount();
			Complete(m_state, base.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew);
		}
	}

	public override void HandleMouseButtonUp(bool directMove = false)
	{
		if (m_MiniCampaignHotspot && DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()) && DIContainerLogic.EventSystemService.IsWaitingForConfirmation(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData))
		{
			DIContainerLogic.EventSystemService.CheckoutClicked(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData);
			return;
		}
		Requirement firstFailedReq = null;
		if (!DIContainerLogic.WorldMapService.CanTravelToHotspot(DIContainerInfrastructure.GetCurrentPlayer(), base.Model, out firstFailedReq))
		{
			if (firstFailedReq.RequirementType == RequirementType.HaveItem)
			{
				IInventoryItemGameData inventoryItemGameData = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, firstFailedReq.NameId, (int)firstFailedReq.Value);
				UIAtlas uIAtlas = null;
				if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(inventoryItemGameData.ItemIconAtlasName))
				{
					GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(inventoryItemGameData.ItemIconAtlasName) as GameObject;
					uIAtlas = gameObject.GetComponent<UIAtlas>();
				}
			}
		}
		else
		{
			base.HandleMouseButtonUp(m_IsDungeonHotspot);
			if ((bool)m_GoldenPig)
			{
				m_GoldenPig.PlaySuprisedAnimation();
			}
		}
	}

	public override void ShowContentView()
	{
		Requirement firstFailedReq = null;
		if (!DIContainerLogic.WorldMapService.CanTravelToHotspot(DIContainerInfrastructure.GetCurrentPlayer(), base.Model, out firstFailedReq))
		{
			if (firstFailedReq.RequirementType == RequirementType.HaveItem)
			{
				IInventoryItemGameData inventoryItemGameData = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, firstFailedReq.NameId, (int)firstFailedReq.Value);
				UIAtlas uIAtlas = null;
				if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(inventoryItemGameData.ItemIconAtlasName))
				{
					GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(inventoryItemGameData.ItemIconAtlasName) as GameObject;
					uIAtlas = gameObject.GetComponent<UIAtlas>();
				}
			}
			return;
		}
		base.ShowContentView();
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_bps") == 0 && DIContainerInfrastructure.GetCurrentPlayer().Birds.Count < 4)
		{
			List<BirdGameData> source = new List<BirdGameData>(DIContainerInfrastructure.GetCurrentPlayer().Birds);
			source = source.OrderBy((BirdGameData b) => b.BalancingData.SortPriority).ToList();
			DIContainerInfrastructure.LocationStateMgr.StartBattle(base.Model, source, null);
		}
		else
		{
			DIContainerInfrastructure.LocationStateMgr.ShowBattlePreperationScreen();
		}
	}

	public void SetChainsEnabled(bool p)
	{
		if ((bool)m_CurrentAssetBase)
		{
			m_CurrentAssetBase.SetChainsEnabled(p);
		}
	}

	private bool HasChains()
	{
		if ((bool)m_CurrentAssetBase)
		{
			return m_CurrentAssetBase.HasChains();
		}
		return false;
	}

	public override bool IsDungeonHotSpot()
	{
		return m_IsDungeonHotspot;
	}

	public void SetGoldenPig(bool pigActive = true)
	{
		if (!pigActive)
		{
			DebugLog.Log("de-spawn golden pig!");
			Object.Destroy(m_GoldenPig);
			HasGoldenPig = false;
			return;
		}
		if (HasGoldenPig && m_GoldenPig != null)
		{
			DebugLog.Log(GetType(), "SetGoldenPig: Golden Pig already set, stepping out...");
			return;
		}
		HasGoldenPig = true;
		if ((bool)m_CharacterControllerWorldMapPrefab && m_GoldenPig == null)
		{
			m_GoldenPig = Object.Instantiate(m_CharacterControllerWorldMapPrefab);
		}
		PigBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<PigBalancingData>("pig_golden_pig");
		if (!(m_GoldenPig == null))
		{
			m_GoldenPig.SetModel(balancingData.NameId);
			m_GoldenPig.transform.parent = base.transform;
			m_GoldenPig.transform.localPosition = m_HotSpotPositions[m_HotSpotPositions.Length - 1];
			m_GoldenPig.transform.localScale = Vector3.Scale(m_GoldenPig.transform.localScale, DIContainerInfrastructure.LocationStateMgr.GetWorldBirdScale());
			StartCoroutine(CheerCharacterRepeating(m_GoldenPig));
			UnityHelper.SetLayerRecusively(m_GoldenPig.gameObject, base.gameObject.layer);
		}
	}

	private IEnumerator CheerCharacterRepeating(CharacterControllerWorldMap character)
	{
		if ((bool)character)
		{
			yield return new WaitForSeconds(Random.Range(1f, 5f));
			yield return new WaitForSeconds(character.PlayCheerCharacter());
		}
	}
}
