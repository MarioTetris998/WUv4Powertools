using System;

namespace WindowsFormsAero.Native;

[Flags]
internal enum WindowExStyles : long
{
	AppWindow = 0x40000L,
	ClientEdge = 0x200L,
	ControlParent = 0x10000L,
	Layered = 0x80000L,
	NoActivate = 0x8000000L,
	ToolWindow = 0x80L,
	TopMost = 8L,
	Transparent = 0x20L
}
