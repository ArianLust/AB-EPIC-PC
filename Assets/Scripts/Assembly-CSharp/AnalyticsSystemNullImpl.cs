using System.Collections.Generic;
using Chimera.Library.Components.Interfaces;

public class AnalyticsSystemNullImpl : IAnalyticsSystem
{
	private string m_currentAppKey;

	public IAnalyticsSystem Init(string appKey)
	{
		DebugLog.Log("StartSession: Analytics system is not available in editor. Init appkey: " + appKey);
		m_currentAppKey = appKey;
		return this;
	}

	public bool StartSession(string appKey)
	{
		m_currentAppKey = appKey;
		DebugLog.Log("StartSession: Analytics system is not available in editor. StartSession appkey: " + appKey);
		return false;
	}

	public bool StartSession()
	{
		DebugLog.Log("StartSession: Analytics system is not available in editor. StartSession");
		return StartSession(m_currentAppKey);
	}

	public void EndSession()
	{
		DebugLog.Log("[AnalyticsSystemNullImpl] analytics system is not available in editor. EndSession");
	}

	public bool LogEvent(string eventName, bool isTimed = false)
	{
		DebugLog.Log("[AnalyticsSystemNullImpl] analytics system is not available in editor. Event: " + eventName);
		return false;
	}

	public bool LogEventWithParameter(string eventName, string parameterName, string parameterValue, bool isTimed = false)
	{
		return LogEventWithParameters(eventName, new Dictionary<string, string> { { parameterName, parameterValue } }, isTimed);
	}

	public bool LogEventWithParameters(string eventName, Dictionary<string, string> parameters, bool isTimed = false)
	{
		DebugLog.Log("[AnalyticsSystemNullImpl] analytics system is not available in editor. Event: " + eventName);
		return false;
	}

	public bool EndTimedEvent(string eventName)
	{
		DebugLog.Log("[AnalyticsSystemNullImpl] analytics system is not available in editor. Event: " + eventName);
		return false;
	}

	public bool EndTimedEvent(string eventName, Dictionary<string, string> parameters)
	{
		DebugLog.Log("[AnalyticsSystemNullImpl] analytics system is not available in editor. Event: " + eventName);
		return false;
	}

	public void SetAge(int age)
	{
		DebugLog.Log("[AnalyticsSystemNullImpl] analytics system is not available in editor. Age: " + age);
	}

	public void SetGenderFemale(bool isFemale)
	{
		DebugLog.Log("[AnalyticsSystemNullImpl] analytics system is not available in editor. isFemale: " + isFemale);
	}
}
