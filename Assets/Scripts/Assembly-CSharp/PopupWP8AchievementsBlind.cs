using System.Collections;
using UnityEngine;

internal class PopupWP8AchievementsBlind : MonoBehaviour
{
	[SerializeField]
	private UILabel m_name;

	[SerializeField]
	private UILabel m_description;

	[SerializeField]
	private UILabel m_gamerscore;

	[SerializeField]
	private UITexture m_completed;

	private IEnumerator LoadIcon(string iconPath)
	{
		DebugLog.Log(GetType(), "Loading " + iconPath);
		WWW www = new WWW(iconPath);
		yield return www;
		DebugLog.Log(GetType(), "Loaded " + iconPath);
		if (!string.IsNullOrEmpty(www.error))
		{
			DebugLog.Error(GetType(), "Error loading " + iconPath + ": " + www.error);
			yield break;
		}
		if (www.texture == null)
		{
			DebugLog.Error(GetType(), "No texture loaded for " + iconPath);
			yield break;
		}
		m_completed.mainTexture = www.texture;
		DebugLog.Log(GetType(), "Set the texture for " + iconPath);
	}
}
