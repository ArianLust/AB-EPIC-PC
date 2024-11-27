using System;
using System.Collections.Generic;
using System.Linq;
using ABH.GameDatas;
using ABH.Services.Logic;
using ABH.Shared.BalancingData;
using ABH.Shared.Generic;
using ABH.Shared.Models.Generic;

public class DailyLoginLogic
{
	public DailyRewardItem[] m_RewardItems;

	private DailyLoginGiftsBalancingData m_loginBalancing;

	private bool m_privateClaimedToday;

	public bool m_popupShownThisSession;

	private List<int> m_highlightedDays;

	private int m_currentDayOfTheMonth;

	public static string BUFF_PLACEMENT
	{
		get
		{
			return "RewardVideo.DailyBonus";
		}
	}

	public bool m_ClaimedToday
	{
		get
		{
			return m_privateClaimedToday;
		}
		set
		{
			m_privateClaimedToday = value;
			DebugLog.Log(GetType(), "m_ClaimedToday set to " + value);
		}
	}

	public DailyLoginLogic()
	{
		m_ClaimedToday = true;
		m_popupShownThisSession = false;
		CheckTimers(null);
	}

	public List<string> GetTodaysReward()
	{
		return m_RewardItems[DIContainerInfrastructure.GetCurrentPlayer().Data.GiftsClaimedThisMonth].m_Loot.Keys.ToList();
	}

	public void CheckTimers(Action callbackfuncOnChange)
	{
		DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
		{
			OnGetServerTime(trustedTime, callbackfuncOnChange);
		});
	}

	private void OnGetServerTime(DateTime trustedTime, Action callbackfuncOnChange)
	{
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[11].NameId = trustedTime.Year + "01";
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[12].NameId = trustedTime.Year + "02";
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[13].NameId = trustedTime.Year + "03";
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[14].NameId = trustedTime.Year + "04";
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[15].NameId = trustedTime.Year + "05";
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[16].NameId = trustedTime.Year + "06";
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[17].NameId = trustedTime.Year + "07";
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[18].NameId = trustedTime.Year + "08";
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[19].NameId = trustedTime.Year + "09";
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[20].NameId = trustedTime.Year + "10";
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[21].NameId = trustedTime.Year + "11";
		DIContainerBalancing.Service.GetBalancingDataList<DailyLoginGiftsBalancingData>()[22].NameId = trustedTime.Year + "12";
		PlayerGameData currentPlayer = DIContainerInfrastructure.GetCurrentPlayer();
		DateTime dateTimeFromTimestamp = DIContainerLogic.GetTimingService().GetDateTimeFromTimestamp(currentPlayer.Data.LastDailyGiftClaimedTime);
		DebugLog.Log(GetType(), "OnGetServerTime: lastClaimed = " + dateTimeFromTimestamp);
		m_currentDayOfTheMonth = trustedTime.Day;
		bool flag = trustedTime.Month > dateTimeFromTimestamp.Month || trustedTime.Year > dateTimeFromTimestamp.Year || currentPlayer.Data.GiftsClaimedThisMonth > trustedTime.Day;
		DateTime dateTime = new DateTime(dateTimeFromTimestamp.Year, dateTimeFromTimestamp.Month, dateTimeFromTimestamp.Day, 0, 0, 0).AddDays(1.0);
		bool flag2 = dateTime > trustedTime;
		DebugLog.Log(GetType(), "OnGetServerTime: nextMidnight = " + dateTime.ToString() + " trustedTime = " + trustedTime.ToString());
		string nameId = trustedTime.Year + trustedTime.Month.ToString("00");
		DailyLoginGiftsBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<DailyLoginGiftsBalancingData>(nameId);
		if (balancingData == null)
		{
			balancingData = DIContainerBalancing.Service.GetBalancingData<DailyLoginGiftsBalancingData>("standard");
		}
		CreateItemArrayFromLoginBalancing(balancingData);
		m_highlightedDays = balancingData.HighLightDays;
		if (flag)
		{
			currentPlayer.Data.GiftsClaimedThisMonth = 0u;
		}
		if (flag2 != m_ClaimedToday)
		{
			DebugLog.Log(GetType(), "OnGetServerTime: A new day has arrived, hooray!");
			m_ClaimedToday = flag2;
			if (callbackfuncOnChange != null)
			{
				callbackfuncOnChange();
			}
		}
		if (callbackfuncOnChange == null && !m_ClaimedToday && !m_popupShownThisSession)
		{
			m_popupShownThisSession = true;
			CoreStateMgr.Instance.ShowDailyLoginUIOnStartUp();
		}
	}

	public bool IsDailyLoginInitialized()
	{
		return m_RewardItems != null;
	}

	private void CreateItemArrayFromLoginBalancing(DailyLoginGiftsBalancingData login)
	{
		int level = DIContainerInfrastructure.GetCurrentPlayer().Data.Level + 2;
		m_loginBalancing = login;
		m_RewardItems = new DailyRewardItem[31];
		LootOperations lootOperationService = DIContainerLogic.GetLootOperationService();
		m_RewardItems[0] = new DailyRewardItem(1, lootOperationService.GenerateLoot(login.Day1, level));
		m_RewardItems[1] = new DailyRewardItem(2, lootOperationService.GenerateLoot(login.Day2, level));
		m_RewardItems[2] = new DailyRewardItem(3, lootOperationService.GenerateLoot(login.Day3, level));
		m_RewardItems[3] = new DailyRewardItem(4, lootOperationService.GenerateLoot(login.Day4, level));
		m_RewardItems[4] = new DailyRewardItem(5, lootOperationService.GenerateLoot(login.Day5, level));
		m_RewardItems[5] = new DailyRewardItem(6, lootOperationService.GenerateLoot(login.Day6, level));
		m_RewardItems[6] = new DailyRewardItem(7, lootOperationService.GenerateLoot(login.Day7, level));
		m_RewardItems[7] = new DailyRewardItem(8, lootOperationService.GenerateLoot(login.Day8, level));
		m_RewardItems[8] = new DailyRewardItem(9, lootOperationService.GenerateLoot(login.Day9, level));
		m_RewardItems[9] = new DailyRewardItem(10, lootOperationService.GenerateLoot(login.Day10, level));
		m_RewardItems[10] = new DailyRewardItem(11, lootOperationService.GenerateLoot(login.Day11, level));
		m_RewardItems[11] = new DailyRewardItem(12, lootOperationService.GenerateLoot(login.Day12, level));
		m_RewardItems[12] = new DailyRewardItem(13, lootOperationService.GenerateLoot(login.Day13, level));
		m_RewardItems[13] = new DailyRewardItem(14, lootOperationService.GenerateLoot(login.Day14, level));
		m_RewardItems[14] = new DailyRewardItem(15, lootOperationService.GenerateLoot(login.Day15, level));
		m_RewardItems[15] = new DailyRewardItem(16, lootOperationService.GenerateLoot(login.Day16, level));
		m_RewardItems[16] = new DailyRewardItem(17, lootOperationService.GenerateLoot(login.Day17, level));
		m_RewardItems[17] = new DailyRewardItem(18, lootOperationService.GenerateLoot(login.Day18, level));
		m_RewardItems[18] = new DailyRewardItem(19, lootOperationService.GenerateLoot(login.Day19, level));
		m_RewardItems[19] = new DailyRewardItem(20, lootOperationService.GenerateLoot(login.Day20, level));
		m_RewardItems[20] = new DailyRewardItem(21, lootOperationService.GenerateLoot(login.Day21, level));
		m_RewardItems[21] = new DailyRewardItem(22, lootOperationService.GenerateLoot(login.Day22, level));
		m_RewardItems[22] = new DailyRewardItem(23, lootOperationService.GenerateLoot(login.Day23, level));
		m_RewardItems[23] = new DailyRewardItem(24, lootOperationService.GenerateLoot(login.Day24, level));
		m_RewardItems[24] = new DailyRewardItem(25, lootOperationService.GenerateLoot(login.Day25, level));
		m_RewardItems[25] = new DailyRewardItem(26, lootOperationService.GenerateLoot(login.Day26, level));
		m_RewardItems[26] = new DailyRewardItem(27, lootOperationService.GenerateLoot(login.Day27, level));
		m_RewardItems[27] = new DailyRewardItem(28, lootOperationService.GenerateLoot(login.Day28, level));
		m_RewardItems[28] = new DailyRewardItem(29, lootOperationService.GenerateLoot(login.Day29, level));
		m_RewardItems[29] = new DailyRewardItem(30, lootOperationService.GenerateLoot(login.Day30, level));
		m_RewardItems[30] = new DailyRewardItem(31, lootOperationService.GenerateLoot(login.Day31, level));
	}

	public List<IInventoryItemGameData> ClaimGift(bool isForced = false)
	{
		PlayerGameData player = DIContainerInfrastructure.GetCurrentPlayer();
		player.Data.GiftsClaimedThisMonth++;
		if (!isForced)
		{
			m_ClaimedToday = true;
		}
		Dictionary<string, LootInfoData> dictionary = m_RewardItems[player.Data.GiftsClaimedThisMonth - 1].m_Loot;
		if (dictionary.FirstOrDefault().Key.Contains("mastery"))
		{
			MasteryItemBalancingData balancingData = DIContainerBalancing.Service.GetBalancingData<MasteryItemBalancingData>(dictionary.FirstOrDefault().Key);
			if (balancingData != null && !DIContainerLogic.InventoryService.IsAddMasteryPossible(balancingData, DIContainerInfrastructure.GetCurrentPlayer()))
			{
				dictionary = DIContainerLogic.GetLootOperationService().GenerateLoot(balancingData.FallbackLootTableDailyLogin, player.Data.Level);
			}
		}
		List<IInventoryItemGameData> list = DIContainerLogic.GetLootOperationService().RewardLoot(player.InventoryGameData, 4, dictionary, "daily_gift", EquipmentSource.Gatcha);
		if (player.Data.CalendarChestLootWon == null)
		{
			player.Data.CalendarChestLootWon = new Dictionary<int, string>();
		}
		player.Data.CalendarChestLootWon.SaveAdd((int)player.Data.GiftsClaimedThisMonth, list.FirstOrDefault().ItemBalancing.NameId);
		if (!isForced)
		{
			DIContainerLogic.GetTimingService().GetTrustedTimeEx(delegate(DateTime trustedTime)
			{
				player.Data.LastDailyGiftClaimedTime = DIContainerLogic.GetTimingService().GetTimestamp(trustedTime);
			});
		}
		player.SavePlayerData();
		return list;
	}

	public bool HighlightDay(int correctDay)
	{
		if (m_highlightedDays == null)
		{
			return false;
		}
		return m_highlightedDays.Contains(correctDay);
	}

	public bool IsVideoRewardAvailable()
	{
		if (!DIContainerInfrastructure.AdService.IsAdShowPossible(BUFF_PLACEMENT))
		{
			return false;
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.GiftsClaimedThisMonth >= m_currentDayOfTheMonth)
		{
			return false;
		}
		if (DIContainerInfrastructure.GetCurrentPlayer().Data.GiftsClaimedThisMonth == m_currentDayOfTheMonth - 1 && !m_ClaimedToday)
		{
			return false;
		}
		return true;
	}

	public void ClearDailyRewardCache()
	{
		m_RewardItems = null;
		CheckTimers(null);
	}

	public Dictionary<string, int> GetRewardForDay(int day)
	{
		switch (day)
		{
		case 1:
			return m_loginBalancing.Day1;
		case 2:
			return m_loginBalancing.Day2;
		case 3:
			return m_loginBalancing.Day3;
		case 4:
			return m_loginBalancing.Day4;
		case 5:
			return m_loginBalancing.Day5;
		case 6:
			return m_loginBalancing.Day6;
		case 7:
			return m_loginBalancing.Day7;
		case 8:
			return m_loginBalancing.Day8;
		case 9:
			return m_loginBalancing.Day9;
		case 10:
			return m_loginBalancing.Day10;
		case 11:
			return m_loginBalancing.Day11;
		case 12:
			return m_loginBalancing.Day12;
		case 13:
			return m_loginBalancing.Day13;
		case 14:
			return m_loginBalancing.Day14;
		case 15:
			return m_loginBalancing.Day15;
		case 16:
			return m_loginBalancing.Day16;
		case 17:
			return m_loginBalancing.Day17;
		case 18:
			return m_loginBalancing.Day18;
		case 19:
			return m_loginBalancing.Day19;
		case 20:
			return m_loginBalancing.Day20;
		case 21:
			return m_loginBalancing.Day21;
		case 22:
			return m_loginBalancing.Day22;
		case 23:
			return m_loginBalancing.Day23;
		case 24:
			return m_loginBalancing.Day24;
		case 25:
			return m_loginBalancing.Day25;
		case 26:
			return m_loginBalancing.Day26;
		case 27:
			return m_loginBalancing.Day27;
		case 28:
			return m_loginBalancing.Day28;
		case 29:
			return m_loginBalancing.Day29;
		case 30:
			return m_loginBalancing.Day30;
		case 31:
			return m_loginBalancing.Day31;
		default:
			return new Dictionary<string, int>();
		}
	}
}
