namespace WindowsFormsAero.Native;

/// <summary>
/// Specifies the width and height of a rectangle.
/// </summary>
/// <remarks>
/// See: https://msdn.microsoft.com/en-us/library/windows/desktop/dd145106(v=vs.85).aspx
/// </remarks>
internal struct Size
{
	public int Width;

	public int Height;

	public Size(int w, int h)
	{
		Width = w;
		Height = h;
	}
}
