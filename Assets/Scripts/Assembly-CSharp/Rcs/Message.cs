using System;
using System.Collections.Generic;

namespace Rcs
{
	public class Message : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		internal Message(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public Message(string content)
			: this(RCSSDKPINVOKE.new_Message_0(content), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public Message(Message other)
			: this(RCSSDKPINVOKE.new_Message_1(getCPtr(other)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public Message()
			: this(RCSSDKPINVOKE.new_Message_2(), true)
		{
		}

		public Message(string messageType, string messageId, string cursor, string creatorId, string senderId, string content, ulong timestamp, Dictionary<string, string> custom)
			: this(RCSSDKPINVOKE.new_Message_3(messageType, messageId, cursor, creatorId, senderId, content, timestamp, StringDict.getCPtr(custom.ToSwigDict())), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		internal static IntPtr getCPtr(Message obj)
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

		~Message()
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
						RCSSDKPINVOKE.delete_Message(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public string GetMessageType()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Message_GetMessageType(swigCPtr);
		}

		public string GetId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Message_GetId(swigCPtr);
		}

		public string GetCreatorId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Message_GetCreatorId(swigCPtr);
		}

		public string GetSenderId()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Message_GetSenderId(swigCPtr);
		}

		public string GetCursor()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Message_GetCursor(swigCPtr);
		}

		public ulong GetTimestamp()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Message_GetTimestamp(swigCPtr);
		}

		public string GetContent()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.Message_GetContent(swigCPtr);
		}

		public string GetCustom(string key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			string result = RCSSDKPINVOKE.Message_GetCustom_0(swigCPtr, key);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public Dictionary<string, string> GetCustom()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			StringDict srcDict = new StringDict(RCSSDKPINVOKE.Message_GetCustom_1(swigCPtr), false);
			return srcDict.ToDictionary();
		}

		public void SetId(string messageId)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.Message_SetId(swigCPtr, messageId);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}
	}
}
