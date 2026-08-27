using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace WUv4Powertools.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("WUv4Powertools.Properties.Resources", typeof(Resources).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static Bitmap _16 => (Bitmap)ResourceManager.GetObject("_16", resourceCulture);

	internal static Bitmap AddUpdate => (Bitmap)ResourceManager.GetObject("AddUpdate", resourceCulture);

	internal static Bitmap Convert1 => (Bitmap)ResourceManager.GetObject("Convert1", resourceCulture);

	internal static Bitmap EditUpdate => (Bitmap)ResourceManager.GetObject("EditUpdate", resourceCulture);

	internal static Icon ICO100 => (Icon)ResourceManager.GetObject("ICO100", resourceCulture);

	internal static Bitmap wButton => (Bitmap)ResourceManager.GetObject("wButton", resourceCulture);

	internal Resources()
	{
	}
}
