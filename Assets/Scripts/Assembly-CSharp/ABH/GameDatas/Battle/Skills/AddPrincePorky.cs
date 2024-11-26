using System.Collections;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;

namespace ABH.GameDatas.Battle.Skills
{
	public class AddPrincePorky : SkillBattleDataBase
	{
		private bool done;

		public override void Init(SkillGameData model)
		{
			base.Init(model);
		}

		public override IEnumerator DoAction(BattleGameData battle, ICombatant source, ICombatant target, bool shared = false, bool illusion = false)
		{
			DoActionInstant(battle, source, target);
			if (!done)
			{
				DebugLog.Log("Trigger environmental skill: " + base.Model.Balancing.NameId + "; Target: " + target.CombatantName);
				m_Source = source;
				BirdGameData porky = new BirdGameData("bird_prince_porky", battle.m_CombatantsPerFaction[Faction.Birds][0].CharacterModel.Level);
				BirdCombatant porkyCombatant = new BirdCombatant(porky);
				battle.m_CombatantsPerFaction[Faction.Birds].Add(porkyCombatant);
				DIContainerLogic.GetBattleService().ReCalculateInitiative(battle);
				done = true;
				yield return m_Source.CombatantView.m_BattleMgr.StartCoroutine(PlacePrincePorky());
				porkyCombatant.CombatantView.SpawnHealthBar();
			}
		}

		public override void DoActionInstant(BattleGameData battle, ICombatant source, ICombatant target)
		{
		}

		private IEnumerator PlacePrincePorky()
		{
			yield return m_Source.CombatantView.m_BattleMgr.StartCoroutine(m_Source.CombatantView.m_BattleMgr.PlaceCharacter(m_Source.CombatantView.m_BattleMgr.m_BirdCenterPosition, Faction.Birds));
			m_Source.CombatantView.m_BattleMgr.StartCoroutine(m_Source.CombatantView.m_BattleMgr.EnterBirds());
		}

		public override string GetLocalizedDescription(ICombatant invoker)
		{
			return string.Empty;
		}

		public override string GetLocalizedName()
		{
			return string.Empty;
		}
	}
}
