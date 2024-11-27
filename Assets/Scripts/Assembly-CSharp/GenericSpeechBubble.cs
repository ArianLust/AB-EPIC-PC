using UnityEngine;

public class GenericSpeechBubble : MonoBehaviour
{
	public UISprite m_BubbleIcon;

	public float m_DefaultDestroyTime = 3f;

	public void SetModel(string assetId, UIAtlas replacementAtlas = null)
	{
		if ((bool)replacementAtlas)
		{
			m_BubbleIcon.atlas = replacementAtlas;
		}
		m_BubbleIcon.spriteName = assetId;
		if (GetComponent<Animation>()["Bubble_PlayEmotion"] != null)
		{
			GetComponent<Animation>().Play("Bubble_PlayEmotion");
			Object.Destroy(base.gameObject, GetComponent<Animation>()["Bubble_PlayEmotion"].length);
		}
		else
		{
			Object.Destroy(base.gameObject, m_DefaultDestroyTime);
		}
	}
}
