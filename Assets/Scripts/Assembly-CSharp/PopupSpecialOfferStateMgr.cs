using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using SmoothMoves;
using UnityEngine;

public class PopupSpecialOfferStateMgr : MonoBehaviour
{
	[SerializeField]
	private UILabel m_OfferNameLabel;

	[SerializeField]
	private UILabel m_OfferDescLabel;

	[SerializeField]
	private UILabel m_DiscountLabel;

	[SerializeField]
	private UILabel m_DiscountOnButtonLabel;

	[SerializeField]
	private UILabel m_TimerLabel;

	[SerializeField]
	public UIInputTrigger m_GetItButton;

	[SerializeField]
	private UIInputTrigger m_AbortButton;

	[SerializeField]
	private GameObject m_GetItButtonRoot;

	[SerializeField]
	private GameObject m_SpecialOfferRoot;

	[SerializeField]
	private GameObject m_DiscountOnButtonRoot;

	[SerializeField]
	private GameObject m_StarCollectionRoot;

	[SerializeField]
	private GameObject m_OfferTimerRoot;

	[SerializeField]
	private GameObject m_StandardLabelRoot;

	[SerializeField]
	private UILabel m_StarCollectionValue;

	[SerializeField]
	private UILabel m_StarCollectionSubHeader;

	[SerializeField]
	private UISprite m_ResourcesOfferSprite;

	[SerializeField]
	private GameObject m_gachaRainbowRiotHand;

	[SerializeField]
	private Transform m_ItemRoot;

	[SerializeField]
	private Transform m_CenteredItemRoot;

	[SerializeField]
	private GameObject m_SpecificClassBundleA;

	[SerializeField]
	private GameObject m_SpecificClassBundleB;

	[SerializeField]
	private GameObject m_MasteryPrefab;

	[SerializeField]
	private GameObject m_StandardBackground;

	[SerializeField]
	private GameObject m_BlueBackground;

	private WWW m_OpponentTextureDownload;

	[SerializeField]
	private UILabel m_SpecialOfferTopLabel;

	[SerializeField]
	private GameObject m_DiscountOptionA;

	[SerializeField]
	private GameObject m_DiscountOptionB;

	[SerializeField]
	private UILabel m_DiscountOldPriceLabel;

	[SerializeField]
	private UILabel m_DiscountNewPriceLabel;

	private List<GameObject> m_FeatureObject = new List<GameObject>();

	private List<string> m_FeatureObjectNameId = new List<string>();

	private BasicItemGameData m_BasicItemGameData;

	[SerializeField]
	private float m_MaximumShowTime = 4.5f;

	private FeatureObjectType m_ObjectType;

	private WaitTimeOrAbort m_AsyncOperation;

	public bool m_IsShowing;

	private bool m_IsRainbowRiot;

	private bool m_IsStarCollection;

	private bool m_IsEventCampaignReward;

	private bool m_IsEnergyTutorial;

	private bool m_ShopOffer;

	private bool m_IsStampCard;

	private bool m_WorldBossPopup;

	private string m_shopCategory = "shop_premium";

	private float m_lastcollectedPopup;

	[SerializeField]
	private CharacterControllerCamp m_CampCharacterControllerPrefab;

	private bool m_IsHint;

	private string m_HintDestination;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup = this;
	}

	private void SetDiscountLabel(BasicShopOfferBalancingData offer)
	{
	}

	private void SetDiscountLabel(SalesManagerBalancingData sale, bool isExclusive)
	{
		SaleItemDetails saleItemDetails = sale.SaleDetails.FirstOrDefault();
		if (saleItemDetails == null)
		{
			DebugLog.Error(GetType(), string.Format("SetDiscountLabel: No offer found in sale {0}", sale.NameId));
			return;
		}
		BasicShopOfferBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<BuyableShopOfferBalancingData>(saleItemDetails.SubjectId);
		if (balancingData == null)
		{
			balancingData = DIContainerBalancing.Service.GetBalancingData<PremiumShopOfferBalancingData>(saleItemDetails.SubjectId);
		}
		if (balancingData == null)
		{
			DebugLog.Error(GetType(), string.Format("SetDiscountLabel: No Balancing found for offer \"{0}\"", saleItemDetails.SubjectId));
			return;
		}
		Requirement requirement = DIContainerLogic.GetShopService().GetBuyResourcesRequirements(1, balancingData, false).FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem);
		if (isExclusive && balancingData is BuyableShopOfferBalancingData)
		{
			m_DiscountNewPriceLabel.text = saleItemDetails.ChangedValue.ToString();
			m_DiscountOldPriceLabel.text = balancingData.BuyRequirements.FirstOrDefault().Value.ToString();
		}
		else if (balancingData is BuyableShopOfferBalancingData && requirement != null && saleItemDetails.SaleParameter == SaleParameter.Price)
		{
			m_DiscountLabel.text = "-" + Mathf.RoundToInt(100f - (float)saleItemDetails.ChangedValue * 100f / requirement.Value).ToString("0") + "%";
			m_DiscountLabel.color = new Color(1f, 0f, 0f);
		}
		else if (saleItemDetails.SaleParameter == SaleParameter.Value)
		{
			int value = balancingData.OfferContents.FirstOrDefault().Value;
			m_DiscountLabel.text = "+" + Mathf.RoundToInt((float)saleItemDetails.ChangedValue * 100f / (float)value - 100f).ToString("0") + "%";
			m_DiscountLabel.color = new Color(0f, 1f, 0f);
		}
	}

	public WaitTimeOrAbort ShowSpecialOfferPopup(BasicItemGameData basicItem)
	{
		m_IsShowing = true;
		m_StandardBackground.SetActive(true);
		m_BlueBackground.SetActive(false);
		m_StarCollectionRoot.SetActive(false);
		if (basicItem == null || (!basicItem.BalancingData.NameId.StartsWith("special_") && !basicItem.BalancingData.NameId.StartsWith("star_popup_") && !basicItem.BalancingData.NameId.StartsWith("collection_reward") && !basicItem.BalancingData.NameId.StartsWith("event_ener") && !basicItem.BalancingData.NameId.StartsWith("daily_post")))
		{
			m_IsShowing = false;
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
			m_AsyncOperation.Abort();
			return m_AsyncOperation;
		}
		ResetPopup(basicItem);
		if (m_BasicItemGameData.ItemBalancing.NameId.StartsWith("special_offer_rainbow_riot"))
		{
			m_IsRainbowRiot = true;
			base.gameObject.SetActive(true);
			m_SpecialOfferRoot.SetActive(false);
			StartCoroutine(CountDownTime(DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData.RainbowRiotTime));
			StartCoroutine("EnterCoroutine");
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 5u,
				showFriendshipEssence = false,
				showLuckyCoins = true,
				showSnoutlings = false
			}, true);
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
			return m_AsyncOperation;
		}
		if (m_BasicItemGameData.ItemBalancing.NameId.StartsWith("special_event_"))
		{
			base.gameObject.SetActive(true);
			m_SpecialOfferRoot.SetActive(true);
			m_OfferTimerRoot.SetActive(true);
			m_DiscountOnButtonRoot.SetActive(true);
			m_shopCategory = "shop_premium";
			m_DiscountOnButtonLabel.text = "-" + DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData.GlobalEventDiscount.ToString("0") + "%";
			StartCoroutine(CountDownTime((float)DIContainerLogic.GetTimingService().TimeLeftUntil(DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData.DailyChainTimerUntilTimestamp)).TotalSeconds));
			StartCoroutine("EnterCoroutine");
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 5u,
				showFriendshipEssence = false,
				showLuckyCoins = true,
				showSnoutlings = false
			}, true);
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
			return m_AsyncOperation;
		}
		if (m_BasicItemGameData.ItemBalancing.NameId.StartsWith("star_popup_"))
		{
			base.gameObject.SetActive(true);
			m_SpecialOfferRoot.SetActive(false);
			m_DiscountOnButtonRoot.SetActive(false);
			m_IsStarCollection = true;
			m_StarCollectionRoot.SetActive(true);
			m_StandardLabelRoot.SetActive(false);
			StartCoroutine("EnterCoroutine");
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 5u,
				showFriendshipEssence = false,
				showLuckyCoins = false,
				showSnoutlings = false
			}, true);
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		}
		else if (m_BasicItemGameData.ItemBalancing.NameId.StartsWith("daily_post"))
		{
			base.gameObject.SetActive(true);
			m_SpecialOfferRoot.SetActive(false);
			m_DiscountOnButtonRoot.SetActive(false);
			m_StandardLabelRoot.SetActive(true);
			m_OfferTimerRoot.SetActive(false);
			m_IsStampCard = true;
			StartCoroutine("EnterCoroutine");
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 5u,
				showFriendshipEssence = false,
				showLuckyCoins = false,
				showSnoutlings = false
			}, true);
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		}
		else if (m_BasicItemGameData.ItemBalancing.NameId.StartsWith("collection_reward"))
		{
			base.gameObject.SetActive(true);
			m_DiscountOnButtonRoot.SetActive(false);
			m_SpecialOfferRoot.SetActive(false);
			m_StandardLabelRoot.SetActive(true);
			m_OfferTimerRoot.SetActive(false);
			m_IsEventCampaignReward = true;
			StartCoroutine("EnterCoroutine");
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 5u,
				showFriendshipEssence = false,
				showLuckyCoins = false,
				showSnoutlings = false
			}, true);
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		}
		else if (m_BasicItemGameData.ItemBalancing.NameId.StartsWith("event_ener"))
		{
			base.gameObject.SetActive(true);
			m_SpecialOfferRoot.SetActive(false);
			m_OfferTimerRoot.SetActive(false);
			m_DiscountOnButtonRoot.SetActive(false);
			StartCoroutine("EnterCoroutine");
			m_IsEnergyTutorial = true;
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 5u,
				showFriendshipEssence = false,
				showLuckyCoins = false,
				showSnoutlings = false
			}, true);
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
			return m_AsyncOperation;
		}
		BasicShopOfferBalancingData specialOffer = null;
		foreach (BasicShopOfferBalancingData allShopOffer in DIContainerLogic.GetShopService().GetAllShopOffers())
		{
			if (allShopOffer.DiscountRequirements == null || !allShopOffer.DiscountRequirements.Any((Requirement r) => r.RequirementType == RequirementType.HaveItem && r.NameId == basicItem.BalancingData.NameId))
			{
				continue;
			}
			specialOffer = allShopOffer;
			break;
		}
		if (specialOffer == null)
		{
			m_IsShowing = false;
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
			return m_AsyncOperation;
		}
		m_SpecialOfferRoot.SetActive(true);
		m_DiscountOptionA.SetActive(true);
		m_DiscountOptionB.SetActive(false);
		ShopBalancingData shopBalancingData = DIContainerBalancing.Service.GetBalancingDataList<ShopBalancingData>().FirstOrDefault((ShopBalancingData shop) => shop.Categories.Any((string c) => c == specialOffer.Category));
		if (shopBalancingData != null)
		{
			m_shopCategory = shopBalancingData.NameId;
		}
		SetDiscountLabel(specialOffer);
		base.gameObject.SetActive(true);
		StartCoroutine(CountDownTime(specialOffer.DiscountDuration));
		StartCoroutine("EnterCoroutine");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, true);
		m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		return m_AsyncOperation;
	}

	private void ResetPopup(BasicItemGameData basicItem)
	{
		m_ResourcesOfferSprite.gameObject.SetActive(false);
		m_DiscountOnButtonRoot.SetActive(false);
		m_DiscountOptionA.SetActive(false);
		m_DiscountOptionB.SetActive(false);
		m_StarCollectionRoot.SetActive(false);
		if (m_FeatureObject != null)
		{
			m_FeatureObject.Clear();
		}
		if (m_FeatureObjectNameId != null)
		{
			m_FeatureObjectNameId.Clear();
		}
		m_BasicItemGameData = basicItem;
	}

	public WaitTimeOrAbort ShowSpecialOfferPopup(BasicItemGameData basicItem, string destination)
	{
		ResetPopup(basicItem);
		base.gameObject.SetActive(true);
		m_DiscountOnButtonRoot.SetActive(false);
		m_StarCollectionRoot.SetActive(false);
		m_SpecialOfferRoot.SetActive(false);
		m_StandardLabelRoot.SetActive(true);
		m_OfferTimerRoot.SetActive(false);
		m_StandardBackground.SetActive(true);
		m_BlueBackground.SetActive(false);
		m_IsHint = true;
		m_HintDestination = destination;
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, true);
		if (m_BasicItemGameData.BalancingData.NameId.StartsWith("hint_"))
		{
			m_StandardBackground.SetActive(false);
			m_BlueBackground.SetActive(true);
			StartCoroutine("EnterCoroutine");
		}
		m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		m_IsShowing = false;
		return m_AsyncOperation;
	}

	private IEnumerator EnterCoroutineWithoutInstantiate()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_unlock_enter");
		SetDragControllerActive(false);
		GetComponent<Animation>().Play("Popup_SpecialOffer_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_SpecialOffer_Enter"].length);
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_unlock_enter");
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_unlock_enter");
		SetDragControllerActive(false);
		m_FeatureObject = InstantiateFeatureObject(m_BasicItemGameData, m_CenteredItemRoot, m_ItemRoot);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u
		}, true);
		GetComponent<Animation>().Play("Popup_SpecialOffer_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_SpecialOffer_Enter"].length);
		if (m_BasicItemGameData.ItemBalancing.NameId.StartsWith("special_offer_rainbow_riot"))
		{
			DebugLog.Log("Let Golden Pig Rioting!");
			if (m_FeatureObject.Count > 0)
			{
				GameObject pigHand = UnityEngine.Object.Instantiate(m_gachaRainbowRiotHand);
				pigHand.transform.parent = m_FeatureObject.FirstOrDefault().transform.Find("Root/Body");
				pigHand.transform.localScale = Vector3.one;
				UISprite sprite = pigHand.transform.Find("Animation/Hand").GetComponent<UISprite>();
				pigHand.SetActive(false);
				yield return new WaitForSeconds(0.3f);
				pigHand.SetActive(true);
				pigHand.GetComponent<Animation>().Play("RainbowRiotMarker_Enter");
				pigHand.transform.localPosition = new Vector3(-10f, 30f, 3f);
				if (DIContainerInfrastructure.GetCurrentPlayer().Data.IsExtraRainbowRiot)
				{
					sprite.spriteName = "Hand_RainbowRiotB";
					pigHand.GetComponentInChildren<UILabel>().text = DIContainerInfrastructure.GetLocaService().Tr("rainbowriot_hand_desc").Replace("{value_1}", DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot2Multi.ToString());
				}
				else
				{
					sprite.spriteName = "Hand_RainbowRiotA";
					pigHand.GetComponentInChildren<UILabel>().text = DIContainerInfrastructure.GetLocaService().Tr("rainbowriot_hand_desc").Replace("{value_1}", DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot1Multi.ToString());
				}
				GameObject firstOrDefault = m_FeatureObject.FirstOrDefault();
				if (firstOrDefault != null)
				{
					BoneAnimation boneAnim = firstOrDefault.GetComponent<BoneAnimation>();
					if ((bool)boneAnim)
					{
						DebugLog.Log("Let Golden start Pig Rioting!");
						boneAnim.Play("RainbowRiot");
					}
				}
			}
		}
		if (m_FeatureObjectNameId.FirstOrDefault() != null)
		{
			DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("special_popup_entered", m_FeatureObjectNameId.FirstOrDefault());
		}
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_unlock_enter");
	}

	private List<GameObject> InstantiateFeatureObject(BasicItemGameData basicItemGameData, Transform centerTransform, Transform rooTransform)
	{
		m_ObjectType = FeatureObjectType.CampPropUnlock;
		if (basicItemGameData.ItemBalancing.NameId.StartsWith("event_energy"))
		{
			m_OfferNameLabel.text = DIContainerInfrastructure.GetLocaService().GetItemName("potion_energy_01");
			m_OfferDescLabel.text = DIContainerInfrastructure.GetLocaService().Tr("tutorial_missing_energy");
			List<GameObject> list = new List<GameObject>();
			list.Add(CreateFeatureObjectAsset("EnergyDrinks", basicItemGameData, centerTransform, rooTransform));
			return list;
		}
		if (basicItemGameData.ItemBalancing.ItemType != InventoryItemType.Story)
		{
			return null;
		}
		if (m_IsStarCollection)
		{
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			int itemValue = DIContainerLogic.InventoryService.GetItemValue(currentPlayer.InventoryGameData, "star_collection");
			List<float> list2 = new List<float>();
			List<BasicShopOfferBalancingData> allShopOffersInCategory = DIContainerLogic.GetShopService().GetAllShopOffersInCategory("star_rewards");
			BasicShopOfferBalancingData basicShopOfferBalancingData = null;
			foreach (BasicShopOfferBalancingData item in allShopOffersInCategory)
			{
				if (item == null)
				{
					continue;
				}
				foreach (Requirement buyRequirement in item.BuyRequirements)
				{
					if (buyRequirement.NameId == "star_collection")
					{
						list2.Add(buyRequirement.Value);
						break;
					}
				}
				if (item.NameId.EndsWith(basicItemGameData.BalancingData.NameId.Replace("star_popup", string.Empty)))
				{
					basicShopOfferBalancingData = item;
				}
			}
			if (basicShopOfferBalancingData == null)
			{
				Debug.LogError("No reward to gain found! returning...");
				return new List<GameObject>();
			}
			list2.Sort();
			float num4 = 0f;
			foreach (Requirement item2 in basicShopOfferBalancingData.BuyRequirements.Where((Requirement b) => b.RequirementType == RequirementType.HaveItem && b.NameId == "star_collection"))
			{
				num4 = item2.Value;
			}
			for (int i = 0; i < list2.Count; i++)
			{
				if ((float)itemValue >= list2[i] && list2[i] >= num4)
				{
					num2 = list2[i];
					num3 = i + 1;
					if (i < list2.Count - 1)
					{
						num = list2[i + 1];
					}
					if (num2 > m_lastcollectedPopup)
					{
						m_lastcollectedPopup = num2;
						break;
					}
				}
			}
			if (num != num2 && num != 0f)
			{
				string text = DIContainerInfrastructure.GetLocaService().Tr("popup_starcollection_subheader");
				text = text.Replace("{value_1}", num.ToString());
				m_StarCollectionSubHeader.text = text;
			}
			else
			{
				m_StarCollectionSubHeader.gameObject.SetActive(false);
			}
			m_StarCollectionValue.text = itemValue.ToString();
			string text2 = DIContainerInfrastructure.GetLocaService().Tr("popup_starcollection_desc");
			text2 = text2.Replace("{value_1}", num2.ToString());
			text2 = text2.Replace("{value_2}", m_BasicItemGameData.ItemLocalizedName);
			int value = basicShopOfferBalancingData.OfferContents.FirstOrDefault().Value;
			text2 = ((value != 1) ? text2.Replace("{value_3}", value + "x") : text2.Replace("{value_3} ", string.Empty));
			m_OfferDescLabel.text = text2;
		}
		else if (m_IsEventCampaignReward)
		{
			m_OfferNameLabel.text = m_BasicItemGameData.ItemLocalizedName;
			m_OfferDescLabel.text = DIContainerInfrastructure.GetLocaService().Tr(m_BasicItemGameData.BalancingData.LocaBaseId + "_reward");
		}
		else
		{
			m_OfferNameLabel.text = m_BasicItemGameData.ItemLocalizedName;
			m_OfferDescLabel.text = m_BasicItemGameData.ItemLocalizedDesc;
		}
		if (m_IsRainbowRiot)
		{
			int num5 = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot1Multi;
			if (basicItemGameData.ItemBalancing.NameId == "special_offer_rainbow_riot_02")
			{
				num5 = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot2Multi;
			}
			m_OfferDescLabel.text = m_OfferDescLabel.text.Replace("{value_1}", num5.ToString());
		}
		string[] array = basicItemGameData.ItemAssetName.Split(';');
		List<GameObject> list3 = new List<GameObject>();
		if (array.Length == 1)
		{
			list3.Add(CreateFeatureObjectAsset(array[0], basicItemGameData, centerTransform, rooTransform));
		}
		return list3;
	}

	private GameObject CreateFeatureObjectAsset(string assetName, BasicItemGameData basicItemGameData, Transform centerTransform, Transform rooTransform)
	{
		if (assetName.StartsWith("bird_"))
		{
			m_FeatureObjectNameId.Add(assetName);
			CharacterControllerCamp characterControllerCamp = UnityEngine.Object.Instantiate(m_CampCharacterControllerPrefab);
			m_ObjectType = FeatureObjectType.BirdUnlock;
			characterControllerCamp.transform.parent = rooTransform;
			characterControllerCamp.transform.localPosition = Vector3.zero;
			characterControllerCamp.transform.localScale = Vector3.one;
			characterControllerCamp.SetModel(new BirdGameData(assetName, DIContainerInfrastructure.GetCurrentPlayer().Data.Level), false);
			UnityHelper.SetLayerRecusively(characterControllerCamp.gameObject, base.gameObject.layer);
			return characterControllerCamp.gameObject;
		}
		if (assetName.StartsWith("pig_"))
		{
			m_FeatureObjectNameId.Add(assetName);
			CharacterControllerCamp characterControllerCamp2 = UnityEngine.Object.Instantiate(m_CampCharacterControllerPrefab);
			m_ObjectType = FeatureObjectType.BirdUnlock;
			characterControllerCamp2.transform.parent = rooTransform;
			characterControllerCamp2.transform.localPosition = Vector3.zero;
			characterControllerCamp2.transform.localScale = Vector3.one;
			PigBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<PigBalancingData>(assetName);
			characterControllerCamp2.SetModel(balancingData.NameId, false);
			StartCoroutine(CheerCharacterRepeating(characterControllerCamp2));
			UnityHelper.SetLayerRecusively(characterControllerCamp2.gameObject, base.gameObject.layer);
			return characterControllerCamp2.gameObject;
		}
		if (assetName.StartsWith("Headgear_"))
		{
			m_FeatureObjectNameId.Add(assetName);
			GameObject gameObject = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(assetName, centerTransform, Vector3.zero, Quaternion.identity, false);
			m_ObjectType = FeatureObjectType.ClassUnlock;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			UnityHelper.SetLayerRecusively(gameObject.gameObject, base.gameObject.layer);
			return gameObject.gameObject;
		}
		if (assetName.StartsWith("ShopOffer_"))
		{
			m_ResourcesOfferSprite.gameObject.SetActive(true);
			if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset("ShopAndSocialElements"))
			{
				GameObject gameObject2 = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject("ShopAndSocialElements") as GameObject;
				m_ResourcesOfferSprite.atlas = gameObject2.GetComponent<UIAtlas>();
			}
			m_ResourcesOfferSprite.spriteName = assetName;
			return m_ResourcesOfferSprite.gameObject;
		}
		if (assetName.StartsWith("EnergyDrinks"))
		{
			m_FeatureObjectNameId.Add(assetName);
			GameObject gameObject3 = DIContainerInfrastructure.PropLiteAssetProvider().InstantiateObject(assetName, centerTransform, Vector3.zero, Quaternion.identity);
			m_ObjectType = FeatureObjectType.ResourceUnlock;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localScale = Vector3.one;
			UnityHelper.SetLayerRecusively(gameObject3.gameObject, base.gameObject.layer);
			return gameObject3.gameObject;
		}
		if ((assetName == "GoldenPigMachine" && DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "story_goldenpig_advanced") >= 1) || assetName == "StarCollectionReward_AdvGoldenPig")
		{
			assetName = "AdvGoldenPigMachine";
		}
		if (DIContainerInfrastructure.PropLiteAssetProvider().ContainsAsset(basicItemGameData.ItemAssetName))
		{
			m_FeatureObjectNameId.Add(assetName);
			GameObject gameObject4 = DIContainerInfrastructure.PropLiteAssetProvider().InstantiateObject(assetName, centerTransform, Vector3.zero, Quaternion.identity);
			gameObject4.SetActive(true);
			VectorContainer component = gameObject4.GetComponent<VectorContainer>();
			if ((bool)component)
			{
				gameObject4.transform.parent = centerTransform;
				gameObject4.transform.localPosition = Vector3.zero;
				gameObject4.transform.localScale = Vector3.one;
				gameObject4.transform.localPosition += component.m_Vector;
			}
			else
			{
				gameObject4.transform.parent = rooTransform;
				gameObject4.transform.localPosition = Vector3.zero;
				gameObject4.transform.localScale = Vector3.one;
			}
			UnityHelper.SetLayerRecusively(gameObject4, base.gameObject.layer);
			return gameObject4;
		}
		return null;
	}

	private IEnumerator CountDownTime(float timeLeft)
	{
		while (timeLeft >= 0f)
		{
			m_TimerLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(TimeSpan.FromSeconds(timeLeft));
			yield return new WaitForSeconds(1f);
			timeLeft -= 1f;
		}
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator CheerCharacterRepeating(CharacterControllerCamp character)
	{
		if ((bool)character)
		{
			yield return new WaitForSeconds(character.PlayCheerCharacter());
			yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 6f));
			StartCoroutine(CheerCharacterRepeating(character));
		}
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, AbortButtonClicked);
		m_AbortButton.Clicked += AbortButtonClicked;
		m_GetItButton.Clicked += GetItButtonClicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_AbortButton.Clicked -= AbortButtonClicked;
		m_GetItButton.Clicked -= GetItButtonClicked;
	}

	public void QuickLeave()
	{
		DeRegisterEventHandlers();
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		m_IsShowing = false;
		m_IsRainbowRiot = false;
		m_IsStarCollection = false;
		m_IsEventCampaignReward = false;
		m_IsHint = false;
		m_HintDestination = string.Empty;
		m_IsEnergyTutorial = false;
		m_IsStampCard = false;
		m_ShopOffer = false;
		if (m_AsyncOperation != null)
		{
			m_AsyncOperation.Abort();
		}
		m_AsyncOperation = null;
		base.gameObject.SetActive(false);
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_unlock_leave");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		GetComponent<Animation>().Play("Popup_SpecialOffer_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_SpecialOffer_Leave"].length);
		if (m_ShopOffer)
		{
			for (int index = 0; index < m_FeatureObject.Count; index++)
			{
				UnityEngine.Object.Destroy(m_FeatureObject[index]);
			}
		}
		else if (m_ObjectType == FeatureObjectType.BirdUnlock)
		{
			foreach (GameObject o in m_FeatureObject)
			{
				o.GetComponent<CharacterControllerCamp>().DestroyCharacter();
			}
		}
		else if (m_ObjectType == FeatureObjectType.CampPropUnlock)
		{
			for (int index2 = 0; index2 < m_FeatureObjectNameId.Count; index2++)
			{
				string feature = m_FeatureObjectNameId[index2];
				DIContainerInfrastructure.PropLiteAssetProvider().DestroyObject(feature, m_FeatureObject[index2]);
			}
		}
		else if (m_ObjectType == FeatureObjectType.ClassUnlock)
		{
			for (int index3 = 0; index3 < m_FeatureObjectNameId.Count; index3++)
			{
				string feature2 = m_FeatureObjectNameId[index3];
				DIContainerInfrastructure.GetClassAssetProvider().DestroyObject(feature2, m_FeatureObject[index3]);
			}
		}
		else if (m_ObjectType == FeatureObjectType.ResourceUnlock)
		{
			for (int index4 = 0; index4 < m_FeatureObjectNameId.Count; index4++)
			{
				string feature3 = m_FeatureObjectNameId[index4];
				DIContainerInfrastructure.PropLiteAssetProvider().DestroyObject(feature3, m_FeatureObject[index4]);
			}
		}
		if (m_FeatureObject != null)
		{
			m_FeatureObject.Clear();
		}
		if (m_FeatureObjectNameId != null)
		{
			m_FeatureObjectNameId.Clear();
		}
		m_IsShowing = false;
		m_IsRainbowRiot = false;
		m_IsStarCollection = false;
		m_IsEventCampaignReward = false;
		m_IsHint = false;
		m_HintDestination = string.Empty;
		m_IsEnergyTutorial = false;
		m_ShopOffer = false;
		m_IsStampCard = false;
		m_WorldBossPopup = false;
		m_AsyncOperation.Abort();
		m_AsyncOperation = null;
		if (m_BasicItemGameData != null)
		{
			DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("feature_unlocked", m_BasicItemGameData.BalancingData.NameId);
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_unlock_leave");
		base.gameObject.SetActive(false);
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	private void AbortButtonClicked()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", "abort_button_pressed");
		StartCoroutine("LeaveCoroutine");
	}

	private void GetItButtonClicked()
	{
		DIContainerInfrastructure.LocationStateMgr.StopPopupCoroutine();
		DeRegisterEventHandlers();
		DIContainerInfrastructure.GetCoreStateMgr().StopAutoDailyLoginPopup();
		DIContainerInfrastructure.GetCoreStateMgr().m_DailyLoginUi.StopEnterCoroutine();
		if (m_WorldBossPopup)
		{
			EventSystemWorldMapStateMgr eventsWorldMapStateMgr = DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr;
			EventPositionNode bossNode = eventsWorldMapStateMgr.GetBossNode();
			if (bossNode != null)
			{
				DIContainerInfrastructure.LocationStateMgr.TweenCameraToTransform(bossNode.transform);
			}
		}
		else if (m_IsRainbowRiot)
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreenViaHotlink("story_goldenpig");
		}
		else if (m_IsStarCollection)
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreenViaHotlink("Starcollection");
		}
		else if (m_IsEventCampaignReward)
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreen();
		}
		else if (m_IsEnergyTutorial)
		{
			DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreenViaHotlink("story_cauldron");
		}
		else if (m_ShopOffer)
		{
			if (!(m_shopCategory == "dojo_001"))
			{
				QuickLeave();
				DIContainerInfrastructure.LocationStateMgr.BlockFeatureUnlocks = true;
				DIContainerInfrastructure.GetCoreStateMgr().ShowShop(m_shopCategory, delegate
				{
					DIContainerInfrastructure.LocationStateMgr.WorldMenuUI.Enter();
				}, 0, false, "Popup");
				return;
			}
			OpenDojoScreen();
		}
		else if (m_IsStampCard)
		{
			OpenDojoScreen();
		}
		else if (m_IsHint)
		{
			HintCheckoutClickedHandler();
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().ShowShop(m_shopCategory, delegate
			{
			});
		}
		StartCoroutine("LeaveCoroutine");
	}

	private void OpenDojoScreen()
	{
		if (DIContainerInfrastructure.LocationStateMgr is WorldMapStateMgr)
		{
			DIContainerInfrastructure.LocationStateMgr.BlockFeatureUnlocks = true;
			(DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr).ZoomToDojo();
		}
	}

	private void HintCheckoutClickedHandler()
	{
		WorldMapStateMgr worldMapStateMgr = DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr;
		switch (m_HintDestination)
		{
		case "samurai_shop":
			if ((bool)worldMapStateMgr)
			{
				DIContainerInfrastructure.LocationStateMgr.BlockFeatureUnlocks = true;
				HotspotGameData value2 = null;
				DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue("hotspot_032_02_trainerhut", out value2);
				HotSpotWorldMapViewBase worldMapView2 = value2.WorldMapView;
				worldMapStateMgr.TweenCameraToTransform(worldMapView2.transform);
				worldMapView2.ShowContentView();
			}
			break;
		case "samurai_equip":
			DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreenViaHotlink("bird_manager:bird_red");
			break;
		case "lightningbird_shop":
			if ((bool)worldMapStateMgr)
			{
				DIContainerInfrastructure.LocationStateMgr.BlockFeatureUnlocks = true;
				HotspotGameData value = null;
				DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue("hotspot_015_trainerhut", out value);
				HotSpotWorldMapViewBase worldMapView = value.WorldMapView;
				worldMapStateMgr.TweenCameraToTransform(worldMapView.transform);
				worldMapView.ShowContentView();
			}
			break;
		case "lightningbird_equip":
			DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreenViaHotlink("bird_manager:bird_yellow");
			break;
		}
	}

	public WaitTimeOrAbort ShowSpecialOfferPopup(SalesManagerBalancingData sale)
	{
		m_IsShowing = true;
		base.gameObject.SetActive(true);
		m_StandardBackground.SetActive(false);
		m_BlueBackground.SetActive(true);
		m_SpecialOfferRoot.SetActive(false);
		m_DiscountOnButtonRoot.SetActive(false);
		m_StarCollectionRoot.SetActive(false);
		m_OfferTimerRoot.SetActive(true);
		m_StandardLabelRoot.SetActive(true);
		bool flag = sale.SaleType == SaleAvailabilityType.Conditional || sale.SaleType == SaleAvailabilityType.ConditionalCooldown;
		if (sale.ContentType == SaleContentType.RainbowRiot)
		{
			m_IsRainbowRiot = true;
			m_DiscountOptionA.SetActive(false);
			m_DiscountOptionB.SetActive(false);
		}
		else
		{
			m_ShopOffer = true;
			m_shopCategory = (from o in DIContainerBalancing.Service.GetBalancingDataList<ShopBalancingData>()
				where o.Categories.Contains(sale.CheckoutCategory)
				select o).FirstOrDefault().NameId;
			if (sale.IsAnyBundle)
			{
				m_DiscountOptionA.SetActive(false);
				m_DiscountOptionB.SetActive(false);
			}
			else
			{
				m_DiscountOptionA.SetActive(!flag && sale.ContentType != SaleContentType.LuckyCoinDiscount);
				m_DiscountOptionB.SetActive(flag);
			}
			SetDiscountLabel(sale, flag);
		}
		string dummyText = DIContainerInfrastructure.GetLocaService().Tr("shop_specialofferlabel");
		m_SpecialOfferTopLabel.text = DIContainerInfrastructure.GetLocaService().Tr(sale.LocaBaseId + "_sticker", dummyText);
		if (!string.IsNullOrEmpty(sale.PopupIconId))
		{
			m_ResourcesOfferSprite.gameObject.SetActive(true);
			if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(sale.PopupAtlasId))
			{
				GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(sale.PopupAtlasId) as GameObject;
				m_ResourcesOfferSprite.atlas = gameObject.GetComponent<UIAtlas>();
			}
			else if (DIContainerInfrastructure.GetShopIconAtlasAssetProvider().ContainsAsset(sale.PopupAtlasId))
			{
				GameObject gameObject2 = DIContainerInfrastructure.GetShopIconAtlasAssetProvider().GetObject(sale.PopupAtlasId) as GameObject;
				m_ResourcesOfferSprite.atlas = gameObject2.GetComponent<UIAtlas>();
			}
			m_ResourcesOfferSprite.spriteName = sale.PopupIconId;
			m_ResourcesOfferSprite.MakePixelPerfect();
		}
		m_OfferNameLabel.text = DIContainerInfrastructure.GetLocaService().GetShopOfferName(sale.LocaBaseId);
		m_OfferDescLabel.text = DIContainerInfrastructure.GetLocaService().GetShopOfferDesc(sale.LocaBaseId);
		m_SpecialOfferRoot.SetActive(true);
		base.gameObject.SetActive(true);
		StartCoroutine(CountDownTime(DIContainerLogic.GetSalesManagerService().GetRemainingSaleDuration(sale)));
		StartCoroutine(EnterCoroutineWithoutInstantiate());
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, true);
		m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		return m_AsyncOperation;
	}
}
