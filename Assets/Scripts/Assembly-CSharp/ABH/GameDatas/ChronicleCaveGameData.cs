using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;

namespace ABH.GameDatas
{
	public class ChronicleCaveGameData : GameDataBase<ChronicleCaveBalancingData, ChronicleCaveData>
	{
		public List<ChronicleCaveFloorGameData> ChronicleCaveFloorGameDatas = new List<ChronicleCaveFloorGameData>();

		public int CurrentFloorIndex;

		private HotspotGameData m_currentHotspotGameData;

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

		[method: MethodImpl(32)]
		public event Action ChronicleCaveChanged;

		public ChronicleCaveGameData(string nameId)
		{
			_instancedData = CreateNewInstance(nameId);
		}

		public ChronicleCaveGameData(ChronicleCaveData instance)
		{
			_instancedData = instance;
			ChronicleCaveFloorGameDatas = new List<ChronicleCaveFloorGameData>();
			if (instance.CronicleCaveFloors != null)
			{
				for (int i = 0; i < instance.CronicleCaveFloors.Count; i++)
				{
					ChronicleCaveFloorData instance2 = instance.CronicleCaveFloors[i];
					ChronicleCaveFloorGameData item = new ChronicleCaveFloorGameData(instance2);
					ChronicleCaveFloorGameDatas.Add(item);
				}
				if (instance.CurrentHotSpotInstance != null)
				{
					CurrentHotspotGameData = ChronicleCaveFloorGameDatas[instance.CurrentBirdFloorIndex].HotspotGameDatas[instance.CurrentHotSpotInstance.NameId];
				}
				CurrentFloorIndex = instance.CurrentBirdFloorIndex;
			}
		}

		public void RaiseChronicleCaveChanged(InventoryItemType itype)
		{
			if (this.ChronicleCaveChanged != null)
			{
				this.ChronicleCaveChanged();
			}
		}

		protected override ChronicleCaveData CreateNewInstance(string nameId)
		{
			ChronicleCaveData chronicleCaveData = new ChronicleCaveData();
			ChronicleCaveFloorGameDatas = new List<ChronicleCaveFloorGameData>();
			chronicleCaveData.CronicleCaveFloors = new List<ChronicleCaveFloorData>();
			CurrentFloorIndex = 0;
			chronicleCaveData.NameId = "chronicle_cave";
			return chronicleCaveData;
		}

		public bool SetNewHotspot(int floorIndex, string nameId)
		{
			if (!SwitchToFloor(floorIndex))
			{
				return false;
			}
			HotspotGameData value = null;
			if (ChronicleCaveFloorGameDatas[floorIndex].HotspotGameDatas.TryGetValue(nameId, out value))
			{
				CurrentHotspotGameData = value;
				return true;
			}
			return false;
		}

		public bool SwitchToFloor(int floorIndex)
		{
			bool flag = ChronicleCaveFloorGameDatas.Count > floorIndex;
			if (flag)
			{
				CurrentFloorIndex = floorIndex;
			}
			return flag;
		}

		public ChronicleCaveFloorGameData GetFloorAndCreateIfNext(int floorIndex)
		{
			if (floorIndex < 0)
			{
				return null;
			}
			if (ChronicleCaveFloorGameDatas.Count > floorIndex)
			{
				return GetFloor(floorIndex);
			}
			if (ChronicleCaveFloorGameDatas.Count < floorIndex)
			{
				return null;
			}
			ChronicleCaveFloorGameData chronicleCaveFloorGameData = new ChronicleCaveFloorGameData((floorIndex % DIContainerBalancing.Service.GetBalancingDataList<ChronicleCaveFloorBalancingData>().Count).ToString("0"));
			chronicleCaveFloorGameData.Data.FloorId = floorIndex;
			ChronicleCaveFloorGameDatas.Add(chronicleCaveFloorGameData);
			if (_instancedData.CronicleCaveFloors == null)
			{
				_instancedData.CronicleCaveFloors = new List<ChronicleCaveFloorData>();
			}
			_instancedData.CronicleCaveFloors.Add(chronicleCaveFloorGameData.Data);
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "chronicle_cave_progress", out data))
			{
				data.ItemData.Level = floorIndex + 1;
			}
			else
			{
				data = DIContainerLogic.InventoryService.AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "chronicle_cave_progress", 1, "Cave_Unlock");
				DIContainerLogic.InventoryService.AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "skill_env_cc_floor_01", 1, "cave_unlock");
			}
			if (CurrentHotspotGameData == null)
			{
				CurrentHotspotGameData = chronicleCaveFloorGameData.HotspotGameDatas[chronicleCaveFloorGameData.BalancingData.FirstChronicleCaveHotspotId];
			}
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			return chronicleCaveFloorGameData;
		}

		public ChronicleCaveFloorGameData GetFloor(int floorIndex)
		{
			if (ChronicleCaveFloorGameDatas.Count <= floorIndex || 0 > floorIndex)
			{
				return null;
			}
			return ChronicleCaveFloorGameDatas[floorIndex];
		}

		public Dictionary<string, HotspotGameData> GetAllHotSpots()
		{
			Dictionary<string, HotspotGameData> dictionary = new Dictionary<string, HotspotGameData>();
			foreach (ChronicleCaveFloorGameData chronicleCaveFloorGameData in ChronicleCaveFloorGameDatas)
			{
				foreach (KeyValuePair<string, HotspotGameData> hotspotGameData in chronicleCaveFloorGameData.HotspotGameDatas)
				{
					dictionary.SaveAdd(hotspotGameData.Key, hotspotGameData.Value);
				}
			}
			return dictionary;
		}
	}
}
