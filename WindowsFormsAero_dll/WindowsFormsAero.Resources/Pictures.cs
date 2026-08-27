using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace WindowsFormsAero.Resources;

/// <summary>
///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
/// </summary>
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
public class Pictures
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	/// <summary>
	///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("WindowsFormsAero.Resources.Pictures", typeof(Pictures).Assembly);
			}
			return resourceMan;
		}
	}

	/// <summary>
	///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
	///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
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

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	public static Bitmap ActiveSearch => (Bitmap)ResourceManager.GetObject("ActiveSearch", resourceCulture);

	/// <summary>
	///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
	/// </summary>
	public static Bitmap InactiveSearch => (Bitmap)ResourceManager.GetObject("InactiveSearch", resourceCulture);

	internal Pictures()
	{
	}
}
