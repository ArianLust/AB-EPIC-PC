using System.Collections.Generic;
using UnityEngine;

namespace Prime31
{
	public class FlurryAnalytics
	{
		private static AndroidJavaClass _flurryAgent;

		private static AndroidJavaObject _plugin;

		static FlurryAnalytics()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			_flurryAgent = new AndroidJavaClass("com.flurry.android.FlurryAgent");
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.analytics.FlurryAnalytics"))
			{
				_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
		}

		public static void startSession(string apiKey, bool enableLogging = false)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("onStartSession", apiKey, enableLogging);
			}
		}

		public static void onEndSession()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("onEndSession");
			}
		}

		public static void addUserCookie(string key, string value)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("addUserCookie", key, value);
			}
		}

		public static void clearUserCookies()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("clearUserCookies");
			}
		}

		public static void setContinueSessionMillis(long milliseconds)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_flurryAgent.CallStatic("setContinueSessionMillis", milliseconds);
			}
		}

		public static void logEvent(string eventName)
		{
			logEvent(eventName, false);
		}

		public static void logEvent(string eventName, bool isTimed)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				if (isTimed)
				{
					_plugin.Call("logTimedEvent", eventName);
				}
				else
				{
					_plugin.Call("logEvent", eventName);
				}
			}
		}

		public static void logEvent(string eventName, Dictionary<string, string> parameters)
		{
			logEvent(eventName, parameters, false);
		}

		public static void logEvent(string eventName, Dictionary<string, string> parameters, bool isTimed)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				if (parameters == null)
				{
					Debug.LogError("attempting to call logEvent with null parameters");
				}
				else if (isTimed)
				{
					_plugin.Call("logTimedEventWithParams", eventName, parameters.toJson());
				}
				else
				{
					_plugin.Call("logEventWithParams", eventName, parameters.toJson());
				}
			}
		}

		public static void endTimedEvent(string eventName)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("endTimedEvent", eventName);
			}
		}

		public static void endTimedEvent(string eventName, Dictionary<string, string> parameters)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				if (parameters == null)
				{
					Debug.LogError("attempting to call endTimedEvent with null parameters");
					return;
				}
				_plugin.Call("endTimedEventWithParams", eventName, parameters.toJson());
			}
		}

		public static void onPageView()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_flurryAgent.CallStatic("onPageView");
			}
		}

		public static void onError(string errorId, string message, string errorClass)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_flurryAgent.CallStatic("onError", errorId, message, errorClass);
			}
		}

		public static void setUserID(string userId)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_flurryAgent.CallStatic("setUserId", userId);
			}
		}

		public static void setAge(int age)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_flurryAgent.CallStatic("setAge", age);
			}
		}

		public static void setGender(FlurryGender gender)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				AndroidJavaObject @static = new AndroidJavaClass("com.flurry.android.Contants").GetStatic<AndroidJavaObject>(gender.ToString().ToUpper());
				_flurryAgent.CallStatic("setGender", @static);
			}
		}

		public static void setLogEnabled(bool enable)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_flurryAgent.CallStatic("setLogEnabled", enable);
			}
		}
	}
}
