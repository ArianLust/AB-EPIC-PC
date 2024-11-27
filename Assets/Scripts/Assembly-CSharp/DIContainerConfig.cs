using System.Runtime.InteropServices;
using System.Text;
using ABH.Shared.BalancingData;
using UnityEngine;

public class DIContainerConfig
{
	private static string m_key;

	private static ClientConfigBalancingData m_clientConfigBalancingData;

	private static ConstantsConfig m_constants;

	public static string Key
	{
		get
		{
			if (string.IsNullOrEmpty(m_key))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				PlayDoom(stringBuilder, stringBuilder.Capacity);
				m_key = stringBuilder.ToString();
			}
			return m_key;
		}
	}

	[DllImport("chimera")]
	public static extern void PlayDoom(StringBuilder sb, int len);

	public static ConstantsConfig GetConstants()
	{
		return m_constants ?? (m_constants = new ConstantsConfig());
	}

	public static string GetAppDisplayName()
	{
		return "{AppDisplayName}";
	}

	public static string GetSkuName()
	{
		return "GooglePlaystore";
	}

	public static ClientConfigBalancingData GetClientConfig()
	{
		if (m_clientConfigBalancingData == null)
		{
			m_clientConfigBalancingData = DIContainerBalancing.Service.GetBalancingData<ClientConfigBalancingData>("rovio");
		}
		return m_clientConfigBalancingData;
	}

	private static string GetTextFromResourceFile(string resourcePath)
	{
		TextAsset textAsset = null;
		textAsset = Resources.Load(resourcePath) as TextAsset;
		if (textAsset != null)
		{
			return textAsset.text.Trim();
		}
		DebugLog.Error(typeof(DIContainerBalancing), resourcePath + " not found!!");
		return string.Empty;
	}

	internal static string GetGooglePlayKey()
	{
		string empty = string.Empty;
		return GetTextFromResourceFile("Config/googleplay_key_rovio");
	}

	internal static bool GetSkynestClientIdAndSecret(out string clientId, out string clientSecret)
	{
		string empty = string.Empty;
		empty = GetTextFromResourceFile("Config/skynest_id_secret_rovio");
		if (string.IsNullOrEmpty(empty))
		{
			clientId = (clientSecret = string.Empty);
			return false;
		}
		string[] array = empty.Split(':');
		clientId = array[0];
		clientSecret = ((empty.Length <= 0) ? string.Empty : array[1]);
		return true;
	}
}
