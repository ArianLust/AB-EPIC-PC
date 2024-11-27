using System;
using System.Collections.Generic;

namespace Rcs
{
	public class PlayerData : IDisposable
	{
		public enum Gender
		{
			GenderUnknown = 0,
			GenderMale = 1,
			GenderFemale = 2
		}

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal PlayerData(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public PlayerData()
			: this(RCSSDKPINVOKE.new_PlayerData_0(), true)
		{
		}

		public PlayerData(PlayerData arg0)
			: this(RCSSDKPINVOKE.new_PlayerData_1(getCPtr(arg0)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		internal static IntPtr getCPtr(PlayerData obj)
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

		~PlayerData()
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
						RCSSDKPINVOKE.delete_PlayerData(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public Dictionary<string, string> GetPublic()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			StringDict srcDict = new StringDict(RCSSDKPINVOKE.PlayerData_GetPublic(swigCPtr), false);
			return srcDict.ToDictionary();
		}

		public Dictionary<string, string> GetPrivate()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			StringDict srcDict = new StringDict(RCSSDKPINVOKE.PlayerData_GetPrivate(swigCPtr), false);
			return srcDict.ToDictionary();
		}

		public bool SetPublic(Dictionary<string, string> data)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			bool result = RCSSDKPINVOKE.PlayerData_SetPublic_0(swigCPtr, StringDict.getCPtr(data.ToSwigDict()));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public bool SetPublic(string key, string value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			bool result = RCSSDKPINVOKE.PlayerData_SetPublic_1(swigCPtr, key, value);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public bool SetPrivate(Dictionary<string, string> data)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			bool result = RCSSDKPINVOKE.PlayerData_SetPrivate_0(swigCPtr, StringDict.getCPtr(data.ToSwigDict()));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public bool SetPrivate(string key, string value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			bool result = RCSSDKPINVOKE.PlayerData_SetPrivate_1(swigCPtr, key, value);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void SetBirthday(string date)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.PlayerData_SetBirthday(swigCPtr, date);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public string GetBirthday()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.PlayerData_GetBirthday(swigCPtr);
		}

		public void SetGender(Gender gender)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.PlayerData_SetGender(swigCPtr, (int)gender);
		}

		public Gender GetGender()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return (Gender)RCSSDKPINVOKE.PlayerData_GetGender(swigCPtr);
		}
	}
}
