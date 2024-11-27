using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

public class CharacterControlHUD : MonoBehaviour
{
	private ICombatant m_Source;

	private ICombatant m_Target;

	private Vector3 m_TargetPos;

	[SerializeField]
	private Transform m_RotationControl;

	[SerializeField]
	private Transform m_BaseLine;

	[SerializeField]
	private Transform m_OrginMarker;

	[SerializeField]
	private Transform m_Marker;

	[SerializeField]
	private Material m_baseMaterial;

	[SerializeField]
	private Color m_supportColor;

	[SerializeField]
	private Color m_attackColor;

	[SerializeField]
	private Color m_neutraColor;

	private Material m_groupSharedMaterial;

	private Quaternion m_BaseRotation;

	[SerializeField]
	private float m_LineOffsetToMarker = 90f;

	[SerializeField]
	private float m_LineOffsetToOrigin = 60f;

	[SerializeField]
	private float m_MaxLineOvalCutoff = 20f;

	private CharacterControllerBattleGroundBase m_CachedTargetCharacter;

	private CharacterControllerBattleGroundBase m_SourceCharacter;

	private Quaternion m_MarkerInitialRotation;

	private Vector3 targetPos;

	private bool m_Initialized;

	private void Awake()
	{
		m_BaseRotation = m_RotationControl.rotation;
		m_groupSharedMaterial = new Material(m_baseMaterial);
		m_groupSharedMaterial.name = m_groupSharedMaterial.name + "_" + base.gameObject.name;
		m_MarkerInitialRotation = m_Marker.rotation;
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.sharedMaterial = m_groupSharedMaterial;
		}
	}

	public CharacterControlHUD SetCharacter(CharacterControllerBattleGround character)
	{
		m_SourceCharacter = character;
		m_Initialized = true;
		return this;
	}

	private void LateUpdate()
	{
		if (m_Initialized)
		{
			base.transform.position = m_SourceCharacter.transform.position;
		}
	}

	public void SetState(Transform sourceTransform, CharacterControllerBattleGroundBase target, Vector3 targetP, BattleMgrBase battleMgr, bool isInvalidTarget = false)
	{
		bool flag = target != null;
		if ((bool)m_CachedTargetCharacter && m_CachedTargetCharacter != target && (bool)m_CachedTargetCharacter.GetControlHUD() && !battleMgr.CharacterIsActing(m_CachedTargetCharacter.GetModel().CombatantNameId))
		{
			m_CachedTargetCharacter.GetControlHUD().gameObject.SetActive(!m_CachedTargetCharacter.GetModel().ActedThisTurn);
		}
		m_CachedTargetCharacter = target;
		Faction faction = ((!flag) ? Faction.None : target.GetModel().CombatantFaction);
		Vector3 position = sourceTransform.position;
		targetPos = ((!flag) ? targetP : target.transform.position);
		if (!flag && targetPos.y > battleMgr.m_UpperBorder.position.y)
		{
			targetPos = new Vector3(targetPos.x, Mathf.Min(targetPos.y, battleMgr.m_UpperBorder.position.y), Mathf.Min(targetPos.z, battleMgr.m_UpperBorder.position.z));
		}
		if (faction == Faction.Birds && !isInvalidTarget)
		{
			m_groupSharedMaterial.color = m_supportColor;
		}
		else if (faction == Faction.Pigs && !isInvalidTarget)
		{
			m_groupSharedMaterial.color = m_attackColor;
		}
		else
		{
			m_groupSharedMaterial.color = m_neutraColor;
		}
		Vector3 right = sourceTransform.right;
		Vector3 to = targetPos - sourceTransform.position;
		bool flag2 = flag && sourceTransform == target.transform;
		m_BaseLine.gameObject.SetActive(!flag2);
		m_OrginMarker.gameObject.SetActive(!flag2);
		m_Marker.localPosition = new Vector3(to.magnitude, 0f, 0f);
		m_Marker.rotation = m_MarkerInitialRotation;
		m_RotationControl.localRotation = Quaternion.Euler(new Vector3(m_RotationControl.localRotation.eulerAngles.x, Vector3.Angle(right, to) * (float)((targetPos.z <= sourceTransform.position.z) ? 1 : (-1))));
		m_BaseLine.localPosition = new Vector3(Mathf.Max(25f, m_MaxLineOvalCutoff * (1f - GetOvalCutoff(m_RotationControl.rotation.eulerAngles.y))), 0f, 0f);
		m_BaseLine.localScale = new Vector3(to.magnitude - m_LineOffsetToOrigin - m_LineOffsetToMarker - 1f * (m_MaxLineOvalCutoff * (1f - GetOvalCutoff(m_RotationControl.rotation.eulerAngles.y))), 1f, 1f);
		if ((bool)target && (bool)target.GetControlHUD() && target.GetControlHUD() != this) //&& target.GetModel().ActedThisTurn)
		{
			target.GetControlHUD().gameObject.SetActive(false);
		}
	}

	public float GetOvalCutoff(float eulerAngle)
	{
		if (eulerAngle <= 90f)
		{
			return eulerAngle / 90f;
		}
		if (eulerAngle <= 180f)
		{
			return (90f - (eulerAngle - 90f)) / 90f;
		}
		if (eulerAngle <= 270f)
		{
			return (eulerAngle - 180f) / 90f;
		}
		return (90f - (eulerAngle - 270f)) / 90f;
	}

	public void ResetControlHUD()
	{
		m_BaseLine.gameObject.SetActive(false);
		m_OrginMarker.gameObject.SetActive(false);
		m_BaseLine.localScale = new Vector3(0f, 1f, 1f);
		m_groupSharedMaterial.color = m_neutraColor;
		m_Marker.localPosition = Vector3.zero;
		m_Marker.rotation = m_MarkerInitialRotation;
		if ((bool)m_CachedTargetCharacter && (bool)m_CachedTargetCharacter.GetControlHUD() && !m_CachedTargetCharacter.m_BattleMgr.CharacterIsActing(m_CachedTargetCharacter.GetModel().CombatantNameId))
		{
			m_CachedTargetCharacter.GetControlHUD().gameObject.SetActive(!m_CachedTargetCharacter.GetModel().ActedThisTurn);
		}
		m_CachedTargetCharacter = null;
	}
}
