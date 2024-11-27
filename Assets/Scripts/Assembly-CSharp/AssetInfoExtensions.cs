using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ABH.Shared.Models;
using Rcs;

internal static class AssetInfoExtensions
{
	public static Dictionary<string, AssetInfo> ToABhAssetInfos(this Dictionary<string, SkynestAssets.AssetInfo> assetInfoDict)
	{
		Dictionary<string, AssetInfo> dictionary = new Dictionary<string, AssetInfo>();
		if (assetInfoDict != null)
		{
			foreach (string key in assetInfoDict.Keys)
			{
				dictionary.Add(key, assetInfoDict[key].ToABHAssetInfo());
			}
		}
		return dictionary;
	}

	public static string Explain(this SkynestAssets.AssetInfo assetInfo)
	{
		return assetInfo.ToABHAssetInfo().Explain();
	}

	public static string Explain(this AssetInfo assetInfo)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Name: " + assetInfo.Name);
		stringBuilder.AppendLine("ClientVersion: " + assetInfo.ClientVersion);
		stringBuilder.AppendLine("Hash: " + assetInfo.Hash);
		stringBuilder.AppendLine("Size: " + assetInfo.Size);
		stringBuilder.AppendLine("DistributionChannel: " + assetInfo.DistributionChannel);
		stringBuilder.AppendLine("CdnURL: " + assetInfo.CdnURL);
		stringBuilder.AppendLine("Os: " + assetInfo.Os);
		stringBuilder.AppendLine("AssetVersion: " + assetInfo.AssetVersion);
		stringBuilder.AppendLine("Checksum: " + assetInfo.Checksum);
		return stringBuilder.ToString();
	}

	public static SkynestAssets.AssetInfo ToSkynetAssetInfo(this AssetInfo assetInfo)
	{
		SkynestAssets.AssetInfo info = new SkynestAssets.AssetInfo();
		info.Hash = assetInfo.Hash;
		info.Name = assetInfo.Name;
		info.Size = (uint)assetInfo.Size;
		return info;
	}

	public static AssetInfo ToABHAssetInfo(this SkynestAssets.AssetInfo assetInfo)
	{
		AssetInfo assetInfo2 = new AssetInfo();
		assetInfo2.ClientVersion = "1.0.0";
		assetInfo2.DistributionChannel = "unknown";
		assetInfo2.Hash = assetInfo.Hash ?? string.Empty;
		assetInfo2.Name = assetInfo.Name ?? "unknown";
		assetInfo2.Os = "unknown";
		assetInfo2.Size = assetInfo.Size;
		return assetInfo2;
	}

	public static string GetFilePathWithPixedFileTripleSlashes(this AssetInfo info)
	{
		if (info.FilePath.StartsWith("file:///"))
		{
			return info.FilePath;
		}
		return ((!info.FilePath.StartsWith("/")) ? "file:///" : "file://") + info.FilePath;
	}

	public static bool FileExistsCheck(this AssetInfo info)
	{
		return File.Exists(info.GetFilePathWithPixedFileTripleSlashes().Replace("file:///", string.Empty));
	}

	public static byte[] GetMD5(this AssetInfo info)
	{
		if (!info.FileExistsCheck())
		{
			return null;
		}
		using (MD5 mD = MD5.Create())
		{
			using (FileStream inputStream = File.OpenRead(info.GetFilePathWithPixedFileTripleSlashes().Replace("file:///", string.Empty)))
			{
				return mD.ComputeHash(inputStream);
			}
		}
	}

	public static bool DeletePhysical(this AssetInfo info)
	{
		if (!info.FileExistsCheck())
		{
			DebugLog.Log("AssetInfo.DeletePhysical: File not found: " + info.GetFilePathWithPixedFileTripleSlashes());
			return true;
		}
		try
		{
			File.Delete("/private" + info.FilePath);
			DebugLog.Log("AssetInfo.DeletePhysical: File delete successful!");
			return true;
		}
		catch (Exception)
		{
			DebugLog.Warn("AssetInfo.DeletePhysical: File delete FAILED: /private" + info.FilePath);
			return false;
		}
	}
}
