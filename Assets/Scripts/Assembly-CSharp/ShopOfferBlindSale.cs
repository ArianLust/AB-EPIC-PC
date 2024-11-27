using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class ShopOfferBlindSale : ShopOfferBlindBase
{
	[SerializeField]
	private LootDisplayContoller m_LootDisplay;

	[SerializeField]
	private UISprite m_SpriteDisplay;

	[SerializeField]
	private UILabel m_youHaveText;

	[SerializeField]
	private UILabel m_AmountLabel;

	[SerializeField]
	private UILabel m_DiscountAmountOldLabel;

	[SerializeField]
	private UILabel m_OldCostValue;

	[SerializeField]
	private UILabel m_TimerLabel;

	[SerializeField]
	private UILabel m_saleMarkerLabel;

	[Header("Background")]
	[SerializeField]
	private GameObject m_genericBackInfo;

	[SerializeField]
	private GameObject m_classBackInfo;

	[SerializeField]
	private UILabel m_genericBackDesc;

	[SerializeField]
	private SkillBlind m_classSkillPrimary;

	[SerializeField]
	private SkillBlind m_classSkillSecondary;

	[SerializeField]
	private UIGrid m_previewGrid;

	public override void SetModel(BasicShopOfferBalancingData model, ShopWindowStateMgr stateMgr)
	{
		base.SetModel(model, stateMgr);
		SetAmountLabel(m_AmountLabel, m_DiscountAmountOldLabel);
		SetDescriptionLabels(m_youHaveText, null);
		SetSaleSticketLabelText();
		if (m_items.Count > 1)
		{
			SetupBundleGrid(m_previewGrid);
			GetComponent<Animator>().SetBool("IsBundleItem", true);
		}
		SetOfferIcon(m_SpriteDisplay, m_LootDisplay);
		SetupCostBlind(m_OldCostValue);
		StartCoroutine(ShowTimer(m_TimerLabel));
		if (m_isClassItem || m_isSkinItem)
		{
			m_classBackInfo.SetActive(true);
			m_genericBackInfo.SetActive(false);
			GenerateSkillInfo(m_classSkillPrimary, m_classSkillSecondary);
		}
		else
		{
			m_classBackInfo.SetActive(false);
			m_genericBackInfo.SetActive(true);
			ConsumableItemGameData consumableItemGameData = m_item as ConsumableItemGameData;
			m_genericBackDesc.text = ((consumableItemGameData == null) ? DIContainerInfrastructure.GetLocaService().GetShopOfferDesc(model.LocaId) : consumableItemGameData.ItemLocalizedDesc);
		}
		SetParameters();
	}

	private void SetSaleSticketLabelText()
	{
		string text = ((!m_saleModel.IsEmpty()) ? m_saleModel.SaleBalancing.LocaBaseId : string.Empty);
		string text2 = ((!string.IsNullOrEmpty(text)) ? (text + "_" + m_model.LocaId) : string.Empty);
		if (string.IsNullOrEmpty(text))
		{
			m_saleMarkerLabel.text = "%";
			return;
		}
		string text3 = DIContainerInfrastructure.GetLocaService().Tr(text2 + "_sticker", string.Empty);
		string text4 = DIContainerInfrastructure.GetLocaService().Tr(text2 + "_sticker", string.Empty);
		string text5 = ((!string.IsNullOrEmpty(text3)) ? text3 : text4);
		m_saleMarkerLabel.text = ((!string.IsNullOrEmpty(text5)) ? text5 : "%");
	}

	private void SetParameters()
	{
		Animator component = GetComponent<Animator>();
		if (!(component == null))
		{
			component.SetBool("IsClassItem", m_isClassItem);
			component.SetBool("IsClassSkinItem", m_isSkinItem);
			bool value = DIContainerLogic.GetShopService().IsPriceDiscount(base.OfferModel);
			component.SetBool("IsPriceSale", value);
			bool value2 = DIContainerLogic.GetShopService().IsValueDiscount(base.OfferModel);
			component.SetBool("IsAmountSale", value2);
		}
	}
}
