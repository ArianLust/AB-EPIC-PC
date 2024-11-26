using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class SkinEntryPointTutorialStep : BaseTutorialStep
	{
		private PopupSpecialOfferStateMgr m_spofferMgr;

		private UIInputTrigger m_checkoutBtn;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger == "enter_worldmap")
			{
				StepBackStep();
			}
			if (!(trigger != "special_popup_entered") || !(trigger != "triggered_forced"))
			{
				DebugLog.Log("[Tutorial] Try Start Tutorial: " + m_TutorialIdent + " Step: " + GetType().ToString());
				m_spofferMgr = Object.FindObjectOfType(typeof(PopupSpecialOfferStateMgr)) as PopupSpecialOfferStateMgr;
				if (!(m_spofferMgr == null))
				{
					m_checkoutBtn = m_spofferMgr.m_GetItButton;
					m_checkoutBtn.Clicked -= checkoutClickedHandler;
					m_checkoutBtn.Clicked += checkoutClickedHandler;
					AddHelpersAndBlockers();
					m_Started = true;
				}
			}
		}

		private void checkoutClickedHandler()
		{
			FinishStep("checkout_clicked", new List<string>());
		}

		private void AddHelpersAndBlockers()
		{
			m_checkoutBtn.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_checkoutBtn.transform, TutorialStepType.SkinEntryPoint.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "checkout_clicked"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_TutorialMgr.SkipToTutorialStep(m_TutorialIdent, 2);
				m_Started = false;
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			if ((bool)m_checkoutBtn)
			{
				m_checkoutBtn.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.SkinEntryPoint.ToString(), finish);
			m_TutorialMgr.SetTutorialCameras(false);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_checkoutBtn)
			{
				m_checkoutBtn.Clicked -= checkoutClickedHandler;
			}
			RemoveHelpersAndBlockers(false);
			m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
			m_Started = false;
		}
	}
}
