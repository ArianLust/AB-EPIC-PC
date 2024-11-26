using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class HealAndAddRage : SkillBattleDataBase
	{
		protected float m_Percent;

		protected float m_Fixed;

		protected bool m_All;

		protected bool m_Self;

		protected bool m_InvokersHealth;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("health_in_percent", out m_Percent);
			base.Model.SkillParameters.TryGetValue("increase_rage", out m_Fixed);
			m_All = base.Model.SkillParameters.ContainsKey("all");
			m_InvokersHealth = base.Model.SkillParameters.ContainsKey("invoker");
			m_Self = base.Model.SkillParameters.ContainsKey("self");
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
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlayRageSkillAnimation());
			foreach (ICombatant skillTarget in m_Targets)
			{
				float referencedHealth = ((!m_InvokersHealth) ? skillTarget.ModifiedHealth : m_Source.ModifiedHealth);
				skillTarget.HealDamage(referencedHealth * m_Percent / 100f, skillTarget);
				SpawnVisualEffects(VisualEffectSpawnTiming.Affected, m_VisualEffectSetting);
				DIContainerLogic.GetBattleService().HealCurrentTurn(skillTarget, battle, true, false, false, false, true, source);
			}
			DIContainerLogic.GetBattleService().AddFixRage(battle, source, m_Fixed);
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (m_Self)
			{
				dictionary.Add("{value_1}", (invoker.ModifiedHealth * m_Percent / 100f).ToString("0"));
			}
			else
			{
				dictionary.Add("{value_1}", m_Percent.ToString("0"));
			}
			dictionary.Add("{value_7}", m_Fixed.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
