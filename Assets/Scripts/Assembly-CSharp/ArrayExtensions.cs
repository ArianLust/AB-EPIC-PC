using System;

public static class ArrayExtensions
{
	public static void ForEach<T>(this T[] arr, Action<T> action) where T : class
	{
		Array.ForEach(arr, action);
	}
}
