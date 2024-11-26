using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class SelectClassWithSkinsTutorialStep : BaseTutorialStep
	{
		private CampStateMgr m_campStateMgr;

		private InventoryItemSlot m_itemSlot;

		private string m_itemName;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "items_filled" && trigger != "birdmanager_entered" && trigger != "triggered_forced")
			{
				return;
			}
			DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
			m_campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			SkinItemGameData skinItemGameData = null;
			foreach (IInventoryItemGameData item in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Skin])
			{
				if (item.ItemBalancing.SortPriority != 0)
				{
					skinItemGameData = (SkinItemGameData)item;
					if (DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, skinItemGameData.BalancingData.OriginalClass))
					{
						break;
					}
				}
			}
			if (skinItemGameData == null)
			{
				return;
			}
			string originalClass = skinItemGameData.BalancingData.OriginalClass;
			InventoryItemSlot[] componentsInChildren = m_campStateMgr.m_BirdManager.GetComponentsInChildren<InventoryItemSlot>();
			InventoryItemSlot[] array = componentsInChildren;
			foreach (InventoryItemSlot inventoryItemSlot in array)
			{
				DebugLog.Log("Item Slot Name: " + inventoryItemSlot.GetModel().ItemBalancing.NameId);
				if (originalClass == inventoryItemSlot.GetModel().ItemBalancing.NameId)
				{
					m_itemSlot = inventoryItemSlot;
					break;
				}
			}
			if ((bool)m_itemSlot)
			{
				if (m_itemSlot == m_campStateMgr.m_BirdManager.m_SelectedSlot || m_itemSlot.IsUnavailable())
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
