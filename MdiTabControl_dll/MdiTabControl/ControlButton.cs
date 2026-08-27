using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace MdiTabControl;

[DesignerGenerated]
internal class ControlButton : Control
{
	public enum ButtonStyle
	{
		Close,
		Drop
	}

	private IContainer components;

	private bool m_hot;

	private Color m_BackHighColor;

	private Color m_BackLowColor;

	private Color m_BorderColor;

	private ButtonStyle m_style;

	private ToolStripRenderMode m_RenderMode;

	private readonly Color defaultBackHighColor;

	private readonly Color defaultBackLowColor;

	private readonly Color defaultBorderColor;

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

	public ButtonStyle Style
	{
		get
		{
			return m_style;
		}
		set
		{
			m_style = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	public Color BackHighColor
	{
		get
		{
			return m_BackHighColor;
		}
		set
		{
			m_BackHighColor = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	public Color BackLowColor
	{
		get
		{
			return m_BackLowColor;
		}
		set
		{
			m_BackLowColor = value;
		}
	}

	[Browsable(true)]
	[Category("Appearance")]
	public Color BorderColor
	{
		get
		{
			return m_BorderColor;
		}
		set
		{
			m_BorderColor = value;
		}
	}

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

	public ControlButton()
	{
		base.MouseEnter += MdiTab_MouseEnter;
		base.MouseLeave += MdiTab_MouseLeave;
		m_hot = false;
		defaultBackHighColor = SystemColors.ControlLightLight;
		defaultBackLowColor = SystemColors.ControlDark;
		defaultBorderColor = SystemColors.ControlDarkDark;
		InitializeComponent();
		SuspendLayout();
		SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
		base.BackColor = Color.Transparent;
		ResetBackHighColor();
		ResetBackLowColor();
		ResetBorderColor();
		ResumeLayout();
	}

	public bool ShouldSerializeBackHighColor()
	{
		return m_BackHighColor != defaultBackHighColor;
	}

	public void ResetBackHighColor()
	{
		m_BackHighColor = defaultBackHighColor;
	}

	public bool ShouldSerializeBackLowColor()
	{
		return m_BackLowColor != defaultBackLowColor;
	}

	public void ResetBackLowColor()
	{
		m_BackLowColor = defaultBackLowColor;
	}

	public bool ShouldSerializeBorderColor()
	{
		return m_BorderColor != defaultBorderColor;
	}

	public void ResetBorderColor()
	{
		m_BorderColor = defaultBorderColor;
	}

	[DebuggerStepThrough]
	protected override void OnPaint(PaintEventArgs e)
	{
		Point[] points = new Point[3]
		{
			new Point(0, 0),
			new Point(11, 0),
			new Point(5, 6)
		};
		Point[] points2 = new Point[12]
		{
			new Point(0, 0),
			new Point(2, 0),
			new Point(5, 3),
			new Point(8, 0),
			new Point(10, 0),
			new Point(6, 4),
			new Point(10, 8),
			new Point(8, 8),
			new Point(5, 5),
			new Point(2, 8),
			new Point(0, 8),
			new Point(4, 4)
		};
		checked
		{
			Rectangle rect = new Rectangle
			{
				Size = new Size(base.Width - 1, base.Height - 1)
			};
			if (m_hot)
			{
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				e.Graphics.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(0, base.Height), Helper.RenderColors.ControlButtonBackHighColor(m_RenderMode, m_BackHighColor), Helper.RenderColors.ControlButtonBackLowColor(m_RenderMode, m_BackLowColor)), rect);
				e.Graphics.DrawRectangle(new Pen(Helper.RenderColors.ControlButtonBorderColor(m_RenderMode, m_BorderColor)), rect);
				e.Graphics.SmoothingMode = SmoothingMode.Default;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			Matrix matrix = new Matrix();
			int num = (int)Math.Round((double)(base.Width - 11) / 2.0);
			int num2 = (int)Math.Round((double)(base.Height - 11) / 2.0 + 1.0);
			if (m_style == ButtonStyle.Drop)
			{
				e.Graphics.FillRectangle(new SolidBrush(ForeColor), num, num2, 11, 2);
				graphicsPath.AddPolygon(points);
				matrix.Translate(num, num2 + 3);
				graphicsPath.Transform(matrix);
				e.Graphics.FillPolygon(new SolidBrush(ForeColor), graphicsPath.PathPoints);
			}
			else
			{
				graphicsPath.AddPolygon(points2);
				matrix.Translate(num, num2);
				graphicsPath.Transform(matrix);
				e.Graphics.DrawPolygon(new Pen(ForeColor), graphicsPath.PathPoints);
				e.Graphics.FillPolygon(new SolidBrush(ForeColor), graphicsPath.PathPoints);
			}
			graphicsPath.Dispose();
			matrix.Dispose();
		}
	}

	private void MdiTab_MouseEnter(object sender, EventArgs e)
	{
		m_hot = true;
		Invalidate();
	}

	private void MdiTab_MouseLeave(object sender, EventArgs e)
	{
		m_hot = false;
		Invalidate();
	}
}
