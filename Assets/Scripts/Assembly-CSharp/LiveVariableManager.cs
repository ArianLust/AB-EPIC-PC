using System;
using System.Collections.Generic;
using UnityEngine;

public class LiveVariableManager : MonoBehaviour
{
	public bool m_showLiveVariables = true;

	private static List<Type> m_supportedTypes = new List<Type>
	{
		typeof(string),
		typeof(bool),
		typeof(int),
		typeof(float),
		typeof(Vector3),
		typeof(Quaternion)
	};

	private Rect m_windowRect = new Rect(0f, 0f, 400f, 400f);

	private static Dictionary<string, object> m_registeredVaiables = new Dictionary<string, object>();

	public static void CheckOutVariable<T>(string variableName, ref T variable)
	{
		if (m_registeredVaiables.ContainsKey(variableName))
		{
			if (m_registeredVaiables[variableName] is T)
			{
				variable = (T)m_registeredVaiables[variableName];
			}
			else
			{
				Debug.LogWarning("[LiveVariableManager] - VariableName: " + variableName + " is already used by a variable of another type");
			}
		}
		else if (m_supportedTypes.Contains(typeof(T)))
		{
			m_registeredVaiables.Add(variableName, variable);
		}
		else
		{
			Debug.LogWarning(string.Concat("[LiveVariableManager] - Type: ", typeof(T), " is not supported"));
		}
	}

	private void OnGUI()
	{
		if (m_showLiveVariables)
		{
			m_windowRect = GUI.Window(1, m_windowRect, LiveVariableWindow, "LiveVariableManager");
		}
	}

	private void LiveVariableWindow(int windowID)
	{
		if (windowID != 1)
		{
			return;
		}
		GUILayout.BeginArea(new Rect(10f, Screen.height / 6, 400f, 400f));
		List<string> list = new List<string>(m_registeredVaiables.Keys);
		foreach (string item in list)
		{
			GUILayout.BeginHorizontal();
			if (m_registeredVaiables[item].GetType() == typeof(string))
			{
				GUI.skin.textField.fontSize = 15;
				m_registeredVaiables[item] = GUILayout.TextField((string)m_registeredVaiables[item]);
			}
			else if (m_registeredVaiables[item].GetType() == typeof(bool))
			{
				GUI.skin.toggle.fontSize = 15;
				m_registeredVaiables[item] = GUILayout.Toggle((bool)m_registeredVaiables[item], item);
			}
			else if (m_registeredVaiables[item].GetType() == typeof(int))
			{
				GUI.skin.textField.fontSize = 15;
				m_registeredVaiables[item] = int.Parse(GUILayout.TextField(((int)m_registeredVaiables[item]).ToString()));
				GUI.skin.label.fontSize = 15;
				GUILayout.Label(item);
			}
			else if (m_registeredVaiables[item].GetType() == typeof(float))
			{
				GUI.skin.textField.fontSize = 15;
				m_registeredVaiables[item] = float.Parse(GUILayout.TextField(((float)m_registeredVaiables[item]).ToString()));
				GUI.skin.label.fontSize = 15;
				GUILayout.Label(item);
			}
			else if (m_registeredVaiables[item].GetType() == typeof(Vector3))
			{
				GUI.skin.textField.fontSize = 15;
				Vector3 zero = Vector3.zero;
				zero.x = float.Parse(GUILayout.TextField(((float)m_registeredVaiables[item]).ToString()));
				zero.y = float.Parse(GUILayout.TextField(((float)m_registeredVaiables[item]).ToString()));
				zero.z = float.Parse(GUILayout.TextField(((float)m_registeredVaiables[item]).ToString()));
				m_registeredVaiables[item] = zero;
				GUI.skin.label.fontSize = 15;
				GUILayout.Label(item);
			}
			else if (m_registeredVaiables[item].GetType() == typeof(Quaternion))
			{
				GUI.skin.textField.fontSize = 15;
				Quaternion identity = Quaternion.identity;
				identity.x = float.Parse(GUILayout.TextField(((float)m_registeredVaiables[item]).ToString()));
				identity.y = float.Parse(GUILayout.TextField(((float)m_registeredVaiables[item]).ToString()));
				identity.z = float.Parse(GUILayout.TextField(((float)m_registeredVaiables[item]).ToString()));
				identity.w = float.Parse(GUILayout.TextField(((float)m_registeredVaiables[item]).ToString()));
				m_registeredVaiables[item] = identity;
				GUI.skin.label.fontSize = 15;
				GUILayout.Label(item);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		GUI.DragWindow();
	}
}
