using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Rcs;
using UnityEngine;

public class BattlePreperationUI : MonoBehaviour
{
	[Header("Battle Hints")]
	[SerializeField]
	private BattleHintPopup m_battleHintPopup;

	[SerializeField]
	private UIInputTrigger m_battleHintTrigger;

	[SerializeField]
	private UILabel m_battleHintHeader;

	[SerializeField]
	private UILabel m_battleHintDesc;

	[SerializeField]
	[Header("Animations")]
	private Animation m_BackAnimation;

	[SerializeField]
	private Animation m_StartAnimation;

	[SerializeField]
	private Animation m_SkipAnimation;

	[SerializeField]
	private Animation m_BattleLockedAnimation;

	[SerializeField]
	private Animation m_classChangeButtonAnimation;

	[Header("Sponsored Buff")]
	[SerializeField]
	private GameObject m_HealthBuffRoot;

	[SerializeField]
	private UIInputTrigger m_classChangeButton;

	[SerializeField]
	private GameObject m_SponsoredBuffRootLarge;

	[SerializeField]
	private UIInputTrigger m_SponsoredBuffButtonLarge;

	[SerializeField]
	private GameObject m_SponsoredBuffButtonRootLarge;

	[SerializeField]
	private GameObject m_SponsoredBuffCheckRootLarge;

	[SerializeField]
	private UILabel m_SponsoredTextLarge;

	[SerializeField]
	[Header("Board")]
	private UISprite m_battleTypeSprite;

	[SerializeField]
	private GameObject m_Spacer;

	[SerializeField]
	private GameObject m_DungeonSpecialLootRoot;

	[SerializeField]
	private UILabel m_TopLootDungeonText;

	[SerializeField]
	private GameObject m_BattleRules;

	[SerializeField]
	public UIGrid m_RulesGrid;

	[SerializeField]
	private GameObject m_RulesPrefabBirdNumber;

	[SerializeField]
	private GameObject m_RulesPrefabBirdForbidden;

	[SerializeField]
	private GameObject m_RulesPrefabBirdRequired;

	[SerializeField]
	private GameObject m_RulesPrefabEnvironmentEffect;

	[SerializeField]
	private UILabel m_birdPowerLevel;

	[SerializeField]
	private UILabel m_pigPowerLevel;

	[SerializeField]
	private List<Transform> m_pigPreviewRoots;

	[SerializeField]
	private CharacterControllerWorldMap m_pigPrefab;

	[SerializeField]
	private UILabel m_StageNameText;

	[SerializeField]
	private GameObject m_WaveRoot;

	[SerializeField]
	private UILabel m_WaveCountText;

	[SerializeField]
	private UILabel m_TopLootCountText;

	[SerializeField]
	private UILabel m_BirdCountText;

	[SerializeField]
	public UIInputTrigger m_startButtonTrigger;

	[SerializeField]
	private UISprite m_StartButtonSprite;

	public Color m_PowerLevelColorDefault;

	public Color m_PowerLevelColorEasy;

	public Color m_PowerLevelColorNormal;

	public Color m_PowerLevelColorHard;

	[SerializeField]
	[Header("Options")]
	private UIInputTrigger m_dungeonNormalModeButton;

	[SerializeField]
	private UIInputTrigger m_dungeonHardModeButton;

	[SerializeField]
	private Animator m_dungeonNormalModeButtonAnimation;

	[SerializeField]
	private Animator m_dungeonHardModeButtonAnimation;

	[SerializeField]
	public UIInputTrigger m_backButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_skipButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_energyPotionButtonTrigger;

	[SerializeField]
	private UILabel m_energyPotionAmount;

	[SerializeField]
	private ResourceCostBlind m_skipCost;

	[Header("FriendBird")]
	[SerializeField]
	private UIInputTrigger m_chooseFriendBirdButtonTrigger;

	[SerializeField]
	private GameObject m_FriendBirdRoot;

	[SerializeField]
	private Animation m_AddFriendBirdButtonAnimation;

	[SerializeField]
	private Transform m_FriendBirdParent;

	[SerializeField]
	private LootDisplayContoller m_MajorLoot;

	[SerializeField]
	private UIAtlas m_GenericIconsAtlas;

	[SerializeField]
	private UIAtlas m_ResourceIconsAtlas;

	[SerializeField]
	private Transform m_CharacterRoot;

	[SerializeField]
	private GameObject m_CharacterButtonPrefab;

	[Header("Locked")]
	[SerializeField]
	private GameObject m_BattleLockedTimerRoot;

	[SerializeField]
	private GameObject m_BattleLockedInfoRoot;

	[SerializeField]
	private GameObject m_BattleLockedRoot;

	[SerializeField]
	private UILabel m_BattleLockedTimer;

	[SerializeField]
	private UILabel m_BattleLockedInfo;

	[Header("Event")]
	[SerializeField]
	private GameObject m_EventEnergyRoot;

	[SerializeField]
	private ResourceCostBlind m_EventEnergyCost;

	[SerializeField]
	private UILabel m_EventEnergyDescription;

	private float m_lastAdCancelledTime;

	private float m_lastAdCompletedTime;

	private ClassManagerUi m_classMgr;

	[SerializeField]
	private UIGrid m_BirdGrid;

	private BattleBalancingData m_battleBalancing;

	private BattleHintBalancingData m_battleHintBalancing;

	private HotspotBalancingData m_hotspotBalancing;

	private List<BattlePrepCharacterButton> m_buttonList = new List<BattlePrepCharacterButton>();

	private FriendGameData m_SelectedFriend;

	private BattleParticipantTableBalancingData m_GoldenPigAddtion;

	private SkillBattleDataBase m_SponsoredAdSkill;

	private bool m_EnteringBattle;

	private BaseLocationStateManager m_worldMapStateMgr;

	private HotspotGameData m_HotspotGameData;

	private bool m_Locked;

	[HideInInspector]
	public bool m_OneBirdLeft;

	private float m_BattleLevel;

	public float m_RotationOffset = 270f;

	private int m_MaxSelectableBirdsCount;

	private int m_FriendBirdsCount;

	private List<int> m_SelectedBirds;

	private bool m_birdSelectionFirst;

	public static string BUFF_PLACEMENT = "RewardVideo.BattleBuff";

	public bool m_Entered;

	private bool m_EnergyFooterActive;

	private string m_forcedBird;

	private bool m_isEventBattle;

	private Action m_ActionOnLeave;

	private bool m_hardModeSelected;

	[SerializeField]
	[Header("Hard Mode Stuff")]
	private GameObject m_InactiveHardButton;

	[SerializeField]
	private UILabel m_InactiveHardmodeDescription;

	[SerializeField]
	private GameObject m_genericBattleInfos;

	private int m_ownPowerLevel;

	private List<string> m_previewPigNames;

	private float m_currentPigStrength;

	[HideInInspector]
	public UIInputTrigger ClassChangeButton
	{
		get
		{
			return m_classChangeButton;
		}
	}

	private void Awake()
	{
		m_hardModeSelected = false;
		InvokeRepeating("CheckIfLocked", 1f, 1f);
		base.transform.position += DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.transform.position;
		m_SponsoredAdSkill = new SkillGameData(DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").SponsoredAdBuffName).GenerateSkillBattleData();
		CreateBirds();
	}

	private void OpenClassInfoScreen()
	{
		if (m_classMgr == null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Window_ClassManager", OnClassInfoLoaded);
			return;
		}
		Leave(true);
		m_classMgr.EnterClassManager(false, this);
	}

	public void OnClassInfoLoaded()
	{
		m_classMgr = UnityEngine.Object.FindObjectOfType(typeof(ClassManagerUi)) as ClassManagerUi;
		Leave(true);
		m_classMgr.EnterClassManager(false, this);
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(1, HandleBackButton);
		m_startButtonTrigger.Clicked += OnStartButtonClicked;
		m_backButtonTrigger.Clicked += OnBackButtonClicked;
		m_chooseFriendBirdButtonTrigger.Clicked += OnChooseFriendBirdButtonTriggerClicked;
		m_skipButtonTrigger.Clicked += OnSkipButtonClicked;
		m_SponsoredBuffButtonLarge.Clicked += OnSponsoredBuffButtonClicked;
		m_energyPotionButtonTrigger.Clicked += EnergyPotionButtonTriggerClicked;
		m_classChangeButton.Clicked += OpenClassInfoScreen;
		m_battleHintTrigger.Clicked += OpenBattleHint;
		m_dungeonHardModeButton.Clicked += SwitchBoards;
		m_dungeonNormalModeButton.Clicked += SwitchBoards;
		DIContainerInfrastructure.GetCoreStateMgr().OnShopClosed += OnShopClosed;
		DIContainerInfrastructure.AdService.RewardResult += RewardSponsoredAdResult;
	}

	private void DeregisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(1);
		m_startButtonTrigger.Clicked -= OnStartButtonClicked;
		m_backButtonTrigger.Clicked -= OnBackButtonClicked;
		m_chooseFriendBirdButtonTrigger.Clicked -= OnChooseFriendBirdButtonTriggerClicked;
		m_skipButtonTrigger.Clicked -= OnSkipButtonClicked;
		m_SponsoredBuffButtonLarge.Clicked -= OnSponsoredBuffButtonClicked;
		m_energyPotionButtonTrigger.Clicked -= EnergyPotionButtonTriggerClicked;
		m_classChangeButton.Clicked -= OpenClassInfoScreen;
		m_battleHintTrigger.Clicked -= OpenBattleHint;
		m_dungeonHardModeButton.Clicked -= SwitchBoards;
		m_dungeonNormalModeButton.Clicked -= SwitchBoards;
		DIContainerInfrastructure.GetCoreStateMgr().OnShopClosed -= OnShopClosed;
		DIContainerInfrastructure.AdService.RewardResult -= RewardSponsoredAdResult;
	}

	private void OpenBattleHint()
	{
		m_battleHintPopup.Enter(m_battleHintBalancing);
	}

	private void OnShopClosed()
	{
		int itemValue = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "potion_energy");
		if (itemValue > 0)
		{
			m_energyPotionAmount.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(itemValue);
			m_EventEnergyDescription.text = DIContainerInfrastructure.GetLocaService().Tr("bps_energypotion_desc");
		}
		else
		{
			m_energyPotionAmount.text = DIContainerInfrastructure.GetLocaService().Tr("special_offer_shop", "Shop");
			m_EventEnergyDescription.text = DIContainerInfrastructure.GetLocaService().Tr("bps_eventbattlefooter");
		}
	}

	private void EnergyPotionButtonTriggerClicked()
	{
		IInventoryItemGameData data = null;
		if (DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "potion_energy") <= 0)
		{
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "event_energy", out data) && data.ItemBalancing.NameId == "event_energy")
			{
				int startIndex = 0;
				string category = "shop_global_consumables";
				DIContainerInfrastructure.GetCoreStateMgr().ShowShop(category, RefreshEnergyValues, startIndex);
			}
			return;
		}
		IInventoryItemGameData data2 = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "potion_energy", out data2))
		{
			if (DIContainerLogic.EventSystemService.HasMaximumEnergy(DIContainerInfrastructure.GetCurrentPlayer()))
			{
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("event_energy_max_reached", "You have already reached maximum energy!"), "energy", DispatchMessage.Status.Info);
				return;
			}
			ConsumableItemGameData consumableItemGameData = data2 as ConsumableItemGameData;
			SkillBattleDataBase skillBattleDataBase = consumableItemGameData.ConsumableSkill.GenerateSkillBattleData();
			skillBattleDataBase.DoActionInstant(null, null, null);
			DIContainerLogic.InventoryService.RemoveItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "potion_energy", 1, "used_energy_potion");
			m_energyPotionAmount.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "potion_energy"));
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateEventEnergyBar();
			RefreshEnergyValues();
		}
	}

	private void ShowMissingStaminaPopup()
	{
		float value = m_battleBalancing.BattleRequirements.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem && r.NameId.Contains("event_energy")).Value;
		DIContainerInfrastructure.GetCoreStateMgr().m_MissingEnergyPopup.gameObject.SetActive(true);
		StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_MissingEnergyPopup.ShowPopup(value, this));
	}

	private void OnSponsoredBuffButtonClicked()
	{
		if (DIContainerInfrastructure.AdService.IsAdShowPossible(BUFF_PLACEMENT) && string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentSponsoredBuff))
		{
			m_SponsoredBuffRootLarge.SetActive(false);
			if (!DIContainerInfrastructure.AdService.ShowAd(BUFF_PLACEMENT))
			{
				m_SponsoredBuffRootLarge.SetActive(true);
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_no_ad_available", "There is currently no Ad scheduled"), "no_ad", DispatchMessage.Status.Info);
			}
			else
			{
				m_startButtonTrigger.gameObject.SetActive(false);
				DIContainerInfrastructure.AdService.MutedGameSoundForPlacement(BUFF_PLACEMENT);
			}
		}
	}

	private void RewardSponsoredAdResult(string placement, Ads.RewardResult result, string voucherId)
	{
		if (placement != BUFF_PLACEMENT)
		{
			return;
		}
		DebugLog.Log("[GachaPopupUI] Reward Result received: " + result);
		m_startButtonTrigger.gameObject.SetActive(true);
		switch (result)
		{
		case Ads.RewardResult.RewardCanceled:
			m_lastAdCancelledTime = Time.time;
			break;
		case Ads.RewardResult.RewardCompleted:
			m_lastAdCompletedTime = Time.time;
			break;
		case Ads.RewardResult.RewardConfirmed:
			if (m_lastAdCancelledTime > m_lastAdCompletedTime)
			{
				if (Time.time - m_lastAdCancelledTime < 60f)
				{
					OnAdAbortedForBuffRoll();
				}
			}
			else if (Time.time - m_lastAdCompletedTime < 60f)
			{
				OnAdWatchedForBuffRoll();
			}
			break;
		case Ads.RewardResult.RewardFailed:
			OnAdAbortedForBuffRoll();
			break;
		default:
			throw new ArgumentOutOfRangeException("result");
		}
	}

	private void OnAdWatchedForBuffRoll()
	{
		DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentSponsoredBuff = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").SponsoredAdBuffName;
		m_SponsoredBuffButtonLarge.Clicked -= OnSponsoredBuffButtonClicked;
		if (string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentSponsoredBuff))
		{
			DebugLog.Log("OnAdWatchedForBuffRoll: DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentSponsoredBuff is empty or null");
			StartCoroutine(LeaveSponsoredAdsBar());
		}
		else
		{
			m_SponsoredBuffRootLarge.SetActive(true);
			m_SponsoredBuffButtonRootLarge.SetActive(false);
			m_SponsoredBuffCheckRootLarge.SetActive(true);
		}
		UpdateBirdPowerLevel();
	}

	private IEnumerator LeaveSponsoredAdsBar()
	{
		m_SponsoredBuffRootLarge.GetComponent<Animation>().Play("Footer_Leave");
		yield return new WaitForSeconds(m_SponsoredBuffRootLarge.GetComponent<Animation>()["Footer_Leave"].clip.length);
		m_SponsoredBuffRootLarge.SetActive(false);
	}

	private void OnAdAbortedForBuffRoll()
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_advideo_cancelled", "You did not watch the whole video"));
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		Leave();
	}

	private void OnChooseFriendBirdButtonTriggerClicked()
	{
		UpdateSelectedBirds();
		m_ActionOnLeave = delegate
		{
			DIContainerInfrastructure.GetCoreStateMgr().ShowFriendList(FriendListType.GetBird, EnterSec, SelectFriend, m_SelectedFriend);
		};
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 3u
		}, false);
		Leave();
	}

	public void PopupMissingEnergyUIStartHandler()
	{
		if (m_FriendBirdsCount <= 0 || m_SelectedFriend != null)
		{
			OnStartButtonClicked();
		}
	}

	public void OnStartButtonClicked()
	{
		if (IsDungeon() && !AllowHardMode() && m_hardModeSelected)
		{
			return;
		}
		List<BirdGameData> list = new List<BirdGameData>();
		foreach (BattlePrepCharacterButton button in m_buttonList)
		{
			if (button.IsSelected())
			{
				list.Add(button.GetBirdGameData());
			}
		}
		if (list.Count > m_MaxSelectableBirdsCount)
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("gen_toast_battlewithtoomuchbirds", "?Can't enter battle with more than 3 birds?"), "too_much_birds", DispatchMessage.Status.Info);
			return;
		}
		if (list.Count == 0)
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("gen_toast_battlewithnobirds", "?Can't enter battle with no own birds?"), "no_birds", DispatchMessage.Status.Info);
			return;
		}
		List<Requirement> failedRequirements = new List<Requirement>();
		Requirement firstFailedReq = null;
		if (!DIContainerLogic.WorldMapService.IsHotspotEnterable(DIContainerInfrastructure.GetCurrentPlayer(), m_HotspotGameData, out firstFailedReq) || !DIContainerLogic.RequirementService.CheckGenericRequirements(DIContainerInfrastructure.GetCurrentPlayer(), (m_battleBalancing.BattleRequirements == null) ? null : Enumerable.ToList(m_battleBalancing.BattleRequirements.Where((Requirement r) => DIContainerLogic.GetBattleService().GetBattleRulesRequirements().Contains(r.RequirementType) || r.RequirementType == RequirementType.PayItem)), out failedRequirements) || !DIContainerLogic.RequirementService.ExecuteRequirements(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_battleBalancing.BattleRequirements, "enter_battle"))
		{
			if (firstFailedReq == null)
			{
				firstFailedReq = failedRequirements.FirstOrDefault();
			}
			switch (firstFailedReq.RequirementType)
			{
			case RequirementType.CooldownFinished:
				DebugLog.Log("CoolDown not finished, time left in Seconds: " + firstFailedReq.Value);
				break;
			case RequirementType.PayItem:
			{
				IInventoryItemGameData data = null;
				if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, firstFailedReq.NameId, out data))
				{
					if (data.ItemBalancing.NameId == "lucky_coin")
					{
						DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[1].m_StatBar.SwitchToShop();
					}
					else if (data.ItemBalancing.NameId == "gold")
					{
						DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[0].m_StatBar.SwitchToShop();
					}
					else if (data.ItemBalancing.NameId == "friendship_essence")
					{
						DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[2].m_StatBar.SwitchToShop();
					}
					else if (data.ItemBalancing.NameId == "event_energy")
					{
						ShowMissingStaminaPopup();
					}
				}
				break;
			}
			case RequirementType.NotUseBirdInBattle:
			{
				string characterName2 = DIContainerInfrastructure.GetLocaService().GetCharacterName(DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>(firstFailedReq.NameId).LocaId);
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_rule_notallowedbird", "?You tried to enter a battle with a restricted bird?").Replace("{value_1}", characterName2), "bird_restricted", DispatchMessage.Status.Info);
				break;
			}
			case RequirementType.UseBirdInBattle:
			{
				string characterName = DIContainerInfrastructure.GetLocaService().GetCharacterName(DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>(firstFailedReq.NameId).LocaId);
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_ruleneededbird", "?You have not selected a required bird?").Replace("{value_1}", characterName), "bird_missing", DispatchMessage.Status.Info);
				break;
			}
			}
			return;
		}
		if (m_SelectedFriend != null && m_SelectedFriend.IsFriendBirdLoaded)
		{
			if (DIContainerLogic.SocialService.IsGetFriendBirdPossible(DIContainerInfrastructure.GetCurrentPlayer(), m_SelectedFriend) || !string.IsNullOrEmpty(m_forcedBird))
			{
				list.Add(m_SelectedFriend.FriendBird);
				if (!m_SelectedFriend.isNpcFriend)
				{
					MessageDataIncoming messageDataIncoming = new MessageDataIncoming();
					messageDataIncoming.MessageType = MessageType.ResponseBirdBorrowMessage;
					messageDataIncoming.Sender = DIContainerInfrastructure.GetCurrentPlayer().GetFriendData();
					messageDataIncoming.SentAt = DIContainerLogic.GetDeviceTimingService().GetCurrentTimestamp();
					messageDataIncoming.Parameter1 = m_SelectedFriend.FriendBird.BalancingData.NameId;
					MessageDataIncoming message = messageDataIncoming;
					ABHAnalyticsHelper.SendSocialEvent(message, m_SelectedFriend.FriendData);
					DIContainerInfrastructure.MessagingService.SendMessages(message, new List<string> { m_SelectedFriend.FriendId });
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("BirdName", m_SelectedFriend.FriendBird.BalancingData.NameId);
				dictionary.Add("BattleName", m_battleBalancing.NameId);
				ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
				ABHAnalyticsHelper.AddFriendsCountToTracking(dictionary);
				DIContainerInfrastructure.GetAnalyticsSystem().LogEventWithParameters("FriendBirdUsed", dictionary);
				m_SelectedFriend.UseFriendBird();
				if (m_SelectedFriend.isNpcFriend && m_SelectedFriend.HasPaid)
				{
					m_SelectedFriend.HasPaid = false;
				}
				else
				{
					DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
					{
						DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.GetBirdCooldowns[m_SelectedFriend.FriendId] = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
						DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
					});
				}
			}
			else
			{
				DebugLog.Error("Time to lend bird not done!");
			}
		}
		m_EnteringBattle = true;
		Leave();
		list = Enumerable.ToList(list.OrderBy((BirdGameData b) => b.BalancingData.SortPriority));
		m_worldMapStateMgr.StartBattle(m_HotspotGameData, list, m_GoldenPigAddtion, m_hardModeSelected);
	}

	public void OnBackButtonClicked()
	{
		Leave();
	}

	public void OnSkipButtonClicked()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (DIContainerLogic.RequirementService.ExecuteRequirements(currentPlayer.InventoryGameData, new List<Requirement> { DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").DungeonSkipRequirement }, new Dictionary<string, string>
		{
			{ "TypeOfUse", "SkipDungeonCooldown" },
			{ "DungeonName", m_hotspotBalancing.NameId },
			{
				"Day",
				DateTime.Now.DayOfWeek.ToString()
			}
		}))
		{
			currentPlayer.Data.TemporaryOpenHotspots.Add(m_hotspotBalancing.NameId);
			HotSpotWorldMapViewBattle hotSpotWorldMapViewBattle = m_HotspotGameData.WorldMapView as HotSpotWorldMapViewBattle;
			if (hotSpotWorldMapViewBattle != null)
			{
				hotSpotWorldMapViewBattle.SetChainsEnabled(false);
			}
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
			currentPlayer.SavePlayerData();
			m_skipButtonTrigger.Clicked -= OnSkipButtonClicked;
			return;
		}
		Requirement nextClassSkipRequirement = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").NextClassSkipRequirement;
		if (nextClassSkipRequirement != null && nextClassSkipRequirement.RequirementType == RequirementType.PayItem)
		{
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(currentPlayer.InventoryGameData, nextClassSkipRequirement.NameId, out data))
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_MissingCurrencyPopup.EnterPopup(data.ItemBalancing.NameId, nextClassSkipRequirement.Value);
			}
		}
	}

	public void SetEventPlacement(EventItemGameData eventItem, EventPlacementBalancingData placement, WorldMapStateMgr mapMgr)
	{
		if (eventItem.BalancingData.ItemType == InventoryItemType.EventBattleItem || eventItem.BalancingData.ItemType == InventoryItemType.EventBossItem)
		{
			SetEventInvasionBattlePlacement(eventItem, placement, mapMgr);
		}
	}

	public void SetEventInvasionBattlePlacement(EventItemGameData eventItem, EventPlacementBalancingData placement, WorldMapStateMgr mapMgr)
	{
		if (eventItem.Data.Quality == 0)
		{
			eventItem.Data.Quality = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		}
		if (eventItem.BalancingData.ItemType != InventoryItemType.EventBattleItem && eventItem.BalancingData.ItemType != InventoryItemType.EventBossItem)
		{
			DebugLog.Error("[EventSystem] The current event Item (" + eventItem.BalancingData.NameId + ") is no Battle Event Item!");
			return;
		}
		HotspotBalancingData hotspotBalancingData = new HotspotBalancingData();
		hotspotBalancingData.BattleId = eventItem.BalancingData.EventParameters;
		hotspotBalancingData.NameId = placement.NameId + "_battleground";
		hotspotBalancingData.ZoneLocaIdent = eventItem.BalancingData.LocaBaseId + "_battleground";
		hotspotBalancingData.Type = HotspotType.Battle;
		hotspotBalancingData.ZoneStageIndex = 0;
		hotspotBalancingData.IsSpawnEventPossible = true;
		HotspotGameData hotspotGameData = new HotspotGameData(hotspotBalancingData, new HotspotData
		{
			NameId = eventItem.BalancingData.NameId + "_battleground",
			RandomSeed = eventItem.Data.Quality
		});
		if (!string.IsNullOrEmpty(placement.OverrideBattleGroundName))
		{
			hotspotGameData.OverrideBattleGround = placement.OverrideBattleGroundName;
		}
		SetEventHotSpot(hotspotGameData, mapMgr);
	}

	public void SetMiniCampaignHotSpot(HotspotGameData spotData, EventCampaignStateMgr mapMgr)
	{
		DebugLog.Log("BPS: Setting Mini Campaign Hotspot...");
		m_worldMapStateMgr = mapMgr;
		m_hotspotBalancing = spotData.BalancingData;
		m_HotspotGameData = spotData;
		spotData.BalancingData.IsSpawnEventPossible = true;
		Requirement firstFailedReq = null;
		m_Locked = false;
		if (!DIContainerLogic.WorldMapService.IsHotspotEnterable(DIContainerInfrastructure.GetCurrentPlayer(), m_HotspotGameData, out firstFailedReq))
		{
			m_Locked = true;
			FillNotUnlockedInfo(firstFailedReq);
		}
		DebugLog.Log("BPS: Get Battle Balancing");
		m_battleBalancing = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(DIContainerLogic.GetBattleService().GetFirstPossibleBattle(m_hotspotBalancing.BattleId, DIContainerInfrastructure.GetCurrentPlayer(), false, true));
		m_BattleLevel = ((m_battleBalancing.BaseLevel <= 0) ? DIContainerInfrastructure.GetCurrentPlayer().Data.Level : m_battleBalancing.BaseLevel);
		SetVideoBuffInfoSize();
		m_MaxSelectableBirdsCount = m_battleBalancing.MaxBirdsInBattle - m_battleBalancing.UsableFriendBirdsCount;
		m_FriendBirdsCount = m_battleBalancing.UsableFriendBirdsCount;
		m_forcedBird = m_battleBalancing.Force_Character;
		m_BattleRules.SetActive(true);
		foreach (Transform item in m_RulesGrid.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		Invoke("SpawnEventRules", Time.deltaTime);
		m_SelectedFriend = null;
	}

	public void SetHotSpot(HotspotGameData spotData, WorldMapStateMgr mapMgr)
	{
		m_worldMapStateMgr = mapMgr;
		m_hotspotBalancing = spotData.BalancingData;
		m_HotspotGameData = spotData;
		Requirement firstFailedReq = null;
		m_Locked = false;
		if (!DIContainerLogic.WorldMapService.IsHotspotEnterable(DIContainerInfrastructure.GetCurrentPlayer(), m_HotspotGameData, out firstFailedReq) && (firstFailedReq.RequirementType == RequirementType.CooldownFinished || firstFailedReq.RequirementType == RequirementType.IsSpecificWeekday) && !DIContainerInfrastructure.GetCurrentPlayer().Data.TemporaryOpenHotspots.Contains(m_hotspotBalancing.NameId))
		{
			m_Locked = true;
			if (m_hotspotBalancing.EnterRequirements != null)
			{
				foreach (Requirement enterRequirement in m_hotspotBalancing.EnterRequirements)
				{
					if (enterRequirement.RequirementType == RequirementType.IsSpecificWeekday && (DIContainerLogic.GetTimingService().GetPresentTime().DayOfWeek != (DayOfWeek)(int)Enum.Parse(typeof(DayOfWeek), enterRequirement.NameId, true) || DIContainerInfrastructure.GetCurrentPlayer().Data.DungeonsAlreadyPlayedToday.Contains(m_hotspotBalancing.NameId)))
					{
						firstFailedReq = enterRequirement;
					}
				}
			}
			if (firstFailedReq != null)
			{
				FillNotUnlockedInfo(firstFailedReq);
			}
		}
		m_battleBalancing = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(DIContainerLogic.GetBattleService().GetFirstPossibleBattle(m_hotspotBalancing.BattleId, DIContainerInfrastructure.GetCurrentPlayer()));
		if (DIContainerLogic.WorldMapService.TryGetGoldenPigBattleAddition(DIContainerInfrastructure.GetCurrentPlayer(), spotData, out m_GoldenPigAddtion))
		{
			m_BattleLevel = ((m_battleBalancing.BaseLevel <= 0) ? DIContainerInfrastructure.GetCurrentPlayer().Data.Level : ((m_battleBalancing.BaseLevel + DIContainerInfrastructure.GetCurrentPlayer().Data.Level) / 2));
		}
		else
		{
			m_BattleLevel = ((m_battleBalancing.BaseLevel <= 0) ? DIContainerInfrastructure.GetCurrentPlayer().Data.Level : m_battleBalancing.BaseLevel);
		}
		SetVideoBuffInfoSize();
		if (m_hotspotBalancing.NameId.Contains("dungeon"))
		{
			m_InactiveHardmodeDescription.text = DIContainerInfrastructure.GetLocaService().Tr("bps_dungeon_mode_hard_desc").Replace("{value_1}", m_HotspotGameData.StageNamePure);
		}
		m_MaxSelectableBirdsCount = m_battleBalancing.MaxBirdsInBattle - m_battleBalancing.UsableFriendBirdsCount;
		m_FriendBirdsCount = m_battleBalancing.UsableFriendBirdsCount;
		m_forcedBird = m_battleBalancing.Force_Character;
		m_BattleRules.SetActive(false);
		m_EventEnergyCost.gameObject.SetActive(false);
		m_EventEnergyRoot.gameObject.SetActive(false);
		m_SelectedFriend = null;
	}

	private void SetHardBalancing()
	{
		m_battleBalancing = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(DIContainerLogic.GetBattleService().GetFirstPossibleBattle(m_hotspotBalancing.BattleId, DIContainerInfrastructure.GetCurrentPlayer(), false, true));
		if (m_battleBalancing == null)
		{
			m_battleBalancing = DIContainerBalancing.Service.GetBalancingData<ChronicleCaveBattleBalancingData>(DIContainerLogic.GetBattleService().GetFirstPossibleBattle(m_hotspotBalancing.BattleId, DIContainerInfrastructure.GetCurrentPlayer(), true, true));
		}
		m_BattleLevel = ((m_battleBalancing.BaseLevel <= 0) ? DIContainerInfrastructure.GetCurrentPlayer().Data.Level : m_battleBalancing.BaseLevel);
		SetVideoBuffInfoSize();
	}

	private void SetNormalBalancing()
	{
		m_battleBalancing = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(DIContainerLogic.GetBattleService().GetFirstPossibleBattle(m_hotspotBalancing.BattleId, DIContainerInfrastructure.GetCurrentPlayer()));
		if (m_battleBalancing == null)
		{
			m_battleBalancing = DIContainerBalancing.Service.GetBalancingData<ChronicleCaveBattleBalancingData>(DIContainerLogic.GetBattleService().GetFirstPossibleBattle(m_hotspotBalancing.BattleId, DIContainerInfrastructure.GetCurrentPlayer(), true));
		}
		m_BattleLevel = ((m_HotspotGameData == null) ? DIContainerInfrastructure.GetCurrentPlayer().Data.Level : m_HotspotGameData.GetPigLevelForHotspot(m_hardModeSelected));
		SetVideoBuffInfoSize();
	}

	public void SetEventHotSpot(HotspotGameData spotData, WorldMapStateMgr mapMgr)
	{
		m_worldMapStateMgr = mapMgr;
		m_hotspotBalancing = spotData.BalancingData;
		m_HotspotGameData = spotData;
		Requirement firstFailedReq = null;
		m_Locked = false;
		if (!DIContainerLogic.WorldMapService.IsHotspotEnterable(DIContainerInfrastructure.GetCurrentPlayer(), m_HotspotGameData, out firstFailedReq))
		{
			m_Locked = true;
			FillNotUnlockedInfo(firstFailedReq);
		}
		m_battleBalancing = DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(DIContainerLogic.GetBattleService().GetFirstPossibleBattle(m_hotspotBalancing.BattleId, DIContainerInfrastructure.GetCurrentPlayer(), true));
		m_BattleLevel = ((m_battleBalancing.BaseLevel <= 0) ? DIContainerInfrastructure.GetCurrentPlayer().Data.Level : m_battleBalancing.BaseLevel);
		SetVideoBuffInfoSize();
		m_GoldenPigAddtion = null;
		m_MaxSelectableBirdsCount = m_battleBalancing.MaxBirdsInBattle - m_battleBalancing.UsableFriendBirdsCount;
		m_FriendBirdsCount = m_battleBalancing.UsableFriendBirdsCount;
		m_forcedBird = m_battleBalancing.Force_Character;
		m_BattleRules.SetActive(true);
		foreach (Transform item in m_RulesGrid.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		Invoke("SpawnEventRules", Time.deltaTime);
		m_SelectedFriend = null;
	}

	private void SpawnEventRules()
	{
		if (m_battleBalancing.EnvironmentalEffects != null && m_battleBalancing.EnvironmentalEffects.Count > 0)
		{
			string value = string.Empty;
			if (m_battleBalancing.EnvironmentalEffects.TryGetValue(Faction.Birds, out value))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_RulesPrefabEnvironmentEffect);
				SkillGameData skillGameData = new SkillGameData(value);
				gameObject.transform.Find("Icon").GetComponent<UISprite>().spriteName = skillGameData.m_SkillIconName;
				gameObject.GetComponent<RulesOverlayInvoker>().m_EnvironmentalSkill = skillGameData;
				gameObject.transform.parent = m_RulesGrid.transform;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
			}
			if (m_battleBalancing.EnvironmentalEffects.TryGetValue(Faction.Pigs, out value))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(m_RulesPrefabEnvironmentEffect);
				SkillGameData skillGameData2 = new SkillGameData(value);
				gameObject2.transform.Find("Icon").GetComponent<UISprite>().spriteName = skillGameData2.m_SkillIconName;
				gameObject2.GetComponent<RulesOverlayInvoker>().m_EnvironmentalSkill = skillGameData2;
				gameObject2.transform.parent = m_RulesGrid.transform;
				gameObject2.transform.localScale = Vector3.one;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
			}
		}
		foreach (Requirement battleRequirement in m_battleBalancing.BattleRequirements)
		{
			GameObject gameObject3 = null;
			if (battleRequirement.RequirementType == RequirementType.NotUseBirdInBattle)
			{
				gameObject3 = UnityEngine.Object.Instantiate(m_RulesPrefabBirdForbidden);
			}
			else if (battleRequirement.RequirementType == RequirementType.UseBirdInBattle)
			{
				gameObject3 = UnityEngine.Object.Instantiate(m_RulesPrefabBirdRequired);
			}
			if (gameObject3 != null)
			{
				UISprite component = gameObject3.transform.Find("BirdIcon").GetComponent<UISprite>();
				string assetId = DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>(battleRequirement.NameId).AssetId;
				component.spriteName = assetId;
				component.MakePixelPerfect();
				gameObject3.transform.parent = m_RulesGrid.transform;
				gameObject3.transform.localScale = Vector3.one;
				gameObject3.transform.localPosition = Vector3.zero;
				gameObject3.transform.localRotation = Quaternion.identity;
				gameObject3.GetComponent<RulesOverlayInvoker>().m_RestrictedBirdName = DIContainerInfrastructure.GetLocaService().GetCharacterName(DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>(battleRequirement.NameId).LocaId);
			}
		}
		if (m_battleBalancing.MaxBirdsInBattle != 3)
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate(m_RulesPrefabBirdNumber);
			gameObject4.transform.parent = m_RulesGrid.transform;
			gameObject4.transform.localScale = Vector3.one;
			gameObject4.transform.localPosition = Vector3.zero;
			gameObject4.transform.Find("Value").GetComponent<UILabel>().text = m_battleBalancing.MaxBirdsInBattle.ToString();
			gameObject4.transform.localRotation = Quaternion.identity;
			gameObject4.GetComponent<RulesOverlayInvoker>().m_AllowedBirdsNum = m_battleBalancing.MaxBirdsInBattle;
		}
		m_RulesGrid.Reposition();
	}

	public void SetChronicleCaveHotSpot(HotspotGameData spotData, ChronicleCaveGameData ccave, ChronicleCaveStateMgr mapMgr)
	{
		m_worldMapStateMgr = mapMgr;
		m_hotspotBalancing = spotData.BalancingData;
		m_HotspotGameData = spotData;
		Requirement firstFailedReq = null;
		m_Locked = false;
		if (!DIContainerLogic.WorldMapService.IsHotspotEnterable(DIContainerInfrastructure.GetCurrentPlayer(), m_HotspotGameData, out firstFailedReq))
		{
			m_Locked = true;
			FillNotUnlockedInfo(firstFailedReq);
		}
		m_GoldenPigAddtion = null;
		m_battleBalancing = DIContainerBalancing.Service.GetBalancingData<ChronicleCaveBattleBalancingData>(DIContainerLogic.GetBattleService().GetFirstPossibleBattle(m_hotspotBalancing.BattleId, DIContainerInfrastructure.GetCurrentPlayer(), true));
		m_BattleLevel = ((m_battleBalancing.BaseLevel <= 0) ? DIContainerInfrastructure.GetCurrentPlayer().Data.Level : m_battleBalancing.BaseLevel);
		SetVideoBuffInfoSize();
		m_MaxSelectableBirdsCount = m_battleBalancing.MaxBirdsInBattle - m_battleBalancing.UsableFriendBirdsCount;
		m_FriendBirdsCount = m_battleBalancing.UsableFriendBirdsCount;
		m_forcedBird = m_battleBalancing.Force_Character;
		m_SelectedFriend = null;
	}

	private void FillNotUnlockedInfo(Requirement failed)
	{
		switch (failed.RequirementType)
		{
		case RequirementType.IsSpecificWeekday:
		{
			m_BattleLockedTimerRoot.gameObject.SetActive(false);
			m_BattleLockedInfoRoot.gameObject.SetActive(true);
			string text = DIContainerInfrastructure.GetLocaService().Tr("loca_generic_weekday_" + failed.NameId);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", string.Empty + text);
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.DungeonsAlreadyPlayedToday.Contains(m_hotspotBalancing.NameId))
			{
				m_BattleLockedInfo.text = DIContainerInfrastructure.GetLocaService().Tr("loca_generic_weekday_dungeon_played", dictionary);
			}
			else
			{
				m_BattleLockedInfo.text = DIContainerInfrastructure.GetLocaService().Tr("loca_generic_weekday_dungeon", dictionary);
			}
			break;
		}
		case RequirementType.CooldownFinished:
		{
			m_BattleLockedTimerRoot.gameObject.SetActive(true);
			m_BattleLockedInfoRoot.gameObject.SetActive(false);
			TimeSpan timeSpan = TimeSpan.FromSeconds(failed.Value);
			m_BattleLockedTimer.text = timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
			break;
		}
		case RequirementType.HaveItem:
			m_BattleLockedTimerRoot.gameObject.SetActive(false);
			m_BattleLockedInfoRoot.gameObject.SetActive(true);
			m_BattleLockedInfo.text = "Not have item: " + DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(failed.NameId);
			break;
		case RequirementType.Level:
			m_BattleLockedTimerRoot.gameObject.SetActive(true);
			m_BattleLockedInfoRoot.gameObject.SetActive(false);
			m_BattleLockedInfo.text = "Not have level: " + failed.Value;
			break;
		case RequirementType.None:
			m_BattleLockedTimerRoot.gameObject.SetActive(true);
			m_BattleLockedInfoRoot.gameObject.SetActive(false);
			break;
		case RequirementType.NotHaveItem:
		case RequirementType.NotHaveClass:
			m_BattleLockedTimerRoot.gameObject.SetActive(true);
			m_BattleLockedInfoRoot.gameObject.SetActive(false);
			m_BattleLockedInfo.text = "Have item: " + DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(failed.NameId);
			break;
		case RequirementType.PayItem:
			m_BattleLockedTimerRoot.gameObject.SetActive(false);
			m_BattleLockedInfoRoot.gameObject.SetActive(true);
			m_BattleLockedInfo.text = "Not have item: " + DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(failed.NameId);
			break;
		}
	}

	private void EnterSec()
	{
		Enter();
	}

	public void Enter(bool fromClassMgr = false)
	{
		m_Entered = true;
		if (!fromClassMgr)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter(false);
			DIContainerInfrastructure.GetCoreStateMgr().m_DailyLoginUi.StopEnterCoroutine();
			DIContainerInfrastructure.GetCoreStateMgr().m_MissingEnergyPopup.gameObject.SetActive(false);
			m_worldMapStateMgr.WorldMenuUI.Leave();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveNonInteractableTooltip();
		}
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	public void SelectFriend(FriendGameData friend)
	{
		m_SelectedFriend = friend;
	}

	private void UpdatePigPowerLevel()
	{
		List<ICharacter> list = new List<ICharacter>();
		List<PigBalancingData> strongestPigs = GetStrongestPigs();
		for (int i = 0; i < Mathf.Min(strongestPigs.Count, 3); i++)
		{
			PigBalancingData pigBalancingData = strongestPigs[i];
			PigGameData pigGameData = new PigGameData(pigBalancingData.NameId);
			pigGameData.SetDifficulties(m_HotspotGameData.GetPigLevelForHotspot(m_hardModeSelected), m_battleBalancing);
			list.Add(pigGameData);
		}
		SetPigPowerLevelDisplay(list);
	}

	private void SwitchBoards()
	{
		if (m_HotspotGameData.WorldMapView.m_MiniCampaignHotspot)
		{
			return;
		}
		bool flag = AllowHardMode();
		if (m_hardModeSelected)
		{
			m_dungeonHardModeButtonAnimation.Play("Hide");
			m_dungeonNormalModeButtonAnimation.Play("Show");
			m_hardModeSelected = false;
			SetNormalBalancing();
			UpdatePigPowerLevel();
			if (!flag)
			{
				if (m_Locked)
				{
					m_SkipAnimation.Play("BackButton_Enter");
				}
				if (!m_skipButtonTrigger.gameObject.activeSelf)
				{
					m_StartAnimation.Play("StartBattleButton_Enter");
				}
			}
			m_InactiveHardButton.SetActive(false);
			m_genericBattleInfos.SetActive(true);
			m_Spacer.SetActive(true);
			foreach (BattlePrepCharacterButton button in m_buttonList)
			{
				button.gameObject.SetActive(true);
			}
			m_FriendBirdRoot.SetActive(m_FriendBirdsCount > 0 && m_SelectedFriend != null);
			m_chooseFriendBirdButtonTrigger.gameObject.SetActive(true);
			m_classChangeButton.gameObject.SetActive(true);
			m_SponsoredBuffRootLarge.SetActive(true);
		}
		else
		{
			m_dungeonNormalModeButtonAnimation.Play("Hide");
			m_dungeonHardModeButtonAnimation.Play("Show");
			m_hardModeSelected = true;
			SetHardBalancing();
			if (flag)
			{
				UpdatePigPowerLevel();
			}
			else
			{
				if (!m_skipButtonTrigger.gameObject.activeSelf)
				{
					m_StartAnimation.Play("StartBattleButton_Leave");
				}
				if (m_Locked)
				{
					m_SkipAnimation.Play("BackButton_Leave");
				}
				m_InactiveHardButton.SetActive(true);
				m_genericBattleInfos.SetActive(false);
				m_Spacer.SetActive(false);
				foreach (BattlePrepCharacterButton button2 in m_buttonList)
				{
					button2.gameObject.SetActive(false);
				}
				m_FriendBirdRoot.SetActive(false);
				m_chooseFriendBirdButtonTrigger.gameObject.SetActive(false);
				m_classChangeButton.gameObject.SetActive(false);
				m_SponsoredBuffRootLarge.SetActive(false);
			}
		}
		FillInfos();
		m_WaveRoot.SetActive(!m_hardModeSelected || flag);
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("animate_bprep");
		m_ActionOnLeave = null;
		m_SponsoredBuffRootLarge.SetActive(false);
		m_isEventBattle = m_battleBalancing.BattleRequirements != null && m_battleBalancing.BattleRequirements.Any((Requirement r) => r.RequirementType == RequirementType.PayItem && r.NameId == "event_energy");
		EnterLockedDependentParts();
		SetBirdsAvailable();
		SetAnimationParams();
		SetBattleTypeSprite();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false,
			showEnergy = false
		}, true);
		string battleType = ((!string.IsNullOrEmpty(m_battleBalancing.BattleType)) ? m_battleBalancing.BattleType : "Generic");
		m_battleHintBalancing = DIContainerBalancing.Service.GetBalancingData<BattleHintBalancingData>(battleType);
		m_battleHintHeader.text = DIContainerInfrastructure.GetLocaService().Tr("bps_battletype_" + m_battleHintBalancing.LocaId + "_name");
		m_battleHintDesc.text = DIContainerInfrastructure.GetLocaService().Tr("bps_battletype_" + m_battleHintBalancing.LocaId + "_desc");
		GetComponent<Animator>().SetTrigger("Show");
		foreach (BattlePrepCharacterButton characterButton in m_buttonList)
		{
			characterButton.gameObject.SetActive(true);
		}
		m_chooseFriendBirdButtonTrigger.gameObject.SetActive(m_battleBalancing.UsableFriendBirdsCount > 0);
		m_classChangeButton.gameObject.SetActive(true);
		if (IsDungeon())
		{
			m_hardModeSelected = !m_hardModeSelected;
			yield return new WaitForEndOfFrame();
			SwitchBoards();
			m_DungeonSpecialLootRoot.SetActive(true);
		}
		else
		{
			m_InactiveHardButton.SetActive(false);
			m_genericBattleInfos.SetActive(true);
			m_DungeonSpecialLootRoot.SetActive(false);
			m_Spacer.SetActive(true);
			FillInfos();
		}
		if ((bool)DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr)
		{
			DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr.ToggleBossIdleAnimation(false);
		}
		UpdateBirdPowerLevel();
		EventManagerGameData emgd = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
		if (m_isEventBattle && emgd.IsBossEvent)
		{
			StartCoroutine(CreatePreviewBoss());
		}
		else
		{
			StartCoroutine(CreatePreviewPigs());
		}
		m_classChangeButtonAnimation.Play("BackButton_Enter");
		m_BackAnimation.Play("BackButton_Enter");
		yield return new WaitForSeconds(0.3f);
		EnterEnergyBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		DIContainerInfrastructure.GetCoreStateMgr().m_BonusCodeManager.Leave();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("battle_preparation", m_hotspotBalancing.NameId);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		RegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("animate_bprep");
		if (IsSponsoredAdPossible() && !m_SponsoredBuffRootLarge.activeSelf && (!IsDungeon() || AllowHardMode()))
		{
			m_SponsoredBuffRootLarge.SetActive(true);
			EnterSponsoredBuff();
		}
	}

	private void SetBattleTypeSprite()
	{
		if (m_battleBalancing is ChronicleCaveBattleBalancingData)
		{
			m_battleTypeSprite.spriteName = "Battletyp_ChronicleCave";
		}
		else if (m_isEventBattle)
		{
			EventManagerGameData currentEventManagerGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData;
			if (currentEventManagerGameData.IsBossEvent)
			{
				m_battleTypeSprite.spriteName = "Battletyp_Event_Boss";
			}
			else if (currentEventManagerGameData.IsCampaignEvent)
			{
				m_battleTypeSprite.spriteName = "Battletyp_Event_Campaign";
			}
			else
			{
				m_battleTypeSprite.spriteName = "Battletyp_Event_Invasion";
			}
		}
		else if (m_hotspotBalancing.NameId.Contains("castle"))
		{
			m_battleTypeSprite.spriteName = "Battletyp_Castle";
		}
		else
		{
			m_battleTypeSprite.spriteName = "Battletyp_Worldmap";
		}
	}

	private void SetBirdsAvailable()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		Animator component = GetComponent<Animator>();
		component.SetBool("HaveRedBird", currentPlayer.Birds.Any((BirdGameData b) => b.Name == "bird_red"));
		component.SetBool("HaveYellowBird", currentPlayer.Birds.Any((BirdGameData b) => b.Name == "bird_yellow"));
		component.SetBool("HaveWhiteBird", currentPlayer.Birds.Any((BirdGameData b) => b.Name == "bird_white"));
		component.SetBool("HaveBlackBird", currentPlayer.Birds.Any((BirdGameData b) => b.Name == "bird_black"));
		component.SetBool("HaveBlueBird", currentPlayer.Birds.Any((BirdGameData b) => b.Name == "bird_blue"));
	}

	private void SetAnimationParams()
	{
		Animator component = GetComponent<Animator>();
		bool value = m_hotspotBalancing.NameId.Contains("dungeon") && !(m_hotspotBalancing is ChronicleCaveHotspotBalancingData);
		if (m_battleBalancing.BattleRequirements != null)
		{
			component.SetBool("IsEventBattle", m_isEventBattle);
		}
		else
		{
			component.SetBool("IsEventBattle", false);
		}
		component.SetBool("IsDungeon", value);
		component.SetBool("AccumulaticeReward", value);
	}

	private bool IsSponsoredAdPossible()
	{
		return false;
		return !m_Locked && (DIContainerInfrastructure.AdService.IsAdShowPossible(BUFF_PLACEMENT) || !string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentSponsoredBuff));
	}

	private void EnterSponsoredBuff()
	{
		#if USE_BEACON
		if (!m_EnergyFooterActive)
		{
			m_SponsoredBuffRootLarge.SetActive(true);
			m_SponsoredBuffRootLarge.GetComponent<Animation>().Play("Footer_Enter");
			if (m_SponsoredAdSkill != null)
			{
				m_SponsoredTextLarge.text = m_SponsoredAdSkill.GetLocalizedDescription(null);
			}
			if (string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentSponsoredBuff))
			{
				m_SponsoredBuffButtonRootLarge.SetActive(true);
				m_SponsoredBuffCheckRootLarge.SetActive(false);
			}
			else
			{
				m_SponsoredBuffButtonRootLarge.SetActive(false);
				m_SponsoredBuffCheckRootLarge.SetActive(true);
			}
		}
		#endif
	}

	private void EnterEnergyBar()
	{
		bool showLuckyCoins = IsDungeon() && m_Locked;
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u,
			showFriendshipEssence = false,
			showLuckyCoins = showLuckyCoins,
			showSnoutlings = false,
			showEnergy = m_hotspotBalancing.IsSpawnEventPossible
		}, true);
		SetEnergyTimer();
	}

	private void SetEnergyTimer()
	{
		if (m_hotspotBalancing.IsSpawnEventPossible)
		{
			if (!DIContainerLogic.EventSystemService.HasMaximumEnergy(DIContainerInfrastructure.GetCurrentPlayer()))
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnableEnergyTimer(true);
				StopCoroutine("CountDownTimer");
				StartCoroutine("CountDownTimer");
			}
			else
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnableEnergyTimer(false);
			}
		}
	}

	private IEnumerator CountDownTimer()
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		DateTime targetTime = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerInfrastructure.GetCurrentPlayer().Data.LastEnergyAddTime).AddSeconds(DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.BalancingData.EnergyRefreshTimeInSeconds);
		while (targetTime > trustedTime)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = targetTime - trustedTime;
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.SetEnergyTimer(DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft));
			}
			yield return new WaitForSeconds(1f);
		}
		EnergyUpdateMgr energyUpdateManager = DIContainerInfrastructure.GetCoreStateMgr().GetComponent<EnergyUpdateMgr>();
		if ((bool)energyUpdateManager)
		{
			energyUpdateManager.Run();
		}
		SetEnergyTimer();
	}

	private void LeaveCoinBars()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
	}

	public float EnterLockedDependentParts()
	{
		if (m_FriendBirdsCount == 0)
		{
			m_FriendBirdRoot.SetActive(false);
		}
		bool flag = IsDungeon();
		if (!m_Locked)
		{
			SetVideoBuffInfoSize();
			m_startButtonTrigger.gameObject.SetActive(true);
			m_CharacterRoot.gameObject.SetActive(true);
			m_skipButtonTrigger.gameObject.SetActive(false);
			bool flag2 = !string.IsNullOrEmpty(m_forcedBird);
			if (m_FriendBirdsCount > 0 && m_SelectedFriend == null)
			{
				foreach (BattlePrepCharacterButton button in m_buttonList)
				{
					button.Selectable(true);
				}
				m_AddFriendBirdButtonAnimation.Play("FriendBirdButton_Enter");
				m_FriendBirdRoot.SetActive(false);
			}
			else if (m_FriendBirdsCount > 0)
			{
				m_AddFriendBirdButtonAnimation.Play("FriendBirdButton_Leave");
				m_FriendBirdRoot.SetActive(true);
				m_FriendBirdRoot.GetComponent<FriendInfoElement>().SetModel(m_SelectedFriend);
				if (m_FriendBirdParent.childCount > 0)
				{
					m_FriendBirdParent.GetChild(0).GetComponent<BattlePrepCharacterButton>().Selected -= OnChooseFriendBirdButtonTriggerClicked;
					UnityEngine.Object.Destroy(m_FriendBirdParent.GetChild(0).gameObject);
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(m_CharacterButtonPrefab);
				gameObject.transform.parent = m_FriendBirdParent;
				BattlePrepCharacterButton component = gameObject.GetComponent<BattlePrepCharacterButton>();
				component.Init(m_SelectedFriend.FriendBird, this);
				gameObject.transform.localPosition = Vector3.zero;
				component.Select(true, true);
				component.Selectable(true);
				component.Selected -= OnChooseFriendBirdButtonTriggerClicked;
				component.Selected += OnChooseFriendBirdButtonTriggerClicked;
				UpdateBirdPowerLevel();
				foreach (BattlePrepCharacterButton button2 in m_buttonList)
				{
					if (button2.GetBirdGameData().Name == m_SelectedFriend.FriendBird.Name)
					{
						button2.Select(false);
						button2.Selectable(false);
						button2.GetComponent<Animation>().Play("CharacterSlot_Inactive");
						continue;
					}
					if (!button2.IsSelectable() || m_FriendBirdsCount == 0)
					{
						button2.GetComponent<Animation>().Play("CharacterSlot_Deselected");
					}
					button2.Selectable(true);
				}
			}
			else if (m_FriendBirdsCount == 0)
			{
				foreach (BattlePrepCharacterButton button3 in m_buttonList)
				{
					if (!button3.IsSelected())
					{
						button3.GetComponent<Animation>().Play("CharacterSlot_Deselected");
						button3.Selectable(true);
					}
				}
			}
			if (m_HotspotGameData == null || m_HotspotGameData.WorldMapView == null)
			{
				m_EventEnergyCost.gameObject.SetActive(false);
				m_EventEnergyRoot.SetActive(false);
			}
			else if (!m_HotspotGameData.WorldMapView.m_MiniCampaignHotspot)
			{
				if (IsSponsoredAdPossible())
				{
					m_SponsoredBuffRootLarge.gameObject.SetActive(true);
					m_SponsoredBuffRootLarge.gameObject.PlayAnimationOrAnimatorState("Footer_Enter");
					m_SponsoredTextLarge.text = m_SponsoredAdSkill.GetLocalizedDescription(null);
				}
				m_EventEnergyCost.gameObject.SetActive(false);
				m_EventEnergyRoot.SetActive(false);
			}
			else if (m_HotspotGameData.WorldMapView.m_MiniCampaignHotspot)
			{
				m_EventEnergyCost.gameObject.SetActive(false);
				m_EventEnergyRoot.SetActive(false);
			}
			else if (flag2)
			{
				m_MaxSelectableBirdsCount--;
				m_SelectedFriend = GetFriendForBird(m_forcedBird);
			}
			else
			{
				m_EventEnergyCost.gameObject.SetActive(false);
				m_EventEnergyRoot.SetActive(false);
			}
			if (m_hotspotBalancing.IsSpawnEventPossible)
			{
				StartCoroutine(HandleEnterEventBattle());
			}
			m_BattleLockedRoot.SetActive(false);
			if (!m_skipButtonTrigger.gameObject.activeSelf && ((flag && AllowHardMode() && m_hardModeSelected) || (flag && !m_hardModeSelected) || !flag))
			{
				m_StartAnimation.Play("StartBattleButton_Enter");
			}
			return 1f;
		}
		if ((flag && AllowHardMode() && m_hardModeSelected) || (flag && !m_hardModeSelected) || !flag)
		{
			m_SkipAnimation.Play("BackButton_Enter");
		}
		Requirement dungeonSkipRequirement = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").DungeonSkipRequirement;
		IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(dungeonSkipRequirement.NameId);
		m_skipCost.SetModel(balancingData.AssetBaseId, null, dungeonSkipRequirement.Value, string.Empty);
		if (m_HotspotGameData.WorldMapView.m_MiniCampaignHotspot)
		{
			m_skipButtonTrigger.gameObject.SetActive(false);
		}
		else
		{
			m_skipButtonTrigger.gameObject.SetActive(true);
		}
		m_CharacterRoot.gameObject.SetActive(false);
		m_BattleLockedRoot.SetActive(true);
		m_CharacterRoot.gameObject.SetActive(false);
		m_BattleLockedAnimation.Play("Footer_Enter");
		return m_BattleLockedAnimation["Footer_Enter"].length;
	}

	private FriendGameData GetFriendForBird(string forcedBird)
	{
		int level = DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
		FriendData friendData = new FriendData();
		friendData.IsNPC = true;
		friendData.FirstName = "npc_" + forcedBird;
		friendData.Id = forcedBird;
		friendData.IsSilhouettePicture = false;
		friendData.Level = Math.Max(level, 1);
		friendData.PictureUrl = string.Empty;
		FriendData friendData2 = friendData;
		FriendGameData friendGameData = new FriendGameData(friendData2.Id);
		friendGameData.SetFriendData(friendData2);
		friendGameData.isNpcFriend = true;
		if (friendGameData.PublicPlayerData != null)
		{
			friendGameData.PublicPlayerData.Level = friendData2.Level;
		}
		BirdGameData birdGameData = new BirdGameData(forcedBird, level);
		birdGameData.IsNPC = true;
		BirdGameData birdGameData2 = birdGameData;
		birdGameData2.ClassItem.Data.Level = Mathf.Max(1, GetAverageMastery() - 1);
		friendGameData.SetFriendBird(birdGameData2);
		return friendGameData;
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

	public void RefreshEnergyValues()
	{
		StartCoroutine(HandleEnterEventBattle(false));
	}

	private IEnumerator HandleEnterEventBattle(bool animate = true)
	{
		m_EventEnergyCost.gameObject.SetActive(true);
		Requirement energyRequirement = m_battleBalancing.BattleRequirements.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem && r.NameId.Contains("event_energy"));
		int ownEnergy = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "event_energy");
		int ownPotions = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "potion_energy");
		if (energyRequirement == null)
		{
			DebugLog.Error("no energy needed for event battle, this shouldn't happen");
			yield break;
		}
		if (energyRequirement.Value <= (float)ownEnergy)
		{
			m_EnergyFooterActive = false;
			m_EventEnergyRoot.SetActive(false);
			if (m_EventEnergyRoot.activeInHierarchy && !animate)
			{
				m_EventEnergyRoot.GetComponent<Animation>().Play("Footer_Leave");
			}
			if (!string.IsNullOrEmpty(m_forcedBird) || m_FriendBirdsCount > 0)
			{
			}
			EnterSponsoredBuff();
		}
		else if (ownPotions > 0)
		{
			m_EnergyFooterActive = true;
			m_EventEnergyRoot.SetActive(true);
			if (animate)
			{
				m_EventEnergyRoot.GetComponent<Animation>().Play("Footer_Enter");
			}
			m_energyPotionAmount.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(ownPotions);
			m_EventEnergyDescription.text = DIContainerInfrastructure.GetLocaService().Tr("bps_energypotion_desc");
		}
		else
		{
			m_EnergyFooterActive = true;
			m_EventEnergyRoot.SetActive(true);
			if (animate)
			{
				m_EventEnergyRoot.GetComponent<Animation>().Play("Footer_Enter");
			}
			m_energyPotionAmount.text = DIContainerInfrastructure.GetLocaService().Tr("special_offer_shop", "Shop");
			m_EventEnergyDescription.text = DIContainerInfrastructure.GetLocaService().Tr("bps_eventbattlefooter");
		}
		if (!string.IsNullOrEmpty(energyRequirement.NameId))
		{
			m_EventEnergyCost.gameObject.SetActive(true);
			m_EventEnergyCost.SetModel(string.Empty, null, "-" + energyRequirement.Value, string.Empty);
		}
		else
		{
			m_EventEnergyCost.gameObject.SetActive(false);
		}
		if (animate)
		{
			yield return new WaitForSeconds(0.5f);
		}
		if (animate)
		{
			DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("battle_rules", string.Empty);
		}
	}

	private float LeaveLockedDependentParts()
	{
		if (!m_Locked)
		{
			if (m_FriendBirdsCount <= 0 && string.IsNullOrEmpty(m_forcedBird) && m_hotspotBalancing.IsSpawnEventPossible)
			{
				HandleLeaveEventBattle();
			}
			if (!m_skipButtonTrigger.gameObject.activeSelf && m_StartAnimation.transform.GetChild(0).localPosition.y == 0f)
			{
				m_StartAnimation.Play("StartBattleButton_Leave");
			}
			return 1f;
		}
		bool flag = IsDungeon();
		if ((flag && AllowHardMode() && m_hardModeSelected) || (flag && !m_hardModeSelected) || !flag)
		{
			m_SkipAnimation.Play("BackButton_Leave");
		}
		m_BattleLockedAnimation.Play("Footer_Leave");
		return m_BattleLockedAnimation["Footer_Leave"].length;
	}

	private void HandleLeaveEventBattle()
	{
		m_EventEnergyRoot.GetComponent<Animation>().Play("Footer_Leave");
	}

	public void Leave(bool forClassMgr = false)
	{
		StartCoroutine(LeaveCoroutine(forClassMgr));
	}

	private IEnumerator LeaveCoroutine(bool forClassMgr = false)
	{
		DeregisterEventHandler();
		if (!forClassMgr)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		}
		if (m_SponsoredBuffRootLarge.activeInHierarchy)
		{
			m_SponsoredBuffRootLarge.GetComponent<Animation>().Play("Footer_Leave");
		}
		GetComponent<Animator>().SetTrigger("Hide");
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("animate_bprep");
		m_BackAnimation.Play("BackButton_Leave");
		if (m_FriendBirdsCount > 0 && m_SelectedFriend == null)
		{
			m_AddFriendBirdButtonAnimation.Play("FriendBirdButton_Leave");
		}
		m_classChangeButtonAnimation.Play("BackButton_Leave");
		if (!forClassMgr)
		{
			LeaveCoinBars();
		}
		LeaveLockedDependentParts();
		yield return new WaitForSeconds(DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.GetLeaveLength());
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("animate_bprep");
		m_BattleLockedRoot.SetActive(false);
		m_startButtonTrigger.gameObject.SetActive(false);
		m_SponsoredBuffRootLarge.SetActive(false);
		if ((bool)DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr && !forClassMgr)
		{
			DIContainerInfrastructure.LocationStateMgr.EventsWorldMapStateMgr.ToggleBossIdleAnimation();
		}
		if (m_ActionOnLeave != null)
		{
			m_ActionOnLeave();
		}
		else if (!m_EnteringBattle && !forClassMgr)
		{
			m_worldMapStateMgr.WorldMenuUI.Enter();
			m_SelectedFriend = null;
		}
		m_Entered = false;
		base.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		if ((bool)DIContainerInfrastructure.BackButtonMgr)
		{
			DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(1);
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("animate_bprep");
		}
	}

	private void FillInfos()
	{
		m_CharacterRoot.gameObject.SetActive(!m_Locked);
		m_birdSelectionFirst = true;
		m_SelectedBirds = new List<int>();
		List<int> list = Enumerable.ToList(DIContainerInfrastructure.GetCurrentPlayer().SelectedBirdIndices.Where((int i) => m_SelectedFriend == null || DIContainerInfrastructure.GetCurrentPlayer().Birds.Count >= i || DIContainerInfrastructure.GetCurrentPlayer().Birds[i] == null || DIContainerInfrastructure.GetCurrentPlayer().Birds[i].BalancingData.NameId != m_SelectedFriend.FriendBird.BalancingData.NameId));
		for (int j = 0; j < m_MaxSelectableBirdsCount && j < list.Count; j++)
		{
			m_SelectedBirds.Add(list[j]);
		}
		DebugLog.Log("Selected Birds Count: " + m_SelectedBirds.Count);
		for (int k = 0; k < m_SelectedBirds.Count; k++)
		{
			DebugLog.Log(k + " Selected Bird is " + m_SelectedBirds[k]);
		}
		if (m_SelectedBirds.Count < m_MaxSelectableBirdsCount)
		{
			int num = m_MaxSelectableBirdsCount - DIContainerInfrastructure.GetCurrentPlayer().SelectedBirdIndices.Count;
			for (int l = 0; l < DIContainerInfrastructure.GetCurrentPlayer().Birds.Count; l++)
			{
				if (num <= 0)
				{
					break;
				}
				if (!m_SelectedBirds.Contains(l))
				{
					m_SelectedBirds.Insert(l, l);
					num--;
				}
			}
		}
		DebugLog.Log("Bird count in profile " + DIContainerInfrastructure.GetCurrentPlayer().Birds.Count);
		for (int m = 0; m < m_buttonList.Count; m++)
		{
			BattlePrepCharacterButton battlePrepCharacterButton = m_buttonList[m];
			battlePrepCharacterButton.Selected -= OnBirdSelected;
			battlePrepCharacterButton.Selected += OnBirdSelected;
			battlePrepCharacterButton.PlayCharacterIdle();
			if (m_SelectedBirds.Contains(m))
			{
				DebugLog.Log("Selected Bird: " + m);
				battlePrepCharacterButton.Select(true);
			}
			else
			{
				battlePrepCharacterButton.Select(false);
			}
		}
		m_birdSelectionFirst = false;
		DIContainerInfrastructure.GetCurrentPlayer().SelectedBirdIndices = m_SelectedBirds;
		m_OneBirdLeft = m_SelectedBirds.Count <= 1;
		m_StageNameText.text = m_HotspotGameData.StageName;
		if (m_battleBalancing.BattleParticipantsIds.Count > 1)
		{
			GetComponent<Animator>().SetBool("IsWaveBattle", true);
			m_WaveCountText.text = m_battleBalancing.BattleParticipantsIds.Count.ToString();
			m_WaveRoot.SetActive(true);
		}
		else
		{
			GetComponent<Animator>().SetBool("IsWaveBattle", false);
			m_WaveRoot.SetActive(false);
		}
		int num2 = Mathf.Min(m_buttonList.Count, m_MaxSelectableBirdsCount);
		m_BirdCountText.text = GetCurrentBirdCount() + "/" + num2;
		SetTopLoot();
		UpdateSelectedBirds();
	}

	private void SetTopLoot()
	{
		DIContainerLogic.EventSystemService.UpdateCachedFallbackLoot(m_hotspotBalancing);
		LootTableBalancingData balancing = null;
		int num = 0;
		int level = DIContainerInfrastructure.GetCurrentPlayer().Data.Level;
		ExperienceLevelBalancingData balancing2;
		if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + level.ToString("00"), out balancing2) || DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + (level - 1).ToString("00"), out balancing2))
		{
			num = balancing2.MatchmakingRangeIndex;
		}
		string nameId = m_battleBalancing.LootTableWheel.FirstOrDefault().Key.Replace("{levelrange}", num.ToString("00"));
		DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(nameId, out balancing);
		LootTableBalancingData balancing3 = null;
		if (DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(balancing.LootTableEntries[0].NameId, out balancing3))
		{
			m_MajorLoot.SetModel(null, new List<IInventoryItemGameData>(), LootDisplayType.Major);
			return;
		}
		int baseValue = balancing.LootTableEntries[0].BaseValue;
		BonusEventBalancingData currentValidBalancing = DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing;
		if (m_HotspotGameData.IsDungeon())
		{
			baseValue += balancing.LootTableEntries[1].BaseValue;
			baseValue += balancing.LootTableEntries[2].BaseValue;
			if (currentValidBalancing != null && currentValidBalancing.BonusType == BonusEventType.DungeonBonus)
			{
				float num2 = currentValidBalancing.BonusFactor / 100f;
				baseValue += (int)((float)baseValue * num2);
			}
			m_TopLootDungeonText.text = baseValue.ToString();
		}
		else
		{
			IInventoryItemGameData mainItem = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, (int)m_BattleLevel, 1, balancing.LootTableEntries[0].NameId, baseValue, EquipmentSource.LootBird);
			m_MajorLoot.SetModel(mainItem, new List<IInventoryItemGameData>(), LootDisplayType.Major);
		}
	}

	private void ClearOldBirdButtons()
	{
		foreach (BattlePrepCharacterButton button in m_buttonList)
		{
			button.Selected -= OnBirdSelected;
			UnityEngine.Object.Destroy(button.gameObject);
		}
		if (m_FriendBirdParent.childCount > 0)
		{
			m_FriendBirdParent.GetChild(0).GetComponent<BattlePrepCharacterButton>().Selected -= OnChooseFriendBirdButtonTriggerClicked;
			UnityEngine.Object.Destroy(m_FriendBirdParent.GetChild(0).gameObject);
		}
		m_buttonList.Clear();
	}

	private void OnBirdSelected()
	{
		int num = Mathf.Min(m_buttonList.Count, m_MaxSelectableBirdsCount);
		m_BirdCountText.text = GetCurrentBirdCount() + "/" + num;
		if (!m_birdSelectionFirst)
		{
			UpdateSelectedBirds();
		}
		UpdateBirdPowerLevel();
	}

	private void UpdateBirdPowerLevel()
	{
		m_ownPowerLevel = DIContainerInfrastructure.GetPowerLevelCalculator().GetTeamPowerLevel(DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer, DIContainerInfrastructure.GetCurrentPlayer().SelectedBirdIndices);
		if (m_SelectedFriend != null)
		{
			m_ownPowerLevel += DIContainerInfrastructure.GetPowerLevelCalculator().GetBirdPowerLevel(m_SelectedFriend.FriendBird);
		}
		if (!string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentSponsoredBuff))
		{
			m_ownPowerLevel += (int)((float)m_ownPowerLevel * (DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>("env_sponsored_health_and_attack").SkillParameters.FirstOrDefault().Value / 100f));
		}
		m_birdPowerLevel.color = ((!string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentSponsoredBuff)) ? m_PowerLevelColorEasy : m_PowerLevelColorDefault);
		m_birdPowerLevel.text = m_ownPowerLevel.ToString();
		UpdatePigPowerLevelColor();
	}

	private void UpdateSelectedBirds()
	{
		m_SelectedBirds = new List<int>();
		for (int i = 0; i < m_buttonList.Count; i++)
		{
			BattlePrepCharacterButton battlePrepCharacterButton = m_buttonList[i];
			if (battlePrepCharacterButton.IsSelected())
			{
				m_SelectedBirds.Add(i);
			}
		}
		DIContainerInfrastructure.GetCurrentPlayer().SelectedBirdIndices = m_SelectedBirds;
		m_BirdCountText.color = ((m_SelectedBirds.Count == Mathf.Min(DIContainerInfrastructure.GetCurrentPlayer().Birds.Count, m_MaxSelectableBirdsCount)) ? DIContainerLogic.GetVisualEffectsBalancing().ColorOffersBuyable : DIContainerLogic.GetVisualEffectsBalancing().ColorOffersNotBuyable);
		m_OneBirdLeft = m_SelectedBirds.Count <= ((DIContainerInfrastructure.GetCurrentPlayer().AllBirds.Count >= 4) ? 1 : Mathf.Min(DIContainerInfrastructure.GetCurrentPlayer().Birds.Count, m_MaxSelectableBirdsCount));
	}

	public void CreateBirds()
	{
		ClearOldBirdButtons();
		if (DIContainerInfrastructure.GetCurrentPlayer().Birds != null)
		{
			m_birdSelectionFirst = true;
			for (int i = 0; i < DIContainerInfrastructure.GetCurrentPlayer().Birds.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_CharacterButtonPrefab);
				gameObject.transform.parent = m_CharacterRoot;
				BattlePrepCharacterButton component = gameObject.GetComponent<BattlePrepCharacterButton>();
				component.Selected -= OnBirdSelected;
				component.Selected += OnBirdSelected;
				component.Init(DIContainerInfrastructure.GetCurrentPlayer().Birds[i], this);
				m_buttonList.Add(component);
			}
			m_birdSelectionFirst = false;
		}
		if (m_BirdGrid != null)
		{
			m_BirdGrid.repositionNow = true;
		}
	}

	private int GetCurrentBirdCount()
	{
		int num = 0;
		foreach (BattlePrepCharacterButton button in m_buttonList)
		{
			if (button.IsSelected())
			{
				num++;
			}
		}
		return num;
	}

	private void CheckIfLocked()
	{
		Requirement firstFailedReq = null;
		if (base.gameObject.activeInHierarchy && m_HotspotGameData != null && !DIContainerLogic.WorldMapService.IsHotspotEnterable(DIContainerInfrastructure.GetCurrentPlayer(), m_HotspotGameData, out firstFailedReq) && !DIContainerInfrastructure.GetCurrentPlayer().Data.TemporaryOpenHotspots.Contains(m_hotspotBalancing.NameId))
		{
			if (!m_Locked)
			{
				StartCoroutine(SwitchLockState());
			}
			if (m_hotspotBalancing.EnterRequirements != null)
			{
				foreach (Requirement enterRequirement in m_hotspotBalancing.EnterRequirements)
				{
					if (enterRequirement.RequirementType == RequirementType.IsSpecificWeekday && (DIContainerLogic.GetTimingService().GetPresentTime().DayOfWeek != (DayOfWeek)(int)Enum.Parse(typeof(DayOfWeek), enterRequirement.NameId, true) || DIContainerInfrastructure.GetCurrentPlayer().Data.DungeonsAlreadyPlayedToday.Contains(m_hotspotBalancing.NameId)))
					{
						firstFailedReq = enterRequirement;
					}
				}
			}
			FillNotUnlockedInfo(firstFailedReq);
		}
		else if (base.gameObject.activeInHierarchy && m_HotspotGameData != null && m_Locked)
		{
			StartCoroutine(SwitchLockState());
		}
	}

	private IEnumerator SwitchLockState()
	{
		float duration = LeaveLockedDependentParts();
		m_Locked = !m_Locked;
		yield return new WaitForSeconds(duration);
		float lockedPart = EnterLockedDependentParts();
		yield return new WaitForSeconds(lockedPart);
	}

	private bool IsDungeon()
	{
		return m_hotspotBalancing.NameId.Contains("dungeon") && !(m_hotspotBalancing is ChronicleCaveHotspotBalancingData) && !m_HotspotGameData.WorldMapView.m_MiniCampaignHotspot;
	}

	private bool AllowHardMode()
	{
		foreach (HotspotGameData value in DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.Values)
		{
			if (value.BalancingData.NameId.Contains("dungeon") && !value.IsCompleted())
			{
				return false;
			}
		}
		return true;
	}

	private void SetVideoBuffInfoSize()
	{
		m_SponsoredBuffButtonLarge.Clicked -= OnSponsoredBuffButtonClicked;
		m_SponsoredBuffButtonLarge.Clicked += OnSponsoredBuffButtonClicked;
		m_chooseFriendBirdButtonTrigger.Clicked -= OnChooseFriendBirdButtonTriggerClicked;
		m_chooseFriendBirdButtonTrigger.Clicked += OnChooseFriendBirdButtonTriggerClicked;
	}

	private List<PigBalancingData> GetStrongestPigs()
	{
		List<PigBalancingData> list = new List<PigBalancingData>();
		bool flag = m_battleBalancing is ChronicleCaveBattleBalancingData;
		foreach (string battleParticipantsId in m_battleBalancing.BattleParticipantsIds)
		{
			List<BattleParticipantTableEntry> list2 = ((!flag) ? DIContainerBalancing.Service.GetBalancingData<BattleParticipantTableBalancingData>(battleParticipantsId).BattleParticipants : DIContainerBalancing.Service.GetBalancingData<ChronicleCaveBattleParticipantTableBalancingData>(battleParticipantsId).BattleParticipants);
			foreach (BattleParticipantTableEntry item in list2)
			{
				list.Add(DIContainerBalancing.Service.GetBalancingData<PigBalancingData>(item.NameId));
			}
		}
		return Enumerable.ToList(list.OrderByDescending((PigBalancingData p) => p.PigStrength));
	}

	private IEnumerator CreatePreviewBoss()
	{
		foreach (Transform trans in m_pigPreviewRoots)
		{
			if (trans.childCount > 0)
			{
				UnityEngine.Object.Destroy(trans.GetChild(0).gameObject);
			}
		}
		yield return new WaitForEndOfFrame();
		m_previewPigNames = new List<string>();
		string bossName = DIContainerBalancing.Service.GetBalancingData<BattleParticipantTableBalancingData>(m_battleBalancing.BattleParticipantsIds.FirstOrDefault()).BattleParticipants.FirstOrDefault((BattleParticipantTableEntry p) => p.NameId.Contains("boss")).NameId;
		SetPigPowerLevelDisplay(new List<ICharacter> { SpawnPigController(bossName, 0, true).GetModel() });
		m_pigPreviewRoots[1].GetComponent<BoxCollider>().enabled = false;
		m_pigPreviewRoots[2].GetComponent<BoxCollider>().enabled = false;
	}

	private IEnumerator CreatePreviewPigs()
	{
		foreach (Transform trans in m_pigPreviewRoots)
		{
			if (trans.childCount > 0)
			{
				UnityEngine.Object.Destroy(trans.GetChild(0).gameObject);
			}
		}
		yield return new WaitForEndOfFrame();
		List<PigBalancingData> battlePigs = GetStrongestPigs();
		PigBalancingData strongestPig = battlePigs[0];
		PigBalancingData secondStrongestPig = null;
		PigBalancingData thirdStrongestPig = null;
		int amountOfPreviewPigs = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").MaxPreviewPigsInBps;
		if (strongestPig != null && amountOfPreviewPigs >= 2)
		{
			secondStrongestPig = battlePigs.FirstOrDefault((PigBalancingData p) => p.NameId != strongestPig.NameId);
		}
		if (secondStrongestPig != null && amountOfPreviewPigs >= 3)
		{
			thirdStrongestPig = battlePigs.FirstOrDefault((PigBalancingData p) => p.NameId != secondStrongestPig.NameId && p.NameId != strongestPig.NameId);
		}
		CharacterControllerWorldMap[] pigControllers = new CharacterControllerWorldMap[amountOfPreviewPigs];
		m_previewPigNames = new List<string>();
		pigControllers[0] = SpawnPigController(battlePigs[0].NameId, 0, false);
		if (secondStrongestPig != null && !m_isEventBattle)
		{
			pigControllers[1] = SpawnPigController(secondStrongestPig.NameId, 1, false);
			m_pigPreviewRoots[1].GetComponent<BoxCollider>().enabled = true;
		}
		else
		{
			m_pigPreviewRoots[1].GetComponent<BoxCollider>().enabled = false;
		}
		if (thirdStrongestPig != null && !m_isEventBattle)
		{
			pigControllers[2] = SpawnPigController(thirdStrongestPig.NameId, 2, false);
			m_pigPreviewRoots[2].GetComponent<BoxCollider>().enabled = true;
		}
		else
		{
			m_pigPreviewRoots[2].GetComponent<BoxCollider>().enabled = false;
		}
		List<ICharacter> pigModels = new List<ICharacter>();
		CharacterControllerWorldMap[] array = pigControllers;
		foreach (CharacterControllerWorldMap pig in array)
		{
			if (pig != null)
			{
				pigModels.Add(pig.GetModel());
			}
		}
		SetPigPowerLevelDisplay(pigModels);
	}

	private void SetPigPowerLevelDisplay(List<ICharacter> pigControllers)
	{
		m_currentPigStrength = 0f;
		for (int i = 0; i < pigControllers.Count; i++)
		{
			if (pigControllers[i] != null)
			{
				ICharacter character = pigControllers[i];
				if (character is PigGameData)
				{
					(character as PigGameData).SetDifficulties(m_HotspotGameData.GetPigLevelForHotspot(m_hardModeSelected), m_battleBalancing);
				}
				else if (character is BossGameData)
				{
					(character as BossGameData).SetDifficulties(m_HotspotGameData.GetPigLevelForHotspot(m_hardModeSelected), m_battleBalancing);
				}
				int pigPowerLevel = DIContainerInfrastructure.GetPowerLevelCalculator().GetPigPowerLevel(character, m_battleBalancing);
				m_currentPigStrength += pigPowerLevel;
			}
		}
		UpdatePigPowerLevelColor();
	}

	private void UpdatePigPowerLevelColor()
	{
		float num = (m_currentPigStrength - (float)m_ownPowerLevel) / (float)m_ownPowerLevel;
		if (num < 0f)
		{
			m_pigPowerLevel.color = m_PowerLevelColorEasy;
		}
		else if (num == 0f)
		{
			m_pigPowerLevel.color = m_PowerLevelColorDefault;
		}
		else if (num < 0.15f)
		{
			m_pigPowerLevel.color = m_PowerLevelColorNormal;
		}
		else
		{
			m_pigPowerLevel.color = m_PowerLevelColorHard;
		}
		m_pigPowerLevel.text = m_currentPigStrength.ToString();
	}

	private CharacterControllerWorldMap SpawnPigController(string pigName, int iD, bool isBoss)
	{
		if (iD < 0 || iD > 2)
		{
			Debug.LogError("Wrong ID, cannot sort pig! Use id between 0 and 2");
			return null;
		}
		m_previewPigNames.Insert(iD, pigName);
		CharacterControllerWorldMap characterControllerWorldMap = UnityEngine.Object.Instantiate(m_pigPrefab, Vector3.zero, Quaternion.identity) as CharacterControllerWorldMap;
		characterControllerWorldMap.transform.parent = m_pigPreviewRoots[iD];
		characterControllerWorldMap.SetModel(pigName, false, false, m_HotspotGameData.GetPigLevelForHotspot(m_hardModeSelected));
		characterControllerWorldMap.transform.localRotation = Quaternion.identity;
		characterControllerWorldMap.transform.localPosition = Vector3.zero;
		characterControllerWorldMap.transform.localScale = ((!isBoss) ? new Vector3(0.4f, 0.4f, 1f) : new Vector3(0.2f, 0.2f, 1f));
		UnityHelper.SetLayerRecusively(characterControllerWorldMap.gameObject, LayerMask.NameToLayer("Interface"));
		return characterControllerWorldMap;
	}

	private void ShowTooltip(int pigIndex)
	{
		ICharacter model = m_pigPreviewRoots[pigIndex].GetComponentInChildren<CharacterControllerWorldMap>().GetModel();
		if (model is BossGameData)
		{
			(model as BossGameData).SetDifficulties(m_HotspotGameData.GetPigLevelForHotspot(m_hardModeSelected), m_battleBalancing);
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowCharacterOverlay(m_pigPreviewRoots[pigIndex], new BossCombatant(model as BossGameData), true, false);
		}
		else if (model is PigGameData)
		{
			(model as PigGameData).SetDifficulties(m_HotspotGameData.GetPigLevelForHotspot(m_hardModeSelected), m_battleBalancing);
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowCharacterOverlay(m_pigPreviewRoots[pigIndex], new PigCombatant(model as PigGameData), true, false);
		}
	}

	public void ShowPigTooltipA()
	{
		ShowTooltip(0);
	}

	public void ShowPigTooltipB()
	{
		ShowTooltip(1);
	}

	public void ShowPigTooltipC()
	{
		ShowTooltip(2);
	}

	public void HideAllTooltips()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
	}
}
