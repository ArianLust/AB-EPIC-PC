using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UIInputTrigger : MonoBehaviour
{
	private bool m_triggeredThisFrame;

	private float m_lastTriggerTime;

	private bool m_wp8_pressState;

	private bool m_pressTriggeredThisFrame;

	[method: MethodImpl(32)]
	public event Action Clicked;

	[method: MethodImpl(32)]
	public event Action<bool> Pressed;

	public bool IsClickAllowed()
	{
		return this.Clicked != null;
	}

	public bool IsPressAllowed()
	{
		return this.Clicked != null || this.Pressed != null;
	}

	public void OnClick()
	{
		if (this.Clicked != null && !DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.IsLoading())
		{
			this.Clicked();
		}
	}

	public void OnPress(bool isPressed)
	{
		if (this.Pressed != null && !DIContainerInfrastructure.GetCoreStateMgr().SceneLoadingMgr.IsLoading())
		{
			this.Pressed(isPressed);
		}
	}

	private IEnumerator ResetTriggeredThisFrame()
	{
		yield return new WaitForEndOfFrame();
		m_triggeredThisFrame = false;
		DebugLog.Log("[UIInputTrigger:WP8] m_tiggeredThisFrame reset for " + base.name + ", #" + GetInstanceID() + ", time: " + Time.time);
	}
}
