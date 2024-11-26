using System.Collections;
using UnityEngine;

public class WaitTimeOrAbort
{
	private float m_Time;

	private bool checkTime = true;

	public bool isDone { get; private set; }

	public WaitTimeOrAbort(float time)
	{
		if (time == 0f)
		{
			checkTime = false;
		}
		m_Time = time;
	}

	public void Abort()
	{
		isDone = true;
	}

	public IEnumerator Run()
	{
		while (!isDone && (!checkTime || m_Time > 0f))
		{
			yield return new WaitForEndOfFrame();
			m_Time -= Time.deltaTime;
		}
	}
}
