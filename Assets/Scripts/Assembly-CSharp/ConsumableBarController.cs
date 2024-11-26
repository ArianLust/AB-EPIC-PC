using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using Rcs;
using UnityEngine;

public class ConsumableBarController : MonoBehaviour
{
	private const string CONSUMABLE_PLACEMENT = "RewardVideo.Consumable";

	private BattleMgrBase m_BattleMgr;

	[SerializeField]
	private ConsumableBattleButtonController m_ConsumableBarButtonPrefab;

	[SerializeField]
	public CoinBarController m_LuckyCoinsController;

	[SerializeField]
	private Vector2 m_OffsetPerConsumableButton = new Vector2(144f, 0f);

	[SerializeField]
	public UIGrid m_Grid;

	[SerializeField]
	public GameObject m_SponsoredAdRoot;

	[SerializeField]
	public UIInputTrigger m_SponsoredAdButton;

	[SerializeField]
	public UILabel m_SponsoredAdDesc;

	[SerializeField]
	public LootDisplayContoller m_SponsoredAdLootDisplay;

	[SerializeField]
	private UIInputTrigger m_LeaveButton;

	[SerializeField]
	private UIInputTrigger m_LeaveOverlayButton;

	private Vector3 m_BaseScaleCenter;

	private Vector3 m_BasePosRight;

	public bool entered;

	private bool m_locked;

	private ConsumableItemGameData m_SponsoredConsumable;

	private float m_lastAdCancelledTime;

	private float m_lastAdCompletedTime;

	private void Awake()
	{
		SetBackgroundButtonActive(false);
	}

	public void UpdateCoinValues()
	{
		m_LuckyCoinsController.UpdateAnim();
	}

	public void SetBattleMgr(BattleMgrBase battleMgr)
	{
		m_BattleMgr = battleMgr;
		if (m_BattleMgr.Model != null)
		{
			m_LuckyCoinsController.SetInventory(battleMgr.Model.m_ControllerInventory).SetReEnterAction(UpdateCoinValues);
			DIContainerInfrastructure.AdService.AddPlacement("RewardVideo.Consumable");
		}
	}

	private void DeRegisterEventHandlers()
	{
		m_LeaveButton.Clicked -= m_LeaveButton_Clicked;
		m_LeaveOverlayButton.Clicked -= m_LeaveButton_Clicked;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		m_SponsoredAdButton.Clicked -= OnSponsoredConsumableButtonClicked;
		m_LuckyCoinsController.DeRegisterEventHandlers();
		DIContainerInfrastructure.AdService.RewardResult -= RewardSponsoredAdResult;
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		m_SponsoredAdButton.Clicked += OnSponsoredConsumableButtonClicked;
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, HandleBackButton);
		m_LeaveButton.Clicked += m_LeaveButton_Clicked;
		m_LeaveOverlayButton.Clicked += m_LeaveButton_Clicked;
		m_LuckyCoinsController.RegisterEventHandlers();
		DIContainerInfrastructure.AdService.RewardResult += RewardSponsoredAdResult;
	}

	private void OnSponsoredConsumableButtonClicked()
	{
		if (DIContainerInfrastructure.AdService.IsAdShowPossible("RewardVideo.Consumable"))
		{
			if (!DIContainerInfrastructure.AdService.ShowAd("RewardVideo.Consumable"))
			{
				DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_no_ad_available", "There is currently no Ad scheduled"), "no_ad", DispatchMessage.Status.Info);
			}
			else
			{
				DIContainerInfrastructure.AdService.MutedGameSoundForPlacement("RewardVideo.Consumable");
			}
		}
	}

	private void RewardSponsoredAdResult(string placement, Ads.RewardResult result, string voucherId)
	{
		if (placement != "RewardVideo.Consumable")
		{
			return;
		}
		DebugLog.Log("[GachaPopupUI] Reward Result received: " + result);
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
					OnAdAbortedForConsumableRoll();
				}
			}
			else if (Time.time - m_lastAdCompletedTime < 60f)
			{
				OnAdWatchedForConsumableRoll();
			}
			break;
		case Ads.RewardResult.RewardFailed:
			OnAdAbortedForConsumableRoll();
			break;
		default:
			throw new ArgumentOutOfRangeException("result");
		}
	}

	private void OnAdWatchedForConsumableRoll()
	{
		if (m_SponsoredConsumable == null)
		{
			return;
		}
		DIContainerLogic.InventoryService.AddItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_SponsoredConsumable.Data.Level, m_SponsoredConsumable.Data.Quality, m_SponsoredConsumable.BalancingData.NameId, 1, "sponsored_ad_RewardVideo.Consumable");
		ConsumableBattleButtonController[] componentsInChildren = m_Grid.GetComponentsInChildren<ConsumableBattleButtonController>();
		ConsumableBattleButtonController[] array = componentsInChildren;
		foreach (ConsumableBattleButtonController consumableBattleButtonController in array)
		{
			DebugLog.Log("Consumable Button: " + consumableBattleButtonController.getConsumableNameId());
			if (m_SponsoredConsumable.BalancingData.NameId == consumableBattleButtonController.getConsumableNameId())
			{
				consumableBattleButtonController.SetModel(m_SponsoredConsumable.BalancingData, m_BattleMgr, m_SponsoredConsumable.Data.Level);
			}
		}
		m_SponsoredAdButton.Clicked -= OnSponsoredConsumableButtonClicked;
		StartCoroutine(LeaveSponsoredAdsBar());
	}

	private IEnumerator LeaveSponsoredAdsBar()
	{
		m_SponsoredAdRoot.GetComponent<Animation>().Play("Footer_Leave");
		yield return new WaitForSeconds(m_SponsoredAdRoot.GetComponent<Animation>()["Footer_Leave"].clip.length);
		m_SponsoredAdRoot.SetActive(false);
	}

	private void OnAdAbortedForConsumableRoll()
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_advideo_cancelled", "You did not watch the whole video"));
		StartCoroutine(LeaveSponsoredAdsBar());
	}

	private void m_LeaveButton_Clicked()
	{
		m_BattleMgr.m_BattleUI.LeaveConsumableBar();
	}

	private void OnDisable()
	{
		DeRegisterEventHandlers();
	}

	public void Enter()
	{
		entered = true;
		base.gameObject.SetActive(true);
		StartCoroutine(EnterCoroutine());
	}

	public IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("battle_consumable_enter");
		m_LuckyCoinsController.UpdateValueOnly();
		yield return StartCoroutine(RefreshBlinds());
		GetComponent<Animation>().Play("ConsumableSelection_Enter");
		if (m_SponsoredConsumable != null && DIContainerInfrastructure.AdService.IsAdShowPossible("RewardVideo.Consumable"))
		{
			m_SponsoredAdRoot.SetActive(true);
			m_SponsoredAdRoot.GetComponent<Animation>().Play("Footer_Enter");
			m_SponsoredAdLootDisplay.SetModel(m_SponsoredConsumable, new List<IInventoryItemGameData>(), LootDisplayType.None);
			m_SponsoredAdDesc.text = DIContainerInfrastructure.GetLocaService().Tr("sponsoredreward_reward_potion", new Dictionary<string, string> { { "{value_1}", m_SponsoredConsumable.ItemLocalizedName } });
		}
		else
		{
			m_SponsoredAdRoot.SetActive(false);
		}
		yield return new WaitForSeconds(GetComponent<Animation>()["ConsumableSelection_Enter"].length - 0.15f);
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("consumable_bar_entered", string.Empty);
		yield return new WaitForSeconds(0.15f);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("battle_consumable_enter");
		SetBackgroundButtonActive(true);
		RegisterEventHandlers();
	}

	private void HandleBackButton()
	{
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		m_LeaveButton_Clicked();
	}

	private IEnumerator RefreshBlinds()
	{
		foreach (Transform child in m_Grid.transform)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		yield return new WaitForEndOfFrame();
		m_SponsoredConsumable = null;
		List<string> consumableNames = new List<string>();
		foreach (List<IInventoryItemGameData> consumableRecipe in m_BattleMgr.Model.m_ControllerInventory.CraftingRecipes[InventoryItemType.Consumable].Values)
		{
			CraftingRecipeGameData recipe = consumableRecipe.LastOrDefault() as CraftingRecipeGameData;
			ConsumableItemBalancingData cbd4 = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<ConsumableItemBalancingData>(recipe.GetResultLoot().Keys.FirstOrDefault(), out cbd4) && !string.IsNullOrEmpty(cbd4.InstantBuyOfferCategoryId) && (!(cbd4.NameId == "potion_xp_01") || DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "xp_multiplier_consumable_01") <= 0))
			{
				consumableNames.Add(cbd4.NameId);
				ConsumableBattleButtonController battleButton5 = UnityEngine.Object.Instantiate(m_ConsumableBarButtonPrefab);
				battleButton5.SetModel(cbd4, m_BattleMgr, recipe.Data.Level);
				battleButton5.transform.parent = m_Grid.transform;
				battleButton5.transform.localPosition = Vector3.zero;
				if (DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").SponsoredAdPotionType == cbd4.ConsumableStatckingType)
				{
					m_SponsoredConsumable = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(recipe.Data.Level, 1, cbd4.NameId, 1) as ConsumableItemGameData;
				}
			}
		}
		foreach (ConsumableItemGameData item in m_BattleMgr.Model.m_ControllerInventory.Items[InventoryItemType.Consumable])
		{
			if (!consumableNames.Exists((string n) => n == item.BalancingData.NameId) && !string.IsNullOrEmpty(item.BalancingData.InstantBuyOfferCategoryId) && (!(item.BalancingData.NameId == "potion_xp_01") || DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "xp_multiplier_consumable_01") <= 0))
			{
				consumableNames.Add(item.BalancingData.NameId);
				ConsumableBattleButtonController battleButton4 = UnityEngine.Object.Instantiate(m_ConsumableBarButtonPrefab);
				battleButton4.SetModel(item.BalancingData, m_BattleMgr, item.Data.Level);
				battleButton4.transform.parent = m_Grid.transform;
				battleButton4.transform.localPosition = Vector3.zero;
				if (DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").SponsoredAdPotionType == item.BalancingData.ConsumableStatckingType)
				{
					m_SponsoredConsumable = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(item.Data.Level, 1, item.BalancingData.NameId, 1) as ConsumableItemGameData;
				}
			}
		}
		if (!consumableNames.Exists((string n) => n == "potion_xp_01"))
		{
			ConsumableItemBalancingData cbd3 = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<ConsumableItemBalancingData>("potion_xp_01", out cbd3) && DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "xp_multiplier_consumable_01") <= 0)
			{
				ConsumableBattleButtonController battleButton3 = UnityEngine.Object.Instantiate(m_ConsumableBarButtonPrefab);
				battleButton3.SetModel(cbd3, m_BattleMgr, 1);
				battleButton3.transform.parent = m_Grid.transform;
				battleButton3.transform.localPosition = Vector3.zero;
				if (DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").SponsoredAdPotionType == cbd3.ConsumableStatckingType)
				{
					m_SponsoredConsumable = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, cbd3.NameId, 1) as ConsumableItemGameData;
				}
			}
		}
		if (!consumableNames.Exists((string n) => n == "potion_damage_all_01"))
		{
			ConsumableItemBalancingData cbd2 = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<ConsumableItemBalancingData>("potion_damage_all_01", out cbd2))
			{
				ConsumableBattleButtonController battleButton2 = UnityEngine.Object.Instantiate(m_ConsumableBarButtonPrefab);
				battleButton2.SetModel(cbd2, m_BattleMgr, 1);
				battleButton2.transform.parent = m_Grid.transform;
				battleButton2.transform.localPosition = Vector3.zero;
				if (DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").SponsoredAdPotionType == cbd2.ConsumableStatckingType)
				{
					m_SponsoredConsumable = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, cbd2.NameId, 1) as ConsumableItemGameData;
				}
			}
		}
		if (!consumableNames.Exists((string n) => n == "potion_purify_01"))
		{
			ConsumableItemBalancingData cbd = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<ConsumableItemBalancingData>("potion_purify_01", out cbd))
			{
				ConsumableBattleButtonController battleButton = UnityEngine.Object.Instantiate(m_ConsumableBarButtonPrefab);
				battleButton.SetModel(cbd, m_BattleMgr, 1);
				battleButton.transform.parent = m_Grid.transform;
				battleButton.transform.localPosition = Vector3.zero;
				if (DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").SponsoredAdPotionType == cbd.ConsumableStatckingType)
				{
					m_SponsoredConsumable = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, cbd.NameId, 1) as ConsumableItemGameData;
				}
			}
		}
		m_Grid.Reposition();
	}

	public void Leave()
	{
		SetBackgroundButtonActive(false);
		StartCoroutine(LeaveCoroutine());
	}

	public IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("battle_consumable_leave");
		DIContainerInfrastructure.AdService.HideAd("RewardVideo.Consumable");
		GetComponent<Animation>().Play("ConsumableSelection_Leave");
		if (m_SponsoredAdRoot.activeInHierarchy)
		{
			m_SponsoredAdRoot.GetComponent<Animation>().Play("Footer_Leave");
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideMissingCurrencyOverlay();
		yield return new WaitForSeconds(GetComponent<Animation>()["ConsumableSelection_Leave"].length);
		foreach (GameObject characterBlocker in m_BattleMgr.m_CharacterInteractionBlockedItems)
		{
			characterBlocker.SetActive(false);
		}
		m_SponsoredAdRoot.SetActive(false);
		if (DIContainerInfrastructure.GetCoreStateMgr().IsShopLoading() || DIContainerInfrastructure.GetCoreStateMgr().IsShopOpen())
		{
			DIContainerInfrastructure.GetCoreStateMgr().LeaveShop();
		}
		entered = false;
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("battle_consumable_leave");
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("battle_consumable_leave", string.Empty);
		base.gameObject.SetActive(false);
	}

	public void SetBackgroundButtonActive(bool activate)
	{
		m_LeaveOverlayButton.gameObject.SetActive(activate);
	}

	public void ConsumableClicked(ConsumableItemGameData consumable)
	{
		if (consumable == null)
		{
			DebugLog.Log("No Consumable Item Clicked");
			return;
		}
		DebugLog.Log("Consumable Item Clicked: " + consumable.Name);
		if (!m_BattleMgr.Model.CurrentCombatant.CombatantView.m_IsWaitingForInput)
		{
			DebugLog.Log("No bird on turn");
		}
		else if (m_BattleMgr.Model.CurrentCombatant.UsedConsumable)
		{
			DebugLog.Log("Already used Consumable this turn");
		}
		else
		{
			m_BattleMgr.Model.CurrentCombatant.CombatantView.ActivateConsumableSkill(consumable);
		}
	}

	public void RefreshCoins()
	{
		m_LuckyCoinsController.UpdateAnim();
	}
}
