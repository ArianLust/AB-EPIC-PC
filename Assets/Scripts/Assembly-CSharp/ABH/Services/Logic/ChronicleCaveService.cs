using System;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Services.Logic.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Services.Logic
{
	public class ChronicleCaveService
	{
		private IRequirementOperationService m_requirementService;

		private Action<string> DebugLog;

		private Action<string> ErrorLog;

		public ChronicleCaveService(IRequirementOperationService requirementService)
		{
			m_requirementService = requirementService;
		}

		public ChronicleCaveService SetDebugLog(Action<string> debugLog)
		{
			DebugLog = debugLog;
			return this;
		}

		public ChronicleCaveService SetErrorLog(Action<string> errorLog)
		{
			ErrorLog = errorLog;
			return this;
		}

		private void LogDebug(string message)
		{
			if (DebugLog != null)
			{
				DebugLog(message);
			}
		}

		private void LogError(string message)
		{
			if (ErrorLog != null)
			{
				ErrorLog(message);
			}
		}

		public void TravelToHotSpot(PlayerGameData playerGameData, int floorLevel, HotspotGameData target)
		{
			playerGameData.ChronicleCaveGameData.CurrentHotspotGameData = target;
			playerGameData.ChronicleCaveGameData.Data.CurrentBirdFloorIndex = floorLevel;
			playerGameData.SavePlayerData();
		}

		public bool SetupHotspotBattle(PlayerGameData playerGameData, HotspotGameData hotspot, List<BirdGameData> battleBirdList, BattleParticipantTableBalancingData addition, ChronicleCaveFloorBalancingData floor)
		{
			if (!DIContainerLogic.WorldMapService.EnterHotspot(playerGameData, hotspot))
			{
				return false;
			}
			Dictionary<Faction, Dictionary<string, float>> dictionary = new Dictionary<Faction, Dictionary<string, float>>();
			dictionary.Add(Faction.Birds, new Dictionary<string, float>());
			if (hotspot.Data.RandomSeed == 0)
			{
				hotspot.Data.RandomSeed = UnityEngine.Random.Range(1, int.MaxValue);
			}
			string firstPossibleBattle = DIContainerLogic.GetBattleService().GetFirstPossibleBattle(hotspot.BalancingData.BattleId, playerGameData, true);
			string backgroundAssetId = DIContainerBalancing.Service.GetBalancingData<ChronicleCaveBattleBalancingData>(firstPossibleBattle).BackgroundAssetId;
			string currentSponsoredBuff = playerGameData.Data.CurrentSponsoredBuff;
			playerGameData.Data.CurrentSponsoredBuff = string.Empty;
			BattleStartGameData battleStartGameData = new BattleStartGameData();
			battleStartGameData.m_BackgroundAssetId = backgroundAssetId;
			battleStartGameData.m_RageAvailiable = DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_rage");
			battleStartGameData.m_ChronicleCaveBattle = true;
			battleStartGameData.m_Birds = battleBirdList;
			battleStartGameData.m_BattleBalancingNameId = firstPossibleBattle;
			battleStartGameData.callback = delegate(IAsyncResult a)
			{
				OnHotspotBattleDone(playerGameData, hotspot, a);
			};
			battleStartGameData.m_Inventory = playerGameData.InventoryGameData;
			battleStartGameData.m_InvokerLevel = playerGameData.Data.Level;
			battleStartGameData.m_InjectableParticipantTable = addition;
			battleStartGameData.m_FactionBuffs = dictionary;
			battleStartGameData.m_EnvironmentalEffects = floor.EnvironmentalEffects;
			battleStartGameData.m_BattleRandomSeed = hotspot.Data.RandomSeed;
			battleStartGameData.m_SponsoredEnvironmentalEffect = currentSponsoredBuff;
			battleStartGameData.m_IsChronicleCave = hotspot.IsChronicleCave();
			ClientInfo.CurrentBattleStartGameData = battleStartGameData;
			return true;
		}

		public void OnHotspotBattleDone(PlayerGameData playerGameData, HotspotGameData hotspot, IAsyncResult result)
		{
			LogDebug("Battle callback received!");
			if (result == null)
			{
				return;
			}
			LogDebug("Battle Async Result != null!");
			BattleEndGameData battleResult = DIContainerLogic.GetBattleService().EndBattle(result);
			LogDebug("Battle EndData is null: " + (battleResult == null));
			if (battleResult.m_WinnerFaction != 0)
			{
				return;
			}
			DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
			{
				if (hotspot.BalancingData.CooldownInSeconds != 0)
				{
					hotspot.Data.LastVisitDateTime = trustedTime;
				}
				DIContainerLogic.WorldMapService.CompleteHotSpot(playerGameData, hotspot, battleResult.m_BattlePerformanceStars, battleResult.m_Score);
				SetCCHotspotProgress(playerGameData, hotspot);
			});
		}

		private void SetCCHotspotProgress(PlayerGameData playerGameData, HotspotGameData hotspot)
		{
			if (hotspot.BalancingData.ProgressId > 0)
			{
				if (!playerGameData.SocialEnvironmentGameData.Data.LocationProgress.ContainsKey(LocationType.ChronicleCave))
				{
					playerGameData.SocialEnvironmentGameData.Data.LocationProgress.Add(LocationType.ChronicleCave, hotspot.BalancingData.ProgressId);
				}
				else if (playerGameData.SocialEnvironmentGameData.Data.LocationProgress[LocationType.ChronicleCave] < hotspot.BalancingData.ProgressId)
				{
					playerGameData.SocialEnvironmentGameData.Data.LocationProgress[LocationType.ChronicleCave] = hotspot.BalancingData.ProgressId;
				}
			}
		}
	}
}
