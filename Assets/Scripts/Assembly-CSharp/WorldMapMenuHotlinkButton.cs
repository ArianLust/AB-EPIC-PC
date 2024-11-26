using System;
using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class WorldMapMenuHotlinkButton : MonoBehaviour
{
	[SerializeField]
	private UISprite m_SpecialOfferSprite;

	[SerializeField]
	private UILabel m_SpecialOfferTimer;

	[SerializeField]
	private UIInputTrigger m_ButtonTrigger;

	[SerializeField]
	private GameObject m_IconPrefabContainer;

	[SerializeField]
	private GameObject m_LoadingSpinnerRoot;

	[SerializeField]
	private CharacterHealthBar m_HealthBarBoss;

	[SerializeField]
	private Animation m_BossReviveTimerAnimation;

	[SerializeField]
	private UILabel m_BossReviveTimerLabel;

	[SerializeField]
	private UILabel m_bossReviveTextLabel;

	private DateTime m_targetTime;

	private SalesManagerBalancingData m_shopBalancing;

	private EventManagerGameData m_EventModel;

	private bool m_IsBossCooldownActive;

	[SerializeField]
	private GameObject m_FinishedHighlight;

	private void OnDestroy()
	{
		m_ButtonTrigger.Clicked -= OnEventButtonClicked;
		m_ButtonTrigger.Clicked -= OnSpecialOfferButtonClicked;
		m_IsBossCooldownActive = false;
	}

	public void InitEvent()
	{
		if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			m_EventModel = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
			m_targetTime = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(m_EventModel.Balancing.EventEndTimeStamp);
			if (m_EventModel.CurrentEventManagerState >= EventManagerState.Finished)
			{
				m_FinishedHighlight.SetActive(true);
				m_SpecialOfferTimer.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_finished", "Finished!");
			}
			else
			{
				StartCoroutine(ShowTimer());
			}
			StartCoroutine(HandleEventIcon());
			if (m_EventModel.IsBossEvent)
			{
				if (DIContainerLogic.EventSystemService.IsBossOnCooldown())
				{
					ShowBossCooldownTimer();
				}
				else
				{
					StartCoroutine(ShowHealthbar());
				}
			}
			m_ButtonTrigger.Clicked -= OnEventButtonClicked;
			m_ButtonTrigger.Clicked += OnEventButtonClicked;
		}
		else
		{
			DebugLog.Error(GetType(), "InitEvent: Event unavailable or invalid!");
		}
	}

	public void InitOffer(SalesManagerBalancingData saleBalancing)
	{
		m_shopBalancing = saleBalancing;
		int remainingSaleDuration = DIContainerLogic.GetSalesManagerService().GetRemainingSaleDuration(saleBalancing);
		m_targetTime = DIContainerLogic.GetTimingService().GetPresentTime().AddSeconds(remainingSaleDuration);
		SetSpecialOfferIcon();
		StartCoroutine(ShowTimer());
		m_ButtonTrigger.Clicked -= OnSpecialOfferButtonClicked;
		m_ButtonTrigger.Clicked += OnSpecialOfferButtonClicked;
	}

	private IEnumerator ShowTimer()
	{
		m_SpecialOfferTimer.text = DIContainerInfrastructure.GetLocaService().Tr("event_banner_calculating", "Calculating!");
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		while (m_targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				m_SpecialOfferTimer.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(DIContainerLogic.GetTimingService().TimeLeftUntil(m_targetTime));
			}
			yield return new WaitForSeconds(1f);
		}
		if (m_shopBalancing != null)
		{
			RemoveOffer();
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (m_EventModel != null)
		{
			m_FinishedHighlight.SetActive(true);
		}
	}

	private void SetSpecialOfferIcon()
	{
		if (DIContainerInfrastructure.GetShopIconAtlasAssetProvider().ContainsAsset(m_shopBalancing.PopupAtlasId))
		{
			GameObject gameObject = DIContainerInfrastructure.GetShopIconAtlasAssetProvider().GetObject(m_shopBalancing.PopupAtlasId) as GameObject;
			if (gameObject != null)
			{
				m_SpecialOfferSprite.atlas = gameObject.GetComponent<UIAtlas>();
				m_SpecialOfferSprite.spriteName = m_shopBalancing.PopupIconId.Replace("ShopOffer", "Icon");
			}
			else
			{
				Debug.LogError("atlasGob is null!", base.gameObject);
			}
		}
		else
		{
			GameObject gameObject2 = DIContainerInfrastructure.GetShopIconAtlasAssetProvider().GetObject("ShopIconElements") as GameObject;
			m_SpecialOfferSprite.atlas = gameObject2.GetComponent<UIAtlas>();
			m_SpecialOfferSprite.spriteName = "Icon_Default";
		}
		m_SpecialOfferSprite.MakePixelPerfect();
	}

	private IEnumerator HandleEventIcon()
	{
		EventManagerGameData eventModel = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		while (eventModel == null || (!eventModel.IsAssetValid && m_IconPrefabContainer.transform.childCount == 0))
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
			GameObject eventIcon = DIContainerInfrastructure.EventSystemStateManager.InstantiateEventObject("Icon", m_IconPrefabContainer.transform);
			if ((bool)eventIcon)
			{
				eventIcon.transform.localScale = Vector3.one;
			}
		}
	}

	private void OnSpecialOfferButtonClicked()
	{
		string text = m_shopBalancing.CheckoutCategory;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("OfferID", m_shopBalancing.NameId);
		dictionary.Add("ShopCategory", text);
		dictionary.Add("SpecialOfferPrio", m_shopBalancing.SortPriority.ToString());
		dictionary.Add("IconID", m_shopBalancing.PopupIconId);
		dictionary.Add("UserConverted", DIContainerInfrastructure.GetCurrentPlayer().Data.IsUserConverted.ToString());
		Dictionary<string, string> parameters = dictionary;
		DIContainerInfrastructure.GetAnalyticsSystem(true).LogEventWithParameters("HotlinkButtonClicked", parameters);
		if (text == "shop_global_premium" || text == "shop_global_premium_soft")
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.GetLuckyCoinController().SwitchToShop("HotlinkButton");
		}
		if (text == "shop_dojo_mastery")
		{
			if (DIContainerInfrastructure.LocationStateMgr is WorldMapStateMgr)
			{
				(DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr).ZoomToDojo();
			}
			return;
		}
		if (text == "global_shop_01_potions")
		{
			text = "shop_global_consumables";
		}
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop(text, delegate
		{
		});
	}

	private void OnEventButtonClicked()
	{
		DIContainerLogic.EventSystemService.CheckoutClicked(m_EventModel);
	}

	private IEnumerator ShowHealthbar()
	{
		EventSystemWorldMapStateMgr eventSystemWorldMapStateMgr = DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr;
		while (eventSystemWorldMapStateMgr.m_WorldMapBossCombatant == null || !eventSystemWorldMapStateMgr.m_BossInitialized)
		{
			yield return new WaitForSeconds(1f);
		}
		m_HealthBarBoss.SetModel(eventSystemWorldMapStateMgr.m_WorldMapBossCombatant);
		m_HealthBarBoss.gameObject.PlayAnimationOrAnimatorState("HealthBar_Show");
	}

	public void SetHealth(int currentHealth, int previuousHealth)
	{
		if (currentHealth < previuousHealth)
		{
			m_HealthBarBoss.gameObject.PlayAnimationOrAnimatorState("HealthBar_Damage");
		}
		m_HealthBarBoss.UpdateHealth();
	}

	public void ShowBossCooldownTimer()
	{
		if (!m_IsBossCooldownActive)
		{
			m_IsBossCooldownActive = true;
			m_BossReviveTimerAnimation.Play("Show");
			StartCoroutine(SetBossCooldownLabel());
		}
	}

	public IEnumerator SetBossCooldownLabel()
	{
		if (m_IsBossCooldownActive)
		{
			while (DIContainerLogic.EventSystemService.IsBossOnCooldown())
			{
				string locaIdent = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentEventBoss.BalancingData.DefeatedLabelLocaId;
				m_bossReviveTextLabel.text = DIContainerInfrastructure.GetLocaService().Tr(locaIdent);
				m_BossReviveTimerLabel.text = DIContainerLogic.EventSystemService.GetFormattedBossCooldown();
				yield return new WaitForSeconds(1f);
			}
			HideBossCooldownTimer();
			StartCoroutine(ShowHealthbar());
		}
	}

	public void HideBossCooldownTimer()
	{
		if (m_IsBossCooldownActive)
		{
			m_IsBossCooldownActive = false;
			m_BossReviveTimerAnimation.Play("Hide");
		}
	}

	private void RemoveOffer()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (currentPlayer.Data.CurrentSpecialShopOffers != null)
		{
			currentPlayer.Data.CurrentSpecialShopOffers.Remove(m_shopBalancing.NameId);
		}
		DebugLog.Log("[SpecialOffersBlind] Removed Special Offer: " + m_shopBalancing.NameId);
		DIContainerLogic.GetSalesManagerService().UpdateSales();
	}
}
