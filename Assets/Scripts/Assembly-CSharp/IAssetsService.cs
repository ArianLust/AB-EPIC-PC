using System;
using System.Collections.Generic;
using ABH.Shared.Models;

public interface IAssetsService
{
	IAssetsService Initialize();

	void Load(string[] files, Action<Dictionary<string, string>> onSuccess, Action<string[], int> onError, Action<Dictionary<string, string>, string[], double, double> onProgress);

	void Load(string file, Action<string> callback, Action<float> onupdate, Action<bool> onslowprogres = null);

	void LoadMetadata(string[] files, Action<Dictionary<string, AssetInfo>> onSuccess, Action<string[], int> onError);

	void LoadMetadata(Action<Dictionary<string, AssetInfo>> onSuccess, Action<string[], int> onError);

	void LoadAllNewAssets(Action<Dictionary<string, string>> onSuccess, Action<string[], int> onError, Action<Dictionary<string, string>, string[], double, double> onProgress, string onlyWithPrefix, HashSet<string> exclude, Func<long, bool> freeSpaceCheck);

	bool NeedToDownloadAsset(string assetName);

	void ReloadBalancingIfneeded(Action onSuccess);
}
