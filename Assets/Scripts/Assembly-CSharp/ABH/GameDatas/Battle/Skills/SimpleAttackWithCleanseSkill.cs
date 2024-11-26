using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class SimpleAttackWithCleanseSkill : AttackSkillTemplate
	{
		private float m_CleanseChance;

		private bool m_Cleanse;

		private float m_Damage;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out m_Damage);
			base.Model.SkillParameters.TryGetValue("cleanse_chance", out m_CleanseChance);
			ModificationsOnDamageCalculation.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				m_Cleanse = Random.value <= m_CleanseChance / 100f;
				if (m_Cleanse && source.CurrrentEffects != null)
				{
					int num = DIContainerLogic.GetBattleService().RemoveBattleEffects(source, SkillEffectTypes.Curse);
					if (num > 0)
					{
						VisualEffectSetting setting = null;
						if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting("Cleanse", out setting))
						{
							SpawnVisualEffects(VisualEffectSpawnTiming.Affected, setting, new List<ICombatant> { source });
						}
					}
				}
				return damage;
			});
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + (int)(m_Damage / 100f * invoker.ModifiedAttack));
			dictionary.Add("{value_2}", string.Empty + base.Model.Balancing.EffectDuration);
			dictionary.Add("{value_3}", string.Empty + m_CleanseChance);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
