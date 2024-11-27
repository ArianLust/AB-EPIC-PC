using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIWidget))]
public class AnimateNGUIAlphaAnimator : MonoBehaviour
{
	private const float m_overlayOffset = 0.2f;

	public Animator m_animation;

	[SerializeField]
	public UIWidget m_widget;

	[SerializeField]
	public bool m_isOverlay;

	private float m_InitialAlpha;

	[SerializeField]
	public List<ClipAnimationCurve> m_animCurves = new List<ClipAnimationCurve>();

	private string m_ClipName;

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
			m_animation = parent.GetComponent<Animator>();
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
		if (!m_animation || m_animation.GetCurrentAnimatorClipInfo(0).Length <= 0)
		{
			return;
		}
		AnimatorClipInfo animatorClipInfo = m_animation.GetCurrentAnimatorClipInfo(0)[0];
		AnimatorStateInfo currentAnimatorStateInfo = m_animation.GetCurrentAnimatorStateInfo(0);
		m_ClipName = animatorClipInfo.clip.name;
		foreach (ClipAnimationCurve animCurf in m_animCurves)
		{
			animCurf.IsPlaying = m_ClipName == animCurf.ClipName;
			m_widget.alpha = animCurf.Curve.Evaluate((!currentAnimatorStateInfo.loop) ? (currentAnimatorStateInfo.normalizedTime * currentAnimatorStateInfo.length) : (currentAnimatorStateInfo.normalizedTime % 1f * currentAnimatorStateInfo.length));
		}
	}
}
