using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using SmoothMoves;
using UnityEngine;

public class CharacterControllerCamp : MonoBehaviour
{
	[HideInInspector]
	public CharacterAssetController m_AssetController;

	private BoxCollider m_BoxCollider;

	private bool m_TooltipTapBegan;

	private ICharacter m_Model;

	private Transform m_CachedTransform;

	public bool m_ShowEquipment = true;

	[SerializeField]
	private UITapHoldTrigger m_TapHoldTrigger;

	[SerializeField]
	private CHMotionTween m_MotionTween;

	private bool m_UseScaleController = true;

	private bool m_IsTapping;

	private bool m_IsLite;

	[SerializeField]
	private GameObject m_updateIndikatorPrefab;

	[SerializeField]
	private Vector3 m_updateIndicatorPos = new Vector3(60f, -10f, -5f);

	private GameObject m_updateIndikator;

	[method: MethodImpl(32)]
	public event Action<ICombatant> Clicked;

	[method: MethodImpl(32)]
	public event Action<ICharacter> BirdClicked;

	private void OnTapReleased()
	{
		DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.HideCharacterOverlay();
		m_IsTapping = false;
	}

	private void OnTapEnd()
	{
	}

	private void OnTapBegin()
	{
		DebugLog.Log("OnTapBegin");
		if (m_Model is BirdGameData)
		{
			BirdCombatant birdCombatant = new BirdCombatant(m_Model as BirdGameData).SetPvPBird(DIContainerInfrastructure.GetCoreStateMgr().m_IsWithinPvP);
			Dictionary<string, float> currentStatBuffs = new Dictionary<string, float>();
			birdCombatant.CurrentStatBuffs = currentStatBuffs;
			birdCombatant.RefreshHealth();
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowCharacterOverlay(m_AssetController.BodyCenter, birdCombatant, base.gameObject.layer == LayerMask.NameToLayer("Interface") || base.gameObject.layer == LayerMask.NameToLayer("InterfaceCharacter"), false);
		}
		else if (m_Model is BannerGameData)
		{
			BannerCombatant bannerCombatant = new BannerCombatant(m_Model as BannerGameData);
			Dictionary<string, float> currentStatBuffs2 = new Dictionary<string, float>();
			bannerCombatant.CurrentStatBuffs = currentStatBuffs2;
			bannerCombatant.RefreshHealth();
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowCharacterOverlay(m_AssetController.BodyCenter, bannerCombatant, base.gameObject.layer == LayerMask.NameToLayer("Interface") || base.gameObject.layer == LayerMask.NameToLayer("InterfaceCharacter"), false);
		}
		else
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowCharacterOverlay(m_AssetController.BodyCenter, new PigCombatant(m_Model as PigGameData), base.gameObject.layer == LayerMask.NameToLayer("Interface"), false);
		}
		m_IsTapping = true;
	}

	public void SetModel(ICharacter model, bool useScaleController = true, bool isLightWeight = true, bool showItems = true)
	{
		m_IsLite = isLightWeight;
		base.gameObject.name = model.Name;
		m_Model = model;
		m_CachedTransform = base.transform;
		m_UseScaleController = useScaleController;
		RecreateAssetController(showItems);
		RegisterEventHandler();
		if (GetComponentInChildren<AudioSource>()) GetComponentInChildren<AudioSource>().spatialBlend = 0f; // fix sound bug
	}

	public void PositionBubble(CharacterControllerCamp character, GameObject speechBubble)
	{
		speechBubble.transform.parent = character.transform;
		speechBubble.transform.localScale = Vector3.one / character.m_Model.Scale;
		speechBubble.transform.localPosition = Vector3.Scale(Vector3.Scale(character.m_AssetController.BubblePosition, m_AssetController.transform.localScale), base.transform.localScale);
		speechBubble.transform.localScale = new Vector3((float)Math.Sign(speechBubble.transform.lossyScale.x) * speechBubble.transform.localScale.x, speechBubble.transform.localScale.x, speechBubble.transform.localScale.z);
		speechBubble.transform.localRotation = Quaternion.identity;
	}

	public void PositionComparisionBubble(CharacterControllerCamp character, GameObject speechBubble)
	{
		Vector3 a = new Vector3(0f, character.m_AssetController.BubblePosition.y, -5f);
		speechBubble.transform.localPosition = Vector3.Scale(Vector3.Scale(a, m_AssetController.transform.lossyScale), base.transform.localScale);
		speechBubble.transform.localRotation = Quaternion.identity;
	}

	public void SetModel(string modelID, bool useScaleController = true, bool isLightWeight = true)
	{
		m_IsLite = isLightWeight;
		ICharacter model = null;
		BirdBalancingData balancing;
		PigBalancingData balancing2;
		BossBalancingData balancing3;
		if (DIContainerBalancing.Service.TryGetBalancingData<BirdBalancingData>(modelID, out balancing))
		{
			model = new BirdGameData(modelID);
		}
		else if (DIContainerBalancing.Service.TryGetBalancingData<PigBalancingData>(modelID, out balancing2))
		{
			model = new PigGameData(modelID).SetDifficulties(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, null);
		}
		else if (DIContainerBalancing.Service.TryGetBalancingData<BossBalancingData>(modelID, out balancing3))
		{
			model = new BossGameData(modelID).SetDifficulties(DIContainerInfrastructure.GetCurrentPlayer().Data.Level, null);
		}
		SetModel(model, useScaleController);
		if (GetComponentInChildren<AudioSource>()) GetComponentInChildren<AudioSource>().spatialBlend = 0f; // fix sound bug
	}

	public ICharacter GetModel()
	{
		return m_Model;
	}

	public void UserTrigger(UserTriggerEvent triggerEvent)
	{
	}

	public void DestroyCharacter()
	{
		DestroyAsset();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void DestroyAsset()
	{
		if ((bool)m_AssetController)
		{
			GenericAssetProvider genericAssetProvider = null;
			genericAssetProvider = ((!(m_Model is BannerGameData)) ? DIContainerInfrastructure.GetCharacterAssetProvider(false) : DIContainerInfrastructure.GetBannerAssetProvider());
			genericAssetProvider.DestroyObject(m_Model.AssetName, m_AssetController.gameObject);
			m_AssetController = null;
		}
	}

	private void OnDestroy()
	{
		DeregisterEventHandler();
		if ((bool)m_AssetController)
		{
			GenericAssetProvider genericAssetProvider = null;
			genericAssetProvider = ((!(m_Model is BannerGameData)) ? DIContainerInfrastructure.GetCharacterAssetProvider(false) : DIContainerInfrastructure.GetBannerAssetProvider());
			genericAssetProvider.DestroyObject(m_Model.AssetName, m_AssetController.gameObject);
		}
		if (m_IsTapping)
		{
			OnTapReleased();
			if ((bool)m_TapHoldTrigger)
			{
				m_TapHoldTrigger.ResetUICamera();
			}
		}
	}

	public void RecreateAssetController(bool showItems)
	{
		if (m_Model != null && m_AssetController == null)
		{
			InstantiateAsset(showItems);
		}
		else
		{
			m_AssetController.SetModel(m_Model, false, m_ShowEquipment, m_UseScaleController, false, showItems);
		}
		Invoke("ReSizeCollider", 1f);
	}

	private void InstantiateAsset(bool showItems)
	{
		GenericAssetProvider genericAssetProvider = null;
		genericAssetProvider = ((!(m_Model is BannerGameData)) ? DIContainerInfrastructure.GetCharacterAssetProvider(false) : DIContainerInfrastructure.GetBannerAssetProvider());
		m_AssetController = genericAssetProvider.InstantiateObject(m_Model.AssetName, base.transform, Vector3.zero, Quaternion.identity, !(m_Model is BannerGameData)).GetComponent<CharacterAssetController>();
		m_AssetController.SetModel(m_Model, false, m_ShowEquipment, m_UseScaleController, m_IsLite, showItems);
		Vector3 vector = Vector3.one;
		if (AssetIsOnWrongSide(m_AssetController.m_AssetFaction, m_Model.CharacterFaction))
		{
			vector = new Vector3(-1f, 1f, 1f);
		}
		base.transform.localScale = vector * m_Model.Scale;
	}

	private bool AssetIsOnWrongSide(AssetFaction assetFaction, Faction faction)
	{
		switch (assetFaction)
		{
		case AssetFaction.Pig:
			return faction == Faction.Birds;
		case AssetFaction.Bird:
			return faction == Faction.Pigs;
		default:
			return false;
		}
	}

	private void ReSizeCollider()
	{
		if ((bool)m_AssetController)
		{
			if (!m_BoxCollider)
			{
				m_BoxCollider = GetComponent<Collider>().GetComponent<BoxCollider>();
			}
			m_BoxCollider.size = Vector3.Scale(m_AssetController.ColliderSize, m_AssetController.transform.localScale);
			m_BoxCollider.center = Vector3.Scale(m_AssetController.ColliderOffset, m_AssetController.transform.localScale);
		}
	}

	private float PlayAffectedAnim()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayAffectedAnim();
			return m_AssetController.GetAffectedAnimationLength();
		}
		return 0f;
	}

	private float PlayHitAnim()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayHitAnim();
			return m_AssetController.GetHitAnimationLength();
		}
		return 0f;
	}

	private void RegisterEventHandler()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.m_BoneAnimation.RegisterUserTriggerDelegate(UserTrigger);
		}
		m_TapHoldTrigger.OnTapBegin += OnTapBegin;
		m_TapHoldTrigger.OnTapEnd += OnTapEnd;
		m_TapHoldTrigger.OnTapReleased += OnTapReleased;
	}

	public void DeregisterEventHandler()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.m_BoneAnimation.RegisterUserTriggerDelegate(UserTrigger);
		}
		if ((bool)m_TapHoldTrigger)
		{
			m_TapHoldTrigger.OnTapBegin -= OnTapBegin;
			m_TapHoldTrigger.OnTapEnd -= OnTapEnd;
			m_TapHoldTrigger.OnTapReleased -= OnTapReleased;
		}
	}

	private void OnTouchClicked()
	{
		HandleClicked();
	}

	private void HandleClicked()
	{
		DebugLog.Log("Character Clicked");
		if (m_TooltipTapBegan)
		{
			m_TooltipTapBegan = false;
			return;
		}
		ShowNewMarker(false);
		if (this.BirdClicked != null)
		{
			this.BirdClicked(m_Model);
		}
	}

	public float PlayDefeatCharacter()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayDefeatAnim();
			return m_AssetController.GetDefeatAnimationLength();
		}
		return 0f;
	}

	public float PlayCheerCharacter()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayCheerAnim();
			return m_AssetController.GetCheerAnimationLength();
		}
		return 0f;
	}

	public float PlayKnockOutCharacter()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayKnockoutAnim();
			return m_AssetController.GetKnockOutAnimationLength();
		}
		return 0f;
	}

	public float PlayAttackAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayAttackAnim(false);
			return m_AssetController.GetAttackAnimationLength();
		}
		return 0f;
	}

	public float PlayTumbledAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayFailAnim();
			return m_AssetController.GetFailAnimationLength();
		}
		return 0f;
	}

	public float PlaySupportAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlaySupportAnim();
			return m_AssetController.GetSupportAnimationLength();
		}
		return 0f;
	}

	public float PlayLaughAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayLaughAnim();
			return m_AssetController.GetLaughAnimationLength();
		}
		return 0f;
	}

	public float PlayRageSkillAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayRageSkillAnim();
			return m_AssetController.GetRageSkillAnimationLength();
		}
		return 0f;
	}

	public float PlayRageAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayRageAnim(false);
			return m_AssetController.GetRageAnimationLength();
		}
		return 0f;
	}

	public float PlayMourneAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayMournAnim();
			return m_AssetController.GetMournAnimationLength();
		}
		return 0f;
	}

	public float PlayTauntAnimation()
	{
		if ((bool)m_AssetController)
		{
			m_AssetController.PlayTauntAnim();
			return m_AssetController.GetPlayTauntAnimationLength();
		}
		return 0f;
	}

	public void DisableTabAndHold()
	{
		if ((bool)m_TapHoldTrigger)
		{
			m_TapHoldTrigger.OnTapBegin -= OnTapBegin;
			m_TapHoldTrigger.OnTapEnd -= OnTapEnd;
			m_TapHoldTrigger.OnTapReleased -= OnTapReleased;
			m_TapHoldTrigger.enabled = false;
		}
	}

	public void ShowNewMarker(bool show)
	{
		DebugLog.Log("Show new Marker on Bird: " + m_Model.Name + ":  " + show);
		if ((bool)m_updateIndikator)
		{
			m_updateIndikator.SetActive(show);
		}
		else if (show)
		{
			m_updateIndikator = UnityEngine.Object.Instantiate(m_updateIndikatorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			m_updateIndikator.transform.parent = base.transform;
			m_updateIndikator.transform.localPosition = m_updateIndicatorPos;
			m_updateIndikator.transform.position = new Vector3(m_updateIndikator.transform.position.x, m_updateIndikator.transform.position.y, m_updateIndicatorPos.z);
			if (base.transform.name.Contains("banner"))
			{
				m_updateIndikator.transform.localPosition = new Vector3(m_updateIndikator.transform.localPosition.x, 170f, -60f);
			}
		}
	}

	public void PlayIdleAnimation()
	{
		if (m_AssetController != null)
		{
			m_AssetController.PlayIdleAnimation();
		}
	}
}
