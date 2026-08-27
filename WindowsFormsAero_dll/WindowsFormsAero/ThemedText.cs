using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

/// <summary>
/// Renders themed text.
/// </summary>
/// <remarks>
/// Needs major reworking to be exposed as a public class.
/// </remarks>
internal class ThemedText : IDisposable
{
	private static int _win32Black = ColorTranslator.ToWin32(Color.Black);

	private static VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Window.Caption.Active);

	private bool _invalidated = true;

	private string _text = string.Empty;

	private Font _font = SystemFonts.CaptionFont;

	private Padding _padding = Padding.Empty;

	private int _win32Color = ColorTranslator.ToWin32(Color.Black);

	private TextFormatFlags _formatFlags;

	/// <summary>
	/// Default glow size.
	/// </summary>
	public const int DefaultGlowSize = 10;

	/// <summary>
	/// Glow size used commonly by Office 2007 in titles.
	/// </summary>
	public const int Word2007GlowSize = 15;

	private int _glowSize = 10;

	private bool _glowEnabled = true;

	private IntPtr _textHdc = IntPtr.Zero;

	private IntPtr _dibSectionRef;

	private int _lastHdcWidth = -1;

	private int _lastHdcHeight = -1;

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			if (_text != value)
			{
				_invalidated = true;
			}
			_text = value;
		}
	}

	public Font Font
	{
		get
		{
			return _font;
		}
		set
		{
			if (_font != value)
			{
				_invalidated = true;
			}
			_font = value;
		}
	}

	public Padding Padding
	{
		get
		{
			return _padding;
		}
		set
		{
			if (_padding != value)
			{
				_invalidated = true;
			}
			_padding = value;
		}
	}

	public Color Color
	{
		get
		{
			return ColorTranslator.FromWin32(_win32Black);
		}
		set
		{
			_invalidated = true;
			_win32Color = ColorTranslator.ToWin32(value);
		}
	}

	public TextFormatFlags FormatFlags
	{
		get
		{
			return _formatFlags;
		}
		set
		{
			if (_formatFlags != value)
			{
				_invalidated = true;
			}
			_formatFlags = value;
		}
	}

	public int GlowSize
	{
		get
		{
			return _glowSize;
		}
		set
		{
			if (_glowSize != value)
			{
				_invalidated = true;
			}
			_glowSize = value;
		}
	}

	public bool GlowEnabled
	{
		get
		{
			return _glowEnabled;
		}
		set
		{
			if (_glowEnabled != value)
			{
				_invalidated = true;
			}
			_glowEnabled = value;
		}
	}

	~ThemedText()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (_textHdc != IntPtr.Zero)
		{
			Methods.DeleteDC(_textHdc);
			_textHdc = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	public void Draw(Graphics g, System.Drawing.Point location, System.Drawing.Size size)
	{
		Draw(g, location.X, location.Y, size.Width, size.Height);
	}

	public void Draw(Graphics g, Rectangle rect)
	{
		Draw(g, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void Draw(Graphics g, int x, int y, int width, int height)
	{
		IntPtr outputHdc = g.GetHdc();
		IntPtr sourceHdc = PrepareHdc(outputHdc, width, height);
		Methods.BitBlt(outputHdc, x, y, width, height, sourceHdc, 0, 0, BitBltOp.SRCCOPY);
		g.ReleaseHdc(outputHdc);
	}

	/// <summary>
	/// Ensures that a valid source HDC exists and has been rendered to.
	/// </summary>
	private IntPtr PrepareHdc(IntPtr outputHdc, int width, int height)
	{
		if (width == _lastHdcWidth && height == _lastHdcHeight && !_invalidated)
		{
			return _textHdc;
		}
		_lastHdcWidth = width;
		_lastHdcHeight = height;
		if (_textHdc != IntPtr.Zero)
		{
			Methods.DeleteObject(_dibSectionRef);
			Methods.DeleteDC(_textHdc);
		}
		_textHdc = Methods.CreateCompatibleDC(outputHdc);
		BitmapInfo info = new BitmapInfo
		{
			biSize = Marshal.SizeOf(typeof(BitmapInfo)),
			biWidth = width,
			biHeight = -height,
			biPlanes = 1,
			biBitCount = 32,
			biCompression = 0
		};
		_dibSectionRef = Methods.CreateDIBSection(outputHdc, ref info, 0u, 0, IntPtr.Zero, 0u);
		Methods.SelectObject(_textHdc, _dibSectionRef);
		IntPtr hFont = Font.ToHfont();
		Methods.SelectObject(_textHdc, hFont);
		DttOpts dttOpts = new DttOpts
		{
			dwSize = Marshal.SizeOf(typeof(DttOpts)),
			dwFlags = (DttOptsFlags.DTT_TEXTCOLOR | DttOptsFlags.DTT_COMPOSITED),
			crText = _win32Color
		};
		if (_glowEnabled)
		{
			dttOpts.dwFlags |= DttOptsFlags.DTT_GLOWSIZE;
			dttOpts.iGlowSize = _glowSize;
		}
		Rect paddedBounds = new Rect(_padding.Left, _padding.Top, width - _padding.Right, height - _padding.Bottom);
		int ret = Methods.DrawThemeTextEx(renderer.Handle, _textHdc, 0, 0, _text, -1, (int)_formatFlags, ref paddedBounds, ref dttOpts);
		if (ret != 0)
		{
			Marshal.ThrowExceptionForHR(ret);
		}
		Methods.DeleteObject(hFont);
		return _textHdc;
	}
}
