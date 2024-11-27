using System.Collections;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.Generic;
using UnityEngine;

public class CharacterContainerAssetController : CharacterAssetController
{
	[SerializeField]
	private string m_ContainedCharacterId;

	[SerializeField]
	private string m_ContainedCharacterAssetId;

	[SerializeField]
	private string m_ForeGroundId;

	[SerializeField]
	public Transform m_ContainedCharacterRoot;

	[SerializeField]
	public Transform m_ForeGroundRoot;

	private CharacterAssetController m_ContainedCharacter;

	private GameObject m_ForeGround;

	protected override void DeregisterEventHandler()
	{
	}

	protected override void RegisterEventHandler()
	{
	}

	public override float GetAffectedAnimationLength()
	{
		return 0f;
	}

	public override float GetAttackAnimationLength()
	{
		return GetAnimationLength("Action_Attention");
	}

	public override float GetCheerAnimationLength()
	{
		return 0f;
	}

	public override float GetDefeatAnimationLength()
	{
		return 100f;
	}

	public override float GetFailAnimationLength()
	{
		return 0f;
	}

	public override float GetHitAnimationLength()
	{
		return GetAnimationLength("Hit");
	}

	public override float GetKnockOutAnimationLength()
	{
		return GetAnimationLength("KnockOut");
	}

	public override float GetLaughAnimationLength()
	{
		return GetAnimationLength("Action_Attention");
	}

	public override float GetRageAnimationLength()
	{
		return 0f;
	}

	public override float GetRageSkillAnimationLength()
	{
		return 0f;
	}

	public override float GetSupportAnimationLength()
	{
		return 0f;
	}

	protected override void InventoryGameData_InventoryOfTypeChanged(InventoryItemType type, IInventoryItemGameData item)
	{
		base.InventoryGameData_InventoryOfTypeChanged(type, item);
	}

	public override void PlayAffectedAnim()
	{
	}

	public override void PlayAttackAnim(bool useOffhand)
	{
		PlayAnimation("Action_Attention");
		PlayAnimationQueued("Idle");
	}

	protected override void PlayBlinkAnimation()
	{
		if (GetComponent<Animation>().IsPlaying("Idle") && !GetComponent<Animation>().IsPlaying("Stunned"))
		{
			PlayAnimation("Blink");
		}
		Invoke("PlayBlinkAnimation", Random.Range(MinMaxTimeForBlink.x, MinMaxTimeForBlink.y));
	}

	public override void PlayCheerAnim()
	{
	}

	public override void PlayDefeatAnim()
	{
		PlayAnimation("Defeated");
	}

	public override IEnumerator PlayDefeatAnimation()
	{
		yield return PlayAnimationAndWaitForFinish("Defeated");
	}

	public override void PlayFailAnim()
	{
	}

	public override void PlayIdleAnimationQueued()
	{
		PlayAnimationQueued("Idle");
	}

	public override void PlayHitAnim()
	{
		PlayAnimation("Hit");
		PlayAnimationQueued("Idle");
	}

	public override void PlayKnockoutAnim()
	{
		PlayAnimation("KnockOut");
	}

	public override IEnumerator PlayKnockOutAnimation()
	{
		yield return PlayAnimationAndWaitForFinish("KnockOut");
	}

	public override void PlayLaughAnim()
	{
		PlayAnimation("Action_Attention");
		PlayAnimationQueued("Idle");
	}

	public override void PlayRageAnim(bool isStunned)
	{
	}

	public override void PlayRageSkillAnim()
	{
	}

	public override void PlaySupportAnim()
	{
	}

	protected override void SetEquipment(bool isWorldMap, bool showItems)
	{
		base.SetEquipment(isWorldMap, showItems);
	}

	protected override void SetEquipmentItem(IInventoryItemGameData itemGameData, string boneName, string suffix = "")
	{
		base.SetEquipmentItem(itemGameData, boneName, suffix);
	}

	public override void SetModel(ICharacter model, bool isWorldMap, bool showEquipment = true, bool useScaleController = true, bool isLite = true, bool showItem = true)
	{
		Model = model;
		m_IsWorldMap = isWorldMap;
		base.gameObject.layer = base.transform.parent.gameObject.layer;
		RegisterEventHandler();
		if (!m_ContainedCharacter)
		{
			GameObject gameObject = DIContainerInfrastructure.GetCharacterAssetProvider(isWorldMap).InstantiateObject(m_ContainedCharacterAssetId, m_ContainedCharacterRoot, Vector3.zero, Quaternion.identity);
			m_ContainedCharacter = gameObject.GetComponent<CharacterAssetController>();
			m_ContainedCharacter.transform.Translate(0f, 0f, -1f);
		}
		m_ContainedCharacter.SetModel(m_ContainedCharacterId, isWorldMap, showEquipment, false);
		m_ContainedCharacter.PlayIdleAnimation();
		if (!m_ForeGround)
		{
			m_ForeGround = DIContainerInfrastructure.GetCharacterAssetProvider(isWorldMap).InstantiateObject(m_ForeGroundId, m_ForeGroundRoot, Vector3.zero, Quaternion.identity);
		}
		PlayAnimation("Idle");
		CancelInvoke();
		Invoke("PlayBlinkAnimation", Random.Range(MinMaxTimeForBlink.x, MinMaxTimeForBlink.y));
	}
}
