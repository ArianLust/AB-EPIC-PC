using ABH.GameDatas;
using UnityEngine;

public class FriendProgressIndicator : MonoBehaviour
{
	[SerializeField]
	private GameObject m_MultipleFriendsIndicatorSprite;

	[SerializeField]
	private FriendInfoElement m_FriendInfo;

	private FriendGameData m_Model;

	public void SetModel(FriendGameData friend)
	{
		if (m_Model != null && m_Model.FriendLevel >= friend.FriendLevel)
		{
			if ((bool)m_MultipleFriendsIndicatorSprite)
			{
				m_MultipleFriendsIndicatorSprite.gameObject.SetActive(true);
			}
		}
		else
		{
			m_Model = friend;
			m_FriendInfo.SetDefault();
			m_FriendInfo.SetModel(m_Model);
			GetComponent<Animation>().Play("FriendMarker_Show");
			GetComponent<Animation>().PlayQueued("FriendMarker_Idle");
		}
	}
}
