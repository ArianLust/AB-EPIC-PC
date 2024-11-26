using System;

internal class ChWebResponseNullImpl : IChWebResponse
{
	public byte[] Payload
	{
		get
		{
			return null;
		}
	}

	public string PayloadText
	{
		get
		{
			return string.Empty;
		}
	}

	public int StatusCode
	{
		get
		{
			return 502;
		}
	}

	public override string ToString()
	{
		return string.Format("{0}, {1} bytes starting with \"{2}\"", StatusCode, (Payload == null) ? "n/a" : Payload.Length.ToString(), (PayloadText == null) ? "n/a" : PayloadText.Substring(0, Math.Min(15, PayloadText.Length)));
	}
}
