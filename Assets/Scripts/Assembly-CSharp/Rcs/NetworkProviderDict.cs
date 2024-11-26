using System;
using System.Collections;
using System.Collections.Generic;

namespace Rcs
{
	public class NetworkProviderDict : IDisposable, IEnumerable, IDictionary<NetworkProvider, string>, ICollection<KeyValuePair<NetworkProvider, string>>, IEnumerable<KeyValuePair<NetworkProvider, string>>
	{
		public sealed class NetworkProviderDictEnumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<NetworkProvider, string>>
		{
			private NetworkProviderDict collectionRef;

			private IList<NetworkProvider> keyCollection;

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

			public KeyValuePair<NetworkProvider, string> Current
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
					return (KeyValuePair<NetworkProvider, string>)currentObject;
				}
			}

			public NetworkProviderDictEnumerator(NetworkProviderDict collection)
			{
				collectionRef = collection;
				keyCollection = new List<NetworkProvider>(collection.Keys);
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
					NetworkProvider key = keyCollection[currentIndex];
					currentObject = new KeyValuePair<NetworkProvider, string>(key, collectionRef[key]);
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

		public string this[NetworkProvider key]
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				return getitem(key);
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				setitem(key, value);
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

		public ICollection<NetworkProvider> Keys
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				ICollection<NetworkProvider> collection = new List<NetworkProvider>();
				int count = Count;
				if (count > 0)
				{
					IntPtr swigiterator = create_iterator_begin();
					for (int i = 0; i < count; i++)
					{
						collection.Add(get_next_key(swigiterator));
					}
					destroy_iterator(swigiterator);
				}
				return collection;
			}
		}

		public ICollection<string> Values
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				ICollection<string> collection = new List<string>();
				using (NetworkProviderDictEnumerator networkProviderDictEnumerator = GetEnumerator())
				{
					while (networkProviderDictEnumerator.MoveNext())
					{
						collection.Add(networkProviderDictEnumerator.Current.Value);
					}
					return collection;
				}
			}
		}

		internal NetworkProviderDict(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public NetworkProviderDict()
			: this(RCSSDKPINVOKE.new_NetworkProviderDict_0(), true)
		{
		}

		public NetworkProviderDict(NetworkProviderDict other)
			: this(RCSSDKPINVOKE.new_NetworkProviderDict_1(getCPtr(other)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		IEnumerator<KeyValuePair<NetworkProvider, string>> IEnumerable<KeyValuePair<NetworkProvider, string>>.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new NetworkProviderDictEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new NetworkProviderDictEnumerator(this);
		}

		internal static IntPtr getCPtr(NetworkProviderDict obj)
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

		~NetworkProviderDict()
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
						RCSSDKPINVOKE.delete_NetworkProviderDict(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public bool TryGetValue(NetworkProvider key, out string value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			if (ContainsKey(key))
			{
				value = this[key];
				return true;
			}
			value = null;
			return false;
		}

		public void Add(KeyValuePair<NetworkProvider, string> item)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			Add(item.Key, item.Value);
		}

		public bool Remove(KeyValuePair<NetworkProvider, string> item)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			if (Contains(item))
			{
				return Remove(item.Key);
			}
			return false;
		}

		public bool Contains(KeyValuePair<NetworkProvider, string> item)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			if (this[item.Key] == item.Value)
			{
				return true;
			}
			return false;
		}

		public void CopyTo(KeyValuePair<NetworkProvider, string>[] array)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			CopyTo(array, 0);
		}

		public void CopyTo(KeyValuePair<NetworkProvider, string>[] array, int arrayIndex)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", "Value is less than zero");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("Multi dimensional array.", "array");
			}
			if (arrayIndex + Count > array.Length)
			{
				throw new ArgumentException("Number of elements to copy is too large.");
			}
			IList<NetworkProvider> list = new List<NetworkProvider>(Keys);
			for (int i = 0; i < list.Count; i++)
			{
				NetworkProvider key = list[i];
				array.SetValue(new KeyValuePair<NetworkProvider, string>(key, this[key]), arrayIndex + i);
			}
		}

		public NetworkProviderDictEnumerator GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new NetworkProviderDictEnumerator(this);
		}

		private uint size()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.NetworkProviderDict_size(swigCPtr);
		}

		public bool empty()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.NetworkProviderDict_empty(swigCPtr);
		}

		public void Clear()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.NetworkProviderDict_Clear(swigCPtr);
		}

		private string getitem(NetworkProvider key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			string result = RCSSDKPINVOKE.NetworkProviderDict_getitem(swigCPtr, (int)key);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private void setitem(NetworkProvider key, string x)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.NetworkProviderDict_setitem(swigCPtr, (int)key, x);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public bool ContainsKey(NetworkProvider key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.NetworkProviderDict_ContainsKey(swigCPtr, (int)key);
		}

		public void Add(NetworkProvider key, string val)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.NetworkProviderDict_Add(swigCPtr, (int)key, val);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public bool Remove(NetworkProvider key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.NetworkProviderDict_Remove(swigCPtr, (int)key);
		}

		private IntPtr create_iterator_begin()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.NetworkProviderDict_create_iterator_begin(swigCPtr);
		}

		private NetworkProvider get_next_key(IntPtr swigiterator)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return (NetworkProvider)RCSSDKPINVOKE.NetworkProviderDict_get_next_key(swigCPtr, swigiterator);
		}

		private void destroy_iterator(IntPtr swigiterator)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.NetworkProviderDict_destroy_iterator(swigCPtr, swigiterator);
		}
	}
}
