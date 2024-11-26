using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using UnityEngine;

public class BannerEquipmentPreviewUI : MonoBehaviour
{
	[SerializeField]
	private Animation m_CharacterAnimation;

	[SerializeField]
	public CharacterControllerCamp m_CharacterController;

	[SerializeField]
	private StatisticsElement m_HealthStat;

	[SerializeField]
	private UITapHoldTrigger m_HealthOverlayTrigger;

	private float m_OldHealth;

	private BannerGameData m_Model;

	private BannerCombatant m_Combatant;

	public void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if (m_HealthOverlayTrigger != null)
		{
			m_HealthOverlayTrigger.OnTapBegin += m_HealthOverlayTrigger_OnTapBegin;
			m_HealthOverlayTrigger.OnTapEnd += OnStatTapEnd;
		}
	}

	private void OnStatTapEnd()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideCharacterAttributeOverlay();
	}

	private void m_HealthOverlayTrigger_OnTapBegin()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowCharacterAttributeOverlay(m_HealthStat.transform, m_Model, true);
	}

	public void DeRegisterEventHandler()
	{
		if (m_HealthOverlayTrigger != null)
		{
			m_HealthOverlayTrigger.OnTapBegin -= m_HealthOverlayTrigger_OnTapBegin;
			m_HealthOverlayTrigger.OnTapEnd -= OnStatTapEnd;
		}
	}

	public void SetModel(BannerGameData banner)
	{
		m_Model = banner;
		m_CharacterController.gameObject.SetActive(true);
		m_CharacterController.SetModel(banner, false);
		m_CharacterController.transform.localPosition = Vector3.zero;
		m_CharacterController.transform.localPosition -= new Vector3(0f, 0f, 10f);
		m_CharacterController.gameObject.layer = LayerMask.NameToLayer("Interface");
		m_CharacterController.transform.localScale = Vector3.one;
		m_CharacterController.m_AssetController.transform.localScale = Vector3.one;
		m_CharacterController.m_AssetController.gameObject.layer = LayerMask.NameToLayer("Interface");
		m_Combatant = new BannerCombatant(m_Model);
		Dictionary<string, float> currentStatBuffs = new Dictionary<string, float>();
		m_Combatant.CurrentStatBuffs = currentStatBuffs;
		m_Combatant.RefreshHealth();
		m_OldHealth = Mathf.RoundToInt(m_Combatant.ModifiedHealth);
		RefreshStats(false, true);
		RegisterEventHandler();
	}

	public IEnumerator Enter()
	{
		m_CharacterController.gameObject.SetActive(true);
		GetComponent<Animation>().Play("CharacterDisplay_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["CharacterDisplay_Enter"].clip.length);
	}

	public void RefreshStats(bool showChange, bool init = false)
	{
		if (m_Model != null && !(m_CharacterController == null))
		{
			m_CharacterController.SetModel(m_Model, false);
			UnityHelper.SetLayerRecusively(m_CharacterController.gameObject, LayerMask.NameToLayer("Interface"));
			m_Combatant = new BannerCombatant(m_Model);
			Dictionary<string, float> currentStatBuffs = new Dictionary<string, float>();
			m_Combatant.CurrentStatBuffs = currentStatBuffs;
			m_Combatant.RefreshHealth();
			float num = Mathf.RoundToInt(m_Combatant.ModifiedHealth);
			if (m_HealthStat != null && (num != m_OldHealth || init))
			{
				m_HealthStat.RefreshStat(showChange, false, num, m_OldHealth);
			}
			m_OldHealth = num;
		}
	}

	public IEnumerator Leave()
	{
		GetComponent<Animation>().Play("CharacterDisplay_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["CharacterDisplay_Leave"].clip.length);
		m_CharacterController.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		DeRegisterEventHandler();
	}
}
