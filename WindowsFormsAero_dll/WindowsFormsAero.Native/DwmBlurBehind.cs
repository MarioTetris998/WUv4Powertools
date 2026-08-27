using System;

namespace WindowsFormsAero.Native;

internal struct DwmBlurBehind
{
	public DwmBlurBehindFlags dwFlags;

	public bool fEnable;

	public IntPtr hRgnBlur;

	public bool fTransitionOnMaximized;
}
