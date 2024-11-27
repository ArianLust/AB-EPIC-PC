using System;
using System.Collections;
using System.Collections.Generic;

namespace Rcs
{
	public class SocialNetworks : IDisposable, IEnumerable, IEnumerable<User.SocialNetwork>
	{
		public sealed class SocialNetworksEnumerator : IDisposable, IEnumerator, IEnumerator<User.SocialNetwork>
		{
			private SocialNetworks collectionRef;

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

			public User.SocialNetwork Current
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
					return (User.SocialNetwork)(int)currentObject;
				}
			}

			public SocialNetworksEnumerator(SocialNetworks collection)
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

		public User.SocialNetwork this[int index]
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

		internal SocialNetworks(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public SocialNetworks(ICollection c)
			: this()
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			foreach (int item in c)
			{
				Add((User.SocialNetwork)item);
			}
		}

		public SocialNetworks()
			: this(RCSSDKPINVOKE.new_SocialNetworks_0(), true)
		{
		}

		public SocialNetworks(SocialNetworks other)
			: this(RCSSDKPINVOKE.new_SocialNetworks_1(getCPtr(other)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public SocialNetworks(int capacity)
			: this(RCSSDKPINVOKE.new_SocialNetworks_2(capacity), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		IEnumerator<User.SocialNetwork> IEnumerable<User.SocialNetwork>.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new SocialNetworksEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new SocialNetworksEnumerator(this);
		}

		internal static IntPtr getCPtr(SocialNetworks obj)
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

		~SocialNetworks()
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
						RCSSDKPINVOKE.delete_SocialNetworks(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void CopyTo(User.SocialNetwork[] array)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			CopyTo(0, array, 0, Count);
		}

		public void CopyTo(User.SocialNetwork[] array, int arrayIndex)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			CopyTo(0, array, arrayIndex, Count);
		}

		public void CopyTo(int index, User.SocialNetwork[] array, int arrayIndex, int count)
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

		public SocialNetworksEnumerator GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new SocialNetworksEnumerator(this);
		}

		public void Clear()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworks_Clear(swigCPtr);
		}

		public void Add(User.SocialNetwork x)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworks_Add(swigCPtr, (int)x);
		}

		private uint size()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.SocialNetworks_size(swigCPtr);
		}

		private uint capacity()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.SocialNetworks_capacity(swigCPtr);
		}

		private void reserve(uint n)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworks_reserve(swigCPtr, n);
		}

		private User.SocialNetwork getitemcopy(int index)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			User.SocialNetwork result = (User.SocialNetwork)RCSSDKPINVOKE.SocialNetworks_getitemcopy(swigCPtr, index);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private User.SocialNetwork getitem(int index)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			User.SocialNetwork result = (User.SocialNetwork)RCSSDKPINVOKE.SocialNetworks_getitem(swigCPtr, index);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private void setitem(int index, User.SocialNetwork val)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworks_setitem(swigCPtr, index, (int)val);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void AddRange(SocialNetworks values)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworks_AddRange(swigCPtr, getCPtr(values));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public SocialNetworks GetRange(int index, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr intPtr = RCSSDKPINVOKE.SocialNetworks_GetRange(swigCPtr, index, count);
			SocialNetworks result = ((!(intPtr == IntPtr.Zero)) ? new SocialNetworks(intPtr, true) : null);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void Insert(int index, User.SocialNetwork x)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworks_Insert(swigCPtr, index, (int)x);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void InsertRange(int index, SocialNetworks values)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworks_InsertRange(swigCPtr, index, getCPtr(values));
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
			RCSSDKPINVOKE.SocialNetworks_RemoveAt(swigCPtr, index);
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
			RCSSDKPINVOKE.SocialNetworks_RemoveRange(swigCPtr, index, count);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public static SocialNetworks Repeat(User.SocialNetwork value, int count)
		{
			IntPtr intPtr = RCSSDKPINVOKE.SocialNetworks_Repeat((int)value, count);
			SocialNetworks result = ((!(intPtr == IntPtr.Zero)) ? new SocialNetworks(intPtr, true) : null);
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
			RCSSDKPINVOKE.SocialNetworks_Reverse_0(swigCPtr);
		}

		public void Reverse(int index, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworks_Reverse_1(swigCPtr, index, count);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetRange(int index, SocialNetworks values)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworks_SetRange(swigCPtr, index, getCPtr(values));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}
	}
}
