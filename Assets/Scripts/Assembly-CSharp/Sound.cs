using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound : ISound
{
	public string NameId;

	public List<string> AssetNames = new List<string>();

	public int ChannelId;

	private bool OneShot;

	private bool Music;

	private bool MusicSecondary;

	private bool Looping;

	public bool Muted;

	public float Volume = 1f;

	private float FadeTime;

	[SerializeField]
	public List<SoundParameter> Params = new List<SoundParameter>();

	[SerializeField]
	public AssetbundleIndex AssetPrioritiy;

	[HideInInspector]
	public AudioSource Source;

	public float Time { get; set; }

	public bool IsPlaying
	{
		get
		{
			return Source.isPlaying;
		}
	}

	public float Length
	{
		get
		{
			if (Source == null || Source.clip == null)
			{
				return 0f;
			}
			return Source.clip.length;
		}
	}

	public void Init()
	{
		foreach (SoundParameter param in Params)
		{
			switch (param.Type)
			{
			case SoundParameterType.Looping:
				Looping = true;
				break;
			case SoundParameterType.OneShot:
				OneShot = true;
				break;
			case SoundParameterType.CurrentPrimaryMusicSource:
				Music = true;
				Source = DIContainerInfrastructure.PrimaryMusicSource;
				break;
			case SoundParameterType.CurrentSecondaryMusicSource:
				MusicSecondary = true;
				Source = DIContainerInfrastructure.SecondaryMusicSource;
				break;
			case SoundParameterType.Croosfade:
				FadeTime = param.Value;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	private string GetAssetName()
	{
		if (AssetNames.Count <= 0)
		{
			return NameId;
		}
		return AssetNames[UnityEngine.Random.Range(0, AssetNames.Count)];
	}

	public void Start()
	{
		if (Music)
		{
			Source = DIContainerInfrastructure.PrimaryMusicSource;
		}
		if (MusicSecondary)
		{
			Source = DIContainerInfrastructure.SecondaryMusicSource;
		}
		if (OneShot)
		{
			if (!Muted)
			{
				Source.PlayOneShot(DIContainerInfrastructure.GetAudioAssetProvider(AssetPrioritiy).GetAudioClip(GetAssetName()), Volume);
			}
		}
		else if (FadeTime > 0f)
		{
			Source.loop = Looping;
			Source.volume = Volume;
			AudioClip audioClip = DIContainerInfrastructure.GetAudioAssetProvider(AssetPrioritiy).GetAudioClip(GetAssetName());
			if (!Source || !(Source.clip == audioClip))
			{
				Source.mute = Muted;
				DIContainerInfrastructure.AudioManager.StartCoroutine(DIContainerInfrastructure.AudioManager.FadeMusic(Source, audioClip, FadeTime, Time));
			}
		}
		else
		{
			Source.loop = Looping;
			Source.mute = Muted;
			Source.volume = Volume;
			AudioClip audioClip2 = DIContainerInfrastructure.GetAudioAssetProvider(AssetPrioritiy).GetAudioClip(GetAssetName());
			Source.clip = audioClip2;
			Source.Play();
		}
	}

	public void Remove()
	{
		if ((bool)Source && !OneShot)
		{
			Source.Stop();
			Source.clip = null;
		}
	}

	public void Stop()
	{
		if ((bool)Source && !OneShot)
		{
			Source.Pause();
		}
	}
}
