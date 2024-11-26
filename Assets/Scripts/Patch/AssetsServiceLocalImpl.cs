using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ABH.GameDatas;
using ABH.Shared.Models;
using Interfaces.GameClient;
using UnityEngine;

public class AssetsServiceLocalImpl : IAssetsService
{
	public void Initialize()
	{
	}

	public void Load(string file, Action<string> callback, Action<float> onupdate, Action<bool> onSlowProgress = null)
	{
		file = ReplaceUnmappableCharacters(file);
		AssetInfo assetInfoFor = DIContainerInfrastructure.GetAssetData().GetAssetInfoFor(file);
		if (assetInfoFor == null || NeedToDownloadAsset(file))
		{
			LoadMetadata(new string[1] { file }, delegate(Dictionary<string, AssetInfo> infos)
			{
				DoLoadFromSkynest(infos, delegate(Dictionary<string, string> d)
				{
					callback(d.Values.First());
				}, delegate
				{
					callback(null);
				}, delegate(Dictionary<string, string> downloaded, string[] loading, double totalToDownload, double nowDownloaded)
				{
					onupdate((float)(nowDownloaded / totalToDownload));
				}, onSlowProgress);
			}, delegate
			{
				callback(null);
			});
		}
		else
		{
			callback(assetInfoFor.FilePath);
		}
	}

	public void Load(string[] files, Action<Dictionary<string, string>> onSuccess, Action<string[], int> onError, Action<Dictionary<string, string>, string[], double, double> onProgress)
	{
		if (onSuccess == null)
		{
			DebugLog.Error("[AssetsServiceLocalImpl] Load: Cannot load assets as onSuccess handler is null.");
		}
		else if (onError == null)
		{
			DebugLog.Error("[AssetsServiceLocalImpl] Load: Cannot load assets as onError handler is null.");
		}
		else if (onProgress == null)
		{
			DebugLog.Error("[AssetsServiceLocalImpl] Load: Cannot load assets as onProgress is null.");
		}
		else if (files == null)
		{
			DebugLog.Error("[AssetsServiceLocalImpl] Load: Cannot load assets as files are null.");
		}
		else
		{
			LoadIfNotCached(files, onSuccess, onError, onProgress);
		}
	}

	private void LoadIfNotCached(string[] files, Action<Dictionary<string, string>> onSuccess, Action<string[], int> onError, Action<Dictionary<string, string>, string[], double, double> onProgress)
	{
		DebugLog.Log("[AssetsServiceLocalImpl] Load: Start loading assets: " + string.Join(",", files));
		AssetData assetData = DIContainerInfrastructure.GetAssetData();
		LoadMetadata(files, delegate(Dictionary<string, AssetInfo> metadataDict)
		{
			List<string> list = new List<string>();
			foreach (string key in metadataDict.Keys)
			{
				AssetInfo assetInfoFor = assetData.GetAssetInfoFor(key);
				if (assetInfoFor != null && assetInfoFor.Hash == metadataDict[key].Hash)
				{
					DebugLog.Log("[AssetsServiceLocalImpl] Skipping " + key + " because it is already in the cache");
				}
				else
				{
					list.Add(key);
				}
			}
			if (list.Count == 0)
			{
				DebugLog.Log("[AssetsServiceLocalImpl] Not loading any asset because they are all inside the cache: " + string.Join(",", files));
				onProgress(new Dictionary<string, string>(), new string[1] { "all assets up to date" }, 1.0, 1.0);
				onSuccess(files.ToDictionary((string s) => s, (string s) => assetData.GetAssetInfoFor(s).FilePath));
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item in list)
				{
					stringBuilder.AppendLine("Need to load from remote: " + item);
				}
				DebugLog.Log("[AssetsServiceLocalImpl] " + stringBuilder.ToString());
				DoLoadFromSkynest(metadataDict, onSuccess, onError, onProgress, null);
			}
		}, delegate(string[] fileList, int error)
		{
			DebugLog.Error("[AssetsServiceLocalImpl] Loading assets metadata error (code " + error + ") for: " + string.Join(",", fileList));
		});
	}

	public bool NeedToDownloadAsset(string assetName)
	{
		if (!DIContainerInfrastructure.GetSkynestMetaDataCache().IsInitialized)
		{
			DebugLog.Log("[AssetsServiceLocalImpl] NeedToDownloadAsset: " + assetName + " must be downloaded because the MetaDataCache is not yet initialized");
			return true;
		}
		AssetInfo assetInfoFor = DIContainerInfrastructure.GetAssetData().GetAssetInfoFor(assetName);
		AssetInfo assetInfo = DIContainerInfrastructure.GetSkynestMetaDataCache().GetSkynestAssetInfoFor(assetName).ToABHAssetInfo();
		return assetInfoFor == null || string.IsNullOrEmpty(assetInfo.Hash) || !File.Exists(assetInfoFor.FilePath);
	}

	private void DoLoadFromSkynest(Dictionary<string, AssetInfo> metadataDict, Action<Dictionary<string, string>> onSuccess, Action<string[], int> onError, Action<Dictionary<string, string>, string[], double, double> onProgress, Action<bool> onslowprogres)
	{
		AssetData assetData = DIContainerInfrastructure.GetAssetData();
		long totalSize = 0L;
		if (metadataDict != null && metadataDict.Count > 0)
		{
			foreach (AssetInfo value in metadataDict.Values)
			{
				if (value != null)
				{
					totalSize += value.Size;
				}
			}
		}
		double lastSize = 0.0;
		float lastTime = Time.realtimeSinceStartup;
		float startTime = lastTime;
		List<double> kbpsList = new List<double>();
		LocalAssets.Load(metadataDict.Keys.ToArray(), delegate(Dictionary<string, string> fileList)
		{
			assetData.AssetsUpdated(fileList, metadataDict.Select((KeyValuePair<string, AssetInfo> d) => new KeyValuePair<string, AssetInfo>(d.Key, d.Value)).ToDictionary((KeyValuePair<string, AssetInfo> kvp) => kvp.Key, (KeyValuePair<string, AssetInfo> kvp) => kvp.Value));
			DebugLog.Log("updated asset data, contents: " + DIContainerInfrastructure.GetAssetData().Assets.Aggregate(string.Empty, (string acc, KeyValuePair<string, AssetInfo> kvp) => string.Concat(acc, "[", kvp.Key, " => ", kvp.Value, "]")));
			if (metadataDict != null && metadataDict.Count > 0)
			{
				foreach (AssetInfo value2 in metadataDict.Values)
				{
					if (value2 != null)
					{
						value2.AssetVersion = Mathf.Min(999, value2.AssetVersion + 1);
					}
				}
			}
			assetData.Save();
			onSuccess(fileList);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string key in fileList.Keys)
			{
				stringBuilder.AppendLine("Loaded remote " + key + " to local folder " + fileList[key]);
			}
			DebugLog.Log("[AssetsServiceLocalImpl] " + stringBuilder.ToString());
		}, delegate(string[] efiles, int errorCode)
		{
			onError(efiles, errorCode);
		}, delegate(Dictionary<string, string> downloaded, string[] loading, double download, double nowDownloaded)
		{
			onProgress(downloaded, loading, totalSize, nowDownloaded);
			if (onslowprogres != null)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				float num = realtimeSinceStartup - lastTime;
				double item = (nowDownloaded - lastSize) / 1024.0 / (double)num;
				if (kbpsList.Count >= 20)
				{
					kbpsList.RemoveAt(0);
				}
				kbpsList.Add(item);
				double num2 = kbpsList.Sum() / (double)kbpsList.Count;
				onslowprogres(realtimeSinceStartup - startTime > 5f && num2 < 45.0);
				lastTime = realtimeSinceStartup;
				lastSize = nowDownloaded;
			}
		});
	}

	public void LoadMetadata(string[] files, Action<Dictionary<string, AssetInfo>> onSuccess, Action<string[], int> onError)
	{
		if (onSuccess == null)
		{
			DebugLog.Error("[AssetsServiceLocalImpl] LoadMetadata: Cannot load metadata as onSuccess handler is null.");
			return;
		}
		if (onError == null)
		{
			DebugLog.Error("[AssetsServiceLocalImpl] LoadMetadata: Cannot load metadata as onError handler is null.");
			return;
		}
		if (files == null)
		{
			DebugLog.Error("[AssetsServiceLocalImpl] LoadMetadata: Cannot load metadata as files are null.");
			return;
		}
		DebugLog.Log(GetType(), "Removing invalid characters from file list and replacing them with valid ones.");
		for (int i = 0; i < files.Length; i++)
		{
			string fixedFileName = ReplaceUnmappableCharacters(files[i]);
			files[i] = fixedFileName;
		}
		
		DebugLog.Log("[AssetsServiceLocalImpl] LoadMetadata: Start loading metadata for: " + string.Join(", ", files));
		
		Dictionary<string, SkynestAssets.AssetInfo> obj = new Dictionary<string, SkynestAssets.AssetInfo>();
		foreach (var metadata in DIContainerInfrastructure.GetSkynestMetaDataCache().AllMetaData)
		{
			DebugLog.Log(GetType(), "Checking if file list contains metadata key. File list: " + string.Join(", ", files) + ", Metadata: " + metadata);
			
			if (files.Contains(metadata.Key))
			{
				DebugLog.Log(GetType(), "File list contains: " + metadata.Key);
				obj.Add(metadata.Key, metadata.Value);
			}
			else
			{
				DebugLog.Log(GetType(), "File list does not contain metadata key. Metadata: " + metadata);
			}
		}
		Dictionary<string, AssetInfo> result = obj.ToABhAssetInfos();
		onSuccess(result);
	}

	public string ReplaceUnmappableCharacters(string localizedString)
	{
		if (string.IsNullOrEmpty(localizedString))
		{
			return localizedString;
		}
		DebugLog.Log(GetType(), "[ReplaceUnmappableCharacters] Old string: " + localizedString);
		localizedString = localizedString
			.Replace("æ", "e")
			.Replace("ø", "oe")
			.Replace("å", "aa")
			.Replace("æ", "ae")
			.Replace("Æ", "Ae")
			.Replace("Ø", "Oe")
			.Replace("Ǿ", "Oe")
			.Replace("ǿ", "oe")
			.Replace("ǽ", "ae")
			.Replace("Ǽ", "Ae")
			.Replace("Å", "Aa")
			.Replace("ı", "i");
		DebugLog.Log(GetType(), "[ReplaceUnmappableCharacters] New string: " + localizedString);
		return localizedString;
	}
	
	public void LoadMetadata(Action<Dictionary<string, AssetInfo>> onSuccess, Action<string[], int> onError)
	{
		DebugLog.Log("[AssetsServiceLocalImpl] LoadMetadata: Start loading metadata for all");
		if (onSuccess == null)
		{
			DebugLog.Error("[AssetsServiceLocalImpl] LoadMetadata: Cannot load metadata as onSuccess handler is null.");
		}
		else if (onError == null)
		{
			DebugLog.Error("[AssetsServiceLocalImpl] LoadMetadata: Cannot load metadata as onError handler is null.");
		}
		else
		{
			onSuccess(DIContainerInfrastructure.GetSkynestMetaDataCache().AllMetaData.ToABhAssetInfos());
		}
	}

	public void LoadAllNewAssets(Action<Dictionary<string, string>> onSuccess, Action<string[], int> onError, Action<Dictionary<string, string>, string[], double, double> onProgress, string onlyWithPrefix, HashSet<string> except, Func<long, bool> freeSpaceCheck)
	{
		Dictionary<string, AssetInfo> dictionary = DIContainerInfrastructure.GetSkynestMetaDataCache().AllMetaData.ToABhAssetInfos();
		if (dictionary == null || dictionary.Count == 0)
		{
			DebugLog.Warn("[AssetsServiceLocalImpl] LoadAllNewAssets: No remote assets found!");
			onSuccess(new Dictionary<string, string>());
			return;
		}
		Dictionary<string, AssetInfo> dictionary2 = new Dictionary<string, AssetInfo>(dictionary);
		foreach (string key in dictionary.Keys)
		{
			bool flag = false;
			if (except != null && except.Contains(key))
			{
				flag = true;
				DebugLog.Log("[AssetsServiceLocalImpl] LoadAllNewAssets: skipping " + key + " because it is on the blacklist which was provided.");
			}
			if ((flag || (!string.IsNullOrEmpty(onlyWithPrefix) && !key.StartsWith(onlyWithPrefix))) && !flag)
			{
				DebugLog.Log("[AssetsServiceLocalImpl] LoadAllNewAssets: skipping " + key + " because it did not start with the prefix \"" + onlyWithPrefix + "\"");
				flag = true;
			}
			if ((flag || !NeedToDownloadAsset(key)) && !flag)
			{
				DebugLog.Log("[AssetsServiceLocalImpl] LoadAllNewAssets: skipping " + key + " because the version does not match");
				flag = true;
			}
			if (flag)
			{
				dictionary2.Remove(key);
			}
		}
		long num = dictionary2.Aggregate(0L, (long acc, KeyValuePair<string, AssetInfo> kvp) => acc + kvp.Value.Size);
		if (freeSpaceCheck != null && !freeSpaceCheck(num))
		{
			DebugLog.Log("[AssetsServiceLocalImpl] free space check orders stop. Not loading the assets. downloadSize = " + num);
			return;
		}
		DebugLog.Log("[AssetsServiceLocalImpl] Now loading " + string.Join(",", dictionary2.Keys.ToArray()));
		if (dictionary2.Count > 0)
		{
			DoLoadFromSkynest(dictionary2, onSuccess, onError, onProgress, null);
		}
		else
		{
			DebugLog.Log("[AssetsServiceLocalImpl] LoadAllNewAssets: No new assets found which must be loaded.");
		}
	}

    IAssetsService IAssetsService.Initialize()
    {
        throw new NotImplementedException();
    }

	public void ReloadBalancingIfneeded(Action onSuccess)
	{
		//DebugLog.Log(GetType(), "Checking if new Balancing is available...");
		//if (onSuccess == null)
		//{
		//	DebugLog.Error(GetType(), "Load: Cannot load assets as onSuccess handler is null.");
		//	return;
		//}
		//PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		//if (currentPlayer != null && currentPlayer.Data.Experience == 0f && currentPlayer.Data.Level == 1 && !currentPlayer.Data.IsUserConverted)
		//{
		//	DebugLog.Error(GetType(), "ReloadBalancingIfNeeded: Balancing reload prohibited because user data not yet saved");
		//	return;
		//}
		//string balancingName = DIContainerBalancing.BalancingDataAssetFilename;
		//string eventbalancingName = DIContainerBalancing.EventBalancingDataAssetFilename;
		//List<string> list = new List<string>();
		//list.Add(balancingName);
		//list.Add(eventbalancingName);
		//list.Add(DIContainerInfrastructure.GetTargetBuildGroup() + "_shopiconatlasassetprovider.assetbundle");
		//list.Add(DIContainerInfrastructure.GetTargetBuildGroup() + "_" + DIContainerInfrastructure.GetStartupLocaService().CurrentLanguageKey + ".bytes");
		//List<string> assetList = list;
		//DebugLog.Log(GetType(), "Balancing name:" + balancingName);
		//AssetInfo assetInfoFor = DIContainerInfrastructure.GetAssetData().GetAssetInfoFor(balancingName);
		//string currentMd5Checksum = ((assetInfoFor == null) ? null : assetInfoFor.Checksum);
		//AssetInfo assetInfoFor2 = DIContainerInfrastructure.GetAssetData().GetAssetInfoFor(eventbalancingName);
		//string currentMd5EventChecksum = ((assetInfoFor2 == null) ? null : assetInfoFor2.Checksum);
		//LocalAssets.Load(assetList, delegate (Dictionary<string, AssetInfo> assets)
		//{
		//	UpdateAssetDatas(assets);
		//	string text = currentMd5Checksum;
		//	string text2 = currentMd5EventChecksum;
		//	foreach (string key in assets.Keys)
		//	{
		//		if (key == balancingName)
		//		{
		//			text = DIContainerInfrastructure.GetAssetData().GetAssetInfoFor(key).Checksum;
		//		}
		//		else if (key == eventbalancingName)
		//		{
		//			text2 = DIContainerInfrastructure.GetAssetData().GetAssetInfoFor(key).Checksum;
		//		}
		//	}
		//	if (!string.IsNullOrEmpty(currentMd5Checksum) && text != currentMd5Checksum)
		//	{
		//		onSuccess();
		//	}
		//	else if (!string.IsNullOrEmpty(currentMd5EventChecksum) && text2 != currentMd5EventChecksum)
		//	{
		//		onSuccess();
		//	}
		//}, DummyCallback1, DummyCallback2);
	}

    private void DummyCallback2(Dictionary<string, string> downloaded, List<string> loading, double totalToDownload, double nowDownloaded)
    {
    }

    private void DummyCallback1(List<string> assetList, List<string> assetsMissing, Rcs.Assets.ErrorCode status, string message)
    {
    }

    private void UpdateAssetDatas(Dictionary<string, string> assets)
    {
    }
}
