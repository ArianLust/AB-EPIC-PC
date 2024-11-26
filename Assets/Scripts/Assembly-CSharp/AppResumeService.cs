using System;
using System.Security.Cryptography;
using ABH.Shared.Models;
using Assets.Scripts.Services.Infrastructure.Time;
using Rcs;
using UnityEngine;

public class AppResumeService
{
	private const float NewSessionAfterPausedSeconds = 10f;

	private const float NewSynchAfterPausedSeconds = 15f;

	private DateTime m_lastSessionPauseTimestamp = DateTime.MinValue;

	private DateTime m_lastSynchAfterPausedTimestamp = DateTime.MinValue;

	private string lastHash = string.Empty;

	private readonly MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

	public bool OnAppResumed()
	{
		DebugLog.Log("[AppResumeService] OnAppResumed...");
		if (m_lastSynchAfterPausedTimestamp == DateTime.MinValue || DIContainerLogic.GetDeviceTimingService().TimeSince(m_lastSynchAfterPausedTimestamp).TotalSeconds > 15.0)
		{
			DebugLog.Log("[AppResumeService] Check and Synch for Profile Update");
			DIContainerInfrastructure.GetCoreStateMgr().m_CronJob.RegisterCronJobBlock("app_resume");
			ABH.Shared.Models.PlayerData currentProfile = DIContainerInfrastructure.GetProfileMgr().CurrentProfile;
			if (currentProfile == null || DIContainerInfrastructure.GetBinarySerializer().SerializeToBytes(currentProfile) == null)
			{
				DebugLog.Error(GetType(), "OnAppResumed: PlayerProfile could not be serialized - " + DIContainerInfrastructure.GetProfileMgr().CurrentProfile);
				return false;
			}
			byte[] array = md5.ComputeHash(DIContainerInfrastructure.GetBinarySerializer().SerializeToBytes(currentProfile));
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 16).PadLeft(2, '0');
			}
			lastHash = text.PadLeft(32, '0');
			DIContainerInfrastructure.RemoteStorageService.SyncProfileAndResolveConflict(DIContainerInfrastructure.GetProfileMgr().CurrentProfile, HandleConflictAndResume, delegate
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_CronJob.DeRegisterCronJobBlockReason("app_resume");
			});
		}
		else
		{
			DebugLog.Log("[AppResumeService] Resume too short to check and synch Profile");
		}
		m_lastSynchAfterPausedTimestamp = DIContainerLogic.GetDeviceTimingService().GetPresentTime();
		ResumeSession();
		if (DIContainerLogic.GetTimingService() is TimingServiceSkynestStrictImpl)
		{
			(DIContainerLogic.GetTimingService() as TimingServiceSkynestStrictImpl).OnAppResumed();
		}
		if (DIContainerLogic.GetServerOnlyTimingService() is TimingServiceSkynestOnlyImpl)
		{
			(DIContainerLogic.GetServerOnlyTimingService() as TimingServiceSkynestOnlyImpl).OnAppResumed();
		}
		if (ContentLoader.Instance.m_BeaconConnectionMgr != null && ContentLoader.Instance.m_BeaconConnectionMgr.IsInitialized)
		{
			DebugLog.Log(GetType(), "Beacon Activate called");
		}
		return true;
	}

	private ABH.Shared.Models.PlayerData HandleConflictAndResume(ABH.Shared.Models.PlayerData currentPlayerData, ABH.Shared.Models.PlayerData remotePlayerData)
	{
		if (remotePlayerData != null)
		{
			byte[] array = md5.ComputeHash(DIContainerInfrastructure.GetBinarySerializer().SerializeToBytes(remotePlayerData));
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 16).PadLeft(2, '0');
			}
			text = text.PadLeft(32, '0');
			DebugLog.Log("Old Hash: " + lastHash + " new Hash: " + text);
			if (lastHash == text)
			{
				DebugLog.Error("[AppResumeService] Hash of Profiles is equal therfore ignore Synch!!");
				DIContainerInfrastructure.GetCoreStateMgr().m_CronJob.DeRegisterCronJobBlockReason("app_resume");
				DIContainerInfrastructure.RemoteStorageService.EnableProfileSync("SyncProfileAndGetConflictedProfile");
			}
			else
			{
				DIContainerInfrastructure.GetCoreStateMgr().ResetProfileAfterMessage(remotePlayerData);
			}
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_CronJob.DeRegisterCronJobBlockReason("app_resume");
			DIContainerInfrastructure.RemoteStorageService.EnableProfileSync("SyncProfileAndGetConflictedProfile");
		}
		return remotePlayerData;
	}

	private void ResumeSession()
	{
		CheckServerTime();
		CheckIfLaunchedFromUrl();
		if (m_lastSessionPauseTimestamp == DateTime.MinValue || DIContainerLogic.GetDeviceTimingService().TimeSince(m_lastSessionPauseTimestamp).TotalSeconds > 10.0)
		{
			DIContainerInfrastructure.GetAnalyticsSystem().StartSession();
			DIContainerInfrastructure.AdService.StartSession();
			DebugLog.Log("[AppResumeService] Starting new session " + DIContainerInfrastructure.GetProfileMgr().CurrentProfile.SkynestAnalyticsSessionId);
		}
		else
		{
			DebugLog.Log("[AppResumeService] Continuing current session " + DIContainerInfrastructure.GetProfileMgr().CurrentProfile.SkynestAnalyticsSessionId);
		}
		m_lastSessionPauseTimestamp = DIContainerLogic.GetDeviceTimingService().GetPresentTime();
	}

	private void CheckIfLaunchedFromUrl()
	{
		if (UnityEngine.Application.platform != RuntimePlatform.IPhonePlayer)
		{
			return;
		}
		string @string = DIContainerInfrastructure.GetPlayerPrefsService().GetString("launched_with_url", null);
		if (@string != null)
		{
			if (@string.Contains("startgame"))
			{
			}
			DIContainerInfrastructure.GetPlayerPrefsService().DeleteKey("launched_with_url");
		}
	}

	public void OnAppPaused()
	{
		m_lastSessionPauseTimestamp = DIContainerLogic.GetTimingService().GetPresentTime();
		m_lastSynchAfterPausedTimestamp = DIContainerLogic.GetTimingService().GetPresentTime();
		if (DIContainerLogic.GetTimingService() is TimingServiceSkynestStrictImpl)
		{
			(DIContainerLogic.GetTimingService() as TimingServiceSkynestStrictImpl).OnAppPaused();
		}
		if (DIContainerLogic.GetServerOnlyTimingService() is TimingServiceSkynestOnlyImpl)
		{
			(DIContainerLogic.GetServerOnlyTimingService() as TimingServiceSkynestOnlyImpl).OnAppPaused();
		}
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr().m_CronJob)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_CronJob.SyncProfile();
		}
	}

	private bool CheckServerTime()
	{
		return true;
	}
}
