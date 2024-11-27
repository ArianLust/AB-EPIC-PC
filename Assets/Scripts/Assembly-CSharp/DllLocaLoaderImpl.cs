using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DllLocaLoaderImpl : ILocaLoader
{
	public bool IsAsynch
	{
		get
		{
			return false;
		}
	}

	[method: MethodImpl(32)]
	public event Action<string> LocaChanged;

	public Dictionary<string, string> Load(string language)
	{
		Type type = Type.GetType("Chimera.AngryBirdsHeroes.Localization." + language + ",Chimera.AngryBirdsHeroes.Localization");
		FieldInfo field = type.GetField("Texts", BindingFlags.Static | BindingFlags.Public);
		Dictionary<string, string> result = (Dictionary<string, string>)field.GetValue(null);
		if (this.LocaChanged != null)
		{
			this.LocaChanged(language);
		}
		return result;
	}

	public IEnumerator LoadAsync(string language, Action<Dictionary<string, string>> onFinished)
	{
		yield return null;
		Dictionary<string, string> res = Load(language);
		onFinished(res);
	}

	public ILocaLoader InjectLocaAsset(List<TextAsset> textAsset)
	{
		return this;
	}
}
