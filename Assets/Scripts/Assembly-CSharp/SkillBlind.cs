using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

public class SkillBlind : MonoBehaviour
{
	[SerializeField]
	private UISprite m_SkillIcon;

	[SerializeField]
	private UISprite m_SkillTargetIcon;

	[SerializeField]
	private GameObject m_SkillTargetRoot;

	[SerializeField]
	private GameObject m_PerkBackgroundRoot;

	[SerializeField]
	private UILabel m_SkillName;

	[SerializeField]
	private UILabel m_SkillDescription;

	[SerializeField]
	private UISprite m_Background;

	[SerializeField]
	private Color m_OffensiveColor = Color.red;

	[SerializeField]
	private Color m_SupportColor = Color.blue;

	[SerializeField]
	private Color m_PassiveColor = Color.green;

	[SerializeField]
	private Color m_NoneColor = Color.grey;

	[SerializeField]
	private GameObject m_ChargeProgressRoot;

	[SerializeField]
	private UISprite m_ChargeProgress;

	[SerializeField]
	private List<GameObject> m_ChargeProgressDivider = new List<GameObject>();

	public void ShowSkillOverlay(SkillBattleDataBase skill, ICombatant invoker, bool isPassive)
	{
		if (skill == null)
		{
			if (isPassive)
			{
				if (m_SkillName != null)
				{
					m_SkillName.text = DIContainerInfrastructure.GetLocaService().Tr("sko_header_no_passive", "No Passive Skill");
				}
				m_SkillDescription.text = DIContainerInfrastructure.GetLocaService().Tr("sko_text_no_passive", "This Pig doesn't have a Passive Skill");
			}
			else
			{
				if (m_SkillName != null)
				{
					m_SkillName.text = string.Empty;
				}
				m_SkillDescription.text = string.Empty;
			}
			m_SkillIcon.gameObject.SetActive(false);
			m_ChargeProgressRoot.SetActive(false);
			m_Background.color = m_NoneColor;
			return;
		}
		m_SkillIcon.gameObject.SetActive(true);
		if (m_SkillName != null)
		{
			m_SkillName.text = skill.GetLocalizedName(invoker);
		}
		int duration = 0;
		if (skill.TryGetChargeDurationLeft(out duration, invoker))
		{
			if ((bool)m_ChargeProgressRoot)
			{
				m_ChargeProgressRoot.SetActive(true);
				for (int i = 0; i < m_ChargeProgressDivider.Count; i++)
				{
					m_ChargeProgressDivider[i].SetActive(i == skill.GetChargeDuration() - 2);
				}
				m_ChargeProgress.fillAmount = 1f - (float)duration / (float)skill.GetChargeDuration();
			}
		}
		else if ((bool)m_ChargeProgressRoot)
		{
			m_ChargeProgressRoot.SetActive(false);
		}
		m_SkillDescription.text = skill.GetLocalizedDescription(invoker);
		if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(skill.Model.Balancing.IconAtlasId))
		{
			GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(skill.Model.Balancing.IconAtlasId) as GameObject;
			m_SkillIcon.atlas = gameObject.GetComponent<UIAtlas>();
		}
		if ((bool)m_PerkBackgroundRoot && skill.Model.IsPseudoPerk())
		{
			m_PerkBackgroundRoot.SetActive(true);
			m_SkillIcon.spriteName = BannerItemGameData.GetPerkIconNameByPerk(skill.Model.GetPerkType());
			m_SkillIcon.MakePixelPerfect();
		}
		else
		{
			if ((bool)m_PerkBackgroundRoot)
			{
				m_PerkBackgroundRoot.SetActive(false);
			}
			m_SkillIcon.spriteName = skill.Model.m_SkillIconName;
			m_SkillIcon.MakePixelPerfect();
		}
		bool flag = skill.Model.SkillParameters != null && skill.Model.SkillParameters.ContainsKey("all");
		bool flag2 = skill.Model.Balancing.TargetType == SkillTargetTypes.Passive || skill.Model.Balancing.TargetType == SkillTargetTypes.Support;
		bool flag3 = (flag2 && invoker is PigCombatant) || (!flag2 && invoker is BirdCombatant);
		if ((bool)m_SkillTargetRoot)
		{
			m_SkillTargetRoot.gameObject.SetActive(false);
		}
		string text = ((!flag3) ? "Target_Bird" : "Target_Pig");
		if (flag)
		{
			text += "s";
		}
		if ((bool)m_Background)
		{
			switch (skill.Model.Balancing.TargetType)
			{
			case SkillTargetTypes.Attack:
				m_Background.color = m_OffensiveColor;
				break;
			case SkillTargetTypes.Passive:
				m_Background.color = m_PassiveColor;
				break;
			case SkillTargetTypes.Support:
				m_Background.color = m_SupportColor;
				break;
			}
		}
		if (m_SkillTargetIcon != null)
		{
			m_SkillTargetIcon.spriteName = text;
		}
	}
}
