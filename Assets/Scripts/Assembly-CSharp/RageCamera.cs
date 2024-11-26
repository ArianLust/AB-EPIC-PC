using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class RageCamera : MonoBehaviour
{
	[SerializeField]
	private Camera _camera;

	private bool _started;

	public void OnEnable()
	{
		if (!_started)
		{
			_camera = GetComponent<Camera>();
			_camera.transparencySortMode = TransparencySortMode.Orthographic;
			_started = true;
		}
	}
}
