using System;
using Rcs;

namespace Interfaces.Identity
{
	public interface IIdentityService
	{
		IdentityCredentials UserCredentials { get; }

		string AccountId { get; }

		string SharedId { get; }

		event Action OnLoggedIn;

		event Action<int, string> OnLoginError;

		void Initialize();

		void Login(Rcs.Identity.LoginView view);

		void LoginGuest();

		void LoginAuto();

		void LoginFacebook();

		void Logout();

		bool IsGuest();

		void ValidateNickname(string nickname, Rcs.Identity.ValidateNicknameSuccessCallback OnSuccess, Rcs.Identity.ValidateNicknameErrorCallback OnError);
	}
}
