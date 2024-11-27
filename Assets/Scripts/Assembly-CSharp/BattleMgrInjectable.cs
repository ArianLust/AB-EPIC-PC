using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

public class BattleMgrInjectable : BattleMgrBase
{
	public int m_CurrentSkill = 1;

	public bool m_SelfInitializing = true;

	public bool m_StepProgressing = true;

	private bool m_Aborted;

	[method: MethodImpl(32)]
	public event Action<ICombatant> CurrentCombatantsTurnEnded;

	private void Awake()
	{
		if (m_SelfInitializing)
		{
			m_Model = DIContainerLogic.GetBattleService().GenerateBattle(ClientInfo.CurrentBattleStartGameData);
			ClientInfo.CurrentBattleGameData = m_Model;
			if (DIContainerLogic.GetBattleService().BeginBattle(ClientInfo.CurrentBattleStartGameData, base.Model) == null)
			{
				DebugLog.Error("Error on Battle start!");
				ClientInfo.CurrentBattleStartGameData = null;
			}
		}
	}

	public IEnumerator InitializeBattle(BattleStartGameData battleStartData)
	{
		m_Model = DIContainerLogic.GetBattleService().GenerateBattle(battleStartData);
		if (DIContainerLogic.GetBattleService().BeginBattle(battleStartData, base.Model) == null)
		{
			DebugLog.Error("Battle Injected failed to start!");
			yield break;
		}
		RemoveHandlers();
		RegisterHandlers();
		DebugLog.Log("Battle Injected started!");
		yield return StartCoroutine(SetupInitialPositions());
	}

	private IEnumerator Start()
	{
		if (m_SelfInitializing)
		{
			RemoveHandlers();
			RegisterHandlers();
			DebugLog.Log("Battle Injected started!");
			yield return StartCoroutine(SetupInitialPositions());
			StartCoroutine("DoNextStep");
		}
	}

	private void OnDestroy()
	{
		RemoveHandlers();
	}

	private void RegisterHandlers()
	{
	}

	public void DoTurnWithSkillRepeating(int skill)
	{
		m_CurrentSkill = skill;
	}

	private void RemoveHandlers()
	{
		if (m_Model == null)
		{
			return;
		}
		foreach (ICombatant item in base.Model.m_CombatantsByInitiative)
		{
			if (item.CombatantView != null)
			{
				item.CombatantView.DeregisterEventHandler();
			}
		}
	}

	public override void SpawnLootEffects(List<IInventoryItemGameData> pigDefeatedLoot, Vector3 position, Vector3 scale, bool useBonus)
	{
	}

	public override IEnumerator SpawnCoin(Vector3 position, Vector3 scale, float delay, bool adv)
	{
		yield break;
	}

	public override IEnumerator SpawnExp(Vector3 position, Vector3 scale, float delay, bool adv)
	{
		yield break;
	}

	public override void EnterConsumableButton()
	{
	}

	public override void LeaveConsumableButton()
	{
	}

	public override IEnumerator PlaceCharacter(Transform centerTransform, Faction faction)
	{
		ScaleMgr scaleMgr = base.transform.GetComponentInChildren<ScaleMgr>();
		int combatantCount = m_Model.m_CombatantsPerFaction[faction].Count;
		ContainerControl cc = centerTransform.GetComponent<ContainerControl>();
		Vector2 size = new Vector2(cc.m_Size.x, cc.m_Size.y);
		size /= Mathf.Sin(45f);
		float maxDistance = ((!m_HorizontalPositioning) ? size.y : size.x);
		float maxShift = ((!m_HorizontalPositioning) ? size.x : size.y);
		float offsetPos = maxDistance / (float)combatantCount;
		if (combatantCount >= 3)
		{
			offsetPos = maxDistance / (float)(combatantCount - 1);
		}
		float startPos = maxDistance / 2f;
		bool shift = combatantCount > 3;
		float shiftValue = maxShift / 4f;
		float scaleOffset = 0f;
		for (int i = 0; i < combatantCount; i++)
		{
			yield return new WaitForEndOfFrame();
			CharacterControllerBattleGroundBase ccontr = UnityEngine.Object.Instantiate(m_CharacterControllerBattlegroundPrefab, centerTransform.position, Quaternion.identity) as CharacterControllerBattleGroundBase;
			ccontr.transform.parent = m_BattleArea;
			float sizefactor = 1f;
			switch (m_Model.m_CombatantsPerFaction[faction][i].CharacterModel.CharacterSize)
			{
			case CharacterSizeType.Boss:
				sizefactor *= 2f;
				break;
			case CharacterSizeType.Large:
				sizefactor *= 1.5f;
				break;
			case CharacterSizeType.Medium:
				sizefactor *= 1.25f;
				break;
			}
			if (m_HorizontalPositioning)
			{
				float sign = ((faction != 0) ? 1 : (-1));
				shift = combatantCount > 2;
				if (combatantCount < 3)
				{
					ccontr.transform.localPosition += new Vector3(sign * ((float)i * offsetPos * sizefactor - startPos + offsetPos / 2f), 0f, 0f);
				}
				else
				{
					ccontr.transform.localPosition += new Vector3(sign * ((float)i * offsetPos * sizefactor - startPos - (float)i * 7f), 0f, 0f);
				}
				if (shift)
				{
					ShiftYPos(i % 2 == 0, shiftValue, ccontr.transform);
				}
				ccontr.SetModel(m_Model.m_CombatantsPerFaction[faction][i], this);
			}
			else
			{
				if (combatantCount < 3)
				{
					ccontr.transform.localPosition += new Vector3(0f, 0f, 0f - ((float)i * offsetPos * sizefactor - startPos + offsetPos / 2f));
				}
				else
				{
					ccontr.transform.localPosition += new Vector3(0f, 0f, 0f - ((float)i * offsetPos * sizefactor - startPos - (float)i * 7f));
				}
				if (shift)
				{
					ShiftXPos(i % 2 == 0, shiftValue, ccontr.transform);
				}
				ccontr.SetModel(m_Model.m_CombatantsPerFaction[faction][i], this);
			}
		}
	}

	public void DoNextStepStoppable()
	{
		StartCoroutine("DoNextStep");
	}

	public override IEnumerator DoNextStep()
	{
		yield return StartCoroutine(CheckForStageProgressOrBattleEnd());
		DIContainerLogic.GetBattleService().UpdateCurrentCombatant(base.Model);
		if (!base.Model.CurrentCombatant.IsAlive)
		{
			DIContainerLogic.GetBattleService().RemoveCombatantFromBattle(base.Model, base.Model.CurrentCombatant);
		}
		else if (DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.BeforeStartOfTurn, base.Model.CurrentCombatant, null) > 0f)
		{
			yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeFromAttackPosToBasePosInSec);
		}
		else
		{
			yield return base.Model.CurrentCombatant.CombatantView.StartCoroutine("DoTurn", m_CurrentSkill);
			for (int i = base.Model.m_CombatantsByInitiative.Count - 1; i >= 0; i--)
			{
				ICombatant character = base.Model.m_CombatantsByInitiative[i];
				if (character.CombatantView.m_CounterAttack && base.Model.CurrentCombatant.IsAlive && character.IsAlive)
				{
					yield return StartCoroutine(character.CombatantView.CounterAttack());
				}
			}
		}
		if (this.CurrentCombatantsTurnEnded != null)
		{
			this.CurrentCombatantsTurnEnded(base.Model.CurrentCombatant);
		}
		StartCoroutine("DoNextStep");
	}

	protected override IEnumerator SetupInitialPositions()
	{
		int pigCount = (m_Model.m_CombatantsPerFaction.ContainsKey(Faction.Pigs) ? m_Model.m_CombatantsPerFaction[Faction.Pigs].Count : 0);
		float maxDistance = 300f;
		float startPos = maxDistance / 2f;
		float offsetPos = maxDistance / (float)pigCount;
		yield return StartCoroutine(PlaceCharacter(m_BirdCenterPosition, Faction.Birds));
		yield return StartCoroutine(PlaceCharacter(m_PigCenterPosition, Faction.Pigs));
	}

	public IEnumerator ClearBattle()
	{
		StopCoroutine("DoNextStep");
		RemoveHandlers();
		m_CurrentSkill = 1;
		foreach (Transform item in m_BattleArea)
		{
			CharacterControllerBattleGroundPreview character = item.GetComponent<CharacterControllerBattleGroundPreview>();
			character.StopCoroutine("DoTurn");
			UnityEngine.Object.Destroy(item.gameObject);
		}
		yield return new WaitForEndOfFrame();
	}

	private IEnumerator CheckForStageProgressOrBattleEnd()
	{
		yield break;
	}

	private void LeaveBattleMainLoop()
	{
		RemoveHandlers();
		StopCoroutine("DoNextStep");
		foreach (ICombatant item in base.Model.m_CombatantsByInitiative)
		{
			item.CombatantView.DeregisterEventHandler();
		}
	}

	private IEnumerator HandleBirdsWon()
	{
		LeaveBattleMainLoop();
		yield break;
	}

	private IEnumerator HandlePigsWon()
	{
		LeaveBattleMainLoop();
		yield break;
	}

	private bool AllCleaningUpOfFaction(Faction faction)
	{
		return false;
	}
}
