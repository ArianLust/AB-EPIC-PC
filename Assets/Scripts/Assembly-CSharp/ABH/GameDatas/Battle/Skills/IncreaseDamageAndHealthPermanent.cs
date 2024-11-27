using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using SmoothMoves;

namespace ABH.GameDatas.Battle.Skills
{
	public class IncreaseDamageAndHealthPermanent : SkillBattleDataBase
	{
		private float m_HealthIncreaseInPercent;

		private float m_AttackIncreaseInPercent;

		private float m_Turns;

		private bool m_AlreadyAdded;

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("health_in_percent", out m_HealthIncreaseInPercent);
			model.SkillParameters.TryGetValue("attack_in_percent", out m_AttackIncreaseInPercent);
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
			BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.Instant,
					EffectType = BattleEffectType.IncreaseDamagePermanentOnce,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = new List<float> { m_AttackIncreaseInPercent },
					Duration = base.Model.Balancing.EffectDuration,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				}
			}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData.AddEffect();
			BattleEffectGameData battleEffectGameData2 = new BattleEffectGameData(source, target, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.Instant,
					EffectType = BattleEffectType.IncreaseHealthPermanentOnce,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = new List<float> { m_HealthIncreaseInPercent },
					Duration = base.Model.Balancing.EffectDuration
				}
			}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId + "_health", base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData2.AddEffect();
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", m_AttackIncreaseInPercent.ToString("0"));
			dictionary.Add("{value_5}", m_HealthIncreaseInPercent.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
