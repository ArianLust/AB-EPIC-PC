using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class PvPAITargeting
{
	private BattleGameData m_battleData;

	private PvPAIService m_service;

	private PvPAIBalancingData m_balancing;

	private CachedCombatats m_cachedCombatants;

	private DamageValues m_damageValues;

	private bool m_showLog = true;

	private PvPAIDamageCalculation m_damageCalculation
	{
		get
		{
			return m_service.m_DamageCalculation;
		}
	}

	public PvPAITargeting(PvPAIService service, BattleGameData battleData, CachedCombatats cachedCombatants, PvPAIBalancingData balancing)
	{
		m_service = service;
		m_battleData = battleData;
		m_cachedCombatants = cachedCombatants;
		m_balancing = balancing;
	}

	public SkillBattleDataBase GetSkillForBird(ICombatant bird)
	{
		if (m_battleData.IsRageFull(Faction.Pigs) && !m_service.m_RageUsedThisTurn && m_cachedCombatants.m_RageBird == bird)
		{
			m_service.m_RageUsedThisTurn = true;
			return bird.GetSkill(2);
		}
		string nameId = bird.CombatantClass.BalancingData.NameId;
		string replacementClassNameId = bird.CombatantClass.BalancingData.ReplacementClassNameId;
		if ((nameId == "class_samurai" || replacementClassNameId == "class_samurai") && m_battleData.IsRageFull(Faction.Birds))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Enemy Rage is full, samurai will protect");
			}
			return bird.GetSkill(1);
		}
		if ((nameId == "class_mage" || replacementClassNameId == "class_mage") && TauntingAllyWithoutDebuffExists("ShockShield"))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Ally is taunting, using shockshield");
			}
			return bird.GetSkill(1);
		}
		if ((nameId == "class_wizard" || replacementClassNameId == "class_wizard") && TauntingAllyWithoutDebuffExists("Energize"))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Ally is taunting, using energize");
			}
			return bird.GetSkill(1);
		}
		if ((nameId == "class_druid" || replacementClassNameId == "class_druid") && (OneAllyBelowPercentageHP(80f, true) || AmountofAlliesDamaged(3)))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Healing damaged team");
			}
			return bird.GetSkill(1);
		}
		if ((nameId == "class_princess" || replacementClassNameId == "class_princess") && (OneAllyBelowPercentageHP(80f, true) || TargetWithDebuffAmountsExists(2)))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Healing damaged or debuffed bird");
			}
			return bird.GetSkill(1);
		}
		if ((nameId == "class_cannoneer" || replacementClassNameId == "class_cannoneer") && TauntingAllyWithoutDebuffExists("Counter", false))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Ally is taunting, using counter");
			}
			return bird.GetSkill(1);
		}
		if ((nameId == "class_berserk" || replacementClassNameId == "class_berserk") && AmountOfLivingEnemies(3))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Using berserk support against 3 or more enemies");
			}
			return bird.GetSkill(1);
		}
		if ((nameId == "class_tricksters" || replacementClassNameId == "class_tricksters") && AmountOfDebuffsOnOwnTeam(3))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Trickster will cleanse team because of 3 or more debuffs");
			}
			return bird.GetSkill(1);
		}
		if ((nameId == "class_marksmen" || replacementClassNameId == "class_marksmen") && CheckMarksmenCondition(bird))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Marksmen should use support");
			}
			return bird.GetSkill(1);
		}
		if ((nameId == "class_avenger" || replacementClassNameId == "class_avenger") && m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Birds).Count() <= 1)
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Only banner alive, avenger will attack");
			}
			return bird.GetSkill(0);
		}
		return DIContainerLogic.GetBattleService().GetNextSkill(m_battleData, bird);
	}

	public ICombatant GetTargetForSupportSkill(ICombatant bird)
	{
		if (bird.CombatantClass.BalancingData.NameId != "class_avenger" && bird.CombatantNameId.Contains("bird_red"))
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Trying to protect  weakest ally");
			}
			return GetWeakestCombatantOfFaction(Faction.Pigs, true, bird.GetSkill(1).Model.Balancing.AssetId);
		}
		if (bird.CombatantClass.BalancingData.NameId == "class_priest")
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Priest supporting lowest HP ally (not self)");
			}
			return GetMostDamagedAlly(bird, bird.GetSkill(1).Model.Balancing.AssetId);
		}
		if (bird.CombatantNameId.Contains("bird_white") || bird.CombatantClass.BalancingData.NameId == "class_spies")
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Trying to heal ally with lowest % of health");
			}
			return GetMostDamagedAlly(null, bird.GetSkill(1).Model.Balancing.AssetId);
		}
		if (bird.CombatantNameId.Contains("bird_black") && bird.CombatantClass.BalancingData.NameId != "class_berserk")
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Trying to support ally with highest damage");
			}
			return GetHighestDamageBird(Faction.Pigs, bird, bird.GetSkill(1).Model.Balancing.AssetId);
		}
		if (bird.CombatantClass.BalancingData.NameId == "class_skulkers")
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Supporting special target for Skulkers");
			}
			return GetSkulkerSupportTarget(bird);
		}
		if (bird.CombatantClass.BalancingData.NameId == "class_lightingbird")
		{
			if (m_showLog)
			{
				DebugLog.Log("[PvPAIService] Supporting special target for Lightning-Bird");
			}
			return GetHighestDamageBird(Faction.Pigs, bird, bird.GetSkill(1).Model.Balancing.AssetId, true, false);
		}
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Trying to support target with highest hitpoints");
		}
		return GetStrongestBird(Faction.Pigs, bird.GetSkill(1).Model.Balancing.AssetId);
	}

	private bool TauntingAllyWithoutDebuffExists(string assetId = "", bool includeBanner = true)
	{
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Birds))
		{
			ICombatant tauntTarget = item.GetTauntTarget();
			if (tauntTarget != null)
			{
				if (!includeBanner && tauntTarget.IsBanner)
				{
					return false;
				}
				if (string.IsNullOrEmpty(assetId) || !tauntTarget.CurrrentEffects.ContainsKey(assetId))
				{
					m_cachedCombatants.m_CachedTargetForBuffingTauntingAlly = tauntTarget;
					return true;
				}
			}
		}
		return false;
	}

	private bool AmountofAlliesDamaged(int allies)
	{
		int num = 0;
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if (item.CombatantFaction == Faction.Pigs && item.CurrentHealth < item.ModifiedHealth)
			{
				num++;
			}
		}
		return num >= allies;
	}

	private bool OneAllyBelowPercentageHP(float percentage, bool includeBanner)
	{
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if (item.CombatantFaction == Faction.Pigs && (!item.IsBanner || includeBanner) && item.CurrentHealth / item.ModifiedHealth <= percentage / 100f)
			{
				return true;
			}
		}
		return false;
	}

	private bool TargetWithDebuffAmountsExists(int amountOfDebuffs)
	{
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if (item.CombatantFaction != Faction.Pigs)
			{
				continue;
			}
			int num = 0;
			foreach (BattleEffectGameData value in item.CurrrentEffects.Values)
			{
				if (value.m_EffectType == SkillEffectTypes.Curse)
				{
					num++;
				}
			}
			if (num >= amountOfDebuffs)
			{
				return true;
			}
		}
		return false;
	}

	private bool AmountOfLivingEnemies(int enemies)
	{
		int num = 0;
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if (item.CombatantFaction == Faction.Birds)
			{
				num++;
			}
		}
		return num >= enemies;
	}

	private bool AmountOfDebuffsOnOwnTeam(int debuffs)
	{
		int num = 0;
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if (item.CombatantFaction != Faction.Pigs)
			{
				continue;
			}
			foreach (BattleEffectGameData value in item.CurrrentEffects.Values)
			{
				if (value.m_EffectType == SkillEffectTypes.Curse)
				{
					num++;
				}
			}
		}
		return num >= debuffs;
	}

	private bool CheckMarksmenCondition(ICombatant bird)
	{
		if (bird.CurrentHealth / bird.ModifiedHealth > 0.6f)
		{
			return false;
		}
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if (item.CombatantFaction == Faction.Pigs && item != bird && item.CurrentHealth / item.ModifiedHealth > 0.8f && !item.CurrrentEffects.ContainsKey("Ambush"))
			{
				m_cachedCombatants.m_CachedTargetForBuffingTauntingAlly = item;
				return true;
			}
		}
		return false;
	}

	public ICombatant GetRandomTarget()
	{
		List<ICombatant> list = m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Birds).ToList();
		return list[Random.Range(0, list.Count)];
	}

	public ICombatant GetWeakestCombatantOfFaction(Faction team, bool includeBanner, string assetIdOfBuff, bool ignoreDyingTarget = false)
	{
		float num = 0f;
		ICombatant result = null;
		bool purgeAvailable = false;
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != team && !c.IsBanner))
		{
			if (item.GetSkill(0).Model.Balancing.SkillTemplateType == "AttackWithPurgeSkill" && item.CombatantClass.BalancingData.NameId != "class_lightningbird")
			{
				purgeAvailable = true;
			}
		}
		foreach (ICombatant item2 in m_battleData.m_CombatantsByInitiative)
		{
			if ((!string.IsNullOrEmpty(assetIdOfBuff) && item2.CurrrentEffects.ContainsKey(assetIdOfBuff)) || item2.CombatantFaction != team || ((includeBanner || item2.IsBanner) && !includeBanner))
			{
				continue;
			}
			float totalHealthIncludingBuffs = GetTotalHealthIncludingBuffs(item2, purgeAvailable);
			if (!(totalHealthIncludingBuffs < num) && num != 0f)
			{
				continue;
			}
			if (ignoreDyingTarget && totalHealthIncludingBuffs <= 0f)
			{
				if (m_showLog)
				{
					DebugLog.Log("[PvPAIService] Target " + item2.CombatantName + " will die next round anyway... Ignoring");
				}
			}
			else
			{
				num = totalHealthIncludingBuffs;
				result = item2;
			}
		}
		return result;
	}

	private ICombatant GetMostDamagedAlly(ICombatant caster, string assetIdOfBuff)
	{
		float num = 0f;
		ICombatant result = null;
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if (item.CombatantFaction == Faction.Pigs && (string.IsNullOrEmpty(assetIdOfBuff) || !item.CurrrentEffects.ContainsKey(assetIdOfBuff)) && (caster == null || caster != item))
			{
				float num2 = item.CurrentHealth / item.ModifiedHealth;
				if (num2 < num || num == 0f)
				{
					num = num2;
					result = item;
				}
			}
		}
		return result;
	}

	private ICombatant GetHighestDamageBird(Faction team, ICombatant self, string assetIdOfBuff, bool includeSelf = false, bool includeStunned = true)
	{
		float num = 0f;
		ICombatant result = null;
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == team && !c.IsBanner))
		{
			if ((includeSelf || item != self) && (includeStunned || !item.IsStunned) && (string.IsNullOrEmpty(assetIdOfBuff) || !item.CurrrentEffects.ContainsKey(assetIdOfBuff)))
			{
				float birdDamage = 0f;
				float value = 0f;
				if (item.GetSkill(0).Model.SkillParameters.TryGetValue("damage_in_percent", out value))
				{
					birdDamage = item.ModifiedAttack * (value / 100f) * m_damageCalculation.m_DamageValues.m_DamageIncrease;
				}
				birdDamage = m_damageCalculation.CheckForBonusDamage(item, birdDamage);
				if (birdDamage > num || num == 0f)
				{
					num = birdDamage;
					result = item;
				}
			}
		}
		return result;
	}

	public ICombatant GetStrongestBird(Faction team, string assetIdOfBuff)
	{
		float num = 0f;
		ICombatant result = null;
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
		{
			if ((string.IsNullOrEmpty(assetIdOfBuff) || !item.CurrrentEffects.ContainsKey(assetIdOfBuff)) && item.CombatantFaction == team && !item.IsBanner)
			{
				float totalHealthIncludingBuffs = GetTotalHealthIncludingBuffs(item, false);
				if (totalHealthIncludingBuffs > num || num == 0f)
				{
					num = totalHealthIncludingBuffs;
					result = item;
				}
			}
		}
		return result;
	}

	private ICombatant GetSkulkerSupportTarget(ICombatant skulker)
	{
		foreach (ICombatant item in m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Pigs && !c.IsBanner))
		{
			string nameId = item.CombatantClass.BalancingData.NameId;
			string replacementClassNameId = item.CombatantClass.BalancingData.ReplacementClassNameId;
			if (nameId == "class_druid" && CountBirdsWithoutDebuff("ThornyVine") > 0)
			{
				return item;
			}
			if ((nameId == "class_thunderbird" || replacementClassNameId == "class_thunderbird") && CountBirdsWithoutDebuff("RageOfThunder") > 0)
			{
				return item;
			}
			if ((nameId == "class_guardian" || replacementClassNameId == "class_guardian") && CountBirdsWithoutDebuff("Overpower") > 0)
			{
				return item;
			}
			if ((nameId == "class_cannoneer" || replacementClassNameId == "class_cannoneer") && CountBirdsWithoutDebuff("CoverFire") > 0)
			{
				return item;
			}
			if ((nameId == "class_rainbird" || replacementClassNameId == "class_rainbird") && CountBirdsWithoutDebuff("AcidRain") > 0)
			{
				return item;
			}
			if ((nameId == "class_skulkers" || replacementClassNameId == "class_skulkers") && CountBirdsWithoutDebuff("WeakSpot") > 0)
			{
				return item;
			}
		}
		return GetHighestDamageBird(Faction.Pigs, skulker, string.Empty, false, false);
	}

	private int CountBirdsWithoutDebuff(string assetId)
	{
		List<ICombatant> list = m_battleData.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Birds).ToList();
		int num = list.Count;
		foreach (ICombatant item in list)
		{
			if (item.CurrrentEffects.ContainsKey(assetId))
			{
				num--;
			}
		}
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Birds that don't have debuff " + assetId + ": " + num);
		}
		return num;
	}

	private float GetTotalHealthIncludingBuffs(ICombatant bird, bool purgeAvailable)
	{
		float num = bird.CurrentHealth;
		foreach (BattleEffectGameData value in bird.CurrrentEffects.Values)
		{
			if (value.m_EffectType == SkillEffectTypes.Curse)
			{
				foreach (BattleEffect effect in value.m_Effects)
				{
					if (effect.EffectType == BattleEffectType.IncreaseDamageReceived && effect.EffectTrigger == EffectTriggerType.OnReceiveDamage)
					{
						num /= 1f + effect.Values[0] / 100f;
					}
					else if (effect.EffectType == BattleEffectType.DoDamage && effect.EffectTrigger == EffectTriggerType.OnDealDamagePerTurn)
					{
						num -= value.m_Source.ModifiedAttack * effect.Values[0] / 100f;
					}
					else if (effect.EffectType == BattleEffectType.VolleyDamage && effect.EffectTrigger == EffectTriggerType.OnReceiveDamage)
					{
						num -= value.m_Source.ModifiedAttack * effect.Values[0] / 100f * 3f;
					}
				}
			}
			else
			{
				if (value.m_EffectType != SkillEffectTypes.Blessing || purgeAvailable)
				{
					continue;
				}
				foreach (BattleEffect effect2 in value.m_Effects)
				{
					if (effect2.EffectType == BattleEffectType.HealCounter && effect2.EffectTrigger == EffectTriggerType.AfterReceiveDamage)
					{
						if (effect2.Values[0] >= 100f)
						{
							return 1000000f;
						}
						num *= 100f / (100f - effect2.Values[0]);
					}
					else if (effect2.EffectType == BattleEffectType.ReduceDamageReceived && effect2.EffectTrigger == EffectTriggerType.OnReceiveDamage)
					{
						if (effect2.Values[0] >= 100f)
						{
							return 1000000f;
						}
						num *= 100f / (100f - effect2.Values[0]);
					}
					else if (effect2.EffectType == BattleEffectType.DoHeal && effect2.EffectTrigger == EffectTriggerType.OnDealDamagePerTurn)
					{
						num += value.m_Target.ModifiedHealth * effect2.Values[0] / 100f;
					}
				}
			}
		}
		return num;
	}

	public ICombatant RollForTarget()
	{
		if (TargetBanner())
		{
			foreach (ICombatant item in m_battleData.m_CombatantsByInitiative)
			{
				if (item.CombatantFaction == Faction.Birds && item.IsBanner)
				{
					if (m_showLog)
					{
						DebugLog.Log("[PvPAIService] Target Roll was Banner");
					}
					m_cachedCombatants.m_LastTurnTarget = item;
					return item;
				}
			}
		}
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Target Roll was weakest bird");
		}
		return m_cachedCombatants.m_WeakestBird;
	}

	private bool TargetBanner()
	{
		if (m_balancing == null)
		{
			DebugLog.Error("ERROR: AI BALANCING IS NULL!!");
			return false;
		}
		int num = 0;
		int num2 = m_balancing.BasicBannerWeight;
		foreach (ICombatant item in m_battleData.m_BirdsByInitiative)
		{
			if (item.CombatantFaction != 0)
			{
				continue;
			}
			float num3 = item.CurrentHealth / item.ModifiedHealth;
			if (item.IsBanner)
			{
				if (num3 < 0.8f)
				{
					num2 += m_balancing.AddBannerWeightBelow80;
				}
				if (num3 < 0.6f)
				{
					num2 += m_balancing.AddBannerWeightBelow60;
				}
				if (num3 < 0.4f)
				{
					num2 += m_balancing.AddBannerWeightBelow40;
				}
				if (num3 < 0.2f)
				{
					num2 += m_balancing.AddBannerWeightBelow20;
				}
			}
			else
			{
				num += m_balancing.BasicBirdWeight;
				if (num3 < 0.5f)
				{
					num2 += m_balancing.AddBirdWeightBelow50;
				}
				if (num3 < 0.3f)
				{
					num2 += m_balancing.AddBirdWeightBelow30;
				}
			}
		}
		if (m_showLog)
		{
			DebugLog.Log("[PvPAIService] Bannerweight: " + num2 + " vs combined bird weight: : " + num);
		}
		if (num + num2 == 0)
		{
			num = 1;
			num2 = 1;
		}
		float num4 = Random.Range(0, num + num2);
		return num4 > (float)num;
	}
}
