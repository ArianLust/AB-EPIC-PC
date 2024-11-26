using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class RageBlackBird : AttackSkillTemplate
	{
		private float m_DamageMod;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
			float value = 0f;
			base.Model.SkillParameters.TryGetValue("damage_in_percent", out value);
			m_DamageMod = value / 100f;
			m_FallBackTime *= 4f;
			m_AttackAnimation = (ICombatant c) => c.CombatantView.PlayRageSkillAnimation();
			m_ApplyPerks = false;
			m_UseCenterPosition = true;
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			List<ICombatant> aliveEnemiesBefore = battle.m_CombatantsPerFaction[Faction.Pigs].Where((ICombatant c) => c.IsAlive).ToList();
			battle.SetRageAvailable(source.CombatantFaction, false);
			yield return DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(base.DoAction(battle, source, target, shared, false));
			if (source.CombatantFaction == Faction.Pigs)
			{
				battle.SetFactionRage(Faction.Pigs, 0f);
				battle.RegisterRageUsed(100f, source);
			}
			battle.SetRageAvailable(source.CombatantFaction, true);
			if (battle.IsPvP && source.CombatantFaction == Faction.Birds && !battle.IsUnranked)
			{
				CalculateRageKills(battle, aliveEnemiesBefore);
			}
		}

		private void CalculateRageKills(BattleGameData battle, List<ICombatant> aliveEnemiesBefore)
		{
			List<ICombatant> list = battle.m_CombatantsPerFaction[Faction.Pigs].Where((ICombatant c) => c.IsAlive).ToList();
			foreach (ICombatant item in list)
			{
				aliveEnemiesBefore.Remove(item);
			}
			if (aliveEnemiesBefore.Count <= 0)
			{
				return;
			}
			foreach (ICombatant item2 in aliveEnemiesBefore)
			{
				DIContainerLogic.GetPvpObjectivesService().RageUsedToKill(item2);
			}
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
