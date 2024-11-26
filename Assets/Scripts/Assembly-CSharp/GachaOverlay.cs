using UnityEngine;

public class GachaOverlay : MonoBehaviour
{
	private Camera m_InterfaceCamera;

	[SerializeField]
	private UILabel m_Header;

	[SerializeField]
	private bool m_IsPvp;

	private void Awake()
	{
		m_InterfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
	}

	internal void ShowGachaOverlay(Transform root, Camera orientatedCamera, bool isAdvanced)
	{
		Vector3 vector = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		ABHLocaService locaService = DIContainerInfrastructure.GetLocaService();
		if (m_IsPvp)
		{
			if (isAdvanced)
			{
				m_Header.text = locaService.Tr("advarenagacha_tt_header_01");
			}
			else
			{
				m_Header.text = locaService.Tr("arenagacha_tt_header_01");
			}
		}
		else if (isAdvanced)
		{
			m_Header.text = locaService.Tr("advgacha_tt_header_01");
		}
		else
		{
			m_Header.text = locaService.Tr("gacha_tt_header_01");
		}
		DebugLog.Log("Begin show GachaOverlay on Object: " + root.gameObject.name);
		base.transform.localPosition = new Vector3(vector.x, vector.y, base.transform.localPosition.z);
		GetComponent<Animation>().Play("InfoOverlay_Enter");
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
