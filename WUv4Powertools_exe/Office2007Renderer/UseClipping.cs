using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Office2007Renderer;

public class UseClipping : IDisposable
{
	private Graphics _g;

	private Region _old;

	public UseClipping(Graphics g, GraphicsPath path)
	{
		_g = g;
		_old = g.Clip;
		Region clip = _old.Clone();
		clip.Intersect(path);
		_g.Clip = clip;
	}

	public UseClipping(Graphics g, Region region)
	{
		_g = g;
		_old = g.Clip;
		Region clip = _old.Clone();
		clip.Intersect(region);
		_g.Clip = clip;
	}

	public void Dispose()
	{
		_g.Clip = _old;
	}
}
