using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpotWorldMapViewCampNode : HotSpotWorldMapViewBase
{
	private int m_Performance;

	private bool m_BlockInput;

	[SerializeField]
	private List<GameObject> m_Eggs = new List<GameObject>();

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
			DIContainerInfrastructure.GetCoreStateMgr().m_ArenaLockedPopup.LeavePopup();
			DIContainerInfrastructure.GetCoreStateMgr().GotoCampScreen();
		}
	}

	public override IEnumerator ActivateFollowUpStagesAsync(HotSpotWorldMapViewBase parentHotSpot, HotSpotWorldMapViewBase activateTo, bool instant = false)
	{
		yield return StartCoroutine(base.ActivateFollowUpStagesAsync(parentHotSpot, activateTo, instant));
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
		if (m_Eggs.Count >= 1)
		{
			m_Eggs[0].gameObject.SetActive(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "egg_01") > 0);
		}
		if (m_Eggs.Count >= 2)
		{
			m_Eggs[1].gameObject.SetActive(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "egg_02") > 0);
		}
		if (m_Eggs.Count >= 3)
		{
			m_Eggs[2].gameObject.SetActive(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "egg_03") > 0);
		}
		if (m_Eggs.Count >= 4)
		{
			m_Eggs[3].gameObject.SetActive(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "egg_04") > 0);
		}
		if (m_Eggs.Count >= 5)
		{
			m_Eggs[4].gameObject.SetActive(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "egg_05") > 0);
		}
	}
}
