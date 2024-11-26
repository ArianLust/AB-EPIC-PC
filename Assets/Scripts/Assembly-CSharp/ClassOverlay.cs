using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using UnityEngine;

public class ClassOverlay : MonoBehaviour
{
	private Camera m_InterfaceCamera;

	public UILabel m_Header;

	public SkillBlind m_SkillOffensiveBlind;

	public SkillBlind m_SkillSupportBlind;

	public UISprite m_Arrow;

	public ContainerControl m_ContainerControl;

	public ContainerControl m_AllOverlaysContainerControl;

	private Vector3 initialSize;

	public Vector2 blindSize;

	private Vector3 initialContainerControlPos;

	private Vector3 initialContainerControlSize;

	private Vector3 initialArrowSize;

	public Color m_PigColor = Color.green;

	public Color m_RedColor = Color.red;

	public Color m_YellowColor = Color.yellow;

	public Color m_BlackColor = Color.black;

	public Color m_WhiteColor = Color.white;

	public Color m_BlueColor = Color.blue;

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

	public void ShowClassOverlay(Transform root, IInventoryItemGameData classItem, BirdGameData bird, Camera orientatedCamera)
	{
		if (classItem is ClassItemGameData)
		{
			FillContent(classItem as ClassItemGameData, bird);
		}
		else if (classItem is SkinItemGameData)
		{
			FillContent(classItem as SkinItemGameData, bird);
		}
		DebugLog.Log("Begin show ClassOverlay " + classItem.ItemLocalizedName + " for: " + bird.Name);
		Vector3 anchorPosition = m_InterfaceCamera.ScreenToWorldPoint(orientatedCamera.WorldToScreenPoint(root.position));
		base.transform.localPosition = new Vector3(anchorPosition.x, anchorPosition.y, base.transform.localPosition.z);
		m_ContainerControl.transform.localPosition = PositionRelativeToAnchorPositionFixedOnAnchor(anchorPosition, m_OffsetLeft);
		if ((bool)m_Arrow)
		{
			m_Arrow.cachedTransform.localPosition = PositionArrowRelativeToAnchorPositionFixedOnScreen(anchorPosition, m_OffsetLeft);
			m_Arrow.cachedTransform.localScale = Vector3.Scale(initialArrowSize, new Vector3(-1f * Mathf.Sign(anchorPosition.x), 1f, 1f));
			m_Arrow.gameObject.SetActive(false);
		}
		GetComponent<Animation>().Play("InfoOverlay_Enter");
	}

	private void FillContent(SkinItemGameData skinItem, BirdGameData bird)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		InventoryGameData inventoryGameData = currentPlayer.InventoryGameData;
		if ((bool)m_Header)
		{
			m_Header.text = skinItem.ItemLocalizedName;
		}
		ClassItemGameData classItem = new ClassItemGameData(skinItem.BalancingData.OriginalClass);
		ClassItemGameData classItemGameData = inventoryGameData.Items[InventoryItemType.Class].Where((IInventoryItemGameData c) => c.ItemBalancing.NameId == classItem.BalancingData.NameId).FirstOrDefault() as ClassItemGameData;
		if (classItemGameData == null)
		{
			ClassItemBalancingData potentialUpgadeClass = (from b in DIContainerBalancing.Service.GetBalancingDataList<ClassItemBalancingData>()
				where b.ReplacementClassNameId == classItem.BalancingData.NameId
				select b).FirstOrDefault();
			if (potentialUpgadeClass != null)
			{
				classItem = new ClassItemGameData(potentialUpgadeClass.NameId);
				classItemGameData = inventoryGameData.Items[InventoryItemType.Class].Where((IInventoryItemGameData c) => c.ItemBalancing.NameId == potentialUpgadeClass.NameId).FirstOrDefault() as ClassItemGameData;
			}
		}
		if (classItemGameData != null)
		{
			classItem.Data.Level = classItemGameData.ItemData.Level;
		}
		if (classItem.Data.Level == 0 && DIContainerLogic.InventoryService.GetItemValue(inventoryGameData, "unlock_mastery_badge") > 0)
		{
			currentPlayer.AdvanceBirdMasteryToHalfOfHighest(classItem);
		}
		BirdGameData birdGameData = new BirdGameData(bird.Data);
		birdGameData.OverrideClassItem = classItem;
		birdGameData.ClassSkin = skinItem;
		BirdCombatant birdCombatant = new BirdCombatant(birdGameData).SetPvPBird(DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP);
		Dictionary<string, float> currentStatBuffs = new Dictionary<string, float>();
		birdCombatant.CurrentStatBuffs = currentStatBuffs;
		birdCombatant.RefreshHealth();
		m_SkillOffensiveBlind.ShowSkillOverlay(birdCombatant.GetSkill(0), birdCombatant, false);
		m_SkillSupportBlind.ShowSkillOverlay(birdCombatant.GetSkill(1), birdCombatant, false);
	}

	private void FillContent(ClassItemGameData classItem, BirdGameData bird)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if ((bool)m_Header)
		{
			m_Header.text = classItem.ItemLocalizedName;
		}
		BirdGameData birdGameData = new BirdGameData(bird.Data);
		birdGameData.OverrideClassItem = classItem;
		BirdCombatant birdCombatant = new BirdCombatant(birdGameData).SetPvPBird(DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP);
		Dictionary<string, float> currentStatBuffs = new Dictionary<string, float>();
		birdCombatant.CurrentStatBuffs = currentStatBuffs;
		birdCombatant.RefreshHealth();
		m_SkillOffensiveBlind.ShowSkillOverlay(birdCombatant.GetSkill(0), birdCombatant, false);
		m_SkillSupportBlind.ShowSkillOverlay(birdCombatant.GetSkill(1), birdCombatant, false);
	}

	private Vector3 PositionRelativeToAnchorPositionFixedOnAnchor(Vector3 anchorPosition, float offset)
	{
		if (Mathf.Sign(anchorPosition.x) < 0f)
		{
			return new Vector3(initialContainerControlPos.x, initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(initialContainerControlSize.y * 0.5f, (0f - initialContainerControlSize.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
		}
		return new Vector3(0f - initialContainerControlPos.x, initialContainerControlPos.y + InfoOverlayMgr.GetOverlayY(anchorPosition.y, new Vector2(initialContainerControlSize.y * 0.5f, (0f - initialContainerControlSize.y) * 0.5f), m_AllOverlaysContainerControl) - anchorPosition.y, initialContainerControlPos.z);
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
