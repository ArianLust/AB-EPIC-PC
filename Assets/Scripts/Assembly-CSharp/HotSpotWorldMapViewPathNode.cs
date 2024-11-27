using System.Collections;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class HotSpotWorldMapViewPathNode : HotSpotWorldMapViewBase
{
	[SerializeField]
	protected Vector3 m_bubbleOffset = Vector3.zero;

	protected GameObject m_Bubble;

	[SerializeField]
	protected string m_overrideTooltip;

	[SerializeField]
	protected bool ShowTooltipIfNotActive;

	[SerializeField]
	protected GameObject[] m_activateObjects;

	public override void ActivateAsset(bool activate)
	{
		GameObject[] activateObjects = m_activateObjects;
		foreach (GameObject gameObject in activateObjects)
		{
			gameObject.SetActive(true);
		}
	}

	public virtual void ShowTooltip()
	{
		if (ShowTooltipIfNotActive || base.Model.Data.UnlockState >= HotspotUnlockState.Active)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(base.transform, DIContainerInfrastructure.GetLocaService().Tr(m_overrideTooltip), false);
		}
	}

	protected override void InitialSetupHotspot()
	{
		base.InitialSetupHotspot();
		if (m_active && (base.Model.BalancingData.EnterRequirements == null || base.Model.BalancingData.EnterRequirements.Count == 0))
		{
			Unlock();
		}
		else if (IsCompleted() && (bool)GetComponent<Animation>())
		{
			GetComponent<Animation>().Play(AnimationPrefix + "SetOpen");
		}
	}

	private bool IsKeyRequirementMet()
	{
		Requirement firstFailedReq = null;
		if (DIContainerLogic.WorldMapService.CanTravelToHotspot(DIContainerInfrastructure.GetCurrentPlayer(), base.Model, out firstFailedReq))
		{
			return true;
		}
		return false;
	}

	public override void ShowContentView()
	{
		base.ShowContentView();
		Requirement firstFailedReq = null;
		if (base.Model.BalancingData.EnterRequirements != null && base.Model.BalancingData.EnterRequirements.Any((Requirement r) => r.RequirementType == RequirementType.UsedFriends))
		{
			DIContainerInfrastructure.LocationStateMgr.ShowFriendshipGateScreen(Unlock, base.Model);
		}
		else if (DIContainerLogic.WorldMapService.CanTravelToHotspot(DIContainerInfrastructure.GetCurrentPlayer(), base.Model, out firstFailedReq))
		{
			Unlock();
		}
		else if (firstFailedReq.RequirementType == RequirementType.HaveItem)
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
	}

	public override void HandleMouseButtonUp(bool directMove = false)
	{
		if (base.Model.Data.UnlockState < HotspotUnlockState.Active || !IsKeyRequirementMet())
		{
			string requiredKeyAssetName = GetRequiredKeyAssetName();
			if (string.IsNullOrEmpty(requiredKeyAssetName))
			{
				return;
			}
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.ShowNonInteractableTooltip(requiredKeyAssetName, requiredKeyAssetName.ToLower() + "_tt_header", requiredKeyAssetName.ToLower() + "_tt_desc");
		}
		if (m_active)
		{
			ShowContentView();
		}
	}

	private string GetRequiredKeyAssetName()
	{
		switch (base.Model.BalancingData.EnterRequirements.FirstOrDefault().NameId)
		{
		case "key_blue":
			return "PigKey_Blue";
		case "key_yellow":
			return "PigKey_Yellow";
		case "key_red":
			return "PigKey_Red";
		default:
			return string.Empty;
		}
	}

	public virtual void Unlock()
	{
		if (!IsCompleted())
		{
			DIContainerLogic.WorldMapService.UnlockHotSpotInstant(DIContainerInfrastructure.GetCurrentPlayer(), base.Model);
			StartCoroutine("UnlockCoroutine");
		}
	}

	private IEnumerator UnlockCoroutine()
	{
		if (GetComponent<Animation>() != null && GetComponent<Animation>()[AnimationPrefix + "Open"] != null)
		{
			GetComponent<Animation>().Play(AnimationPrefix + "Open");
			yield return new WaitForSeconds(GetComponent<Animation>()[AnimationPrefix + "Open"].length);
		}
		yield return StartCoroutine(ActivateFollowUpStagesAsync(null, null));
	}

	public override IEnumerator ActivateFollowUpStagesAsync(HotSpotWorldMapViewBase parentHotSpot, HotSpotWorldMapViewBase activateTo, bool instant = false)
	{
		yield return StartCoroutine(base.ActivateFollowUpStagesAsync(parentHotSpot, activateTo, instant));
	}

	public override void SynchBalancing()
	{
		base.SynchBalancing();
		m_active = false;
	}
}
