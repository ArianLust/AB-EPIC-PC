using System.Collections.Generic;
using UnityEngine;

public class TimeScaleMgr : MonoBehaviour
{
	private Dictionary<string, float> m_TimeScales = new Dictionary<string, float>();

	private float currentTimeScale = 1f;

	public void AddTimeScale(string tag, float timeScale)
	{
		if (m_TimeScales.ContainsKey(tag))
		{
			if (timeScale == 1f)
			{
				m_TimeScales.Remove(tag);
			}
			else
			{
				m_TimeScales[tag] = timeScale;
			}
		}
		else if (timeScale != 1f)
		{
			m_TimeScales.Add(tag, timeScale);
		}
	}

	public float GetTimeScale(string tag)
	{
		if (m_TimeScales.ContainsKey(tag))
		{
			return m_TimeScales[tag];
		}
		return currentTimeScale;
	}

	public void ResetTimeScales()
	{
		m_TimeScales.Clear();
	}

	public void RemoveTimeScale(string tag)
	{
		m_TimeScales.Remove(tag);
	}

	private void Update()
	{
		if (m_TimeScales.Count == 0)
		{
			if (Time.timeScale != 1f)
			{
				Time.timeScale = 1f;
			}
			currentTimeScale = 1f;
			return;
		}
		currentTimeScale = float.MaxValue;
		foreach (float value in m_TimeScales.Values)
		{
			float num = value;
			if (num <= currentTimeScale)
			{
				currentTimeScale = num;
			}
		}
		Time.timeScale = currentTimeScale;
	}
}
