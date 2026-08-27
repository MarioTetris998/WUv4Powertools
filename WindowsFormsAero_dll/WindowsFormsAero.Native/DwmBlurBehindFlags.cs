using System;

namespace WindowsFormsAero.Native;

[Flags]
internal enum DwmBlurBehindFlags
{
	Enable = 1,
	BlurRegion = 2,
	TransitionOnMaximized = 4
}
