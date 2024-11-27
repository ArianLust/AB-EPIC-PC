using System;
using System.Collections;
using System.Collections.Generic;

namespace Rcs
{
	public class SocialNetworkProfiles : IDisposable, IEnumerable, IEnumerable<User.SocialNetworkProfile>
	{
		public sealed class SocialNetworkProfilesEnumerator : IDisposable, IEnumerator, IEnumerator<User.SocialNetworkProfile>
		{
			private SocialNetworkProfiles collectionRef;

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

			public User.SocialNetworkProfile Current
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
					return (User.SocialNetworkProfile)currentObject;
				}
			}

			public SocialNetworkProfilesEnumerator(SocialNetworkProfiles collection)
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

		public User.SocialNetworkProfile this[int index]
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

		internal SocialNetworkProfiles(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public SocialNetworkProfiles(ICollection c)
			: this()
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			foreach (User.SocialNetworkProfile item in c)
			{
				Add(item);
			}
		}

		public SocialNetworkProfiles()
			: this(RCSSDKPINVOKE.new_SocialNetworkProfiles_0(), true)
		{
		}

		public SocialNetworkProfiles(SocialNetworkProfiles other)
			: this(RCSSDKPINVOKE.new_SocialNetworkProfiles_1(getCPtr(other)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public SocialNetworkProfiles(int capacity)
			: this(RCSSDKPINVOKE.new_SocialNetworkProfiles_2(capacity), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		IEnumerator<User.SocialNetworkProfile> IEnumerable<User.SocialNetworkProfile>.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new SocialNetworkProfilesEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new SocialNetworkProfilesEnumerator(this);
		}

		internal static IntPtr getCPtr(SocialNetworkProfiles obj)
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

		~SocialNetworkProfiles()
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
						RCSSDKPINVOKE.delete_SocialNetworkProfiles(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public void CopyTo(User.SocialNetworkProfile[] array)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			CopyTo(0, array, 0, Count);
		}

		public void CopyTo(User.SocialNetworkProfile[] array, int arrayIndex)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			CopyTo(0, array, arrayIndex, Count);
		}

		public void CopyTo(int index, User.SocialNetworkProfile[] array, int arrayIndex, int count)
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

		public SocialNetworkProfilesEnumerator GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new SocialNetworkProfilesEnumerator(this);
		}

		public void Clear()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworkProfiles_Clear(swigCPtr);
		}

		public void Add(User.SocialNetworkProfile x)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworkProfiles_Add(swigCPtr, User.SocialNetworkProfile.getCPtr(x));
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
			return RCSSDKPINVOKE.SocialNetworkProfiles_size(swigCPtr);
		}

		private uint capacity()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.SocialNetworkProfiles_capacity(swigCPtr);
		}

		private void reserve(uint n)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworkProfiles_reserve(swigCPtr, n);
		}

		private User.SocialNetworkProfile getitemcopy(int index)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			User.SocialNetworkProfile result = new User.SocialNetworkProfile(RCSSDKPINVOKE.SocialNetworkProfiles_getitemcopy(swigCPtr, index), true);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private User.SocialNetworkProfile getitem(int index)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			User.SocialNetworkProfile result = new User.SocialNetworkProfile(RCSSDKPINVOKE.SocialNetworkProfiles_getitem(swigCPtr, index), false);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private void setitem(int index, User.SocialNetworkProfile val)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworkProfiles_setitem(swigCPtr, index, User.SocialNetworkProfile.getCPtr(val));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void AddRange(SocialNetworkProfiles values)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworkProfiles_AddRange(swigCPtr, getCPtr(values));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public SocialNetworkProfiles GetRange(int index, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			IntPtr intPtr = RCSSDKPINVOKE.SocialNetworkProfiles_GetRange(swigCPtr, index, count);
			SocialNetworkProfiles result = ((!(intPtr == IntPtr.Zero)) ? new SocialNetworkProfiles(intPtr, true) : null);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void Insert(int index, User.SocialNetworkProfile x)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworkProfiles_Insert(swigCPtr, index, User.SocialNetworkProfile.getCPtr(x));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void InsertRange(int index, SocialNetworkProfiles values)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworkProfiles_InsertRange(swigCPtr, index, getCPtr(values));
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
			RCSSDKPINVOKE.SocialNetworkProfiles_RemoveAt(swigCPtr, index);
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
			RCSSDKPINVOKE.SocialNetworkProfiles_RemoveRange(swigCPtr, index, count);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public static SocialNetworkProfiles Repeat(User.SocialNetworkProfile value, int count)
		{
			IntPtr intPtr = RCSSDKPINVOKE.SocialNetworkProfiles_Repeat(User.SocialNetworkProfile.getCPtr(value), count);
			SocialNetworkProfiles result = ((!(intPtr == IntPtr.Zero)) ? new SocialNetworkProfiles(intPtr, true) : null);
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
			RCSSDKPINVOKE.SocialNetworkProfiles_Reverse_0(swigCPtr);
		}

		public void Reverse(int index, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworkProfiles_Reverse_1(swigCPtr, index, count);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public void SetRange(int index, SocialNetworkProfiles values)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.SocialNetworkProfiles_SetRange(swigCPtr, index, getCPtr(values));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}
	}
}
