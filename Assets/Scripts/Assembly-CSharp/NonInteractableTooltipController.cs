using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class NonInteractableTooltipController : MonoBehaviour
{
	[SerializeField]
	private Transform m_IconRoot;

	[SerializeField]
	private Transform m_IconRootCentered;

	[SerializeField]
	private UILabel m_TextBox;

	[SerializeField]
	private UILabel m_Header;

	[SerializeField]
	private CharacterControllerCamp m_CampCharacterControllerPrefab;

	private string m_CurrentAssetName;

	private GameObject m_CurrentAssetObject;

	public float Enter()
	{
		base.gameObject.SetActive(true);
		GetComponent<Animation>().Play("Tooltip_Enter");
		return GetComponent<Animation>()["Tooltip_Enter"].clip.length;
	}

	public float Leave()
	{
		GetComponent<Animation>().Play("Tooltip_Leave");
		Invoke("Disable", GetComponent<Animation>()["Tooltip_Leave"].clip.length);
		return GetComponent<Animation>()["Tooltip_Leave"].clip.length;
	}

	public void Disable()
	{
		base.gameObject.SetActive(false);
		RemoveAsset();
	}

	public void SetTooltip(string iconAssetID, string headerLocaId, string descLocaId, Dictionary<string, string> replacementDictionary = null)
	{
		m_CurrentAssetObject = CreateFeatureObjectAsset(iconAssetID);
		m_Header.text = DIContainerInfrastructure.GetLocaService().Tr(headerLocaId);
		string text = DIContainerInfrastructure.GetLocaService().Tr(descLocaId);
		if (replacementDictionary != null)
		{
			foreach (KeyValuePair<string, string> item in replacementDictionary)
			{
				text = text.Replace(item.Key, item.Value);
			}
		}
		m_TextBox.text = text;
	}

	private GameObject CreateFeatureObjectAsset(string assetName)
	{
		if (assetName.StartsWith("bird_"))
		{
			CharacterControllerCamp characterControllerCamp = Object.Instantiate(m_CampCharacterControllerPrefab);
			characterControllerCamp.transform.parent = m_IconRoot;
			characterControllerCamp.transform.localPosition = Vector3.zero;
			characterControllerCamp.transform.localScale = Vector3.one;
			characterControllerCamp.SetModel(new BirdGameData(assetName, DIContainerInfrastructure.GetCurrentPlayer().Data.Level), false);
			UnityHelper.SetLayerRecusively(characterControllerCamp.gameObject, base.gameObject.layer);
			m_CurrentAssetName = string.Empty;
			return characterControllerCamp.gameObject;
		}
		if (assetName.StartsWith("pig_"))
		{
			CharacterControllerCamp characterControllerCamp2 = Object.Instantiate(m_CampCharacterControllerPrefab);
			characterControllerCamp2.transform.parent = m_IconRoot;
			characterControllerCamp2.transform.localPosition = Vector3.zero;
			characterControllerCamp2.transform.localScale = Vector3.one;
			PigBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<PigBalancingData>(assetName);
			characterControllerCamp2.SetModel(balancingData.NameId, false);
			characterControllerCamp2.transform.localScale = Vector3.one;
			UnityHelper.SetLayerRecusively(characterControllerCamp2.gameObject, base.gameObject.layer);
			m_CurrentAssetName = string.Empty;
			return characterControllerCamp2.gameObject;
		}
		if (DIContainerInfrastructure.PropLiteAssetProvider().ContainsAsset(assetName))
		{
			GameObject gameObject = DIContainerInfrastructure.PropLiteAssetProvider().InstantiateObject(assetName, m_IconRootCentered, Vector3.zero, Quaternion.identity);
			gameObject.SetActive(true);
			gameObject.transform.parent = m_IconRootCentered;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			UnityHelper.SetLayerRecusively(gameObject, base.gameObject.layer);
			m_CurrentAssetName = assetName;
			return gameObject;
		}
		return null;
	}

	private void RemoveAsset()
	{
		if (string.IsNullOrEmpty(m_CurrentAssetName))
		{
			DebugLog.Log("Destroy Character");
			m_CurrentAssetObject.GetComponent<CharacterControllerCamp>().DestroyCharacter();
		}
		else
		{
			DIContainerInfrastructure.PropLiteAssetProvider().DestroyObject(m_CurrentAssetName, m_CurrentAssetObject);
		}
	}
}
