using UnityEngine;

public class CameraResolutionMgr : MonoBehaviour
{
	public enum CameraTypes
	{
		Scenery = 0,
		UI = 1,
		FullscreenUI = 2
	}

	[SerializeField]
	private CameraTypes m_CameraType;

	public void Start()
	{
		AutoAdjustCamera();
	}

	public void AutoAdjustCamera()
	{
		switch (m_CameraType)
		{
		case CameraTypes.Scenery:
			AdjustSceneryCam();
			break;
		case CameraTypes.UI:
			AdjustUICam();
			break;
		case CameraTypes.FullscreenUI:
			AdjustFullscreenUICam();
			break;
		}
	}

	private void AdjustUICam()
	{
		float num = 1280f / ((Screen.width == 0) ? 1f : ((float)Screen.width)) * (float)Screen.height / 2f;
		base.GetComponent<Camera>().orthographicSize = Mathf.Max(num, 360f);
	}

	private void AdjustSceneryCam()
	{
		Camera component = GetComponent<Camera>();
		if (component == null)
		{
			Debug.LogError("Error adjusting CameraResolution: No Camera found on Component");
		}
		if (component.aspect < 1.7777779f)
		{
			component.orthographicSize = 384f;
		}
		else
		{
			component.orthographicSize = 360f;
		}
	}

	private void AdjustFullscreenUICam()
	{
		Camera component = GetComponent<Camera>();
		if (component == null)
		{
			Debug.LogError("Error adjusting CameraResolution: No Camera found on Component");
			return;
		}
		float value = Screen.height / 2;
		float max = 384f;
		float min = 320f;
		if (component.aspect >= 1.7777778f)
		{
			max = 360f;
		}
		component.orthographicSize = Mathf.Clamp(value, min, max);
	}
}
