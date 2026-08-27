using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsAero;

internal static class GeometryExtensions
{
	/// <summary>
	/// Returns true if any side of the padding is a positive, non-zero value.
	/// </summary>
	public static bool IsPositive(this Padding p)
	{
		if (p.All <= 0 && p.Top <= 0 && p.Bottom <= 0 && p.Left <= 0)
		{
			return p.Right > 0;
		}
		return true;
	}

	/// <summary>
	/// Returns true if all sides are negative.
	/// </summary>
	public static bool AllNegative(this Padding p)
	{
		if (p.Top < 0 && p.Bottom < 0 && p.Left < 0)
		{
			return p.Right < 0;
		}
		return false;
	}

	/// <summary>
	/// Returns whether a point in client coordinates is outside the padded region.
	/// </summary>
	/// <param name="point">Point in client coordinates.</param>
	/// <param name="size">Full size of the region on which padding is applied.</param>
	public static bool IsOutside(this Padding p, Point point, Size size)
	{
		if (point.X >= p.Left && point.X <= size.Width - p.Right && point.Y >= p.Top)
		{
			return point.Y > size.Height - p.Bottom;
		}
		return true;
	}
}
