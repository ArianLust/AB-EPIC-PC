using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackWithIncreaseDamageReceived : AttackSkillTemplate
	{
		private int m_BuffDuration;

		private float m_DamageMod;

		private float m_Percent;

		private float m_AttackCount;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			m_BuffDuration = base.Model.Balancing.EffectDuration;
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			if (!base.Model.SkillParameters.TryGetValue("increase_in_percent", out m_Percent))
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
			if (isShared || !base.Model.SkillParameters.ContainsKey("effect_only_target") || source.AttackTarget == target)
			{
				float value = 0f;
				if (!base.Model.SkillParameters.TryGetValue("chance", out value))
				{
					value = 0f;
				}
				if (!(value / 100f < UnityEngine.Random.value))
				{
					List<float> list = new List<float>();
					list.Add(m_Percent);
					List<float> values = list;
					BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
					{
						new BattleEffect
						{
							EffectTrigger = EffectTriggerType.OnReceiveDamage,
							EffectType = BattleEffectType.IncreaseDamageReceived,
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
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_2}", string.Empty + m_BuffDuration);
			dictionary.Add("{value_3}", string.Empty + m_Percent);
			dictionary.Add("{value_4}", string.Empty + m_AttackCount);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
