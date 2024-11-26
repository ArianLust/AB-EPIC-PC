using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class EnterCampTutorialStep : BaseTutorialStep
	{
		private WorldMapMenuUI m_worldMapMenuUI;

		private WorldMapStateMgr m_stateMgr;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "enter_worldmap") || !(trigger != "triggered_forced"))
			{
				DebugLog.Log("Started Camp Button Tutorial");
				m_stateMgr = Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
				if ((bool)m_stateMgr)
				{
					m_stateMgr.m_ProgressIndicatorBlocked = true;
					m_worldMapMenuUI = m_stateMgr.m_WorldMenuUI;
					m_worldMapMenuUI.m_CampButton.Clicked -= OnCampButtonClicked;
					m_worldMapMenuUI.m_CampButton.Clicked += OnCampButtonClicked;
					DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
					m_worldMapMenuUI.m_CampButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
					m_TutorialMgr.SetTutorialCameras(true);
					m_TutorialMgr.ShowHelp(m_worldMapMenuUI.m_CampButton.transform, TutorialStepType.EnterCamp.ToString(), 0f, 0f);
					m_Started = true;
				}
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "campButton_clicked"))
			{
				m_TutorialMgr.HideHelp(TutorialStepType.EnterCamp.ToString(), true);
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_stateMgr.m_ProgressIndicatorBlocked = false;
				m_Started = false;
			}
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_worldMapMenuUI)
			{
				m_worldMapMenuUI.m_CampButton.Clicked -= OnCampButtonClicked;
				m_worldMapMenuUI.m_CampButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.EnterCamp.ToString(), false);
			m_TutorialMgr.SetTutorialCameras(false);
			m_stateMgr.m_ProgressIndicatorBlocked = false;
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}

		private void OnCampButtonClicked()
		{
			if ((bool)m_worldMapMenuUI)
			{
				m_worldMapMenuUI.m_CampButton.Clicked -= OnCampButtonClicked;
				m_worldMapMenuUI.m_CampButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.SetTutorialCameras(false);
			FinishStep("campButton_clicked", new List<string>());
		}
	}
}
