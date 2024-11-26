using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class BuyClassItemInfo : MonoBehaviour
{
	[SerializeField]
	private UISprite SupportSkillSprite;

	[SerializeField]
	private UISprite OffensiveSkillSprite;

	[SerializeField]
	private UILabel SupportSkillName;

	[SerializeField]
	private UILabel OffensiveSkillName;

	[SerializeField]
	private UISprite OffensiveSkillTargetSprite;

	[SerializeField]
	private UISprite SupportSkillTargetSprite;

	[SerializeField]
	private BirdGameData m_SelectedBird;

	[SerializeField]
	private ClassItemGameData m_SelectedClass;

	[SerializeField]
	private Transform m_ClassNameLowerPosition;

	[SerializeField]
	private UILabel m_ClassName;

	[SerializeField]
	[Header("Buy Icon")]
	private ResourceCostBlind m_CostBlind;

	[SerializeField]
	private ResourceCostBlind m_DiscountCostBlind;

	[SerializeField]
	private UIInputTrigger m_BuyClassButton;

	[SerializeField]
	private GameObject m_BuyDiscountObject;

	[SerializeField]
	private GameObject m_BuyNormalObject;

	[SerializeField]
	private UILabel m_DiscountOldPrice;

	[SerializeField]
	private GameObject m_TimerObject;

	[SerializeField]
	private UILabel m_TimerLabel;

	[SerializeField]
	private GameObject m_BuyIndicatorPrefab;

	private bool HasInitialized;

	private bool HasStarted;

	public BirdWindowUI m_BirdUI;

	public ClassManagerUi m_ClassMgr;

	private BuyableShopOfferBalancingData m_premiumClassOffer;

	private void Awake()
	{
		m_BuyClassButton.Clicked -= BuyClass;
		m_BuyClassButton.Clicked += BuyClass;
	}

	private void OnDestroy()
	{
		m_BuyClassButton.Clicked -= BuyClass;
	}

	public void ShowAttackSkillTooltip()
	{
		BirdGameData birdGameData = new BirdGameData(m_SelectedBird.Data);
		birdGameData.OverrideClassItem = m_SelectedClass;
		BirdCombatant birdCombatant = new BirdCombatant(birdGameData).SetPvPBird(DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowSkillOverlay(OffensiveSkillSprite.cachedTransform, birdCombatant.CharacterModel, m_SelectedClass.PrimarySkill, true);
	}

	public void ShowSupportSkillTooltip()
	{
		BirdGameData birdGameData = new BirdGameData(m_SelectedBird.Data);
		birdGameData.OverrideClassItem = m_SelectedClass;
		BirdCombatant birdCombatant = new BirdCombatant(birdGameData).SetPvPBird(DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP);
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowSkillOverlay(SupportSkillSprite.cachedTransform, birdCombatant.CharacterModel, m_SelectedClass.SecondarySkill, true);
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

	private void BuyClass()
	{
		List<Requirement> failed;
		if (!DIContainerLogic.GetShopService().IsOfferBuyable(DIContainerInfrastructure.GetCurrentPlayer(), m_premiumClassOffer, out failed))
		{
			Requirement requirement = failed.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem);
			if (requirement == null || requirement.RequirementType != RequirementType.PayItem)
			{
				return;
			}
			IInventoryItemGameData data = null;
			if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, requirement.NameId, out data))
			{
				if (m_ClassMgr != null)
				{
					m_ClassMgr.Leave(false);
				}
				CoinBarController controllerForResourceBar = DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.GetControllerForResourceBar(data.ItemBalancing.NameId);
				controllerForResourceBar.SetReEnterAction(m_ClassMgr.ReEnterFromShop);
				controllerForResourceBar.SwitchToShop();
			}
		}
		else
		{
			StartCoroutine(BuyClassCoroutine());
		}
	}

	private IEnumerator BuyClassCoroutine()
	{
		m_BuyClassButton.Clicked -= BuyClass;
		DIContainerLogic.GetShopService().BuyShopOffer(DIContainerInfrastructure.GetCurrentPlayer(), m_premiumClassOffer);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateFriendshipEssenceBar();
		yield return new WaitForSeconds(ShowBoughtIndicator());
		if (m_BirdUI != null)
		{
			m_BirdUI.RefreshAll();
		}
		else if (m_ClassMgr != null)
		{
			m_ClassMgr.RefreshAll();
		}
		m_BuyClassButton.Clicked += BuyClass;
	}

	private float ShowBoughtIndicator()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_BuyIndicatorPrefab);
		if (gameObject != null)
		{
			UnityHelper.SetLayerRecusively(gameObject, base.gameObject.layer);
			gameObject.transform.position = m_BuyClassButton.transform.position + new Vector3(0f, 0f, -20f);
			UnityEngine.Object.Destroy(gameObject, gameObject.GetComponent<Animation>().clip.length);
			return gameObject.GetComponent<Animation>().clip.length;
		}
		return 0f;
	}

	public void SetModel(ClassItemGameData classItemGameData, BirdGameData selectedBird)
	{
		if (classItemGameData == null)
		{
			DebugLog.Error(GetType(), "Given class item game data is null!");
			return;
		}
		UIAtlas uIAtlas = null;
		IList<BuyableShopOfferBalancingData> balancingDataList = DIContainerBalancing.Service.GetBalancingDataList<BuyableShopOfferBalancingData>();
		m_premiumClassOffer = balancingDataList.FirstOrDefault((BuyableShopOfferBalancingData offer) => offer.OfferContents != null && offer.OfferContents.Count == 1 && offer.OfferContents.ContainsKey(classItemGameData.BalancingData.NameId) && offer.Category == "shop_global_classes");
		if (m_premiumClassOffer == null)
		{
			DebugLog.Error(GetType(), "Premium class offer is null!");
			return;
		}
		SetupShopButton();
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
		m_ClassName.text = classItemGameData.ItemLocalizedName;
	}

	private IEnumerator TimerRoutine()
	{
		DateTime trustedTime;
		while (!DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
		{
			yield return new WaitForSeconds(1f);
		}
		uint remainingSeconds = (uint)DIContainerLogic.GetSalesManagerService().GetRemainingSaleDuration(m_premiumClassOffer);
		DateTime dateTimeFromTimestamp = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(DIContainerLogic.GetTimingService().GetCurrentTimestamp() + remainingSeconds);
		if (DIContainerLogic.GetDeviceTimingService().IsAfter(dateTimeFromTimestamp))
		{
			yield break;
		}
		while (trustedTime < dateTimeFromTimestamp)
		{
			if (DIContainerLogic.GetTimingService().TryGetTrustedTime(out trustedTime))
			{
				TimeSpan timeLeft = dateTimeFromTimestamp - trustedTime;
				m_TimerLabel.text = DIContainerInfrastructure.GetFormatProvider().GetDurationFormatStandard(timeLeft);
			}
			yield return new WaitForSeconds(1f);
		}
		SetupShopButton();
	}

	private void SetupShopButton()
	{
		Requirement requirement = m_premiumClassOffer.BuyRequirements.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem);
		float value = requirement.Value;
		if (DIContainerLogic.GetShopService().IsDiscountValid(m_premiumClassOffer) && DIContainerLogic.GetShopService().IsPriceDiscount(m_premiumClassOffer))
		{
			m_BuyDiscountObject.SetActive(true);
			m_BuyNormalObject.SetActive(false);
			m_TimerObject.SetActive(true);
			StartCoroutine(TimerRoutine());
			m_DiscountCostBlind.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(requirement.NameId).AssetBaseId, null, m_premiumClassOffer.DiscountPrice, string.Empty);
			m_DiscountOldPrice.text = value.ToString();
		}
		else
		{
			m_BuyDiscountObject.SetActive(false);
			m_BuyNormalObject.SetActive(true);
			m_TimerObject.SetActive(false);
			m_CostBlind.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(requirement.NameId).AssetBaseId, null, value, string.Empty);
		}
	}
}
