using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using SmoothMoves;

namespace ABH.GameDatas.Battle.Skills
{
	public class IncreaseDamageAndHealthPerTurn : SkillBattleDataBase
	{
		private float m_IncreaseInPercent;

		private float m_Turns;

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("increase_in_percent", out m_IncreaseInPercent);
			model.SkillParameters.TryGetValue("max_turns", out m_Turns);
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
					EffectTrigger = EffectTriggerType.BeforeStartOfTurn,
					EffectType = BattleEffectType.IncreaseDamagePermanent,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = new List<float> { m_IncreaseInPercent },
					Duration = (int)m_Turns,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				}
			}, (int)m_Turns, battle, base.Model.Balancing.AssetId + "_attack", base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData.AddEffect();
			BattleEffectGameData battleEffectGameData2 = new BattleEffectGameData(source, target, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.BeforeStartOfTurn,
					EffectType = BattleEffectType.IncreaseHealthPermanent,
					AfflicionType = base.Model.Balancing.EffectType,
					Values = new List<float> { m_IncreaseInPercent },
					Duration = (int)m_Turns,
					EffectAssetId = base.Model.Balancing.EffectIconAssetId,
					EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
				}
			}, (int)m_Turns, battle, base.Model.Balancing.AssetId + "_health", base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
			battleEffectGameData2.AddEffect();
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> replacementStrings = new Dictionary<string, string>();
			WorldBalancingData worldBalancingData = DIContainerBalancing.Service.GetBalancingDataList<WorldBalancingData>().FirstOrDefault();
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, replacementStrings);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
