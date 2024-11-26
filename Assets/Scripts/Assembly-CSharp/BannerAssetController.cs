using System.Collections.Generic;
using ABH.GameDatas;
using ABH.GameDatas.Interfaces;
using UnityEngine;

public class BannerAssetController : CharacterAssetController
{
	[SerializeField]
	public Transform m_BannerBaseRoot;

	[SerializeField]
	public Transform m_BannerTipRoot;

	[SerializeField]
	public Transform m_BannerFlagRoot;

	private GameObject m_tip;

	private GameObject m_center;

	private GameObject m_emblem;

	private GameObject m_base;

	private UiSortBehaviour m_uiSortingBehaviour;

	private void Start()
	{
		if ((bool)GetComponent<Animator>())
		{
			GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)Object.Instantiate(Resources.Load("Content/4_Characters/8_Base/6_Banner/Banner"));
		}
		if (base.transform.parent != null)
		{
			m_uiSortingBehaviour = base.transform.parent.GetComponentInParent<UiSortBehaviour>();
		}
	}

	public override float GetAttackAnimationLength()
	{
		return 0f;
	}

	public override void PlayAttackAnim(bool useOffhand)
	{
	}

	protected override void PlayBlinkAnimation()
	{
	}

	public override void PlayHitAnim()
	{
		base.gameObject.PlayAnimationOrAnimatorStateQueued(new List<string> { "Hit", "Idle" }, this);
	}

	public override void PlayAffectedAnim()
	{
		base.gameObject.PlayAnimationOrAnimatorStateQueued(new List<string> { "Affected", "Idle" }, this);
	}

	protected override void SetEquipment(bool isWorldMap, bool showItems)
	{
		m_EquipmentSetOnce = true;
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
		if ((bool)m_emblem)
		{
			m_emblem.transform.parent = null;
			Object.Destroy(m_emblem, 0.01f);
		}
		if ((bool)m_tip)
		{
			m_tip.transform.parent = null;
			Object.Destroy(m_tip, 0.01f);
		}
		if ((bool)m_center)
		{
			m_center.transform.parent = null;
			Object.Destroy(m_center, 0.01f);
		}
		if ((bool)m_base)
		{
			m_base.transform.parent = null;
			Object.Destroy(m_base, 0.01f);
		}
		if (model is BannerGameData)
		{
			BannerGameData bannerGameData = model as BannerGameData;
			if (bannerGameData.BannerTip != null)
			{
				m_tip = DIContainerInfrastructure.GetBannerAssetProvider().InstantiateObject(bannerGameData.BannerTip.BalancingData.AssetBaseId, m_BannerTipRoot, Vector3.zero, Quaternion.identity);
				if (m_tip != null)
				{
					m_tip.transform.localScale = Vector3.one;
				}
			}
			if (bannerGameData.BannerCenter != null)
			{
				m_center = DIContainerInfrastructure.GetBannerAssetProvider().InstantiateObject(bannerGameData.BannerCenter.BalancingData.AssetBaseId, m_BannerFlagRoot, Vector3.zero, Quaternion.identity);
				if ((bool)m_center)
				{
					BannerFlagAssetController component = m_center.GetComponent<BannerFlagAssetController>();
					m_center.transform.localScale = Vector3.one;
					m_center.name = "Banner_Flag";
					if ((bool)component)
					{
						if (bannerGameData.BannerEmblem != null)
						{
							m_emblem = DIContainerInfrastructure.GetBannerAssetProvider().InstantiateObject(bannerGameData.BannerEmblem.BalancingData.AssetBaseId, component.m_BannerEmblemRoot, Vector3.zero, Quaternion.identity);
							if ((bool)m_emblem)
							{
								m_emblem.name = "Banner_Emblem";
								m_emblem.transform.localScale = Vector3.one;
								BannerEmblemAssetController component2 = m_emblem.GetComponent<BannerEmblemAssetController>();
								if (component2 != null)
								{
									component2.SetColors(GetColorFromList(bannerGameData.BannerEmblem.BalancingData.ColorVector));
								}
							}
						}
						m_base = DIContainerInfrastructure.GetBannerAssetProvider().InstantiateObject(component.m_BannerBaseAssetName, m_BannerBaseRoot, Vector3.zero, Quaternion.identity);
						if (m_base != null)
						{
							m_base.transform.localScale = Vector3.one;
						}
						component.SetColors(GetColorFromList(bannerGameData.BannerCenter.BalancingData.ColorVector));
					}
				}
			}
		}
		if ((bool)m_uiSortingBehaviour)
		{
			m_uiSortingBehaviour.ClearAndAutoSearchRenderers();
		}
		Animator component3 = GetComponent<Animator>();
		if (component3 != null)
		{
			component3.Rebind();
		}
	}

	public Color GetColorFromList(List<float> list)
	{
		Color white = Color.white;
		for (int i = 0; i < list.Count; i++)
		{
			float num = list[i];
			switch (i)
			{
			case 0:
				white.r = num;
				break;
			case 1:
				white.g = num;
				break;
			case 2:
				white.b = num;
				break;
			case 3:
				white.a = num;
				break;
			}
		}
		return white;
	}

	public void PlayFocusTipAnim()
	{
		PlayAnimation("Focus_Tip");
		StartCoroutine(WaitThenPlayAnimation("Focus_Tip", "Idle", GetAnimationLength("Focus_Tip")));
		if ((bool)m_uiSortingBehaviour)
		{
			m_uiSortingBehaviour.ClearAndAutoSearchRenderers();
		}
	}

	public void PlayFocusBannerAnim()
	{
		PlayAnimation("Focus_Flag");
		StartCoroutine(WaitThenPlayAnimation("Focus_Flag", "Idle", GetAnimationLength("Focus_Flag")));
		if ((bool)m_uiSortingBehaviour)
		{
			m_uiSortingBehaviour.ClearAndAutoSearchRenderers();
		}
	}

	public void PlayFocusEmblemAnim()
	{
		PlayAnimation("Focus_Emblem");
		StartCoroutine(WaitThenPlayAnimation("Focus_Emblem", "Idle", GetAnimationLength("Focus_Emblem")));
		if ((bool)m_uiSortingBehaviour)
		{
			m_uiSortingBehaviour.ClearAndAutoSearchRenderers();
		}
	}
}
