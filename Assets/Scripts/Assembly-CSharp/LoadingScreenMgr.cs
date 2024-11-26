using System;
using System.Collections.Generic;
using System.Linq;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class LoadingScreenMgr : MonoBehaviour
{
	[Serializable]
	private struct LoadingBackgroundData
	{
		public LoadingArea m_Target;

		public GameObject m_Background;

		public GameObject m_Tutor;
	}

	public UILabel m_Label;

	public Animation m_Animation;

	private bool notIgnoreFirstShow;

	[SerializeField]
	[Header("Backgrounds and Tutors")]
	private List<LoadingBackgroundData> m_loadingBackgrounds;

	public void Awake()
	{
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.LoadingScreen = this;
		DebugLog.Log("Loading screen set: " + DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.LoadingScreen);
		base.gameObject.SetActive(false);
	}

	public float Show(LoadingArea target)
	{
		if (!notIgnoreFirstShow)
		{
			if ((bool)ContentLoader.Instance)
			{
				ContentLoader.Instance.SetDownloadProgress(0.5f);
			}
			notIgnoreFirstShow = true;
			base.gameObject.SetActive(true);
			m_Animation.Play("LoadingScreen_Enter");
			base.gameObject.SetActive(false);
			return 0f;
		}
		CancelInvoke("HideNow");
		base.gameObject.SetActive(true);
		m_Animation.Play("LoadingScreen_Enter");
		List<LoadingHintBalancingData> list = new List<LoadingHintBalancingData>();
		List<LoadingHintBalancingData> list2 = DIContainerBalancing.Service.GetBalancingDataList<LoadingHintBalancingData>().ToList();
		float num = 0f;
		foreach (LoadingHintBalancingData item in list2)
		{
			if (DIContainerLogic.RequirementService.CheckGenericRequirements(DIContainerInfrastructure.GetCurrentPlayer(), item.ShowRequirements) && item.TargetAreas.Contains(target))
			{
				list.Add(item);
				num += item.Weight;
			}
		}
		SetBackgroundAndTutor(target);
		string text = string.Empty;
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			num3 += list[i].Weight;
			if (num2 <= num3)
			{
				text = DIContainerInfrastructure.GetLocaService().Tr(list[i].NameId);
				break;
			}
		}
		m_Label.text = text;
		return m_Animation["LoadingScreen_Enter"].length;
	}

	private void SetBackgroundAndTutor(LoadingArea target)
	{
		bool flag = true;
		foreach (LoadingBackgroundData loadingBackground in m_loadingBackgrounds)
		{
			bool flag2 = loadingBackground.m_Target == target;
			loadingBackground.m_Background.SetActive(flag2);
			loadingBackground.m_Tutor.SetActive(flag2);
			if (flag2)
			{
				flag = false;
			}
		}
		if (flag && m_loadingBackgrounds != null && m_loadingBackgrounds.Count > 0)
		{
			m_loadingBackgrounds[0].m_Background.SetActive(true);
			m_loadingBackgrounds[0].m_Tutor.SetActive(true);
		}
	}

	public float Hide()
	{
		if (!notIgnoreFirstShow)
		{
			notIgnoreFirstShow = true;
			base.gameObject.SetActive(true);
			float num = m_Animation["LoadingScreen_CloseIris"].length + m_Animation["LoadingScreen_OpenIris"].length;
			Invoke("OpenIris", CloseIris());
			Invoke("HideNow", num);
			if ((bool)ContentLoader.Instance)
			{
				ContentLoader.Instance.DestroyLoadingScreen(m_Animation["LoadingScreen_CloseIris"].length);
			}
			return num;
		}
		base.gameObject.SetActive(true);
		m_Animation.Play("LoadingScreen_Leave");
		Invoke("HideNow", m_Animation.GetClip("LoadingScreen_Leave").length);
		if ((bool)ContentLoader.Instance)
		{
			ContentLoader.Instance.DestroyLoadingScreen(m_Animation["LoadingScreen_Leave"].length / 2f);
		}
		return m_Animation["LoadingScreen_Leave"].length;
	}

	public float HideLength()
	{
		return m_Animation["LoadingScreen_Leave"].length;
	}

	private void HideNow()
	{
		base.gameObject.SetActive(false);
	}

	private void ShowLabel()
	{
	}

	public float CloseIris()
	{
		m_Animation.Play("LoadingScreen_CloseIris");
		return m_Animation["LoadingScreen_CloseIris"].length;
	}

	public float OpenIris()
	{
		m_Animation.Play("LoadingScreen_OpenIris");
		return m_Animation["LoadingScreen_OpenIris"].length;
	}
}
