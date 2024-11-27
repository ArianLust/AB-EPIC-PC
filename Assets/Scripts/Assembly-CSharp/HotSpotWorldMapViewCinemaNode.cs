using System.Collections;
using ABH.Shared.Generic;
using UnityEngine;

public class HotSpotWorldMapViewCinemaNode : HotSpotWorldMapViewBase
{
	private bool m_IsLoading;

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

	public override void SynchBalancing()
	{
		base.SynchBalancing();
		m_active = false;
	}

	protected override void InitialSetupHotspot()
	{
		base.InitialSetupHotspot();
		switch (base.Model.Data.UnlockState)
		{
		case HotspotUnlockState.Active:
		case HotspotUnlockState.ResolvedNew:
		case HotspotUnlockState.Resolved:
		case HotspotUnlockState.ResolvedBetter:
			DIContainerInfrastructure.AdService.AddPlacement("ChannelInterstitial");
			break;
		}
	}

	public override void HandleMouseButtonUp(bool directMove = false)
	{
		base.HandleMouseButtonUp(true);
	}

	public override void ShowContentView()
	{
		base.ShowContentView();
		DIContainerInfrastructure.LocationStateMgr.WorldMenuUI.Leave();
		DIContainerInfrastructure.AdService.ShowAd("ChannelInterstitial");
		DIContainerInfrastructure.LocationStateMgr.ShowNewsUi(NewsUi.NewsUiState.Toons);
	}

	public override IEnumerator ActivateFollowUpStagesAsync(HotSpotWorldMapViewBase parentHotSpot, HotSpotWorldMapViewBase activateTo, bool instant = false)
	{
		yield return StartCoroutine(base.ActivateFollowUpStagesAsync(parentHotSpot, activateTo, instant));
		if (base.Model.Data.UnlockState > HotspotUnlockState.Hidden)
		{
			PlayActiveAnimation();
		}
	}
}
