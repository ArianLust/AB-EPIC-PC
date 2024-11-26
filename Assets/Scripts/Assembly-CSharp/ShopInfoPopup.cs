using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class ShopInfoPopup : MonoBehaviour
{
	[SerializeField]
	private UILabel m_HeaderLabel;

	[SerializeField]
	private UILabel m_SubHeaderLabel;

	[SerializeField]
	private UILabel m_DescriptionLabel;

	[SerializeField]
	private UIInputTrigger m_CloseButton;

	[SerializeField]
	private Transform m_CharacterSlot;

	[SerializeField]
	private Transform m_SpriteSlot;

	[SerializeField]
	private UILabel m_ObtainText;

	[SerializeField]
	private List<LootDisplayContoller> m_LootDisplays;

	[SerializeField]
	private GameObject m_TextRoot;

	[SerializeField]
	private GameObject m_ItemlistRoot;

	private string m_categoryText;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_ShopInfoPopup = this;
	}

	public void Enter(BasicShopOfferBalancingData offer, GameObject icon)
	{
		SetupContent(offer);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("EnterShopInfo");
		GetComponent<Animation>().Play("Popup_Enter");
		SetupIcon(icon);
		m_SubHeaderLabel.text = m_categoryText;
		Invoke("EnterFinish", 0.6f);
	}

	private void EnterFinish()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("EnterShopInfo");
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(8, ClosePopup);
		m_CloseButton.Clicked -= ClosePopup;
		m_CloseButton.Clicked += ClosePopup;
	}

	private void SetupContent(BasicShopOfferBalancingData offer)
	{
		List<IInventoryItemGameData> shopOfferContent = DIContainerLogic.GetShopService().GetShopOfferContent(DIContainerInfrastructure.GetCurrentPlayer(), offer, DIContainerLogic.GetSalesManagerService().GetOfferSaleDetails(offer.NameId));
		m_HeaderLabel.text = DIContainerInfrastructure.GetLocaService().GetShopOfferName(offer.LocaId);
		m_TextRoot.SetActive(true);
		m_ItemlistRoot.SetActive(false);
		if (shopOfferContent.Count > 1)
		{
			m_TextRoot.SetActive(false);
			m_ItemlistRoot.SetActive(true);
			m_ObtainText.text = DIContainerInfrastructure.GetLocaService().Tr("tt_bundle_contains");
			SetItemListContent(shopOfferContent);
		}
		else if (shopOfferContent == null || shopOfferContent.Count == 0)
		{
			m_DescriptionLabel.text = DIContainerInfrastructure.GetLocaService().Tr(offer.LocaId + "_desc");
		}
		else if (shopOfferContent[0] is MasteryItemGameData)
		{
			MasteryItemGameData masteryItemGameData = shopOfferContent[0] as MasteryItemGameData;
			m_DescriptionLabel.text = shopOfferContent[0].ItemLocalizedTooltipDesc(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData).Replace("{value_2}", masteryItemGameData.GetMasteryClassName(DIContainerInfrastructure.GetCurrentPlayer()));
		}
		else
		{
			m_DescriptionLabel.text = shopOfferContent[0].ItemLocalizedTooltipDesc(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData);
		}
	}

	private void SetItemListContent(List<IInventoryItemGameData> items)
	{
		List<IInventoryItemGameData> list = new List<IInventoryItemGameData>();
		foreach (IInventoryItemGameData item in items)
		{
			if (!(item.Name == "unlock_skin"))
			{
				if (item is SkinItemGameData || item is ClassItemGameData)
				{
					list.Insert(0, item);
				}
				else
				{
					list.Add(item);
				}
			}
		}
		for (int i = 0; i < m_LootDisplays.Count; i++)
		{
			if (list.Count > i)
			{
				m_LootDisplays[i].gameObject.SetActive(true);
				m_LootDisplays[i].SetModel(list[i], new List<IInventoryItemGameData>(), LootDisplayType.None, "_Small");
			}
			else
			{
				m_LootDisplays[i].gameObject.SetActive(false);
			}
		}
	}

	private void SetupIcon(GameObject icon)
	{
		if (!(icon == null))
		{
			if (m_CharacterSlot.childCount > 0)
			{
				Object.Destroy(m_CharacterSlot.GetChild(0).gameObject);
			}
			if (m_SpriteSlot.childCount > 0)
			{
				Object.Destroy(m_SpriteSlot.GetChild(0).gameObject);
			}
			GameObject gameObject = Object.Instantiate(icon);
			if (icon.GetComponent<UISprite>() != null)
			{
				gameObject.transform.parent = m_SpriteSlot;
				gameObject.GetComponentInChildren<UISprite>().MakePixelPerfect();
			}
			else
			{
				gameObject.transform.parent = m_CharacterSlot;
				gameObject.transform.localScale = Vector3.one;
			}
			gameObject.transform.localPosition = icon.transform.localPosition;
		}
	}

	private void ClosePopup()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("LeaveShopInfo");
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(8);
		m_CloseButton.Clicked -= ClosePopup;
		GetComponent<Animation>().Play("Popup_Leave");
		Invoke("LeaveFinish", 0.6f);
		if (m_CharacterSlot.childCount > 0)
		{
			Object.Destroy(m_CharacterSlot.GetChild(0).gameObject);
		}
		if (m_SpriteSlot.childCount > 0)
		{
			Object.Destroy(m_SpriteSlot.GetChild(0).gameObject);
		}
	}

	private void LeaveFinish()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("LeaveShopInfo");
		base.gameObject.SetActive(false);
	}

	public void SetSubHeaderText(string text)
	{
		m_categoryText = text;
	}
}
