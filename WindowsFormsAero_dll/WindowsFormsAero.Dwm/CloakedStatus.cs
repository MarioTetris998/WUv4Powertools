using System;

namespace WindowsFormsAero.Dwm;

/// <summary>
/// Describes whether and why a window is cloaked by the DWM.
/// </summary>
[Flags]
public enum CloakedStatus
{
	/// <summary>
	/// Window is not cloaked.
	/// </summary>
	Uncloaked = 0,
	/// <summary>
	/// The window was cloaked by its owner application.
	/// </summary>
	ApplicationLevel = 1,
	/// <summary>
	/// The window was cloaked by the Shell.
	/// </summary>
	ShellLevel = 2,
	/// <summary>
	/// The cloak value was inherited from its owner window.
	/// </summary>
	Inherited = 4
}
