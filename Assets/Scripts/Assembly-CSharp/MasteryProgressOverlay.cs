using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class MasteryProgressOverlay : MonoBehaviour
{
	public UILabel m_Header;

	public UILabel m_Desc;

	public ContainerControl m_ContainerControl;

	public ContainerControl m_AllOverlaysContainerControl;

	private Camera m_InterfaceCamera;

	public UILabel m_MasteryRank;

	public UILabel m_MasteryNeededPointsText;

	public UILabel m_MasteryNeededPointsValue;

	private Vector3 initialSize;

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
	}

	public void ShowMasteryProgressOverlay(Transform root, ClassItemGameData classItem, Camera orientatedCamera)
	{
		SetContent(classItem);
		Show(root, orientatedCamera);
	}

	private void Show(Transform root, Camera orientatedCamera)
	{
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = PositionRelativeToAnchorPositionFixedOnAnchor(anchorPosition, m_OffsetLeft);
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	private void SetContent(ClassItemGameData classItem)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		m_Header.text = DIContainerInfrastructure.GetLocaService().Tr("camp_mastery_tt_title");
		m_Desc.text = DIContainerInfrastructure.GetLocaService().Tr("camp_mastery_tt_desc");
		if (classItem != null)
		{
			m_MasteryRank.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(classItem.Data.Level);
			if (classItem.Data.Level < classItem.MasteryMaxRank())
			{
				m_MasteryNeededPointsValue.gameObject.SetActive(true);
				m_MasteryNeededPointsValue.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(classItem.MasteryNeededForNextRank() - classItem.ItemValue);
				m_MasteryNeededPointsText.text = DIContainerInfrastructure.GetLocaService().Tr("camp_mastery_tt_nextrank");
			}
			else
			{
				m_MasteryNeededPointsText.text = DIContainerInfrastructure.GetLocaService().Tr("mastery_max_rank");
				m_MasteryNeededPointsValue.gameObject.SetActive(false);
			}
		}
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
