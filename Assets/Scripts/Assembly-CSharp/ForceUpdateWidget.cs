using System.Collections.Generic;
using UnityEngine;

public class ForceUpdateWidget : MonoBehaviour
{
	[SerializeField]
	private UIWidget[] m_widgets;

	[SerializeField]
	private bool m_useDictionary;

	private Dictionary<string, UIWidget> m_widgetDict;

	private void Awake()
	{
		if (m_widgets == null || !m_useDictionary)
		{
			return;
		}
		m_widgetDict = new Dictionary<string, UIWidget>();
		for (int i = 0; i < m_widgets.Length; i++)
		{
			string key = m_widgets[i].gameObject.name;
			if (!m_widgetDict.ContainsKey(key))
			{
				m_widgetDict.Add(m_widgets[i].gameObject.name, m_widgets[i]);
			}
		}
	}

	public void ForceUpdate(string widgetName)
	{
		if (m_widgetDict != null && m_widgetDict.ContainsKey(widgetName))
		{
			m_widgetDict[widgetName].MarkAsChanged();
		}
	}

	public void ForceUpdateAll()
	{
		if (m_widgets != null)
		{
			for (int i = 0; i < m_widgets.Length; i++)
			{
				m_widgets[i].MarkAsChanged();
			}
		}
	}
}
