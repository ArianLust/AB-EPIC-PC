public interface ISound
{
	bool IsPlaying { get; }

	float Time { get; set; }

	float Length { get; }

	void Start();

	void Stop();
}
