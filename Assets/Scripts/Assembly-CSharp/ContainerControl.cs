using System.Collections.Generic;
using UnityEngine;

public class ContainerControl : MonoBehaviour
{
	public enum eAxis
	{
		xy = 0,
		yz = 1,
		zx = 2
	}

	public Color m_Color = Color.magenta;

	public Vector3 m_Size = new Vector3(10f, 10f, 1f);

	public Vector3 m_RelativeSize = new Vector3(1f, 1f, 1f);

	public Vector3 m_MinSize = new Vector3(0f, 0f, 1f);

	public Vector3 m_MaxSize = new Vector3(2048f, 2048f, 1f);

	public LayoutSizeModes m_WidthMode;

	public LayoutSizeModes m_HeightMode;

	public Vector3 m_Offset = Vector3.zero;

	public Camera m_RenderCamera;

	public eAxis m_Axis;

	public List<TableLine> m_Rows = new List<TableLine>();

	public List<TableLine> m_Columns = new List<TableLine>();

	public int m_newRowCount;

	public int m_newColumnCount;

	public int m_iAnchorPoint = 4;

	public string[] m_sAnchorNames = new string[9] { "UL", "UM", "UR", "ML", "MM", "MR", "BL", "BM", "BR" };

	public GizmoVisibility m_GizmoVisibilty = GizmoVisibility.Always;

	public bool m_IsHiddenAtStart;

	public bool m_IsDisabledAtStart;

	private void Awake()
	{
		LateUpdate();
		UpdateOffset();
	}

	private void Start()
	{
		CheckStartupOptions();
	}

	private void CheckStartupOptions()
	{
		if (m_IsHiddenAtStart)
		{
			MeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				meshRenderer.enabled = false;
			}
		}
		if (m_IsDisabledAtStart)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void LateUpdate()
	{
		if (m_RenderCamera == null)
		{
			m_RenderCamera = UICamera.mainCamera;
		}
		if (m_RenderCamera != null && (m_WidthMode == LayoutSizeModes.ScreenPrecent || m_HeightMode == LayoutSizeModes.ScreenPrecent))
		{
			if (m_WidthMode == LayoutSizeModes.ScreenPrecent)
			{
				float num = (float)Screen.width / (float)Screen.height;
				m_Size.x = Mathf.Clamp(m_RenderCamera.orthographicSize * num * 2f * m_RelativeSize.x, m_MinSize.x, m_MaxSize.x);
			}
			if (m_HeightMode == LayoutSizeModes.ScreenPrecent)
			{
				m_Size.y = Mathf.Clamp(m_RenderCamera.orthographicSize * 2f * m_RelativeSize.y, m_MinSize.y, m_MaxSize.y);
			}
		}
	}

	public void UpdateLayoutChildren()
	{
		LayoutControl[] componentsInChildren = GetComponentsInChildren<LayoutControl>(true);
		foreach (LayoutControl layoutControl in componentsInChildren)
		{
			layoutControl.CalculatePosition();
		}
	}

	public void UpdateOffset()
	{
		switch (m_iAnchorPoint)
		{
		case 0:
			if (m_Axis == eAxis.xy)
			{
				m_Offset.x = m_Size.x * 0.5f;
				m_Offset.y = (0f - m_Size.y) * 0.5f;
				m_Offset.z = 0f;
			}
			else if (m_Axis == eAxis.yz)
			{
				m_Offset.x = 0f;
				m_Offset.y = (0f - m_Size.y) * 0.5f;
				m_Offset.z = m_Size.x * 0.5f;
			}
			else if (m_Axis == eAxis.zx)
			{
				m_Offset.x = m_Size.x * 0.5f;
				m_Offset.y = 0f;
				m_Offset.z = (0f - m_Size.y) * 0.5f;
			}
			break;
		case 1:
			if (m_Axis == eAxis.xy)
			{
				m_Offset.x = 0f;
				m_Offset.y = (0f - m_Size.y) * 0.5f;
				m_Offset.z = 0f;
			}
			else if (m_Axis == eAxis.yz)
			{
				m_Offset.x = 0f;
				m_Offset.y = (0f - m_Size.y) * 0.5f;
				m_Offset.z = 0f;
			}
			else if (m_Axis == eAxis.zx)
			{
				m_Offset.x = 0f;
				m_Offset.y = 0f;
				m_Offset.z = (0f - m_Size.y) * 0.5f;
			}
			break;
		case 2:
			if (m_Axis == eAxis.xy)
			{
				m_Offset.x = (0f - m_Size.x) * 0.5f;
				m_Offset.y = (0f - m_Size.y) * 0.5f;
				m_Offset.z = 0f;
			}
			else if (m_Axis == eAxis.yz)
			{
				m_Offset.x = 0f;
				m_Offset.y = (0f - m_Size.y) * 0.5f;
				m_Offset.z = (0f - m_Size.x) * 0.5f;
			}
			else if (m_Axis == eAxis.zx)
			{
				m_Offset.x = (0f - m_Size.x) * 0.5f;
				m_Offset.y = 0f;
				m_Offset.z = (0f - m_Size.y) * 0.5f;
			}
			break;
		case 3:
			if (m_Axis == eAxis.xy)
			{
				m_Offset.x = m_Size.x * 0.5f;
				m_Offset.y = 0f;
				m_Offset.z = 0f;
			}
			else if (m_Axis == eAxis.yz)
			{
				m_Offset.x = 0f;
				m_Offset.y = 0f;
				m_Offset.z = m_Size.x * 0.5f;
			}
			else if (m_Axis == eAxis.zx)
			{
				m_Offset.x = m_Size.x * 0.5f;
				m_Offset.y = 0f;
				m_Offset.z = 0f;
			}
			break;
		case 4:
			m_Offset = Vector3.zero;
			break;
		case 5:
			if (m_Axis == eAxis.xy)
			{
				m_Offset.x = (0f - m_Size.x) * 0.5f;
				m_Offset.y = 0f;
				m_Offset.z = 0f;
			}
			else if (m_Axis == eAxis.yz)
			{
				m_Offset.x = 0f;
				m_Offset.y = 0f;
				m_Offset.z = (0f - m_Size.x) * 0.5f;
			}
			else if (m_Axis == eAxis.zx)
			{
				m_Offset.x = (0f - m_Size.x) * 0.5f;
				m_Offset.y = 0f;
				m_Offset.z = 0f;
			}
			break;
		case 6:
			if (m_Axis == eAxis.xy)
			{
				m_Offset.x = m_Size.x * 0.5f;
				m_Offset.y = m_Size.y * 0.5f;
				m_Offset.z = 0f;
			}
			else if (m_Axis == eAxis.yz)
			{
				m_Offset.x = 0f;
				m_Offset.y = m_Size.y * 0.5f;
				m_Offset.z = m_Size.x * 0.5f;
			}
			else if (m_Axis == eAxis.zx)
			{
				m_Offset.x = m_Size.x * 0.5f;
				m_Offset.y = 0f;
				m_Offset.z = m_Size.y * 0.5f;
			}
			break;
		case 7:
			if (m_Axis == eAxis.xy)
			{
				m_Offset.x = 0f;
				m_Offset.y = m_Size.y * 0.5f;
				m_Offset.z = 0f;
			}
			else if (m_Axis == eAxis.yz)
			{
				m_Offset.x = 0f;
				m_Offset.y = m_Size.y * 0.5f;
				m_Offset.z = 0f;
			}
			else if (m_Axis == eAxis.zx)
			{
				m_Offset.x = 0f;
				m_Offset.y = 0f;
				m_Offset.z = m_Size.y * 0.5f;
			}
			break;
		case 8:
			if (m_Axis == eAxis.xy)
			{
				m_Offset.x = (0f - m_Size.x) * 0.5f;
				m_Offset.y = m_Size.y * 0.5f;
				m_Offset.z = 0f;
			}
			else if (m_Axis == eAxis.yz)
			{
				m_Offset.x = 0f;
				m_Offset.y = m_Size.y * 0.5f;
				m_Offset.z = (0f - m_Size.x) * 0.5f;
			}
			else if (m_Axis == eAxis.zx)
			{
				m_Offset.x = (0f - m_Size.x) * 0.5f;
				m_Offset.y = 0f;
				m_Offset.z = m_Size.y * 0.5f;
			}
			break;
		default:
			DebugLog.Log("No clue what to do");
			break;
		}
	}

	public void UpdateGrid()
	{
		AdjustListeSizeByCount(m_newColumnCount, m_Columns);
		AdjustListeSizeByCount(m_newRowCount, m_Rows);
	}

	private void AdjustListeSizeByCount(int newCount, List<TableLine> list)
	{
		if (list.Count > newCount)
		{
			list.RemoveRange(m_newColumnCount, list.Count - newCount);
		}
		if (list.Count < newCount)
		{
			for (int i = 0; i < m_newColumnCount - list.Count; i++)
			{
				list.Add(new TableLine());
			}
		}
	}
}
