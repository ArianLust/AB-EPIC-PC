using System.Collections.Generic;
using ABH.Shared.Models;
using UnityEngine;

public class CronJob : MonoBehaviour
{
	private const float CronJobRepeateTimerSeconds = 90f;

	private static IProfileMerger m_profileMerger;

	private List<string> m_BlockReasons = new List<string>();

	public void RegisterCronJobBlock(string blockReason)
	{
		if (!m_BlockReasons.Contains(blockReason))
		{
			m_BlockReasons.Add(blockReason);
		}
	}

	public void DeRegisterCronJobBlockReason(string blockReason)
	{
		m_BlockReasons.Remove(blockReason);
	}

	public void ResetBlockReason()
	{
		m_BlockReasons.Clear();
	}

	private void Start()
	{
		m_profileMerger = DIContainerLogic.ProfileMerger;
		CancelInvoke("Run");
		InvokeRepeating("Run", 5f, 90f);
	}

	public void Run()
	{
		DebugLog.Log("[CronJob] <b>Executing CronJob...</b>");
		if (m_BlockReasons.Count > 0)
		{
			DebugLog.Log("[CronJob] Cron Job is currently blocked by " + string.Join(", ", m_BlockReasons.ToArray()));
			return;
		}
		DIContainerInfrastructure.MessagingService.GetMessages(10u);
		DIContainerLogic.SocialService.ReAddResendMessages(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData);
		DIContainerInfrastructure.GetPurchaseProcessor().CheckForVoucherChanges();
		SyncProfile();
	}

	private PlayerData OnProfileSyncError(PlayerData current, PlayerData remotePlayerData)
	{
		if (remotePlayerData != null)
		{
			DebugLog.Log(GetType(), "OnProfileSyncError: Remote profile is different than ours. We now decide what to do with it.");
			PlayerData mergedProfile;
			if (m_profileMerger.TryMergeProfile(DIContainerInfrastructure.GetProfileMgr().CurrentProfile, remotePlayerData, out mergedProfile))
			{
				DebugLog.Log(GetType(), "OnProfileSyncError: Remote profile is was merged, we will now reset the game with the new profile");
				DIContainerInfrastructure.GetCoreStateMgr().ResetProfileAfterMessage(remotePlayerData);
			}
			return mergedProfile;
		}
		DebugLog.Log(GetType(), "OnProfileSyncError: Remote profile is null");
		return current;
	}

	public void SyncProfile()
	{
		DebugLog.Log(GetType(), "using the ProfileMerger for SyncProfileAndResolveConflict");
		DIContainerInfrastructure.RemoteStorageService.SyncProfileAndResolveConflict(DIContainerInfrastructure.GetProfileMgr().CurrentProfile, OnProfileSyncError, delegate
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_CronJob.DeRegisterCronJobBlockReason("app_resume");
		});
	}

	public void RunPushOnly()
	{
		DebugLog.Log("[CronJob] <b>Executing PushOnly...</b>");
		if (m_BlockReasons.Count > 0)
		{
			DebugLog.Log("[CronJob] PushOnly is currently blocked!");
		}
		else
		{
			DIContainerInfrastructure.RemoteStorageService.PushProfile(DIContainerInfrastructure.GetProfileMgr().CurrentProfile);
		}
	}
}
