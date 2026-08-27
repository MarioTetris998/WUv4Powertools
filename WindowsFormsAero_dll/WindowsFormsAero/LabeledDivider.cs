using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsAero;

/// <summary>
/// The labeled divider provides a Aero styled divider with an optional caption,
/// similiar to what is seen in the Control Panel dialogs of Windows 7 and Vista.
/// </summary>
public class LabeledDivider : Label
{
	/// <summary>
	/// The positions that the divider line can be drawn in
	/// </summary>
	public enum DividerPositions
	{
		/// <summary>
		/// The divider will be centered after the text caption and will begin drawing after the string.  This is the default behavior.
		/// </summary>
		Center,
		/// <summary>
		/// The divider will be drawn below the text caption.
		/// </summary>
		Below
	}

	private DividerPositions _dividerPosition;

	private Color _dividerColor = Color.FromArgb(176, 191, 222);

	private string _text = "";

	/// <summary>
	/// The position of the divider line.
	/// </summary>
	/// <remarks>
	/// The default value is the center position which is consistent on how this type of divider has been used throughout the Windows
	/// 7 and Vista UI's.
	/// </remarks>
	[Description("The placement of the divider line.")]
	[Category("Appearance")]
	[DefaultValue(DividerPositions.Center)]
	public DividerPositions DividerPosition
	{
		get
		{
			return _dividerPosition;
		}
		set
		{
			_dividerPosition = value;
			Invalidate();
		}
	}

	/// <summary>
	/// The color of the divider line.
	/// </summary>
	[Description("The color of the divider line.")]
	[Category("Appearance")]
	public Color DividerColor
	{
		get
		{
			return _dividerColor;
		}
		set
		{
			_dividerColor = value;
			Invalidate();
		}
	}

	/// <summary>
	/// The text that should be used for the caption.  If the caption is set to blank and the divider position is set to center then
	/// a simple divider line will be drawn.
	/// </summary>
	/// <remarks>
	/// After a change is made to the text property we want to invalidate the control so it triggers a new paint message being sent.
	/// </remarks>
	[Description("The text that will display as the caption.")]
	[Category("Appearance")]
	[DefaultValue("DividerLabel")]
	public override string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			Invalidate();
		}
	}

	/// <summary>
	/// Constructor
	/// </summary>
	public LabeledDivider()
	{
		Font = new Font(Font.FontFamily, 9f);
		ForeColor = Color.FromArgb(0, 51, 170);
		AutoSize = false;
	}

	/// <summary>
	/// The actual painting of the background of our control.
	/// </summary>
	/// <param name="e"></param>
	/// <remarks>
	/// The colors in use here were extracted from an image of the Control Panel taken from a Windows 7 RC1 installation.
	/// </remarks>
	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		e.Graphics.Clear(BackColor);
		SolidBrush sbDividerColor = new SolidBrush(_dividerColor);
		SolidBrush solidBrush = new SolidBrush(ForeColor);
		e.Graphics.DrawString(Text, Font, Brushes.Black, base.ClientRectangle.X, base.ClientRectangle.Y);
		SizeF sf = e.Graphics.MeasureString(Text, Font);
		if (DividerPosition == DividerPositions.Center)
		{
			Rectangle rect = new Rectangle((int)sf.Width, (int)sf.Height / 2 + 1, base.Width - (int)sf.Width, 1);
			e.Graphics.FillRectangle(sbDividerColor, rect);
		}
		else if (DividerPosition == DividerPositions.Below)
		{
			Rectangle rect2 = new Rectangle(1, (int)sf.Height, base.Width, 1);
			e.Graphics.FillRectangle(sbDividerColor, rect2);
		}
		solidBrush.Dispose();
		sbDividerColor.Dispose();
	}
}
