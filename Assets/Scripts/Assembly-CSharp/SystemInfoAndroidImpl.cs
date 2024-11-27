using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class SystemInfoAndroidImpl : ISystemInfo
{
	private const string m_appInfoClassName = "de.chimeraentertainment.android.systemtools.AppInfo";

	private readonly AndroidJavaClass m_storageInfoClass;

	public SystemInfoAndroidImpl()
	{
		m_storageInfoClass = new AndroidJavaClass("de.chimeraentertainment.android.systemtools.StorageInfo");
	}

	public string GetLocalCurrencyCode()
	{
		return "n/a";
	}

	public long GetFreeStorageExternal()
	{
		return m_storageInfoClass.CallStatic<long>("getFreeStorageExternal", new object[0]);
	}

	public long GetFreeStorageInternal()
	{
		return m_storageInfoClass.CallStatic<long>("getFreeStorageInternal", new object[0]);
	}

	public long GetTotalStorageExternal()
	{
		return m_storageInfoClass.CallStatic<long>("getTotalStorageExternal", new object[0]);
	}

	public long GetTotalStorageInternal()
	{
		return m_storageInfoClass.CallStatic<long>("getTotalStorageInternal", new object[0]);
	}

	public long GetUsedStorageExternal()
	{
		return m_storageInfoClass.CallStatic<long>("getUsedStorageExternal", new object[0]);
	}

	public long GetUsedStorageInternal()
	{
		return m_storageInfoClass.CallStatic<long>("getUsedStorageInternal", new object[0]);
	}

	public InstallLocation GetInstallLocation()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			return (InstallLocation)m_storageInfoClass.CallStatic<int>("getInstallLocation", new object[1] { @static });
		}
	}

	public long GetInstalledTimeSecondsUTC()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("de.chimeraentertainment.android.systemtools.AppInfo"))
				{
					return androidJavaClass2.CallStatic<long>("getFirstInstalledTime", new object[1] { androidJavaObject });
				}
			}
		}
	}
}
