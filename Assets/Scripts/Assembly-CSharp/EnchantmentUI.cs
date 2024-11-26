using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Interfaces;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class EnchantmentUI : MonoBehaviour
{
	[SerializeField]
	private UIInputTrigger m_infoButton;

	[SerializeField]
	private UIInputTrigger m_closeInfoScreenButton;

	[SerializeField]
	private UIInputTrigger m_closeButton;

	[SerializeField]
	private UIInputTrigger m_enchantButton;

	[SerializeField]
	private UISprite m_enchantButtonSprite;

	[SerializeField]
	private GameObject m_enchantingCostSingleObject;

	[SerializeField]
	private GameObject m_enchantingCostDoubleObject;

	[SerializeField]
	private ResourceCostBlind m_enchantingCostSingle;

	[SerializeField]
	private ResourceCostBlind m_enchantingCostDoubleA;

	[SerializeField]
	private ResourceCostBlind m_enchantingCostDoubleB;

	[SerializeField]
	private InventoryItemSlot m_currentItemSlot;

	[SerializeField]
	private UISprite m_progressBar;

	[SerializeField]
	private UISprite m_progressBarPreview;

	[SerializeField]
	private UILabel m_progressLabel;

	[SerializeField]
	private UIGrid m_resourceGrid;

	[SerializeField]
	private GameObject m_ListElementResourcePrefab;

	[SerializeField]
	private GameObject m_enchantFooterObject;

	[SerializeField]
	private GameObject m_forgeFooterObject;

	[SerializeField]
	private UIInputTrigger m_forgeShopLink;

	[SerializeField]
	private Animation m_InfoScreenAnimation;

	[SerializeField]
	private Animation m_headerAnim;

	[SerializeField]
	private Animation m_displayAnim;

	[SerializeField]
	private Animation m_categoriesAnim;

	[SerializeField]
	private CoinBarController m_ShardController;

	[SerializeField]
	private CoinBarController m_SnoutlingController;

	[SerializeField]
	private UIInputTrigger m_openSkipButton;

	[SerializeField]
	private EnchantingItemSlot m_skipSelectButton;

	[SerializeField]
	private GameObject m_skipFooterObject;

	[SerializeField]
	private Animation m_SkipPopup;

	[SerializeField]
	private UIInputTrigger m_skipPopupCloseButton;

	[SerializeField]
	private UIInputTrigger m_skipButton;

	[SerializeField]
	private UILabel m_skipDescriptionLabel;

	[SerializeField]
	private InventoryItemSlot m_skipCurrentItemSlot;

	[SerializeField]
	private InventoryItemSlot m_skipAfterEnchantingItemSlot;

	[SerializeField]
	private ResourceCostBlind m_skipCost;

	[SerializeField]
	private GameObject m_hundredPercentObject;

	private BirdWindowUI m_birdParent;

	private BannerWindowUI m_bannerParent;

	private EnchantingResultPopup m_resultPopup;

	private EnchantingBalancingData m_enchBalancing;

	private List<Requirement> m_enchantRequirements;

	private int m_maxResourceFromInventory;

	private int m_theoreticalEnchLevel;

	private int m_currentEnchLevel;

	private int m_maxEnchLevel;

	private int m_maxResourceForMaxLevel;

	private float m_hoveringResources;

	private float m_hoveringResourcesUsedInPreviousLevels;

	private float m_bonusFromSelectedResource;

	private Dictionary<string, int> m_selectedResources = new Dictionary<string, int>();

	private EquipmentGameData m_selectedBirdItem;

	private BannerItemGameData m_selectedBannerItem;

	private BaseItemSlot m_lastSelectedSlot;

	public IInventoryItemGameData m_currentSelectedResource;

	private int m_currentSelectedRecipeIndex;

	private Dictionary<string, List<IInventoryItemGameData>> m_recipes;

	private List<EnchantingItemSlot> m_currentItemSlots = new List<EnchantingItemSlot>();

	private bool m_isBannerEnchant;

	private List<Requirement> m_skipRequirements;

	private Dictionary<string, int> m_AllocatedResources;

	private float m_AllocatedResourceValue;

	private int m_MaxEnchantLevel;

	private float m_AnvilBonusValue;

	[SerializeField]
	private UILabel m_extraRanksLabel;

	[SerializeField]
	private GameObject m_anvilIconGold;

	[SerializeField]
	private GameObject m_anvilIconDiamond;

	[SerializeField]
	private UpgradeItemSlot m_anvilUpgradeSlot;

	[SerializeField]
	private GameObject m_backgroundLevelup;

	[SerializeField]
	private UISprite m_mainIcon;

	[SerializeField]
	private UILabel m_totalStatsLabel;

	[SerializeField]
	private UILabel m_improvedLabel;

	private bool m_enchantInProgress;

	public UIInputTrigger EnchantButton
	{
		get
		{
			return m_enchantButton;
		}
	}

	[method: MethodImpl(32)]
	public event Action OnSufficientResourcesAllocated;

	[method: MethodImpl(32)]
	public event Action<string> OnAllResourcesSpent;

	public void EnterBanner(BannerItemGameData item, BannerWindowUI parentWindow, EnchantingResultPopup resultPopup)
	{
		m_isBannerEnchant = true;
		m_selectedBannerItem = item;
		m_bannerParent = parentWindow;
		m_resultPopup = resultPopup;
		m_enchBalancing = DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBannerItem);
		m_currentEnchLevel = m_selectedBannerItem.EnchantementLevel;
		m_maxEnchLevel = DIContainerLogic.EnchantmentLogic.GetMaxEnchantmentLevel(item);
		m_theoreticalEnchLevel = m_selectedBannerItem.EnchantementLevel + 1;
		m_hoveringResources = GetRequiredResourcesWithBonus(m_enchBalancing) * m_selectedBannerItem.EnchantmentProgress;
		base.gameObject.SetActive(true);
		StartCoroutine(SetPreviewBannerItems(m_currentItemSlot, m_theoreticalEnchLevel));
		GenericEnter();
	}

	public void EnterBird(EquipmentGameData item, BirdWindowUI parentWindow, EnchantingResultPopup resultPopup)
	{
		m_isBannerEnchant = false;
		m_selectedBirdItem = item;
		m_birdParent = parentWindow;
		m_resultPopup = resultPopup;
		m_enchBalancing = DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBirdItem);
		m_currentEnchLevel = m_selectedBirdItem.EnchantementLevel;
		m_maxEnchLevel = DIContainerLogic.EnchantmentLogic.GetMaxEnchantmentLevel(item);
		m_theoreticalEnchLevel = m_selectedBirdItem.EnchantementLevel + 1;
		m_hoveringResources = GetRequiredResourcesWithBonus(m_enchBalancing) * m_selectedBirdItem.EnchantmentProgress;
		base.gameObject.SetActive(true);
		SetPreviewBirdItems(m_currentItemSlot, m_theoreticalEnchLevel);
		GenericEnter();
	}

	private void GenericEnter()
	{
		InventoryGameData inventoryGameData = DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData;
		m_ShardController.SetInventory(inventoryGameData).SetShopLink(true);
		m_SnoutlingController.SetInventory(inventoryGameData).SetShopLink(true);
		m_hoveringResourcesUsedInPreviousLevels = 0f;
		m_backgroundLevelup.SetActive(false);
		m_hundredPercentObject.SetActive(false);
		m_selectedResources.Clear();
		m_enchantRequirements = new List<Requirement>();
		SetCostBlind();
		SetForgeData();
		inventoryGameData.CraftingRecipes.TryGetValue(InventoryItemType.Resources, out m_recipes);
		StartCoroutine(InitResourceButtons());
		StartCoroutine(EnterCoroutine());
	}

	private void SetForgeData()
	{
		IInventoryItemGameData data = null;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, "forge_leveled", out data))
		{
			DebugLog.Log(GetType(), "Get leveled forge");
		}
		else
		{
			DebugLog.Log(GetType(), "No leveled item");
		}
		m_anvilUpgradeSlot.SetModel(data, false);
		if (m_anvilUpgradeSlot.gameObject.activeInHierarchy)
		{
			m_anvilUpgradeSlot.OnUsed -= UpgradeSelected;
			m_anvilUpgradeSlot.OnUsed += UpgradeSelected;
		}
		WorldBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<WorldBalancingData>("piggy_island");
		int level = data.ItemData.Level;
		m_AnvilBonusValue = 0f;
		if (level >= 2)
		{
			m_anvilIconGold.SetActive(true);
			m_AnvilBonusValue += balancingData.GoldenAnvilBonus;
		}
		if (level >= 3)
		{
			m_anvilIconDiamond.SetActive(true);
			m_AnvilBonusValue += balancingData.DiamondAnvilBonus;
		}
		DebugLog.Log(GetType(), "Anvil level found: " + level + ". Anvil-Bonus = " + m_AnvilBonusValue);
	}

	private IEnumerator EnterCoroutine()
	{
		m_enchantInProgress = false;
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("enchantmentUi_enter");
		m_enchantButtonSprite.spriteName = "Button_Square_Large_D";
		m_enchantButton.GetComponent<BoxCollider>().enabled = false;
		SetCurrentItemStats();
		m_headerAnim.Play("Header_Enter");
		m_displayAnim.Play("EnchantmentDisplay_Enter");
		m_categoriesAnim.Play("Categories_Enter");
		yield return new WaitForSeconds(0.5f);
		RegisterEventHandler();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enchantment_entered", string.Empty);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("enchantmentUi_enter");
	}

	public void Leave()
	{
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		m_headerAnim.Play("Header_Leave");
		m_displayAnim.Play("EnchantmentDisplay_Leave");
		m_categoriesAnim.Play("Categories_Leave");
		yield return new WaitForSeconds(0.2f);
		DeRegisterEventHandler();
		if (m_isBannerEnchant)
		{
			m_bannerParent.DeactivateEnchanting();
		}
		else
		{
			m_birdParent.DeactivateEnchanting();
		}
		base.gameObject.SetActive(false);
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(3, Leave);
		m_closeButton.Clicked += Leave;
		m_enchantButton.Clicked += EnchantButtonClicked;
		m_openSkipButton.Clicked += OpenSkipPopup;
		m_skipSelectButton.OnUsed += SelectSkip;
		m_infoButton.Clicked += OpenInfoScreen;
		m_closeInfoScreenButton.Clicked += CloseInfoScreen;
		m_skipPopupCloseButton.Clicked += CloseSkipPopup;
		m_skipButton.Clicked += BuySkip;
		m_forgeShopLink.Clicked += GoToForgeShop;
		m_ShardController.RegisterEventHandlers();
		m_ShardController.SetReEnterAction(UpdateBank);
		m_SnoutlingController.RegisterEventHandlers();
		m_SnoutlingController.SetReEnterAction(UpdateBank);
		m_skipSelectButton.RegisterEventHandler();
	}

	private void DeRegisterEventHandler()
	{
		m_closeButton.Clicked -= Leave;
		m_enchantButton.Clicked -= EnchantButtonClicked;
		m_openSkipButton.Clicked -= OpenSkipPopup;
		m_skipSelectButton.OnUsed -= SelectSkip;
		m_infoButton.Clicked -= OpenInfoScreen;
		m_closeInfoScreenButton.Clicked -= CloseInfoScreen;
		m_skipPopupCloseButton.Clicked -= CloseSkipPopup;
		m_skipButton.Clicked -= BuySkip;
		m_forgeShopLink.Clicked -= GoToForgeShop;
		m_ShardController.DeRegisterEventHandlers();
		m_SnoutlingController.DeRegisterEventHandlers();
		m_skipSelectButton.DeRegisterEventHandler();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
	}

	private void SetPreviewBirdItems(InventoryItemSlot beforeSlot, int nextLevel)
	{
		beforeSlot.RemoveAssets();
		beforeSlot.SetModel(m_selectedBirdItem, m_birdParent.isPvp());
		EquipmentGameData equipmentGameData = new EquipmentGameData(m_selectedBirdItem.Data.NameId);
		equipmentGameData.Data.Quality = m_selectedBirdItem.Data.Quality;
		equipmentGameData.Data.Value = m_selectedBirdItem.Data.Value;
		equipmentGameData.EnchantmentProgress = 0f;
		equipmentGameData.EnchantementLevel = nextLevel;
		equipmentGameData.Data.ItemSource = m_selectedBirdItem.Data.ItemSource;
		equipmentGameData.Data.Level = m_selectedBirdItem.Data.Level;
		equipmentGameData.Data.ScrapLoot = m_selectedBirdItem.Data.ScrapLoot;
		UnityHelper.SetLayerRecusively(beforeSlot.gameObject, LayerMask.NameToLayer("Interface"));
	}

	private IEnumerator SetPreviewBannerItems(InventoryItemSlot beforeSlot, int nextLevel)
	{
		beforeSlot.RemoveAssets();
		beforeSlot.SetModel(m_selectedBannerItem, true);
		BannerItemGameData enchantedItem = new BannerItemGameData(m_selectedBannerItem.Data.NameId);
		enchantedItem.Data.Quality = m_selectedBannerItem.Data.Quality;
		enchantedItem.Data.Stars = m_selectedBannerItem.Data.Stars;
		enchantedItem.Data.Value = m_selectedBannerItem.Data.Value;
		enchantedItem.EnchantmentProgress = 0f;
		enchantedItem.EnchantementLevel = nextLevel;
		enchantedItem.Data.ItemSource = m_selectedBannerItem.Data.ItemSource;
		enchantedItem.Data.Level = m_selectedBannerItem.Data.Level;
		yield return new WaitForEndOfFrame();
		beforeSlot.GetComponentInChildren<BannerPartAssetController>().ApplyLootTransformations();
		UnityHelper.SetLayerRecusively(beforeSlot.gameObject, LayerMask.NameToLayer("Interface"));
	}

	private void SetCostBlind()
	{
		Requirement requirement = new Requirement();
		requirement.RequirementType = RequirementType.PayItem;
		requirement.NameId = "shard";
		requirement.Value = 0f;
		Requirement requirement2 = new Requirement();
		requirement2.RequirementType = RequirementType.PayItem;
		requirement2.NameId = "gold";
		requirement2.Value = 0f;
		foreach (Requirement enchantRequirement in m_enchantRequirements)
		{
			if (enchantRequirement.NameId == "shard")
			{
				requirement.Value += enchantRequirement.Value;
			}
			if (enchantRequirement.NameId == "gold")
			{
				requirement2.Value += enchantRequirement.Value;
			}
		}
		IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(requirement.NameId);
		IInventoryItemBalancingData balancingData2 = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(requirement2.NameId);
		if (requirement.Value == 0f && requirement2.Value == 0f)
		{
			m_enchantingCostDoubleObject.SetActive(false);
			m_enchantingCostSingleObject.SetActive(false);
		}
		else if (requirement.Value == 0f)
		{
			m_enchantingCostDoubleObject.SetActive(false);
			m_enchantingCostSingleObject.SetActive(true);
			m_enchantingCostSingle.SetModel(balancingData2.AssetBaseId, null, requirement2.Value, string.Empty);
		}
		else if (requirement2.Value == 0f)
		{
			m_enchantingCostDoubleObject.SetActive(false);
			m_enchantingCostSingleObject.SetActive(true);
			m_enchantingCostSingle.SetModel(balancingData.AssetBaseId, null, requirement.Value, string.Empty);
		}
		else
		{
			m_enchantingCostDoubleObject.SetActive(true);
			m_enchantingCostSingleObject.SetActive(false);
			m_enchantingCostDoubleA.SetModel(balancingData.AssetBaseId, null, requirement.Value, string.Empty);
			m_enchantingCostDoubleB.SetModel(balancingData2.AssetBaseId, null, requirement2.Value, string.Empty);
		}
	}

	private void ResetProgressBar()
	{
		float num = 0f;
		num = ((!m_isBannerEnchant) ? m_selectedBirdItem.EnchantmentProgress : m_selectedBannerItem.EnchantmentProgress);
		if (num < 1f)
		{
			m_progressBar.fillAmount = num + m_AnvilBonusValue / 100f * (1f - num);
		}
		else
		{
			m_progressBar.fillAmount = num;
		}
		m_progressLabel.text = Mathf.FloorToInt(num * 100f).ToString("0") + "%";
		UpdateUiAfterResChange();
		m_progressBarPreview.fillAmount = 0f;
	}

	public void IncreaseResource(int stackSize)
	{
		if (m_selectedResources[m_currentSelectedResource.Name] >= m_maxResourceForMaxLevel || m_selectedResources[m_currentSelectedResource.Name] >= m_maxResourceFromInventory)
		{
			if (this.OnAllResourcesSpent != null)
			{
				this.OnAllResourcesSpent(m_currentSelectedResource.ItemBalancing.NameId);
			}
			return;
		}
		EnchantingItemSlot enchantingItemSlot = m_lastSelectedSlot as EnchantingItemSlot;
		if (!(enchantingItemSlot == null))
		{
			int val = enchantingItemSlot.GetMax() - m_selectedResources[m_currentSelectedResource.Name];
			int val2 = m_maxResourceForMaxLevel - m_selectedResources[m_currentSelectedResource.Name];
			int val3 = Math.Min(val, stackSize);
			val3 = Math.Min(val2, val3);
			m_hoveringResources += m_bonusFromSelectedResource * (float)val3;
			Dictionary<string, int> selectedResources;
			Dictionary<string, int> dictionary = (selectedResources = m_selectedResources);
			string key;
			string key2 = (key = m_currentSelectedResource.Name);
			int num = selectedResources[key];
			dictionary[key2] = num + val3;
			UpdateUiAfterResChange();
		}
	}

	public void DecreaseResource(int stackSize)
	{
		if (m_selectedResources[m_currentSelectedResource.Name] == 0)
		{
			UpdateUiAfterResChange();
			return;
		}
		int num = Math.Min(m_selectedResources[m_currentSelectedResource.Name], stackSize);
		m_hoveringResources -= m_bonusFromSelectedResource * (float)num;
		Dictionary<string, int> selectedResources;
		Dictionary<string, int> dictionary = (selectedResources = m_selectedResources);
		string key;
		string key2 = (key = m_currentSelectedResource.Name);
		int num2 = selectedResources[key];
		dictionary[key2] = num2 - num;
		if (m_hoveringResources < 0f)
		{
			m_hoveringResources = 0f;
		}
		UpdateUiAfterResChange();
	}

	private void UpdateUiAfterResChange()
	{
		int theoreticalEnchLevel = m_theoreticalEnchLevel;
		RecheckLevel();
		if (!(m_currentItemSlot == m_skipSelectButton) && !(m_lastSelectedSlot == m_skipSelectButton) && m_currentSelectedResource != null)
		{
			EnchantingItemSlot enchantingItemSlot = m_lastSelectedSlot as EnchantingItemSlot;
			if (enchantingItemSlot != null)
			{
				enchantingItemSlot.UpdateLabel(m_selectedResources[m_currentSelectedResource.Name], m_maxResourceFromInventory);
			}
			float num = 1f;
			if (m_enchBalancing.ResourceCosts > 0f && m_theoreticalEnchLevel != m_enchBalancing.EnchantmentLevel)
			{
				num = m_AnvilBonusValue / 100f + (m_hoveringResources - m_hoveringResourcesUsedInPreviousLevels) / m_enchBalancing.ResourceCosts;
				m_progressBarPreview.fillAmount = num;
			}
			else
			{
				m_progressBarPreview.fillAmount = 1f;
			}
			float num2 = num * 100f;
			float num3 = 100 * (m_theoreticalEnchLevel - 1 - m_currentEnchLevel);
			m_progressLabel.text = Mathf.FloorToInt(num2 + num3) + "%";
			if (IsAnyResourceSelected())
			{
				m_enchantButtonSprite.spriteName = "Button_Square_Large";
				m_enchantButton.GetComponent<BoxCollider>().enabled = true;
			}
			else
			{
				m_enchantButtonSprite.spriteName = "Button_Square_Large_D";
				m_enchantButton.GetComponent<BoxCollider>().enabled = false;
			}
		}
	}

	private void RecheckLevel()
	{
		bool flag = false;
		while (m_hoveringResources - m_hoveringResourcesUsedInPreviousLevels >= GetRequiredResourcesWithBonus(m_enchBalancing) && m_enchBalancing.ScrappingBonus > 0f)
		{
			flag = true;
			if ((m_isBannerEnchant && m_selectedBannerItem.IsSetItem) || (!m_isBannerEnchant && m_selectedBirdItem.IsSetItem))
			{
				for (int i = 0; i < m_enchBalancing.BuyRequirementsSet.Count; i++)
				{
					IncrementBuyRequirements(m_enchantRequirements, m_enchBalancing.BuyRequirementsSet[i]);
				}
			}
			else
			{
				for (int j = 0; j < m_enchBalancing.BuyRequirements.Count; j++)
				{
					IncrementBuyRequirements(m_enchantRequirements, m_enchBalancing.BuyRequirements[j]);
				}
			}
			m_backgroundLevelup.SetActive(true);
			m_hundredPercentObject.SetActive(true);
			m_hoveringResourcesUsedInPreviousLevels += GetRequiredResourcesWithBonus(m_enchBalancing);
			if (m_isBannerEnchant)
			{
				m_enchBalancing = DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBannerItem, m_theoreticalEnchLevel);
			}
			else
			{
				m_enchBalancing = DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBirdItem, m_theoreticalEnchLevel);
			}
			if (m_theoreticalEnchLevel < m_maxEnchLevel)
			{
				m_theoreticalEnchLevel++;
				m_progressBar.fillAmount = 0f;
			}
		}
		while (m_hoveringResources - m_hoveringResourcesUsedInPreviousLevels < 0f)
		{
			flag = true;
			if (m_enchBalancing.EnchantmentLevel != m_maxEnchLevel && m_theoreticalEnchLevel > 1)
			{
				m_theoreticalEnchLevel--;
			}
			if (m_isBannerEnchant)
			{
				m_enchBalancing = DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBannerItem, m_theoreticalEnchLevel - 1);
			}
			else
			{
				m_enchBalancing = DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBirdItem, m_theoreticalEnchLevel - 1);
			}
			m_hoveringResourcesUsedInPreviousLevels -= GetRequiredResourcesWithBonus(m_enchBalancing);
			if ((m_isBannerEnchant && m_selectedBannerItem.IsSetItem) || (!m_isBannerEnchant && m_selectedBirdItem.IsSetItem))
			{
				for (int k = 0; k < m_enchBalancing.BuyRequirementsSet.Count; k++)
				{
					DecrementBuyRequirements(m_enchantRequirements, m_enchBalancing.BuyRequirementsSet[k]);
				}
			}
			else
			{
				for (int l = 0; l < m_enchBalancing.BuyRequirements.Count; l++)
				{
					DecrementBuyRequirements(m_enchantRequirements, m_enchBalancing.BuyRequirements[l]);
				}
			}
			if (m_theoreticalEnchLevel == m_currentEnchLevel + 1)
			{
				m_backgroundLevelup.SetActive(false);
				m_hundredPercentObject.SetActive(false);
				ResetProgressBar();
			}
		}
		if (flag)
		{
			SetCurrentItemStats();
			SetCostBlind();
			if (this.OnSufficientResourcesAllocated != null)
			{
				this.OnSufficientResourcesAllocated();
			}
		}
	}

	private void SetCurrentItemStats()
	{
		int num = m_theoreticalEnchLevel - 1;
		if (m_theoreticalEnchLevel == m_maxEnchLevel && m_enchBalancing.EnchantmentLevel == m_theoreticalEnchLevel)
		{
			num++;
		}
		int num2 = num - m_currentEnchLevel;
		if (num2 == 0)
		{
			m_extraRanksLabel.gameObject.SetActive(false);
			m_improvedLabel.gameObject.SetActive(false);
		}
		else
		{
			m_extraRanksLabel.gameObject.SetActive(true);
			m_improvedLabel.gameObject.SetActive(true);
			if (num2 == 1)
			{
				m_extraRanksLabel.text = "+ " + num2 + " " + DIContainerInfrastructure.GetLocaService().Tr("enchantment_rank_01", "Ranks");
			}
			else
			{
				m_extraRanksLabel.text = "+ " + num2 + " " + DIContainerInfrastructure.GetLocaService().Tr("enchantment_rank_02", "Ranks");
			}
		}
		if (m_isBannerEnchant)
		{
			SetResultStats(m_selectedBannerItem, num);
		}
		else
		{
			SetResultStats(m_selectedBirdItem, num);
		}
	}

	private void IncrementBuyRequirements(List<Requirement> reqList, Requirement reqToAdd)
	{
		bool flag = false;
		for (int i = 0; i < reqList.Count; i++)
		{
			Requirement requirement = m_enchantRequirements[i];
			if (requirement.NameId == reqToAdd.NameId)
			{
				requirement.Value += reqToAdd.Value;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Requirement requirement2 = new Requirement();
			requirement2.NameId = reqToAdd.NameId;
			requirement2.Value = reqToAdd.Value;
			requirement2.RequirementType = reqToAdd.RequirementType;
			reqList.Add(requirement2);
		}
	}

	private void DecrementBuyRequirements(List<Requirement> reqList, Requirement reqToRemove)
	{
		for (int i = 0; i < reqList.Count; i++)
		{
			Requirement requirement = m_enchantRequirements[i];
			if (requirement.NameId == reqToRemove.NameId)
			{
				requirement.Value = Mathf.Max(requirement.Value - reqToRemove.Value, 0f);
				break;
			}
		}
	}

	private void ClearOldRecipeList()
	{
		foreach (Transform item in m_resourceGrid.transform)
		{
			if (!item.name.StartsWith("00001"))
			{
				item.GetComponent<EnchantingItemSlot>().OnUsed -= ResourceSelected;
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		m_currentItemSlots.Clear();
	}

	private IEnumerator InitResourceButtons()
	{
		ClearOldRecipeList();
		yield return new WaitForEndOfFrame();
		foreach (string str in m_recipes.Keys)
		{
			List<IInventoryItemGameData> itemList;
			m_recipes.TryGetValue(str, out itemList);
			GameObject go = UnityEngine.Object.Instantiate(m_ListElementResourcePrefab);
			go.transform.parent = m_resourceGrid.transform;
			go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, m_resourceGrid.transform.position.z);
			go.name = itemList[itemList.Count - 1].ItemBalancing.SortPriority.ToString("000") + "_" + go.name;
			EnchantingItemSlot slot2 = go.GetComponent<EnchantingItemSlot>();
			m_currentItemSlots.Add(slot2);
			bool isPvp = true;
			if (!m_isBannerEnchant)
			{
				isPvp = m_birdParent.isPvp();
			}
			slot2.SetModel(itemList[itemList.Count - 1], isPvp);
			slot2.m_UpdateIndikatorRoot.SetActive(false);
			slot2.OnUsed -= ResourceSelected;
			slot2.OnUsed += ResourceSelected;
			m_selectedResources.Add(itemList[itemList.Count - 1].Name, 0);
		}
		m_currentItemSlots = m_currentItemSlots.OrderBy((EnchantingItemSlot slot) => slot.GetModel().ItemBalancing.SortPriority).ToList();
		if (m_currentSelectedResource == null)
		{
			m_lastSelectedSlot = m_currentItemSlots.FirstOrDefault();
			m_currentSelectedResource = m_lastSelectedSlot.GetModel();
		}
		else if (m_lastSelectedSlot == null || !m_lastSelectedSlot.gameObject.activeSelf)
		{
			m_lastSelectedSlot = m_currentItemSlots.FirstOrDefault((EnchantingItemSlot s) => s.GetModel() == m_currentSelectedResource);
		}
		m_currentSelectedRecipeIndex = 0;
		m_resourceGrid.Reposition();
		SelectLastSlot();
		ResetProgressBar();
		if (m_currentSelectedResource != null)
		{
			SetResourceBalancing();
		}
	}

	private void EnchantButtonClicked()
	{
		Enchant(false);
	}

	private void Enchant(bool skipped)
	{
		if (m_enchantInProgress)
		{
			return;
		}
		DebugLog.Log(GetType(), "Enchant clicked!");
		m_enchantInProgress = true;
		int num = 0;
		if (!skipped)
		{
			if (!CheckBuyReq(m_enchantRequirements))
			{
				m_enchantInProgress = false;
				return;
			}
			DIContainerLogic.RequirementService.ExecuteRequirements(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_enchantRequirements, "Enchant item");
			DebugLog.Log(GetType(), "requirements Executed (shard etc removed!");
			num = ConsumeRessourcesForEnchanting();
			if (num == m_currentEnchLevel)
			{
				DebugLog.Log(GetType(), "no level up, just fused some ressources");
				ResetProgressBar();
				m_enchantInProgress = false;
				return;
			}
		}
		else if (m_isBannerEnchant)
		{
			num = m_selectedBannerItem.EnchantementLevel + 1;
			m_selectedBannerItem.EnchantmentProgress = 0f;
		}
		else
		{
			num = m_selectedBirdItem.EnchantementLevel + 1;
			m_selectedBirdItem.EnchantmentProgress = 0f;
		}
		bool flag = false;
		if (m_isBannerEnchant)
		{
			TrackEnchantingAnalyticsForBanner();
			int enchantementLevel = m_selectedBannerItem.EnchantementLevel;
			m_selectedBannerItem.EnchantementLevel = num;
			m_resultPopup.Enter(m_selectedBannerItem, enchantementLevel);
			flag = m_selectedBannerItem.IsMaxEnchanted();
			m_enchBalancing = DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBannerItem);
		}
		else
		{
			TrackEnchantingAnalyticsForBird();
			DebugLog.Log(GetType(), "Set new Enchantmentlevel for " + m_selectedBirdItem);
			int enchantementLevel2 = m_selectedBirdItem.EnchantementLevel;
			m_selectedBirdItem.EnchantementLevel = num;
			m_resultPopup.Enter(m_selectedBirdItem, enchantementLevel2);
			flag = m_selectedBirdItem.IsMaxEnchanted();
			m_enchBalancing = DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBirdItem);
		}
		m_hoveringResourcesUsedInPreviousLevels = 0f;
		DebugLog.Log(GetType(), "Saving player data");
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
		if (!flag)
		{
			m_currentEnchLevel = num;
			m_theoreticalEnchLevel = num + 1;
			SetResourceBalancing();
			m_enchantRequirements.Clear();
			SetCostBlind();
			m_ShardController.SetInventory(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData).SetShopLink(true);
			m_SnoutlingController.SetInventory(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData).SetShopLink(true);
			m_backgroundLevelup.SetActive(false);
			m_hundredPercentObject.SetActive(false);
			ResetProgressBar();
			m_extraRanksLabel.gameObject.SetActive(false);
			m_improvedLabel.gameObject.SetActive(false);
			if (m_isBannerEnchant)
			{
				StartCoroutine(SetPreviewBannerItems(m_currentItemSlot, m_theoreticalEnchLevel));
				SetResultStats(m_selectedBannerItem, m_currentEnchLevel);
			}
			else
			{
				SetPreviewBirdItems(m_currentItemSlot, m_theoreticalEnchLevel);
				SetResultStats(m_selectedBirdItem, m_currentEnchLevel);
			}
		}
		else
		{
			Leave();
		}
		m_enchantInProgress = false;
	}

	private bool CheckBuyReq(List<Requirement> buyRequirements)
	{
		List<Requirement> failedRequirements = new List<Requirement>();
		DIContainerLogic.RequirementService.CheckGenericRequirements(DIContainerInfrastructure.GetCurrentPlayer(), buyRequirements, out failedRequirements);
		Requirement requirement = failedRequirements.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem);
		if (requirement != null && requirement.RequirementType == RequirementType.PayItem)
		{
			if (requirement.NameId == "lucky_coin")
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[1].m_StatBar.SwitchToShop();
			}
			else if (requirement.NameId == "gold")
			{
				m_SnoutlingController.SwitchToShop();
			}
			else if (requirement.NameId == "friendship_essence")
			{
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_PlayerStatsController[2].m_StatBar.SwitchToShop();
			}
			else if (requirement.NameId == "shard")
			{
				m_ShardController.SwitchToShop();
			}
			return false;
		}
		return true;
	}

	private int ConsumeRessourcesForEnchanting()
	{
		if (m_hoveringResources == 0f)
		{
			return 0;
		}
		foreach (EnchantingItemSlot currentItemSlot in m_currentItemSlots)
		{
			string text = currentItemSlot.GetModel().Name;
			DIContainerLogic.InventoryService.RemoveItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, text, m_selectedResources[text], "Add to enchanting");
			m_selectedResources[text] = 0;
		}
		foreach (EnchantingItemSlot currentItemSlot2 in m_currentItemSlots)
		{
			currentItemSlot2.ResetLabel();
		}
		EnchantingItemSlot enchantingItemSlot = m_lastSelectedSlot as EnchantingItemSlot;
		if (enchantingItemSlot != null)
		{
			m_maxResourceFromInventory = enchantingItemSlot.GetMax();
			enchantingItemSlot.UpdateLabel(0, m_maxResourceFromInventory);
		}
		int num = m_currentEnchLevel;
		EnchantingBalancingData enchantingBalancingData = null;
		enchantingBalancingData = ((!m_isBannerEnchant) ? DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBirdItem, num) : DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBannerItem, num));
		if (enchantingBalancingData == null)
		{
			Debug.LogError("couldn't find enchanting balancing!  " + num);
			return 0;
		}
		float requiredResourcesWithBonus = GetRequiredResourcesWithBonus(enchantingBalancingData);
		while (m_hoveringResources >= requiredResourcesWithBonus)
		{
			m_hoveringResources -= requiredResourcesWithBonus;
			num++;
			if (num > m_maxEnchLevel)
			{
				num--;
				break;
			}
			enchantingBalancingData = ((!m_isBannerEnchant) ? DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBirdItem, num) : DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBannerItem, num));
			if (enchantingBalancingData == null)
			{
				Debug.LogError("couldn't find enchanting balancing!  " + num);
				return 0;
			}
			requiredResourcesWithBonus = GetRequiredResourcesWithBonus(enchantingBalancingData);
		}
		if (m_isBannerEnchant)
		{
			m_selectedBannerItem.EnchantmentProgress = m_hoveringResources / requiredResourcesWithBonus;
		}
		else
		{
			m_selectedBirdItem.EnchantmentProgress = m_hoveringResources / requiredResourcesWithBonus;
		}
		return num;
	}

	private void SelectSkip(InventoryItemSlot slot)
	{
		m_enchantFooterObject.SetActive(false);
		m_skipFooterObject.SetActive(true);
		m_forgeFooterObject.SetActive(false);
		DeselectLast(m_currentSelectedResource == null || m_selectedResources[m_currentSelectedResource.Name] > 0);
		m_lastSelectedSlot = slot;
		slot.Select();
	}

	public void ResourceSelected(InventoryItemSlot slot)
	{
		m_enchantFooterObject.SetActive(true);
		m_skipFooterObject.SetActive(false);
		m_forgeFooterObject.SetActive(false);
		DeselectLast(m_currentSelectedResource == null || m_selectedResources[m_currentSelectedResource.Name] > 0);
		m_lastSelectedSlot = slot;
		List<IInventoryItemGameData> value = new List<IInventoryItemGameData>();
		foreach (string key in ((CraftingRecipeGameData)slot.GetModel()).GetResultLoot().Keys)
		{
			m_recipes.TryGetValue(key, out value);
		}
		m_currentSelectedResource = value.FirstOrDefault();
		SetResourceBalancing();
		SelectLastSlot();
		DIContainerInfrastructure.TutorialMgr.ShowTutorialGuideIfNecessary("enchantment_resource_clicked", string.Empty);
	}

	private void DeselectLast(bool hadRes)
	{
		if (!(m_lastSelectedSlot != null))
		{
			return;
		}
		m_lastSelectedSlot.Deselect();
		EnchantingItemSlot enchantingItemSlot = m_lastSelectedSlot as EnchantingItemSlot;
		if (enchantingItemSlot != null)
		{
			if (!hadRes)
			{
				enchantingItemSlot.ResetLabel();
			}
			enchantingItemSlot.DisableEnchanting();
		}
	}

	private void SelectLastSlot()
	{
		m_lastSelectedSlot.Select();
		EnchantingItemSlot enchantingItemSlot = m_lastSelectedSlot as EnchantingItemSlot;
		if (enchantingItemSlot != null)
		{
			m_maxResourceFromInventory = enchantingItemSlot.GetMax();
			if (m_currentSelectedResource != null)
			{
				enchantingItemSlot.EnableEnchanting(this);
				enchantingItemSlot.UpdateLabel(m_selectedResources[m_currentSelectedResource.Name], m_maxResourceFromInventory);
			}
		}
	}

	private void SetResourceBalancing()
	{
		CraftingItemBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<CraftingItemBalancingData>(((CraftingRecipeBalancingData)m_currentSelectedResource.ItemBalancing).AssetBaseId);
		int num = 1;
		if (balancingData.ValueOfBaseItem == 2)
		{
			num = 2;
		}
		else if (balancingData.ValueOfBaseItem == 4)
		{
			num = 3;
		}
		switch (num)
		{
		case 1:
			m_bonusFromSelectedResource = m_enchBalancing.Lvl1ResPoints;
			break;
		case 2:
			m_bonusFromSelectedResource = m_enchBalancing.Lvl2ResPoints;
			break;
		case 3:
			m_bonusFromSelectedResource = m_enchBalancing.Lvl3ResPoints;
			break;
		default:
			m_bonusFromSelectedResource = 1f;
			Debug.LogError("Error: item level could not be defined!");
			break;
		}
		float num2 = 0f;
		for (int i = m_currentEnchLevel; i < m_maxEnchLevel; i++)
		{
			num2 = ((!m_isBannerEnchant) ? (num2 + GetRequiredResourcesWithBonus(DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBirdItem, i))) : (num2 + GetRequiredResourcesWithBonus(DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBannerItem, i))));
		}
		num2 -= m_hoveringResources;
		m_maxResourceForMaxLevel = Mathf.CeilToInt(num2 / m_bonusFromSelectedResource) + m_selectedResources[m_currentSelectedResource.Name];
	}

	private void TrackEnchantingAnalyticsForBird()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.SaveAdd("ItemName", m_selectedBirdItem.ItemBalancing.NameId);
		dictionary.SaveAdd("TypeOfItem", m_selectedBirdItem.ItemBalancing.ItemType.ToString());
		dictionary.SaveAdd("NewEnchantmentLevel", m_selectedBirdItem.EnchantementLevel.ToString());
		dictionary.SaveAdd("ItemQuality", m_selectedBirdItem.ItemData.Quality.ToString());
		dictionary.SaveAdd("ItemLevel", m_selectedBirdItem.ItemData.Level.ToString());
		ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
		DIContainerInfrastructure.GetAnalyticsSystem(true).LogEventWithParameters("Enchanted", dictionary);
	}

	private void TrackEnchantingAnalyticsForBanner()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.SaveAdd("ItemName", m_selectedBannerItem.ItemBalancing.NameId);
		dictionary.SaveAdd("TypeOfItem", m_selectedBannerItem.ItemBalancing.ItemType.ToString());
		dictionary.SaveAdd("NewEnchantmentLevel", m_selectedBannerItem.EnchantementLevel.ToString());
		dictionary.SaveAdd("ItemQuality", m_selectedBannerItem.ItemData.Quality.ToString());
		dictionary.SaveAdd("ItemLevel", m_selectedBannerItem.ItemData.Level.ToString());
		ABHAnalyticsHelper.AddPlayerStatusToTracking(dictionary);
		DIContainerInfrastructure.GetAnalyticsSystem(true).LogEventWithParameters("Enchanted", dictionary);
	}

	private void OpenInfoScreen()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(4, CloseInfoScreen);
		m_InfoScreenAnimation.gameObject.SetActive(true);
		m_InfoScreenAnimation.Play("Popup_Enter");
	}

	private void CloseInfoScreen()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(4);
		m_InfoScreenAnimation.Play("Popup_Leave");
		Invoke("DeactiveInfoScreen", m_InfoScreenAnimation["Popup_Leave"].length);
	}

	private void DeactiveInfoScreen()
	{
		m_InfoScreenAnimation.gameObject.SetActive(false);
	}

	private void OpenSkipPopup()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(5, CloseSkipPopup);
		m_SkipPopup.Play("Popup_Enter");
		m_SkipPopup.gameObject.SetActive(true);
		m_skipRequirements = new List<Requirement>();
		float fillAmount = m_progressBar.fillAmount;
		EnchantingBalancingData enchantingBalancingData = null;
		enchantingBalancingData = ((!m_isBannerEnchant) ? DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBirdItem, m_currentEnchLevel) : DIContainerLogic.EnchantmentLogic.GetBalancing(m_selectedBannerItem, m_currentEnchLevel));
		if (enchantingBalancingData == null)
		{
			Debug.LogError("Couldn't find balancing " + m_currentEnchLevel);
			return;
		}
		Requirement requirement = enchantingBalancingData.SkipCostRequirement.FirstOrDefault();
		Requirement requirement2 = new Requirement();
		requirement2.Value = Mathf.Ceil(requirement.Value * (1f - fillAmount));
		requirement2.RequirementType = requirement.RequirementType;
		requirement2.NameId = requirement.NameId;
		m_skipRequirements.Add(requirement2);
		IInventoryItemBalancingData balancingData = DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(m_skipRequirements.FirstOrDefault().NameId);
		m_skipCost.SetModel(balancingData.AssetBaseId, null, m_skipRequirements.FirstOrDefault().Value, string.Empty);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (m_isBannerEnchant)
		{
			dictionary.Add("{value_1}", m_selectedBannerItem.ItemLocalizedName);
			dictionary.Add("{value_2}", m_selectedBannerItem.EnchantementLevel.ToString());
			dictionary.Add("{value_3}", (m_selectedBannerItem.EnchantementLevel + 1).ToString());
			StartCoroutine(SetPreviewBannerItems(m_skipCurrentItemSlot, m_currentEnchLevel + 1));
		}
		else
		{
			dictionary.Add("{value_1}", m_selectedBirdItem.ItemLocalizedName);
			dictionary.Add("{value_2}", m_selectedBirdItem.EnchantementLevel.ToString());
			dictionary.Add("{value_3}", (m_selectedBirdItem.EnchantementLevel + 1).ToString());
			SetPreviewBirdItems(m_skipCurrentItemSlot, m_currentEnchLevel + 1);
		}
		m_skipDescriptionLabel.text = DIContainerInfrastructure.GetLocaService().Tr("enchantment_skip_desc", dictionary);
	}

	private void BuySkip()
	{
		CloseSkipPopup();
		if (CheckBuyReq(m_skipRequirements))
		{
			DIContainerLogic.RequirementService.ExecuteRequirements(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_skipRequirements, "Enchant item");
			Enchant(true);
		}
	}

	private void CloseSkipPopup()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(5);
		m_SkipPopup.Play("Popup_Leave");
		Invoke("DeactivateSkipPopup", m_SkipPopup["Popup_Leave"].length);
	}

	private void DeactivateSkipPopup()
	{
		m_skipCurrentItemSlot.RemoveAssets();
		m_skipAfterEnchantingItemSlot.RemoveAssets();
		m_SkipPopup.gameObject.SetActive(false);
	}

	private void UpdateBank()
	{
		m_ShardController.UpdateValueOnly();
		m_SnoutlingController.UpdateValueOnly();
	}

	private void OnDestroy()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(3);
	}

	public bool IsAnyResourceSelected()
	{
		if (m_selectedResources != null && m_selectedResources.Count != 0)
		{
			foreach (KeyValuePair<string, int> selectedResource in m_selectedResources)
			{
				if (selectedResource.Value > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public int GetSelectedResource(string resourceName)
	{
		if (m_selectedResources.ContainsKey(resourceName))
		{
			return m_selectedResources[resourceName];
		}
		return 0;
	}

	private void PrintAllDebugInfo()
	{
		StringBuilder stringBuilder = new StringBuilder(string.Empty);
		stringBuilder.AppendLine("m_currentEnchLevel = " + m_currentEnchLevel);
		stringBuilder.AppendLine("m_theoreticalEnchLevel = " + m_theoreticalEnchLevel);
		stringBuilder.AppendLine("m_selectedResources[m_currentSelectedResource] = " + m_selectedResources[m_currentSelectedResource.Name]);
		stringBuilder.AppendLine("m_bonusFromSelectedResource" + m_bonusFromSelectedResource);
		stringBuilder.AppendLine("m_hoveringResourcesUsedInPreviousLevels = " + m_hoveringResourcesUsedInPreviousLevels);
		stringBuilder.AppendLine("m_hoveringResources = " + m_hoveringResources);
		stringBuilder.AppendLine("m_maxResourceForMaxLevel = " + m_maxResourceForMaxLevel);
		stringBuilder.AppendLine("m_maxResourceFromInventory = " + m_maxResourceFromInventory);
		stringBuilder.AppendLine("m_progressBar = " + m_progressBar.fillAmount);
		stringBuilder.AppendLine("m_progressBarPreview = " + m_progressBarPreview.fillAmount);
		DebugLog.Error(stringBuilder);
	}

	public void TutorialHelperToggleDragging(bool dragEnabled = true)
	{
		UIScrollView componentInChildren = base.gameObject.GetComponentInChildren<UIScrollView>();
		if (componentInChildren == null)
		{
			DebugLog.Error(GetType(), "TutorialHelperToggleDragging: DragPanel not found!");
		}
		else
		{
			componentInChildren.enabled = dragEnabled;
		}
	}

	public void UpgradeSelected(UpgradeItemSlot slot)
	{
		DeselectLast(m_currentSelectedResource == null || m_selectedResources[m_currentSelectedResource.Name] > 0);
		slot.Select();
		m_lastSelectedSlot = slot;
		m_enchantFooterObject.SetActive(false);
		m_skipFooterObject.SetActive(false);
		m_forgeFooterObject.SetActive(true);
	}

	private void GoToForgeShop()
	{
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop("shop_global_specials", GenericEnter);
	}

	private void SetResultStats(EquipmentGameData originalItem, int newLevel)
	{
		if (originalItem.BalancingData.ItemType == InventoryItemType.MainHandEquipment)
		{
			m_mainIcon.spriteName = "Character_Damage_Large";
		}
		else
		{
			m_mainIcon.spriteName = "Character_Health_Large";
		}
		m_totalStatsLabel.text = originalItem.ItemMainStat.ToString("0");
		m_improvedLabel.text = "+ " + (originalItem.GetItemMainStatWithEnchantmentLevel(newLevel) - originalItem.ItemMainStat).ToString("0");
	}

	private void SetResultStats(BannerItemGameData originalItem, int newLevel)
	{
		m_mainIcon.spriteName = "Character_Health_Large";
		m_totalStatsLabel.text = originalItem.ItemMainStat.ToString("0");
		m_improvedLabel.text = "+ " + (originalItem.GetItemMainStatWithEnchantmentLevel(newLevel) - originalItem.ItemMainStat).ToString("0");
	}

	private float GetRequiredResourcesWithBonus(EnchantingBalancingData balancing)
	{
		float num = m_AnvilBonusValue / 100f * balancing.ResourceCosts;
		return balancing.ResourceCosts - num;
	}
}
