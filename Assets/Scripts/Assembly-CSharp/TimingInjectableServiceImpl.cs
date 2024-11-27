using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using Chimera.Library.Components.Interfaces;

public class TimingInjectableServiceImpl : ITimingService
{
	private DateTime m_CurrentTime = MinDateTime;

	private static readonly DateTime MinDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

	public string DateFormatShort
	{
		get
		{
			return "d.M.yyyy";
		}
	}

	public string DateFormatLong
	{
		get
		{
			return "dd. MMMM yyyy";
		}
	}

	private DateTime TimeNow
	{
		get
		{
			return m_CurrentTime;
		}
	}

	private DateTime TimeNowNonUtc
	{
		get
		{
			return m_CurrentTime.ToLocalTime();
		}
	}

	[method: MethodImpl(32)]
	public event Action<long> OnServerTimeReceived;

	public void Reset()
	{
		m_CurrentTime = MinDateTime;
	}

	public void SetPresentTime(DateTime newTime)
	{
		m_CurrentTime = newTime;
	}

	public DateTime GetPresentTime()
	{
		return TimeNow;
	}

	public DateTime GetPresentTimeNonUtc()
	{
		return TimeNowNonUtc;
	}

	public TimeSpan TimeLeftUntil(DateTime targetServerTime)
	{
		return targetServerTime - GetPresentTime();
	}

	public TimeSpan TimeSince(DateTime targetServerTime)
	{
		return -TimeLeftUntil(targetServerTime);
	}

	public bool IsAfter(DateTime targetServerTime)
	{
		return TimeLeftUntil(targetServerTime).TotalMilliseconds <= 0.0;
	}

	public bool IsBefore(DateTime targetServerTime)
	{
		return TimeLeftUntil(targetServerTime).TotalMilliseconds > 0.0;
	}

	public bool IsSameDay(DateTime d1, DateTime d2)
	{
		return d1.Day == d2.Day && d1.Month == d2.Month && d1.Year == d2.Year;
	}

	public bool IsBeforeDay(DateTime d1, DateTime d2)
	{
		return DateTime.Compare(d1, d2) < 0 && !IsSameDay(d1, d2);
	}

	public bool IsAfterDay(DateTime d1, DateTime d2)
	{
		return DateTime.Compare(d1, d2) > 0 && !IsSameDay(d1, d2);
	}

	public DateTime ClampToHighNoonFirstOfMonth(DateTime d)
	{
		return new DateTime(d.Year, d.Month, 1, 12, 0, 0);
	}

	public DateTime ClampToHighNoon(DateTime d)
	{
		return new DateTime(d.Year, d.Month, d.Day, 12, 0, 0);
	}

	public bool IsToday(DateTime d)
	{
		return IsSameDay(d, TimeNow);
	}

	public bool IsDayBeforeToday(DateTime d)
	{
		return IsBeforeDay(d, TimeNow);
	}

	public bool IsDayAfterToday(DateTime d)
	{
		return IsAfterDay(d, TimeNow);
	}

	public DateTime GetFirstDayOfNextMonth(DateTime d)
	{
		return new DateTime((d.Month != 12) ? d.Year : (d.Year + 1), (d.Month == 12) ? 1 : (d.Month + 1), 1, 12, 0, 0);
	}

	public DateTime GetFirstDayOfPrevMonth(DateTime d)
	{
		return new DateTime((d.Month != 1) ? d.Year : (d.Year - 1), (d.Month != 1) ? (d.Month - 1) : 12, 1, 12, 0, 0);
	}

	public long GetDifferenceInDays(DateTime d1, DateTime d2)
	{
		if (d1.Ticks > d2.Ticks)
		{
			return -1L;
		}
		return (long)TimeSpan.FromTicks(d2.Ticks - d1.Ticks).TotalDays;
	}

	public long GetDifferenceInWeeks(DateTime d1, DateTime d2)
	{
		long differenceInDays = GetDifferenceInDays(d1, d2);
		if (differenceInDays < 0)
		{
			return -1L;
		}
		if (differenceInDays % 7 != 0L)
		{
			return -1L;
		}
		return differenceInDays % 7;
	}

	public uint GetCurrentTimestamp()
	{
		return GetTimestamp(DateTime.UtcNow);
	}

	public double GetCurrentTimestampWithMs(int decimals)
	{
		return Math.Round((DateTime.UtcNow - MinDateTime).TotalMilliseconds / 1000.0, decimals);
	}

	public uint GetTimestamp(DateTime p_dtFromTime)
	{
		return (uint)(p_dtFromTime - MinDateTime).TotalSeconds;
	}

	public DateTime GetDateTimeFromTimestamp(uint ts)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(ts);
	}

	public DateTime ConvertFacebookDayToDateTimeFqlApi(string dayAsString)
	{
		if (!string.IsNullOrEmpty(dayAsString))
		{
			return DateTime.ParseExact(dayAsString, "MMMM d, yyyy", CultureInfo.InvariantCulture);
		}
		return GetDateTimeFromTimestamp(0u);
	}

	public IEnumerator GetTrustedTime(Action<DateTime> callback)
	{
		callback(TimeNow);
		yield break;
	}

	public bool TryGetTrustedTime(out DateTime trustedTime)
	{
		trustedTime = TimeNow;
		return true;
	}

	public bool RequestTimeFromServer()
	{
		return false;
	}

	public bool SetTimeFromServer(int time)
	{
		return false;
	}

	public DateTime ConvertFacebookDayToDateTimeGraphApi(string dayAsString)
	{
		if (string.IsNullOrEmpty(dayAsString))
		{
			return GetDateTimeFromTimestamp(0u);
		}
		string[] array = dayAsString.Split("/".ToCharArray(), 3, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 3)
		{
			return DateTime.MinValue;
		}
		DateTime result;
		try
		{
			result = new DateTime(int.Parse(array[2]), int.Parse(array[0]), int.Parse(array[1]));
		}
		catch
		{
			return DateTime.MinValue;
		}
		return result;
	}

	public DateTime GetNextDay(DateTime previousDay)
	{
		DateTime dateTime = previousDay.AddDays(1.0);
		return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
	}
}
