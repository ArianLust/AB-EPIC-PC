using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;

namespace ABH.GameDatas.Battle.Skills
{
	public class AttackWithHealWeakerAlly : AttackSkillTemplate
	{
		private float m_DamageMod;

		private float m_Percent;

		private float m_AttackCount = 1f;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			base.Model.SkillParameters.TryGetValue("attack_count", out m_AttackCount);
			m_DamageMod = value / 100f;
			if (!base.Model.SkillParameters.TryGetValue("heal_ally", out m_Percent))
			{
				m_Percent = 0f;
			}
			ActionsOnDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				ICombatant healTarget = source;
				List<ICombatant> list = new List<ICombatant>();
				list.AddRange(battle.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == healTarget.CombatantFaction && c.CurrentHealth > 0f).ToList());
				foreach (ICombatant item in list)
				{
					if (item.CurrentHealth / item.ModifiedHealth < healTarget.CurrentHealth / healTarget.ModifiedHealth)
					{
						healTarget = item;
					}
				}
				if (!(healTarget.CurrentHealth <= 0f))
				{
					VisualEffectSetting setting = null;
					float num = damage * m_Percent / 100f;
					if (model.SkillParameters.ContainsKey("intensity") && num > 0f && DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting((model.SkillParameters["intensity"] != 1f) ? "Heal_Strong" : "Heal_Weak", out setting))
					{
						SpawnVisualEffects(VisualEffectSpawnTiming.Affected, setting, new List<ICombatant> { healTarget });
					}
					healTarget.HealDamage(num, source);
					DIContainerLogic.GetBattleService().HealCurrentTurn(healTarget, battle);
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
