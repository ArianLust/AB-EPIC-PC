using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class DailyObjectiveBaseElement : MonoBehaviour
{
	[SerializeField]
	private GameObject m_oneIconRoot;

	[SerializeField]
	private GameObject m_twoIconRoot;

	[SerializeField]
	private UISprite m_oneIconMainSprite;

	[SerializeField]
	private GameObject m_oneIconTargetSprite;

	[SerializeField]
	private GameObject m_oneIconWithoutSprite;

	[SerializeField]
	private GameObject m_oneIconClubSprite;

	[SerializeField]
	private GameObject m_oneIconProtectSprite;

	[SerializeField]
	private GameObject m_twoIconAttackSprite;

	[SerializeField]
	private UISprite m_twoIconMainSpriteA;

	[SerializeField]
	private GameObject m_twoIconTargetSpriteA;

	[SerializeField]
	private GameObject m_twoIconWithoutSpriteA;

	[SerializeField]
	private UISprite m_twoIconMainSpriteB;

	[SerializeField]
	private GameObject m_twoIconTargetSpriteB;

	[SerializeField]
	private GameObject m_twoIconWithoutSpriteB;

	[SerializeField]
	protected UILabel m_rewardLabel;

	[SerializeField]
	protected UILabel m_progressLabel;

	public PvPObjectivesGameData m_GameData;

	public virtual void Init(PvPObjectivesGameData gameData)
	{
		m_GameData = gameData;
		if (string.IsNullOrEmpty(m_GameData.Requirement2))
		{
			m_twoIconRoot.SetActive(false);
			m_oneIconRoot.SetActive(true);
			m_oneIconMainSprite.spriteName = "DailyObjective_" + m_GameData.IconSpriteName;
			if (m_GameData.RequirementType == ObjectivesRequirement.useClass)
			{
				string replacementName = string.Empty;
				if (ClassItemGameData.CheckForReplacement("class_" + m_GameData.Requirement1, out replacementName))
				{
					m_oneIconMainSprite.spriteName = "DailyObjective_" + replacementName.Replace("Headgear_", string.Empty);
				}
			}
			m_oneIconMainSprite.MakePixelPerfect();
			m_oneIconTargetSprite.SetActive(m_GameData.RequirementType == ObjectivesRequirement.killBird);
			m_oneIconWithoutSprite.SetActive(m_GameData.RequirementType == ObjectivesRequirement.notUseBird);
			m_oneIconClubSprite.SetActive(m_GameData.RequirementType == ObjectivesRequirement.killWithBird);
			m_oneIconProtectSprite.SetActive(m_GameData.RequirementType == ObjectivesRequirement.protectBird);
			return;
		}
		m_twoIconRoot.SetActive(true);
		m_oneIconRoot.SetActive(false);
		string[] array = m_GameData.IconSpriteName.Split('&');
		if (array.Length < 2)
		{
			DebugLog.Error("Incorrect spritename Format for 2 requirements");
			return;
		}
		m_twoIconMainSpriteA.spriteName = "DailyObjective_" + array[0];
		m_twoIconMainSpriteB.spriteName = "DailyObjective_" + array[1];
		if (m_GameData.RequirementType == ObjectivesRequirement.useClass)
		{
			string replacementName2 = string.Empty;
			if (ClassItemGameData.CheckForReplacement("class_" + m_GameData.Requirement1, out replacementName2))
			{
				m_oneIconMainSprite.spriteName = "DailyObjective_" + replacementName2.Replace("Headgear_", string.Empty);
			}
			if (ClassItemGameData.CheckForReplacement("class_" + m_GameData.Requirement2, out replacementName2))
			{
				m_twoIconMainSpriteB.spriteName = "DailyObjective_" + replacementName2.Replace("Headgear_", string.Empty);
			}
		}
		m_twoIconMainSpriteA.MakePixelPerfect();
		m_twoIconMainSpriteB.MakePixelPerfect();
		m_twoIconAttackSprite.SetActive(m_GameData.RequirementType == ObjectivesRequirement.killWithBird);
		m_twoIconTargetSpriteA.SetActive(m_GameData.RequirementType == ObjectivesRequirement.killBird);
		m_twoIconWithoutSpriteA.SetActive(m_GameData.RequirementType == ObjectivesRequirement.notUseBird);
		m_twoIconTargetSpriteB.SetActive(m_GameData.RequirementType == ObjectivesRequirement.killBird);
		m_twoIconWithoutSpriteB.SetActive(m_GameData.RequirementType == ObjectivesRequirement.notUseBird);
		if (m_GameData.RequirementType == ObjectivesRequirement.killWithBird)
		{
			m_twoIconMainSpriteA.spriteName += "_Attack";
			m_twoIconMainSpriteB.spriteName += "_Target";
		}
	}
}
