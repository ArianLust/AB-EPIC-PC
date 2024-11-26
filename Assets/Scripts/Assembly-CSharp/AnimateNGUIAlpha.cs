using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIWidget))]
public class AnimateNGUIAlpha : MonoBehaviour
{
	private const float m_overlayOffset = 0.2f;

	public Animation m_animation;

	[SerializeField]
	public UIWidget m_widget;

	[SerializeField]
	public bool m_isOverlay;

	private float m_InitialAlpha;

	[SerializeField]
	public List<ClipAnimationCurve> m_animCurves = new List<ClipAnimationCurve>();

	private void Awake()
	{
		if ((!m_widget || !m_animation) && !SetDefaultWidgetAndAnimation())
		{
			Object.Destroy(this);
		}
		if ((bool)m_widget)
		{
			m_InitialAlpha = m_widget.alpha;
		}
	}

	public bool SetDefaultWidgetAndAnimation()
	{
		m_widget = GetComponent<UIWidget>();
		Transform parent = base.transform;
		while (parent != null)
		{
			if (parent.GetComponent<Animation>() == null)
			{
				parent = parent.parent;
				continue;
			}
			m_animation = parent.GetComponent<Animation>();
			break;
		}
		return (bool)m_widget && (bool)m_animation;
	}

	private void OnEnable()
	{
		LateUpdate();
	}

	private void OnDisable()
	{
		LateUpdate();
	}

	private void OnDestroy()
	{
		LateUpdate();
	}

	private void LateUpdate()
	{
	}
}
