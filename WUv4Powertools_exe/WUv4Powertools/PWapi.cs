using System.Linq;

namespace WUv4Powertools;

public static class PWapi
{
	public static string getProvString(string provider)
	{
		return provider switch
		{
			"win98se" => "win98se.windows98andwindows98secondedition.ver_platform_win32_windows.4.10.x86", 
			"winme" => "winme.windowsmillenniumedition.ver_platform_win32_windows.4.90.x86", 
			"win2k" => "win2k.windows2000.ver_platform_win32_nt.5.0.x86", 
			"winxp" => "winxp.windowsxp.ver_platform_win32_nt.5.1.x86", 
			"netserver" => "netserver.windowsnetserver2003family.ver_platform_win32_nt.5.2.x86",
			"ie50x" => "ie50x.internetexplorer5x.ver_platform_win32_nt.5.0.x86",
			"ie55x" => "ie55x.internetexplorer55x.ver_platform_win32_nt.5.0.x86",
			"ie60x" => "ie60x.internetexplorer6x.ver_platform_win32_nt.5.0.x86",
			_ => null, 
		};
	}

	public static string[] DuoOptimize(string[] lines)
	{
		return lines.Distinct().ToArray();
	}
}
