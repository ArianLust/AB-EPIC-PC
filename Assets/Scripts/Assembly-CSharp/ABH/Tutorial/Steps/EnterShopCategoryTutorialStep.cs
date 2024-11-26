using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class EnterShopCategoryTutorialStep : BaseTutorialStep
	{
		private ShopCategoryButton m_categoryBlind;

		private string m_categoryName;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "back_button_pressed";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "enter_shop")
			{
				return;
			}
			DebugLog.Log("Start Tutorial: " + m_TutorialIdent);
			m_categoryName = string.Empty;
			if (m_possibleParams.Count > 0)
			{
				m_categoryName = m_possibleParams[0];
			}
			if (string.IsNullOrEmpty(m_categoryName))
			{
				return;
			}
			Object[] array = Object.FindObjectsOfType(typeof(ShopCategoryButton));
			Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ShopCategoryButton shopCategoryButton = (ShopCategoryButton)array2[i];
				if (shopCategoryButton.m_CategoryName == m_categoryName)
				{
					m_categoryBlind = shopCategoryButton;
					break;
				}
			}
			if ((bool)m_categoryBlind)
			{
				m_categoryBlind.m_ButtonTrigger.Clicked -= OnCategorySelected;
				m_categoryBlind.m_ButtonTrigger.Clicked += OnCategorySelected;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void OnCategorySelected()
		{
			if ((bool)m_categoryBlind)
			{
				m_categoryBlind.m_ButtonTrigger.Clicked -= OnCategorySelected;
			}
			FinishStep("category_clicked", new List<string> { m_categoryBlind.m_CategoryName });
		}

		private void AddHelpersAndBlockers()
		{
			m_categoryBlind.m_ButtonTrigger.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.ShowHelp(m_categoryBlind.transform, TutorialStepType.EnterShopCategory.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "category_clicked"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			if ((bool)m_categoryBlind)
			{
				m_categoryBlind.m_ButtonTrigger.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.SetTutorialCameras(false);
			m_TutorialMgr.HideHelp(TutorialStepType.EnterShopCategory.ToString(), finish);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_categoryBlind)
			{
				m_categoryBlind.m_ButtonTrigger.Clicked -= OnCategorySelected;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}
	}
}
