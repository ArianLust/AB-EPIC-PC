using System.Collections.Generic;
using ABH.Shared.Generic;
using UnityEngine;

public class BattleFXController : MonoBehaviour
{
	public float m_DestroyTime;

	public Vector3 LineRendererOffset = new Vector3(0f, -10f, 0f);

	public LineRenderer m_LineRenderer;

	public UILabel m_Text;

	private float scaleY = 1f;

	private Transform m_ReferencedTransform;

	[SerializeField]
	private Transform m_ToSourceTransform;

	private Transform m_CachedFrom;

	private Transform m_CachedTo;

	public bool m_DisableScale;

	public List<Transform> m_IgnoreScaleTransforms = new List<Transform>();

	private bool m_UpdateLineRenderer;

	public void SetText(string text)
	{
		if ((bool)m_Text)
		{
			m_Text.text = text;
		}
	}

	public void SetSize(CharacterSizeType sizeType, float characterScale)
	{
		if (m_DisableScale)
		{
			return;
		}
		float num = 1f;
		switch (sizeType)
		{
		case CharacterSizeType.Boss:
			num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorBoss;
			break;
		case CharacterSizeType.Large:
			num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorLarge;
			break;
		case CharacterSizeType.Medium:
			num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorMedium;
			break;
		case CharacterSizeType.Small:
			num = DIContainerLogic.GetVisualEffectsBalancing().EffectSizeFactorSmall;
			break;
		}
		base.transform.localScale = base.transform.localScale * num / characterScale;
		foreach (Transform ignoreScaleTransform in m_IgnoreScaleTransforms)
		{
			ignoreScaleTransform.localScale /= num / characterScale;
		}
	}

	private void Start()
	{
		if (m_DestroyTime > 0f)
		{
			Object.Destroy(base.gameObject, m_DestroyTime);
		}
	}

	public void SendParameter(string parameter)
	{
		SendMessage("SetEffectParameter", parameter, SendMessageOptions.DontRequireReceiver);
	}

	private void OnDisable()
	{
		Object.Destroy(base.gameObject);
	}

	public void SetLineRenderer(Transform from, Transform to)
	{
		if ((bool)m_LineRenderer)
		{
			m_CachedFrom = from;
			m_CachedTo = to;
			m_LineRenderer.SetPosition(0, from.position + LineRendererOffset);
			m_LineRenderer.SetPosition(1, to.position + LineRendererOffset);
			m_UpdateLineRenderer = true;
		}
	}

	private void LateUpdate()
	{
		if (!m_UpdateLineRenderer)
		{
			return;
		}
		if (!m_CachedFrom || !m_CachedTo)
		{
			CancelInvoke("UpdateLineRenderer");
			Object.Destroy(m_LineRenderer);
			return;
		}
		if ((bool)m_ToSourceTransform)
		{
			m_ToSourceTransform.position = m_CachedFrom.position;
		}
		m_LineRenderer.SetPosition(0, m_CachedFrom.position + LineRendererOffset);
		m_LineRenderer.SetPosition(1, m_CachedTo.position + LineRendererOffset);
	}
}
