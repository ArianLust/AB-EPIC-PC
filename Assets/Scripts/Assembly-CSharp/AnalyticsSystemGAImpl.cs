using System;
using System.Collections.Generic;
using System.Text;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class AnalyticsSystemGAImpl : IAnalyticsSystem
{
	private const string Url = "http://www.google-analytics.com/collect?";

	private readonly Dictionary<string, string> m_required = new Dictionary<string, string>();

	private string m_appKey;

	private bool m_sessionActive;

	public IAnalyticsSystem Init(string appKey)
	{
		m_appKey = appKey;
		PerformInit();
		return this;
	}

	public bool StartSession()
	{
		if (string.IsNullOrEmpty(m_appKey))
		{
			throw new Exception("appKey not set!");
		}
		StartSession(m_appKey);
		return true;
	}

	public bool StartSession(string appKey)
	{
		m_appKey = appKey;
		PerformInit();
		if (m_sessionActive)
		{
			return false;
		}
		m_sessionActive = true;
		LogEventWithParameters(null, new Dictionary<string, string> { { "sc", "start" } }, false);
		return true;
	}

	private void PerformInit()
	{
		m_required.Clear();
		m_required.Add("tid", m_appKey);
		m_required.Add("cid", default(Guid).ToString());
		m_required.Add("v", "1");
		m_required.Add("t", "pageview");
	}

	public bool LogEvent(string eventName, bool isTimed)
	{
		throw new NotImplementedException("this does not make sense right now because this class was introduced to only do performance logging");
	}

	public bool LogEventWithParameters(string eventName, Dictionary<string, string> parameters, bool isTimed)
	{
		string text = MakeUrl(parameters);
		if (text != null)
		{
			MakeRequest(text);
		}
		return text != null;
	}

	public bool EndTimedEvent(string eventName)
	{
		throw new NotImplementedException("this does not make sense right now because this class was introduced to only do performance logging");
	}

	public bool EndTimedEvent(string eventName, Dictionary<string, string> parameters)
	{
		throw new NotImplementedException("this does not make sense right now because this class was introduced to only do performance logging");
	}

	public void EndSession()
	{
		if (m_sessionActive)
		{
			m_sessionActive = false;
			LogEventWithParameters(null, new Dictionary<string, string> { { "sc", "end" } }, false);
		}
	}

	public void SetAge(int age)
	{
		throw new NotImplementedException("should be possible to implement, todo");
	}

	public void SetGenderFemale(bool isFemale)
	{
		throw new NotImplementedException("should be possible to implement, todo");
	}

	private string MakeUrl(Dictionary<string, string> additionalParams)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>(m_required);
		if (additionalParams != null)
		{
			foreach (KeyValuePair<string, string> additionalParam in additionalParams)
			{
				dictionary.Add(additionalParam.Key, additionalParam.Value);
			}
		}
		StringBuilder stringBuilder = new StringBuilder("http://www.google-analytics.com/collect?");
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			stringBuilder.Append(string.Format("?{0}={1}", item.Key, item.Value));
		}
		return WWW.EscapeURL(stringBuilder.ToString());
	}

	private void MakeRequest(string url)
	{
		WWW.LoadFromCacheOrDownload(url, 1);
	}

	public bool LogEventWithParameter(string eventName, string parameterName, string parameterValue, bool isTimed = false)
	{
		throw new NotImplementedException();
	}
}
