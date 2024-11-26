using System.Collections.Generic;
using ABH.GameDatas;
using ABH.Shared.Events.BalancingData;
using Assets.Scripts.Services.Infrastructure.Time;
using Chimera.Library.Components.Services;

public class NewsLogic
{
	private Dictionary<string, int> m_PlacementsWithUpdate;

	private bool FixPending;

	public NewsItemStruct[] GetUpcomingEvents()
	{
		NewsItemStruct[] array = new NewsItemStruct[3];
		List<NewsItemStruct> bonusEventList = GetBonusEventList();
		List<NewsItemStruct> upcomingGameplayEvents = GetUpcomingGameplayEvents();
		FixPending = false;
		if (upcomingGameplayEvents.Count >= 1)
		{
			array[0] = upcomingGameplayEvents[0];
		}
		else
		{
			array[0] = default(NewsItemStruct);
			array[0].type = NewsEventType.Gameplay;
		}
		if (upcomingGameplayEvents.Count >= 2)
		{
			array[1] = upcomingGameplayEvents[1];
		}
		else
		{
			array[1] = default(NewsItemStruct);
			array[1].type = NewsEventType.Gameplay;
		}
		if (bonusEventList.Count >= 1)
		{
			array[2] = bonusEventList[0];
		}
		else
		{
			array[2] = default(NewsItemStruct);
			array[2].type = NewsEventType.Bonus;
		}
		return array;
	}

	public Dictionary<string, int> GetPlacementsWithUpdate()
	{
		if (m_PlacementsWithUpdate == null)
		{
			m_PlacementsWithUpdate = new Dictionary<string, int>();
		}
		return m_PlacementsWithUpdate;
	}

	public void RemoveUpdateForPlacement(string placementId)
	{
		if (m_PlacementsWithUpdate != null && m_PlacementsWithUpdate.ContainsKey(placementId))
		{
			m_PlacementsWithUpdate[placementId] = 0;
		}
	}

	public void SetNewContentUpdateHandler()
	{
		DIContainerInfrastructure.AdService.NewsFeedContentUpdate += NewNewsContentAvailable;
	}

	private void NewNewsContentAvailable(string placementId, int numberOfNewItems)
	{
		DebugLog.Log(GetType(), "TESTING NEWSFEED UPDATES: got " + numberOfNewItems + " newItems in placement " + placementId);
		if (placementId.Contains("NewsFeed") && !placementId.Contains("pause") && numberOfNewItems > 0)
		{
			if (m_PlacementsWithUpdate == null)
			{
				m_PlacementsWithUpdate = new Dictionary<string, int>();
			}
			m_PlacementsWithUpdate[placementId] = numberOfNewItems;
		}
	}

	private List<NewsItemStruct> GetUpcomingGameplayEvents()
	{
		List<NewsItemStruct> list = new List<NewsItemStruct>();
		if (FixPending)
		{
			return list;
		}
		uint currentTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		foreach (EventManagerBalancingData balancingData in DIContainerBalancing.EventBalancingService.GetBalancingDataList<EventManagerBalancingData>())
		{
			NewsItemStruct item = default(NewsItemStruct);
			item.type = NewsEventType.Gameplay;
			if (currentTimestamp < balancingData.EventTeaserStartTimeStamp || currentTimestamp > balancingData.EventEndTimeStamp)
			{
				continue;
			}
			if (currentTimestamp > balancingData.EventTeaserStartTimeStamp && currentTimestamp < balancingData.EventStartTimeStamp)
			{
				DebugLog.Log(GetType(), "NewsUI: event " + balancingData.NameId + " is teasing.");
				item.nameId = balancingData.NameId + "_false";
				item.isRunning = false;
				item.targetTimestamp = balancingData.EventStartTimeStamp;
				item.gameplayEventBalancing = balancingData;
				list.Add(item);
				// if (CanFixInvalidEventTime(item.targetTimestamp))
				// {
				// 	return list;
				// }
			}
			else if (currentTimestamp > balancingData.EventStartTimeStamp && currentTimestamp < balancingData.EventEndTimeStamp)
			{
				DebugLog.Log(GetType(), "NewsUI: event " + balancingData.NameId + " is running.");
				item.isRunning = true;
				item.targetTimestamp = balancingData.EventEndTimeStamp;
				item.nameId = balancingData.NameId + "_true";
				item.gameplayEventBalancing = balancingData;
				list.Add(item);
			}
		}
		return list;
	}

	private List<NewsItemStruct> GetBonusEventList()
	{
		List<NewsItemStruct> list = new List<NewsItemStruct>();
		uint currentTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		foreach (BonusEventBalancingData balancingData in DIContainerBalancing.EventBalancingService.GetBalancingDataList<BonusEventBalancingData>())
		{
			uint num = 0u;
			bool flag = false;
			if (currentTimestamp > balancingData.StartDate && currentTimestamp < balancingData.EndDate)
			{
				flag = true;
				num = balancingData.EndDate;
			}
			else
			{
				if (currentTimestamp >= balancingData.StartDate || !balancingData.TeasedBeforeRunning)
				{
					continue;
				}
				if (CanFixInvalidEventTime(balancingData.StartDate))
				{
					return list;
				}
				num = balancingData.StartDate;
			}
			NewsItemStruct item = default(NewsItemStruct);
			item.type = NewsEventType.Bonus;
			item.targetTimestamp = num;
			item.bonusEventBalancing = balancingData;
			item.isRunning = flag;
			item.nameId = balancingData.NameId + "_" + flag;
			list.Add(item);
		}
		return list;
	}

	private NewsItemStruct GetNextGameplayEvent()
	{
		NewsItemStruct result = default(NewsItemStruct);
		result.type = NewsEventType.Gameplay;
		uint currentTimestamp = DIContainerLogic.GetTimingService().GetCurrentTimestamp();
		uint num = 2147483647u;
		EventManagerBalancingData eventManagerBalancingData = null;
		foreach (EventManagerBalancingData balancingData in DIContainerBalancing.EventBalancingService.GetBalancingDataList<EventManagerBalancingData>())
		{
			if (currentTimestamp > balancingData.EventStartTimeStamp && currentTimestamp < balancingData.EventEndTimeStamp)
			{
				result.isRunning = true;
				result.targetTimestamp = balancingData.EventEndTimeStamp;
				result.nameId = balancingData.NameId + "_true";
				return result;
			}
			if (currentTimestamp < balancingData.EventStartTimeStamp && balancingData.EventStartTimeStamp < num)
			{
				num = balancingData.EventStartTimeStamp;
				eventManagerBalancingData = balancingData;
			}
		}
		if (eventManagerBalancingData == null)
		{
			DebugLog.Warn(GetType(), "GetNextGameplayEvent: No gameplay events scheduled!!");
			return result;
		}
		result.nameId = eventManagerBalancingData.NameId + "_false";
		result.isRunning = false;
		result.targetTimestamp = num;
		return result;
	}

	public bool HasNewItemsAvailable()
	{
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		if (currentPlayer.Data.LastwatchedNewsItems == null)
		{
			return DIContainerLogic.InventoryService.CheckForItem(currentPlayer.InventoryGameData, "unlock_events");
		}
		NewsItemStruct[] upcomingEvents = GetUpcomingEvents();
		for (int i = 0; i < upcomingEvents.Length; i++)
		{
			if (upcomingEvents[i].nameId != null && !currentPlayer.Data.LastwatchedNewsItems.Contains(upcomingEvents[i].nameId))
			{
				return true;
			}
		}
		return false;
	}

	private bool CanFixInvalidEventTime(uint eventStartTime)
	{
		// if (!FixPending && DIContainerConfig.GetClientConfig().UseSkynestTimingService && (DIContainerLogic.GetTimingService() as TimingServiceSkynestStrictImpl).ResetEventTime(2505600, eventStartTime))
		// {
		// 	ContentLoader.Instance.Hatch2_OnBalancingDataLoadedQuickStart();
		// 	FixPending = true;
		// 	return true;
		// }
		return false;
	}
}
