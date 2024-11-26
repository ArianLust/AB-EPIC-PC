using System;
using System.Collections;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class HotSpotWorldMapViewResource : HotSpotWorldMapViewBase
{
	private bool m_resourcesAvaible;

	private int m_resourceAmount;

	private bool m_harvestingStarted;

	[SerializeField]
	private GameObject[] m_resourceObjects;

	[SerializeField]
	private Vector3 m_SpawnOffset = new Vector3(0f, 0f, -20f);

	public BattleLootVisualization m_ResourcePrefab;

	[SerializeField]
	private GameObject[] m_activateObjects;

	[method: MethodImpl(32)]
	public event Action OnDespawnResource;

	public override void ActivateAsset(bool activate)
	{
		GameObject[] activateObjects = m_activateObjects;
		foreach (GameObject gameObject in activateObjects)
		{
			gameObject.SetActive(true);
		}
	}

	public override void Initialize()
	{
		if (IsCompleted())
		{
			GameObject[] resourceObjects = m_resourceObjects;
			foreach (GameObject gameObject in resourceObjects)
			{
				gameObject.SetActive(false);
			}
		}
	}

	protected override void InitialSetupHotspot()
	{
		base.InitialSetupHotspot();
		if (base.Model.Data.UnlockState != HotspotUnlockState.Hidden && base.Model.Data.UnlockState != 0)
		{
			DIContainerLogic.GetResourceNodeManager().AddResourceSpot(this);
			m_resourceAmount = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ResourceSpawnAmountPerNode;
		}
		if (base.Model.Data.UnlockState == HotspotUnlockState.Active || base.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew)
		{
			PlayActiveIdleAnimation();
		}
	}

	public void OnDestroy()
	{
		if (DIContainerLogic.GetResourceNodeManager() != null)
		{
			DIContainerLogic.GetResourceNodeManager().RemoveResourceSpot(this);
		}
	}

	public override bool IsCompleted()
	{
		return base.Model.Data.UnlockState == HotspotUnlockState.Resolved;
	}

	public bool IsResourceAvaible()
	{
		return base.Model.Data.UnlockState == HotspotUnlockState.Active || base.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew;
	}

	public override void HandleMouseButtonUp(bool directMove = false)
	{
		DebugLog.Log("ResourceNode " + base.gameObject.name + " has state " + base.Model.Data.UnlockState);
		if (m_resourcesAvaible)
		{
			ShowContentView();
		}
	}

	public override void ShowContentView()
	{
		Requirement firstFailedReq = null;
		if (!DIContainerLogic.WorldMapService.CanTravelToHotspot(DIContainerInfrastructure.GetCurrentPlayer(), base.Model, out firstFailedReq))
		{
			if (firstFailedReq.RequirementType == RequirementType.HaveItem)
			{
				IInventoryItemGameData inventoryItemGameData = DIContainerLogic.InventoryService.GenerateNewInventoryItemGameData(1, 1, firstFailedReq.NameId, (int)firstFailedReq.Value);
				UIAtlas uIAtlas = null;
				if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(inventoryItemGameData.ItemIconAtlasName))
				{
					GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(inventoryItemGameData.ItemIconAtlasName) as GameObject;
					uIAtlas = gameObject.GetComponent<UIAtlas>();
				}
			}
			return;
		}
		m_resourceAmount--;
		if (m_resourceAmount <= 0)
		{
			CancelInvoke("DespawnResource");
			DespawnResource();
			return;
		}
		if (!m_harvestingStarted)
		{
			m_harvestingStarted = true;
			Invoke("DespawnResource", 10f);
		}
		base.ShowContentView();
		DIContainerLogic.GetResourceNodeManager().SpawnResource(base.gameObject, m_SpawnOffset);
		PlayUsedAnimation();
	}

	public void DespawnResource()
	{
		m_resourceAmount = 0;
		m_resourcesAvaible = false;
		PlayInactiveResourceAnimation();
		GameObject[] resourceObjects = m_resourceObjects;
		foreach (GameObject gameObject in resourceObjects)
		{
			gameObject.SetActive(false);
		}
		if (this.OnDespawnResource != null)
		{
			this.OnDespawnResource();
		}
	}

	public void PlayInactiveResourceAnimation()
	{
		if ((bool)GetComponent<Animation>()[AnimationPrefix + "Idle_Active"])
		{
			GetComponent<Animation>().Stop(AnimationPrefix + "Idle_Active");
		}
		StartCoroutine(PlayInactiveAnimationWhenUsedDone());
	}

	private IEnumerator PlayInactiveAnimationWhenUsedDone()
	{
		while (GetComponent<Animation>().isPlaying)
		{
			yield return new WaitForEndOfFrame();
		}
		if ((bool)GetComponent<Animation>()[AnimationPrefix + "Idle_Inactive"])
		{
			GetComponent<Animation>().Play(AnimationPrefix + "Idle_Inactive");
		}
	}

	public override void SynchBalancing()
	{
		base.SynchBalancing();
		m_active = false;
	}

	public override IEnumerator ActivateFollowUpStagesAsync(HotSpotWorldMapViewBase parentHotSpot, HotSpotWorldMapViewBase activateTo, bool instant = false)
	{
		DIContainerLogic.GetResourceNodeManager().AddResourceSpot(this);
		if (!DIContainerLogic.WorldMapService.IsHotspotVisible(DIContainerInfrastructure.GetCurrentPlayer(), base.Model))
		{
			yield break;
		}
		if (!m_active && m_litePath != null && !m_litePath.gameObject.activeInHierarchy)
		{
			m_litePath.gameObject.SetActive(true);
			float timer = 0f;
			if (!instant && !instant && m_litePath.GetComponent<Animation>() != null && m_litePath.GetComponent<Animation>()["Path_Show"] != null)
			{
				m_litePath.GetComponent<Animation>().Play("Path_Show");
				yield return new WaitForSeconds(m_litePath.GetComponent<Animation>()["Path_Show"].length);
			}
		}
		if (!m_active)
		{
			GameObject[] activateObjects = m_activateObjects;
			foreach (GameObject go in activateObjects)
			{
				go.SetActive(true);
			}
			if (!instant && (bool)GetComponent<Animation>() && GetComponent<Animation>()["SetActive"] != null)
			{
				GetComponent<Animation>().Play("SetActive");
				yield return new WaitForSeconds(GetComponent<Animation>()["SetActive"].length);
			}
			DIContainerLogic.WorldMapService.UnlockHotSpot(DIContainerInfrastructure.GetCurrentPlayer(), base.Model);
		}
		else if (base.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew || base.Model.Data.UnlockState == HotspotUnlockState.ResolvedBetter)
		{
			DIContainerLogic.WorldMapService.UnlockHotSpot(DIContainerInfrastructure.GetCurrentPlayer(), base.Model);
		}
		m_active = true;
		if (IsCompleted() || m_active)
		{
			foreach (HotSpotWorldMapViewBase hotspot in m_outgoingHotSpots)
			{
				if (!(hotspot == parentHotSpot) && (!(activateTo != null) || !(hotspot == activateTo)))
				{
					yield return StartCoroutine(hotspot.ActivateFollowUpStagesAsync(this, activateTo, instant));
				}
			}
		}
		if (base.Model.Data.UnlockState == HotspotUnlockState.ResolvedNew || base.Model.Data.UnlockState == HotspotUnlockState.Active)
		{
			Respawn();
			DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("resourceSpot_respawn", base.Model.BalancingData.NameId);
		}
	}

	protected override void HotSpotChanged(bool startUpSetting)
	{
	}

	public void Respawn()
	{
		GameObject[] resourceObjects = m_resourceObjects;
		foreach (GameObject gameObject in resourceObjects)
		{
			gameObject.SetActive(true);
		}
		m_resourcesAvaible = true;
		m_resourceAmount = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island").ResourceSpawnAmountPerNode;
		m_harvestingStarted = false;
		base.Model.Data.UnlockState = HotspotUnlockState.Active;
		DebugLog.Log("node respawned");
		PlayActiveAnimation();
	}
}
