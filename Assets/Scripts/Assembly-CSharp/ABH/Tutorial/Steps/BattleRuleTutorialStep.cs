using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class BattleRuleTutorialStep : BaseTutorialStep
	{
		private BattlePreperationUI m_battleUI;

		private RulesOverlayInvoker m_targetedRule;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_ResetTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void ResetStep()
		{
			if (m_battleUI != null)
			{
				m_battleUI.m_startButtonTrigger.Clicked -= StartButtonTriggerClicked;
				m_battleUI.m_backButtonTrigger.Clicked -= StartButtonTriggerClicked;
			}
			if ((bool)m_targetedRule)
			{
				m_targetedRule.m_TapHoldTrigger.OnTapBegin -= OnTooltipInvoked;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "battle_rules"))
			{
				m_battleUI = Object.FindObjectOfType(typeof(BattlePreperationUI)) as BattlePreperationUI;
				RulesOverlayInvoker[] componentsInChildren = m_battleUI.m_RulesGrid.GetComponentsInChildren<RulesOverlayInvoker>(true);
				m_targetedRule = null;
				m_targetedRule = componentsInChildren.FirstOrDefault();
				if ((bool)m_targetedRule)
				{
					m_battleUI.m_startButtonTrigger.Clicked -= StartButtonTriggerClicked;
					m_battleUI.m_startButtonTrigger.Clicked += StartButtonTriggerClicked;
					m_battleUI.m_backButtonTrigger.Clicked -= StartButtonTriggerClicked;
					m_battleUI.m_backButtonTrigger.Clicked += StartButtonTriggerClicked;
					m_targetedRule.m_TapHoldTrigger.OnTapBegin -= OnTooltipInvoked;
					m_targetedRule.m_TapHoldTrigger.OnTapBegin += OnTooltipInvoked;
					DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
					AddHelpersAndBlockers();
					m_Started = true;
				}
			}
		}

		private void StartButtonTriggerClicked()
		{
			ResetStep();
		}

		private void AddHelpersAndBlockers()
		{
			if ((bool)m_battleUI)
			{
				m_battleUI.m_startButtonTrigger.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
				m_battleUI.m_backButtonTrigger.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
				BattlePrepCharacterButton[] componentsInChildren = m_battleUI.GetComponentsInChildren<BattlePrepCharacterButton>();
				for (int i = 0; i < componentsInChildren.Count(); i++)
				{
					BattlePrepCharacterButton battlePrepCharacterButton = componentsInChildren[i];
					battlePrepCharacterButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
				}
				m_targetedRule.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
				m_TutorialMgr.ShowHelp(m_targetedRule.transform, TutorialStepType.BattleRule.ToString(), 0f, 0f);
				m_TutorialMgr.SetTutorialCameras(true);
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			m_TutorialMgr.SetTutorialCameras(false);
			if ((bool)m_battleUI)
			{
				m_battleUI.m_startButtonTrigger.gameObject.layer = LayerMask.NameToLayer("Interface");
				m_battleUI.m_backButtonTrigger.gameObject.layer = LayerMask.NameToLayer("Interface");
				m_targetedRule.gameObject.layer = LayerMask.NameToLayer("Interface");
				BattlePrepCharacterButton[] componentsInChildren = m_battleUI.GetComponentsInChildren<BattlePrepCharacterButton>();
				for (int i = 0; i < componentsInChildren.Count(); i++)
				{
					BattlePrepCharacterButton battlePrepCharacterButton = componentsInChildren[i];
					battlePrepCharacterButton.gameObject.layer = LayerMask.NameToLayer("Interface");
				}
				m_TutorialMgr.HideHelp(TutorialStepType.BattleRule.ToString(), finish);
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "tooltip_invoked"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void OnTooltipInvoked()
		{
			if (m_battleUI != null)
			{
				m_battleUI.m_startButtonTrigger.Clicked -= StartButtonTriggerClicked;
				m_battleUI.m_backButtonTrigger.Clicked -= StartButtonTriggerClicked;
			}
			if ((bool)m_targetedRule)
			{
				m_targetedRule.m_TapHoldTrigger.OnTapBegin -= OnTooltipInvoked;
			}
			FinishStep("tooltip_invoked", new List<string>());
		}
	}
}
