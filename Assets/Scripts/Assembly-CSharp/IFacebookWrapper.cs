using System;
using System.Collections;
using System.Collections.Generic;

public interface IFacebookWrapper
{
	event Action<List<string>> receivedFriends;

	event Action loginSucceededEvent;

	event Action logoutSucceededEvent;

	event Action logoutFailedEvent;

	event Action<string> loginFailedEvent;

	IFacebookWrapper SetFallbackWrapper(IFacebookWrapper fallback);

	void BeginLogin();

	void BeginLogin(string[] permissions);

	void BeginPostMessage(string message);

	void BeginSendRequest(string message, string[] to = null, string title = "", Action<string, string> responseWithTextAndErrorCallback = null);

	void CheckConnectionAsynch(Action<bool> connected);

	IEnumerator TriggerFallbackLogin();

	bool EndLogin(bool loginSuccessful);

	bool EndPostMessage();

	bool EndSendRequest();

	bool GetFriendIds();

	void Initialize(Action<string> callback);

	bool IsUserAuthenticated();

	string GetUserAuthToken();

	void ReautorizeWithPublishPermissions();

	bool HasPublishPermissions();

	bool Logout();

	string GetNetwork();
}
