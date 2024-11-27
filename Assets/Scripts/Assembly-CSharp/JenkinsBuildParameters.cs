using System.Collections.Generic;

public class JenkinsBuildParameters
{
	public List<BuildAction> actions { get; set; }

	public List<object> artifacts { get; set; }

	public bool building { get; set; }

	public object description { get; set; }

	public int duration { get; set; }

	public int estimatedDuration { get; set; }

	public string fullDisplayName { get; set; }

	public string id { get; set; }

	public bool keepLog { get; set; }

	public int number { get; set; }

	public object result { get; set; }

	public long timestamp { get; set; }

	public string url { get; set; }

	public string builtOn { get; set; }

	public BuildScmChangeSet changeSet { get; set; }

	public List<object> culprits { get; set; }

	public T GetValueForParameter<T>(string parametername)
	{
		foreach (BuildAction action in actions)
		{
			if (action.parameters.Count <= 0)
			{
				continue;
			}
			foreach (BuildParameter parameter in action.parameters)
			{
				if (parameter.name.Equals(parametername))
				{
					return (T)parameter.value;
				}
			}
		}
		return default(T);
	}
}
