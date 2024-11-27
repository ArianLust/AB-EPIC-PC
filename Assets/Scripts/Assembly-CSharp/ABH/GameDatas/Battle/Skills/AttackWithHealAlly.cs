using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackWithHealAlly : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_Percent;

		private float m_AttackCount = 1f;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			base.Model.SkillParameters.TryGetValue("heal_ally", out m_Percent);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			ActionsOnDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				if (damage <= 0f)
				{
					return;
				}
				ICombatant healTarget = source;
				List<ICombatant> list = new List<ICombatant>();
				list.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == healTarget.CombatantFaction).ToList());
				foreach (ICombatant item in list)
				{
					if (!(item.CurrentHealth <= 0f))
					{
						VisualEffectSetting setting = null;
						if (model.SkillParameters.ContainsKey("intensity") && DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting((model.SkillParameters["intensity"] != 1f) ? "Heal_Strong" : "Heal_Weak", out setting))
						{
							SpawnVisualEffects(VisualEffectSpawnTiming.Affected, setting, new List<ICombatant> { item });
						}
						item.HealDamage(damage * m_Percent / 100f, source);
						DIContainerLogic.GetBattleService().HealCurrentTurn(item, battle);
					}
				}
			});
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
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
