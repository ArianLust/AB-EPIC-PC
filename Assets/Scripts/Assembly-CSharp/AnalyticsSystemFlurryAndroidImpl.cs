using System;
using System.Collections.Generic;
using Chimera.Library.Components.Interfaces;
using Prime31;

public class AnalyticsSystemFlurryAndroidImpl : IAnalyticsSystem
{
	private string m_currentAppKey;

	public IAnalyticsSystem Init(string appKey)
	{
		m_currentAppKey = appKey;
		return this;
	}

	public IAnalyticsSystem Init(string appKey, Action<string> debugLogAction, Action<string> errorLogAction)
	{
		m_currentAppKey = appKey;
		return this;
	}

	public bool StartSession(string appKey)
	{
		m_currentAppKey = appKey;
		FlurryAnalytics.startSession(appKey);
		return true;
	}

	public bool StartSession()
	{
		if (string.IsNullOrEmpty(m_currentAppKey))
		{
			DebugLog.Error("[AnalyticsSystemFlurryAndroidImpl] App key not set!");
			return false;
		}
		return StartSession(m_currentAppKey);
	}

	public void EndSession()
	{
	}

	public bool LogEvent(string eventName, bool isTimed = false)
	{
		FlurryAnalytics.logEvent(eventName, isTimed);
		return true;
	}

	public bool LogEventWithParameter(string eventName, string parameterName, string parameterValue, bool isTimed = false)
	{
		if (string.IsNullOrEmpty(parameterName) || string.IsNullOrEmpty(parameterValue))
		{
			FlurryAnalytics.logEvent(eventName, isTimed);
		}
		else
		{
			FlurryAnalytics.logEvent(eventName, new Dictionary<string, string> { { parameterName, parameterValue } }, isTimed);
		}
		return true;
	}

	public bool LogEventWithParameters(string eventName, Dictionary<string, string> parameters, bool isTimed = false)
	{
		DebugLog.Log("track flurry event: " + eventName + " with parameters!");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (string key in parameters.Keys)
		{
			if (key != null && parameters[key] != null)
			{
				dictionary.Add(key, parameters[key]);
			}
		}
		if (dictionary.Count > 10)
		{
			DebugLog.Log("Flurry event has too many parameters (" + dictionary.Count + "), splitting up the event.");
			int num = 0;
			int num2 = 0;
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				num++;
				dictionary2.Add(item.Key, item.Value);
				if (num % 10 == 0)
				{
					LogEventWithParameters(eventName + num2, dictionary2, isTimed);
					dictionary2.Clear();
					num2++;
				}
			}
			if (dictionary2.Count > 0)
			{
				LogEventWithParameters(eventName + num2, dictionary2, isTimed);
			}
			return true;
		}
		FlurryAnalytics.logEvent(eventName, dictionary, isTimed);
		return true;
	}

	public bool EndTimedEvent(string eventName)
	{
		FlurryAnalytics.endTimedEvent(eventName);
		return true;
	}

	public bool EndTimedEvent(string eventName, Dictionary<string, string> parameters)
	{
		FlurryAnalytics.endTimedEvent(eventName, parameters);
		return true;
	}

	private string DictionaryToString(Dictionary<string, string> dict)
	{
		List<string> list = new List<string>();
		foreach (string key in dict.Keys)
		{
			list.Add(string.Format("{0}||{1}", key, dict[key]));
		}
		return string.Join("|||", list.ToArray());
	}

	public void SetAge(int age)
	{
		FlurryAnalytics.setAge(age);
	}

	public void SetGenderFemale(bool isFemale)
	{
		FlurryAnalytics.setGender(isFemale ? FlurryGender.Female : FlurryGender.Male);
	}
}
