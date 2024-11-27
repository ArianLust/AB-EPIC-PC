using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackWithBonusDamageVsDebuff : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_ChargeTurns;

		private float m_DamageVsDebuff = 100f;

		private float m_AttackCount = 1f;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("damage_against_debuff", out m_DamageVsDebuff);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			ModificationsOnDamageCalculation.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				if (m_DamageVsDebuff > 0f)
				{
					bool flag = false;
					foreach (BattleEffectGameData value2 in target.CurrrentEffects.Values)
					{
						if (value2.m_EffectType == SkillEffectTypes.Curse)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						damage *= m_DamageVsDebuff / 100f;
					}
				}
				return damage;
			});
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			dictionary.Add("{value_4}", string.Empty + m_AttackCount);
			if (m_DamageVsDebuff > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageVsDebuff);
			}
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
