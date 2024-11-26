using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class WorldMapShopMenuUI : MonoBehaviour
{
	private WorldMapStateMgr m_StateMgr;

	private ShopBalancingData m_Model;

	[SerializeField]
	private ContainerControl m_CategoryContentContainerController;

	[SerializeField]
	private UIGrid m_OfferListGrid;

	[SerializeField]
	private UILabel m_HeaderLabel;

	[SerializeField]
	private Animation m_BoardAnimation;

	[SerializeField]
	private Animation m_GroundAnimation;

	[SerializeField]
	private Animation m_BackButtonAnimation;

	[SerializeField]
	public UIInputTrigger m_BackButton;

	[SerializeField]
	private GameObject m_SkipTimer;

	[SerializeField]
	private GameObject m_EmptyLabel;

	[SerializeField]
	private Animation m_SkipButtonAnimation;

	[SerializeField]
	private UIInputTrigger m_SkipButton;

	[SerializeField]
	private ResourceCostBlind m_SkipCost;

	[SerializeField]
	private GameObject m_AnvilProp;

	[SerializeField]
	private UIInputTrigger m_AnvilPropButton;

	[SerializeField]
	private GameObject[] m_AnvilObjects;

	[SerializeField]
	private GameObject m_AlchemyProp;

	[SerializeField]
	private UIInputTrigger m_AlchemyPropButton;

	[SerializeField]
	private GameObject[] m_CauldronObjects;

	[SerializeField]
	private GameObject m_EagleProp;

	[SerializeField]
	private GameObject m_DiscountBubbleProp;

	[SerializeField]
	private UILabel m_DiscountPercentageLabel;

	[SerializeField]
	private UILabel m_StampCardAmountLabel;

	[SerializeField]
	private List<ShopOfferBlindWorldmap> m_ShopOfferBlinds = new List<ShopOfferBlindWorldmap>();

	[SerializeField]
	private UILabel m_TimerLabel;

	[SerializeField]
	[Header("DojoSale")]
	private GameObject m_SaleTimerObject;

	[SerializeField]
	private UILabel m_SaleTimerLabel;

	[SerializeField]
	private GameObject m_SaleBubbleProp;

	[SerializeField]
	private UILabel m_SaleBubbleLabel;

	public ShopMenuType m_MenuType;

	private bool m_NoOffersLeft;

	private bool m_EnteredOnce;

	private HotSpotWorldMapViewBase m_hotspot;

	private bool m_isMasterySale;

	[method: MethodImpl(32)]
	public event Action BackButtonPressed;

	private void Awake()
	{
		base.transform.position += DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.transform.position;
		m_AnvilProp.gameObject.SetActive(false);
	}

	private void HandleBackButton()
	{
		if (!DIContainerInfrastructure.GetCoreStateMgr().IsShopLoading())
		{
			DebugLog.Log("Pressed Back Button: " + GetType());
			DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
			Leave();
		}
	}

	public void SetModel(ShopBalancingData model, ShopMenuType menuType, HotSpotWorldMapViewBase hotspot)
	{
		m_Model = model;
		m_MenuType = menuType;
		m_hotspot = hotspot;
	}

	public void SetStateMgr(WorldMapStateMgr worldMapStateMgr)
	{
		m_StateMgr = worldMapStateMgr;
	}

	public void Enter()
	{
		if (m_Model != null)
		{
			m_HeaderLabel.text = DIContainerInfrastructure.GetLocaService().GetShopTypeName(m_MenuType);
			base.gameObject.SetActive(true);
			m_StateMgr.m_WorldMenuUI.Leave();
			StartCoroutine(EnterCoroutine());
		}
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter(false);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("worldmap_shop_animate");
		if (m_MenuType == ShopMenuType.Dojo && DIContainerLogic.GetTimingService().IsAfter(DIContainerLogic.GetTimingService().GetPresentTime().AddSeconds(DIContainerLogic.GetShopService().GetClassSwitchTimeLeft(DIContainerInfrastructure.GetCurrentPlayer()).TotalSeconds)))
		{
			ResetMasteryShopOffers();
		}
		m_EmptyLabel.SetActive(false);
		yield return StartCoroutine(RefreshBlinds());
		base.gameObject.GetComponent<UIPanel>().enabled = true;
		m_BoardAnimation.Play("Board_Enter");
		m_GroundAnimation.Play("Ground_Enter");
		m_BackButtonAnimation.Play("BackButton_Enter");
		GameObject[] anvilObjects = m_AnvilObjects;
		foreach (GameObject go in anvilObjects)
		{
			go.SetActive(false);
		}
		GameObject[] cauldronObjects = m_CauldronObjects;
		foreach (GameObject go2 in cauldronObjects)
		{
			go2.SetActive(false);
		}
		switch (m_MenuType)
		{
		case ShopMenuType.Workshop:
			m_AnvilProp.SetActive(true);
			m_AlchemyProp.SetActive(false);
			m_EagleProp.SetActive(false);
			m_DiscountBubbleProp.SetActive(false);
			m_SaleBubbleProp.SetActive(false);
			m_SaleTimerObject.gameObject.SetActive(false);
			HandleAnvil();
			break;
		case ShopMenuType.Witchhut:
			m_AnvilProp.SetActive(false);
			m_AlchemyProp.SetActive(true);
			m_EagleProp.SetActive(false);
			m_DiscountBubbleProp.SetActive(false);
			m_SaleBubbleProp.SetActive(false);
			m_SaleTimerObject.gameObject.SetActive(false);
			HandleCauldron();
			break;
		case ShopMenuType.Trainer:
			m_AnvilProp.SetActive(false);
			m_AlchemyProp.SetActive(false);
			m_SaleBubbleProp.SetActive(false);
			m_EagleProp.SetActive(true);
			m_EagleProp.gameObject.PlayAnimationOrAnimatorState("Ground_Enter");
			m_DiscountBubbleProp.SetActive(false);
			m_SaleTimerObject.gameObject.SetActive(false);
			if (!m_EnteredOnce && (bool)m_CategoryContentContainerController)
			{
				m_CategoryContentContainerController.UpdateLayoutChildren();
				m_EnteredOnce = true;
			}
			break;
		case ShopMenuType.Dojo:
			m_AnvilProp.SetActive(false);
			m_AlchemyProp.SetActive(false);
			m_EagleProp.SetActive(true);
			m_EagleProp.gameObject.PlayAnimationOrAnimatorState("Ground_Enter");
			m_SaleTimerObject.gameObject.SetActive(m_isMasterySale);
			m_DiscountBubbleProp.gameObject.SetActive(!m_isMasterySale);
			m_SaleBubbleProp.gameObject.SetActive(m_isMasterySale);
			m_DiscountPercentageLabel.text = "-" + GetDiscountPercentage() + "%";
			m_StampCardAmountLabel.text = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "daily_post_card").ToString();
			break;
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		m_SkipButtonAnimation.gameObject.SetActive(m_MenuType == ShopMenuType.Dojo);
		m_SkipTimer.gameObject.SetActive(m_MenuType == ShopMenuType.Dojo);
		if (!m_NoOffersLeft)
		{
			m_SkipTimer.GetComponent<Animation>().Play("BackButton_Enter");
			m_SkipButtonAnimation.Play("BackButton_Enter");
		}
		if (m_MenuType == ShopMenuType.Dojo)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 2u,
				showLuckyCoins = true,
				showSnoutlings = true
			}, true);
			Requirement cost = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").NextClassSkipRequirement;
			IInventoryItemBalancingData costItem = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(cost.NameId);
			m_SkipCost.SetModel(costItem.AssetBaseId, null, cost.Value, string.Empty);
			StopCoroutine("CountDownTimer");
			StartCoroutine("CountDownTimer", DIContainerLogic.GetTimingService().GetPresentTime().AddSeconds(DIContainerLogic.GetShopService().GetClassSwitchTimeLeft(DIContainerInfrastructure.GetCurrentPlayer()).TotalSeconds));
		}
		yield return new WaitForSeconds(m_BoardAnimation["Board_Enter"].clip.length);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("worldmap_shop_animate");
		RegisterEventHandlers();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("entered_workshop", m_Model.NameId);
	}

	private void HandleAnvil()
	{
		IInventoryItemGameData data;
		DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "forge_leveled", out data);
		for (int i = 0; i < 3; i++)
		{
			m_AnvilObjects[i].SetActive(i + 1 == data.ItemData.Level);
		}
	}

	private void HandleCauldron()
	{
		IInventoryItemGameData data;
		DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "cauldron_leveled", out data);
		for (int i = 0; i < 3; i++)
		{
			m_CauldronObjects[i].SetActive(i + 1 == data.ItemData.Level);
		}
	}

	private float GetDiscountPercentage()
	{
		int itemValue = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "daily_post_card");
		float num = 0f;
		float num2 = 0f;
		int[] array = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").DojoOfferDiscountThresholds.ToArray();
		float[] array2 = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").DojoOfferDiscount.ToArray();
		for (int i = 0; i < 5; i++)
		{
			if (i == 0)
			{
				num = (float)Mathf.Min(itemValue, array[0]) * array2[0];
				num2 = num;
			}
			else
			{
				num = num2 + (float)(Mathf.Min(itemValue, array[i]) - array[i - 1]) * array2[i];
				num2 = num;
			}
			if (itemValue <= array[i])
			{
				break;
			}
		}
		return Mathf.Min(num, 30f);
	}

	private IEnumerator RefreshBlinds()
	{
		ClearBlinds();
		yield return new WaitForEndOfFrame();
		List<BasicShopOfferBalancingData> offers = new List<BasicShopOfferBalancingData>();
		offers = ((m_MenuType != ShopMenuType.Dojo) ? DIContainerLogic.GetShopService().GetShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), m_Model.NameId) : DIContainerLogic.GetShopService().GetShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), m_Model.NameId, false));
		offers = offers.OrderBy((BasicShopOfferBalancingData o) => o.SlotId).ToList();
		foreach (BasicShopOfferBalancingData shopOfferBalancingData in offers)
		{
			bool isNew = false;
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.ShopOffersNew.TryGetValue(shopOfferBalancingData.NameId, out isNew) && isNew)
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.ShopOffersNew[shopOfferBalancingData.NameId] = false;
				break;
			}
			DebugLog.Log("Class Upgrade Offers: " + shopOfferBalancingData.NameId);
		}
		if (offers.Count == 0 && !m_NoOffersLeft)
		{
			m_SkipButtonAnimation.Play("BackButton_Leave");
			m_SkipTimer.GetComponent<Animation>().Play("BackButton_Leave");
			m_NoOffersLeft = true;
		}
		else if (offers.Count > 0 && m_NoOffersLeft)
		{
			m_NoOffersLeft = false;
		}
		if (m_MenuType == ShopMenuType.Dojo)
		{
			HandleMasteryOffers(offers);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}
		else
		{
			HandleItemOffers(offers);
		}
		yield return new WaitForEndOfFrame();
		m_OfferListGrid.Reposition();
	}

	private void HandleMasteryOffers(List<BasicShopOfferBalancingData> offers)
	{
		int j;
		for (j = 0; j < m_ShopOfferBlinds.Count; j++)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers == null)
			{
				break;
			}
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers.Count == 0)
			{
				break;
			}
			BasicShopOfferBalancingData basicShopOfferBalancingData = offers.FirstOrDefault((BasicShopOfferBalancingData o) => o.NameId == DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers[j]);
			if (basicShopOfferBalancingData == null)
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers.Add(string.Empty);
			}
		}
		m_isMasterySale = IsMasterySale(offers);
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers == null || DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers.Count != m_ShopOfferBlinds.Count || DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers.Contains(string.Empty))
		{
			offers.Shuffle();
			for (int k = DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers.Count; k < m_ShopOfferBlinds.Count; k++)
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers.Add(string.Empty);
			}
			List<string> list = (from o in offers
				where !DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers.Contains(o.NameId)
				select o.NameId).ToList();
			int num = m_ShopOfferBlinds.Count;
			if (m_isMasterySale)
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers[m_ShopOfferBlinds.Count - 1] = "offer_mastery_all";
				list.Remove("offer_mastery_all");
				num--;
			}
			for (int l = 0; l < num; l++)
			{
				string offerName = DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers[l];
				if (offerName == string.Empty || offers.Where((BasicShopOfferBalancingData o) => o.NameId == offerName).ToList().Count == 0)
				{
					int num2 = UnityEngine.Random.Range(0, 100);
					WorldBalancingData balancingData = DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData;
					int num3 = DIContainerInfrastructure.GetCurrentPlayer().Data.DojoOffersBought * (int)balancingData.MasteryChancePlus;
					int num4 = Mathf.Min((int)balancingData.AllBirdsMasteryChance + num3, (int)balancingData.MasteryChanceBonusCap);
					int num5 = num4 + Mathf.Min((int)balancingData.SingleBirdMasteryChance + num3, (int)balancingData.MasteryChanceBonusCap);
					string text = ((num2 < num4 && list.Any((string o) => o == "offer_mastery_all")) ? "offer_mastery_all" : ((num2 >= num5 || !list.Any((string o) => o.StartsWith("offer_mastery_bird_"))) ? list.Where((string o) => o.StartsWith("offer_mastery_class_")).FirstOrDefault() : list.Where((string o) => o.StartsWith("offer_mastery_bird_")).FirstOrDefault()));
					if (!string.IsNullOrEmpty(text))
					{
						DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers[l] = text;
						list.Remove(text);
					}
				}
			}
		}
		int num6 = 0;
		int i;
		for (i = 0; i < m_ShopOfferBlinds.Count; i++)
		{
			ShopOfferBlindWorldmap shopOfferBlindWorldmap = m_ShopOfferBlinds[i];
			if (Mathf.Min(m_Model.Slots, offers.Count) <= i || string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers[i]))
			{
				StartCoroutine(shopOfferBlindWorldmap.Disable(shopOfferBlindWorldmap.HideInstant()));
				continue;
			}
			BuyableShopOfferBalancingData balancing = offers.FirstOrDefault((BasicShopOfferBalancingData o) => o.NameId == DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers[i]) as BuyableShopOfferBalancingData;
			if (!DIContainerBalancing.Service.TryGetBalancingData<BuyableShopOfferBalancingData>(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers[i], out balancing))
			{
				StartCoroutine(shopOfferBlindWorldmap.Disable(shopOfferBlindWorldmap.HideInstant()));
				continue;
			}
			bool applyDojoMasteryBonus = false;
			float bonusFactor = 1f;
			BonusEventBalancingData currentValidBalancing = DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing;
			if (currentValidBalancing != null && currentValidBalancing.BonusType == BonusEventType.MasteryBonus)
			{
				applyDojoMasteryBonus = true;
				bonusFactor = currentValidBalancing.BonusFactor;
			}
			BuyableShopOfferBalancingData model = CopyAndDiscount(balancing, applyDojoMasteryBonus, bonusFactor);
			num6++;
			shopOfferBlindWorldmap.gameObject.SetActive(true);
			shopOfferBlindWorldmap.SetModel(model, m_StateMgr, applyDojoMasteryBonus);
			shopOfferBlindWorldmap.Show();
		}
		if (m_isMasterySale)
		{
			BasicShopOfferBalancingData basicShopOfferBalancingData2 = offers.Where((BasicShopOfferBalancingData a) => !string.IsNullOrEmpty(a.SpeechBubbleLoca)).FirstOrDefault();
			m_SaleBubbleLabel.text = DIContainerInfrastructure.GetLocaService().Tr(basicShopOfferBalancingData2.SpeechBubbleLoca);
			SalesManagerBalancingData activeSaleForOffer = DIContainerLogic.GetSalesManagerService().GetActiveSaleForOffer(basicShopOfferBalancingData2);
			StartCoroutine(SaleTimer(DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(activeSaleForOffer.EndTime)));
		}
		m_EmptyLabel.SetActive(num6 == 0);
	}

	private bool IsMasterySale(List<BasicShopOfferBalancingData> offers)
	{
		foreach (BasicShopOfferBalancingData offer in offers)
		{
			if (offer.BuyRequirements.Where((Requirement r) => r.NameId == "lucky_coin").Count() > 0 && DIContainerLogic.GetShopService().IsDiscountValid(offer))
			{
				return true;
			}
		}
		return false;
	}

	private BuyableShopOfferBalancingData CopyAndDiscount(BuyableShopOfferBalancingData data, bool applyDojoMasteryBonus, float bonusFactor)
	{
		BuyableShopOfferBalancingData buyableShopOfferBalancingData = new BuyableShopOfferBalancingData();
		BuyableShopOfferBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<BuyableShopOfferBalancingData>(data.NameId);
		List<Requirement> list = new List<Requirement>();
		bool flag = false;
		SaleOfferTupel saleOffer;
		foreach (Requirement buyRequirement in data.BuyRequirements)
		{
			if (buyRequirement.RequirementType == RequirementType.PayItem && buyRequirement.NameId == "gold")
			{
				Requirement requirement = new Requirement();
				if (DIContainerLogic.GetShopService().GetActiveSaleDetailsForOffer(data.NameId, out saleOffer) && saleOffer.OfferDetails.SaleParameter == SaleParameter.Price)
				{
					int changedValue = saleOffer.OfferDetails.ChangedValue;
					buyableShopOfferBalancingData.DiscountPrice = (int)((float)changedValue - (float)changedValue * (GetDiscountPercentage() / 100f));
				}
				else
				{
					requirement.Value = buyRequirement.Value - buyRequirement.Value * (GetDiscountPercentage() / 100f);
				}
				requirement.RequirementType = RequirementType.PayItem;
				requirement.NameId = "gold";
				list.Add(requirement);
				flag = true;
				foreach (Requirement buyRequirement2 in balancingData.BuyRequirements)
				{
					if (buyRequirement2.RequirementType == RequirementType.PayItem && buyRequirement2.NameId == "gold" && buyRequirement2.Value > buyRequirement.Value)
					{
						return data;
					}
				}
			}
			else
			{
				list.Add(buyRequirement);
			}
		}
		if (!flag && DIContainerLogic.GetShopService().GetActiveSaleDetailsForOffer(data.NameId, out saleOffer) && saleOffer.OfferDetails.SaleParameter == SaleParameter.Price)
		{
			buyableShopOfferBalancingData.DiscountPrice = saleOffer.OfferDetails.ChangedValue;
		}
		buyableShopOfferBalancingData.AssetId = data.AssetId;
		buyableShopOfferBalancingData.BuyRequirements = list;
		buyableShopOfferBalancingData.Category = data.Category;
		buyableShopOfferBalancingData.DiscountEndDate = data.DiscountEndDate;
		buyableShopOfferBalancingData.DiscountStartDate = data.DiscountStartDate;
		buyableShopOfferBalancingData.SpecialOfferLabelColor = data.SpecialOfferLabelColor;
		buyableShopOfferBalancingData.SpecialOfferBackgroundColor = data.SpecialOfferBackgroundColor;
		buyableShopOfferBalancingData.DiscountCooldown = data.DiscountCooldown;
		buyableShopOfferBalancingData.DiscountDuration = data.DiscountDuration;
		buyableShopOfferBalancingData.DiscountRequirements = data.DiscountRequirements;
		buyableShopOfferBalancingData.Level = data.Level;
		buyableShopOfferBalancingData.LocaId = data.LocaId;
		buyableShopOfferBalancingData.NameId = data.NameId;
		if (applyDojoMasteryBonus)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (KeyValuePair<string, int> offerContent in data.OfferContents)
			{
				dictionary.Add(offerContent.Key, offerContent.Value + (int)((float)offerContent.Value * (100f / bonusFactor)));
			}
			buyableShopOfferBalancingData.OfferContents = dictionary;
		}
		else
		{
			buyableShopOfferBalancingData.OfferContents = data.OfferContents;
		}
		buyableShopOfferBalancingData.ShowRequirements = data.ShowRequirements;
		buyableShopOfferBalancingData.SlotId = data.SlotId;
		buyableShopOfferBalancingData.SortPriority = data.SortPriority;
		buyableShopOfferBalancingData.UniqueOffer = data.UniqueOffer;
		return buyableShopOfferBalancingData;
	}

	private void HandleItemOffers(List<BasicShopOfferBalancingData> offers)
	{
		for (int i = 0; i < m_ShopOfferBlinds.Count; i++)
		{
			ShopOfferBlindWorldmap shopOfferBlindWorldmap = m_ShopOfferBlinds[i];
			if (Mathf.Min(m_Model.Slots, offers.Count) <= i)
			{
				StartCoroutine(shopOfferBlindWorldmap.Disable(shopOfferBlindWorldmap.HideInstant()));
				continue;
			}
			BasicShopOfferBalancingData basicShopOfferBalancingData = offers[i];
			if (basicShopOfferBalancingData != null)
			{
				shopOfferBlindWorldmap.gameObject.SetActive(true);
				shopOfferBlindWorldmap.SetModel(basicShopOfferBalancingData, m_StateMgr);
				shopOfferBlindWorldmap.Show();
			}
		}
	}

	private void ClearBlinds()
	{
		for (int num = m_ShopOfferBlinds.Count - 1; num >= 0; num--)
		{
			ShopOfferBlindWorldmap slot = m_ShopOfferBlinds[num];
			DeRegisterEventHandlerFromSlot(slot);
		}
	}

	private void DeRegisterEventHandlerFromSlot(ShopOfferBlindWorldmap slot)
	{
		slot.ShopOfferBought -= blind_ShopOfferBought;
	}

	private void RegisterEventHandlerFromSlot(ShopOfferBlindWorldmap slot)
	{
		DeRegisterEventHandlerFromSlot(slot);
		slot.ShopOfferBought += blind_ShopOfferBought;
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("worldmap_shop_animate");
		DeRegisterEventHandlers();
		m_BoardAnimation.Play("Board_Leave");
		m_GroundAnimation.Play("Ground_Leave");
		if (m_EagleProp.activeInHierarchy)
		{
			m_EagleProp.gameObject.PlayAnimationOrAnimatorState("Ground_Leave");
		}
		m_BackButtonAnimation.Play("BackButton_Leave");
		if (!m_NoOffersLeft)
		{
			m_SkipButtonAnimation.Play("BackButton_Leave");
			m_SkipTimer.GetComponent<Animation>().Play("BackButton_Leave");
		}
		yield return new WaitForSeconds(m_BoardAnimation["Board_Leave"].length);
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
		foreach (ShopOfferBlindWorldmap blind in m_ShopOfferBlinds)
		{
			blind.gameObject.SetActive(false);
		}
		m_StateMgr.m_WorldMenuUI.Enter();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("worldmap_shop_animate");
		base.gameObject.SetActive(false);
		if (m_hotspot is HotSpotWorldMapViewShopNode)
		{
			HotSpotWorldMapViewShopNode shop = m_hotspot as HotSpotWorldMapViewShopNode;
			shop.CheckForNewMarker();
		}
		if (m_MenuType == ShopMenuType.Dojo)
		{
			DIContainerInfrastructure.LocationStateMgr.ProcessRankUpPopUp();
		}
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		if ((bool)m_BackButton)
		{
			m_BackButton.Clicked += m_BackButton_Clicked;
		}
		if ((bool)m_SkipButton)
		{
			m_SkipButton.Clicked += m_SkipButton_Clicked;
		}
		if ((bool)m_AnvilPropButton)
		{
			m_AnvilPropButton.Clicked += m_AnvilPropButton_Clicked;
		}
		if ((bool)m_AlchemyPropButton)
		{
			m_AlchemyPropButton.Clicked += m_AlchemyPropButton_Clicked;
		}
		foreach (ShopOfferBlindWorldmap shopOfferBlind in m_ShopOfferBlinds)
		{
			RegisterEventHandlerFromSlot(shopOfferBlind);
		}
	}

	private void m_AlchemyPropButton_Clicked()
	{
		DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreenViaHotlink("story_cauldron");
		DeRegisterEventHandlers();
	}

	private void m_AnvilPropButton_Clicked()
	{
		DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreenViaHotlink("story_forge");
		DeRegisterEventHandlers();
	}

	private void m_SkipButton_Clicked()
	{
		if (DIContainerLogic.RequirementService.ExecuteRequirements(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, new List<Requirement> { DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").NextClassSkipRequirement }, "skip_class_upgrade_time"))
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.LastClassSwitchTime = 0u;
			ResetMasteryShopOffers();
			RefreshAndReEnter();
			return;
		}
		Requirement nextClassSkipRequirement = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").NextClassSkipRequirement;
		if (nextClassSkipRequirement != null && nextClassSkipRequirement.RequirementType == RequirementType.PayItem)
		{
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, nextClassSkipRequirement.NameId, out data))
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_MissingCurrencyPopup.EnterPopup(data.ItemBalancing.NameId, nextClassSkipRequirement.Value);
			}
		}
	}

	private void blind_ShopOfferBought(BasicShopOfferBalancingData blind)
	{
		StartCoroutine(RefreshAndReEnterCoroutine(blind));
	}

	public void RefreshAndReEnter()
	{
		StartCoroutine(RefreshAndReEnterCoroutine(null));
	}

	private IEnumerator RefreshAndReEnterCoroutine(BasicShopOfferBalancingData blind)
	{
		if (m_MenuType != ShopMenuType.Dojo)
		{
			DeRegisterEventHandlers();
		}
		if (blind != null)
		{
			ShopOfferBlindWorldmap boughtBlind = m_ShopOfferBlinds.FirstOrDefault((ShopOfferBlindWorldmap b) => b.GetModel() != null && b.GetModel().NameId == blind.NameId);
			if (boughtBlind != null)
			{
				boughtBlind.Hide();
				DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
				yield return new WaitForSeconds(boughtBlind.ShowBoughtIndicator());
			}
		}
		yield return StartCoroutine(RefreshBlinds());
		if (m_MenuType == ShopMenuType.Dojo)
		{
			Requirement cost = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").NextClassSkipRequirement;
			IInventoryItemBalancingData costItem = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(cost.NameId);
			m_SkipCost.SetModel(costItem.AssetBaseId, null, cost.Value, string.Empty);
			StopCoroutine("CountDownTimer");
			StartCoroutine("CountDownTimer", DIContainerLogic.GetTimingService().GetPresentTime().AddSeconds(DIContainerLogic.GetShopService().GetClassSwitchTimeLeft(DIContainerInfrastructure.GetCurrentPlayer()).TotalSeconds));
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateCoinsBar();
		RegisterEventHandlers();
		yield return new WaitForEndOfFrame();
		m_OfferListGrid.Reposition();
	}

	private void ResetMasteryShopOffers()
	{
		if (m_MenuType == ShopMenuType.Dojo)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.DojoOffersBought = 0;
			DIContainerInfrastructure.GetCurrentPlayer().Data.LastClassSwitchTime = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.ClearInventoryOfType(InventoryItemType.Mastery);
			DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentClassUpgradeShopOffers.Clear();
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}
	}

	private void m_BackButton_Clicked()
	{
		if (this.BackButtonPressed != null)
		{
			this.BackButtonPressed();
		}
		Leave();
	}

	private IEnumerator SaleTimer(DateTime targetTime)
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_SaleTimerLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft);
			}
			yield return new WaitForSeconds(1f);
		}
		ResetMasteryShopOffers();
		m_StateMgr.m_WorkShopUI.RefreshAndReEnter();
	}

	private IEnumerator CountDownTimer(DateTime targetTime)
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_TimerLabel.text = DIContainerInfrastructure.GetLocaService().Tr("lbl_timer_classupgrade", new Dictionary<string, string> { 
				{
					"{value_1}",
					DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft)
				} });
			}
			yield return new WaitForSeconds(1f);
		}
		ResetMasteryShopOffers();
		m_StateMgr.m_WorkShopUI.RefreshAndReEnter();
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		if ((bool)m_BackButton)
		{
			m_BackButton.Clicked -= m_BackButton_Clicked;
		}
		if ((bool)m_SkipButton)
		{
			m_SkipButton.Clicked -= m_SkipButton_Clicked;
		}
		if ((bool)m_AnvilPropButton)
		{
			m_AnvilPropButton.Clicked -= m_AnvilPropButton_Clicked;
		}
		if ((bool)m_AlchemyPropButton)
		{
			m_AlchemyPropButton.Clicked -= m_AlchemyPropButton_Clicked;
		}
		foreach (ShopOfferBlindWorldmap shopOfferBlind in m_ShopOfferBlinds)
		{
			DeRegisterEventHandlerFromSlot(shopOfferBlind);
		}
	}

	private void OnDisable()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("worldmap_shop_animate");
	}
}
