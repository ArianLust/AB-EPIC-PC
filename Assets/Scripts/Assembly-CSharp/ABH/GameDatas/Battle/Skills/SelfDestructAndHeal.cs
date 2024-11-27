using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;

namespace ABH.GameDatas.Battle.Skills
{
	public class SelfDestructAndHeal : SkillBattleDataBase
	{
		private float m_HealthInPercent;

		private float m_ChargeTurns;

		private bool m_All;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("health_in_percent", out m_HealthInPercent);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			m_All = base.Model.SkillParameters.ContainsKey("all");
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				yield break;
			}
			List<ICombatant> possibleTargets = new List<ICombatant>();
			if (m_All)
			{
				ICombatant source2 = source;
				possibleTargets = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == source2.CombatantFaction).ToList();
			}
			else
			{
				possibleTargets.Add(target);
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting, possibleTargets);
			for (int i = 0; i < possibleTargets.Count; i++)
			{
				possibleTargets[i].HealDamage(possibleTargets[i].ModifiedHealth * m_HealthInPercent / 100f, source);
				DIContainerLogic.GetBattleService().HealCurrentTurn(possibleTargets[i], battle, true, false, false, false, true, source);
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Affected, m_VisualEffectSetting, possibleTargets);
			source.ReceiveDamage(source.CurrentHealth, source);
			DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(source, battle, source);
			source.CombatantView.gameObject.SetActive(false);
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_HealthInPercent);
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
