using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackWithBonusDamage : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_ChargeTurns;

		private float m_DamageBonusVsWet = 100f;

		private float m_DamageBonusVsGoo = 100f;

		private float m_DamageBonusVsStun = 100f;

		private float m_DamageBonusVsPaint = 100f;

		private float m_DamageBonusVsChocolate = 100f;

		private float m_DamageBonusVsPumpkin = 100f;

		private float m_DamageBonusVsSpotlight = 100f;

		private float m_DamageBonusVsInk = 100f;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_wet", out m_DamageBonusVsWet);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_goo", out m_DamageBonusVsGoo);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_paint", out m_DamageBonusVsPaint);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_chocolate", out m_DamageBonusVsChocolate);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_pumpkin", out m_DamageBonusVsPumpkin);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_stun", out m_DamageBonusVsStun);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_spotlight", out m_DamageBonusVsSpotlight);
			base.Model.SkillParameters.TryGetValue("bonus_damage_vs_ink", out m_DamageBonusVsInk);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			ModificationsOnDamageCalculation.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				if (m_DamageBonusVsStun > 0f && target.IsStunned)
				{
					damage *= m_DamageBonusVsStun / 100f;
				}
				if (m_DamageBonusVsWet > 0f && target.CurrrentEffects.ContainsKey("WaterBomb"))
				{
					damage *= m_DamageBonusVsWet / 100f;
				}
				if (m_DamageBonusVsChocolate > 0f && target.CurrrentEffects.ContainsKey("ChocolateRain"))
				{
					damage *= m_DamageBonusVsChocolate / 100f;
				}
				if (m_DamageBonusVsPumpkin > 0f && target.CurrrentEffects.ContainsKey("StickyPumpkin"))
				{
					damage *= m_DamageBonusVsPumpkin / 100f;
				}
				if (m_DamageBonusVsPaint > 0f && target.CurrrentEffects.ContainsKey("ColorfulAttack"))
				{
					damage *= m_DamageBonusVsPaint / 100f;
				}
				if (m_DamageBonusVsSpotlight > 0f && target.CurrrentEffects.ContainsKey("InTheSpotlight"))
				{
					damage *= m_DamageBonusVsSpotlight / 100f;
				}
				if (m_DamageBonusVsInk > 0f && target.CurrrentEffects.ContainsKey("BreathOfTheSea"))
				{
					damage *= m_DamageBonusVsInk / 100f;
				}
				if (m_DamageBonusVsGoo > 0f && target.CurrrentEffects.ContainsKey("GooBomb"))
				{
					damage *= m_DamageBonusVsGoo / 100f;
					VisualEffectSetting setting = null;
					string ident = "GooBomb_Additional";
					if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(ident, out setting))
					{
						SpawnVisualEffects(VisualEffectSpawnTiming.Affected, setting, new List<ICombatant> { target });
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
			if (m_DamageBonusVsWet > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsWet);
			}
			if (m_DamageBonusVsGoo > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsGoo);
			}
			if (m_DamageBonusVsStun > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsStun);
			}
			if (m_DamageBonusVsChocolate > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsChocolate);
			}
			if (m_DamageBonusVsPumpkin > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsPumpkin);
			}
			if (m_DamageBonusVsPaint > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsPaint);
			}
			if (m_DamageBonusVsSpotlight > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsSpotlight);
			}
			if (m_DamageBonusVsInk > 0f)
			{
				dictionary.Add("{value_3}", string.Empty + m_DamageBonusVsInk);
			}
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
