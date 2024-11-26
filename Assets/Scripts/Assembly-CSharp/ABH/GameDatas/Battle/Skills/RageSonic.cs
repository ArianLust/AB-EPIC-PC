using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class RageSonic : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_ChargeTurns;

		private float m_AttackCount;

		private float m_ReduceCharge;

		private bool m_PlaySurprise;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			base.Model.SkillParameters.TryGetValue("reduce_charge", out m_ReduceCharge);
			m_AttackAnimation = (ICombatant c) => c.CombatantView.PlayRageSkillAnimation();
			ActionsAfterDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				ShareableActionPart(battle, source, target, false);
			});
			m_ApplyPerks = false;
			m_IsMelee = true;
		}

		public override void ShareableActionPart(BattleGameData battle, ICombatant source, ICombatant target, bool isShared)
		{
			if (target != null && target.IsAlive && target.IsCharging)
			{
				target.ReduceChargeBy((int)m_ReduceCharge);
			}
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			battle.SetRageAvailable(source.CombatantFaction, false);
			DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(base.DoAction(battle, source, target, shared, false));
			yield return DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(PlaySonicMovement(source, battle));
			if (source.CombatantFaction == Faction.Pigs)
			{
				battle.SetFactionRage(Faction.Pigs, 0f);
				battle.RegisterRageUsed(100f, source);
			}
			battle.SetRageAvailable(source.CombatantFaction, true);
		}

		private IEnumerator PlaySonicMovement(ICombatant sonic, BattleGameData battle)
		{
			yield return new WaitForSeconds(2.75f);
			CHMotionTween tween = sonic.CombatantView.m_TweenMovement;
			float durationPerStep = 0.3f;
			tween.m_StartTransform = sonic.CombatantView.transform;
			tween.m_StartOffset = Vector3.zero;
			tween.m_EndTransform = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != sonic.CombatantFaction).LastOrDefault().CombatantView.transform;
			tween.m_EndOffset = new Vector3(120f, 0f, 0f);
			tween.m_DurationInSeconds = durationPerStep;
			tween.Play();
			yield return new WaitForSeconds(durationPerStep);
			tween.m_StartTransform = sonic.CombatantView.transform;
			tween.m_StartOffset = Vector3.zero;
			tween.m_EndTransform = sonic.CombatantView.m_BattleMgr.m_BirdCenterPosition;
			tween.m_EndOffset = new Vector3(350f, 0f, 0f);
			tween.m_DurationInSeconds = durationPerStep;
			tween.Play();
			yield return new WaitForSeconds(durationPerStep);
			if (battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != sonic.CombatantFaction).Count() != 0)
			{
				tween.m_StartTransform = sonic.CombatantView.transform;
				tween.m_StartOffset = Vector3.zero;
				tween.m_EndTransform = battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != sonic.CombatantFaction).FirstOrDefault().CombatantView.transform;
				tween.m_EndOffset = new Vector3(120f, 0f, 0f);
				tween.m_DurationInSeconds = durationPerStep;
				tween.Play();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_5}", string.Empty + m_ChargeTurns);
			dictionary.Add("{value_4}", string.Empty + m_AttackCount);
			dictionary.Add("{value_3}", string.Empty + m_ReduceCharge);
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
