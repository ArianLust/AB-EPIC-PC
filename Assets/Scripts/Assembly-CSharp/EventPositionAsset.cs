using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class EventPositionAsset : MonoBehaviour
{
	[SerializeField]
	private Transform m_CharacterRoot;

	[SerializeField]
	private Transform m_AdditionRoot;

	private EventItemGameData m_placedItem;

	[SerializeField]
	private List<GameObject> m_ObjectsToEnable = new List<GameObject>();

	[SerializeField]
	private CharacterControllerWorldMap m_CampCharacterControllerPrefab;

	private bool m_instant;

	public bool m_IsSpawned;

	private GameObject m_AdditionalProp;

	public void SetItem(EventItemGameData eventItem, bool instant = false)
	{
		if (m_placedItem != null)
		{
			return;
		}
		m_instant = instant;
		m_placedItem = eventItem;
		string assetBaseId = m_placedItem.BalancingData.AssetBaseId;
		if (assetBaseId.StartsWith("pig_"))
		{
			CharacterControllerWorldMap characterControllerWorldMap = Object.Instantiate(m_CampCharacterControllerPrefab);
			PigBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<PigBalancingData>(assetBaseId);
			if (characterControllerWorldMap == null)
			{
				return;
			}
			characterControllerWorldMap.SetModel(balancingData.NameId);
			characterControllerWorldMap.transform.parent = ((!m_CharacterRoot) ? base.transform : m_CharacterRoot);
			characterControllerWorldMap.transform.localPosition = Vector3.zero;
			characterControllerWorldMap.transform.localScale = Vector3.Scale(characterControllerWorldMap.transform.localScale, DIContainerInfrastructure.LocationStateMgr.GetWorldBirdScale());
			StartCoroutine(SpawnCharacters(characterControllerWorldMap));
			UnityHelper.SetLayerRecusively(characterControllerWorldMap.gameObject, base.gameObject.layer);
		}
		else if (assetBaseId.Contains("BossNode"))
		{
			m_AdditionalProp = DIContainerInfrastructure.GetCharacterAssetProvider(true).InstantiateObject(assetBaseId, (!m_AdditionRoot) ? base.transform : m_AdditionRoot, Vector3.zero, Quaternion.identity);
			if (m_AdditionalProp == null)
			{
				return;
			}
			m_AdditionalProp.transform.localPosition = Vector3.zero;
			StartCoroutine(SpawnAddition());
			UnityHelper.SetLayerRecusively(m_AdditionalProp.gameObject, base.gameObject.layer);
		}
		else
		{
			m_AdditionalProp = DIContainerInfrastructure.PropLiteAssetProvider().InstantiateObject(assetBaseId, (!m_AdditionRoot) ? base.transform : m_AdditionRoot, Vector3.zero, Quaternion.identity);
			if (m_AdditionalProp == null)
			{
				return;
			}
			m_AdditionalProp.transform.localPosition = Vector3.zero;
			StartCoroutine(SpawnAddition());
			UnityHelper.SetLayerRecusively(m_AdditionalProp.gameObject, base.gameObject.layer);
		}
		foreach (GameObject item in m_ObjectsToEnable)
		{
			item.SetActive(false);
		}
	}

	private IEnumerator SpawnAddition()
	{
		while (!DIContainerInfrastructure.LocationStateMgr.IsInitialized && !DIContainerInfrastructure.LocationStateMgr.ForceSpawnEventNodes)
		{
			yield return new WaitForSeconds(1f);
		}
		if (!m_instant)
		{
			if (m_placedItem == null)
			{
				yield return new WaitForSeconds(Random.Range(2f, 6f));
			}
			else if (m_placedItem.BalancingData.ItemType == InventoryItemType.EventCampaignItem || m_placedItem.BalancingData.ItemType == InventoryItemType.EventBossItem)
			{
				yield return new WaitForEndOfFrame();
			}
			else
			{
				yield return new WaitForSeconds(Random.Range(2f, 6f));
			}
		}
		foreach (GameObject o in m_ObjectsToEnable)
		{
			o.SetActive(true);
		}
		if (!m_instant)
		{
			yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("EventNode_Show"));
		}
		m_IsSpawned = true;
	}

	private IEnumerator SpawnCharacters(CharacterControllerWorldMap character)
	{
		while (!DIContainerInfrastructure.LocationStateMgr.IsInitialized)
		{
			yield return new WaitForSeconds(1f);
		}
		if (!m_instant && m_placedItem.BalancingData.ItemType != InventoryItemType.EventCampaignItem)
		{
			if (m_placedItem.BalancingData.ItemType == InventoryItemType.EventCampaignItem || m_placedItem.BalancingData.ItemType == InventoryItemType.EventBossItem)
			{
				yield return new WaitForEndOfFrame();
			}
			else
			{
				yield return new WaitForSeconds(Random.Range(2f, 6f));
			}
		}
		foreach (GameObject o in m_ObjectsToEnable)
		{
			o.SetActive(true);
		}
		character.SetModel(character.GetModel());
		if (!m_instant)
		{
			yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("EventNode_Show"));
			StartCoroutine(CheerCharacterRepeating(character));
		}
		m_IsSpawned = true;
	}

	private IEnumerator CheerCharacterRepeating(CharacterControllerWorldMap character)
	{
		if ((bool)character)
		{
			yield return new WaitForSeconds(Random.Range(1f, 5f));
			yield return new WaitForSeconds(character.PlayCheerCharacter());
		}
	}

	public bool HasItem()
	{
		return m_placedItem != null;
	}

	public float DespawnItem()
	{
		m_placedItem = null;
		float a = 0f;
		CharacterControllerWorldMap[] componentsInChildren = GetComponentsInChildren<CharacterControllerWorldMap>();
		Object.Destroy(base.gameObject, Mathf.Max(a, base.gameObject.PlayAnimationOrAnimatorState("EventNode_Hide")));
		return Mathf.Max(a, base.gameObject.PlayAnimationOrAnimatorState("EventNode_Hide"));
	}

	public float AccomplishItem()
	{
		m_placedItem = null;
		float a = 0f;
		CharacterControllerWorldMap[] componentsInChildren = GetComponentsInChildren<CharacterControllerWorldMap>();
		Object.Destroy(base.gameObject, Mathf.Max(a, base.gameObject.PlayAnimationOrAnimatorState("EventNode_Accomplished")));
		return Mathf.Max(a, base.gameObject.PlayAnimationOrAnimatorState("EventNode_Accomplished"));
	}

	public float OpenCollectible()
	{
		return m_AdditionalProp.PlayAnimationOrAnimatorState("Collectible_Open");
	}
}
