using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiSortBehaviour : MonoBehaviour
{
	private enum OrderPosition
	{
		BelowWidget = -1,
		AboveWidget = 1
	}

	private enum WidgetSerachOption
	{
		None,
		Local,
		Parent
	}

	[Header("Widget (optional if auto search is enabled)")]
	[SerializeField]
	private UIWidget m_orderWidget;

	[SerializeField]
	[Header("Widget auto search")]
	private WidgetSerachOption m_widgetSearchOption;

	[Header("Order Settings")]
	[SerializeField]
	private OrderPosition m_orderPosition = OrderPosition.BelowWidget;

	[SerializeField]
	private bool m_reorderOnlyOnce = true;

	[SerializeField]
	[Header("Renderers")]
	private bool m_autoSearchRenderers;

	[SerializeField]
	private List<Renderer> m_renderers;

	[Header("Particle Systems - Legacy: Please drag into renderers instead!")]
	[SerializeField]
	private bool m_autoSearchParticleSystems;

	[SerializeField]
	private List<ParticleSystem> m_particleSystems;

	private int m_lastRenderQueue = -1;

	private void Awake()
	{
		InitializeWidget();
		if (m_orderWidget == null)
		{
			DebugLog.Error(GetType(), "No ui widget has been found. Disabling this script.");
			base.enabled = false;
		}
	}

	private void OnEnable()
	{
		InitializeRenderElements();
	}

	private void OnDisable()
	{
		if ((bool)m_orderWidget)
		{
			UIWidget orderWidget = m_orderWidget;
			orderWidget.onRender = (UIDrawCall.OnRenderCallback)Delegate.Remove(orderWidget.onRender, new UIDrawCall.OnRenderCallback(AdjustRenderQueueValues));
		}
		if (m_autoSearchRenderers)
		{
			m_renderers.Clear();
		}
	}

	public void ClearAndAutoSearchRenderers()
	{
		m_renderers.Clear();
		StartCoroutine(ReorderAfterFrame());
	}

	private void LateUpdate()
	{
		if (m_orderWidget == null)
		{
			InitializeWidget();
		}
		if (m_orderWidget == null)
		{
			base.enabled = false;
			return;
		}
		if (m_autoSearchRenderers)
		{
			for (int num = m_renderers.Count - 1; num >= 0; num--)
			{
				if (m_renderers[num] == null || !m_renderers[num].gameObject.activeInHierarchy)
				{
					m_renderers.RemoveAt(num);
				}
			}
			if (m_renderers.Count == 0)
			{
				FindRenderers();
				if (m_renderers.Count == 0)
				{
					return;
				}
				if (m_reorderOnlyOnce)
				{
					AdjustRenderQueueValues();
				}
			}
		}
		if (!m_reorderOnlyOnce)
		{
			AdjustRenderQueueValues();
		}
	}

	private void InitializeWidget()
	{
		if (m_widgetSearchOption == WidgetSerachOption.None)
		{
			return;
		}
		if (m_widgetSearchOption == WidgetSerachOption.Local)
		{
			m_orderWidget = GetComponent<UIWidget>();
		}
		else
		{
			if (m_widgetSearchOption != WidgetSerachOption.Parent)
			{
				return;
			}
			m_orderWidget = GetComponentInParent<UIWidget>();
			if (m_orderWidget != null)
			{
				return;
			}
			Transform parent = base.transform;
			do
			{
				parent = parent.parent;
				if (parent != null)
				{
					m_orderWidget = parent.GetComponent<UIWidget>();
				}
			}
			while (parent != null && m_orderWidget == null);
		}
	}

	private void InitializeRenderElements()
	{
		FindRenderers();
		FindParticleSystems();
		if ((bool)m_orderWidget)
		{
			UIWidget orderWidget = m_orderWidget;
			orderWidget.onRender = (UIDrawCall.OnRenderCallback)Delegate.Combine(orderWidget.onRender, new UIDrawCall.OnRenderCallback(AdjustRenderQueueValues));
			StartCoroutine(ReorderAfterFrame());
		}
	}

	private void FindRenderers()
	{
		if (m_autoSearchRenderers)
		{
			m_renderers.Clear();
			m_renderers.AddRange(GetComponentsInChildren<Renderer>());
		}
	}

	private void FindParticleSystems()
	{
		if (m_autoSearchParticleSystems)
		{
			m_particleSystems.Clear();
			m_particleSystems.AddRange(GetComponentsInChildren<ParticleSystem>());
		}
		foreach (ParticleSystem particleSystem in m_particleSystems)
		{
			if (!(particleSystem == null))
			{
				Renderer[] componentsInChildren = particleSystem.GetComponentsInChildren<Renderer>();
				m_renderers.AddRange(componentsInChildren);
			}
		}
	}

	private void AdjustRenderQueueValues()
	{
		if (!m_orderWidget.drawCall)
		{
			return;
		}
		int renderQueue = (m_lastRenderQueue = (int)(m_orderWidget.drawCall.renderQueue + m_orderPosition));
		foreach (Renderer renderer in m_renderers)
		{
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				material.renderQueue = renderQueue;
			}
		}
	}

	private void AdjustRenderQueueValues(Material mat)
	{
		if (!mat)
		{
			return;
		}
		int num = (int)(mat.renderQueue + m_orderPosition);
		if (num == m_lastRenderQueue)
		{
			return;
		}
		m_lastRenderQueue = num;
		for (int num2 = m_renderers.Count - 1; num2 >= 0; num2--)
		{
			if (m_renderers[num2] == null)
			{
				m_renderers.RemoveAt(num2);
			}
			else
			{
				Material[] materials = m_renderers[num2].materials;
				foreach (Material material in materials)
				{
					material.renderQueue = num;
				}
			}
		}
	}

	private IEnumerator ReorderAfterFrame()
	{
		yield return new WaitForEndOfFrame();
		if (m_autoSearchRenderers)
		{
			for (int i = m_renderers.Count - 1; i >= 0; i--)
			{
				if (m_renderers[i] == null || !m_renderers[i].gameObject.activeInHierarchy)
				{
					m_renderers.RemoveAt(i);
				}
			}
			if (m_renderers.Count == 0)
			{
				FindRenderers();
				if (m_renderers.Count == 0)
				{
					yield break;
				}
			}
		}
		AdjustRenderQueueValues();
	}
}
