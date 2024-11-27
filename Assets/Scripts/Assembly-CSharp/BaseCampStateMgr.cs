using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using UnityEngine;

public class BaseCampStateMgr : MonoBehaviour
{
	[HideInInspector]
	public Dictionary<string, bool> m_LoadedLevels = new Dictionary<string, bool>();

	[SerializeField]
	public Transform m_CharacterRoot;

	[HideInInspector]
	public GachaPopupUI m_GachaPopup;

	[HideInInspector]
	public EnchantmentUI m_EnchantmentUi;

	[HideInInspector]
	public EnchantingResultPopup m_EnchantmentPopup;

	[HideInInspector]
	public GameProgressPopup m_ProgressUI;

	[HideInInspector]
	public DungeonInfoPopup m_DungeonUI;

	[SerializeField]
	public FriendInfoElement m_FriendInfo;

	[SerializeField]
	public GameObject m_NotLoggedInIndicator;

	[SerializeField]
	public UILabel m_StarCollectionLabel;

	[SerializeField]
	public CharacterControllerCamp m_CharacterCampPrefab;

	[HideInInspector]
	public List<CharacterControllerCamp> m_CharactersCamp = new List<CharacterControllerCamp>();

	[HideInInspector]
	public List<BirdGameData> m_Birds = new List<BirdGameData>();

	[HideInInspector]
	public BirdWindowUIBase m_BirdManager;

	[HideInInspector]
	public SocialWindowUI m_SocialWindow;

	[SerializeField]
	public List<CampProp> m_CampProps = new List<CampProp>();

	[SerializeField]
	public CampProp m_ShopCamp;

	[SerializeField]
	public CampProp m_MailBoxCamp;

	[SerializeField]
	public CampProp m_FriendListCamp;

	[SerializeField]
	public CampProp m_RovioIdCamp;

	[SerializeField]
	public ParticleSystem m_RainbowRiotEffect;

	[SerializeField]
	public CampProp m_GoldenPigCamp;

	[SerializeField]
	public CampProp m_AdvGoldenPigCamp;

	[SerializeField]
	public GameObject m_FreeGachaSign;

	[SerializeField]
	public GameObject m_VideoGachaSign;

	[SerializeField]
	public List<Vector3List> m_BirdPositionsByCount = new List<Vector3List>();

	public List<bool> m_IsBirdMirrored = new List<bool>();

	private SocialWindowCategory m_cachedCategory;

	protected string m_birdName;

	public void EventuallyShowGooglePlaySignIn()
	{
		// if (DIContainerInfrastructure.GetCoreStateMgr().m_googlePlusAsked)
		// {
		// 	return;
		// }
		// bool? isSignedIn = DIContainerInfrastructure.GetAchievementService().IsSignedIn;
		// if (!isSignedIn.HasValue || !isSignedIn.Value)
		// {
		// 	DIContainerInfrastructure.GetCoreStateMgr().m_googlePlusAsked = true;
		// 	DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("social_google_signin_promo", "Sign in to Google+ and collect awesome Achievements!"), delegate
		// 	{
		// 		DIContainerInfrastructure.GetAchievementService().Init(DIContainerInfrastructure.GetCoreStateMgr(), true);
		// 	}, delegate
		// 	{
		// 	}, true, "ShopAndSocialElements", "GooglePlus");
		// } google plus 🤣 get real
	}

	public void UpdateLoggedInIndicator()
	{
		if (!ClientInfo.IsFriend && (bool)m_NotLoggedInIndicator)
		{
			m_NotLoggedInIndicator.SetActive(DIContainerInfrastructure.IdentityService.IsGuest());
		}
	}

	public List<CharacterControllerCamp> getBirds()
	{
		return m_CharactersCamp;
	}

	public void OnGachaLoaded()
	{
		m_GachaPopup = UnityEngine.Object.FindObjectOfType(typeof(GachaPopupUI)) as GachaPopupUI;
		m_GachaPopup.SetStateMgr(this);
		m_GachaPopup.Enter();
	}

	public void OnBirdManagerLoaded()
	{
		UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(BirdWindowUIBase));
		m_BirdManager = @object as BirdWindowUIBase;
		m_BirdManager.gameObject.SetActive(false);
		BirdGameData birdGameData = m_Birds.Where((BirdGameData b) => b.BalancingData.NameId == m_birdName).FirstOrDefault();
		if (birdGameData != null)
		{
			m_BirdManager.SetStateMgr(this).SetModel(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_Birds, m_Birds.IndexOf(birdGameData));
		}
	}

	public void ResetRiotAnim()
	{
		if (!DIContainerLogic.GetShopService().HasRainbowRiot(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			if (m_GoldenPigCamp != null)
			{
				m_GoldenPigCamp.PlayBoneAnimation("Idle");
			}
			if (m_RainbowRiotEffect != null)
			{
				m_RainbowRiotEffect.gameObject.SetActive(false);
			}
		}
	}

	public void CampStateMgr_MessageChanged(IMailboxMessageGameData obj)
	{
		if (!(m_MailBoxCamp == null))
		{
			m_MailBoxCamp.SetCounter(GetViewableMessagesCount());
		}
	}

	public int GetViewableMessagesCount()
	{
		return DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MailboxMessages.Values.Count((IMailboxMessageGameData m) => !m.IsViewed);
	}

	public void ShopCampOnPropClicked(BasicItemGameData item)
	{
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop(string.Empty, null);
	}

	public void MailBoxCampOnPropClicked(BasicItemGameData obj)
	{
		GoToSocial(SocialWindowCategory.Mailbox);
	}

	public virtual void RovioIdCampOnPropClicked(BasicItemGameData obj)
	{
		GoToSocial(SocialWindowCategory.RovioId);
	}

	public void FriendListCampOnPropClicked(BasicItemGameData obj)
	{
		GoToSocial(SocialWindowCategory.Friends);
	}

	public void GoToSocial(SocialWindowCategory category)
	{
		if (ClientInfo.IsFriend)
		{
			return;
		}
		m_cachedCategory = category;
		if (m_SocialWindow == null)
		{
			if (this is ArenaCampStateMgr)
			{
				DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_ArenaSocial", OnSocialLoaded);
			}
			else
			{
				DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_Social", OnSocialLoaded);
			}
		}
		else
		{
			m_SocialWindow.SetCategory(m_cachedCategory);
			m_SocialWindow.Enter();
		}
	}

	public void OnSocialLoaded()
	{
		m_SocialWindow = UnityEngine.Object.FindObjectOfType(typeof(SocialWindowUI)) as SocialWindowUI;
		m_SocialWindow.SetStateMgr(this);
		m_SocialWindow.SetCategory(m_cachedCategory);
		m_SocialWindow.Enter();
	}

	public void OnBirdClicked(ICharacter data)
	{
		if (!ClientInfo.IsFriend)
		{
			if (m_BirdManager == null)
			{
				m_birdName = data.Name;
				DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_BirdManager", OnBirdManagerLoaded);
			}
			else if (data != null)
			{
				m_BirdManager.SetStateMgr(this).SetModel(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_Birds, m_Birds.IndexOf(data as BirdGameData));
			}
		}
	}

	public void RemoveAllNewMarkersFromBird(BirdGameData bird)
	{
		foreach (IInventoryItemGameData item in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Class])
		{
			if (item.ItemData.IsNew && item.IsValidForBird(bird))
			{
				item.ItemData.IsNew = false;
			}
		}
		foreach (IInventoryItemGameData item2 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.MainHandEquipment])
		{
			if (item2.ItemData.IsNew && item2.IsValidForBird(bird))
			{
				item2.ItemData.IsNew = false;
			}
		}
		foreach (IInventoryItemGameData item3 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.OffHandEquipment])
		{
			if (item3.ItemData.IsNew && item3.IsValidForBird(bird))
			{
				item3.ItemData.IsNew = false;
			}
		}
		foreach (IInventoryItemGameData item4 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Skin])
		{
			if (item4.ItemData.IsNew && item4.IsValidForBird(bird))
			{
				item4.ItemData.IsNew = false;
			}
		}
		foreach (CharacterControllerCamp item5 in m_CharactersCamp)
		{
			if (item5.GetModel() == bird)
			{
				item5.ShowNewMarker(false);
			}
		}
	}

	private bool IsFreeFriendGacha()
	{
		if (!ClientInfo.IsFriend || ClientInfo.InspectedFriend == null)
		{
			return false;
		}
		bool flag = this is ArenaCampStateMgr;
		if (!flag && DIContainerLogic.SocialService.HasFreeGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer()))
		{
			return true;
		}
		if (flag && DIContainerLogic.SocialService.HasFreePvpGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer()))
		{
			return true;
		}
		return false;
	}

	protected void CheckForAdvancedGacha()
	{
		bool flag = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "story_goldenpig_advanced") >= 1;
		if (ClientInfo.IsFriend)
		{
			InventoryGameData inventory = new InventoryGameData(ClientInfo.InspectedFriend.PublicPlayerData.Inventory);
			flag = DIContainerLogic.InventoryService.GetItemValue(inventory, "story_goldenpig_advanced") >= 1;
		}
		if (flag)
		{
			m_AdvGoldenPigCamp.gameObject.SetActive(true);
			m_GoldenPigCamp.gameObject.SetActive(false);
			m_GoldenPigCamp = m_AdvGoldenPigCamp;
		}
	}

	public virtual void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if ((m_GoldenPigCamp.gameObject.activeInHierarchy && !ClientInfo.IsFriend) || IsFreeFriendGacha())
		{
			m_GoldenPigCamp.OnPropClicked += GoldenPigCampOnPropClicked;
		}
		else if (m_GoldenPigCamp.gameObject.activeInHierarchy && ClientInfo.IsFriend)
		{
			m_GoldenPigCamp.OnPropClicked += GoldenPigCampOnPropNotAvailiableClicked;
		}
		if (ClientInfo.IsFriend)
		{
			return;
		}
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MessageAdded += CampStateMgr_MessageChanged;
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MessageRemoved += CampStateMgr_MessageChanged;
		if (m_ShopCamp != null && m_ShopCamp.gameObject.activeInHierarchy)
		{
			m_ShopCamp.OnPropClicked += ShopCampOnPropClicked;
		}
		if (m_MailBoxCamp != null && m_MailBoxCamp.gameObject.activeInHierarchy)
		{
			m_MailBoxCamp.OnPropClicked += MailBoxCampOnPropClicked;
		}
		if (m_FriendListCamp.gameObject.activeInHierarchy)
		{
			m_FriendListCamp.OnPropClicked += FriendListCampOnPropClicked;
		}
		if (m_RovioIdCamp.gameObject.activeInHierarchy)
		{
			m_RovioIdCamp.OnPropClicked += RovioIdCampOnPropClicked;
		}
		foreach (CharacterControllerCamp item in m_CharactersCamp)
		{
			item.BirdClicked += OnBirdClicked;
		}
	}

	public virtual void RefreshCampContent()
	{
	}

	public virtual void DeRegisterEventHandler()
	{
		if (ClientInfo.IsFriend)
		{
			return;
		}
		if (m_GoldenPigCamp != null && m_GoldenPigCamp.gameObject.activeInHierarchy)
		{
			m_GoldenPigCamp.OnPropClicked -= GoldenPigCampOnPropClicked;
		}
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MessageAdded -= CampStateMgr_MessageChanged;
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.MessageRemoved -= CampStateMgr_MessageChanged;
		if ((bool)m_ShopCamp && m_ShopCamp.gameObject.activeInHierarchy)
		{
			m_ShopCamp.OnPropClicked -= ShopCampOnPropClicked;
		}
		if (m_MailBoxCamp != null && (bool)m_MailBoxCamp && m_MailBoxCamp.gameObject.activeInHierarchy)
		{
			m_MailBoxCamp.OnPropClicked -= MailBoxCampOnPropClicked;
		}
		if (m_FriendListCamp != null && m_FriendListCamp.gameObject.activeInHierarchy)
		{
			m_FriendListCamp.OnPropClicked -= FriendListCampOnPropClicked;
		}
		if (m_RovioIdCamp != null && m_RovioIdCamp.gameObject.activeInHierarchy)
		{
			m_RovioIdCamp.OnPropClicked -= RovioIdCampOnPropClicked;
		}
		if (m_CharactersCamp == null)
		{
			return;
		}
		foreach (CharacterControllerCamp item in m_CharactersCamp)
		{
			item.BirdClicked -= OnBirdClicked;
		}
	}

	public void RefreshBirdMarkers()
	{
		if (!ClientInfo.IsFriend || DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData == null || DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items == null)
		{
			return;
		}
		foreach (CharacterControllerCamp item in m_CharactersCamp)
		{
			bool flag = false;
			foreach (IInventoryItemGameData item2 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Class])
			{
				if (item2.ItemData.IsNew && item2.IsValidForBird(item.GetModel() as BirdGameData))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				item.ShowNewMarker(true);
				continue;
			}
			foreach (IInventoryItemGameData item3 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.MainHandEquipment])
			{
				if (item3.ItemData.IsNew && item3.IsValidForBird(item.GetModel() as BirdGameData))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				item.ShowNewMarker(true);
				continue;
			}
			foreach (IInventoryItemGameData item4 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.OffHandEquipment])
			{
				if (item4.ItemData.IsNew && item4.IsValidForBird(item.GetModel() as BirdGameData))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				item.ShowNewMarker(true);
				continue;
			}
			foreach (IInventoryItemGameData item5 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Skin])
			{
				if (item5.ItemData.IsNew && item5.IsValidForBird(item.GetModel() as BirdGameData))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				item.ShowNewMarker(true);
			}
		}
	}

	public void HideNewMarkerForBird(BirdGameData selectedBird)
	{
		RefreshBirdMarkers();
	}

	public void CheckForPiggieMcCoolVisits()
	{
		DateTime presentTime = DIContainerLogic.GetTimingService().GetPresentTime();
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (currentPlayer.Data.SocialEnvironment.McCoolSendsEssenceTimestamp == 0)
		{
			currentPlayer.SocialEnvironmentGameData.SetNewPiggieMcCoolDate(MessageType.ResponseFriendshipEssenceMessage);
		}
		if (currentPlayer.Data.SocialEnvironment.McCoolLendsBirdTimestamp == 0)
		{
			currentPlayer.SocialEnvironmentGameData.SetNewPiggieMcCoolDate(MessageType.ResponseBirdBorrowMessage);
		}
		if (currentPlayer.Data.SocialEnvironment.McCoolVisitsGachaTimestamp == 0)
		{
			currentPlayer.SocialEnvironmentGameData.SetNewPiggieMcCoolDate(MessageType.ResponseGachaUseMessage);
		}
		if (currentPlayer.Data.SocialEnvironment.McCoolVisitsPvpGachaTimestamp == 0)
		{
			currentPlayer.SocialEnvironmentGameData.SetNewPiggieMcCoolDate(MessageType.ResponsePvpGachaUseMessage);
		}
		List<MessageDataIncoming> list = new List<MessageDataIncoming>();
		DateTime dateTimeFromTimestamp = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(currentPlayer.Data.SocialEnvironment.McCoolVisitsGachaTimestamp);
		DateTime dateTimeFromTimestamp2 = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(currentPlayer.Data.SocialEnvironment.McCoolVisitsPvpGachaTimestamp);
		DateTime dateTimeFromTimestamp3 = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(currentPlayer.Data.SocialEnvironment.McCoolLendsBirdTimestamp);
		DateTime dateTimeFromTimestamp4 = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(currentPlayer.Data.SocialEnvironment.McCoolSendsEssenceTimestamp);
		if (dateTimeFromTimestamp <= presentTime)
		{
			MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
			messageDataIncoming.Id = Guid.NewGuid().ToString();
			messageDataIncoming.MessageType = MessageType.ResponseGachaUseMessage;
			messageDataIncoming.ReceivedAt = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
			messageDataIncoming.Sender = DIContainerLogic.SocialService.GetLowNPCFriend(currentPlayer.Data.Level);
			MessageDataIncoming item = messageDataIncoming;
			list.Add(item);
		}
		if (dateTimeFromTimestamp2 <= presentTime)
		{
			MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
			messageDataIncoming.Id = Guid.NewGuid().ToString();
			messageDataIncoming.MessageType = MessageType.ResponsePvpGachaUseMessage;
			messageDataIncoming.ReceivedAt = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
			messageDataIncoming.Sender = DIContainerLogic.SocialService.GetLowNPCFriend(currentPlayer.Data.Level);
			MessageDataIncoming item2 = messageDataIncoming;
			list.Add(item2);
		}
		if (dateTimeFromTimestamp3 <= presentTime)
		{
			MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
			messageDataIncoming.Id = Guid.NewGuid().ToString();
			messageDataIncoming.MessageType = MessageType.ResponseBirdBorrowMessage;
			messageDataIncoming.ReceivedAt = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
			messageDataIncoming.Sender = DIContainerLogic.SocialService.GetLowNPCFriend(currentPlayer.Data.Level);
			messageDataIncoming.Parameter1 = "bird_red";
			MessageDataIncoming item3 = messageDataIncoming;
			list.Add(item3);
		}
		if (dateTimeFromTimestamp4 <= presentTime)
		{
			MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
			messageDataIncoming.Id = Guid.NewGuid().ToString();
			messageDataIncoming.MessageType = MessageType.ResponseFriendshipEssenceMessage;
			messageDataIncoming.ReceivedAt = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
			messageDataIncoming.Sender = DIContainerLogic.SocialService.GetLowNPCFriend(currentPlayer.Data.Level);
			MessageDataIncoming item4 = messageDataIncoming;
			list.Add(item4);
		}
		currentPlayer.SocialEnvironmentGameData.AddIncomingMessages(list);
		currentPlayer.SavePlayerData();
	}

	public virtual void ShowLeaderBoardScreen(bool directly)
	{
	}

	public void GoToGacha()
	{
		if (m_GachaPopup == null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Popup_Gacha", OnGachaLoaded);
		}
		else
		{
			m_GachaPopup.Enter();
		}
	}

	public void GoldenPigCampOnPropClicked(BasicItemGameData obj)
	{
		bool flag = this is ArenaCampStateMgr;
		if (ClientInfo.IsFriend && !flag && !DIContainerLogic.SocialService.HasFreeGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer()))
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_nofreeroll", "No free roll available yet! Visit your other friends for a free roll."), "nofreeroll", DispatchMessage.Status.Info);
			return;
		}
		if (ClientInfo.IsFriend && flag && !DIContainerLogic.SocialService.HasFreePvpGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer()))
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_nofreeroll", "No free roll available yet! Visit your other friends for a free roll."), "nofreeroll", DispatchMessage.Status.Info);
			return;
		}
		DebugLog.Log("Gacha Clicked!");
		OnGachaClicked();
	}

	private void GoldenPigCampOnPropNotAvailiableClicked(BasicItemGameData obj)
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_nofreeroll", "No free roll available! Come back later."), "nofreeroll", DispatchMessage.Status.Info);
	}

	protected void OnGachaClicked()
	{
		GoToGacha();
		CancelInvoke("CheckAndPlayRiotAgain");
		Invoke("CheckAndPlayRiotAgain", 3f);
	}

	private void CheckAndPlayRiotAgain()
	{
		CheckAndSetRainbowRiot();
	}

	public void UpdateFreeGachaSign()
	{
		bool flag = this is ArenaCampStateMgr;
		if (ClientInfo.IsFriend)
		{
			if (!flag && DIContainerLogic.SocialService.HasFreeGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer()))
			{
				m_FreeGachaSign.SetActive(true);
			}
			else if (flag && DIContainerLogic.SocialService.HasFreePvpGachaRoll(ClientInfo.InspectedFriend, DIContainerInfrastructure.GetCurrentPlayer()))
			{
				m_FreeGachaSign.SetActive(true);
			}
			else
			{
				m_FreeGachaSign.SetActive(false);
				m_GoldenPigCamp.SetClickable(false);
			}
		}
		else
		{
			if ((bool)m_VideoGachaSign && (m_GoldenPigCamp.gameObject.activeSelf || m_AdvGoldenPigCamp.gameObject.activeSelf))
			{
				uint num = 0u;
				num = ((!flag) ? DIContainerInfrastructure.GetCurrentPlayer().Data.TimeStampOfLastVideoGacha : DIContainerInfrastructure.GetCurrentPlayer().Data.TimeStampOfLastVideoPvPGacha);
				uint num2 = num + (uint)(DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").GachaVideoTimespan * 60);
				// m_VideoGachaSign.SetActive(num2 <= DIContainerLogic.GetTimingService().GetCurrentTimestamp()); no ad indicator because there are no ads..
				m_VideoGachaSign.SetActive(false);
			}
			if ((bool)m_FriendListCamp && !flag)
			{
				m_FriendListCamp.SetCounter(DIContainerLogic.SocialService.HasAnyGachaFreeRoll(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData) ? 1 : 0);
			}
			else if ((bool)m_FriendListCamp && flag)
			{
				m_FriendListCamp.SetCounter(DIContainerLogic.SocialService.HasAnyPvpGachaFreeRoll(DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData) ? 1 : 0);
			}
		}
		CheckAndSetRainbowRiot();
	}

	private void CheckAndSetRainbowRiot()
	{
		if (!ClientInfo.IsFriend)
		{
			if (DIContainerLogic.GetShopService().HasRainbowRiot(DIContainerInfrastructure.GetCurrentPlayer()) && m_RainbowRiotEffect != null)
			{
				m_GoldenPigCamp.PlayBoneAnimation("RainbowRiot");
				m_RainbowRiotEffect.gameObject.SetActive(true);
				m_RainbowRiotEffect.Play();
			}
			else if (m_RainbowRiotEffect != null)
			{
				m_RainbowRiotEffect.gameObject.SetActive(false);
			}
		}
	}
}
