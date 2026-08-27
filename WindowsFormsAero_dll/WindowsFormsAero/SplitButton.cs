using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

/// <summary>
/// A complex button provided with a secondary split push button
/// that open up a context menu.
/// </summary>
/// <remarks>
/// See: http://www.codeproject.com/KB/vista/themedvistacontrols.aspx
/// </remarks>
public class SplitButton : Button
{
	private bool _alignLeft;

	private bool _noSplit;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams obj = base.CreateParams;
			obj.Style |= (base.IsDefault ? 13 : 12);
			return obj;
		}
	}

	/// <summary>
	/// Gets or sets the associated context menu that is displayed when the split
	/// glyph of the button is clicked.
	/// </summary>
	[Description("Sets the context menu that is displayed by clicking on the split button.")]
	[Category("Behavior")]
	[DefaultValue(null)]
	public ContextMenuStrip SplitMenuStrip { get; set; }

	/// <summary>
	/// Gets or sets the associated context menu that is displayed when the split
	/// glyph of the button is clicked.
	/// </summary>
	/// <remarks>
	/// Exposed for backward compatibility with legacy context menu classes.
	/// If both <see cref="P:WindowsFormsAero.SplitButton.SplitMenuStrip" /> and <see cref="P:WindowsFormsAero.SplitButton.SplitMenu" /> are
	/// set, the first is preferred.
	/// </remarks>
	[Description("Sets the context menu that is displayed by clicking on the split button.")]
	[Category("Behavior")]
	[DefaultValue(null)]
	public ContextMenu SplitMenu { get; set; }

	/// <summary>
	/// Gets or sets whether the split button should be aligned on the left side of the button.
	/// </summary>
	[Description("Align the split button on the left side of the button.")]
	[Category("Appearance")]
	[DefaultValue(false)]
	public bool SplitButtonAlignLeft
	{
		get
		{
			return _alignLeft;
		}
		set
		{
			_alignLeft = value;
			UpdateStyle();
		}
	}

	/// <summary>
	/// Gets or sets whether the split button should be shown or not.
	/// </summary>
	[Description("Hide the split button.")]
	[Category("Appearance")]
	[DefaultValue(false)]
	public bool SplitButtonNoSplit
	{
		get
		{
			return _noSplit;
		}
		set
		{
			_noSplit = value;
			UpdateStyle();
		}
	}

	/// <summary>
	/// Occurs when the split label is clicked.
	/// </summary>
	[Description("Occurs when the split button is clicked.")]
	[Category("Action")]
	public event EventHandler<SplitMenuEventArgs> SplitClick;

	/// <summary>
	/// Occurs when the split label is clicked, but before the associated
	/// context menu is displayed by the control.
	/// </summary>
	[Description("Occurs when the split label is clicked, but before the associated context menu is displayed.")]
	[Category("Action")]
	public event EventHandler<SplitMenuEventArgs> SplitMenuOpening;

	protected virtual void OnSplitClick(SplitMenuEventArgs e)
	{
		this.SplitClick?.Invoke(this, e);
		if (SplitMenu == null && SplitMenuStrip == null)
		{
			return;
		}
		this.SplitMenuOpening?.Invoke(this, e);
		if (!e.PreventOpening)
		{
			System.Drawing.Point pBottomLeft = new System.Drawing.Point(e.DrawArea.Left, e.DrawArea.Bottom);
			if (SplitMenu != null)
			{
				SplitMenu.Show(this, pBottomLeft);
			}
			else if (SplitMenuStrip != null)
			{
				SplitMenuStrip.Width = e.DrawArea.Width;
				SplitMenuStrip.Show(this, pBottomLeft);
			}
		}
	}

	private void UpdateStyle()
	{
		using StructWrapper<ButtonSplitInfo> hSplitInfo = new StructWrapper<ButtonSplitInfo>(new ButtonSplitInfo
		{
			Mask = ButtonSplitInfo.MaskType.Style,
			Style = (SplitButtonAlignLeft ? ButtonSplitInfo.SplitStyle.AlignLeft : ButtonSplitInfo.SplitStyle.None)
		});
		Methods.SendMessage(base.Handle, 5639u, IntPtr.Zero, hSplitInfo);
	}

	protected override void WndProc(ref Message m)
	{
		if (m.Msg == 5638 && m.WParam.ToInt32() == 1)
		{
			OnSplitClick(new SplitMenuEventArgs(base.ClientRectangle));
		}
		base.WndProc(ref m);
	}
}
