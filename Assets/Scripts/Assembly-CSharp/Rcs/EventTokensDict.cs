using System;
using System.Collections;
using System.Collections.Generic;

namespace Rcs
{
	public class EventTokensDict : IDisposable, IEnumerable, IDictionary<AppTrack.Event, string>, ICollection<KeyValuePair<AppTrack.Event, string>>, IEnumerable<KeyValuePair<AppTrack.Event, string>>
	{
		public sealed class EventTokensDictEnumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<AppTrack.Event, string>>
		{
			private EventTokensDict collectionRef;

			private IList<AppTrack.Event> keyCollection;

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

			public KeyValuePair<AppTrack.Event, string> Current
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
					return (KeyValuePair<AppTrack.Event, string>)currentObject;
				}
			}

			public EventTokensDictEnumerator(EventTokensDict collection)
			{
				collectionRef = collection;
				keyCollection = new List<AppTrack.Event>(collection.Keys);
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
					AppTrack.Event key = keyCollection[currentIndex];
					currentObject = new KeyValuePair<AppTrack.Event, string>(key, collectionRef[key]);
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

		public string this[AppTrack.Event key]
		{
			get
			{
				return getitem(key);
			}
			set
			{
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

		public ICollection<AppTrack.Event> Keys
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
				}
				ICollection<AppTrack.Event> collection = new List<AppTrack.Event>();
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
				using (EventTokensDictEnumerator eventTokensDictEnumerator = GetEnumerator())
				{
					while (eventTokensDictEnumerator.MoveNext())
					{
						collection.Add(eventTokensDictEnumerator.Current.Value);
					}
					return collection;
				}
			}
		}

		internal EventTokensDict(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public EventTokensDict()
			: this(RCSSDKPINVOKE.new_EventTokensDict_0(), true)
		{
		}

		public EventTokensDict(EventTokensDict other)
			: this(RCSSDKPINVOKE.new_EventTokensDict_1(getCPtr(other)), true)
		{
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		IEnumerator<KeyValuePair<AppTrack.Event, string>> IEnumerable<KeyValuePair<AppTrack.Event, string>>.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new EventTokensDictEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new EventTokensDictEnumerator(this);
		}

		internal static IntPtr getCPtr(EventTokensDict obj)
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

		~EventTokensDict()
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
						RCSSDKPINVOKE.delete_EventTokensDict(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public bool TryGetValue(AppTrack.Event key, out string value)
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

		public void Add(KeyValuePair<AppTrack.Event, string> item)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			Add(item.Key, item.Value);
		}

		public bool Remove(KeyValuePair<AppTrack.Event, string> item)
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

		public bool Contains(KeyValuePair<AppTrack.Event, string> item)
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

		public void CopyTo(KeyValuePair<AppTrack.Event, string>[] array)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			CopyTo(array, 0);
		}

		public void CopyTo(KeyValuePair<AppTrack.Event, string>[] array, int arrayIndex)
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
			IList<AppTrack.Event> list = new List<AppTrack.Event>(Keys);
			for (int i = 0; i < list.Count; i++)
			{
				AppTrack.Event key = list[i];
				array.SetValue(new KeyValuePair<AppTrack.Event, string>(key, this[key]), arrayIndex + i);
			}
		}

		public EventTokensDictEnumerator GetEnumerator()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return new EventTokensDictEnumerator(this);
		}

		private uint size()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.EventTokensDict_size(swigCPtr);
		}

		public bool empty()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.EventTokensDict_empty(swigCPtr);
		}

		public void Clear()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.EventTokensDict_Clear(swigCPtr);
		}

		private string getitem(AppTrack.Event key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			string result = RCSSDKPINVOKE.EventTokensDict_getitem(swigCPtr, (int)key);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
			return result;
		}

		private void setitem(AppTrack.Event key, string x)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.EventTokensDict_setitem(swigCPtr, (int)key, x);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public bool ContainsKey(AppTrack.Event key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.EventTokensDict_ContainsKey(swigCPtr, (int)key);
		}

		public void Add(AppTrack.Event key, string val)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.EventTokensDict_Add(swigCPtr, (int)key, val);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public bool Remove(AppTrack.Event key)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.EventTokensDict_Remove(swigCPtr, (int)key);
		}

		private IntPtr create_iterator_begin()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return RCSSDKPINVOKE.EventTokensDict_create_iterator_begin(swigCPtr);
		}

		private AppTrack.Event get_next_key(IntPtr swigiterator)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			return (AppTrack.Event)RCSSDKPINVOKE.EventTokensDict_get_next_key(swigCPtr, swigiterator);
		}

		private void destroy_iterator(IntPtr swigiterator)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed object.");
			}
			RCSSDKPINVOKE.EventTokensDict_destroy_iterator(swigCPtr, swigiterator);
		}
	}
}
