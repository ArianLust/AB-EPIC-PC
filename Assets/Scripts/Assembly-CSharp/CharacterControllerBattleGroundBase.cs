using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using UnityEngine;

public abstract class CharacterControllerBattleGroundBase : MonoBehaviour
{
	[SerializeField]
	private GameObject m_ReviveMePrefab;

	private GameObject m_InstantiatedReviveMe;

	public BattleMgrBase m_BattleMgr;

	protected ICombatant m_Model;

	protected ICombatant m_CounterModel;

	protected ICombatant m_CounterModelTarget;

	public bool m_WasKnockedOut;

	public bool m_AlreadyPlaced;

	public bool m_AllowClick = true;

	public bool m_AllowDrag = true;

	public float m_StartPositionY;

	public bool m_reviveClicked;

	public bool m_CommandGiven;

	public bool m_stunnedDuringAttack;

	public float m_maxSpawnDelay;

	public float m_minSpawnDelay;

	public Dictionary<string, GameObject> LastingVisualEffects = new Dictionary<string, GameObject>();

	public ScrollingCombatText m_ScrollingCombatTextPrefab;

	public ScrollingCombatText m_ScrollingCombatTextWithLabelPrefab;

	public GameObject m_CriticalAdditionalEffect;

	public ScorePopup m_ScorePopup;

	public CharacterSpeechBubble m_CharacterSpeechBubblePrefab;

	public UITapHoldTrigger m_TapHoldTrigger;

	public CHMotionTween m_TweenMovement;

	[HideInInspector]
	public PerkType m_CurrentPerkType;

	[HideInInspector]
	public CharacterAssetController m_AssetController;

	public Dictionary<string, CharacterSpeechBubble> m_SpeechBubbles = new Dictionary<string, CharacterSpeechBubble>();

	public BoxCollider m_BoxCollider;

	public ICombatant m_tauntedTarget;

	[HideInInspector]
	public bool m_IsWaitingForInput;

	[HideInInspector]
	public SkillBattleDataBase m_SkillToDo;

	[HideInInspector]
	public SkillBattleDataBase m_CounterSkillToDo;

	protected float m_CachedZ;

	public Transform m_CachedTransform;

	protected bool m_TooltipTapBegan;

	protected CharacterSpeechBubble m_GoalMarkerBubble;

	protected float m_BeShelteredRetreatValue = -150f;

	protected bool m_ReadyToDestroy;

	protected Vector3 m_StartPosition;

	protected float m_LastHealthUpdate;

	public float OffsetFromEnemyZ = -10f;

	[SerializeField]
	private Vector3 m_BirdFocusPosOffset = new Vector3(300f, 0f, 0f);

	[SerializeField]
	private BattleFXController m_RankUpEffect;

	[HideInInspector]
	public Vector3 m_InitialStartPosOffset = default(Vector3);

	private Vector3 m_CachedFocusPos;

	private Vector3 m_CachedUnfocusedPos;

	private Vector3 m_StartSize;

	public Vector3 Center
	{
		get
		{
			return m_AssetController.ColliderOffset * GetCharacterAssetScale();
		}
	}

	public Vector2 Extents
	{
		get
		{
			return m_AssetController.ColliderSize * GetCharacterAssetScale() / 2f;
		}
	}

	public Vector2 Size
	{
		get
		{
			return m_AssetController.ColliderSize * GetCharacterAssetScale();
		}
	}

	public bool m_CounterAttack { get; set; }

	public ICombatant targetSheltered { get; set; }

	public Vector3 CachedUnfocusedPos
	{
		get
		{
			return m_CachedUnfocusedPos;
		}
		set
		{
			m_CachedFocusPos = value + m_BirdFocusPosOffset;
			m_CachedUnfocusedPos = value;
		}
	}

	[method: MethodImpl(32)]
	public event Action UsedConsumable;

	[method: MethodImpl(32)]
	public event Action<ICombatant> ShowTooltip;

	[method: MethodImpl(32)]
	public event Action<ICombatant> Clicked;

	public virtual CharacterControlHUD GetControlHUD()
	{
		return null;
	}

	public virtual void ActivateControlHUD(bool activate)
	{
	}

	public void SetModel(ICombatant model, BattleMgrBase battleMgr)
	{
		base.gameObject.name = model.CombatantNameId;
		m_Model = model;
		m_BattleMgr = battleMgr;
		m_Model.CombatantView = this;
		m_CachedTransform = base.transform;
		SetCachedUnfocusedPos(base.transform.position);
		RecreateAssetController();
		SetInitialHUD();
		RegisterEventHandler();
		m_StartSize = m_AssetController.gameObject.GetComponent<ScaleController>().m_BaseScale;
	}

	protected virtual void SetInitialHUD()
	{
	}

	public void CheckForTauntTarget()
	{
		m_tauntedTarget = null;
		if (!(m_Model is BirdCombatant))
		{
			return;
		}
		for (int i = 0; i < m_Model.CurrrentEffects.Count; i++)
		{
			BattleEffectGameData battleEffectGameData = m_Model.CurrrentEffects.Values.ElementAt(i);
			if (battleEffectGameData.m_Effects == null || battleEffectGameData.m_Effects.Count <= 0)
			{
				continue;
			}
			for (int j = 0; j < battleEffectGameData.m_Effects.Count; j++)
			{
				BattleEffect battleEffect = battleEffectGameData.m_Effects[j];
				if (battleEffect.EffectType == BattleEffectType.Taunt)
				{
					m_tauntedTarget = battleEffectGameData.m_Source;
				}
			}
		}
	}

	public void SetCachedUnfocusedPos(Vector3 startPoint)
	{
		if (m_Model.CombatantFaction == Faction.Birds)
		{
			CachedUnfocusedPos = startPoint - m_BattleMgr.m_BirdCenterPosition.position;
		}
		else
		{
			CachedUnfocusedPos = startPoint - m_BattleMgr.m_PigCenterPosition.position;
		}
	}

	public bool CheckAndContainsVisualEffect(string nameId)
	{
		GameObject value = null;
		if (LastingVisualEffects.TryGetValue(nameId, out value))
		{
			if ((bool)value)
			{
				return true;
			}
			DebugLog.Warn("Effect gameobject isn't present anymore, removed it!");
			LastingVisualEffects.Remove(nameId);
			return false;
		}
		return false;
	}

	public ICombatant GetModel()
	{
		return m_Model;
	}

	public void RegisterShowToolTip()
	{
		if (this.ShowTooltip != null)
		{
			this.ShowTooltip(m_Model);
		}
	}

	public void RegisterClicked()
	{
		if (this.Clicked != null)
		{
			this.Clicked(m_Model);
		}
	}

	protected virtual void LevelChanged(int oldLevel, int newLevel)
	{
		m_BattleMgr.m_blocked = true;
	}

	public virtual void RecreateAssetController()
	{
		if (m_Model != null && m_AssetController == null)
		{
			m_AssetController = DIContainerInfrastructure.GetCharacterAssetProvider(false).InstantiateObject(m_Model.CombatantAssetId, base.transform, Vector3.zero, Quaternion.identity, !m_Model.IsBanner).GetComponent<CharacterAssetController>();
			m_AssetController.SetModel(m_Model.CharacterModel, false);
			ReSizeCollider();
			base.transform.localScale = Vector3.one * m_Model.CharacterModel.Scale;
		}
	}

	public float GetCharacterAssetScale()
	{
		return m_Model.CharacterModel.Scale;
	}

	public void ReSizeCollider()
	{
		if (!m_AssetController)
		{
			return;
		}
		if (m_BoxCollider == null && (bool)GetComponent<Collider>())
		{
			m_BoxCollider = GetComponent<Collider>().GetComponent<BoxCollider>();
		}
		float num = 1f;
		float t = 1f;
		if (m_BattleMgr != null && m_BattleMgr.m_ScaleMgr != null)
		{
			t = (base.transform.localPosition.z - m_BattleMgr.m_ScaleMgr.m_Near_z_Border) / (m_BattleMgr.m_ScaleMgr.m_Far_z_Border - m_BattleMgr.m_ScaleMgr.m_Near_z_Border);
		}
		float num2 = Mathf.Lerp(0.85f, 1.33f, t);
		if (m_Model != null && m_Model.CharacterModel != null)
		{
			switch (m_Model.CharacterModel.CharacterSize)
			{
			case CharacterSizeType.Boss:
				num = 0.85f;
				break;
			case CharacterSizeType.Large:
				num = 0.85f;
				break;
			case CharacterSizeType.Medium:
				num = 1f;
				break;
			case CharacterSizeType.Small:
				num = 1.33f;
				break;
			}
		}
		if (!(m_BoxCollider == null))
		{
			m_BoxCollider.size = num2 * num * Vector3.Scale(m_AssetController.ColliderSize, m_AssetController.transform.localScale);
			m_BoxCollider.center = Vector3.Scale(m_AssetController.ColliderOffset, m_AssetController.transform.localScale) + new Vector3(0f, 0f, -30f);
		}
	}

	public virtual void SpawnHealthBar()
	{
	}

	protected virtual void OnHealthChanged(float oldHealth, float newHealth)
	{
		Vector3 zero = Vector3.zero;
		if (m_AssetController == null)
		{
			return;
		}
		if (Time.time - m_LastHealthUpdate <= DIContainerLogic.GetPacingBalancing().TimeForFillHealthBarChangedInSec)
		{
			zero.x = UnityEngine.Random.Range((0f - m_AssetController.ColliderSize.x) * 0.5f, m_AssetController.ColliderSize.x * 0.5f);
		}
		if (oldHealth > newHealth)
		{
			ScrollingCombatText scrollingCombatText = InstantiateScrollingCombatText(m_ScrollingCombatTextPrefab);
			SetLayerRecusively(scrollingCombatText.gameObject, base.gameObject.layer);
			scrollingCombatText.ShowHealthChange((int)(newHealth - oldHealth));
			scrollingCombatText.transform.parent = base.transform;
			scrollingCombatText.transform.Translate(zero);
			HandleAfterDamageAnim();
		}
		else if (oldHealth < newHealth)
		{
			ScrollingCombatText scrollingCombatText2 = InstantiateScrollingCombatText(m_ScrollingCombatTextPrefab);
			SetLayerRecusively(scrollingCombatText2.gameObject, base.gameObject.layer);
			scrollingCombatText2.transform.parent = base.transform;
			scrollingCombatText2.transform.Translate(zero);
			scrollingCombatText2.ShowHealthChange((int)(newHealth - oldHealth));
			if (m_Model.CombatantFaction == Faction.Pigs && !m_Model.IsPvPBird)
			{
				PlayAffectedAnim();
			}
		}
		else
		{
			ScrollingCombatText scrollingCombatText3 = InstantiateScrollingCombatText(m_ScrollingCombatTextPrefab);
			scrollingCombatText3.ShowHealthChange((int)(newHealth - oldHealth));
			scrollingCombatText3.transform.parent = base.transform;
			scrollingCombatText3.transform.Translate(zero);
		}
		m_CurrentPerkType = PerkType.None;
		m_LastHealthUpdate = Time.time;
	}

	private ScrollingCombatText InstantiateScrollingCombatText(ScrollingCombatText prefab)
	{
		return (ScrollingCombatText)UnityEngine.Object.Instantiate(prefab, m_CachedTransform.position + Vector3.Scale(m_AssetController.CombatTextPosition, new Vector3(m_AssetController.transform.localScale.x, m_AssetController.transform.localScale.y, 1f * prefab.transform.localScale.z)), Quaternion.identity);
	}

	public void PlayIdle()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayIdleAnimation();
		}
	}

	internal void PlayStunnedAnimation()
	{
		if ((bool)m_AssetController && !IsImmune())
		{
			m_AssetController.PlayStunnedAnimation();
		}
	}

	private bool IsImmune()
	{
		for (int i = 0; i < m_Model.CurrrentEffects.Count; i++)
		{
			BattleEffectGameData battleEffectGameData = m_Model.CurrrentEffects.Values.ElementAt(i);
			for (int j = 0; j < battleEffectGameData.m_Effects.Count; j++)
			{
				BattleEffect battleEffect = battleEffectGameData.m_Effects[j];
				if (battleEffect.EffectType == BattleEffectType.ImmunityToCurses)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void PlayMissed()
	{
		ScrollingCombatText scrollingCombatText = InstantiateScrollingCombatText(m_ScrollingCombatTextWithLabelPrefab);
		GameObject gameObject = UnityEngine.Object.Instantiate(m_CriticalAdditionalEffect);
		gameObject.transform.parent = m_AssetController.BodyRoot;
		gameObject.transform.localPosition = Vector3.zero;
		scrollingCombatText.ShowHealthChangedWithText(0, DIContainerInfrastructure.GetLocaService().Tr("btl_miss", "MISS"), "CombatText_Damage_Critical");
		scrollingCombatText.transform.parent = base.transform;
	}

	private float PlayAffectedAnim()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayAffectedAnim();
			if (m_Model.IsStunned)
			{
				m_AssetController.PlayStunnedAnimation();
			}
			return m_AssetController.GetAffectedAnimationLength();
		}
		return 0f;
	}

	internal float PlayAttentionAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayAttentionAnimation();
			if (m_Model.IsStunned)
			{
				m_AssetController.PlayStunnedAnimation();
			}
			return m_AssetController.GetAttentionAnimationLength();
		}
		return 0f;
	}

	public float PlayReviveAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayReviveAnim();
			return m_AssetController.GetReviveAnimationLength();
		}
		return 0f;
	}

	protected void HandleAfterDamageAnim()
	{
		if (m_Model.IsParticipating)
		{
			if (!m_Model.OverrideHitAnimation)
			{
				PlayHitAnim();
			}
		}
		else
		{
			DIContainerLogic.GetBattleService().RemoveCombatantFromBattle(m_BattleMgr.Model, m_Model);
		}
	}

	protected float PlayHitAnim()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayHitAnim();
			if (m_Model.IsStunned)
			{
				m_AssetController.PlayStunnedAnimation();
			}
			TentacleBossReaction();
			return m_AssetController.GetHitAnimationLength();
		}
		return 0f;
	}

	private void TentacleBossReaction()
	{
		if (m_AssetController is TentacleAssetController)
		{
			BossAssetController componentInChildren = m_BattleMgr.GetComponentInChildren<BossAssetController>();
			if (componentInChildren != null)
			{
				componentInChildren.PlayCustomReactionAnimation();
			}
		}
	}

	protected virtual void RegisterEventHandler()
	{
		DeregisterEventHandler();
		m_Model.CharacterModel.LevelChanged += LevelChanged;
		m_Model.Defeated += m_Model_Defeated;
		m_Model.KnockedOut += m_Model_KnockedOut;
		if (m_Model is BirdCombatant)
		{
			m_Model.CombatantClass.RankUpFromLevel += RankUpFromLevel;
		}
		if (m_Model != null)
		{
			m_Model.HealthChanged += OnHealthChanged;
			m_Model.Runaway += m_Model_Runaway;
			m_Model.PerkUsed += m_Model_PerkUsed;
			m_Model.DropItems += m_Model_DropItems;
		}
	}

	private void RankUpFromLevel(IInventoryItemGameData item, int fromLevel)
	{
		if ((bool)m_RankUpEffect)
		{
			m_RankUpEffect.m_Text.text = item.ItemData.Level.ToString("0");
			return;
		}
		m_RankUpEffect = DIContainerInfrastructure.GetBattleEffectAssetProvider().InstantiateObject("CharacterFX_MasteryUp", m_AssetController.BodyRoot, m_AssetController.BodyRoot.position, Quaternion.identity).GetComponent<BattleFXController>();
		m_RankUpEffect.m_Text.text = item.ItemData.Level.ToString("0");
		StartCoroutine(DelayRankUpEffectTillOnBasePos());
	}

	private IEnumerator DelayRankUpEffectTillOnBasePos()
	{
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().LevelUpDelayOnCharacters);
		m_RankUpEffect.gameObject.SetActive(true);
		m_RankUpEffect.transform.localScale = Vector3.one;
		UnityEngine.Object.Destroy(m_RankUpEffect.gameObject, m_RankUpEffect.gameObject.PlayAnimationOrAnimatorState("CharacterFX_RankUp"));
	}

	private void m_Model_DropItems(List<IInventoryItemGameData> obj)
	{
		Transform transform = ((!m_AssetController.OffHandBone) ? m_AssetController.BodyCenter : m_AssetController.OffHandBone);
		m_BattleMgr.SpawnLootEffects(obj, transform.position + new Vector3(0f, 0f, -5f), base.transform.localScale, false);
	}

	private void m_Model_PerkUsed(PerkType type, ICombatant target)
	{
		VisualEffectSetting setting = null;
		if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(GetPerkVisualEffectIdent(type), out setting))
		{
			ICombatant perkVisualEffectTarget = GetPerkVisualEffectTarget(type, m_Model, target);
			for (int i = 0; i < setting.VisualEffects.Count; i++)
			{
				VisualEffect effect = setting.VisualEffects[i];
				m_BattleMgr.InstantiateEffect(m_Model, effect, setting, new List<ICombatant> { perkVisualEffectTarget }, false);
			}
		}
	}

	private ICombatant GetPerkVisualEffectTarget(PerkType type, ICombatant source, ICombatant target)
	{
		if (type == PerkType.HocusPokus)
		{
			return source;
		}
		return target;
	}

	private string GetPerkVisualEffectIdent(PerkType type)
	{
		switch (type)
		{
		case PerkType.ChainAttack:
			return "Chain";
		case PerkType.CriticalStrike:
			return "Crit";
		case PerkType.Dispel:
			return "Purge";
		case PerkType.HocusPokus:
			return "Heal_Weak";
		default:
			return string.Empty;
		}
	}

	protected virtual void m_Model_KnockedOut()
	{
		DIContainerLogic.GetBattleService().RemoveCombatantFromBattle(m_BattleMgr.Model, m_Model);
		RemoveCharacterFromChargeBubbles();
		RemoveTauntsFromOpponents();
		int score = 0;
		if (m_BattleMgr.Model.IsPvP)
		{
			if (m_Model.CombatantFaction == Faction.Pigs && !m_Model.IsBanner)
			{
				score = DIContainerLogic.GetBattleService().GetScoreForPig(m_Model, m_BattleMgr.Model, true);
				DIContainerLogic.GetBattleService().AddScore(score, m_BattleMgr.Model);
			}
			if (!m_BattleMgr.Model.IsUnranked)
			{
				CheckKillAchievement();
			}
		}
		m_CounterAttack = false;
		if (m_Model.CombatantFaction == Faction.Birds && !m_BattleMgr.Model.IsPvP && DIContainerConfig.GetClientConfig().EnableSingleBirdRevive)
		{
			StartCoroutine(KnockOutBird());
		}
		else
		{
			StartCoroutine(KnockOutCharacter(score));
		}
	}

	private void CheckKillAchievement()
	{
		AchievementData achievementTracking = DIContainerInfrastructure.GetCurrentPlayer().Data.AchievementTracking;
		if (achievementTracking.DefeatedClasses == null)
		{
			achievementTracking.DefeatedClasses = new List<string>();
		}
		if (achievementTracking.DefeatedClasses.Contains("$AchievementTracked$") || GetModel().IsBanner)
		{
			return;
		}
		if (!achievementTracking.DefeatedClasses.Contains(GetModel().CombatantClass.BalancingData.NameId) && !achievementTracking.DefeatedClasses.Contains(GetModel().CombatantClass.BalancingData.ReplacementClassNameId))
		{
			achievementTracking.DefeatedClasses.Add(GetModel().CombatantClass.BalancingData.NameId);
		}
		int num = 0;
		List<ClassItemBalancingData> list = DIContainerBalancing.Service.GetBalancingDataList<ClassItemBalancingData>() as List<ClassItemBalancingData>;
		for (int i = 0; i < list.Count; i++)
		{
			ClassItemBalancingData classItemBalancingData = list[i];
			if (string.IsNullOrEmpty(classItemBalancingData.ReplacementClassNameId) && !string.IsNullOrEmpty(classItemBalancingData.RestrictedBirdId) && !classItemBalancingData.Inactive)
			{
				num++;
			}
		}
		if (achievementTracking.DefeatedClasses.Count == num)
		{
			string achievementIdForStoryItemIfExists = DIContainerInfrastructure.GetAchievementService().GetAchievementIdForStoryItemIfExists("defeatAllClasses");
			if (!string.IsNullOrEmpty(achievementIdForStoryItemIfExists))
			{
				DIContainerInfrastructure.GetAchievementService().ReportUnlocked(achievementIdForStoryItemIfExists);
				achievementTracking.DefeatedClasses.Add("$AchievementTracked$");
			}
		}
	}

	private void RemoveCharacterFromChargeBubbles()
	{
		IEnumerable<ICombatant> source = m_BattleMgr.Model.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != m_Model.CombatantFaction && c.AttackTarget == m_Model && c.IsCharging);
		for (int i = 0; i < source.Count(); i++)
		{
			ICombatant combatant = source.ElementAt(i);
			ICombatant model = GetModel();
			if (combatant.ChachedSkill != null && !DIContainerLogic.GetBattleService().GetNextTarget(m_BattleMgr.Model, combatant, combatant.ChachedSkill.Model, combatant.CombatantFaction))
			{
				combatant.AttackTarget = model;
			}
			CharacterSpeechBubble characterSpeechBubble = combatant.CombatantView.m_SpeechBubbles.Values.FirstOrDefault();
			if (characterSpeechBubble != null && characterSpeechBubble.m_IsTargetedBubble)
			{
				if (combatant.AttackTarget != null)
				{
					characterSpeechBubble.SetTargetIcon("Target_" + combatant.AttackTarget.CombatantAssetId);
				}
				characterSpeechBubble.UpdateSkill();
			}
		}
	}

	private void RemoveTauntsFromOpponents()
	{
		IEnumerable<ICombatant> source = m_BattleMgr.Model.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction != m_Model.CombatantFaction);
		for (int i = 0; i < source.Count(); i++)
		{
			ICombatant combatant = source.ElementAt(i);
			CharacterControllerBattleGround characterControllerBattleGround = combatant.CombatantView as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround && characterControllerBattleGround.m_tauntedTarget == m_Model)
			{
				characterControllerBattleGround.m_tauntedTarget = null;
			}
		}
	}

	protected virtual void m_Model_Defeated()
	{
		DIContainerLogic.GetBattleService().RemoveCombatantFromBattle(m_BattleMgr.Model, m_Model);
		RemoveCharacterFromChargeBubbles();
		RemoveTauntsFromOpponents();
		m_CounterAttack = false;
		int score = 0;
		if (m_Model.CombatantFaction == Faction.Pigs && (!m_Model.IsPvPBird || m_Model.IsBanner))
		{
			score = DIContainerLogic.GetBattleService().GetScoreForPig(m_Model, m_BattleMgr.Model, true);
			DIContainerLogic.GetBattleService().AddScore(score, m_BattleMgr.Model);
		}
		if (m_Model.CombatantFaction == Faction.Birds && !m_Model.IsPvPBird && DIContainerConfig.GetClientConfig().EnableSingleBirdRevive)
		{
			StartCoroutine(KnockOutBird());
		}
		else
		{
			StartCoroutine(DefeatCharacter(score));
		}
	}

	private void m_Model_Runaway()
	{
		StartCoroutine(RunawayCoroutine());
	}

	private IEnumerator RunawayCoroutine()
	{
		yield return new WaitForSeconds(PlayGoToStartPositionFromBasePosition(true));
		DestroyCharacter();
	}

	public virtual void DeregisterEventHandler()
	{
		if (m_Model != null)
		{
			if (m_Model is BirdCombatant)
			{
				m_Model.CombatantClass.RankUpFromLevel -= RankUpFromLevel;
			}
			m_Model.CharacterModel.LevelChanged -= LevelChanged;
			m_Model.Defeated -= m_Model_Defeated;
			m_Model.KnockedOut -= m_Model_KnockedOut;
			m_Model.HealthChanged -= OnHealthChanged;
			m_Model.Runaway -= m_Model_Runaway;
			m_Model.PerkUsed -= m_Model_PerkUsed;
			m_Model.DropItems -= m_Model_DropItems;
		}
	}

	public abstract IEnumerator DoTurn(int turn);

	public void ActivateRageSkill()
	{
		DebugLog.Log("User activated rage skill: " + m_Model.CombatantNameId);
		if (m_Model.GetSkill(2) == null)
		{
			DebugLog.Error("No Rage Skill!");
			return;
		}
		m_SkillToDo = m_Model.GetSkill(2);
		m_SkillToDo.m_IsRageSkill = true;
		DebugLog.Log("Character triggered rage skill");
		if (m_SkillToDo.Model.Balancing.TargetType == SkillTargetTypes.Attack)
		{
			m_Model.AttackTarget = m_BattleMgr.Model.m_CombatantsByInitiative.FirstOrDefault((ICombatant c) => c.CombatantFaction == DIContainerLogic.GetBattleService().GetOppositeFaction(m_Model.CombatantFaction));
		}
		else
		{
			m_Model.AttackTarget = m_BattleMgr.Model.m_CombatantsByInitiative.FirstOrDefault((ICombatant c) => c.CombatantFaction == m_Model.CombatantFaction);
		}
		m_BattleMgr.Model.SetFactionRage(m_Model.CombatantFaction, 0f);
		m_BattleMgr.Model.RegisterRageUsed(100f, m_Model);
		m_BattleMgr.Model.m_BattleEndData.m_RageUsedByBird.Add(GetModel().CombatantNameId);
		m_IsWaitingForInput = false;
	}

	public void ActivateConsumableSkill(ConsumableItemGameData consumable)
	{
		StartCoroutine(DoConsumableSkill(consumable));
	}

	public IEnumerator DoConsumableSkill(ConsumableItemGameData consumable)
	{
		DeregisterFromInput();
		if (!DIContainerLogic.GetBattleService().UseConsumable(m_BattleMgr.Model.m_ControllerInventory, consumable, m_Model, m_BattleMgr.Model))
		{
			yield break;
		}
		if (!m_BattleMgr.Model.m_PotionsUsed.ContainsKey(consumable.Name))
		{
			m_BattleMgr.Model.m_PotionsUsed.Add(consumable.Name, 0);
		}
		Dictionary<string, int> potionsUsed;
		Dictionary<string, int> dictionary = (potionsUsed = m_BattleMgr.Model.m_PotionsUsed);
		string key;
		string key2 = (key = consumable.Name);
		int num = potionsUsed[key];
		dictionary[key2] = num + 1;
		Dictionary<string, string> values = new Dictionary<string, string> { { "ConsumableName", consumable.Name } };
		ABHAnalyticsHelper.AddPlayerStatusToTracking(values);
		DIContainerInfrastructure.GetAnalyticsSystem(true).LogEventWithParameters("ConsumableUsed", values);
		m_BattleMgr.Model.m_LastUsedConsumable = consumable.BalancingData.NameId;
		DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.OnConsumableUsed, m_Model, m_Model);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		if ((bool)m_AssetController)
		{
			m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(consumable.CombatSkill.BoneAnimationUserTrigger);
			m_AssetController.m_BoneAnimation.RegisterUserTriggerDelegate(consumable.CombatSkill.BoneAnimationUserTrigger);
		}
		if (consumable.ConsumableSkill.Balancing.TargetType == SkillTargetTypes.Attack)
		{
			m_BattleMgr.IsConsumableUsePossible = false;
			yield return StartCoroutine(consumable.CombatSkill.DoAction(m_BattleMgr.Model, m_Model, m_BattleMgr.Model.m_CombatantsByInitiative.FirstOrDefault((ICombatant c) => c.CombatantFaction != m_Model.CombatantFaction)));
		}
		else
		{
			yield return StartCoroutine(consumable.CombatSkill.DoAction(m_BattleMgr.Model, m_Model, m_Model));
		}
		if ((bool)m_AssetController)
		{
			m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(consumable.CombatSkill.BoneAnimationUserTrigger);
		}
		m_BattleMgr.m_BattleUI.m_ConsumableBar.RefreshCoins();
		if (this.UsedConsumable != null)
		{
			this.UsedConsumable();
		}
	}

	protected abstract void FocusInitiaive();

	public float PlayGoToBasePositionFromStartPosition()
	{
		if (m_Model.IsBanner)
		{
			return 0f;
		}
		if (m_Model.CombatantFaction == Faction.Birds)
		{
			m_StartPosition = m_InitialStartPosOffset;
			SetupTweenMovementPositions(null, Vector3.zero, m_BattleMgr.m_BirdCenterPosition, CachedUnfocusedPos);
		}
		else
		{
			m_StartPosition = m_InitialStartPosOffset;
			if (m_Model is BossCombatant)
			{
				SetupTweenMovementPositions(null, Vector3.zero, base.transform.parent, CachedUnfocusedPos);
			}
			else
			{
				if (m_Model.CombatantView.transform.localScale == new Vector3(0.01f, 0.01f, 0.01f))
				{
					StartCoroutine(PlaySpawnWithDelay());
					return 0f;
				}
				SetupTweenMovementPositions(null, Vector3.zero, m_BattleMgr.m_PigCenterPosition, CachedUnfocusedPos);
			}
		}
		float num = DIContainerLogic.GetPacingBalancing().BaseBattleEnterDelay;
		if (m_Model.CombatantNameId.Contains("boss_tinker"))
		{
			m_BattleMgr.m_CameraAnimationRoot.GetComponent<Animation>().Play("Camera_Shake_BossMovement");
			m_BattleMgr.m_CameraAnimationRoot.GetComponent<Animation>().wrapMode = WrapMode.Loop;
			Invoke("StopCameraShake", 3.5f);
			num *= 3.5f;
		}
		else if (m_Model.CombatantNameId.Contains("boss_kraken"))
		{
			m_BattleMgr.m_CameraAnimationRoot.GetComponent<Animation>().Play("Camera_Reset");
			m_BattleMgr.m_CameraAnimationRoot.GetComponent<Animation>().Play("Camera_Shake_KrakenBossMovement");
			num *= 2.5f;
		}
		m_TweenMovement.m_DurationInSeconds = num;
		m_TweenMovement.Play();
		m_AssetController.PlayAnimationForTime("Move_Loop", "Idle", num);
		ReSizeCollider();
		return num;
	}

	private IEnumerator PlaySpawnWithDelay()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(m_minSpawnDelay, m_maxSpawnDelay));
		m_Model.CombatantView.transform.localScale = Vector3.one;
		m_Model.CombatantView.m_AssetController.PlayAnimation("Spawn");
		ReSizeCollider();
		yield return new WaitForEndOfFrame();
		if ((bool)GetComponentInChildren<CharacterHealthBar>())
		{
			GetComponentInChildren<CharacterHealthBar>().transform.localScale = Vector3.one;
		}
		yield return new WaitForSeconds(0.96f);
		m_Model.CombatantView.m_AssetController.PlayIdleAnimation();
	}

	private void StopCameraShake()
	{
		m_BattleMgr.m_CameraAnimationRoot.GetComponent<Animation>().Play("Camera_Reset");
		m_BattleMgr.m_CameraAnimationRoot.GetComponent<Animation>().wrapMode = WrapMode.Once;
	}

	public float PlayGoToStartPositionFromBasePosition(bool destroy)
	{
		if (m_Model.IsBanner)
		{
			return 0f;
		}
		SetupTweenMovementPositions(null, Vector3.zero, null, m_StartPosition);
		float baseBattleEnterDelay = DIContainerLogic.GetPacingBalancing().BaseBattleEnterDelay;
		if ((bool)m_TweenMovement)
		{
			m_TweenMovement.m_DurationInSeconds = baseBattleEnterDelay;
			m_TweenMovement.Play();
		}
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayAnimationForTime("Move_Loop", "Idle", baseBattleEnterDelay);
		}
		if (base.gameObject != null && destroy)
		{
			Invoke("DestroyCharacter", baseBattleEnterDelay);
			UnityEngine.Object.Destroy(base.gameObject, baseBattleEnterDelay + 0.1f);
		}
		return baseBattleEnterDelay;
	}

	public float PlayGoToStartPositionFromFocusPosition(bool destroy)
	{
		if (m_Model.IsBanner)
		{
			return 0f;
		}
		SetupTweenMovementPositions(null, Vector3.zero, null, m_StartPosition - m_BirdFocusPosOffset);
		float baseBattleEnterDelay = DIContainerLogic.GetPacingBalancing().BaseBattleEnterDelay;
		m_TweenMovement.m_DurationInSeconds = baseBattleEnterDelay;
		m_TweenMovement.Play();
		m_AssetController.PlayAnimationForTime("Move_Loop", "Idle", baseBattleEnterDelay);
		if (destroy)
		{
			Invoke("DestroyCharacter", baseBattleEnterDelay);
			UnityEngine.Object.Destroy(base.gameObject, baseBattleEnterDelay);
		}
		return baseBattleEnterDelay;
	}

	public float PlayGoToFocusPositionFromStartPosition()
	{
		if (m_Model.IsBanner)
		{
			return 0f;
		}
		if (m_Model.CombatantFaction == Faction.Birds)
		{
			m_StartPosition = m_InitialStartPosOffset;
			SetupTweenMovementPositions(null, Vector3.zero, m_BattleMgr.m_BirdCenterPosition, m_CachedFocusPos);
		}
		else
		{
			m_StartPosition = m_InitialStartPosOffset;
			SetupTweenMovementPositions(null, Vector3.zero, m_BattleMgr.m_PigCenterPosition, m_CachedFocusPos);
		}
		float baseBattleEnterDelay = DIContainerLogic.GetPacingBalancing().BaseBattleEnterDelay;
		m_TweenMovement.m_DurationInSeconds = baseBattleEnterDelay;
		m_TweenMovement.Play();
		m_AssetController.PlayAnimationForTime("Move_Loop", (!m_Model.IsStunned) ? "Idle" : "Stunned", baseBattleEnterDelay);
		return baseBattleEnterDelay;
	}

	public float PlayGoToBasePosition()
	{
		if (m_TweenMovement == null || m_BattleMgr == null || m_AssetController == null || m_Model == null || m_Model is BossCombatant || m_Model.IsBanner || (!m_AssetController.HasNoBoneAnimation && m_AssetController.m_BoneAnimation != null && m_AssetController.m_BoneAnimation.IsPlaying("Defeated")))
		{
			return 0f;
		}
		if (m_Model.CombatantFaction == Faction.Birds)
		{
			SetupTweenMovementPositions(base.transform, Vector3.zero, m_BattleMgr.m_BirdCenterPosition, CachedUnfocusedPos);
		}
		else
		{
			SetupTweenMovementPositions(base.transform, Vector3.zero, m_BattleMgr.m_PigCenterPosition, CachedUnfocusedPos);
		}
		float timeFromFocusPosToBasePosInSec = DIContainerLogic.GetPacingBalancing().TimeFromFocusPosToBasePosInSec;
		m_TweenMovement.m_DurationInSeconds = timeFromFocusPosToBasePosInSec;
		m_TweenMovement.Play();
		PlayMoveAnim();
		return m_TweenMovement.MovementDuration;
	}

	private void PlayMoveAnim()
	{
		if (!m_Model.IsKnockedOut && !m_AssetController.IsPlayingAnimation("Defeated"))
		{
			m_AssetController.PlayAnimation("Move_Once");
			if (m_Model.IsStunned)
			{
				m_AssetController.PlayStunnedAnimation();
			}
			else
			{
				m_AssetController.PlayIdleAnimationQueued();
			}
		}
	}

	public float PlayGoToPosition(Vector3 offset, float timeToMove, bool run, Transform targetTransform = null)
	{
		if (m_Model.IsBanner)
		{
			return 0f;
		}
		SetupTweenMovementPositions(base.transform, Vector3.zero, targetTransform, offset);
		m_TweenMovement.m_DurationInSeconds = timeToMove;
		m_TweenMovement.Play();
		if (run)
		{
			m_AssetController.PlayAnimationForTime("Move_Loop", "Idle", timeToMove);
		}
		else
		{
			m_AssetController.PlayAnimationForTime("Move_Loop", "Idle", timeToMove);
		}
		return m_TweenMovement.MovementDuration;
	}

	private void SetupTweenMovementPositions(Transform startTransform, Vector3 startOffset, Transform endTransform, Vector3 endOffset)
	{
		m_TweenMovement.m_StartTransform = startTransform;
		m_TweenMovement.m_StartOffset = startOffset;
		m_TweenMovement.m_EndTransform = endTransform;
		m_TweenMovement.m_EndOffset = endOffset;
	}

	public void BeSheltered()
	{
		if (!m_Model.IsBanner)
		{
			float num = ((m_Model.CombatantFaction == Faction.Birds) ? 1 : (-1));
			SetupTweenMovementPositions(base.transform, Vector3.zero, base.transform, new Vector3(m_BeShelteredRetreatValue * num, 0f, 0f));
			float timeToJumpInFrontOfAlly = DIContainerLogic.GetPacingBalancing().TimeToJumpInFrontOfAlly;
			m_TweenMovement.m_DurationInSeconds = timeToJumpInFrontOfAlly;
			m_TweenMovement.Play();
			PlayMoveAnim();
		}
	}

	public float Shelter(ICombatant victim)
	{
		if (m_Model.IsBanner || !m_Model.IsAlive)
		{
			return 0f;
		}
		if (victim.IsBanner)
		{
			Vector3 endOffset = Vector3.zero;
			float num = ((m_Model.CombatantFaction != 0) ? (-1f) : 1f);
			endOffset = new Vector3(num * (m_AssetController.OffsetFromEnemyX + victim.CombatantView.m_AssetController.OffsetFromEnemyX) * m_AssetController.transform.localScale.x, 0f, OffsetFromEnemyZ);
			SetupTweenMovementPositions(base.transform, Vector3.zero, victim.CombatantView.transform, endOffset);
		}
		else
		{
			SetupTweenMovementPositions(base.transform, Vector3.zero, victim.CombatantView.transform, Vector3.zero);
		}
		float timeToJumpInFrontOfAlly = DIContainerLogic.GetPacingBalancing().TimeToJumpInFrontOfAlly;
		m_TweenMovement.m_DurationInSeconds = timeToJumpInFrontOfAlly;
		m_TweenMovement.Play();
		PlayMoveAnim();
		return timeToJumpInFrontOfAlly;
	}

	public float PlayGoToFocusPosition()
	{
		if (m_Model.IsBanner)
		{
			return 0f;
		}
		float num = Vector3.Magnitude(base.transform.position - (m_BattleMgr.m_BirdCenterPosition.position + m_CachedFocusPos));
		if (num < 1f)
		{
			return 0f;
		}
		if (m_Model.CombatantFaction == Faction.Birds)
		{
			SetupTweenMovementPositions(null, Vector3.zero, m_BattleMgr.m_BirdCenterPosition, m_CachedFocusPos);
			PlayMoveAnim();
		}
		else if (m_Model is BossCombatant)
		{
			BossAssetController bossAssetController = m_AssetController as BossAssetController;
			SetupTweenMovementPositions(null, Vector3.zero, null, Vector3.zero);
		}
		else
		{
			SetupTweenMovementPositions(null, Vector3.zero, m_BattleMgr.m_PigFocusPosition, Vector3.zero);
			PlayMoveAnim();
		}
		float timeFromBasePosToFocusPosInSec = DIContainerLogic.GetPacingBalancing().TimeFromBasePosToFocusPosInSec;
		if (m_TweenMovement == null || m_TweenMovement.GetMoveDistance() <= 0.5f)
		{
			return timeFromBasePosToFocusPosInSec;
		}
		m_TweenMovement.m_DurationInSeconds = timeFromBasePosToFocusPosInSec;
		m_TweenMovement.Play();
		return timeFromBasePosToFocusPosInSec;
	}

	public float PlayGoToAttackPosition(bool centerPosition = false, bool focusPosition = false)
	{
		if (m_Model.IsBanner)
		{
			return 0f;
		}
		if (m_Model.CombatantView.targetSheltered == null)
		{
			if (m_Model.CombatantFaction == Faction.Birds)
			{
				if (centerPosition)
				{
					SetupTweenMovementPositions(m_BattleMgr.m_BirdCenterPosition, m_CachedFocusPos, m_BattleMgr.m_PigCenterPosition, Vector3.zero);
				}
				else if (focusPosition)
				{
					SetupTweenMovementPositions(m_BattleMgr.m_BirdCenterPosition, m_CachedFocusPos, m_BattleMgr.m_BirdFocusPosition, Vector3.zero);
				}
				else
				{
					SetupTweenMovementPositions(m_BattleMgr.m_BirdCenterPosition, m_CachedFocusPos, m_Model.AttackTarget.CombatantView.transform, new Vector3(0f - (m_AssetController.OffsetFromEnemyX + m_Model.AttackTarget.CombatantView.m_AssetController.OffsetFromEnemyX) * m_AssetController.transform.localScale.x, 0f, OffsetFromEnemyZ));
				}
			}
			else if (centerPosition)
			{
				SetupTweenMovementPositions(m_BattleMgr.m_PigCenterPosition, m_CachedFocusPos, m_BattleMgr.m_BirdCenterPosition, Vector3.zero);
			}
			else if (focusPosition)
			{
				SetupTweenMovementPositions(m_Model.CombatantView.transform, Vector3.zero, m_BattleMgr.m_PigFocusPosition, Vector3.zero);
			}
			else
			{
				SetupTweenMovementPositions(m_BattleMgr.m_BirdFocusPosition, Vector3.zero, m_Model.AttackTarget.CombatantView.transform, new Vector3(m_AssetController.OffsetFromEnemyX + m_Model.AttackTarget.CombatantView.m_AssetController.OffsetFromEnemyX * m_AssetController.transform.localScale.x, 0f, OffsetFromEnemyZ));
			}
		}
		else if (m_Model.CombatantFaction == Faction.Birds)
		{
			Vector3 endOffset = new Vector3((0f - (m_AssetController.OffsetFromEnemyX + m_Model.AttackTarget.CombatantView.m_AssetController.OffsetFromEnemyX)) * m_AssetController.transform.localScale.x, 0f, OffsetFromEnemyZ);
			if (m_Model.AttackTarget.IsBanner)
			{
				endOffset += new Vector3(endOffset.x / 1.5f, 0f, 0f);
			}
			SetupTweenMovementPositions(m_BattleMgr.m_BirdCenterPosition, m_CachedFocusPos, m_Model.CombatantView.targetSheltered.CombatantView.transform, endOffset);
		}
		else
		{
			Vector3 endOffset2 = new Vector3((m_AssetController.OffsetFromEnemyX + m_Model.AttackTarget.CombatantView.m_AssetController.OffsetFromEnemyX) * m_AssetController.transform.localScale.x, 0f, OffsetFromEnemyZ);
			if (m_Model.AttackTarget.IsBanner)
			{
				endOffset2 += new Vector3(endOffset2.x / 1.5f, 0f, 0f);
			}
			SetupTweenMovementPositions(m_BattleMgr.m_BirdFocusPosition, Vector3.zero, m_Model.CombatantView.targetSheltered.CombatantView.transform, endOffset2);
		}
		float timeFromFocusPosToAttackPosInSec = DIContainerLogic.GetPacingBalancing().TimeFromFocusPosToAttackPosInSec;
		m_TweenMovement.m_DurationInSeconds = timeFromFocusPosToAttackPosInSec;
		m_TweenMovement.Play();
		return timeFromFocusPosToAttackPosInSec;
	}

	protected void RegisterForInput()
	{
		m_IsWaitingForInput = true;
		for (int num = m_BattleMgr.Model.m_CombatantsByInitiative.Count - 1; num >= 0; num--)
		{
			ICombatant combatant = m_BattleMgr.Model.m_CombatantsByInitiative[num];
			if (combatant != null && (bool)combatant.CombatantView)
			{
				combatant.CombatantView.ReSizeCollider();
			}
		}
		if (m_BattleMgr.Model.m_CombatantsByInitiative.Any((ICombatant c) => c.CanUseConsumable))
		{
			m_BattleMgr.EnterConsumableButton();
		}
	}

	public void ReviveCombatant(ICombatant combatant, float healthPercentage)
	{
		combatant.HealDamage(combatant.ModifiedHealth * healthPercentage / 100f - combatant.CurrentHealth, combatant);
		DIContainerLogic.GetBattleService().HealCurrentTurn(combatant, m_BattleMgr.Model, true, true);
		if (combatant.IsKnockedOut)
		{
			combatant.IsKnockedOut = false;
			DIContainerLogic.GetBattleService().AddNewCombatantToBattle(m_BattleMgr.Model, combatant);
			DIContainerLogic.GetBattleService().ReCalculateInitiative(m_BattleMgr.Model);
			combatant.CombatantView.SpawnHealthBar();
		}
		if (m_InstantiatedReviveMe != null)
		{
			ToggleReviveme(false);
			Invoke("DestroyReviveMe", m_InstantiatedReviveMe.GetAnimationOrAnimatorStateLength("ReviveIndicator_Hide"));
		}
	}

	private void DestroyReviveMe()
	{
		UnityEngine.Object.Destroy(m_InstantiatedReviveMe);
	}

	protected abstract void OnCombatantClicked(ICombatant sender);

	protected void DeregisterFromInput()
	{
		for (int num = m_BattleMgr.Model.m_CombatantsByInitiative.Count - 1; num >= 0; num--)
		{
			ICombatant combatant = m_BattleMgr.Model.m_CombatantsByInitiative[num];
			CharacterControllerBattleGroundBase combatantView = combatant.CombatantView;
			combatantView.Clicked = (Action<ICombatant>)Delegate.Remove(combatantView.Clicked, new Action<ICombatant>(OnCombatantClicked));
		}
	}

	public float PlayDefeatCharacter()
	{
		if ((bool)m_AssetController && (m_AssetController.HasNoBoneAnimation || !m_AssetController.GetComponent<Animation>().IsPlaying("Defeated")))
		{
			m_AssetController.PlayDefeatAnim();
			TentacleBossReaction();
			return m_AssetController.GetDefeatAnimationLength();
		}
		return 0f;
	}

	public float PlayCheerCharacter()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayCheerAnim();
			if (m_Model.IsStunned)
			{
				m_AssetController.PlayStunnedAnimation();
			}
			return m_AssetController.GetCheerAnimationLength();
		}
		return 0f;
	}

	public float PlayKnockOutCharacter()
	{
		if ((bool)m_AssetController && (!m_BattleMgr.Model.IsPvP || m_Model.IsBanner))
		{
			m_AssetController.PlayDefeatAnim();
			return m_AssetController.GetDefeatAnimationLength();
		}
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayKnockoutAnim();
			return m_AssetController.GetKnockOutAnimationLength();
		}
		return 0f;
	}

	public float PlayAttackAnimation(bool useOffhand = false)
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayAttackAnim(useOffhand);
			return m_AssetController.GetAttackAnimationLength();
		}
		return 0f;
	}

	public float PlaySecondaryAttackAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlaySecondaryAttackAnim();
			return m_AssetController.GetSecondaryAttackAnimationLength();
		}
		return 0f;
	}

	public float PlayTumbledAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayFailAnim();
			return m_AssetController.GetFailAnimationLength();
		}
		return 0f;
	}

	public float PlayMournAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayMournAnim();
			return m_AssetController.GetMournAnimationLength();
		}
		return 0f;
	}

	public float PlaySurprisedAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlaySuprisedAnim(m_Model.IsStunned);
			return m_AssetController.GetSuprisedAnimationLength();
		}
		return 0f;
	}

	public float PlayHitAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayHitAnim();
			return m_AssetController.GetHitAnimationLength();
		}
		return 0f;
	}

	public float PlaySupportAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlaySupportAnim();
			return m_AssetController.GetSupportAnimationLength();
		}
		return 0f;
	}

	public void PlayAffectedAnimationQueued()
	{
		if ((bool)m_AssetController)
		{
			string returnAnimation = "Idle";
			if (m_Model.IsStunned)
			{
				returnAnimation = "Stunned";
			}
			if (m_Model.CombatantFaction == Faction.Pigs && !m_Model.IsPvPBird)
			{
				m_AssetController.PlayAffectedAnimQueued(returnAnimation);
			}
		}
	}

	public float PlayLaughAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayLaughAnim();
			return m_AssetController.GetLaughAnimationLength();
		}
		return 0f;
	}

	public float PlayRageSkillAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayRageSkillAnim();
			return m_AssetController.GetRageSkillAnimationLength();
		}
		return 0f;
	}

	public float PlayRageAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayRageAnim(m_Model.IsStunned);
			return m_AssetController.GetRageAnimationLength();
		}
		return 0f;
	}

	public float PlayAffectedAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayAffectedAnim();
			return m_AssetController.GetAffectedAnimationLength();
		}
		return 0f;
	}

	public void HandleSummedDamage()
	{
		if (m_Model.SummedDamagePerTurn != 0f)
		{
			DebugLog.Log("End dmg after round: " + DIContainerLogic.GetBattleService().DealDamageFromCurrentTurn(m_Model, m_BattleMgr.Model, null));
		}
	}

	public abstract void UpdateInitiative();

	public IEnumerator DefeatCharacter(int score, bool spawnLoot = true, bool playAnimation = true)
	{
		Dictionary<string, LootInfoData> pigDefeatedLoot = m_Model.CharacterModel.DefeatLoot;
		bool useBonus = false;
		BonusEventBalancingData bonusBalancing = DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing;
		if (m_BattleMgr.Model.IsChronicleCave && bonusBalancing != null && bonusBalancing.BonusType == BonusEventType.CcLootBonus)
		{
			useBonus = true;
			foreach (LootInfoData lootData in pigDefeatedLoot.Values)
			{
				lootData.Value += (int)((float)lootData.Value * (bonusBalancing.BonusFactor / 100f));
			}
		}
		if (pigDefeatedLoot != null && pigDefeatedLoot.ContainsKey("experience"))
		{
			float extraPercentage = GetXpDifferenceFactor();
			pigDefeatedLoot["experience"].Value = (int)((float)pigDefeatedLoot["experience"].Value * extraPercentage);
		}
		if (m_BattleMgr.Model.Balancing.BonusLoot != null && m_BattleMgr.Model.CurrentWaveIndex == m_BattleMgr.Model.Balancing.BattleParticipantsIds.Count - 1 && m_BattleMgr.Model.m_CombatantsPerFaction[Faction.Pigs].Where((ICombatant p) => p.IsAlive).Count() == 0 && m_BattleMgr.Model.Balancing.BonusLoot.ContainsKey("experience") && !m_BattleMgr.m_bonusXpGained)
		{
			int bonusXp = m_BattleMgr.Model.Balancing.BonusLoot.Where((KeyValuePair<string, int> p) => p.Key.Equals("experience")).FirstOrDefault().Value;
			if (pigDefeatedLoot.ContainsKey("experience"))
			{
				pigDefeatedLoot["experience"].Value += bonusXp;
			}
			else
			{
				LootInfoData xpLoot = new LootInfoData
				{
					Value = bonusXp,
					Level = 1,
					Quality = 1
				};
				pigDefeatedLoot.Add("experience", xpLoot);
			}
			m_BattleMgr.m_bonusXpGained = true;
		}
		List<IInventoryItemGameData> loot = new List<IInventoryItemGameData>();
		if (spawnLoot && m_Model.CombatantFaction == Faction.Pigs && pigDefeatedLoot != null)
		{
			loot = DIContainerLogic.GetLootOperationService().RewardLootGetInputCopy(m_BattleMgr.Model.m_ControllerInventory, 0, pigDefeatedLoot, "defeated_pig");
			if (m_Model.CombatantNameId == "pig_golden_pig")
			{
				DIContainerLogic.WorldMapService.HandleGoldenPigFinishState(DIContainerInfrastructure.GetCurrentPlayer(), GoldenPigFinishState.Defeated);
				DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			}
			else if (m_Model.CombatantNameId == "pig_golden_pig_boss")
			{
				DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentMiniCampaign.CurrentHotspotGameData.Data.LastVisitDateTime = DIContainerLogic.GetTimingService().GetPresentTime();
				DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			}
			int masteryCount = 0;
			for (int i = 0; i < loot.Count; i++)
			{
				if (loot[i].ItemData.NameId.Equals("experience"))
				{
					masteryCount++;
				}
			}
			List<ClassItemGameData> classItemList = (from b in m_BattleMgr.Model.m_CombatantsPerFaction[Faction.Birds]
				where !b.CharacterModel.IsNPC
				select b into c
				select c.CombatantClass).ToList();
			DebugLog.Log("ADD " + masteryCount + " MASTERY TO " + classItemList.Count + " BIRDS");
			DIContainerLogic.InventoryService.AddMastery(m_BattleMgr.Model.m_ControllerInventory, classItemList, masteryCount);
		}
		if (!m_AssetController.HasNoBoneAnimation)
		{
			m_AssetController.m_BoneAnimation.playAutomatically = false;
		}
		if (!m_Model.IsKnockedOut)
		{
			if (m_Model is BossCombatant)
			{
				PlayDefeatCharacter();
				yield return new WaitForSeconds(1.5f);
			}
			else
			{
				yield return new WaitForSeconds(PlayDefeatCharacter());
			}
		}
		yield return new WaitForEndOfFrame();
		if (spawnLoot && m_Model.CombatantFaction == Faction.Pigs)
		{
			if (m_AssetController.BodyRoot != null)
			{
				m_BattleMgr.SpawnLootEffects(loot, m_AssetController.BodyRoot.position, base.transform.localScale, useBonus);
			}
			else
			{
				m_BattleMgr.SpawnLootEffects(loot, m_AssetController.transform.position, base.transform.localScale, useBonus);
			}
			VisualEffectSetting setting = null;
			switch (m_Model.CharacterModel.CharacterSize)
			{
			case CharacterSizeType.Small:
			{
				string effectName = "PigDefeated_Small";
				if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(effectName, out setting))
				{
					yield return StartCoroutine(m_BattleMgr.InstantiateEffects(m_Model, setting.VisualEffects[0], setting, new List<ICombatant> { m_Model }, false));
				}
				break;
			}
			case CharacterSizeType.Medium:
			{
				string effectName = "PigDefeated_Medium";
				if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(effectName, out setting))
				{
					yield return StartCoroutine(m_BattleMgr.InstantiateEffects(m_Model, setting.VisualEffects[0], setting, new List<ICombatant> { m_Model }, false));
				}
				break;
			}
			case CharacterSizeType.Large:
			{
				string effectName = "PigDefeated_Large";
				if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(effectName, out setting))
				{
					yield return StartCoroutine(m_BattleMgr.InstantiateEffects(m_Model, setting.VisualEffects[0], setting, new List<ICombatant> { m_Model }, false));
				}
				break;
			}
			case CharacterSizeType.Boss:
			{
				string effectName = "PigDefeated_VeryLarge";
				if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(effectName, out setting))
				{
					yield return StartCoroutine(m_BattleMgr.InstantiateEffects(m_Model, setting.VisualEffects[0], setting, new List<ICombatant> { m_Model }, false));
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		if (m_Model.CombatantFaction == Faction.Pigs && (!m_Model.IsPvPBird || m_Model.IsBanner) && score > 0)
		{
			ShowScore(score);
		}
		if (!(m_Model is BossCombatant))
		{
			DestroyCharacter();
		}
	}

	private float GetXpDifferenceFactor()
	{
		float num = DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
		float num2 = m_Model.CharacterModel.Level;
		float num3 = num2 - num;
		if (num3 == 0f)
		{
			return 1f;
		}
		foreach (ExperienceScalingBalancingData item in from b in DIContainerBalancing.Service.GetBalancingDataList<ExperienceScalingBalancingData>()
			orderby b.Difference
			select b)
		{
			if (item.Difference >= num3)
			{
				return item.XpModifier / 100f;
			}
		}
		return 1f;
	}

	public IEnumerator KnockOutBird()
	{
		if (!m_AssetController.HasNoBoneAnimation)
		{
			m_AssetController.m_BoneAnimation.playAutomatically = false;
		}
		PlayGoToBasePosition();
		m_AssetController.PlayKnockoutAnim();
		yield return new WaitForSeconds(m_AssetController.GetKnockOutAnimationLength());
		for (int i = 0; i < LastingVisualEffects.Count; i++)
		{
			GameObject effect = LastingVisualEffects.Values.ElementAt(i);
			if ((bool)effect)
			{
				UnityEngine.Object.Destroy(effect);
			}
		}
		if (m_ReviveMePrefab != null && !m_BattleMgr.Model.IsPvP && DIContainerConfig.GetClientConfig().EnableSingleBirdRevive)
		{
			SpawnReviveMe();
		}
	}

	private void SpawnReviveMe()
	{
		Vector3 vector = Vector3.Scale(m_AssetController.HealthBarPosition, m_AssetController.transform.localScale);
		m_InstantiatedReviveMe = UnityEngine.Object.Instantiate(m_ReviveMePrefab);
		m_InstantiatedReviveMe.transform.parent = base.transform;
		m_InstantiatedReviveMe.transform.localPosition = new Vector3(vector.x, vector.y, m_AssetController.HealthBarPosition.z);
		if (m_BattleMgr.m_BirdTurnStarted)
		{
			ToggleReviveme(true);
		}
	}

	public IEnumerator KnockOutCharacter(int score)
	{
		if (!m_AssetController.HasNoBoneAnimation)
		{
			m_AssetController.m_BoneAnimation.playAutomatically = false;
		}
		PlayGoToBasePosition();
		RemoveAllAppliedEffects();
		if (m_BattleMgr.Model.IsPvP && !m_Model.IsBanner)
		{
			m_Model.KnockOutOnDefeat = true;
			if (m_Model.KnockedOutSkill == null)
			{
				m_Model.KnockedOutSkill = new SkillGameData("pvp_support_knockout_revive_self").GenerateSkillBattleData();
			}
			if (m_Model.CombatantFaction == Faction.Pigs && score > 0)
			{
				ShowScore(score);
			}
		}
		yield return new WaitForSeconds(PlayKnockOutCharacter());
		if (m_Model.KnockedOutSkill != null)
		{
			m_Model.KnockOutOnDefeat = true;
			if ((bool)m_Model.CombatantView.m_AssetController)
			{
				m_Model.CombatantView.m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(m_Model.KnockedOutSkill.BoneAnimationUserTrigger);
				m_Model.CombatantView.m_AssetController.m_BoneAnimation.RegisterUserTriggerDelegate(m_Model.KnockedOutSkill.BoneAnimationUserTrigger);
			}
			m_Model.AttackTarget = m_Model;
			yield return StartCoroutine(m_Model.KnockedOutSkill.DoAction(m_BattleMgr.Model, m_Model, m_Model.AttackTarget));
			if ((bool)m_Model.CombatantView.m_AssetController)
			{
				m_Model.CombatantView.m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(m_Model.KnockedOutSkill.BoneAnimationUserTrigger);
			}
		}
		if (m_Model is BirdCombatant && !m_BattleMgr.Model.IsPvP)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void RemoveAllAppliedEffects()
	{
		if (m_Model.CurrrentEffects != null)
		{
			List<BattleEffectGameData> list = m_Model.CurrrentEffects.Values.ToList();
			for (int num = list.Count - 1; num >= 0; num--)
			{
				list[num].RemoveEffect();
			}
		}
	}

	private void DestroyCharacter()
	{
		StartCoroutine(CheckIfDestroyBeforeEndOfTurn());
	}

	private IEnumerator CheckIfDestroyBeforeEndOfTurn()
	{
		while (m_BattleMgr.Model.CurrentCombatant == m_Model && !m_ReadyToDestroy)
		{
			yield return new WaitForEndOfFrame();
		}
		for (int i = 0; i < LastingVisualEffects.Count; i++)
		{
			GameObject effect = LastingVisualEffects.Values.ElementAt(i);
			if ((bool)effect)
			{
				UnityEngine.Object.Destroy(effect);
			}
		}
		if ((bool)m_AssetController)
		{
			DIContainerInfrastructure.GetCharacterAssetProvider(false).DestroyObject(m_Model.CombatantAssetId, m_AssetController.gameObject);
			m_AssetController = null;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void InstantiateChargeBubble(SkillGameData skill, List<ICombatant> targets, ICombatant source, int duration)
	{
		StartCoroutine(SpawnChargeBubble(skill, targets, source, duration));
	}

	private IEnumerator SpawnChargeBubble(SkillGameData skill, List<ICombatant> targets, ICombatant source, int duration)
	{
		CharacterControllerBattleGroundBase character = source.CombatantView;
		if (character == null || (!character.m_Model.IsParticipating && !character.m_Model.IsKnockedOut))
		{
			yield break;
		}
		CharacterSpeechBubble speechBubble = GenerateChargeBubble();
		SetupChargeBubble(duration, character, speechBubble);
		speechBubble.DisableTargetIcon(false);
		if (targets.Count > 1)
		{
			if (targets[0].CombatantFaction == Faction.Pigs)
			{
				speechBubble.SetTargetIcon("Target_Pigs");
			}
			else
			{
				speechBubble.SetTargetIcon("Target_Birds");
			}
		}
		else if (targets.Count == 1)
		{
			if (targets[0].CombatantFaction == Faction.Pigs)
			{
				speechBubble.SetTargetIcon("Target_Pig");
				yield break;
			}
			speechBubble.SetTargetIcon("Target_" + targets.FirstOrDefault().CombatantAssetId);
			speechBubble.m_IsTargetedBubble = true;
		}
	}

	public void InstantiateBubble(BubbleSetting m_BubbleSetting, List<ICombatant> m_Targets, ICombatant m_Source, int duration)
	{
		StartCoroutine(InstantiateBubbleCoroutine(m_BubbleSetting, m_Targets, m_Source, duration));
	}

	private IEnumerator InstantiateBubbleCoroutine(BubbleSetting setting, List<ICombatant> m_Targets, ICombatant m_Source, int duration)
	{
		List<CharacterControllerBattleGroundBase> targetedCharacters = new List<CharacterControllerBattleGroundBase>();
		CharacterControllerBattleGroundBase source = m_Source.CombatantView;
		if (m_Targets != null)
		{
			for (int i = 0; i < m_Targets.Count; i++)
			{
				targetedCharacters.Add(m_Targets[i].CombatantView);
			}
		}
		switch (setting.TargetCombatant)
		{
		case VisualEffectTargetCombatant.Origin:
			yield return StartCoroutine(SpawnBubblesOnSource(setting, targetedCharacters, source, duration));
			break;
		case VisualEffectTargetCombatant.Target:
			yield return StartCoroutine(SpawnBubblesOnTargets(setting, targetedCharacters, source, duration));
			break;
		default:
			yield return StartCoroutine(SpawnBubblesOnTargets(setting, targetedCharacters, source, duration));
			break;
		}
	}

	public void ResetBubbleTargetIcon()
	{
		if (m_SpeechBubbles != null)
		{
			CharacterSpeechBubble characterSpeechBubble = m_SpeechBubbles.Values.FirstOrDefault((CharacterSpeechBubble b) => b.m_IsTargetedBubble);
		}
	}

	private IEnumerator SpawnBubblesOnTargets(BubbleSetting setting, List<CharacterControllerBattleGroundBase> targets, CharacterControllerBattleGroundBase source, int duration)
	{
		for (int i = 0; i < targets.Count; i++)
		{
			CharacterControllerBattleGroundBase character = targets[i];
			if (character == null || !character.m_Model.IsParticipating || character.m_GoalMarkerBubble != null)
			{
				continue;
			}
			if (setting.CharacterIcon)
			{
				CharacterSpeechBubble targetingBubble = character.m_SpeechBubbles.Values.FirstOrDefault();
				if (targetingBubble != null)
				{
					if (targetingBubble.m_IsTargetedBubble)
					{
						targetingBubble.SetTargetIcon("Target_" + source.m_Model.CombatantAssetId);
						targetingBubble.UpdateSkill();
					}
					continue;
				}
			}
			CharacterSpeechBubble speechBubble = GenerateBubble(setting);
			SetupBubble(setting, duration, character, speechBubble);
			if (setting.CharacterIcon)
			{
				speechBubble.SetTargetIcon("Target_" + source.m_Model.CombatantAssetId);
			}
			switch (setting.Type)
			{
			case BubbleType.Large:
				yield return new WaitForSeconds(speechBubble.ShowAndMinimize());
				break;
			case BubbleType.Text:
				yield return new WaitForSeconds(speechBubble.ShowAndMinimize());
				break;
			case BubbleType.Icon:
				yield return new WaitForSeconds(speechBubble.ShowAndMinimize());
				break;
			case BubbleType.Targeted:
				speechBubble.PlayEmotion();
				speechBubble.RemoveBubble();
				character.m_SpeechBubbles.Remove(setting.BalancingId);
				break;
			default:
				yield return new WaitForSeconds(speechBubble.ShowAndMinimize());
				break;
			}
		}
	}

	private IEnumerator SpawnBubblesOnSource(BubbleSetting setting, List<CharacterControllerBattleGroundBase> targets, CharacterControllerBattleGroundBase source, int duration)
	{
		if (source == null || (!source.m_Model.IsParticipating && !source.m_Model.IsKnockedOut))
		{
			yield break;
		}
		CharacterSpeechBubble speechBubble = GenerateBubble(setting);
		SetupBubble(setting, duration, source, speechBubble);
		if (setting.CharacterIcon)
		{
			speechBubble.DisableTargetIcon(false);
			if (setting.AtAll)
			{
				if (setting.AtPigs)
				{
					speechBubble.SetTargetIcon("Target_Pigs");
				}
				else
				{
					speechBubble.SetTargetIcon("Target_Birds");
				}
			}
			else if (setting.AtPigs)
			{
				speechBubble.SetTargetIcon("Target_Pig");
			}
			else
			{
				speechBubble.SetTargetIcon("Target_" + targets.FirstOrDefault().m_Model.CombatantAssetId);
				speechBubble.m_IsTargetedBubble = true;
			}
		}
		else
		{
			speechBubble.DisableTargetIcon(true);
		}
		switch (setting.Type)
		{
		case BubbleType.Large:
			yield return new WaitForSeconds(speechBubble.ShowAndMinimize());
			break;
		case BubbleType.Text:
			yield return new WaitForSeconds(speechBubble.ShowAndMinimize());
			break;
		case BubbleType.Icon:
			yield return new WaitForSeconds(speechBubble.ShowAndMinimize());
			break;
		case BubbleType.Targeted:
			speechBubble.PlayEmotion();
			speechBubble.RemoveBubble();
			source.m_SpeechBubbles.Remove(setting.BalancingId);
			break;
		default:
			yield return new WaitForSeconds(speechBubble.ShowAndMinimize());
			break;
		}
	}

	private void SetupChargeBubble(int duration, CharacterControllerBattleGroundBase character, CharacterSpeechBubble speechBubble)
	{
		PositionBubble(character, speechBubble.gameObject);
		if (character.m_SpeechBubbles.ContainsKey("Charge"))
		{
			UnityEngine.Object.Destroy(character.m_SpeechBubbles["Charge"].gameObject);
			character.m_SpeechBubbles.Remove("Charge");
		}
		character.m_SpeechBubbles.Add("Charge", speechBubble);
		speechBubble.SetModel(character.m_Model, null, duration);
		if (IsAssetAtWrongSide())
		{
			speechBubble.transform.localScale = new Vector3(0f - speechBubble.transform.localScale.x, speechBubble.transform.localScale.y, speechBubble.transform.localScale.z);
		}
	}

	public bool IsAssetAtWrongSide()
	{
		return AssetIsOnWrongSide(m_AssetController.m_AssetFaction, m_Model.CombatantFaction);
	}

	protected bool AssetIsOnWrongSide(AssetFaction assetFaction, Faction faction)
	{
		switch (assetFaction)
		{
		case AssetFaction.Pig:
			return faction == Faction.Birds;
		case AssetFaction.Bird:
			return faction == Faction.Pigs;
		default:
			return false;
		}
	}

	private void SetupBubble(BubbleSetting setting, int duration, CharacterControllerBattleGroundBase character, CharacterSpeechBubble speechBubble)
	{
		PositionBubble(character, speechBubble.gameObject);
		speechBubble.m_ReverseFill = setting.ReverseFill;
		if (character.m_SpeechBubbles.ContainsKey(setting.BalancingId))
		{
			UnityEngine.Object.Destroy(character.m_SpeechBubbles[setting.BalancingId].gameObject);
			character.m_SpeechBubbles.Remove(setting.BalancingId);
		}
		character.m_SpeechBubbles.Add(setting.BalancingId, speechBubble);
		speechBubble.SetModel(character.m_Model, string.Empty, duration);
		if (AssetIsOnWrongSide(m_AssetController.m_AssetFaction, m_Model.CombatantFaction))
		{
			speechBubble.transform.localScale = new Vector3(0f - speechBubble.transform.localScale.x, speechBubble.transform.localScale.y, speechBubble.transform.localScale.z);
		}
	}

	public void PositionBubble(CharacterControllerBattleGroundBase character, GameObject speechBubble)
	{
		speechBubble.transform.parent = character.transform;
		speechBubble.transform.localScale = Vector3.one / character.m_Model.CharacterModel.Scale;
		Vector3 vector = Vector3.Scale(character.m_AssetController.BubblePosition, m_AssetController.transform.localScale);
		speechBubble.transform.localPosition = new Vector3(vector.x, vector.y, character.m_AssetController.BubblePosition.z);
		speechBubble.transform.position = new Vector3(speechBubble.transform.position.x, speechBubble.transform.position.y, Mathf.Max(character.m_AssetController.BubblePosition.z, -600f));
		speechBubble.transform.localRotation = Quaternion.identity;
	}

	private CharacterSpeechBubble GenerateBubble(BubbleSetting setting)
	{
		CharacterSpeechBubble characterSpeechBubble;
		switch (setting.Type)
		{
		case BubbleType.Large:
			characterSpeechBubble = UnityEngine.Object.Instantiate(DIContainerLogic.GetVisualEffectsBalancing().LargeBubble);
			break;
		case BubbleType.Text:
			characterSpeechBubble = UnityEngine.Object.Instantiate(DIContainerLogic.GetVisualEffectsBalancing().OnlyTimerBubble);
			break;
		case BubbleType.Icon:
			characterSpeechBubble = UnityEngine.Object.Instantiate(DIContainerLogic.GetVisualEffectsBalancing().OnlyIconBubble);
			break;
		case BubbleType.Targeted:
			characterSpeechBubble = UnityEngine.Object.Instantiate(DIContainerLogic.GetVisualEffectsBalancing().TargetedBubble);
			break;
		case BubbleType.KnockedOut:
			characterSpeechBubble = UnityEngine.Object.Instantiate(DIContainerLogic.GetVisualEffectsBalancing().KoBubble);
			break;
		default:
			characterSpeechBubble = UnityEngine.Object.Instantiate(DIContainerLogic.GetVisualEffectsBalancing().LargeBubble);
			break;
		}
		SetLayerRecusively(characterSpeechBubble.gameObject, base.gameObject.layer);
		return characterSpeechBubble;
	}

	private CharacterSpeechBubble GenerateChargeBubble()
	{
		CharacterSpeechBubble characterSpeechBubble = UnityEngine.Object.Instantiate(DIContainerLogic.GetVisualEffectsBalancing().LargeBubble);
		SetLayerRecusively(characterSpeechBubble.gameObject, base.gameObject.layer);
		return characterSpeechBubble;
	}

	private void SetLayerRecusively(GameObject go, int layer)
	{
		go.layer = layer;
		for (int i = 0; i < go.transform.childCount; i++)
		{
			SetLayerRecusively(go.transform.GetChild(i).gameObject, layer);
		}
	}

	public virtual void RefreshFromStun()
	{
	}

	public void InitCounterAttack(ICombatant attacker, ICombatant target, ICombatant victim, float damageModifier)
	{
		if (attacker == null || !attacker.IsAlive || (!attacker.CombatantView.m_CounterAttack && !attacker.CombatantView.m_AssetController.m_IsIllusion))
		{
			DebugLog.Log("Try init Counter!");
			m_CounterModel = victim;
			m_CounterSkillToDo = victim.GetSkill(0);
			m_CounterModelTarget = target;
			if (m_CounterModel == null || m_CounterModel.IsStunned || !m_CounterModel.IsParticipating)
			{
				DebugLog.Log("Init Counter invalid!");
				m_CounterModel = null;
				m_CounterSkillToDo = null;
				m_CounterModelTarget = null;
			}
			else
			{
				DebugLog.Log("Init Counter valid!");
				m_CounterAttack = true;
				m_CounterModel.DamageModifier = damageModifier / 100f;
			}
		}
	}

	public IEnumerator CounterAttack()
	{
		ICombatant cachedTarget = m_CounterModel.AttackTarget;
		m_CounterModel.AttackTarget = m_CounterModelTarget;
		DebugLog.Log("Try start Counter!");
		if (m_CounterModel.IsStunned || m_CounterModel.AttackTarget == null || !m_CounterModel.IsParticipating || !m_CounterModel.AttackTarget.IsParticipating)
		{
			DebugLog.Log("Counter invalid!");
			m_CounterModel.DamageModifier = 1f;
			m_CounterAttack = false;
			m_CounterModel.AttackTarget = cachedTarget;
			m_CounterModel = null;
			m_CounterModelTarget = null;
			yield break;
		}
		DebugLog.Log("Counter valid!");
		VisualEffectSetting setting = null;
		if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting("BonusAttack", out setting))
		{
			DIContainerLogic.GetBattleService().SpawnVisualEffects(VisualEffectSpawnTiming.Triggered, setting, new List<ICombatant> { m_CounterModel.AttackTarget }, m_CounterModel, false);
		}
		yield return new WaitForSeconds(PlayGoToFocusPosition());
		if (m_CounterModel.AttackTarget != m_CounterModelTarget)
		{
			cachedTarget = m_CounterModel.AttackTarget;
			m_CounterModel.AttackTarget = m_CounterModelTarget;
		}
		if ((bool)m_AssetController)
		{
			m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(m_CounterSkillToDo.BoneAnimationUserTrigger);
			m_AssetController.m_BoneAnimation.RegisterUserTriggerDelegate(m_CounterSkillToDo.BoneAnimationUserTrigger);
		}
		yield return StartCoroutine(m_CounterSkillToDo.DoAction(m_BattleMgr.Model, m_Model, m_CounterModel.AttackTarget));
		BattleEffectGameData chargeEffect = null;
		if (m_CounterModel.CurrrentEffects != null && m_CounterModel.CurrrentEffects.TryGetValue("Charge_" + m_CounterSkillToDo.Model.Balancing.AssetId, out chargeEffect) && chargeEffect.GetTurnsLeft() == m_CounterSkillToDo.GetChargeDuration())
		{
			m_SkillToDo = m_CounterSkillToDo;
			cachedTarget = m_CounterModel.AttackTarget;
			if (m_CounterSkillToDo.EvaluateCharge(m_BattleMgr.Model, m_Model, new List<ICombatant> { m_CounterModel.AttackTarget }, null))
			{
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForPigToChooseTargetAndDoSkillInSec);
			}
		}
		if ((bool)m_AssetController)
		{
			m_AssetController.m_BoneAnimation.UnregisterUserTriggerDelegate(m_CounterSkillToDo.BoneAnimationUserTrigger);
		}
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeForShowDamageAndReturnToBasePosInSec);
		if (m_CounterModel != null)
		{
			m_CounterModel.DamageModifier = 1f;
		}
		if (!m_Model.IsParticipating)
		{
			m_CounterAttack = false;
			m_CounterModelTarget = null;
			m_CounterModel.AttackTarget = cachedTarget;
			yield break;
		}
		if (m_Model.CombatantFaction == Faction.Birds && m_BattleMgr.m_BirdTurnStarted)
		{
			yield return new WaitForSeconds(PlayGoToFocusPosition() * DIContainerLogic.GetPacingBalancing().WaitFactorForReturnToBasePos);
		}
		else
		{
			yield return new WaitForSeconds(PlayGoToBasePosition() * DIContainerLogic.GetPacingBalancing().WaitFactorForReturnToBasePos);
		}
		if (targetSheltered != null)
		{
			yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeAfterSheltering);
			targetSheltered.CombatantView.PlayGoToBasePosition();
			m_Model.AttackTarget.CombatantView.PlayGoToBasePosition();
			targetSheltered = null;
		}
		m_CounterAttack = false;
		if (m_CounterModel != null && m_CounterModel.AttackTarget == m_CounterModelTarget)
		{
			m_CounterModel.AttackTarget = cachedTarget;
		}
		m_CounterModelTarget = null;
	}

	public virtual void DisableGlow()
	{
	}

	public void ShowScore(int score)
	{
		if (score != 0)
		{
			DebugLog.Log("Show score for " + m_Model.CombatantName + ": " + score);
			ScorePopup scorePopup = UnityEngine.Object.Instantiate(m_ScorePopup);
			scorePopup.transform.position = m_CachedTransform.position + Vector3.Scale(m_AssetController.CombatTextPosition, m_AssetController.transform.localScale);
			scorePopup.ShowScore(score, m_BattleMgr.Model, GetComponentInChildren<CharacterHealthBar>(), m_Model.CombatantFaction);
		}
	}

	public void ToggleReviveme(bool show)
	{
		if (!m_BattleMgr.Model.IsPvP && m_InstantiatedReviveMe != null)
		{
			m_InstantiatedReviveMe.SetActive(true);
			if (show)
			{
				m_InstantiatedReviveMe.PlayAnimationOrAnimatorState("ReviveIndicator_Show");
			}
			if (!show)
			{
				m_InstantiatedReviveMe.PlayAnimationOrAnimatorState("ReviveIndicator_Hide");
			}
		}
	}

	public void CleanBlessingsWithDelay(float delay)
	{
		StartCoroutine(CleanBlessingsWithDelayCoroutine(delay));
	}

	public void CleanCursesWithDelay(float delay)
	{
		StartCoroutine(CleanCursesWithDelayCoroutine(delay));
	}

	private IEnumerator CleanBlessingsWithDelayCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		DIContainerLogic.GetBattleService().RemoveBattleEffects(GetModel(), SkillEffectTypes.Blessing, true);
	}

	private IEnumerator CleanCursesWithDelayCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		DIContainerLogic.GetBattleService().RemoveBattleEffects(GetModel(), SkillEffectTypes.Curse, true);
	}

	public virtual void UpdateHealthBar()
	{
	}

	public void GrowCharacter(float growValue)
	{
		if (m_AssetController != null && m_AssetController.gameObject.GetComponent<ScaleController>() != null)
		{
			m_AssetController.gameObject.GetComponent<ScaleController>().m_BaseScale += m_StartSize * growValue;
		}
	}
}
