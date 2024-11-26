using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class FriendAddBlind : MonoBehaviour
{
	[SerializeField]
	private UILabel m_IndexLabel;

	[SerializeField]
	public UIInputTrigger m_AddFriend;

	[SerializeField]
	private GameObject m_BonusRoot;

	[SerializeField]
	private Vector3 m_AlternatingCharacterOffset = new Vector3(-150f, 0f, 0f);

	private SocialWindowUI m_StateMgr;

	private int m_Index;

	public void Initialize(SocialWindowUI stateMgr, int index)
	{
		m_Index = index;
		m_StateMgr = stateMgr;
		base.gameObject.name = index.ToString("000") + "_AddBlind";
		if ((bool)m_IndexLabel)
		{
			m_IndexLabel.text = (index + 1).ToString("0");
		}
		RegisterEventHandler();
	}

	public void SetAddFriendBonus(int index, GameObject bonus)
	{
		bonus.transform.parent = m_BonusRoot.transform;
		bonus.transform.localPosition = Vector3.zero + m_AlternatingCharacterOffset * GetOffset(index);
	}

	private int GetOffset(int index)
	{
		if (index % 2 == 1)
		{
			return 1;
		}
		return -1;
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if ((bool)m_AddFriend)
		{
			m_AddFriend.Clicked += AddFriendClicked;
		}
	}

	private void DeRegisterEventHandler()
	{
		if ((bool)m_AddFriend)
		{
			m_AddFriend.Clicked -= AddFriendClicked;
		}
	}

	private void AddFriendClicked()
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowLocalLoading(DIContainerInfrastructure.GetLocaService().Tr("toast_fb_operation", "Please wait..."), true);
		List<string> list = new List<string>();
		Dictionary<string, FriendGameData> friends = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends;
		DIContainerInfrastructure.GetFacebookWrapper().BeginSendRequest(DIContainerInfrastructure.GetLocaService().Tr("facebook_apprequest_message", "Play Epic Request Message"), null, DIContainerInfrastructure.GetLocaService().Tr("facebook_apprequest_title", "Play Epic Request Title"), OnAppRequestResponse);
	}

	private void OnAppRequestResponse(string text, string error)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("social_network", DIContainerInfrastructure.GetFacebookWrapper().GetNetwork());
		dictionary.Add("social_network_request_id", "RequestInvitationMessage");
		dictionary.Add("social_network_request_info", string.Format("Text: {0}, Error: {1}", text, error));
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("SocialEvent", dictionary);
		DIContainerInfrastructure.GetAsynchStatusService().HideLocalLoading();
	}

	private void OnDestroy()
	{
		DeRegisterEventHandler();
	}
}
