using System.Collections.Generic;

public class BuildScmChangeSet
{
	public List<object> items { get; set; }

	public string kind { get; set; }

	public List<BuildScmRevision> revisions { get; set; }
}
