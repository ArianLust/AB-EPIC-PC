using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class BattleHintPopup : MonoBehaviour
{
	private struct ClassRecommendationHelper
	{
		public ClassItemBalancingData m_Balancing;

		public int m_Prio;

		public bool m_Available;
	}

	[SerializeField]
	private Animation m_popupAnim;

	[SerializeField]
	private UILabel m_headerLabel;

	[SerializeField]
	private UILabel m_enemyDescriptionLabel;

	[SerializeField]
	private UILabel m_hintLabel;

	[SerializeField]
	private UIInputTrigger m_closeButton;

	[SerializeField]
	private UISprite m_upperIcon;

	[SerializeField]
	private UISprite m_lowerIcon;

	[SerializeField]
	private ClassRecommendation m_recommendClassPrefab;

	[SerializeField]
	private Transform m_classGrid;

	private string m_categoryText;

	private BattleHintBalancingData m_balancing;

	public void Enter(BattleHintBalancingData balancing)
	{
		if (balancing != null)
		{
			m_balancing = balancing;
		}
		else
		{
			m_balancing = DIContainerBalancing.Service.GetBalancingData<BattleHintBalancingData>("Generic");
		}
		base.gameObject.SetActive(true);
		SetupClassGrid();
		m_headerLabel.text = DIContainerInfrastructure.GetLocaService().Tr(m_balancing.LocaId + "_title");
		m_enemyDescriptionLabel.text = DIContainerInfrastructure.GetLocaService().Tr(m_balancing.LocaId + "_desc1");
		m_hintLabel.text = DIContainerInfrastructure.GetLocaService().Tr(m_balancing.LocaId + "_desc2");
		SetupIcons(m_upperIcon, m_balancing.TopAtlasId, m_balancing.TopIconId);
		SetupIcons(m_lowerIcon, m_balancing.BottomAtlasId, m_balancing.BottomIconId);
		StartCoroutine(EnterCoroutine());
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("EnterBattleHint");
		m_popupAnim.Play("Popup_Enter");
		yield return new WaitForSeconds(m_popupAnim["Popup_Enter"].length);
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("EnterBattleHint");
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(8, ClosePopup);
		m_closeButton.Clicked -= ClosePopup;
		m_closeButton.Clicked += ClosePopup;
	}

	private void SetupClassGrid()
	{
		if (m_balancing.RecommendedClasses == null)
		{
			return;
		}
		Dictionary<string, ClassRecommendationHelper> classRecommendationHelpers = GetClassRecommendationHelpers();
		foreach (KeyValuePair<string, ClassRecommendationHelper> item in classRecommendationHelpers)
		{
			ClassItemBalancingData balancing = item.Value.m_Balancing;
			ClassRecommendation classRecommendation = Object.Instantiate(m_recommendClassPrefab);
			classRecommendation.Init(balancing, this);
			classRecommendation.transform.parent = m_classGrid;
			classRecommendation.transform.localPosition = new Vector3(classRecommendation.transform.localPosition.x, classRecommendation.transform.localPosition.y, 0f);
			classRecommendation.transform.localScale = Vector3.one;
		}
		m_classGrid.GetComponent<UIGrid>().Reposition();
	}

	private Dictionary<string, ClassRecommendationHelper> GetClassRecommendationHelpers()
	{
		Dictionary<string, ClassRecommendationHelper> dictionary = new Dictionary<string, ClassRecommendationHelper>();
		foreach (KeyValuePair<string, int> recommendedClass in m_balancing.RecommendedClasses)
		{
			ClassItemBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<ClassItemBalancingData>(recommendedClass.Key);
			PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
			if (currentPlayer.GetBird(balancingData.RestrictedBirdId) == null)
			{
				continue;
			}
			ClassRecommendationHelper value = default(ClassRecommendationHelper);
			value.m_Available = DIContainerLogic.InventoryService.CheckForItem(currentPlayer.InventoryGameData, balancingData.NameId) || balancingData.IsPremium;
			value.m_Prio = recommendedClass.Value;
			value.m_Balancing = balancingData;
			if (!dictionary.ContainsKey(balancingData.RestrictedBirdId))
			{
				dictionary.Add(balancingData.RestrictedBirdId, value);
				continue;
			}
			ClassRecommendationHelper classRecommendationHelper = dictionary[balancingData.RestrictedBirdId];
			if (!classRecommendationHelper.m_Available && ((!classRecommendationHelper.m_Available && value.m_Available) || value.m_Prio < classRecommendationHelper.m_Prio))
			{
				dictionary[balancingData.RestrictedBirdId] = value;
			}
		}
		return dictionary;
	}

	private void SetupIcons(UISprite sprite, string atlasId, string spriteName)
	{
		if (DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().ContainsAsset(atlasId))
		{
			GameObject gameObject = DIContainerInfrastructure.GetGenericIconAtlasAssetProvider().GetObject(atlasId) as GameObject;
			sprite.atlas = gameObject.GetComponent<UIAtlas>();
		}
		else if (DIContainerInfrastructure.GetShopIconAtlasAssetProvider().ContainsAsset(atlasId))
		{
			GameObject gameObject2 = DIContainerInfrastructure.GetShopIconAtlasAssetProvider().GetObject(atlasId) as GameObject;
			sprite.atlas = gameObject2.GetComponent<UIAtlas>();
		}
		sprite.spriteName = spriteName;
		sprite.MakePixelPerfect();
	}

	public void ClosePopup()
	{
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("EnterBattleHint");
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(8);
		m_closeButton.Clicked -= ClosePopup;
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		m_popupAnim.Play("Popup_Leave");
		yield return new WaitForSeconds(m_popupAnim["Popup_Leave"].length);
		foreach (Transform child in m_classGrid)
		{
			Object.Destroy(child.gameObject);
		}
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("EnterBattleHint");
		base.gameObject.SetActive(false);
	}
}
