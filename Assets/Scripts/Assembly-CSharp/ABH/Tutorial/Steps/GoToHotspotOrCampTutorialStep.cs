using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class GoToHotspotOrCampTutorialStep : BaseTutorialStep
	{
		private WorldMapMenuUI m_worldMapMenuUI;

		private HotSpotWorldMapViewBase m_targetHotspot;

		private HotSpotWorldMapViewBase m_campHotspot;

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
			m_targetHotspot = worldMapStateMgr.m_startingHotSpot.GetHotspotWorldMapView(value.BalancingData.NameId);
			if (m_targetHotspot.Model.IsActive())
			{
				WorldMapStateMgr worldMapStateMgr2 = Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
				if ((bool)worldMapStateMgr2)
				{
					m_campHotspot = worldMapStateMgr2.m_startingHotSpot;
					m_worldMapMenuUI = worldMapStateMgr2.m_WorldMenuUI;
					m_worldMapMenuUI.m_CampButton.Clicked -= OnCampButtonClicked;
					m_worldMapMenuUI.m_CampButton.Clicked += OnCampButtonClicked;
					DebugLog.Log("[Tutorial] Start Tutorial: " + m_TutorialIdent + " Step: " + GetType().ToString());
					m_targetHotspot.gameObject.layer = LayerMask.NameToLayer("TutorialScenery");
					m_campHotspot.gameObject.layer = LayerMask.NameToLayer("TutorialScenery");
					m_worldMapMenuUI.m_CampButton.gameObject.layer = LayerMask.NameToLayer("TutorialInterface");
					m_TutorialMgr.SetTutorialCameras(true);
					m_TutorialMgr.ShowHelp(m_targetHotspot.transform, TutorialStepType.GoToHotspotOrCamp.ToString(), 0f, 0f);
					m_campHotspot.HotspotClicked -= CampHotspotClicked;
					m_campHotspot.HotspotClicked += CampHotspotClicked;
					m_targetHotspot.HotspotClicked -= OnHotspotClicked;
					m_targetHotspot.HotspotClicked += OnHotspotClicked;
					m_Started = true;
				}
			}
		}

		private void CampHotspotClicked(HotSpotWorldMapViewBase obj)
		{
			DeregisterEventHandlers();
			m_targetHotspot.gameObject.layer = LayerMask.NameToLayer("Scenery");
			m_campHotspot.gameObject.layer = LayerMask.NameToLayer("Scenery");
			m_worldMapMenuUI.m_CampButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			m_TutorialMgr.SetTutorialCameras(false);
			FinishStepReset();
		}

		private void OnCampButtonClicked()
		{
			DeregisterEventHandlers();
			m_targetHotspot.gameObject.layer = LayerMask.NameToLayer("Scenery");
			m_campHotspot.gameObject.layer = LayerMask.NameToLayer("Scenery");
			m_worldMapMenuUI.m_CampButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			m_TutorialMgr.SetTutorialCameras(false);
			FinishStepReset();
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "hotspot_clicked") && ContainsParameter(parameters))
			{
				m_TutorialMgr.HideHelp(TutorialStepType.GoToHotspotOrCamp.ToString(), true);
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		protected void FinishStepReset()
		{
			m_TutorialMgr.HideHelp(TutorialStepType.GoToHotspotOrCamp.ToString(), false);
			m_TutorialMgr.ResetTutorial(m_TutorialIdent);
			m_Started = false;
		}

		protected override void ResetStep()
		{
			base.ResetStep();
			OnCampButtonClicked();
		}

		private void OnHotspotClicked(HotSpotWorldMapViewBase hotspot)
		{
			DeregisterEventHandlers();
			m_targetHotspot.gameObject.layer = LayerMask.NameToLayer("Scenery");
			m_campHotspot.gameObject.layer = LayerMask.NameToLayer("Scenery");
			m_worldMapMenuUI.m_CampButton.gameObject.layer = LayerMask.NameToLayer("Interface");
			m_TutorialMgr.SetTutorialCameras(false);
			FinishStep("hotspot_clicked", new List<string> { hotspot.Model.BalancingData.NameId });
		}

		private void DeregisterEventHandlers()
		{
			m_targetHotspot.HotspotClicked -= OnHotspotClicked;
			m_campHotspot.HotspotClicked -= CampHotspotClicked;
			m_worldMapMenuUI.m_CampButton.Clicked -= OnCampButtonClicked;
		}
	}
}
