using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class RageWhiteBird : HealAndCleanseSkill
	{
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
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlayRageSkillAnimation());
			foreach (ICombatant skillTarget in m_Targets)
			{
				Heal(skillTarget, battle, source);
			}
			Heal(source, battle, source);
			if (source.CombatantFaction == Faction.Pigs)
			{
				battle.SetFactionRage(Faction.Pigs, 0f);
				battle.RegisterRageUsed(100f, source);
			}
		}

		private void Heal(ICombatant skillTarget, BattleGameData battle, ICombatant source)
		{
			if (m_Percent == 0f)
			{
				return;
			}
			if (skillTarget.IsBanner)
			{
				skillTarget.HealDamage(skillTarget.ModifiedHealth * m_PercentBanner / 100f, skillTarget);
			}
			else
			{
				skillTarget.HealDamage(skillTarget.ModifiedHealth * m_Percent / 100f, skillTarget);
			}
			if (Random.value <= m_CleanseChance / 100f)
			{
				int num = DIContainerLogic.GetBattleService().RemoveBattleEffects(skillTarget, SkillEffectTypes.Curse);
				if (num > 0)
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
	}
}
