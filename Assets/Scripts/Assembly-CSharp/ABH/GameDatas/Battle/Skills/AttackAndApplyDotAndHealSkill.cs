using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackAndApplyDotAndHealSkill : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_ChargeTurns;

		private float m_AttackCount;

		private float m_DotChance;

		private float m_DotModifier;

		private float m_HealModifier;

		private float m_UseTotalOfDamage;

		private float m_ShrinkAmount;

		private float m_damageDealt;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			base.Model.SkillParameters.TryGetValue("dot_chance", out m_DotChance);
			base.Model.SkillParameters.TryGetValue("dot_damage_in_percent", out m_DotModifier);
			base.Model.SkillParameters.TryGetValue("dot_heal_in_percent", out m_HealModifier);
			base.Model.SkillParameters.TryGetValue("use_total_of_damage", out m_UseTotalOfDamage);
			base.Model.SkillParameters.TryGetValue("shrink_percent", out m_ShrinkAmount);
			ActionsOnDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				m_damageDealt = damage;
				ShareableActionPart(battle, source, target, false);
			});
		}

		public override void ShareableActionPart(BattleGameData battle, ICombatant source, ICombatant target, bool isShared)
		{
			if (UnityEngine.Random.value <= m_DotChance / 100f && target.CurrentHealth > 0f)
			{
				List<float> list = new List<float>();
				list.Add(m_DotModifier);
				list.Add(m_HealModifier);
				list.Add(m_UseTotalOfDamage);
				list.Add(m_damageDealt);
				List<float> values = list;
				BattleEffectGameData battleEffectGameData = new BattleEffectGameData(source, target, new List<BattleEffect>
				{
					new BattleEffect
					{
						EffectTrigger = EffectTriggerType.OnDealDamagePerTurn,
						EffectType = BattleEffectType.HealOnDOT,
						Values = values,
						AfflicionType = base.Model.Balancing.EffectType,
						Duration = base.Model.Balancing.EffectDuration,
						EffectAssetId = base.Model.Balancing.EffectIconAssetId,
						EffectAtlasId = base.Model.Balancing.EffectIconAtlasId
					}
				}, base.Model.Balancing.EffectDuration, battle, base.Model.Balancing.AssetId, SkillEffectTypes.Curse, GetLocalizedName(), base.Model.SkillNameId);
				battleEffectGameData.EffectRemovedAction = onremove;
				if (battleEffectGameData.AddEffect())
				{
					target.CombatantView.GrowCharacter(0f - m_ShrinkAmount / 100f);
				}
			}
		}

		private void onremove(BattleEffectGameData e)
		{
			e.m_Target.CombatantView.GrowCharacter(m_ShrinkAmount / 100f);
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_5}", string.Empty + m_ChargeTurns);
			dictionary.Add("{value_7}", string.Empty + Mathf.RoundToInt(m_DotModifier / 100f * invoker.ModifiedAttack).ToString("0"));
			dictionary.Add("{value_8}", string.Empty + Mathf.RoundToInt(m_HealModifier / 100f * invoker.ModifiedAttack).ToString("0"));
			dictionary.Add("{value_4}", string.Empty + m_AttackCount);
			dictionary.Add("{value_2}", string.Empty + base.Model.Balancing.EffectDuration);
			dictionary.Add("{value_3}", string.Empty + m_DotChance);
			dictionary.Add("{value_9}", string.Empty + m_DotModifier);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, dictionary);
		}
	}
}
