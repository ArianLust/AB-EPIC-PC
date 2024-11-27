using System.Collections.Generic;
using UnityEngine;

public class BannerPartAssetController : MonoBehaviour
{
	[SerializeField]
	private Vector3 m_LootPlacementPosition = Vector3.zero;

	[SerializeField]
	private Vector3 m_LootPlacementScale = Vector3.one;

	public void ApplyLootTransformations()
	{
		base.transform.localPosition = m_LootPlacementPosition;
		base.transform.localScale = m_LootPlacementScale;
	}

	public virtual void SetColors(Color color)
	{
	}

	public Color GetColorFromList(List<float> list)
	{
		Color white = Color.white;
		for (int i = 0; i < list.Count; i++)
		{
			float num = list[i];
			switch (i)
			{
			case 0:
				white.r = num;
				break;
			case 1:
				white.g = num;
				break;
			case 2:
				white.b = num;
				break;
			case 3:
				white.a = num;
				break;
			}
		}
		return white;
	}
}
