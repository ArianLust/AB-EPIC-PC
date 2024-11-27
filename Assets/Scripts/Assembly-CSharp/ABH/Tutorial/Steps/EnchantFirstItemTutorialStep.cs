using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class EnchantFirstItemTutorialStep : BaseTutorialStep
	{
		private CampStateMgr m_campStateMgr;

		private UIInputTrigger m_EnchantButton;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "gacha_entered") || !(trigger != "triggered_forced"))
			{
				DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
				m_campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
				m_EnchantButton = m_campStateMgr.m_EnchantmentUi.EnchantButton;
				if (m_EnchantButton == null || !m_campStateMgr.m_EnchantmentUi.IsAnyResourceSelected())
				{
					m_Started = true;
					m_TutorialMgr.FinishTutorial(m_TutorialIdent);
					m_Started = false;
				}
				else
				{
					m_EnchantButton.Clicked -= OnEnchantButtonClicked;
					m_EnchantButton.Clicked += OnEnchantButtonClicked;
					AddHelpersAndBlockers();
					m_Started = true;
				}
			}
		}

		private void OnEnchantButtonClicked()
		{
			if ((bool)m_campStateMgr)
			{
				m_EnchantButton.Clicked -= OnEnchantButtonClicked;
			}
			FinishStep("enchant_clicked", new List<string>());
		}

		private void AddHelpersAndBlockers()
		{
			m_EnchantButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_EnchantButton.transform, TutorialStepType.EnchantFirstItem.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "enchant_clicked"))
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
				m_EnchantButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.EnchantFirstItem.ToString(), finish);
			m_TutorialMgr.SetTutorialCameras(false);
		}
	}
}
