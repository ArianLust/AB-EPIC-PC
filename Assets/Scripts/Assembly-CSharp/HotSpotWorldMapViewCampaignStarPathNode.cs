using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class HotSpotWorldMapViewCampaignStarPathNode : HotSpotWorldMapViewPathNode
{
	[SerializeField]
	protected List<UISprite> m_eventStarFields;

	private void Start()
	{
		if (base.Model != null)
		{
			if (base.Model.Data.AnimationState < HotspotAnimationState.Open)
			{
				base.gameObject.PlayAnimationOrAnimatorState(AnimationPrefix + "SetActive");
			}
			else
			{
				base.gameObject.PlayAnimationOrAnimatorState(AnimationPrefix + "SetOpen");
			}
			Requirement requirement = base.Model.BalancingData.EnterRequirements.FirstOrDefault();
			bool flag = DIContainerLogic.RequirementService.CheckGenericRequirements(DIContainerInfrastructure.GetCurrentPlayer(), base.Model.BalancingData.EnterRequirements);
			int itemValue = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, requirement.NameId);
			m_eventStarFields[0].spriteName = (itemValue / 10).ToString();
			m_eventStarFields[1].spriteName = (itemValue % 10).ToString();
			m_eventStarFields[2].spriteName = ((int)requirement.Value / 10).ToString();
			m_eventStarFields[3].spriteName = ((int)requirement.Value % 10).ToString();
		}
	}

	public override void ShowTooltip()
	{
		if (ShowTooltipIfNotActive || base.Model.Data.UnlockState >= HotspotUnlockState.Active)
		{
			string localizedText = DIContainerInfrastructure.GetLocaService().Tr(m_overrideTooltip).Replace("{value_1}", GetNumberOfStarsRequired().ToString());
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(base.transform, localizedText, false);
		}
	}

	public override void HandleMouseButtonUp(bool directMove = false)
	{
		if (!m_active)
		{
			Requirement requirement = base.Model.BalancingData.EnterRequirements.FirstOrDefault();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{value_1}", requirement.Value.ToString());
			Dictionary<string, string> replacementDictionary = dictionary;
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.ShowNonInteractableTooltip("StarGate", "star_gate_tt_header", "star_gate_tt_header_tt_desc", 3f, 0f, replacementDictionary);
		}
		else
		{
			ShowContentView();
		}
	}

	public override void Unlock()
	{
		if (!IsCompleted())
		{
			DIContainerLogic.WorldMapService.UnlockHotSpotInstant(DIContainerInfrastructure.GetCurrentPlayer(), base.Model);
			StartCoroutine("UnlockCoroutine");
		}
	}

	private IEnumerator UnlockCoroutine()
	{
		HotSpotWorldMapViewTimedPathNode nextGate = GetNextHotspotWhere((HotSpotWorldMapViewBase spot) => spot.m_MiniCampaignHotspot && spot is HotSpotWorldMapViewTimedPathNode) as HotSpotWorldMapViewTimedPathNode;
		if (nextGate == null)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.Balancing.EventId == "event_campaign_movie")
			{
				DIContainerLogic.InventoryService.SetItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "unlock_eventcampaign_moviedungeon", 1, "last_gate_unlocked");
			}
			else
			{
				DIContainerLogic.InventoryService.SetItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, 1, 1, "unlock_eventcampaign_dungeon", 1, "last_gate_unlocked");
			}
		}
		yield return StartCoroutine(ActivateFollowUpStagesAsync(null, null));
	}

	public override IEnumerator ActivateFollowUpStagesAsync(HotSpotWorldMapViewBase parentHotSpot, HotSpotWorldMapViewBase activateTo, bool instant = false)
	{
		yield return StartCoroutine(base.ActivateFollowUpStagesAsync(parentHotSpot, activateTo, instant));
		if (m_MiniCampaignHotspot && base.Model.Data.UnlockState == HotspotUnlockState.Active)
		{
			ShowContentView();
		}
	}

	public override void SynchBalancing()
	{
		base.SynchBalancing();
		m_active = false;
	}

	private int GetNumberOfStarsRequired()
	{
		Requirement requirement = base.Model.BalancingData.EnterRequirements.FirstOrDefault();
		return (int)requirement.Value;
	}

	public void SetStarModelAndState()
	{
		DebugLog.Log(GetType(), "SetStarModel");
		Requirement requirement = base.Model.BalancingData.EnterRequirements.FirstOrDefault();
		if (DIContainerLogic.RequirementService.CheckGenericRequirements(DIContainerInfrastructure.GetCurrentPlayer(), base.Model.BalancingData.EnterRequirements))
		{
			if (base.Model.Data.AnimationState < HotspotAnimationState.Open)
			{
				base.gameObject.PlayAnimationOrAnimatorState(AnimationPrefix + "Open");
				base.Model.Data.AnimationState = HotspotAnimationState.Open;
			}
			else
			{
				base.gameObject.PlayAnimationOrAnimatorState(AnimationPrefix + "SetOpen");
			}
			if (base.Model.Data.UnlockState >= HotspotUnlockState.Active)
			{
				DebugLog.Log(GetType(), string.Concat("SetStarModel: StarGate is ", base.Model.Data.UnlockState, ". Showing Content View."));
				ShowContentView();
				return;
			}
		}
		int itemValue = DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, requirement.NameId);
		DebugLog.Log(GetType(), "SetStarModelAndState: required number of " + requirement.NameId + " is  " + requirement.Value + ". Currently in inventory = " + itemValue);
		m_eventStarFields[0].spriteName = (itemValue / 10).ToString();
		m_eventStarFields[1].spriteName = (itemValue % 10).ToString();
		m_eventStarFields[2].spriteName = ((int)requirement.Value / 10).ToString();
		m_eventStarFields[3].spriteName = ((int)requirement.Value % 10).ToString();
	}
}
