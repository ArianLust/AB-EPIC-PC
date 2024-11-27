using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Rcs
{
	public class AgeGenderQuery : IDisposable
	{
		public class Color : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public byte R
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.AgeGenderQuery_Color_R_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.AgeGenderQuery_Color_R_set(swigCPtr, value);
				}
			}

			public byte G
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.AgeGenderQuery_Color_G_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.AgeGenderQuery_Color_G_set(swigCPtr, value);
				}
			}

			public byte B
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.AgeGenderQuery_Color_B_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.AgeGenderQuery_Color_B_set(swigCPtr, value);
				}
			}

			public byte A
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					return RCSSDKPINVOKE.AgeGenderQuery_Color_A_get(swigCPtr);
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.AgeGenderQuery_Color_A_set(swigCPtr, value);
				}
			}

			internal Color(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Color(byte red, byte green, byte blue, byte alpha)
				: this(RCSSDKPINVOKE.new_AgeGenderQuery_Color_0(red, green, blue, alpha), true)
			{
			}

			public Color(byte red, byte green, byte blue)
				: this(RCSSDKPINVOKE.new_AgeGenderQuery_Color_1(red, green, blue), true)
			{
			}

			internal static IntPtr getCPtr(Color obj)
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

			~Color()
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
							RCSSDKPINVOKE.delete_AgeGenderQuery_Color(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}

			public static Color White()
			{
				return new Color(RCSSDKPINVOKE.AgeGenderQuery_Color_White(), true);
			}

			public static Color Black()
			{
				return new Color(RCSSDKPINVOKE.AgeGenderQuery_Color_Black(), true);
			}

			public static Color Red()
			{
				return new Color(RCSSDKPINVOKE.AgeGenderQuery_Color_Red(), true);
			}

			public static Color Green()
			{
				return new Color(RCSSDKPINVOKE.AgeGenderQuery_Color_Green(), true);
			}

			public static Color Blue()
			{
				return new Color(RCSSDKPINVOKE.AgeGenderQuery_Color_Blue(), true);
			}

			public static Color Yellow()
			{
				return new Color(RCSSDKPINVOKE.AgeGenderQuery_Color_Yellow(), true);
			}
		}

		public delegate void CancelledCallback();

		public delegate void CompletedCallback(int age, string dateOfBirth, string gender);

		private delegate void SwigDelegateAgeGenderQuery_0(IntPtr cb);

		private delegate void SwigDelegateAgeGenderQuery_1(IntPtr cb, int age, string dateOfBirth, string gender);

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		private List<IntPtr> pendingCallbacks = new List<IntPtr>();

		private SwigDelegateAgeGenderQuery_0 swigDelegate0;

		private SwigDelegateAgeGenderQuery_1 swigDelegate1;

		internal AgeGenderQuery(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
			SwigDirectorConnect();
		}

		public AgeGenderQuery(bool cancellable, string titleText)
			: this(RCSSDKPINVOKE.new_AgeGenderQuery_0(cancellable, titleText), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			SwigDirectorConnect();
		}

		public AgeGenderQuery(bool cancellable)
			: this(RCSSDKPINVOKE.new_AgeGenderQuery_1(cancellable), true)
		{
			SwigDirectorConnect();
		}

		public AgeGenderQuery()
			: this(RCSSDKPINVOKE.new_AgeGenderQuery_2(), true)
		{
			SwigDirectorConnect();
		}

		private IntPtr AddPendingCallback(AsyncCallInfo<AgeGenderQuery> callInfo)
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

		internal static IntPtr getCPtr(AgeGenderQuery obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		~AgeGenderQuery()
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
						RCSSDKPINVOKE.delete_AgeGenderQuery(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void SetRequiresGender(bool requiresGender)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.AgeGenderQuery_SetRequiresGender(swigCPtr, requiresGender);
		}

		public void Show(CompletedCallback onCompleted, CancelledCallback onCancelled)
		{
			AsyncCallInfo<AgeGenderQuery> asyncCallInfo = new AsyncCallInfo<AgeGenderQuery>(this);
			IntPtr intPtr = AddPendingCallback(asyncCallInfo);
			asyncCallInfo.AddHandler(onCompleted);
			asyncCallInfo.AddHandler(onCancelled);
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.AgeGenderQuery_Show(swigCPtr, intPtr, intPtr);
		}

		public void SetTextColor(Color color)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.AgeGenderQuery_SetTextColor(swigCPtr, Color.getCPtr(color));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetButtonTextColor(Color color)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.AgeGenderQuery_SetButtonTextColor(swigCPtr, Color.getCPtr(color));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetHighlightColor(Color color)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.AgeGenderQuery_SetHighlightColor(swigCPtr, Color.getCPtr(color));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		private static void OnCancelledCallback(CancelledCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb();
		}

		private static void OnCompletedCallback(CompletedCallback cb, int age, string dateOfBirth, string gender)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			cb(age, dateOfBirth, gender);
		}

		private void SwigDirectorConnect()
		{
			swigDelegate0 = SwigDirectorOnCancelledCallback;
			swigDelegate1 = SwigDirectorOnCompletedCallback;
			RCSSDKPINVOKE.AgeGenderQuery_director_connect(swigCPtr, Marshal.GetFunctionPointerForDelegate(swigDelegate0), Marshal.GetFunctionPointerForDelegate(swigDelegate1));
		}

		private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
		{
			MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
			return method.DeclaringType.IsSubclassOf(typeof(AgeGenderQuery));
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAgeGenderQuery_0))]
		private static void SwigDirectorOnCancelledCallback(IntPtr cb)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.AgeGenderQuery] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<AgeGenderQuery> asyncCallInfo = (AsyncCallInfo<AgeGenderQuery>)gCHandle.Target;
			CancelledCallback handler = asyncCallInfo.GetHandler<CancelledCallback>();
			try
			{
				OnCancelledCallback(handler);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}

		[MonoPInvokeCallback(typeof(SwigDelegateAgeGenderQuery_1))]
		private static void SwigDirectorOnCompletedCallback(IntPtr cb, int age, string dateOfBirth, string gender)
		{
			GCHandle gCHandle;
			try
			{
				gCHandle = GCHandle.FromIntPtr(cb);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("[Rcs.AgeGenderQuery] Ignoring callback from previously disposed object instance");
				return;
			}
			AsyncCallInfo<AgeGenderQuery> asyncCallInfo = (AsyncCallInfo<AgeGenderQuery>)gCHandle.Target;
			CompletedCallback handler = asyncCallInfo.GetHandler<CompletedCallback>();
			try
			{
				OnCompletedCallback(handler, age, dateOfBirth, gender);
			}
			finally
			{
				gCHandle.Free();
				asyncCallInfo.Service.RemovePendingCallback(cb);
			}
		}
	}
}
