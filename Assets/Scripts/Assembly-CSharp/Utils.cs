using System;
using Chimera.Library.Components.Interfaces;

internal static class Utils
{
	public static void GetTrustedTimeEx(this ITimingService service, Action<DateTime> callback)
	{
		if (DIContainerInfrastructure.GetCoreStateMgr() != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(service.GetTrustedTime(callback));
			return;
		}
		DebugLog.Error("[Utils] Timing: GetTrustedTime -> no CoreStateMgr available, retuning a dummy time");
		callback(service.GetPresentTime());
	}
}
