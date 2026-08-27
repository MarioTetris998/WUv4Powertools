using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

[ToolboxBitmap(typeof(ComboBox))]
public class ComboBox : System.Windows.Forms.ComboBox
{
	private string _cueBannerText = string.Empty;

	[Description("Gets or sets the cue text that is displayed on a ComboBox control.")]
	[Category("Appearance")]
	[DefaultValue("")]
	public string CueBannerText
	{
		get
		{
			return _cueBannerText;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			if (_cueBannerText != value)
			{
				Methods.SendMessage(base.Handle, 5891u, 0, value);
			}
			_cueBannerText = value;
		}
	}

	public ComboBox()
	{
		base.FlatStyle = FlatStyle.System;
		base.DropDownStyle = ComboBoxStyle.DropDownList;
	}
}
