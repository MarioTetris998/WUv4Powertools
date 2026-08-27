using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

[ToolboxBitmap(typeof(Button))]
public class Button : System.Windows.Forms.Button
{
	private bool _showShield;

	[Description("Gets or sets whether if the control should use an elevated shield icon.")]
	[Category("Appearance")]
	[DefaultValue(false)]
	public bool ShowShield
	{
		get
		{
			return _showShield;
		}
		set
		{
			if (_showShield != value)
			{
				if (value)
				{
					base.FlatStyle = FlatStyle.System;
				}
				else if (base.Image != null)
				{
					base.FlatStyle = FlatStyle.Standard;
				}
				if (base.IsHandleCreated)
				{
					Methods.SendMessage(base.Handle, 5644u, 0, value ? 1 : 0);
				}
			}
			_showShield = value;
		}
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		Methods.SendMessage(base.Handle, 5644u, 0, _showShield ? 1 : 0);
	}
}
