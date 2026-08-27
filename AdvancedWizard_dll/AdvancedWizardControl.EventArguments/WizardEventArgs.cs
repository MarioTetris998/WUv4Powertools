using System;

namespace AdvancedWizardControl.EventArguments;

public class WizardEventArgs : EventArgs
{
	public bool AllowPageChange { get; set; }

	public int CurrentPageIndex { get; private set; }

	public int NextPageIndex { get; set; }

	public WizardEventArgs(int currentPageIndex)
	{
		CurrentPageIndex = currentPageIndex;
		NextPageIndex = currentPageIndex + 1;
		AllowPageChange = true;
	}
}
