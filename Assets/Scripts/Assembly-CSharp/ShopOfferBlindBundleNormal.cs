using ABH.Shared.BalancingData;
using UnityEngine;

public class ShopOfferBlindBundleNormal : ShopOfferBlindBundleBase
{
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
	private UILabel m_subItemsLabelA;

	[SerializeField]
	private UILabel m_subItemsLabelB;

	[SerializeField]
	private UILabel m_subItemsLabelC;

	public override void SetModel(BasicShopOfferBalancingData model, ShopWindowStateMgr stateMgr)
	{
		base.SetModel(model, stateMgr);
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
			m_genericBackDesc.text = DIContainerInfrastructure.GetLocaService().GetShopOfferDesc(model.LocaId);
		}
		if (m_items.Count > 3)
		{
			m_subItemsLabelC.text = m_items[3].ItemLocalizedName;
		}
		if (m_items.Count > 2)
		{
			m_subItemsLabelB.text = m_items[2].ItemLocalizedName;
		}
		m_subItemsLabelA.text = m_items[1].ItemLocalizedName;
	}
}
