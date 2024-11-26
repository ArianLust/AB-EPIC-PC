using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class BannerItemInfo : MonoBehaviour
{
	[SerializeField]
	[Header("Skill Footer")]
	private UISprite m_SkillSprite;

	[SerializeField]
	private UILabel m_SkillName;

	[SerializeField]
	private GameObject m_SkillRoot;

	[SerializeField]
	[Header("Perk Footer")]
	private UISprite m_PerkSprite;

	[SerializeField]
	private UILabel m_PerkName;

	[SerializeField]
	private GameObject m_PerkRoot;

	[Header("Set Banner Footer")]
	[SerializeField]
	private UISprite m_PerkSetSprite;

	[SerializeField]
	private UILabel m_PerkSetName;

	[SerializeField]
	private UISprite m_SetSprite;

	[SerializeField]
	private UILabel m_SetName;

	[SerializeField]
	private GameObject m_SetRoot;

	[Header("Elite Emblem Footer")]
	[SerializeField]
	private GameObject m_emblemSetRoot;

	[SerializeField]
	private UISprite m_emblemSkillSprite;

	[SerializeField]
	private UILabel m_emblemSkillLabel;

	[SerializeField]
	private UISprite m_eliteSkillSprite;

	[SerializeField]
	private UILabel m_eliteSkillLabel;

	[Header("Generic")]
	[SerializeField]
	private StatisticsElement m_RegularStats;

	[SerializeField]
	private UILabel m_BannerItemName;

	public BannerGameData m_SelectedBanner;

	public BannerItemGameData m_SelectedBannerItem;

	private bool HasInitialized;

	private bool HasStarted;

	private SkillGameData m_BannerSkill;

	public void ShowSkillTooltip()
	{
		if (m_BannerSkill != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowSkillOverlay(m_SkillSprite.cachedTransform, m_SelectedBanner, m_BannerSkill, true);
		}
	}

	public void ShowSkillAndSetTooltip()
	{
		if (m_SelectedBannerItem != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(m_SkillSprite.cachedTransform, m_SelectedBannerItem, true, true);
		}
	}

	public void ShowEquipmentTooltip()
	{
		if (m_SelectedBannerItem != null && (bool)m_PerkSprite)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(base.transform, m_SelectedBannerItem, true, true);
		}
	}

	public void SetModel(BannerGameData bannerGameData, BannerItemGameData bannerItemGameData)
	{
		m_BannerSkill = bannerItemGameData.PrimarySkill;
		if (m_BannerSkill != null)
		{
			if (bannerItemGameData.IsSetItem && bannerItemGameData.BalancingData.ItemType != InventoryItemType.BannerEmblem)
			{
				m_SkillRoot.SetActive(false);
				m_PerkRoot.SetActive(false);
				m_SetRoot.SetActive(true);
				m_emblemSetRoot.SetActive(false);
				UIAtlas skillAtlas = GetSkillAtlas();
				if ((bool)skillAtlas)
				{
					m_PerkSetSprite.atlas = skillAtlas;
					m_SetSprite.atlas = skillAtlas;
				}
				m_RegularStats.SetValueLabel(bannerItemGameData.ItemMainStat);
				m_PerkSetSprite.spriteName = BannerItemGameData.GetPerkIconNameByPerk(bannerItemGameData.GetPerkTypeOfSkill());
				m_SetSprite.spriteName = BannerItemGameData.GetPerkIconNameByPerk(bannerItemGameData.SetItemSkill.GetPerkType());
				m_PerkSetName.text = m_BannerSkill.SkillLocalizedName;
				m_SetName.text = bannerItemGameData.SetItemSkill.SkillLocalizedName;
			}
			else if (bannerItemGameData.IsSetItem)
			{
				m_SkillRoot.SetActive(false);
				m_PerkRoot.SetActive(false);
				m_SetRoot.SetActive(false);
				m_emblemSetRoot.SetActive(true);
				UIAtlas skillAtlas2 = GetSkillAtlas();
				if ((bool)skillAtlas2)
				{
					m_emblemSkillSprite.atlas = skillAtlas2;
				}
				m_RegularStats.SetValueLabel(bannerItemGameData.ItemMainStat);
				m_emblemSkillSprite.spriteName = m_BannerSkill.m_SkillIconName;
				m_eliteSkillSprite.spriteName = BannerItemGameData.GetPerkIconNameByPerk(bannerItemGameData.SetItemSkill.GetPerkType());
				m_emblemSkillLabel.text = m_BannerSkill.SkillLocalizedName;
				m_eliteSkillLabel.text = bannerItemGameData.SetItemSkill.SkillLocalizedName;
			}
			else if (bannerItemGameData.HasPerkSkill())
			{
				m_SkillRoot.SetActive(false);
				m_PerkRoot.SetActive(true);
				m_SetRoot.SetActive(false);
				m_emblemSetRoot.SetActive(false);
				UIAtlas skillAtlas3 = GetSkillAtlas();
				if ((bool)skillAtlas3)
				{
					m_PerkSprite.atlas = skillAtlas3;
				}
				m_RegularStats.SetValueLabel(bannerItemGameData.ItemMainStat);
				m_PerkSprite.spriteName = BannerItemGameData.GetPerkIconNameByPerk(bannerItemGameData.GetPerkTypeOfSkill());
				m_PerkName.text = m_BannerSkill.SkillLocalizedName;
			}
			else
			{
				m_SkillRoot.SetActive(true);
				m_PerkRoot.SetActive(false);
				m_SetRoot.SetActive(false);
				m_emblemSetRoot.SetActive(false);
				m_RegularStats.SetValueLabel(bannerItemGameData.ItemMainStat);
				UIAtlas skillAtlas4 = GetSkillAtlas();
				if ((bool)skillAtlas4)
				{
					m_SkillSprite.atlas = skillAtlas4;
				}
				m_SkillSprite.spriteName = m_BannerSkill.m_SkillIconName;
				m_SkillName.text = m_BannerSkill.SkillLocalizedName;
			}
		}
		else
		{
			m_SkillRoot.SetActive(false);
		}
		m_SelectedBanner = bannerGameData;
		m_SelectedBannerItem = bannerItemGameData;
		if ((bool)m_BannerItemName)
		{
			m_BannerItemName.text = m_SelectedBannerItem.ItemLocalizedName;
		}
	}

	private UIAtlas GetSkillAtlas()
	{
		UIAtlas result = null;
		if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(m_BannerSkill.Balancing.IconAtlasId))
		{
			GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(m_BannerSkill.Balancing.IconAtlasId) as GameObject;
			result = gameObject.GetComponent<UIAtlas>();
		}
		return result;
	}
}
