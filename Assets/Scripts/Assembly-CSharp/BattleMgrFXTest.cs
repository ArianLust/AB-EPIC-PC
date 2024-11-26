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

public class BattleMgrFXTest : BattleMgrBase
{
	public bool m_Restart;

	public int m_CurrentSkill = 1;

	public bool m_SelfInitializing = true;

	public bool m_StepProgressing = true;

	private bool m_Aborted;

	public List<string> m_BirdNameIds = new List<string>();

	public List<string> m_PigNameIds = new List<string>();

	public List<string> m_FirstBirdEquipment = new List<string>();

	public string m_CurrentBirdSkill = string.Empty;

	public string m_CurrentPigSkill = string.Empty;

	public string m_SkillToDo = string.Empty;

	public bool m_PigTurn;

	public BattleStartGameData m_StartData;

	public SkillBattleDataBase m_CurrentSkillToDo;

	public bool m_ClearEffects;

	[method: MethodImpl(32)]
	public event Action<ICombatant> CurrentCombatantsTurnEnded;

	private void Awake()
	{
		InitBattleStartData();
	}

	public void InitBattleStartData()
	{
		BattleStartGameData battleStartGameData = new BattleStartGameData();
		List<BirdGameData> list = new List<BirdGameData>();
		foreach (string birdNameId in m_BirdNameIds)
		{
			BirdGameData birdGameData = new BirdGameData(birdNameId);
			if (birdGameData != null)
			{
				list.Add(birdGameData);
			}
		}
		battleStartGameData.m_Birds = list;
		battleStartGameData.m_BattleBalancingNameId = "battle_empty";
		List<IInventoryItemGameData> list2 = new List<IInventoryItemGameData>();
		foreach (string item in m_FirstBirdEquipment)
		{
			list2.Add(DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(list.FirstOrDefault().Level, 2, item, 1));
		}
		if (list2.Count > 0)
		{
			DIContainerLogic.InventoryService.EquipBirdWithItem(list2, InventoryItemType.MainHandEquipment, list.FirstOrDefault().InventoryGameData);
		}
		battleStartGameData.m_FactionBuffs = new Dictionary<Faction, Dictionary<string, float>>();
		battleStartGameData.m_Inventory = new InventoryGameData("player_inventory");
		battleStartGameData.m_InvokerLevel = 1;
		battleStartGameData.m_InjectableParticipantTable = new BattleParticipantTableBalancingData();
		battleStartGameData.m_InjectableParticipantTable.VictoryCondition = new VictoryCondition
		{
			Type = VictoryConditionTypes.DefeatAll,
			NameId = string.Empty,
			Value = 1f
		};
		battleStartGameData.m_InjectableParticipantTable.Type = BattleParticipantTableType.IgnoreStrength;
		battleStartGameData.m_InjectableParticipantTable.NameId = "injected";
		battleStartGameData.m_InjectableParticipantTable.BattleParticipants = new List<BattleParticipantTableEntry>();
		foreach (string pigNameId in m_PigNameIds)
		{
			battleStartGameData.m_InjectableParticipantTable.BattleParticipants.Add(new BattleParticipantTableEntry
			{
				Amount = 1f,
				LevelDifference = 0,
				NameId = pigNameId,
				Probability = 1f
			});
		}
		m_StartData = battleStartGameData;
	}

	private void LateUpdate()
	{
		if (!m_ClearEffects)
		{
			return;
		}
		foreach (ICombatant item in base.Model.m_CombatantsByInitiative)
		{
			item.CombatantView.RemoveAllAppliedEffects();
		}
		m_ClearEffects = false;
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

	public IEnumerator Start()
	{
		DebugLog.Log("Battle Injected started!");
		yield return StartCoroutine(InitializeBattle(m_StartData));
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

	private void OnGUI()
	{
		if (GUI.Button(new Rect(0f, 0f, 100f, 50f), "Speed x 1"))
		{
			DIContainerInfrastructure.TimeScaleMgr.AddTimeScale("Cheat", 1f);
		}
		if (GUI.Button(new Rect(0f, 50f, 100f, 50f), "Speed x 0.5"))
		{
			DIContainerInfrastructure.TimeScaleMgr.AddTimeScale("Cheat", 0.5f);
		}
		if (GUI.Button(new Rect(0f, 100f, 100f, 50f), "Speed x 0.25"))
		{
			DIContainerInfrastructure.TimeScaleMgr.AddTimeScale("Cheat", 0.25f);
		}
		if (GUI.Button(new Rect(0f, 150f, 100f, 50f), "Remove Effects"))
		{
			m_ClearEffects = true;
		}
	}

	public void StartBirdSkill()
	{
		m_PigTurn = false;
		StartCoroutine("DoNextStep");
	}

	public void StartPigSkill()
	{
		m_PigTurn = true;
		StartCoroutine("DoNextStep");
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
		foreach (ICombatant combatant in base.Model.m_CombatantsByInitiative)
		{
			combatant.CurrentHealth = 1000f;
		}
		if (!m_PigTurn)
		{
			base.Model.CurrentCombatant = base.Model.m_CombatantsPerFaction[Faction.Birds].FirstOrDefault();
			SkillBalancingData skbd = null;
			if (!string.IsNullOrEmpty(m_SkillToDo) && DIContainerBalancing.Service.TryGetBalancingData<SkillBalancingData>(m_SkillToDo, out skbd))
			{
				SkillGameData skill = new SkillGameData(skbd.NameId);
				m_CurrentSkillToDo = skill.GenerateSkillBattleData();
			}
			else
			{
				m_CurrentSkillToDo = base.Model.CurrentCombatant.GetSkills().FirstOrDefault();
			}
		}
		else
		{
			base.Model.CurrentCombatant = base.Model.m_CombatantsPerFaction[Faction.Pigs].FirstOrDefault();
			SkillBalancingData skbd2 = null;
			if (!string.IsNullOrEmpty(m_SkillToDo) && DIContainerBalancing.Service.TryGetBalancingData<SkillBalancingData>(m_SkillToDo, out skbd2))
			{
				SkillGameData skill2 = new SkillGameData(skbd2.NameId);
				m_CurrentSkillToDo = skill2.GenerateSkillBattleData();
			}
			else
			{
				m_CurrentSkillToDo = base.Model.CurrentCombatant.GetSkills().FirstOrDefault();
			}
		}
		if (DIContainerLogic.GetBattleService().ApplyEffectsOnTriggerType(0f, EffectTriggerType.BeforeStartOfTurn, base.Model.CurrentCombatant, null) > 0f)
		{
			yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().TimeFromAttackPosToBasePosInSec);
			yield break;
		}
		base.Model.CurrentCombatant.CombatantView.m_SkillToDo = m_CurrentSkillToDo;
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
			CharacterControllerBattleGroundSkillPreview character = item.GetComponent<CharacterControllerBattleGroundSkillPreview>();
			if (character != null)
			{
				character.StopCoroutine("DoTurn");
				character.RemoveCharacterAsset();
				UnityEngine.Object.Destroy(item.gameObject);
			}
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
