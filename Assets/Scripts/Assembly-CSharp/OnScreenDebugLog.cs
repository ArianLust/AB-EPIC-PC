using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class OnScreenDebugLog : MonoBehaviour
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct LogEntry
	{
		public string LogString { get; set; }

		public string StackTrace { get; set; }

		public LogType Type { get; set; }
	}

	public bool m_ShowLog = true;

	public bool m_ShowLogButtons = true;

	public bool m_ShowMessage = true;

	public bool m_ShowWarning = true;

	public bool m_ShowError = true;

	private bool m_log = true;

	private static int m_buttonCount = 5;

	private static int m_mainThreadId;

	private Queue<LogEntry> m_logQueue = new Queue<LogEntry>();

	public static float m_logWidth;

	private static float m_logY;

	public List<LogEntry> LogEntries = new List<LogEntry>();

	private Vector2 m_scrollPosition = Vector2.zero;

	private bool m_isResizing;

	private Rect m_windowRect = new Rect(m_logX, m_logY, LogWidth, m_logHeight);

	private Rect m_windowResizeStart;

	private Vector2 m_minWindowSize = new Vector2(75f, m_buttonHeight);

	private readonly GUIContent m_gcDrag = new GUIContent(string.Empty, "drag to resize");

	private readonly GUIStyle m_styleWindowResize = GUIStyle.none;

	private ScreenOrientation m_screenOrientation;

	private bool m_initDone;

	private static int screenWidth
	{
		get
		{
			return Screen.width;
		}
	}

	private static int screenHeight
	{
		get
		{
			return Screen.height;
		}
	}

	private static int m_currentThreadId
	{
		get
		{
			return Thread.CurrentThread.ManagedThreadId;
		}
	}

	public static float LogWidth
	{
		get
		{
			if (m_logWidth == 0f)
			{
				m_logWidth = screenWidth / 5 * 2;
			}
			return m_logWidth;
		}
		set
		{
			m_logWidth = value;
		}
	}

	private static float m_buttonHeight
	{
		get
		{
			return screenHeight / 8;
		}
	}

	private static float m_buttonWidth
	{
		get
		{
			return LogWidth / (float)m_buttonCount;
		}
	}

	private static float m_logHeight
	{
		get
		{
			return (float)screenHeight - m_buttonHeight;
		}
	}

	private static float m_logX
	{
		get
		{
			return (float)screenWidth - LogWidth;
		}
	}

	private static float m_buttonsY
	{
		get
		{
			return m_logHeight;
		}
	}

	private static float m_buttonsX
	{
		get
		{
			return m_logX;
		}
	}

	public Vector2 ScrollPosition
	{
		get
		{
			return m_scrollPosition;
		}
		set
		{
			m_scrollPosition = value;
		}
	}

	[method: MethodImpl(32)]
	public static event Action<ScreenOrientation> OnScreenOrientationChange;

	private void Update()
	{
		if (m_screenOrientation != 0 && Screen.orientation != m_screenOrientation)
		{
			OnScreenOrientationChanged();
		}
		m_screenOrientation = Screen.orientation;
		if (m_logQueue.Count != 0)
		{
			LogEntry logEntry = m_logQueue.Dequeue();
			HandleLog(logEntry.LogString, logEntry.StackTrace, logEntry.Type);
		}
	}

	private void OnScreenOrientationChanged()
	{
		Debug.Log("[OnScreenDebugLog] OnScreenOrientationChanged: " + Screen.orientation);
		if (OnScreenDebugLog.OnScreenOrientationChange != null)
		{
			OnScreenDebugLog.OnScreenOrientationChange(Screen.orientation);
		}
	}

	private void Awake()
	{
		m_mainThreadId = Thread.CurrentThread.ManagedThreadId;
		LogEntry logEntry = default(LogEntry);
		logEntry.LogString = "LogFilePath = " + Application.persistentDataPath;
		logEntry.Type = LogType.Log;
		LogEntry item = logEntry;
		LogEntries.Add(item);
		Application.RegisterLogCallbackThreaded(HandleLog);
		m_initDone = true;
	}

	private void OnDestroy()
	{
		Application.RegisterLogCallbackThreaded(null);
	}

	public void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (m_currentThreadId != m_mainThreadId)
		{
			m_logQueue.Enqueue(new LogEntry
			{
				LogString = logString,
				StackTrace = stackTrace,
				Type = type
			});
			return;
		}
		if (type == LogType.Log || type == LogType.Warning)
		{
			stackTrace = string.Empty;
		}
		if ((m_ShowLog || type != LogType.Log) && (m_ShowWarning || type != LogType.Warning) && (m_ShowError || type != 0))
		{
			LogEntry logEntry = default(LogEntry);
			logEntry.LogString = logString.Trim();
			logEntry.StackTrace = stackTrace;
			logEntry.Type = type;
			LogEntry item = logEntry;
			if (LogEntries.Count >= 200)
			{
				LogEntries.RemoveAt(LogEntries.Count - 1);
			}
			LogEntries.Insert(0, item);
		}
	}

	private void OnGUI()
	{
		if (!m_initDone)
		{
			return;
		}
		if (m_ShowLogButtons)
		{
			if (GUI.Button(new Rect(m_buttonsX, m_buttonsY, m_buttonWidth, m_buttonHeight), (!m_ShowLog) ? "Show" : "Hide"))
			{
				m_ShowLog = !m_ShowLog;
			}
			if (GUI.Button(new Rect(m_buttonsX + m_buttonWidth, m_buttonsY, m_buttonWidth, m_buttonHeight), "Clear"))
			{
				LogEntries = new List<LogEntry>();
			}
			if (GUI.Button(new Rect(m_buttonsX + 2f * m_buttonWidth, m_buttonsY, m_buttonWidth, m_buttonHeight), "Stop/Start"))
			{
				if (m_log)
				{
					Application.RegisterLogCallback(null);
				}
				else
				{
					Application.RegisterLogCallback(HandleLog);
				}
				m_log = !m_log;
			}
			if (GUI.Button(new Rect(m_buttonsX + 3f * m_buttonWidth, m_buttonsY, m_buttonWidth, m_buttonHeight), "Mailto"))
			{
				SendViaEmail();
			}
			if (GUI.Button(new Rect(m_buttonsX + -1f * m_buttonWidth, m_buttonsY, m_buttonWidth, m_buttonHeight), "Load Shader Scene"))
			{
				Application.LoadLevel("ShaderScene");
			}
			if (GUI.Button(new Rect(m_buttonsX + 4f * m_buttonWidth, m_buttonsY, m_buttonWidth, m_buttonHeight), "Save"))
			{
				SaveToFile();
			}
		}
		if (m_ShowLog)
		{
			m_windowRect.height = m_logHeight;
			m_windowRect.width = LogWidth;
			m_windowRect.x = m_logX;
			m_windowRect.y = m_logY;
			m_windowRect = GUI.Window(0, m_windowRect, DebugLogWindow, "OnScreenDebugLog");
		}
	}

	public void ClearLog()
	{
		LogEntries.Clear();
	}

	private void SendViaEmail()
	{
		string text = string.Empty;
		for (int i = 0; i < LogEntries.Count; i++)
		{
			string text2 = text;
			text = text2 + LogEntries[i].LogString + "\n" + LogEntries[i].StackTrace + "\n\n";
		}
		text = WWW.EscapeURL(text).Replace("+", "%20");
		Application.OpenURL("mailto:?subject=OnScreenDebugLogMail&body=" + text);
	}

	private void SaveToFile()
	{
		string path = Application.persistentDataPath + "\\InGameDebugLog_" + DateTime.Now.Date.ToString("dd.MM.yyyy") + "_" + DateTime.Now.ToString("hhmmss") + ".txt";
		using (StreamWriter streamWriter = new StreamWriter(path))
		{
			for (int i = 0; i < LogEntries.Count; i++)
			{
				streamWriter.WriteLine(LogEntries[i].LogString + "\r\n" + LogEntries[i].StackTrace + "\r\n");
			}
		}
	}

	private void DebugLogWindow(int windowID)
	{
		if (windowID != 0)
		{
			return;
		}
		m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, true);
		int i = 0;
		for (int count = LogEntries.Count; i < count; i++)
		{
			switch (LogEntries[i].Type)
			{
			case LogType.Log:
				GUI.contentColor = Color.white;
				break;
			case LogType.Warning:
				GUI.contentColor = Color.yellow;
				break;
			case LogType.Error:
				GUI.contentColor = Color.red;
				break;
			case LogType.Exception:
				GUI.contentColor = Color.magenta;
				break;
			case LogType.Assert:
				GUI.contentColor = Color.cyan;
				break;
			}
			GUILayout.Label(LogEntries[i].LogString + ((!string.IsNullOrEmpty(LogEntries[i].StackTrace)) ? ("\n" + LogEntries[i].StackTrace) : string.Empty));
		}
		GUILayout.EndScrollView();
		m_windowRect = ResizeWindow(m_windowRect);
		GUI.DragWindow(new Rect(0f, 0f, m_windowRect.width, 20f));
	}

	private Rect ResizeWindow(Rect windowRect)
	{
		Vector2 point = GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, (float)screenHeight - Input.mousePosition.y));
		Rect rect = GUILayoutUtility.GetRect(m_gcDrag, m_styleWindowResize);
		if (Event.current.type == EventType.MouseDown && rect.Contains(point))
		{
			m_isResizing = true;
			m_windowResizeStart = new Rect(point.x, point.y, windowRect.width, windowRect.height);
			Event.current.Use();
		}
		else if (Event.current.type == EventType.MouseUp && m_isResizing)
		{
			m_isResizing = false;
			Event.current.Use();
		}
		else if (!Input.GetMouseButton(0))
		{
			m_isResizing = false;
		}
		if (m_isResizing)
		{
			windowRect.width = Mathf.Max(m_minWindowSize.x, m_windowResizeStart.width + (point.x - m_windowResizeStart.x));
			windowRect.height = Mathf.Max(m_minWindowSize.y, m_windowResizeStart.height + (point.y - m_windowResizeStart.y));
			windowRect.xMax = Mathf.Min(screenWidth, windowRect.xMax);
			windowRect.yMax = Mathf.Min(screenHeight, windowRect.yMax);
		}
		return windowRect;
	}
}
