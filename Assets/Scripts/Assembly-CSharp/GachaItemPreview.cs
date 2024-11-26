using System.Collections.Generic;
using ABH.GameDatas;
using UnityEngine;

public class GachaItemPreview : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem m_FXBasic;

	[SerializeField]
	private ParticleSystem m_FXSet;

	[SerializeField]
	private CHMotionTween m_MotionTween;

	[SerializeField]
	private LootDisplayContoller m_LootDisplay;

	[SerializeField]
	private float m_Distance = 800f;

	private IInventoryItemGameData m_Model;

	[SerializeField]
	private Animator m_Animator;

	public void SetModel(IInventoryItemGameData equipment)
	{
		m_Model = equipment;
		m_LootDisplay.SetModel(equipment, new List<IInventoryItemGameData>(), LootDisplayType.None);
	}

	public void SetPosition(Vector3 posVector3)
	{
		base.transform.localPosition = posVector3;
	}

	public void StartInDirection(Vector3 direction, bool left)
	{
		m_MotionTween.m_EndOffset = direction * m_Distance * ((float)Screen.width / (float)Screen.height / 1.7777778f);
		m_MotionTween.Play();
		CancelInvoke("DisableAfterDone");
		base.gameObject.SetActive(true);
		if (m_Model is EquipmentGameData)
		{
			EquipmentGameData equipmentGameData = m_Model as EquipmentGameData;
			m_FXBasic.gameObject.SetActive(!equipmentGameData.IsSetItem);
			m_FXSet.gameObject.SetActive(equipmentGameData.IsSetItem);
		}
		else if (m_Model is BannerItemGameData)
		{
			BannerItemGameData bannerItemGameData = m_Model as BannerItemGameData;
			m_FXBasic.gameObject.SetActive(!bannerItemGameData.IsSetItem);
			m_FXSet.gameObject.SetActive(bannerItemGameData.IsSetItem);
		}
		m_FXBasic.Play();
		m_FXSet.Play();
		if ((bool)m_Animator)
		{
			m_Animator.Play((!left) ? "GachaItemPreview_Right" : "GachaItemPreview_Left", 0, 0f);
		}
		Invoke("DisableAfterDone", m_MotionTween.m_DurationInSeconds + 0.5f);
	}

	private void DisableAfterDone()
	{
		base.gameObject.SetActive(false);
	}
}
