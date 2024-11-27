using System;
using System.Collections.Generic;
using UnityEngine;

public class GooglePlayServicesManager : MonoBehaviour
{
	public enum ActionAfterConnect
	{
		None = 0,
		SignIn = 1
	}

	public enum ActionAfterSignin
	{
		None = 0,
		ShowAchievementUI = 1,
		ReportUnlocked = 2,
		ReportProgress = 3
	}

	public const string ON_SIGNIN_SUCCEEDED_METHOD = "OnSigninSucceededFromJava";

	public const string ON_SIGNIN_FAILED_METHOD = "OnSigninFailedFromJava";

	private ActionAfterSignin m_actionAfterSignin;

	private ActionAfterConnect m_actionAfterConnect;

	private Dictionary<ActionAfterSignin, object[]> m_actionAfterSigninPayloadMap = new Dictionary<ActionAfterSignin, object[]>();

	public bool AutoSignInOnAnyAction { get; set; }

	public Action OnSignedIn { get; set; }

	public Action OnSigninFailed { get; set; }

	private AndroidJavaObject m_unityGoogleGamesClientBridge { get; set; }

	private void OnDestroy()
	{
		if (m_unityGoogleGamesClientBridge != null)
		{
			m_unityGoogleGamesClientBridge.Dispose();
		}
	}

	private void Start()
	{
		DebugLog.Log("[GooglePlayServicesManager] Start");
		Init();
	}

	public void Init()
	{
		DebugLog.Log("[GooglePlayServicesManager] Init");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (m_unityGoogleGamesClientBridge == null)
			{
				m_unityGoogleGamesClientBridge = new AndroidJavaObject("de.chimeraentertainment.unity.plugins.UnityGoogleGamesClientBridge");
			}
			m_unityGoogleGamesClientBridge.SetStatic("RECEIVER_GAMEOBJECT", base.gameObject.name);
			m_unityGoogleGamesClientBridge.SetStatic("ON_SIGNIN_FAILED_METHOD", "OnSigninFailedFromJava");
			m_unityGoogleGamesClientBridge.SetStatic("ON_SIGNIN_SUCCEEDED_METHOD", "OnSigninSucceededFromJava");
			m_unityGoogleGamesClientBridge.Call("InitOnUiThread", @static);
		}
	}

	private void OnSigninFailedFromJava(string @null)
	{
		DebugLog.Log("[GooglePlayServicesManager] OnSigninFailedFromJava");
		if (OnSigninFailed != null)
		{
			OnSigninFailed();
		}
	}

	private void OnSigninSucceededFromJava(string @null)
	{
		DebugLog.Log("[GooglePlayServicesManager] OnSigninSucceededFromJava");
		switch (m_actionAfterSignin)
		{
		case ActionAfterSignin.ShowAchievementUI:
			ShowAchievementUI();
			break;
		case ActionAfterSignin.ReportUnlocked:
		{
			object[] value2;
			if (m_actionAfterSigninPayloadMap.TryGetValue(m_actionAfterSignin, out value2))
			{
				ReportUnlocked(value2[0].ToString());
			}
			break;
		}
		case ActionAfterSignin.ReportProgress:
		{
			object[] value;
			if (m_actionAfterSigninPayloadMap.TryGetValue(m_actionAfterSignin, out value))
			{
				ReportProgress(value[0].ToString(), (double)value[1]);
			}
			break;
		}
		}
		m_actionAfterSignin = ActionAfterSignin.None;
		m_actionAfterSigninPayloadMap.Remove(m_actionAfterSignin);
		if (OnSignedIn != null)
		{
			OnSignedIn();
		}
	}

	public void SignIn()
	{
		DebugLog.Log("[GooglePlayServicesManager] SignIn");
		EnsureConnectedAndSignedIn();
	}

	public void SignOut()
	{
		DebugLog.Log("[GooglePlayServicesManager] SignOut");
		m_unityGoogleGamesClientBridge.Call("SignOut");
		Disconnect();
	}

	public bool IsSignedIn()
	{
		DebugLog.Log("[GooglePlayServicesManager] IsSignedIn");
		return m_unityGoogleGamesClientBridge.Call<bool>("IsSignedIn", new object[0]);
	}

	public bool IsConnected()
	{
		DebugLog.Log("[GooglePlayServicesManager] IsConnected");
		return m_unityGoogleGamesClientBridge.Call<bool>("IsConnected", new object[0]);
	}

	public void Disconnect()
	{
		DebugLog.Log("[GooglePlayServicesManager] Disconnect");
		m_unityGoogleGamesClientBridge.Call("Disconnect");
	}

	public void Reconnect()
	{
		DebugLog.Log("[GooglePlayServicesManager] Reconnect");
		m_unityGoogleGamesClientBridge.Call("Reconnect");
	}

	private void OnConnectionFailed()
	{
		DebugLog.Error("[GooglePlayServicesManager] OnConnectionFailed (sign in failed)");
	}

	public void ReportProgress(string achievementId, double progress)
	{
		DebugLog.Log("[GooglePlayServicesManager] ReportProgress " + achievementId + ", " + progress);
		if (AutoSignInOnAnyAction && !IsSignedIn())
		{
			DebugLog.Warn("[GooglePlayServicesManager] Not connected, cannot ReportProgress");
			SignIn();
			m_actionAfterSignin = ActionAfterSignin.ReportProgress;
			m_actionAfterSigninPayloadMap.Add(ActionAfterSignin.ReportProgress, new object[2] { achievementId, progress });
		}
		else if (IsSignedIn())
		{
			m_unityGoogleGamesClientBridge.Call("IncrementAchievement", achievementId, Convert.ToInt32(progress));
		}
	}

	public void ReportUnlocked(string achievementId)
	{
		DebugLog.Log("[GooglePlayServicesManager] ReportUnlocked " + achievementId);
		if (AutoSignInOnAnyAction && !IsSignedIn())
		{
			DebugLog.Warn("[GooglePlayServicesManager] Not connected, cannot ReportUnlocked");
			SignIn();
			m_actionAfterSignin = ActionAfterSignin.ReportUnlocked;
			m_actionAfterSigninPayloadMap.Add(ActionAfterSignin.ReportUnlocked, new object[1] { achievementId });
		}
		else if (IsSignedIn())
		{
			m_unityGoogleGamesClientBridge.Call("UnlockAchievement", achievementId);
		}
	}

	public void ShowAchievementUI()
	{
		DebugLog.Log("[GooglePlayServicesManager] ShowAchievementUI");
		if (!EnsureConnectedAndSignedIn())
		{
			m_actionAfterSignin = ActionAfterSignin.ShowAchievementUI;
		}
		else if (AutoSignInOnAnyAction && !IsSignedIn())
		{
			DebugLog.Warn("[GooglePlayServicesManager] Not signed in, cannot ShowAchievementUI");
			SignIn();
		}
		else if (IsSignedIn())
		{
			m_unityGoogleGamesClientBridge.Call("ShowAchievementUI");
		}
	}

	private bool EnsureConnectedAndSignedIn()
	{
		DebugLog.Log("[GooglePlayServicesManager] EnsureConnectedAndSignedIn");
		if (!IsConnected())
		{
			DebugLog.Warn("[GooglePlayServicesManager] EnsureConnectedAndSignedIn: Not connected for signin, now reconnecting...");
			Disconnect();
			Init();
			m_unityGoogleGamesClientBridge.Call("SignIn");
			return false;
		}
		if (!IsSignedIn())
		{
			DebugLog.Warn("[GooglePlayServicesManager] EnsureConnectedAndSignedIn: signed in, signing in now...");
			m_unityGoogleGamesClientBridge.Call("SignIn");
			return false;
		}
		return true;
	}
}
