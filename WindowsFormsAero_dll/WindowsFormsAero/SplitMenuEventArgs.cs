using System;
using System.Drawing;

namespace WindowsFormsAero;

/// <summary>
/// Provides data for the clicking of split buttons and the opening
/// of context menus.
/// </summary>
/// <remarks>
/// See: http://www.codeproject.com/KB/vista/themedvistacontrols.aspx
/// </remarks>
public class SplitMenuEventArgs : EventArgs
{
	/// <summary>
	/// Represents the bounding box of the clicked button.
	/// </summary>
	/// <remarks>
	/// A menu should be opened, with top-left coordinates in the left-bottom
	/// point of the rectangle and with width equal (or greater) than the width
	/// of the rectangle.
	/// </remarks>
	public Rectangle DrawArea { get; set; }

	/// <summary>
	/// Set to true if you want to prevent the menu from opening.
	/// </summary>
	public bool PreventOpening { get; set; }

	public SplitMenuEventArgs()
	{
		PreventOpening = false;
	}

	public SplitMenuEventArgs(Rectangle drawArea)
	{
		DrawArea = drawArea;
	}
}
