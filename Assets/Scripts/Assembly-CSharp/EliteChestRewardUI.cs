using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class EliteChestRewardUI : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_ConfirmPrizeButton;

	[SerializeField]
	public UIInputTrigger m_RerollButton;

	[SerializeField]
	public UIInputTrigger m_openBoxButton;

	[SerializeField]
	private LootDisplayContoller m_ResultLootController;

	[SerializeField]
	private GameObject m_LootRoot;

	[SerializeField]
	private UILabel m_ResultTitleLabel;

	[SerializeField]
	private ResourceCostBlind m_CostBlind;

	[SerializeField]
	private SoundTriggerList m_SoundTriggers;

	[SerializeField]
	[Header("ChestPreviewGrid")]
	private EliteChestInfoPopup m_contentPreviewGridRoot;

	private List<IInventoryItemGameData> m_availableLoot;

	private bool m_isLeaving;

	[HideInInspector]
	public bool m_IsShowing;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_EliteChestUnlockPopup = this;
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	public void Init()
	{
		base.gameObject.SetActive(true);
		m_IsShowing = true;
		m_isLeaving = false;
		DIContainerInfrastructure.LocationStateMgr.WorldMenuUI.Leave();
		SetDragControllerActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u
		}, true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.LeaveLevelDisplay();
		WorldBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island");
		Requirement rerollChestRequirement = balancingData.RerollChestRequirement;
		if (rerollChestRequirement != null)
		{
			m_CostBlind.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(rerollChestRequirement.NameId).AssetBaseId, null, rerollChestRequirement.Value, string.Empty);
		}
		else
		{
			m_CostBlind.gameObject.SetActive(false);
			m_RerollButton.gameObject.SetActive(false);
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u,
			showLuckyCoins = true
		}, true);
		InitializeContentPreview();
		RegisterEventHandlers();
		m_contentPreviewGridRoot.gameObject.PlayAnimationOrAnimatorState("Enter");
		base.gameObject.PlayAnimationOrAnimatorState("Popup_EliteChestUnlock_Step1_Enter");
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		if ((bool)m_RerollButton)
		{
			m_RerollButton.Clicked += RerollButtonClicked;
		}
		if ((bool)m_ConfirmPrizeButton)
		{
			m_ConfirmPrizeButton.Clicked += ConfirmRewardButtonClicked;
		}
		if ((bool)m_openBoxButton)
		{
			m_openBoxButton.Clicked += OpenChest;
		}
	}

	private void OpenChest()
	{
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		if (m_availableLoot == null || m_availableLoot.Count == 0)
		{
			m_availableLoot = DIContainerLogic.EventSystemService.GetAvailableEliteChestReward(player);
		}
		IInventoryItemGameData inventoryItemGameData = null;
		if (!string.IsNullOrEmpty(player.Data.CachedChestRewardItem))
		{
			try
			{
				inventoryItemGameData = m_availableLoot.First((IInventoryItemGameData option) => option.ItemData.NameId == player.Data.CachedChestRewardItem);
			}
			catch (Exception)
			{
				DebugLog.Warn(GetType(), "OpenChest: Cached Reward is no longer available!");
			}
		}
		if (inventoryItemGameData == null)
		{
			int index = UnityEngine.Random.Range(0, m_availableLoot.Count);
			inventoryItemGameData = m_availableLoot[index];
		}
		m_availableLoot.Remove(inventoryItemGameData);
		m_contentPreviewGridRoot.gameObject.PlayAnimationOrAnimatorState("Leave");
		player.Data.CachedChestRewardItem = inventoryItemGameData.ItemData.NameId;
		player.SavePlayerData();
		player.RolledChestReward = inventoryItemGameData;
		ShowItemReward(inventoryItemGameData);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("chest_enter_result");
		StartCoroutine(OpenChestAnimationCoroutine());
	}

	private IEnumerator OpenChestAnimationCoroutine()
	{
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_EliteChestUnlock_Step1_Step2"));
		if (!m_isLeaving)
		{
			m_contentPreviewGridRoot.gameObject.PlayAnimationOrAnimatorState("Enter");
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("chest_enter_result");
	}

	private void ShowItemReward(IInventoryItemGameData item)
	{
		m_ResultLootController.gameObject.SetActive(true);
		m_ResultLootController.SetModel(item, null, LootDisplayType.Major);
		m_ResultTitleLabel.text = item.ItemLocalizedName;
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u,
			showLuckyCoins = true
		}, true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(2, ConfirmRewardButtonClicked);
	}

	private void DeRegisterEventHandlers(bool buttonsOnly = false)
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(2);
		if ((bool)m_RerollButton)
		{
			m_RerollButton.Clicked -= RerollButtonClicked;
		}
		if ((bool)m_ConfirmPrizeButton)
		{
			m_ConfirmPrizeButton.Clicked -= ConfirmRewardButtonClicked;
		}
		if ((bool)m_openBoxButton)
		{
			m_openBoxButton.Clicked -= OpenChest;
		}
	}

	private IEnumerator ReturnToClosedChestCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_EliteChestUnlock_Step2_Step1"));
		RegisterEventHandlers();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 2u
		}, true);
	}

	private void RerollButtonClicked()
	{
		DebugLog.Log("Reroll");
		DeRegisterEventHandlers();
		WorldBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island");
		Requirement rerollChestRequirement = balancingData.RerollChestRequirement;
		if (DIContainerLogic.RequirementService.ExecuteRequirements(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, new List<Requirement> { rerollChestRequirement }, "reroll_elite_chest"))
		{
			DIContainerInfrastructure.GetCurrentPlayer().Data.CachedChestRewardItem = null;
			StartCoroutine(ReturnToClosedChestCoroutine());
		}
		else
		{
			if (rerollChestRequirement == null || rerollChestRequirement.RequirementType != RequirementType.PayItem)
			{
				return;
			}
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, rerollChestRequirement.NameId, out data))
			{
				CoinBarController controllerForResourceBar = DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.GetControllerForResourceBar(data.ItemBalancing.NameId);
				if (!(controllerForResourceBar == null))
				{
					controllerForResourceBar.SetReEnterAction(RegisterEventHandlers);
					controllerForResourceBar.SwitchToShop();
				}
			}
		}
	}

	private void ConfirmRewardButtonClicked()
	{
		DeRegisterEventHandlers();
		if (!DIContainerLogic.EventSystemService.ConfirmEliteChestReward(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			DebugLog.Error(GetType(), "ConfirmRewardButtonClicked: Could not confirm Elite Chest Reward!!!");
		}
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		m_isLeaving = true;
		m_contentPreviewGridRoot.gameObject.PlayAnimationOrAnimatorState("Leave");
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_EliteChestUnlock_Step2_Leave"));
		SetDragControllerActive(true);
		DIContainerInfrastructure.LocationStateMgr.WorldMenuUI.Enter();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.EnterLevelDisplay();
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr())
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
		}
		base.gameObject.SetActive(false);
		m_IsShowing = false;
	}

	private void InitializeContentPreview()
	{
		List<IInventoryItemGameData> availableEliteChestReward = DIContainerLogic.EventSystemService.GetAvailableEliteChestReward(DIContainerInfrastructure.GetCurrentPlayer());
		m_contentPreviewGridRoot.InitializeItems(availableEliteChestReward);
	}

	private void OnDestroy()
	{
		DeRegisterEventHandlers();
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr())
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(2u);
		}
	}
}
