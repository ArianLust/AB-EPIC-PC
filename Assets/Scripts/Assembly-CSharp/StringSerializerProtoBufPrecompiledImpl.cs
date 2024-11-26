using System;
using System.IO;
using Chimera.Library.Components.Interfaces;

public class StringSerializerProtoBufPrecompiledImpl : ISerializer, IHasLogger
{
	private ABHModelSerializer m_abhModelSerializer = new ABHModelSerializer();

	public Action<string> LogError { get; set; }

	public Action<string> Log { get; set; }

	public string Serialize<T>(T obj) where T : class
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			DebugLog.Log("Serializing type " + obj.GetType());
			m_abhModelSerializer.Serialize(memoryStream, obj);
			return Convert.ToBase64String(memoryStream.ToArray());
		}
	}

	public T Deserialize<T>(string stringBase64) where T : class
	{
		byte[] buffer = Convert.FromBase64String(stringBase64);
		using (MemoryStream source = new MemoryStream(buffer))
		{
			object obj = m_abhModelSerializer.Deserialize(source, null, typeof(T));
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
			return m_abhModelSerializer.Deserialize(source, null, returnType);
		}
	}

	public T Deserialize<T>(Stream stream) where T : class
	{
		object obj = m_abhModelSerializer.Deserialize(stream, null, typeof(T));
		return obj as T;
	}

	public object Deserialize(Stream stream, Type returnType)
	{
		return m_abhModelSerializer.Deserialize(stream, null, returnType);
	}

	public object Deserialize(byte[] bytes, Type returnType)
	{
		using (MemoryStream source = new MemoryStream(bytes))
		{
			return m_abhModelSerializer.Deserialize(source, null, returnType);
		}
	}

	public T Deserialize<T>(byte[] bytes) where T : class
	{
		return Deserialize(bytes, typeof(T)) as T;
	}

	public byte[] SerializeToBytes<T>(T obj) where T : class
	{
		throw new NotImplementedException();
	}
}
