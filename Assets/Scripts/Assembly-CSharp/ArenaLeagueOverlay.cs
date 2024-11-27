using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class ArenaLeagueOverlay : MonoBehaviour
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

	public UILabel m_OwnLeagueTitle;

	public UILabel m_OwnLeagueRank;

	public UITable m_LeagueTable;

	public GameObject m_OwnLeaguePrefab;

	public GameObject m_OtherLeaguePrefab;

	private Vector3 initialSize;

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

	private Vector3 initialHeaderPos;

	private void Awake()
	{
		m_InterfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
		initialSize = m_ContainerControl.m_Size;
		initialContainerControlPos = m_ContainerControl.transform.localPosition;
		initialContainerControlSize = m_ContainerControl.m_Size;
		initialTopPos = m_Top.localPosition;
		initialBottomPos = m_Bottom.localPosition;
		initialCenterPos = m_Center.localPosition;
		initialSpriteSizeDelta = initialContainerControlSize.y - m_CenterSprites[0].cachedTransform.localScale.y;
		if ((bool)m_HeaderTextBox)
		{
			initialHeaderPos = m_HeaderTextBox.transform.localPosition;
		}
	}

	internal void ShowArenaLeagueOverlay(Transform root, PvPSeasonManagerGameData pvpData, int league, int rank, Camera orientatedCamera)
	{
		StartCoroutine(ConstructArenaLeagueOverlay(root, pvpData, league, rank, orientatedCamera));
	}

	internal IEnumerator ConstructArenaLeagueOverlay(Transform root, PvPSeasonManagerGameData pvpData, int league, int rank, Camera orientatedCamera)
	{
		m_OwnLeagueTitle.text = DIContainerInfrastructure.GetLocaService().GetLeagueName(league);
		m_OwnLeagueRank.text = "#" + rank;
		foreach (Transform t in m_LeagueTable.transform)
		{
			Object.Destroy(t.gameObject);
		}
		yield return new WaitForEndOfFrame();
		for (int i = 1; i <= pvpData.Balancing.MaxLeague; i++)
		{
			GameObject cIcon = ((league != i) ? Object.Instantiate(m_OtherLeaguePrefab) : Object.Instantiate(m_OwnLeaguePrefab));
			cIcon.transform.parent = m_LeagueTable.transform;
			cIcon.transform.localPosition = Vector3.zero;
			cIcon.transform.localScale = m_LeagueTable.transform.localScale;
			UISprite iconSprite = cIcon.transform.Find("Icon").GetComponent<UISprite>();
			iconSprite.spriteName = PvPSeasonManagerGameData.GetLeagueAssetName(i);
		}
		m_LeagueTable.Reposition();
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
