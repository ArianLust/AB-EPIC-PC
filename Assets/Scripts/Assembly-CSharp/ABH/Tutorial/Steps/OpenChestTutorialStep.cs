using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

namespace ABH.Tutorial.Steps
{
	public class OpenChestTutorialStep : BaseTutorialStep
	{
		private ChestController m_chest;

		public override ITutorialStep SetupStep(string allowedTrigger, string tutorialIdent, List<string> possibleParams, bool autoStart)
		{
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
			if (DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(list[0], out value) && value.Data.UnlockState >= HotspotUnlockState.ResolvedNew)
			{
				DebugLog.Log("[Tutorial] Try Start Tutorial: " + m_TutorialIdent + " Step: " + GetType().ToString());
				WorldMapStateMgr worldMapStateMgr = Object.FindObjectOfType(typeof(WorldMapStateMgr)) as WorldMapStateMgr;
				HotSpotWorldMapViewBase hotspotWorldMapView = worldMapStateMgr.m_startingHotSpot.GetHotspotWorldMapView(value.BalancingData.NameId);
				ChestPositioning componentInChildren = hotspotWorldMapView.GetComponentInChildren<ChestPositioning>();
				if ((bool)componentInChildren)
				{
					m_chest = componentInChildren.m_ChestButton;
				}
				if ((bool)m_chest)
				{
					DebugLog.Log("[Tutorial] Start Tutorial: " + m_TutorialIdent + " Step: " + GetType().ToString());
					m_chest.Clicked -= OnChestClicked;
					m_chest.Clicked += OnChestClicked;
					AddHelpersAndBlockers();
					m_Started = true;
				}
			}
		}

		private void AddHelpersAndBlockers()
		{
			m_chest.gameObject.layer = LayerMask.NameToLayer("TutorialScenery");
			m_TutorialMgr.SetTutorialCameras(true);
			m_TutorialMgr.ShowHelp(m_chest.transform, TutorialStepType.OpenChest.ToString(), 0f, 0f);
		}

		protected override void FinishStep(string trigger, List<string> parameters)
		{
			if (!(trigger != "chest_clicked"))
			{
				RemoveHelpersAndBlockers();
				m_TutorialMgr.FinishTutorialStep(m_TutorialIdent);
				m_Started = false;
			}
		}

		private void OnChestClicked()
		{
			m_chest.Clicked -= OnChestClicked;
			FinishStep("chest_clicked", new List<string>());
		}

		private void RemoveHelpersAndBlockers()
		{
			if ((bool)m_chest)
			{
				m_chest.gameObject.layer = LayerMask.NameToLayer("Scenery");
			}
			m_TutorialMgr.SetTutorialCameras(false);
			m_TutorialMgr.HideHelp(TutorialStepType.OpenChest.ToString(), true);
		}
	}
}
