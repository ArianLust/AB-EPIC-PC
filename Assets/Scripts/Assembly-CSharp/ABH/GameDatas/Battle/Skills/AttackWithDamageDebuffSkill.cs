using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackWithDamageDebuffSkill : AttackSkillTemplate
	{
		private int m_BuffDuration;

		private float m_ChargeTurns;

		private float m_DamageMod;

		private float m_Percent;

		private float m_AttackCount;

		private float m_DebuffChance;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			m_BuffDuration = base.Model.Balancing.EffectDuration;
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("chance", out m_DebuffChance);
			if (!base.Model.SkillParameters.TryGetValue("reduction_in_percent", out m_Percent))
			{
				m_Percent = 0f;
			}
			ActionsAfterDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				ShareableActionPart(battle, source, target, false);
			});
		}

		public override void ShareableActionPart(BattleGameData battle, ICombatant source, ICombatant target, bool isShared)
		{
			if (!(m_DebuffChance / 100f < UnityEngine.Random.value))
			{
				List<float> list = new List<float>();
				list.Add(m_Percent);
				List<float> values = list;
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnDealDamage,
						EffectType = BattleEffectType.ReduceDamageDealt,
						AfflicionType = base.Model.Balancing.EffectType,
						Values = values,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, base.Model.Balancing.EffectType, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.AddEffect();
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_5}", string.Empty + m_ChargeTurns);
			dictionary.Add("{value_3}", string.Empty + m_Percent);
			dictionary.Add("{value_4}", string.Empty + m_AttackCount);
			dictionary.Add("{value_2}", string.Empty + m_BuffDuration);
			dictionary.Add("{value_6}", string.Empty + m_Percent);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
