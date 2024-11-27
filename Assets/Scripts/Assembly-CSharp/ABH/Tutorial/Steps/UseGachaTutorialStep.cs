using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class UseGachaTutorialStep : BaseTutorialStep
	{
		private CampStateMgr m_campStateMgr;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "gacha_entered") || !(trigger != "triggered_forced"))
			{
				DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
				m_campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
				m_campStateMgr.m_GachaPopup.m_PigMachineButton.Clicked -= OnGachaButtonClicked;
				m_campStateMgr.m_GachaPopup.m_PigMachineButton.Clicked += OnGachaButtonClicked;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void OnGachaButtonClicked()
		{
			if ((bool)m_campStateMgr)
			{
				m_campStateMgr.m_GachaPopup.m_PigMachineButton.Clicked -= OnGachaButtonClicked;
			}
			FinishStep("gacha_clicked", new List<string>());
		}

		private void AddHelpersAndBlockers()
		{
			m_campStateMgr.m_GachaPopup.m_PigMachineButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_campStateMgr.m_GachaPopup.m_PigMachineButton.transform, TutorialStepType.UseGacha.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "gacha_clicked"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			if ((bool)m_campStateMgr)
			{
				m_campStateMgr.m_GachaPopup.m_PigMachineButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.UseGacha.ToString(), finish);
			m_TutorialMgr.SetTutorialCameras(false);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_campStateMgr)
			{
				m_campStateMgr.m_GachaPopup.m_PigMachineButton.Clicked -= OnGachaButtonClicked;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}
	}
}
