using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebugUiBase : MonoBehaviour
{
	public enum SpecialDebugEntryType
	{
		None = 0,
		Boolean = 1
	}

	public class DebugEntry
	{
		public bool IsNewLine { get; set; }

		public string EntryName { get; set; }

		public Action DebugAction { get; set; }

		public Func<bool> BoolCheckFunction { get; set; }

		public SpecialDebugEntryType DebugEntryType { get; set; }
	}

	[SerializeField]
	private string m_debugMenuName = "unnamed";

	[SerializeField]
	private int m_referenceScreenX = 1024;

	[SerializeField]
	private int m_referenceScreenY = 768;

	[SerializeField]
	private int m_xOffset = 100;

	[SerializeField]
	private int m_xSize = 100;

	[SerializeField]
	private int m_ySize = 40;

	[SerializeField]
	private int m_ySpacing = 2;

	[SerializeField]
	private bool m_isTopDown = true;

	private int m_startRow;

	private int m_currentRow;

	private int m_currentColumn;

	private float m_factorX;

	private float m_factorY;

	private List<DebugEntry> m_debugSetup;

	private bool m_isOpened;
}
