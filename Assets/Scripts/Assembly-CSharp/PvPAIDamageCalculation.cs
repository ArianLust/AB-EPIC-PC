using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

public class PvPAIDamageCalculation
{
	public DamageValues m_DamageValues;

	private BattleGameData m_battleData;

	private PvPAIService m_service;

	private bool m_showLog = true;

	private PvPAITargeting m_targeting
	{
		get
		{
			return m_service.m_Targeting;
		}
	}

	private PvPAIRageCalculation m_rageCalculation
	{
		get
		{
			return m_service.m_RageCalculation;
		}
	}

	public PvPAIDamageCalculation(PvPAIService service, BattleGameData data)
	{
		m_battleData = data;
		m_service = service;
		m_DamageValues = new DamageValues();
	}

	public bool IsKillPossible(ICombatant target, out List<BirdCommand> turnsToKill)
	{
		turnsToKill = new List<BirdCommand>();
		if (target == null)
		{
			return false;
		}
		if (target.CurrrentEffects.ContainsKey("Devotion"))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Target is protected by Paladin, returning");
			}
			return false;
		}
		bool flag = false;
		float num = 0f;
		m_DamageValues.m_DamageIncrease = 1f;
		m_DamageValues.m_BonusDamagePerAttack = 0f;
		m_DamageValues.m_NextTurnDamage = 0f;
		CalculatePreTurnEffects(target);
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Calculated Pre turn effects!");
			DebugLog.Log("[PvPAIService] Damage factor: " + m_DamageValues.m_DamageIncrease);
			DebugLog.Log("[PvPAIService] Damage per hit: " + m_DamageValues.m_BonusDamagePerAttack);
			DebugLog.Log("[PvPAIService] Damage next turn: " + m_DamageValues.m_NextTurnDamage);
		}
		List<ICombatant> birdOrderForMaxDamage = GetBirdOrderForMaxDamage();
		if (m_showLog)
		{
			for (int i = 0; i < birdOrderForMaxDamage.Count; i++)
			{
				if (birdOrderForMaxDamage[i] != null)
				{
					DebugLog.Log("[PvPAIService] BirdOrder: " + i + " " + birdOrderForMaxDamage[i].CombatantName);
				}
			}
		}
		ICombatant combatant = null;
		if (m_battleData.IsRageFull(Faction.Pigs))
		{
			combatant = m_rageCalculation.GetRageBirdForKill(target);
			if (m_showLog && combatant != null)
			{
				DebugLog.Log("[PvPAIService] Rage will be used by this bird in case of kill: " + combatant.CombatantName);
			}
		}
		for (int j = 0; j < birdOrderForMaxDamage.Count; j++)
		{
			ICombatant combatant2 = birdOrderForMaxDamage[j];
			if (combatant2 == null)
			{
				continue;
			}
			string nameId = combatant2.CombatantClass.BalancingData.NameId;
			string replacementClassNameId = combatant2.CombatantClass.BalancingData.ReplacementClassNameId;
			if (combatant2.GetTauntTarget() != null && combatant2.GetTauntTarget() != target && nameId != "class_mage" && replacementClassNameId != "class_mage" && nameId != "class_rainbird" && replacementClassNameId != "class_rainbird" && nameId != "class_lightningbird" && replacementClassNameId != "class_lightningbird")
			{
				if (!m_battleData.IsRageFull(Faction.Pigs) || !combatant2.CombatantNameId.Contains("bird_black"))
				{
					continue;
				}
				combatant = combatant2;
			}
			num += m_DamageValues.m_BonusDamagePerAttack;
			float value = 0f;
			float num2 = 0f;
			if (combatant != null && combatant == combatant2)
			{
				num2 += m_rageCalculation.GetRageDamage(combatant) * m_DamageValues.m_DamageIncrease;
			}
			else
			{
				if (combatant2.GetSkill(0).Model.SkillParameters.TryGetValue("damage_in_percent", out value))
				{
					num2 = combatant2.ModifiedAttack * (value / 100f) * m_DamageValues.m_DamageIncrease;
				}
				if (m_showLog)
				{
					DebugLog.Log("[PvPAIService] Bird " + combatant2.CombatantName + " would do " + num2 + " base damage");
				}
				num2 = CheckForBonusDamage(combatant2, num2);
				num2 = CheckForDispel(combatant2, num2, target);
				float value2;
				if (combatant2.GetSkill(0).Model.SkillParameters.TryGetValue("attack_count", out value2))
				{
					num2 *= value2;
				}
				num2 += CheckForGangUp(combatant2);
			}
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Bird " + combatant2.CombatantName + " would do " + num2 + " recalculated damage");
			}
			num += num2;
			BirdCommand birdCommand = new BirdCommand();
			birdCommand.m_Source = combatant2;
			birdCommand.m_Sortorder = j;
			birdCommand.m_Target = target;
			bool flag2 = false;
			foreach (BattleEffectGameData value3 in combatant2.CurrrentEffects.Values)
			{
				foreach (BattleEffect effect in value3.m_Effects)
				{
					if (effect.EffectType == BattleEffectType.RageBlocked)
					{
						flag2 = true;
						break;
					}
				}
			}
			if (!flag2 && combatant != null && combatant == combatant2)
			{
				birdCommand.m_UsedSkill = combatant2.GetSkill(2);
				flag = true;
				if (combatant2.CombatantNameId.Contains("bird_yellow"))
				{
					birdCommand.m_Target = combatant2;
				}
			}
			else
			{
				birdCommand.m_UsedSkill = combatant2.GetSkill(0);
				if ((nameId == "class_lightningbird" || replacementClassNameId == "class_lightningbird") && m_battleData.m_CombatantsPerFaction[Faction.Birds].Count <= 1)
				{
					GetMostDamageFromLightningBird(combatant2, birdCommand);
				}
				CalculateDamageEffects(combatant2, target);
			}
			turnsToKill.Add(birdCommand);
		}
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Is kill possible?  totalPossibleDamage: " + num + " + m_nextTurnDamage: " + m_DamageValues.m_NextTurnDamage + "  vs CurrentHealth of target(" + target.CombatantNameId + ") : " + target.CurrentHealth);
		}
		if (num + m_DamageValues.m_NextTurnDamage >= target.CurrentHealth)
		{
			if (flag)
			{
				m_service.m_RageUsedThisTurn = true;
			}
			return true;
		}
		return false;
	}

	private void GetMostDamageFromLightningBird(ICombatant currentBird, BirdCommand attack)
	{
		attack.m_UsedSkill = currentBird.GetSkill(1);
		float num = 0f;
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if (item.CombatantFaction != Faction.Pigs || item.IsBanner)
			{
				continue;
			}
			float value = 0f;
			if (item.GetSkill(0).Model.SkillParameters.TryGetValue("damage_in_percent", out value))
			{
				float num2 = item.ModifiedAttack * (value / 100f) * m_DamageValues.m_DamageIncrease;
				if (num2 > num)
				{
					num = num2;
					attack.m_Target = item;
				}
			}
		}
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Lightning Bird would use support skill on: " + attack.m_Target.CombatantName);
		}
	}

	private float CheckForGangUp(ICombatant source)
	{
		if (!source.CurrrentEffects.ContainsKey("GangUp"))
		{
			return 0f;
		}
		float num = 0f;
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => !c.IsBanner && c.CombatantFaction == source.CombatantFaction))
		{
			if (item.CombatantNameId.Contains("bird_black"))
			{
				num += m_DamageValues.m_BonusDamagePerAttack;
				float value = 0f;
				SkillBalancingData balancing = item.GetSkill(0).Model.Balancing;
				if (balancing.SkillParameters.TryGetValue("damage_in_percent", out value))
				{
					num = item.ModifiedAttack * (value / 100f) * m_DamageValues.m_DamageIncrease;
				}
				if (balancing.SkillParameters.TryGetValue("bonus_damage", out value))
				{
					float num2 = item.CurrentHealth / item.ModifiedHealth * 100f;
					num *= 1f - (100f - num2) * value / 100f / 100f;
				}
			}
		}
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Bird is buffed by gangup and will do " + num + " bonus damage with blacks help");
		}
		return num;
	}

	private void CalculatePreTurnEffects(ICombatant target)
	{
		foreach (BattleEffectGameData value in target.CurrrentEffects.Values)
		{
			if (value.m_EffectType == SkillEffectTypes.Curse)
			{
				foreach (BattleEffect effect in value.m_Effects)
				{
					if (effect.EffectType == BattleEffectType.IncreaseDamageReceived && effect.EffectTrigger == EffectTriggerType.OnReceiveDamage)
					{
						m_DamageValues.m_DamageIncrease += effect.Values[0] / 100f;
					}
					else if (effect.EffectType == BattleEffectType.DoDamage && effect.EffectTrigger == EffectTriggerType.OnDealDamagePerTurn)
					{
						m_DamageValues.m_NextTurnDamage += value.m_Source.ModifiedAttack * effect.Values[0] / 100f;
					}
					else if (effect.EffectType == BattleEffectType.VolleyDamage && effect.EffectTrigger == EffectTriggerType.OnReceiveDamage)
					{
						m_DamageValues.m_BonusDamagePerAttack += value.m_Source.ModifiedAttack * effect.Values[0] / 100f;
					}
				}
			}
			else
			{
				if (value.m_EffectType != SkillEffectTypes.Blessing)
				{
					continue;
				}
				foreach (BattleEffect effect2 in value.m_Effects)
				{
					if (effect2.EffectType == BattleEffectType.HealCounter && effect2.EffectTrigger == EffectTriggerType.AfterReceiveDamage)
					{
						m_DamageValues.m_DamageIncrease -= effect2.Values[0] / 100f;
					}
					else if (effect2.EffectType == BattleEffectType.ReduceDamageReceived && effect2.EffectTrigger == EffectTriggerType.OnReceiveDamage)
					{
						m_DamageValues.m_DamageIncrease -= effect2.Values[0] / 100f;
					}
					else if (effect2.EffectType == BattleEffectType.DoHeal && effect2.EffectTrigger == EffectTriggerType.OnDealDamagePerTurn)
					{
						m_DamageValues.m_NextTurnDamage -= value.m_Target.ModifiedHealth * effect2.Values[0] / 100f;
					}
				}
			}
		}
	}

	private List<ICombatant> GetBirdOrderForMaxDamage()
	{
		SortedDictionary<int, ICombatant> sortedDictionary = new SortedDictionary<int, ICombatant>();
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if (item.CombatantFaction == Faction.Pigs && !item.IsBanner && !item.ActedThisTurn)
			{
				string nameId = item.CombatantClass.BalancingData.NameId;
				string replacementClassNameId = item.CombatantClass.BalancingData.ReplacementClassNameId;
				if (nameId == "class_skulkers" || replacementClassNameId == "class_skulkers")
				{
					sortedDictionary.Add(1, item);
				}
				else if (nameId == "class_thunderbird" || replacementClassNameId == "class_thunderbird")
				{
					sortedDictionary.Add(2, item);
				}
				else if (nameId == "class_marksmen" || replacementClassNameId == "class_marksmen")
				{
					sortedDictionary.Add(3, item);
				}
				else if (nameId == "class_tricksters" || replacementClassNameId == "class_tricksters")
				{
					sortedDictionary.Add(4, item);
				}
				else if (nameId == "class_captn" || replacementClassNameId == "class_captn")
				{
					sortedDictionary.Add(5, item);
				}
				else if (nameId == "class_berserk" || replacementClassNameId == "class_berserk")
				{
					sortedDictionary.Add(50, item);
				}
			}
		}
		int num = 10;
		foreach (ICombatant item2 in m_battleData.m_CombatantsByInitiative)
		{
			if (sortedDictionary.Count == 3)
			{
				break;
			}
			if (item2.CombatantFaction == Faction.Pigs && !item2.IsBanner && !item2.ActedThisTurn && !sortedDictionary.ContainsValue(item2))
			{
				sortedDictionary.Add(num, item2);
				num++;
			}
		}
		return sortedDictionary.Values.ToList();
	}

	public float CheckForBonusDamage(ICombatant currentBird, float birdDamage)
	{
		string nameId = currentBird.CombatantClass.BalancingData.NameId;
		string replacementClassNameId = currentBird.CombatantClass.BalancingData.ReplacementClassNameId;
		if (nameId == "class_berserk" || replacementClassNameId == "class_berserk")
		{
			float value = 0f;
			if (DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>("bird_attack_black_berserk").SkillParameters.TryGetValue("bonus_damage", out value))
			{
				float factionRage = m_battleData.GetFactionRage(currentBird.CombatantFaction);
				birdDamage *= (100f - factionRage) * value / 100f / 100f;
			}
		}
		else if (nameId == "class_avenger" || replacementClassNameId == "class_avenger")
		{
			float value2 = 0f;
			if (DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>("bird_attack_red_avenger").SkillParameters.TryGetValue("bonus_damage", out value2))
			{
				float num = currentBird.CurrentHealth / currentBird.ModifiedHealth * 100f;
				birdDamage *= 1f + (100f - num) * value2 / 100f / 100f;
			}
		}
		else if (nameId == "class_seadog" || replacementClassNameId == "class_seadog")
		{
			float value3 = 0f;
			if (DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>("bird_attack_black_seadog").SkillParameters.TryGetValue("bonus_damage", out value3))
			{
				float num2 = currentBird.CurrentHealth / currentBird.ModifiedHealth * 100f;
				birdDamage *= 1f - (100f - num2) * value3 / 100f / 100f;
			}
		}
		return birdDamage;
	}

	private float CheckForDispel(ICombatant currentBird, float birdDamage, ICombatant target)
	{
		if (currentBird.CombatantClass.BalancingData.NameId == "class_tricksters" || currentBird.CombatantClass.BalancingData.NameId == "class_captn")
		{
			foreach (BattleEffectGameData value2 in target.CurrrentEffects.Values)
			{
				if (value2.m_EffectType != SkillEffectTypes.Blessing)
				{
					continue;
				}
				foreach (BattleEffect effect in value2.m_Effects)
				{
					if (effect.EffectType == BattleEffectType.HealCounter && effect.EffectTrigger == EffectTriggerType.AfterReceiveDamage)
					{
						m_DamageValues.m_DamageIncrease += effect.Values[0] / 100f;
					}
					else if (effect.EffectType == BattleEffectType.ReduceDamageReceived && effect.EffectTrigger == EffectTriggerType.OnReceiveDamage)
					{
						m_DamageValues.m_DamageIncrease += effect.Values[0] / 100f;
					}
					else if (effect.EffectType == BattleEffectType.DoHeal && effect.EffectTrigger == EffectTriggerType.OnDealDamagePerTurn)
					{
						m_DamageValues.m_NextTurnDamage += value2.m_Target.ModifiedHealth * effect.Values[0] / 100f;
					}
				}
			}
			float value = 0f;
			if (currentBird.GetSkill(0).Model.SkillParameters.TryGetValue("damage_in_percent", out value))
			{
				birdDamage = currentBird.ModifiedAttack * (value / 100f) * m_DamageValues.m_DamageIncrease;
			}
		}
		return birdDamage;
	}

	private void CalculateDamageEffects(ICombatant currentBird, ICombatant target)
	{
		string nameId = currentBird.CombatantClass.BalancingData.NameId;
		string replacementClassNameId = currentBird.CombatantClass.BalancingData.ReplacementClassNameId;
		if (nameId == "class_skulkers" || replacementClassNameId == "class_skulkers")
		{
			float value = 0f;
			if (DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>("bird_attack_blues_skulkers").SkillParameters.TryGetValue("increase_in_percent", out value))
			{
				m_DamageValues.m_DamageIncrease += value / 100f;
			}
		}
		else if (nameId == "class_thunderbird" || replacementClassNameId == "class_thunderbird")
		{
			float value2 = 0f;
			if (DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>("bird_attack_yellow_thunderbird").SkillParameters.TryGetValue("increase_in_percent", out value2))
			{
				m_DamageValues.m_DamageIncrease += value2 / 100f;
			}
		}
		else if (nameId == "class_marksmen" || replacementClassNameId == "class_marksmen")
		{
			float value3 = 0f;
			if (DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>("bird_attack_yellow_thunderbird").SkillParameters.TryGetValue("volley_in_percent", out value3))
			{
				m_DamageValues.m_BonusDamagePerAttack += currentBird.ModifiedAttack * value3 / 100f;
			}
		}
		else if ((nameId == "class_druid" || replacementClassNameId == "class_druid") && !target.CurrrentEffects.ContainsKey("ThornyVine"))
		{
			float value4 = 0f;
			if (DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>("bird_attack_white_druid").SkillParameters.TryGetValue("dot_damage_in_percent", out value4))
			{
				m_DamageValues.m_NextTurnDamage += currentBird.ModifiedAttack * value4 / 100f;
			}
		}
		else if ((nameId == "class_rogues" || replacementClassNameId == "class_rogues") && !target.CurrrentEffects.ContainsKey("StickyGoo"))
		{
			float value5 = 0f;
			if (DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>("bird_attack_blues_rogues").SkillParameters.TryGetValue("dot_damage_in_percent", out value5))
			{
				m_DamageValues.m_NextTurnDamage += currentBird.ModifiedAttack * value5 / 100f;
			}
		}
		else if ((nameId == "class_rainbird" || replacementClassNameId == "class_rainbird") && !target.CurrrentEffects.ContainsKey("AcidRain"))
		{
			float value6 = 0f;
			if (DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>("bird_attack_yellow_rainbird").SkillParameters.TryGetValue("dot_damage_in_percent", out value6))
			{
				m_DamageValues.m_NextTurnDamage += currentBird.ModifiedAttack * value6 / 100f;
			}
		}
	}
}
