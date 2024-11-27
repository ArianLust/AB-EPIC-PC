using System;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackWithPurgeSkill : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_ChargeTurns;

		private float m_AttackCount;

		private float m_PurgeChance;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("charge", out m_ChargeTurns);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			base.Model.SkillParameters.TryGetValue("purge_chance", out m_PurgeChance);
			ModificationsOnEarlyDamageDealtCalculation.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				ShareableActionPart(battle, source, target, false);
				return damage;
			});
		}

		public override void ShareableActionPart(BattleGameData battle, ICombatant source, ICombatant target, bool isShared)
		{
			if (!(UnityEngine.Random.value <= m_PurgeChance / 100f))
			{
				return;
			}
			int num = DIContainerLogic.GetBattleService().RemoveBattleEffects(target, SkillEffectTypes.Blessing);
			if (num > 0)
			{
				VisualEffectSetting setting = null;
				if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting("Purge", out setting))
				{
					SpawnVisualEffects(VisualEffectSpawnTiming.Affected, setting, new List<ICombatant> { target });
				}
			}
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			dictionary.Add("{value_2}", string.Empty + m_ChargeTurns);
			dictionary.Add("{value_3}", string.Empty + m_PurgeChance);
			dictionary.Add("{value_4}", string.Empty + m_AttackCount);
			dictionary.Add("{value_6}", string.Empty + m_PurgeChance);
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
