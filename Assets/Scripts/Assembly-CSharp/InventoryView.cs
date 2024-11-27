using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
	public List<InventoryItemInfo> ItemInfos = new List<InventoryItemInfo>();

	public void AddInventoryItemsToInventory(InventoryGameData inventory)
	{
		foreach (InventoryItemInfo itemInfo in ItemInfos)
		{
			DIContainerLogic.InventoryService.AddItem(inventory, itemInfo.Level, itemInfo.Quality, itemInfo.NameId, itemInfo.Amount, "by_Editor", itemInfo.EquipmentSource);
		}
	}
}
