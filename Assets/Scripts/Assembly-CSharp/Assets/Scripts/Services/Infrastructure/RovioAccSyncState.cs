using System;
using ABH.Shared.Models;
using Rcs;

namespace Assets.Scripts.Services.Infrastructure
{
	public class RovioAccSyncState
	{
		private enum AsyncOperation
		{
			Nothing = 0,
			SaveBeforeLogin = 1,
			SaveBeforeLogout = 2,
			Login = 3,
			Logout = 4,
			LoginRetrievingProfile = 5,
			LogoutRetrievingProfile = 6,
			MoveToCloudQuestion = 7,
			ResettingGame = 8,
			LoginErrorSignBackInAsGuest = 9
		}

		private AsyncOperation m_currentOp;

		private bool m_exited;

		private ABH.Shared.Models.PlayerData m_localGuestAcc;

		private bool m_enteredUsingRovioAcc;

		private ABH.Shared.Models.PlayerData m_dataAfter;

		private bool m_isDataAfterValid;

		private bool m_listingForIdentityEvents;

		public bool IsAccountActionPossible()
		{
			return m_currentOp == AsyncOperation.Nothing;
		}

		public void SignInSignOutButtonPressed()
		{
			DebugLog.Log(GetType(), "SignInSignOutButtonPressed: enter");
			DebugLog.Log(GetType(), "SignInSignOutButtonPressed: ContentLoader.Instance: " + ((!(ContentLoader.Instance == null)) ? "not null" : "null"));
			DebugLog.Log(GetType(), "SignInSignOutButtonPressed: ContentLoader.Instance.m_BeaconConnectionMgr: " + ((!(ContentLoader.Instance.m_BeaconConnectionMgr == null)) ? "not null" : "null"));
			DebugLog.Log(GetType(), "SignInSignOutButtonPressed: ContentLoader.Instance.m_BeaconConnectionMgr.IsGuestAccount(): " + ContentLoader.Instance.m_BeaconConnectionMgr.IsGuestAccount());
			if (ContentLoader.Instance.m_BeaconConnectionMgr.IsGuestAccount())
			{
				LoginIfPossible(delegate
				{
					DIContainerInfrastructure.IdentityService.Login(Identity.LoginView.ViewSignIn);
					DIContainerInfrastructure.GetCoreStateMgr().BlockAllInput(true);
				});
				return;
			}
			DIContainerInfrastructure.GetCoreStateMgr().ShowConfirmationPopup(DIContainerInfrastructure.GetLocaService().Tr("popup_confirmation_sign_out", "Your progress will be saved online only when you are signed in.\nYou can continue your game when you sign in to your Rovio Account"), delegate
			{
				// LogoutIfPossible(delegate
				// {
				// 	DIContainerInfrastructure.IdentityService.Logout();
				// 	DIContainerInfrastructure.GetAssetData().FetchAndResetProfileOnNextLogin = true;
				// 	DIContainerInfrastructure.GetAssetData().Save();
				// 	DIContainerInfrastructure.IdentityService.LoginGuest();
				// });
			}, delegate
			{
			});
		}

		private void ShowLoadingSpinner()
		{
			DIContainerInfrastructure.GetAsynchStatusService().ShowLocalLoading(DIContainerInfrastructure.GetLocaService().Tr("toast_account_sync", "Syncing Account"), true);
		}

		private void HideLoadingSpinner()
		{
			DIContainerInfrastructure.GetAsynchStatusService().HideLocalLoading();
		}

		private void LoginIfPossible(Action doAfter)
		{
			if (m_currentOp != 0)
			{
				DebugLog.Warn(GetType(), "LoginIfPossible but ignoring, " + this);
				return;
			}
			ShowLoadingSpinner();
			DebugLog.Warn(GetType(), "LoginIfPossible: starting login. " + this);
			m_currentOp = AsyncOperation.SaveBeforeLogin;
			SaveOneLastTime(AsyncOperation.Login, doAfter);
		}

		private void LogoutIfPossible(Action doAfter)
		{
			if (m_currentOp != 0)
			{
				DebugLog.Warn(GetType(), "LogoutIfPossible but ignoring, " + this);
				return;
			}
			ShowLoadingSpinner();
			DebugLog.Warn(GetType(), "LogoutIfPossible: starting logout. " + this);
			m_currentOp = AsyncOperation.SaveBeforeLogout;
			SaveOneLastTime(AsyncOperation.Logout, doAfter);
		}

		private void SaveOneLastTime(AsyncOperation opThen, Action doAfter)
		{
			ABH.Shared.Models.PlayerData currentProfile = DIContainerInfrastructure.GetProfileMgr().CurrentProfile;
			if (currentProfile == null)
			{
				DebugLog.Warn(GetType(), string.Concat("SaveOneLastTime: not saved because profile == null, setting state to  ", opThen, ", ", this));
				EnterAsyncOp(opThen);
				doAfter();
				return;
			}
			DIContainerInfrastructure.RemoteStorageService.SyncProfileAndResolveConflict(DIContainerInfrastructure.GetProfileMgr().CurrentProfile, DIContainerLogic.ProfileMerger.MergeProfilesSilent, delegate
			{
				DebugLog.Warn(GetType(), string.Concat("SaveOneLastTime: Save completed, setting state to  ", opThen, ", ", this));
				EnterAsyncOp(opThen);
				doAfter();
			}, true);
		}

		private void EnterAsyncOp(AsyncOperation asyncOperation)
		{
			if (!m_listingForIdentityEvents)
			{
				RegisterIdentityEvents();
			}
			DIContainerInfrastructure.RemoteStorageService.DisableProfileSync(GetType().Name, true);
			bool flag = DIContainerInfrastructure.IdentityService.IsGuest();
			if (flag)
			{
				m_localGuestAcc = DIContainerInfrastructure.GetProfileMgr().CurrentProfile;
			}
			m_enteredUsingRovioAcc = m_enteredUsingRovioAcc || !flag;
			m_currentOp = asyncOperation;
			DebugLog.Log(GetType(), "EnterAsyncOp, " + this);
		}

		private void RegisterIdentityEvents()
		{
			m_listingForIdentityEvents = true;
			DIContainerInfrastructure.IdentityService.OnLoggedIn += BeaconIdentityLoggedIn;
			DIContainerInfrastructure.IdentityService.OnLoginError += BeaconIdentityLoginError;
		}

		private void UnregisterIdentityEvents()
		{
			DIContainerInfrastructure.IdentityService.OnLoggedIn -= BeaconIdentityLoggedIn;
			DIContainerInfrastructure.IdentityService.OnLoginError -= BeaconIdentityLoginError;
			m_listingForIdentityEvents = false;
		}

		public void RegisterButtonPressed()
		{
			LoginIfPossible(delegate
			{
				DIContainerInfrastructure.IdentityService.Login(Identity.LoginView.ViewSignUp);
				DIContainerInfrastructure.GetCoreStateMgr().BlockAllInput(true);
			});
		}

		private void LoginCompleted()
		{
			if (m_currentOp == AsyncOperation.LoginErrorSignBackInAsGuest)
			{
				DebugLog.Warn(GetType(), "Login as guest completed, now setting async op to Nothing " + this);
				FinishAsyncOp();
			}
			else if (m_currentOp != AsyncOperation.Login && m_currentOp != AsyncOperation.Logout)
			{
				DebugLog.Warn(GetType(), "LoginCompleted but ignoring, " + this);
			}
			else
			{
				RetrieveCurrentProfile();
			}
		}

		public void ExitRovioIdScreen()
		{
			if (m_currentOp == AsyncOperation.MoveToCloudQuestion)
			{
				FinishAsyncOp();
				DebugLog.Log(GetType(), "ExitRovioIdScreen while MoveToCloudQuestion was active. assuming this was cancelled. " + this);
			}
			if (m_currentOp == AsyncOperation.Nothing && !m_isDataAfterValid)
			{
				DebugLog.Log(GetType(), "ExitRovioIdScreen, game reset not necessary, " + this);
				ResetAsyncOpState();
			}
			else if (m_currentOp == AsyncOperation.Nothing)
			{
				DebugLog.Log(GetType(), "ExitRovioIdScreen, game reset NOW, " + this);
				ResetGame();
			}
			else
			{
				DebugLog.Log(GetType(), "ExitRovioIdScreen, game reset maybe later, " + this);
				m_exited = true;
			}
		}

		private void RetrieveCurrentProfile()
		{
			if (m_currentOp == AsyncOperation.Login)
			{
				m_currentOp = AsyncOperation.LoginRetrievingProfile;
			}
			else if (m_currentOp == AsyncOperation.Logout)
			{
				m_currentOp = AsyncOperation.LogoutRetrievingProfile;
			}
			DIContainerInfrastructure.GetAssetData().FetchAndResetProfileOnNextLogin = true;
			DIContainerInfrastructure.GetAssetData().Save();
			DIContainerInfrastructure.RemoteStorageService.GetPrivateProfile(ProcessRetrievedProfile, RetrieveProfileError);
			DebugLog.Log(GetType(), "RetrieveCurrentProfile, " + this);
		}

		private void RetrieveProfileError(string error)
		{
			DebugLog.Log(GetType(), "RetrieveProfileError: " + error + ", State: " + this);
			DIContainerInfrastructure.GetAsynchStatusService().ShowError(DIContainerInfrastructure.GetLocaService().Tr("toast_newer_profile", "Game update required for this profile"));
			m_currentOp = AsyncOperation.LoginErrorSignBackInAsGuest;
			DIContainerInfrastructure.IdentityService.Logout();
			DIContainerInfrastructure.IdentityService.LoginGuest();
		}

		private void ProcessRetrievedProfile(ABH.Shared.Models.PlayerData newProfile)
		{
			DIContainerInfrastructure.GetAssetData().FetchAndResetProfileOnNextLogin = false;
			DIContainerInfrastructure.GetAssetData().Save();
			if (newProfile == null)
			{
				DebugLog.Log(GetType(), "ProcessRetrievedProfile, fresh profile detected");
			}
			DIContainerInfrastructure.GetProfileMgr().CurrentProfile = newProfile;
			bool flag = DIContainerInfrastructure.IdentityService.IsGuest();
			m_dataAfter = newProfile;
			m_isDataAfterValid = true;
			DebugLog.Log(GetType(), "ProcessRetrievedProfile, newProfileIsGuest: " + flag + ", " + this);
			if (m_currentOp == AsyncOperation.LoginRetrievingProfile && !flag && m_dataAfter == null)
			{
				m_currentOp = AsyncOperation.MoveToCloudQuestion;
				MoveToCloud();
			}
			else
			{
				if (m_dataAfter != null)
				{
					DIContainerInfrastructure.ResetWithNewProfile(m_dataAfter, false);
				}
				DebugLog.Log(GetType(), "ProcessRetrievedProfile: not offering move to cloud. isGuest: " + flag + ", " + this);
			}
			FinishAsyncOp();
			if (m_exited)
			{
				DebugLog.Log(GetType(), "ProcessRetrievedProfile: rovioid screen exited, checking if game reset is necessary, " + this);
				ResetGame();
			}
		}

		private void MoveToCloud()
		{
			if (!DIContainerInfrastructure.GetAssetData().ZeroTheGuestProfileOnNextLogin)
			{
				DebugLog.Log(GetType(), "MoveToCloud (saving assetData.ZeroTheGuestProfileOnNextLogin)");
				SetZeroTheGuestProfileOnNextLogin(true);
				if (m_localGuestAcc == null)
				{
					DebugLog.Warn(GetType(), "MoveToCloud m_localGuestAcc == null, " + this);
				}
				DIContainerInfrastructure.GetProfileMgr().CurrentProfile = m_localGuestAcc;
				m_dataAfter = m_localGuestAcc;
				if (!m_enteredUsingRovioAcc)
				{
					m_dataAfter = null;
					m_isDataAfterValid = false;
				}
			}
			else
			{
				DebugLog.Warn(GetType(), "MoveToCloud: attempting to move the guest acc progress to another account, stopped. " + this);
			}
		}

		private static void SetZeroTheGuestProfileOnNextLogin(bool val)
		{
			AssetData assetData = DIContainerInfrastructure.GetAssetData();
			assetData.ZeroTheGuestProfileOnNextLogin = val;
			assetData.Save();
		}

		private void ResetGame()
		{
			if (m_isDataAfterValid)
			{
				DebugLog.Log(GetType(), "ResetGame (when the user confirms) NOW, " + this);
				m_currentOp = AsyncOperation.ResettingGame;
				if (DIContainerInfrastructure.IdentityService.IsGuest() && DIContainerInfrastructure.GetAssetData().ZeroTheGuestProfileOnNextLogin)
				{
					DebugLog.Log(GetType(), "ResetGame with empty guest account, " + this);
					SetZeroTheGuestProfileOnNextLogin(false);
					m_dataAfter = null;
				}
				else
				{
					DebugLog.Log(GetType(), "ResetGame with new profile, " + this);
				}
				DIContainerInfrastructure.ResetWithNewProfile(m_dataAfter);
				ResetAsyncOpState();
			}
			else
			{
				DebugLog.Log(GetType(), "ResetGame NOT resetting the game, " + this);
				ResetAsyncOpState();
			}
		}

		private void FinishAsyncOp()
		{
			m_currentOp = AsyncOperation.Nothing;
			HideLoadingSpinner();
		}

		private void ResetAsyncOpState()
		{
			UnregisterIdentityEvents();
			FinishAsyncOp();
			m_localGuestAcc = null;
			m_dataAfter = null;
			m_isDataAfterValid = false;
			m_exited = false;
			m_enteredUsingRovioAcc = false;
			DIContainerInfrastructure.RemoteStorageService.EnableProfileSync(GetType().Name);
			DebugLog.Log(GetType(), "ResetAsyncOpState done.");
		}

		private void BeaconIdentityLoginError(int errorcode, string message)
		{
			if (m_currentOp == AsyncOperation.Login || m_currentOp == AsyncOperation.Logout)
			{
				DebugLog.Log(GetType(), string.Concat("BeaconIdentityLoginError in state = ", this, ", reconnecting as guest. errorcode: ", errorcode, ", msg: ", message));
				m_currentOp = AsyncOperation.LoginErrorSignBackInAsGuest;
				DIContainerInfrastructure.IdentityService.Logout();
				DIContainerInfrastructure.IdentityService.LoginGuest();
			}
			else if (m_currentOp == AsyncOperation.LoginRetrievingProfile || m_currentOp == AsyncOperation.LogoutRetrievingProfile)
			{
				DebugLog.Log(GetType(), string.Concat("BeaconIdentityLoginError in state = ", this, ", trying to retrieve profile again. errorcode: ", errorcode, ", msg: ", message));
				RetrieveCurrentProfile();
			}
			else
			{
				DebugLog.Log(GetType(), "BeaconIdentityLoginError resetting async state, errorcode: " + errorcode + ", msg: " + message);
				FinishAsyncOp();
			}
			DIContainerInfrastructure.GetCoreStateMgr().BlockAllInput(false);
		}

		private void BeaconIdentityLoggedIn()
		{
			DebugLog.Log(GetType(), "BeaconIdentityLoggedIn");
			LoginCompleted();
			DIContainerInfrastructure.GetCoreStateMgr().BlockAllInput(false);
		}

		public override string ToString()
		{
			return string.Concat(m_currentOp, ", data: ", (!m_isDataAfterValid) ? "invalid" : ((m_dataAfter == null) ? "null" : ("(NameId: " + m_dataAfter.NameId + ")")), ", exited: ", m_exited, ", listening: ", m_listingForIdentityEvents, ", enteredUsingRovioAcc: ", m_enteredUsingRovioAcc);
		}
	}
}
