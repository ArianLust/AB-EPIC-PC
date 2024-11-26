using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ABH.Shared.Models;
using Chimera.Library.Components.ClientLib.CrossPlatformLib.Source.Models;
using Chimera.Library.Components.Interfaces;
using Rcs;

public class StorageServiceBeaconImpl : IRemoteStorageService
{
	private class RetrieveFriendsState
	{
		private readonly Queue<List<string>> m_requests = new Queue<List<string>>();

		private readonly Action<string, Dictionary<string, string>> m_onSuccess;

		private readonly Action<string, string> m_onError;

		private bool m_valid = true;

		private readonly Dictionary<string, string> m_resultBuffer = new Dictionary<string, string>();

		private bool m_errorReported;

		private Storage m_storage;

		public RetrieveFriendsState(string[] friendIds, Action<string, Dictionary<string, string>> onSuccess, Action<string, string> onError)
		{
			m_storage = new Storage(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy);
			m_onSuccess = onSuccess;
			m_onError = onError;
			List<string> list = new List<string>();
			foreach (string item in friendIds)
			{
				list.Add(item);
				if (list.Count >= DIContainerConfig.GetClientConfig().FacebookFriendsPerRequest)
				{
					m_requests.Enqueue(list);
					list = new List<string>();
				}
			}
			if (list.Count > 0)
			{
				m_requests.Enqueue(list);
			}
			DebugLog.Log(GetType(), "Starting the friend requests. There are " + friendIds.Length + " friends to query, split up into " + m_requests.Count + " requests.");
			MakeNextRequest();
		}

		public void Terminate()
		{
			m_valid = false;
		}

		private void OnSuccess(string key, Dictionary<string, string> accountToValueDictionary)
		{
			if (!m_valid)
			{
				return;
			}
			foreach (string key2 in accountToValueDictionary.Keys)
			{
				if (!m_resultBuffer.ContainsKey(key2))
				{
					m_resultBuffer.Add(key2, accountToValueDictionary[key2]);
				}
			}
			if (m_requests.Count > 0)
			{
				MakeNextRequest();
				return;
			}
			DebugLog.Log(GetType(), "Done with the friend requests. Now we have " + m_resultBuffer.Count + " friend datas.");
			m_onSuccess(key, m_resultBuffer);
		}

		private void OnError(string key, string error)
		{
			if (m_valid && !m_errorReported)
			{
				m_errorReported = true;
				m_onError(key, error);
			}
		}

		private void MakeNextRequest()
		{
			List<string> list = m_requests.Dequeue();
			if (list != null)
			{
				string[] array = list.ToArray();
				DebugLog.Log(GetType(), "Requesting " + array.Length + " friends: " + array.Aggregate(string.Empty, (string acc, string curr) => acc + curr + ","));
				m_storage.Get(new List<string>(array), DIContainerConfig.GetConstants().StoragePublicProfileKey, OnSuccess, delegate(string key, Storage.ErrorCode errorCode)
				{
					OnError(key, errorCode.ToString());
				});
			}
			else
			{
				DebugLog.Error(GetType(), "Requesting no friends!");
			}
		}
	}

	public const string GetPrivateProfileDeserializationError = "Deserialization Error";

	private ABHStorageHandler m_abhStorageHandler;

	private ISerializer m_stringSerializer;

	private List<string> m_syncDisableReasons = new List<string>();

	private Storage m_storage;

	private bool m_syncEnabledPublicProfile = true;

	public bool m_exceptionOnDeserialize;

	private RetrieveFriendsState m_retrieveFriendsState;

	private ABH.Shared.Models.PlayerData LocalPlayerData;

	private ABH.Shared.Models.PlayerData CloudPlayerData;

	public bool SyncEnabled
	{
		get
		{
			return m_syncDisableReasons.Count == 0;
		}
	}

	public StorageServiceBeaconImpl(ISerializer stringSerializer)
	{
		m_abhStorageHandler = new ABHStorageHandler();
		m_stringSerializer = stringSerializer;
		m_storage = new Storage(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy);
	}

	public void GetPublicPlayerDatas(string[] ids, Action<Dictionary<string, PublicPlayerData>> callback, Action<string> error)
	{
		DebugLog.Log(GetType(), "Trying to get public player datas: " + DIContainerConfig.GetConstants().StoragePublicProfileKey + " for " + ids.Count() + " ids");
		m_storage.Get(new List<string>(ids), DIContainerConfig.GetConstants().StoragePublicProfileKey, delegate(string key, Dictionary<string, string> accountToValueDictionary)
		{
			m_abhStorageHandler.OnGotPublicProfiles(key, accountToValueDictionary, callback);
		}, delegate(string key, Storage.ErrorCode errorCode)
		{
			error(errorCode.ToString());
		});
	}

	public void EnableProfileSync(string reason)
	{
		m_syncDisableReasons.Remove(reason);
		m_syncEnabledPublicProfile = true;
	}

	public void DisableProfileSync(string reason, bool publicProfileAlso)
	{
		if (!m_syncDisableReasons.Contains(reason))
		{
			m_syncDisableReasons.Add(reason);
		}
		m_syncEnabledPublicProfile = !publicProfileAlso;
	}

	public void SyncProfileAndResolveConflict(ABH.Shared.Models.PlayerData playerData, Func<ABH.Shared.Models.PlayerData, ABH.Shared.Models.PlayerData, ABH.Shared.Models.PlayerData> resolver, Action completedCallback, bool force = false)
	{
		if (playerData == null)
		{
			return;
		}
		completedCallback = completedCallback ?? ((Action)delegate
		{
		});
		DebugLog.Log(GetType(), "SyncProfileAndResolveConflict");
		if (SyncEnabled)
		{
			DisableProfileSync("SyncProfileAndResolveConflict", false);
			playerData.ClientVersion = DIContainerInfrastructure.GetVersionService().StoreVersion;
			m_storage.Set(DIContainerConfig.GetConstants().StoragePrivateProfileKey, m_stringSerializer.Serialize(playerData), delegate(string key)
			{
				DebugLog.Log(GetType(), string.Format("SyncProfileAndResolveConflict success: " + key));
				EnableProfileSync("SyncProfileAndResolveConflict");
				completedCallback();
			}, delegate(string key, Storage.ErrorCode errorCode)
			{
				DebugLog.Warn(GetType(), string.Format("SyncProfileAndResolveConflict error, key = {0}, errorCode = {1}", key, errorCode));
				EnableProfileSync("SyncProfileAndResolveConflict");
				completedCallback();
			}, delegate(string key, string localValue, string remoteValue)
			{
				try
				{
					DebugLog.Log(GetType(), "SyncProfileAndResolveConflict Conflict. Invoking resolver.");
					remoteValue = remoteValue.Replace('-', '+');
					remoteValue = remoteValue.Replace('_', '/');
					string result = m_stringSerializer.Serialize(resolver(playerData, m_stringSerializer.Deserialize<ABH.Shared.Models.PlayerData>(remoteValue)));
					DebugLog.Log(GetType(), "SyncProfileAndResolveConflict Conflict resolved.");
					return result;
				}
				finally
				{
					EnableProfileSync("SyncProfileAndResolveConflict");
					completedCallback();
				}
			});
		}
		else
		{
			DebugLog.Log(GetType(), "SyncEnabled == false, skipping this sync! Reasons: " + string.Join(", ", m_syncDisableReasons.ToArray()));
			completedCallback();
		}
		if (m_syncEnabledPublicProfile)
		{
			PushPublicProfile(playerData.GetPublicPlayerData());
		}
	}

	public void PushProfile(ABH.Shared.Models.PlayerData playerData)
	{
		if (SyncEnabled)
		{
			playerData.ClientVersion = DIContainerInfrastructure.GetVersionService().StoreVersion;
			m_storage.Set(DIContainerConfig.GetConstants().StoragePrivateProfileKey, m_stringSerializer.Serialize(playerData), delegate
			{
				DebugLog.Log("PushProfile: Success!");
			}, delegate(string key, Storage.ErrorCode errorcode)
			{
				DebugLog.Log("PushProfile: Failed! " + errorcode);
			}, (string key, string local, string remote) => local);
		}
	}

	public void SyncProfile(ABH.Shared.Models.PlayerData playerData, Action<bool> callback = null, bool force = false)
	{
		if (playerData == null)
		{
			return;
		}
		DebugLog.Log(GetType(), "SyncProfile");
		if (SyncEnabled || force)
		{
			playerData.ClientVersion = DIContainerInfrastructure.GetVersionService().StoreVersion;
			m_storage.Set(DIContainerConfig.GetConstants().StoragePrivateProfileKey, m_stringSerializer.Serialize(playerData), delegate(string key)
			{
				DebugLog.Log(GetType(), string.Format("SyncProfile success: " + key));
				if (callback != null)
				{
					callback(true);
				}
			}, delegate(string key, Storage.ErrorCode errorCode)
			{
				DebugLog.Warn(GetType(), string.Format("SyncProfile error, key = {0}, errorCode = {1}", key, errorCode));
				if (callback != null)
				{
					callback(false);
				}
			}, delegate(string key, string localValue, string remoteValue)
			{
				DebugLog.Warn(GetType(), string.Format("SyncProfile error, overriding the cloud data with our local data. key = {0}, errorCode = SyncProfileError", key));
				return localValue;
			});
		}
		if (m_syncEnabledPublicProfile)
		{
			PushPublicProfile(playerData.GetPublicPlayerData());
		}
	}

	public void SyncProfileAndGetConflictedProfile(ABH.Shared.Models.PlayerData playerData, Action<bool, ABH.Shared.Models.PlayerData> callback = null)
	{
		if (playerData == null)
		{
			return;
		}
		DebugLog.Log(GetType(), "SyncProfile");
		if (SyncEnabled)
		{
			DisableProfileSync("SyncProfileAndGetConflictedProfile", false);
			playerData.ClientVersion = DIContainerInfrastructure.GetVersionService().StoreVersion;
			m_storage.Set(DIContainerConfig.GetConstants().StoragePrivateProfileKey, m_stringSerializer.Serialize(playerData), delegate(string key)
			{
				DebugLog.Log(GetType(), string.Format("SyncProfile success: " + key));
				EnableProfileSync("SyncProfileAndGetConflictedProfile");
				if (callback != null)
				{
					callback(true, null);
				}
			}, delegate(string key, Storage.ErrorCode errorCode)
			{
				DebugLog.Warn(GetType(), string.Format("SyncProfile error, key = {0}, errorCode = {1}", key, errorCode));
				EnableProfileSync("SyncProfileAndGetConflictedProfile");
				if (callback != null)
				{
					callback(false, null);
				}
			}, delegate(string key, string localValue, string remoteValue)
			{
				if (callback != null)
				{
					callback(false, m_stringSerializer.Deserialize<ABH.Shared.Models.PlayerData>(remoteValue));
				}
				return remoteValue;
			});
		}
		if (m_syncEnabledPublicProfile)
		{
			PushPublicProfile(playerData.GetPublicPlayerData());
		}
	}

	public void GetPrivateProfile(Action<ABH.Shared.Models.PlayerData> callback, Action<string> error)
	{
		DebugLog.Log(GetType(), "GetPrivateProfile requesting... current profile: " + DIContainerInfrastructure.IdentityService.UserCredentials.Email + ", isGuest: " + DIContainerInfrastructure.IdentityService.IsGuest());
		m_storage.Get(DIContainerConfig.GetConstants().StoragePrivateProfileKey, delegate(string key, string value)
		{
			LocalPlayerData = DIContainerInfrastructure.GetCurrentPlayer().Data;
			try
			{
				if (string.IsNullOrEmpty(value))
				{
					DebugLog.Error(GetType(), "GetPrivateProfile: null or empty, invoking callback(null)");
					callback(null);
				}
				else
				{
					DebugLog.Log(GetType(), "GetPrivateProfile got current profile: " + DIContainerInfrastructure.IdentityService.UserCredentials.Email + ", isGuest: " + DIContainerInfrastructure.IdentityService.IsGuest());
					try
					{
						CloudPlayerData = m_stringSerializer.Deserialize<ABH.Shared.Models.PlayerData>(value);
					}
					catch (Exception)
					{
						DebugLog.Log(GetType(), "deserialize failed, trying on alternative representation");
						value = value.Replace('_', '/').Replace('-', '+');
						CloudPlayerData = m_stringSerializer.Deserialize<ABH.Shared.Models.PlayerData>(value);
						DebugLog.Log(GetType(), "deserialize failed, trying on alternative representation SUCCESS");
					}
					callback(CloudPlayerData);
				}
			}
			catch (Exception ex2)
			{
				if (CloudPlayerData != null && DIContainerInfrastructure.GetVersionService().FullVersion.IsOlderThan(new ChimeraVersionNumber().FromString(CloudPlayerData.ClientVersion)))
				{
					DIContainerInfrastructure.GetPlayerPrefsService().SetString(DIContainerConfig.GetConstants().ProfileKey + "_" + CloudPlayerData.ClientVersion, value);
					callback(LocalPlayerData);
				}
				else
				{
					DebugLog.Error(GetType(), string.Concat("GetPrivateProfile: error deserializing the profile: ", ex2, ", trace: ", ex2.StackTrace));
					error("Deserialization Error");
				}
			}
		}, delegate(string key, Storage.ErrorCode errorCode)
		{
			if (CloudPlayerData != null && DIContainerInfrastructure.GetVersionService().FullVersion.IsOlderThan(new ChimeraVersionNumber().FromString(CloudPlayerData.ClientVersion)))
			{
				callback(LocalPlayerData);
			}
			else if (errorCode == Storage.ErrorCode.ErrorNoSuchKey)
			{
				DebugLog.Warn(GetType(), "GetPrivateProfile: NoSuchKey -> This must be a new account");
				callback(null);
			}
			else
			{
				DebugLog.Log(GetType(), string.Concat("GetPrivateProfile Error (", key, ", ", errorCode, ")"));
				error(errorCode.ToString());
			}
		});
	}

	public void PushPublicProfile(PublicPlayerData publicPlayerData)
	{
		m_storage.Set(DIContainerConfig.GetConstants().StoragePublicProfileKey, m_stringSerializer.Serialize(publicPlayerData), delegate(string key)
		{
			DebugLog.Log(GetType(), string.Format("SyncProfile success: " + key + ": " + publicPlayerData));
		}, delegate(string key, Storage.ErrorCode errorCode)
		{
			DebugLog.Warn(GetType(), string.Format("PushPublicProfile error, doing nothing. errorCode: " + errorCode));
		}, delegate(string key, string localValue, string remoteValue)
		{
			DebugLog.Log(GetType(), string.Format("PushPublicProfile error, overriding the cloud data with our local data. key = {0}", key));
			return localValue;
		});
	}

	public void RefreshPublicPlayerDataFromFriends(IEnumerable<string> friendIds)
	{
		string[] array = (friendIds as string[]) ?? friendIds.ToArray();
		if (friendIds != null && array.Any())
		{
			DebugLog.Log(GetType(), "Start refresh Friend Datas!");
			if (m_retrieveFriendsState != null)
			{
				m_retrieveFriendsState.Terminate();
			}
			m_retrieveFriendsState = new RetrieveFriendsState(array, m_abhStorageHandler.OnGetPublicPlayerDataFromFriendsSuccess, delegate(string key, string errorCode)
			{
				m_abhStorageHandler.OnGetPublicPlayerDataFromFriendsError(key, errorCode.ToString(CultureInfo.InvariantCulture));
			});
		}
	}

	private static bool CheckIfRemoteProfileVersionFromNewerClient(ABH.Shared.Models.PlayerData remotePlayerData)
	{
		ChimeraVersionNumber chimeraVersionNumber = new ChimeraVersionNumber().FromString(remotePlayerData.ClientVersion);
		bool flag = CheckIfRemoteProfileVersionFromNewerClient(chimeraVersionNumber);
		DebugLog.Log(typeof(StorageServiceBeaconImpl), string.Concat(": CheckIfRemoteProfileVersionFromNewerClient: remotePlayerData.ClientVersion: ", remotePlayerData.ClientVersion, ", remoteVersion: ", chimeraVersionNumber, ", res: ", flag));
		return flag;
	}

	private static bool CheckIfRemoteProfileVersionFromNewerClient(ChimeraVersionNumber remoteVersion)
	{
		if (remoteVersion == null)
		{
			return false;
		}
		if (remoteVersion.IsNewerThan(DIContainerInfrastructure.GetProfileMgr().CurrentProfile.ClientVersion))
		{
			return true;
		}
		return false;
	}
}
