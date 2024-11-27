using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class ExitWorldShopTutorialStep : BaseTutorialStep
	{
		private WorldMapShopMenuUI m_WorldShopUI;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "triggered_forced"))
			{
				DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
				m_WorldShopUI = Object.FindObjectOfType(typeof(WorldMapShopMenuUI)) as WorldMapShopMenuUI;
				m_WorldShopUI.BackButtonPressed -= OnExitButtonClicked;
				m_WorldShopUI.BackButtonPressed += OnExitButtonClicked;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void OnExitButtonClicked()
		{
			if ((bool)m_WorldShopUI)
			{
				m_WorldShopUI.BackButtonPressed -= OnExitButtonClicked;
			}
			FinishStep("back_clicked", new List<string>());
		}

		private void AddHelpersAndBlockers()
		{
			m_WorldShopUI.m_BackButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_WorldShopUI.m_BackButton.transform, TutorialStepType.ExitWorldShop.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "back_clicked"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			if ((bool)m_WorldShopUI)
			{
				m_WorldShopUI.m_BackButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.ExitWorldShop.ToString(), finish);
			m_TutorialMgr.SetTutorialCameras(false);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_WorldShopUI)
			{
				m_WorldShopUI.BackButtonPressed -= OnExitButtonClicked;
			}
			FinishStep("back_clicked", new List<string>());
		}
	}
}
