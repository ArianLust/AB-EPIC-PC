using System;
using UnityEngine;

[Serializable]
public class RageCurve
{
	public Vector3[] precalcNormals;

	public Vector3[] precalcPositions;

	public RageSplinePoint[] points;

	public float PercentPrecalcNormals = 1f;

	public bool Triangulate = true;

	private Vector3[] _pointsArray;

	public Vector3[] PointsArray
	{
		get
		{
			if (_pointsArray == null || _pointsArray.Length != points.Length)
			{
				_pointsArray = new Vector3[points.Length];
				for (int i = 0; i < points.Length; i++)
				{
					_pointsArray[i] = points[i].point;
				}
			}
			return _pointsArray;
		}
	}

	public RageCurve(Vector3[] pts, Vector3[] ctrl, bool[] natural, float[] width)
	{
		points = new RageSplinePoint[pts.Length];
		for (int i = 0; i < pts.Length; i++)
		{
			points[i] = new RageSplinePoint(pts[i], ctrl[i * 2], ctrl[i * 2 + 1], width[i], natural[i]);
		}
	}

	public RageCurve Clone()
	{
		Vector3[] array = new Vector3[points.Length];
		Vector3[] array2 = new Vector3[points.Length * 2];
		float[] array3 = new float[points.Length];
		bool[] array4 = new bool[points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = points[i].point;
			array3[i] = points[i].widthMultiplier;
			array2[i] = points[i].inCtrl;
			array2[i + 1] = points[i].outCtrl;
			array4[i] = points[i].natural;
		}
		return new RageCurve(array, array2, array4, array3);
	}

	public float GetWidth(float t)
	{
		if (points.Length > 0)
		{
			if (t > 0.999f || t < 0f)
			{
				t = mod(t, 0.999f);
			}
			int floorIndex = GetFloorIndex(t);
			float num = t * (float)points.Length - (float)floorIndex;
			num = ((!(num < 0.5f)) ? ((1f - (1f - (num - 0.5f) * 2f) * (1f - (num - 0.5f) * 2f)) / 2f + 0.5f) : (num * 2f * (num * 2f) / 2f));
			if (floorIndex < points.Length - 1)
			{
				return points[floorIndex].widthMultiplier * (1f - num) + points[floorIndex + 1].widthMultiplier * num;
			}
			return points[floorIndex].widthMultiplier * (1f - num) + points[0].widthMultiplier * num;
		}
		return 0f;
	}

	public Vector3 GetNormal(float splinePosition)
	{
		if (splinePosition > 0.999f || splinePosition < 0f)
		{
			splinePosition = mod(splinePosition, 0.999f);
		}
		return precalcNormals[Mathf.Clamp(Mathf.FloorToInt(splinePosition * (float)precalcNormals.Length), 0, precalcNormals.Length - 1)];
	}

	public Vector3 GetNormalInterpolated(float splinePosition, bool openEnded)
	{
		if (openEnded && splinePosition > 0.999f)
		{
			splinePosition = 0.999f;
		}
		else if (splinePosition > 0.999f || splinePosition < 0f)
		{
			splinePosition = mod(splinePosition, 0.999f);
		}
		Vector3 vector = precalcNormals[Mathf.Clamp(Mathf.FloorToInt(splinePosition * (float)precalcNormals.Length), 0, precalcNormals.Length - 1)];
		Vector3 vector2 = precalcNormals[Mathf.Clamp(Mathf.FloorToInt(splinePosition * (float)precalcNormals.Length) + 1, 0, precalcNormals.Length - 1)];
		float num = splinePosition * (float)precalcNormals.Length - (float)Mathf.FloorToInt(splinePosition * (float)precalcNormals.Length);
		Vector3 result = vector * (1f - num) + vector2 * num;
		result.Normalize();
		return result;
	}

	public Vector3 GetNormal(int i)
	{
		if (i >= points.Length || i < 0)
		{
			i = mod(i, points.Length);
		}
		return precalcNormals[Mathf.Clamp(Mathf.FloorToInt((float)i / (float)points.Length * (float)precalcNormals.Length), 0, precalcNormals.Length - 1)];
	}

	public Vector3 CalculateNormal(float t, Vector3 up)
	{
		if (points.Length <= 0)
		{
			return new Vector3(1f, 0f, 0f);
		}
		t = Mathf.Clamp01(t);
		float x = t - 0.001f;
		x = mod(x, 1f);
		int floorIndex = GetFloorIndex(x);
		int ceilIndex = GetCeilIndex(x);
		float segmentPosition = GetSegmentPosition(x);
		RageSplinePoint rageSplinePoint = points[floorIndex];
		RageSplinePoint rageSplinePoint2 = points[ceilIndex];
		float x2 = t + 0.001f;
		x2 = mod(x2, 1f);
		int floorIndex2 = GetFloorIndex(x2);
		int ceilIndex2 = GetCeilIndex(x2);
		float segmentPosition2 = GetSegmentPosition(x2);
		RageSplinePoint rageSplinePoint3 = points[floorIndex2];
		RageSplinePoint rageSplinePoint4 = points[ceilIndex2];
		Vector3 vector = (-3f * rageSplinePoint.point + 9f * (rageSplinePoint.point + rageSplinePoint.outCtrl) - 9f * (rageSplinePoint2.point + rageSplinePoint2.inCtrl) + 3f * rageSplinePoint2.point) * segmentPosition * segmentPosition + (6f * rageSplinePoint.point - 12f * (rageSplinePoint.point + rageSplinePoint.outCtrl) + 6f * (rageSplinePoint2.point + rageSplinePoint2.inCtrl)) * segmentPosition - 3f * rageSplinePoint.point + 3f * (rageSplinePoint.point + rageSplinePoint.outCtrl);
		Vector3 vector2 = (-3f * rageSplinePoint3.point + 9f * (rageSplinePoint3.point + rageSplinePoint3.outCtrl) - 9f * (rageSplinePoint4.point + rageSplinePoint4.inCtrl) + 3f * rageSplinePoint4.point) * segmentPosition2 * segmentPosition2 + (6f * rageSplinePoint3.point - 12f * (rageSplinePoint3.point + rageSplinePoint3.outCtrl) + 6f * (rageSplinePoint4.point + rageSplinePoint4.inCtrl)) * segmentPosition2 - 3f * rageSplinePoint3.point + 3f * (rageSplinePoint3.point + rageSplinePoint3.outCtrl);
		return Vector3.Cross((vector.normalized + vector2.normalized) * 0.5f, up).normalized;
	}

	public Vector3 GetAvgNormal(float t, float dist, int samples)
	{
		Vector3 vector = default(Vector3);
		float num = 999999f;
		float num2 = -999999f;
		int ceilIndex = GetCeilIndex(t);
		int floorIndex = GetFloorIndex(t);
		if (!points[ceilIndex].natural)
		{
			num = ((ceilIndex <= 0) ? ((float)points.Length - 0.01f) : ((float)ceilIndex / (float)points.Length - 0.01f));
		}
		if (!points[floorIndex].natural)
		{
			num2 = ((floorIndex >= points.Length - 1) ? 0.01f : ((float)floorIndex / (float)points.Length + 0.01f));
		}
		for (float num3 = t - dist / 2f; num3 < t + dist / 2f + dist * 0.5f / (float)samples; num3 += dist / (float)samples)
		{
			if (num3 > num2 && num3 < num)
			{
				vector += GetNormal(num3);
			}
		}
		return vector.normalized;
	}

	public void setCtrl(int index, int ctrlIndex, Vector3 value)
	{
		if (points[index].natural)
		{
			if (ctrlIndex == 0)
			{
				points[index].inCtrl = value;
				points[index].outCtrl = value * -1f;
			}
			else
			{
				points[index].inCtrl = value * -1f;
				points[index].outCtrl = value;
			}
		}
		else if (ctrlIndex == 0)
		{
			points[index].inCtrl = value;
		}
		else
		{
			points[index].outCtrl = value;
		}
	}

	public int GetFloorIndex(float t)
	{
		int num = Mathf.FloorToInt(t * (float)points.Length);
		if (num >= points.Length || num < 0)
		{
			num = mod(num, points.Length);
		}
		return num;
	}

	public int GetCeilIndex(float t)
	{
		int num = Mathf.FloorToInt(t * (float)points.Length) + 1;
		if (num >= points.Length || num < 0)
		{
			num = mod(num, points.Length);
		}
		return num;
	}

	public RageSplinePoint GetRageSplinePoint(int index)
	{
		if (index >= points.Length || index < 0)
		{
			index = mod(index, points.Length);
		}
		return points[index];
	}

	public Vector3 GetPointFast(float splinePosition)
	{
		if (splinePosition > 0.999f || splinePosition < 0f)
		{
			splinePosition = mod(splinePosition, 0.999f);
		}
		return precalcPositions[Mathf.Clamp(Mathf.FloorToInt(splinePosition * (float)precalcPositions.Length), 0, precalcPositions.Length - 1)];
	}

	public Vector3 GetPointFastInterpolated(float splinePosition)
	{
		if (splinePosition > 0.999f || splinePosition < 0f)
		{
			splinePosition = mod(splinePosition, 0.999f);
		}
		Vector3 vector = precalcPositions[Mathf.Clamp(Mathf.FloorToInt(splinePosition * (float)precalcPositions.Length), 0, precalcPositions.Length - 1)];
		Vector3 vector2 = precalcPositions[Mathf.Clamp(Mathf.FloorToInt(splinePosition * (float)precalcPositions.Length) + 1, 0, precalcPositions.Length - 1)];
		float num = splinePosition * (float)precalcPositions.Length - (float)Mathf.FloorToInt(splinePosition * (float)precalcPositions.Length);
		return vector * (1f - num) + vector2 * num;
	}

	public Vector3 GetPoint(float t)
	{
		if (t < 1E-05f || t > 0.99999f)
		{
			t = mod(t, 1f);
		}
		int floorIndex = GetFloorIndex(t);
		int ceilIndex = GetCeilIndex(t);
		float segmentPosition = GetSegmentPosition(t);
		RageSplinePoint rageSplinePoint = points[floorIndex];
		RageSplinePoint rageSplinePoint2 = points[ceilIndex];
		float num = 1f - segmentPosition;
		float num2 = num * num;
		float num3 = segmentPosition * segmentPosition;
		return num2 * num * rageSplinePoint.point + 3f * num2 * segmentPosition * (rageSplinePoint.point + rageSplinePoint.outCtrl) + 3f * num * num3 * (rageSplinePoint2.point + rageSplinePoint2.inCtrl) + num3 * segmentPosition * rageSplinePoint2.point;
	}

	public float GetSegmentPosition(float t)
	{
		if (t < 0f || t > 1f)
		{
			t = mod(t, 1f);
		}
		int floorIndex = GetFloorIndex(t);
		return Mathf.Clamp01(t * (float)points.Length - (float)floorIndex);
	}

	public Vector3 GetMiddle(int accuracy)
	{
		Vector3 vector = default(Vector2);
		for (int i = 0; i < accuracy; i++)
		{
			vector += GetPoint((float)i / (float)accuracy);
		}
		return vector * (1f / (float)accuracy);
	}

	public float GetLength(int accuracy, float lastSplinePos)
	{
		float num = 0f;
		float num2 = lastSplinePos / (float)accuracy;
		Vector3 vector = GetPoint(0f);
		for (int i = 1; i < accuracy; i++)
		{
			Vector3 point = GetPoint((float)i * num2);
			num += (point - vector).magnitude;
			vector = point;
		}
		return num;
	}

	public Vector3 GetMin(int accuracy, float start, float end)
	{
		start = Mathf.Clamp01(start);
		end = Mathf.Clamp01(end);
		Vector3 result = new Vector3(100000000f, 100000000f, 100000000f);
		for (int i = 0; i < accuracy; i++)
		{
			Vector3 point = GetPoint((float)i / (float)accuracy * (end - start) + start);
			if (point.x < result.x)
			{
				result.x = point.x;
			}
			if (point.y < result.y)
			{
				result.y = point.y;
			}
			if (point.z < result.z)
			{
				result.z = point.z;
			}
		}
		return result;
	}

	public Vector3 GetMax(int accuracy, float start, float end)
	{
		start = Mathf.Clamp01(start);
		end = Mathf.Clamp01(end);
		Vector3 result = new Vector3(-100000000f, -100000000f, -100000000f);
		for (int i = 0; i < accuracy; i++)
		{
			Vector3 point = GetPoint((float)i / (float)accuracy * (end - start) + start);
			if (point.x > result.x)
			{
				result.x = point.x;
			}
			if (point.y > result.y)
			{
				result.y = point.y;
			}
			if (point.z > result.z)
			{
				result.z = point.z;
			}
		}
		return result;
	}

	public Vector3[] GetSmoothCtrlForNewPoint(float splinePosition)
	{
		int ceilIndex = GetCeilIndex(splinePosition);
		float segmentPosition = GetSegmentPosition(splinePosition);
		Vector3 point = GetPoint(splinePosition);
		Vector3 point2 = points[mod(ceilIndex - 1, points.Length)].point;
		Vector3 point3 = points[mod(ceilIndex, points.Length)].point;
		Vector3 vector = point2 + points[mod(ceilIndex - 1, points.Length)].outCtrl;
		Vector3 vector2 = point3 + points[mod(ceilIndex, points.Length)].inCtrl;
		Vector3 a = Vector3.Lerp(point2, vector, segmentPosition);
		Vector3 vector3 = Vector3.Lerp(vector, vector2, segmentPosition);
		Vector3 b = Vector3.Lerp(vector2, point3, segmentPosition);
		Vector3 vector4 = Vector3.Lerp(a, vector3, segmentPosition);
		Vector3 vector5 = Vector3.Lerp(vector3, b, segmentPosition);
		return new Vector3[2]
		{
			vector4 - point,
			vector5 - point
		};
	}

	private int PointsIndex(int index, RageSplinePoint[] pointsArray)
	{
		return mod(index, pointsArray.Length);
	}

	public int AddRageSplinePoint(float splinePosition)
	{
		RageSplinePoint[] array = new RageSplinePoint[points.Length + 1];
		int ceilIndex = GetCeilIndex(splinePosition);
		Vector3 normalized = (GetPoint(splinePosition + 0.001f) - GetPoint(splinePosition - 0.001f)).normalized;
		float num = points[mod(ceilIndex - 1, points.Length)].outCtrl.magnitude * 0.25f + points[mod(ceilIndex, points.Length)].inCtrl.magnitude * 0.25f;
		int num2 = PointsIndex(ceilIndex - 1, points);
		int num3 = PointsIndex(ceilIndex, points);
		RageSplinePoint rageSplinePoint = points[num2];
		RageSplinePoint rageSplinePoint2 = points[num3];
		RageSplinePoint rageSplinePoint3 = (array[ceilIndex] = new RageSplinePoint(GetPoint(splinePosition), num * normalized * -1f, num * normalized, GetWidth(splinePosition), false));
		float num4 = GetNearestSplinePoint(rageSplinePoint.point, 1000);
		float num5 = GetNearestSplinePoint(rageSplinePoint2.point, 1000);
		float nearestSplinePoint = GetNearestSplinePoint(rageSplinePoint3.point, 1000);
		if (num4 > nearestSplinePoint)
		{
			num4 -= 1f;
		}
		if (num5 < nearestSplinePoint)
		{
			num5 += 1f;
		}
		float num6 = Mathf.Abs(num5 - num4);
		float num7 = 0.5f;
		if (!Mathf.Approximately(num6, 0f))
		{
			num7 = Mathf.Abs(nearestSplinePoint - num4) / num6;
		}
		Vector3 vector = num7 * rageSplinePoint.outCtrl;
		Vector3 vector2 = (1f - num7) * rageSplinePoint2.inCtrl;
		Vector3 vector3 = (1f - num7) * (rageSplinePoint.point + rageSplinePoint.outCtrl) + num7 * (rageSplinePoint2.point + rageSplinePoint2.inCtrl);
		Vector3 vector4 = (1f - num7) * (rageSplinePoint.point + vector) + num7 * vector3;
		Vector3 vector5 = (1f - num7) * vector3 + num7 * (rageSplinePoint2.point + vector2);
		rageSplinePoint3.inCtrl = rageSplinePoint3.inCtrl.normalized * (vector4 - rageSplinePoint3.point).magnitude;
		rageSplinePoint3.outCtrl = rageSplinePoint3.outCtrl.normalized * (vector5 - rageSplinePoint3.point).magnitude;
		rageSplinePoint.natural = (rageSplinePoint2.natural = false);
		rageSplinePoint.outCtrl = vector;
		rageSplinePoint2.inCtrl = vector2;
		for (int i = 0; i < array.Length; i++)
		{
			if (i < ceilIndex)
			{
				array[i] = points[i];
			}
			if (i > ceilIndex)
			{
				array[i] = points[i - 1];
			}
		}
		points = array;
		return ceilIndex;
	}

	private float CalculateReferenceDistance(Vector3 referencePoint, float referenceWeight)
	{
		return Vector3.SqrMagnitude(referencePoint - GetPoint(referenceWeight));
	}

	private float CalculateReferenceDistance(Vector3 referencePoint, int referenceIdx)
	{
		return Vector3.SqrMagnitude(referencePoint - points[referenceIdx].point);
	}

	private float GetWeightFromIndex(int idx)
	{
		return GetNearestSplinePoint(points[idx].point, 1000);
	}

	public void AddRageSplinePoint(int index, Vector3 position)
	{
		RageSplinePoint[] array = new RageSplinePoint[points.Length + 1];
		float num = (float)index / (float)points.Length + 1f / (float)points.Length;
		Vector3 vector = position - GetPoint(num - 0.001f).normalized;
		float num2 = (points[mod(index, points.Length)].point - points[mod(index + 1, points.Length)].point).magnitude * 0.25f;
		array[index] = new RageSplinePoint(position, num2 * vector * -1f, num2 * vector, GetWidth(num), true);
		for (int i = 0; i < array.Length; i++)
		{
			if (i < index)
			{
				array[i] = points[i];
			}
			else if (i > index)
			{
				array[i] = points[i - 1];
			}
		}
		points = array;
	}

	public void ClearPoints()
	{
		points = new RageSplinePoint[0];
	}

	public void AddRageSplinePoint(int index, Vector3 position, Vector3 inCtrl, Vector3 outCtrl, float width, bool natural)
	{
		RageSplinePoint[] array = new RageSplinePoint[points.Length + 1];
		for (int i = 0; i < array.Length; i++)
		{
			if (i < index)
			{
				array[i] = points[i];
			}
			if (i > index)
			{
				array[i] = points[i - 1];
			}
		}
		array[index] = new RageSplinePoint(position, inCtrl, outCtrl, width, natural);
		points = array;
	}

	public void SmartDelPoint(int index)
	{
		if (points.Length <= 2)
		{
			return;
		}
		int num = ((index <= 0) ? (points.Length - 1) : (index - 1));
		int num2 = ((index < points.Length - 1) ? (index + 1) : 0);
		RageSplinePoint rageSplinePoint = points[num];
		RageSplinePoint rageSplinePoint2 = points[index];
		RageSplinePoint rageSplinePoint3 = points[num2];
		rageSplinePoint.natural = (rageSplinePoint3.natural = false);
		float num3 = GetNearestSplinePoint(rageSplinePoint.point, 1000);
		float num4 = GetNearestSplinePoint(rageSplinePoint3.point, 1000);
		float nearestSplinePoint = GetNearestSplinePoint(rageSplinePoint2.point, 1000);
		if (num3 > nearestSplinePoint)
		{
			num3 -= 1f;
		}
		if (num4 < nearestSplinePoint)
		{
			num4 += 1f;
		}
		float num5 = Mathf.Abs(num4 - num3);
		if (Mathf.Approximately(num5, 0f))
		{
			DelPoint(index);
			return;
		}
		float num6 = Mathf.Abs(nearestSplinePoint - num3) / num5;
		if (num6 > 0f)
		{
			rageSplinePoint.outCtrl /= num6;
		}
		if (num6 < 1f)
		{
			rageSplinePoint3.inCtrl /= 1f - num6;
		}
		DelPoint(index);
	}

	public void DelPoint(int index)
	{
		if (points.Length <= 2)
		{
			return;
		}
		RageSplinePoint[] array = new RageSplinePoint[points.Length - 1];
		for (int i = 0; i < array.Length; i++)
		{
			if (i < index)
			{
				array[i] = points[i];
			}
			else
			{
				array[i] = points[i + 1];
			}
		}
		points = array;
	}

	public float GetNearestSplinePoint(Vector3 position, int accuracy)
	{
		float num = 1E+11f;
		float result = 0f;
		for (int i = 0; i < accuracy; i++)
		{
			Vector3 point = GetPoint((float)i / (float)accuracy);
			if ((position - point).sqrMagnitude < num)
			{
				result = (float)i / (float)accuracy;
				num = (position - point).sqrMagnitude;
			}
		}
		return result;
	}

	public int GetNearestSplinePointIndex(float splinePosition)
	{
		float segmentPosition = GetSegmentPosition(splinePosition);
		if (segmentPosition > 0.5f)
		{
			return GetCeilIndex(splinePosition);
		}
		return GetFloorIndex(splinePosition);
	}

	public void PrecalcNormals(int points)
	{
		precalcNormals = new Vector3[points];
		Vector3 up = new Vector3(0f, 0f, -1f);
		for (int i = 0; i < points; i++)
		{
			precalcNormals[i] = CalculateNormal((float)i / (float)(points - 1), up);
		}
	}

	public void PrecalcPositions(int points)
	{
		precalcPositions = new Vector3[points];
		for (int i = 0; i < points; i++)
		{
			precalcPositions[i] = GetPoint((float)i / (float)(points - 1));
			Debug.Log("precalcPositions[" + i + "]=" + precalcPositions[i]);
		}
	}

	public void ForceZeroZ()
	{
		RageSplinePoint[] array = points;
		foreach (RageSplinePoint rageSplinePoint in array)
		{
			if (!Mathf.Approximately(rageSplinePoint.point.z, 0f))
			{
				rageSplinePoint.point = new Vector3(rageSplinePoint.point.x, rageSplinePoint.point.y, 0f);
			}
			if (!Mathf.Approximately(rageSplinePoint.inCtrl.z, 0f))
			{
				rageSplinePoint.inCtrl = new Vector3(rageSplinePoint.inCtrl.x, rageSplinePoint.inCtrl.y, 0f);
			}
			if (!Mathf.Approximately(rageSplinePoint.outCtrl.z, 0f))
			{
				rageSplinePoint.outCtrl = new Vector3(rageSplinePoint.outCtrl.x, rageSplinePoint.outCtrl.y, 0f);
			}
		}
	}

	private int mod(int x, int m)
	{
		return (x % m + m) % m;
	}

	private float mod(float x, float m)
	{
		return (x % m + m) % m;
	}

	public bool Intersects(Vector3 limitStart, Vector3 limitDirection, Vector3 vecStart, Vector3 vec)
	{
		float vectorSide = GetVectorSide(limitStart, limitDirection, vecStart);
		float vectorSide2 = GetVectorSide(limitStart, limitDirection, vecStart + vec);
		return (vectorSide > 0f && vectorSide2 < 0f) || (vectorSide < 0f && vectorSide2 > 0f);
	}

	public Vector3 LimitToOtherSideOfVector(Vector3 limitStart, Vector3 limitDirection, Vector3 vecStart, Vector3 vec)
	{
		float vectorSide = GetVectorSide(limitStart, limitDirection, vecStart);
		float vectorSide2 = GetVectorSide(limitStart, limitDirection, vecStart + vec);
		if ((vectorSide > 0f && vectorSide2 < 0f) || (vectorSide < 0f && vectorSide2 > 0f))
		{
			return Intersect(limitStart, limitStart + limitDirection, vecStart, vecStart + vec);
		}
		return vecStart + vec;
	}

	public float GetVectorSide(Vector3 offset, Vector3 vec, Vector3 position)
	{
		Vector3 vector = offset;
		Vector3 vector2 = offset + vec;
		Vector3 vector3 = position;
		return (vector2.x - vector.x) * (vector3.y - vector.y) - (vector2.y - vector.y) * (vector3.x - vector.x);
	}

	public Vector3 Intersect(Vector3 line1V1, Vector3 line1V2, Vector3 line2V1, Vector3 line2V2)
	{
		float num = line1V2.y - line1V1.y;
		float num2 = line1V1.x - line1V2.x;
		float num3 = num * line1V1.x + num2 * line1V1.y;
		float num4 = line2V2.y - line2V1.y;
		float num5 = line2V1.x - line2V2.x;
		float num6 = num4 * line2V1.x + num5 * line2V1.y;
		float num7 = num * num5 - num4 * num2;
		if (num7 == 0f)
		{
			return line2V2;
		}
		float x = (num5 * num3 - num2 * num6) / num7;
		float y = (num * num6 - num4 * num3) / num7;
		return new Vector3(x, y, 0f);
	}
}
