using Chimera.Library.Components.Interfaces;

public class SystemInfoNullImpl : ISystemInfo
{
	public long GetFreeStorageExternal()
	{
		return 2147483647L;
	}

	public long GetFreeStorageInternal()
	{
		return 2147483647L;
	}

	public long GetTotalStorageExternal()
	{
		return 2147483647L;
	}

	public long GetTotalStorageInternal()
	{
		return 2147483647L;
	}

	public long GetUsedStorageExternal()
	{
		return 0L;
	}

	public long GetUsedStorageInternal()
	{
		return 0L;
	}

	public InstallLocation GetInstallLocation()
	{
		return InstallLocation.Unknown;
	}

	public string GetLocalCurrencyCode()
	{
		return "N/A";
	}

	public long GetInstalledTimeSecondsUTC()
	{
		return 0L;
	}
}
