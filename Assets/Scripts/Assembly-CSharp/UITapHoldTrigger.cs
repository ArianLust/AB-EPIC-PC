using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UITapHoldTrigger : MonoBehaviour
{
	public float TapStartThreshold = 0.5f;

	public float TapEndThreshold = 0.1f;

	public float m_MaxPosDistanceForTap = 10f;

	private bool m_IsTapping;

	private UICamera m_UICamera;

	private Camera m_SceneryCamera;

	private Camera m_interfaceCamera;

	private Transform tapTarget;

	private Vector3 InterfaceCamPos;

	[method: MethodImpl(32)]
	public event Action OnTapBegin;

	[method: MethodImpl(32)]
	public event Action OnTapReleased;

	[method: MethodImpl(32)]
	public event Action OnTapEnd;

	private void Awake()
	{
		m_interfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
		m_UICamera = m_interfaceCamera.GetComponent<UICamera>();
		m_SceneryCamera = Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("SceneryCamera"));
	}

	private void OnTouchDown()
	{
		HandlePress();
	}

	private void HandlePress()
	{
		if (base.enabled)
		{
			tapTarget = base.transform;
			
			#if UNITY_ANDROID || UNITY_IOS
			InterfaceCamPos = m_interfaceCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
			#else
			InterfaceCamPos = m_interfaceCamera.ScreenToWorldPoint(Input.mousePosition);
			#endif
			
			if (Input.GetMouseButtonDown(0))
			{
				Invoke("BeginTap", TapStartThreshold);
			}
			else if (Input.GetMouseButtonUp(0))
			{
				HandleRelease();
			}
		}
	}

	private void OnTouchDrag()
	{
		HandleDrag();
	}

	private void HandleDrag()
	{
		if (base.enabled)
		{
			#if UNITY_ANDROID || UNITY_IOS
			Vector3 a = m_interfaceCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
			#else
			Vector3 a = m_interfaceCamera.ScreenToWorldPoint(Input.mousePosition);
			#endif
			
			if (Vector3.Distance(a, InterfaceCamPos) > m_MaxPosDistanceForTap)
			{
				CancelInvoke("BeginTap");
			}
		}
	}

	private void OnTouchReleased()
	{
		HandleRelease();
	}

	private void HandleRelease()
	{
		if (base.enabled)
		{
			if (!m_IsTapping)
			{
				CancelInvoke("BeginTap");
			}
			if (this.OnTapReleased != null)
			{
				this.OnTapReleased();
			}
			Invoke("StopTap", TapEndThreshold);
			m_IsTapping = false;
		}
	}

	private void BeginTap()
	{
		if (!m_IsTapping)
		{
			m_IsTapping = true;
			if ((bool)m_SceneryCamera)
			{
				m_SceneryCamera.eventMask = 0;
			}
			if (this.OnTapBegin != null)
			{
				this.OnTapBegin();
			}
		}
	}

	private void StopTap()
	{
		if (this.OnTapEnd != null)
		{
			this.OnTapEnd();
		}
		ResetUICamera();
	}

	public void ResetUICamera()
	{
		if ((bool)m_SceneryCamera)
		{
			m_SceneryCamera.eventMask = 1 << LayerMask.NameToLayer("Scenery");
		}
	}
}
