using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using UnityEngine;

public class CharacterStatOverlay : MonoBehaviour
{
	private Camera m_InterfaceCamera;

	public UILabel m_Header;

	public UILabel m_StatText;

	public UILabel m_StatName;

	public UILabel m_StatComposition;

	public UISprite m_StatIcon;

	public UISprite m_Arrow;

	public ContainerControl m_ContainerControl;

	public ContainerControl m_AllOverlaysContainerControl;

	private Vector3 initialSize;

	public Vector2 blindSize;

	private Vector3 initialContainerControlPos;

	private Vector3 initialContainerControlSize;

	private Vector3 initialArrowSize;

	public float m_OffsetLeft = 50f;

	private void Awake()
	{
		m_InterfaceCamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
		initialSize = m_ContainerControl.m_Size;
		initialContainerControlPos = m_ContainerControl.transform.localPosition;
		initialContainerControlSize = m_ContainerControl.m_Size;
		if ((bool)m_Arrow)
		{
			initialArrowSize = m_Arrow.cachedTransform.localScale;
		}
	}

	internal void ShowStatOverlay(Transform root, BirdGameData bird, StatType stattype, Camera orientatedCamera)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		BirdCombatant birdCombatant = new BirdCombatant(bird).SetPvPBird(DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP);
		Dictionary<string, float> currentStatBuffs = new Dictionary<string, float>();
		birdCombatant.CurrentStatBuffs = currentStatBuffs;
		birdCombatant.RefreshHealth();
		if ((bool)m_Header)
		{
			m_Header.text = DIContainerInfrastructure.GetLocaService().GetCharacterName(bird.BalancingData.LocaId);
		}
		switch (stattype)
		{
		case StatType.Attack:
			m_StatIcon.spriteName = "Character_Damage_Large";
			m_StatName.text = DIContainerInfrastructure.GetLocaService().Tr("gen_stat_attack", "Attack");
			dictionary.Add("{value_1}", bird.CharacterAttack.ToString("0"));
			dictionary.Add("{value_2}", bird.MainHandItem.ItemMainStat.ToString("0"));
			dictionary.Add("{value_3}", ((float)Mathf.RoundToInt(birdCombatant.ModifiedAttack) - bird.ClassIndependentAttack).ToString("0"));
			m_StatText.text = Mathf.RoundToInt(birdCombatant.ModifiedAttack).ToString("0");
			dictionary.Add("{value_4}", DIContainerInfrastructure.GetLocaService().GetCharacterName(bird.BalancingData.LocaId));
			dictionary.Add("{value_5}", bird.Data.Level.ToString("0"));
			m_StatComposition.text = DIContainerInfrastructure.GetLocaService().Tr("ovrl_batr_composition_attack", dictionary);
			break;
		case StatType.Health:
			m_StatIcon.spriteName = "Character_Health_Large";
			m_StatName.text = DIContainerInfrastructure.GetLocaService().Tr("gen_stat_health", "Health");
			dictionary.Add("{value_1}", bird.CharacterHealth.ToString("0"));
			dictionary.Add("{value_2}", bird.OffHandItem.ItemMainStat.ToString("0"));
			dictionary.Add("{value_3}", ((float)Mathf.RoundToInt(birdCombatant.ModifiedHealth) - bird.ClassIndependentHealth).ToString("0"));
			m_StatText.text = Mathf.RoundToInt(birdCombatant.ModifiedHealth).ToString("0");
			dictionary.Add("{value_4}", DIContainerInfrastructure.GetLocaService().GetCharacterName(bird.BalancingData.LocaId));
			dictionary.Add("{value_5}", bird.Data.Level.ToString("0"));
			m_StatComposition.text = DIContainerInfrastructure.GetLocaService().Tr("ovrl_batr_composition", dictionary);
			break;
		}
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		DebugLog.Log(string.Concat("Begin show StatOverlay ", stattype, " for: ", bird.Name));
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = Vector3.Scale(initialContainerControlPos, new Vector3(-1f * Mathf.Sign(anchorPosition.x), 1f, 1f));
		if ((bool)m_Arrow)
		{
			m_Arrow.cachedTransform.localPosition = PositionArrowRelativeToAnchorPositionFixedOnScreen(anchorPosition, m_OffsetLeft);
			m_Arrow.cachedTransform.localScale = Vector3.Scale(initialArrowSize, new Vector3(-1f * Mathf.Sign(anchorPosition.x), 1f, 1f));
			m_Arrow.gameObject.SetActive(false);
		}
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	internal void ShowStatOverlay(Transform root, BannerGameData banner, Camera orientatedCamera)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		BannerCombatant bannerCombatant = new BannerCombatant(banner);
		Dictionary<string, float> currentStatBuffs = new Dictionary<string, float>();
		bannerCombatant.CurrentStatBuffs = currentStatBuffs;
		bannerCombatant.RefreshHealth();
		if ((bool)m_Header)
		{
			m_Header.text = DIContainerInfrastructure.GetLocaService().GetCharacterName(banner.BalancingData.LocaId);
		}
		m_StatIcon.spriteName = "Character_Health_Large";
		m_StatName.text = DIContainerInfrastructure.GetLocaService().Tr("gen_stat_health", "Health");
		dictionary.Add("{value_1}", banner.BannerTip.ItemMainStat.ToString("0"));
		dictionary.Add("{value_2}", banner.BannerCenter.ItemMainStat.ToString("0"));
		dictionary.Add("{value_3}", banner.BannerEmblem.ItemMainStat.ToString("0"));
		m_StatText.text = Mathf.RoundToInt(bannerCombatant.ModifiedHealth).ToString("0");
		m_StatComposition.text = DIContainerInfrastructure.GetLocaService().Tr("ovrl_batr_composition_banner", dictionary);
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = Vector3.Scale(initialContainerControlPos, new Vector3(-1f * Mathf.Sign(anchorPosition.x), 1f, 1f));
		if ((bool)m_Arrow)
		{
			m_Arrow.cachedTransform.localPosition = PositionArrowRelativeToAnchorPositionFixedOnScreen(anchorPosition, m_OffsetLeft);
			m_Arrow.cachedTransform.localScale = Vector3.Scale(initialArrowSize, new Vector3(-1f * Mathf.Sign(anchorPosition.x), 1f, 1f));
			m_Arrow.gameObject.SetActive(false);
		}
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnAnchor(Vector3 anchorPosition, float offset)
	{
		return new Vector3(anchorPosition.x + -1f * Mathf.Sign(anchorPosition.x) * (m_ContainerControl.m_Size.x * 0.5f + offset * 2f), anchorPosition.y, initialContainerControlPos.z);
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * -1f * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (m_ContainerControl.m_Size.x * 0.5f + offset)), initialContainerControlPos.y, initialContainerControlPos.z);
	}

	private Vector3 PositionArrowRelativeToAnchorPositionFixedOnAnchor(Vector3 anchorPosition)
	{
		return new Vector3(anchorPosition.x + -1f * Mathf.Sign(anchorPosition.x) * initialArrowSize.x, anchorPosition.y, m_Arrow.cachedTransform.localPosition.z);
	}

	private Vector3 PositionArrowRelativeToAnchorPositionFixedOnScreen(Vector3 anchorPosition, float offset)
	{
		return new Vector3(Mathf.Sign(anchorPosition.x) * -1f * (m_AllOverlaysContainerControl.m_Size.x * 0.5f + -1f * (initialArrowSize.x + m_ContainerControl.m_Size.x + offset)), anchorPosition.y, m_Arrow.cachedTransform.localPosition.z);
	}

	private float SkillOverlayOffset(int numberOfSkills)
	{
		return (float)(3 - numberOfSkills) * blindSize.y;
	}

	public void Hide()
	{
		if (base.gameObject.activeInHierarchy)
		{
			GetComponent<Animation>().Play("InfoOverlay_Leave");
			Invoke("Disable", GetComponent<Animation>()["InfoOverlay_Leave"].length);
		}
	}

	private void Disable()
	{
		base.gameObject.SetActive(false);
	}
}
