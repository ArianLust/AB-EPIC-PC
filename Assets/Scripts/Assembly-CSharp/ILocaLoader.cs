using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILocaLoader
{
	bool IsAsynch { get; }

	event Action<string> LocaChanged;

	Dictionary<string, string> Load(string language);

	IEnumerator LoadAsync(string language, Action<Dictionary<string, string>> onFinished);

	ILocaLoader InjectLocaAsset(List<TextAsset> textAsset);
}
