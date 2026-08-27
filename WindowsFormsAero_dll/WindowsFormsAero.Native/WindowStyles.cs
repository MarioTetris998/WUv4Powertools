using System;

namespace WindowsFormsAero.Native;

[Flags]
internal enum WindowStyles : long
{
	None = 0L,
	Border = 0x800000L,
	Caption = 0xC00000L,
	Child = 0x40000000L,
	DialogFrame = 0x400000L,
	Disabled = 0x8000000L,
	Maximize = 0x1000000L,
	MaximizeBox = 0x10000L,
	Minimize = 0x20000000L,
	MinimizeBox = 0x20000L,
	Overlapped = 0L,
	SysMenu = 0x80000L,
	ThickFrame = 0x40000L,
	Visible = 0x10000000L,
	OverlappedWindow = 0xCF0000L
}
