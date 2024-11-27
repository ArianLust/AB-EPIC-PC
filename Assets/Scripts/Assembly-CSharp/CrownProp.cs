using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CrownProp : MonoBehaviour
{
	[HideInInspector]
	public bool m_IsInitialized;

	private bool m_ClickDisabled;

	[method: MethodImpl(32)]
	public event Action OnPropClicked;

	public void Init()
	{
		m_IsInitialized = true;
	}

	private void OnTouchClicked()
	{
		if (!m_ClickDisabled)
		{
			HandleClicked();
		}
	}

	public void HandleClicked()
	{
		DebugLog.Log("Prop Clicked");
		if (this.OnPropClicked != null)
		{
			this.OnPropClicked();
		}
	}

	internal void SetClickable(bool isClickable)
	{
		m_ClickDisabled = !isClickable;
	}
}
