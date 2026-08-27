using System;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

/// <summary>
/// Static class providing information about the running OS's version.
/// </summary>
public static class OsSupport
{
	private const int VistaMajorVersion = 6;

	private const int SevenMinorVersion = 1;

	private const int EightMinorVersion = 2;

	private const int EightDotOneMinorVersion = 3;

	private const int TenMajorVersion = 10;

	private const int TenAnniversaryBuild = 14393;

	/// <summary>
	/// Gets whether the running operating system is Windows Vista or a more recent
	/// version.
	/// </summary>
	[Obsolete("Use IsVistaOrLater")]
	public static bool IsVistaOrBetter
	{
		get
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				return Environment.OSVersion.Version.Major >= 6;
			}
			return false;
		}
	}

	/// <summary>
	/// Gets whether the running operating system is Windows Vista or a more recent
	/// version.
	/// </summary>
	public static bool IsVistaOrLater
	{
		get
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				return Environment.OSVersion.Version.Major >= 6;
			}
			return false;
		}
	}

	/// <summary>
	/// Gets whether the running operating system is Windows Seven or a more recent
	/// version.
	/// </summary>
	[Obsolete("Use IsSevenOrLater")]
	public static bool IsSevenOrBetter => IsSevenOrLater;

	/// <summary>
	/// Gets whether the running operating system is Windows Seven or a more recent
	/// version.
	/// </summary>
	public static bool IsSevenOrLater
	{
		get
		{
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				return false;
			}
			Version version = Environment.OSVersion.Version;
			if (version.Major > 6)
			{
				return true;
			}
			if (version.Major == 6)
			{
				return version.Minor >= 1;
			}
			return false;
		}
	}

	/// <summary>
	/// Gets whether the running operating system is Windows 8 or a more recent
	/// version.
	/// </summary>
	[Obsolete("Use IsEightOrLater")]
	public static bool IsEightOrBetter => IsEightOrLater;

	/// <summary>
	/// Gets whether the running operating system is Windows 8 or a more recent
	/// version.
	/// </summary>
	public static bool IsEightOrLater
	{
		get
		{
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				return false;
			}
			Version version = Environment.OSVersion.Version;
			if (version.Major > 6)
			{
				return true;
			}
			if (version.Major == 6)
			{
				return version.Minor >= 2;
			}
			return false;
		}
	}

	/// <summary>
	/// Gets whether the running operating system is Windows 8.1 or a more recent
	/// version.
	/// </summary>
	[Obsolete("Use IsEightDotOneOrLater")]
	public static bool IsEightDotOneOrBetter => IsEightDotOneOrLater;

	/// <summary>
	/// Gets whether the running operating system is Windows 8.1 or a more recent
	/// version.
	/// </summary>
	public static bool IsEightDotOneOrLater
	{
		get
		{
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				return false;
			}
			Version version = Environment.OSVersion.Version;
			if (version.Major > 6)
			{
				return true;
			}
			if (version.Major == 6)
			{
				return version.Minor >= 3;
			}
			return false;
		}
	}

	/// <summary>
	/// Gets whether the running operating system is Windows 10 or a more recent
	/// version.
	/// </summary>
	[Obsolete("Use IsTenOrLater")]
	public static bool IsTenOrBetter => IsTenOrLater;

	/// <summary>
	/// Gets whether the running operating system is Windows 10 or a more recent
	/// version.
	/// </summary>
	public static bool IsTenOrLater
	{
		get
		{
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				return false;
			}
			if (Environment.OSVersion.Version.Major >= 10)
			{
				return true;
			}
			return false;
		}
	}

	/// <summary>
	/// Gets whether the running operating system is Windows 10 "Anniversary Edition"
	/// or a more recent version.
	/// </summary>
	[Obsolete("Use IsTenAnniversaryEditionOrLater")]
	public static bool IsTenAnniversaryEditionOrBetter => IsTenAnniversaryEditionOrLater;

	/// <summary>
	/// Gets whether the running operating system is Windows 10 "Anniversary Edition"
	/// or a more recent version.
	/// </summary>
	public static bool IsTenAnniversaryEditionOrLater
	{
		get
		{
			if (!IsTenOrLater)
			{
				return false;
			}
			return Environment.OSVersion.Version.Build >= 14393;
		}
	}

	/// <summary>
	/// Is true if the DWM composition engine is currently enabled.
	/// </summary>
	public static bool IsCompositionEnabled
	{
		get
		{
			if (!IsVistaOrLater)
			{
				return false;
			}
			try
			{
				DwmMethods.DwmIsCompositionEnabled(out var enabled);
				return enabled;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
