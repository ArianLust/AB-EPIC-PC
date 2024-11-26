using System.Collections.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class PointOutNextHotspotTutorialStep : BaseTutorialStep
	{
		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_ResetTrigger = "reset_indicator";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if ((trigger != "unlocked_hotspot" && trigger != "enter_worldmap" && trigger != "triggered_forced") || (trigger == "unlocked_hotspot" && !ContainsParameter(parameters)))
			{
				return;
			}
			DebugLog.Log("[Tutorial] Try Start Tutorial: " + m_TutorialIdent + " Step: " + GetType().ToString());
			WorldMapStateMgr worldMapStateMgr = Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
			if (!worldMapStateMgr || worldMapStateMgr.m_ProgressIndicatorBlocked)
			{
				return;
			}
			HotSpotWorldMapViewBase hotSpotWorldMapViewBase = worldMapStateMgr.GetNextProgressHotspot(DIContainerInfrastructure.GetCurrentPlayer());
			if (hotSpotWorldMapViewBase == null)
			{
				DebugLog.Log(GetType(), "StartStep: No progress node found to go to. Maybe user has already unlocked all hotspots!");
				return;
			}
			if (!hotSpotWorldMapViewBase.Model.IsActive() || !hotSpotWorldMapViewBase.Model.BalancingData.UseProgressIndicator)
			{
				if (!(hotSpotWorldMapViewBase.Model.BalancingData.NameId == "hotspot_042_battleground") || hotSpotWorldMapViewBase.Model.IsActive())
				{
					return;
				}
				hotSpotWorldMapViewBase = worldMapStateMgr.m_startingHotSpot.GetHotspotWorldMapView("hotspot_041_piggate_yellow");
			}
			worldMapStateMgr.TweenCameraToTransform(hotSpotWorldMapViewBase.transform);
			DebugLog.Log("[Tutorial] Start Tutorial: " + m_TutorialIdent + " Step: " + GetType().ToString());
			m_TutorialMgr.ShowHelp(hotSpotWorldMapViewBase.transform, TutorialStepType.PointOutNextHotspot.ToString(), 0f, 0f);
			hotSpotWorldMapViewBase.HotspotClicked -= OnHotspotClicked;
			hotSpotWorldMapViewBase.HotspotClicked += OnHotspotClicked;
			m_Started = true;
		}

		protected override void ResetStep()
		{
			base.ResetStep();
			m_Started = false;
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "hotspot_clicked"))
			{
				m_TutorialMgr.HideHelp(TutorialStepType.PointOutNextHotspot.ToString(), true);
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void OnHotspotClicked(HotSpotWorldMapViewBase hotspot)
		{
			hotspot.HotspotClicked -= OnHotspotClicked;
			FinishStep("hotspot_clicked", new List<string> { hotspot.Model.BalancingData.NameId });
		}
	}
}
