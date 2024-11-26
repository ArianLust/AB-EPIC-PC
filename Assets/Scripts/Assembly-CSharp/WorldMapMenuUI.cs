using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class WorldMapMenuUI : MonoBehaviour, IMapUI
{
	public UIInputTrigger m_CampButton;

	public UIInputTrigger m_PvpCampButton;

	public UIInputTrigger m_DailyLoginButton;

	public UIInputTrigger m_CrossPromoButton;

	public Animation m_CampButtonAnimation;

	public Animation m_PvpCampButtonAnimation;

	public Animation m_CrossPromoButtonAnimation;

	public Animation m_EventButtonListAnimation;

	public OptionsMgr m_OptionsMgr;

	[SerializeField]
	public WorldMapMenuButtonStates m_campButtonStates;

	[SerializeField]
	public WorldMapMenuButtonStates m_arenaButtonStates;

	[SerializeField]
	private GameObject m_NewGiftIndicator;

	[SerializeField]
	private GameObject m_NewGiftIndicatorTopLevel;

	[SerializeField]
	private GameObject m_AdIndicatorDailyGift;

	[SerializeField]
	public GameObject m_NewsBanner;

	[SerializeField]
	private Animator m_NewsBannerAnim;

	[SerializeField]
	private UIInputTrigger m_NewsButton;

	[SerializeField]
	private GameObject m_NewsUpdateIndicator;

	[SerializeField]
	private GameObject m_NewsLockedObject;

	[SerializeField]
	private GameObject m_SpecialOfferButtonprefab;

	[SerializeField]
	private GameObject m_EventButtonPrefab;

	[SerializeField]
	public UIGrid m_SpecialButtonGrid;

	private bool m_storySequenceVisible;

	private WorldMapStateMgr m_StateMgr;

	private bool m_CalendarUnlocked;

	private bool m_switchToGames;

	private void Awake()
	{
		m_CampButton.gameObject.SetActive(false);
		m_PvpCampButton.gameObject.SetActive(false);
		m_OptionsMgr.gameObject.SetActive(false);
		m_DailyLoginButton.gameObject.SetActive(false);
		m_CrossPromoButton.gameObject.SetActive(false);
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	private void DeRegisterEventHandler()
	{
		if ((bool)m_CampButton)
		{
			m_CampButton.Clicked -= CampButton_Clicked;
		}
		if ((bool)m_PvpCampButton)
		{
			m_PvpCampButton.Clicked -= PvpCampButton_Clicked;
		}
		if ((bool)m_DailyLoginButton)
		{
			m_DailyLoginButton.Clicked -= DailyLoginButton_Clicked;
		}
		if ((bool)m_CrossPromoButton)
		{
			m_CrossPromoButton.Clicked -= CrossPromoButton_Clicked;
		}
		if ((bool)m_NewsButton)
		{
			m_NewsButton.Clicked -= OnNewsButtonClicked;
		}
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if ((bool)m_CampButton)
		{
			m_CampButton.Clicked += CampButton_Clicked;
		}
		if ((bool)m_PvpCampButton)
		{
			m_PvpCampButton.Clicked += PvpCampButton_Clicked;
		}
		if ((bool)m_DailyLoginButton)
		{
			m_DailyLoginButton.Clicked += DailyLoginButton_Clicked;
		}
		if ((bool)m_CrossPromoButton)
		{
			m_CrossPromoButton.Clicked += CrossPromoButton_Clicked;
		}
		if ((bool)m_NewsButton)
		{
			m_NewsButton.Clicked += OnNewsButtonClicked;
		}
	}

	public void SetStateMgr(WorldMapStateMgr stateMgr)
	{
		m_StateMgr = stateMgr;
		m_StateMgr.m_isMovementPossible = () => m_OptionsMgr == null || !m_OptionsMgr.IsAnimationRunning;
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
	}

	private void CampButton_Clicked()
	{
		CoreStateMgr.Instance.GotoCampScreen();
		DeRegisterEventHandler();
	}

	private void PvpCampButton_Clicked()
	{
		CoreStateMgr.Instance.GotoPvpCampScreen();
		DeRegisterEventHandler();
	}

	private void DailyLoginButton_Clicked()
	{
		CoreStateMgr.Instance.ShowDailyLoginUI();
		DeRegisterEventHandler();
	}

	public void ComeBackFromDailyLogin()
	{
		RegisterEventHandler();
	}

	private void CrossPromoButton_Clicked()
	{
		m_switchToGames = true;
		OnNewsButtonClicked();
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			StartCoroutine(CreateHotlinkButtons());
		}
	}

	public void RecheckHotlinkButtons()
	{
		StartCoroutine(CreateHotlinkButtons());
	}

	public void Enter()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	public IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		bool allowXpromo = DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_xpromo");
		m_CrossPromoButton.gameObject.SetActive(allowXpromo);
		bool newsUnlocked = DIContainerLogic.InventoryService.GetItemValue(player.InventoryGameData, "news_introduction") >= 1;
		m_NewsBanner.SetActive(newsUnlocked);
		if (newsUnlocked)
		{
			SetupNewsBanner();
		}
		StartCoroutine(CreateHotlinkButtons());
		HandleDailyLoginBonus();
		m_CampButton.gameObject.SetActive(true);
		m_PvpCampButton.gameObject.SetActive(DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(player));
		m_CampButtonAnimation.Play("BackButton_Enter");
		m_PvpCampButtonAnimation.Play("ArenaButton_Enter");
		m_CrossPromoButtonAnimation.Play("xPromoButton_Enter");
		m_EventButtonListAnimation.Play("EventList_Enter");
		m_NewsBannerAnim.Play("NewsBanner_Enter");
		m_OptionsMgr.gameObject.SetActive(true);
		m_OptionsMgr.Enter();
		base.gameObject.GetComponent<UIPanel>().enabled = true;
		yield return new WaitForSeconds(m_CampButtonAnimation["BackButton_Enter"].length);
		m_CampButtonAnimation["BackButton_Enter"].time = m_CampButtonAnimation["BackButton_Enter"].length;
		m_CampButtonAnimation.Sample();
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_xpromo") >= 1)
		{
			DIContainerInfrastructure.AdService.AddPlacement("MainMenuPopup", OnMainCrossPromotionAdReady);
		}
		DebugLog.Log("WORLDMAP UI ENTERED");
		RegisterEventHandler();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enter_worldmap_ui", string.Empty);
	}

	private void HandleDailyLoginBonus()
	{
		m_DailyLoginButton.gameObject.SetActive(true);
		m_CalendarUnlocked = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "daily_chain_introduction") >= 1;
		if (m_CalendarUnlocked)
		{
			m_DailyLoginButton.transform.Find("Animation/Body").GetComponent<UISprite>().spriteName = "Button_Round_SubSmall";
			m_DailyLoginButton.GetComponent<BoxCollider>().enabled = true;
		}
		else
		{
			m_DailyLoginButton.transform.Find("Animation/Body").GetComponent<UISprite>().spriteName = "Button_Round_SubSmall_D";
			m_DailyLoginButton.GetComponent<BoxCollider>().enabled = false;
		}
	}

	private IEnumerator CreateHotlinkButtons()
	{
		foreach (Transform child in m_SpecialButtonGrid.transform)
		{
			Object.Destroy(child.gameObject);
		}
		yield return new WaitForEndOfFrame();
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		bool eventFound = false;
		if (player.CurrentEventManagerGameData != null && player.CurrentEventManagerGameData.CurrentEventManagerState != 0)
		{
			eventFound = true;
			GameObject eventButton = Object.Instantiate(m_EventButtonPrefab);
			eventButton.transform.parent = m_SpecialButtonGrid.transform;
			eventButton.GetComponent<WorldMapMenuHotlinkButton>().InitEvent();
		}
		List<SalesManagerBalancingData> salesSorted2 = DIContainerLogic.GetSalesManagerService().GetAllActiveSales(true);
		if (salesSorted2.Count > 0)
		{
			salesSorted2 = salesSorted2.Where((SalesManagerBalancingData sale) => sale.ContentType != SaleContentType.RainbowRiot).ToList();
			SalesManagerBalancingData saleWithHighestPrio = salesSorted2.FirstOrDefault();
			SalesManagerBalancingData saleWithSecondHighestPrio = null;
			SalesManagerBalancingData saleWithThirdHighestPrio = null;
			if (salesSorted2.Count > 1)
			{
				saleWithSecondHighestPrio = salesSorted2[1];
			}
			if (salesSorted2.Count > 2)
			{
				saleWithThirdHighestPrio = salesSorted2[2];
			}
			if (saleWithHighestPrio != null)
			{
				GameObject offerButton3 = Object.Instantiate(m_SpecialOfferButtonprefab);
				offerButton3.transform.parent = m_SpecialButtonGrid.transform;
				offerButton3.GetComponent<WorldMapMenuHotlinkButton>().InitOffer(saleWithHighestPrio);
			}
			if (saleWithSecondHighestPrio != null)
			{
				GameObject offerButton2 = Object.Instantiate(m_SpecialOfferButtonprefab);
				offerButton2.transform.parent = m_SpecialButtonGrid.transform;
				offerButton2.GetComponent<WorldMapMenuHotlinkButton>().InitOffer(saleWithSecondHighestPrio);
			}
			if (!eventFound && saleWithThirdHighestPrio != null)
			{
				GameObject offerButton = Object.Instantiate(m_SpecialOfferButtonprefab);
				offerButton.transform.parent = m_SpecialButtonGrid.transform;
				offerButton.GetComponent<WorldMapMenuHotlinkButton>().InitOffer(saleWithThirdHighestPrio);
			}
		}
		m_SpecialButtonGrid.Reposition();
	}

	private void SetupNewsBanner()
	{
		if (m_StateMgr.m_NewsLogic.HasNewItemsAvailable())
		{
			DebugLog.Log(GetType(), "TESTING NEWSFEED UPDATES: SetupNewsBanner: found new event, showing update indicator and skipping newsfeeds!");
			m_NewsUpdateIndicator.SetActive(true);
			return;
		}
		Dictionary<string, int> placementsWithUpdate = m_StateMgr.m_NewsLogic.GetPlacementsWithUpdate();
		foreach (KeyValuePair<string, int> item in placementsWithUpdate)
		{
			if (item.Value >= 0)
			{
				DebugLog.Log(GetType(), "TESTING NEWSFEED UPDATES: SetupNewsBanner: found " + item.Value + " updates for " + item.Key);
				m_NewsUpdateIndicator.SetActive(true);
				return;
			}
		}
		DebugLog.Log(GetType(), "TESTING NEWSFEED UPDATES: SetupNewsBanner: found no updates. hiding news indicator on worldmap!");
		m_NewsUpdateIndicator.SetActive(false);
	}

	private void LeaveUi()
	{
		CoreStateMgr coreStateMgr = DIContainerInfrastructure.GetCoreStateMgr();
		coreStateMgr.m_GenericUI.LeaveLevelDisplay();
		coreStateMgr.m_GenericUI.DeRegisterBar(0u);
		if ((bool)m_CampButtonAnimation)
		{
			m_CampButtonAnimation.Play("BackButton_Leave");
		}
		if ((bool)m_CrossPromoButtonAnimation)
		{
			m_CrossPromoButtonAnimation.Play("xPromoButton_Leave");
		}
		if ((bool)m_PvpCampButtonAnimation)
		{
			m_PvpCampButtonAnimation.Play("ArenaButton_Leave");
		}
		if ((bool)m_EventButtonListAnimation)
		{
			m_EventButtonListAnimation.Play("EventList_Leave");
		}
		if ((bool)m_OptionsMgr)
		{
			m_OptionsMgr.Leave();
		}
		if ((bool)m_NewsBannerAnim)
		{
			m_NewsBannerAnim.Play("NewsBanner_Leave");
		}
	}

	private void OnNewsButtonClicked()
	{
		if (!m_StateMgr.IsBirdWalking() && !DIContainerInfrastructure.GetCoreStateMgr().IsAnyPopupActive)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("UserConverted", DIContainerInfrastructure.GetCurrentPlayer().Data.IsUserConverted.ToString());
			Dictionary<string, string> dictionary2 = dictionary;
			ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary2);
			DIContainerInfrastructure.GetAnalyticsSystem(true).LogEventWithParameters("NewsButtonClicked", dictionary2);
			CoreStateMgr coreStateMgr = DIContainerInfrastructure.GetCoreStateMgr();
			coreStateMgr.m_WindowRoot.Enter();
			coreStateMgr.m_GenericUI.LeaveLevelDisplay();
			NewsUi.NewsUiState startingState = (m_switchToGames ? NewsUi.NewsUiState.NewsFeed : NewsUi.NewsUiState.Events);
			m_StateMgr.ShowNewsUi(startingState);
			m_switchToGames = false;
		}
	}

	private bool OnMainCrossPromotionAdReady(string placement, string contentType, List<byte> content)
	{
		DeRegisterEventHandler();
		DebugLog.Log(GetType(), "OnMainCrossPromotionAdReady");
		if (m_OptionsMgr.m_AdCanvas == null)
		{
			return false;
		}
		LeaveUi();
		bool flag = m_OptionsMgr.m_AdCanvas.Hatch2_OnRenderableReady(placement, contentType, content, false);
		if (!flag)
		{
			ComebackFromCrossPromoAd();
		}
		return flag;
	}

	public void ComebackFromCrossPromoAd()
	{
		RegisterEventHandler();
		m_CampButtonAnimation.Play("BackButton_Enter");
		m_CrossPromoButtonAnimation.Play("xPromoButton_Enter");
		m_PvpCampButtonAnimation.Play("ArenaButton_Enter");
		m_EventButtonListAnimation.Play("EventList_Enter");
		m_OptionsMgr.Enter();
		m_NewsBannerAnim.Play("NewsBanner_Enter");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 0u,
			showFriendshipEssence = true,
			showLuckyCoins = true,
			showSnoutlings = true
		}, true);
	}

	public void Leave()
	{
		DebugLog.Log(GetType(), "Leave()");
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(LeaveCoroutine());
		}
	}

	public void ActivateCampButton()
	{
		if ((bool)m_CampButton)
		{
			m_CampButton.Clicked -= CampButton_Clicked;
			m_CampButton.Clicked += CampButton_Clicked;
		}
		if ((bool)m_PvpCampButton)
		{
			m_PvpCampButton.Clicked -= PvpCampButton_Clicked;
			m_PvpCampButton.Clicked += PvpCampButton_Clicked;
		}
	}

	public void DeactivateCampButton()
	{
		if ((bool)m_CampButton)
		{
			m_CampButton.Clicked -= CampButton_Clicked;
		}
		if ((bool)m_PvpCampButton)
		{
			m_PvpCampButton.Clicked -= PvpCampButton_Clicked;
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		if (m_OptionsMgr.gameObject.activeSelf)
		{
			DeRegisterEventHandler();
			m_CampButtonAnimation.Play("BackButton_Leave");
			m_PvpCampButtonAnimation.Play("ArenaButton_Leave");
			m_EventButtonListAnimation.Play("EventList_Leave");
			m_CrossPromoButtonAnimation.Play("xPromoButton_Leave");
			m_NewsBannerAnim.Play("NewsBanner_Leave");
			DIContainerInfrastructure.GetCoreStateMgr().m_ArenaLockedPopup.LeavePopup();
			DIContainerInfrastructure.GetCoreStateMgr().m_BonusCodeManager.Leave();
			DIContainerInfrastructure.GetCoreStateMgr().m_DailyLoginUi.ClosePopup();
			yield return new WaitForSeconds(Mathf.Max(m_CampButtonAnimation["BackButton_Leave"].length, m_OptionsMgr.GetLeaveTime()));
			m_CampButton.gameObject.SetActive(false);
			m_PvpCampButton.gameObject.SetActive(false);
			m_CrossPromoButton.gameObject.SetActive(false);
			m_OptionsMgr.gameObject.SetActive(false);
			base.gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr())
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
			if (DIContainerInfrastructure.GetCoreStateMgr().m_BonusCodeManager != null)
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_BonusCodeManager.Leave();
			}
		}
		DeRegisterEventHandler();
	}

	public void CheckForNewGiftMarker()
	{
		m_NewGiftIndicator.SetActive(m_CalendarUnlocked && !DIContainerLogic.DailyLoginLogic.m_ClaimedToday);
		m_NewGiftIndicatorTopLevel.SetActive(m_CalendarUnlocked && !DIContainerLogic.DailyLoginLogic.m_ClaimedToday);
		m_AdIndicatorDailyGift.SetActive(m_CalendarUnlocked && DIContainerLogic.DailyLoginLogic.IsVideoRewardAvailable());
	}
}
