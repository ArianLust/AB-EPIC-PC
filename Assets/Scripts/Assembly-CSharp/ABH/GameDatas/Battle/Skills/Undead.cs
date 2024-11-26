using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using SmoothMoves;

namespace ABH.GameDatas.Battle.Skills
{
	public class Undead : SkillBattleDataBase
	{
		private float m_Percent;

		private float m_Duration;

		private string m_effectString = string.Empty;

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("revive_health_percent", out m_Percent);
			model.SkillParameters.TryGetValue("revive_duration", out m_Duration);
			foreach (string key in base.Model.SkillParameters.Keys)
			{
				if (base.Model.SkillParameters[key] == -1f)
				{
					m_effectString = key;
				}
			}
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			yield break;
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
			DebugLog.Log("Trigger passive skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
			m_Source = source;
			SkillGameData skillGameData = new SkillGameData(m_effectString);
			SkillBattleDataBase knockedOutSkill = skillGameData.GenerateSkillBattleData();
			m_Source.KnockedOutSkill = knockedOutSkill;
			m_Source.KnockOutOnDefeat = true;
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			WorldBalancingData worldBalancingData = DIContainerBalancing.Service.GetBalancingDataList<WorldBalancingData>().FirstOrDefault();
			dictionary.Add("{value_2}", m_Duration.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
