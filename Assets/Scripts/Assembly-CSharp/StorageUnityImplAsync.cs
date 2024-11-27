using System;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class StorageUnityImplAsync : IStorageServiceAsync
{
	public bool SetInt(string key, int value, Action<bool> onSet)
	{
		PlayerPrefs.SetInt(key, value);
		Save(delegate(bool success)
		{
			if (onSet != null)
			{
				onSet(success);
			}
		});
		return true;
	}

	public bool GetInt(string key, Action<int> onValueReceived)
	{
		int @int = PlayerPrefs.GetInt(key);
		if (onValueReceived != null)
		{
			onValueReceived(@int);
		}
		return true;
	}

	public bool GetInt(string key, int standardValue, Action<int> onValueReceived)
	{
		int @int = PlayerPrefs.GetInt(key, standardValue);
		if (onValueReceived != null)
		{
			onValueReceived(@int);
		}
		return true;
	}

	public bool SetFloat(string key, float value, Action<bool> onSet)
	{
		PlayerPrefs.SetFloat(key, value);
		Save(delegate(bool success)
		{
			if (onSet != null)
			{
				onSet(success);
			}
		});
		return true;
	}

	public bool GetFloat(string key, Action<float> onValueReceived)
	{
		float @float = PlayerPrefs.GetFloat(key);
		if (onValueReceived != null)
		{
			onValueReceived(@float);
		}
		return true;
	}

	public bool GetFloat(string key, float standardValue, Action<float> onValueReceived)
	{
		float @float = PlayerPrefs.GetFloat(key, standardValue);
		if (onValueReceived != null)
		{
			onValueReceived(@float);
		}
		return true;
	}

	public bool SetString(string key, string value, Action<bool> onSet)
	{
		PlayerPrefs.SetString(key, value);
		Save(delegate(bool success)
		{
			if (onSet != null)
			{
				onSet(success);
			}
		});
		return true;
	}

	public bool GetString(string key, Action<string> onValueReceived)
	{
		string @string = PlayerPrefs.GetString(key);
		if (onValueReceived != null)
		{
			onValueReceived(@string);
		}
		return true;
	}

	public bool GetString(string key, string standardValue, Action<string> onValueReceived)
	{
		string @string = PlayerPrefs.GetString(key, standardValue);
		if (onValueReceived != null)
		{
			onValueReceived(@string);
		}
		return true;
	}

	public bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	public bool DeleteKey(string key, Action<bool> onDeleted)
	{
		PlayerPrefs.DeleteKey(key);
		Save(delegate(bool success)
		{
			if (onDeleted != null)
			{
				onDeleted(success);
			}
		});
		return true;
	}

	public bool DeleteAll(Action<bool> onDeleted)
	{
		PlayerPrefs.DeleteAll();
		Save(delegate(bool success)
		{
			if (onDeleted != null)
			{
				onDeleted(success);
			}
		});
		return true;
	}

	public bool Save(Action<bool> onSaved)
	{
		PlayerPrefs.Save();
		if (onSaved != null)
		{
			onSaved(true);
		}
		return true;
	}

	public bool GetBytes(string key, byte[] standardValue, Action<byte[]> onValueReceived)
	{
		throw new NotImplementedException();
	}

	public bool SetBytes(string key, byte[] value, Action<bool> onSet)
	{
		throw new NotImplementedException();
	}
}
