using System.Drawing;

namespace WindowsFormsAero.Native;

internal struct Rect
{
	public int Left;

	public int Top;

	public int Right;

	public int Bottom;

	public int Width
	{
		get
		{
			return Right - Left;
		}
		set
		{
			Right = Left + value;
		}
	}

	public int Height
	{
		get
		{
			return Bottom - Top;
		}
		set
		{
			Bottom = Top + value;
		}
	}

	public Rect(int left, int top, int right, int bottom)
	{
		Left = left;
		Top = top;
		Right = right;
		Bottom = bottom;
	}

	public Rect(Rectangle rect)
	{
		Left = rect.X;
		Top = rect.Y;
		Right = rect.Right;
		Bottom = rect.Bottom;
	}

	public Rectangle ToRectangle()
	{
		return new Rectangle(Left, Top, Right - Left, Bottom - Top);
	}
}
