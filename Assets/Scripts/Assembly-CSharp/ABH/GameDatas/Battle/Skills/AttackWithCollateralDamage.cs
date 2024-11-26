using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackWithCollateralDamage : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_ChargeTurns;

		private float m_AttackCount;

		private float m_CollateralDamageMod;

		private float m_CollateralDamage;

		private float m_AdditionalEnemiesAffected;

		private string m_AddtionalEffectInfo = string.Empty;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			base.Model.SkillParameters.TryGetValue("additional_enemies_affected", out m_AdditionalEnemiesAffected);
			base.Model.SkillParameters.TryGetValue("collateral_damage_in_percent", out m_CollateralDamage);
			m_CollateralDamageMod = m_CollateralDamage / 100f;
			foreach (string key in base.Model.SkillParameters.Keys)
			{
				if (Math.Abs(base.Model.SkillParameters[key] - -1f) < 0.5f)
				{
					m_AddtionalEffectInfo = key;
				}
			}
			ActionsOnDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
				List<ICombatant> list = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != source.CombatantFaction && target != c && c.CombatantView != null).ToList();
				if (list.Count > 0)
				{
					int num = 0;
					while (list.Count > 0 && (float)num < m_AdditionalEnemiesAffected)
					{
						ICombatant combatant = list[UnityEngine.Random.Range(0, list.Count)];
						list.Remove(combatant);
						List<ICombatant> list2 = new List<ICombatant> { combatant };
						float effectedParam = 1f;
						float effectedParam2 = 1f;
						float num2 = source.ModifiedAttack * m_CollateralDamageMod;
						float currentSkillAttackValue = m_Source.CurrentSkillAttackValue;
						m_Source.CurrentSkillAttackValue = num2;
						effectedParam2 = DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(effectedParam2, EffectTriggerType.OnDealDamage, m_Source, combatant);
						num2 *= effectedParam2;
						effectedParam = DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(effectedParam, EffectTriggerType.OnReceiveDamage, combatant, m_Source);
						combatant.ReceiveDamage(num2 * effectedParam, source);
						VisualEffectSetting setting = null;
						if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(m_AddtionalEffectInfo, out setting))
						{
							SpawnVisualEffects(VisualEffectSpawnTiming.Start, setting, new List<ICombatant> { combatant });
						}
						DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(combatant, battle, source);
						m_Source.CurrentSkillAttackValue = currentSkillAttackValue;
						num++;
					}
				}
			});
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			dictionary.Add("{value_4}", string.Empty + m_AttackCount);
			dictionary.Add("{value_7}", string.Empty + DIContainerInfrastructure.GetFormatProvider().GetBattleStatsFormat(invoker.ModifiedAttack * (m_CollateralDamage / 100f)));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, dictionary);
		}
	}
}
