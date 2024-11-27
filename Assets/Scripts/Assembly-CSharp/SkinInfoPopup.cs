using System;
using System.Collections;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class SkinInfoPopup : MonoBehaviour
{
	[SerializeField]
	private UILabel m_ClassTitle;

	[SerializeField]
	private UILabel m_ClassDescription;

	[SerializeField]
	private UILabel m_SkinDescription;

	[SerializeField]
	private UIInputTrigger m_CloseButtonTrigger;

	[SerializeField]
	private UIInputTrigger m_ShopButtonTrigger;

	[SerializeField]
	private GameObject m_ShopButton;

	[SerializeField]
	private Animation m_PopupAnimation;

	[SerializeField]
	private Transform m_HeadgearSlot;

	[SerializeField]
	private UiSortBehaviour m_HeadgearSlotSorting;

	private ClassSkinBalancingData m_upgradeSkin;

	private ClassManagerUi m_classManager;

	private void OnDestroy()
	{
		DeregisterEventHandler();
	}

	private void RegisterEventHandler()
	{
		DeregisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(7, ClosePopup);
		m_CloseButtonTrigger.Clicked += ClosePopup;
		m_ShopButtonTrigger.Clicked += GoToShop;
	}

	private void DeregisterEventHandler()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(7);
		m_CloseButtonTrigger.Clicked -= ClosePopup;
		m_ShopButtonTrigger.Clicked -= GoToShop;
	}

	public void ClosePopup()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(LeaveCoroutine());
		}
	}

	private IEnumerator LeaveCoroutine()
	{
		DeregisterEventHandler();
		m_PopupAnimation.Play("Popup_Leave");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(6u);
		yield return new WaitForSeconds(0.5f);
		base.gameObject.SetActive(false);
	}

	public void Show(ClassItemGameData birdClass, ClassManagerUi classManager)
	{
		base.gameObject.SetActive(true);
		m_classManager = classManager;
		m_upgradeSkin = DIContainerBalancing.Service.GetBalancingDataList<ClassSkinBalancingData>().FirstOrDefault((ClassSkinBalancingData sb) => sb.OriginalClass == birdClass.BalancingData.NameId && sb.SortPriority == 1);
		m_ClassTitle.text = DIContainerInfrastructure.GetLocaService().GetClassName(m_upgradeSkin.LocaBaseId);
		m_ClassDescription.text = birdClass.ItemLocalizedDesc;
		if (IsShopOfferAvailable())
		{
			m_ShopButton.SetActive(true);
			m_SkinDescription.text = DIContainerInfrastructure.GetLocaService().Tr("info_" + m_upgradeSkin.LocaBaseId + "_desc_02");
		}
		else
		{
			m_ShopButton.SetActive(false);
			m_SkinDescription.text = DIContainerInfrastructure.GetLocaService().Tr("info_" + m_upgradeSkin.LocaBaseId + "_desc_01");
		}
		SpawnHeadgear(m_upgradeSkin);
		StartCoroutine(EnterCoroutine());
	}

	private void SpawnHeadgear(ClassSkinBalancingData UpgradeSkin)
	{
		if (m_HeadgearSlot.childCount > 0)
		{
			UnityEngine.Object.Destroy(m_HeadgearSlot.GetChild(0).gameObject);
		}
		GameObject gameObject = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(UpgradeSkin.AssetBaseId, m_HeadgearSlot, Vector3.zero, Quaternion.identity, false);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.gameObject.layer = LayerMask.NameToLayer("Interface");
		gameObject.transform.localScale = Vector3.one;
		if ((bool)m_HeadgearSlotSorting)
		{
			m_HeadgearSlotSorting.ClearAndAutoSearchRenderers();
		}
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("skin_info");
		m_PopupAnimation.Play("Popup_Enter");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 6u,
			showEnergy = false,
			showFriendshipEssence = false,
			showLuckyCoins = false,
			showSnoutlings = false
		}, false);
		yield return new WaitForSeconds(0.5f);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("skin_info");
		RegisterEventHandler();
	}

	private bool IsShopOfferAvailable()
	{
		return DIContainerBalancing.Service.GetBalancingDataList<BuyableShopOfferBalancingData>().Any((BuyableShopOfferBalancingData o) => o.OfferContents != null && o.OfferContents.Keys.Contains(m_upgradeSkin.NameId) && o.Category != "none" && !DIContainerLogic.GetShopService().IsExclusiveOfferHidden(o));
	}

	private void GoToShop()
	{
		ClosePopup();
		BirdWindowUI birdWindowUI = UnityEngine.Object.FindObjectOfType(typeof(BirdWindowUI)) as BirdWindowUI;
		Action reEnterAction = null;
		if (birdWindowUI != null)
		{
			reEnterAction = birdWindowUI.RefreshAll;
		}
		if (m_classManager != null)
		{
			m_classManager.Leave(false);
			reEnterAction = m_classManager.ReEnterFromShop;
		}
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop("shop_global_classes", reEnterAction);
	}
}
