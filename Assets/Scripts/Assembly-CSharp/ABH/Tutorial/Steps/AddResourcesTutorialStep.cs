using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class AddResourcesTutorialStep : BaseTutorialStep
	{
		private CampStateMgr m_campStateMgr;

		private EnchantingItemSlot m_itemSlot;

		private string m_itemName;

		private int m_requiredAmount;

		private UITapHoldTrigger m_AddResourceButton;

		private EnchantmentUI m_EnchantmentUi;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "enchantment_resource_clicked" && trigger != "triggered_forced")
			{
				return;
			}
			m_campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			m_EnchantmentUi = Object.FindObjectOfType<EnchantmentUI>();
			if (m_EnchantmentUi != null)
			{
				m_EnchantmentUi.OnSufficientResourcesAllocated += OnLevelUpPossible;
				m_EnchantmentUi.OnAllResourcesSpent += OnResourcesSpent;
			}
			if (m_possibleParams.Count > 0)
			{
				m_itemName = m_possibleParams[0];
			}
			if (m_possibleParams.Count > 1)
			{
				m_requiredAmount = int.Parse(m_possibleParams[1]);
			}
			Object[] array = Object.FindObjectsOfType(typeof(InventoryItemSlot));
			Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				InventoryItemSlot inventoryItemSlot = (InventoryItemSlot)array2[i];
				if (inventoryItemSlot.GetModel().ItemBalancing.NameId == m_itemName)
				{
					m_itemSlot = inventoryItemSlot as EnchantingItemSlot;
					break;
				}
			}
			if (m_itemSlot == null)
			{
				DebugLog.Error(GetType(), "StartStep: EnchantingItemSlot could not be found or cast!");
				return;
			}
			if (m_EnchantmentUi.m_currentSelectedResource.ItemBalancing.NameId != m_itemName)
			{
				FinishStep("triggered_forced", new List<string>());
			}
			m_AddResourceButton = m_itemSlot.m_EnchantingPlusButton;
			AddHelpersAndBlockers();
			m_Started = true;
		}

		private void OnResourcesSpent(string resourceName)
		{
			if (resourceName == m_itemName)
			{
				FinishStep("resources_spent", new List<string>());
			}
		}

		private void OnLevelUpPossible()
		{
			m_EnchantmentUi.OnSufficientResourcesAllocated -= OnLevelUpPossible;
			FinishStep("resources_added", new List<string>());
		}

		private void AddHelpersAndBlockers()
		{
			m_AddResourceButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_EnchantmentUi.TutorialHelperToggleDragging(false);
			m_TutorialMgr.ShowHelp(m_AddResourceButton.transform, TutorialStepType.AddResources.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "resources_added") || !(trigger != "resources_spent") || !(trigger != "triggered_forced"))
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
				m_AddResourceButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_EnchantmentUi.TutorialHelperToggleDragging();
			m_TutorialMgr.HideHelp(TutorialStepType.AddResources.ToString(), finish);
			m_TutorialMgr.SetTutorialCameras(false);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_EnchantmentUi)
			{
				m_EnchantmentUi.OnSufficientResourcesAllocated -= OnLevelUpPossible;
				m_EnchantmentUi.OnAllResourcesSpent -= OnResourcesSpent;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}
	}
}
