using System;
using ABH.Shared.Generic;
using UnityEngine;

[Serializable]
public class SizeTypeToSpacerScale
{
	public CharacterSizeType CharacterSize = CharacterSizeType.Medium;

	public Vector3 SpacerScale;

	public float RootScale = 0.45f;
}
