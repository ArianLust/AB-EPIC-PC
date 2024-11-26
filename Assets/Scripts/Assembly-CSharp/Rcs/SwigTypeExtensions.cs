using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rcs
{
	public static class SwigTypeExtensions
	{
		private class CopyProxy<T>
		{
			private Type paramType = typeof(T);

			private bool noCopy;

			public CopyProxy()
			{
				if (paramType.IsArray)
				{
					throw new ArgumentException("Array copping dose not supported");
				}
				noCopy = paramType.IsValueType || typeof(T) == typeof(string);
				if (!noCopy)
				{
					ConstructorInfo constructor = paramType.GetConstructor(new Type[1] { paramType });
					if (constructor == null)
					{
						throw new ArgumentException("Type " + paramType.ToString() + " has no copy constructor");
					}
				}
			}

			public T copy(T src)
			{
				if (noCopy)
				{
					return src;
				}
				return (T)Activator.CreateInstance(typeof(T), src);
			}
		}

		private delegate T CopyDelegate<T>(T arg);

		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDictionary<TKey, TValue> srcDict)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			CopyDelegate<TKey> copyDelegate = new CopyProxy<TKey>().copy;
			CopyDelegate<TValue> copyDelegate2 = new CopyProxy<TValue>().copy;
			foreach (KeyValuePair<TKey, TValue> item in srcDict)
			{
				TKey key = copyDelegate(item.Key);
				TValue value = copyDelegate2(item.Value);
				dictionary.Add(key, value);
			}
			return dictionary;
		}

		public static List<TValue> ToList<TValue>(this IList<TValue> srcList)
		{
			List<TValue> list = new List<TValue>();
			CopyDelegate<TValue> copyDelegate = new CopyProxy<TValue>().copy;
			foreach (TValue src in srcList)
			{
				TValue item = copyDelegate(src);
				list.Add(item);
			}
			return list;
		}

		public static List<TValue> ToList<TValue>(this IEnumerable<TValue> srcList)
		{
			List<TValue> list = new List<TValue>();
			CopyDelegate<TValue> copyDelegate = new CopyProxy<TValue>().copy;
			foreach (TValue src in srcList)
			{
				TValue item = copyDelegate(src);
				list.Add(item);
			}
			return list;
		}

		public static StringDict ToSwigDict(this Dictionary<string, string> srcDict)
		{
			StringDict stringDict = new StringDict();
			foreach (KeyValuePair<string, string> item in srcDict)
			{
				stringDict.Add(item.Key, item.Value);
			}
			return stringDict;
		}

		public static EventTokensDict ToSwigDict(this Dictionary<AppTrack.Event, string> srcDict)
		{
			EventTokensDict eventTokensDict = new EventTokensDict();
			foreach (KeyValuePair<AppTrack.Event, string> item in srcDict)
			{
				eventTokensDict.Add(item.Key, item.Value);
			}
			return eventTokensDict;
		}

		public static VariantDict ToSwigDict(this Dictionary<string, Variant> srcDict)
		{
			VariantDict variantDict = new VariantDict();
			foreach (KeyValuePair<string, Variant> item in srcDict)
			{
				variantDict.Add(item.Key, item.Value);
			}
			return variantDict;
		}
	}
}
