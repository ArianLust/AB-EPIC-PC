using UnityEngine;

public class EnchantingItemSlot : InventoryItemSlot
{
	[SerializeField]
	public UITapHoldTrigger m_EnchantingPlusButton;

	[SerializeField]
	public UITapHoldTrigger m_EnchantingMinusButton;

	private EnchantmentUI m_parent;

	private float m_timePerTick = 0.4f;

	private bool m_isTappingPlus;

	private bool m_isTappingMinus;

	private float m_capToSetMax = 4f;

	private float m_secondsOnButton;

	private float m_secondsOnButtonTotal;

	private float m_stackSize = 1f;

	private float m_stackGrowthPerTick = 0.05f;

	[SerializeField]
	private Color m_SelectedResourceColor = new Color(0.596f, 0.99f, 0.2f);

	public void EnableEnchanting(EnchantmentUI parent)
	{
		if (!(m_EnchantingPlusButton == null))
		{
			m_parent = parent;
			m_EnchantingPlusButton.gameObject.SetActive(true);
			m_EnchantingMinusButton.gameObject.SetActive(true);
			RegisterEventHandler();
			UIPlayAnimation[] componentsInChildren = m_EnchantingPlusButton.GetComponentsInChildren<UIPlayAnimation>();
			foreach (UIPlayAnimation uIPlayAnimation in componentsInChildren)
			{
				uIPlayAnimation.enabled = true;
			}
			UIPlayAnimation[] componentsInChildren2 = m_EnchantingMinusButton.GetComponentsInChildren<UIPlayAnimation>();
			foreach (UIPlayAnimation uIPlayAnimation2 in componentsInChildren2)
			{
				uIPlayAnimation2.enabled = true;
			}
		}
	}

	public void DisableEnchanting()
	{
		if (!(m_EnchantingPlusButton == null))
		{
			m_EnchantingPlusButton.gameObject.SetActive(false);
			m_EnchantingMinusButton.gameObject.SetActive(false);
			DeRegisterEventHandler();
		}
	}

	private new void RegisterEventHandler()
	{
		DeRegisterEventHandler();
		m_EnchantingPlusButton.OnTapBegin += OnResourceTapStartIncrease;
		m_EnchantingPlusButton.OnTapEnd += OnResourceTapEndIncrease;
		m_EnchantingMinusButton.OnTapBegin += OnResourceTapStartDecrease;
		m_EnchantingMinusButton.OnTapEnd += OnResourceTapEndDecrease;
	}

	private new void DeRegisterEventHandler()
	{
		m_EnchantingPlusButton.OnTapBegin -= OnResourceTapStartIncrease;
		m_EnchantingPlusButton.OnTapEnd -= OnResourceTapEndIncrease;
		m_EnchantingMinusButton.OnTapBegin -= OnResourceTapStartDecrease;
		m_EnchantingMinusButton.OnTapEnd -= OnResourceTapEndDecrease;
	}

	private void OnResourceTapStartIncrease()
	{
		m_isTappingPlus = true;
		m_stackSize = 1f;
		IncreaseByStep(1);
	}

	private void OnResourceTapEndIncrease()
	{
		m_isTappingPlus = false;
		m_secondsOnButton = 0f;
		m_secondsOnButtonTotal = 0f;
		m_timePerTick = 0.4f;
	}

	private void OnResourceTapStartDecrease()
	{
		m_isTappingMinus = true;
		m_stackSize = 1f;
		DecreaseByStep(1);
	}

	private void OnResourceTapEndDecrease()
	{
		m_isTappingMinus = false;
		m_secondsOnButton = 0f;
		m_secondsOnButtonTotal = 0f;
		m_timePerTick = 0.4f;
	}

	private void Update()
	{
		if (!m_isTappingPlus && !m_isTappingMinus)
		{
			return;
		}
		m_secondsOnButton += Time.deltaTime;
		m_secondsOnButtonTotal += Time.deltaTime;
		if (m_secondsOnButton > m_timePerTick)
		{
			if (m_isTappingPlus)
			{
				IncreaseByStep(Mathf.FloorToInt(m_stackSize));
			}
			else if (m_isTappingMinus)
			{
				DecreaseByStep(Mathf.FloorToInt(m_stackSize));
			}
			m_secondsOnButton = 0f;
			m_stackSize += m_stackSize * m_stackGrowthPerTick;
			m_timePerTick *= 0.75f;
		}
	}

	private void IncreaseByStep(int stepSize)
	{
		if ((bool)m_parent)
		{
			m_parent.IncreaseResource(stepSize);
		}
	}

	private void DecreaseByStep(int stepSize)
	{
		if ((bool)m_parent)
		{
			m_parent.DecreaseResource(stepSize);
		}
	}

	public int GetMax()
	{
		if (m_FinalItem == null)
		{
			return 0;
		}
		return DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_FinalItem.ItemBalancing.NameId);
	}

	public void UpdateLabel(int selected, int max)
	{
		if (selected > 0)
		{
			m_BaseStatValue.color = m_SelectedResourceColor;
		}
		else
		{
			m_BaseStatValue.color = new Color(1f, 1f, 1f);
		}
		if (m_BaseStatValue != null)
		{
			m_BaseStatValue.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(selected) + "/" + DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(max);
		}
	}

	public void ResetLabel()
	{
		if (m_BaseStatValue != null)
		{
			m_BaseStatValue.text = DIContainerInfrastructure.GetFormatProvider().GetResourceAmountFormat(DIContainerLogic.InventoryService.GetItemValue(DIContainerInfrastructure.GetCurrentPlayer().InventoryGameData, m_FinalItem.ItemBalancing.NameId));
			m_BaseStatValue.color = new Color(1f, 1f, 1f);
		}
	}

	private void OnDestroy()
	{
		if ((bool)m_EnchantingPlusButton)
		{
			DeRegisterEventHandler();
		}
	}
}
