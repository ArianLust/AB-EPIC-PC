using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.GameDatas.MailboxMessages;
using ABH.Services.Logic;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using UnityEngine;

public class SocialWindowUI : MonoBehaviour
{
	[SerializeField]
	private FriendVisitingBlind m_FriendVisitingBlind;

	[SerializeField]
	private FriendVisitingBlind m_FriendVisitingBlindArena;

	[SerializeField]
	private FriendAddBlind m_AddFriendBlind;

	[SerializeField]
	private FriendBonusDisplay m_ClassFriendBonus;

	[SerializeField]
	private FriendBonusDisplay m_StatFriendBonus;

	[SerializeField]
	private UILabel m_FriendBadgeCount;

	[SerializeField]
	private GameObject m_FriendBadgeRoot;

	private List<FriendGameData> m_FriendsList = new List<FriendGameData>();

	private List<FriendGameData> m_LowLevelNpcs = new List<FriendGameData>();

	private List<FriendVisitingBlind> m_VisitingBlinds = new List<FriendVisitingBlind>();

	private List<FriendVisitingBlind> m_LowLevelNpcBlinds = new List<FriendVisitingBlind>();

	private int m_MaxPage;

	[SerializeField]
	public bool m_IsPvp;

	private bool m_IsRefreshing;

	private bool m_finishedSpring;

	private int m_PageCount;

	[SerializeField]
	private int m_MessagesPerPage = 20;

	[SerializeField]
	private LootDisplayContoller m_LootForExplosionPrefab;

	[SerializeField]
	private MailboxMessageBlind m_MailboxMessagePrefab;

	[SerializeField]
	private UILabel m_MailboxBadgeCount;

	[SerializeField]
	private GameObject m_MailboxBadgeRoot;

	private List<IMailboxMessageGameData> m_ShowAbleMessages = new List<IMailboxMessageGameData>();

	private List<MailboxMessageBlind> m_MailboxMessages = new List<MailboxMessageBlind>();

	private SocialWindowCategory m_CurrentCategory = SocialWindowCategory.Mailbox;

	private GameObject m_TempArrowTransform;

	[SerializeField]
	private List<Transform> m_LeagueRoots;

	[SerializeField]
	private Transform m_Highlight;

	private bool IsAvatarLoaded;

	[SerializeField]
	private UILabel m_AchievementsButtonText;

	[SerializeField]
	private UISprite m_AchievementsIcon;

	[SerializeField]
	private GameObject m_RovioSocialRoot;

	[SerializeField]
	private UIInputTrigger m_RovioIdSignInButton;

	[SerializeField]
	private GameObject m_RovioIdRegisterText;

	[SerializeField]
	private GameObject m_RovioIdRegisterButtonObject;

	[SerializeField]
	private UIInputTrigger m_RovioIdRegisterButton;

	[SerializeField]
	private UILabel m_RovioIdAccountName;

	[SerializeField]
	private UITexture m_RovioIdAccountPicture;

	[SerializeField]
	private UILabel m_RovioIdSignInButtonText;

	[SerializeField]
	private UILabel m_RovioIdSignInAdditionalText;

	[SerializeField]
	private UIInputTrigger m_FacebookSignInButton;

	[SerializeField]
	private UILabel m_FacebookSignInButtonText;

	[SerializeField]
	private UILabel m_FacebookSignInAdditionalText;

	[SerializeField]
	private GameObject m_AchievementsRoot;

	[SerializeField]
	private UIInputTrigger m_AchievementsButton;

	[SerializeField]
	private UILabel m_AchievementsCompletionPercent;

	[SerializeField]
	private UILabel m_RovioIdBadgeCount;

	[SerializeField]
	private GameObject m_RovioIdBadgeRoot;

	private bool achievmentLoginTrigger;

	[SerializeField]
	private bool useRovioId = true;

	public UIInputTrigger m_BackButton;

	public UIInputTrigger m_RovioIdButton;

	public UIInputTrigger m_MailboxButton;

	public UIInputTrigger m_FriendsButton;

	[SerializeField]
	public UIInputTrigger m_LeftButton;

	[SerializeField]
	private UIInputTrigger m_FriendshipEssenceButton;

	[SerializeField]
	private Animation m_TimerAnimation;

	[SerializeField]
	private UILabel m_TimerLabel;

	[SerializeField]
	public UIInputTrigger m_FacebookShortcut;

	[SerializeField]
	public UIInputTrigger m_RightButton;

	[SerializeField]
	private GameObject m_EmptyMailboxRoot;

	[SerializeField]
	private Transform m_ArrowTransform;

	[SerializeField]
	private float m_arrowStartPositionCompensation;

	[SerializeField]
	private GameObject m_RovioIdRoot;

	[SerializeField]
	private GameObject m_MailboxRoot;

	[SerializeField]
	private GameObject m_FriendsRoot;

	[SerializeField]
	private GameObject m_RovioIdFooterRoot;

	[SerializeField]
	private GameObject m_MailboxFooterRoot;

	[SerializeField]
	private GameObject m_FriendsFooterRoot;

	[SerializeField]
	private GameObject m_ChallengeFooterRoot;

	[SerializeField]
	private GameObject m_ArenaSocialFooterRoot;

	[SerializeField]
	private GameObject m_FacebookFooterRoot;

	[SerializeField]
	private GameObject m_FriendshipEssenceFooterRoot;

	[SerializeField]
	private GameObject m_RovioLoginFooterRoot;

	private Dictionary<SocialWindowCategory, GameObject> m_RootDictionary = new Dictionary<SocialWindowCategory, GameObject>(3);

	private Dictionary<SocialWindowCategory, GameObject> m_FooterDictionary = new Dictionary<SocialWindowCategory, GameObject>(3);

	private Dictionary<SocialWindowCategory, Vector3> m_ArrowPostions = new Dictionary<SocialWindowCategory, Vector3>(3);

	private Dictionary<SocialWindowCategory, int> m_CurrentPage = new Dictionary<SocialWindowCategory, int>(2);

	[SerializeField]
	public UILabel m_HeaderText;

	[SerializeField]
	private Animation m_HeaderAnimation;

	[SerializeField]
	private Animation m_AreaSelectionAnimation;

	[SerializeField]
	private Animation m_FooterAnimation;

	[SerializeField]
	private Animation m_ContentAnimation;

	[SerializeField]
	private UIGrid m_MailboxGrid;

	[SerializeField]
	private UIGrid m_FriendListGrid;

	[SerializeField]
	private UIScrollView m_MailboxPanel;

	[SerializeField]
	private UIScrollView m_FriendListPanel;

	private bool m_sendFriendshipMessagesInProgress;

	public BaseCampStateMgr m_StateMgr;

	[SerializeField]
	private GameObject m_PvPInfoRoot;

	[SerializeField]
	private GameObject m_PvPInfoFooterRoot;

	[SerializeField]
	private UIInputTrigger m_PvPInfoButton;

	private List<FriendData> GetNPCFriends()
	{
		int level = DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
		List<FriendData> list = new List<FriendData>();
		list.Add(DIContainerLogic.SocialService.GetLowNPCFriend(level));
		return list;
	}

	public void FriendListAwake()
	{
		m_LowLevelNpcs.Clear();
		foreach (FriendData nPCFriend in GetNPCFriends())
		{
			FriendGameData value = null;
			if (DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.TryGetValue(nPCFriend.Id, out value))
			{
				m_LowLevelNpcs.Add(value);
				value.SetFriendData(nPCFriend);
			}
		}
		m_FriendBadgeRoot.SetActive(false);
		if (m_IsPvp)
		{
			DIContainerLogic.SocialService.UpdateFreePvpGachaRolls(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData);
		}
		else
		{
			DIContainerLogic.SocialService.UpdateFreeGachaRolls(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData);
		}
	}

	private IEnumerator InitializeFriends()
	{
		m_FriendListPanel.ResetPosition();
		m_MaxPage = DIContainerLogic.SocialService.GetMaxInspectPageCount(DIContainerInfrastructure.GetCurrentPlayer(), m_IsPvp);
		m_LeftButton.gameObject.SetActive(m_MaxPage > 1);
		m_RightButton.gameObject.SetActive(m_MaxPage > 1);
		DebugLog.Log("Friend List max page Count: " + m_MaxPage);
		m_HeaderText.text = DIContainerInfrastructure.GetLocaService().Tr("friend_header_friends", "Friends: ") + " " + (m_CurrentPage[SocialWindowCategory.Friends] + 1).ToString("0") + "/" + Mathf.Max(1, m_MaxPage).ToString("0");
		foreach (Transform child in getGrid().transform)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		m_VisitingBlinds.Clear();
		yield return new WaitForEndOfFrame();
		int blindCount = DIContainerLogic.SocialService.GetFriendsPerPage();
		DebugLog.Log("Friend per page: " + blindCount);
		DebugLog.Log("Current Page: " + m_CurrentPage[SocialWindowCategory.Friends]);
		if (m_MaxPage - 1 == m_CurrentPage[SocialWindowCategory.Friends])
		{
			DebugLog.Log("Is on last page! FriendCount: " + DIContainerLogic.SocialService.GetFriendCount(DIContainerInfrastructure.GetCurrentPlayer(), true, true, m_IsPvp));
			blindCount = Mathf.Min(DIContainerLogic.SocialService.GetFriendCount(DIContainerInfrastructure.GetCurrentPlayer(), true, true, m_IsPvp) - DIContainerLogic.SocialService.GetFriendsPerPage() * m_CurrentPage[SocialWindowCategory.Friends], DIContainerLogic.SocialService.GetFriendsPerPage());
			DebugLog.Log("Final Blind Count: " + blindCount);
		}
		if (m_MaxPage == 0)
		{
			blindCount = 0;
		}
		DebugLog.Log("Final Blind Count: " + blindCount);
		int negativeIndex = -2;
		foreach (FriendGameData npc in m_LowLevelNpcs)
		{
			FriendVisitingBlind npcBlind2 = null;
			npcBlind2 = ((!m_IsPvp) ? UnityEngine.Object.Instantiate(m_FriendVisitingBlind) : UnityEngine.Object.Instantiate(m_FriendVisitingBlindArena));
			m_LowLevelNpcBlinds.Add(npcBlind2);
			npcBlind2.transform.parent = getGrid().transform;
			npcBlind2.transform.localPosition = Vector3.zero;
			npcBlind2.Initialize(this, negativeIndex, m_IsPvp);
			npcBlind2.transform.parent = getGrid().transform;
			npcBlind2.transform.localPosition = Vector3.zero;
			npcBlind2.SetModel(npc);
			DebugLog.Log("Added NPC: " + npc.FriendName);
			negativeIndex--;
		}
		int index = 0;
		int j = 0;
		for (j = 0; j < blindCount; j++)
		{
			DebugLog.Log("Adding blind, index = " + index + ", i = " + j);
			AddFriendBlind(index);
			if (m_FriendsList.Count - 1 >= j + m_CurrentPage[SocialWindowCategory.Friends] * DIContainerLogic.SocialService.GetFriendsPerPage())
			{
				m_VisitingBlinds[j].SetModel(m_FriendsList[j + m_CurrentPage[SocialWindowCategory.Friends] * DIContainerLogic.SocialService.GetFriendsPerPage()]);
			}
			index++;
		}
		if (DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			FriendAddBlind addFriendBlind = UnityEngine.Object.Instantiate(m_AddFriendBlind);
			addFriendBlind.Initialize(this, index + m_CurrentPage[SocialWindowCategory.Friends] * DIContainerLogic.SocialService.GetFriendsPerPage());
			addFriendBlind.transform.parent = getGrid().transform;
			addFriendBlind.transform.localPosition = Vector3.zero;
		}
		GetFriends();
		yield return new WaitForEndOfFrame();
		getGrid().Reposition();
	}

	private void AddFriendBlind(int i)
	{
		FriendVisitingBlind friendVisitingBlind = ((!m_IsPvp) ? UnityEngine.Object.Instantiate(m_FriendVisitingBlind) : UnityEngine.Object.Instantiate(m_FriendVisitingBlindArena));
		friendVisitingBlind.transform.parent = getGrid().transform;
		friendVisitingBlind.transform.localPosition = Vector3.zero;
		friendVisitingBlind.Initialize(this, i + m_CurrentPage[SocialWindowCategory.Friends] * DIContainerLogic.SocialService.GetFriendsPerPage(), m_IsPvp);
		m_VisitingBlinds.Insert(i, friendVisitingBlind);
	}

	private GameObject InstantiateFriendBonus(int i, bool active)
	{
		FriendCountBonusInfo friendBonus = DIContainerLogic.SocialService.GetFriendBonus(i + 1);
		if (friendBonus.Classes.Count > 0)
		{
			FriendBonusDisplay friendBonusDisplay = UnityEngine.Object.Instantiate(m_ClassFriendBonus);
			friendBonusDisplay.transform.parent = getGrid().transform;
			friendBonusDisplay.SetModel(friendBonus.Classes.FirstOrDefault(), string.Empty + 1, m_FriendListPanel);
			friendBonusDisplay.SetActive(active);
			return friendBonusDisplay.gameObject;
		}
		FriendBonusDisplay friendBonusDisplay2 = null;
		if (friendBonus.AttackBonus > 0f)
		{
			friendBonusDisplay2 = UnityEngine.Object.Instantiate(m_StatFriendBonus);
			friendBonusDisplay2.transform.parent = getGrid().transform;
			friendBonusDisplay2.SetModel("Character_Damage_Large", friendBonus.AttackBonus.ToString("0"));
		}
		else if (friendBonus.HealthBonus > 0f)
		{
			friendBonusDisplay2 = UnityEngine.Object.Instantiate(m_StatFriendBonus);
			friendBonusDisplay2.transform.parent = getGrid().transform;
			friendBonusDisplay2.SetModel("Character_Health_Large", friendBonus.HealthBonus.ToString("0"));
		}
		else
		{
			if (!(friendBonus.XPBonus > 0f))
			{
				return null;
			}
			friendBonusDisplay2 = UnityEngine.Object.Instantiate(m_StatFriendBonus);
			friendBonusDisplay2.transform.parent = getGrid().transform;
			friendBonusDisplay2.SetModel("Resource_XP", friendBonus.XPBonus.ToString("0") + "%");
		}
		friendBonusDisplay2.SetActive(active);
		return friendBonusDisplay2.gameObject;
	}

	public void FriendList_ChangePage(int change)
	{
		int num = m_CurrentPage[SocialWindowCategory.Friends] + change;
		if (num < 0)
		{
			num = m_MaxPage + num;
		}
		if (num >= m_MaxPage)
		{
			num = -1 + change;
		}
		m_CurrentPage[SocialWindowCategory.Friends] = num;
		RefreshWindow();
	}

	public void ResetWindow()
	{
		RegisterFriendListEventHandlers();
		m_FriendsList = new List<FriendGameData>();
		GetFriends();
		StartCoroutine(InitializeFriends());
	}

	public void RefreshWindow()
	{
		StartCoroutine(InitializeFriends());
	}

	public UIGrid getGrid()
	{
		return m_FriendListGrid;
	}

	private void DeRegisterFriendListEventHandlers()
	{
		if ((bool)m_LeftButton)
		{
			m_LeftButton.Clicked -= FriendList_m_LeftButton_Clicked;
		}
		if ((bool)m_RightButton)
		{
			m_RightButton.Clicked -= FriendList_m_RightButton_Clicked;
		}
		foreach (FriendVisitingBlind lowLevelNpcBlind in m_LowLevelNpcBlinds)
		{
			lowLevelNpcBlind.DeRegisterEventHandler();
		}
		foreach (FriendVisitingBlind visitingBlind in m_VisitingBlinds)
		{
			visitingBlind.DeRegisterEventHandler();
		}
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.AcceptedFriendsReceived -= FriendList_FriendsRefreshed;
	}

	private void RegisterFriendListEventHandlers()
	{
		DeRegisterFriendListEventHandlers();
		m_LeftButton.Clicked += FriendList_m_LeftButton_Clicked;
		m_RightButton.Clicked += FriendList_m_RightButton_Clicked;
		foreach (FriendVisitingBlind lowLevelNpcBlind in m_LowLevelNpcBlinds)
		{
			lowLevelNpcBlind.RegisterEventHandler();
		}
		foreach (FriendVisitingBlind visitingBlind in m_VisitingBlinds)
		{
			visitingBlind.RegisterEventHandler();
		}
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.AcceptedFriendsReceived += FriendList_FriendsRefreshed;
	}

	private void FriendList_m_RightButton_Clicked()
	{
		FriendList_ChangePage(1);
	}

	private void FriendList_m_LeftButton_Clicked()
	{
		FriendList_ChangePage(-1);
	}

	private void GetFriends()
	{
		FriendList_FriendsRefreshed(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.Values.ToList());
	}

	private void FriendList_FriendsRefreshed(List<FriendGameData> friends)
	{
		DebugLog.Log("Refreshing Friends!");
		List<FriendGameData> list = new List<FriendGameData>();
		foreach (FriendGameData friend in friends)
		{
			if ((m_IsPvp && DIContainerLogic.SocialService.HasFreePvpGachaRoll(friend, DIContainerInfrastructure.GetCurrentPlayer())) || (!m_IsPvp && DIContainerLogic.SocialService.HasFreeGachaRoll(friend, DIContainerInfrastructure.GetCurrentPlayer())))
			{
				list.Add(friend);
			}
		}
		list = list.OrderBy((FriendGameData f) => f.FriendLevel).ToList();
		int num = 0;
		if (m_IsPvp)
		{
			m_FriendsList = (from f in friends
				where !f.isNpcFriend && f.PublicPlayerData != null && f.PublicPlayerData.Banner != null
				orderby f.FriendLevel descending
				select f).ToList();
		}
		else
		{
			m_FriendsList = (from f in friends
				where !f.isNpcFriend
				orderby f.FriendLevel descending
				select f).ToList();
		}
		foreach (FriendGameData item in list)
		{
			m_FriendsList.Remove(item);
			m_FriendsList.Insert(0, item);
		}
		for (num = 0; num < m_VisitingBlinds.Count; num++)
		{
			if (num + m_CurrentPage[SocialWindowCategory.Friends] * DIContainerLogic.SocialService.GetFriendsPerPage() <= DIContainerLogic.SocialService.GetFriendCount(DIContainerInfrastructure.GetCurrentPlayer(), false, true, m_IsPvp))
			{
				m_VisitingBlinds[num].SetModel(m_FriendsList[num + m_CurrentPage[SocialWindowCategory.Friends] * DIContainerLogic.SocialService.GetFriendsPerPage()]);
			}
		}
	}

	private void MailboxAwake()
	{
		int viewableMessagesCount = GetViewableMessagesCount();
		m_MailboxBadgeRoot.gameObject.SetActive(viewableMessagesCount > 0);
		m_MailboxBadgeCount.text = viewableMessagesCount.ToString("0");
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MessageAdded -= MessagesChanged;
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MessageRemoved -= MessagesChanged;
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MessageAdded += MessagesChanged;
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MessageRemoved += MessagesChanged;
	}

	private void MessagesChanged(IMailboxMessageGameData obj)
	{
		int viewableMessagesCount = GetViewableMessagesCount();
		m_MailboxBadgeRoot.gameObject.SetActive(viewableMessagesCount > 0);
		m_MailboxBadgeCount.text = viewableMessagesCount.ToString("0");
	}

	private int GetViewableMessagesCount()
	{
		return DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MailboxMessages.Values.Count((IMailboxMessageGameData m) => !m.IsViewed);
	}

	private void MailboxDestroy()
	{
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MessageAdded -= MessagesChanged;
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MessageRemoved -= MessagesChanged;
	}

	private IEnumerator InitializeMailboxMessages()
	{
		m_MailboxPanel.ResetPosition();
		foreach (Transform child in m_MailboxGrid.transform)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		m_ShowAbleMessages = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MailboxMessages.Values.Where((IMailboxMessageGameData m) => !m.IsViewed).ToList();
		CombinePvpChallengeMessages(m_ShowAbleMessages);
		m_ShowAbleMessages = m_ShowAbleMessages.OrderByDescending((IMailboxMessageGameData m) => m.SendTime).ToList();
		m_MailboxMessages.Clear();
		SetMailboxHeader();
		yield return new WaitForEndOfFrame();
		for (int i = m_MessagesPerPage * m_CurrentPage[SocialWindowCategory.Mailbox]; i < Mathf.Min(m_ShowAbleMessages.Count, m_MessagesPerPage * (m_CurrentPage[SocialWindowCategory.Mailbox] + 1)); i++)
		{
			AddMessageBlind(message: m_ShowAbleMessages[i], i: i, offset: m_MessagesPerPage * m_CurrentPage[SocialWindowCategory.Mailbox]);
		}
		m_MailboxGrid.Reposition();
		yield return new WaitForEndOfFrame();
	}

	private void CombinePvpChallengeMessages(List<IMailboxMessageGameData> Messages)
	{
		WonInPvpChallengeMessage wonInPvpChallengeMessage = null;
		List<IMailboxMessageGameData> list = new List<IMailboxMessageGameData>();
		foreach (IMailboxMessageGameData Message in Messages)
		{
			if (Message is WonInPvpChallengeMessage)
			{
				if (wonInPvpChallengeMessage == null)
				{
					wonInPvpChallengeMessage = Message as WonInPvpChallengeMessage;
					continue;
				}
				wonInPvpChallengeMessage.CombineMessage(Message as WonInPvpChallengeMessage);
				list.Add(Message);
			}
		}
		foreach (IMailboxMessageGameData item in list)
		{
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.RemoveMessage(item);
			Messages.Remove(item);
		}
	}

	private void SetMailboxHeader()
	{
		if (m_ShowAbleMessages.Count <= m_MessagesPerPage)
		{
			m_LeftButton.gameObject.SetActive(false);
			m_RightButton.gameObject.SetActive(false);
		}
		else
		{
			m_LeftButton.gameObject.SetActive(true);
			m_RightButton.gameObject.SetActive(true);
		}
		m_EmptyMailboxRoot.gameObject.SetActive(m_ShowAbleMessages.Count == 0);
		m_PageCount = Mathf.Max(m_ShowAbleMessages.Count - 1, 0) / m_MessagesPerPage + 1;
		m_HeaderText.text = ((m_PageCount <= 1) ? DIContainerInfrastructure.GetLocaService().Tr("mail_header_mailbox") : (DIContainerInfrastructure.GetLocaService().Tr("mail_header_mailbox") + " " + (m_CurrentPage[SocialWindowCategory.Mailbox] + 1) + "/" + m_PageCount));
	}

	private void AddMessageBlind(int i, int offset, IMailboxMessageGameData message)
	{
		MailboxMessageBlind mailboxMessageBlind = UnityEngine.Object.Instantiate(m_MailboxMessagePrefab);
		mailboxMessageBlind.transform.parent = m_MailboxGrid.transform;
		mailboxMessageBlind.transform.localPosition = Vector3.zero;
		bool flag = message is WonInPvpChallengeMessage;
		mailboxMessageBlind.SetModel(message, m_StateMgr, i, flag);
		if (flag)
		{
			mailboxMessageBlind.GetComponentInChildren<FriendInfoElement>().m_NPCAvatar.spriteName = "Avatar_Terence";
		}
		m_MailboxMessages.Insert(i - offset, mailboxMessageBlind);
	}

	public void SoftMailboxRefresh(MailboxMessageBlind mailToRemove)
	{
		if (!mailToRemove)
		{
			for (int i = 0; i < m_ShowAbleMessages.Count; i++)
			{
				IMailboxMessageGameData model = m_ShowAbleMessages[i];
				Transform child = m_MailboxGrid.transform.GetChild(i);
				if ((bool)child)
				{
					MailboxMessageBlind component = child.GetComponent<MailboxMessageBlind>();
					component.SetModel(model, m_StateMgr, i);
				}
			}
		}
		else
		{
			StartCoroutine(RemoveMailAndRefresh(mailToRemove));
		}
	}

	public void SoftMailboxRefreshAdd(IMailboxMessageGameData mailToAdd)
	{
		if (mailToAdd == null)
		{
			for (int i = 0; i < m_ShowAbleMessages.Count; i++)
			{
				IMailboxMessageGameData model = m_ShowAbleMessages[i];
				Transform child = m_MailboxGrid.transform.GetChild(i);
				if ((bool)child)
				{
					MailboxMessageBlind component = child.GetComponent<MailboxMessageBlind>();
					component.SetModel(model, m_StateMgr, i);
				}
			}
		}
		else
		{
			StartCoroutine(AddMailAndRefresh(mailToAdd));
		}
	}

	private IEnumerator AddMailAndRefresh(IMailboxMessageGameData mailToAdd)
	{
		m_IsRefreshing = true;
		List<MailboxMessageBlind> nextMessages = m_MailboxMessages;
		float slideDuration = DIContainerLogic.GetPacingBalancing().EquipmentRepositionDuration;
		if (m_ShowAbleMessages.Count > 0)
		{
			foreach (MailboxMessageBlind nextMessage in nextMessages)
			{
				StartCoroutine(nextMessage.MoveOffset(new Vector2(0f, 0f - m_MailboxGrid.cellHeight), slideDuration));
				yield return new WaitForSeconds(slideDuration);
			}
		}
		yield return new WaitForEndOfFrame();
		m_ShowAbleMessages.Insert(0, mailToAdd);
		AddMessageBlind(0, 0, mailToAdd);
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < m_MailboxMessages.Count; i++)
		{
			m_MailboxMessages[i].SetModel(m_MailboxMessages[i].GetModel(), m_StateMgr, i);
		}
		m_MailboxGrid.Reposition();
		m_EmptyMailboxRoot.gameObject.SetActive(m_ShowAbleMessages.Count == 0);
		m_IsRefreshing = false;
	}

	private IEnumerator RemoveMailAndRefresh(MailboxMessageBlind mailToRemove)
	{
		while (m_IsRefreshing)
		{
			yield return new WaitForEndOfFrame();
		}
		m_IsRefreshing = true;
		MailboxMessageBlind destroyed = null;
		List<MailboxMessageBlind> nextMessages = new List<MailboxMessageBlind>();
		bool takeNext = false;
		bool selectedNext = false;
		int index = 0;
		float slideDuration = DIContainerLogic.GetPacingBalancing().EquipmentRepositionDuration;
		foreach (MailboxMessageBlind message in m_MailboxMessages)
		{
			if (takeNext && !selectedNext)
			{
				selectedNext = true;
			}
			if (message == mailToRemove && !takeNext)
			{
				takeNext = true;
				destroyed = message;
			}
			if (selectedNext)
			{
				nextMessages.Add(message);
			}
			index++;
		}
		if ((bool)destroyed)
		{
			m_MailboxMessages.Remove(destroyed);
			m_ShowAbleMessages.Remove(destroyed.GetModel());
			if (destroyed.GetModel().HasReward)
			{
				LootDisplayContoller item = UnityEngine.Object.Instantiate(m_LootForExplosionPrefab, destroyed.m_ConfirmButtonTrigger.transform.position + new Vector3(0f, 0f, -3f), Quaternion.identity) as LootDisplayContoller;
				UnityHelper.SetLayerRecusively(item.gameObject, LayerMask.NameToLayer("Interface"));
				item.SetModel(null, DIContainerLogic.GetLootOperationService().GetItemsFromLoot(destroyed.GetModel().Loot), LootDisplayType.None);
				List<LootDisplayContoller> explodedItems = item.Explode(true, false, 0f, true, 0f, 0f);
				foreach (LootDisplayContoller explodedLoot in explodedItems)
				{
					UnityEngine.Object.Destroy(explodedLoot.gameObject, explodedLoot.gameObject.GetComponent<Animation>().clip.length);
				}
			}
			UnityEngine.Object.Destroy(destroyed.gameObject);
		}
		foreach (MailboxMessageBlind nextMessage in nextMessages)
		{
			StartCoroutine(nextMessage.MoveOffset(new Vector2(0f, m_MailboxGrid.cellHeight), slideDuration));
		}
		yield return new WaitForSeconds(slideDuration);
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < m_MailboxMessages.Count; i++)
		{
			m_MailboxMessages[i].SetModel(m_MailboxMessages[i].GetModel(), m_StateMgr, m_MessagesPerPage * m_CurrentPage[SocialWindowCategory.Mailbox] + i);
		}
		if (m_ShowAbleMessages.Count >= m_MessagesPerPage * (m_CurrentPage[SocialWindowCategory.Mailbox] + 1))
		{
			AddMessageBlind(message: m_ShowAbleMessages[m_MessagesPerPage * (m_CurrentPage[SocialWindowCategory.Mailbox] + 1) - 1], i: m_MessagesPerPage * (m_CurrentPage[SocialWindowCategory.Mailbox] + 1) - 1, offset: m_MessagesPerPage * m_CurrentPage[SocialWindowCategory.Mailbox]);
		}
		if (m_ShowAbleMessages.Count <= m_MessagesPerPage)
		{
			m_LeftButton.gameObject.SetActive(false);
			m_RightButton.gameObject.SetActive(false);
		}
		else
		{
			m_LeftButton.gameObject.SetActive(true);
			m_RightButton.gameObject.SetActive(true);
		}
		if (m_PageCount > Mathf.Max(m_ShowAbleMessages.Count - 1, 0) / m_MessagesPerPage + 1 && m_CurrentPage[SocialWindowCategory.Mailbox] == m_PageCount - 1)
		{
			m_CurrentPage[SocialWindowCategory.Mailbox] = Mathf.Max(m_ShowAbleMessages.Count - 1, 0) / m_MessagesPerPage;
			yield return StartCoroutine(InitializeMailboxMessages());
			m_IsRefreshing = false;
			m_EmptyMailboxRoot.gameObject.SetActive(m_ShowAbleMessages.Count == 0);
		}
		else
		{
			SetMailboxHeader();
			m_MailboxGrid.repositionNow = true;
			m_IsRefreshing = false;
		}
	}

	private void OnFinishedPanelSpring()
	{
		m_finishedSpring = true;
	}

	private void Mailbox_m_RightButton_Clicked()
	{
		DebugLog.Log("Next Page Selected");
		Mailbox_ChangePage(1);
	}

	private void Mailbox_m_LeftButton_Clicked()
	{
		DebugLog.Log("Previous Page Selected");
		Mailbox_ChangePage(-1);
	}

	public void Mailbox_ChangePage(int change)
	{
		int num = m_CurrentPage[SocialWindowCategory.Mailbox] + change;
		if (num < 0)
		{
			num = m_PageCount + num;
		}
		if (num >= m_PageCount)
		{
			num = -1 + change;
		}
		m_CurrentPage[SocialWindowCategory.Mailbox] = num;
		StartCoroutine(InitializeMailboxMessages());
	}

	private void DeRegisterMailboxEventHandlers()
	{
		if ((bool)m_LeftButton)
		{
			m_LeftButton.Clicked -= Mailbox_m_LeftButton_Clicked;
		}
		if ((bool)m_RightButton)
		{
			m_RightButton.Clicked -= Mailbox_m_RightButton_Clicked;
		}
	}

	private void RegisterMailboxEventHandlers()
	{
		DeRegisterMailboxEventHandlers();
		m_LeftButton.Clicked += Mailbox_m_LeftButton_Clicked;
		m_RightButton.Clicked += Mailbox_m_RightButton_Clicked;
	}

	private void PvpInfoAwake()
	{
	}

	private IEnumerator InitializePvPInfo()
	{
		PvPSeasonManagerGameData seasondata = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		string seasonName = DIContainerInfrastructure.GetLocaService().Tr(seasondata.Balancing.LocaBaseId + "_name");
		Dictionary<string, string> ReplacementDic = new Dictionary<string, string>
		{
			{ "{value_1}", seasonName },
			{
				"{value_2}",
				seasondata.Data.CurrentSeason.ToString()
			},
			{
				"{value_3}",
				seasondata.Balancing.SeasonTurnAmount.ToString()
			}
		};
		m_HeaderText.text = DIContainerInfrastructure.GetLocaService().Tr("header_pvpinfo", ReplacementDic);
		m_LeftButton.gameObject.SetActive(false);
		m_RightButton.gameObject.SetActive(false);
		IInventoryItemGameData currentLeagueItem = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "pvp_league_crown", out currentLeagueItem))
		{
			int currentLeague = currentLeagueItem.ItemData.Level;
			if (currentLeague <= m_LeagueRoots.Count)
			{
				m_Highlight.position = m_LeagueRoots[currentLeague - 1].position;
			}
		}
		yield break;
	}

	private void DeRegisterPvpInfoEventHandlers()
	{
		DebugLog.Log("[SocialWindowUI] DeRegisterRovioIdEventHandlers");
	}

	private void RegisterPvpInfoEventHandlers()
	{
		DeRegisterPvpInfoEventHandlers();
		DebugLog.Log("[SocialWindowUI] RegisterRovioIdEventHandlers");
	}

	private void ExitPvpInfoScreen()
	{
	}

	private void RovioIdAwake()
	{
		m_RovioIdBadgeRoot.SetActive(false);
		DIContainerInfrastructure.GetFacebookWrapper().CheckConnectionAsynch(OnConnectionChecked);
		DIContainerInfrastructure.GetAchievementService().GetGlobalAchievementProgress(updateAchievementProgress);
		InvokeRepeating("CheckAndRewardRovioIdLogin", 2f, 2f);
	}

	private void CheckAndRewardRovioIdLogin()
	{
		if (!DIContainerInfrastructure.IdentityService.IsGuest() && DIContainerInfrastructure.GetCurrentPlayer() != null && !DIContainerInfrastructure.GetCurrentPlayer().Data.RovioIdRegisterOnce && DIContainerInfrastructure.GetRovioAccSyncState().IsAccountActionPossible())
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.RovioIdRegisterOnce = true;
			List<IInventoryItemGameData> items = DIContainerLogic.GetLootOperationService().RewardLootGetInputCopy(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, DIContainerLogic.GetLootOperationService().GenerateLoot(new Dictionary<string, int> { { "loot_rovioaccount_01", 1 } }, 1), "registered_for_rovio_id");
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfoAndLootItems(DIContainerInfrastructure.GetLocaService().Tr("toast_signin_reward", "Sign in Reward!"), items, "signinreward");
			DIContainerInfrastructure.GetAttributionService().TrackEvent(AdjustTrackingEvent.Signup);
		}
	}

	private void updateAchievementProgress(float progress)
	{
		m_AchievementsCompletionPercent.text = Mathf.RoundToInt(progress * 100f) + "%";
	}

	private void OnConnectionChecked(bool connected)
	{
		DebugLog.Log("[SocialWindowUI] User is Connected to Facebook: " + connected);
	}

	private void DeRegisterRovioIdEventHandlers()
	{
		DebugLog.Log("[SocialWindowUI] DeRegisterRovioIdEventHandlers");
		if ((bool)m_FacebookSignInButton)
		{
			m_FacebookSignInButton.Clicked -= FacebookSignInButtonClicked;
		}
		if ((bool)m_RovioIdSignInButton)
		{
			m_RovioIdSignInButton.Clicked -= RovioIdSignInButtonClicked;
		}
		if ((bool)m_AchievementsButton)
		{
			m_AchievementsButton.Clicked -= AchievementsButtonClicked;
		}
		if ((bool)m_RovioIdRegisterButton)
		{
			m_RovioIdRegisterButton.Clicked -= RovioIdRegisterButtonClicked;
		}
		if ((bool)m_BackButton)
		{
			m_BackButton.Clicked -= ExitRovioIdScreen;
		}
		DIContainerInfrastructure.IdentityService.OnLoggedIn -= SkynestIdentityLoggedIn;
		DIContainerInfrastructure.IdentityService.OnLoginError -= SkynestIdentityLoginError;
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= SocialWindowUIFacebookLoginSucceededEvent;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= SocialWindowUIFacebookLoginFailedEvent;
		DIContainerInfrastructure.GetFacebookWrapper().logoutFailedEvent -= SocialWindowUIFacebookLogoutFailedEvent;
		DIContainerInfrastructure.GetFacebookWrapper().logoutSucceededEvent -= SocialWindowUIFacebookLogoutSucceededEvent;
	}

	private void LeaveRovioIdUI()
	{
		if (m_CurrentCategory == SocialWindowCategory.RovioId)
		{
			ExitRovioIdScreen();
		}
	}

	private void RegisterRovioIdEventHandlers()
	{
		DeRegisterRovioIdEventHandlers();
		DebugLog.Log("[SocialWindowUI] RegisterRovioIdEventHandlers");
		m_FacebookSignInButton.Clicked += FacebookSignInButtonClicked;
		m_RovioIdSignInButton.Clicked += RovioIdSignInButtonClicked;
		m_AchievementsButton.Clicked += AchievementsButtonClicked;
		m_RovioIdRegisterButton.Clicked += RovioIdRegisterButtonClicked;
		m_BackButton.Clicked += ExitRovioIdScreen;
		DIContainerInfrastructure.IdentityService.OnLoggedIn += SkynestIdentityLoggedIn;
		DIContainerInfrastructure.IdentityService.OnLoginError += SkynestIdentityLoginError;
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent += SocialWindowUIFacebookLoginSucceededEvent;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent += SocialWindowUIFacebookLoginFailedEvent;
		DIContainerInfrastructure.GetFacebookWrapper().logoutFailedEvent += SocialWindowUIFacebookLogoutFailedEvent;
		DIContainerInfrastructure.GetFacebookWrapper().logoutSucceededEvent += SocialWindowUIFacebookLogoutSucceededEvent;
	}

	private void SocialWindowUIFacebookLoginFailedEvent(string obj)
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_failed", "Facebook login failed!"), "facebook", DispatchMessage.Status.Error);
		DIContainerInfrastructure.GetAsynchStatusService().ClearAfterSomeTime();
	}

	private void SocialWindowUIFacebookLogoutFailedEvent()
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_logout_failed", "Facebook logout failed!"), "facebook", DispatchMessage.Status.Info);
		StartCoroutine(SetLoginStates());
	}

	private void SocialWindowUIFacebookLogoutSucceededEvent()
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_logout_success", "Facebook logout success!"), "facebook", DispatchMessage.Status.Info);
		StartCoroutine(SetLoginStates());
	}

	private void SocialWindowUIFacebookLoginSucceededEvent()
	{
		DIContainerInfrastructure.GetCoreStateMgr().StartRefreshFriends();
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_succes", "Facebook login succesfully!"), "facebook", DispatchMessage.Status.Info);
		StartCoroutine(SetLoginStates());
	}

	private void SkynestIdentityRegistered()
	{
		AfterSkynestResponseCallbackWhenSoftKeyboardOrNavigationBarHasBeenShown();
		StartCoroutine(SetLoginStates());
	}

	private void SkynestIdentityLoginError(int errorCode, string errorMessage)
	{
		if (errorCode == 3 && ContentLoader.Instance != null && !string.IsNullOrEmpty(ContentLoader.Instance.m_BeaconConnectionMgr.Identiy.GetRefreshToken()))
		{
			StartCoroutine(ContentLoader.Instance.SubmitErrorReport("This account have not been confirmed.\r\nRefresh token: " + ContentLoader.Instance.m_BeaconConnectionMgr.Identiy.GetRefreshToken()));
		}
		AfterSkynestResponseCallbackWhenSoftKeyboardOrNavigationBarHasBeenShown();
		DebugLog.Log("[SocialWindowUI.RovioId] SkynestIdentityLoginError errorCode = " + errorCode + ", errorMessage = " + errorMessage);
	}

	private void AfterSkynestResponseCallbackWhenSoftKeyboardOrNavigationBarHasBeenShown()
	{
		// AndroidTools.EnableImmersiveMode();
	}

	private void SkynestIdentityLoggedIn()
	{
		AfterSkynestResponseCallbackWhenSoftKeyboardOrNavigationBarHasBeenShown();
		StartCoroutine(SetLoginStates());
		m_StateMgr.UpdateLoggedInIndicator();
	}

	private void CheckAchievmentSignIn()
	{
		if (achievmentLoginTrigger != DIContainerInfrastructure.GetAchievementService().IsSignedIn)
		{
			SetLoginStatesDelayed();
			CancelInvoke("CheckAchievmentSignIn");
		}
	}

	private void AchievementsButtonClicked()
	{
		DebugLog.Log("AchievementsButtonClicked clicked!");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Button", "Achievements");
		dictionary.Add("Destination", "Achievements");
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("LinkClicked", dictionary);
		if (DIContainerInfrastructure.GetAchievementService().IsSignedIn == false || !DIContainerInfrastructure.GetAchievementService().IsSignedIn.HasValue)
		{
			DIContainerInfrastructure.GetAchievementService().Init(DIContainerInfrastructure.GetCoreStateMgr(), true);
			InvokeRepeating("CheckAchievmentSignIn", 1f, 1f);
		}
		else
		{
			DIContainerInfrastructure.GetAchievementService().ShowAchievementUI();
		}
	}

	private void SetLoginStatesDelayed()
	{
		StartCoroutine(SetLoginStates());
	}

	private void RovioIdRegisterButtonClicked()
	{
		DebugLog.Log("Register Skynest Id clicked!");
		if (ContentLoader.Instance != null && ContentLoader.Instance.m_BeaconConnectionMgr != null)
		{
			ContentLoader.Instance.m_BeaconConnectionMgr.RegistrationStarted = true;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Button", "RovioIDRegister");
		dictionary.Add("Destination", "Rovio");
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("LinkClicked", dictionary);
		DIContainerInfrastructure.GetRovioAccSyncState().RegisterButtonPressed();
	}

	private void RovioIdSignInButtonClicked()
	{
		ContentLoader.Instance.LastAccountWasNew = DIContainerInfrastructure.GetCurrentPlayer().Data.IsNewCreatedAccount;
		DebugLog.Log("RovioIdSignInButtonClicked clicked!");
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("LinkClicked", new Dictionary<string, string>
		{
			{ "Button", "RovioIDSignIn" },
			{ "Destination", "RovioID" }
		});
		DIContainerInfrastructure.GetRovioAccSyncState().SignInSignOutButtonPressed();
	}

	private void FacebookSignInButtonClicked()
	{
		DebugLog.Log("FacebookSignInButtonClicked clicked!");
		DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("LinkClicked", new Dictionary<string, string>
		{
			{ "Button", "FacebookSignIn" },
			{ "Destination", "facebook" }
		});
		if (!DIContainerInfrastructure.GetCoreStateMgr().TryLoginOnFacebook())
		{
			if (!DIContainerInfrastructure.GetCoreStateMgr().TryLogoutOfFacebook())
			{
				DebugLog.Error("FacebookSignInButtonClicked - no action taken");
			}
			else if (m_StateMgr != null)
			{
				m_StateMgr.UpdateFreeGachaSign();
			}
		}
	}

	public IEnumerator SetLoginStates()
	{
		m_LeftButton.gameObject.SetActive(false);
		m_RightButton.gameObject.SetActive(false);
		if ((bool)m_AchievementsIcon)
		{
			if (DIContainerInfrastructure.GetAchievementService().IsSignedIn == false || !DIContainerInfrastructure.GetAchievementService().IsSignedIn.HasValue)
			{
				m_AchievementsIcon.spriteName = "GooglePlus";
				m_AchievementsButtonText.text = DIContainerInfrastructure.GetLocaService().Tr("social_rovid_btn_signin");
			}
			else
			{
				m_AchievementsIcon.spriteName = "GooglePlay";
				m_AchievementsButtonText.text = DIContainerInfrastructure.GetLocaService().Tr("social_googleplay_button");
			}
		}
		m_HeaderText.text = DIContainerInfrastructure.GetLocaService().Tr("header_rovioid", "Account Settings");
		if (ContentLoader.Instance.m_BeaconConnectionMgr.IsGuestAccount())
		{
			HandleGuestRovioIdLoggedInState();
		}
		else
		{
			HandleRovioIdLoggedInState();
		}
		DebugLog.Log(GetType(), "SetLoginStates: IsUserAuthenticated = " + DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated());
		if (DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
		{
			HandleFacebookLoggedInState();
		}
		else
		{
			HandleFacebookLoggedOutState();
		}
		if (DIContainerInfrastructure.GetAchievementService().IsSignedIn == true)
		{
			HandleGooglePlaySignedInState();
		}
		else
		{
			HandleGooglePlaySignedOutState();
		}
		yield break;
	}

	private void HandleGooglePlaySignedInState()
	{
	}

	private void HandleGooglePlaySignedOutState()
	{
	}

	private void HandleFacebookLoggedOutState()
	{
		m_FacebookSignInButtonText.text = DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login", "Connect");
	}

	private void HandleFacebookLoggedInState()
	{
		m_FacebookSignInButtonText.text = DIContainerInfrastructure.GetLocaService().Tr("social_facebook_logout", "Logout");
	}

	private void HandleRovioIdNotLoggedInState()
	{
		m_RovioIdSignInButton.gameObject.SetActive(true);
		m_RovioIdRegisterText.gameObject.SetActive(true);
		m_RovioIdRegisterButtonObject.gameObject.SetActive(true);
		m_RovioIdAccountName.text = DIContainerInfrastructure.GetLocaService().Tr("social_rovid_notloggedin", "Not Logged In!");
		m_RovioIdSignInButtonText.text = DIContainerInfrastructure.GetLocaService().Tr("social_rovid_btn_signin", "Sign In");
	}

	private void HandleRovioIdLoggedInState()
	{
		DebugLog.Log("[SocialWindowUI] HandleRovioIdLoggedInState()");
		m_RovioIdSignInButton.gameObject.SetActive(true);
		m_RovioIdRegisterText.gameObject.SetActive(false);
		m_RovioIdRegisterButtonObject.gameObject.SetActive(false);
		string email = DIContainerInfrastructure.IdentityService.UserCredentials.Email;
		m_RovioIdAccountName.text = email;
		m_RovioIdSignInButtonText.text = DIContainerInfrastructure.GetLocaService().Tr("social_rovid_btn_signout", "Sign Out");
		StartCoroutine(LoadAvatarTexture(DIContainerInfrastructure.IdentityService.UserCredentials.AvatarAsset));
	}

	private void ExitRovioIdScreen()
	{
		DIContainerInfrastructure.GetCoreStateMgr().CloseConfirmationPopup();
		DIContainerInfrastructure.GetRovioAccSyncState().ExitRovioIdScreen();
	}

	private IEnumerator LoadAvatarTexture(string avatarAsset)
	{
		if (!IsAvatarLoaded && !string.IsNullOrEmpty(avatarAsset))
		{
			WWW pictureDownload = new WWW(avatarAsset);
			yield return pictureDownload;
			if (m_RovioIdAccountPicture != null && pictureDownload != null)
			{
				m_RovioIdAccountPicture.mainTexture = pictureDownload.texture;
			}
			IsAvatarLoaded = true;
		}
	}

	private void HandleGuestRovioIdLoggedInState()
	{
		m_RovioIdSignInButton.gameObject.SetActive(true);
		m_RovioIdRegisterText.gameObject.SetActive(true);
		m_RovioIdRegisterButtonObject.gameObject.SetActive(true);
		m_RovioIdSignInButtonText.text = DIContainerInfrastructure.GetLocaService().Tr("social_rovid_btn_signin", "Sign In");
		m_RovioIdAccountName.text = DIContainerInfrastructure.GetLocaService().Tr("social_rovid_guestname", "Guest");
	}

	private void Awake()
	{
		base.transform.position += DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.transform.position;
		m_RootDictionary.Add(SocialWindowCategory.Friends, m_FriendsRoot);
		m_RootDictionary.Add(SocialWindowCategory.Mailbox, m_MailboxRoot);
		if (useRovioId)
		{
			m_RootDictionary.Add(SocialWindowCategory.RovioId, m_RovioIdRoot);
		}
		else
		{
			m_RootDictionary.Add(SocialWindowCategory.PvPInfo, m_PvPInfoRoot);
		}
		m_CurrentPage.Add(SocialWindowCategory.Friends, 0);
		m_CurrentPage.Add(SocialWindowCategory.Mailbox, 0);
		m_FooterDictionary.Add(SocialWindowCategory.Friends, m_FriendsFooterRoot);
		m_FooterDictionary.Add(SocialWindowCategory.Mailbox, m_MailboxFooterRoot);
		if (useRovioId)
		{
			m_FooterDictionary.Add(SocialWindowCategory.RovioId, m_RovioIdFooterRoot);
		}
		else
		{
			m_FooterDictionary.Add(SocialWindowCategory.PvPInfo, m_PvPInfoFooterRoot);
		}
		m_ArrowTransform.position = new Vector3(m_ArrowTransform.position.x, m_FriendsButton.transform.position.y + m_arrowStartPositionCompensation, m_ArrowTransform.position.z);
		m_ArrowPostions.Add(SocialWindowCategory.Friends, m_ArrowTransform.localPosition);
		m_ArrowTransform.position = new Vector3(m_ArrowTransform.position.x, m_MailboxButton.transform.position.y + m_arrowStartPositionCompensation, m_ArrowTransform.position.z);
		m_ArrowPostions.Add(SocialWindowCategory.Mailbox, m_ArrowTransform.localPosition);
		if (useRovioId)
		{
			m_ArrowTransform.position = new Vector3(m_ArrowTransform.position.x, m_RovioIdButton.transform.position.y + m_arrowStartPositionCompensation, m_ArrowTransform.position.z);
			m_ArrowPostions.Add(SocialWindowCategory.RovioId, m_ArrowTransform.localPosition);
		}
		else
		{
			m_ArrowTransform.position = new Vector3(m_ArrowTransform.position.x, m_PvPInfoButton.transform.position.y + m_arrowStartPositionCompensation, m_ArrowTransform.position.z);
			m_ArrowPostions.Add(SocialWindowCategory.PvPInfo, m_ArrowTransform.localPosition);
		}
		m_TempArrowTransform = new GameObject("Temp_Arrow_Transform");
		m_TempArrowTransform.transform.parent = m_ArrowTransform.parent;
		m_TempArrowTransform.transform.position = m_ArrowTransform.position;
		FriendListAwake();
		MailboxAwake();
		if (useRovioId)
		{
			RovioIdAwake();
		}
		else
		{
			PvpInfoAwake();
		}
	}

	private void Start()
	{
		GetComponent<UIPanel>().enabled = true;
	}

	private void OnDestroy()
	{
		DeRegisterEventHandlers();
		MailboxDestroy();
	}

	public SocialWindowUI SetStateMgr(BaseCampStateMgr mgr)
	{
		m_StateMgr = mgr;
		return this;
	}

	private bool UseAlternativeFooter(SocialWindowCategory category)
	{
		if (category == SocialWindowCategory.RovioId)
		{
			return false;
		}
		return !DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated();
	}

	public void SetCategory(SocialWindowCategory category)
	{
		m_CurrentCategory = category;
	}

	public void Enter()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	public void Leave(bool withRoot = true)
	{
		StartCoroutine(LeaveCoroutine(withRoot));
	}

	private IEnumerator EnterCoroutine()
	{
		float enterTime = 0f;
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("social_enter");
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter();
		yield return StartCoroutine(SwitchCategory(m_CurrentCategory, false));
		if ((bool)m_AreaSelectionAnimation && m_AreaSelectionAnimation.Play("AreaSelection_Enter"))
		{
			enterTime = Mathf.Max(enterTime, m_AreaSelectionAnimation["AreaSelection_Enter"].length);
		}
		if ((bool)m_HeaderAnimation && m_HeaderAnimation.Play("Header_Enter"))
		{
			enterTime = Mathf.Max(enterTime, m_HeaderAnimation["Header_Enter"].length);
		}
		if ((bool)m_FooterAnimation && m_FooterAnimation.Play("Footer_Enter"))
		{
			enterTime = Mathf.Max(enterTime, m_FooterAnimation["Footer_Enter"].length);
		}
		if ((bool)m_TimerAnimation)
		{
			if (!DIContainerLogic.SocialService.IsGetFriendshipEssencePossible(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData))
			{
				DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
				{
					m_TimerAnimation.Play("Button_Timer_SetShown");
					StartCoroutine(CountDownTimer(trustedTime + DIContainerLogic.SocialService.GetFriendshipEssenceTimeLeft(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData)));
				});
			}
			else
			{
				m_TimerAnimation.Play("Button_Timer_SetHidden");
			}
		}
		if ((bool)m_ContentAnimation && m_ContentAnimation.Play("List_Enter"))
		{
			enterTime = Mathf.Max(enterTime, m_ContentAnimation["List_Enter"].length);
		}
		yield return new WaitForSeconds(enterTime);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("social_enter");
		RegisterEventHandlers();
	}

	private IEnumerator CountDownTimer(DateTime targetTime)
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				m_TimerLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft);
			}
			yield return new WaitForSeconds(1f);
		}
		m_TimerAnimation.Play("Button_Timer_Hide");
	}

	private IEnumerator LeaveCoroutine(bool withRoot = true)
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("social_leave");
		float leaveTime = 0f;
		if ((bool)m_AreaSelectionAnimation && m_AreaSelectionAnimation.Play("AreaSelection_Leave"))
		{
			leaveTime = Mathf.Max(leaveTime, m_AreaSelectionAnimation["AreaSelection_Leave"].length);
		}
		if ((bool)m_HeaderAnimation && m_HeaderAnimation.Play("Header_Leave"))
		{
			leaveTime = Mathf.Max(leaveTime, m_HeaderAnimation["Header_Leave"].length);
		}
		if ((bool)m_FooterAnimation && m_FooterAnimation.Play("Footer_Leave"))
		{
			leaveTime = Mathf.Max(leaveTime, m_FooterAnimation["Footer_Leave"].length);
		}
		if ((bool)m_ContentAnimation && m_ContentAnimation.Play("List_Leave"))
		{
			leaveTime = Mathf.Max(leaveTime, m_ContentAnimation["List_Leave"].length);
		}
		yield return new WaitForSeconds(leaveTime);
		if (withRoot)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("social_leave");
		base.gameObject.SetActive(false);
	}

	private IEnumerator SwitchCategory(SocialWindowCategory category, bool animated)
	{
		DeRegisterFriendListEventHandlers();
		DeRegisterMailboxEventHandlers();
		if (useRovioId)
		{
			DeRegisterRovioIdEventHandlers();
		}
		else
		{
			DeRegisterPvpInfoEventHandlers();
		}
		DeRegisterCategoryButtons();
		if (m_CurrentCategory != category)
		{
			RegisterCategoryClosed(m_CurrentCategory);
		}
		if (animated)
		{
			float changeOutTime = 0f;
			if ((bool)m_ContentAnimation && m_ContentAnimation.Play("List_Change_Out"))
			{
				changeOutTime = Mathf.Max(changeOutTime, m_ContentAnimation["List_Change_Out"].length);
			}
			if ((bool)m_FooterAnimation && m_FooterAnimation.Play("Footer_Leave"))
			{
				changeOutTime = Mathf.Max(changeOutTime, m_FooterAnimation["Footer_Leave"].length);
			}
			yield return new WaitForSeconds(changeOutTime);
		}
		foreach (SocialWindowCategory key in m_RootDictionary.Keys)
		{
			GameObject value = m_RootDictionary[key];
			value.SetActive(key == category);
		}
		switch (category)
		{
		case SocialWindowCategory.RovioId:
			if (m_ArenaSocialFooterRoot != null)
			{
				m_ArenaSocialFooterRoot.SetActive(false);
			}
			m_FacebookFooterRoot.SetActive(false);
			m_FriendshipEssenceFooterRoot.SetActive(false);
			m_ChallengeFooterRoot.SetActive(false);
			m_RovioLoginFooterRoot.SetActive(true);
			break;
		case SocialWindowCategory.Friends:
			if (m_ArenaSocialFooterRoot != null)
			{
				m_ArenaSocialFooterRoot.SetActive(false);
			}
			if (!DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
			{
				m_FacebookFooterRoot.SetActive(true);
				m_FriendshipEssenceFooterRoot.SetActive(false);
				m_ChallengeFooterRoot.SetActive(false);
				m_RovioLoginFooterRoot.SetActive(false);
				break;
			}
			m_FacebookFooterRoot.SetActive(false);
			if (m_IsPvp)
			{
				m_ChallengeFooterRoot.SetActive(true);
				m_FriendshipEssenceFooterRoot.SetActive(false);
			}
			else
			{
				m_ChallengeFooterRoot.SetActive(false);
				m_FriendshipEssenceFooterRoot.SetActive(true);
			}
			m_RovioLoginFooterRoot.SetActive(false);
			break;
		case SocialWindowCategory.Mailbox:
			if (m_ArenaSocialFooterRoot != null)
			{
				m_ArenaSocialFooterRoot.SetActive(false);
			}
			if (!DIContainerInfrastructure.GetFacebookWrapper().IsUserAuthenticated())
			{
				m_FacebookFooterRoot.SetActive(true);
				m_FriendshipEssenceFooterRoot.SetActive(false);
				m_ChallengeFooterRoot.SetActive(false);
				m_RovioLoginFooterRoot.SetActive(false);
			}
			else
			{
				m_FacebookFooterRoot.SetActive(false);
				m_FriendshipEssenceFooterRoot.SetActive(true);
				m_ChallengeFooterRoot.SetActive(false);
				m_RovioLoginFooterRoot.SetActive(false);
			}
			break;
		case SocialWindowCategory.PvPInfo:
			m_ArenaSocialFooterRoot.SetActive(true);
			m_FriendshipEssenceFooterRoot.SetActive(false);
			m_ChallengeFooterRoot.SetActive(false);
			m_RovioLoginFooterRoot.SetActive(false);
			m_FacebookFooterRoot.SetActive(false);
			break;
		}
		m_CurrentCategory = category;
		DebugLog.Log("Switched to Category: " + m_CurrentCategory);
		switch (m_CurrentCategory)
		{
		case SocialWindowCategory.RovioId:
			yield return StartCoroutine(SetLoginStates());
			break;
		case SocialWindowCategory.Mailbox:
			yield return StartCoroutine(InitializeMailboxMessages());
			break;
		case SocialWindowCategory.Friends:
			yield return StartCoroutine(InitializeFriends());
			break;
		case SocialWindowCategory.PvPInfo:
			yield return StartCoroutine(InitializePvPInfo());
			break;
		}
		m_ArrowTransform.localPosition = m_ArrowPostions[m_CurrentCategory];
		if (animated)
		{
			float changeInTime = 0f;
			if ((bool)m_ContentAnimation && m_ContentAnimation.Play("List_Change_In"))
			{
				changeInTime = Mathf.Max(changeInTime, m_ContentAnimation["List_Change_In"].length);
			}
			if ((bool)m_FooterAnimation && m_FooterAnimation.Play("Footer_Enter"))
			{
				changeInTime = Mathf.Max(changeInTime, m_FooterAnimation["Footer_Enter"].length);
			}
			yield return new WaitForSeconds(changeInTime);
		}
		RegisterCategoryButtons();
		switch (m_CurrentCategory)
		{
		case SocialWindowCategory.RovioId:
			RegisterRovioIdEventHandlers();
			break;
		case SocialWindowCategory.Mailbox:
			RegisterMailboxEventHandlers();
			break;
		case SocialWindowCategory.Friends:
			RegisterFriendListEventHandlers();
			break;
		case SocialWindowCategory.PvPInfo:
			DeRegisterPvpInfoEventHandlers();
			break;
		}
	}

	private void RegisterCategoryClosed(SocialWindowCategory currentCategory)
	{
		if (currentCategory == SocialWindowCategory.RovioId)
		{
			ExitRovioIdScreen();
		}
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		LeaveRovioIdUI();
		Leave();
	}

	private void RegisterEventHandlers()
	{
		DebugLog.Log("[SocialWindowUI] RegisterEventHandlers");
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		if ((bool)m_BackButton)
		{
			m_BackButton.Clicked += BackButton_Clicked;
		}
		if ((bool)m_FacebookShortcut)
		{
			m_FacebookShortcut.Clicked -= FacebookShortcutClicked;
			m_FacebookShortcut.Clicked += FacebookShortcutClicked;
		}
		if ((bool)m_FriendshipEssenceButton)
		{
			m_FriendshipEssenceButton.Clicked += m_FriendshipEssenceButton_Clicked;
		}
		RegisterCategoryButtons();
		switch (m_CurrentCategory)
		{
		case SocialWindowCategory.RovioId:
			RegisterRovioIdEventHandlers();
			break;
		case SocialWindowCategory.Mailbox:
			RegisterMailboxEventHandlers();
			break;
		case SocialWindowCategory.Friends:
			RegisterFriendListEventHandlers();
			break;
		case SocialWindowCategory.PvPInfo:
			RegisterPvpInfoEventHandlers();
			break;
		}
	}

	private void DeRegisterEventHandlers()
	{
		if ((bool)DIContainerInfrastructure.BackButtonMgr)
		{
			DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		}
		if ((bool)m_BackButton)
		{
			m_BackButton.Clicked -= BackButton_Clicked;
		}
		if ((bool)m_FacebookShortcut)
		{
			m_FacebookShortcut.Clicked -= FacebookShortcutClicked;
		}
		if ((bool)m_FriendshipEssenceButton)
		{
			m_FriendshipEssenceButton.Clicked -= m_FriendshipEssenceButton_Clicked;
		}
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= FacebookLoginSucdeeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= FacebookLoginFailed;
		DeRegisterFriendListEventHandlers();
		DeRegisterMailboxEventHandlers();
		if (useRovioId)
		{
			DeRegisterRovioIdEventHandlers();
		}
		else
		{
			DeRegisterPvpInfoEventHandlers();
		}
		DeRegisterCategoryButtons();
	}

	private void RegisterCategoryButtons()
	{
		DeRegisterCategoryButtons();
		if ((bool)m_RovioIdButton)
		{
			m_RovioIdButton.Clicked += m_RovioIdButton_Clicked;
		}
		if ((bool)m_PvPInfoButton)
		{
			m_PvPInfoButton.Clicked += m_PvPButton_Clicked;
		}
		if ((bool)m_MailboxButton)
		{
			m_MailboxButton.Clicked += m_MailboxButton_Clicked;
		}
		if ((bool)m_FriendsButton)
		{
			m_FriendsButton.Clicked += m_FriendsButton_Clicked;
		}
	}

	private void DeRegisterCategoryButtons()
	{
		if ((bool)m_RovioIdButton)
		{
			m_RovioIdButton.Clicked -= m_RovioIdButton_Clicked;
		}
		if ((bool)m_PvPInfoButton)
		{
			m_PvPInfoButton.Clicked -= m_PvPButton_Clicked;
		}
		if ((bool)m_MailboxButton)
		{
			m_MailboxButton.Clicked -= m_MailboxButton_Clicked;
		}
		if ((bool)m_FriendsButton)
		{
			m_FriendsButton.Clicked -= m_FriendsButton_Clicked;
		}
	}

	private void FacebookShortcutClicked()
	{
		DebugLog.Log("Facebook Button Clicked");
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= FacebookLoginSucdeeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= FacebookLoginFailed;
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent += FacebookLoginSucdeeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent += FacebookLoginFailed;
		DIContainerInfrastructure.GetCoreStateMgr().TryLoginOnFacebook();
	}

	private void FacebookLoginFailed(string error)
	{
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= FacebookLoginSucdeeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= FacebookLoginFailed;
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_failed", "Facebook login failed!"), "facebook", DispatchMessage.Status.Error);
		DIContainerInfrastructure.GetAsynchStatusService().ClearAfterSomeTime();
	}

	private void FacebookLoginSucdeeded()
	{
		DIContainerInfrastructure.GetFacebookWrapper().loginSucceededEvent -= FacebookLoginSucdeeded;
		DIContainerInfrastructure.GetFacebookWrapper().loginFailedEvent -= FacebookLoginFailed;
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("social_facebook_login_succes", "Facebook login succesfully!"), "facebook", DispatchMessage.Status.Info);
		DIContainerInfrastructure.GetCoreStateMgr().StartRefreshFriends();
		StartCoroutine(SwitchCategory(m_CurrentCategory, true));
		DIContainerInfrastructure.GetAttributionService().TrackEvent(AdjustTrackingEvent.Signup);
	}

	private void m_FriendsButton_Clicked()
	{
		if (m_CurrentCategory != SocialWindowCategory.Friends)
		{
			DebugLog.Log("Friends Button Clicked");
			StartCoroutine(SwitchCategory(SocialWindowCategory.Friends, true));
		}
	}

	private void m_MailboxButton_Clicked()
	{
		if (m_CurrentCategory != SocialWindowCategory.Mailbox)
		{
			DebugLog.Log("Mailbox Button Clicked");
			StartCoroutine(SwitchCategory(SocialWindowCategory.Mailbox, true));
		}
	}

	private void m_PvPButton_Clicked()
	{
		if (m_CurrentCategory != SocialWindowCategory.PvPInfo)
		{
			DebugLog.Log("Mailbox Button Clicked");
			StartCoroutine(SwitchCategory(SocialWindowCategory.PvPInfo, true));
		}
	}

	private void m_RovioIdButton_Clicked()
	{
		if (m_CurrentCategory != 0)
		{
			DebugLog.Log("RovioId Button Clicked");
			StartCoroutine(SwitchCategory(SocialWindowCategory.RovioId, true));
		}
	}

	private void m_FriendshipEssenceButton_Clicked()
	{
		if (m_sendFriendshipMessagesInProgress)
		{
			return;
		}
		m_sendFriendshipMessagesInProgress = true;
		if (!DIContainerLogic.SocialService.IsGetFriendshipEssencePossible(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData))
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (FriendGameData value in DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Friends.Values)
		{
			if (!value.isNpcFriend)
			{
				list.Add(value.FriendId);
			}
		}
		MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
		messageDataIncoming.MessageType = MessageType.RequestFriendshipEssenceMessage;
		messageDataIncoming.Sender = DIContainerInfrastructure.GetCurrentPlayer().GetFriendData();
		messageDataIncoming.SentAt = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
		MessageDataIncoming message = messageDataIncoming;
		ABHAnalyticsHelper.SendSocialEvent(message, null);
		DIContainerInfrastructure.MessagingService.SendMessages(message, list);
		DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
		{
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipEssenceCooldown = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
			m_TimerAnimation.Play("Button_Timer_Show");
			StartCoroutine(CountDownTimer(trustedTime + DIContainerLogic.SocialService.GetFriendshipEssenceTimeLeft(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData)));
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("gen_toast_successendfessencemessage", "Messages sent!"), "essencesuccess", DispatchMessage.Status.Info);
			m_sendFriendshipMessagesInProgress = false;
		});
	}

	private void BackButton_Clicked()
	{
		Leave();
	}
}
