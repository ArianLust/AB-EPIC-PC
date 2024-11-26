using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class PvPStartBattleTutorialStep : BaseTutorialStep
	{
		private ArenaBattlePreperationUI m_battleUI;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "battle_preparation") || !(trigger != "triggered_forced"))
			{
				m_battleUI = Object.FindObjectOfType(typeof(ArenaBattlePreperationUI)) as ArenaBattlePreperationUI;
				m_battleUI.m_startButtonTrigger.Clicked -= OnStartButtonClicked;
				m_battleUI.m_startButtonTrigger.Clicked += OnStartButtonClicked;
				m_battleUI.m_backButtonTrigger.Clicked -= StepBackStep;
				m_battleUI.m_backButtonTrigger.Clicked += StepBackStep;
				DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
				m_TutorialMgr.ShowHelp(m_battleUI.m_startButtonTrigger.transform, TutorialStepType.StartBattle.ToString(), 0f, 0f);
				m_Started = true;
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "startButton_clicked"))
			{
				m_TutorialMgr.HideHelp(TutorialStepType.StartBattle.ToString(), true);
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_battleUI)
			{
				m_battleUI.m_startButtonTrigger.Clicked -= OnStartButtonClicked;
				m_battleUI.m_backButtonTrigger.Clicked -= StepBackStep;
				m_battleUI.m_startButtonTrigger.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.StartBattle.ToString(), false);
			m_TutorialMgr.SetTutorialCameras(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}

		private void OnStartButtonClicked()
		{
			if ((bool)m_battleUI)
			{
				m_battleUI.m_startButtonTrigger.Clicked -= OnStartButtonClicked;
				m_battleUI.m_backButtonTrigger.Clicked -= StepBackStep;
				m_battleUI.m_startButtonTrigger.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.SetTutorialCameras(false);
			FinishStep("startButton_clicked", new List<string>());
		}
	}
}
