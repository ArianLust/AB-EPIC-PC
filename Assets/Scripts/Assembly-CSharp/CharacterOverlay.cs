using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

public class CharacterOverlay : MonoBehaviour
{
	private Camera m_InterfaceCamera;

	[SerializeField]
	private BattleEffectBlind m_EffectBlindPrefab;

	[SerializeField]
	private UIGrid m_EffectGrid;

	[SerializeField]
	private UILabel m_Header;

	[SerializeField]
	private UILabel m_LevelText;

	[SerializeField]
	private UILabel m_HealthText;

	[SerializeField]
	private UILabel m_PowerLevelText;

	[SerializeField]
	private Transform m_HealthBarTransform;

	[SerializeField]
	private UILabel m_BossLevel;

	[SerializeField]
	private UISprite m_HealthBar;

	[SerializeField]
	private List<UISprite> m_BackGroundCenterSprites = new List<UISprite>();

	[SerializeField]
	private UISprite m_EffectHeaderBody;

	[SerializeField]
	private Transform m_BottomRoot;

	[SerializeField]
	private Transform m_EffectHeader;

	[SerializeField]
	private Transform m_EffectHeaderTextRoot;

	[SerializeField]
	private float m_MirroredOffset = 16f;

	[SerializeField]
	private float m_MirroredTextOffset = 20f;

	[SerializeField]
	private ContainerControl m_ContainerControl;

	[SerializeField]
	private ContainerControl m_EffectContainerControl;

	[SerializeField]
	private ContainerControl m_AllOverlaysContainerControl;

	[SerializeField]
	private Transform m_MasteryRoot;

	[SerializeField]
	private UILabel m_MasteryRank;

	[SerializeField]
	private UISprite m_MasteryProgress;

	[SerializeField]
	private List<SkillBlind> m_SkillBlinds = new List<SkillBlind>();

	[SerializeField]
	private Vector2 blindSize;

	private Vector3 initialContainerControlPos;

	private Vector3 initialContainerControlSize;

	private List<Vector3> initialBackGroundPositions = new List<Vector3>();

	private List<float> initialBackGroundScales = new List<float>();

	private Vector3 initialBottomRootPosition;

	public Transform m_EffectsRoot;

	[SerializeField]
	private float m_effectsHeaderWidth;

	private Vector3 initialEffectsRootPos;

	[SerializeField]
	private float m_sideBorder = 20f;

	public Color m_PigColor = Color.green;

	public Color m_RedColor = Color.red;

	public Color m_YellowColor = Color.yellow;

	public Color m_BlackColor = Color.black;

	public Color m_WhiteColor = Color.white;

	public Color m_BlueColor = Color.blue;

	private float m_EffectGridBorder = 10f;

	private Vector3 initialEffectsContainerControlPos;

	private Vector3 initialHeaderPos;

	private void Awake()
	{
		m_InterfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
		for (int i = 0; i < m_BackGroundCenterSprites.Count; i++)
		{
			initialBackGroundPositions.Add(m_BackGroundCenterSprites[i].gameObject.transform.localPosition);
			initialBackGroundScales.Add(m_BackGroundCenterSprites[i].height);
		}
		initialContainerControlPos = m_ContainerControl.transform.localPosition;
		initialContainerControlSize = m_ContainerControl.m_Size;
		initialHeaderPos = m_EffectHeader.localPosition;
		initialEffectsContainerControlPos = m_EffectContainerControl.transform.localPosition;
		initialEffectsRootPos = m_EffectsRoot.localPosition;
		if ((bool)m_BottomRoot)
		{
			initialBottomRootPosition = m_BottomRoot.transform.localPosition;
		}
	}

	internal void ShowCharacterOverlay(Transform root, ICombatant combatant, Camera interfaceCamera, bool pvp)
	{
		Vector3 vector = m_InterfaceCamera.ScreenToWorldPoint(interfaceCamera.WorldToScreenPoint(root.position));
		if ((combatant is PigCombatant || combatant is BossCombatant) && !pvp)
		{
			SetPigOrBossCombatant(combatant);
		}
		else if (combatant is BannerCombatant)
		{
			SetBannerCombatant((BannerCombatant)combatant);
		}
		else if (combatant is BirdCombatant || pvp)
		{
			SetBirdOrPvpCombatant(combatant);
		}
		m_HealthBar.fillAmount = combatant.CurrentHealth / combatant.ModifiedHealth;
		m_HealthText.text = DIContainerInfrastructure.GetFormatProvider().GetBattleStatsFormat(combatant.CurrentHealth) + "/" + DIContainerInfrastructure.GetFormatProvider().GetBattleStatsFormat(combatant.ModifiedHealth);
		m_LevelText.text = combatant.CharacterModel.Level.ToString("0");
		IInventoryItemGameData data;
		if (!combatant.IsBanner && combatant is BirdCombatant && DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_mastery_badge", out data))
		{
			m_MasteryRoot.gameObject.SetActive(true);
			m_MasteryRank.text = combatant.CombatantClass.Data.Level.ToString();
			m_MasteryProgress.fillAmount = combatant.CombatantClass.MasteryProgressNextRank();
		}
		else if (!combatant.IsBanner && combatant.IsPvPBird)
		{
			m_MasteryRoot.gameObject.SetActive(true);
			m_MasteryRank.text = combatant.CombatantClass.Data.Level.ToString();
			m_MasteryProgress.fillAmount = combatant.CombatantClass.MasteryProgressNextRank();
		}
		else
		{
			m_MasteryRoot.gameObject.SetActive(false);
		}
		int num = SetSkillInfos(combatant);
		for (int i = 0; i < m_BackGroundCenterSprites.Count; i++)
		{
			m_BackGroundCenterSprites[i].height = (int)(initialBackGroundScales[i] - SkillOverlayOffset(num));
			m_BackGroundCenterSprites[i].cachedTransform.localPosition = initialBackGroundPositions[i] + new Vector3(0f, SkillOverlayOffset(num) * 0.5f, 0f);
		}
		if ((bool)m_BottomRoot)
		{
			m_BottomRoot.localPosition = initialBottomRootPosition + new Vector3(0f, SkillOverlayOffset(num), 0f);
		}
		m_ContainerControl.m_Size = initialContainerControlSize - new Vector3(0f, SkillOverlayOffset(num), 0f);
		m_ContainerControl.transform.localPosition = PositionRelativeToAnchorPositionFixedOnScreen(vector, m_sideBorder, num);
		if (combatant.CurrrentEffects.Values.Any((BattleEffectGameData e) => !string.IsNullOrEmpty(e.m_IconAssetId) && !string.IsNullOrEmpty(e.m_IconAtlasId)))
		{
			m_EffectsRoot.gameObject.SetActive(true);
			m_EffectsRoot.localPosition = PositionEffectsRelativeToAnchorPositionFixedOnScreen(vector, m_MirroredOffset) - Mathf.Sign(vector.x) * new Vector3(m_sideBorder, 0f, 0f);
			m_EffectHeader.localPosition = Vector3.Scale(initialHeaderPos, new Vector3(0f - Mathf.Sign(vector.x), 1f, 1f)) - Mathf.Sign(vector.x) * new Vector3(m_effectsHeaderWidth / 2f, 0f, 0f);
			StartCoroutine(UpdateSkillEffects(combatant, vector));
		}
		else
		{
			m_EffectsRoot.gameObject.SetActive(false);
		}
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	private void SetBirdOrPvpCombatant(ICombatant combatant)
	{
		if (combatant is PigCombatant)
		{
			m_Header.text = (combatant.CharacterModel as PigGameData).GetClassName();
		}
		else
		{
			m_Header.text = (combatant.CharacterModel as BirdGameData).GetClassName();
		}
		if (combatant.CombatantNameId.Contains("_red"))
		{
			m_Header.color = m_RedColor;
		}
		else if (combatant.CombatantNameId.Contains("_yellow"))
		{
			m_Header.color = m_YellowColor;
		}
		else if (combatant.CombatantNameId.Contains("_black"))
		{
			m_Header.color = m_BlackColor;
		}
		else if (combatant.CombatantNameId.Contains("_white"))
		{
			m_Header.color = m_WhiteColor;
		}
		else if (combatant.CombatantNameId.Contains("_blue"))
		{
			m_Header.color = m_BlueColor;
		}
		else
		{
			m_Header.color = m_PigColor;
		}
		m_PowerLevelText.gameObject.SetActive(true);
		int birdPowerLevel = DIContainerInfrastructure.GetPowerLevelCalculator().GetBirdPowerLevel(combatant.CharacterModel);
		m_PowerLevelText.text = DIContainerInfrastructure.GetLocaService().Tr("player_stat_powerlevel").Replace("{value_1}", birdPowerLevel.ToString());
		if ((bool)m_HealthBarTransform)
		{
			m_HealthBarTransform.transform.localPosition = new Vector3(-85f, 225f, 0f);
		}
	}

	private void SetPigOrBossCombatant(ICombatant combatant)
	{
		m_Header.text = combatant.CombatantName;
		if (combatant.CombatantNameId.Contains("_yellow"))
		{
			m_Header.color = m_YellowColor;
		}
		else
		{
			m_Header.color = m_PigColor;
		}
		if ((bool)m_PowerLevelText)
		{
			m_PowerLevelText.gameObject.SetActive(false);
		}
		if (combatant is BossCombatant && (bool)m_BossLevel)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss == null)
			{
				m_BossLevel.text = "0";
			}
			else
			{
				m_BossLevel.text = DIContainerInfrastructure.GetCurrentPlayer().Data.WorldBoss.DeathCount.ToString();
			}
		}
		if ((bool)m_HealthBarTransform)
		{
			m_HealthBarTransform.transform.localPosition = new Vector3(0f, 225f, 0f);
		}
	}

	private void SetBannerCombatant(BannerCombatant combatant)
	{
		m_Header.text = combatant.CombatantName;
		m_Header.color = m_YellowColor;
		m_PowerLevelText.gameObject.SetActive(true);
		int bannerPowerLevel = DIContainerInfrastructure.GetPowerLevelCalculator().GetBannerPowerLevel(combatant.CharacterModel as BannerGameData);
		m_PowerLevelText.text = DIContainerInfrastructure.GetLocaService().Tr("player_stat_powerlevel").Replace("{value_1}", bannerPowerLevel.ToString());
		if ((bool)m_HealthBarTransform)
		{
			m_HealthBarTransform.transform.localPosition = new Vector3(-85f, 225f, 0f);
		}
	}

	private IEnumerator UpdateSkillEffects(ICombatant owner, Vector3 anchorPos)
	{
		foreach (Transform child in m_EffectGrid.transform)
		{
			Object.Destroy(child.gameObject);
		}
		yield return new WaitForEndOfFrame();
		foreach (BattleEffectGameData effect in owner.CurrrentEffects.Values)
		{
			if (!string.IsNullOrEmpty(effect.m_IconAssetId) && !string.IsNullOrEmpty(effect.m_IconAtlasId))
			{
				BattleEffectBlind newBlind = Object.Instantiate(m_EffectBlindPrefab);
				newBlind.transform.parent = m_EffectGrid.transform;
				newBlind.transform.localPosition = Vector3.zero;
				newBlind.transform.localScale = Vector3.one;
				newBlind.ShowEffectBlind(effect, owner);
			}
		}
		yield return new WaitForEndOfFrame();
		m_EffectGrid.Reposition();
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset, int numOfSkills)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * -1f * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (m_ContainerControl.m_Size.x * 0.5f + offset)), initialContainerControlPos.y - (float)(3 - numOfSkills) * blindSize.y / 2f, initialContainerControlPos.z);
	}

	private Vector3 PositionEffectsRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (m_EffectContainerControl.m_Size.x * 0.5f) - Mathf.Max(Mathf.Sign(anchorPosition.x), 0f) * offset), initialEffectsContainerControlPos.y, initialEffectsContainerControlPos.z);
	}

	private float SkillOverlayOffset(int numberOfSkills)
	{
		return (float)(3 - numberOfSkills) * blindSize.y;
	}

	private int SetSkillInfos(ICombatant combatant)
	{
		int result = 3;
		if (combatant is BirdCombatant)
		{
			for (int i = 0; i < m_SkillBlinds.Count; i++)
			{
				SkillBlind skillBlind = m_SkillBlinds[i];
				skillBlind.gameObject.SetActive(true);
				skillBlind.ShowSkillOverlay(combatant.GetSkill(i), combatant, false);
			}
		}
		else if (combatant is PigCombatant)
		{
			PigCombatant pigCombatant = combatant as PigCombatant;
			List<SkillBattleDataBase> list = new List<SkillBattleDataBase>();
			foreach (SkillBattleDataBase skill in pigCombatant.GetSkills())
			{
				list.Add(skill);
			}
			for (int j = 0; j < list.Count && j < m_SkillBlinds.Count; j++)
			{
				SkillBlind skillBlind2 = m_SkillBlinds[j];
				skillBlind2.gameObject.SetActive(true);
				skillBlind2.ShowSkillOverlay(list[j], combatant, list[j].Model.Balancing.EffectType == SkillEffectTypes.Passive);
			}
			result = list.Count;
			for (int k = list.Count; k < m_SkillBlinds.Count; k++)
			{
				m_SkillBlinds[k].gameObject.SetActive(false);
			}
		}
		else if (combatant is BossCombatant)
		{
			BossCombatant bossCombatant = combatant as BossCombatant;
			List<SkillBattleDataBase> list2 = new List<SkillBattleDataBase>();
			foreach (SkillBattleDataBase skill2 in bossCombatant.GetSkills())
			{
				list2.Add(skill2);
			}
			for (int l = 0; l < list2.Count && l < m_SkillBlinds.Count; l++)
			{
				SkillBlind skillBlind3 = m_SkillBlinds[l];
				skillBlind3.gameObject.SetActive(true);
				skillBlind3.ShowSkillOverlay(list2[l], combatant, list2[l].Model.Balancing.EffectType == SkillEffectTypes.Passive);
			}
			result = list2.Count;
			for (int m = list2.Count; m < m_SkillBlinds.Count; m++)
			{
				m_SkillBlinds[m].gameObject.SetActive(false);
			}
		}
		else if (combatant is BannerCombatant)
		{
			List<SkillBattleDataBase> list3 = new List<SkillBattleDataBase>();
			foreach (SkillBattleDataBase skill3 in combatant.GetSkills())
			{
				list3.Add(skill3);
			}
			for (int n = 0; n < list3.Count && n < m_SkillBlinds.Count; n++)
			{
				SkillBlind skillBlind4 = m_SkillBlinds[n];
				skillBlind4.gameObject.SetActive(true);
				skillBlind4.ShowSkillOverlay(list3[n], combatant, list3[n].Model.Balancing.EffectType == SkillEffectTypes.Passive);
			}
			result = list3.Count;
			for (int num = list3.Count; num < m_SkillBlinds.Count; num++)
			{
				m_SkillBlinds[num].gameObject.SetActive(false);
			}
		}
		return result;
	}

	public void Hide()
	{
		GetComponent<Animation>().Play("InfoOverlay_Leave");
		Invoke("Disable", GetComponent<Animation>()["InfoOverlay_Leave"].length);
	}

	private void Disable()
	{
		base.gameObject.SetActive(false);
	}
}
