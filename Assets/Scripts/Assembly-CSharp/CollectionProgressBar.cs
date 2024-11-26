using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class CollectionProgressBar : MonoBehaviour
{
	private CollectionGroupBalancingData m_collectionBalancing;

	[SerializeField]
	private CollectionItemSlot m_rewardSlot;

	[SerializeField]
	private List<CollectionItemSlot> m_collectionItemSlots;

	private void Start()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData == null || DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentMiniCampaign == null)
		{
			DebugLog.Log("No collection found for CollectionProgressBar!");
			base.gameObject.PlayAnimationOrAnimatorState("RewardProgress_Leave");
		}
		else
		{
			m_collectionBalancing = DIContainerInfrastructure.GetCurrentPlayer().CurrentEventManagerGameData.CurrentMiniCampaign.CollectionGroupBalancing;
			SetSlotModels();
		}
	}

	public void SetSlotModels()
	{
		for (int i = 0; i < m_collectionBalancing.ComponentRequirements.Count; i++)
		{
			CollectionItemSlot collectionItemSlot = m_collectionItemSlots[i];
			string nameId = m_collectionBalancing.ComponentRequirements[i].NameId;
			float value = m_collectionBalancing.ComponentRequirements[i].Value;
			InventoryGameData inventoryGameData = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData;
			IInventoryItemGameData data = new BasicItemGameData(nameId);
			if (DIContainerLogic.InventoryService.CheckForItem(inventoryGameData, nameId))
			{
				DIContainerLogic.InventoryService.TryGetItemGameData(inventoryGameData, nameId, out data);
			}
			collectionItemSlot.SetModel(data, m_collectionBalancing.ComponentRequirements[i]);
		}
		Dictionary<string, int> loot = ((!DIContainerInfrastructure.EventSystemStateManager.UseCollectionFallbackReward() && DIContainerLogic.EventSystemService.GetAvailableEliteChestReward(DIContainerInfrastructure.GetCurrentPlayer()).Count != 0) ? m_collectionBalancing.Reward : m_collectionBalancing.FallbackReward);
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerLogic.GetLootOperationService().GenerateLoot(loot, 1));
		m_rewardSlot.SetModel(itemsFromLoot[0]);
	}

	public void UpdateProgressStatus()
	{
		for (int i = 0; i < m_collectionItemSlots.Count; i++)
		{
			CollectionItemSlot collectionItemSlot = m_collectionItemSlots[i];
			collectionItemSlot.UpdateStatus();
		}
		m_rewardSlot.UpdateStatus();
	}

	private IEnumerator EnterCoroutine()
	{
		while (DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.IsLoading())
		{
			yield return new WaitForEndOfFrame();
		}
		base.gameObject.PlayAnimationOrAnimatorState("RewardProgress_Enter");
		SetSlotModels();
		yield return new WaitForSeconds(base.gameObject.GetAnimationOrAnimatorStateLength("RewardProgress_Enter"));
		UpdateProgressStatus();
	}

	public void Enter()
	{
		StartCoroutine(EnterCoroutine());
	}
}
