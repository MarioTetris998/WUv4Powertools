using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WindowsFormsAero;

/// <summary>
/// A Label displaying text with themeing options (glow, drop shadows, etc.)
/// that can be shown on transparent surfaces (esp. extended frames for the
/// so-called "Glass Sheet" effect).
/// </summary>
[DefaultProperty("Text")]
public class ThemeLabel : Control
{
	private ThemedText _themeText;

	private int _glowSize = 10;

	private bool _glowEnabled = true;

	private HorizontalAlignment _horizontal;

	private VerticalAlignment _vertical;

	private bool _singleLine = true;

	private bool _endEllipsis;

	private bool _wordBreak;

	private bool _wordEllipsis;

	public override string Text
	{
		set
		{
			base.Text = value;
			UpdateText();
		}
	}

	public override Font Font
	{
		set
		{
			base.Font = value;
			UpdateText();
		}
	}

	public new Padding Padding
	{
		get
		{
			return base.Padding;
		}
		set
		{
			base.Padding = value;
			UpdateText();
		}
	}

	public override Color ForeColor
	{
		set
		{
			base.ForeColor = value;
			UpdateText();
		}
	}

	[Browsable(false)]
	public new Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
		}
	}

	[Browsable(false)]
	public new Image BackgroundImage
	{
		get
		{
			return base.BackgroundImage;
		}
		set
		{
		}
	}

	[Browsable(false)]
	public new ImageLayout BackgroundImageLayout
	{
		get
		{
			return base.BackgroundImageLayout;
		}
		set
		{
		}
	}

	/// <summary>
	/// Size of the glow effect around the text.
	/// </summary>
	[Description("Size of the glow effect around the text.")]
	[Category("Appearance")]
	[DefaultValue(10)]
	public int GlowSize
	{
		get
		{
			return _glowSize;
		}
		set
		{
			_glowSize = value;
			UpdateText();
		}
	}

	/// <summary>
	/// Enables or disables the glow effect around the text.
	/// </summary>
	[Description("Enables or disables the glow effect around the text.")]
	[Category("Appearance")]
	[DefaultValue(true)]
	public bool GlowEnabled
	{
		get
		{
			return _glowEnabled;
		}
		set
		{
			_glowEnabled = value;
			UpdateText();
		}
	}

	/// <summary>
	/// Gets or sets the horizontal text alignment setting.
	/// </summary>
	[Description("Horizontal text alignment.")]
	[Category("Appearance")]
	[DefaultValue(typeof(HorizontalAlignment), "Left")]
	public HorizontalAlignment TextAlign
	{
		get
		{
			return _horizontal;
		}
		set
		{
			_horizontal = value;
			UpdateText();
		}
	}

	/// <summary>
	/// Gets or sets the vertical text alignment setting.
	/// </summary>
	[Description("Vertical text alignment.")]
	[Category("Appearance")]
	[DefaultValue(typeof(VerticalAlignment), "Top")]
	public VerticalAlignment TextAlignVertical
	{
		get
		{
			return _vertical;
		}
		set
		{
			_vertical = value;
			UpdateText();
		}
	}

	/// <summary>
	/// Gets or sets whether the text will be laid out on a single line or on
	/// multiple lines.
	/// </summary>
	[Description("Single line text.")]
	[Category("Appearance")]
	[DefaultValue(true)]
	public bool SingleLine
	{
		get
		{
			return _singleLine;
		}
		set
		{
			_singleLine = value;
			UpdateText();
		}
	}

	/// <summary>
	/// Gets or sets whether the text lines over the label's border should
	/// be trimmed with an ellipsis.
	/// </summary>
	[Description("Removes the end of trimmed lines and replaces them with an ellipsis.")]
	[Category("Appearance")]
	[DefaultValue(false)]
	public bool EndEllipsis
	{
		get
		{
			return _endEllipsis;
		}
		set
		{
			_endEllipsis = value;
			UpdateText();
		}
	}

	/// <summary>
	/// Gets or sets whether the text should break only at the end of a word.
	/// </summary>
	[Description("Break the text at the end of a word.")]
	[Category("Appearance")]
	[DefaultValue(false)]
	public bool WordBreak
	{
		get
		{
			return _wordBreak;
		}
		set
		{
			_wordBreak = value;
			UpdateText();
		}
	}

	/// <summary>
	/// Gets or sets whether the text should be trimmed to the last word
	/// and an ellipse should be placed at the end of the line.
	/// </summary>
	[Description("Trims the line to the nearest word and an ellipsis is placed at the end of a trimmed line.")]
	[Category("Appearance")]
	[DefaultValue(false)]
	public bool WordEllipsis
	{
		get
		{
			return _wordBreak;
		}
		set
		{
			_wordBreak = value;
			UpdateText();
		}
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		_themeText = new ThemedText();
		UpdateText();
		base.OnHandleCreated(e);
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		if (_themeText != null)
		{
			_themeText.Dispose();
		}
		_themeText = null;
		base.OnHandleDestroyed(e);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		Invalidate(invalidateChildren: false);
	}

	private void UpdateText()
	{
		if (_themeText != null)
		{
			_themeText.Text = Text;
			_themeText.Font = Font;
			_themeText.Padding = Padding;
			_themeText.Color = ForeColor;
			_themeText.FormatFlags = CreateFormatFlags();
		}
		Invalidate(invalidateChildren: false);
	}

	protected override void WndProc(ref Message m)
	{
		if (m.Msg == 132 && !base.DesignMode)
		{
			base.WndProc(ref m);
			m.Result = (IntPtr)(-1);
		}
		else
		{
			base.WndProc(ref m);
		}
	}

	protected TextFormatFlags CreateFormatFlags()
	{
		TextFormatFlags ret = TextFormatFlags.Default;
		switch (_horizontal)
		{
		case HorizontalAlignment.Left:
			ret |= TextFormatFlags.Default;
			break;
		case HorizontalAlignment.Center:
			ret |= TextFormatFlags.HorizontalCenter;
			break;
		case HorizontalAlignment.Right:
			ret |= TextFormatFlags.Right;
			break;
		}
		switch (_vertical)
		{
		case VerticalAlignment.Top:
			ret |= TextFormatFlags.Default;
			break;
		case VerticalAlignment.Center:
			ret |= TextFormatFlags.VerticalCenter;
			break;
		case VerticalAlignment.Bottom:
			ret |= TextFormatFlags.Bottom;
			break;
		}
		if (_singleLine)
		{
			ret |= TextFormatFlags.SingleLine;
		}
		if (_endEllipsis)
		{
			ret |= TextFormatFlags.EndEllipsis;
		}
		if (_wordBreak)
		{
			ret |= TextFormatFlags.WordBreak;
		}
		if (_wordEllipsis)
		{
			ret |= TextFormatFlags.WordEllipsis;
		}
		if (RightToLeft == RightToLeft.Yes)
		{
			ret |= TextFormatFlags.RightToLeft;
		}
		return ret;
	}

	private StringFormat CreateStringFormat()
	{
		StringFormat sf = new StringFormat();
		switch (_horizontal)
		{
		case HorizontalAlignment.Left:
			sf.Alignment = StringAlignment.Near;
			break;
		case HorizontalAlignment.Center:
			sf.Alignment = StringAlignment.Center;
			break;
		case HorizontalAlignment.Right:
			sf.Alignment = StringAlignment.Far;
			break;
		}
		switch (_vertical)
		{
		case VerticalAlignment.Top:
			sf.LineAlignment = StringAlignment.Near;
			break;
		case VerticalAlignment.Center:
			sf.LineAlignment = StringAlignment.Center;
			break;
		case VerticalAlignment.Bottom:
			sf.LineAlignment = StringAlignment.Far;
			break;
		}
		return sf;
	}

	protected override void OnInvalidated(InvalidateEventArgs e)
	{
		base.OnInvalidated(e);
		if (base.Parent != null)
		{
			base.Parent.Invalidate(base.ClientRectangle, invalidateChildren: false);
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (base.DesignMode)
		{
			e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new RectangleF(Padding.Left, Padding.Top, base.Size.Width - Padding.Horizontal, base.Size.Height - Padding.Vertical), CreateStringFormat());
		}
		else if (base.Visible)
		{
			_themeText.Draw(e.Graphics, 0, 0, base.Size.Width, base.Size.Height);
		}
	}
}
