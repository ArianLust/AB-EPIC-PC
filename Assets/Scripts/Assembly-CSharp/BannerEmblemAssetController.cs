using System.Collections.Generic;
using UnityEngine;

public class BannerEmblemAssetController : BannerPartAssetController
{
	[SerializeField]
	public List<GameObject> m_ColorableObjects = new List<GameObject>();

	public override void SetColors(Color color)
	{
		foreach (GameObject colorableObject in m_ColorableObjects)
		{
			UISprite[] componentsInChildren = colorableObject.GetComponentsInChildren<UISprite>();
			foreach (UISprite uISprite in componentsInChildren)
			{
				uISprite.color = color;
			}
			Renderer[] componentsInChildren2 = colorableObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren2)
			{
				Material[] materials = renderer.materials;
				foreach (Material material in materials)
				{
					material.color = color;
				}
			}
		}
	}
}
