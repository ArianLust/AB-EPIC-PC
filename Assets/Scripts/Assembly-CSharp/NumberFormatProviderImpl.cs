using System;
using System.Collections.Generic;

public class NumberFormatProviderImpl
{
	public string GetDurationFormatStandard(TimeSpan span, bool withAddition = true)
	{
		if (span.Days > 2)
		{
			if (withAddition)
			{
				return DIContainerInfrastructure.GetLocaService().Tr("gen_time_days", new Dictionary<string, string> { 
				{
					"{value_1}",
					span.Days.ToString("0")
				} });
			}
			return span.Days.ToString("0");
		}
		if (span.TotalSeconds < 0.0)
		{
			return DIContainerInfrastructure.GetLocaService().Tr("gen_time_over", "Finished!");
		}
		return span.Hours + span.Days * 24 + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
	}

	public string GetDurationFormatStandardDown(TimeSpan span, bool withAddition = true)
	{
		if (span.Days > 2)
		{
			if (withAddition)
			{
				return DIContainerInfrastructure.GetLocaService().Tr("event_teaser_timeleft_days", new Dictionary<string, string> { 
				{
					"{value_1}",
					span.Days.ToString("0")
				} });
			}
			return span.Days.ToString("0");
		}
		if (span.TotalSeconds < 0.0)
		{
			return DIContainerInfrastructure.GetLocaService().Tr("gen_time_over", "Finished!");
		}
		string value = span.Hours + span.Days * 24 + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
		return DIContainerInfrastructure.GetLocaService().Tr("event_teaser_timeleft", new Dictionary<string, string> { { "{value_1}", value } });
	}

	public string GetResourceAmountFormat(int value)
	{
		string text = value.ToString("0");
		int length = text.Length;
		for (int num = length - 1; num >= 1; num--)
		{
			if ((length - num) % 3 == 0)
			{
				text = text.Insert(num, DIContainerInfrastructure.GetLocaService().Tr("gen_thousandseperator", "."));
			}
		}
		return text;
	}

	public string GetBattleStatsFormat(float stat)
	{
		return stat.ToString("0");
	}

	public string GetBattleStatsFractionalFormat(float stat)
	{
		return stat.ToString("0.##");
	}
}
