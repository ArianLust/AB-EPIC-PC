using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class EquipmentSetInfoElement : MonoBehaviour
{
	[SerializeField]
	private UISprite m_ArrowSprite;

	[SerializeField]
	private UISprite m_BaseStatType;

	[SerializeField]
	private UILabel m_BaseStatValue;

	[SerializeField]
	private UISprite m_PerkType;

	[SerializeField]
	private GameObject m_NewBody;

	[SerializeField]
	private GameObject m_StandardBody;

	[SerializeField]
	private UILabel m_TimerLabel;

	[SerializeField]
	private Transform m_ItemSpawnRoot;

	[SerializeField]
	private CHMotionTween m_Tween;

	[HideInInspector]
	public EquipmentSetInfoElement m_Partner;

	[HideInInspector]
	public bool m_IsSecondary;

	[HideInInspector]
	public IInventoryItemGameData m_Item;

	private GachaSetItemInfoPopup m_parentWindow;

	private Vector3 m_Position;

	public void Init(IInventoryItemGameData item, EquipmentSetInfoElement partner, GachaSetItemInfoPopup parentWindow)
	{
		m_Partner = partner;
		m_Item = item;
		m_parentWindow = parentWindow;
		bool flag = false;
		if (item.ItemBalancing is EquipmentBalancingData)
		{
			EquipmentBalancingData equipmentBalancingData = item.ItemBalancing as EquipmentBalancingData;
			m_IsSecondary = equipmentBalancingData.ItemType == InventoryItemType.OffHandEquipment;
			flag = equipmentBalancingData.ShowAsNew;
			GameObject gameObject = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(item.ItemAssetName, m_ItemSpawnRoot, Vector3.zero, Quaternion.identity, false);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			SetItemFooter();
		}
		else
		{
			if (!(item.ItemBalancing is BannerItemBalancingData))
			{
				DebugLog.Error("Wrong item type!!  " + item.ItemBalancing.GetType());
				return;
			}
			BannerItemBalancingData bannerItemBalancingData = item.ItemBalancing as BannerItemBalancingData;
			m_IsSecondary = bannerItemBalancingData.ItemType == InventoryItemType.Banner;
			flag = bannerItemBalancingData.FlagAsNew;
			GameObject gameObject2 = DIContainerInfrastructure.GetBannerAssetProvider().InstantiateObject(item.ItemAssetName, m_ItemSpawnRoot, Vector3.zero, Quaternion.identity);
			gameObject2.transform.localScale = Vector3.one;
			gameObject2.transform.localPosition = Vector3.zero;
			SetBannerFooter();
		}
		if (flag)
		{
			m_NewBody.SetActive(true);
			m_StandardBody.SetActive(false);
		}
		else
		{
			m_NewBody.SetActive(false);
			m_StandardBody.SetActive(true);
		}
	}

	public void SetPosition()
	{
		if ((bool)m_Tween)
		{
			m_Position = m_Tween.transform.localPosition;
		}
	}

	public void ShowTooltip()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(base.transform, m_Item, true, false);
	}

	private void OnClick()
	{
		if (!GetComponent<ActionOverlayInvoker>().m_IsTapping)
		{
			m_parentWindow.OnItemSelected(this);
		}
	}

	private void SetItemFooter()
	{
		EquipmentBalancingData equipmentBalancingData = m_Item.ItemBalancing as EquipmentBalancingData;
		float itemMainStat = EquipmentGameData.GetItemMainStat(equipmentBalancingData.BaseStat, equipmentBalancingData.StatPerLevel, m_Item.ItemData.Level, 2, equipmentBalancingData.StatPerQuality, equipmentBalancingData.StatPerQualityPercent, 0);
		float num = 0f;
		BirdGameData bird = DIContainerInfrastructure.GetCurrentPlayer().GetBird(equipmentBalancingData.RestrictedBirdId);
		if (bird != null)
		{
			if (equipmentBalancingData.ItemType == InventoryItemType.MainHandEquipment)
			{
				num = bird.MainHandItem.ItemMainStat;
				m_BaseStatType.spriteName = "Character_Damage_Small";
			}
			else if (equipmentBalancingData.ItemType == InventoryItemType.OffHandEquipment)
			{
				num = bird.OffHandItem.ItemMainStat;
				m_BaseStatType.spriteName = "Character_Health_Small";
			}
		}
		float comparisionFooter = itemMainStat - num;
		SetComparisionFooter(comparisionFooter);
		m_PerkType.spriteName = EquipmentGameData.GetPerkIcon(m_Item as EquipmentGameData);
	}

	private void SetBannerFooter()
	{
		BannerItemBalancingData bannerItemBalancingData = m_Item.ItemBalancing as BannerItemBalancingData;
		float itemMainStat = BannerItemGameData.GetItemMainStat(m_Item as BannerItemGameData, m_Item.ItemData.Quality);
		float num = 0f;
		m_BaseStatType.spriteName = "Character_Health_Small";
		BannerGameData bannerGameData = DIContainerInfrastructure.GetCurrentPlayer().BannerGameData;
		if (bannerGameData != null)
		{
			if (bannerItemBalancingData.ItemType == InventoryItemType.Banner)
			{
				num = bannerGameData.BannerCenter.ItemMainStat;
				m_BaseStatType.spriteName = "Character_Damage_Small";
			}
			else if (bannerItemBalancingData.ItemType == InventoryItemType.BannerTip)
			{
				num = bannerGameData.BannerTip.ItemMainStat;
				m_BaseStatType.spriteName = "Character_Health_Small";
			}
		}
		float comparisionFooter = itemMainStat - num;
		SetComparisionFooter(comparisionFooter);
		m_PerkType.spriteName = EquipmentGameData.GetPerkIcon(m_Item as EquipmentGameData);
		m_PerkType.spriteName = BannerItemGameData.GetPerkIconNameByPerk((m_Item as BannerItemGameData).GetPerkTypeOfSkill());
	}

	private void SetComparisionFooter(float delta)
	{
		if (delta < 0f)
		{
			m_ArrowSprite.gameObject.SetActive(true);
			m_ArrowSprite.spriteName = "StatComparison_Lower";
		}
		else if (delta > 0f)
		{
			m_ArrowSprite.gameObject.SetActive(true);
			m_ArrowSprite.spriteName = "StatComparison_Higher";
		}
		else
		{
			m_ArrowSprite.gameObject.SetActive(false);
		}
		m_BaseStatValue.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(Mathf.Abs((int)delta));
	}

	public void ShowPerkTooltip()
	{
		EquipmentGameData equipmentGameData = m_Item as EquipmentGameData;
		if (equipmentGameData != null && m_PerkType != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowPerkOverlay(m_PerkType.cachedTransform, equipmentGameData, true);
		}
	}

	public float FlyToTransform(Transform root)
	{
		m_Tween.InvertCurves(m_Tween.transform.position.y > root.position.y);
		m_Tween.m_Timing = CHMotionTween.TimingTypes.Duration;
		m_Tween.m_EndTransform = root;
		m_Tween.m_EndOffset = Vector3.zero;
		m_Tween.Play();
		return m_Tween.MovementDuration;
	}

	public void ResetFromFly()
	{
		if ((bool)m_Tween)
		{
			m_Tween.transform.localPosition = m_Position;
		}
	}
}
