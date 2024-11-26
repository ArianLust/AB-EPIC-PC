using System;
using UnityEngine;

[Serializable]
public class ActionNode
{
	public Rect editorRect;

	public int nodeID;

	public int[] nodesIn;

	public int[] nodesOut;

	public int[] nodesOutIndex;

	public int target;

	public string text;

	public string[] param;

	public bool enabled = true;

	public bool pass;

	public NodeType type;

	public UnityEngine.Object refObject;

	public UnityEngine.Object refObject1;

	public UnityEngine.Object refObject2;

	public int customInt;

	public int customInt2;

	public int customInt3;

	public float customFloat;

	public float customFloat2;

	public bool customBool;

	public bool customBool2;

	public int objectType;

	public string objectName;

	public Vector3 customVec1;

	public Vector3 customVec2;

	public bool QueueIdle;

	public string secondaryText = string.Empty;
}
