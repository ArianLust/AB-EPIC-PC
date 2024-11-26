using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prime31
{
	public class GoogleCloudMessaging
	{
		private static AndroidJavaObject _plugin;

		static GoogleCloudMessaging()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.GoogleCloudMessagingPlugin"))
			{
				_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
		}

		public static void checkForNotifications()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("checkForNotifications");
			}
		}

		public static void register(string gcmSenderId)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("register", gcmSenderId);
			}
		}

		public static void unRegister()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("unRegister");
			}
		}

		public static void cancelAll()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("cancelAll");
			}
		}

		public static IEnumerator registerDeviceWithPushIO(string deviceId, string pushIOApiKey, List<string> pushIOCategories, Action<bool, string> completionHandler)
		{
			Debug.LogWarning("Push.io support is disabled by default. To enable it uncomment the first line in the GoogleCloudMessaging.cs file");
			yield return null;
		}

		public static void setPushNotificationAlternateKey(string originalKey, string alternateKey)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("setPushNotificationAlternateKey", originalKey, alternateKey);
			}
		}

		public static void setPushNotificationDefaultValueForKey(string key, string value)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("setPushNotificationDefaultValueForKey", key, value);
			}
		}
	}
}
