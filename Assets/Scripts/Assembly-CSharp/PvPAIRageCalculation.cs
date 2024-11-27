using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

public class PvPAIRageCalculation
{
	private BattleGameData m_battleData;

	private PvPAIBalancingData m_balancing;

	private PvPAIService m_service;

	private bool m_showLog = true;

	private PvPAIDamageCalculation m_damageCalculation
	{
		get
		{
			return m_service.m_DamageCalculation;
		}
	}

	private PvPAITargeting m_targeting
	{
		get
		{
			return m_service.m_Targeting;
		}
	}

	public PvPAIRageCalculation(PvPAIService service, BattleGameData gameData, PvPAIBalancingData balancing)
	{
		m_battleData = gameData;
		m_balancing = balancing;
		m_service = service;
	}

	private ICombatant[] GetRageBirdsByPrio()
	{
		ICombatant[] array = new ICombatant[5];
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Pigs && !c.IsBanner))
		{
			if (item.CombatantNameId.Contains("bird_red"))
			{
				array[m_balancing.RageRedPrio - 1] = item;
			}
			else if (item.CombatantNameId.Contains("bird_yellow"))
			{
				array[m_balancing.RageYellowPrio - 1] = item;
			}
			else if (item.CombatantNameId.Contains("bird_black"))
			{
				array[m_balancing.RageBlackPrio - 1] = item;
			}
			else if (item.CombatantNameId.Contains("bird_blue"))
			{
				array[m_balancing.RageBluePrio - 1] = item;
			}
			else if (item.CombatantNameId.Contains("bird_white"))
			{
				array[m_balancing.RageWhitePrio - 1] = item;
			}
		}
		return array;
	}

	public ICombatant GetRageBirdForKill(ICombatant target)
	{
		ICombatant[] rageBirdsByPrio = GetRageBirdsByPrio();
		int count = m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Birds).ToList().Count;
		for (int i = 0; i < 5; i++)
		{
			ICombatant combatant = rageBirdsByPrio[i];
			if (combatant != null)
			{
				if (combatant.CombatantNameId.Contains("bird_red") && (count == 1 || m_targeting.GetStrongestBird(Faction.Birds, string.Empty) == target))
				{
					return combatant;
				}
				if ((combatant.CombatantNameId.Contains("bird_blue") || combatant.CombatantNameId.Contains("bird_yellow")) && count == 1)
				{
					return combatant;
				}
				if (combatant.CombatantNameId == "bird_black")
				{
					return combatant;
				}
			}
		}
		return null;
	}

	public ICombatant GetRageBirdForStandardSituation()
	{
		ICombatant[] rageBirdsByPrio = GetRageBirdsByPrio();
		ICombatant combatant = null;
		for (int i = 0; i < rageBirdsByPrio.Length; i++)
		{
			combatant = rageBirdsByPrio[i];
			if (combatant == null)
			{
				continue;
			}
			bool flag = false;
			foreach (BattleEffectGameData value in combatant.CurrrentEffects.Values)
			{
				foreach (BattleEffect effect in value.m_Effects)
				{
					if (effect.EffectType == BattleEffectType.RageBlocked)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				combatant = null;
				continue;
			}
			if (combatant.CombatantNameId.Contains("bird_white"))
			{
				if (GetTotalTeamHealthPercentage(Faction.Pigs) < 0.5f)
				{
					return combatant;
				}
				combatant = null;
				continue;
			}
			if (combatant.CombatantNameId.Contains("bird_black"))
			{
				if (m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Birds).ToList().Count == 4)
				{
					return combatant;
				}
				combatant = null;
				continue;
			}
			if (combatant.CombatantNameId.Contains("bird_yellow") && combatant.CombatantClass.BalancingData.NameId == "class_rainbird")
			{
				if (m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Pigs).ToList().Count == 2)
				{
					combatant = null;
				}
				continue;
			}
			return combatant;
		}
		return combatant;
	}

	private float GetTotalTeamHealthPercentage(Faction team)
	{
		float num = 0f;
		float num2 = 0f;
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == team))
		{
			num += item.CurrentHealth / item.ModifiedHealth;
			num2 += 1f;
		}
		num /= num2;
		if (m_showLog)
		{
			DebugLog.Log(string.Concat("[PvPAIService] Total Percentage of Team ", team, " health is: ", num));
		}
		return num;
	}

	public float GetRageDamage(ICombatant source)
	{
		float num = 0f;
		float value = 0f;
		switch (source.CombatantNameId.Replace("pvp_", string.Empty))
		{
		case "bird_red":
		case "bird_blue":
		case "bird_black":
			if (source.GetSkill(2).Model.SkillParameters.TryGetValue("damage_in_percent", out value))
			{
				num = source.ModifiedAttack * (value / 100f) * m_damageCalculation.m_DamageValues.m_DamageIncrease;
			}
			break;
		case "bird_yellow":
			num = GetYellowRageDamage();
			break;
		}
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Rage Damage was calculated to be: " + num + " damage");
		}
		return num;
	}

	private float GetYellowRageDamage()
	{
		float num = 0f;
		float value = 0f;
		List<ICombatant> list = m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Pigs && !c.IsBanner).ToList();
		ICombatant combatant = list.FirstOrDefault();
		if (combatant.GetSkill(0).Model.SkillParameters.TryGetValue("damage_in_percent", out value))
		{
			num += combatant.ModifiedAttack * (value / 100f) * m_damageCalculation.m_DamageValues.m_DamageIncrease;
			num = m_damageCalculation.CheckForBonusDamage(combatant, num);
			if (list.Count == 3)
			{
				num *= 2f;
			}
			else if (list.Count == 2)
			{
				num *= 3f;
			}
			else if (list.Count == 1)
			{
				num *= 5f;
			}
		}
		if (list.Count > 1)
		{
			ICombatant combatant2 = list[1];
			if (combatant2.GetSkill(0).Model.SkillParameters.TryGetValue("damage_in_percent", out value))
			{
				num += combatant2.ModifiedAttack * (value / 100f) * m_damageCalculation.m_DamageValues.m_DamageIncrease;
				num = m_damageCalculation.CheckForBonusDamage(combatant2, num);
				num *= 2f;
			}
			if (list.Count > 2)
			{
				ICombatant combatant3 = list[2];
				if (combatant3.GetSkill(0).Model.SkillParameters.TryGetValue("damage_in_percent", out value))
				{
					num += combatant3.ModifiedAttack * (value / 100f) * m_damageCalculation.m_DamageValues.m_DamageIncrease;
					num = m_damageCalculation.CheckForBonusDamage(combatant3, num);
				}
			}
		}
		return num;
	}

	public ICombatant GetRageTargetForBird(ICombatant rageBird)
	{
		if (rageBird.CombatantNameId.Contains("bird_white") || rageBird.CombatantNameId.Contains("bird_yellow"))
		{
			return m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == rageBird.CombatantFaction).FirstOrDefault();
		}
		return m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != rageBird.CombatantFaction).FirstOrDefault();
	}
}
