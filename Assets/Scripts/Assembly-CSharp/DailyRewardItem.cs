using System.Collections.Generic;
using ABH.Shared.Models.Generic;

public struct DailyRewardItem
{
	public int m_Day;

	public Dictionary<string, LootInfoData> m_Loot;

	public DailyLoginButtonState m_State;

	public DailyRewardItem(int day, Dictionary<string, LootInfoData> loot)
	{
		m_Day = day;
		m_Loot = loot;
		m_State = DailyLoginButtonState.OPEN;
	}
}
