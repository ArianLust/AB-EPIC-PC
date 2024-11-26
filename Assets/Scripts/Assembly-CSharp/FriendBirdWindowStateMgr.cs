using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Services.Logic;
using ABH.Shared.BalancingData;
using ABH.Shared.Models;
using ABH.Shared.Models.Character;
using UnityEngine;

public class FriendBirdWindowStateMgr : MonoBehaviour
{
	private FriendListType m_FriendListType;

	[SerializeField]
	public UIInputTrigger m_LeftButton;

	[SerializeField]
	public UIInputTrigger m_RightButton;

	[SerializeField]
	private UIGrid m_Grid;

	[SerializeField]
	private UIScrollView m_Panel;

	[SerializeField]
	private UIInputTrigger m_CloseButton;

	[SerializeField]
	private GameObject m_UseFriendBirdRoot;

	[SerializeField]
	public CoinBarController m_LuckyCoinController;

	[SerializeField]
	private GameObject m_FacebookLoginRoot;

	[SerializeField]
	private UIInputTrigger m_FacebookLoginButton;

	[SerializeField]
	private FriendGetBirdBlind m_FriendVisitingBlind;

	[SerializeField]
	private FriendGetBirdBlind m_FriendGetBirdBlind;

	[SerializeField]
	private Animation m_ListAnimation;

	[SerializeField]
	private Animation m_HeaderAnimation;

	[SerializeField]
	private Animation m_FooterAnimation;

	[SerializeField]
	private GameObject m_FooterVisitRoot;

	[SerializeField]
	private GameObject m_FooterGetBirdRoot;

	[SerializeField]
	private UILabel m_PageLabel;

	[SerializeField]
	private FriendBonusDisplay m_ClassFriendBonus;

	[SerializeField]
	private FriendBonusDisplay m_StatFriendBonus;

	[SerializeField]
	private Camera m_characterCamera;

	private Action m_ReEnterAction;

	private Action<FriendGameData> m_SelectFriendAction;

	private List<FriendGameData> m_FriendsList = new List<FriendGameData>();

	private List<FriendGameData> m_LowLevelNpcs = new List<FriendGameData>();

	private FriendGameData m_HighLevelNpc;

	private List<FriendGetBirdBlind> m_VisitingBlinds = new List<FriendGetBirdBlind>();

	private List<FriendGetBirdBlind> m_LowLevelNpcBlinds = new List<FriendGetBirdBlind>();

	private FriendGetBirdBlind m_HighLevelNpcBlind;

	private FriendGameData m_SelectedFriend;

	private bool m_IsRefreshing;

	private int m_CurrentPage;

	private int m_MaxPage;

	private bool m_HideCharacters;

	private void Awake()
	{
		base.transform.position += DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.transform.position;
		base.gameObject.GetComponent<UIPanel>().enabled = false;
	}

	private void Update()
	{
		HideCharacters(DIContainerInfrastructure.GetCoreStateMgr().IsShopLoading() || DIContainerInfrastructure.GetCoreStateMgr().IsShopOpen());
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		Leave();
	}

	public UIGrid getGrid()
	{
		return m_Grid;
	}

	public FriendBirdWindowStateMgr SetType(FriendListType type)
	{
		m_FriendListType = type;
		int mastery = Mathf.Max(1, GetAverageMastery() - 1);
		m_LowLevelNpcs.Clear();
		foreach (FriendData nPCFriend in GetNPCFriends())
		{
			FriendGameData value = null;
			if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.TryGetValue(nPCFriend.Id, out value))
			{
				m_LowLevelNpcs.Add(value);
				value.SetFriendData(nPCFriend);
				if (value.PublicPlayerData != null)
				{
					value.PublicPlayerData.Level = nPCFriend.Level;
				}
				DebugLog.Log("Friend Level: " + value.FriendLevel);
				BirdGameData birdGameData = new BirdGameData(DIContainerLogic.SocialService.GetNPCBird(value.FriendLevel, value.FriendId, mastery));
				birdGameData.IsNPC = true;
				value.SetFriendBird(birdGameData);
			}
		}
		if (type == FriendListType.VisitFriend)
		{
			DIContainerLogic.SocialService.UpdateFreeGachaRolls(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData);
			DIContainerLogic.SocialService.UpdateFreePvpGachaRolls(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData);
		}
		FriendData highNPCFriend = DIContainerLogic.SocialService.GetHighNPCFriend(DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
		if (highNPCFriend != null)
		{
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.TryGetValue(highNPCFriend.Id, out m_HighLevelNpc);
			if (m_HighLevelNpc != null)
			{
				m_HighLevelNpc.SetFriendData(highNPCFriend);
				if (m_HighLevelNpc.PublicPlayerData != null)
				{
					m_HighLevelNpc.PublicPlayerData.Level = highNPCFriend.Level;
				}
				int count = DIContainerBalancing.Service.GetBalancingDataList<ExperienceMasteryBalancingData>().Count;
				int masteryLevel = (int)Mathf.Min((float)GetAverageMastery() * 1.2f, count);
				int enchantmentlevel = 10;
				BirdGameData birdGameData2 = new BirdGameData(DIContainerLogic.SocialService.GetRandomBirdWithLevelHigh(m_HighLevelNpc.FriendLevel, 2, masteryLevel, enchantmentlevel, string.Empty));
				birdGameData2.IsNPC = true;
				m_HighLevelNpc.SetFriendBird(birdGameData2);
			}
		}
		m_LuckyCoinController.SetInventory(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData).SetShopLink(true);
		return this;
	}

	private int GetAverageMastery()
	{
		float num = 0f;
		int num2 = 0;
		foreach (BirdGameData allBird in DIContainerInfrastructure.GetCurrentPlayer().AllBirds)
		{
			num += (float)allBird.ClassItem.Data.Level;
			num2++;
		}
		num /= (float)num2;
		return Convert.ToInt32(Mathf.Round(num));
	}

	private int GetAverageEnchantment()
	{
		float num = 0f;
		int num2 = 0;
		foreach (BirdGameData allBird in DIContainerInfrastructure.GetCurrentPlayer().AllBirds)
		{
			num += (float)allBird.MainHandItem.EnchantementLevel;
			num += (float)allBird.OffHandItem.EnchantementLevel;
			num2 += 2;
		}
		num /= (float)num2;
		return Convert.ToInt32(Mathf.Round(num));
	}

	public FriendBirdWindowStateMgr SetReEnterAction(Action reEnterAction)
	{
		m_ReEnterAction = reEnterAction;
		return this;
	}

	public FriendBirdWindowStateMgr SetSelectFriendAction(Action<FriendGameData> selectFriendAction)
	{
		m_SelectFriendAction = selectFriendAction;
		return this;
	}

	public FriendBirdWindowStateMgr SetSelectedFriend(string friendId)
	{
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.TryGetValue(friendId, out m_SelectedFriend);
		return this;
	}

	public void Enter()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		base.gameObject.SetActive(true);
		m_CurrentPage = 0;
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator InitializeFriends()
	{
		m_Panel.ResetPosition();
		foreach (Transform child in m_Grid.transform)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		m_VisitingBlinds.Clear();
		m_LowLevelNpcBlinds.Clear();
		yield return new WaitForEndOfFrame();
		FriendListWindowStateMgr_FriendsRefreshed(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.Values.ToList());
		if (m_FriendListType == FriendListType.GetBird)
		{
			m_MaxPage = DIContainerLogic.SocialService.GetMaxGetBirdPageCount(DIContainerInfrastructure.GetCurrentPlayer());
		}
		else
		{
			m_MaxPage = DIContainerLogic.SocialService.GetMaxInspectPageCount(DIContainerInfrastructure.GetCurrentPlayer(), false);
		}
		m_LeftButton.gameObject.SetActive(m_MaxPage > 1);
		m_RightButton.gameObject.SetActive(m_MaxPage > 1);
		if (m_MaxPage > 1)
		{
			m_PageLabel.gameObject.SetActive(true);
			m_PageLabel.text = DIContainerInfrastructure.GetLocaService().Tr("friend_header_friends", "Friends: ") + " " + (m_CurrentPage + 1).ToString("0") + "/" + Mathf.Max(1, m_MaxPage).ToString("0");
		}
		else
		{
			m_PageLabel.gameObject.SetActive(false);
		}
		int blindCount = DIContainerLogic.SocialService.GetFriendsPerPage();
		if (m_MaxPage - 1 == m_CurrentPage)
		{
			blindCount = ((m_FriendListType != FriendListType.GetBird) ? Mathf.Min(DIContainerLogic.SocialService.GetFriendCount(DIContainerInfrastructure.GetCurrentPlayer(), true, true, false) - DIContainerLogic.SocialService.GetFriendsPerPage() * m_CurrentPage, DIContainerLogic.SocialService.GetFriendsPerPage()) : Mathf.Min(DIContainerLogic.SocialService.GetFriendCount(DIContainerInfrastructure.GetCurrentPlayer(), true, false, false) - DIContainerLogic.SocialService.GetFriendsPerPage() * m_CurrentPage, DIContainerLogic.SocialService.GetFriendsPerPage()));
		}
		if (m_MaxPage == 0)
		{
			blindCount = 0;
		}
		int index = 0;
		if (m_FriendListType == FriendListType.VisitFriend)
		{
			foreach (FriendGameData npc in m_LowLevelNpcs)
			{
				FriendGetBirdBlind npcBlind2 = UnityEngine.Object.Instantiate(m_FriendVisitingBlind);
				m_LowLevelNpcBlinds.Add(npcBlind2);
				npcBlind2.transform.parent = m_Grid.transform;
				npcBlind2.transform.localPosition = Vector3.zero;
				npcBlind2.Initialize(this, index);
				npcBlind2.transform.parent = m_Grid.transform;
				npcBlind2.transform.localPosition = Vector3.zero;
				npcBlind2.SetModel(npc, m_SelectedFriend);
				index++;
			}
		}
		int j = 0;
		for (j = 0; j < blindCount; j++)
		{
			AddFriendBlind(index);
			if (m_FriendsList.Count - 1 >= j + m_CurrentPage * DIContainerLogic.SocialService.GetFriendsPerPage())
			{
				m_VisitingBlinds[j].SetModel(m_FriendsList[j + m_CurrentPage * DIContainerLogic.SocialService.GetFriendsPerPage()], m_SelectedFriend);
			}
			index++;
		}
		for (int k = j; k < DIContainerLogic.SocialService.GetFriendsPerPage(); k++)
		{
			if (m_FriendListType == FriendListType.GetBird)
			{
				break;
			}
		}
		if (m_FriendListType == FriendListType.GetBird)
		{
			if (m_MaxPage - 1 == m_CurrentPage || m_MaxPage == 0)
			{
				foreach (FriendGameData npc2 in m_LowLevelNpcs)
				{
					FriendGetBirdBlind npcBlind = UnityEngine.Object.Instantiate(m_FriendGetBirdBlind);
					m_LowLevelNpcBlinds.Add(npcBlind);
					npcBlind.transform.parent = m_Grid.transform;
					npcBlind.transform.localPosition = Vector3.zero;
					npcBlind.Initialize(this, m_Grid.transform.childCount);
					npcBlind.transform.parent = m_Grid.transform;
					npcBlind.transform.localPosition = Vector3.zero;
					npcBlind.SetModel(npc2, m_SelectedFriend);
				}
			}
			m_HighLevelNpcBlind = UnityEngine.Object.Instantiate(m_FriendGetBirdBlind);
			m_HighLevelNpcBlind.transform.parent = m_Grid.transform;
			m_HighLevelNpcBlind.transform.localPosition = Vector3.zero;
			m_HighLevelNpcBlind.Initialize(this, m_Grid.transform.childCount);
			m_HighLevelNpcBlind.transform.parent = m_Grid.transform;
			m_HighLevelNpcBlind.transform.localPosition = Vector3.zero;
			if (m_HighLevelNpc != null)
			{
				m_HighLevelNpcBlind.SetModel(m_HighLevelNpc, m_SelectedFriend);
			}
		}
		yield return new WaitForEndOfFrame();
		if ((bool)m_HighLevelNpcBlind)
		{
			m_HighLevelNpcBlind.SetAlternatingOffset(0);
			m_HighLevelNpcBlind.name = "000_Friend_Bird_Blind";
		}
		int m;
		for (m = 0; m < m_VisitingBlinds.Count; m++)
		{
			m_VisitingBlinds[m].SetAlternatingOffset(m + 1);
			m_VisitingBlinds[m].name = (m + 1).ToString("000") + "_Friend_Bird_Blind";
		}
		for (int l = 0; l < m_LowLevelNpcBlinds.Count; l++)
		{
			m_LowLevelNpcBlinds[l].SetAlternatingOffset(l + m + 1);
			m_LowLevelNpcBlinds[l].name = (l + m + 1).ToString("000") + "_Friend_Bird_Blind";
		}
		m_Grid.Reposition();
	}

	private void AddFriendBlind(int i)
	{
		FriendGetBirdBlind friendGetBirdBlind = ((m_FriendListType != 0) ? UnityEngine.Object.Instantiate(m_FriendGetBirdBlind) : UnityEngine.Object.Instantiate(m_FriendVisitingBlind));
		friendGetBirdBlind.transform.parent = m_Grid.transform;
		friendGetBirdBlind.transform.localPosition = Vector3.zero;
		friendGetBirdBlind.Initialize(this, m_Grid.transform.childCount);
		m_VisitingBlinds.Insert(i, friendGetBirdBlind);
	}

	private GameObject InstantiateFriendBonus(int i, bool active)
	{
		FriendCountBonusInfo friendBonus = DIContainerLogic.SocialService.GetFriendBonus(i + 1);
		if (friendBonus.Classes.Count > 0)
		{
			FriendBonusDisplay friendBonusDisplay = UnityEngine.Object.Instantiate(m_ClassFriendBonus);
			friendBonusDisplay.transform.parent = m_Grid.transform;
			friendBonusDisplay.SetModel(friendBonus.Classes.FirstOrDefault(), string.Empty + 1, m_Panel);
			friendBonusDisplay.SetActive(active);
			return friendBonusDisplay.gameObject;
		}
		FriendBonusDisplay friendBonusDisplay2 = null;
		if (friendBonus.AttackBonus > 0f)
		{
			friendBonusDisplay2 = UnityEngine.Object.Instantiate(m_StatFriendBonus);
			friendBonusDisplay2.SetModel("Character_Damage_Large", friendBonus.AttackBonus.ToString("0"));
		}
		else if (friendBonus.HealthBonus > 0f)
		{
			friendBonusDisplay2 = UnityEngine.Object.Instantiate(m_StatFriendBonus);
			friendBonusDisplay2.SetModel("Character_Health_Large", friendBonus.HealthBonus.ToString("0"));
		}
		else
		{
			if (!(friendBonus.XPBonus > 0f))
			{
				return null;
			}
			friendBonusDisplay2 = UnityEngine.Object.Instantiate(m_StatFriendBonus);
			friendBonusDisplay2.SetModel("Resource_XP", friendBonus.XPBonus.ToString("0") + "%");
		}
		friendBonusDisplay2.transform.parent = m_Grid.transform;
		friendBonusDisplay2.SetActive(active);
		return friendBonusDisplay2.gameObject;
	}

	public void ChangePage(int change)
	{
		int num = m_CurrentPage + change;
		if (num < 0)
		{
			num = m_MaxPage + num;
		}
		if (num >= m_MaxPage)
		{
			num = -1 + change;
		}
		m_CurrentPage = num;
		RefreshWindow();
	}

	public void ResetWindow()
	{
		RegisterEventHandler();
		m_FriendsList = new List<FriendGameData>();
		Invoke("GetFriends", 0f);
		StartCoroutine(InitializeFriends());
	}

	public void RefreshWindow()
	{
		StartCoroutine(InitializeFriends());
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("friendbirds_enter");
		yield return StartCoroutine(InitializeFriends());
		UIPanel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPanel>();
		foreach (UIPanel panel in componentsInChildren)
		{
			panel.enabled = true;
		}
		if (DIContainerInfrastructure.GetCoreStateMgr().IsShopLoading() || DIContainerInfrastructure.GetCoreStateMgr().IsShopOpen())
		{
			HideCharacters(true);
		}
		if (!DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			m_FacebookLoginRoot.SetActive(true);
			m_UseFriendBirdRoot.SetActive(false);
		}
		else
		{
			m_FacebookLoginRoot.SetActive(false);
			m_UseFriendBirdRoot.SetActive(true);
		}
		m_ListAnimation.Play("List_Enter");
		m_HeaderAnimation.Play("Header_Enter");
		m_FooterAnimation.Play("Footer_Enter");
		yield return new WaitForSeconds(m_ListAnimation["List_Enter"].length);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("friendbirds_enter");
		RegisterEventHandler();
	}

	private List<FriendData> GetNPCFriends()
	{
		int level = DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
		List<FriendData> list = new List<FriendData>();
		if (DIContainerInfrastructure.GetCoreStateMgr().m_ChronicleCave && m_FriendListType == FriendListType.GetBird)
		{
			list.Add(DIContainerLogic.SocialService.GetPorkyFriend(level));
			list.Add(DIContainerLogic.SocialService.GetAdventurerFriend(level));
		}
		list.Add(DIContainerLogic.SocialService.GetLowNPCFriend(level));
		return list;
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("friendbirds_leave");
		m_ListAnimation.Play("List_Leave");
		m_HeaderAnimation.Play("Header_Leave");
		m_FooterAnimation.Play("Footer_Leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		yield return new WaitForSeconds(Mathf.Max(m_ListAnimation["List_Leave"].length, DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.GetLeaveLength()));
		if (m_ReEnterAction != null)
		{
			m_ReEnterAction();
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("friendbirds_leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(3u);
		UnityEngine.Object.Destroy(base.gameObject);
		base.gameObject.SetActive(false);
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		m_CloseButton.Clicked += m_ButtonClose_Clicked;
		m_LuckyCoinController.RegisterEventHandlers();
		m_FacebookLoginButton.Clicked += m_FacebookLoginButton_Clicked;
		m_LeftButton.Clicked += m_LeftButton_Clicked;
		m_RightButton.Clicked += m_RightButton_Clicked;
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, HandleBackButton);
	}

	private void m_FacebookLoginButton_Clicked()
	{
		DebugLog.Log("FacebookSignInButtonClicked clicked!");
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("LinkClicked", new Dictionary<string, string>
		{
			{ "Button", "FacebookSignIn" },
			{ "Destination", "friendBirds" }
		});
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= FacebookLoginSucdeeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= FacebookLoginFailed;
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent += FacebookLoginSucdeeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent += FacebookLoginFailed;
		if (!DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			DIContainerInfrastructure.GetCoreStateMgr().TryLoginOnFacebook();
		}
	}

	private void FacebookLoginFailed(string error)
	{
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= FacebookLoginSucdeeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= FacebookLoginFailed;
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_failed", "Facebook login failed!"), "facebook", DispatchMessage.Status.Error);
	}

	private void FacebookLoginSucdeeded()
	{
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= FacebookLoginSucdeeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= FacebookLoginFailed;
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_succes", "Facebook login succesfully!"), "facebook", DispatchMessage.Status.Info);
		DIContainerInfrastructure.GetCoreStateMgr().StartRefreshFriends();
		DIContainerInfrastructure.GetAttributionService().TrackEvent(AdjustTrackingEvent.Signup);
		StartCoroutine(ReEnterFooter());
	}

	private IEnumerator ReEnterFooter()
	{
		m_FooterAnimation.Play("Footer_Leave");
		yield return new WaitForSeconds(m_FooterAnimation["Footer_Leave"].length);
		if (!DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			m_FacebookLoginRoot.SetActive(true);
			m_UseFriendBirdRoot.SetActive(false);
		}
		else
		{
			m_FacebookLoginRoot.SetActive(false);
			m_UseFriendBirdRoot.SetActive(true);
		}
		m_FooterAnimation.Play("Footer_Enter");
	}

	private void FriendListWindowStateMgr_FriendsRefreshed(List<FriendGameData> friends)
	{
		int num = 0;
		List<FriendGameData> source = friends.OrderByDescending((FriendGameData f) => f.FriendLevel).ToList();
		switch (m_FriendListType)
		{
		case FriendListType.VisitFriend:
			m_FriendsList = source.Where((FriendGameData f) => !f.isNpcFriend).ToList();
			break;
		case FriendListType.GetBird:
			m_FriendsList = source.Where((FriendGameData f) => !f.isNpcFriend).ToList();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		for (num = 0; num < m_VisitingBlinds.Count; num++)
		{
			if (num + m_CurrentPage * DIContainerLogic.SocialService.GetFriendsPerPage() <= DIContainerLogic.SocialService.GetFriendCount(DIContainerInfrastructure.GetCurrentPlayer(), m_FriendListType == FriendListType.GetBird, m_FriendListType == FriendListType.VisitFriend, false))
			{
				m_VisitingBlinds[num].SetModel(m_FriendsList[num + m_CurrentPage * DIContainerLogic.SocialService.GetFriendsPerPage()], m_SelectedFriend);
			}
		}
		if (m_FriendListType == FriendListType.GetBird)
		{
			Invoke("GetFriendDatas", 0f);
		}
	}

	private void GetFriendDatas()
	{
		List<FriendGameData> list = new List<FriendGameData>();
		list.AddRange(m_FriendsList);
		list.Add(m_HighLevelNpc);
		list.AddRange(m_LowLevelNpcs);
		List<string> list2 = new List<string>();
		foreach (FriendGameData friends in m_FriendsList)
		{
			list2.Add(friends.FriendId);
		}
		Dictionary<string, BirdData> dictionary = new Dictionary<string, BirdData>();
		foreach (FriendGameData friends2 in m_FriendsList)
		{
			if (friends2.PublicPlayerData != null)
			{
				List<BirdData> birds = friends2.PublicPlayerData.Birds;
				if (birds == null || birds.Count <= 0)
				{
					DebugLog.Error("Invalid Birds value in Friend Profile: " + friends2.PublicPlayerData.SocialId);
				}
				else
				{
					dictionary.Add(friends2.PublicPlayerData.SocialId, birds[UnityEngine.Random.Range(0, birds.Count)]);
				}
			}
		}
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.RefreshFriendsBirds(dictionary);
	}

	private void m_RightButton_Clicked()
	{
		ChangePage(1);
	}

	private void m_LeftButton_Clicked()
	{
		ChangePage(-1);
	}

	private void DeRegisterEventHandler()
	{
		m_CloseButton.Clicked -= m_ButtonClose_Clicked;
		m_LuckyCoinController.DeRegisterEventHandlers();
		m_FacebookLoginButton.Clicked -= m_FacebookLoginButton_Clicked;
		m_LeftButton.Clicked -= m_LeftButton_Clicked;
		m_RightButton.Clicked -= m_RightButton_Clicked;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
	}

	private void m_ButtonClose_Clicked()
	{
		Leave();
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine());
	}

	public void SelectedFriend(FriendGameData friend)
	{
		m_SelectedFriend = friend;
		if (m_SelectFriendAction != null)
		{
			m_SelectFriendAction(friend);
		}
	}

	public void HideCharacters(bool hide)
	{
		if (m_HideCharacters != hide)
		{
			m_HideCharacters = hide;
			m_characterCamera.enabled = !hide;
		}
	}
}
