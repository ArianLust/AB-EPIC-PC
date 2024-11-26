using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.Shared.BalancingData;
using SmoothMoves;
using UnityEngine;

public class PopupSpecialGachaStateMgr : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_AbortButton;

	[SerializeField]
	private float m_MaximumShowTime = 4.5f;

	private List<BasicShopOfferBalancingData> m_Offers = new List<BasicShopOfferBalancingData>();

	[SerializeField]
	private ShopOfferBlindBase m_OfferBlind;

	private WaitTimeOrAbort m_AsyncOperation;

	public bool m_IsShowing;

	private Action m_returnAction;

	[SerializeField]
	private BoneAnimation m_BoneAnimation;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_SpecialGachaPopup = this;
	}

	public PopupSpecialGachaStateMgr SetReturnAction(Action returnAction)
	{
		m_returnAction = returnAction;
		return this;
	}

	public WaitTimeOrAbort ShowSpecialGachaPopup()
	{
		m_IsShowing = true;
		m_Offers = DIContainerLogic.GetShopService().GetShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), "shop_premium_gacha_shortcut");
		if (m_Offers == null || m_Offers.Count == 0)
		{
			m_IsShowing = false;
			m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
			m_AsyncOperation.Abort();
			return m_AsyncOperation;
		}
		base.gameObject.SetActive(true);
		StartCoroutine("EnterCoroutine");
		m_OfferBlind.SetModel(m_Offers.FirstOrDefault(), null);
		m_AsyncOperation = new WaitTimeOrAbort(m_MaximumShowTime);
		return m_AsyncOperation;
	}

	private void OnShopOfferBought(BasicShopOfferBalancingData offer)
	{
		DeRegisterEventHandlers();
		StartCoroutine(DelayedLeaveCoroutine(1f));
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_gacha_special_anim");
		SetDragControllerActive(false);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 5u,
			showFriendshipEssence = false,
			showLuckyCoins = true,
			showSnoutlings = false
		}, true);
		GetComponent<Animation>().Play("Popup_SpecialGachaOffer_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_SpecialGachaOffer_Leave"].length);
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_gacha_special_anim");
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, AbortButtonClicked);
		m_AbortButton.Clicked += AbortButtonClicked;
		m_OfferBlind.ShopOfferBought += OnShopOfferBought;
		m_OfferBlind.RegisterEventHandlers();
		m_OfferBlind.RegisterExternalEventHandlers();
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_AbortButton.Clicked -= AbortButtonClicked;
		m_OfferBlind.ShopOfferBought -= OnShopOfferBought;
		m_OfferBlind.DeRegisterEventHandlers();
		m_OfferBlind.DeRegisterExternalEventHandlers();
	}

	private IEnumerator DelayedLeaveCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		StartCoroutine(LeaveCoroutine());
	}

	public IEnumerator LeaveCoroutine()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_gacha_special_anim");
		SetDragControllerActive(true);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(5u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		GetComponent<Animation>().Play("Popup_SpecialGachaOffer_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["Popup_SpecialGachaOffer_Leave"].length);
		m_IsShowing = false;
		if (m_AsyncOperation != null)
		{
			m_AsyncOperation.Abort();
			m_AsyncOperation = null;
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_gacha_special_anim");
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
		DeRegisterEventHandlers();
		StartCoroutine("LeaveCoroutine");
	}
}
