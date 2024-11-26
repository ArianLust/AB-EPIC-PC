using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;

namespace ABH.GameDatas
{
	public class WorldGameData : GameDataBase<WorldBalancingData, WorldData>
	{
		public Dictionary<string, HotspotGameData> HotspotGameDatas = new Dictionary<string, HotspotGameData>();

		public Dictionary<int, string> StoryProgressHotspotIds = new Dictionary<int, string>();

		private HotspotGameData m_currentHotspotGameData;

		private HotspotGameData m_dailyHotspotGameData;

		public HotspotGameData CurrentHotspotGameData
		{
			get
			{
				return m_currentHotspotGameData;
			}
			set
			{
				m_currentHotspotGameData = value;
				if (Data != null)
				{
					Data.CurrentHotSpotInstance = m_currentHotspotGameData.Data;
				}
			}
		}

		public HotspotGameData DailyHotspotGameData
		{
			get
			{
				return m_dailyHotspotGameData;
			}
			set
			{
				m_dailyHotspotGameData = value;
				if (Data != null)
				{
					Data.DailyHotspotInstance = m_dailyHotspotGameData.Data;
				}
			}
		}

		[method: MethodImpl(32)]
		public event Action WorldChanged;

		public WorldGameData(string nameId)
			: base(nameId)
		{
		}

		public WorldGameData(WorldData instance)
			: base(instance)
		{
			for (int i = 0; i < instance.HotSpotInstances.Count; i++)
			{
				HotspotData hotspotData = instance.HotSpotInstances[i];
				HotspotGameData hotspotGameData = new HotspotGameData(hotspotData);
				if (hotspotGameData.BalancingData != null)
				{
					HotspotGameDatas.Remove(hotspotData.NameId);
					HotspotGameDatas.Add(hotspotData.NameId, hotspotGameData);
				}
			}
			foreach (HotspotBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<HotspotBalancingData>())
			{
				if (balancingData.ProgressId != 0 && !StoryProgressHotspotIds.ContainsKey(balancingData.ProgressId))
				{
					StoryProgressHotspotIds.Add(balancingData.ProgressId, balancingData.NameId);
				}
			}
			CurrentHotspotGameData = HotspotGameDatas[instance.CurrentHotSpotInstance.NameId];
			if (Data.DailyHotspotInstance == null || Data.DailyHotspotInstance.NameId != BalancingData.DailyHotspotNameId)
			{
				Data.DailyHotspotInstance = new HotspotData
				{
					LastVisitDateTime = DateTime.MinValue,
					Looted = false,
					RandomSeed = 0,
					NameId = BalancingData.DailyHotspotNameId,
					Score = 0,
					StarCount = 0,
					UnlockState = HotspotUnlockState.Hidden
				};
			}
			DailyHotspotGameData = new HotspotGameData(Data.DailyHotspotInstance);
		}

		public void RaiseWorldChanged(InventoryItemType itype)
		{
			if (this.WorldChanged != null)
			{
				this.WorldChanged();
			}
		}

		public HotspotGameData AddNewHotspot(HotspotBalancingData hotspotbal)
		{
			HotspotGameData hotspotGameData = new HotspotGameData(hotspotbal.NameId);
			HotspotGameDatas.Add(hotspotbal.NameId, hotspotGameData);
			Data.HotSpotInstances.Add(hotspotGameData.Data);
			return hotspotGameData;
		}

		protected override WorldData CreateNewInstance(string nameId)
		{
			WorldData worldData = new WorldData();
			worldData.NameId = nameId;
			worldData.HotSpotInstances = new List<HotspotData>();
			HotspotGameData hotspotGameData = new HotspotGameData(BalancingData.FirstHotspotNameId);
			HotspotGameDatas.Add(BalancingData.FirstHotspotNameId, hotspotGameData);
			worldData.HotSpotInstances.Add(hotspotGameData.Data);
			CurrentHotspotGameData = HotspotGameDatas[BalancingData.FirstHotspotNameId];
			foreach (HotspotBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<HotspotBalancingData>())
			{
				if (balancingData.ProgressId != 0 && !StoryProgressHotspotIds.ContainsKey(balancingData.ProgressId))
				{
					StoryProgressHotspotIds.Add(balancingData.ProgressId, balancingData.NameId);
				}
			}
			worldData.CurrentHotSpotInstance = CurrentHotspotGameData.Data;
			if (worldData.DailyHotspotInstance == null)
			{
				worldData.DailyHotspotInstance = new HotspotData
				{
					LastVisitDateTime = DateTime.MinValue,
					Looted = false,
					RandomSeed = 0,
					NameId = BalancingData.DailyHotspotNameId,
					Score = 0,
					StarCount = 0,
					UnlockState = HotspotUnlockState.Hidden
				};
			}
			DailyHotspotGameData = new HotspotGameData(worldData.DailyHotspotInstance);
			return worldData;
		}
	}
}
