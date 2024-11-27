using System.Collections;
using System.Collections.Generic;
using ABH.Shared.BalancingData;
using SmoothMoves;
using UnityEngine;

public class PlayAnimationSceneMgr : CoreStateMgr
{
	public string m_CharacterBalancingNameId = "bird_red";

	public CharacterControllerCamp m_CharacterControllerCamp;

	public List<string> m_BalancingDataIds = new List<string>();

	public string m_MainHandWeapon = string.Empty;

	public string m_OffHandWeapon = string.Empty;

	public string m_ClassItem = string.Empty;

	public CharacterControllerCamp m_CharacterPrefab;

	private bool m_balancingDataInitialized;

	public void SetBalancingNames()
	{
		foreach (BirdBalancingData balancingData in DIContainerBalancing.Service.GetBalancingDataList<BirdBalancingData>())
		{
			m_BalancingDataIds.Add(balancingData.NameId);
		}
		foreach (PigBalancingData balancingData2 in DIContainerBalancing.Service.GetBalancingDataList<PigBalancingData>())
		{
			m_BalancingDataIds.Add(balancingData2.NameId);
		}
	}

	protected override void Awake()
	{
		CoreStateMgr.Instance = this;
		DebugLog.Log("[CoreStateMgr] Starting " + DIContainerConfig.GetAppDisplayName());
		Object.DontDestroyOnLoad(base.gameObject);
		DIContainerInfrastructure.GetVersionService().Init();
		Object.DontDestroyOnLoad(base.gameObject);
		DIContainerInfrastructure.GetLocaService().InitDefaultLoca(this);
		Object[] array = Object.FindObjectsOfType(typeof(AnimationManager));
		for (int num = array.Length - 1; num >= 0; num--)
		{
			Object.Destroy(array[num]);
		}
	}

	protected override IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		DIContainerBalancing.OnBalancingDataInitialized += delegate
		{
			m_balancingDataInitialized = true;
		};
		DIContainerBalancing.Init();
		while (!m_balancingDataInitialized)
		{
			yield return new WaitForEndOfFrame();
		}
		DIContainerInfrastructure.InitCurrentPlayerIfNecessary(ResetCharacter);
	}

	public void ResetCharacter()
	{
		if (Application.isPlaying)
		{
			if ((bool)m_CharacterControllerCamp)
			{
				Object.Destroy(m_CharacterControllerCamp.gameObject);
			}
			m_CharacterControllerCamp = Object.Instantiate(m_CharacterPrefab, new Vector3(0f, -240f), Quaternion.identity) as CharacterControllerCamp;
			m_CharacterControllerCamp.SetModel(m_CharacterBalancingNameId);
			UnityHelper.SetLayerRecusively(m_CharacterControllerCamp.gameObject, LayerMask.NameToLayer("Interface"));
		}
	}
}
