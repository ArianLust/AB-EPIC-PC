using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using ABH.Shared.Models.InventoryItems;
using Chimera.Library.Components.Interfaces;
using Chimera.Library.Components.Models;
using Chimera.Library.Components.Services;
using UnityEngine;

public class DIContainerBalancing
{
	private static readonly string m_serializedBalancingDataContainerFileExtension;

	private static IBalancingDataLoaderService m_service;

	private static bool m_isInitializing;

	public static Action<string> ReportError;

	private static LootTableBalancingDataProvider m_lootTableBalancingDataPovider;

	private static InventoryItemBalancingDataPovider m_inventoryItemBalancingDataPovider;

	private static IBalancingDataLoaderService m_eventBalancingService;

	public static string BalancingDataAssetFilename
	{
		get
		{
			return DIContainerInfrastructure.GetTargetBuildGroup() + "_" + BalancingDataResourceFilename + "_" + DIContainerInfrastructure.GetVersionService().StoreVersion + ".bytes";
		}
	}

	public static string EventBalancingDataAssetFilename
	{
		get
		{
			return DIContainerInfrastructure.GetTargetBuildGroup() + "_" + EventBalancingDataResourceFilename + ".bytes";
		}
	}

	public static string BalancingDataResourceFilename
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(typeof(SerializedBalancingDataContainer).Name);
			return stringBuilder.ToString();
		}
	}

	public static string EventBalancingDataResourceFilename
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SerializedEventBalancingDataContainer");
			return stringBuilder.ToString();
		}
	}

	public static IBalancingDataLoaderService Service
	{
		get
		{
			if (m_isInitializing)
			{
				ReportError("Balancing Service is initializing, please try again!");
			}
			if (m_service == null)
			{
				ReportError("Balancing Service not initialized!");
			}
			return m_service;
		}
	}

	public static LootTableBalancingDataProvider LootTableBalancingDataPovider
	{
		get
		{
			return m_lootTableBalancingDataPovider ?? (m_lootTableBalancingDataPovider = new LootTableBalancingDataProvider());
		}
		set
		{
			m_lootTableBalancingDataPovider = value;
		}
	}

	public static IBalancingDataLoaderService EventBalancingService
	{
		get
		{
			if (m_eventBalancingService == null)
			{
				DebugLog.Log("Event Balancing Service not initialized!");
			}
			return m_eventBalancingService;
		}
	}

	public static bool EventBalancingLoadingPending { get; private set; }

	public static bool IsInitialized { get; private set; }

	[method: MethodImpl(32)]
	public static event Action OnBalancingDataInitialized;

	static DIContainerBalancing()
	{
		m_serializedBalancingDataContainerFileExtension = ".bytes";
		ReportError = DebugLog.Error;
	}

	public static bool Init(Action<BalancingInitErrorCode> errorCallback = null, bool restart = false)
	{
		if (restart)
		{
			IsInitialized = false;
		}
		if (m_isInitializing)
		{
			if (errorCallback != null)
			{
				errorCallback(BalancingInitErrorCode.INIT_IN_PROGRESS);
			}
			return false;
		}
		if (IsInitialized)
		{
			if (DIContainerBalancing.OnBalancingDataInitialized != null)
			{
				DIContainerBalancing.OnBalancingDataInitialized();
			}
			return true;
		}
		DebugLog.Log("[DIContainerBalancing] Init");
		bool flag = false;
		m_isInitializing = true;
		AssetInfo assetInfoFor = DIContainerInfrastructure.GetAssetData().GetAssetInfoFor(BalancingDataAssetFilename);
		byte[] outBytes;
		if (assetInfoFor == null)
		{
			DebugLog.Log(typeof(DIContainerBalancing), "Asset info for " + BalancingDataAssetFilename + " is null. Loading from local: " + BalancingDataResourceFilename);
			string path = "SerializedBalancingData/" + BalancingDataResourceFilename;
			TextAsset textAsset = Resources.Load(path) as TextAsset;
			if (textAsset == null)
			{
				string text = "Could not load " + BalancingDataResourceFilename + "! (#1)";
				ReportError(text);
				DebugLog.Error(typeof(DIContainerBalancing), text);
				if (errorCallback != null)
				{
					errorCallback(BalancingInitErrorCode.FILE_NOT_FOUND);
				}
				return false;
			}
			outBytes = textAsset.bytes;
		}
		else
		{
			string path = assetInfoFor.FilePath;
			if (!File.Exists(path))
			{
				ReportError("[DIContainerBalancing] Could not load " + BalancingDataResourceFilename + "! (file does not exist: " + path + ")");
				if (errorCallback != null)
				{
					errorCallback(BalancingInitErrorCode.FILE_NOT_FOUND);
				}
				return false;
			}
			outBytes = FileHelper.ReadAllBytes(path);
		}
		if (flag)
		{
			DebugLog.Log("[DIContainerBalancing] Trying to decrypt asset file");
			TryDecrypt(outBytes, out outBytes);
		}
		DebugLog.Log("[DIContainerBalancing] Trying to decompress asset file, Info = " + assetInfoFor);
		byte[] array = DIContainerInfrastructure.GetCompressionService().DecompressIfNecessary(outBytes);
		if (array != null)
		{
			outBytes = array;
		}
		DebugLog.Log("[DIContainerBalancing] Loaded " + outBytes.Length + " bytes of possibly originally compressed and " + ((!flag) ? "un" : string.Empty) + "encrypted asset data.");
		try
		{
			m_service = new BalancingDataLoaderServiceProtobufImpl(outBytes, DIContainerInfrastructure.GetBalancingDataSerializer().Deserialize, delegate(string msg)
			{
				DebugLog.Log(typeof(BalancingDataLoaderServiceProtobufImpl), msg);
			}, delegate(string msg)
			{
				DebugLog.Error(typeof(BalancingDataLoaderServiceProtobufImpl), msg);
			});
		}
		catch (Exception ex)
		{
			DebugLog.Error(ex.ToString());
			if (flag)
			{
				DebugLog.Error("Maybe you chose the wrong decryption key and/or -algorithm?");
			}
			throw ex;
		}
		m_isInitializing = false;
		IsInitialized = true;
		if (DIContainerBalancing.OnBalancingDataInitialized != null)
		{
			DIContainerBalancing.OnBalancingDataInitialized();
		}
		return true;
	}

	private static bool TryDecrypt(byte[] inBytes, out byte[] outBytes)
	{
		try
		{
			outBytes = DIContainerInfrastructure.GetEncryptionService().Decrypt3DES(inBytes, DIContainerConfig.Key, DIContainerConfig.GetConstants().EncryptionAlgo);
		}
		catch (Exception ex)
		{
			DebugLog.Error("[DIContainerBalancing] " + ex);
			outBytes = inBytes;
			return false;
		}
		return true;
	}

	public static void Reset()
	{
		m_service = null;
		m_inventoryItemBalancingDataPovider = null;
		m_lootTableBalancingDataPovider = null;
		m_eventBalancingService = null;
	}

	public static InventoryItemBalancingDataPovider GetInventoryItemBalancingDataPovider()
	{
		if (m_inventoryItemBalancingDataPovider == null)
		{
			m_inventoryItemBalancingDataPovider = new InventoryItemBalancingDataPovider();
		}
		return m_inventoryItemBalancingDataPovider;
	}

	public static bool GetEventBalancingDataPoviderAsynch(Action<IBalancingDataLoaderService> callback)
	{
		if (EventBalancingLoadingPending)
		{
			DebugLog.Error("Event balancing already loading! Stopped to prevent skynest crash");
			return false;
		}
		EventBalancingLoadingPending = true;
		if (DIContainerInfrastructure.GetAssetsService().NeedToDownloadAsset(EventBalancingDataAssetFilename))
		{
			DIContainerInfrastructure.GetAssetsService().Load(EventBalancingDataAssetFilename, delegate(string result)
			{
				if (result != null)
				{
					EventBalancingLoadingPending = false;
					FinishWithEventBalancingInit(callback);
				}
				else
				{
					EventBalancingLoadingPending = false;
					callback(null);
				}
			}, SetDownloadProgress, SetSlowProgress);
			return true;
		}
		if (m_eventBalancingService != null)
		{
			EventBalancingLoadingPending = false;
			if (callback != null)
			{
				callback(m_eventBalancingService);
			}
			return false;
		}
		EventBalancingLoadingPending = false;
		FinishWithEventBalancingInit(callback);
		return false;
	}

	public static void SetDownloadProgress(float loadingProgress)
	{
	}

	private static void SetSlowProgress(bool isSlow)
	{
	}

	private static bool FinishWithEventBalancingInit(Action<IBalancingDataLoaderService> callback)
	{
		AssetInfo assetInfoFor = DIContainerInfrastructure.GetAssetData().GetAssetInfoFor(EventBalancingDataAssetFilename);
		if (assetInfoFor == null)
		{
			DebugLog.Log(EventBalancingDataAssetFilename + " asset data does not exist, contents: " + DIContainerInfrastructure.GetAssetData().Assets.Aggregate(string.Empty, (string acc, KeyValuePair<string, AssetInfo> kvp) => string.Concat(acc, "[", kvp.Key, " => ", kvp.Value, "]")));
		}
		byte[] data;
		if (assetInfoFor == null)
		{
			DebugLog.Log("[DIContainerBalancing] Asset info for " + EventBalancingDataAssetFilename + " is null. Loading from local: " + EventBalancingDataResourceFilename);
			TextAsset textAsset = Resources.Load("SerializedBalancingData/" + EventBalancingDataResourceFilename) as TextAsset;
			if (textAsset == null)
			{
				string obj = "Could not load " + EventBalancingDataResourceFilename + "! (#1)";
				ReportError(obj);
				DebugLog.Error("[DIContainerBalancing] error");
				callback(null);
				return false;
			}
			data = textAsset.bytes;
		}
		else
		{
			string filePath = assetInfoFor.FilePath;
			if (!File.Exists(filePath))
			{
				ReportError("Could not load " + EventBalancingDataResourceFilename + "! (file does not exist: " + filePath + ")");
				callback(null);
				return false;
			}
			data = FileHelper.ReadAllBytes(filePath);
		}
		DebugLog.Log("[DIContainerBalancing] Trying to decompress asset file, Info = " + assetInfoFor);
		data = DIContainerInfrastructure.GetCompressionService().DecompressIfNecessary(data);
		DebugLog.Log("[DIContainerBalancing] Loaded " + data.Length + " bytes of possibly originally compressed");
		try
		{
			m_eventBalancingService = new BalancingDataLoaderServiceProtobufImpl(data, DIContainerInfrastructure.GetBalancingDataSerializer().Deserialize, null, null);
			IList<EventManagerBalancingData> balancingDataList = m_eventBalancingService.GetBalancingDataList<EventManagerBalancingData>();
			DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
			{
				// sales
				IList<SalesManagerBalancingData> balancingDataList24 = DIContainerBalancing.m_service.GetBalancingDataList<SalesManagerBalancingData>();
				balancingDataList24[6].StartTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 25, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList24[6].EndTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 26, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList24[7].StartTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 25, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList24[7].EndTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 4, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList24[8].StartTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 25, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList24[8].EndTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 4, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList24[9].StartTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 25, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList24[9].EndTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 4, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList24[10].StartTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 25, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList24[10].EndTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 4, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList24[11].StartTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 17, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList24[11].EndTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 18, 21, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				if (trustedTime.CompareTo(new DateTime(trustedTime.Year, 3, 5, 20, 0, 0)) != -1)
				{
					balancingDataList[30].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 18, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[30].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 27, 20, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
			        balancingDataList[31].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 8, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[31].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 10, 20, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[32].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 22, 14, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[32].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 1, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[33].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 1, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[33].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 13, 14, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[34].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 2, 14, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[34].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 12, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[35].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 22, 13, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[35].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 26, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[36].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 7, 12, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[36].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 17, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[37].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 29, 12, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[37].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 31, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[38].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 8, 11, 12, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[38].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 8, 21, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[39].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 7, 12, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[39].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 11, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[40].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 22, 12, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[40].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 2, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[41].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 14, 12, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[41].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 16, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[42].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 27, 12, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[42].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 11, 6, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[43].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 11, 16, 12, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[43].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 11, 20, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[44].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 2, 12, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[44].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 4, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[45].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 15, 16, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList[45].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 25, 19, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				}
				balancingDataList[46].EventStartTimeStamp = Convert.ToUInt32(new DateTime((trustedTime.Month >= 12) ? trustedTime.Year : (trustedTime.Year - 1), 12, 30, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList[46].EventEndTimeStamp = Convert.ToUInt32(new DateTime((trustedTime.Month <= 12) ? trustedTime.Year : (trustedTime.Year + 1), 1, 1, 20, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList[47].EventStartTimeStamp = Convert.ToUInt32(new DateTime((trustedTime.Month <= 12) ? trustedTime.Year : (trustedTime.Year + 1), 1, 11, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList[47].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 1, 15, 20, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList[48].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 1, 27, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList[48].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 1, 29, 20, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList[49].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 9, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList[49].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 19, 20, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList[50].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 24, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList[50].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 26, 20, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList[51].EventStartTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 1, 15, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList[51].EventEndTimeStamp = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 5, 20, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				IList<BonusEventBalancingData> balancingDataList25 = DIContainerBalancing.m_eventBalancingService.GetBalancingDataList<BonusEventBalancingData>();
				if (trustedTime.CompareTo(new DateTime(trustedTime.Year, 3, 4, 7, 0, 0)) != -1)
				{
					// bonus events
					balancingDataList25[0].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 16, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[0].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 27, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[1].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 27, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[1].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 3, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[2].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 3, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[2].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 6, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[3].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 6, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[3].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 10, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[4].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 10, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[4].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 17, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[5].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 17, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[5].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 24, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[6].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 24, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[6].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 25, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[7].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 25, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[7].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 4, 27, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[8].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 4, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[8].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 6, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[9].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 7, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[9].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 10, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[10].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 14, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[10].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 17, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[11].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 18, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[11].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 20, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[12].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 21, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[12].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 24, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[13].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 27, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[13].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 5, 30, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[14].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 4, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[14].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 6, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[15].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 6, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[15].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 7, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[16].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 10, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[16].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 13, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[17].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 15, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[17].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 16, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[18].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 18, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[18].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 20, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[19].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 21, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[19].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 22, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[20].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 24, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[20].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 27, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[21].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 6, 30, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[21].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 4, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[22].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 4, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[22].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 6, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[23].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 8, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[23].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 9, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[24].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 16, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[24].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 18, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[25].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 18, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[25].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 19, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[26].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 20, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[26].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 21, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[27].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 26, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[27].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 7, 27, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[28].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 8, 5, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[28].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 8, 8, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[29].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 8, 8, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[29].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 8, 10, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[30].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 8, 20, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[30].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 8, 21, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[31].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 8, 26, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[31].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 8, 27, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[32].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 8, 30, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[32].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 2, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[33].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 10, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[33].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 12, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[34].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 17, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[34].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 18, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[35].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 23, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[35].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 25, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[36].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 26, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[36].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 9, 28, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[37].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 4, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[37].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 5, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[38].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 9, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[38].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 11, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[39].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 17, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[39].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 20, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[40].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 25, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[40].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 10, 27, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[41].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 11, 3, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[41].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 11, 5, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[42].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 11, 8, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[42].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 11, 9, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[43].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 11, 13, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[43].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 11, 15, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[44].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 11, 22, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[44].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 11, 25, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[45].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 2, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[45].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 4, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[46].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 6, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[46].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 8, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[47].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 14, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[47].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 15, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[48].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 24, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
					balancingDataList25[48].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 27, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				}
				balancingDataList25[49].StartDate = Convert.ToUInt32(new DateTime((trustedTime.Month <= 12) ? trustedTime.Year : (trustedTime.Year + 1), 1, 1, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[49].EndDate = Convert.ToUInt32(new DateTime((trustedTime.Month <= 12) ? trustedTime.Year : (trustedTime.Year + 1), 1, 4, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[50].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 1, 14, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[50].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 1, 15, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[51].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 1, 18, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[51].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 1, 20, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[52].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 1, 27, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[52].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 1, 30, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[53].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 4, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[53].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 5, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[54].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 7, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[54].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 9, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[55].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 16, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[55].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 19, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[56].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 24, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[56].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 2, 27, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[57].StartDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 3, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				balancingDataList25[57].EndDate = Convert.ToUInt32(new DateTime(trustedTime.Year, 3, 4, 7, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
			});
			// some type of igtbap fix idk
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.Inventory.SkinItems != null)
			{
				List<SkinItemData> skinItems = DIContainerInfrastructure.GetCurrentPlayer().Data.Inventory.SkinItems;
				List<ClassItemData> classItems = DIContainerInfrastructure.GetCurrentPlayer().Data.Inventory.ClassItems;
				IList<CollectionGroupBalancingData> balancingDataList26 = DIContainerBalancing.m_service.GetBalancingDataList<CollectionGroupBalancingData>();
				for (int i = 0; i < skinItems.Count; i++)
				{
					for (int j = 0; j < classItems.Count; j++)
					{
						for (int k = 0; k < balancingDataList26.Count; k++)
						{
							Dictionary<string, int> reward = balancingDataList26[k].Reward;
							string text2 = reward.ElementAt(reward.Count - 1).Key.Split("_".ToCharArray()).Last<string>();
							if (classItems[j].NameId == "class_" + text2 || (text2.Contains("elite") && skinItems[i].NameId == "skin_" + text2))
							{
								if (DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentEventManager != null && (DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentEventManager.EventCampaignData == null || DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentEventManager.EventCampaignData.RewardStatus != EventCampaignRewardStatus.unlocked))
								{
									balancingDataList26[k].LocaBaseId = "";
									balancingDataList26[k].Reward = new Dictionary<string, int>();
									balancingDataList26[k].Reward["collection_elite_chest_campaign"] = 1;
								}
								if (balancingDataList26[k].FallbackReward == null || balancingDataList26[k].FallbackRewardAssetBaseId.StartsWith("Headergear") || balancingDataList26[k].FallbackRewardLocaId.Contains("class") || balancingDataList26[k].FallbackRewardLocaId.Contains("elite"))
								{
									balancingDataList26[k].FallbackReward = new Dictionary<string, int>();
									balancingDataList26[k].FallbackReward["lucky_coin"] = 80;
									balancingDataList26[k].FallbackRewardAssetBaseId = "Resource_LuckyCoin";
									balancingDataList26[k].FallbackRewardLocaId = "player_stat_lucky_coin";
								}
								balancingDataList26[k].RewardAssetBaseId = "EliteChest_Large";
								balancingDataList26[k].RewardLocaId = "";
							}
						}
					}
				}
			}
			// offers for birdday
			ICollection<BuyableShopOfferBalancingData> balancingDataList2 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData.Level = 1;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary["skin_eliteknight"] = 1;
			buyableShopOfferBalancingData.OfferContents = dictionary;
			buyableShopOfferBalancingData.SortPriority = 1;
			buyableShopOfferBalancingData.SlotId = 101;
			buyableShopOfferBalancingData.Category = "shop_global_classes";
			buyableShopOfferBalancingData.NameId = "offer_class_red_skin_knight_02";
			buyableShopOfferBalancingData.AssetId = string.Empty;
			buyableShopOfferBalancingData.LocaId = "bird_class_knight_adv";
			buyableShopOfferBalancingData.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData.PopupLoca = "unlock_premiumeliteskins";
			buyableShopOfferBalancingData.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliteknight",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_red",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_knight",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_knight",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliteknight",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData.HideUnlessOnSale = true;
			balancingDataList2.Add(buyableShopOfferBalancingData);
			ICollection<BuyableShopOfferBalancingData> balancingDataList3 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData2 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData2.Level = 1;
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			dictionary2["skin_eliteguardian"] = 1;
			buyableShopOfferBalancingData2.OfferContents = dictionary2;
			buyableShopOfferBalancingData2.SortPriority = 1;
			buyableShopOfferBalancingData2.SlotId = 102;
			buyableShopOfferBalancingData2.Category = "shop_global_classes";
			buyableShopOfferBalancingData2.NameId = "offer_class_red_skin_eliteguardian_02";
			buyableShopOfferBalancingData2.AssetId = string.Empty;
			buyableShopOfferBalancingData2.LocaId = "bird_class_guardian_adv";
			buyableShopOfferBalancingData2.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData2.PopupLoca = "unlock_t2eliteskins";
			buyableShopOfferBalancingData2.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliteguardian",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_red",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_guardian",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData2.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_guardian",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliteguardian",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData2.HideUnlessOnSale = true;
			balancingDataList3.Add(buyableShopOfferBalancingData2);
			ICollection<BuyableShopOfferBalancingData> balancingDataList4 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData3 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData3.Level = 1;
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			dictionary3["skin_elitesamurai"] = 1;
			buyableShopOfferBalancingData3.OfferContents = dictionary3;
			buyableShopOfferBalancingData3.SortPriority = 1;
			buyableShopOfferBalancingData3.SlotId = 103;
			buyableShopOfferBalancingData3.Category = "shop_global_classes";
			buyableShopOfferBalancingData3.NameId = "offer_class_red_skin_elitesamurai_02";
			buyableShopOfferBalancingData3.AssetId = string.Empty;
			buyableShopOfferBalancingData3.LocaId = "bird_class_samurai_adv";
			buyableShopOfferBalancingData3.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData3.PopupLoca = "unlock_t3eliteskins";
			buyableShopOfferBalancingData3.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitesamurai",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_red",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_samurai",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData3.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_samurai",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitesamurai",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData3.HideUnlessOnSale = true;
			balancingDataList4.Add(buyableShopOfferBalancingData3);
			ICollection<BuyableShopOfferBalancingData> balancingDataList5 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData4 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData4.Level = 1;
			Dictionary<string, int> dictionary4 = new Dictionary<string, int>();
			dictionary4["skin_eliteavenger"] = 1;
			buyableShopOfferBalancingData4.OfferContents = dictionary4;
			buyableShopOfferBalancingData4.SortPriority = 1;
			buyableShopOfferBalancingData4.SlotId = 104;
			buyableShopOfferBalancingData4.Category = "shop_global_classes";
			buyableShopOfferBalancingData4.NameId = "offer_class_red_skin_eliteavenger_02";
			buyableShopOfferBalancingData4.AssetId = string.Empty;
			buyableShopOfferBalancingData4.LocaId = "bird_class_avenger_adv";
			buyableShopOfferBalancingData4.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData4.PopupLoca = "unlock_t4eliteskins";
			buyableShopOfferBalancingData4.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliteavenger",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_red",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_avenger",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData4.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_avenger",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliteavenger",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData4.HideUnlessOnSale = true;
			balancingDataList5.Add(buyableShopOfferBalancingData4);
			ICollection<BuyableShopOfferBalancingData> balancingDataList6 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData5 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData5.Level = 1;
			Dictionary<string, int> dictionary5 = new Dictionary<string, int>();
			dictionary5["skin_elitemage"] = 1;
			buyableShopOfferBalancingData5.OfferContents = dictionary5;
			buyableShopOfferBalancingData5.SortPriority = 1;
			buyableShopOfferBalancingData5.SlotId = 201;
			buyableShopOfferBalancingData5.Category = "shop_global_classes";
			buyableShopOfferBalancingData5.NameId = "offer_class_yellow_skin_mage_02";
			buyableShopOfferBalancingData5.AssetId = string.Empty;
			buyableShopOfferBalancingData5.LocaId = "bird_class_mage_adv";
			buyableShopOfferBalancingData5.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData5.PopupLoca = "unlock_premiumeliteskins";
			buyableShopOfferBalancingData5.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitemage",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_yellow",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_mage",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData5.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_mage",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitemage",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData5.HideUnlessOnSale = true;
			balancingDataList6.Add(buyableShopOfferBalancingData5);
			ICollection<BuyableShopOfferBalancingData> balancingDataList7 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData6 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData6.Level = 1;
			Dictionary<string, int> dictionary6 = new Dictionary<string, int>();
			dictionary6["skin_elitelightningbird"] = 1;
			buyableShopOfferBalancingData6.OfferContents = dictionary6;
			buyableShopOfferBalancingData6.SortPriority = 1;
			buyableShopOfferBalancingData6.SlotId = 202;
			buyableShopOfferBalancingData6.Category = "shop_global_classes";
			buyableShopOfferBalancingData6.NameId = "offer_class_yellow_skin_elitelightningbird_02";
			buyableShopOfferBalancingData6.AssetId = string.Empty;
			buyableShopOfferBalancingData6.LocaId = "bird_class_lightningbird_adv";
			buyableShopOfferBalancingData6.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData6.PopupLoca = "unlock_t2eliteskins";
			buyableShopOfferBalancingData6.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitelightningbird",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_yellow",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_lightningbird",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData6.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_lightningbird",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitelightningbird",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData6.HideUnlessOnSale = true;
			balancingDataList7.Add(buyableShopOfferBalancingData6);
			ICollection<BuyableShopOfferBalancingData> balancingDataList8 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData7 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData7.Level = 1;
			Dictionary<string, int> dictionary7 = new Dictionary<string, int>();
			dictionary7["skin_eliterainbird"] = 1;
			buyableShopOfferBalancingData7.OfferContents = dictionary7;
			buyableShopOfferBalancingData7.SortPriority = 1;
			buyableShopOfferBalancingData7.SlotId = 203;
			buyableShopOfferBalancingData7.Category = "shop_global_classes";
			buyableShopOfferBalancingData7.NameId = "offer_class_yellow_skin_eliterainbird_02";
			buyableShopOfferBalancingData7.AssetId = string.Empty;
			buyableShopOfferBalancingData7.LocaId = "bird_class_rainbird_adv";
			buyableShopOfferBalancingData7.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData7.PopupLoca = "unlock_t3eliteskins";
			buyableShopOfferBalancingData7.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliterainbird",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_yellow",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_rainbird",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData7.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_rainbird",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliterainbird",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData7.HideUnlessOnSale = true;
			balancingDataList8.Add(buyableShopOfferBalancingData7);
			ICollection<BuyableShopOfferBalancingData> balancingDataList9 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData8 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData8.Level = 1;
			Dictionary<string, int> dictionary8 = new Dictionary<string, int>();
			dictionary8["skin_elitewizard"] = 1;
			buyableShopOfferBalancingData8.OfferContents = dictionary8;
			buyableShopOfferBalancingData8.SortPriority = 1;
			buyableShopOfferBalancingData8.SlotId = 204;
			buyableShopOfferBalancingData8.Category = "shop_global_classes";
			buyableShopOfferBalancingData8.NameId = "offer_class_yellow_skin_wizard_02";
			buyableShopOfferBalancingData8.AssetId = string.Empty;
			buyableShopOfferBalancingData8.LocaId = "bird_class_wizard_adv";
			buyableShopOfferBalancingData8.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData8.PopupLoca = "unlock_t4eliteskins";
			buyableShopOfferBalancingData8.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitewizard",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_yellow",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_wizard",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData8.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_wizard",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitewizard",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData8.HideUnlessOnSale = true;
			balancingDataList9.Add(buyableShopOfferBalancingData8);
			ICollection<BuyableShopOfferBalancingData> balancingDataList10 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData9 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData9.Level = 1;
			Dictionary<string, int> dictionary9 = new Dictionary<string, int>();
			dictionary9["skin_elitecleric"] = 1;
			buyableShopOfferBalancingData9.OfferContents = dictionary9;
			buyableShopOfferBalancingData9.SortPriority = 1;
			buyableShopOfferBalancingData9.SlotId = 301;
			buyableShopOfferBalancingData9.Category = "shop_global_classes";
			buyableShopOfferBalancingData9.NameId = "offer_class_white_skin_cleric_02";
			buyableShopOfferBalancingData9.AssetId = string.Empty;
			buyableShopOfferBalancingData9.LocaId = "bird_class_cleric_adv";
			buyableShopOfferBalancingData9.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData9.PopupLoca = "unlock_premiumeliteskins";
			buyableShopOfferBalancingData9.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitecleric",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_white",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_cleric",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData9.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_cleric",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitecleric",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData9.HideUnlessOnSale = true;
			balancingDataList10.Add(buyableShopOfferBalancingData9);
			ICollection<BuyableShopOfferBalancingData> balancingDataList11 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData10 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData10.Level = 1;
			Dictionary<string, int> dictionary10 = new Dictionary<string, int>();
			dictionary10["skin_elitedruid"] = 1;
			buyableShopOfferBalancingData10.OfferContents = dictionary10;
			buyableShopOfferBalancingData10.SortPriority = 1;
			buyableShopOfferBalancingData10.SlotId = 302;
			buyableShopOfferBalancingData10.Category = "shop_global_classes";
			buyableShopOfferBalancingData10.NameId = "offer_class_white_skin_elitedruid_02";
			buyableShopOfferBalancingData10.AssetId = string.Empty;
			buyableShopOfferBalancingData10.LocaId = "bird_class_druid_adv";
			buyableShopOfferBalancingData10.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData10.PopupLoca = "unlock_t2eliteskins";
			buyableShopOfferBalancingData10.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitedruid",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_white",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_druid",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData10.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_druid",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitedruid",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData10.HideUnlessOnSale = true;
			balancingDataList11.Add(buyableShopOfferBalancingData10);
			ICollection<BuyableShopOfferBalancingData> balancingDataList12 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData11 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData11.Level = 1;
			Dictionary<string, int> dictionary11 = new Dictionary<string, int>();
			dictionary11["skin_eliteprincess"] = 1;
			buyableShopOfferBalancingData11.OfferContents = dictionary11;
			buyableShopOfferBalancingData11.SortPriority = 1;
			buyableShopOfferBalancingData11.SlotId = 303;
			buyableShopOfferBalancingData11.Category = "shop_global_classes";
			buyableShopOfferBalancingData11.NameId = "offer_class_white_skin_eliteprincess_02";
			buyableShopOfferBalancingData11.AssetId = string.Empty;
			buyableShopOfferBalancingData11.LocaId = "bird_class_princess_adv";
			buyableShopOfferBalancingData11.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData11.PopupLoca = "unlock_t3eliteskins";
			buyableShopOfferBalancingData11.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliteprincess",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_white",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_princess",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData11.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_princess",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliteprincess",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData11.HideUnlessOnSale = true;
			balancingDataList12.Add(buyableShopOfferBalancingData11);
			ICollection<BuyableShopOfferBalancingData> balancingDataList13 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData12 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData12.Level = 1;
			Dictionary<string, int> dictionary12 = new Dictionary<string, int>();
			dictionary12["skin_elitepriest"] = 1;
			buyableShopOfferBalancingData12.OfferContents = dictionary12;
			buyableShopOfferBalancingData12.SortPriority = 1;
			buyableShopOfferBalancingData12.SlotId = 304;
			buyableShopOfferBalancingData12.Category = "shop_global_classes";
			buyableShopOfferBalancingData12.NameId = "offer_class_white_skin_elitepriest_02";
			buyableShopOfferBalancingData12.AssetId = string.Empty;
			buyableShopOfferBalancingData12.LocaId = "bird_class_priest_adv";
			buyableShopOfferBalancingData12.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData12.PopupLoca = "unlock_t4eliteskins";
			buyableShopOfferBalancingData12.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitepriest",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_white",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_priest",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData12.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_priest",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitepriest",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData12.HideUnlessOnSale = true;
			balancingDataList13.Add(buyableShopOfferBalancingData12);
			ICollection<BuyableShopOfferBalancingData> balancingDataList14 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData13 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData13.Level = 1;
			Dictionary<string, int> dictionary13 = new Dictionary<string, int>();
			dictionary13["skin_elitepirate"] = 1;
			buyableShopOfferBalancingData13.OfferContents = dictionary13;
			buyableShopOfferBalancingData13.SortPriority = 1;
			buyableShopOfferBalancingData13.SlotId = 401;
			buyableShopOfferBalancingData13.Category = "shop_global_classes";
			buyableShopOfferBalancingData13.NameId = "offer_class_black_skin_pirate_02";
			buyableShopOfferBalancingData13.AssetId = string.Empty;
			buyableShopOfferBalancingData13.LocaId = "bird_class_pirate_adv";
			buyableShopOfferBalancingData13.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData13.PopupLoca = "unlock_premiumeliteskins";
			buyableShopOfferBalancingData13.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitepirate",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_black",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_pirate",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData13.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_pirate",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitepirate",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData13.HideUnlessOnSale = true;
			balancingDataList14.Add(buyableShopOfferBalancingData13);
			ICollection<BuyableShopOfferBalancingData> balancingDataList15 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData14 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData14.Level = 1;
			Dictionary<string, int> dictionary14 = new Dictionary<string, int>();
			dictionary14["skin_elitecannoneer"] = 1;
			buyableShopOfferBalancingData14.OfferContents = dictionary14;
			buyableShopOfferBalancingData14.SortPriority = 1;
			buyableShopOfferBalancingData14.SlotId = 402;
			buyableShopOfferBalancingData14.Category = "shop_global_classes";
			buyableShopOfferBalancingData14.NameId = "offer_class_black_skin_elitecannoneer_02";
			buyableShopOfferBalancingData14.AssetId = string.Empty;
			buyableShopOfferBalancingData14.LocaId = "bird_class_cannoneer_adv";
			buyableShopOfferBalancingData14.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData14.PopupLoca = "unlock_t2eliteskins";
			buyableShopOfferBalancingData14.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitecannoneer",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_black",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_cannoneer",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData14.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_cannoneer",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitecannoneer",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData14.HideUnlessOnSale = true;
			balancingDataList15.Add(buyableShopOfferBalancingData14);
			ICollection<BuyableShopOfferBalancingData> balancingDataList16 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData15 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData15.Level = 1;
			Dictionary<string, int> dictionary15 = new Dictionary<string, int>();
			dictionary15["skin_eliteberserk"] = 1;
			buyableShopOfferBalancingData15.OfferContents = dictionary15;
			buyableShopOfferBalancingData15.SortPriority = 1;
			buyableShopOfferBalancingData15.SlotId = 403;
			buyableShopOfferBalancingData15.Category = "shop_global_classes";
			buyableShopOfferBalancingData15.NameId = "offer_class_black_skin_eliteberserk_02";
			buyableShopOfferBalancingData15.AssetId = string.Empty;
			buyableShopOfferBalancingData15.LocaId = "bird_class_berserk_adv";
			buyableShopOfferBalancingData15.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData15.PopupLoca = "unlock_t3eliteskins";
			buyableShopOfferBalancingData15.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliteberserk",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_black",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_berserk",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData15.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_berserk",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliteberserk",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData15.HideUnlessOnSale = true;
			balancingDataList16.Add(buyableShopOfferBalancingData15);
			ICollection<BuyableShopOfferBalancingData> balancingDataList17 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData16 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData16.Level = 1;
			Dictionary<string, int> dictionary16 = new Dictionary<string, int>();
			dictionary16["skin_elitecaptn"] = 1;
			buyableShopOfferBalancingData16.OfferContents = dictionary16;
			buyableShopOfferBalancingData16.SortPriority = 1;
			buyableShopOfferBalancingData16.SlotId = 404;
			buyableShopOfferBalancingData16.Category = "shop_global_classes";
			buyableShopOfferBalancingData16.NameId = "offer_class_black_skin_elitecaptn_02";
			buyableShopOfferBalancingData16.AssetId = string.Empty;
			buyableShopOfferBalancingData16.LocaId = "bird_class_captn_adv";
			buyableShopOfferBalancingData16.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData16.PopupLoca = "unlock_t4eliteskins";
			buyableShopOfferBalancingData16.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitecaptn",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_black",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_captn",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData16.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_captn",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitecaptn",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData16.HideUnlessOnSale = true;
			balancingDataList17.Add(buyableShopOfferBalancingData16);
			ICollection<BuyableShopOfferBalancingData> balancingDataList18 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData17 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData17.Level = 1;
			Dictionary<string, int> dictionary17 = new Dictionary<string, int>();
			dictionary17["skin_elitetricksters"] = 1;
			buyableShopOfferBalancingData17.OfferContents = dictionary17;
			buyableShopOfferBalancingData17.SortPriority = 1;
			buyableShopOfferBalancingData17.SlotId = 501;
			buyableShopOfferBalancingData17.Category = "shop_global_classes";
			buyableShopOfferBalancingData17.NameId = "offer_class_blue_skin_tricksters_02";
			buyableShopOfferBalancingData17.AssetId = string.Empty;
			buyableShopOfferBalancingData17.LocaId = "bird_class_tricksters_adv";
			buyableShopOfferBalancingData17.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData17.PopupLoca = "unlock_premiumeliteskins";
			buyableShopOfferBalancingData17.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitetricksters",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_blue",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_tricksters",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData17.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_tricksters",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitetricksters",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData17.HideUnlessOnSale = true;
			balancingDataList18.Add(buyableShopOfferBalancingData17);
			ICollection<BuyableShopOfferBalancingData> balancingDataList19 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData18 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData18.Level = 1;
			Dictionary<string, int> dictionary18 = new Dictionary<string, int>();
			dictionary18["skin_eliterogues"] = 1;
			buyableShopOfferBalancingData18.OfferContents = dictionary18;
			buyableShopOfferBalancingData18.SortPriority = 1;
			buyableShopOfferBalancingData18.SlotId = 502;
			buyableShopOfferBalancingData18.Category = "shop_global_classes";
			buyableShopOfferBalancingData18.NameId = "offer_class_blue_skin_eliterogues_02";
			buyableShopOfferBalancingData18.AssetId = string.Empty;
			buyableShopOfferBalancingData18.LocaId = "bird_class_rogues_adv";
			buyableShopOfferBalancingData18.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData18.PopupLoca = "unlock_t2eliteskins";
			buyableShopOfferBalancingData18.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliterogues",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_blue",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_rogues",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData18.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_rogues",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_eliterogues",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData18.HideUnlessOnSale = true;
			balancingDataList19.Add(buyableShopOfferBalancingData18);
			ICollection<BuyableShopOfferBalancingData> balancingDataList20 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData19 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData19.Level = 1;
			Dictionary<string, int> dictionary19 = new Dictionary<string, int>();
			dictionary19["skin_elitemarksmen"] = 1;
			buyableShopOfferBalancingData19.OfferContents = dictionary19;
			buyableShopOfferBalancingData19.SortPriority = 1;
			buyableShopOfferBalancingData19.SlotId = 503;
			buyableShopOfferBalancingData19.Category = "shop_global_classes";
			buyableShopOfferBalancingData19.NameId = "offer_class_blue_skin_elitemarksmen_02";
			buyableShopOfferBalancingData19.AssetId = string.Empty;
			buyableShopOfferBalancingData19.LocaId = "bird_class_marksmen_adv";
			buyableShopOfferBalancingData19.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData19.PopupLoca = "unlock_t3eliteskins";
			buyableShopOfferBalancingData19.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitemarksmen",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_blue",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_marksmen",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData19.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_rogues",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitemarksmen",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData19.HideUnlessOnSale = true;
			balancingDataList20.Add(buyableShopOfferBalancingData19);
			ICollection<BuyableShopOfferBalancingData> balancingDataList21 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData20 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData20.Level = 1;
			Dictionary<string, int> dictionary20 = new Dictionary<string, int>();
			dictionary20["skin_elitespies"] = 1;
			buyableShopOfferBalancingData20.OfferContents = dictionary20;
			buyableShopOfferBalancingData20.SortPriority = 1;
			buyableShopOfferBalancingData20.SlotId = 504;
			buyableShopOfferBalancingData20.Category = "shop_global_classes";
			buyableShopOfferBalancingData20.NameId = "offer_class_blue_skin_elitespies_02";
			buyableShopOfferBalancingData20.AssetId = string.Empty;
			buyableShopOfferBalancingData20.LocaId = "bird_class_spies_adv";
			buyableShopOfferBalancingData20.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData20.PopupLoca = "unlock_t4eliteskins";
			buyableShopOfferBalancingData20.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitespies",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_blue",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_spies",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData20.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_spies",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitespies",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData20.HideUnlessOnSale = true;
			balancingDataList21.Add(buyableShopOfferBalancingData20);
			ICollection<BuyableShopOfferBalancingData> balancingDataList22 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData21 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData21.Level = 1;
			Dictionary<string, int> dictionary21 = new Dictionary<string, int>();
			dictionary21["skin_elitetricksters"] = 1;
			buyableShopOfferBalancingData21.OfferContents = dictionary21;
			buyableShopOfferBalancingData21.SortPriority = 3;
			buyableShopOfferBalancingData21.SlotId = 1;
			buyableShopOfferBalancingData21.Category = "shop_global_classes";
			buyableShopOfferBalancingData21.NameId = "offer_class_skin_container_02";
			buyableShopOfferBalancingData21.AssetId = string.Empty;
			buyableShopOfferBalancingData21.LocaId = "offer_class_fake";
			buyableShopOfferBalancingData21.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData21.PopupLoca = "unlock_t2eliteskins";
			buyableShopOfferBalancingData21.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitetricksters",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_white",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_cleric",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData21.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData21.HideUnlessOnSale = true;
			balancingDataList22.Add(buyableShopOfferBalancingData21);
			ICollection<BuyableShopOfferBalancingData> balancingDataList23 = DIContainerBalancing.m_service.GetBalancingDataList<BuyableShopOfferBalancingData>();
			BuyableShopOfferBalancingData buyableShopOfferBalancingData22 = new BuyableShopOfferBalancingData();
			buyableShopOfferBalancingData22.Level = 1;
			Dictionary<string, int> dictionary22 = new Dictionary<string, int>();
			dictionary22["skin_elitetricksters"] = 1;
			buyableShopOfferBalancingData22.OfferContents = dictionary22;
			buyableShopOfferBalancingData22.SortPriority = 3;
			buyableShopOfferBalancingData22.SlotId = 1;
			buyableShopOfferBalancingData22.Category = "shop_global_classes";
			buyableShopOfferBalancingData22.NameId = "offer_class_skin_fake";
			buyableShopOfferBalancingData22.AssetId = string.Empty;
			buyableShopOfferBalancingData22.LocaId = "offer_class_fake";
			buyableShopOfferBalancingData22.AtlasNameId = string.Empty;
			buyableShopOfferBalancingData22.PopupLoca = "unlock_skintest";
			buyableShopOfferBalancingData22.BuyRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.NotHaveClass,
					NameId = "skin_elitetricksters",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveBird,
					NameId = "bird_white",
					Value = 1f
				},
				new Requirement
				{
					RequirementType = RequirementType.PayItem,
					NameId = "lucky_coin",
					Value = 90f
				},
				new Requirement
				{
					RequirementType = RequirementType.HaveClass,
					NameId = "class_cleric",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData22.ShowRequirements = new List<Requirement>
			{
				new Requirement
				{
					RequirementType = RequirementType.HaveItem,
					NameId = "unlock_skins",
					Value = 1f
				}
			};
			buyableShopOfferBalancingData22.HideUnlessOnSale = true;
			balancingDataList23.Add(buyableShopOfferBalancingData22);
			DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
			{
				// add birdday sale where all elite classes become buyable
				DIContainerBalancing.m_service.GetBalancingDataList<SalesManagerBalancingData>().Add(new SalesManagerBalancingData
				{
					NameId = "birdday_" + trustedTime.Year,
					SaleDetails = new List<SaleItemDetails>
					{
						new SaleItemDetails
						{
							SubjectId = "offer_class_red_skin_knight_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_red_skin_eliteguardian_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_red_skin_elitesamurai_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_red_skin_eliteavenger_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_yellow_skin_mage_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_yellow_skin_elitelightningbird_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_yellow_skin_eliterainbird_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_yellow_skin_wizard_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_white_skin_cleric_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_white_skin_elitedruid_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_white_skin_eliteprincess_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_white_skin_elitepriest_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_black_skin_pirate_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_black_skin_elitecannoneer_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_black_skin_eliteberserk_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_black_skin_elitecaptn_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_blue_skin_tricksters_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_blue_skin_eliterogues_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_blue_skin_elitemarksmen_02",
							SaleParameter = SaleParameter.Special
						},
						new SaleItemDetails
						{
							SubjectId = "offer_class_blue_skin_elitespies_02",
							SaleParameter = SaleParameter.Special
						}
					},
					StartTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 10, 8, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds),
					EndTime = Convert.ToUInt32(new DateTime(trustedTime.Year, 12, 11, 20, 0, 0).Subtract(new DateTime(1970, 1, 1)).TotalSeconds),
					Requirements = new List<Requirement>
					{
						new Requirement
						{
							RequirementType = RequirementType.HaveUnlockedHotpsot,
							NameId = "hotspot_018_battleground",
							Value = 1f
						}
					},
					SortPriority = 1,
					PopupIconId = "ShopOffer_SkinIntroduction",
					PopupAtlasId = "ShopIconElements4",
					CheckoutCategory = "shop_global_classes"
				});
			});
		}
		catch (Exception ex)
		{
			DebugLog.Error(ex.ToString());
			throw ex;
		}
		DIContainerInfrastructure.GetCurrentPlayer().RemoveInvalidTrophyFix();
		callback(m_eventBalancingService);
		return true;
	}
}
