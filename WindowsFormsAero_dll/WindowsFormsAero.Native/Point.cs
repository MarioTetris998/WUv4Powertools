using System.Drawing;

namespace WindowsFormsAero.Native;

internal struct Point
{
	public int X;

	public int Y;

	public Point(int x, int y)
	{
		X = x;
		Y = y;
	}

	public Point(System.Drawing.Point p)
	{
		X = p.X;
		Y = p.Y;
	}

	public Point(PointF p)
	{
		X = (int)p.X;
		Y = (int)p.Y;
	}
}
