using System;
using UnityEngine;

[Serializable]
public class EditorAssetInfo
{
	public string NameId;

	public LoadingType AssetLoadingType;

	public string Path;

	public UnityEngine.Object AssetLink;

	public bool DeleteOnBuild;

	public string Extension;
}
