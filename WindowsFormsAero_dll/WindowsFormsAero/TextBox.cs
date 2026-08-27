using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

[ToolboxBitmap(typeof(TextBox))]
public class TextBox : System.Windows.Forms.TextBox
{
	private string _cueBannerText = string.Empty;

	private bool _showCueFocused;

	/// <summary>
	/// Gets or sets the cue text that is displayed on the TextBox control.
	/// </summary>
	[Description("Text that is displayed as Cue banner.")]
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
			_cueBannerText = value;
			UpdateControl();
		}
	}

	/// <summary>
	/// Gets or sets whether the Cue text should be displyed even
	/// when the control has keybord focus.
	/// </summary>
	/// <remarks>
	/// If true, the Cue text will disappear as soon as the user starts typing.
	/// </remarks>
	[Description("If true, the Cue text will be displayed even when the control has keyboard focus.")]
	[Category("Appearance")]
	[DefaultValue(false)]
	public bool ShowCueFocused
	{
		get
		{
			return _showCueFocused;
		}
		set
		{
			_showCueFocused = value;
			UpdateControl();
		}
	}

	private void UpdateControl()
	{
		if (base.IsHandleCreated)
		{
			Methods.SendMessage(base.Handle, 5377u, _showCueFocused ? 1 : 0, _cueBannerText);
		}
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		UpdateControl();
	}
}
