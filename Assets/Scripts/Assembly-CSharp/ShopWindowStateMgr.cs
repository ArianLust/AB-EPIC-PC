using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class ShopWindowStateMgr : MonoBehaviour
{
	private string m_CurrentOpenCategory = string.Empty;

	[SerializeField]
	[Header("Header")]
	private HeaderBanner m_normalHeader;

	[SerializeField]
	private HeaderBanner m_saleHeader;

	[SerializeField]
	private HeaderBanner m_bundleHeader;

	[SerializeField]
	[Header("Category Buttons")]
	private List<ShopCategoryButton> m_CategoryButtonList;

	[SerializeField]
	private Transform m_activeButtonMarker;

	[SerializeField]
	[Header("Animations")]
	private Animation m_backgroundAnimation;

	[SerializeField]
	private Animation m_footerAnimation;

	[SerializeField]
	private Animation m_offerRootAnimation;

	[SerializeField]
	private Animation m_headerAnimation;

	[Header("Prefabs")]
	[SerializeField]
	private ShopOfferBlindPlain m_NormalOfferPrefab;

	[SerializeField]
	private ShopOfferBlindSale m_SaleOfferPrefab;

	[SerializeField]
	private ShopOfferBlindBundleNormal m_NormalBundleOfferPrefab;

	[SerializeField]
	private ShopOfferBlindBundleSet m_SetBundleOfferPrefab;

	[Header("Grid")]
	[SerializeField]
	private UIScrollView m_panel;

	[SerializeField]
	private UITable m_offerGrid;

	[SerializeField]
	private float m_xPositionShopOffers;

	[SerializeField]
	private float m_gridWidthShopOffers;

	[Header("Misc")]
	[SerializeField]
	private GameObject m_EmptyLabelRoot;

	[SerializeField]
	public UIInputTrigger m_BackButton;

	private Action m_ReEnterAction;

	private Vector3 m_InitialPosition;

	private bool m_movingToIndex;

	private bool m_leaving;

	private bool m_saleHeaderActivated;

	private SalesManagerBalancingData m_activeSale;

	[HideInInspector]
	public string m_Entersource;

	private int m_startIndex;

	public void SetStartScrollIndex(int index)
	{
		m_startIndex = index;
	}

	private void Awake()
	{
		if (DIContainerInfrastructure.PurchasingService.IsSupported() && !DIContainerInfrastructure.PurchasingService.IsInitializing() && !DIContainerInfrastructure.PurchasingService.IsInitialized() && !string.IsNullOrEmpty(DIContainerConfig.GetClientConfig().BundleId))
		{
			DIContainerInfrastructure.PurchasingService.Initialize(DIContainerConfig.GetClientConfig().BundleId);
		}
	}

	private void HandleBackButton()
	{
		DebugLog.Log("Pressed Back Button: " + GetType());
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("back_button_pressed", string.Empty);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
		m_BackButton_Clicked();
	}

	public ShopWindowStateMgr SetCategory(string category, bool refresh = true)
	{
		if (!refresh)
		{
			SetupCategoryButtons();
			CheckForActiveSale();
		}
		category = ((!string.IsNullOrEmpty(category)) ? GetMappedCategory(category) : "shop_premium");
		if (m_CurrentOpenCategory == category)
		{
			refresh = false;
		}
		m_CurrentOpenCategory = category;
		StartCoroutine(SetCategoryCoroutine(refresh));
		if (!m_leaving)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
			{
				Depth = 10u,
				showFriendshipEssence = true,
				showLuckyCoins = true,
				showSnoutlings = true
			}, true);
		}
		return this;
	}

	private void CheckForActiveSale()
	{
		m_activeSale = DIContainerLogic.GetSalesManagerService().GetAllActiveSales(true).FirstOrDefault((SalesManagerBalancingData sale) => sale.ContentType != SaleContentType.RainbowRiot && sale.ContentType != SaleContentType.Mastery);
	}

	private IEnumerator SetCategoryCoroutine(bool finalRefresh)
	{
		StartCoroutine(ChoseCorrectHeader());
		StartCoroutine(PlaceMarker());
		if (finalRefresh)
		{
			yield return StartCoroutine(RefreshCurrentCategory());
		}
		StartCoroutine(MoveToIndex());
	}

	private IEnumerator PlaceMarker()
	{
		Animation markerAnim = m_activeButtonMarker.GetComponent<Animation>();
		foreach (ShopCategoryButton button in m_CategoryButtonList)
		{
			if (button.m_CategoryName == m_CurrentOpenCategory)
			{
				markerAnim.Play("Hide");
				yield return new WaitForSeconds(markerAnim["Hide"].length);
				m_activeButtonMarker.position = button.transform.position;
				markerAnim.Play("Show");
				markerAnim.PlayQueued("Loop");
				break;
			}
		}
	}

	private void SetupCategoryButtons()
	{
		for (int i = 0; i < m_CategoryButtonList.Count; i++)
		{
			ShopCategoryButton button = m_CategoryButtonList[i];
			ShopBalancingData balancing = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(button.m_CategoryName, out balancing))
			{
				List<BasicShopOfferBalancingData> list = new List<BasicShopOfferBalancingData>();
				if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(button.m_CategoryName, out balancing))
				{
					list = DIContainerLogic.GetShopService().GetShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), button.m_CategoryName);
				}
				bool active = DIContainerLogic.GetSalesManagerService().ActiveSales.Exists((SalesManagerBalancingData sale) => GetMappedCategory(sale.CheckoutCategory) == button.m_CategoryName);
				button.m_SaleMarker.SetActive(active);
				button.m_UpdateMarker.SetActive(false);
			}
		}
	}

	private IEnumerator ChoseCorrectHeader()
	{
		if (m_activeSale != null && m_activeSale.IsAnyBundle)
		{
			m_saleHeaderActivated = true;
			m_bundleHeader.m_ParentObject.SetActive(true);
			m_saleHeader.m_ParentObject.SetActive(false);
			m_normalHeader.m_ParentObject.SetActive(false);
			SetupHeader(m_bundleHeader);
		}
		else if (m_activeSale != null && (m_activeSale.ContentType == SaleContentType.ShopItems || m_activeSale.ContentType == SaleContentType.LuckyCoinDiscount))
		{
			m_saleHeaderActivated = true;
			m_bundleHeader.m_ParentObject.SetActive(false);
			m_saleHeader.m_ParentObject.SetActive(true);
			m_normalHeader.m_ParentObject.SetActive(false);
			SetupHeader(m_saleHeader);
		}
		else
		{
			m_bundleHeader.m_ParentObject.SetActive(false);
			m_saleHeader.m_ParentObject.SetActive(false);
			m_normalHeader.m_ParentObject.SetActive(true);
			SetupHeader(m_normalHeader);
		}
		if (!m_saleHeaderActivated)
		{
			m_headerAnimation.Play("Header_Change_Out");
			yield return new WaitForSeconds(m_headerAnimation["Header_Change_Out"].length);
			m_headerAnimation.Play("Header_Change_In");
		}
	}

	private void SetupHeader(HeaderBanner header)
	{
		if (m_activeSale != null)
		{
			header.m_Header.text = DIContainerInfrastructure.GetLocaService().Tr(m_activeSale.LocaBaseId + "_link");
			header.m_CheckoutButton.gameObject.SetActive(m_CurrentOpenCategory != GetMappedCategory(m_activeSale.CheckoutCategory));
		}
		else
		{
			header.m_Header.text = DIContainerInfrastructure.GetLocaService().Tr("camp_shop");
		}
		if (header.m_CheckoutButton != null)
		{
			header.m_CheckoutButton.Clicked -= CheckOutSale;
			header.m_CheckoutButton.Clicked += CheckOutSale;
		}
	}

	private string GetMappedCategory(string offerCategory)
	{
		if (offerCategory == "shop_global_premium")
		{
			return "shop_premium";
		}
		if (offerCategory == "global_shop_01_potions")
		{
			return "shop_global_consumables";
		}
		return offerCategory;
	}

	private void CheckOutSale()
	{
		SetCategory(GetMappedCategory(m_activeSale.CheckoutCategory));
	}

	private IEnumerator MoveToIndex()
	{
		if (!m_movingToIndex)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.BlockShopLinks(true);
			m_movingToIndex = true;
			 if (m_startIndex == 0)
			 {
				yield return StartCoroutine(RestorePosition());
			 }
			 else
			 {
			 	m_panel.ResetPosition();
			 	m_offerGrid.Reposition();
			 	// yield return StartCoroutine(RestorePosition());
			 	// yield return new WaitForSeconds(2f);
			 	m_panel.MoveAbsolute(new Vector3((float)(-m_startIndex) * m_gridWidthShopOffers, 0f, 0f));
			 }
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.BlockShopLinks(false);
			m_movingToIndex = false;
		}
	}

	private IEnumerator RefreshCurrentCategory()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.BlockShopLinks(true);
		yield return new WaitForSeconds(PlayCategoryChangedAnimation(false));
		StopCoroutine("RefreshCurrentCategoryContent");
		yield return StartCoroutine("RefreshCurrentCategoryContent");
		yield return new WaitForSeconds(PlayCategoryChangedAnimation(true));
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.BlockShopLinks(false);
	}

	private IEnumerator RestorePosition()
	{
		m_panel.DisableSpring();
		m_panel.ResetPosition();
		yield return new WaitForEndOfFrame();
		m_offerGrid.Reposition();
		yield return new WaitForEndOfFrame();
		m_panel.RestrictWithinBounds(true);
	}

	private IEnumerator RefreshCurrentCategoryContent()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.BlockShopLinks(true);
		foreach (Transform oldBlind in m_offerGrid.transform)
		{
			UnityEngine.Object.Destroy(oldBlind.gameObject);
		}
		yield return new WaitForEndOfFrame();
		if ((bool)m_EmptyLabelRoot)
		{
			m_EmptyLabelRoot.SetActive(false);
		}
		SetContent();
		m_offerGrid.Reposition();
		if (!string.IsNullOrEmpty(m_CurrentOpenCategory) && m_startIndex == 0)
		{
			yield return StartCoroutine(RestorePosition());
		}
		else
		{
			yield return StartCoroutine(RestorePosition());
			m_panel.MoveAbsolute(new Vector3((float)(-m_startIndex) * m_gridWidthShopOffers, 0f, 0f));
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.BlockShopLinks(false);
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enter_shop", m_CurrentOpenCategory);
	}

	private void SetContent()
	{
		List<BasicShopOfferBalancingData> list = new List<BasicShopOfferBalancingData>();
		ShopBalancingData balancing = null;
		if (!DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(m_CurrentOpenCategory, out balancing))
		{
			return;
		}
		List<BasicShopOfferBalancingData> shopOffers = DIContainerLogic.GetShopService().GetShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), m_CurrentOpenCategory, true, true);
		for (int i = 0; i < shopOffers.Count; i++)
		{
			list.Add(shopOffers[i]);
		}
		if (list.Count == 0)
		{
			if ((bool)m_EmptyLabelRoot)
			{
				m_EmptyLabelRoot.SetActive(true);
			}
			return;
		}
		foreach (BasicShopOfferBalancingData item in list)
		{
			ShopOfferBlindBase shopOfferBlindBase = InstantiateOfferBlind(item);
			shopOfferBlindBase.transform.parent = m_offerGrid.transform;
			shopOfferBlindBase.transform.localPosition = Vector3.zero;
			shopOfferBlindBase.SetModel(item, this);
		}
	}

	private ShopOfferBlindBase InstantiateOfferBlind(BasicShopOfferBalancingData offer)
	{
		ShopOfferBlindBase shopOfferBlindBase = null;
		if (offer.OfferContents.Count >= 2)
		{
			shopOfferBlindBase = ((!DIContainerLogic.GetShopService().IsSetBundle(offer)) ? ((ShopOfferBlindBundleBase)UnityEngine.Object.Instantiate(m_NormalBundleOfferPrefab)) : ((ShopOfferBlindBundleBase)UnityEngine.Object.Instantiate(m_SetBundleOfferPrefab)));
			shopOfferBlindBase.gameObject.name = "A_" + offer.SlotId.ToString("00") + "_ShopOffer";
		}
		else if (DIContainerLogic.GetShopService().IsDiscountValid(offer))
		{
			shopOfferBlindBase = UnityEngine.Object.Instantiate(m_SaleOfferPrefab);
			shopOfferBlindBase.gameObject.name = "B_" + offer.SlotId.ToString("00") + "_ShopOffer";
		}
		else
		{
			shopOfferBlindBase = UnityEngine.Object.Instantiate(m_NormalOfferPrefab);
			shopOfferBlindBase.gameObject.name = "C_" + offer.SlotId.ToString("00") + "_ShopOffer";
		}
		return shopOfferBlindBase;
	}

	private float PlayCategoryChangedAnimation(bool moveIn)
	{
		string text = ((!moveIn) ? "Out" : "In");
		m_offerRootAnimation.Play("ShopOffers_" + text);
		return m_offerRootAnimation["ShopOffers_" + text].length;
	}

	public void Enter(string enterSource)
	{
		base.gameObject.SetActive(true);
		m_Entersource = enterSource;
		m_offerRootAnimation.gameObject.SetActive(true);
		if (DIContainerInfrastructure.GetCoreStateMgr().m_SpecialGachaPopup.m_IsShowing)
		{
			StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_SpecialGachaPopup.LeaveCoroutine());
		}
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		yield return new WaitForEndOfFrame();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("shop_enter");
		m_backgroundAnimation.Play("RootWindow_Enter");
		m_offerRootAnimation.Play("ShopOffers_Enter");
		m_footerAnimation.Play("BackButton_Enter");
		m_headerAnimation.Play("Header_Enter");
		yield return StartCoroutine(RefreshCurrentCategoryContent());
		yield return new WaitForSeconds(m_offerRootAnimation["ShopOffers_Enter"].length);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("shop_enter");
		RegisterEventHandler();
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideAllTooltips();
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine(delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}));
	}

	private IEnumerator LeaveCoroutine(Action actionAfterLeave)
	{
		if (!m_leaving)
		{
			m_leaving = true;
			DeRegisterEventHandler();
			DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("shop_leave");
			m_offerRootAnimation.Play("ShopOffers_Leave");
			m_footerAnimation.Play("BackButton_Leave");
			m_backgroundAnimation.Play("RootWindow_Leave");
			m_headerAnimation.Play("Header_Leave");
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(10u);
			yield return new WaitForSeconds(m_offerRootAnimation["ShopOffers_Leave"].length);
			m_leaving = false;
			DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("shop_leave");
			if (actionAfterLeave != null)
			{
				actionAfterLeave();
			}
			if (m_ReEnterAction != null)
			{
				m_ReEnterAction();
			}
		}
	}

	private void m_BackButton_Clicked()
	{
		Leave();
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(5, HandleBackButton);
		m_BackButton.Clicked += m_BackButton_Clicked;
	}

	private void DeRegisterEventHandler()
	{
		if ((bool)DIContainerInfrastructure.BackButtonMgr)
		{
			DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(5);
		}
		if ((bool)m_BackButton)
		{
			m_BackButton.Clicked -= m_BackButton_Clicked;
		}
	}

	private void OnDestroy()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.BlockShopLinks(false);
		DeRegisterEventHandler();
		DIContainerInfrastructure.GetCoreStateMgr().RegisterShopClosed();
	}

	public void HardRefresh()
	{
		StartCoroutine(RefreshCurrentCategory());
	}

	public void SoftRefresh()
	{
		foreach (Transform item in m_offerGrid.transform)
		{
			ShopOfferBlindBase component = item.GetComponent<ShopOfferBlindBase>();
			if (component != null)
			{
				component.SetModel(component.OfferModel, this);
			}
		}
	}

	public ShopWindowStateMgr SetReEnterAction(Action reEnterAction)
	{
		if (reEnterAction != null)
		{
			m_ReEnterAction = reEnterAction;
		}
		return this;
	}
}
