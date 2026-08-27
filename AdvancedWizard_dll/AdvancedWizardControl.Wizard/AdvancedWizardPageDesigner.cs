using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using AdvancedWizardControl.WizardPages;

namespace AdvancedWizardControl.Wizard;

internal class AdvancedWizardPageDesigner : ParentControlDesigner
{
	private AdvancedWizardPage _page;

	public override SelectionRules SelectionRules => DoNotAllowPageToBeManipulatedByMouse();

	public override void Initialize(IComponent c)
	{
		base.Initialize(c);
		GetReferenceToWizardPage();
		InitializeWizardPage();
		InitializeDesigner();
	}

	protected override void OnDragDrop(DragEventArgs de)
	{
		de.Effect = DragDropEffects.Move;
		base.OnDragDrop(de);
	}

	private void GetReferenceToWizardPage()
	{
		_page = Control as AdvancedWizardPage;
	}

	private void InitializeWizardPage()
	{
		_page.AllowDrop = false;
	}

	private void InitializeDesigner()
	{
		DrawGrid = true;
		EnableDragDrop(value: true);
	}

	private SelectionRules DoNotAllowPageToBeManipulatedByMouse()
	{
		_ = base.SelectionRules;
		return SelectionRules.None;
	}
}
