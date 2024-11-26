using System;
using System.IO;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class StorageUnityImpl : IStorageService
{
	public StorageUnityImpl()
	{
		if (!Directory.Exists(GetFile("")))
			Directory.CreateDirectory(GetFile(""));
	}
	
	private string GetFile(string key)
	{
		#if UNITY_STANDALONE_LINUX
		return Path.Combine(Application.persistentDataPath, "prefs_linux", key);
		#else
		return Path.Combine(Application.persistentDataPath, "prefs", key);
		#endif
	}

	private string GetFileData(string key)
	{
		if (!File.Exists(GetFile(key))) return null;
		return File.ReadAllText(GetFile(key));
	}

	private void SetFileData(string key, string data)
	{
		File.WriteAllText(GetFile(key), data);
	}


	public bool SetInt(string key, int value)
	{
		SetFileData(key, value.ToString());
		Save();
		return true;
	}

	public int GetInt(string key)
	{
		var data = GetFileData(key);
		if (data == null)
			return PlayerPrefs.GetInt(key);
		return int.Parse(data);
	}

	public int GetInt(string key, int standardValue)
	{
		var data = GetFileData(key);
		if (data == null)
			return PlayerPrefs.GetInt(key, standardValue);
		return int.Parse(data);
	}

	public bool SetFloat(string key, float value)
	{
		SetFileData(key, value.ToString());
		Save();
		return true;
	}

	public float GetFloat(string key)
	{
		var data = GetFileData(key);
		if (data == null)
			return PlayerPrefs.GetFloat(key);
		return float.Parse(data);
	}

	public float GetFloat(string key, float standardValue)
	{
		var data = GetFileData(key);
		if (data == null)
			return PlayerPrefs.GetFloat(key, standardValue);
		return float.Parse(data);
	}

	public bool SetString(string key, string value)
	{
		SetFileData(key, value);
		Save();
		return true;
	}

	public string GetString(string key)
	{
		return GetFileData(key) ?? PlayerPrefs.GetString(key);
	}

	public string GetString(string key, string standardValue)
	{
		return GetFileData(key) ?? PlayerPrefs.GetString(key, standardValue);
	}

	public bool HasKey(string key)
	{
		return File.Exists(GetFile(key)) || PlayerPrefs.HasKey(key);
	}

	public bool DeleteKey(string key)
	{
		File.Delete(GetFile(key));

		PlayerPrefs.DeleteKey(key);
		Save();
		return true;
	}

	public bool DeleteAll()
	{
		Directory.Delete(GetFile(""));
		Directory.CreateDirectory(GetFile(""));

		PlayerPrefs.DeleteAll();
		Save();
		return true;
	}

	public bool Save()
	{
		PlayerPrefs.Save();
		return true;
	}

	public byte[] GetBytes(string key)
	{
		throw new NotImplementedException();
	}

	public byte[] GetBytes(string key, byte[] standardValue)
	{
		throw new NotImplementedException();
	}

	public bool SetBytes(string key, byte[] value)
	{
		throw new NotImplementedException();
	}
}
