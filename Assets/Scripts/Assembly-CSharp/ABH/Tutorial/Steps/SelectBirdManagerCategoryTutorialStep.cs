using System.Collections.Generic;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class SelectBirdManagerCategoryTutorialStep : BaseTutorialStep
	{
		private CampStateMgr m_campStateMgr;

		private OpenInventoryButton m_categoryButton;

		private string m_categoryName;

		private string m_itemName;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "birdmanager_entered")
			{
				return;
			}
			DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
			m_campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			m_categoryName = string.Empty;
			m_itemName = string.Empty;
			if (m_possibleParams.Count > 0)
			{
				m_categoryName = m_possibleParams[0];
			}
			Object[] array = Object.FindObjectsOfType(typeof(OpenInventoryButton));
			Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				OpenInventoryButton openInventoryButton = (OpenInventoryButton)array2[i];
				DebugLog.Log("Category Name: " + openInventoryButton.m_ItemType);
				if (openInventoryButton.m_ItemType.ToString() == m_categoryName)
				{
					m_categoryButton = openInventoryButton;
					break;
				}
			}
			if ((bool)m_categoryButton)
			{
				m_categoryButton.OnButtonClicked -= OnCategoryButtonClicked;
				m_categoryButton.OnButtonClicked += OnCategoryButtonClicked;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void OnCategoryButtonClicked(InventoryItemType category)
		{
			if ((bool)m_categoryButton)
			{
				m_categoryButton.OnButtonClicked -= OnCategoryButtonClicked;
			}
			FinishStep("category_birdmanager_clicked", new List<string> { category.ToString() });
		}

		private void AddHelpersAndBlockers()
		{
			m_categoryButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_categoryButton.transform, TutorialStepType.SelectCategory.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "category_birdmanager_clicked") || !(trigger != "triggered_forced"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			if ((bool)m_categoryButton)
			{
				m_categoryButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.SetTutorialCameras(false);
			m_TutorialMgr.HideHelp(TutorialStepType.SelectCategory.ToString(), finish);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_categoryButton)
			{
				m_categoryButton.OnButtonClicked -= OnCategoryButtonClicked;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}
	}
}
