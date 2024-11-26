using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.Shared.Models;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class ProtobufSerializedSkynestLocaLoaderImpl : ILocaLoader
{
	private string m_resourceFolderPath = "Loca/";

	private ISerializer m_binarySerializer;

	private string EditorLocaResourcesPath
	{
		get
		{
			return m_resourceFolderPath;
		}
	}

	public bool IsAsynch
	{
		get
		{
			return true;
		}
	}

	[method: MethodImpl(32)]
	public event Action<string> LocaChanged;

	public ILocaLoader InjectLocaAsset(List<TextAsset> textAsset)
	{
		return this;
	}

	public ProtobufSerializedSkynestLocaLoaderImpl Init(ISerializer binarySerializer)
	{
		m_binarySerializer = binarySerializer;
		return this;
	}

	public IEnumerator LoadAsync(string language, Action<Dictionary<string, string>> onFinished)
	{
		DebugLog.Log(GetType(), "LoadAsync");
		if (m_binarySerializer == null)
		{
			DebugLog.Error(GetType(), "Binary serializer not set in " + GetType().Name + "! Please set it.");
			yield break;
		}
		byte[] bytes2 = null;
		AssetInfo assetInfo = GetLocaAssetInfo(language);
		if (assetInfo != null)
		{
			if (assetInfo.ClientVersion == "FALLBACK")
			{
				TextAsset locaResourceAsset = Resources.Load("Loca/" + language, typeof(TextAsset)) as TextAsset;
				if (locaResourceAsset != null)
				{
					bytes2 = locaResourceAsset.bytes;
				}
				else
				{
					DebugLog.Error(GetType(), "Serialized loca file " + language + ".bytes not found! Not loading loca for " + language);
				}
			}
			else if (assetInfo.FileExistsCheck())
			{
				DebugLog.Log(GetType(), "LoadAsync, Starting www call to download english loca");
				using (WWW www = new WWW(assetInfo.GetFilePathWithPixedFileTripleSlashes()))
				{
					DebugLog.Log(GetType(), string.Format("LoadAsync, on www progress: '{0}'", www.progress));
					while (!www.isDone)
					{
						yield return null;
					}
					bytes2 = www.bytes;
				}
			}
			else
			{
				DebugLog.Error(GetType(), "Loca file for language " + language + " could not be found (but is needed at this point!)");
			}
		}
		else
		{
			DebugLog.Error("[ProtobufSerializedSkynestLocaLoaderImpl] No loca entry found in the asset data for language " + language);
		}
		if (bytes2 != null)
		{
			DebugLog.Log(GetType(), "Loading loca: " + language);
			bytes2 = DIContainerInfrastructure.GetCompressionService().DecompressIfNecessary(bytes2);
			SerializedLocalizedTexts deserializedLoca = m_binarySerializer.Deserialize<SerializedLocalizedTexts>(bytes2);
			onFinished(deserializedLoca.Texts);
			if (this.LocaChanged != null)
			{
				this.LocaChanged(language);
			}
		}
	}

	public Dictionary<string, string> Load(string language)
	{
		throw new NotSupportedException("[ProtobufSerializedSkynestLocaLoaderImpl] no synchronous loading possible with this implementation");
	}

	private AssetInfo GetLocaAssetInfo(string language)
	{
		AssetInfo assetInfo = DIContainerInfrastructure.GetAssetData().GetAssetInfoFor(DIContainerInfrastructure.GetTargetBuildGroup() + "_" + language);
		if (assetInfo == null)
		{
			assetInfo = new AssetInfo();
			assetInfo.FilePath = EditorLocaResourcesPath + language;
			assetInfo.Name = language;
			assetInfo.ClientVersion = "FALLBACK";
			return assetInfo;
		}
		return assetInfo;
	}
}
