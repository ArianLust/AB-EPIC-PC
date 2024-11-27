using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackWithLeech : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_Percent;

		private float m_ChargeTurns;

		private float m_AttackCount;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("heal", out m_Percent);
			if (!base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount))
			{
				m_AttackCount = 1f;
			}
			ActionsOnDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				source.HealDamage(damage * m_Percent / 100f, source);
				DIContainerLogic.GetBattleService().HealCurrentTurn(source, battle);
			});
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
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
