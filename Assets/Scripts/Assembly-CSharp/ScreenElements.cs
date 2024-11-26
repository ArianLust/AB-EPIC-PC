using System.Collections;
using UnityEngine;

public class ScreenElements : MonoBehaviour
{
	[SerializeField]
	private Animation m_StorySequenceAnimation;

	private static ScreenElements _instance;

	private bool m_storySequenceVisible;

	[SerializeField]
	private BoxCollider m_StorySequenceSceneryCollider;

	[SerializeField]
	private UIInputTrigger m_SpeedUpToggleButton;

	[SerializeField]
	private Camera m_StoryCamera;

	private Camera m_UICamera;

	private bool m_IsSpeedUpAllowed;

	private bool m_SpeededUp;

	private float m_lastToggleTime;

	public static ScreenElements Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		base.transform.parent = CoreStateMgr.Instance.m_GenericInterfaceRoot;
		_instance = this;
		m_UICamera = DIContainerInfrastructure.GetCoreStateMgr().m_InterfaceCamera;
	}

	public float EnableStorySequence(bool enable)
	{
		if ((bool)m_StorySequenceSceneryCollider)
		{
			m_StorySequenceSceneryCollider.gameObject.SetActive(enable);
		}
		if (m_storySequenceVisible == enable)
		{
			return 0f;
		}
		m_storySequenceVisible = enable;
		string animation;
		if (enable)
		{
			m_IsSpeedUpAllowed = true;
			RegisterEventHandler();
			if ((bool)m_UICamera && (bool)m_StoryCamera)
			{
				m_UICamera.gameObject.SetActive(false);
				m_StoryCamera.gameObject.SetActive(true);
			}
			if ((bool)DIContainerInfrastructure.BackButtonMgr)
			{
				DIContainerInfrastructure.BackButtonMgr.RegisterBlockReason("story_sequence_running");
			}
			animation = "StorySequence_Start";
		}
		else
		{
			DIContainerInfrastructure.TimeScaleMgr.RemoveTimeScale("story_speedup");
			m_SpeedUpToggleButton.GetComponent<Animation>().Play("SpeedUpButton_NormalSpeed_Toggled");
			m_SpeededUp = false;
			m_IsSpeedUpAllowed = false;
			animation = "StorySequence_End";
			if ((bool)m_UICamera && (bool)m_StoryCamera)
			{
				StartCoroutine(DelayedActivateUICamera(true, m_StorySequenceAnimation[animation].length));
			}
			if ((bool)DIContainerInfrastructure.BackButtonMgr)
			{
				DIContainerInfrastructure.BackButtonMgr.DeRegisterBlockReason("story_sequence_running");
			}
		}
		if (DIContainerInfrastructure.CurrentDragController != null)
		{
			DIContainerInfrastructure.CurrentDragController.SetActiveDepth(!enable, 0);
		}
		m_StorySequenceAnimation.Play(animation);
		return 0f;
	}

	private void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		m_SpeedUpToggleButton.Pressed += ToggleButtonIsPressed;
	}

	private void DeRegisterEventHandler()
	{
		m_SpeedUpToggleButton.Pressed -= ToggleButtonIsPressed;
	}

	public void ToggleButtonIsPressed(bool pressed)
	{
		if (m_IsSpeedUpAllowed)
		{
			m_lastToggleTime = Time.time;
			if (pressed)
			{
				m_SpeedUpToggleButton.GetComponent<Animation>().Play((!m_SpeededUp) ? "SpeedUpButton_SpeedUp_Pressed" : "SpeedUpButton_NormalSpeed_Pressed");
				return;
			}
			m_SpeedUpToggleButton.GetComponent<Animation>().Play((!m_SpeededUp) ? "SpeedUpButton_SpeedUp_Toggled" : "SpeedUpButton_NormalSpeed_Toggled");
			SetSpeedUp(!m_SpeededUp);
		}
	}

	public void ToggleSpeedUp()
	{
		SetSpeedUp(!m_SpeededUp);
	}

	public void SetSpeedUp(bool speedUp)
	{
		if (m_IsSpeedUpAllowed && m_SpeededUp != speedUp)
		{
			m_SpeededUp = speedUp;
			if (m_SpeededUp)
			{
				DIContainerInfrastructure.TimeScaleMgr.AddTimeScale("story_speedup", 3.5f);
			}
			else
			{
				DIContainerInfrastructure.TimeScaleMgr.RemoveTimeScale("story_speedup");
			}
		}
	}

	private IEnumerator DelayedActivateUICamera(bool activate, float delay)
	{
		yield return new WaitForSeconds(delay);
		m_UICamera.gameObject.SetActive(activate);
		m_StoryCamera.gameObject.SetActive(!activate);
	}
}
