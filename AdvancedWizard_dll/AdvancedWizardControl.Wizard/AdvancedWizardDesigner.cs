using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using AdvancedWizardControl.WizardPages;

namespace AdvancedWizardControl.Wizard;

internal class AdvancedWizardDesigner : ParentControlDesigner
{
	private const int VerbPrevious = 1;

	private const int VerbNext = 2;

	private IComponentChangeService _changeService;

	private IDesignerHost _designer;

	private ISelectionService _selectionService;

	private DesignerVerbCollection _verbs;

	private AdvancedWizard _wizard;

	public override DesignerVerbCollection Verbs => _verbs ?? (_verbs = new DesignerVerbCollection
	{
		new DesignerVerb("New WizardPage", OnVerbNew),
		new DesignerVerb("Prev WizardPage", OnVerbPrev),
		new DesignerVerb("Next WizardPage", OnVerbNext),
		new DesignerVerb("About", OnVerbAbout)
	});

	public override void Initialize(IComponent c)
	{
		base.Initialize(c);
		GetReferenceToWizardControl(c);
		GetReferenceToIDesignerHost();
		GetReferenceToIComponentChangeService();
		GetReferenceToISelectionService();
		InitializeWizardControl();
		InitializeDesigner();
	}

	protected override void Dispose(bool disposing)
	{
		_changeService.ComponentAdded -= ChangeServiceComponentAdded;
		_changeService.ComponentRemoved -= ChangeServiceComponentRemoved;
		base.Dispose(disposing);
	}

	protected override void PostFilterProperties(IDictionary properties)
	{
		base.PostFilterProperties(properties);
		properties.Remove("BackColor");
		properties.Remove("BackgroundImage");
	}

	protected override bool GetHitTest(Point point)
	{
		Point point2 = Control.PointToClient(point);
		UpdateMenuCommands();
		return _wizard.UserClickedAButtonAtDesignTime(point2);
	}

	protected void UpdateMenuCommands()
	{
		if (_wizard.WizardPages.Count > 1)
		{
			if (_wizard.IndexOfCurrentPage() == _wizard.WizardPages.Count - 1)
			{
				Verbs[1].Enabled = true;
				Verbs[2].Enabled = false;
			}
			else if (_wizard.IndexOfCurrentPage() == 0)
			{
				Verbs[1].Enabled = false;
				Verbs[2].Enabled = true;
			}
			else
			{
				Verbs[1].Enabled = true;
				Verbs[2].Enabled = true;
			}
		}
		else
		{
			Verbs[1].Enabled = false;
			Verbs[2].Enabled = false;
		}
	}

	private void OnVerbNew(object sender, EventArgs e)
	{
		_designer.CreateComponent(typeof(AdvancedWizardPage));
	}

	private void OnVerbPrev(object sender, EventArgs e)
	{
		if (!_wizard.WizardHasNoPages() && _wizard.IndexOfCurrentPage() > 0)
		{
			_wizard.ClickBack();
			Verbs[2].Enabled = true;
			if (_wizard.OnFirstPage())
			{
				Verbs[1].Enabled = false;
			}
		}
	}

	private void OnVerbNext(object sender, EventArgs e)
	{
		if (((AdvancedWizard)Control).WizardPages.Count > 0)
		{
			_wizard.ClickNext();
			Verbs[1].Enabled = true;
			if (_wizard.OnLastPage())
			{
				Verbs[2].Enabled = false;
			}
		}
	}

	private static void OnVerbAbout(object sender, EventArgs e)
	{
		MessageBox.Show("Written by Steve Bate", "About AdvancedWizard", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	}

	private void InitializeWizardControl()
	{
		_wizard.AllowDrop = false;
	}

	private void InitializeDesigner()
	{
		DrawGrid = false;
	}

	private void GetReferenceToWizardControl(IComponent c)
	{
		_wizard = ((Control)c) as AdvancedWizard;
	}

	private void GetReferenceToIComponentChangeService()
	{
		_changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
		_changeService.ComponentAdded += ChangeServiceComponentAdded;
		_changeService.ComponentRemoved += ChangeServiceComponentRemoved;
	}

	private void GetReferenceToIDesignerHost()
	{
		_designer = (IDesignerHost)GetService(typeof(IDesignerHost));
	}

	private void GetReferenceToISelectionService()
	{
		_selectionService = (ISelectionService)GetService(typeof(ISelectionService));
	}

	private void UpdateWizard(AdvancedWizardPage page)
	{
		_wizard.SelectWizardPage(page);
		_wizard.SetButtonStates();
	}

	private static void DisplayPage(AdvancedWizardPage page)
	{
		page.Dock = DockStyle.Fill;
		page.BringToFront();
	}

	private void AddPageToContainers(AdvancedWizardPage page)
	{
		_wizard.WizardPages.Add(page);
		_wizard.Controls.Add(page);
	}

	private void SelectPageInProperyGrid(AdvancedWizardPage page)
	{
		_selectionService.SetSelectedComponents(new object[1] { page }, SelectionTypes.Primary);
	}

	private void ChangeServiceComponentAdded(object sender, ComponentEventArgs e)
	{
		if (!((IDesignerHost)sender).Loading && e.Component is AdvancedWizardPage)
		{
			AdvancedWizardPage page = e.Component as AdvancedWizardPage;
			if (!_wizard.WizardPages.Contains(page))
			{
				AddPageToContainers(page);
				DisplayPage(page);
				SelectPageInProperyGrid(page);
				UpdateWizard(page);
			}
		}
	}

	private void ChangeServiceComponentRemoved(object sender, ComponentEventArgs e)
	{
		if (!((IDesignerHost)sender).Loading && e.Component is AdvancedWizardPage page)
		{
			_wizard.WizardPages.Remove(page);
			_wizard.SelectPreviousPage();
			_wizard.SetButtonStates();
		}
	}
}
