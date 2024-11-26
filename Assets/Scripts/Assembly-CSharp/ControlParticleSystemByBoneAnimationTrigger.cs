using System.Collections.Generic;
using SmoothMoves;
using UnityEngine;

[RequireComponent(typeof(BoneAnimation))]
public class ControlParticleSystemByBoneAnimationTrigger : MonoBehaviour
{
	public List<ParticleSystem> m_ParticleSystems;

	private BoneAnimation m_BoneAnimation;

	public string m_TriggerTag;

	public ParticleSystemActionType m_ActionType;

	private void Awake()
	{
		m_BoneAnimation = GetComponent<BoneAnimation>();
		m_BoneAnimation.RegisterUserTriggerDelegate(TriggerDelegate);
	}

	private void TriggerDelegate(UserTriggerEvent triggerEvent)
	{
		if (m_ParticleSystems == null || triggerEvent.tag != m_TriggerTag)
		{
			return;
		}
		switch (m_ActionType)
		{
		case ParticleSystemActionType.Play:
		{
			foreach (ParticleSystem particleSystem in m_ParticleSystems)
			{
				if (!particleSystem)
				{
					DebugLog.Error("ControlParticleSystemByBoneAnimationTrigger Particle System on trigger: " + triggerEvent.tag + " is missing!");
				}
				else
				{
					particleSystem.Play();
				}
			}
			break;
		}
		case ParticleSystemActionType.Stop:
		{
			foreach (ParticleSystem particleSystem2 in m_ParticleSystems)
			{
				if (!particleSystem2)
				{
					DebugLog.Error("ControlParticleSystemByBoneAnimationTrigger Particle System on trigger: " + triggerEvent.tag + " is missing!");
				}
				else
				{
					particleSystem2.Stop();
				}
			}
			break;
		}
		case ParticleSystemActionType.Destroy:
		{
			foreach (ParticleSystem particleSystem3 in m_ParticleSystems)
			{
				if (!particleSystem3)
				{
					DebugLog.Error("ControlParticleSystemByBoneAnimationTrigger Particle System on trigger: " + triggerEvent.tag + " is missing!");
					continue;
				}
				particleSystem3.Clear();
				Object.Destroy(particleSystem3);
			}
			break;
		}
		}
	}

	private void OnDestroy()
	{
		if (m_BoneAnimation != null)
		{
			m_BoneAnimation.UnregisterUserTriggerDelegate(TriggerDelegate);
		}
	}
}
