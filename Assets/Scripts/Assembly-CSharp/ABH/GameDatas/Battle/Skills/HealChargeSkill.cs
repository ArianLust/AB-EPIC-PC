using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class HealChargeSkill : SkillBattleDataBase
	{
		private float m_Percent;

		private float m_Fixed;

		private float m_ChargeRounds;

		private bool m_All;

		private bool m_Self;

		private bool m_Revive;

		protected bool m_InvokersHealth;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("health_in_percent", out m_Percent);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeRounds);
			m_All = base.Model.SkillParameters.ContainsKey("all");
			m_Self = base.Model.SkillParameters.ContainsKey("self");
			m_Revive = base.Model.SkillParameters.ContainsKey("revive");
			m_InvokersHealth = base.Model.SkillParameters.ContainsKey("invoker");
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
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target2.CombatantFaction).ToList());
			}
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				m_Source.CombatantView.targetSheltered = null;
				yield break;
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			if (!m_Source.IsKnockedOut)
			{
				yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			}
			foreach (ICombatant skillTarget in m_Targets)
			{
				if (m_Percent != 0f)
				{
					float referencedHealth = ((!m_InvokersHealth) ? skillTarget.ModifiedHealth : m_Source.ModifiedHealth);
					SpawnVisualEffects(VisualEffectSpawnTiming.Affected, m_VisualEffectSetting);
					skillTarget.HealDamage(referencedHealth * m_Percent / 100f, skillTarget);
					DIContainerLogic.GetBattleService().HealCurrentTurn(skillTarget, battle, true, true, true, true, true, source);
					if (skillTarget != source)
					{
						skillTarget.CombatantView.PlayCheerCharacter();
					}
					if (m_Revive && skillTarget.IsKnockedOut)
					{
						skillTarget.IsKnockedOut = false;
						skillTarget.ActedThisTurn = false;
						DIContainerLogic.GetBattleService().ReCalculateInitiative(battle);
						DIContainerLogic.GetBattleService().AddNewCombatantToBattle(battle, skillTarget);
						skillTarget.CombatantView.SpawnHealthBar();
						skillTarget.CombatantView.PlayReviveAnimation();
						DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(m_Percent, EffectTriggerType.OnRevive, skillTarget, skillTarget);
					}
				}
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (m_Self || m_InvokersHealth)
			{
				dictionary.Add("{value_1}", (invoker.ModifiedHealth * m_Percent / 100f).ToString("0"));
			}
			else
			{
				dictionary.Add("{value_1}", m_Percent.ToString("0"));
			}
			dictionary.Add("{value_2}", m_ChargeRounds.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
