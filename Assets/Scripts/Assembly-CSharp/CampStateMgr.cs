using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class CampStateMgr : BaseCampStateMgr
{
	[HideInInspector]
	public MenuCrafting m_ForgeWindow;

	[HideInInspector]
	public CraftingResultUI m_ForgeResultWindow;

	[HideInInspector]
	public CampMenuUI m_CampUI;

	[HideInInspector]
	public FriendCampMenuUI m_FriendCampUI;

	public List<CampProp> m_ForgeCampLeveled;

	public List<CampProp> m_CauldronCampLeveled;

	public CampProp m_GoldenChiliCamp;

	private int m_ForgeLevel;

	private int m_CauldronLevel;

	[SerializeField]
	public CampProp m_ForgeProp;

	[SerializeField]
	public CampProp m_CauldronProp;

	[SerializeField]
	private CampProp m_DungeonInfoButton;

	[SerializeField]
	private GameObject m_FreeDungeonSign;

	[SerializeField]
	private CampProp m_ProgressInfoButton;

	private List<DungeonInfoPopup.DungeonInfo> m_dungeonList;

	private bool m_startProgresswithStarCollec;

	private CraftingMenuType m_cachedCraftingType;

	private int m_firstDungeonProgressID = 44;

	private void Awake()
	{
		DIContainerInfrastructure.AdService.AddPlacement(GachaPopupUI.GACHA_PLACEMENT);
		DIContainerInfrastructure.GetCoreStateMgr().m_SceneryAudioListener = base.transform.GetComponentInChildren<AudioListener>();
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.ForceLoading = true;
		DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP = false;
		m_CampProps.AddRange(m_ForgeCampLeveled);
		m_CampProps.AddRange(m_CauldronCampLeveled);
		if (m_FriendInfo != null && ClientInfo.IsFriend)
		{
			if (ClientInfo.InspectedFriend.PublicPlayerData.Inventory != null)
			{
				InventoryGameData inventory = new InventoryGameData(ClientInfo.InspectedFriend.PublicPlayerData.Inventory);
				int itemValue = DIContainerLogic.InventoryService.GetItemValue(inventory, "star_collection");
				if (itemValue > 0)
				{
					m_StarCollectionLabel.text = itemValue.ToString();
				}
				else
				{
					m_StarCollectionLabel.transform.parent.gameObject.SetActive(false);
				}
			}
			else
			{
				m_StarCollectionLabel.transform.parent.gameObject.SetActive(false);
			}
		}
		else
		{
			m_StarCollectionLabel.text = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "star_collection").ToString();
		}
		if ((bool)m_ShopCamp)
		{
			m_CampProps.Add(m_ShopCamp);
		}
		if ((bool)m_GoldenPigCamp)
		{
			m_CampProps.Add(m_GoldenPigCamp);
		}
		if ((bool)m_MailBoxCamp)
		{
			m_CampProps.Add(m_MailBoxCamp);
		}
		if ((bool)m_FriendListCamp)
		{
			m_CampProps.Add(m_FriendListCamp);
		}
		if ((bool)m_RovioIdCamp)
		{
			m_CampProps.Add(m_RovioIdCamp);
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr = this;
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			ContentLoader.Instance.CheckforRestartApp();
		}
	}

	private IEnumerator Start()
	{
		CheckForAdvancedGacha();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.IsInBattle = false;
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.ResetRegistration();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("camp_enter");
		DIContainerInfrastructure.AudioManager.PlayMusic("music_camp");
		if (DIContainerInfrastructure.PurchasingService.IsSupported() && !DIContainerInfrastructure.PurchasingService.IsInitializing() && !DIContainerInfrastructure.PurchasingService.IsInitialized() && !string.IsNullOrEmpty(DIContainerConfig.GetClientConfig().BundleId))
		{
			DIContainerInfrastructure.PurchasingService.Initialize(DIContainerConfig.GetClientConfig().BundleId);
		}
		if ((bool)m_FriendInfo && ClientInfo.IsFriend)
		{
			m_FriendInfo.SetDefault();
			m_FriendInfo.SetModel(ClientInfo.InspectedFriend);
		}
		m_LoadedLevels.Add("Menu_Camp", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Menu_Camp", OnMenuCampLoaded);
		if (DIContainerInfrastructure.GetCurrentPlayer().GetCurrentWorldProgress() < m_firstDungeonProgressID)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Popup_DungeonsLocked");
		}
		yield return new WaitForEndOfFrame();
		m_Birds = ClientInfo.CurrentCampBirds;
		DebugLog.Log("Birds: " + m_Birds.Count);
		DebugLog.Log("BirdPositions: " + m_BirdPositionsByCount.Count);
		if (m_Birds.Count > 0)
		{
			List<Vector3> positions = ((m_BirdPositionsByCount.Count <= m_Birds.Count - 1) ? m_BirdPositionsByCount[0].Vectors : m_BirdPositionsByCount[m_Birds.Count - 1].Vectors);
			for (int i = 0; i < m_Birds.Count; i++)
			{
				yield return new WaitForEndOfFrame();
				BirdGameData bird = m_Birds[i];
				CharacterControllerCamp characterController = Object.Instantiate(m_CharacterCampPrefab);
				characterController.SetModel(bird);
				characterController.transform.parent = m_CharacterRoot;
				characterController.transform.localPosition = positions[i];
				if (m_IsBirdMirrored.Count > i && m_IsBirdMirrored[i])
				{
					characterController.transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				m_CharactersCamp.Add(characterController);
			}
		}
		DeRegisterEventHandler();
		foreach (CampProp prop in m_CampProps)
		{
			if (!prop.m_IsInitialized)
			{
				prop.Awake();
			}
		}
		UpdateLoggedInIndicator();
		while (m_LoadedLevels.Values.Count((bool e) => !e) > 0 || m_CampProps.Count((CampProp p) => !p.m_IsInitialized) > 0)
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_DailyLoginUi.StopEnterCoroutine();
		RefreshLeveledForge();
		RefreshLeveledCauldron();
		m_ForgeProp = ((m_ForgeLevel <= 0 || m_ForgeCampLeveled.Count < m_ForgeLevel) ? null : m_ForgeCampLeveled[m_ForgeLevel - 1]);
		if (m_ForgeProp != null && m_ForgeProp.GetModel() != null)
		{
			m_ForgeProp.GetModel().Data.IsNew = m_ForgeProp.GetModel().Data.IsNew || ClientInfo.CurrentCampInventory.HasNewItemForge();
			m_ForgeProp.CheckUpdateIndicator();
		}
		m_CauldronProp = ((m_CauldronLevel <= 0 || m_CauldronCampLeveled.Count < m_CauldronLevel) ? null : m_CauldronCampLeveled[m_CauldronLevel - 1]);
		if (m_CauldronProp != null && m_CauldronProp.GetModel() != null)
		{
			m_CauldronProp.GetModel().Data.IsNew |= ClientInfo.CurrentCampInventory.HasNewItemAlchemy();
			m_CauldronProp.CheckUpdateIndicator();
		}
		if ((bool)m_MailBoxCamp)
		{
			m_MailBoxCamp.SetCounter(GetViewableMessagesCount());
		}
		if ((bool)m_FriendListCamp)
		{
			m_FriendListCamp.SetCounter(0);
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Leave();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 0u,
			showFriendshipEssence = true,
			showLuckyCoins = true,
			showSnoutlings = true
		}, true);
		if (!ClientInfo.IsFriend)
		{
			RefreshBirdMarkers();
			CheckForPiggieMcCoolVisits();
			m_dungeonList = SetupDungeonInfos();
		}
		DeRegisterEventHandler();
		RegisterEventHandler();
		InvokeRepeating("HandleShopDisplay", 1f, 10f);
		DebugLog.Log("Camp Initialized!");
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enter_camp", string.Empty);
		DIContainerLogic.SocialService.UpdateFreeGachaRolls(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData);
		string hotLinkName2 = DIContainerInfrastructure.GetCoreStateMgr().m_HotLinkItemName;
		if (!string.IsNullOrEmpty(hotLinkName2))
		{
			switch (hotLinkName2)
			{
			case "RovioId":
				GoToSocial(SocialWindowCategory.RovioId);
				hotLinkName2 = null;
				break;
			case "story_cauldron":
				GoToAlchemy();
				hotLinkName2 = null;
				break;
			case "story_forge":
				GoToForge();
				hotLinkName2 = null;
				break;
			case "story_goldenpig":
				GoToGacha();
				hotLinkName2 = null;
				break;
			default:
				if (hotLinkName2.StartsWith("bird_manager"))
				{
					string birdId = hotLinkName2.Split(':')[1];
					GoToBirdManager(birdId);
					hotLinkName2 = null;
				}
				else if (hotLinkName2.StartsWith("Starcollection"))
				{
					ShowProgressUI(false, true);
					hotLinkName2 = null;
				}
				else
				{
					StartCoroutine(ShowShop());
				}
				break;
			}
		}
		InvokeRepeating("UpdateFreeGachaSign", 1f, 10f);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("camp_enter");
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.ForceLoading = false;
		ContentLoader.Instance.CheckforRestartApp();
		yield return new WaitForSeconds(1f);
		EventuallyShowGooglePlaySignIn();
	}

	private List<DungeonInfoPopup.DungeonInfo> SetupDungeonInfos()
	{
		List<HotspotBalancingData> source = DIContainerBalancing.Service.GetBalancingDataList<HotspotBalancingData>().ToList();
		List<HotspotBalancingData> dungeonHotSpots = source.Where((HotspotBalancingData h) => h.NameId.Contains("_dungeon") && !(h is ChronicleCaveHotspotBalancingData) && h.EnterRequirements != null && h.EnterRequirements.FirstOrDefault().RequirementType == RequirementType.IsSpecificWeekday).ToList();
		List<DungeonInfoPopup.DungeonInfo> list = new List<DungeonInfoPopup.DungeonInfo>();
		dungeonHotSpots = OrderHotspotsByWeekday(dungeonHotSpots);
		bool flag = false;
		for (int i = 0; i < dungeonHotSpots.Count; i++)
		{
			HotspotBalancingData hotspotBalancingData = dungeonHotSpots[i];
			DungeonInfoPopup.DungeonInfo item = default(DungeonInfoPopup.DungeonInfo);
			item.LocalizedName = DIContainerInfrastructure.GetLocaService().GetZoneName(hotspotBalancingData.ZoneLocaIdent);
			BattleBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(hotspotBalancingData.BattleId.LastOrDefault());
			BattleBalancingData balancingData2 = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(hotspotBalancingData.BattleId.LastOrDefault() + "_hard");
			int num = 0;
			int level = DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
			ExperienceLevelBalancingData balancing;
			if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + level.ToString("00"), out balancing) || DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + (level - 1).ToString("00"), out balancing))
			{
				num = balancing.MatchmakingRangeIndex;
			}
			string nameId = balancingData.LootTableWheel.FirstOrDefault().Key.Replace("{levelrange}", num.ToString("00"));
			string nameId2 = balancingData2.LootTableWheel.FirstOrDefault().Key.Replace("{levelrange}", num.ToString("00"));
			LootTableBalancingData balancingData3 = DIContainerBalancing.Service.GetBalancingData<LootTableBalancingData>(nameId);
			LootTableBalancingData balancingData4 = DIContainerBalancing.Service.GetBalancingData<LootTableBalancingData>(nameId2);
			if (balancingData3.LootTableEntries != null && balancingData3.LootTableEntries.Count > 2)
			{
				item.lowPrice = balancingData3.LootTableEntries[0].BaseValue + balancingData3.LootTableEntries[1].BaseValue + balancingData3.LootTableEntries[2].BaseValue;
			}
			else
			{
				Debug.LogError("No low loot table for dungeon " + hotspotBalancingData.NameId);
			}
			if (balancingData4.LootTableEntries != null && balancingData4.LootTableEntries.Count > 2)
			{
				item.highPrice = balancingData4.LootTableEntries[0].BaseValue + balancingData4.LootTableEntries[1].BaseValue + balancingData4.LootTableEntries[2].BaseValue;
			}
			else
			{
				Debug.LogError("No hard loot table for dungeon " + hotspotBalancingData.NameId);
			}
			if (DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing != null && DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing.BonusType == BonusEventType.DungeonBonus)
			{
				float num2 = DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing.BonusFactor / 100f;
				item.lowPrice += (int)((float)item.lowPrice * num2);
				item.highPrice += (int)((float)item.highPrice * num2);
			}
			HotspotGameData value = null;
			if (!DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(hotspotBalancingData.NameId, out value))
			{
				DebugLog.Error("Could not find hot spot for " + hotspotBalancingData.NameId);
				continue;
			}
			Requirement firstFailedReq = new Requirement();
			bool flag2 = DIContainerLogic.WorldMapService.IsHotspotEnterable(DIContainerInfrastructure.GetCurrentPlayer(), value, out firstFailedReq);
			bool isFree = true;
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.TemporaryOpenHotspots != null && !DIContainerInfrastructure.GetCurrentPlayer().Data.TemporaryOpenHotspots.Contains(hotspotBalancingData.NameId))
			{
				if (!flag2 && firstFailedReq.RequirementType == RequirementType.IsSpecificWeekday)
				{
					isFree = false;
				}
				if (!flag2 && firstFailedReq.RequirementType == RequirementType.CooldownFinished)
				{
					isFree = false;
				}
			}
			item.isFree = isFree;
			item.isLocked = value.Data.UnlockState == HotspotUnlockState.Hidden;
			flag = (item.isFree && !item.isLocked) || flag;
			list.Add(item);
		}
		m_FreeDungeonSign.SetActive(flag);
		return list;
	}

	private List<HotspotBalancingData> OrderHotspotsByWeekday(List<HotspotBalancingData> DungeonHotSpots)
	{
		HotspotBalancingData[] array = new HotspotBalancingData[7];
		for (int i = 0; i < DungeonHotSpots.Count; i++)
		{
			HotspotBalancingData hotspotBalancingData = DungeonHotSpots[i];
			if (hotspotBalancingData.EnterRequirements.FirstOrDefault().NameId == "monday")
			{
				array[0] = hotspotBalancingData;
			}
			else if (hotspotBalancingData.EnterRequirements.FirstOrDefault().NameId == "tuesday")
			{
				array[1] = hotspotBalancingData;
			}
			else if (hotspotBalancingData.EnterRequirements.FirstOrDefault().NameId == "wednesday")
			{
				array[2] = hotspotBalancingData;
			}
			else if (hotspotBalancingData.EnterRequirements.FirstOrDefault().NameId == "thursday")
			{
				array[3] = hotspotBalancingData;
			}
			else if (hotspotBalancingData.EnterRequirements.FirstOrDefault().NameId == "friday")
			{
				array[4] = hotspotBalancingData;
			}
			else if (hotspotBalancingData.EnterRequirements.FirstOrDefault().NameId == "saturday")
			{
				array[5] = hotspotBalancingData;
			}
			else if (hotspotBalancingData.EnterRequirements.FirstOrDefault().NameId == "sunday")
			{
				array[6] = hotspotBalancingData;
			}
		}
		return array.ToList();
	}

	private void HandleShopDisplay()
	{
		if ((bool)m_ShopCamp)
		{
			m_ShopCamp.DisableUpdateIndikator();
			m_ShopCamp.CheckSaleIndikator();
		}
	}

	private IEnumerator ShowShop()
	{
		while (DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.IsLoading())
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop(DIContainerInfrastructure.GetCoreStateMgr().m_HotLinkItemName, delegate
		{
		}, 0, false, DIContainerInfrastructure.GetCoreStateMgr().m_ShopEnterSource);
		DIContainerInfrastructure.GetCoreStateMgr().m_HotLinkItemName = null;
		DIContainerInfrastructure.GetCoreStateMgr().m_ShopEnterSource = "Standard";
	}

	public void OnMenuCampLoaded()
	{
		m_LoadedLevels["Menu_Camp"] = true;
		m_CampUI = Object.FindObjectOfType(typeof(CampMenuUI)) as CampMenuUI;
		m_CampUI.SetCampStateMgr(this);
		DebugLog.Log("Menu_Camp loaded!");
	}

	public void RefreshLeveledCauldron()
	{
		IInventoryItemGameData data = null;
		if (!DIContainerLogic.InventoryService.TryGetItemGameData(ClientInfo.CurrentCampInventory, "cauldron_leveled", out data))
		{
			return;
		}
		m_CauldronLevel = data.ItemData.Level;
		for (int i = 0; i < m_CauldronCampLeveled.Count; i++)
		{
			if (i + 1 == m_CauldronLevel)
			{
				m_CauldronCampLeveled[i].gameObject.SetActive(true);
			}
			else
			{
				m_CauldronCampLeveled[i].gameObject.SetActive(false);
			}
		}
	}

	private void OpenDungeonInfo(BasicItemGameData whyThisParamIDontEvent = null)
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().GetCurrentWorldProgress() < m_firstDungeonProgressID && DIContainerInfrastructure.GetCoreStateMgr().m_DungeonsLockedPopup != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_DungeonsLockedPopup.ShowDungeonLockedPopup();
		}
		else if (m_DungeonUI != null)
		{
			m_DungeonUI.Show(m_dungeonList);
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_DailyDungeonInfo", OnDungeonInfoLoaded);
		}
	}

	public void OnDungeonInfoLoaded()
	{
		m_DungeonUI = Object.FindObjectOfType(typeof(DungeonInfoPopup)) as DungeonInfoPopup;
		m_DungeonUI.Show(m_dungeonList);
	}

	private void OpenProgressInfo(BasicItemGameData whyThisParamIDontEvent = null)
	{
		ShowProgressUI(ClientInfo.IsFriend, false);
	}

	private void ShowProgressUI(bool isFriend, bool starcollec)
	{
		if (m_ProgressUI == null)
		{
			m_startProgresswithStarCollec = starcollec;
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_GameProgress", OnGameProgressLoaded);
		}
		else
		{
			m_ProgressUI.Show(isFriend, starcollec);
		}
	}

	public void OnGameProgressLoaded()
	{
		m_ProgressUI = Object.FindObjectOfType(typeof(GameProgressPopup)) as GameProgressPopup;
		m_ProgressUI.Show(ClientInfo.IsFriend, m_startProgresswithStarCollec);
	}

	private void RefreshLeveledForge()
	{
		IInventoryItemGameData data = null;
		if (!DIContainerLogic.InventoryService.TryGetItemGameData(ClientInfo.CurrentCampInventory, "forge_leveled", out data))
		{
			return;
		}
		m_ForgeLevel = data.ItemData.Level;
		for (int i = 0; i < m_ForgeCampLeveled.Count; i++)
		{
			if (i + 1 == m_ForgeLevel)
			{
				m_ForgeCampLeveled[i].gameObject.SetActive(true);
			}
			else
			{
				m_ForgeCampLeveled[i].gameObject.SetActive(false);
			}
		}
	}

	private void OnCraftingLoaded()
	{
		m_ForgeWindow = Object.FindObjectOfType(typeof(MenuCrafting)) as MenuCrafting;
		m_ForgeWindow.gameObject.SetActive(true);
		m_ForgeWindow.SetCampStateMgr(this);
		m_ForgeWindow.Enter(m_cachedCraftingType);
	}

	private void OnCraftingPopupLoaded()
	{
		m_ForgeResultWindow = Object.FindObjectOfType(typeof(CraftingResultUI)) as CraftingResultUI;
		m_ForgeResultWindow.gameObject.SetActive(false);
	}

	private void LeveledCauldronItemDataChanged(IInventoryItemGameData item, float value)
	{
		RefreshLeveledCauldron();
	}

	private void LeveledForgeItemDataChanged(IInventoryItemGameData item, float value)
	{
		RefreshLeveledForge();
	}

	public override void RegisterEventHandler()
	{
		base.RegisterEventHandler();
		IInventoryItemGameData data = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "forge_leveled", out data))
		{
			data.ItemDataChanged += LeveledForgeItemDataChanged;
		}
		IInventoryItemGameData data2 = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "cauldron_leveled", out data2))
		{
			data2.ItemDataChanged += LeveledCauldronItemDataChanged;
		}
		if ((bool)m_DungeonInfoButton)
		{
			m_DungeonInfoButton.OnPropClicked += OpenDungeonInfo;
		}
		if ((bool)m_ProgressInfoButton)
		{
			m_ProgressInfoButton.OnPropClicked += OpenProgressInfo;
		}
		foreach (CampProp item in m_CauldronCampLeveled)
		{
			item.OnPropClicked += CauldronCampOnPropClicked;
		}
		foreach (CampProp item2 in m_ForgeCampLeveled)
		{
			item2.OnPropClicked += ForgeCampOnPropClicked;
		}
	}

	public override void DeRegisterEventHandler()
	{
		base.DeRegisterEventHandler();
		IInventoryItemGameData data = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "forge_leveled", out data))
		{
			data.ItemDataChanged -= LeveledForgeItemDataChanged;
		}
		IInventoryItemGameData data2 = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "cauldron_leveled", out data2))
		{
			data2.ItemDataChanged -= LeveledCauldronItemDataChanged;
		}
		if ((bool)m_DungeonInfoButton)
		{
			m_DungeonInfoButton.OnPropClicked -= OpenDungeonInfo;
		}
		if ((bool)m_ProgressInfoButton)
		{
			m_ProgressInfoButton.OnPropClicked -= OpenProgressInfo;
		}
		foreach (CampProp item in m_CauldronCampLeveled)
		{
			item.OnPropClicked -= CauldronCampOnPropClicked;
		}
		foreach (CampProp item2 in m_ForgeCampLeveled)
		{
			item2.OnPropClicked -= ForgeCampOnPropClicked;
		}
	}

	private void ForgeCampOnPropClicked(BasicItemGameData item)
	{
		OnForgeClicked();
	}

	private void CauldronCampOnPropClicked(BasicItemGameData obj)
	{
		OnAlchemyClicked();
	}

	private void OnForgeClicked()
	{
		GoToForge();
	}

	private void OnAlchemyClicked()
	{
		GoToAlchemy();
	}

	public void GoToForge()
	{
		if (!ClientInfo.IsFriend)
		{
			if (m_ForgeWindow == null)
			{
				m_cachedCraftingType = CraftingMenuType.Forge;
				DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_Crafting", OnCraftingLoaded);
				DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Popup_Crafting", OnCraftingPopupLoaded);
			}
			else
			{
				m_ForgeWindow.gameObject.SetActive(true);
				m_ForgeWindow.Enter(CraftingMenuType.Forge);
			}
		}
	}

	public void GoToAlchemy()
	{
		if (!ClientInfo.IsFriend)
		{
			if (m_ForgeWindow == null)
			{
				m_cachedCraftingType = CraftingMenuType.Alchemy;
				DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_Crafting", OnCraftingLoaded);
				DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Popup_Crafting", OnCraftingPopupLoaded);
			}
			if (m_ForgeWindow != null)
			{
				m_ForgeWindow.gameObject.SetActive(true);
				m_ForgeWindow.Enter(CraftingMenuType.Alchemy);
			}
		}
	}

	public void GoToBirdManager(string birdNameId)
	{
		if (ClientInfo.IsFriend)
		{
			return;
		}
		if (m_BirdManager == null)
		{
			m_birdName = birdNameId;
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_BirdManager", base.OnBirdManagerLoaded);
			return;
		}
		BirdGameData birdGameData = m_Birds.Where((BirdGameData b) => b.BalancingData.NameId == birdNameId).FirstOrDefault();
		if (birdGameData != null)
		{
			m_BirdManager.SetStateMgr(this).SetModel(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_Birds, m_Birds.IndexOf(birdGameData));
		}
	}

	public void ShowCraftingResult(IInventoryItemGameData item, CraftingRecipeGameData recipe, int amount = 1)
	{
		m_ForgeResultWindow.SetStateMgr(this);
		m_ForgeResultWindow.SetItem(item, recipe);
		m_ForgeResultWindow.Enter(true, amount);
	}

	private void OnDisable()
	{
		PigBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<PigBalancingData>("pig_instructor");
		BirdBalancingData balancingData2 = DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>("bird_instructor");
		DIContainerInfrastructure.GetCharacterAssetProvider(false).DestroyCachedObjects(balancingData.AssetId);
		DIContainerInfrastructure.GetCharacterAssetProvider(false).DestroyCachedObjects(balancingData2.AssetId);
		DIContainerInfrastructure.GetCoreStateMgr().UnloadUnusedAssets();
		DeRegisterEventHandler();
	}

	internal void ForceAddCauldron()
	{
		RefreshLeveledCauldron();
		CampProp campProp = m_CauldronCampLeveled.FirstOrDefault();
		if (campProp != null)
		{
			campProp.Awake();
		}
		RegisterEventHandler();
	}

	internal void ForceAddChili()
	{
		m_GoldenChiliCamp.Awake();
	}

	public void OnMenuFriendCampLoaded()
	{
		m_LoadedLevels["Menu_FriendCamp"] = true;
		m_FriendCampUI = Object.FindObjectOfType(typeof(FriendCampMenuUI)) as FriendCampMenuUI;
		m_FriendCampUI.SetCampStateMgr(this);
		DebugLog.Log("Menu_FriendCamp loaded!");
	}
}
