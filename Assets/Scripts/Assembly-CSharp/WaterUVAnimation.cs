using UnityEngine;

public class WaterUVAnimation : MonoBehaviour
{
	public string m_TextureName = "_MainTex";

	public float m_XSpeed;

	private Material m_cachedMaterial;

	private void Start()
	{
		m_cachedMaterial = GetComponent<Renderer>().sharedMaterial;
	}

	private void Update()
	{
		Vector2 textureOffset = m_cachedMaterial.GetTextureOffset(m_TextureName);
		textureOffset.x += m_XSpeed * Time.deltaTime;
		m_cachedMaterial.SetTextureOffset(m_TextureName, new Vector2(textureOffset.x % 1f, textureOffset.y % 1f));
	}
}
