using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ConsoleControl;

namespace WUv4Powertools;

public class frmConsole : Form
{
	private IContainer components;

	private global::ConsoleControl.ConsoleControl consoleControl1;

	public frmConsole()
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WUv4Powertools.frmConsole));
		this.consoleControl1 = new global::ConsoleControl.ConsoleControl();
		base.SuspendLayout();
		this.consoleControl1.AllowInput = true;
		this.consoleControl1.BackColor = System.Drawing.Color.Black;
		this.consoleControl1.ConsoleBackgroundColor = System.Drawing.Color.Black;
		this.consoleControl1.ConsoleForegroundColor = System.Drawing.Color.LightGray;
		this.consoleControl1.CurrentBackgroundColor = System.Drawing.Color.Black;
		this.consoleControl1.CurrentForegroundColor = System.Drawing.Color.LightGray;
		this.consoleControl1.CursorType = ConsoleControl.CursorTypes.Underline;
		this.consoleControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.consoleControl1.EchoInput = true;
		this.consoleControl1.ForeColor = System.Drawing.Color.LightGray;
		this.consoleControl1.Location = new System.Drawing.Point(0, 0);
		this.consoleControl1.Name = "consoleControl1";
		this.consoleControl1.ShowCursor = true;
		this.consoleControl1.Size = new System.Drawing.Size(646, 377);
		this.consoleControl1.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(646, 377);
		base.Controls.Add(this.consoleControl1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.Name = "frmConsole";
		this.Text = "PowerTools Console";
		base.ResumeLayout(false);
	}
}
