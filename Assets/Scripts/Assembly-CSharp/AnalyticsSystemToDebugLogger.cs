using System;
using System.Collections.Generic;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class AnalyticsSystemToDebugLogger : IAnalyticsSystem
{
	public IAnalyticsSystem Init(string appKey)
	{
		Debug.Log("AnalyticsToFileLogger.init(" + appKey + ")");
		return this;
	}

	public bool StartSession()
	{
		Debug.Log("AnalyticsToFileLogger.StartSession()");
		return true;
	}

	public bool StartSession(string appKey)
	{
		Debug.Log("AnalyticsToFileLogger.StartSession(" + appKey + ")");
		return true;
	}

	public bool LogEvent(string eventName, bool isTimed)
	{
		Debug.Log("AnalyticsToFileLogger.LogEvent(" + eventName + ", " + isTimed + ")");
		return true;
	}

	public bool LogEventWithParameters(string eventName, Dictionary<string, string> parameters, bool isTimed)
	{
		Debug.Log("AnalyticsToFileLogger.LogEventWithParameters(" + eventName + ", parameters, " + isTimed + ")");
		return true;
	}

	public bool EndTimedEvent(string eventName)
	{
		Debug.Log("AnalyticsToFileLogger.EndTimedEvent(" + eventName + ")");
		return true;
	}

	public bool EndTimedEvent(string eventName, Dictionary<string, string> parameters)
	{
		Debug.Log("AnalyticsToFileLogger.EndTimedEvent(" + eventName + ", parameters)");
		return true;
	}

	public void EndSession()
	{
		Debug.Log("AnalyticsToFileLogger.EndSession()");
	}

	public void SetAge(int age)
	{
		Debug.Log("AnalyticsToFileLogger.SetAge(" + age + ")");
	}

	public void SetGenderFemale(bool isFemale)
	{
		Debug.Log("AnalyticsToFileLogger.SetGenderFemale(" + isFemale + ")");
	}

	public bool LogEventWithParameter(string eventName, string parameterName, string parameterValue, bool isTimed = false)
	{
		throw new NotImplementedException();
	}
}
