using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class HarvestTutorialStep : BaseTutorialStep
	{
		private HotSpotWorldMapViewResource hotSpotWorldMapView;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
			m_ResetTrigger = "balancing_reload";
			return base.SetupStep(allowedTrigger, tutorialIdent, possibleParams, autoStart);
		}

		protected override void StartStep(string trigger, List<string> parameters)
		{
			if (trigger != "unlocked_hotspot" && trigger != "enter_worldmap" && trigger != "triggered_forced")
			{
				return;
			}
			List<string> list = parameters;
			switch (trigger)
			{
			case "unlocked_hotspot":
				if (!ContainsParameter(parameters))
				{
					return;
				}
				break;
			case "enter_worldmap":
			case "triggered_forced":
				list = m_possibleParams;
				break;
			}
			HotspotGameData value = null;
			if (!DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(list[0], out value) || value.Data.UnlockState == HotspotUnlockState.Hidden)
			{
				return;
			}
			DebugLog.Log("[Tutorial] Try Start Tutorial: " + m_TutorialIdent + " Step: " + GetType().ToString());
			WorldMapStateMgr worldMapStateMgr = Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
			HotSpotWorldMapViewBase hotspotWorldMapView = worldMapStateMgr.m_startingHotSpot.GetHotspotWorldMapView(value.BalancingData.NameId);
			if (hotspotWorldMapView.Model.Data.UnlockState == HotspotUnlockState.Hidden)
			{
				return;
			}
			if (hotspotWorldMapView.Model.Data.UnlockState > HotspotUnlockState.ResolvedNew)
			{
				FinishStep("hotspot_harvested", new List<string> { hotspotWorldMapView.Model.BalancingData.NameId });
				return;
			}
			DebugLog.Log("[Tutorial] Start Tutorial: " + m_TutorialIdent + " Step: " + GetType().ToString());
			hotSpotWorldMapView = hotspotWorldMapView as HotSpotWorldMapViewResource;
			if ((bool)hotSpotWorldMapView)
			{
				hotspotWorldMapView.gameObject.layer = LayerMask.NameToLayer("TutorialScenery");
				m_TutorialMgr.SetTutorialCameras(true);
				m_TutorialMgr.ShowHelp(hotspotWorldMapView.transform, TutorialStepType.Harvest.ToString(), 0f, 0f);
				hotSpotWorldMapView.OnDespawnResource -= OnDespawnResource;
				hotSpotWorldMapView.OnDespawnResource += OnDespawnResource;
				m_Started = true;
			}
		}

		private void OnDespawnResource()
		{
			if ((bool)hotSpotWorldMapView)
			{
				hotSpotWorldMapView.OnDespawnResource -= OnDespawnResource;
				hotSpotWorldMapView.gameObject.layer = LayerMask.NameToLayer("Scenery");
			}
			m_TutorialMgr.SetTutorialCameras(false);
			FinishStep("hotspot_harvested", new List<string> { hotSpotWorldMapView.Model.BalancingData.NameId });
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "hotspot_harvested") && ContainsParameter(parameters))
			{
				m_TutorialMgr.HideHelp(TutorialStepType.Harvest.ToString(), true);
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}
	}
}
