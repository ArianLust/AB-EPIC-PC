using System.Collections.Generic;
using Interfaces.Notification;

public class NotificationServiceNullImpl : INotificationService
{
	private List<LocalNotificationPlatformIndependent> m_scheduledNotifications = new List<LocalNotificationPlatformIndependent>();

	private List<LocalNotificationPlatformIndependent> m_localNotifications = new List<LocalNotificationPlatformIndependent>();

	private List<RemoteNotificationPlatformIndependent> m_remoteNotifications = new List<RemoteNotificationPlatformIndependent>();

	private string DebugTag = "[NotificationServiceNullImpl] ";

	public byte[] deviceToken
	{
		get
		{
			return null;
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
			return string.Empty;
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
	}

	public void CheckForNotifications()
	{
	}

	public void CancelAllLocalNotifications()
	{
		DebugLog.Log(DebugTag + "Cancel All Local Notifications");
		m_scheduledNotifications.Clear();
	}

	public void CancelLocalNotification(LocalNotificationPlatformIndependent notification)
	{
		DebugLog.Log(DebugTag + "Cancel Local Notification: " + notification.alertAction);
		m_scheduledNotifications.Remove(notification);
	}

	public void ClearLocalNotifications()
	{
		DebugLog.Log(DebugTag + "Clear Local Notifications");
		m_localNotifications.Clear();
		m_scheduledNotifications.Clear();
	}

	public void ClearRemoteNotifications()
	{
		DebugLog.Log(DebugTag + "Clear Remote Notifications");
		m_remoteNotifications.Clear();
	}

	public LocalNotificationPlatformIndependent GetLocalNotification(int index)
	{
		if (m_localNotifications.Count <= index)
		{
			return null;
		}
		return m_localNotifications[index];
	}

	public RemoteNotificationPlatformIndependent GetRemoteNotification(int index)
	{
		if (m_remoteNotifications.Count <= index)
		{
			return null;
		}
		return m_remoteNotifications[index];
	}

	public void PresentLocalNotificationNow(LocalNotificationPlatformIndependent notification)
	{
		DebugLog.Log(DebugTag + "Present Local Notification now! " + notification.alertAction);
	}

	public void RegisterForRemoteNotificationTypes(RemoteNotificationTypePlatformIndependent notificationTypes)
	{
		DebugLog.Log(DebugTag + "Register for Remote Notification Types: " + notificationTypes);
	}

	public void ScheduleLocalNotification(LocalNotificationPlatformIndependent notification)
	{
		DebugLog.Log(DebugTag + "Schedule Local Notification: " + notification.alertAction + "     " + notification.fireDate);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Type", notification.alertAction);
		dictionary.Add("Description", notification.alertBody);
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("NotificationSent", dictionary);
		if (!m_localNotifications.Contains(notification))
		{
			m_localNotifications.Add(notification);
		}
		m_scheduledNotifications.Add(notification);
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
		return null;
	}
}
