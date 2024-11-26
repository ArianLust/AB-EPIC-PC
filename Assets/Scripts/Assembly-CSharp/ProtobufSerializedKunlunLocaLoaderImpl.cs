using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.Shared.BalancingData.Loca;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class ProtobufSerializedKunlunLocaLoaderImpl : ILocaLoader
{
	private ISerializer m_binarySerializer;

	private string m_resourceFolderPath = "Loca/";

	private List<TextAsset> m_InjectedTexts = new List<TextAsset>();

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
			return false;
		}
	}

	[method: MethodImpl(32)]
	public event Action<string> LocaChanged;

	public ProtobufSerializedKunlunLocaLoaderImpl Init(ISerializer binarySerializer)
	{
		m_binarySerializer = binarySerializer;
		return this;
	}

	public ILocaLoader InjectLocaAsset(List<TextAsset> textAssets)
	{
		DebugLog.Log(GetType(), "InjectLocaAsset");
		m_InjectedTexts = textAssets;
		return this;
	}

	protected string GetLocaFilenameFromLanguage(string language)
	{
		return language + "LocaBalancingData";
	}

	public Dictionary<string, string> Load(string language)
	{
		if (m_binarySerializer == null)
		{
			DebugLog.Error(GetType(), "Binary serializer not set in " + GetType().Name + "! Please set it.");
			return null;
		}
		DebugLog.Log(GetType(), "Loading loca: " + language);
		if (m_InjectedTexts.Count <= 0)
		{
			string path = EditorLocaResourcesPath + GetLocaFilenameFromLanguage(language);
			TextAsset textAsset = Resources.Load(path, typeof(TextAsset)) as TextAsset;
			if (textAsset == null)
			{
				DebugLog.Error(GetType(), "Serialized loca file " + language + ".bytes not found! Not loading loca for " + language);
				return null;
			}
			m_InjectedTexts.Add(textAsset);
		}
		if (m_InjectedTexts.Count <= 0)
		{
			DebugLog.Error(GetType(), "Serialized loca file " + language + ".bytes not found! Not loading loca for " + language);
			return null;
		}
		TextAsset textAsset2 = null;
		foreach (TextAsset injectedText in m_InjectedTexts)
		{
			if (injectedText.name.StartsWith(language))
			{
				DebugLog.Log(GetType(), "Found matching loca: " + language);
				textAsset2 = injectedText;
			}
		}
		if (textAsset2 == null || textAsset2.bytes == null)
		{
			DebugLog.Error(GetType(), "No serializedText found!");
			return null;
		}
		byte[] bytes = DIContainerInfrastructure.GetCompressionService().DecompressIfNecessary(textAsset2.bytes);
		IList<LocaBalancingDataBase> list = m_binarySerializer.Deserialize<IList<LocaBalancingDataBase>>(bytes);
		foreach (TextAsset injectedText2 in m_InjectedTexts)
		{
			Resources.UnloadAsset(injectedText2);
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (LocaBalancingDataBase item in list)
		{
			dictionary.Add(item.NameId, item.TranslatedText);
		}
		m_InjectedTexts.Clear();
		if (this.LocaChanged != null)
		{
			this.LocaChanged(language);
		}
		return dictionary;
	}

	public IEnumerator LoadAsync(string language, Action<Dictionary<string, string>> onFinished)
	{
		DebugLog.Log(GetType(), "LoadAsync loca: " + language);
		Dictionary<string, string> res = Load(language);
		if (onFinished != null)
		{
			onFinished(res);
		}
		yield break;
	}
}
