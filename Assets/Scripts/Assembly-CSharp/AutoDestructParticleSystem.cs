using System.Collections.Generic;
using UnityEngine;

public class AutoDestructParticleSystem : MonoBehaviour
{
	public List<ParticleSystem> m_ParticleSystems;

	public float CheckFrequencyInSec = 1f;

	private void Awake()
	{
		InvokeRepeating("CheckDestroy", 0f, CheckFrequencyInSec);
	}

	private void CheckDestroy()
	{
		bool flag = true;
		foreach (ParticleSystem particleSystem in m_ParticleSystems)
		{
			if (particleSystem.IsAlive())
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		CancelInvoke();
	}
}
