using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class FacebookWrapperNullImpl : IFacebookWrapper
{
	public string FacebookAppId
	{
		get
		{
			return "219805834833490";
		}
	}

	[method: MethodImpl(32)]
	public event Action loginSucceededEvent;

	[method: MethodImpl(32)]
	public event Action logoutSucceededEvent;

	[method: MethodImpl(32)]
	public event Action logoutFailedEvent;

	[method: MethodImpl(32)]
	public event Action<string> loginFailedEvent;

	[method: MethodImpl(32)]
	public event Action<List<string>> receivedFriends;

	[method: MethodImpl(32)]
	public event Action initSucceededEvent;

	public IFacebookWrapper SetFallbackWrapper(IFacebookWrapper fallback)
	{
		return this;
	}

	public void BeginLogin()
	{
		throw new NotImplementedException();
	}

	public void BeginLogin(string[] permissions)
	{
	}

	public void BeginPostMessage(string message)
	{
		throw new NotImplementedException();
	}

	public bool HasPublishPermissions()
	{
		return true;
	}

	public void BeginSendRequest(string request)
	{
		throw new NotImplementedException();
	}

	public bool EndLogin(bool loginSuccessfull)
	{
		throw new NotImplementedException();
	}

	public bool EndPostMessage()
	{
		throw new NotImplementedException();
	}

	public bool EndSendRequest()
	{
		throw new NotImplementedException();
	}

	public void Initialize(Action<string> callback)
	{
		if (callback != null)
		{
			callback(null);
		}
	}

	public bool IsUserAuthenticated()
	{
		return true;
	}

	public string GetUserAuthToken()
	{
		return "CAADH6Yi0blIBAKLxOhcE5ypFCuhP9unIM8eMEZA28bW9ilIavPPW5g4KwE9ZBKGKN1vXWKs4IsKpEUEAqrNuMXESIytJg904Pi7cQREdbyS9PtBPr3a40BnyZArHieDPepva9uX4LxbUSrFLtaJZBDspUfOI3JwXvB1W9ZBxD7ZADoIfZBFiBQ8vlrFERYnNFsXImxaNHwGUwZDZD";
	}

	public void ReautorizeWithPublishPermissions()
	{
		throw new NotImplementedException();
	}

	public bool Logout()
	{
		return true;
	}

	public bool GetFriendIds()
	{
		if (this.receivedFriends != null)
		{
			this.receivedFriends(new List<string>());
		}
		return true;
	}

	public void CheckConnectionAsynch(Action<bool> connected)
	{
	}

	public void BeginSendRequest(string message, string[] to = null, string title = "", Action<string, string> responseWithTextAndErrorCallback = null)
	{
		responseWithTextAndErrorCallback?.Invoke("text", "error");
	}

	public IEnumerator TriggerFallbackLogin()
	{
		yield break;
	}

	public string GetNetwork()
	{
		return "FacebookWrapperNullImpl";
	}
}
