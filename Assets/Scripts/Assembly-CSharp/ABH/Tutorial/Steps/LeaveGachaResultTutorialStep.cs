using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class LeaveGachaResultTutorialStep : BaseTutorialStep
	{
		private CampStateMgr m_campStateMgr;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "gacha_finished") || !(trigger != "triggered_forced"))
			{
				DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
				m_campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
				m_campStateMgr.m_GachaPopup.m_EquipButton.Clicked -= OnEquipButtonClicked;
				m_campStateMgr.m_GachaPopup.m_EquipButton.Clicked += OnEquipButtonClicked;
				m_campStateMgr.m_GachaPopup.m_AcceptButton.Clicked -= OnEquipButtonClicked;
				m_campStateMgr.m_GachaPopup.m_AcceptButton.Clicked += OnEquipButtonClicked;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void OnEquipButtonClicked()
		{
			if ((bool)m_campStateMgr)
			{
				m_campStateMgr.m_GachaPopup.m_EquipButton.Clicked -= OnEquipButtonClicked;
				m_campStateMgr.m_GachaPopup.m_AcceptButton.Clicked -= OnEquipButtonClicked;
			}
			FinishStep("gacha_equipped", new List<string>());
		}

		private void AddHelpersAndBlockers()
		{
			m_campStateMgr.m_GachaPopup.m_EquipButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_campStateMgr.m_GachaPopup.m_AcceptButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_campStateMgr.m_GachaPopup.m_EquipButton.transform, TutorialStepType.LeaveGachaResult.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "gacha_equipped"))
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
				m_campStateMgr.m_GachaPopup.m_EquipButton.gameObject.layer = LayerMask.NameToLayer("Interface");
				m_campStateMgr.m_GachaPopup.m_AcceptButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.LeaveGachaResult.ToString(), finish);
			m_TutorialMgr.SetTutorialCameras(false);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_campStateMgr)
			{
				m_campStateMgr.m_GachaPopup.m_EquipButton.Clicked -= OnEquipButtonClicked;
				m_campStateMgr.m_GachaPopup.m_AcceptButton.Clicked -= OnEquipButtonClicked;
			}
			FinishStep("gacha_equipped", new List<string>());
		}
	}
}
