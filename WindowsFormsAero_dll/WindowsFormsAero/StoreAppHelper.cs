using System.Text;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

/// <summary>
/// Exposes helpers and auxilary methods for Windows Store apps
/// (i.e., desktop applications converted through Centennial).
/// </summary>
public static class StoreAppHelper
{
	/// <summary>
	/// Gets whether the current process is running inside an UWP container
	/// "Windows Store" application.
	/// </summary>
	/// <remarks>
	/// Taken from https://github.com/qmatteoq/DesktopBridgeHelpers by Matteo Pagani.
	/// </remarks>
	public static bool IsRunningAsStoreApp()
	{
		if (!OsSupport.IsEightOrLater)
		{
			return false;
		}
		StringBuilder sb = new StringBuilder(0);
		int length = 0;
		return UwpMethods.GetCurrentPackageFullName(ref length, ref sb) == 122;
	}
}
