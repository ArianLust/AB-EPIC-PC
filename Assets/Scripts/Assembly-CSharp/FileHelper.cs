using System.IO;

internal static class FileHelper
{
	public static byte[] ReadAllBytes(string path)
	{
		return File.ReadAllBytes(path);
	}

	public static string ReadAllText(string path)
	{
		return File.ReadAllText(path);
	}
}
