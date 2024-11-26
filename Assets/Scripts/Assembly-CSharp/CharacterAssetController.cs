using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using SmoothMoves;
using UnityEngine;

public class CharacterAssetController : AssetControllerWithBoneAnimationBase
{
	protected ICharacter Model;

	public Transform MainHandBone;

	public Transform OffHandBone;

	public Transform HeadGearBone;

	public Transform BodyCenter;

	public Transform BodyRoot;

	public Vector3 HealthBarPosition;

	public Vector3 CombatTextPosition;

	public Vector3 BubblePosition;

	public Vector3 ColliderSize;

	public Vector3 ColliderOffset;

	public Vector2 MinMaxTimeForBlink = new Vector2(2f, 4f);

	public float OffsetFromEnemyX = 100f;

	public bool m_IsWorldMap = true;

	public bool m_IsIllusion;

	protected string TextureName = string.Empty;

	protected bool m_EquipmentSetOnce;

	public List<string> m_LoadingAtlasNames = new List<string>();

	private bool m_ShowEquipment = true;

	public bool m_IsLite;

	public AssetFaction m_AssetFaction;

	public virtual void SetModel(ICharacter model, bool isWorldMap, bool showEquipment = true, bool useScaleController = true, bool lightWeighted = false, bool showItems = true)
	{
		Behaviour[] components = GetComponents<Behaviour>();
		foreach (Behaviour behaviour in components)
		{
			behaviour.enabled = true;
		}
		m_IsLite = lightWeighted;
		m_ShowEquipment = showEquipment;
		Model = model;
		RegisterEventHandler();
		m_IsWorldMap = isWorldMap;
		if ((bool)base.transform.parent)
		{
			base.gameObject.layer = base.transform.parent.gameObject.layer;
		}
		if (!useScaleController)
		{
			base.gameObject.GetComponent<ScaleController>().enabled = false;
			base.transform.localScale = Vector3.one;
		}
		else
		{
			ScaleController component = base.gameObject.GetComponent<ScaleController>();
			if ((bool)component)
			{
				component.enabled = true;
			}
		}
		SetEquipment(!m_ShowEquipment, showItems);
		PlayAnimation("Idle");
		CancelInvoke();
		if (HasAnimation("Blink"))
		{
			Invoke("PlayBlinkAnimation", Random.Range(MinMaxTimeForBlink.x, MinMaxTimeForBlink.y));
		}
	}

	protected virtual void PlayBlinkAnimation()
	{
		if (IsPlayingAnimation("Idle") && !IsPlayingAnimation("Stunned"))
		{
			PlayAnimation("Blink");
		}
		Invoke("PlayBlinkAnimation", Random.Range(MinMaxTimeForBlink.x, MinMaxTimeForBlink.y));
	}

	protected virtual void InventoryGameData_InventoryOfTypeChanged(InventoryItemType type, IInventoryItemGameData item)
	{
		if (base.gameObject == null)
		{
			DeregisterEventHandler();
		}
		else if (base.gameObject.activeInHierarchy)
		{
			switch (type)
			{
			case InventoryItemType.Class:
			case InventoryItemType.Skin:
				SetEquipment(!m_ShowEquipment, true);
				break;
			case InventoryItemType.MainHandEquipment:
				SetEquipment(!m_ShowEquipment, true);
				break;
			case InventoryItemType.OffHandEquipment:
				SetEquipment(!m_ShowEquipment, true);
				break;
			case InventoryItemType.Resources:
			case InventoryItemType.Ingredients:
			case InventoryItemType.Consumable:
			case InventoryItemType.Premium:
			case InventoryItemType.Story:
			case InventoryItemType.PlayerToken:
			case InventoryItemType.Points:
			case InventoryItemType.PlayerStats:
			case InventoryItemType.CraftingRecipes:
			case InventoryItemType.EventBattleItem:
			case InventoryItemType.EventCollectible:
			case InventoryItemType.Mastery:
			case InventoryItemType.BannerTip:
			case InventoryItemType.Banner:
			case InventoryItemType.BannerEmblem:
			case InventoryItemType.EventCampaignItem:
			case InventoryItemType.EventBossItem:
			case InventoryItemType.CollectionComponent:
			case InventoryItemType.Trophy:
				break;
			}
		}
	}

	protected void OnDestroy()
	{
		DeregisterEventHandler();
	}

	private void OnDisable()
	{
		DeregisterEventHandler();
	}

	protected virtual void SetEquipment(bool isWorldMap, bool showItems)
	{
		if (base.HasNoBoneAnimation)
		{
			return;
		}
		if (isWorldMap && Model is BirdGameData)
		{
			m_BoneAnimation.HideBone("Headgear", true);
			if (m_BoneAnimation.GetBoneTransform("Headgear_Back") != null)
			{
				m_BoneAnimation.HideBone("Headgear_Back", true);
			}
			m_BoneAnimation.HideBone("MainHand", true);
			m_BoneAnimation.HideBone("OffHand", true);
		}
		else if (Model is PigGameData)
		{
			if (isWorldMap)
			{
				if ((bool)HeadGearBone && HeadGearBone.name.EndsWith("_A"))
				{
					SetEquipmentItem(Model.ClassItem, "Headgear_A", string.Empty);
					SetEquipmentItem(Model.ClassItem, "Headgear_Back_A", "_Back");
					if (m_BoneAnimation.GetBoneTransform("MainHand_A") != null)
					{
						m_BoneAnimation.HideBone("MainHand_A", true);
					}
					if (m_BoneAnimation.GetBoneTransform("OffHand_A") != null)
					{
						m_BoneAnimation.HideBone("OffHand_A", true);
					}
					SetEquipmentItem(Model.ClassItem, "Headgear_B", string.Empty);
					SetEquipmentItem(Model.ClassItem, "Headgear_Back_B", "_Back");
					if (m_BoneAnimation.GetBoneTransform("MainHand_B") != null)
					{
						m_BoneAnimation.HideBone("MainHand_B", true);
					}
					if (m_BoneAnimation.GetBoneTransform("OffHand_B") != null)
					{
						m_BoneAnimation.HideBone("OffHand_B", true);
					}
					SetEquipmentItem(Model.ClassItem, "Headgear_C", string.Empty);
					SetEquipmentItem(Model.ClassItem, "Headgear_Back_C", "_Back");
					if (m_BoneAnimation.GetBoneTransform("MainHand_C") != null)
					{
						m_BoneAnimation.HideBone("MainHand_C", true);
					}
					if (m_BoneAnimation.GetBoneTransform("OffHand_C") != null)
					{
						m_BoneAnimation.HideBone("OffHand_C", true);
					}
				}
				else
				{
					SetEquipmentItem(Model.ClassItem, "Headgear", string.Empty);
					SetEquipmentItem(Model.ClassItem, "Headgear_Back", "_Back");
					if (m_BoneAnimation.GetBoneTransform("MainHand") != null)
					{
						m_BoneAnimation.HideBone("MainHand", true);
					}
					if (m_BoneAnimation.GetBoneTransform("OffHand") != null)
					{
						m_BoneAnimation.HideBone("OffHand", true);
					}
				}
			}
			else if ((bool)HeadGearBone && HeadGearBone.name.EndsWith("_A"))
			{
				SetEquipmentItem(Model.ClassItem, "Headgear_A", string.Empty);
				SetEquipmentItem(Model.ClassItem, "Headgear_Back_A", "_Back");
				if (showItems)
				{
					SetEquipmentItem(Model.MainHandItem, "MainHand_A", string.Empty);
					SetEquipmentItem(Model.OffHandItem, "OffHand_A", string.Empty);
					if (m_BoneAnimation.GetBoneTransform("OffHand_Back_A") != null)
					{
						SetEquipmentItem(Model.OffHandItem, "OffHand_Back_A", "_Back");
					}
					SetEquipmentItem(Model.MainHandItem, "MainHand_B", string.Empty);
					SetEquipmentItem(Model.OffHandItem, "OffHand_B", string.Empty);
					if (m_BoneAnimation.GetBoneTransform("OffHand_Back_B") != null)
					{
						SetEquipmentItem(Model.OffHandItem, "OffHand_Back_B", "_Back");
					}
					SetEquipmentItem(Model.MainHandItem, "MainHand_C", string.Empty);
					SetEquipmentItem(Model.OffHandItem, "OffHand_C", string.Empty);
					if (m_BoneAnimation.GetBoneTransform("OffHand_Back_C") != null)
					{
						SetEquipmentItem(Model.OffHandItem, "OffHand_Back_C", "_Back");
					}
				}
				else
				{
					if (m_BoneAnimation.GetBoneTransform("MainHand") != null)
					{
						m_BoneAnimation.HideBone("MainHand", true);
					}
					if (m_BoneAnimation.GetBoneTransform("OffHand") != null)
					{
						m_BoneAnimation.HideBone("OffHand", true);
					}
				}
				SetEquipmentItem(Model.ClassItem, "Headgear_B", string.Empty);
				SetEquipmentItem(Model.ClassItem, "Headgear_Back_B", "_Back");
				SetEquipmentItem(Model.ClassItem, "Headgear_C", string.Empty);
				SetEquipmentItem(Model.ClassItem, "Headgear_Back_C", "_Back");
			}
			else
			{
				if ((Model as PigGameData).ClassSkin != null && Model.ClassItem.BalancingData.NameId == (Model as PigGameData).ClassSkin.BalancingData.OriginalClass)
				{
					SetEquipmentItem((Model as PigGameData).ClassSkin, "Headgear", string.Empty);
					SetEquipmentItem((Model as PigGameData).ClassSkin, "Headgear_Back", "_Back");
				}
				else
				{
					SetEquipmentItem(Model.ClassItem, "Headgear", string.Empty);
					SetEquipmentItem(Model.ClassItem, "Headgear_Back", "_Back");
				}
				if (showItems)
				{
					SetEquipmentItem(Model.MainHandItem, "MainHand", string.Empty);
					SetEquipmentItem(Model.OffHandItem, "OffHand", string.Empty);
					if (m_BoneAnimation.GetBoneTransform("OffHand_Back") != null)
					{
						SetEquipmentItem(Model.OffHandItem, "OffHand_Back", "_Back");
					}
					if (m_BoneAnimation.GetBoneTransform("MainHand_Arrow") != null)
					{
						SetEquipmentItem(Model.MainHandItem, "MainHand_Arrow", "_Arrow");
					}
				}
				else
				{
					if (m_BoneAnimation.GetBoneTransform("MainHand") != null)
					{
						m_BoneAnimation.HideBone("MainHand", true);
					}
					if (m_BoneAnimation.GetBoneTransform("OffHand") != null)
					{
						m_BoneAnimation.HideBone("OffHand", true);
					}
				}
			}
		}
		else
		{
			if (Model is BirdGameData && (Model as BirdGameData).ClassSkin != null && Model.ClassItem != null && Model.ClassItem.BalancingData.NameId == (Model as BirdGameData).ClassSkin.BalancingData.OriginalClass)
			{
				SetEquipmentItem((Model as BirdGameData).ClassSkin, "Headgear", string.Empty);
				SetEquipmentItem((Model as BirdGameData).ClassSkin, "Headgear_Back", "_Back");
			}
			else
			{
				SetEquipmentItem(Model.ClassItem, "Headgear", string.Empty);
				SetEquipmentItem(Model.ClassItem, "Headgear_Back", "_Back");
			}
			if (showItems)
			{
				SetEquipmentItem(Model.MainHandItem, "MainHand", string.Empty);
				SetEquipmentItem(Model.OffHandItem, "OffHand", string.Empty);
				if (m_BoneAnimation.GetBoneTransform("MainHand_Arrow") != null)
				{
					SetEquipmentItem(Model.MainHandItem, "MainHand_Arrow", "_Arrow");
				}
			}
			else
			{
				m_BoneAnimation.HideBone("MainHand", true);
				m_BoneAnimation.HideBone("OffHand", true);
			}
		}
		m_EquipmentSetOnce = true;
	}

	protected virtual void SetEquipmentItem(IInventoryItemGameData itemGameData, string boneName, string suffix = "")
	{
		if (base.HasNoBoneAnimation)
		{
			return;
		}
		AnimationBone animationBone = m_BoneAnimation.mBoneSource.FirstOrDefault((AnimationBone b) => b.boneName == boneName);
		if (animationBone == null)
		{
			return;
		}
		int boneIndex = animationBone.boneNodeIndex;
		TriggerFrame triggerFrame = m_BoneAnimation.triggerFrames.FirstOrDefault((TriggerFrame f) => f.GetTriggerFrameBone(boneIndex) != null && !string.IsNullOrEmpty(f.GetTriggerFrameBone(boneIndex).originalTextureGUID));
		if (triggerFrame == null)
		{
			DebugLog.Error("Original Texture is never set!");
			return;
		}
		TriggerFrameBone triggerFrameBone = triggerFrame.GetTriggerFrameBone(boneIndex);
		if (itemGameData == null || string.IsNullOrEmpty(itemGameData.ItemAssetName))
		{
			m_BoneAnimation.HideBone(boneName, true);
			return;
		}
		TextureAtlas textureAtlas = null;
		TextureAtlas[] textureAtlases = m_BoneAnimation.textureAtlases;
		foreach (TextureAtlas textureAtlas2 in textureAtlases)
		{
			if (textureAtlas2 == null)
			{
				Debug.LogError(string.Concat(GetType(), ": There is a null value referenced as a texture atlas on a BoneAnimation script!"), base.gameObject);
			}
			else if (textureAtlas2.textureNames.Contains(itemGameData.ItemAssetName + suffix))
			{
				textureAtlas = textureAtlas2;
				TextureName = textureAtlas.name;
			}
		}
		if (textureAtlas == null)
		{
			m_BoneAnimation.HideBone(boneName, true);
			return;
		}
		int num = -1;
		TextureAtlas textureAtlas3 = null;
		TextureAtlas[] textureAtlases2 = m_BoneAnimation.textureAtlases;
		foreach (TextureAtlas textureAtlas4 in textureAtlases2)
		{
			num = textureAtlas4.GetTextureIndex(triggerFrameBone.originalTextureGUID);
			if (num != -1)
			{
				textureAtlas3 = textureAtlas4;
				break;
			}
		}
		m_BoneAnimation.HideBone(boneName, false);
		if (m_EquipmentSetOnce)
		{
			m_BoneAnimation.RestoreBoneTexture(boneName);
		}
		m_BoneAnimation.SwapBoneTexture(boneName, textureAtlas3.name, textureAtlas3.textureNames[num], TextureName, itemGameData.ItemAssetName + suffix);
	}

	protected virtual void RegisterEventHandler()
	{
		DeregisterEventHandler();
		if (Model != null && Model.InventoryGameData != null)
		{
			Model.InventoryGameData.InventoryOfTypeChanged += InventoryGameData_InventoryOfTypeChanged;
		}
	}

	protected virtual void DeregisterEventHandler()
	{
		if (Model != null && Model.InventoryGameData != null)
		{
			Model.InventoryGameData.InventoryOfTypeChanged -= InventoryGameData_InventoryOfTypeChanged;
		}
	}

	public virtual void PlayDefeatAnim()
	{
		if (Model.CharacterFaction == Faction.Pigs)
		{
			base.gameObject.StopAnimationOrAnimatorState(new List<string> { "Idle", "Hit", "Affected", "Cheer" }, this);
		}
		else
		{
			base.gameObject.StopAnimationOrAnimatorState(new List<string> { "Idle", "Hit", "Cheer" }, this);
		}
		PlayAnimation("Defeated");
	}

	public virtual void PlayKnockoutAnim()
	{
		PlayAnimation("KnockedOut");
	}

	public virtual IEnumerator PlayDefeatAnimation()
	{
		yield return PlayAnimationAndWaitForFinish("Defeated");
	}

	public virtual IEnumerator PlayKnockOutAnimation()
	{
		yield return PlayAnimationAndWaitForFinish("KnockedOut");
	}

	public virtual float GetDefeatAnimationLength()
	{
		return GetAnimationLength("Defeated");
	}

	public virtual float GetKnockOutAnimationLength()
	{
		return GetAnimationLength("KnockedOut");
	}

	public virtual void PlayHitAnim()
	{
		if (this is BannerAssetController || this is BossAssetController || base.HasNoBoneAnimation || !m_BoneAnimation.IsPlaying("Action_Rage"))
		{
			PlayAnimation("Hit");
			PlayAnimationQueued("Idle");
		}
	}

	public virtual float GetHitAnimationLength()
	{
		return GetAnimationLength("Hit");
	}

	public virtual void PlayAffectedAnim()
	{
		PlayAnimation("Affected");
		PlayAnimationQueued("Idle");
	}

	public virtual void PlayIdleAnimationQueued()
	{
		PlayAnimationQueued("Idle");
	}

	public virtual float GetAffectedAnimationLength()
	{
		return GetAnimationLength("Affected");
	}

	public virtual void PlayCheerAnim()
	{
		if (this is BannerAssetController || this is BossAssetController || base.HasNoBoneAnimation || !m_BoneAnimation.IsPlaying("Defeated"))
		{
			PlayAnimation("Cheer");
			PlayAnimationQueued("Idle");
		}
	}

	public virtual float GetCheerAnimationLength()
	{
		return GetAnimationLength("Cheer");
	}

	public virtual void PlayAttackAnim(bool useOffhand)
	{
		if (Model.MainHandItem == null)
		{
			PlayAnimation("Action_Attack_1");
		}
		else
		{
			PlayAnimation("Action_Attack_" + ((!useOffhand) ? Model.MainHandItem.BalancingData.AnimationIndex : Model.OffHandItem.BalancingData.AnimationIndex));
		}
		PlayAnimationQueued("Idle");
	}

	public virtual void PlaySecondaryAttackAnim()
	{
		PlayAnimation("Action_Attack_4");
		PlayAnimationQueued("Idle");
	}

	public virtual void PlayFocusOffHandAnimation()
	{
		PlayAnimation("Focus_OffHand");
		PlayAnimationQueued("Idle");
	}

	public virtual void PlayFocusWeaponAnimation()
	{
		PlayAnimation("Focus_MainHand");
		PlayAnimationQueued("Idle");
	}

	public virtual float GetAttackAnimationLength()
	{
		if (Model.MainHandItem == null)
		{
			return GetAnimationLength("Action_Attack_1");
		}
		return GetAnimationLength("Action_Attack_" + Model.MainHandItem.BalancingData.AnimationIndex);
	}

	public virtual float GetSecondaryAttackAnimationLength()
	{
		return GetAnimationLength("Action_Attack_4");
	}

	public virtual void PlayFailAnim()
	{
		PlayAnimation("Action_Fail");
		PlayAnimationQueued("Idle");
	}

	public virtual float GetFailAnimationLength()
	{
		return GetAnimationLength("Action_Fail");
	}

	public virtual void PlaySupportAnim()
	{
		PlayAnimation("Action_Support");
		PlayAnimationQueued("Idle");
	}

	public virtual float GetSupportAnimationLength()
	{
		return GetAnimationLength("Action_Support");
	}

	public virtual void PlayLaughAnim()
	{
		PlayAnimation("Action_Laugh");
		PlayAnimationQueued("Idle");
	}

	public virtual float GetLaughAnimationLength()
	{
		return GetAnimationLength("Action_Laugh");
	}

	public virtual void PlayRageSkillAnim()
	{
		PlayAnimation("Action_Rage");
		PlayAnimationQueued("Idle");
	}

	public virtual float GetRageSkillAnimationLength()
	{
		return GetAnimationLength("Action_Rage");
	}

	public virtual void PlayRageAnim(bool isStunned)
	{
		PlayAnimation("Rage");
		if (isStunned)
		{
			PlayAnimationQueued("Stunned");
		}
		else
		{
			PlayAnimationQueued("Idle");
		}
	}

	public virtual float GetRageAnimationLength()
	{
		return GetAnimationLength("Rage");
	}

	public void PlaySuprisedAnim(bool isStunned)
	{
		PlayAnimation("Surprised");
		if (isStunned)
		{
			PlayAnimationQueued("Stunned");
		}
		else
		{
			PlayAnimationQueued("Idle");
		}
	}

	public float GetSuprisedAnimationLength()
	{
		return GetAnimationLength("Surprised");
	}

	public void SetModel(string characterId, bool isWorldMap, bool showEquipment, bool useScaleController)
	{
		ICharacter model = null;
		BirdBalancingData balancing;
		PigBalancingData balancing2;
		BossBalancingData balancing3;
		if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>(characterId, out balancing))
		{
			model = new BirdGameData(characterId);
		}
		else if (DIContainerBalancing.Service.TryGetBalancingData<PigBalancingData>(characterId, out balancing2))
		{
			model = new PigGameData(characterId).SetDifficulties(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, null);
		}
		else if (DIContainerBalancing.Service.TryGetBalancingData<BossBalancingData>(characterId, out balancing3))
		{
			model = new BossGameData(characterId).SetDifficulties(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, null);
		}
		SetModel(model, isWorldMap, showEquipment, useScaleController);
	}

	public virtual void PlayMournAnim()
	{
		PlayAnimation("Mourn");
		PlayAnimationQueued("Idle");
	}

	public float GetMournAnimationLength()
	{
		return GetAnimationLength("Mourn");
	}

	public void PlayTauntAnim()
	{
		PlayAnimation("Taunt");
		PlayAnimationQueued("Idle");
	}

	public float GetPlayTauntAnimationLength()
	{
		return GetAnimationLength("Taunt");
	}

	public void PlayIdleAnimation()
	{
		PlayAnimation("Idle");
		CancelInvoke();
		PlayAnimation("Blink");
		Invoke("PlayBlinkAnimation", Random.Range(MinMaxTimeForBlink.x, MinMaxTimeForBlink.y));
	}

	public virtual void PlayStunnedAnimation()
	{
		StopAnimation("Idle");
		CancelInvoke();
		PlayAnimationQueued("Stunned");
	}

	public virtual void PlayAffectedAnimQueued(string returnAnimation = "Idle")
	{
		StopAnimation(returnAnimation);
		PlayAnimationQueued("Affected");
		PlayAnimationQueued(returnAnimation);
	}

	public void PlayReviveAnim()
	{
		if (Model is BirdGameData || Model.IsPvPBird)
		{
			PlayAnimation("KnockedOutEnd");
		}
		else
		{
			PlayAnimation("Resurrection");
		}
		PlayAnimationQueued("Idle");
	}

	public float GetReviveAnimationLength()
	{
		return GetAnimationLength("Resurrection");
	}

	public void PlayAttentionAnimation()
	{
		PlayAnimation("Action_Attention");
		PlayAnimationQueued("Idle");
	}

	public float GetAttentionAnimationLength()
	{
		return GetAnimationLength("Action_Attention");
	}

	public void StopAnimations()
	{
		m_BoneAnimation.Stop();
	}
}
