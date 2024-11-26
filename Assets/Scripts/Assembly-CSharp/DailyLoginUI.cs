using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using Rcs;
using UnityEngine;

public class DailyLoginUI : MonoBehaviour
{
	[SerializeField]
	private UILabel m_Title;

	[SerializeField]
	private UIInputTrigger m_CloseButton;

	[SerializeField]
	private UIInputTrigger m_InfoButton;

	[SerializeField]
	private UIInputTrigger m_CloseInfoScreenButton;

	[SerializeField]
	private Animation m_InfoScreenAnimation;

	private GameObject[] m_DailyRewards;

	[SerializeField]
	private List<GameObject> m_DailyRewardBackgrounds;

	[SerializeField]
	private GameObject m_FacebookActiveObject;

	[SerializeField]
	private GameObject m_FacebookDeactiveObject;

	[SerializeField]
	private UIInputTrigger m_FacebookButton;

	[SerializeField]
	private LootDisplayContoller m_FlyingLuckyCoinPrefab;

	[SerializeField]
	private GameObject m_ButtonOpen;

	[SerializeField]
	private GameObject m_ButtonCurrent;

	[SerializeField]
	private GameObject m_ButtonNext;

	[SerializeField]
	private GameObject m_ButtonCompleted;

	[SerializeField]
	private GameObject m_ButtonOpenBling;

	[SerializeField]
	private GameObject m_ButtonCurrentBling;

	[SerializeField]
	private GameObject m_ButtonNextBling;

	[SerializeField]
	private UIGrid m_ContainerGrid;

	[SerializeField]
	private Animation m_videoRewardRoot;

	[SerializeField]
	private UIInputTrigger m_videoRewardButtonTrigger;

	[SerializeField]
	private GameObject m_redChestPrefab;

	[SerializeField]
	private GameObject m_yellowChestPrefab;

	[SerializeField]
	private GameObject m_whiteChestPrefab;

	[SerializeField]
	private GameObject m_blackChestPrefab;

	[SerializeField]
	private GameObject m_bluesChestPrefab;

	private bool m_isShowing;

	private bool m_stopShow;

	private bool m_enterComplete;

	private float m_lastAdCancelledTime;

	private float m_lastAdCompletedTime;

	private void Awake()
	{
		StartCoroutine(InitItemsOnce());
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_DailyLoginUi = this;
		DIContainerInfrastructure.AdService.AddPlacement(DailyLoginLogic.BUFF_PLACEMENT);
	}

	private void OnDisable()
	{
		m_enterComplete = false;
	}

	private IEnumerator InitItemsOnce()
	{
		DebugLog.Log(GetType(), "InitItemsOnce: starting delay of 1s...");
		m_DailyRewards = new GameObject[31];
		base.gameObject.SetActive(false);
		yield return new WaitForSeconds(1f);
		do
		{
			DebugLog.Log(GetType(), "InitItemsOnce: not yet initialized. another delay of .5s...");
			yield return new WaitForSeconds(0.5f);
		}
		while (!DIContainerLogic.DailyLoginLogic.IsDailyLoginInitialized());
		base.gameObject.SetActive(true);
		DateTime trustedTime;
		if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			m_Title.text = DIContainerInfrastructure.GetLocaService().Tr("daily_rewards_header_" + trustedTime.Month.ToString("00"));
		}
		yield return StartCoroutine(SetupGiftsWithDelay());
		if (!m_isShowing)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void ClearItemDisplayCache()
	{
		m_DailyRewards = new GameObject[31];
		for (int num = m_ContainerGrid.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(m_ContainerGrid.transform.GetChild(num).gameObject);
		}
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		SetDragControllerActive(false);
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, ClosePopup);
		m_CloseButton.Clicked += ClosePopup;
		m_InfoButton.Clicked += OpenInfoScreen;
		m_CloseInfoScreenButton.Clicked += CloseInfoScreen;
		m_FacebookButton.Clicked += OnFacebookButtonClicked;
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent += SocialWindowUIFacebookLoginSucceededEvent;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent += SocialWindowUIFacebookLoginFailedEvent;
		m_videoRewardButtonTrigger.Clicked += OnVideoButtonClicked;
		DIContainerInfrastructure.AdService.RewardResult += RewardSponsoredAdResult;
	}

	private void DeregisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		m_CloseButton.Clicked -= ClosePopup;
		m_InfoButton.Clicked -= OpenInfoScreen;
		m_CloseInfoScreenButton.Clicked -= CloseInfoScreen;
		m_FacebookButton.Clicked -= OnFacebookButtonClicked;
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= SocialWindowUIFacebookLoginSucceededEvent;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= SocialWindowUIFacebookLoginFailedEvent;
		m_videoRewardButtonTrigger.Clicked -= OnVideoButtonClicked;
		DIContainerInfrastructure.AdService.RewardResult -= RewardSponsoredAdResult;
	}

	public void ClosePopup()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(LeaveCoroutine());
		}
	}

	private void OpenInfoScreen()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, CloseInfoScreen);
		m_InfoScreenAnimation.gameObject.SetActive(true);
		m_InfoScreenAnimation.Play("Popup_Enter");
	}

	private void CloseInfoScreen()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
		m_InfoScreenAnimation.Play("Popup_Leave");
		Invoke("DeactiveInfoScreen", m_InfoScreenAnimation["Popup_Leave"].length);
	}

	private void DeactiveInfoScreen()
	{
		m_InfoScreenAnimation.gameObject.SetActive(false);
	}

	private IEnumerator LeaveCoroutine()
	{
		DeregisterEventHandler();
		CancelInvoke("UpdateTimers");
		float sponsoredRollLeaveTime = m_videoRewardRoot.gameObject.PlayAnimationOrAnimatorState("SponsoredRoll_Leave");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		base.gameObject.PlayAnimationOrAnimatorState("Popup_DailyQuest_Leave");
		if (DIContainerLogic.InventoryService.RewardDelayedRewards(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData))
		{
			if (DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
			{
				DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 0, DIContainerLogic.GetLootOperationService().GenerateLoot(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.BalancingData.FacebookDailyBonus, 1), "daily_facebook_bonus");
				DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			}
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
		}
		m_enterComplete = false;
		yield return new WaitForSeconds(Mathf.Max(1f, sponsoredRollLeaveTime));
		if (DIContainerInfrastructure.LocationStateMgr != null)
		{
			if (DIContainerInfrastructure.LocationStateMgr.WorldMenuUI != null)
			{
				DIContainerInfrastructure.LocationStateMgr.WorldMenuUI.ComeBackFromDailyLogin();
				WorldMapMenuUI worldMenu = DIContainerInfrastructure.LocationStateMgr.WorldMenuUI as WorldMapMenuUI;
				if (worldMenu != null)
				{
					worldMenu.CheckForNewGiftMarker();
				}
			}
			DIContainerInfrastructure.LocationStateMgr.StartCoroutine(DIContainerInfrastructure.LocationStateMgr.StoppablePopupCoroutine());
		}
		base.gameObject.SetActive(false);
	}

	public void Show()
	{
		m_isShowing = true;
		base.gameObject.SetActive(true);
		DIContainerLogic.DailyLoginLogic.m_popupShownThisSession = true;
		SetFacebookState();
		StartCoroutine(EnterCoroutine());
	}

	public void StopEnterCoroutine()
	{
		m_stopShow = true;
		if (m_enterComplete)
		{
			StartCoroutine(LeaveCoroutine());
		}
	}

	private IEnumerator EnterCoroutine()
	{
		m_stopShow = false;
		yield return new WaitForSeconds(0.1f);
		if (!DIContainerLogic.DailyLoginLogic.IsDailyLoginInitialized())
		{
			if (DIContainerInfrastructure.LocationStateMgr != null && DIContainerInfrastructure.LocationStateMgr.WorldMenuUI != null)
			{
				DIContainerInfrastructure.LocationStateMgr.WorldMenuUI.ComeBackFromDailyLogin();
			}
			m_isShowing = false;
			base.gameObject.SetActive(false);
			yield break;
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		m_Title.text = string.Empty;
		yield return StartCoroutine(SetupGiftsWithDelay());
		if (m_stopShow)
		{
			m_stopShow = false;
			yield break;
		}
		base.gameObject.PlayAnimationOrAnimatorState("Popup_DailyQuest_Enter");
		yield return base.gameObject.GetAnimationOrAnimatorStateLength("Popup_DailyQuest_Enter");
		RegisterEventHandler();
		InvokeRepeating("UpdateTimers", 0f, 10f);
		if (m_stopShow)
		{
			m_stopShow = false;
			StartCoroutine(LeaveCoroutine());
		}
		if (IsVideoRewardAvailable())
		{
			m_videoRewardRoot.gameObject.SetActive(true);
			m_videoRewardRoot.Play("SponsoredRoll_Enter");
		}
		else
		{
			m_videoRewardRoot.gameObject.SetActive(false);
		}
		m_enterComplete = true;
	}

	private void UpdateTimers()
	{
		DateTime trustedTime;
		if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			m_Title.text = DIContainerInfrastructure.GetLocaService().Tr("daily_rewards_header_" + trustedTime.Month.ToString("00"));
		}
		DIContainerLogic.DailyLoginLogic.CheckTimers(SetupGifts);
	}

	public void SetupGifts()
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(SetupGiftsWithDelay());
		}
	}

	private IEnumerator SetupGiftsWithDelay()
	{
		bool claimedToday = DIContainerLogic.DailyLoginLogic.m_ClaimedToday;
		int giftsClaimed = (int)DIContainerInfrastructure.GetCurrentPlayer().Data.GiftsClaimedThisMonth;
		DailyRewardItem[] dailyRewards = DIContainerLogic.DailyLoginLogic.m_RewardItems;
		for (int i = 0; i < 31; i++)
		{
			dailyRewards[i].m_State = GetRewardItemState(i, giftsClaimed, claimedToday);
			DailyLoginButton rewardButton = UpdateDailyRewardItemButton(dailyRewards[i], i, giftsClaimed);
			if (rewardButton != null)
			{
				yield return StartCoroutine(SwitchPrefabs(rewardButton, i));
			}
		}
		yield return new WaitForEndOfFrame();
		m_ContainerGrid.Reposition();
	}

	private DailyLoginButton UpdateDailyRewardItemButton(DailyRewardItem dailyRewardItem, int dayIndex, int giftsClaimed)
	{
		GameObject gameObject = null;
		Dictionary<string, LootInfoData> loot = dailyRewardItem.m_Loot;
		if (loot == null || loot.Count == 0)
		{
			m_DailyRewardBackgrounds[dayIndex].SetActive(false);
			return null;
		}
		m_DailyRewardBackgrounds[dayIndex].SetActive(true);
		if (m_DailyRewards[dayIndex] == null)
		{
			DebugLog.Log(GetType(), "CreateDailyRewardItemButton: no button found for day " + dayIndex);
			GameObject buttonPrefab = GetButtonPrefab(dailyRewardItem);
			gameObject = UnityEngine.Object.Instantiate(buttonPrefab, new Vector3(500f, 500f, 500f), m_ContainerGrid.transform.rotation) as GameObject;
		}
		else
		{
			DailyLoginButton dailyLoginButton = null;
			dailyLoginButton = m_DailyRewards[dayIndex].GetComponent<DailyLoginButton>();
			if (!(dailyLoginButton != null) || dailyRewardItem.m_State == dailyLoginButton.m_State)
			{
				if (dailyLoginButton.m_State == DailyLoginButtonState.NEXT)
				{
					dailyLoginButton.InitTimer();
					return null;
				}
				return null;
			}
			DebugLog.Log(GetType(), "CreateDailyRewardItemButton: replacing button for day " + dayIndex);
			GameObject buttonPrefab2 = GetButtonPrefab(dailyRewardItem);
			gameObject = UnityEngine.Object.Instantiate(buttonPrefab2, new Vector3(0f, 0f, 100f), m_ContainerGrid.transform.rotation) as GameObject;
		}
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(loot, EquipmentSource.DailyGift);
		if (itemsFromLoot == null || gameObject.GetComponentInChildren<LootDisplayContoller>() == null)
		{
			DebugLog.Error("Could not create item data from loot: " + loot.Keys.FirstOrDefault() + "   dayIndex: " + dayIndex);
			return null;
		}
		IInventoryItemGameData inventoryItemGameData = null;
		List<IInventoryItemGameData> list = null;
		string text = string.Empty;
		if (itemsFromLoot.Count == 1 && DIContainerBalancing.Service.GetBalancingData<LootTableBalancingData>(DIContainerLogic.DailyLoginLogic.GetRewardForDay(dayIndex + 1).FirstOrDefault().Key) == null)
		{
			inventoryItemGameData = itemsFromLoot.FirstOrDefault();
			inventoryItemGameData.ItemData.Quality = 4;
		}
		else
		{
			list = itemsFromLoot;
			EquipmentGameData equipmentGameData = itemsFromLoot.FirstOrDefault() as EquipmentGameData;
			if (equipmentGameData != null && equipmentGameData.IsSetItem)
			{
				text = equipmentGameData.BalancingData.RestrictedBirdId;
			}
		}
		LootDisplayType displayType = ((dailyRewardItem.m_State == DailyLoginButtonState.CURRENT) ? LootDisplayType.Minor : LootDisplayType.None);
		Transform transform = null;
		LootDisplayContoller componentInChildren = gameObject.GetComponentInChildren<LootDisplayContoller>();
		if (giftsClaimed <= dayIndex && DIContainerLogic.DailyLoginLogic.HighlightDay(dayIndex + 1))
		{
			displayType = LootDisplayType.Major;
		}
		if (!string.IsNullOrEmpty(text) && componentInChildren != null)
		{
			string value = string.Empty;
			Dictionary<int, string> calendarChestLootWon = DIContainerInfrastructure.GetCurrentPlayer().Data.CalendarChestLootWon;
			if (calendarChestLootWon != null)
			{
				calendarChestLootWon.TryGetValue(dayIndex + 1, out value);
			}
			if (dailyRewardItem.m_State != DailyLoginButtonState.COMPLETED || string.IsNullOrEmpty(value))
			{
				transform = GetSetChest(text);
				transform.parent = componentInChildren.GetItemRoot();
				componentInChildren.m_IconSprite.gameObject.SetActive(false);
				componentInChildren.m_AmountText.gameObject.SetActive(false);
				transform.localPosition = new Vector3(0f, 0f, -2f);
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
			}
			else
			{
				EquipmentGameData equipmentGameData2 = new EquipmentGameData(value);
				equipmentGameData2.ItemData.Level = DIContainerInfrastructure.GetCurrentPlayer().Data.Level + 2;
				equipmentGameData2.ItemData.Quality = 4;
				componentInChildren.SetModel(equipmentGameData2, list, displayType, "_Large", false, false, false, null, true);
			}
		}
		else
		{
			componentInChildren.SetModel(inventoryItemGameData, list, displayType, "_Large", false, false, false, null, true);
		}
		gameObject.GetComponent<DailyLoginButton>().Init(this, dayIndex + 1, list, transform, text);
		if (inventoryItemGameData is BannerItemGameData)
		{
			StartCoroutine(DelayedBannerReset(gameObject));
		}
		return gameObject.GetComponent<DailyLoginButton>();
	}

	private Transform GetSetChest(string chestId)
	{
		switch (chestId)
		{
		case "bird_red":
			return UnityEngine.Object.Instantiate(m_redChestPrefab).transform;
		case "bird_yellow":
			return UnityEngine.Object.Instantiate(m_yellowChestPrefab).transform;
		case "bird_white":
			return UnityEngine.Object.Instantiate(m_whiteChestPrefab).transform;
		case "bird_black":
			return UnityEngine.Object.Instantiate(m_blackChestPrefab).transform;
		case "bird_blue":
			return UnityEngine.Object.Instantiate(m_bluesChestPrefab).transform;
		default:
			return UnityEngine.Object.Instantiate(m_redChestPrefab).transform;
		}
	}

	private IEnumerator SwitchPrefabs(DailyLoginButton newItem, int itemIndex)
	{
		GameObject oldItem = m_DailyRewards[itemIndex];
		m_DailyRewards[itemIndex] = newItem.gameObject;
		newItem.transform.parent = m_ContainerGrid.transform;
		newItem.transform.localScale = Vector3.one;
		newItem.transform.localPosition = ((!oldItem) ? new Vector3(newItem.transform.localPosition.x, newItem.transform.localPosition.y, 1f) : oldItem.transform.localPosition);
		if (newItem.transform.GetSiblingIndex() != itemIndex)
		{
			newItem.transform.SetSiblingIndex(itemIndex);
		}
		if (oldItem != null)
		{
			UnityEngine.Object.Destroy(oldItem);
		}
		yield break;
	}

	private GameObject GetButtonPrefab(DailyRewardItem dailyRewardItem)
	{
		bool flag = DIContainerLogic.DailyLoginLogic.HighlightDay(dailyRewardItem.m_Day);
		switch (dailyRewardItem.m_State)
		{
		case DailyLoginButtonState.COMPLETED:
			return m_ButtonCompleted;
		case DailyLoginButtonState.CURRENT:
			return (!flag) ? m_ButtonCurrent : m_ButtonCurrentBling;
		case DailyLoginButtonState.NEXT:
			return (!flag) ? m_ButtonNext : m_ButtonNextBling;
		case DailyLoginButtonState.OPEN:
			return (!flag) ? m_ButtonOpen : m_ButtonOpenBling;
		default:
			return null;
		}
	}

	private DailyLoginButtonState GetRewardItemState(int dayIndex, int giftsClaimed, bool claimedToday)
	{
		if (giftsClaimed > dayIndex)
		{
			return DailyLoginButtonState.COMPLETED;
		}
		if (giftsClaimed == dayIndex && !claimedToday)
		{
			return DailyLoginButtonState.CURRENT;
		}
		if (giftsClaimed == dayIndex && claimedToday)
		{
			return DailyLoginButtonState.NEXT;
		}
		return DailyLoginButtonState.OPEN;
	}

	private IEnumerator DelayedBannerReset(GameObject item)
	{
		yield return new WaitForEndOfFrame();
		item.GetComponentInChildren<BannerFlagAssetController>().transform.localPosition = new Vector3(0f, 43f, 0f);
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	private void SetFacebookState()
	{
		if (DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			UIPlayAnimation[] componentsInChildren = m_FacebookButton.GetComponentsInChildren<UIPlayAnimation>();
			UIPlayAnimation[] array = componentsInChildren;
			foreach (UIPlayAnimation uIPlayAnimation in array)
			{
				uIPlayAnimation.enabled = false;
			}
			m_FacebookActiveObject.gameObject.SetActive(true);
			m_FacebookDeactiveObject.gameObject.SetActive(false);
		}
		else
		{
			UIPlayAnimation[] componentsInChildren2 = m_FacebookButton.GetComponentsInChildren<UIPlayAnimation>();
			UIPlayAnimation[] array2 = componentsInChildren2;
			foreach (UIPlayAnimation uIPlayAnimation2 in array2)
			{
				uIPlayAnimation2.enabled = true;
			}
			m_FacebookActiveObject.gameObject.SetActive(false);
			m_FacebookDeactiveObject.gameObject.SetActive(true);
		}
	}

	public void ClaimGift()
	{
		bool flag = false;
		if (m_videoRewardRoot.gameObject.activeInHierarchy && !IsVideoRewardAvailable())
		{
			flag = true;
			StartCoroutine(LeaveVideoRewardOption());
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in DIContainerLogic.DailyLoginLogic.GetTodaysReward())
		{
			stringBuilder.Append(item);
			stringBuilder.Append("; ");
		}
		dictionary.Add("WasVideoReward", stringBuilder.ToString());
		dictionary.Add("GiftClaimed", flag.ToString());
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("DailyRewardClaimed", dictionary);
		if (DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 0, DIContainerLogic.GetLootOperationService().GenerateLoot(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.BalancingData.FacebookDailyBonus, 1), "daily_facebook_bonus");
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			StartCoroutine(UpdateLuckyCoinsBarAndForceAmount());
		}
		else
		{
			DIContainerLogic.InventoryService.RewardDelayedRewards(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData);
		}
	}

	private IEnumerator UpdateLuckyCoinsBarAndForceAmount()
	{
		LootDisplayContoller flyingCoin = UnityEngine.Object.Instantiate(m_FlyingLuckyCoinPrefab);
		flyingCoin.transform.parent = m_FacebookActiveObject.transform;
		flyingCoin.transform.localPosition = Vector3.zero;
		BasicItemGameData coin = new BasicItemGameData("lucky_coin");
		flyingCoin.SetModel(null, new List<IInventoryItemGameData> { coin }, LootDisplayType.None);
		List<LootDisplayContoller> explodedLoot = flyingCoin.Explode(true, false, 0f, true, 0f, 0f);
		foreach (LootDisplayContoller explodedItem in explodedLoot)
		{
			UnityEngine.Object.Destroy(explodedItem.gameObject, explodedItem.gameObject.GetComponent<Animation>().clip.length);
		}
		yield return new WaitForSeconds(DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar());
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.AddLuckyCoinsOnlyToUi(1);
	}

	private void SocialWindowUIFacebookLoginFailedEvent(string obj)
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_failed", "Facebook login failed!"), "facebook", DispatchMessage.Status.Error);
	}

	private void SocialWindowUIFacebookLoginSucceededEvent()
	{
		DIContainerInfrastructure.GetCoreStateMgr().StartRefreshFriends();
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_succes", "Facebook login succesfully!"), "facebook", DispatchMessage.Status.Info);
		SetFacebookState();
	}

	private void OnFacebookButtonClicked()
	{
		DebugLog.Log("FacebookSignInButtonClicked clicked!");
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("LinkClicked", new Dictionary<string, string>
		{
			{ "Button", "FacebookSignIn" },
			{ "Destination", "dailyQuest" }
		});
		if (!DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			DIContainerInfrastructure.GetCoreStateMgr().TryLoginOnFacebook();
		}
	}

	private bool IsVideoRewardAvailable()
	{
		return DIContainerLogic.DailyLoginLogic.IsVideoRewardAvailable();
	}

	private void RewardSponsoredAdResult(string placement, Ads.RewardResult result, string voucherId)
	{
		if (placement != DailyLoginLogic.BUFF_PLACEMENT)
		{
			return;
		}
		switch (result)
		{
		case Ads.RewardResult.RewardCanceled:
			m_lastAdCancelledTime = Time.time;
			break;
		case Ads.RewardResult.RewardCompleted:
			m_lastAdCompletedTime = Time.time;
			break;
		case Ads.RewardResult.RewardConfirmed:
			if (!(m_lastAdCancelledTime <= m_lastAdCompletedTime))
			{
				break;
			}
			if (m_DailyRewards.Length > DIContainerInfrastructure.GetCurrentPlayer().Data.GiftsClaimedThisMonth)
			{
				GameObject gameObject = m_DailyRewards[DIContainerInfrastructure.GetCurrentPlayer().Data.GiftsClaimedThisMonth];
				if (gameObject.GetComponent<DailyLoginButton>() != null)
				{
					gameObject.GetComponent<DailyLoginButton>().ClaimFromVideo();
				}
			}
			ClaimGift();
			break;
		case Ads.RewardResult.RewardFailed:
			break;
		}
	}

	private void OnVideoButtonClicked()
	{
		if (!DIContainerInfrastructure.AdService.IsAdShowPossible(DailyLoginLogic.BUFF_PLACEMENT))
		{
			DebugLog.Error(GetType(), "OnVideoButtonClicked: No Ad available! This button shouldn't even be here!");
			return;
		}
		if (!DIContainerInfrastructure.AdService.ShowAd(DailyLoginLogic.BUFF_PLACEMENT))
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_no_ad_available", "There is currently no Ad scheduled"), "no_ad", DispatchMessage.Status.Info);
			return;
		}
		m_videoRewardRoot.Play("SponsoredRoll_Leave");
		DIContainerInfrastructure.AdService.MutedGameSoundForPlacement(DailyLoginLogic.BUFF_PLACEMENT);
	}

	private DailyLoginButton GetNextButtonInLine()
	{
		int giftsClaimedThisMonth = (int)DIContainerInfrastructure.GetCurrentPlayer().Data.GiftsClaimedThisMonth;
		if (m_DailyRewards.Count() <= giftsClaimedThisMonth)
		{
			return null;
		}
		DailyLoginButton component = m_DailyRewards[giftsClaimedThisMonth].GetComponent<DailyLoginButton>();
		if (component == null)
		{
			return null;
		}
		return component;
	}

	private IEnumerator LeaveVideoRewardOption()
	{
		if (m_videoRewardRoot.gameObject.activeInHierarchy)
		{
			m_videoRewardButtonTrigger.Clicked -= OnVideoButtonClicked;
			yield return new WaitForSeconds(m_videoRewardRoot.gameObject.PlayAnimationOrAnimatorState("SponsoredRoll_Leave"));
			m_videoRewardRoot.gameObject.SetActive(false);
		}
	}
}
