using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class UseRageTutorialStep : BaseTutorialStep
	{
		private CharacterControllerBattleGroundBase m_target;

		private BattleMgr m_battleMgr;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_PauseTrigger = "enter_pause_menu";
			m_ResumeTrigger = "leave_pause_menu";
			m_ResetTrigger = "battle_to_worldmap";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void ResetStep()
		{
			base.ResetStep();
			if ((bool)m_battleMgr)
			{
				m_battleMgr.Model.RageUsed -= OnRageUsed;
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
			m_battleMgr.m_BattleUI.m_RageMeter.m_DragTrigger.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
			m_target.m_AllowClick = false;
			m_target.m_AllowDrag = false;
			m_TutorialMgr.SetTutorialCameras(true);
			m_TutorialMgr.ShowFromToHelp(m_battleMgr, m_battleMgr.m_BattleUI.m_RageMeter.m_RageMeterPosInScene.transform, m_battleMgr.m_BattleUI.m_RageMeter.m_RageMeterPosInScene.transform, m_target.m_AssetController.BodyCenter, TutorialStepType.UseRage.ToString(), -200f);
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			m_TutorialMgr.HideHelp(TutorialStepType.UseRage.ToString(), finish);
			if ((bool)m_battleMgr && (bool)m_target)
			{
				m_battleMgr.m_BattleUI.m_RageMeter.m_DragTrigger.gameObject.layer = LayerMask.NameToLayer("Interface");
				m_target.gameObject.layer = LayerMask.NameToLayer("Scenery");
				m_target.m_AllowClick = true;
				m_target.m_AllowDrag = true;
			}
			m_TutorialMgr.SetTutorialCameras(false);
		}

		private void OnRageUsed(float value, ICombatant combatant)
		{
			m_battleMgr.Model.RageUsed -= OnRageUsed;
			FinishStep("rage_used", new List<string> { combatant.CombatantNameId });
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "bird_turn_started")
			{
				return;
			}
			m_battleMgr = Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			Object[] array = Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
			if (m_battleMgr.Model.m_CurrentRage < 100f)
			{
				return;
			}
			DebugLog.Log("Show RageMeter Tutorial!");
			m_target = null;
			string text = string.Empty;
			if (m_possibleParams.Count > 0)
			{
				text = m_possibleParams[0];
			}
			for (int i = 0; i < array.Length; i++)
			{
				CharacterControllerBattleGround characterControllerBattleGround = array[i] as CharacterControllerBattleGround;
				if ((bool)characterControllerBattleGround && characterControllerBattleGround.GetModel().CombatantFaction == Faction.Birds && characterControllerBattleGround.GetModel().IsRageAvailiable && (string.IsNullOrEmpty(text) || text == characterControllerBattleGround.GetModel().CombatantNameId))
				{
					m_target = characterControllerBattleGround;
					break;
				}
			}
			if ((bool)m_target)
			{
				m_battleMgr.Model.RageUsed -= OnRageUsed;
				m_battleMgr.Model.RageUsed += OnRageUsed;
				AddHelpersAndBlockers();
				m_Started = true;
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (trigger == "rage_used")
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}
	}
}
