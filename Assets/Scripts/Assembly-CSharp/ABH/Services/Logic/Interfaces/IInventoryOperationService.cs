using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;

namespace ABH.Services.Logic.Interfaces
{
	public interface IInventoryOperationService
	{
		bool InitializeInventoryOperations();

		void InitializeNewInventory(InventoryGameData inventory);

		IInventoryItemGameData InitItem(InventoryGameData inventory, IInventoryItemGameData item);

		IInventoryItemGameData AddItem(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo addInfo, Dictionary<string, string> addTrackingInfo, EquipmentSource source);

		IInventoryItemGameData AddItem(InventoryGameData inventory, int level, int quality, string itemName, int added, Dictionary<string, string> addTrackingInfo, EquipmentSource source);

		IInventoryItemGameData AddItem(InventoryGameData inventory, IInventoryItemGameData item, ItemAddInfo addInfo, string addReason, EquipmentSource source);

		IInventoryItemGameData AddItem(InventoryGameData inventory, int level, int quality, string itemName, int added, string addReason, EquipmentSource source);

		bool TryGetItemGameData(InventoryGameData inventory, string itemName, out IInventoryItemGameData data);

		bool TryGetItemGameData(InventoryGameData inventory, IInventoryItemBalancingData itembal, out IInventoryItemGameData data);

		bool CheckForItem(InventoryGameData inventory, string itemName);

		int GetItemValue(InventoryData inventory, string itemName);

		int GetItemValue(InventoryGameData inventory, string itemName);

		int GetItemValue(InventoryGameData inventory, IInventoryItemGameData item);

		bool IsPersitentItem(IInventoryItemGameData item);

		bool RemoveItem(InventoryGameData inventory, IInventoryItemGameData item, int removed, Dictionary<string, string> removeTrackingInfo, bool ignoreInventoryChange = false);

		bool RemoveItem(InventoryGameData inventory, string itemName, int removed, Dictionary<string, string> removeTrackingInfo);

		bool RemoveItem(InventoryGameData inventory, IInventoryItemGameData item, int removed, string removeReason, bool ignoreInventoryChange = false);

		bool RemoveItem(InventoryGameData inventory, string itemName, int removed, string removeReason);

		int GetGlobalUpgradeLevel(InventoryGameData inventory, string upgradeName);

		void AddUpgradeLevel(InventoryGameData inventory, string upgrade, int nextLevel);

		IInventoryItemData GenerateNewInventoryItem(InventoryGameData inventory, int level, int quality, string nameId, int value);

		IInventoryItemGameData GenerateNewInventoryItemGameData(InventoryGameData inventory, int level, int quality, string nameId, int value, EquipmentSource source);

		IInventoryItemGameData GenerateNewInventoryItemGameData(InventoryGameData inventory, IInventoryItemData item);

		IInventoryItemGameData ReinitNewInventoryItemGameData(InventoryGameData inventory, IInventoryItemData item);

		Dictionary<string, LootInfoData> GetFallbackLootFromRecipe(CraftingRecipeGameData newRecipe, int level);
	}
}
