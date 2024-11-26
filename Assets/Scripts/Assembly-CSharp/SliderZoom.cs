using UnityEngine;

public class SliderZoom : MonoBehaviour
{
	[SerializeField]
	private int m_minSize;

	[SerializeField]
	private int m_maxSize;

	[SerializeField]
	private UISlider m_slider;

	[SerializeField]
	private Camera m_cam;

	public void Awake()
	{
		if (m_slider == null)
		{
			Object.Destroy(this);
		}
	}

	public void OnSliderChange()
	{
		m_cam.orthographicSize = (float)(m_maxSize - m_minSize) * m_slider.sliderValue + (float)m_minSize;
	}
}
