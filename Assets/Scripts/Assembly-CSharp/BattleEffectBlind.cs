using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

public class BattleEffectBlind : MonoBehaviour
{
	[SerializeField]
	private UISprite m_EffectIcon;

	[SerializeField]
	private UILabel m_EffectName;

	[SerializeField]
	private UILabel m_Duration;

	[SerializeField]
	private GameObject m_DurationRoot;

	[SerializeField]
	private UISprite m_Background;

	[SerializeField]
	private Color m_CurseColor = Color.red;

	[SerializeField]
	private Color m_BlessingColor = Color.green;

	[SerializeField]
	private Color m_PassiveColor = Color.blue;

	[SerializeField]
	private Color m_NoneColor = Color.grey;

	[SerializeField]
	private Color m_SetColor = Color.yellow;

	public void ShowEffectBlind(BattleEffectGameData effect, ICombatant owner)
	{
		if (string.IsNullOrEmpty(effect.m_IconAssetId))
		{
			return;
		}
		int turnsLeft = effect.GetTurnsLeft();
		m_Duration.text = effect.GetTurnsLeft().ToString("0");
		m_DurationRoot.SetActive(turnsLeft > 0);
		if (effect.m_IconAssetId == "Stun")
		{
			if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset("Skills_Pigs_01"))
			{
				GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject("Skills_Pigs_01") as GameObject;
				m_EffectIcon.atlas = gameObject.GetComponent<UIAtlas>();
			}
			m_EffectIcon.spriteName = "AttackCurse";
		}
		else
		{
			if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(effect.m_IconAtlasId))
			{
				GameObject gameObject2 = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(effect.m_IconAtlasId) as GameObject;
				m_EffectIcon.atlas = gameObject2.GetComponent<UIAtlas>();
			}
			m_EffectIcon.spriteName = effect.m_IconAssetId;
		}
		m_EffectName.text = effect.m_LocalizedName;
		switch (effect.m_EffectType)
		{
		case SkillEffectTypes.Blessing:
			m_Background.color = m_BlessingColor;
			break;
		case SkillEffectTypes.Curse:
			m_Background.color = m_CurseColor;
			break;
		case SkillEffectTypes.None:
			m_Background.color = m_NoneColor;
			break;
		case SkillEffectTypes.Passive:
			m_Background.color = m_PassiveColor;
			break;
		case SkillEffectTypes.Environmental:
			m_Background.color = m_PassiveColor;
			break;
		case SkillEffectTypes.SetPassive:
			m_Background.color = m_SetColor;
			break;
		}
	}
}
