using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class ClickWheelTutorialStep : BaseTutorialStep
	{
		public BattleResultWon m_battleWon { get; set; }

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_ResetTrigger = "battle_to_worldmap";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void ResetStep()
		{
			base.ResetStep();
			if (m_battleWon != null)
			{
				m_battleWon.OnWheelSpinned -= OnWheelClicked;
			}
			m_TutorialMgr.HideHelp(TutorialStepType.ClickWheel.ToString(), false);
			m_Started = false;
			m_TutorialMgr.ResetTutorial(m_TutorialIdent);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "battle_won_wheel_started") || !(trigger != "triggered_forced"))
			{
				DebugLog.Log("click_wheel_tutorial");
				m_battleWon = Object.FindObjectOfType(typeof(BattleResultWon)) as BattleResultWon;
				if ((bool)m_battleWon)
				{
					m_battleWon.OnWheelSpinned -= OnWheelClicked;
					m_battleWon.OnWheelSpinned += OnWheelClicked;
					m_TutorialMgr.ShowHelp(m_battleWon.m_WheelButton.gameObject.transform, TutorialStepType.ClickWheel.ToString(), 0f, 0f);
					m_Started = true;
				}
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (trigger == "wheel_clicked")
			{
				m_TutorialMgr.HideHelp(TutorialStepType.ClickWheel.ToString(), true);
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void OnWheelClicked()
		{
			if (m_battleWon != null)
			{
				m_battleWon.OnWheelSpinned -= OnWheelClicked;
			}
			FinishStep("wheel_clicked", new List<string>());
		}
	}
}
