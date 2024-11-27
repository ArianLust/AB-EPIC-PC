using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class HotSpotWorldMapViewShopNode : HotSpotWorldMapViewBase
{
	public ShopMenuType m_ShopMenuType;

	[SerializeField]
	private Vector3 m_bubbleOffset = Vector3.zero;

	public GameObject m_UpdateIndikator;

	private GameObject m_Bubble;

	public GameObject m_SoldOutIndicator;

	[SerializeField]
	private GameObject[] m_activateObjects;

	public override void ActivateAsset(bool activate)
	{
		GameObject[] activateObjects = m_activateObjects;
		foreach (GameObject gameObject in activateObjects)
		{
			gameObject.SetActive(true);
		}
	}

	public override bool IsCompleted()
	{
		return base.Model.Data.UnlockState == HotspotUnlockState.Resolved;
	}

	public override void HandleMouseButtonUp(bool directMove = false)
	{
		base.HandleMouseButtonUp(true);
	}

	public override void SynchBalancing()
	{
		base.SynchBalancing();
		m_active = false;
	}

	public override void Initialize()
	{
		if ((bool)m_SoldOutIndicator)
		{
			m_SoldOutIndicator.SetActive(false);
		}
		if (!base.Model.IsActive())
		{
			return;
		}
		bool flag = DIContainerLogic.RequirementService.CheckGenericRequirements(DIContainerInfrastructure.GetCurrentPlayer(), base.Model.BalancingData.EnterRequirements);
		if (m_ShopMenuType == ShopMenuType.Dojo)
		{
			uint ts = DIContainerInfrastructure.GetCurrentPlayer().Data.LastClassSwitchTime + DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").TimeForNextClassUpgrade;
			if (DIContainerLogic.GetTimingService().IsAfter(DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(ts)))
			{
				m_UpdateIndikator.SetActive(true);
			}
		}
		else if (flag)
		{
			CheckForNewMarker();
		}
	}

	public void CheckForNewMarker()
	{
		ShopBalancingData balancing = null;
		List<BasicShopOfferBalancingData> list = new List<BasicShopOfferBalancingData>();
		if (DIContainerBalancing.Service.TryGetBalancingData<ShopBalancingData>(base.Model.BalancingData.HotspotContents.Keys.FirstOrDefault(), out balancing))
		{
			list = DIContainerLogic.GetShopService().GetShopOffers(DIContainerInfrastructure.GetCurrentPlayer(), base.Model.BalancingData.HotspotContents.Keys.FirstOrDefault());
		}
		bool flag = false;
		if (m_ShopMenuType != ShopMenuType.Dojo)
		{
			foreach (BasicShopOfferBalancingData item in list)
			{
				if (DIContainerLogic.GetShopService().IsOfferShowable(DIContainerInfrastructure.GetCurrentPlayer(), item) && DIContainerLogic.GetShopService().IsOfferBuyableIgnoreCost(DIContainerInfrastructure.GetCurrentPlayer(), item))
				{
					flag = true;
					break;
				}
			}
		}
		if ((bool)m_UpdateIndikator)
		{
			m_UpdateIndikator.SetActive(flag);
		}
		if ((bool)m_SoldOutIndicator)
		{
			m_SoldOutIndicator.SetActive(!flag);
		}
	}

	public override void ShowContentView()
	{
		Requirement firstFailedReq = null;
		if (!DIContainerLogic.WorldMapService.CanTravelToHotspot(DIContainerInfrastructure.GetCurrentPlayer(), base.Model, out firstFailedReq))
		{
			if (firstFailedReq.RequirementType == RequirementType.HaveItem)
			{
				IInventoryItemGameData inventoryItemGameData = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, firstFailedReq.NameId, (int)firstFailedReq.Value);
				UIAtlas atlas = null;
				if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(inventoryItemGameData.ItemIconAtlasName))
				{
					GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(inventoryItemGameData.ItemIconAtlasName) as GameObject;
					atlas = gameObject.GetComponent<UIAtlas>();
				}
				if (!m_Bubble)
				{
					m_Bubble = DIContainerInfrastructure.LocationStateMgr.GetEmoteBubble(inventoryItemGameData.ItemAssetName, m_bubbleOffset, base.transform, atlas);
				}
			}
			return;
		}
		base.ShowContentView();
		switch (m_ShopMenuType)
		{
		case ShopMenuType.Workshop:
			DIContainerInfrastructure.LocationStateMgr.ShowWorkshopScreen(base.Model.BalancingData.HotspotContents.Keys.FirstOrDefault(), this);
			break;
		case ShopMenuType.Witchhut:
			DIContainerInfrastructure.LocationStateMgr.ShowWitchHutScreen(base.Model.BalancingData.HotspotContents.Keys.FirstOrDefault(), this);
			break;
		case ShopMenuType.Trainer:
			DIContainerInfrastructure.LocationStateMgr.ShowTrainerScreen(base.Model.BalancingData.HotspotContents.Keys.FirstOrDefault(), this);
			break;
		case ShopMenuType.Dojo:
			DIContainerInfrastructure.LocationStateMgr.ShowDojoScreen(base.Model.BalancingData.HotspotContents.Keys.FirstOrDefault(), this);
			break;
		}
	}

	public override IEnumerator ActivateFollowUpStagesAsync(HotSpotWorldMapViewBase parentHotSpot, HotSpotWorldMapViewBase activateTo, bool instant = false)
	{
		bool wasHidden = base.Model.Data.UnlockState < HotspotUnlockState.Resolved;
		yield return StartCoroutine(base.ActivateFollowUpStagesAsync(parentHotSpot, activateTo, instant));
		if (wasHidden && base.Model.Data.UnlockState == HotspotUnlockState.Resolved)
		{
			PlayActiveAnimation();
			CheckForNewMarker();
		}
		else if (base.Model.Data.UnlockState == HotspotUnlockState.Resolved)
		{
			PlayActiveIdleAnimation();
		}
	}
}
