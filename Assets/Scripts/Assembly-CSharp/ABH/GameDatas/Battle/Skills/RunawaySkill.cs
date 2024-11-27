using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using SmoothMoves;

namespace ABH.GameDatas.Battle.Skills
{
	public class RunawaySkill : SkillBattleDataBase
	{
		private float m_ChargeRounds;

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeRounds);
			m_ForceSourceIcon = true;
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger attack skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			if (EvaluateCharge(battle, source, m_Targets, target))
			{
				m_Source.CombatantView.targetSheltered = null;
			}
			else
			{
				DIContainerLogic.GetBattleService().RunAway(source, battle);
			}
			yield break;
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_2}", string.Empty + m_ChargeRounds);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
