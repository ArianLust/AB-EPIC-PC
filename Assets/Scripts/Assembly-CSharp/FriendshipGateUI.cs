using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.GameDatas.MailboxMessages;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Rcs;
using UnityEngine;

public class FriendshipGateUI : MonoBehaviour
{
	private const string BUFF_PLACEMENT = "RewardVideo.FriendshipGate";

	[SerializeField]
	private List<UIPanel> m_Panels;

	[SerializeField]
	private Animation m_BackAnimation;

	[SerializeField]
	private Animation m_HelpAnimation;

	[SerializeField]
	private Animation m_TooltipAnimation;

	[SerializeField]
	private Animation m_GateAnimation;

	[SerializeField]
	private Animation m_OpenAnimation;

	[SerializeField]
	private UIAtlas m_WorldMapElementsAtlas;

	[SerializeField]
	private GameObject m_OptionsRoot;

	[SerializeField]
	private UILabel m_AskFriendsText;

	[SerializeField]
	private UISprite m_AskFriendsButton;

	[SerializeField]
	private Animation m_TimerAnimation;

	[SerializeField]
	private UILabel m_TimerLabel;

	[SerializeField]
	private ResourceCostBlind m_BuyFriendsCost;

	[SerializeField]
	private List<FriendInfoElement> m_FriendInfos;

	[SerializeField]
	private List<AutoScalingTextBox> m_FriendBubbles;

	[SerializeField]
	private UIInputTrigger m_buyFriendsButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_sendHelpFriendsButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_backButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_DoorButtonTrigger;

	private BaseLocationStateManager m_worldMapStateMgr;

	private HotspotBalancingData m_hotspotBalancing;

	private HotspotGameData m_hotspotGameData;

	private bool m_locked;

	private Action m_returnAction;

	[SerializeField]
	private float m_autoUnlockDelay = 1f;

	[Header("Video Reward Option")]
	[SerializeField]
	private Animation m_videoOptionRoot;

	[SerializeField]
	private UIInputTrigger m_videoTrigger;

	private bool m_friendshipMessagesSendInProgress;

	private float m_lastAdCancelledTime;

	private float m_lastAdCompletedTime;

	private void Awake()
	{
		DIContainerInfrastructure.AdService.AddPlacement("RewardVideo.FriendshipGate");
		base.transform.position += DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.transform.position;
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		m_buyFriendsButtonTrigger.Clicked += OnBuyFriendsButtonClicked;
		m_backButtonTrigger.Clicked += OnBackButtonClicked;
		m_sendHelpFriendsButtonTrigger.Clicked += OnSendHelpFriendsButtonClicked;
		m_videoTrigger.Clicked += OnVideoButtonClicked;
		DIContainerInfrastructure.AdService.RewardResult += RewardSponsoredAdResult;
		DIContainerInfrastructure.AdService.AdReady += OnAdReady;
	}

	private void OnAdReady(string obj)
	{
		if (m_hotspotGameData.Data.UnlockState >= HotspotUnlockState.ResolvedNew)
		{
			m_videoOptionRoot.gameObject.SetActive(false);
			return;
		}
		if (!m_videoOptionRoot.gameObject.activeInHierarchy)
		{
			m_videoOptionRoot.gameObject.SetActive(true);
		}
		m_videoOptionRoot.Play("SponsoredRoll_Enter");
	}

	private void RewardSponsoredAdResult(string placement, Ads.RewardResult result, string voucherId)
	{
		if (placement != "RewardVideo.FriendshipGate")
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
			if (!(m_lastAdCancelledTime > m_lastAdCompletedTime) && Time.time - m_lastAdCompletedTime < 60f)
			{
				OnAdWatchedForFriendGate();
			}
			break;
		case Ads.RewardResult.RewardFailed:
			break;
		default:
			throw new ArgumentOutOfRangeException("result");
		}
	}

	private void OnAdWatchedForFriendGate()
	{
		AddNPCFriendUnlock();
		FillInfos();
		m_locked = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.GetAndSetFriendshipGateLockState(m_hotspotGameData);
		if (!m_locked)
		{
			DeregisterEventHandler();
			m_HelpAnimation.Play("PlayButton_Leave");
			m_TooltipAnimation.Play("Footer_Leave");
			m_videoOptionRoot.Play("SponsoredRoll_Leave");
			DoorButtonTriggerClicked(true);
		}
	}

	private void DeregisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		m_buyFriendsButtonTrigger.Clicked -= OnBuyFriendsButtonClicked;
		m_backButtonTrigger.Clicked -= OnBackButtonClicked;
		m_sendHelpFriendsButtonTrigger.Clicked -= OnSendHelpFriendsButtonClicked;
		m_videoTrigger.Clicked -= OnVideoButtonClicked;
		DIContainerInfrastructure.AdService.RewardResult -= RewardSponsoredAdResult;
		DIContainerInfrastructure.AdService.AdReady -= OnAdReady;
	}

	private void OnVideoButtonClicked()
	{
		if (!DIContainerInfrastructure.AdService.IsAdShowPossible("RewardVideo.FriendshipGate"))
		{
			DebugLog.Error(GetType(), "OnVideoButtonClicked: No Ad available! This button shouldn't even be here!");
			return;
		}
		if (!DIContainerInfrastructure.AdService.ShowAd("RewardVideo.FriendshipGate"))
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_no_ad_available", "There is currently no Ad scheduled"), "no_ad", DispatchMessage.Status.Info);
			return;
		}
		m_videoOptionRoot.Play("SponsoredRoll_Leave");
		DIContainerInfrastructure.AdService.MutedGameSoundForPlacement("RewardVideo.FriendshipGate");
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		Leave(m_returnAction);
	}

	private void DoorButtonTriggerClicked()
	{
		DoorButtonTriggerClicked(false);
	}

	private void DoorButtonTriggerClicked(bool leave)
	{
		Requirement firstFailedReq = null;
		if (DIContainerLogic.WorldMapService.IsHotspotEnterable(DIContainerInfrastructure.GetCurrentPlayer(), m_hotspotGameData, out firstFailedReq) || firstFailedReq.RequirementType != RequirementType.UsedFriends)
		{
			m_OpenAnimation.Play("FriendshipGate_Open_Show");
			if (leave)
			{
				StartCoroutine(DelayedLeave(m_OpenAnimation["FriendshipGate_Open_Show"].length));
			}
			else
			{
				m_OpenAnimation.PlayQueued("FriendshipGate_Open_Idle");
			}
			m_locked = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.GetAndSetFriendshipGateLockState(m_hotspotGameData);
		}
	}

	private IEnumerator DelayedLeave(float delay)
	{
		yield return new WaitForSeconds(delay);
		Leave(m_returnAction);
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
				m_TimerLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft);
			}
			yield return new WaitForSeconds(1f);
		}
		m_TimerAnimation.Play("Button_Timer_Hide");
	}

	private void OnBuyFriendsButtonClicked()
	{
		if (DIContainerLogic.RequirementService.ExecuteRequirements(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, new List<Requirement> { GetNeededFriendsCost() }, "buy_friends"))
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
			int neededFriends = GetNeededFriends(m_hotspotBalancing.EnterRequirements, DIContainerInfrastructure.GetCurrentPlayer());
			for (int i = 0; i < neededFriends; i++)
			{
				AddNPCFriendUnlock();
			}
			FillInfos();
			DeregisterEventHandler();
			m_HelpAnimation.Play("PlayButton_Leave");
			m_TooltipAnimation.Play("Footer_Leave");
			m_locked = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.GetAndSetFriendshipGateLockState(m_hotspotGameData);
			DoorButtonTriggerClicked(true);
			return;
		}
		Requirement neededFriendsCost = GetNeededFriendsCost();
		if (neededFriendsCost != null && neededFriendsCost.RequirementType == RequirementType.PayItem)
		{
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, neededFriendsCost.NameId, out data))
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_MissingCurrencyPopup.EnterPopup(data.ItemBalancing.NameId, neededFriendsCost.Value);
			}
		}
	}

	private void AddNPCFriendUnlock()
	{
		FriendData lowNPCFriend = DIContainerLogic.SocialService.GetLowNPCFriend(DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
		if (!DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks.ContainsKey(m_hotspotBalancing.NameId))
		{
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks.Add(m_hotspotBalancing.NameId, new List<string>());
		}
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks[m_hotspotBalancing.NameId].Add(lowNPCFriend.Id);
	}

	private Requirement GetNeededFriendsCost()
	{
		Requirement requirement = new Requirement();
		BasicShopOfferBalancingData basicShopOfferBalancingData = DIContainerLogic.GetShopService().GetShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), "friend_gate").FirstOrDefault();
		if (basicShopOfferBalancingData != null)
		{
			Requirement requirement2 = basicShopOfferBalancingData.BuyRequirements.FirstOrDefault();
			if (requirement2 != null)
			{
				requirement.RequirementType = requirement2.RequirementType;
				requirement.NameId = requirement2.NameId;
				requirement.Value = requirement2.Value;
			}
		}
		requirement.Value *= GetNeededFriends(m_hotspotBalancing.EnterRequirements, DIContainerInfrastructure.GetCurrentPlayer());
		return requirement;
	}

	public int GetNeededFriends(List<Requirement> reqs, PlayerGameData player)
	{
		Requirement requirement = reqs.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.UsedFriends);
		List<string> value = new List<string>();
		player.SocialEnvironmentGameData.Data.FriendShipGateUnlocks.TryGetValue(m_hotspotBalancing.NameId, out value);
		int num = ((value != null) ? value.Count : 0);
		return (int)requirement.Value - num;
	}

	public void OnBackButtonClicked()
	{
		Leave(m_returnAction);
	}

	public FriendshipGateUI SetHotSpot(HotspotGameData spotData, BaseLocationStateManager mapMgr)
	{
		m_worldMapStateMgr = mapMgr;
		m_hotspotBalancing = spotData.BalancingData;
		m_hotspotGameData = spotData;
		m_locked = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.GetAndSetFriendshipGateLockState(m_hotspotGameData);
		return this;
	}

	public FriendshipGateUI SetReturnAction(Action returnAction)
	{
		m_returnAction = returnAction;
		return this;
	}

	public void Enter()
	{
		base.gameObject.SetActive(true);
		if (m_worldMapStateMgr != null && m_worldMapStateMgr.WorldMenuUI != null)
		{
			m_worldMapStateMgr.WorldMenuUI.Leave();
		}
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("worldmap_gate_enter");
		FillInfos();
		yield return null;
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		m_BackAnimation.Play("BackButton_Enter");
		SetFacebookText();
		EnterLockedDependentParts();
		if ((bool)m_GateAnimation)
		{
			m_GateAnimation.Play("FriendshipGate_Enter");
		}
		foreach (UIPanel panel in m_Panels)
		{
			if (panel != null)
			{
				panel.enabled = true;
			}
		}
		if ((bool)m_GateAnimation)
		{
			yield return new WaitForSeconds(m_GateAnimation["FriendshipGate_Enter"].length);
		}
		EnterCoinBars();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		RegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("worldmap_gate_enter");
		Invoke("GetFriends", 0f);
		HandleFriendGateMessages(DIContainerInfrastructure.GetCurrentPlayer());
	}

	private void SetFacebookText()
	{
		if (DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			m_AskFriendsButton.atlas = m_WorldMapElementsAtlas;
			m_AskFriendsButton.spriteName = "AskFriend";
			m_AskFriendsButton.MakePixelPerfect();
			m_AskFriendsText.text = DIContainerInfrastructure.GetLocaService().Tr("fgate_footer_askfriends", "Ask your friends for help to open the friendship gate!");
			return;
		}
		if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset("ShopAndSocialElements"))
		{
			GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject("ShopAndSocialElements") as GameObject;
			if (gameObject != null)
			{
				m_AskFriendsButton.atlas = gameObject.GetComponent<UIAtlas>();
			}
			m_AskFriendsButton.spriteName = "Facebook";
			m_AskFriendsButton.MakePixelPerfect();
		}
		m_AskFriendsText.text = DIContainerInfrastructure.GetLocaService().Tr("fgate_footer_facebook", "Login in Facebook to ask your friends!");
	}

	private float EnterLockedDependentParts()
	{
		if (m_locked)
		{
			m_HelpAnimation.gameObject.SetActive(true);
			m_TooltipAnimation.gameObject.SetActive(true);
			m_HelpAnimation.Play("PlayButton_Enter");
			m_TooltipAnimation.Play("Footer_Enter");
			m_OpenAnimation.Play("FriendshipGate_Open_Hide");
			if (!DIContainerLogic.SocialService.IsSendFriendshipGateHelpPossible(m_hotspotBalancing.NameId, DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData))
			{
				m_TimerAnimation.Play("Button_Timer_SetShown");
				StartCoroutine(CountDownTimer(DIContainerLogic.GetTimingService().GetPresentTime() + DIContainerLogic.SocialService.GetSendFriendshipGateHelpTimeLeft(m_hotspotBalancing.NameId, DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData)));
			}
			else
			{
				m_TimerAnimation.Play("Button_Timer_SetHidden");
			}
			if (IsVideoRewardAvailable())
			{
				m_videoOptionRoot.Play("SponsoredRoll_Enter");
			}
			else
			{
				m_videoOptionRoot.gameObject.SetActive(false);
			}
			return m_TooltipAnimation["Footer_Enter"].length;
		}
		m_HelpAnimation.gameObject.SetActive(false);
		m_TooltipAnimation.gameObject.SetActive(false);
		m_videoOptionRoot.gameObject.SetActive(false);
		if (m_hotspotGameData.Data.UnlockState == HotspotUnlockState.ResolvedNew)
		{
			m_OpenAnimation.Play("FriendshipGate_Open_Hide");
		}
		else
		{
			m_OpenAnimation.Play("FriendshipGate_Open_Idle");
		}
		return 0f;
	}

	private bool IsVideoRewardAvailable()
	{
		return DIContainerInfrastructure.AdService.IsAdShowPossible("RewardVideo.FriendshipGate");
	}

	private float LeaveLockedDependentParts()
	{
		if (m_locked)
		{
			m_HelpAnimation.Play("PlayButton_Leave");
			m_TooltipAnimation.Play("Footer_Leave");
			return m_TooltipAnimation["Footer_Leave"].length;
		}
		return 0f;
	}

	public void Leave(Action actionAfterLeave = null)
	{
		List<string> value;
		if (!DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.NewFriendShipGateUnlocks.TryGetValue(m_hotspotGameData.BalancingData.NameId, out value) || value == null)
		{
			value = new List<string>();
		}
		value.Clear();
		StartCoroutine(LeaveCoroutine(actionAfterLeave));
	}

	private IEnumerator LeaveCoroutine(Action actionAfterLeave)
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("worldmap_gate_leave");
		DeregisterEventHandler();
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		LeaveLockedDependentParts();
		m_BackAnimation.Play("BackButton_Leave");
		if ((bool)m_GateAnimation)
		{
			m_GateAnimation.Play("FriendshipGate_Leave");
		}
		LeaveCoinBars();
		if ((bool)m_GateAnimation)
		{
			yield return new WaitForSeconds(m_GateAnimation["FriendshipGate_Leave"].length);
		}
		foreach (UIPanel panel in m_Panels)
		{
			if (panel != null)
			{
				panel.enabled = false;
			}
		}
		if (actionAfterLeave != null)
		{
			actionAfterLeave();
		}
		m_worldMapStateMgr.WorldMenuUI.Enter();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("worldmap_gate_leave");
		base.gameObject.SetActive(false);
	}

	private void FillInfos()
	{
		List<string> value = new List<string>();
		List<string> list = new List<string>();
		List<string> value2 = new List<string>();
		if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks == null)
		{
			DebugLog.Log("No Friendship Gate Unlocks!");
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks = new Dictionary<string, List<string>>();
		}
		if (!DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks.TryGetValue(m_hotspotBalancing.NameId, out value) || value == null)
		{
			value = new List<string>();
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.NewFriendShipGateUnlocks == null)
		{
			DebugLog.Log("No new Friendship Gate Unlocks!");
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.NewFriendShipGateUnlocks = new Dictionary<string, List<string>>();
		}
		if (!DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.NewFriendShipGateUnlocks.TryGetValue(m_hotspotGameData.BalancingData.NameId, out value2) || value2 == null)
		{
			value2 = new List<string>();
		}
		foreach (string item in value2)
		{
			list.Add(item);
		}
		int num = 0;
		for (num = 0; num < Mathf.Min(value.Count, m_FriendInfos.Count); num++)
		{
			DebugLog.Log("[FriendshipGateUI] " + num + " Unlock start!");
			string text = value[num];
			FriendGameData value3 = null;
			if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.TryGetValue(text, out value3))
			{
				DebugLog.Log("[FriendshipGateUI] " + num + " Got Friend!");
				m_FriendInfos[num].SetDefault();
				m_FriendInfos[num].SetModel(value3);
				DebugLog.Log("[FriendshipGateUI] " + num + " Set Friend info!");
				m_FriendInfos[num].SetNew(list.Remove(text));
				DebugLog.Log("[FriendshipGateUI] " + num + " Set Friend info new!");
				if (!value3.isNpcFriend)
				{
					DebugLog.Log("[FriendshipGateUI] " + num + " Not NPC Friend new!");
					UITexture[] componentsInChildren = m_FriendInfos[num].GetComponentsInChildren<UITexture>(true);
					foreach (UITexture uITexture in componentsInChildren)
					{
						uITexture.gameObject.SetActive(true);
					}
				}
			}
			else
			{
				m_FriendInfos[num].SetNPCIcon(true);
			}
		}
		for (int j = num; j < m_FriendInfos.Count; j++)
		{
			m_FriendInfos[j].SetNPCIcon(false);
			UITexture[] componentsInChildren2 = m_FriendInfos[j].GetComponentsInChildren<UITexture>(true);
			foreach (UITexture uITexture2 in componentsInChildren2)
			{
				uITexture2.gameObject.SetActive(false);
			}
		}
		Requirement neededFriendsCost = GetNeededFriendsCost();
		string assetId = string.Empty;
		BasicItemBalancingData balancing = null;
		if (DIContainerBalancing.Service.TryGetBalancingData<BasicItemBalancingData>(neededFriendsCost.NameId, out balancing))
		{
			assetId = balancing.AssetBaseId;
		}
		m_BuyFriendsCost.SetModel(assetId, null, neededFriendsCost.Value, string.Empty);
	}

	private void EnterCoinBars()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u,
			showFriendshipEssence = false,
			showLuckyCoins = true,
			showSnoutlings = false
		}, true);
	}

	private void LeaveCoinBars()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
	}

	private List<FriendData> GetNPCFriends()
	{
		List<FriendData> list = new List<FriendData>();
		list.Add(DIContainerLogic.SocialService.GetHighNPCFriend(DIContainerInfrastructure.GetCurrentPlayer().Data.Level));
		list.Add(DIContainerLogic.SocialService.GetLowNPCFriend(DIContainerInfrastructure.GetCurrentPlayer().Data.Level));
		return list;
	}

	private void OnGetFriendDataUpdate(List<FriendData> friends)
	{
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.RefreshFriends(friends);
	}

	private void HandleFriendGateMessages(PlayerGameData player)
	{
		List<IMailboxMessageGameData> list = Enumerable.ToList(player.SocialEnvironmentGameData.MailboxMessages.Values.Where((IMailboxMessageGameData m) => m is ResponseFriendshipGateMessage));
		foreach (IMailboxMessageGameData item in list)
		{
			item.UseMessageContent(player, OnFriendForGateReceived);
		}
		m_locked = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.GetAndSetFriendshipGateLockState(m_hotspotGameData);
		if (m_hotspotGameData.Data.UnlockState == HotspotUnlockState.ResolvedNew)
		{
			Invoke("DoorButtonTriggerClicked", m_autoUnlockDelay);
		}
	}

	private void OnFriendForGateReceived(bool success, IMailboxMessageGameData message)
	{
		if (!success)
		{
			return;
		}
		FriendGameData value = null;
		if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.TryGetValue(message.Sender.FriendId, out value))
		{
			value.SetFriendData(message.Sender.Data);
			if (!DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks.ContainsKey(m_hotspotBalancing.NameId))
			{
				DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks.Add(m_hotspotBalancing.NameId, new List<string>());
			}
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks[m_hotspotBalancing.NameId].Add(value.FriendId);
		}
		FillInfos();
	}

	private void SocialWindowUIFacebookLoginFailedEvent(string obj)
	{
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= SocialWindowUIFacebookLoginSucceededEvent;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= SocialWindowUIFacebookLoginFailedEvent;
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_failed", "Facebook login failed!"), "facebook", DispatchMessage.Status.Error);
		RegisterEventHandler();
	}

	private void SocialWindowUIFacebookLoginSucceededEvent()
	{
		DIContainerInfrastructure.GetCoreStateMgr().StartRefreshFriends();
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= SocialWindowUIFacebookLoginSucceededEvent;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= SocialWindowUIFacebookLoginFailedEvent;
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_succes", "Facebook login succesfully!"), "facebook", DispatchMessage.Status.Info);
		SetFacebookText();
		RegisterEventHandler();
	}

	private void OnSendHelpFriendsButtonClicked()
	{
		DeregisterEventHandler();
		if (!DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= SocialWindowUIFacebookLoginSucceededEvent;
			DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= SocialWindowUIFacebookLoginFailedEvent;
			DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent += SocialWindowUIFacebookLoginSucceededEvent;
			DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent += SocialWindowUIFacebookLoginFailedEvent;
			DebugLog.Log("FacebookSignInButtonClicked clicked!");
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("LinkClicked", new Dictionary<string, string>
			{
				{ "Button", "FacebookSignIn" },
				{ "Destination", "friendshipGate" }
			});
			DIContainerInfrastructure.GetCoreStateMgr().TryLoginOnFacebook();
			return;
		}
		if (!DIContainerLogic.SocialService.IsSendFriendshipGateHelpPossible(m_hotspotBalancing.NameId, DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData))
		{
			RegisterEventHandler();
			return;
		}
		List<string> list = new List<string>();
		List<string> value = new List<string>();
		if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks == null)
		{
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks = new Dictionary<string, List<string>>();
		}
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks.TryGetValue(m_hotspotBalancing.NameId, out value);
		if (value == null)
		{
			value = new List<string>();
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends == null)
		{
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends = new Dictionary<string, FriendGameData>();
		}
		foreach (FriendGameData value2 in DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.Values)
		{
			if (value2 != null && !value2.isNpcFriend && !value.Contains(value2.FriendId))
			{
				list.Add(value2.FriendId);
			}
		}
		SendFriendshipGateMessage(list);
	}

	private void SendFriendshipGateMessage(List<string> friendIds)
	{
		if (m_friendshipMessagesSendInProgress)
		{
			return;
		}
		m_friendshipMessagesSendInProgress = true;
		DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
		{
			MessageDataIncoming message = new MessageDataIncoming
			{
				MessageType = MessageType.RequestFriendshipGateMessage,
				Sender = DIContainerInfrastructure.GetCurrentPlayer().GetFriendData(),
				SentAt = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime),
				Parameter1 = m_hotspotBalancing.NameId
			};
			ABHAnalyticsHelper.SendSocialEvent(message, null);
			DIContainerInfrastructure.MessagingService.SendMessages(new MessageDataIncoming
			{
				MessageType = MessageType.RequestFriendshipGateMessage,
				Sender = DIContainerInfrastructure.GetCurrentPlayer().GetFriendData(),
				SentAt = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime),
				Parameter1 = m_hotspotBalancing.NameId
			}, friendIds);
			if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateHelpCooldowns == null)
			{
				DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateHelpCooldowns = new Dictionary<string, uint>();
			}
			if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateHelpCooldowns.ContainsKey(m_hotspotBalancing.NameId))
			{
				DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateHelpCooldowns[m_hotspotBalancing.NameId] = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
			}
			else
			{
				DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateHelpCooldowns.Add(m_hotspotBalancing.NameId, DIContainerLogic.GetTimingService().GetTimestamp(trustedTime));
			}
			m_TimerAnimation.Play("Button_Timer_Show");
			StartCoroutine(CountDownTimer(trustedTime + DIContainerLogic.SocialService.GetSendFriendshipGateHelpTimeLeft(m_hotspotBalancing.NameId, DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData)));
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("gen_toast_successendfgatemessage", "Messages sent!"), "fgatesuccess", DispatchMessage.Status.Info);
			Leave(m_returnAction);
			m_friendshipMessagesSendInProgress = false;
		});
	}
}
