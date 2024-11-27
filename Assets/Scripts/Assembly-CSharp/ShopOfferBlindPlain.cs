using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class ShopOfferBlindPlain : ShopOfferBlindBase
{
	[SerializeField]
	private LootDisplayContoller m_lootDisplay;

	[SerializeField]
	private UISprite m_spriteDisplay;

	[SerializeField]
	private UILabel m_youHaveText;

	[SerializeField]
	private UILabel m_lockedDescription;

	[SerializeField]
	private UILabel m_amountLabel;

	[SerializeField]
	[Header("Background")]
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
		SetAmountLabel(m_amountLabel, null);
		SetDescriptionLabels(m_youHaveText, m_lockedDescription);
		if (m_items.Count > 1)
		{
			GetComponent<Animator>().SetBool("IsBundleItem", true);
			SetupBundleGrid(m_previewGrid);
		}
		SetOfferIcon(m_spriteDisplay, m_lootDisplay);
		SetupCostBlind(null);
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

	private void SetParameters()
	{
		Animator component = GetComponent<Animator>();
		if (!(component == null))
		{
			component.SetBool("Obtained", m_isPurchased);
			component.SetBool("Locked", m_lockedBird);
			component.SetBool("IsClassItem", m_isClassItem);
			component.SetBool("IsClassSkinItem", m_isSkinItem);
		}
	}
}
