using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.InventoryItems;
using UnityEngine;

namespace ABH.GameDatas
{
	public class BannerItemGameData : GameDataBase<BannerItemBalancingData, BannerItemData>, IInventoryItemGameData
	{
		public List<InterruptCondition> m_InterruptCondition = new List<InterruptCondition>();

		private SkillGameData m_PrimarySkill;

		private SkillGameData m_SecondarySkill;

		private SkillGameData m_SetItemSkill;

		private BannerItemBalancingData m_bannerItemBalancing;

		public string Name
		{
			get
			{
				return BalancingData.LocaBaseId;
			}
		}

		public IInventoryItemBalancingData ItemBalancing
		{
			get
			{
				return BalancingData;
			}
		}

		public IInventoryItemData ItemData
		{
			get
			{
				return Data;
			}
		}

		public int EnchantementLevel
		{
			get
			{
				return Data.EnchantmentLevel;
			}
			set
			{
				Data.EnchantmentLevel = value;
			}
		}

		public float EnchantmentProgress
		{
			get
			{
				return Data.EnchantmentProgress;
			}
			set
			{
				Data.EnchantmentProgress = value;
			}
		}

		public float ItemMainStat
		{
			get
			{
				return GetItemMainStat(BalancingData.BaseStat, BalancingData.StatPerLevelInPercent, Data.Level, Data.Quality, BalancingData.StatPerQualityBase, BalancingData.StatPerQualityPercent, Data.EnchantmentLevel);
			}
		}

		public SkillGameData PrimarySkill
		{
			get
			{
				if (m_PrimarySkill == null && BalancingData.SkillNameIds != null && BalancingData.SkillNameIds.Count >= 1)
				{
					m_PrimarySkill = new SkillGameData(BalancingData.SkillNameIds[0]);
					m_PrimarySkill.SetOverrideSkillParamerters(m_PrimarySkill.SkillParameters);
				}
				return m_PrimarySkill;
			}
		}

		public SkillGameData SecondarySkill
		{
			get
			{
				if (m_SecondarySkill == null && BalancingData.SkillNameIds != null && BalancingData.SkillNameIds.Count >= 2)
				{
					m_SecondarySkill = new SkillGameData(BalancingData.SkillNameIds[1]);
					m_SecondarySkill.SetOverrideSkillParamerters(m_SecondarySkill.SkillParameters);
				}
				return m_SecondarySkill;
			}
		}

		public SkillGameData SetItemSkill
		{
			get
			{
				if (!IsSetItem)
				{
					return null;
				}
				if (m_SetItemSkill != null)
				{
					return m_SetItemSkill;
				}
				DebugLog.Log("Set Item Skill Name Id: " + BalancingData.UnlockableSetSkillNameId);
				m_SetItemSkill = new SkillGameData(BalancingData.UnlockableSetSkillNameId);
				return m_SetItemSkill;
			}
		}

		public BannerItemBalancingData CorrespondingSetItem
		{
			get
			{
				if (!IsSetItem)
				{
					return null;
				}
				if (m_bannerItemBalancing != null)
				{
					return m_bannerItemBalancing;
				}
				DIContainerBalancing.Service.TryGetBalancingData<BannerItemBalancingData>(BalancingData.CorrespondingSetItem, out m_bannerItemBalancing);
				return m_bannerItemBalancing;
			}
		}

		public bool IsSetItem
		{
			get
			{
				return !string.IsNullOrEmpty(BalancingData.CorrespondingSetItem);
			}
		}

		public string ItemAssetName
		{
			get
			{
				return BalancingData.AssetBaseId;
			}
		}

		public string ItemLocalizedName
		{
			get
			{
				return DIContainerInfrastructure.GetLocaService().GetBannerItemName(BalancingData.LocaBaseId);
			}
		}

		public string ItemLocalizedDesc
		{
			get
			{
				return DIContainerInfrastructure.GetLocaService().GetClassDesc(BalancingData.LocaBaseId);
			}
		}

		public string ItemIconAtlasName
		{
			get
			{
				return string.Empty;
			}
		}

		public int ItemValue
		{
			get
			{
				return Data.Value;
			}
			set
			{
				Data.Value = value;
			}
		}

		[method: MethodImpl(32)]
		public event Action<IInventoryItemGameData, float> ItemDataChanged;

		public BannerItemGameData(string nameId)
			: base(nameId)
		{
		}

		public BannerItemGameData(BannerItemData instance)
			: base(instance)
		{
		}

		protected override BannerItemData CreateNewInstance(string nameId)
		{
			BannerItemData bannerItemData = new BannerItemData();
			bannerItemData.NameId = nameId;
			bannerItemData.IsNew = true;
			bannerItemData.Level = 1;
			bannerItemData.Quality = 1;
			bannerItemData.Value = 1;
			return bannerItemData;
		}

		public void RaiseItemDataChanged(float delta)
		{
			m_PrimarySkill = null;
			m_SecondarySkill = null;
			if (this.ItemDataChanged != null)
			{
				this.ItemDataChanged(this, delta);
			}
		}

		public static float GetItemMainStat(BannerItemGameData data, int quality)
		{
			if (data == null)
			{
				return 0f;
			}
			return GetItemMainStat(data.BalancingData.BaseStat, data.BalancingData.StatPerLevelInPercent, data.Data.Level, quality, data.BalancingData.StatPerQualityBase, data.BalancingData.StatPerQualityPercent, data.EnchantementLevel);
		}

		public float GetItemMainStatWithEnchantmentLevel(int enchantmentLevel)
		{
			return GetItemMainStat(BalancingData.BaseStat, BalancingData.StatPerLevelInPercent, Data.Level, Data.Quality, BalancingData.StatPerQualityBase, BalancingData.StatPerQualityPercent, enchantmentLevel);
		}

		public static float GetItemMainStat(int baseStat, float statPerLevelInPercent, int level, int quality, List<int> qualityBaseList, List<int> qualityPerLevelList, int enchantmentLevel)
		{
			float num = DIContainerLogic.EnchantmentLogic.GetBalancing(enchantmentLevel).StatsBoost / 100f;
			float num2 = ((float)baseStat + GetBaseStatPerQuality(qualityBaseList, quality)) * (1f + ((float)GetBaseStatInPercentPerQuality(qualityPerLevelList, quality) / 100f + statPerLevelInPercent * (float)(level - 1) / 100f)) * (1f + (float)GetMasteryModifierForLevel(level) / 100f);
			if (enchantmentLevel > 0)
			{
				num2 += num2 * num;
			}
			return num2;
		}

		public static int GetMasteryModifierForLevel(int level)
		{
			if (level <= 0)
			{
				return 0;
			}
			ExperienceLevelBalancingData balancing;
			if (level == DIContainerLogic.PlayerOperationsService.GetPlayerMaxLevel() || !DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + level.ToString("00"), out balancing))
			{
				float num = 0f;
				foreach (ExperienceLevelBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<ExperienceLevelBalancingData>())
				{
					if (balancingData.MasteryModifier > num)
					{
						num = balancingData.MasteryModifier;
					}
				}
				return (int)num;
			}
			return (int)balancing.MasteryModifier;
		}

		private static float GetBaseStatPerQuality(List<int> qualityBaseList, int quality)
		{
			if (qualityBaseList == null)
			{
				return 0f;
			}
			if (qualityBaseList.Count <= 0)
			{
				return 0f;
			}
			if (qualityBaseList.Count < quality)
			{
				return qualityBaseList.LastOrDefault();
			}
			if (quality == 0)
			{
				return 0f;
			}
			return qualityBaseList[quality - 1];
		}

		private static int GetBaseStatInPercentPerQuality(List<int> qualityPerLevelList, int level)
		{
			if (qualityPerLevelList == null)
			{
				return 0;
			}
			if (qualityPerLevelList.Count <= 0)
			{
				return 0;
			}
			if (qualityPerLevelList.Count < level)
			{
				return qualityPerLevelList.LastOrDefault();
			}
			return qualityPerLevelList[level - 1];
		}

		public bool HasPerkSkill()
		{
			return PrimarySkill != null && PrimarySkill.IsPseudoPerk();
		}

		public PerkType GetPerkTypeOfSkill()
		{
			if (!HasPerkSkill())
			{
				return PerkType.None;
			}
			return PrimarySkill.GetPerkType();
		}

		public static string GetPerkIconNameByPerk(PerkType perkType)
		{
			switch (perkType)
			{
			case PerkType.Bedtime:
				return "PassiveEffect_Stun";
			case PerkType.ChainAttack:
				return "PassiveEffect_ChainAttack";
			case PerkType.CriticalStrike:
				return "PassiveEffect_CriticalStrike";
			case PerkType.Enrage:
				return "PassiveEffect_AngerManagement";
			case PerkType.Dispel:
				return "PassiveEffect_Dispel";
			case PerkType.ReduceRespawn:
				return "PassiveEffect_SquirePig";
			case PerkType.HocusPokus:
				return "PassiveEffect_Vampiricaura";
			case PerkType.Might:
				return "PassiveEffect_Poweraura";
			case PerkType.Vigor:
				return "PassiveEffect_Vigor";
			case PerkType.ShareBirdDamage:
				return "PassiveEffect_BirdBond";
			case PerkType.Vitality:
				return "PassiveEffect_Vitality";
			case PerkType.IncreaseHealing:
				return "PassiveEffect_Boosthealing";
			case PerkType.IncreaseRage:
				return "PassiveEffect_PerfectBalance";
			case PerkType.MythicProtection:
				return "PassiveEffect_EliteEmblem";
			case PerkType.Finisher:
				return "PassiveEffect_AncientMight";
			case PerkType.Stronghold:
				return "PassiveEffect_SoaringProtection";
			case PerkType.Justice:
				return "PassiveEffect_WingedJustice";
			default:
				return "Character_Health_Large";
			}
		}

		public bool IsSetCompleted(BannerGameData banner)
		{
			if (IsSetItem && ItemBalancing.ItemType == InventoryItemType.BannerEmblem)
			{
				return true;
			}
			return IsSetItem && (banner.BannerCenter.BalancingData.NameId == CorrespondingSetItem.NameId || banner.BannerTip.BalancingData.NameId == CorrespondingSetItem.NameId);
		}

		public bool IsValidForBird(BirdGameData bird)
		{
			return false;
		}

		public string ItemLocalizedTooltipDesc(InventoryGameData inventory)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", DIContainerLogic.InventoryService.GetItemValue(inventory, BalancingData.NameId).ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetItemTooltipDesc(BalancingData.LocaBaseId, dictionary);
		}

		public void ResetValue()
		{
			Data.Value = 1;
		}

		public int GetStars()
		{
			if (Data.Stars == 0)
			{
				if (BalancingData.Stars < 4)
				{
					return BalancingData.Stars;
				}
				return 2;
			}
			return Data.Stars - 1;
		}

		public bool IsMaxEnchanted()
		{
			int enchantmentLevel = Data.EnchantmentLevel;
			EnchantingBalancingData balancing = DIContainerLogic.EnchantmentLogic.GetBalancing(this);
			if (balancing == null)
			{
				return false;
			}
			if (IsSetItem)
			{
				return !balancing.SetAllowed;
			}
			if (GetStars() == 0)
			{
				return !balancing.Stars0Allowed;
			}
			if (GetStars() == 1)
			{
				return !balancing.Stars1Allowed;
			}
			if (GetStars() == 2)
			{
				return !balancing.Stars2Allowed;
			}
			if (GetStars() == 3)
			{
				return !balancing.Stars3Allowed;
			}
			return false;
		}

		public bool AllowEnchanting()
		{
			if (ItemBalancing.ItemType != InventoryItemType.MainHandEquipment && ItemBalancing.ItemType != InventoryItemType.OffHandEquipment && ItemBalancing.ItemType != InventoryItemType.Banner && ItemBalancing.ItemType != InventoryItemType.BannerEmblem && ItemBalancing.ItemType != InventoryItemType.BannerTip)
			{
				return false;
			}
			if (!DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_enchantment"))
			{
				return false;
			}
			return true;
		}

		public Dictionary<string, int> GetScrapLoot()
		{
			if (BalancingData.ScrapLoot == null)
			{
				return new Dictionary<string, int>();
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>(BalancingData.ScrapLoot);
			if (Data.EnchantmentLevel == 0)
			{
				return dictionary;
			}
			int num = 0;
			int enchantementLevel = EnchantementLevel;
			for (int i = 0; i < Data.EnchantmentLevel; i++)
			{
				EnchantingBalancingData balancing = DIContainerLogic.EnchantmentLogic.GetBalancing(this, i);
				if (balancing != null)
				{
					num += (int)(balancing.ResourceCosts * balancing.ScrappingBonus / 100f);
				}
			}
			int num2 = dictionary.Keys.Where((string k) => k != "friendship_essence").Count();
			foreach (string key4 in BalancingData.ScrapLoot.Keys)
			{
				if (key4 == "friendship_essence")
				{
					continue;
				}
				string key;
				int num3;
				if (key4 == "shard" && IsSetItem)
				{
					Dictionary<string, int> dictionary2;
					Dictionary<string, int> dictionary3 = (dictionary2 = dictionary);
					string key2 = (key = key4);
					num3 = dictionary2[key];
					dictionary3[key2] = num3 + enchantementLevel;
					continue;
				}
				EnchantingBalancingData balancing2 = DIContainerLogic.EnchantmentLogic.GetBalancing(this);
				float num4 = 1f;
				if (balancing2 != null)
				{
					num4 = GetBonusFromResource(key4, balancing2);
				}
				int num5 = (int)((float)(num / num2) / num4);
				Dictionary<string, int> dictionary4;
				Dictionary<string, int> dictionary5 = (dictionary4 = dictionary);
				string key3 = (key = key4);
				num3 = dictionary4[key];
				dictionary5[key3] = num3 + num5;
			}
			return dictionary;
		}

		private float GetBonusFromResource(string resourceId, EnchantingBalancingData enchBalancing)
		{
			CraftingItemBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<CraftingItemBalancingData>(resourceId);
			int num = 1;
			if (balancingData.ValueOfBaseItem == 2)
			{
				num = 2;
			}
			else if (balancingData.ValueOfBaseItem == 4)
			{
				num = 3;
			}
			switch (num)
			{
			case 1:
				return enchBalancing.Lvl1ResPoints;
			case 2:
				return enchBalancing.Lvl2ResPoints;
			case 3:
				return enchBalancing.Lvl3ResPoints;
			default:
				Debug.LogError("Error: item level could not be defined!");
				return 1f;
			}
		}
	}
}
