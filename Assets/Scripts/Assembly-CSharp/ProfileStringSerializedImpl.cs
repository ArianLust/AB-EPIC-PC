using System;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class ProfileStringSerializedImpl<T> : IProfileMgr<T> where T : class, ISerializedPlayerProfile
{
	public ISerializer Serializer { get; set; }

	public IStorageService StorageService { get; set; }

	public T CurrentProfile { get; set; }

	public string GetDeviceName()
	{
		return "none";
	}

	public bool SaveProfile(T player)
	{
		DebugLog.Log(GetType(), "SaveProfile start");
		player.SetParserVersionPropertyValue(DIContainerInfrastructure.GetStringSerializer().GetSerializerUniqueName());
		player.ClientVersion = DIContainerInfrastructure.GetVersionService().StoreVersion;
		string value = DIContainerInfrastructure.GetStringSerializer().Serialize(player);
		bool flag = DIContainerInfrastructure.GetPlayerPrefsService().SetString(DIContainerConfig.GetConstants().ProfileKey, value);
		if (flag)
		{
			CurrentProfile = player;
		}
		if (player.LastSaveTimestamp != 0)
		{
			DateTime dateTimeFromTimestamp = DIContainerLogic.GetDeviceTimingService().GetDateTimeFromTimestamp(player.LastSaveTimestamp);
			DateTime presentTime = DIContainerLogic.GetDeviceTimingService().GetPresentTime();
			if (!DIContainerLogic.GetDeviceTimingService().IsSameDay(dateTimeFromTimestamp, presentTime))
			{
				int num = Mathf.FloorToInt((float)(presentTime - dateTimeFromTimestamp).TotalDays);
				player.ActivityIndicator = ((num >= 1) ? Math.Max(player.ActivityIndicator - num, -5) : Math.Min(Mathf.Max(player.ActivityIndicator, 0) + 1, 5));
			}
			DebugLog.Log("[ProfileStringSerializedImpl] Current Player Activity Indicator: " + player.ActivityIndicator);
		}
		player.LastSaveTimestamp = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
		DebugLog.Log(GetType(), "SaveProfile done, success = " + flag);
		return flag;
	}

	public bool SaveCurrentProfile()
	{
		return SaveProfile(CurrentProfile);
	}

	public bool RemoveProfile()
	{
		return DIContainerInfrastructure.GetPlayerPrefsService().SetString(DIContainerConfig.GetConstants().ProfileKey, string.Empty);
	}

	public bool LoadCurrentProfile()
	{
		string @string = DIContainerInfrastructure.GetPlayerPrefsService().GetString(DIContainerConfig.GetConstants().ProfileKey, string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			DebugLog.Log("No Profile");
			CurrentProfile = (T)null;
			return false;
		}
		try
		{
			CurrentProfile = DIContainerInfrastructure.GetStringSerializer().Deserialize<T>(@string);
		}
		catch (Exception ex)
		{
			DebugLog.Log("invalid profile: " + ex);
			CurrentProfile = (T)null;
			return false;
		}
		return true;
	}
}
