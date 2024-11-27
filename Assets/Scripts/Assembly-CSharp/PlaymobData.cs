using System;
using System.Collections.Generic;
using System.Text;
using Chimera.Library.Components.Services;

public class PlaymobData
{
	private bool m_debug;

	private int m_campaignId
	{
		get
		{
			return 172;
		}
	}

	public string PlaymobUrl
	{
		get
		{
			return "https://api.playmob.com/api/v3/campaign_items/" + m_campaignId + "/purchases.json";
		}
	}

	private string m_apiKey
	{
		get
		{
			return "42d94dcc9daab26a0b9c23e81fe9cbfd";
		}
	}

	private string m_apiAccessId
	{
		get
		{
			return "44cd88ded836ea9c136ef285f5ac8ab7";
		}
	}

	public PlaymobData(bool debug)
	{
		m_debug = debug;
	}

	private Dictionary<string, object> ConvertInfo()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("api_access_id", m_apiAccessId);
		dictionary.Add("campaign_item_id", m_campaignId);
		dictionary.Add("nonce", GetNonce());
		dictionary.Add("time", DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:sszzz"));
		dictionary.Add("platform_identifier", GetPlatformID());
		dictionary.Add("test", m_debug.ToString().ToLower());
		dictionary.Add("app_store_transaction_details", "{}");
		dictionary.Add("google_play_signature", "{}");
		return dictionary;
	}

	public string GetPayloadJson()
	{
		Dictionary<string, object> dictionary = ConvertInfo();
		dictionary.Add("signature", SortAndGetSHA1(dictionary));
		return string.Empty;
	}

	public string GetPayloadFormData()
	{
		Dictionary<string, object> dictionary = ConvertInfo();
		dictionary.Add("signature", SortAndGetSHA1(dictionary));
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			stringBuilder.Append(item.Key).Append("=").Append(item.Value)
				.Append("&");
		}
		return stringBuilder.ToString().Substring(0, stringBuilder.Length - 1);
	}

	private string GetNonce()
	{
		return Guid.NewGuid().ToString();
	}

	private string GetPlatformID()
	{
		return "android";
	}

	public string SortAndGetSHA1(Dictionary<string, object> data)
	{
		SortedDictionary<string, object> sortedDictionary = new SortedDictionary<string, object>(data);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, object> item in sortedDictionary)
		{
			stringBuilder.Append(item.Key);
			stringBuilder.Append(item.Value);
		}
		stringBuilder.Append(m_apiKey);
		return new HashEmbeddedImpl().HashSha1(stringBuilder.ToString());
	}
}
