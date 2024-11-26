using System.Collections.Generic;

public class LocaConfig
{
	public readonly Dictionary<string, string> _locaReplacementDictionary = new Dictionary<string, string>();

	public Dictionary<string, string> LocaDictionary { get; set; }

	public Dictionary<string, string> LocaReplacementDictionary
	{
		get
		{
			return _locaReplacementDictionary;
		}
	}
}
