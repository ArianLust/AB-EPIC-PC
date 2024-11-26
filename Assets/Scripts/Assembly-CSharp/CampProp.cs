using System;
using System.Runtime.CompilerServices;
using ABH.GameDatas;
using SmoothMoves;
using UnityEngine;

public class CampProp : MonoBehaviour
{
	[HideInInspector]
	public bool m_IsInitialized;

	[SerializeField]
	private GameObject m_UpdateIndikatorRoot;

	[SerializeField]
	private GameObject m_SaleIndikatorRoot;

	[SerializeField]
	private UILabel m_Counter;

	[SerializeField]
	private Animation m_CounterAnimation;

	[SerializeField]
	private GameObject m_CounterRoot;

	[SerializeField]
	private string m_StoryItemName;

	[SerializeField]
	private GameObject m_InactiveAlternative;

	[SerializeField]
	private UITapHoldTrigger m_TapHoldTrigger;

	private bool m_TooltipTapBegan;

	[SerializeField]
	private string m_IdleAnimationName = "Idle";

	private BasicItemGameData m_Model;

	private BoneAnimation m_BoneAnimation;

	public string m_AnimationPrefix = string.Empty;

	[SerializeField]
	private BoneAnimation m_CheerAnimation;

	private bool m_ClickDisabled;

	[method: MethodImpl(32)]
	public event Action<BasicItemGameData> OnPropClicked;

	[method: MethodImpl(32)]
	public event Action<BasicItemGameData> ShowTooltip;

	public void Awake()
	{
		IInventoryItemGameData data = null;
		if ((bool)m_SaleIndikatorRoot)
		{
			m_SaleIndikatorRoot.gameObject.SetActive(false);
		}
		if (DIContainerLogic.InventoryService.TryGetItemGameData(ClientInfo.CurrentCampInventory, m_StoryItemName, out data))
		{
			m_Model = data as BasicItemGameData;
			base.gameObject.SetActive(true);
			if (m_InactiveAlternative != null)
			{
				m_InactiveAlternative.SetActive(false);
			}
			if ((bool)m_UpdateIndikatorRoot)
			{
				if (m_Model.Data.IsNew && !ClientInfo.IsFriend)
				{
					m_UpdateIndikatorRoot.SetActive(true);
				}
				else
				{
					m_UpdateIndikatorRoot.SetActive(false);
				}
			}
		}
		else
		{
			base.gameObject.SetActive(false);
			if (m_InactiveAlternative != null)
			{
				m_InactiveAlternative.SetActive(true);
			}
		}
		RegisterEventHandlers();
		m_IsInitialized = true;
		m_BoneAnimation = GetComponent<BoneAnimation>();
		if ((bool)m_BoneAnimation)
		{
			m_BoneAnimation.Play(m_IdleAnimationName);
		}
	}

	public void CheckUpdateIndicator()
	{
		if ((bool)m_UpdateIndikatorRoot)
		{
			if (m_Model.Data.IsNew && !ClientInfo.IsFriend)
			{
				m_UpdateIndikatorRoot.SetActive(true);
			}
			else
			{
				m_UpdateIndikatorRoot.SetActive(false);
			}
		}
	}

	private void RegisterEventHandlers()
	{
		DeRegisterEventHandlers();
		if ((bool)m_TapHoldTrigger)
		{
			m_TapHoldTrigger.OnTapBegin += OnTapBegin;
			m_TapHoldTrigger.OnTapEnd += OnTapEnd;
			m_TapHoldTrigger.OnTapReleased += OnTapReleased;
		}
	}

	private void DeRegisterEventHandlers()
	{
		if ((bool)m_TapHoldTrigger)
		{
			m_TapHoldTrigger.OnTapBegin -= OnTapBegin;
			m_TapHoldTrigger.OnTapEnd -= OnTapEnd;
			m_TapHoldTrigger.OnTapReleased -= OnTapReleased;
		}
	}

	public BasicItemGameData GetModel()
	{
		return m_Model;
	}

	private void OnTouchClicked()
	{
		if (!m_ClickDisabled)
		{
			HandleClicked();
		}
	}

	public void HandleClicked()
	{
		DebugLog.Log("Prop Clicked");
		if (m_Model == null)
		{
			return;
		}
		if (m_TooltipTapBegan)
		{
			m_TooltipTapBegan = false;
			return;
		}
		if (this.OnPropClicked != null)
		{
			this.OnPropClicked(m_Model);
		}
		if ((bool)m_BoneAnimation)
		{
			if (m_BoneAnimation.AnimationClipExists("Clicked"))
			{
				m_BoneAnimation.Play("Clicked");
			}
			m_BoneAnimation.PlayQueued(m_IdleAnimationName);
		}
		if ((bool)GetComponent<Animation>() && (bool)GetComponent<Animation>()[m_AnimationPrefix + "Pressed"] && (bool)GetComponent<Animation>()[m_AnimationPrefix + "Released"])
		{
			GetComponent<Animation>().Play(m_AnimationPrefix + "Pressed");
			GetComponent<Animation>().PlayQueued(m_AnimationPrefix + "Released");
			if ((bool)GetComponent<Animation>() && (bool)GetComponent<Animation>()[m_AnimationPrefix + "Idle"])
			{
				GetComponent<Animation>().PlayQueued(m_AnimationPrefix + "Idle");
			}
		}
		if ((bool)m_CheerAnimation)
		{
			m_CheerAnimation.Play("Cheer");
			m_CheerAnimation.PlayQueued("Idle");
		}
		m_Model.ItemData.IsNew = false;
		if ((bool)m_UpdateIndikatorRoot && !m_Model.ItemData.IsNew && !ClientInfo.IsFriend)
		{
			DisableUpdateIndikator();
		}
	}

	public void PlayBoneAnimation(string animationName)
	{
		if ((bool)m_BoneAnimation)
		{
			m_BoneAnimation.Play(animationName);
		}
	}

	public void SetCounter(int counter)
	{
		if ((bool)m_Counter)
		{
			m_Counter.text = counter.ToString("0");
			m_Counter.gameObject.SetActive(counter > 0);
		}
		if ((bool)m_CounterRoot)
		{
			m_CounterRoot.SetActive(counter > 0);
		}
		if (counter > 0)
		{
			if ((bool)m_CounterAnimation)
			{
				m_CounterAnimation.Play("MailFlag_Active");
			}
		}
		else if ((bool)m_CounterAnimation)
		{
			m_CounterAnimation.Play("MailFlag_Inactive");
		}
	}

	public void DisableUpdateIndikator()
	{
		m_UpdateIndikatorRoot.SetActive(false);
	}

	private void OnTapReleased()
	{
	}

	private void OnTapEnd()
	{
	}

	private void OnTapBegin()
	{
		DebugLog.Log("OnTapBegin");
		if (this.ShowTooltip != null)
		{
			this.ShowTooltip(m_Model);
		}
	}

	private void OnDestroy()
	{
		DeRegisterEventHandlers();
	}

	public void CheckSaleIndikator()
	{
		bool active = DIContainerLogic.GetSalesManagerService().IsShopSaleActive();
		if ((bool)m_SaleIndikatorRoot)
		{
			m_SaleIndikatorRoot.SetActive(active);
		}
	}

	internal void SetClickable(bool isClickable)
	{
		m_ClickDisabled = !isClickable;
	}
}
