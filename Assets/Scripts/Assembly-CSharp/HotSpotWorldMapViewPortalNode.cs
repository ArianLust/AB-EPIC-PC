using System.Collections;
using UnityEngine;

public class HotSpotWorldMapViewPortalNode : HotSpotWorldMapViewBase
{
	private int m_Performance;

	private bool m_BlockInput;

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

	public override void HandleMouseButtonUp(bool directMove = false)
	{
		base.HandleMouseButtonUp();
	}

	public override void ShowContentView()
	{
		if (!m_BlockInput)
		{
			m_BlockInput = true;
			base.ShowContentView();
			DIContainerInfrastructure.GetCoreStateMgr().GotoWorldMap();
		}
	}

	public override void Complete(HotSpotState state, bool startUp)
	{
		StartCoroutine(PlayCompleteAnimAsync(startUp));
	}

	private IEnumerator PlayCompleteAnimAsync(bool startUp)
	{
		if (startUp)
		{
			yield break;
		}
		if (!startUp)
		{
			ExecuteActionTree execute2 = GetComponent<ExecuteActionTree>();
			if (execute2 != null)
			{
				execute2.SetStateMgr(DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr);
				if (execute2.m_executeBeforeUnlock)
				{
					yield return StartCoroutine(PlayActionTree(execute2));
				}
			}
		}
		yield return StartCoroutine(ActivateFollowUpStagesAsync(GetPreviousHotspot(), null));
		if (startUp)
		{
			yield break;
		}
		ExecuteActionTree execute = GetComponent<ExecuteActionTree>();
		if (execute != null)
		{
			execute.SetStateMgr(DIContainerInfrastructure.LocationStateMgr as WorldMapStateMgr);
			if (!execute.m_executeBeforeUnlock)
			{
				yield return StartCoroutine(PlayActionTree(execute));
			}
		}
	}

	public new IEnumerator PlayActionTree(ExecuteActionTree execute)
	{
		execute.StartActionTree(this);
		do
		{
			yield return null;
		}
		while (!execute.IsDone());
		DIContainerInfrastructure.LocationStateMgr.EnableInput(true);
	}

	protected override void HotSpotChanged(bool startUp)
	{
		base.HotSpotChanged(startUp);
		if (base.Model.IsCompleted() && base.Model.GetStarCount() > m_Performance)
		{
			m_Performance = base.Model.GetStarCount();
			Complete(m_state, startUp);
		}
	}

	protected override void InitialSetupHotspot()
	{
		base.InitialSetupHotspot();
	}
}
