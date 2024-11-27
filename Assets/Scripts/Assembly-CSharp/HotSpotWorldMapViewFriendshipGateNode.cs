using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.Generic;
using ABH.Shared.Models;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class HotSpotWorldMapViewFriendshipGateNode : HotSpotWorldMapViewBase
{
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
		if (base.Model.Data.UnlockState > HotspotUnlockState.ResolvedNew)
		{
			return true;
		}
		foreach (HotSpotWorldMapViewBase outgoingHotSpot in m_outgoingHotSpots)
		{
			if (outgoingHotSpot.Model.BalancingData.Type == HotspotType.Battle && outgoingHotSpot.IsCompleted())
			{
				FillWithNPCs();
				base.Model.Data.UnlockState = HotspotUnlockState.Resolved;
				return true;
			}
			if (outgoingHotSpot.Model.BalancingData.Type != HotspotType.Node)
			{
				continue;
			}
			foreach (HotSpotWorldMapViewBase outgoingHotSpot2 in outgoingHotSpot.m_outgoingHotSpots)
			{
				if (outgoingHotSpot2.Model.BalancingData.Type == HotspotType.Battle && outgoingHotSpot2.IsCompleted())
				{
					FillWithNPCs();
					base.Model.Data.UnlockState = HotspotUnlockState.Resolved;
					return true;
				}
			}
		}
		return false;
	}

	private void FillWithNPCs()
	{
		FriendData lowNPCFriend = DIContainerLogic.SocialService.GetLowNPCFriend(DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
		for (int i = 0; i < 5; i++)
		{
			if (!DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks.ContainsKey(base.Model.BalancingData.NameId))
			{
				DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks.Add(base.Model.BalancingData.NameId, new List<string>());
			}
			DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.FriendShipGateUnlocks[base.Model.BalancingData.NameId].Add(lowNPCFriend.Id);
		}
	}

	protected override void InitialSetupHotspot()
	{
		base.InitialSetupHotspot();
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.GetAndSetFriendshipGateLockState(base.Model);
		if (base.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew)
		{
			GetComponent<Animation>().Play(AnimationPrefix + "Ready");
		}
		else if (IsCompleted())
		{
			GetComponent<Animation>().Play(AnimationPrefix + "SetOpen");
		}
	}

	protected override void HotSpotChanged(bool startUpSetting)
	{
		base.HotSpotChanged(startUpSetting);
		if (base.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew)
		{
			GetComponent<Animation>().Play(AnimationPrefix + "Ready");
		}
	}

	public override void ShowContentView()
	{
		base.ShowContentView();
		Requirement requirement = null;
		if (base.Model.BalancingData.EnterRequirements != null && base.Model.BalancingData.EnterRequirements.Any((Requirement r) => r.RequirementType == RequirementType.UsedFriends))
		{
			DIContainerInfrastructure.LocationStateMgr.ShowFriendshipGateScreen(Unlock, base.Model);
		}
		else if (requirement.RequirementType == RequirementType.HaveItem)
		{
			IInventoryItemGameData inventoryItemGameData = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, requirement.NameId, (int)requirement.Value);
			UIAtlas uIAtlas = null;
			if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(inventoryItemGameData.ItemIconAtlasName))
			{
				GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(inventoryItemGameData.ItemIconAtlasName) as GameObject;
				uIAtlas = gameObject.GetComponent<UIAtlas>();
			}
		}
	}

	public override void HandleMouseButtonUp(bool directMove = false)
	{
		if (base.Model.Data.UnlockState < HotspotUnlockState.Active)
		{
			string iconAssetID = "Friendship";
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.ShowNonInteractableTooltip(iconAssetID, "friendship_tt_header", "friendship_tt_desc");
		}
		if (m_active && DIContainerInfrastructure.LocationStateMgr.IsShowContentPossible())
		{
			ShowContentView();
		}
	}

	public override void SynchBalancing()
	{
		base.SynchBalancing();
		m_active = false;
	}

	public void Unlock()
	{
		if (base.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew)
		{
			DIContainerLogic.NotificationPopupController.RequestNotificationPopupForReason(NotificationPopupTrigger.SocialGate);
			DIContainerLogic.WorldMapService.UnlockHotSpotInstant(DIContainerInfrastructure.GetCurrentPlayer(), base.Model);
			GetComponent<Animation>().Play(AnimationPrefix + "Open");
			StartCoroutine(ActivateFollowUpStagesAsync(null, null));
		}
	}
}
