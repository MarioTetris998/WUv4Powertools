using System;
using System.Net;
using System.Windows.Forms;

namespace WUv4Powertools;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		// The framework this is built against asks for SSL 3.0 and TLS 1.0, which servers now
		// refuse outright, so checking a download link came back saying a secure channel could
		// not be created. The newer protocols are named by number because this framework
		// version has no names for them, and any the machine does not know is skipped.
		foreach (int protocol in new int[] { 12288, 3072, 768 })
		{
			try
			{
				ServicePointManager.SecurityProtocol |= (SecurityProtocolType)protocol;
			}
			catch (NotSupportedException)
			{
				// Not on this machine. The ones it does have are still set.
			}
		}

		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Application.Run(new frmMain());
	}
}
