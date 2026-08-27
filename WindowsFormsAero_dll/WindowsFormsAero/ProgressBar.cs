using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsAero.Native;

namespace WindowsFormsAero;

[ToolboxBitmap(typeof(ProgressBar))]
public class ProgressBar : System.Windows.Forms.ProgressBar
{
	private ProgressBarState _state;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams obj = base.CreateParams;
			obj.Style |= 16;
			return obj;
		}
	}

	[Description("Gets or sets the ProgressBar state.")]
	[Category("Appearance")]
	[DefaultValue(ProgressBarState.Normal)]
	public ProgressBarState State
	{
		get
		{
			return _state;
		}
		set
		{
			if (_state != value)
			{
				SetState(value);
			}
			_state = value;
		}
	}

	private void SetState(ProgressBarState targetState)
	{
		if (base.IsHandleCreated)
		{
			Methods.SendMessage(base.Handle, 1040u, (int)targetState.ToNative(), 0);
		}
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		SetState(_state);
	}

	protected override void WndProc(ref Message m)
	{
		if (m.Msg == 1026 && _state != ProgressBarState.Normal)
		{
			SetState(ProgressBarState.Normal);
		}
		base.WndProc(ref m);
		if (m.Msg == 1026 && _state != ProgressBarState.Normal)
		{
			SetState(_state);
		}
	}
}
