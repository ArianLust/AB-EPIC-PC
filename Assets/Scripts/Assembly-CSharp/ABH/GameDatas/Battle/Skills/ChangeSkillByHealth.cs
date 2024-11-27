using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;

namespace ABH.GameDatas.Battle.Skills
{
	public class ChangeSkillByHealth : AttackSkillTemplate
	{
		private List<float> m_skillChangeHpPercentages;

		private List<SkillBattleDataBase> m_skills;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			m_skillChangeHpPercentages = new List<float>();
			m_skills = new List<SkillBattleDataBase>();
			foreach (string key in base.Model.SkillParameters.Keys)
			{
				if (base.Model.SkillParameters[key] != -1f)
				{
					SkillGameData skillGameData = new SkillGameData(key);
					SkillBattleDataBase skillBattleDataBase = Activator.CreateInstance(Type.GetType("ABH.GameDatas.Battle.Skills." + skillGameData.Balancing.SkillTemplateType, true, true)) as SkillBattleDataBase;
					skillBattleDataBase.Init(skillGameData);
					m_skills.Add(skillBattleDataBase);
					m_skillChangeHpPercentages.Add(base.Model.SkillParameters[key] / 100f);
				}
			}
		}

		private void ChangeWeapon(int i, ICombatant invoker)
		{
			BossCombatant bossCombatant = invoker as BossCombatant;
			if (bossCombatant != null)
			{
				bossCombatant.SetWeaponIndex(i);
			}
			if (m_UseOffhandAnim)
			{
				if (bossCombatant.CombatantOffHandEquipment != null)
				{
					m_SkillProjectileAssetId = bossCombatant.CombatantOffHandEquipment.ProjectileAssetName;
				}
			}
			else if (bossCombatant.CombatantMainHandEquipment != null)
			{
				m_SkillProjectileAssetId = bossCombatant.CombatantMainHandEquipment.ProjectileAssetName;
			}
		}

		public override SkillBattleDataBase CheckForReplacement(ICombatant invoker)
		{
			float num = invoker.CurrentHealth / invoker.ModifiedHealth;
			for (int i = 0; i < m_skillChangeHpPercentages.Count; i++)
			{
				if (num >= m_skillChangeHpPercentages[i] && m_skills.Count >= i + 1)
				{
					ChangeWeapon(i, invoker);
					return m_skills[i];
				}
			}
			ChangeWeapon(m_skills.Count - 1, invoker);
			return m_skills.LastOrDefault();
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			float num = invoker.CurrentHealth / invoker.ModifiedHealth;
			for (int i = 0; i < m_skillChangeHpPercentages.Count; i++)
			{
				if (num >= m_skillChangeHpPercentages[i] && m_skills.Count >= i + 1)
				{
					return m_skills[i].GetLocalizedDescription(invoker);
				}
			}
			return m_skills.LastOrDefault().GetLocalizedDescription(invoker);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}

		public override string GetLocalizedName(ICombatant invoker)
		{
			float num = invoker.CurrentHealth / invoker.ModifiedHealth;
			for (int i = 0; i < m_skillChangeHpPercentages.Count; i++)
			{
				if (num >= m_skillChangeHpPercentages[i] && m_skills.Count >= i + 1)
				{
					return m_skills[i].GetLocalizedName(invoker);
				}
			}
			return m_skills.LastOrDefault().GetLocalizedName(invoker);
		}
	}
}
