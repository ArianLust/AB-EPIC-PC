using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData;
using ABH.Shared.Models.Generic;
using Rcs;

public class AdServiceBeaconImpl : IAdService
{
	private readonly Ads m_adService;

	private List<string> readyPlacements = new List<string>();

	private List<string> shownSoundMutingInterstitialPlacements = new List<string>();

	private float m_TimeForRefreshInSec = 600f;

	private string m_lastActiveNewsFeedPlacementId;

	[method: MethodImpl(32)]
	public event Ads.RewardResultHandler RewardResult;

	[method: MethodImpl(32)]
	public event Action<string> AdReady;

	[method: MethodImpl(32)]
	public event Action<string, int> NewsFeedContentUpdate;

	public AdServiceBeaconImpl(Ads adService)
	{
		m_adService = adService;
	}

	public bool Init()
	{
		DebugLog.Log(GetType(), " Init Rcs.Ads service -- this does nothing");
		m_adService.SetActionInvokedHandler(HandleActionInvoked);
		m_adService.SetStateChangedHandler(HandleStateChanged);
		m_adService.SetRewardResultHandler(HandleRewardResult);
		m_adService.SetSizeChangedHandler(HandleSizeChanged);
		AddPlacementsPreemptively();
		return true;
	}

	private void AddPlacementsPreemptively()
	{
		AddPlacement("RewardVideo.Eventenergy");
		AddPlacement("RewardVideo.Campaign");
	}

	private void HandleSizeChanged(string placement, int width, int height)
	{
		DebugLog.Log(GetType(), string.Format("HandleSizeChanged: placement = {0}, width = {1}, height = {2}", placement, width, height));
	}

	private void HandleStateChanged(string placement, Ads.State state)
	{
		DebugLog.Log(GetType(), " Ad placement " + placement + " state changed to: " + state);
		switch (state)
		{
		case Ads.State.Ready:
			if (!readyPlacements.Contains(placement))
			{
				readyPlacements.Add(placement);
				if (this.AdReady != null)
				{
					this.AdReady(placement);
				}
				DebugLog.Log(GetType(), " Set Placement to ready: " + placement);
			}
			break;
		case Ads.State.Hidden:
		case Ads.State.Failed:
			readyPlacements.Remove(placement);
			if (shownSoundMutingInterstitialPlacements.Contains(placement))
			{
				shownSoundMutingInterstitialPlacements.Remove(placement);
			}
			break;
		case Ads.State.Expanded:
			MutedGameSoundForPlacement(placement);
			break;
		case Ads.State.Shown:
			if (shownSoundMutingInterstitialPlacements.Contains(placement))
			{
				shownSoundMutingInterstitialPlacements.Remove(placement);
			}
			break;
		default:
			if (readyPlacements.Contains(placement))
			{
				DebugLog.Log(GetType(), " Set Placement to not ready: " + placement);
				readyPlacements.Remove(placement);
			}
			break;
		}
		if (shownSoundMutingInterstitialPlacements.Count <= 0)
		{
			DIContainerInfrastructure.AudioManager.RemoveMuteReason(1, GetType().ToString());
			DIContainerInfrastructure.AudioManager.RemoveMuteReason(0, GetType().ToString());
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("ad_blocked");
			DIContainerInfrastructure.GetCoreStateMgr().BlockAllInput(false);
		}
	}

	public bool MutedGameSoundForPlacement(string placementId)
	{
		if (!shownSoundMutingInterstitialPlacements.Contains(placementId))
		{
			shownSoundMutingInterstitialPlacements.Add(placementId);
			DIContainerInfrastructure.AudioManager.AddMuteReason(1, GetType().ToString());
			DIContainerInfrastructure.AudioManager.AddMuteReason(0, GetType().ToString());
			DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("ad_blocked");
			DIContainerInfrastructure.GetCoreStateMgr().BlockAllInput(true);
		}
		return true;
	}

	private void HandleRewardResult(string placement, Ads.RewardResult result, string voucherId)
	{
		DebugLog.Log(GetType(), string.Concat(" HandleRewardResult(", placement, ", ", result, ", ", voucherId));
		if (this.RewardResult != null)
		{
			this.RewardResult(placement, result, voucherId);
		}
	}

	private bool HandleActionInvoked(string placement, string action)
	{
		DebugLog.Log(GetType(), string.Format("HandleActionInvoked: {0}, {1}", placement, action));
		string[] array = action.Split('/');
		string text = string.Empty;
		if (array.Length > 0)
		{
			text = array[0];
		}
		if (text.Equals("OpenToons"))
		{
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			if (array.Length > 1)
			{
				text2 = array[1];
			}
			if (array.Length > 2)
			{
				text3 = array[2];
			}
			if (array.Length > 3)
			{
				text4 = array[3];
			}
			DebugLog.Log(GetType(), string.Format("HandeActionInvoked OpenToons: ChannelId={0}, groupId={1}, videoId={2}", text3, text2, text4));
			DIContainerInfrastructure.GetChannelService().DisplayToonsTv(text2, text3, text4);
			return true;
		}
		if (text.Equals("OpenShop"))
		{
			if (ClientInfo.CurrentBattleGameData != null)
			{
				BattleUIStateMgr battleUIStateMgr = UnityEngine.Object.FindObjectOfType<BattleUIStateMgr>();
				if (battleUIStateMgr != null)
				{
					battleUIStateMgr.LeavePauseButton();
					ClientInfo.CoreStateMgr.ShowShop("shop_premium", null);
					return true;
				}
				return false;
			}
			ClientInfo.CoreStateMgr.ShowShop(string.Empty, null);
			return true;
		}
		return false;
	}

	public bool StartSession()
	{
		DebugLog.Log(GetType(), "Start a new ad session");
		m_adService.StartSession();
		return true;
	}

	public bool AddPlacement(string placementId, Ads.RendererHandler onRenderableReadyCallback = null)
	{
		DebugLog.Log(GetType(), " AddPlacement: " + placementId);
		if (m_adService.GetState("LevelStartInterstitial") != Ads.State.Ready)
		{
		}
		if (onRenderableReadyCallback != null)
		{
			DebugLog.Log(GetType(), "AddPlacement: requesting renderable ad");
			m_adService.AddPlacement(placementId, onRenderableReadyCallback);
		}
		else
		{
			m_adService.AddPlacement(placementId);
		}
		return true;
	}

	public bool AddPlacement(string placementId, int x, int y, int width, int height)
	{
		DebugLog.Log(GetType(), " AddPlacement: " + placementId + ", " + x + "," + y + "," + width + "," + height);
		m_adService.AddPlacement(placementId, x, y, width, height);
		return true;
	}

	public bool AddPlacement(string placementId, float x, float y, float width, float height)
	{
		DebugLog.Log(GetType(), " AddPlacement: " + placementId + ", " + x + "," + y + "," + width + "," + height);
		m_adService.AddPlacementNormalized(placementId, x, y, width, height);
		return true;
	}

	public bool ShowAd(string placementId)
	{
		DebugLog.Log(GetType(), " ShowAd: " + placementId + " , user converted: " + DIContainerInfrastructure.GetProfileMgr().CurrentProfile.IsUserConverted);
		if (placementId.Contains("Interstitial"))
		{
			List<Requirement> showInterstitialRequirements = DIContainerConfig.GetClientConfig().ShowInterstitialRequirements;
			if (DIContainerInfrastructure.GetProfileMgr().CurrentProfile.IsUserConverted || !DIContainerLogic.RequirementService.CheckGenericRequirements(DIContainerInfrastructure.GetCurrentPlayer(), showInterstitialRequirements))
			{
				DebugLog.Log(GetType(), " Skipping ad; user is converted to pay user");
				return false;
			}
			if (IsUserInAdCooldown())
			{
				DebugLog.Log(GetType(), " Skipping ad; user is in ad cooldown");
				return false;
			}
			bool flag = m_adService.Show(placementId);
			if (flag)
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.LastAdShownTime = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
			}
			DebugLog.Log(GetType(), " Show ad for placement " + placementId + ": " + flag);
			return flag;
		}
		if (placementId.Contains("RewardVideo"))
		{
			bool flag2 = m_adService.Show(placementId);
			if (flag2)
			{
				DIContainerInfrastructure.GetCurrentPlayer().Data.LastAdShownTime = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
			}
			DebugLog.Log(GetType(), " Show ad for placement " + placementId + ": " + flag2);
			return flag2;
		}
		bool flag3 = m_adService.Show(placementId);
		if (flag3 && placementId.StartsWith("NewsFeed"))
		{
			m_lastActiveNewsFeedPlacementId = placementId;
		}
		DebugLog.Log(GetType(), " Show ad for placement " + placementId + ": " + flag3);
		return flag3;
	}

	private bool IsUserInAdCooldown()
	{
		if (!ClientInfo.IsAdCooldownEnabled)
		{
			return false;
		}
		DateTime trustedTime;
		if (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			return true;
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.CreationDate == 0)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.CreationDate = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.LastAdShownTime == 0)
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.LastAdShownTime = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
		}
		TimeSpan timeSpan = trustedTime - DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().Data.LastAdShownTime);
		DebugLog.Log(GetType(), " Time since last ad display: " + timeSpan);
		if (timeSpan.TotalSeconds > (double)GetCooldownTimeInSeconds(trustedTime))
		{
			return false;
		}
		return true;
	}

	private int GetCooldownTimeInSeconds(DateTime trustedTime)
	{
		Dictionary<int, int> adCooldownBalancing = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").AdCooldownBalancing;
		TimeSpan timeSinceCreation = trustedTime - DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().Data.CreationDate);
		if (timeSinceCreation < TimeSpan.Zero)
		{
			DebugLog.Error(GetType(), "  Negative time since creation, using zero: " + timeSinceCreation);
			timeSinceCreation = TimeSpan.Zero;
		}
		IEnumerable<int> source = adCooldownBalancing.Keys.Where((int key) => (double)key <= timeSinceCreation.TotalDays);
		int num = source.Max();
		DebugLog.Log(GetType(), string.Concat(" Checking cooldown: timeSinceCreation: ", timeSinceCreation, ", user is past x days: ", num, ", using cooldown: ", adCooldownBalancing[num]));
		return adCooldownBalancing[num];
	}

	public bool HideAd(string placementId)
	{
		DebugLog.Log(GetType(), " HidePlacement: " + placementId);
		m_adService.Hide(placementId);
		if (placementId.StartsWith("NewsFeed"))
		{
			m_lastActiveNewsFeedPlacementId = string.Empty;
		}
		return true;
	}

	public bool RemoveRenderer(string placementId)
	{
		DebugLog.Log(GetType(), " RemoveRenderer: " + placementId);
		return true;
	}

	public bool HandleClick(string placementId)
	{
		DebugLog.Log(GetType(), " HandleClicks: " + placementId);
		m_adService.HandleClick(placementId);
		return true;
	}

	public bool TrackNativeAdImpression(string placementId)
	{
		DebugLog.Log(GetType(), " TrackImpression: " + placementId);
		m_adService.TrackEvent(placementId, Ads.EventType.Impression);
		return true;
	}

	public bool IsAdShowPossible(string placementId)
	{
		return DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_sponsored_rewards") > 0 && GetState(placementId) == 3;
	}

	public bool IsNewsFeedShowPossible(string placementId)
	{
		Ads.State state = m_adService.GetState(placementId);
		DebugLog.Log(GetType(), string.Format("IsNewsFeedShowPossible: {0} returns state {1}", placementId, state));
		switch (state)
		{
		case Ads.State.Hidden:
		case Ads.State.Shown:
		case Ads.State.Expanded:
		case Ads.State.Ready:
			return true;
		default:
			return false;
		}
	}

	public class jsonBase
	{
		public List<Ad> ads { get; set; }
	}

	public class Ad
	{
		public string adName { get; set; }
		public uint showTimestamp { get; set; }
	}
	
	public int GetState(string placementId)
	{
		return (int)m_adService.GetState(placementId);
	}

	public bool SuspendNewsFeeds()
	{
		if (!string.IsNullOrEmpty(m_lastActiveNewsFeedPlacementId))
		{
			m_adService.Hide(m_lastActiveNewsFeedPlacementId);
			return true;
		}
		return false;
	}

	public bool RestoreSuspendedNewsFeed()
	{
		
		if (!string.IsNullOrEmpty(m_lastActiveNewsFeedPlacementId))
		{
			ShowAd(m_lastActiveNewsFeedPlacementId);
			m_lastActiveNewsFeedPlacementId = string.Empty;
			return true;
		}
		return false;
	}
}
