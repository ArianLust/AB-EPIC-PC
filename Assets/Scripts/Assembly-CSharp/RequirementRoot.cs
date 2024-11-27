using System;
using System.Collections.Generic;
using ABH.Shared.Generic;
using UnityEngine;

[Serializable]
public class RequirementRoot
{
	public RequirementType Type;

	public List<GameObject> Roots;

	public UILabel TextLabel;

	public UISprite IconSprite;
}
