using ABH.GameDatas;
using UnityEngine;

public class RulesOverlayInvoker : MonoBehaviour
{
	public UITapHoldTrigger m_TapHoldTrigger;

	public string m_OpenActionName = "ShowTooltip";

	public string m_HideActionName = "HideAllTooltips";

	[HideInInspector]
	public SkillGameData m_EnvironmentalSkill;

	[HideInInspector]
	public int m_AllowedBirdsNum;

	[HideInInspector]
	public string m_RestrictedBirdName;

	[HideInInspector]
	public bool m_IsTapping;

	private void Awake()
	{
		m_TapHoldTrigger = GetComponent<UITapHoldTrigger>();
		RegisterEventHandlers();
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		m_TapHoldTrigger.OnTapBegin += OnTapBegin;
		m_TapHoldTrigger.OnTapReleased += OnTapEnd;
	}

	private void OnTapEnd()
	{
		if ((bool)DIContainerInfrastructure.GetCoreStateMgr() && (bool)DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.SendMessage(m_HideActionName);
		}
		m_IsTapping = false;
	}

	private void OnTapBegin()
	{
		Invoke(m_OpenActionName, 0f);
		m_IsTapping = true;
	}

	private void DeRegisterEventHandlers()
	{
		if ((bool)m_TapHoldTrigger)
		{
			m_TapHoldTrigger.OnTapBegin -= OnTapBegin;
			m_TapHoldTrigger.OnTapReleased -= OnTapEnd;
		}
	}

	private void OnDestroy()
	{
		if (m_IsTapping)
		{
			OnTapEnd();
			if ((bool)m_TapHoldTrigger)
			{
				m_TapHoldTrigger.ResetUICamera();
			}
		}
		DeRegisterEventHandlers();
	}

	public void ShowBirdNumberTooltip()
	{
		string localizedText = DIContainerInfrastructure.GetLocaService().Tr("rules_bird_amount_tt").Replace("{value_1}", m_AllowedBirdsNum.ToString());
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(base.transform, localizedText, true);
	}

	public void ShowBirdRestrictedTooltip()
	{
		string localizedText = DIContainerInfrastructure.GetLocaService().Tr("rules_bird_illegal_tt").Replace("{value_1}", m_RestrictedBirdName);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(base.transform, localizedText, true);
	}

	public void ShowBirdRequiredTooltip()
	{
		string localizedText = DIContainerInfrastructure.GetLocaService().Tr("rules_bird_needed_tt").Replace("{value_1}", m_RestrictedBirdName);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowGenericOverlay(base.transform, localizedText, true);
	}

	public void ShowEnvironmentalEffectTooltip()
	{
		if (m_EnvironmentalSkill != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowSkillOverlay(base.transform, null, m_EnvironmentalSkill, true);
		}
	}
}
