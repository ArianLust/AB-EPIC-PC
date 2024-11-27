using UnityEngine;

public class BattleFXSpriteChange : MonoBehaviour
{
	public UISprite m_Sprite;

	public void SetEffectParameter(string parameter)
	{
		m_Sprite.spriteName = parameter;
	}
}
