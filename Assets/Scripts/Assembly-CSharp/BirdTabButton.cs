using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BirdTabButton : MonoBehaviour
{
	[SerializeField]
	public UIInputTrigger m_ButtonTrigger;

	[SerializeField]
	public GameObject m_NewMarker;

	[SerializeField]
	private GameObject m_Activated;

	[SerializeField]
	private GameObject m_Deactivated;

	[SerializeField]
	public string m_BirdName;

	[SerializeField]
	public GameObject m_BirdShadowObject;

	private bool m_nonClickable;

	[method: MethodImpl(32)]
	public event Action<string> OnButtonClicked;

	private void Awake()
	{
		RegisterEventHandler();
	}

	public void SetInactive()
	{
		m_Activated.SetActive(false);
		m_Deactivated.SetActive(false);
		GetComponent<BoxCollider>().enabled = false;
		m_nonClickable = true;
	}

	public void Activate(bool activated)
	{
		if (!m_nonClickable)
		{
			m_Activated.SetActive(activated);
			m_Deactivated.SetActive(!activated);
		}
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if ((bool)m_ButtonTrigger)
		{
			m_ButtonTrigger.Clicked += RegisterButtonClicked;
		}
	}

	private void DeRegisterEventHandler()
	{
		if ((bool)m_ButtonTrigger)
		{
			m_ButtonTrigger.Clicked -= RegisterButtonClicked;
		}
	}

	private void RegisterButtonClicked()
	{
		if (this.OnButtonClicked != null)
		{
			this.OnButtonClicked(m_BirdName);
		}
	}
}
