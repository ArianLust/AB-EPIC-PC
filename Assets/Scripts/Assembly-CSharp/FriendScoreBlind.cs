using ABH.GameDatas;
using UnityEngine;

public class FriendScoreBlind : MonoBehaviour
{
	[SerializeField]
	private UILabel m_RankLabel;

	[SerializeField]
	private UILabel m_ScoreLabel;

	[SerializeField]
	private FriendInfoElement m_FriendInfo;

	private FriendGameData m_Model;

	public void SetModel(FriendGameData friend, int rank, int score)
	{
		m_Model = friend;
		m_FriendInfo.SetDefault();
		m_FriendInfo.SetModel(m_Model);
		base.gameObject.name = score.ToString("00") + "_" + base.gameObject.name;
		if ((bool)m_ScoreLabel)
		{
			m_ScoreLabel.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(score);
		}
		if ((bool)m_RankLabel)
		{
			m_RankLabel.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(rank);
		}
	}
}
