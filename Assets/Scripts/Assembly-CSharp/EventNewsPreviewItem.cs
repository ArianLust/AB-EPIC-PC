using System;
using System.Collections;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class EventNewsPreviewItem : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_CheckOutButton;

	[SerializeField]
	private UILabel m_HeaderLabel;

	[SerializeField]
	private UISprite m_Icon;

	[SerializeField]
	private GameObject m_IconPrefabContainer;

	[SerializeField]
	private GameObject m_LoadingSpinnerRoot;

	[SerializeField]
	private UILabel m_TimeLabel;

	[SerializeField]
	private GameObject m_LockedState;

	[SerializeField]
	private UIInputTrigger m_InfoButton;

	[SerializeField]
	private UISprite m_EventTypeIcon;

	[SerializeField]
	private UILabel m_NextLabel;

	private EventManagerGameData m_EventModel;

	private DateTime m_targetTime;

	private bool m_isRunning;

	private NewsUi m_newsParent;

	private BasicShopOfferBalancingData m_salesModel;

	private BonusEventType m_bonusEventType;

	private bool m_IsInitialized;

	[SerializeField]
	private bool m_IsSoloItem;

	private int m_currentUiIndex;

	public void Init(EventManagerGameData eventMgr, NewsUi newsWindow, int uiIndex)
	{
		m_EventModel = eventMgr;
		m_currentUiIndex = uiIndex;
		if (m_EventModel == null)
		{
			return;
		}
		if (DIContainerInfrastructure.EventSystemStateManager.GetCurrentStateForEventManager(m_EventModel) == EventManagerState.Teasing)
		{
			m_targetTime = DIContainerLogic.EventSystemService.GetTeasingEndTime(m_EventModel.Balancing);
		}
		else
		{
			m_targetTime = DIContainerLogic.EventSystemService.GetEventEndTime(m_EventModel.Balancing);
		}
		m_newsParent = newsWindow;
		m_CheckOutButton.gameObject.SetActive(DIContainerLogic.EventSystemService.AllowCheckout());
		GenericInit(m_EventModel.EventBalancing.LocaBaseId, 0f);
		if ((bool)m_EventTypeIcon)
		{
			if (m_EventModel.IsCampaignEvent)
			{
				m_EventTypeIcon.spriteName = "GameplayEvent_Campaign";
			}
			else if (m_EventModel.IsBossEvent)
			{
				m_EventTypeIcon.spriteName = "GameplayEvent_Boss";
			}
			else
			{
				m_EventTypeIcon.spriteName = "GameplayEvent_Invasion";
			}
			m_EventTypeIcon.MakePixelPerfect();
		}
		if (m_Icon != null)
		{
			m_Icon.gameObject.SetActive(false);
		}
		if (!(m_IconPrefabContainer == null))
		{
			m_IconPrefabContainer.SetActive(true);
			m_IsInitialized = true;
			StartCoroutine(HandleIconAsset());
		}
	}

	public void Init(BasicShopOfferBalancingData balancing, DateTime targetTime, NewsUi newsWindow, int uiIndex)
	{
		m_isRunning = true;
		m_targetTime = targetTime;
		m_salesModel = balancing;
		m_newsParent = newsWindow;
		m_currentUiIndex = uiIndex;
		GenericInit(balancing.LocaId, 0f);
		if (m_IconPrefabContainer != null)
		{
			m_IconPrefabContainer.gameObject.SetActive(false);
		}
		if (!(m_Icon == null))
		{
			SetIcon(balancing.PopupIconId);
		}
	}

	public void Init(BonusEventBalancingData balancing, bool isRunning, DateTime targetTime, NewsUi newsWindow, int uiIndex)
	{
		m_isRunning = isRunning;
		m_targetTime = targetTime;
		m_bonusEventType = balancing.BonusType;
		GenericInit(balancing.LocaId, balancing.BonusFactor);
		m_newsParent = newsWindow;
		m_currentUiIndex = uiIndex;
		if (m_IconPrefabContainer != null)
		{
			m_IconPrefabContainer.gameObject.SetActive(false);
		}
		if (!(m_Icon == null))
		{
			SetIcon(balancing.IconId);
			m_newsParent.SetInfoPopupDesc(balancing.LocaId);
			bool flag = false;
			switch (balancing.BonusType)
			{
			case BonusEventType.DungeonBonus:
				flag = !CheckIfHotspotUnlocked();
				break;
			case BonusEventType.CcLootBonus:
				flag = !CheckIfCaveUnlocked();
				break;
			case BonusEventType.MasteryBonus:
				flag = !CheckIfDojoUnlocked();
				break;
			case BonusEventType.ArenaPointBonus:
				flag = !CheckIfArenaUnlocked();
				break;
			}
			if (balancing.BonusType == BonusEventType.DungeonBonus)
			{
				flag = CheckIfHotspotUnlocked();
			}
			else if (balancing.BonusType == BonusEventType.CcLootBonus)
			{
				flag = CheckIfCaveUnlocked();
			}
			else if (balancing.BonusType == BonusEventType.MasteryBonus)
			{
				flag = CheckIfDojoUnlocked();
			}
			m_LockedState.SetActive(flag);
			if (m_isRunning && m_CheckOutButton != null)
			{
				m_CheckOutButton.gameObject.SetActive(!flag);
			}
		}
	}

	private void GenericInit(string locaId, float value = 0f)
	{
		if ((bool)m_CheckOutButton)
		{
			m_CheckOutButton.Clicked -= CheckOut;
			m_CheckOutButton.Clicked += CheckOut;
		}
		if ((bool)m_InfoButton)
		{
			m_InfoButton.Clicked -= InfoButtonClicked;
			m_InfoButton.Clicked += InfoButtonClicked;
		}
		HandleTimerLabel();
		m_HeaderLabel.gameObject.SetActive(true);
		m_HeaderLabel.text = DIContainerInfrastructure.GetLocaService().Tr(locaId + "_name");
	}

	private void InfoButtonClicked()
	{
		m_newsParent.ShowInfoPopup();
	}

	private bool CheckIfHotspotUnlocked()
	{
		HotspotGameData value = null;
		if (!DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(GetNameIdOfCurrentHotSpot(), out value))
		{
			DebugLog.Error("Could not find hotspot!");
			return false;
		}
		return value.Data.UnlockState <= HotspotUnlockState.Hidden;
	}

	private bool CheckIfCaveUnlocked()
	{
		HotspotGameData value = null;
		if (!DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue("hotspot_104_01_chroniclecave", out value))
		{
			DebugLog.Error("Could not find hotspot!");
			return false;
		}
		return value.Data.UnlockState <= HotspotUnlockState.Hidden;
	}

	private bool CheckIfDojoUnlocked()
	{
		HotspotGameData value = null;
		if (!DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue("hotspot_092_01_mightyeagledojo", out value))
		{
			DebugLog.Error("Could not find hotspot!");
			return false;
		}
		return value.Data.UnlockState <= HotspotUnlockState.Hidden;
	}

	private bool CheckIfArenaUnlocked()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		return DIContainerLogic.InventoryService.CheckForItem(currentPlayer.InventoryGameData, "unlock_pvp");
	}

	private string GetNameIdOfCurrentHotSpot()
	{
		switch (DIContainerLogic.GetTimingService().GetPresentTime().DayOfWeek)
		{
		case DayOfWeek.Monday:
			return "hotspot_047_01_dungeon";
		case DayOfWeek.Tuesday:
			return "hotspot_066_01_dungeon";
		case DayOfWeek.Wednesday:
			return "hotspot_049_04_dungeon";
		case DayOfWeek.Thursday:
			return "hotspot_080_01_dungeon";
		case DayOfWeek.Friday:
			return "hotspot_092_02_dungeon";
		case DayOfWeek.Saturday:
			return "hotspot_080_10_dungeon";
		case DayOfWeek.Sunday:
			return "hotspot_099_03_dungeon";
		default:
			return string.Empty;
		}
	}

	private void SetIcon(string iconId)
	{
		m_Icon.gameObject.SetActive(true);
		m_Icon.spriteName = iconId.Replace("ShopOffer", "Icon");
		m_Icon.MakePixelPerfect();
		m_newsParent.SetInfoPopupIcon(iconId);
	}

	private IEnumerator HandleIconAsset()
	{
		DIContainerInfrastructure.EventSystemStateManager.CheckAndLoadEventAssets(m_EventModel);
		while (m_EventModel == null || (!m_EventModel.IsAssetValid && m_IconPrefabContainer.transform.childCount == 0))
		{
			if ((bool)m_LoadingSpinnerRoot)
			{
				m_LoadingSpinnerRoot.SetActive(true);
			}
			yield return new WaitForEndOfFrame();
		}
		if ((bool)m_LoadingSpinnerRoot)
		{
			m_LoadingSpinnerRoot.SetActive(false);
		}
		if (m_IconPrefabContainer.transform.childCount == 0)
		{
			GameObject eventIcon = DIContainerInfrastructure.EventSystemStateManager.InstantiateEventObject("Icon_Large", m_IconPrefabContainer.transform, m_EventModel);
			if ((bool)eventIcon)
			{
				eventIcon.transform.localScale = Vector3.one;
			}
		}
	}

	private void CheckOut()
	{
		if (m_EventModel != null)
		{
			DIContainerLogic.EventSystemService.CheckoutClicked(m_EventModel, "NewsUI");
			m_newsParent.HideOnly();
			return;
		}
		if (m_salesModel != null)
		{
			string category = m_salesModel.Category;
			if (category == "shop_global_premium" || category == "shop_global_premium_soft")
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.GetLuckyCoinController().SwitchToShop();
				m_newsParent.HideOnly();
			}
			else
			{
				DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreenViaHotlink(category);
			}
			return;
		}
		bool flag = DIContainerInfrastructure.LocationStateMgr is WorldMapStateMgr;
		switch (m_bonusEventType)
		{
		case BonusEventType.ArenaPointBonus:
		case BonusEventType.ShardsForObjective:
			DIContainerInfrastructure.GetCoreStateMgr().GotoArenaScreenViaHotlink(string.Empty);
			break;
		case BonusEventType.CcLootBonus:
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(1u);
			DIContainerInfrastructure.GetCoreStateMgr().GotoChronlicleCave();
			break;
		case BonusEventType.DungeonBonus:
		{
			int dayOfWeek = (int)DIContainerLogic.GetTimingService().GetPresentTime().DayOfWeek;
			DIContainerInfrastructure.GetCoreStateMgr().m_ZoomToDungeonId = (dayOfWeek + 6) % 7;
			if (flag)
			{
				m_newsParent.HideOnly();
				(DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr).ZoomToDungeon();
			}
			else
			{
				DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
			}
			break;
		}
		case BonusEventType.MasteryBonus:
			if (flag)
			{
				m_newsParent.HideOnly();
				(DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr).ZoomToDojo();
			}
			else
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GoToDojo = true;
				DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
			}
			break;
		case BonusEventType.RainbowbarBonus:
			DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreenViaHotlink("story_goldenpig");
			break;
		}
	}

	private void OnDestroy()
	{
		if ((bool)m_CheckOutButton)
		{
			m_CheckOutButton.Clicked -= CheckOut;
		}
	}

	private IEnumerator CountDownTimerToStart()
	{
		DateTime trustedTime = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerLogic.GetTimingService().GetCurrentTimestamp());
		while (m_targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = DIContainerLogic.GetTimingService().TimeLeftUntil(m_targetTime);
				if (m_NextLabel != null && !m_isRunning)
				{
					if (m_currentUiIndex == 0 && !m_IsSoloItem)
					{
						m_NextLabel.text = DIContainerInfrastructure.GetLocaService().Tr("news_header_next");
					}
					else if (timeLeft.Days < 3)
					{
						m_NextLabel.text = DIContainerInfrastructure.GetLocaService().Tr("news_header_soon");
					}
					else
					{
						m_NextLabel.text = DIContainerInfrastructure.GetLocaService().Tr("news_header_upcoming");
					}
				}
				m_TimeLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandardDown(timeLeft);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private IEnumerator CountDownTimerToEnd()
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		while (m_targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				m_TimeLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(DIContainerLogic.GetTimingService().TimeLeftUntil(m_targetTime));
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void HandleTimerLabel()
	{
		StopCoroutine("CountDownTimerToEnd");
		StopCoroutine("CountDownTimerToStart");
		if (m_EventModel != null)
		{
			if (m_EventModel.IsResultValid)
			{
				m_TimeLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_finished", "Finished!");
			}
			else
			{
				switch (DIContainerInfrastructure.EventSystemStateManager.GetCurrentStateForEventManager(m_EventModel))
				{
				case EventManagerState.Teasing:
					StartCoroutine("CountDownTimerToStart");
					break;
				case EventManagerState.Running:
					StartCoroutine("CountDownTimerToEnd");
					break;
				case EventManagerState.Finished:
					m_TimeLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_calculating", "Calculating!");
					break;
				case EventManagerState.FinishedWithoutPoints:
					m_TimeLabel.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_finished", "Finished!");
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}
		else if (m_isRunning)
		{
			StartCoroutine("CountDownTimerToEnd");
		}
		else
		{
			StartCoroutine("CountDownTimerToStart");
		}
		StopCoroutine("HandleIconAsset");
		if (base.gameObject.activeSelf)
		{
			StartCoroutine("HandleIconAsset");
		}
	}
}
