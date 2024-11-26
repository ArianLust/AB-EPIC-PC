using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Character;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class InfoOverlayMgr : MonoBehaviour
{
	public CharacterOverlay m_CharacterOverlay;

	public CharacterOverlay m_BossOverlay;

	public CharacterStatOverlay m_CharacterStatOverlay;

	public GenericOverlay m_GenericOverlay;

	public GenericOverlay m_GenericOverlayWithHeader;

	public EquipmentOverlay m_EquipmentOverlay;

	public ClassOverlay m_ClassOverlay;

	public MissingCurrencyOverlay m_MissingCurrencyOverlay;

	public MaterialOverlay m_MaterialOverlay;

	public MaterialOverlay m_MaterialCraftableOverlay;

	public EquipmentOverlay m_SetEquipmentOverlay;

	public ConsumableOverlay m_ConsumableOverlay;

	public SkillOverlay m_SkillOverlay;

	public MasteryProgressOverlay m_MasteryProgressOverlay;

	public MasteryBadgeOverlay m_MasteryBadgeOverlay;

	public GachaOverlay m_GachaOverlay;

	public GachaOverlay m_PvpGachaOverlay;

	public ConsumableOverlay m_ConsumableWithLevelReqOverlay;

	public PerkOverlay m_PerkOverlay;

	public EquipmentOverlay m_SetPerkOverlay;

	public BattleOverlay m_BattleOverlay;

	public BannerItemOverlay m_BannerItemOverlay;

	public BannerItemOverlay m_BannerSetItemOverlay;

	public BannerItemOverlay m_EmblemSetItemOverlay;

	public XPOverlay m_XPOverlay;

	public ArenaLeagueOverlay m_ArenaLeagueOverlay;

	public ObjectiveOverlay m_DailyObjectiveOverlay;

	public TrophyOverlay m_TrophyOverlay;

	public MissingArenaEnergyOverlay m_MissingArenaChargesOverlay;

	public GameObject m_InputBlocker;

	public UIInputTrigger m_FallbackHide;

	private Camera m_ReferencedCamera;

	private UICamera m_UICamera;

	[SerializeField]
	private float m_DisableBlockerDelay = 0.25f;

	private void Awake()
	{
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays = this;
		m_FallbackHide.Clicked -= m_FallbackHide_Clicked;
		m_FallbackHide.Clicked += m_FallbackHide_Clicked;
	}

	private void m_FallbackHide_Clicked()
	{
		HideAllTooltips();
	}

	public void ShowCharacterOverlay(Transform root, ICombatant combatant, bool interfaceCam, bool pvp)
	{
		Camera interfaceCamera = AdjustToCamera(interfaceCam);
		if (combatant is BossCombatant)
		{
			m_BossOverlay.gameObject.SetActive(true);
			m_BossOverlay.ShowCharacterOverlay(root, combatant, interfaceCamera, pvp);
		}
		else
		{
			m_CharacterOverlay.gameObject.SetActive(true);
			m_CharacterOverlay.ShowCharacterOverlay(root, combatant, interfaceCamera, pvp);
		}
	}

	public void HideCharacterOverlay()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_CharacterOverlay.Hide();
		m_BossOverlay.Hide();
	}

	public void ShowSkillOverlay(Transform root, ICharacter character, int index, bool interfaceCam)
	{
		m_SkillOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_SkillOverlay.ShowSkillOverlay(root, character, index, orientatedCamera);
	}

	public void ShowMasteryProgressOverlay(Transform root, ClassItemGameData classItem, bool interfaceCam)
	{
		m_MasteryProgressOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_MasteryProgressOverlay.ShowMasteryProgressOverlay(root, classItem, orientatedCamera);
	}

	public void HideMasteryProgressOverlay()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_MasteryProgressOverlay.Hide();
	}

	public void ShowMasteryBadgeOverlay(Transform root, IInventoryItemGameData badge, bool interfaceCam)
	{
		m_MasteryBadgeOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_MasteryBadgeOverlay.ShowMasteryBadgeOverlay(root, badge, orientatedCamera);
	}

	public void HideMasteryBadgeOverlay()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_MasteryBadgeOverlay.Hide();
	}

	public void ShowSkillOverlay(Transform root, ICharacter character, SkillGameData skill, bool interfaceCam)
	{
		m_SkillOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_SkillOverlay.ShowSkillOverlay(root, character, skill, orientatedCamera);
	}

	public void HideSkillOverlay()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_CharacterOverlay.Hide();
	}

	public void ShowClassOverlay(Transform root, IInventoryItemGameData classItem, BirdGameData bird, bool interfaceCam)
	{
		m_ClassOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_ClassOverlay.ShowClassOverlay(root, classItem, bird, orientatedCamera);
	}

	public void HideClassOverlay()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_ClassOverlay.Hide();
	}

	public void ShowDailyObjectiveOverlay(Transform root, bool interfaceCam)
	{
		m_DailyObjectiveOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_DailyObjectiveOverlay.ShowOverlay(root, orientatedCamera);
	}

	public void HideDailyObjectiveOverlay()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_DailyObjectiveOverlay.Hide();
	}

	public void ShowCharacterAttributeOverlay(Transform root, BirdGameData bird, StatType stattype, bool interfaceCam)
	{
		m_CharacterStatOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_CharacterStatOverlay.ShowStatOverlay(root, bird, stattype, orientatedCamera);
	}

	public void ShowCharacterAttributeOverlay(Transform root, BannerGameData banner, bool interfaceCam)
	{
		m_CharacterStatOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_CharacterStatOverlay.ShowStatOverlay(root, banner, orientatedCamera);
	}

	public void HideCharacterAttributeOverlay()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_CharacterStatOverlay.Hide();
	}

	public void ShowBattleOverlay(Transform root, HotspotGameData hotspot, BattleBalancingData battle, string overrideIdent, bool interfaceCam)
	{
		m_BattleOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_BattleOverlay.ShowBattleOverlay(root, hotspot, battle, overrideIdent, orientatedCamera);
	}

	public void ShowXPOverlayOverlay(Transform root, PlayerGameData player, bool interfaceCam)
	{
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_XPOverlay.gameObject.SetActive(true);
		m_XPOverlay.ShowXPOverlay(root, player, orientatedCamera);
	}

	public void HideBattleOverlay()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_BattleOverlay.Hide();
	}

	public void ShowPerkOverlay(Transform root, EquipmentGameData equipment, bool interfaceCam)
	{
		m_PerkOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_PerkOverlay.ShowPerkOverlay(root, equipment, orientatedCamera);
	}

	public void HidePerkOverlay()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_PerkOverlay.Hide();
	}

	public void ShowSetPerkOverlay(Transform root, EquipmentGameData equipment, bool interfaceCam, bool isArena)
	{
		m_SetPerkOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_SetPerkOverlay.ShowEquipmentOverlay(root, equipment, orientatedCamera, isArena);
	}

	public void HideSetPerkOverlay()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_SetPerkOverlay.Hide();
	}

	public void ShowGachaOverlay(Transform root, bool interfaceCam, bool isAdvanced)
	{
		m_GachaOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_GachaOverlay.ShowGachaOverlay(root, orientatedCamera, isAdvanced);
	}

	public void ShowPvpGachaOverlay(Transform root, bool interfaceCam, bool isAdvanced)
	{
		m_PvpGachaOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_PvpGachaOverlay.ShowGachaOverlay(root, orientatedCamera, isAdvanced);
	}

	internal void ShowItemOverlay(Transform root, IInventoryItemGameData item, bool interfaceCam, bool isArena, int levelreq = 0)
	{
		Camera camera = AdjustToCamera(interfaceCam);
		switch (item.ItemBalancing.ItemType)
		{
		case InventoryItemType.BannerTip:
		case InventoryItemType.Banner:
		case InventoryItemType.BannerEmblem:
		{
			BannerGameData bannerGameData = DIContainerInfrastructure.GetCurrentPlayer().BannerGameData;
			BannerItemGameData bannerItemGameData = item as BannerItemGameData;
			BannerItemOverlay bannerItemOverlay = null;
			bannerItemOverlay = ((bannerItemGameData == null || !bannerItemGameData.IsSetItem) ? m_BannerItemOverlay : ((item.ItemBalancing.ItemType != InventoryItemType.BannerEmblem) ? m_BannerSetItemOverlay : m_EmblemSetItemOverlay));
			if (bannerItemOverlay != null)
			{
				bannerItemOverlay.gameObject.SetActive(true);
				bannerItemOverlay.ShowBannerItemOverlay(root, item as BannerItemGameData, bannerGameData, camera);
			}
			break;
		}
		case InventoryItemType.Skin:
		{
			SkinItemGameData skinItemGameData = item as SkinItemGameData;
			if (skinItemGameData != null)
			{
				ClassItemGameData classItemGameData2 = new ClassItemGameData(skinItemGameData.BalancingData.OriginalClass);
				if (classItemGameData2 != null)
				{
					BirdGameData birdGameData = DIContainerInfrastructure.GetCurrentPlayer().GetBird(classItemGameData2.BalancingData.RestrictedBirdId);
					if (birdGameData == null)
					{
						birdGameData = new BirdGameData("bird_blue", DIContainerInfrastructure.GetCurrentPlayer().Data.Level);
					}
					m_ClassOverlay.gameObject.SetActive(true);
					ShowClassOverlay(root, item as SkinItemGameData, birdGameData, interfaceCam);
				}
			}
			else
			{
				HideItemOverlays();
			}
			break;
		}
		case InventoryItemType.Class:
		{
			ClassItemGameData classItemGameData = item as ClassItemGameData;
			BirdGameData bird = DIContainerInfrastructure.GetCurrentPlayer().GetBird(classItemGameData.BalancingData.RestrictedBirdId);
			if (bird != null)
			{
				m_ClassOverlay.gameObject.SetActive(true);
				ShowClassOverlay(root, item as ClassItemGameData, bird, interfaceCam);
			}
			else
			{
				HideItemOverlays();
			}
			break;
		}
		case InventoryItemType.Consumable:
			if (levelreq <= 0)
			{
				m_ConsumableOverlay.gameObject.SetActive(true);
				m_ConsumableOverlay.ShowConsumableOverlay(root, item as ConsumableItemGameData, camera);
			}
			else
			{
				m_ConsumableWithLevelReqOverlay.gameObject.SetActive(true);
				m_ConsumableWithLevelReqOverlay.ShowConsumableOverlay(root, item as ConsumableItemGameData, camera, levelreq);
			}
			break;
		case InventoryItemType.CraftingRecipes:
			EvaluateRecipeOverlay(root, item, interfaceCam, camera, levelreq);
			break;
		case InventoryItemType.Resources:
		case InventoryItemType.Ingredients:
			EvaluateCraftingItemOverlay(root, interfaceCam, camera, item);
			break;
		case InventoryItemType.MainHandEquipment:
		case InventoryItemType.OffHandEquipment:
		{
			EquipmentGameData equipmentGameData = item as EquipmentGameData;
			if (equipmentGameData != null && equipmentGameData.IsSetItem)
			{
				m_SetEquipmentOverlay.gameObject.SetActive(true);
				m_SetEquipmentOverlay.ShowEquipmentOverlay(root, equipmentGameData, camera, isArena);
			}
			else if (equipmentGameData != null && DIContainerInfrastructure.GetCurrentPlayer().GetBird(equipmentGameData.BalancingData.RestrictedBirdId, true) == null)
			{
				m_GenericOverlay.gameObject.SetActive(true);
				ShowGenericOverlay(root, DIContainerInfrastructure.GetLocaService().Tr("item_tt_missing_bird"), interfaceCam);
			}
			else
			{
				m_EquipmentOverlay.gameObject.SetActive(true);
				m_EquipmentOverlay.ShowEquipmentOverlay(root, equipmentGameData, camera, isArena);
			}
			break;
		}
		case InventoryItemType.CollectionComponent:
			m_MaterialOverlay.gameObject.SetActive(true);
			m_MaterialOverlay.ShowEventCampaignCollectionComponentItemOverlay(root, item as BasicItemGameData, camera);
			break;
		case InventoryItemType.Mastery:
		{
			string masteryClassName = (item as MasteryItemGameData).GetMasteryClassName(DIContainerInfrastructure.GetCurrentPlayer());
			m_GenericOverlay.gameObject.SetActive(true);
			ShowGenericOverlay(root, item.ItemLocalizedTooltipDesc(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData).Replace("{value_2}", masteryClassName), interfaceCam);
			break;
		}
		default:
			m_GenericOverlay.gameObject.SetActive(true);
			ShowGenericOverlay(root, item.ItemLocalizedTooltipDesc(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData), interfaceCam);
			break;
		}
	}

	internal void ShowItemOverlay(Transform root, List<IInventoryItemGameData> items, BasicShopOfferBalancingData balancingData, bool interfaceCam, int levelreq = 0)
	{
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_ConsumableOverlay.gameObject.SetActive(true);
		m_ConsumableOverlay.ShowItemBundleOverlay(root, items, balancingData, orientatedCamera);
	}

	private void EvaluateRecipeOverlay(Transform root, IInventoryItemGameData item, bool interfaceCam, Camera referencedCamera, int levelreq)
	{
		CraftingRecipeGameData craftingRecipeGameData = item as CraftingRecipeGameData;
		Dictionary<string, LootInfoData> loot = DIContainerLogic.GetLootOperationService().GenerateLoot(craftingRecipeGameData.GetResultLoot(), craftingRecipeGameData.Data.Level);
		List<IInventoryItemGameData> itemsFromLoot = DIContainerLogic.GetLootOperationService().GetItemsFromLoot(DIContainerInfrastructure.GetCurrentPlayer(), loot);
		if (craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.MainHandEquipment || craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.OffHandEquipment)
		{
			m_EquipmentOverlay.gameObject.SetActive(true);
			m_EquipmentOverlay.ShowRecipeOverlay(root, craftingRecipeGameData, itemsFromLoot[0] as EquipmentGameData, referencedCamera);
		}
		else if (craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Consumable)
		{
			if (levelreq <= 0)
			{
				m_ConsumableOverlay.gameObject.SetActive(true);
				m_ConsumableOverlay.ShowConsumableOverlay(root, itemsFromLoot[0] as ConsumableItemGameData, referencedCamera);
			}
			else
			{
				m_ConsumableWithLevelReqOverlay.gameObject.SetActive(true);
				m_ConsumableWithLevelReqOverlay.ShowConsumableOverlay(root, itemsFromLoot[0] as ConsumableItemGameData, referencedCamera, levelreq);
			}
		}
		else if (craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Ingredients || craftingRecipeGameData.BalancingData.RecipeCategoryType == InventoryItemType.Resources)
		{
			EvaluateCraftingItemOverlay(root, interfaceCam, referencedCamera, itemsFromLoot[0]);
		}
	}

	private void EvaluateCraftingItemOverlay(Transform root, bool interfaceCam, Camera referencedCamera, IInventoryItemGameData item)
	{
		CraftingItemGameData craftingItemGameData = item as CraftingItemGameData;
		if (craftingItemGameData != null)
		{
			List<KeyValuePair<string, int>> craftingCosts = new List<KeyValuePair<string, int>>();
			DIContainerLogic.CraftingService.AddCraftingCostsForItem(ref craftingCosts, craftingItemGameData.BalancingData.NameId, craftingItemGameData.Data.Level);
			if (craftingCosts.Count > 0)
			{
				m_MaterialCraftableOverlay.gameObject.SetActive(true);
				m_MaterialCraftableOverlay.ShowCraftingItemOverlay(root, craftingItemGameData, referencedCamera);
			}
			else
			{
				m_MaterialOverlay.gameObject.SetActive(true);
				m_MaterialOverlay.ShowCraftingItemOverlay(root, craftingItemGameData, referencedCamera);
			}
		}
		else
		{
			m_GenericOverlay.gameObject.SetActive(true);
			ShowGenericOverlay(root, item.ItemLocalizedTooltipDesc(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData), interfaceCam);
		}
	}

	private void HideItemOverlays()
	{
		m_EquipmentOverlay.Hide();
		m_GenericOverlay.Hide();
		m_ClassOverlay.Hide();
		Invoke("DisableBlocker", m_DisableBlockerDelay);
	}

	internal void ShowGenericOverlay(Transform root, string localizedText, bool interfaceCam)
	{
		m_GenericOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_GenericOverlay.ShowGenericOverlay(root, localizedText, orientatedCamera);
	}

	internal void ShowGenericOverlay(Transform root, string localizedHeader, string localizedText, bool interfaceCam)
	{
		m_GenericOverlayWithHeader.gameObject.SetActive(true);
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_GenericOverlayWithHeader.ShowGenericOverlay(root, localizedHeader, localizedText, orientatedCamera);
	}

	internal void ShowMissingArenaEnergyOverlay()
	{
		m_MissingArenaChargesOverlay.gameObject.SetActive(true);
		m_MissingArenaChargesOverlay.ShowGenericOverlay();
		CancelInvoke("HideMissingArenaChargesOverlay");
		Invoke("HideMissingArenaChargesOverlay", DIContainerLogic.GetPacingBalancing().MissingCurrencyOverlayTime);
	}

	internal void ShowMissingCurrencyOverlay(Transform root, string spriteName, string text, UIAtlas atlas, bool interfaceCam)
	{
		m_MissingCurrencyOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = ((!interfaceCam) ? Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("SceneryCamera")) : Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("UICamera")));
		m_MissingCurrencyOverlay.ShowGenericOverlay(root, spriteName, text, orientatedCamera, atlas);
		CancelInvoke("HideMissingCurrencyOverlay");
		Invoke("HideMissingCurrencyOverlay", DIContainerLogic.GetPacingBalancing().MissingCurrencyOverlayTime);
	}

	internal void ShowMissingCurrencyOverlay(Transform root, BasicItemGameData item, bool interfaceCam)
	{
		m_MissingCurrencyOverlay.gameObject.SetActive(true);
		Camera orientatedCamera = ((!interfaceCam) ? Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("SceneryCamera")) : Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("UICamera")));
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("{value_1}", item.ItemLocalizedName);
		Dictionary<string, string> replacementStrings = dictionary;
		string localizedText = DIContainerInfrastructure.GetLocaService().Tr("gen_tt_missing_currency", replacementStrings);
		if (item.BalancingData.NameId == "event_energy")
		{
			UIAtlas atlasSwitch = null;
			if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset("Consumables"))
			{
				GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject("Consumables") as GameObject;
				atlasSwitch = gameObject.GetComponent<UIAtlas>();
			}
			m_MissingCurrencyOverlay.ShowGenericOverlay(root, "EnergyDrink", localizedText, orientatedCamera, atlasSwitch);
		}
		else
		{
			m_MissingCurrencyOverlay.ShowGenericOverlay(root, item.ItemAssetName, localizedText, orientatedCamera);
		}
		CancelInvoke("HideMissingCurrencyOverlay");
		Invoke("HideMissingCurrencyOverlay", DIContainerLogic.GetPacingBalancing().MissingCurrencyOverlayTime);
	}

	public void HideGenericOverlay()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_GenericOverlay.Hide();
	}

	public void HideMissingCurrencyOverlay()
	{
		m_MissingCurrencyOverlay.Hide();
	}

	public void HideMissingArenaChargesOverlay()
	{
		m_MissingArenaChargesOverlay.Hide();
	}

	public void ShowArenaLeagueOverlay(Transform root, int league, int rank, bool interfaceCam)
	{
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		PvPSeasonManagerGameData currentPvPSeasonGameData = DIContainerInfrastructure.GetCurrentPlayer().CurrentPvPSeasonGameData;
		m_ArenaLeagueOverlay.gameObject.SetActive(true);
		m_ArenaLeagueOverlay.ShowArenaLeagueOverlay(root, currentPvPSeasonGameData, league, rank, orientatedCamera);
	}

	public void HideArenaLeagueOverlay()
	{
		m_ArenaLeagueOverlay.Hide();
	}

	public void ShowTrophyOverlay(Transform root, TrophyData Trophy, bool interfaceCam)
	{
		Camera orientatedCamera = AdjustToCamera(interfaceCam);
		m_TrophyOverlay.gameObject.SetActive(true);
		m_TrophyOverlay.ShowTrophyOverlay(root, Trophy, orientatedCamera);
	}

	public void HideTrophyLeagueOverlay()
	{
		m_TrophyOverlay.Hide();
	}

	private void SetDragControllerActive(bool flag)
	{
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(flag, 2);
		}
	}

	public void DisableBlocker()
	{
		SetDragControllerActive(true);
		m_InputBlocker.SetActive(false);
	}

	public static float GetOverlayY(float currentY, Vector2 dimensionUpDown, ContainerControl cameraRelativeContainerControl)
	{
		if (currentY + dimensionUpDown.x > cameraRelativeContainerControl.transform.localPosition.y + cameraRelativeContainerControl.m_Size.y * 0.5f)
		{
			return cameraRelativeContainerControl.transform.localPosition.y + cameraRelativeContainerControl.m_Size.y * 0.5f - dimensionUpDown.x;
		}
		if (currentY - dimensionUpDown.x < cameraRelativeContainerControl.transform.localPosition.y - cameraRelativeContainerControl.m_Size.y * 0.5f)
		{
			return cameraRelativeContainerControl.transform.localPosition.y - cameraRelativeContainerControl.m_Size.y * 0.5f + dimensionUpDown.x;
		}
		return currentY;
	}

	private Camera AdjustToCamera(bool interfaceCam)
	{
		if (interfaceCam)
		{
			m_ReferencedCamera = Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("UICamera"));
		}
		else
		{
			m_ReferencedCamera = Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("SceneryCamera"));
		}
		SetDragControllerActive(false);
		m_InputBlocker.SetActive(true);
		return m_ReferencedCamera;
	}

	public void HideAllTooltips()
	{
		Invoke("DisableBlocker", m_DisableBlockerDelay);
		m_CharacterStatOverlay.Hide();
		m_EquipmentOverlay.Hide();
		m_GenericOverlay.Hide();
		m_CharacterStatOverlay.Hide();
		m_CharacterOverlay.Hide();
		m_BossOverlay.Hide();
		m_ClassOverlay.Hide();
		m_SetEquipmentOverlay.Hide();
		m_MaterialCraftableOverlay.Hide();
		m_MaterialOverlay.Hide();
		m_ConsumableOverlay.Hide();
		m_GenericOverlayWithHeader.Hide();
		m_SkillOverlay.Hide();
		m_GachaOverlay.Hide();
		m_PvpGachaOverlay.Hide();
		m_ConsumableWithLevelReqOverlay.Hide();
		m_BattleOverlay.Hide();
		m_MasteryBadgeOverlay.Hide();
		m_MasteryProgressOverlay.Hide();
		m_PerkOverlay.Hide();
		m_SetPerkOverlay.Hide();
		m_XPOverlay.Hide();
		m_BannerItemOverlay.Hide();
		m_ArenaLeagueOverlay.Hide();
		m_BannerSetItemOverlay.Hide();
		m_EmblemSetItemOverlay.Hide();
		m_DailyObjectiveOverlay.Hide();
		m_TrophyOverlay.Hide();
		Camera camera = Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("SceneryCamera"));
		if ((bool)camera)
		{
			camera.eventMask = 1 << LayerMask.NameToLayer("Scenery");
		}
	}

	public void HideAllTooltipsInstant()
	{
		DisableBlocker();
		m_CharacterStatOverlay.gameObject.SetActive(false);
		m_EquipmentOverlay.gameObject.SetActive(false);
		m_GenericOverlay.gameObject.SetActive(false);
		m_CharacterStatOverlay.gameObject.SetActive(false);
		m_CharacterOverlay.gameObject.SetActive(false);
		m_BossOverlay.gameObject.SetActive(false);
		m_ClassOverlay.gameObject.SetActive(false);
		m_SetEquipmentOverlay.gameObject.SetActive(false);
		m_MaterialCraftableOverlay.gameObject.SetActive(false);
		m_MaterialOverlay.gameObject.SetActive(false);
		m_ConsumableOverlay.gameObject.SetActive(false);
		m_GenericOverlayWithHeader.gameObject.SetActive(false);
		m_SkillOverlay.gameObject.SetActive(false);
		m_ConsumableWithLevelReqOverlay.gameObject.SetActive(false);
		m_BattleOverlay.gameObject.SetActive(false);
		m_PerkOverlay.gameObject.SetActive(false);
		m_SetPerkOverlay.gameObject.SetActive(false);
		m_XPOverlay.gameObject.SetActive(false);
		m_ArenaLeagueOverlay.gameObject.SetActive(false);
		m_BannerSetItemOverlay.gameObject.SetActive(false);
		m_EmblemSetItemOverlay.gameObject.SetActive(false);
		m_DailyObjectiveOverlay.gameObject.SetActive(false);
		m_TrophyOverlay.gameObject.SetActive(false);
		Camera camera = Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("SceneryCamera"));
		if ((bool)camera)
		{
			camera.eventMask = 1 << LayerMask.NameToLayer("Scenery");
		}
	}
}
