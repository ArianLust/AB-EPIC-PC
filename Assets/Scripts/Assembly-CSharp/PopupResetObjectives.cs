using System;
using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Rcs;
using UnityEngine;

public class PopupResetObjectives : MonoBehaviour
{
	[SerializeField]
	private List<UIPanel> m_Panels;

	[SerializeField]
	public UIInputTrigger m_AbortButton;

	[SerializeField]
	private UIInputTrigger m_RefreshButton;

	[SerializeField]
	private Animation m_AbortButtonAnimation;

	[SerializeField]
	private Animation m_RefreshButtonAnimation;

	[SerializeField]
	private ResourceCostBlind m_RefreshButtonCost;

	[SerializeField]
	private UILabel m_InfoTextLabel;

	[SerializeField]
	[Header("Objectives")]
	private DailyObjectiveDetailElement m_ObjectivePrefab;

	[SerializeField]
	private Transform m_ObjectivesGrid;

	[SerializeField]
	private Animation m_GridAnimation;

	[SerializeField]
	[Header("Video Ad Object")]
	private GameObject m_ObjectivesVideoObject;

	[SerializeField]
	private GameObject m_ObjectivesTimerObject;

	[SerializeField]
	private UILabel m_ObjectivesTimerText;

	[SerializeField]
	private GameObject m_AdPendingSpinner;

	[SerializeField]
	private Animation m_AdRefreshAnimation;

	[SerializeField]
	private GameObject m_FreeRollIndicator;

	[SerializeField]
	private UIInputTrigger m_SponsoredRefreshButton;

	private static string REFRESHOBJECTIVES_PLACEMENT = "RewardVideo.Objectives";

	private float m_lastAdCancelledTime;

	private float m_lastAdCompletedTime;

	private bool m_isFree;

	private ArenaCampStateMgr m_stateMgr;

	private int m_counter;

	private bool m_RefreshButtonEntered;

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		AbortButtonClicked();
	}

	private void Awake()
	{
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.AdService.AddPlacement(REFRESHOBJECTIVES_PLACEMENT);
		Requirement rerollPvpObjectivesRequirement = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").RerollPvpObjectivesRequirement;
		m_RefreshButtonCost.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(rerollPvpObjectivesRequirement.NameId).AssetBaseId, null, rerollPvpObjectivesRequirement.Value, string.Empty);
		foreach (UIPanel panel in m_Panels)
		{
			panel.enabled = false;
		}
	}

	public void OnDestroy()
	{
		DeRegisterEventHandlers();
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
		}
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, HandleBackButton);
		m_AbortButton.Clicked += AbortButtonClicked;
		m_RefreshButton.Clicked += RefreshButtonClicked;
		DIContainerInfrastructure.AdService.RewardResult += RewardSponsoredAdResult;
		m_SponsoredRefreshButton.Clicked += OnSponsoredRefreshButtonClicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_AbortButton.Clicked -= AbortButtonClicked;
		m_RefreshButton.Clicked -= RefreshButtonClicked;
		DIContainerInfrastructure.AdService.RewardResult -= RewardSponsoredAdResult;
		m_SponsoredRefreshButton.Clicked -= OnSponsoredRefreshButtonClicked;
	}

	private void RefreshButtonClicked()
	{
		DeRegisterEventHandlers();
		if (m_stateMgr.RefreshObjectives(true))
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
			StartCoroutine(ShowRefreshAnimation());
		}
		else
		{
			RegisterEventHandlers();
		}
	}

	private IEnumerator ShowRefreshAnimation()
	{
		int j = 1;
		foreach (Transform element in m_ObjectivesGrid)
		{
			if (!element.GetComponent<DailyObjectiveDetailElement>().m_GameData.Data.Solved)
			{
				element.name = "ListElement_DailyObjective_Popup_" + j;
			}
			j++;
		}
		if (m_RefreshButtonEntered)
		{
			m_RefreshButtonAnimation.Play("Button_Leave");
			m_RefreshButtonEntered = false;
			m_RefreshButtonEntered = false;
		}
		m_GridAnimation.Play("Refresh");
		yield return new WaitForSeconds(2.2f);
		j = 0;
		List<PvPObjectivesGameData> objectives = DIContainerLogic.GetPvpObjectivesService().GetDailyObjectives();
		foreach (PvPObjectivesGameData gameData in objectives)
		{
			m_ObjectivesGrid.GetChild(j).GetComponent<DailyObjectiveDetailElement>().Init(gameData);
			j++;
		}
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(EnterRefreshButton());
		RegisterEventHandlers();
	}

	private void AbortButtonClicked()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		DeRegisterEventHandlers();
		StartCoroutine(LeaveCoroutine());
	}

	public void Enter(ArenaCampStateMgr stateMgr)
	{
		m_stateMgr = stateMgr;
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 1u,
			showFriendshipEssence = false,
			showLuckyCoins = true,
			showSnoutlings = false
		}, true);
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_objectives_enter");
		StartCoroutine(SetupDailyObjectives());
		foreach (UIPanel panel in m_Panels)
		{
			panel.enabled = true;
		}
		List<PvPObjectiveData> blub = DIContainerLogic.GetPvpObjectivesService().GetUnsolvedObjectives(DIContainerInfrastructure.GetCurrentPlayer());
		if (blub.Count > 0)
		{
			StartCoroutine(WaitForAdReadyStatus());
			m_InfoTextLabel.gameObject.SetActive(true);
		}
		else
		{
			m_InfoTextLabel.gameObject.SetActive(false);
		}
		m_AbortButtonAnimation.Play("Button_Enter");
		GetComponent<Animation>().Play("Enter");
		yield return StartCoroutine(EnterRefreshButton());
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_objectives_enter");
	}

	private IEnumerator SetupDailyObjectives()
	{
		foreach (Transform child in m_ObjectivesGrid)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		yield return new WaitForEndOfFrame();
		List<PvPObjectivesGameData> objectives = DIContainerLogic.GetPvpObjectivesService().GetDailyObjectives();
		foreach (PvPObjectivesGameData gameData in objectives)
		{
			DailyObjectiveDetailElement element = UnityEngine.Object.Instantiate(m_ObjectivePrefab);
			Transform objectiveTransform = element.transform;
			objectiveTransform.parent = m_ObjectivesGrid;
			objectiveTransform.localScale = Vector3.one;
			objectiveTransform.localPosition = Vector3.zero;
			element.Init(gameData);
		}
		m_ObjectivesGrid.GetComponent<UIGrid>().Reposition();
	}

	private IEnumerator LeaveCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(1u);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_objectives_leave");
		List<PvPObjectiveData> unsolvedObjectives = DIContainerLogic.GetPvpObjectivesService().GetUnsolvedObjectives(DIContainerInfrastructure.GetCurrentPlayer());
		if (unsolvedObjectives.Count > 0 && m_RefreshButtonEntered)
		{
			m_RefreshButtonAnimation.Play("Button_Leave");
			m_RefreshButtonEntered = false;
		}
		m_AbortButtonAnimation.Play("Button_Leave");
		GetComponent<Animation>().Play("Leave");
		foreach (Transform child in m_ObjectivesGrid)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		if (!DIContainerInfrastructure.GetCurrentPlayer().Data.ObjectiveVideoFreeRefreshUsed && DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "free_refresh_use") == 0 && unsolvedObjectives.Count > 0)
		{
			// m_AdRefreshAnimation.Play("SponsoredRoll_Leave"); no need to play the exit anim when it never entered
		}
		yield return new WaitForSeconds(GetComponent<Animation>()["Leave"].length);
		m_stateMgr.RefreshObjectivesPopupClosed();
		base.gameObject.SetActive(false);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_objectives_leave");
	}

	private IEnumerator WaitForAdReadyStatus()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.ObjectiveVideoFreeRefreshUsed || DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "free_refresh_use") >= 1)
		{
			DebugLog.Log(GetType(), "WaitForAdReadyStatus: Player already used refresh! breaking coroutine");
			yield break;
		}
		while (!DIContainerInfrastructure.AdService.IsAdShowPossible(REFRESHOBJECTIVES_PLACEMENT))
		{
			yield return new WaitForEndOfFrame();
		}
		// m_AdRefreshAnimation.Play("SponsoredRoll_Enter"); offline == no ads 😎
	}

	private void OnSponsoredRefreshButtonClicked()
	{
		if (DIContainerInfrastructure.AdService.IsAdShowPossible(REFRESHOBJECTIVES_PLACEMENT))
		{
			if (!DIContainerInfrastructure.AdService.ShowAd(REFRESHOBJECTIVES_PLACEMENT))
			{
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_no_ad_available", "There is currently no Ad scheduled"), "no_ad", DispatchMessage.Status.Info);
			}
			else
			{
				DIContainerInfrastructure.AdService.MutedGameSoundForPlacement(REFRESHOBJECTIVES_PLACEMENT);
			}
		}
	}

	private void RewardSponsoredAdResult(string placement, Ads.RewardResult result, string voucherId)
	{
		if (placement != REFRESHOBJECTIVES_PLACEMENT)
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
			if (m_lastAdCancelledTime > m_lastAdCompletedTime)
			{
				if (Time.time - m_lastAdCancelledTime < 60f)
				{
					OnAdAbortedForFreeObjectiveRefresh();
				}
			}
			else if (Time.time - m_lastAdCompletedTime < 60f)
			{
				OnAdWatchedForFreeObjectiveRefresh();
			}
			break;
		case Ads.RewardResult.RewardFailed:
			OnAdAbortedForFreeObjectiveRefresh();
			break;
		default:
			throw new ArgumentOutOfRangeException("result");
		}
	}

	private IEnumerator EnterRefreshButton()
	{
		List<PvPObjectiveData> blub = DIContainerLogic.GetPvpObjectivesService().GetUnsolvedObjectives(DIContainerInfrastructure.GetCurrentPlayer());
		if (blub.Count == 0)
		{
			m_RefreshButtonCost.gameObject.SetActive(false);
			m_FreeRollIndicator.SetActive(false);
			yield break;
		}
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "free_refresh_use") >= 1)
		{
			m_FreeRollIndicator.SetActive(true);
			m_RefreshButtonCost.gameObject.SetActive(false);
		}
		else
		{
			m_FreeRollIndicator.SetActive(false);
			m_RefreshButtonCost.gameObject.SetActive(true);
		}
		if (!m_RefreshButtonEntered)
		{
			m_RefreshButtonAnimation.Play("Button_Enter");
		}
		yield return new WaitForSeconds(1f);
		m_RefreshButtonEntered = true;
		RegisterEventHandlers();
	}

	private void OnAdWatchedForFreeObjectiveRefresh()
	{
		DateTime trustedTime;
		if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.TimeStampOfLastVideoObjectives = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
		}
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "free_refresh_use") <= 0)
		{
			DIContainerLogic.InventoryService.AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "free_refresh_use", 1, "sponsored_free_refresh_use");
		}
		m_AdRefreshAnimation.Play("SponsoredRoll_Leave");
		m_RefreshButtonEntered = false;
		StartCoroutine(EnterRefreshButton());
	}

	private void OnAdAbortedForFreeObjectiveRefresh()
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_advideo_cancelled", "You did not watch the whole video"));
		StartCoroutine(EnterRefreshButton());
	}
}
