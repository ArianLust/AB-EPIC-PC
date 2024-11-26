using UnityEngine;

public class RageSplineLite : MonoBehaviour
{
	[SerializeField]
	private RageCurve m_SplineCurve;

	[SerializeField]
	private bool m_IsOpenEnded;

	public void CreateFromSpline()
	{
		RageSpline component = GetComponent<RageSpline>();
		if ((bool)component)
		{
			m_SplineCurve = component.spline.Clone();
			m_IsOpenEnded = component.SplineIsOpenEnded();
		}
	}

	public Vector3 GetPositionWorldSpace(float splinePosition)
	{
		return base.transform.TransformPoint(m_SplineCurve.GetPoint(splinePosition * GetLastSplinePosition()));
	}

	public float GetLastSplinePosition()
	{
		if (m_IsOpenEnded)
		{
			return (float)(GetPointCount() - 1) / (float)GetPointCount();
		}
		return 1f;
	}

	public int GetPointCount()
	{
		return m_SplineCurve.points.Length;
	}

	public float GetLength()
	{
		return m_SplineCurve.GetLength(128, GetLastSplinePosition());
	}
}
