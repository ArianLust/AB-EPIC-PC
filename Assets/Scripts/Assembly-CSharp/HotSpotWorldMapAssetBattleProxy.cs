using System;
using System.Collections;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class HotSpotWorldMapAssetBattleProxy : HotSpotWorldMapAssetBase
{
	[SerializeField]
	private GameObject[] m_flagPrefabs;

	[SerializeField]
	private Transform m_flagRoot;

	private GameObject m_currentFlag;

	[SerializeField]
	private GameObject m_initialFlag;

	[SerializeField]
	protected GameObject[] m_activateObjects;

	public override void SetModel(HotspotGameData model)
	{
		base.Model = model;
	}

	public override bool HasActivateObjects()
	{
		return m_activateObjects.Length > 0;
	}

	public override void ActivateContainingAssets(bool activate)
	{
	}

	public override void CreateInitialFlag()
	{
		if ((bool)m_initialFlag)
		{
			DebugLog.Log("initial flag");
			m_currentFlag = UnityEngine.Object.Instantiate(m_initialFlag);
			m_currentFlag.transform.parent = m_flagRoot;
			m_currentFlag.transform.localPosition = Vector3.zero;
			m_currentFlag.transform.localEulerAngles = Vector3.one;
			m_currentFlag.transform.localScale = Vector3.one;
		}
	}

	public override void HandleFlagBeforeUnlockOnState(HotspotUnlockState hotspotUnlockState)
	{
		switch (hotspotUnlockState)
		{
		case HotspotUnlockState.Unknown:
		case HotspotUnlockState.Hidden:
		case HotspotUnlockState.Active:
		case HotspotUnlockState.ResolvedBetter:
			break;
		case HotspotUnlockState.ResolvedNew:
			if ((bool)m_initialFlag)
			{
				if (m_currentFlag != null)
				{
					UnityEngine.Object.Destroy(m_currentFlag);
				}
				DebugLog.Log("hotspot resolved new with initial flag: " + base.Model.StageName);
				StartCoroutine(SetAccomplished(false));
			}
			else
			{
				if (m_currentFlag != null)
				{
					UnityEngine.Object.Destroy(m_currentFlag);
				}
				DebugLog.Log("hotspot resolved new non initial flag: " + base.Model.StageName);
				StartCoroutine(SetAccomplished(false));
			}
			break;
		case HotspotUnlockState.Resolved:
			if (!m_initialFlag)
			{
				if (m_currentFlag != null)
				{
					UnityEngine.Object.Destroy(m_currentFlag);
				}
				CreateFlag();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("hotspotUnlockState");
		}
	}

	public override void HandleFlagAfterUnlockOnState(HotspotUnlockState hotspotUnlockState)
	{
		switch (hotspotUnlockState)
		{
		case HotspotUnlockState.Unknown:
		case HotspotUnlockState.Hidden:
		case HotspotUnlockState.Active:
			break;
		case HotspotUnlockState.ResolvedNew:
			if (!m_initialFlag && (bool)m_currentFlag)
			{
				UnityEngine.Object.Destroy(m_currentFlag);
			}
			break;
		case HotspotUnlockState.Resolved:
			if ((bool)m_currentFlag)
			{
				UnityEngine.Object.Destroy(m_currentFlag);
			}
			CreateFlag();
			StartCoroutine(SetAccomplished(true));
			break;
		case HotspotUnlockState.ResolvedBetter:
			if ((bool)m_initialFlag)
			{
				StartCoroutine(SetAccomplished(true));
				break;
			}
			if ((bool)m_currentFlag)
			{
				UnityEngine.Object.Destroy(m_currentFlag);
			}
			CreateFlag();
			StartCoroutine(SetAccomplished(true));
			break;
		default:
			throw new ArgumentOutOfRangeException("hotspotUnlockState");
		}
	}

	private void CreateFlag()
	{
		m_currentFlag = UnityEngine.Object.Instantiate(m_flagPrefabs[Mathf.Max(0, base.Model.Data.StarCount - 1)]);
		if (m_currentFlag != null)
		{
			m_currentFlag.transform.parent = m_flagRoot;
			m_currentFlag.transform.localPosition = Vector3.zero;
			m_currentFlag.transform.localEulerAngles = Vector3.one;
			m_currentFlag.transform.localScale = Vector3.one;
		}
	}

	private IEnumerator SetAccomplished(bool initial, bool rerise = false)
	{
		if (initial)
		{
			if (m_currentFlag != null)
			{
				UnityEngine.Object.Destroy(m_currentFlag);
			}
			CreateFlag();
		}
		else
		{
			if (m_currentFlag != null)
			{
				UnityEngine.Object.Destroy(m_currentFlag);
			}
			CreateFlag();
		}
		yield return new WaitForSeconds(1f);
	}

	public override void SetChainsEnabled(bool p)
	{
		if ((bool)m_LockedChains)
		{
			m_LockedChains.SetActive(p);
		}
	}

	public override bool HasChains()
	{
		return m_LockedChains;
	}
}
