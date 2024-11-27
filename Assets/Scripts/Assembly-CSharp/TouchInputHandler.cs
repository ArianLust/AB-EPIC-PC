using System.Collections.Generic;
using UnityEngine;

public class TouchInputHandler : MonoBehaviour
{
	private Transform targetedObject;

	[SerializeField]
	private LayerMask m_layers;

	[SerializeField]
	private LayerMask m_layersToCollider;

	[SerializeField]
	public Camera m_camera;

	[SerializeField]
	private bool m_UsePriorInputHandler = true;

	public List<TouchInputHandler> m_SecondaryInputHandlers = new List<TouchInputHandler>();

	private TouchInputHandler m_PrimaryInputHandler;

	private LayerMask m_usedlayers;

	private float m_pressTime;

	[SerializeField]
	private float m_clickedTimeThreashold = 0.25f;

	private void Awake()
	{
		if (m_UsePriorInputHandler && DIContainerInfrastructure.GetCoreStateMgr() != null)
		{
			m_PrimaryInputHandler = DIContainerInfrastructure.GetCoreStateMgr().GetComponent<TouchInputHandler>();
			if ((bool)m_PrimaryInputHandler)
			{
				m_PrimaryInputHandler.m_SecondaryInputHandlers.Add(this);
			}
		}
		else
		{
			m_SecondaryInputHandlers.Add(this);
		}
	}

	private void OnDestroy()
	{
		if ((bool)m_PrimaryInputHandler)
		{
			m_PrimaryInputHandler.m_SecondaryInputHandlers.Remove(this);
		}
	}

	private void Update()
	{
		if ((!(m_PrimaryInputHandler == null) || m_SecondaryInputHandlers.Count != 0) && !m_UsePriorInputHandler)
		{
			#if UNITY_ANDROID || UNITY_IOS
			EvaluateTouches();
			#else
			EvaluateMouse();
			#endif
		}
	}

	public bool GetRayCastHit(Vector3 pos, out RaycastHit hit)
	{
		m_usedlayers = m_layers.value;
		Ray ray = m_camera.ScreenPointToRay(pos);
		return Physics.Raycast(ray, out hit, 10000f, m_usedlayers);
	}

	public void EvaluateMouse()
	{
		if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0) && !Input.GetMouseButton(0))
		{
			if ((bool)targetedObject)
			{
				targetedObject.SendMessage("OnTouchReleased", SendMessageOptions.DontRequireReceiver);
			}
			targetedObject = null;
			return;
		}
		Transform transform = null;
		bool flag = false;
		for (int num = m_SecondaryInputHandlers.Count - 1; num >= 0; num--)
		{
			RaycastHit hit;
			if (m_SecondaryInputHandlers[num].GetRayCastHit(Input.mousePosition, out hit))
			{
				RaycastHit raycastHit = hit;
				transform = raycastHit.transform;
				flag = true;
			}
		}
		if (Input.GetMouseButtonDown(0) && !targetedObject)
		{
			if (flag)
			{
				targetedObject = transform;
				targetedObject.SendMessage("OnTouchDown", SendMessageOptions.DontRequireReceiver);
				m_pressTime = Time.realtimeSinceStartup;
			}
			else
			{
				targetedObject = null;
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (flag)
			{
				if ((bool)targetedObject && targetedObject != transform)
				{
					targetedObject.SendMessage("OnTouchReleased", SendMessageOptions.DontRequireReceiver);
					targetedObject = null;
				}
				else if ((bool)targetedObject)
				{
					targetedObject.SendMessage("OnTouchReleased", SendMessageOptions.DontRequireReceiver);
					if (Time.realtimeSinceStartup - m_pressTime < m_clickedTimeThreashold)
					{
						targetedObject.SendMessage("OnTouchClicked", SendMessageOptions.DontRequireReceiver);
					}
					targetedObject = null;
				}
			}
			else if ((bool)targetedObject)
			{
				targetedObject.SendMessage("OnTouchReleased", SendMessageOptions.DontRequireReceiver);
				targetedObject = null;
			}
		}
		else if ((bool)targetedObject)
		{
			targetedObject.SendMessage("OnTouchDrag", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void EvaluateTouches()
	{
		if (Input.touchCount <= 0)
		{
			if ((bool)targetedObject)
			{
				targetedObject.SendMessage("OnTouchReleased", SendMessageOptions.DontRequireReceiver);
			}
			targetedObject = null;
			return;
		}
		Touch touch = Input.GetTouch(0);
		Transform transform = null;
		bool flag = false;
		if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
		{
			for (int num = m_SecondaryInputHandlers.Count - 1; num >= 0; num--)
			{
				RaycastHit hit;
				if (m_SecondaryInputHandlers[num].GetRayCastHit(touch.position, out hit))
				{
					RaycastHit raycastHit = hit;
					transform = raycastHit.transform;
					flag = true;
				}
			}
		}
		if (touch.phase == TouchPhase.Began)
		{
			if (flag)
			{
				targetedObject = transform;
				targetedObject.SendMessage("OnTouchDown", SendMessageOptions.DontRequireReceiver);
				m_pressTime = Time.realtimeSinceStartup;
			}
			else
			{
				targetedObject = null;
			}
		}
		else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && (bool)targetedObject)
		{
			targetedObject.SendMessage("OnTouchDrag", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
			{
				return;
			}
			if (flag)
			{
				if ((bool)targetedObject && targetedObject != transform)
				{
					targetedObject.SendMessage("OnTouchReleased", SendMessageOptions.DontRequireReceiver);
					targetedObject = null;
				}
				else if ((bool)targetedObject)
				{
					targetedObject.SendMessage("OnTouchReleased", SendMessageOptions.DontRequireReceiver);
					if (Time.realtimeSinceStartup - m_pressTime < m_clickedTimeThreashold)
					{
						targetedObject.SendMessage("OnTouchClicked", SendMessageOptions.DontRequireReceiver);
					}
					targetedObject = null;
				}
			}
			else if ((bool)targetedObject)
			{
				targetedObject.SendMessage("OnTouchReleased", SendMessageOptions.DontRequireReceiver);
				targetedObject = null;
			}
		}
	}

	public void SetTutorialLayers(bool activate)
	{
		if (activate)
		{
			if (m_camera.CompareTag("SceneryCamera"))
			{
				m_layers = 1 << LayerMask.NameToLayer("TutorialScenery");
			}
			else
			{
				m_layers = (1 << LayerMask.NameToLayer("TutorialInterface")) | (1 << LayerMask.NameToLayer("IgnoreTutorialInterface")) | (1 << LayerMask.NameToLayer("InterfaceCharacter"));
			}
		}
		else if (m_camera.CompareTag("SceneryCamera"))
		{
			m_layers = 1 << LayerMask.NameToLayer("Scenery");
		}
		else
		{
			m_layers = (1 << LayerMask.NameToLayer("Interface")) | (1 << LayerMask.NameToLayer("IgnoreTutorialInterface")) | (1 << LayerMask.NameToLayer("InterfaceCharacter"));
		}
		if (m_SecondaryInputHandlers == null)
		{
			return;
		}
		foreach (TouchInputHandler secondaryInputHandler in m_SecondaryInputHandlers)
		{
			if (secondaryInputHandler != this)
			{
				secondaryInputHandler.SetTutorialLayers(activate);
			}
		}
	}
}
