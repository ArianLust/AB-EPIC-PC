using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class SimpleAttackWithDamageBonusFromRage : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_bonusValue;

		private float m_ChargeTurns;

		private float m_UseTargetRage;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out m_DamageMod);
			m_DamageMod /= 100f;
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			base.Model.SkillParameters.TryGetValue("bonus_damage", out m_bonusValue);
			base.Model.SkillParameters.TryGetValue("opponent_rage", out m_UseTargetRage);
			ModificationsOnDamageDealtCalculation.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				float num = ((m_UseTargetRage != 0f) ? battle.GetFactionRage(target.CombatantFaction) : battle.GetFactionRage(source.CombatantFaction));
				bool flag = base.Model.SkillParameters.ContainsKey("rage_missing");
				float num2 = m_bonusValue / 100f;
				float num3 = 0f;
				if (flag)
				{
					num3 = (100f - num) * num2 / 100f;
					return damage + num3;
				}
				num3 = num * num2 / 100f;
				return damage + num3;
			});
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			float num2 = m_bonusValue / 100f;
			float num3 = Mathf.Min(num2, 1f);
			dictionary.Add("{value_3}", string.Empty + Mathf.CeilToInt(num2).ToString("0"));
			dictionary.Add("{value_6}", string.Empty + Mathf.RoundToInt(1f / num3));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
