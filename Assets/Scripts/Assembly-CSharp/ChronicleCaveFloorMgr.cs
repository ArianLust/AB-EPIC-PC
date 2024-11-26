using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class ChronicleCaveFloorMgr : ChronicleCaveFloorSlot
{
	[SerializeField]
	private Transform m_BossRoot;

	private Action m_ActionAfterWalkingDone;

	[SerializeField]
	private UILabel m_CurrentCaveCutsceneLabel;

	public bool Loaded;

	[SerializeField]
	private ActionTree m_IntroCutscene;

	[SerializeField]
	public GameObject m_Gate;

	public float m_BossAnimationInterval = 4f;

	private CharacterControllerWorldMap m_CharacterControllerWorldMap;

	private SkillGameData m_EnvironmentalSkill;

	public void ShowCurrentCaveEnvironmentalEffectTooltip()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(m_BossRoot, m_EnvironmentalSkill.GenerateSkillBattleData().GetLocalizedName(), m_EnvironmentalSkill.GenerateSkillBattleData().GetLocalizedDescription(null), false);
	}

	private void SynchBalancing(HotSpotWorldMapViewBase[] hotspots)
	{
		foreach (HotSpotWorldMapViewBase hotSpotWorldMapViewBase in hotspots)
		{
			hotSpotWorldMapViewBase.SynchBalancing();
		}
	}

	private void Awake()
	{
		m_StateMgr = DIContainerInfrastructure.LocationStateMgr as ChronicleCaveStateMgr;
		base.transform.parent = m_StateMgr.m_FloorRoot;
		m_StateMgr.m_CurrentFloor = this;
		m_Model = m_StateMgr.m_CurrentCronicleCaveFloor;
		m_EnvironmentalSkill = new SkillGameData(m_Model.BalancingData.EnvironmentalEffects.Values.FirstOrDefault());
		m_StateMgr.m_Floors.Add(this);
		base.gameObject.name = m_Model.BalancingData.NameId + "_" + base.gameObject.name;
		HotspotGameData hotspotGameData = m_Model.HotspotGameDatas.Values.FirstOrDefault((HotspotGameData h) => h.BalancingData.Type == HotspotType.Battle);
		if (hotspotGameData != null)
		{
			m_CurrentCaveCutsceneLabel.text = DIContainerInfrastructure.GetLocaService().Tr("cc_floorunlocked", new Dictionary<string, string>
			{
				{
					"{value_1}",
					m_StateMgr.m_Floors.Count.ToString("0")
				},
				{
					"{value_2}",
					DIContainerInfrastructure.GetLocaService().GetZoneName(hotspotGameData.BalancingData.ZoneLocaIdent)
				}
			});
		}
		base.transform.localPosition += (m_StateMgr.m_Floors.Count - 1) * m_StateMgr.m_UpperFloorPosition;
		if (m_StateMgr.m_CurrentCronicleCaveFloor != null && m_StateMgr.m_CurrentCronicleCaveFloor.IsFinished() && DIContainerBalancing.Service.GetBalancingDataList<ChronicleCaveFloorBalancingData>().Count > m_StateMgr.m_Floors.Count)
		{
			m_StateMgr.InitNextIfDone(m_StateMgr.m_Floors.Count);
		}
		else if (DIContainerBalancing.Service.GetBalancingDataList<ChronicleCaveFloorBalancingData>().Count > m_StateMgr.m_Floors.Count)
		{
			m_StateMgr.InitNextProxyFloor(m_StateMgr.m_Floors.Count);
		}
		HotSpotWorldMapViewBase[] componentsInChildren = base.transform.GetComponentsInChildren<HotSpotWorldMapViewBase>(true);
		SynchBalancing(componentsInChildren);
		HotSpotWorldMapViewBase[] array = componentsInChildren;
		foreach (HotSpotWorldMapViewBase hotSpotWorldMapViewBase in array)
		{
			hotSpotWorldMapViewBase.SetChronicleCave(m_Model);
		}
		if ((bool)m_BossRoot)
		{
			m_CharacterControllerWorldMap = UnityEngine.Object.Instantiate(m_StateMgr.m_WorldMapCharacterController, m_BossRoot.position, Quaternion.identity) as CharacterControllerWorldMap;
			m_CharacterControllerWorldMap.transform.parent = m_BossRoot;
			m_CharacterControllerWorldMap.SetModel(m_Model.BalancingData.BossNameId, false);
			m_CharacterControllerWorldMap.transform.localScale = Vector3.Scale(m_CharacterControllerWorldMap.transform.localScale, DIContainerInfrastructure.LocationStateMgr.GetWorldBirdScale());
			if (m_Model.IsFinished())
			{
				StartCoroutine(BossCrying(m_CharacterControllerWorldMap));
			}
			else
			{
				StartCoroutine(BossTaunting(m_CharacterControllerWorldMap));
			}
		}
	}

	public override void InstantiateRedKeyBubble()
	{
		if (!m_LeavingHotSpot)
		{
			return;
		}
		string firstPossibleBattle = DIContainerLogic.GetBattleService().GetFirstPossibleBattle(m_LeavingHotSpot.Model.BalancingData.BattleId, DIContainerInfrastructure.GetCurrentPlayer(), true);
		ChronicleCaveBattleBalancingData balancing = null;
		if (!string.IsNullOrEmpty(firstPossibleBattle) && DIContainerBalancing.Service.TryGetBalancingData<ChronicleCaveBattleBalancingData>(firstPossibleBattle, out balancing) && balancing.LootTableAdditional != null && balancing.LootTableAdditional.ContainsKey("key_red"))
		{
			DebugLog.Log("[ChronicleCave] Spawn Red Key Bubble!");
			GameObject gameObject = UnityEngine.Object.Instantiate(m_StateMgr.m_RedKeyBubble);
			if (gameObject != null)
			{
				gameObject.transform.parent = m_CharacterControllerWorldMap.transform;
				m_CharacterControllerWorldMap.PositionBubble(gameObject);
				gameObject.transform.localScale = new Vector3(1f / gameObject.transform.lossyScale.x, 1f / gameObject.transform.lossyScale.y, 1f / gameObject.transform.lossyScale.z);
				gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -400f);
			}
		}
	}

	public override void SetFriendProgressMarker(ChronicleCaveFloorBalancingData ccfbd, FriendProgressIndicator fpi, HotspotBalancingData hotspot)
	{
		HotSpotWorldMapViewBase hotspotWorldMapView = m_EnteringHotSpot.GetHotspotWorldMapView(hotspot.NameId);
		fpi.transform.parent = hotspotWorldMapView.transform;
		fpi.transform.localPosition = Vector3.zero;
		fpi.GetComponent<Animation>().Play("FriendMarker_Show");
		fpi.GetComponent<Animation>().PlayQueued("FriendMarker_Idle");
	}

	private IEnumerator BossCrying(CharacterControllerWorldMap boss)
	{
		yield return new WaitForSeconds(m_BossAnimationInterval);
		yield return new WaitForSeconds(boss.PlayMourneAnimation());
		StartCoroutine(BossCrying(boss));
	}

	private IEnumerator BossTaunting(CharacterControllerWorldMap boss)
	{
		yield return new WaitForSeconds(m_BossAnimationInterval);
		yield return new WaitForSeconds(boss.PlayTauntAnimtation());
		StartCoroutine(BossTaunting(boss));
	}

	public override IEnumerator ActivateHotspots()
	{
		yield return StartCoroutine(m_EnteringHotSpot.ActivateFollowUpStagesAsync(null, null));
	}

	public override ActionTree GetIntroCutscene()
	{
		return m_IntroCutscene;
	}

	public override void HideBoss(bool hide)
	{
		m_BossRoot.gameObject.SetActive(!hide);
	}

	public override GameObject GetGate()
	{
		return m_Gate;
	}
}
