using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces.Notification;
using Prime31;
using Rcs;

public class NotificationServiceAndroidImpl : INotificationService
{
	private List<LocalNotificationPlatformIndependent> m_scheduledNotifications = new List<LocalNotificationPlatformIndependent>();

	private List<LocalNotificationPlatformIndependent> m_localNotifications = new List<LocalNotificationPlatformIndependent>();

	private List<RemoteNotificationPlatformIndependent> m_remoteNotifications = new List<RemoteNotificationPlatformIndependent>();

	private PushNotifications m_Notifications;

	private string DebugTag = "[NotificationServiceAndroidImpl] ";

	private bool m_initialized;

	private string m_deviceId;

	private string m_registrationError = string.Empty;

	public byte[] deviceToken
	{
		get
		{
			if (!string.IsNullOrEmpty(m_deviceId))
			{
				return Encoding.UTF8.GetBytes(m_deviceId);
			}
			return new byte[0];
		}
	}

	public RemoteNotificationTypePlatformIndependent enabledRemoteNotificationTypes
	{
		get
		{
			return RemoteNotificationTypePlatformIndependent.None;
		}
	}

	public int localNotificationCount
	{
		get
		{
			return m_localNotifications.Count;
		}
	}

	public LocalNotificationPlatformIndependent[] localNotifications
	{
		get
		{
			return m_localNotifications.ToArray();
		}
	}

	public string registrationError
	{
		get
		{
			return m_registrationError;
		}
	}

	public int remoteNotificationCount
	{
		get
		{
			return m_remoteNotifications.Count;
		}
	}

	public RemoteNotificationPlatformIndependent[] remoteNotifications
	{
		get
		{
			return m_remoteNotifications.ToArray();
		}
	}

	public LocalNotificationPlatformIndependent[] scheduledLocalNotifications
	{
		get
		{
			return m_scheduledNotifications.ToArray();
		}
	}

	public void Init()
	{
		if (!m_initialized)
		{
			m_initialized = true;
			EtceteraAndroid.checkForNotifications();
			string googleAppId = DIContainerConfig.GetClientConfig().GoogleAppId;
			DebugLog.Log(DebugTag + "Init: GoogleCloudMessaging.register(" + googleAppId + ")");
			GoogleCloudMessaging.register(googleAppId);
			RegisterGsmEvents();
		}
	}

	private void RegisterGsmEvents()
	{
		UnregisterGcmEvents();
		GoogleCloudMessagingManager.registrationSucceededEvent += GoogleCloudMessagingManager_registrationSucceededEvent;
		GoogleCloudMessagingManager.registrationFailedEvent += GoogleCloudMessagingManager_registrationFailedEvent;
		EtceteraAndroidManager.notificationReceivedEvent += NotificationReceived;
	}

	private void GoogleCloudMessagingManager_registrationFailedEvent(string error)
	{
		DebugLog.Error(DebugTag + "GoogleCloudMessagingManager_registrationFailedEvent with error = " + error);
		m_registrationError = error;
		UnregisterGcmEvents();
	}

	private void GoogleCloudMessagingManager_registrationSucceededEvent(string registrationId)
	{
		m_deviceId = registrationId;
		DebugLog.Log(DebugTag + "GoogleCloudMessagingManager_registrationSucceededEvent with registrationId = " + registrationId);
		UnregisterGcmEvents();
		m_Notifications = new PushNotifications(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy, registrationId);
		m_Notifications.RegisterDevice(SkynestNotificationService_RegisterSuccess, SkynestNotificationService_RegisterError);
	}

	private void SkynestNotificationService_RegisterError(int status, string errorMsg)
	{
		DebugLog.Error(DebugTag + "SkynestNotificationService_RegisterError with status " + status + " and errorMsg = " + errorMsg);
	}

	private void SkynestNotificationService_RegisterSuccess()
	{
		DebugLog.Log(DebugTag + "SkynestNotificationService_RegisterSuccess!");
	}

	private void UnregisterGcmEvents()
	{
		GoogleCloudMessagingManager.registrationSucceededEvent -= GoogleCloudMessagingManager_registrationSucceededEvent;
		GoogleCloudMessagingManager.registrationFailedEvent -= GoogleCloudMessagingManager_registrationFailedEvent;
	}

	public void CheckForNotifications()
	{
		EtceteraAndroid.checkForNotifications();
	}

	public void CancelAllLocalNotifications()
	{
		DebugLog.Log(DebugTag + "Cancel All Local Notifications");
		foreach (LocalNotificationPlatformIndependent scheduledNotification in m_scheduledNotifications)
		{
			EtceteraAndroid.cancelNotification(scheduledNotification.Id);
		}
		m_scheduledNotifications.Clear();
	}

	public void CancelLocalNotification(LocalNotificationPlatformIndependent notification)
	{
		DebugLog.Log(DebugTag + "Cancel Local Notification: " + notification.alertAction);
		EtceteraAndroid.cancelNotification(notification.Id);
		m_scheduledNotifications.Remove(notification);
	}

	public void ClearLocalNotifications()
	{
		DebugLog.Log(DebugTag + "Clear Local Notifications");
		foreach (LocalNotificationPlatformIndependent localNotification in m_localNotifications)
		{
			EtceteraAndroid.cancelNotification(localNotification.Id);
		}
		m_localNotifications.Clear();
		m_scheduledNotifications.Clear();
	}

	public void ClearRemoteNotifications()
	{
		DebugLog.Log(DebugTag + "Clear Remote Notifications");
		foreach (RemoteNotificationPlatformIndependent remoteNotification in m_remoteNotifications)
		{
			EtceteraAndroid.cancelNotification(remoteNotification.Id);
		}
		m_remoteNotifications.Clear();
	}

	public LocalNotificationPlatformIndependent GetLocalNotification(int index)
	{
		return m_localNotifications.FirstOrDefault((LocalNotificationPlatformIndependent n) => n.Id == index);
	}

	public RemoteNotificationPlatformIndependent GetRemoteNotification(int index)
	{
		return m_remoteNotifications.FirstOrDefault((RemoteNotificationPlatformIndependent n) => n.Id == index);
	}

	public void PresentLocalNotificationNow(LocalNotificationPlatformIndependent notification)
	{
		DebugLog.Log(DebugTag + "Present Local Notification now! " + notification.alertAction);
		EtceteraAndroid.scheduleNotification(new AndroidNotificationConfiguration(0L, notification.alertAction, notification.alertBody, notification.alertAction));
	}

	public void RegisterForRemoteNotificationTypes(RemoteNotificationTypePlatformIndependent notificationTypes)
	{
		DebugLog.Log(DebugTag + "Register for Remote Notification Types: " + notificationTypes);
		EtceteraAndroid.checkForNotifications();
	}

	public void ScheduleLocalNotification(LocalNotificationPlatformIndependent notification)
	{
		DebugLog.Log(DebugTag + "Schedule Local Notification: " + notification.alertAction);
		notification.Id = EtceteraAndroid.scheduleNotification(new AndroidNotificationConfiguration((long)(notification.fireDate - DateTime.Now).TotalSeconds, notification.alertAction, notification.alertSubheader, notification.alertBody));
		if (notification.Id != -1)
		{
			if (!m_localNotifications.Contains(notification))
			{
				m_localNotifications.Add(notification);
			}
			m_scheduledNotifications.Add(notification);
		}
	}

	public void UnregisterForRemoteNotifications()
	{
	}

	public string GetDefaultSoundName()
	{
		return string.Empty;
	}

	public string GetDeviceTokenAsString()
	{
		return m_deviceId;
	}

	private void NotificationReceived(string someThing)
	{
		DebugLog.Warn(GetType(), "NotificationReceived: " + someThing);
	}
}
