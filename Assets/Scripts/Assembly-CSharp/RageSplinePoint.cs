using System;
using UnityEngine;

[Serializable]
public class RageSplinePoint
{
	public Vector3 point;

	public Vector3 inCtrl;

	public Vector3 outCtrl;

	public float widthMultiplier = 1f;

	public bool natural;

	public RageSplinePoint(Vector3 point, Vector3 inCtrl, Vector3 outCtrl, float width, bool natural)
	{
		this.point = point;
		this.inCtrl = inCtrl;
		this.outCtrl = outCtrl;
		widthMultiplier = width;
		this.natural = natural;
	}

	public RageSplinePoint Clone()
	{
		return new RageSplinePoint(point, inCtrl, outCtrl, widthMultiplier, natural);
	}
}
