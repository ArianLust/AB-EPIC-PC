using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Character;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class InventoryItemSlot : BaseItemSlot
{
	[SerializeField]
	public GameObject m_UpdateIndikatorRoot;

	[SerializeField]
	private UISprite m_SpecialSprite;

	[SerializeField]
	private UISprite m_PerkType;

	[SerializeField]
	private GameObject m_BadgeRoot;

	[SerializeField]
	private List<UISprite> m_StarRoots = new List<UISprite>();

	[SerializeField]
	private UISprite m_ArrowSprite;

	[SerializeField]
	private List<GameObject> m_itemSourceRoots = new List<GameObject>();

	[SerializeField]
	private GameObject m_ItemInfoRoot;

	[SerializeField]
	private UISprite m_BaseStatType;

	[SerializeField]
	protected UILabel m_BaseStatValue;

	[SerializeField]
	public UIInputTrigger m_InputTrigger;

	[SerializeField]
	private UISprite m_ButtonBody;

	private GameObject m_SelectionFrame;

	[SerializeField]
	private GameObject m_SelectionFramePrefab;

	[SerializeField]
	private Transform m_ItemSpriteSpawnRoot;

	[SerializeField]
	private CHMotionTween m_Tween;

	[SerializeField]
	private GameObject m_EnchantmentParent;

	[SerializeField]
	private UILabel m_EnchantmentLevel;

	[SerializeField]
	private UISprite m_EnchantmentSprite;

	[SerializeField]
	public GameObject m_purchaseIndicator;

	[SerializeField]
	public UISprite m_purchaseIndicatorBody;

	private GameObject m_ItemSprite;

	private IInventoryItemGameData m_Model;

	protected IInventoryItemGameData m_FinalItem;

	[HideInInspector]
	public bool m_Used;

	[HideInInspector]
	public bool m_UseSwipe;

	private CHMotionTween m_LocalTween;

	private Vector3 m_Position;

	private bool m_IsSetToDestroy;

	private bool m_IsUnavailable;

	private bool m_classPreviewIsNext;

	private bool m_isPvp;

	private TrophyData m_trophy;

	public TrophyData Trophy
	{
		get
		{
			return m_trophy;
		}
		set
		{
			m_trophy = value;
			GetComponentInChildren<CHMeshSprite>().m_SpriteName = value.NameId;
			GetComponentInChildren<CHMeshSprite>().UpdateSprite(false, true);
		}
	}

	[method: MethodImpl(32)]
	public event Action<InventoryItemSlot> OnSelected;

	[method: MethodImpl(32)]
	public event Action<InventoryItemSlot> BeforeUsed;

	[method: MethodImpl(32)]
	public event Action<InventoryItemSlot> OnUsed;

	[method: MethodImpl(32)]
	public event Action<InventoryItemSlot> OnScrap;

	[method: MethodImpl(32)]
	public event Action<bool> OnModifyHorizontalDrag;

	[method: MethodImpl(32)]
	public event Action<float> OnSetVerticalPosition;

	public override bool SetModel(IInventoryItemGameData item, bool isPvp)
	{
		if ((bool)m_purchaseIndicator && m_purchaseIndicator.name != "Guide_Ok") // sometimes the purchaseindicator is set to the guide_ok prefab, and when it disables it, it disables the entire prefab
		{
			m_purchaseIndicator.SetActive(false);
		}
		m_isPvp = isPvp;
		m_LocalTween = GetComponent<CHMotionTween>();
		m_Model = item;
		if ((bool)m_Tween)
		{
			m_Position = m_Tween.transform.localPosition;
		}
		DeRegisterEventHandler();
		RegisterEventHandler();
		if (item.ItemBalancing.ItemType != InventoryItemType.CraftingRecipes)
		{
			if ((bool)m_UpdateIndikatorRoot)
			{
				bool flag = m_Model.ItemData.IsNew;
				if (DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData != null && DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items != null && item is ClassItemGameData && !flag)
				{
					try
					{
						foreach (IInventoryItemGameData item2 in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Skin])
						{
							if (item2.ItemData.IsNew && (item2 as SkinItemGameData).BalancingData.OriginalClass == m_Model.ItemBalancing.NameId)
							{
								flag = true;
								break;
							}
						}
					}
					catch
					{
						return false;
					}
				}
				m_UpdateIndikatorRoot.SetActive(flag);
			}
		}
		else
		{
			bool active = m_Model.ItemData.IsNew;
			CraftingRecipeGameData craftingRecipeGameData = m_Model as CraftingRecipeGameData;
			if (craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Consumable || craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Resources || craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Ingredients)
			{
				IInventoryItemGameData data = null;
				if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, craftingRecipeGameData.GetResultLoot().Keys.FirstOrDefault(), out data) && data.ItemData.IsNew)
				{
					active = true;
				}
			}
			if ((bool)m_UpdateIndikatorRoot)
			{
				m_UpdateIndikatorRoot.SetActive(active);
			}
		}
		switch (item.ItemBalancing.ItemType)
		{
		case InventoryItemType.Class:
			SetClassItem(item);
			break;
		case InventoryItemType.CraftingRecipes:
			SetRecipeItem((CraftingRecipeGameData)item);
			break;
		case InventoryItemType.MainHandEquipment:
			SetMainHandItem(item);
			break;
		case InventoryItemType.OffHandEquipment:
			SetOffHandItem(item);
			break;
		case InventoryItemType.BannerTip:
		case InventoryItemType.Banner:
		case InventoryItemType.BannerEmblem:
			SetBannerItem(item);
			break;
		}
		return true;
	}

	public bool IsDestroyedCurrently()
	{
		return m_IsSetToDestroy;
	}

	public void SetToDestroy(bool toDestroy)
	{
		m_IsSetToDestroy = toDestroy;
	}

	public IEnumerator MoveOffset(Vector2 offset, float duration)
	{
		Vector3 move = new Vector3(offset.x, offset.y, 0f);
		if ((bool)m_LocalTween)
		{
			m_LocalTween.m_EndOffset = offset;
			m_LocalTween.m_DurationInSeconds = duration;
			m_LocalTween.Play();
			yield return new WaitForSeconds(duration);
		}
	}

	private void SetOffHandItem(IInventoryItemGameData item)
	{
		EquipmentGameData equipmentGameData = item as EquipmentGameData;
		if (equipmentGameData != null && m_EnchantmentParent != null && equipmentGameData.AllowEnchanting())
		{
			m_EnchantmentParent.SetActive(true);
			m_EnchantmentLevel.enabled = true;
			m_EnchantmentLevel.text = equipmentGameData.EnchantementLevel.ToString();
			bool flag = equipmentGameData.IsMaxEnchanted();
			if (flag && equipmentGameData.EnchantementLevel == 0)
			{
				m_EnchantmentLevel.enabled = false;
				m_EnchantmentSprite.spriteName = "Enchantment_NA";
			}
			else if (flag)
			{
				m_EnchantmentSprite.spriteName = "Enchantment_Max";
			}
			else
			{
				m_EnchantmentSprite.spriteName = "Enchantment";
			}
		}
		else if (m_EnchantmentParent != null)
		{
			m_EnchantmentParent.SetActive(false);
		}
		if ((bool)m_SpecialSprite)
		{
			m_SpecialSprite.gameObject.SetActive(false);
		}
		m_BaseStatType.spriteName = "Character_Health_Small";
		m_ItemSprite = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(m_Model.ItemAssetName, m_ItemSpriteSpawnRoot, Vector3.zero, Quaternion.identity, false);
		if (m_ItemSprite != null)
		{
			m_ItemSprite.transform.localScale = Vector3.one;
		}
		if (equipmentGameData == null)
		{
			return;
		}
		if (m_PerkType != null)
		{
			m_PerkType.spriteName = EquipmentGameData.GetPerkIcon(equipmentGameData);
		}
		if (m_StarRoots.Count > 0)
		{
			m_StarRoots[0].transform.parent.gameObject.SetActive(true);
			if (equipmentGameData.IsSetItem)
			{
				for (int i = 0; i < m_StarRoots.Count; i++)
				{
					m_StarRoots[i].spriteName = m_StarRoots[i].spriteName.Replace("_Empty", "_Set");
					m_StarRoots[i].spriteName = m_StarRoots[i].spriteName.Replace("_Full", "_Set");
				}
				foreach (UISprite starRoot in m_StarRoots)
				{
					starRoot.gameObject.SetActive(true);
				}
				if ((bool)m_BadgeRoot)
				{
					m_BadgeRoot.SetActive(false);
				}
			}
			else
			{
				foreach (UISprite starRoot2 in m_StarRoots)
				{
					starRoot2.gameObject.SetActive(true);
				}
				if ((bool)m_BadgeRoot)
				{
					m_BadgeRoot.SetActive(false);
				}
				for (int j = 0; j < m_Model.ItemData.Quality && j < m_StarRoots.Count; j++)
				{
					m_StarRoots[j].spriteName = m_StarRoots[j].spriteName.Replace("_Empty", "_Full");
					m_StarRoots[j].spriteName = m_StarRoots[j].spriteName.Replace("_Set", "_Full");
				}
				for (int k = m_Model.ItemData.Quality; k < m_StarRoots.Count; k++)
				{
					m_StarRoots[k].spriteName = m_StarRoots[k].spriteName.Replace("_Set", "_Empty");
					m_StarRoots[k].spriteName = m_StarRoots[k].spriteName.Replace("_Full", "_Empty");
				}
			}
		}
		RefreshItemStat(item);
	}

	private void SetBannerItem(IInventoryItemGameData item)
	{
		BannerItemGameData bannerItemGameData = item as BannerItemGameData;
		if (bannerItemGameData != null && m_EnchantmentParent != null && bannerItemGameData.AllowEnchanting())
		{
			m_EnchantmentParent.SetActive(true);
			m_EnchantmentLevel.enabled = true;
			m_EnchantmentLevel.text = bannerItemGameData.EnchantementLevel.ToString();
			bool flag = bannerItemGameData.IsMaxEnchanted();
			if (flag && bannerItemGameData.EnchantementLevel == 0)
			{
				m_EnchantmentLevel.enabled = false;
				m_EnchantmentSprite.spriteName = "Enchantment_NA";
			}
			else if (flag)
			{
				m_EnchantmentSprite.spriteName = "Enchantment_Max";
			}
			else
			{
				m_EnchantmentSprite.spriteName = "Enchantment";
			}
		}
		else if (m_EnchantmentParent != null)
		{
			m_EnchantmentParent.SetActive(false);
		}
		if (m_StarRoots.Count > 0)
		{
			m_StarRoots[0].transform.parent.gameObject.SetActive(false);
		}
		m_ItemSprite = DIContainerInfrastructure.GetBannerAssetProvider().InstantiateObject(item.ItemBalancing.AssetBaseId, m_ItemSpriteSpawnRoot, Vector3.zero, Quaternion.identity);
		BannerItemGameData bannerItemGameData2 = item as BannerItemGameData;
		if (m_ItemSprite != null)
		{
			m_ItemSprite.transform.localScale = Vector3.one;
			BannerFlagAssetController component = m_ItemSprite.GetComponent<BannerFlagAssetController>();
			if ((bool)component)
			{
				component.SetColors(component.GetColorFromList(bannerItemGameData2.BalancingData.ColorVector));
			}
			BannerEmblemAssetController component2 = m_ItemSprite.GetComponent<BannerEmblemAssetController>();
			if ((bool)component2)
			{
				component2.SetColors(component2.GetColorFromList(bannerItemGameData2.BalancingData.ColorVector));
			}
		}
		if ((bool)m_SpecialSprite)
		{
			m_SpecialSprite.gameObject.SetActive(false);
		}
		if (bannerItemGameData2.HasPerkSkill() && (bool)m_PerkType)
		{
			m_PerkType.spriteName = BannerItemGameData.GetPerkIconNameByPerk(bannerItemGameData2.GetPerkTypeOfSkill());
		}
		if (m_StarRoots.Count > 0)
		{
			m_StarRoots[0].transform.parent.gameObject.SetActive(true);
			if (bannerItemGameData2.IsSetItem)
			{
				for (int i = 0; i < m_StarRoots.Count; i++)
				{
					m_StarRoots[i].spriteName = m_StarRoots[i].spriteName.Replace("_Empty", "_Set");
					m_StarRoots[i].spriteName = m_StarRoots[i].spriteName.Replace("_Full", "_Set");
				}
				foreach (UISprite starRoot in m_StarRoots)
				{
					starRoot.gameObject.SetActive(true);
				}
				if ((bool)m_BadgeRoot)
				{
					m_BadgeRoot.SetActive(false);
				}
			}
			else
			{
				foreach (UISprite starRoot2 in m_StarRoots)
				{
					starRoot2.gameObject.SetActive(true);
				}
				if ((bool)m_BadgeRoot)
				{
					m_BadgeRoot.SetActive(false);
				}
				for (int j = 0; j < bannerItemGameData2.GetStars() && j < m_StarRoots.Count; j++)
				{
					m_StarRoots[j].spriteName = m_StarRoots[j].spriteName.Replace("_Empty", "_Full");
					m_StarRoots[j].spriteName = m_StarRoots[j].spriteName.Replace("_Set", "_Full");
				}
				for (int k = bannerItemGameData2.GetStars(); k < m_StarRoots.Count; k++)
				{
					m_StarRoots[k].spriteName = m_StarRoots[k].spriteName.Replace("_Set", "_Empty");
					m_StarRoots[k].spriteName = m_StarRoots[k].spriteName.Replace("_Full", "_Empty");
				}
			}
		}
		RefreshItemStat(item);
	}

	private void SetMainHandItem(IInventoryItemGameData item)
	{
		EquipmentGameData equipmentGameData = item as EquipmentGameData;
		if (equipmentGameData != null && m_EnchantmentParent != null && equipmentGameData.AllowEnchanting())
		{
			m_EnchantmentParent.SetActive(true);
			m_EnchantmentLevel.enabled = true;
			m_EnchantmentLevel.text = equipmentGameData.EnchantementLevel.ToString();
			bool flag = equipmentGameData.IsMaxEnchanted();
			if (flag && equipmentGameData.EnchantementLevel == 0)
			{
				m_EnchantmentLevel.enabled = false;
				m_EnchantmentSprite.spriteName = "Enchantment_NA";
			}
			else if (flag)
			{
				m_EnchantmentSprite.spriteName = "Enchantment_Max";
			}
			else
			{
				m_EnchantmentSprite.spriteName = "Enchantment";
			}
		}
		else if (m_EnchantmentParent != null)
		{
			m_EnchantmentParent.SetActive(false);
		}
		if ((bool)m_SpecialSprite)
		{
			m_SpecialSprite.gameObject.SetActive(false);
		}
		m_BaseStatType.spriteName = "Character_Damage_Small";
		m_ItemSprite = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(m_Model.ItemAssetName, m_ItemSpriteSpawnRoot, Vector3.zero, Quaternion.identity, false);
		if (m_ItemSprite != null)
		{
			m_ItemSprite.transform.localScale = Vector3.one;
		}
		if (equipmentGameData == null)
		{
			return;
		}
		if (m_PerkType != null)
		{
			m_PerkType.spriteName = EquipmentGameData.GetPerkIcon(equipmentGameData);
		}
		if (m_StarRoots.Count > 0)
		{
			m_StarRoots[0].transform.parent.gameObject.SetActive(true);
			if (equipmentGameData.IsSetItem)
			{
				for (int i = 0; i < m_StarRoots.Count; i++)
				{
					m_StarRoots[i].spriteName = m_StarRoots[i].spriteName.Replace("_Empty", "_Set");
					m_StarRoots[i].spriteName = m_StarRoots[i].spriteName.Replace("_Full", "_Set");
				}
				foreach (UISprite starRoot in m_StarRoots)
				{
					starRoot.gameObject.SetActive(true);
				}
				if ((bool)m_BadgeRoot)
				{
					m_BadgeRoot.SetActive(false);
				}
			}
			else
			{
				foreach (UISprite starRoot2 in m_StarRoots)
				{
					starRoot2.gameObject.SetActive(true);
				}
				if ((bool)m_BadgeRoot)
				{
					m_BadgeRoot.SetActive(false);
				}
				for (int j = 0; j < m_Model.ItemData.Quality && j < m_StarRoots.Count; j++)
				{
					m_StarRoots[j].spriteName = m_StarRoots[j].spriteName.Replace("_Empty", "_Full");
					m_StarRoots[j].spriteName = m_StarRoots[j].spriteName.Replace("_Set", "_Full");
				}
				for (int k = m_Model.ItemData.Quality; k < m_StarRoots.Count; k++)
				{
					m_StarRoots[k].spriteName = m_StarRoots[k].spriteName.Replace("_Set", "_Empty");
					m_StarRoots[k].spriteName = m_StarRoots[k].spriteName.Replace("_Full", "_Empty");
				}
			}
		}
		RefreshItemStat(item);
	}

	public void RefreshItemStat(IInventoryItemGameData itemData)
	{
		float itemMainStat = itemData.ItemMainStat;
		float num = 0f;
		float num2 = 0f;
		EquipmentGameData equipmentGameData = itemData as EquipmentGameData;
		if (equipmentGameData != null)
		{
			BirdGameData bird = DIContainerInfrastructure.GetCurrentPlayer().GetBird(equipmentGameData.BalancingData.RestrictedBirdId);
			if (bird != null)
			{
				if (itemData.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment)
				{
					num = bird.MainHandItem.ItemMainStat;
				}
				else if (itemData.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment)
				{
					num = bird.OffHandItem.ItemMainStat;
				}
			}
			num2 = itemMainStat - num;
		}
		else if (itemData is BannerItemGameData)
		{
			BannerGameData bannerGameData = DIContainerInfrastructure.GetCurrentPlayer().BannerGameData;
			if (bannerGameData != null)
			{
				if (itemData.ItemBalancing.ItemType == InventoryItemType.Banner)
				{
					num = bannerGameData.BannerCenter.ItemMainStat;
				}
				else if (itemData.ItemBalancing.ItemType == InventoryItemType.BannerEmblem)
				{
					num = bannerGameData.BannerEmblem.ItemMainStat;
				}
				else if (itemData.ItemBalancing.ItemType == InventoryItemType.BannerTip)
				{
					num = bannerGameData.BannerTip.ItemMainStat;
				}
			}
			BannerItemGameData bannerItemGameData = itemData as BannerItemGameData;
			num2 = bannerItemGameData.ItemMainStat - num;
		}
		if ((bool)m_ArrowSprite)
		{
			if (num2 < 0f)
			{
				m_ArrowSprite.gameObject.SetActive(true);
				m_ArrowSprite.spriteName = "StatComparison_Lower";
			}
			else if (num2 > 0f)
			{
				m_ArrowSprite.gameObject.SetActive(true);
				m_ArrowSprite.spriteName = "StatComparison_Higher";
			}
			else
			{
				m_ArrowSprite.gameObject.SetActive(false);
			}
		}
		m_BaseStatValue.text = DIContainerInfrastructure.GetFormatProvider().GetBattleStatsFormat(Math.Abs(num2));
	}

	private void SetRecipeItem(IInventoryItemGameData item)
	{
		switch (((CraftingRecipeGameData)item).BalancingData.RecipeCategoryType)
		{
		case InventoryItemType.MainHandEquipment:
			SetRecipeMainHandItem(item);
			break;
		case InventoryItemType.OffHandEquipment:
			SetRecipeOffHandItem(item);
			break;
		case InventoryItemType.Resources:
			SetRecipeResourceItem(item);
			break;
		case InventoryItemType.Consumable:
			SetRecipeConsumableItem(item);
			break;
		case InventoryItemType.Ingredients:
			SetRecipeIngredientItem(item);
			break;
		default:
			DebugLog.Error("Unhandeled CraftingREcipe ItemType " + ((CraftingRecipeGameData)item).BalancingData.RecipeCategoryType);
			break;
		}
	}

	private void SetRecipeResourceItem(IInventoryItemGameData item)
	{
		CraftingRecipeGameData craftingRecipeGameData = (CraftingRecipeGameData)item;
		Dictionary<string, LootInfoData> loot = DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level);
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerInfrastructure.GetCurrentPlayer(), loot);
		CraftingItemBalancingData craftingItemBalancingData = (CraftingItemBalancingData)itemsFromLoot[0].ItemBalancing;
		m_FinalItem = itemsFromLoot[0];
		if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(craftingItemBalancingData.AtlasNameId))
		{
			GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(craftingItemBalancingData.AtlasNameId) as GameObject;
			m_BaseStatType.atlas = gameObject.GetComponent<UIAtlas>();
		}
		m_BaseStatType.spriteName = craftingItemBalancingData.AssetBaseId;
		m_BaseStatValue.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_FinalItem.ItemBalancing.NameId));
	}

	private void SetRecipeIngredientItem(IInventoryItemGameData item)
	{
		CraftingRecipeGameData craftingRecipeGameData = (CraftingRecipeGameData)item;
		Dictionary<string, LootInfoData> loot = DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level);
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerInfrastructure.GetCurrentPlayer(), loot);
		CraftingItemBalancingData craftingItemBalancingData = (CraftingItemBalancingData)itemsFromLoot[0].ItemBalancing;
		m_FinalItem = itemsFromLoot[0];
		if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(craftingItemBalancingData.AtlasNameId))
		{
			GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(craftingItemBalancingData.AtlasNameId) as GameObject;
			m_BaseStatType.atlas = gameObject.GetComponent<UIAtlas>();
		}
		m_BaseStatType.spriteName = craftingItemBalancingData.AssetBaseId;
		m_BaseStatValue.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_FinalItem.ItemBalancing.NameId));
	}

	private void SetRecipeConsumableItem(IInventoryItemGameData item)
	{
		CraftingRecipeGameData craftingRecipeGameData = (CraftingRecipeGameData)item;
		Dictionary<string, LootInfoData> loot = DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level);
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerInfrastructure.GetCurrentPlayer(), loot);
		ConsumableItemBalancingData consumableItemBalancingData = (ConsumableItemBalancingData)itemsFromLoot[0].ItemBalancing;
		m_FinalItem = itemsFromLoot[0];
		if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset("Consumables"))
		{
			GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject("Consumables") as GameObject;
			m_BaseStatType.atlas = gameObject.GetComponent<UIAtlas>();
		}
		m_BaseStatType.spriteName = consumableItemBalancingData.AssetBaseId;
		m_BaseStatValue.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_FinalItem.ItemBalancing.NameId));
	}

	private void SetRecipeOffHandItem(IInventoryItemGameData item)
	{
		CraftingRecipeGameData craftingRecipeGameData = (CraftingRecipeGameData)item;
		Dictionary<string, LootInfoData> loot = DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level);
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerInfrastructure.GetCurrentPlayer(), loot);
		EquipmentBalancingData equipment = (EquipmentBalancingData)itemsFromLoot[0].ItemBalancing;
		m_BaseStatType.spriteName = "Character_Health_Small";
		if ((bool)m_SpecialSprite)
		{
			m_SpecialSprite.gameObject.SetActive(true);
			m_SpecialSprite.spriteName = EquipmentGameData.GetRestrictedBirdIcon(itemsFromLoot[0] as EquipmentGameData);
		}
		if (m_PerkType != null)
		{
			m_PerkType.spriteName = EquipmentGameData.GetPerkIcon(itemsFromLoot[0] as EquipmentGameData);
		}
		RefreshRecipeEntry(itemsFromLoot[0], equipment);
	}

	private void RefreshRecipeEntry(IInventoryItemGameData finalItem, EquipmentBalancingData equipment)
	{
		m_FinalItem = finalItem;
		float itemMainStat = EquipmentGameData.GetItemMainStat(equipment.BaseStat, equipment.StatPerLevel, finalItem.ItemData.Level, 3, equipment.StatPerQuality, equipment.StatPerQualityPercent, 0);
		float num = 0f;
		BirdGameData bird = DIContainerInfrastructure.GetCurrentPlayer().GetBird(equipment.RestrictedBirdId);
		if (bird != null)
		{
			if (equipment.ItemType == InventoryItemType.MainHandEquipment)
			{
				num = bird.MainHandItem.ItemMainStat;
			}
			else if (equipment.ItemType == InventoryItemType.OffHandEquipment)
			{
				num = bird.OffHandItem.ItemMainStat;
			}
		}
		float num2 = itemMainStat - num;
		if ((bool)m_ArrowSprite)
		{
			if (num2 < 0f)
			{
				m_ArrowSprite.gameObject.SetActive(true);
				m_ArrowSprite.spriteName = "StatComparison_Lower";
			}
			else if (num2 > 0f)
			{
				m_ArrowSprite.gameObject.SetActive(true);
				m_ArrowSprite.spriteName = "StatComparison_Higher";
			}
			else
			{
				m_ArrowSprite.gameObject.SetActive(false);
			}
		}
		m_BaseStatValue.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(Mathf.Abs((int)num2));
		if (!m_ItemSprite)
		{
			m_ItemSprite = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(m_FinalItem.ItemAssetName, m_ItemSpriteSpawnRoot, Vector3.zero, Quaternion.identity, false);
			Renderer componentInChildren = m_ItemSprite.GetComponentInChildren<Renderer>();
			StartCoroutine(SetRecipeShader(componentInChildren));
		}
	}

	private IEnumerator SetRecipeShader(Renderer rend)
	{
		yield return new WaitForEndOfFrame();
		if (!(rend.material.shader == DIContainerLogic.GetVisualEffectsBalancing().m_RecipeItemMaterial.shader))
		{
			rend.material = new Material(rend.sharedMaterial);
			rend.material.shader = DIContainerLogic.GetVisualEffectsBalancing().m_RecipeItemMaterial.shader;
			rend.material.color = DIContainerLogic.GetVisualEffectsBalancing().m_RecipeItemMaterial.color;
		}
	}

	public bool IsUnavailable()
	{
		return m_IsUnavailable;
	}

	public void SetSlotUnavailable()
	{
		m_IsUnavailable = true;
		if ((bool)m_ItemSprite)
		{
			List<Renderer> list = m_ItemSprite.GetComponentsInChildren<Renderer>().ToList();
			for (int i = 0; i < list.Count; i++)
			{
				StartCoroutine(SetClassShader(list[i]));
			}
		}
		if ((bool)m_ButtonBody)
		{
			DebugLog.Log("Crafting Button Disabled!");
			m_ButtonBody.spriteName = m_ButtonBody.spriteName.Replace("_D", string.Empty);
			m_ButtonBody.spriteName += "_D";
		}
	}

	private IEnumerator SetClassShader(Renderer rend)
	{
		yield return new WaitForEndOfFrame();
		if (!(rend.material.shader == DIContainerLogic.GetVisualEffectsBalancing().m_ClassItemUnavailableMaterial.shader))
		{
			rend.material = new Material(rend.sharedMaterial);
			rend.material.shader = DIContainerLogic.GetVisualEffectsBalancing().m_ClassItemUnavailableMaterial.shader;
			rend.material.color = DIContainerLogic.GetVisualEffectsBalancing().m_ClassItemUnavailableMaterial.color;
		}
	}

	private void SetRecipeMainHandItem(IInventoryItemGameData item)
	{
		CraftingRecipeGameData craftingRecipeGameData = (CraftingRecipeGameData)item;
		Dictionary<string, LootInfoData> loot = DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level);
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerInfrastructure.GetCurrentPlayer(), loot);
		EquipmentBalancingData equipment = (EquipmentBalancingData)itemsFromLoot[0].ItemBalancing;
		m_BaseStatType.spriteName = "Character_Damage_Small";
		if ((bool)m_SpecialSprite)
		{
			m_SpecialSprite.gameObject.SetActive(true);
			m_SpecialSprite.spriteName = EquipmentGameData.GetRestrictedBirdIcon(itemsFromLoot[0] as EquipmentGameData);
		}
		if (m_PerkType != null)
		{
			m_PerkType.spriteName = EquipmentGameData.GetPerkIcon(itemsFromLoot[0] as EquipmentGameData);
		}
		RefreshRecipeEntry(itemsFromLoot[0], equipment);
	}

	public void ShowTooltip()
	{
		if (m_IsUnavailable)
		{
			return;
		}
		if (m_Model == null)
		{
			BasicShopOfferBalancingData shopOffer = DIContainerLogic.GetShopService().GetShopOffer("offer_resource_bundle_01");
			List<IInventoryItemGameData> shopOfferContent = DIContainerLogic.GetShopService().GetShopOfferContent(DIContainerInfrastructure.GetCurrentPlayer(), shopOffer, DIContainerLogic.GetSalesManagerService().GetOfferSaleDetails(shopOffer.NameId));
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(base.transform, shopOfferContent, shopOffer, true);
			return;
		}
		IInventoryItemGameData item = m_Model;
		if (m_Model is ClassItemGameData)
		{
			item = TryGetOverrideSkin(m_Model);
		}
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(base.transform, item, true, m_isPvp);
	}

	private IInventoryItemGameData TryGetOverrideSkin(IInventoryItemGameData classItem)
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (currentPlayer.Data.EquippedSkins.ContainsKey(classItem.ItemBalancing.NameId))
		{
			return new SkinItemGameData(currentPlayer.Data.EquippedSkins[classItem.ItemBalancing.NameId]);
		}
		ClassSkinBalancingData classSkinBalancingData = (from b in DIContainerBalancing.Service.GetBalancingDataList<ClassSkinBalancingData>()
			where b.OriginalClass == classItem.ItemBalancing.NameId
			select b).FirstOrDefault();
		return new SkinItemGameData(classSkinBalancingData.NameId);
	}

	public void ShowPerkTooltip()
	{
		EquipmentGameData equipmentGameData = m_Model as EquipmentGameData;
		if (equipmentGameData != null && m_PerkType != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowPerkOverlay(m_PerkType.cachedTransform, equipmentGameData, true);
		}
		else if (m_FinalItem != null)
		{
			EquipmentGameData equipmentGameData2 = m_FinalItem as EquipmentGameData;
			if (equipmentGameData2 != null && m_PerkType != null)
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowPerkOverlay(m_PerkType.cachedTransform, equipmentGameData2, true);
			}
		}
	}

	public void UpdateIcon(string AssetId)
	{
		if (m_ItemSpriteSpawnRoot != null && m_ItemSpriteSpawnRoot.childCount > 0)
		{
			UnityEngine.Object.Destroy(m_ItemSpriteSpawnRoot.GetChild(0).gameObject);
		}
		StartCoroutine(CreateSprite(AssetId));
	}

	private IEnumerator CreateSprite(string AssetId)
	{
		yield return new WaitForEndOfFrame();
		m_ItemSprite = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(AssetId, m_ItemSpriteSpawnRoot, Vector3.zero, Quaternion.identity, false);
	}

	private void SetClassItem(IInventoryItemGameData item)
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		string assetBaseId = item.ItemBalancing.AssetBaseId;
		if (currentPlayer.Data.EquippedSkins.ContainsKey(item.ItemBalancing.NameId))
		{
			assetBaseId = DIContainerBalancing.Service.GetBalancingData<ClassSkinBalancingData>(currentPlayer.Data.EquippedSkins[item.ItemBalancing.NameId]).AssetBaseId;
		}
		m_ItemSprite = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(assetBaseId, m_ItemSpriteSpawnRoot, Vector3.zero, Quaternion.identity, false);
		if (!m_BadgeRoot)
		{
			return;
		}
		m_BadgeRoot.SetActive(false);
		IInventoryItemGameData data = null;
		if (!DIContainerLogic.InventoryService.TryGetItemGameData(currentPlayer.InventoryGameData, "unlock_mastery_badge", out data))
		{
			return;
		}
		int level = (item as ClassItemGameData).Data.Level;
		if (level > 0)
		{
			m_BadgeRoot.SetActive(true);
			UILabel componentInChildren = m_BadgeRoot.GetComponentInChildren<UILabel>();
			if (componentInChildren != null)
			{
				componentInChildren.text = level.ToString();
			}
		}
	}

	public override IInventoryItemGameData GetModel()
	{
		return m_Model;
	}

	public void SelectItemData()
	{
		RaiseOnSelected();
	}

	private void RaiseOnScrap()
	{
		if (!m_Used)
		{
			DebugLog.Log("Raised Scrapped!");
			if (this.OnScrap != null)
			{
				this.OnScrap(this);
			}
		}
	}

	private void RaiseOnUsed()
	{
		if (m_IsUnavailable)
		{
			return;
		}
		DebugLog.Log("Raised Used!");
		if (!m_Used)
		{
			if (this.BeforeUsed != null)
			{
				this.BeforeUsed(this);
			}
			if (this.OnUsed != null)
			{
				this.OnUsed(this);
			}
		}
	}

	public void RaiseOnSelected()
	{
		if (!m_Used && this.OnSelected != null)
		{
			this.OnSelected(this);
		}
	}

	private void Awake()
	{
	}

	public override void Select(bool classPreviewIsThis = false)
	{
		m_Used = true;
		StopCoroutine("DeselectCoroutine");
		if (!m_SelectionFrame)
		{
			m_SelectionFrame = UnityEngine.Object.Instantiate(m_SelectionFramePrefab, base.transform.position, Quaternion.identity) as GameObject;
			m_SelectionFrame.transform.parent = base.transform;
		}
		else
		{
			m_SelectionFrame.transform.Find("Frame").gameObject.SetActive(true);
			if ((bool)m_SelectionFrame.transform.Find("EquippedStatus"))
			{
				m_SelectionFrame.transform.Find("EquippedStatus").gameObject.SetActive(true);
			}
		}
		if ((bool)m_ItemInfoRoot)
		{
			m_ItemInfoRoot.SetActive(false);
		}
		m_SelectionFrame.SetActive(true);
		IInventoryItemGameData inventoryItemGameData = null;
		if (m_Model != null && !m_Model.ItemData.IsNew && (bool)m_UpdateIndikatorRoot)
		{
			DisableUpdateIndikator();
		}
		if ((bool)m_SelectionFrame.GetComponent<Animation>()["Show"])
		{
			m_SelectionFrame.GetComponent<Animation>().Play("Show");
		}
		if ((bool)m_SelectionFrame.GetComponent<Animation>()["Loop"])
		{
			m_SelectionFrame.GetComponent<Animation>().PlayQueued("Loop");
		}
		UIPlayAnimation[] componentsInChildren = m_InputTrigger.GetComponentsInChildren<UIPlayAnimation>();
		UIPlayAnimation[] array = componentsInChildren;
		foreach (UIPlayAnimation uIPlayAnimation in array)
		{
			uIPlayAnimation.enabled = false;
		}
		if (classPreviewIsThis && (bool)m_SelectionFrame.transform.Find("EquippedStatus"))
		{
			m_SelectionFrame.transform.Find("EquippedStatus").gameObject.SetActive(false);
		}
	}

	private void DisableUpdateIndikator()
	{
		if ((bool)m_UpdateIndikatorRoot)
		{
			m_UpdateIndikatorRoot.SetActive(false);
		}
	}

	public void RefreshStat()
	{
		if (m_Model.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment || m_Model.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment || m_Model.ItemBalancing.ItemType == InventoryItemType.BannerEmblem || m_Model.ItemBalancing.ItemType == InventoryItemType.Banner || m_Model.ItemBalancing.ItemType == InventoryItemType.BannerTip)
		{
			RefreshItemStat(m_Model);
		}
		else if (m_FinalItem != null && (m_FinalItem.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment || m_FinalItem.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment))
		{
			RefreshRecipeEntry(m_FinalItem as EquipmentGameData, m_FinalItem.ItemBalancing as EquipmentBalancingData);
		}
	}

	public void RemoveLeftOverSelection()
	{
		if (m_SelectionFrame != null)
		{
			UnityEngine.Object.Destroy(m_SelectionFrame);
		}
	}

	public override void Deselect(bool classPreviewIsNext = false)
	{
		m_classPreviewIsNext = classPreviewIsNext;
		m_Used = false;
		StartCoroutine("DeselectCoroutine");
	}

	private IEnumerator DeselectCoroutine()
	{
		if ((bool)m_SelectionFrame.GetComponent<Animation>()["Hide"])
		{
			m_SelectionFrame.GetComponent<Animation>().Play("Hide");
			yield return new WaitForSeconds(m_SelectionFrame.GetComponent<Animation>()["Hide"].length);
		}
		if ((bool)m_ItemInfoRoot)
		{
			m_ItemInfoRoot.SetActive(true);
		}
		UIPlayAnimation[] buttonAnimations = m_InputTrigger.GetComponentsInChildren<UIPlayAnimation>();
		UIPlayAnimation[] array = buttonAnimations;
		foreach (UIPlayAnimation UIPlayAnimation in array)
		{
			UIPlayAnimation.enabled = true;
		}
		if (!m_classPreviewIsNext)
		{
			UnityEngine.Object.Destroy(m_SelectionFrame);
		}
		else
		{
			m_SelectionFrame.transform.Find("Frame").gameObject.SetActive(false);
		}
	}

	public void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if (!m_UseSwipe && (bool)m_InputTrigger)
		{
			m_InputTrigger.Clicked += RaiseOnUsed;
		}
	}

	private void RaiseDragUpDown(float upDownSummedDelta)
	{
		if (!m_Used)
		{
			m_ItemSpriteSpawnRoot.localPosition = new Vector3(m_ItemSpriteSpawnRoot.localPosition.x, upDownSummedDelta, m_ItemSpriteSpawnRoot.localPosition.z);
		}
	}

	private void RaiseSwipeBegan(bool began)
	{
		if (!m_Used && this.OnModifyHorizontalDrag != null)
		{
			this.OnModifyHorizontalDrag(!began);
		}
	}

	public void DeRegisterEventHandler()
	{
		if ((bool)m_InputTrigger)
		{
			m_InputTrigger.Clicked -= RaiseOnUsed;
		}
	}

	private void OnDestroy()
	{
		RemoveAssets();
		DeRegisterEventHandler();
	}

	public void RemoveAssets()
	{
		if (m_Model == null || m_Model.ItemBalancing == null)
		{
			return;
		}
		switch (m_Model.ItemBalancing.ItemType)
		{
		case InventoryItemType.Class:
			if ((bool)DIContainerInfrastructure.GetClassAssetProvider())
			{
				DIContainerInfrastructure.GetClassAssetProvider().DestroyObject(m_Model.ItemBalancing.AssetBaseId, m_ItemSprite);
			}
			break;
		case InventoryItemType.Consumable:
			break;
		case InventoryItemType.CraftingRecipes:
			if ((bool)DIContainerInfrastructure.GetEquipmentAssetProvider())
			{
				DIContainerInfrastructure.GetEquipmentAssetProvider().DestroyObject(m_FinalItem.ItemAssetName, m_ItemSprite);
			}
			break;
		case InventoryItemType.Ingredients:
			break;
		case InventoryItemType.MainHandEquipment:
		case InventoryItemType.OffHandEquipment:
			if ((bool)DIContainerInfrastructure.GetEquipmentAssetProvider())
			{
				DIContainerInfrastructure.GetEquipmentAssetProvider().DestroyObject(m_Model.ItemAssetName, m_ItemSprite);
			}
			break;
		case InventoryItemType.BannerTip:
		case InventoryItemType.Banner:
		case InventoryItemType.BannerEmblem:
			if ((bool)DIContainerInfrastructure.GetBannerAssetProvider())
			{
				DIContainerInfrastructure.GetBannerAssetProvider().DestroyObject(m_Model.ItemAssetName, m_ItemSprite);
			}
			break;
		case InventoryItemType.PlayerStats:
			break;
		case InventoryItemType.PlayerToken:
			break;
		case InventoryItemType.Points:
			break;
		case InventoryItemType.Premium:
			break;
		case InventoryItemType.Resources:
			break;
		case InventoryItemType.Story:
			break;
		case InventoryItemType.EventBattleItem:
		case InventoryItemType.EventCollectible:
		case InventoryItemType.Mastery:
			break;
		}
	}

	public void RefreshAssets(IInventoryItemGameData inventoryItemGameData)
	{
		RemoveAssets();
		SetModel(inventoryItemGameData, m_isPvp);
	}

	public void FlyToTransformThenReset(Transform root, Vector3 offset)
	{
		StartCoroutine(FlyToTransformThenResetCoroutine(root, offset, 0f));
	}

	public void FlyToTransformThenReset(Transform root, Vector3 offset, float duration)
	{
		StartCoroutine(FlyToTransformThenResetCoroutine(root, offset, duration));
	}

	private IEnumerator FlyToTransformThenResetCoroutine(Transform root, Vector3 offset)
	{
		yield return StartCoroutine(FlyToTransformThenResetCoroutine(root, offset, 0f));
	}

	private IEnumerator FlyToTransformThenResetCoroutine(Transform root, Vector3 offset, float duration)
	{
		if (!(m_Tween == null))
		{
			yield return new WaitForSeconds(FlyToTransform(root, offset, duration));
			if (!(m_Tween == null))
			{
				m_Tween.transform.localPosition = m_Position;
			}
		}
	}

	public float FlyToTransform(Transform root, Vector3 offset, bool removeCollider = false)
	{
		return FlyToTransform(root, offset, 0f, removeCollider);
	}

	public float FlyToTransform(Transform root, Vector3 offset, float duration, bool removeCollider = false)
	{
		DebugLog.Log("Fly to Transform");
		m_Tween.InvertCurves(m_Tween.transform.position.y > root.position.y);
		m_Tween.m_EndTransform = root;
		m_Tween.m_EndOffset = offset;
		if (removeCollider && m_Tween.GetComponent<Collider>() != null)
		{
			m_Tween.GetComponent<Collider>().enabled = false;
		}
		if (duration > 0f)
		{
			m_Tween.m_DurationInSeconds = duration;
		}
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

	public void SetIsNew(bool isNew)
	{
		IInventoryItemGameData data = null;
		if (m_FinalItem != null && DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_FinalItem.ItemBalancing.NameId, out data))
		{
			data.ItemData.IsNew = isNew;
		}
		m_Model.ItemData.IsNew = isNew;
		if (!isNew && (bool)m_UpdateIndikatorRoot)
		{
			m_UpdateIndikatorRoot.SetActive(false);
		}
	}

	internal void SetUsed(bool used)
	{
		m_Used = used;
		SetIsNew(false);
	}
}
