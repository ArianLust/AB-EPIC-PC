using System;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using Interfaces.Notification;
using UnityEngine;

public class NotificationMgr : MonoBehaviour
{
	public const int NotificationUsageStateNotAsked = 0;

	public const int NotificationUsageStateDeclined = 1;

	public const int NotificationUsageStateAccepted = 2;

	private bool m_HasReportedDeviceTokenToOmniata;

	private void OnApplicationPause(bool paused)
	{
		ManageNotifications(paused);
	}

	public void ManageNotifications(bool paused)
	{
		if (DIContainerInfrastructure.GetCurrentPlayer() != null && DIContainerInfrastructure.GetCurrentPlayer().Data != null)
		{
			DebugLog.Log("[NotificationMgr] OnApplicationPause(paused = " + paused + ")  Notification usage state: " + DIContainerInfrastructure.GetCurrentPlayer().Data.NotificationUsageState);
		}
		if (DIContainerInfrastructure.GetCoreStateMgr() != null && DIContainerInfrastructure.GetCoreStateMgr().m_isInitialized)
		{
			DIContainerInfrastructure.NotificationService().Init();
			if (!paused)
			{
				DIContainerInfrastructure.NotificationService().CheckForNotifications();
				DIContainerInfrastructure.NotificationService().CancelAllLocalNotifications();
				DIContainerInfrastructure.NotificationService().ClearLocalNotifications();
				ReportDeviceTokenToOmniataIfNeeded();
			}
			else if (DIContainerInfrastructure.GetCurrentPlayer().Data.NotificationUsageState == 2)
			{
				FireEventNotifications();
				FireArenaLeagueNotifications();
				FireEventEnergyFullNotification();
			}
		}
	}

	private void ReportDeviceTokenToOmniataIfNeeded()
	{
		if (m_HasReportedDeviceTokenToOmniata)
		{
			return;
		}
		string text = "na";
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			text = "apns";
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			text = "gcm";
		}
		string deviceTokenAsString = DIContainerInfrastructure.NotificationService().GetDeviceTokenAsString();
		if (DIContainerInfrastructure.NotificationService().deviceToken != null && !string.IsNullOrEmpty(deviceTokenAsString))
		{
			DebugLog.Log("[NotificationMgr] Send decivetoken to omniata, token: " + deviceTokenAsString);
			DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameter("om_" + text + "_enable", "om_device_token", deviceTokenAsString);
			m_HasReportedDeviceTokenToOmniata = true;
			return;
		}
		string parameterValue = "disabled";
		if (!string.IsNullOrEmpty(deviceTokenAsString))
		{
			parameterValue = deviceTokenAsString;
		}
		DebugLog.Log("[NotificationMgr] Could not send device token to omniata. token is empty. Report this as disabled.");
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameter("om_" + text + "_disable", "om_device_token", parameterValue);
		m_HasReportedDeviceTokenToOmniata = true;
	}

	private void FireEventNotifications()
	{
		if (!DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			return;
		}
		EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("{value_1}", DIContainerInfrastructure.GetLocaService().Tr(currentEventManagerGameData.EventBalancing.LocaBaseId + "_name"));
		if (DIContainerLogic.EventSystemService.IsEventTeasing(currentEventManagerGameData.Balancing))
		{
			DateTime teasingEndTime = DIContainerLogic.EventSystemService.GetTeasingEndTime(currentEventManagerGameData.Balancing);
			LocalNotificationPlatformIndependent localNotificationPlatformIndependent = null;
			LocalNotificationPlatformIndependent localNotificationPlatformIndependent2 = new LocalNotificationPlatformIndependent();
			localNotificationPlatformIndependent2.alertAction = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventstart_name", dictionary));
			localNotificationPlatformIndependent2.userInfo = new Dictionary<string, string>();
			localNotificationPlatformIndependent2.fireDate = DateTime.Now.AddSeconds(DIContainerLogic.GetTimingService().TimeLeftUntil(teasingEndTime).TotalSeconds);
			localNotificationPlatformIndependent2.alertBody = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventstart_desc", dictionary));
			localNotificationPlatformIndependent2.alertSubheader = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventstart_short"));
			localNotificationPlatformIndependent2.hasAction = false;
			localNotificationPlatformIndependent2.applicationIconBadgeNumber = -1;
			localNotificationPlatformIndependent2.soundName = DIContainerInfrastructure.NotificationService().GetDefaultSoundName();
			localNotificationPlatformIndependent = localNotificationPlatformIndependent2;
			DIContainerInfrastructure.NotificationService().ScheduleLocalNotification(localNotificationPlatformIndependent);
		}
		else if (!DIContainerLogic.EventSystemService.IsWaitingForConfirmation(currentEventManagerGameData))
		{
			DateTime dateTime = DIContainerLogic.GetTimingService().GetPresentTime() + DIContainerLogic.EventSystemService.GetTimeTillEventEnd(currentEventManagerGameData.Balancing);
			LocalNotificationPlatformIndependent localNotificationPlatformIndependent3 = null;
			LocalNotificationPlatformIndependent localNotificationPlatformIndependent2 = new LocalNotificationPlatformIndependent();
			localNotificationPlatformIndependent2.alertAction = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventends_name", dictionary));
			localNotificationPlatformIndependent2.userInfo = new Dictionary<string, string>();
			localNotificationPlatformIndependent2.fireDate = DateTime.Now.AddSeconds(DIContainerLogic.EventSystemService.GetTimeTillEventEnd(currentEventManagerGameData.Balancing).TotalSeconds);
			localNotificationPlatformIndependent2.alertBody = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventends_desc", dictionary));
			localNotificationPlatformIndependent2.alertSubheader = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventends_short", dictionary));
			localNotificationPlatformIndependent2.hasAction = false;
			localNotificationPlatformIndependent2.applicationIconBadgeNumber = -1;
			localNotificationPlatformIndependent2.soundName = DIContainerInfrastructure.NotificationService().GetDefaultSoundName();
			localNotificationPlatformIndependent3 = localNotificationPlatformIndependent2;
			DIContainerInfrastructure.NotificationService().ScheduleLocalNotification(localNotificationPlatformIndependent3);
			DateTime dateTime2 = dateTime - new TimeSpan(2, 0, 0);
			if (dateTime2 > DIContainerLogic.GetTimingService().GetPresentTime())
			{
				LocalNotificationPlatformIndependent localNotificationPlatformIndependent4 = null;
				localNotificationPlatformIndependent2 = new LocalNotificationPlatformIndependent();
				localNotificationPlatformIndependent2.alertAction = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventendssoon_name", dictionary));
				localNotificationPlatformIndependent2.userInfo = new Dictionary<string, string>();
				localNotificationPlatformIndependent2.fireDate = DateTime.Now.AddSeconds(Mathf.Max((float)DIContainerLogic.GetTimingService().TimeLeftUntil(dateTime2).TotalSeconds, 300f));
				localNotificationPlatformIndependent2.alertBody = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventendssoon_desc", dictionary));
				localNotificationPlatformIndependent2.alertSubheader = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventendssoon_short", dictionary));
				localNotificationPlatformIndependent2.hasAction = false;
				localNotificationPlatformIndependent2.applicationIconBadgeNumber = -1;
				localNotificationPlatformIndependent2.soundName = DIContainerInfrastructure.NotificationService().GetDefaultSoundName();
				localNotificationPlatformIndependent4 = localNotificationPlatformIndependent2;
				DIContainerInfrastructure.NotificationService().ScheduleLocalNotification(localNotificationPlatformIndependent4);
			}
		}
	}

	private void FireEventEnergyFullNotification()
	{
		if (!DIContainerLogic.EventSystemService.IsCurrentEventAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			return;
		}
		EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("{value_1}", DIContainerInfrastructure.GetLocaService().Tr(currentEventManagerGameData.EventBalancing.LocaBaseId + "_name"));
		if (!DIContainerLogic.EventSystemService.IsEventTeasing(currentEventManagerGameData.Balancing) && !DIContainerLogic.EventSystemService.IsWaitingForConfirmation(currentEventManagerGameData))
		{
			int num = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ItemMaxCaps["event_energy"];
			float energyRefreshTimeInSeconds = DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData.EnergyRefreshTimeInSeconds;
			int itemValue = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "event_energy");
			float num2 = (float)(num - itemValue) * energyRefreshTimeInSeconds;
			if (!(num2 <= 0f))
			{
				LocalNotificationPlatformIndependent localNotificationPlatformIndependent = null;
				LocalNotificationPlatformIndependent localNotificationPlatformIndependent2 = new LocalNotificationPlatformIndependent();
				localNotificationPlatformIndependent2.alertAction = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventenergy_name", dictionary));
				localNotificationPlatformIndependent2.userInfo = new Dictionary<string, string>();
				localNotificationPlatformIndependent2.fireDate = DateTime.Now.AddSeconds(num2);
				localNotificationPlatformIndependent2.alertBody = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventenergy_desc", dictionary));
				localNotificationPlatformIndependent2.alertSubheader = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_eventenergy_short", dictionary));
				localNotificationPlatformIndependent2.hasAction = false;
				localNotificationPlatformIndependent2.applicationIconBadgeNumber = -1;
				localNotificationPlatformIndependent2.soundName = DIContainerInfrastructure.NotificationService().GetDefaultSoundName();
				localNotificationPlatformIndependent = localNotificationPlatformIndependent2;
				DIContainerInfrastructure.NotificationService().ScheduleLocalNotification(localNotificationPlatformIndependent);
			}
		}
	}

	private void FireArenaLeagueNotifications()
	{
		if (!DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			return;
		}
		Dictionary<string, string> replacementStrings = new Dictionary<string, string>();
		PvPSeasonManagerGameData currentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		if (DIContainerLogic.PvPSeasonService.IsPvPTurnRunning(currentPvPSeasonGameData))
		{
			DateTime pvpTurnEndTime = DIContainerLogic.PvPSeasonService.GetPvpTurnEndTime(currentPvPSeasonGameData);
			DateTime dateTime = pvpTurnEndTime - new TimeSpan(2, 0, 0);
			LocalNotificationPlatformIndependent localNotificationPlatformIndependent2;
			if (dateTime > DIContainerLogic.GetTimingService().GetPresentTime())
			{
				LocalNotificationPlatformIndependent localNotificationPlatformIndependent = null;
				localNotificationPlatformIndependent2 = new LocalNotificationPlatformIndependent();
				localNotificationPlatformIndependent2.alertAction = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_leagueoversoon_name", replacementStrings));
				localNotificationPlatformIndependent2.userInfo = new Dictionary<string, string>();
				localNotificationPlatformIndependent2.fireDate = DateTime.Now.AddSeconds(Mathf.Max((float)DIContainerLogic.GetTimingService().TimeLeftUntil(dateTime).TotalSeconds, 300f));
				localNotificationPlatformIndependent2.alertBody = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_leagueoversoon_desc", replacementStrings));
				localNotificationPlatformIndependent2.alertSubheader = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_leagueoversoon_short"));
				localNotificationPlatformIndependent2.hasAction = false;
				localNotificationPlatformIndependent2.applicationIconBadgeNumber = -1;
				localNotificationPlatformIndependent2.soundName = DIContainerInfrastructure.NotificationService().GetDefaultSoundName();
				localNotificationPlatformIndependent = localNotificationPlatformIndependent2;
				DIContainerInfrastructure.NotificationService().ScheduleLocalNotification(localNotificationPlatformIndependent);
			}
			LocalNotificationPlatformIndependent localNotificationPlatformIndependent3 = null;
			localNotificationPlatformIndependent2 = new LocalNotificationPlatformIndependent();
			localNotificationPlatformIndependent2.alertAction = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_leagueovernow_name", replacementStrings));
			localNotificationPlatformIndependent2.userInfo = new Dictionary<string, string>();
			localNotificationPlatformIndependent2.fireDate = DateTime.Now.AddSeconds(DIContainerLogic.GetTimingService().TimeLeftUntil(pvpTurnEndTime).TotalSeconds);
			localNotificationPlatformIndependent2.alertBody = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_leagueovernow_desc", replacementStrings));
			localNotificationPlatformIndependent2.alertSubheader = NGUITools.StripSymbols(DIContainerInfrastructure.GetLocaService().Tr("notify_leagueovernow_short"));
			localNotificationPlatformIndependent2.hasAction = false;
			localNotificationPlatformIndependent2.applicationIconBadgeNumber = -1;
			localNotificationPlatformIndependent2.soundName = DIContainerInfrastructure.NotificationService().GetDefaultSoundName();
			localNotificationPlatformIndependent3 = localNotificationPlatformIndependent2;
			DIContainerInfrastructure.NotificationService().ScheduleLocalNotification(localNotificationPlatformIndependent3);
		}
	}
}
