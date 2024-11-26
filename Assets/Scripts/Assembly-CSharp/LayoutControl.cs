using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class LayoutControl : MonoBehaviour
{
	public LayoutSet m_LayoutSet = new LayoutSet();

	public List<bool> m_bFoldouts = new List<bool>();

	private Vector3 m_v3Offset = Vector3.zero;

	private float m_fAspect = 1f;

	private Camera m_Cam;

	private ContainerControl m_Container;

	private bool m_dirtyFlag;

	private void Awake()
	{
		m_dirtyFlag = true;
	}

	private void OnEnable()
	{
		m_dirtyFlag = true;
	}

	private void LateUpdate()
	{
		if ((m_dirtyFlag || (m_Cam == null && m_Container == null)) && m_LayoutSet.m_goSource != null)
		{
			if ((m_Cam = m_LayoutSet.m_goSource.GetComponent<Camera>()) != null)
			{
				m_Container = null;
				CalculatePosition();
			}
			else if ((m_Container = m_LayoutSet.m_goSource.GetComponent<ContainerControl>()) != null)
			{
				m_Cam = null;
				CalculatePosition();
			}
		}
		CalculatePosition();
	}

	public void CalculatePosition()
	{
		m_fAspect = (float)Screen.width / (float)Screen.height;
		LayoutSet layoutSet = m_LayoutSet;
		CalculateOffset(layoutSet);
		if (m_Container != null)
		{
			CalculateObjectPosInContainer(layoutSet);
		}
		else if (m_Cam != null)
		{
			CalculateObjectPosInCam(layoutSet);
		}
		m_dirtyFlag = false;
	}

	private void CalculateObjectPosInContainer(LayoutSet LS)
	{
		Vector3 v3Position = LS.m_v3Position;
		if (LS.m_IsPositionPercentX)
		{
			v3Position.x = m_Container.m_Size.x * LS.m_v3Position.x;
		}
		if (LS.m_IsPositionPercentY)
		{
			v3Position.y = m_Container.m_Size.y * LS.m_v3Position.y;
		}
		if (LS.m_IsPositionPercentZ)
		{
			v3Position.z = m_Container.m_Size.z * LS.m_v3Position.z;
		}
		v3Position += LS.m_goSource.transform.position + m_v3Offset;
		base.transform.position = v3Position;
	}

	private void CalculateObjectPosInCam(LayoutSet LS)
	{
		Vector3 v3Position = LS.m_v3Position;
		if (LS.m_IsPositionPercentX)
		{
			v3Position.x = m_Cam.orthographicSize * m_fAspect / 50f * LS.m_v3Position.x;
		}
		if (LS.m_IsPositionPercentY)
		{
			v3Position.y = m_Cam.orthographicSize / 50f * LS.m_v3Position.y;
		}
		if (LS.m_IsPositionPercentZ)
		{
			v3Position.z = 1f * LS.m_v3Position.z;
		}
		if (base.transform.parent == LS.m_goSource.transform)
		{
			v3Position += m_v3Offset;
		}
		else
		{
			v3Position += LS.m_goSource.transform.position + m_v3Offset;
		}
		base.transform.position = v3Position;
	}

	private void CalculateOffset(LayoutSet LS)
	{
		float num = 0f;
		float num2 = 0f;
		if (m_Cam != null)
		{
			num = m_Cam.orthographicSize * m_fAspect;
			num2 = m_Cam.orthographicSize;
		}
		else if (m_Container != null)
		{
			num = m_Container.m_Size.x * 0.5f;
			num2 = m_Container.m_Size.y * 0.5f;
		}
		switch (LS.m_iRelativeToScreen)
		{
		case 0:
			m_v3Offset.x = 0f - num;
			m_v3Offset.y = num2;
			m_v3Offset.z = 0f;
			break;
		case 1:
			m_v3Offset.x = 0f;
			m_v3Offset.y = num2;
			m_v3Offset.z = 0f;
			break;
		case 2:
			m_v3Offset.x = num;
			m_v3Offset.y = num2;
			m_v3Offset.z = 0f;
			break;
		case 3:
			m_v3Offset.x = 0f - num;
			m_v3Offset.y = 0f;
			m_v3Offset.z = 0f;
			break;
		case 4:
			m_v3Offset = Vector3.zero;
			break;
		case 5:
			m_v3Offset.x = num;
			m_v3Offset.y = 0f;
			m_v3Offset.z = 0f;
			break;
		case 6:
			m_v3Offset.x = 0f - num;
			m_v3Offset.y = 0f - num2;
			m_v3Offset.z = 0f;
			break;
		case 7:
			m_v3Offset.x = 0f;
			m_v3Offset.y = 0f - num2;
			m_v3Offset.z = 0f;
			break;
		case 8:
			m_v3Offset.x = num;
			m_v3Offset.y = 0f - num2;
			m_v3Offset.z = 0f;
			break;
		default:
			DebugLog.Log("No clue what to do");
			break;
		}
		if (m_Container != null)
		{
			m_v3Offset += m_Container.m_Offset;
		}
	}

	public void SetDirtyFlag()
	{
		m_dirtyFlag = true;
	}
}
