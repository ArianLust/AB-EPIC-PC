using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.InventoryItems;
using UnityEngine;

namespace ABH.GameDatas
{
	public class EquipmentGameData : GameDataBase<EquipmentBalancingData, EquipmentData>, IInventoryItemGameData
	{
		private EquipmentBalancingData m_EquipmentItemBalancing;

		private SkillGameData m_SetItemSkill;

		private SkillGameData m_PvpSetItemSkill;

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

		public float ItemMainStat
		{
			get
			{
				return GetItemMainStat(BalancingData.BaseStat, BalancingData.StatPerLevel, Data.Level, Data.Quality, BalancingData.StatPerQuality, BalancingData.StatPerQualityPercent, Data.EnchantmentLevel);
			}
		}

		public SkillGameData PrimarySkill
		{
			get
			{
				return null;
			}
		}

		public SkillGameData SecondarySkill
		{
			get
			{
				return null;
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

		public EquipmentBalancingData CorrespondingSetItem
		{
			get
			{
				if (!IsSetItem)
				{
					return null;
				}
				if (m_EquipmentItemBalancing != null)
				{
					return m_EquipmentItemBalancing;
				}
				DIContainerBalancing.Service.TryGetBalancingData<EquipmentBalancingData>(BalancingData.CorrespondingSetItemId, out m_EquipmentItemBalancing);
				return m_EquipmentItemBalancing;
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
				DebugLog.Log("Set Item Skill Name Id: " + BalancingData.SetItemSkill);
				m_SetItemSkill = new SkillGameData(BalancingData.SetItemSkill);
				return m_SetItemSkill;
			}
		}

		public SkillGameData PvpSetItemSkill
		{
			get
			{
				if (!IsSetItem)
				{
					return null;
				}
				if (m_PvpSetItemSkill != null)
				{
					return m_PvpSetItemSkill;
				}
				if (string.IsNullOrEmpty(BalancingData.PvpSetItemSkill))
				{
					m_PvpSetItemSkill = new SkillGameData(BalancingData.SetItemSkill);
				}
				else
				{
					m_PvpSetItemSkill = new SkillGameData(BalancingData.PvpSetItemSkill);
				}
				return m_PvpSetItemSkill;
			}
		}

		public bool IsSetItem
		{
			get
			{
				return !string.IsNullOrEmpty(BalancingData.CorrespondingSetItemId);
			}
		}

		public bool IsRanged
		{
			get
			{
				return BalancingData.IsRanged;
			}
		}

		public string Name
		{
			get
			{
				return BalancingData.LocaBaseId + "_" + Data.Level.ToString("00");
			}
		}

		public string ItemAssetName
		{
			get
			{
				return GetItemAssetId(this);
			}
		}

		public string ProjectileAssetName
		{
			get
			{
				return GetProjectileAssetId(this);
			}
		}

		public string ItemLocalizedName
		{
			get
			{
				return GetFinalEquipmentName(BalancingData, Data.Level, Data.ItemSource);
			}
		}

		public string ItemLocalizedDesc
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{value_1}", ItemMainStat.ToString("0"));
				return DIContainerInfrastructure.GetLocaService().GetEquipmentDesc(BalancingData.LocaBaseId, dictionary);
			}
		}

		public string ItemIconAtlasName
		{
			get
			{
				return string.Empty;
			}
		}

		[method: MethodImpl(32)]
		public event Action<IInventoryItemGameData, float> ItemDataChanged;

		public EquipmentGameData(string nameId)
			: base(nameId)
		{
		}

		public EquipmentGameData(EquipmentData instance)
			: base(instance)
		{
		}

		protected override EquipmentData CreateNewInstance(string nameId)
		{
			return new EquipmentData();
		}

		public void RaiseItemDataChanged(float delta)
		{
			if (this.ItemDataChanged != null)
			{
				this.ItemDataChanged(this, delta);
			}
		}

		public float GetItemMainStatWithEnchantmentLevel(int enchantmentLevel)
		{
			return GetItemMainStat(BalancingData.BaseStat, BalancingData.StatPerLevel, Data.Level, Data.Quality, BalancingData.StatPerQuality, BalancingData.StatPerQualityPercent, enchantmentLevel);
		}

		public bool IsValidForBird(BirdGameData bird)
		{
			if (string.IsNullOrEmpty(BalancingData.RestrictedBirdId))
			{
				return false;
			}
			return BalancingData.RestrictedBirdId.Equals(bird.BalancingData.NameId);
		}

		public static string GetRestrictedBirdIcon(EquipmentGameData equipmentGameData)
		{
			if (equipmentGameData == null)
			{
				return string.Empty;
			}
			switch (equipmentGameData.BalancingData.RestrictedBirdId)
			{
			case "bird_red":
				return "Target_RedBird";
			case "bird_yellow":
				return "Target_YellowBird";
			case "bird_white":
				return "Target_WhiteBird";
			case "bird_black":
				return "Target_BlackBird";
			case "bird_blue":
				return "Target_BlueBirds";
			default:
				DebugLog.Error("Unknown RestrictedBirdID : " + equipmentGameData.BalancingData.RestrictedBirdId);
				return string.Empty;
			}
		}

		public static string GetPerkIcon(EquipmentGameData equipmentGameData)
		{
			if (equipmentGameData == null)
			{
				return string.Empty;
			}
			return GetPerkIconNameByPerk(equipmentGameData.BalancingData.Perk.Type);
		}

		public static string GetPerkIcon(BannerItemGameData equipmentGameData)
		{
			if (equipmentGameData == null)
			{
				return string.Empty;
			}
			return GetPerkIconNameByPerk(equipmentGameData.GetPerkTypeOfSkill());
		}

		public static string GetPerkIconNameByPerk(PerkType perkType)
		{
			switch (perkType)
			{
			case PerkType.Bedtime:
				return "PassiveEffect_Bedtime";
			case PerkType.ChainAttack:
				return "PassiveEffect_ChainAttack";
			case PerkType.CriticalStrike:
				return "PassiveEffect_CriticalStrike";
			case PerkType.Dispel:
				return "PassiveEffect_Dispel";
			case PerkType.HocusPokus:
				return "PassiveEffect_HocusPocus";
			case PerkType.Might:
				return "PassiveEffect_Might";
			case PerkType.Vigor:
				return "PassiveEffect_Vigor";
			case PerkType.Vitality:
				return "PassiveEffect_Vitality";
			case PerkType.IncreaseHealing:
				return "PassiveEffect_Sweet";
			case PerkType.IncreaseRage:
				return "PassiveEffect_Balance";
			default:
				return "Character_Health_Large";
			}
		}

		private static string GetCraftingItemPrefix(int level)
		{
			if (level <= 33)
			{
				return "craftingitemprefix_01";
			}
			if (level <= 66)
			{
				return "craftingitemprefix_02";
			}
			if (level < 100)
			{
				return "craftingitemprefix_03";
			}
			DebugLog.Error("no level range for CraftingItemPrefix found");
			return string.Empty;
		}

		private static string GetItemIteration(EquipmentBalancingData balancing, int level)
		{
			return string.Empty;
		}

		public static string GetFinalEquipmentName(EquipmentBalancingData balancing, int level, EquipmentSource source)
		{
			string itemAssetOrLoca = GetItemAssetOrLoca(balancing, level, balancing.LocaBaseId);
			if (balancing.DirectAssetAndLoca)
			{
				return DIContainerInfrastructure.GetLocaService().GetEquipmentName(balancing.LocaBaseId);
			}
			switch (source)
			{
			case EquipmentSource.Loot:
			case EquipmentSource.Crafting:
			case EquipmentSource.LootBird:
				itemAssetOrLoca += "_crafting";
				return GetItemIteration(balancing, level) + DIContainerInfrastructure.GetLocaService().GetEquipmentName(itemAssetOrLoca);
			case EquipmentSource.Gatcha:
				itemAssetOrLoca += "_gatcha";
				return GetItemIteration(balancing, level) + DIContainerInfrastructure.GetLocaService().GetEquipmentName(itemAssetOrLoca);
			case EquipmentSource.SetItem:
				DebugLog.Error("GetFinalEquipmentName for EquipmentSource.SetItem not implemented yet");
				return string.Empty;
			default:
				DebugLog.Error("Unknown EquipmentSourceType " + source);
				return string.Empty;
			}
		}

		public static float GetItemMainStat(EquipmentGameData data, int quality)
		{
			if (data == null)
			{
				return 0f;
			}
			return GetItemMainStat(data.BalancingData.BaseStat, data.BalancingData.StatPerLevel, data.Data.Level, quality, data.BalancingData.StatPerQuality, data.BalancingData.StatPerQualityPercent, data.EnchantementLevel);
		}

		public static float GetItemMainStat(float baseStat, float statPerLevelInPercent, int level, int quality, float statPerQuality, List<int> statsPerQualityInPercent, int enchantmentLevel)
		{
			int num = 0;
			if (statsPerQualityInPercent != null)
			{
				num = ((statsPerQualityInPercent.Count <= quality) ? statsPerQualityInPercent[statsPerQualityInPercent.Count - 1] : statsPerQualityInPercent[quality]);
			}
			float num2 = ((num > 0) ? 1 : 0);
			float num3 = Mathf.Round(baseStat + baseStat * (statPerLevelInPercent / 100f) * (float)(level - 1));
			float num4 = ((quality <= 0) ? 0f : (num3 * ((float)num / (float)quality / 100f)));
			float num5 = DIContainerLogic.EnchantmentLogic.GetBalancing(enchantmentLevel).StatsBoost / 100f;
			float num6 = num3 + statPerQuality * (float)quality + Mathf.Max(Mathf.Round(num4 * (float)quality), num2 * (float)quality);
			if (enchantmentLevel > 0)
			{
				num6 += num6 * num5;
			}
			return num6;
		}

		private string GetItemAssetId(EquipmentGameData eqgd)
		{
			string assetBaseId = eqgd.ItemBalancing.AssetBaseId;
			return GetItemAssetOrLoca(eqgd.BalancingData, eqgd.Data.Level, assetBaseId);
		}

		private static string GetItemAssetOrLoca(EquipmentBalancingData eqbd, int level, string baseId)
		{
			if (string.IsNullOrEmpty(eqbd.RestrictedBirdId) || eqbd.DirectAssetAndLoca)
			{
				return baseId;
			}
			if (level >= 0 && (eqbd.ItemType == InventoryItemType.MainHandEquipment || eqbd.ItemType == InventoryItemType.OffHandEquipment))
			{
				int assetAndLocaBaseLevel = GetAssetAndLocaBaseLevel(eqbd, level);
				return baseId + "_" + ((assetAndLocaBaseLevel - 1) / 3 % eqbd.AssetCycleCount + 1).ToString("00");
			}
			return baseId;
		}

		private static int GetAssetAndLocaBaseLevel(EquipmentBalancingData eqbd, int level)
		{
			int num = level + eqbd.AssetLevelOffset;
			if (num <= 0)
			{
				num = eqbd.AssetCycleCount * 3 + num;
			}
			return num;
		}

		private string GetProjectileAssetId(EquipmentGameData eqgd)
		{
			if (!eqgd.BalancingData.IsRanged)
			{
				return string.Empty;
			}
			if (eqgd.BalancingData.DirectProjectileAssetAndLoca)
			{
				return eqgd.BalancingData.ProjectileAssetId;
			}
			if (eqgd.ItemData.Level >= 0 && eqgd.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment)
			{
				int num = eqgd.ItemData.Level + eqgd.BalancingData.AssetLevelOffset;
				if (num <= 0)
				{
					num = 27 + num;
				}
				return eqgd.BalancingData.ProjectileAssetId + "_" + ((num - 1) / 3 % 9 + 1).ToString("00");
			}
			if (eqgd.ItemData.Level >= 0 && eqgd.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment)
			{
				int num2 = eqgd.ItemData.Level + eqgd.BalancingData.AssetLevelOffset;
				if (num2 <= 0)
				{
					num2 = 24 + num2;
				}
				return eqgd.BalancingData.ProjectileAssetId + "_" + ((num2 - 1) / 3 % 8 + 1).ToString("00");
			}
			return eqgd.BalancingData.ProjectileAssetId;
		}

		internal static string GetPerkName(EquipmentGameData item)
		{
			return DIContainerInfrastructure.GetLocaService().GetPerkName(item.BalancingData.Perk.Type);
		}

		internal static string GetPerkDesc(EquipmentGameData item)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", item.BalancingData.Perk.ProbablityInPercent.ToString("0"));
			dictionary.Add("{value_2}", item.BalancingData.Perk.PerkValue.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetPerkDesc(item.BalancingData.Perk.Type, dictionary);
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

		public bool IsSetCompleted(ICharacter bird)
		{
			return IsSetItem && (bird.OffHandItem.BalancingData.NameId == CorrespondingSetItem.NameId || bird.MainHandItem.BalancingData.NameId == CorrespondingSetItem.NameId);
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
			if (Data.Quality == 0)
			{
				return !balancing.Stars0Allowed;
			}
			if (Data.Quality == 1)
			{
				return !balancing.Stars1Allowed;
			}
			if (Data.Quality == 2)
			{
				return !balancing.Stars2Allowed;
			}
			if (Data.Quality == 3)
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
			if (Data.ScrapLoot == null)
			{
				Data.ScrapLoot = DIContainerLogic.CraftingService.GenerateScrapLootOnNewEquipment(Data.Level, EquipmentSource.Gatcha, BalancingData.NameId, BalancingData.ItemType);
			}
			if (Data.ScrapLoot == null)
			{
				return new Dictionary<string, int>();
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>(Data.ScrapLoot);
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
			foreach (string key4 in Data.ScrapLoot.Keys)
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
				float num4 = 1f;
				EnchantingBalancingData balancing2 = DIContainerLogic.EnchantmentLogic.GetBalancing(this);
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
