using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackWithChainSkill : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_ChargeTurns;

		private float m_AttackCount;

		private float m_ChainCount;

		private float m_ReductionPerChain;

		private string m_AddtionalEffectInfo = string.Empty;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			base.Model.SkillParameters.TryGetValue("chain", out m_ChainCount);
			base.Model.SkillParameters.TryGetValue("chain_reduction", out m_ReductionPerChain);
			foreach (string key in base.Model.SkillParameters.Keys)
			{
				if (base.Model.SkillParameters[key] == -1f)
				{
					m_AddtionalEffectInfo = key;
				}
			}
			ActionsOnDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				List<ICombatant> list = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != source.CombatantFaction && target != c && c.CombatantView != null).ToList();
				float num = m_DamageMod * source.ModifiedAttack;
				for (int i = 0; (float)i < m_ChainCount; i++)
				{
					if (list.Count <= 0)
					{
						break;
					}
					ICombatant combatant = list[UnityEngine.Random.Range(0, list.Count)];
					list.Remove(combatant);
					float effectedParam = 1f;
					float num2 = num * Mathf.Pow((100f - m_ReductionPerChain) / 100f, i + 1);
					float currentSkillAttackValue = m_Source.CurrentSkillAttackValue;
					m_Source.CurrentSkillAttackValue = num2;
					effectedParam = DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(effectedParam, EffectTriggerType.OnReceiveDamage, combatant, m_Source);
					num2 *= effectedParam;
					DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(1f, EffectTriggerType.BeforeReceiveDamage, combatant, source);
					VisualEffectSetting setting = null;
					if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(m_AddtionalEffectInfo, out setting))
					{
						SpawnVisualEffects(VisualEffectSpawnTiming.Start, setting, new List<ICombatant> { combatant });
					}
					float effectedParam2 = 1f;
					if (num2 > 0f && source.CombatantMainHandEquipment != null && m_ApplyPerks && DIContainerLogic.GetBattleService().ApplyDelayedPerk(source.CombatantMainHandEquipment.BalancingData.Perk, source, combatant, battle, ref effectedParam2) && combatant.CombatantView != null)
					{
						combatant.CombatantView.m_CurrentPerkType = source.CombatantMainHandEquipment.BalancingData.Perk.Type;
					}
					combatant.ReceiveDamage(num2, source);
					battle.m_DamageLastTurn += DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(combatant, battle, source);
					m_Source.CurrentSkillAttackValue = currentSkillAttackValue;
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
			dictionary.Add("{value_7}", string.Empty + m_ChainCount);
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
