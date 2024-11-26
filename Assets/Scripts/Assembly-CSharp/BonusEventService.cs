using System.Collections.Generic;
using ABH.Shared.Events.BalancingData;

public class BonusEventService
{
	public List<BonusEventBalancingData> m_BalancingList;

	public BonusEventBalancingData m_CurrentValidBalancing;

	public BonusEventService()
	{
		if (DIContainerBalancing.EventBalancingService == null)
		{
			DebugLog.Log(GetType(), "EventBalancingService not yet initialized.");
		}
		else
		{
			m_BalancingList = DIContainerBalancing.EventBalancingService.GetBalancingDataList<BonusEventBalancingData>() as List<BonusEventBalancingData>;
		}
	}

	public BonusEventService UpdateEvents()
	{
		if (m_BalancingList == null)
		{
			DebugLog.Log("No BonusEvent Balancing!");
			return this;
		}
		uint currentTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		m_CurrentValidBalancing = null;
		for (int i = 0; i < m_BalancingList.Count; i++)
		{
			BonusEventBalancingData bonusEventBalancingData = m_BalancingList[i];
			if (bonusEventBalancingData.StartDate < currentTimestamp && currentTimestamp < bonusEventBalancingData.EndDate)
			{
				m_CurrentValidBalancing = bonusEventBalancingData;
				break;
			}
		}
		return this;
	}
}
