using System;
using ABH.GameDatas;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class CollectionItemSlot : BaseItemSlot
{
	[SerializeField]
	private UISprite m_CollectionSpriteIcon;

	[SerializeField]
	private Transform m_ItemSpriteSpawnRoot;

	[SerializeField]
	private UILabel m_Label;

	[SerializeField]
	private UISprite m_Fill;

	[SerializeField]
	private GameObject m_CompletedRoot;

	[SerializeField]
	private UISprite m_ClassAffiliationIcon;

	[SerializeField]
	public UIInputTrigger m_InputTrigger;

	[SerializeField]
	private UISprite m_ButtonBody;

	[SerializeField]
	private CHMotionTween m_Tween;

	private GameObject m_ItemSprite;

	private IInventoryItemGameData m_Model;

	private IInventoryItemGameData m_FinalItem;

	[SerializeField]
	private GameObject m_ChestRewardRoot;

	private Vector3 m_Position;

	private bool m_IsSetToDestroy;

	private Requirement m_requirement;

	public bool SetModel(IInventoryItemGameData item, Requirement req = null)
	{
		m_Model = item;
		if (req != null)
		{
			m_requirement = req;
		}
		switch (item.ItemBalancing.ItemType)
		{
		case InventoryItemType.Class:
		case InventoryItemType.Skin:
			SetClassItem(item);
			break;
		case InventoryItemType.CollectionComponent:
			SetCollectionComponent(item, req);
			break;
		case InventoryItemType.Story:
			if (item.ItemBalancing.NameId.Contains("elite_chest"))
			{
				SetChestReward(item);
			}
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
		else
		{
			if (!(itemData is BannerItemGameData))
			{
				return;
			}
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
	}

	public void ShowTooltip()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(base.transform, m_Model, true, false);
	}

	private void SetCollectionComponent(IInventoryItemGameData item, Requirement req)
	{
		m_CollectionSpriteIcon.atlas = DIContainerInfrastructure.EventSystemStateManager.GetCurrentEventUiAtlas();
		m_CollectionSpriteIcon.spriteName = item.ItemAssetName;
		m_Label.text = Math.Min(req.Value, item.ItemValue) + " / " + req.Value;
		m_Fill.fillAmount = (float)item.ItemValue / req.Value;
		if ((float)item.ItemValue >= req.Value)
		{
			base.gameObject.PlayAnimationOrAnimatorState("Material_Idle_Active");
		}
	}

	private void SetClassItem(IInventoryItemGameData item)
	{
		m_ItemSprite = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(item.ItemBalancing.AssetBaseId, m_ItemSpriteSpawnRoot, Vector3.zero, Quaternion.identity, false);
		m_ItemSprite.transform.localScale = Vector3.one;
		m_ClassAffiliationIcon.spriteName = ClassItemGameData.GetRestrictedBirdIcon(item.ItemBalancing);
		m_ClassAffiliationIcon.MarkAsChanged();
		if (DIContainerLogic.EventSystemService.IsCollectionGroupAvailable())
		{
			switch (DIContainerLogic.EventSystemService.GetCurrentCollectionRewardStatus())
			{
			case EventCampaignRewardStatus.locked:
				base.gameObject.PlayAnimationOrAnimatorState("Reward_Idle_Inactive");
				break;
			case EventCampaignRewardStatus.unlocked:
			case EventCampaignRewardStatus.unlocked_fallback:
				base.gameObject.PlayAnimationOrAnimatorState("Reward_Idle_Active");
				break;
			case EventCampaignRewardStatus.unlocked_new:
			case EventCampaignRewardStatus.unlocked_new_fallback:
				break;
			}
		}
	}

	private void SetChestReward(IInventoryItemGameData item)
	{
		if ((bool)m_ChestRewardRoot)
		{
			m_ChestRewardRoot.gameObject.SetActive(true);
		}
		if (DIContainerLogic.EventSystemService.IsCollectionGroupAvailable())
		{
			if (m_ClassAffiliationIcon != null)
			{
				m_ClassAffiliationIcon.gameObject.SetActive(false);
			}
			switch (DIContainerLogic.EventSystemService.GetCurrentCollectionRewardStatus())
			{
			case EventCampaignRewardStatus.locked:
				base.gameObject.PlayAnimationOrAnimatorState("Reward_Idle_Inactive");
				break;
			case EventCampaignRewardStatus.unlocked:
			case EventCampaignRewardStatus.unlocked_fallback:
				base.gameObject.PlayAnimationOrAnimatorState("Reward_Idle_Active");
				break;
			case EventCampaignRewardStatus.chest_claimed:
				base.gameObject.PlayAnimationOrAnimatorState("Reward_Idle_Claimed");
				break;
			case EventCampaignRewardStatus.unlocked_new:
			case EventCampaignRewardStatus.unlocked_new_fallback:
				break;
			}
		}
	}

	public void UpdateStatus()
	{
		if (this.m_Model == null)
		{
			DebugLog.Error("CollectionItemSlot: No Model Found! Where is my Model?!?");
			return;
		}
		InventoryItemType itemType = this.m_Model.ItemBalancing.ItemType;
		if (itemType != InventoryItemType.CollectionComponent)
		{
			EventCampaignGameData currentMiniCampaign = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentMiniCampaign;
			EventCampaignRewardStatus rewardStatus = currentMiniCampaign.Data.RewardStatus;
			if (rewardStatus == EventCampaignRewardStatus.locked)
			{
				base.gameObject.PlayAnimationOrAnimatorState("Reward_Idle_Inactive");
			}
			if (rewardStatus == EventCampaignRewardStatus.unlocked_new || rewardStatus == EventCampaignRewardStatus.unlocked_new_fallback)
			{
				base.gameObject.PlayAnimationOrAnimatorState("Reward_SetActive");
				currentMiniCampaign.Data.RewardStatus = ((!DIContainerInfrastructure.EventSystemStateManager.UseCollectionFallbackReward(null)) ? EventCampaignRewardStatus.unlocked : EventCampaignRewardStatus.unlocked_fallback);
			}
			if (rewardStatus == EventCampaignRewardStatus.unlocked || rewardStatus == EventCampaignRewardStatus.unlocked_fallback)
			{
				base.gameObject.PlayAnimationOrAnimatorState("Reward_Idle_Active");
			}
		}
		else if (this.m_requirement == null)
		{
			DebugLog.Warn("CollectionItemSlot: No Requirement found to make CollectionComponent check: " + this.m_Model.ItemBalancing.NameId);
		}
		else if ((float)this.m_Model.ItemValue >= this.m_requirement.Value)
		{
			if (this.m_Model.ItemData.IsNew)
			{
				base.gameObject.PlayAnimationOrAnimatorState("Material_SetActive");
				this.m_Model.ItemData.IsNew = false;
			}
			else
			{
				base.gameObject.PlayAnimationOrAnimatorState("Material_Idle_Active");
			}
		}
		else
		{
			base.gameObject.PlayAnimationOrAnimatorState("Material_Idle_Inactive");
		}
	}

	public override IInventoryItemGameData GetModel()
	{
		return m_Model;
	}

	public void RefreshStat()
	{
		if (m_Model.ItemBalancing.ItemType == InventoryItemType.MainHandEquipment || m_Model.ItemBalancing.ItemType == InventoryItemType.OffHandEquipment || m_Model.ItemBalancing.ItemType == InventoryItemType.BannerEmblem || m_Model.ItemBalancing.ItemType == InventoryItemType.Banner || m_Model.ItemBalancing.ItemType == InventoryItemType.BannerTip)
		{
			RefreshItemStat(m_Model);
		}
	}

	private void OnDestroy()
	{
		RemoveAssets();
	}

	public void RemoveAssets()
	{
		if (m_Model == null)
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
		}
	}

	public void RefreshAssets(IInventoryItemGameData inventoryItemGameData)
	{
		RemoveAssets();
		SetModel(inventoryItemGameData);
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
	}

	internal void SetUsed(bool used)
	{
		SetIsNew(false);
	}
}
