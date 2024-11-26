using System;
using System.Collections.Generic;

namespace Rcs
{
	public class AppTrack : IDisposable
	{
		public class Params : IDisposable
		{
			private IntPtr swigCPtr;

			protected bool swigCMemOwn;

			private bool disposed;

			public string Vendor
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.AppTrack_Params_Vendor_get(swigCPtr);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
					return result;
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.AppTrack_Params_Vendor_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string PublisherId
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.AppTrack_Params_PublisherId_get(swigCPtr);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
					return result;
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.AppTrack_Params_PublisherId_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			public string ClientId
			{
				get
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					string result = RCSSDKPINVOKE.AppTrack_Params_ClientId_get(swigCPtr);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
					return result;
				}
				set
				{
					if (disposed)
					{
						throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
					}
					RCSSDKPINVOKE.AppTrack_Params_ClientId_set(swigCPtr, value);
					if (RCSSDK.SWIGPendingException.Pending)
					{
						throw RCSSDK.SWIGPendingException.Retrieve();
					}
				}
			}

			internal Params(IntPtr cPtr, bool cMemoryOwn)
			{
				swigCMemOwn = cMemoryOwn;
				swigCPtr = cPtr;
			}

			public Params()
				: this(RCSSDKPINVOKE.new_AppTrack_Params(), true)
			{
			}

			internal static IntPtr getCPtr(Params obj)
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

			~Params()
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
							RCSSDKPINVOKE.delete_AppTrack_Params(swigCPtr);
						}
						swigCPtr = IntPtr.Zero;
					}
				}
			}
		}

		public enum Event
		{
			Sale = 0,
			Signup = 1,
			TutorialPassed = 2,
			PlayerProgress1 = 3,
			PlayerProgress2 = 4,
			PlayerProgress3 = 5,
			PlayerProgress4 = 6,
			PlayerProgress5 = 7,
			PlayerProgress6 = 8,
			PlayerProgress7 = 9,
			PlayerProgress8 = 10,
			PlayerProgress9 = 11,
			PlayerProgress10 = 12
		}

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal AppTrack(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public AppTrack(IdentitySessionBase identity, Params arg1, bool debugMode)
		{
			IntPtr cPtr;
			if (identity == null)
			{
				SessionSharedPtr sessionSharedPtr = SwigTools.MakeSessionSharedPtr(IntPtr.Zero);
				cPtr = sessionSharedPtr.CPtr;
			}
			else
			{
				cPtr = identity.SharedPtr.CPtr;
			}
			swigCMemOwn = true;
			swigCPtr = RCSSDKPINVOKE.new_AppTrack(cPtr, Params.getCPtr(arg1), debugMode);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		internal static IntPtr getCPtr(AppTrack obj)
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

		~AppTrack()
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
						RCSSDKPINVOKE.delete_AppTrack(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void TrackEvent(Event arg0)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.AppTrack_TrackEvent(swigCPtr, (int)arg0);
		}

		public void TrackSale(string amount, string currency)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.AppTrack_TrackSale(swigCPtr, amount, currency);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void TrackCustomEvent(string eventToken)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.AppTrack_TrackCustomEvent(swigCPtr, eventToken);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetEventTokens(Dictionary<Event, string> eventTokens)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.AppTrack_SetEventTokens(swigCPtr, EventTokensDict.getCPtr(eventTokens.ToSwigDict()));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}
	}
}
