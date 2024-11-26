using ABH.Shared.Models;

public interface IProfileMerger
{
	bool TryMergeProfile(PlayerData currentProfile, PlayerData otherProfile, out PlayerData mergedProfile);

	PlayerData MergeProfilesSilent(PlayerData current, PlayerData remote);
}
