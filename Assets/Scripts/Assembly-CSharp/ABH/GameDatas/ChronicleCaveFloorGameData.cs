using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;

namespace ABH.GameDatas
{
	public class ChronicleCaveFloorGameData : GameDataBase<ChronicleCaveFloorBalancingData, ChronicleCaveFloorData>
	{
		public Dictionary<string, HotspotGameData> HotspotGameDatas = new Dictionary<string, HotspotGameData>();

		private SkillGameData m_EnvironmentalEffect;

		public SkillGameData PrimaryEnvironmentalEffect
		{
			get
			{
				return m_EnvironmentalEffect ?? (m_EnvironmentalEffect = new SkillGameData(BalancingData.EnvironmentalEffects.Values.FirstOrDefault()));
			}
		}

		[method: MethodImpl(32)]
		public event Action FloorChanged;

		public ChronicleCaveFloorGameData(string nameId)
			: base(nameId)
		{
			foreach (string chronicleCaveHotspotId in BalancingData.ChronicleCaveHotspotIds)
			{
				AddNewHotspot(DIContainerBalancing.Service.GetBalancingData<ChronicleCaveHotspotBalancingData>(chronicleCaveHotspotId));
			}
		}

		public ChronicleCaveFloorGameData(ChronicleCaveFloorData instance)
			: base(instance)
		{
			HotspotGameDatas = new Dictionary<string, HotspotGameData>();
			if (instance.HotSpotInstances == null)
			{
				instance.HotSpotInstances = new List<HotspotData>();
			}
			foreach (HotspotData hotSpotInstance in instance.HotSpotInstances)
			{
				HotspotGameData value = new HotspotGameData(DIContainerBalancing.Service.GetBalancingData<ChronicleCaveHotspotBalancingData>(hotSpotInstance.NameId), hotSpotInstance);
				HotspotGameDatas.Add(hotSpotInstance.NameId, value);
			}
		}

		public void RaiseChronicleCaveFloorChanged(InventoryItemType itype)
		{
			if (this.FloorChanged != null)
			{
				this.FloorChanged();
			}
		}

		public HotspotGameData AddNewHotspot(HotspotBalancingData hotspotbal)
		{
			HotspotGameData hotspotGameData = new HotspotGameData(hotspotbal);
			HotspotGameDatas.Add(hotspotbal.NameId, hotspotGameData);
			Data.HotSpotInstances.Add(hotspotGameData.Data);
			return hotspotGameData;
		}

		protected override ChronicleCaveFloorData CreateNewInstance(string nameId)
		{
			ChronicleCaveFloorData chronicleCaveFloorData = new ChronicleCaveFloorData();
			chronicleCaveFloorData.NameId = nameId;
			chronicleCaveFloorData.HotSpotInstances = new List<HotspotData>();
			return chronicleCaveFloorData;
		}

		public bool IsFinished()
		{
			return HotspotGameDatas[BalancingData.LastChronicleCaveHotspotId].Data.UnlockState >= HotspotUnlockState.ResolvedNew;
		}
	}
}
