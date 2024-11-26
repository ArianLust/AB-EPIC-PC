using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.GameDatas.MailboxMessages;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Rcs;
using SmoothMoves;
using UnityEngine;

public class GachaPopupUI : MonoBehaviour
{
	[SerializeField]
	private GameObject m_GachaRoot;

	[SerializeField]
	private GameObject m_ResultRoot;

	[SerializeField]
	private GachaItemPreview m_GachaItemPreview;

	[SerializeField]
	private Transform m_PreviewRoot;

	[SerializeField]
	private GameObject m_SponsoredRollRoot;

	[SerializeField]
	private GameObject m_RainbowRiotNoticeRoot;

	[SerializeField]
	private UISprite m_RainbowRiotNoticeSprite;

	[SerializeField]
	private UIInputTrigger m_SponsoredRollButton;

	[SerializeField]
	private UIInputTrigger m_SetInfoButton;

	[SerializeField]
	private GameObject m_NewSetIndicator;

	[SerializeField]
	private Animation m_GatchaAnimation;

	[SerializeField]
	private Animation m_ResultAnimation;

	[SerializeField]
	private BoneAnimation m_PigMachineAnimation;

	[SerializeField]
	private Transform m_PigMachineRoot;

	[SerializeField]
	private Transform m_AdvPigMachineRoot;

	[SerializeField]
	private GameObject m_AdvRainbowRiotNoticeRoot;

	[SerializeField]
	private UISprite m_AdvRainbowRiotNoticeSprite;

	[SerializeField]
	private ParticleSystem m_AdvRainbowRiotEffect;

	[SerializeField]
	private Transform m_AdvGachaHeaderCrownIcon;

	private bool m_isAdvancedGacha;

	[SerializeField]
	private Animation[] m_StarAnimations;

	[SerializeField]
	private UISprite[] m_StarBodySprites;

	[SerializeField]
	private UISprite[] m_RainbowStarsOverlays;

	private List<bool> m_starList = new List<bool>();

	[SerializeField]
	private GameObject m_GoldenPig;

	[SerializeField]
	private EquipmentStatsSpeechBubble m_BubbleStats;

	[SerializeField]
	private StatisticsElement m_StatsWithChangeIndicator;

	[SerializeField]
	private UISprite m_PerkSprite;

	[SerializeField]
	private GameObject m_PerkObjectSprite;

	[SerializeField]
	private UISprite m_PerkSkillSprite;

	[SerializeField]
	private GameObject m_PerkSkillObjectSprite;

	[SerializeField]
	public UITapHoldTrigger m_TapHoldTrigger;

	[SerializeField]
	private UISprite m_SetProgressBar;

	[SerializeField]
	private GameObject m_RiotTimerRoot;

	[SerializeField]
	private UILabel m_RiotTimer;

	[SerializeField]
	private ParticleSystem m_RainbowRiotEffect;

	[SerializeField]
	private LootDisplayContoller m_ItemDisplay;

	[SerializeField]
	private LootDisplayContoller m_SetItemDisplay;

	[SerializeField]
	private LootDisplayContoller m_OtherSetItemDisplay;

	[SerializeField]
	private UILabel m_OtherSetItemLabel;

	[SerializeField]
	private UISprite m_PreviewProgressBar;

	[SerializeField]
	private Animation m_RainbowStarAnimation;

	[SerializeField]
	private GameObject m_SetItemProgress;

	[SerializeField]
	private GameObject m_Arrow;

	[SerializeField]
	private float m_ArrowRotationInDegrees;

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
	private GameObject m_GainedRoot;

	[SerializeField]
	private CharacterControllerCamp m_CampViewController;

	[SerializeField]
	private UILabel m_GachaHeader;

	[SerializeField]
	private Transform m_GachaHeaderCrownIcon;

	[SerializeField]
	public UIInputTrigger m_AcceptButton;

	[SerializeField]
	public UIInputTrigger m_EquipButton;

	[SerializeField]
	public UIInputTrigger m_BackButton;

	[SerializeField]
	public UITapHoldTrigger m_GachaTooltip;

	[SerializeField]
	public UIInputTrigger m_PigMachineButton;

	[SerializeField]
	public UISprite m_PigMachineButtonIcon;

	[SerializeField]
	public UIInputTrigger m_PigMachineHighButton;

	[SerializeField]
	public UISprite m_PigMachineHighButtonIcon;

	[SerializeField]
	private UILabel m_PigMachineHighLabel;

	[SerializeField]
	private UISprite m_PvpRainbowStarSprite;

	[SerializeField]
	private GameObject m_FreeRollIndicator;

	[SerializeField]
	public ResourceCostBlind m_GachaCost;

	[SerializeField]
	public ResourceCostBlind m_GachaCostHigh;

	[SerializeField]
	private FriendInfoElement m_FlyingFriendIcon;

	[SerializeField]
	private GameObject m_CharacterRoot;

	[SerializeField]
	private SoundTriggerList m_SoundTriggers;

	[SerializeField]
	private GameObject m_GachaVideoObject;

	[SerializeField]
	private GameObject m_GachaTimerObject;

	[SerializeField]
	private UILabel m_GachaTimerText;

	private GameObject m_EquipmentSprite;

	private bool m_equipableItem;

	private bool m_bannerItem;

	private List<IInventoryItemGameData> m_items;

	private IInventoryItemGameData m_currentItem;

	private BirdGameData m_PossibleBird;

	private BoneAnimation m_birdAnimation;

	private Animator m_bannerAnimation;

	private CharacterControllerCamp m_equipCharacter;

	private bool m_betterItem;

	private int m_delta;

	private bool m_EquipPressed;

	private List<LootDisplayContoller> m_ExplodedLoot = new List<LootDisplayContoller>();

	private bool m_ScrapInfoShown;

	private float m_SlicedBubbleBaseSize;

	private BaseCampStateMgr m_CampStateMgr;

	private BasicShopOfferBalancingData m_GachaOffer;

	private bool m_RainbowStars;

	private bool m_GatchaStarted;

	private float m_currentGachaUses;

	private float m_neededGachaUses;

	private Quaternion m_ArrowStartRotation;

	private int m_ArrowStartHeight;

	private float m_OldProgress;

	private bool m_bIsSetItem;

	private bool m_AcceptPressed;

	private bool m_arenaGacha;

	public static string GACHA_PLACEMENT = "RewardVideo.Gacha";

	public static string PVPGACHA_PLACEMENT = "RewardVideo.PvPGacha";

	private float m_lastAdCancelledTime;

	private float m_lastAdCompletedTime;

	private float m_pvpLastAdCancelledTime;

	private float m_pvpLastAdCompletedTime;

	private GachaLogic m_gachaLogic;

	private bool m_isMultiGacha;

	[SerializeField]
	private float m_segmentSizeInEulerAngle = 30f;

	[SerializeField]
	private float m_yAngleOffset = 20f;

	[SerializeField]
	private float m_PreviewFireTimeMin = 0.5f;

	[SerializeField]
	private float m_PreviewFireTimeRandomPart = 0.3f;

	[SerializeField]
	private GameObject m_SkipCollider;

	[SerializeField]
	private UIInputTrigger m_SkipTrigger;

	private BannerGameData m_banner;

	private bool m_waitingForInput;

	[SerializeField]
	private GameObject m_AdPendingSpinner;

	[method: MethodImpl(32)]
	public event Action<BirdGameData> EquipBirdClicked;

	[method: MethodImpl(32)]
	public event Action<BannerGameData> EquipBannerClicked;

	[method: MethodImpl(32)]
	public event Action ConfirmedGatchaClicked;

	private void Awake()
	{
		m_ArrowStartRotation = m_Arrow.transform.localRotation;
		m_ArrowStartHeight = m_Arrow.GetComponent<UISprite>().height;
		m_BackButton.gameObject.SetActive(false);
		m_PigMachineButton.gameObject.SetActive(false);
		m_PigMachineHighButton.gameObject.SetActive(false);
	}

	private void Start()
	{
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
		}
	}

	private void OnEnable()
	{
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		Leave();
	}

	public void ShowPerkTooltip()
	{
		EquipmentGameData equipmentGameData = m_currentItem as EquipmentGameData;
		if (equipmentGameData != null && (bool)m_PerkSprite)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowPerkOverlay(m_PerkSprite.cachedTransform, equipmentGameData, true);
		}
	}

	public void SetStateMgr(BaseCampStateMgr stateMgr)
	{
		m_CampStateMgr = stateMgr;
		if (stateMgr is ArenaCampStateMgr)
		{
			m_arenaGacha = true;
		}
		else
		{
			m_arenaGacha = false;
		}
		m_gachaLogic = new GachaLogic(m_arenaGacha);
	}

	public void Enter(bool enterBackground = true)
	{
		m_EquipPressed = false;
		m_AcceptPressed = false;
		m_ScrapInfoShown = false;
		base.gameObject.SetActive(true);
		m_isAdvancedGacha = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "story_goldenpig_advanced") >= 1;
		if (ClientInfo.IsFriend)
		{
			InventoryGameData inventory = new InventoryGameData(ClientInfo.InspectedFriend.PublicPlayerData.Inventory);
			m_isAdvancedGacha = DIContainerLogic.InventoryService.GetItemValue(inventory, "story_goldenpig_advanced") >= 1;
		}
		if (m_isAdvancedGacha)
		{
			m_AdvPigMachineRoot.gameObject.SetActive(true);
			m_PigMachineRoot.gameObject.SetActive(false);
			m_PigMachineAnimation = m_AdvPigMachineRoot.GetComponent<BoneAnimation>();
			m_GachaTooltip = m_AdvPigMachineRoot.GetComponent<UITapHoldTrigger>();
			m_PigMachineRoot = m_AdvPigMachineRoot;
			m_GachaHeaderCrownIcon = m_AdvGachaHeaderCrownIcon;
			m_RainbowRiotEffect = m_AdvRainbowRiotEffect;
			m_RainbowRiotNoticeSprite = m_AdvRainbowRiotNoticeSprite;
			m_RainbowRiotNoticeRoot = m_AdvRainbowRiotNoticeRoot;
		}
		m_GatchaStarted = false;
		m_GachaOffer = DIContainerLogic.GetShopService().GetGachaOffer(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), m_isAdvancedGacha);
		if (m_arenaGacha)
		{
			m_PreviewProgressBar.fillAmount = GetPvpSetProgress();
			m_PvpRainbowStarSprite.spriteName = "GachaBar_Star_PvP";
		}
		else
		{
			m_PreviewProgressBar.fillAmount = GetSetProgress();
			m_PvpRainbowStarSprite.spriteName = "GachaBar_Star";
		}
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.transform.position.z);
		// m_SponsoredRollRoot.SetActive(!ClientInfo.IsFriend); no ads, so no free rolls
		m_SponsoredRollRoot.SetActive(false);
		m_SponsoredRollRoot.GetComponent<Animation>().Play("SponsoredRoll_Enter");
		if (enterBackground)
		{
			StartCoroutine(GachaVideoCoroutine());
		}
		StartCoroutine(EnterCoroutine(enterBackground));
	}

	private float GetSetProgress()
	{
		BasicShopOfferBalancingData gachaOffer = DIContainerLogic.GetShopService().GetGachaOffer(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), m_isAdvancedGacha);
		if (gachaOffer.Category.Contains("_set"))
		{
			return 1f;
		}
		if (gachaOffer.NameId == "offer_gacha_first_roll")
		{
			return 0f;
		}
		m_currentGachaUses = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "gacha_standard_uses");
		Requirement requirement = DIContainerLogic.GetShopService().GetModifiedShowRequirements(gachaOffer).FirstOrDefault((Requirement r) => r.NameId == "gacha_standard_uses" && r.RequirementType == RequirementType.NotHaveItem);
		if (requirement != null)
		{
			m_neededGachaUses = requirement.Value;
		}
		return m_currentGachaUses / (m_neededGachaUses + 1f);
	}

	private float GetPvpSetProgress()
	{
		BasicShopOfferBalancingData gachaOffer = DIContainerLogic.GetShopService().GetGachaOffer(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), m_isAdvancedGacha);
		if (gachaOffer.Category.Contains("_set"))
		{
			return 1f;
		}
		if (gachaOffer.NameId == "offer_pvpgacha_first_roll")
		{
			return 0f;
		}
		m_currentGachaUses = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "pvpgacha_standard_uses");
		Requirement requirement = DIContainerLogic.GetShopService().GetModifiedShowRequirements(gachaOffer).FirstOrDefault((Requirement r) => r.NameId == "pvpgacha_standard_uses" && r.RequirementType == RequirementType.NotHaveItem);
		if (requirement != null)
		{
			m_neededGachaUses = requirement.Value;
		}
		return m_currentGachaUses / (m_neededGachaUses + 1f);
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine(true));
	}

	private IEnumerator LeaveCoroutine(bool disable, bool leaveBackground = true)
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("gacha_leave");
		m_CampStateMgr.ResetRiotAnim();
		if (m_SponsoredRollRoot.activeInHierarchy)
		{
			m_SponsoredRollRoot.GetComponent<Animation>().Play("SponsoredRoll_Leave");
		}
		if (m_RainbowRiotNoticeRoot.activeInHierarchy)
		{
			m_RainbowRiotNoticeRoot.GetComponent<Animation>().Play("RainbowRiotMarker_Leave");
		}
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
		if (m_GatchaStarted)
		{
			yield return StartCoroutine(CleanUpGachaWindow());
		}
		else
		{
			m_GatchaAnimation.Play("GachaStep_1_Exit");
			yield return new WaitForSeconds(m_GatchaAnimation["GachaStep_1_Exit"].length);
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("gacha_leave");
		m_SponsoredRollRoot.SetActive(false);
		m_RainbowRiotNoticeRoot.SetActive(false);
		if (disable)
		{
			m_CampStateMgr.RefreshBirdMarkers();
			if (m_CampStateMgr is ArenaCampStateMgr)
			{
				(m_CampStateMgr as ArenaCampStateMgr).RefreshBannerMarkers();
			}
			base.gameObject.SetActive(false);
		}
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
		if (m_BubbleStats.gameObject.activeInHierarchy)
		{
			m_BubbleStats.transform.parent = m_CharacterRoot.transform;
			yield return new WaitForSeconds(m_BubbleStats.Hide());
			m_BubbleStats.gameObject.SetActive(false);
		}
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		m_AcceptButton.Clicked += OnAcceptButtonClicked;
		m_EquipButton.Clicked += OnEquipButtonClicked;
		m_PigMachineButton.Clicked += OnPigMachineButtonClicked;
		m_PigMachineHighButton.Clicked += OnPigMachineHighButtonClicked;
		m_BackButton.Clicked += OnBackButtonClicked;
		m_SetInfoButton.Clicked += ShowSetInfo;
		if (m_arenaGacha)
		{
			m_SponsoredRollButton.Clicked += OnPvPSponsoredRollButtonClicked;
		}
		else
		{
			m_SponsoredRollButton.Clicked += OnSponsoredRollButtonClicked;
		}
		m_SkipTrigger.Clicked += OnSkipClicked;
		m_GachaTooltip.OnTapBegin += GachaTooltipOnTapBegin;
		m_GachaTooltip.OnTapEnd += GachaTooltipOnTapEnd;
		m_GachaTooltip.OnTapReleased += GachaTooltipOnTapReleased;
		DIContainerInfrastructure.AdService.RewardResult += RewardSponsoredAdResult;
		DIContainerInfrastructure.AdService.RewardResult += PvPRewardSponsoredAdResult;
	}

	private void DeregisterEventHandler()
	{
		m_AcceptButton.Clicked -= OnAcceptButtonClicked;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		m_EquipButton.Clicked -= OnEquipButtonClicked;
		m_PigMachineButton.Clicked -= OnPigMachineButtonClicked;
		m_PigMachineHighButton.Clicked -= OnPigMachineHighButtonClicked;
		m_BackButton.Clicked -= OnBackButtonClicked;
		m_SponsoredRollButton.Clicked -= OnSponsoredRollButtonClicked;
		m_SponsoredRollButton.Clicked -= OnPvPSponsoredRollButtonClicked;
		m_SetInfoButton.Clicked -= ShowSetInfo;
		m_GachaTooltip.OnTapBegin -= GachaTooltipOnTapBegin;
		m_GachaTooltip.OnTapEnd -= GachaTooltipOnTapEnd;
		m_GachaTooltip.OnTapReleased -= GachaTooltipOnTapReleased;
		m_SkipTrigger.Clicked -= OnSkipClicked;
		DIContainerInfrastructure.AdService.RewardResult -= RewardSponsoredAdResult;
		DIContainerInfrastructure.AdService.RewardResult -= PvPRewardSponsoredAdResult;
	}

	private void OnSponsoredRollButtonClicked()
	{
		if (DIContainerInfrastructure.AdService.IsAdShowPossible(GACHA_PLACEMENT))
		{
			if (!DIContainerInfrastructure.AdService.ShowAd(GACHA_PLACEMENT))
			{
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_no_ad_available", "There is currently no Ad scheduled"), "no_ad", DispatchMessage.Status.Info);
			}
			else
			{
				DIContainerInfrastructure.AdService.MutedGameSoundForPlacement(GACHA_PLACEMENT);
			}
		}
	}

	private void OnPvPSponsoredRollButtonClicked()
	{
		if (DIContainerInfrastructure.AdService.IsAdShowPossible(PVPGACHA_PLACEMENT))
		{
			if (!DIContainerInfrastructure.AdService.ShowAd(PVPGACHA_PLACEMENT))
			{
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_no_ad_available", "There is currently no Ad scheduled"), "no_ad", DispatchMessage.Status.Info);
			}
			else
			{
				DIContainerInfrastructure.AdService.MutedGameSoundForPlacement(PVPGACHA_PLACEMENT);
			}
		}
	}

	private void RewardSponsoredAdResult(string placement, Ads.RewardResult result, string voucherId)
	{
		if (placement != GACHA_PLACEMENT)
		{
			return;
		}
		DebugLog.Log("[GachaPopupUI] Reward Result received: " + result);
		switch (result)
		{
		case Ads.RewardResult.RewardCanceled:
			m_lastAdCancelledTime = Time.time;
			break;
		case Ads.RewardResult.RewardCompleted:
			m_lastAdCompletedTime = Time.time;
			break;
		case Ads.RewardResult.RewardConfirmed:
			if (m_lastAdCancelledTime > m_lastAdCompletedTime)
			{
				if (Time.time - m_lastAdCancelledTime < 60f)
				{
					OnAdAbortedForFreeGachaRoll();
				}
			}
			else if (Time.time - m_lastAdCompletedTime < 60f)
			{
				OnAdWatchedForFreeGachaRoll();
			}
			break;
		case Ads.RewardResult.RewardFailed:
			OnAdAbortedForFreeGachaRoll();
			break;
		default:
			throw new ArgumentOutOfRangeException("result");
		}
	}

	private void PvPRewardSponsoredAdResult(string placement, Ads.RewardResult result, string voucherId)
	{
		if (placement != PVPGACHA_PLACEMENT)
		{
			return;
		}
		DebugLog.Log("[PvpGachaPopupUI] Reward Result received: " + result);
		switch (result)
		{
		case Ads.RewardResult.RewardCanceled:
			m_pvpLastAdCancelledTime = Time.time;
			break;
		case Ads.RewardResult.RewardCompleted:
			m_pvpLastAdCompletedTime = Time.time;
			break;
		case Ads.RewardResult.RewardConfirmed:
			if (m_pvpLastAdCancelledTime > m_pvpLastAdCompletedTime)
			{
				if (Time.time - m_pvpLastAdCancelledTime < 60f)
				{
					OnPvPAdAbortedForFreeGachaRoll();
				}
			}
			else if (Time.time - m_pvpLastAdCompletedTime < 60f)
			{
				OnPvPAdWatchedForFreeGachaRoll();
			}
			break;
		case Ads.RewardResult.RewardFailed:
			OnPvPAdAbortedForFreeGachaRoll();
			break;
		default:
			throw new ArgumentOutOfRangeException("result");
		}
	}

	private void GachaTooltipOnTapReleased()
	{
	}

	private void GachaTooltipOnTapEnd()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
	}

	private void GachaTooltipOnTapBegin()
	{
		if (m_arenaGacha)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowPvpGachaOverlay(m_PigMachineRoot, true, m_isAdvancedGacha);
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGachaOverlay(m_PigMachineRoot, true, m_isAdvancedGacha);
		}
	}

	private void OnPigMachineHighButtonClicked()
	{
		List<Requirement> failed = new List<Requirement>();
		if (DIContainerLogic.GetShopService().IsGachaOfferBuyAble(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), out failed, m_isAdvancedGacha, true))
		{
			m_isMultiGacha = true;
			DebugLog.Log("Offer is buyable!");
			List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
			int multiGachaAmount = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").MultiGachaAmount;
			int starCount = 0;
			List<IInventoryItemGameData> list2 = DIContainerLogic.GetShopService().BuyGachaOffer(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), false, m_isAdvancedGacha, out starCount, true, multiGachaAmount);
			for (int i = 0; i < list2.Count; i++)
			{
				IInventoryItemGameData item = list2[i];
				list.Add(m_gachaLogic.CheckForDuplicateSetItems(item));
			}
			m_items = list;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int j = 0; j < m_items.Count; j++)
			{
				IInventoryItemGameData inventoryItemGameData = m_items[j];
				dictionary.Add("ItemName " + j, inventoryItemGameData.ItemBalancing.NameId);
			}
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("GachaUsedHigh", dictionary);
			if (m_items != null)
			{
				StartCoroutine(MultiGacha());
			}
			else
			{
				DebugLog.Log("No Item Generated!");
			}
		}
		else if (DIContainerLogic.GetShopService().GetBuyResourcesRequirements(1, DIContainerLogic.GetShopService().GetGachaOffer(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), m_isAdvancedGacha, true)).Count > 0)
		{
			StartCoroutine(HandleRedirectShop());
		}
		else
		{
			DebugLog.Error("Invalid State On Gatcha!");
		}
	}

	private IEnumerator MultiGacha()
	{
		bool firstItem = true;
		StartCoroutine(UpdateAndLeaveCoins());
		m_BackButton.Clicked -= OnBackButtonClicked;
		m_PigMachineButton.Clicked -= OnPigMachineButtonClicked;
		m_PigMachineHighButton.Clicked -= OnPigMachineHighButtonClicked;
		m_GachaTooltip.OnTapBegin -= GachaTooltipOnTapBegin;
		m_GachaTooltip.OnTapEnd -= GachaTooltipOnTapEnd;
		m_GachaTooltip.OnTapReleased -= GachaTooltipOnTapReleased;
		m_CampStateMgr.UpdateFreeGachaSign();
		m_ScrapInfoShown = false;
		m_GatchaStarted = true;
		m_RiotTimerRoot.SetActive(false);
		m_RainbowStarAnimation.Play("RainbowStar_Inactive");
		m_banner = DIContainerInfrastructure.GetCurrentPlayer().BannerGameData;
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("gacha_started");
		if (m_SponsoredRollRoot.activeInHierarchy && !m_SponsoredRollRoot.GetComponent<Animation>().isPlaying)
		{
			m_SponsoredRollRoot.GetComponent<Animation>().Play("SponsoredRoll_Leave");
		}
		if (m_RainbowRiotNoticeRoot.activeInHierarchy && !m_RainbowRiotNoticeRoot.GetComponent<Animation>().isPlaying)
		{
			m_RainbowRiotNoticeRoot.GetComponent<Animation>().Play("RainbowRiotMarker_Leave");
		}
		for (int i = 0; i < m_items.Count; i++)
		{
			m_EquipPressed = false;
			m_AcceptPressed = false;
			m_currentItem = m_items[i];
			if (m_currentItem.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment || m_currentItem.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment)
			{
				m_equipableItem = true;
			}
			SetItem(starCount: m_currentItem.ItemData.Level - DIContainerInfrastructure.GetCurrentPlayer().Data.Level + 2, newItem: m_currentItem);
			if (firstItem)
			{
				firstItem = false;
				yield return StartCoroutine("FirstGachaSequence");
			}
			else if (i + 1 < m_items.Count)
			{
				yield return StartCoroutine("MultiGachaSequence");
			}
			else
			{
				yield return StartCoroutine("LastGachaSequence");
			}
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("gacha_started");
		m_BackButton.Clicked -= OnBackButtonClicked;
		m_BackButton.Clicked += OnBackButtonClicked;
	}

	private IEnumerator FirstGachaSequence()
	{
		if (UnityEngine.Random.Range(0f, 10f) > 9f)
		{
			m_PigMachineAnimation.Play("GachaRoll_Multiple_Alt");
		}
		else
		{
			m_PigMachineAnimation.Play("GachaRoll_Multiple");
		}
		m_GatchaAnimation.Play("GachaStep_1_Leave");
		yield return new WaitForSeconds(m_GatchaAnimation["GachaStep_1_Leave"].length);
		m_SponsoredRollRoot.SetActive(false);
		m_RainbowRiotNoticeRoot.SetActive(false);
		yield return new WaitForSeconds(m_PigMachineAnimation["GachaRoll"].length);
		ShowStarsForItemAnimation();
		m_AcceptButton.gameObject.SetActive(m_betterItem);
		m_waitingForInput = true;
		yield return new WaitForSeconds(m_ResultAnimation["GachaStep_2_EnterFast"].clip.length);
		if (m_betterItem)
		{
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("gacha_started");
			DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, OnAcceptButtonClicked);
			while (m_waitingForInput)
			{
				yield return new WaitForEndOfFrame();
			}
			DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
			DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("gacha_started");
			yield return new WaitForSeconds(0.25f);
		}
		else
		{
			yield return new WaitForSeconds(0.25f);
			yield return StartCoroutine(CleanUpGachaWindow());
		}
	}

	private IEnumerator MultiGachaSequence()
	{
		ShowStarsForItemAnimation();
		m_AcceptButton.gameObject.SetActive(m_betterItem);
		m_waitingForInput = true;
		RegisterEventHandler();
		yield return new WaitForSeconds(m_ResultAnimation["GachaStep_2_EnterFast"].clip.length);
		if (m_betterItem)
		{
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("gacha_started");
			DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, OnAcceptButtonClicked);
			while (m_waitingForInput)
			{
				yield return new WaitForEndOfFrame();
			}
			DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
			DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("gacha_started");
			yield return new WaitForSeconds(0.25f);
		}
		else
		{
			yield return new WaitForSeconds(0.25f);
			yield return StartCoroutine(CleanUpGachaWindow());
		}
	}

	private IEnumerator LastGachaSequence()
	{
		ShowStarsForItemAnimation();
		m_AcceptButton.gameObject.SetActive(m_betterItem);
		m_waitingForInput = true;
		RegisterEventHandler();
		yield return new WaitForSeconds(m_ResultAnimation["GachaStep_2_EnterFast"].clip.length);
		if (m_betterItem)
		{
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("gacha_started");
			DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, OnAcceptButtonClicked);
			while (m_waitingForInput)
			{
				yield return new WaitForEndOfFrame();
			}
			DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
			DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("gacha_started");
		}
		DeregisterEventHandler();
		yield return new WaitForSeconds(1f);
		ReEnter();
	}

	private void ShowStarsForItemAnimation()
	{
		if (m_equipableItem && m_PossibleBird != null)
		{
			float itemMainStat = m_currentItem.ItemMainStat;
			float num = ((m_currentItem.ItemBalancing.ItemType != InventoryItemType.MainHandEquipment) ? m_PossibleBird.OffHandItem.ItemMainStat : m_PossibleBird.MainHandItem.ItemMainStat);
			m_betterItem = itemMainStat > num;
		}
		else if (m_bannerItem && m_banner != null)
		{
			float itemMainStat2 = m_currentItem.ItemMainStat;
			float num2 = ((m_currentItem.ItemBalancing.ItemType == InventoryItemType.Banner) ? m_banner.BannerCenter.ItemMainStat : ((m_currentItem.ItemBalancing.ItemType != InventoryItemType.BannerEmblem) ? m_banner.BannerTip.ItemMainStat : m_banner.BannerEmblem.ItemMainStat));
			m_betterItem = itemMainStat2 > num2;
		}
		else if (m_equipCharacter != null)
		{
			m_equipCharacter.gameObject.SetActive(false);
		}
		if (!m_betterItem)
		{
			m_currentItem.ItemData.IsNew = false;
		}
		m_SponsoredRollRoot.SetActive(false);
		m_RainbowRiotNoticeRoot.SetActive(false);
		m_GachaRoot.SetActive(false);
		m_ItemStatsRoot.SetActive(m_equipableItem || m_bannerItem);
		m_GainedRoot.SetActive(true);
		m_ResultRoot.SetActive(true);
		SetupGachaResultItems();
		m_ResultAnimation.Play((!m_bIsSetItem) ? "GachaStep_2_EnterFast" : "GachaStep_2_EnterFast_SetItem");
		int num3 = 0;
		for (int i = 0; i < 3; i++)
		{
			m_StarBodySprites[i].spriteName = m_StarBodySprites[i].spriteName.Replace("_Desaturated", string.Empty);
			string animation = "ValueStar_GainedWithBonus";
			if (m_RainbowStars)
			{
				m_RainbowStarsOverlays[i].gameObject.SetActive(true);
				animation = "ValueStar_GainedWithBonus";
			}
			else
			{
				m_RainbowStarsOverlays[i].gameObject.SetActive(false);
				if (!m_starList[i])
				{
					m_StarBodySprites[i].spriteName = m_StarBodySprites[i].spriteName + "_Desaturated";
					animation = "ValueStar_GainedNormal";
				}
				else
				{
					num3++;
				}
			}
			m_StarAnimations[i].Play(animation);
		}
		if (m_RainbowStars)
		{
			if ((bool)m_SoundTriggers)
			{
				m_SoundTriggers.OnTriggerEventFired("result_star_rainbow");
			}
		}
		else if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("result_star_" + num3);
		}
		if (m_bIsSetItem)
		{
			m_SetItemDisplay.PlayGainedAnimation();
			m_OtherSetItemDisplay.PlayGainedAnimation();
		}
		else
		{
			m_ItemDisplay.PlayGainedAnimation();
		}
	}

	private IEnumerator CleanUpGachaWindow()
	{
		float HideTime = 0f;
		StopCoroutine("AnimateBird");
		StartCoroutine(HideComparisonBubble());
		if (m_ExplodedLoot.Count > 0)
		{
			for (int j = 0; j < m_ExplodedLoot.Count; j++)
			{
				LootDisplayContoller item = m_ExplodedLoot[j];
				item.HideThenDestroy();
			}
			m_ExplodedLoot.Clear();
		}
		else
		{
			if (m_ItemDisplay.gameObject.activeInHierarchy)
			{
				HideTime = m_ItemDisplay.PlayHideAnimation();
				m_OtherSetItemLabel.gameObject.SetActive(false);
				m_OtherSetItemDisplay.gameObject.SetActive(false);
			}
			if (m_SetItemDisplay.gameObject.activeInHierarchy)
			{
				HideTime = m_SetItemDisplay.PlayHideAnimation();
				m_OtherSetItemDisplay.PlayHideAnimation();
			}
		}
		HideScrapBubble();
		m_ResultAnimation.Play("GachaStep_2_Leave");
		for (int i = 0; i < m_StarAnimations.Length; i++)
		{
			Animation sanim = m_StarAnimations[i];
			sanim.Play("ValueStar_Disappear");
		}
		yield return new WaitForSeconds(Mathf.Max(m_ResultAnimation["GachaStep_2_Leave"].length, HideTime));
		RemoveEquipmentSprite(m_currentItem as EquipmentGameData);
	}

	private void OnAdWatchedForFreeGachaRoll()
	{
		DateTime trustedTime;
		if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.TimeStampOfLastVideoGacha = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
		}
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "free_gacha_use") <= 0)
		{
			DIContainerLogic.InventoryService.AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "free_gacha_use", 1, "sponsored_free_gacha_roll");
		}
		StartCoroutine(GachaVideoCoroutine());
		StartCoroutine(ReEnterPopup());
	}

	private void OnAdAbortedForFreeGachaRoll()
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_advideo_cancelled", "You did not watch the whole video"));
		StartCoroutine(ReEnterPopup());
	}

	private void OnPvPAdWatchedForFreeGachaRoll()
	{
		DateTime trustedTime;
		if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.TimeStampOfLastVideoPvPGacha = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
		}
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "free_pvpgacha_use") <= 0)
		{
			DIContainerLogic.InventoryService.AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "free_pvpgacha_use", 1, "sponsored_free_pvpgacha_roll");
		}
		StartCoroutine(GachaVideoCoroutine());
		StartCoroutine(ReEnterPopup());
	}

	private void OnPvPAdAbortedForFreeGachaRoll()
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_advideo_cancelled", "You did not watch the whole video"));
		StartCoroutine(ReEnterPopup());
	}

	private void OnPigMachineButtonClicked()
	{
		List<Requirement> failed = new List<Requirement>();
		bool flag = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "free_pvpgacha_use") > 0;
		if (DIContainerLogic.GetShopService().IsGachaOfferBuyAble(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), out failed, m_isAdvancedGacha) || IsFreeFriendGacha())
		{
			DebugLog.Log("Offer is buyable!");
			m_isMultiGacha = false;
			m_waitingForInput = false;
			m_AcceptButton.gameObject.SetActive(true);
			int starCount = 0;
			List<IInventoryItemGameData> source = DIContainerLogic.GetShopService().BuyGachaOffer(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), IsFreeFriendGacha(), m_isAdvancedGacha, out starCount);
			m_currentItem = source.FirstOrDefault();
			m_currentItem = m_gachaLogic.CheckForDuplicateSetItems(m_currentItem);
			if (m_currentItem != null)
			{
				if (ClientInfo.IsFriend)
				{
					m_FreeRollIndicator.SetActive(false);
					if (m_arenaGacha)
					{
						DIContainerLogic.SocialService.UsedFreePvpGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer());
					}
					else
					{
						DIContainerLogic.SocialService.UsedFreeGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer());
					}
					SendFreeGachaUsedMessage(ClientInfo.InspectedFriend);
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
					ABHAnalyticsHelper.AddFriendsCountToTracking(dictionary);
					dictionary.Add("ItemName", m_currentItem.ItemBalancing.NameId);
					DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("FriendGachaUsed", dictionary);
				}
				else
				{
					Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
					dictionary2.Add("PlayerLevel", DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString());
					dictionary2.Add("ItemName", m_currentItem.ItemBalancing.NameId);
					dictionary2.Add("VideoAdUsed", flag.ToString());
					DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("GachaUsed", dictionary2);
				}
				if (m_currentItem.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment || m_currentItem.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment)
				{
					m_equipableItem = true;
				}
				SetItem(m_currentItem, starCount);
				m_BackButton.Clicked -= OnBackButtonClicked;
				m_PigMachineButton.Clicked -= OnPigMachineButtonClicked;
				m_PigMachineHighButton.Clicked -= OnPigMachineHighButtonClicked;
				m_GachaTooltip.OnTapBegin -= GachaTooltipOnTapBegin;
				m_GachaTooltip.OnTapEnd -= GachaTooltipOnTapEnd;
				m_GachaTooltip.OnTapReleased -= GachaTooltipOnTapReleased;
				StartCoroutine("StartGachaSequence");
			}
			else
			{
				DebugLog.Log("No Item Generated!");
			}
		}
		else if (DIContainerLogic.GetShopService().GetBuyResourcesRequirements(1, DIContainerLogic.GetShopService().GetGachaOffer(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), m_isAdvancedGacha)).Count > 0)
		{
			StartCoroutine(HandleRedirectShop());
		}
		else
		{
			DebugLog.Error("Invalid State On Gatcha!");
		}
	}

	private void SendFreeGachaUsedMessage(FriendGameData friendGameData)
	{
		if (!friendGameData.isNpcFriend)
		{
			if (m_arenaGacha)
			{
				MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
				messageDataIncoming.MessageType = MessageType.ResponsePvpGachaUseMessage;
				messageDataIncoming.Sender = DIContainerInfrastructure.GetCurrentPlayer().GetFriendData();
				messageDataIncoming.SentAt = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
				MessageDataIncoming message = messageDataIncoming;
				ABHAnalyticsHelper.SendSocialEvent(message, friendGameData.FriendData);
				DIContainerInfrastructure.MessagingService.SendMessages(message, new List<string> { friendGameData.FriendId });
			}
			else
			{
				MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
				messageDataIncoming.MessageType = MessageType.ResponseGachaUseMessage;
				messageDataIncoming.Sender = DIContainerInfrastructure.GetCurrentPlayer().GetFriendData();
				messageDataIncoming.SentAt = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
				MessageDataIncoming message2 = messageDataIncoming;
				ABHAnalyticsHelper.SendSocialEvent(message2, friendGameData.FriendData);
				DIContainerInfrastructure.MessagingService.SendMessages(message2, new List<string> { friendGameData.FriendId });
			}
		}
	}

	private IEnumerator HandleRedirectShop()
	{
		IInventoryItemGameData missingItemGameData = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "lucky_coin", out missingItemGameData) && missingItemGameData.ItemBalancing.NameId == "lucky_coin")
		{
			DIContainerInfrastructure.GetCoreStateMgr().ShowShop("shop_premium", delegate
			{
			}, 0, true);
		}
		yield break;
	}

	private void OnBackButtonClicked()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
		Leave();
	}

	public void OnAcceptButtonClicked()
	{
		if (!m_AcceptPressed)
		{
			m_AcceptPressed = true;
			if (this.ConfirmedGatchaClicked != null)
			{
				this.ConfirmedGatchaClicked();
			}
			DeregisterEventHandler();
			if (m_waitingForInput)
			{
				m_waitingForInput = false;
			}
			else
			{
				StartCoroutine(ReEnterPopup());
			}
		}
	}

	public IEnumerator ReEnterPopup()
	{
		if (ClientInfo.IsFriend)
		{
			SendGachaUsedMessage();
			Leave();
		}
		else
		{
			yield return StartCoroutine(LeaveCoroutine(false, false));
			Enter(false);
		}
	}

	private void SendGachaUsedMessage()
	{
	}

	public void OnScrapButtonClicked()
	{
		if (m_currentItem != null)
		{
			if ((bool)m_SoundTriggers)
			{
				m_SoundTriggers.OnTriggerEventFired("item_scrapped");
			}
			if (m_SetItemDisplay.gameObject.activeInHierarchy)
			{
				m_SetItemDisplay.SetModel(m_currentItem, DIContainerLogic.CraftingService.ScrapEquipment(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_currentItem as EquipmentGameData), LootDisplayType.Set);
				m_ExplodedLoot = m_SetItemDisplay.Explode(false, false, 0f, true, 0f, 0f);
				m_OtherSetItemDisplay.PlayHideAnimation();
				m_OtherSetItemLabel.gameObject.SetActive(false);
			}
			if (m_ItemDisplay.gameObject.activeInHierarchy)
			{
				m_ItemDisplay.SetModel(m_currentItem, DIContainerLogic.CraftingService.ScrapEquipment(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_currentItem as EquipmentGameData), LootDisplayType.Minor);
				m_ExplodedLoot = m_ItemDisplay.Explode(false, false, 0f, true, 0f, 0f);
			}
			StopCoroutine("AnimateBird");
			m_equipCharacter.PlayCheerCharacter();
			m_ItemStatsRoot.SetActive(false);
			StartCoroutine(HideComparisonBubble());
			HideScrapBubble();
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}
	}

	public void OnEquipButtonClicked()
	{
		if (m_EquipPressed)
		{
			return;
		}
		BannerGameData bannerGameData = DIContainerInfrastructure.GetCurrentPlayer().BannerGameData;
		m_EquipPressed = true;
		DebugLog.Log("Equip Button pressed");
		m_currentItem.ItemData.IsNew = false;
		List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
		list.Add(m_currentItem);
		List<IInventoryItemGameData> newContent = list;
		if (m_equipableItem)
		{
			DIContainerLogic.InventoryService.EquipBirdWithItem(newContent, m_currentItem.ItemBalancing.ItemType, m_PossibleBird.InventoryGameData);
			m_equipCharacter.SetModel(m_PossibleBird, false);
		}
		else
		{
			DIContainerLogic.InventoryService.EquipBirdWithItem(newContent, m_currentItem.ItemBalancing.ItemType, bannerGameData.InventoryGameData);
			m_equipCharacter.SetModel(bannerGameData, false);
			UnityHelper.SetLayerRecusively(m_equipCharacter.gameObject, LayerMask.NameToLayer("Interface"));
		}
		StopCoroutine("AnimateBird");
		if (m_bannerAnimation == null)
		{
			m_bannerAnimation = m_equipCharacter.m_AssetController.GetComponent<Animator>();
		}
		switch (m_currentItem.ItemBalancing.ItemType)
		{
		case InventoryItemType.MainHandEquipment:
			m_equipCharacter.m_AssetController.PlayFocusWeaponAnimation();
			break;
		case InventoryItemType.OffHandEquipment:
			m_equipCharacter.m_AssetController.PlayFocusOffHandAnimation();
			break;
		case InventoryItemType.Banner:
			m_bannerAnimation.Play("Focus_Flag");
			break;
		case InventoryItemType.BannerEmblem:
			m_bannerAnimation.Play("Focus_Emblem");
			break;
		case InventoryItemType.BannerTip:
			m_bannerAnimation.Play("Focus_Tip");
			break;
		default:
			m_equipCharacter.PlayCheerCharacter();
			break;
		}
		HideScrapBubble();
		StartCoroutine(HideComparisonBubble());
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		if (this.EquipBirdClicked != null)
		{
			if (m_equipableItem)
			{
				this.EquipBirdClicked(m_PossibleBird);
			}
			else
			{
				this.EquipBannerClicked(bannerGameData);
			}
		}
		DeregisterEventHandler();
		if (m_waitingForInput)
		{
			m_waitingForInput = false;
		}
		else
		{
			Invoke("ReEnter", 1f);
		}
	}

	private void ReEnter()
	{
		StartCoroutine(ReEnterPopup());
	}

	private IEnumerator UpdateProgressBar(UISprite progressBar, float oldValue, float newValue, float duration)
	{
		for (float timeLeft = duration; timeLeft > 0f; timeLeft -= Time.deltaTime)
		{
			yield return new WaitForEndOfFrame();
			progressBar.fillAmount = timeLeft / duration * oldValue + (1f - timeLeft / duration) * newValue;
			m_Arrow.transform.localRotation = m_ArrowStartRotation;
			float offsetBonus = 0f;
			offsetBonus = ((!(progressBar.fillAmount > 0.5f)) ? (1f + progressBar.fillAmount) : (1.9f - progressBar.fillAmount));
			float offset = 0.1f + progressBar.fillAmount * offsetBonus;
			if (offset > 1.1f)
			{
				offset = 1.1f;
			}
			float currentRotation = m_ArrowRotationInDegrees * (progressBar.fillAmount * offset);
			if (currentRotation > m_ArrowRotationInDegrees)
			{
				currentRotation = m_ArrowRotationInDegrees;
			}
			m_Arrow.transform.Rotate(0f, 0f, currentRotation);
			m_Arrow.GetComponent<UISprite>().height = (int)((float)m_ArrowStartHeight * Mathf.Abs(Mathf.Sin(m_Arrow.transform.localRotation.eulerAngles.z * (float)Math.PI / 180f)));
		}
		progressBar.fillAmount = newValue;
	}

	private IEnumerator GachaClicked()
	{
		m_PigMachineAnimation.Play("Clicked");
		yield return new WaitForSeconds(m_PigMachineAnimation["Clicked"].length);
		StartCoroutine("StartGachaSequence");
	}

	private IEnumerator StartGachaSequence()
	{
		m_CampStateMgr.UpdateFreeGachaSign();
		m_GatchaStarted = true;
		m_AcceptPressed = false;
		m_RiotTimerRoot.SetActive(false);
		m_RainbowStarAnimation.Play("RainbowStar_Inactive");
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("gacha_started");
		m_banner = DIContainerInfrastructure.GetCurrentPlayer().BannerGameData;
		if (m_equipableItem && m_PossibleBird != null)
		{
			float newStat2 = m_currentItem.ItemMainStat;
			float oldStat2 = ((m_currentItem.ItemBalancing.ItemType != InventoryItemType.MainHandEquipment) ? m_PossibleBird.OffHandItem.ItemMainStat : m_PossibleBird.MainHandItem.ItemMainStat);
			m_betterItem = newStat2 > oldStat2;
		}
		else if (m_bannerItem && m_banner != null)
		{
			float newStat = m_currentItem.ItemMainStat;
			float oldStat = ((m_currentItem.ItemBalancing.ItemType == InventoryItemType.Banner) ? m_banner.BannerCenter.ItemMainStat : ((m_currentItem.ItemBalancing.ItemType != InventoryItemType.BannerEmblem) ? m_banner.BannerTip.ItemMainStat : m_banner.BannerEmblem.ItemMainStat));
			m_betterItem = newStat > oldStat;
		}
		else if (m_equipCharacter != null)
		{
			m_equipCharacter.gameObject.SetActive(false);
		}
		if (!m_betterItem)
		{
			m_currentItem.ItemData.IsNew = false;
		}
		m_PigMachineAnimation.Play("GachaRoll");
		if (m_SponsoredRollRoot.activeInHierarchy && !m_SponsoredRollRoot.GetComponent<Animation>().isPlaying)
		{
			m_SponsoredRollRoot.GetComponent<Animation>().Play("SponsoredRoll_Leave");
		}
		if (m_RainbowRiotNoticeRoot.activeInHierarchy && !m_RainbowRiotNoticeRoot.GetComponent<Animation>().isPlaying)
		{
			m_RainbowRiotNoticeRoot.GetComponent<Animation>().Play("RainbowRiotMarker_Leave");
		}
		StartCoroutine(UpdateAndLeaveCoins());
		m_GatchaAnimation.Play("GachaStep_1_Leave");
		yield return new WaitForSeconds(m_GatchaAnimation["GachaStep_1_Leave"].length);
		m_SkipCollider.SetActive(true);
		m_SponsoredRollRoot.SetActive(false);
		m_RainbowRiotNoticeRoot.SetActive(false);
		yield return new WaitForSeconds(m_PigMachineAnimation["GachaRoll"].length);
		m_GachaRoot.SetActive(false);
		m_ItemStatsRoot.SetActive(m_equipableItem || m_bannerItem);
		m_GainedRoot.SetActive(true);
		m_ResultRoot.SetActive(true);
		SetupGachaResultItems();
		m_ResultAnimation.Play((!m_bIsSetItem) ? "GachaStep_2_Enter" : "GachaStep_2_Enter_SetItem");
		int stars = 0;
		for (int i = 0; i < 3; i++)
		{
			m_StarBodySprites[i].spriteName = m_StarBodySprites[i].spriteName.Replace("_Desaturated", string.Empty);
			string animString = "ValueStar_GainedWithBonus";
			if (m_RainbowStars)
			{
				m_RainbowStarsOverlays[i].gameObject.SetActive(true);
				animString = "ValueStar_GainedWithBonus";
			}
			else
			{
				m_RainbowStarsOverlays[i].gameObject.SetActive(false);
				if (!m_starList[i])
				{
					m_StarBodySprites[i].spriteName = m_StarBodySprites[i].spriteName + "_Desaturated";
					animString = "ValueStar_GainedNormal";
				}
				else
				{
					stars++;
				}
			}
			m_StarAnimations[i].Play(animString);
		}
		if (m_RainbowStars)
		{
			if ((bool)m_SoundTriggers)
			{
				m_SoundTriggers.OnTriggerEventFired("result_star_rainbow");
			}
		}
		else if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("result_star_" + stars);
		}
		if (m_bIsSetItem)
		{
			m_SetItemDisplay.PlayGainedAnimation();
			m_OtherSetItemDisplay.PlayGainedAnimation();
		}
		else
		{
			m_ItemDisplay.PlayGainedAnimation();
		}
		yield return new WaitForSeconds(m_ResultAnimation["GachaStep_2_Enter"].clip.length);
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("gacha_finished", string.Empty);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("gacha_started");
		m_BackButton.Clicked -= OnBackButtonClicked;
		m_BackButton.Clicked += OnBackButtonClicked;
		m_SkipCollider.SetActive(false);
	}

	private void OnSkipClicked()
	{
		if (m_isMultiGacha)
		{
			RemoveEquipmentSprite(m_currentItem as EquipmentGameData);
			return;
		}
		StopCoroutine("StartGachaSequence");
		m_SkipCollider.SetActive(false);
		m_SponsoredRollRoot.SetActive(false);
		m_RainbowRiotNoticeRoot.SetActive(false);
		m_GachaRoot.SetActive(false);
		m_ItemStatsRoot.SetActive(m_equipableItem || m_bannerItem);
		m_GainedRoot.SetActive(true);
		m_ResultRoot.SetActive(true);
		SetupGachaResultItems();
		m_ResultAnimation.Play((!m_bIsSetItem) ? "GachaStep_2_EnterFast" : "GachaStep_2_EnterFast_SetItem");
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			m_StarBodySprites[i].spriteName = m_StarBodySprites[i].spriteName.Replace("_Desaturated", string.Empty);
			string animation = "ValueStar_GainedWithBonus";
			if (m_RainbowStars)
			{
				m_RainbowStarsOverlays[i].gameObject.SetActive(true);
				animation = "ValueStar_GainedWithBonus";
			}
			else
			{
				m_RainbowStarsOverlays[i].gameObject.SetActive(false);
				if (!m_starList[i])
				{
					m_StarBodySprites[i].spriteName = m_StarBodySprites[i].spriteName + "_Desaturated";
					animation = "ValueStar_GainedNormal";
				}
				else
				{
					num++;
				}
			}
			m_StarAnimations[i].Play(animation);
		}
		if (m_bIsSetItem)
		{
			m_SetItemDisplay.PlayGainedAnimation();
			m_OtherSetItemDisplay.PlayGainedAnimation();
		}
		else
		{
			m_ItemDisplay.PlayGainedAnimation();
		}
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("gacha_finished", string.Empty);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("gacha_started");
		m_BackButton.Clicked -= OnBackButtonClicked;
		m_BackButton.Clicked += OnBackButtonClicked;
	}

	private void SetupGachaResultItems()
	{
		List<IInventoryItemGameData> items = new List<IInventoryItemGameData>();
		if (m_currentItem is EquipmentGameData)
		{
			EquipmentGameData equipmentGameData = m_currentItem as EquipmentGameData;
			m_PerkSprite.spriteName = EquipmentGameData.GetPerkIcon(m_currentItem as EquipmentGameData);
			if (equipmentGameData != null && equipmentGameData.IsSetItem)
			{
				m_bIsSetItem = true;
				m_ItemDisplay.gameObject.SetActive(false);
				m_SetItemDisplay.gameObject.SetActive(true);
				EquipmentGameData equipmentGameData2 = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(equipmentGameData.ItemData.Level, equipmentGameData.ItemData.Quality, equipmentGameData.CorrespondingSetItem.NameId, 1) as EquipmentGameData;
				m_OtherSetItemDisplay.gameObject.SetActive(true);
				m_OtherSetItemDisplay.SetModel(equipmentGameData2, new List<IInventoryItemGameData>(), LootDisplayType.Missing);
				m_OtherSetItemLabel.gameObject.SetActive(true);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{value}", equipmentGameData2.ItemLocalizedName);
				Dictionary<string, string> replacementStrings = dictionary;
				m_OtherSetItemLabel.text = DIContainerInfrastructure.GetLocaService().Tr("gacha_lbl_collectsetitem", replacementStrings);
				m_SetItemDisplay.SetModel(m_currentItem, items, LootDisplayType.Set);
				UnityHelper.SetLayerRecusively(m_SetItemDisplay.gameObject, LayerMask.NameToLayer("Interface"));
			}
			else
			{
				m_bIsSetItem = false;
				m_ItemDisplay.gameObject.SetActive(true);
				m_SetItemDisplay.gameObject.SetActive(false);
				m_OtherSetItemDisplay.gameObject.SetActive(false);
				m_OtherSetItemLabel.gameObject.SetActive(false);
				m_ItemDisplay.SetModel(m_currentItem, items, LootDisplayType.Minor);
			}
		}
		else if (m_currentItem is BannerItemGameData)
		{
			BannerItemGameData bannerItemGameData = m_currentItem as BannerItemGameData;
			string perkIcon = EquipmentGameData.GetPerkIcon(bannerItemGameData);
			if (perkIcon == "Character_Health_Large")
			{
				perkIcon = bannerItemGameData.PrimarySkill.m_SkillIconName;
				m_PerkObjectSprite.SetActive(false);
				m_PerkSkillObjectSprite.SetActive(true);
				m_PerkSkillSprite.spriteName = perkIcon;
			}
			else
			{
				m_PerkObjectSprite.SetActive(true);
				m_PerkSkillObjectSprite.SetActive(false);
				m_PerkSprite.spriteName = perkIcon;
			}
			if (bannerItemGameData != null && bannerItemGameData.IsSetItem)
			{
				m_bIsSetItem = true;
				m_ItemDisplay.gameObject.SetActive(false);
				m_SetItemDisplay.gameObject.SetActive(true);
				BannerItemGameData bannerItemGameData2 = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(bannerItemGameData.ItemData.Level, bannerItemGameData.ItemData.Quality, bannerItemGameData.CorrespondingSetItem.NameId, 1) as BannerItemGameData;
				m_OtherSetItemDisplay.gameObject.SetActive(true);
				m_OtherSetItemDisplay.SetModel(bannerItemGameData2, new List<IInventoryItemGameData>(), LootDisplayType.Missing);
				m_OtherSetItemLabel.gameObject.SetActive(true);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("{value}", bannerItemGameData2.ItemLocalizedName);
				Dictionary<string, string> replacementStrings2 = dictionary;
				m_OtherSetItemLabel.text = DIContainerInfrastructure.GetLocaService().Tr("gacha_lbl_collectsetitem", replacementStrings2);
				m_SetItemDisplay.SetModel(m_currentItem, items, LootDisplayType.Set);
				UnityHelper.SetLayerRecusively(m_SetItemDisplay.gameObject, LayerMask.NameToLayer("Interface"));
			}
			else
			{
				m_bIsSetItem = false;
				m_ItemDisplay.gameObject.SetActive(true);
				m_SetItemDisplay.gameObject.SetActive(false);
				m_OtherSetItemDisplay.gameObject.SetActive(false);
				m_OtherSetItemLabel.gameObject.SetActive(false);
				m_ItemDisplay.SetModel(m_currentItem, items, LootDisplayType.Minor);
			}
		}
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("entered_gacha_result", null);
		if (m_equipableItem || m_bannerItem)
		{
			switch (m_currentItem.ItemBalancing.ItemType)
			{
			case InventoryItemType.MainHandEquipment:
				m_StatsWithChangeIndicator.SetIconSprite("Character_Damage_Large");
				m_StatsWithChangeIndicator.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(m_currentItem as EquipmentGameData, m_currentItem.ItemData.Quality), (m_PossibleBird == null) ? 0f : m_PossibleBird.MainHandItem.ItemMainStat);
				break;
			case InventoryItemType.OffHandEquipment:
				m_StatsWithChangeIndicator.SetIconSprite("Character_Health_Large");
				m_StatsWithChangeIndicator.RefreshStat(false, true, EquipmentGameData.GetItemMainStat(m_currentItem as EquipmentGameData, m_currentItem.ItemData.Quality), (m_PossibleBird == null) ? 0f : m_PossibleBird.OffHandItem.ItemMainStat);
				break;
			case InventoryItemType.Banner:
				m_StatsWithChangeIndicator.SetIconSprite("Character_Health_Large");
				m_StatsWithChangeIndicator.RefreshStat(false, true, BannerItemGameData.GetItemMainStat(m_currentItem as BannerItemGameData, m_currentItem.ItemData.Quality), (m_banner == null) ? 0f : m_banner.BannerCenter.ItemMainStat);
				break;
			case InventoryItemType.BannerTip:
				m_StatsWithChangeIndicator.SetIconSprite("Character_Health_Large");
				m_StatsWithChangeIndicator.RefreshStat(false, true, BannerItemGameData.GetItemMainStat(m_currentItem as BannerItemGameData, m_currentItem.ItemData.Quality), (m_banner == null) ? 0f : m_banner.BannerTip.ItemMainStat);
				break;
			case InventoryItemType.BannerEmblem:
				m_StatsWithChangeIndicator.SetIconSprite("Character_Health_Large");
				m_StatsWithChangeIndicator.RefreshStat(false, true, BannerItemGameData.GetItemMainStat(m_currentItem as BannerItemGameData, m_currentItem.ItemData.Quality), (m_banner == null) ? 0f : m_banner.BannerEmblem.ItemMainStat);
				break;
			}
		}
		if ((m_equipableItem && m_PossibleBird != null) || (m_bannerItem && m_banner != null))
		{
			m_equipCharacter.gameObject.SetActive(true);
			SetLayerRecusively(m_equipCharacter.gameObject, 8);
			m_equipCharacter.m_AssetController.PlayIdleAnimation();
			StartCoroutine("AnimateBird");
		}
		if (m_equipableItem || m_bannerItem)
		{
			if (m_PossibleBird != null)
			{
				SpawnComparisonBubble();
				float num = ((m_currentItem.ItemBalancing.ItemType != InventoryItemType.MainHandEquipment) ? m_PossibleBird.OffHandItem.ItemMainStat : m_PossibleBird.MainHandItem.ItemMainStat);
				if (num >= m_currentItem.ItemMainStat)
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
			else if (m_banner != null)
			{
				SpawnComparisonBubble();
				float num2 = 0f;
				num2 = ((m_currentItem.ItemBalancing.ItemType == InventoryItemType.Banner) ? m_banner.BannerCenter.ItemMainStat : ((m_currentItem.ItemBalancing.ItemType != InventoryItemType.BannerEmblem) ? m_banner.BannerTip.ItemMainStat : m_banner.BannerEmblem.ItemMainStat));
				if (num2 >= m_currentItem.ItemMainStat)
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
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		DIContainerLogic.RateAppController.RequestRatePopupForReason(RatePopupTrigger.SetItemGained);
	}

	private IEnumerator UpdateAndLeaveCoins()
	{
		yield return new WaitForSeconds(DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar());
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u
		}, true);
	}

	private IEnumerator EnterCoroutine(bool enterBackground = true)
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("gacha_enter");
		m_ItemStatsRoot.SetActive(false);
		m_ResultRoot.SetActive(false);
		m_GainedRoot.SetActive(false);
		m_SetItemProgress.SetActive(!ClientInfo.IsFriend);
		m_ItemDisplay.gameObject.SetActive(false);
		m_OtherSetItemDisplay.gameObject.SetActive(false);
		m_SetItemDisplay.gameObject.SetActive(false);
		m_NewSetIndicator.SetActive(!DIContainerInfrastructure.GetCurrentPlayer().Data.SetInfoDisplayed);
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.SetItemsInTotal < Enumerable.ToList(from b in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
			where !string.IsNullOrEmpty(b.CorrespondingSetItemId)
			select b).Count)
		{
			m_NewSetIndicator.SetActive(true);
		}
		m_PigMachineRoot.gameObject.SetActive(true);
		m_GachaRoot.SetActive(true);
		if (!ClientInfo.IsFriend && DIContainerLogic.GetShopService().IsRainbowRiotRunning(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			m_RiotTimerRoot.SetActive(true);
			m_RainbowRiotEffect.gameObject.SetActive(true);
			m_RainbowRiotEffect.Play();
			m_GachaHeader.text = DIContainerInfrastructure.GetLocaService().Tr("gachapopup_riot_header");
			IInventoryItemGameData m_LeagueItemGameData2;
			if (m_arenaGacha && DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "pvp_league_crown_max", out m_LeagueItemGameData2))
			{
				m_GachaHeaderCrownIcon.gameObject.SetActive(false);
			}
			else
			{
				m_GachaHeaderCrownIcon.gameObject.SetActive(false);
			}
			StopCoroutine("StartRiotTimer");
			StartCoroutine("StartRiotTimer", DIContainerLogic.GetShopService().GetRainbowRiotEndTime(DIContainerInfrastructure.GetCurrentPlayer()));
		}
		else
		{
			IInventoryItemGameData m_LeagueItemGameData;
			if (m_arenaGacha && DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "pvp_league_crown_max", out m_LeagueItemGameData))
			{
				PvPSeasonManagerGameData seasondata = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
				string seasonName = DIContainerInfrastructure.GetLocaService().Tr(seasondata.Balancing.LocaBaseId + "_name");
				string turnName = DIContainerInfrastructure.GetLocaService().GetLeagueName(m_LeagueItemGameData.ItemData.Level);
				Dictionary<string, string> replacementDic = new Dictionary<string, string>
				{
					{ "{value_1}", turnName },
					{ "{value_2}", seasonName }
				};
				m_GachaHeader.text = DIContainerInfrastructure.GetLocaService().Tr("gachapvppopup_header", replacementDic);
				m_GachaHeaderCrownIcon.gameObject.SetActive(true);
			}
			else
			{
				m_GachaHeader.text = DIContainerInfrastructure.GetLocaService().Tr("gachapopup_header");
				m_GachaHeaderCrownIcon.gameObject.SetActive(false);
			}
			m_RiotTimerRoot.SetActive(false);
			m_RainbowRiotEffect.gameObject.SetActive(false);
		}
		if (m_arenaGacha)
		{
			Requirement gachaRequirementCost2 = DIContainerLogic.GetShopService().GetGachaOffer(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), m_isAdvancedGacha).BuyRequirements.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem);
			if (gachaRequirementCost2 != null)
			{
				m_GachaCost.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(gachaRequirementCost2.NameId).AssetBaseId, null, gachaRequirementCost2.Value, string.Empty);
			}
			BasicShopOfferBalancingData offer = DIContainerLogic.GetShopService().GetGachaOffer(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), m_isAdvancedGacha, true);
			Requirement gachaRequirementHighCost2 = offer.BuyRequirements.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem);
			if (gachaRequirementHighCost2 != null)
			{
				m_GachaCostHigh.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(gachaRequirementHighCost2.NameId).AssetBaseId, null, gachaRequirementHighCost2.Value, string.Empty);
			}
			int HighGachaAmount2 = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").MultiGachaAmount;
			m_PigMachineHighLabel.text = HighGachaAmount2.ToString();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 2u
			}, true);
			if (!ClientInfo.IsFriend)
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
				{
					Depth = 2u,
					showLuckyCoins = true,
					showSnoutlings = false
				}, true);
			}
		}
		else
		{
			Requirement gachaRequirementCost = DIContainerLogic.GetShopService().GetGachaOffer(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), m_isAdvancedGacha).BuyRequirements.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem);
			if (gachaRequirementCost != null)
			{
				m_GachaCost.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(gachaRequirementCost.NameId).AssetBaseId, null, gachaRequirementCost.Value, string.Empty);
			}
			Requirement gachaRequirementHighCost = DIContainerLogic.GetShopService().GetGachaOffer(m_arenaGacha, DIContainerInfrastructure.GetCurrentPlayer(), m_isAdvancedGacha, true).BuyRequirements.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem);
			if (gachaRequirementHighCost != null)
			{
				m_GachaCostHigh.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(gachaRequirementHighCost.NameId).AssetBaseId, null, gachaRequirementHighCost.Value, string.Empty);
			}
			int HighGachaAmount = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").MultiGachaAmount;
			m_PigMachineHighLabel.text = HighGachaAmount.ToString();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 2u
			}, true);
			if (!ClientInfo.IsFriend)
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
				{
					Depth = 2u,
					showLuckyCoins = true
				}, true);
			}
		}
		if (enterBackground)
		{
			if (m_arenaGacha)
			{
				m_SetProgressBar.fillAmount = GetPvpSetProgress();
			}
			else
			{
				m_SetProgressBar.fillAmount = GetSetProgress();
			}
			DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.Enter();
			m_Arrow.transform.localRotation = m_ArrowStartRotation;
			float offset = 0.1f + m_SetProgressBar.fillAmount * 1.5f;
			if (offset > 1.1f)
			{
				offset = 1.1f;
			}
			float currentRotation = m_ArrowRotationInDegrees * (m_SetProgressBar.fillAmount * offset);
			if (currentRotation > m_ArrowRotationInDegrees)
			{
				currentRotation = m_ArrowRotationInDegrees;
			}
			m_Arrow.transform.Rotate(0f, 0f, currentRotation);
			m_Arrow.GetComponent<UISprite>().height = (int)((float)m_ArrowStartHeight * Mathf.Abs(Mathf.Sin(m_Arrow.transform.localRotation.eulerAngles.z * (float)Math.PI / 180f)));
			if (m_arenaGacha)
			{
				m_SetProgressBar.fillAmount = GetPvpSetProgress();
			}
			else
			{
				m_SetProgressBar.fillAmount = GetSetProgress();
			}
		}
		m_OldProgress = m_SetProgressBar.fillAmount;
		if ((m_GachaOffer != null && m_GachaOffer.NameId.Contains("_free_")) || IsFreeFriendGacha())
		{
			m_FreeRollIndicator.SetActive(true);
			m_GachaCost.gameObject.SetActive(false);
		}
		else
		{
			m_FreeRollIndicator.SetActive(false);
			m_GachaCost.gameObject.SetActive(true);
		}
		if (!ClientInfo.IsFriend && DIContainerLogic.GetShopService().IsRainbowRiotRunning(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			m_RainbowRiotNoticeRoot.SetActive(true);
			m_RainbowRiotNoticeRoot.GetComponent<Animation>().Play("RainbowRiotMarker_Enter");
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.IsExtraRainbowRiot)
			{
				m_RainbowRiotNoticeSprite.spriteName = "Hand_RainbowRiotB";
				m_RainbowRiotNoticeRoot.GetComponentInChildren<UILabel>().text = DIContainerInfrastructure.GetLocaService().Tr("rainbowriot_hand_desc").Replace("{value_1}", DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot2Multi.ToString());
			}
			else
			{
				m_RainbowRiotNoticeSprite.spriteName = "Hand_RainbowRiotA";
				m_RainbowRiotNoticeRoot.GetComponentInChildren<UILabel>().text = DIContainerInfrastructure.GetLocaService().Tr("rainbowriot_hand_desc").Replace("{value_1}", DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RainbowRiot1Multi.ToString());
			}
		}
		else
		{
			m_RainbowRiotNoticeRoot.SetActive(false);
		}
		m_GatchaAnimation.Play("GachaStep_1_Enter");
		yield return new WaitForSeconds(m_GatchaAnimation.GetComponent<Animation>()["GachaStep_1_Enter"].length);
		if (!ClientInfo.IsFriend && DIContainerLogic.GetShopService().IsRainbowRiotRunning(DIContainerInfrastructure.GetCurrentPlayer()) && (bool)m_PigMachineAnimation)
		{
			m_PigMachineAnimation.Play("RainbowRiot");
		}
		if (!enterBackground && !ClientInfo.IsFriend)
		{
			if (m_arenaGacha)
			{
				m_PreviewProgressBar.fillAmount = GetPvpSetProgress();
				yield return StartCoroutine(UpdateProgressBar(m_SetProgressBar, m_OldProgress, GetPvpSetProgress(), DIContainerLogic.GetPacingBalancing().CraftingTimeForTillStarAppearance));
			}
			else
			{
				m_PreviewProgressBar.fillAmount = GetSetProgress();
				yield return StartCoroutine(UpdateProgressBar(m_SetProgressBar, m_OldProgress, GetSetProgress(), DIContainerLogic.GetPacingBalancing().CraftingTimeForTillStarAppearance));
			}
		}
		m_BackButton.gameObject.SetActive(true);
		m_PigMachineButton.gameObject.SetActive(true);
		if (!ClientInfo.IsFriend)
		{
			m_PigMachineHighButton.gameObject.SetActive(true);
		}
		else
		{
			m_PigMachineHighButton.gameObject.SetActive(false);
		}
		m_GatchaAnimation.Play("GachaStep_1_Enter_Buttons");
		yield return new WaitForSeconds(m_GatchaAnimation.GetComponent<Animation>()["GachaStep_1_Enter_Buttons"].length - 0.15f);
		if (!ClientInfo.IsFriend && !m_arenaGacha)
		{
			List<IMailboxMessageGameData> gachaResponseMails = Enumerable.ToList(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MailboxMessages.Values.Where((IMailboxMessageGameData m) => m is ResponseGachaUseMessage));
			for (int j = 0; j < gachaResponseMails.Count; j++)
			{
				ResponseGachaUseMessage gachaMessage = gachaResponseMails[j] as ResponseGachaUseMessage;
				if (gachaMessage != null && !gachaMessage.IsUsed)
				{
					Vector3 screenPosToStartFrom2 = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera.ScreenToWorldPoint(new Vector3((UnityEngine.Random.Range(0, 2) >= 1) ? Screen.width : 0, (float)Screen.height * UnityEngine.Random.value, 0f));
					FriendInfoElement gachaFriendIcon2 = UnityEngine.Object.Instantiate(position: new Vector3(screenPosToStartFrom2.x, screenPosToStartFrom2.y, base.transform.position.z - 10f), original: m_FlyingFriendIcon, rotation: Quaternion.identity) as FriendInfoElement;
					gachaFriendIcon2.SetDefault();
					gachaFriendIcon2.SetModel(gachaMessage.Sender);
					CHMotionTween friendMotion2 = gachaFriendIcon2.GetComponent<CHMotionTween>();
					if ((bool)friendMotion2)
					{
						friendMotion2.m_EndTransform = m_SetProgressBar.transform;
						friendMotion2.Play();
						yield return new WaitForSeconds(friendMotion2.m_DurationInSeconds);
						UnityEngine.Object.Destroy(gachaFriendIcon2.gameObject);
					}
					if (gachaMessage.UseMessageContent(DIContainerInfrastructure.GetCurrentPlayer(), delegate(bool result, IMailboxMessageGameData message)
					{
						DebugLog.Log("Free Gacha Message used: " + result);
					}))
					{
						m_OldProgress = m_SetProgressBar.fillAmount;
						yield return StartCoroutine(UpdateProgressBar(m_SetProgressBar, m_OldProgress, GetSetProgress(), DIContainerLogic.GetPacingBalancing().CraftingTimeForTillStarAppearance));
					}
				}
			}
		}
		if (!ClientInfo.IsFriend && m_arenaGacha)
		{
			List<IMailboxMessageGameData> pvpGachaResponseMails = Enumerable.ToList(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MailboxMessages.Values.Where((IMailboxMessageGameData m) => m is ResponsePvpGachaUseMessage));
			for (int i = 0; i < pvpGachaResponseMails.Count; i++)
			{
				ResponsePvpGachaUseMessage pvpGachaMessage = pvpGachaResponseMails[i] as ResponsePvpGachaUseMessage;
				if (pvpGachaMessage != null && !pvpGachaMessage.IsUsed)
				{
					Vector3 screenPosToStartFrom = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera.ScreenToWorldPoint(new Vector3((UnityEngine.Random.Range(0, 2) >= 1) ? Screen.width : 0, (float)Screen.height * UnityEngine.Random.value, 0f));
					FriendInfoElement gachaFriendIcon = UnityEngine.Object.Instantiate(position: new Vector3(screenPosToStartFrom.x, screenPosToStartFrom.y, base.transform.position.z - 10f), original: m_FlyingFriendIcon, rotation: Quaternion.identity) as FriendInfoElement;
					gachaFriendIcon.SetDefault();
					gachaFriendIcon.SetModel(pvpGachaMessage.Sender);
					CHMotionTween friendMotion = gachaFriendIcon.GetComponent<CHMotionTween>();
					if ((bool)friendMotion)
					{
						friendMotion.m_EndTransform = m_SetProgressBar.transform;
						friendMotion.Play();
						yield return new WaitForSeconds(friendMotion.m_DurationInSeconds);
						UnityEngine.Object.Destroy(gachaFriendIcon.gameObject);
					}
					if (pvpGachaMessage.UseMessageContent(DIContainerInfrastructure.GetCurrentPlayer(), delegate(bool result, IMailboxMessageGameData message)
					{
						DebugLog.Log("Free Gacha Message used: " + result);
					}))
					{
						m_OldProgress = m_SetProgressBar.fillAmount;
						yield return StartCoroutine(UpdateProgressBar(m_SetProgressBar, m_OldProgress, GetPvpSetProgress(), DIContainerLogic.GetPacingBalancing().CraftingTimeForTillStarAppearance));
					}
				}
			}
		}
		if (m_SetProgressBar.fillAmount == 1f && !ClientInfo.IsFriend)
		{
			m_RainbowStarAnimation.Play("RainbowStar_Active");
		}
		else
		{
			m_RainbowStarAnimation.Play("RainbowStar_Inactive");
		}
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("gacha_entered", string.Empty);
		yield return new WaitForSeconds(0.05f);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("gacha_enter");
		RegisterEventHandler();
	}

	private bool IsFreeFriendGacha()
	{
		if (!ClientInfo.IsFriend || ClientInfo.InspectedFriend == null)
		{
			return false;
		}
		if (!m_arenaGacha && DIContainerLogic.SocialService.HasFreeGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer()))
		{
			return true;
		}
		if (m_arenaGacha && DIContainerLogic.SocialService.HasFreePvpGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer()))
		{
			return true;
		}
		return false;
	}

	private IEnumerator StartRiotTimer(DateTime targetTime)
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
				m_RiotTimer.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft);
			}
			yield return new WaitForSeconds(1f);
		}
		StartCoroutine(ReEnterPopup());
	}

	private void SpawnScrappingBubble()
	{
		List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
		EquipmentGameData equipmentGameData = m_currentItem as EquipmentGameData;
		if (equipmentGameData != null && equipmentGameData.GetScrapLoot() != null)
		{
			list = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(equipmentGameData.GetScrapLoot(), 0));
		}
		if (list.Count == 0)
		{
			return;
		}
		m_ScrapInfoShown = true;
		float num = ((m_PossibleBird == null) ? 0f : (m_currentItem.ItemMainStat - m_PossibleBird.MainHandItem.ItemMainStat));
		m_ScrapInfoAnmiation.Play("ScrapInfo_Show");
		if (num <= 0f)
		{
			m_ScrapInfoAnmiation.PlayQueued("ScrapInfo_Focus");
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
			if (m_currentItem.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment)
			{
				if (Math.Abs(m_PossibleBird.MainHandItem.ItemMainStat - m_currentItem.ItemMainStat) < 0.01f)
				{
					return;
				}
				m_BubbleStats.gameObject.SetActive(true);
				m_BubbleStats.SetComparisionValues("Character_Damage_Large", InventoryItemType.MainHandEquipment, m_PossibleBird.MainHandItem.ItemMainStat, m_PossibleBird.MainHandItem.BalancingData.Perk.Type);
			}
			else
			{
				if (Math.Abs(m_PossibleBird.OffHandItem.ItemMainStat - m_currentItem.ItemMainStat) < 0.01f)
				{
					return;
				}
				m_BubbleStats.gameObject.SetActive(true);
				m_BubbleStats.SetComparisionValues("Character_Health_Large", InventoryItemType.OffHandEquipment, m_PossibleBird.OffHandItem.ItemMainStat, m_PossibleBird.OffHandItem.BalancingData.Perk.Type);
			}
			m_equipCharacter.PositionComparisionBubble(m_equipCharacter, m_BubbleStats.gameObject);
			m_BubbleStats.Show();
		}
		else
		{
			if (!m_bannerItem)
			{
				return;
			}
			BannerGameData bannerGameData = DIContainerInfrastructure.GetCurrentPlayer().BannerGameData;
			if (m_currentItem.ItemBalancing.ItemType == InventoryItemType.Banner)
			{
				if (Math.Abs(bannerGameData.BannerCenter.ItemMainStat - m_currentItem.ItemMainStat) < 0.01f)
				{
					return;
				}
				m_BubbleStats.gameObject.SetActive(true);
				m_BubbleStats.SetComparisionValues("Character_Health_Large", InventoryItemType.Banner, bannerGameData.BannerCenter.ItemMainStat, bannerGameData.BannerCenter.GetPerkTypeOfSkill());
			}
			else if (m_currentItem.ItemBalancing.ItemType == InventoryItemType.BannerEmblem)
			{
				if (Math.Abs(bannerGameData.BannerEmblem.ItemMainStat - m_currentItem.ItemMainStat) < 0.01f)
				{
					return;
				}
				m_BubbleStats.gameObject.SetActive(true);
				m_BubbleStats.SetComparisionValues("Character_Health_Large", InventoryItemType.BannerEmblem, bannerGameData.BannerEmblem.ItemMainStat, bannerGameData.BannerEmblem.GetPerkTypeOfSkill());
			}
			else
			{
				if (Math.Abs(bannerGameData.BannerTip.ItemMainStat - m_currentItem.ItemMainStat) < 0.01f)
				{
					return;
				}
				m_BubbleStats.gameObject.SetActive(true);
				m_BubbleStats.SetComparisionValues("Character_Health_Large", InventoryItemType.BannerTip, bannerGameData.BannerTip.ItemMainStat, bannerGameData.BannerTip.GetPerkTypeOfSkill());
			}
			m_equipCharacter.PositionComparisionBubble(m_equipCharacter, m_BubbleStats.gameObject);
			m_BubbleStats.Show();
		}
	}

	private void CreateEquipButtonBird(BirdGameData birdData)
	{
		if (m_equipCharacter != null)
		{
			m_equipCharacter.DestroyCharacter();
			m_birdAnimation = null;
		}
		m_equipCharacter = UnityEngine.Object.Instantiate(m_CampViewController, m_EquipButton.transform.position, m_EquipButton.transform.rotation) as CharacterControllerCamp;
		m_equipCharacter.transform.parent = m_CharacterRoot.transform;
		m_equipCharacter.transform.localPosition = Vector3.zero;
		m_equipCharacter.gameObject.SetActive(true);
		if (birdData == null)
		{
			m_equipCharacter.SetModel(DIContainerInfrastructure.GetCurrentPlayer().BannerGameData, false);
			m_equipCharacter.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		else
		{
			m_equipCharacter.SetModel(birdData, false);
		}
		m_equipCharacter.DisableTabAndHold();
		m_equipCharacter.gameObject.SetActive(false);
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
		float minWaitTime2 = 0f;
		if (m_equipableItem)
		{
			if (m_birdAnimation == null)
			{
				m_birdAnimation = m_equipCharacter.m_AssetController.m_BoneAnimation;
			}
			if (m_birdAnimation == null)
			{
				yield break;
			}
			minWaitTime2 = m_birdAnimation["Cheer"].length + m_birdAnimation["Idle"].length;
		}
		else
		{
			if (m_bannerAnimation == null)
			{
				m_bannerAnimation = m_equipCharacter.m_AssetController.GetComponent<Animator>();
			}
			if (m_bannerAnimation == null)
			{
				yield break;
			}
			minWaitTime2 = 15.625f;
		}
		while (true)
		{
			if (m_betterItem && m_equipableItem)
			{
				m_birdAnimation.CrossFade("Cheer");
				m_birdAnimation.CrossFadeQueued("Idle");
			}
			else if (m_betterItem)
			{
				m_bannerAnimation.Play("Affected");
				m_bannerAnimation.PlayAnimatorStatesQueued(new List<string> { "Idle" });
			}
			yield return new WaitForSeconds(UnityEngine.Random.Range(minWaitTime2, minWaitTime2 * 2f));
		}
	}

	public void SetItem(IInventoryItemGameData newItem, int starCount)
	{
		m_currentItem = newItem;
		m_equipableItem = m_currentItem is EquipmentGameData;
		m_bannerItem = m_currentItem is BannerItemGameData;
		m_starList.Clear();
		for (int i = 0; i < 3; i++)
		{
			m_starList.Add(false);
		}
		for (int j = 0; j < Mathf.Min(m_starList.Count, starCount); j++)
		{
			m_starList[j] = true;
		}
		if (starCount == 4)
		{
			m_RainbowStars = true;
		}
		else
		{
			m_RainbowStars = false;
		}
		if (m_equipableItem)
		{
			EquipmentGameData equipmentGameData = m_currentItem as EquipmentGameData;
			m_PossibleBird = DIContainerInfrastructure.GetCurrentPlayer().GetBird(equipmentGameData.BalancingData.RestrictedBirdId, true);
			m_ItemName.text = equipmentGameData.ItemLocalizedName;
			CreateEquipButtonBird(m_PossibleBird);
		}
		else
		{
			BannerItemGameData bannerItemGameData = m_currentItem as BannerItemGameData;
			m_ItemName.text = bannerItemGameData.ItemLocalizedName;
			CreateEquipButtonBird(null);
		}
	}

	private void AddEquipmentSprite(EquipmentGameData equip)
	{
		m_GainedRoot.transform.localScale = Vector3.one;
		switch (equip.BalancingData.ItemType)
		{
		case InventoryItemType.Class:
			m_EquipmentSprite = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(equip.ItemAssetName, m_GainedRoot.transform, Vector3.zero, Quaternion.identity, false);
			break;
		case InventoryItemType.MainHandEquipment:
			m_EquipmentSprite = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(equip.ItemAssetName, m_GainedRoot.transform, Vector3.zero, Quaternion.identity, false);
			break;
		case InventoryItemType.OffHandEquipment:
			m_EquipmentSprite = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(equip.ItemAssetName, m_GainedRoot.transform, Vector3.zero, Quaternion.identity, false);
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
			}
		}
	}

	private void ShowSetInfo()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_SetItemInfoUi.Show(!m_arenaGacha, m_GatchaAnimation);
		DIContainerInfrastructure.GetCurrentPlayer().Data.SetInfoDisplayed = true;
		DIContainerInfrastructure.GetCurrentPlayer().Data.SetItemsInTotal = (uint)Enumerable.ToList(from b in DIContainerBalancing.Service.GetBalancingDataList<EquipmentBalancingData>()
			where !string.IsNullOrEmpty(b.CorrespondingSetItemId)
			select b).Count;
		m_NewSetIndicator.SetActive(false);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
	}

	private IEnumerator GachaVideoCoroutine()
	{
		if (ClientInfo.IsFriend)
		{
			yield break;
		}
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		uint lastTimeStamp2 = 0u;
		lastTimeStamp2 = ((!m_arenaGacha) ? DIContainerInfrastructure.GetCurrentPlayer().Data.TimeStampOfLastVideoGacha : DIContainerInfrastructure.GetCurrentPlayer().Data.TimeStampOfLastVideoPvPGacha);
		m_GachaVideoObject.SetActive(false);
		m_GachaTimerObject.SetActive(true);
		m_AdPendingSpinner.SetActive(true);
		uint nextTimeStamp = lastTimeStamp2 + (uint)(DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").GachaVideoTimespan * 60);
		DateTime targetTime = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(nextTimeStamp);
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_GachaTimerText.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft);
				m_AdPendingSpinner.SetActive(false);
			}
			yield return new WaitForSeconds(1f);
		}
		if ((DIContainerInfrastructure.AdService.IsAdShowPossible(PVPGACHA_PLACEMENT) && m_arenaGacha) || (DIContainerInfrastructure.AdService.IsAdShowPossible(GACHA_PLACEMENT) && !m_arenaGacha))
		{
			m_GachaVideoObject.SetActive(true);
			m_GachaTimerObject.SetActive(false);
			yield break;
		}
		yield return new WaitForSeconds(1f);
		if (!m_arenaGacha && DIContainerInfrastructure.AdService.GetState(GACHA_PLACEMENT) == 4)
		{
			DIContainerInfrastructure.AdService.AddPlacement(GACHA_PLACEMENT);
		}
		else if (m_arenaGacha && DIContainerInfrastructure.AdService.GetState(PVPGACHA_PLACEMENT) == 4)
		{
			DIContainerInfrastructure.AdService.AddPlacement(PVPGACHA_PLACEMENT);
		}
		StartCoroutine(GachaVideoCoroutine());
	}
}
