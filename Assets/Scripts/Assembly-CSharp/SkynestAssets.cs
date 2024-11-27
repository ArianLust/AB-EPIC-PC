using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

public static class SkynestAssets
{
	public struct AssetInfo
	{
		public string Name;

		public string Hash;

		public string CdnURL;

		public string Os;

		public string DistributionChannel;

		public string ClientVersion;

		public uint Size;
	}

	public delegate void LoadSuccessHandler(Dictionary<string, string> fileList);

	public delegate void LoadErrorHandler(string[] files, int errorCode);

	public delegate void LoadProgressHandler(Dictionary<string, string> downloaded, string[] loading, double totalToDownload, double nowDownloaded);

	public delegate void LoadMetadataSuccessHandler(Dictionary<string, AssetInfo> loadMetadataSuccessCallback);

	public delegate void LoadMetadataErrorHandler(string[] files, int errorCode);

}
