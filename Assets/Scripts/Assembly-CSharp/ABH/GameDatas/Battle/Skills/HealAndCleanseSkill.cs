using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class HealAndCleanseSkill : SkillBattleDataBase
	{
		protected float m_PercentBanner;

		protected float m_Percent;

		protected float m_Fixed;

		protected float m_CleanseChance;

		protected float m_AdditionalPercentOnTarget;

		protected bool m_All;

		protected bool m_Self;

		protected bool m_CleanseOnlyTarget;

		protected bool m_InvokersHealth;

		protected bool m_FirsTargetHealth;

		protected float m_ChargeRounds;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("health_in_percent", out m_Percent);
			base.Model.SkillParameters.TryGetValue("cleanse_chance", out m_CleanseChance);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeRounds);
			m_All = base.Model.SkillParameters.ContainsKey("all");
			m_InvokersHealth = base.Model.SkillParameters.ContainsKey("invoker");
			m_FirsTargetHealth = base.Model.SkillParameters.ContainsKey("first_target");
			m_Self = base.Model.SkillParameters.ContainsKey("self");
			m_CleanseOnlyTarget = base.Model.SkillParameters.ContainsKey("cleanse_target");
			model.SkillParameters.TryGetValue("additional_percent_on_target", out m_AdditionalPercentOnTarget);
			if (!base.Model.SkillParameters.TryGetValue("health_in_percent_banner", out m_PercentBanner))
			{
				m_PercentBanner = m_Percent;
			}
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			m_InitialTarget = target;
			if (!m_All)
			{
				if (m_Self)
				{
					m_Targets = new List<ICombatant> { source };
					m_InitialTarget = source;
				}
				else
				{
					m_Targets = new List<ICombatant> { target };
				}
			}
			else
			{
				m_Targets = new List<ICombatant>();
				ICombatant target2 = target;
				ICombatant source2 = source;
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target2.CombatantFaction && c != source2).ToList());
			}
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				m_Source.CombatantView.targetSheltered = null;
				yield break;
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			foreach (ICombatant skillTarget in m_Targets)
			{
				Heal(skillTarget, battle, source, target);
			}
			if (m_All)
			{
				Heal(source, battle, source, target);
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Affected, m_VisualEffectSetting);
		}

		private void Heal(ICombatant skillTarget, BattleGameData battle, ICombatant source, ICombatant target)
		{
			if (m_Percent != 0f)
			{
				float num = m_Percent;
				if (skillTarget == target)
				{
					num += m_AdditionalPercentOnTarget;
				}
				float num2 = (m_InvokersHealth ? m_Source.ModifiedHealth : ((!m_FirsTargetHealth) ? skillTarget.ModifiedHealth : target.ModifiedHealth));
				skillTarget.HealDamage(num2 * num / 100f, skillTarget);
			}
			if (skillTarget != source)
			{
				skillTarget.CombatantView.PlayCheerCharacter();
			}
			if ((!m_CleanseOnlyTarget || skillTarget == target) && Random.value <= m_CleanseChance / 100f)
			{
				int num3 = DIContainerLogic.GetBattleService().RemoveBattleEffects(skillTarget, SkillEffectTypes.Curse);
				if (num3 > 0)
				{
					VisualEffectSetting setting = null;
					if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting("Cleanse", out setting))
					{
						SpawnVisualEffects(VisualEffectSpawnTiming.Affected, setting, new List<ICombatant> { skillTarget });
					}
				}
			}
			DIContainerLogic.GetBattleService().HealCurrentTurn(skillTarget, battle, true, false, false, false, true, source);
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (m_Self || m_InvokersHealth)
			{
				dictionary.Add("{value_1}", (invoker.ModifiedHealth * m_Percent / 100f).ToString("0"));
				dictionary.Add("{value_7}", (invoker.ModifiedHealth * (m_Percent + m_AdditionalPercentOnTarget) / 100f).ToString("0"));
				dictionary.Add("{value_9}", (invoker.ModifiedHealth * m_PercentBanner / 100f).ToString("0"));
			}
			else
			{
				dictionary.Add("{value_1}", m_Percent.ToString("0"));
				dictionary.Add("{value_7}", (m_Percent + m_AdditionalPercentOnTarget).ToString("0"));
				dictionary.Add("{value_9}", m_PercentBanner.ToString("0"));
			}
			dictionary.Add("{value_3}", m_CleanseChance.ToString("0"));
			dictionary.Add("{value_2}", m_ChargeRounds.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
