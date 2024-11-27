using System;
using System.IO;
using Chimera.Library.Components.ClientLib.CrossPlatformLib.Source.Models;
using UnityEngine;

public class Version
{
	private const char m_separator = '.';

	private const string m_versionNumberFileName = "version";

	private ChimeraVersionNumber m_thisVersion;

	private bool m_isInitialized;

	public int MajorVersion
	{
		get
		{
			return m_thisVersion.MajorVersion;
		}
	}

	public int MinorVersion
	{
		get
		{
			return m_thisVersion.MinorVersion;
		}
	}

	public int Revision
	{
		get
		{
			return m_thisVersion.Revision;
		}
	}

	public int BuildNumber
	{
		get
		{
			return m_thisVersion.BuildNumber;
		}
	}

	public string StoreVersion { get; set; }

	public string FullVersionString
	{
		get
		{
			if (!m_isInitialized)
			{
				Init();
			}
			return m_thisVersion.ToString();
		}
	}

	public ChimeraVersionNumber FullVersion
	{
		get
		{
			if (!m_isInitialized)
			{
				Init();
			}
			return m_thisVersion;
		}
	}

	public int AssetVersionNumber { get; private set; }

	public bool Init()
	{
		if (m_isInitialized)
		{
			return true;
		}
		string versionNumberFromFile = GetVersionNumberFromFile("0.0.0.0");
		m_thisVersion = new ChimeraVersionNumber().FromString(versionNumberFromFile);
		m_thisVersion.ReportError = Debug.Log;
		DebugLog.Log("[Version] Versionnumber for this build is " + m_thisVersion);
		m_isInitialized = true;
		TextAsset textAsset = Resources.Load("CFBundleShortVersionString") as TextAsset;
		StoreVersion = ((!(textAsset != null)) ? "unknown" : (string.Empty + textAsset.text).Trim());
		return true;
	}

	private bool WriteUnityVersionNumberToFile()
	{
		if (!Application.isEditor)
		{
			return false;
		}
		DateTime now = DateTime.Now;
		try
		{
			using (StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/version.txt", false))
			{
				streamWriter.Write(now.Year.ToString("00").Substring(2) + '.' + now.Month.ToString("00") + now.Day.ToString("00") + '.' + now.Hour.ToString("00") + now.Minute.ToString("00") + '.' + "0");
			}
			return true;
		}
		catch (Exception ex)
		{
			DebugLog.Error(ex.ToString());
		}
		return false;
	}

	private string GetVersionNumberFromFile(string standardVersion)
	{
		string result = standardVersion;
		TextAsset textAsset = Resources.Load("version", typeof(TextAsset)) as TextAsset;
		if (textAsset != null)
		{
			result = textAsset.text;
		}
		return result;
	}
}
