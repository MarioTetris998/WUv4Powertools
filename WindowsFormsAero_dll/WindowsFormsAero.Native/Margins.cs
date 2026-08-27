using System.Windows.Forms;

namespace WindowsFormsAero.Native;

internal struct Margins
{
	public int Left;

	public int Right;

	public int Top;

	public int Bottom;

	/// <summary>
	/// Gets a static readonly 0-pixel margin.
	/// </summary>
	public static readonly Margins Zero = new Margins(0);

	public Margins(int left, int right, int top, int bottom)
	{
		Left = left;
		Right = right;
		Top = top;
		Bottom = bottom;
	}

	public Margins(int all)
	{
		Left = all;
		Right = all;
		Top = all;
		Bottom = all;
	}

	/// <summary>
	/// Converts margins to a <see cref="!:Padding" /> instance.
	/// </summary>
	public Padding ToPadding()
	{
		return new Padding(Left, Top, Right, Bottom);
	}

	/// <summary>
	/// Creates margins from a <see cref="!:Padding" /> instance.
	/// </summary>
	public static Margins FromPadding(Padding padding)
	{
		return new Margins(padding.Left, padding.Right, padding.Top, padding.Bottom);
	}

	public override string ToString()
	{
		return $"{{{Left},{Right},{Top},{Bottom}}}";
	}
}
