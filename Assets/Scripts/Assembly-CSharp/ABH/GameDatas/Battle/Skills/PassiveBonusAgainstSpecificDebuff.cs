using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using SmoothMoves;

namespace ABH.GameDatas.Battle.Skills
{
	public class PassiveBonusAgainstSpecificDebuff : SkillBattleDataBase
	{
		private float m_DamageBonusVsWet = 100f;

		private float m_DamageBonusVsGoo = 100f;

		private float m_DamageBonusVsStun = 100f;

		private float m_DamageBonusVsPaint = 100f;

		private float m_DamageBonusVsPumpkin = 100f;

		private float m_DamageBonusVsChocolate = 100f;

		private float m_DamageBonusVsSpotlight = 100f;

		private float m_DamageBonusVsInk = 100f;

		public override void BoneAnimationUserTrigger(UserTriggerEvent triggerEvent)
		{
			base.BoneAnimationUserTrigger(triggerEvent);
		}

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_wet", out m_DamageBonusVsWet);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_goo", out m_DamageBonusVsGoo);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_stun", out m_DamageBonusVsStun);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_paint", out m_DamageBonusVsPaint);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_pumpkin", out m_DamageBonusVsPumpkin);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_chocolate", out m_DamageBonusVsChocolate);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_spotlight", out m_DamageBonusVsSpotlight);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_ink", out m_DamageBonusVsInk);
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
			list.Add(m_DamageBonusVsWet);
			list.Add(m_DamageBonusVsGoo);
			list.Add(m_DamageBonusVsStun);
			list.Add(m_DamageBonusVsPaint);
			list.Add(m_DamageBonusVsChocolate);
			list.Add(m_DamageBonusVsPumpkin);
			list.Add(m_DamageBonusVsSpotlight);
			list.Add(m_DamageBonusVsInk);
			List<float> values = list;
			BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
			{
				new BattleEffect
				{
					EffectTrigger = EffectTriggerType.OnDealDamage,
					EffectType = BattleEffectType.ModifyDamageVsDebuff,
					AfflicionType = base.Model.Balancing.EffectType,
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
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (m_DamageBonusVsWet > 0f)
			{
				dictionary.Add("{value_1}", string.Empty + m_DamageBonusVsWet);
			}
			if (m_DamageBonusVsGoo > 0f)
			{
				dictionary.Add("{value_1}", string.Empty + m_DamageBonusVsGoo);
			}
			if (m_DamageBonusVsStun > 0f)
			{
				dictionary.Add("{value_1}", string.Empty + m_DamageBonusVsStun);
			}
			if (m_DamageBonusVsPaint > 0f)
			{
				dictionary.Add("{value_1}", string.Empty + m_DamageBonusVsPaint);
			}
			if (m_DamageBonusVsPumpkin > 0f)
			{
				dictionary.Add("{value_1}", string.Empty + m_DamageBonusVsPumpkin);
			}
			if (m_DamageBonusVsChocolate > 0f)
			{
				dictionary.Add("{value_1}", string.Empty + m_DamageBonusVsChocolate);
			}
			if (m_DamageBonusVsSpotlight > 0f)
			{
				dictionary.Add("{value_1}", string.Empty + m_DamageBonusVsSpotlight);
			}
			if (m_DamageBonusVsInk > 0f)
			{
				dictionary.Add("{value_1}", string.Empty + m_DamageBonusVsInk);
			}
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
