using System;
using System.Collections.Generic;
using ABH.Shared.Interfaces;

namespace ABH.Shared.BalancingData
{
	public class InventoryItemBalancingDataPovider
	{
		private const bool DEBUG_LOG_SPAM = false;

		private readonly List<IInventoryItemBalancingData> m_inventoryItemCacheList;

		private bool m_eventItemsSet;

		public Action<string> Log { get; set; }

		public Action<string> LogError { get; set; }

		public InventoryItemBalancingDataPovider()
		{
			m_inventoryItemCacheList = GetIInventoryItemBalancing();
		}

		private List<IInventoryItemBalancingData> GetIInventoryItemBalancing()
		{
			List<IInventoryItemBalancingData> list = new List<IInventoryItemBalancingData>();
			foreach (CraftingItemBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<CraftingItemBalancingData>())
			{
				list.Add(balancingData);
			}
			foreach (ClassItemBalancingData balancingData2 in DIContainerBalancing.Service.GetBalancingDataList<ClassItemBalancingData>())
			{
				list.Add(balancingData2);
			}
			foreach (EquipmentBalancingData balancingData3 in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>())
			{
				list.Add(balancingData3);
			}
			foreach (CraftingRecipeBalancingData balancingData4 in DIContainerBalancing.Service.GetBalancingDataList<CraftingRecipeBalancingData>())
			{
				list.Add(balancingData4);
			}
			foreach (BasicItemBalancingData balancingData5 in DIContainerBalancing.Service.GetBalancingDataList<BasicItemBalancingData>())
			{
				list.Add(balancingData5);
			}
			foreach (ConsumableItemBalancingData balancingData6 in DIContainerBalancing.Service.GetBalancingDataList<ConsumableItemBalancingData>())
			{
				list.Add(balancingData6);
			}
			foreach (MasteryItemBalancingData balancingData7 in DIContainerBalancing.Service.GetBalancingDataList<MasteryItemBalancingData>())
			{
				list.Add(balancingData7);
			}
			foreach (EventItemBalancingData balancingData8 in DIContainerBalancing.Service.GetBalancingDataList<EventItemBalancingData>())
			{
				list.Add(balancingData8);
			}
			foreach (BannerItemBalancingData balancingData9 in DIContainerBalancing.Service.GetBalancingDataList<BannerItemBalancingData>())
			{
				list.Add(balancingData9);
			}
			foreach (ClassSkinBalancingData balancingData10 in DIContainerBalancing.Service.GetBalancingDataList<ClassSkinBalancingData>())
			{
				list.Add(balancingData10);
			}
			return list;
		}

		public IInventoryItemBalancingData GetBalancingData(string nameId, bool ignoreCase = true)
		{
			int count = m_inventoryItemCacheList.Count;
			for (int i = 0; i < count; i++)
			{
				IInventoryItemBalancingData inventoryItemBalancingData = m_inventoryItemCacheList[i];
				if (string.Compare(inventoryItemBalancingData.NameId, nameId, ignoreCase) == 0)
				{
					return inventoryItemBalancingData;
				}
			}
			return null;
		}

		public List<IInventoryItemBalancingData> GetBalancingDataList()
		{
			return m_inventoryItemCacheList;
		}
	}
}
