using System.Collections.Generic;
using UnityEngine;

public class SoundTriggerList : MonoBehaviour
{
	public List<SoundTriggerPair> m_SoundTriggerPairs = new List<SoundTriggerPair>();

	public void OnTriggerEventFired(string triggerEvent)
	{
		foreach (SoundTriggerPair soundTriggerPair in m_SoundTriggerPairs)
		{
			if (triggerEvent == soundTriggerPair.TriggerId)
			{
				DIContainerInfrastructure.AudioManager.PlaySound(soundTriggerPair.SoundId);
			}
		}
	}

	public void OnTriggerEventStop(string triggerEvent)
	{
		foreach (SoundTriggerPair soundTriggerPair in m_SoundTriggerPairs)
		{
			if (triggerEvent == soundTriggerPair.TriggerId)
			{
				DIContainerInfrastructure.AudioManager.StopSound(soundTriggerPair.SoundId);
			}
		}
	}
}
