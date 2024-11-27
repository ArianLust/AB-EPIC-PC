using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

public class AsyncWebRequestMB : MonoBehaviour, IAsyncWebRequest
{
	private int m_requestTimeoutSeconds = 30;

	private DateTime m_timeoutCounter;

	private bool m_requestIsRunning;

	private bool m_useTimeout = true;

	private Dictionary<string, string> m_headers;

	private ThreadPriority m_threadPriority = UnityEngine.ThreadPriority.Normal;

	private Action<string> m_logDebug;

	private Action<string> m_logError;

	public int RequestTimeoutSeconds
	{
		get
		{
			return m_requestTimeoutSeconds;
		}
		set
		{
			m_requestTimeoutSeconds = value;
		}
	}

	public Action<string> LogDebug
	{
		get
		{
			return m_logDebug ?? ((Action<string>)delegate(string s)
			{
				Debug.Log(string.Format("[{0}] {1}", GetType(), s));
			});
		}
		set
		{
			m_logDebug = value;
		}
	}

	public Action<string> LogError
	{
		get
		{
			return m_logError ?? ((Action<string>)delegate(string s)
			{
				Debug.LogError(string.Format("[{0}] {1}", GetType(), s));
			});
		}
		set
		{
			m_logError = value;
		}
	}

	public string ThreadPriority
	{
		get
		{
			return m_threadPriority.ToString();
		}
		set
		{
			try
			{
				m_threadPriority = (ThreadPriority)(int)Enum.Parse(typeof(ThreadPriority), value);
			}
			catch
			{
			}
		}
	}

	[method: MethodImpl(32)]
	public event Action<string, int> OnRequestTimeout;

	public string GetHeaderValue(string key)
	{
		string value = null;
		m_headers.TryGetValue(key, out value);
		return value;
	}

	public void AddHeaders(Dictionary<string, string> headers)
	{
		foreach (KeyValuePair<string, string> header in headers)
		{
			SetHeader(header.Key, header.Value);
		}
	}

	public void DeleteHeader(string type)
	{
		m_headers.Remove(type);
	}

	public void SetHeader(string type, string value)
	{
		if (m_headers == null)
		{
			m_headers = new Dictionary<string, string>();
		}
		if (m_headers.ContainsKey(type))
		{
			m_headers[type] = value;
		}
		else
		{
			m_headers.Add(type, value);
		}
	}

	public void LoadUrl(Action<IAsyncResult, byte[], string, Dictionary<string, string>> callback, IAsyncResult ar, string url, byte[] postData = null)
	{
		StartCoroutine(_LoadUrl(callback, ar, url, postData));
	}

	public void LoadUrlWithCustomHeaders(Action<IAsyncResult, byte[], string, Dictionary<string, string>> callback, IAsyncResult ar, string url, Dictionary<string, string> useOnlyTheseHeaders, byte[] postData = null)
	{
		Dictionary<string, string> headers = m_headers;
		m_headers = useOnlyTheseHeaders;
		LoadUrl(callback, ar, url, postData);
		m_headers = headers;
	}

	private WWW GetWWW(string url, byte[] postData, Dictionary<string, string> headers)
	{
		return new WWW(url, postData, headers);
	}

	private void SetThreadPriority(WWW www)
	{
		www.threadPriority = m_threadPriority;
	}

	private IEnumerator _LoadUrl(Action<IAsyncResult, byte[], string, Dictionary<string, string>> callback, IAsyncResult ar, string url, byte[] postData)
	{
		WWW www = GetWWW(url, postData, m_headers);
		SetThreadPriority(www);
		using (www)
		{
			if (!m_requestIsRunning)
			{
				m_timeoutCounter = DateTime.Now;
			}
			double timeGoneBy = (DateTime.Now - m_timeoutCounter).TotalSeconds;
			if (m_useTimeout && timeGoneBy > (double)RequestTimeoutSeconds && !www.isDone)
			{
				LogDebug("Request timeout to " + url + ": took " + timeGoneBy + " seconds.");
				if (this.OnRequestTimeout != null)
				{
					this.OnRequestTimeout(url, (int)timeGoneBy);
				}
				yield break;
			}
			m_requestIsRunning = true;
			yield return www;
			m_requestIsRunning = false;
			if (!string.IsNullOrEmpty(www.error))
			{
				string errorText = ((!string.IsNullOrEmpty(www.text)) ? www.text : ((www.bytes == null || www.bytes.Length <= 0) ? "(nothing)" : Encoding.UTF8.GetString(www.bytes)));
				LogError("Web request error calling " + url + ": " + www.error + ", text: " + errorText + ", postData: " + ((postData == null) ? "null" : Encoding.UTF8.GetString(postData)) + " --- headers: " + m_headers.Aggregate(string.Empty, (string acc, KeyValuePair<string, string> curr) => acc + curr.Key + " -> " + curr.Value.Substring(0, Math.Min(15, curr.Value.Length)) + "...,"));
			}
			else
			{
				string response = ((!string.IsNullOrEmpty(www.text)) ? www.text : ((www.bytes == null || www.bytes.Length <= 0) ? "(nothing)" : Encoding.UTF8.GetString(www.bytes)));
				LogDebug("[AsyncWebRequestMB] Response for " + url + " received: " + response);
			}
			if (callback != null)
			{
				callback(ar, www.bytes, www.error, www.responseHeaders);
			}
		}
	}
}
