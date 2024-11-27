using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class SacrificeUndeadForHealthSkill : SkillBattleDataBase
	{
		protected float m_Percent;

		protected bool m_All;

		protected bool m_Self;

		protected float m_Charge;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("health_in_percent", out m_Percent);
			base.Model.SkillParameters.TryGetValue("charge", out m_Charge);
			m_All = base.Model.SkillParameters.ContainsKey("all");
			m_Self = base.Model.SkillParameters.ContainsKey("self");
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger support skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			m_InitialTarget = target;
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				yield break;
			}
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
			ICombatant source2 = source;
			List<ICombatant> toSacrifice = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.KnockOutOnDefeat && c.CombatantFaction == source2.CombatantFaction).ToList();
			toSacrifice.AddRange(battle.m_CombatantsPerFaction[source.CombatantFaction].Where((ICombatant c) => c.IsKnockedOut));
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			yield return new WaitForSeconds(source.CombatantView.PlaySupportAnimation());
			float healFactor = 0f;
			bool oneKnockedOut = false;
			float animationLength = 0f;
			foreach (ICombatant undead in toSacrifice)
			{
				if (undead.IsKnockedOut)
				{
					healFactor += 0.5f;
					undead.RaiseCombatantDefeated();
					undead.IsKnockedOut = false;
					undead.IsParticipating = false;
					continue;
				}
				healFactor += 1f;
				animationLength = undead.CombatantView.m_AssetController.GetKnockOutAnimationLength();
				undead.ReceiveDamage(undead.CurrentHealth, source);
				DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(undead, battle, source);
				undead.RaiseCombatantDefeated();
				undead.IsKnockedOut = false;
				undead.IsParticipating = false;
				oneKnockedOut = true;
			}
			if (oneKnockedOut)
			{
				yield return new WaitForSeconds(animationLength);
			}
			source.HealDamage(source.ModifiedHealth * m_Percent / 100f * healFactor, source);
			DIContainerLogic.GetBattleService().HealCurrentTurn(source, battle, true, false, false, false, true, source);
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", (invoker.ModifiedHealth * m_Percent / 100f).ToString("0"));
			dictionary.Add("{value_5}", m_Charge.ToString("0"));
			dictionary.Add("{value_3}", (invoker.ModifiedHealth * (m_Percent / 2f) / 100f).ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
