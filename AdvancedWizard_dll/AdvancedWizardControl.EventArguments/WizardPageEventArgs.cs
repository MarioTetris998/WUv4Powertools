using System;
using AdvancedWizardControl.WizardPages;

namespace AdvancedWizardControl.EventArguments;

public class WizardPageEventArgs : EventArgs
{
	private readonly AdvancedWizardPage _page;

	public AdvancedWizardPage Page => _page;

	public WizardPageEventArgs(AdvancedWizardPage page)
	{
		_page = page;
	}
}
