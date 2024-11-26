using UnityEngine;

public class PopupWP8AchievementsUI : MonoBehaviour
{
	[SerializeField]
	private UIGrid m_achievementList;

	[SerializeField]
	private GameObject m_listEntryPrefab;

	[SerializeField]
	private UILabel m_achievementProgress;

	[SerializeField]
	private UILabel m_gamerscoreProgress;

	[SerializeField]
	private UIInputTrigger m_closeButton;

	public void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_popupWp8Achievements = this;
	}
}
