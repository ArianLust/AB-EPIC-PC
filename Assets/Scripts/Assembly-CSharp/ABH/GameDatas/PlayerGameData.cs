using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Character;
using ABH.Shared.Models.Generic;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

namespace ABH.GameDatas
{
	public class PlayerGameData : GameDataBase<IBalancingData, PlayerData>
	{
		public bool FireLostUnlockFeaturePopup;

		public bool m_FirstTutorialTracked;

		public string m_LastBattle;

		public bool m_UnlockDailyCalendarSessionFlag;

		private ChronicleCaveGameData m_ChronicleCaveGameData;

		private List<BirdGameData> m_Birds;

		public string m_authToken;

		private float m_timeTillPublicPlayerUpdate = 60f;

		private float m_lastPublicPlayerUpdate;

		private PublicPlayerData m_publicPlayer;

		public IInventoryItemGameData RolledChestReward;

		public InventoryGameData InventoryGameData { get; set; }

		public WorldGameData WorldGameData { get; set; }

		public SocialEnvironmentGameData SocialEnvironmentGameData { get; set; }

		public BannerGameData BannerGameData { get; set; }

		public ChronicleCaveGameData ChronicleCaveGameData
		{
			get
			{
				if (m_ChronicleCaveGameData == null && Data.ChronicleCave == null)
				{
					m_ChronicleCaveGameData = new ChronicleCaveGameData(string.Empty);
					Data.ChronicleCave = m_ChronicleCaveGameData.Data;
				}
				else if (m_ChronicleCaveGameData == null)
				{
					m_ChronicleCaveGameData = new ChronicleCaveGameData(Data.ChronicleCave);
				}
				return m_ChronicleCaveGameData;
			}
		}

		public List<BirdGameData> Birds
		{
			get
			{
				return m_Birds.Where((BirdGameData b) => !b.Data.IsUnavaliable).ToList();
			}
			set
			{
				m_Birds = value;
			}
		}

		public List<BirdGameData> AllBirds
		{
			get
			{
				return m_Birds;
			}
		}

		public EventManagerGameData CurrentEventManagerGameData { get; set; }

		public PvPSeasonManagerGameData CurrentPvPSeasonGameData { get; set; }

		public bool IsEventResultPending { get; private set; }

		public bool IsPvPResultPending { get; private set; }

		public PublicPlayerData PublicPlayer
		{
			get
			{
				return _instancedData.GetPublicPlayerData();
			}
		}

		public List<int> SelectedBirdIndices
		{
			get
			{
				List<int> list = Data.SelectedBirdIndices;
				if (list == null)
				{
					List<int> list2 = new List<int>();
					list2.Add(0);
					list2 = list2;
					Data.SelectedBirdIndices = list2;
					list = list2;
				}
				return list;
			}
			set
			{
				Data.SelectedBirdIndices = value;
			}
		}

		[method: MethodImpl(32)]
		public event Action<int> CharacterLevelChanged;

		[method: MethodImpl(32)]
		public event Action<int, int> ExperienceChanged;

		[method: MethodImpl(32)]
		public event Action<EventManagerGameData> ShowEventResult;

		[method: MethodImpl(32)]
		public event Action<PvPSeasonManagerGameData> ShowPvPResult;

		[method: MethodImpl(32)]
		public event Action<CurrentGlobalEventState, CurrentGlobalEventState> GlobalEventStateChanged;

		[method: MethodImpl(32)]
		public event Action<CurrentGlobalEventState, CurrentGlobalEventState> GlobalPvPStateChanged;

		[method: MethodImpl(32)]
		public event Action GlobalPvPScoresUpdated;

		[method: MethodImpl(32)]
		public event Action ClassRankedUp;

		[method: MethodImpl(32)]
		public event Action<IInventoryItemGameData, int> ClassRankUpFromLevel;

		public PlayerGameData()
		{
		}

		public PlayerGameData(PlayerData instance)
		{
			_instancedData = instance;
			InventoryGameData = new InventoryGameData(Data.Inventory);
			WorldGameData = new WorldGameData(Data.World);
			if (instance.RandomDecisionSeed == 0f)
			{
				instance.RandomDecisionSeed = UnityEngine.Random.Range(0f, float.MaxValue);
			}
			try
			{
				SocialEnvironmentGameData = new SocialEnvironmentGameData(Data.SocialEnvironment);
				if (ContentLoader.Instance != null && ContentLoader.Instance.m_BeaconConnectionMgr.CachedSocialEnvironmentGameData != null)
				{
					SocialEnvironmentGameData.Data.IdLoginEmail = ContentLoader.Instance.m_BeaconConnectionMgr.CachedSocialEnvironmentGameData.IdLoginEmail;
					ContentLoader.Instance.m_BeaconConnectionMgr.CachedSocialEnvironmentGameData = SocialEnvironmentGameData.Data;
				}
			}
			catch
			{
				if (ContentLoader.Instance != null)
				{
					SocialEnvironmentGameData.FillSocialEnvironmentDataIfEmpty(ContentLoader.Instance.m_BeaconConnectionMgr.CachedSocialEnvironmentGameData);
					SocialEnvironmentGameData = new SocialEnvironmentGameData(ContentLoader.Instance.m_BeaconConnectionMgr.CachedSocialEnvironmentGameData);
					SocialEnvironmentGameData.Data.IdLoginEmail = ContentLoader.Instance.m_BeaconConnectionMgr.CachedSocialEnvironmentGameData.IdLoginEmail;
					SocialEnvironmentGameData.Data.IdPassword = "a"; // ContentLoader.Instance.m_BeaconConnectionMgr.CachedSocialEnvironmentGameData.IdPassword;
					SocialEnvironmentGameData.Data.EventPlayerName = "a";
					SocialEnvironmentGameData.Data.SocialId = "a";
					SocialEnvironmentGameData.Data.SocialPictureUrl = "a";
					SocialEnvironmentGameData.Data.SocialPlayerName = "a";
				}
				else
				{
					SocialEnvironmentGameData = new SocialEnvironmentGameData("default");
					SocialEnvironmentGameData.Data.IdLoginEmail = Data.SocialEnvironment.IdLoginEmail;
					SocialEnvironmentGameData.Data.IdPassword = Data.SocialEnvironment.IdPassword;
					SocialEnvironmentGameData.Data.EventPlayerName = Data.SocialEnvironment.EventPlayerName;
					SocialEnvironmentGameData.Data.SocialId = Data.SocialEnvironment.SocialId;
					SocialEnvironmentGameData.Data.SocialPictureUrl = Data.SocialEnvironment.SocialPictureUrl;
					SocialEnvironmentGameData.Data.SocialPlayerName = Data.SocialEnvironment.SocialPlayerName;
				}
			}
			m_Birds = new List<BirdGameData>();
			foreach (BirdData bird in instance.Birds)
			{
				m_Birds.Add(new BirdGameData(bird));
			}
			if (instance.PvPBanner == null)
			{
				BannerGameData = new BannerGameData("bird_banner", Data.Level);
				instance.PvPBanner = BannerGameData.Data;
				instance.PvPBanner.Level = Data.Level;
				foreach (List<IInventoryItemGameData> value in BannerGameData.InventoryGameData.Items.Values)
				{
					foreach (IInventoryItemGameData item in value)
					{
						InventoryGameData.AddNewItemToInventory(item);
					}
				}
			}
			else
			{
				BannerGameData = new BannerGameData(instance.PvPBanner);
			}
			if (instance.CurrentClassUpgradeShopOffers == null)
			{
				instance.CurrentClassUpgradeShopOffers = new List<string>();
			}
			if (instance.SponsoredAdUses == null)
			{
				instance.SponsoredAdUses = new Dictionary<string, uint>();
			}
			if (instance.CurrentSpecialShopOffers == null)
			{
				instance.CurrentSpecialShopOffers = new Dictionary<string, DateTime>();
			}
			if (instance.TemporaryOpenHotspots == null)
			{
				instance.TemporaryOpenHotspots = new List<string>();
			}
			if (instance.ShopOffersNew == null)
			{
				instance.ShopOffersNew = new Dictionary<string, bool>();
			}
			if (instance.DungeonsAlreadyPlayedToday == null)
			{
				instance.DungeonsAlreadyPlayedToday = new List<string>();
			}
			if (instance.PvpObjectives == null)
			{
				instance.PvpObjectives = new List<PvPObjectiveData>();
			}
			if (instance.AchievementTracking == null)
			{
				instance.AchievementTracking = new AchievementData();
			}
			if (instance.EquippedSkins == null)
			{
				instance.EquippedSkins = new Dictionary<string, string>();
			}
			AddExperienceChangedHandler();
			Data.CinematricIntroStarted = true;
			foreach (ConditionalInventoryBalancingData item2 in from cbd in DIContainerBalancing.Service.GetBalancingDataList<ConditionalInventoryBalancingData>()
				where cbd.Trigger == ConditionalLootTableDropTrigger.NotFirstStartUp
				select cbd)
			{
				try
				{
					if (DIContainerLogic.RequirementService.CheckGenericRequirements(this, item2.DropRequirements) && DIContainerLogic.RequirementService.ExecuteRequirements(this, item2.DropRequirements, "cond_inv_" + item2.NameId))
					{
						DebugLog.Log("[PlayerGameData] Adding Conditional Inventory: " + item2.NameId);
						DIContainerLogic.GetLootOperationService().RewardLoot(InventoryGameData, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(item2.Content, (item2.InitializingLevel <= 0) ? Data.Level : item2.InitializingLevel), "cond_inv_" + item2.NameId);
					}
				}
				catch
				{
				}
			}
			foreach (ConditionalInventoryBalancingData item3 in from cbd in DIContainerBalancing.Service.GetBalancingDataList<ConditionalInventoryBalancingData>()
				where cbd.Trigger == ConditionalLootTableDropTrigger.RemoveNotFirstStartUp
				select cbd)
			{
				try
				{
					if (!DIContainerLogic.RequirementService.CheckGenericRequirements(this, item3.DropRequirements))
					{
						/*Error: Could not find block for branch target IL_072a*/;
					}
					if (!DIContainerLogic.RequirementService.ExecuteRequirements(this, item3.DropRequirements, "cond_inv_" + item3.NameId))
					{
						/*Error: Could not find block for branch target IL_072a*/;
					}
					DebugLog.Log("[PlayerGameData] Removing Conditional Inventory: " + item3.NameId);
					Dictionary<string, LootInfoData> dictionary = DIContainerLogic.GetLootOperationService().GenerateLoot(item3.Content, (item3.InitializingLevel <= 0) ? Data.Level : item3.InitializingLevel);
					using (Dictionary<string, LootInfoData>.KeyCollection.Enumerator enumerator6 = dictionary.Keys.GetEnumerator())
					{
						while (enumerator6.MoveNext())
						{
							string current6 = enumerator6.Current;
							LootInfoData lootInfoData = dictionary[current6];
							DIContainerLogic.InventoryService.RemoveItem(InventoryGameData, current6, lootInfoData.Value, "cond_inv_" + item3.NameId);
						}
						/*Error near IL_0712: Could not find block for branch target IL_072a*/;
					}
				}
				catch
				{
				}
			}
			IInventoryItemGameData data = null;
			if (!DIContainerLogic.InventoryService.TryGetItemGameData(InventoryGameData, "event_energy", out data))
			{
				DIContainerLogic.InventoryService.AddItem(InventoryGameData, 1, 1, "event_energy", (WorldGameData.BalancingData.ItemMaxCaps != null && WorldGameData.BalancingData.ItemMaxCaps.ContainsKey("event_energy")) ? WorldGameData.BalancingData.ItemMaxCaps["event_energy"] : 0, "repair_energy");
			}
			DIContainerLogic.WorldMapService.FixPlayerProgression(WorldGameData, this);
			DIContainerLogic.InventoryService.FixPotionsInInventory(InventoryGameData);
			InitSkinSet();
			RecheckExperienceProgress();
			RecheckMasteryProgress();
			if (!Data.ConvertionFor153)
			{
				ConvertMasteryTo60();
				if (Data.BossIntrosPlayed == null)
				{
					Data.BossIntrosPlayed = new List<string>();
				}
				Data.BossIntrosPlayed.Clear();
				InventoryGameData.Items[InventoryItemType.EventBossItem].Clear();
				InventoryGameData.Data.EventItems.Clear();
				DIContainerLogic.InventoryService.MapPotionRecipes(InventoryGameData);
				AddXPotionIfNeeded();
				MoveFromDeprecatedHotSpots();
				Data.ConvertionFor153 = true;
			}
			if (WorldGameData != null && WorldGameData.HotspotGameDatas != null)
			{
				if (WorldGameData.HotspotGameDatas.Remove("hotspot_003_battleground"))
				{
					WorldGameData.Data.HotSpotInstances.RemoveAll((HotspotData hs) => hs.NameId == "hotspot_003_battleground");
				}
				if (WorldGameData.HotspotGameDatas.Remove("hotspot_005_battleground"))
				{
					WorldGameData.Data.HotSpotInstances.RemoveAll((HotspotData hs) => hs.NameId == "hotspot_005_battleground");
				}
			}
			if (Data.TutorialTracks != null && Data.TutorialTracks.ContainsKey("tutorial_equip_crafted_item"))
			{
				Data.TutorialTracks.Remove("tutorial_equip_crafted_item");
			}
			IInventoryItemGameData data2 = null;
			if (!DIContainerLogic.InventoryService.TryGetItemGameData(InventoryGameData, "pvp_league_crown_max", out data2))
			{
				IInventoryItemGameData data3 = null;
				if (DIContainerLogic.InventoryService.TryGetItemGameData(InventoryGameData, "pvp_league_crown", out data3))
				{
					DIContainerLogic.InventoryService.AddItem(InventoryGameData, data3.ItemData.Level, 1, "pvp_league_crown_max", 1, "FirstLoadOfNewVersion");
				}
			}
			foreach (BirdGameData bird2 in m_Birds)
			{
				foreach (List<IInventoryItemGameData> value2 in bird2.InventoryGameData.Items.Values)
				{
					List<IInventoryItemGameData> list = new List<IInventoryItemGameData>(value2);
					foreach (IInventoryItemGameData item4 in list)
					{
						IInventoryItemGameData data4 = null;
						if (DIContainerLogic.InventoryService.TryGetItemGameData(InventoryGameData, item4.ItemBalancing.NameId, out data4))
						{
							DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { data4 }, item4.ItemBalancing.ItemType, bird2.InventoryGameData);
						}
					}
				}
			}
			if (DIContainerConfig.GetClientConfig().ApplyMinusBillionFix)
			{
				DIContainerLogic.InventoryService.FixInventory(InventoryGameData);
				DIContainerLogic.InventoryService.FixInventoryOfBirds(this);
			}
			DIContainerLogic.InventoryService.FixStampCount(this);
			if (WorldGameData.BalancingData.IsLimeGreen)
			{
				ValidateProfileForLimeGreen(this, InventoryGameData);
			}
			AchievementData achievementTracking = Data.AchievementTracking;
			IInventoryItemGameData data5 = null;
			if (!achievementTracking.Pvpunlocked && DIContainerLogic.InventoryService.TryGetItemGameData(InventoryGameData, "unlock_pvp", out data5) && data5.ItemValue >= 1)
			{
				string achievementIdForStoryItemIfExists = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("unlock_pvp");
				if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists))
				{
					DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists);
					achievementTracking.Pvpunlocked = true;
				}
			}
		}

		public void RegisterGlobalEventStateChanged(CurrentGlobalEventState oldState, CurrentGlobalEventState newState)
		{
			if (this.GlobalEventStateChanged != null)
			{
				this.GlobalEventStateChanged(oldState, newState);
			}
		}

		public void RegisterGlobalPvPScoresUpdated()
		{
			if (CurrentPvPSeasonGameData != null && CurrentPvPSeasonGameData.CurrentSeasonTurn != null)
			{
				CurrentPvPSeasonGameData.Data.CurrentRank = CurrentPvPSeasonGameData.CurrentSeasonTurn.GetCurrentRank;
			}
			if (this.GlobalPvPScoresUpdated != null)
			{
				this.GlobalPvPScoresUpdated();
			}
		}

		public void RegisterGlobalPvPStateChanged(CurrentGlobalEventState oldState, CurrentGlobalEventState newState)
		{
			if (this.GlobalPvPStateChanged != null)
			{
				this.GlobalPvPStateChanged(oldState, newState);
			}
		}

		public void RegisterClassRankUpFromLevel(IInventoryItemGameData itemData, int oldLevel)
		{
			if (Data.PendingClassRankUps == null)
			{
				Data.PendingClassRankUps = new Dictionary<string, int>();
			}
			if (Data.PendingClassRankUps.ContainsKey(itemData.ItemBalancing.NameId))
			{
				Data.PendingClassRankUps[itemData.ItemBalancing.NameId] = Mathf.Min(oldLevel, Data.PendingClassRankUps[itemData.ItemBalancing.NameId]);
			}
			else
			{
				Data.PendingClassRankUps.Add(itemData.ItemBalancing.NameId, oldLevel);
			}
			DIContainerLogic.RateAppController.RequestRatePopupForReason(RatePopupTrigger.MasteryUp);
			if (this.ClassRankUpFromLevel != null)
			{
				this.ClassRankUpFromLevel(itemData, oldLevel);
			}
		}

		public void RegisterClassRankedUp()
		{
			if (this.ClassRankedUp != null)
			{
				this.ClassRankedUp();
			}
		}

		public PlayerGameData Init(string nameId)
		{
			_instancedData = CreateNewInstance(nameId);
			AddExperienceChangedHandler();
			return this;
		}

		private void AddXPotionIfNeeded()
		{
			int totalAccumulatedStars = DIContainerLogic.WorldMapService.GetTotalAccumulatedStars(this);
			if (totalAccumulatedStars >= 75 && totalAccumulatedStars < 155)
			{
				DIContainerLogic.InventoryService.AddItem(InventoryGameData, 1, 1, "potion_xp_01", 3, "Change of stars in 153");
			}
		}

		private void MoveFromDeprecatedHotSpots()
		{
			if (WorldGameData.CurrentHotspotGameData.BalancingData.NameId == "hotspot_003_battleground")
			{
				WorldGameData.CurrentHotspotGameData = WorldGameData.HotspotGameDatas["hotspot_002_battleground"];
			}
			else if (WorldGameData.CurrentHotspotGameData.BalancingData.NameId == "hotspot_005_battleground")
			{
				WorldGameData.CurrentHotspotGameData = WorldGameData.HotspotGameDatas["hotspot_004_battleground"];
			}
			else if (WorldGameData.CurrentHotspotGameData.BalancingData.NameId == "hotspot_018_10_trainerhut")
			{
				WorldGameData.CurrentHotspotGameData = WorldGameData.HotspotGameDatas["hotspot_018_10_battleground"];
			}
			else if (WorldGameData.CurrentHotspotGameData.BalancingData.NameId == "hotspot_094_10_trainerhut")
			{
				WorldGameData.CurrentHotspotGameData = WorldGameData.HotspotGameDatas["hotspot_094_10_battleground"];
			}
		}

		private void RecheckExperienceProgress()
		{
			ExperienceLevelBalancingData balancing;
			if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + Data.Level.ToString("00"), out balancing))
			{
				float num = balancing.Experience;
				float num2 = Data.ExperienceForNextLevel;
				if (num2 == 0f)
				{
					num2 = balancing.OldExperience;
					Data.ExperienceForNextLevel = balancing.Experience;
				}
				if (num2 > 0f && num2 != num)
				{
					float experience = Data.Experience * (num / num2);
					Data.Experience = experience;
					Data.ExperienceForNextLevel = balancing.Experience;
				}
			}
		}

		private void ConvertMasteryTo60()
		{
			foreach (ClassItemGameData item in InventoryGameData.Items[InventoryItemType.Class])
			{
				ExperienceMasteryBalancingData balancing;
				if (!DIContainerBalancing.Service.TryGetBalancingData<ExperienceMasteryBalancingData>("Level_" + item.Data.Level.ToString("00"), out balancing))
				{
					Debug.LogError("could not find mastery entry for level " + item.Data.Level);
					item.Data.Level = Math.Min(item.MasteryMaxRank(), Math.Max(1, item.Data.Level));
					continue;
				}
				float num = 0f;
				if (item.Data.ExperienceForNextLevel == 0f && balancing.AncientExperience > 0)
				{
					num = item.ItemValue / balancing.AncientExperience;
				}
				else if (balancing.OldExperience > 0)
				{
					num = (float)item.ItemValue / (float)balancing.OldExperience;
				}
				int num2 = Math.Min(3, (int)(num * 4f));
				item.Data.Level *= 4;
				item.Data.Level += num2;
				item.ItemValue = 0;
			}
			BirdGameData bird;
			foreach (BirdGameData bird2 in m_Birds)
			{
				bird = bird2;
				List<IInventoryItemGameData> list = InventoryGameData.Items[InventoryItemType.Class].Where((IInventoryItemGameData c) => c.IsValidForBird(bird)).ToList();
				foreach (ClassItemGameData item2 in list)
				{
					float num3 = UnityEngine.Random.Range(0.1f, 0.3f);
					item2.ItemValue = (int)((float)item2.MasteryNeededForNextRank() * num3);
					item2.Data.ExperienceForNextLevel = DIContainerBalancing.Service.GetBalancingData<ExperienceMasteryBalancingData>("Level_" + item2.Data.Level.ToString("00")).Experience;
				}
			}
		}

		public void AdvanceBirdMasteryToHalfOfHighest(ClassItemGameData classToEdit)
		{
			BirdGameData correctBird = Birds.Where((BirdGameData b) => classToEdit.IsValidForBird(b)).FirstOrDefault();
			if (correctBird != null && classToEdit.ItemData.Level <= 1)
			{
				List<IInventoryItemGameData> list = InventoryGameData.Items[InventoryItemType.Class].Where((IInventoryItemGameData c) => c.IsValidForBird(correctBird)).ToList();
				int num = list.Max((IInventoryItemGameData classGameData) => (classGameData as ClassItemGameData).Data.Level);
				if (classToEdit.Data.Level < num)
				{
					classToEdit.ItemData.Level = Mathf.Max(1, Mathf.FloorToInt((float)num / 2f));
				}
			}
		}

		private void RecheckMasteryProgress()
		{
			foreach (ClassItemGameData item in InventoryGameData.Items[InventoryItemType.Class])
			{
				ExperienceMasteryBalancingData balancing;
				if (!DIContainerBalancing.Service.TryGetBalancingData<ExperienceMasteryBalancingData>("Level_" + item.Data.Level.ToString("00"), out balancing))
				{
					Debug.LogError("could not find mastery entry for level " + item.Data.Level);
				}
				else if (0 == 0 && balancing.OldExperience != 0 && item.Data.Level < item.MasteryMaxRank())
				{
					float num = balancing.Experience;
					float num2 = item.Data.ExperienceForNextLevel;
					if (num2 == 0f)
					{
						num2 = balancing.AncientExperience;
						item.Data.ExperienceForNextLevel = balancing.Experience;
					}
					if (num2 > 0f && num2 != num)
					{
						int itemValue = (int)((float)item.ItemValue * (num / num2));
						item.ItemValue = itemValue;
						item.Data.ExperienceForNextLevel = balancing.Experience;
					}
				}
			}
		}

		private void InitSkinSet()
		{
			foreach (IInventoryItemGameData item in InventoryGameData.Items[InventoryItemType.Class])
			{
				ClassItemGameData classItemGameData = item as ClassItemGameData;
				if (classItemGameData == null || Data.EquippedSkins.ContainsKey(classItemGameData.BalancingData.NameId))
				{
					continue;
				}
				foreach (ClassSkinBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<ClassSkinBalancingData>())
				{
					if (balancingData.SortPriority == 0 && balancingData.OriginalClass == classItemGameData.BalancingData.NameId)
					{
						IInventoryItemGameData inventoryItemGameData = DIContainerLogic.GetLootOperationService().RewardLoot(InventoryGameData, 0, DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { { balancingData.NameId, 1 } }, 1), "init_basicskin").FirstOrDefault();
						inventoryItemGameData.ItemData.IsNew = false;
						BirdGameData bird = GetBird(classItemGameData.BalancingData.RestrictedBirdId, true);
						Data.EquippedSkins.Add(classItemGameData.BalancingData.NameId, balancingData.NameId);
						if (bird.InventoryGameData.Items[InventoryItemType.Class].FirstOrDefault().ItemBalancing.NameId == classItemGameData.BalancingData.NameId)
						{
							DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { inventoryItemGameData }, InventoryItemType.Skin, bird.InventoryGameData);
							break;
						}
					}
				}
			}
		}

		public void RemoveInvalidTrophyFix()
		{
			int currentSeason = 0;
			uint currentTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
			foreach (PvPSeasonManagerBalancingData balancingData in DIContainerBalancing.EventBalancingService.GetBalancingDataList<PvPSeasonManagerBalancingData>())
			{
				if (currentTimestamp < balancingData.SeasonEndTimeStamp && currentTimestamp >= balancingData.SeasonStartTimeStamp)
				{
					currentSeason = int.Parse(Regex.Match(balancingData.NameId, "\\d+").Value);
					break;
				}
			}
			RemoveInvalidTrophiesForSeason(currentSeason);
		}

		public void RemoveInvalidTrophiesForSeason(int currentSeason)
		{
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			foreach (IInventoryItemGameData item in InventoryGameData.Items[InventoryItemType.Trophy])
			{
				if (item.ItemData.Level >= currentSeason)
				{
					list.Add(item);
				}
			}
			foreach (IInventoryItemGameData item2 in list)
			{
				InventoryGameData.RemoveItemFromInventory(item2);
			}
		}

		private void SetItemsToQualityFourFix()
		{
			foreach (BirdGameData bird in m_Birds)
			{
				foreach (IInventoryItemGameData item in bird.InventoryGameData.Items[InventoryItemType.MainHandEquipment])
				{
					EquipmentGameData equipmentGameData = item as EquipmentGameData;
					if (equipmentGameData.IsSetItem && equipmentGameData.ItemData.Quality == 2)
					{
						equipmentGameData.ItemData.Level -= 2;
						equipmentGameData.ItemData.Quality = 4;
					}
				}
				foreach (IInventoryItemGameData item2 in bird.InventoryGameData.Items[InventoryItemType.OffHandEquipment])
				{
					EquipmentGameData equipmentGameData2 = item2 as EquipmentGameData;
					if (equipmentGameData2.IsSetItem && equipmentGameData2.ItemData.Quality == 2)
					{
						equipmentGameData2.ItemData.Level -= 2;
						equipmentGameData2.ItemData.Quality = 4;
					}
				}
			}
			foreach (IInventoryItemGameData item3 in InventoryGameData.Items[InventoryItemType.MainHandEquipment])
			{
				EquipmentGameData equipmentGameData3 = item3 as EquipmentGameData;
				if (equipmentGameData3.IsSetItem && equipmentGameData3.ItemData.Quality == 2)
				{
					equipmentGameData3.ItemData.Level -= 2;
					equipmentGameData3.ItemData.Quality = 4;
				}
			}
			foreach (IInventoryItemGameData item4 in InventoryGameData.Items[InventoryItemType.OffHandEquipment])
			{
				EquipmentGameData equipmentGameData4 = item4 as EquipmentGameData;
				if (equipmentGameData4.IsSetItem && equipmentGameData4.ItemData.Quality == 2)
				{
					equipmentGameData4.ItemData.Level -= 2;
					equipmentGameData4.ItemData.Quality = 4;
				}
			}
		}

		private bool IsCurrentTimeZoneValidOrPersistNeeded()
		{
			if (TimeZoneInfo.Local.GetUtcOffset(DateTime.Now) - TimeSpan.FromSeconds(Data.UtcOffset) <= new TimeSpan(0, 1, 0, 0) && Data.IsDaylightSavingTime != TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now))
			{
				DebugLog.Log("Persist new Timezone because the timezone is the same only the daylight saving time switched");
				return true;
			}
			if (DIContainerLogic.GetServerOnlyTimingService().IsAfter(DIContainerLogic.GetServerOnlyTimingService().GetDateTimeFromTimestamp(Data.LastTimezonePersistTimestamp).AddSeconds(DIContainerConfig.GetClientConfig().TimeZonePersistCooldownInSec)))
			{
				DebugLog.Log("Persist new Timezone because the cool down ran out");
				return true;
			}
			DebugLog.Log("Persist new Timezone because the cool down ran out");
			return false;
		}

		public void GenerateEventManagerFromProfile()
		{
			if (Data.CurrentEventManager != null)
			{
				CurrentEventManagerGameData = new EventManagerGameData().CreateFromInstance(Data.CurrentEventManager);
			}
		}

		public void GeneratePvPManagerFromProfile()
		{
			if (Data.PvpSeasonManager != null)
			{
				CurrentPvPSeasonGameData = new PvPSeasonManagerGameData().CreateFromInstance(Data.PvpSeasonManager);
			}
		}

		private void ValidateProfileForLimeGreen(PlayerGameData playerGameData, InventoryGameData InventoryGameData)
		{
			int itemValue = DIContainerLogic.InventoryService.GetItemValue(InventoryGameData, "lucky_coin");
			if (playerGameData.WorldGameData.BalancingData.LimeGreenValue > 0 && itemValue >= playerGameData.WorldGameData.BalancingData.LimeGreenValue)
			{
				bool flag = false;
				if (!playerGameData.Data.IsUserConverted)
				{
					DIContainerLogic.InventoryService.RemoveItem(InventoryGameData, "lucky_coin", itemValue, "cht");
					int itemValue2 = DIContainerLogic.InventoryService.GetItemValue(InventoryGameData, "gold");
					DIContainerLogic.InventoryService.RemoveItem(InventoryGameData, "gold", itemValue2, "cht");
					int itemValue3 = DIContainerLogic.InventoryService.GetItemValue(InventoryGameData, "friendship_essence");
					DIContainerLogic.InventoryService.RemoveItem(InventoryGameData, "friendship_essence", itemValue3, "cht");
					DIContainerLogic.InventoryService.AddItem(InventoryGameData, 1, 1, "friendship_essence", 5, "cht");
					flag = true;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("IsUserConverted", playerGameData.Data.IsUserConverted.ToString());
				dictionary.Add("TookAction", flag.ToString());
				DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("UserIsChtng", dictionary);
			}
		}

		private void AddExperienceChangedHandler()
		{
			IInventoryItemGameData data;
			DIContainerLogic.InventoryService.TryGetItemGameData(InventoryGameData, "experience", out data);
			if (Data.PendingFeatureUnlocks == null)
			{
				Data.PendingFeatureUnlocks = new List<string>();
			}
			if (data != null)
			{
				data.ItemDataChanged += OnExperienceChanged;
			}
			InventoryGameData.InventoryOfTypeChanged -= NewFeatureChanged;
			InventoryGameData.InventoryOfTypeChanged += NewFeatureChanged;
		}

		private void NewFeatureChanged(InventoryItemType type, IInventoryItemGameData item)
		{
			if (item.ItemBalancing.NameId == "unlock_events" && DIContainerInfrastructure.GetCurrentPlayer() != null)
			{
				DIContainerLogic.InventoryService.AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "event_energy", 15, "Unlocked Events!");
			}
			if (type == InventoryItemType.Story && item.ItemBalancing.SortPriority > 0 && item.ItemValue > 0 && !(item.ItemBalancing.NameId == "story_goldenpig_advanced") && !Data.PendingFeatureUnlocks.Contains(item.ItemBalancing.NameId) && (!ClientInfo.ShowMasteryConversionPopup || item.ItemBalancing.NameId != "unlock_mastery_badge"))
			{
				Data.PendingFeatureUnlocks.Add(item.ItemBalancing.NameId);
			}
		}

		public List<IInventoryItemGameData> RewardLevelUpLoot(int level)
		{
			ExperienceLevelBalancingData balancing;
			if (!DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + level.ToString("00"), out balancing) && !DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + (level - 1).ToString("00"), out balancing))
			{
				return null;
			}
			return DIContainerLogic.GetLootOperationService().RewardLootGetInputCopy(InventoryGameData, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(balancing.LootTableAdditional, level), "level_up_reward");
		}

		public int GetNeededExperience()
		{
			ExperienceLevelBalancingData balancing;
			if (!DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + Data.Level.ToString("00"), out balancing))
			{
				return -1;
			}
			return (int)((float)balancing.Experience - Data.Experience);
		}

		public void OnExperienceChanged(IInventoryItemGameData item, float delta)
		{
			ExperienceLevelBalancingData balancing;
			if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + Data.Level.ToString("00"), out balancing))
			{
				Data.Experience += delta;
				int experience = balancing.Experience;
				if (Data.Experience >= (float)experience)
				{
					ApplyLevelUp(experience);
				}
				if (this.ExperienceChanged != null)
				{
					this.ExperienceChanged((int)Data.Experience, experience);
				}
			}
		}

		private void ApplyLevelUp(int neededExperience)
		{
			Data.Experience -= neededExperience;
			Data.Level++;
			ExperienceLevelBalancingData balancing;
			if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + Data.Level.ToString("00"), out balancing))
			{
				Data.ExperienceForNextLevel = balancing.Experience;
			}
			GachaLogic.m_itemsGotThisSession.Clear();
			if (BannerGameData != null)
			{
				BannerGameData.Data.Level = Data.Level;
			}
			HandleNewShopOffers();
			int num = 0;
			foreach (BirdGameData bird in m_Birds)
			{
				int level = bird.Data.Level;
				num += DIContainerInfrastructure.GetPowerLevelCalculator().GetBirdPowerLevel(bird);
				bird.Data.Level = Data.Level;
				bird.RaiseLevelChanged(level, bird.Data.Level);
			}
			string item = string.Format("level_up_{0}:{1}", Data.Level.ToString("00"), num);
			DIContainerLogic.PlayerOperationsService.UpdateHighestPowerLevelEver(this);
			if (!Data.PendingFeatureUnlocks.Contains(item))
			{
				Data.PendingFeatureUnlocks.Add(item);
			}
			if (this.CharacterLevelChanged != null)
			{
				this.CharacterLevelChanged(Data.Level);
			}
			DIContainerLogic.RateAppController.RequestRatePopupForReason(RatePopupTrigger.LevelUp);
			DebugLog.Log("Level Up to level " + Data.Level);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
			if (ClientInfo.CurrentBattleGameData != null)
			{
				dictionary.Add("BattleName", ClientInfo.CurrentBattleGameData.Balancing.NameId);
			}
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("LevelUp", dictionary);
			DIContainerInfrastructure.GetAttributionService().TrackPlayerLevelProgress(Data.Level);
		}

		private void HandleNewShopOffers()
		{
			ShopBalancingData balancing = null;
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			List<BasicShopOfferBalancingData> list = new List<BasicShopOfferBalancingData>();
			if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>("shop_all_worldshops", out balancing))
			{
				list = DIContainerLogic.GetShopService().GetShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), "shop_all_worldshops");
			}
			foreach (BasicShopOfferBalancingData item in list)
			{
				if (!_instancedData.ShopOffersNew.ContainsKey(item.NameId))
				{
					List<Requirement> failed = new List<Requirement>();
					if (!DIContainerLogic.GetShopService().IsOfferBuyableIgnoreCost(this, item, out failed))
					{
						continue;
					}
					List<Requirement> buyRequirements = item.BuyRequirements;
					foreach (Requirement item2 in buyRequirements)
					{
						if (item2.RequirementType == RequirementType.Level && (float)_instancedData.Level >= item2.Value)
						{
							dictionary.Add(item.NameId, true);
						}
					}
				}
				else if (_instancedData.ShopOffersNew[item.NameId])
				{
					dictionary.Add(item.NameId, true);
				}
			}
			_instancedData.ShopOffersNew = dictionary;
		}

		protected override PlayerData CreateNewInstance(string nameId)
		{
			InventoryGameData = new InventoryGameData("player_inventory");
			WorldGameData = new WorldGameData("piggy_island");
			if (ContentLoader.Instance != null)
			{
				SocialEnvironmentGameData.FillSocialEnvironmentDataIfEmpty(ContentLoader.Instance.m_BeaconConnectionMgr.CachedSocialEnvironmentGameData);
				SocialEnvironmentGameData = new SocialEnvironmentGameData(ContentLoader.Instance.m_BeaconConnectionMgr.CachedSocialEnvironmentGameData);
			}
			else
			{
				SocialEnvironmentGameData = new SocialEnvironmentGameData("default");
			}
			m_Birds = new List<BirdGameData>();
			List<BirdData> list = new List<BirdData>();
			BirdBalancingData balancing = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>("bird_red", out balancing))
			{
				BirdGameData birdGameData = new BirdGameData(balancing.NameId);
				m_Birds.Add(birdGameData);
				list.Add(birdGameData.Data);
			}
			int wheelIndex = 0;
			BannerGameData = new BannerGameData("bird_banner");
			PlayerData playerData = new PlayerData();
			playerData.NameId = nameId;
			playerData.UserToken = "0";
			playerData.Inventory = InventoryGameData.Data;
			playerData.World = WorldGameData.Data;
			playerData.Birds = list;
			playerData.SocialEnvironment = SocialEnvironmentGameData.Data;
			playerData.Level = 1;
			playerData.SponsoredAdUses = new Dictionary<string, uint>();
			playerData.CurrentClassUpgradeShopOffers = new List<string>();
			playerData.CurrentSpecialShopOffers = new Dictionary<string, DateTime>();
			playerData.TemporaryOpenHotspots = new List<string>();
			playerData.ShopOffersNew = new Dictionary<string, bool>();
			playerData.DungeonsAlreadyPlayedToday = new List<string>();
			playerData.PvPBanner = BannerGameData.Data;
			playerData.PvpObjectives = new List<PvPObjectiveData>();
			playerData.AchievementTracking = new AchievementData();
			playerData.RandomDecisionSeed = UnityEngine.Random.Range(0f, float.MaxValue);
			playerData.EquippedSkins = new Dictionary<string, string>();
			playerData.ConvertionFor153 = true;
			PlayerData playerData2 = playerData;
			foreach (ClassSkinBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<ClassSkinBalancingData>())
			{
				if (balancingData.OriginalClass == "class_knight" && balancingData.SortPriority == 0)
				{
					playerData2.EquippedSkins.Add("class_knight", balancingData.NameId);
				}
			}
			foreach (BirdGameData bird in m_Birds)
			{
				foreach (List<IInventoryItemGameData> value in bird.InventoryGameData.Items.Values)
				{
					if (value != null && value.Count > 0 && value.FirstOrDefault().ItemBalancing.ItemType == InventoryItemType.Skin)
					{
						continue;
					}
					foreach (IInventoryItemGameData item in value)
					{
						InventoryGameData.AddNewItemToInventory(item);
					}
				}
			}
			foreach (List<IInventoryItemGameData> value2 in BannerGameData.InventoryGameData.Items.Values)
			{
				foreach (IInventoryItemGameData item2 in value2)
				{
					InventoryGameData.AddNewItemToInventory(item2);
				}
			}
			DIContainerLogic.InventoryService.InitItem(InventoryGameData, DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(InventoryGameData, 1, 0, "experience", 0));
			DIContainerLogic.InventoryService.InitItem(InventoryGameData, DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(InventoryGameData, 1, 0, "gold", 0));
			DIContainerLogic.InventoryService.InitItem(InventoryGameData, DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(InventoryGameData, 1, 0, "lucky_coin", 0));
			DIContainerLogic.InventoryService.InitItem(InventoryGameData, DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(InventoryGameData, 1, 0, "friendship_essence", 0));
			List<IInventoryItemGameData> list2 = DIContainerLogic.GetLootOperationService().RewardLoot(InventoryGameData, 0, DIContainerLogic.GetLootOperationService().GenerateLoot(InventoryGameData.BalancingData.DefaultInventoryContent, playerData2.Level, ref wheelIndex), "player_creation");
			foreach (List<IInventoryItemGameData> value3 in InventoryGameData.Items.Values)
			{
				foreach (IInventoryItemGameData item3 in value3)
				{
					if (!item3.ItemBalancing.NameId.StartsWith("comic_cutscene"))
					{
						item3.ItemData.IsNew = false;
					}
					else
					{
						item3.ItemData.IsNew = true;
						DebugLog.Log("[StoryCutscene]: Cutscene is new!");
					}
					if (item3.ItemBalancing.ItemType == InventoryItemType.CraftingRecipes)
					{
						CraftingRecipeGameData craftingRecipeGameData = item3 as CraftingRecipeGameData;
						if (craftingRecipeGameData != null)
						{
							craftingRecipeGameData.Data.IsNewInShop = false;
						}
					}
				}
			}
			return playerData2;
		}

		private void PersistNewTimeZone(bool setOnCooldown)
		{
			DebugLog.Log("Old Time Zone: " + Data.UtcOffset + " is daylight saving time: " + Data.IsDaylightSavingTime);
			if (setOnCooldown)
			{
				Data.LastTimezonePersistTimestamp = DIContainerLogic.GetServerOnlyTimingService().GetCurrentTimestamp();
			}
			Data.UtcOffset = (int)TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalSeconds;
			Data.IsDaylightSavingTime = TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now);
			DebugLog.Log("New Time Zone: " + Data.UtcOffset + " is daylight saving time: " + Data.IsDaylightSavingTime);
		}

		public bool SetBirdUnavailable(string nameId)
		{
			if (string.IsNullOrEmpty(nameId))
			{
				return false;
			}
			foreach (BirdGameData bird in Birds)
			{
				if (string.Compare(bird.Name, nameId, true) == 0)
				{
					bird.Data.IsUnavaliable = true;
					return true;
				}
			}
			return false;
		}

		public BirdGameData AddBird(string nameId)
		{
			BirdBalancingData balancing = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>(nameId, out balancing))
			{
				foreach (BirdGameData bird in m_Birds)
				{
					if (bird.BalancingData.NameId == nameId)
					{
						DebugLog.Log("Bird: " + nameId + " is arleady added but unavaliable: " + bird.Data.IsUnavaliable + " setting it to available!");
						bird.Data.IsUnavaliable = false;
						return bird;
					}
				}
				BirdGameData birdGameData = new BirdGameData(nameId, Data.Level);
				m_Birds.Add(birdGameData);
				Data.Birds.Add(birdGameData.Data);
				birdGameData.Data.Level = Data.Level;
				int wheelIndex = 0;
				List<IInventoryItemGameData> list = DIContainerLogic.GetLootOperationService().RewardLoot(InventoryGameData, 0, DIContainerLogic.GetLootOperationService().GenerateLoot(birdGameData.InventoryGameData.BalancingData.DefaultInventoryContent, Data.Level, ref wheelIndex), "bird_creation_");
				foreach (IInventoryItemGameData item in list)
				{
					item.ItemData.IsNew = false;
				}
				DIContainerLogic.RateAppController.RequestRatePopupForReason(RatePopupTrigger.UnlockFeature);
				return birdGameData;
			}
			return null;
		}

		public BirdGameData GetBird(string nameId, bool includeInactive = false)
		{
			if (string.IsNullOrEmpty(nameId))
			{
				return null;
			}
			foreach (BirdGameData bird in Birds)
			{
				if (string.Compare(bird.Name, nameId, true) == 0)
				{
					return bird;
				}
			}
			if (includeInactive)
			{
				foreach (BirdGameData allBird in AllBirds)
				{
					if (allBird.Name.ToLower() == nameId.ToLower())
					{
						return allBird;
					}
				}
			}
			return null;
		}

		public void SavePlayerData()
		{
			SaveAndUpdateActivity();
		}

		public void SavePlayerDataManual()
		{
			SaveAndUpdateActivity();
		}

		private void SaveAndUpdateActivity()
		{
			if (DIContainerLogic.PlayerOperationsService.UpdateHighestPowerLevelEver(this))
			{
				DebugLog.Log(GetType(), "SaveAndUpdateActivity: new HighestPowerLevelEver!");
			}
			if (DIContainerInfrastructure.GetCoreStateMgr() != null && DIContainerInfrastructure.GetCoreStateMgr().m_isInitialized)
			{
				if (Data != null && InventoryGameData != null && DIContainerLogic.InventoryService.CheckForItem(InventoryGameData, "unlock_events") && Data.CurrentEventManager != null && Data.CurrentEventManager.CurrentScore != 0)
				{
					DIContainerLogic.EventSystemService.RemoveCheatingEventScoreFix();
				}
				if (Data != null && InventoryGameData != null && DIContainerLogic.InventoryService.CheckForItem(InventoryGameData, "unlock_pvp") && Data.PvpSeasonManager != null && Data.PvpSeasonManager.CurrentSeasonTurn != null && Data.PvpSeasonManager.CurrentSeasonTurn.CurrentScore != 0)
				{
					DIContainerLogic.PvPSeasonService.RemoveCheatingPvPScoreFix();
				}
				if (Data.CurrentEventManager == null && DIContainerLogic.EventSystemService.PreviousCurrentOpponents != null)
				{
					Data.CurrentEventManager = new EventManagerData();
					Data.CurrentEventManager.CurrentOpponents = DIContainerLogic.EventSystemService.PreviousCurrentOpponents;
				}
			}
			DIContainerInfrastructure.GetProfileMgr().SaveProfile(Data);
		}

		public FriendData GetFriendData()
		{
			FriendData friendData = new FriendData();
			friendData.FirstName = PublicPlayer.SocialPlayerName;
			friendData.Level = PublicPlayer.Level;
			friendData.Id = "local";
			friendData.HasBonus = true;
			friendData.IsSilhouettePicture = false;
			friendData.PictureUrl = PublicPlayer.SocialAvatarUrl;
			return friendData;
		}

		public void RemovePvPTurnManager()
		{
			if (CurrentPvPSeasonGameData != null)
			{
				CurrentPvPSeasonGameData.CurrentSeasonTurn = null;
				CurrentPvPSeasonGameData.Data.CurrentSeasonTurn = null;
				if (CurrentPvPSeasonGameData.Data.CurrentSeason == CurrentPvPSeasonGameData.Balancing.SeasonTurnAmount)
				{
					CurrentPvPSeasonGameData = null;
					Data.PvpSeasonManager = null;
				}
			}
		}

		public void RemoveEventManager()
		{
			if (Data.CurrentEventManager != null && Data.CurrentEventManager.CurrentOpponents != null)
			{
				DIContainerLogic.EventSystemService.PreviousCurrentOpponents = Data.CurrentEventManager.CurrentOpponents;
			}
			CurrentEventManagerGameData = null;
			Data.CurrentEventManager = null;
		}

		public void SetEventManager(EventManagerGameData eventManagerGameData)
		{
			CurrentEventManagerGameData = eventManagerGameData;
			Data.CurrentEventManager = eventManagerGameData.Data;
		}

		public void RegisterShowEventResult(EventManagerGameData eventManagerGameData)
		{
			IsEventResultPending = true;
			if (this.ShowEventResult != null)
			{
				this.ShowEventResult(eventManagerGameData);
			}
		}

		public void RegisterShowPvPResult(PvPSeasonManagerGameData pvpManagerGameData)
		{
			IsPvPResultPending = true;
			if (this.ShowPvPResult != null)
			{
				this.ShowPvPResult(pvpManagerGameData);
			}
		}

		public DateTime ConvertToLocalTime(DateTime dateTime)
		{
			return dateTime + TimeSpan.FromSeconds(Data.UtcOffset);
		}

		public DateTime ConvertToUniversalTime(DateTime dateTime)
		{
			return dateTime - TimeSpan.FromSeconds(Data.UtcOffset);
		}

		public int GetCurrentWorldProgress()
		{
			return SocialEnvironmentGameData.Data.LocationProgress[LocationType.World];
		}

		public bool OwnsClass(string classNameId)
		{
			return DIContainerLogic.InventoryService.CheckForItem(InventoryGameData, classNameId);
		}

		public bool CanAddMastery(MasteryItemGameData mastery)
		{
			bool flag = true;
			List<ClassSkinBalancingData> list = (from b in DIContainerBalancing.Service.GetBalancingDataList<ClassSkinBalancingData>()
				where b.OriginalClass == mastery.BalancingData.AssociatedClass
				select b).ToList();
			foreach (ClassSkinBalancingData item in list)
			{
				if (OwnsClass(item.NameId))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return false;
			}
			ClassItemGameData classItemGameData = InventoryGameData.Items[InventoryItemType.Class].Where((IInventoryItemGameData c) => c.ItemBalancing.NameId == mastery.BalancingData.AssociatedClass).FirstOrDefault() as ClassItemGameData;
			if (classItemGameData == null || classItemGameData.Data.Level == Data.Level)
			{
				return false;
			}
			return classItemGameData.ItemValue < classItemGameData.MasteryNeededForNextRank();
		}

		public void FixRareCauldronCase()
		{
			if (!DIContainerLogic.InventoryService.CheckForItem(InventoryGameData, "story_cauldron") && DIContainerLogic.InventoryService.CheckForItem(InventoryGameData, "cauldron_leveled"))
			{
				DIContainerLogic.InventoryService.AddItem(InventoryGameData, 1, 1, "story_cauldron", 1, "Cauldron v2 hotfix");
			}
		}
	}
}
