using System;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Services.Logic.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using ABH.Shared.Models.InventoryItems;

namespace ABH.Services.Logic
{
	public class InventoryOperationServiceNullImpl : IInventoryOperationService
	{
		public bool InitializeInventoryOperations()
		{
			return true;
		}

		public void InitializeNewInventory(InventoryGameData inventory)
		{
		}

		public IInventoryItemGameData InitItem(InventoryGameData inventory, IInventoryItemGameData item)
		{
			return item;
		}

		public IInventoryItemGameData AddItem(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo added, string addreason, EquipmentSource source = EquipmentSource.Loot)
		{
			return item;
		}

		public IInventoryItemGameData AddItem(InventoryGameData inventory, int level, int quality, string itemName, int added, string addreason, EquipmentSource source = EquipmentSource.Loot)
		{
			return null;
		}

		public bool TryGetItemGameData(InventoryGameData inventory, string itemName, out IInventoryItemGameData data)
		{
			data = null;
			return false;
		}

		public bool TryGetItemGameData(InventoryGameData inventory, IInventoryItemBalancingData itembal, out IInventoryItemGameData data)
		{
			data = null;
			return false;
		}

		public bool CheckForItem(InventoryGameData inventory, string itemName)
		{
			return false;
		}

		public int GetItemValue(InventoryData ownerData, string itemName)
		{
			return 1;
		}

		public int GetItemValue(InventoryGameData inventory, string itemName)
		{
			return 1;
		}

		public int GetItemValue(InventoryGameData inventory, IInventoryItemGameData item)
		{
			return 1;
		}

		public bool IsPersitentItem(IInventoryItemGameData item)
		{
			return true;
		}

		public bool RemoveItem(InventoryGameData inventory, IInventoryItemGameData item, int removed, string removereason, bool ignoreInventoryChange = false)
		{
			return true;
		}

		public bool RemoveItem(InventoryGameData inventory, string itemName, int removed, string removereason)
		{
			return true;
		}

		public int GetGlobalUpgradeLevel(InventoryGameData inventory, string upgradeName)
		{
			return 1;
		}

		public void AddUpgradeLevel(InventoryGameData inventory, string upgrade, int nextLevel)
		{
		}

		public IInventoryItemData GenerateNewInventoryItem(InventoryGameData inventory, int level, int quality, string nameId, int value)
		{
			BasicItemData basicItemData = new BasicItemData();
			basicItemData.Level = level;
			basicItemData.NameId = nameId;
			basicItemData.Value = value;
			basicItemData.Quality = quality;
			return basicItemData;
		}

		public IInventoryItemGameData GenerateNewInventoryItemGameData(InventoryGameData inventory, int level, int quality, string nameId, int value, EquipmentSource source)
		{
			return new BasicItemGameData((BasicItemData)GenerateNewInventoryItem(inventory, level, quality, nameId, value));
		}

		public IInventoryItemGameData GenerateNewInventoryItemGameData(InventoryGameData inventory, IInventoryItemData item)
		{
			return new BasicItemGameData((BasicItemData)item);
		}

		public IInventoryItemGameData ReinitNewInventoryItemGameData(InventoryGameData inventory, IInventoryItemData item)
		{
			return null;
		}

		public IInventoryItemGameData AddItem(InventoryGameData inventory, IInventoryItemGameData item, int added, string addreason, EquipmentSource source)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, LootInfoData> GetFallbackLootFromRecipe(CraftingRecipeGameData newRecipe, int level)
		{
			return null;
		}

		public IInventoryItemGameData AddItem(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo addInfo, Dictionary<string, string> addTrackingInfo, EquipmentSource source)
		{
			return item;
		}

		public IInventoryItemGameData AddItem(InventoryGameData inventory, int level, int quality, string itemName, int added, Dictionary<string, string> addTrackingInfo, EquipmentSource source)
		{
			return null;
		}

		public bool RemoveItem(InventoryGameData inventory, IInventoryItemGameData item, int removed, Dictionary<string, string> removeTrackingInfo, bool ignoreInventoryChange = false)
		{
			return false;
		}

		public bool RemoveItem(InventoryGameData inventory, string itemName, int removed, Dictionary<string, string> removeTrackingInfo)
		{
			return false;
		}
	}
}
