using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsAero.Dwm;

public class ThumbnailViewer : Control
{
	private Thumbnail _thumbnail;

	private Form _topLevelForm;

	private Control _parentControl;

	private EventHandler _parentChangeHandler;

	private bool _onlyClientArea = true;

	private byte _opacity = byte.MaxValue;

	private ContentAlignment _alignment = ContentAlignment.MiddleCenter;

	private bool _scaleSmallerThumbnails = true;

	private bool _lastVisibilityStatus = true;

	[Description("Determines whether to show only the client area of the window or the whole window.")]
	[Category("Appearance")]
	[DefaultValue(true)]
	public bool ShowOnlyClientArea
	{
		get
		{
			return _onlyClientArea;
		}
		set
		{
			_onlyClientArea = value;
			UpdateThumbnailInternal(base.Visible);
		}
	}

	[Description("Sets the opacity of the thumbnail.")]
	[Category("Appearance")]
	[DefaultValue(byte.MaxValue)]
	public byte Opacity
	{
		get
		{
			return _opacity;
		}
		set
		{
			_opacity = value;
			UpdateThumbnailInternal(base.Visible);
		}
	}

	public ContentAlignment ThumbnailAlignment
	{
		get
		{
			return _alignment;
		}
		set
		{
			_alignment = value;
			UpdateThumbnailInternal(base.Visible);
		}
	}

	public bool ScaleSmallerThumbnails
	{
		get
		{
			return _scaleSmallerThumbnails;
		}
		set
		{
			_scaleSmallerThumbnails = value;
			UpdateThumbnailInternal(base.Visible);
		}
	}

	public ThumbnailViewer()
	{
		_parentChangeHandler = _parentControl_VisibleChanged;
	}

	/// <summary>Sets the origin of the thumbnail and shows the thumbnail on the control.</summary>
	/// <param name="originForm">The Form instance that will be thumbnailed.</param>
	/// <param name="trackFormUpdates">True if the control should automatically update itself in case the thumbnailed
	/// form changes size or is closed.</param>
	public void SetThumbnail(Form originForm, bool trackFormUpdates)
	{
		SetThumbnail(originForm.Handle);
		if (trackFormUpdates)
		{
			originForm.SizeChanged += originForm_SizeChanged;
			originForm.FormClosed += originForm_FormClosed;
		}
	}

	public void SetThumbnail(Form originForm)
	{
		SetThumbnail(originForm.Handle);
	}

	public void SetThumbnail(IntPtr originHandle)
	{
		RecomputeParentForm();
		if (_topLevelForm != null)
		{
			_thumbnail = DwmManager.Register(_topLevelForm, originHandle);
			UpdateThumbnailInternal(base.Visible);
			return;
		}
		throw new Exception("Control must have an owner.");
	}

	/// <summary>
	/// Forces an update of the thumbnail.
	/// </summary>
	/// <remarks>
	/// Use this method if you know that the thumbnailed window has been resized
	/// and the thumbnail control should react to these changes.
	/// </remarks>
	public void UpdateThumbnail()
	{
		UpdateThumbnailInternal(base.Visible);
	}

	protected override void OnVisibleChanged(EventArgs e)
	{
		base.OnVisibleChanged(e);
		UpdateThumbnailInternal(base.Visible);
	}

	protected override void OnParentChanged(EventArgs e)
	{
		RecomputeParentForm();
		if (_parentControl != null)
		{
			_parentControl.VisibleChanged -= _parentChangeHandler;
		}
		_parentControl = base.Parent;
		_parentControl.VisibleChanged += _parentChangeHandler;
		base.OnParentChanged(e);
	}

	private void _parentControl_VisibleChanged(object sender, EventArgs e)
	{
		UpdateThumbnailInternal(_parentControl.Visible);
	}

	protected override void OnLocationChanged(EventArgs e)
	{
		base.OnLocationChanged(e);
		UpdateThumbnailInternal(base.Visible);
	}

	protected override void OnSizeChanged(EventArgs e)
	{
		base.OnSizeChanged(e);
		UpdateThumbnailInternal(base.Visible);
	}

	private void originForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		if (_thumbnail != null)
		{
			_thumbnail.Dispose();
			_thumbnail = null;
		}
	}

	private void originForm_SizeChanged(object sender, EventArgs e)
	{
		UpdateThumbnail();
	}

	protected void UpdateThumbnailInternal(bool visible)
	{
		if (_lastVisibilityStatus || visible)
		{
			if (_thumbnail != null)
			{
				_thumbnail.Update(RecomputeThumbnailRectangle(), _opacity, visible, _onlyClientArea);
			}
			_lastVisibilityStatus = visible;
		}
	}

	private Rectangle RecomputeThumbnailRectangle()
	{
		if (_topLevelForm == null || _thumbnail == null)
		{
			throw new Exception("whops, no parent or no thumbnail");
		}
		Point offset = Point.Empty;
		Control ctrl = this;
		do
		{
			offset = new Point(offset.X + ctrl.Location.X, offset.Y + ctrl.Location.Y);
			ctrl = ctrl.Parent;
		}
		while (ctrl != null && ctrl != base.TopLevelControl);
		Size destination = base.ClientSize;
		Size source = _thumbnail.GetSourceSize();
		if (source.Width < destination.Width && source.Height < destination.Height && !_scaleSmallerThumbnails)
		{
			destination = source;
		}
		if (source.Width > destination.Width || source.Height > destination.Height)
		{
			double ratio = (double)source.Width / (double)source.Height;
			if (source.Width < source.Height)
			{
				destination.Width = (int)((double)destination.Height * ratio);
			}
			else
			{
				destination.Height = (int)((double)destination.Width / ratio);
			}
		}
		int dx = base.ClientSize.Width - destination.Width;
		int dy = base.ClientSize.Height - destination.Height;
		if (ThumbnailAlignment == ContentAlignment.MiddleCenter || ThumbnailAlignment == ContentAlignment.MiddleLeft || ThumbnailAlignment == ContentAlignment.MiddleRight)
		{
			offset = new Point(offset.X, offset.Y + dy / 2);
		}
		if (ThumbnailAlignment == ContentAlignment.BottomCenter || ThumbnailAlignment == ContentAlignment.BottomLeft || ThumbnailAlignment == ContentAlignment.BottomRight)
		{
			offset = new Point(offset.X, offset.Y + dy);
		}
		if (ThumbnailAlignment == ContentAlignment.BottomCenter || ThumbnailAlignment == ContentAlignment.MiddleCenter || ThumbnailAlignment == ContentAlignment.TopCenter)
		{
			offset = new Point(offset.X + dx / 2, offset.Y);
		}
		if (ThumbnailAlignment == ContentAlignment.BottomRight || ThumbnailAlignment == ContentAlignment.MiddleRight || ThumbnailAlignment == ContentAlignment.TopRight)
		{
			offset = new Point(offset.X + dx, offset.Y);
		}
		return new Rectangle(offset, destination);
	}

	private void RecomputeParentForm()
	{
		if (!(base.TopLevelControl is Form nextParent))
		{
			_topLevelForm = null;
			if (_thumbnail != null)
			{
				_thumbnail.Dispose();
				_thumbnail = null;
			}
		}
		else
		{
			if (_thumbnail != null && _topLevelForm != nextParent)
			{
				_thumbnail.Dispose();
				_thumbnail = null;
			}
			_topLevelForm = nextParent;
		}
	}
}
