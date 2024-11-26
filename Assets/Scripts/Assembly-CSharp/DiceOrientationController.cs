using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceOrientationController : MonoBehaviour
{
	[Serializable]
	private class VectorList
	{
		public List<Vector3> m_StarOrientations;
	}

	[SerializeField]
	private List<VectorList> m_StarOrientationsList;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetOrientationFromStars(int stars)
	{
		if (stars <= m_StarOrientationsList.Count)
		{
			Quaternion localRotation = default(Quaternion);
			VectorList vectorList = m_StarOrientationsList[stars];
			localRotation.eulerAngles = vectorList.m_StarOrientations[UnityEngine.Random.Range(0, vectorList.m_StarOrientations.Count)];
			base.transform.localRotation = localRotation;
		}
	}
}
