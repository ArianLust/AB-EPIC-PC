using UnityEngine;

public static class AndroidTools
{
	private const string m_standardActivityName = "de.chimeraentertainment.unity.UnityPlayerActivity";

	private static string m_androidCodenameCache;

	private static int m_androidAPILevelCache;

	public static void DisableBackButton(string activity = "de.chimeraentertainment.unity.UnityPlayerActivity")
	{
		_DisableBackButton(activity, true);
	}

	public static void EnableBackButton(string activity = "de.chimeraentertainment.unity.UnityPlayerActivity")
	{
		_DisableBackButton(activity, false);
	}

	private static void _DisableBackButton(string activity, bool disabled)
	{
		Debug.Log("[AndroidTools] _DisableBackButton " + disabled);
	}

	public static AndroidJavaObject GetCurrentActivity(string activity = "de.chimeraentertainment.unity.UnityPlayerActivity")
	{
		Debug.Log("[AndroidTools] GetCurrentActivity");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(activity))
		{
			return androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		}
	}

	public static void ShowNavigationBar()
	{
		Debug.Log("[AndroidTools] ShowNavigationBar");
		using (AndroidJavaObject androidJavaObject = GetCurrentActivity())
		{
			androidJavaObject.Call("showNavigationBar");
		}
	}

	public static void EnableScreenAwake()
	{
		Debug.Log("[AndroidTools] EnableScreenAwake");
		using (AndroidJavaObject androidJavaObject = GetCurrentActivity())
		{
			androidJavaObject.Call("enableScreenAwake");
		}
	}

	public static void DisableScreenAwake()
	{
		Debug.Log("[AndroidTools] DisableScreenAwake");
		using (AndroidJavaObject androidJavaObject = GetCurrentActivity())
		{
			androidJavaObject.Call("disableScreenAwake");
		}
	}

	public static void EnableImmersiveMode()
	{
		Debug.Log("[AndroidTools] EnableImmersiveMode");
		using (AndroidJavaObject androidJavaObject = GetCurrentActivity())
		{
			androidJavaObject.Call("enableImmersiveMode");
		}
	}
}
