using System;
using System.ComponentModel.Design;
using AdvancedWizardControl.WizardPages;

namespace AdvancedWizardControl.Wizard;

public class AdvancedWizardCollectionEditor : CollectionEditor
{
	public AdvancedWizardCollectionEditor(Type wizardPage)
		: base(wizardPage)
	{
	}

	protected override Type[] CreateNewItemTypes()
	{
		return new Type[1] { typeof(AdvancedWizardPage) };
	}
}
