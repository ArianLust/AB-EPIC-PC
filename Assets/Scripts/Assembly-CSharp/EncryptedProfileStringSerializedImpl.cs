using System;
using Chimera.Library.Components.Interfaces;

public class EncryptedProfileStringSerializedImpl<T> : IProfileMgr<T> where T : class, ISerializedPlayerProfile
{
	public ISerializer UnencryptedSerializerFallback { get; set; }

	public ISerializer Serializer { get; set; }

	public IStorageService StorageService { get; set; }

	public T CurrentProfile { get; set; }

	public bool SaveProfile(T player)
	{
		player.SetParserVersionPropertyValue(DIContainerInfrastructure.GetStringSerializer().GetSerializerUniqueName());
		player.ClientVersion = DIContainerInfrastructure.GetVersionService().StoreVersion;
		byte[] input = DIContainerInfrastructure.GetBalancingDataSerializer().SerializeToBytes(player);
		byte[] inArray = DIContainerInfrastructure.GetEncryptionService().Encrypt3DES(input, DIContainerConfig.Key, DIContainerConfig.GetConstants().EncryptionAlgo);
		string value = Convert.ToBase64String(inArray);
		bool flag = DIContainerInfrastructure.GetPlayerPrefsService().SetString(DIContainerConfig.GetConstants().ProfileKey, value);
		player.LastSaveTimestamp = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
		if (flag)
		{
			CurrentProfile = player;
		}
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
			return false;
		}
		byte[] array = null;
		try
		{
			byte[] input = Convert.FromBase64String(@string);
			array = DIContainerInfrastructure.GetEncryptionService().Decrypt3DES(input, DIContainerConfig.Key, DIContainerConfig.GetConstants().EncryptionAlgo);
			CurrentProfile = DIContainerInfrastructure.GetStringSerializer().Deserialize<T>(array);
		}
		catch (Exception ex)
		{
			DebugLog.Error("Couldn't decrypt player profile, using fallback! " + ex);
		}
		if (array == null || CurrentProfile == null)
		{
			try
			{
				CurrentProfile = UnencryptedSerializerFallback.Deserialize<T>(@string);
				SaveCurrentProfile();
			}
			catch (Exception ex2)
			{
				DebugLog.Error("invalid profile: " + ex2);
				CurrentProfile = (T)null;
				return false;
			}
		}
		return true;
	}
}
