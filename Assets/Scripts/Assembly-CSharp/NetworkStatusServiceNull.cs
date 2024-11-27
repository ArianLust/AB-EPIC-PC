using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Chimera.Library.Components.Interfaces;
using Chimera.Library.Components.Models;

public class NetworkStatusServiceNull : INetworkStatusService, IHasLogger
{
	public Action<string> Log { get; set; }

	public Action<string> LogError { get; set; }

	public Func<bool> ExternalIsNetworkReachable { get; set; }

	[method: MethodImpl(32)]
	public event Action<string> InternetAvailabilityResponseReceived;

	public bool IsNetworkReachable()
	{
		return true;
	}

	public IEnumerator CheckInternetAvailability(WebRequest wr, InternetAvailabilityCallback callback)
	{
		callback(wr, true);
		yield break;
	}
}
