using ABH.GameDatas;
using ABH.Shared.BalancingData;
using UnityEngine;

public class ClassRecommendation : MonoBehaviour
{
	[SerializeField]
	private GameObject m_checkoutObject;

	[SerializeField]
	private UIInputTrigger m_buyClassTrigger;

	[SerializeField]
	private Transform m_headGearParent;

	private BattleHintPopup m_parentPopup;

	public void Init(ClassItemBalancingData classBalancing, BattleHintPopup parentPopup)
	{
		m_parentPopup = parentPopup;
		string text = classBalancing.AssetBaseId;
		string replacementName = string.Empty;
		if (ClassItemGameData.CheckForReplacement(text.Replace("Headgear", "Class").ToLower(), out replacementName))
		{
			text = replacementName;
		}
		GameObject gameObject = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(text, m_headGearParent, Vector3.zero, Quaternion.identity, false);
		gameObject.transform.localScale = Vector3.one;
		bool active = classBalancing.IsPremium && !DIContainerLogic.InventoryService.CheckForItem(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, classBalancing.NameId);
		m_checkoutObject.SetActive(active);
		m_buyClassTrigger.Clicked -= GoToShop;
		m_buyClassTrigger.Clicked += GoToShop;
	}

	private void GoToShop()
	{
		m_parentPopup.ClosePopup();
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop("shop_global_classes", delegate
		{
		});
	}

	private void OnDestroy()
	{
		m_buyClassTrigger.Clicked -= GoToShop;
	}
}
