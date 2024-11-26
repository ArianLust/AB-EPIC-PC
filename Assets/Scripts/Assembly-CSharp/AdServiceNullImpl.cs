using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Rcs;
using UnityEngine;

public class AdServiceNullImpl : IAdService
{
	private string m_currentPlacementId;

	[method: MethodImpl(32)]
	public event Action<string> AdReady;

	[method: MethodImpl(32)]
	public event Action<string, int> NewsFeedContentUpdate;

	[method: MethodImpl(32)]
	public event Ads.RewardResultHandler RewardResult;

	public bool Init()
	{
		DebugLog.Log("[AdServiceNullImpl] Init null impl of ad service");
		return true;
	}

	public bool StartSession()
	{
		DebugLog.Log("[AdServiceNullImpl] Start a new ad session");
		return true;
	}

	public bool AddPlacement(string placementId, Ads.RendererHandler onRenderableReadyCallback = null)
	{
		DebugLog.Log("[AdServiceNullImpl] AddPlacement: " + placementId);
		return true;
	}

	public bool AddPlacement(string placementId, int x, int y, int width, int height)
	{
		DebugLog.Log("[AdServiceNullImpl] AddPlacement: " + placementId + ", " + x + "," + y + "," + width + "," + height);
		return true;
	}

	public bool ShowAd(string placementId)
	{
		DebugLog.Log("[AdServiceNullImpl] ShowAd: " + placementId);
		if (this.RewardResult != null)
		{
			this.RewardResult(placementId, Ads.RewardResult.RewardCompleted, null);
			this.RewardResult(placementId, Ads.RewardResult.RewardConfirmed, null);
		}
		return false;
	}

	public bool HideAd(string placementId)
	{
		DebugLog.Log("[AdServiceNullImpl] HidePlacement: " + placementId);
		return true;
	}

	public bool RemoveRenderer(string placementId)
	{
		DebugLog.Log("[AdServiceNullImpl] RemoveRenderer: " + placementId);
		return true;
	}

	public bool HandleClick(string placementId)
	{
		DebugLog.Log("[AdServiceNullImpl] HandleClick: " + placementId);
		return true;
	}

	public bool TrackNativeAdImpression(string placementId)
	{
		DebugLog.Log("[AdServiceNullImpl] TrackImpression: " + placementId);
		return true;
	}

	public bool IsAdShowPossible(string placementId)
	{
		return false;
	}

	public bool IsNewsFeedShowPossible(string placementId)
	{
		return false;
	}

	public int GetState(string placementId)
	{
		return 1;
	}

	public bool SuspendNewsFeeds()
	{
		return false;
	}

	public bool RestoreSuspendedNewsFeed()
	{
		return false;
	}

	public bool MutedGameSoundForPlacement(string placementId)
	{
		m_currentPlacementId = placementId;
		DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(SendRewardResult());
		return true;
	}

	private IEnumerator SendRewardResult()
	{
		yield return new WaitForSeconds(0.5f);
		if (this.RewardResult != null)
		{
			this.RewardResult(m_currentPlacementId, Ads.RewardResult.RewardCompleted, string.Empty);
		}
	}

	public bool AddPlacement(string placementId, float x, float y, float width, float height)
	{
		return true;
	}
}
