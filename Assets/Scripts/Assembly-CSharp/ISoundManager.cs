using UnityEngine;

public interface ISoundManager
{
	ISoundManager Initialize();

	ISound PlaySound(string key);

	ISound PlaySound(string key, GameObject gob);

	ISound GetSound(string key);

	ISound GetSound(string key, GameObject gob);

	void SetVolume(float volume, int channelId);

	float GetVolume(int channelId);

	void AddMuteReason(int channelId, string reason);

	void RemoveMuteReason(int channelId, string reason);

	bool IsMuted(int channelId);
}
