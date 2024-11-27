using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class EventCampaignMenuUI : MonoBehaviour, IMapUI
{
	public UIInputTrigger m_CampButton;

	public Animation m_CampButtonAnimation;

	public UIInputTrigger m_WorldMapButton;

	public Animation m_WorldMapButtonAnimation;

	public CollectionProgressBar m_rewardProgress;

	public Animation m_EventButtonListAnimation;

	[SerializeField]
	private GameObject m_UpdateIndicatorCamp;

	[SerializeField]
	private GameObject m_SaleIndicatorCamp;

	[Header("News Banner")]
	[SerializeField]
	public GameObject m_NewsBanner;

	[SerializeField]
	private Animator m_NewsBannerAnim;

	[SerializeField]
	private UIInputTrigger m_NewsButton;

	[SerializeField]
	private GameObject m_NewsUpdateIndicator;

	[Header("Event Buttons")]
	[SerializeField]
	private GameObject m_SpecialOfferButtonprefab;

	[SerializeField]
	private GameObject m_EventButtonPrefab;

	[SerializeField]
	private UIGrid m_SpecialButtonGrid;

	private bool m_storySequenceVisible;

	private EventCampaignStateMgr m_StateMgr;

	private SalesManagerBalancingData m_priotizedSpecialOffer;

	private void Awake()
	{
		m_CampButton.gameObject.SetActive(false);
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged -= OnGlobalEventStateHasChanged;
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged += OnGlobalEventStateHasChanged;
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if ((bool)m_WorldMapButton)
		{
			m_WorldMapButton.Clicked += WorldMapButton_Clicked;
		}
		if ((bool)m_CampButton)
		{
			m_CampButton.Clicked += CampButton_Clicked;
		}
		if ((bool)m_NewsButton)
		{
			m_NewsButton.Clicked += OnNewsButtonClicked;
		}
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged += OnGlobalEventStateHasChanged;
	}

	private void OnNewsButtonClicked()
	{
		if (!m_StateMgr.IsBirdWalking())
		{
			CoreStateMgr coreStateMgr = DIContainerInfrastructure.GetCoreStateMgr();
			coreStateMgr.m_WindowRoot.Enter();
			coreStateMgr.m_GenericUI.LeaveLevelDisplay();
			m_StateMgr.m_EventNews.Enter();
		}
	}

	public void SetStateMgr(EventCampaignStateMgr stateMgr)
	{
		m_StateMgr = stateMgr;
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
	}

	private void CampButton_Clicked()
	{
		CoreStateMgr.Instance.GotoCampScreen();
		DeRegisterEventHandler();
	}

	public void UpdateCollectionBar()
	{
		m_rewardProgress.SetSlotModels();
		m_rewardProgress.UpdateProgressStatus();
	}

	public void Enter()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		foreach (Transform child in m_SpecialButtonGrid.transform)
		{
			Object.Destroy(child.gameObject);
		}
		yield return new WaitForEndOfFrame();
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		SetupSpecialsGrid();
		m_CampButton.gameObject.SetActive(true);
		m_CampButtonAnimation.Play("BackButton_Enter");
		base.gameObject.GetComponent<UIPanel>().enabled = true;
		m_WorldMapButtonAnimation.Play("BackButton_Enter");
		m_NewsBannerAnim.Play("NewsBanner_Enter");
		m_EventButtonListAnimation.Play("EventList_Enter");
		bool newsUnlocked = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "news_introduction") >= 1;
		m_NewsBanner.SetActive(newsUnlocked);
		if (newsUnlocked)
		{
			SetupNewsBanner();
		}
		if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			m_rewardProgress.Enter();
		}
		yield return new WaitForSeconds(m_CampButtonAnimation["BackButton_Enter"].length);
		DebugLog.Log("WORLDMAP UI ENTERED");
		RegisterEventHandler();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enter_worldmap_ui", string.Empty);
	}

	private IEnumerator TryEnterEventBanner()
	{
		while (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData == null)
		{
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void SetupSpecialsGrid()
	{
		List<SalesManagerBalancingData> allActiveSales = DIContainerLogic.GetSalesManagerService().GetAllActiveSales(true);
		if (allActiveSales.Count > 0)
		{
			SalesManagerBalancingData salesManagerBalancingData = null;
			for (int i = 0; i < allActiveSales.Count; i++)
			{
				SalesManagerBalancingData salesManagerBalancingData2 = allActiveSales[i];
				if ((salesManagerBalancingData == null || salesManagerBalancingData2.SortPriority < salesManagerBalancingData.SortPriority) && salesManagerBalancingData2.SortPriority != 0)
				{
					salesManagerBalancingData = salesManagerBalancingData2;
				}
			}
			m_priotizedSpecialOffer = salesManagerBalancingData;
			if (m_priotizedSpecialOffer != null)
			{
				GameObject gameObject = Object.Instantiate(m_SpecialOfferButtonprefab);
				gameObject.transform.parent = m_SpecialButtonGrid.transform;
				gameObject.GetComponent<WorldMapMenuHotlinkButton>().InitOffer(m_priotizedSpecialOffer);
			}
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData != null && DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentEventManagerState != 0)
		{
			GameObject gameObject2 = Object.Instantiate(m_EventButtonPrefab);
			gameObject2.transform.parent = m_SpecialButtonGrid.transform;
			gameObject2.GetComponent<WorldMapMenuHotlinkButton>().InitEvent();
		}
		m_SpecialButtonGrid.Reposition();
	}

	private void SetupNewsBanner()
	{
		m_NewsUpdateIndicator.SetActive(m_StateMgr.m_NewsLogic.HasNewItemsAvailable());
	}

	public void Leave()
	{
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
	}

	public void DeactivateCampButton()
	{
		if ((bool)m_CampButton)
		{
			m_CampButton.Clicked -= CampButton_Clicked;
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandler();
		m_CampButtonAnimation.Play("BackButton_Leave");
		m_NewsBannerAnim.Play("NewsBanner_Leave");
		m_EventButtonListAnimation.Play("EventList_Leave");
		m_WorldMapButtonAnimation.Play("BackButton_Leave");
		yield return new WaitForSeconds(m_WorldMapButtonAnimation["BackButton_Leave"].length);
		m_rewardProgress.gameObject.PlayAnimationOrAnimatorState("RewardProgress_Leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_BonusCodeManager.Leave();
		m_CampButton.gameObject.SetActive(false);
		base.gameObject.SetActive(false);
	}

	private void OnGlobalEventStateHasChanged(CurrentGlobalEventState oldState, CurrentGlobalEventState newState)
	{
		DebugLog.Log("Event ui changed");
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

	private void DeRegisterEventHandler()
	{
		if ((bool)m_WorldMapButton)
		{
			m_WorldMapButton.Clicked -= WorldMapButton_Clicked;
		}
		if ((bool)m_CampButton)
		{
			m_CampButton.Clicked -= CampButton_Clicked;
		}
		if ((bool)m_NewsButton)
		{
			m_NewsButton.Clicked -= OnNewsButtonClicked;
		}
		DIContainerInfrastructure.GetCurrentPlayer().GlobalEventStateChanged -= OnGlobalEventStateHasChanged;
	}

	public void ShowSaleOnCampButton(bool show)
	{
		DebugLog.Log("Show Sale Indicator: " + show);
		if ((bool)m_SaleIndicatorCamp)
		{
			m_SaleIndicatorCamp.SetActive(show);
		}
	}

	public void ShowNewMarkerOnCampButton(bool show)
	{
		DebugLog.Log("Show New Indicator: " + show);
		if ((bool)m_UpdateIndicatorCamp)
		{
			m_UpdateIndicatorCamp.SetActive(show);
		}
	}

	private void WorldMapButton_Clicked()
	{
		CoreStateMgr.Instance.GotoWorldMap();
		DeRegisterEventHandler();
	}

	public void ComeBackFromDailyLogin()
	{
	}
}
