using System.Collections;
using SmoothMoves;
using UnityEngine;

public class ShowTeaserSceneMgr : CoreStateMgr
{
	public EventPreviewUI EventPreviewUI;

	private bool m_balancingDataInitialized;

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
		DIContainerInfrastructure.InitCurrentPlayerIfNecessary(delegate
		{
			EventPreviewUI.SetModel(null);
			EventPreviewUI.SetHasChanged();
			EventPreviewUI.Enter();
		});
	}
}
