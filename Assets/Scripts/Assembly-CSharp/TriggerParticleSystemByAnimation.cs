using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Chimera/TriggerParticleSystemByAnimation")]
public class TriggerParticleSystemByAnimation : MonoBehaviour
{
	public List<ParticleSystem> m_ParticleSystems;

	public bool m_OverrideRunningAnimation;

	public void PlayParticleSystem(string nameOfParticleSystem)
	{
		foreach (ParticleSystem particleSystem in m_ParticleSystems)
		{
			if (!(particleSystem == null) && (!particleSystem.isPlaying || m_OverrideRunningAnimation))
			{
				if (string.IsNullOrEmpty(nameOfParticleSystem))
				{
					particleSystem.Play();
				}
				else if (particleSystem.name == nameOfParticleSystem)
				{
					particleSystem.Play();
				}
			}
		}
	}

	public void StopParticleSystem(string nameOfParticleSystem)
	{
		foreach (ParticleSystem particleSystem in m_ParticleSystems)
		{
			if (!(particleSystem == null))
			{
				if (string.IsNullOrEmpty(nameOfParticleSystem))
				{
					particleSystem.Stop();
				}
				else if (particleSystem.name == nameOfParticleSystem)
				{
					particleSystem.Stop();
				}
			}
		}
	}
}
