using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.GameDatas.Battle.Skills
{
	public class RagePrincePorky : AttackSkillTemplate
	{
		private float m_DamageMod;

		private bool m_All;

		private bool m_OneKnockedOut;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			m_All = base.Model.SkillParameters.ContainsKey("all");
			m_FallBackTime *= 4f;
			m_AttackAnimation = (ICombatant c) => c.CombatantView.PlayRageSkillAnimation();
			m_ApplyPerks = false;
			m_IsMelee = false;
			m_UseFocusPosition = true;
			ActionsAfterTargetSelection.Add(delegate(BattleGameData battle, ICombatant source, ICombatant target)
			{
				List<ICombatant> list2 = new List<ICombatant>();
				if (m_All)
				{
					list2 = battle.m_CombatantsPerFaction[Faction.Pigs].Where((ICombatant c) => c.IsAlive || c.IsKnockedOut).ToList();
				}
				else
				{
					list2.Add(target);
				}
				source.AttackTarget = m_Targets.FirstOrDefault();
				m_Targets = list2;
				return 0f;
			});
			ActionsOnDamageDealt.Add(delegate(float damage, BattleGameData battle, ICombatant source, ICombatant target)
			{
				if (target.IsKnockedOut)
				{
					m_OneKnockedOut = true;
				}
			});
			ActionsToDelayEnd.Add(delegate
			{
				float num = 0f;
				foreach (ICombatant item in m_Targets.Where((ICombatant t) => t.IsKnockedOut))
				{
					num = Mathf.Max(num, item.CombatantView.m_AssetController.GetKnockOutAnimationLength());
				}
				return m_OneKnockedOut ? num : 0f;
			});
			ActionsOnEnd.Add(delegate
			{
				List<ICombatant> list = m_Targets.Where((ICombatant c) => c.IsKnockedOut).ToList();
				for (int i = 0; i < list.Count; i++)
				{
					list[i].RaiseCombatantDefeated();
					list[i].IsKnockedOut = false;
				}
				return 0f;
			});
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Convert.ToInt32(m_DamageMod * invoker.ModifiedAttack);
			dictionary.Add("{value_1}", string.Empty + num);
			return DIContainerInfrastructure.GetLocaService().GetSkillDescriptions(base.Model.SkillDescription, dictionary);
		}

		public override string GetLocalizedName()
		{
			return DIContainerInfrastructure.GetLocaService().GetSkillName(base.Model.SkillDescription, new Dictionary<string, string>());
		}
	}
}
