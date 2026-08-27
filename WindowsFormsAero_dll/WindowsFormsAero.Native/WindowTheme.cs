using System;
using System.Runtime.InteropServices;

namespace WindowsFormsAero.Native;

internal static class WindowTheme
{
	public enum WindowThemeAttributeType
	{
		WTA_NONCLIENT = 1
	}

	[DllImport("uxtheme.dll")]
	public static extern int SetWindowThemeAttribute(IntPtr hWnd, WindowThemeAttributeType wtype, ref WTA_OPTIONS attributes, uint size);

	public static int SetWindowThemeNonClientAttributes(IntPtr hwnd, WindowThemeNonClientAttributes mask, WindowThemeNonClientAttributes attributes)
	{
		WTA_OPTIONS opt = new WTA_OPTIONS
		{
			Flags = attributes,
			Mask = mask
		};
		return SetWindowThemeAttribute(hwnd, WindowThemeAttributeType.WTA_NONCLIENT, ref opt, (uint)Marshal.SizeOf(typeof(WTA_OPTIONS)));
	}
}
