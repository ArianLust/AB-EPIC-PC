using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using Rcs;
using UnityEngine;

public class PopupLowEnergy : MonoBehaviour
{
	[SerializeField]
	private UILabel m_Description;

	[SerializeField]
	private UIInputTrigger m_VideoTrigger;

	[SerializeField]
	private UIInputTrigger m_ShopTrigger;

	[SerializeField]
	private UIInputTrigger m_CloseTrigger;

	[SerializeField]
	private GameObject m_VideoRoot;

	[SerializeField]
	private UILabel m_RewardAmountLabel;

	[SerializeField]
	private UILabel m_ShopAmountLabel;

	[SerializeField]
	private UILabel m_ShopOldPriceLabel;

	[SerializeField]
	private UILabel m_ShopDiscountPriceLabel;

	[SerializeField]
	private UILabel m_EnergyRestoreTimeLabel;

	[SerializeField]
	private GameObject m_LoadingSpinner;

	private float m_lastAdCancelledTime;

	private float m_lastAdCompletedTime;

	private Dictionary<string, LootInfoData> m_loot;

	private BuyableShopOfferBalancingData m_offerData;

	public bool m_PopupShowing;

	private void Awake()
	{
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_LowEnergyPopup = this;
		base.gameObject.SetActive(false);
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused && !DIContainerInfrastructure.AdService.IsAdShowPossible("RewardVideo.Eventenergy"))
		{
			DebugLog.Log(GetType(), "OnApplicationPause: Ad no longer available. Closing low energy popup...");
			Close();
		}
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 2);
		}
	}

	private void RegisterPopupTriggers()
	{
		DeregisterPopupTriggers();
		if (m_ShopTrigger != null)
		{
			m_ShopTrigger.Clicked += OpenShop;
		}
		if (m_CloseTrigger != null)
		{
			m_CloseTrigger.Clicked += Close;
		}
		if (m_VideoTrigger != null)
		{
			m_VideoTrigger.Clicked += ShowVideo;
		}
		SetDragControllerActive(false);
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, Close);
		DIContainerInfrastructure.AdService.RewardResult += RewardSponsoredAdResult;
	}

	private void DeregisterPopupTriggers()
	{
		if (m_ShopTrigger != null)
		{
			m_ShopTrigger.Clicked -= OpenShop;
		}
		SetDragControllerActive(true);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		if (m_CloseTrigger != null)
		{
			m_CloseTrigger.Clicked -= Close;
		}
		if (m_VideoTrigger != null)
		{
			m_VideoTrigger.Clicked -= ShowVideo;
		}
		DIContainerInfrastructure.AdService.RewardResult -= RewardSponsoredAdResult;
	}

	private void OnDestroy()
	{
		DeregisterPopupTriggers();
	}

	public void ShowPopup()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		bool flag = DIContainerLogic.EventSystemService.IsCurrentEventAvailable(currentPlayer);
		bool flag2 = DIContainerInfrastructure.AdService.IsAdShowPossible("RewardVideo.Eventenergy");
		if (!flag || !flag2 || base.gameObject.activeSelf || !RefreshDailyEventAd() || !IsLowEnergy())
		{
			return;
		}
		base.gameObject.SetActive(true);
		if (!currentPlayer.Data.EventEnergyTutorialDisplayed)
		{
			StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup.ShowSpecialOfferPopup(new BasicItemGameData("event_energy")).Run());
			base.gameObject.SetActive(false);
			currentPlayer.Data.EventEnergyTutorialDisplayed = true;
			return;
		}
		m_PopupShowing = true;
		RegisterPopupTriggers();
		StopCoroutine("Deactivate");
		if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(currentPlayer))
		{
			currentPlayer.CurrentEventManagerGameData.Data.WatchedDailyEventRewardTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
			currentPlayer.SavePlayerData();
		}
		base.gameObject.PlayAnimationOrAnimatorState("Popup_EnergyMissing_Enter");
		StartCoroutine(WaitForEventEnergyAdReady());
	}

	private IEnumerator WaitForEventEnergyAdReady()
	{
		m_VideoRoot.SetActive(true);
		Dictionary<string, int> EventAdRewardString = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").DailyEventAdLoot;
		m_loot = DIContainerLogic.GetLootOperationService().GenerateLoot(EventAdRewardString, 1);
		int lootAmount = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(m_loot).FirstOrDefault()
			.ItemValue;
		m_RewardAmountLabel.text = lootAmount.ToString();
		m_Description.text = DIContainerInfrastructure.GetLocaService().Tr("popup_stamina_lowstamina_video_desc", "?Watch a Video and get {value_1} stamina for free?").Replace("{value_1}", lootAmount.ToString());
		m_VideoTrigger.gameObject.SetActive(false);
		m_LoadingSpinner.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		m_LoadingSpinner.SetActive(false);
		m_VideoTrigger.gameObject.SetActive(true);
	}

	private void SetEnergyTimer()
	{
		if (m_offerData == null || m_offerData.DiscountDuration == 0)
		{
			m_EnergyRestoreTimeLabel.gameObject.SetActive(false);
			return;
		}
		m_EnergyRestoreTimeLabel.gameObject.SetActive(true);
		StopCoroutine("CountDownTimer");
		StartCoroutine("CountDownTimer");
	}

	private IEnumerator CountDownTimer()
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		DateTime targetTime = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.WatchedDailyEventRewardTimestamp).AddSeconds(m_offerData.DiscountDuration);
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_EnergyRestoreTimeLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft);
			}
			yield return new WaitForSeconds(1f);
		}
		Close();
	}

	private void Close()
	{
		base.gameObject.PlayAnimationOrAnimatorState("Popup_EnergyMissing_Leave");
		if (m_offerData != null && m_offerData.DiscountDuration == 0 && DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "show_energy_discount") > 0)
		{
			DIContainerLogic.InventoryService.RemoveItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "show_energy_discount", 1, "energy_discount_only_once");
		}
		DeregisterPopupTriggers();
		StartCoroutine("Deactivate");
	}

	private IEnumerator Deactivate()
	{
		yield return new WaitForSeconds(0.5f);
		m_PopupShowing = false;
		yield return new WaitForSeconds(1f);
		base.gameObject.SetActive(false);
	}

	private void OpenShop()
	{
		int startIndex = 0;
		string category = "shop_global_consumables";
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop(category, null, startIndex);
		Invoke("Close", 2f);
	}

	private void ShowVideo()
	{
		if (!DIContainerInfrastructure.AdService.ShowAd("RewardVideo.Eventenergy"))
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_no_ad_available", "There is currently no Ad scheduled"), "no_ad", DispatchMessage.Status.Info);
		}
		else
		{
			DIContainerInfrastructure.AdService.MutedGameSoundForPlacement("RewardVideo.Eventenergy");
		}
	}

	private void RewardSponsoredAdResult(string placement, Ads.RewardResult result, string voucherId)
	{
		if (placement != "RewardVideo.Eventenergy")
		{
			return;
		}
		DebugLog.Log("[EventRewardAd] Reward Result received: " + result);
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
					OnAdAbortedForEventChest();
				}
			}
			else if (Time.time - m_lastAdCompletedTime < 60f)
			{
				OnAdWatchedForEventChest();
			}
			break;
		case Ads.RewardResult.RewardFailed:
			OnAdAbortedForEventChest();
			break;
		default:
			throw new ArgumentOutOfRangeException("result");
		}
	}

	private void OnAdWatchedForEventChest()
	{
		RewardForWatching();
		Close();
	}

	private void OnAdAbortedForEventChest()
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_advideo_cancelled", "You did not watch the whole video"));
	}

	private void RewardForWatching()
	{
		DIContainerLogic.GetLootOperationService().RewardLoot(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 2, m_loot, "eventy_daily_ad_reward");
	}

	public bool RefreshDailyEventAd()
	{
		DateTime trustedTime;
		if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			return false;
		}
		bool flag = false;
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		DateTime dateTimeFromTimestamp = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Data.WatchedDailyEventRewardTimestamp);
		if (currentPlayer.WorldGameData.BalancingData.TimeDailyEventPopupSpawn != 0)
		{
			return trustedTime > dateTimeFromTimestamp.AddSeconds(currentPlayer.WorldGameData.BalancingData.TimeDailyEventPopupSpawn);
		}
		DateTime dateTime = dateTimeFromTimestamp.ToLocalTime().AddDays(1.0);
		DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Local).ToUniversalTime();
		DebugLog.Log(string.Concat("[EventRewardAd] Next Day Clamped UTC Time: ", dateTime2, " Time left ", dateTime2 - trustedTime, "Last Time: ", dateTimeFromTimestamp, " Current Time: ", trustedTime));
		DebugLog.Log("[EventRewardAd] Current Local Time: " + trustedTime.ToLocalTime());
		return trustedTime > dateTime2;
	}

	private bool IsLowEnergy()
	{
		EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		bool flag = false;
		Dictionary<string, LootInfoData> loot;
		if (!currentEventManagerGameData.IsCampaignEvent)
		{
			loot = ((!currentEventManagerGameData.IsBossEvent) ? DIContainerLogic.GetLootOperationService().GenerateLootPreview(currentEventManagerGameData.EventBalancing.EventGeneratorItemLootTable, DIContainerInfrastructure.GetCurrentPlayer().Data.Level) : DIContainerLogic.GetLootOperationService().GenerateLootPreview(currentEventManagerGameData.EventBalancing.EventBossItemLootTable, DIContainerInfrastructure.GetCurrentPlayer().Data.Level));
		}
		else
		{
			flag = true;
			loot = DIContainerLogic.GetLootOperationService().GenerateLootPreview(currentEventManagerGameData.EventBalancing.EventMiniCampaignItemLootTable, DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
		}
		List<EventItemGameData> list = new List<EventItemGameData>();
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(loot);
		foreach (IInventoryItemGameData item in itemsFromLoot)
		{
			if (item is EventItemGameData)
			{
				list.Add(item as EventItemGameData);
			}
		}
		float num = -1f;
		num = ((!flag) ? GetLowestInvasionCost(list) : GetLowestMiniCampaignCost());
		float f = (float)DIContainerLogic.GetTimingService().TimeSince(DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().Data.LastEnergyAddTime)).TotalSeconds / DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData.EnergyRefreshTimeInSeconds;
		int num2 = Mathf.FloorToInt(f);
		int itemValue = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "event_energy");
		if ((float)(itemValue + num2) < num)
		{
			return true;
		}
		return false;
	}

	private float GetLowestInvasionCost(List<EventItemGameData> possibleEventItems)
	{
		float num = -1f;
		foreach (EventItemGameData possibleEventItem in possibleEventItems)
		{
			string firstPossibleBattle = DIContainerLogic.GetBattleService().GetFirstPossibleBattle(possibleEventItem.BalancingData.EventParameters, DIContainerInfrastructure.GetCurrentPlayer());
			BattleBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(firstPossibleBattle);
			if (balancingData != null)
			{
				float num2 = 0f;
				Requirement requirement = balancingData.BattleRequirements.FirstOrDefault((Requirement br) => br.RequirementType == RequirementType.PayItem && br.NameId == "event_energy");
				if (requirement != null)
				{
					num2 = requirement.Value;
				}
				if (num == -1f || num > num2)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	private float GetLowestMiniCampaignCost()
	{
		EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		if (currentEventManagerGameData == null || currentEventManagerGameData.CurrentMiniCampaign == null || !currentEventManagerGameData.IsValid)
		{
			DebugLog.Warn(GetType(), "GetLowestMiniCampaignCost: MiniCampaign not yet initialized!");
			return -1f;
		}
		List<HotspotGameData> list = Enumerable.ToList(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentMiniCampaign.HotspotGameDatas.Values);
		List<string> list2 = new List<string>();
		for (int i = 0; i < list.Count; i++)
		{
			HotspotGameData hotspotGameData = list[i];
			if (hotspotGameData.Data.UnlockState > HotspotUnlockState.Hidden && hotspotGameData.BalancingData.BattleId != null)
			{
				list2.AddRange(hotspotGameData.BalancingData.BattleId);
			}
		}
		string firstPossibleBattle = DIContainerLogic.GetBattleService().GetFirstPossibleBattle(list2, DIContainerInfrastructure.GetCurrentPlayer());
		if (string.IsNullOrEmpty(firstPossibleBattle))
		{
			return -1f;
		}
		BattleBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(firstPossibleBattle);
		if (balancingData == null)
		{
			return -1f;
		}
		DebugLog.Log(GetType(), "G");
		Requirement requirement = balancingData.BattleRequirements.FirstOrDefault((Requirement br) => br.RequirementType == RequirementType.PayItem && br.NameId == "event_energy");
		DebugLog.Log(GetType(), "H");
		if (requirement != null)
		{
			return requirement.Value;
		}
		return -1f;
	}
}
