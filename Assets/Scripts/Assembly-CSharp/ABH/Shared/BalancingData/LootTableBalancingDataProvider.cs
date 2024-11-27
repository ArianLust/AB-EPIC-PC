using System;
using System.Collections.Generic;

namespace ABH.Shared.BalancingData
{
	public class LootTableBalancingDataProvider
	{
		private List<LootTableBalancingData> m_lootTableCacheList;

		public Action<string> Log { get; set; }

		public Action<string> LogError { get; set; }

		public LootTableBalancingDataProvider()
		{
			m_lootTableCacheList = GetIInventoryItemBalancing();
		}

		private List<LootTableBalancingData> GetIInventoryItemBalancing()
		{
			List<LootTableBalancingData> list = new List<LootTableBalancingData>();
			foreach (LootTableBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<LootTableBalancingData>())
			{
				list.Add(balancingData);
			}
			return list;
		}

		public void ResetCache()
		{
			m_lootTableCacheList = GetIInventoryItemBalancing();
		}

		public LootTableBalancingData GetBalancingData(string nameId)
		{
			for (int i = 0; i < m_lootTableCacheList.Count; i++)
			{
				LootTableBalancingData lootTableBalancingData = m_lootTableCacheList[i];
				if (lootTableBalancingData.NameId == nameId)
				{
					return lootTableBalancingData;
				}
			}
			return null;
		}

		public bool TryGetBalancingData(string nameId, out LootTableBalancingData balancing)
		{
			if (Log != null)
			{
				Log("[BalancingDataLoaderServiceProtobufImpl] TryGetBalancingData with name id " + nameId + " of type " + typeof(LootTableBalancingData).Name);
			}
			balancing = GetBalancingData(nameId);
			return balancing != null;
		}

		public List<LootTableBalancingData> GetBalancingDataList()
		{
			return m_lootTableCacheList;
		}
	}
}
