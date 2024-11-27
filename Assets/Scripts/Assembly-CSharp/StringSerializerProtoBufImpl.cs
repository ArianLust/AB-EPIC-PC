using System;
using System.IO;
using Chimera.Library.Components.Interfaces;
using ProtoBuf;

public class StringSerializerProtoBufImpl : ISerializer, IHasLogger
{
	public Action<string> LogError { get; set; }

	public Action<string> Log { get; set; }

	public string Serialize<T>(T obj) where T : class
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			DebugLog.Log("Serializing type " + obj.GetType());
			Serializer.NonGeneric.Serialize(memoryStream, obj);
			return Convert.ToBase64String(memoryStream.ToArray());
		}
	}

	public T Deserialize<T>(string stringBase64) where T : class
	{
		byte[] buffer = Convert.FromBase64String(stringBase64);
		using (MemoryStream source = new MemoryStream(buffer))
		{
			object obj = Serializer.NonGeneric.Deserialize(typeof(T), source);
			return obj as T;
		}
	}

	public string GetSerializerUniqueName()
	{
		return "protobuf";
	}

	public string GetParserVersionFromString(string json, string parserVersionPropertyName)
	{
		return string.Empty;
	}

	public object Deserialize(string stringBase64, Type returnType)
	{
		byte[] buffer = Convert.FromBase64String(stringBase64);
		using (MemoryStream source = new MemoryStream(buffer))
		{
			return Serializer.NonGeneric.Deserialize(returnType, source);
		}
	}

	public T Deserialize<T>(Stream stream) where T : class
	{
		object obj = Serializer.NonGeneric.Deserialize(typeof(T), stream);
		return obj as T;
	}

	public object Deserialize(Stream stream, Type returnType)
	{
		return Serializer.NonGeneric.Deserialize(returnType, stream);
	}

	public object Deserialize(byte[] bytes, Type returnType)
	{
		throw new NotImplementedException();
	}

	public T Deserialize<T>(byte[] bytes) where T : class
	{
		throw new NotImplementedException();
	}

	public byte[] SerializeToBytes<T>(T obj) where T : class
	{
		throw new NotImplementedException();
	}
}
