using System;
using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Models;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class BaseLocationStateManager : MonoBehaviour
{
	[HideInInspector]
	public int m_movementTargetIndex = -1;

	[SerializeField]
	protected FriendProgressIndicator m_FriendProgressIndicatorPrefab;

	[SerializeField]
	protected Transform m_AssetProviderRoot;

	protected bool m_isInitialized;

	private EventSystemWorldMapStateMgr m_EventSystemWorldMapStateMgr;

	private bool restedOnlyOnce;

	protected bool m_FeatureUnlocksRunning;

	public bool m_HadCutsceneError;

	private List<string> DisplayedFeaturePopups = new List<string>();

	public List<GameObject> m_Birds;

	protected IEnumerator m_FeatureUnlockCoroutineInstance;

	[SerializeField]
	protected float m_CheckForSpecialOfferFrequency = 30f;

	public bool ForceSpawnEventNodes { get; set; }

	public bool IsEventResultRunning { get; set; }

	public Camera SceneryCamera { get; protected set; }

	public EventSystemWorldMapStateMgr EventsWorldMapStateMgr
	{
		get
		{
			if (m_EventSystemWorldMapStateMgr == null)
			{
				m_EventSystemWorldMapStateMgr = base.transform.GetComponent<EventSystemWorldMapStateMgr>();
			}
			return m_EventSystemWorldMapStateMgr;
		}
	}

	public bool IsInitialized
	{
		get
		{
			return m_isInitialized;
		}
	}

	public bool FeatureUnlocksRunning
	{
		get
		{
			return m_FeatureUnlocksRunning;
		}
	}

	public bool BlockFeatureUnlocks { get; set; }

	public virtual IMapUI WorldMenuUI
	{
		get
		{
			return null;
		}
	}

	public virtual void EnableInput(bool enable)
	{
	}

	public virtual bool IsBirdWalking()
	{
		return false;
	}

	public virtual void SetNewHotSpot(HotSpotWorldMapViewBase hotSpotWorldMapViewBase, Action actionAfterWalkingDone, bool instantMove = false)
	{
		actionAfterWalkingDone();
	}
	
	public virtual void SetNewHotSpotDuringCutscene(HotSpotWorldMapViewBase hotSpotWorldMapViewBase, Action actionAfterWalkingDone, bool instantMove = false)
	{
		actionAfterWalkingDone();
	}

	public virtual float TweenCameraToTransform(Transform target)
	{
		return 0f;
	}

	public IEnumerator StoppablePopupCoroutine()
	{
		if (!m_FeatureUnlocksRunning)
		{
			if (m_FeatureUnlockCoroutineInstance != null)
			{
				StopCoroutine(m_FeatureUnlockCoroutineInstance);
			}
			m_FeatureUnlockCoroutineInstance = HandleFeatureUnlocksAndLevelUps();
			yield return StartCoroutine(m_FeatureUnlockCoroutineInstance);
		}
	}

	public void StopPopupCoroutine()
	{
		StopCoroutine(m_FeatureUnlockCoroutineInstance);
		m_FeatureUnlocksRunning = false;
		for (int i = 0; i < DisplayedFeaturePopups.Count; i++)
		{
			string item = DisplayedFeaturePopups[i];
			DIContainerInfrastructure.GetCurrentPlayer().Data.PendingFeatureUnlocks.Remove(item);
		}
		DisplayedFeaturePopups.Clear();
	}

	public IEnumerator HandleFeatureUnlocksAndLevelUps()
	{
		while (DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.IsLoading(true))
		{
			yield return new WaitForEndOfFrame();
		}
		if (BlockFeatureUnlocks)
		{
			yield break;
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_AllowCalendar = false;
		while (m_FeatureUnlocksRunning)
		{
			yield return new WaitForEndOfFrame();
		}
		WorldMenuUI.DeactivateCampButton();
		m_FeatureUnlocksRunning = true;
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		DIContainerInfrastructure.EventSystemStateManager.UpdateEventRewardStatus();
		DIContainerLogic.WorldMapService.EvaluateStarCollection(player);
		if (player.Data.PendingFeatureUnlocks != null)
		{
			for (int i = player.Data.PendingFeatureUnlocks.Count - 1; i >= 0; i--)
			{
				string cFeatureName2 = player.Data.PendingFeatureUnlocks[i];
				if (cFeatureName2.StartsWith("level_up"))
				{
					string[] splitz = cFeatureName2.Split(':');
					string levelString = string.Empty;
					int oldPowerLevelTotal = 0;
					if (splitz.Length > 0)
					{
						levelString = splitz[0].Replace("level_up_", string.Empty);
					}
					if (splitz.Length > 1)
					{
						oldPowerLevelTotal = int.Parse(splitz[1]);
					}
					int level;
					if (!int.TryParse(levelString, out level))
					{
						DebugLog.Log("[LevelUp] Parse Level failed! Take Player Level");
						level = player.Data.Level;
					}
					player.Data.PendingFeatureUnlocks.Remove(cFeatureName2);
					yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_LevelUpPopup.ShowLeveUpPopup(level, oldPowerLevelTotal).Run());
					while (DIContainerInfrastructure.GetCoreStateMgr().m_LevelUpPopup.m_IsShowing)
					{
						yield return new WaitForEndOfFrame();
					}
				}
			}
			for (int j = 0; j < player.Data.PendingFeatureUnlocks.Count; j++)
			{
				string cFeatureName = player.Data.PendingFeatureUnlocks[j];
				IInventoryItemGameData igd = null;
				DIContainerLogic.InventoryService.TryGetItemGameData(player.InventoryGameData, cFeatureName, out igd);
				DisplayedFeaturePopups.Add(cFeatureName);
				if (cFeatureName.StartsWith("daily_post"))
				{
					IInventoryItemGameData dojoItem = null;
					if (DIContainerLogic.InventoryService.TryGetItemGameData(player.InventoryGameData, "mighty_eagle_dojo", out dojoItem))
					{
						yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup.ShowSpecialOfferPopup(igd as BasicItemGameData).Run());
						while (DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup.m_IsShowing)
						{
							yield return new WaitForEndOfFrame();
						}
					}
					else
					{
						yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup.ShowUnlockFeaturePopup(igd as BasicItemGameData).Run());
						while (DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup.m_IsShowing)
						{
							yield return new WaitForEndOfFrame();
						}
					}
				}
				else if (cFeatureName == "unlock_rovio_account" || cFeatureName == "unlock_facebook")
				{
					if ((cFeatureName != "unlock_rovio_account" || !DIContainerInfrastructure.IdentityService.IsGuest()) && (cFeatureName != "unlock_facebook" || DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated()))
					{
						continue;
					}
					yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_SocialUnlockedPopup.ShowUnlockFeaturePopup(igd as BasicItemGameData).Run());
					while (DIContainerInfrastructure.GetCoreStateMgr().m_SocialUnlockedPopup.m_IsShowing)
					{
						yield return new WaitForEndOfFrame();
					}
				}
				else if ((cFeatureName.StartsWith("special_") || cFeatureName.StartsWith("star_popup_") || cFeatureName.StartsWith("collection_reward")) && cFeatureName != "special_cauldron_offer")
				{
					bool gachaUnlucked = DIContainerLogic.InventoryService.GetItemValue(player.InventoryGameData, "story_goldenpig") > 0;
					if (cFeatureName.StartsWith("special_offer_rainbow") && !gachaUnlucked)
					{
						yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup.ShowUnlockFeaturePopup(igd as BasicItemGameData).Run());
					}
					else
					{
						yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup.ShowSpecialOfferPopup(igd as BasicItemGameData).Run());
					}
					while (DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup.m_IsShowing)
					{
						yield return new WaitForEndOfFrame();
					}
				}
				else if (cFeatureName.StartsWith("hint_"))
				{
					DIContainerInfrastructure.GetCoreStateMgr().EvalulateAndShowHintPopup(igd as BasicItemGameData);
					while (DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup.m_IsShowing || DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup.m_IsShowing)
					{
						yield return new WaitForEndOfFrame();
					}
				}
				else if (cFeatureName.StartsWith("sonic_teaser_") && IsEventTeasingOrRunning("event_campaign_sonic"))
				{
					DIContainerInfrastructure.GetCoreStateMgr().m_EventLockedPopup.ShowSonicEventLockedPopup();
					while (DIContainerInfrastructure.GetCoreStateMgr().m_EventLockedPopup.m_IsShowing)
					{
						yield return new WaitForEndOfFrame();
					}
				}
				else if (cFeatureName == "unlock_enchantment")
				{
					yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup.ShowUnlockFeaturePopup(igd as BasicItemGameData).Run());
					while (DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup.m_IsShowing)
					{
						yield return new WaitForEndOfFrame();
					}
					DIContainerInfrastructure.TutorialMgr.StartTutorial("tutorial_enchantment");
				}
				else
				{
					yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup.ShowUnlockFeaturePopup(igd as BasicItemGameData).Run());
					while (DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup.m_IsShowing)
					{
						yield return new WaitForEndOfFrame();
					}
				}
				if (!FeatureUnlocksRunning)
				{
					DebugLog.Warn(GetType(), "HandleFeatureUnlocksAndLevelUps: FeatureUnlocksRunning is false -> exit loop");
					break;
				}
				DebugLog.Warn(GetType(), "HandleFeatureUnlocksAndLevelUps: Removed item: " + player.Data.PendingFeatureUnlocks[j]);
				player.Data.PendingFeatureUnlocks.Remove(cFeatureName);
				j--;
			}
		}
		yield return StartCoroutine(EliteChestPopupCoroutine());
		yield return StartCoroutine(ProcessRankUpPopUpCoroutine());
		if (DIContainerLogic.EventSystemService.IsCurrentEventAvailable(player) && DIContainerLogic.EventSystemService.IsEventRunning(player.CurrentEventManagerGameData.Balancing))
		{
			if (!player.CurrentEventManagerGameData.Data.PopupTeaserShown)
			{
				float timeX = 0f;
				while (!player.CurrentEventManagerGameData.IsAssetValid && timeX < 3f)
				{
					timeX += Time.deltaTime;
					yield return new WaitForEndOfFrame();
				}
				if (player.CurrentEventManagerGameData.IsAssetValid)
				{
					player.CurrentEventManagerGameData.Data.PopupTeaserShown = true;
					player.SavePlayerData();
					yield return StartCoroutine(ShowEventStartPopup());
				}
			}
			DIContainerInfrastructure.GetCoreStateMgr().m_LowEnergyPopup.ShowPopup();
		}
		while (DIContainerInfrastructure.GetCoreStateMgr().m_RankUpPopup.m_IsShowing)
		{
			yield return new WaitForEndOfFrame();
		}
		while (DIContainerInfrastructure.GetCoreStateMgr().m_LowEnergyPopup.m_PopupShowing || DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup.m_IsShowing)
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerLogic.GetSalesManagerService().UpdateSales();
		foreach (SalesManagerBalancingData sale in DIContainerLogic.GetSalesManagerService().GetAllActiveSales())
		{
			if (player.Data.ShownShopPopups == null)
			{
				player.Data.ShownShopPopups = new List<string>();
			}
			if (!player.Data.ShownShopPopups.Contains(sale.NameId))
			{
				if (m_FeatureUnlocksRunning)
				{
					player.Data.ShownShopPopups.Add(sale.NameId);
					player.SavePlayerData();
					yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup.ShowSpecialOfferPopup(sale).Run());
				}
				while (DIContainerInfrastructure.GetCoreStateMgr().m_SpecialOfferPopup.m_IsShowing)
				{
					yield return new WaitForEndOfFrame();
				}
				while (DIContainerInfrastructure.GetCoreStateMgr().IsShopOpen())
				{
					yield return new WaitForEndOfFrame();
				}
			}
		}
		if (DIContainerLogic.InventoryService.GetItemValue(player.InventoryGameData, "daily_chain_introduction") >= 1)
		{
			DIContainerLogic.DailyLoginLogic.IsDailyLoginInitialized();
		}
		if (DIContainerLogic.RateAppController.IsPopupAvailable())
		{
			yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_RateAppPopup.ShowRatingPopup().Run());
			while (DIContainerInfrastructure.GetCoreStateMgr().m_RateAppPopup.m_IsShowing)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		if (DIContainerLogic.NotificationPopupController.IsPopupAvailable())
		{
			yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_NotificationPopup.ShowNotificationPopup().Run());
			while (DIContainerInfrastructure.GetCoreStateMgr().m_NotificationPopup.m_IsShowing)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		WorldMenuUI.ActivateCampButton();
		DIContainerInfrastructure.GetCoreStateMgr().m_AllowCalendar = true;
		m_FeatureUnlocksRunning = false;
	}

	protected virtual IEnumerator ProcessWorldBossVictories()
	{
		yield break;
	}

	private bool IsEventTeasingOrRunning(string eventId)
	{
		ITimingService timingService = DIContainerLogic.GetTimingService();
		foreach (EventPopupBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<EventPopupBalancingData>())
		{
			if (eventId == balancingData.NameId)
			{
				return balancingData.StartTimeStamp != 0 && timingService.IsAfter(timingService.GetDateTimeFromTimestamp(balancingData.StartTimeStamp)) && timingService.IsBefore(timingService.GetDateTimeFromTimestamp(balancingData.EndTimeStamp));
			}
		}
		return false;
	}

	public void ProcessRankUpPopUp()
	{
		StartCoroutine(ProcessRankUpPopUpCoroutine());
	}

	private IEnumerator EliteChestPopupCoroutine()
	{
		EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		if (!DIContainerLogic.EventSystemService.IsChestRewardPending(currentEventManagerGameData))
		{
			yield break;
		}
		BasicItemGameData chestItem = DIContainerInfrastructure.EventSystemStateManager.CreateChestRewardPopupItem(currentEventManagerGameData);
		if (chestItem != null)
		{
			yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_FeatureUnlockedPopup.ShowUnlockFeaturePopup(chestItem).Run());
		}
		if (!DIContainerInfrastructure.GetCoreStateMgr().m_EliteChestUnlockPopup.m_IsShowing)
		{
			ShowEliteChestUnlockUI();
			while (DIContainerInfrastructure.GetCoreStateMgr().m_EliteChestUnlockPopup.m_IsShowing)
			{
				yield return new WaitForEndOfFrame();
			}
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.PendingClassRankUps != null)
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.PendingClassRankUps.Clear();
			}
		}
	}

	private IEnumerator ProcessRankUpPopUpCoroutine()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.PendingClassRankUps != null && DIContainerInfrastructure.GetCurrentPlayer().Data.PendingClassRankUps.Count > 0 && !DIContainerInfrastructure.GetCoreStateMgr().m_RankUpPopup.m_IsShowing)
		{
			Dictionary<string, int> rankUps = DIContainerInfrastructure.GetCurrentPlayer().Data.PendingClassRankUps;
			yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_RankUpPopup.ShowRankUpPopup(DIContainerInfrastructure.GetCurrentPlayer().Data.PendingClassRankUps).Run());
			while (DIContainerInfrastructure.GetCoreStateMgr().m_RankUpPopup.m_IsShowing)
			{
				yield return new WaitForEndOfFrame();
			}
			DIContainerInfrastructure.GetCurrentPlayer().Data.PendingClassRankUps.Clear();
		}
	}

	protected virtual IEnumerator ShowEventStartPopup()
	{
		ShowEventPreviewScreen(DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData, true);
		while (DIContainerInfrastructure.GetCoreStateMgr().m_eventTeaserScreen == null || DIContainerInfrastructure.GetCoreStateMgr().m_eventTeaserScreen.gameObject.activeSelf)
		{
			yield return new WaitForEndOfFrame();
		}
	}

	public virtual Vector3 GetWorldBirdScale()
	{
		return Vector3.one;
	}

	public virtual GameObject GetEmoteBubble(string spriteName, Vector3 offset, Transform root, UIAtlas atlas)
	{
		return null;
	}

	public virtual void ShowBattlePreperationScreen()
	{
	}

	public virtual void SetFriendshipGateHotspot(HotspotGameData model)
	{
	}

	public virtual void ShowFriendshipGateScreen(Action unlockAction, HotspotGameData hotspot)
	{
	}

	public virtual void ShowWorkshopScreen(string param, HotSpotWorldMapViewBase hotspot)
	{
	}

	public virtual void ShowEventDetailScreen(EventManagerGameData evt)
	{
	}

	public virtual void ShowEventPreviewScreen(EventManagerGameData eMgr = null, bool showStarting = false, string origin = null)
	{
	}

	public virtual void ShowEventResultPopup()
	{
	}

	public virtual void ShowLeaderBoardScreen(WorldBossTeamData ownTeam = null, WorldBossTeamData enemyTeam = null, EventDetailUI detailUi = null)
	{
	}

	public virtual void ShowWitchHutScreen(string param, HotSpotWorldMapViewBase hotspot)
	{
	}

	public virtual void ShowTrainerScreen(string param, HotSpotWorldMapViewBase hotspot)
	{
	}

	public virtual void ShowDojoScreen(string param, HotSpotWorldMapViewBase hotspot)
	{
	}

	public virtual bool IsShowContentPossible()
	{
		return true;
	}

	public virtual ChronicleCaveFloorGameData GetCurrentFloor()
	{
		return null;
	}

	public virtual void StartBattle(HotspotGameData m_HotspotGameData, List<BirdGameData> list, BattleParticipantTableBalancingData m_GoldenPigAddtion, bool hardmode = false)
	{
	}

	public void ResolveCutsceneError()
	{
		if (m_HadCutsceneError)
		{
			m_HadCutsceneError = false;
			DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
		}
	}

	public GenericAssetProvider GetAssetProviderByNameId(string m_AssetProviderNameId)
	{
		Transform transform = m_AssetProviderRoot.Find(m_AssetProviderNameId);
		if ((bool)transform)
		{
			return transform.GetComponent<GenericAssetProvider>();
		}
		return null;
	}

	public virtual GameObject GetBird(string str)
	{
		foreach (GameObject bird in m_Birds)
		{
			if (bird.name == str)
			{
				return bird;
			}
		}
		return null;
	}

	public virtual void ResetBirdPositions()
	{
	}

	public virtual bool ShowNewsUi(NewsUi.NewsUiState startingState = NewsUi.NewsUiState.Events)
	{
		return false;
	}

	public virtual void ShowEliteChestUnlockUI()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_EliteChestUnlockPopup.Init();
	}

	protected virtual void CheckForSpecialOffer()
	{
		DIContainerLogic.GetSalesManagerService().UpdateSales();
	}
}
