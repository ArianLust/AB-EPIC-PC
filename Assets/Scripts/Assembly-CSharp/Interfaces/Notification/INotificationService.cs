namespace Interfaces.Notification
{
	public interface INotificationService
	{
		byte[] deviceToken { get; }

		RemoteNotificationTypePlatformIndependent enabledRemoteNotificationTypes { get; }

		int localNotificationCount { get; }

		LocalNotificationPlatformIndependent[] localNotifications { get; }

		string registrationError { get; }

		int remoteNotificationCount { get; }

		RemoteNotificationPlatformIndependent[] remoteNotifications { get; }

		LocalNotificationPlatformIndependent[] scheduledLocalNotifications { get; }

		void Init();

		void CheckForNotifications();

		string GetDeviceTokenAsString();

		void CancelAllLocalNotifications();

		void CancelLocalNotification(LocalNotificationPlatformIndependent notification);

		void ClearLocalNotifications();

		void ClearRemoteNotifications();

		LocalNotificationPlatformIndependent GetLocalNotification(int index);

		RemoteNotificationPlatformIndependent GetRemoteNotification(int index);

		void PresentLocalNotificationNow(LocalNotificationPlatformIndependent notification);

		void RegisterForRemoteNotificationTypes(RemoteNotificationTypePlatformIndependent notificationTypes);

		void ScheduleLocalNotification(LocalNotificationPlatformIndependent notification);

		void UnregisterForRemoteNotifications();

		string GetDefaultSoundName();
	}
}
