using System.Collections.Generic;

public class ABHTimingHandler
{
	public int ProcessTimeFromSkynestTimeService(string jsonWebResponse)
	{
		if (jsonWebResponse == null)
		{
			return 0;
		}
		int result = 0;
		foreach (KeyValuePair<string, object> item in SimpleJsonConverter.DecodeJsonDict(jsonWebResponse))
		{
			if (item.Key.ToLower().Equals("time"))
			{
				DebugLog.Log(item.Value.ToString());
				if (int.TryParse(item.Value.ToString(), out result))
				{
					break;
				}
			}
		}
		return result;
	}
}
