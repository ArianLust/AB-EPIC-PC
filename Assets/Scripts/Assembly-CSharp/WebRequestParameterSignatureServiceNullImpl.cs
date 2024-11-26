using System;
using Chimera.Library.Components.Interfaces;

public class WebRequestParameterSignatureServiceNullImpl : IHasLogger, IWebRequestParameterSignatureService
{
	public Func<string> SignatureSalt
	{
		get
		{
			return () => string.Empty;
		}
		set
		{
		}
	}

	public Action<string> Log
	{
		get
		{
			return delegate
			{
			};
		}
		set
		{
		}
	}

	public Action<string> LogError
	{
		get
		{
			return delegate
			{
			};
		}
		set
		{
		}
	}

	public string AppendSignatureParameterToUrl(string url, byte[] postData, bool includePathAsParameters = false)
	{
		return string.Empty;
	}

	public ParameterSignatureValidationResult ValidateSignature(string salt, string url, string postDataByte64, bool includePathAsParameters = false)
	{
		return ParameterSignatureValidationResult.Failed;
	}

	public ParameterSignatureValidationResult ValidateSignature(string salt, string url, byte[] postData, bool includePathAsParameters = false)
	{
		return ParameterSignatureValidationResult.Failed;
	}
}
