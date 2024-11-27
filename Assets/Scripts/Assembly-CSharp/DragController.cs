using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Shared.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{
	[SerializeField]
	private ContainerControl m_DragAreaContainer;

	[SerializeField]
	private float m_DragTreshhold = 10f;

	[SerializeField]
	private float m_dampingValue;

	[SerializeField]
	private Vector2 m_scale = Vector2.one;

	[SerializeField]
	private float m_ReferencedResolutionHeight = 768f;

	public Camera m_camera;

	private Camera m_InterfaceCamera;

	private Vector2 m_startPosition;

	private Vector2 m_lastPosition;

	private bool m_pressed;

	public bool m_dragging;

	private float m_dragStartTimeStamp;

	private Vector2 velocity;

	private Vector2 m_lowerLeftArea = Vector3.zero;

	private Vector2 m_upperRightArea = Vector3.zero;

	private bool[] m_ActiveArray = new bool[3] { true, true, true };

	[SerializeField]
	private float m_speed = 1000f;

	private float m_SpeedHeightFactor = 1f;

	private float m_BaseOrthoSize;

	[SerializeField]
	private List<ContainerControl> m_DragContainers;

	[SerializeField]
	private List<string> m_DragContainersHotspots;

	private void Awake()
	{
		for (int i = 0; i < m_DragContainers.Count; i++)
		{
			m_DragContainers[i].gameObject.SetActive(false);
		}
		HotspotGameData value = null;
		for (int num = m_DragContainers.Count - 1; num >= 0; num--)
		{
			if (DIContainerInfrastructure.GetCurrentPlayer().WorldGameData.HotspotGameDatas.TryGetValue(m_DragContainersHotspots[num], out value) && value.Data.UnlockState >= HotspotUnlockState.ResolvedNew)
			{
				m_DragContainers[num].gameObject.SetActive(true);
				m_DragAreaContainer = m_DragContainers[num];
				break;
			}
		}
		CalculateBounds();
		DIContainerInfrastructure.CurrentDragController = this;
		m_InterfaceCamera = Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("UICamera"));
		m_BaseOrthoSize = m_camera.orthographicSize;
	}

	private void OnDestroy()
	{
		if (DIContainerInfrastructure.CurrentDragController == this)
		{
			DIContainerInfrastructure.CurrentDragController = null;
		}
	}

	public void SetActiveDepth(bool active, int depth)
	{
		if (depth > 2)
		{
			return;
		}
		m_ActiveArray[depth] = active;
		for (int i = 0; i < m_ActiveArray.Length; i++)
		{
			if (!m_ActiveArray[i])
			{
				base.enabled = false;
				return;
			}
		}
		base.enabled = true;
	}

	private void Update()
	{
		if (DIContainerInfrastructure.GetCoreStateMgr().m_WindowRoot.entered || DIContainerInfrastructure.GetCoreStateMgr().m_PopupRoot.entered || DIContainerInfrastructure.GetCoreStateMgr().IsShopOpen())
		{
			return;
		}
		if (Input.GetMouseButtonDown(0) && Input.touchCount < 2)
		{
			StartDragging();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			EndDragging();
		}
		else if (Input.GetMouseButton(0) && Input.touchCount < 2)
		{
			if (!m_pressed)
			{
				StartDragging();
			}
			else
			{
				Drag();
			}
		}
		else if (m_pressed)
		{
			EndDragging();
		}
	}

	private void StartDragging()
	{
		StopCoroutine("MomentumDragging");
		m_startPosition = GetCurrentMouseWorldPositionRelativeToInterfaceCamera();
		m_lastPosition = GetCurrentMouseWorldPositionRelativeToInterfaceCamera();
		m_pressed = true;
	}

	private Vector3 GetCurrentMouseWorldPositionRelativeToInterfaceCamera()
	{
		return m_InterfaceCamera.ScreenToWorldPoint(Input.mousePosition);
	}

	private void EndDragging()
	{
		m_lastPosition = GetCurrentMouseWorldPositionRelativeToInterfaceCamera();
		StartCoroutine("MomentumDragging");
		m_pressed = false;
		m_dragging = false;
	}

	private IEnumerator MomentumDragging()
	{
		Vector2 outOfBounds = GetOutOfBoundsValue(base.transform.position);
		Vector2 lastDelta = Vector2.zero;
		float passedTime = 1f;
		do
		{
			yield return null;
			passedTime -= Time.deltaTime * 4f;
			if (passedTime < 0f)
			{
				passedTime = 0f;
			}
			if (outOfBounds != Vector2.zero && passedTime > 0f)
			{
				float degree = 0f - SinusoidalEaseInOut(passedTime, 0f, 1f);
				Vector2 delta = outOfBounds * degree;
				base.transform.Translate(delta - lastDelta);
				lastDelta = delta;
			}
			Vector2 offset = DampVec2(ref velocity, 9f, Time.deltaTime);
			Vector2 momentumBounds = GetOutOfBoundsValue((Vector2)base.transform.position + offset);
			if (momentumBounds.x > 0f)
			{
				velocity.x *= 0.75f;
			}
			if (momentumBounds.y > 0f)
			{
				velocity.y *= 0.75f;
			}
			if (outOfBounds == Vector2.zero)
			{
				base.transform.Translate(new Vector3(offset.x, offset.y, 0f));
			}
			else if (outOfBounds.x == 0f)
			{
				base.transform.Translate(new Vector3(offset.x, 0f, 0f));
			}
			else if (outOfBounds.y == 0f)
			{
				base.transform.Translate(new Vector3(0f, offset.y, 0f));
			}
		}
		while (velocity.magnitude > 1f || passedTime > 0f);
		if (GetBoundsFactor() != Vector2.one)
		{
			StartCoroutine("MomentumDragging");
		}
	}

	private float SinusoidalEaseInOut(float t, float start, float end)
	{
		t = Mathf.Clamp01(t);
		return (0f - end) / 2f * (Mathf.Cos((float)Math.PI * t) - 1f) + start - 1f;
	}

	private Vector2 DampVec2(ref Vector2 velocity, float strength, float deltaTime)
	{
		if (deltaTime > 1f)
		{
			deltaTime = 1f;
		}
		float num = 1f - strength * 0.001f;
		int num2 = Mathf.RoundToInt(deltaTime * 1000f);
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < num2; i++)
		{
			zero += velocity * 0.06f;
			velocity *= num;
		}
		return zero;
	}

	private void Drag()
	{
		Vector2 vector = GetCurrentMouseWorldPositionRelativeToInterfaceCamera();
		if (!m_dragging)
		{
			if (!(Vector2.Distance(m_startPosition, vector) > m_DragTreshhold))
			{
				return;
			}
			velocity = Vector2.zero;
			m_lastPosition = vector;
			m_dragStartTimeStamp = Time.time;
			m_dragging = true;
		}
		Vector2 a = Vector2.Scale(vector - m_lastPosition, m_scale);
		float num = m_camera.orthographicSize / m_BaseOrthoSize;
		a = Vector2.Scale(a, new Vector2(num, num));
		a = Vector2.Scale(a, GetBoundsFactor());
		velocity = Vector2.Lerp(velocity, velocity - a * 0.35f, 0.67f);
		base.transform.Translate(0f - a.x, 0f - a.y, 0f);
		m_lastPosition = vector;
		DampVec2(ref velocity, 9f, Time.deltaTime);
	}

	public void CalculateBounds()
	{
		if (!m_DragAreaContainer)
		{
			DebugLog.Warn(GetType(), "CalculateBounds: m_DragAreaContainer unassigned");
			return;
		}
		Vector2 vector = m_DragAreaContainer.m_Size / 2f;
		m_lowerLeftArea = new Vector2(m_DragAreaContainer.transform.position.x - vector.x, m_DragAreaContainer.transform.position.y - vector.y);
		m_upperRightArea = new Vector2(m_DragAreaContainer.transform.position.x + vector.x, m_DragAreaContainer.transform.position.y + vector.y);
	}

	public void SetDragAreaContainer(ContainerControl dragCollider)
	{
		m_DragAreaContainer = dragCollider;
		CalculateBounds();
	}

	private Vector2 GetBoundsFactor()
	{
		if (m_DragAreaContainer == null)
		{
			return Vector2.one;
		}
		Vector2 one = Vector2.one;
		Vector3 position = base.transform.position;
		Vector2 vector = new Vector2(m_camera.orthographicSize * ((float)Screen.width / (float)Screen.height), m_camera.orthographicSize);
		if (position.x < m_lowerLeftArea.x + vector.x)
		{
			one.x = 1f - Mathf.Abs((position.x - (m_lowerLeftArea.x + vector.x)) / (vector.x / 2f));
		}
		else if (position.x > m_upperRightArea.x - vector.x)
		{
			one.x = 1f - Mathf.Abs((position.x - (m_upperRightArea.x - vector.x)) / (vector.x / 2f));
		}
		if (position.y < m_lowerLeftArea.y + vector.y)
		{
			one.y = 1f - Mathf.Abs((position.y - (m_lowerLeftArea.y + vector.y)) / (vector.x / 2f));
		}
		else if (position.y > m_upperRightArea.y - vector.y)
		{
			one.y = 1f - Mathf.Abs((position.y - (m_upperRightArea.y - vector.y)) / (vector.x / 2f));
		}
		return one;
	}

	public Vector2 GetOutOfBoundsValue(Vector3 currentPos)
	{
		if (m_DragAreaContainer == null)
		{
			return Vector2.zero;
		}
		Vector2 zero = Vector2.zero;
		Vector2 vector = new Vector2(m_camera.orthographicSize * ((float)Screen.width / (float)Screen.height), m_camera.orthographicSize);
		if (currentPos.x < m_lowerLeftArea.x + vector.x)
		{
			zero.x = currentPos.x - (m_lowerLeftArea.x + vector.x);
		}
		else if (currentPos.x > m_upperRightArea.x - vector.x)
		{
			zero.x = currentPos.x - (m_upperRightArea.x - vector.x);
		}
		if (currentPos.y < m_lowerLeftArea.y + vector.y)
		{
			zero.y = currentPos.y - (m_lowerLeftArea.y + vector.y);
		}
		else if (currentPos.y > m_upperRightArea.y - vector.y)
		{
			zero.y = currentPos.y - (m_upperRightArea.y - vector.y);
		}
		return -zero;
	}
}
