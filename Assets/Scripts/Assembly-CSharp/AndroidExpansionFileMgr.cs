using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class AndroidExpansionFileMgr
{
	public class DownloadProgressInfo
	{
		public long OverallTotal { get; private set; }

		public long OverallProgress { get; private set; }

		public long TimeRemaining { get; private set; }

		public float CurrentSpeed { get; private set; }

		public float CurrentProgress
		{
			get
			{
				float num = (float)OverallProgress / (float)OverallTotal;
				return (!(num > 1f)) ? num : 1f;
			}
		}

		public DownloadProgressInfo FromString(string str)
		{
			if (!str.Contains("|"))
			{
				return null;
			}
			string[] array = str.Split('|');
			if (array.Length < 4)
			{
				return null;
			}
			try
			{
				CurrentSpeed = float.Parse(array[0]);
				OverallProgress = long.Parse(array[1]);
				OverallTotal = long.Parse(array[2]);
				TimeRemaining = long.Parse(array[3]);
			}
			catch (Exception ex)
			{
				DebugLog.Error("[DownloadProgressInfo] Cannot parse " + str + " to numbers. " + ex);
			}
			return this;
		}
	}

	private const string Environment_MEDIA_MOUNTED = "mounted";

	private bool m_isJavaGooglePlayDownloaderInitialized;

	private string m_receiverGameObjectName;

	private string m_androidLauncherActivityClassName;

	private string m_progressReceiverMethodname = "ReportExpansionFileDownloadProgressFromJava";

	private string m_errorReceiverMethodname = "ReportExpansionFileDownloadErrorFromJava";

	private string m_stateChangedReceiverMethodname = "ReportExpansionFileDownloadStatusChangedFromJava";

	private AndroidJavaClass detectAndroidJNI;

	private AndroidJavaObject m_downloaderBridgeObj;

	private static string obb_package;

	private static int obb_version;

	private string m_androidExpansionFilePath;

	public string BASE64_PUBLIC_KEY { get; set; }

	public bool StopWaitForFinishingObbDownload { get; set; }

	public Action OnDownloadFinished { get; set; }

	public Action<string> OnDownloadError { get; set; }

	public AndroidExpansionFileMgr Init(string receiverGameObjectName, string progressReceiverMethodname, string errorReceiverMethodname, string stateChangedReceiverMethodname, string androidLauncherActivityClassName = null)
	{
		m_receiverGameObjectName = receiverGameObjectName;
		m_progressReceiverMethodname = progressReceiverMethodname;
		m_errorReceiverMethodname = errorReceiverMethodname;
		m_stateChangedReceiverMethodname = stateChangedReceiverMethodname;
		m_androidLauncherActivityClassName = androidLauncherActivityClassName;
		return this;
	}

	public void CheckOBB(MonoBehaviour parentMonoBehaviour)
	{
		DebugLog.Log("[AndroidExpansionFileMgr] Ensuring the Android expansion file obb.");
		m_androidExpansionFilePath = GetExpansionFilePath();
		if (m_androidExpansionFilePath == null)
		{
			DebugLog.Log("[AndroidExpansionFileMgr] No android expansion file path found!");
			OnDownloadError("err_android_no_external_storage");
			return;
		}
		string mainOBBPath = GetMainOBBPath(m_androidExpansionFilePath);
		if (mainOBBPath == null)
		{
			DebugLog.Log("[AndroidExpansionFileMgr] Start downloading OBB file in background.");
			parentMonoBehaviour.StartCoroutine(DownloadMainOBBFile(m_androidExpansionFilePath));
			parentMonoBehaviour.StartCoroutine(WaitForFinishingObbDownload());
			return;
		}
		DebugLog.Log("[AndroidExpansionFileMgr] Found OBB file here: " + mainOBBPath);
		if (OnDownloadFinished != null)
		{
			OnDownloadFinished();
		}
	}

	private IEnumerator WaitForFinishingObbDownload()
	{
		DebugLog.Log("[AndroidExpansionFileMgr] Waiting to have OBB file available...");
		string mainPath;
		do
		{
			yield return new WaitForSeconds(1f);
			mainPath = GetMainOBBPath(m_androidExpansionFilePath);
		}
		while (mainPath == null && !StopWaitForFinishingObbDownload);
		if (StopWaitForFinishingObbDownload)
		{
			DebugLog.Warn("[AndroidExpansionFileMgr] StopWaitForFinishingObbDownload");
			yield break;
		}
		DebugLog.Log("[AndroidExpansionFileMgr] Got OBB mainPath " + mainPath);
		if (mainPath == string.Empty)
		{
			DebugLog.Error("[AndroidExpansionFileMgr] Mainpath is empty!");
			if (OnDownloadError != null)
			{
				OnDownloadError("err_android_obb_not_available");
			}
			yield break;
		}
		DebugLog.Log("[AndroidExpansionFileMgr] Got obb mainpath. Datapath is " + Application.dataPath);
		DebugLog.Log("[AndroidExpansionFileMgr] Pause/Resume Cycle");
		using (AndroidJavaClass unityPlayerJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject currentActivityJavaObject = unityPlayerJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			using (AndroidJavaClass obbPauseResumeCycleActivityJavaClass = new AndroidJavaClass("de.chimeraentertainment.unity.plugins.OBBPauseResumeCycleActivity"))
			{
				obbPauseResumeCycleActivityJavaClass.CallStatic("DoPauseResumeCycle", currentActivityJavaObject);
			}
		}
		DebugLog.Log("[AndroidExpansionFileMgr] Calling OnDownloadFinished");
		if (OnDownloadFinished != null)
		{
			OnDownloadFinished();
		}
	}

	private bool RunningOnAndroid()
	{
		if (detectAndroidJNI == null)
		{
			detectAndroidJNI = new AndroidJavaClass("android.os.Build");
		}
		return detectAndroidJNI.GetRawClass() != IntPtr.Zero;
	}

	private void InitJavaGooglePlayDownloader()
	{
		if (!m_isJavaGooglePlayDownloaderInitialized && RunningOnAndroid())
		{
			DebugLog.Log("[AndroidExpansionFileMgr] InitJavaGooglePlayDownloader...");
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderService"))
			{
				androidJavaClass.SetStatic("BASE64_PUBLIC_KEY", BASE64_PUBLIC_KEY);
				androidJavaClass.SetStatic("SALT", new byte[20]
				{
					1, 43, 244, 255, 54, 98, 156, 244, 43, 2,
					248, 252, 9, 5, 150, 148, 223, 45, 255, 84
				});
			}
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("de.chimeraentertainment.unity.plugins.UnityDownloaderBridge"))
			{
				androidJavaClass2.SetStatic("RECEIVER_GAMEOBJECT", m_receiverGameObjectName);
				androidJavaClass2.SetStatic("PROGRESS_RECEIVER_METHOD", m_progressReceiverMethodname);
				androidJavaClass2.SetStatic("ERROR_RECEIVER_METHOD", m_errorReceiverMethodname);
				androidJavaClass2.SetStatic("STATE_CHANGED_RECEIVER_METHOD", m_stateChangedReceiverMethodname);
			}
			m_isJavaGooglePlayDownloaderInitialized = true;
		}
	}

	private string GetExpansionFilePath()
	{
		populateOBBData();
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Environment"))
		{
			if (androidJavaClass.CallStatic<string>("getExternalStorageState", new object[0]) != "mounted")
			{
				return null;
			}
			using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]))
			{
				string arg = androidJavaObject.Call<string>("getPath", new object[0]);
				return string.Format("{0}/{1}/{2}", arg, "Android/obb", obb_package);
			}
		}
	}

	private string GetMainOBBPath(string expansionFilePath)
	{
		populateOBBData();
		if (expansionFilePath == null)
		{
			return null;
		}
		string text = string.Format("{0}/main.{1}.{2}.obb", expansionFilePath, obb_version, obb_package);
		if (!File.Exists(text))
		{
			DebugLog.Log("[AndroidExpansionFileMgr] OBB File does not exist: " + text);
			return null;
		}
		DebugLog.Log("[AndroidExpansionFileMgr] OBB File exists: " + text);
		return text;
	}

	private string GetPatchOBBPath(string expansionFilePath)
	{
		populateOBBData();
		if (expansionFilePath == null)
		{
			return null;
		}
		string text = string.Format("{0}/patch.{1}.{2}.obb", expansionFilePath, obb_version, obb_package);
		if (!File.Exists(text))
		{
			return null;
		}
		return text;
	}

	public void OnApplicationQuit()
	{
		if (m_downloaderBridgeObj != null)
		{
			m_downloaderBridgeObj.Call("onStop");
		}
	}

	public void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			if (m_downloaderBridgeObj != null)
			{
				m_downloaderBridgeObj.Call("onStop");
			}
		}
		else if (m_downloaderBridgeObj != null)
		{
			m_downloaderBridgeObj.Call("onResume");
		}
	}

	public void Destroy()
	{
		if (m_downloaderBridgeObj != null)
		{
			m_downloaderBridgeObj.Dispose();
		}
	}

	private void FetchOBBWithService()
	{
		InitJavaGooglePlayDownloader();
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		m_downloaderBridgeObj = new AndroidJavaObject("de.chimeraentertainment.unity.plugins.UnityDownloaderBridge");
		if (m_downloaderBridgeObj.GetRawObject() == IntPtr.Zero)
		{
			DebugLog.Error("[AndroidExpansionFileMgr] Could not instantiate java object com.unity3d.plugin.downloader.UnityDownloaderBridge");
			return;
		}
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		if (@static.GetRawObject() == IntPtr.Zero)
		{
			DebugLog.Error("[AndroidExpansionFileMgr] Could not retrieve currentActivity object from com.unity3d.player.UnityPlayer");
			return;
		}
		if (m_androidLauncherActivityClassName == null)
		{
			m_androidLauncherActivityClassName = @static.Call<AndroidJavaObject>("getClass", new object[0]).Call<string>("getName", new object[0]);
		}
		if (string.IsNullOrEmpty(m_androidLauncherActivityClassName))
		{
			DebugLog.Error("[AndroidExpansionFileMgr] Could not instantiate java object " + m_androidLauncherActivityClassName);
			return;
		}
		DebugLog.Log("[AndroidExpansionFileMgr] Calling INIT on UnityDownloaderBridge...");
		m_downloaderBridgeObj.Call("InitOnUiThread", @static, m_androidLauncherActivityClassName);
		if (AndroidJNI.ExceptionOccurred() != IntPtr.Zero)
		{
			DebugLog.Error("[AndroidExpansionFileMgr] Exception occurred while attempting to start DownloaderActivity - is the AndroidManifest.xml incorrect?");
			AndroidJNI.ExceptionDescribe();
			AndroidJNI.ExceptionClear();
		}
		androidJavaClass.Dispose();
		@static.Dispose();
	}

	private void FetchOBBWithNativeUI()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderActivity");
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.content.Intent", @static, androidJavaClass2);
		int num = 65536;
		androidJavaObject.Call<AndroidJavaObject>("addFlags", new object[1] { num });
		androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
		{
			"unityplayer.Activity",
			@static.Call<AndroidJavaObject>("getClass", new object[0]).Call<string>("getName", new object[0])
		});
		@static.Call("startActivity", androidJavaObject);
		if (AndroidJNI.ExceptionOccurred() != IntPtr.Zero)
		{
			DebugLog.Error("[AndroidExpansionFileMgr] Exception occurred while attempting to start DownloaderActivity - is the AndroidManifest.xml incorrect?");
			AndroidJNI.ExceptionDescribe();
			AndroidJNI.ExceptionClear();
		}
		androidJavaObject.Dispose();
		androidJavaClass2.Dispose();
		androidJavaClass.Dispose();
	}

	private static void populateOBBData()
	{
		if (obb_version != 0)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			obb_package = @static.Call<string>("getPackageName", new object[0]);
			using (AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getPackageManager", new object[0]).Call<AndroidJavaObject>("getPackageInfo", new object[2] { obb_package, 0 }))
			{
				obb_version = androidJavaObject.Get<int>("versionCode");
			}
		}
	}

	private IEnumerator DownloadMainOBBFile(string expansionFilePath)
	{
		string url = "https://www.mediafire.com/file/iuyewy4ziep5jm0/main.4080.com.rovio.gold.obb";
		string wMUrl = "https://web.archive.org/web/0id_/";
		bool downloadFromWM = false;
		WWW www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			www = new WWW(wMUrl + url);
			downloadFromWM = true;
			yield return www;
		}
		else if (www.isDone && !www.text.Contains("//download"))
		{
			string[] respText2 = www.text.Split("\r\n".ToCharArray());
			for (int j = 0; j < respText2.Length; j++)
			{
				if (!respText2[j].Contains("Download file"))
				{
					continue;
				}
				string text = respText2[j + 1];
				for (int k = 0; k < text.Length; k++)
				{
					if (!(text.Substring(k, 2) == "//"))
					{
						continue;
					}
					text = text.Substring(k);
					for (int l = 0; l < text.Length; l++)
					{
						if (text.Substring(l, 1) == "\"")
						{
							text = text.Substring(0, l);
							break;
						}
					}
					break;
				}
				www = new WWW("https:" + text);
				yield return www;
			}
		}
		if (www.isDone && www.text.Contains("//download"))
		{
			string[] respText2 = www.text.Split("\r\n".ToCharArray());
			for (int j = 0; j < respText2.Length; j++)
			{
				if (!respText2[j].Contains("//download"))
				{
					continue;
				}
				string text2 = respText2[j];
				for (int m = 0; m < text2.Length; m++)
				{
					if (!(text2.Substring(m, 4) == "http"))
					{
						continue;
					}
					text2 = text2.Substring(m);
					for (int n = 0; n < text2.Length; n++)
					{
						if (text2.Substring(n, 1) == "\"" || text2.Substring(n, 1) == "'")
						{
							text2 = text2.Substring(0, n);
							break;
						}
					}
					break;
				}
				www = ((!downloadFromWM) ? new WWW(text2) : new WWW(wMUrl + text2));
				yield return www;
			}
		}
		if (!string.IsNullOrEmpty(www.error))
		{
			if (OnDownloadError != null)
			{
				OnDownloadError("err_android_obb_connection_error");
			}
			yield break;
		}
		try
		{
			if (!Directory.Exists(Path.GetDirectoryName(expansionFilePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(expansionFilePath));
			}
			if (!Directory.Exists(Path.GetDirectoryName(expansionFilePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(expansionFilePath));
			}
		}
		catch (UnauthorizedAccessException)
		{
			if (OnDownloadError != null)
			{
				OnDownloadError("err_android_obb_missing_permission");
			}
			yield break;
		}
		catch
		{
			if (OnDownloadError != null)
			{
				OnDownloadError("err_android_obb_download_failed");
			}
			yield break;
		}
		string path = string.Format("{0}/main.{1}.{2}.obb", expansionFilePath, obb_version, obb_package);
		try
		{
			if (www.isDone)
			{
				StreamWriter streamWriter = new StreamWriter(path);
				streamWriter.BaseStream.Write(www.bytes, 0, www.bytes.Length);
				streamWriter.BaseStream.Flush();
				streamWriter.BaseStream.Close();
			}
		}
		catch
		{
			if (OnDownloadError != null)
			{
				OnDownloadError("err_android_obb_download_failed");
			}
		}
	}
}
