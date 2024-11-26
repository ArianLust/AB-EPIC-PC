using System;
using System.IO;
using System.Text;
using UnityEngine;

public class BuildParameterReader
{
	public string DemystifyTextFromFile(string filename, int magic)
	{
		if (!File.Exists(filename))
		{
			return null;
		}
		try
		{
			string content = ReadAllText(filename);
			string text = DemystifyText(content, magic);
			Debug.Log("Demystified: " + text);
			return text;
		}
		catch (Exception)
		{
		}
		return null;
	}

	private static string ReadAllText(string path)
	{
		using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8, true))
		{
			return streamReader.ReadToEnd();
		}
	}

	public string DemystifyText(string content, int magic)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in content)
		{
			stringBuilder.Append((char)(c ^ magic));
		}
		return stringBuilder.ToString();
	}
}
