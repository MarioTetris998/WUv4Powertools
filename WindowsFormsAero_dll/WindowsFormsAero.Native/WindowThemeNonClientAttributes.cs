using System;

namespace WindowsFormsAero.Native;

[Flags]
internal enum WindowThemeNonClientAttributes : uint
{
	NullAttribute = 0u,
	NoDrawCaption = 1u,
	NoDrawIcon = 2u
}
