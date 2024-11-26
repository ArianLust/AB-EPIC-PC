using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class ClickCampPropTutorialStep : BaseTutorialStep
	{
		private CampStateMgr m_campStateMgr;

		private CampProp m_campProp;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "enter_camp" && trigger != "triggered_forced")
			{
				return;
			}
			DebugLog.Log("[Tutorial] Try Start Tutorial: " + m_TutorialIdent + " Step: " + GetType().ToString());
			m_campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			CampProp[] componentsInChildren = m_campStateMgr.GetComponentsInChildren<CampProp>();
			CampProp[] array = componentsInChildren;
			foreach (CampProp campProp in array)
			{
				if (campProp.GetModel() != null && ContainsParameter(new List<string> { campProp.GetModel().BalancingData.NameId }))
				{
					m_campProp = campProp;
					break;
				}
			}
			if ((bool)m_campProp)
			{
				m_campProp.OnPropClicked -= OnPropClicked;
				m_campProp.OnPropClicked += OnPropClicked;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void OnPropClicked(BasicItemGameData propUnlockItem)
		{
			if ((bool)m_campProp && !(propUnlockItem.BalancingData.NameId != m_campProp.GetModel().BalancingData.NameId))
			{
				m_campProp.OnPropClicked -= OnPropClicked;
				FinishStep("prop_clicked", new List<string> { propUnlockItem.ItemBalancing.NameId });
			}
		}

		private void AddHelpersAndBlockers()
		{
			m_campProp.gameObject.layer = LayerMask.NameToLayer("TutorialScenery");
			if (m_campProp.transform.name == "GoldenPigMachine")
			{
				m_TutorialMgr.ShowHelp(m_campProp.transform, TutorialStepType.ClickCampProp.ToString(), 0f, 50f);
			}
			else
			{
				m_TutorialMgr.ShowHelp(m_campProp.transform, TutorialStepType.ClickCampProp.ToString(), 0f, 0f);
			}
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "prop_clicked"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			if ((bool)m_campProp)
			{
				m_campProp.gameObject.layer = LayerMask.NameToLayer("Scenery");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.ClickCampProp.ToString(), finish);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_campProp)
			{
				m_campProp.OnPropClicked -= OnPropClicked;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}
	}
}
