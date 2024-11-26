using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ABH.Shared.Models;
using Chimera.Library.Components.ClientLib.CrossPlatformLib.Source.Models;
using Prime31;
using UnityEngine;

public class ClientUpdateService
{
	public Action<Action, Action> Wp8AdditionalNewVersionCheck;

	public Action Wp8OpenStoreAction;

	public string StoreVersionsFileName
	{
		get
		{
			return DIContainerInfrastructure.GetTargetBuildGroup() + "_StoreVersionsFile.txt";
		}
	}

	public void DisplayNewVersionAvailableDialog()
	{
		string message = DIContainerInfrastructure.GetLocaService().Tr("conf_popup_new_clientversion_available", "There is a new version is available!");
		DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(message, delegate
		{
			DIContainerInfrastructure.GetClientUpdateService().DisplayOpenAppropriateAppStoreForNewVersionDialog();
		}, null);
	}

	public void OpenAppropriateReviewPage()
	{
		EtceteraAndroid.openReviewPageInPlayStore();
	}

	public void DisplayOpenAppropriateAppStoreForNewVersionDialog()
	{
		DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("conf_popup_opening_appstore", "Now opening app store..."), delegate
		{
		}, null);
	}

	public void OpenAppropriateAppStoreForNewVersion()
	{
		DebugLog.Log("Opening Google Play Store...");
		Application.OpenURL(string.Format("http://play.google.com/store/apps/details?id={0}", DIContainerConfig.GetClientConfig().BundleId));
	}

	public void Hatch2_CheckForNewVersionAvailable(Action<bool> downloadedCallbackAction)
	{
		DIContainerInfrastructure.GetAssetsService().Load(StoreVersionsFileName, delegate(string assetPath)
		{
			OnStoreVersionDownloadedCallback(assetPath, downloadedCallbackAction);
		}, delegate
		{
		});
	}

	private void OnStoreVersionDownloadedCallback(string assetPath, Action<bool> downloadedCallbackAction)
	{
		DebugLog.Log(GetType(), "OnStoreVersionDownloadedCallback: retrieved the " + StoreVersionsFileName + ", result is " + assetPath);
		if (ParseFileAndCheckVersion(assetPath))
		{
			downloadedCallbackAction(true);
		}
		else
		{
			PerformAdditionalVersionCheck(downloadedCallbackAction);
		}
	}

	private bool ParseFileAndCheckVersion(string assetPath)
	{
		DebugLog.Log(GetType(), "ParseFileAndContinue");
		if (string.IsNullOrEmpty(assetPath) || !File.Exists(assetPath))
		{
			DebugLog.Error(GetType(), "ParseFileAndContinue: Path doesn't exist: " + assetPath);
			return false;
		}
		byte[] array = FileHelper.ReadAllBytes(assetPath);
		string @string = Encoding.UTF8.GetString(array, 0, array.Length);
		DebugLog.Log(GetType(), "ParseFileAndContinue: parsed minimum version: " + @string);
		if (!string.IsNullOrEmpty(@string))
		{
		}
		ChimeraVersionNumber chimeraVersionNumber = new ChimeraVersionNumber();
		chimeraVersionNumber.ReportError = Debug.LogError;
		chimeraVersionNumber = chimeraVersionNumber.FromString(@string);
		if (!string.IsNullOrEmpty(DIContainerInfrastructure.GetVersionService().StoreVersion))
		{
			ChimeraVersionNumber chimeraVersionNumber2 = new ChimeraVersionNumber();
			chimeraVersionNumber2.ReportError = Debug.LogError;
			chimeraVersionNumber2 = chimeraVersionNumber.FromString(DIContainerInfrastructure.GetVersionService().StoreVersion);
			bool flag = chimeraVersionNumber2.IsOlderThan(chimeraVersionNumber);
			DebugLog.Log(string.Concat("[VersionCheck] Remote Version: ", chimeraVersionNumber, " Local Version: ", chimeraVersionNumber2, ", is store version newer: ", flag));
			return flag;
		}
		bool flag2 = DIContainerInfrastructure.GetVersionService().FullVersion.IsOlderThan(chimeraVersionNumber);
		DebugLog.Log(string.Concat("[VersionCheck] Remote Version: ", chimeraVersionNumber, " Local Version: ", DIContainerInfrastructure.GetVersionService().FullVersion, ", is store version newer: ", flag2));
		return flag2;
	}

	public IEnumerator CheckForNewVersionAvailable(MonoBehaviour syncObj, Action onDownloadStarted, Action<bool> callback, Action<float> onDownloadProgress, Action<bool> onIsSlowCallback)
	{
		DebugLog.Log("[VersionCheck] Start Check!");
		if (DIContainerInfrastructure.GetAssetsService().NeedToDownloadAsset(StoreVersionsFileName))
		{
			DebugLog.Log("[VersionCheck] Need to retrieve the " + StoreVersionsFileName);
			onDownloadStarted();
			List<bool> done = new List<bool>();
			string result = null;
			DIContainerInfrastructure.GetAssetsService().Load(StoreVersionsFileName, delegate(string res)
			{
				DebugLog.Log("[VersionCheck] retrieved the " + StoreVersionsFileName + ", result is " + ((res != null) ? "not null" : "null"));
				result = res;
				done.Add(true);
				DebugLog.Log("[VersionCheck] set done to true");
			}, onDownloadProgress, onIsSlowCallback);
			DebugLog.Log("[VersionCheck] starting to wait for the loading of " + StoreVersionsFileName);
			while (done.Count == 0)
			{
				yield return new WaitForEndOfFrame();
			}
			DebugLog.Log("[VersionCheck] waiting finished");
			if (result != null)
			{
				DebugLog.Log("[VersionCheck] starting the CheckForNewVersionAvailable coroutine, syncObj is " + ((!syncObj) ? "null" : "not null"));
				yield return syncObj.StartCoroutine(CheckForNewVersionAvailable(syncObj, callback));
			}
			else
			{
				DebugLog.Log("[VersionCheck] the " + StoreVersionsFileName + " file was not successfully retrieved, invoking callback with null");
				PerformAdditionalVersionCheck(callback);
			}
		}
		else
		{
			yield return syncObj.StartCoroutine(CheckForNewVersionAvailable(syncObj, callback, onDownloadStarted));
		}
	}

	private void PerformAdditionalVersionCheck(Action<bool> callback)
	{
		if (Wp8AdditionalNewVersionCheck != null)
		{
			Wp8AdditionalNewVersionCheck(delegate
			{
				callback(true);
			}, delegate
			{
				callback(false);
			});
		}
		else
		{
			callback(false);
		}
	}

	private IEnumerator CheckForNewVersionAvailable(MonoBehaviour syncObj, Action<bool> callback, Action onDownloadStarted = null)
	{
		DebugLog.Log("[VersionCheck] Checking the version of the latest store app with the help of the " + StoreVersionsFileName);
		AssetInfo versionAssetInfo = DIContainerInfrastructure.GetAssetData().GetAssetInfoFor(StoreVersionsFileName);
		if (versionAssetInfo != null)
		{
			string path = versionAssetInfo.FilePath;
			if (File.Exists(path))
			{
				byte[] storeVersionBytes = FileHelper.ReadAllBytes(path);
				string version = Encoding.UTF8.GetString(storeVersionBytes, 0, storeVersionBytes.Length);
				if (string.IsNullOrEmpty(version))
				{
				}
				ChimeraVersionNumber remoteVersion = new ChimeraVersionNumber
				{
					ReportError = Debug.LogError
				}.FromString(version);
				if (!string.IsNullOrEmpty(DIContainerInfrastructure.GetVersionService().StoreVersion))
				{
					ChimeraVersionNumber localVersion2 = new ChimeraVersionNumber
					{
						ReportError = Debug.LogError
					};
					localVersion2 = remoteVersion.FromString(DIContainerInfrastructure.GetVersionService().StoreVersion);
					bool isNew = localVersion2.IsOlderThan(remoteVersion);
					DebugLog.Log(string.Concat("[VersionCheck] Remote Version: ", remoteVersion, " Local Version: ", localVersion2, ", is store version newer: ", isNew));
					if (isNew)
					{
						callback(true);
					}
					else
					{
						PerformAdditionalVersionCheck(callback);
					}
				}
				else
				{
					bool isNew2 = DIContainerInfrastructure.GetVersionService().FullVersion.IsOlderThan(remoteVersion);
					DebugLog.Log(string.Concat("[VersionCheck] Remote Version: ", remoteVersion, " Local Version: ", DIContainerInfrastructure.GetVersionService().FullVersion, ", is store version newer: ", isNew2));
					if (isNew2)
					{
						callback(true);
					}
					else
					{
						PerformAdditionalVersionCheck(callback);
					}
				}
				yield break;
			}
			DebugLog.Error("[VersionCheck] Could not load " + StoreVersionsFileName + "! (file does not exist: " + path + ")");
		}
		else
		{
			DebugLog.Error("[VersionCheck] " + StoreVersionsFileName + " has been downloaded but is not inside the AssetData");
		}
		PerformAdditionalVersionCheck(callback);
	}
}
