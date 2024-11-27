using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class LootWheelController : MonoBehaviour
{
	private List<List<IInventoryItemGameData>> m_itemListContainer;

	[SerializeField]
	private List<LootDisplayContoller> m_LootItemSlots = new List<LootDisplayContoller>();

	[SerializeField]
	private List<UISprite> m_Stars = new List<UISprite>();

	public void SetLootIcons(string lootTableNameId, int level, int stars)
	{
		LootTableBalancingData balancing = null;
		if (!DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(lootTableNameId, out balancing))
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		m_itemListContainer = new List<List<IInventoryItemGameData>>();
		for (int i = 0; i < m_Stars.Count; i++)
		{
			if (stars > i)
			{
				m_Stars[i].spriteName = m_Stars[i].spriteName.Replace("_Desaturated", string.Empty);
				continue;
			}
			m_Stars[i].spriteName = m_Stars[i].spriteName.Replace("_Desaturated", string.Empty);
			m_Stars[i].spriteName = m_Stars[i].spriteName + "_Desaturated";
		}
		Dictionary<string, LootInfoData> dictionary = new Dictionary<string, LootInfoData>();
		for (int j = 0; j < balancing.LootTableEntries.Count; j++)
		{
			LootTableEntry lootTableEntry = balancing.LootTableEntries[j];
			LootTableBalancingData balancing2 = null;
			if (DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(lootTableEntry.NameId, out balancing2))
			{
				DebugLog.Log("Entry was Chest: " + lootTableEntry.NameId);
				m_itemListContainer.Add(new List<IInventoryItemGameData>());
			}
			else
			{
				m_itemListContainer.Add(new List<IInventoryItemGameData> { DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, level, 1, lootTableEntry.NameId, lootTableEntry.BaseValue, EquipmentSource.LootBird) });
			}
		}
		for (int k = 0; k < m_itemListContainer.Count; k++)
		{
			LootDisplayType displayType = LootDisplayType.None;
			if ((k + 1) % 8 == 1)
			{
				displayType = LootDisplayType.Major;
			}
			if (m_itemListContainer[k].Count == 1)
			{
				m_LootItemSlots[(k + 1) % 8].SetModel(m_itemListContainer[k][0], new List<IInventoryItemGameData>(), displayType);
			}
			else if (m_itemListContainer[k].Count == 0)
			{
				DebugLog.Log("Empty Chest");
				m_LootItemSlots[(k + 1) % 8].SetModel(null, m_itemListContainer[k], displayType);
			}
			else
			{
				m_LootItemSlots[(k + 1) % 8].SetModel(null, m_itemListContainer[k], displayType);
				Invoke("SetLayerDelayed", 1f);
			}
		}
	}
}
