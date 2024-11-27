using System;
using UnityEngine;

public static class GuiX
{
	public static Action<Action> Horizontal = delegate(Action horizontalBlockActions)
	{
		GUILayout.BeginHorizontal();
		horizontalBlockActions();
		GUILayout.EndHorizontal();
	};

	public static Action<Action> Vertical = delegate(Action verticalBlockActions)
	{
		GUILayout.BeginVertical();
		verticalBlockActions();
		GUILayout.EndVertical();
	};

	public static Action<GUIStyle, Action> VerticalStyled = delegate(GUIStyle blockStyle, Action verticalBlockActions)
	{
		if (blockStyle == null)
		{
			GUILayout.BeginVertical();
		}
		else
		{
			GUILayout.BeginVertical(blockStyle);
		}
		verticalBlockActions();
		GUILayout.EndVertical();
	};
}
