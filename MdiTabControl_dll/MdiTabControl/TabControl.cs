using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MdiTabControl;

[DesignerGenerated]
[DesignTimeVisible(true)]
public class TabControl : UserControl
{
	[Description("Provides data for the MdiTabControl.TabControl.GetTabRegion event.")]
	public class GetTabRegionEventArgs : EventArgs
	{
		private Point[] m_Points;

		private int m_TabWidth;

		private int m_TabHeight;

		private bool m_Selected;

		[Description("Returns whether the tab is selected or not.")]
		public int Selected => 0 - (m_Selected ? 1 : 0);

		[Description("Returns the tab width.")]
		public int TabWidth => m_TabWidth;

		[Description("Returns the tab height.")]
		public int TabHeight => m_TabHeight;

		[Description("Gets or sets an array of System.Drawing.Point structures that represents the points through which the tab path is constructed.")]
		public Point[] Points
		{
			get
			{
				return m_Points;
			}
			set
			{
				m_Points = value;
			}
		}

		private GetTabRegionEventArgs()
		{
		}

		[Description("Initializes a new instance of the MdiTabControl.TabControl.GetTabRegionEventArgs class.")]
		public GetTabRegionEventArgs(Point[] Points, int Width, int Height, bool Selected)
		{
			m_Points = Points;
			m_TabWidth = Width;
			m_TabHeight = Height;
			m_Selected = Selected;
		}
	}

	[Description("Provides data for the MdiTabControl.TabControl.TabPaint event.")]
	public class TabPaintEventArgs : PaintEventArgs
	{
		private bool m_Handled;

		private bool m_Selected;

		private bool m_Hot;

		private GraphicsPath m_GraphicPath;

		private int m_TabWidth;

		private int m_TabHeight;

		[Description("Returns the tab's hot state.")]
		public bool Hot => m_Hot;

		[Description("Returns whether the tab is selected or not.")]
		public bool Selected => m_Selected;

		[Description("Gets or sets a value that indicates whether the event handler has completely handled the paint or whether the system should continue its own processing.")]
		public bool Handled
		{
			get
			{
				return m_Handled;
			}
			set
			{
				m_Handled = value;
			}
		}

		[Description("Returns the tab width.")]
		public int TabWidth => m_TabWidth;

		[Description("Returns the tab height.")]
		public int TabHeight => m_TabHeight;

		[Description("Represents a series of connected lines and curves which the tab path is constructed.")]
		public GraphicsPath GraphicPath => m_GraphicPath;

		[Description("Initializes a new instance of the MdiTabControl.TabControl.GetTabRegionEventArgs class.")]
		public TabPaintEventArgs(Graphics graphics, Rectangle clipRect, bool Selected, bool Hot, GraphicsPath GraphicPath, int Width, int Height)
			: base(graphics, clipRect)
		{
			m_Handled = false;
			m_Selected = false;
			m_Hot = false;
			m_Selected = Selected;
			m_Hot = Hot;
			m_GraphicPath = GraphicPath;
			m_TabWidth = Width;
			m_TabHeight = Height;
		}
	}

	[Description("Contains a collection of MdiTabControl.TabPage objects.")]
	public class TabPageCollection : CollectionBase
	{
		internal delegate void GetTabRegionEventHandler(object sender, GetTabRegionEventArgs e);

		internal delegate void TabPaintBackgroundEventHandler(object sender, TabPaintEventArgs e);

		internal delegate void TabPaintBorderEventHandler(object sender, TabPaintEventArgs e);

		private TabControl TabControl;

		private bool IsReorder;

		private TabPage CurrentTab;

		[Description("Gets a TabPage in the position Index from the collection.")]
		public TabPage this[int Index] => (TabPage)base.List[Index];

		[Description("Gets a TabPage associated with the Form from the collection.")]
		public TabPage this[Form Form]
		{
			get
			{
				int num = this.get_IndexOf(Form);
				if (num == -1)
				{
					return null;
				}
				return (TabPage)base.List[num];
			}
		}

		[Description("Returns the index of the specified TabPage in the collection.")]
		public int get_IndexOf(TabPage tabPage)
		{
			return base.List.IndexOf(tabPage);
		}

		[Description("Sets the index of the specified TabPage in the collection.")]
		public void set_IndexOf(TabPage tabPage, int value)
		{
			IsReorder = true;
			base.List.Remove(tabPage);
			base.List.Insert(value, tabPage);
			TabControl.ArrangeItems();
			IsReorder = false;
		}

		[Description("Returns the index of the specified TabPage associated with the Form in the collection.")]
		public int get_IndexOf(Form Form)
		{
			int result = -1;
			checked
			{
				int num = base.List.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					if (((TabPage)base.List[i]).m_Form.Equals(Form))
					{
						result = i;
						break;
					}
				}
				return result;
			}
		}

		internal event GetTabRegionEventHandler GetTabRegion;

		[Description("Occurs when the Tab Background has been painted.")]
		internal event TabPaintBackgroundEventHandler TabPaintBackground;

		[Description("Occurs when the Tab Border has been painted.")]
		internal event TabPaintBorderEventHandler TabPaintBorder;

		internal event EventHandler SelectedChanged;

		internal TabPageCollection(TabControl Owner)
		{
			IsReorder = false;
			CurrentTab = null;
			TabControl = Owner;
		}

		[Description("Create a new TabPage and adds it to the collection whit the Form associated and returns the created TabPage.")]
		public TabPage Add(Form Form)
		{
			TabPage tabPage = new TabPage(Form);
			tabPage.SuspendLayout();
			TabControl.SuspendLayout();
			TabControl.AddingPage = true;
			tabPage.BackHighColor = TabControl.TabBackHighColor;
			tabPage.BackHighColorDisabled = TabControl.TabBackHighColorDisabled;
			tabPage.BackLowColor = TabControl.TabBackLowColor;
			tabPage.BackLowColorDisabled = TabControl.TabBackLowColorDisabled;
			tabPage.BorderColor = TabControl.BorderColor;
			tabPage.BorderColorDisabled = TabControl.BorderColorDisabled;
			tabPage.ForeColor = TabControl.ForeColor;
			tabPage.ForeColorDisabled = TabControl.ForeColorDisabled;
			tabPage.MaximumWidth = TabControl.TabMaximumWidth;
			tabPage.MinimumWidth = TabControl.TabMinimumWidth;
			tabPage.PadLeft = TabControl.TabPadLeft;
			tabPage.PadRight = TabControl.TabPadRight;
			tabPage.CloseButtonVisible = TabControl.TabCloseButtonVisible;
			tabPage.CloseButtonImage = TabControl.TabCloseButtonImage;
			tabPage.CloseButtonImageHot = TabControl.TabCloseButtonImageHot;
			tabPage.CloseButtonImageDisabled = TabControl.TabCloseButtonImageDisabled;
			tabPage.CloseButtonSize = TabControl.TabCloseButtonSize;
			tabPage.CloseButtonBackHighColor = TabControl.TabCloseButtonBackHighColor;
			tabPage.CloseButtonBackLowColor = TabControl.TabCloseButtonBackLowColor;
			tabPage.CloseButtonBorderColor = TabControl.TabCloseButtonBorderColor;
			tabPage.CloseButtonForeColor = TabControl.TabCloseButtonForeColor;
			tabPage.CloseButtonBackHighColorDisabled = TabControl.TabCloseButtonBackHighColorDisabled;
			tabPage.CloseButtonBackLowColorDisabled = TabControl.TabCloseButtonBackLowColorDisabled;
			tabPage.CloseButtonBorderColorDisabled = TabControl.TabCloseButtonBorderColorDisabled;
			tabPage.CloseButtonForeColorDisabled = TabControl.TabCloseButtonForeColorDisabled;
			tabPage.CloseButtonBackHighColorHot = TabControl.TabCloseButtonBackHighColorHot;
			tabPage.CloseButtonBackLowColorHot = TabControl.TabCloseButtonBackLowColorHot;
			tabPage.CloseButtonBorderColorHot = TabControl.TabCloseButtonBorderColorHot;
			tabPage.CloseButtonForeColorHot = TabControl.TabCloseButtonForeColorHot;
			tabPage.HotTrack = TabControl.HotTrack;
			tabPage.Font = TabControl.Font;
			tabPage.FontBoldOnSelect = TabControl.FontBoldOnSelect;
			tabPage.IconSize = TabControl.TabIconSize;
			tabPage.SmoothingMode = TabControl.SmoothingMode;
			tabPage.Alignment = TabControl.Alignment;
			tabPage.GlassGradient = TabControl.TabGlassGradient;
			tabPage.BorderEnhanced = TabControl.m_TabBorderEnhanced;
			tabPage.RenderMode = TabControl.RenderMode;
			tabPage.BorderEnhanceWeight = TabControl.TabBorderEnhanceWeight;
			tabPage.Top = 0;
			tabPage.Left = TabControl.LeftOffset;
			tabPage.Height = TabControl.TabHeight;
			TabControl.TabToolTip.SetToolTip(tabPage, tabPage.m_Form.Text);
			tabPage.Click += TabPage_Clicked;
			tabPage.Close += TabPage_Closed;
			tabPage.GetTabRegion += TabPage_GetTabRegion;
			tabPage.TabPaintBackground += TabPage_TabPaintBackground;
			tabPage.TabPaintBorder += TabPage_TabPaintBorder;
			tabPage.SizeChanged += TabPage_SizeChanged;
			tabPage.Dragging += TabPage_Dragging;
			tabPage.EndDrag += TabPage_EndDrag;
			tabPage.EnterForm += TabPage_Enter;
			tabPage.LeaveForm += TabPage_Leave;
			base.List.Add(tabPage);
			TabControl.ResumeLayout();
			tabPage.ResumeLayout();
			return tabPage;
		}

		[Description("Removes a TabPage from the collection.")]
		public void Remove(TabPage TabPage)
		{
			try
			{
				TabControl.IsDelete = true;
				if (TabControl.pnlBottom.Controls.Count > 1)
				{
					TabControl.pnlBottom.Controls[1].Dock = DockStyle.Fill;
					TabControl.pnlBottom.Controls[1].Visible = true;
				}
				base.List.Remove(TabPage);
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				ProjectData.ClearProjectError();
			}
		}

		public Form TearOff(TabPage TabPage)
		{
			Form form = TabPage.m_Form;
			TabControl.pnlBottom.Controls.Remove(form);
			TabPage.m_Form = new Form();
			Remove(TabPage);
			form.TopLevel = true;
			form.Dock = DockStyle.None;
			form.FormBorderStyle = FormBorderStyle.Sizable;
			return form;
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, RuntimeHelpers.GetObjectValue(value));
			if (!IsReorder)
			{
				TabControl.pnlBottom.Controls.Add(((TabPage)value).m_Form);
				TabControl.pnlTabs.Controls.Add((TabPage)value);
				((TabPage)value).Select();
				TabControl.AddingPage = false;
				TabControl.ArrangeItems();
				TabControl.Background.Visible = false;
			}
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			base.OnRemoveComplete(index, RuntimeHelpers.GetObjectValue(value));
			if (!IsReorder)
			{
				if (base.List.Count == 0)
				{
					TabControl.Background.Visible = true;
				}
				TabControl.ArrangeItems();
				TabControl.pnlBottom.Controls.Remove(((TabPage)value).m_Form);
				((TabPage)value).m_Form.Dispose();
				TabControl.pnlTabs.Controls.Remove((TabPage)value);
				((TabPage)value).Dispose();
				TabControl.SelectItem(null);
			}
		}

		protected override void OnClear()
		{
			base.OnClear();
			TabControl.Background.Visible = true;
		}

		protected override void OnClearComplete()
		{
			base.OnClearComplete();
			TabControl.pnlBottom.Controls.Clear();
			TabControl.pnlTabs.Controls.Clear();
		}

		[Description("Returns the selected TabPage.")]
		public TabPage SelectedTab()
		{
			foreach (TabPage item in base.List)
			{
				if (item.IsSelected)
				{
					return item;
				}
			}
			return null;
		}

		[Description("Returns the index of the selected TabPage.")]
		public int SelectedIndex()
		{
			int result = default(int);
			foreach (TabPage item in base.List)
			{
				if (item.IsSelected)
				{
					result = base.List.IndexOf(item);
					return result;
				}
			}
			return result;
		}

		private void TabPage_Clicked(object sender, EventArgs e)
		{
			TabControl.SelectItem((TabPage)sender);
			SelectedChanged?.Invoke(RuntimeHelpers.GetObjectValue(sender), e);
		}

		private void TabPage_Closed(object sender, EventArgs e)
		{
			Remove((TabPage)sender);
		}

		private void TabPage_GetTabRegion(object sender, GetTabRegionEventArgs e)
		{
			GetTabRegion?.Invoke(RuntimeHelpers.GetObjectValue(sender), e);
		}

		private void TabPage_TabPaintBackground(object sender, TabPaintEventArgs e)
		{
			TabPaintBackground?.Invoke(RuntimeHelpers.GetObjectValue(sender), e);
		}

		private void TabPage_TabPaintBorder(object sender, TabPaintEventArgs e)
		{
			TabPaintBorder?.Invoke(RuntimeHelpers.GetObjectValue(sender), e);
		}

		private void TabPage_SizeChanged(object sender, EventArgs e)
		{
			TabControl.ArrangeItems();
		}

		private void TabPage_Dragging(object sender, MouseEventArgs e)
		{
			if (TabControl.AllowTabReorder && e.Button == MouseButtons.Left)
			{
				TabPage tabPage = GetTabPage((TabPage)sender, e.X, e.Y);
				if (tabPage == null)
				{
					CurrentTab = null;
				}
				else if (tabPage != CurrentTab)
				{
					this.set_IndexOf(tabPage, this.get_IndexOf((TabPage)sender));
					CurrentTab = tabPage;
				}
			}
		}

		private void TabPage_EndDrag(object sender, MouseEventArgs e)
		{
			CurrentTab = null;
		}

		private TabPage GetTabPage(TabPage TabPage, int x, int y)
		{
			checked
			{
				int num = base.List.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					if ((TabPage)base.List[i] != TabPage && ((TabPage)base.List[i]).TabVisible && ((TabPage)base.List[i]).RectangleToScreen(((TabPage)base.List[i]).ClientRectangle).Contains(TabPage.PointToScreen(new Point(x, y))))
					{
						return (TabPage)base.List[i];
					}
				}
				return null;
			}
		}

		public void TabPage_Enter(object sender, EventArgs e)
		{
			if (!TabControl.m_Focused)
			{
				TabControl.SetFocus = true;
			}
		}

		public void TabPage_Leave(object sender, EventArgs e)
		{
			if (TabControl.m_Focused)
			{
				TabControl.SetFocus = false;
			}
		}
	}

	[Description("Gets or sets the specified alignment for the control.")]
	public enum TabAlignment
	{
		Top,
		Bottom
	}

	[Description("Gets or sets the specified direction for the control.")]
	public enum FlowDirection
	{
		LeftToRight = 0,
		RightToLeft = 2
	}

	public enum Weight
	{
		Soft = 2,
		Medium,
		Strong,
		Strongest
	}

	public delegate void GetTabRegionEventHandler(object sender, GetTabRegionEventArgs e);

	public delegate void TabPaintBackgroundEventHandler(object sender, TabPaintEventArgs e);

	public delegate void TabPaintBorderEventHandler(object sender, TabPaintEventArgs e);

	public class KeyHandledEventArgs : HandledEventArgs
	{
		public int Index;

		public bool Shift;

		public KeyHandledEventArgs()
		{
			Shift = false;
		}
	}

	public delegate void TabPressedEventHandler(object sender, KeyHandledEventArgs e);

	public delegate void F4PressedEventHandler(object sender, KeyHandledEventArgs e);

	private IContainer components;

	[CompilerGenerated]
	[AccessedThroughProperty("pnlTop")]
	private Panel _pnlTop;

	[CompilerGenerated]
	[AccessedThroughProperty("DropButton")]
	private ControlButton _DropButton;

	[CompilerGenerated]
	[AccessedThroughProperty("CloseButton")]
	private ControlButton _CloseButton;

	private bool AddingPage;

	private int LeftOffset;

	private bool IsDelete;

	private Panel Background;

	[CompilerGenerated]
	[AccessedThroughProperty("Items")]
	private TabPageCollection _Items;

	private FlowDirection m_TabsDirection;

	private int m_TabMaximumWidth;

	private int m_tabMinimumWidth;

	private Color m_BackLowColor;

	private Color m_BackHighColor;

	private Color m_BorderColor;

	private Color m_TabBackHighColor;

	private Color m_TabBackLowColor;

	private Color m_TabBackHighColorDisabled;

	private Color m_TabBackLowColorDisabled;

	private Color m_BorderColorDisabled;

	private Color m_ForeColorDisabled;

	private bool m_TopSeparator;

	private int m_TabTop;

	private int m_TabHeight;

	private int m_TabOffset;

	private int m_TabPadLeft;

	private int m_TabPadRight;

	private object m_TabSmoothingMode;

	private Size m_TabIconSize;

	private TabAlignment m_Alignment;

	private bool m_FontBoldOnSelect;

	private bool m_HotTrack;

	private Size m_TabCloseButtonSize;

	private bool m_TabCloseButtonVisible;

	private Image m_TabCloseButtonImage;

	private Image m_TabCloseButtonImageHot;

	private Image m_TabCloseButtonImageDisabled;

	private Color m_TabCloseButtonBackHighColor;

	private Color m_TabCloseButtonBackLowColor;

	private Color m_TabCloseButtonBorderColor;

	private Color m_TabCloseButtonForeColor;

	private Color m_TabCloseButtonBackHighColorDisabled;

	private Color m_TabCloseButtonBackLowColorDisabled;

	private Color m_TabCloseButtonBorderColorDisabled;

	private Color m_TabCloseButtonForeColorDisabled;

	private Color m_TabCloseButtonBackHighColorHot;

	private Color m_TabCloseButtonBackLowColorHot;

	private Color m_TabCloseButtonBorderColorHot;

	private Color m_TabCloseButtonForeColorHot;

	private bool m_AllowTabReorder;

	private bool m_TabGlassGradient;

	private bool m_TabBorderEnhanced;

	private ToolStripRenderMode m_RenderMode;

	private ToolStripRenderer m_ContextMenuRenderer;

	private Weight m_TabBorderEnhanceWeight;

	private bool m_Focused;

	internal readonly Padding defaultPadding;

	internal readonly Color defaultBackLowColor;

	internal readonly Color defaultBackHighColor;

	internal readonly Color defaultBorderColor;

	internal readonly Color defaultTabBackHighColor;

	internal readonly Color defaultTabBackLowColor;

	internal readonly Color defaultTabBackHighColorDisabled;

	internal readonly Color defaultTabBackLowColorDisabled;

	internal readonly Color defaultBorderColorDisabled;

	internal readonly Color defaultForeColorDisabled;

	internal readonly Color defaultControlButtonBackHighColor;

	internal readonly Color defaultControlButtonBackLowColor;

	internal readonly Color defaultControlButtonBorderColor;

	internal readonly Color defaultControlButtonForeColor;

	internal readonly Size defaultTabCloseButtonSize;

	internal readonly Size defaultTabIconSize;

	internal readonly Color defaultTabCloseButtonBackHighColor;

	internal readonly Color defaultTabCloseButtonBackHighColorDisabled;

	internal readonly Color defaultTabCloseButtonBackHighColorHot;

	internal readonly Color defaultTabCloseButtonBackLowColor;

	internal readonly Color defaultTabCloseButtonBackLowColorDisabled;

	internal readonly Color defaultTabCloseButtonBackLowColorHot;

	internal readonly Color defaultTabCloseButtonBorderColor;

	internal readonly Color defaultTabCloseButtonBorderColorDisabled;

	internal readonly Color defaultTabCloseButtonBorderColorHot;

	internal readonly Color defaultTabCloseButtonForeColor;

	internal readonly Color defaultTabCloseButtonForeColorDisabled;

	internal readonly Color defaultTabCloseButtonForeColorHot;

	internal readonly ToolStripRenderMode defaultRenderMode;

	private bool m_KeyCloseEnabled;

	private bool m_KeyTabEnabled;

	internal virtual Panel pnlTop
	{
		[CompilerGenerated]
		get
		{
			return _pnlTop;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = pnlTop_SizeChanged;
			PaintEventHandler value3 = pnlTop_Paint;
			Panel panel = _pnlTop;
			if (panel != null)
			{
				panel.SizeChanged -= value2;
				panel.Paint -= value3;
			}
			_pnlTop = value;
			panel = _pnlTop;
			if (panel != null)
			{
				panel.SizeChanged += value2;
				panel.Paint += value3;
			}
		}
	}

	[field: AccessedThroughProperty("pnlTabs")]
	internal virtual Panel pnlTabs
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("pnlBottom")]
	internal virtual Panel pnlBottom
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("WinMenu")]
	internal virtual ContextMenuStrip WinMenu
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual ControlButton DropButton
	{
		[CompilerGenerated]
		get
		{
			return _DropButton;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			MouseEventHandler value2 = DropButton_MouseDown;
			ControlButton controlButton = _DropButton;
			if (controlButton != null)
			{
				controlButton.MouseDown -= value2;
			}
			_DropButton = value;
			controlButton = _DropButton;
			if (controlButton != null)
			{
				controlButton.MouseDown += value2;
			}
		}
	}

	[field: AccessedThroughProperty("TabToolTip")]
	internal virtual ToolTip TabToolTip
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual ControlButton CloseButton
	{
		[CompilerGenerated]
		get
		{
			return _CloseButton;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			MouseEventHandler value2 = CloseButton_MouseDown;
			ControlButton controlButton = _CloseButton;
			if (controlButton != null)
			{
				controlButton.MouseDown -= value2;
			}
			_CloseButton = value;
			controlButton = _CloseButton;
			if (controlButton != null)
			{
				controlButton.MouseDown += value2;
			}
		}
	}

	[field: AccessedThroughProperty("pnlControls")]
	internal virtual Panel pnlControls
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual TabPageCollection Items
	{
		[CompilerGenerated]
		get
		{
			return _Items;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			TabPageCollection.GetTabRegionEventHandler obj = Items_GetTabRegion;
			EventHandler obj2 = Items_SelectedChanged;
			TabPageCollection.TabPaintBackgroundEventHandler obj3 = Items_TabPaintBackground;
			TabPageCollection.TabPaintBorderEventHandler obj4 = Items_TabPaintBorder;
			TabPageCollection tabPageCollection = _Items;
			if (tabPageCollection != null)
			{
				tabPageCollection.GetTabRegion -= obj;
				tabPageCollection.SelectedChanged -= obj2;
				tabPageCollection.TabPaintBackground -= obj3;
				tabPageCollection.TabPaintBorder -= obj4;
			}
			_Items = value;
			tabPageCollection = _Items;
			if (tabPageCollection != null)
			{
				tabPageCollection.GetTabRegion += obj;
				tabPageCollection.SelectedChanged += obj2;
				tabPageCollection.TabPaintBackground += obj3;
				tabPageCollection.TabPaintBorder += obj4;
			}
		}
	}

	[Browsable(false)]
	public override bool Focused => m_Focused;

	internal bool SetFocus
	{
		set
		{
			m_Focused = value;
			FocusedChanged?.Invoke(this, new EventArgs());
		}
	}

	[Browsable(false)]
	public object SelectedForm
	{
		get
		{
			if (pnlBottom.Controls.Count > 0)
			{
				return pnlBottom.Controls[0];
			}
			return null;
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[DefaultValue(0)]
	[Description("Gets or sets the the direction which the tabs are drawn.")]
	public FlowDirection TabsDirection
	{
		get
		{
			return m_TabsDirection;
		}
		set
		{
			m_TabsDirection = value;
			SelectItem(null);
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[DefaultValue(false)]
	[Description("Gets or sets if the tab background will paint with glass style.")]
	public bool TabGlassGradient
	{
		get
		{
			return m_TabGlassGradient;
		}
		set
		{
			m_TabGlassGradient = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.GlassGradient = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[DefaultValue(false)]
	[Description("Gets or sets if the tab border will paint with enhanced style.")]
	public bool TabBorderEnhanced
	{
		get
		{
			return m_TabBorderEnhanced;
		}
		set
		{
			m_TabBorderEnhanced = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.BorderEnhanced = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the starting color of the Background linear gradient for the tab close button.")]
	public Color TabCloseButtonBackHighColor
	{
		get
		{
			return m_TabCloseButtonBackHighColor;
		}
		set
		{
			m_TabCloseButtonBackHighColor = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the ending color of the Background linear gradient for the tab close button.")]
	public Color TabCloseButtonBackLowColor
	{
		get
		{
			return m_TabCloseButtonBackLowColor;
		}
		set
		{
			m_TabCloseButtonBackLowColor = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the border color for the tab close button.")]
	public Color TabCloseButtonBorderColor
	{
		get
		{
			return m_TabCloseButtonBorderColor;
		}
		set
		{
			m_TabCloseButtonBorderColor = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the fore color for the tab close button.")]
	public Color TabCloseButtonForeColor
	{
		get
		{
			return m_TabCloseButtonForeColor;
		}
		set
		{
			m_TabCloseButtonForeColor = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the starting color of the Background linear gradient for the disabled tab close button.")]
	public Color TabCloseButtonBackHighColorDisabled
	{
		get
		{
			return m_TabCloseButtonBackHighColorDisabled;
		}
		set
		{
			m_TabCloseButtonBackHighColorDisabled = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the ending color of the Background linear gradient for the disabled tab close button.")]
	public Color TabCloseButtonBackLowColorDisabled
	{
		get
		{
			return m_TabCloseButtonBackLowColorDisabled;
		}
		set
		{
			m_TabCloseButtonBackLowColorDisabled = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the border color for the disabled tab close button.")]
	public Color TabCloseButtonBorderColorDisabled
	{
		get
		{
			return m_TabCloseButtonBorderColorDisabled;
		}
		set
		{
			m_TabCloseButtonBorderColorDisabled = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the disabled fore color for the tab close button.")]
	public Color TabCloseButtonForeColorDisabled
	{
		get
		{
			return m_TabCloseButtonForeColorDisabled;
		}
		set
		{
			m_TabCloseButtonForeColorDisabled = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the starting color of the Background linear gradient for the Hot tab close button.")]
	public Color TabCloseButtonBackHighColorHot
	{
		get
		{
			return m_TabCloseButtonBackHighColorHot;
		}
		set
		{
			m_TabCloseButtonBackHighColorHot = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the ending color of the Background linear gradient for the Hot tab close button.")]
	public Color TabCloseButtonBackLowColorHot
	{
		get
		{
			return m_TabCloseButtonBackLowColorHot;
		}
		set
		{
			m_TabCloseButtonBackLowColorHot = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the border color for the Hot tab close button.")]
	public Color TabCloseButtonBorderColorHot
	{
		get
		{
			return m_TabCloseButtonBorderColorHot;
		}
		set
		{
			m_TabCloseButtonBorderColorHot = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the Hot fore color for the tab close button.")]
	public Color TabCloseButtonForeColorHot
	{
		get
		{
			return m_TabCloseButtonForeColorHot;
		}
		set
		{
			m_TabCloseButtonForeColorHot = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the tab close button image.")]
	public Image TabCloseButtonImage
	{
		get
		{
			return m_TabCloseButtonImage;
		}
		set
		{
			m_TabCloseButtonImage = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.CloseButtonImage = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the tab close button image in hot state.")]
	public Image TabCloseButtonImageHot
	{
		get
		{
			return m_TabCloseButtonImageHot;
		}
		set
		{
			m_TabCloseButtonImageHot = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.CloseButtonImageHot = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the tab close button image in disabled state.")]
	public Image TabCloseButtonImageDisabled
	{
		get
		{
			return m_TabCloseButtonImageDisabled;
		}
		set
		{
			m_TabCloseButtonImageDisabled = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.CloseButtonImageDisabled = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[DefaultValue(true)]
	[Description("Gets or sets whether the tab close button is visble or not.")]
	public bool TabCloseButtonVisible
	{
		get
		{
			return m_TabCloseButtonVisible;
		}
		set
		{
			m_TabCloseButtonVisible = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.CloseButtonVisible = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the size of the icon displayed at the tab.")]
	public Size TabIconSize
	{
		get
		{
			return m_TabIconSize;
		}
		set
		{
			m_TabIconSize = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.IconSize = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the size of the close button displayed at the tab.")]
	public Size TabCloseButtonSize
	{
		get
		{
			return m_TabCloseButtonSize;
		}
		set
		{
			m_TabCloseButtonSize = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.CloseButtonSize = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[DefaultValue(3)]
	[Description("Specifies whether smoothing (antialiasing) is applied to lines and curves and the edges of filled areas.")]
	public SmoothingMode SmoothingMode
	{
		get
		{
			return (SmoothingMode)Conversions.ToInteger(m_TabSmoothingMode);
		}
		set
		{
			m_TabSmoothingMode = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.SmoothingMode = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[DefaultValue(5)]
	[Description("Gets or sets the amount of space on the right side of the tab.")]
	public int TabPadRight
	{
		get
		{
			return m_TabPadRight;
		}
		set
		{
			m_TabPadRight = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.PadRight = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[DefaultValue(5)]
	[Description("Gets or sets the amount of space on the left side of the tab.")]
	public int TabPadLeft
	{
		get
		{
			return m_TabPadLeft;
		}
		set
		{
			m_TabPadLeft = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.PadLeft = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[DefaultValue(3)]
	[Description("Gets or sets the amount of space between the tabs.")]
	public int TabOffset
	{
		get
		{
			return m_TabOffset;
		}
		set
		{
			m_TabOffset = value;
			ArrangeItems();
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[DefaultValue(28)]
	[Description("Gets or sets the height of the tab.")]
	public int TabHeight
	{
		get
		{
			return m_TabHeight;
		}
		set
		{
			if (m_TabHeight == value)
			{
				return;
			}
			m_TabHeight = value;
			pnlTabs.Height = m_TabHeight;
			pnlTabs.Top = checked(pnlTop.Height - pnlTabs.Height);
			AdjustHeight();
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.Height = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[DefaultValue(3)]
	[Description("Gets or sets the distance between the top of the control and the top of the tab.")]
	public int TabTop
	{
		get
		{
			return m_TabTop;
		}
		set
		{
			m_TabTop = value;
			AdjustHeight();
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the starting color of the Background linear gradient.")]
	public Color TabBackHighColor
	{
		get
		{
			return m_TabBackHighColor;
		}
		set
		{
			m_TabBackHighColor = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.BackHighColor = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the ending color of the Background linear gradient.")]
	public Color TabBackLowColor
	{
		get
		{
			return m_TabBackLowColor;
		}
		set
		{
			m_TabBackLowColor = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.BackLowColor = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the starting color of the Background linear gradient for a non selected tab.")]
	public Color TabBackHighColorDisabled
	{
		get
		{
			return m_TabBackHighColorDisabled;
		}
		set
		{
			m_TabBackHighColorDisabled = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.BackHighColorDisabled = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the ending color of the Background linear gradient for a non selected tab.")]
	public Color TabBackLowColorDisabled
	{
		get
		{
			return m_TabBackLowColorDisabled;
		}
		set
		{
			m_TabBackLowColorDisabled = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.BackLowColorDisabled = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
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
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.BorderColorDisabled = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
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
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.ForeColorDisabled = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[DefaultValue(100)]
	[Description("Gets or sets the minimum width for the tab.")]
	public int TabMinimumWidth
	{
		get
		{
			return m_tabMinimumWidth;
		}
		set
		{
			m_tabMinimumWidth = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.MinimumWidth = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[DefaultValue(200)]
	[Description("Gets or sets the maximum width for the tab.")]
	public int TabMaximumWidth
	{
		get
		{
			return m_TabMaximumWidth;
		}
		set
		{
			m_TabMaximumWidth = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.MaximumWidth = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[DefaultValue(true)]
	[Description("Gets or sets whether the font on the selected tab is displayed in bold.")]
	public bool FontBoldOnSelect
	{
		get
		{
			return m_FontBoldOnSelect;
		}
		set
		{
			m_FontBoldOnSelect = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.FontBoldOnSelect = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Behavior")]
	[DefaultValue(true)]
	[Description("Gets or sets a value indicating whether the control's tabs change in appearance when the mouse passes over them.")]
	public bool HotTrack
	{
		get
		{
			return m_HotTrack;
		}
		set
		{
			m_HotTrack = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.HotTrack = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Behavior")]
	[DefaultValue(true)]
	[Description("Gets or sets a value indicating whether the user can reorder tabs draging.")]
	public bool AllowTabReorder
	{
		get
		{
			return m_AllowTabReorder;
		}
		set
		{
			m_AllowTabReorder = value;
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[DefaultValue(false)]
	[Description("Gets or sets a value indicating whether the close button is displayed or not.")]
	public bool CloseButtonVisible
	{
		get
		{
			return CloseButton.Visible;
		}
		set
		{
			if (CloseButton.Visible != value)
			{
				CloseButton.Visible = value;
				SetControlsSizeLocation();
			}
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[DefaultValue(true)]
	[Description("Gets or sets a value indicating whether the drop button is displayed or not.")]
	public bool DropButtonVisible
	{
		get
		{
			return DropButton.Visible;
		}
		set
		{
			if (DropButton.Visible != value)
			{
				DropButton.Visible = value;
				SetControlsSizeLocation();
			}
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[DefaultValue(true)]
	[Description("Gets or sets a value indicating whether a double line separator is displayed at the top of the control.")]
	public bool TopSeparator
	{
		get
		{
			return m_TopSeparator;
		}
		set
		{
			m_TopSeparator = value;
			AdjustHeight();
		}
	}

	[Browsable(true)]
	[Category("Behavior")]
	[DefaultValue(0)]
	[Description("Gets or sets the area of the control (for example, along the top) where the tabs are aligned.")]
	public TabAlignment Alignment
	{
		get
		{
			return m_Alignment;
		}
		set
		{
			m_Alignment = value;
			AdjustHeight();
			PositionButtons();
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.Alignment = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Layout")]
	[Description("Gets or sets the amount of space around the form on the control's tab pages.")]
	public new Padding Padding
	{
		get
		{
			return pnlBottom.Padding;
		}
		set
		{
			pnlBottom.Padding = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the starting color of the Background linear gradient for the control button.")]
	public Color ControlButtonBackHighColor
	{
		get
		{
			return DropButton.BackHighColor;
		}
		set
		{
			DropButton.BackHighColor = value;
			CloseButton.BackHighColor = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the ending color of the Background linear gradient for the control button.")]
	public Color ControlButtonBackLowColor
	{
		get
		{
			return DropButton.BackLowColor;
		}
		set
		{
			DropButton.BackLowColor = value;
			CloseButton.BackLowColor = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the border color for the control button.")]
	public Color ControlButtonBorderColor
	{
		get
		{
			return DropButton.BorderColor;
		}
		set
		{
			DropButton.BorderColor = value;
			CloseButton.BorderColor = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the ForeColor for the control button.")]
	public Color ControlButtonForeColor
	{
		get
		{
			return DropButton.ForeColor;
		}
		set
		{
			DropButton.ForeColor = value;
			CloseButton.ForeColor = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the ending color of the Background linear gradient for the tabs region.")]
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

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the starting color of the Background linear gradient for the tabs region.")]
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

	[Browsable(true)]
	[Category("Appearance")]
	[Description("Gets or sets the System.Drawing.Color structure that represents the border color.")]
	public Color BorderColor
	{
		get
		{
			return m_BorderColor;
		}
		set
		{
			m_BorderColor = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.BorderColor = value;
			}
			pnlTabs.Invalidate();
			pnlTop.Invalidate();
		}
	}

	[Browsable(false)]
	[Description("Gets the collection of tab pages in this tab control.")]
	public TabPageCollection TabPages => Items;

	[Browsable(true)]
	[Category("Appearance")]
	[Description("The painting style applied to the control.")]
	public ToolStripRenderMode RenderMode
	{
		get
		{
			return m_RenderMode;
		}
		set
		{
			m_RenderMode = value;
			DropButton.RenderMode = value;
			CloseButton.RenderMode = value;
			WinMenu.RenderMode = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.RenderMode = value;
			}
		}
	}

	[Browsable(false)]
	public ToolStripRenderer MenuRenderer
	{
		get
		{
			return m_ContextMenuRenderer;
		}
		set
		{
			m_ContextMenuRenderer = value;
			WinMenu.RenderMode = ToolStripRenderMode.System;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	[DefaultValue(3)]
	[Description("The weight of the border.")]
	public Weight TabBorderEnhanceWeight
	{
		get
		{
			return m_TabBorderEnhanceWeight;
		}
		set
		{
			m_TabBorderEnhanceWeight = value;
			foreach (TabPage tabPage in TabPages)
			{
				tabPage.BorderEnhanceWeight = value;
			}
		}
	}

	[Browsable(true)]
	[Category("Behavior")]
	[DefaultValue(true)]
	[Description("Gets or sets if the CTRL+TAB/CTRL+SHIFT+TAB will select the next/previous tab.")]
	public bool KeyTabEnabled
	{
		get
		{
			return m_KeyTabEnabled;
		}
		set
		{
			m_KeyTabEnabled = value;
		}
	}

	[Browsable(true)]
	[Category("Behavior")]
	[DefaultValue(true)]
	[Description("Gets or sets if the CTRL+F4 will close the selected tab.")]
	public bool KeyCloseEnabled
	{
		get
		{
			return m_KeyCloseEnabled;
		}
		set
		{
			m_KeyCloseEnabled = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[Category("Appearance")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new BorderStyle BorderStyle
	{
		get
		{
			BorderStyle result = default(BorderStyle);
			return result;
		}
		set
		{
		}
	}

	[Description("Occurs when the Tab Page requests the tab region.")]
	public event GetTabRegionEventHandler GetTabRegion;

	[Description("Occurs when the Tab Background has been painted.")]
	public event TabPaintBackgroundEventHandler TabPaintBackground;

	[Description("Occurs when the Tab Border has been painted.")]
	public event TabPaintBorderEventHandler TabPaintBorder;

	[Description("Occurs when the TabControl Focus changes.")]
	public event EventHandler FocusedChanged;

	public event EventHandler SelectedTabChanged;

	public event TabPressedEventHandler TabPressed;

	public event F4PressedEventHandler F4Pressed;

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
		this.pnlTop = new System.Windows.Forms.Panel();
		this.pnlControls = new System.Windows.Forms.Panel();
		this.DropButton = new MdiTabControl.ControlButton();
		this.CloseButton = new MdiTabControl.ControlButton();
		this.pnlTabs = new System.Windows.Forms.Panel();
		this.pnlBottom = new System.Windows.Forms.Panel();
		this.WinMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.TabToolTip = new System.Windows.Forms.ToolTip(this.components);
		this.pnlTop.SuspendLayout();
		this.pnlControls.SuspendLayout();
		base.SuspendLayout();
		this.pnlTop.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlTop.BackColor = System.Drawing.Color.Transparent;
		this.pnlTop.Controls.Add(this.pnlControls);
		this.pnlTop.Controls.Add(this.pnlTabs);
		this.pnlTop.Location = new System.Drawing.Point(0, 0);
		this.pnlTop.Name = "pnlTop";
		this.pnlTop.Size = new System.Drawing.Size(200, 31);
		this.pnlTop.TabIndex = 6;
		this.pnlControls.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.pnlControls.Controls.Add(this.DropButton);
		this.pnlControls.Controls.Add(this.CloseButton);
		this.pnlControls.Location = new System.Drawing.Point(175, 0);
		this.pnlControls.Name = "pnlControls";
		this.pnlControls.Size = new System.Drawing.Size(25, 30);
		this.pnlControls.TabIndex = 1;
		this.DropButton.BackColor = System.Drawing.Color.Transparent;
		this.DropButton.Location = new System.Drawing.Point(4, 8);
		this.DropButton.Name = "DropButton";
		this.DropButton.Size = new System.Drawing.Size(17, 15);
		this.DropButton.Style = MdiTabControl.ControlButton.ButtonStyle.Drop;
		this.DropButton.TabIndex = 0;
		this.CloseButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.CloseButton.BackColor = System.Drawing.Color.Transparent;
		this.CloseButton.Location = new System.Drawing.Point(4, 8);
		this.CloseButton.Name = "CloseButton";
		this.CloseButton.Size = new System.Drawing.Size(17, 15);
		this.CloseButton.Style = MdiTabControl.ControlButton.ButtonStyle.Close;
		this.CloseButton.TabIndex = 0;
		this.CloseButton.Visible = false;
		this.pnlTabs.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlTabs.BackColor = System.Drawing.Color.Transparent;
		this.pnlTabs.Location = new System.Drawing.Point(0, 3);
		this.pnlTabs.Name = "pnlTabs";
		this.pnlTabs.Size = new System.Drawing.Size(200, 28);
		this.pnlTabs.TabIndex = 0;
		this.pnlBottom.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnlBottom.Location = new System.Drawing.Point(0, 31);
		this.pnlBottom.Name = "pnlBottom";
		this.pnlBottom.Size = new System.Drawing.Size(200, 99);
		this.pnlBottom.TabIndex = 7;
		this.WinMenu.Name = "WinMenu";
		this.WinMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.WinMenu.Size = new System.Drawing.Size(61, 4);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		base.Controls.Add(this.pnlTop);
		base.Controls.Add(this.pnlBottom);
		base.Name = "TabControl";
		base.Size = new System.Drawing.Size(200, 130);
		this.pnlTop.ResumeLayout(false);
		this.pnlControls.ResumeLayout(false);
		base.ResumeLayout(false);
	}

	public TabControl()
	{
		base.FontChanged += TabControl_FontChanged;
		base.ForeColorChanged += TabControl_ForeColorChanged;
		base.Paint += TabControl_Paint;
		base.Resize += TabControl_Resize;
		base.Load += TabControl_Load;
		AddingPage = false;
		LeftOffset = 3;
		IsDelete = false;
		Background = new Panel();
		Items = new TabPageCollection(this);
		m_TabsDirection = FlowDirection.LeftToRight;
		m_TabMaximumWidth = 200;
		m_tabMinimumWidth = 100;
		m_TopSeparator = true;
		m_TabTop = 3;
		m_TabHeight = 28;
		m_TabOffset = 3;
		m_TabPadLeft = 5;
		m_TabPadRight = 5;
		m_TabSmoothingMode = SmoothingMode.None;
		m_TabIconSize = new Size(16, 16);
		m_Alignment = TabAlignment.Top;
		m_FontBoldOnSelect = true;
		m_HotTrack = true;
		m_TabCloseButtonSize = new Size(17, 17);
		m_TabCloseButtonVisible = true;
		m_AllowTabReorder = true;
		m_TabGlassGradient = false;
		m_TabBorderEnhanced = false;
		m_TabBorderEnhanceWeight = Weight.Medium;
		defaultPadding = new Padding(0, 0, 0, 0);
		defaultBackLowColor = SystemColors.ControlLightLight;
		defaultBackHighColor = SystemColors.Control;
		defaultBorderColor = SystemColors.ControlDarkDark;
		defaultTabBackHighColor = SystemColors.Window;
		defaultTabBackLowColor = SystemColors.Control;
		defaultTabBackHighColorDisabled = SystemColors.Control;
		defaultTabBackLowColorDisabled = SystemColors.ControlDark;
		defaultBorderColorDisabled = SystemColors.ControlDark;
		defaultForeColorDisabled = SystemColors.ControlText;
		defaultControlButtonBackHighColor = SystemColors.GradientInactiveCaption;
		defaultControlButtonBackLowColor = SystemColors.GradientInactiveCaption;
		defaultControlButtonBorderColor = SystemColors.HotTrack;
		defaultControlButtonForeColor = SystemColors.ControlText;
		defaultTabCloseButtonSize = new Size(17, 17);
		defaultTabIconSize = new Size(16, 16);
		defaultTabCloseButtonBackHighColor = Color.IndianRed;
		defaultTabCloseButtonBackHighColorDisabled = Color.LightGray;
		defaultTabCloseButtonBackHighColorHot = Color.LightCoral;
		defaultTabCloseButtonBackLowColor = Color.Firebrick;
		defaultTabCloseButtonBackLowColorDisabled = Color.DarkGray;
		defaultTabCloseButtonBackLowColorHot = Color.IndianRed;
		defaultTabCloseButtonBorderColor = Color.DarkRed;
		defaultTabCloseButtonBorderColorDisabled = Color.Gray;
		defaultTabCloseButtonBorderColorHot = Color.Firebrick;
		defaultTabCloseButtonForeColor = Color.White;
		defaultTabCloseButtonForeColorDisabled = Color.White;
		defaultTabCloseButtonForeColorHot = Color.White;
		defaultRenderMode = ToolStripRenderMode.ManagerRenderMode;
		m_KeyCloseEnabled = true;
		m_KeyTabEnabled = true;
		InitializeComponent();
		SuspendLayout();
		SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
		Background.BackColor = SystemColors.AppWorkspace;
		Background.BorderStyle = BorderStyle.Fixed3D;
		Background.Dock = DockStyle.Fill;
		base.Controls.Add(Background);
		Background.BringToFront();
		ResetBackLowColor();
		ResetBackHighColor();
		ResetBorderColor();
		ResetTabBackHighColor();
		ResetTabBackLowColor();
		ResetTabBackHighColorDisabled();
		ResetTabBackLowColorDisabled();
		ResetBorderColorDisabled();
		ResetForeColorDisabled();
		ResetControlButtonBackHighColor();
		ResetControlButtonBackLowColor();
		ResetControlButtonBorderColor();
		ResetControlButtonForeColor();
		ResetTabCloseButtonBackHighColor();
		ResetTabCloseButtonBackLowColor();
		ResetTabCloseButtonBorderColor();
		ResetTabCloseButtonForeColor();
		ResetTabCloseButtonBackHighColorDisabled();
		ResetTabCloseButtonBackLowColorDisabled();
		ResetTabCloseButtonBorderColorDisabled();
		ResetTabCloseButtonForeColorDisabled();
		ResetTabCloseButtonBackHighColorHot();
		ResetTabCloseButtonBackLowColorHot();
		ResetTabCloseButtonBorderColorHot();
		ResetTabCloseButtonForeColorHot();
		ResetPadding();
		ResetTabCloseButtonSize();
		ResetTabIconSize();
		ResetRenderMode();
		AdjustHeight();
		DropButton.RenderMode = RenderMode;
		CloseButton.RenderMode = RenderMode;
		ResumeLayout();
	}

	internal bool ShouldSerializeTabCloseButtonBackHighColor()
	{
		return m_TabCloseButtonBackHighColor != defaultTabCloseButtonBackHighColor;
	}

	internal void ResetTabCloseButtonBackHighColor()
	{
		m_TabCloseButtonBackHighColor = defaultTabCloseButtonBackHighColor;
	}

	internal bool ShouldSerializeTabCloseButtonBackLowColor()
	{
		return m_TabCloseButtonBackLowColor != defaultTabCloseButtonBackLowColor;
	}

	internal void ResetTabCloseButtonBackLowColor()
	{
		m_TabCloseButtonBackLowColor = defaultTabCloseButtonBackLowColor;
	}

	internal bool ShouldSerializeTabCloseButtonBorderColor()
	{
		return m_TabCloseButtonBorderColor != defaultTabCloseButtonBorderColor;
	}

	internal void ResetTabCloseButtonBorderColor()
	{
		m_TabCloseButtonBorderColor = defaultTabCloseButtonBorderColor;
	}

	internal bool ShouldSerializeTabCloseButtonForeColor()
	{
		return m_TabCloseButtonForeColor != defaultTabCloseButtonForeColor;
	}

	internal void ResetTabCloseButtonForeColor()
	{
		m_TabCloseButtonForeColor = defaultTabCloseButtonForeColor;
	}

	internal bool ShouldSerializeTabCloseButtonBackHighColorDisabled()
	{
		return m_TabCloseButtonBackHighColorDisabled != defaultTabCloseButtonBackHighColorDisabled;
	}

	internal void ResetTabCloseButtonBackHighColorDisabled()
	{
		m_TabCloseButtonBackHighColorDisabled = defaultTabCloseButtonBackHighColorDisabled;
	}

	internal bool ShouldSerializeTabCloseButtonBackLowColorDisabled()
	{
		return m_TabCloseButtonBackLowColorDisabled != defaultTabCloseButtonBackLowColorDisabled;
	}

	internal void ResetTabCloseButtonBackLowColorDisabled()
	{
		m_TabCloseButtonBackLowColorDisabled = defaultTabCloseButtonBackLowColorDisabled;
	}

	internal bool ShouldSerializeTabCloseButtonBorderColorDisabled()
	{
		return m_TabCloseButtonBorderColorDisabled != defaultTabCloseButtonBorderColorDisabled;
	}

	internal void ResetTabCloseButtonBorderColorDisabled()
	{
		m_TabCloseButtonBorderColorDisabled = defaultTabCloseButtonBorderColorDisabled;
	}

	internal bool ShouldSerializeTabCloseButtonForeColorDisabled()
	{
		return m_TabCloseButtonForeColorDisabled != defaultTabCloseButtonForeColorDisabled;
	}

	internal void ResetTabCloseButtonForeColorDisabled()
	{
		m_TabCloseButtonForeColorDisabled = defaultTabCloseButtonForeColorDisabled;
	}

	internal bool ShouldSerializeTabCloseButtonBackHighColorHot()
	{
		return m_TabCloseButtonBackHighColorHot != defaultTabCloseButtonBackHighColorHot;
	}

	internal void ResetTabCloseButtonBackHighColorHot()
	{
		m_TabCloseButtonBackHighColorHot = defaultTabCloseButtonBackHighColorHot;
	}

	internal bool ShouldSerializeTabCloseButtonBackLowColorHot()
	{
		return m_TabCloseButtonBackLowColorHot != defaultTabCloseButtonBackLowColorHot;
	}

	internal void ResetTabCloseButtonBackLowColorHot()
	{
		m_TabCloseButtonBackLowColorHot = defaultTabCloseButtonBackLowColorHot;
	}

	internal bool ShouldSerializeTabCloseButtonBorderColorHot()
	{
		return m_TabCloseButtonBorderColorHot != defaultTabCloseButtonBorderColorHot;
	}

	internal void ResetTabCloseButtonBorderColorHot()
	{
		m_TabCloseButtonBorderColorHot = defaultTabCloseButtonBorderColorHot;
	}

	internal bool ShouldSerializeTabCloseButtonForeColorHot()
	{
		return m_TabCloseButtonForeColorHot != defaultTabCloseButtonForeColorHot;
	}

	internal void ResetTabCloseButtonForeColorHot()
	{
		m_TabCloseButtonForeColorHot = defaultTabCloseButtonForeColorHot;
	}

	internal bool ShouldSerializeTabIconSize()
	{
		return m_TabIconSize != defaultTabIconSize;
	}

	internal void ResetTabIconSize()
	{
		m_TabIconSize = defaultTabIconSize;
	}

	internal bool ShouldSerializeTabCloseButtonSize()
	{
		return m_TabCloseButtonSize != defaultTabCloseButtonSize;
	}

	internal void ResetTabCloseButtonSize()
	{
		m_TabCloseButtonSize = defaultTabCloseButtonSize;
	}

	internal bool ShouldSerializeTabBackHighColor()
	{
		return m_TabBackHighColor != defaultTabBackHighColor;
	}

	internal void ResetTabBackHighColor()
	{
		m_TabBackHighColor = defaultTabBackHighColor;
	}

	internal bool ShouldSerializeTabBackLowColor()
	{
		return m_TabBackLowColor != defaultTabBackLowColor;
	}

	internal void ResetTabBackLowColor()
	{
		m_TabBackLowColor = defaultTabBackLowColor;
	}

	internal bool ShouldSerializeTabBackHighColorDisabled()
	{
		return m_TabBackHighColorDisabled != defaultTabBackHighColorDisabled;
	}

	internal void ResetTabBackHighColorDisabled()
	{
		m_TabBackHighColorDisabled = defaultTabBackHighColorDisabled;
	}

	internal bool ShouldSerializeTabBackLowColorDisabled()
	{
		return m_TabBackLowColorDisabled != defaultTabBackLowColorDisabled;
	}

	internal void ResetTabBackLowColorDisabled()
	{
		m_TabBackLowColorDisabled = defaultTabBackLowColorDisabled;
	}

	internal bool ShouldSerializeBorderColorDisabled()
	{
		return m_BorderColorDisabled != defaultBorderColorDisabled;
	}

	internal void ResetBorderColorDisabled()
	{
		m_BorderColorDisabled = defaultBorderColorDisabled;
	}

	internal bool ShouldSerializeForeColorDisabled()
	{
		return m_ForeColorDisabled != defaultForeColorDisabled;
	}

	internal void ResetForeColorDisabled()
	{
		m_ForeColorDisabled = defaultForeColorDisabled;
	}

	internal bool ShouldSerializePadding()
	{
		return pnlBottom.Padding != defaultPadding;
	}

	internal void ResetPadding()
	{
		pnlBottom.Padding = defaultPadding;
	}

	internal bool ShouldSerializeControlButtonBackHighColor()
	{
		return DropButton.BackHighColor != defaultControlButtonBackHighColor;
	}

	internal void ResetControlButtonBackHighColor()
	{
		DropButton.BackHighColor = defaultControlButtonBackHighColor;
		CloseButton.BackHighColor = defaultControlButtonBackHighColor;
	}

	internal bool ShouldSerializeControlButtonBackLowColor()
	{
		return DropButton.BackLowColor != defaultControlButtonBackLowColor;
	}

	internal void ResetControlButtonBackLowColor()
	{
		DropButton.BackLowColor = defaultControlButtonBackLowColor;
		CloseButton.BackLowColor = defaultControlButtonBackLowColor;
	}

	internal bool ShouldSerializeControlButtonBorderColor()
	{
		return DropButton.BorderColor != defaultControlButtonBorderColor;
	}

	internal void ResetControlButtonBorderColor()
	{
		DropButton.BorderColor = defaultControlButtonBorderColor;
		CloseButton.BorderColor = defaultControlButtonBorderColor;
	}

	internal bool ShouldSerializeControlButtonForeColor()
	{
		return DropButton.ForeColor != defaultControlButtonForeColor;
	}

	internal void ResetControlButtonForeColor()
	{
		DropButton.ForeColor = defaultControlButtonForeColor;
		CloseButton.ForeColor = defaultControlButtonForeColor;
	}

	internal bool ShouldSerializeBackLowColor()
	{
		return m_BackLowColor != defaultBackLowColor;
	}

	internal void ResetBackLowColor()
	{
		m_BackLowColor = defaultBackLowColor;
	}

	internal bool ShouldSerializeBackHighColor()
	{
		return m_BackHighColor != defaultBackHighColor;
	}

	internal void ResetBackHighColor()
	{
		m_BackHighColor = defaultBackHighColor;
	}

	internal bool ShouldSerializeBorderColor()
	{
		return m_BorderColor != defaultBorderColor;
	}

	internal void ResetBorderColor()
	{
		m_BorderColor = defaultBorderColor;
	}

	internal bool ShouldSerializeRenderMode()
	{
		return m_RenderMode != defaultRenderMode;
	}

	internal void ResetRenderMode()
	{
		m_RenderMode = defaultRenderMode;
	}

	private void SetControlsSizeLocation()
	{
		if (DropButton.Visible & CloseButton.Visible)
		{
			pnlControls.Width = 43;
		}
		else if (DropButton.Visible | CloseButton.Visible)
		{
			pnlControls.Width = 25;
		}
		else
		{
			pnlControls.Width = 3;
		}
		pnlControls.Left = checked(base.Width - pnlControls.Width);
		CheckVisibility();
	}

	private void AdjustHeight()
	{
		checked
		{
			if (Alignment == TabAlignment.Top)
			{
				pnlTop.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				pnlTop.Height = pnlTabs.Height + m_TabTop;
				pnlTop.Top = Conversions.ToInteger(Interaction.IIf(m_TopSeparator, (object)2, (object)0));
				pnlTabs.Top = m_TabTop;
				pnlBottom.Height = Conversions.ToInteger(Operators.SubtractObject((object)base.Height, Operators.AddObject((object)pnlTop.Height, Interaction.IIf(m_TopSeparator, (object)2, (object)0))));
				pnlBottom.Top = base.Height - pnlBottom.Height;
			}
			else
			{
				pnlTop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				pnlTop.Height = pnlTabs.Height + m_TabTop;
				pnlTop.Top = base.Height - pnlTop.Height;
				pnlTabs.Top = 0;
				pnlBottom.Height = Conversions.ToInteger(Operators.SubtractObject((object)base.Height, Operators.AddObject((object)pnlTop.Height, Interaction.IIf(m_TopSeparator, (object)2, (object)0))));
				pnlBottom.Top = Conversions.ToInteger(Interaction.IIf(m_TopSeparator, (object)2, (object)0));
			}
			pnlTop.Invalidate();
		}
	}

	private void ArrangeItems()
	{
		pnlTabs.SuspendLayout();
		if (Items.Count == 0)
		{
			return;
		}
		int num = LeftOffset;
		checked
		{
			int num2 = Items.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				Items[i].TabVisible = num + Items[i].Width < pnlControls.Left;
				if (Items[i].IsSelected & !Items[i].TabVisible)
				{
					SelectItem(Items[i]);
					return;
				}
				Items[i].TabLeft = num;
				num += Items[i].Width + m_TabOffset - 1;
			}
			if (!AddingPage)
			{
				if (IsDelete)
				{
					for (int j = Items.Count - 1; j >= 0; j += -1)
					{
						ShowTab(j);
					}
					IsDelete = false;
				}
				else
				{
					int num3 = Items.Count - 1;
					for (int k = 0; k <= num3; k++)
					{
						ShowTab(k);
					}
				}
			}
			pnlTabs.ResumeLayout();
		}
	}

	private void CheckVisibility()
	{
		if (Items == null)
		{
			return;
		}
		int num = LeftOffset;
		checked
		{
			int num2 = Items.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				if (Items[i].TabVisible != num + Items[i].Width < pnlControls.Left)
				{
					if (Items[i].TabVisible)
					{
						Items[i].TabVisible = false;
						if (Items[i].IsSelected)
						{
							SelectItem(Items[i]);
						}
						else
						{
							ShowTab(i);
						}
						break;
					}
					Items[i].TabVisible = true;
					Items[i].TabLeft = num;
					ShowTab(i);
				}
				else if (!Items[i].TabVisible)
				{
					break;
				}
				num += Items[i].Width + m_TabOffset - 1;
				if (num > pnlControls.Left)
				{
					break;
				}
			}
		}
	}

	private void ShowTab(int i)
	{
		Items[i].Visible = Items[i].TabVisible;
		if (Items[0].Width != 1)
		{
			Items[i].Left = Items[i].TabLeft;
		}
	}

	private void SelectItem(TabPage TabPage)
	{
		foreach (TabPage tabPage3 in TabPages)
		{
			tabPage3.IsSelected = false;
		}
		if (TabPage != null)
		{
			foreach (TabPage tabPage4 in TabPages)
			{
				if (m_TabsDirection == FlowDirection.LeftToRight)
				{
					tabPage4.SendToBack();
				}
				else
				{
					tabPage4.BringToFront();
				}
			}
			TabPage.m_Form.Dock = DockStyle.Fill;
			TabPage.m_Form.Visible = true;
			TabPage.BringToFront();
			TabPage.m_Form.BringToFront();
			TabPage.m_Form.Focus();
			if (pnlBottom.Controls.Count > 1)
			{
				pnlBottom.Controls[1].Visible = false;
				pnlBottom.Controls[1].Dock = DockStyle.None;
			}
			TabPage.IsSelected = true;
			if (!TabPage.TabVisible & (TabPages.get_IndexOf(TabPage) != 0))
			{
				TabPages.set_IndexOf(TabPage, 0);
			}
		}
		else
		{
			if (pnlTabs.Controls.Count <= 0)
			{
				return;
			}
			foreach (TabPage item in Items)
			{
				if (item.m_Form.Equals(pnlBottom.Controls[0]))
				{
					item.Select();
					break;
				}
			}
		}
	}

	private void TabControl_FontChanged(object sender, EventArgs e)
	{
		foreach (TabPage tabPage in TabPages)
		{
			tabPage.Font = Font;
		}
	}

	private void TabControl_ForeColorChanged(object sender, EventArgs e)
	{
		foreach (TabPage tabPage in TabPages)
		{
			tabPage.ForeColor = ForeColor;
		}
	}

	private void TabControl_Paint(object sender, PaintEventArgs e)
	{
		if (m_TopSeparator)
		{
			ControlPaint.DrawBorder3D(e.Graphics, new Rectangle(-2, 0, checked(base.Width + 4), -2));
		}
	}

	private void TabControl_Resize(object sender, EventArgs e)
	{
		CheckVisibility();
	}

	private void pnlTop_SizeChanged(object sender, EventArgs e)
	{
		PositionButtons();
	}

	private void PositionButtons()
	{
		DropButton.Top = Conversions.ToInteger(Operators.AddObject((object)Math.Ceiling((double)checked(pnlTop.Height - DropButton.Height) / 2.0), Interaction.IIf((Alignment == TabAlignment.Top) & TopSeparator, (object)(-1), (object)0)));
		CloseButton.Top = DropButton.Top;
	}

	private void pnlTop_Paint(object sender, PaintEventArgs e)
	{
		LinearGradientBrush linearGradientBrush = new LinearGradientBrush(new Point(0, 0), new Point(0, pnlTop.Height), Helper.RenderColors.BackHighColor(m_RenderMode, BackHighColor), Helper.RenderColors.BackLowColor(m_RenderMode, BackLowColor));
		e.Graphics.FillRectangle(linearGradientBrush, new Rectangle(0, 0, pnlTop.Width, pnlTop.Height));
		Pen pen = new Pen(Helper.RenderColors.BorderColor(m_RenderMode, BorderColor));
		if (Alignment == TabAlignment.Top)
		{
			Graphics graphics = e.Graphics;
			object[] obj = new object[5]
			{
				pen,
				0,
				Operators.SubtractObject(NewLateBinding.LateGet(sender, (Type)null, "Height", new object[0], (string[])null, (Type[])null, (bool[])null), (object)1),
				Operators.AddObject(NewLateBinding.LateGet(sender, (Type)null, "Width", new object[0], (string[])null, (Type[])null, (bool[])null), (object)1),
				Operators.SubtractObject(NewLateBinding.LateGet(sender, (Type)null, "Height", new object[0], (string[])null, (Type[])null, (bool[])null), (object)1)
			};
			object[] array = obj;
			bool[] obj2 = new bool[5] { true, false, false, false, false };
			bool[] array2 = obj2;
			NewLateBinding.LateCall((object)graphics, (Type)null, "DrawLine", obj, (string[])null, (Type[])null, obj2, true);
			if (array2[0])
			{
				pen = (Pen)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array[0]), typeof(Pen));
			}
		}
		else
		{
			Graphics graphics2 = e.Graphics;
			object[] obj3 = new object[5]
			{
				pen,
				0,
				0,
				Operators.AddObject(NewLateBinding.LateGet(sender, (Type)null, "Width", new object[0], (string[])null, (Type[])null, (bool[])null), (object)1),
				0
			};
			object[] array = obj3;
			bool[] obj4 = new bool[5] { true, false, false, false, false };
			bool[] array2 = obj4;
			NewLateBinding.LateCall((object)graphics2, (Type)null, "DrawLine", obj3, (string[])null, (Type[])null, obj4, true);
			if (array2[0])
			{
				pen = (Pen)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array[0]), typeof(Pen));
			}
		}
		pen.Dispose();
		linearGradientBrush.Dispose();
	}

	private void DropButton_MouseDown(object sender, MouseEventArgs e)
	{
		WinMenu.Items.Clear();
		checked
		{
			int num = Items.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				Items[i].MenuItem.Image = TabPages[i].Icon.ToBitmap();
				WinMenu.Items.Add(Items[i].MenuItem);
				Items[i].MenuItem.Click += MenuClick;
			}
			WinMenu.Show(pnlTop, pnlTop.Width - WinMenu.Width, pnlTop.Height - 1);
		}
	}

	private void MenuClick(object sender, EventArgs e)
	{
		NewLateBinding.LateCall(NewLateBinding.LateGet(sender, (Type)null, "Tag", new object[0], (string[])null, (Type[])null, (bool[])null), (Type)null, "Select", new object[0], (string[])null, (Type[])null, (bool[])null, true);
	}

	private void CloseButton_MouseDown(object sender, MouseEventArgs e)
	{
		Items.SelectedTab().m_Form.Close();
	}

	private void Items_GetTabRegion(object sender, GetTabRegionEventArgs e)
	{
		GetTabRegion?.Invoke(RuntimeHelpers.GetObjectValue(sender), e);
	}

	private void Items_SelectedChanged(object sender, EventArgs e)
	{
		SelectedTabChanged?.Invoke(RuntimeHelpers.GetObjectValue(sender), e);
	}

	private void Items_TabPaintBackground(object sender, TabPaintEventArgs e)
	{
		TabPaintBackground?.Invoke(RuntimeHelpers.GetObjectValue(sender), e);
	}

	private void Items_TabPaintBorder(object sender, TabPaintEventArgs e)
	{
		TabPaintBorder?.Invoke(RuntimeHelpers.GetObjectValue(sender), e);
	}

	public void SetColors(ProfessionalColorTable ColorTable)
	{
		BackHighColor = ColorTable.ToolStripGradientEnd;
		BackLowColor = ColorTable.ToolStripGradientBegin;
		BorderColor = ColorTable.GripDark;
		BorderColorDisabled = ColorTable.SeparatorDark;
		ControlButtonBackHighColor = ColorTable.ButtonSelectedGradientBegin;
		ControlButtonBackLowColor = ColorTable.ButtonSelectedGradientEnd;
		ControlButtonBorderColor = ColorTable.ButtonPressedBorder;
		TabBackHighColor = ColorTable.MenuItemPressedGradientBegin;
		TabBackLowColor = ColorTable.MenuItemPressedGradientEnd;
		TabBackHighColorDisabled = ColorTable.ToolStripDropDownBackground;
		TabBackLowColorDisabled = ColorTable.ToolStripGradientMiddle;
		TabCloseButtonBackHighColor = Color.Transparent;
		TabCloseButtonBackHighColorDisabled = Color.Transparent;
		TabCloseButtonBackHighColorHot = Color.WhiteSmoke;
		TabCloseButtonBackLowColor = Color.Transparent;
		TabCloseButtonBackLowColorDisabled = Color.Transparent;
		TabCloseButtonBackLowColorHot = Color.LightGray;
		TabCloseButtonBorderColor = Color.Transparent;
		TabCloseButtonBorderColorDisabled = Color.Transparent;
		TabCloseButtonBorderColorHot = Color.Gray;
		TabCloseButtonForeColor = Color.Gray;
		TabCloseButtonForeColorDisabled = Color.Gray;
		TabCloseButtonForeColorHot = Color.Firebrick;
	}

	private void TabControl_Load(object sender, EventArgs e)
	{
		try
		{
			base.ParentForm.KeyPreview = true;
			base.ParentForm.KeyDown += Owner_KeyDown;
		}
		catch (Exception projectError)
		{
			ProjectData.SetProjectError(projectError);
			ProjectData.ClearProjectError();
		}
	}

	private void Owner_KeyDown(object sender, KeyEventArgs e)
	{
		if (TabPages.Count == 0)
		{
			return;
		}
		KeyHandledEventArgs e2 = new KeyHandledEventArgs();
		checked
		{
			if (KeyTabEnabled & (e.KeyCode == Keys.Tab) & e.Control & !e.Alt)
			{
				int num;
				if (e.Shift)
				{
					num = TabPages.SelectedIndex() - 1;
					if (num < 0)
					{
						num = TabPages.Count - 1;
					}
					e2.Shift = true;
				}
				else
				{
					num = TabPages.SelectedIndex() + 1;
					if (num > TabPages.Count - 1)
					{
						num = 0;
					}
				}
				e2.Index = num;
				e2.Handled = false;
				TabPressed?.Invoke(this, e2);
				if (!e2.Handled)
				{
					TabPages[num].Select();
				}
			}
			else if (KeyCloseEnabled & (e.KeyCode == Keys.F4) & e.Control & !e.Shift & !e.Alt)
			{
				e2.Index = TabPages.SelectedIndex();
				e2.Handled = false;
				F4Pressed?.Invoke(this, e2);
				if (!e2.Handled)
				{
					TabPages.SelectedTab().m_Form.Close();
				}
			}
		}
	}
}
