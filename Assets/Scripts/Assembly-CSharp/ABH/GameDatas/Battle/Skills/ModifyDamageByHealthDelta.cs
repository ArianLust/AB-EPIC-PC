using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using SmoothMoves;

namespace ABH.GameDatas.Battle.Skills
{
	public class ModifyDamageByHealthDelta : SkillBattleDataBase
	{
		private float m_MinMaxPercentValueToApply;

		private float m_PercentPerHealthPercentDelta;

		private float m_LowerOrHigher;

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("min_max_value", out m_MinMaxPercentValueToApply);
			model.SkillParameters.TryGetValue("percent_per_delta", out m_PercentPerHealthPercentDelta);
			model.SkillParameters.TryGetValue("lower_or_higher", out m_LowerOrHigher);
			DebugLog.Log("Percentage Delta: " + m_PercentPerHealthPercentDelta);
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
			List<float> list = new List<float>();
			list.Add(m_LowerOrHigher);
			list.Add(m_MinMaxPercentValueToApply);
			list.Add(m_PercentPerHealthPercentDelta);
			List<float> values = list;
			BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnDealDamage,
					AfflicionType = base.Model.Balancing.EffectType,
					EffectType = BattleEffectType.ModifyDamageByHealth,
					Values = values,
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				}
			}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData.AddEffect();
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> replacementStrings = new Dictionary<string, string>();
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, replacementStrings);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
