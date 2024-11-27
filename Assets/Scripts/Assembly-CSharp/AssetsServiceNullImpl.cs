using System;
using System.Collections.Generic;
using ABH.Shared.Models;

public class AssetsServiceNullImpl : IAssetsService
{
	public IAssetsService Initialize()
	{
		return this;
	}

	public void Load(string file, Action<string> callback, Action<float> onupdate, Action<bool> onslowprogress)
	{
		callback(null);
	}

	public void Load(string[] files, Action<Dictionary<string, string>> onSuccess, Action<string[], int> onError, Action<Dictionary<string, string>, string[], double, double> onProgress)
	{
		if (onSuccess != null)
		{
			onSuccess(new Dictionary<string, string>());
		}
	}

	public void LoadMetadata(string[] files, Action<Dictionary<string, AssetInfo>> onSuccess, Action<string[], int> onError)
	{
	}

	public void LoadMetadata(Action<Dictionary<string, AssetInfo>> onSuccess, Action<string[], int> onError)
	{
	}

	public void LoadAllNewAssets(Action<Dictionary<string, string>> onSuccess, Action<string[], int> onError, Action<Dictionary<string, string>, string[], double, double> onProgress, string onlyWithPrefix, HashSet<string> except, Func<long, bool> freeSpaceCheck)
	{
	}

	public bool NeedToDownloadAsset(string assetName)
	{
		return false;
	}

	public void ReloadBalancingIfneeded(Action onSuccess)
	{
		onSuccess();
	}
}
