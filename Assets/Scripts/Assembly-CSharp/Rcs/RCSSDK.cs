using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Rcs
{
	public class RCSSDK
	{
		protected class SWIGExceptionHelper
		{
			private static SWIGExceptionDelegate applicationDelegate;

			private static SWIGExceptionDelegate arithmeticDelegate;

			private static SWIGExceptionDelegate divideByZeroDelegate;

			private static SWIGExceptionDelegate indexOutOfRangeDelegate;

			private static SWIGExceptionDelegate invalidCastDelegate;

			private static SWIGExceptionDelegate invalidOperationDelegate;

			private static SWIGExceptionDelegate ioDelegate;

			private static SWIGExceptionDelegate nullReferenceDelegate;

			private static SWIGExceptionDelegate outOfMemoryDelegate;

			private static SWIGExceptionDelegate overflowDelegate;

			private static SWIGExceptionDelegate systemDelegate;

			private static SWIGExceptionArgumentDelegate argumentDelegate;

			private static SWIGExceptionArgumentDelegate argumentNullDelegate;

			private static SWIGExceptionArgumentDelegate argumentOutOfRangeDelegate;

			static SWIGExceptionHelper()
			{
				applicationDelegate = SetPendingApplicationException;
				arithmeticDelegate = SetPendingArithmeticException;
				divideByZeroDelegate = SetPendingDivideByZeroException;
				indexOutOfRangeDelegate = SetPendingIndexOutOfRangeException;
				invalidCastDelegate = SetPendingInvalidCastException;
				invalidOperationDelegate = SetPendingInvalidOperationException;
				ioDelegate = SetPendingIOException;
				nullReferenceDelegate = SetPendingNullReferenceException;
				outOfMemoryDelegate = SetPendingOutOfMemoryException;
				overflowDelegate = SetPendingOverflowException;
				systemDelegate = SetPendingSystemException;
				argumentDelegate = SetPendingArgumentException;
				argumentNullDelegate = SetPendingArgumentNullException;
				argumentOutOfRangeDelegate = SetPendingArgumentOutOfRangeException;
				RCSSDKPINVOKE.SWIGRegisterExceptionCallbacks_RCSSDK(Marshal.GetFunctionPointerForDelegate(applicationDelegate), Marshal.GetFunctionPointerForDelegate(arithmeticDelegate), Marshal.GetFunctionPointerForDelegate(divideByZeroDelegate), Marshal.GetFunctionPointerForDelegate(indexOutOfRangeDelegate), Marshal.GetFunctionPointerForDelegate(invalidCastDelegate), Marshal.GetFunctionPointerForDelegate(invalidOperationDelegate), Marshal.GetFunctionPointerForDelegate(ioDelegate), Marshal.GetFunctionPointerForDelegate(nullReferenceDelegate), Marshal.GetFunctionPointerForDelegate(outOfMemoryDelegate), Marshal.GetFunctionPointerForDelegate(overflowDelegate), Marshal.GetFunctionPointerForDelegate(systemDelegate));
				RCSSDKPINVOKE.SWIGRegisterExceptionArgumentCallbacks_RCSSDK(Marshal.GetFunctionPointerForDelegate(argumentDelegate), Marshal.GetFunctionPointerForDelegate(argumentNullDelegate), Marshal.GetFunctionPointerForDelegate(argumentOutOfRangeDelegate));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionDelegate))]
			private static void SetPendingApplicationException(string message)
			{
				SWIGPendingException.Set(new ApplicationException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionDelegate))]
			private static void SetPendingArithmeticException(string message)
			{
				SWIGPendingException.Set(new ArithmeticException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionDelegate))]
			private static void SetPendingDivideByZeroException(string message)
			{
				SWIGPendingException.Set(new DivideByZeroException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionDelegate))]
			private static void SetPendingIndexOutOfRangeException(string message)
			{
				SWIGPendingException.Set(new IndexOutOfRangeException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionDelegate))]
			private static void SetPendingInvalidCastException(string message)
			{
				SWIGPendingException.Set(new InvalidCastException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionDelegate))]
			private static void SetPendingInvalidOperationException(string message)
			{
				SWIGPendingException.Set(new InvalidOperationException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionDelegate))]
			private static void SetPendingIOException(string message)
			{
				SWIGPendingException.Set(new IOException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionDelegate))]
			private static void SetPendingNullReferenceException(string message)
			{
				SWIGPendingException.Set(new NullReferenceException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionDelegate))]
			private static void SetPendingOutOfMemoryException(string message)
			{
				SWIGPendingException.Set(new OutOfMemoryException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionDelegate))]
			private static void SetPendingOverflowException(string message)
			{
				SWIGPendingException.Set(new OverflowException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionDelegate))]
			private static void SetPendingSystemException(string message)
			{
				SWIGPendingException.Set(new SystemException(message, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionArgumentDelegate))]
			private static void SetPendingArgumentException(string message, string paramName)
			{
				SWIGPendingException.Set(new ArgumentException(message, paramName, SWIGPendingException.Retrieve()));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionArgumentDelegate))]
			private static void SetPendingArgumentNullException(string message, string paramName)
			{
				Exception ex = SWIGPendingException.Retrieve();
				if (ex != null)
				{
					message = message + " Inner Exception: " + ex.Message;
				}
				SWIGPendingException.Set(new ArgumentNullException(paramName, message));
			}

			[MonoPInvokeCallback(typeof(SWIGExceptionArgumentDelegate))]
			private static void SetPendingArgumentOutOfRangeException(string message, string paramName)
			{
				Exception ex = SWIGPendingException.Retrieve();
				if (ex != null)
				{
					message = message + " Inner Exception: " + ex.Message;
				}
				SWIGPendingException.Set(new ArgumentOutOfRangeException(paramName, message));
			}
		}

		public class SWIGPendingException
		{
			[ThreadStatic]
			private static Exception pendingException;

			private static int numExceptionsPending;

			public static bool Pending
			{
				get
				{
					bool result = false;
					if (numExceptionsPending > 0 && pendingException != null)
					{
						result = true;
					}
					return result;
				}
			}

			public static void Set(Exception e)
			{
				if (pendingException != null)
				{
					throw new ApplicationException("FATAL: An earlier pending exception from unmanaged code was missed and thus not thrown (" + pendingException.ToString() + ")", e);
				}
				pendingException = e;
				lock (typeof(RCSSDK))
				{
					numExceptionsPending++;
				}
			}

			public static Exception Retrieve()
			{
				Exception result = null;
				if (numExceptionsPending > 0 && pendingException != null)
				{
					result = pendingException;
					pendingException = null;
					lock (typeof(RCSSDK))
					{
						numExceptionsPending--;
					}
				}
				return result;
			}
		}

		protected class SWIGStringHelper
		{
			private static SWIGStringDelegate stringDelegate;

			static SWIGStringHelper()
			{
				stringDelegate = CreateString;
				RCSSDKPINVOKE.SWIGRegisterStringCallback_RCSSDK(Marshal.GetFunctionPointerForDelegate(stringDelegate));
			}

			[MonoPInvokeCallback(typeof(SWIGStringDelegate))]
			private static string CreateString(string cString)
			{
				return cString;
			}
		}

		public delegate void SWIGExceptionDelegate(string message);

		public delegate void SWIGExceptionArgumentDelegate(string message, string paramName);

		public delegate string SWIGStringDelegate(string message);

		public delegate void Logger(string debugInfo);

		protected static SWIGExceptionHelper swigExceptionHelper;

		protected static SWIGStringHelper swigStringHelper;

		public static readonly int RovioSdkMajor;

		public static readonly int RovioSdkMinor;

		public static readonly int RovioSdkRevision;

		public static readonly int RovioSdkHotfix;

		public static readonly int RovioSdkVersion;

		public static readonly int SocialNetworkReturnCodeDefaultValue;

		public static Version SdkVersion
		{
			get
			{
				IntPtr intPtr = RCSSDKPINVOKE.SdkVersion_get();
				return (!(intPtr == IntPtr.Zero)) ? new Version(intPtr, false) : null;
			}
		}

		public static Version EngineVersion
		{
			get
			{
				IntPtr intPtr = RCSSDKPINVOKE.EngineVersion_get();
				return (!(intPtr == IntPtr.Zero)) ? new Version(intPtr, false) : null;
			}
		}

		public static string FacebookService
		{
			get
			{
				return RCSSDKPINVOKE.FacebookService_get();
			}
		}

		public static string OtherService
		{
			get
			{
				return RCSSDKPINVOKE.OtherService_get();
			}
		}

		public static string PlatformService
		{
			get
			{
				return RCSSDKPINVOKE.PlatformService_get();
			}
		}

		public static string DigitsService
		{
			get
			{
				return RCSSDKPINVOKE.DigitsService_get();
			}
		}

		public static string DefaultChannelGroupId
		{
			get
			{
				return RCSSDKPINVOKE.DefaultChannelGroupId_get();
			}
		}

		static RCSSDK()
		{
			swigExceptionHelper = new SWIGExceptionHelper();
			swigStringHelper = new SWIGStringHelper();
			RovioSdkMajor = RCSSDKPINVOKE.RovioSdkMajor_get();
			RovioSdkMinor = RCSSDKPINVOKE.RovioSdkMinor_get();
			RovioSdkRevision = RCSSDKPINVOKE.RovioSdkRevision_get();
			RovioSdkHotfix = RCSSDKPINVOKE.RovioSdkHotfix_get();
			RovioSdkVersion = RCSSDKPINVOKE.RovioSdkVersion_get();
			SocialNetworkReturnCodeDefaultValue = RCSSDKPINVOKE.SocialNetworkReturnCodeDefaultValue_get();
		}

		public static void Initialize(string publisherName, string productName)
		{
			Application.Initialize(publisherName, productName);
		}

		public static void RemoveSessionRefreshToken()
		{
			RCSSDKPINVOKE.RemoveSessionRefreshToken();
		}

		public static bool LeaderboardScores_EQU(Leaderboard.Score a, Leaderboard.Score b)
		{
			bool result = RCSSDKPINVOKE.LeaderboardScores_EQU(Leaderboard.Score.getCPtr(a), Leaderboard.Score.getCPtr(b));
			if (SWIGPendingException.Pending)
			{
				throw SWIGPendingException.Retrieve();
			}
			return result;
		}

		public static bool LeaderboardScores_NEQ(Leaderboard.Score a, Leaderboard.Score b)
		{
			bool result = RCSSDKPINVOKE.LeaderboardScores_NEQ(Leaderboard.Score.getCPtr(a), Leaderboard.Score.getCPtr(b));
			if (SWIGPendingException.Pending)
			{
				throw SWIGPendingException.Retrieve();
			}
			return result;
		}

		public static bool LeaderboardScores_LT(Leaderboard.Score a, Leaderboard.Score b)
		{
			bool result = RCSSDKPINVOKE.LeaderboardScores_LT(Leaderboard.Score.getCPtr(a), Leaderboard.Score.getCPtr(b));
			if (SWIGPendingException.Pending)
			{
				throw SWIGPendingException.Retrieve();
			}
			return result;
		}

		public static bool LeaderboardScores_LTE(Leaderboard.Score a, Leaderboard.Score b)
		{
			bool result = RCSSDKPINVOKE.LeaderboardScores_LTE(Leaderboard.Score.getCPtr(a), Leaderboard.Score.getCPtr(b));
			if (SWIGPendingException.Pending)
			{
				throw SWIGPendingException.Retrieve();
			}
			return result;
		}

		public static bool LeaderboardScores_GT(Leaderboard.Score a, Leaderboard.Score b)
		{
			bool result = RCSSDKPINVOKE.LeaderboardScores_GT(Leaderboard.Score.getCPtr(a), Leaderboard.Score.getCPtr(b));
			if (SWIGPendingException.Pending)
			{
				throw SWIGPendingException.Retrieve();
			}
			return result;
		}

		public static bool LeaderboardScores_GTE(Leaderboard.Score a, Leaderboard.Score b)
		{
			bool result = RCSSDKPINVOKE.LeaderboardScores_GTE(Leaderboard.Score.getCPtr(a), Leaderboard.Score.getCPtr(b));
			if (SWIGPendingException.Pending)
			{
				throw SWIGPendingException.Retrieve();
			}
			return result;
		}
	}
}
