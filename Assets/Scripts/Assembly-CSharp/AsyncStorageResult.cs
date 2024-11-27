using System;
using System.Threading;

public class AsyncStorageResult : IAsyncResult
{
	public Type ReturnType { get; private set; }

	public DateTime CreatedAt { get; private set; }

	public string StorageKey { get; private set; }

	public bool NoConnection { get; private set; }

	public bool IsCompleted
	{
		get
		{
			return false;
		}
	}

	public WaitHandle AsyncWaitHandle
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public object AsyncState { get; private set; }

	public bool CompletedSynchronously
	{
		get
		{
			return false;
		}
	}

	public AsyncStorageResult(object state, string key, bool noConnection = false)
		: this(state, key, null, noConnection)
	{
	}

	public AsyncStorageResult(object state, string storageKey, Type returnType, bool noConnection = false)
	{
		NoConnection = noConnection;
		AsyncState = state;
		StorageKey = storageKey;
		CreatedAt = DateTime.UtcNow;
		ReturnType = returnType;
	}
}
