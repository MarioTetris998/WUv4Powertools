using System;

namespace WindowsFormsAero.TaskDialog;

public class TimerEventArgs : EventArgs
{
	public long Ticks { get; set; }

	public bool ResetCount { get; set; }

	public TimerEventArgs(long ticks)
	{
		Ticks = ticks;
		ResetCount = false;
	}
}
