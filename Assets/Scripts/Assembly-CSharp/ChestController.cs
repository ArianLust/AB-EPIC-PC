using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ChestController : MonoBehaviour
{
	public InstantiateAndTriggerParticleSystemByAnimation m_ParticleSystemSpawner;

	[method: MethodImpl(32)]
	public event Action Clicked;

	private void OnTouchClicked()
	{
		HandleClicked();
	}

	private void HandleClicked()
	{
		if (this.Clicked != null)
		{
			this.Clicked();
		}
	}
}
