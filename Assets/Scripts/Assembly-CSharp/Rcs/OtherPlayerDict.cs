using System;
using System.Collections;
using System.Collections.Generic;

namespace Rcs
{
	public class OtherPlayerDict : IDisposable, IEnumerable, IDictionary<string, OtherPlayer>, ICollection<KeyValuePair<string, OtherPlayer>>, IEnumerable<KeyValuePair<string, OtherPlayer>>
	{
		public sealed class OtherPlayerDictEnumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<string, OtherPlayer>>
		{
			private OtherPlayerDict collectionRef;

			private IList<string> keyCollection;

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

			public KeyValuePair<string, OtherPlayer> Current
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
					return (KeyValuePair<string, OtherPlayer>)currentObject;
				}
			}

			public OtherPlayerDictEnumerator(OtherPlayerDict collection)
			{
				collectionRef = collection;
				keyCollection = new List<string>(collection.Keys);
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
					string key = keyCollection[currentIndex];
					currentObject = new KeyValuePair<string, OtherPlayer>(key, collectionRef[key]);
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

		public OtherPlayer this[string key]
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

		public ICollection<string> Keys
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				ICollection<string> collection = new List<string>();
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

		public ICollection<OtherPlayer> Values
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				ICollection<OtherPlayer> collection = new List<OtherPlayer>();
				using (OtherPlayerDictEnumerator otherPlayerDictEnumerator = GetEnumerator())
				{
					while (otherPlayerDictEnumerator.MoveNext())
					{
						collection.Add(otherPlayerDictEnumerator.Current.Value);
					}
					return collection;
				}
			}
		}

		internal OtherPlayerDict(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public OtherPlayerDict()
			: this(RCSSDKPINVOKE.new_OtherPlayerDict_0(), true)
		{
		}

		public OtherPlayerDict(OtherPlayerDict other)
			: this(RCSSDKPINVOKE.new_OtherPlayerDict_1(getCPtr(other)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		IEnumerator<KeyValuePair<string, OtherPlayer>> IEnumerable<KeyValuePair<string, OtherPlayer>>.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new OtherPlayerDictEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new OtherPlayerDictEnumerator(this);
		}

		internal static IntPtr getCPtr(OtherPlayerDict obj)
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

		~OtherPlayerDict()
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
						RCSSDKPINVOKE.delete_OtherPlayerDict(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public bool TryGetValue(string key, out OtherPlayer value)
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

		public void Add(KeyValuePair<string, OtherPlayer> item)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			Add(item.Key, item.Value);
		}

		public bool Remove(KeyValuePair<string, OtherPlayer> item)
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

		public bool Contains(KeyValuePair<string, OtherPlayer> item)
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

		public void CopyTo(KeyValuePair<string, OtherPlayer>[] array)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			CopyTo(array, 0);
		}

		public void CopyTo(KeyValuePair<string, OtherPlayer>[] array, int arrayIndex)
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
			IList<string> list = new List<string>(Keys);
			for (int i = 0; i < list.Count; i++)
			{
				string key = list[i];
				array.SetValue(new KeyValuePair<string, OtherPlayer>(key, this[key]), arrayIndex + i);
			}
		}

		public OtherPlayerDictEnumerator GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new OtherPlayerDictEnumerator(this);
		}

		private uint size()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.OtherPlayerDict_size(swigCPtr);
		}

		public bool empty()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.OtherPlayerDict_empty(swigCPtr);
		}

		public void Clear()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.OtherPlayerDict_Clear(swigCPtr);
		}

		private OtherPlayer getitem(string key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			OtherPlayer result = new OtherPlayer(RCSSDKPINVOKE.OtherPlayerDict_getitem(swigCPtr, key), false);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private void setitem(string key, OtherPlayer x)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.OtherPlayerDict_setitem(swigCPtr, key, OtherPlayer.getCPtr(x));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public bool ContainsKey(string key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			bool result = RCSSDKPINVOKE.OtherPlayerDict_ContainsKey(swigCPtr, key);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		public void Add(string key, OtherPlayer val)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.OtherPlayerDict_Add(swigCPtr, key, OtherPlayer.getCPtr(val));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public bool Remove(string key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			bool result = RCSSDKPINVOKE.OtherPlayerDict_Remove(swigCPtr, key);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private IntPtr create_iterator_begin()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.OtherPlayerDict_create_iterator_begin(swigCPtr);
		}

		private string get_next_key(IntPtr swigiterator)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.OtherPlayerDict_get_next_key(swigCPtr, swigiterator);
		}

		private void destroy_iterator(IntPtr swigiterator)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.OtherPlayerDict_destroy_iterator(swigCPtr, swigiterator);
		}
	}
}
