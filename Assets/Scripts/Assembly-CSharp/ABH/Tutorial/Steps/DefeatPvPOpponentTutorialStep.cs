using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class DefeatPvPOpponentTutorialStep : BaseTutorialStep
	{
		private CharacterControllerBattleGroundBase m_target;

		private CharacterControllerBattleGroundBase m_source;

		private Object[] m_characters;

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
			if ((bool)m_source)
			{
				m_source.GetModel().SkillTriggered -= OnActionTaken;
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

		private void OnActionTaken(ICombatant source, ICombatant target)
		{
			for (int i = 0; i < m_characters.Length; i++)
			{
				CharacterControllerBattleGround characterControllerBattleGround = m_characters[i] as CharacterControllerBattleGround;
				if ((bool)characterControllerBattleGround)
				{
					characterControllerBattleGround.GetModel().SkillTriggered -= OnActionTaken;
				}
			}
			FinishStep("action_taken", new List<string>());
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "bird_turn_started" && trigger != "triggered_forced")
			{
				return;
			}
			m_characters = Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
			m_battleMgr = Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			m_target = null;
			m_source = null;
			string text = string.Empty;
			string text2 = string.Empty;
			if (m_possibleParams.Count > 0)
			{
				text = m_possibleParams[0];
			}
			if (m_possibleParams.Count > 1)
			{
				text2 = m_possibleParams[1];
			}
			List<CharacterControllerBattleGround> list = new List<CharacterControllerBattleGround>();
			for (int i = 0; i < m_characters.Length; i++)
			{
				CharacterControllerBattleGround characterControllerBattleGround = m_characters[i] as CharacterControllerBattleGround;
				if (characterControllerBattleGround != null && characterControllerBattleGround.GetModel().CombatantFaction == Faction.Pigs && !characterControllerBattleGround.GetModel().IsBanner)
				{
					list.Add(characterControllerBattleGround);
				}
				if ((bool)characterControllerBattleGround && characterControllerBattleGround.GetModel().CombatantFaction == Faction.Pigs && (string.IsNullOrEmpty(text2) || text2 == characterControllerBattleGround.GetModel().CombatantNameId))
				{
					m_target = characterControllerBattleGround;
				}
			}
			List<CharacterControllerBattleGround> list2 = new List<CharacterControllerBattleGround>();
			for (int j = 0; j < m_characters.Length; j++)
			{
				CharacterControllerBattleGround characterControllerBattleGround2 = m_characters[j] as CharacterControllerBattleGround;
				if (!(characterControllerBattleGround2 == null) && !characterControllerBattleGround2.GetModel().IsBanner && characterControllerBattleGround2.GetModel().CombatantFaction == Faction.Birds)
				{
					list2.Add(characterControllerBattleGround2);
				}
			}
			if (int.Parse(text) >= list2.Count)
			{
				FinishStep("less_than_3_birds", new List<string>());
				return;
			}
			for (int k = 0; k < list2.Count; k++)
			{
				CharacterControllerBattleGround source = list2[k];
				if (k.ToString() == text)
				{
					m_source = source;
					break;
				}
			}
			if (m_source.GetModel().ActedThisTurn)
			{
				m_Started = false;
				FinishStep("already_acted", new List<string>());
				return;
			}
			if (!m_source || !m_target)
			{
				m_source = null;
				m_target = null;
				return;
			}
			DebugLog.Warn(string.Concat("[DefeatPvPOpponentTutorialStep] SOURCE = ", m_source, "   TARGET = ", m_target));
			if (!m_target.GetModel().IsAlive)
			{
				m_Started = false;
				FinishStep("target_ko", new List<string>());
				return;
			}
			m_source.GetModel().SkillTriggered -= OnActionTaken;
			m_source.GetModel().SkillTriggered += OnActionTaken;
			AddHelpersAndBlockers();
			m_Started = true;
			m_source.StartCoroutine(CheckForTarget());
		}

		private void OnAnythingClicked(ICombatant clickedCombatant)
		{
			RemoveHelpersAndBlockers(false);
		}

		private IEnumerator CheckForTarget()
		{
			while (m_Started && m_target != null)
			{
				yield return new WaitForSeconds(0.5f);
				if (!m_target.GetModel().IsAlive)
				{
					FinishStep("target_defeated", new List<string>());
				}
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			DebugLog.Log("[DefeatPvPOpponentTutorialStep] FINISH Step - " + trigger);
			if (trigger == "target_defeated")
			{
				RemoveHelpersAndBlockers();
			}
			else
			{
				m_TutorialMgr.HideHelp(TutorialStepType.DefeatPvPOpponent.ToString(), true);
			}
			if ((bool)m_source)
			{
				m_source.GetModel().SkillTriggered -= OnActionTaken;
			}
			m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
			m_Started = false;
		}

		private void AddHelpersAndBlockers()
		{
			if ((bool)m_battleMgr && (bool)m_source && (bool)m_target)
			{
				m_TutorialMgr.ShowFromToHelp(m_battleMgr, m_source.transform, m_source.m_AssetController.BodyCenter, m_target.m_AssetController.BodyCenter, TutorialStepType.DefeatPvPOpponent.ToString(), -60f);
			}
		}

		private void RemoveHelpersAndBlockers(bool finish = true)
		{
			m_TutorialMgr.HideHelp(TutorialStepType.DefeatPvPOpponent.ToString(), finish);
			m_TutorialMgr.SetTutorialCameras(false);
		}
	}
}
