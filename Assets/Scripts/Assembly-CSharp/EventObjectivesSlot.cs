using System.Collections.Generic;
using System.Linq;
using ABH.Shared.Generic;
using UnityEngine;

public class EventObjectivesSlot : MonoBehaviour
{
	[SerializeField]
	private CharacterControllerWorldMap m_CharacterControllerPrefab;

	[SerializeField]
	private Transform m_CharacterControllerContainer;

	[SerializeField]
	private Transform m_SpacerTransform;

	[SerializeField]
	private UILabel m_difficultyLabel;

	public List<SizeTypeToSpacerScale> m_SizeTypeToSpacerScaleList = new List<SizeTypeToSpacerScale>();

	public float SetModelAndGetExtent(string assetName, int difficulty, float energyCost)
	{
		GetComponent<GenericOverlayInvoker>().m_LocaIdent = "eventbattle_info_" + difficulty + "_tt";
		m_difficultyLabel.text = DIContainerInfrastructure.GetLocaService().Tr("eventbattle_info_difficulty_" + difficulty);
		if (assetName.StartsWith("bird_") || assetName.StartsWith("pig_"))
		{
			CharacterControllerWorldMap character = Object.Instantiate(m_CharacterControllerPrefab);
			character.SetModel(assetName, false);
			character.transform.parent = m_CharacterControllerContainer ?? base.transform;
			SizeTypeToSpacerScale sizeTypeToSpacerScale = m_SizeTypeToSpacerScaleList.FirstOrDefault((SizeTypeToSpacerScale s) => s.CharacterSize == character.GetModel().CharacterSize);
			if (sizeTypeToSpacerScale != null)
			{
				character.transform.localScale = new Vector3(sizeTypeToSpacerScale.RootScale, sizeTypeToSpacerScale.RootScale, character.transform.localScale.z);
			}
			character.transform.localPosition = Vector3.zero;
			UnityHelper.SetLayerRecusively(character.gameObject, base.gameObject.layer);
			if (character.transform.name.Contains("drone_doom"))
			{
				character.transform.localScale *= 1.4f;
			}
			if (sizeTypeToSpacerScale != null)
			{
				m_SpacerTransform.localScale = sizeTypeToSpacerScale.SpacerScale;
			}
			if (sizeTypeToSpacerScale != null)
			{
				return m_SpacerTransform.lossyScale.x;
			}
		}
		else
		{
			GameObject gameObject = DIContainerInfrastructure.PropLiteAssetProvider().InstantiateObject(assetName, base.transform, Vector3.zero, Quaternion.identity);
			if (gameObject != null)
			{
				UnityHelper.SetLayerRecusively(gameObject.gameObject, base.gameObject.layer);
			}
			SizeTypeToSpacerScale sizeTypeToSpacerScale2 = m_SizeTypeToSpacerScaleList.FirstOrDefault((SizeTypeToSpacerScale s) => s.CharacterSize == CharacterSizeType.Medium);
			if (sizeTypeToSpacerScale2 != null)
			{
				gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + sizeTypeToSpacerScale2.SpacerScale.y / 2f, gameObject.transform.localPosition.z);
				if (sizeTypeToSpacerScale2 != null)
				{
					m_SpacerTransform.localScale = sizeTypeToSpacerScale2.SpacerScale;
				}
				if (sizeTypeToSpacerScale2 != null)
				{
					return m_SpacerTransform.lossyScale.x;
				}
			}
		}
		return 0f;
	}
}
