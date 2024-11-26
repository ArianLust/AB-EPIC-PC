using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class CraftItemTutorialStep : BaseTutorialStep
	{
		private CampStateMgr m_campStateMgr;

		private InventoryItemSlot m_itemSlot;

		private string m_categoryName;

		private string m_itemName;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "recipes_filled" && trigger != "triggered_forced")
			{
				return;
			}
			DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
			m_campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			m_categoryName = string.Empty;
			m_itemName = string.Empty;
			if (m_possibleParams.Count > 0)
			{
				m_itemName = m_possibleParams[0];
			}
			if (m_possibleParams.Count > 1)
			{
				m_categoryName = m_possibleParams[1];
			}
			if (m_campStateMgr.m_ForgeWindow.m_selectedCategory.ToString() != m_categoryName && !string.IsNullOrEmpty(m_categoryName))
			{
				return;
			}
			Object[] array = Object.FindObjectsOfType(typeof(InventoryItemSlot));
			Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				InventoryItemSlot inventoryItemSlot = (InventoryItemSlot)array2[i];
				DebugLog.Log("Item Slot Name: " + inventoryItemSlot.GetModel().ItemBalancing.NameId);
				if (inventoryItemSlot.GetModel().ItemBalancing.NameId == m_itemName)
				{
					m_itemSlot = inventoryItemSlot;
					break;
				}
			}
			if ((bool)m_itemSlot && m_itemSlot.m_Used)
			{
				m_campStateMgr.m_ForgeWindow.m_CraftButton.Clicked -= OnCraftButtonClicked;
				m_campStateMgr.m_ForgeWindow.m_CraftButton.Clicked += OnCraftButtonClicked;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void OnCraftButtonClicked()
		{
			if ((bool)m_campStateMgr)
			{
				m_campStateMgr.m_ForgeWindow.m_CraftButton.Clicked -= OnCraftButtonClicked;
			}
			FinishStep("craft_clicked", new List<string> { m_itemSlot.GetModel().ItemBalancing.NameId });
		}

		private void AddHelpersAndBlockers()
		{
			m_campStateMgr.m_ForgeWindow.m_CraftButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_campStateMgr.m_ForgeWindow.m_CraftButton.transform, TutorialStepType.CraftItem.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "craft_clicked"))
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
				m_campStateMgr.m_ForgeWindow.m_CraftButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.CraftItem.ToString(), finish);
			m_TutorialMgr.SetTutorialCameras(false);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_campStateMgr)
			{
				m_campStateMgr.m_ForgeWindow.m_CraftButton.Clicked -= OnCraftButtonClicked;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}
	}
}
