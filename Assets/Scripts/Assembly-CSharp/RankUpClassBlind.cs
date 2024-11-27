using ABH.GameDatas;
using UnityEngine;

public class RankUpClassBlind : MonoBehaviour
{
	[SerializeField]
	private Transform m_ClassRoot;

	[SerializeField]
	private UILabel m_FromLevel;

	[SerializeField]
	private UILabel m_ToLevel;

	[SerializeField]
	private UISprite m_AssociatedBirdSprite;

	[SerializeField]
	private CHMotionTween m_MotionTween;

	private GameObject m_ClassAsset;

	private ClassItemGameData m_Model;

	public void SetModel(ClassItemGameData classItem, int oldLevel)
	{
		if (classItem == null)
		{
			Object.Destroy(base.gameObject);
		}
		m_Model = classItem;
		m_FromLevel.text = oldLevel.ToString("0");
		m_ToLevel.text = classItem.ItemData.Level.ToString("0");
		if ((bool)m_AssociatedBirdSprite)
		{
			m_AssociatedBirdSprite.spriteName = ClassItemGameData.GetRestrictedBirdIcon(classItem.BalancingData);
		}
		string nameId = m_Model.ItemAssetName;
		foreach (BirdGameData allBird in DIContainerInfrastructure.GetCurrentPlayer().AllBirds)
		{
			if (allBird.ClassItem.BalancingData.NameId == classItem.BalancingData.NameId && allBird.ClassSkin != null && allBird.ClassSkin.BalancingData.SortPriority > 0)
			{
				nameId = allBird.ClassSkin.BalancingData.AssetBaseId;
				break;
			}
		}
		m_ClassAsset = DIContainerInfrastructure.GetClassAssetProvider().InstantiateObject(nameId, m_ClassRoot, Vector3.zero, Quaternion.identity, false);
		m_ClassAsset.transform.localScale = Vector3.one;
		UnityHelper.SetLayerRecusively(m_ClassAsset, LayerMask.NameToLayer("InterfaceCharacter"));
	}

	private void OnDestroy()
	{
		if (!(m_ClassAsset == null) && m_Model != null && (bool)DIContainerInfrastructure.GetClassAssetProvider())
		{
			DIContainerInfrastructure.GetClassAssetProvider().DestroyObject(m_Model.ItemBalancing.AssetBaseId, m_ClassAsset);
		}
	}
}
