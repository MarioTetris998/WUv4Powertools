using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace MdiTabControl;

[StandardModule]
internal sealed class Helper
{
	internal class Colors
	{
		public Color BackHighColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => Color.Transparent, 
				ToolStripRenderMode.Professional => ProfessionalColors.ToolStripGradientEnd, 
				_ => ManagedColor, 
			};
		}

		public Color BackLowColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => Color.Transparent, 
				ToolStripRenderMode.Professional => ProfessionalColors.ToolStripGradientBegin, 
				_ => ManagedColor, 
			};
		}

		public Color BorderColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.ControlDarkDark, 
				ToolStripRenderMode.Professional => ProfessionalColors.GripDark, 
				_ => ManagedColor, 
			};
		}

		public Color BorderColorDisabled(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.ControlDark, 
				ToolStripRenderMode.Professional => ProfessionalColors.SeparatorDark, 
				_ => ManagedColor, 
			};
		}

		public Color ControlButtonBackHighColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.ButtonHighlight, 
				ToolStripRenderMode.Professional => ProfessionalColors.ButtonSelectedGradientBegin, 
				_ => ManagedColor, 
			};
		}

		public Color ControlButtonBackLowColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.ButtonHighlight, 
				ToolStripRenderMode.Professional => ProfessionalColors.ButtonSelectedGradientEnd, 
				_ => ManagedColor, 
			};
		}

		public Color ControlButtonBorderColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.HotTrack, 
				ToolStripRenderMode.Professional => ProfessionalColors.ButtonPressedBorder, 
				_ => ManagedColor, 
			};
		}

		public Color TabBackHighColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.Control, 
				ToolStripRenderMode.Professional => ProfessionalColors.MenuItemPressedGradientBegin, 
				_ => ManagedColor, 
			};
		}

		public Color TabBackLowColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.Control, 
				ToolStripRenderMode.Professional => ProfessionalColors.MenuItemPressedGradientEnd, 
				_ => ManagedColor, 
			};
		}

		public Color TabBackHighColorDisabled(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.Control, 
				ToolStripRenderMode.Professional => ProfessionalColors.ToolStripDropDownBackground, 
				_ => ManagedColor, 
			};
		}

		public Color TabBackLowColorDisabled(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.Control, 
				ToolStripRenderMode.Professional => ProfessionalColors.ToolStripGradientMiddle, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonBackHighColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => Color.Transparent, 
				ToolStripRenderMode.Professional => Color.Transparent, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonBackHighColorDisabled(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => Color.Transparent, 
				ToolStripRenderMode.Professional => Color.Transparent, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonBackHighColorHot(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.ButtonHighlight, 
				ToolStripRenderMode.Professional => Color.WhiteSmoke, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonBackLowColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => Color.Transparent, 
				ToolStripRenderMode.Professional => Color.Transparent, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonBackLowColorDisabled(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => Color.Transparent, 
				ToolStripRenderMode.Professional => Color.Transparent, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonBackLowColorHot(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.ButtonHighlight, 
				ToolStripRenderMode.Professional => Color.LightGray, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonBorderColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.ControlDark, 
				ToolStripRenderMode.Professional => Color.Transparent, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonBorderColorDisabled(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.GrayText, 
				ToolStripRenderMode.Professional => Color.Transparent, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonBorderColorHot(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.HotTrack, 
				ToolStripRenderMode.Professional => Color.Gray, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonForeColor(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.ControlText, 
				ToolStripRenderMode.Professional => Color.Gray, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonForeColorDisabled(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.GrayText, 
				ToolStripRenderMode.Professional => Color.Gray, 
				_ => ManagedColor, 
			};
		}

		public Color TabCloseButtonForeColorHot(ToolStripRenderMode RenderMode, Color ManagedColor)
		{
			return RenderMode switch
			{
				ToolStripRenderMode.System => SystemColors.ControlText, 
				ToolStripRenderMode.Professional => Color.Firebrick, 
				_ => ManagedColor, 
			};
		}
	}

	internal static Colors RenderColors = new Colors();

	internal static LinearGradientBrush CreateGlassGradientBrush(Rectangle Rectangle, Color Color1, Color Color2)
	{
		LinearGradientBrush linearGradientBrush = new LinearGradientBrush(Rectangle, Color1, Color2, LinearGradientMode.Vertical);
		Bitmap bitmap = new Bitmap(1, Rectangle.Height);
		Graphics graphics = Graphics.FromImage(bitmap);
		graphics.FillRectangle(linearGradientBrush, new Rectangle(0, 0, 1, Rectangle.Height));
		ColorBlend colorBlend = new ColorBlend(4);
		colorBlend.Colors[0] = bitmap.GetPixel(0, 0);
		checked
		{
			colorBlend.Colors[1] = bitmap.GetPixel(0, (int)Math.Round((double)bitmap.Height / 3.0));
			colorBlend.Colors[2] = bitmap.GetPixel(0, bitmap.Height - 1);
			colorBlend.Colors[3] = bitmap.GetPixel(0, (int)Math.Round((double)bitmap.Height / 3.0));
			colorBlend.Positions[0] = 0f;
			colorBlend.Positions[1] = 0.335f;
			colorBlend.Positions[2] = 0.335f;
			colorBlend.Positions[3] = 1f;
			linearGradientBrush.InterpolationColors = colorBlend;
			graphics.Dispose();
			bitmap.Dispose();
			return linearGradientBrush;
		}
	}
}
