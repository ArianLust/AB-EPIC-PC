using System;
using System.Collections;
using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using UnityEngine;

public class SoundManager : MonoBehaviour, ISoundManager
{
	public List<Sound> Sounds = new List<Sound>();

	public AudioSource m_DefaultSource;

	private Dictionary<int, Func<bool>> UnmuteActions = new Dictionary<int, Func<bool>>();

	private readonly Dictionary<string, Sound> m_SoundLookup = new Dictionary<string, Sound>();

	private readonly Dictionary<int, List<Sound>> m_ChannelLookup = new Dictionary<int, List<Sound>>();

	[SerializeField]
	public List<PriorityAssetProvider> m_AudioAssetProviders;

	[SerializeField]
	public readonly Dictionary<int, List<string>> m_muteReasonsPerChannel = new Dictionary<int, List<string>>();

	public ISoundManager Initialize()
	{
		foreach (Sound sound in Sounds)
		{
			if (!m_SoundLookup.ContainsKey(sound.NameId))
			{
				m_SoundLookup.Add(sound.NameId, sound);
			}
			if (!m_ChannelLookup.ContainsKey(sound.ChannelId))
			{
				m_ChannelLookup.Add(sound.ChannelId, new List<Sound>());
			}
			m_ChannelLookup[sound.ChannelId].Add(sound);
			sound.Source = m_DefaultSource;
			sound.Init();
		}
		Sounds.Clear();
		if (DIContainerInfrastructure.GetCurrentPlayer() != null && DIContainerInfrastructure.GetCurrentPlayer().Data.IsMusicMuted)
		{
			Mute(true, 0);
		}
		if (DIContainerInfrastructure.GetCurrentPlayer() != null && DIContainerInfrastructure.GetCurrentPlayer().Data.IsSoundMuted)
		{
			Mute(true, 1);
		}
		return this;
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public SoundManager AddUnmuteAction(int channel, Func<bool> action)
	{
		if (!UnmuteActions.ContainsKey(channel))
		{
			UnmuteActions.Add(channel, action);
		}
		return this;
	}

	public void PlayCharacterTypeSound(string baseName, ICharacter character, GameObject objectSource)
	{
		if (character is BirdGameData)
		{
			PlaySound("Bird_" + baseName, objectSource);
		}
		else
		{
			PlaySound("Pig_" + baseName, objectSource);
		}
	}

	public void PlayCharacterSound(string baseName, ICharacter character, GameObject objectSource)
	{
		PlaySound(character.AssetName + "_" + baseName, objectSource);
	}

	public void PlayMusic(string key)
	{
		PlaySound(key, DIContainerInfrastructure.PrimaryMusicSource.gameObject);
	}

	public void StopSound(string key)
	{
		if (m_SoundLookup.ContainsKey(key))
		{
			Sound sound = m_SoundLookup[key];
			sound.Stop();
			if (sound.Source != null && sound.Source.clip != null && key == sound.Source.clip.name)
			{
				m_SoundLookup[key].Source.clip = null;
			}
		}
		else
		{
			DebugLog.Error(GetType(), string.Format("... sound is not listed: '{0}'", key));
		}
	}

	public ISound PlaySound(string key)
	{
		return PlaySound(key, true);
	}

	public ISound PlaySound(string key, bool defaultSource)
	{
		if (m_SoundLookup.ContainsKey(key))
		{
			if (defaultSource)
			{
				m_SoundLookup[key].Source = m_DefaultSource;
			}
			m_SoundLookup[key].Start();
			return m_SoundLookup[key];
		}
		return null;
	}

	public ISound PlaySound(string key, GameObject gob)
	{
		if (!gob || !gob.GetComponent<AudioSource>())
		{
			return PlaySound(key);
		}
		if (m_SoundLookup.ContainsKey(key))
		{
			m_SoundLookup[key].Source = gob.GetComponent<AudioSource>();
			m_SoundLookup[key].Start();
			return m_SoundLookup[key];
		}
		return null;
	}

	public ISound GetSound(string key)
	{
		if (m_SoundLookup.ContainsKey(key))
		{
			return m_SoundLookup[key];
		}
		return null;
	}

	public ISound GetSound(string key, GameObject gob)
	{
		if (m_SoundLookup.ContainsKey(key))
		{
			m_SoundLookup[key].Source = gob.GetComponent<AudioSource>();
			return m_SoundLookup[key];
		}
		DebugLog.Error("No such Sound available");
		return null;
	}

	public void SetVolume(float volume, int channelId)
	{
		if (!m_ChannelLookup.ContainsKey(channelId))
		{
			return;
		}
		foreach (Sound item in m_ChannelLookup[channelId])
		{
			if (item.Source != null)
			{
				item.Source.volume = volume;
			}
			item.Volume = volume;
		}
	}

	public float GetVolume(int channelId)
	{
		if (!m_ChannelLookup.ContainsKey(channelId))
		{
			return 0f;
		}
		using (List<Sound>.Enumerator enumerator = m_ChannelLookup[channelId].GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Sound current = enumerator.Current;
				if (current.Source != null)
				{
					return current.Source.volume;
				}
				return current.Volume;
			}
		}
		return 0f;
	}

	public IEnumerator FadeOut(float time)
	{
		float cTime = 0f;
		while (DIContainerInfrastructure.AudioManager.GetVolume(0) > 0f && DIContainerInfrastructure.AudioManager.GetVolume(1) > 0f)
		{
			cTime += Time.deltaTime;
			float cVol0 = Mathf.Lerp(GetVolume(0), 0f, cTime / time);
			float cVol1 = Mathf.Lerp(GetVolume(1), 0f, cTime / time);
			SetVolume(cVol0, 0);
			SetVolume(cVol1, 1);
			yield return new WaitForEndOfFrame();
		}
	}

	public IEnumerator FadeIn(float time)
	{
		float cTime = 0f;
		while (DIContainerInfrastructure.AudioManager.GetVolume(0) < 1f && DIContainerInfrastructure.AudioManager.GetVolume(1) < 1f)
		{
			cTime += Time.deltaTime;
			float cVol0 = Mathf.Lerp(DIContainerInfrastructure.AudioManager.GetVolume(0), 1f, cTime / time);
			float cVol1 = Mathf.Lerp(DIContainerInfrastructure.AudioManager.GetVolume(1), 1f, cTime / time);
			DIContainerInfrastructure.AudioManager.SetVolume(cVol0, 0);
			DIContainerInfrastructure.AudioManager.SetVolume(cVol1, 1);
			yield return new WaitForEndOfFrame();
		}
	}

	public void AddMuteReason(int channelId, string reason)
	{
		List<string> muteReasonsFor = GetMuteReasonsFor(channelId);
		if (!muteReasonsFor.Contains(reason))
		{
			muteReasonsFor.Add(reason);
			if (muteReasonsFor.Count == 1)
			{
				Mute(true, channelId);
			}
		}
	}

	public void RemoveMuteReason(int channelId, string reason)
	{
		List<string> muteReasonsFor = GetMuteReasonsFor(channelId);
		if (muteReasonsFor.Remove(reason) && muteReasonsFor.Count == 0)
		{
			Mute(false, channelId);
		}
	}

	private List<string> GetMuteReasonsFor(int channelId)
	{
		List<string> value;
		if (!m_muteReasonsPerChannel.TryGetValue(channelId, out value))
		{
			value = new List<string>();
			m_muteReasonsPerChannel.Add(channelId, value);
		}
		return value;
	}

	private void Mute(bool mute, int channelId)
	{
		if (!m_ChannelLookup.ContainsKey(channelId))
		{
			return;
		}
		foreach (Sound item in m_ChannelLookup[channelId])
		{
			if (item.Source != null)
			{
				item.Source.mute = mute;
			}
			item.Muted = mute;
		}
	}

	public bool IsMuted(int channelId)
	{
		if (!m_ChannelLookup.ContainsKey(channelId))
		{
			return true;
		}
		using (List<Sound>.Enumerator enumerator = m_ChannelLookup[channelId].GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Sound current = enumerator.Current;
				if (current.Source != null)
				{
					return current.Source.mute;
				}
				return current.Muted;
			}
		}
		return true;
	}

	public IEnumerator FadeMusic(AudioSource Source, AudioClip audioClip, float FadeTime, float startTime)
	{
		float timeLeft = FadeTime;
		float startVolume = Source.volume;
		if (Source.clip != null)
		{
			AudioClip oldClip = Source.clip;
			while (timeLeft > FadeTime * 0.5f)
			{
				timeLeft -= Time.deltaTime;
				Source.volume = Mathf.Lerp(startVolume, 0f, 1f - (timeLeft - FadeTime * 0.5f) / FadeTime * 0.5f);
				yield return new WaitForEndOfFrame();
			}
			Source.Stop();
		}
		else
		{
			Source.volume = 0f;
		}
		Source.clip = audioClip;
		Source.time = startTime;
		Source.Play();
		while (timeLeft > 0f)
		{
			timeLeft -= Time.deltaTime;
			Source.volume = Mathf.Lerp(0f, startVolume, 1f - timeLeft / FadeTime * 0.5f);
			yield return new WaitForEndOfFrame();
		}
	}
}
