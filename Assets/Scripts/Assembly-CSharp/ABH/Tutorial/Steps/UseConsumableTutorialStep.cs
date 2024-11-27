using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class UseConsumableTutorialStep : BaseTutorialStep
	{
		private CharacterControllerBattleGroundBase m_target;

		private BattleMgr m_battleMgr;

		private ConsumableBattleButtonController m_consumableButton;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_StepBackTrigger = "battle_consumable_leave";
			m_ResetTrigger = "battle_to_worldmap";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StepBackStep()
		{
			base.StepBackStep();
			if ((bool)m_target)
			{
				m_target.UsedConsumable -= OnConsumableUsed;
			}
			RemoveHelpersAndBlockers(false);
			m_Started = false;
			m_TutorialMgr.StepBackOneTutorialStep(m_TutorialIdent);
		}

		protected override void ResetStep()
		{
			base.ResetStep();
			if ((bool)m_target)
			{
				m_target.UsedConsumable -= OnConsumableUsed;
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
			m_target.gameObject.layer = LayerMask.NameToLayer("TutorialScenery");
			m_consumableButton.m_Drag.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_target.m_AllowClick = false;
			m_target.m_AllowDrag = false;
			m_TutorialMgr.SetTutorialCameras(true);
			GameObject gameObject = new GameObject("TutmarkerInScene");
			gameObject.layer = LayerMask.NameToLayer("Interface");
			Camera interfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
			Camera sceneryCamera = m_battleMgr.m_SceneryCamera;
			gameObject.transform.position = interfaceCamera.ScreenToWorldPoint(sceneryCamera.WorldToScreenPoint(m_target.m_AssetController.BodyCenter.position));
			m_TutorialMgr.ShowFromToHelp(m_battleMgr, m_consumableButton.transform, m_consumableButton.transform, gameObject.transform, TutorialStepType.UseConsumable.ToString(), -200f);
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			foreach (ICombatant item in m_battleMgr.Model.m_CombatantsPerFaction[Faction.Birds])
			{
				item.CombatantView.DisableGlow();
			}
			m_TutorialMgr.HideHelp(TutorialStepType.UseConsumable.ToString(), finish);
			if ((bool)m_battleMgr && (bool)m_target && (bool)m_consumableButton)
			{
				m_consumableButton.m_Drag.gameObject.layer = LayerMask.NameToLayer("Interface");
				m_target.gameObject.layer = LayerMask.NameToLayer("Scenery");
				m_target.m_AllowClick = true;
				m_target.m_AllowDrag = true;
			}
			m_TutorialMgr.SetTutorialCameras(false);
		}

		private void OnConsumableUsed()
		{
			if ((bool)m_target)
			{
				m_target.UsedConsumable -= OnConsumableUsed;
			}
			FinishStep("consumable_used", new List<string> { m_consumableButton.getConsumableName() });
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "consumable_bar_entered")
			{
				return;
			}
			m_battleMgr = Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			ConsumableBattleButtonController[] componentsInChildren = m_battleMgr.m_BattleUI.m_ConsumableBar.m_Grid.gameObject.GetComponentsInChildren<ConsumableBattleButtonController>(true);
			DebugLog.Log("ListLength: " + componentsInChildren.Length);
			string text = string.Empty;
			if (m_possibleParams.Count > 0)
			{
				text = m_possibleParams[0];
			}
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				DebugLog.Log(componentsInChildren[i].getConsumableName());
				if (componentsInChildren[i].getConsumableName() == text)
				{
					m_consumableButton = componentsInChildren[i];
					break;
				}
			}
			if (!m_consumableButton)
			{
				return;
			}
			Object[] array = Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
			for (int j = 0; j < array.Length; j++)
			{
				CharacterControllerBattleGround characterControllerBattleGround = array[j] as CharacterControllerBattleGround;
				if ((bool)characterControllerBattleGround && characterControllerBattleGround.GetModel().CombatantFaction == Faction.Birds && characterControllerBattleGround.GetModel().IsParticipating && characterControllerBattleGround.GetModel().CurrentHealth / characterControllerBattleGround.GetModel().ModifiedHealth <= m_battleMgr.m_BattleUI.m_WarningHealthPercent)
				{
					m_target = characterControllerBattleGround;
					break;
				}
			}
			if ((bool)m_target)
			{
				m_target.UsedConsumable -= OnConsumableUsed;
				m_target.UsedConsumable += OnConsumableUsed;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (trigger == "consumable_used")
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}
	}
}
