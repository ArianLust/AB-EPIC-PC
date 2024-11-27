using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class ReduceRageAttackSkill : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_RageReduce;

		private float m_TotalDamage;

		private float m_AttackCount;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			m_IgnoreRage = true;
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			m_DamageMod = value / 100f;
			ModificationsOnDamageCalculation.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				m_TotalDamage = m_Source.CurrentSkillAttackValue;
				return damage;
			});
			if (!base.Model.SkillParameters.TryGetValue("ragereduce", out m_RageReduce))
			{
				m_RageReduce = 15f;
			}
			ActionsOnDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				float value2 = 2f;
				if (!base.Model.SkillParameters.TryGetValue("fallofFactor", out value2))
				{
					value2 = 2f;
				}
				float num = 0f;
				num = m_RageReduce;
				DIContainerLogic.GetBattleService().ReduceRageFromAttack(num, battle, target);
				if (target.IsAlive)
				{
					target.CombatantView.PlayCheerCharacter();
				}
			});
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num.ToString("0"));
			dictionary.Add("{value_3}", string.Empty + Mathf.RoundToInt(m_RageReduce).ToString("0"));
			dictionary.Add("{value_4}", string.Empty + m_AttackCount.ToString("0"));
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
