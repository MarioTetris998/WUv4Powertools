using System;

namespace WindowsFormsAero.TaskDialog;

public class HyperlinkEventArgs : EventArgs
{
	public string Url { get; set; }

	public HyperlinkEventArgs(string url)
	{
		Url = url;
	}
}
