using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using SmoothMoves;
using UnityEngine;

public class CharacterControllerBattleGround : CharacterControllerBattleGroundBase
{
	[SerializeField]
	private CharacterHealthBar m_HealthBarPrefab;

	[SerializeField]
	private CharacterHealthBar m_BossHealthBarPrefab;

	private CharacterHealthBar m_HealthBar;

	private bool m_ClickedThisTurn;

	public bool m_IsTapping;

	private GlowController m_CurrentGlow;

	[SerializeField]
	private CharacterControlHUD m_ControlHUDPrefab;

	private CharacterControlHUD m_CurrentControlHUD;

	private Camera m_sceneryCamera;

	private bool m_CachedTargetInvalid;

	private bool m_rageUsed;

	private bool m_autoBattleTurn;

	private Transform m_cachedOverCharacterTransform;

	private CharacterControllerBattleGroundBase m_cachedOverCharacter;

	private bool m_NoSelfSelection;

	public override CharacterControlHUD GetControlHUD()
	{
		return m_CurrentControlHUD;
	}

	private void OnTapReleased()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideCharacterOverlay();
		m_IsTapping = false;
		m_BattleMgr.LockDragVisualizationByCode = false;
	}

	private void OnTapEnd()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideCharacterOverlay();
		m_IsTapping = false;
		m_BattleMgr.LockDragVisualizationByCode = false;
	}

	private void OnTapBegin()
	{
		DebugLog.Log("OnTapBegin");
		if (GetModel().IsKnockedOut && !m_BattleMgr.Model.IsPvP)
		{
			DebugLog.Log(GetType(), "OnTapBegin: NOT showing character info overlay because combatant is knocked out");
			return;
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowCharacterOverlay(m_AssetController.BodyCenter, m_Model, false, m_BattleMgr.Model.IsPvP);
		RegisterShowToolTip();
		m_BattleMgr.LockDragVisualizationByCode = true;
		m_IsTapping = true;
	}

	public void UserTrigger(UserTriggerEvent triggerEvent)
	{
	}

	private void OnDestroy()
	{
		DeregisterEventHandler();
		if ((bool)m_CurrentControlHUD)
		{
			UnityEngine.Object.Destroy(m_CurrentControlHUD.gameObject);
		}
		for (int i = 0; i < LastingVisualEffects.Count; i++)
		{
			GameObject gameObject = LastingVisualEffects.Values.ElementAt(i);
			if ((bool)gameObject)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		if (m_GoalMarkerBubble != null)
		{
			m_GoalMarkerBubble.transform.parent = null;
			m_GoalMarkerBubble.Hide();
		}
		if ((bool)m_AssetController && (bool)DIContainerInfrastructure.GetCharacterAssetProvider(false))
		{
			DIContainerInfrastructure.GetCharacterAssetProvider(false).DestroyObject(m_Model.CombatantAssetId, m_AssetController.gameObject);
		}
		m_Model.CharacterModel.LevelChanged -= LevelChanged;
		if (m_IsTapping)
		{
			OnTapReleased();
			if ((bool)m_TapHoldTrigger)
			{
				m_TapHoldTrigger.ResetUICamera();
			}
		}
	}

	public override void RecreateAssetController()
	{
		m_sceneryCamera = Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("SceneryCamera"));
		if (m_Model != null && m_AssetController == null)
		{
			m_AssetController = null;
			if (m_Model.IsBanner)
			{
				m_AssetController = DIContainerInfrastructure.GetBannerAssetProvider().InstantiateObject(m_Model.CombatantAssetId, base.transform, Vector3.zero, Quaternion.identity, !m_Model.IsBanner).GetComponent<CharacterAssetController>();
			}
			else
			{
				m_AssetController = DIContainerInfrastructure.GetCharacterAssetProvider(false).InstantiateObject(m_Model.CombatantAssetId, base.transform, Vector3.zero, Quaternion.identity, !m_Model.IsBanner).GetComponent<CharacterAssetController>();
			}
			m_AssetController.SetModel(m_Model.CharacterModel, false);
			BossAssetController bossAssetController = m_AssetController as BossAssetController;
			if (bossAssetController != null)
			{
				bossAssetController.SetController(this);
			}
			TentacleAssetController tentacleAssetController = m_AssetController as TentacleAssetController;
			if (tentacleAssetController != null)
			{
				tentacleAssetController.SetController(this);
			}
			Vector3 vector = Vector3.one;
			if (AssetIsOnWrongSide(m_AssetController.m_AssetFaction, m_Model.CombatantFaction))
			{
				vector = new Vector3(-1f, 1f, 1f);
			}
			base.transform.localScale = new Vector3(vector.x * m_Model.CharacterModel.Scale, vector.y * m_Model.CharacterModel.Scale, 1f);
			ReSizeCollider();
		}
	}

	private void OnEnable()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayIdleAnimationQueued();
		}
	}

	public override void UpdateHealthBar()
	{
		m_HealthBar.UpdateHealth();
	}

	public override void SpawnHealthBar()
	{
		GetComponent<Collider>().enabled = true;
		if (m_WasKnockedOut)
		{
			m_WasKnockedOut = false;
		}
		if (!m_HealthBar)
		{
			Vector3 vector = Vector3.Scale(m_AssetController.HealthBarPosition, m_AssetController.transform.localScale);
			if (GetModel() is BossCombatant)
			{
				m_HealthBar = UnityEngine.Object.Instantiate(m_BossHealthBarPrefab);
			}
			else
			{
				m_HealthBar = UnityEngine.Object.Instantiate(m_HealthBarPrefab);
			}
			m_HealthBar.gameObject.SetActive(true);
			m_HealthBar.transform.parent = base.transform;
			m_HealthBar.transform.localPosition = new Vector3(vector.x, vector.y, m_AssetController.HealthBarPosition.z);
			m_HealthBar.SetModel(m_Model);
		}
	}

	protected override void RegisterEventHandler()
	{
		base.RegisterEventHandler();
		if ((bool)m_AssetController)
		{
			m_AssetController.m_BoneAnimation.RegisterUserTriggerDelegate(UserTrigger);
		}
		m_TapHoldTrigger.OnTapBegin += OnTapBegin;
		m_TapHoldTrigger.OnTapEnd += OnTapEnd;
		m_TapHoldTrigger.OnTapReleased += OnTapReleased;
	}

	protected override void m_Model_KnockedOut()
	{
		base.m_Model_KnockedOut();
		m_WasKnockedOut = true;
		if (m_Model.CombatantFaction == Faction.Birds && !m_BattleMgr.Model.IsPvP && m_IsTapping)
		{
			OnTapReleased();
			if ((bool)m_TapHoldTrigger)
			{
				m_TapHoldTrigger.ResetUICamera();
			}
		}
		m_BattleMgr.OnCombatantKnockedOut(m_Model);
		ActivateControlHUD(false);
	}

	protected override void m_Model_Defeated()
	{
		base.m_Model_Defeated();
		m_WasKnockedOut = true;
		ActivateControlHUD(false);
	}

	protected override void LevelChanged(int oldLevel, int newLevel)
	{
		m_BattleMgr.m_blocked = true;
		StartCoroutine(SpawnLevelUpEffect());
	}

	public override void RefreshFromStun()
	{
		base.RefreshFromStun();
		if (m_BattleMgr.m_BirdTurnStarted)
		{
			m_BoxCollider.enabled = true;
			if (!m_CommandGiven)
			{
				StartCoroutine(m_BattleMgr.StartCombatantTurnImmeadiatly(m_Model));
			}
		}
	}

	private IEnumerator SpawnLevelUpEffect()
	{
		while (!DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.IsPlayingLevelUp())
		{
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().LevelUpDelayOnCharacters);
		VisualEffectSetting setting = null;
		if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting("LevelUp", out setting))
		{
			DebugLog.Log("Try to instantiate Leveled Up effect");
			GameObject effect = DIContainerInfrastructure.GetBattleEffectAssetProvider().InstantiateObject(setting.VisualEffects[0].PrefabName, m_AssetController.BodyRoot, m_AssetController.BodyRoot.position, Quaternion.identity);
			effect.transform.localPosition = Vector3.zero;
			effect.transform.localScale = Vector3.one;
			UnityEngine.Object.Destroy(effect, effect.GetComponent<Animation>()["LevelUpFX"].clip.length);
		}
		m_Model.RaiseBuffsChanged();
		m_BattleMgr.m_blocked = false;
	}

	public override void DeregisterEventHandler()
	{
		base.DeregisterEventHandler();
		if ((bool)m_AssetController)
		{
			m_AssetController.m_BoneAnimation.RegisterUserTriggerDelegate(UserTrigger);
		}
		if ((bool)m_TapHoldTrigger)
		{
			m_TapHoldTrigger.OnTapBegin -= OnTapBegin;
			m_TapHoldTrigger.OnTapEnd -= OnTapEnd;
			m_TapHoldTrigger.OnTapReleased -= OnTapReleased;
		}
	}

	public override IEnumerator DoTurn(int turn)
	{
		m_ReadyToDestroy = false;
		base.m_CounterAttack = false;
		m_stunnedDuringAttack = false;
		m_CounterModel = null;
		if (m_BoxCollider != null)
		{
			if (m_Model.IsStunned)
			{
				m_BoxCollider.enabled = false;
			}
			else
			{
				m_BoxCollider.enabled = true;
			}
		}
		CheckForTauntTarget();
		if (m_Model is BirdCombatant && m_BattleMgr.AutoBattle && m_BattleMgr.Model.IsRageFull(Faction.Birds) && !m_BattleMgr.Model.m_CombatantsPerFaction[Faction.Birds].Any((ICombatant b) => b is BirdCombatant && (b as BirdCombatant).UseRage) && m_BattleMgr is BattleMgr)
		{
			(m_BattleMgr as BattleMgr).AutoBattleSelectRageSkillWithFallback();
		}
		if (m_Model is BirdCombatant && m_BattleMgr.AutoBattle && m_BattleMgr.Model.IsRageFull(Faction.Birds) && (m_Model as BirdCombatant).UseRage && (m_BattleMgr as BattleMgr).AutoBattleDoRage())
		{
			if (m_BattleMgr.Model.m_CombatantsPerFaction[Faction.Pigs].Any((ICombatant p) => p.CurrentHealth != 0f))
			{
				m_SkillToDo = m_Model.GetSkill(2);
			}
		}
		else if (m_Model.ChachedSkill != null)
		{
			m_SkillToDo = m_Model.ChachedSkill;
			if (m_Model.AttackTarget != null && !m_Model.AttackTarget.IsParticipating)
			{
				SafeEvaluateTarget();
			}
			DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.OnTarget, m_Model, m_Model.AttackTarget);
		}
		else
		{
			m_SkillToDo = DIContainerLogic.GetBattleService().GetNextSkill(m_BattleMgr.Model, m_Model);
			SafeEvaluateTarget();
			DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.OnTarget, m_Model, m_Model.AttackTarget);
			if (m_SkillToDo != null && m_SkillToDo.EvaluateCharge(m_BattleMgr.Model, m_Model, new List<ICombatant> { m_Model.AttackTarget }, null))
			{
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForPigToChooseTargetAndDoSkillInSec);
			}
		}
		if (!m_BattleMgr.Model.IsRageFull(Faction.Birds) && m_Model.CombatantFaction == Faction.Birds && m_Model.GetSkill(2) == m_SkillToDo)
		{
			m_SkillToDo = null;
			(m_Model as BirdCombatant).UseRage = false;
		}
		if (((m_Model is BirdCombatant && m_BattleMgr.AutoBattle) || (m_Model is PigCombatant && m_BattleMgr.Model.IsPvP)) && m_Model.GetSkill(2) == m_SkillToDo)
		{
			if (m_BattleMgr.Model.IsRageFull(Faction.Birds))
			{
				ActivateRageSkill();
				m_rageUsed = true;
			}
		}
		else if (m_SkillToDo == null || m_Model.AttackTarget == null)
		{
			SafeEvaluateTarget();
		}
		if (!m_SkillToDo.Model.SkillParameters.ContainsKey("tentacle_grab"))
		{
			yield return new WaitForSeconds(PlayGoToFocusPosition());
		}
		if (m_Model is PigCombatant || m_Model is BossCombatant || m_BattleMgr.AutoBattle)
		{
			yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForPigToChooseTargetAndDoSkillInSec);
		}
		FocusInitiaive();
		DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(turn, EffectTriggerType.OnCharge, m_Model, null);
		List<BattleEffectGameData> chargeEffects = m_Model.CurrrentEffects.Values.Where((BattleEffectGameData e) => e.m_Effects.Any((BattleEffect ef) => ef.EffectTrigger == EffectTriggerType.OnCharge)).ToList();
		if (m_Model is PigCombatant && chargeEffects.Count > 0)
		{
			PlayAffectedAnimation();
			for (int j = chargeEffects.Count - 1; j >= 0; j--)
			{
				if ((m_Model as PigCombatant).ChargeDone)
				{
					BattleEffectGameData effectGameData = chargeEffects[j];
					effectGameData.IncrementCurrentTurnManual(false);
				}
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForFocusInituativeAndDOTS);
			}
		}
		else if (m_Model is BossCombatant && chargeEffects.Count > 0)
		{
			PlayAffectedAnimation();
			for (int i = chargeEffects.Count - 1; i >= 0; i--)
			{
				if ((m_Model as BossCombatant).ChargeDone)
				{
					BattleEffectGameData effectGameData2 = chargeEffects[i];
					effectGameData2.IncrementCurrentTurnManual(false);
				}
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForFocusInituativeAndDOTS);
			}
		}
		if (!m_Model.IsParticipating)
		{
			m_ReadyToDestroy = true;
			yield return new WaitForSeconds((!(m_Model.CombatantView != null)) ? DIContainerLogic.GetPacingBalancing().TimeForPigToChooseTargetAndDoSkillInSec : m_Model.CombatantView.m_AssetController.GetDefeatAnimationLength());
			yield break;
		}
		m_Model.UsedConsumable = false;
		if (m_Model.CombatantFaction == Faction.Birds)
		{
			DebugLog.Log("Register for input: " + base.gameObject.name);
			RegisterForInput();
			if (!m_Model.IsBanner)
			{
				DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("bird_turn_started", m_Model.CharacterModel.Name);
			}
		}
		bool autoBattle = true;
		if (m_Model is BirdCombatant && !m_BattleMgr.AutoBattle)
		{
			DebugLog.Log("Waiting for input: " + base.gameObject.name);
			ActivateControlHUD(true);
			while (m_IsWaitingForInput && m_Model.IsParticipating && m_Model.StartedHisTurn && !m_BattleMgr.AutoBattle)
			{
				yield return new WaitForEndOfFrame();
				if (m_stunnedDuringAttack)
				{
					yield break;
				}
			}
			m_autoBattleTurn = m_BattleMgr.AutoBattle;
			if (!m_BattleMgr.AutoBattle)
			{
				autoBattle = false;
				if (!m_Model.StartedHisTurn)
				{
					DebugLog.Log("Breaked Turn!");
					yield break;
				}
				ActivateControlHUD(false);
				DeregisterFromInput();
				DebugLog.Log("Input processed: " + base.gameObject.name);
				if (!m_Model.IsParticipating)
				{
					m_ReadyToDestroy = true;
					yield break;
				}
				yield return StartCoroutine(m_BattleMgr.QueueOrExecuteCharacterAction(m_Model.CombatantNameId, DoTurnAfterInput(), false));
			}
			else
			{
				DebugLog.Log("End Wait for Input Auto Battle");
			}
		}
		m_autoBattleTurn = m_BattleMgr.AutoBattle;
		if (!(m_Model is BossCombatant) && !(m_Model is PigCombatant) && !autoBattle)
		{
			yield break;
		}
		if (m_Model.ChachedSkill != null)
		{
			m_SkillToDo = m_Model.ChachedSkill;
		}
		if (m_SkillToDo == null || m_Model.AttackTarget == null)
		{
			SafeEvaluateTarget();
			if (m_Model.AttackTarget == null)
			{
				DebugLog.Log("m_Model.AttackTarget == null");
				yield break;
			}
		}
		if (m_Model is PigCombatant || m_Model is BossCombatant)
		{
			yield return StartCoroutine(DoTurnAfterInput());
		}
		else
		{
			yield return StartCoroutine(m_BattleMgr.QueueOrExecuteCharacterAction(m_Model.CombatantNameId, DoTurnAfterInput(), true));
		}
	}

	private void SafeEvaluateTarget()
	{
		if (m_SkillToDo == null)
		{
			EvaluateFallbackSkillsAndTargets();
		}
		else if (!DIContainerLogic.GetBattleService().GetNextTarget(m_BattleMgr.Model, m_Model, m_SkillToDo.Model, m_Model.CombatantFaction))
		{
			EvaluateFallbackSkillsAndTargets();
		}
	}

	private void EvaluateFallbackSkillsAndTargets()
	{
		m_SkillToDo = DIContainerLogic.GetBattleService().GetFallbackSkill(m_Model);
		if (m_SkillToDo != null && m_Model != null && !DIContainerLogic.GetBattleService().GetNextTarget(m_BattleMgr.Model, m_Model, m_SkillToDo.Model, m_Model.CombatantFaction))
		{
			m_SkillToDo = DIContainerLogic.GetBattleService().GetSupportSkill(m_Model);
			if (m_SkillToDo != null && !DIContainerLogic.GetBattleService().GetNextTarget(m_BattleMgr.Model, m_Model, m_SkillToDo.Model, m_Model.CombatantFaction))
			{
				DIContainerLogic.GetBattleService().GetRandomTarget(m_BattleMgr.Model, m_Model, m_SkillToDo.Model, m_Model.CombatantFaction);
			}
		}
	}

	private IEnumerator DoTurnAfterInput()
	{
		if (m_Model.GetSkill(2) != m_SkillToDo)
		{
			m_rageUsed = false;
		}
		else
		{
			m_rageUsed = true;
		}
		if (m_Model is BirdCombatant && (m_Model as BirdCombatant).UseRage)
		{
			(m_Model as BirdCombatant).UseRage = false;
		}
		if ((!m_BattleMgr.AutoBattle && m_autoBattleTurn) || m_BattleMgr.IsBattleEnded() || (m_Model is BirdCombatant && (m_Model.AttackTarget == null || !m_Model.AttackTarget.IsParticipating)))
		{
			m_autoBattleTurn = m_BattleMgr.AutoBattle;
			yield return new WaitForEndOfFrame();
			if (m_BattleMgr.AutoBattle && !m_BattleMgr.IsBattleEnded())
			{
				DebugLog.Log("[CharacterControllerBattleGround] Auto Battle: DoTurnAfterInput m_Model.AttackTarget dead");
				SafeEvaluateTarget();
				if (m_Model.AttackTarget == null || !m_Model.AttackTarget.IsParticipating)
				{
					DebugLog.Log("[CharacterControllerBattleGround] Auto Battle: DoTurnAfterInput m_Model.AttackTarget still dead");
					yield break;
				}
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.OnTarget, m_Model, m_Model.AttackTarget);
			}
			else
			{
				DebugLog.Log("Waiting for input: " + base.gameObject.name);
				m_IsWaitingForInput = true;
				if (!m_BattleMgr.IsBattleEnded() && !m_BattleMgr.AutoBattle)
				{
					m_BattleMgr.IsConsumableUsePossible = true;
				}
				if (m_BattleMgr.actingCharacters.Count > 0 && m_BattleMgr.actingCharacters.Peek() == m_Model.CombatantNameId)
				{
					m_BattleMgr.actingCharacters.Dequeue();
				}
				ActivateControlHUD(true);
				while (m_IsWaitingForInput && m_Model.IsParticipating && (!m_BattleMgr.AutoBattle || m_BattleMgr.IsBattleEnded()))
				{
					yield return new WaitForEndOfFrame();
				}
				ActivateControlHUD(false);
				DeregisterFromInput();
				if (m_BattleMgr.AutoBattle)
				{
					m_autoBattleTurn = m_BattleMgr.AutoBattle;
					SafeEvaluateTarget();
					DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.OnTarget, m_Model, m_Model.AttackTarget);
				}
				DebugLog.Log("Input processed: " + base.gameObject.name);
			}
			if (!m_Model.IsParticipating)
			{
				m_ReadyToDestroy = true;
				DIContainerLogic.GetBattleService().RemoveCombatantFromBattle(m_BattleMgr.Model, m_Model);
				yield break;
			}
			if (m_Model.AttackTarget == null)
			{
				yield break;
			}
			yield return StartCoroutine(m_BattleMgr.QueueOrExecuteCharacterAction(m_Model.CombatantNameId, DoTurnAfterInput(), m_BattleMgr.AutoBattle));
		}
		if (m_SkillToDo != null)
		{
			if (m_BattleMgr.Model.IsPvP && GetModel().CombatantFaction == Faction.Pigs)
			{
				BirdCommand aiTurn = DIContainerLogic.GetBattleService().m_PvpIntelligence.GetCommand(GetModel());
				if (aiTurn != null && aiTurn.m_Target != null && aiTurn.m_Target.IsAlive)
				{
					DebugLog.Log("Overwriting skillusage by Pvp AI: Target: " + aiTurn.m_Target.CombatantNameId + "  skill: " + aiTurn.m_UsedSkill.Model.Balancing.NameId);
					m_Model.AttackTarget = aiTurn.m_Target;
					m_SkillToDo = aiTurn.m_UsedSkill;
				}
			}
			if ((bool)m_AssetController)
			{
				m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(m_SkillToDo.BoneAnimationUserTrigger);
				m_AssetController.m_BoneAnimation.RegisterUserTriggerDelegate(m_SkillToDo.BoneAnimationUserTrigger);
			}
			if (m_Model.CombatantFaction == m_Model.AttackTarget.CombatantFaction)
			{
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.OnSupportSkillReceived, m_Model.AttackTarget, m_Model);
			}
			bool lightningBird = GetModel().CombatantClass != null && GetModel().CombatantClass.BalancingData.NameId.Contains("lightningbird");
			bool skulker = GetModel().CombatantClass != null && GetModel().CombatantClass.BalancingData.NameId.Contains("skulker");
			ICombatant cachedTarget = null;
			if (lightningBird || skulker)
			{
				cachedTarget = m_Model.AttackTarget.AttackTarget;
			}
			if (m_Model.CombatantFaction != m_Model.AttackTarget.CombatantFaction)
			{
				DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.BeforeAttack, m_Model, m_Model.AttackTarget);
			}
			if (m_BattleMgr.m_IllusionistCombatant != null && m_BattleMgr.m_IllusionistCombatant == m_Model && m_SkillToDo == m_Model.GetSkill(0))
			{
				AttackSkillTemplate attack = m_Model.GetSkill(0) as AttackSkillTemplate;
				if (attack != null && attack.IsMeeleAttack(m_Model))
				{
					StartCoroutine(m_SkillToDo.DoAction(m_BattleMgr.Model, m_Model, m_Model.AttackTarget));
					yield return new WaitForSeconds(0.25f);
					yield return StartCoroutine(MirrorAttackMelee());
				}
				else if (attack != null)
				{
					StartCoroutine(m_SkillToDo.DoAction(m_BattleMgr.Model, m_Model, m_Model.AttackTarget));
					yield return new WaitForSeconds(0.25f);
					yield return StartCoroutine(MirrorAttackRanged());
				}
				else
				{
					DebugLog.Error("[Mirror Image] First Skill is not attack an attack skill!");
					yield return StartCoroutine(m_SkillToDo.DoAction(m_BattleMgr.Model, m_Model, m_Model.AttackTarget));
				}
				m_BattleMgr.m_IllusionistCombatant = null;
			}
			else
			{
				if (m_Model.CombatantFaction == m_Model.AttackTarget.CombatantFaction && m_SkillToDo.Model.Balancing.TargetType == SkillTargetTypes.Support && !m_SkillToDo.m_IsRageSkill)
				{
					DIContainerLogic.GetPvpObjectivesService().SupportSkillUsed(m_Model.AttackTarget);
				}
				yield return StartCoroutine(m_SkillToDo.DoAction(m_BattleMgr.Model, m_Model, m_Model.AttackTarget));
			}
			if (m_SkillToDo.m_IsRageSkill)
			{
				foreach (ICombatant enemy in m_BattleMgr.Model.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != m_Model.CombatantFaction))
				{
					DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(1f, EffectTriggerType.AfterEnemyRageUsed, enemy, m_Model);
				}
				foreach (ICombatant ally in m_BattleMgr.Model.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == m_Model.CombatantFaction))
				{
					DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(1f, EffectTriggerType.AfterOwnRageUsed, ally, m_Model);
				}
			}
			if (base.targetSheltered != null)
			{
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeAfterSheltering);
				base.targetSheltered.CombatantView.PlayGoToBasePosition();
				m_Model.AttackTarget.CombatantView.PlayGoToBasePosition();
				base.targetSheltered = null;
			}
			if ((lightningBird || skulker) && m_Model.AttackTarget.CombatantView.targetSheltered != null && !m_Model.AttackTarget.GetSkill(0).Model.SkillParameters.ContainsKey("all"))
			{
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeAfterSheltering);
				m_Model.AttackTarget.CombatantView.targetSheltered.CombatantView.PlayGoToBasePosition();
				m_Model.AttackTarget.AttackTarget.CombatantView.PlayGoToBasePosition();
				m_Model.AttackTarget.CombatantView.targetSheltered = null;
				m_Model.AttackTarget.AttackTarget = cachedTarget;
			}
			if ((bool)m_AssetController)
			{
				if (m_SkillToDo != null)
				{
					m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(m_SkillToDo.BoneAnimationUserTrigger);
				}
				else
				{
					DebugLog.Error("m_SkillToDo == null");
				}
			}
			m_SkillToDo = null;
		}
		DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.AfterSkillUse, m_Model, m_Model.AttackTarget);
		if (m_rageUsed && m_Model is BirdCombatant)
		{
			StartCoroutine((m_Model as BirdCombatant).CombatantView.m_BattleMgr.m_BattleUI.m_RageMeter.ResetRageMeterAfterUse());
		}
		m_rageUsed = false;
		DebugLog.Log("Character finished his turn: " + m_Model.CombatantNameId);
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForShowDamageAndReturnToBasePosInSec);
		if (!m_Model.IsParticipating)
		{
			m_ReadyToDestroy = true;
			DIContainerLogic.GetBattleService().RemoveCombatantFromBattle(m_BattleMgr.Model, m_Model);
			yield break;
		}
		if (m_Model is BirdCombatant && (m_BattleMgr.CanCharactersAct(1, m_Model.CombatantNameId) || base.m_CounterAttack))
		{
			yield return new WaitForSeconds(PlayGoToFocusPosition());
		}
		else if (m_BattleMgr.Model.m_CombatantsOutOfInitiativeOrder.Count > 0 && m_BattleMgr.Model.m_CombatantsOutOfInitiativeOrder.FirstOrDefault() == m_Model)
		{
			yield return new WaitForSeconds(PlayGoToBasePosition());
		}
		else
		{
			yield return new WaitForSeconds(PlayGoToBasePosition() * DIContainerLogic.GetPacingBalancing().WaitFactorForReturnToBasePos);
		}
		yield return new WaitForEndOfFrame();
	}

	private IEnumerator MirrorAttackRanged()
	{
		ICombatant attackTarget = m_Model.AttackTarget;
		yield return new WaitForSeconds(0.5f);
		SkillBattleDataBase skill = m_Model.GetSkills().FirstOrDefault((SkillBattleDataBase s) => s.Model.Balancing.TargetType == SkillTargetTypes.Attack);
		float copyOffset = Mathf.Sign(m_Model.CombatantView.transform.position.x) * 100f;
		if (m_Model.CombatantFaction == Faction.Pigs)
		{
			copyOffset = 0f - copyOffset;
		}
		m_BattleMgr.m_IllusionistCopy = DIContainerInfrastructure.GetCharacterAssetProvider(false).InstantiateObject(m_Model.CombatantAssetId, null, Vector3.zero, Quaternion.identity).GetComponent<CharacterAssetController>();
		m_BattleMgr.m_IllusionistCopy.transform.position = new Vector3(m_Model.CombatantView.transform.position.x - copyOffset, m_Model.CombatantView.transform.position.y, m_Model.CombatantView.transform.position.z);
		m_BattleMgr.m_IllusionistCopy.transform.parent = m_Model.CombatantView.transform.parent;
		m_BattleMgr.m_IllusionistCopy.transform.rotation = m_Model.CombatantView.transform.rotation;
		m_BattleMgr.m_CurrentOriginal = m_Model;
		m_BattleMgr.m_IllusionistCopy.SetModel(m_Model.CharacterModel, false);
		m_BattleMgr.m_IllusionistCopy.m_IsIllusion = true;
		switch (m_Model.CharacterModel.AssetName)
		{
		default:
			if (m_Model.CombatantFaction != Faction.Pigs)
			{
				break;
			}
			goto case "PrincePorky";
		case "PrincePorky":
		case "MerchantPig":
		case "PigPirate":
			m_BattleMgr.m_IllusionistCopy.GetComponent<ScaleController>().m_BaseScale = new Vector3(-1f, 1f, 1f);
			break;
		}
		yield return new WaitForSeconds(0.5f);
		m_Model.Defeated -= skillTarget_Defeated;
		m_Model.Defeated += skillTarget_Defeated;
		float cachedModifier = m_Model.DamageModifier;
		m_Model.DamageModifier = m_BattleMgr.m_IllusionistDamageFactor / 100f;
		CharacterAssetController originalAsset = m_Model.CombatantView.m_AssetController;
		m_Model.CombatantView.m_AssetController = m_BattleMgr.m_IllusionistCopy;
		yield return m_Model.CombatantView.StartCoroutine(skill.DoAction(m_BattleMgr.Model, m_Model, attackTarget, false, true));
		m_Model.CombatantView.m_AssetController = originalAsset;
		m_Model.DamageModifier = cachedModifier;
		m_Model.Defeated -= skillTarget_Defeated;
		m_BattleMgr.m_IllusionistCopy.PlayAnimation("Move_Once");
		CHMotionTween motion = m_BattleMgr.m_IllusionistCopy.gameObject.AddComponent<CHMotionTween>();
		motion.m_DurationInSeconds = DIContainerLogic.GetPacingBalancing().TimeFromBasePosToFocusPosInSec;
		motion.m_EndTransform = m_Model.CombatantView.transform;
		motion.Play();
		yield return new WaitForSeconds(motion.m_DurationInSeconds);
		UnityEngine.Object.Destroy(m_BattleMgr.m_IllusionistCopy.gameObject);
	}

	private IEnumerator MirrorAttackMelee()
	{
		ICombatant attackTarget = m_Model.AttackTarget;
		SkillBattleDataBase skill = m_Model.GetSkills().FirstOrDefault((SkillBattleDataBase s) => s.Model.Balancing.TargetType == SkillTargetTypes.Attack);
		m_BattleMgr.m_IllusionistCopy = DIContainerInfrastructure.GetCharacterAssetProvider(false).InstantiateObject(m_Model.CombatantAssetId, null, Vector3.zero, Quaternion.identity).GetComponent<CharacterAssetController>();
		m_BattleMgr.m_IllusionistCopy.transform.position = new Vector3(m_Model.CombatantView.transform.position.x, m_Model.CombatantView.transform.position.y, m_Model.CombatantView.transform.position.z);
		m_BattleMgr.m_IllusionistCopy.transform.parent = m_Model.CombatantView.transform.parent;
		m_BattleMgr.m_IllusionistCopy.transform.rotation = m_Model.CombatantView.transform.rotation;
		m_BattleMgr.m_CurrentOriginal = m_Model;
		m_BattleMgr.m_IllusionistCopy.SetModel(m_Model.CharacterModel, false);
		m_BattleMgr.m_IllusionistCopy.m_IsIllusion = true;
		switch (m_Model.CharacterModel.AssetName)
		{
		default:
			if (m_Model.CombatantFaction != Faction.Pigs)
			{
				break;
			}
			goto case "PrincePorky";
		case "PrincePorky":
		case "MerchantPig":
		case "PigPirate":
			m_BattleMgr.m_IllusionistCopy.GetComponent<ScaleController>().m_BaseScale = new Vector3(-1f, 1f, 1f);
			break;
		}
		yield return new WaitForSeconds(0.1f);
		CHMotionTween motion = m_BattleMgr.m_IllusionistCopy.gameObject.AddComponent<CHMotionTween>();
		m_Model.Defeated -= skillTarget_Defeated;
		m_Model.Defeated += skillTarget_Defeated;
		CharacterAssetController originalAsset = m_Model.CombatantView.m_AssetController;
		m_Model.CombatantView.m_AssetController = m_BattleMgr.m_IllusionistCopy;
		yield return new WaitForSeconds(0.5f);
		if (attackTarget == null || attackTarget.CombatantView == null)
		{
			UnityEngine.Object.Destroy(m_BattleMgr.m_IllusionistCopy.gameObject);
			m_Model.CombatantView.m_AssetController = originalAsset;
			m_Model.Defeated -= skillTarget_Defeated;
			yield break;
		}
		Vector3 targetOffset = new Vector3((0f - (m_BattleMgr.m_IllusionistCopy.OffsetFromEnemyX + attackTarget.CombatantView.m_AssetController.OffsetFromEnemyX)) * m_BattleMgr.m_IllusionistCopy.transform.localScale.x, 0f, -15f);
		if (attackTarget.IsBanner)
		{
			targetOffset += new Vector3(targetOffset.x / 1.5f, 0f, 0f);
		}
		m_BattleMgr.m_IllusionistCopy.PlayAttackAnim(false);
		motion.m_StartTransform = m_Model.CombatantView.m_BattleMgr.m_BirdCenterPosition;
		motion.m_StartOffset = m_Model.CombatantView.CachedUnfocusedPos;
		motion.m_EndTransform = attackTarget.CombatantView.transform;
		motion.m_EndOffset = targetOffset;
		float duration = DIContainerLogic.GetPacingBalancing().TimeFromFocusPosToAttackPosInSec;
		motion.m_DurationInSeconds = duration;
		motion.Play();
		float cachedModifier = m_Model.DamageModifier;
		m_Model.DamageModifier = m_BattleMgr.m_IllusionistDamageFactor / 100f;
		yield return m_Model.CombatantView.StartCoroutine(skill.DoAction(m_BattleMgr.Model, m_Model, attackTarget, false, true));
		m_Model.CombatantView.m_AssetController = originalAsset;
		m_Model.DamageModifier = cachedModifier;
		m_Model.Defeated -= skillTarget_Defeated;
		yield return new WaitForSeconds(motion.m_DurationInSeconds);
		UnityEngine.Object.Destroy(m_BattleMgr.m_IllusionistCopy.gameObject);
	}

	private void skillTarget_Defeated()
	{
		if ((bool)m_BattleMgr.m_IllusionistCopy && m_BattleMgr.m_CurrentOriginal != null)
		{
			m_BattleMgr.m_CurrentOriginal.Defeated -= skillTarget_Defeated;
			UnityEngine.Object.Destroy(m_BattleMgr.m_IllusionistCopy.gameObject);
		}
	}

	public override void ActivateControlHUD(bool activate)
	{
		if ((bool)m_CurrentControlHUD)
		{
			if (activate)
			{
				m_CurrentControlHUD.SetCharacter(this);
				m_CurrentControlHUD.transform.position = base.transform.position;
				m_CurrentControlHUD.ResetControlHUD();
			}
			m_CurrentControlHUD.gameObject.SetActive(activate);
		}
	}

	protected override void SetInitialHUD()
	{
		base.SetInitialHUD();
		if (m_CurrentControlHUD == null && m_Model is BirdCombatant)
		{
			m_CurrentControlHUD = UnityEngine.Object.Instantiate(m_ControlHUDPrefab, m_AssetController.BodyRoot.position, Quaternion.identity) as CharacterControlHUD;
			m_CurrentControlHUD.transform.parent = m_BattleMgr.m_BattleArea;
			m_CurrentControlHUD.gameObject.SetActive(m_Model.StartedHisTurn);
			m_CurrentControlHUD.ResetControlHUD();
			CancelInvoke("CheckSelectionMarker");
			InvokeRepeating("CheckSelectionMarker", 0.1f, 0.2f);
		}
	}

	protected override void OnCombatantClicked(ICombatant sender)
	{
		m_ClickedThisTurn = true;
		m_CommandGiven = true;
		m_Model.AttackTarget = sender;
		if (sender.CombatantFaction == m_Model.CombatantFaction)
		{
			DebugLog.Log("User activated support skill, clicked: " + sender.CombatantView.gameObject.name);
			if (m_Model.GetSkill(1) == null)
			{
				DebugLog.Error("No Support Skill!");
				return;
			}
			m_SkillToDo = m_Model.GetSkill(1);
			DebugLog.Log("Character triggered support skill");
			m_IsWaitingForInput = false;
		}
		else
		{
			DebugLog.Log("User activated active skill, clicked: " + sender.CombatantView.gameObject.name);
			if (m_Model.GetSkill(0) == null)
			{
				DebugLog.Error("No Offensive Skill!");
				return;
			}
			m_SkillToDo = m_Model.GetSkill(0);
			DebugLog.Log("Character triggered offensive skill");
			m_IsWaitingForInput = false;
		}
	}

	private void OnTouchDrag()
	{
		if (m_AllowDrag)
		{
			HandleDrag();
		}
	}

	private void CheckSelectionMarker()
	{
		if (GetModel().ActedThisTurn && (bool)m_CurrentControlHUD && m_CurrentControlHUD.gameObject.activeInHierarchy)
		{
			ActivateControlHUD(false);
		}
	}

	private void HandleDrag()
	{
		if (GetModel().CombatantFaction == Faction.Pigs || GetModel().ActedThisTurn || m_BattleMgr.m_LockControlHUDs || m_BattleMgr.AutoBattle || CurrentlyAnimating())
		{
			return;
		}
		if (m_IsTapping && m_CurrentControlHUD != null)
		{
			if ((bool)m_CurrentGlow && m_CurrentGlow.gameObject.activeInHierarchy)
			{
				m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Hide");
				Invoke("DisableGlow", m_CurrentGlow.GetComponent<Animation>()["CharacterSelectionGlow_Hide"].length);
			}
			m_CurrentControlHUD.ResetControlHUD();
			m_cachedOverCharacterTransform = null;
			m_BattleMgr.m_DraggedCharacter = null;
			m_cachedOverCharacter = null;
			return;
		}
		if (m_tauntedTarget != null)
		{
			for (int i = 0; i < m_BattleMgr.Model.m_CombatantsByInitiative.Count; i++)
			{
				ICombatant combatant = m_BattleMgr.Model.m_CombatantsByInitiative[i];
				if (combatant != m_tauntedTarget && combatant.CombatantFaction == Faction.Pigs)
				{
					m_BattleMgr.m_CharacterInteractionBlockedItems[i].SetActive(true);
					m_BattleMgr.m_CharacterInteractionBlockedItems[i].transform.position = combatant.CombatantView.m_AssetController.BodyCenter.position + new Vector3(0f, 0f, -5f);
				}
			}
		}
		for (int j = 0; j < m_BattleMgr.Model.m_CombatantsByInitiative.Count; j++)
		{
			ICombatant combatant2 = m_BattleMgr.Model.m_CombatantsByInitiative[j];
			SkillBattleDataBase skill = ((combatant2.CombatantFaction != m_Model.CombatantFaction) ? m_Model.GetSkills()[0] : m_Model.GetSkills()[1]);
			if (!DIContainerLogic.GetBattleService().isTargetAllowedForSkill(combatant2, skill))
			{
				m_BattleMgr.m_CharacterInteractionBlockedItems[j].SetActive(true);
				m_BattleMgr.m_CharacterInteractionBlockedItems[j].transform.position = combatant2.CombatantView.m_AssetController.BodyCenter.position + new Vector3(0f, 0f, -5f);
			}
		}
		m_BattleMgr.LockDragVisualizationByCode = true;
		m_BattleMgr.m_DraggedCharacter = this;
		SetDragHUD(Input.mousePosition);
	}

	public void SetDragHUD(Vector3 currentScreenPos)
	{
		if (m_CurrentControlHUD == null)
		{
			return;
		}
		if (m_CurrentGlow == null)
		{
			m_CurrentGlow = m_BattleMgr.m_CurrentGlow;
			m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Show");
			m_CurrentGlow.gameObject.SetActive(false);
		}
		Ray ray = m_sceneryCamera.ScreenPointToRay(currentScreenPos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 10000f, (1 << LayerMask.NameToLayer("TutorialScenery")) | (1 << LayerMask.NameToLayer("Scenery"))))
		{
			if (m_cachedOverCharacterTransform != hitInfo.transform)
			{
				m_cachedOverCharacterTransform = hitInfo.transform;
				CharacterControllerBattleGroundBase cachedOverCharacter = m_cachedOverCharacter;
				m_cachedOverCharacter = m_cachedOverCharacterTransform.GetComponent<CharacterControllerBattleGroundBase>();
				if (cachedOverCharacter != m_cachedOverCharacter)
				{
					if (m_cachedOverCharacter != null && (m_cachedOverCharacter.GetModel().IsKnockedOut || m_cachedOverCharacter.gameObject.layer != ((!DIContainerInfrastructure.TutorialMgr.IsCurrentlyLocked) ? LayerMask.NameToLayer("Scenery") : LayerMask.NameToLayer("TutorialScenery"))))
					{
						m_cachedOverCharacter = null;
					}
					if (m_cachedOverCharacter != null)
					{
						SkillBattleDataBase skill = ((m_cachedOverCharacter.GetModel().CombatantFaction != m_Model.CombatantFaction) ? m_Model.GetSkills()[0] : m_Model.GetSkills()[1]);
						m_CachedTargetInvalid = m_cachedOverCharacter != null && !DIContainerLogic.GetBattleService().isTargetAllowedForSkill(m_cachedOverCharacter.GetModel(), skill);
					}
					if (m_cachedOverCharacter == null || m_cachedOverCharacter == this || (m_tauntedTarget != null && m_cachedOverCharacter.GetModel() != m_tauntedTarget && m_tauntedTarget.CombatantFaction == m_cachedOverCharacter.GetModel().CombatantFaction) || m_CachedTargetInvalid)
					{
						m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Hide");
						Invoke("DisableGlow", m_CurrentGlow.GetComponent<Animation>()["CharacterSelectionGlow_Hide"].length);
					}
					else
					{
						m_CurrentGlow.gameObject.SetActive(true);
						CancelInvoke("DisableGlow");
						m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Show");
						m_CurrentGlow.SetStateColor((m_cachedOverCharacter.GetModel().CombatantFaction != m_Model.CombatantFaction) ? GlowState.Attack : GlowState.Support);
					}
				}
			}
			if ((bool)m_cachedOverCharacter && !(m_cachedOverCharacter == this) && (m_tauntedTarget == null || m_cachedOverCharacter.GetModel() == m_tauntedTarget || m_tauntedTarget.CombatantFaction != m_cachedOverCharacter.GetModel().CombatantFaction))
			{
				float num = 1f;
				switch (m_cachedOverCharacter.GetModel().CharacterModel.CharacterSize)
				{
				case CharacterSizeType.Boss:
					num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorBoss;
					break;
				case CharacterSizeType.Large:
					num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorLarge;
					break;
				case CharacterSizeType.Medium:
					num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorMedium;
					break;
				case CharacterSizeType.Small:
					num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorSmall;
					break;
				}
				m_CurrentGlow.transform.localScale = m_cachedOverCharacter.transform.localScale * num / m_cachedOverCharacter.GetModel().CharacterModel.Scale;
				m_CurrentGlow.transform.position = m_cachedOverCharacter.m_AssetController.BodyCenter.position;
			}
			if (m_CachedTargetInvalid || (m_tauntedTarget != null && m_cachedOverCharacter != null && m_cachedOverCharacter.GetModel() != m_tauntedTarget && m_tauntedTarget.CombatantFaction == m_cachedOverCharacter.GetModel().CombatantFaction))
			{
				m_CurrentControlHUD.SetState(base.transform, m_cachedOverCharacter, hitInfo.point, m_BattleMgr, true);
			}
			else
			{
				m_CurrentControlHUD.SetState(base.transform, m_cachedOverCharacter, hitInfo.point, m_BattleMgr);
			}
		}
		else
		{
			m_CurrentGlow.gameObject.SetActive(false);
		}
	}

	public override void DisableGlow()
	{
		if ((bool)m_CurrentGlow)
		{
			m_CurrentGlow.gameObject.SetActive(false);
		}
	}

	private void OnTouchClicked()
	{
		if (m_WasKnockedOut && m_BattleMgr.m_BirdTurnStarted && !m_reviveClicked && !m_BattleMgr.Model.IsPvP && DIContainerConfig.GetClientConfig().EnableSingleBirdRevive)
		{
			m_reviveClicked = true;
			m_BattleMgr.m_BattleUI.ShowSingleRevivePopup(m_Model);
		}
		if (m_AllowClick)
		{
			HandleClicked();
		}
	}

	public void HandleClicked()
	{
		for (int i = 0; i < m_BattleMgr.m_CharacterInteractionBlockedItems.Count; i++)
		{
			m_BattleMgr.m_CharacterInteractionBlockedItems[i].SetActive(false);
		}
		if (!m_IsTapping && !m_BattleMgr.m_LockControlHUDs && !CurrentlyAnimating() && GetModel().CombatantFaction != Faction.Pigs && !GetModel().ActedThisTurn)
		{
			if (m_CurrentGlow == null)
			{
				m_CurrentGlow = m_BattleMgr.m_CurrentGlow;
				m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Show");
				m_CurrentGlow.gameObject.SetActive(false);
			}
			float num = 1f;
			switch (m_Model.CharacterModel.CharacterSize)
			{
			case CharacterSizeType.Boss:
				num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorBoss;
				break;
			case CharacterSizeType.Large:
				num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorLarge;
				break;
			case CharacterSizeType.Medium:
				num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorMedium;
				break;
			case CharacterSizeType.Small:
				num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorSmall;
				break;
			}
			m_CurrentGlow.transform.localScale = base.transform.localScale * num / GetModel().CharacterModel.Scale;
			m_CurrentGlow.transform.position = m_AssetController.BodyCenter.position;
			m_CurrentGlow.gameObject.SetActive(true);
			m_CurrentGlow.SetStateColor(GlowState.Support);
			CancelInvoke("DisableGlow");
			m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Show");
			m_CurrentGlow.GetComponent<Animation>().PlayQueued("CharacterSelectionGlow_Hide");
			Invoke("DisableGlow", m_CurrentGlow.GetComponent<Animation>()["CharacterSelectionGlow_Show"].length + m_CurrentGlow.GetComponent<Animation>()["CharacterSelectionGlow_Hide"].length);
			if (m_AllowClick)
			{
				base.Clicked -= OnCombatantClicked;
				base.Clicked += OnCombatantClicked;
				RegisterClicked();
				base.Clicked -= OnCombatantClicked;
			}
		}
	}

	private bool CurrentlyAnimating()
	{
		return !m_IsWaitingForInput || m_Model.IsAttacking;
	}

	private void OnTouchReleased()
	{
		HandleRelease(Input.mousePosition);
	}

	public void HandleRelease(Vector2 releasePos)
	{
		for (int i = 0; i < m_BattleMgr.m_CharacterInteractionBlockedItems.Count; i++)
		{
			m_BattleMgr.m_CharacterInteractionBlockedItems[i].SetActive(false);
		}
		if (m_IsTapping || (CurrentlyAnimating() && m_BattleMgr.m_DraggedCharacter == null))
		{
			m_NoSelfSelection = false;
			return;
		}
		if (GetModel().CombatantFaction == Faction.Pigs || GetModel().ActedThisTurn)
		{
			m_BattleMgr.m_DraggedCharacter = null;
			m_NoSelfSelection = false;
			return;
		}
		if (!m_BattleMgr.m_DraggedCharacter)
		{
			m_BattleMgr.m_DraggedCharacter = null;
			m_NoSelfSelection = false;
			return;
		}
		if (m_CurrentGlow != null)
		{
			m_CurrentGlow.GetComponent<Animation>().Stop();
			m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Hide");
			Invoke("DisableGlow", m_CurrentGlow.GetComponent<Animation>()["CharacterSelectionGlow_Hide"].length);
		}
		if (m_CurrentControlHUD != null)
		{
			m_CurrentControlHUD.ResetControlHUD();
		}
		m_BattleMgr.LockDragVisualizationByCode = false;
		if (CurrentlyAnimating())
		{
			m_BattleMgr.m_DraggedCharacter = null;
			m_NoSelfSelection = false;
			return;
		}
		Ray ray = m_sceneryCamera.ScreenPointToRay(releasePos);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, 10000f, (!DIContainerInfrastructure.TutorialMgr.IsCurrentlyLocked) ? (1 << LayerMask.NameToLayer("Scenery")) : (1 << LayerMask.NameToLayer("TutorialScenery"))))
		{
			m_BattleMgr.m_DraggedCharacter = null;
			m_NoSelfSelection = false;
			return;
		}
		DebugLog.Log("RayCastHit! " + hitInfo.transform.gameObject.name);
		CharacterControllerBattleGroundBase component = hitInfo.transform.GetComponent<CharacterControllerBattleGroundBase>();
		if (component != null)
		{
			SkillBattleDataBase skill = ((component.GetModel().CombatantFaction != m_Model.CombatantFaction) ? m_Model.GetSkills()[0] : m_Model.GetSkills()[1]);
			if ((m_tauntedTarget != null && component != null && m_tauntedTarget != component.GetModel() && m_tauntedTarget.CombatantFaction == component.GetModel().CombatantFaction) || !DIContainerLogic.GetBattleService().isTargetAllowedForSkill(component.GetModel(), skill))
			{
				return;
			}
		}
		if (!component || !component.GetModel().IsParticipating)
		{
			m_BattleMgr.m_DraggedCharacter = null;
			m_NoSelfSelection = false;
			return;
		}
		if (m_BattleMgr.m_DraggedCharacter.GetModel().CombatantNameId != component.GetModel().CombatantNameId)
		{
			DebugLog.Log("Acting Character: " + m_BattleMgr.m_DraggedCharacter.GetModel().CombatantName);
			DebugLog.Log("Targeted Character: " + component.GetModel().CombatantName);
			component.Clicked -= OnCombatantClicked;
			component.Clicked += OnCombatantClicked;
			component.RegisterClicked();
			component.Clicked -= OnCombatantClicked;
		}
		m_BattleMgr.m_DraggedCharacter = null;
		m_NoSelfSelection = false;
	}

	public override void UpdateInitiative()
	{
		if (m_AssetController != null && m_HealthBar != null)
		{
			m_HealthBar.RefreshInitative();
		}
	}

	public void LateUpdate()
	{
		if ((bool)m_CachedTransform && Math.Abs(m_CachedZ - m_CachedTransform.position.y) > 1f && (bool)m_HealthBar)
		{
			m_HealthBar.transform.localPosition = Vector3.Scale(m_AssetController.HealthBarPosition, new Vector3(m_AssetController.transform.localScale.x, m_AssetController.transform.localScale.y, 1f * m_HealthBar.transform.localScale.z));
		}
	}

	protected override void FocusInitiaive()
	{
		m_HealthBar.Focus();
	}
}
