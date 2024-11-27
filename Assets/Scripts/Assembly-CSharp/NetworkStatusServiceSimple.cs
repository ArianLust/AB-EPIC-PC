using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Chimera.Library.Components.Interfaces;
using Chimera.Library.Components.Models;
using UnityEngine;

public class NetworkStatusServiceSimple : INetworkStatusService, IHasLogger
{
	private string m_checkAvailalabilityUrl;

	private readonly int m_errorToleranceCount;

	private int m_errorCountSinceLastSuccess;

	public Action<string> Log { get; set; }

	public Action<string> LogError { get; set; }

	public static Func<bool> WP8CheckInternet { get; set; }

	public WWW m_internet { get; set; }

	public Func<bool> ExternalIsNetworkReachable { get; set; }

	[method: MethodImpl(32)]
	public event Action<string> InternetAvailabilityResponseReceived;

	public NetworkStatusServiceSimple(string checkAvailalabilityUrl, int errorToleranceCount)
	{
		Debug.Log("[NetworkStatusServiceSimple] Init with url: " + checkAvailalabilityUrl + " and errorToleranceCount " + errorToleranceCount);
		m_checkAvailalabilityUrl = checkAvailalabilityUrl;
		m_errorToleranceCount = errorToleranceCount;
	}

	public bool IsNetworkReachable()
	{
		return Application.internetReachability != NetworkReachability.NotReachable;
	}

	public IEnumerator CheckInternetAvailability(WebRequest wr, InternetAvailabilityCallback callback)
	{
		Debug.Log("[NetworkStatusServiceSimple] CheckInternetAvailability for " + ((wr == null) ? ("(null) via " + m_checkAvailalabilityUrl) : wr.Url));
		if (m_internet != null)
		{
			Debug.Log("[NetworkStatusServiceSimple] CheckInternetAvailability already running");
			yield break;
		}
		if (Application.internetReachability == NetworkReachability.NotReachable && callback != null)
		{
			Debug.LogError("[NetworkStatusServiceSimple] Internet is not reachable");
			WebRequestFailed(wr, callback);
			yield break;
		}
		string glue = getGlue();
		m_internet = new WWW(m_checkAvailalabilityUrl + glue + "_random=" + UnityEngine.Random.value);
		yield return m_internet;
		if (m_internet.error != null)
		{
			WebRequestFailed(wr, callback);
			if (m_internet != null)
			{
				m_internet.Dispose();
				m_internet = null;
			}
			yield break;
		}
		if (m_internet.error == null && this.InternetAvailabilityResponseReceived != null)
		{
			this.InternetAvailabilityResponseReceived(m_internet.text);
		}
		if (m_internet != null)
		{
			m_internet.Dispose();
			m_internet = null;
		}
		WebRequestSucceeded(wr, callback);
	}

	private string getGlue()
	{
		if (m_checkAvailalabilityUrl.Contains("?"))
		{
			if (m_checkAvailalabilityUrl.EndsWith("?"))
			{
				return string.Empty;
			}
			return "&";
		}
		return "?";
	}

	private void WebRequestFailed(WebRequest wr, InternetAvailabilityCallback callbackToInvokeIfToleranceExceeded)
	{
		if (++m_errorCountSinceLastSuccess > m_errorToleranceCount && callbackToInvokeIfToleranceExceeded != null)
		{
			callbackToInvokeIfToleranceExceeded(wr, false);
		}
	}

	private void WebRequestSucceeded(WebRequest wr, InternetAvailabilityCallback callbackToInvoke)
	{
		m_errorCountSinceLastSuccess = 0;
		if (callbackToInvoke != null)
		{
			callbackToInvoke(wr, true);
		}
	}
}
