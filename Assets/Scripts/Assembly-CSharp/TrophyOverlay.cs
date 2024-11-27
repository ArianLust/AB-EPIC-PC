using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Models.Character;
using UnityEngine;

public class TrophyOverlay : MonoBehaviour
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

	public UILabel m_SeasonTrophyLabel;

	public UILabel m_SeasonTrophyDescription;

	public UITable m_TrophyTable;

	public GameObject m_OwnTrophyPrefab;

	public GameObject m_OtherTrophyPrefab;

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

	internal void ShowTrophyOverlay(Transform root, TrophyData Trophy, Camera orientatedCamera)
	{
		StartCoroutine(ConstructArenaLeagueOverlay(root, Trophy, orientatedCamera));
	}

	internal IEnumerator ConstructArenaLeagueOverlay(Transform root, TrophyData Trophy, Camera orientatedCamera)
	{
		m_SeasonTrophyLabel.text = DIContainerInfrastructure.GetLocaService().Tr("pvp_trophy_s" + Trophy.Seasonid.ToString("00") + "_l" + Trophy.FinishedLeagueId.ToString("00") + "_name");
		m_SeasonTrophyDescription.text = DIContainerInfrastructure.GetLocaService().Tr("pvp_trophy_tt_desc");
		foreach (Transform t in m_TrophyTable.transform)
		{
			Object.Destroy(t.gameObject);
		}
		yield return new WaitForEndOfFrame();
		PvPSeasonManagerGameData pvpData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		for (int i = 1; i <= pvpData.Balancing.MaxLeague; i++)
		{
			GameObject cIcon = ((Trophy.FinishedLeagueId != i) ? Object.Instantiate(m_OtherTrophyPrefab) : Object.Instantiate(m_OwnTrophyPrefab));
			cIcon.transform.parent = m_TrophyTable.transform;
			cIcon.transform.localPosition = Vector3.zero;
			cIcon.transform.localScale = m_TrophyTable.transform.localScale;
			UISprite iconSprite = cIcon.transform.Find("Animation/Icon").GetComponent<UISprite>();
			string LeagueName = "Wood";
			switch (i)
			{
			case 1:
				LeagueName = "Wood";
				break;
			case 2:
				LeagueName = "Stone";
				break;
			case 3:
				LeagueName = "Silver";
				break;
			case 4:
				LeagueName = "Gold";
				break;
			case 5:
				LeagueName = "Platinum";
				break;
			case 6:
				LeagueName = "Diamond";
				break;
			}
			iconSprite.spriteName = "Season" + ((Trophy.Seasonid - 1) % 5 + 1) + LeagueName;
			iconSprite.MakePixelPerfect();
		}
		m_TrophyTable.Reposition();
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
