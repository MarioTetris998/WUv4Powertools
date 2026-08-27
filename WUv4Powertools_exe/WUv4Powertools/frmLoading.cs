using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WUv4Powertools;

public class frmLoading : Form
{
	private IContainer components;

	private ProgressBar pbLoad;

	public frmLoading()
	{
		InitializeComponent();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.pbLoad = new System.Windows.Forms.ProgressBar();
		base.SuspendLayout();
		this.pbLoad.Location = new System.Drawing.Point(12, 12);
		this.pbLoad.Name = "pbLoad";
		this.pbLoad.Size = new System.Drawing.Size(216, 23);
		this.pbLoad.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
		this.pbLoad.TabIndex = 0;
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		base.ClientSize = new System.Drawing.Size(236, 44);
		base.ControlBox = false;
		base.Controls.Add(this.pbLoad);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmLoading";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Please Wait";
		base.ResumeLayout(false);
	}
}
