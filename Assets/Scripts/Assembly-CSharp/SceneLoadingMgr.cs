using System;
using System.Collections;
using System.Collections.Generic;
using ABH.Shared.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingMgr : MonoBehaviour
{
	private AsyncOperation m_LoadingOperation;

	public bool AsynchLoading;

	public bool m_UseUnloadBuffer;

	public float m_LastLoadingTimeInSec;

	private bool m_SkipFirstLoadingScreen = true;

	private Rect mRect;

	private bool LoadedFirstTime;

	private Dictionary<string, bool> m_LoadedLevels = new Dictionary<string, bool>();

	public LoadingScreenMgr LoadingScreen { get; set; }

	public bool ForceLoading { get; set; }

	private void Awake()
	{
		mRect = new Rect(0f, 0f, 1f, 1f);
	}

	public void LoadGameScene(string name, List<string> additionalUiScenes = null)
	{
		if (DIContainerInfrastructure.GetCoreStateMgr() != null && DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveNonInteractableTooltip();
		}
		StartCoroutine(LoadGameSceneCoroutine(name, additionalUiScenes));
	}

	private void OnGUI()
	{
		if (IsLoading() || !LoadedFirstTime)
		{
			GUI.backgroundColor = new Color(0f, 0f, 0f, 0.1f);
			GUI.Button(mRect, string.Empty);
			GUI.backgroundColor = Color.white;
		}
	}

	private LoadingArea GetLoadingScene(string sceneName)
	{
		switch (sceneName)
		{
		case "WorldMap_Generated":
			return LoadingArea.Worldmap;
		case "Camp":
			return LoadingArea.Camp;
		case "Arena":
		case "Battleground_Arena_01":
			return LoadingArea.Arena;
		case "ChronicleCave":
			return LoadingArea.ChronicleCave;
		default:
			if (sceneName.StartsWith("Battleground_"))
			{
				return LoadingArea.Battle;
			}
			return LoadingArea.Worldmap;
		}
	}

	private IEnumerator LoadGameSceneCoroutine(string name, List<string> additionalUiScenes = null)
	{
		DebugLog.Log(GetType(), "LoadGameSceneCoroutine " + name + " start");
		if (IsLoading())
		{
			DebugLog.Error("Already Loading new Game Scene!!");
			yield break;
		}
		LoadingArea sceneType = GetLoadingScene(name);
		DIContainerInfrastructure.BackButtonMgr.Reset();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("scene_loading");
		AndroidTools.DisableBackButton();
		yield return new WaitForSeconds(LoadingScreen.Show(sceneType));
		float loadingStarted = Time.realtimeSinceStartup;
		if (m_UseUnloadBuffer && AsynchLoading)
		{
			SceneManager.LoadScene("UnloadBuffer");
			yield return StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().UnloadUnusedAssetsCoroutine());
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
		yield return new WaitForEndOfFrame();
		AddLevel(name, false, AsynchLoading, null);
		DebugLog.Log(GetType(), "LoadGameSceneCoroutine " + name + " adding additional ui scenes");
		if (additionalUiScenes != null)
		{
			foreach (string scene in additionalUiScenes)
			{
				DebugLog.Log(GetType(), "LoadGameSceneCoroutine " + name + " adding additional ui scenes: " + scene);
				AddUILevel(scene);
			}
		}
		DebugLog.Log(GetType(), "LoadGameSceneCoroutine " + name + " adding additional ui scenes done");
		while (Application.isLoadingLevel)
		{
			yield return new WaitForEndOfFrame();
		}
		DebugLog.Log(GetType(), "LoadGameSceneCoroutine " + name + " waiting for !Application.isLoadingLevel done");
		while (ForceLoading)
		{
			yield return new WaitForEndOfFrame();
		}
		DebugLog.Log(GetType(), "LoadGameSceneCoroutine " + name + " waiting for ForceLoading done");
		m_LastLoadingTimeInSec = Time.realtimeSinceStartup - loadingStarted;
		DebugLog.Log("Loading Duration of Level " + name + " is: " + m_LastLoadingTimeInSec.ToString("0.##"));
		Dictionary<string, string> sceneLoadingTrack = new Dictionary<string, string>
		{
			{ "SceneName", name },
			{
				"TimeInSec",
				m_LastLoadingTimeInSec.ToString("F")
			}
		};
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("SceneLoading", sceneLoadingTrack);
		LoadingScreen.Hide();
		StartCoroutine(DisableBackButtonBlockerCoroutine());
		DebugLog.Log(GetType(), "LoadGameSceneCoroutine " + name + " end");
	}

	private IEnumerator DisableBackButtonBlockerCoroutine()
	{
		while (IsLoading())
		{
			yield return new WaitForEndOfFrame();
		}
		LoadedFirstTime = true;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("scene_loading");
		AndroidTools.EnableBackButton();
	}

	public void AddUILevel(string sceneName)
	{
		AddLevel(sceneName, true, AsynchLoading, null);
	}

	public void AddUILevel(string sceneName, Action callback)
	{
		AddLevel(sceneName, true, AsynchLoading, callback);
	}

	public void AddLevel(string sceneName, bool additive, bool asynch, Action callback)
	{
		DebugLog.Log(GetType(), "AddLevel: " + sceneName);
		if (asynch && AsynchLoading)
		{
			DebugLog.Log(GetType(), "async loading start, additive: " + additive);
			StartCoroutine(TakeActionAfterLevelLoaded(SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single), delegate
			{
				DebugLog.Log(GetType(), "AddLevel " + sceneName + " finished async");
				if (callback != null)
				{
					callback();
				}
			}, true));
			DebugLog.Log(GetType(), "async loading returned");
		}
		else
		{
			DebugLog.Log(GetType(), "sync loading start, additive: " + additive);
			SceneManager.LoadScene(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
			DebugLog.Log(GetType(), "sync loading end");
			StartCoroutine(WaitForCallback(callback));
		}
	}

	private IEnumerator WaitForCallback(Action callback)
	{
		yield return new WaitForSeconds(0f);
		if (callback != null)
		{
			callback();
		}
	}

	private IEnumerator TakeActionAfterLevelLoaded(AsyncOperation aop, Action callback, bool blocking)
	{
		while (!aop.isDone)
		{
			yield return new WaitForEndOfFrame();
		}
		if (callback != null)
		{
			callback();
		}
	}

	public bool IsLoading(bool includeStartup = false)
	{
		bool flag = LoadingScreen != null && LoadingScreen.gameObject.activeInHierarchy;
		if ((bool)ContentLoader.Instance && includeStartup)
		{
			return (ContentLoader.Instance.m_contentLoaderUI != null && ContentLoader.Instance.m_contentLoaderUI.activeSelf) || flag;
		}
		return flag;
	}

	public float CloseIris()
	{
		return LoadingScreen.CloseIris();
	}

	public float OpenIris()
	{
		return LoadingScreen.OpenIris();
	}

	public IEnumerator LoadInitialStartupScenesCoroutine()
	{
		m_LoadedLevels.Add("Toaster", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Toaster", delegate
		{
			m_LoadedLevels["Toaster"] = true;
		});
		while (m_LoadedLevels.Values.Count((bool e) => !e) > 0)
		{
			yield return new WaitForEndOfFrame();
		}
		SetDownloadProgressTextInContentLoader(DIContainerInfrastructure.GetStartupLocaService().Tr("startup_loading_levels", "Loading Levels..."));
		m_LoadedLevels.Add("LoadingScreen", false);
		AddUILevel("LoadingScreen", delegate
		{
			m_LoadedLevels["LoadingScreen"] = true;
		});
		m_LoadedLevels.Add("DisplayElements", false);
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("DisplayElements", delegate
		{
			m_LoadedLevels["DisplayElements"] = true;
		});
		m_LoadedLevels.Add("StorySequence", false);
		AddUILevel("StorySequence", delegate
		{
			m_LoadedLevels["StorySequence"] = true;
		});
		m_LoadedLevels.Add("InfoOverlays", false);
		AddUILevel("InfoOverlays", delegate
		{
			m_LoadedLevels["InfoOverlays"] = true;
		});
		m_LoadedLevels.Add("AlwaysOn_Root", false);
		AddUILevel("Popup_NetworkFailure", delegate
		{
			m_LoadedLevels["AlwaysOn_Root"] = true;
		});
		m_LoadedLevels.Add("Window_Root", false);
		AddUILevel("Window_Root", delegate
		{
			m_LoadedLevels["Window_Root"] = true;
		});
		m_LoadedLevels.Add("Popup_Root", false);
		AddUILevel("Popup_Root", delegate
		{
			m_LoadedLevels["Popup_Root"] = true;
		});
		m_LoadedLevels.Add("Popup_FeatureUnlocked", false);
		AddUILevel("Popup_FeatureUnlocked", delegate
		{
			m_LoadedLevels["Popup_FeatureUnlocked"] = true;
		});
		m_LoadedLevels.Add("Popup_Invitation", false);
		AddUILevel("Popup_Invitation", delegate
		{
			m_LoadedLevels["Popup_Invitation"] = true;
		});
		m_LoadedLevels.Add("Popup_SpecialOffer", false);
		AddUILevel("Popup_SpecialOffer", delegate
		{
			m_LoadedLevels["Popup_SpecialOffer"] = true;
		});
		m_LoadedLevels.Add("Popup_SpecialGachaOffer", false);
		AddUILevel("Popup_SpecialGachaOffer", delegate
		{
			m_LoadedLevels["Popup_SpecialGachaOffer"] = true;
		});
		m_LoadedLevels.Add("Popup_MissingResources", false);
		AddUILevel("Popup_MissingResources", delegate
		{
			m_LoadedLevels["Popup_MissingResources"] = true;
		});
		m_LoadedLevels.Add("Popup_RateApp", false);
		AddUILevel("Popup_RateApp", delegate
		{
			m_LoadedLevels["Popup_RateApp"] = true;
		});
		m_LoadedLevels.Add("Popup_LocalNotifications", false);
		AddUILevel("Popup_LocalNotifications", delegate
		{
			m_LoadedLevels["Popup_LocalNotifications"] = true;
		});
		m_LoadedLevels.Add("Popup_LevelUp", false);
		AddUILevel("Popup_LevelUp", delegate
		{
			m_LoadedLevels["Popup_LevelUp"] = true;
		});
		m_LoadedLevels.Add("Popup_MasteryUp", false);
		AddUILevel("Popup_MasteryUp", delegate
		{
			m_LoadedLevels["Popup_MasteryUp"] = true;
		});
		m_LoadedLevels.Add("Popup_ArenaLocked", false);
		AddUILevel("Popup_ArenaLocked", delegate
		{
			m_LoadedLevels["Popup_ArenaLocked"] = true;
		});
		m_LoadedLevels.Add("Popup_EventLocked", false);
		AddUILevel("Popup_EventLocked", delegate
		{
			m_LoadedLevels["Popup_EventLocked"] = true;
		});
		m_LoadedLevels.Add("Popup_UseVoucherCode", false);
		AddUILevel("Popup_UseVoucherCode", delegate
		{
			m_LoadedLevels["Popup_UseVoucherCode"] = true;
		});
		m_LoadedLevels.Add("Popup_EnergyLow", false);
		AddUILevel("Popup_EnergyLow", delegate
		{
			m_LoadedLevels["Popup_EnergyLow"] = true;
		});
		m_LoadedLevels.Add("Popup_EnergyMissing", false);
		AddUILevel("Popup_EnergyMissing", delegate
		{
			m_LoadedLevels["Popup_EnergyMissing"] = true;
		});
		m_LoadedLevels.Add("Popup_CurrencyMissing", false);
		AddUILevel("Popup_CurrencyMissing", delegate
		{
			m_LoadedLevels["Popup_CurrencyMissing"] = true;
		});
		m_LoadedLevels.Add("Popup_EnterName", false);
		AddUILevel("Popup_EnterName", delegate
		{
			m_LoadedLevels["Popup_EnterName"] = true;
		});
		m_LoadedLevels.Add("Window_WP8Achievements", false);
		AddUILevel("Window_WP8Achievements", delegate
		{
			m_LoadedLevels["Window_WP8Achievements"] = true;
		});
		m_LoadedLevels.Add("Popup_SeasonFinished", false);
		AddUILevel("Popup_SeasonFinished", delegate
		{
			m_LoadedLevels["Popup_SeasonFinished"] = true;
		});
		m_LoadedLevels.Add("Popup_DailyQuest", false);
		AddUILevel("Popup_DailyQuest", delegate
		{
			m_LoadedLevels["Popup_DailyQuest"] = true;
		});
		m_LoadedLevels.Add("Window_SetItemInfo", false);
		AddUILevel("Window_SetItemInfo", delegate
		{
			m_LoadedLevels["Window_SetItemInfo"] = true;
		});
		m_LoadedLevels.Add("Popup_ShopOfferInfo", false);
		AddUILevel("Popup_ShopOfferInfo", delegate
		{
			m_LoadedLevels["Popup_ShopOfferInfo"] = true;
		});
		m_LoadedLevels.Add("Popup_EliteChestUnlock", false);
		AddUILevel("Popup_EliteChestUnlock", delegate
		{
			m_LoadedLevels["Popup_EliteChestUnlock"] = true;
		});
		int notloadedCount = m_LoadedLevels.Values.Count((bool e) => !e);
		while (notloadedCount > 0)
		{
			yield return new WaitForEndOfFrame();
			notloadedCount = m_LoadedLevels.Values.Count((bool e) => !e);
			if (ContentLoader.Instance != null)
			{
				ContentLoader.Instance.SetDownloadProgress((float)(m_LoadedLevels.Count - notloadedCount) / (float)m_LoadedLevels.Count * 0.5f);
			}
		}
	}

	private void SetDownloadProgressTextInContentLoader(string txt)
	{
		if (ContentLoader.Instance != null)
		{
			ContentLoader.Instance.SetDownloadProgressText(txt);
		}
	}
}
