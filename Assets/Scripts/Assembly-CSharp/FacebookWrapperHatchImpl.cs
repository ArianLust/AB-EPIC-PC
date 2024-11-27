using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Facebook.Unity;
using Rcs;

public class FacebookWrapperHatchImpl : IFacebookWrapper
{
	private readonly Friends m_friendService;

	private bool m_isAuthenticated;

	private bool m_retrieveFriendsInProgress;

	private readonly Dictionary<string, string> m_friendRovioAccIdsToFacebookIdsMapping = new Dictionary<string, string>();

	private readonly Action<User.SocialNetworkProfile> m_doSomethingWhenOwnProfileIsRetrieved;

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

	public FacebookWrapperHatchImpl(Friends friendsService, Action<User.SocialNetworkProfile> doSomethingWhenOwnProfileIsRetrieved)
	{
		m_friendService = friendsService;
		m_doSomethingWhenOwnProfileIsRetrieved = doSomethingWhenOwnProfileIsRetrieved ?? ((Action<User.SocialNetworkProfile>)delegate
		{
		});
	}

	public void BeginLogin()
	{
		DebugLog.Log("[FacebookWrapperHatchImpl] Begin Login!");
		m_friendService.Connect(User.SocialNetwork.SocialNetworkFacebook, OnConnected, OnConnectError);
	}

	public void BeginLogin(string[] permissions)
	{
		DebugLog.Warn(GetType(), "Begin Login called with permission parameter. This impl ignores those parameters!");
		BeginLogin();
	}

	public void Initialize(Action<string> callback)
	{
		DebugLog.Log(GetType(), "Initializing!");
		FB.Init(delegate
		{
			OnInitCompleted();
			if (callback != null)
			{
				callback(null);
			}
		});
	}

	private void OnInitCompleted()
	{
		DebugLog.Log(GetType(), "OnInitCompleted:");
		DebugLog.Log(GetType(), string.Format("FB AppId: {0}", FB.AppId));
		DebugLog.Log(GetType(), string.Format("LoggedIn: {0}", FB.IsLoggedIn));
		m_friendService.IsConnected(User.SocialNetwork.SocialNetworkFacebook, OnIsConnectedSuccess, OnIsConnectedError);
	}

	private void OnIsConnectedError(User.SocialNetwork socialNetwork, User.SocialNetworkProfile profileInIdentity, User.SocialNetworkProfile profileInDevice, Friends.IsConnectedError error)
	{
		DebugLog.Warn("[FacebookWrapperHatchImpl] Error with IsConnected: " + error);
		m_isAuthenticated = false;
	}

	private void OnIsConnectedSuccess(User.SocialNetwork socialNetwork, User.SocialNetworkProfile profileInIdentity, User.SocialNetworkProfile profileInDevice)
	{
		DebugLog.Log("[FacebookWrapperHatchImpl] User is connected");
		m_isAuthenticated = true;
	}

	public bool IsUserAuthenticated()
	{
		DebugLog.Log("[FacebookWrapperHatchImpl] asking for authentication: " + m_isAuthenticated);
		return m_isAuthenticated;
	}

	public bool Logout()
	{
		DebugLog.Log(GetType(), "Logout!");
		m_friendService.Disconnect(User.SocialNetwork.SocialNetworkFacebook, OnDisconnectSuccess, OnDisconnectError);
		return true;
	}

	public bool GetFriendIds()
	{
		if (m_retrieveFriendsInProgress)
		{
			return true;
		}
		if (!IsUserAuthenticated())
		{
			DebugLog.Log(GetType(), "Not Connected abort refresh!");
			return false;
		}
		m_retrieveFriendsInProgress = true;
		DebugLog.Log(GetType(), "Refresh Friend List!");
		m_friendService.GetFriends(OnGetFriendsSuccess, OnGetFriendsError);
		return true;
	}

	public void CheckConnectionAsynch(Action<bool> connected)
	{
		connected = connected ?? ((Action<bool>)delegate
		{
		});
		DebugLog.Log(GetType(), "Check connection to Network: " + User.SocialNetwork.SocialNetworkFacebook);
		m_friendService.IsConnected(User.SocialNetwork.SocialNetworkFacebook, delegate
		{
			DebugLog.Log(GetType(), "CheckConnectionAsynch: already connected!");
			connected(true);
		}, delegate(User.SocialNetwork network, User.SocialNetworkProfile identity, User.SocialNetworkProfile device, Friends.IsConnectedError error)
		{
			DebugLog.Warn(GetType(), "CheckConnectionAsynch: not yet connected, error: " + error);
			connected(false);
		});
	}

	public string GetNetwork()
	{
		return Social.Service.ServiceFacebook.ToString();
	}

	private void OnConnectError(User.SocialNetwork socialnetwork, Friends.ConnectError error)
	{
		m_isAuthenticated = false;
		DebugLog.Error(GetType(), "OnConnectError: " + error);
		if (this.loginFailedEvent != null)
		{
			this.loginFailedEvent(error.ToString());
		}
	}

	private void OnConnected(User.SocialNetwork socialnetwork, User.SocialNetworkProfile profile)
	{
		DebugLog.Log(GetType(), "OnConnected success, now retrieving the friend list");
		m_doSomethingWhenOwnProfileIsRetrieved(profile);
		m_isAuthenticated = true;
		if (this.loginSucceededEvent != null)
		{
			this.loginSucceededEvent();
		}
		GetFriendIds();
	}

	private void OnGetFriendsError(Friends.GetFriendsError error)
	{
		DebugLog.Error(GetType(), "OnGetFriendsError: " + error);
		m_retrieveFriendsInProgress = false;
		if (this.receivedFriends != null)
		{
			this.receivedFriends(new List<string>());
		}
	}

	private void OnGetFriendsSuccess(List<User> users)
	{
		m_retrieveFriendsInProgress = false;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (User user in users)
		{
			User.SocialNetworkProfile socialNetworkProfile = user.GetSocialNetworkProfiles().FirstOrDefault((User.SocialNetworkProfile profile) => profile.SocialNetwork == User.SocialNetwork.SocialNetworkFacebook);
			if (socialNetworkProfile != null && !string.IsNullOrEmpty(socialNetworkProfile.Uid))
			{
				dictionary.Add(user.GetAccountId(), socialNetworkProfile.Uid);
				DebugLog.Log(GetType(), "Found facebook Id for rovio nick (global / social)" + user.GetName(User.Type.TypeGlobal) + " / " + user.GetName(User.Type.TypeSocial) + ": " + socialNetworkProfile.Uid);
			}
			else
			{
				DebugLog.Log(GetType(), "Found no facebook Id for rovio nick (global / social)" + user.GetName(User.Type.TypeGlobal) + " / " + user.GetName(User.Type.TypeSocial));
			}
		}
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			if (!m_friendRovioAccIdsToFacebookIdsMapping.ContainsKey(item.Key))
			{
				m_friendRovioAccIdsToFacebookIdsMapping.Add(item.Key, item.Value);
			}
		}
		List<string> list = Enumerable.ToList(users.Select((User u) => u.GetAccountId()));
		DebugLog.Log(GetType(), "OnGetFriendsSuccess: Got " + list.Count + " friends");
		if (this.receivedFriends != null)
		{
			this.receivedFriends(list);
		}
	}

	private void OnDisconnectError(User.SocialNetwork socialNetwork)
	{
		DebugLog.Error(GetType(), "OnDisconnectError for network " + socialNetwork);
		if (this.logoutFailedEvent != null)
		{
			this.logoutFailedEvent();
		}
	}

	private void OnDisconnectSuccess(User.SocialNetwork socialNetwork)
	{
		DebugLog.Log(GetType(), "On Disconnected success");
		m_isAuthenticated = false;
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.ReceivedFriends(new List<string>());
		if (this.logoutSucceededEvent != null)
		{
			this.logoutSucceededEvent();
		}
	}

	public void BeginSendRequest(string message, string[] to = null, string title = "", Action<string, string> responseWithTextAndErrorCallback = null)
	{
		responseWithTextAndErrorCallback = responseWithTextAndErrorCallback ?? ((Action<string, string>)delegate
		{
		});
		FB.AppRequest(message, to, null, null, null, null, title, delegate(IAppRequestResult result)
		{
			DebugLog.Log(GetType(), string.Format("BeginSendRequest FB.AppRequest callback. Text = {0}, Error = {1}", result.RawResult, result.Error));
			responseWithTextAndErrorCallback(result.RawResult, result.Error);
		});
	}

	public void BeginPostMessage(string message)
	{
		DebugLog.Warn(GetType(), "BeginPostMessage not implemented/needed");
	}

	public bool HasPublishPermissions()
	{
		DebugLog.Warn(GetType(), "HasPublishPermissions not implemented/needed, returning true always");
		return true;
	}

	public bool EndLogin(bool loginSuccessfull)
	{
		DebugLog.Warn(GetType(), "EndLogin not implemented/needed");
		return false;
	}

	public bool EndPostMessage()
	{
		DebugLog.Warn(GetType(), "EndPostMessage not implemented/needed");
		return false;
	}

	public bool EndSendRequest()
	{
		DebugLog.Warn(GetType(), "EndSendRequest not implemented/needed");
		return false;
	}

	public IFacebookWrapper SetFallbackWrapper(IFacebookWrapper fallback)
	{
		DebugLog.Warn(GetType(), "SetFallbackWrapper not implemented/needed");
		return this;
	}

	public IEnumerator TriggerFallbackLogin()
	{
		DebugLog.Warn(GetType(), "TriggerFallbackLogin not implemented/needed");
		yield break;
	}

	public string GetUserAuthToken()
	{
		DebugLog.Warn(GetType(), "GetUserAuthToken not implemented/needed, retuning string.Empty");
		return string.Empty;
	}

	public void ReautorizeWithPublishPermissions()
	{
		DebugLog.Warn(GetType(), "ReautorizeWithPublishPermissions not implemented/needed");
	}

	public string GetFacebookIdForFriendRovioAccId(string rovioAccId)
	{
		string value = null;
		m_friendRovioAccIdsToFacebookIdsMapping.TryGetValue(rovioAccId, out value);
		return value;
	}
}
