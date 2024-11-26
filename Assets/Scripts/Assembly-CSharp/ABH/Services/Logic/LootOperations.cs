using System;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.Services.Logic
{
	public class LootOperations
	{
		public bool IsProbabilitySatisfied(LootTableEntry lootEntry)
		{
			return UnityEngine.Random.value <= lootEntry.Probability;
		}

		public void GenerateAndSetLootBase(IHasLootBalancing bdata, IHasLootData idata, int level, ref int wheelIndex)
		{
			Dictionary<string, LootInfoData> lootedItems = new Dictionary<string, LootInfoData>();
			foreach (string key in bdata.LootValueTables.Keys)
			{
				lootedItems.Add(key, new LootInfoData
				{
					Value = bdata.LootValueTables[key],
					Level = level,
					Quality = 0
				});
			}
			GenerateLootBase(bdata.LootValueTables, level, out lootedItems, ref wheelIndex);
			Dictionary<string, LootInfoData> dictionary = new Dictionary<string, LootInfoData>();
			foreach (string key2 in lootedItems.Keys)
			{
				AddItemValue(dictionary, key2, lootedItems[key2].Value, lootedItems[key2].Level);
			}
			idata.Loot = dictionary;
		}

		private void AddItemValue(Dictionary<string, LootInfoData> result, string key, int value, int level)
		{
			if (result.ContainsKey(key))
			{
				result[key].Value += value;
				return;
			}
			result.Add(key, new LootInfoData
			{
				Level = level,
				Quality = 0,
				Value = value
			});
		}

		public void GenerateLootBase(Dictionary<string, int> lootValueTable, int level, out Dictionary<string, LootInfoData> lootedItems, ref int wheelIndex)
		{
			lootedItems = new Dictionary<string, LootInfoData>();
			if (lootValueTable == null)
			{
				return;
			}
			foreach (string key in lootValueTable.Keys)
			{
				string text = key;
				if (text == null)
				{
					DebugLog.Error("Could not find item with name " + key);
				}
				else
				{
					GenerateLootRecursive(new KeyValuePair<string, int>(key, lootValueTable[key]), level, 1, lootedItems, 0, 10, ref wheelIndex);
				}
			}
		}

		public Dictionary<string, LootInfoData> GenerateLootPreview(Dictionary<string, int> loot, int level)
		{
			int wheelIndex = 0;
			return GenerateLoot(loot, level, 1, true, ref wheelIndex, false);
		}

		public Dictionary<string, LootInfoData> GenerateLoot(Dictionary<string, int> loot, int level)
		{
			int wheelIndex = 0;
			return GenerateLoot(loot, level, ref wheelIndex);
		}

		public Dictionary<string, LootInfoData> GenerateLoot(Dictionary<string, int> loot, int level, int wheelCount, ref int wheelIndex)
		{
			return GenerateLoot(loot, level, wheelCount, false, ref wheelIndex, false);
		}

		public Dictionary<string, LootInfoData> GenerateLootForcedWheelIndex(Dictionary<string, int> loot, int level, int wheelCount, ref int wheelIndex)
		{
			return GenerateLoot(loot, level, wheelCount, false, ref wheelIndex, true);
		}

		public Dictionary<string, LootInfoData> GenerateLoot(Dictionary<string, int> loot, int level, ref int wheelIndex)
		{
			return GenerateLoot(loot, level, 1, false, ref wheelIndex, false);
		}

		public Dictionary<string, LootInfoData> GenerateLoot(Dictionary<string, int> loot, int level, int wheelCount, bool preview, ref int wheelIndex, bool wheelForced)
		{
			Dictionary<string, LootInfoData> dictionary = new Dictionary<string, LootInfoData>();
			if (loot == null)
			{
				return new Dictionary<string, LootInfoData>();
			}
			foreach (KeyValuePair<string, int> item in loot)
			{
				LootTableBalancingData balancing = null;
				int value = item.Value;
				if (DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(item.Key, out balancing))
				{
					GenerateLootRecursive(new KeyValuePair<string, int>(item.Key, value), level, wheelCount, dictionary, 0, 10, ref wheelIndex, preview, wheelForced);
				}
				else if (value >= 1)
				{
					AddItemValue(dictionary, item.Key, value, level);
				}
			}
			return dictionary;
		}

		public void GenerateLootRecursive(KeyValuePair<string, int> item, int lootLevel, int lootWheelDropAmount, Dictionary<string, LootInfoData> lootedItems, int currentRecusionDepth, int maximumRecursionDepth, ref int wheelIndex, bool preview = false, bool wheelForced = false)
		{
			if (currentRecusionDepth >= maximumRecursionDepth)
			{
				throw new Exception("Reached Maximum Recursion Depth " + maximumRecursionDepth + " in Battle Loot Generation. (Maybe endless recursion loop)");
			}
			LootTableBalancingData balancing = null;
			if (DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(item.Key, out balancing))
			{
				if (balancing.LootTableEntries == null)
				{
					return;
				}
				if (preview)
				{
					GenerateListByInventory(item, lootLevel, lootedItems, currentRecusionDepth, maximumRecursionDepth, balancing, ref wheelIndex);
					return;
				}
				switch (balancing.Type)
				{
				case LootTableType.Inventory:
					GenerateListByInventory(item, lootLevel, lootedItems, currentRecusionDepth, maximumRecursionDepth, balancing, ref wheelIndex);
					break;
				case LootTableType.Probability:
					GenerateListByProbabilities(item, lootLevel, lootedItems, currentRecusionDepth, maximumRecursionDepth, balancing, ref wheelIndex);
					break;
				case LootTableType.Weighted:
					GenerateListByWeights(item, lootLevel, lootedItems, currentRecusionDepth, maximumRecursionDepth, balancing, ref wheelIndex);
					break;
				case LootTableType.Wheel:
					if (wheelForced)
					{
						GenerateListByWheelForced(item, lootLevel, lootWheelDropAmount, lootedItems, currentRecusionDepth, maximumRecursionDepth, balancing, ref wheelIndex);
					}
					else
					{
						GenerateListByWheel(item, lootLevel, lootWheelDropAmount, lootedItems, currentRecusionDepth, maximumRecursionDepth, balancing, ref wheelIndex);
					}
					break;
				case LootTableType.WheelForced:
					GenerateListByWheelForced(item, lootLevel, lootWheelDropAmount, lootedItems, currentRecusionDepth, maximumRecursionDepth, balancing, ref wheelIndex);
					break;
				default:
					GenerateListByInventory(item, lootLevel, lootedItems, currentRecusionDepth, maximumRecursionDepth, balancing, ref wheelIndex);
					break;
				}
			}
			else if (item.Value > 0)
			{
				AddItemValue(lootedItems, item.Key, item.Value, lootLevel);
			}
		}

		private void GenerateListByWeights(KeyValuePair<string, int> item, int lootLevel, Dictionary<string, LootInfoData> lootedItems, int currentRecusionDepth, int maximumRecursionDepth, LootTableBalancingData lootTable, ref int wheelIndex)
		{
			if (item.Value <= 0)
			{
				return;
			}
			float num = 0f;
			List<LootTableEntry> list = new List<LootTableEntry>();
			for (int i = 0; i < lootTable.LootTableEntries.Count; i++)
			{
				LootTableEntry lootTableEntry = lootTable.LootTableEntries[i];
				if (lootTableEntry.IsConditionSatisfied(lootLevel))
				{
					list.Add(lootTableEntry);
					num += lootTable.LootTableEntries[i].Probability;
				}
			}
			for (int j = 0; j < item.Value; j++)
			{
				float num2 = UnityEngine.Random.value * num;
				float num3 = 0f;
				for (int k = 0; k < list.Count; k++)
				{
					num3 += list[k].Probability;
					if (num3 >= num2)
					{
						ProcessLootEntry(lootLevel, lootedItems, currentRecusionDepth, maximumRecursionDepth, list[k], ref wheelIndex);
						wheelIndex = k;
						break;
					}
				}
			}
		}

		private void GenerateListByProbabilities(KeyValuePair<string, int> item, int lootLevel, Dictionary<string, LootInfoData> lootedItems, int currentRecusionDepth, int maximumRecursionDepth, LootTableBalancingData lootTable, ref int wheelIndex)
		{
			int num = 0;
			for (int i = 0; i < lootTable.LootTableEntries.Count; i++)
			{
				LootTableEntry lootTableEntry = lootTable.LootTableEntries[i];
				if (lootTableEntry.IsConditionSatisfied(lootLevel) && IsProbabilitySatisfied(lootTableEntry))
				{
					num++;
					ProcessLootEntry(lootLevel, lootedItems, currentRecusionDepth, maximumRecursionDepth, lootTableEntry, ref wheelIndex);
					if (item.Value != -1 && num >= item.Value)
					{
						wheelIndex = i;
						break;
					}
				}
			}
		}

		private void GenerateListByInventory(KeyValuePair<string, int> item, int lootLevel, Dictionary<string, LootInfoData> lootedItems, int currentRecusionDepth, int maximumRecursionDepth, LootTableBalancingData lootTable, ref int wheelIndex)
		{
			for (int i = 0; i < item.Value; i++)
			{
				for (int j = 0; j < lootTable.LootTableEntries.Count; j++)
				{
					LootTableEntry entry = lootTable.LootTableEntries[j];
					ProcessLootEntry(lootLevel, lootedItems, currentRecusionDepth, maximumRecursionDepth, entry, ref wheelIndex);
				}
			}
		}

		private void GenerateListByWheel(KeyValuePair<string, int> item, int lootLevel, int wheelDropAmount, Dictionary<string, LootInfoData> lootedItems, int currentRecusionDepth, int maximumRecursionDepth, LootTableBalancingData lootTable, ref int wheelIndex)
		{
			int num = (wheelIndex = GetLootIndexFromWheel(lootTable));
			for (int i = 0; i < wheelDropAmount; i++)
			{
				int wheelIndex2 = 0;
				ProcessLootEntry(lootLevel, lootedItems, currentRecusionDepth, maximumRecursionDepth, lootTable.LootTableEntries[num % lootTable.LootTableEntries.Count], ref wheelIndex2);
				num++;
			}
		}

		private void GenerateListByWheelForced(KeyValuePair<string, int> item, int lootLevel, int wheelDropAmount, Dictionary<string, LootInfoData> lootedItems, int currentRecusionDepth, int maximumRecursionDepth, LootTableBalancingData lootTable, ref int wheelIndex)
		{
			int num = (wheelIndex = wheelIndex);
			for (int i = 0; i < wheelDropAmount; i++)
			{
				int wheelIndex2 = 0;
				ProcessLootEntry(lootLevel, lootedItems, currentRecusionDepth, maximumRecursionDepth, lootTable.LootTableEntries[num % lootTable.LootTableEntries.Count], ref wheelIndex2);
				num++;
			}
		}

		private void ProcessLootEntry(int lootLevel, Dictionary<string, LootInfoData> lootedItems, int currentRecusionDepth, int maximumRecursionDepth, LootTableEntry entry, ref int wheelIndex)
		{
			string nameId = entry.NameId;
			if (nameId == null)
			{
				DebugLog.Error("Could not find loot item with name " + entry.NameId);
				return;
			}
			int val = entry.BaseValue + UnityEngine.Random.Range(0, entry.Span + 1);
			float num = Math.Max(0, val);
			KeyValuePair<string, int> item = new KeyValuePair<string, int>(entry.NameId, (int)num);
			int val2 = lootLevel + entry.CurrentPlayerLevelDelta;
			LootTableBalancingData balancing = null;
			if (!DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(item.Key, out balancing))
			{
				if (item.Value > 0)
				{
					AddItemValue(lootedItems, item.Key, item.Value, Math.Max(0, val2));
				}
			}
			else
			{
				currentRecusionDepth++;
				GenerateLootRecursive(item, Math.Max(0, val2), item.Value, lootedItems, currentRecusionDepth, maximumRecursionDepth, ref wheelIndex);
				currentRecusionDepth--;
			}
		}

		public int GetLootIndexFromWheel(LootTableBalancingData lootTable)
		{
			return UnityEngine.Random.Range(0, lootTable.LootTableEntries.Count);
		}

		public List<IInventoryItemGameData> GetItemsFromLoot(Dictionary<string, LootInfoData> loot, EquipmentSource source = EquipmentSource.LootBird, bool ReplacePotions = false)
		{
			return GetItemsFromLoot(null, loot, source, ReplacePotions);
		}

		public List<IInventoryItemGameData> GetItemsFromLoot(PlayerGameData player, Dictionary<string, LootInfoData> loot, EquipmentSource source = EquipmentSource.LootBird, bool ReplacePotions = false)
		{
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			foreach (string key in loot.Keys)
			{
				LootInfoData lootInfoData = loot[key];
				list.Add(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData((player == null) ? null : player.InventoryGameData, lootInfoData.Level, lootInfoData.Quality, key, lootInfoData.Value, source));
			}
			if (ReplacePotions)
			{
				List<IInventoryItemGameData> list2 = new List<IInventoryItemGameData>();
				for (int i = 0; i < list.Count; i++)
				{
					IInventoryItemGameData item = list[i];
					list2.Add(CheckForReplacementPotion(item));
				}
				return list2;
			}
			return list;
		}

		public IInventoryItemGameData CheckForReplacementPotion(IInventoryItemGameData item)
		{
			ConsumableItemGameData consumableItemGameData = item as ConsumableItemGameData;
			if (consumableItemGameData != null && string.IsNullOrEmpty(consumableItemGameData.BalancingData.ConsumableStatckingType))
			{
				return item;
			}
			if (consumableItemGameData == null || DIContainerInfrastructure.GetCurrentPlayer() == null)
			{
				return item;
			}
			InventoryGameData inventoryGameData = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData;
			List<IInventoryItemGameData> list = inventoryGameData.Items[InventoryItemType.CraftingRecipes];
			List<CraftingRecipeGameData> list2 = new List<CraftingRecipeGameData>();
			List<ConsumableItemGameData> list3 = new List<ConsumableItemGameData>();
			for (int i = 0; i < list.Count; i++)
			{
				IInventoryItemGameData inventoryItemGameData = list[i];
				CraftingRecipeGameData craftingRecipeGameData = inventoryItemGameData as CraftingRecipeGameData;
				if (craftingRecipeGameData != null && craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Consumable)
				{
					list2.Add(craftingRecipeGameData);
					List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level));
					ConsumableItemGameData item2 = itemsFromLoot[0] as ConsumableItemGameData;
					list3.Add(item2);
				}
			}
			for (int j = 0; j < list3.Count; j++)
			{
				ConsumableItemGameData consumableItemGameData2 = list3[j];
				if (consumableItemGameData2.BalancingData.ConsumableStatckingType == consumableItemGameData.BalancingData.ConsumableStatckingType && consumableItemGameData2.BalancingData.ConversionPoints != consumableItemGameData.BalancingData.ConversionPoints && consumableItemGameData2.BalancingData.NameId != consumableItemGameData.BalancingData.NameId)
				{
					consumableItemGameData2.ItemValue = consumableItemGameData.ItemValue;
					return consumableItemGameData2;
				}
			}
			return item;
		}

		public List<IInventoryItemGameData> RewardLoot(InventoryGameData inventory, int quality, Dictionary<string, LootInfoData> loot, string reason, EquipmentSource source = EquipmentSource.Loot, int amount = 1)
		{
			return RewardLoot(inventory, quality, loot, new Dictionary<string, string> { { "TypeOfGain", reason } }, source, amount);
		}

		public List<IInventoryItemGameData> RewardLoot(InventoryGameData inventory, int quality, Dictionary<string, LootInfoData> loot, Dictionary<string, string> trackDictionary, EquipmentSource source = EquipmentSource.Loot, int amount = 1)
		{
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			foreach (string key in loot.Keys)
			{
				LootInfoData lootInfoData = loot[key];
				list.Add(DIContainerLogic.InventoryService.AddItem(inventory, lootInfoData.Level, quality, key, lootInfoData.Value * amount, trackDictionary, source));
			}
			return list;
		}

		public List<IInventoryItemGameData> RewardLootGetInputCopy(InventoryGameData inventory, int quality, Dictionary<string, LootInfoData> loot, string reason, EquipmentSource source = EquipmentSource.Loot)
		{
			return RewardLootGetInputCopy(inventory, quality, loot, new Dictionary<string, string> { { "TypeOfGain", reason } }, source);
		}

		public List<IInventoryItemGameData> RewardLootGetInputCopy(InventoryGameData inventory, int quality, Dictionary<string, LootInfoData> loot, Dictionary<string, string> trackDictionary, EquipmentSource source = EquipmentSource.Loot)
		{
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			foreach (string key in loot.Keys)
			{
				LootInfoData lootInfoData = loot[key];
				int itemValue = DIContainerLogic.InventoryService.GetItemValue(inventory, key);
				IInventoryItemGameData inventoryItemGameData = DIContainerLogic.InventoryService.AddItem(inventory, lootInfoData.Level, quality, key, lootInfoData.Value, trackDictionary, source);
				list.Add(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(inventory, inventoryItemGameData.ItemData.Level, inventoryItemGameData.ItemData.Quality, inventoryItemGameData.ItemBalancing.NameId, inventoryItemGameData.ItemValue - itemValue));
			}
			return list;
		}
	}
}
