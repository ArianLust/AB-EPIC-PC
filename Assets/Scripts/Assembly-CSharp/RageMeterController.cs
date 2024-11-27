using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Services.Logic;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class RageMeterController : MonoBehaviour
{
	public UISprite RageMeterProgress;

	public Animation RageUpdateAnimations;

	private BattleMgrBase m_battleMgr;

	public Transform m_RageEffectTransform;

	public UIDragTrigger m_DragTrigger;

	private GlowController m_CurrentGlow;

	private Camera m_sceneryCamera;

	private Transform m_cachedOverCharacterTransform;

	private CharacterControllerBattleGroundBase m_cachedOverCharacter;

	[SerializeField]
	private Transform m_DraggedChili;

	[SerializeField]
	private GameObject m_GoldBody;

	[SerializeField]
	private GameObject m_GoldBodyDragging;

	public GameObject m_RageMeterPosInScene;

	private Camera m_interfaceCamera;

	[SerializeField]
	private Transform m_DraggableTransform;

	private CHMotionTween m_DraggableTransformMotionTween;

	[SerializeField]
	private Transform m_ResetingTransform;

	[SerializeField]
	private Transform m_ChiliAnimationTransform;

	public bool m_Changing;

	private bool m_dragging;

	private bool m_firstEnter;

	private bool m_goldChiliActive;

	public Transform DraggedChili
	{
		get
		{
			return m_DraggedChili;
		}
	}

	public void SetBattleMgr(BattleMgrBase battleMgr)
	{
		m_battleMgr = battleMgr;
		m_sceneryCamera = Camera.allCameras.FirstOrDefault((Camera c) => c.CompareTag("SceneryCamera"));
		m_interfaceCamera = Camera.allCameras.FirstOrDefault((Camera c) => !c.CompareTag("SceneryCamera"));
	}

	public void Enter()
	{
		base.gameObject.SetActive(true);
		if ((bool)m_DraggableTransformMotionTween)
		{
			m_DraggableTransformMotionTween = m_DraggableTransform.GetComponent<CHMotionTween>();
		}
		StartCoroutine(EnterCoroutine());
	}

	public IEnumerator EnterCoroutine()
	{
		GetComponent<Animation>().Play("RageOMeter_Enter");
		if (!m_Changing)
		{
			RageMeterProgress.fillAmount = m_battleMgr.Model.m_CurrentRage / 100f;
			if (!m_firstEnter)
			{
				m_GoldBody.SetActive(RageMeterProgress.fillAmount >= 1f);
				m_GoldBodyDragging.SetActive(RageMeterProgress.fillAmount >= 1f);
				m_firstEnter = true;
				m_goldChiliActive = RageMeterProgress.fillAmount >= 1f;
			}
		}
		m_Changing = false;
		yield return new WaitForSeconds(GetComponent<Animation>()["RageOMeter_Enter"].length);
		if (m_battleMgr.Model.m_CurrentRage >= 100f && !m_battleMgr.m_IsRagedBlocked)
		{
			RageUpdateAnimations.Play("RageOMeter_Ready_Start");
			RageUpdateAnimations.PlayQueued("RageOMeter_Ready_Loop");
			RegisterEventHandlers();
		}
		else
		{
			RageUpdateAnimations.Play("RageOMeter_Start");
			DeRegisterEventHandlers();
		}
	}

	public IEnumerator Leave()
	{
		if (RageMeterProgress.fillAmount >= 1f)
		{
			RageUpdateAnimations.Play("RageOMeter_Start");
		}
		DeRegisterEventHandlers();
		GetComponent<Animation>().Play("RageOMeter_Leave");
		yield return new WaitForSeconds(GetComponent<Animation>()["RageOMeter_Leave"].length);
	}

	public void OnRageMeterIncreased(float value, ICombatant source, bool isAttacking, SkillBattleDataBase skill, Vector3 targetOffset)
	{
		if (!(RageMeterProgress.fillAmount >= 1f))
		{
			StartCoroutine(SpawnFX(value, source, isAttacking, skill, targetOffset));
		}
	}

	private IEnumerator SpawnFX(float value, ICombatant source, bool isAttacking, SkillBattleDataBase skill, Vector3 targetOffset)
	{
		int j = 0;
		float oldValue = m_battleMgr.Model.m_CurrentRage - value;
		for (float i = 0f; i < value; i += 3f)
		{
			Transform rageFX = null;
			if (!isAttacking || skill == null || skill.m_Targets == null)
			{
				rageFX = (Transform)Object.Instantiate(m_RageEffectTransform, new Vector3(source.CombatantView.transform.position.x, source.CombatantView.transform.position.y, 0f), Quaternion.identity);
				rageFX.GetComponent<RageMeterFXMovement>().Init(this, source, targetOffset);
			}
			else if (skill.Model.SkillParameters.ContainsKey("all") && skill.m_Targets[j] != null)
			{
				if (skill.m_Targets[j].CombatantView != null)
				{
					rageFX = (Transform)Object.Instantiate(m_RageEffectTransform, new Vector3(skill.m_Targets[j].CombatantView.transform.position.x, skill.m_Targets[j].CombatantView.transform.position.y, 0f), Quaternion.identity);
					rageFX.GetComponent<RageMeterFXMovement>().Init(this, skill.m_Targets[j], targetOffset);
				}
				j++;
				if (j >= skill.m_Targets.Count)
				{
					j = 0;
				}
			}
			else if (source.AttackTarget.CombatantView != null)
			{
				rageFX = (Transform)Object.Instantiate(m_RageEffectTransform, new Vector3(source.AttackTarget.CombatantView.transform.position.x, source.AttackTarget.CombatantView.transform.position.y, 0f), Quaternion.identity);
				rageFX.GetComponent<RageMeterFXMovement>().Init(this, source.AttackTarget, targetOffset);
			}
			if (rageFX == null)
			{
				break;
			}
			rageFX.eulerAngles = new Vector3(rageFX.eulerAngles.x, rageFX.eulerAngles.y, Random.Range(0f, 360f));
			if (i == 0f)
			{
				yield return new WaitForEndOfFrame();
				StartCoroutine(UpdateRageMeterDelayed(rageFX.GetComponent<RageMeterFXMovement>().GetFlyTime(), oldValue, m_battleMgr.Model.m_CurrentRage));
			}
			yield return new WaitForSeconds(Random.Range(0.01f, 0.15f));
		}
	}

	private IEnumerator UpdateRageMeterDelayed(float delay, float oldValue, float newValue)
	{
		yield return new WaitForSeconds(delay);
		StartCoroutine(UpdateRageMeter(oldValue, newValue));
	}

	public IEnumerator UpdateRageMeter(float oldValue, float newValue)
	{
		if (!GetComponent<Animation>().IsPlaying("RageOMeter_Used"))
		{
			if (oldValue < newValue)
			{
				RageUpdateAnimations.Play("RageOMeter_RageGained");
				yield return StartCoroutine(ChangeValueOverTime(RageMeterProgress, 100f, oldValue, newValue, RageUpdateAnimations["RageOMeter_RageGained"].length));
			}
			if (oldValue > newValue)
			{
				RageUpdateAnimations.Play("RageOMeter_RageLost");
				yield return StartCoroutine(ChangeValueOverTime(RageMeterProgress, 100f, oldValue, newValue, RageUpdateAnimations["RageOMeter_RageLost"].length));
			}
			if (m_battleMgr.Model.m_CurrentRage >= 100f && !m_battleMgr.m_IsRagedBlocked)
			{
				RageUpdateAnimations.Play("RageOMeter_Ready_Start");
				RageUpdateAnimations.PlayQueued("RageOMeter_Ready_Loop");
				RegisterEventHandlers();
			}
			else
			{
				RageUpdateAnimations.Play("RageOMeter_Start");
				DeRegisterEventHandlers();
			}
		}
	}

	private IEnumerator ChangeValueOverTime(UISprite bar, float maxHealth, float oldValue, float newValue, float time)
	{
		m_Changing = true;
		float delta = newValue - oldValue;
		float timeLeft = time;
		while (timeLeft > 0f)
		{
			yield return new WaitForEndOfFrame();
			timeLeft -= Time.deltaTime;
			float fill = (oldValue + delta * Mathf.Min(1f, 1f - timeLeft / time)) / maxHealth;
			bar.fillAmount = fill;
		}
		m_Changing = false;
	}

	public void OnRageUsed(float value, ICombatant source)
	{
		float num = m_battleMgr.Model.m_CurrentRage + value;
	}

	public void OnRageDecreasedByOpponent(float value, ICombatant opponent)
	{
		float oldValue = m_battleMgr.Model.m_CurrentRage + value;
		StartCoroutine(UpdateRageMeter(oldValue, m_battleMgr.Model.m_CurrentRage));
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		m_DragTrigger.onDrag += m_DragTrigger_onDrag;
		m_DragTrigger.onDrop += m_DragTrigger_onDrop;
		m_DragTrigger.onRelease += m_DragTrigger_onRelease;
		m_DragTrigger.onPress += m_DragTrigger_onPress;
	}

	private void DeRegisterEventHandlers()
	{
		m_DragTrigger.onDrag -= m_DragTrigger_onDrag;
		m_DragTrigger.onDrop -= m_DragTrigger_onDrop;
		m_DragTrigger.onRelease -= m_DragTrigger_onRelease;
		m_DragTrigger.onPress -= m_DragTrigger_onPress;
	}

	private void DisableGlow()
	{
		m_CurrentGlow.gameObject.SetActive(false);
	}

	public void m_DragTrigger_onPress(GameObject obj)
	{
		if (!m_battleMgr.m_LockControlHUDs && m_battleMgr.Model.IsRageFull(Faction.Birds) && m_battleMgr.IsRagemeterUsePossible)
		{
			m_dragging = true;
			m_GoldBody.SetActive(false);
			m_DraggedChili.gameObject.SetActive(true);
			m_DraggedChili.GetComponent<Animation>().Play("DraggedChili_Show");
			RageMeterProgress.fillAmount = 0f;
			m_DraggedChili.position = m_ResetingTransform.position;
			List<ICombatant> list = m_battleMgr.Model.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Birds && !c.IsRageAvailiable).ToList();
			for (int i = 0; i < list.Count; i++)
			{
				ICombatant combatant = list[i];
				m_battleMgr.m_CharacterInteractionBlockedItems[i].SetActive(true);
				m_battleMgr.m_CharacterInteractionBlockedItems[i].transform.position = combatant.CombatantView.m_AssetController.BodyCenter.position + new Vector3(0f, 0f, -5f);
			}
			m_ChiliAnimationTransform.localPosition = Vector3.zero;
			m_ChiliAnimationTransform.localEulerAngles = Vector3.zero;
			RageUpdateAnimations.Play("RageOMeter_PickedUp");
		}
	}

	private void m_DragTrigger_onRelease(GameObject obj)
	{
		OnRelease(Input.mousePosition);
	}

	public void OnRelease(Vector3 pos)
	{
		if (!m_dragging)
		{
			return;
		}
		m_dragging = false;
		if (m_battleMgr.m_LockControlHUDs)
		{
			return;
		}
		foreach (GameObject characterInteractionBlockedItem in m_battleMgr.m_CharacterInteractionBlockedItems)
		{
			characterInteractionBlockedItem.SetActive(false);
		}
		if ((bool)m_CurrentGlow)
		{
			m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Hide");
			Invoke("DisableGlow", m_CurrentGlow.GetComponent<Animation>()["CharacterSelectionGlow_Hide"].length);
		}
		m_battleMgr.LockDragVisualizationByCode = false;
		Ray ray = m_sceneryCamera.ScreenPointToRay(pos);
		DeRegisterEventHandlers();
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, 10000f, (!DIContainerInfrastructure.TutorialMgr.IsCurrentlyLocked) ? (1 << LayerMask.NameToLayer("Scenery")) : (1 << LayerMask.NameToLayer("TutorialScenery"))))
		{
			if ((bool)m_DraggableTransform)
			{
				CHMotionTween component = m_DraggedChili.GetComponent<CHMotionTween>();
				component.m_EndTransform = m_ResetingTransform;
				component.Play();
				Invoke("PlayRageReady", component.m_DurationInSeconds);
			}
			m_GoldBody.SetActive(m_goldChiliActive);
			return;
		}
		DebugLog.Log("RayCastHit! " + hitInfo.transform.gameObject.name);
		CharacterControllerBattleGroundBase component2 = hitInfo.transform.GetComponent<CharacterControllerBattleGroundBase>();
		if (!component2 || !component2.GetModel().IsParticipating || !m_battleMgr.IsRagemeterUsePossible)
		{
			m_battleMgr.m_DraggedCharacter = null;
			if ((bool)m_DraggableTransform)
			{
				CHMotionTween component3 = m_DraggedChili.GetComponent<CHMotionTween>();
				component3.m_EndTransform = m_ResetingTransform;
				component3.Play();
				Invoke("PlayRageReady", component3.m_DurationInSeconds);
			}
			m_GoldBody.SetActive(m_goldChiliActive);
		}
		else
		{
			DebugLog.Log("Targeted Character: " + component2.GetModel().CombatantName);
			ActivateRageSkillForCombatantAndResetRageMeter(component2.GetModel());
		}
	}

	public void ActivateRageSkillForCombatantAndResetRageMeter(ICombatant combatant)
	{
		if (ActivateRageSkillForCombatant(combatant))
		{
			if ((bool)m_DraggableTransform)
			{
				StartCoroutine(ResetRageMeterAfterUse());
			}
			return;
		}
		if ((bool)m_DraggableTransform)
		{
			CHMotionTween component = m_DraggedChili.GetComponent<CHMotionTween>();
			component.m_EndTransform = m_ResetingTransform;
			component.Play();
			Invoke("PlayRageReady", component.m_DurationInSeconds);
		}
		m_GoldBody.SetActive(m_goldChiliActive);
	}

	private bool UseRageIsPossible(ICombatant combatant)
	{
		if (combatant is PigCombatant || !combatant.StartedHisTurn || combatant.IsStunned || m_battleMgr.AnyCharacterIsActingOrInQueue() || m_battleMgr.m_BattleMainLoopDone || combatant.ActedThisTurn || m_battleMgr.WaveEnded || m_battleMgr.IsBattleEnded())
		{
			return false;
		}
		return true;
	}

	public IEnumerator ResetRageMeterAfterUse()
	{
		m_DraggedChili.GetComponent<Animation>().Play("DraggedChili_Used");
		RageMeterProgress.fillAmount = 0f;
		m_goldChiliActive = false;
		Invoke("PlayRageReady", m_DraggedChili.GetComponent<Animation>()["DraggedChili_Used"].length);
		yield return new WaitForSeconds(0.25f);
		m_GoldBody.SetActive(false);
		m_GoldBodyDragging.SetActive(false);
		m_DraggedChili.position = m_ResetingTransform.position;
		m_ChiliAnimationTransform.localPosition = Vector3.zero;
		m_ChiliAnimationTransform.localEulerAngles = Vector3.zero;
	}

	private void PlayRageReady()
	{
		m_DraggedChili.gameObject.SetActive(false);
		if (m_battleMgr.Model.m_CurrentRage >= 100f && !m_battleMgr.m_IsRagedBlocked)
		{
			RageUpdateAnimations.Play("RageOMeter_Ready_Start");
			RageUpdateAnimations.PlayQueued("RageOMeter_Ready_Loop");
			RageMeterProgress.fillAmount = m_battleMgr.Model.m_CurrentRage;
			RegisterEventHandlers();
		}
		else
		{
			RageUpdateAnimations.Play("RageOMeter_Start");
			DeRegisterEventHandlers();
		}
	}

	private void m_DragTrigger_onDrop(GameObject draggedObject, GameObject target)
	{
		DebugLog.Log("Dropped: " + draggedObject.name + " on " + ((!(target != null)) ? "nothing" : target.name));
	}

	public void OnDrag(Vector3 dragPos)
	{
		if (m_battleMgr.m_LockControlHUDs || !m_battleMgr.IsRagemeterUsePossible)
		{
			return;
		}
		m_dragging = true;
		if (m_CurrentGlow == null)
		{
			m_CurrentGlow = m_battleMgr.m_CurrentGlow;
			m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Show");
			m_CurrentGlow.gameObject.SetActive(false);
		}
		m_battleMgr.LockDragVisualizationByCode = true;
		Vector3 vector = m_battleMgr.m_InterfaceCamera.ScreenToWorldPoint(dragPos);
		m_dragging = true;
		m_DraggedChili.position = new Vector3(vector.x, vector.y, m_DraggableTransform.position.z);
		Ray ray = m_sceneryCamera.ScreenPointToRay(dragPos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 10000f, (1 << LayerMask.NameToLayer("TutorialScenery")) | (1 << LayerMask.NameToLayer("Scenery"))))
		{
			if (m_cachedOverCharacterTransform != hitInfo.transform)
			{
				m_cachedOverCharacterTransform = hitInfo.transform;
				CharacterControllerBattleGroundBase cachedOverCharacter = m_cachedOverCharacter;
				m_cachedOverCharacter = m_cachedOverCharacterTransform.GetComponent<CharacterControllerBattleGroundBase>();
				if ((bool)m_cachedOverCharacter && !m_cachedOverCharacter.GetModel().IsRageAvailiable)
				{
					m_cachedOverCharacter = null;
				}
				if (cachedOverCharacter != m_cachedOverCharacter)
				{
					if (m_cachedOverCharacter != null && m_cachedOverCharacter.gameObject.layer != ((!DIContainerInfrastructure.TutorialMgr.IsCurrentlyLocked) ? LayerMask.NameToLayer("Scenery") : LayerMask.NameToLayer("TutorialScenery")))
					{
						m_cachedOverCharacter = null;
					}
					if (m_cachedOverCharacter == null)
					{
						m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Hide");
						Invoke("DisableGlow", m_CurrentGlow.GetComponent<Animation>()["CharacterSelectionGlow_Hide"].length);
					}
					else
					{
						m_CurrentGlow.gameObject.SetActive(true);
						m_CurrentGlow.SetStateColor(GlowState.Neutral);
						CancelInvoke("DisableGlow");
						m_CurrentGlow.GetComponent<Animation>().Play("CharacterSelectionGlow_Show");
					}
				}
			}
			if ((bool)m_cachedOverCharacter)
			{
				float num = 1f;
				switch (m_cachedOverCharacter.GetModel().CharacterModel.CharacterSize)
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
				m_CurrentGlow.transform.localScale = m_cachedOverCharacter.transform.localScale * num / m_cachedOverCharacter.GetModel().CharacterModel.Scale;
				m_CurrentGlow.transform.position = m_cachedOverCharacter.m_AssetController.BodyCenter.position;
			}
			int layer = hitInfo.transform.gameObject.layer;
		}
		else
		{
			m_CurrentGlow.gameObject.SetActive(false);
		}
	}

	private void m_DragTrigger_onDrag(GameObject draggedObject, Vector2 delta)
	{
		OnDrag(Input.mousePosition);
	}

	public void ActivateRageSkill()
	{
		ActivateRageSkillForCombatant(m_battleMgr.Model.CurrentCombatant);
	}

	private bool ActivateRageSkillForCombatant(ICombatant combatant)
	{
		DebugLog.Log("Rage Skill Button Pressed");
		if (!combatant.CombatantView.m_IsWaitingForInput || !UseRageIsPossible(combatant))
		{
			DebugLog.Log("No bird on turn");
			return false;
		}
		if (m_battleMgr.Model.m_CurrentRage < 100f)
		{
			DIContainerLogic.GetBattleService().LogDebug("Not enough rage: " + m_battleMgr.Model.m_CurrentRage, BattleLogTypes.Rage);
			return false;
		}
		foreach (BattleEffectGameData value in combatant.CurrrentEffects.Values)
		{
			foreach (BattleEffect effect in value.m_Effects)
			{
				if (effect.EffectType == BattleEffectType.RageBlocked)
				{
					DebugLog.Log("Rage ability blocked by enemy skill");
					RageUpdateAnimations.Play("RageOMeter_Blocked_Start");
					return false;
				}
			}
		}
		combatant.CombatantView.ActivateRageSkill();
		return true;
	}
}
