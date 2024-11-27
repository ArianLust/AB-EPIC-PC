using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.Shared.Generic;
using UnityEngine;

public class TutorialMgr : MonoBehaviour, ITutorialMgr
{
	private Dictionary<string, TutorialTemplate> m_TutorialTemplates = new Dictionary<string, TutorialTemplate>();

	private Dictionary<string, Tutorial> m_TutorialProgress = new Dictionary<string, Tutorial>();

	private Dictionary<string, int> m_CachedTutorialTracks = new Dictionary<string, int>();

	private Dictionary<string, Action<string, string>> m_TutorialStepStartActions = new Dictionary<string, Action<string, string>>();

	private Dictionary<string, Action<string, string>> m_TutorialStepFinishedActions = new Dictionary<string, Action<string, string>>();

	private Dictionary<string, GuideController> m_ActiveGuides = new Dictionary<string, GuideController>();

	private GuideController CurrentDragAndDropGuide;

	public float GuideZPosition = -200f;

	[SerializeField]
	private List<TriggerStepTypeHelperMapping> m_TriggerMapping = new List<TriggerStepTypeHelperMapping>();

	[SerializeField]
	private GameObject m_FinishedMarker;

	private CharacterControllerBattleGroundBase m_cachedOverCharacter;

	private GlowController m_CurrentGlow;

	private CharacterControlHUD m_CurrentControlHUD;

	private Transform m_cachedOverCharacterTransform;

	private bool m_IsCurrentlyLocked;

	private bool m_DragVisualizationLocked;

	private BattleMgrBase m_battleMgr;

	private Transform m_sourceRoot;

	private CharacterControllerBattleGroundBase m_character;

	private Transform m_source;

	private Transform m_target;

	private CHMotionTween m_motionTween;

	private GuideController m_guide;

	public Dictionary<string, TutorialTemplate> TutorialTemplates
	{
		get
		{
			return m_TutorialTemplates;
		}
	}

	public bool IsCurrentlyLocked
	{
		get
		{
			return m_IsCurrentlyLocked;
		}
		set
		{
			if (value)
			{
				if (m_battleMgr == null)
				{
					m_battleMgr = UnityEngine.Object.FindObjectOfType(typeof(BattleMgr)) as BattleMgr;
				}
				if (m_battleMgr != null)
				{
					m_battleMgr.AutoBattle = false;
				}
			}
			m_IsCurrentlyLocked = value;
		}
	}

	public void SetTutorialCameras(bool activate)
	{
		if (!DIContainerInfrastructure.GetCoreStateMgr())
		{
			return;
		}
		TouchInputHandler component = DIContainerInfrastructure.GetCoreStateMgr().GetComponent<TouchInputHandler>();
		UICamera component2 = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera.GetComponent<UICamera>();
		IsCurrentlyLocked = activate;
		if ((bool)component)
		{
			component.SetTutorialLayers(activate);
		}
		if (activate)
		{
			if ((bool)component2)
			{
				component2.eventReceiverMask = (1 << LayerMask.NameToLayer("TutorialInterface")) | (1 << LayerMask.NameToLayer("IgnoreTutorialInterface")) | (1 << LayerMask.NameToLayer("InterfaceCharacter"));
			}
		}
		else if ((bool)component2)
		{
			component2.eventReceiverMask = (1 << LayerMask.NameToLayer("Interface")) | (1 << LayerMask.NameToLayer("IgnoreTutorialInterface")) | (1 << LayerMask.NameToLayer("InterfaceCharacter"));
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
			TriggerStepTypeHelperMapping triggerStepTypeHelperMapping = m_TriggerMapping.FirstOrDefault((TriggerStepTypeHelperMapping m) => m.StepType.ToString() == trigger);
			if (triggerStepTypeHelperMapping != null && !(pos == null))
			{
				guideController = (GuideController)UnityEngine.Object.Instantiate(triggerStepTypeHelperMapping.Guide, pos.position, Quaternion.identity);
				guideController.gameObject.transform.parent = pos;
				guideController.gameObject.transform.position = new Vector3(pos.position.x + triggerStepTypeHelperMapping.Offsets.x, pos.position.y + triggerStepTypeHelperMapping.Offsets.y + yOffset, GuideZPosition);
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
		TriggerStepTypeHelperMapping triggerStepTypeHelperMapping = m_TriggerMapping.FirstOrDefault((TriggerStepTypeHelperMapping m) => m.StepType.ToString() == trigger);
		if (triggerStepTypeHelperMapping != null && !(source == null))
		{
			guideController = (GuideController)UnityEngine.Object.Instantiate(triggerStepTypeHelperMapping.Guide, source.position, Quaternion.identity);
			guideController.gameObject.transform.position = new Vector3(source.position.x + triggerStepTypeHelperMapping.Offsets.x, source.position.y + triggerStepTypeHelperMapping.Offsets.y, GuideZPosition);
			guideController.gameObject.layer = source.gameObject.layer;
			SetLayerRecusively(guideController.gameObject, source.gameObject.layer);
			guideController.Enter(trigger);
			m_ActiveGuides.Add(trigger, guideController);
			CurrentDragAndDropGuide = guideController.SetThumbUpPosition(new Vector3(target.position.x, target.position.y, GuideZPosition));
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
		if ((bool)CurrentDragAndDropGuide && m_DragVisualizationLocked != locked)
		{
			DebugLog.Log("Drag visual blocked: " + locked);
			CurrentDragAndDropGuide.gameObject.SetActive(!locked);
			if (locked)
			{
				StopCoroutine("EvaluateDragProgress");
				CurrentDragAndDropGuide.m_Animation.Stop();
				if ((bool)m_CurrentControlHUD)
				{
					m_CurrentControlHUD.ResetControlHUD();
				}
				m_motionTween.Stop();
				m_motionTween.Reset();
			}
			else
			{
				StopCoroutine("EvaluateDragProgress");
				StartCoroutine("EvaluateDragProgress");
			}
		}
		m_DragVisualizationLocked = locked;
	}

	private IEnumerator RepeatMoveFromTo(BattleMgrBase battleMgr, Transform sourceRoot, CharacterControllerBattleGroundBase character, Transform source, Transform target, GuideController c, float zOffset)
	{
		CHMotionTween motionTween = c.GetComponentInChildren<CHMotionTween>();
		if (!motionTween)
		{
			yield break;
		}
		motionTween.m_StartTransform = source;
		motionTween.m_StartOffset = new Vector3(0f, 0f, zOffset);
		motionTween.m_EndTransform = target;
		motionTween.m_EndOffset = new Vector3(0f, 0f, zOffset);
		motionTween.m_DurationInSeconds = 1.5f;
		m_battleMgr = battleMgr;
		m_sourceRoot = sourceRoot;
		m_character = character;
		m_source = source;
		m_target = target;
		m_motionTween = motionTween;
		m_guide = c;
		DebugLog.Log("Drag visual is blocked on instantiate: " + m_DragVisualizationLocked);
		if ((bool)CurrentDragAndDropGuide)
		{
			CurrentDragAndDropGuide.gameObject.SetActive(!m_DragVisualizationLocked);
		}
		if ((bool)m_CurrentControlHUD)
		{
			m_CurrentControlHUD.gameObject.SetActive(false);
			m_CurrentControlHUD = null;
		}
		if (!m_DragVisualizationLocked)
		{
			StopCoroutine("EvaluateDragProgress");
			StartCoroutine("EvaluateDragProgress");
			yield break;
		}
		StopCoroutine("EvaluateDragProgress");
		if ((bool)CurrentDragAndDropGuide)
		{
			CurrentDragAndDropGuide.m_Animation.Stop();
		}
		if ((bool)m_CurrentControlHUD)
		{
			m_CurrentControlHUD.ResetControlHUD();
		}
		m_motionTween.Stop();
		m_motionTween.Reset();
	}

	private IEnumerator EvaluateDragProgress()
	{
		if ((bool)m_character)
		{
			if ((bool)m_character.GetControlHUD())
			{
				m_character.ActivateControlHUD(true);
			}
		}
		else
		{
			m_CurrentControlHUD = null;
		}
		if (m_guide.m_Animation.Play("Guide_Drag_Tab"))
		{
			yield return new WaitForSeconds(m_guide.m_Animation["Guide_Drag_Tab"].length);
		}
		if (!m_motionTween)
		{
			yield break;
		}
		m_motionTween.Play();
		while ((bool)m_motionTween && m_motionTween.IsPlaying)
		{
			if ((bool)m_battleMgr)
			{
				ShowCharacterControlHUD(m_battleMgr, m_character, m_sourceRoot, (m_sourceRoot.gameObject.layer != LayerMask.NameToLayer("Interface") && m_sourceRoot.gameObject.layer != LayerMask.NameToLayer("TutorialInterface")) ? m_battleMgr.m_SceneryCamera.WorldToScreenPoint(m_motionTween.transform.position) : DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera.WorldToScreenPoint(m_motionTween.transform.position));
			}
			yield return new WaitForEndOfFrame();
		}
		if (!m_motionTween)
		{
			yield break;
		}
		if (m_guide.m_Animation.Play("Guide_Drag_Release"))
		{
			yield return new WaitForSeconds(m_guide.m_Animation["Guide_Drag_Release"].length);
		}
		if (!m_motionTween)
		{
			yield break;
		}
		if ((bool)m_battleMgr && !m_battleMgr.LockDragVisualizationByCode)
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
		m_motionTween.Reset();
		StartCoroutine("EvaluateDragProgress");
	}

	public void ShowCharacterControlHUD(BattleMgrBase battleMgr, CharacterControllerBattleGroundBase character, Transform rootPosition, Vector3 currentScreenPos)
	{
		if (m_CurrentGlow == null)
		{
			m_CurrentGlow = battleMgr.m_CurrentGlow;
			m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Show");
			m_CurrentGlow.gameObject.SetActive(false);
		}
		if (m_CurrentControlHUD == null && (bool)character)
		{
			m_CurrentControlHUD = character.GetControlHUD();
		}
		Ray ray = battleMgr.m_SceneryCamera.ScreenPointToRay(currentScreenPos);
		if ((bool)m_CurrentControlHUD)
		{
			m_CurrentControlHUD.transform.position = rootPosition.position;
			m_CurrentControlHUD.gameObject.SetActive(true);
		}
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
					if (m_cachedOverCharacter == null || m_cachedOverCharacter == character)
					{
						if (m_CurrentGlow.gameObject.activeInHierarchy)
						{
							m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Hide");
							Invoke("DisableGlow", m_CurrentGlow.GetComponent<Animation>()["CharacterSelectionGlow_Hide"].length);
						}
					}
					else
					{
						m_CurrentGlow.gameObject.SetActive(true);
						if ((bool)character)
						{
							m_CurrentGlow.SetStateColor((m_cachedOverCharacter.GetModel().CombatantFaction != 0) ? GlowState.Attack : GlowState.Support);
						}
						else
						{
							m_CurrentGlow.SetStateColor(GlowState.Neutral);
						}
						CancelInvoke("DisableGlow");
						m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Show");
					}
				}
			}
			if ((bool)m_cachedOverCharacter && m_cachedOverCharacter != character)
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
			if ((bool)m_CurrentControlHUD)
			{
				m_CurrentControlHUD.SetState(rootPosition, m_cachedOverCharacter, hitInfo.point, battleMgr);
			}
		}
		else
		{
			m_CurrentGlow.gameObject.SetActive(false);
		}
	}

	public void HideHelp()
	{
		foreach (GuideController value in m_ActiveGuides.Values)
		{
			if (value != null)
			{
				value.Leave();
				if (value.transform.parent != null)
				{
					ResetParentLayer(value.transform.parent);
				}
			}
		}
		m_ActiveGuides.Clear();
	}

	public void HideHelp(string trigger, bool finished)
	{
		GuideController value = null;
		if (!m_ActiveGuides.TryGetValue(trigger, out value))
		{
			return;
		}
		m_ActiveGuides.Remove(trigger);
		if (finished)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_FinishedMarker);
			Vector3 spawnPos = default(Vector3);
			if (value.TryGetFinishSpawnPos(out spawnPos))
			{
				gameObject.gameObject.transform.position = spawnPos;
			}
			else
			{
				gameObject.gameObject.transform.position = value.transform.position;
			}
			gameObject.gameObject.layer = value.gameObject.layer;
			SetLayerRecusively(gameObject.gameObject, value.gameObject.layer);
			UnityEngine.Object.Destroy(gameObject.gameObject, gameObject.GetComponent<Animation>().clip.length);
		}
		if ((bool)value)
		{
			value.Leave();
			if (value.transform.parent != null)
			{
				ResetParentLayer(value.transform.parent);
			}
		}
	}

	private void ResetParentLayer(Transform parent)
	{
		parent.gameObject.layer = LayerMask.NameToLayer(LayerMask.LayerToName(parent.gameObject.layer).Replace("Tutorial", string.Empty));
		SetTutorialCameras(false);
	}

	private void Start()
	{
		AddIntroBattleTutorial();
		AddStartFirstBattleTutorial();
		AddCraftFirstItemTutorial();
		AddEquipCraftedItemTutorial();
		AddGachaTooltipTutorial();
		AddCraftHealingPotionTutorial();
		AddUseConsumableTutorial();
		AddCraftSecondItemTutorial();
		AddCraftRerollTutorial();
		AddDefensiveAbilityTutorial();
		AddSaveYellowTutorial();
		AddUseWhiteTutorial();
		AddUseOffensiveYellowTutorial();
		AddUseDefensiveYellowTutorial();
		AddUseGachaTutorial();
		AddRerollBattleLootTutorial();
		AddBuyConsumableWorldMapTutorial();
		AddBuyMainHandEquipmentTutorial();
		AddBuyMainHandEquipmentTutorial2();
		AddBattleRuleTooltipTutorial();
		AddBuyClassTutorial();
		AddEquipClassTutorial();
		AddTooltipEnemyTutorial();
		AddHarvestTutorial();
		AddExitBuyClassTutorial();
		AddExitBuyMainHandEquipmentTutorial();
		AddReadPvPDailyObjectivesTutorial();
		AddPointOutNextHotspotTutorial();
		AddEnchantmentTutorial();
		AddEquipSkinTutorial();
		AddSkinShoppedTutorial();
		AddSkinShoppedWorldmapTutorial();
		InitPlayerTutorialTracks();
	}

	public void InitPlayerTutorialTracks()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks == null)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks = new Dictionary<string, int>();
			StartTutorial("tutorial_start_intro_battle");
			return;
		}
		foreach (string key in DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Keys)
		{
			StartTutorial(key);
		}
	}

	private void AddTooltipEnemyTutorial()
	{
		m_TutorialTemplates.Add("tutorial_tooltip_enemy", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnteredBattle,
					PossibleParameters = new List<string> { "battle_008" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.TapAndHold,
					PossibleParameters = new List<string>
					{
						string.Empty,
						"pigs"
					}
				}
			}
		});
	}

	private void AddEquipClassTutorial()
	{
		m_TutorialTemplates.Add("tutorial_equip_class", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.GoToHotspot,
					PossibleParameters = new List<string> { "hotspot_016_battleground" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ChangeClassInBps,
					PossibleParameters = new List<string>()
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SelectItem,
					PossibleParameters = new List<string> { "class_guardian" }
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_buy_mainhandequipment", "tutorial_exit_birdmanager" }
		});
	}

	private void AddBuyClassTutorial()
	{
		m_TutorialTemplates.Add("tutorial_buy_class", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.GoToHotspot,
					PossibleParameters = new List<string> { "hotspot_015_trainerhut" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.BuyWorldShopOffer,
					PossibleParameters = new List<string> { "offer_class_red_guardian_01" }
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_exit_buy_class" }
		});
	}

	private void AddExitBuyClassTutorial()
	{
		m_TutorialTemplates.Add("tutorial_exit_buy_class", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ExitWorldShop,
					PossibleParameters = new List<string> { "offer_class_yellow_lightningbird_01" },
					AutoStartStep = true
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_equip_class" },
			ForceStartNextTutorials = true
		});
	}

	private void AddBuyMainHandEquipmentTutorial()
	{
		m_TutorialTemplates.Add("tutorial_buy_mainhandequipment", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.OpenChest,
					PossibleParameters = new List<string> { "hotspot_016_battleground" }
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_buy_mainhandequipment_2" },
			ForceStartNextTutorials = true
		});
	}

	private void AddBuyMainHandEquipmentTutorial2()
	{
		m_TutorialTemplates.Add("tutorial_buy_mainhandequipment_2", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.GoToHotspot,
					PossibleParameters = new List<string> { "hotspot_017_workshop" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.BuyWorldShopOffer,
					PossibleParameters = new List<string> { "offer_recipe_offhand_yellow_orb_01" }
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_buy_consumable", "tutorial_craft_second_item", "tutorial_exit_buy_equipment" }
		});
	}

	private void AddExitBuyMainHandEquipmentTutorial()
	{
		m_TutorialTemplates.Add("tutorial_exit_buy_equipment", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ExitWorldShop,
					PossibleParameters = new List<string>(),
					AutoStartStep = true
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_craft_second_item" },
			ForceStartNextTutorials = true
		});
	}

	private void AddBuyConsumableWorldMapTutorial()
	{
		m_TutorialTemplates.Add("tutorial_buy_consumable", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.GoToHotspot,
					PossibleParameters = new List<string> { "hotspot_024_piglab" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.BuyWorldShopOffer,
					PossibleParameters = new List<string> { "offer_recipe_potion_rage_01" }
				}
			}
		});
	}

	private void AddUseDefensiveYellowTutorial()
	{
		m_TutorialTemplates.Add("tutorial_defensive_yellow", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnteredBattle,
					PossibleParameters = new List<string> { "battle_007" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.TapAndHold,
					PossibleParameters = new List<string> { "bird_yellow" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SupportOther,
					PossibleParameters = new List<string> { "bird_yellow", "bird_red" },
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.AttackPig,
					PossibleParameters = new List<string> { "bird_red" },
					AutoStartStep = true
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_tooltip_enemy" }
		});
	}

	private void AddUseOffensiveYellowTutorial()
	{
		m_TutorialTemplates.Add("tutorial_offensive_yellow", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.StartBattle,
					PossibleParameters = new List<string> { "hotspot_007_battleground" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnteredBattle,
					PossibleParameters = new List<string> { "battle_006" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.AttackPig,
					PossibleParameters = new List<string> { "bird_yellow", "pig_minion_rogue_bird1" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SupportOther,
					PossibleParameters = new List<string> { "bird_red", "bird_yellow" },
					AutoStartStep = true
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_defensive_yellow", "tutorial_white" }
		});
	}

	private void AddUseWhiteTutorial()
	{
		m_TutorialTemplates.Add("tutorial_white", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnteredBattle,
					PossibleParameters = new List<string> { "battle_020" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SupportSelf,
					PossibleParameters = new List<string> { "bird_white" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.AttackPig,
					PossibleParameters = new List<string> { "bird_white", "pig_caster_pyropig" }
				}
			}
		});
	}

	private void AddSaveYellowTutorial()
	{
		m_TutorialTemplates.Add("tutorial_save_yellow", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.GoToHotspotOrCamp,
					PossibleParameters = new List<string> { "hotspot_006_battleground" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnteredBattle,
					PossibleParameters = new List<string> { "battle_005" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnteredBattleWave,
					PossibleParameters = new List<string> { "2" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.DefeatPig,
					PossibleParameters = new List<string> { "bird_red", "pig_yellow_cage" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.FinishedBattle,
					PossibleParameters = new List<string>()
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_offensive_yellow" }
		});
	}

	private void AddDefensiveAbilityTutorial()
	{
		m_TutorialTemplates.Add("tutorial_defensive_ability", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.GoToHotspotOrCamp,
					PossibleParameters = new List<string> { "hotspot_004_battleground" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnteredBattle,
					PossibleParameters = new List<string> { "battle_003" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.AttackPig,
					PossibleParameters = new List<string> { "bird_red" },
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.AttackPig,
					PossibleParameters = new List<string> { "bird_red" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SupportSelf,
					PossibleParameters = new List<string> { "bird_red" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.AttackPig,
					PossibleParameters = new List<string> { "bird_red" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.UseRage,
					PossibleParameters = new List<string> { "bird_red" }
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_save_yellow" }
		});
	}

	private void AddCraftSecondItemTutorial()
	{
		m_TutorialTemplates.Add("tutorial_craft_second_item", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnterCamp,
					PossibleParameters = new List<string> { "story_forge" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ClickCampProp,
					PossibleParameters = new List<string> { "story_forge" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SelectCategory,
					PossibleParameters = new List<string> { InventoryItemType.OffHandEquipment.ToString() }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SelectRecipe,
					PossibleParameters = new List<string> { "recipe_offhand_yellow_orb_01" },
					StepBackWidth = 1
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.CraftItem,
					PossibleParameters = new List<string> { "recipe_offhand_yellow_orb_01" },
					AutoStartStep = true,
					StepBackWidth = 2
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_craft_reroll" }
		});
	}

	private void AddCraftRerollTutorial()
	{
		m_TutorialTemplates.Add("tutorial_craft_reroll", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.RerollCrafting,
					PossibleParameters = new List<string>()
				}
			},
			NonPersistant = true
		});
	}

	private void AddUseConsumableTutorial()
	{
		m_TutorialTemplates.Add("tutorial_use_consumable", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnterConsumableBar,
					PossibleParameters = new List<string> { "potion_healing_00" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.UseConsumable,
					PossibleParameters = new List<string> { "potion_healing_new_00" },
					StepBackWidth = 1
				}
			}
		});
	}

	private void AddCraftHealingPotionTutorial()
	{
		m_TutorialTemplates.Add("tutorial_craft_healing_potion", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnterCamp,
					PossibleParameters = new List<string> { "story_cauldron" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ClickCampProp,
					PossibleParameters = new List<string> { "story_cauldron" },
					StepBackWidth = 1
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SelectRecipe,
					PossibleParameters = new List<string> { "recipe_potion_healing_00" },
					StepBackWidth = 1
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.CraftPotion,
					PossibleParameters = new List<string> { "recipe_potion_healing_00" },
					StepBackWidth = 2,
					AutoStartStep = true
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_use_consumable", "tutorial_buy_class" }
		});
	}

	private void AddExitShopTutorial()
	{
		m_TutorialTemplates.Add("tutorial_exit_shop", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.LeaveShop,
					PossibleParameters = new List<string>(),
					AutoStartStep = true
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_craft_healing_potion" },
			NextTutorialStartStep = 1,
			ForceStartNextTutorials = true,
			NonPersistant = true
		});
	}

	private void AddBuyCauldronTutorial()
	{
		m_TutorialTemplates.Add("tutorial_buy_cauldron", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnterCamp,
					PossibleParameters = new List<string> { "story_shop" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ClickCampProp,
					PossibleParameters = new List<string> { "story_shop" },
					StepBackWidth = 1
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnterShopCategory,
					PossibleParameters = new List<string> { "shop_global_specials" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.BuyShopOffer,
					PossibleParameters = new List<string> { "offer_buy_cauldron" },
					StepBackWidth = 1
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_exit_shop", "tutorial_craft_healing_potion", "tutorial_buy_class" }
		});
	}

	private void AddRerollBattleLootTutorial()
	{
		m_TutorialTemplates.Add("tutorial_reroll_battle", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.RerollBattle,
					PossibleParameters = new List<string>()
				}
			},
			NonPersistant = true
		});
	}

	private void AddBattleRuleTooltipTutorial()
	{
		m_TutorialTemplates.Add("tutorial_battle_rule", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.BattleRule,
					PossibleParameters = new List<string>()
				}
			}
		});
	}

	private void AddEquipCraftedItemTutorial()
	{
		m_TutorialTemplates.Add("tutorial_equip_crafted_item", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EquipItemFromCraftingResult,
					PossibleParameters = new List<string>()
				}
			},
			FollowUpTutorials = new List<string>(),
			NonPersistant = true
		});
	}

	private void AddCraftFirstItemTutorial()
	{
		m_TutorialTemplates.Add("tutorial_craft_first_item", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnterCamp,
					PossibleParameters = new List<string> { "story_forge" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ClickCampProp,
					PossibleParameters = new List<string> { "story_forge" },
					StepBackWidth = 1
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SelectRecipe,
					PossibleParameters = new List<string> { "recipe_weapon_red_lance_01" },
					StepBackWidth = 1
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.CraftItem,
					PossibleParameters = new List<string> { "recipe_weapon_red_lance_01" },
					StepBackWidth = 2,
					AutoStartStep = true
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_equip_crafted_item", "tutorial_harvest" }
		});
	}

	private void AddHarvestTutorial()
	{
		m_TutorialTemplates.Add("tutorial_harvest", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.Harvest,
					PossibleParameters = new List<string> { "hotspot_011_01_resourcenode" }
				}
			}
		});
	}

	private void AddGachaTooltipTutorial()
	{
		m_TutorialTemplates.Add("tutorial_tooltip_gacha", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.LeaveGachaResult,
					PossibleParameters = new List<string>()
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ReadGachaTooltip,
					PossibleParameters = new List<string>()
				}
			},
			NonPersistant = true
		});
	}

	private void AddUseGachaTutorial()
	{
		m_TutorialTemplates.Add("tutorial_use_gacha", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnterCamp,
					PossibleParameters = new List<string> { "story_goldenpig" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ClickCampProp,
					PossibleParameters = new List<string> { "story_goldenpig" },
					StepBackWidth = 1
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.UseGacha,
					PossibleParameters = new List<string>(),
					StepBackWidth = 1
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_tooltip_gacha" }
		});
	}

	private void AddIntroBattleTutorial()
	{
		m_TutorialTemplates.Add("tutorial_start_intro_battle", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnteredBattle,
					PossibleParameters = new List<string> { "battle_000" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.AttackPig,
					PossibleParameters = new List<string> { "bird_red_cinematic", "pig_caster_pyropig_intro" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.AttackPig,
					PossibleParameters = new List<string> { "bird_yellow_cinematic", "pig_caster_pyropig_intro" },
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.AttackPig,
					PossibleParameters = new List<string> { "bird_red_cinematic", "pig_caster_pyropig_intro" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.AttackPig,
					PossibleParameters = new List<string> { "bird_yellow_cinematic", "pig_caster_pyropig_intro" },
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.UseRage,
					PossibleParameters = new List<string> { "bird_red_cinematic" }
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_start_first_battle" }
		});
	}

	private void AddStartFirstBattleTutorial()
	{
		m_TutorialTemplates.Add("tutorial_start_first_battle", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.GoToHotspotOrCamp,
					PossibleParameters = new List<string> { "hotspot_002_battleground" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnteredBattle,
					PossibleParameters = new List<string> { "battle_001" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ClickWheel,
					PossibleParameters = new List<string>()
				}
			},
			FollowUpTutorials = new List<string> { "tutorial_defensive_ability" }
		});
	}

	private void AddReadPvPDailyObjectivesTutorial()
	{
		m_TutorialTemplates.Add("tutorial_pvp_first_fight", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.PvPClickFightButton,
					PossibleParameters = new List<string>(),
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.PvPStartBattle,
					PossibleParameters = new List<string>()
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.DefeatPvPOpponent,
					PossibleParameters = new List<string> { "2", "pvp_pvptut_bird_yellow" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.DefeatPvPOpponent,
					PossibleParameters = new List<string> { "1", "pvp_pvptut_bird_yellow" },
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.DefeatPvPOpponent,
					PossibleParameters = new List<string> { "0", "pvp_pvptut_bird_yellow" },
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.DefeatPvPOpponent,
					PossibleParameters = new List<string> { "2", "bird_banner_pvptut" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.DefeatPvPOpponent,
					PossibleParameters = new List<string> { "1", "bird_banner_pvptut" },
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.DefeatPvPOpponent,
					PossibleParameters = new List<string> { "0", "bird_banner_pvptut" },
					AutoStartStep = true
				}
			}
		});
	}

	private void AddEnchantmentTutorial()
	{
		m_TutorialTemplates.Add("tutorial_enchantment", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnterCamp,
					PossibleParameters = new List<string> { "story_enchantment" },
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ClickCampBird,
					PossibleParameters = new List<string> { "bird_red", "bird_white" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SelectBirdManagerCategory,
					PossibleParameters = new List<string> { InventoryItemType.MainHandEquipment.ToString() },
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnterEnchantment,
					PossibleParameters = new List<string>(),
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SelectEnchantmentResource,
					PossibleParameters = new List<string> { "resource_flotsam", "recipe_flotsam" }
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.AddResources,
					PossibleParameters = new List<string> { "recipe_flotsam", "25" },
					StepBackWidth = 1,
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnchantFirstItem,
					PossibleParameters = new List<string>(),
					AutoStartStep = true
				}
			}
		});
	}

	private void AddEquipSkinTutorial()
	{
		m_TutorialTemplates.Add("tutorial_equip_skin", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SkinEntryPoint,
					PossibleParameters = new List<string>()
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnterCamp,
					PossibleParameters = new List<string>(),
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ClickCampBirdWithSkin,
					PossibleParameters = new List<string>()
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SelectClassWithSkins,
					PossibleParameters = new List<string>()
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SwapSkin,
					PossibleParameters = new List<string>(),
					AutoStartStep = true,
					StepBackWidth = 2
				}
			}
		});
	}

	private void AddSkinShoppedTutorial()
	{
		m_TutorialTemplates.Add("tutorial_skin_shopped", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.LeaveShopOrSkip,
					PossibleParameters = new List<string>(),
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ClickCampBirdWithSkin,
					PossibleParameters = new List<string>(),
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SelectClassWithSkins,
					PossibleParameters = new List<string>()
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SwapSkin,
					PossibleParameters = new List<string>(),
					AutoStartStep = true,
					StepBackWidth = 2
				}
			}
		});
	}

	private void AddSkinShoppedWorldmapTutorial()
	{
		m_TutorialTemplates.Add("tutorial_skin_shopped_worldmap", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.LeaveShop,
					PossibleParameters = new List<string>(),
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.EnterCamp,
					PossibleParameters = new List<string>(),
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.ClickCampBirdWithSkin,
					PossibleParameters = new List<string>(),
					AutoStartStep = true
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SelectClassWithSkins,
					PossibleParameters = new List<string>()
				},
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.SwapSkin,
					PossibleParameters = new List<string>(),
					AutoStartStep = true,
					StepBackWidth = 2
				}
			}
		});
	}

	private void AddPointOutNextHotspotTutorial()
	{
		m_TutorialTemplates.Add("point_out_next_hotspot", new TutorialTemplate
		{
			StepTemplates = new List<TutorialStepTemplate>
			{
				new TutorialStepTemplate
				{
					StepTemplate = TutorialStepType.PointOutNextHotspot,
					PossibleParameters = new List<string>()
				}
			},
			NonPersistant = true
		});
	}

	public void FinishWholeTutorial()
	{
		foreach (KeyValuePair<string, TutorialTemplate> tutorialTemplate in m_TutorialTemplates)
		{
			FinishActiveTutorials(tutorialTemplate.Key);
		}
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
	}

	public void FinishActiveTutorials(string ident)
	{
		TutorialTemplate value = null;
		if (m_TutorialTemplates.TryGetValue(ident, out value))
		{
			if (!value.NonPersistant)
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[ident] = 1;
			}
			DebugLog.Log("Tutorial Completed: " + ident);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}
	}

	public void FinishTutorialStep(string ident)
	{
		Tutorial value = null;
		DebugLog.Log("Try Finish Tutorial Step from Tutorial: " + ident);
		if (!m_TutorialProgress.TryGetValue(ident, out value))
		{
			return;
		}
		DebugLog.Log("Finished Tutorial Step: " + (value.TutorialProgress + 1) + " from Tutorial: " + ident);
		value.TutorialProgress++;
		if (value.IsFinished)
		{
			FinishTutorial(ident);
			return;
		}
		if (value.TutorialSteps[value.TutorialProgress].Step.IsAutoStarted)
		{
			value.TutorialSteps[value.TutorialProgress].Step.EvaluateStep("triggered_forced", new List<string>());
		}
		if (value.TutorialSteps.Count > value.TutorialProgress)
		{
			DebugLog.Log("Next Tutorial Step Type: " + value.TutorialSteps[value.TutorialProgress].Step.GetType().ToString());
		}
	}

	public void FinishTutorial(string ident)
	{
		TutorialTemplate value = null;
		if (!m_TutorialTemplates.TryGetValue(ident, out value))
		{
			return;
		}
		if (!value.NonPersistant)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[ident] = 1;
		}
		foreach (string followUpTutorial in value.FollowUpTutorials)
		{
			StartTutorial(followUpTutorial, value.NextTutorialStartStep);
			if (value.ForceStartNextTutorials)
			{
				Tutorial value2 = null;
				if (m_TutorialProgress.TryGetValue(followUpTutorial, out value2))
				{
					value2.TutorialSteps[value2.TutorialProgress].Step.EvaluateStep("triggered_forced", new List<string>());
				}
			}
		}
		int num = 0;
		foreach (int value3 in DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Values)
		{
			if (value3 == 1)
			{
				num++;
			}
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("TutorialName", ident);
		dictionary.Add("TutorialStepsCompleted", num.ToString("0"));
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("TutorialCompleted", dictionary);
		DebugLog.Log("Tutorial Completed: " + ident);
		DebugLog.Log("Tutorialsteps Completed: " + num);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		if (ident == "tutorial_use_gacha")
		{
			DIContainerInfrastructure.GetAttributionService().TrackEvent(AdjustTrackingEvent.TutorialPassed);
		}
	}

	public void ShowTutorialGuideIfNecessary(string trigger, string additionalParam = "")
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks == null)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks = new Dictionary<string, int>();
		}
		List<Tutorial> list = m_TutorialProgress.Values.Where((Tutorial t) => !t.IsFinished).ToList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			Tutorial tutorial = list[num];
			tutorial.TutorialSteps[tutorial.TutorialProgress].Step.EvaluateStep(trigger, new List<string> { additionalParam });
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

	public void StartTutorial(string ident, int startStep = 0)
	{
		TutorialTemplate value = null;
		if (!m_TutorialTemplates.TryGetValue(ident, out value))
		{
			return;
		}
		if (!value.NonPersistant)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.ContainsKey(ident))
			{
				DebugLog.Log("StartTutorial: " + ident);
				DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks.Add(ident, 0);
			}
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.TutorialTracks[ident] == 1)
			{
				return;
			}
		}
		Tutorial value2 = null;
		if (m_TutorialProgress.TryGetValue(ident, out value2))
		{
			value2.TutorialProgress = startStep;
		}
		else
		{
			value2 = GenerateTutorialFromTemplate(ident, value);
			m_TutorialProgress.Add(ident, value2);
			value2.TutorialProgress = startStep;
		}
		if (value2.TutorialSteps[value2.TutorialProgress].Step.IsAutoStarted)
		{
			value2.TutorialSteps[value2.TutorialProgress].Step.EvaluateStep("triggered_forced", new List<string>());
		}
	}

	private Tutorial GenerateTutorialFromTemplate(string ident, TutorialTemplate tutorialTemplate)
	{
		Tutorial tutorial = new Tutorial();
		for (int i = 0; i < tutorialTemplate.StepTemplates.Count; i++)
		{
			TutorialStepState tutorialStepState = new TutorialStepState();
			tutorialStepState.FinishType = TutorialStepFinishType.Active;
			tutorialStepState.Step = Activator.CreateInstance(Type.GetType(string.Concat("ABH.Tutorial.Steps.", tutorialTemplate.StepTemplates[i].StepTemplate, "TutorialStep"), true, true)) as ITutorialStep;
			TutorialStepState tutorialStepState2 = tutorialStepState;
			tutorialStepState2.Step.SetupStep(string.Empty, ident, tutorialTemplate.StepTemplates[i].PossibleParameters, tutorialTemplate.StepTemplates[i].AutoStartStep);
			tutorial.TutorialSteps.Add(tutorialStepState2);
		}
		return tutorial;
	}

	public void StepBackOneTutorialStep(string ident)
	{
		Tutorial value = null;
		if (m_TutorialProgress.TryGetValue(ident, out value))
		{
			int num = value.TutorialProgress - m_TutorialTemplates[ident].StepTemplates[value.TutorialProgress].StepBackWidth;
			DebugLog.Log("Try to step back in Tutorial:" + ident + " to Step: " + num);
			value.TutorialProgress = Mathf.Max(0, num);
			value.TutorialSteps[value.TutorialProgress].Step.EvaluateStep("triggered_forced", new List<string>());
		}
	}

	public void SkipToTutorialStep(string ident, int stepIndex, bool forceTrigger = false)
	{
		try
		{
			Tutorial value = null;
			if (m_TutorialProgress.TryGetValue(ident, out value))
			{
				value.TutorialProgress = stepIndex;
				if (forceTrigger || value.TutorialSteps[value.TutorialProgress].Step.IsAutoStarted)
				{
					value.TutorialSteps[value.TutorialProgress].Step
						.EvaluateStep("triggered_forced", new List<string>());
				}
			}
			else
			{
				DebugLog.Error(GetType(), "SkipToTutorialStep: Tutorial " + ident + " not found!");
			}
		}
		catch (Exception err)
		{
			// some softlock arises here idk so just catch it
		}
	}

	public void ResetTutorial(string ident)
	{
		Tutorial value = null;
		if (m_TutorialProgress.TryGetValue(ident, out value))
		{
			value.TutorialProgress = 0;
		}
	}

	public void Remove()
	{
		DebugLog.Warn("Tutorial Mgr Destroyed, it is only allowed on Reset!");
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
