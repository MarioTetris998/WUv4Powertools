using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

[ToolboxBitmap(typeof(Button))]
public class CommandLink : Button
{
	private string _note = string.Empty;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams cp = base.CreateParams;
			if (OsSupport.IsVistaOrLater)
			{
				cp.Style |= (base.IsDefault ? 15 : 14);
			}
			else
			{
				cp.Style |= ((!base.IsDefault) ? 1 : 0);
			}
			return cp;
		}
	}

	[Description("Gets or sets the note that is displayed on a button control.")]
	[Category("Appearance")]
	[DefaultValue("")]
	public string Note
	{
		get
		{
			return _note;
		}
		set
		{
			if (base.IsHandleCreated)
			{
				Methods.SendMessage(base.Handle, 5641u, 0, _note);
			}
			_note = value;
		}
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		Methods.SendMessage(base.Handle, 5641u, 0, _note);
	}
}
