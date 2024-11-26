using System;
using System.Collections.Generic;
using ABH.GameDatas;

public class DispatchMessage
{
	public enum Status
	{
		Exception = 0,
		Warning = 1,
		Error = 2,
		Info = 3,
		GlobalLoading = 4,
		LocalLoading = 5,
		LocalLoadingNonBlocking = 6,
		InfoAndIcon = 7,
		InfoAndLoot = 8
	}

	public string m_CompareTag;

	public Status m_DispatchStatus;

	public string m_DispatchMessage;

	public string m_DispatchAsset;

	public float m_DispatchProgress = 1f;

	public List<IInventoryItemGameData> m_DispatchItems;

	public string MessageId { get; private set; }

	public DispatchMessage()
	{
		MessageId = Guid.NewGuid().ToString();
	}

	public override bool Equals(object obj)
	{
		return this == obj;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(DispatchMessage a, DispatchMessage b)
	{
		return a.m_CompareTag.Equals(b.m_CompareTag);
	}

	public static bool operator !=(DispatchMessage a, DispatchMessage b)
	{
		return a.m_CompareTag != b.m_CompareTag;
	}
}
