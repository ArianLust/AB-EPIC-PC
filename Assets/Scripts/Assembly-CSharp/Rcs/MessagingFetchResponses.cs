using System;
using System.Collections;
using System.Collections.Generic;

namespace Rcs
{
	public class MessagingFetchResponses : IDisposable, IEnumerable, IEnumerable<Messaging.FetchResponse>
	{
		public sealed class MessagingFetchResponsesEnumerator : IDisposable, IEnumerator, IEnumerator<Messaging.FetchResponse>
		{
			private MessagingFetchResponses collectionRef;

			private int currentIndex;

			private object currentObject;

			private int currentSize;

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public Messaging.FetchResponse Current
			{
				get
				{
					if (currentIndex == -1)
					{
						throw new InvalidOperationException("Enumeration not started.");
					}
					if (currentIndex > currentSize - 1)
					{
						throw new InvalidOperationException("Enumeration finished.");
					}
					if (currentObject == null)
					{
						throw new InvalidOperationException("Collection modified.");
					}
					return (Messaging.FetchResponse)currentObject;
				}
			}

			public MessagingFetchResponsesEnumerator(MessagingFetchResponses collection)
			{
				collectionRef = collection;
				currentIndex = -1;
				currentObject = null;
				currentSize = collectionRef.Count;
			}

			public bool MoveNext()
			{
				int count = collectionRef.Count;
				bool flag = currentIndex + 1 < count && count == currentSize;
				if (flag)
				{
					currentIndex++;
					currentObject = collectionRef[currentIndex];
				}
				else
				{
					currentObject = null;
				}
				return flag;
			}

			public void Reset()
			{
				currentIndex = -1;
				currentObject = null;
				if (collectionRef.Count != currentSize)
				{
					throw new InvalidOperationException("Collection modified.");
				}
			}

			public void Dispose()
			{
				currentIndex = -1;
				currentObject = null;
			}
		}

		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		public bool IsFixedSize
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return false;
			}
		}

		public Messaging.FetchResponse this[int index]
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return getitem(index);
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				setitem(index, value);
			}
		}

		public int Capacity
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return (int)capacity();
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				if (value < size())
				{
					throw new ArgumentOutOfRangeException("Capacity");
				}
				reserve((uint)value);
			}
		}

		public int Count
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return (int)size();
			}
		}

		public bool IsSynchronized
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return false;
			}
		}

		internal MessagingFetchResponses(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public MessagingFetchResponses(ICollection c)
			: this()
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			foreach (Messaging.FetchResponse item in c)
			{
				Add(item);
			}
		}

		public MessagingFetchResponses()
			: this(RCSSDKPINVOKE.new_MessagingFetchResponses_0(), true)
		{
		}

		public MessagingFetchResponses(MessagingFetchResponses other)
			: this(RCSSDKPINVOKE.new_MessagingFetchResponses_1(getCPtr(other)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public MessagingFetchResponses(int capacity)
			: this(RCSSDKPINVOKE.new_MessagingFetchResponses_2(capacity), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		IEnumerator<Messaging.FetchResponse> IEnumerable<Messaging.FetchResponse>.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new MessagingFetchResponsesEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new MessagingFetchResponsesEnumerator(this);
		}

		internal static IntPtr getCPtr(MessagingFetchResponses obj)
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

		~MessagingFetchResponses()
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
						RCSSDKPINVOKE.delete_MessagingFetchResponses(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void CopyTo(Messaging.FetchResponse[] array)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			CopyTo(0, array, 0, Count);
		}

		public void CopyTo(Messaging.FetchResponse[] array, int arrayIndex)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			CopyTo(0, array, arrayIndex, Count);
		}

		public void CopyTo(int index, Messaging.FetchResponse[] array, int arrayIndex, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Value is less than zero");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", "Value is less than zero");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Value is less than zero");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("Multi dimensional array.", "array");
			}
			if (index + count > Count || arrayIndex + count > array.Length)
			{
				throw new ArgumentException("Number of elements to copy is too large.");
			}
			for (int i = 0; i < count; i++)
			{
				array.SetValue(getitemcopy(index + i), arrayIndex + i);
			}
		}

		public MessagingFetchResponsesEnumerator GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new MessagingFetchResponsesEnumerator(this);
		}

		public void Clear()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_Clear(swigCPtr);
		}

		public void Add(Messaging.FetchResponse x)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_Add(swigCPtr, Messaging.FetchResponse.getCPtr(x));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		private uint size()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.MessagingFetchResponses_size(swigCPtr);
		}

		private uint capacity()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.MessagingFetchResponses_capacity(swigCPtr);
		}

		private void reserve(uint n)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_reserve(swigCPtr, n);
		}

		private Messaging.FetchResponse getitemcopy(int index)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			Messaging.FetchResponse result = new Messaging.FetchResponse(RCSSDKPINVOKE.MessagingFetchResponses_getitemcopy(swigCPtr, index), true);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private Messaging.FetchResponse getitem(int index)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			Messaging.FetchResponse result = new Messaging.FetchResponse(RCSSDKPINVOKE.MessagingFetchResponses_getitem(swigCPtr, index), false);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private void setitem(int index, Messaging.FetchResponse val)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_setitem(swigCPtr, index, Messaging.FetchResponse.getCPtr(val));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void AddRange(MessagingFetchResponses values)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_AddRange(swigCPtr, getCPtr(values));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public MessagingFetchResponses GetRange(int index, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr intPtr = RCSSDKPINVOKE.MessagingFetchResponses_GetRange(swigCPtr, index, count);
			MessagingFetchResponses result = ((!(intPtr == IntPtr.Zero)) ? new MessagingFetchResponses(intPtr, true) : null);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void Insert(int index, Messaging.FetchResponse x)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_Insert(swigCPtr, index, Messaging.FetchResponse.getCPtr(x));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void InsertRange(int index, MessagingFetchResponses values)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_InsertRange(swigCPtr, index, getCPtr(values));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void RemoveAt(int index)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_RemoveAt(swigCPtr, index);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void RemoveRange(int index, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_RemoveRange(swigCPtr, index, count);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public static MessagingFetchResponses Repeat(Messaging.FetchResponse value, int count)
		{
			IntPtr intPtr = RCSSDKPINVOKE.MessagingFetchResponses_Repeat(Messaging.FetchResponse.getCPtr(value), count);
			MessagingFetchResponses result = ((!(intPtr == IntPtr.Zero)) ? new MessagingFetchResponses(intPtr, true) : null);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void Reverse()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_Reverse_0(swigCPtr);
		}

		public void Reverse(int index, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_Reverse_1(swigCPtr, index, count);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetRange(int index, MessagingFetchResponses values)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.MessagingFetchResponses_SetRange(swigCPtr, index, getCPtr(values));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}
	}
}
