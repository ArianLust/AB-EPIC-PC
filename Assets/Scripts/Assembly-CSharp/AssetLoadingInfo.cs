using System;
using UnityEngine;

[Serializable]
public class AssetLoadingInfo
{
	public string LoadPath = string.Empty;

	public DateTime LastRequestTime;

	public bool loaded;

	public LoadingType AssetLoadingType;

	public UnityEngine.Object Asset;
}
