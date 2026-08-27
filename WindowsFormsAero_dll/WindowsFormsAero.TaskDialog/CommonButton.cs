using System;

namespace WindowsFormsAero.TaskDialog;

/// <summary>
/// Common Task Dialog buttons.
/// </summary>
[Flags]
public enum CommonButton
{
	OK = 1,
	Cancel = 8,
	Yes = 2,
	No = 4,
	Retry = 0x10,
	Close = 0x20
}
