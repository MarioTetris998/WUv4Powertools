using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
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
internal class ExceptionMessages
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	/// <summary>
	///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("WindowsFormsAero.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
			}
			return resourceMan;
		}
	}

	/// <summary>
	///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
	///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
	/// </summary>
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

	/// <summary>
	///   Busca una cadena traducida similar a Common Controls library version 6.0 not loaded. Application must run on Vista and must provide a manifest..
	/// </summary>
	internal static string CommonControlEntryPointNotFound => ResourceManager.GetString("CommonControlEntryPointNotFound", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Unable to cloak target window..
	/// </summary>
	internal static string DwmCloakFail => ResourceManager.GetString("DwmCloakFail", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Unable to disallow Aero Peek for target window..
	/// </summary>
	internal static string DwmDisallowPeekFail => ResourceManager.GetString("DwmDisallowPeekFail", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Unable to exclude window from Aero Peek..
	/// </summary>
	internal static string DwmExcludePeekFail => ResourceManager.GetString("DwmExcludePeekFail", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Unable to change window Flip 3D policy..
	/// </summary>
	internal static string DwmFlip3dFailPolicy => ResourceManager.GetString("DwmFlip3dFailPolicy", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Unable to set DWM frozen representation..
	/// </summary>
	internal static string DwmFreezeRepresentationFail => ResourceManager.GetString("DwmFreezeRepresentationFail", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Desktop composition is not enabled..
	/// </summary>
	internal static string DwmNotEnabled => ResourceManager.GetString("DwmNotEnabled", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Desktop composition is not supported by operating system..
	/// </summary>
	internal static string DwmOsNotSupported => ResourceManager.GetString("DwmOsNotSupported", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Unable to get thumbnail's window size..
	/// </summary>
	internal static string DwmThumbnailQueryFailure => ResourceManager.GetString("DwmThumbnailQueryFailure", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Thumbnail source rectangle cannot have null or negative size..
	/// </summary>
	internal static string DwmThumbnailSourceInvalid => ResourceManager.GetString("DwmThumbnailSourceInvalid", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Unable to update thumbnail properties..
	/// </summary>
	internal static string DwmThumbnailUpdateFailure => ResourceManager.GetString("DwmThumbnailUpdateFailure", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Source and target windows cannot be the same..
	/// </summary>
	internal static string DwmWindowMatch => ResourceManager.GetString("DwmWindowMatch", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Native call to {0} failed..
	/// </summary>
	internal static string NativeCallFailure => ResourceManager.GetString("NativeCallFailure", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Failed to create Task Dialog..
	/// </summary>
	internal static string TaskDialogFailure => ResourceManager.GetString("TaskDialogFailure", resourceCulture);

	internal ExceptionMessages()
	{
	}
}
