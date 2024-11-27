using System.Collections.Generic;
using UnityEngine;

public class GenericOverlay : MonoBehaviour
{
	private Camera m_InterfaceCamera;

	[SerializeField]
	private Transform m_Top;

	[SerializeField]
	private Transform m_Bottom;

	[SerializeField]
	private Transform m_Center;

	[SerializeField]
	private List<UISprite> m_CenterSprites;

	public ContainerControl m_ContainerControl;

	public ContainerControl m_AllOverlaysContainerControl;

	private Vector3 initialContainerControlPos;

	private Vector3 initialContainerControlSize;

	public float m_OffsetLeft = 50f;

	public AutoScalingTextBox m_TextBox;

	public AutoScalingTextBox m_HeaderTextBox;

	public float m_ArrowShiftRight = 4f;

	private Vector3 initialTopPos;

	private Vector3 initialBottomPos;

	private Vector3 initialCenterPos;

	private float initialSpriteSizeDelta;

	private void Awake()
	{
		m_InterfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
		initialContainerControlPos = m_ContainerControl.transform.localPosition;
		initialContainerControlSize = m_ContainerControl.m_Size;
		if (m_Top != null)
		{
			initialTopPos = m_Top.localPosition;
		}
		if (m_Bottom != null)
		{
			initialBottomPos = m_Bottom.localPosition;
		}
		if (m_Center != null)
		{
			initialCenterPos = m_Center.localPosition;
		}
		initialSpriteSizeDelta = initialContainerControlSize.y - m_CenterSprites[0].cachedTransform.localScale.y;
	}

	internal void ShowGenericOverlay(Transform root, string localizedText, Camera orientatedCamera)
	{
		Vector3 vector = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		m_TextBox.SetTextWithAlignment(localizedText, (!(vector.x < 0f)) ? XAlignmentTypes.Right : XAlignmentTypes.Left, YAlignmentTypes.Center);
		base.transform.localPosition = new Vector3(vector.x, vector.y, base.transform.localPosition.z);
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	internal void ShowGenericOverlay(Transform root, string localizedHeader, string localizedText, Camera orientatedCamera)
	{
		Vector3 vector = m_HeaderTextBox.SetTextAndGetBackgroundSize(localizedHeader);
		Vector3 vector2 = m_TextBox.SetTextAndGetBackgroundSize(localizedText);
		Vector3 size = new Vector3(Mathf.Max(vector.x, vector2.x), vector.y + vector2.y, 0f);
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		DebugLog.Log("Begin show GenericOverlay on Object: " + root.gameObject.name);
		m_ContainerControl.m_Size = size;
		foreach (UISprite centerSprite in m_CenterSprites)
		{
			centerSprite.height = (int)(m_ContainerControl.m_Size.y + 8f - initialSpriteSizeDelta);
		}
		m_Top.transform.localPosition = initialTopPos + new Vector3(0f, m_ContainerControl.m_Size.y / 2f - initialSpriteSizeDelta * 0.5f, 0f);
		m_Bottom.transform.localPosition = initialBottomPos - new Vector3(0f, m_ContainerControl.m_Size.y / 2f - initialSpriteSizeDelta * 0.5f, 0f);
		m_Center.transform.localPosition = initialCenterPos;
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = PositionRelativeToAnchorPositionFixedOnAnchor(anchorPosition, m_OffsetLeft);
		Vector3 textPosition = m_HeaderTextBox.textPosition;
		m_HeaderTextBox.textPosition = new Vector3(textPosition.x, m_Top.transform.localPosition.y, textPosition.z);
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnAnchor(Vector3 anchorPosition, float offset)
	{
		if (Mathf.Sign(anchorPosition.x) < 0f)
		{
			return new Vector3(-1f * Mathf.Sign(anchorPosition.x) * (m_ContainerControl.m_Size.x + offset), initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(m_ContainerControl.m_Size.y * 0.5f, (0f - m_ContainerControl.m_Size.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
		}
		return new Vector3(-1f * Mathf.Sign(anchorPosition.x) * (m_ContainerControl.m_Size.x + offset), initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(m_ContainerControl.m_Size.y * 0.5f, (0f - m_ContainerControl.m_Size.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * -1f * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (m_ContainerControl.m_Size.x * 0.5f + offset)), initialContainerControlPos.y, initialContainerControlPos.z);
	}

	public void Hide()
	{
		if (base.gameObject.activeInHierarchy)
		{
			GetComponent<Animation>().Play("InfoOverlay_Leave");
			Invoke("Disable", GetComponent<Animation>()["InfoOverlay_Leave"].length);
		}
	}

	private void Disable()
	{
		base.gameObject.SetActive(false);
	}
}
