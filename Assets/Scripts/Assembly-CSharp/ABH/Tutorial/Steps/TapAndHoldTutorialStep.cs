using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class TapAndHoldTutorialStep : BaseTutorialStep
	{
		private CharacterControllerBattleGroundBase m_character;

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
			if ((bool)m_character)
			{
				m_character.ShowTooltip -= OnCharacterTooltipShown;
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

		private void AddHelpersAndBlockers()
		{
			m_character.gameObject.layer = LayerMask.NameToLayer("TutorialScenery");
			m_character.m_AllowDrag = false;
			m_character.m_AllowClick = false;
			m_TutorialMgr.SetTutorialCameras(true);
			m_TutorialMgr.ShowHelp(m_character.m_AssetController.BodyCenter, TutorialStepType.TapAndHold.ToString(), -60f, 0f);
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			m_TutorialMgr.HideHelp(TutorialStepType.TapAndHold.ToString(), finish);
			if ((bool)m_character)
			{
				m_character.gameObject.layer = LayerMask.NameToLayer("Scenery");
				m_character.m_AllowDrag = true;
				m_character.m_AllowClick = true;
				m_TutorialMgr.SetTutorialCameras(false);
			}
		}

		private void OnCharacterTooltipShown(ICombatant combatant)
		{
			if ((bool)m_character)
			{
				m_character.ShowTooltip -= OnCharacterTooltipShown;
			}
			FinishStep("character_showtooltip", new List<string> { combatant.CombatantNameId });
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "bird_turn_started" && trigger != "triggered_forced")
			{
				return;
			}
			Object[] array = Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
			BattleMgr battleMgr = Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			m_character = null;
			string text = string.Empty;
			string empty = string.Empty;
			if (m_possibleParams.Count > 0)
			{
				text = m_possibleParams[0];
			}
			Faction faction = Faction.Birds;
			if (m_possibleParams.Count > 1 && m_possibleParams[1] == "pigs")
			{
				faction = Faction.Pigs;
			}
			for (int i = 0; i < array.Length; i++)
			{
				CharacterControllerBattleGround characterControllerBattleGround = array[i] as CharacterControllerBattleGround;
				if ((bool)characterControllerBattleGround && characterControllerBattleGround.GetModel().CombatantFaction == faction && (string.IsNullOrEmpty(text) || text == characterControllerBattleGround.GetModel().CombatantNameId))
				{
					m_character = characterControllerBattleGround;
					break;
				}
			}
			if ((bool)m_character)
			{
				m_character.ShowTooltip -= OnCharacterTooltipShown;
				m_character.ShowTooltip += OnCharacterTooltipShown;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (trigger == "character_showtooltip")
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}
	}
}
