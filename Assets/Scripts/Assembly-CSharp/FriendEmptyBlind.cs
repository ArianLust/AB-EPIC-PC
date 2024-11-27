using UnityEngine;

public class FriendEmptyBlind : MonoBehaviour
{
	[SerializeField]
	private UILabel m_IndexLabel;

	[SerializeField]
	private GameObject m_BonusRoot;

	[SerializeField]
	private Vector3 m_AlternatingCharacterOffset = new Vector3(-150f, 0f, 0f);

	private SocialWindowUI m_StateMgr;

	private int m_Index;

	private void Awake()
	{
	}

	public void Initialize(SocialWindowUI stateMgr, int index)
	{
		m_Index = index;
		m_StateMgr = stateMgr;
		base.gameObject.name = index.ToString("000") + "_EmptyBlind";
		if ((bool)m_IndexLabel)
		{
			m_IndexLabel.text = (index + 1).ToString("0");
		}
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

	private void OnDestroy()
	{
	}
}
