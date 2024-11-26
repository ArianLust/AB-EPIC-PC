using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class ClickCampBirdWithSkinTutorialStep : BaseTutorialStep
	{
		private CampStateMgr m_campStateMgr;

		private CharacterControllerCamp m_character;

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
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Remove("tutorial_equip_skin");
				return;
			}
			List<CharacterControllerCamp> birds = m_campStateMgr.getBirds();
			foreach (CharacterControllerCamp item2 in birds)
			{
				if (item2.GetModel() != null && skinItemGameData.IsValidForBird(item2.GetModel() as BirdGameData))
				{
					m_character = item2;
					break;
				}
			}
			if ((bool)m_character)
			{
				m_character.BirdClicked -= OnBirdClicked;
				m_character.BirdClicked += OnBirdClicked;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		private void OnBirdClicked(ICharacter combatant)
		{
			if ((bool)m_character)
			{
				m_character.BirdClicked -= OnBirdClicked;
			}
			FinishStep("bird_clicked", new List<string> { m_character.name });
		}

		private void AddHelpersAndBlockers()
		{
			m_character.gameObject.layer = LayerMask.NameToLayer("TutorialScenery");
			m_TutorialMgr.ShowHelp(m_character.transform, TutorialStepType.ClickCampBirdWithSkin.ToString(), 0f, 0f);
			m_TutorialMgr.SetTutorialCameras(true);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "bird_clicked"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			if ((bool)m_character)
			{
				m_character.gameObject.layer = LayerMask.NameToLayer("Scenery");
			}
			m_TutorialMgr.HideHelp(TutorialStepType.ClickCampBirdWithSkin.ToString(), finish);
			m_TutorialMgr.SetTutorialCameras(false);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_character)
			{
				m_character.BirdClicked -= OnBirdClicked;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}
	}
}
