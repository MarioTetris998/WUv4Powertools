using System;

namespace WindowsFormsAero.TaskDialog;

public class ExpandEventArgs : EventArgs
{
	public bool IsExpanded { get; set; }

	public ExpandEventArgs(bool state)
	{
		IsExpanded = state;
	}
}
