using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using WindowsFormsAero.Dwm;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

/// <summary>
/// Base form class that automatically sets its font according
/// to the Windows UX guidelines and supports some Aero properties.
/// </summary>
public class AeroForm : Form
{
	private UserPreferenceChangedEventHandler _preferencesHandler;

	private Padding _glassMargins = Padding.Empty;

	private bool _glassEnabled;

	private bool _glassFull;

	private bool _hideTitle;

	private bool _hideCaption;

	/// <summary>
	/// Gets or sets the glass margins of the form.
	/// If set to <see cref="F:System.Windows.Forms.Padding.Empty" /> the glass effect is disabled.
	/// </summary>
	/// <remarks>
	/// The <see cref="T:System.Windows.Forms.Padding" /> value contains the borders (in pixels) that are
	/// extended into the client area. If all padding values are negative, then the
	/// glass area extends to the whole client area.
	/// Client area that is marked as glass MUST have a full black background (this
	/// is handled automatically by <see cref="T:WindowsFormsAero.AeroForm" />. Controls rendered on TOP
	/// of the glass region must use GDI+ for correct alpha handling (default Win32
	/// and Windows Forms controls do not render correctly). Use the provided
	/// <see cref="T:WindowsFormsAero.ThemeLabel" /> to render text on top of glass.
	/// </remarks>
	[Description("The glass margins which are extended inside the client area of the window.")]
	[Category("Appearance")]
	public Padding GlassMargins
	{
		get
		{
			return _glassMargins;
		}
		set
		{
			_glassMargins = value;
			_glassEnabled = _glassMargins != Padding.Empty;
			_glassFull = _glassMargins.AllNegative();
			UpdateGlass();
		}
	}

	/// <summary>
	/// Gets or sets whether the window title and icon should be hidden.
	/// </summary>
	/// <remarks>
	/// The window caption will still be visible, but title text and icon will not.
	/// A form with a hidden title will look like an Explorer window on Windows Vista
	/// or Windows 7.
	/// </remarks>
	[Description("Shows or hides the title and icon of the window.")]
	[Category("Appearance")]
	[DefaultValue(false)]
	public bool HideTitle
	{
		get
		{
			return _hideTitle;
		}
		set
		{
			if (value != _hideTitle)
			{
				_hideTitle = value;
				if (Environment.OSVersion.Version.Major >= 6)
				{
					ApplyWindowTheme();
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets whether the window caption should be hidden altogether.
	/// </summary>
	/// <remarks>
	/// Should be set before handle creation.
	/// </remarks>
	[Description("Shows or hides the window caption completely.")]
	[Category("Appearance")]
	[DefaultValue(false)]
	public bool HideCaption
	{
		get
		{
			return _hideCaption;
		}
		set
		{
			if (value != _hideCaption)
			{
				_hideCaption = value;
				if (base.IsHandleCreated)
				{
					RecreateHandle();
				}
			}
		}
	}

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams parms = base.CreateParams;
			if (HideCaption)
			{
				parms.Style &= -12582913;
			}
			return parms;
		}
	}

	/// <summary>
	/// Gets or sets whether mouse dragging on glass should be handled automatically.
	/// </summary>
	[Description("True if mouse dragging of the window on glass should be handled automatically.")]
	[Category("Behavior")]
	[DefaultValue(true)]
	public bool HandleMouseOnGlass { get; set; } = true;

	public AeroForm()
	{
		Font = SystemFonts.MessageBoxFont;
		base.ResizeRedraw = true;
		_preferencesHandler = SystemEvents_UserPreferenceChanged;
		SystemEvents.UserPreferenceChanged += _preferencesHandler;
	}

	protected override void Dispose(bool disposing)
	{
		SystemEvents.UserPreferenceChanged -= _preferencesHandler;
		base.Dispose(disposing);
	}

	private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
	{
		Font = SystemFonts.MessageBoxFont;
	}

	private void UpdateGlass()
	{
		if (!base.DesignMode)
		{
			if (_glassEnabled)
			{
				DwmManager.EnableGlassFrame(this, _glassMargins);
			}
			else
			{
				DwmManager.DisableGlassFrame(this);
			}
			Invalidate();
		}
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		if (Environment.OSVersion.Version.Major >= 6)
		{
			ApplyWindowTheme();
		}
	}

	private void ApplyWindowTheme()
	{
		WindowThemeNonClientAttributes attr = (HideTitle ? (WindowThemeNonClientAttributes.NoDrawCaption | WindowThemeNonClientAttributes.NoDrawIcon) : WindowThemeNonClientAttributes.NullAttribute);
		WindowTheme.SetWindowThemeNonClientAttributes(base.Handle, WindowThemeNonClientAttributes.NoDrawCaption | WindowThemeNonClientAttributes.NoDrawIcon, attr);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (_glassEnabled)
		{
			if (_glassFull)
			{
				e.Graphics.Clear(Color.Black);
				return;
			}
			e.Graphics.FillRectangles(Brushes.Black, new Rectangle[4]
			{
				new Rectangle(0, 0, base.ClientSize.Width, _glassMargins.Top),
				new Rectangle(base.ClientSize.Width - _glassMargins.Right, 0, _glassMargins.Right, base.ClientSize.Height),
				new Rectangle(0, base.ClientSize.Height - _glassMargins.Bottom, base.ClientSize.Width, _glassMargins.Bottom),
				new Rectangle(0, 0, _glassMargins.Left, base.ClientSize.Height)
			});
		}
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
		if (!HandleMouseOnGlass || !_glassEnabled || m.Msg != 132 || m.Result.ToInt32() != 1)
		{
			return;
		}
		if (_glassFull)
		{
			m.Result = (IntPtr)2;
			return;
		}
		int val = m.LParam.ToInt32();
		ushort x = IntHelpers.LowWord((uint)val);
		ushort y = IntHelpers.HighWord((uint)val);
		System.Drawing.Point clientPoint = PointToClient(new System.Drawing.Point(x, y));
		if (_glassMargins.IsOutside(clientPoint, base.ClientSize))
		{
			m.Result = (IntPtr)2;
		}
	}
}
