using System;

namespace WindowsFormsAero.TaskDialog;

public class CheckEventArgs : EventArgs
{
	public bool IsChecked { get; set; }

	public CheckEventArgs(bool state)
	{
		IsChecked = state;
	}
}
