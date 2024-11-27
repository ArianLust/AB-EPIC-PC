using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class RerollBattleTutorialStep : BaseTutorialStep
	{
		private BattleResultWon m_battleWon;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "battle_won_wheel_done") || !(trigger != "triggered_forced"))
			{
				DebugLog.Log("reroll_tutorial");
				m_battleWon = Object.FindObjectOfType(typeof(BattleResultWon)) as BattleResultWon;
				m_battleWon.m_RerollButton.Clicked -= OnRerollClicked;
				m_battleWon.m_RerollButton.Clicked += OnRerollClicked;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void AddHelpersAndBlockers()
		{
			m_battleWon.m_RerollButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.SetTutorialCameras(true);
			m_TutorialMgr.ShowHelp(m_battleWon.m_RerollButton.transform, TutorialStepType.RerollBattle.ToString(), -200f, 0f);
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			m_TutorialMgr.HideHelp(TutorialStepType.RerollBattle.ToString(), finish);
			if ((bool)m_battleWon)
			{
				m_battleWon.m_RerollButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.SetTutorialCameras(false);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (trigger == "reroll_clicked")
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void OnRerollClicked()
		{
			if ((bool)m_battleWon)
			{
				m_battleWon.m_RerollButton.Clicked -= OnRerollClicked;
			}
			FinishStep("reroll_clicked", new List<string>());
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_battleWon)
			{
				m_battleWon.m_RerollButton.Clicked -= OnRerollClicked;
			}
			FinishStep("reroll_clicked", new List<string>());
		}
	}
}
