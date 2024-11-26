using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UIDragTrigger : MonoBehaviour
{
	private bool m_wp8_pressState;

	[method: MethodImpl(32)]
	public event Action<GameObject, Vector2> onDrag;

	[method: MethodImpl(32)]
	public event Action<GameObject, GameObject> onDrop;

	[method: MethodImpl(32)]
	public event Action<GameObject> onRelease;

	[method: MethodImpl(32)]
	public event Action<GameObject> onPress;

	public void OnPress(bool pressed)
	{
		if (!pressed && this.onRelease != null)
		{
			this.onRelease(base.gameObject);
		}
		if (pressed && this.onPress != null)
		{
			this.onPress(base.gameObject);
		}
	}

	public void OnDrag(Vector2 delta)
	{
		if (this.onDrag != null)
		{
			this.onDrag(base.gameObject, delta);
		}
	}

	public void OnDrop(GameObject go)
	{
		if (this.onDrop != null)
		{
			this.onDrop(base.gameObject, go);
		}
	}
}
