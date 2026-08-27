using System;

namespace WindowsFormsAero.Native;

/// <summary>
/// Contains information that defines a split button.
/// </summary>
/// <remarks>
/// See: https://msdn.microsoft.com/en-us/library/windows/desktop/bb775955(v=vs.85).aspx
/// </remarks>
internal struct ButtonSplitInfo
{
	[Flags]
	public enum MaskType : uint
	{
		Glyph = 1u,
		Image = 2u,
		Style = 4u,
		Size = 8u
	}

	[Flags]
	public enum SplitStyle : uint
	{
		None = 0u,
		NoSplit = 1u,
		Stretch = 2u,
		AlignLeft = 4u,
		Image = 8u
	}

	public MaskType Mask;

	public IntPtr GlyphList;

	public SplitStyle Style;

	public Size Size;
}
