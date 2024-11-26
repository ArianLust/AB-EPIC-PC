using System.Collections;
using ABH.GameDatas;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class HotSpotWorldMapViewArenaNode : HotSpotWorldMapViewBase
{
	[SerializeField]
	private GameObject[] m_activateObjects;

	public override void ActivateAsset(bool activate)
	{
		GameObject[] activateObjects = m_activateObjects;
		foreach (GameObject gameObject in activateObjects)
		{
			gameObject.SetActive(true);
		}
	}

	public override bool IsCompleted()
	{
		return base.Model.Data.UnlockState == HotspotUnlockState.Resolved;
	}

	protected override void InitialSetupHotspot()
	{
		base.InitialSetupHotspot();
		if (base.Model.Data.UnlockState > HotspotUnlockState.Hidden && (bool)GetComponent<Animation>()[AnimationPrefix + "Idle_Active"])
		{
			GetComponent<Animation>().Play(AnimationPrefix + "Idle_Active");
		}
	}

	protected override void InitialActivateHotspot()
	{
		base.InitialActivateHotspot();
		if (base.Model.Data.UnlockState > HotspotUnlockState.Hidden && (bool)GetComponent<Animation>()[AnimationPrefix + "Idle_Active"])
		{
			GetComponent<Animation>().Play(AnimationPrefix + "Idle_Active");
		}
	}

	public override void HandleMouseButtonUp(bool directMove = false)
	{
		DebugLog.Log("Arena Hotspot Clicked!");
		if (!m_active || !DIContainerLogic.PvPSeasonService.IsCurrentPvPTurnAvailable(DIContainerInfrastructure.GetCurrentPlayer()))
		{
			StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_ArenaLockedPopup.ShowArenaLockedPopup(!m_active).Run());
		}
		else
		{
			DIContainerInfrastructure.LocationStateMgr.SetNewHotSpot(this, null);
		}
	}

	public override void ShowContentView()
	{
		Requirement requirement = null;
		base.ShowContentView();
		DIContainerInfrastructure.GetCoreStateMgr().GotoPvpCampScreen();
	}

	public override IEnumerator ActivateFollowUpStagesAsync(HotSpotWorldMapViewBase parentHotSpot, HotSpotWorldMapViewBase activateTo, bool instant = false)
	{
		yield return StartCoroutine(base.ActivateFollowUpStagesAsync(parentHotSpot, activateTo, instant));
	}

	public override void SynchBalancing()
	{
		base.SynchBalancing();
	}

	public void ShowTooltip()
	{
		bool flag = false;
		foreach (BasicItemGameData item in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Story])
		{
			if (item.BalancingData.NameId == "unlock_pvp")
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(base.transform, DIContainerInfrastructure.GetLocaService().Tr("hotspot_tt_arena", "Bird Arena"), base.gameObject.layer == LayerMask.NameToLayer("Interface"));
		}
		else
		{
			StartCoroutine(DIContainerInfrastructure.GetCoreStateMgr().m_ArenaLockedPopup.ShowArenaLockedPopup(true).Run());
		}
	}
}
