using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AndroidBackButtonMgr : MonoBehaviour
{
	[SerializeField]
	private List<BackButtonRegistry> m_currentBackButtonActions = new List<BackButtonRegistry>();

	public List<string> m_BlockReasons = new List<string>();

	public bool IsBackButtonAvailiable
	{
		get
		{
			return m_BlockReasons.Count == 0 && Input.touchCount <= 0 && m_currentBackButtonActions.Count > 0;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Joystick8Button19) && IsBackButtonAvailiable) // Input.GetKeyDown(KeyCode.Escape) && 
		{
			m_currentBackButtonActions.LastOrDefault().BackButtonAction();
		}
	}

	public void RegisterAction(int depth, Action action)
	{
		bool flag = false;
		for (int i = 0; i < m_currentBackButtonActions.Count; i++)
		{
			if (m_currentBackButtonActions[i].Depth == depth)
			{
				m_currentBackButtonActions[i].BackButtonAction = action;
				flag = true;
			}
		}
		if (!flag)
		{
			m_currentBackButtonActions.Add(new BackButtonRegistry
			{
				Depth = depth,
				BackButtonAction = action
			});
		}
		m_currentBackButtonActions = m_currentBackButtonActions.OrderBy((BackButtonRegistry a) => a.Depth).ToList();
	}

	public void DeRegisterAction(int depth)
	{
		for (int num = m_currentBackButtonActions.Count - 1; num >= 0; num--)
		{
			if (m_currentBackButtonActions[num].Depth == depth)
			{
				m_currentBackButtonActions.RemoveAt(num);
			}
		}
	}

	public void RegisterBlockReason(string reason)
	{
		if (!m_BlockReasons.Contains(reason))
		{
			m_BlockReasons.Add(reason);
		}
	}

	public void DeRegisterBlockReason(string reason)
	{
		m_BlockReasons.Remove(reason);
		foreach (string blockReason in m_BlockReasons)
		{
			DebugLog.Log("Deregistered " + reason + ", still left: " + blockReason);
		}
	}

	public void Reset()
	{
		m_BlockReasons.Clear();
		m_currentBackButtonActions.Clear();
	}
}
