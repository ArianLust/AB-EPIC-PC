using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class EnterEnchantmentTutorialStep : BaseTutorialStep
	{
		private CampStateMgr m_campStateMgr;

		private BirdWindowUI m_BirdManager;

		private EquipmentGameData m_Equipment;

		private BirdEquipmentPreviewUI m_BirdEquipmentPreview;

		private UIInputTrigger m_EnchantButton;

		private EnchantmentTutorialInfoPopup m_EnchantmentTutorialPopup;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "triggered_forced")
			{
				return;
			}
			DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
			m_campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			m_BirdManager = m_campStateMgr.m_BirdManager as BirdWindowUI;
			if (m_BirdManager == null)
			{
				DebugLog.Error(GetType(), "StartStep: No BirdManager found!");
				ResetStep();
				return;
			}
			m_BirdEquipmentPreview = m_BirdManager.BirdEquipmentPreview;
			m_Equipment = m_BirdEquipmentPreview.GetMainHandEquipment();
			if (m_Equipment.Data.Quality == 0)
			{
				FinishStep("item_non_enchantable", new List<string>());
				return;
			}
			if (m_Equipment.IsMaxEnchanted())
			{
				FinishStep("item_already_enchanted", new List<string>());
				return;
			}
			m_EnchantButton = m_BirdManager.m_EnchantItemButton;
			m_EnchantButton.Clicked -= OnCategoryButtonClicked;
			m_EnchantButton.Clicked += OnCategoryButtonClicked;
			AddHelpersAndBlockers();
			m_Started = true;
		}

		private void OnCategoryButtonClicked()
		{
			if ((bool)m_EnchantButton)
			{
				m_EnchantButton.Clicked -= OnCategoryButtonClicked;
			}
			FinishStep("entered_enchantment", new List<string>());
		}

		private void AddHelpersAndBlockers()
		{
			m_EnchantButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_EnchantButton.transform, TutorialStepType.EnterEnchantment.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (trigger == "item_non_enchantable" || trigger == "item_already_enchanted")
			{
				if (trigger == "item_non_enchantable")
				{
					if (m_EnchantmentTutorialPopup == null)
					{
						DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Popup_EnchantmentTutorialInfo", OnEnchantmentTutorialInfoLoaded);
					}
					else
					{
						m_EnchantmentTutorialPopup.gameObject.SetActive(true);
						m_EnchantmentTutorialPopup.Show();
					}
				}
				m_TutorialMgr.FinishTutorial("tutorial_enchantment");
			}
			else if (!(trigger != "entered_enchantment") || !(trigger != "triggered_forced"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void OnEnchantmentTutorialInfoLoaded()
		{
			m_EnchantmentTutorialPopup = Object.FindObjectOfType(typeof(EnchantmentTutorialInfoPopup)) as EnchantmentTutorialInfoPopup;
			m_EnchantmentTutorialPopup.gameObject.SetActive(true);
			m_EnchantmentTutorialPopup.Show();
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			if ((bool)m_EnchantButton)
			{
				m_EnchantButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.SetTutorialCameras(false);
			m_TutorialMgr.HideHelp(TutorialStepType.EnterEnchantment.ToString(), finish);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_EnchantButton)
			{
				m_EnchantButton.Clicked -= OnCategoryButtonClicked;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}
	}
}
