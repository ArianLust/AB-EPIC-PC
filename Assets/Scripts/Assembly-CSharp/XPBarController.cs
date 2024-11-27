using System.Collections;
using System.Collections.Generic;
using ABH.Shared.BalancingData;
using UnityEngine;

public class XPBarController : MonoBehaviour
{
	private bool IsEntering;

	private bool HasLeft;

	private float TimeTillLeave;

	private bool HadLevelUp;

	private int Level;

	public Animation UpdateAnimation;

	public Transform LevelDisplay;

	[SerializeField]
	private List<GameObject> m_MaxLevelDisplays = new List<GameObject>();

	public float LevelUpAnimationDelay;

	private float neededXPForLevelUp;

	private float m_OldPercentage;

	[SerializeField]
	private UILabel m_LevelLabel;

	[SerializeField]
	private UISprite m_FillAnimationSprite;

	[SerializeField]
	private UISprite m_FillIndikatorSprite;

	private bool m_Updating;

	private bool m_DoingLevelUp;

	private float m_OldValue;

	private void Awake()
	{
		TimeTillLeave = DIContainerLogic.GetPacingBalancing().XpAndGoldBarStayDuration - DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.m_XPBarController.GetEnterDuration();
		m_LevelLabel.text = DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString();
		SetNeededXP();
		float experience = DIContainerInfrastructure.GetCurrentPlayer().Data.Experience;
		float num = experience / neededXPForLevelUp;
		DebugLog.Log("set start up XP to " + num + "        " + experience + "/" + neededXPForLevelUp);
		m_FillAnimationSprite.fillAmount = num;
		m_FillIndikatorSprite.fillAmount = num;
		DIContainerInfrastructure.GetCurrentPlayer().CharacterLevelChanged -= LevelChangedHandler;
		DIContainerInfrastructure.GetCurrentPlayer().CharacterLevelChanged += LevelChangedHandler;
		LevelUpAnimationDelay = GetComponent<Animation>()["Display_Top_Enter"].length + TimeTillLeave;
		m_OldPercentage = DIContainerInfrastructure.GetCurrentPlayer().Data.Experience / neededXPForLevelUp;
		m_OldValue = DIContainerInfrastructure.GetCurrentPlayer().Data.Experience;
	}

	private void SetNeededXP()
	{
		ExperienceLevelBalancingData balancing;
		if (DIContainerBalancing.Service.TryGetBalancingData<ExperienceLevelBalancingData>("Level_" + DIContainerInfrastructure.GetCurrentPlayer().Data.Level.ToString("00"), out balancing))
		{
			foreach (GameObject maxLevelDisplay in m_MaxLevelDisplays)
			{
				maxLevelDisplay.SetActive(false);
			}
			neededXPForLevelUp = balancing.Experience;
			return;
		}
		neededXPForLevelUp = (int)DIContainerInfrastructure.GetCurrentPlayer().Data.Experience;
		foreach (GameObject maxLevelDisplay2 in m_MaxLevelDisplays)
		{
			maxLevelDisplay2.SetActive(true);
		}
	}

	public void LevelChangedHandler(int level)
	{
		HadLevelUp = true;
		Level = level;
	}

	private IEnumerator LevelUpCoroutine(int level)
	{
		m_DoingLevelUp = true;
		m_FillIndikatorSprite.fillAmount = 1f;
		SetNeededXP();
		float percentageOldLevel = 1f - m_OldPercentage;
		float percentageNewLevel = DIContainerInfrastructure.GetCurrentPlayer().Data.Experience / neededXPForLevelUp;
		float percentageSum = percentageOldLevel + percentageNewLevel;
		yield return StartCoroutine(ChangeValueOverTime(m_FillAnimationSprite, 1f, m_OldPercentage, 1f, TimeTillLeave * 0.25f));
		GetComponent<Animation>().Play("XPBar_LevelUp");
		m_OldPercentage = DIContainerInfrastructure.GetCurrentPlayer().Data.Experience / neededXPForLevelUp;
		m_FillIndikatorSprite.fillAmount = m_OldPercentage;
		m_FillAnimationSprite.fillAmount = 0f;
		m_LevelLabel.text = level.ToString();
		yield return new WaitForSeconds(GetComponent<Animation>()["XPBar_LevelUp"].length);
		yield return StartCoroutine(ChangeValueOverTime(m_FillAnimationSprite, 1f, 0f, m_OldPercentage, TimeTillLeave * 0.25f));
		GetComponent<Animation>().Play("Display_Top_Leave");
		m_OldValue = DIContainerInfrastructure.GetCurrentPlayer().Data.Experience;
		yield return new WaitForSeconds(GetComponent<Animation>()["Display_Top_Leave"].length);
		SetLeft();
		m_DoingLevelUp = false;
	}

	public void EnterUpdateAndLeave()
	{
		StartCoroutine(EnterUpdateAndLeaveCoroutine());
	}

	private IEnumerator EnterUpdateAndLeaveCoroutine()
	{
		GetComponent<Animation>().Play("Display_Top_Enter");
		yield return new WaitForSeconds(GetComponent<Animation>()["Display_Top_Enter"].length);
		bool levelUp = HadLevelUp;
		yield return new WaitForSeconds(UpdateAnim());
		if (!levelUp)
		{
			GetComponent<Animation>().Play("Display_Top_Leave");
		}
	}

	public void Enter()
	{
		if (!m_DoingLevelUp)
		{
			if (!IsEntering)
			{
				GetComponent<Animation>().Play("Display_Top_Enter");
				IsEntering = true;
				HasLeft = false;
			}
			if (!HadLevelUp)
			{
				CancelInvoke();
				Invoke("Leave", GetComponent<Animation>()["Display_Top_Enter"].length + TimeTillLeave);
			}
		}
	}

	public float GetEnterDuration()
	{
		return GetComponent<Animation>()["Display_Top_Enter"].length;
	}

	public float Leave()
	{
		GetComponent<Animation>().Play("Display_Top_Leave");
		Invoke("SetLeft", GetComponent<Animation>()["Display_Top_Leave"].length);
		return GetComponent<Animation>()["Display_Top_Leave"].length;
	}

	public float UpdateAnim()
	{
		UpdateAnimation.Play("Display_Update");
		if (HadLevelUp)
		{
			HadLevelUp = false;
			CancelInvoke();
			StartCoroutine(LevelUpCoroutine(Level));
			return GetComponent<Animation>()["XPBar_LevelUp"].length + GetComponent<Animation>()["Display_Top_Leave"].length;
		}
		StartCoroutine("UpdateXPBarValue");
		return DIContainerLogic.GetPacingBalancing().XpAndGoldBarStayDuration;
	}

	private IEnumerator ChangeValueOverTime(UISprite bar, float maxHealth, float oldValue, float newValue, float time)
	{
		float delta = newValue - oldValue;
		float absDelta = Mathf.Abs(delta);
		float step = delta / time;
		float absStep = Mathf.Abs(step);
		float timeLeft = time;
		float currentDelta = 0f;
		while (timeLeft > 0f)
		{
			yield return new WaitForEndOfFrame();
			timeLeft -= Time.deltaTime;
			bar.fillAmount = (oldValue + delta * Mathf.Min(1f, 1f - timeLeft / time)) / maxHealth;
		}
	}

	private IEnumerator UpdateXPBarValue()
	{
		if (!m_Updating && !m_DoingLevelUp)
		{
			m_Updating = true;
			float target = DIContainerInfrastructure.GetCurrentPlayer().Data.Experience;
			float targetPercent = target / neededXPForLevelUp;
			m_FillAnimationSprite.fillAmount = m_OldPercentage;
			m_FillIndikatorSprite.fillAmount = targetPercent;
			float currentPercent = m_OldPercentage;
			m_OldPercentage = targetPercent;
			m_OldValue = DIContainerInfrastructure.GetCurrentPlayer().Data.Experience;
			yield return StartCoroutine(ChangeValueOverTime(m_FillAnimationSprite, 1f, currentPercent, targetPercent, DIContainerLogic.GetPacingBalancing().XpAndGoldBarStayDuration));
			m_Updating = false;
		}
	}

	public bool IsPlayingLevelUp()
	{
		return GetComponent<Animation>().IsPlaying("XPBar_LevelUp");
	}

	public void SetLeft()
	{
		IsEntering = false;
	}

	internal void ResetXP()
	{
		Awake();
	}

	internal void RemoveEventHandlers()
	{
		if (DIContainerInfrastructure.GetCurrentPlayer() != null)
		{
			DIContainerInfrastructure.GetCurrentPlayer().CharacterLevelChanged -= LevelChangedHandler;
		}
	}
}
