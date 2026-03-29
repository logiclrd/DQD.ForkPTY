namespace DQD.ForkPTY;

public struct Size
{
	public int Width;
	public int Height;

	public Size() {}

	public Size(int width, int height)
	{
		Width = width;
		Height = height;
	}

	public static implicit operator Size((int, int) tuple)
		=> new Size(tuple.Item1, tuple.Item2);
}
