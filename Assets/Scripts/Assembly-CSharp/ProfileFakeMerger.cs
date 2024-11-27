using ABH.Shared.Models;

public class ProfileFakeMerger : IProfileMerger
{
	public bool TryMergeProfile(PlayerData currentProfile, PlayerData otherProfile, out PlayerData mergedProfile)
	{
		DebugLog.Log(GetType(), "Offline Fake Profile Merger picked local profile!");
		mergedProfile = currentProfile;
		return false;
	}

	public PlayerData MergeProfilesSilent(PlayerData current, PlayerData remote)
	{
		return current;
	}
}
