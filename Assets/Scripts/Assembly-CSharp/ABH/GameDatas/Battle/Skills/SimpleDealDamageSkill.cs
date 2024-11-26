using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class SimpleDealDamageSkill : SkillBattleDataBase
	{
		private float m_Fixed;

		private float m_Percent;

		private bool m_All;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("increase_rage", out m_Fixed);
			base.Model.SkillParameters.TryGetValue("damage_percent_of_target", out m_Percent);
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
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting, m_Targets);
			source.CombatantView.PlaySupportAnimation();
			yield return new WaitForSeconds(2.25f);
			SpawnVisualEffects(VisualEffectSpawnTiming.Impact, m_VisualEffectSetting, m_Targets);
			foreach (ICombatant skillTarget in m_Targets)
			{
				if (m_Percent == 0f)
				{
					continue;
				}
				BattleEffectGameData demonicEffectGameData = skillTarget.CurrrentEffects.Values.FirstOrDefault((BattleEffectGameData e) => e.m_Effects.Any((BattleEffect f) => f.EffectType == BattleEffectType.DoNotGetKilledByBird));
				if (demonicEffectGameData != null)
				{
					demonicEffectGameData.RemoveEffect();
				}
				skillTarget.ReceiveDamage(skillTarget.ModifiedHealth * (m_Percent / 100f), source);
				DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(skillTarget, battle, source, false, true, true, true);
			}
			yield return new WaitForSeconds(2f);
			m_Source.CombatantView.m_BattleMgr.m_ForcedCheckProgress = true;
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + m_Percent);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
