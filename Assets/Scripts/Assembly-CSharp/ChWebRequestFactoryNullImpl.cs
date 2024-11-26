using System;
using System.Collections.Generic;

internal class ChWebRequestFactoryNullImpl : IChWebRequestFactory
{
	private readonly IChLogger m_logger;

	public ChWebRequestFactoryNullImpl(IChLogger logger)
	{
		if (logger == null)
		{
			throw new ArgumentException("logger");
		}
		m_logger = logger;
	}

	public IChWebRequest Create(string url, string method, Dictionary<string, string> headers, byte[] postData, Action<IChWebRequest> callbackMayBeNull)
	{
		m_logger.Warn(GetType(), "ChWebRequestFactoryNullImpl returning a null impl web request object");
		return new ChWebRequestNullImpl(url, method, headers, postData, callbackMayBeNull, m_logger);
	}
}
