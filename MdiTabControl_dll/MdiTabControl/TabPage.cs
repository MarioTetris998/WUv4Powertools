using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MdiTabControl;

[DesignerGenerated]
[DesignTimeVisible(false)]
[Description("Represents a single tab page in a MdiTabControl.TabControl.")]
public class TabPage : Control
{
	public delegate void ClickEventHandler(object sender, EventArgs e);

	internal delegate void CloseEventHandler(object sender, EventArgs e);

	internal delegate void GetTabRegionEventHandler(object sender, TabControl.GetTabRegionEventArgs e);

	internal delegate void TabPaintBackgroundEventHandler(object sender, TabControl.TabPaintEventArgs e);

	internal delegate void TabPaintBorderEventHandler(object sender, TabControl.TabPaintEventArgs e);

	internal delegate void DraggingEventHandler(object sender, MouseEventArgs e);

	internal delegate void EndDragEventHandler(object sender, MouseEventArgs e);

	private IContainer components;

	private Color m_BackHighColor;

	private Color m_BackHighColorDisabled;

	private Color m_BackLowColor;

	private Color m_BackLowColorDisabled;

	private Color m_BorderColor;

	private Color m_BorderColorDisabled;

	private Color m_ForeColorDisabled;

	private bool m_Selected;

	private bool m_Hot;

	private int m_MaximumWidth;

	private int m_MinimumWidth;

	private int m_PadLeft;

	private int m_PadRight;

	private bool m_CloseButtonVisible;

	private Image m_CloseButton;

	private Image m_CloseButtonImageHot;

	private Image m_CloseButtonImageDisabled;

	private Color m_CloseButtonBackHighColor;

	private Color m_CloseButtonBackLowColor;

	private Color m_CloseButtonBorderColor;

	private Color m_CloseButtonForeColor;

	private Color m_CloseButtonBackHighColorDisabled;

	private Color m_CloseButtonBackLowColorDisabled;

	private Color m_CloseButtonBorderColorDisabled;

	private Color m_CloseButtonForeColorDisabled;

	private Color m_CloseButtonBackHighColorHot;

	private Color m_CloseButtonBackLowColorHot;

	private Color m_CloseButtonBorderColorHot;

	private Color m_CloseButtonForeColorHot;

	private bool m_HotTrack;

	private Size m_CloseButtonSize;

	private bool m_FontBoldOnSelect;

	private Size m_IconSize;

	private SmoothingMode m_SmoothingMode;

	private TabControl.TabAlignment m_Alignment;

	private bool m_GlassGradient;

	private bool m_BorderEnhanced;

	private ToolStripRenderMode m_RenderMode;

	private TabControl.Weight m_BorderEnhanceWeight;

	[CompilerGenerated]
	[AccessedThroughProperty("m_Form")]
	private Form _m_Form;

	internal bool TabVisible;

	internal int TabLeft;

	internal ToolStripMenuItem MenuItem;

	private bool MouseOverCloseButton;

	[SpecialName]
	private bool _0024STATIC_0024Tab_MouseMove_002420211C12815_0024State;

	[SpecialName]
	private StaticLocalInitFlag _0024STATIC_0024Tab_MouseMove_002420211C12815_0024State_0024Init;

	internal virtual Form m_Form
	{
		[CompilerGenerated]
		get
		{
			return _m_Form;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = TabContents_Enter;
			FormClosedEventHandler value3 = TabContent_FormClosed;
			EventHandler value4 = m_Form_Leave;
			EventHandler value5 = TabContent_TextChanged;
			Form form = _m_Form;
			if (form != null)
			{
				form.Enter -= value2;
				form.FormClosed -= value3;
				form.Leave -= value4;
				form.TextChanged -= value5;
			}
			_m_Form = value;
			form = _m_Form;
			if (form != null)
			{
				form.Enter += value2;
				form.FormClosed += value3;
				form.Leave += value4;
				form.TextChanged += value5;
			}
		}
	}

	[Description("Gets the form associated with the tab page")]
	public object Form => m_Form;

	[Description("Gets or sets the System.Drawing.Color structure that represents the starting color of the Background linear gradient for the tab.")]
	public Color BackHighColor
	{
		get
		{
			return m_BackHighColor;
		}
		set
		{
			m_BackHighColor = value;
			Invalidate();
		}
	}

	[Description("Gets or sets the System.Drawing.Color structure that represents the ending color of the Background linear gradient for the tab.")]
	public Color BackLowColor
	{
		get
		{
			return m_BackLowColor;
		}
		set
		{
			m_BackLowColor = value;
			Invalidate();
		}
	}

	[Description("Gets or sets the System.Drawing.Color structure that represents the border color.")]
	internal Color BorderColor
	{
		get
		{
			return m_BorderColor;
		}
		set
		{
			m_BorderColor = value;
			Invalidate();
		}
	}

	[Description("Gets or sets the System.Drawing.Color structure that represents the starting color of the Background linear gradient for a non selected tab.")]
	public Color BackHighColorDisabled
	{
		get
		{
			return m_BackHighColorDisabled;
		}
		set
		{
			m_BackHighColorDisabled = value;
			Invalidate();
		}
	}

	[Description("Gets or sets the System.Drawing.Color structure that represents the ending color of the Background linear gradient for a non selected tab.")]
	public Color BackLowColorDisabled
	{
		get
		{
			return m_BackLowColorDisabled;
		}
		set
		{
			m_BackLowColorDisabled = value;
			Invalidate();
		}
	}

	[Description("Gets or sets the System.Drawing.Color structure that represents the border color of the tab when not selected.")]
	public Color BorderColorDisabled
	{
		get
		{
			return m_BorderColorDisabled;
		}
		set
		{
			m_BorderColorDisabled = value;
			Invalidate();
		}
	}

	[Description("Gets or sets the System.Drawing.Color structure that represents the fore color of the tab when not selected.")]
	public Color ForeColorDisabled
	{
		get
		{
			return m_ForeColorDisabled;
		}
		set
		{
			m_ForeColorDisabled = value;
			Invalidate();
		}
	}

	internal bool IsSelected
	{
		get
		{
			return m_Selected;
		}
		set
		{
			if (m_Selected != value)
			{
				m_Selected = value;
				if (m_Selected)
				{
					m_Hot = false;
				}
				Invalidate();
			}
		}
	}

	[Description("Returns whether the tab is selected or not.")]
	public bool Selected => IsSelected;

	internal int MaximumWidth
	{
		get
		{
			return m_MaximumWidth;
		}
		set
		{
			m_MaximumWidth = value;
			CalculateWidth();
			Invalidate();
		}
	}

	public int MinimumWidth
	{
		get
		{
			return m_MinimumWidth;
		}
		set
		{
			m_MinimumWidth = value;
			CalculateWidth();
			Invalidate();
		}
	}

	public int PadLeft
	{
		get
		{
			return m_PadLeft;
		}
		set
		{
			m_PadLeft = value;
			CalculateWidth();
			Invalidate();
		}
	}

	internal int PadRight
	{
		get
		{
			return m_PadRight;
		}
		set
		{
			m_PadRight = value;
			CalculateWidth();
			Invalidate();
		}
	}

	[Description("Gets or sets whether the tab close button is visble or not.")]
	public bool CloseButtonVisible
	{
		get
		{
			return m_CloseButtonVisible;
		}
		set
		{
			if (m_CloseButtonVisible != value)
			{
				m_CloseButtonVisible = value;
				CalculateWidth();
				Invalidate();
			}
		}
	}

	public Image CloseButtonImage
	{
		get
		{
			return m_CloseButton;
		}
		set
		{
			m_CloseButton = value;
			Invalidate();
		}
	}

	public Image CloseButtonImageHot
	{
		get
		{
			return m_CloseButtonImageHot;
		}
		set
		{
			m_CloseButtonImageHot = value;
			Invalidate();
		}
	}

	public Image CloseButtonImageDisabled
	{
		get
		{
			return m_CloseButtonImageDisabled;
		}
		set
		{
			m_CloseButtonImageDisabled = value;
			Invalidate();
		}
	}

	public Color CloseButtonBackHighColor
	{
		get
		{
			return m_CloseButtonBackHighColor;
		}
		set
		{
			m_CloseButtonBackHighColor = value;
		}
	}

	public Color CloseButtonBackLowColor
	{
		get
		{
			return m_CloseButtonBackLowColor;
		}
		set
		{
			m_CloseButtonBackLowColor = value;
		}
	}

	public Color CloseButtonBorderColor
	{
		get
		{
			return m_CloseButtonBorderColor;
		}
		set
		{
			m_CloseButtonBorderColor = value;
		}
	}

	public Color CloseButtonForeColor
	{
		get
		{
			return m_CloseButtonForeColor;
		}
		set
		{
			m_CloseButtonForeColor = value;
		}
	}

	public Color CloseButtonBackHighColorDisabled
	{
		get
		{
			return m_CloseButtonBackHighColorDisabled;
		}
		set
		{
			m_CloseButtonBackHighColorDisabled = value;
		}
	}

	public Color CloseButtonBackLowColorDisabled
	{
		get
		{
			return m_CloseButtonBackLowColorDisabled;
		}
		set
		{
			m_CloseButtonBackLowColorDisabled = value;
		}
	}

	public Color CloseButtonBorderColorDisabled
	{
		get
		{
			return m_CloseButtonBorderColorDisabled;
		}
		set
		{
			m_CloseButtonBorderColorDisabled = value;
		}
	}

	public Color CloseButtonForeColorDisabled
	{
		get
		{
			return m_CloseButtonForeColorDisabled;
		}
		set
		{
			m_CloseButtonForeColorDisabled = value;
		}
	}

	public Color CloseButtonBackHighColorHot
	{
		get
		{
			return m_CloseButtonBackHighColorHot;
		}
		set
		{
			m_CloseButtonBackHighColorHot = value;
		}
	}

	public Color CloseButtonBackLowColorHot
	{
		get
		{
			return m_CloseButtonBackLowColorHot;
		}
		set
		{
			m_CloseButtonBackLowColorHot = value;
		}
	}

	public Color CloseButtonBorderColorHot
	{
		get
		{
			return m_CloseButtonBorderColorHot;
		}
		set
		{
			m_CloseButtonBorderColorHot = value;
		}
	}

	public Color CloseButtonForeColorHot
	{
		get
		{
			return m_CloseButtonForeColorHot;
		}
		set
		{
			m_CloseButtonForeColorHot = value;
		}
	}

	internal bool HotTrack
	{
		get
		{
			return m_HotTrack;
		}
		set
		{
			m_HotTrack = value;
			Invalidate();
		}
	}

	internal Size CloseButtonSize
	{
		get
		{
			return m_CloseButtonSize;
		}
		set
		{
			m_CloseButtonSize = value;
			CalculateWidth();
			Invalidate();
		}
	}

	internal bool FontBoldOnSelect
	{
		get
		{
			return m_FontBoldOnSelect;
		}
		set
		{
			m_FontBoldOnSelect = value;
			CalculateWidth();
			Invalidate();
		}
	}

	internal Size IconSize
	{
		get
		{
			return m_IconSize;
		}
		set
		{
			m_IconSize = value;
			CalculateWidth();
			Invalidate();
		}
	}

	internal SmoothingMode SmoothingMode
	{
		get
		{
			return m_SmoothingMode;
		}
		set
		{
			m_SmoothingMode = value;
			Invalidate();
		}
	}

	internal TabControl.TabAlignment Alignment
	{
		get
		{
			return m_Alignment;
		}
		set
		{
			m_Alignment = value;
			Invalidate();
		}
	}

	internal bool GlassGradient
	{
		get
		{
			return m_GlassGradient;
		}
		set
		{
			m_GlassGradient = value;
		}
	}

	internal bool BorderEnhanced
	{
		get
		{
			return m_BorderEnhanced;
		}
		set
		{
			m_BorderEnhanced = value;
		}
	}

	internal ToolStripRenderMode RenderMode
	{
		get
		{
			return m_RenderMode;
		}
		set
		{
			m_RenderMode = value;
			Invalidate();
		}
	}

	internal TabControl.Weight BorderEnhanceWeight
	{
		get
		{
			return m_BorderEnhanceWeight;
		}
		set
		{
			m_BorderEnhanceWeight = value;
		}
	}

	public Icon Icon
	{
		get
		{
			return m_Form.Icon;
		}
		set
		{
			m_Form.Icon = value;
			Region region = new Region(new Rectangle(PadLeft, checked((int)Math.Round((double)base.Height / 2.0 - (double)m_IconSize.Height / 2.0)), m_IconSize.Width, m_IconSize.Height));
			Invalidate(region);
			region.Dispose();
			region = null;
			MenuItem.Image = value.ToBitmap();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Size MinimumSize
	{
		get
		{
			Size result = default(Size);
			return result;
		}
		set
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Size MaximumSize
	{
		get
		{
			Size result = default(Size);
			return result;
		}
		set
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Padding Padding
	{
		get
		{
			Padding result = default(Padding);
			return result;
		}
		set
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Color BackColor
	{
		get
		{
			Color result = default(Color);
			return result;
		}
		set
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override DockStyle Dock
	{
		get
		{
			DockStyle result = default(DockStyle);
			return result;
		}
		set
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override AnchorStyles Anchor
	{
		get
		{
			AnchorStyles result = default(AnchorStyles);
			return result;
		}
		set
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override string Text
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	[Description("Occurs when the user clicks the Tab Control.")]
	public new event ClickEventHandler Click;

	internal event CloseEventHandler Close;

	internal event GetTabRegionEventHandler GetTabRegion;

	internal event TabPaintBackgroundEventHandler TabPaintBackground;

	internal event TabPaintBorderEventHandler TabPaintBorder;

	internal event DraggingEventHandler Dragging;

	internal event EndDragEventHandler EndDrag;

	internal event EventHandler EnterForm;

	internal event EventHandler LeaveForm;

	[DebuggerNonUserCode]
	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	[System.Diagnostics.DebuggerStepThrough]
	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
	}

	public TabPage(Form Form)
	{
		base.MouseDown += Tab_MouseDown;
		base.MouseEnter += Tab_MouseEnter;
		base.MouseLeave += Tab_MouseLeave;
		base.MouseMove += Tab_MouseMove;
		m_Selected = false;
		m_Hot = false;
		MenuItem = new ToolStripMenuItem();
		MouseOverCloseButton = false;
		InitializeComponent();
		SuspendLayout();
		SetStyle(ControlStyles.DoubleBuffer, value: true);
		SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
		SetStyle(ControlStyles.UserPaint, value: true);
		SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
		base.BackColor = Color.Transparent;
		base.Visible = false;
		base.Size = new Size(1, 1);
		Form.TopLevel = false;
		Form.MdiParent = null;
		Form.FormBorderStyle = FormBorderStyle.None;
		Form.Dock = DockStyle.Fill;
		m_Form = Form;
		MenuItem.Text = Form.Text;
		MenuItem.Image = Form.Icon.ToBitmap();
		MenuItem.Tag = this;
		ResumeLayout(performLayout: false);
	}

	[Description("Selects the TabPage.")]
	public new void Select()
	{
		if (!IsSelected)
		{
			Click?.Invoke(this, new EventArgs());
		}
	}

	private LinearGradientBrush CreateGradientBrush(Rectangle Rectangle, Color Color1, Color Color2)
	{
		if (m_GlassGradient)
		{
			return Helper.CreateGlassGradientBrush(Rectangle, Color1, Color2);
		}
		return new LinearGradientBrush(Rectangle, Color1, Color2, LinearGradientMode.Vertical);
	}

	private void TabContents_Enter(object sender, EventArgs e)
	{
		EnterForm?.Invoke(this, e);
	}

	private void TabContent_FormClosed(object sender, FormClosedEventArgs e)
	{
		Close?.Invoke(this, new EventArgs());
	}

	private void m_Form_Leave(object sender, EventArgs e)
	{
		LeaveForm?.Invoke(this, e);
	}

	private void TabContent_TextChanged(object sender, EventArgs e)
	{
		CalculateWidth();
		Invalidate();
		MenuItem.Text = m_Form.Text;
	}

	private void Tab_MouseDown(object sender, MouseEventArgs e)
	{
		if (!(m_Selected & !(MouseOverCloseButton & m_CloseButtonVisible)) && e.Button == MouseButtons.Left)
		{
			if (MouseOverCloseButton & m_CloseButtonVisible)
			{
				m_Form.Close();
			}
			else
			{
				Select();
			}
		}
	}

	private void Tab_MouseEnter(object sender, EventArgs e)
	{
		if (!m_Selected)
		{
			if (m_HotTrack)
			{
				m_Hot = true;
			}
			Invalidate();
		}
	}

	private void Tab_MouseLeave(object sender, EventArgs e)
	{
		MouseOverCloseButton = false;
		m_Hot = false;
		Invalidate();
	}

	private void Tab_MouseMove(object sender, MouseEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (_0024STATIC_0024Tab_MouseMove_002420211C12815_0024State_0024Init == null)
		{
			Interlocked.CompareExchange(ref _0024STATIC_0024Tab_MouseMove_002420211C12815_0024State_0024Init, new StaticLocalInitFlag(), null);
		}
		Monitor.Enter(_0024STATIC_0024Tab_MouseMove_002420211C12815_0024State_0024Init);
		try
		{
			if (_0024STATIC_0024Tab_MouseMove_002420211C12815_0024State_0024Init.State == 0)
			{
				_0024STATIC_0024Tab_MouseMove_002420211C12815_0024State_0024Init.State = 2;
				_0024STATIC_0024Tab_MouseMove_002420211C12815_0024State = false;
			}
			else if (_0024STATIC_0024Tab_MouseMove_002420211C12815_0024State_0024Init.State == 2)
			{
				throw new IncompleteInitialization();
			}
		}
		finally
		{
			_0024STATIC_0024Tab_MouseMove_002420211C12815_0024State_0024Init.State = 1;
			Monitor.Exit(_0024STATIC_0024Tab_MouseMove_002420211C12815_0024State_0024Init);
		}
		checked
		{
			if (m_CloseButtonVisible)
			{
				int num = base.Width - PadRight - m_CloseButtonSize.Width - 2;
				int num2 = (int)Math.Round((double)base.Height / 2.0 - (double)m_CloseButtonSize.Height / 2.0);
				MouseOverCloseButton = (e.X >= num) & (e.X <= num + m_CloseButtonSize.Width - 1) & (e.Y >= num2) & (e.Y <= num2 + m_CloseButtonSize.Height - 1);
				if ((_0024STATIC_0024Tab_MouseMove_002420211C12815_0024State != MouseOverCloseButton) & m_CloseButtonVisible)
				{
					_0024STATIC_0024Tab_MouseMove_002420211C12815_0024State = MouseOverCloseButton;
					Region region = new Region(new Rectangle(num, num2, m_CloseButtonSize.Width, m_CloseButtonSize.Height));
					Invalidate(region);
					region.Dispose();
					region = null;
				}
			}
			if (RectangleToScreen(base.ClientRectangle).Contains(PointToScreen(new Point(e.X, e.Y))))
			{
				Cursor = Cursors.Default;
				EndDrag?.Invoke(this, e);
			}
			else
			{
				Dragging?.Invoke(this, e);
				Cursor = Cursors.No;
			}
		}
	}

	private void DrawText(Graphics g)
	{
		Font font = new Font(Font, (FontStyle)Conversions.ToInteger(Interaction.IIf(m_Selected & m_FontBoldOnSelect, (object)FontStyle.Bold, (object)FontStyle.Regular)));
		object obj = Interaction.IIf(m_Selected | m_Hot, (object)ForeColor, (object)m_ForeColorDisabled);
		Brush brush = new SolidBrush((obj != null) ? ((Color)obj) : default(Color));
		RectangleF layoutRectangle = new RectangleF(Conversions.ToSingle(Operators.AddObject(Operators.AddObject((object)PadLeft, Interaction.IIf(m_Form.Icon == null, (object)0, (object)m_IconSize.Width)), (object)2)), 1f, Conversions.ToSingle(Operators.SubtractObject(Operators.SubtractObject(Operators.SubtractObject(Operators.SubtractObject((object)checked(base.Width - PadLeft), Interaction.IIf(m_Form.Icon == null, (object)0, (object)m_IconSize.Height)), (object)5), Interaction.IIf(m_CloseButtonVisible, (object)m_CloseButtonSize.Width, (object)0)), (object)PadRight)), DisplayRectangle.Height);
		StringFormat stringFormat = new StringFormat();
		stringFormat.FormatFlags = StringFormatFlags.NoWrap;
		stringFormat.LineAlignment = StringAlignment.Center;
		stringFormat.Trimming = StringTrimming.EllipsisCharacter;
		g.DrawString(m_Form.Text, font, brush, layoutRectangle, stringFormat);
		stringFormat.Dispose();
		brush.Dispose();
		font.Dispose();
		stringFormat = null;
		brush = null;
		font = null;
	}

	private void DrawIcon(Graphics g)
	{
		try
		{
			if (m_Form.Icon != null)
			{
				Rectangle targetRect = new Rectangle(PadLeft, checked((int)Math.Round((double)base.Height / 2.0 - (double)m_IconSize.Height / 2.0)), m_IconSize.Width, m_IconSize.Height);
				Icon icon = new Icon(m_Form.Icon, m_IconSize);
				g.DrawIcon(icon, targetRect);
				DestroyIcon(icon.Handle);
				icon.Dispose();
				icon = null;
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			ProjectData.ClearProjectError();
		}
	}

	[DllImport("user32.dll")]
	private static extern bool DestroyIcon(IntPtr handle);

	private void DrawCloseButton(Graphics g)
	{
		checked
		{
			try
			{
				int num = base.Width - (m_CloseButtonSize.Width + PadRight + 2);
				int num2 = (int)Math.Round((double)base.Height / 2.0 - (double)m_CloseButtonSize.Height / 2.0);
				Bitmap bitmap = (MouseOverCloseButton ? ((Bitmap)m_CloseButtonImageHot) : ((!m_Selected) ? ((Bitmap)m_CloseButtonImageDisabled) : ((Bitmap)m_CloseButton)));
				bool flag = false;
				if (bitmap == null)
				{
					bitmap = GetButton();
					flag = true;
				}
				Icon icon = Icon.FromHandle(bitmap.GetHicon());
				Rectangle targetRect = new Rectangle(num, num2, m_CloseButtonSize.Width, m_CloseButtonSize.Height);
				g.DrawIcon(icon, targetRect);
				if (flag)
				{
					bitmap.Dispose();
					bitmap = null;
				}
				DestroyIcon(icon.Handle);
				icon.Dispose();
				icon = null;
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				ProjectData.ClearProjectError();
			}
		}
	}

	private Bitmap GetButton()
	{
		Point[] points = new Point[14]
		{
			new Point(1, 0),
			new Point(3, 0),
			new Point(5, 2),
			new Point(7, 0),
			new Point(9, 0),
			new Point(6, 3),
			new Point(6, 4),
			new Point(9, 7),
			new Point(7, 7),
			new Point(5, 5),
			new Point(3, 7),
			new Point(1, 7),
			new Point(4, 4),
			new Point(4, 3)
		};
		GraphicsPath graphicsPath = new GraphicsPath();
		Matrix matrix = new Matrix();
		Point[] points2 = new Point[8]
		{
			new Point(0, 1),
			new Point(1, 0),
			new Point(15, 0),
			new Point(16, 1),
			new Point(16, 14),
			new Point(15, 15),
			new Point(1, 15),
			new Point(0, 14)
		};
		Color color;
		Color color2;
		Color color3;
		Color color4;
		if (MouseOverCloseButton)
		{
			color = Helper.RenderColors.TabCloseButtonBackHighColorHot(m_RenderMode, CloseButtonBackHighColorHot);
			color2 = Helper.RenderColors.TabCloseButtonBackLowColorHot(m_RenderMode, CloseButtonBackLowColorHot);
			color3 = Helper.RenderColors.TabCloseButtonBorderColorHot(m_RenderMode, CloseButtonBorderColorHot);
			color4 = Helper.RenderColors.TabCloseButtonForeColorHot(m_RenderMode, CloseButtonForeColorHot);
		}
		else if (m_Selected)
		{
			color = Helper.RenderColors.TabCloseButtonBackHighColor(m_RenderMode, CloseButtonBackHighColor);
			color2 = Helper.RenderColors.TabCloseButtonBackLowColor(m_RenderMode, CloseButtonBackLowColor);
			color3 = Helper.RenderColors.TabCloseButtonBorderColor(m_RenderMode, CloseButtonBorderColor);
			color4 = Helper.RenderColors.TabCloseButtonForeColor(m_RenderMode, CloseButtonForeColor);
		}
		else
		{
			color = Helper.RenderColors.TabCloseButtonBackHighColorDisabled(m_RenderMode, CloseButtonBackHighColorDisabled);
			color2 = Helper.RenderColors.TabCloseButtonBackLowColorDisabled(m_RenderMode, CloseButtonBackLowColorDisabled);
			color3 = Helper.RenderColors.TabCloseButtonBorderColorDisabled(m_RenderMode, CloseButtonBorderColorDisabled);
			color4 = Helper.RenderColors.TabCloseButtonForeColorDisabled(m_RenderMode, CloseButtonForeColorDisabled);
		}
		Bitmap bitmap = new Bitmap(17, 17);
		bitmap.MakeTransparent();
		Graphics graphics = Graphics.FromImage(bitmap);
		graphics.SmoothingMode = SmoothingMode.AntiAlias;
		LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, 15), color, color2);
		graphics.FillPolygon(brush, points2);
		Pen pen = new Pen(color3);
		graphics.DrawPolygon(pen, points2);
		graphics.SmoothingMode = SmoothingMode.Default;
		graphicsPath.AddPolygon(points);
		matrix.Translate(3f, 4f);
		graphicsPath.Transform(matrix);
		pen.Dispose();
		pen = new Pen(color4);
		graphics.DrawPolygon(pen, graphicsPath.PathPoints);
		SolidBrush solidBrush = new SolidBrush(color4);
		graphics.FillPolygon(solidBrush, graphicsPath.PathPoints);
		solidBrush.Dispose();
		pen.Dispose();
		graphicsPath.Dispose();
		graphics.Dispose();
		matrix.Dispose();
		return bitmap;
	}

	private void CalculateWidth()
	{
		Graphics graphics = CreateGraphics();
		int num = 0;
		int num2 = 0;
		int num3 = base.Width;
		if (m_Form.Icon != null)
		{
			num = m_IconSize.Width;
		}
		if (m_CloseButtonVisible)
		{
			num2 = m_CloseButtonSize.Width;
		}
		Font font = new Font(Font, (FontStyle)Conversions.ToInteger(Interaction.IIf(m_FontBoldOnSelect, (object)FontStyle.Bold, (object)FontStyle.Regular)));
		checked
		{
			num3 = (int)Math.Round((float)(PadLeft + num + 3) + graphics.MeasureString(m_Form.Text, font).Width + 3f + (float)num2 + (float)m_PadRight + 2f);
			font.Dispose();
			if (num3 < m_MinimumWidth + 1)
			{
				num3 = m_MinimumWidth + 1;
			}
			else if (num3 > m_MaximumWidth + 1)
			{
				num3 = m_MaximumWidth + 1;
			}
			if (num3 != base.Width)
			{
				base.Width = num3;
			}
			graphics.Dispose();
		}
	}

	private Point[] GetRegion(int W, int H, int H1)
	{
		checked
		{
			Point[] points = new Point[6]
			{
				new Point(0, H),
				new Point(0, 2),
				new Point(2, 0),
				new Point(W - 3, 0),
				new Point(W - 1, 2),
				new Point(W - 1, H)
			};
			TabControl.GetTabRegionEventArgs e = new TabControl.GetTabRegionEventArgs(points, W, H, IsSelected);
			GetTabRegion?.Invoke(this, e);
			Point[] array = e.Points;
			Array.Resize(ref array, e.Points.Length + 2);
			e.Points = array;
			Array.Copy(e.Points, 0, e.Points, 1, e.Points.Length - 1);
			e.Points[0] = new Point(e.Points[1].X, H1);
			e.Points[e.Points.Length - 1] = new Point(e.Points[e.Points.Length - 2].X, H1);
			return e.Points;
		}
	}

	private void MirrorPath(GraphicsPath GraphicPath)
	{
		Matrix matrix = new Matrix();
		matrix.Translate(0f, checked(base.Height - 1));
		matrix.Scale(1f, -1f);
		GraphicPath.Transform(matrix);
		matrix.Dispose();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (false)
		{
			return;
		}
		SuspendLayout();
		GraphicsPath graphicsPath = new GraphicsPath();
		int num = base.Width;
		CalculateWidth();
		if (num != base.Width)
		{
			graphicsPath.Dispose();
			return;
		}
		Color color;
		Color color2;
		Color color3;
		Color color4;
		if (m_Selected)
		{
			color = Helper.RenderColors.BorderColor(m_RenderMode, BorderColor);
			color2 = Helper.RenderColors.TabBackHighColor(m_RenderMode, BackHighColor);
			color3 = Helper.RenderColors.TabBackLowColor(m_RenderMode, BackLowColor);
			color4 = Helper.RenderColors.TabBackLowColor(m_RenderMode, BackLowColor);
		}
		else if (m_Hot)
		{
			color = Helper.RenderColors.BorderColor(m_RenderMode, BorderColor);
			color2 = Helper.RenderColors.TabBackHighColor(m_RenderMode, BackHighColor);
			color3 = Helper.RenderColors.TabBackLowColor(m_RenderMode, BackLowColor);
			color4 = Helper.RenderColors.BorderColor(m_RenderMode, BorderColor);
		}
		else
		{
			color = Helper.RenderColors.BorderColorDisabled(m_RenderMode, BorderColorDisabled);
			color2 = Helper.RenderColors.TabBackHighColorDisabled(m_RenderMode, BackHighColorDisabled);
			color3 = Helper.RenderColors.TabBackLowColorDisabled(m_RenderMode, BackLowColorDisabled);
			color4 = Helper.RenderColors.BorderColor(m_RenderMode, BorderColor);
		}
		e.Graphics.SmoothingMode = m_SmoothingMode;
		checked
		{
			graphicsPath.AddPolygon(GetRegion(base.Width - 1, base.Height - 1, Conversions.ToInteger(Interaction.IIf(IsSelected, (object)base.Height, (object)(base.Height - 1)))));
			if (m_Alignment == TabControl.TabAlignment.Bottom)
			{
				MirrorPath(graphicsPath);
				Color color5 = color2;
				color2 = color3;
				color3 = color5;
			}
			Region region = new Region(graphicsPath);
			Region region2 = new Region(graphicsPath);
			Region region3 = new Region(graphicsPath);
			Region region4 = new Region(graphicsPath);
			Matrix matrix = new Matrix();
			Matrix matrix2 = new Matrix();
			Matrix matrix3 = new Matrix();
			matrix.Translate(0f, -0.5f);
			matrix2.Translate(0f, 0.5f);
			matrix3.Translate(1f, 0f);
			region2.Transform(matrix);
			region3.Transform(matrix2);
			region4.Transform(matrix3);
			region.Union(region2);
			region.Union(region3);
			region.Union(region4);
			base.Region = region;
			RectangleF bounds = region.GetBounds(e.Graphics);
			Rectangle clipRect = new Rectangle(0, 0, (int)Math.Round(bounds.Width), (int)Math.Round(bounds.Height));
			TabControl.TabPaintEventArgs e2 = new TabControl.TabPaintEventArgs(e.Graphics, clipRect, m_Selected, m_Hot, graphicsPath, base.Width, base.Height);
			TabPaintBackground?.Invoke(this, e2);
			LinearGradientBrush linearGradientBrush = CreateGradientBrush(new Rectangle(0, 0, base.Width, base.Height), color2, color3);
			if (!e2.Handled)
			{
				e.Graphics.FillPath(linearGradientBrush, graphicsPath);
			}
			linearGradientBrush.Dispose();
			e2.Dispose();
			e2 = new TabControl.TabPaintEventArgs(e.Graphics, clipRect, m_Selected, m_Hot, graphicsPath, base.Width, base.Height);
			TabPaintBorder?.Invoke(this, e2);
			if (!e2.Handled)
			{
				if (m_BorderEnhanced)
				{
					object obj = Interaction.IIf(m_Alignment == TabControl.TabAlignment.Bottom, (object)color3, (object)color2);
					Color color6 = ((obj != null) ? ((Color)obj) : default(Color));
					Pen pen = new Pen(color6, (float)m_BorderEnhanceWeight);
					e.Graphics.DrawLines(pen, graphicsPath.PathPoints);
					pen.Dispose();
				}
				Pen pen2 = new Pen(color);
				e.Graphics.DrawLines(pen2, graphicsPath.PathPoints);
				pen2.Dispose();
			}
			e2.Dispose();
			e.Graphics.SmoothingMode = SmoothingMode.None;
			e.Graphics.DrawLine(new Pen(color4), graphicsPath.PathPoints[0], graphicsPath.PathPoints[graphicsPath.PointCount - 1]);
			e.Graphics.SmoothingMode = m_SmoothingMode;
			DrawIcon(e.Graphics);
			DrawText(e.Graphics);
			if (m_CloseButtonVisible)
			{
				DrawCloseButton(e.Graphics);
			}
			ResumeLayout();
			graphicsPath.Dispose();
			matrix.Dispose();
			matrix2.Dispose();
			matrix3.Dispose();
			region2.Dispose();
			region3.Dispose();
			region4.Dispose();
			region.Dispose();
			e2.Dispose();
		}
	}
}
