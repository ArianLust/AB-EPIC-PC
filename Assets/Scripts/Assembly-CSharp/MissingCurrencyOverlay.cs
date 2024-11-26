using UnityEngine;

public class MissingCurrencyOverlay : MonoBehaviour
{
	private Camera m_InterfaceCamera;

	public UISprite m_Arrow;

	public UISprite m_BackgroundSprite;

	public UISprite m_IconSprite;

	public ContainerControl m_ContainerControl;

	public ContainerControl m_AllOverlaysContainerControl;

	private Vector3 initialSize;

	private Vector3 initialContainerControlPos;

	private Vector3 initialContainerControlSize;

	private Vector3 initialArrowSize;

	private Vector3 initialArrowPostition;

	public float m_OffsetLeft = 50f;

	public AutoScalingTextBox m_TextBox;

	public float m_ArrowShiftRight = 8f;

	private void Awake()
	{
		m_InterfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
		initialSize = m_ContainerControl.m_Size;
		initialContainerControlPos = m_ContainerControl.transform.localPosition;
		initialContainerControlSize = m_ContainerControl.m_Size;
		if ((bool)m_Arrow)
		{
			initialArrowSize = m_Arrow.cachedTransform.localScale;
			initialArrowPostition = m_Arrow.cachedTransform.localPosition;
		}
	}

	public void ShowGenericOverlay(Transform root, string iconAsset, string localizedText, Camera orientatedCamera, UIAtlas atlasSwitch = null)
	{
		m_TextBox.SetText(localizedText);
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		DebugLog.Log("Begin show GenericOverlay on Object: " + root.gameObject.name);
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = PositionRelativeToAnchorPositionFixedOnAnchor(anchorPosition, m_OffsetLeft);
		if ((bool)atlasSwitch)
		{
			m_IconSprite.atlas = atlasSwitch;
		}
		else if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset("GenericElements"))
		{
			GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject("GenericElements") as GameObject;
			if (gameObject != null)
			{
				m_IconSprite.atlas = gameObject.GetComponent<UIAtlas>();
			}
		}
		m_IconSprite.spriteName = iconAsset;
		if ((bool)m_Arrow)
		{
			m_Arrow.cachedTransform.localPosition = PositionArrowRelativeToAnchorPositionFixedOnAnchor(anchorPosition, m_OffsetLeft);
			m_Arrow.cachedTransform.localScale = Vector3.Scale(initialArrowSize, new Vector3(-1f * Mathf.Sign(anchorPosition.x), 1f, 1f));
		}
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnAnchor(Vector3 anchorPosition, float offset)
	{
		if (Mathf.Sign(anchorPosition.x) < 0f)
		{
			return new Vector3(-1f * Mathf.Sign(anchorPosition.x) * offset, initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(m_BackgroundSprite.cachedTransform.localScale.y * 0.5f, (0f - m_BackgroundSprite.cachedTransform.localScale.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
		}
		return new Vector3(-1f * Mathf.Sign(anchorPosition.x) * (m_BackgroundSprite.cachedTransform.localScale.x + offset), initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(m_BackgroundSprite.cachedTransform.localScale.y * 0.5f, (0f - m_BackgroundSprite.cachedTransform.localScale.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * -1f * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (m_ContainerControl.m_Size.x * 0.5f + offset)), initialContainerControlPos.y, initialContainerControlPos.z);
	}

	private Vector3 PositionArrowRelativeToAnchorPositionFixedOnAnchor(Vector3 anchorPosition, float offset)
	{
		if (Mathf.Sign(anchorPosition.x) > 0f)
		{
			return new Vector3(-1f * Mathf.Sign(anchorPosition.x) * offset - m_ArrowShiftRight, initialArrowPostition.y, initialArrowPostition.z);
		}
		return new Vector3(-1f * Mathf.Sign(anchorPosition.x) + offset, initialArrowPostition.y, initialArrowPostition.z);
	}

	private Vector3 PositionArrowRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * -1f * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (initialArrowSize.x + m_ContainerControl.m_Size.x + offset)), anchorPosition.y, m_Arrow.cachedTransform.localPosition.z);
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
