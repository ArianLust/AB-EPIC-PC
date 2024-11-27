using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class PvPClickFightButtonTutorialStep : BaseTutorialStep
	{
		private ArenaCampMenuUI m_menu;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "enter_arena_camp") || !(trigger != "triggered_forced"))
			{
				DebugLog.Log("Started Start PvP Battle Tutorial");
				ArenaCampStateMgr arenaCampStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_ArenaCampStateMgr;
				if ((bool)arenaCampStateMgr)
				{
					m_menu = arenaCampStateMgr.m_CampUI;
					m_menu.StartPvpButton.Clicked -= OnFightButtonClicked;
					m_menu.StartPvpButton.Clicked += OnFightButtonClicked;
					DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
					m_menu.StartPvpButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
					m_menu.WorldMapButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
					m_TutorialMgr.SetTutorialCameras(true);
					m_TutorialMgr.ShowHelp(m_menu.StartPvpButton.transform, TutorialStepType.EnterCamp.ToString(), 0f, 0f);
					m_Started = true;
				}
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "fightbutton_clicked"))
			{
				m_TutorialMgr.HideHelp(TutorialStepType.EnterCamp.ToString(), true);
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_menu)
			{
				m_menu.StartPvpButton.Clicked -= OnFightButtonClicked;
				m_menu.StartPvpButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.EnterCamp.ToString(), false);
			m_TutorialMgr.SetTutorialCameras(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}

		private void OnFightButtonClicked()
		{
			if ((bool)m_menu)
			{
				m_menu.StartPvpButton.Clicked -= OnFightButtonClicked;
				m_menu.StartPvpButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.SetTutorialCameras(false);
			FinishStep("fightbutton_clicked", new List<string>());
		}
	}
}
