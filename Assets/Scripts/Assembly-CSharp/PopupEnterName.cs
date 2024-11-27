using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupEnterName : MonoBehaviour
{
	public enum NicknameAllowed
	{
		Allowed = 0,
		NotAllowed = 1,
		NicknameCheckFailure = 2
	}

	[SerializeField]
	private UIInputTrigger m_OkButton;

	[SerializeField]
	private UIInputTrigger m_CancelButton;

	[SerializeField]
	private UILabel m_OpponentNameLabel;

	private string m_enteredNickname;

	internal string m_TranslatedNickname;

	public bool m_ValidateNicknameProgress;

	private string m_TranslatingNicknameErrorMessage;

	private void Awake()
	{
		base.gameObject.SetActive(false);
		base.transform.parent = DIContainerInfrastructure.GetCoreStateMgr().m_GenericInterfaceRoot;
		DIContainerInfrastructure.GetCoreStateMgr().m_EnterNamePopup = this;
	}

	public void ShowEnterNamePopup()
	{
		base.gameObject.SetActive(true);
		StartCoroutine("EnterCoroutine");
	}

	private IEnumerator EnterCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(true);
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_enter_name_enter");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.RegisterBar(new BarRegistry
		{
			Depth = 6u,
			showSnoutlings = false
		}, true);
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_UseVoucherCode_Enter"));
		RegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_enter_name_enter");
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterAction(5, CancelButtonClicked);
		m_OkButton.Clicked += OkButtonClicked;
		m_CancelButton.Clicked += CancelButtonClicked;
	}

	private void DeRegisterEventHandlers()
	{
		DIContainerInfrastructure.BackButtonMgr.DeRegisterAction(5);
		m_OkButton.Clicked -= OkButtonClicked;
		m_CancelButton.Clicked -= CancelButtonClicked;
	}

	public void OnSubmit(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			m_enteredNickname = value;
			DIContainerInfrastructure.IdentityService.ValidateNickname(value, CheckBlackListSuccess, CheckBlackListFailed);
		}
	}

	private void CheckBlackListSuccess(bool success, string message)
	{
		if (ContainsSymbol(m_enteredNickname))
		{
			CheckBlackListFailed((message != "NICKNAME_OK") ? message : "NICKNAME_BLACKLISTED");
			return;
		}
		StartCoroutine(TranslateNickname(m_enteredNickname, delegate
		{
			NicknameAllowed nicknameAllowed = IsNicknameAllowed(m_enteredNickname);
			if (success && nicknameAllowed == NicknameAllowed.Allowed)
			{
				m_OpponentNameLabel.text = DIContainerInfrastructure.GetLocaService().ReplaceUnmappableCharacters(m_enteredNickname);
			}
			else if (success && nicknameAllowed == NicknameAllowed.NicknameCheckFailure)
			{
				StartCoroutine(DelayedResetName());
			}
			else
			{
				CheckBlackListFailed((message != "NICKNAME_OK") ? message : "NICKNAME_BLACKLISTED");
			}
		}));
	}

	private void CheckBlackListFailed(string message)
	{
		DIContainerInfrastructure.GetAsynchStatusService().ShowInfo(DIContainerInfrastructure.GetLocaService().Tr("toast_playername_blacklist"), "blacklistfail", DispatchMessage.Status.Error);
		if (!string.IsNullOrEmpty(message))
		{
			DebugLog.Warn("Invalid nickname entered: " + message);
		}
		StartCoroutine(DelayedResetName());
	}

	private IEnumerator DelayedResetName()
	{
		yield return new WaitForEndOfFrame();
		string realName = DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.EventPlayerName;
		m_enteredNickname = DIContainerInfrastructure.GetLocaService().ReplaceUnmappableCharacters(realName);
		m_OpponentNameLabel.text = m_enteredNickname;
	}

	private void OkButtonClicked()
	{
		SaveNewlyEnteredName();
		StartCoroutine(LeaveCoroutine());
	}

	private void SaveNewlyEnteredName()
	{
		DIContainerInfrastructure.GetCurrentPlayer().SocialEnvironmentGameData.Data.EventPlayerName = m_OpponentNameLabel.text;
		DebugLog.Log(GetType(), "saving the name entered: " + m_OpponentNameLabel.text);
		DIContainerInfrastructure.GetCurrentPlayer().SavePlayerData();
	}

	private void CancelButtonClicked()
	{
		StartCoroutine(LeaveCoroutine());
	}

	private IEnumerator LeaveCoroutine()
	{
		DIContainerInfrastructure.GetCoreStateMgr().RegisterPopupEntered(false);
		DeRegisterEventHandlers();
		DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("popup_enter_name_enter");
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.DeRegisterBar(6u);
		DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateAllBars();
		yield return new WaitForSeconds(base.gameObject.PlayAnimationOrAnimatorState("Popup_UseVoucherCode_Leave"));
		DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("popup_enter_name_enter");
		base.gameObject.SetActive(false);
	}

	internal IEnumerator TranslateNickname(string nickname, Action onFinishCallback)
	{
		if (string.IsNullOrEmpty(nickname))
		{
			Debug.LogError("[PopupEnterName] Nickname is not yet set");
			yield break;
		}
		WWW www = new WWW("https://translate.google.com/translate_a/single?dj=1&q=" + nickname + "&sl=auto&tl=en&hl=id-ID&ie=UTF-8&oe=UTF-8&client=at&dt=t&dt=ld&dt=qca&dt=rm&dt=bd&dt=md&dt=ss&dt=ex&source=t2t_rd&otf=3", null, new Dictionary<string, string> { { "User-Agent", "GoogleTranslate/6.3.0.RC06.277163268" } });
		yield return www;
		if (www.isDone && string.IsNullOrEmpty(www.error))
		{
			if (string.IsNullOrEmpty(www.text))
			{
				Debug.LogError("[PopupEnterName] Text is empty, quitting translate nickname");
				onFinishCallback();
				yield break;
			}
			if (!www.text.Contains("sentences") && !www.text.Contains("src"))
			{
				Debug.LogError("[PopupEnterName] Unsupported response code: " + www.text);
				StartCoroutine(ContentLoader.Instance.SubmitErrorReport("Unsupported response code: " + www.text));
				onFinishCallback();
				yield break;
			}
			m_TranslatedNickname = ProcessTranslatedNickname(www.text, nickname);
		}
		else if (www.isDone && !string.IsNullOrEmpty(www.error))
		{
			string text = (string.IsNullOrEmpty(www.text) ? null : ("\r\nError source: " + www.text));
			StartCoroutine(ContentLoader.Instance.SubmitErrorReport("Error msg: " + www.error + text));
			m_TranslatingNicknameErrorMessage = www.error;
			Debug.LogError("[PopupEnterName] Translating nickname failed: " + www.error);
		}
		onFinishCallback();
	}

	private NicknameAllowed IsNicknameAllowed(string nickname)
	{
		if (string.IsNullOrEmpty(m_TranslatedNickname) && m_TranslatingNicknameErrorMessage != "429: Too Many Requests")
		{
			return NicknameAllowed.NicknameCheckFailure;
		}
		string text = ((m_TranslatingNicknameErrorMessage != "429: Too Many Requests") ? m_TranslatedNickname.ToLower() : nickname.ToLower());
		if (text.Contains("boob") || text.Contains("pussy") || text.Contains("vagina") || text.Contains("dick") || text.Contains("arse") || text.Contains("sexy") || CheckWithSymbolOrSpaceFilter(text, "poo") || text.Contains("fard") || text.Contains("fart") || CheckWithSymbolOrSpaceFilter(text, "pee") || text.Contains("porn") || ContainsSymbol(text) || CheckWithSymbolOrSpaceFilter(text, "butt") || text.Contains("xxx") || text.Contains("penis") || text.Contains("poop") || text.Contains("shart") || CheckWithSymbolOrSpaceFilter(text, "bra") || text.Contains("bralette") || text.Contains("bikini"))
		{
			return NicknameAllowed.NotAllowed;
		}
		return NicknameAllowed.Allowed;
	}

	private bool CheckWithSymbolOrSpaceFilter(string str, string value)
	{
		if (!(str == value) && !str.Contains(" " + value) && !str.Contains(value + " ") && !str.Contains("-" + value) && !str.Contains(value + "-") && !str.Contains("_" + value) && !str.Contains(value + "_") && !str.Contains("+" + value) && !str.Contains(value + "+") && !str.Contains("," + value) && !str.Contains(value + ",") && !str.Contains("." + value) && !str.Contains(value + ".") && !str.Contains(";" + value) && !str.Contains(value + ";") && !str.Contains("'" + value) && !str.Contains(value + "'") && !str.Contains("\"" + value) && !str.Contains(value + "\"") && !str.Contains("/" + value) && !str.Contains(value + "/") && !str.Contains("(" + value + ")") && !str.Contains("[" + value + "]"))
		{
			return str.Contains("{" + value + "}");
		}
		return true;
	}

	internal bool ForceValidateNickname(string nickname, Action onSuccessCallback, Action<string> onErrorCallback)
	{
		if (string.IsNullOrEmpty(nickname))
		{
			return true;
		}
		m_ValidateNicknameProgress = true;
		bool result = false;
		DIContainerInfrastructure.IdentityService.ValidateNickname(nickname, delegate(bool success, string message)
		{
			if (success && IsNicknameAllowed(nickname) == NicknameAllowed.Allowed)
			{
				m_ValidateNicknameProgress = false;
				if (onSuccessCallback != null)
				{
					onSuccessCallback();
				}
				result = true;
			}
			else
			{
				m_ValidateNicknameProgress = false;
				if (onErrorCallback != null)
				{
					onErrorCallback(message);
				}
				result = false;
			}
		}, delegate(string message)
		{
			m_ValidateNicknameProgress = false;
			if (onErrorCallback != null)
			{
				onErrorCallback(message);
			}
			result = false;
		});
		return result;
	}

	private string ProcessTranslatedNickname(string googleTranslateJsonResponse, string nicknameFallback)
	{
		string result = null;
		foreach (KeyValuePair<string, object> item in SimpleJsonConverter.DecodeJsonDict(googleTranslateJsonResponse))
		{
			if (item.Key.ToLower().Equals("sentences"))
			{
				foreach (object item2 in item.Value as IEnumerable)
				{
					Dictionary<string, object> dictionary = item2 as Dictionary<string, object>;
					if (dictionary.ContainsKey("trans"))
					{
						result = dictionary["trans"].ToString();
					}
				}
			}
			if (item.Key.ToLower().Equals("src"))
			{
				if (item.Value.ToString() == "en")
				{
					result = nicknameFallback;
				}
				break;
			}
		}
		return result;
	}

	internal bool ContainsSymbol(string nickname)
	{
		if (!nickname.Contains("@") && !nickname.Contains("#") && !nickname.Contains("%") && !nickname.Contains("^") && !nickname.Contains("&"))
		{
			return nickname.Contains("*");
		}
		return true;
	}
}
