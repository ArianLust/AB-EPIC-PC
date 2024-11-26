using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIPanel))]
public class AnimateNGUIPanelAlpha : MonoBehaviour
{
	public Animation m_animation;

	[SerializeField]
	public UIPanel m_panel;

	[SerializeField]
	public List<ClipAnimationCurve> m_animCurves = new List<ClipAnimationCurve>();

	private void Update()
	{
	}
}
