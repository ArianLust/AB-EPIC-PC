using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Shared.BalancingData;
using ABH.Shared.Events.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using Rcs;
using UnityEngine;

public class ArenaBattlePreperationUI : MonoBehaviour
{
	private const string BUFF_PLACEMENT = "RewardVideo.ArenaBattleBuff";

	[SerializeField]
	private const string ENTER_TRIGGER_PARAMETER = "Show";

	[SerializeField]
	private const string LEAVE_TRIGGER_PARAMETER = "Hide";

	[SerializeField]
	private const string UNRANKED_BOOL_PARAMETER = "IsUnrankedBattle";

	private Animator m_animator;

	[SerializeField]
	private List<UIPanel> m_Panels;

	[SerializeField]
	private Action m_ActionOnLeave;

	[SerializeField]
	private Animation m_BackAnimation;

	[SerializeField]
	private Animation m_StartAnimation;

	[SerializeField]
	private UILabel m_StageNameText;

	[SerializeField]
	private UILabel m_TopLootCountText;

	[SerializeField]
	private UILabel m_BirdCountText;

	[SerializeField]
	private UIAtlas m_ArenaAtlas;

	[SerializeField]
	public UIInputTrigger m_startButtonTrigger;

	[SerializeField]
	public UIInputTrigger m_backButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_pvpEnergyButtonTrigger;

	[SerializeField]
	private ResourceCostBlind m_pvpEnergyCost;

	[SerializeField]
	private LootDisplayContoller m_MajorLoot;

	[SerializeField]
	private GameObject m_WheelObject;

	[SerializeField]
	private UIAtlas m_GenericIconsAtlas;

	[SerializeField]
	private UIAtlas m_ResourceIconsAtlas;

	[SerializeField]
	private Transform m_CharacterRoot;

	[SerializeField]
	private GameObject m_CharacterButtonPrefab;

	[SerializeField]
	private UISprite[] m_EnergyCharges;

	[SerializeField]
	private GameObject m_PvpEnergyRoot;

	[SerializeField]
	private Transform[] m_EnemyCharacterRoots;

	[SerializeField]
	private Transform m_EnemyBannerRoot;

	[SerializeField]
	private Transform m_OwnBannerRoot;

	[SerializeField]
	private UILabel m_OwnPowerLevelLabel;

	[SerializeField]
	private UILabel m_EnemyPowerLevelLabel;

	[SerializeField]
	private UILabel m_OwnPlayerName;

	[SerializeField]
	private OpponentInfoElement m_opponentAvatar;

	[SerializeField]
	private CharacterControllerCamp m_CharacterCampPrefab;

	[SerializeField]
	private UILabel m_OutOfEnergyText;

	[SerializeField]
	private List<Vector3List> m_BirdPositionsByCount = new List<Vector3List>();

	[Header("Sponsored Buff")]
	private SkillBattleDataBase m_SponsoredAdSkill;

	private float m_lastAdCancelledTime;

	private float m_lastAdCompletedTime;

	[SerializeField]
	private GameObject m_SponsoredBuffRoot;

	[SerializeField]
	private UIInputTrigger m_SponsoredBuffButton;

	[SerializeField]
	private GameObject m_SponsoredBuffButtonRoot;

	[SerializeField]
	private GameObject m_SponsoredBuffCheckRoot;

	[SerializeField]
	private UILabel m_SponsoredText;

	[SerializeField]
	private GameObject m_HealthBuffRoot;

	[SerializeField]
	private Color m_PowerLevelColorDefault;

	[SerializeField]
	private Color m_PowerLevelColorEasy;

	[SerializeField]
	public Color m_PowerLevelColorNormal;

	[SerializeField]
	public Color m_PowerLevelColorHard;

	private List<BattlePrepCharacterButton> m_buttonList = new List<BattlePrepCharacterButton>();

	private bool m_EnteringBattle;

	private ArenaCampStateMgr m_campStateMgr;

	[HideInInspector]
	public bool m_OneBirdLeft;

	private int m_BattleLevel;

	public float m_RotationOffset = 270f;

	private int m_MaxSelectableBirdsCount;

	private List<int> m_SelectedBirds;

	private bool m_birdSelectionFirst;

	private PublicPlayerData m_opponent;

	private List<int> m_EnemySelectedBirds = new List<int>();

	[HideInInspector]
	public bool m_Entered;

	private BattleBalancingData m_battleBalancing;

	[SerializeField]
	private UIGrid m_BirdGrid;

	private int m_opponentPowerlevel;

	private int m_ownPowerLevel;

	private void Awake()
	{
		foreach (UIPanel panel in m_Panels)
		{
			panel.enabled = false;
		}
		if (!m_animator)
		{
			m_animator = GetComponent<Animator>();
		}
		base.transform.position += DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.transform.position;
		DIContainerInfrastructure.AdService.AddPlacement("RewardVideo.ArenaBattleBuff");
		m_SponsoredAdSkill = new SkillGameData(DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").SponsoredAdBuffName).GenerateSkillBattleData();
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(1, HandleBackButton);
		m_startButtonTrigger.Clicked += OnStartButtonClicked;
		m_backButtonTrigger.Clicked += OnBackButtonClicked;
		m_SponsoredBuffButton.Clicked += OnSponsoredBuffButtonClicked;
		m_pvpEnergyButtonTrigger.Clicked += EnergyButtonTriggerClicked;
		DIContainerInfrastructure.GetCoreStateMgr().OnShopClosed += OnShopClosed;
		DIContainerInfrastructure.GetCurrentPlayer().GlobalPvPStateChanged += OnGlobalPvPStateChanged;
		DIContainerInfrastructure.AdService.RewardResult += RewardSponsoredAdResult;
	}

	private void DeregisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(1);
		m_startButtonTrigger.Clicked -= OnStartButtonClicked;
		m_backButtonTrigger.Clicked -= OnBackButtonClicked;
		m_pvpEnergyButtonTrigger.Clicked -= EnergyButtonTriggerClicked;
		m_SponsoredBuffButton.Clicked -= OnSponsoredBuffButtonClicked;
		DIContainerInfrastructure.GetCoreStateMgr().OnShopClosed -= OnShopClosed;
		DIContainerInfrastructure.GetCurrentPlayer().GlobalPvPStateChanged -= OnGlobalPvPStateChanged;
		DIContainerInfrastructure.AdService.RewardResult -= RewardSponsoredAdResult;
	}

	private void OnSponsoredBuffButtonClicked()
	{
		if (DIContainerInfrastructure.AdService.IsAdShowPossible("RewardVideo.ArenaBattleBuff") && m_battleBalancing.UsableFriendBirdsCount <= 0 && string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentPvPBuff))
		{
			m_SponsoredBuffRoot.SetActive(false);
			if (!DIContainerInfrastructure.AdService.ShowAd("RewardVideo.ArenaBattleBuff"))
			{
				m_SponsoredBuffRoot.SetActive(true);
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_no_ad_available", "There is currently no Ad scheduled"), "no_ad", DispatchMessage.Status.Info);
			}
			else
			{
				m_startButtonTrigger.gameObject.SetActive(false);
				DIContainerInfrastructure.AdService.MutedGameSoundForPlacement("RewardVideo.ArenaBattleBuff");
			}
		}
	}

	private void RewardSponsoredAdResult(string placement, Ads.RewardResult result, string voucherId)
	{
		if (placement != "RewardVideo.ArenaBattleBuff")
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
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		currentPlayer.Data.CurrentPvPBuff = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").SponsoredAdBuffName;
		m_SponsoredBuffButton.Clicked -= OnSponsoredBuffButtonClicked;
		if (string.IsNullOrEmpty(currentPlayer.Data.CurrentPvPBuff))
		{
			DebugLog.Log("OnAdWatchedForBuffRoll: DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentPvPBuff is empty or null");
			StartCoroutine(LeaveSponsoredAdsBar());
		}
		else
		{
			ClientInfo.CurrentBattleStartGameData.m_SponsoredEnvironmentalEffect = currentPlayer.Data.CurrentPvPBuff;
			m_SponsoredBuffRoot.SetActive(true);
			m_SponsoredBuffButtonRoot.SetActive(false);
			m_SponsoredBuffCheckRoot.SetActive(true);
		}
		int pvPTeamPowerLevel = DIContainerInfrastructure.GetPowerLevelCalculator().GetPvPTeamPowerLevel(DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer, DIContainerInfrastructure.GetCurrentPlayer().SelectedBirdIndices);
		SetPowerLevelLabel(pvPTeamPowerLevel);
	}

	private IEnumerator LeaveSponsoredAdsBar()
	{
		m_SponsoredBuffRoot.GetComponent<Animation>().Play("Footer_Leave");
		yield return new WaitForSeconds(m_SponsoredBuffRoot.GetComponent<Animation>()["Footer_Leave"].clip.length);
		m_SponsoredBuffRoot.SetActive(false);
	}

	private void OnAdAbortedForBuffRoll()
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_advideo_cancelled", "You did not watch the whole video"));
	}

	private void OnGlobalPvPStateChanged(CurrentGlobalEventState oldState, CurrentGlobalEventState newState)
	{
		Leave();
	}

	private void OnShopClosed()
	{
	}

	private void EnergyButtonTriggerClicked()
	{
		BasicShopOfferBalancingData offer = DIContainerLogic.GetShopService().GetShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), "shop_global_pvp_energy").FirstOrDefault();
		List<Requirement> failed = new List<Requirement>();
		if (!DIContainerLogic.GetShopService().IsOfferBuyable(DIContainerInfrastructure.GetCurrentPlayer(), offer, out failed))
		{
			Requirement requirement = failed.FirstOrDefault();
			RequirementType requirementType = requirement.RequirementType;
			if (requirementType != RequirementType.PayItem)
			{
				return;
			}
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, requirement.NameId, out data))
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
			}
		}
		else if (DIContainerLogic.GetShopService().BuyShopOffer(DIContainerInfrastructure.GetCurrentPlayer(), offer, "RefillPvPEnergy") != null)
		{
			StartCoroutine(UpdateEnergy());
			m_pvpEnergyButtonTrigger.Clicked -= EnergyButtonTriggerClicked;
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideMissingCurrencyOverlay();
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		}
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		Leave();
	}

	public void OnStartButtonClicked()
	{
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
		if (!ClientInfo.CurrentBattleStartGameData.m_IsUnranked && DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "pvp_energy") <= 0)
		{
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "pvp_energy", out data))
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowMissingArenaEnergyOverlay();
				return;
			}
		}
		DIContainerLogic.InventoryService.FixInventoryOfBirds(DIContainerInfrastructure.GetCurrentPlayer());
		m_EnteringBattle = true;
		Leave();
		list = Enumerable.ToList(list.OrderBy((BirdGameData b) => b.BalancingData.SortPriority));
		if (ClientInfo.CurrentBattleStartGameData != null)
		{
			ClientInfo.CurrentBattleStartGameData.m_Birds = list;
			ClientInfo.CurrentBattleStartGameData.m_BirdBanner = DIContainerInfrastructure.GetCurrentPlayer().BannerGameData;
			DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentPvPBuff = string.Empty;
			DIContainerInfrastructure.GetCoreStateMgr().m_ReturnToArena = true;
			DIContainerInfrastructure.GetCurrentPlayer().Data.SelectedPvPBirdIndices = DIContainerInfrastructure.GetCurrentPlayer().SelectedBirdIndices;
			DIContainerInfrastructure.GetCoreStateMgr().GotoBattle(ClientInfo.CurrentBattleStartGameData.m_BackgroundAssetId);
		}
	}

	public void OnBackButtonClicked()
	{
		Leave();
	}

	public void Enter(ArenaCampStateMgr mapMgr, PublicPlayerData opponent, List<int> enemySelectedBirdsIndices, bool unranked = false, FriendGameData friend = null)
	{
		if (enemySelectedBirdsIndices == null || enemySelectedBirdsIndices.Count == 0)
		{
			if (opponent.Birds == null)
			{
				return;
			}
			enemySelectedBirdsIndices = new List<int>();
			for (int i = 0; i < Mathf.Min(3, opponent.Birds.Count); i++)
			{
				enemySelectedBirdsIndices.Add(i);
			}
		}
		CreateBirds();
		m_Entered = true;
		m_campStateMgr = mapMgr;
		m_opponent = opponent;
		m_EnemySelectedBirds = enemySelectedBirdsIndices;
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		List<BirdGameData> list = new List<BirdGameData>();
		for (int j = 0; j < m_EnemySelectedBirds.Count; j++)
		{
			BirdGameData giantEnemyBird = new BirdGameData(m_opponent.Birds[m_EnemySelectedBirds[j]]);
			if (giantEnemyBird.ClassItem == null || !giantEnemyBird.ClassItem.IsValidForBird(giantEnemyBird))
			{
				InventoryGameData inventoryGameData = new InventoryGameData(opponent.Inventory);
				DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { inventoryGameData.Items[InventoryItemType.Class].FirstOrDefault((IInventoryItemGameData item) => item.IsValidForBird(giantEnemyBird)) }, InventoryItemType.Class, giantEnemyBird.InventoryGameData);
			}
			list.Add(giantEnemyBird);
		}
		BattleStartGameData battleStartGameData = new BattleStartGameData();
		battleStartGameData.m_BackgroundAssetId = "Battleground_Arena_01";
		battleStartGameData.m_RageAvailiable = DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_rage");
		battleStartGameData.m_Birds = new List<BirdGameData>();
		battleStartGameData.m_PvPBirds = list;
		battleStartGameData.m_BattleBalancingNameId = ((!unranked) ? ("battle_pvp_" + DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData.Data.CurrentLeague.ToString("00")) : "battle_pvp_unranked");
		battleStartGameData.callback = DIContainerLogic.PvPSeasonService.RemoveCurrentOpponent;
		battleStartGameData.m_Inventory = currentPlayer.InventoryGameData;
		battleStartGameData.m_InvokerLevel = currentPlayer.Data.Level;
		battleStartGameData.m_InjectableParticipantTable = null;
		battleStartGameData.m_BattleRandomSeed = 0;
		battleStartGameData.m_PossibleFollowUpBattles = new List<string>();
		battleStartGameData.m_SponsoredEnvironmentalEffect = currentPlayer.Data.CurrentPvPBuff;
		battleStartGameData.m_BirdBanner = null;
		battleStartGameData.m_PigBanner = new BannerGameData(m_opponent.Banner);
		battleStartGameData.m_FactionBuffs = new Dictionary<Faction, Dictionary<string, float>>();
		battleStartGameData.m_IsUnranked = unranked;
		battleStartGameData.m_OpponentId = ((friend == null) ? opponent.SocialId : friend.FriendId);
		ClientInfo.CurrentBattleStartGameData = battleStartGameData;
		m_EnergyCharges.ForEach(delegate(UISprite energy)
		{
			energy.gameObject.SetActive(!unranked);
		});
		SetBattleBalancing(DIContainerBalancing.Service.GetBalancingData<BattleBalancingData>(ClientInfo.CurrentBattleStartGameData.m_BattleBalancingNameId));
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Enter(false);
		base.gameObject.SetActive(true);
		m_campStateMgr.m_CampUI.Leave();
		m_startButtonTrigger.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	public ArenaBattlePreperationUI SetBattleBalancing(BattleBalancingData battleBalancing)
	{
		m_battleBalancing = battleBalancing;
		return this;
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("animate_bprep");
		m_ActionOnLeave = null;
		m_PvpEnergyRoot.SetActive(false);
		FillInfos();
		m_ownPowerLevel = DIContainerInfrastructure.GetPowerLevelCalculator().GetPvPTeamPowerLevel(DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer, DIContainerInfrastructure.GetCurrentPlayer().SelectedBirdIndices);
		m_opponentPowerlevel = DIContainerInfrastructure.GetPowerLevelCalculator().GetPvPTeamPowerLevel(m_opponent, m_EnemySelectedBirds);
		SetPowerLevelLabel(m_ownPowerLevel);
		m_EnemyPowerLevelLabel.text = DIContainerInfrastructure.GetLocaService().Tr("player_stat_powerlevel").Replace("{value_1}", m_opponentPowerlevel.ToString());
		yield return new WaitForEndOfFrame();
		m_animator.SetBool("IsUnrankedBattle", ClientInfo.CurrentBattleStartGameData.m_IsUnranked);
		m_animator.SetTrigger("Show");
		m_BackAnimation.Play("BackButton_Enter");
		m_StartAnimation.Play("StartBattleButton_Enter");
		foreach (UIPanel panel in m_Panels)
		{
			panel.enabled = true;
		}
		EnterCoinBars();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("battle_preparation", string.Empty);
		RegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("animate_bprep");
	}

	private bool IsSponsoredAdPossible()
	{
		return DIContainerInfrastructure.AdService.IsAdShowPossible("RewardVideo.ArenaBattleBuff") || !string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentPvPBuff);
	}

	private void EnterSponsoredBuff(bool playChangedAnimations)
	{
		m_SponsoredBuffRoot.SetActive(true);
		m_SponsoredBuffRoot.GetComponent<Animation>().Play("Footer_Enter");
		if (m_SponsoredAdSkill != null)
		{
			m_SponsoredText.text = m_SponsoredAdSkill.GetLocalizedDescription(null);
		}
		if (string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentPvPBuff))
		{
			m_SponsoredBuffButtonRoot.SetActive(true);
			m_SponsoredBuffCheckRoot.SetActive(false);
		}
		else
		{
			m_SponsoredBuffButtonRoot.SetActive(false);
			m_SponsoredBuffCheckRoot.SetActive(true);
		}
	}

	private void EnterCoinBars()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u,
			showFriendshipEssence = false,
			showLuckyCoins = true,
			showSnoutlings = false,
			showEnergy = false
		}, true);
	}

	private void LeaveCoinBars()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
	}

	public void Leave()
	{
		DeregisterEventHandler();
		StartCoroutine(LeaveCoroutine());
	}

	public IEnumerator LeaveCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.Leave();
		if (m_SponsoredBuffRoot.activeInHierarchy)
		{
			m_SponsoredBuffRoot.GetComponent<Animation>().Play("Footer_Leave");
		}
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("animate_bprep");
		m_animator.SetTrigger("Hide");
		m_BackAnimation.Play("BackButton_Leave");
		m_StartAnimation.Play("StartBattleButton_Leave");
		if (m_PvpEnergyRoot.activeInHierarchy)
		{
			m_PvpEnergyRoot.PlayAnimationOrAnimatorState("Footer_Leave");
		}
		LeaveCoinBars();
		if (m_SponsoredBuffRoot.activeInHierarchy)
		{
			yield return StartCoroutine(LeaveSponsoredAdsBar());
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("animate_bprep");
		m_startButtonTrigger.gameObject.SetActive(false);
		foreach (UIPanel panel in m_Panels)
		{
			panel.enabled = false;
		}
		if (m_ActionOnLeave != null)
		{
			m_ActionOnLeave();
		}
		else if (!m_EnteringBattle)
		{
			m_campStateMgr.RegisterEventHandler();
			m_campStateMgr.m_CampUI.Enter();
		}
		m_Entered = false;
		yield return new WaitForSeconds(0.317f); // wait until hide animation finishes (idk how to get the anim time from the animator so i hardcoded the wait time)
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
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		CreateOpponentInfo();
		string eventPlayerName = player.SocialEnvironmentGameData.Data.EventPlayerName;
		if (!string.IsNullOrEmpty(eventPlayerName))
		{
			m_OwnPlayerName.text = eventPlayerName;
		}
		else
		{
			m_OwnPlayerName.text = DIContainerInfrastructure.GetLocaService().Tr("pvp_bps_user_name", "You");
		}
		m_MaxSelectableBirdsCount = ((m_battleBalancing != null) ? m_battleBalancing.MaxBirdsInBattle : 0);
		m_birdSelectionFirst = true;
		m_SelectedBirds = new List<int>();
		List<int> list = Enumerable.ToList(player.SelectedBirdIndices.Where((int i) => player.Birds.Count >= i || player.Birds[i] == null));
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
			int num = m_MaxSelectableBirdsCount - player.SelectedBirdIndices.Count;
			for (int l = 0; l < player.Birds.Count; l++)
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
		player.SelectedBirdIndices = m_SelectedBirds;
		m_OneBirdLeft = m_SelectedBirds.Count <= 1;
		m_StageNameText.text = DIContainerInfrastructure.GetLocaService().Tr(m_battleBalancing.NameId + "_name");
		int num2 = Mathf.Min(m_buttonList.Count, m_MaxSelectableBirdsCount);
		m_BirdCountText.text = GetCurrentBirdCount() + "/" + num2;
		SetTopLoot();
		UpdateSelectedBirds();
		StartCoroutine(UpdateEnergy());
		if (IsSponsoredAdPossible() && !m_PvpEnergyRoot.activeInHierarchy)
		{
			EnterSponsoredBuff(false);
		}
		else
		{
			m_SponsoredBuffRoot.SetActive(false);
		}
	}

	private void SetTopLoot()
	{
		LootTableBalancingData balancing = null;
		if (m_battleBalancing.LootTableWheel == null || !DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(m_battleBalancing.LootTableWheel.Keys.FirstOrDefault(), out balancing))
		{
			m_MajorLoot.gameObject.SetActive(false);
			m_WheelObject.SetActive(false);
			return;
		}
		m_MajorLoot.gameObject.SetActive(true);
		m_WheelObject.SetActive(true);
		LootTableBalancingData balancing2 = null;
		if (DIContainerBalancing.LootTableBalancingDataPovider.TryGetBalancingData(balancing.LootTableEntries[0].NameId, out balancing2))
		{
			m_MajorLoot.SetModel(null, new List<IInventoryItemGameData>(), LootDisplayType.Major);
			return;
		}
		int num = balancing.LootTableEntries[0].BaseValue;
		BonusEventBalancingData currentValidBalancing = DIContainerLogic.GetBonusEventService.m_CurrentValidBalancing;
		if (currentValidBalancing != null && currentValidBalancing.BonusType == BonusEventType.ArenaPointBonus)
		{
			float num2 = currentValidBalancing.BonusFactor / 100f;
			num += (int)((float)num * num2);
		}
		IInventoryItemGameData mainItem = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_BattleLevel, 1, balancing.LootTableEntries[0].NameId, num, EquipmentSource.LootBird);
		m_MajorLoot.SetModel(mainItem, new List<IInventoryItemGameData>(), LootDisplayType.Major);
	}

	private IEnumerator UpdateEnergy()
	{
		int pvpEnergyCharges = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "pvp_energy");
		for (int i = 0; i < m_EnergyCharges.Length; i++)
		{
			m_EnergyCharges[i].spriteName = ((pvpEnergyCharges <= i) ? "PvPCharge_Empty" : "PvPCharge_Full");
		}
		if (ClientInfo.CurrentBattleStartGameData.m_IsUnranked)
		{
			m_PvpEnergyRoot.SetActive(false);
		}
		else if (pvpEnergyCharges <= 0)
		{
			m_SponsoredBuffRoot.SetActive(false);
			bool playAnimation = !m_PvpEnergyRoot.activeInHierarchy;
			BasicShopOfferBalancingData pvpEnergyOffer = DIContainerLogic.GetShopService().GetShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), "shop_global_pvp_energy").FirstOrDefault();
			Requirement cost = DIContainerLogic.GetShopService().GetBuyResourcesRequirements(1, pvpEnergyOffer, false).FirstOrDefault();
			IInventoryItemBalancingData costItem = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(cost.NameId);
			m_pvpEnergyCost.SetModel(costItem.AssetBaseId, m_GenericIconsAtlas, cost.Value, string.Empty);
			m_PvpEnergyRoot.SetActive(true);
			StartCoroutine(UpdateOutOfEnergyText());
			if (playAnimation)
			{
				m_PvpEnergyRoot.PlayAnimationOrAnimatorState("Footer_Enter");
			}
		}
		else if (m_PvpEnergyRoot.activeInHierarchy)
		{
			yield return StartCoroutine(LeaveAndDisable());
			if (IsSponsoredAdPossible())
			{
				EnterSponsoredBuff(true);
			}
		}
		else
		{
			m_PvpEnergyRoot.SetActive(false);
		}
	}

	private IEnumerator UpdateOutOfEnergyText()
	{
		string locaString = DIContainerInfrastructure.GetLocaService().Tr(m_OutOfEnergyText.GetComponent<LocaScript>().m_locaIdent, string.Empty);
		while (m_PvpEnergyRoot.activeInHierarchy)
		{
			TimeSpan time = DIContainerLogic.PvPSeasonService.GetDailyPvpRefreshTimeLeft(DIContainerInfrastructure.GetCurrentPlayer(), DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData);
			m_OutOfEnergyText.text = locaString.Replace("{value_1}", DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(time));
			yield return new WaitForSeconds(1f);
			if (time.TotalSeconds <= 0.0)
			{
				StartCoroutine(UpdateEnergy());
				break;
			}
		}
	}

	private IEnumerator LeaveAndDisable()
	{
		yield return new WaitForSeconds(m_PvpEnergyRoot.PlayAnimationOrAnimatorState("Footer_Leave"));
		m_PvpEnergyRoot.SetActive(false);
	}

	private void CreateOpponentInfo()
	{
		m_opponentAvatar.SetModel(new OpponentGameData(m_opponent), false);
		if (ClientInfo.CurrentBattleStartGameData == null)
		{
			return;
		}
		int num = 0;
		foreach (BirdGameData pvPBird in ClientInfo.CurrentBattleStartGameData.m_PvPBirds)
		{
			foreach (Transform item in m_EnemyCharacterRoots[num])
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			CharacterControllerCamp characterControllerCamp = UnityEngine.Object.Instantiate(m_CharacterCampPrefab);
			characterControllerCamp.SetModel(pvPBird);
			characterControllerCamp.transform.parent = m_EnemyCharacterRoots[num];
			characterControllerCamp.transform.localPosition = Vector3.zero;
			characterControllerCamp.transform.localScale = Vector3.one;
			UnityHelper.SetLayerRecusively(characterControllerCamp.gameObject, LayerMask.NameToLayer("Interface"));
			num++;
		}
		foreach (Transform item2 in m_EnemyBannerRoot)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		BannerGameData pigBanner = ClientInfo.CurrentBattleStartGameData.m_PigBanner;
		CharacterControllerCamp characterControllerCamp2 = UnityEngine.Object.Instantiate(m_CharacterCampPrefab);
		characterControllerCamp2.SetModel(pigBanner);
		characterControllerCamp2.transform.parent = m_EnemyBannerRoot;
		characterControllerCamp2.transform.localPosition = Vector3.zero;
		characterControllerCamp2.transform.localScale = Vector3.one;
		UnityHelper.SetLayerRecusively(characterControllerCamp2.gameObject, LayerMask.NameToLayer("Interface"));
	}

	private void ClearOldBirdButtons()
	{
		foreach (BattlePrepCharacterButton button in m_buttonList)
		{
			button.Selected -= OnBirdSelected;
			UnityEngine.Object.Destroy(button.gameObject);
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
		m_ownPowerLevel = DIContainerInfrastructure.GetPowerLevelCalculator().GetPvPTeamPowerLevel(DIContainerInfrastructure.GetCurrentPlayer().PublicPlayer, DIContainerInfrastructure.GetCurrentPlayer().SelectedBirdIndices);
		SetPowerLevelLabel(m_ownPowerLevel);
	}

	private void SetPowerLevelLabel(int calcPowerLevel)
	{
		if (!string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentPvPBuff))
		{
			calcPowerLevel += Mathf.RoundToInt((float)calcPowerLevel * (DIContainerBalancing.Service.GetBalancingData<SkillBalancingData>("env_sponsored_health_and_attack").SkillParameters.FirstOrDefault().Value / 100f));
		}
		m_OwnPowerLevelLabel.color = ((!string.IsNullOrEmpty(DIContainerInfrastructure.GetCurrentPlayer().Data.CurrentSponsoredBuff)) ? m_PowerLevelColorEasy : m_PowerLevelColorDefault);
		m_OwnPowerLevelLabel.text = DIContainerInfrastructure.GetLocaService().Tr("player_stat_powerlevel").Replace("{value_1}", calcPowerLevel.ToString());
		UpdatePigPowerLevelColor();
	}

	private void UpdateSelectedBirds()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		m_SelectedBirds = new List<int>();
		for (int i = 0; i < m_buttonList.Count; i++)
		{
			BattlePrepCharacterButton battlePrepCharacterButton = m_buttonList[i];
			if (battlePrepCharacterButton.IsSelected())
			{
				m_SelectedBirds.Add(i);
			}
		}
		currentPlayer.SelectedBirdIndices = m_SelectedBirds;
		m_BirdCountText.color = ((m_SelectedBirds.Count == Mathf.Min(currentPlayer.Birds.Count, m_MaxSelectableBirdsCount)) ? DIContainerLogic.GetVisualEffectsBalancing().ColorOffersBuyable : DIContainerLogic.GetVisualEffectsBalancing().ColorOffersNotBuyable);
		m_OneBirdLeft = m_SelectedBirds.Count <= ((currentPlayer.Birds.Count >= 4) ? 1 : Mathf.Min(currentPlayer.Birds.Count, m_MaxSelectableBirdsCount));
	}

	private void CreateBirds()
	{
		int num = 0;
		ClearOldBirdButtons();
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (currentPlayer.Birds != null)
		{
			m_birdSelectionFirst = true;
			for (int i = 0; i < currentPlayer.Birds.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_CharacterButtonPrefab);
				gameObject.transform.parent = m_CharacterRoot;
				BattlePrepCharacterButton component = gameObject.GetComponent<BattlePrepCharacterButton>();
				component.Selected -= OnBirdSelected;
				component.Selected += OnBirdSelected;
				component.Init(currentPlayer.Birds[i], this);
				m_buttonList.Add(component);
			}
			m_birdSelectionFirst = false;
		}
		if (m_BirdGrid != null)
		{
			m_BirdGrid.repositionNow = true;
		}
		if (m_OwnBannerRoot.childCount > 0)
		{
			UnityEngine.Object.Destroy(m_OwnBannerRoot.GetChild(0).gameObject);
		}
		BannerGameData bannerGameData = currentPlayer.BannerGameData;
		CharacterControllerCamp characterControllerCamp = UnityEngine.Object.Instantiate(m_CharacterCampPrefab);
		characterControllerCamp.SetModel(bannerGameData);
		characterControllerCamp.transform.parent = m_OwnBannerRoot;
		characterControllerCamp.transform.localPosition = Vector3.zero;
		characterControllerCamp.transform.localScale = Vector3.one;
		UnityHelper.SetLayerRecusively(characterControllerCamp.gameObject, LayerMask.NameToLayer("Interface"));
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

	private void UpdatePigPowerLevelColor()
	{
		float num = (float)(m_opponentPowerlevel - m_ownPowerLevel) / (float)m_ownPowerLevel;
		if (num < 0f)
		{
			m_EnemyPowerLevelLabel.color = m_PowerLevelColorEasy;
		}
		else if (num == 0f)
		{
			m_EnemyPowerLevelLabel.color = m_PowerLevelColorDefault;
		}
		else if (num < 0.15f)
		{
			m_EnemyPowerLevelLabel.color = m_PowerLevelColorNormal;
		}
		else
		{
			m_EnemyPowerLevelLabel.color = m_PowerLevelColorHard;
		}
	}
}
