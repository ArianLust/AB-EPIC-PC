using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class DefeatPigTutorialStep : BaseTutorialStep
	{
		private CharacterControllerBattleGroundBase m_target;

		private CharacterControllerBattleGroundBase m_source;

		private BattleMgr m_battleMgr;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_PauseTrigger = "enter_pause_menu";
			m_ResumeTrigger = "leave_pause_menu";
			m_ResetTrigger = "battle_to_worldmap";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void ResetStep()
		{
			base.ResetStep();
			if ((bool)m_target)
			{
				m_target.GetModel().Defeated -= OnTargetDefeated;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.ResetTutorial(m_TutorialIdent);
		}

		protected override void PauseStep()
		{
			base.PauseStep();
			RemoveHelpersAndBlockers(false);
		}

		protected override void ResumeStep()
		{
			base.ResumeStep();
			AddHelpersAndBlockers();
		}

		private void OnTargetDefeated()
		{
			if ((bool)m_target)
			{
				m_target.GetModel().Defeated -= OnTargetDefeated;
			}
			FinishStep("enemy_defeated", new List<string> { m_target.GetModel().CombatantNameId });
		}

		private void OnTargetClicked(ICombatant combatant)
		{
			if ((bool)m_target)
			{
				m_target.Clicked -= OnTargetClicked;
			}
			FinishStep("enemy_clicked", new List<string> { combatant.CombatantNameId });
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "bird_turn_started" && trigger != "triggered_forced")
			{
				return;
			}
			Object[] array = Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
			m_battleMgr = Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			m_target = null;
			m_source = null;
			string text = string.Empty;
			string text2 = string.Empty;
			if (m_possibleParams.Count > 0)
			{
				text = m_possibleParams[0];
			}
			if (m_possibleParams.Count > 1)
			{
				text2 = m_possibleParams[1];
			}
			for (int i = 0; i < array.Length; i++)
			{
				CharacterControllerBattleGround characterControllerBattleGround = array[i] as CharacterControllerBattleGround;
				if ((bool)characterControllerBattleGround && characterControllerBattleGround.GetModel().CombatantFaction == Faction.Pigs && (string.IsNullOrEmpty(text2) || text2 == characterControllerBattleGround.GetModel().CombatantNameId))
				{
					m_target = characterControllerBattleGround;
					break;
				}
			}
			for (int j = 0; j < array.Length; j++)
			{
				CharacterControllerBattleGround characterControllerBattleGround2 = array[j] as CharacterControllerBattleGround;
				if ((bool)characterControllerBattleGround2 && characterControllerBattleGround2.GetModel().CombatantFaction == Faction.Birds && (string.IsNullOrEmpty(text) || text == characterControllerBattleGround2.GetModel().CombatantNameId))
				{
					m_source = characterControllerBattleGround2;
					break;
				}
			}
			if (!m_source || !m_target)
			{
				m_source = null;
				m_target = null;
				return;
			}
			m_target.GetModel().Defeated -= OnTargetDefeated;
			m_target.GetModel().Defeated += OnTargetDefeated;
			m_target.Clicked -= OnTargetClicked;
			m_target.Clicked += OnTargetClicked;
			AddHelpersAndBlockers();
			m_Started = true;
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (trigger == "enemy_clicked")
			{
				m_TutorialMgr.HideHelp(TutorialStepType.DefeatPig.ToString(), true);
				m_Started = false;
			}
			if (trigger == "enemy_defeated")
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void AddHelpersAndBlockers()
		{
			m_target.gameObject.layer = LayerMask.NameToLayer("TutorialScenery");
			m_source.gameObject.layer = LayerMask.NameToLayer("TutorialScenery");
			m_source.m_AllowClick = false;
			m_TutorialMgr.SetTutorialCameras(true);
			if ((bool)m_battleMgr && (bool)m_source && (bool)m_target)
			{
				m_TutorialMgr.ShowFromToHelp(m_battleMgr, m_source.transform, m_source.m_AssetController.BodyCenter, m_target.m_AssetController.BodyCenter, TutorialStepType.DefeatPig.ToString(), -60f);
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			m_TutorialMgr.HideHelp(TutorialStepType.DefeatPig.ToString(), finish);
			if ((bool)m_target && (bool)m_source)
			{
				m_target.gameObject.layer = LayerMask.NameToLayer("Scenery");
				m_source.gameObject.layer = LayerMask.NameToLayer("Scenery");
				m_source.m_AllowClick = true;
			}
			m_TutorialMgr.SetTutorialCameras(false);
		}
	}
}
