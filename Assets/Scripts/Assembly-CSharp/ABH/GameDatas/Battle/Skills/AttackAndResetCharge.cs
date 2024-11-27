using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackAndResetCharge : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_ReduceCharge;

		private float m_Chance;

		private bool m_PlaySurprise;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("chance", out m_Chance);
			base.Model.SkillParameters.TryGetValue("reduce_charge", out m_ReduceCharge);
			ActionsAfterDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				ShareableActionPart(battle, source, target, false);
			});
		}

		public override void ShareableActionPart(BattleGameData battle, ICombatant source, ICombatant target, bool isShared)
		{
			if (target != null && target.IsAlive && target.IsCharging && UnityEngine.Random.value <= m_Chance / 100f)
			{
				target.ReduceChargeBy((int)m_ReduceCharge);
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_2}", string.Empty + m_Chance);
			dictionary.Add("{value_3}", string.Empty + m_ReduceCharge);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			Dictionary<string, string> replacementStrings = new Dictionary<string, string>();
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, replacementStrings);
		}
	}
}
