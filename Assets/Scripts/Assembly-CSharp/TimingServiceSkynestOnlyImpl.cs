using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

internal class TimingServiceSkynestOnlyImpl : ITimingService
{
	private const float LogCurrentTimeInterval = 15f;

	private const float DeltaTimeTolerance = 1f;

	private DateTime m_lastServerTime = MinDateTime;

	private float m_localTimeAtLastServerTime = Time.time;

	private bool m_serverTimeRequested;

	private bool m_appWasPausedRecently;

	private int m_lastGetTimeFrameNumber;

	private DateTime m_cachedTime;

	private TimeSpan m_cheatTimeOffset;

	private float m_lastLogTime;

	private static readonly DateTime MinDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

	public string DateFormatLong { get; private set; }

	public string DateFormatShort { get; private set; }

	[method: MethodImpl(32)]
	public event Action<long> OnServerTimeReceived;

	public TimingServiceSkynestOnlyImpl()
	{
		DIContainerInfrastructure.GetNetworkStatusService().InternetAvailabilityResponseReceived += OnServerTimeJsonReceived;
		OnAppResumed();
	}

	public void OnAppResumed()
	{
		m_appWasPausedRecently = true;
		MaybeRequestServerTime();
	}

	public void OnAppPaused()
	{
		m_appWasPausedRecently = true;
	}

	public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
	}

	public static int DateTimeToUnixTimestamp(DateTime dateTime)
	{
		return (int)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	private void OnServerTimeJsonReceived(string serverTimeJson)
	{
		int timeFromServer = new ABHTimingHandler().ProcessTimeFromSkynestTimeService(serverTimeJson);
		SetTimeFromServer(timeFromServer);
	}

	private void MaybeRequestServerTime()
	{
		if (!m_serverTimeRequested || m_appWasPausedRecently)
		{
			if (ContentLoader.Instance != null)
			{
				m_appWasPausedRecently = false;
				m_serverTimeRequested = true;
				ContentLoader.Instance.CheckConnectivityAsync();
				DebugLog.Log(GetType(), "MaybeRequestServerTime: requesting server time...");
			}
			else
			{
				DebugLog.Log(GetType(), "MaybeRequestServerTime: ContentLoader is null, not requesting server time!");
			}
		}
	}

	private DateTime GetCurrentTime()
	{
		int frameCount = Time.frameCount;
		if (!m_appWasPausedRecently && m_lastGetTimeFrameNumber == frameCount)
		{
			return m_cachedTime;
		}
		m_lastGetTimeFrameNumber = frameCount;
		if (m_appWasPausedRecently)
		{
			MaybeRequestServerTime();
			float num = ((!(Time.deltaTime < 1f)) ? 0f : Time.deltaTime);
			return m_cachedTime += TimeSpan.FromSeconds(num);
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(Time.time - m_localTimeAtLastServerTime);
		m_cachedTime = m_lastServerTime + timeSpan;
		LogTime();
		return m_cachedTime;
	}

	public IEnumerator GetTrustedTime(Action<DateTime> callback)
	{
		callback(GetCurrentTime());
		yield break;
	}

	public bool TryGetTrustedTime(out DateTime trustedTime)
	{
		trustedTime = GetCurrentTime();
		return true;
	}

	private void LogTime()
	{
	}

	public DateTime ClampToHighNoon(DateTime d)
	{
		return new DateTime(d.Year, d.Month, d.Day, 12, 0, 0);
	}

	public DateTime GetTodayWithOffset(DateTime d, TimeSpan addedSpan)
	{
		return new DateTime(d.Year, d.Month, d.Day, 0, 0, 0) + addedSpan;
	}

	public DateTime ClampToHighNoonFirstOfMonth(DateTime d)
	{
		return new DateTime(d.Year, d.Month, 1, 12, 0, 0);
	}

	public uint GetCurrentTimestamp()
	{
		return (uint)DateTimeToUnixTimestamp(GetPresentTime());
	}

	public double GetCurrentTimestampWithMs(int decimals)
	{
		return Math.Round((GetPresentTime() - MinDateTime).TotalMilliseconds / 1000.0, decimals);
	}

	public DateTime GetDateTimeFromTimestamp(uint ts)
	{
		return UnixTimeStampToDateTime((int)ts);
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

	public DateTime GetFirstDayOfNextMonth(DateTime d)
	{
		return new DateTime((d.Month != 12) ? d.Year : (d.Year + 1), (d.Month == 12) ? 1 : (d.Month + 1), 1, 12, 0, 0);
	}

	public DateTime GetFirstDayOfPrevMonth(DateTime d)
	{
		return new DateTime((d.Month != 1) ? d.Year : (d.Year - 1), (d.Month != 1) ? (d.Month - 1) : 12, 1, 12, 0, 0);
	}

	public DateTime GetPresentTime()
	{
		return GetCurrentTime();
	}

	public uint GetTimestamp(DateTime p_dtFromTime)
	{
		return (uint)DateTimeToUnixTimestamp(p_dtFromTime);
	}

	public bool IsAfter(DateTime targetServerTime)
	{
		return TimeLeftUntil(targetServerTime).TotalMilliseconds <= 0.0;
	}

	public bool IsAfter(DateTime d1, DateTime d2)
	{
		return DateTime.Compare(d1, d2) > 0;
	}

	public bool IsAfterDay(DateTime d1, DateTime d2)
	{
		return DateTime.Compare(d1, d2) > 0 && !IsSameDay(d1, d2);
	}

	public bool IsBefore(DateTime targetServerTime)
	{
		return TimeLeftUntil(targetServerTime).TotalMilliseconds > 0.0;
	}

	public bool IsBeforeDay(DateTime d1, DateTime d2)
	{
		return DateTime.Compare(d1, d2) < 0 && !IsSameDay(d1, d2);
	}

	public bool IsBefore(DateTime d1, DateTime d2)
	{
		return DateTime.Compare(d1, d2) < 0;
	}

	public bool IsDayAfterToday(DateTime d)
	{
		return IsAfterDay(d, GetPresentTime());
	}

	public bool IsDayBeforeToday(DateTime d)
	{
		return IsBeforeDay(d, GetPresentTime());
	}

	public bool IsSameDay(DateTime d1, DateTime d2)
	{
		return d1.Day == d2.Day && d1.Month == d2.Month && d1.Year == d2.Year;
	}

	public bool IsToday(DateTime d)
	{
		return IsSameDay(d, GetPresentTime());
	}

	public TimeSpan TimeLeftUntil(DateTime targetServerTime)
	{
		return targetServerTime - GetPresentTime();
	}

	public TimeSpan TimeSince(DateTime targetServerTime)
	{
		return -TimeLeftUntil(targetServerTime);
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

	public DateTime ConvertFacebookDayToDateTimeFqlApi(string dayAsString)
	{
		if (!string.IsNullOrEmpty(dayAsString))
		{
			return DateTime.ParseExact(dayAsString, "MMMM d, yyyy", CultureInfo.InvariantCulture);
		}
		return GetDateTimeFromTimestamp(0u);
	}

	public bool RequestTimeFromServer()
	{
		return true;
	}

	public bool SetTimeFromServer(int currServerTime)
	{
		m_appWasPausedRecently = false;
		m_serverTimeRequested = false;
		m_lastServerTime = GetDateTimeFromTimestamp((uint)currServerTime) + m_cheatTimeOffset;
		m_localTimeAtLastServerTime = Time.time;
		m_cachedTime = m_lastServerTime;
		if (this.OnServerTimeReceived != null)
		{
			this.OnServerTimeReceived(currServerTime);
		}
		return true;
	}

	public void IncreaseTimeByCheat(int secondsToAdd)
	{
		m_cheatTimeOffset += TimeSpan.FromSeconds(secondsToAdd);
	}

	public DateTime GetPresentTimeNonUtc()
	{
		return GetCurrentTime().ToLocalTime();
	}

	public DateTime GetNextDay(DateTime previousDay)
	{
		DateTime dateTime = previousDay.AddDays(1.0);
		return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
	}
}
