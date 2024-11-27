using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Services.Logic;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class PvPAIService
{
	public PvPAIDamageCalculation m_DamageCalculation;

	public PvPAITargeting m_Targeting;

	public PvPAIRageCalculation m_RageCalculation;

	public bool m_RageUsedThisTurn;

	private BattleGameData m_battleData;

	private CachedCombatats m_cachedCombatants;

	private DamageValues m_damageValues;

	private List<BirdCommand> m_currentTurn;

	private int currentCombatantNumber;

	private PvPAIBalancingData m_balancing;

	private BattleService m_service;

	private bool m_showLog = true;

	public PvPAIService(string balancingName = "standard_behaviour")
	{
		if (!DIContainerBalancing.Service.TryGetBalancingData<PvPAIBalancingData>(balancingName, out m_balancing))
		{
			DebugLog.Error("Could not find AI balancing for: " + balancingName);
		}
		m_service = DIContainerLogic.GetBattleService();
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Initialized AI Service");
		}
	}

	public List<BirdCommand> CalculateTurn(BattleGameData data = null)
	{
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Calculating turn...");
		}
		if (data != null)
		{
			m_RageUsedThisTurn = false;
			m_battleData = data;
		}
		else if (m_battleData == null)
		{
			DebugLog.Error("unknown parameter: BattleGameData m_battleData - in: PvPAIServices::CalculateTurn()");
		}
		ResetValues();
		ICombatant target = null;
		m_cachedCombatants.m_WeakestBird = m_Targeting.GetWeakestCombatantOfFaction(Faction.Birds, false, string.Empty, true);
		m_cachedCombatants.m_WeakestPig = m_Targeting.GetWeakestCombatantOfFaction(Faction.Pigs, true, string.Empty);
		if (m_showLog && m_cachedCombatants.m_WeakestBird != null)
		{
			DebugLog.Log("[PvPAIService] The weakest Target is: " + m_cachedCombatants.m_WeakestBird.CombatantName);
		}
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if (item.CombatantFaction == Faction.Birds && item.IsBanner)
			{
				target = item;
				break;
			}
		}
		List<BirdCommand> turnsToKill;
		if (m_DamageCalculation.IsKillPossible(target, out turnsToKill))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Attempting to kill banner this turn");
			}
			m_currentTurn = turnsToKill;
			return m_currentTurn;
		}
		if (FocusBird() && m_DamageCalculation.IsKillPossible(m_cachedCombatants.m_WeakestBird, out turnsToKill))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Attempting to kill this bird now: " + m_cachedCombatants.m_WeakestBird.CombatantName);
			}
			m_currentTurn = turnsToKill;
			return m_currentTurn;
		}
		if (m_cachedCombatants.m_LastTurnTarget != null && m_cachedCombatants.m_WeakestBird == m_cachedCombatants.m_LastTurnTarget)
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Keeping focus on the weakest target");
			}
			m_currentTurn = GetStandardBehaviour(m_cachedCombatants.m_LastTurnTarget);
			return m_currentTurn;
		}
		if ((float)Random.Range(0, 100) >= 100f - m_balancing.ChanceToUseRandomTarget)
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Just Attacking Random Birds");
			}
			m_currentTurn = GetStandardBehaviour(null);
			return m_currentTurn;
		}
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Just Attacking a weighted target");
		}
		m_currentTurn = GetStandardBehaviour(m_Targeting.RollForTarget());
		return m_currentTurn;
	}

	private void ResetValues()
	{
		m_currentTurn = new List<BirdCommand>();
		currentCombatantNumber = 0;
		m_cachedCombatants = new CachedCombatats();
		m_Targeting = new PvPAITargeting(this, m_battleData, m_cachedCombatants, m_balancing);
		m_DamageCalculation = new PvPAIDamageCalculation(this, m_battleData);
		m_RageCalculation = new PvPAIRageCalculation(this, m_battleData, m_balancing);
	}

	public ICombatant GetNextCombatant()
	{
		foreach (BirdCommand item in m_currentTurn)
		{
			if (item.m_Sortorder == currentCombatantNumber)
			{
				currentCombatantNumber++;
				return item.m_Source;
			}
		}
		return null;
	}

	public BirdCommand GetCommand(ICombatant source)
	{
		foreach (BirdCommand item in m_currentTurn)
		{
			if (item.m_Source == source)
			{
				return item;
			}
		}
		return null;
	}

	private List<BirdCommand> GetStandardBehaviour(ICombatant target)
	{
		List<BirdCommand> list = new List<BirdCommand>();
		if (m_battleData.IsRageFull(Faction.Pigs))
		{
			m_cachedCombatants.m_RageBird = m_RageCalculation.GetRageBirdForStandardSituation();
			if (m_showLog && m_cachedCombatants.m_RageBird != null)
			{
				DebugLog.Log("[PvPAIService] This bird should use the rage chili: " + m_cachedCombatants.m_RageBird.CombatantName);
			}
		}
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if (item.CombatantFaction != Faction.Pigs || item.IsBanner || item.ActedThisTurn)
			{
				continue;
			}
			BirdCommand birdCommand = new BirdCommand();
			birdCommand.m_Source = item;
			birdCommand.m_UsedSkill = m_Targeting.GetSkillForBird(item);
			birdCommand.m_Sortorder = -1;
			ICombatant tauntTarget = item.GetTauntTarget();
			if (m_cachedCombatants.m_RageBird == item)
			{
				birdCommand.m_Target = m_RageCalculation.GetRageTargetForBird(item);
				if (m_showLog)
				{
					DebugLog.Log("[PvPAIService] Rage Chili Target: " + birdCommand.m_Target.CombatantNameId);
				}
				list.Add(birdCommand);
				continue;
			}
			if (birdCommand.m_UsedSkill.Model.Balancing.TargetType == SkillTargetTypes.Attack)
			{
				if (tauntTarget == null)
				{
					if (target == null)
					{
						target = m_Targeting.GetRandomTarget();
					}
				}
				else
				{
					target = tauntTarget;
				}
				birdCommand.m_Target = target;
			}
			else if (m_cachedCombatants.m_CachedTargetForBuffingTauntingAlly != null)
			{
				birdCommand.m_Target = m_cachedCombatants.m_CachedTargetForBuffingTauntingAlly;
			}
			else if (birdCommand.m_UsedSkill.Model.Balancing.TargetType == SkillTargetTypes.Support)
			{
				birdCommand.m_Target = m_Targeting.GetTargetForSupportSkill(item);
				if (m_showLog)
				{
					if (birdCommand.m_Target != null)
					{
						DebugLog.Log("[PvPAIService] Target for support skill \"" + birdCommand.m_UsedSkill.Model.Balancing.AssetId + "\" is: " + birdCommand.m_Target.CombatantName);
					}
					else
					{
						DebugLog.Log("[PvPAIService] No valid target for support skill, switchting to attack");
					}
				}
				if (birdCommand.m_Target == null)
				{
					birdCommand.m_UsedSkill = item.GetSkill(0);
					if (tauntTarget != null)
					{
						target = tauntTarget;
					}
					birdCommand.m_Target = target;
				}
			}
			list.Add(birdCommand);
		}
		list = GetTurnOrder(list);
		if (m_showLog)
		{
			foreach (BirdCommand item2 in list)
			{
				DebugLog.Log("[PvPAIService] Turnorder for standard behaviour: " + item2.m_Sortorder + "  " + item2.m_Source.CombatantName + "  " + item2.m_UsedSkill.Model.Balancing.NameId);
			}
		}
		return list;
	}

	private List<BirdCommand> GetTurnOrder(List<BirdCommand> birdTurns)
	{
		int num = 0;
		foreach (BirdCommand birdTurn in birdTurns)
		{
			if (birdTurn.m_Sortorder == -1 && birdTurn.m_UsedSkill.Model.Balancing.TargetType == SkillTargetTypes.Support && birdTurn.m_UsedSkill.Model.Balancing.AssetId != "LightningFast")
			{
				birdTurn.m_Sortorder = num;
				num++;
			}
		}
		foreach (BirdCommand birdTurn2 in birdTurns)
		{
			if (birdTurn2.m_Sortorder == -1 && birdTurn2.m_UsedSkill.Model.Balancing.SkillTemplateType == "AttackWithPurgeSkill")
			{
				birdTurn2.m_Sortorder = num;
				num++;
			}
		}
		foreach (BirdCommand birdTurn3 in birdTurns)
		{
			if (num == 3)
			{
				break;
			}
			if (birdTurn3.m_Sortorder == -1)
			{
				birdTurn3.m_Sortorder = num;
				num++;
			}
		}
		return birdTurns;
	}

	private bool FocusBird()
	{
		int num = m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Birds && c.IsAlive && !c.IsBanner).Count();
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Check if we try to focus on a specific bird. Birds alive: " + num);
		}
		float num2 = Random.Range(0, 100);
		if (num == 3 && num2 < m_balancing.ChanceToFocusBirdWith3)
		{
			return true;
		}
		if (num == 2 && num2 < m_balancing.ChanceToFocusBirdWith2)
		{
			return true;
		}
		if (num == 1 && num2 < m_balancing.ChanceToFocusBirdWith1)
		{
			return true;
		}
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] There seems to be more important stuff than knocking out enemy birds");
		}
		return false;
	}
}
