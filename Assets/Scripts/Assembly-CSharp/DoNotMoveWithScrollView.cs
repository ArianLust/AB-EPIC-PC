using System;
using UnityEngine;

public class DoNotMoveWithScrollView : MonoBehaviour
{
	[SerializeField]
	private UIScrollView m_scrollView;

	private Transform m_scrollTransform;

	private Vector3 m_offset;

	private void Awake()
	{
		m_scrollTransform = m_scrollView.transform;
		m_offset = m_scrollTransform.localPosition + base.transform.localPosition;
		UIScrollView scrollView = m_scrollView;
		scrollView.onDragStarted = (UIScrollView.OnDragNotification)Delegate.Remove(scrollView.onDragStarted, new UIScrollView.OnDragNotification(OnStartMove));
		UIScrollView scrollView2 = m_scrollView;
		scrollView2.onStoppedMoving = (UIScrollView.OnDragNotification)Delegate.Remove(scrollView2.onStoppedMoving, new UIScrollView.OnDragNotification(OnEndMove));
		UIScrollView scrollView3 = m_scrollView;
		scrollView3.onDragStarted = (UIScrollView.OnDragNotification)Delegate.Combine(scrollView3.onDragStarted, new UIScrollView.OnDragNotification(OnStartMove));
		UIScrollView scrollView4 = m_scrollView;
		scrollView4.onStoppedMoving = (UIScrollView.OnDragNotification)Delegate.Combine(scrollView4.onStoppedMoving, new UIScrollView.OnDragNotification(OnEndMove));
	}

	private void LateUpdate()
	{
		base.transform.localPosition = m_offset - m_scrollTransform.localPosition;
	}

	private void OnStartMove()
	{
		base.enabled = true;
	}

	private void OnEndMove()
	{
		base.enabled = false;
		LateUpdate();
	}

	private void OnDestroy()
	{
		UIScrollView scrollView = m_scrollView;
		scrollView.onDragStarted = (UIScrollView.OnDragNotification)Delegate.Remove(scrollView.onDragStarted, new UIScrollView.OnDragNotification(OnStartMove));
		UIScrollView scrollView2 = m_scrollView;
		scrollView2.onStoppedMoving = (UIScrollView.OnDragNotification)Delegate.Remove(scrollView2.onStoppedMoving, new UIScrollView.OnDragNotification(OnEndMove));
	}
}
