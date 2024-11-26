using System;
using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class ClickAnvilTutorialStep : BaseTutorialStep
	{
		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "battle_preparation") || !(trigger != "triggered_forced"))
			{
				BattlePreperationUI battlePreperationUI = UnityEngine.Object.FindObjectOfType(typeof(BattlePreperationUI)) as BattlePreperationUI;
				battlePreperationUI.m_startButtonTrigger.Clicked -= StartButtonClicked(m_TutorialIdent, battlePreperationUI);
				battlePreperationUI.m_startButtonTrigger.Clicked += StartButtonClicked(m_TutorialIdent, battlePreperationUI);
				DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
				battlePreperationUI.m_startButtonTrigger.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
				m_TutorialMgr.SetTutorialCameras(true);
				m_TutorialMgr.ShowHelp(battlePreperationUI.m_startButtonTrigger.transform, m_TutorialIdent, 0f, 0f);
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "startButton_clicked"))
			{
				m_TutorialMgr.HideHelp(m_TutorialIdent, true);
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_TutorialMgr.SetTutorialCameras(false);
			}
		}

		private Action StartButtonClicked(string tutIdent, BattlePreperationUI battleUI)
		{
			return delegate
			{
				OnStartButtonClicked(tutIdent, battleUI);
			};
		}

		private void OnStartButtonClicked(string tutIdent, BattlePreperationUI battleUI)
		{
			battleUI.m_startButtonTrigger.Clicked -= StartButtonClicked(tutIdent, battleUI);
			battleUI.m_startButtonTrigger.gameObject.layer = LayerMask.NameToLayer("Interface");
			m_TutorialMgr.SetTutorialCameras(false);
			FinishStep("startButton_clicked", new List<string>());
		}
	}
}
