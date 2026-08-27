using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

[ToolboxBitmap(typeof(TreeView))]
public class TreeView : System.Windows.Forms.TreeView
{
	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams obj = base.CreateParams;
			obj.Style |= 32768;
			return obj;
		}
	}

	[Browsable(false)]
	private new bool HotTracking
	{
		get
		{
			return base.HotTracking;
		}
		set
		{
		}
	}

	[Browsable(false)]
	private new bool ShowLines
	{
		get
		{
			return base.ShowLines;
		}
		set
		{
		}
	}

	public TreeView()
	{
		base.HotTracking = true;
		base.ShowLines = false;
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		if (Environment.OSVersion.Version.Major >= 6)
		{
			Methods.SetWindowTheme(base.Handle, "explorer", null);
			uint style = (uint)Methods.SendMessage(base.Handle, 4397u, 0, 0).ToInt64();
			style |= 0x20;
			style |= 0x40;
			style |= 4;
			Methods.SendMessage(base.Handle, 4396u, 0u, style);
		}
	}
}
