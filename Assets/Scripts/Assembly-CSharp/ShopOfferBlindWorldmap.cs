using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class ShopOfferBlindWorldmap : MonoBehaviour
{
	[SerializeField]
	private List<RequirementRoot> m_RequirementRoots = new List<RequirementRoot>();

	[SerializeField]
	private Transform m_IconRoot;

	[SerializeField]
	private GameObject m_SlotRoot;

	[SerializeField]
	private GameObject m_RankUpEffectPrefab;

	[SerializeField]
	private GameObject m_BuyIndicatorPrefab;

	[SerializeField]
	private GameObject m_RecipeScrollRoot;

	[SerializeField]
	private GameObject m_MasteryBadgeRoot;

	private GameObject m_IconInstanciated;

	[SerializeField]
	private UILabel m_BlindMainLabel;

	[SerializeField]
	private ResourceCostBlind m_CostBlind;

	[SerializeField]
	private ResourceCostBlind m_DiscountCostBlind;

	[SerializeField]
	private UILabel m_DiscountOldCost;

	[SerializeField]
	private UISprite m_AdditionalSprite;

	[SerializeField]
	private UISprite m_ItemIconSprite;

	[SerializeField]
	public UIInputTrigger m_BuyButtonTrigger;

	private BasicShopOfferBalancingData m_Model;

	private IInventoryItemGameData m_Item;

	[SerializeField]
	private StatisticsElement m_ItemStats;

	[SerializeField]
	private UISprite m_PerkSprite;

	[SerializeField]
	private UISprite m_ConsumableEffectSprite;

	[SerializeField]
	private UILabel m_ConsumableEffectInfo;

	[SerializeField]
	private UILabel m_ConsumableLabel;

	[SerializeField]
	private GameObject m_ConsumableEffectRoot;

	[SerializeField]
	private GameObject m_WeaponStatsRoot;

	[SerializeField]
	private GameObject m_MasteryTextRoot;

	[SerializeField]
	private UILabel m_MasteryTextTitle;

	[SerializeField]
	private UILabel m_MasteryTextDescription;

	[SerializeField]
	private UISprite m_MasteryBarCurrent;

	[SerializeField]
	private UISprite m_MasteryBarDelta;

	[SerializeField]
	private UILabel m_MasteryRank;

	[SerializeField]
	private GameObject m_ClassNameRoot;

	[SerializeField]
	private UILabel m_ClassName;

	private SoundTriggerList m_SoundTriggers;

	private bool m_IsHidden = true;

	private WorldMapStateMgr m_StateMgr;

	private CraftingRecipeGameData m_RecipeItem;

	private int m_requirementLevel;

	private bool m_failedLevelRequirement;

	private int m_ClassRanksGained;

	[method: MethodImpl(32)]
	public event Action<BasicShopOfferBalancingData> ShopOfferBought;

	public BasicShopOfferBalancingData GetModel()
	{
		return m_Model;
	}

	public void ShowTooltip()
	{
		if (m_RecipeItem != null)
		{
			if (m_failedLevelRequirement)
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(m_IconRoot, m_RecipeItem, true, false, m_requirementLevel);
			}
			else
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(m_IconRoot, m_RecipeItem, true, false);
			}
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(m_IconRoot, m_Item, true, false);
		}
	}

	public void ShowPerkTooltip()
	{
		EquipmentGameData equipmentGameData = m_Item as EquipmentGameData;
		if (equipmentGameData != null && (bool)m_PerkSprite)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowPerkOverlay(m_PerkSprite.cachedTransform, equipmentGameData, true);
		}
	}

	public void SetModel(BasicShopOfferBalancingData model, WorldMapStateMgr stateMgr, bool applyDojoMasteryBonus = false)
	{
		RegisterEventHandlers();
		m_SoundTriggers = GetComponent<SoundTriggerList>();
		RemoveAsset(m_Item);
		m_RecipeItem = null;
		m_Item = null;
		m_Model = model;
		m_StateMgr = stateMgr;
		List<IInventoryItemGameData> shopOfferContent = DIContainerLogic.GetShopService().GetShopOfferContent(DIContainerInfrastructure.GetCurrentPlayer(), m_Model, DIContainerLogic.GetSalesManagerService().GetOfferSaleDetails(m_Model.NameId));
		m_ItemIconSprite.gameObject.SetActive(false);
		m_requirementLevel = 0;
		m_failedLevelRequirement = false;
		m_WeaponStatsRoot.SetActive(false);
		m_ConsumableEffectRoot.SetActive(false);
		m_MasteryTextRoot.SetActive(false);
		m_ClassNameRoot.SetActive(false);
		m_SlotRoot.SetActive(false);
		m_MasteryBarCurrent.transform.parent.gameObject.SetActive(false);
		m_MasteryBadgeRoot.SetActive(false);
		if (string.IsNullOrEmpty(m_Model.AssetId))
		{
			m_AdditionalSprite.gameObject.SetActive(true);
			IInventoryItemGameData inventoryItemGameData = shopOfferContent.FirstOrDefault();
			if (inventoryItemGameData.ItemBalancing.ItemType == InventoryItemType.CraftingRecipes)
			{
				m_MasteryBadgeRoot.SetActive(false);
				m_RecipeItem = inventoryItemGameData as CraftingRecipeGameData;
				m_RecipeScrollRoot.SetActive(true);
				CraftingRecipeGameData craftingRecipeGameData = inventoryItemGameData as CraftingRecipeGameData;
				Dictionary<string, LootInfoData> loot = DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level);
				List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerInfrastructure.GetCurrentPlayer(), loot);
				m_Item = itemsFromLoot[0];
				if (craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Consumable)
				{
					m_WeaponStatsRoot.SetActive(false);
					m_ConsumableEffectRoot.SetActive(true);
					m_AdditionalSprite.gameObject.SetActive(false);
					m_ItemIconSprite.gameObject.SetActive(true);
					if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset("Consumables"))
					{
						GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject("Consumables") as GameObject;
						m_ItemIconSprite.atlas = gameObject.GetComponent<UIAtlas>();
					}
					m_ItemIconSprite.spriteName = m_Item.ItemAssetName;
					ConsumableItemGameData consumableItemGameData = m_Item as ConsumableItemGameData;
					ConsumableEffectInfo effectValueString = ConsumableItemGameData.GetEffectValueString(consumableItemGameData.BalancingData, craftingRecipeGameData.Data.Level, "_Large");
					m_ConsumableEffectInfo.text = effectValueString.LocalizedText;
					m_ConsumableLabel.text = consumableItemGameData.ItemLocalizedName;
				}
				else if (craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.MainHandEquipment)
				{
					m_WeaponStatsRoot.SetActive(true);
					m_ConsumableEffectRoot.SetActive(false);
					EquipmentGameData equip2 = m_Item as EquipmentGameData;
					m_AdditionalSprite.spriteName = EquipmentGameData.GetRestrictedBirdIcon(equip2);
					BirdGameData birdGameData = DIContainerInfrastructure.GetCurrentPlayer().Birds.FirstOrDefault((BirdGameData b) => b.BalancingData.NameId == equip2.BalancingData.RestrictedBirdId);
					if ((bool)m_PerkSprite)
					{
						m_PerkSprite.spriteName = EquipmentGameData.GetPerkIcon(equip2);
					}
					m_ItemStats.SetIconSprite("Character_Damage_Large");
					m_ItemStats.RefreshStat(false, birdGameData != null, EquipmentGameData.GetItemMainStat(equip2, 3), (birdGameData == null) ? 0f : birdGameData.MainHandItem.ItemMainStat);
				}
				else if (craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.OffHandEquipment)
				{
					m_WeaponStatsRoot.SetActive(true);
					m_ConsumableEffectRoot.SetActive(false);
					EquipmentGameData equip = m_Item as EquipmentGameData;
					m_AdditionalSprite.spriteName = EquipmentGameData.GetRestrictedBirdIcon(equip);
					if ((bool)m_PerkSprite)
					{
						m_PerkSprite.spriteName = EquipmentGameData.GetPerkIcon(equip);
					}
					BirdGameData birdGameData2 = DIContainerInfrastructure.GetCurrentPlayer().Birds.FirstOrDefault((BirdGameData b) => b.BalancingData.NameId == equip.BalancingData.RestrictedBirdId);
					m_ItemStats.SetIconSprite("Character_Health_Large");
					m_ItemStats.RefreshStat(false, birdGameData2 != null, EquipmentGameData.GetItemMainStat(equip, 3), (birdGameData2 == null) ? 0f : birdGameData2.OffHandItem.ItemMainStat);
				}
				InstantiateAsset(m_Item, m_IconRoot);
			}
			else
			{
				m_RecipeScrollRoot.SetActive(false);
				m_Item = inventoryItemGameData;
				if (m_Item is MasteryItemGameData)
				{
					HandleMastery();
				}
				else if (m_Item is ClassItemGameData)
				{
					m_SlotRoot.SetActive(true);
					InstantiateAsset(m_Item, m_IconRoot);
					m_AdditionalSprite.spriteName = ClassItemGameData.GetRestrictedBirdIcon(m_Item.ItemBalancing);
				}
				else
				{
					InstantiateAsset(m_Item, m_IconRoot);
				}
			}
		}
		else
		{
			m_RecipeScrollRoot.SetActive(false);
			m_ItemIconSprite.gameObject.SetActive(false);
		}
		m_BlindMainLabel.gameObject.SetActive(true);
		Requirement requirement = DIContainerLogic.GetShopService().GetBuyResourcesRequirements(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, m_Model).FirstOrDefault();
		if (m_CostBlind != null && m_Model is BuyableShopOfferBalancingData && DIContainerLogic.GetShopService().IsPriceDiscount(m_Model))
		{
			m_CostBlind.gameObject.SetActive(false);
			m_DiscountCostBlind.gameObject.SetActive(true);
			m_DiscountOldCost.text = m_Model.BuyRequirements.Where((Requirement r) => r.NameId == "lucky_coin").FirstOrDefault().Value.ToString();
			if (m_Model.SpecialOfferLabelColor != null)
			{
				m_DiscountCostBlind.m_Value.color = new Color(m_Model.SpecialOfferLabelColor[0], m_Model.SpecialOfferLabelColor[1], m_Model.SpecialOfferLabelColor[2]);
			}
			else
			{
				m_DiscountCostBlind.m_Value.color = new Color(0f, 0f, 0f);
			}
			if (m_DiscountCostBlind.m_CostBody != null)
			{
				if (m_Model.SpecialOfferBackgroundColor != null)
				{
					m_DiscountCostBlind.m_CostBody.color = new Color(m_Model.SpecialOfferBackgroundColor[0], m_Model.SpecialOfferBackgroundColor[1], m_Model.SpecialOfferBackgroundColor[2]);
				}
				else
				{
					m_DiscountCostBlind.m_CostBody.color = new Color(0.5f, 1f, 0f);
				}
			}
			if (requirement != null)
			{
				m_DiscountCostBlind.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(requirement.NameId).AssetBaseId, null, requirement.Value, string.Empty);
			}
		}
		else
		{
			if (requirement != null)
			{
				m_CostBlind.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(requirement.NameId).AssetBaseId, null, requirement.Value, string.Empty);
			}
			m_CostBlind.gameObject.SetActive(true);
			m_DiscountCostBlind.gameObject.SetActive(false);
		}
		List<Requirement> failed = new List<Requirement>();
		m_BuyButtonTrigger.Clicked -= BuyOfferClicked;
		m_BuyButtonTrigger.Clicked += BuyOfferClicked;
		if (DIContainerLogic.GetShopService().IsOfferBuyable(DIContainerInfrastructure.GetCurrentPlayer(), m_Model, out failed))
		{
			RequirementRoot requirementRoot = null;
			foreach (RequirementRoot requirementRoot3 in m_RequirementRoots)
			{
				foreach (GameObject root in requirementRoot3.Roots)
				{
					root.SetActive(false);
				}
				if (requirementRoot3.Type == RequirementType.None)
				{
					requirementRoot = requirementRoot3;
				}
			}
			foreach (GameObject root2 in requirementRoot.Roots)
			{
				root2.SetActive(true);
			}
			SetBlindLabelItemText(m_Item);
		}
		else
		{
			Requirement requirement2 = failed.FirstOrDefault();
			IInventoryItemGameData inventoryItemGameData2 = shopOfferContent.FirstOrDefault();
			RequirementRoot requirementRoot2 = null;
			foreach (RequirementRoot requirementRoot4 in m_RequirementRoots)
			{
				foreach (GameObject root3 in requirementRoot4.Roots)
				{
					root3.SetActive(false);
				}
				if (requirementRoot4.Type == requirement2.RequirementType)
				{
					requirementRoot2 = requirementRoot4;
				}
			}
			foreach (GameObject root4 in requirementRoot2.Roots)
			{
				root4.SetActive(true);
			}
			switch (requirement2.RequirementType)
			{
			case RequirementType.HaveItem:
			case RequirementType.HaveClass:
				requirementRoot2.TextLabel.text = DIContainerInfrastructure.GetLocaService().Tr("req_failed_haveitem", "You do not own this Item");
				break;
			case RequirementType.Level:
				requirementRoot2.TextLabel.text = requirement2.Value.ToString("0");
				m_requirementLevel = (int)requirement2.Value;
				m_failedLevelRequirement = true;
				break;
			case RequirementType.NotHaveItem:
			case RequirementType.NotHaveClass:
				SetBlindLabelItemText(m_Item);
				break;
			case RequirementType.PayItem:
				SetBlindLabelItemText(m_Item);
				break;
			case RequirementType.HaveBird:
			{
				BirdBalancingData balancing = null;
				if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>(requirement2.NameId, out balancing))
				{
					requirementRoot2.IconSprite.spriteName = balancing.AssetId;
				}
				break;
			}
			}
			HandleMasteryRequirementFailed(requirement2);
		}
		if (applyDojoMasteryBonus)
		{
			m_MasteryTextTitle.color = new Color(0.46f, 1f, 0f);
		}
	}

	private void HandleMasteryRequirementFailed(Requirement firstFailed)
	{
		switch (firstFailed.RequirementType)
		{
		case RequirementType.None:
			break;
		case RequirementType.PayItem:
			break;
		case RequirementType.HaveItem:
		case RequirementType.HaveClass:
			m_MasteryBarCurrent.transform.parent.gameObject.SetActive(false);
			break;
		case RequirementType.NotHaveItem:
		case RequirementType.NotHaveClass:
			m_MasteryBarCurrent.transform.parent.gameObject.SetActive(false);
			break;
		case RequirementType.HaveBird:
			break;
		case RequirementType.Level:
			break;
		case RequirementType.CooldownFinished:
			break;
		case RequirementType.IsSpecificWeekday:
			break;
		case RequirementType.IsNotSpecificWeekday:
			break;
		case RequirementType.HaveCurrentHotpsotState:
			break;
		case RequirementType.HavePassedCycleTime:
			break;
		case RequirementType.NotHavePassedCycleTime:
			break;
		case RequirementType.NotHaveItemWithLevel:
			m_MasteryBarCurrent.transform.parent.gameObject.SetActive(false);
			break;
		case RequirementType.HaveItemWithLevel:
			m_MasteryBarCurrent.transform.parent.gameObject.SetActive(false);
			break;
		case RequirementType.UsedFriends:
			break;
		case RequirementType.HaveUnlockedHotpsot:
			break;
		case RequirementType.NotHaveUnlockedHotpsot:
			break;
		case RequirementType.HaveLessThan:
			break;
		case RequirementType.HaveBirdCount:
			break;
		case RequirementType.IsConverted:
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void HandleMastery()
	{
		m_AdditionalSprite.gameObject.SetActive(false);
		m_MasteryBadgeRoot.SetActive(true);
		m_MasteryTextRoot.SetActive(true);
		MasteryItemGameData masteryItemGameData = m_Item as MasteryItemGameData;
		if (masteryItemGameData == null)
		{
			return;
		}
		string associatedClass = masteryItemGameData.BalancingData.AssociatedClass;
		if (associatedClass.Equals("ALL") || associatedClass.StartsWith("bird_"))
		{
			m_ItemIconSprite.gameObject.SetActive(true);
			m_MasteryBarCurrent.transform.parent.gameObject.SetActive(false);
			if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset("GenericElements"))
			{
				GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject("GenericElements") as GameObject;
				if (gameObject != null)
				{
					m_ItemIconSprite.atlas = gameObject.GetComponent<UIAtlas>();
				}
				m_ItemIconSprite.spriteName = m_Item.ItemAssetName;
				m_ItemIconSprite.MakePixelPerfect();
			}
		}
		else
		{
			InstantiateAsset(m_Item, m_IconRoot);
			if (m_IconInstanciated != null)
			{
				m_IconInstanciated.transform.localScale = Vector3.one * 0.6f;
			}
			m_ItemIconSprite.gameObject.SetActive(false);
			IInventoryItemGameData data;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, masteryItemGameData.BalancingData.AssociatedClass, out data))
			{
				ClassItemGameData classItemGameData = data as ClassItemGameData;
				m_MasteryBarCurrent.transform.parent.gameObject.SetActive(true);
				if (classItemGameData != null)
				{
					m_MasteryBarCurrent.fillAmount = classItemGameData.MasteryProgressNextRank();
					m_MasteryBarDelta.fillAmount = classItemGameData.MasteryProgressNextRank(masteryItemGameData.ItemValue);
					m_MasteryRank.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(classItemGameData.Data.Level);
				}
			}
			else
			{
				m_MasteryBarCurrent.transform.parent.gameObject.SetActive(false);
			}
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("{value_1}", DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(masteryItemGameData.ItemValue));
		m_MasteryTextTitle.text = DIContainerInfrastructure.GetLocaService().Tr("offer_mastery_increase", dictionary);
		m_MasteryTextDescription.text = m_Item.ItemLocalizedDesc.Replace("{value_2}", masteryItemGameData.GetMasteryClassName(DIContainerInfrastructure.GetCurrentPlayer()));
	}

	private void SetBlindLabelItemText(IInventoryItemGameData item)
	{
		m_BlindMainLabel.text = item.ItemLocalizedName;
		if (item is ClassItemGameData)
		{
			m_ClassNameRoot.SetActive(true);
			m_ClassName.text = item.ItemLocalizedName;
		}
	}

	private void BuyCoinsClicked()
	{
		List<Requirement> allModifiedBuyRequirements = DIContainerLogic.GetShopService().GetAllModifiedBuyRequirements(DIContainerInfrastructure.GetCurrentPlayer(), m_Model);
		Requirement requirement = allModifiedBuyRequirements.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem);
		if (requirement != null && requirement.RequirementType == RequirementType.PayItem)
		{
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, requirement.NameId, out data))
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_MissingCurrencyPopup.EnterPopup(data.ItemBalancing.NameId, requirement.Value);
			}
		}
	}

	private void BuyOfferClicked()
	{
		List<Requirement> failed = new List<Requirement>();
		if (!DIContainerLogic.GetShopService().IsOfferBuyable(DIContainerInfrastructure.GetCurrentPlayer(), m_Model, out failed))
		{
			if ((bool)m_SoundTriggers)
			{
				m_SoundTriggers.OnTriggerEventFired("purchase_failed");
			}
			if (failed.Any((Requirement r) => r.RequirementType == RequirementType.PayItem))
			{
				BuyCoinsClicked();
			}
			return;
		}
		if (m_StateMgr.m_WorkShopUI != null && m_StateMgr.m_WorkShopUI.m_MenuType == ShopMenuType.Dojo)
		{
			DIContainerInfrastructure.GetCurrentPlayer().ClassRankedUp -= ClassRankedUp;
			DIContainerInfrastructure.GetCurrentPlayer().ClassRankedUp += ClassRankedUp;
		}
		if (DIContainerLogic.GetShopService().BuyShopOffer(DIContainerInfrastructure.GetCurrentPlayer(), m_Model) == null)
		{
			DebugLog.Error("Failed to buy Offer!");
			if ((bool)m_SoundTriggers)
			{
				m_SoundTriggers.OnTriggerEventFired("purchase_failed");
			}
			return;
		}
		if (m_StateMgr.m_WorkShopUI != null && m_StateMgr.m_WorkShopUI.m_MenuType == ShopMenuType.Dojo)
		{
			bool flag = false;
			foreach (Requirement buyRequirement in m_Model.BuyRequirements)
			{
				if (buyRequirement.RequirementType == RequirementType.PayItem && buyRequirement.NameId == "lucky_coin" && buyRequirement.Value > 0f)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.DojoOffersBought = 0;
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.DojoOffersBought++;
			}
			if (m_ClassRanksGained > 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_RankUpEffectPrefab, base.transform.position, Quaternion.identity) as GameObject;
				if (gameObject != null)
				{
					UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
					componentInChildren.text = ((m_ClassRanksGained != 1) ? DIContainerInfrastructure.GetLocaService().Tr("dojo_rankup_plural", new Dictionary<string, string> { 
					{
						"{value_1}",
						m_ClassRanksGained.ToString("0")
					} }) : DIContainerInfrastructure.GetLocaService().Tr("dojo_rankup_singular", "Rank Up!"));
					UnityEngine.Object.Destroy(gameObject, gameObject.PlayAnimationOrAnimatorState("RankUpEffect"));
				}
				m_ClassRanksGained = 0;
			}
			for (int i = 0; i < DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers.Count; i++)
			{
				string text = DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers[i];
				if (text == m_Model.NameId)
				{
					DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers[i] = string.Empty;
				}
			}
			DIContainerInfrastructure.GetCurrentPlayer().ClassRankedUp -= ClassRankedUp;
		}
		if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("purchase_successful");
		}
		DeRegisterEventHandlers();
		if (this.ShopOfferBought != null)
		{
			this.ShopOfferBought(m_Model);
		}
	}

	private void ClassRankedUp()
	{
		m_ClassRanksGained++;
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
	}

	private void DeRegisterEventHandlers()
	{
		if ((bool)m_BuyButtonTrigger)
		{
			m_BuyButtonTrigger.Clicked -= BuyCoinsClicked;
			m_BuyButtonTrigger.Clicked -= BuyOfferClicked;
		}
	}

	private void OnDestroy()
	{
		if ((bool)m_IconInstanciated)
		{
		}
		DeRegisterEventHandlers();
	}

	private void RemoveAsset(IInventoryItemGameData item)
	{
		if ((bool)m_IconInstanciated && item != null)
		{
			switch (item.ItemBalancing.ItemType)
			{
			case InventoryItemType.Class:
				DIContainerInfrastructure.GetClassAssetProvider().DestroyObject(item.ItemAssetName, m_IconInstanciated);
				break;
			case InventoryItemType.Consumable:
				break;
			case InventoryItemType.CraftingRecipes:
				DIContainerInfrastructure.GetEquipmentAssetProvider().DestroyObject(item.ItemAssetName, m_IconInstanciated);
				break;
			case InventoryItemType.Ingredients:
				break;
			case InventoryItemType.MainHandEquipment:
				DIContainerInfrastructure.GetEquipmentAssetProvider().DestroyObject(item.ItemAssetName, m_IconInstanciated);
				break;
			case InventoryItemType.OffHandEquipment:
				DIContainerInfrastructure.GetEquipmentAssetProvider().DestroyObject(item.ItemAssetName, m_IconInstanciated);
				break;
			case InventoryItemType.PlayerStats:
				break;
			case InventoryItemType.PlayerToken:
				break;
			case InventoryItemType.Points:
				break;
			case InventoryItemType.Premium:
				break;
			case InventoryItemType.Resources:
				break;
			case InventoryItemType.Story:
				break;
			case InventoryItemType.Mastery:
				DIContainerInfrastructure.GetClassAssetProvider().DestroyObject(item.ItemBalancing.AssetBaseId, m_IconInstanciated);
				break;
			case InventoryItemType.EventBattleItem:
			case InventoryItemType.EventCollectible:
				break;
			}
		}
	}

	private void InstantiateAsset(IInventoryItemGameData item, Transform root)
	{
		if ((bool)m_IconInstanciated)
		{
			RemoveAsset(item);
		}
		if (item == null)
		{
			return;
		}
		switch (item.ItemBalancing.ItemType)
		{
		case InventoryItemType.Class:
			m_IconInstanciated = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(item.ItemAssetName, m_IconRoot, Vector3.zero, Quaternion.identity, false);
			break;
		case InventoryItemType.CraftingRecipes:
			m_IconInstanciated = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(item.ItemAssetName, m_IconRoot, Vector3.zero, Quaternion.identity, false);
			break;
		case InventoryItemType.MainHandEquipment:
			m_IconInstanciated = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(item.ItemAssetName, m_IconRoot, Vector3.zero, Quaternion.identity, false);
			break;
		case InventoryItemType.OffHandEquipment:
			m_IconInstanciated = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(item.ItemAssetName, m_IconRoot, Vector3.zero, Quaternion.identity, false);
			break;
		case InventoryItemType.Mastery:
		{
			string assetBaseId = item.ItemBalancing.AssetBaseId;
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			string associatedClass = (item as MasteryItemGameData).BalancingData.AssociatedClass;
			if (currentPlayer.Data.EquippedSkins.ContainsKey(associatedClass))
			{
				assetBaseId = DIContainerBalancing.Service.GetBalancingData<ClassSkinBalancingData>(currentPlayer.Data.EquippedSkins[associatedClass]).AssetBaseId;
			}
			m_IconInstanciated = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(assetBaseId, m_IconRoot, Vector3.zero, Quaternion.identity, false);
			break;
		}
		}
		if ((bool)m_IconInstanciated)
		{
			m_IconInstanciated.transform.localScale = Vector3.one;
		}
	}

	public float ShowBoughtIndicator()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_BuyIndicatorPrefab);
		UnityHelper.SetLayerRecusively(gameObject, LayerMask.NameToLayer("Interface"));
		gameObject.transform.localPosition = base.transform.position;
		UnityEngine.Object.Destroy(gameObject, gameObject.GetComponent<Animation>().clip.length);
		return gameObject.GetComponent<Animation>().clip.length;
	}

	public float ShowInstant()
	{
		if (m_IsHidden)
		{
			m_IsHidden = false;
			if (GetComponent<Animation>() != null && (bool)GetComponent<Animation>()["WorldShopOffer_Show"])
			{
				GetComponent<Animation>().Play("WorldShopOffer_Show");
				GetComponent<Animation>()["WorldShopOffer_Show"].time = GetComponent<Animation>()["WorldShopOffer_Show"].length;
				return 0f;
			}
		}
		return 0f;
	}

	public float Show()
	{
		if (m_IsHidden)
		{
			m_IsHidden = false;
			if (GetComponent<Animation>() != null && (bool)GetComponent<Animation>()["WorldShopOffer_Show"])
			{
				GetComponent<Animation>().Play("WorldShopOffer_Show");
				return GetComponent<Animation>()["WorldShopOffer_Show"].length;
			}
		}
		return 0f;
	}

	internal float Hide()
	{
		float result = 0f;
		if (!m_IsHidden)
		{
			if (GetComponent<Animation>() != null && (bool)GetComponent<Animation>()["WorldShopOffer_Hide"])
			{
				GetComponent<Animation>().Play("WorldShopOffer_Hide");
				result = GetComponent<Animation>()["WorldShopOffer_Hide"].length;
			}
			m_IsHidden = true;
		}
		return result;
	}

	public IEnumerator Disable(float delay)
	{
		yield return new WaitForSeconds(delay);
		base.gameObject.SetActive(false);
	}

	public float HideInstant()
	{
		float result = 0f;
		if (!m_IsHidden)
		{
			if (GetComponent<Animation>() != null && (bool)GetComponent<Animation>()["WorldShopOffer_Hide"])
			{
				GetComponent<Animation>().Play("WorldShopOffer_Hide");
				GetComponent<Animation>()["WorldShopOffer_Hide"].time = GetComponent<Animation>()["WorldShopOffer_Hide"].length;
			}
			m_IsHidden = true;
		}
		return result;
	}
}
