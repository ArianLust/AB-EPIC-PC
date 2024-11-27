using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.Generic;
using Interfaces.Purchasing;
using Rcs;
using UnityEngine;

public class ShopOfferBlindBase : MonoBehaviour
{
	[SerializeField]
	private ResourceCostBlind m_CostBlind;

	[SerializeField]
	private GameObject m_BuyIndicatorPrefab;

	[SerializeField]
	private UILabel m_BlindHeader;

	[SerializeField]
	public UIInputTrigger m_BuyButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_InfoButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_BackButtonTrigger;

	[SerializeField]
	private SoundTriggerList m_soundTriggers;

	[SerializeField]
	public UISprite m_birdIcon;

	protected BasicShopOfferBalancingData m_model;

	protected List<IInventoryItemGameData> m_items;

	private ShopWindowStateMgr m_stateMgr;

	protected IInventoryItemGameData m_item;

	private bool m_managedExternal;

	private bool m_unavailable;

	private Product m_product;

	private Product m_discountProduct;

	protected ClassItemBalancingData m_classItemBalancing;

	protected bool m_lockedBird;

	protected bool m_isPurchased;

	protected bool m_isClassItem;

	protected bool m_isSkinItem;

	private bool m_discountOffer;

	private bool m_validPremiumCostDiscount;

	private bool m_flippedToBack;

	protected SaleOfferTupel m_saleModel;

	public BasicShopOfferBalancingData OfferModel
	{
		get
		{
			return m_model;
		}
	}

	[method: MethodImpl(32)]
	public event Action<BasicShopOfferBalancingData> ShopOfferBought;

	public virtual void SetModel(BasicShopOfferBalancingData model, ShopWindowStateMgr stateMgr)
	{
		if (model == null || stateMgr == null)
		{
			Debug.LogError("Set Model was initialized with null!");
			return;
		}
		m_model = model;
		m_stateMgr = stateMgr;
		m_saleModel = DIContainerLogic.GetSalesManagerService().GetOfferSaleDetails(m_model.NameId);
		m_discountOffer = !m_saleModel.IsEmpty();
		m_items = Enumerable.ToList(from i in DIContainerLogic.GetShopService().GetShopOfferContent(DIContainerInfrastructure.GetCurrentPlayer(), m_model, m_saleModel)
			where !i.Name.Contains("unlock")
			select i);
		m_item = m_items.FirstOrDefault();
		if (m_model is PremiumShopOfferBalancingData)
		{
			m_managedExternal = true;
			PreparePremiumOffer();
		}
		else
		{
			m_managedExternal = false;
		}
		m_BlindHeader.text = (string.IsNullOrEmpty(m_model.LocaId) ? m_product.name : DIContainerInfrastructure.GetLocaService().GetShopOfferName(m_model.LocaId));
		if (m_managedExternal)
		{
			SetupPremiumOfferCostblind();
		}
		CheckForBirdState();
		m_InfoButtonTrigger.gameObject.SetActive(IsInfoButtonAvailable());
		List<Requirement> remainingReqs;
		m_isPurchased = DIContainerLogic.GetShopService().WasOfferBought(m_model, DIContainerInfrastructure.GetCurrentPlayer(), out remainingReqs);
		RegisterEventHandlers();
	}

	private bool IsInfoButtonAvailable()
	{
		bool flag = m_model.Category == "shop_global_premium_soft" || m_model.Category == "shop_global_premium";
		bool flag2 = m_items.Count((IInventoryItemGameData i) => i.ItemBalancing.ItemType == InventoryItemType.Class) > 1;
		return !flag && !flag2;
	}

	protected void SetupCostBlind(UILabel oldPrice)
	{
		List<Requirement> list = null;
		list = DIContainerLogic.GetShopService().GetBuyResourcesRequirements(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, m_model);
		Requirement requirement = list.FirstOrDefault();
		if (requirement != null)
		{
			IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(requirement.NameId);
			m_CostBlind.SetModel(balancingData.AssetBaseId, null, requirement.Value, DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, balancingData.NameId)));
		}
		if (!(oldPrice != null))
		{
			return;
		}
		if (m_model is BuyableShopOfferBalancingData)
		{
			List<Requirement> buyResourcesRequirements = DIContainerLogic.GetShopService().GetBuyResourcesRequirements(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, m_model, false);
			Requirement requirement2 = buyResourcesRequirements.FirstOrDefault();
			if (requirement2 != null)
			{
				oldPrice.text = requirement2.Value.ToString();
			}
		}
		if (m_model is PremiumShopOfferBalancingData && m_discountProduct.price != null)
		{
			oldPrice.text = m_discountProduct.price;
		}
	}

	public void ShowTooltip()
	{
		if (m_items.Count > 1)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(base.transform, m_items, m_model, true);
		}
		else if (m_item != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(base.transform, m_item, true, false);
		}
	}

	private void PurchaseFailed()
	{
		List<Requirement> allModifiedBuyRequirements = DIContainerLogic.GetShopService().GetAllModifiedBuyRequirements(DIContainerInfrastructure.GetCurrentPlayer(), m_model);
		Requirement requirement = allModifiedBuyRequirements.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem);
		if (requirement == null || requirement.RequirementType != RequirementType.PayItem)
		{
			return;
		}
		IInventoryItemGameData data = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, requirement.NameId, out data))
		{
			int index = 0;
			if (data.ItemBalancing.NameId == "lucky_coin")
			{
				index = 1;
			}
			else if (data.ItemBalancing.NameId == "gold")
			{
				index = 0;
			}
			else if (data.ItemBalancing.NameId == "friendship_essence")
			{
				index = 2;
			}
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[index].m_StatBar.SwitchToShop();
		}
	}

	private void BuyOfferClicked()
	{
		if (CheckForCampDoublePurchase())
		{
			DebugLog.Log(GetType(), "BuyOfferClicked CheckForCampDoublePurchase failed");
			return;
		}
		if (m_managedExternal)
		{
			HandleInAppPurchase();
			return;
		}
		List<Requirement> failed;
		if (!DIContainerLogic.GetShopService().IsOfferBuyable(DIContainerInfrastructure.GetCurrentPlayer(), m_model, out failed))
		{
			PurchaseFailed();
			if ((bool)m_soundTriggers)
			{
				m_soundTriggers.OnTriggerEventFired("purchase_failed");
			}
			return;
		}
		List<IInventoryItemGameData> list = DIContainerLogic.GetShopService().BuyShopOffer(DIContainerInfrastructure.GetCurrentPlayer(), m_model, "buyShopOffer", false, 0, m_stateMgr.m_Entersource);
		if (list == null)
		{
			DebugLog.Error("Failed to buy Offer!");
			if ((bool)m_soundTriggers)
			{
				m_soundTriggers.OnTriggerEventFired("purchase_failed");
			}
		}
		else
		{
			HandleOfferBought();
		}
	}

	private bool CheckForCampDoublePurchase()
	{
		IInventoryItemGameData data = null;
		IInventoryItemGameData data2 = null;
		InventoryGameData inventoryGameData = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData;
		DIContainerLogic.InventoryService.TryGetItemGameData(inventoryGameData, "cauldron_leveled", out data);
		DIContainerLogic.InventoryService.TryGetItemGameData(inventoryGameData, "forge_leveled", out data2);
		if (m_model.NameId.Contains("offer_upgrade_cauldron_01") && data.ItemData.Level >= 2)
		{
			return true;
		}
		if (m_model.NameId.Contains("offer_upgrade_cauldron_02") && data.ItemData.Level >= 3)
		{
			return true;
		}
		if (m_model.NameId.Contains("offer_upgrade_forge_01") && data2.ItemData.Level >= 2)
		{
			return true;
		}
		if (m_model.NameId.Contains("offer_upgrade_forge_02") && data2.ItemData.Level >= 3)
		{
			return true;
		}
		return false;
	}

	private void HandleOfferBought()
	{
		if ((bool)m_soundTriggers)
		{
			m_soundTriggers.OnTriggerEventFired("purchase_successful");
		}
		DeRegisterExternalEventHandlers();
		DeRegisterEventHandlers();
		if (this.ShopOfferBought != null)
		{
			this.ShopOfferBought(m_model);
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateFriendshipEssenceBar();
		if (m_model.UniqueOffer)
		{
			RemoveOffer();
			UnityEngine.Object.Destroy(base.gameObject, 1f);
		}
		List<Requirement> failed;
		if (!m_model.UniqueOffer && DIContainerLogic.GetShopService().IsOfferShowable(DIContainerInfrastructure.GetCurrentPlayer(), m_model, out failed))
		{
			StartCoroutine(BuyAndSoftRefresh());
		}
		else
		{
			StartCoroutine(BuyAndRefresh());
		}
		if (m_item != null && (m_item.ItemBalancing.ItemType == InventoryItemType.Class || m_item.ItemBalancing.ItemType == InventoryItemType.Skin) && DIContainerInfrastructure.BaseStateMgr != null)
		{
			DIContainerInfrastructure.BaseStateMgr.RefreshBirdMarkers();
		}
		if (m_model.NameId == "offer_buy_cauldron")
		{
			CampStateMgr campStateMgr = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			if ((bool)campStateMgr)
			{
				campStateMgr.ForceAddCauldron();
			}
		}
		else if (m_model.NameId.Contains("offer_permanent_golden_chili"))
		{
			CampStateMgr campStateMgr2 = DIContainerInfrastructure.GetCoreStateMgr().m_CampStateMgr;
			if ((bool)campStateMgr2)
			{
				campStateMgr2.ForceAddChili();
			}
		}
	}

	private IEnumerator BuyAndSoftRefresh()
	{
		DeRegisterEventHandlers();
		yield return new WaitForSeconds(ShowBoughtIndicator());
		if (m_stateMgr != null)
		{
			m_stateMgr.SoftRefresh();
		}
		RegisterEventHandlers();
	}

	private IEnumerator BuyAndRefresh()
	{
		DeRegisterEventHandlers();
		yield return new WaitForSeconds(ShowBoughtIndicator());
		if (m_stateMgr != null)
		{
			m_stateMgr.HardRefresh();
		}
	}

	public void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		if (m_model != null)
		{
			m_BuyButtonTrigger.Clicked += BuyOfferClicked;
		}
		if (m_InfoButtonTrigger != null)
		{
			m_InfoButtonTrigger.Clicked += SwapBlind;
		}
		if (m_BackButtonTrigger != null)
		{
			m_BackButtonTrigger.Clicked += SwapBlind;
		}
	}

	public void DeRegisterEventHandlers()
	{
		if (m_model != null && (bool)m_BuyButtonTrigger)
		{
			m_BuyButtonTrigger.Clicked -= BuyOfferClicked;
		}
		if (m_InfoButtonTrigger != null)
		{
			m_InfoButtonTrigger.Clicked -= SwapBlind;
		}
		if (m_BackButtonTrigger != null)
		{
			m_BackButtonTrigger.Clicked -= SwapBlind;
		}
	}

	private void SwapBlind()
	{
		if (!(GetComponent<Animator>() == null))
		{
			string trigger = ((!m_flippedToBack) ? "Flipped" : "FlippedBack");
			GetComponent<Animator>().SetTrigger(trigger);
			m_flippedToBack = !m_flippedToBack;
		}
	}

	private void OnDestroy()
	{
		DeRegisterExternalEventHandlers();
		DeRegisterEventHandlers();
	}

	public float ShowBoughtIndicator()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_BuyIndicatorPrefab);
		if (gameObject != null)
		{
			UnityHelper.SetLayerRecusively(gameObject, base.gameObject.layer);
			gameObject.transform.position = base.transform.position + new Vector3(0f, 0f, -20f);
			UnityEngine.Object.Destroy(gameObject, gameObject.GetComponent<Animation>().clip.length);
			return gameObject.GetComponent<Animation>().clip.length;
		}
		return 0f;
	}

	protected IEnumerator ShowTimer(UILabel timerLabel)
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		float remainingDuration = DIContainerLogic.GetSalesManagerService().GetRemainingSaleDuration(m_model);
		DateTime targetTime = DIContainerLogic.GetTimingService().GetPresentTime().AddSeconds(remainingDuration);
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				timerLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(DIContainerLogic.GetTimingService().TimeLeftUntil(targetTime));
			}
			yield return new WaitForSeconds(1f);
		}
		RemoveOffer();
		m_stateMgr.HardRefresh();
	}

	private void RemoveOffer()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		DebugLog.Log("[SpecialOffersBlind] Removed Special Offer: " + m_model.NameId);
		if (m_model.UniqueOffer || m_model.Duration > 0 || m_model.EndDate > 0)
		{
			currentPlayer.Data.UniqueSpecialShopOffers.Add(m_model.NameId);
		}
		if (currentPlayer.Data.CurrentCooldownOffers == null)
		{
			currentPlayer.Data.CurrentCooldownOffers = new Dictionary<string, DateTime>();
		}
		if (m_model.DiscountCooldown > 0 && !currentPlayer.Data.CurrentCooldownOffers.ContainsKey(m_model.NameId))
		{
			currentPlayer.Data.CurrentCooldownOffers.Add(m_model.NameId, DIContainerLogic.GetTimingService().GetPresentTime());
		}
	}

	protected void SetDescriptionLabels(UILabel youHaveText, UILabel lockedLabel)
	{
		if (lockedLabel != null)
		{
			lockedLabel.gameObject.SetActive(m_lockedBird);
			if (m_lockedBird)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{value_1}", DIContainerInfrastructure.GetLocaService().GetCharacterName(m_classItemBalancing.RestrictedBirdId));
				lockedLabel.text = DIContainerInfrastructure.GetLocaService().Tr("shop_offer_birdrequired", dictionary);
			}
		}
		if (!(youHaveText == null))
		{
			if (m_item.ItemBalancing.ItemType != InventoryItemType.Consumable && !m_item.ItemBalancing.NameId.Contains("shard"))
			{
				youHaveText.gameObject.SetActive(false);
				return;
			}
			int itemValue = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_item.ItemBalancing.NameId);
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2.Add("{value_1}", DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(itemValue));
			Dictionary<string, string> replacementStrings = dictionary2;
			youHaveText.text = DIContainerInfrastructure.GetLocaService().Tr("shop_lbl_itemamount", replacementStrings);
		}
	}

	protected void SetAmountLabel(UILabel amountLabel, UILabel oldAmountLabel)
	{
		if (amountLabel == null)
		{
			return;
		}
		if (m_items.Count > 1)
		{
			amountLabel.gameObject.SetActive(false);
			return;
		}
		int num = m_model.OfferContents.FirstOrDefault().Value;
		if (!m_saleModel.IsEmpty() && m_saleModel.OfferDetails.SaleParameter == SaleParameter.Value)
		{
			num = m_saleModel.OfferDetails.ChangedValue;
		}
		if (num > 1)
		{
			amountLabel.gameObject.SetActive(true);
			amountLabel.text = DIContainerInfrastructure.GetLocaService().Tr("gen_prefix_multiplication", "x") + DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(num);
		}
		else
		{
			amountLabel.gameObject.SetActive(false);
		}
		if (oldAmountLabel != null)
		{
			oldAmountLabel.gameObject.SetActive(true);
			oldAmountLabel.text = DIContainerInfrastructure.GetLocaService().Tr("gen_prefix_multiplication", "x") + DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(m_model.OfferContents.FirstOrDefault().Value);
		}
	}

	protected void CheckForBirdState()
	{
		if (m_items.Count > 1 && m_item.ItemBalancing.ItemType != InventoryItemType.Skin && m_item.ItemBalancing.ItemType != InventoryItemType.Class)
		{
			IInventoryItemGameData inventoryItemGameData = m_items.FirstOrDefault((IInventoryItemGameData i) => i.ItemBalancing.ItemType == InventoryItemType.Class || i.ItemBalancing.ItemType == InventoryItemType.Skin);
			if (inventoryItemGameData != null)
			{
				m_item = inventoryItemGameData;
			}
		}
		BirdBalancingData birdBalancingData = null;
		switch (m_item.ItemBalancing.ItemType)
		{
		case InventoryItemType.Class:
			m_classItemBalancing = m_item.ItemBalancing as ClassItemBalancingData;
			birdBalancingData = DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>(m_classItemBalancing.RestrictedBirdId);
			m_isClassItem = true;
			break;
		case InventoryItemType.Skin:
		{
			ClassSkinBalancingData classSkinBalancingData = m_item.ItemBalancing as ClassSkinBalancingData;
			string originalClass = classSkinBalancingData.OriginalClass;
			m_classItemBalancing = DIContainerBalancing.Service.GetBalancingData<ClassItemBalancingData>(originalClass);
			birdBalancingData = DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>(m_classItemBalancing.RestrictedBirdId);
			m_isSkinItem = true;
			break;
		}
		}
		if (birdBalancingData == null)
		{
			return;
		}
		m_lockedBird = true;
		foreach (BirdGameData bird in DIContainerInfrastructure.GetCurrentPlayer().Birds)
		{
			if (bird.BalancingData.NameId == birdBalancingData.NameId)
			{
				m_lockedBird = false;
				break;
			}
		}
	}

	protected void SetupBundleGrid(UIGrid grid)
	{
		for (int i = 0; i < grid.transform.childCount; i++)
		{
			Transform child = grid.transform.GetChild(i);
			if (m_items.Count > i)
			{
				child.gameObject.SetActive(true);
				child.GetComponent<LootDisplayContoller>().SetModel(m_items[i], null, LootDisplayType.None, "_Large", false, false, true);
			}
			else
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	protected void SetOfferIcon(UISprite displaySprite, LootDisplayContoller ldc, IInventoryItemGameData item = null)
	{
		if (item == null)
		{
			item = m_item;
		}
		string assetId = m_model.AssetId;
		string atlasNameId = m_model.AtlasNameId;
		if (!string.IsNullOrEmpty(assetId) && (bool)displaySprite && m_model.OfferContents.Count == 1)
		{
			displaySprite.gameObject.SetActive(true);
			if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(atlasNameId))
			{
				GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(atlasNameId) as GameObject;
				displaySprite.atlas = gameObject.GetComponent<UIAtlas>();
			}
			else if (DIContainerInfrastructure.GetShopIconAtlasAssetProvider().ContainsAsset(atlasNameId))
			{
				GameObject gameObject2 = DIContainerInfrastructure.GetShopIconAtlasAssetProvider().GetObject(atlasNameId) as GameObject;
				displaySprite.atlas = gameObject2.GetComponent<UIAtlas>();
			}
			displaySprite.spriteName = assetId;
			displaySprite.MakePixelPerfect();
		}
		else
		{
			if ((bool)displaySprite)
			{
				displaySprite.gameObject.SetActive(false);
			}
			if (m_model.OfferContents.Count > 1)
			{
				displaySprite.gameObject.SetActive(true);
				ldc.SetModel(item, new List<IInventoryItemGameData>(), LootDisplayType.None, string.Empty, false, false, true, null, false, false, false, false);
			}
			else
			{
				ldc.SetModel(item, new List<IInventoryItemGameData>(), LootDisplayType.None, string.Empty, false, false, true, m_model, false, false, false, false);
			}
		}
		if (m_birdIcon != null && (m_isSkinItem || m_isClassItem))
		{
			switch (m_classItemBalancing.RestrictedBirdId)
			{
			case "bird_red":
				m_birdIcon.spriteName = "RedBird";
				break;
			case "bird_yellow":
				m_birdIcon.spriteName = "YellowBird";
				break;
			case "bird_white":
				m_birdIcon.spriteName = "WhiteBird";
				break;
			case "bird_black":
				m_birdIcon.spriteName = "BlackBird";
				break;
			case "bird_blue":
				m_birdIcon.spriteName = "BlueBirds";
				break;
			}
		}
	}

	protected void GenerateSkillInfo(SkillBlind primaryBlind, SkillBlind secondaryBlind)
	{
		BirdGameData birdGameData = DIContainerInfrastructure.GetCurrentPlayer().AllBirds.FirstOrDefault((BirdGameData b) => b.BalancingData.NameId == m_classItemBalancing.RestrictedBirdId);
		BirdGameData birdGameData2 = ((birdGameData == null) ? new BirdGameData(m_classItemBalancing.RestrictedBirdId) : new BirdGameData(birdGameData));
		if (m_item.ItemBalancing.ItemType == InventoryItemType.Class)
		{
			ClassItemGameData classItemGameData = new ClassItemGameData(m_item.ItemBalancing.NameId);
			DIContainerInfrastructure.GetCurrentPlayer().AdvanceBirdMasteryToHalfOfHighest(classItemGameData);
			DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { classItemGameData }, InventoryItemType.Class, birdGameData2.InventoryGameData);
		}
		else if (m_item.ItemBalancing.ItemType == InventoryItemType.Skin)
		{
			SkinItemGameData item = new SkinItemGameData(m_item.ItemBalancing.NameId);
			DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { item }, InventoryItemType.Skin, birdGameData2.InventoryGameData);
		}
		BirdCombatant invoker = new BirdCombatant(birdGameData2);
		SkillGameData skillGameData = new SkillGameData(m_classItemBalancing.SkillNameIds[0]);
		SkillGameData skillGameData2 = new SkillGameData(m_classItemBalancing.SkillNameIds[1]);
		primaryBlind.ShowSkillOverlay(skillGameData.GenerateSkillBattleData(), invoker, false);
		secondaryBlind.ShowSkillOverlay(skillGameData2.GenerateSkillBattleData(), invoker, false);
	}

	public void RegisterExternalEventHandlers()
	{
		DeRegisterExternalEventHandlers();
		if (m_managedExternal)
		{
			DIContainerInfrastructure.PurchasingService.PurchaseProgress += PurchasingService_PurchaseProgress;
			DIContainerInfrastructure.PurchasingService.PurchaseSuccess += PurchasingService_PurchaseSuccess;
			DIContainerInfrastructure.PurchasingService.RestorePurchaseProgress += PurchasingService_PurchaseProgress;
			DIContainerInfrastructure.PurchasingService.ConsumeVoucherSuccess += PurchasingServiceOnConsumeVoucherSuccess;
			DIContainerInfrastructure.PurchasingService.ConsumeVoucherError += PurchasingServiceOnConsumeVoucherError;
		}
	}

	public void DeRegisterExternalEventHandlers()
	{
		if (m_managedExternal)
		{
			DIContainerInfrastructure.PurchasingService.PurchaseProgress -= PurchasingService_PurchaseProgress;
			DIContainerInfrastructure.PurchasingService.PurchaseSuccess -= PurchasingService_PurchaseSuccess;
			DIContainerInfrastructure.PurchasingService.RestorePurchaseProgress -= PurchasingService_PurchaseProgress;
			DIContainerInfrastructure.PurchasingService.ConsumeVoucherSuccess -= PurchasingServiceOnConsumeVoucherSuccess;
			DIContainerInfrastructure.PurchasingService.ConsumeVoucherError -= PurchasingServiceOnConsumeVoucherError;
		}
	}

	private void PurchasingServiceOnConsumeVoucherError(Payment.ErrorCode code, string managedString)
	{
		DebugLog.Log(string.Concat("Consume Voucher failed: Code: ", code, " ", managedString));
	}

	private void PurchasingServiceOnConsumeVoucherSuccess(string managedString)
	{
		DebugLog.Log("Consume Voucher suceeded: " + managedString);
	}

	private void PurchasingService_PurchaseSuccess(string productId)
	{
		if ((m_validPremiumCostDiscount || !(productId != m_product.productId)) && (!m_validPremiumCostDiscount || !(productId != m_discountProduct.productId)))
		{
			HandleOfferBought();
		}
	}

	private void PurchasingService_PurchaseProgress(Purchase purchaseInfo)
	{
		if ((m_validPremiumCostDiscount || !(purchaseInfo.productID != m_product.productId)) && (!m_validPremiumCostDiscount || !(purchaseInfo.productID != m_discountProduct.productId)))
		{
			switch (purchaseInfo.status)
			{
			case PurchaseStatus.PURCHASE_SUCCEEDED:
				HandleOfferBought();
				break;
			case PurchaseStatus.PURCHASE_FAILED:
				DebugLog.Error("Purchase Failed!");
				DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("shop_purchase_failed", "Purchase Product has failed!"), "shop_purchase_failed");
				RegisterEventHandlers();
				break;
			case PurchaseStatus.PURCHASE_CANCELED:
				DebugLog.Log("Purchase Canceled!");
				RegisterEventHandlers();
				break;
			case PurchaseStatus.PURCHASE_PENDING:
				DebugLog.Warn("Purchase Pending!");
				RegisterEventHandlers();
				break;
			case PurchaseStatus.PURCHASE_REFUNDED:
				RegisterEventHandlers();
				break;
			case PurchaseStatus.PURCHASE_RESTORED:
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("shop_purchase_restored", "Product has been restored!"), "shop_purchase_restored", DispatchMessage.Status.Info);
				HandleOfferBought();
				break;
			case PurchaseStatus.PURCHASE_WAITING:
				DebugLog.Warn("Purchase Waiting!");
				break;
			}
		}
	}

	private void HandleInAppPurchase()
	{
		if (m_unavailable)
		{
			DebugLog.Error(GetType(), "Purchase " + m_product.name + " is not available");
			DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("confirm_purchase_unavailable", "In-App Purchases have been disallowed."), delegate
			{
			}, null);
			return;
		}
		DebugLog.Log(GetType(), "BuyOfferClicked: Is Within Limit");
		string productId = ((!m_validPremiumCostDiscount) ? m_product.productId : m_discountProduct.productId);
		DIContainerInfrastructure.PurchasingService.PurchaseProduct(productId);
		DeRegisterEventHandlers();
	}

	private void PreparePremiumOffer()
	{
		if (DIContainerInfrastructure.PurchasingService.IsInitializing() || !DIContainerInfrastructure.PurchasingService.IsInitialized())
		{
			m_unavailable = true;
			DebugLog.Warn(GetType(), string.Format("PreparePremiumOffer: Couldn't initialize purchase blind data, service is unavailable: IsInitializing()={0}, IsInitialized()={1}, IsEnabled={2}, IsSupported={3}", DIContainerInfrastructure.PurchasingService.IsInitializing(), DIContainerInfrastructure.PurchasingService.IsInitialized(), DIContainerInfrastructure.PurchasingService.IsEnabled(), DIContainerInfrastructure.PurchasingService.IsSupported()));
		}
		else
		{
			if (m_unavailable)
			{
				return;
			}
			List<Product> catalog = DIContainerInfrastructure.PurchasingService.GetCatalog();
			string productPaymentId = DIContainerBalancing.Service.GetBalancingData<ThirdPartyIdBalancingData>(m_model.NameId).PaymentProductId;
			string discountPaymentId = string.Empty;
			m_validPremiumCostDiscount = m_discountOffer && m_saleModel.SaleBalancing.ContentType == SaleContentType.LuckyCoinDiscount;
			if (m_validPremiumCostDiscount)
			{
				discountPaymentId = m_saleModel.OfferDetails.ReplacementProductId;
			}
			if (catalog != null && catalog.Any((Product p) => p.productId == productPaymentId))
			{
				RegisterExternalEventHandlers();
				m_product = catalog.FirstOrDefault((Product p) => p.productId == productPaymentId);
				if (!string.IsNullOrEmpty(discountPaymentId))
				{
					m_discountProduct = catalog.FirstOrDefault((Product p) => p.productId == discountPaymentId);
				}
			}
			else
			{
				m_unavailable = true;
				DebugLog.Error("Couldn't initialize blind; missing product data, id is: " + productPaymentId);
			}
		}
	}

	private void SetupPremiumOfferCostblind()
	{
		if (m_unavailable)
		{
			m_CostBlind.SetModel(string.Empty, null, DIContainerInfrastructure.GetLocaService().Tr("gen_lbl_purchaseunavailable", "Unavailable"), string.Empty);
			m_CostBlind.SetColor(DIContainerLogic.GetVisualEffectsBalancing().ColorOffersNotBuyable);
		}
		else
		{
			if (!m_CostBlind)
			{
				return;
			}
			m_CostBlind.gameObject.SetActive(true);
			List<Product> catalog = DIContainerInfrastructure.PurchasingService.GetCatalog();
			string productPaymentId = DIContainerBalancing.Service.GetBalancingData<ThirdPartyIdBalancingData>(m_model.NameId).PaymentProductId;
			Product product = default(Product);
			if (catalog != null && catalog.Any((Product p) => p.productId == productPaymentId))
			{
				product = catalog.FirstOrDefault((Product p) => p.productId == productPaymentId);
			}
			m_CostBlind.SetModel(string.Empty, null, product.price, string.Empty);
			m_CostBlind.CenterValue();
		}
	}
}
