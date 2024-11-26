using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using ABH.GameDatas.Battle;
using ABH.GameDatas.Interfaces;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;
using UnityEngine;

public class ConsumableBattleButtonController : MonoBehaviour
{
	private ConsumableItemBalancingData m_Model;

	[SerializeField]
	private UISprite m_ConsumableIconSprite;

	[SerializeField]
	private GameObject m_AmountRoot;

	[SerializeField]
	private UILabel m_AmountText;

	[SerializeField]
	public UIDragTrigger m_Drag;

	[SerializeField]
	private Transform m_DraggableTransform;

	private Transform m_DraggableTransformClone;

	[SerializeField]
	private ResourceCostBlind m_ResourceCost;

	private BasicShopOfferBalancingData m_instantBuyOffer;

	private IInventoryItemGameData m_consumable;

	private BattleMgrBase m_battleMgr;

	private int m_level;

	private bool m_startDrag;

	private GlowController m_CurrentGlow;

	private Transform m_cachedOverCharacterTransform;

	private CharacterControllerBattleGroundBase m_cachedOverCharacter;

	public Transform DraggableClone
	{
		get
		{
			return m_DraggableTransformClone;
		}
	}

	[method: MethodImpl(32)]
	public event Action ButtonClicked;

	public string getConsumableName()
	{
		if (m_consumable != null)
		{
			return m_consumable.Name;
		}
		return m_instantBuyOffer.NameId;
	}

	public string getConsumableNameId()
	{
		if (m_consumable != null)
		{
			return m_consumable.Name;
		}
		return m_instantBuyOffer.OfferContents.Keys.FirstOrDefault();
	}

	public void SetModel(ConsumableItemBalancingData model, BattleMgrBase battleMgr, int level)
	{
		m_Model = model;
		m_level = level;
		m_battleMgr = battleMgr;
		base.gameObject.name = model.SortPriority.ToString("00") + base.gameObject.name;
		m_ConsumableIconSprite.spriteName = model.AssetBaseId;
		m_instantBuyOffer = null;
		m_consumable = null;
		int num = 0;
		if (DIContainerLogic.InventoryService.TryGetItemGameData(m_battleMgr.Model.m_ControllerInventory, model.NameId, out m_consumable))
		{
			num = m_consumable.ItemValue;
		}
		if (num == 0)
		{
			m_instantBuyOffer = DIContainerLogic.GetShopService().GetInstantOffersForCategoryAndLevel(model.InstantBuyOfferCategoryId, level);
		}
		else
		{
			m_instantBuyOffer = null;
		}
		m_AmountText.text = "x" + num.ToString("0");
		m_AmountRoot.SetActive(true);
		if (m_instantBuyOffer == null)
		{
			m_ResourceCost.gameObject.SetActive(false);
			if (num == 0)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
		}
		else
		{
			List<Requirement> buyResourcesRequirements = DIContainerLogic.GetShopService().GetBuyResourcesRequirements(m_battleMgr.Model.m_ControllerLevel, m_instantBuyOffer);
			if (buyResourcesRequirements.Count <= 0)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			m_ResourceCost.gameObject.SetActive(true);
			m_AmountRoot.SetActive(false);
			Requirement requirement = buyResourcesRequirements[0];
			m_ResourceCost.SetModel(DIContainerBalancing.GetInventoryItemBalancingDataPovider().GetBalancingData(requirement.NameId).AssetBaseId, null, requirement.Value, string.Empty);
		}
		RegisterEventHandlers();
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		if ((bool)m_Drag)
		{
			m_Drag.onPress += DragOnPress;
			m_Drag.onRelease += DragOnRelease;
			m_Drag.onDrag += DragOnDrag;
		}
	}

	private void DeRegisterEventHandlers()
	{
		if ((bool)m_Drag)
		{
			m_Drag.onPress -= DragOnPress;
			m_Drag.onRelease -= DragOnRelease;
			m_Drag.onDrag -= DragOnDrag;
		}
	}

	public void ShowTooltip()
	{
		if (m_consumable != null)
		{
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(base.transform, m_consumable, true, m_battleMgr.Model.IsPvP);
		}
		else if (m_instantBuyOffer != null)
		{
			List<IInventoryItemGameData> shopOfferContent = DIContainerLogic.GetShopService().GetShopOfferContent(DIContainerInfrastructure.GetCurrentPlayer(), m_instantBuyOffer, DIContainerLogic.GetSalesManagerService().GetOfferSaleDetails(m_instantBuyOffer.NameId));
			DIContainerInfrastructure.GetCoreStateMgr().m_InfoOverlays.ShowItemOverlay(base.transform, shopOfferContent.FirstOrDefault(), true, m_battleMgr.Model.IsPvP);
		}
	}

	private void DisableGlow()
	{
		m_CurrentGlow.gameObject.SetActive(false);
	}

	private IEnumerator DelayedConsumableClicked(ConsumableItemGameData model, float p, ICombatant combatant)
	{
		yield return new WaitForSeconds(p);
		ConsumableClickedForCombatant(model, combatant);
	}

	private bool ConsumableUseOnCharacterPossible(ICombatant combatant)
	{
		if (!combatant.CanUseConsumable)
		{
			DebugLog.Log("Already used Consumable this turn");
			return false;
		}
		if (m_battleMgr.AnyCharacterIsActingOrInQueue())
		{
			DebugLog.Log("Character is currently acting");
			return false;
		}
		return true;
	}

	public void ConsumableClickedForCombatant(ConsumableItemGameData consumable, ICombatant combatant, bool leaveBar = true)
	{
		if (consumable == null)
		{
			DebugLog.Log("No Consumable Item Clicked");
			return;
		}
		DebugLog.Log("Consumable Item Clicked: " + consumable.Name);
		if (!combatant.CanUseConsumable)
		{
			DebugLog.Log("Already used Consumable this turn");
			return;
		}
		if (leaveBar)
		{
			m_battleMgr.m_BattleUI.LeaveConsumableBar();
		}
		combatant.CombatantView.ActivateConsumableSkill(consumable);
	}

	public void ConsumableClicked(ConsumableItemGameData consumable, bool leaveBar = true)
	{
		ConsumableClickedForCombatant(consumable, m_battleMgr.Model.CurrentCombatant, leaveBar);
	}

	private void OnDestroy()
	{
		DeRegisterEventHandlers();
	}

	private void DragOnDrag(GameObject arg1, Vector2 arg2)
	{
		OnDragging(Input.mousePosition);
	}

	public void OnDragging(Vector3 dragPos)
	{
		if (m_battleMgr.m_LockControlHUDs || !m_startDrag)
		{
			return;
		}
		if (m_CurrentGlow == null)
		{
			m_CurrentGlow = m_battleMgr.m_CurrentGlow;
			m_CurrentGlow.gameObject.SetActive(false);
		}
		m_battleMgr.m_BattleUI.m_ConsumableBar.SetBackgroundButtonActive(false);
		m_battleMgr.LockDragVisualizationByCode = true;
		Vector3 vector = m_battleMgr.m_InterfaceCamera.ScreenToWorldPoint(dragPos);
		m_DraggableTransformClone.position = new Vector3(vector.x, vector.y, m_DraggableTransformClone.position.z);
		Ray ray = m_battleMgr.m_SceneryCamera.ScreenPointToRay(dragPos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, (1 << LayerMask.NameToLayer("TutorialScenery")) | (1 << LayerMask.NameToLayer("Scenery"))))
		{
			if (m_cachedOverCharacterTransform != hitInfo.transform)
			{
				m_cachedOverCharacterTransform = hitInfo.transform;
				CharacterControllerBattleGroundBase cachedOverCharacter = m_cachedOverCharacter;
				m_cachedOverCharacter = m_cachedOverCharacterTransform.GetComponent<CharacterControllerBattleGroundBase>();
				if ((bool)m_cachedOverCharacter && !m_cachedOverCharacter.GetModel().CanUseConsumable)
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

	private void DragOnRelease(GameObject obj)
	{
		OnDragRelease(Input.mousePosition);
	}

	public void OnDragRelease(Vector3 releasePos)
	{
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
		Ray ray = m_battleMgr.m_SceneryCamera.ScreenPointToRay(releasePos);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, 10000f, (!DIContainerInfrastructure.TutorialMgr.IsCurrentlyLocked) ? (1 << LayerMask.NameToLayer("Scenery")) : (1 << LayerMask.NameToLayer("TutorialScenery"))))
		{
			CancelConsumableMovement();
			if (m_startDrag)
			{
				m_battleMgr.m_BattleUI.m_ConsumableBar.SetBackgroundButtonActive(true);
			}
			return;
		}
		DebugLog.Log("RayCastHit! " + hitInfo.transform.gameObject.name);
		if (m_startDrag)
		{
			m_battleMgr.m_BattleUI.m_ConsumableBar.SetBackgroundButtonActive(true);
		}
		CharacterControllerBattleGroundBase component = hitInfo.transform.GetComponent<CharacterControllerBattleGroundBase>();
		if (!component || !component.GetModel().IsParticipating || !ConsumableUseOnCharacterPossible(component.GetModel()))
		{
			m_battleMgr.m_DraggedCharacter = null;
			CancelConsumableMovement();
			return;
		}
		DebugLog.Log("Targeted Character: " + component.GetModel().CombatantName);
		if (m_instantBuyOffer != null)
		{
			List<Requirement> failed = new List<Requirement>();
			if (!DIContainerLogic.GetShopService().IsOfferBuyable(DIContainerInfrastructure.GetCurrentPlayer(), m_instantBuyOffer, out failed))
			{
				m_startDrag = false;
				DebugLog.Error("Failed to Buy Consumable!");
				Requirement requirement = failed.FirstOrDefault((Requirement r) => r.RequirementType == RequirementType.PayItem);
				if (requirement != null && requirement.RequirementType == RequirementType.PayItem)
				{
					IInventoryItemGameData data = null;
					if (DIContainerLogic.InventoryService.TryGetItemGameData(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, requirement.NameId, out data) && data.ItemBalancing.NameId == "lucky_coin")
					{
						m_battleMgr.m_BattleUI.m_ConsumableBar.m_LuckyCoinsController.SwitchToShop();
					}
				}
				m_battleMgr.m_DraggedCharacter = null;
				CancelConsumableMovement();
				return;
			}
		}
		if (m_instantBuyOffer != null)
		{
			if (m_instantBuyOffer.NameId.Contains("healing_all"))
			{
				bool flag = false;
				foreach (ICombatant item in m_battleMgr.Model.m_CombatantsPerFaction[Faction.Birds].Where((ICombatant c) => c.IsParticipating))
				{
					if (item.CurrentHealth < item.ModifiedHealth)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					CancelConsumableMovement();
					return;
				}
			}
			else if (m_instantBuyOffer.NameId.Contains("heal") && component.GetModel().CurrentHealth == component.GetModel().ModifiedHealth)
			{
				CancelConsumableMovement();
				return;
			}
			if (m_instantBuyOffer.NameId.Contains("rage") && component.m_BattleMgr.Model.m_CurrentRage >= 100f)
			{
				CancelConsumableMovement();
				return;
			}
			if (m_instantBuyOffer.NameId.Contains("purify"))
			{
				bool flag2 = false;
				foreach (BattleEffectGameData value in component.GetModel().CurrrentEffects.Values)
				{
					if (value.m_EffectType == SkillEffectTypes.Curse)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					CancelConsumableMovement();
					return;
				}
			}
			List<Requirement> failed2 = new List<Requirement>();
			if (DIContainerLogic.GetShopService().IsOfferBuyable(DIContainerInfrastructure.GetCurrentPlayer(), m_instantBuyOffer, out failed2))
			{
				DIContainerLogic.GetShopService().BuyShopOffer(DIContainerInfrastructure.GetCurrentPlayer(), m_instantBuyOffer, "buyConsumable_instant");
				SetModel(m_Model, m_battleMgr, m_level);
				DIContainerLogic.InventoryService.TryGetItemGameData(m_battleMgr.Model.m_ControllerInventory, m_Model.NameId, out m_consumable);
				ConsumableClickedForCombatant(m_consumable as ConsumableItemGameData, component.GetModel(), false);
				Invoke("LeaveConsumableBar", DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateCoinsBar());
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateCoinsBar();
				DIContainerInfrastructure.GetCoreStateMgr().m_GenericUI.UpdateLuckyCoinsBar();
				m_battleMgr.m_BattleUI.m_ConsumableBar.UpdateCoinValues();
			}
		}
		else if (m_consumable != null)
		{
			if ((m_consumable as ConsumableItemGameData).BalancingData.NameId.Contains("healing_all"))
			{
				bool flag3 = false;
				foreach (ICombatant item2 in m_battleMgr.Model.m_CombatantsPerFaction[Faction.Birds].Where((ICombatant c) => c.IsParticipating))
				{
					if (item2.CurrentHealth < item2.ModifiedHealth)
					{
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					CancelConsumableMovement();
					return;
				}
			}
			else if ((m_consumable as ConsumableItemGameData).BalancingData.NameId.Contains("heal") && component.GetModel().CurrentHealth == component.GetModel().ModifiedHealth)
			{
				CancelConsumableMovement();
				return;
			}
			if ((m_consumable as ConsumableItemGameData).BalancingData.NameId.Contains("rage") && component.m_BattleMgr.Model.m_CurrentRage >= 100f)
			{
				CancelConsumableMovement();
				return;
			}
			if ((m_consumable as ConsumableItemGameData).BalancingData.NameId.Contains("purify"))
			{
				bool flag4 = false;
				foreach (BattleEffectGameData value2 in component.GetModel().CurrrentEffects.Values)
				{
					if (value2.m_EffectType == SkillEffectTypes.Curse)
					{
						flag4 = true;
						break;
					}
				}
				if (!flag4)
				{
					CancelConsumableMovement();
					return;
				}
			}
			ConsumableClickedForCombatant(m_consumable as ConsumableItemGameData, component.GetModel());
		}
		if ((bool)m_DraggableTransformClone)
		{
			m_DraggableTransformClone.GetComponent<Animation>().Play("Consumable_Drop");
			UnityEngine.Object.Destroy(m_DraggableTransformClone.gameObject, m_DraggableTransformClone.GetComponent<Animation>()["Consumable_Drop"].length);
		}
	}

	private void CancelConsumableMovement()
	{
		if ((bool)m_DraggableTransformClone)
		{
			CHMotionTween component = m_DraggableTransformClone.GetComponent<CHMotionTween>();
			m_DraggableTransformClone.GetComponent<Animation>().Play("Consumable_Drop");
			component.m_EndTransform = m_DraggableTransform;
			component.m_DurationInSeconds = m_DraggableTransformClone.GetComponent<Animation>()["Consumable_Drop"].length;
			component.Play();
			UnityEngine.Object.Destroy(m_DraggableTransformClone.gameObject, component.m_DurationInSeconds);
			m_battleMgr.m_LockControlHUDs = true;
			Invoke("AllowControlHUDs", component.m_DurationInSeconds);
		}
	}

	private void AllowControlHUDs()
	{
		m_battleMgr.m_LockControlHUDs = false;
	}

	private void LeaveConsumableBar()
	{
		m_battleMgr.m_BattleUI.LeaveConsumableBar();
	}

	public void ResetDraggedPosition()
	{
		if ((bool)m_DraggableTransformClone)
		{
			UnityEngine.Object.Destroy(m_DraggableTransformClone.gameObject);
		}
	}

	private void OnDisable()
	{
		if ((bool)m_DraggableTransformClone)
		{
			UnityEngine.Object.Destroy(m_DraggableTransformClone.gameObject);
		}
	}

	public void DragOnPress(GameObject obj)
	{
		if (m_battleMgr.m_LockControlHUDs)
		{
			return;
		}
		m_startDrag = true;
		if (m_startDrag)
		{
			Ray ray = m_battleMgr.m_SceneryCamera.ScreenPointToRay(m_battleMgr.m_InterfaceCamera.WorldToScreenPoint(m_Drag.transform.position));
			m_DraggableTransformClone = UnityEngine.Object.Instantiate(m_DraggableTransform, m_DraggableTransform.position, Quaternion.identity) as Transform;
			if (m_DraggableTransformClone != null)
			{
				m_DraggableTransformClone.Translate(0f, 0f, -10f);
				m_DraggableTransformClone.GetComponent<Animation>().Play("Consumable_Take");
			}
			DebugLog.Log("RageOMeter Ray: " + ray);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("Scenery")))
			{
				m_battleMgr.m_CurrentConsumableControlHUDRoot.transform.position = hitInfo.point;
			}
			List<ICombatant> list = m_battleMgr.Model.m_CombatantsByInitiative.Where((ICombatant c) => c.CombatantFaction == Faction.Birds && !c.CanUseConsumable).ToList();
			for (int i = 0; i < list.Count; i++)
			{
				ICombatant combatant = list[i];
				m_battleMgr.m_CharacterInteractionBlockedItems[i].SetActive(true);
				m_battleMgr.m_CharacterInteractionBlockedItems[i].transform.position = combatant.CombatantView.m_AssetController.BodyCenter.position + new Vector3(0f, 0f, -5f);
			}
		}
	}
}
