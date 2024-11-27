using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

[Serializable]
public class EventPreviewCharacterSlot : MonoBehaviour
{
	public Transform RootTransform;

	public string CharacterName;

	public bool showEquipment;

	public bool blockIdle;

	public bool ignoreFirstDelay;

	public float AnimationStartDelay;

	public bool LoopAnimationList = true;

	public List<AnimationNameDelayPair> AnimationDelyDelayPairs = new List<AnimationNameDelayPair>();

	private CharacterControllerCamp m_model;

	public void SetModel(CharacterControllerCamp characterController)
	{
		m_model = characterController;
		characterController.transform.parent = RootTransform;
		characterController.transform.localRotation = Quaternion.identity;
		characterController.transform.localPosition = Vector3.zero;
		characterController.transform.localScale = Vector3.one;
		characterController.m_ShowEquipment = showEquipment;
		characterController.SetModel(CharacterName, false);
		BirdGameData birdGameData = characterController.GetModel() as BirdGameData;
		if (birdGameData != null && birdGameData.InventoryGameData.Items[InventoryItemType.Skin].Count > 1)
		{
			SkinItemGameData equippedSkin = birdGameData.ClassSkin;
			SkinItemGameData item = birdGameData.InventoryGameData.Items[InventoryItemType.Skin].FirstOrDefault((IInventoryItemGameData s) => s != equippedSkin) as SkinItemGameData;
			DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { item }, InventoryItemType.Skin, birdGameData.InventoryGameData);
		}
		characterController.DisableTabAndHold();
		UnityHelper.SetLayerRecusively(characterController.gameObject, RootTransform.gameObject.layer);
		StartCoroutine("AnimateRepeated");
	}

	public void ReInitialize()
	{
		if ((bool)m_model)
		{
			StopCoroutine("AnimateRepeated");
			StartCoroutine("AnimateRepeated");
		}
	}

	private IEnumerator AnimateRepeated()
	{
		int index = 0;
		yield return new WaitForSeconds(AnimationStartDelay);
		bool ignoredFirst = false;
		while (AnimationDelyDelayPairs.Count > 0)
		{
			if (AnimationDelyDelayPairs[index] == null)
			{
				index = (index + 1) % AnimationDelyDelayPairs.Count;
				continue;
			}
			if (!ignoreFirstDelay || ignoredFirst)
			{
				yield return new WaitForSeconds(AnimationDelyDelayPairs[index].Delay);
			}
			ignoredFirst = true;
			m_model.m_AssetController.PlayAnimation(AnimationDelyDelayPairs[index].AnimationName);
			yield return new WaitForSeconds(m_model.m_AssetController.GetAnimationLength(AnimationDelyDelayPairs[index].AnimationName));
			if (!blockIdle)
			{
				Invoke("PlayIdleAnimation", m_model.m_AssetController.GetAnimationLength(AnimationDelyDelayPairs[index].AnimationName));
			}
		}
		if (AnimationDelyDelayPairs.Count == 0)
		{
			PlayIdleAnimation();
		}
	}

	private void PlayIdleAnimation()
	{
		m_model.PlayIdleAnimation();
	}
}
