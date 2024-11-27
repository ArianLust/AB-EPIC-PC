using UnityEngine;

[RequireComponent(typeof(UIWidget))]
public class ColorWrapper : MonoBehaviour
{
	public Color color = Color.white;

	private UIWidget mWidget;

	private void Awake()
	{
		mWidget = GetComponent<UIWidget>();
	}

	private void Update()
	{
		mWidget.color = color;
	}
}
