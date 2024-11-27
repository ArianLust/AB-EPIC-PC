using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using SmoothMoves;

namespace ABH.GameDatas.Battle.Skills
{
	public class SuicideSkill : SkillBattleDataBase
	{
		protected float m_Charge;

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("charge", out m_Charge);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger attack skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				yield break;
			}
			SpawnVisualEffects(VisualEffectSpawnTiming.Start, m_VisualEffectSetting);
			ICombatant source2 = source;
			foreach (ICombatant combatant in battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != source2.CombatantFaction))
			{
				combatant.CombatantView.PlaySurprisedAnimation();
			}
			source.ReceiveDamage(source.CurrentHealth, source);
			DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(source, battle, source);
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_2}", m_Charge.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
