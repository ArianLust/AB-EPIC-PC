using System.Collections.Generic;
using UnityEngine;

public class AssetBundleCrcChecker
{
	private Dictionary<string, uint> m_assetBundleFileNameToCrc = new Dictionary<string, uint>();

	public AssetBundleCrcChecker Init()
	{
		string assetBundleCrcMapResourceFileName = DIContainerConfig.GetConstants().AssetBundleCrcMapResourceFileName;
		DebugLog.Log("Loading " + assetBundleCrcMapResourceFileName + "...");
		TextAsset textAsset = Resources.Load(assetBundleCrcMapResourceFileName.Replace(".txt", string.Empty)) as TextAsset;
		if (textAsset == null)
		{
			DebugLog.Error(assetBundleCrcMapResourceFileName + " not found!");
			return this;
		}
		string[] array = textAsset.text.Replace("\r", string.Empty).Split('\n');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(DIContainerConfig.GetConstants().AssetBundleCrcMapSeparator);
			if (array3.Length == 2)
			{
				string key = array3[0];
				uint result;
				if (uint.TryParse(array3[1], out result))
				{
					m_assetBundleFileNameToCrc.Add(key, result);
				}
			}
		}
		return this;
	}

	public uint GetBuildTimeCrcForFile(string assetbundleFilename)
	{
		uint value;
		m_assetBundleFileNameToCrc.TryGetValue(assetbundleFilename, out value);
		return value;
	}
}
