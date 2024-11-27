using UnityEngine;

[AddComponentMenu("Chimera/TriggerSoundByAnimation")]
public class TriggerSoundByAnimation : MonoBehaviour
{
	public void PlaySound(string nameOfSound)
	{
		SoundManager audioManager = DIContainerInfrastructure.AudioManager;
		if (audioManager != null)
		{
			audioManager.PlaySound(nameOfSound, base.gameObject);
		}
	}

	public void PlaySoundFromDefaultSource(string nameOfSound)
	{
		DIContainerInfrastructure.AudioManager.PlaySound(nameOfSound);
	}

	public void StopSound(string nameOfSound)
	{
		DIContainerInfrastructure.AudioManager.StopSound(nameOfSound);
	}
}
