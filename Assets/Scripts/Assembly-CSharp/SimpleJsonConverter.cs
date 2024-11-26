using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class SimpleJsonConverter
{
	public static Dictionary<string, object> DecodeJsonDict(string json)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		List<object> list = new List<object>();
		int end;
		DecodeJson(json, 0, out end, dictionary, list);
		if (dictionary.Count == 0 && list.Count > 0)
		{
			throw new InvalidOperationException("SimpleJsonConverter.DecodeJsonDict: Expected a dict but received a list!");
		}
		return dictionary;
	}

	public static Dictionary<string, string> DecodeJsonDictString(string json)
	{
		return DecodeJsonDict(json).ToDictionary((KeyValuePair<string, object> o) => o.Key, (KeyValuePair<string, object> o) => o.Value.ToString());
	}

	public static List<object> DecodeJsonList(string json)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		List<object> list = new List<object>();
		int end;
		DecodeJson(json, 0, out end, dictionary, list);
		if (dictionary.Count > 0 && list.Count == 0)
		{
			throw new InvalidOperationException("SimpleJsonConverter.DecodeJsonDict: Expected a list but received a dict!");
		}
		return list;
	}

	public static List<string> DecodeJsonListString(string json)
	{
		return (from o in DecodeJsonList(json)
			select o.ToString()).ToList();
	}

	public static IEnumerable<Dictionary<string, object>> DecodeArrayOfDicts(string json)
	{
		return from o in DecodeJsonList(json)
			select o as Dictionary<string, object>;
	}

	private static void DecodeJson(string json, int start, out int end, Dictionary<string, object> dict, List<object> list)
	{
		json = json.Trim();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		string text = null;
		StringBuilder stringBuilder = new StringBuilder();
		List<object> list2 = null;
		Regex regex = new Regex("\\\\u([0-9a-z]{4})", RegexOptions.IgnoreCase);
		int num = 0;
		for (int i = start; i < json.Length; i++)
		{
			char c = json[i];
			if (c == '\\')
			{
				flag = !flag;
			}
			if (!flag)
			{
				if (c == '"')
				{
					flag3 = !flag3;
					if (!flag3 && list2 != null)
					{
						list2.Add(DecodeString(regex, stringBuilder.ToString()).Trim());
						stringBuilder.Length = 0;
					}
					continue;
				}
				if (!flag3)
				{
					switch (c)
					{
					case '{':
						if (i != start)
						{
							Dictionary<string, object> dictionary = new Dictionary<string, object>();
							List<object> list3 = new List<object>();
							int end2;
							DecodeJson(json, i, out end2, dictionary, list3);
							if (list2 != null)
							{
								list2.Add(dictionary);
							}
							else
							{
								dict.Add(text.Trim(), dictionary);
								text = null;
							}
							i = end2;
						}
						continue;
					case '}':
						end = i;
						if (text != null)
						{
							if (list2 != null)
							{
								dict.Add(text.Trim(), list2.ToArray());
							}
							else
							{
								dict.Add(text.Trim(), DecodeString(regex, stringBuilder.ToString()).Trim());
							}
						}
						return;
					case '[':
						list2 = new List<object>();
						continue;
					case ']':
						if (text == null)
						{
							text = "array" + num;
							num++;
						}
						if (list2 != null && stringBuilder.Length > 0)
						{
							list2.Add(ProcessChild(stringBuilder.ToString().Trim()));
							stringBuilder.Length = 0;
						}
						dict.Add(text.Trim(), list2.ToArray());
						list2 = null;
						text = null;
						continue;
					case ',':
						if (list2 == null && text != null)
						{
							dict.Add(text.Trim(), ProcessChild(DecodeString(regex, stringBuilder.ToString()).Trim()));
							text = null;
							stringBuilder.Length = 0;
						}
						if (list2 != null && stringBuilder.Length > 0)
						{
							list2.Add(ProcessChild(stringBuilder.ToString().Trim()));
							stringBuilder.Length = 0;
						}
						continue;
					case ':':
						text = DecodeString(regex, stringBuilder.ToString());
						stringBuilder.Length = 0;
						continue;
					}
				}
			}
			stringBuilder.Append(c);
			if (flag2)
			{
				flag = false;
			}
			flag2 = (flag ? true : false);
		}
		end = json.Length - 1;
		if (dict.Count != 0 && dict.Values.First() is IEnumerable)
		{
			list.AddRange((dict.Values.First() as IEnumerable).Cast<object>());
			dict.Clear();
		}
	}

	private static object ProcessChild(string child)
	{
		int result;
		if (int.TryParse(child, out result))
		{
			return result;
		}
		return child;
	}

	private static string DecodeString(Regex regex, string str)
	{
		return Regex.Unescape(regex.Replace(str, (Match match) => char.ConvertFromUtf32(int.Parse(match.Groups[1].Value, NumberStyles.HexNumber))));
	}

	public static T DecodeJsonToGeneric<T>(string json, Dictionary<string, Action<T, object>> propertySetters) where T : new()
	{
		T val = new T();
		Dictionary<string, object> props = DecodeJsonDict(json);
		DispatchProps(propertySetters, props, val);
		return val;
	}

	public static void DecodeJsonToInstance(string json, object instance, Dictionary<string, Action<object, object>> propertySetters)
	{
		Dictionary<string, object> props = DecodeJsonDict(json);
		DispatchProps(propertySetters, props, instance);
	}

	private static void DispatchProps<T>(Dictionary<string, Action<T, object>> propertySetters, Dictionary<string, object> props, T res) where T : new()
	{
		foreach (KeyValuePair<string, object> prop in props)
		{
			DispatchProp(propertySetters, res, prop.Key, prop.Value);
		}
	}

	private static void DispatchProp<T>(Dictionary<string, Action<T, object>> propertySetters, T res, string propName, object propValue) where T : new()
	{
		Action<T, object> value;
		if (propertySetters.TryGetValue(propName, out value))
		{
			value(res, propValue);
			return;
		}
		throw new InvalidOperationException(string.Concat("Unable to decode Json of ", typeof(T), ": Property ", propName, " has no setter defined."));
	}

	public static T DecodeJsonDict<T>(object mustBeDictionary, Dictionary<string, Action<T, object>> propertySetters) where T : new()
	{
		if (!(mustBeDictionary is Dictionary<string, object>))
		{
			throw new InvalidOperationException(string.Concat("SimpleJsonConverter.DecodeJsonDict<", typeof(T), "> is no Dictionary<string, object>"));
		}
		T val = new T();
		DispatchProps(propertySetters, mustBeDictionary as Dictionary<string, object>, val);
		return val;
	}

	public static List<T> DecodeJsonListOfBasicTypes<T>(object mustBeEnumerable, Func<object, T> makeBasicType) where T : new()
	{
		return DecodeJsonListInternal(mustBeEnumerable, null, makeBasicType);
	}

	public static List<T> DecodeJsonListOfCompositeObjects<T>(object mustBeEnumerable, Dictionary<string, Action<T, object>> compositeTypePropSetters) where T : new()
	{
		return DecodeJsonListInternal(mustBeEnumerable, compositeTypePropSetters);
	}

	private static List<T> DecodeJsonListInternal<T>(object mustBeEnumerable, Dictionary<string, Action<T, object>> compositeTypePropSetters = null, Func<object, T> makeBasicType = null) where T : new()
	{
		if (!(mustBeEnumerable is IEnumerable) && !(mustBeEnumerable is string))
		{
			throw new InvalidOperationException(string.Concat("SimpleJsonConverter.DecodeJsonListInternal<", typeof(T), "> is no IEnumerable"));
		}
		List<T> list = new List<T>();
		foreach (object item in mustBeEnumerable as IEnumerable)
		{
			if (item is Dictionary<string, object>)
			{
				if (compositeTypePropSetters == null)
				{
					throw new InvalidOperationException("compositeTypePropSetters is null but we need to decode a composite object");
				}
				list.Add(DecodeJsonDict(item, compositeTypePropSetters));
				continue;
			}
			if (item is IEnumerable && !(item is string))
			{
				throw new InvalidOperationException("Lists of Lists are currently not supported");
			}
			if (makeBasicType == null)
			{
				throw new InvalidOperationException("makeBasicType is null but we need to decode a basic type");
			}
			list.Add(makeBasicType(item));
		}
		return list;
	}

	public static string EncodeToJson(object dictOrList)
	{
		if (dictOrList is Dictionary<string, object>)
		{
			return EncodeDictToJson(dictOrList as Dictionary<string, object>);
		}
		if (dictOrList is Dictionary<string, string>)
		{
			return EncodeDictToJson(dictOrList as Dictionary<string, string>);
		}
		if (dictOrList is IEnumerable && !(dictOrList is string))
		{
			return EncodeArrayToJson(dictOrList as IEnumerable);
		}
		if (dictOrList is KeyValuePair<string, object>)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(((KeyValuePair<string, object>)dictOrList).Key, ((KeyValuePair<string, object>)dictOrList).Value);
			return EncodeDictToJson(dictionary);
		}
		throw new InvalidOperationException(string.Concat("Can not encode ", dictOrList, " to Json"));
	}

	public static string EncodeDictToJson<TX>(Dictionary<string, TX> data)
	{
		StringBuilder stringBuilder = new StringBuilder();
		EncodeDictToJson(data, stringBuilder);
		return stringBuilder.ToString();
	}

	public static string EncodeArrayToJson(IEnumerable array)
	{
		StringBuilder stringBuilder = new StringBuilder();
		EncodeArrayToJson(array, stringBuilder);
		return stringBuilder.ToString();
	}

	private static void EncodeDictToJson<TX>(Dictionary<string, TX> data, StringBuilder output)
	{
		output.Append("{");
		bool flag = true;
		foreach (KeyValuePair<string, TX> datum in data)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				output.Append(",");
			}
			output.Append(string.Format("\"{0}\":", datum.Key));
			EncodeValueToJson(datum.Value, output);
		}
		output.Append("}");
	}

	private static void EncodeValueToJson(object val, StringBuilder output)
	{
		if (val is IDictionary)
		{
			EncodeDictToJson(val as Dictionary<string, object>, output);
		}
		else if (val is IEnumerable && !(val is string))
		{
			EncodeArrayToJson(val as IEnumerable, output);
		}
		else if (IsNumber(val))
		{
			output.Append(val);
		}
		else
		{
			output.Append(string.Format("\"{0}\"", val));
		}
	}

	private static void EncodeArrayToJson(IEnumerable data, StringBuilder output)
	{
		output.Append("[");
		bool flag = true;
		foreach (object datum in data)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				output.Append(",");
			}
			EncodeValueToJson(datum, output);
		}
		output.Append("]");
	}

	private static bool IsNumber(object value)
	{
		return value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal;
	}
}
