using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using SmoothMoves;
using UnityEngine;

public class BossAssetController : CharacterAssetController
{
	[SerializeField]
	public Vector3 m_SpawnOffset;

	[SerializeField]
	public bool m_ImmovableMinions;

	[SerializeField]
	public float m_PassiveAnimationSpawnDelay;

	[SerializeField]
	public Vector3 m_FocusOffset;

	[SerializeField]
	private Animation m_ReviveTimer;

	[SerializeField]
	private UILabel m_TimerLabel;

	[SerializeField]
	public CharacterHealthBar m_HealthBar;

	private CharacterControllerBattleGroundBase m_controller;

	private bool m_playingDefeat;

	private bool m_ReviveTimerRunning;

	private bool m_playingCustom;

	public override float GetAttackAnimationLength()
	{
		return GetAnimationLength("Action_Attack_1");
	}

	public override void PlayAttackAnim(bool useOffhand)
	{
		CancelInvoke("PlayIdle");
		Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Action_Attack_1"));
	}

	public void PlayPassiveAnim()
	{
		CancelInvoke("PlayIdle");
		Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Action_Passive_1"));
	}

	private void PlayIdle()
	{
		if (!m_playingDefeat)
		{
			m_playingCustom = false;
			PlayAnimation("Idle");
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
			base.gameObject.StopAnimationOrAnimatorState(new List<string> { "Idle", "Hit" }, this);
			PlayAnimation("Defeated");
			SpawnBossReviveTimer();
		}
	}

	public override void PlayHitAnim()
	{
		if (!m_playingDefeat)
		{
			m_playingCustom = true;
			CancelInvoke("PlayIdle");
			Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Hit"));
		}
	}

	public override void PlayAffectedAnim()
	{
		if (!m_playingDefeat)
		{
			CancelInvoke("PlayIdle");
			Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Affected"));
		}
	}

	public override void PlaySupportAnim()
	{
		CancelInvoke("PlayIdle");
		Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Action_Support_1"));
	}

	public override void PlayMournAnim()
	{
		CancelInvoke("PlayIdle");
		Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Mourn"));
	}

	public override void PlayCheerAnim()
	{
		CancelInvoke("PlayIdle");
		Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Cheer"));
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

	public void PlayCustomReactionAnimation()
	{
		if (!m_playingDefeat && !m_playingCustom)
		{
			Invoke("PlayIdle", base.gameObject.PlayAnimationOrAnimatorState("Reaction"));
		}
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

	private IEnumerator HandleReviveTimer()
	{
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		if (player.CurrentEventManagerGameData == null || m_ReviveTimerRunning)
		{
			DebugLog.Warn(GetType(), "HandleReviveTimer: No EventManagerGamedata found!");
			yield break;
		}
		m_ReviveTimerRunning = true;
		uint targetTime = player.Data.BossStartTime + (uint)player.CurrentEventManagerGameData.CurrentEventBoss.BalancingData.TimeToReactivate;
		int timeRemaining2 = (int)(targetTime - DIContainerLogic.GetTimingService().GetCurrentTimestamp());
		while (DIContainerLogic.EventSystemService.IsBossOnCooldown() && base.gameObject.activeSelf)
		{
			timeRemaining2 = (int)(targetTime - DIContainerLogic.GetTimingService().GetCurrentTimestamp());
			int minutes = timeRemaining2 % 3600 / 60;
			int seconds = timeRemaining2 % 60;
			string locaIdent = player.CurrentEventManagerGameData.CurrentEventBoss.BalancingData.DefeatedLabelLocaId;
			m_TimerLabel.text = DIContainerInfrastructure.GetLocaService().Tr(locaIdent) + DIContainerLogic.EventSystemService.GetFormattedBossCooldown();
			yield return new WaitForSeconds(1f);
		}
		if (m_IsWorldMap && (bool)m_ReviveTimer)
		{
			yield return new WaitForSeconds(m_ReviveTimer.gameObject.PlayAnimationOrAnimatorState("Hide"));
			EventSystemWorldMapStateMgr eventSystemWorldMapStateMgr = DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr;
			if (eventSystemWorldMapStateMgr != null)
			{
				eventSystemWorldMapStateMgr.StartSpawnBossCoroutine();
			}
		}
		m_ReviveTimerRunning = false;
	}

	public void SpawnBossReviveTimer()
	{
		if (m_IsWorldMap && m_ReviveTimer != null)
		{
			m_ReviveTimer.Play("Show");
			StartCoroutine(HandleReviveTimer());
			if (m_HealthBar != null)
			{
				m_HealthBar.gameObject.PlayAnimationOrAnimatorState("HealthBar_Hide");
			}
		}
	}

	public void SetController(CharacterControllerBattleGroundBase cc)
	{
		m_controller = cc;
	}

	internal void SetDefeatState()
	{
		CancelInvoke("PlayIdle");
		base.gameObject.StopAnimationOrAnimatorState(new List<string> { "Idle", "Hit" }, this);
		PlayAnimation("SetDefeated");
		SpawnBossReviveTimer();
	}
}
