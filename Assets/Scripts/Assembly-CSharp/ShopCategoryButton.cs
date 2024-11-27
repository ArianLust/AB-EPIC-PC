using UnityEngine;

public class ShopCategoryButton : MonoBehaviour
{
	[SerializeField]
	public string m_CategoryName;

	[SerializeField]
	public UIInputTrigger m_ButtonTrigger;

	[SerializeField]
	public GameObject m_SaleMarker;

	[SerializeField]
	public GameObject m_UpdateMarker;

	private void Start()
	{
		RegisterEventHandlers();
	}

	private void OpenCategoryClicked()
	{
		DIContainerInfrastructure.GetCoreStateMgr().ShowShop(m_CategoryName, null);
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		m_ButtonTrigger.Clicked += OpenCategoryClicked;
	}

	private void DeRegisterEventHandlers()
	{
		m_ButtonTrigger.Clicked -= OpenCategoryClicked;
	}

	private void OnDestroy()
	{
		DeRegisterEventHandlers();
	}
}
