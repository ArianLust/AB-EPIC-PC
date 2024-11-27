using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using UnityEngine;

public class ChestPositioning : MonoBehaviour
{
	[SerializeField]
	private Vector3 m_Offset;

	[SerializeField]
	private GameObject m_ChestPrefab;

	[SerializeField]
	private bool m_AllowTooltipIfHotspotActive;

	private GameObject m_Chest;

	[HideInInspector]
	public ChestController m_ChestButton;

	[SerializeField]
	private LootDisplayContoller m_ExplodeLootItems;

	[SerializeField]
	private GenericAssetProvider m_fxAssetProvider;

	[method: MethodImpl(32)]
	public event Action OnChestClicked;

	public bool SpawnChest()
	{
		if (m_Chest != null)
		{
			return true;
		}
		m_Chest = UnityEngine.Object.Instantiate(m_ChestPrefab, base.transform.position + m_Offset, Quaternion.identity) as GameObject;
		if (m_Chest != null)
		{
			m_Chest.transform.parent = base.transform;
			m_ChestButton = m_Chest.GetComponent<ChestController>();
			if ((bool)m_ChestButton)
			{
				m_ChestButton.m_ParticleSystemSpawner.m_ParticleSystemsAssetProvider = m_fxAssetProvider;
			}
		}
		return m_Chest != null;
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		if (m_ChestButton != null)
		{
			DebugLog.Log("Chest Registered!");
			m_ChestButton.Clicked += ChestClicked;
		}
	}

	private void DeRegisterEventHandlers()
	{
		if (m_ChestButton != null)
		{
			m_ChestButton.Clicked -= ChestClicked;
		}
	}

	private void ChestClicked()
	{
		DebugLog.Log("Chest clicked!");
		if (this.OnChestClicked != null)
		{
			this.OnChestClicked();
		}
	}

	public bool RemoveChest(List<IInventoryItemGameData> lootItems)
	{
		DeRegisterEventHandlers();
		LootDisplayContoller lootDisplayContoller = UnityEngine.Object.Instantiate(m_ExplodeLootItems, m_Chest.transform.position, Quaternion.identity) as LootDisplayContoller;
		lootDisplayContoller.SetModel(null, lootItems, LootDisplayType.None);
		lootDisplayContoller.gameObject.SetActive(false);
		StartCoroutine(RemoveChestCoroutine(lootItems, lootDisplayContoller));
		return true;
	}

	private IEnumerator RemoveChestCoroutine(List<IInventoryItemGameData> lootItems, LootDisplayContoller explodingLoot)
	{
		float animLength = m_Chest.PlayAnimationOrAnimatorState("TreasureChest_Open");
		yield return new WaitForSeconds(animLength / 2f);
		if (explodingLoot != null)
		{
			explodingLoot.transform.parent = base.transform;
		}
		explodingLoot.gameObject.SetActive(true);
		List<LootDisplayContoller> explodedLoot = explodingLoot.Explode(true, false, 0.5f, false, 0f, 0f);
		explodingLoot.gameObject.SetActive(false);
		yield return new WaitForSeconds(animLength / 2f);
		UnityEngine.Object.Destroy(m_Chest);
		UnityEngine.Object.Destroy(explodingLoot);
		foreach (LootDisplayContoller explodedItem in explodedLoot)
		{
			UnityEngine.Object.Destroy(explodedItem.gameObject, explodedItem.gameObject.GetComponent<Animation>().clip.length);
		}
		Invoke("RecheckFeatureUnlocks", 1.5f);
	}

	public void RecheckFeatureUnlocks()
	{
		StartCoroutine(DIContainerInfrastructure.LocationStateMgr.StoppablePopupCoroutine());
	}

	public void SetOpenable()
	{
		if ((bool)m_Chest)
		{
			m_Chest.GetComponent<Collider>().enabled = true;
		}
		RegisterEventHandlers();
		if (m_Chest != null)
		{
			m_Chest.PlayAnimationOrAnimatorState("TreasureChest_Idle_Active");
		}
	}

	public void SetActive()
	{
		if ((bool)m_Chest && !m_AllowTooltipIfHotspotActive)
		{
			m_Chest.GetComponent<Collider>().enabled = false;
		}
	}
}
