using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Office2007Renderer;

public class Office2007Renderer : ToolStripProfessionalRenderer
{
	private class GradientItemColors
	{
		public Color InsideTop1;

		public Color InsideTop2;

		public Color InsideBottom1;

		public Color InsideBottom2;

		public Color FillTop1;

		public Color FillTop2;

		public Color FillBottom1;

		public Color FillBottom2;

		public Color Border1;

		public Color Border2;

		public GradientItemColors(Color insideTop1, Color insideTop2, Color insideBottom1, Color insideBottom2, Color fillTop1, Color fillTop2, Color fillBottom1, Color fillBottom2, Color border1, Color border2)
		{
			InsideTop1 = insideTop1;
			InsideTop2 = insideTop2;
			InsideBottom1 = insideBottom1;
			InsideBottom2 = insideBottom2;
			FillTop1 = fillTop1;
			FillTop2 = fillTop2;
			FillBottom1 = fillBottom1;
			FillBottom2 = fillBottom2;
			Border1 = border1;
			Border2 = border2;
		}
	}

	private static int _gripOffset;

	private static int _gripSquare;

	private static int _gripSize;

	private static int _gripMove;

	private static int _gripLines;

	private static int _checkInset;

	private static int _marginInset;

	private static int _separatorInset;

	private static float _cutToolItemMenu;

	private static float _cutContextMenu;

	private static float _cutMenuItemBack;

	private static float _contextCheckTickThickness;

	private static Blend _statusStripBlend;

	private static Color _c1;

	private static Color _c2;

	private static Color _c3;

	private static Color _c4;

	private static Color _c5;

	private static Color _c6;

	private static Color _r1;

	private static Color _r2;

	private static Color _r3;

	private static Color _r4;

	private static Color _r5;

	private static Color _r6;

	private static Color _r7;

	private static Color _r8;

	private static Color _r9;

	private static Color _rA;

	private static Color _rB;

	private static Color _rC;

	private static Color _rD;

	private static Color _rE;

	private static Color _rF;

	private static Color _rG;

	private static Color _rH;

	private static Color _rI;

	private static Color _rJ;

	private static Color _rK;

	private static Color _rL;

	private static Color _rM;

	private static Color _rN;

	private static Color _rO;

	private static Color _rP;

	private static Color _rQ;

	private static Color _rR;

	private static Color _rS;

	private static Color _rT;

	private static Color _rU;

	private static Color _rV;

	private static Color _rW;

	private static Color _rX;

	private static Color _rY;

	private static Color _rZ;

	private static Color _textDisabled;

	private static Color _textMenuStripItem;

	private static Color _textStatusStripItem;

	private static Color _textContextMenuItem;

	private static Color _arrowDisabled;

	private static Color _arrowLight;

	private static Color _arrowDark;

	private static Color _separatorMenuLight;

	private static Color _separatorMenuDark;

	private static Color _contextMenuBack;

	private static Color _contextCheckBorder;

	private static Color _contextCheckTick;

	private static Color _statusStripBorderDark;

	private static Color _statusStripBorderLight;

	private static Color _gripDark;

	private static Color _gripLight;

	private static GradientItemColors _itemContextItemEnabledColors;

	private static GradientItemColors _itemDisabledColors;

	private static GradientItemColors _itemToolItemSelectedColors;

	private static GradientItemColors _itemToolItemPressedColors;

	private static GradientItemColors _itemToolItemCheckedColors;

	private static GradientItemColors _itemToolItemCheckPressColors;

	static Office2007Renderer()
	{
		_gripOffset = 1;
		_gripSquare = 2;
		_gripSize = 3;
		_gripMove = 4;
		_gripLines = 3;
		_checkInset = 1;
		_marginInset = 2;
		_separatorInset = 31;
		_cutToolItemMenu = 1f;
		_cutContextMenu = 0f;
		_cutMenuItemBack = 1.2f;
		_contextCheckTickThickness = 1.6f;
		_c1 = Color.FromArgb(167, 167, 167);
		_c2 = Color.FromArgb(21, 66, 139);
		_c3 = Color.FromArgb(76, 83, 92);
		_c4 = Color.FromArgb(250, 250, 250);
		_c5 = Color.FromArgb(248, 248, 248);
		_c6 = Color.FromArgb(243, 243, 243);
		_r1 = Color.FromArgb(255, 255, 251);
		_r2 = Color.FromArgb(255, 249, 227);
		_r3 = Color.FromArgb(255, 242, 201);
		_r4 = Color.FromArgb(255, 248, 181);
		_r5 = Color.FromArgb(255, 252, 229);
		_r6 = Color.FromArgb(255, 235, 166);
		_r7 = Color.FromArgb(255, 213, 103);
		_r8 = Color.FromArgb(255, 228, 145);
		_r9 = Color.FromArgb(160, 188, 228);
		_rA = Color.FromArgb(121, 153, 194);
		_rB = Color.FromArgb(182, 190, 192);
		_rC = Color.FromArgb(155, 163, 167);
		_rD = Color.FromArgb(233, 168, 97);
		_rE = Color.FromArgb(247, 164, 39);
		_rF = Color.FromArgb(246, 156, 24);
		_rG = Color.FromArgb(253, 173, 17);
		_rH = Color.FromArgb(254, 185, 108);
		_rI = Color.FromArgb(253, 164, 97);
		_rJ = Color.FromArgb(252, 143, 61);
		_rK = Color.FromArgb(255, 208, 134);
		_rL = Color.FromArgb(249, 192, 103);
		_rM = Color.FromArgb(250, 195, 93);
		_rN = Color.FromArgb(248, 190, 81);
		_rO = Color.FromArgb(255, 208, 49);
		_rP = Color.FromArgb(254, 214, 168);
		_rQ = Color.FromArgb(252, 180, 100);
		_rR = Color.FromArgb(252, 161, 54);
		_rS = Color.FromArgb(254, 238, 170);
		_rT = Color.FromArgb(249, 202, 113);
		_rU = Color.FromArgb(250, 205, 103);
		_rV = Color.FromArgb(248, 200, 91);
		_rW = Color.FromArgb(255, 218, 59);
		_rX = Color.FromArgb(254, 185, 108);
		_rY = Color.FromArgb(252, 161, 54);
		_rZ = Color.FromArgb(254, 238, 170);
		_textDisabled = _c1;
		_textMenuStripItem = _c2;
		_textStatusStripItem = _c2;
		_textContextMenuItem = _c2;
		_arrowDisabled = _c1;
		_arrowLight = Color.FromArgb(106, 126, 197);
		_arrowDark = Color.FromArgb(64, 70, 90);
		_separatorMenuLight = Color.FromArgb(245, 245, 245);
		_separatorMenuDark = Color.FromArgb(197, 197, 197);
		_contextMenuBack = _c4;
		_contextCheckBorder = Color.FromArgb(242, 149, 54);
		_contextCheckTick = Color.FromArgb(66, 75, 138);
		_statusStripBorderDark = Color.FromArgb(86, 125, 176);
		_statusStripBorderLight = Color.White;
		_gripDark = Color.FromArgb(114, 152, 204);
		_gripLight = _c5;
		_itemContextItemEnabledColors = new GradientItemColors(_r1, _r2, _r3, _r4, _r5, _r6, _r7, _r8, Color.FromArgb(217, 203, 150), Color.FromArgb(192, 167, 118));
		_itemDisabledColors = new GradientItemColors(_c4, _c6, Color.FromArgb(236, 236, 236), Color.FromArgb(230, 230, 230), _c6, Color.FromArgb(224, 224, 224), Color.FromArgb(200, 200, 200), Color.FromArgb(210, 210, 210), Color.FromArgb(212, 212, 212), Color.FromArgb(195, 195, 195));
		_itemToolItemSelectedColors = new GradientItemColors(_r1, _r2, _r3, _r4, _r5, _r6, _r7, _r8, _r9, _rA);
		_itemToolItemPressedColors = new GradientItemColors(_rD, _rE, _rF, _rG, _rH, _rI, _rJ, _rK, _r9, _rA);
		_itemToolItemCheckedColors = new GradientItemColors(_rL, _rM, _rN, _rO, _rP, _rQ, _rR, _rS, _r9, _rA);
		_itemToolItemCheckPressColors = new GradientItemColors(_rT, _rU, _rV, _rW, _rX, _rI, _rY, _rZ, _r9, _rA);
		_statusStripBlend = new Blend();
		_statusStripBlend.Positions = new float[6] { 0f, 0.25f, 0.25f, 0.57f, 0.86f, 1f };
		_statusStripBlend.Factors = new float[6] { 0.1f, 0.6f, 1f, 0.4f, 0f, 0.95f };
	}

	public Office2007Renderer()
		: base(new Office2007ColorTable())
	{
	}

	protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
	{
		if (e.ArrowRectangle.Width <= 0 || e.ArrowRectangle.Height <= 0)
		{
			return;
		}
		using GraphicsPath arrowPath = CreateArrowPath(e.Item, e.ArrowRectangle, e.Direction);
		RectangleF boundsF = arrowPath.GetBounds();
		boundsF.Inflate(1f, 1f);
		Color color1 = (e.Item.Enabled ? _arrowLight : _arrowDisabled);
		Color color2 = (e.Item.Enabled ? _arrowDark : _arrowDisabled);
		float angle = 0f;
		switch (e.Direction)
		{
		case ArrowDirection.Right:
			angle = 0f;
			break;
		case ArrowDirection.Left:
			angle = 180f;
			break;
		case ArrowDirection.Down:
			angle = 90f;
			break;
		case ArrowDirection.Up:
			angle = 270f;
			break;
		}
		using LinearGradientBrush arrowBrush = new LinearGradientBrush(boundsF, color1, color2, angle);
		e.Graphics.FillPath(arrowBrush, arrowPath);
	}

	protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
	{
		ToolStripButton button = (ToolStripButton)e.Item;
		if (button.Selected || button.Pressed || button.Checked)
		{
			RenderToolButtonBackground(e.Graphics, button, e.ToolStrip);
		}
	}

	protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
	{
		if (e.Item.Selected || e.Item.Pressed)
		{
			RenderToolDropButtonBackground(e.Graphics, e.Item, e.ToolStrip);
		}
	}

	protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
	{
		Rectangle checkBox = e.ImageRectangle;
		checkBox.Inflate(1, 1);
		if (checkBox.Top > _checkInset)
		{
			int diff = checkBox.Top - _checkInset;
			checkBox.Y -= diff;
			checkBox.Height += diff;
		}
		if (checkBox.Height <= e.Item.Bounds.Height - _checkInset * 2)
		{
			int diff2 = e.Item.Bounds.Height - _checkInset * 2 - checkBox.Height;
			checkBox.Height += diff2;
		}
		using (new UseAntiAlias(e.Graphics))
		{
			using GraphicsPath borderPath = CreateBorderPath(checkBox, _cutMenuItemBack);
			using (SolidBrush fillBrush = new SolidBrush(base.ColorTable.CheckBackground))
			{
				e.Graphics.FillPath(fillBrush, borderPath);
			}
			using (Pen borderPen = new Pen(_contextCheckBorder))
			{
				e.Graphics.DrawPath(borderPen, borderPath);
			}
			if (e.Image == null)
			{
				return;
			}
			CheckState checkState = CheckState.Unchecked;
			if (e.Item is ToolStripMenuItem)
			{
				checkState = ((ToolStripMenuItem)e.Item).CheckState;
			}
			switch (checkState)
			{
			case CheckState.Checked:
			{
				using GraphicsPath tickPath2 = CreateTickPath(checkBox);
				using Pen tickPen = new Pen(_contextCheckTick, _contextCheckTickThickness);
				e.Graphics.DrawPath(tickPen, tickPath2);
				break;
			}
			case CheckState.Indeterminate:
			{
				using GraphicsPath tickPath = CreateIndeterminatePath(checkBox);
				using SolidBrush tickBrush = new SolidBrush(_contextCheckTick);
				e.Graphics.FillPath(tickBrush, tickPath);
				break;
			}
			}
		}
	}

	protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
	{
		if (e.ToolStrip is MenuStrip || e.ToolStrip != null || e.ToolStrip is ContextMenuStrip || e.ToolStrip is ToolStripDropDownMenu)
		{
			if (!e.Item.Enabled)
			{
				e.TextColor = _textDisabled;
			}
			else if (e.ToolStrip is MenuStrip && !e.Item.Pressed && !e.Item.Selected)
			{
				e.TextColor = _textMenuStripItem;
			}
			else if (e.ToolStrip is StatusStrip && !e.Item.Pressed && !e.Item.Selected)
			{
				e.TextColor = _textStatusStripItem;
			}
			else
			{
				e.TextColor = _textContextMenuItem;
			}
			using (new UseClearTypeGridFit(e.Graphics))
			{
				base.OnRenderItemText(e);
				return;
			}
		}
		base.OnRenderItemText(e);
	}

	protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
	{
		if (e.ToolStrip is ContextMenuStrip || e.ToolStrip is ToolStripDropDownMenu)
		{
			if (e.Image != null)
			{
				if (e.Item.Enabled)
				{
					e.Graphics.DrawImage(e.Image, e.ImageRectangle);
				}
				else
				{
					ControlPaint.DrawImageDisabled(e.Graphics, e.Image, e.ImageRectangle.X, e.ImageRectangle.Y, Color.Transparent);
				}
			}
		}
		else
		{
			base.OnRenderItemImage(e);
		}
	}

	protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
	{
		if (e.ToolStrip is MenuStrip || e.ToolStrip is ContextMenuStrip || e.ToolStrip is ToolStripDropDownMenu)
		{
			if (e.Item.Pressed && e.ToolStrip is MenuStrip)
			{
				DrawContextMenuHeader(e.Graphics, e.Item);
			}
			else
			{
				if (!e.Item.Selected)
				{
					return;
				}
				if (e.Item.Enabled)
				{
					if (e.ToolStrip is MenuStrip)
					{
						DrawGradientToolItem(e.Graphics, e.Item, _itemToolItemSelectedColors);
					}
					else
					{
						DrawGradientContextMenuItem(e.Graphics, e.Item, _itemContextItemEnabledColors);
					}
					return;
				}
				Point mousePos = e.ToolStrip.PointToClient(Control.MousePosition);
				if (!e.Item.Bounds.Contains(mousePos))
				{
					if (e.ToolStrip is MenuStrip)
					{
						DrawGradientToolItem(e.Graphics, e.Item, _itemDisabledColors);
					}
					else
					{
						DrawGradientContextMenuItem(e.Graphics, e.Item, _itemDisabledColors);
					}
				}
			}
		}
		else
		{
			base.OnRenderMenuItemBackground(e);
		}
	}

	protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
	{
		if (e.Item.Selected || e.Item.Pressed)
		{
			ToolStripSplitButton splitButton = (ToolStripSplitButton)e.Item;
			RenderToolSplitButtonBackground(e.Graphics, splitButton, e.ToolStrip);
			Rectangle arrowBounds = splitButton.DropDownButtonBounds;
			OnRenderArrow(new ToolStripArrowRenderEventArgs(e.Graphics, splitButton, arrowBounds, SystemColors.ControlText, ArrowDirection.Down));
		}
		else
		{
			base.OnRenderSplitButtonBackground(e);
		}
	}

	protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
	{
		using SolidBrush darkBrush = new SolidBrush(_gripDark);
		using SolidBrush lightBrush = new SolidBrush(_gripLight);
		bool rtl = e.ToolStrip.RightToLeft == RightToLeft.Yes;
		int y = e.AffectedBounds.Bottom - _gripSize * 2 + 1;
		for (int i = _gripLines; i >= 1; i--)
		{
			int x = (rtl ? (e.AffectedBounds.Left + 1) : (e.AffectedBounds.Right - _gripSize * 2 + 1));
			for (int j = 0; j < i; j++)
			{
				DrawGripGlyph(e.Graphics, x, y, darkBrush, lightBrush);
				x -= (rtl ? (-_gripMove) : _gripMove);
			}
			y -= _gripMove;
		}
	}

	protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
	{
		base.OnRenderToolStripContentPanelBackground(e);
		if (e.ToolStripContentPanel.Width > 0 && e.ToolStripContentPanel.Height > 0)
		{
			using (LinearGradientBrush backBrush = new LinearGradientBrush(e.ToolStripContentPanel.ClientRectangle, base.ColorTable.ToolStripContentPanelGradientEnd, base.ColorTable.ToolStripContentPanelGradientBegin, 90f))
			{
				e.Graphics.FillRectangle(backBrush, e.ToolStripContentPanel.ClientRectangle);
			}
		}
	}

	protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
	{
		if (e.ToolStrip is ContextMenuStrip || e.ToolStrip is ToolStripDropDownMenu)
		{
			using (Pen lightPen = new Pen(_separatorMenuLight))
			{
				using Pen darkPen = new Pen(_separatorMenuDark);
				DrawSeparator(e.Graphics, e.Vertical, e.Item.Bounds, lightPen, darkPen, _separatorInset, e.ToolStrip.RightToLeft == RightToLeft.Yes);
				return;
			}
		}
		if (e.ToolStrip is StatusStrip)
		{
			using (Pen lightPen2 = new Pen(base.ColorTable.SeparatorLight))
			{
				using Pen darkPen2 = new Pen(base.ColorTable.SeparatorDark);
				DrawSeparator(e.Graphics, e.Vertical, e.Item.Bounds, lightPen2, darkPen2, 0, rtl: false);
				return;
			}
		}
		base.OnRenderSeparator(e);
	}

	protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
	{
		if (e.ToolStrip is ContextMenuStrip || e.ToolStrip is ToolStripDropDownMenu)
		{
			using (GraphicsPath borderPath = CreateBorderPath(e.AffectedBounds, _cutContextMenu))
			{
				using GraphicsPath clipPath = CreateClipBorderPath(e.AffectedBounds, _cutContextMenu);
				using (new UseClipping(e.Graphics, clipPath))
				{
					using SolidBrush backBrush = new SolidBrush(_contextMenuBack);
					e.Graphics.FillPath(backBrush, borderPath);
					return;
				}
			}
		}
		if (e.ToolStrip is StatusStrip)
		{
			RectangleF backRect = new RectangleF(0f, 1.5f, e.ToolStrip.Width, e.ToolStrip.Height - 2);
			if (backRect.Width > 0f && backRect.Height > 0f)
			{
				using (LinearGradientBrush backBrush2 = new LinearGradientBrush(backRect, base.ColorTable.StatusStripGradientBegin, base.ColorTable.StatusStripGradientEnd, 90f))
				{
					backBrush2.Blend = _statusStripBlend;
					e.Graphics.FillRectangle(backBrush2, backRect);
				}
			}
		}
		else
		{
			base.OnRenderToolStripBackground(e);
		}
	}

	protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
	{
		if (e.ToolStrip is ContextMenuStrip || e.ToolStrip is ToolStripDropDownMenu)
		{
			Rectangle marginRect = e.AffectedBounds;
			bool rtl = e.ToolStrip.RightToLeft == RightToLeft.Yes;
			marginRect.Y += _marginInset;
			marginRect.Height -= _marginInset * 2;
			if (!rtl)
			{
				marginRect.X += _marginInset;
			}
			else
			{
				marginRect.X += _marginInset / 2;
			}
			using (SolidBrush backBrush = new SolidBrush(base.ColorTable.ImageMarginGradientBegin))
			{
				e.Graphics.FillRectangle(backBrush, marginRect);
			}
			using Pen lightPen = new Pen(_separatorMenuLight);
			using Pen darkPen = new Pen(_separatorMenuDark);
			if (!rtl)
			{
				e.Graphics.DrawLine(lightPen, marginRect.Right, marginRect.Top, marginRect.Right, marginRect.Bottom);
				e.Graphics.DrawLine(darkPen, marginRect.Right - 1, marginRect.Top, marginRect.Right - 1, marginRect.Bottom);
			}
			else
			{
				e.Graphics.DrawLine(lightPen, marginRect.Left - 1, marginRect.Top, marginRect.Left - 1, marginRect.Bottom);
				e.Graphics.DrawLine(darkPen, marginRect.Left, marginRect.Top, marginRect.Left, marginRect.Bottom);
			}
			return;
		}
		base.OnRenderImageMargin(e);
	}

	protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
	{
		if (e.ToolStrip is ContextMenuStrip || e.ToolStrip is ToolStripDropDownMenu)
		{
			if (!e.ConnectedArea.IsEmpty)
			{
				using SolidBrush excludeBrush = new SolidBrush(_contextMenuBack);
				e.Graphics.FillRectangle(excludeBrush, e.ConnectedArea);
			}
			using GraphicsPath borderPath = CreateBorderPath(e.AffectedBounds, e.ConnectedArea, _cutContextMenu);
			using GraphicsPath insidePath = CreateInsideBorderPath(e.AffectedBounds, e.ConnectedArea, _cutContextMenu);
			using GraphicsPath clipPath = CreateClipBorderPath(e.AffectedBounds, e.ConnectedArea, _cutContextMenu);
			using Pen borderPen = new Pen(base.ColorTable.MenuBorder);
			using Pen insidePen = new Pen(_separatorMenuLight);
			using (new UseClipping(e.Graphics, clipPath))
			{
				using (new UseAntiAlias(e.Graphics))
				{
					e.Graphics.DrawPath(insidePen, insidePath);
					e.Graphics.DrawPath(borderPen, borderPath);
				}
				e.Graphics.DrawLine(borderPen, e.AffectedBounds.Right, e.AffectedBounds.Bottom, e.AffectedBounds.Right - 1, e.AffectedBounds.Bottom - 1);
				return;
			}
		}
		if (e.ToolStrip is StatusStrip)
		{
			using (Pen darkBorder = new Pen(_statusStripBorderDark))
			{
				using Pen lightBorder = new Pen(_statusStripBorderLight);
				e.Graphics.DrawLine(darkBorder, 0, 0, e.ToolStrip.Width, 0);
				e.Graphics.DrawLine(lightBorder, 0, 1, e.ToolStrip.Width, 1);
				return;
			}
		}
		base.OnRenderToolStripBorder(e);
	}

	private void RenderToolButtonBackground(Graphics g, ToolStripButton button, ToolStrip toolstrip)
	{
		if (button.Enabled)
		{
			if (button.Checked)
			{
				if (button.Pressed)
				{
					DrawGradientToolItem(g, button, _itemToolItemPressedColors);
				}
				else if (button.Selected)
				{
					DrawGradientToolItem(g, button, _itemToolItemCheckPressColors);
				}
				else
				{
					DrawGradientToolItem(g, button, _itemToolItemCheckedColors);
				}
			}
			else if (button.Pressed)
			{
				DrawGradientToolItem(g, button, _itemToolItemPressedColors);
			}
			else if (button.Selected)
			{
				DrawGradientToolItem(g, button, _itemToolItemSelectedColors);
			}
		}
		else if (button.Selected)
		{
			Point mousePos = toolstrip.PointToClient(Control.MousePosition);
			if (!button.Bounds.Contains(mousePos))
			{
				DrawGradientToolItem(g, button, _itemDisabledColors);
			}
		}
	}

	private void RenderToolDropButtonBackground(Graphics g, ToolStripItem item, ToolStrip toolstrip)
	{
		if (!item.Selected && !item.Pressed)
		{
			return;
		}
		if (item.Enabled)
		{
			if (item.Pressed)
			{
				DrawContextMenuHeader(g, item);
			}
			else
			{
				DrawGradientToolItem(g, item, _itemToolItemSelectedColors);
			}
			return;
		}
		Point mousePos = toolstrip.PointToClient(Control.MousePosition);
		if (!item.Bounds.Contains(mousePos))
		{
			DrawGradientToolItem(g, item, _itemDisabledColors);
		}
	}

	private void RenderToolSplitButtonBackground(Graphics g, ToolStripSplitButton splitButton, ToolStrip toolstrip)
	{
		if (!splitButton.Selected && !splitButton.Pressed)
		{
			return;
		}
		if (splitButton.Enabled)
		{
			if (!splitButton.Pressed && splitButton.ButtonPressed)
			{
				DrawGradientToolSplitItem(g, splitButton, _itemToolItemPressedColors, _itemToolItemSelectedColors, _itemContextItemEnabledColors);
			}
			else if (splitButton.Pressed && !splitButton.ButtonPressed)
			{
				DrawContextMenuHeader(g, splitButton);
			}
			else
			{
				DrawGradientToolSplitItem(g, splitButton, _itemToolItemSelectedColors, _itemToolItemSelectedColors, _itemContextItemEnabledColors);
			}
		}
		else
		{
			Point mousePos = toolstrip.PointToClient(Control.MousePosition);
			if (!splitButton.Bounds.Contains(mousePos))
			{
				DrawGradientToolItem(g, splitButton, _itemDisabledColors);
			}
		}
	}

	private void DrawGradientToolItem(Graphics g, ToolStripItem item, GradientItemColors colors)
	{
		DrawGradientItem(g, new Rectangle(Point.Empty, item.Bounds.Size), colors);
	}

	private void DrawGradientToolSplitItem(Graphics g, ToolStripSplitButton splitButton, GradientItemColors colorsButton, GradientItemColors colorsDrop, GradientItemColors colorsSplit)
	{
		Rectangle backRect = new Rectangle(Point.Empty, splitButton.Bounds.Size);
		Rectangle backRectDrop = splitButton.DropDownButtonBounds;
		if (backRect.Width <= 0 || backRectDrop.Width <= 0 || backRect.Height <= 0 || backRectDrop.Height <= 0)
		{
			return;
		}
		Rectangle backRectButton = backRect;
		int splitOffset;
		if (backRectDrop.X > 0)
		{
			backRectButton.Width = backRectDrop.Left;
			backRectDrop.X--;
			backRectDrop.Width++;
			splitOffset = backRectDrop.X;
		}
		else
		{
			backRectButton.Width -= backRectDrop.Width - 2;
			backRectButton.X = backRectDrop.Right - 1;
			backRectDrop.Width++;
			splitOffset = backRectDrop.Right - 1;
		}
		using (CreateBorderPath(backRect, _cutMenuItemBack))
		{
			DrawGradientBack(g, backRectButton, colorsButton);
			DrawGradientBack(g, backRectDrop, colorsDrop);
			using (LinearGradientBrush splitBrush = new LinearGradientBrush(new Rectangle(backRect.X + splitOffset, backRect.Top, 1, backRect.Height + 1), colorsSplit.Border1, colorsSplit.Border2, 90f))
			{
				splitBrush.SetSigmaBellShape(0.5f);
				using Pen splitPen = new Pen(splitBrush);
				g.DrawLine(splitPen, backRect.X + splitOffset, backRect.Top + 1, backRect.X + splitOffset, backRect.Bottom - 1);
			}
			DrawGradientBorder(g, backRect, colorsButton);
		}
	}

	private void DrawContextMenuHeader(Graphics g, ToolStripItem item)
	{
		Rectangle itemRect = new Rectangle(Point.Empty, item.Bounds.Size);
		using GraphicsPath borderPath = CreateBorderPath(itemRect, _cutToolItemMenu);
		using (CreateInsideBorderPath(itemRect, _cutToolItemMenu))
		{
			using GraphicsPath clipPath = CreateClipBorderPath(itemRect, _cutToolItemMenu);
			using (new UseClipping(g, clipPath))
			{
				using (SolidBrush backBrush = new SolidBrush(_separatorMenuLight))
				{
					g.FillPath(backBrush, borderPath);
				}
				using Pen borderPen = new Pen(base.ColorTable.MenuBorder);
				g.DrawPath(borderPen, borderPath);
			}
		}
	}

	private void DrawGradientContextMenuItem(Graphics g, ToolStripItem item, GradientItemColors colors)
	{
		Rectangle backRect = new Rectangle(2, 0, item.Bounds.Width - 3, item.Bounds.Height);
		DrawGradientItem(g, backRect, colors);
	}

	private void DrawGradientItem(Graphics g, Rectangle backRect, GradientItemColors colors)
	{
		if (backRect.Width > 0 && backRect.Height > 0)
		{
			DrawGradientBack(g, backRect, colors);
			DrawGradientBorder(g, backRect, colors);
		}
	}

	private void DrawGradientBack(Graphics g, Rectangle backRect, GradientItemColors colors)
	{
		backRect.Inflate(-1, -1);
		int y2 = backRect.Height / 2;
		Rectangle backRect2 = new Rectangle(backRect.X, backRect.Y, backRect.Width, y2);
		Rectangle backRect3 = new Rectangle(backRect.X, backRect.Y + y2, backRect.Width, backRect.Height - y2);
		Rectangle backRect1I = backRect2;
		Rectangle backRect2I = backRect3;
		backRect1I.Inflate(1, 1);
		backRect2I.Inflate(1, 1);
		using (LinearGradientBrush insideBrush1 = new LinearGradientBrush(backRect1I, colors.InsideTop1, colors.InsideTop2, 90f))
		{
			using LinearGradientBrush insideBrush2 = new LinearGradientBrush(backRect2I, colors.InsideBottom1, colors.InsideBottom2, 90f);
			g.FillRectangle(insideBrush1, backRect2);
			g.FillRectangle(insideBrush2, backRect3);
		}
		y2 = backRect.Height / 2;
		backRect2 = new Rectangle(backRect.X, backRect.Y, backRect.Width, y2);
		backRect3 = new Rectangle(backRect.X, backRect.Y + y2, backRect.Width, backRect.Height - y2);
		backRect1I = backRect2;
		backRect2I = backRect3;
		backRect1I.Inflate(1, 1);
		backRect2I.Inflate(1, 1);
		using LinearGradientBrush fillBrush1 = new LinearGradientBrush(backRect1I, colors.FillTop1, colors.FillTop2, 90f);
		using LinearGradientBrush fillBrush2 = new LinearGradientBrush(backRect2I, colors.FillBottom1, colors.FillBottom2, 90f);
		backRect.Inflate(-1, -1);
		y2 = backRect.Height / 2;
		backRect2 = new Rectangle(backRect.X, backRect.Y, backRect.Width, y2);
		backRect3 = new Rectangle(backRect.X, backRect.Y + y2, backRect.Width, backRect.Height - y2);
		g.FillRectangle(fillBrush1, backRect2);
		g.FillRectangle(fillBrush2, backRect3);
	}

	private void DrawGradientBorder(Graphics g, Rectangle backRect, GradientItemColors colors)
	{
		using (new UseAntiAlias(g))
		{
			Rectangle backRectI = backRect;
			backRectI.Inflate(1, 1);
			using LinearGradientBrush borderBrush = new LinearGradientBrush(backRectI, colors.Border1, colors.Border2, 90f);
			borderBrush.SetSigmaBellShape(0.5f);
			using Pen borderPen = new Pen(borderBrush);
			using GraphicsPath borderPath = CreateBorderPath(backRect, _cutMenuItemBack);
			g.DrawPath(borderPen, borderPath);
		}
	}

	private void DrawGripGlyph(Graphics g, int x, int y, Brush darkBrush, Brush lightBrush)
	{
		g.FillRectangle(lightBrush, x + _gripOffset, y + _gripOffset, _gripSquare, _gripSquare);
		g.FillRectangle(darkBrush, x, y, _gripSquare, _gripSquare);
	}

	private void DrawSeparator(Graphics g, bool vertical, Rectangle rect, Pen lightPen, Pen darkPen, int horizontalInset, bool rtl)
	{
		if (vertical)
		{
			int l = rect.Width / 2;
			int t = rect.Y;
			int b = rect.Bottom;
			g.DrawLine(darkPen, l, t, l, b);
			g.DrawLine(lightPen, l + 1, t, l + 1, b);
		}
		else
		{
			int y = rect.Height / 2;
			int l2 = rect.X + ((!rtl) ? horizontalInset : 0);
			int r = rect.Right - (rtl ? horizontalInset : 0);
			g.DrawLine(darkPen, l2, y, r, y);
			g.DrawLine(lightPen, l2, y + 1, r, y + 1);
		}
	}

	private GraphicsPath CreateBorderPath(Rectangle rect, Rectangle exclude, float cut)
	{
		if (exclude.IsEmpty)
		{
			return CreateBorderPath(rect, cut);
		}
		rect.Width--;
		rect.Height--;
		List<PointF> pts = new List<PointF>();
		float l = rect.X;
		float t = rect.Y;
		float r = rect.Right;
		float b = rect.Bottom;
		float x0 = (float)rect.X + cut;
		float x3 = (float)rect.Right - cut;
		float y0 = (float)rect.Y + cut;
		float y3 = (float)rect.Bottom - cut;
		float cutBack = ((cut == 0f) ? 1f : cut);
		if (rect.Y >= exclude.Top && rect.Y <= exclude.Bottom)
		{
			float x4 = (float)(exclude.X - 1) - cut;
			float x5 = (float)exclude.Right + cut;
			if (x0 <= x4)
			{
				pts.Add(new PointF(x0, t));
				pts.Add(new PointF(x4, t));
				pts.Add(new PointF(x4 + cut, t - cutBack));
			}
			else
			{
				x4 = exclude.X - 1;
				pts.Add(new PointF(x4, t));
				pts.Add(new PointF(x4, t - cutBack));
			}
			if (x3 > x5)
			{
				pts.Add(new PointF(x5 - cut, t - cutBack));
				pts.Add(new PointF(x5, t));
				pts.Add(new PointF(x3, t));
			}
			else
			{
				x5 = exclude.Right;
				pts.Add(new PointF(x5, t - cutBack));
				pts.Add(new PointF(x5, t));
			}
		}
		else
		{
			pts.Add(new PointF(x0, t));
			pts.Add(new PointF(x3, t));
		}
		pts.Add(new PointF(r, y0));
		pts.Add(new PointF(r, y3));
		pts.Add(new PointF(x3, b));
		pts.Add(new PointF(x0, b));
		pts.Add(new PointF(l, y3));
		pts.Add(new PointF(l, y0));
		GraphicsPath path = new GraphicsPath();
		for (int i = 1; i < pts.Count; i++)
		{
			path.AddLine(pts[i - 1], pts[i]);
		}
		path.AddLine(pts[pts.Count - 1], pts[0]);
		return path;
	}

	private GraphicsPath CreateBorderPath(Rectangle rect, float cut)
	{
		rect.Width--;
		rect.Height--;
		GraphicsPath graphicsPath = new GraphicsPath();
		graphicsPath.AddLine((float)rect.Left + cut, rect.Top, (float)rect.Right - cut, rect.Top);
		graphicsPath.AddLine((float)rect.Right - cut, rect.Top, rect.Right, (float)rect.Top + cut);
		graphicsPath.AddLine(rect.Right, (float)rect.Top + cut, rect.Right, (float)rect.Bottom - cut);
		graphicsPath.AddLine(rect.Right, (float)rect.Bottom - cut, (float)rect.Right - cut, rect.Bottom);
		graphicsPath.AddLine((float)rect.Right - cut, rect.Bottom, (float)rect.Left + cut, rect.Bottom);
		graphicsPath.AddLine((float)rect.Left + cut, rect.Bottom, rect.Left, (float)rect.Bottom - cut);
		graphicsPath.AddLine(rect.Left, (float)rect.Bottom - cut, rect.Left, (float)rect.Top + cut);
		graphicsPath.AddLine(rect.Left, (float)rect.Top + cut, (float)rect.Left + cut, rect.Top);
		return graphicsPath;
	}

	private GraphicsPath CreateInsideBorderPath(Rectangle rect, float cut)
	{
		rect.Inflate(-1, -1);
		return CreateBorderPath(rect, cut);
	}

	private GraphicsPath CreateInsideBorderPath(Rectangle rect, Rectangle exclude, float cut)
	{
		rect.Inflate(-1, -1);
		return CreateBorderPath(rect, exclude, cut);
	}

	private GraphicsPath CreateClipBorderPath(Rectangle rect, float cut)
	{
		rect.Width++;
		rect.Height++;
		return CreateBorderPath(rect, cut);
	}

	private GraphicsPath CreateClipBorderPath(Rectangle rect, Rectangle exclude, float cut)
	{
		rect.Width++;
		rect.Height++;
		return CreateBorderPath(rect, exclude, cut);
	}

	private GraphicsPath CreateArrowPath(ToolStripItem item, Rectangle rect, ArrowDirection direction)
	{
		int x;
		int y;
		if (direction == ArrowDirection.Left || direction == ArrowDirection.Right)
		{
			x = rect.Right - (rect.Width - 4) / 2;
			y = rect.Y + rect.Height / 2;
		}
		else
		{
			x = rect.X + rect.Width / 2;
			y = rect.Bottom - (rect.Height - 3) / 2;
			if (item is ToolStripDropDownButton && item.RightToLeft == RightToLeft.Yes)
			{
				x++;
			}
		}
		GraphicsPath path = new GraphicsPath();
		switch (direction)
		{
		case ArrowDirection.Right:
			path.AddLine(x, y, x - 4, y - 4);
			path.AddLine(x - 4, y - 4, x - 4, y + 4);
			path.AddLine(x - 4, y + 4, x, y);
			break;
		case ArrowDirection.Left:
			path.AddLine(x - 4, y, x, y - 4);
			path.AddLine(x, y - 4, x, y + 4);
			path.AddLine(x, y + 4, x - 4, y);
			break;
		case ArrowDirection.Down:
			path.AddLine((float)x + 3f, (float)y - 3f, (float)x - 2f, (float)y - 3f);
			path.AddLine((float)x - 2f, (float)y - 3f, x, y);
			path.AddLine(x, y, (float)x + 3f, (float)y - 3f);
			break;
		case ArrowDirection.Up:
			path.AddLine((float)x + 3f, y, (float)x - 3f, y);
			path.AddLine((float)x - 3f, y, x, (float)y - 4f);
			path.AddLine(x, (float)y - 4f, (float)x + 3f, y);
			break;
		}
		return path;
	}

	private GraphicsPath CreateTickPath(Rectangle rect)
	{
		int x = rect.X + rect.Width / 2;
		int y = rect.Y + rect.Height / 2;
		GraphicsPath graphicsPath = new GraphicsPath();
		graphicsPath.AddLine(x - 4, y, x - 2, y + 4);
		graphicsPath.AddLine(x - 2, y + 4, x + 3, y - 5);
		return graphicsPath;
	}

	private GraphicsPath CreateIndeterminatePath(Rectangle rect)
	{
		int x = rect.X + rect.Width / 2;
		int y = rect.Y + rect.Height / 2;
		GraphicsPath graphicsPath = new GraphicsPath();
		graphicsPath.AddLine(x - 3, y, x, y - 3);
		graphicsPath.AddLine(x, y - 3, x + 3, y);
		graphicsPath.AddLine(x + 3, y, x, y + 3);
		graphicsPath.AddLine(x, y + 3, x - 3, y);
		return graphicsPath;
	}
}
