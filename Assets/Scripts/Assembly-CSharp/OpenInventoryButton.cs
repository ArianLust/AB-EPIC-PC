using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.Shared.Generic;
using UnityEngine;

public class OpenInventoryButton : MonoBehaviour
{
	[SerializeField]
	public InventoryItemType m_ItemType;

	[SerializeField]
	private Transform ButtonsRoot;

	[SerializeField]
	private UIInputTrigger m_InputTrigger;

	[SerializeField]
	private GameObject m_ActiveBody;

	[SerializeField]
	private GameObject m_InActiveBody;

	[SerializeField]
	private GameObject m_NewMarker;

	[SerializeField]
	private UISprite m_ButtonBody;

	private List<OpenInventoryButton> m_OtherButtons = new List<OpenInventoryButton>();

	private bool m_Selected;

	private bool m_NewMarkerShowing;

	private bool m_usable = true;

	[method: MethodImpl(32)]
	public event Action<InventoryItemType> OnButtonClicked;

	private void Awake()
	{
		DeRegisterEventHandler();
		RegisterEventHandler();
		m_OtherButtons = (from b in ButtonsRoot.GetComponentsInChildren<OpenInventoryButton>(true)
			where b != this
			select b).ToList();
		m_NewMarker.gameObject.SetActive(false);
	}

	public OpenInventoryButton SetUsable(bool usable)
	{
		m_usable = usable;
		return this;
	}

	private void RegisterButtonClicked()
	{
		Select();
		if (this.OnButtonClicked != null)
		{
			this.OnButtonClicked(m_ItemType);
		}
	}

	public void Select()
	{
		if (!m_usable)
		{
			return;
		}
		m_Selected = true;
		foreach (OpenInventoryButton otherButton in m_OtherButtons)
		{
			otherButton.Deselect();
		}
		GetComponent<Collider>().enabled = false;
		m_ActiveBody.SetActive(true);
		m_InActiveBody.SetActive(false);
	}

	public void Deselect()
	{
		if (m_Selected)
		{
			GetComponent<Collider>().enabled = true;
			m_ActiveBody.SetActive(false);
			m_InActiveBody.SetActive(true);
		}
	}

	public void Activate(bool activate)
	{
		if (activate)
		{
			RegisterEventHandler();
		}
		else
		{
			DeRegisterEventHandler();
		}
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		if ((bool)m_InputTrigger)
		{
			m_InputTrigger.Clicked += RegisterButtonClicked;
		}
	}

	private void DeRegisterEventHandler()
	{
		if ((bool)m_InputTrigger)
		{
			m_InputTrigger.Clicked -= RegisterButtonClicked;
		}
	}

	private void OnDestroy()
	{
		DeRegisterEventHandler();
	}

	public void SetNewMarker(bool set)
	{
		if (set && !m_NewMarkerShowing)
		{
			m_NewMarker.SetActive(true);
			m_NewMarkerShowing = true;
		}
		else if (!set && m_NewMarkerShowing)
		{
			m_NewMarker.SetActive(false);
			m_NewMarkerShowing = false;
		}
	}

	private void DisableUpdateIndikator()
	{
		m_NewMarker.SetActive(false);
		m_NewMarkerShowing = false;
	}
}
