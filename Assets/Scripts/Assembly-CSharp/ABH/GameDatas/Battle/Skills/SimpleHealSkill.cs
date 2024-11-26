using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;

namespace ABH.GameDatas.Battle.Skills
{
	public class SimpleHealSkill : SkillBattleDataBase
	{
		private float m_Percent;

		private float m_Fixed;

		private bool m_All;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("damage_percent", out m_Percent);
			base.Model.SkillParameters.TryGetValue("damage_fix", out m_Fixed);
			m_All = base.Model.SkillParameters.ContainsKey("all");
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			if (!m_All)
			{
				m_Targets = new List<ICombatant> { target };
			}
			else
			{
				m_Targets = new List<ICombatant>();
				ICombatant target2 = target;
				m_Targets.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == target2.CombatantFaction).ToList());
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting, new List<ICombatant> { target });
			foreach (ICombatant skillTarget in m_Targets)
			{
				if (m_Fixed != 0f)
				{
					skillTarget.CombatantView.PlayCheerCharacter();
					skillTarget.HealDamage(m_Fixed, skillTarget);
					DIContainerLogic.GetBattleService().HealCurrentTurn(skillTarget, battle, true, true);
				}
			}
			yield break;
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_Fixed);
			dictionary.Add("{value_2}", string.Empty + m_Percent);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
