using System;
using UnityEngine;

public class MakeOrthoIndependent : MonoBehaviour
{
	[SerializeField]
	private Camera m_ScreenPosCamera;

	private Transform cachedTransform;

	private Vector3 initialLocalPosition;

	private float initialOrtho;

	private float lastOrtho;

	public void Init()
	{
		cachedTransform = base.transform;
		initialLocalPosition = cachedTransform.localPosition;
		initialOrtho = m_ScreenPosCamera.orthographicSize;
		lastOrtho = initialOrtho;
	}

	private void OnEnable()
	{
		Init();
	}

	private void Update()
	{
		if (Math.Abs(m_ScreenPosCamera.orthographicSize - lastOrtho) > 0.01f)
		{
			cachedTransform.localPosition = new Vector3(m_ScreenPosCamera.orthographicSize / initialOrtho * initialLocalPosition.x, m_ScreenPosCamera.orthographicSize / initialOrtho * initialLocalPosition.y, cachedTransform.position.z);
			cachedTransform.localScale = m_ScreenPosCamera.orthographicSize / initialOrtho * Vector3.one;
		}
		lastOrtho = m_ScreenPosCamera.orthographicSize;
	}
}
