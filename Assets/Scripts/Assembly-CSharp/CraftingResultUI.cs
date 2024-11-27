using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using SmoothMoves;
using UnityEngine;

public class CraftingResultUI : MonoBehaviour
{
	[SerializeField]
	private GameObject m_CraftingRoot;

	[SerializeField]
	private GameObject m_ResultRoot;

	[SerializeField]
	private GameObject m_OptionsRoot;

	[SerializeField]
	private List<GameObject> m_DiceRoots;

	[SerializeField]
	private List<DiceOrientationController> m_DiceControllers;

	[SerializeField]
	private GameObject m_DiceDestroyFX;

	[SerializeField]
	private Transform m_FXRoot;

	[SerializeField]
	private Animation m_CraftingAnimation;

	[SerializeField]
	private Animation m_ResultAnimation;

	[SerializeField]
	private Animation m_OptionsAnimation;

	[SerializeField]
	private Animation[] m_StarAnimations;

	[SerializeField]
	private UISprite[] m_StarBodySprites;

	[SerializeField]
	private ParticleSystem[] m_StarParticles;

	[SerializeField]
	private EquipmentStatsSpeechBubble m_BubbleComparison;

	[SerializeField]
	private StatisticsElement m_StatsWithChangeIndicator;

	[SerializeField]
	private StatisticsElement m_StatsWithComparsionIndicator;

	[SerializeField]
	private UISprite m_PerkSprite;

	[SerializeField]
	private ResourceCostBlind m_CostBlind;

	[SerializeField]
	private LootDisplayContoller m_ItemDisplay;

	[SerializeField]
	private List<LootDisplayContoller> m_ScrapLootDisplays = new List<LootDisplayContoller>();

	[SerializeField]
	private Animation m_ScrapInfoAnmiation;

	[SerializeField]
	private UISprite m_SlicedBubble;

	[SerializeField]
	private UILabel m_ItemName;

	[SerializeField]
	private GameObject m_ItemStatsRoot;

	[SerializeField]
	private GameObject m_CraftedItemRoot;

	[SerializeField]
	private CharacterControllerCamp m_CampViewController;

	[SerializeField]
	private GameObject m_StatComparisionPrefab;

	[SerializeField]
	public UIInputTrigger m_AcceptButton;

	[SerializeField]
	public UIInputTrigger m_RerollButton;

	[SerializeField]
	public UIInputTrigger m_EquipButton;

	[SerializeField]
	public UIInputTrigger m_RepeatButton;

	[SerializeField]
	public UILabel m_RepeatAmountLabel;

	[SerializeField]
	private UISprite m_RepeatButtonSprite;

	[SerializeField]
	private GameObject m_CharacterRoot;

	[SerializeField]
	private SoundTriggerList m_SoundTriggers;

	[SerializeField]
	private GameObject m_SkipCollider;

	[SerializeField]
	private UIInputTrigger m_SkipTrigger;

	private GameObject m_EquipmentSprite;

	private bool m_equipableItem;

	private List<bool> m_starList = new List<bool>();

	private IInventoryItemGameData m_item;

	private CraftingRecipeGameData m_recipe;

	private BirdGameData m_PossibleBird;

	private BoneAnimation m_birdAnimation;

	private CharacterControllerCamp m_equipBird;

	private bool m_betterItem;

	private int m_delta;

	private bool m_EquipPressed;

	private List<LootDisplayContoller> m_ExplodedLoot = new List<LootDisplayContoller>();

	private bool m_ScrapInfoShown;

	private float m_SlicedBubbleBaseSize;

	private CampStateMgr m_CampStateMgr;

	private bool m_IsAlchemy;

	private int m_ForgeLevel;

	private bool m_AcceptPressed;

	private int m_amount = 1;

	private bool m_enterBackground;

	private bool m_diceAnimationPlayed;

	[method: MethodImpl(32)]
	public event Action<BirdGameData> EquipBirdClicked;

	[method: MethodImpl(32)]
	public event Action ConfirmedCraftingClicked;

	private void Awake()
	{
		if (m_SlicedBubble != null)
		{
			m_SlicedBubbleBaseSize = m_SlicedBubble.cachedTransform.localScale.x;
		}
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		if (m_betterItem)
		{
			OnEquipButtonClicked();
		}
		else
		{
			OnAcceptButtonClicked();
		}
	}

	public void ShowPerkTooltip()
	{
		EquipmentGameData equipmentGameData = m_item as EquipmentGameData;
		if (equipmentGameData != null && (bool)m_PerkSprite)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowPerkOverlay(m_PerkSprite.cachedTransform, equipmentGameData, true);
		}
	}

	public void SetStateMgr(CampStateMgr stateMgr)
	{
		m_CampStateMgr = stateMgr;
	}

	public void Enter(bool enterBackground = true, int amount = 1)
	{
		m_EquipPressed = false;
		m_AcceptPressed = false;
		m_ScrapInfoShown = false;
		base.gameObject.SetActive(true);
		m_amount = amount;
		m_RepeatAmountLabel.text = "x" + m_amount;
		m_RepeatAmountLabel.gameObject.SetActive(m_amount > 1);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		IInventoryItemGameData data;
		if (m_IsAlchemy)
		{
			DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "cauldron_leveled", out data);
			m_RepeatButtonSprite.spriteName = "Craft_Alchemy";
		}
		else
		{
			DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "forge_leveled", out data);
			m_RepeatButtonSprite.spriteName = "Craft_Forging";
		}
		m_ForgeLevel = data.ItemData.Level;
		m_RerollButton.gameObject.SetActive(true);
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.transform.position.z);
		m_enterBackground = enterBackground;
		StartCoroutine("EnterCoroutine");
		if (m_amount == 1)
		{
			Requirement rerollRequirement = DIContainerLogic.CraftingService.GetRerollRequirement();
			m_CostBlind.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(rerollRequirement.NameId).AssetBaseId, null, rerollRequirement.Value, string.Empty);
		}
		else
		{
			Requirement multiRerollRequirement = DIContainerLogic.CraftingService.GetMultiRerollRequirement();
			m_CostBlind.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(multiRerollRequirement.NameId).AssetBaseId, null, multiRerollRequirement.Value, string.Empty);
		}
	}

	public void Leave()
	{
		DIContainerInfrastructure.GetCoreStateMgr().LeaveShop();
		StartCoroutine(LeaveCoroutine(true));
	}

	private IEnumerator LeaveCoroutine(bool disable, bool leaveBackground = true)
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("crafting_result_leave");
		DeregisterEventHandler();
		StopCoroutine("AnimateBird");
		StartCoroutine(HideComparisonBubble());
		if (leaveBackground)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Leave();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 2u
			}, true);
		}
		if (m_ExplodedLoot.Count > 0)
		{
			foreach (LootDisplayContoller item in m_ExplodedLoot)
			{
				item.HideThenDestroy();
			}
			m_ExplodedLoot.Clear();
		}
		else
		{
			m_ItemDisplay.PlayHideAnimation();
		}
		HideScrapBubble();
		m_ResultAnimation.Play("Crafting_Result_Leave");
		m_OptionsAnimation.Play("Crafting_Options_Leave");
		Animation[] starAnimations = m_StarAnimations;
		foreach (Animation sanim in starAnimations)
		{
			sanim.Play("ValueStar_Disappear");
		}
		if (leaveBackground)
		{
			m_CampStateMgr.m_ForgeWindow.Refresh();
			m_CampStateMgr.RefreshBirdMarkers();
		}
		yield return new WaitForSeconds(m_ResultAnimation["Crafting_Result_Leave"].length);
		RemoveEquipmentSprite(m_item as EquipmentGameData);
		if (disable)
		{
			base.gameObject.SetActive(false);
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("crafting_result_leave");
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("crafting_result_leave", string.Empty);
	}

	private void HideScrapBubble()
	{
		if (m_ScrapInfoShown)
		{
			m_ScrapInfoShown = false;
			m_ScrapInfoAnmiation.Play("ScrapInfo_Hide");
		}
	}

	private IEnumerator HideComparisonBubble()
	{
		if (m_BubbleComparison.gameObject.activeInHierarchy)
		{
			yield return new WaitForSeconds(m_BubbleComparison.Hide());
			m_BubbleComparison.gameObject.SetActive(false);
		}
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, HandleBackButton);
		m_AcceptButton.Clicked += OnAcceptButtonClicked;
		m_RerollButton.Clicked += OnRerollButtonClicked;
		m_EquipButton.Clicked += OnEquipButtonClicked;
		m_RepeatButton.Clicked += OnRepeatClicked;
		m_SkipTrigger.Clicked += OnSkipClicked;
	}

	private void DeregisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
		m_AcceptButton.Clicked -= OnAcceptButtonClicked;
		m_RerollButton.Clicked -= OnRerollButtonClicked;
		m_EquipButton.Clicked -= OnEquipButtonClicked;
		m_RepeatButton.Clicked -= OnRepeatClicked;
		m_SkipTrigger.Clicked -= OnSkipClicked;
	}

	public void OnSkipClicked()
	{
		StopCoroutine("EnterCoroutine");
		m_SkipCollider.SetActive(false);
		for (int i = 0; i < m_DiceRoots.Count; i++)
		{
			m_DiceRoots[i].SetActive(false);
		}
		m_ResultRoot.SetActive(true);
		int num = 0;
		if (!m_diceAnimationPlayed)
		{
			for (int j = 0; j < 3; j++)
			{
				m_StarBodySprites[j].spriteName = m_StarBodySprites[j].spriteName.Replace("_Desaturated", string.Empty);
				string animation = "ValueStar_GainedWithBonus";
				if (!m_starList[j])
				{
					m_StarBodySprites[j].spriteName = m_StarBodySprites[j].spriteName + "_Desaturated";
					animation = "ValueStar_GainedNormal";
				}
				else
				{
					num++;
				}
				m_StarAnimations[j].Play(animation);
			}
		}
		m_ItemStatsRoot.SetActive(m_equipableItem);
		m_CraftedItemRoot.SetActive(true);
		List<IInventoryItemGameData> items = new List<IInventoryItemGameData>();
		m_ItemDisplay.gameObject.SetActive(true);
		m_ItemDisplay.SetModel(m_item, items, LootDisplayType.Minor);
		if (m_equipableItem)
		{
			m_PerkSprite.spriteName = EquipmentGameData.GetPerkIcon(m_item as EquipmentGameData);
			if (m_item.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment)
			{
				m_StatsWithChangeIndicator.SetIconSprite("Character_Damage_Large");
				m_StatsWithComparsionIndicator.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, 0), m_PossibleBird.MainHandItem.ItemMainStat);
				m_StatsWithComparsionIndicator.SetIconSprite("Character_Damage_Large");
			}
			else if (m_item.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment)
			{
				m_StatsWithChangeIndicator.SetIconSprite("Character_Health_Large");
				m_StatsWithComparsionIndicator.SetIconSprite("Character_Damage_Large");
				m_StatsWithComparsionIndicator.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, 0), m_PossibleBird.OffHandItem.ItemMainStat);
			}
		}
		m_StatsWithChangeIndicator.RefreshStat(false, false, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, 0), 0f);
		m_ResultAnimation.Play("Crafting_Result_Enter");
		m_ItemDisplay.transform.localScale = Vector3.one;
		m_ItemDisplay.OverrideAmount(1 * m_amount);
		m_ItemDisplay.PlayGainedAnimation();
		int num2 = 0;
		for (int k = 0; k < m_starList.Count; k++)
		{
			if (!m_starList[k])
			{
				continue;
			}
			m_StarAnimations[k].Play("ValueStar_BonusUsed");
			num2++;
			if (m_equipableItem)
			{
				m_StatsWithChangeIndicator.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, num2), EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, num2 - 1));
				if (m_item.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment)
				{
					m_StatsWithComparsionIndicator.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, num2), m_PossibleBird.MainHandItem.ItemMainStat);
				}
				else
				{
					m_StatsWithComparsionIndicator.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, num2), m_PossibleBird.OffHandItem.ItemMainStat);
				}
			}
			else
			{
				m_ItemDisplay.OverrideAmount(DIContainerLogic.CraftingService.GetAmountByQuality(k + 1) * m_amount);
			}
		}
		m_OptionsRoot.SetActive(true);
		m_OptionsAnimation.Play("Crafting_Options_Enter");
		if (m_equipableItem)
		{
			m_equipBird.gameObject.SetActive(true);
			SetLayerRecusively(m_equipBird.gameObject, 8);
			StartCoroutine("AnimateBird");
			if (m_PossibleBird != null)
			{
				if (m_PossibleBird.MainHandItem.ItemMainStat >= m_item.ItemMainStat)
				{
					m_EquipButton.gameObject.SetActive(false);
					m_AcceptButton.transform.position = m_EquipButton.transform.position;
				}
				else
				{
					m_EquipButton.gameObject.SetActive(true);
					m_AcceptButton.transform.localPosition = Vector3.zero;
				}
			}
			else
			{
				m_EquipButton.gameObject.SetActive(false);
				m_AcceptButton.transform.position = m_EquipButton.transform.position;
			}
		}
		else
		{
			m_EquipButton.gameObject.SetActive(false);
			m_AcceptButton.transform.position = m_EquipButton.transform.position;
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u,
			showFriendshipEssence = true
		}, true);
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("crafting_finished", string.Empty);
		if (m_equipableItem)
		{
			SpawnComparisonBubble();
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("crafting_result_enter");
	}

	public void OnRepeatClicked()
	{
		List<IInventoryItemGameData> failedItems = new List<IInventoryItemGameData>();
		if (!DIContainerLogic.CraftingService.IsCraftAble(m_recipe, DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_amount) || !DIContainerLogic.CraftingService.IsCraftingPossible(m_recipe, DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, out failedItems, m_amount))
		{
			if (failedItems.Count > 0)
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_MissingResourcesPopup.ShowMissingResourcesPopup(failedItems);
			}
			DebugLog.Log("not craftable");
		}
		else
		{
			List<IInventoryItemGameData> list = DIContainerLogic.CraftingService.CraftItem(m_recipe, DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_IsAlchemy, m_amount);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			List<KeyValuePair<string, int>> craftingCosts = new List<KeyValuePair<string, int>>();
			DIContainerLogic.CraftingService.AddCraftingCostsForItem(ref craftingCosts, list[0].ItemBalancing.NameId, list[0].ItemData.Level, m_amount);
			DeregisterEventHandler();
			StartCoroutine(ReEnterCraftingPopup());
		}
	}

	public void OnAcceptButtonClicked()
	{
		if (!m_AcceptPressed)
		{
			m_AcceptPressed = true;
			Leave();
			if (this.ConfirmedCraftingClicked != null)
			{
				this.ConfirmedCraftingClicked();
			}
		}
	}

	public void OnRerollButtonClicked()
	{
		if (DIContainerInfrastructure.GetCoreStateMgr().IsShopLoading())
		{
			return;
		}
		if (DIContainerLogic.CraftingService.IsRerollPossible(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData) && DIContainerLogic.CraftingService.ExecuteRerollCost(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_amount))
		{
			DeregisterEventHandler();
			DIContainerLogic.CraftingService.RerollItemQuality(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, ref m_item, m_amount);
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateFriendshipEssenceBar();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 2u,
				showFriendshipEssence = true
			}, true);
			StartCoroutine(ReEnterCraftingPopup());
			return;
		}
		Requirement rerollCraftingReqirement = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RerollCraftingReqirement;
		if (rerollCraftingReqirement == null || rerollCraftingReqirement.RequirementType != RequirementType.PayItem)
		{
			return;
		}
		IInventoryItemGameData data = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, rerollCraftingReqirement.NameId, out data))
		{
			if (data.ItemBalancing.NameId == "lucky_coin")
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[1].m_StatBar.SwitchToShop();
			}
			else if (data.ItemBalancing.NameId == "gold")
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[0].m_StatBar.SwitchToShop();
			}
			else if (data.ItemBalancing.NameId == "friendship_essence")
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[2].m_StatBar.SwitchToShop();
			}
		}
	}

	private IEnumerator ReEnterCraftingPopup()
	{
		yield return StartCoroutine(LeaveCoroutine(false, false));
		SetItem(m_item, m_recipe);
		Enter(false, m_amount);
	}

	public void OnScrapButtonClicked()
	{
		if (m_item != null)
		{
			if ((bool)m_SoundTriggers)
			{
				m_SoundTriggers.OnTriggerEventFired("item_scrapped");
			}
			m_ItemDisplay.SetModel(m_item, DIContainerLogic.CraftingService.ScrapEquipment(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_item as EquipmentGameData), LootDisplayType.Minor);
			m_ExplodedLoot = m_ItemDisplay.Explode(false, false, 0f, true, 0f, 0f);
			StopCoroutine("AnimateBird");
			m_equipBird.PlayCheerCharacter();
			m_ItemStatsRoot.SetActive(false);
			StartCoroutine(HideComparisonBubble());
			HideScrapBubble();
			m_RerollButton.gameObject.SetActive(false);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}
	}

	public void OnEquipButtonClicked()
	{
		if (!m_EquipPressed)
		{
			m_EquipPressed = true;
			DebugLog.Log("Equip Button pressed");
			m_item.ItemData.IsNew = false;
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			list.Add(m_item);
			List<IInventoryItemGameData> newContent = list;
			DIContainerLogic.InventoryService.EquipBirdWithItem(newContent, m_item.ItemBalancing.ItemType, m_PossibleBird.InventoryGameData);
			m_equipBird.SetModel(m_PossibleBird, false);
			StopCoroutine("AnimateBird");
			switch (m_item.ItemBalancing.ItemType)
			{
			case InventoryItemType.MainHandEquipment:
				m_equipBird.m_AssetController.PlayFocusWeaponAnimation();
				break;
			case InventoryItemType.OffHandEquipment:
				m_equipBird.m_AssetController.PlayFocusOffHandAnimation();
				break;
			default:
				m_equipBird.PlayCheerCharacter();
				break;
			}
			m_RerollButton.gameObject.SetActive(false);
			HideScrapBubble();
			StartCoroutine(HideComparisonBubble());
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			if (this.EquipBirdClicked != null)
			{
				this.EquipBirdClicked(m_PossibleBird);
			}
			Invoke("Leave", 1f);
		}
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("crafting_result_enter");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u
		}, true);
		RegisterEventHandler();
		m_SkipCollider.SetActive(true);
		m_diceAnimationPlayed = false;
		if (m_equipableItem)
		{
			float newStat = m_item.ItemMainStat;
			float oldStat = ((m_item.ItemBalancing.ItemType != InventoryItemType.MainHandEquipment) ? m_PossibleBird.OffHandItem.ItemMainStat : m_PossibleBird.MainHandItem.ItemMainStat);
			m_betterItem = newStat > oldStat;
		}
		else if (m_equipBird != null)
		{
			m_equipBird.gameObject.SetActive(false);
		}
		if (!m_betterItem)
		{
			m_item.ItemData.IsNew = false;
		}
		m_ItemStatsRoot.SetActive(false);
		m_ResultRoot.SetActive(false);
		m_CraftedItemRoot.SetActive(false);
		if (m_enterBackground)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Enter();
		}
		m_DiceRoots[m_ForgeLevel - 1].SetActive(true);
		m_DiceControllers[m_ForgeLevel - 1].SetOrientationFromStars(m_item.ItemData.Quality);
		m_CraftingRoot.SetActive(true);
		m_CraftingAnimation.Play("Crafting_DiceRoll");
		yield return new WaitForSeconds(m_CraftingAnimation.GetComponent<Animation>()["Crafting_DiceRoll"].length);
		yield return new WaitForSeconds(DIContainerLogic.GetPacingBalancing().CraftingTimeForTillStarAppearance);
		GameObject destroyFX = UnityEngine.Object.Instantiate(m_DiceDestroyFX, m_FXRoot.transform.position, Quaternion.identity) as GameObject;
		destroyFX.transform.parent = m_FXRoot.transform;
		destroyFX.transform.position = m_FXRoot.transform.position;
		destroyFX.transform.localScale = Vector3.one;
		destroyFX.transform.localRotation = Quaternion.identity;
		destroyFX.GetComponent<Animation>().Play("ScrapItemFX");
		StartCoroutine(DestroyAfterDelay(destroyFX, destroyFX.GetComponent<Animation>()["ScrapItemFX"].length));
		for (int k = 0; k < m_DiceRoots.Count; k++)
		{
			m_DiceRoots[k].SetActive(false);
		}
		m_ResultRoot.SetActive(true);
		int stars = 0;
		for (int j = 0; j < 3; j++)
		{
			m_StarBodySprites[j].spriteName = m_StarBodySprites[j].spriteName.Replace("_Desaturated", string.Empty);
			string animString = "ValueStar_GainedWithBonus";
			if (!m_starList[j])
			{
				m_StarBodySprites[j].spriteName = m_StarBodySprites[j].spriteName + "_Desaturated";
				animString = "ValueStar_GainedNormal";
			}
			else
			{
				stars++;
			}
			m_StarAnimations[j].Play(animString);
		}
		if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("star_result_" + stars);
		}
		m_ItemStatsRoot.SetActive(m_equipableItem);
		m_CraftedItemRoot.SetActive(true);
		List<IInventoryItemGameData> scrapItems = new List<IInventoryItemGameData>();
		m_ItemDisplay.gameObject.SetActive(true);
		m_ItemDisplay.SetModel(m_item, scrapItems, LootDisplayType.Minor);
		if (m_equipableItem)
		{
			m_PerkSprite.spriteName = EquipmentGameData.GetPerkIcon(m_item as EquipmentGameData);
			if (m_item.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment)
			{
				m_StatsWithChangeIndicator.SetIconSprite("Character_Damage_Large");
				m_StatsWithComparsionIndicator.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, 0), m_PossibleBird.MainHandItem.ItemMainStat);
				m_StatsWithComparsionIndicator.SetIconSprite("Character_Damage_Large");
			}
			else if (m_item.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment)
			{
				m_StatsWithChangeIndicator.SetIconSprite("Character_Health_Large");
				m_StatsWithComparsionIndicator.SetIconSprite("Character_Damage_Large");
				m_StatsWithComparsionIndicator.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, 0), m_PossibleBird.OffHandItem.ItemMainStat);
			}
		}
		m_StatsWithChangeIndicator.RefreshStat(false, false, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, 0), 0f);
		m_ResultAnimation.Play("Crafting_Result_Enter");
		m_ItemDisplay.OverrideAmount(1 * m_amount);
		m_ItemDisplay.PlayGainedAnimation();
		int qualityUsed = 0;
		m_diceAnimationPlayed = true;
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < m_starList.Count; i++)
		{
			if (!m_starList[i])
			{
				continue;
			}
			m_StarAnimations[i].Play("ValueStar_BonusUsed");
			qualityUsed++;
			yield return new WaitForSeconds(0.5f);
			if (m_equipableItem)
			{
				m_StatsWithChangeIndicator.RefreshStat(true, true, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, qualityUsed), EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, qualityUsed - 1));
				if (m_item.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment)
				{
					m_StatsWithComparsionIndicator.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, qualityUsed), m_PossibleBird.MainHandItem.ItemMainStat);
				}
				else
				{
					m_StatsWithComparsionIndicator.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(m_item as EquipmentGameData, qualityUsed), m_PossibleBird.OffHandItem.ItemMainStat);
				}
			}
			else
			{
				m_ItemDisplay.OverrideAmount(DIContainerLogic.CraftingService.GetAmountByQuality(i + 1) * m_amount);
			}
			yield return new WaitForSeconds(0.5f);
		}
		yield return new WaitForSeconds(0.5f);
		m_OptionsRoot.SetActive(true);
		m_OptionsAnimation.Play("Crafting_Options_Enter");
		if (m_equipableItem)
		{
			m_equipBird.gameObject.SetActive(true);
			SetLayerRecusively(m_equipBird.gameObject, 8);
			StartCoroutine("AnimateBird");
			if (m_PossibleBird != null)
			{
				if (m_PossibleBird.MainHandItem.ItemMainStat >= m_item.ItemMainStat)
				{
					m_EquipButton.gameObject.SetActive(false);
					m_AcceptButton.transform.position = m_EquipButton.transform.position;
				}
				else
				{
					m_EquipButton.gameObject.SetActive(true);
					m_AcceptButton.transform.localPosition = Vector3.zero;
				}
			}
			else
			{
				m_EquipButton.gameObject.SetActive(false);
				m_AcceptButton.transform.position = m_EquipButton.transform.position;
			}
		}
		else
		{
			m_EquipButton.gameObject.SetActive(false);
			m_AcceptButton.transform.position = m_EquipButton.transform.position;
		}
		yield return new WaitForSeconds(m_OptionsAnimation["Crafting_Options_Enter"].length);
		m_SkipCollider.SetActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u,
			showFriendshipEssence = true
		}, true);
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("crafting_finished", string.Empty);
		if (m_equipableItem)
		{
			SpawnComparisonBubble();
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("crafting_result_enter");
	}

	private IEnumerator DestroyAfterDelay(GameObject go, float delay)
	{
		yield return new WaitForSeconds(delay);
		UnityEngine.Object.Destroy(go);
	}

	private void SpawnScrappingBubble()
	{
		m_ScrapInfoShown = true;
		float num = m_item.ItemMainStat - m_PossibleBird.MainHandItem.ItemMainStat;
		m_ScrapInfoAnmiation.Play("ScrapInfo_Show");
		if (num <= 0f)
		{
			m_ScrapInfoAnmiation.PlayQueued("ScrapInfo_Focus");
		}
		List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
		EquipmentGameData equipmentGameData = m_item as EquipmentGameData;
		if (equipmentGameData != null && equipmentGameData.GetScrapLoot() != null)
		{
			list = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(equipmentGameData.GetScrapLoot(), 0));
		}
		m_SlicedBubble.cachedTransform.localScale = new Vector3(m_SlicedBubbleBaseSize * ((float)list.Count / 3f), m_SlicedBubble.cachedTransform.localScale.y, m_SlicedBubble.cachedTransform.localScale.z);
		for (int i = 0; i < m_ScrapLootDisplays.Count; i++)
		{
			if (list.Count > i)
			{
				m_ScrapLootDisplays[i].gameObject.SetActive(true);
				m_ScrapLootDisplays[i].SetModel(list[i], new List<IInventoryItemGameData>(), LootDisplayType.None, "_Small");
			}
			else
			{
				m_ScrapLootDisplays[i].gameObject.SetActive(false);
			}
		}
	}

	private void SpawnComparisonBubble()
	{
		if (m_equipableItem)
		{
			EquipmentGameData equipmentGameData = m_item as EquipmentGameData;
			m_BubbleComparison.gameObject.SetActive(true);
			if (m_item.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment)
			{
				m_BubbleComparison.SetComparisionValues("Character_Damage_Large", m_item.ItemBalancing.ItemType, m_PossibleBird.MainHandItem.ItemMainStat, m_PossibleBird.MainHandItem.BalancingData.Perk.Type);
			}
			else
			{
				m_BubbleComparison.SetComparisionValues("Character_Health_Large", m_item.ItemBalancing.ItemType, m_PossibleBird.OffHandItem.ItemMainStat, m_PossibleBird.OffHandItem.BalancingData.Perk.Type);
			}
			m_equipBird.PositionComparisionBubble(m_equipBird, m_BubbleComparison.gameObject);
			m_BubbleComparison.Show();
		}
	}

	private void CreateEquipButtonBird(BirdGameData birdData)
	{
		CancelInvoke();
		if (m_equipBird != null)
		{
			m_equipBird.DestroyCharacter();
			m_birdAnimation = null;
		}
		m_equipBird = UnityEngine.Object.Instantiate(m_CampViewController, m_EquipButton.transform.position, m_EquipButton.transform.rotation) as CharacterControllerCamp;
		m_equipBird.transform.parent = m_CharacterRoot.transform;
		m_equipBird.transform.localPosition = Vector3.zero;
		m_equipBird.SetModel(birdData, false);
		m_equipBird.DisableTabAndHold();
		m_equipBird.gameObject.SetActive(false);
	}

	private void SetLayerRecusively(GameObject go, int layer)
	{
		go.layer = layer;
		foreach (Transform item in go.transform)
		{
			SetLayerRecusively(item.gameObject, layer);
		}
	}

	private IEnumerator AnimateBird()
	{
		if (m_birdAnimation == null)
		{
			m_birdAnimation = m_equipBird.m_AssetController.m_BoneAnimation;
		}
		if (m_birdAnimation == null)
		{
			yield break;
		}
		float minWaitTime = m_birdAnimation["Cheer"].length + m_birdAnimation["Idle"].length;
		while (true)
		{
			if (m_betterItem)
			{
				m_birdAnimation.CrossFade("Cheer");
			}
			m_birdAnimation.CrossFadeQueued("Idle");
			yield return new WaitForSeconds(UnityEngine.Random.Range(minWaitTime, minWaitTime * 2f));
		}
	}

	public void SetItem(IInventoryItemGameData newItem, CraftingRecipeGameData recipe)
	{
		m_item = newItem;
		m_recipe = recipe;
		if (m_item.ItemBalancing.ItemType == InventoryItemType.Ingredients || m_item.ItemBalancing.ItemType == InventoryItemType.Resources || m_item.ItemBalancing.ItemType == InventoryItemType.Consumable)
		{
			m_RepeatButton.gameObject.SetActive(true);
		}
		else
		{
			m_RepeatButton.gameObject.SetActive(false);
		}
		m_IsAlchemy = m_item.ItemBalancing.ItemType == InventoryItemType.Consumable || m_item.ItemBalancing.ItemType == InventoryItemType.Ingredients;
		m_equipableItem = m_item is EquipmentGameData;
		m_starList.Clear();
		for (int i = 0; i < 3; i++)
		{
			m_starList.Add(false);
		}
		for (int j = 0; j < m_item.ItemData.Quality; j++)
		{
			m_starList[j] = true;
		}
		if (m_equipableItem)
		{
			EquipmentGameData equipmentGameData = m_item as EquipmentGameData;
			m_PossibleBird = DIContainerInfrastructure.GetCurrentPlayer().GetBird(equipmentGameData.BalancingData.RestrictedBirdId);
			m_ItemName.text = equipmentGameData.ItemLocalizedName;
			CreateEquipButtonBird(m_PossibleBird);
		}
		else
		{
			m_ItemName.text = m_item.ItemLocalizedName;
		}
	}

	private void AddEquipmentSprite(EquipmentGameData equip)
	{
		m_CraftedItemRoot.transform.localScale = Vector3.one;
		switch (equip.BalancingData.ItemType)
		{
		case InventoryItemType.Class:
			m_EquipmentSprite = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(equip.ItemAssetName, m_CraftedItemRoot.transform, Vector3.zero, Quaternion.identity, false);
			break;
		case InventoryItemType.MainHandEquipment:
			m_EquipmentSprite = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(equip.ItemAssetName, m_CraftedItemRoot.transform, Vector3.zero, Quaternion.identity, false);
			break;
		case InventoryItemType.OffHandEquipment:
			m_EquipmentSprite = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(equip.ItemAssetName, m_CraftedItemRoot.transform, Vector3.zero, Quaternion.identity, false);
			break;
		}
		if ((bool)m_EquipmentSprite)
		{
			m_EquipmentSprite.transform.localScale = Vector3.one;
		}
	}

	private void RemoveEquipmentSprite(EquipmentGameData equip)
	{
		if (equip != null && (bool)m_EquipmentSprite)
		{
			switch (equip.BalancingData.ItemType)
			{
			case InventoryItemType.Class:
				DIContainerInfrastructure.GetClassAssetProvider().DestroyObject(equip.ItemAssetName, m_EquipmentSprite);
				break;
			case InventoryItemType.MainHandEquipment:
				DIContainerInfrastructure.GetEquipmentAssetProvider().DestroyObject(equip.ItemAssetName, m_EquipmentSprite);
				break;
			case InventoryItemType.OffHandEquipment:
				DIContainerInfrastructure.GetEquipmentAssetProvider().DestroyObject(equip.ItemAssetName, m_EquipmentSprite);
				break;
			case InventoryItemType.Resources:
			case InventoryItemType.Ingredients:
			case InventoryItemType.Consumable:
			case InventoryItemType.Premium:
			case InventoryItemType.Story:
			case InventoryItemType.PlayerToken:
			case InventoryItemType.Points:
			case InventoryItemType.PlayerStats:
			case InventoryItemType.CraftingRecipes:
				break;
			}
		}
	}

	private void SetRebuildButtonIcon()
	{
	}
}
