using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class HotSpotWorldMapAssetBase : MonoBehaviour
{
	[SerializeField]
	public GameObject m_LockedChains;

	public string AnimationPrefix = string.Empty;

	public HotspotGameData Model { get; protected set; }

	public virtual void SetModel(HotspotGameData model)
	{
		Model = model;
	}

	public virtual bool HasActivateObjects()
	{
		return true;
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

	public virtual void PlaySetActiveAnimation()
	{
	}

	public virtual void ActivateContainingAssets(bool activate)
	{
	}

	public virtual void CreateInitialFlag()
	{
	}

	public virtual void HandleFlagBeforeUnlockOnState(HotspotUnlockState hotspotUnlockState)
	{
	}

	public virtual void HandleFlagAfterUnlockOnState(HotspotUnlockState hotspotUnlockState)
	{
	}

	public virtual void SetChainsEnabled(bool p)
	{
	}

	public virtual bool HasChains()
	{
		return false;
	}
}
