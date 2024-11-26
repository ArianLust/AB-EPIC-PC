public class Connection
{
	public int nodeIn;

	public int nodeOut;

	public int nodeOutIndex;

	public Connection(int o, int i, int oi = 0)
	{
		nodeIn = i;
		nodeOut = o;
		nodeOutIndex = oi;
	}
}
