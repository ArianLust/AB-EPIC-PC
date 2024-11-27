using UnityEngine;

public abstract class UnitySoundManager : MonoBehaviour, ISoundManager
{
	public abstract ISoundManager Initialize();

	public abstract ISound PlaySound(string key);

	public abstract ISound PlaySound(string key, GameObject gob);

	public abstract ISound GetSound(string key);

	public abstract ISound GetSound(string key, GameObject gob);

	public abstract void SetVolume(float volume, int channelId);

	public abstract float GetVolume(int channelId);

	public abstract void AddMuteReason(int channelId, string reason);

	public abstract void RemoveMuteReason(int channelId, string reason);

	public abstract void Mute(bool mute, int channelId);

	public abstract bool IsMuted(int channelId);
}
