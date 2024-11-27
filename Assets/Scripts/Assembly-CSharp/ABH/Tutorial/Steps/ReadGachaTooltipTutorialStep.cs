using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class ReadGachaTooltipTutorialStep : BaseTutorialStep
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
				m_campStateMgr.m_GachaPopup.m_TapHoldTrigger.OnTapBegin -= OnTapBegan;
				m_campStateMgr.m_GachaPopup.m_TapHoldTrigger.OnTapBegin += OnTapBegan;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void OnTapBegan()
		{
			if ((bool)m_campStateMgr)
			{
				m_campStateMgr.m_GachaPopup.m_TapHoldTrigger.OnTapBegin -= OnTapBegan;
			}
			FinishStep("gacha_tooltip", new List<string>());
		}

		private void AddHelpersAndBlockers()
		{
			m_campStateMgr.m_GachaPopup.m_TapHoldTrigger.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_campStateMgr.m_GachaPopup.m_TapHoldTrigger.transform, TutorialStepType.ReadGachaTooltip.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "gacha_tooltip"))
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
				m_campStateMgr.m_GachaPopup.m_TapHoldTrigger.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.ReadGachaTooltip.ToString(), finish);
			m_TutorialMgr.SetTutorialCameras(false);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_campStateMgr)
			{
				m_campStateMgr.m_GachaPopup.m_TapHoldTrigger.OnTapBegin -= OnTapBegan;
			}
			FinishStep("gacha_tooltip", new List<string>());
		}
	}
}
