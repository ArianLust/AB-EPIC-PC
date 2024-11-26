using System;
using System.Collections.Generic;

public class BuildScmRevision
{
	public static readonly Dictionary<string, Action<BuildScmRevision, object>> JsonDeserializeInfo = new Dictionary<string, Action<BuildScmRevision, object>>
	{
		{
			"module",
			delegate(BuildScmRevision t, object o)
			{
				t.module = o.ToString();
			}
		},
		{
			"revision",
			delegate(BuildScmRevision t, object o)
			{
				t.revision = int.Parse(o.ToString());
			}
		}
	};

	public string module { get; set; }

	public int revision { get; set; }
}
