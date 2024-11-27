using System;
using System.IO;
using Chimera.Library.Components.Interfaces;

public class BinarySerializerProtoBufPrecompiledImpl : ISerializer, IHasLogger
{
	private readonly ABHModelSerializer m_modelSerializer = new ABHModelSerializer();

	public Action<string> LogError { get; set; }

	public Action<string> Log { get; set; }

	string ISerializer.Serialize<T>(T obj)
	{
		throw new NotImplementedException();
	}

	public byte[] SerializeToBytes<T>(T obj) where T : class
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			m_modelSerializer.Serialize(memoryStream, obj);
			return memoryStream.ToArray();
		}
	}

	public T Deserialize<T>(byte[] bytes) where T : class
	{
		if (Log != null)
		{
			Log("Deserialize to type " + typeof(T).Name);
		}
		using (MemoryStream source = new MemoryStream(bytes))
		{
			object obj = m_modelSerializer.Deserialize(source, null, typeof(T));
			return obj as T;
		}
	}

	public string GetSerializerUniqueName()
	{
		return "protobuf";
	}

	public object Deserialize(byte[] bytes, Type returnType)
	{
		using (MemoryStream source = new MemoryStream(bytes))
		{
			if (Log != null)
			{
				Log("Deserialize to type " + returnType.Name);
			}
			return m_modelSerializer.Deserialize(source, null, returnType);
		}
	}

	public T Deserialize<T>(Stream stream) where T : class
	{
		object obj = m_modelSerializer.Deserialize(stream, null, typeof(T));
		return obj as T;
	}

	public object Deserialize(Stream stream, Type returnType)
	{
		return m_modelSerializer.Deserialize(stream, null, returnType);
	}

	public object Deserialize(string str, Type returnType)
	{
		throw new NotImplementedException();
	}

	public T Deserialize<T>(string str) where T : class
	{
		throw new NotImplementedException();
	}

	public string GetParserVersionFromString(string json, string parserVersionPropertyName)
	{
		throw new NotImplementedException();
	}
}
