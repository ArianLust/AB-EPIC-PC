using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;

namespace ABH.GameDatas
{
	public class HotspotGameData : GameDataBase<HotspotBalancingData, HotspotData>
	{
		private string m_overrideBattleGround = string.Empty;

		public HotSpotWorldMapViewBase WorldMapView { get; set; }

		public string OverrideBattleGround
		{
			get
			{
				return m_overrideBattleGround;
			}
			set
			{
				m_overrideBattleGround = value;
			}
		}

		public string StageNamePure
		{
			get
			{
				return DIContainerInfrastructure.GetLocaService().GetZoneName(BalancingData.ZoneLocaIdent.Replace("zone_", string.Empty));
			}
		}

		public string StageName
		{
			get
			{
				if (BalancingData.ZoneStageIndex > 0)
				{
					return DIContainerInfrastructure.GetLocaService().GetZoneName(BalancingData.ZoneLocaIdent) + " - " + BalancingData.ZoneStageIndex;
				}
				return DIContainerInfrastructure.GetLocaService().GetZoneName(BalancingData.ZoneLocaIdent);
			}
		}

		[method: MethodImpl(32)]
		public event Action<bool> HotSpotChanged;

		public HotspotGameData(string nameId)
			: base(nameId)
		{
		}

		public HotspotGameData(HotspotBalancingData balancing)
			: base(balancing)
		{
		}

		public HotspotGameData(HotspotData instance)
			: base(instance)
		{
		}

		public HotspotGameData(HotspotBalancingData balancing, HotspotData instance)
			: base(balancing, instance)
		{
		}

		public bool IsActive()
		{
			return Data.UnlockState == HotspotUnlockState.Active || Data.UnlockState == HotspotUnlockState.Resolved || Data.UnlockState == HotspotUnlockState.ResolvedNew || Data.UnlockState == HotspotUnlockState.ResolvedBetter;
		}

		public void RaiseHotspotChanged()
		{
			if (this.HotSpotChanged != null)
			{
				this.HotSpotChanged(false);
			}
		}

		protected override HotspotData CreateNewInstance(string nameId)
		{
			HotspotData hotspotData = new HotspotData();
			hotspotData.NameId = nameId;
			hotspotData.UnlockState = HotspotUnlockState.Hidden;
			hotspotData.StarCount = 0;
			return hotspotData;
		}

		public bool IsCompleted()
		{
			return Data.UnlockState == HotspotUnlockState.Resolved || Data.UnlockState == HotspotUnlockState.ResolvedNew || Data.UnlockState == HotspotUnlockState.ResolvedBetter;
		}

		public int GetStarCount()
		{
			return Data.StarCount;
		}

		public bool IsDungeon()
		{
			return BalancingData.NameId.Contains("_dungeon") && !(BalancingData is ChronicleCaveHotspotBalancingData) && BalancingData.EnterRequirements != null && BalancingData.EnterRequirements.FirstOrDefault().RequirementType == RequirementType.IsSpecificWeekday;
		}

		public bool IsChronicleCave()
		{
			return BalancingData is ChronicleCaveHotspotBalancingData;
		}

		public int GetPigLevelForHotspot(bool isHardModeDungeon = false)
		{
			if (BalancingData is ChronicleCaveHotspotBalancingData)
			{
				ChronicleCaveBattleBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<ChronicleCaveBattleBalancingData>(DIContainerLogic.GetBattleService().GetFirstPossibleBattle(BalancingData.BattleId, DIContainerInfrastructure.GetCurrentPlayer(), true, isHardModeDungeon));
				return balancingData.BaseLevel;
			}
			if (Data.UnlockState == HotspotUnlockState.Active || Data.UnlockState == HotspotUnlockState.Hidden || Data.UnlockState == HotspotUnlockState.Unknown || BalancingData.NameId.Contains("dungeon"))
			{
				return DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
			}
			if (Data.CompletionPlayerLevel == 0)
			{
				BattleBalancingData balancingData2 = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(DIContainerLogic.GetBattleService().GetFirstPossibleBattle(BalancingData.BattleId, DIContainerInfrastructure.GetCurrentPlayer(), false, isHardModeDungeon));
				return balancingData2.BaseLevel;
			}
			return Data.CompletionPlayerLevel;
		}
	}
}
