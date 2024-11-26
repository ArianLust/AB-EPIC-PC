using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class ChangeClassInBpsTutorialStep : BaseTutorialStep
	{
		private BattlePreperationUI m_battleUI;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "battle_preparation"))
			{
				m_battleUI = Object.FindObjectOfType(typeof(BattlePreperationUI)) as BattlePreperationUI;
				m_battleUI.ClassChangeButton.Clicked -= OnStartButtonClicked;
				m_battleUI.ClassChangeButton.Clicked += OnStartButtonClicked;
				m_battleUI.ClassChangeButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
				m_TutorialMgr.SetTutorialCameras(true);
				m_TutorialMgr.ShowHelp(m_battleUI.ClassChangeButton.transform, TutorialStepType.ChangeClassInBps.ToString(), 0f, 0f);
				m_Started = true;
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "startButton_clicked"))
			{
				m_TutorialMgr.HideHelp(TutorialStepType.ChangeClassInBps.ToString(), true);
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_battleUI)
			{
				m_battleUI.ClassChangeButton.Clicked -= OnStartButtonClicked;
				m_battleUI.ClassChangeButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.ChangeClassInBps.ToString(), false);
			m_TutorialMgr.SetTutorialCameras(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}

		private void OnStartButtonClicked()
		{
			if ((bool)m_battleUI)
			{
				m_battleUI.ClassChangeButton.Clicked -= OnStartButtonClicked;
				m_battleUI.ClassChangeButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.SetTutorialCameras(false);
			FinishStep("startButton_clicked", new List<string>());
		}
	}
}
