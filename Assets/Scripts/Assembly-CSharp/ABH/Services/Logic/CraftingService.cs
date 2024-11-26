using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.Generic;
using ABH.Shared.Models.InventoryItems;

namespace ABH.Services.Logic
{
	public class CraftingService
	{
		private Dictionary<string, List<KeyValuePair<int, ResourceCostPerLevelBalancingData>>> m_resourceCostPerItemNameOrderedByLevel = new Dictionary<string, List<KeyValuePair<int, ResourceCostPerLevelBalancingData>>>();

		private InventoryOperationServiceRealImpl m_inventoryService;

		private LootOperations m_lootService;

		private WorldBalancingData m_worldBalancing;

		private Action<string> DebugLog;

		private Action<string> ErrorLog;

		public CraftingService()
		{
			SetResourceCostPerLevelBalancingData();
		}

		public bool ItemCraftedOnce(InventoryGameData inventory)
		{
			return m_inventoryService.GetItemValue(inventory, "unlock_itemcraftedonce") > 0;
		}

		public bool ForcedBadRoll(InventoryGameData inventory)
		{
			return m_inventoryService.GetItemValue(inventory, "unlock_craft_reroll_failed_roll") > 0;
		}

		public bool ApplyBadRoll(InventoryGameData inventory)
		{
			return m_inventoryService.RemoveItem(inventory, "unlock_craft_reroll_failed_roll", m_inventoryService.GetItemValue(inventory, "unlock_craft_reroll_failed_roll"), "tutorial_forced_0star");
		}

		public bool ForcedGoodRoll(InventoryGameData inventory)
		{
			return m_inventoryService.GetItemValue(inventory, "unlock_craft_reroll_first_reroll") > 0;
		}

		public bool ApplyGoodRoll(InventoryGameData inventory)
		{
			return m_inventoryService.RemoveItem(inventory, "unlock_craft_reroll_first_reroll", m_inventoryService.GetItemValue(inventory, "unlock_craft_reroll_first_reroll"), "tutorial_forced_3star");
		}

		public bool PotionCraftedOnce(InventoryGameData inventory)
		{
			return m_inventoryService.GetItemValue(inventory, "unlock_potioncraftedonce") > 0;
		}

		public CraftingService SetInventoryService(InventoryOperationServiceRealImpl inventoryService)
		{
			m_inventoryService = inventoryService;
			return this;
		}

		public CraftingService SetLootService(LootOperations lootService)
		{
			m_lootService = lootService;
			return this;
		}

		public CraftingService SetWorldBalancing(WorldBalancingData worldBalancing)
		{
			m_worldBalancing = worldBalancing;
			return this;
		}

		public CraftingService SetResourceCostPerLevelBalancingData()
		{
			m_resourceCostPerItemNameOrderedByLevel.Clear();
			IList<ResourceCostPerLevelBalancingData> balancingDataList = DIContainerBalancing.Service.GetBalancingDataList<ResourceCostPerLevelBalancingData>();
			if (balancingDataList == null)
			{
				LogError("No ResourceCostPerLevelBalancingData found!");
			}
			foreach (ResourceCostPerLevelBalancingData item in balancingDataList)
			{
				if (!m_resourceCostPerItemNameOrderedByLevel.ContainsKey(item.AppliedItemNameId))
				{
					m_resourceCostPerItemNameOrderedByLevel.Add(item.AppliedItemNameId, new List<KeyValuePair<int, ResourceCostPerLevelBalancingData>>());
				}
				bool flag = false;
				for (int i = 0; i < m_resourceCostPerItemNameOrderedByLevel[item.AppliedItemNameId].Count; i++)
				{
					if (m_resourceCostPerItemNameOrderedByLevel[item.AppliedItemNameId][i].Key > item.Level)
					{
						flag = true;
						m_resourceCostPerItemNameOrderedByLevel[item.AppliedItemNameId].Insert(i, new KeyValuePair<int, ResourceCostPerLevelBalancingData>(item.Level, item));
						break;
					}
				}
				if (!flag)
				{
					m_resourceCostPerItemNameOrderedByLevel[item.AppliedItemNameId].Add(new KeyValuePair<int, ResourceCostPerLevelBalancingData>(item.Level, item));
				}
			}
			return this;
		}

		public CraftingService SetDebugLog(Action<string> debugLog)
		{
			DebugLog = debugLog;
			return this;
		}

		public CraftingService SetErrorLog(Action<string> errorLog)
		{
			ErrorLog = errorLog;
			return this;
		}

		private void LogDebug(string message)
		{
			if (DebugLog != null)
			{
				DebugLog(message);
			}
		}

		private void LogError(string message)
		{
			if (ErrorLog != null)
			{
				ErrorLog(message);
			}
		}

		public KeyValuePair<string, int> GetFallbackRecipeItem(CraftingRecipeBalancingData recipe, int itemLevel)
		{
			string key = recipe.ResultLoot.FirstOrDefault().Key;
			List<KeyValuePair<int, ResourceCostPerLevelBalancingData>> value;
			if (!m_resourceCostPerItemNameOrderedByLevel.TryGetValue(key, out value))
			{
				LogError("No ResourceCost Balancing found for: " + recipe.NameId);
				return default(KeyValuePair<string, int>);
			}
			ResourceCostPerLevelBalancingData resourceCostPerLevelBalancingData = null;
			foreach (KeyValuePair<int, ResourceCostPerLevelBalancingData> item in value)
			{
				if (resourceCostPerLevelBalancingData == null)
				{
					resourceCostPerLevelBalancingData = item.Value;
					continue;
				}
				if (item.Key > itemLevel)
				{
					break;
				}
				resourceCostPerLevelBalancingData = item.Value;
			}
			if (resourceCostPerLevelBalancingData == null)
			{
				LogError("No ResourceCost for Level found: " + key);
				return default(KeyValuePair<string, int>);
			}
			return new KeyValuePair<string, int>(resourceCostPerLevelBalancingData.FallbackItemName, resourceCostPerLevelBalancingData.FallbackItemCount);
		}

		public void AddCraftingCostsForItem(ref List<KeyValuePair<string, int>> craftingCosts, string itemName, int itemLevel, int amount = 1)
		{
			List<KeyValuePair<int, ResourceCostPerLevelBalancingData>> value;
			if (!m_resourceCostPerItemNameOrderedByLevel.TryGetValue(itemName, out value))
			{
				return;
			}
			ResourceCostPerLevelBalancingData resourceCostPerLevelBalancingData = null;
			foreach (KeyValuePair<int, ResourceCostPerLevelBalancingData> item in value)
			{
				if (resourceCostPerLevelBalancingData == null)
				{
					resourceCostPerLevelBalancingData = item.Value;
					continue;
				}
				if (item.Key > itemLevel)
				{
					break;
				}
				resourceCostPerLevelBalancingData = item.Value;
			}
			if (resourceCostPerLevelBalancingData != null)
			{
				AddCraftingCost(ref craftingCosts, resourceCostPerLevelBalancingData.FirstMaterialNameId, resourceCostPerLevelBalancingData.FirstMaterialAmount, amount);
				AddCraftingCost(ref craftingCosts, resourceCostPerLevelBalancingData.SecondMaterialNameId, resourceCostPerLevelBalancingData.SecondMaterialAmount, amount);
				AddCraftingCost(ref craftingCosts, resourceCostPerLevelBalancingData.ThirdMaterialNameId, resourceCostPerLevelBalancingData.ThirdMaterialAmount, amount);
			}
		}

		public bool IsCraftAble(CraftingRecipeGameData recipe, InventoryGameData inventoryGameData, int amount = 1)
		{
			if (m_inventoryService == null || m_lootService == null || m_worldBalancing == null)
			{
				LogError("Not every needed Service is implemented!");
				return false;
			}
			if (recipe == null)
			{
				return false;
			}
			Dictionary<string, LootInfoData> dictionary = m_lootService.GenerateLoot(recipe.GetResultLoot(), recipe.ItemData.Level);
			List<KeyValuePair<string, int>> craftingCosts = new List<KeyValuePair<string, int>>();
			foreach (KeyValuePair<string, LootInfoData> item in dictionary)
			{
				AddCraftingCostsForItem(ref craftingCosts, item.Key, item.Value.Level, amount);
			}
			return craftingCosts.Count > 0;
		}

		public bool IsCraftingPossible(CraftingRecipeGameData recipe, InventoryGameData inventoryGameData)
		{
			List<IInventoryItemGameData> failedItems = new List<IInventoryItemGameData>();
			return IsCraftingPossible(recipe, inventoryGameData, out failedItems);
		}

		public bool IsCraftingPossible(CraftingRecipeGameData recipe, InventoryGameData inventoryGameData, out List<IInventoryItemGameData> failedItems, int amount = 1)
		{
			if (m_inventoryService == null || m_lootService == null || m_worldBalancing == null)
			{
				LogError("Not every needed Service is implemented!");
				failedItems = new List<IInventoryItemGameData>();
				return false;
			}
			Dictionary<string, LootInfoData> dictionary = m_lootService.GenerateLoot(recipe.GetResultLoot(), recipe.ItemData.Level);
			List<KeyValuePair<string, int>> craftingCosts = new List<KeyValuePair<string, int>>();
			foreach (KeyValuePair<string, LootInfoData> item in dictionary)
			{
				AddCraftingCostsForItem(ref craftingCosts, item.Key, item.Value.Level, amount);
			}
			bool result = true;
			failedItems = new List<IInventoryItemGameData>();
			for (int i = 0; i < craftingCosts.Count; i++)
			{
				KeyValuePair<string, int> keyValuePair = craftingCosts[i];
				if (m_inventoryService.GetItemValue(inventoryGameData, keyValuePair.Key) < keyValuePair.Value)
				{
					result = false;
					failedItems.Add(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(inventoryGameData, 1, 1, keyValuePair.Key, keyValuePair.Value - m_inventoryService.GetItemValue(inventoryGameData, keyValuePair.Key)));
				}
			}
			return result;
		}

		public bool IsRerollPossible(InventoryGameData inventoryGameData)
		{
			return m_worldBalancing.RerollCraftingReqirement == null || (float)m_inventoryService.GetItemValue(inventoryGameData, m_worldBalancing.RerollCraftingReqirement.NameId) >= m_worldBalancing.RerollCraftingReqirement.Value;
		}

		public bool ExecuteRerollCost(InventoryGameData inventoryGameData, int amount = 1, bool isHardMode = false)
		{
			Requirement requirement = null;
			requirement = ((amount <= 1) ? m_worldBalancing.RerollCraftingReqirement : m_worldBalancing.RerollMultiCraftingReqirement);
			if (isHardMode)
			{
				return requirement == null || m_inventoryService.RemoveItem(inventoryGameData, requirement.NameId, 3, "crafting_reroll");
			}
			return requirement == null || m_inventoryService.RemoveItem(inventoryGameData, requirement.NameId, (int)requirement.Value, "crafting_reroll");
		}

		public Requirement GetRerollRequirement()
		{
			return m_worldBalancing.RerollCraftingReqirement;
		}

		public Requirement GetMultiRerollRequirement()
		{
			return m_worldBalancing.RerollMultiCraftingReqirement;
		}

		public List<IInventoryItemGameData> CraftItem(CraftingRecipeGameData recipe, InventoryGameData inventoryGameData, bool isAlchemy, int craftAmount = 1)
		{
			if (m_inventoryService == null || m_lootService == null || m_worldBalancing == null)
			{
				LogError("Not every needed Service is implemented!");
			}
			Dictionary<string, LootInfoData> dictionary = m_lootService.GenerateLoot(recipe.GetResultLoot(), recipe.ItemData.Level);
			List<KeyValuePair<string, int>> craftingCosts = new List<KeyValuePair<string, int>>();
			foreach (KeyValuePair<string, LootInfoData> item in dictionary)
			{
				AddCraftingCostsForItem(ref craftingCosts, item.Key, item.Value.Level, craftAmount);
			}
			bool flag = true;
			for (int i = 0; i < craftingCosts.Count; i++)
			{
				KeyValuePair<string, int> keyValuePair = craftingCosts[i];
				flag &= m_inventoryService.RemoveItem(inventoryGameData, keyValuePair.Key, keyValuePair.Value, "crafting");
			}
			if (!flag)
			{
				LogError("Not enough Resources for Crafting!");
				return null;
			}
			int num = 0;
			int value = 1;
			if (!isAlchemy)
			{
				if (!ItemCraftedOnce(inventoryGameData))
				{
					num = 3;
					m_inventoryService.AddItem(inventoryGameData, 1, 1, "unlock_itemcraftedonce", 1, "first_craft_equipment");
				}
				else if (ForcedBadRoll(inventoryGameData))
				{
					IInventoryItemGameData data = null;
					num = ((m_inventoryService.TryGetItemGameData(inventoryGameData, "forge_leveled", out data) && data.ItemData.Level > 1) ? ((data.ItemData.Level == 2) ? 1 : 2) : 0);
					ApplyBadRoll(inventoryGameData);
				}
				else
				{
					num = GetQuality(isAlchemy);
				}
			}
			else if (PotionCraftedOnce(inventoryGameData))
			{
				num = GetQuality(isAlchemy);
			}
			else
			{
				num = 2;
				m_inventoryService.AddItem(inventoryGameData, 1, 1, "unlock_potioncraftedonce", 1, "first_craft_alchemy");
			}
			if (recipe.BalancingData.RecipeCategoryType == InventoryItemType.Resources || recipe.BalancingData.RecipeCategoryType == InventoryItemType.Consumable || recipe.BalancingData.RecipeCategoryType == InventoryItemType.Ingredients)
			{
				value = GetAmountByQuality(num);
				foreach (LootInfoData value2 in dictionary.Values)
				{
					value2.Value = value;
					value2.Quality = num;
				}
			}
			DebugLog("created item with quality " + num);
			List<IInventoryItemGameData> list = m_lootService.RewardLoot(inventoryGameData, num, dictionary, "crafting", EquipmentSource.Crafting, craftAmount);
			foreach (IInventoryItemGameData item2 in list)
			{
				if (item2 is EquipmentGameData)
				{
					EquipmentGameData equipmentGameData = item2 as EquipmentGameData;
					EquipmentData data2 = equipmentGameData.Data;
					object firstCraftingBal;
					if (craftingCosts.Count >= 1)
					{
						IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(craftingCosts[0].Key);
						firstCraftingBal = balancingData;
					}
					else
					{
						firstCraftingBal = null;
					}
					int firstCraftingCost = ((craftingCosts.Count >= 1) ? craftingCosts[0].Value : 0);
					object secondCraftingBal;
					if (craftingCosts.Count >= 2)
					{
						IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(craftingCosts[1].Key);
						secondCraftingBal = balancingData;
					}
					else
					{
						secondCraftingBal = null;
					}
					int secondCraftingCost = ((craftingCosts.Count >= 2) ? craftingCosts[1].Value : 0);
					object thirdCraftingBal;
					if (craftingCosts.Count >= 3)
					{
						IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(craftingCosts[2].Key);
						thirdCraftingBal = balancingData;
					}
					else
					{
						thirdCraftingBal = null;
					}
					data2.ScrapLoot = GenerateScrapLoot((IInventoryItemBalancingData)firstCraftingBal, firstCraftingCost, (IInventoryItemBalancingData)secondCraftingBal, secondCraftingCost, (IInventoryItemBalancingData)thirdCraftingBal, (craftingCosts.Count >= 3) ? craftingCosts[2].Value : 0, m_worldBalancing.CostToScrapLootRateCrafting, equipmentGameData.Data.ItemSource);
				}
			}
			foreach (IInventoryItemGameData item3 in list)
			{
				item3.ItemData.Quality = num;
				DebugLog(item3.Name);
			}
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2.Add("ItemName", list[0].Name);
			dictionary2.Add("ItemType", list[0].ItemBalancing.ItemType.ToString());
			dictionary2.Add("RollQuality", num.ToString("0"));
			dictionary2.Add("Amount", value.ToString("0"));
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary2);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("ItemCrafted", dictionary2);
			return list;
		}

		public int GetQuality(bool isAlchemy)
		{
			IInventoryItemGameData data;
			if (isAlchemy)
			{
				DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "cauldron_leveled", out data);
			}
			else
			{
				DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "forge_leveled", out data);
			}
			if (data.ItemData.Level == 1)
			{
				return QualityStrategy(DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").StandardDiceWeights);
			}
			if (data.ItemData.Level == 2)
			{
				return QualityStrategy(DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").GoldDiceWeights);
			}
			if (data.ItemData.Level == 3)
			{
				return QualityStrategy(DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").CrystalDiceWeights);
			}
			return 0;
		}

		private int QualityStrategy(List<float> weights)
		{
			Random random = new Random();
			double num = random.NextDouble();
			int result = 0;
			for (int i = 0; i < weights.Count; i++)
			{
				float num2 = weights[i] / 100f;
				if (num < (double)num2)
				{
					result = i;
					break;
				}
				num -= (double)num2;
			}
			return result;
		}

		private void AddCraftingCost(ref List<KeyValuePair<string, int>> craftingCosts, string itemName, int amount, int factor)
		{
			if (amount > 0 && !string.IsNullOrEmpty(itemName))
			{
				craftingCosts.Add(new KeyValuePair<string, int>(itemName, amount * factor));
			}
		}

		public int GetAmountByQuality(int quality)
		{
			int result = 1;
			switch (quality)
			{
			case 1:
				result = 2;
				break;
			case 2:
				result = 3;
				break;
			case 3:
				result = 4;
				break;
			}
			return result;
		}

		public void RerollItemQuality(InventoryGameData owner, ref IInventoryItemGameData item, int craftingAmount = 1)
		{
			int quality = item.ItemData.Quality;
			int quality2 = GetQuality(item.ItemBalancing.ItemType == InventoryItemType.Consumable || item.ItemBalancing.ItemType == InventoryItemType.Ingredients);
			if (item.ItemBalancing.ItemType == InventoryItemType.Resources || item.ItemBalancing.ItemType == InventoryItemType.Consumable || item.ItemBalancing.ItemType == InventoryItemType.Ingredients)
			{
				int num = 1;
				switch (quality2)
				{
				case 1:
					num = 2;
					break;
				case 2:
					num = 3;
					break;
				case 3:
					num = 4;
					break;
				}
				int amountByQuality = GetAmountByQuality(item.ItemData.Quality);
				if (amountByQuality < num)
				{
					DIContainerLogic.InventoryService.AddItem(owner, item.ItemData.Level, item.ItemData.Quality, item.ItemBalancing.NameId, (num - amountByQuality) * craftingAmount, "reroll_crafting_add");
				}
				else
				{
					DIContainerLogic.InventoryService.RemoveItem(owner, item.ItemBalancing.NameId, (num - amountByQuality) * craftingAmount, "reroll_crafting_remove");
				}
				item.ItemData.Quality = quality2;
			}
			else if (item.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment || item.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment)
			{
				if (ForcedGoodRoll(owner))
				{
					item.ItemData.Quality = 3;
					ApplyGoodRoll(owner);
				}
				else
				{
					item.ItemData.Quality = quality2;
				}
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ItemName", item.Name);
			dictionary.Add("ItemType", item.ItemBalancing.ItemType.ToString());
			dictionary.Add("OldItemQuality", quality.ToString("0"));
			dictionary.Add("NewItemQuality", quality2.ToString("0"));
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("CraftResultRerolled", dictionary);
		}

		public Dictionary<string, int> GenerateScrapLoot(IInventoryItemBalancingData firstCraftingBal, int firstCraftingCost, IInventoryItemBalancingData secondCraftingBal, int secondCraftingCost, IInventoryItemBalancingData thirdCraftingBal, int thirdCraftingCost, float costToScrapLootRate, EquipmentSource source)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			string text = string.Empty;
			string key = string.Empty;
			string key2 = string.Empty;
			if (source != EquipmentSource.SetItem)
			{
				if (firstCraftingBal != null)
				{
					CraftingItemBalancingData craftingItemBalancingData = firstCraftingBal as CraftingItemBalancingData;
					if (craftingItemBalancingData != null)
					{
						num = craftingItemBalancingData.ValueOfBaseItem * firstCraftingCost;
						text = craftingItemBalancingData.BaseItemNameId;
					}
					else
					{
						num = firstCraftingCost;
						text = firstCraftingBal.NameId;
					}
				}
				if (secondCraftingBal != null)
				{
					CraftingItemBalancingData craftingItemBalancingData2 = secondCraftingBal as CraftingItemBalancingData;
					if (craftingItemBalancingData2 != null)
					{
						num2 = craftingItemBalancingData2.ValueOfBaseItem * secondCraftingCost;
						key = craftingItemBalancingData2.BaseItemNameId;
					}
					else
					{
						num2 = secondCraftingCost;
						key = secondCraftingBal.NameId;
					}
				}
				if (thirdCraftingBal != null)
				{
					CraftingItemBalancingData craftingItemBalancingData3 = thirdCraftingBal as CraftingItemBalancingData;
					if (craftingItemBalancingData3 != null)
					{
						num3 = craftingItemBalancingData3.ValueOfBaseItem * thirdCraftingCost;
						key2 = craftingItemBalancingData3.BaseItemNameId;
					}
					else
					{
						num3 = thirdCraftingCost;
						key2 = thirdCraftingBal.NameId;
					}
				}
			}
			else
			{
				if (firstCraftingBal != null)
				{
					num = firstCraftingCost;
					text = firstCraftingBal.NameId;
				}
				if (secondCraftingBal != null)
				{
					num2 = secondCraftingCost;
					key = secondCraftingBal.NameId;
				}
				if (thirdCraftingBal != null)
				{
					num3 = thirdCraftingCost;
					key2 = thirdCraftingBal.NameId;
				}
			}
			int num4 = (int)Math.Floor((float)num * costToScrapLootRate);
			if (string.IsNullOrEmpty(text))
			{
				num4 = 0;
			}
			else if (num4 < 1)
			{
				num4 = 1;
				dictionary.Add(text, num4);
			}
			else
			{
				dictionary.Add(text, num4);
			}
			int num5 = (int)Math.Floor((float)num2 * costToScrapLootRate);
			if (num5 > 0)
			{
				dictionary.Add(key, num5);
			}
			int num6 = (int)Math.Floor((float)num3 * costToScrapLootRate);
			if (num6 > 0)
			{
				dictionary.Add(key2, num6);
			}
			return dictionary;
		}

		public List<IInventoryItemGameData> ScrapEquipment(InventoryGameData owner, EquipmentGameData equipment)
		{
			if (m_inventoryService == null || m_lootService == null)
			{
				LogError("Not every needed Service is implemented!");
			}
			List<IInventoryItemGameData> list = null;
			list = ((equipment.GetScrapLoot() != null) ? m_lootService.RewardLootGetInputCopy(owner, 0, m_lootService.GenerateLoot(equipment.GetScrapLoot(), 0), "scraped_item") : new List<IInventoryItemGameData>());
			m_inventoryService.RemoveItem(owner, equipment, 1, "scraped_item");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ItemName", equipment.Name);
			dictionary.Add("ItemType", equipment.ItemBalancing.ItemType.ToString());
			dictionary.Add("ItemQuality", equipment.ItemData.Quality.ToString("0"));
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("ItemScrapped", dictionary);
			return list;
		}

		public bool IsScrapPossible(InventoryGameData owner, IInventoryItemGameData equipment)
		{
			return equipment != null && equipment is EquipmentGameData && owner.Items[equipment.ItemBalancing.ItemType].Where((IInventoryItemGameData e) => (e as EquipmentGameData).BalancingData.RestrictedBirdId == (equipment as EquipmentGameData).BalancingData.RestrictedBirdId).ToList().Count > 1;
		}

		public float GetScrapResultFactor(EquipmentSource source)
		{
			switch (source)
			{
			case EquipmentSource.Gatcha:
				return m_worldBalancing.CostToScrapLootRateGacha;
			case EquipmentSource.SetItem:
				return m_worldBalancing.CostToScrapLootRateSet;
			default:
				return m_worldBalancing.CostToScrapLootRateCrafting;
			}
		}

		public Dictionary<string, int> GenerateScrapLootOnNewEquipment(int level, EquipmentSource source, string newEquipmentNameId, InventoryItemType newEquipmentType)
		{
			if (source != EquipmentSource.Crafting && source != EquipmentSource.LootBird)
			{
				List<KeyValuePair<string, int>> craftingCosts = new List<KeyValuePair<string, int>>();
				AddCraftingCostsForItem(ref craftingCosts, newEquipmentNameId, level);
				if (craftingCosts.Count > 0)
				{
					object firstCraftingBal;
					if (craftingCosts.Count >= 1)
					{
						IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(craftingCosts[0].Key);
						firstCraftingBal = balancingData;
					}
					else
					{
						firstCraftingBal = null;
					}
					int firstCraftingCost = ((craftingCosts.Count >= 1) ? craftingCosts[0].Value : 0);
					object secondCraftingBal;
					if (craftingCosts.Count >= 2)
					{
						IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(craftingCosts[1].Key);
						secondCraftingBal = balancingData;
					}
					else
					{
						secondCraftingBal = null;
					}
					int secondCraftingCost = ((craftingCosts.Count >= 2) ? craftingCosts[1].Value : 0);
					object thirdCraftingBal;
					if (craftingCosts.Count >= 3)
					{
						IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(craftingCosts[2].Key);
						thirdCraftingBal = balancingData;
					}
					else
					{
						thirdCraftingBal = null;
					}
					return GenerateScrapLoot((IInventoryItemBalancingData)firstCraftingBal, firstCraftingCost, (IInventoryItemBalancingData)secondCraftingBal, secondCraftingCost, (IInventoryItemBalancingData)thirdCraftingBal, (craftingCosts.Count >= 3) ? craftingCosts[2].Value : 0, GetScrapResultFactor(source), source);
				}
			}
			return null;
		}

		public bool IsScrapPossible(InventoryGameData owner, BannerItemGameData bannerItemGameData)
		{
			return bannerItemGameData != null && owner.Items[bannerItemGameData.BalancingData.ItemType].Count > 1;
		}

		public List<IInventoryItemGameData> ScrapBannerItem(InventoryGameData inventoryGameData, BannerItemGameData bannerItemGameData)
		{
			List<IInventoryItemGameData> list = null;
			list = ((bannerItemGameData.GetScrapLoot() != null) ? m_lootService.RewardLootGetInputCopy(inventoryGameData, 0, m_lootService.GenerateLoot(bannerItemGameData.GetScrapLoot(), 1), "scraped_item") : new List<IInventoryItemGameData>());
			m_inventoryService.RemoveItem(inventoryGameData, bannerItemGameData, 1, "scraped_item");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ItemName", bannerItemGameData.Name);
			dictionary.Add("ItemType", bannerItemGameData.ItemBalancing.ItemType.ToString());
			dictionary.Add("ItemQuality", bannerItemGameData.ItemData.Quality.ToString("0"));
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("ItemScrapped", dictionary);
			return list;
		}
	}
}
