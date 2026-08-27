using System.ComponentModel.Design;
using AdvancedWizardControl.Enums;
using AdvancedWizardControl.EventArguments;
using AdvancedWizardControl.Wizard;
using AdvancedWizardControl.WizardPages;

namespace AdvancedWizardControl.Strategies;

public class RuntimeWizardStrategy : WizardStrategy
{
	private readonly AdvancedWizard _wizard;

	public RuntimeWizardStrategy(AdvancedWizard wizard)
	{
		_wizard = wizard;
	}

	public override void Loading()
	{
		if (_wizard.HasPages())
		{
			GoToPage(0);
		}
	}

	public override void SetButtonStates()
	{
		if (_wizard.OnLastPage() && _wizard.HasOnePage())
		{
			_wizard.BackButtonEnabled = false;
			_wizard.NextButtonEnabled = false;
		}
		else if (_wizard.OnLastPage() && _wizard.HasExplicitFinishButton())
		{
			_wizard.BackButtonEnabled = true;
			_wizard.NextButtonEnabled = false;
		}
		else if (_wizard.OnLastPage())
		{
			_wizard.BackButtonEnabled = true;
			_wizard.FinishButtonEnabled = true;
			_wizard.SetButtonText("btnNext", _wizard.FinishButtonText);
		}
		else if (_wizard.OnFirstPage())
		{
			_wizard.BackButtonEnabled = false;
			_wizard.NextButtonEnabled = _wizard.WizardPages.Count > 1;
			_wizard.SetButtonText("btnNext", _wizard.ReadNextText());
		}
		else if (_wizard.OnAMiddlePage())
		{
			_wizard.BackButtonEnabled = true;
			_wizard.NextButtonEnabled = _wizard.NextButtonEnabledState;
			_wizard.SetButtonText("btnNext", _wizard.ReadNextText());
		}
	}

	public override void Cancel()
	{
		if (_wizard.CancelEventAssigned())
		{
			_wizard.FireCancelEvent();
		}
	}

	public override void Help()
	{
		if (_wizard.HelpEventAssigned())
		{
			_wizard.FireHelpEvent();
		}
	}

	public override void Finish()
	{
		if (_wizard.FinishEventAssigned())
		{
			_wizard.FireFinishEvent();
		}
	}

	public override void Back(ISelectionService selection)
	{
		MoveToPreviousPage();
		SetButtonStates();
	}

	public override void Next(ISelectionService selection)
	{
		if (!Finishing() && UserAllowsMoveToProceed(Direction.Forward, out var eventArgs) && _wizard.MoreThanOnePageExists())
		{
			MoveToNextPage(eventArgs);
			SetButtonStates();
		}
	}

	public override void GoToPage(int pageIndex)
	{
		int index = _wizard.IndexOfCurrentPage();
		_wizard.SelectWizardPage(pageIndex);
		_wizard.StoreIndexOfCurrentPage(index);
		_wizard.CurrentPage.FirePageShowEvent();
		if (_wizard.PageChangedEventAssigned())
		{
			_wizard.FirePageChanged(_wizard.IndexOfCurrentPage());
		}
	}

	public override void GoToPage(AdvancedWizardPage page)
	{
		int index = _wizard.IndexOfCurrentPage();
		_wizard.SelectWizardPage(page);
		_wizard.StoreIndexOfCurrentPage(index);
		SetButtonStates();
		page.FirePageShowEvent();
		if (_wizard.PageChangedEventAssigned())
		{
			_wizard.FirePageChanged(_wizard.IndexOfCurrentPage());
		}
	}

	private bool UserAllowsMoveToProceed(Direction direction, out WizardEventArgs eventArgs)
	{
		return (eventArgs = ((direction == Direction.Forward) ? AttemptMoveToNextPage() : AttemptMoveToPreviousPage())).AllowPageChange;
	}

	private void MoveToPreviousPage()
	{
		_wizard.SelectWizardPage(_wizard.ReadIndexOfPreviousPage());
		_wizard.NextButtonEnabledState = true;
		int index = _wizard.IndexOfCurrentPage();
		_wizard.WizardPages[index].FirePageShowEvent();
		if (_wizard.PageChangedEventAssigned())
		{
			_wizard.FirePageChanged(index);
		}
	}

	private void MoveToNextPage(WizardEventArgs args)
	{
		if (CanMoveToNextPage(args))
		{
			_wizard.SelectWizardPage(args.NextPageIndex);
			_wizard.StoreIndexOfCurrentPage(args.CurrentPageIndex);
			_wizard.WizardPages[args.NextPageIndex].FirePageShowEvent();
			if (_wizard.PageChangedEventAssigned())
			{
				_wizard.FirePageChanged(args.NextPageIndex);
			}
			if (args.NextPageIndex == _wizard.WizardPages.Count - 1 && _wizard.LastPageEventAssigned())
			{
				_wizard.FireLastPage();
			}
		}
	}

	private bool CanMoveToNextPage(WizardEventArgs args)
	{
		return args.NextPageIndex < _wizard.WizardPages.Count;
	}

	private WizardEventArgs AttemptMoveToPreviousPage()
	{
		return FireBackEvent() ?? new WizardEventArgs(_wizard.IndexOfCurrentPage());
	}

	private WizardEventArgs AttemptMoveToNextPage()
	{
		return FireNextEvent() ?? new WizardEventArgs(_wizard.IndexOfCurrentPage());
	}

	private WizardEventArgs FireBackEvent()
	{
		int currentTabIndex = _wizard.IndexOfCurrentPage();
		WizardEventArgs e = null;
		if (_wizard.BackEventAssigned())
		{
			e = _wizard.FireBackEvent(currentTabIndex);
			_wizard.CheckForUserChangesToEventParameters(e, out var _, out var _);
		}
		return e;
	}

	private WizardEventArgs FireNextEvent()
	{
		int currentTabIndex = _wizard.IndexOfCurrentPage();
		WizardEventArgs e = null;
		if (_wizard.NextEventAssigned())
		{
			e = _wizard.FireNextEvent(currentTabIndex);
			_wizard.CheckForUserChangesToEventParameters(e, out var _, out var _);
		}
		return e;
	}

	private bool Finishing()
	{
		bool result = false;
		if (_wizard.OnLastPage() && !_wizard.HasExplicitFinishButton())
		{
			if (_wizard.FinishEventAssigned())
			{
				_wizard.FireFinishEvent();
			}
			result = true;
		}
		return result;
	}
}
