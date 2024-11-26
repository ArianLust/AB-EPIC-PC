using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.Generic;
using SmoothMoves;
using UnityEngine;

public class PopupMissingResourcesStateMgr : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_BuyButton;

	[SerializeField]
	private UIInputTrigger m_AbortButton;

	[SerializeField]
	private SoundTriggerList m_SoundTriggers;

	[SerializeField]
	private ResourceCostBlind m_ResourceCost;

	private List<IInventoryItemGameData> m_Items = new List<IInventoryItemGameData>();

	[SerializeField]
	private float m_MaximumShowTime = 4.5f;

	[SerializeField]
	private UIGrid m_Grid;

	[SerializeField]
	private List<LootDisplayContoller> m_LootDisplays = new List<LootDisplayContoller>();

	private List<BasicShopOfferBalancingData> m_SummedInstantOffers = new List<BasicShopOfferBalancingData>();

	private WaitTimeOrAbort m_AsyncOperation;

	public bool m_IsShowing;

	private bool m_showLuckyCoins;

	private Action m_returnAction;

	[SerializeField]
	private BoneAnimation m_BoneAnimation;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_MissingResourcesPopup = this;
	}

	public PopupMissingResourcesStateMgr SetReturnAction(Action returnAction)
	{
		m_returnAction = returnAction;
		return this;
	}

	public WaitTimeOrAbort ShowMissingResourcesPopup(List<IInventoryItemGameData> items)
	{
		m_IsShowing = true;
		if (items == null || items.Count <= 0)
		{
			m_IsShowing = false;
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
			m_AsyncOperation.Abort();
			return m_AsyncOperation;
		}
		base.gameObject.SetActive(true);
		m_Items = items;
		int num = 0;
		for (num = 0; num < Mathf.Min(m_Items.Count, m_LootDisplays.Count); num++)
		{
			m_LootDisplays[num].gameObject.SetActive(true);
			m_LootDisplays[num].SetModel(items[m_Items.Count - (num + 1)], new List<IInventoryItemGameData>(), LootDisplayType.None);
		}
		for (int i = num; i < m_LootDisplays.Count; i++)
		{
			m_LootDisplays[i].gameObject.SetActive(false);
		}
		m_SummedInstantOffers.Clear();
		m_Grid.transform.localPosition = new Vector3((float)m_Items.Count * ((0f - m_Grid.cellWidth) / 2f), m_Grid.transform.localPosition.y, m_Grid.transform.localPosition.z);
		m_Grid.Reposition();
		List<BasicShopOfferBalancingData> shopOffers = DIContainerLogic.GetShopService().GetShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), "shop_global_instant_buy_items");
		foreach (IInventoryItemGameData item in m_Items)
		{
			foreach (BasicShopOfferBalancingData item2 in shopOffers)
			{
				if (item2.OfferContents.ContainsKey(item.ItemBalancing.NameId))
				{
					for (int j = 0; j < item.ItemValue; j++)
					{
						m_SummedInstantOffers.Add(item2);
					}
				}
			}
		}
		List<Requirement> summedBuyRequirements = DIContainerLogic.GetShopService().GetSummedBuyRequirements(DIContainerInfrastructure.GetCurrentPlayer(), m_SummedInstantOffers);
		Requirement requirement = summedBuyRequirements.FirstOrDefault();
		if (requirement != null)
		{
			IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(requirement.NameId);
			m_ResourceCost.SetModel(balancingData.AssetBaseId, null, requirement.Value, string.Empty);
			m_showLuckyCoins = balancingData.NameId == "lucky_coin";
		}
		StartCoroutine("EnterCoroutine");
		m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		return m_AsyncOperation;
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_missing_resources_enter");
		SetDragControllerActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 6u,
			showFriendshipEssence = false,
			showLuckyCoins = m_showLuckyCoins,
			showSnoutlings = !m_showLuckyCoins
		}, true);
		GetComponent<Animation>().Play("Popup_MissingResources_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_MissingResources_Leave"].length);
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_missing_resources_enter");
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, AbortButtonClicked);
		m_AbortButton.Clicked += AbortButtonClicked;
		m_BuyButton.Clicked += BuyButtonClicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_AbortButton.Clicked -= AbortButtonClicked;
		m_BuyButton.Clicked -= BuyButtonClicked;
	}

	private IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_missing_resources_leave");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(6u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		GetComponent<Animation>().Play("Popup_MissingResources_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_MissingResources_Leave"].length);
		m_IsShowing = false;
		m_AsyncOperation.Abort();
		m_AsyncOperation = null;
		m_SummedInstantOffers.Clear();
		m_Items.Clear();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_missing_resources_leave");
		if (m_returnAction != null)
		{
			m_returnAction();
		}
		base.gameObject.SetActive(false);
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 1);
		}
	}

	private void AbortButtonClicked()
	{
		StartCoroutine("LeaveCoroutine");
	}

	private void BuyButtonClicked()
	{
		List<Requirement> failed = new List<Requirement>();
		if (DIContainerLogic.GetShopService().AreOffersBuyable(DIContainerInfrastructure.GetCurrentPlayer(), m_SummedInstantOffers, out failed))
		{
			if (DIContainerLogic.GetShopService().BuyShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), m_SummedInstantOffers).Count <= 0)
			{
				DebugLog.Error("Failed to buy Offer!");
				if ((bool)m_SoundTriggers)
				{
					m_SoundTriggers.OnTriggerEventFired("purchase_failed");
				}
			}
			HandleOfferBought();
		}
		else
		{
			IInventoryItemGameData data = null;
			Requirement requirement = failed.FirstOrDefault();
			if (requirement != null && DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, requirement.NameId, out data))
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
		StartCoroutine("LeaveCoroutine");
	}

	private void HandleOfferBought()
	{
		if ((bool)m_SoundTriggers)
		{
			m_SoundTriggers.OnTriggerEventFired("purchase_successful");
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateFriendshipEssenceBar();
	}
}
