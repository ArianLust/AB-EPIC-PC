using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class LeaveShopOrSkipTutorialStep : BaseTutorialStep
	{
		private ShopWindowStateMgr m_shopStateMgr;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			m_ResetTrigger = "birdmanager_entered";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void ResetStep()
		{
			base.ResetStep();
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			StartStep("birdmanager_entered", new List<string>());
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "birdmanager_entered" && trigger != "triggered_forced")
			{
				return;
			}
			DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
			if (trigger == "birdmanager_entered")
			{
				m_TutorialMgr.SkipToTutorialStep(m_TutorialIdent, 3, true);
				return;
			}
			if (DIContainerInfrastructure.LocationStateMgr != null)
			{
				m_TutorialMgr.StartTutorial("tutorial_skin_shopped_worldmap");
				m_TutorialMgr.FinishTutorial(m_TutorialIdent);
				return;
			}
			m_shopStateMgr = Object.FindObjectOfType(typeof(ShopWindowStateMgr)) as ShopWindowStateMgr;
			if (!(m_shopStateMgr == null))
			{
				m_shopStateMgr.m_BackButton.Clicked -= OnExitButtonClicked;
				m_shopStateMgr.m_BackButton.Clicked += OnExitButtonClicked;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void OnExitButtonClicked()
		{
			if ((bool)m_shopStateMgr)
			{
				m_shopStateMgr.m_BackButton.Clicked -= OnExitButtonClicked;
			}
			FinishStep("back_clicked", new List<string>());
		}

		private void AddHelpersAndBlockers()
		{
			m_shopStateMgr.m_BackButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_shopStateMgr.m_BackButton.transform, TutorialStepType.LeaveShop.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "back_clicked") || !(trigger != "triggered_forced"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_TutorialMgr.SkipToTutorialStep(m_TutorialIdent, 5, true);
				m_Started = false;
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			if ((bool)m_shopStateMgr)
			{
				m_shopStateMgr.m_BackButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.LeaveShop.ToString(), finish);
			m_TutorialMgr.SetTutorialCameras(false);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_shopStateMgr)
			{
				m_shopStateMgr.m_BackButton.Clicked -= OnExitButtonClicked;
			}
			FinishStep("back_clicked", new List<string>());
		}
	}
}
