using System;
using AdvancedWizardControl.WizardPages;

namespace AdvancedWizardControl.EventArguments;

public class WizardPageChangedEventArgs : EventArgs
{
	private AdvancedWizardPage _page;

	private readonly int _pageIndex;

	public AdvancedWizardPage Page
	{
		get
		{
			return _page;
		}
		set
		{
			_page = value;
		}
	}

	public int PageIndex => _pageIndex;

	public bool SetAsFinishPage { get; set; }

	public WizardPageChangedEventArgs(AdvancedWizardPage page, int pageIndex)
	{
		Page = page;
		_pageIndex = pageIndex;
	}
}
