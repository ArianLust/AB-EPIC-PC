using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;

namespace ABH.GameDatas.Battle.Skills
{
	public class SimpleAttackWithDamageBonusFromHealth : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_bonusValue = 1f;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out m_DamageMod);
			m_DamageMod /= 100f;
			base.Model.SkillParameters.TryGetValue("bonus_damage", out m_bonusValue);
			ModificationsOnDamageDealtCalculation.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				float num = source.CurrentHealth / source.ModifiedHealth * 100f;
				bool flag = base.Model.SkillParameters.ContainsKey("health_missing");
				float num2 = m_bonusValue / 100f;
				float num3 = 0f;
				if (flag)
				{
					num3 = (100f - num) * num2 / 100f;
					return damage + damage * num3;
				}
				num3 = (100f - num) * num2 / 100f;
				return damage * (1f - num3);
			});
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_3}", string.Empty + (m_bonusValue / 100f).ToString("0"));
			dictionary.Add("{value_6}", string.Empty + 1);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
