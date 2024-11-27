using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class HotSpotWorldMapViewBase : MonoBehaviour
{
	public TransportMeaningType m_TransportMeaning;

	public Vector3[] m_HotSpotPositions = new Vector3[6];

	[SerializeField]
	public RageSplineLite m_litePath;

	[SerializeField]
	public HotSpotWorldMapViewBase m_previousHotSpot;

	[SerializeField]
	public List<HotSpotWorldMapViewBase> m_outgoingHotSpots;

	[SerializeField]
	public string m_nameId;

	public bool m_ChronicleCaveHotspot;

	public bool m_MiniCampaignHotspot;

	private ChronicleCaveFloorGameData m_ChronicleCaveFloor;

	public int ChronicleCaveFloor;

	protected bool m_active;

	protected HotSpotState m_state;

	public string AnimationPrefix = string.Empty;

	public HotspotGameData Model { get; protected set; }

	[method: MethodImpl(32)]
	public event Action<HotSpotWorldMapViewBase> HotspotClicked;

	public virtual void ActivateAsset(bool activate)
	{
	}

	public virtual void Initialize()
	{
		ActivateAsset(true);
		if (m_active)
		{
			InitialActivateHotspot();
		}
		AddHandlers();
		InitialSetupHotspot();
		HotSpotChanged(true);
	}

	public virtual void SynchBalancing()
	{
		if (m_ChronicleCaveHotspot || m_MiniCampaignHotspot)
		{
			return;
		}
		if (string.IsNullOrEmpty(m_nameId))
		{
			m_nameId = base.gameObject.name.ToLower();
		}
		RemoveHandlers();
		HotspotGameData value;
		if (DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(m_nameId, out value))
		{
			Model = value;
		}
		else
		{
			HotspotBalancingData balancing;
			if (!DIContainerBalancing.Service.TryGetBalancingData<HotspotBalancingData>(m_nameId, out balancing))
			{
				DebugLog.Error("Couldn't find HotspotBalancingData nameId: " + m_nameId);
				return;
			}
			Model = DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.AddNewHotspot(balancing);
		}
		m_active = Model.IsActive();
		Model.WorldMapView = this;
	}

	public void SyncWithMiniCampaign(EventCampaignGameData mcGameData)
	{
		HotspotGameData value = null;
		string nameId = m_nameId;
		if (!mcGameData.HotspotGameDatas.TryGetValue(nameId, out value))
		{
			HotspotBalancingData balancing = null;
			if (!DIContainerBalancing.Service.TryGetBalancingData<HotspotBalancingData>(nameId, out balancing))
			{
				return;
			}
			Model = mcGameData.AddNewHotspot(balancing);
		}
		else
		{
			Model = value;
		}
		m_active = Model.IsActive();
		Model.WorldMapView = this;
		Initialize();
	}

	public void SetChronicleCave(ChronicleCaveFloorGameData cave)
	{
		m_ChronicleCaveFloor = cave;
		ChronicleCaveFloor = cave.Data.FloorId;
		if (string.IsNullOrEmpty(m_nameId))
		{
			m_nameId = base.gameObject.name.ToLower();
		}
		RemoveHandlers();
		HotspotGameData value = null;
		if (m_ChronicleCaveHotspot)
		{
			if (!m_ChronicleCaveFloor.HotspotGameDatas.TryGetValue(m_nameId + "_" + cave.Data.FloorId.ToString("000"), out value))
			{
				DebugLog.Error("No Hotspot Data avaliable for Hotspot: " + m_nameId + "_" + cave.Data.FloorId.ToString("000"));
				ChronicleCaveHotspotBalancingData balancing = null;
				if (!DIContainerBalancing.Service.TryGetBalancingData<ChronicleCaveHotspotBalancingData>(m_nameId + "_" + cave.Data.FloorId.ToString("000"), out balancing))
				{
					DebugLog.Error("No Hotspot Balancing avaliable for Hotspot: " + m_nameId + "_" + cave.Data.FloorId.ToString("000"));
					return;
				}
				DebugLog.Log("Adding Hotspot Data for Hotspot: " + m_nameId + "_" + cave.Data.FloorId.ToString("000"));
				Model = m_ChronicleCaveFloor.AddNewHotspot(balancing);
			}
			else
			{
				Model = value;
			}
		}
		else if (!DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(m_nameId, out value))
		{
			HotspotBalancingData balancing2 = null;
			if (DIContainerBalancing.Service.TryGetBalancingData<HotspotBalancingData>(m_nameId, out balancing2))
			{
				Model = DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.AddNewHotspot(balancing2);
			}
			else
			{
				DebugLog.Error("No Balancing Data avaliable for Hotspot: " + m_nameId);
			}
		}
		else
		{
			Model = value;
		}
		m_active = Model.IsActive();
		Model.WorldMapView = this;
		Initialize();
	}

	protected virtual void InitialSetupHotspot()
	{
	}

	public virtual CharacterAssetController SetGoldenPig()
	{
		return null;
	}

	protected virtual void InitialActivateHotspot()
	{
		if (m_litePath != null && !m_litePath.gameObject.activeInHierarchy)
		{
			m_litePath.gameObject.SetActive(true);
		}
	}

	protected virtual void AddHandlers()
	{
		if (Model != null)
		{
			Model.HotSpotChanged += HotSpotChanged;
		}
	}

	private void OnDestroy()
	{
		RemoveHandlers();
	}

	protected virtual void HotSpotChanged(bool startUpSetting)
	{
	}

	protected virtual void RemoveHandlers()
	{
		if (Model != null)
		{
			Model.HotSpotChanged -= HotSpotChanged;
		}
	}

	public virtual bool IsCompleted()
	{
		if (Model == null)
		{
			DebugLog.Error("Hotspot Balancing Missing: " + m_nameId);
			return false;
		}
		return Model.IsCompleted();
	}

	public void SetState(HotSpotState state)
	{
		m_state = state;
	}

	public IEnumerator PlayActionTree(ExecuteActionTree execute)
	{
		execute.StartActionTree(this);
		do
		{
			yield return null;
		}
		while (!execute.IsDone());
		DIContainerInfrastructure.LocationStateMgr.EnableInput(true);
	}

	public virtual void Complete(HotSpotState state, bool startUp)
	{
		if (startUp)
		{
			return;
		}
		ExecuteActionTree component = GetComponent<ExecuteActionTree>();
		if (component != null)
		{
			component.SetStateMgr(DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr);
			if (component.m_executeBeforeUnlock)
			{
				component.StartActionTree(this);
				StartCoroutine(WaitForActionTreeFinish(component));
			}
		}
	}

	private IEnumerator WaitForActionTreeFinish(ExecuteActionTree execute)
	{
		do
		{
			yield return null;
		}
		while (!execute.IsDone());
		DIContainerInfrastructure.LocationStateMgr.EnableInput(true);
		StartCoroutine(ActivateFollowUpStagesAsync(null, null));
	}

	public virtual IEnumerator ActivateFollowUpStagesAsync(HotSpotWorldMapViewBase parentHotSpot, HotSpotWorldMapViewBase activateTo, bool instant = false)
	{
		if (!DIContainerLogic.WorldMapService.IsHotspotVisible(DIContainerInfrastructure.GetCurrentPlayer(), Model))
		{
			yield break;
		}
		if (!m_active && m_litePath != null && !m_litePath.gameObject.activeInHierarchy)
		{
			m_litePath.gameObject.SetActive(true);
			float timer = 0f;
			if (!instant && m_litePath.GetComponent<Animation>() != null && m_litePath.GetComponent<Animation>()["Path_Show"] != null)
			{
				m_litePath.GetComponent<Animation>().Play("Path_Show");
				yield return new WaitForSeconds(m_litePath.GetComponent<Animation>()["Path_Show"].length);
			}
		}
		if (!m_active)
		{
			ActivateAsset(true);
			if (!instant && (bool)GetComponent<Animation>() && GetComponent<Animation>()["SetActive"] != null)
			{
				GetComponent<Animation>().Play("SetActive");
				yield return new WaitForSeconds(GetComponent<Animation>()["SetActive"].length);
			}
			DIContainerLogic.WorldMapService.UnlockHotSpot(DIContainerInfrastructure.GetCurrentPlayer(), Model);
		}
		else if (Model.Data.UnlockState == HotspotUnlockState.ResolvedNew || Model.Data.UnlockState == HotspotUnlockState.ResolvedBetter)
		{
			DIContainerLogic.WorldMapService.UnlockHotSpot(DIContainerInfrastructure.GetCurrentPlayer(), Model);
		}
		m_active = true;
		if (!IsCompleted())
		{
			yield break;
		}
		for (int i = m_outgoingHotSpots.Count - 1; i >= 0; i--)
		{
			HotSpotWorldMapViewBase hotspot = m_outgoingHotSpots[i];
			if (!(hotspot == parentHotSpot) && (!(activateTo != null) || !(hotspot == activateTo)))
			{
				yield return StartCoroutine(hotspot.ActivateFollowUpStagesAsync(this, activateTo, instant));
			}
		}
	}

	public HotSpotWorldMapViewBase GetPreviousHotspot()
	{
		return m_previousHotSpot;
	}

	public HotSpotWorldMapViewBase GetHotspotWorldMapView(string spotName)
	{
		if (string.IsNullOrEmpty(spotName))
		{
			return null;
		}
		HotspotGameData value = null;
		if (m_ChronicleCaveHotspot)
		{
			if (m_ChronicleCaveFloor.HotspotGameDatas.TryGetValue(spotName, out value))
			{
				if (value.WorldMapView == null)
				{
				}
				return value.WorldMapView;
			}
		}
		else if (m_MiniCampaignHotspot)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentMiniCampaign.HotspotGameDatas.TryGetValue(spotName, out value))
			{
				return value.WorldMapView;
			}
		}
		else if (DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(spotName, out value))
		{
			if (value.WorldMapView == null)
			{
			}
			return value.WorldMapView;
		}
		DebugLog.Error("HotSpot not in World: " + spotName);
		return null;
	}

	public bool CalculatePath(HotSpotWorldMapViewBase parent, HotSpotWorldMapViewBase target, ref List<HotSpotWorldMapViewBase> list)
	{
		if (this == target)
		{
			list.Add(this);
			return true;
		}
		if (m_previousHotSpot != null && m_previousHotSpot != parent && m_previousHotSpot.CalculatePath(this, target, ref list))
		{
			list.Add(this);
			return true;
		}
		for (int i = 0; i < m_outgoingHotSpots.Count; i++)
		{
			HotSpotWorldMapViewBase hotSpotWorldMapViewBase = m_outgoingHotSpots[i];
			if (hotSpotWorldMapViewBase == null)
			{
				DebugLog.Error("Not Assigned Hotspot in array at " + base.gameObject.name);
			}
			else if (hotSpotWorldMapViewBase.gameObject.activeInHierarchy && !(hotSpotWorldMapViewBase == parent) && hotSpotWorldMapViewBase.CalculatePath(this, target, ref list))
			{
				list.Add(this);
				return true;
			}
		}
		return false;
	}

	public RageSplineLite GetPath()
	{
		return m_litePath;
	}

	private void OnTouchClicked()
	{
		HandleClicked();
	}

	private void HandleClicked()
	{
		if (!DIContainerInfrastructure.CurrentDragController.m_dragging && DIContainerInfrastructure.LocationStateMgr.IsInitialized)
		{
			HandleMouseButtonUp();
		}
	}

	public virtual void HandleMouseButtonUp(bool directMove = false)
	{
		DebugLog.Log("Hotspot Clicked!");
		if (m_active && !DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.IsLoading())
		{
			DIContainerInfrastructure.LocationStateMgr.SetNewHotSpot(this, null, directMove);
		}
	}

	public Bounds GetBoundingBox()
	{
		Collider component = GetComponent<Collider>();
		if (component != null)
		{
			return component.bounds;
		}
		return new Bounds(new Vector3(10000000f, 10000000f), Vector3.zero);
	}

	public virtual void ShowContentView()
	{
		if (this.HotspotClicked != null)
		{
			this.HotspotClicked(this);
		}
	}

	public List<HotSpotWorldMapViewBase> GetOutgoingHotspots()
	{
		return m_outgoingHotSpots;
	}

	public void PlayActiveAnimation()
	{
		if (!(GetComponent<Animation>() == null))
		{
			if ((bool)GetComponent<Animation>()[AnimationPrefix + "SetActive"])
			{
				GetComponent<Animation>().Play(AnimationPrefix + "SetActive");
			}
			if ((bool)GetComponent<Animation>()[AnimationPrefix + "Idle_Active"])
			{
				GetComponent<Animation>().PlayQueued(AnimationPrefix + "Idle_Active");
			}
			if ((bool)GetComponent<Animation>()[AnimationPrefix + "Idle"])
			{
				GetComponent<Animation>().PlayQueued(AnimationPrefix + "Idle");
			}
		}
	}

	public void PlayActiveIdleAnimation()
	{
		if ((bool)GetComponent<Animation>()[AnimationPrefix + "Idle_Active"] && !GetComponent<Animation>().IsPlaying("Idle_Active"))
		{
			GetComponent<Animation>().Play(AnimationPrefix + "Idle_Active");
		}
	}

	public void PlayUsedAnimation()
	{
		if ((bool)GetComponent<Animation>()[AnimationPrefix + "Used"] && !GetComponent<Animation>().IsPlaying(AnimationPrefix + "Used"))
		{
			GetComponent<Animation>().Play(AnimationPrefix + "Used");
		}
		if ((bool)GetComponent<Animation>()[AnimationPrefix + "Idle_Active"])
		{
			GetComponent<Animation>().PlayQueued(AnimationPrefix + "Idle_Active");
		}
	}

	public void PlayInactiveAnimation()
	{
		if ((bool)GetComponent<Animation>()[AnimationPrefix + "Idle_Active"])
		{
			GetComponent<Animation>().Stop(AnimationPrefix + "Idle_Active");
		}
		if ((bool)GetComponent<Animation>()[AnimationPrefix + "Idle_Inactive"])
		{
			GetComponent<Animation>().PlayQueued(AnimationPrefix + "Idle_Inactive");
		}
	}

	public void HandleMovingObjectVisibility(GameObject movingObject, HotSpotWorldMapViewBase target)
	{
		if (target == null)
		{
			return;
		}
		if (m_TransportMeaning == TransportMeaningType.Land && m_TransportMeaning == target.m_TransportMeaning)
		{
			movingObject.SetActive(movingObject.CompareTag("Untagged"));
		}
		else if (m_TransportMeaning == TransportMeaningType.Ship || target.m_TransportMeaning == TransportMeaningType.Ship)
		{
			if (movingObject.CompareTag("Ship") && !movingObject.activeInHierarchy)
			{
				movingObject.SetActive(movingObject.CompareTag("Ship"));
				movingObject.GetComponentInChildren<Animation>().Play("Show");
			}
			else
			{
				movingObject.SetActive(movingObject.CompareTag("Ship"));
			}
		}
		else if (m_TransportMeaning == TransportMeaningType.AirShip || target.m_TransportMeaning == TransportMeaningType.AirShip)
		{
			if (movingObject.CompareTag("AirShip") && !movingObject.activeInHierarchy)
			{
				movingObject.SetActive(movingObject.CompareTag("AirShip"));
				movingObject.GetComponentInChildren<Animation>().Play("Show");
			}
			else
			{
				movingObject.SetActive(movingObject.CompareTag("AirShip"));
			}
		}
		else if (m_TransportMeaning == TransportMeaningType.Submarine || target.m_TransportMeaning == TransportMeaningType.Submarine)
		{
			if (movingObject.CompareTag("Submarine") && !movingObject.activeInHierarchy)
			{
				movingObject.SetActive(movingObject.CompareTag("Submarine"));
				movingObject.GetComponentInChildren<Animation>().Play("Show");
			}
			else
			{
				movingObject.SetActive(movingObject.CompareTag("Submarine"));
			}
		}
	}

	protected HotSpotWorldMapViewBase GetNextHotspotWhere(Func<HotSpotWorldMapViewBase, bool> condition)
	{
		for (int i = 0; i < m_outgoingHotSpots.Count; i++)
		{
			if (condition(m_outgoingHotSpots[i]))
			{
				return m_outgoingHotSpots[i];
			}
		}
		for (int j = 0; j < m_outgoingHotSpots.Count; j++)
		{
			HotSpotWorldMapViewBase nextHotspotWhere = m_outgoingHotSpots[j].GetNextHotspotWhere(condition);
			if (nextHotspotWhere != null)
			{
				return nextHotspotWhere;
			}
		}
		return null;
	}

	public virtual bool IsDungeonHotSpot()
	{
		return false;
	}
}
