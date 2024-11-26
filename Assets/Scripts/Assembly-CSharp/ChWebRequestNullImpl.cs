using System;
using System.Collections.Generic;

internal class ChWebRequestNullImpl : IChWebRequest
{
	private readonly IChLogger m_logger;

	private readonly Action<IChWebRequest> m_callback;

	public string Url { get; set; }

	public string Method { get; set; }

	public Dictionary<string, string> Headers { get; private set; }

	public byte[] PostData { get; set; }

	public IChWebResponse Response { get; private set; }

	public ChWebRequestNullImpl(string url, string method, Dictionary<string, string> headers, byte[] postData, Action<IChWebRequest> onDone, IChLogger logger)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new ArgumentException("URL");
		}
		if (logger == null)
		{
			throw new ArgumentException("logger");
		}
		m_logger = logger;
		m_callback = onDone ?? ((Action<IChWebRequest>)delegate
		{
		});
		Url = url;
		Method = method;
		PostData = postData;
		Headers = headers ?? new Dictionary<string, string>();
	}

	public IChWebRequest Start()
	{
		m_logger.Warn(GetType(), "Web Request Null Impl started. Will return null.");
		m_callback(null);
		return this;
	}
}
