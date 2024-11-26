using UnityEngine;

[RequireComponent(typeof(UIScrollView))]
public class PanelClippingLayoutTLBRControl : MonoBehaviour
{
	[SerializeField]
	private Transform m_Top;

	[SerializeField]
	private Transform m_Bottom;

	public UIScrollView m_DragPanel;

	private Vector2 m_Size;

	private Vector3 cachedPos;

	private Vector3 initialDragPanelPosition;

	private float centery;

	private float yOffset;

	private float scale;

	private void Awake()
	{
		initialDragPanelPosition = m_DragPanel.transform.position;
	}

	public void Start()
	{
		Recalculate();
	}

	private void Recalculate()
	{
		scale = Vector2.Distance(new Vector2(0f, m_Top.position.y), new Vector2(0f, m_Bottom.position.y));
		centery = m_Top.position.y - scale / 2f;
		yOffset = initialDragPanelPosition.y - centery;
		m_DragPanel.panel.clipRange = new Vector4(m_DragPanel.panel.clipRange.x, yOffset, m_DragPanel.panel.clipRange.z, scale);
		m_DragPanel.transform.position = initialDragPanelPosition;
		m_DragPanel.RestrictWithinBounds(false);
	}

	private void Update()
	{
		if (cachedPos != m_Bottom.position)
		{
			cachedPos = m_Bottom.position;
			Recalculate();
		}
	}
}
