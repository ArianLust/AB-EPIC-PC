using UnityEngine;

public class CloudUvAnimation : MonoBehaviour
{
	public string m_TextureName = "_MainTex";

	public float m_XSpeed;

	public float m_YMin;

	public float m_YMax;

	public float m_YPeriod = 1f;

	private Material m_cachedMaterial;

	private float m_YSinusCenter;

	private float m_YSinusAmplitude;

	private void Start()
	{
		m_cachedMaterial = GetComponent<Renderer>().sharedMaterial;
		m_YSinusCenter = (m_YMax + m_YMin) / 2f;
		m_YSinusAmplitude = m_YMax - m_YSinusCenter;
	}

	private void Update()
	{
		Vector2 vector = new Vector2(m_cachedMaterial.GetFloat("_OffsetX"), m_cachedMaterial.GetFloat("_OffsetY"));
		vector.x += m_XSpeed * Time.deltaTime;
		vector.y = Mathf.Sin(Time.time * m_YPeriod) * m_YSinusAmplitude + m_YSinusCenter;
		m_cachedMaterial.SetFloat("_OffsetX", Mathf.Repeat(vector.x, 1f));
		m_cachedMaterial.SetFloat("_OffsetY", Mathf.Repeat(vector.y, 1f));
	}
}
