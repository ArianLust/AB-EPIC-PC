using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Models.Generic;

namespace ABH.Services.Logic
{
	public class PlayerService
	{
		public float GetXPMultiplierForLevelDifference(PlayerGameData player, int targetLevel)
		{
			List<LevelRangeValueTable> levelRubberBandTables = player.WorldGameData.BalancingData.LevelRubberBandTables;
			if (levelRubberBandTables == null)
			{
				return 0f;
			}
			int num = targetLevel - player.Data.Level;
			float num2 = 0f;
			DebugLog.Log("[PlayerService] Level Delta is: " + num);
			for (int i = 0; i < levelRubberBandTables.Count; i++)
			{
				LevelRangeValueTable levelRangeValueTable = levelRubberBandTables[i];
				if (CheckIfLevelRangeIsValid(num, levelRangeValueTable))
				{
					num2 = levelRangeValueTable.Value;
					break;
				}
			}
			DebugLog.Log("[PlayerService] Apply XP modifier: " + num2 + "%");
			return num2;
		}

		private bool CheckIfLevelRangeIsValid(int deltaLevel, LevelRangeValueTable levelRangeValueTable)
		{
			if (deltaLevel > 0 && levelRangeValueTable.Value > 0)
			{
				return CheckIfPositiveLevelRangeIsValid(deltaLevel, levelRangeValueTable);
			}
			if (deltaLevel < 0 && levelRangeValueTable.Value < 0)
			{
				return CheckIfNegativeLevelRangeIsValid(deltaLevel, levelRangeValueTable);
			}
			return false;
		}

		private bool CheckIfPositiveLevelRangeIsValid(int deltaLevel, LevelRangeValueTable levelRangeValueTable)
		{
			return (deltaLevel >= levelRangeValueTable.FromLevel && levelRangeValueTable.ToLevel == 0) || (deltaLevel <= levelRangeValueTable.ToLevel && levelRangeValueTable.FromLevel == 0) || (deltaLevel <= levelRangeValueTable.ToLevel && deltaLevel >= levelRangeValueTable.FromLevel);
		}

		private bool CheckIfNegativeLevelRangeIsValid(int deltaLevel, LevelRangeValueTable levelRangeValueTable)
		{
			return (deltaLevel <= levelRangeValueTable.FromLevel && levelRangeValueTable.ToLevel == 0) || (deltaLevel >= levelRangeValueTable.ToLevel && levelRangeValueTable.FromLevel == 0) || (deltaLevel >= levelRangeValueTable.ToLevel && deltaLevel <= levelRangeValueTable.FromLevel);
		}

		public bool UpdateHighestPowerLevelEver(PlayerGameData player)
		{
			int playerHighestPowerLevel = DIContainerInfrastructure.GetPowerLevelCalculator().GetPlayerHighestPowerLevel(player);
			if (playerHighestPowerLevel > player.Data.HighestPowerLevelEver)
			{
				player.Data.HighestPowerLevelEver = playerHighestPowerLevel;
				return true;
			}
			return false;
		}

		public int GetPlayerMaxLevel()
		{
			return DIContainerBalancing.Service.GetBalancingDataList<ExperienceLevelBalancingData>().Count + 1;
		}
	}
}
