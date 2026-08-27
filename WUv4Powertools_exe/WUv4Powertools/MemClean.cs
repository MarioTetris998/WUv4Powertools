using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WUv4Powertools;

public class MemClean
{
	[DllImport("KERNEL32.DLL", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetProcessWorkingSetSize", SetLastError = true)]
	internal static extern bool SetProcessWorkingSetSize32Bit(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

	[DllImport("KERNEL32.DLL", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetProcessWorkingSetSize", SetLastError = true)]
	internal static extern bool SetProcessWorkingSetSize64Bit(IntPtr pProcess, long dwMinimumWorkingSetSize, long dwMaximumWorkingSetSize);

	public static void FlushMem()
	{
		GC.Collect();
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			SetProcessWorkingSetSize32Bit(Process.GetCurrentProcess().Handle, -1, -1);
		}
	}
}
