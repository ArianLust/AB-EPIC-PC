using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class BonusDamageAndRandomTarget : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_BonusDamage;

		private float m_SprayDamage;

		private float m_SprayCount;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_goo", out m_BonusDamage);
			base.Model.SkillParameters.TryGetValue("spray_damage", out m_SprayDamage);
			base.Model.SkillParameters.TryGetValue("spray_count", out m_SprayCount);
			ModificationsOnDamageCalculation.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				if (m_BonusDamage > 0f && target.CurrrentEffects.ContainsKey("GooBomb"))
				{
					damage *= m_BonusDamage / 100f;
				}
				VisualEffectSetting setting = null;
				string ident = "GooBomb_Additional";
				if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(ident, out setting))
				{
					SpawnVisualEffects(VisualEffectSpawnTiming.Affected, setting, new List<ICombatant> { target });
				}
				return damage;
			});
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			yield return source.CombatantView.StartCoroutine(base.DoAction(battle, source, target, shared, false));
			yield return new WaitForSeconds(0.5f);
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			ICombatant source2 = source;
			ICombatant target2 = target;
			List<ICombatant> possibleAdditionalTargets = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != source2.CombatantFaction && target2 != c).ToList();
			if (possibleAdditionalTargets.Count <= 0)
			{
				yield break;
			}
			int i = 0;
			while (possibleAdditionalTargets.Count > 0 && (float)i < m_SprayCount)
			{
				ICombatant additionalTarget = possibleAdditionalTargets[UnityEngine.Random.Range(0, possibleAdditionalTargets.Count)];
				possibleAdditionalTargets.Remove(additionalTarget);
				List<ICombatant> targets = new List<ICombatant> { additionalTarget };
				float modificationreceived2 = 1f;
				float modificationdealt2 = 1f;
				float attackValue2 = source.ModifiedAttack * (m_SprayDamage / 100f);
				float cachedValue = m_Source.CurrentSkillAttackValue;
				m_Source.CurrentSkillAttackValue = attackValue2;
				modificationdealt2 = DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(modificationdealt2, EffectTriggerType.OnDealDamage, m_Source, additionalTarget);
				if (additionalTarget.CurrrentEffects.ContainsKey("GooBomb"))
				{
					VisualEffectSetting setting = null;
					string gooDamageEffect = "GooBomb_Additional";
					if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(gooDamageEffect, out setting))
					{
						SpawnVisualEffects(VisualEffectSpawnTiming.Impact, setting, new List<ICombatant> { additionalTarget });
					}
					modificationdealt2 *= m_BonusDamage / 100f;
				}
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(1f, EffectTriggerType.BeforeReceiveDamage, additionalTarget, source);
				attackValue2 *= modificationdealt2;
				modificationreceived2 = DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(modificationreceived2, EffectTriggerType.OnReceiveDamage, additionalTarget, m_Source);
				additionalTarget.ReceiveDamage(attackValue2 * modificationreceived2, source);
				DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(additionalTarget, battle, source);
				m_Source.CurrentSkillAttackValue = cachedValue;
				i++;
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_3}", string.Empty + m_BonusDamage);
			dictionary.Add("{value_4}", string.Empty + m_SprayCount);
			dictionary.Add("{value_7}", string.Empty + DIContainerInfrastructure.GetFormatProvider().GetBattleStatsFormat(invoker.ModifiedAttack * (m_SprayDamage / 100f)));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
