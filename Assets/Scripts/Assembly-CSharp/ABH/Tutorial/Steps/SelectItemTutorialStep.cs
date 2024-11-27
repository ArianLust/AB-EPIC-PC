using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class SelectItemTutorialStep : BaseTutorialStep
	{
		private ClassManagerUi m_classManagerUi;

		private InventoryItemSlot m_itemSlot;

		private string m_itemName;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "items_filled" && trigger != "bps_classmanager_entered" && trigger != "triggered_forced")
			{
				return;
			}
			DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
			m_itemName = string.Empty;
			m_classManagerUi = Object.FindObjectOfType(typeof(ClassManagerUi)) as ClassManagerUi;
			if (!m_classManagerUi)
			{
				DebugLog.Warn(GetType(), "StartStepForBpsSetting: Did not find ClassManagerUi in scene. Force finishing step!");
				FinishStep("triggered_forced", new List<string>());
				return;
			}
			if (m_possibleParams.Count > 0)
			{
				m_itemName = m_possibleParams[0];
			}
			InventoryItemSlot[] componentsInChildren = m_classManagerUi.GetComponentsInChildren<InventoryItemSlot>();
			InventoryItemSlot[] array = componentsInChildren;
			foreach (InventoryItemSlot inventoryItemSlot in array)
			{
				DebugLog.Log("Item Slot Name: " + inventoryItemSlot.GetModel().ItemBalancing.NameId);
				if (inventoryItemSlot.GetModel().ItemBalancing.NameId == m_itemName)
				{
					m_itemSlot = inventoryItemSlot;
					break;
				}
			}
			if ((bool)m_itemSlot)
			{
				if (m_itemSlot == m_classManagerUi.SelectedSlot || m_itemSlot.IsUnavailable())
				{
					FinishStep("triggered_forced", new List<string>());
					return;
				}
				m_itemSlot.m_InputTrigger.Clicked -= OnItemSlotSelected;
				m_itemSlot.m_InputTrigger.Clicked += OnItemSlotSelected;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void OnItemSlotSelected()
		{
			if ((bool)m_itemSlot)
			{
				m_itemSlot.m_InputTrigger.Clicked -= OnItemSlotSelected;
			}
			FinishStep("slot_clicked", new List<string> { m_itemSlot.GetModel().ItemBalancing.NameId });
		}

		private void AddHelpersAndBlockers()
		{
			m_itemSlot.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
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
			m_TutorialMgr.SetTutorialCameras(false);
			m_TutorialMgr.HideHelp(TutorialStepType.SelectItem.ToString(), finish);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_itemSlot)
			{
				m_itemSlot.m_InputTrigger.Clicked -= OnItemSlotSelected;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}
	}
}
