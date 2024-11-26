using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class Leaderboard : IDisposable
	{
		public class Score : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			internal Score(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Score(string levelName)
				: this(RCSSDKPINVOKE.new_Leaderboard_Score_0(levelName), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public Score(Score other)
				: this(RCSSDKPINVOKE.new_Leaderboard_Score_1(getCPtr(other)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public Score()
				: this(RCSSDKPINVOKE.new_Leaderboard_Score_2(), true)
			{
			}

			public Score(string levelName, string accountId)
				: this(RCSSDKPINVOKE.new_Leaderboard_Score_3(levelName, accountId), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(Score obj)
			{
				return (!(obj == null)) ? obj.swigCPtr : IntPtr.Zero;
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!disposed)
				{
					_DisposeUnmanaged();
					disposed = true;
				}
			}

			~Score()
			{
				Dispose(false);
			}

			private void _DisposeUnmanaged()
			{
				lock (this)
				{
					if (swigCPtr != IntPtr.Zero)
					{
						if (swigCMemOwn)
						{
							swigCMemOwn = false;
							RCSSDKPINVOKE.delete_Leaderboard_Score(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public override bool Equals(object obj)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				if (obj == null)
				{
					return false;
				}
				return RCSSDK.LeaderboardScores_EQU(this, (Score)obj);
			}

			public override int GetHashCode()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return base.GetHashCode();
			}

			public string GetAccountId()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Leaderboard_Score_GetAccountId(swigCPtr);
			}

			public string GetLevelName()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Leaderboard_Score_GetLevelName(swigCPtr);
			}

			public void SetPoints(long points)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Leaderboard_Score_SetPoints(swigCPtr, points);
			}

			public long GetPoints()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Leaderboard_Score_GetPoints(swigCPtr);
			}

			public void SetProperty(string key, string value)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				RCSSDKPINVOKE.Leaderboard_Score_SetProperty(swigCPtr, key, value);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public bool HasProperty(string name)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				bool result = RCSSDKPINVOKE.Leaderboard_Score_HasProperty(swigCPtr, name);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
				return result;
			}

			public string GetProperty(string key)
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				string result = RCSSDKPINVOKE.Leaderboard_Score_GetProperty(swigCPtr, key);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
				return result;
			}

			public Dictionary<string, string> GetProperties()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				StringDict srcDict = new StringDict(RCSSDKPINVOKE.Leaderboard_Score_GetProperties(swigCPtr), false);
				return srcDict.ToDictionary();
			}

			public static Score FromString(string score)
			{
				Score result = new Score(RCSSDKPINVOKE.Leaderboard_Score_FromString(score), true);
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
				return result;
			}

			public override string ToString()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Leaderboard_Score_ToString(swigCPtr);
			}

			public static bool operator ==(Score a, Score b)
			{
				if ((object)a == null || (object)b == null)
				{
					return (object)a == b;
				}
				return RCSSDK.LeaderboardScores_EQU(a, b);
			}

			public static bool operator !=(Score a, Score b)
			{
				if ((object)a == null || (object)b == null)
				{
					return (object)a != b;
				}
				return RCSSDK.LeaderboardScores_NEQ(a, b);
			}

			public static bool operator >=(Score a, Score b)
			{
				return RCSSDK.LeaderboardScores_GTE(a, b);
			}

			public static bool operator <=(Score a, Score b)
			{
				return RCSSDK.LeaderboardScores_LTE(a, b);
			}

			public static bool operator >(Score a, Score b)
			{
				return RCSSDK.LeaderboardScores_GT(a, b);
			}

			public static bool operator <(Score a, Score b)
			{
				return RCSSDK.LeaderboardScores_LT(a, b);
			}
		}

		public class Result : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			internal Result(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Result()
				: this(RCSSDKPINVOKE.new_Leaderboard_Result_0(), true)
			{
			}

			public Result(long rank, Score score)
				: this(RCSSDKPINVOKE.new_Leaderboard_Result_1(rank, Score.getCPtr(score)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			public Result(Result other)
				: this(RCSSDKPINVOKE.new_Leaderboard_Result_2(getCPtr(other)), true)
			{
				if (RCSSDK.SWIGPendingException.Pending)
				{
					throw RCSSDK.SWIGPendingException.Retrieve();
				}
			}

			internal static IntPtr getCPtr(Result obj)
			{
				return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!disposed)
				{
					_DisposeUnmanaged();
					disposed = true;
				}
			}

			~Result()
			{
				Dispose(false);
			}

			private void _DisposeUnmanaged()
			{
				lock (this)
				{
					if (swigCPtr != IntPtr.Zero)
					{
						if (swigCMemOwn)
						{
							swigCMemOwn = false;
							RCSSDKPINVOKE.delete_Leaderboard_Result(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public long GetRank()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return RCSSDKPINVOKE.Leaderboard_Result_GetRank(swigCPtr);
			}

			public Score GetScore()
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				Score other = new Score(RCSSDKPINVOKE.Leaderboard_Result_GetScore(swigCPtr), false);
				return new Score(other);
			}
		}

		public enum ErrorCode
		{
			ErrorNoSuchLevel = 0,
			ErrorInvalidParameters = 1,
			ErrorNetworkFailure = 2,
			ErrorOtherReason = 3
		}

		public delegate void ScoresFetchedCallback(List<Result> results);

		public delegate void ScoreSubmittedCallback();

		public delegate void ScoreFetchedCallback(Result result);

		public delegate void ErrorCallback(ErrorCode errorCode);

		private delegate void SwigDelegateLeaderboard_0(IntPtr cb, IntPtr results);

		private delegate void SwigDelegateLeaderboard_1(IntPtr cb);

		private delegate void SwigDelegateLeaderboard_2(IntPtr cb, IntPtr result);

		private delegate void SwigDelegateLeaderboard_3(IntPtr cb, int errorCode);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateLeaderboard_0 swigDelegate0;

		private SwigDelegateLeaderboard_1 swigDelegate1;

		private SwigDelegateLeaderboard_2 swigDelegate2;

		private SwigDelegateLeaderboard_3 swigDelegate3;

		internal Leaderboard(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public Leaderboard(IdentitySessionBase identity)
			: this(RCSSDKPINVOKE.new_Leaderboard(identity.SharedPtr.CPtr), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<Leaderboard> callInfo)
		{
			lock (this)
			{
				IntPtr intPtr = callInfo.Pin();
				pendingCallbacks.Add(intPtr);
				return intPtr;
			}
		}

		private void RemovePendingCallback(IntPtr callbackInfoId)
		{
			lock (this)
			{
				pendingCallbacks.Remove(callbackInfoId);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			_DisposeUnmanaged();
			lock (this)
			{
				foreach (IntPtr pendingCallback in pendingCallbacks)
				{
					GCHandle.FromIntPtr(pendingCallback).Free();
				}
				pendingCallbacks.Clear();
			}
			disposed = true;
		}

		internal static IntPtr getCPtr(Leaderboard obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~Leaderboard()
		{
			Dispose(false);
		}

		private void _DisposeUnmanaged()
		{
			lock (this)
			{
				if (swigCPtr != IntPtr.Zero)
				{
					if (swigCMemOwn)
					{
						swigCMemOwn = false;
						RCSSDKPINVOKE.delete_Leaderboard(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void SubmitScore(Score score, ScoreSubmittedCallback onSubmitted, ErrorCallback onError)
		{
			AsyncCallInfo<Leaderboard> asyncCallInfo = new AsyncCallInfo<Leaderboard>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSubmitted);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Leaderboard_SubmitScore(swigCPtr, Score.getCPtr(score), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SubmitScores(List<Score> scores, ScoreSubmittedCallback onSubmitted, ErrorCallback onError)
		{
			AsyncCallInfo<Leaderboard> asyncCallInfo = new AsyncCallInfo<Leaderboard>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onSubmitted);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Leaderboard_SubmitScores(swigCPtr, LeaderboardScores.getCPtr(new LeaderboardScores(scores)), intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void FetchScore(string levelName, ScoreFetchedCallback onFetched, ErrorCallback onError)
		{
			AsyncCallInfo<Leaderboard> asyncCallInfo = new AsyncCallInfo<Leaderboard>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onFetched);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Leaderboard_FetchScore(swigCPtr, levelName, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void FetchScores(List<string> accountIds, string levelName, ScoresFetchedCallback onFetched, ErrorCallback onError)
		{
			AsyncCallInfo<Leaderboard> asyncCallInfo = new AsyncCallInfo<Leaderboard>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onFetched);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Leaderboard_FetchScores(swigCPtr, StringList.getCPtr(new StringList(accountIds)), levelName, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void Matchmake(string levelName, int offset, uint limit, ScoresFetchedCallback onFetched, ErrorCallback onError)
		{
			AsyncCallInfo<Leaderboard> asyncCallInfo = new AsyncCallInfo<Leaderboard>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onFetched);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Leaderboard_Matchmake(swigCPtr, levelName, offset, limit, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void FetchTopScores(string levelName, uint fetchLimit, ScoresFetchedCallback onFetched, ErrorCallback onError)
		{
			AsyncCallInfo<Leaderboard> asyncCallInfo = new AsyncCallInfo<Leaderboard>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onFetched);
			asyncCallInfo.AddHandler(onError);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Leaderboard_FetchTopScores(swigCPtr, levelName, fetchLimit, intPtr, intPtr);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		private static void OnScoresFetchedCallback(ScoresFetchedCallback cb, List<Result> results)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(new LeaderboardResults(results).ToList());
		}

		private static void OnScoreSubmittedCallback(ScoreSubmittedCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private static void OnScoreFetchedCallback(ScoreFetchedCallback cb, Result result)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(result);
		}

		private static void OnErrorCallback(ErrorCallback cb, ErrorCode errorCode)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(errorCode);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnScoresFetchedCallback;
			swigDelegate1 = SwigDirectorOnScoreSubmittedCallback;
			swigDelegate2 = SwigDirectorOnScoreFetchedCallback;
			swigDelegate3 = SwigDirectorOnErrorCallback;
			RCSSDKPINVOKE.Leaderboard_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1), Marshal.GetFunctionPointerForDelegate(swigDelegate2), Marshal.GetFunctionPointerForDelegate(swigDelegate3));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(Leaderboard));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateLeaderboard_0))]
		private static void SwigDirectorOnScoresFetchedCallback(IntPtr cb, IntPtr results)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Leaderboard] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Leaderboard> asyncCallInfo = (AsyncCallInfo<Leaderboard>)gCHandle.Target;
			ScoresFetchedCallback handler = asyncCallInfo.GetHandler<ScoresFetchedCallback>();
			try
			{
				OnScoresFetchedCallback(handler, new LeaderboardResults(results, false).ToList());
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateLeaderboard_1))]
		private static void SwigDirectorOnScoreSubmittedCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Leaderboard] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Leaderboard> asyncCallInfo = (AsyncCallInfo<Leaderboard>)gCHandle.Target;
			ScoreSubmittedCallback handler = asyncCallInfo.GetHandler<ScoreSubmittedCallback>();
			try
			{
				OnScoreSubmittedCallback(handler);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateLeaderboard_2))]
		private static void SwigDirectorOnScoreFetchedCallback(IntPtr cb, IntPtr result)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Leaderboard] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Leaderboard> asyncCallInfo = (AsyncCallInfo<Leaderboard>)gCHandle.Target;
			ScoreFetchedCallback handler = asyncCallInfo.GetHandler<ScoreFetchedCallback>();
			try
			{
				OnScoreFetchedCallback(handler, new Result(result, false));
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateLeaderboard_3))]
		private static void SwigDirectorOnErrorCallback(IntPtr cb, int errorCode)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.Leaderboard] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<Leaderboard> asyncCallInfo = (AsyncCallInfo<Leaderboard>)gCHandle.Target;
			ErrorCallback handler = asyncCallInfo.GetHandler<ErrorCallback>();
			try
			{
				OnErrorCallback(handler, (ErrorCode)errorCode);
			}
			finally
			{
				if (!"ErrorCallback".Contains("Progress"))
				{
					gCHandle.Free();
					asyncCallInfo.Service.RemovePendingCallback(cb);
				}
			}
		}
	}
}
