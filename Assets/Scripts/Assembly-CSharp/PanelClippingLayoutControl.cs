using UnityEngine;

[RequireComponent(typeof(UIScrollView))]
public class PanelClippingLayoutControl : MonoBehaviour
{
	[SerializeField]
	private ContainerControl m_ContainerControll;

	[SerializeField]
	private Vector2 m_Scale = new Vector2(1f, 0f);

	[SerializeField]
	private Vector2 m_RelativeOffset = new Vector2(0f, 0f);

	[SerializeField]
	private bool m_ChangeCenter;

	private UIScrollView m_DragPanel;

	private Vector2 m_Size;

	private void Awake()
	{
		m_DragPanel = GetComponent<UIScrollView>();
	}

	public void Start()
	{
		Vector2 size = new Vector2(m_ContainerControll.m_Size.x * m_Scale.x, m_ContainerControll.m_Size.y * m_Scale.y);
		Vector2 vector = new Vector2(m_DragPanel.panel.baseClipRegion.x, m_DragPanel.panel.baseClipRegion.y);
		if (m_ChangeCenter)
		{
			vector = new Vector2(size.x * m_RelativeOffset.x, size.y * m_RelativeOffset.y);
		}
		m_Size = size;
		m_DragPanel.panel.baseClipRegion = new Vector4(vector.x, vector.y, (!(m_Size.x > 0f)) ? m_DragPanel.panel.baseClipRegion.z : m_Size.x, (!(m_Size.y > 0f)) ? m_DragPanel.panel.baseClipRegion.w : m_Size.y);
	}
}
