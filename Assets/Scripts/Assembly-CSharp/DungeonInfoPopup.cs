using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonInfoPopup : MonoBehaviour
{
	public struct DungeonInfo
	{
		public int lowPrice;

		public int highPrice;

		public string LocalizedName;

		public bool isLocked;

		public bool isFree;
	}

	[SerializeField]
	private UIInputTrigger m_CloseButton;

	[SerializeField]
	private Animation m_HeaderAnimation;

	[SerializeField]
	private Animation m_ContentAnimation;

	[SerializeField]
	private GameObject m_FooterObject;

	[SerializeField]
	private List<UIInputTrigger> m_WeekDayButtons;

	[SerializeField]
	private Animator[] m_tabAnimators;

	[SerializeField]
	private GameObject m_LockedStateObject;

	[SerializeField]
	private Transform m_DungeonLockedIconContainer;

	[SerializeField]
	private UILabel m_LockedInfoLabel;

	[SerializeField]
	private GameObject m_UnlockedStateObject;

	[SerializeField]
	private UIInputTrigger m_CheckOutButton;

	[SerializeField]
	private UILabel m_DungeonNameLabel;

	[SerializeField]
	private UILabel m_MonsterDescriptionLabel;

	[SerializeField]
	private UILabel m_DungeonNormalCoinAmountLabel;

	[SerializeField]
	private UILabel m_DungeonEliteCoinAmountLabel;

	[SerializeField]
	private Transform m_MonsterIconContainer;

	[SerializeField]
	private Transform m_DungeonIconContainer;

	[SerializeField]
	private Animation m_FooterAnimation;

	private Animation m_currentActiveTab;

	private List<DungeonInfo> m_dungeonInfos;

	private int m_currentSelectedDungeonId;

	private void OnDestroy()
	{
		DeregisterEventHandler();
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, ClosePopup);
		m_CloseButton.Clicked += ClosePopup;
		m_CheckOutButton.Clicked += CheckOutDungeon;
		m_WeekDayButtons[0].Clicked += MondayClicked;
		m_WeekDayButtons[1].Clicked += TuesdayClicked;
		m_WeekDayButtons[2].Clicked += WednesdayClicked;
		m_WeekDayButtons[3].Clicked += ThursdayClicked;
		m_WeekDayButtons[4].Clicked += FridayClicked;
		m_WeekDayButtons[5].Clicked += SaturdayClicked;
		m_WeekDayButtons[6].Clicked += SundayClicked;
	}

	private void DeregisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
		m_CloseButton.Clicked -= ClosePopup;
		m_CheckOutButton.Clicked -= CheckOutDungeon;
		m_WeekDayButtons[0].Clicked -= MondayClicked;
		m_WeekDayButtons[1].Clicked -= TuesdayClicked;
		m_WeekDayButtons[2].Clicked -= WednesdayClicked;
		m_WeekDayButtons[3].Clicked -= ThursdayClicked;
		m_WeekDayButtons[4].Clicked -= FridayClicked;
		m_WeekDayButtons[5].Clicked -= SaturdayClicked;
		m_WeekDayButtons[6].Clicked -= SundayClicked;
	}

	public void ClosePopup()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine("LeaveCoroutine");
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		DeregisterEventHandler();
		m_HeaderAnimation.Play("Header_Leave");
		m_ContentAnimation.Play("Categories_Leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		yield return new WaitForSeconds(m_HeaderAnimation["Header_Leave"].length);
		base.gameObject.SetActive(false);
	}

	public void Show(List<DungeonInfo> dungeonList)
	{
		StopCoroutine("LeaveCoroutine");
		base.gameObject.SetActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		m_dungeonInfos = dungeonList;
		if (m_dungeonInfos.Count == m_tabAnimators.Length)
		{
			int id = 0;
			for (int i = 0; i < m_tabAnimators.Length; i++)
			{
				m_tabAnimators[i].SetBool("Locked", m_dungeonInfos[i].isLocked);
				m_tabAnimators[i].SetBool("Free", m_dungeonInfos[i].isFree);
				m_tabAnimators[i].SetBool("Active", false);
			}
			OnDungeonTabClicked(id);
		}
		else
		{
			Debug.LogError("DungeonInfo: Dungeon count is unequal to linked tabs. Please check!");
		}
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		yield return new WaitForSeconds(0.1f);
		m_HeaderAnimation.Play("Header_Enter");
		m_ContentAnimation.Play("Categories_Enter");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		yield return new WaitForSeconds(m_HeaderAnimation["Header_Enter"].length);
		RegisterEventHandler();
	}

	private void OnDungeonTabClicked(int id)
	{
		m_tabAnimators[m_currentSelectedDungeonId].SetBool("Active", false);
		m_currentSelectedDungeonId = id;
		m_tabAnimators[id].SetBool("Active", true);
		DungeonInfo dungeonInfo = m_dungeonInfos[id];
		if (dungeonInfo.isLocked)
		{
			m_FooterObject.SetActive(false);
			m_UnlockedStateObject.SetActive(false);
			m_LockedStateObject.SetActive(true);
			m_LockedInfoLabel.text = DIContainerInfrastructure.GetLocaService().Tr("dailydungeonsinfo_desc_locked_" + id);
			if (m_DungeonLockedIconContainer.childCount > 0)
			{
				Object.Destroy(m_DungeonLockedIconContainer.GetChild(0).gameObject);
			}
			DIContainerInfrastructure.GetBundlePrefabAssetProvider().InstantiateObject("DungeonPreview_" + id, m_DungeonLockedIconContainer, Vector3.zero, Quaternion.identity, false);
			return;
		}
		m_FooterObject.SetActive(true);
		m_UnlockedStateObject.SetActive(true);
		m_LockedStateObject.SetActive(false);
		m_FooterAnimation.Play((!dungeonInfo.isFree) ? "Unlocked" : "Free");
		m_DungeonNormalCoinAmountLabel.text = dungeonInfo.lowPrice.ToString();
		m_DungeonEliteCoinAmountLabel.text = dungeonInfo.highPrice.ToString();
		m_DungeonNameLabel.text = dungeonInfo.LocalizedName;
		m_MonsterDescriptionLabel.text = DIContainerInfrastructure.GetLocaService().Tr("dailydungeonsinfo_enemies_desc_" + id);
		if (m_DungeonIconContainer.childCount > 0)
		{
			Object.Destroy(m_DungeonIconContainer.GetChild(0).gameObject);
		}
		if (m_MonsterIconContainer.childCount > 0)
		{
			Object.Destroy(m_MonsterIconContainer.GetChild(0).gameObject);
		}
		DIContainerInfrastructure.GetBundlePrefabAssetProvider().InstantiateObject("Dungeon_" + id, m_DungeonIconContainer, Vector3.zero, Quaternion.identity, false);
		DIContainerInfrastructure.GetBundlePrefabAssetProvider().InstantiateObject("Enemies_" + id, m_MonsterIconContainer, Vector3.zero, Quaternion.identity, false);
	}

	private void CheckOutDungeon()
	{
		DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
		DIContainerInfrastructure.GetCoreStateMgr().m_ZoomToDungeonId = m_currentSelectedDungeonId;
	}

	private void MondayClicked()
	{
		OnDungeonTabClicked(0);
	}

	private void TuesdayClicked()
	{
		OnDungeonTabClicked(1);
	}

	private void WednesdayClicked()
	{
		OnDungeonTabClicked(2);
	}

	private void ThursdayClicked()
	{
		OnDungeonTabClicked(3);
	}

	private void FridayClicked()
	{
		OnDungeonTabClicked(4);
	}

	private void SaturdayClicked()
	{
		OnDungeonTabClicked(5);
	}

	private void SundayClicked()
	{
		OnDungeonTabClicked(6);
	}
}
