using System.Collections.Generic;
using UnityEngine;

public class BannerFlagAssetController : BannerPartAssetController
{
	[SerializeField]
	public Transform m_BannerEmblemRoot;

	[SerializeField]
	public string m_BannerBaseAssetName;

	[SerializeField]
	public List<GameObject> m_ColorableObjects = new List<GameObject>();

	public override void SetColors(Color color)
	{
		foreach (GameObject colorableObject in m_ColorableObjects)
		{
			UISprite[] components = colorableObject.GetComponents<UISprite>();
			foreach (UISprite uISprite in components)
			{
				uISprite.color = color;
			}
			CHMeshSprite[] components2 = colorableObject.GetComponents<CHMeshSprite>();
			foreach (CHMeshSprite cHMeshSprite in components2)
			{
				cHMeshSprite.m_Color = color;
			}
			Renderer[] components3 = colorableObject.GetComponents<Renderer>();
			foreach (Renderer renderer in components3)
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
