using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class SimpleAttackWithMissSkill : AttackSkillTemplate
	{
		private bool m_Tumbled;

		private float m_DamageMod;

		private float m_TumbleChance;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("tumble", out m_TumbleChance);
			ActionsBeforeTargetSelection.Add(delegate
			{
				if (m_TumbleChance > 0f)
				{
					m_Tumbled = UnityEngine.Random.value <= m_TumbleChance / 100f;
				}
				m_Break = false;
				return 0f;
			});
			ActionsOnStartSkill.Add(delegate(BattleGameData battle, ICombatant source, List<ICombatant> targetsList, ICombatant initialTarget)
			{
				if (m_Tumbled)
				{
					m_Break = true;
					return source.CombatantView.PlayTumbledAnimation();
				}
				return 0f;
			});
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_3}", string.Empty + m_TumbleChance);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
