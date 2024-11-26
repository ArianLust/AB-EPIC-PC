using System.Collections;
using UnityEngine;

public class PlaySoundInIntervalls : MonoBehaviour
{
	[SerializeField]
	public float m_MinimumWaitTime;

	[SerializeField]
	public float m_MaximumWaitTime;

	[SerializeField]
	public float m_MinimumVolume;

	[SerializeField]
	public float m_MaximumVolume;

	[HideInInspector]
	public Sound m_SoundToPlay;

	public string m_SoundName;

	private float m_currentWaitTime;

	private void Awake()
	{
		m_SoundToPlay = DIContainerInfrastructure.AudioManager.GetSound(m_SoundName) as Sound;
		StartCoroutine(PlaySound(m_SoundName, false));
	}

	public IEnumerator PlaySound(string nameOfSound, bool useInterfaceAudioSource)
	{
		while (true)
		{
			yield return new WaitForSeconds(m_currentWaitTime);
			DIContainerInfrastructure.AudioManager.PlaySound(nameOfSound, base.gameObject);
			DebugLog.Log("Sound Played: " + nameOfSound);
			DebugLog.Log("Volume: " + m_SoundToPlay.Volume);
			m_currentWaitTime = Random.Range(m_MinimumWaitTime, m_MaximumWaitTime);
			DebugLog.Log("CoolDown: " + m_currentWaitTime);
		}
	}
}
