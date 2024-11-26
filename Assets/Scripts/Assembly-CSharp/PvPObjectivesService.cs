using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using UnityEngine;

public class PvPObjectivesService
{
	private List<PvPObjectivesGameData> m_dailyObjectives;

	private int m_winsInARow;

	private int m_killedBirdsInThisBattle;

	private int m_usedRageInThisBattle;

	private int m_killedBirdsInThisRound;

	private List<int> m_takenGroupIds;

	public PvPObjectivesService(PlayerGameData player)
	{
		SetPersistedPvPObjectives(player);
	}

	public void SetPersistedPvPObjectives(PlayerGameData player)
	{
		m_dailyObjectives = new List<PvPObjectivesGameData>();
		foreach (PvPObjectiveData pvpObjective in player.Data.PvpObjectives)
		{
			m_dailyObjectives.Add(new PvPObjectivesGameData(pvpObjective));
		}
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			if (dailyObjective.BalancingData == null)
			{
				RollForNewObjectives();
				break;
			}
		}
	}

	public List<PvPObjectivesGameData> GetDailyObjectives()
	{
		if (m_dailyObjectives.Count == 0)
		{
			RollForNewObjectives();
		}
		return m_dailyObjectives;
	}

	public void RerollUnsolved()
	{
		if (m_dailyObjectives == null || m_dailyObjectives.Count <= 0)
		{
			RollForNewObjectives();
			return;
		}
		m_takenGroupIds = new List<int>();
		List<string> list = new List<string>();
		List<PvPObjectivesGameData> list2 = new List<PvPObjectivesGameData>();
		for (int i = 0; i < m_dailyObjectives.Count; i++)
		{
			PvPObjectivesGameData pvPObjectivesGameData = m_dailyObjectives[i];
			m_takenGroupIds.Add(pvPObjectivesGameData.BalancingData.DailyGroupId);
			if (pvPObjectivesGameData.Data.Solved)
			{
				list2.Add(pvPObjectivesGameData);
				continue;
			}
			list.Add(pvPObjectivesGameData.BalancingData.NameId);
			string difficulty = pvPObjectivesGameData.BalancingData.Difficulty;
			PvPObjectivesGameData randomObjective = GetRandomObjective(difficulty, list);
			m_takenGroupIds.Add(randomObjective.BalancingData.DailyGroupId);
			list2.Add(randomObjective);
		}
		m_dailyObjectives = list2;
		List<PvPObjectiveData> pvpObjectives = DIContainerInfrastructure.GetCurrentPlayer().Data.PvpObjectives;
		pvpObjectives.Clear();
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			pvpObjectives.Add(dailyObjective.Data);
		}
	}

	public void RollForNewObjectives()
	{
		m_dailyObjectives = new List<PvPObjectivesGameData>();
		m_takenGroupIds = new List<int>();
		List<string> list = new List<string>();
		List<PvPObjectiveData> pvpObjectives = DIContainerInfrastructure.GetCurrentPlayer().Data.PvpObjectives;
		if (pvpObjectives != null && pvpObjectives.Count > 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = 0;
			foreach (PvPObjectiveData item in pvpObjectives)
			{
				list.Add(item.NameId);
				if (item.Solved)
				{
					num++;
				}
			}
			dictionary.Add("SolvedPvpObjectives", num.ToString());
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("NewPvpObjectives", dictionary);
		}
		PvPObjectivesGameData randomObjective = GetRandomObjective("easy", list);
		m_takenGroupIds.Add(randomObjective.BalancingData.DailyGroupId);
		m_dailyObjectives.Add(randomObjective);
		list.Add(randomObjective.BalancingData.NameId);
		PvPObjectivesGameData randomObjective2 = GetRandomObjective("normal", list);
		m_takenGroupIds.Add(randomObjective2.BalancingData.DailyGroupId);
		list.Add(randomObjective2.BalancingData.NameId);
		PvPObjectivesGameData randomObjective3 = GetRandomObjective("hard", list);
		m_takenGroupIds.Add(randomObjective3.BalancingData.DailyGroupId);
		m_dailyObjectives.Add(randomObjective2);
		m_dailyObjectives.Add(randomObjective3);
		List<PvPObjectiveData> pvpObjectives2 = DIContainerInfrastructure.GetCurrentPlayer().Data.PvpObjectives;
		pvpObjectives2.Clear();
		if (randomObjective != null)
		{
			pvpObjectives2.Add(randomObjective.Data);
		}
		pvpObjectives2.Add(randomObjective2.Data);
		pvpObjectives2.Add(randomObjective3.Data);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
	}

	private PvPObjectivesGameData GetRandomObjective(string difficulty, List<string> takenObjectiveNames)
	{
		if (difficulty == "easy" && DIContainerInfrastructure.GetCurrentPlayer().Data.PvPTutorialDisplayState == 1)
		{
			return new PvPObjectivesGameData("winBattles_easy_tut");
		}
		int level = DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
		Dictionary<int, List<PvPObjectivesBalancingData>> dictionary = new Dictionary<int, List<PvPObjectivesBalancingData>>();
		foreach (PvPObjectivesBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<PvPObjectivesBalancingData>())
		{
			if (!(balancingData.Difficulty != difficulty) && !takenObjectiveNames.Contains(balancingData.NameId) && !m_takenGroupIds.Contains(balancingData.DailyGroupId) && balancingData.Playerlevel <= level)
			{
				if (!dictionary.ContainsKey(balancingData.DailyGroupId))
				{
					dictionary.Add(balancingData.DailyGroupId, new List<PvPObjectivesBalancingData>());
				}
				dictionary[balancingData.DailyGroupId].Add(balancingData);
			}
		}
		if (dictionary.Count == 0)
		{
			return null;
		}
		List<PvPObjectivesBalancingData> value = dictionary.ElementAt(Random.Range(0, dictionary.Count)).Value;
		return new PvPObjectivesGameData(value[Random.Range(0, value.Count)].NameId);
	}

	private void UpdateObjectives()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			if (dailyObjective.Data.Solved)
			{
				dailyObjective.m_SuccessDuringBattle = 0;
			}
			dailyObjective.Data.Progress += dailyObjective.m_SuccessDuringBattle;
			if (dailyObjective.Data.Progress >= dailyObjective.Amount && !dailyObjective.Data.Solved)
			{
				AchievementData achievementTracking = currentPlayer.Data.AchievementTracking;
				achievementTracking.ObjectivesCompleted++;
				if (achievementTracking.ObjectivesCompleted >= DIContainerBalancing.Service.GetBalancingData<AchievementBalancingData>("completeObjectives").Value && !achievementTracking.ObjectivesCompletedAchieved)
				{
					string achievementIdForStoryItemIfExists = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("completeObjectives");
					if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists))
					{
						DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists);
						achievementTracking.ObjectivesCompletedAchieved = true;
					}
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("PvpObjectiveName", dailyObjective.BalancingData.NameId);
				ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
				DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("PvpObjectiveSolved", dictionary);
				dailyObjective.Data.Solved = true;
				DIContainerLogic.InventoryService.AddItem(currentPlayer.InventoryGameData, 1, 1, "pvp_points_standard", dailyObjective.Reward, "objective_solved");
			}
			dailyObjective.Data.Progress = Mathf.Min(dailyObjective.BalancingData.Amount, dailyObjective.Data.Progress);
		}
	}

	private void ResetObjectives(BattleGameData data)
	{
		if (m_dailyObjectives == null)
		{
			RollForNewObjectives();
		}
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			switch (dailyObjective.RequirementType)
			{
			case ObjectivesRequirement.winAfterCoinLose:
				dailyObjective.m_SuccessDuringBattle = (data.m_PigsStartTurn ? 1 : 0);
				break;
			case ObjectivesRequirement.dontHeal:
			case ObjectivesRequirement.notUseBird:
			case ObjectivesRequirement.notKill:
			case ObjectivesRequirement.notUseRage:
			case ObjectivesRequirement.noSupportSkills:
				dailyObjective.m_SuccessDuringBattle = 1;
				break;
			default:
				dailyObjective.m_SuccessDuringBattle = 0;
				break;
			}
		}
	}

	public void BattleStarted(BattleGameData battleData)
	{
		ResetObjectives(battleData);
		m_killedBirdsInThisBattle = 0;
		m_usedRageInThisBattle = 0;
		m_killedBirdsInThisRound = 0;
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			if (dailyObjective.RequirementType == ObjectivesRequirement.winRow && !dailyObjective.Data.Solved)
			{
				m_winsInARow = dailyObjective.Data.Progress;
				dailyObjective.Data.Progress = 0;
			}
			if (dailyObjective.RequirementType != ObjectivesRequirement.protectBird || dailyObjective.Data.Solved)
			{
				continue;
			}
			foreach (ICombatant item in battleData.m_BirdsByInitiative)
			{
				if (item.CombatantNameId.Contains(dailyObjective.Requirement1))
				{
					dailyObjective.m_SuccessDuringBattle = 1;
				}
			}
		}
	}

	public void BattleWon(BattleGameData data)
	{
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			if (dailyObjective.Data.Solved)
			{
				continue;
			}
			int result = 0;
			bool flag = string.IsNullOrEmpty(dailyObjective.Requirement1);
			bool flag2 = string.IsNullOrEmpty(dailyObjective.Requirement2);
			switch (dailyObjective.RequirementType)
			{
			case ObjectivesRequirement.getAmountStars:
				if (int.TryParse(dailyObjective.Requirement1, out result) && data.m_BattleEndData.m_BattlePerformanceStars >= result)
				{
					dailyObjective.m_SuccessDuringBattle = 1;
				}
				break;
			case ObjectivesRequirement.notUseBird:
				CheckIfBirdsNotUsed(dailyObjective, data);
				break;
			case ObjectivesRequirement.useBird:
				CheckIfBirdsUsed(dailyObjective, data);
				break;
			case ObjectivesRequirement.useClass:
				CheckIfClassesUsed(dailyObjective, data);
				break;
			case ObjectivesRequirement.multiUseClasses:
				CheckMultiClasses(dailyObjective, data);
				break;
			case ObjectivesRequirement.withBirdsAlive:
			{
				int num2 = data.m_CombatantsPerFaction[Faction.Birds].Where((ICombatant c) => c.IsAlive && !c.IsBanner).Count();
				if (int.TryParse(dailyObjective.Requirement1, out result) && num2 >= result)
				{
					dailyObjective.m_SuccessDuringBattle = 1;
				}
				break;
			}
			case ObjectivesRequirement.winRow:
				dailyObjective.m_SuccessDuringBattle = m_winsInARow + 1;
				break;
			case ObjectivesRequirement.winTotal:
				dailyObjective.m_SuccessDuringBattle = 1;
				break;
			case ObjectivesRequirement.killBirdsInBattle:
				if (int.TryParse(dailyObjective.Requirement1, out result) && m_killedBirdsInThisBattle >= result)
				{
					dailyObjective.m_SuccessDuringBattle = 1;
				}
				break;
			case ObjectivesRequirement.useRage:
				if (int.TryParse(dailyObjective.Requirement1, out result) && m_usedRageInThisBattle >= result)
				{
					dailyObjective.m_SuccessDuringBattle = 1;
				}
				break;
			case ObjectivesRequirement.winWhileBirdsDead:
			{
				int num = 3 - data.m_PigsByInitiative.Count;
				if (int.TryParse(dailyObjective.Requirement1, out result) && num >= result)
				{
					dailyObjective.m_SuccessDuringBattle = 1;
				}
				break;
			}
			}
		}
		UpdateObjectives();
	}

	private void CheckIfBirdsNotUsed(PvPObjectivesGameData objective, BattleGameData data)
	{
		bool flag = string.IsNullOrEmpty(objective.Requirement2);
		foreach (ICombatant item in data.m_CombatantsPerFaction[Faction.Birds].Where((ICombatant c) => !c.IsBanner))
		{
			if (item.CombatantNameId.Contains(objective.Requirement1) || (!flag && item.CombatantNameId.Contains(objective.Requirement2)))
			{
				objective.m_SuccessDuringBattle = 0;
			}
		}
	}

	private void CheckIfBirdsUsed(PvPObjectivesGameData objective, BattleGameData data)
	{
		int num = 0;
		bool flag = string.IsNullOrEmpty(objective.Requirement2);
		foreach (ICombatant item in data.m_CombatantsPerFaction[Faction.Birds].Where((ICombatant c) => !c.IsBanner))
		{
			if (item.CombatantNameId.Contains(objective.Requirement1) || (!flag && item.CombatantNameId.Contains(objective.Requirement2)))
			{
				num++;
			}
		}
		if ((flag && num >= 1) || num >= 2)
		{
			objective.m_SuccessDuringBattle = 1;
		}
	}

	private void CheckIfClassesUsed(PvPObjectivesGameData objective, BattleGameData data)
	{
		int num = 0;
		bool flag = string.IsNullOrEmpty(objective.Requirement2);
		foreach (ICombatant item in data.m_CombatantsPerFaction[Faction.Birds].Where((ICombatant c) => !c.IsBanner))
		{
			string nameId = item.CombatantClass.BalancingData.NameId;
			if (nameId.Contains(objective.Requirement1) || (!flag && nameId.Contains(objective.Requirement2)))
			{
				num++;
			}
			string replacementClassNameId = item.CombatantClass.BalancingData.ReplacementClassNameId;
			if ((!string.IsNullOrEmpty(replacementClassNameId) && replacementClassNameId.Contains(objective.Requirement1)) || (!flag && replacementClassNameId.Contains(objective.Requirement2)))
			{
				num++;
			}
		}
		if ((flag && num >= 1) || num >= 2)
		{
			objective.m_SuccessDuringBattle = 1;
		}
	}

	private void CheckMultiClasses(PvPObjectivesGameData objective, BattleGameData data)
	{
		List<string> progressList = objective.GetProgressList();
		int num = 0;
		foreach (ICombatant item in data.m_CombatantsPerFaction[Faction.Birds].Where((ICombatant c) => !c.IsBanner))
		{
			string nameId = item.CombatantClass.BalancingData.NameId;
			if (!progressList.Contains(nameId))
			{
				progressList.Add(nameId);
				num++;
			}
		}
		objective.m_SuccessDuringBattle = num;
	}

	public void BattleLost()
	{
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			dailyObjective.m_SuccessDuringBattle = 0;
		}
	}

	public void TargetHealed(ICombatant target)
	{
		if (target.CombatantFaction == Faction.Pigs)
		{
			return;
		}
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			if (dailyObjective.RequirementType == ObjectivesRequirement.dontHeal && dailyObjective.m_SuccessDuringBattle > 0)
			{
				if (target.IsBanner && ObjectiveMatches(dailyObjective, "banner"))
				{
					dailyObjective.m_SuccessDuringBattle = 0;
				}
				else if (!target.IsBanner && ObjectiveMatches(dailyObjective, "birds"))
				{
					dailyObjective.m_SuccessDuringBattle = 0;
				}
			}
		}
	}

	public void SupportSkillUsed(ICombatant target)
	{
		if (target.CombatantFaction == Faction.Pigs)
		{
			return;
		}
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			if (dailyObjective.RequirementType == ObjectivesRequirement.noSupportSkills)
			{
				dailyObjective.m_SuccessDuringBattle = 0;
			}
		}
	}

	public void RoundOver()
	{
		m_killedBirdsInThisRound = 0;
	}

	private bool ObjectiveMatches(PvPObjectivesGameData objective, string targetId)
	{
		return objective.Requirement1 == targetId || objective.Requirement2 == targetId;
	}

	public void TargetKnockedOut(ICombatant target, BattleGameData battleMgr, ICombatant killer)
	{
		int result = 0;
		if (!target.IsBanner && target.CombatantFaction == Faction.Pigs)
		{
			m_killedBirdsInThisBattle++;
		}
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			if (target.CombatantFaction == Faction.Birds)
			{
				if (dailyObjective.RequirementType == ObjectivesRequirement.protectBird && target.CombatantNameId.Contains(dailyObjective.Requirement1))
				{
					dailyObjective.m_SuccessDuringBattle = 0;
				}
			}
			else
			{
				if (target.CombatantFaction != Faction.Pigs)
				{
					continue;
				}
				if (dailyObjective.RequirementType == ObjectivesRequirement.killBird && !target.IsBanner)
				{
					if (string.IsNullOrEmpty(dailyObjective.Requirement1) || target.CombatantNameId.Contains(dailyObjective.Requirement1) || (!string.IsNullOrEmpty(dailyObjective.Requirement2) && target.CombatantNameId.Contains(dailyObjective.Requirement2)))
					{
						dailyObjective.m_SuccessDuringBattle++;
					}
				}
				else if (dailyObjective.RequirementType == ObjectivesRequirement.notKill)
				{
					if (target.CombatantNameId.Contains(dailyObjective.Requirement1) || (!string.IsNullOrEmpty(dailyObjective.Requirement2) && target.CombatantNameId.Contains(dailyObjective.Requirement2)) || (dailyObjective.Requirement1 == "birds" && !target.IsBanner))
					{
						dailyObjective.m_SuccessDuringBattle = 0;
					}
				}
				else if (dailyObjective.RequirementType == ObjectivesRequirement.killWithBanner)
				{
					if (target.AttackTarget != null && target.AttackTarget.IsBanner && target.IsAttacking)
					{
						dailyObjective.m_SuccessDuringBattle++;
					}
				}
				else if (dailyObjective.RequirementType == ObjectivesRequirement.killBannerInEnemyTurn)
				{
					if (!target.CombatantView.m_BattleMgr.m_BirdTurnStarted && target.IsBanner)
					{
						dailyObjective.m_SuccessDuringBattle++;
					}
				}
				else if (dailyObjective.RequirementType == ObjectivesRequirement.killWithBird && !target.IsBanner)
				{
					if (killer != null && killer.CombatantNameId.Contains(dailyObjective.Requirement1) && (string.IsNullOrEmpty(dailyObjective.Requirement2) || target.CombatantNameId.Contains(dailyObjective.Requirement2)))
					{
						dailyObjective.m_SuccessDuringBattle++;
					}
				}
				else if (dailyObjective.RequirementType == ObjectivesRequirement.killBirdsInRound && !target.IsBanner && int.TryParse(dailyObjective.Requirement1, out result) && !string.IsNullOrEmpty(dailyObjective.Requirement1))
				{
					m_killedBirdsInThisRound++;
					if (m_killedBirdsInThisRound >= result)
					{
						dailyObjective.m_SuccessDuringBattle++;
						m_killedBirdsInThisRound = 0;
					}
				}
			}
		}
	}

	public void EnemiesKnockedOutinTotal(int count)
	{
		int result = 0;
		int result2 = 0;
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			if (dailyObjective.RequirementType == ObjectivesRequirement.killAtOnce && ((int.TryParse(dailyObjective.Requirement1, out result) && count == result) || (int.TryParse(dailyObjective.Requirement2, out result2) && count == result2)))
			{
				dailyObjective.m_SuccessDuringBattle++;
			}
		}
	}

	public void RageUsed()
	{
		m_usedRageInThisBattle++;
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			if (dailyObjective.RequirementType == ObjectivesRequirement.notUseRage)
			{
				dailyObjective.m_SuccessDuringBattle = 0;
			}
		}
	}

	public void RageUsedToKill(ICombatant victim)
	{
		if (victim.CombatantFaction == Faction.Birds)
		{
			return;
		}
		foreach (PvPObjectivesGameData dailyObjective in m_dailyObjectives)
		{
			if (dailyObjective.RequirementType == ObjectivesRequirement.killWithRage)
			{
				if (victim.IsBanner && ObjectiveMatches(dailyObjective, "banner"))
				{
					dailyObjective.m_SuccessDuringBattle = 1;
				}
				else if (!victim.IsBanner && ObjectiveMatches(dailyObjective, "birds"))
				{
					dailyObjective.m_SuccessDuringBattle++;
				}
			}
		}
	}

	public List<PvPObjectiveData> GetUnsolvedObjectives(PlayerGameData player)
	{
		return player.Data.PvpObjectives.FindAll((PvPObjectiveData obj) => !obj.Solved);
	}
}
