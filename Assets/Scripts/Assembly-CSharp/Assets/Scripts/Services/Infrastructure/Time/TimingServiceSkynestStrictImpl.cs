using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using ABH.Shared.Models;
using Chimera.Library.Components.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Services.Infrastructure.Time
{
	internal class TimingServiceSkynestStrictImpl : ITimingService
	{
		private const int Tolerance = 1200;

		private const string PlayerPrefsName = "TimingServiceSkynestStrictImpl";

		private const float PersistIntervalTime = 300f;

		private int m_lastDelta = int.MinValue;

		private bool m_noServerTimeReceivedSinceAppStart = true;

		private float m_lastPersistTime = UnityEngine.Time.time;

		private readonly TimingData m_data;

		private float m_unityTimeFromOutOfSync = float.MinValue;

		private bool m_serverTimeRequested;

		private int m_lastGetTimeFrameNumber;

		private DateTime m_cachedTime;

		private bool m_cachedTimeIsTrusted;

		private TimeSpan m_cheatTimeOffset;

		private bool m_appWasPausedRecently;

		private float m_lastLogTime;

		private static readonly DateTime MinDateTime;

		private bool m_loggedOnce;

		private uint m_UpcomingEventTime;

		public string DateFormatLong { get; private set; }

		public string DateFormatShort { get; private set; }

		[method: MethodImpl(32)]
		public event Action<long> OnServerTimeReceived;

		public TimingServiceSkynestStrictImpl()
		{
			DIContainerInfrastructure.GetNetworkStatusService().InternetAvailabilityResponseReceived += OnServerTimeJsonReceived;
			OnAppResumed();
			string @string = PlayerPrefs.GetString("TimingServiceSkynestStrictImpl", string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				m_data = new TimingData();
				return;
			}
			m_data = DIContainerInfrastructure.GetStringSerializer().Deserialize<TimingData>(@string);
			m_lastDelta = m_data.ClientSyncTime - m_data.ServerSyncTime;
		}

		static TimingServiceSkynestStrictImpl()
		{
			MinDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		}

		public void OnAppResumed()
		{
			MaybeRequestServerTime();
			m_unityTimeFromOutOfSync = float.MinValue;
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
			if (!m_serverTimeRequested)
			{
				if (ContentLoader.Instance != null)
				{
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

		private DateTime GetCurrentTime(out bool isTrusted)
		{
			int frameCount = UnityEngine.Time.frameCount;
			if (!m_appWasPausedRecently && m_lastGetTimeFrameNumber == frameCount)
			{
				isTrusted = m_cachedTimeIsTrusted;
				m_loggedOnce = true;
				return m_cachedTime;
			}
			m_loggedOnce = false;
			m_lastGetTimeFrameNumber = frameCount;
			if (m_noServerTimeReceivedSinceAppStart)
			{
				MaybeRequestServerTime();
				m_data.LatestClientTimeReturned++;
				m_cachedTime = UnixTimeStampToDateTime(m_data.LatestClientTimeReturned);
				if (Application.platform != RuntimePlatform.WindowsEditor)
				{
					DebugLog.Warn(GetType(), "GetCurrentTime: no server time received yet, returning " + m_cachedTime);
				}
				m_cachedTimeIsTrusted = (isTrusted = false);
				return m_cachedTime;
			}
			DateTime dateTime = QuerySystemTime();
			long num = DateTimeToUnixTimestamp(dateTime);
			long num2 = (long)m_data.ServerSyncTime - (long)m_data.ClientSyncTime + num;
			string timeDescription;
			if (Math.Abs(num2 - m_data.ServerSyncTime) > 1200)
			{
				if (m_unityTimeFromOutOfSync != float.MinValue)
				{
					m_cachedTimeIsTrusted = (isTrusted = true);
					int num3 = (int)(UnityEngine.Time.time - m_unityTimeFromOutOfSync);
					num = (long)m_data.ClientSyncTime + (long)num3;
					timeDescription = "trusted extrapolated (" + num3 + "secs)";
				}
				else
				{
					m_cachedTimeIsTrusted = (isTrusted = false);
					MaybeRequestServerTime();
					num = m_data.ClientSyncTime;
					timeDescription = "last trusted (= now untrusted) time";
					DebugLog.Warn(GetType(), "GetCurrentTime: tolerance exceeded. returning last trusted time: " + UnixTimeStampToDateTime((int)num));
				}
			}
			else
			{
				m_cachedTimeIsTrusted = (isTrusted = true);
				timeDescription = "trusted";
			}
			m_data.LatestClientTimeReturned = (int)Math.Max(num, m_data.LatestClientTimeReturned);
			DateTime result = (m_cachedTime = UnixTimeStampToDateTime(m_data.LatestClientTimeReturned));
			LogTime(m_data.LatestClientTimeReturned, num, timeDescription);
			return result;
		}

		public IEnumerator GetTrustedTime(Action<DateTime> callback)
		{
			for (int i = 0; i < 80; i++)
			{
				bool isTrusted;
				DateTime time = GetCurrentTime(out isTrusted);
				if (isTrusted)
				{
					callback(time);
					yield break;
				}
				yield return new WaitForSeconds(0.5f);
			}
			bool isFallbackTrusted;
			DateTime fallbackTime = GetCurrentTime(out isFallbackTrusted);
			if (!isFallbackTrusted)
			{
				DebugLog.Error(GetType(), "GetTrustedTime: returning untrusted time: " + fallbackTime);
			}
			callback(fallbackTime);
		}

		public bool TryGetTrustedTime(out DateTime trustedTime)
		{
			bool isTrusted;
			trustedTime = GetCurrentTime(out isTrusted);
			return isTrusted;
		}

		public void LogTime(int time, long nowStamp, string timeDescription)
		{
		}

		public DateTime ClampToHighNoon(DateTime d)
		{
			return new DateTime(d.Year, d.Month, d.Day, 12, 0, 0);
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
			bool isTrusted;
			return GetCurrentTime(out isTrusted);
		}

		public uint GetTimestamp(DateTime p_dtFromTime)
		{
			return (uint)DateTimeToUnixTimestamp(p_dtFromTime);
		}

		public bool IsAfter(DateTime targetServerTime)
		{
			return TimeLeftUntil(targetServerTime).TotalMilliseconds <= 0.0;
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
			currServerTime += (int)m_cheatTimeOffset.TotalSeconds;
			m_serverTimeRequested = false;
			int num = DateTimeToUnixTimestamp(QuerySystemTime());
			if (m_lastDelta != int.MinValue)
			{
				int num2 = currServerTime - m_data.ServerSyncTime + m_data.ClientSyncTime;
				if (Math.Abs(num - num2) > 1200)
				{
					UpdateTime(num2, currServerTime);
					DebugLog.Warn(GetType(), "SetTimeFromServer: tolerance exceeded. setting client time to " + num2 + " instead of " + num + ", delta = " + m_lastDelta);
					if (m_unityTimeFromOutOfSync == float.MinValue)
					{
						m_unityTimeFromOutOfSync = UnityEngine.Time.time;
					}
					return false;
				}
				UpdateTime(num, currServerTime);
				DebugLog.Log(GetType(), "SetTimeFromServer: inside tolerance, setting client time to " + num + ", delta = " + m_lastDelta);
			}
			else
			{
				UpdateTime(num, currServerTime);
				DebugLog.Log(GetType(), "SetTimeFromServer: initial delta calculation, setting client time to " + num + ", delta = " + m_lastDelta);
			}
			m_unityTimeFromOutOfSync = float.MinValue;
			return true;
		}

		private void UpdateTime(int clientTime, int serverTime)
		{
			m_data.ClientSyncTime = clientTime;
			m_data.ServerSyncTime = serverTime;
			m_lastDelta = m_data.ClientSyncTime - m_data.ServerSyncTime;
			if (UnityEngine.Time.time - m_lastPersistTime > 300f || m_noServerTimeReceivedSinceAppStart)
			{
				m_lastPersistTime = UnityEngine.Time.time;
				PersistData();
			}
			m_noServerTimeReceivedSinceAppStart = false;
			if (this.OnServerTimeReceived != null)
			{
				this.OnServerTimeReceived(serverTime);
			}
		}

		private void PersistData()
		{
			string value = DIContainerInfrastructure.GetStringSerializer().Serialize(m_data);
			DIContainerInfrastructure.GetPlayerPrefsService().SetString("TimingServiceSkynestStrictImpl", value);
			DIContainerInfrastructure.GetPlayerPrefsService().Save();
			DebugLog.Log(GetType(), "PersistData done");
		}

		private DateTime QuerySystemTime()
		{
			return DateTime.UtcNow + m_cheatTimeOffset;
		}

		public void IncreaseTimeByCheat(int secondsToAdd)
		{
			m_cheatTimeOffset += TimeSpan.FromSeconds(secondsToAdd);
		}

		public DateTime GetPresentTimeNonUtc()
		{
			bool isTrusted;
			return GetCurrentTime(out isTrusted).ToLocalTime();
		}

		public DateTime GetNextDay(DateTime previousDay)
		{
			DateTime dateTime = previousDay.AddDays(1.0);
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
		}

		public bool ResetEventTime(int maxTime, uint eventStartTime)
		{
			if (GetCurrentTimestamp() <= eventStartTime)
			{
				return false;
			}
			uint num = GetCurrentTimestamp() - eventStartTime;
			bool num2 = (ulong)(num - m_UpcomingEventTime) >= (ulong)maxTime;
			if (num2 && DIContainerInfrastructure.GetPlayerPrefsService().HasKey("TimingServiceSkynestStrictImpl"))
			{
				m_data.ClientSyncTime = m_data.ServerSyncTime;
				m_data.LatestClientTimeReturned = m_data.ServerSyncTime;
				m_lastDelta = m_data.ClientSyncTime - m_data.ServerSyncTime;
				m_cachedTime = UnixTimeStampToDateTime(m_data.ServerSyncTime);
			}
			m_UpcomingEventTime += num;
			return num2;
		}
	}
}
