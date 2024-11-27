using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class GoToHotspotTutorialStep : BaseTutorialStep
	{
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
			if (DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(list[0], out value) && value.Data.UnlockState != HotspotUnlockState.Hidden)
			{
				DebugLog.Log("[Tutorial] Try Start Tutorial: " + m_TutorialIdent + " Step: " + GetType().ToString());
				WorldMapStateMgr worldMapStateMgr = Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
				HotSpotWorldMapViewBase hotspotWorldMapView = worldMapStateMgr.m_startingHotSpot.GetHotspotWorldMapView(value.BalancingData.NameId);
				if (hotspotWorldMapView.Model.IsActive())
				{
					DebugLog.Log("[Tutorial] Start Tutorial: " + m_TutorialIdent + " Step: " + GetType().ToString());
					hotspotWorldMapView.gameObject.layer = LayerMask.NameToLayer("TutorialScenery");
					m_TutorialMgr.SetTutorialCameras(true);
					m_TutorialMgr.ShowHelp(hotspotWorldMapView.transform, TutorialStepType.GoToHotspot.ToString(), 0f, 0f);
					hotspotWorldMapView.HotspotClicked -= OnHotspotClicked;
					hotspotWorldMapView.HotspotClicked += OnHotspotClicked;
					m_Started = true;
				}
			}
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "hotspot_clicked") && ContainsParameter(parameters))
			{
				m_TutorialMgr.HideHelp(TutorialStepType.GoToHotspot.ToString(), true);
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void OnHotspotClicked(HotSpotWorldMapViewBase hotspot)
		{
			hotspot.HotspotClicked -= OnHotspotClicked;
			hotspot.gameObject.layer = LayerMask.NameToLayer("Scenery");
			m_TutorialMgr.SetTutorialCameras(false);
			FinishStep("hotspot_clicked", new List<string> { hotspot.Model.BalancingData.NameId });
		}
	}
}
