using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UISwipeInputTrigger : MonoBehaviour
{
	public Vector2 m_TresholdEnd = new Vector2(0f, 80f);

	public Vector2 m_TresholdBegin = new Vector2(0f, 1f);

	public Vector2 m_NormalizedNonSwipeThreashold = new Vector2(1f, 0f);

	private Vector3 delta = Vector3.zero;

	private bool swipeBegan;

	private bool swipeDone;

	private Vector3 oldPos;

	private Camera m_InterfaceCamera;

	private Vector2 m_normalizedThreashold;

	public List<SwipeDirection> m_PossibleDirections = new List<SwipeDirection> { SwipeDirection.UP };

	private SwipeDirection m_SwipeDirection;

	public float m_DirectionToleranz = 20f;

	[method: MethodImpl(32)]
	public event Action SwipeLeft;

	[method: MethodImpl(32)]
	public event Action SwipeRight;

	[method: MethodImpl(32)]
	public event Action SwipeUp;

	[method: MethodImpl(32)]
	public event Action SwipeDown;

	[method: MethodImpl(32)]
	public event Action<bool> SwipeBegan;

	[method: MethodImpl(32)]
	public event Action<float> OnDragUpDown;

	[method: MethodImpl(32)]
	public event Action<float> OnDragLeftRight;

	private void Awake()
	{
		m_normalizedThreashold = m_TresholdBegin.normalized;
		m_NormalizedNonSwipeThreashold = new Vector2(m_normalizedThreashold.y, m_normalizedThreashold.x);
		m_InterfaceCamera = Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("UICamera"));
	}

	public void OnDrag(Vector2 nguidelta)
	{
		Vector3 vector = m_InterfaceCamera.ScreenToWorldPoint(Input.mousePosition);
		delta += vector - oldPos;
		oldPos = vector;
		if (delta.magnitude <= 20f)
		{
			return;
		}
		if (LostSwipeCondition())
		{
			swipeDone = false;
			m_SwipeDirection = SwipeDirection.NONE;
		}
		if (m_TresholdEnd.y > 0f)
		{
			if (m_PossibleDirections.Contains(SwipeDirection.UP) && delta.y > m_TresholdEnd.y)
			{
				swipeDone = true;
				m_SwipeDirection = SwipeDirection.UP;
			}
			else if (m_PossibleDirections.Contains(SwipeDirection.DOWN) && delta.y < 0f - m_TresholdEnd.y)
			{
				swipeDone = true;
				m_SwipeDirection = SwipeDirection.DOWN;
			}
		}
		if (swipeDone)
		{
			if (swipeBegan)
			{
				RegisterDragMovement();
			}
			return;
		}
		m_SwipeDirection = SwipeDirection.NONE;
		if (!swipeBegan)
		{
			Vector2 to = delta.normalized;
			float num = Mathf.Abs(Vector2.Angle(m_TresholdBegin, to));
			if (num >= 90f)
			{
				num = 180f - num;
			}
			DebugLog.Log("Swipe Degree " + num);
			if (!(num < m_DirectionToleranz))
			{
				swipeDone = true;
				return;
			}
			swipeBegan = true;
			if (swipeBegan && this.SwipeBegan != null)
			{
				this.SwipeBegan(true);
			}
		}
		RegisterDragMovement();
		if (m_TresholdEnd.x > 0f)
		{
			if (m_PossibleDirections.Contains(SwipeDirection.RIGHT) && delta.x > m_TresholdEnd.x)
			{
				DebugLog.Log("Swiped Right");
				swipeDone = true;
				m_SwipeDirection = SwipeDirection.RIGHT;
			}
			else if (m_PossibleDirections.Contains(SwipeDirection.LEFT) && delta.x < 0f - m_TresholdEnd.x)
			{
				DebugLog.Log("Swiped Left");
				swipeDone = true;
				m_SwipeDirection = SwipeDirection.LEFT;
			}
		}
		if (m_TresholdEnd.y > 0f)
		{
			if (m_PossibleDirections.Contains(SwipeDirection.UP) && delta.y > m_TresholdEnd.y)
			{
				DebugLog.Log("Swiped Up");
				swipeDone = true;
				m_SwipeDirection = SwipeDirection.UP;
			}
			else if (m_PossibleDirections.Contains(SwipeDirection.DOWN) && delta.y < 0f - m_TresholdEnd.y)
			{
				DebugLog.Log("Swiped Down");
				swipeDone = true;
				m_SwipeDirection = SwipeDirection.DOWN;
			}
		}
	}

	private bool LostSwipeCondition()
	{
		return m_SwipeDirection != 0 && ((m_SwipeDirection == SwipeDirection.RIGHT && delta.x < m_TresholdEnd.x) || (m_SwipeDirection == SwipeDirection.LEFT && delta.x > 0f - m_TresholdEnd.x) || (m_SwipeDirection == SwipeDirection.UP && delta.y < m_TresholdEnd.y) || (m_SwipeDirection == SwipeDirection.DOWN && delta.y > 0f - m_TresholdEnd.y));
	}

	private void RegisterDragMovement()
	{
		if (this.OnDragUpDown != null && (delta.y == 0f || (delta.y < 0f && m_PossibleDirections.Contains(SwipeDirection.DOWN)) || (delta.y > 0f && m_PossibleDirections.Contains(SwipeDirection.UP))))
		{
			this.OnDragUpDown(delta.y);
		}
		if (this.OnDragLeftRight != null && (delta.x == 0f || (delta.x < 0f && m_PossibleDirections.Contains(SwipeDirection.LEFT)) || (delta.x > 0f && m_PossibleDirections.Contains(SwipeDirection.RIGHT))))
		{
			this.OnDragLeftRight(delta.x);
		}
	}

	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			delta = Vector3.zero;
			oldPos = m_InterfaceCamera.ScreenToWorldPoint(Input.mousePosition);
			swipeBegan = false;
			swipeDone = false;
			return;
		}
		delta = Vector3.zero;
		if (swipeBegan)
		{
			if (!swipeDone)
			{
				if (this.OnDragUpDown != null)
				{
					this.OnDragUpDown(delta.y);
				}
				if (this.OnDragLeftRight != null)
				{
					this.OnDragLeftRight(delta.x);
				}
			}
			else
			{
				switch (m_SwipeDirection)
				{
				case SwipeDirection.UP:
					if (this.SwipeUp != null)
					{
						this.SwipeUp();
					}
					break;
				case SwipeDirection.DOWN:
					if (this.SwipeDown != null)
					{
						this.SwipeDown();
					}
					break;
				case SwipeDirection.LEFT:
					if (this.SwipeLeft != null)
					{
						this.SwipeLeft();
					}
					break;
				case SwipeDirection.RIGHT:
					if (this.SwipeRight != null)
					{
						this.SwipeRight();
					}
					break;
				}
			}
			if (this.SwipeBegan != null)
			{
				this.SwipeBegan(false);
			}
		}
		swipeBegan = false;
		swipeDone = false;
	}
}
