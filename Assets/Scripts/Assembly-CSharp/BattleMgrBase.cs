using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public abstract class BattleMgrBase : MonoBehaviour
{
	[SerializeField]
	private BossSlot[] m_BossSlot;

	[SerializeField]
	private GameObject m_TinkerBossIntroPig;

	[SerializeField]
	private GameObject m_KrakenBossIntroPig;

	protected BattleGameData m_Model;

	public bool PlacedCharactersAtLeastOnce;

	[HideInInspector]
	public BattleUIStateMgr m_BattleUI;

	[HideInInspector]
	public BattlePausedPopup m_BattlePaused;

	[HideInInspector]
	public bool WaveEnded;

	public Transform m_BattlegroundCenterPosition;

	public Transform m_BirdCenterPosition;

	public Transform m_PigCenterPosition;

	public Transform m_UpperBorder;

	public Transform m_BirdBannerPosition;

	public Transform m_PigBannerPosition;

	public Transform m_BirdFocusPosition;

	public Transform m_PigFocusPosition;

	public Transform m_BattleSetup;

	public Transform m_BattleArea;

	public Transform m_BattleAreaPositioning;

	public Func<IEnumerator> m_CurrentSummonAction;

	public GameObject m_CameraAnimationRoot;

	public GameObject m_ScreenFxRoot;

	public CharacterControllerBattleGroundBase m_CharacterControllerBattlegroundPrefab;

	public float m_StartPosOffset = 500f;

	public bool m_blocked;

	public bool m_HorizontalPositioning;

	public float m_PlaneDegree = 30f;

	public CharacterControllerBattleGroundBase m_DraggedCharacter;

	public bool m_ShowRevivePopup;

	public bool m_HasRevived;

	public float m_ShowRevivePopupTimeLeft;

	public bool m_HasConsumables;

	public bool m_BirdTurnStarted;

	private bool m_AutoBattle;

	private BossSlot m_correctBossSlot;

	public bool m_bonusXpGained;

	public bool m_IsRagedBlocked;

	public bool m_IsInShopForRevive;

	public bool m_ForcedCheckProgress;

	public bool m_LockControlHUDs;

	public bool m_BattleMainLoopDone;

	public List<GameObject> m_CharacterInteractionBlockedItems = new List<GameObject>();

	public CharacterControlHUD m_CurrentControlHUD;

	public GlowController m_CurrentGlow;

	public GameObject m_CurrentConsumableControlHUDRoot;

	public Camera m_SceneryCamera;

	public Camera m_InterfaceCamera;

	protected bool m_Ended;

	public ScaleMgr m_ScaleMgr;

	public Queue<string> actingCharacters = new Queue<string>();

	public CharacterAssetController m_IllusionistCopy;

	public ICombatant m_CurrentOriginal;

	public float m_IllusionistDamageFactor;

	public ICombatant m_IllusionistCombatant;

	private bool m_lockDragVisualizationByCode;

	private bool m_IsConsumableUsePossible;

	protected bool m_isAutoBattleUnlocked;

	public BattleGameData Model
	{
		get
		{
			return m_Model;
		}
	}

	public bool AutoBattle
	{
		get
		{
			return m_AutoBattle;
		}
		set
		{
			if (!DIContainerInfrastructure.TutorialMgr.IsCurrentlyLocked && m_isAutoBattleUnlocked)
			{
				m_AutoBattle = value;
			}
			else
			{
				m_AutoBattle = false;
			}
		}
	}

	public bool RestedBonus
	{
		get
		{
			return false;
		}
	}

	public bool LockDragVisualizationByCode
	{
		get
		{
			return m_lockDragVisualizationByCode;
		}
		set
		{
			if (this.DragVisualizationLocked != null)
			{
				this.DragVisualizationLocked(value);
			}
			m_lockDragVisualizationByCode = value;
		}
	}

	public bool CounterRunning
	{
		get
		{
			return Model.m_CombatantsByInitiative.Any((ICombatant c) => c.IsParticipating && c.CombatantView != null && c.CombatantView.m_CounterAttack);
		}
	}

	public bool IsConsumableUsePossible
	{
		get
		{
			return m_HasConsumables && m_IsConsumableUsePossible && !m_Model.IsPvP;
		}
		set
		{
			m_IsConsumableUsePossible = value;
		}
	}

	public bool IsRagemeterUsePossible
	{
		get
		{
			return m_IsConsumableUsePossible;
		}
	}

	public bool IsPausePossible { get; set; }

	public bool IsAutoBattlePossible
	{
		get
		{
			return !DIContainerInfrastructure.TutorialMgr.IsCurrentlyLocked && IsPausePossible && m_isAutoBattleUnlocked && !Model.IsPvP;
		}
	}

	[method: MethodImpl(32)]
	public event Action<bool> DragVisualizationLocked;

	public bool CharacterIsActing(string nameId)
	{
		if (!CounterRunning && (actingCharacters == null || actingCharacters.Count <= 0))
		{
			return false;
		}
		return CounterRunning || actingCharacters.Peek() == nameId;
	}

	public bool CounterIsRunning()
	{
		return CounterRunning;
	}

	public bool CharacterIsActingOrInQueue(string nameId)
	{
		return CounterRunning || (actingCharacters != null && actingCharacters.Contains(nameId));
	}

	protected virtual IEnumerator SetupInitialPositions()
	{
		int pigCount = (m_Model.m_CombatantsPerFaction.ContainsKey(Faction.Pigs) ? m_Model.m_CombatantsPerFaction[Faction.Pigs].Count : 0);
		float maxDistance = 300f;
		float startPos = maxDistance / 2f;
		float offsetPos = maxDistance / (float)pigCount;
		yield return StartCoroutine(PlaceCharacter(m_BirdCenterPosition, Faction.Birds));
		yield return StartCoroutine(PlaceCharacter(m_PigCenterPosition, Faction.Pigs));
		float summedEnterDelay = (float)m_Model.m_CombatantsByInitiative.Count * DIContainerLogic.GetPacingBalancing().PerCombatantEnterDelay + DIContainerLogic.GetPacingBalancing().BaseBattleEnterDelay;
		StartCoroutine(EnterCombatants());
		yield return new WaitForSeconds(summedEnterDelay);
		SpawnHealthBars();
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().AfterEnterBattleGroundDelay - summedEnterDelay);
	}

	public void SpawnHealthBars()
	{
		for (int i = 0; i < m_Model.m_CombatantsByInitiative.Count; i++)
		{
			m_Model.m_CombatantsByInitiative[i].CombatantView.SpawnHealthBar();
		}
	}

	public bool IsBattleEnded()
	{
		Faction faction = DIContainerLogic.GetBattleService().EvaluateVictoryCondition(Model);
		return faction != Faction.None;
	}

	public void ReviveAllBirds()
	{
		m_HasRevived = true;
		m_Ended = false;
		IsPausePossible = true;
		List<ICombatant> list = Model.m_CombatantsPerFaction[Faction.Birds];
		for (int i = 0; i < list.Count; i++)
		{
			ICombatant combatant = list[i];
			combatant.CombatantView.gameObject.SetActive(true);
			combatant.ActedThisTurn = false;
			combatant.CombatantView.m_IsWaitingForInput = true;
			combatant.CombatantView.ReviveCombatant(combatant, 100f);
		}
		for (int j = 0; j < Model.m_CombatantsPerFaction[Faction.Pigs].Count; j++)
		{
			ICombatant combatant2 = Model.m_CombatantsPerFaction[Faction.Pigs][j];
			combatant2.ActedThisTurn = false;
			combatant2.StartedHisTurn = false;
			combatant2.CombatantView.PlayGoToBasePosition();
		}
		DIContainerLogic.GetBattleService().ClearInitiative(Model);
		StartCoroutine(PlaceCharacter(m_BirdCenterPosition, Faction.Birds));
		Model.m_BattleEndData.m_ReviveUsed++;
		for (int k = 0; k < list.Count; k++)
		{
			ICombatant combatant3 = list[k];
			combatant3.CombatantView.SpawnHealthBar();
			if (m_Model.m_SponsoredEnvironmentalEffect != null)
			{
				m_Model.m_SponsoredEnvironmentalEffect.GenerateSkillBattleData().DoActionInstant(m_Model, combatant3, combatant3);
			}
		}
		if (DIContainerLogic.InventoryService.GetItemValue(m_Model.m_ControllerInventory, "xp_multiplier_consumable_01") > 0)
		{
			VisualEffectSetting setting = null;
			string ident = "Consumable_XP";
			if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(ident, out setting))
			{
				DebugLog.Log("Try to instantiate XP doubler effect");
				StartCoroutine(DelayedInstantiateXPEffect(list, setting));
			}
		}
		m_ShowRevivePopup = false;
	}

	public void ReviveBird(ICombatant bird)
	{
		IsPausePossible = true;
		bird.CombatantView.gameObject.SetActive(true);
		bird.CombatantView.ReviveCombatant(bird, 100f);
		bird.KnockOutOnDefeat = true;
		StartCoroutine(StartCombatantTurnImmeadiatly(bird));
		StartCoroutine(PlaceCharacter(m_BirdCenterPosition, Faction.Birds));
		Model.m_BattleEndData.m_ReviveUsed++;
		bird.CombatantView.SpawnHealthBar();
		if (DIContainerLogic.InventoryService.GetItemValue(m_Model.m_ControllerInventory, "xp_multiplier_consumable_01") > 0)
		{
			VisualEffectSetting setting = null;
			string ident = "Consumable_XP";
			if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(ident, out setting))
			{
				DebugLog.Log("Try to instantiate XP doubler effect");
				StartCoroutine(DelayedInstantiateXPEffect(new List<ICombatant> { bird }, setting));
			}
		}
	}

	private IEnumerator DelayedInstantiateXPEffect(List<ICombatant> allBirds, VisualEffectSetting setting)
	{
		yield return new WaitForSeconds(0.3f);
		StartCoroutine(InstantiateEffects(allBirds.FirstOrDefault(), setting.VisualEffects[1], setting, allBirds, false));
	}

	public IEnumerator QueueOrExecuteCharacterAction(string characterId, IEnumerator actionToContiniue, bool autoBattle)
	{
		if (!actingCharacters.Contains(characterId))
		{
			actingCharacters.Enqueue(characterId);
			IsConsumableUsePossible = false;
			DebugLog.Log("Try to start turn! " + characterId);
			if (CounterRunning || (actingCharacters.Count > 0 && actingCharacters.Peek() != characterId))
			{
				DebugLog.Log("Other Character is acting: " + actingCharacters.Peek());
			}
			while (CounterRunning || (actingCharacters.Count > 0 && actingCharacters.Peek() != characterId))
			{
				yield return new WaitForEndOfFrame();
			}
			yield return StartCoroutine(actionToContiniue);
			string dequeuedCharacter = string.Empty;
			if (actingCharacters.Count > 0)
			{
				dequeuedCharacter = actingCharacters.Dequeue();
			}
			if (actingCharacters.Count == 0 && CanCharactersAct(1, dequeuedCharacter))
			{
				IsConsumableUsePossible = true;
			}
		}
		else
		{
			DebugLog.Error("Character " + characterId + " is already in the queue");
		}
	}

	public bool CanCharactersAct(int count, string dequeuedCharacter)
	{
		return Model.m_CombatantsByInitiative.Count((ICombatant c) => c.CombatantNameId != dequeuedCharacter && c.IsParticipating && c.CombatantFaction == Faction.Birds && !c.ActedThisTurn) >= count;
	}

	public virtual IEnumerator PlaceCharacterBoss(Transform centerTransform, Vector3 offset, bool firstSpawn)
	{
		if (offset == Vector3.zero && m_correctBossSlot != null)
		{
			offset = m_correctBossSlot.GetComponentInChildren<BossAssetController>().m_SpawnOffset;
		}
		ScaleMgr scaleMgr = base.transform.GetComponentInChildren<ScaleMgr>();
		List<ICombatant> factionCombatants = m_Model.m_CombatantsPerFaction[Faction.Pigs].Where((ICombatant c) => c.IsParticipating || c.IsKnockedOut).ToList();
		int combatantCount = factionCombatants.Count;
		List<CharacterControllerBattleGroundBase> m_Characters = new List<CharacterControllerBattleGroundBase>();
		for (int i = 0; i < combatantCount; i++)
		{
			if (factionCombatants[i].CombatantView != null)
			{
				factionCombatants[i].CombatantView.m_AlreadyPlaced = true;
				m_Characters.Add(factionCombatants[i].CombatantView);
				continue;
			}
			CharacterControllerBattleGroundBase ccontr = UnityEngine.Object.Instantiate(m_CharacterControllerBattlegroundPrefab, centerTransform.position, Quaternion.identity) as CharacterControllerBattleGroundBase;
			ccontr.transform.parent = m_BattleArea;
			ccontr.SetModel(factionCombatants[i], this);
			UnityHelper.SetLayerRecusively(ccontr.gameObject, LayerMask.NameToLayer("Scenery"));
			ccontr.PlayIdle();
			yield return new WaitForEndOfFrame();
			ccontr.gameObject.SetActive(false);
			m_Characters.Add(ccontr);
		}
		factionCombatants = factionCombatants.OrderByDescending((ICombatant c) => c is BossCombatant).ToList();
		foreach (ICombatant character in factionCombatants)
		{
			if (character.CombatantView.gameObject.activeSelf)
			{
				continue;
			}
			character.CombatantView.gameObject.SetActive(true);
			character.CombatantView.SetModel(character, this);
			if (character is BossCombatant)
			{
				Transform BossPositionTransform = null;
				BossSlot[] bossSlot = m_BossSlot;
				foreach (BossSlot child in bossSlot)
				{
					if (child.transform.name == character.CharacterModel.AssetName)
					{
						m_correctBossSlot = child;
						BossPositionTransform = child.transform;
						break;
					}
				}
				if (BossPositionTransform != null)
				{
					character.CombatantView.transform.parent = BossPositionTransform;
					character.CombatantView.transform.localPosition = new Vector3(600f, 0f, 0f);
				}
				continue;
			}
			bool freeSlot = false;
			if (m_correctBossSlot == null)
			{
				UnityEngine.Object.Destroy(character.CombatantView.gameObject);
				yield break;
			}
			foreach (Transform slot in m_correctBossSlot.m_MinionSlots)
			{
				if (slot.childCount == 0)
				{
					character.CombatantView.transform.parent = slot;
					if (m_correctBossSlot.GetComponentInChildren<BossAssetController>().m_ImmovableMinions)
					{
						character.CombatantView.transform.localPosition = Vector3.zero;
						character.CombatantView.m_minSpawnDelay = ((!firstSpawn) ? 0f : 2f);
						character.CombatantView.m_maxSpawnDelay = ((!firstSpawn) ? 0f : 4f);
						character.CombatantView.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
					}
					else
					{
						character.CombatantView.transform.position = centerTransform.position + offset;
					}
					character.CombatantView.SetCachedUnfocusedPos(slot.position);
					freeSlot = true;
					break;
				}
			}
			if (freeSlot)
			{
				continue;
			}
			m_correctBossSlot.transform.GetChild(0).GetComponentInChildren<BossAssetController>().PlayMournAnim();
			m_Model.m_CombatantsPerFaction[character.CombatantFaction].Remove(character);
			UnityEngine.Object.Destroy(character.CombatantView.gameObject);
			yield break;
		}
		yield return new WaitForEndOfFrame();
		foreach (ICombatant factionCombatant in factionCombatants)
		{
			factionCombatant.CombatantView.m_StartPositionY = factionCombatant.CombatantView.CachedUnfocusedPos.z;
		}
		List<ICombatant> newOrder = factionCombatants.OrderByDescending((ICombatant c) => c.CombatantView.m_StartPositionY).ToList();
		DIContainerLogic.GetBattleService().ReplaceInitiative(Model, newOrder, Faction.Pigs);
		DIContainerLogic.GetBattleService().ReSetCurrentInitiative(Model);
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		if (player.Data.BossIntrosPlayed == null)
		{
			player.Data.BossIntrosPlayed = new List<string>();
		}
		if (!player.Data.BossIntrosPlayed.Contains(m_correctBossSlot.name))
		{
			player.Data.BossIntrosPlayed.Add(m_correctBossSlot.name);
			yield return StartCoroutine(PlayBossIntro(m_correctBossSlot.name));
		}
		yield return new WaitForSeconds(1f);
		PlacedCharactersAtLeastOnce = true;
		player.SavePlayerData();
	}

	public virtual IEnumerator PlaceCharacter(Transform centerTransform, Faction faction)
	{
		List<ICombatant> factionCombatants = m_Model.m_CombatantsPerFaction[faction].Where((ICombatant c) => c.IsParticipating || (c is PigCombatant && c.IsKnockedOut)).ToList();
		for (int i = 0; i < factionCombatants.Count; i++)
		{
			ICombatant comb = factionCombatants[i];
			if (comb is BossCombatant)
			{
				yield return StartCoroutine(PlaceCharacterBoss(centerTransform, Vector3.zero, true));
				yield break;
			}
		}
		ScaleMgr scaleMgr = base.transform.GetComponentInChildren<ScaleMgr>();
		int combatantCount = factionCombatants.Count;
		List<CharacterControllerBattleGroundBase> m_Characters = new List<CharacterControllerBattleGroundBase>();
		List<List<CharacterControllerBattleGroundBase>> m_CharactersPerRow = new List<List<CharacterControllerBattleGroundBase>>();
		List<List<Rect>> m_Rows = new List<List<Rect>>();
		List<CharacterControllerBattleGroundBase> m_SecondRow = new List<CharacterControllerBattleGroundBase>();
		ContainerControl cc = centerTransform.GetComponent<ContainerControl>();
		Vector2 size = new Vector2(cc.m_Size.x, cc.m_Size.y);
		float planeDegree = Vector2.Angle(to: new Vector2(scaleMgr.m_Near_z_Border, scaleMgr.m_Far_z_Border).normalized, from: Vector2.up) + 45f;
		DebugLog.Log("Plane Degree: " + planeDegree);
		Vector3 sizeDiff = new Vector3(size.x, size.y) - Quaternion.Euler(0f - planeDegree, 0f, 0f) * size;
		size += new Vector2(sizeDiff.x, sizeDiff.y);
		float maxDistance = size.y;
		float maxShift = size.x;
		for (int k = 0; k < combatantCount; k++)
		{
			if (factionCombatants[k].CombatantView != null)
			{
				factionCombatants[k].CombatantView.m_AlreadyPlaced = true;
				m_Characters.Add(factionCombatants[k].CombatantView);
				continue;
			}
			CharacterControllerBattleGroundBase ccontr = UnityEngine.Object.Instantiate(m_CharacterControllerBattlegroundPrefab, centerTransform.position, Quaternion.identity) as CharacterControllerBattleGroundBase;
			ccontr.transform.parent = m_BattleArea;
			ccontr.SetModel(factionCombatants[k], this);
			UnityHelper.SetLayerRecusively(ccontr.gameObject, LayerMask.NameToLayer("Scenery"));
			ccontr.gameObject.SetActive(false);
			m_Characters.Add(ccontr);
		}
		yield return new WaitForEndOfFrame();
		int row = 0;
		float minOffset = 50f;
		List<Vector3> m_RowBounds = new List<Vector3>();
		if (Model.IsPvP)
		{
			switch (combatantCount)
			{
			case 4:
			{
				CharacterControllerBattleGroundBase ccontr5 = m_Characters[0];
				CharacterControllerBattleGroundBase ccontr8 = m_Characters[1];
				CharacterControllerBattleGroundBase ccontr10 = m_Characters[2];
				CharacterControllerBattleGroundBase cbanner = m_Characters[3];
				m_Rows.Add(new List<Rect>());
				m_Rows.Add(new List<Rect>());
				m_Rows.Add(new List<Rect>());
				m_RowBounds.Add(new Vector3(0f, 0f, 0f));
				m_RowBounds.Add(new Vector3(0f, 0f, 0f));
				m_RowBounds.Add(new Vector3(0f, 0f, 0f));
				m_CharactersPerRow.Add(new List<CharacterControllerBattleGroundBase>());
				m_CharactersPerRow.Add(new List<CharacterControllerBattleGroundBase>());
				m_CharactersPerRow.Add(new List<CharacterControllerBattleGroundBase>());
				m_RowBounds[1] += new Vector3(0f, ccontr5.Size.y, 0f);
				m_RowBounds[1] = new Vector3(Mathf.Max(m_RowBounds[1].x, ccontr5.Size.x), m_RowBounds[1].y, 0f);
				Rect position3 = new Rect(0f - ccontr5.Extents.x, 0f - (m_RowBounds[1].y - ccontr5.Extents.y + ccontr5.Center.y), ccontr5.Size.x, ccontr5.Size.y);
				m_CharactersPerRow[1].Add(ccontr5);
				m_Rows[1].Add(position3);
				m_RowBounds[0] += new Vector3(0f, ccontr8.Size.y, 0f);
				m_RowBounds[0] = new Vector3(Mathf.Max(m_RowBounds[0].x, ccontr8.Size.x), m_RowBounds[0].y, 0f);
				Rect position6 = new Rect(0f - ccontr5.Extents.x, 0f - (m_RowBounds[0].y - ccontr8.Extents.y + ccontr8.Center.y), ccontr8.Size.x, ccontr8.Size.y);
				m_CharactersPerRow[0].Add(ccontr8);
				m_Rows[0].Add(position6);
				m_RowBounds[1] += new Vector3(0f, ccontr10.Size.y, 0f);
				m_RowBounds[1] = new Vector3(Mathf.Max(m_RowBounds[1].x, ccontr10.Size.x), m_RowBounds[1].y, 0f);
				Rect position10 = new Rect(0f - ccontr5.Extents.x, 0f - (m_RowBounds[1].y - ccontr10.Extents.y + ccontr10.Center.y), ccontr10.Size.x, ccontr10.Size.y);
				m_CharactersPerRow[1].Add(ccontr10);
				m_Rows[1].Add(position10);
				m_RowBounds[2] += new Vector3(0f, cbanner.Size.y, 0f);
				m_RowBounds[2] = new Vector3(Mathf.Max(m_RowBounds[2].x, cbanner.Size.x), m_RowBounds[2].y, 0f);
				Rect position11 = new Rect(0f - cbanner.Extents.x, 0f - (m_RowBounds[2].y - cbanner.Extents.y + cbanner.Center.y), cbanner.Size.x, cbanner.Size.y);
				m_CharactersPerRow[2].Add(cbanner);
				m_Rows[2].Add(position11);
				break;
			}
			case 3:
			{
				CharacterControllerBattleGroundBase ccontr4 = m_Characters[0];
				CharacterControllerBattleGroundBase ccontr7 = m_Characters[1];
				CharacterControllerBattleGroundBase cBanner2 = m_Characters[2];
				m_Rows.Add(new List<Rect>());
				m_Rows.Add(new List<Rect>());
				m_RowBounds.Add(new Vector3(0f, 0f, 0f));
				m_RowBounds.Add(new Vector3(0f, 0f, 0f));
				m_CharactersPerRow.Add(new List<CharacterControllerBattleGroundBase>());
				m_CharactersPerRow.Add(new List<CharacterControllerBattleGroundBase>());
				m_RowBounds[0] += new Vector3(0f, ccontr4.Size.y, 0f);
				m_RowBounds[0] = new Vector3(Mathf.Max(m_RowBounds[0].x, ccontr4.Size.x), m_RowBounds[0].y, 0f);
				Rect position2 = new Rect(0f - ccontr4.Extents.x, 0f - (m_RowBounds[0].y - ccontr4.Extents.y + ccontr4.Center.y), ccontr4.Size.x, ccontr4.Size.y);
				m_CharactersPerRow[0].Add(ccontr4);
				m_Rows[0].Add(position2);
				m_RowBounds[0] += new Vector3(0f, ccontr7.Size.y, 0f);
				m_RowBounds[0] = new Vector3(Mathf.Max(m_RowBounds[0].x, ccontr7.Size.x), m_RowBounds[0].y, 0f);
				Rect position5 = new Rect(0f - ccontr4.Extents.x, 0f - (m_RowBounds[0].y - ccontr7.Extents.y + ccontr7.Center.y), ccontr7.Size.x, ccontr7.Size.y);
				m_CharactersPerRow[0].Add(ccontr7);
				m_Rows[0].Add(position5);
				m_RowBounds[1] += new Vector3(0f, cBanner2.Size.y, 0f);
				m_RowBounds[1] = new Vector3(Mathf.Max(m_RowBounds[1].x, cBanner2.Size.x), m_RowBounds[1].y, 0f);
				Rect position9 = new Rect(0f - ccontr4.Extents.x, 0f - (m_RowBounds[1].y - cBanner2.Extents.y + cBanner2.Center.y), cBanner2.Size.x, cBanner2.Size.y);
				m_CharactersPerRow[1].Add(cBanner2);
				m_Rows[1].Add(position9);
				break;
			}
			default:
			{
				CharacterControllerBattleGroundBase ccontr3 = m_Characters[0];
				CharacterControllerBattleGroundBase cBanner = m_Characters[1];
				m_Rows.Add(new List<Rect>());
				m_Rows.Add(new List<Rect>());
				m_RowBounds.Add(new Vector3(0f, 0f, 0f));
				m_RowBounds.Add(new Vector3(0f, 0f, 0f));
				m_CharactersPerRow.Add(new List<CharacterControllerBattleGroundBase>());
				m_CharactersPerRow.Add(new List<CharacterControllerBattleGroundBase>());
				m_RowBounds[0] += new Vector3(0f, ccontr3.Size.y, 0f);
				m_RowBounds[0] = new Vector3(Mathf.Max(m_RowBounds[0].x, ccontr3.Size.x), m_RowBounds[0].y, 0f);
				Rect position = new Rect(0f - ccontr3.Extents.x, 0f - (m_RowBounds[0].y - ccontr3.Extents.y + ccontr3.Center.y), ccontr3.Size.x, ccontr3.Size.y);
				m_CharactersPerRow[0].Add(ccontr3);
				m_Rows[0].Add(position);
				m_RowBounds[1] += new Vector3(0f, cBanner.Size.y, 0f);
				m_RowBounds[1] = new Vector3(Mathf.Max(m_RowBounds[1].x, cBanner.Size.x), m_RowBounds[1].y, 0f);
				Rect position8 = new Rect(0f - ccontr3.Extents.x, 0f - (m_RowBounds[1].y - cBanner.Extents.y + cBanner.Center.y), cBanner.Size.x, cBanner.Size.y);
				m_CharactersPerRow[1].Add(cBanner);
				m_Rows[1].Add(position8);
				break;
			}
			}
		}
		else if (faction == Faction.Birds && combatantCount == 3)
		{
			CharacterControllerBattleGroundBase ccontr2 = m_Characters[0];
			CharacterControllerBattleGroundBase ccontr6 = m_Characters[1];
			CharacterControllerBattleGroundBase ccontr9 = m_Characters[2];
			m_Rows.Add(new List<Rect>());
			m_Rows.Add(new List<Rect>());
			m_RowBounds.Add(new Vector3(0f, 0f, 0f));
			m_RowBounds.Add(new Vector3(0f, 0f, 0f));
			m_CharactersPerRow.Add(new List<CharacterControllerBattleGroundBase>());
			m_CharactersPerRow.Add(new List<CharacterControllerBattleGroundBase>());
			m_RowBounds[1] += new Vector3(0f, ccontr2.Size.y, 0f);
			m_RowBounds[1] = new Vector3(Mathf.Max(m_RowBounds[1].x, ccontr2.Size.x), m_RowBounds[1].y, 0f);
			Rect position0 = new Rect(0f - ccontr2.Extents.x, 0f - (m_RowBounds[1].y - ccontr2.Extents.y + ccontr2.Center.y), ccontr2.Size.x, ccontr2.Size.y);
			m_CharactersPerRow[1].Add(ccontr2);
			m_Rows[1].Add(position0);
			m_RowBounds[0] += new Vector3(0f, ccontr6.Size.y, 0f);
			m_RowBounds[0] = new Vector3(Mathf.Max(m_RowBounds[0].x, ccontr6.Size.x), m_RowBounds[0].y, 0f);
			Rect position4 = new Rect(0f - ccontr2.Extents.x, 0f - (m_RowBounds[0].y - ccontr6.Extents.y + ccontr6.Center.y), ccontr6.Size.x, ccontr6.Size.y);
			m_CharactersPerRow[0].Add(ccontr6);
			m_Rows[0].Add(position4);
			m_RowBounds[1] += new Vector3(0f, ccontr9.Size.y, 0f);
			m_RowBounds[1] = new Vector3(Mathf.Max(m_RowBounds[1].x, ccontr9.Size.x), m_RowBounds[1].y, 0f);
			Rect position7 = new Rect(0f - ccontr2.Extents.x, 0f - (m_RowBounds[1].y - ccontr9.Extents.y + ccontr9.Center.y), ccontr9.Size.x, ccontr9.Size.y);
			m_CharactersPerRow[1].Add(ccontr9);
			m_Rows[1].Add(position7);
		}
		else
		{
			CollectExtents(m_Characters, m_CharactersPerRow, m_Rows, maxDistance, row, minOffset, m_RowBounds);
		}
		float offsetPos = maxDistance / (float)combatantCount;
		if (combatantCount >= 3)
		{
			offsetPos = maxDistance / (float)(combatantCount - 1);
		}
		float startPos = maxDistance / 2f;
		bool shift = combatantCount > 3;
		float shiftValue = maxShift / 4f;
		float extentsX = 0f;
		for (int i2 = 0; i2 < m_RowBounds.Count; i2++)
		{
			extentsX += m_RowBounds[i2].x;
		}
		float rowDistance = maxShift / (float)m_RowBounds.Count;
		float rowShift = rowDistance * 0.5f * (float)(m_RowBounds.Count - 1);
		float possibleShiftX = Mathf.Max(maxShift - extentsX, 0f) / (float)m_RowBounds.Count;
		float sign = ((faction != 0) ? 1 : (-1));
		for (int iRow = 0; iRow < m_CharactersPerRow.Count; iRow++)
		{
			float ExtentDelta = Mathf.Max(maxDistance - m_RowBounds[iRow].y, 0f);
			float ExtentDeltaPerCharacter = ExtentDelta / (float)(m_CharactersPerRow[iRow].Count + 1);
			DebugLog.Log("ExtentDeltaPerCharacter " + ExtentDeltaPerCharacter);
			for (int j3 = 0; j3 < m_CharactersPerRow[iRow].Count; j3++)
			{
				CharacterControllerBattleGroundBase character2 = m_CharactersPerRow[iRow][j3];
				Rect basePos = m_Rows[iRow][j3];
				float extentForThisCharacter = ExtentDeltaPerCharacter * (float)(j3 + 1);
				if (character2.GetModel().IsBanner && m_BirdBannerPosition != null && m_PigBannerPosition != null)
				{
					character2.transform.localPosition = ((character2.GetModel().CombatantFaction != 0) ? m_PigBannerPosition.localPosition : m_BirdBannerPosition.localPosition);
					DebugLog.Log("Character: " + character2.GetModel().CombatantNameId + " yPos: " + basePos.center.y + " Row: " + row);
					character2.gameObject.SetActive(true);
					character2.SetModel(character2.GetModel(), this);
					continue;
				}
				if (character2.m_AlreadyPlaced)
				{
					Vector3 cachedLocalPos = character2.transform.localPosition;
					character2.transform.localPosition = centerTransform.localPosition + (new Vector3(sign * (float)iRow * rowDistance + basePos.center.x + (0f - sign) * rowShift + ((!character2.GetModel().IsBanner) ? UnityEngine.Random.Range((0f - possibleShiftX) / 2f, possibleShiftX / 2f) : (0f - possibleShiftX)), 0f, basePos.center.y + centerTransform.localPosition.z / 2f) - new Vector3(0f, 0f, extentForThisCharacter));
					for (int m = 0; m < iRow; m++)
					{
						if (m_CharactersPerRow[m].Count == m_CharactersPerRow[iRow].Count)
						{
							character2.transform.localPosition = new Vector3(character2.transform.localPosition.x, character2.transform.localPosition.y, character2.transform.localPosition.z - minOffset);
						}
					}
					character2.CachedUnfocusedPos = character2.transform.position - centerTransform.position;
					character2.transform.localPosition = cachedLocalPos;
					continue;
				}
				character2.transform.localPosition += new Vector3(sign * (float)iRow * rowDistance + basePos.center.x + (0f - sign) * rowShift + ((!character2.GetModel().IsBanner) ? UnityEngine.Random.Range((0f - possibleShiftX) / 2f, possibleShiftX / 2f) : (0f - possibleShiftX)), 0f, basePos.center.y + centerTransform.localPosition.z / 2f) - new Vector3(0f, 0f, extentForThisCharacter);
				for (int n = 0; n < iRow; n++)
				{
					if (m_CharactersPerRow[n].Count == m_CharactersPerRow[iRow].Count)
					{
						character2.transform.localPosition = new Vector3(character2.transform.localPosition.x, character2.transform.localPosition.y, character2.transform.localPosition.z - minOffset);
					}
				}
				DebugLog.Log("Character: " + character2.GetModel().CombatantNameId + " yPos: " + basePos.center.y + " Row: " + row);
				character2.gameObject.SetActive(true);
				character2.SetModel(character2.GetModel(), this);
			}
		}
		float repositioningDelay = 0f;
		for (int l = 0; l < factionCombatants.Count; l++)
		{
			ICombatant character = factionCombatants[l];
			Transform characterTransform = character.CombatantView.transform;
			for (int j2 = 0; j2 < factionCombatants.Count; j2++)
			{
				ICombatant comparedCharacter = factionCombatants[j2];
				if (character.CombatantView.m_AlreadyPlaced || comparedCharacter.CombatantView.m_AlreadyPlaced || character == comparedCharacter || !character.IsBanner)
				{
					continue;
				}
				Transform comparedCharacterTransform = comparedCharacter.CombatantView.transform;
				if (Mathf.Abs(comparedCharacterTransform.localPosition.z - characterTransform.localPosition.z) < 20f)
				{
					DebugLog.Log("Positioning Adjustment due to near between: " + character.CombatantNameId + " and " + comparedCharacter.CombatantNameId);
					float offset = (0f - Mathf.Sign(comparedCharacterTransform.localPosition.z - characterTransform.localPosition.z)) * 20f;
					if (offset == 0f)
					{
						offset = 20f;
					}
					DebugLog.Log("Offset: " + offset);
					character.CombatantView.transform.localPosition = new Vector3(characterTransform.localPosition.x, characterTransform.localPosition.y, characterTransform.localPosition.z + offset);
					character.CombatantView.SetCachedUnfocusedPos(character.CombatantView.transform.position);
				}
			}
			if (character.CombatantView.m_AlreadyPlaced || character.IsBanner)
			{
				repositioningDelay = ((!character.StartedHisTurn) ? Mathf.Max(character.CombatantView.PlayGoToBasePosition()) : Mathf.Max(repositioningDelay, character.CombatantView.PlayGoToFocusPosition()));
			}
			else
			{
				character.CombatantView.m_InitialStartPosOffset = new Vector3(sign * m_StartPosOffset, 0f, 0f);
				character.CombatantView.transform.localPosition += character.CombatantView.m_InitialStartPosOffset;
			}
		}
		if (repositioningDelay > 0f)
		{
			yield return new WaitForSeconds(repositioningDelay);
		}
		yield return new WaitForEndOfFrame();
		for (int j = 0; j < factionCombatants.Count; j++)
		{
			ICombatant factionCombatant = factionCombatants[j];
			factionCombatant.CombatantView.m_StartPositionY = factionCombatant.CombatantView.CachedUnfocusedPos.z;
		}
		List<ICombatant> newOrder = factionCombatants.OrderByDescending((ICombatant c) => c.CombatantView.m_StartPositionY).ToList();
		DIContainerLogic.GetBattleService().ReplaceInitiative(Model, newOrder, faction);
		PlacedCharactersAtLeastOnce = true;
	}

	private static int CollectExtents(List<CharacterControllerBattleGroundBase> m_Characters, List<List<CharacterControllerBattleGroundBase>> m_CharactersPerRow, List<List<Rect>> m_Rows, float maxDistance, int row, float minOffset, List<Vector3> m_RowBounds)
	{
		for (int i = 0; i < m_Characters.Count; i++)
		{
			CharacterControllerBattleGroundBase characterControllerBattleGroundBase = m_Characters[i];
			if (m_Rows.Count <= row)
			{
				m_Rows.Add(new List<Rect>());
				m_RowBounds.Add(Vector3.zero);
				m_CharactersPerRow.Add(new List<CharacterControllerBattleGroundBase>());
			}
			if (m_RowBounds[row].y + characterControllerBattleGroundBase.Size.y <= maxDistance)
			{
				m_RowBounds[row] += new Vector3(0f, characterControllerBattleGroundBase.Size.y, 0f);
				m_RowBounds[row] = new Vector3(Mathf.Max(m_RowBounds[row].x, characterControllerBattleGroundBase.Size.x), m_RowBounds[row].y, 0f);
				Rect item = new Rect(0f - characterControllerBattleGroundBase.Extents.x, 0f - (m_RowBounds[row].y - characterControllerBattleGroundBase.Extents.y + characterControllerBattleGroundBase.Center.y), characterControllerBattleGroundBase.Size.x, characterControllerBattleGroundBase.Size.y);
				DebugLog.Log("Character: " + characterControllerBattleGroundBase.GetModel().CombatantNameId + " " + item.ToString() + " Row: " + row);
				m_CharactersPerRow[row].Add(characterControllerBattleGroundBase);
				m_Rows[row].Add(item);
			}
			else
			{
				row++;
				i--;
			}
		}
		if (m_CharactersPerRow.Count > 2 && m_CharactersPerRow[1].Count > m_CharactersPerRow[2].Count)
		{
			List<CharacterControllerBattleGroundBase> value = m_CharactersPerRow[1];
			List<Rect> value2 = m_Rows[1];
			Vector3 value3 = m_RowBounds[1];
			m_RowBounds[1] = m_RowBounds[2];
			m_RowBounds[2] = value3;
			m_Rows[1] = m_Rows[2];
			m_Rows[2] = value2;
			m_CharactersPerRow[1] = m_CharactersPerRow[2];
			m_CharactersPerRow[2] = value;
		}
		return row;
	}

	private float NormalizeY(float p, ScaleMgr scaleMgr, float maxDistance)
	{
		return scaleMgr.m_Near_z_Border + (scaleMgr.m_Far_z_Border - scaleMgr.m_Near_z_Border) * p / maxDistance;
	}

	protected void ShiftXPos(bool left, float value, Transform trans)
	{
		if (left)
		{
			trans.Translate(0f - value, 0f, 0f);
		}
		else
		{
			trans.Translate(value, 0f, 0f);
		}
	}

	protected void ShiftYPos(bool left, float value, Transform trans)
	{
		if (left)
		{
			trans.Translate(0f, 0f - value, 0f);
		}
		else
		{
			trans.Translate(0f, value, 0f);
		}
	}

	public virtual void SpawnLootEffects(List<IInventoryItemGameData> pigDefeatedLoot, Vector3 position, Vector3 scale, bool useBonus)
	{
	}

	public virtual IEnumerator SpawnCoin(Vector3 position, Vector3 scale, float delay, bool adv)
	{
		yield break;
	}

	public virtual IEnumerator SpawnExp(Vector3 position, Vector3 scale, float delay, bool adv)
	{
		yield break;
	}

	public virtual IEnumerator SpawnBonus(Vector3 position, Vector3 scale, float delay)
	{
		yield break;
	}

	protected IEnumerator EnterCombatants()
	{
		StartCoroutine(EnterBirds());
		yield return StartCoroutine(EnterPigs(0f));
	}

	public IEnumerator EnterBirds()
	{
		float duration = 0f;
		for (int i = 0; i < m_Model.m_CombatantsByInitiative.Count; i++)
		{
			ICombatant cc = m_Model.m_CombatantsByInitiative[i];
			if (cc.CombatantFaction == Faction.Birds && !cc.Entered)
			{
				duration = ((!cc.StartedHisTurn) ? cc.CombatantView.PlayGoToBasePositionFromStartPosition() : cc.CombatantView.PlayGoToFocusPositionFromStartPosition());
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().PerCombatantEnterDelay);
				cc.Entered = true;
			}
		}
		yield return new WaitForSeconds(duration);
	}

	protected IEnumerator LeaveBirds(bool destroy = true)
	{
		float duration = 0f;
		for (int i = 0; i < m_Model.m_CombatantsByInitiative.Count; i++)
		{
			ICombatant cc = m_Model.m_CombatantsByInitiative[i];
			if (cc.CombatantFaction == Faction.Birds)
			{
				cc.Entered = false;
				duration = cc.CombatantView.PlayGoToStartPositionFromBasePosition(destroy);
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().PerCombatantEnterDelay);
			}
		}
		yield return new WaitForSeconds(duration);
	}

	public IEnumerator EnterPigsOnly(float startDelay = 0f)
	{
		yield return new WaitForSeconds(startDelay);
		for (int i = 0; i < m_Model.m_CombatantsByInitiative.Count; i++)
		{
			ICombatant cc = m_Model.m_CombatantsByInitiative[i];
			if (cc.CombatantFaction == Faction.Pigs && !(cc is BossCombatant) && !cc.Entered && !cc.CombatantView.m_AlreadyPlaced)
			{
				cc.CombatantView.PlayGoToBasePositionFromStartPosition();
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().PerCombatantEnterDelay);
				cc.Entered = true;
			}
		}
	}

	public IEnumerator EnterBoss(float startDelay = 0f)
	{
		yield return new WaitForSeconds(startDelay);
		for (int i = 0; i < m_Model.m_CombatantsByInitiative.Count; i++)
		{
			ICombatant cc = m_Model.m_CombatantsByInitiative[i];
			if (cc.CombatantFaction == Faction.Pigs && cc is BossCombatant && !cc.Entered && !cc.CombatantView.m_AlreadyPlaced)
			{
				cc.CombatantView.PlayGoToBasePositionFromStartPosition();
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().PerCombatantEnterDelay);
				cc.Entered = true;
			}
		}
	}

	public IEnumerator EnterPigs(float startDelay = 0f)
	{
		yield return new WaitForSeconds(startDelay);
		for (int i = 0; i < m_Model.m_CombatantsByInitiative.Count; i++)
		{
			ICombatant cc = m_Model.m_CombatantsByInitiative[i];
			if (cc.CombatantFaction == Faction.Pigs && !cc.Entered && !cc.CombatantView.m_AlreadyPlaced)
			{
				cc.CombatantView.PlayGoToBasePositionFromStartPosition();
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().PerCombatantEnterDelay);
				cc.Entered = true;
			}
		}
	}

	public IEnumerator SpawnPigs()
	{
		for (int i = 0; i < m_Model.m_CombatantsByInitiative.Count; i++)
		{
			ICombatant cc = m_Model.m_CombatantsByInitiative[i];
			if (cc.CombatantFaction == Faction.Pigs && !cc.CombatantView.m_AlreadyPlaced)
			{
				cc.CombatantView.transform.position = m_PigCenterPosition.position + cc.CombatantView.CachedUnfocusedPos;
				VisualEffectSetting setting = null;
				string effectName = "PigDefeated_Small";
				if (DIContainerLogic.GetVisualEffectsBalancing().TryGetVisualEffectSetting(effectName, out setting))
				{
					DebugLog.Log("Try to instantiate Pig defeated effect");
					yield return StartCoroutine(InstantiateEffects(cc, setting.VisualEffects[0], setting, new List<ICombatant> { cc }, false));
				}
			}
		}
	}

	public IEnumerator SummonCombatants(List<float> values, Faction combatantFaction, int sourceInitiative, BossAssetController bossController)
	{
		BattleParticipantTableBalancingData waveBalancing = DIContainerBalancing.Service.GetBalancingData<BattleParticipantTableBalancingData>("environmental_summon_table_" + values[3]);
		List<PigGameData> pigList = new List<PigGameData>();
		List<ICombatant> summonedList2 = new List<ICombatant>();
		foreach (BattleParticipantTableEntry entry in waveBalancing.BattleParticipants)
		{
			pigList.Add(new PigGameData(entry.NameId).SetDifficulties(m_Model.GetPlayerLevelForHotSpot(), m_Model.Balancing));
		}
		summonedList2 = DIContainerLogic.GetBattleService().GenerateSummonsWeighted(m_Model.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == combatantFaction).ToList(), waveBalancing, m_Model, (int)values[0], DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").MaxPigsInBattle);
		sourceInitiative = m_Model.CurrentCombatant.CurrentInitiative;
		foreach (ICombatant summon in summonedList2)
		{
			if (!m_Model.m_CombatantsPerFaction.ContainsKey(summon.CombatantFaction))
			{
				m_Model.m_CombatantsPerFaction.Add(summon.CombatantFaction, new List<ICombatant>());
			}
			summon.CurrentInitiative = sourceInitiative + 1;
			m_Model.m_CombatantsPerFaction[summon.CombatantFaction].Add(summon);
			summon.summoningType = SummoningType.Summoned;
		}
		if (bossController == null)
		{
			yield return StartCoroutine(PlaceCharacter(m_PigCenterPosition, Faction.Pigs));
		}
		else
		{
			bossController.PlayPassiveAnim();
			yield return new WaitForSeconds(bossController.m_PassiveAnimationSpawnDelay);
			yield return StartCoroutine(PlaceCharacterBoss(bossController.transform, bossController.m_SpawnOffset, false));
		}
		yield return StartCoroutine(EnterPigs(0f));
		foreach (PigCombatant pig in summonedList2)
		{
			if (pig.CombatantView != null && pig.PassiveSkill != null)
			{
				pig.CombatantView.StartCoroutine(pig.PassiveSkill.DoAction(m_Model, pig, pig));
				pig.IsAttacking = false;
			}
		}
		SpawnHealthBars();
	}

	public IEnumerator LeavePigs()
	{
		for (int i = 0; i < m_Model.m_CombatantsByInitiative.Count; i++)
		{
			ICombatant cc = m_Model.m_CombatantsByInitiative[i];
			if (cc.CombatantFaction == Faction.Pigs)
			{
				cc.CombatantView.PlayGoToStartPositionFromBasePosition(true);
				yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().PerCombatantEnterDelay);
			}
		}
	}

	public virtual void OnCombatantKnockedOut(ICombatant victim)
	{
	}

	public virtual IEnumerator StartCombatantTurnImmeadiatly(ICombatant combatant)
	{
		yield break;
	}

	public virtual void EnterConsumableButton()
	{
	}

	public virtual void LeaveConsumableButton()
	{
	}

	public abstract IEnumerator DoNextStep();

	public bool AnyCharacterIsActingOrInQueue()
	{
		return actingCharacters != null && actingCharacters.Count > 0;
	}

	public void InstantiateEffect(ICombatant invoker, VisualEffect effect, VisualEffectSetting setting, List<ICombatant> targets, bool isCurse)
	{
		StartCoroutine(InstantiateEffects(invoker, effect, setting, targets, isCurse));
	}

	private IEnumerator ChangeTimeScale(ICombatant invoker, VisualEffect effect, VisualEffectSetting setting)
	{
		float elapsedTime2 = 0f;
		float startValue2 = 1f;
		float endValue2 = effect.SlowMotionTimeScale;
		while (elapsedTime2 < effect.InterpolationTime)
		{
			float timePercent2 = elapsedTime2 / effect.InterpolationTime;
			float newScale2 = startValue2 + timePercent2 * (endValue2 - startValue2);
			while (DIContainerInfrastructure.GetCoreStateMgr().m_GameIsPaused)
			{
				yield return new WaitForFixedUpdate();
			}
			DIContainerInfrastructure.TimeScaleMgr.AddTimeScale("slowmotion", newScale2);
			elapsedTime2 += Time.deltaTime / Time.timeScale;
			yield return new WaitForEndOfFrame();
		}
		while (DIContainerInfrastructure.GetCoreStateMgr().m_GameIsPaused)
		{
			yield return new WaitForFixedUpdate();
		}
		Time.timeScale = effect.SlowMotionTimeScale;
		yield return new WaitForSeconds(effect.SlowMotionDuration * Time.timeScale);
		elapsedTime2 = 0f;
		startValue2 = Time.timeScale;
		endValue2 = 1f;
		while (elapsedTime2 < effect.InterpolationTime)
		{
			float timePercent = elapsedTime2 / effect.InterpolationTime;
			float newScale = startValue2 + timePercent * (endValue2 - startValue2);
			while (DIContainerInfrastructure.GetCoreStateMgr().m_GameIsPaused)
			{
				yield return new WaitForFixedUpdate();
			}
			DIContainerInfrastructure.TimeScaleMgr.AddTimeScale("slowmotion", newScale);
			elapsedTime2 += Time.deltaTime / Time.timeScale;
			yield return new WaitForEndOfFrame();
		}
		while (DIContainerInfrastructure.GetCoreStateMgr().m_GameIsPaused)
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerInfrastructure.TimeScaleMgr.RemoveTimeScale("slowmotion");
	}

	public IEnumerator InstantiateEffects(ICombatant invoker, VisualEffect effect, VisualEffectSetting setting, List<ICombatant> targets, bool isCurse)
	{
		if (effect.DelayInSeconds > 0f)
		{
			yield return new WaitForSeconds(effect.DelayInSeconds);
		}
		List<CharacterControllerBattleGroundBase> targetedCharacters = new List<CharacterControllerBattleGroundBase>();
		switch (effect.TargetCombatant)
		{
		case VisualEffectTargetCombatant.Origin:
			targetedCharacters.Add(invoker.CombatantView);
			break;
		case VisualEffectTargetCombatant.Target:
			if (targets != null)
			{
				for (int j = 0; j < targets.Count; j++)
				{
					ICombatant target = targets[j];
					targetedCharacters.Add(target.CombatantView);
				}
			}
			break;
		case VisualEffectTargetCombatant.OriginArea:
			InstantiateEffectOnArea(invoker, effect, setting, invoker.CombatantFaction);
			break;
		case VisualEffectTargetCombatant.TargetArea:
			InstantiateEffectOnArea(invoker, effect, setting, targets.FirstOrDefault().CombatantFaction);
			break;
		case VisualEffectTargetCombatant.CenterArea:
			InstantiateEffectOnArea(invoker, effect, setting, Faction.None);
			break;
		case VisualEffectTargetCombatant.GlobalCamera:
			PlayEffectOnObject(invoker, effect, setting, m_CameraAnimationRoot);
			yield break;
		case VisualEffectTargetCombatant.GlobalScreen:
			PlayEffectOnObject(invoker, effect, setting, m_ScreenFxRoot);
			yield break;
		case VisualEffectTargetCombatant.SlowMotion:
			StartCoroutine(ChangeTimeScale(invoker, effect, setting));
			yield break;
		}
		for (int i = 0; i < targetedCharacters.Count; i++)
		{
			CharacterControllerBattleGroundBase character = targetedCharacters[i];
			if (character == null)
			{
				continue;
			}
			bool debuffFound = false;
			foreach (BattleEffectGameData begd in character.GetModel().CurrrentEffects.Values)
			{
				if (setting.BalancingId == begd.m_EffectIdent)
				{
					debuffFound = true;
				}
			}
			if (!isCurse || debuffFound)
			{
				if (effect.Type == VisualEffectType.Lasting && character.CheckAndContainsVisualEffect(setting.BalancingId))
				{
					UnityEngine.Object.Destroy(t: character.LastingVisualEffects[setting.BalancingId].PlayAnimationOrAnimatorState("End"), obj: character.LastingVisualEffects[setting.BalancingId].gameObject);
					DebugLog.Log("Remove Visual Effect");
					character.LastingVisualEffects.Remove(setting.BalancingId);
				}
				InstantiateEffectOnCharacter(invoker, effect, setting, character);
			}
		}
	}

	private void PlayEffectOnObject(ICombatant invoker, VisualEffect effect, VisualEffectSetting setting, GameObject gameObject)
	{
		if (gameObject.GetComponent<Animation>() != null)
		{
			gameObject.GetComponent<Animation>().Play(effect.PrefabName);
		}
	}

	private void InstantiateEffectOnArea(ICombatant invoker, VisualEffect effect, VisualEffectSetting setting, Faction faction)
	{
		Transform transform;
		switch (faction)
		{
		case Faction.Birds:
			transform = m_BirdCenterPosition;
			break;
		case Faction.Pigs:
			transform = m_PigCenterPosition;
			break;
		default:
			transform = m_BattlegroundCenterPosition;
			break;
		}
		GameObject gameObject = DIContainerInfrastructure.GetBattleEffectAssetProvider().InstantiateObject((!effect.OnHitEffect) ? effect.PrefabName : (effect.OnHitPrefix + invoker.CombatantMainHandEquipment.BalancingData.HitEffectSuffix), transform, transform.position, Quaternion.identity);
		SetLayerRecusively(gameObject, base.gameObject.layer);
		if (faction == Faction.Pigs && !effect.DoNotMirror)
		{
			gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		else
		{
			gameObject.transform.localScale = Vector3.one;
		}
		if (invoker.CombatantView.IsAssetAtWrongSide())
		{
			gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, new Vector3(Mathf.Sign(invoker.CombatantView.transform.lossyScale.x), 1f, 1f));
		}
		gameObject.transform.localRotation = Quaternion.identity;
	}

	private void SetLayerRecusively(GameObject go, int layer)
	{
		go.layer = layer;
		foreach (Transform item in go.transform)
		{
			SetLayerRecusively(item.gameObject, layer);
		}
	}

	private void InstantiateEffectOnCharacter(ICombatant invoker, VisualEffect effect, VisualEffectSetting setting, CharacterControllerBattleGroundBase character)
	{
		Transform transform;
		switch (effect.TargetAnchor)
		{
		case VisualEffectTargetAnchor.Bottom:
			transform = character.m_AssetController.BodyRoot;
			break;
		case VisualEffectTargetAnchor.Center:
			transform = character.m_AssetController.BodyCenter;
			break;
		case VisualEffectTargetAnchor.MainHand:
			transform = character.m_AssetController.MainHandBone;
			break;
		case VisualEffectTargetAnchor.OffHand:
			transform = character.m_AssetController.OffHandBone;
			break;
		default:
			transform = character.m_AssetController.BodyRoot;
			break;
		}
		if (transform == null)
		{
			return;
		}
		GameObject gameObject;
		if (!effect.DoNotParentOnBone)
		{
			string empty = string.Empty;
			empty = ((!effect.OnHitEffect) ? effect.PrefabName : ((invoker.CombatantMainHandEquipment == null) ? effect.PrefabName : (effect.OnHitPrefix + invoker.CombatantMainHandEquipment.BalancingData.HitEffectSuffix)));
			gameObject = DIContainerInfrastructure.GetBattleEffectAssetProvider().InstantiateObject(empty, transform, transform.position, Quaternion.identity);
			if (gameObject == null)
			{
				return;
			}
			gameObject.transform.localPosition = Vector3.zero;
			if (character.GetModel().CombatantFaction == Faction.Pigs && !effect.DoNotMirror)
			{
				gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			else
			{
				gameObject.transform.localScale = Vector3.one;
			}
			if (character.IsAssetAtWrongSide())
			{
				gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, new Vector3(Mathf.Sign(character.transform.lossyScale.x), 1f, 1f));
			}
			gameObject.transform.localRotation = Quaternion.identity;
		}
		else
		{
			gameObject = DIContainerInfrastructure.GetBattleEffectAssetProvider().InstantiateObject((!effect.OnHitEffect) ? effect.PrefabName : (effect.OnHitPrefix + invoker.CombatantMainHandEquipment.BalancingData.HitEffectSuffix), transform, transform.position, m_BattleArea.rotation);
			if (gameObject == null)
			{
				return;
			}
			if (character.GetModel().CombatantFaction == Faction.Pigs)
			{
				gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			else
			{
				gameObject.transform.localScale = Vector3.one;
			}
			gameObject.transform.parent = m_BattleArea;
			if (effect.DoNotMirror)
			{
				gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, new Vector3(Mathf.Sign(gameObject.transform.lossyScale.x), 1f, 1f));
			}
			gameObject.transform.position = transform.position;
		}
		if (effect.UseTargetBoneRotation)
		{
			gameObject.transform.rotation = transform.rotation;
		}
		else
		{
			gameObject.transform.rotation = Quaternion.identity;
		}
		if (effect.UseTargetBoneScale)
		{
			gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, transform.localScale);
		}
		BattleFXController componentInChildren = gameObject.GetComponentInChildren<BattleFXController>();
		if ((bool)componentInChildren)
		{
			if (effect.HasText)
			{
				componentInChildren.SetText(DIContainerInfrastructure.GetLocaService().Tr(effect.EffectTextIdent));
			}
			componentInChildren.SetSize(character.GetModel().CharacterModel.CharacterSize, character.GetModel().CharacterModel.Scale);
			if (effect.ShowEffectInvokerIcon)
			{
				BattleEffectGameData value = null;
				if (character.GetModel().CurrrentEffects.TryGetValue(setting.BalancingId, out value))
				{
					componentInChildren.SendParameter("Target_" + value.m_Source.CombatantAssetId);
				}
			}
			if ((bool)invoker.CombatantView)
			{
				componentInChildren.SetLineRenderer(invoker.CombatantView.m_AssetController.BodyRoot, character.m_AssetController.BodyRoot);
			}
		}
		SetLayerRecusively(gameObject, base.gameObject.layer);
		if (effect.Type == VisualEffectType.Lasting)
		{
			if (!character.CheckAndContainsVisualEffect(setting.BalancingId))
			{
				gameObject.PlayAnimationOrAnimatorStateQueued(new List<string> { "Start", "Loop" }, this);
				character.LastingVisualEffects.Add(setting.BalancingId, gameObject);
			}
			else
			{
				DebugLog.Log("Lasting Effect Already Added!");
				character.LastingVisualEffects[setting.BalancingId].PlayAnimationOrAnimatorStateQueued(new List<string> { "Start", "Loop" }, this);
			}
		}
	}

	private IEnumerator PlayBossIntro(string bossName)
	{
		if (bossName == "TinkerBoss")
		{
			yield return StartCoroutine(StartTinkerBossIntro());
		}
		else if (bossName == "KrakenBoss")
		{
			yield return StartCoroutine(StartKrakenBossIntro());
		}
		yield return new WaitForEndOfFrame();
	}

	private IEnumerator StartTinkerBossIntro()
	{
		GameObject introPig = UnityEngine.Object.Instantiate(m_TinkerBossIntroPig);
		introPig.GetComponentInChildren<CharacterAssetController>().SetModel("pig_minion_stickpig", false, true, true);
		StartCoroutine(EnterBirds());
		introPig.transform.parent = m_correctBossSlot.transform.parent;
		ActionTree tree = introPig.GetComponentInChildren<ActionTree>();
		tree.Load(tree.startNode);
		List<CharacterAssetController> drones = new List<CharacterAssetController>();
		CharacterAssetController[] array = UnityEngine.Object.FindObjectsOfType<CharacterAssetController>();
		foreach (CharacterAssetController cac2 in array)
		{
			if (cac2.transform.name.Contains("PigDrone"))
			{
				drones.Add(cac2);
				cac2.gameObject.SetActive(false);
			}
		}
		StartCoroutine(EnterBoss(5.5f));
		Invoke("SurpriseBirds", 7f);
		do
		{
			yield return new WaitForEndOfFrame();
		}
		while (tree.node != null);
		foreach (CharacterAssetController cac in drones)
		{
			cac.gameObject.SetActive(true);
		}
		StartCoroutine(EnterPigsOnly(0f));
	}

	private void SurpriseBirds()
	{
		foreach (ICombatant item in m_Model.m_CombatantsPerFaction[Faction.Birds])
		{
			item.CombatantView.PlaySurprisedAnimation();
		}
	}

	private IEnumerator StartKrakenBossIntro()
	{
		GameObject introPig = UnityEngine.Object.Instantiate(m_KrakenBossIntroPig);
		introPig.GetComponentInChildren<CharacterAssetController>().SetModel("pig_minion_stickpig", false, true, true);
		StartCoroutine(EnterBirds());
		introPig.transform.parent = m_correctBossSlot.transform.parent;
		ActionTree tree = introPig.GetComponentInChildren<ActionTree>();
		tree.Load(tree.startNode);
		StartCoroutine(EnterPigs(14f));
		Invoke("SurpriseBirds", 16f);
		do
		{
			yield return new WaitForEndOfFrame();
		}
		while (tree.node != null);
	}

	public void TriggerIllusionistCopyAttack(float damageFactor, ICombatant combatant)
	{
		m_IllusionistCombatant = combatant;
		m_IllusionistDamageFactor = damageFactor;
	}

	public virtual void DestroyActionTree()
	{
	}
}
