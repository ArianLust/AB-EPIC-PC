using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class LegacyTutorialMgr : MonoBehaviour, ITutorialMgr
{
	private Dictionary<string, int> m_CachedTutorialTracks = new Dictionary<string, int>();

	private Dictionary<string, Action<string, string>> m_TutorialStepStartActions = new Dictionary<string, Action<string, string>>();

	private Dictionary<string, Action<string, string>> m_TutorialStepFinishedActions = new Dictionary<string, Action<string, string>>();

	private Dictionary<string, GuideController> m_ActiveGuides = new Dictionary<string, GuideController>();

	private GuideController CurrentDragAndDropGuide;

	public float GuideZPosition = -200f;

	[SerializeField]
	private List<TriggerNameHelperMapping> m_TriggerMapping = new List<TriggerNameHelperMapping>();

	[SerializeField]
	private GameObject m_FinishedMarker;

	private CharacterControllerBattleGroundBase m_cachedOverCharacter;

	private GlowController m_CurrentGlow;

	private CharacterControlHUD m_CurrentControlHUD;

	private Transform m_cachedOverCharacterTransform;

	public bool IsCurrentlyLocked
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	private void SetLayerRecusively(GameObject go, int layer)
	{
		go.layer = layer;
		foreach (Transform item in go.transform)
		{
			SetLayerRecusively(item.gameObject, layer);
		}
	}

	public void ShowHelp(Transform pos, string trigger, float zOffset = 0f, float yOffset = 0f)
	{
		if (!m_ActiveGuides.ContainsKey(trigger))
		{
			DebugLog.Log(trigger + " activated tutorial");
			GuideController guideController = null;
			TriggerNameHelperMapping triggerNameHelperMapping = m_TriggerMapping.FirstOrDefault((TriggerNameHelperMapping m) => m.TriggerName.ToLower() == trigger.ToLower());
			if (triggerNameHelperMapping != null && !(pos == null))
			{
				guideController = (GuideController)UnityEngine.Object.Instantiate(triggerNameHelperMapping.Guide, pos.position, Quaternion.identity);
				guideController.gameObject.transform.parent = pos;
				guideController.gameObject.transform.position = new Vector3(pos.position.x + triggerNameHelperMapping.Offsets.x, pos.position.y + triggerNameHelperMapping.Offsets.y, GuideZPosition);
				guideController.gameObject.layer = pos.gameObject.layer;
				SetLayerRecusively(guideController.gameObject, pos.gameObject.layer);
				guideController.Enter(trigger);
				m_ActiveGuides.Add(trigger, guideController);
			}
		}
	}

	public void ShowFromToHelp(BattleMgrBase battleMgr, Transform sourceRoot, Transform source, Transform target, string trigger, float zOffset = 0f)
	{
		if (m_ActiveGuides.ContainsKey(trigger))
		{
			return;
		}
		DebugLog.Log(trigger + " activated tutorial");
		GuideController guideController = null;
		TriggerNameHelperMapping triggerNameHelperMapping = m_TriggerMapping.FirstOrDefault((TriggerNameHelperMapping m) => m.TriggerName.ToLower() == trigger.ToLower());
		if (triggerNameHelperMapping != null && !(source == null))
		{
			guideController = (GuideController)UnityEngine.Object.Instantiate(triggerNameHelperMapping.Guide, source.position, Quaternion.identity);
			guideController.gameObject.transform.position = new Vector3(source.position.x + triggerNameHelperMapping.Offsets.x, source.position.y + triggerNameHelperMapping.Offsets.y, GuideZPosition);
			guideController.gameObject.layer = source.gameObject.layer;
			SetLayerRecusively(guideController.gameObject, source.gameObject.layer);
			guideController.Enter(trigger);
			m_ActiveGuides.Add(trigger, guideController);
			CurrentDragAndDropGuide = guideController;
			if ((bool)battleMgr)
			{
				battleMgr.DragVisualizationLocked -= battleMgrDragVisualizationLocked;
				battleMgr.DragVisualizationLocked += battleMgrDragVisualizationLocked;
			}
			StartCoroutine(RepeatMoveFromTo(battleMgr, sourceRoot, sourceRoot.GetComponent<CharacterControllerBattleGroundBase>(), source, target, guideController, zOffset));
		}
	}

	private void battleMgrDragVisualizationLocked(bool locked)
	{
		if ((bool)CurrentDragAndDropGuide)
		{
			CurrentDragAndDropGuide.gameObject.SetActive(!locked);
		}
	}

	private IEnumerator RepeatMoveFromTo(BattleMgrBase battleMgr, Transform sourceRoot, CharacterControllerBattleGroundBase character, Transform source, Transform target, GuideController c, float zOffset)
	{
		CHMotionTween motionTween = c.GetComponentInChildren<CHMotionTween>();
		if ((bool)motionTween)
		{
			motionTween.m_StartTransform = source;
			motionTween.m_StartOffset = new Vector3(0f, 0f, zOffset);
			motionTween.m_EndTransform = target;
			motionTween.m_EndOffset = new Vector3(0f, 0f, zOffset);
			motionTween.m_DurationInSeconds = 1.5f;
			StartCoroutine(EvaluateDragProgress(battleMgr, sourceRoot, character, source, target, motionTween, c));
		}
		yield break;
	}

	private IEnumerator EvaluateDragProgress(BattleMgrBase battleMgr, Transform sourceRoot, CharacterControllerBattleGroundBase character, Transform source, Transform target, CHMotionTween motionTween, GuideController c)
	{
		if ((bool)character && (bool)character.GetControlHUD())
		{
			character.ActivateControlHUD(true);
		}
		if (c.m_Animation.Play("Guide_Drag_Tab"))
		{
			yield return new WaitForSeconds(c.m_Animation["Guide_Drag_Tab"].length);
		}
		if (!motionTween)
		{
			yield break;
		}
		motionTween.Play();
		while ((bool)motionTween && motionTween.IsPlaying)
		{
			if ((bool)character && (bool)battleMgr && (bool)character.GetControlHUD())
			{
				ShowCharacterControlHUD(battleMgr, character, sourceRoot, battleMgr.m_SceneryCamera.WorldToScreenPoint(motionTween.transform.position));
			}
			yield return new WaitForEndOfFrame();
		}
		if (!motionTween)
		{
			yield break;
		}
		if (c.m_Animation.Play("Guide_Drag_Release"))
		{
			yield return new WaitForSeconds(c.m_Animation["Guide_Drag_Release"].length);
		}
		if (!motionTween)
		{
			yield break;
		}
		if ((bool)battleMgr && !battleMgr.LockDragVisualizationByCode)
		{
			if ((bool)m_CurrentGlow)
			{
				m_CurrentGlow.gameObject.SetActive(false);
				m_CurrentGlow = null;
			}
			if ((bool)m_CurrentControlHUD)
			{
				m_CurrentControlHUD.gameObject.SetActive(false);
				m_CurrentControlHUD = null;
			}
		}
		motionTween.Reset();
		StartCoroutine(EvaluateDragProgress(battleMgr, sourceRoot, character, source, target, motionTween, c));
	}

	public void ShowCharacterControlHUD(BattleMgrBase battleMgr, CharacterControllerBattleGroundBase character, Transform rootPosition, Vector3 currentScreenPos)
	{
		if (battleMgr.LockDragVisualizationByCode)
		{
			return;
		}
		if (m_CurrentGlow == null)
		{
			m_CurrentGlow = battleMgr.m_CurrentGlow;
			m_CurrentGlow.gameObject.SetActive(false);
		}
		if (m_CurrentControlHUD == null)
		{
			m_CurrentControlHUD = character.GetControlHUD();
		}
		Ray ray = battleMgr.m_SceneryCamera.ScreenPointToRay(currentScreenPos);
		m_CurrentControlHUD.transform.position = rootPosition.position;
		m_CurrentControlHUD.gameObject.SetActive(true);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			if (m_cachedOverCharacterTransform != hitInfo.transform)
			{
				m_cachedOverCharacterTransform = hitInfo.transform;
				CharacterControllerBattleGroundBase cachedOverCharacter = m_cachedOverCharacter;
				m_cachedOverCharacter = m_cachedOverCharacterTransform.GetComponent<CharacterControllerBattleGroundBase>();
				if (cachedOverCharacter != m_cachedOverCharacter)
				{
					if (m_cachedOverCharacter == null)
					{
						m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Hide");
						Invoke("DisableGlow", m_CurrentGlow.GetComponent<Animation>()["CharacterSelectionGlow_Hide"].length);
					}
					else
					{
						m_CurrentGlow.gameObject.SetActive(true);
						m_CurrentGlow.SetStateColor((m_cachedOverCharacter.GetModel().CombatantFaction != 0) ? GlowState.Attack : GlowState.Support);
						CancelInvoke("DisableGlow");
						m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Show");
					}
				}
			}
			if ((bool)m_cachedOverCharacter)
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
			int layer = hitInfo.transform.gameObject.layer;
			m_CurrentControlHUD.SetState(rootPosition, m_cachedOverCharacter, hitInfo.point, battleMgr);
		}
		else
		{
			m_CurrentGlow.gameObject.SetActive(false);
		}
	}

	public void HideHelp()
	{
		foreach (KeyValuePair<string, GuideController> activeGuide in m_ActiveGuides)
		{
			if (activeGuide.Value != null)
			{
				activeGuide.Value.Leave();
			}
		}
		m_ActiveGuides.Clear();
	}

	public void HideHelp(string trigger, bool finished)
	{
		GuideController value = null;
		if (m_ActiveGuides.TryGetValue(trigger, out value))
		{
			m_ActiveGuides.Remove(trigger);
			if (finished)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_FinishedMarker);
				gameObject.gameObject.transform.position = value.transform.transform.position;
				gameObject.gameObject.layer = value.gameObject.layer;
				SetLayerRecusively(gameObject.gameObject, value.gameObject.layer);
				UnityEngine.Object.Destroy(gameObject.gameObject, gameObject.GetComponent<Animation>().clip.length);
			}
			if ((bool)value)
			{
				value.Leave();
			}
		}
	}

	private void Awake()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks == null)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks = new Dictionary<string, int> { { "tutorial_attack_enemy", 0 } };
		}
		StartTutorialStep("tutorial_enter_stage1");
		StartTutorialStep("tutorial_click_wheel");
		m_TutorialStepStartActions.Add("tutorial_attack_enemy", delegate(string trigger, string param)
		{
			EvaluateAttackTargetTutorial(trigger);
		});
		m_TutorialStepFinishedActions.Add("tutorial_attack_enemy", delegate(string trigger, string param)
		{
			FinishAttackTargetTutorial(trigger);
		});
		m_TutorialStepStartActions.Add("tutorial_click_wheel", delegate(string trigger, string param)
		{
			EvaluateClickWheelTutorial(trigger);
		});
		m_TutorialStepFinishedActions.Add("tutorial_click_wheel", delegate(string trigger, string param)
		{
			FinishClickWheelTutorial(trigger);
		});
		m_TutorialStepStartActions.Add("tutorial_tooltip_character", delegate(string trigger, string param)
		{
			EvaluateTooltipCharacterTutorial(trigger);
		});
		m_TutorialStepFinishedActions.Add("tutorial_tooltip_character", delegate(string trigger, string param)
		{
			FinishTooltipCharacterTutorial(trigger);
		});
		m_TutorialStepStartActions.Add("tutorial_use_rage", delegate(string trigger, string param)
		{
			EvaluateUseRageTutorial(trigger);
		});
		m_TutorialStepFinishedActions.Add("tutorial_use_rage", delegate(string trigger, string param)
		{
			FinishUseRageTutorial(trigger);
		});
		m_TutorialStepStartActions.Add("tutorial_enter_potion_menu", delegate(string trigger, string param)
		{
			EvaluateEnterPotionsTutorial(trigger);
		});
		m_TutorialStepFinishedActions.Add("tutorial_enter_potion_menu", delegate(string trigger, string param)
		{
			FinishEnterPotionsTutorial(trigger);
		});
		m_TutorialStepStartActions.Add("tutorial_heal_with_potion", delegate(string trigger, string param)
		{
			EvaluateHealWithPotionTutorial(trigger);
		});
		m_TutorialStepFinishedActions.Add("tutorial_heal_with_potion", delegate(string trigger, string param)
		{
			FinishHealWithPotionTutorial(trigger);
		});
		m_TutorialStepStartActions.Add("tutorial_use_supportskill", delegate(string trigger, string param)
		{
			EvaluateUseSupportSkillTutorial(trigger);
		});
		m_TutorialStepFinishedActions.Add("tutorial_use_supportskill", delegate(string trigger, string param)
		{
			FinishUseSupportSkillTutorial(trigger);
		});
		m_TutorialStepStartActions.Add("tutorial_defeat_minionpig", delegate(string trigger, string param)
		{
			EvaluateDefeateMinionPigTutorial(trigger);
		});
		m_TutorialStepFinishedActions.Add("tutorial_defeat_minionpig", delegate(string trigger, string param)
		{
			FinishDefeateMinionPigTutorial(trigger);
		});
		m_TutorialStepStartActions.Add("tutorial_use_supportskill_on_ally", delegate(string trigger, string param)
		{
			EvaluateUseSupportSkillOnAllyTutorial(trigger);
		});
		m_TutorialStepFinishedActions.Add("tutorial_use_supportskill_on_ally", delegate(string trigger, string param)
		{
			FinishUseSupportSkillOnAllyTutorial(trigger);
		});
		m_TutorialStepStartActions.Add("tutorial_use_aoe", delegate(string trigger, string param)
		{
			EvaluateUseAoETutorial(trigger);
		});
		m_TutorialStepFinishedActions.Add("tutorial_use_aoe", delegate(string trigger, string param)
		{
			FinishUseAoETutorial(trigger);
		});
		m_TutorialStepStartActions.Add("tutorial_enter_camp", delegate(string trigger, string param)
		{
			EvaluateEnterCampTutorial(trigger, "tutorial_enter_camp");
		});
		m_TutorialStepFinishedActions.Add("tutorial_enter_camp", delegate(string trigger, string param)
		{
			FinishEnterCampTutorial(trigger, "tutorial_enter_camp");
		});
		m_TutorialStepStartActions.Add("tutorial_forge_01", delegate(string trigger, string param)
		{
			EvaluateEnterForgeMenuTutorial(trigger, "tutorial_forge_01");
		});
		m_TutorialStepFinishedActions.Add("tutorial_forge_01", delegate(string trigger, string param)
		{
			FinishEnterForgeMenuTutorial(trigger, "tutorial_forge_01", new List<string> { "tutorial_forge_03" });
		});
		m_TutorialStepStartActions.Add("tutorial_forge_02", delegate(string trigger, string param)
		{
			EvaluateSelectRecipeTutorial(trigger, "tutorial_forge_02", param);
		});
		m_TutorialStepFinishedActions.Add("tutorial_forge_02", delegate(string trigger, string param)
		{
			FinishSelectRecipeTutorial(trigger, "tutorial_forge_02", new List<string> { "tutorial_forge_03" });
		});
		m_TutorialStepStartActions.Add("tutorial_forge_03", delegate(string trigger, string param)
		{
			EvaluateCraftPressedTutorial(trigger, "tutorial_forge_03", "recipes_filled", "mainhandequipment");
		});
		m_TutorialStepFinishedActions.Add("tutorial_forge_03", delegate(string trigger, string param)
		{
			FinishCraftPressedTutorial(trigger, "tutorial_forge_03", new List<string> { "tutorial_forge_04" });
		});
		m_TutorialStepStartActions.Add("tutorial_forge_04", delegate(string trigger, string param)
		{
			EvaluateEquippedCraftedItemTutorial(trigger, "tutorial_forge_04");
		});
		m_TutorialStepFinishedActions.Add("tutorial_forge_04", delegate(string trigger, string param)
		{
			FinishEquippedCraftedItemTutorial(trigger, "tutorial_forge_04", new List<string> { "tutorial_forge_05" });
		});
		m_TutorialStepStartActions.Add("tutorial_forge_05", delegate(string trigger, string param)
		{
			EvaluateConfirmCraftedItemTutorial(trigger, "tutorial_forge_05");
		});
		m_TutorialStepFinishedActions.Add("tutorial_forge_05", delegate(string trigger, string param)
		{
			FinishConfirmCraftedItemTutorial(trigger, "tutorial_forge_05", new List<string> { "tutorial_forge_06" });
		});
		m_TutorialStepStartActions.Add("tutorial_forge_06", delegate(string trigger, string param)
		{
			EvaluateLeaveForgeScreen(trigger, "tutorial_forge_06");
		});
		m_TutorialStepFinishedActions.Add("tutorial_forge_06", delegate(string trigger, string param)
		{
			FinishLeaveForgeScreenTutorial(trigger, "tutorial_forge_06", new List<string> { "tutorial_forge_07" });
		});
		m_TutorialStepStartActions.Add("tutorial_forge_07", delegate(string trigger, string param)
		{
			EvaluateClickWorldMap(trigger, "tutorial_forge_07");
		});
		m_TutorialStepFinishedActions.Add("tutorial_forge_07", delegate(string trigger, string param)
		{
			FinishClickWorldMapTutorial(trigger, "tutorial_forge_07", new List<string>());
		});
		m_TutorialStepStartActions.Add("tutorial_forge_01_B", delegate(string trigger, string param)
		{
			EvaluateEnterForgeMenuTutorial(trigger, "tutorial_forge_01_B");
		});
		m_TutorialStepFinishedActions.Add("tutorial_forge_01_B", delegate(string trigger, string param)
		{
			FinishEnterForgeMenuTutorial(trigger, "tutorial_forge_01_B", new List<string> { "tutorial_forge_02_B" });
		});
		m_TutorialStepStartActions.Add("tutorial_forge_02_B", delegate(string trigger, string param)
		{
			EvaluateSelectRecipeTutorial(trigger, "tutorial_forge_02_B", "recipe_weapon_yellow_staff_01");
		});
		m_TutorialStepFinishedActions.Add("tutorial_forge_02_B", delegate(string trigger, string param)
		{
			FinishSelectRecipeTutorial(trigger, "tutorial_forge_02_B", new List<string> { "tutorial_forge_03_B" });
		});
		m_TutorialStepStartActions.Add("tutorial_forge_03_B", delegate(string trigger, string param)
		{
			EvaluateCraftPressedTutorial(trigger, "tutorial_forge_03_B", "recipe_selected", "mainhandequipment");
		});
		m_TutorialStepFinishedActions.Add("tutorial_forge_03_B", delegate(string trigger, string param)
		{
			FinishCraftPressedTutorial(trigger, "tutorial_forge_03_B", new List<string> { "tutorial_forge_04" });
		});
		m_TutorialStepStartActions.Add("tutorial_enter_magic_cauldron", delegate(string trigger, string param)
		{
			EvaluateEnterMagicCauldronTutorial(trigger, "tutorial_enter_magic_cauldron");
		});
		m_TutorialStepFinishedActions.Add("tutorial_enter_magic_cauldron", delegate(string trigger, string param)
		{
			FinishEnterMagicCauldronTutorial(trigger, "tutorial_enter_magic_cauldron", new List<string> { "tutorial_cauldron_click_craft" });
		});
		m_TutorialStepStartActions.Add("tutorial_cauldron_click_craft", delegate(string trigger, string param)
		{
			EvaluateCauldronClickCraftTutorial(trigger, "tutorial_cauldron_click_craft");
		});
		m_TutorialStepFinishedActions.Add("tutorial_cauldron_click_craft", delegate(string trigger, string param)
		{
			FinishCauldronClickCraftTutorial(trigger, "tutorial_cauldron_click_craft", new List<string> { "tutorial_cauldron_click_accept" });
		});
		m_TutorialStepStartActions.Add("tutorial_cauldron_click_accept", delegate(string trigger, string param)
		{
			EvaluateCauldronClickAcceptTutorial(trigger, "tutorial_cauldron_click_accept");
		});
		m_TutorialStepFinishedActions.Add("tutorial_cauldron_click_accept", delegate(string trigger, string param)
		{
			FinishCauldronClickAcceptTutorial(trigger, "tutorial_cauldron_click_accept", new List<string>());
		});
		m_TutorialStepStartActions.Add("tutorial_enter_trainer", delegate(string trigger, string param)
		{
			EvaluateEnterHotspotTutorial(trigger, "tutorial_enter_trainer", param, "hotspot_012_01_trainerhut");
		});
		m_TutorialStepFinishedActions.Add("tutorial_enter_trainer", delegate(string trigger, string param)
		{
			FinishEnterHotspotTutorial(trigger, "tutorial_enter_trainer", new List<string> { "tutorial_buy_first_class" }, param, "hotspot_012_01_trainerhut");
		});
		m_TutorialStepStartActions.Add("tutorial_buy_first_class", delegate(string trigger, string param)
		{
			EvaluateBuyWorkshopItemTutorial(trigger, "tutorial_buy_first_class", param, "offer_class_red_guardian_01");
		});
		m_TutorialStepFinishedActions.Add("tutorial_buy_first_class", delegate(string trigger, string param)
		{
			FinishBuyWorkshopTutorial(trigger, "tutorial_buy_first_class", new List<string> { "tutorial_press_backbutton", "tutorial_entercampbybutton", "tutorial_enter_birdmanager" }, param, "offer_class_red_guardian_01");
		});
		m_TutorialStepStartActions.Add("tutorial_enter_birdmanager", delegate(string trigger, string param)
		{
			EvaluateEnterBirdManagerTutorial(trigger, "tutorial_enter_birdmanager");
		});
		m_TutorialStepFinishedActions.Add("tutorial_enter_birdmanager", delegate(string trigger, string param)
		{
			FinishEnterBirdManagerTutorial(trigger, "tutorial_enter_birdmanager", new List<string> { "tutorial_select_class" });
		});
		m_TutorialStepStartActions.Add("tutorial_select_class", delegate(string trigger, string param)
		{
			EvaluateSelectClassTutorial(trigger, "tutorial_select_class");
		});
		m_TutorialStepFinishedActions.Add("tutorial_select_class", delegate(string trigger, string param)
		{
			FinishSelectClassTutorial(trigger, "tutorial_select_class", new List<string>());
		});
		m_TutorialStepStartActions.Add("tutorial_enter_golden_pig", delegate(string trigger, string param)
		{
			EvaluateEnterGoldenPigTutorial(trigger, "tutorial_enter_golden_pig");
		});
		m_TutorialStepFinishedActions.Add("tutorial_enter_golden_pig", delegate(string trigger, string param)
		{
			FinishEnterGoldenPigTutorial(trigger, "tutorial_enter_golden_pig", new List<string> { "tutorial_start_gacha" });
		});
		m_TutorialStepStartActions.Add("tutorial_start_gacha", delegate(string trigger, string param)
		{
			EvaluateStartGachaTutorial(trigger, "tutorial_start_gacha");
		});
		m_TutorialStepFinishedActions.Add("tutorial_start_gacha", delegate(string trigger, string param)
		{
			FinishStartGachaTutorial(trigger, "tutorial_start_gacha", new List<string>());
		});
		m_TutorialStepStartActions.Add("tutorial_enter_piglab", delegate(string trigger, string param)
		{
			EvaluateEnterHotspotTutorial(trigger, "tutorial_enter_piglab", param, "hotspot_016_02_piglab");
		});
		m_TutorialStepFinishedActions.Add("tutorial_enter_piglab", delegate(string trigger, string param)
		{
			FinishEnterHotspotTutorial(trigger, "tutorial_enter_piglab", new List<string> { "tutorial_buy_rage_potion" }, param, "hotspot_016_02_piglab");
		});
		m_TutorialStepStartActions.Add("tutorial_buy_rage_potion", delegate(string trigger, string param)
		{
			EvaluateBuyWorkshopItemTutorial(trigger, "tutorial_buy_rage_potion", param, "offer_recipe_potion_rage_01");
		});
		m_TutorialStepFinishedActions.Add("tutorial_buy_rage_potion", delegate(string trigger, string param)
		{
			FinishBuyWorkshopTutorial(trigger, "tutorial_buy_rage_potion", new List<string> { "tutorial_press_backbutton", "tutorial_entercampbybutton", "tutorial_enter_magic_cauldron_2" }, param, "offer_recipe_potion_rage_01");
		});
		m_TutorialStepStartActions.Add("tutorial_enter_magic_cauldron_2", delegate(string trigger, string param)
		{
			EvaluateEnterMagicCauldronTutorial(trigger, "tutorial_enter_magic_cauldron_2");
		});
		m_TutorialStepFinishedActions.Add("tutorial_enter_magic_cauldron_2", delegate(string trigger, string param)
		{
			FinishEnterMagicCauldronTutorial(trigger, "tutorial_enter_magic_cauldron_2", new List<string> { "tutorial_select_rage_recipe" });
		});
		m_TutorialStepStartActions.Add("tutorial_select_rage_recipe", delegate(string trigger, string param)
		{
			EvaluateSelectPotionRecipeTutorial(trigger, "tutorial_select_rage_recipe", "recipe_potion_rage_01");
		});
		m_TutorialStepFinishedActions.Add("tutorial_select_rage_recipe", delegate(string trigger, string param)
		{
			FinishSelectPotionRecipeTutorial(trigger, "tutorial_select_rage_recipe", new List<string> { "tutorial_brew_rage_potion" });
		});
		m_TutorialStepStartActions.Add("tutorial_brew_rage_potion", delegate(string trigger, string param)
		{
			EvaluatePotionCraftPressedTutorial(trigger, "tutorial_brew_rage_potion", "recipe_selected");
		});
		m_TutorialStepFinishedActions.Add("tutorial_brew_rage_potion", delegate(string trigger, string param)
		{
			FinishCraftPressedTutorial(trigger, "tutorial_brew_rage_potion", new List<string>());
		});
		m_TutorialStepStartActions.Add("enter_friendlist", delegate(string trigger, string param)
		{
			EvaluateClickFriendlist(trigger, "enter_friendlist", "enter_camp");
		});
		m_TutorialStepFinishedActions.Add("enter_friendlist", delegate(string trigger, string param)
		{
			FinishClickFriendlist(trigger, "enter_friendlist", new List<string> { "click_npc_friend" });
		});
		m_TutorialStepStartActions.Add("click_npc_friend", delegate(string trigger, string param)
		{
			EvaluateClickNPCFriend(trigger, "click_npc_friend");
		});
		m_TutorialStepFinishedActions.Add("click_npc_friend", delegate(string trigger, string param)
		{
			FinishClickNPCFriend(trigger, "click_npc_friend", new List<string> { "enter_friend_golden_pig" });
		});
		m_TutorialStepStartActions.Add("enter_friend_golden_pig", delegate(string trigger, string param)
		{
			EvaluateEnterGoldenPigTutorial(trigger, "enter_friend_golden_pig");
		});
		m_TutorialStepFinishedActions.Add("enter_friend_golden_pig", delegate(string trigger, string param)
		{
			FinishEnterGoldenPigTutorial(trigger, "enter_friend_golden_pig", new List<string> { "start_friend_gacha" });
		});
		m_TutorialStepStartActions.Add("start_friend_gacha", delegate(string trigger, string param)
		{
			EvaluateStartGachaTutorial(trigger, "start_friend_gacha");
		});
		m_TutorialStepFinishedActions.Add("start_friend_gacha", delegate(string trigger, string param)
		{
			FinishStartGachaTutorial(trigger, "start_friend_gacha", new List<string> { "back_to_camp" });
		});
		m_TutorialStepStartActions.Add("back_to_camp", delegate(string trigger, string param)
		{
			EvaluateGoBackToCampTutorial(trigger, "back_to_camp");
		});
		m_TutorialStepFinishedActions.Add("back_to_camp", delegate(string trigger, string param)
		{
			FinishGoBackToCampTutorial(trigger, "back_to_camp", new List<string> { "enter_friendlist_2" });
		});
		m_TutorialStepStartActions.Add("enter_friendlist_2", delegate(string trigger, string param)
		{
			EvaluateClickFriendlist(trigger, "enter_friendlist_2", "enter_camp");
		});
		m_TutorialStepFinishedActions.Add("enter_friendlist_2", delegate(string trigger, string param)
		{
			FinishClickFriendlist(trigger, "enter_friendlist_2", new List<string> { "click_friend_invite" });
		});
		m_TutorialStepStartActions.Add("click_friend_invite", delegate(string trigger, string param)
		{
			EvaluateClickAddFriend(trigger, "click_friend_invite");
		});
		m_TutorialStepFinishedActions.Add("click_friend_invite", delegate(string trigger, string param)
		{
			FinishClickAddFriend(trigger, "click_friend_invite", new List<string>());
		});
		m_TutorialStepStartActions.Add("tutorial_enter_stage1", delegate(string trigger, string param)
		{
			EvaluateClickHotspotTutorial(trigger, "tutorial_enter_stage1", param, "hotspot_002_battleground");
		});
		m_TutorialStepFinishedActions.Add("tutorial_enter_stage1", delegate(string trigger, string param)
		{
			FinishClickHotspotTutorial(trigger, "tutorial_enter_stage1", new List<string> { "tutorial_enter_battle" }, param, "hotspot_002_battleground");
		});
		m_TutorialStepStartActions.Add("tutorial_enter_battle", delegate(string trigger, string param)
		{
			EvaluateEnterBattleTutorial(trigger, "tutorial_enter_battle", param);
		});
		m_TutorialStepFinishedActions.Add("tutorial_enter_battle", delegate(string trigger, string param)
		{
			FinishEnterBattleTutorial(trigger, "tutorial_enter_battle", new List<string>(), param);
		});
		m_TutorialStepStartActions.Add("tutorial_enter_workshop", delegate(string trigger, string param)
		{
			EvaluateEnterHotspotTutorial(trigger, "tutorial_enter_workshop", param, "hotspot_008_workshop");
		});
		m_TutorialStepFinishedActions.Add("tutorial_enter_workshop", delegate(string trigger, string param)
		{
			FinishEnterHotspotTutorial(trigger, "tutorial_enter_workshop", new List<string> { "tutorial_buy_first_recipe" }, param, "hotspot_008_workshop");
		});
		m_TutorialStepStartActions.Add("tutorial_harvest_resources", delegate(string trigger, string param)
		{
			EvaluateTapResourceTutorial(trigger, "tutorial_harvest_resources", param, "hotspot_004_02_resourcenode");
		});
		m_TutorialStepFinishedActions.Add("tutorial_harvest_resources", delegate(string trigger, string param)
		{
			FinishTapResourceTutorial(trigger, "tutorial_harvest_resources", new List<string>(), param, "hotspot_004_02_resourcenode");
		});
		m_TutorialStepStartActions.Add("tutorial_buy_first_recipe", delegate(string trigger, string param)
		{
			EvaluateBuyWorkshopItemTutorial(trigger, "tutorial_buy_first_recipe", param, "offer_recipe_weapon_yellow_staff_01");
		});
		m_TutorialStepFinishedActions.Add("tutorial_buy_first_recipe", delegate(string trigger, string param)
		{
			FinishBuyWorkshopTutorial(trigger, "tutorial_buy_first_recipe", new List<string> { "tutorial_press_backbutton", "tutorial_forge_01_B", "tutorial_entercampbybutton" }, param, "recipe_weapon_yellow_staff_01");
		});
		m_TutorialStepStartActions.Add("tutorial_press_backbutton", delegate(string trigger, string param)
		{
			EvaluateBackButtonPressedTutorial(trigger, "tutorial_press_backbutton", param, "offer_recipe_weapon_yellow_staff_01");
		});
		m_TutorialStepFinishedActions.Add("tutorial_press_backbutton", delegate(string trigger, string param)
		{
			FinishBackButtonPressedTutorial(trigger, "tutorial_press_backbutton");
		});
		m_TutorialStepStartActions.Add("tutorial_entercampbybutton", delegate(string trigger, string param)
		{
			EvaluateEnterCampByButtonTutorial(trigger, "tutorial_entercampbybutton");
		});
		m_TutorialStepFinishedActions.Add("tutorial_entercampbybutton", delegate(string trigger, string param)
		{
			FinishEnterCampByButtonTutorial(trigger, "tutorial_entercampbybutton");
		});
		m_TutorialStepStartActions.Add("tutorial_unlock_piggate", delegate(string trigger, string param)
		{
			EvaluateOpenPigGateTutorial(trigger, "tutorial_unlock_piggate", param, "hotspot_035_piggate_yellow");
		});
		m_TutorialStepFinishedActions.Add("tutorial_unlock_piggate", delegate(string trigger, string param)
		{
			FinishClickHotspotTutorial(trigger, "tutorial_unlock_piggate", new List<string>(), param, "hotspot_035_piggate_yellow");
		});
	}

	public void FinishWholeTutorial()
	{
		foreach (TriggerNameHelperMapping item in m_TriggerMapping)
		{
			FinishTutorialStep(item.TriggerName);
		}
	}

	public void FinishTutorialStep(string ident)
	{
		DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[ident] = 1;
		int num = 0;
		foreach (int value in DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Values)
		{
			if (value == 1)
			{
				num++;
			}
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("TutorialName", ident);
		dictionary.Add("TutorialStepsCompleted", num.ToString("0"));
		dictionary.Add("PlayerLevel", DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString());
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("TutorialCompleted", dictionary);
		DebugLog.Log("Tutorial Completed: " + ident);
		DebugLog.Log("Tutorialsteps Completed: " + num);
	}

	private void FinishUseSupportSkillTutorial(string trigger)
	{
		if (trigger == "character_clicked")
		{
			HideHelp("tutorial_use_supportskill", true);
			FinishTutorialStep("tutorial_use_supportskill");
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey("tutorial_tooltip_character"))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add("tutorial_tooltip_character", 0);
			}
			BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			battleMgr.CurrentCombatantsTurnEnded -= delegate
			{
				BmgrOnCurrentCombatantsTurnEnded("tutorial_use_supportskill", battleMgr);
			};
		}
		if (trigger == "turn_ended")
		{
			HideHelp("tutorial_use_supportskill", false);
		}
		if (trigger == "enemy_clicked")
		{
			HideHelp("tutorial_use_supportskill", false);
		}
		if (trigger == "rage_used")
		{
			HideHelp("tutorial_use_supportskill", false);
		}
	}

	private void EvaluateUseSupportSkillTutorial(string trigger)
	{
		if (trigger != "bird_turn_started" || !DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_rage"))
		{
			return;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
		BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
		bool flag = false;
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			CharacterControllerBattleGround characterControllerBattleGround = (CharacterControllerBattleGround)array2[i];
			if (characterControllerBattleGround.GetModel().CombatantFaction == Faction.Pigs)
			{
				PigCombatant pigCombatant = characterControllerBattleGround.GetModel() as PigCombatant;
				foreach (BattleEffectGameData value in pigCombatant.CurrrentEffects.Values)
				{
					if (value.m_Effects.FirstOrDefault((BattleEffect e) => e.EffectType == BattleEffectType.Charge) != null && value.GetTurnsLeft() == 1)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		battleMgr.CurrentCombatantsTurnEnded -= delegate
		{
			BmgrOnCurrentCombatantsTurnEnded("tutorial_use_supportskill", battleMgr);
		};
		battleMgr.CurrentCombatantsTurnEnded += delegate
		{
			BmgrOnCurrentCombatantsTurnEnded("tutorial_use_supportskill", battleMgr);
		};
		for (int j = 0; j < array.Length; j++)
		{
			CharacterControllerBattleGround characterControllerBattleGround2 = array[j] as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround2 && battleMgr.Model.CurrentCombatant.CombatantNameId == characterControllerBattleGround2.GetModel().CombatantNameId)
			{
				characterControllerBattleGround2.Clicked -= delegate(ICombatant c)
				{
					character_Clicked("tutorial_use_supportskill", c);
				};
				characterControllerBattleGround2.Clicked += delegate(ICombatant c)
				{
					character_Clicked("tutorial_use_supportskill", c);
				};
				ShowHelp(characterControllerBattleGround2.m_AssetController.BodyCenter, "tutorial_use_supportskill", 0f, 0f);
				HideHelp("tutorial_tooltip_character", false);
				HideHelp("tutorial_defeat_minionpig", false);
			}
		}
	}

	private void EvaluateUseSupportSkillOnAllyTutorial(string trigger)
	{
		if (trigger != "bird_turn_started")
		{
			return;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
		BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
		if (battleMgr.Model.Balancing.NameId != "battle_005")
		{
			return;
		}
		battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_use_supportskill_on_ally", battleMgr);
		battleMgr.CurrentCombatantsTurnEnded += BmgrOnCurrentCombatantsTurnEnded("tutorial_use_supportskill_on_ally", battleMgr);
		CharacterControllerBattleGround characterControllerBattleGround = null;
		CharacterControllerBattleGround characterControllerBattleGround2 = null;
		for (int i = 0; i < array.Length; i++)
		{
			CharacterControllerBattleGround characterControllerBattleGround3 = array[i] as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround3 && characterControllerBattleGround3.GetModel().CombatantNameId == "bird_yellow")
			{
				characterControllerBattleGround3.Clicked -= CharacterOnClicked("tutorial_use_supportskill_on_ally", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround3.Clicked += CharacterOnClicked("tutorial_use_supportskill_on_ally", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround = characterControllerBattleGround3;
				break;
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			CharacterControllerBattleGround characterControllerBattleGround4 = array[j] as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround4 && characterControllerBattleGround4.GetModel().CombatantNameId == "bird_red")
			{
				characterControllerBattleGround4.Clicked -= CharacterOnClicked("tutorial_use_supportskill_on_ally", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround4.Clicked += CharacterOnClicked("tutorial_use_supportskill_on_ally", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround2 = characterControllerBattleGround4;
				break;
			}
		}
		if ((bool)characterControllerBattleGround2 && (bool)characterControllerBattleGround)
		{
			ShowFromToHelp(battleMgr, characterControllerBattleGround2.transform, characterControllerBattleGround2.m_AssetController.BodyCenter, characterControllerBattleGround.m_AssetController.BodyCenter, "tutorial_use_supportskill_on_ally", -10f);
		}
	}

	private void FinishUseSupportSkillOnAllyTutorial(string trigger)
	{
		DebugLog.Log("FinishSupportSkill: " + trigger);
		if (trigger == "character_clicked")
		{
			BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			if (battleMgr == null)
			{
				return;
			}
			battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_use_supportskill_on_ally", battleMgr);
			HideHelp("tutorial_use_supportskill_on_ally", true);
			FinishTutorialStep("tutorial_use_supportskill_on_ally");
			StartTutorialStep("tutorial_use_aoe");
		}
		if (trigger == "turn_ended")
		{
			HideHelp("tutorial_use_supportskill", false);
		}
		if (trigger == "enemy_clicked")
		{
			HideHelp("tutorial_use_supportskill", false);
		}
		if (trigger == "rage_used")
		{
			HideHelp("tutorial_use_supportskill", false);
		}
	}

	private void EvaluateUseAoETutorial(string trigger)
	{
		if (trigger != "bird_turn_started")
		{
			return;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
		BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
		if (battleMgr.Model.Balancing.NameId != "battle_005")
		{
			return;
		}
		battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_use_aoe", battleMgr);
		battleMgr.CurrentCombatantsTurnEnded += BmgrOnCurrentCombatantsTurnEnded("tutorial_use_aoe", battleMgr);
		CharacterControllerBattleGround characterControllerBattleGround = null;
		CharacterControllerBattleGround characterControllerBattleGround2 = null;
		for (int i = 0; i < array.Length; i++)
		{
			CharacterControllerBattleGround characterControllerBattleGround3 = array[i] as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround3 && characterControllerBattleGround3.GetModel().CombatantFaction == Faction.Pigs)
			{
				characterControllerBattleGround3.Clicked -= CharacterOnClicked("tutorial_use_aoe", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround3.Clicked += CharacterOnClicked("tutorial_use_aoe", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround = characterControllerBattleGround3;
				break;
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			CharacterControllerBattleGround characterControllerBattleGround4 = array[j] as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround4 && characterControllerBattleGround4.GetModel().CombatantFaction == Faction.Birds && characterControllerBattleGround4.GetModel().CombatantNameId == "bird_yellow")
			{
				characterControllerBattleGround4.Clicked -= CharacterOnClicked("tutorial_use_aoe", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround4.Clicked += CharacterOnClicked("tutorial_use_aoe", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround2 = characterControllerBattleGround4;
				break;
			}
		}
		if ((bool)characterControllerBattleGround2 && (bool)characterControllerBattleGround)
		{
			ShowFromToHelp(battleMgr, characterControllerBattleGround2.transform, characterControllerBattleGround2.m_AssetController.BodyCenter, characterControllerBattleGround.m_AssetController.BodyCenter, "tutorial_use_aoe", -10f);
		}
	}

	private void FinishUseAoETutorial(string trigger)
	{
		if (trigger == "character_clicked")
		{
			BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			if (battleMgr == null)
			{
				return;
			}
			HideHelp("tutorial_use_aoe", true);
			battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_use_aoe", battleMgr);
			FinishTutorialStep("tutorial_use_aoe");
		}
		if (trigger == "turn_ended")
		{
			HideHelp("tutorial_attack_enemy", false);
		}
	}

	private void FinishUseRageTutorial(string trigger)
	{
		if (trigger == "character_clicked")
		{
			HideHelp("tutorial_use_rage", false);
		}
		if (trigger == "rage_used")
		{
			HideHelp("tutorial_use_rage", true);
			FinishTutorialStep("tutorial_use_rage");
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
			BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			battleMgr.CurrentCombatantsTurnEnded -= delegate
			{
				BmgrOnCurrentCombatantsTurnEnded("tutorial_use_rage", battleMgr);
			};
		}
		if (trigger == "turn_ended")
		{
			HideHelp("tutorial_use_rage", false);
		}
	}

	private void EvaluateUseRageTutorial(string trigger)
	{
		if (trigger != "bird_turn_started")
		{
			return;
		}
		BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
		if (battleMgr.Model.m_CurrentRage < 100f || !DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_rage"))
		{
			return;
		}
		DebugLog.Log("Show RageMeter Tutorial!");
		battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_use_rage", battleMgr);
		battleMgr.CurrentCombatantsTurnEnded += BmgrOnCurrentCombatantsTurnEnded("tutorial_use_rage", battleMgr);
		battleMgr.Model.RageUsed -= delegate(float v, ICombatant c)
		{
			ragemeter_RageUsed("tutorial_use_rage", battleMgr, v, c);
		};
		battleMgr.Model.RageUsed += delegate(float v, ICombatant c)
		{
			ragemeter_RageUsed("tutorial_use_rage", battleMgr, v, c);
		};
		CharacterControllerBattleGround characterControllerBattleGround = null;
		for (int i = 0; i < array.Length; i++)
		{
			CharacterControllerBattleGround characterControllerBattleGround2 = array[i] as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround2 && characterControllerBattleGround2.GetModel().CombatantFaction == Faction.Birds && characterControllerBattleGround2.GetModel().IsRageAvailiable)
			{
				characterControllerBattleGround = characterControllerBattleGround2;
				break;
			}
		}
		if ((bool)characterControllerBattleGround)
		{
			ShowFromToHelp(battleMgr, battleMgr.m_BattleUI.m_RageMeter.m_RageMeterPosInScene.transform, battleMgr.m_BattleUI.m_RageMeter.m_RageMeterPosInScene.transform, characterControllerBattleGround.transform, "tutorial_use_rage", -200f);
		}
	}

	private void EvaluateEnterPotionsTutorial(string trigger)
	{
		if (!(trigger != "bird_turn_started"))
		{
			BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			DebugLog.Log("enterPotionsTutorial");
			if (!(battleMgr.Model.CurrentCombatant.CurrentHealth > battleMgr.Model.CurrentCombatant.ModifiedHealth * 0.5f))
			{
				battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_enter_potion_menu", battleMgr);
				battleMgr.CurrentCombatantsTurnEnded += BmgrOnCurrentCombatantsTurnEnded("tutorial_enter_potion_menu", battleMgr);
				battleMgr.m_BattleUI.ConsumableButtonClicked -= ConsumableBarButtonClicked("tutorial_enter_potion_menu", battleMgr);
				battleMgr.m_BattleUI.ConsumableButtonClicked += ConsumableBarButtonClicked("tutorial_enter_potion_menu", battleMgr);
				HideHelp();
				ShowHelp(battleMgr.m_BattleUI.m_ConsumableButtonRoot.transform, "tutorial_enter_potion_menu", 0f, 0f);
			}
		}
	}

	private Action ConsumableBarButtonClicked(string trigger, BattleMgr bmgr)
	{
		return delegate
		{
			OnConsumableBarButtonClicked(trigger, bmgr);
		};
	}

	private void OnConsumableBarButtonClicked(string trigger, BattleMgr bmgr)
	{
		bmgr.m_BattleUI.ConsumableButtonClicked -= ConsumableBarButtonClicked(trigger, bmgr);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("consumable_menu_entered", string.Empty);
		}
	}

	private void FinishEnterPotionsTutorial(string trigger)
	{
		if (trigger == "character_clicked")
		{
			HideHelp("tutorial_enter_potion_menu", false);
		}
		if (trigger == "consumable_menu_entered")
		{
			HideHelp("tutorial_enter_potion_menu", true);
			FinishTutorialStep("tutorial_enter_potion_menu");
			StartTutorialStep("tutorial_heal_with_potion");
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
			BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			battleMgr.CurrentCombatantsTurnEnded -= delegate
			{
				BmgrOnCurrentCombatantsTurnEnded("tutorial_enter_potion_menu", battleMgr);
			};
		}
		if (trigger == "turn_ended")
		{
			HideHelp("tutorial_enter_potion_menu", false);
		}
	}

	private void EvaluateHealWithPotionTutorial(string trigger)
	{
		if (trigger != "consumable_bar_entered")
		{
			return;
		}
		BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
		ConsumableBattleButtonController[] componentsInChildren = battleMgr.m_BattleUI.m_ConsumableBar.m_Grid.gameObject.GetComponentsInChildren<ConsumableBattleButtonController>(true);
		DebugLog.Log("ListLength: " + componentsInChildren.Length);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			DebugLog.Log(componentsInChildren[i].getConsumableName());
			if (componentsInChildren[i].getConsumableName() == "potion_healing_01" || componentsInChildren[i].getConsumableName() == "offer_potion_healing_01_01")
			{
				componentsInChildren[i].ButtonClicked -= ConsumableButtonClicked("tutorial_heal_with_potion", battleMgr);
				componentsInChildren[i].ButtonClicked += ConsumableButtonClicked("tutorial_heal_with_potion", battleMgr);
				HideHelp();
				ShowFromToHelp(battleMgr, componentsInChildren[i].transform, componentsInChildren[i].transform, battleMgr.Model.CurrentCombatant.CombatantView.m_AssetController.BodyRoot, "tutorial_heal_with_potion", 0f);
			}
		}
	}

	private Action ConsumableButtonClicked(string trigger, BattleMgr bmgr)
	{
		return delegate
		{
			OnConsumableButtonClicked(trigger, bmgr);
		};
	}

	private void OnConsumableButtonClicked(string trigger, BattleMgr bmgr)
	{
		ConsumableBattleButtonController[] componentsInChildren = bmgr.m_BattleUI.m_ConsumableBar.m_Grid.GetComponentsInChildren<ConsumableBattleButtonController>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].getConsumableName() == "potion_healing_01")
			{
				componentsInChildren[i].ButtonClicked -= ConsumableButtonClicked("tutorial_heal_with_potion", bmgr);
			}
		}
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("consumable_used", string.Empty);
		}
	}

	private void FinishHealWithPotionTutorial(string trigger)
	{
		if (trigger == "consumable_used")
		{
			HideHelp("consumable_button", true);
			FinishTutorialStep("tutorial_heal_with_potion");
		}
	}

	private void FinishTooltipCharacterTutorial(string trigger)
	{
		if (trigger == "character_clicked")
		{
			HideHelp("tutorial_tooltip_character", false);
		}
		if (trigger == "rage_used")
		{
			HideHelp("tutorial_tooltip_character", false);
		}
		if (trigger == "character_showtooltip")
		{
			HideHelp("tutorial_tooltip_character", true);
			FinishTutorialStep("tutorial_tooltip_character");
			BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_tooltip_character", battleMgr);
		}
		if (trigger == "turn_ended")
		{
			HideHelp("tutorial_tooltip_character", false);
		}
	}

	private void EvaluateTooltipCharacterTutorial(string trigger)
	{
		if (trigger != "bird_turn_started" || !DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_rage"))
		{
			return;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
		BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
		battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_tooltip_character", battleMgr);
		battleMgr.CurrentCombatantsTurnEnded += BmgrOnCurrentCombatantsTurnEnded("tutorial_tooltip_character", battleMgr);
		for (int i = 0; i < array.Length; i++)
		{
			CharacterControllerBattleGround characterControllerBattleGround = array[i] as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround && battleMgr.Model.CurrentCombatant.CombatantNameId == characterControllerBattleGround.GetModel().CombatantNameId)
			{
				characterControllerBattleGround.ShowTooltip -= CharacterOnShowTooltip("tutorial_tooltip_character");
				characterControllerBattleGround.ShowTooltip += CharacterOnShowTooltip("tutorial_tooltip_character");
				ShowHelp(characterControllerBattleGround.m_AssetController.BodyCenter, "tutorial_tooltip_character", 0f, 0f);
			}
		}
	}

	private Action<ICombatant> CharacterOnShowTooltip(string trigger)
	{
		return delegate(ICombatant c)
		{
			character_ShowTooltip(trigger, c);
		};
	}

	private void turn_Ended(string trigger, BattleMgr bmgr, ICombatant combatant)
	{
		bmgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded(trigger, bmgr);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("turn_ended", combatant.CombatantNameId);
		}
	}

	private Action<ICombatant> BmgrOnCurrentCombatantsTurnEnded(string trigger, BattleMgr bmgr)
	{
		return delegate(ICombatant c)
		{
			turn_Ended(trigger, bmgr, c);
		};
	}

	private Action<float, ICombatant> ModelOnRageUsed(string trigger, BattleMgr bmgr)
	{
		return delegate(float v, ICombatant c)
		{
			ragemeter_RageUsed(trigger, bmgr, v, c);
		};
	}

	private void FinishDefeateMinionPigTutorial(string trigger)
	{
		if (trigger == "character_clicked")
		{
			HideHelp("tutorial_defeat_minionpig", true);
		}
		if (trigger == "enemy_defeated")
		{
			HideHelp("tutorial_defeat_minionpig", true);
			FinishTutorialStep("tutorial_defeat_minionpig");
			BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_defeat_minionpig", battleMgr);
		}
		if (trigger == "turn_ended")
		{
			HideHelp("tutorial_defeat_minionpig", false);
		}
	}

	private void EvaluateDefeateMinionPigTutorial(string trigger)
	{
		if (trigger != "bird_turn_started")
		{
			return;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
		BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
		if (battleMgr.Model.Balancing.NameId != "battle_003")
		{
			return;
		}
		battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_defeat_minionpig", battleMgr);
		battleMgr.CurrentCombatantsTurnEnded += BmgrOnCurrentCombatantsTurnEnded("tutorial_defeat_minionpig", battleMgr);
		CharacterControllerBattleGround characterControllerBattleGround = null;
		CharacterControllerBattleGround characterControllerBattleGround2 = null;
		for (int i = 0; i < array.Length; i++)
		{
			CharacterControllerBattleGround characterControllerBattleGround3 = array[i] as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround3 && characterControllerBattleGround3.GetModel().CombatantFaction == Faction.Pigs && characterControllerBattleGround3.GetModel().CombatantNameId.Contains("pig_minion_stickpig"))
			{
				characterControllerBattleGround3.GetModel().Defeated -= OnDefeated("tutorial_defeat_minionpig", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround3.GetModel().Defeated += OnDefeated("tutorial_defeat_minionpig", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround3.Clicked -= CharacterOnClicked("tutorial_defeat_minionpig", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround3.Clicked += CharacterOnClicked("tutorial_defeat_minionpig", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround = characterControllerBattleGround3;
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			CharacterControllerBattleGround characterControllerBattleGround4 = array[j] as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround4 && characterControllerBattleGround4.GetModel().CombatantFaction == Faction.Birds && characterControllerBattleGround4.GetModel().CombatantNameId.Contains("bird_red"))
			{
				characterControllerBattleGround4.GetModel().Defeated -= OnDefeated("tutorial_defeat_minionpig", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround4.GetModel().Defeated += OnDefeated("tutorial_defeat_minionpig", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround4.Clicked -= CharacterOnClicked("tutorial_defeat_minionpig", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround4.Clicked += CharacterOnClicked("tutorial_defeat_minionpig", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround2 = characterControllerBattleGround4;
			}
		}
		if ((bool)characterControllerBattleGround2 && (bool)characterControllerBattleGround)
		{
			ShowFromToHelp(battleMgr, characterControllerBattleGround2.transform, characterControllerBattleGround2.m_AssetController.BodyCenter, characterControllerBattleGround.m_AssetController.BodyCenter, "tutorial_defeat_minionpig", -10f);
		}
	}

	private Action<ICombatant> CharacterOnClicked(string trigger, ICombatant character)
	{
		return delegate
		{
			character_Clicked(trigger, character);
		};
	}

	private Action OnDefeated(string trigger, ICombatant character)
	{
		return delegate
		{
			enemy_Defeated(trigger, character);
		};
	}

	private void FinishClickWheelTutorial(string trigger)
	{
		if (trigger == "wheel_clicked")
		{
			HideHelp("tutorial_click_wheel", true);
			FinishTutorialStep("tutorial_click_wheel");
		}
	}

	private void EvaluateClickWheelTutorial(string trigger)
	{
		if (!(trigger != "battle_won_wheel_started"))
		{
			DebugLog.Log("click_wheel_tutorial");
			BattleResultWon battleResultWon = UnityEngine.Object.FindObjectOfType(typeof(BattleResultWon)) as BattleResultWon;
			battleResultWon.m_WheelButton.Clicked -= OnWheelClicked("wheel_clicked");
			battleResultWon.m_WheelButton.Clicked += OnWheelClicked("wheel_clicked");
			ShowHelp(battleResultWon.m_WheelButton.gameObject.transform, "tutorial_click_wheel", 0f, 0f);
		}
	}

	private Action OnWheelClicked(string tutIdent)
	{
		return delegate
		{
			FinishClickWheelTutorial(tutIdent);
		};
	}

	private void FinishAttackTargetTutorial(string trigger)
	{
		if (trigger == "character_clicked")
		{
			HideHelp("tutorial_attack_enemy", true);
		}
		if (trigger == "enemy_defeated")
		{
			HideHelp("tutorial_attack_enemy", true);
			FinishTutorialStep("tutorial_attack_enemy");
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey("tutorial_defeat_minionpig"))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add("tutorial_defeat_minionpig", 0);
			}
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey("tutorial_use_supportskill"))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add("tutorial_use_supportskill", 0);
			}
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey("tutorial_use_rage"))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add("tutorial_use_rage", 0);
			}
			BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
			if (battleMgr != null)
			{
				battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_attack_enemy", battleMgr);
			}
		}
		if (trigger == "turn_ended")
		{
			HideHelp("tutorial_attack_enemy", false);
		}
	}

	private void EvaluateAttackTargetTutorial(string trigger)
	{
		if (trigger != "bird_turn_started")
		{
			return;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterControllerBattleGround));
		BattleMgr battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
		battleMgr.CurrentCombatantsTurnEnded -= BmgrOnCurrentCombatantsTurnEnded("tutorial_attack_enemy", battleMgr);
		battleMgr.CurrentCombatantsTurnEnded += BmgrOnCurrentCombatantsTurnEnded("tutorial_attack_enemy", battleMgr);
		CharacterControllerBattleGround characterControllerBattleGround = null;
		CharacterControllerBattleGround characterControllerBattleGround2 = null;
		for (int i = 0; i < array.Length; i++)
		{
			CharacterControllerBattleGround characterControllerBattleGround3 = array[i] as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround3 && characterControllerBattleGround3.GetModel().CombatantFaction == Faction.Pigs)
			{
				characterControllerBattleGround3.GetModel().Defeated -= OnDefeated("tutorial_attack_enemy", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround3.GetModel().Defeated += OnDefeated("tutorial_attack_enemy", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround3.Clicked -= CharacterOnClicked("tutorial_attack_enemy", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround3.Clicked += CharacterOnClicked("tutorial_attack_enemy", characterControllerBattleGround3.GetModel());
				characterControllerBattleGround = characterControllerBattleGround3;
				break;
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			CharacterControllerBattleGround characterControllerBattleGround4 = array[j] as CharacterControllerBattleGround;
			if ((bool)characterControllerBattleGround4 && characterControllerBattleGround4.GetModel().CombatantFaction == Faction.Birds)
			{
				characterControllerBattleGround4.GetModel().Defeated -= OnDefeated("tutorial_attack_enemy", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround4.GetModel().Defeated += OnDefeated("tutorial_attack_enemy", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround4.Clicked -= CharacterOnClicked("tutorial_attack_enemy", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround4.Clicked += CharacterOnClicked("tutorial_attack_enemy", characterControllerBattleGround4.GetModel());
				characterControllerBattleGround2 = characterControllerBattleGround4;
				break;
			}
		}
		if ((bool)characterControllerBattleGround2 && (bool)characterControllerBattleGround)
		{
			ShowFromToHelp(battleMgr, characterControllerBattleGround2.transform, characterControllerBattleGround2.m_AssetController.BodyCenter, characterControllerBattleGround.m_AssetController.BodyCenter, "tutorial_attack_enemy", -10f);
		}
	}

	private void ragemeter_RageUsed(string trigger, BattleMgr bmgr, float value, ICombatant combatant)
	{
		bmgr.Model.RageUsed -= ModelOnRageUsed("rage_used", bmgr);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("rage_used", combatant.CombatantNameId);
		}
	}

	private void enemy_Defeated(string trigger, ICombatant combatant)
	{
		combatant.Defeated -= OnDefeated(trigger, combatant);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("enemy_defeated", combatant.CombatantNameId);
		}
	}

	private void enemy_Clicked(string trigger, ICombatant combatant)
	{
		combatant.CombatantView.Clicked -= CharacterOnClicked(trigger, combatant);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("enemy_clicked", combatant.CombatantNameId);
		}
	}

	private void character_Clicked(string trigger, ICombatant combatant)
	{
		combatant.CombatantView.Clicked -= CharacterOnClicked(trigger, combatant);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("character_clicked", combatant.CombatantNameId);
		}
	}

	private void character_ShowTooltip(string trigger, ICombatant combatant)
	{
		combatant.CombatantView.ShowTooltip -= CharacterOnShowTooltip(trigger);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("character_showtooltip", combatant.CombatantNameId);
		}
	}

	public void ShowTutorialGuideIfNecessary(string trigger, string additionalParam = "")
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks == null)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks = new Dictionary<string, int>();
		}
		List<KeyValuePair<string, int>> list = DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Where((KeyValuePair<string, int> t) => t.Value <= 0).ToList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			string key = list[num].Key;
			if (m_TutorialStepStartActions.ContainsKey(key))
			{
				m_TutorialStepStartActions[key](trigger, additionalParam);
			}
			if (m_TutorialStepFinishedActions.ContainsKey(key))
			{
				m_TutorialStepFinishedActions[key](trigger, additionalParam);
			}
		}
	}

	public void StartTutorialStep(string ident)
	{
		if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(ident))
		{
			DebugLog.Log("StartTutorialStep: " + ident);
			DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(ident, 0);
		}
	}

	public void SetTutorialCameras(bool activate)
	{
	}

	public void SkipToTutorialStep(string ident, int step, bool forceTrigger)
	{
	}

	public void SkipToTutorialStep(string ident, int step)
	{
	}

	public void StepBackOneTutorialStep(string ident)
	{
		throw new NotImplementedException();
	}

	public void ResetTutorial(string ident)
	{
		throw new NotImplementedException();
	}

	public void StartTutorial(string ident, int startStep = 0)
	{
		StartTutorialStep(ident);
	}

	public void Remove()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void FinishTutorial(string ident)
	{
	}

	public void FinishActiveTutorials(string ident)
	{
	}

	private void EvaluateEnterForgeMenuTutorial(string trigger, string tutIdent)
	{
		if (!(trigger != "enter_camp"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_ForgeProp.OnPropClicked -= ForgeCampOnOnPropClicked(tutIdent, campStateMgr);
			campStateMgr.m_ForgeProp.OnPropClicked += ForgeCampOnOnPropClicked(tutIdent, campStateMgr);
			HideHelp();
			ShowHelp(campStateMgr.m_ForgeProp.transform, tutIdent, 0f, 0f);
		}
	}

	private Action<BasicItemGameData> ForgeCampOnOnPropClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate(BasicItemGameData c)
		{
			OnForgeClicked(tutIdent, c, cstate);
		};
	}

	private void FinishEnterForgeMenuTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "forge_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
		}
	}

	private void OnForgeClicked(string trigger, BasicItemGameData forgeUnlockItem, CampStateMgr cstate)
	{
		cstate.m_ForgeProp.OnPropClicked -= ForgeCampOnOnPropClicked(trigger, cstate);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("forge_clicked", forgeUnlockItem.ItemBalancing.NameId);
		}
	}

	private void EvaluateSelectPotionRecipeTutorial(string trigger, string tutIdent, string itemName)
	{
		if (trigger != "recipes_filled")
		{
			return;
		}
		DebugLog.Log("Start Tutorial: " + tutIdent);
		CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
		HideHelp();
		HideHelp(tutIdent, false);
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(InventoryItemSlot));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			InventoryItemSlot inventoryItemSlot = (InventoryItemSlot)array2[i];
			if (inventoryItemSlot.GetModel().ItemBalancing.NameId == itemName)
			{
				inventoryItemSlot.m_InputTrigger.Clicked -= PotionSlotTriggerOnClicked(tutIdent, inventoryItemSlot, campStateMgr);
				inventoryItemSlot.m_InputTrigger.Clicked += PotionSlotTriggerOnClicked(tutIdent, inventoryItemSlot, campStateMgr);
				ShowHelp(inventoryItemSlot.transform, tutIdent, 0f, 0f);
			}
		}
	}

	private Action PotionSlotTriggerOnClicked(string tutIdent, InventoryItemSlot slot, CampStateMgr cstate)
	{
		return delegate
		{
			OnSlotClicked(tutIdent, slot, cstate);
		};
	}

	private void FinishSelectPotionRecipeTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "slot_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
			}
		}
		ShowTutorialGuideIfNecessary("recipe_selected", string.Empty);
	}

	private void EvaluateSelectRecipeTutorial(string trigger, string tutIdent, string itemName)
	{
		if (trigger != "recipes_filled")
		{
			return;
		}
		DebugLog.Log("Start Tutorial: " + tutIdent);
		CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
		HideHelp();
		HideHelp(tutIdent, false);
		if (campStateMgr.m_ForgeWindow.m_selectedCategory != InventoryItemType.MainHandEquipment)
		{
			return;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(InventoryItemSlot));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			InventoryItemSlot inventoryItemSlot = (InventoryItemSlot)array2[i];
			if (inventoryItemSlot.GetModel().ItemBalancing.NameId == itemName)
			{
				inventoryItemSlot.m_InputTrigger.Clicked -= RecipeSlotTriggerOnClicked(tutIdent, inventoryItemSlot, campStateMgr);
				inventoryItemSlot.m_InputTrigger.Clicked += RecipeSlotTriggerOnClicked(tutIdent, inventoryItemSlot, campStateMgr);
				ShowHelp(inventoryItemSlot.transform, tutIdent, 0f, 0f);
			}
		}
	}

	private Action RecipeSlotTriggerOnClicked(string tutIdent, InventoryItemSlot slot, CampStateMgr cstate)
	{
		return delegate
		{
			OnSlotClicked(tutIdent, slot, cstate);
		};
	}

	private void FinishSelectRecipeTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "slot_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
			}
		}
		ShowTutorialGuideIfNecessary("recipe_selected", string.Empty);
	}

	private void OnSlotClicked(string trigger, InventoryItemSlot slot, CampStateMgr cstate)
	{
		slot.m_InputTrigger.Clicked -= RecipeSlotTriggerOnClicked(trigger, slot, cstate);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("slot_clicked", slot.GetModel().ItemBalancing.NameId);
		}
	}

	private void EvaluateConfirmCraftedItemTutorial(string trigger, string tutIdent)
	{
		if (!(trigger != "crafted_equipped"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			HideHelp();
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_ForgeResultWindow.ConfirmedCraftingClicked -= ForgeResultWindowOnConfirmedCraftingClicked(tutIdent, campStateMgr);
			campStateMgr.m_ForgeResultWindow.ConfirmedCraftingClicked += ForgeResultWindowOnConfirmedCraftingClicked(tutIdent, campStateMgr);
			ShowHelp(campStateMgr.m_ForgeResultWindow.m_AcceptButton.transform, tutIdent, 0f, 0f);
		}
	}

	private Action ForgeResultWindowOnConfirmedCraftingClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate
		{
			OnConfirmCraftingClicked(tutIdent, cstate);
		};
	}

	private void FinishConfirmCraftedItemTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "craft_confirmed")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		DebugLog.Log("Done Tutorial: " + tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
			}
		}
		ShowTutorialGuideIfNecessary("craft_finished", string.Empty);
	}

	private void OnConfirmCraftingClicked(string trigger, CampStateMgr cstate)
	{
		cstate.m_ForgeResultWindow.ConfirmedCraftingClicked -= ForgeResultWindowOnConfirmedCraftingClicked(trigger, cstate);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("craft_confirmed", string.Empty);
		}
	}

	private void EvaluateLeaveForgeScreen(string trigger, string tutIdent)
	{
		if (!(trigger != "craft_finished"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			HideHelp();
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_ForgeWindow.ExitButtonClicked -= ForgeScreenLeaveButtonClicked(tutIdent, campStateMgr);
			campStateMgr.m_ForgeWindow.ExitButtonClicked += ForgeScreenLeaveButtonClicked(tutIdent, campStateMgr);
			ShowHelp(campStateMgr.m_ForgeWindow.m_ExitButton.transform, tutIdent, 0f, 0f);
		}
	}

	private Action ForgeScreenLeaveButtonClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate
		{
			OnLeaveButtonClicked(tutIdent, cstate);
		};
	}

	private void OnLeaveButtonClicked(string trigger, CampStateMgr cstate)
	{
		cstate.m_ForgeWindow.ExitButtonClicked -= ForgeScreenLeaveButtonClicked(trigger, cstate);
		DebugLog.Log(trigger);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("screen_left", string.Empty);
		}
	}

	private void FinishLeaveForgeScreenTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "screen_left")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		DebugLog.Log("Done Tutorial: " + tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
			}
		}
		ShowTutorialGuideIfNecessary("forge_left", string.Empty);
	}

	private void EvaluateEnterMagicCauldronTutorial(string trigger, string tutIdent)
	{
		if (!(trigger != "enter_camp"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_CauldronProp.OnPropClicked -= CauldronOnPropClicked(tutIdent, campStateMgr);
			campStateMgr.m_CauldronProp.OnPropClicked += CauldronOnPropClicked(tutIdent, campStateMgr);
			HideHelp();
			ShowHelp(campStateMgr.m_CauldronProp.transform, tutIdent, 0f, 0f);
		}
	}

	private Action<BasicItemGameData> CauldronOnPropClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate(BasicItemGameData c)
		{
			OnCauldronClicked(tutIdent, c, cstate);
		};
	}

	private void OnCauldronClicked(string trigger, BasicItemGameData forgeUnlockItem, CampStateMgr cstate)
	{
		cstate.m_CauldronProp.OnPropClicked -= CauldronOnPropClicked(trigger, cstate);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("cauldron_clicked", forgeUnlockItem.ItemBalancing.NameId);
		}
	}

	private void FinishEnterMagicCauldronTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "cauldron_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
		}
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enter_cauldron", string.Empty);
	}

	private void EvaluateCauldronClickCraftTutorial(string trigger, string tutIdent)
	{
		if (!(trigger != "enter_cauldron"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_ForgeWindow.m_CraftButton.Clicked -= CraftButtonClicked(tutIdent, campStateMgr);
			campStateMgr.m_ForgeWindow.m_CraftButton.Clicked += CraftButtonClicked(tutIdent, campStateMgr);
			HideHelp();
			ShowHelp(campStateMgr.m_ForgeWindow.m_CraftButton.transform, tutIdent, 0f, 0f);
		}
	}

	private Action CraftButtonClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate
		{
			OnCraftButtonClicked(tutIdent, cstate);
		};
	}

	private void OnCraftButtonClicked(string trigger, CampStateMgr cstate)
	{
		cstate.m_ForgeWindow.m_CraftButton.Clicked -= CraftButtonClicked(trigger, cstate);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("craft_button_clicked", string.Empty);
		}
	}

	private void FinishCauldronClickCraftTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "craft_button_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
		}
	}

	private void EvaluateCauldronClickAcceptTutorial(string trigger, string tutIdent)
	{
		if (!(trigger != "crafting_finished"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_ForgeResultWindow.m_AcceptButton.Clicked -= AcceptButtonClicked(tutIdent, campStateMgr);
			campStateMgr.m_ForgeResultWindow.m_AcceptButton.Clicked += AcceptButtonClicked(tutIdent, campStateMgr);
			HideHelp();
			ShowHelp(campStateMgr.m_ForgeResultWindow.m_AcceptButton.transform, tutIdent, 0f, 0f);
		}
	}

	private Action AcceptButtonClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate
		{
			OnAcceptButtonClicked(tutIdent, cstate);
		};
	}

	private void OnAcceptButtonClicked(string trigger, CampStateMgr cstate)
	{
		cstate.m_ForgeResultWindow.m_AcceptButton.Clicked -= AcceptButtonClicked(trigger, cstate);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("accept_button_clicked", string.Empty);
		}
	}

	private void FinishCauldronClickAcceptTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "accept_button_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
		}
	}

	private void EvaluateStartGachaTutorial(string trigger, string tutIdent)
	{
		if (!(trigger != "enter_gacha"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_GachaPopup.m_PigMachineButton.Clicked -= PigMachineClicked(tutIdent, campStateMgr);
			campStateMgr.m_GachaPopup.m_PigMachineButton.Clicked += PigMachineClicked(tutIdent, campStateMgr);
			HideHelp();
			ShowHelp(campStateMgr.m_GachaPopup.m_PigMachineButton.transform, tutIdent, 0f, 0f);
		}
	}

	private Action PigMachineClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate
		{
			OnPigMachineClicked(tutIdent, cstate);
		};
	}

	private void OnPigMachineClicked(string tutIdent, CampStateMgr cstate)
	{
		cstate.m_GachaPopup.m_PigMachineButton.Clicked -= PigMachineClicked(tutIdent, cstate);
		if (m_TutorialStepFinishedActions.ContainsKey(tutIdent))
		{
			m_TutorialStepFinishedActions[tutIdent]("pigMachine_clicked", string.Empty);
		}
	}

	private void FinishStartGachaTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "pigMachine_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
		}
		ShowTutorialGuideIfNecessary("gacha_finished", string.Empty);
		ShowTutorialGuideIfNecessary("friend_gacha_finished", string.Empty);
	}

	private void EvaluateEnterGoldenPigTutorial(string trigger, string tutIdent)
	{
		if (!(trigger != "enter_camp"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_GoldenPigCamp.OnPropClicked -= GoldenPigOnPropClicked(tutIdent, campStateMgr);
			campStateMgr.m_GoldenPigCamp.OnPropClicked += GoldenPigOnPropClicked(tutIdent, campStateMgr);
			HideHelp();
			ShowHelp(campStateMgr.m_GoldenPigCamp.transform, tutIdent, 0f, 0f);
		}
	}

	private Action<BasicItemGameData> GoldenPigOnPropClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate(BasicItemGameData c)
		{
			OnGoldenPigClicked(tutIdent, c, cstate);
		};
	}

	private void OnGoldenPigClicked(string trigger, BasicItemGameData forgeUnlockItem, CampStateMgr cstate)
	{
		cstate.m_GoldenPigCamp.OnPropClicked -= GoldenPigOnPropClicked(trigger, cstate);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("pig_clicked", forgeUnlockItem.ItemBalancing.NameId);
		}
	}

	private void FinishEnterGoldenPigTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "pig_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
		}
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enter_gacha", string.Empty);
	}

	private void EvaluateSelectClassTutorial(string trigger, string tutIdent)
	{
		if (trigger != "birdmanager_entered")
		{
			return;
		}
		DebugLog.Log("Start Tutorial: " + tutIdent);
		CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
		InventoryItemSlot[] componentsInChildren = campStateMgr.m_BirdManager.getItemGrid().GetComponentsInChildren<InventoryItemSlot>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].GetModel().Name == "bird_class_guardian")
			{
				componentsInChildren[i].m_InputTrigger.Clicked -= Classclicked(tutIdent, componentsInChildren[i]);
				componentsInChildren[i].m_InputTrigger.Clicked += Classclicked(tutIdent, componentsInChildren[i]);
				HideHelp();
				ShowHelp(componentsInChildren[i].transform, tutIdent, 0f, 0f);
			}
		}
	}

	private Action WorldMapClicked(string tutIdent)
	{
		return delegate
		{
			OnWorldMapClicked(tutIdent);
		};
	}

	private void OnWorldMapClicked(string tutIdent)
	{
		DebugLog.Log("WorldMap Clicked");
		HideHelp(tutIdent, false);
	}

	private Action Classclicked(string tutIdent, InventoryItemSlot birdClass)
	{
		return delegate
		{
			OnClassClicked(tutIdent, birdClass);
		};
	}

	private void OnClassClicked(string tutIdent, InventoryItemSlot birdClass)
	{
		birdClass.m_InputTrigger.Clicked -= Classclicked(tutIdent, birdClass);
		if (m_TutorialStepFinishedActions.ContainsKey(tutIdent))
		{
			m_TutorialStepFinishedActions[tutIdent]("class_clicked", string.Empty);
		}
	}

	private void FinishSelectClassTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "class_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		DebugLog.Log("Done Tutorial: " + tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
			}
		}
	}

	private void EvaluateEnterBirdManagerTutorial(string trigger, string tutIdent)
	{
		if (trigger != "enter_camp")
		{
			return;
		}
		DebugLog.Log("Start Tutorial: " + tutIdent);
		CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
		List<CharacterControllerCamp> birds = campStateMgr.getBirds();
		campStateMgr.m_CampUI.WorldMapButton.Clicked -= WorldMapClicked(tutIdent);
		campStateMgr.m_CampUI.WorldMapButton.Clicked += WorldMapClicked(tutIdent);
		for (int i = 0; i < birds.Count; i++)
		{
			if (birds[i].GetModel().Name == "bird_red")
			{
				birds[i].BirdClicked -= BirdClicked(tutIdent, birds[i]);
				birds[i].BirdClicked += BirdClicked(tutIdent, birds[i]);
				HideHelp();
				ShowHelp(birds[i].m_AssetController.BodyCenter, tutIdent, 0f, 0f);
			}
		}
	}

	private Action<ICharacter> BirdClicked(string tutIdent, CharacterControllerCamp bird)
	{
		return delegate
		{
			OnBirdClicked(tutIdent, bird);
		};
	}

	private void OnBirdClicked(string tutIdent, CharacterControllerCamp bird)
	{
		bird.BirdClicked -= BirdClicked(tutIdent, bird);
		DebugLog.Log(tutIdent);
		if (m_TutorialStepFinishedActions.ContainsKey(tutIdent))
		{
			m_TutorialStepFinishedActions[tutIdent]("bird_clicked", string.Empty);
		}
	}

	private void FinishEnterBirdManagerTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "bird_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		DebugLog.Log("Done Tutorial: " + tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
			}
		}
	}

	private void EvaluateClickWorldMap(string trigger, string tutIdent)
	{
		if (!(trigger != "forge_left"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			HideHelp();
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_CampUI.WorldMapButton.Clicked -= WorldMapButtonClicked(tutIdent, campStateMgr);
			campStateMgr.m_CampUI.WorldMapButton.Clicked += WorldMapButtonClicked(tutIdent, campStateMgr);
			ShowHelp(campStateMgr.m_CampUI.WorldMapButton.transform, tutIdent, 0f, 0f);
		}
	}

	private Action WorldMapButtonClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate
		{
			OnWorldMapButtonClicked(tutIdent, cstate);
		};
	}

	private void OnWorldMapButtonClicked(string trigger, CampStateMgr cstate)
	{
		cstate.m_ForgeWindow.ExitButtonClicked -= ForgeScreenLeaveButtonClicked(trigger, cstate);
		DebugLog.Log(trigger);
		if (m_TutorialStepFinishedActions.ContainsKey(trigger))
		{
			m_TutorialStepFinishedActions[trigger]("worldmap_clicked", string.Empty);
		}
	}

	private void FinishClickWorldMapTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "worldmap_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		DebugLog.Log("Done Tutorial: " + tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
			}
		}
	}

	private void EvaluateEquippedCraftedItemTutorial(string trigger, string tutIdent)
	{
		if (!(trigger != "crafting_finished"))
		{
			HideHelp();
			DebugLog.Log("Start Tutorial: tutorial_forge_04");
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_ForgeResultWindow.EquipBirdClicked -= ForgeResultWindowOnEquipBirdClicked(tutIdent, campStateMgr);
			campStateMgr.m_ForgeResultWindow.EquipBirdClicked += ForgeResultWindowOnEquipBirdClicked(tutIdent, campStateMgr);
			campStateMgr.m_ForgeResultWindow.ConfirmedCraftingClicked -= ForgeResultWindowOnConfirmedCraftingClicked(tutIdent, campStateMgr);
			campStateMgr.m_ForgeResultWindow.ConfirmedCraftingClicked += ForgeResultWindowOnConfirmedCraftingClicked(tutIdent, campStateMgr);
			ShowHelp(campStateMgr.m_ForgeResultWindow.m_EquipButton.transform, tutIdent, 0f, 0f);
		}
	}

	private Action<BirdGameData> ForgeResultWindowOnEquipBirdClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate(BirdGameData c)
		{
			OnEquipBirdClicked(tutIdent, c, cstate);
		};
	}

	private void FinishEquippedCraftedItemTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger == "craft_confirmed")
		{
			HideHelp(tutIdent, true);
			FinishTutorialStep(tutIdent);
		}
		else
		{
			if (trigger != "craft_birdequipped")
			{
				return;
			}
			HideHelp(tutIdent, true);
			FinishTutorialStep(tutIdent);
			DebugLog.Log("Done Tutorial: " + tutIdent);
			foreach (string followUpIdent in followUpIdents)
			{
				if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
				{
					DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
				}
				else
				{
					DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
				}
			}
			ShowTutorialGuideIfNecessary("crafted_equipped", string.Empty);
		}
	}

	private void OnEquipBirdClicked(string trigger, BirdGameData c, CampStateMgr cstate)
	{
		cstate.m_ForgeResultWindow.EquipBirdClicked -= ForgeResultWindowOnEquipBirdClicked(trigger, cstate);
		ShowTutorialGuideIfNecessary("craft_birdequipped", c.Data.NameId);
	}

	private void EvaluateCraftPressedTutorial(string trigger, string tutIdent, string possibleTrigger, string category)
	{
		if (!(trigger != possibleTrigger))
		{
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			if (!(campStateMgr.m_ForgeWindow.m_selectedCategory.ToString().ToLower() != category))
			{
				DebugLog.Log("Start Tutorial: " + tutIdent);
				campStateMgr.m_ForgeWindow.CraftItemClicked -= ForgeWindowOnCraftItemClicked(tutIdent, campStateMgr);
				campStateMgr.m_ForgeWindow.CraftItemClicked += ForgeWindowOnCraftItemClicked(tutIdent, campStateMgr);
				HideHelp();
				ShowHelp(campStateMgr.m_ForgeWindow.m_CraftButton.transform, tutIdent, 0f, 0f);
			}
		}
	}

	private void EvaluatePotionCraftPressedTutorial(string trigger, string tutIdent, string possibleTrigger)
	{
		if (!(trigger != possibleTrigger))
		{
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			DebugLog.Log("Start Tutorial: " + tutIdent);
			campStateMgr.m_ForgeWindow.CraftItemClicked -= ForgeWindowOnCraftItemClicked(tutIdent, campStateMgr);
			campStateMgr.m_ForgeWindow.CraftItemClicked += ForgeWindowOnCraftItemClicked(tutIdent, campStateMgr);
			HideHelp();
			ShowHelp(campStateMgr.m_ForgeWindow.m_CraftButton.transform, tutIdent, 0f, 0f);
		}
	}

	private Action<IInventoryItemGameData> ForgeWindowOnCraftItemClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate(IInventoryItemGameData c)
		{
			OnCraftClicked(tutIdent, c, cstate);
		};
	}

	private void FinishCraftPressedTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger == "craft_category_switched")
		{
			HideHelp();
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey("tutorial_forge_02_B") && DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks["tutorial_forge_02_B"] == 1)
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks["tutorial_forge_02_B"] = 0;
			}
		}
		else
		{
			if (trigger != "craft_clicked")
			{
				return;
			}
			HideHelp(tutIdent, true);
			FinishTutorialStep(tutIdent);
			DebugLog.Log("Done Tutorial: " + tutIdent);
			foreach (string followUpIdent in followUpIdents)
			{
				if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
				{
					DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
				}
				else
				{
					DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
				}
			}
		}
	}

	private void OnCraftClicked(string trigger, IInventoryItemGameData data, CampStateMgr cstate)
	{
		cstate.m_ForgeWindow.CraftItemClicked -= ForgeWindowOnCraftItemClicked(trigger, cstate);
		ShowTutorialGuideIfNecessary("craft_clicked", data.ItemBalancing.NameId);
	}

	private void FinishEnterCampTutorial(string trigger, string tutorialIdent)
	{
		if (!(trigger != "enter_camp_clicked"))
		{
			HideHelp(tutorialIdent, true);
			FinishTutorialStep(tutorialIdent);
		}
	}

	private void EvaluateEnterCampTutorial(string trigger, string tutorialIdent)
	{
		if (!(trigger != "enter_worldmap"))
		{
			HideHelp();
			WorldMapStateMgr worldMapStateMgr = UnityEngine.Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
			ShowHelp(worldMapStateMgr.m_WorldMenuUI.m_CampButton.transform, tutorialIdent, 0f, 0f);
			DIContainerInfrastructure.GetCoreStateMgr().CampEntered -= OnCampEntered(tutorialIdent);
			DIContainerInfrastructure.GetCoreStateMgr().CampEntered += OnCampEntered(tutorialIdent);
		}
	}

	private void EvaluateClickFriendlist(string trigger, string tutIdent, string possibleTrigger)
	{
		if (!(trigger != possibleTrigger))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			HideHelp();
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_CampUI.FriendListButton.Clicked -= FriendlistButtonClicked(tutIdent, campStateMgr);
			campStateMgr.m_CampUI.FriendListButton.Clicked += FriendlistButtonClicked(tutIdent, campStateMgr);
			ShowHelp(campStateMgr.m_CampUI.FriendListButton.transform, tutIdent, 0f, 0f);
		}
	}

	private Action FriendlistButtonClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate
		{
			OnFriendlistButtonClicked(tutIdent, cstate);
		};
	}

	private void OnFriendlistButtonClicked(string tutIdent, CampStateMgr cstate)
	{
		cstate.m_CampUI.FriendListButton.Clicked -= FriendlistButtonClicked(tutIdent, cstate);
		DebugLog.Log(tutIdent);
		if (m_TutorialStepFinishedActions.ContainsKey(tutIdent))
		{
			m_TutorialStepFinishedActions[tutIdent]("friendlist_clicked", string.Empty);
		}
	}

	private void FinishClickFriendlist(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "friendlist_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		DebugLog.Log("Done Tutorial: " + tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
			}
		}
	}

	private void EvaluateClickNPCFriend(string trigger, string tutIdent)
	{
		if (trigger != "enter_friendlist")
		{
			return;
		}
		DebugLog.Log("Start Tutorial: " + tutIdent);
		HideHelp();
		FriendBirdWindowStateMgr friendBirdWindowStateMgr = UnityEngine.Object.FindObjectOfType(typeof(FriendBirdWindowStateMgr)) as FriendBirdWindowStateMgr;
		FriendVisitingBlind[] componentsInChildren = friendBirdWindowStateMgr.getGrid().GetComponentsInChildren<FriendVisitingBlind>();
		DebugLog.Log(componentsInChildren.Length);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].GetModel().isNpcFriend)
			{
				componentsInChildren[i].m_VisitFriend.Clicked -= FriendBlindClicked(tutIdent, friendBirdWindowStateMgr, componentsInChildren[i]);
				componentsInChildren[i].m_VisitFriend.Clicked += FriendBlindClicked(tutIdent, friendBirdWindowStateMgr, componentsInChildren[i]);
				ShowHelp(componentsInChildren[i].transform, tutIdent, 0f, 0f);
			}
		}
	}

	private Action FriendBlindClicked(string tutIdent, FriendBirdWindowStateMgr fstate, FriendVisitingBlind blind)
	{
		return delegate
		{
			OnFriendBlindClicked(tutIdent, fstate, blind);
		};
	}

	private void OnFriendBlindClicked(string tutIdent, FriendBirdWindowStateMgr fstate, FriendVisitingBlind blind)
	{
		blind.m_VisitFriend.Clicked -= FriendBlindClicked(tutIdent, fstate, blind);
		DebugLog.Log(tutIdent);
		if (m_TutorialStepFinishedActions.ContainsKey(tutIdent))
		{
			m_TutorialStepFinishedActions[tutIdent]("friendblind_clicked", string.Empty);
		}
	}

	private void FinishClickNPCFriend(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "friendblind_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		DebugLog.Log("Done Tutorial: " + tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
			}
		}
	}

	private void EvaluateGoBackToCampTutorial(string trigger, string tutIdent)
	{
		if (!(trigger != "friend_gacha_finished"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			campStateMgr.m_FriendCampUI.m_BackButton.Clicked -= FriendBackButtonClicked(tutIdent, campStateMgr);
			campStateMgr.m_FriendCampUI.m_BackButton.Clicked += FriendBackButtonClicked(tutIdent, campStateMgr);
			HideHelp();
			ShowHelp(campStateMgr.m_FriendCampUI.m_BackButton.transform, tutIdent, 0f, 0f);
		}
	}

	private Action FriendBackButtonClicked(string tutIdent, CampStateMgr cstate)
	{
		return delegate
		{
			OnFriendBackButtonClicked(tutIdent, cstate);
		};
	}

	private void OnFriendBackButtonClicked(string tutIdent, CampStateMgr cstate)
	{
		cstate.m_FriendCampUI.m_BackButton.Clicked -= FriendBackButtonClicked(tutIdent, cstate);
		if (m_TutorialStepFinishedActions.ContainsKey(tutIdent))
		{
			m_TutorialStepFinishedActions[tutIdent]("friend_camp_left", string.Empty);
		}
	}

	private void FinishGoBackToCampTutorial(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "friend_camp_left")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
		}
	}

	private void EvaluateClickAddFriend(string trigger, string tutIdent)
	{
		if (!(trigger != "enter_friendlist"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			HideHelp();
			FriendBirdWindowStateMgr friendBirdWindowStateMgr = UnityEngine.Object.FindObjectOfType(typeof(FriendBirdWindowStateMgr)) as FriendBirdWindowStateMgr;
			FriendAddBlind[] componentsInChildren = friendBirdWindowStateMgr.getGrid().GetComponentsInChildren<FriendAddBlind>();
			DebugLog.Log(componentsInChildren.Length);
			if (componentsInChildren.Length > 0)
			{
				componentsInChildren[0].m_AddFriend.Clicked -= AddFriendClicked(tutIdent, friendBirdWindowStateMgr, componentsInChildren[0]);
				componentsInChildren[0].m_AddFriend.Clicked += AddFriendClicked(tutIdent, friendBirdWindowStateMgr, componentsInChildren[0]);
				ShowHelp(componentsInChildren[0].m_AddFriend.transform, tutIdent, 0f, 0f);
			}
		}
	}

	private Action AddFriendClicked(string tutIdent, FriendBirdWindowStateMgr fstate, FriendAddBlind blind)
	{
		return delegate
		{
			OnAddFriendClicked(tutIdent, fstate, blind);
		};
	}

	private void OnAddFriendClicked(string tutIdent, FriendBirdWindowStateMgr fstate, FriendAddBlind blind)
	{
		blind.m_AddFriend.Clicked -= AddFriendClicked(tutIdent, fstate, blind);
		DebugLog.Log(tutIdent);
		if (m_TutorialStepFinishedActions.ContainsKey(tutIdent))
		{
			m_TutorialStepFinishedActions[tutIdent]("addfriend_clicked", string.Empty);
		}
	}

	private void FinishClickAddFriend(string trigger, string tutIdent, List<string> followUpIdents)
	{
		if (trigger != "addfriend_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		DebugLog.Log("Done Tutorial: " + tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
			}
		}
	}

	private void EvaluateEnterHotspotTutorial(string trigger, string tutIdent, string param, string possibleParam)
	{
		if (trigger == "enter_worldmap")
		{
			param = possibleParam;
		}
		else if (param != possibleParam)
		{
			return;
		}
		DebugLog.Log("Start Tutorial: " + tutIdent);
		WorldMapStateMgr worldMapStateMgr = UnityEngine.Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
		HotSpotWorldMapViewBase hotspotWorldMapView = worldMapStateMgr.m_startingHotSpot.GetHotspotWorldMapView(param);
		if (hotspotWorldMapView.IsCompleted())
		{
			DebugLog.Log("Hotspot: " + hotspotWorldMapView.m_nameId);
			ShowHelp(hotspotWorldMapView.transform, tutIdent, 0f, 0f);
			hotspotWorldMapView.HotspotClicked -= HotspotOnHotspotClicked(tutIdent);
			hotspotWorldMapView.HotspotClicked += HotspotOnHotspotClicked(tutIdent);
		}
	}

	private void EvaluateClickHotspotTutorial(string trigger, string tutIdent, string param, string possibleParam)
	{
		if (trigger != "unlocked_hotspot" && trigger != "enter_worldmap")
		{
			return;
		}
		if (trigger == "unlocked_hotspot")
		{
			if (param != possibleParam)
			{
				return;
			}
		}
		else if (trigger == "enter_worldmap")
		{
			param = possibleParam;
		}
		WorldMapStateMgr worldMapStateMgr = UnityEngine.Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
		HotSpotWorldMapViewBase hotspotWorldMapView = worldMapStateMgr.m_startingHotSpot.GetHotspotWorldMapView(param);
		if (hotspotWorldMapView.Model.IsActive())
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			ShowHelp(hotspotWorldMapView.transform, tutIdent, 0f, 0f);
			hotspotWorldMapView.HotspotClicked -= HotspotOnHotspotClicked(tutIdent);
			hotspotWorldMapView.HotspotClicked += HotspotOnHotspotClicked(tutIdent);
		}
	}

	private void FinishClickHotspotTutorial(string trigger, string tutIdent, List<string> followUpIdents, string param, string possibleParam)
	{
		if (trigger != "hotspot_clicked" || param != possibleParam)
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
		}
	}

	private Action<HotSpotWorldMapViewBase> HotspotOnHotspotClicked(string tutIdent)
	{
		return delegate(HotSpotWorldMapViewBase c)
		{
			OnHotspotClicked(tutIdent, c);
		};
	}

	private void OnHotspotClicked(string tutIdent, HotSpotWorldMapViewBase hotspot)
	{
		hotspot.HotspotClicked -= HotspotOnHotspotClicked(tutIdent);
		if (m_TutorialStepFinishedActions.ContainsKey(tutIdent))
		{
			m_TutorialStepFinishedActions[tutIdent]("hotspot_clicked", hotspot.Model.BalancingData.NameId);
		}
	}

	private void FinishEnterHotspotTutorial(string trigger, string tutIdent, List<string> followUpIdents, string param, string possibleParam)
	{
		if (trigger != "hotspot_clicked" || param != possibleParam)
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
		}
	}

	private void EvaluateEnterBattleTutorial(string trigger, string tutIdent, string param)
	{
		if (!(trigger != "battle_preparation"))
		{
			BattlePreperationUI battlePreperationUI = UnityEngine.Object.FindObjectOfType(typeof(BattlePreperationUI)) as BattlePreperationUI;
			battlePreperationUI.m_startButtonTrigger.Clicked -= StartButtonClicked(tutIdent, battlePreperationUI);
			battlePreperationUI.m_startButtonTrigger.Clicked += StartButtonClicked(tutIdent, battlePreperationUI);
			DebugLog.Log("Start Tutorial: " + tutIdent);
			ShowHelp(battlePreperationUI.m_startButtonTrigger.transform, tutIdent, 0f, 0f);
		}
	}

	private Action StartButtonClicked(string tutIdent, BattlePreperationUI battleUI)
	{
		return delegate
		{
			OnStartButtonClicked(tutIdent, battleUI);
		};
	}

	private void OnStartButtonClicked(string tutIdent, BattlePreperationUI battleUI)
	{
		battleUI.m_startButtonTrigger.Clicked -= StartButtonClicked(tutIdent, battleUI);
		if (m_TutorialStepFinishedActions.ContainsKey(tutIdent))
		{
			m_TutorialStepFinishedActions[tutIdent]("startButton_clicked", string.Empty);
		}
	}

	private void FinishEnterBattleTutorial(string trigger, string tutIdent, List<string> followUpIdents, string param)
	{
		if (trigger != "startButton_clicked")
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
		}
	}

	private void EvaluateTapResourceTutorial(string trigger, string tutIdent, string param, string possibleParam)
	{
		if (!(trigger != "resourceSpot_respawn") && !(param != possibleParam))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			WorldMapStateMgr worldMapStateMgr = UnityEngine.Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
			HotSpotWorldMapViewBase hotspotWorldMapView = worldMapStateMgr.m_startingHotSpot.GetHotspotWorldMapView(param);
			DebugLog.Log("Start Tutorial: " + tutIdent);
			ShowHelp(hotspotWorldMapView.transform, tutIdent, 0f, 0f);
			hotspotWorldMapView.HotspotClicked -= HotspotOnHotspotClicked(tutIdent);
			hotspotWorldMapView.HotspotClicked += HotspotOnHotspotClicked(tutIdent);
		}
	}

	private void FinishTapResourceTutorial(string trigger, string tutIdent, List<string> followUpIdents, string param, string possibleParam)
	{
		if (trigger != "hotspot_clicked" || param != possibleParam)
		{
			return;
		}
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
		}
	}

	private void FinishBuyWorkshopTutorial(string trigger, string tutIdent, List<string> followUpIdents, string param, string possibleParam)
	{
		if (trigger != "offer_clicked")
		{
			return;
		}
		DebugLog.Log("Finish Tutorial: " + tutIdent);
		HideHelp(tutIdent, true);
		FinishTutorialStep(tutIdent);
		foreach (string followUpIdent in followUpIdents)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(followUpIdent))
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(followUpIdent, 0);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[followUpIdent] = 0;
			}
		}
		ShowTutorialGuideIfNecessary("shopOffer_bought", string.Empty);
	}

	private void EvaluateBuyWorkshopItemTutorial(string trigger, string tutIdent, string param, string possibleParam)
	{
		if (trigger != "entered_workshop")
		{
			return;
		}
		DebugLog.Log("Start Tutorial: " + tutIdent);
		WorldMapStateMgr worldMapStateMgr = UnityEngine.Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
		WorldMapShopMenuUI workShopUI = worldMapStateMgr.m_WorkShopUI;
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(ShopOfferBlindWorldmap));
		HideHelp(tutIdent, false);
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			ShopOfferBlindWorldmap shopOfferBlindWorldmap = (ShopOfferBlindWorldmap)array2[i];
			DebugLog.Log("offer: " + shopOfferBlindWorldmap.GetModel().NameId);
			if (shopOfferBlindWorldmap.GetModel().NameId == possibleParam)
			{
				shopOfferBlindWorldmap.ShopOfferBought -= OfferOnShopOfferBought(tutIdent, shopOfferBlindWorldmap);
				shopOfferBlindWorldmap.ShopOfferBought += OfferOnShopOfferBought(tutIdent, shopOfferBlindWorldmap);
				ShowHelp(shopOfferBlindWorldmap.m_BuyButtonTrigger.transform, tutIdent, 0f, 0f);
			}
		}
	}

	private Action<BasicShopOfferBalancingData> OfferOnShopOfferBought(string tutIdent, ShopOfferBlindWorldmap offer)
	{
		return delegate(BasicShopOfferBalancingData o)
		{
			OnOfferBought(tutIdent, o, offer);
		};
	}

	private void OnOfferBought(string tutIdent, BasicShopOfferBalancingData offerbd, ShopOfferBlindWorldmap offer)
	{
		offer.ShopOfferBought -= OfferOnShopOfferBought(tutIdent, offer);
		if (m_TutorialStepFinishedActions.ContainsKey(tutIdent))
		{
			m_TutorialStepFinishedActions[tutIdent]("offer_clicked", offerbd.NameId);
		}
	}

	private void FinishEnterCampByButtonTutorial(string trigger, string tutorialIdent)
	{
		if (!(trigger != "enter_camp_clicked"))
		{
			HideHelp(tutorialIdent, true);
			FinishTutorialStep(tutorialIdent);
		}
	}

	private void EvaluateBackButtonPressedTutorial(string trigger, string tutIdent, string param, string possibleParam)
	{
		if (!(trigger != "shopOffer_bought"))
		{
			DebugLog.Log("Start Tutorial: " + tutIdent);
			WorldMapStateMgr worldMapStateMgr = UnityEngine.Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
			WorldMapShopMenuUI workShopUI = worldMapStateMgr.m_WorkShopUI;
			HideHelp(tutIdent, false);
			workShopUI.BackButtonPressed -= BackButtonPressed(tutIdent, workShopUI);
			workShopUI.BackButtonPressed += BackButtonPressed(tutIdent, workShopUI);
			ShowHelp(workShopUI.m_BackButton.transform, tutIdent, 0f, 0f);
		}
	}

	private Action BackButtonPressed(string tutIdent, WorldMapShopMenuUI wmshop)
	{
		return delegate
		{
			OnBackButtonPressed(tutIdent, wmshop);
		};
	}

	private void OnBackButtonPressed(string tutIdent, WorldMapShopMenuUI wmshop)
	{
		wmshop.BackButtonPressed -= BackButtonPressed(tutIdent, wmshop);
		if (m_TutorialStepFinishedActions.ContainsKey(tutIdent))
		{
			m_TutorialStepFinishedActions[tutIdent]("backButton_clicked", tutIdent);
		}
	}

	private void FinishBackButtonPressedTutorial(string trigger, string tutorialIdent)
	{
		if (!(trigger != "backButton_clicked"))
		{
			HideHelp(tutorialIdent, true);
			FinishTutorialStep(tutorialIdent);
			ShowTutorialGuideIfNecessary("enter_worldmap", string.Empty);
		}
	}

	private void EvaluateEnterCampByButtonTutorial(string trigger, string tutorialIdent)
	{
		if (!(trigger != "enter_worldmap_ui"))
		{
			DebugLog.Log("Start Tutorial: " + tutorialIdent);
			WorldMapStateMgr worldMapStateMgr = UnityEngine.Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
			ShowHelp(worldMapStateMgr.m_WorldMenuUI.m_CampButton.transform, tutorialIdent, 0f, 0f);
			DIContainerInfrastructure.GetCoreStateMgr().CampEntered -= OnCampEntered(tutorialIdent);
			DIContainerInfrastructure.GetCoreStateMgr().CampEntered += OnCampEntered(tutorialIdent);
		}
	}

	private Action OnCampEntered(string tutorialIdent)
	{
		return delegate
		{
			TutorialMgr_CampEntered(tutorialIdent);
		};
	}

	private void TutorialMgr_CampEntered(string tutorialIdent)
	{
		if (m_TutorialStepFinishedActions.ContainsKey(tutorialIdent))
		{
			m_TutorialStepFinishedActions[tutorialIdent]("enter_camp_clicked", string.Empty);
		}
		DIContainerInfrastructure.GetCoreStateMgr().CampEntered -= OnCampEntered(tutorialIdent);
	}

	private void EvaluateOpenPigGateTutorial(string trigger, string tutIdent, string param, string possibleParam)
	{
		if (!(trigger != "enter_worldmap"))
		{
			WorldMapStateMgr worldMapStateMgr = UnityEngine.Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
			HotSpotWorldMapViewBase hotspotWorldMapView = worldMapStateMgr.m_startingHotSpot.GetHotspotWorldMapView(possibleParam);
			DebugLog.Log("Start Tutorial: " + tutIdent);
			ShowHelp(hotspotWorldMapView.transform, tutIdent, 0f, 0f);
			hotspotWorldMapView.HotspotClicked -= HotspotOnHotspotClicked(tutIdent);
			hotspotWorldMapView.HotspotClicked += HotspotOnHotspotClicked(tutIdent);
		}
	}

	public void InitPlayerTutorialTracks()
	{
	}
}
