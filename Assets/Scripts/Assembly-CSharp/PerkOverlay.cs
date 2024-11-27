using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class PerkOverlay : MonoBehaviour
{
	private Camera m_InterfaceCamera;

	public UILabel m_Header;

	public UILabel m_PerkDesc;

	public UISprite m_PerkIcon;

	public UISprite m_Arrow;

	public ContainerControl m_ContainerControl;

	public ContainerControl m_AllOverlaysContainerControl;

	private Vector3 initialSize;

	public Vector2 blindSize;

	private Vector3 initialContainerControlPos;

	private Vector3 initialContainerControlSize;

	private Vector3 initialArrowSize;

	public float m_OffsetLeft = 50f;

	private void Awake()
	{
		m_InterfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
		initialSize = m_ContainerControl.m_Size;
		initialContainerControlPos = m_ContainerControl.transform.localPosition;
		initialContainerControlSize = m_ContainerControl.m_Size;
		if ((bool)m_Arrow)
		{
			initialArrowSize = m_Arrow.cachedTransform.localScale;
		}
	}

	public void ShowPerkOverlay(Transform root, EquipmentGameData equip, Camera orientatedCamera)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if ((bool)m_Header)
		{
			m_Header.text = EquipmentGameData.GetPerkName(equip);
		}
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		m_PerkDesc.text = EquipmentGameData.GetPerkDesc(equip);
		m_PerkIcon.spriteName = EquipmentGameData.GetPerkIcon(equip);
		DebugLog.Log("Begin show PerkOverlay " + DIContainerInfrastructure.GetLocaService().GetPerkName(equip.BalancingData.Perk.Type));
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = PositionRelativeToAnchorPositionFixedOnAnchor(anchorPosition, m_OffsetLeft);
		if ((bool)m_Arrow)
		{
			m_Arrow.cachedTransform.localPosition = PositionArrowRelativeToAnchorPositionFixedOnScreen(anchorPosition, m_OffsetLeft);
			m_Arrow.cachedTransform.localScale = Vector3.Scale(initialArrowSize, new Vector3(-1f * Mathf.Sign(anchorPosition.x), 1f, 1f));
			m_Arrow.gameObject.SetActive(false);
		}
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnAnchor(Vector3 anchorPosition, float offset)
	{
		if (Mathf.Sign(anchorPosition.x) < 0f)
		{
			return new Vector3(initialContainerControlPos.x, initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(initialContainerControlSize.y * 0.5f, (0f - initialContainerControlSize.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
		}
		return new Vector3(0f - initialContainerControlPos.x, initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(initialContainerControlSize.y * 0.5f, (0f - initialContainerControlSize.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * -1f * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (m_ContainerControl.m_Size.x * 0.5f + offset)), initialContainerControlPos.y, initialContainerControlPos.z);
	}

	private Vector3 PositionArrowRelativeToAnchorPositionFixedOnAnchor(Vector3 anchorPosition)
	{
		return new Vector3(anchorPosition.x + -1f * Mathf.Sign(anchorPosition.x) * initialArrowSize.x, anchorPosition.y, m_Arrow.cachedTransform.localPosition.z);
	}

	private Vector3 PositionArrowRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * -1f * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (initialArrowSize.x + m_ContainerControl.m_Size.x + offset)), anchorPosition.y, m_Arrow.cachedTransform.localPosition.z);
	}

	private float SkillOverlayOffset(int numberOfSkills)
	{
		return (float)(3 - numberOfSkills) * blindSize.y;
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
