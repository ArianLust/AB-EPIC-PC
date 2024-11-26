using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using SmoothMoves;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class DoNothingSkill : SkillBattleDataBase
	{
		protected float m_Charge;

		protected float m_BonusChance;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("charge", out m_Charge);
			base.Model.SkillParameters.TryGetValue("bonus_chance", out m_BonusChance);
		}

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DebugLog.Log("Trigger attack skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			if (!EvaluateCharge(battle, source, m_Targets, target))
			{
				if (base.Model.SkillParameters.ContainsKey("laugh"))
				{
					yield return new WaitForSeconds(source.CombatantView.PlayLaughAnimation());
				}
				else if (base.Model.SkillParameters.ContainsKey("mourn"))
				{
					yield return new WaitForSeconds(source.CombatantView.PlayMournAnimation());
				}
				else if (base.Model.SkillParameters.ContainsKey("tumble"))
				{
					yield return new WaitForSeconds(source.CombatantView.PlayTumbledAnimation());
				}
				else if (base.Model.SkillParameters.ContainsKey("attention"))
				{
					yield return new WaitForSeconds(source.CombatantView.PlayAttentionAnimation());
				}
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_2}", string.Empty + m_Charge);
			dictionary.Add("{value_3}", string.Empty + m_BonusChance);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
