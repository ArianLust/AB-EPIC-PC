using System.Collections;
using UnityEngine;

public class FriendArenaCampMenuUI : MonoBehaviour
{
	public UIInputTrigger m_BackButton;

	public Animation m_BackButtonAnimation;

	public FriendInfoElement m_FriendInfo;

	private BaseCampStateMgr m_CampStateMgr;

	private IEnumerator Start()
	{
		GetComponent<UIPanel>().enabled = true;
		yield return StartCoroutine(EnterCoroutine());
	}

	public void Enter()
	{
		StartCoroutine(EnterCoroutine());
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		m_BackButtonAnimation.Play("Button_Medium_BL_Enter");
		if (ClientInfo.InspectedFriend != null)
		{
			m_FriendInfo.SetDefault();
			m_FriendInfo.SetModel(ClientInfo.InspectedFriend);
		}
		yield return new WaitForSeconds(m_BackButtonAnimation["Button_Medium_BL_Enter"].length);
		RegisterEventHandlers();
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandlers();
		m_BackButtonAnimation.Play("Button_Medium_BL_Leave");
		yield return new WaitForSeconds(m_BackButtonAnimation["Button_Medium_BL_Leave"].length);
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		if ((bool)m_BackButton)
		{
			m_BackButton.Clicked += BackButton_Clicked;
		}
	}

	private void DeRegisterEventHandlers()
	{
		if ((bool)m_BackButton)
		{
			m_BackButton.Clicked -= BackButton_Clicked;
		}
	}

	private void BackButton_Clicked()
	{
		DeRegisterEventHandlers();
		StartCoroutine(LeaveCoroutine());
		DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreen();
	}

	private void OnDestroy()
	{
		DeRegisterEventHandlers();
	}

	public void SetCampStateMgr(ArenaCampStateMgr mgr)
	{
		m_CampStateMgr = mgr;
	}
}
