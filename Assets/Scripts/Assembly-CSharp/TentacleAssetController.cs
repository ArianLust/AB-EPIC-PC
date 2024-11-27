using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using SmoothMoves;
using UnityEngine;

public class TentacleAssetController : CharacterAssetController
{
	private CharacterControllerBattleGroundBase m_controller;

	private bool m_playingDefeat;

	public bool m_IsStunning;

	public override float GetAttackAnimationLength()
	{
		return GetAnimationLength("Action_Attack_1");
	}

	public override void PlayAttackAnim(bool useOffhand)
	{
		CancelInvoke("PlayIdle");
		if (useOffhand)
		{
			Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Action_Attack_2"));
		}
		else
		{
			Invoke("PlayAttackEnd", base.gameObject.PlayAnimationOrAnimatorState("Action_Attack_1"));
		}
	}

	private void PlayAttackEnd()
	{
		Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Action_Attack_1_End"));
	}

	private void PlayIdle()
	{
		if (!m_playingDefeat)
		{
			if (m_IsStunning)
			{
				PlayAnimation("Idle_Grab");
			}
			else
			{
				PlayAnimation("Idle");
			}
		}
	}

	public override void PlayIdleAnimationQueued()
	{
		if (m_IsStunning)
		{
			PlayAnimationQueued("Idle_Grab");
		}
		else
		{
			PlayAnimationQueued("Idle");
		}
	}

	public override void PlayStunnedAnimation()
	{
		StopAnimation("Idle");
		CancelInvoke();
		if (m_IsStunning)
		{
			PlayAnimationQueued("Stunned_Grab");
		}
		else
		{
			PlayAnimationQueued("Stunned");
		}
	}

	protected override void PlayBlinkAnimation()
	{
	}

	public override void PlayDefeatAnim()
	{
		if (!m_playingDefeat)
		{
			m_playingDefeat = true;
			CancelInvoke("PlayIdle");
			base.gameObject.StopAnimationOrAnimatorState(new List<string> { "Idle", "Hit", "Idle_Grab", "Hit_Grab", "Stunned_Grab", "Stunned" }, this);
			PlayAnimation("Defeated");
		}
	}

	public override void PlayHitAnim()
	{
		if (!m_playingDefeat)
		{
			CancelInvoke("PlayIdle");
			if (m_IsStunning)
			{
				Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Hit_Grab"));
			}
			else
			{
				Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Hit"));
			}
		}
	}

	public override void PlayAffectedAnim()
	{
		if (!m_playingDefeat)
		{
			CancelInvoke("PlayIdle");
			if (m_IsStunning)
			{
				Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Affected_Grab"));
			}
			else
			{
				Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Affected"));
			}
		}
	}

	public override void PlayMournAnim()
	{
		CancelInvoke("PlayIdle");
		if (m_IsStunning)
		{
			Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Mourn_Grab"));
		}
		else
		{
			Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Mourn"));
		}
	}

	public override void PlayCheerAnim()
	{
		CancelInvoke("PlayIdle");
		if (m_IsStunning)
		{
			Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Cheer_Grab"));
		}
		else
		{
			Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Cheer"));
		}
	}

	public void PlayMoveAnim()
	{
		CancelInvoke("PlayIdle");
		Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Move_Once"));
	}

	public override void PlayAffectedAnimQueued(string returnAnimation = "Idle")
	{
		CancelInvoke("PlayIdle");
		Invoke("PlayAffectedAnim", base.gameObject.GetAnimationOrAnimatorStateLength("Hit"));
	}

	protected override void SetEquipment(bool isWorldMap, bool showItems)
	{
		m_EquipmentSetOnce = true;
	}

	protected override void SetEquipmentItem(IInventoryItemGameData itemGameData, string boneName, string suffix = "")
	{
		base.SetEquipmentItem(itemGameData, boneName, suffix);
	}

	public override void SetModel(ICharacter model, bool isWorldMap, bool showEquipment = true, bool useScaleController = true, bool isLite = true, bool showItem = true)
	{
		Model = model;
		m_IsWorldMap = isWorldMap;
		base.gameObject.layer = base.transform.parent.gameObject.layer;
		RegisterEventHandler();
		Animator component = GetComponent<Animator>();
		if (component != null)
		{
			component.Rebind();
		}
	}

	public void RegisterTrigger(string trigger)
	{
		UserTriggerEvent userTriggerEvent = new UserTriggerEvent();
		userTriggerEvent.tag = trigger;
		if (m_controller != null && m_controller.m_SkillToDo != null)
		{
			m_controller.m_SkillToDo.BoneAnimationUserTrigger(userTriggerEvent);
		}
		else
		{
			DebugLog.Error("No skill to do for trigger: " + trigger);
		}
	}

	public void SetController(CharacterControllerBattleGroundBase cc)
	{
		m_controller = cc;
	}
}
