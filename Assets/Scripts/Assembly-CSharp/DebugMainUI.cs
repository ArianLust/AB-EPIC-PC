using System;
using UnityEngine;

public class DebugMainUI : GenericDebugUI
{
	private bool m_Opened;

	private bool m_ShowMemory;

	private float m_MemoryInMB;

	private float m_LastLoadTime;

	private bool m_isLoading;

	public FPSDisplay m_FpsDisplay;

	private void ShowMemory()
	{
		m_MemoryInMB = (float)GC.GetTotalMemory(false) / Mathf.Pow(1024f, 2f);
	}
}
