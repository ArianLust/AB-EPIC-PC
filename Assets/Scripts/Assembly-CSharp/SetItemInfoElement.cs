using System.Collections;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class SetItemInfoElement : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_UpdateIndicators;

	[SerializeField]
	private UISprite[] m_BirdIcon;

	[SerializeField]
	private Transform[] m_ItemSpriteSpawnRoot;

	public void Init(IInventoryItemGameData mainItem, IInventoryItemGameData secondItem, bool isNew)
	{
		if (isNew)
		{
			base.transform.name = "1_" + base.transform.name;
		}
		GameObject[] updateIndicators = m_UpdateIndicators;
		foreach (GameObject gameObject in updateIndicators)
		{
			gameObject.SetActive(isNew);
		}
		EquipmentBalancingData equipmentBalancingData = mainItem.ItemBalancing as EquipmentBalancingData;
		if (equipmentBalancingData == null)
		{
			DebugLog.Error("Wrong item type!!");
			return;
		}
		BirdBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<BirdBalancingData>(equipmentBalancingData.RestrictedBirdId);
		UISprite[] birdIcon = m_BirdIcon;
		foreach (UISprite uISprite in birdIcon)
		{
			uISprite.spriteName = "Target_" + balancingData.AssetId;
		}
		GameObject gameObject2 = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(mainItem.ItemAssetName, m_ItemSpriteSpawnRoot[0], Vector3.zero, Quaternion.identity, false);
		Renderer componentInChildren = gameObject2.GetComponentInChildren<Renderer>();
		StartCoroutine(SetRecipeShader(componentInChildren));
		gameObject2 = DIContainerInfrastructure.GetEquipmentAssetProvider().InstantiateObject(secondItem.ItemAssetName, m_ItemSpriteSpawnRoot[1], Vector3.zero, Quaternion.identity, false);
		componentInChildren = gameObject2.GetComponentInChildren<Renderer>();
		StartCoroutine(SetRecipeShader(componentInChildren));
	}

	private IEnumerator SetRecipeShader(Renderer rend)
	{
		yield return new WaitForEndOfFrame();
		if (!(rend.material.shader == DIContainerLogic.GetVisualEffectsBalancing().m_RecipeItemMaterial.shader))
		{
			rend.material = new Material(rend.sharedMaterial);
			rend.material.shader = DIContainerLogic.GetVisualEffectsBalancing().m_RecipeItemMaterial.shader;
			rend.material.color = DIContainerLogic.GetVisualEffectsBalancing().m_RecipeItemMaterial.color;
		}
	}
}
