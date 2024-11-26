using UnityEngine;

public class Preview
{
	public float x;

	public float y;

	public float width;

	public float height;

	public float scale;

	public float offset;

	public Rect GetRect()
	{
		return new Rect(x, y, width, height);
	}
}
