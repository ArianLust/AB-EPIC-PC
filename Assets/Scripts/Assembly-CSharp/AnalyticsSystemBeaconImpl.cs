using System.Collections.Generic;
using Chimera.Library.Components.Interfaces;
using Rcs;

internal class AnalyticsSystemBeaconImpl : IAnalyticsSystem
{
	private const string SessionStartEventSessionIdParameterName = "SessionID";

	private const string RovioAnalyticsFlagParameterName = "RovioAnalytics";

	private const string CustomerIdParameterName = "CustomerID";

	private bool m_sessionEnded;

	private bool m_isSessionStarted;

	private bool m_hasSessionIdBeenIncreased;

	private Analytics m_analytics;

	public IAnalyticsSystem Init(string appKey)
	{
		DebugLog.Log(GetType(), "Init");
		return this;
	}

	public bool StartSession()
	{
		if (m_isSessionStarted)
		{
			return true;
		}
		m_analytics = new Analytics(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy);
		m_isSessionStarted = true;
		return true;
	}

	public void EndSession()
	{
		m_isSessionStarted = false;
		m_hasSessionIdBeenIncreased = false;
	}

	public bool LogEventWithParameter(string eventName, string parameterName, string parameterValue, bool isTimed = false)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(parameterName, parameterValue);
		Dictionary<string, string> parameters = dictionary;
		return LogEventWithParameters(eventName, parameters, isTimed);
	}

	public bool LogEvent(string eventName, bool isTimed = false)
	{
		return LogEventWithParameters(eventName, new Dictionary<string, string>(), isTimed);
	}

	public bool LogEventWithParameters(string eventName, Dictionary<string, string> parameters, bool isTimed = false)
	{
		if (parameters.Count > 0)
		{
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				if (string.IsNullOrEmpty(parameter.Key) || string.IsNullOrEmpty(parameter.Value))
				{
					return false;
				}
			}
		}
		if (!CanLog())
		{
			return false;
		}
		if (!parameters.ContainsKey("SessionID"))
		{
			parameters.Add("SessionID", GetCurrentSkynestSessionId());
		}
		if (!parameters.ContainsKey("RovioAnalytics"))
		{
			parameters.Add("RovioAnalytics", "true");
		}
		if (!parameters.ContainsKey("CustomerID"))
		{
			parameters.Add("CustomerID", DIContainerConfig.GetSkuName());
		}
		m_analytics.Log(eventName, new Dictionary<string, string>(parameters));
		return true;
	}

	private bool CanLog()
	{
		if (!m_isSessionStarted || DIContainerInfrastructure.GetProfileMgr().CurrentProfile == null)
		{
			return false;
		}
		if (!m_hasSessionIdBeenIncreased)
		{
			DIContainerInfrastructure.GetProfileMgr().CurrentProfile.SkynestAnalyticsSessionId++;
			DIContainerInfrastructure.GetProfileMgr().SaveCurrentProfile();
			m_hasSessionIdBeenIncreased = true;
		}
		return true;
	}

	private string GetCurrentSkynestSessionId()
	{
		return DIContainerInfrastructure.GetProfileMgr().CurrentProfile.SkynestAnalyticsSessionId.ToString();
	}

	public void SetAge(int age)
	{
	}

	public void SetGenderFemale(bool isFemale)
	{
	}

	public bool StartSession(string appKey)
	{
		return false;
	}

	public bool EndTimedEvent(string eventName, Dictionary<string, string> parameters)
	{
		return false;
	}

	public bool EndTimedEvent(string eventName)
	{
		return false;
	}
}
