using System;
using System.Collections.Generic;
using ABH.Shared.BalancingData;

public class AchievementServiceAndroidImpl : IAchievementService
{
	private GooglePlayServicesManager m_googlePlayServicesManager;

	private bool m_isInitialized;

	public bool? IsSignedIn { get; private set; }

	public void Init(IMonoBehaviourContainer mainInstance, bool mayUseUI)
	{
		DebugLog.Log("[AchievementServiceAndroidImpl] Init");
		if (!m_isInitialized)
		{
			mainInstance.AddComponentSafely(ref m_googlePlayServicesManager);
			if ((bool)m_googlePlayServicesManager)
			{
				GooglePlayServicesManager googlePlayServicesManager = m_googlePlayServicesManager;
				googlePlayServicesManager.OnSignedIn = (Action)Delegate.Combine(googlePlayServicesManager.OnSignedIn, new Action(OnSignedIn));
				GooglePlayServicesManager googlePlayServicesManager2 = m_googlePlayServicesManager;
				googlePlayServicesManager2.OnSigninFailed = (Action)Delegate.Combine(googlePlayServicesManager2.OnSigninFailed, new Action(OnSigninFailed));
				m_isInitialized = true;
			}
		}
	}

	private void OnSignedIn()
	{
		IsSignedIn = true;
	}

	private void OnSigninFailed()
	{
		IsSignedIn = false;
	}

	public void ShowAchievementUI()
	{
		m_googlePlayServicesManager.ShowAchievementUI();
	}

	public void ReportProgress(string achievementId, double progress)
	{
		m_googlePlayServicesManager.ReportProgress(achievementId, progress);
	}

	public void ReportUnlocked(string achievementId)
	{
		m_googlePlayServicesManager.ReportUnlocked(achievementId);
	}

	public string GetAchievementIdForStoryItemIfExists(string storyItem)
	{
		DebugLog.Log("[AchievementServiceAndroidImpl] GetAchievementIdForStoryItemIfExists " + storyItem);
		ThirdPartyIdBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<ThirdPartyIdBalancingData>(storyItem);
		if (balancingData == null)
		{
			return null;
		}
		string text = null;
		text = balancingData.RovioGooglePlayAchievementId;
		DebugLog.Log("[AchievementServiceAndroidImpl]  found achievement id " + text + " for storyItem " + storyItem);
		return text;
	}

	public void GetGlobalAchievementProgress(Action<float> progressCallback)
	{
		if (progressCallback != null)
		{
			progressCallback(0f);
		}
	}

	public List<string> GetUnlockedAchievements()
	{
		return new List<string>();
	}
}
