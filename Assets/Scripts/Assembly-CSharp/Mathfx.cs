using UnityEngine;

public class Mathfx
{
	public static float Clamp01(float a)
	{
		a = ((!(a > 1f)) ? a : 1f);
		return (!(a < 0f)) ? a : 0f;
	}

	public static int Clamp01(int a)
	{
		a = ((a > 1) ? 1 : a);
		return (a >= 0) ? a : 0;
	}

	public static bool Approximately(float a, float b)
	{
		return a + 5.96E-08f >= b && a - 5.96E-08f <= b;
	}

	public static Vector3 Add(ref Vector3 v1, ref Vector3 v2)
	{
		return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
	}

	public static Vector3 Add(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3)
	{
		return new Vector3(v1.x + v2.x + v3.x, v1.y + v2.y + v3.y, v1.z + v2.z + v3.z);
	}

	public static Vector3 Add(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, ref Vector3 v4)
	{
		return new Vector3(v1.x + v2.x + v3.x + v4.x, v1.y + v2.y + v3.y + v4.y, v1.z + v2.z + v3.z + v4.z);
	}

	public static Vector3 Add(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, ref Vector3 v4, ref Vector3 v5)
	{
		return new Vector3(v1.x + v2.x + v3.x + v4.x + v5.x, v1.y + v2.y + v3.y + v4.y + v5.y, v1.z + v2.z + v3.z + v4.z + v5.z);
	}

	public static Vector3 Add(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, ref Vector3 v4, ref Vector3 v5, ref Vector3 v6)
	{
		return new Vector3(v1.x + v2.x + v3.x + v4.x + v5.x + v6.x, v1.y + v2.y + v3.y + v4.y + v5.y + v6.y, v1.z + v2.z + v3.z + v4.z + v5.z + v6.z);
	}

	public static Vector3 Mult(ref Vector3 v1, ref Vector3 v2)
	{
		return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
	}

	public static Vector3 Mult(ref Vector3 v1, ref float f1)
	{
		return new Vector3(v1.x * f1, v1.y * f1, v1.z * f1);
	}

	public static Vector3 Mult(Vector3 v1, ref float f1)
	{
		return new Vector3(v1.x * f1, v1.y * f1, v1.z * f1);
	}

	public static Vector3 Mult(Vector3 v1, float f1)
	{
		return new Vector3(v1.x * f1, v1.y * f1, v1.z * f1);
	}

	public static Vector3 Mult(ref Vector3 v1, ref float f1, ref float f2)
	{
		return new Vector3(v1.x * f1 * f2, v1.y * f1 * f2, v1.z * f1 * f2);
	}

	public static Vector3 Mult(ref Vector3 v1, ref float f1, float f2)
	{
		return new Vector3(v1.x * f1 * f2, v1.y * f1 * f2, v1.z * f1 * f2);
	}
}
