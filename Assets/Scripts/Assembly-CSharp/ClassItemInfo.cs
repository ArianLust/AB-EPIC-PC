using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

public class ClassItemInfo : MonoBehaviour
{
	public UISprite SupportSkillSprite;

	public UISprite OffensiveSkillSprite;

	public UILabel SupportSkillName;

	public UILabel OffensiveSkillName;

	public UISprite OffensiveSkillTargetSprite;

	public UISprite SupportSkillTargetSprite;

	public BirdGameData m_SelectedBird;

	public ClassItemGameData m_SelectedClass;

	public Transform m_ClassNameLowerPosition;

	public UILabel m_ClassName;

	public Transform MasteryRoot;

	public UILabel MasteryRankValue;

	public UISprite MasteryRankProgressBar;

	public GameObject m_SwitchSkinButtonObject;

	public UIInputTrigger m_SwitchSkinButtonTrigger;

	public GameObject m_NewSkinMarker;

	private BirdEquipmentPreviewUI m_birdEquipmentPreviewUI;

	private bool HasInitialized;

	private bool HasStarted;

	public BirdWindowUI m_BirdUI;

	public ClassManagerUi m_ClassMgr;

	private InventoryItemSlot m_selectedSlot;

	private SkinInfoPopup m_SkinInfoPopup;

	public void ShowMasteryProgressTooltip()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowMasteryProgressOverlay(MasteryRoot, m_SelectedBird.ClassItem, true);
	}

	public void ShowAttackSkillTooltip()
	{
		if (DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowSkillOverlay(OffensiveSkillSprite.cachedTransform, m_SelectedBird, m_SelectedClass.PrimaryPvPSkill, true);
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowSkillOverlay(OffensiveSkillSprite.cachedTransform, m_SelectedBird, m_SelectedClass.PrimarySkill, true);
		}
	}

	public void ShowSupportSkillTooltip()
	{
		if (DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowSkillOverlay(OffensiveSkillSprite.cachedTransform, m_SelectedBird, m_SelectedClass.SecondaryPvPSkill, true);
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowSkillOverlay(SupportSkillSprite.cachedTransform, m_SelectedBird, m_SelectedClass.SecondarySkill, true);
		}
	}

	public string TargetSpriteName(SkillGameData skill, ICharacter invoker)
	{
		bool flag = skill.SkillParameters != null && skill.SkillParameters.ContainsKey("all");
		bool flag2 = skill.Balancing.TargetType == SkillTargetTypes.Passive || skill.Balancing.TargetType == SkillTargetTypes.Support;
		bool flag3 = (flag2 && invoker is PigGameData) || (!flag2 && invoker is BirdGameData);
		string empty = string.Empty;
		empty = ((!flag3) ? "Target_Bird" : "Target_Pig");
		if (flag)
		{
			empty += "s";
		}
		return empty;
	}

	public void SetModel(ClassItemGameData classItemGameData, BirdGameData selectedBird, BirdEquipmentPreviewUI birdEquipmentPreview, InventoryItemSlot selectedSlot)
	{
		UIAtlas uIAtlas = null;
		m_birdEquipmentPreviewUI = birdEquipmentPreview;
		m_selectedSlot = selectedSlot;
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(classItemGameData.SecondarySkill.Balancing.IconAtlasId))
		{
			GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(classItemGameData.SecondarySkill.Balancing.IconAtlasId) as GameObject;
			uIAtlas = gameObject.GetComponent<UIAtlas>();
		}
		if ((bool)uIAtlas)
		{
			SupportSkillSprite.atlas = uIAtlas;
		}
		SupportSkillSprite.spriteName = classItemGameData.SecondarySkill.m_SkillIconName;
		SupportSkillName.text = classItemGameData.SecondarySkill.SkillLocalizedName;
		UIAtlas uIAtlas2 = null;
		if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(classItemGameData.PrimarySkill.Balancing.IconAtlasId))
		{
			GameObject gameObject2 = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(classItemGameData.PrimarySkill.Balancing.IconAtlasId) as GameObject;
			uIAtlas2 = gameObject2.GetComponent<UIAtlas>();
		}
		if ((bool)uIAtlas2)
		{
			OffensiveSkillSprite.atlas = uIAtlas2;
		}
		OffensiveSkillSprite.spriteName = classItemGameData.PrimarySkill.m_SkillIconName;
		OffensiveSkillName.text = classItemGameData.PrimarySkill.SkillLocalizedName;
		m_SelectedBird = selectedBird;
		m_SelectedClass = classItemGameData;
		SupportSkillTargetSprite.spriteName = TargetSpriteName(classItemGameData.SecondarySkill, m_SelectedBird);
		OffensiveSkillTargetSprite.spriteName = TargetSpriteName(classItemGameData.PrimarySkill, m_SelectedBird);
		m_ClassName.text = selectedBird.GetClassName();
		IInventoryItemGameData data = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(currentPlayer.InventoryGameData, "unlock_mastery_badge", out data))
		{
			MasteryRoot.gameObject.SetActive(true);
			MasteryRankValue.text = classItemGameData.Data.Level.ToString();
			MasteryRankProgressBar.fillAmount = classItemGameData.MasteryProgressNextRank();
			m_ClassName.transform.localPosition = Vector3.zero;
		}
		else
		{
			MasteryRoot.gameObject.SetActive(false);
			m_ClassName.transform.position = m_ClassNameLowerPosition.position;
		}
		bool active = false;
		foreach (IInventoryItemGameData item in DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData.Items[InventoryItemType.Skin])
		{
			if (item.ItemData.IsNew && (item as SkinItemGameData).BalancingData.OriginalClass == classItemGameData.BalancingData.NameId)
			{
				active = true;
				break;
			}
		}
		m_NewSkinMarker.SetActive(active);
		bool flag = m_SelectedBird.HasSkinsAvailable();
		bool flag2 = DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "unlock_skins");
		m_SwitchSkinButtonObject.SetActive(flag && flag2);
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("switch_skin", string.Empty);
		m_SwitchSkinButtonTrigger.Clicked -= SwitchSkin;
		m_SwitchSkinButtonTrigger.Clicked += SwitchSkin;
	}

	private void SwitchSkin()
	{
		SkinItemGameData nextSkin = m_SelectedBird.GetNextSkin();
		if (nextSkin == null)
		{
			ShowInfoPopup();
			return;
		}
		nextSkin.Data.IsNew = false;
		m_NewSkinMarker.SetActive(false);
		if (m_BirdUI != null)
		{
			m_BirdUI.UpdateSlotIndicators();
		}
		else if (m_ClassMgr != null)
		{
			m_ClassMgr.UpdateSlotIndicators();
		}
		DIContainerLogic.InventoryService.EquipBirdWithItem(new List<IInventoryItemGameData> { nextSkin }, InventoryItemType.Skin, m_SelectedBird.InventoryGameData);
		DIContainerInfrastructure.GetCurrentPlayer().Data.EquippedSkins[m_SelectedClass.BalancingData.NameId] = nextSkin.BalancingData.NameId;
		m_birdEquipmentPreviewUI.RefreshStats(true);
		m_birdEquipmentPreviewUI.PlayCharacterChanged();
		m_birdEquipmentPreviewUI.m_CurrentCharacterController.m_AssetController.PlayCheerAnim();
		m_selectedSlot.UpdateIcon(nextSkin.ItemAssetName);
		m_ClassName.text = m_SelectedBird.GetClassName();
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
	}

	private void ShowInfoPopup()
	{
		if (m_SkinInfoPopup == null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.AddUILevel("Popup_SkinInfo", OnSkinInfoPopupLoaded);
		}
		else
		{
			m_SkinInfoPopup.Show(m_SelectedClass, m_ClassMgr);
		}
	}

	private void OnDestroy()
	{
		m_SwitchSkinButtonTrigger.Clicked -= SwitchSkin;
	}

	public void OnSkinInfoPopupLoaded()
	{
		m_SkinInfoPopup = Object.FindObjectOfType(typeof(SkinInfoPopup)) as SkinInfoPopup;
		m_SkinInfoPopup.gameObject.SetActive(true);
		m_SkinInfoPopup.Show(m_SelectedClass, m_ClassMgr);
	}
}
