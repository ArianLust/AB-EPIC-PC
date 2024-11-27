using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class SelectEnchantmentResourceTutorialStep : BaseTutorialStep
	{
		private CampStateMgr m_campStateMgr;

		private InventoryItemSlot m_itemSlot;

		private string m_recipeName;

		private string m_itemName;

		private EnchantmentUI m_EnchantmentUi;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "items_filled" && trigger != "enchantment_entered")
			{
				return;
			}
			DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
			m_campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			m_EnchantmentUi = Object.FindObjectOfType<EnchantmentUI>();
			m_itemName = string.Empty;
			if (m_possibleParams.Count > 0)
			{
				m_itemName = m_possibleParams[0];
			}
			if (m_possibleParams.Count > 1)
			{
				m_recipeName = m_possibleParams[1];
			}
			InventoryItemSlot[] componentsInChildren = m_campStateMgr.m_EnchantmentUi.GetComponentsInChildren<InventoryItemSlot>();
			InventoryItemSlot[] array = componentsInChildren;
			foreach (InventoryItemSlot inventoryItemSlot in array)
			{
				if (inventoryItemSlot.GetModel() != null && inventoryItemSlot.GetModel().ItemBalancing != null)
				{
					DebugLog.Log("Item Slot Name: " + inventoryItemSlot.GetModel().ItemBalancing.NameId);
					if (inventoryItemSlot.GetModel().ItemBalancing.NameId == m_recipeName)
					{
						m_itemSlot = inventoryItemSlot;
						break;
					}
				}
			}
			if (!m_itemSlot)
			{
				DebugLog.Error(GetType(), "StartStep: ItemSlot not found!");
				return;
			}
			if ((m_campStateMgr.m_EnchantmentUi.m_currentSelectedResource != null && m_itemName == m_campStateMgr.m_EnchantmentUi.m_currentSelectedResource.Name) || m_itemSlot.IsUnavailable())
			{
				FinishStep("triggered_forced", new List<string>());
				return;
			}
			m_itemSlot.BeforeUsed -= OnItemSlotSelected;
			m_itemSlot.BeforeUsed += OnItemSlotSelected;
			AddHelpersAndBlockers();
			m_Started = true;
		}

		private void OnItemSlotSelected(InventoryItemSlot itemSlot)
		{
			if ((bool)m_itemSlot)
			{
				m_itemSlot.BeforeUsed -= OnItemSlotSelected;
			}
			FinishStep("slot_clicked", new List<string> { m_itemSlot.GetModel().ItemBalancing.NameId });
		}

		private void AddHelpersAndBlockers()
		{
			m_itemSlot.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_EnchantmentUi.TutorialHelperToggleDragging(false);
			m_TutorialMgr.ShowHelp(m_itemSlot.transform, TutorialStepType.SelectItem.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "slot_clicked") || !(trigger != "triggered_forced"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			if ((bool)m_itemSlot)
			{
				m_itemSlot.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_EnchantmentUi.TutorialHelperToggleDragging();
			m_TutorialMgr.SetTutorialCameras(false);
			m_TutorialMgr.HideHelp(TutorialStepType.SelectItem.ToString(), finish);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_itemSlot)
			{
				m_itemSlot.BeforeUsed -= OnItemSlotSelected;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}
	}
}
