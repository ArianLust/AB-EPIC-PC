using System;
using System.Collections.Generic;
using ABH.Shared.BalancingData;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AchievementServiceGamecenterImpl : AchievementServiceBase, IAchievementService
{
	private Action<float> m_progressCallback;

	private List<string> m_unlockedAchievements;

	public bool? IsSignedIn { get; private set; }

	public void Init(IMonoBehaviourContainer mainInstance, bool mayUseUI)
	{
		DebugLog.Log(GetType(), "Init");
		SignInToGamecenter();
	}

	private void SignInToGamecenter()
	{
		DebugLog.Log(GetType(), "SignInToGamecenter");
		if (Social.localUser.authenticated)
		{
			return;
		}
		DebugLog.Log(GetType(), "Authenticate");
		Social.localUser.Authenticate(delegate(bool success)
		{
			DebugLog.Log(GetType(), "Authenticate success:" + success);
			if (success)
			{
				IsSignedIn = true;
				ReReportAllUnlockedAchievements();
				Social.LoadAchievements(OnGotAllAchievements);
			}
			else
			{
				IsSignedIn = false;
			}
		});
	}

	private void OnGotAllAchievements(IAchievement[] achievements)
	{
		m_unlockedAchievements = new List<string>();
		foreach (IAchievement achievement in achievements)
		{
			if (achievement.completed)
			{
				m_unlockedAchievements.Add(achievement.id);
			}
		}
	}

	public void ShowAchievementUI()
	{
		if (!Social.localUser.authenticated)
		{
			SignInToGamecenter();
		}
		else
		{
			Social.ShowAchievementsUI();
		}
	}

	public void ReportProgress(string achievementId, double progress)
	{
		Social.ReportProgress(achievementId, progress, delegate
		{
		});
	}

	public override void ReportUnlocked(string achievementId)
	{
		Social.ReportProgress(achievementId, 100.0, delegate
		{
		});
	}

	public string GetAchievementIdForStoryItemIfExists(string storyItem)
	{
		ThirdPartyIdBalancingData balancing = null;
		if (DIContainerBalancing.Service.TryGetBalancingData<ThirdPartyIdBalancingData>(storyItem, out balancing) && balancing.GamecenterAchievementId != null)
		{
			return balancing.GamecenterAchievementId;
		}
		return null;
	}

	public void GetGlobalAchievementProgress(Action<float> progressCallback)
	{
		m_progressCallback = progressCallback;
		Social.LoadAchievements(processAchievements);
	}

	private void processAchievements(IAchievement[] achievementList)
	{
		DebugLog.Log("[AchievementServiceGamecenterImpl] Getting achievements from gamecenter, count: " + achievementList.Length);
		float obj = (float)achievementList.Length / (float)DIContainerBalancing.Service.GetBalancingDataList<ThirdPartyIdBalancingData>().Count((ThirdPartyIdBalancingData a) => !string.IsNullOrEmpty(a.GamecenterAchievementId));
		if (m_progressCallback != null)
		{
			m_progressCallback(obj);
		}
	}

	public List<string> GetUnlockedAchievements()
	{
		return m_unlockedAchievements;
	}
}
