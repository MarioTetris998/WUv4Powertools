using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using WindowsFormsAero.Native;
using WindowsFormsAero.Resources;

namespace WindowsFormsAero.Dwm;

/// <summary>
/// Handle to a DWM Thumbnail.
/// </summary>
/// <remarks>
/// Handles to <see cref="T:WindowsFormsAero.Dwm.Thumbnail" /> can be created only through
/// <see cref="T:WindowsFormsAero.Dwm.DwmManager" /> by registering a new thumbnail of an existing Form or
/// Win32 window. Thumbnails can be manipulated and should be disposed through this
/// class.
/// Thumbnails can be automatically handled by the <see cref="T:WindowsFormsAero.Dwm.ThumbnailViewer" />
/// Windows Forms control.
/// The <see cref="M:WindowsFormsAero.Dwm.Thumbnail.Update(System.Drawing.Rectangle,System.Byte,System.Boolean,System.Boolean)" /> or <see cref="M:WindowsFormsAero.Dwm.Thumbnail.Update(System.Drawing.Rectangle,System.Drawing.Rectangle,System.Byte,System.Boolean,System.Boolean)" /> methods must be called
/// at least once in order for the Thumbnail to be visible.
/// </remarks>
public sealed class Thumbnail : SafeHandle
{
	private byte _opacity = byte.MaxValue;

	private bool _clientArea;

	private Rectangle _destination;

	private bool _visible;

	public override bool IsInvalid
	{
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		get
		{
			if (!base.IsClosed)
			{
				return handle == IntPtr.Zero;
			}
			return true;
		}
	}

	/// <summary>
	/// Gets or sets the thumbnail opacity value, from 0 (transparent) to 255 (fully
	/// opaque).
	/// </summary>
	/// <remarks>
	/// This property appears to be ignored in Windows 10 Anniversary Update.
	/// </remarks>
	public byte Opacity
	{
		get
		{
			return _opacity;
		}
		set
		{
			DwmThumbnailProperties prop = new DwmThumbnailProperties
			{
				opacity = value,
				dwFlags = DwmThumbnailFlags.Opacity
			};
			if (DwmMethods.DwmUpdateThumbnailProperties(handle, ref prop) != 0)
			{
				throw new DwmCompositionException(ExceptionMessages.DwmThumbnailUpdateFailure);
			}
			_opacity = value;
		}
	}

	/// <summary>
	/// Gets or sets whether only the client area of the thumbnailed window should be
	/// shown or its entire window area.
	/// </summary>
	public bool ShowOnlyClientArea
	{
		get
		{
			return _clientArea;
		}
		set
		{
			DwmThumbnailProperties prop = new DwmThumbnailProperties
			{
				fSourceClientAreaOnly = value,
				dwFlags = DwmThumbnailFlags.SourceClientAreaOnly
			};
			if (DwmMethods.DwmUpdateThumbnailProperties(handle, ref prop) != 0)
			{
				throw new DwmCompositionException(ExceptionMessages.DwmThumbnailUpdateFailure);
			}
			_clientArea = value;
		}
	}

	/// <summary>
	/// Gets or sets the area in the destination window on which the thumbnail should
	/// be drawn.
	/// </summary>
	public Rectangle DestinationRectangle
	{
		get
		{
			return _destination;
		}
		set
		{
			DwmThumbnailProperties prop = new DwmThumbnailProperties
			{
				rcDestination = new Rect(value),
				dwFlags = DwmThumbnailFlags.RectDestination
			};
			if (DwmMethods.DwmUpdateThumbnailProperties(handle, ref prop) != 0)
			{
				throw new DwmCompositionException(ExceptionMessages.DwmThumbnailUpdateFailure);
			}
		}
	}

	/// <summary>
	/// Sets the region of the source window that should be drawn.
	/// </summary>
	/// <remarks>
	/// This read-only property cannot be unset once set.
	/// In order to reset the Thumbnail's source rectangle, a new  instance must be
	/// created.
	/// </remarks>
	public Rectangle SourceRectangle
	{
		set
		{
			if (value.Width < 1 || value.Height < 1)
			{
				throw new ArgumentException(ExceptionMessages.DwmThumbnailSourceInvalid);
			}
			DwmThumbnailProperties prop = new DwmThumbnailProperties
			{
				rcSource = new Rect(value),
				dwFlags = DwmThumbnailFlags.RectSource
			};
			if (DwmMethods.DwmUpdateThumbnailProperties(handle, ref prop) != 0)
			{
				throw new DwmCompositionException(ExceptionMessages.DwmThumbnailUpdateFailure);
			}
		}
	}

	/// <summary>
	/// Gets or sets whether the thumbnail should be shown or not.
	/// </summary>
	public bool Visible
	{
		get
		{
			return _visible;
		}
		set
		{
			DwmThumbnailProperties prop = new DwmThumbnailProperties
			{
				fVisible = value,
				dwFlags = DwmThumbnailFlags.Visible
			};
			if (DwmMethods.DwmUpdateThumbnailProperties(handle, ref prop) != 0)
			{
				throw new DwmCompositionException(ExceptionMessages.DwmThumbnailUpdateFailure);
			}
		}
	}

	internal Thumbnail()
		: base(IntPtr.Zero, ownsHandle: true)
	{
	}

	internal Thumbnail(IntPtr handle)
		: base(IntPtr.Zero, ownsHandle: true)
	{
		SetHandle(handle);
	}

	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	protected override bool ReleaseHandle()
	{
		if (handle == IntPtr.Zero)
		{
			return true;
		}
		return DwmMethods.DwmUnregisterThumbnail(handle) == 0;
	}

	/// <summary>
	/// Retrieves the thumbnailed window's size.
	/// </summary>
	public System.Drawing.Size GetSourceSize()
	{
		if (DwmMethods.DwmQueryThumbnailSourceSize(handle, out var size) != 0)
		{
			throw new DwmCompositionException(ExceptionMessages.DwmThumbnailQueryFailure);
		}
		return size.ToSize();
	}

	/// <summary>
	/// Updates the thumbnail's display settings.
	/// </summary>
	/// <param name="destination">Drawing region on destination window.</param>
	/// <param name="source">Origin region from source window.</param>
	/// <param name="opacity">Opacity. 0 is transparent, 255 opaque.</param>
	/// <param name="visible">Visibility flag.</param>
	/// <param name="onlyClientArea">
	/// If true, only the client area of the window will be rendered. Otherwise, the
	/// borders will be be rendered as well.
	/// </param>
	public void Update(Rectangle destination, Rectangle source, byte opacity, bool visible, bool onlyClientArea)
	{
		if (source.Width < 1 || source.Height < 1)
		{
			throw new ArgumentException(ExceptionMessages.DwmThumbnailSourceInvalid);
		}
		DwmThumbnailProperties prop = new DwmThumbnailProperties
		{
			rcDestination = new Rect(destination),
			rcSource = new Rect(source),
			opacity = opacity,
			fVisible = visible,
			fSourceClientAreaOnly = onlyClientArea,
			dwFlags = (DwmThumbnailFlags.RectDestination | DwmThumbnailFlags.RectSource | DwmThumbnailFlags.Opacity | DwmThumbnailFlags.Visible | DwmThumbnailFlags.SourceClientAreaOnly)
		};
		if (DwmMethods.DwmUpdateThumbnailProperties(handle, ref prop) != 0)
		{
			throw new DwmCompositionException(ExceptionMessages.DwmThumbnailUpdateFailure);
		}
		_destination = destination;
		_opacity = opacity;
		_visible = visible;
		_clientArea = ShowOnlyClientArea;
	}

	/// <summary>
	/// Updates the thumbnail's display settings.
	/// </summary>
	/// <param name="destination">Drawing region on destination window.</param>
	/// <param name="opacity">Opacity. 0 is transparent, 255 opaque.</param>
	/// <param name="visible">Visibility flag.</param>
	/// <param name="onlyClientArea">
	/// If true, only the client area of the window will be rendered. Otherwise, the
	/// borders will be be rendered as well.
	/// </param>
	public void Update(Rectangle destination, byte opacity, bool visible, bool onlyClientArea)
	{
		DwmThumbnailProperties prop = new DwmThumbnailProperties
		{
			rcDestination = new Rect(destination),
			opacity = opacity,
			fVisible = visible,
			fSourceClientAreaOnly = onlyClientArea,
			dwFlags = (DwmThumbnailFlags.RectDestination | DwmThumbnailFlags.Opacity | DwmThumbnailFlags.Visible | DwmThumbnailFlags.SourceClientAreaOnly)
		};
		if (DwmMethods.DwmUpdateThumbnailProperties(handle, ref prop) != 0)
		{
			throw new DwmCompositionException(ExceptionMessages.DwmThumbnailUpdateFailure);
		}
		_destination = destination;
		_opacity = opacity;
		_visible = visible;
		_clientArea = ShowOnlyClientArea;
	}
}
