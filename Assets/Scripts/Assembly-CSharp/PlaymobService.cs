using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlaymobService
{
	public void TrackPurchase(bool debug)
	{
		PlaymobData playmobData = new PlaymobData(debug);
		string payloadFormData = playmobData.GetPayloadFormData();
		payloadFormData = payloadFormData.Replace(":", "%3A").Replace("+", "%2B");
		DIContainerInfrastructure.GetCoreStateMgr().StartCoroutine(PlayMobPostRequestAsync(playmobData, payloadFormData));
	}

	private IEnumerator PlayMobPostRequestAsync(PlaymobData data, string payload)
	{
		WWW request = new WWW(headers: new Dictionary<string, string> { { "Content-Type", "application/x-www-form-urlencoded" } }, url: data.PlaymobUrl, postData: Encoding.UTF8.GetBytes(payload));
		yield return request;
		if (!string.IsNullOrEmpty(request.error))
		{
			DebugLog.Error(GetType(), request.error);
		}
		else
		{
			DebugLog.Log(GetType(), "Answer from playmob: " + request.text);
		}
	}
}
