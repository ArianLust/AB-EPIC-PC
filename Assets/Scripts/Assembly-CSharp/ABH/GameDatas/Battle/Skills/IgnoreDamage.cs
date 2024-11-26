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
	public class IgnoreDamage : SkillBattleDataBase
	{
		private float m_ModifiedReferencedDamageInPercent;

		private float m_LowerOrHigher;

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			model.SkillParameters.TryGetValue("refdamage_percent", out m_ModifiedReferencedDamageInPercent);
			model.SkillParameters.TryGetValue("lower_or_higher", out m_LowerOrHigher);
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
			if (m_LowerOrHigher < 0f)
			{
				List<float> list = new List<float>();
				list.Add(m_ModifiedReferencedDamageInPercent);
				List<float> values = list;
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnReceiveDamage,
						EffectType = BattleEffectType.IgnoreDamageIfLower,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = values,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.AddEffect();
			}
			else if (m_LowerOrHigher > 0f)
			{
				List<float> list = new List<float>();
				list.Add(m_ModifiedReferencedDamageInPercent);
				List<float> values2 = list;
				BattleEffectGameData battleEffectGameData2 = new BattleEffectGameData(source, target, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnReceiveDamage,
						EffectType = BattleEffectType.IgnoreDamageIfHigher,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = values2,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData2.AddEffect();
			}
		}

		public float EvaluateIgnoreDamage(ICombatant invoker)
		{
			int num = 0;
			WorldBalancingData worldBalancingData = DIContainerBalancing.Service.GetBalancingDataList<WorldBalancingData>().FirstOrDefault();
			BattleGameData currentBattleGameData = ClientInfo.CurrentBattleGameData;
			if (m_LowerOrHigher < 0f)
			{
				if (currentBattleGameData != null && currentBattleGameData.m_ironCladValue != 0f)
				{
					return currentBattleGameData.m_ironCladValue * m_ModifiedReferencedDamageInPercent / 100f;
				}
				num = (int)(DIContainerLogic.GetBattleService().GetReferenceDamageIronclad(invoker.CharacterModel.Level, DIContainerInfrastructure.GetCurrentPlayer().Data.Level, worldBalancingData.ReferenceAttackValueBase, worldBalancingData.ReferenceAttackValuePerLevelInPercent) * m_ModifiedReferencedDamageInPercent / 100f);
			}
			else
			{
				if (currentBattleGameData != null && currentBattleGameData.m_dodgeValue != 0f)
				{
					return currentBattleGameData.m_dodgeValue * m_ModifiedReferencedDamageInPercent / 100f;
				}
				num = (int)(DIContainerLogic.GetBattleService().GetReferenceDamageDodge(invoker.CharacterModel.Level, DIContainerInfrastructure.GetCurrentPlayer().Data.Level, worldBalancingData.ReferenceAttackValueBase, worldBalancingData.ReferenceAttackValuePerLevelInPercent) * m_ModifiedReferencedDamageInPercent / 100f);
			}
			return num;
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			WorldBalancingData worldBalancingData = DIContainerBalancing.Service.GetBalancingDataList<WorldBalancingData>().FirstOrDefault();
			int num = 0;
			BattleGameData currentBattleGameData = ClientInfo.CurrentBattleGameData;
			dictionary.Add("{value_1}", ((m_LowerOrHigher < 0f) ? ((currentBattleGameData == null || currentBattleGameData.m_ironCladValue == 0f) ? ((int)(DIContainerLogic.GetBattleService().GetReferenceDamageIronclad(invoker.CharacterModel.Level, DIContainerInfrastructure.GetCurrentPlayer().Data.Level, worldBalancingData.ReferenceAttackValueBase, worldBalancingData.ReferenceAttackValuePerLevelInPercent) * m_ModifiedReferencedDamageInPercent / 100f)) : ((int)(currentBattleGameData.m_ironCladValue * m_ModifiedReferencedDamageInPercent / 100f))) : ((currentBattleGameData == null || currentBattleGameData.m_dodgeValue == 0f) ? ((int)(DIContainerLogic.GetBattleService().GetReferenceDamageDodge(invoker.CharacterModel.Level, DIContainerInfrastructure.GetCurrentPlayer().Data.Level, worldBalancingData.ReferenceAttackValueBase, worldBalancingData.ReferenceAttackValuePerLevelInPercent) * m_ModifiedReferencedDamageInPercent / 100f)) : ((int)(currentBattleGameData.m_dodgeValue * m_ModifiedReferencedDamageInPercent / 100f)))).ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
