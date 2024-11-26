using System.Collections;
using System.Linq;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

public class CharacterControllerBattleGroundPreview : CharacterControllerBattleGroundBase
{
	private int currentTurn;

	private bool m_Focused;

	private void OnTapReleased()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideCharacterOverlay();
	}

	private void OnTapEnd()
	{
	}

	private void OnTapBegin()
	{
		DebugLog.Log("OnTapBegin");
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowCharacterOverlay(m_AssetController.BodyCenter, m_Model, false, m_BattleMgr.Model.IsPvP);
		RegisterShowToolTip();
	}

	public override IEnumerator DoTurn(int turn)
	{
		if (m_Model.CombatantNameId.Contains("instructor"))
		{
			yield break;
		}
		currentTurn = turn;
		if (currentTurn > m_Model.GetSkills().Count)
		{
			currentTurn = 1;
		}
		foreach (ICombatant combatant in m_BattleMgr.Model.m_CombatantsByInitiative)
		{
			combatant.CombatantView.RemoveAllAppliedEffects();
		}
		if (!m_Focused)
		{
			yield return new WaitForSeconds(PlayGoToFocusPosition());
		}
		m_Focused = true;
		DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(turn, EffectTriggerType.OnDealDamagePerTurn, m_Model, null);
		FocusInitiaive();
		m_Model.RaiseTurnStarted(turn);
		m_Model.UsedConsumable = false;
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForPigToChooseTargetAndDoSkillInSec);
		m_SkillToDo = m_Model.GetSkills()[currentTurn - 1];
		if (m_SkillToDo != null)
		{
			switch (m_SkillToDo.Model.Balancing.TargetType)
			{
			case SkillTargetTypes.Attack:
				m_Model.AttackTarget = m_BattleMgr.Model.m_CombatantsByInitiative.FirstOrDefault((ICombatant c) => c.CombatantFaction != m_Model.CombatantFaction);
				break;
			case SkillTargetTypes.Support:
				m_Model.AttackTarget = m_BattleMgr.Model.m_CombatantsByInitiative.FirstOrDefault((ICombatant c) => c.CombatantFaction == m_Model.CombatantFaction && c != m_Model);
				break;
			}
		}
		if (m_SkillToDo != null)
		{
			if ((bool)m_AssetController)
			{
				m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(m_SkillToDo.BoneAnimationUserTrigger);
				m_AssetController.m_BoneAnimation.RegisterUserTriggerDelegate(m_SkillToDo.BoneAnimationUserTrigger);
			}
			yield return StartCoroutine(m_SkillToDo.DoAction(m_BattleMgr.Model, m_Model, m_Model.AttackTarget));
			if (base.targetSheltered != null)
			{
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeAfterSheltering);
				base.targetSheltered.CombatantView.PlayGoToBasePosition();
				m_Model.AttackTarget.CombatantView.PlayGoToBasePosition();
				base.targetSheltered = null;
			}
			if ((bool)m_AssetController)
			{
				m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(m_SkillToDo.BoneAnimationUserTrigger);
			}
			m_SkillToDo = null;
		}
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForShowDamageAndReturnToBasePosInSec);
		yield return new WaitForSeconds(PlayGoToFocusPosition());
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().PreviewWaitTimeBetweenSkills);
	}

	private void OnDestroy()
	{
		DeregisterEventHandler();
		foreach (GameObject value in LastingVisualEffects.Values)
		{
			if ((bool)value)
			{
				Object.Destroy(value);
			}
		}
		if (m_GoalMarkerBubble != null)
		{
			m_GoalMarkerBubble.transform.parent = null;
			m_GoalMarkerBubble.Hide();
		}
		if ((bool)m_AssetController && m_SkillToDo != null && (bool)m_AssetController.m_BoneAnimation)
		{
			m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(m_SkillToDo.BoneAnimationUserTrigger);
		}
		if ((bool)m_AssetController && (bool)DIContainerInfrastructure.GetCharacterAssetProvider(false))
		{
			DIContainerInfrastructure.GetCharacterAssetProvider(false).DestroyObject(m_Model.CombatantAssetId, m_AssetController.gameObject);
		}
		m_Model.CharacterModel.LevelChanged -= LevelChanged;
	}

	protected override void FocusInitiaive()
	{
	}

	protected override void OnCombatantClicked(ICombatant sender)
	{
	}

	public override void UpdateInitiative()
	{
	}
}
