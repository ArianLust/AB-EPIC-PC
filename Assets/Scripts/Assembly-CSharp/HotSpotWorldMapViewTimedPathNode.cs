using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class HotSpotWorldMapViewTimedPathNode : HotSpotWorldMapViewPathNode
{
	[SerializeField]
	protected List<UISprite> m_eventGateTimerFields;

	protected int m_latestUpdatedTime;

	protected bool m_countdownRunning;

	public bool m_timerVisible;

	[SerializeField]
	protected GameObject m_timerAnimation;

	private void Start()
	{
		PlayAnimationForCurrentState(false);
	}

	public override void HandleMouseButtonUp(bool directMove = false)
	{
		if (!m_active)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.ShowNonInteractableTooltip("TimerGate", "timed_gate_tt_header", "timed_gate_tt_header_tt_desc");
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
		if (GetComponent<Animation>() != null && GetComponent<Animation>()[AnimationPrefix + "Open"] != null)
		{
			yield return new WaitForSeconds(GetComponent<Animation>()[AnimationPrefix + "Open"].length);
		}
		HotSpotWorldMapViewBase nextGate = GetNextHotspotWhere((HotSpotWorldMapViewBase spot) => spot.m_MiniCampaignHotspot && (spot is HotSpotWorldMapViewTimedPathNode || spot is HotSpotWorldMapViewCampaignStarPathNode));
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
		if (m_MiniCampaignHotspot)
		{
			DebugLog.Warn(GetType(), "ActivateFollowUpStagesAsync: setting own status: " + base.name);
			if (base.Model.Data.UnlockState == HotspotUnlockState.Active)
			{
				DebugLog.Warn(GetType(), "ActivateFollowUpStagesAsync: picking animation with 'new' parameter set");
				SetAnimationStateForTimeLeft();
			}
			HotSpotWorldMapViewTimedPathNode nextGate = GetNextHotspotWhere((HotSpotWorldMapViewBase spot) => spot.m_MiniCampaignHotspot && spot is HotSpotWorldMapViewTimedPathNode) as HotSpotWorldMapViewTimedPathNode;
			if (nextGate != null)
			{
				DebugLog.Warn(GetType(), "ActivateFollowUpStagesAsync: picking animation for next gate named " + nextGate.name);
				nextGate.SetAnimationStateForTimeLeft();
			}
		}
	}

	public override void SynchBalancing()
	{
		base.SynchBalancing();
		m_active = false;
	}

	public void SetTimeLeft()
	{
		if (!m_MiniCampaignHotspot || m_eventGateTimerFields == null || m_eventGateTimerFields.Count < 6)
		{
			DebugLog.Log("HotSpotWorldMapViewPathNode: Trying to set timer on unsupported node: " + m_nameId);
		}
		else if (!m_countdownRunning && base.Model.Data.UnlockState < HotspotUnlockState.Resolved)
		{
			StartCoroutine(TimeLeftCountdown());
		}
	}

	protected IEnumerator TimeLeftCountdown()
	{
		m_countdownRunning = true;
		while (m_latestUpdatedTime >= 0 && base.Model.Data.UnlockState < HotspotUnlockState.Resolved)
		{
			Requirement req = base.Model.BalancingData.EnterRequirements.Where((Requirement r) => r.RequirementType == RequirementType.HaveItem && r.NameId == "event_gate_unlock").FirstOrDefault();
			int requiredUnlockItems = (int)req.Value;
			m_latestUpdatedTime = (int)DIContainerInfrastructure.EventSystemStateManager.GetCampaignGateOpenTimeLeft(requiredUnlockItems).TotalSeconds;
			int hoursInt = m_latestUpdatedTime / 3600;
			string hours = hoursInt.ToString("D2");
			int minutesInt = m_latestUpdatedTime % 3600 / 60;
			string minutes = minutesInt.ToString("D2");
			string seconds = (m_latestUpdatedTime % 60).ToString("D2");
			if (hoursInt <= 0 && minutesInt <= 15)
			{
				m_timerAnimation.PlayAnimationOrAnimatorState("Timer_Loop");
			}
			m_eventGateTimerFields[0].spriteName = hours[0].ToString();
			m_eventGateTimerFields[1].spriteName = hours[1].ToString();
			m_eventGateTimerFields[2].spriteName = minutes[0].ToString();
			m_eventGateTimerFields[3].spriteName = minutes[1].ToString();
			m_eventGateTimerFields[4].spriteName = seconds[0].ToString();
			m_eventGateTimerFields[5].spriteName = seconds[1].ToString();
			foreach (UISprite field2 in m_eventGateTimerFields)
			{
				field2.MarkAsChanged();
			}
			yield return new WaitForSeconds(1f);
		}
		if (m_latestUpdatedTime <= 0)
		{
			foreach (UISprite field in m_eventGateTimerFields)
			{
				field.spriteName = "0";
				field.MarkAsChanged();
			}
			CountdownFinishedHandler();
		}
		else
		{
			m_timerAnimation.PlayAnimationOrAnimatorState("Timer_Stop");
		}
	}

	private void CountdownFinishedHandler()
	{
		SetAnimationStateForTimeLeft();
		if (base.Model.Data.UnlockState == HotspotUnlockState.Active)
		{
			ShowContentView();
		}
		HotSpotWorldMapViewTimedPathNode hotSpotWorldMapViewTimedPathNode = GetNextHotspotWhere((HotSpotWorldMapViewBase spot) => spot.m_MiniCampaignHotspot && spot is HotSpotWorldMapViewTimedPathNode) as HotSpotWorldMapViewTimedPathNode;
		if (hotSpotWorldMapViewTimedPathNode != null)
		{
			hotSpotWorldMapViewTimedPathNode.m_timerVisible = true;
			hotSpotWorldMapViewTimedPathNode.SetAnimationStateForTimeLeft();
		}
	}

	public int SetAnimationState(HotspotAnimationState newState)
	{
		int num = 0;
		HotspotAnimationState animationState = base.Model.Data.AnimationState;
		if (newState > animationState)
		{
			base.Model.Data.AnimationState = newState;
		}
		if (animationState != base.Model.Data.AnimationState)
		{
			PlayAnimationForCurrentState(true);
			num = 1;
		}
		else
		{
			PlayAnimationForCurrentState(false);
			num = 0;
		}
		if (base.Model.Data.UnlockState == HotspotUnlockState.Active)
		{
			ShowContentView();
		}
		if (base.Model.Data.AnimationState == HotspotAnimationState.Active)
		{
			SetTimeLeft();
		}
		return num;
	}

	public void PlayAnimationForCurrentState(bool inTransition)
	{
		string text = AnimationPrefix;
		switch (base.Model.Data.AnimationState)
		{
		case HotspotAnimationState.None:
		case HotspotAnimationState.Inactive:
			text += "SetInactive";
			break;
		case HotspotAnimationState.Active:
			text += ((!inTransition) ? "SetActive" : "Activate");
			break;
		case HotspotAnimationState.Open:
			text += ((!inTransition) ? "SetOpen" : "Open");
			break;
		}
		base.gameObject.PlayAnimationOrAnimatorState(text);
	}

	public int SetAnimationStateForTimeLeft()
	{
		int numberOfCampaignGateUnlocks = DIContainerInfrastructure.EventSystemStateManager.GetNumberOfCampaignGateUnlocks();
		Requirement requirement = base.Model.BalancingData.EnterRequirements.Where((Requirement r) => r.RequirementType == RequirementType.HaveItem && r.NameId == "event_gate_unlock").FirstOrDefault();
		int num = (int)requirement.Value;
		HotspotAnimationState hotspotAnimationState = HotspotAnimationState.None;
		hotspotAnimationState = ((num <= numberOfCampaignGateUnlocks) ? HotspotAnimationState.Open : ((!m_timerVisible) ? HotspotAnimationState.Inactive : HotspotAnimationState.Active));
		return SetAnimationState(hotspotAnimationState);
	}
}
