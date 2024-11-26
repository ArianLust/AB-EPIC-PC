using System.Collections.Generic;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class EnterConsumableBarTutorialStep : BaseTutorialStep
	{
		private CharacterControllerBattleGroundBase m_target;

		private BattleMgr m_battleMgr;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_PauseTrigger = "enter_pause_menu";
			m_ResumeTrigger = "leave_pause_menu";
			m_ResetTrigger = "battle_leave";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void ResetStep()
		{
			base.ResetStep();
			if ((bool)m_battleMgr)
			{
				m_battleMgr.m_BattleUI.ConsumableButtonClicked -= OnConsumableButtonClicked;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.ResetTutorial(m_TutorialIdent);
		}

		protected override void PauseStep()
		{
			base.PauseStep();
			RemoveHelpersAndBlockers(false);
		}

		protected override void ResumeStep()
		{
			base.ResumeStep();
			AddHelpersAndBlockers();
		}

		private void AddHelpersAndBlockers()
		{
			m_battleMgr.m_BattleUI.m_ConsumableButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_TutorialMgr.SetTutorialCameras(true);
			m_TutorialMgr.ShowHelp(m_battleMgr.m_BattleUI.m_ConsumableButton.transform, TutorialStepType.EnterConsumableBar.ToString(), -200f, 0f);
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			m_TutorialMgr.HideHelp(TutorialStepType.EnterConsumableBar.ToString(), finish);
			if ((bool)m_battleMgr)
			{
				m_battleMgr.m_BattleUI.m_ConsumableButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			}
			m_TutorialMgr.SetTutorialCameras(false);
		}

		private void OnConsumableButtonClicked()
		{
			if ((bool)m_battleMgr)
			{
				m_battleMgr.m_BattleUI.ConsumableButtonClicked -= OnConsumableButtonClicked;
			}
			FinishStep("consumable_menu_entered", new List<string>());
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "bird_turn_started" && trigger != "triggered_forced")
			{
				return;
			}
			m_battleMgr = Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			if (m_battleMgr != null && !m_battleMgr.IsConsumableUsePossible)
			{
				return;
			}
			DebugLog.Log("enterPotionsTutorial");
			Object[] array = Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
			m_target = null;
			string itemName = string.Empty;
			if (m_possibleParams.Count > 0)
			{
				itemName = m_possibleParams[0];
			}
			if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, itemName) < 1)
			{
				DebugLog.Log("[Tutorial] Enter Consumable Bar Step Not have Consumable");
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				CharacterControllerBattleGround characterControllerBattleGround = array[i] as CharacterControllerBattleGround;
				if ((bool)characterControllerBattleGround && characterControllerBattleGround.GetModel().CombatantFaction == Faction.Birds && characterControllerBattleGround.GetModel().IsParticipating && characterControllerBattleGround.GetModel().CurrentHealth / characterControllerBattleGround.GetModel().ModifiedHealth <= m_battleMgr.m_BattleUI.m_WarningHealthPercent)
				{
					m_target = characterControllerBattleGround;
					break;
				}
			}
			if ((bool)m_target)
			{
				DebugLog.Log("Show Consumable Button Tutorial!");
				if ((bool)m_battleMgr)
				{
					m_battleMgr.m_BattleUI.ConsumableButtonClicked += OnConsumableButtonClicked;
				}
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (trigger == "consumable_menu_entered")
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}
	}
}
