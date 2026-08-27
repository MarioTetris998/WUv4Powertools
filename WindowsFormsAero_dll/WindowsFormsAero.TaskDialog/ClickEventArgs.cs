using System;

namespace WindowsFormsAero.TaskDialog;

public class ClickEventArgs : EventArgs
{
	public int ButtonID { get; set; }

	public bool PreventClosing { get; set; }

	public ClickEventArgs(int buttonID)
	{
		ButtonID = buttonID;
		PreventClosing = false;
	}
}
