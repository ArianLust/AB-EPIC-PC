using System;
using System.Collections.Generic;

public class BuildCause
{
	public static readonly Dictionary<string, Action<BuildCause, object>> JsonDeserializeInfo = new Dictionary<string, Action<BuildCause, object>>
	{
		{
			"shortDescription",
			delegate(BuildCause t, object o)
			{
				t.shortDescription = o.ToString();
			}
		},
		{
			"userId",
			delegate(BuildCause t, object o)
			{
				t.userId = o.ToString();
			}
		},
		{
			"userName",
			delegate(BuildCause t, object o)
			{
				t.userName = o.ToString();
			}
		}
	};

	public string shortDescription { get; set; }

	public string userId { get; set; }

	public string userName { get; set; }
}
