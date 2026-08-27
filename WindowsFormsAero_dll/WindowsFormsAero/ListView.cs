using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

[ToolboxBitmap(typeof(ListView))]
public class ListView : System.Windows.Forms.ListView
{
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		if (Environment.OSVersion.Version.Major >= 6)
		{
			Methods.SetWindowTheme(base.Handle, "explorer", null);
			Methods.SendMessage(base.Handle, 4150u, 65536u, 65536u);
		}
	}
}
