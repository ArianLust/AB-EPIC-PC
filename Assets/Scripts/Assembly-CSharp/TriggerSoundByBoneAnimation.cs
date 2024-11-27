using System.Collections.Generic;
using SmoothMoves;
using UnityEngine;

[AddComponentMenu("Chimera/TriggerSoundByBoneAnimation")]
[RequireComponent(typeof(BoneAnimation))]
public class TriggerSoundByBoneAnimation : MonoBehaviour
{
	private BoneAnimation m_BoneAnimation;

	public List<SoundTriggerPair> m_SoundTriggerPairs = new List<SoundTriggerPair>();

	[ExecuteInEditMode]
	private void Awake()
	{
		m_BoneAnimation = GetComponent<BoneAnimation>();
		m_BoneAnimation.RegisterUserTriggerDelegate(OnTriggerEventFired);
	}

	public void OnTriggerEventFired(UserTriggerEvent triggerEvent)
	{
		foreach (SoundTriggerPair soundTriggerPair in m_SoundTriggerPairs)
		{
			if (triggerEvent.tag == soundTriggerPair.TriggerId)
			{
				DIContainerInfrastructure.AudioManager.PlaySound(soundTriggerPair.SoundId, base.gameObject);
			}
		}
	}
}
