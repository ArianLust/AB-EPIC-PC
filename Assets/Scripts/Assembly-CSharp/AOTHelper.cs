using System;
using System.Collections.Generic;

public static class AOTHelper
{
	public static void SaveAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
	{
		if (!dictionary.ContainsKey(key))
		{
			dictionary.Add(key, value);
		}
		else
		{
			dictionary[key] = value;
		}
	}

	public static KeyValuePair<TKey, TValue> FirstOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, bool> func)
	{
		foreach (TKey key in dictionary.Keys)
		{
			TValue value = dictionary[key];
			KeyValuePair<TKey, TValue> keyValuePair = new KeyValuePair<TKey, TValue>(key, value);
			if (func(keyValuePair))
			{
				return keyValuePair;
			}
		}
		return default(KeyValuePair<TKey, TValue>);
	}

	public static KeyValuePair<TKey, TValue> FirstOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
	{
		using (Dictionary<TKey, TValue>.KeyCollection.Enumerator enumerator = dictionary.Keys.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				TKey current = enumerator.Current;
				return new KeyValuePair<TKey, TValue>(current, dictionary[current]);
			}
		}
		return default(KeyValuePair<TKey, TValue>);
	}

	public static int Max<T>(this List<T> list, Func<T, int> func)
	{
		int num = int.MinValue;
		foreach (T item in list)
		{
			num = Math.Max(func(item), num);
		}
		return num;
	}

	public static float Max<T>(this List<T> list, Func<T, float> func)
	{
		float num = -2.1474836E+09f;
		foreach (T item in list)
		{
			num = Math.Max(func(item), num);
		}
		return num;
	}

	public static double Average<T>(this List<T> list, Func<T, int> func)
	{
		double num = 0.0;
		foreach (T item in list)
		{
			num += (double)func(item);
		}
		return num / (double)list.Count;
	}

	public static float Average(this List<float> source)
	{
		float num = 0f;
		foreach (float item in source)
		{
			float num2 = item;
			num += num2;
		}
		return num / (float)source.Count;
	}

	public static int Min<T>(this List<T> list, Func<T, int> func)
	{
		int num = int.MaxValue;
		foreach (T item in list)
		{
			num = Math.Min(func(item), num);
		}
		return num;
	}

	public static int Sum<T>(this List<T> list, Func<T, int> func)
	{
		int num = 0;
		foreach (T item in list)
		{
			num += func(item);
		}
		return num;
	}

	public static int Count<T>(this ICollection<T> list, Func<T, bool> func)
	{
		int num = 0;
		foreach (T item in list)
		{
			if (func(item))
			{
				num++;
			}
		}
		return num;
	}
}
