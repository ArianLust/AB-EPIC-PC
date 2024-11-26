using System.Collections.Generic;
using Chimera.Library.Components.Interfaces;

public class AnalyticsSystemMultipleImpl : IAnalyticsSystem
{
	private List<IAnalyticsSystem> m_analyticsSystemList = new List<IAnalyticsSystem>();

	public void Add(IAnalyticsSystem analyticsSystem)
	{
		DebugLog.Log(GetType(), "Add: " + analyticsSystem.GetType());
		if (!m_analyticsSystemList.Contains(analyticsSystem))
		{
			m_analyticsSystemList.Add(analyticsSystem);
		}
		else
		{
			DebugLog.Log(GetType(), "Add: Already added!" + analyticsSystem.GetType());
		}
	}

	public void EndSession()
	{
		foreach (IAnalyticsSystem analyticsSystem in m_analyticsSystemList)
		{
			analyticsSystem.EndSession();
		}
	}

	public bool EndTimedEvent(string eventName, Dictionary<string, string> parameters)
	{
		bool flag = true;
		foreach (IAnalyticsSystem analyticsSystem in m_analyticsSystemList)
		{
			flag = flag && analyticsSystem.EndTimedEvent(eventName, parameters);
		}
		return flag;
	}

	public bool EndTimedEvent(string eventName)
	{
		bool flag = true;
		foreach (IAnalyticsSystem analyticsSystem in m_analyticsSystemList)
		{
			flag = flag && analyticsSystem.EndTimedEvent(eventName);
		}
		return flag;
	}

	public IAnalyticsSystem Init(string appKey)
	{
		foreach (IAnalyticsSystem analyticsSystem in m_analyticsSystemList)
		{
			analyticsSystem.Init(appKey);
		}
		return this;
	}

	public bool LogEvent(string eventName, bool isTimed = false)
	{
		bool flag = true;
		foreach (IAnalyticsSystem analyticsSystem in m_analyticsSystemList)
		{
			flag = flag && analyticsSystem.LogEvent(eventName, isTimed);
		}
		return flag;
	}

	public bool LogEventWithParameter(string eventName, string parameterName, string parameterValue, bool isTimed = false)
	{
		bool flag = true;
		foreach (IAnalyticsSystem analyticsSystem in m_analyticsSystemList)
		{
			flag = flag && analyticsSystem.LogEventWithParameter(eventName, parameterName, parameterValue, isTimed);
		}
		return flag;
	}

	public bool LogEventWithParameters(string eventName, Dictionary<string, string> parameters, bool isTimed = false)
	{
		bool flag = true;
		foreach (IAnalyticsSystem analyticsSystem in m_analyticsSystemList)
		{
			flag = flag && analyticsSystem.LogEventWithParameters(eventName, parameters, isTimed);
		}
		return flag;
	}

	public void SetAge(int age)
	{
		foreach (IAnalyticsSystem analyticsSystem in m_analyticsSystemList)
		{
			analyticsSystem.SetAge(age);
		}
	}

	public void SetGenderFemale(bool isFemale)
	{
		foreach (IAnalyticsSystem analyticsSystem in m_analyticsSystemList)
		{
			analyticsSystem.SetGenderFemale(isFemale);
		}
	}

	public bool StartSession(string appKey)
	{
		bool flag = true;
		foreach (IAnalyticsSystem analyticsSystem in m_analyticsSystemList)
		{
			flag = flag && analyticsSystem.StartSession(appKey);
		}
		return flag;
	}

	public bool StartSession()
	{
		DebugLog.Log(GetType(), "StartSession: Starting session without appkey...");
		bool flag = true;
		foreach (IAnalyticsSystem analyticsSystem in m_analyticsSystemList)
		{
			flag = flag && analyticsSystem.StartSession();
			DebugLog.Log(GetType(), string.Concat("StartSession: Starting session for ", analyticsSystem.GetType(), ". success = ", flag));
		}
		return flag;
	}
}
