using ABH.GameDatas;
using UnityEngine;

public class ObjectiveOverlay : MonoBehaviour
{
	private Camera m_InterfaceCamera;

	public UILabel[] m_ProgressLabel;

	public UILabel[] m_DescriptionLabel;

	public ContainerControl m_ContainerControl;

	public ContainerControl m_AllOverlaysContainerControl;

	private Vector3 initialContainerControlPos;

	private Vector3 initialContainerControlSize;

	private void Awake()
	{
		m_InterfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
		initialContainerControlPos = m_ContainerControl.transform.localPosition;
		initialContainerControlSize = m_ContainerControl.m_Size;
	}

	public void ShowOverlay(Transform root, Camera orientatedCamera)
	{
		for (int i = 0; i < 3; i++)
		{
			m_DescriptionLabel[i].text = "Objective not found";
			m_ProgressLabel[i].text = string.Empty;
		}
		foreach (PvPObjectivesGameData dailyObjective in DIContainerLogic.GetPvpObjectivesService().GetDailyObjectives())
		{
			int num = 0;
			if (dailyObjective.GetDifficulty() == "normal")
			{
				num = 1;
			}
			else if (dailyObjective.GetDifficulty() == "hard")
			{
				num = 2;
			}
			m_ProgressLabel[num].text = dailyObjective.Data.Progress + "/" + dailyObjective.Amount;
			m_DescriptionLabel[num].text = dailyObjective.GetTooltipText();
		}
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = PositionRelativeToAnchorPositionFixedOnAnchor(anchorPosition, 0f);
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
