using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Office2007Renderer;

namespace WUv4Powertools;

public class frmHome : Form
{
	private frmMain frmMain;

	private IContainer components;

	private Panel panelParent;

	private Panel panelBottom;

	private PictureBox pictureBox1;

	private PictureBox pictureBox2;

	private Button btn0;

	private Button btn1;

	private Button btn3;

	private Button btn2;

	public frmHome(frmMain frmMain)
	{
		InitializeComponent();
		this.frmMain = frmMain;
		btn0.Click += frmMain.openToolStripButton_ButtonClick;
		btn1.Click += frmMain.autoupdateConverter_Click;
		btn2.Click += frmMain.openWURServers_Click;
	}

	private void frmHome_Paint(object sender, PaintEventArgs e)
	{
		Graphics graphics = e.Graphics;
		LinearGradientBrush gradientBrush = new LinearGradientBrush(color1: Color.White, color2: Office2007ColorTable._menuToolBack, rect: base.ClientRectangle, linearGradientMode: LinearGradientMode.Vertical);
		graphics.FillRectangle(gradientBrush, base.ClientRectangle);
	}

	private void frmHome_Resize(object sender, EventArgs e)
	{
		panelParent.Left = (base.Width - panelParent.Width) / 2;
		Invalidate();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WUv4Powertools.frmHome));
		this.panelParent = new System.Windows.Forms.Panel();
		this.pictureBox2 = new System.Windows.Forms.PictureBox();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.panelBottom = new System.Windows.Forms.Panel();
		this.btn3 = new System.Windows.Forms.Button();
		this.btn1 = new System.Windows.Forms.Button();
		this.btn0 = new System.Windows.Forms.Button();
		this.btn2 = new System.Windows.Forms.Button();
		this.panelParent.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		this.panelBottom.SuspendLayout();
		base.SuspendLayout();
		this.panelParent.BackColor = System.Drawing.Color.Transparent;
		this.panelParent.Controls.Add(this.pictureBox2);
		this.panelParent.Controls.Add(this.pictureBox1);
		this.panelParent.Controls.Add(this.panelBottom);
		this.panelParent.Location = new System.Drawing.Point(0, 0);
		this.panelParent.Name = "panelParent";
		this.panelParent.Size = new System.Drawing.Size(480, 390);
		this.panelParent.TabIndex = 0;
		this.pictureBox2.Image = (System.Drawing.Image)resources.GetObject("pictureBox2.Image");
		this.pictureBox2.Location = new System.Drawing.Point(152, 30);
		this.pictureBox2.Name = "pictureBox2";
		this.pictureBox2.Size = new System.Drawing.Size(218, 53);
		this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
		this.pictureBox2.TabIndex = 2;
		this.pictureBox2.TabStop = false;
		this.pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
		this.pictureBox1.Location = new System.Drawing.Point(376, 12);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(92, 92);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pictureBox1.TabIndex = 1;
		this.pictureBox1.TabStop = false;
		this.panelBottom.BackColor = System.Drawing.Color.White;
		this.panelBottom.BackgroundImage = (System.Drawing.Image)resources.GetObject("panelBottom.BackgroundImage");
		this.panelBottom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
		this.panelBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.panelBottom.Controls.Add(this.btn3);
		this.panelBottom.Controls.Add(this.btn1);
		this.panelBottom.Controls.Add(this.btn0);
		this.panelBottom.Controls.Add(this.btn2);
		this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelBottom.Location = new System.Drawing.Point(0, 110);
		this.panelBottom.Name = "panelBottom";
		this.panelBottom.Size = new System.Drawing.Size(480, 280);
		this.panelBottom.TabIndex = 0;
		this.btn3.BackColor = System.Drawing.Color.Transparent;
		this.btn3.Enabled = false;
		this.btn3.FlatAppearance.BorderSize = 0;
		this.btn3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(255, 128, 0);
		this.btn3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btn3.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn3.ForeColor = System.Drawing.Color.DarkBlue;
		this.btn3.Image = (System.Drawing.Image)resources.GetObject("btn3.Image");
		this.btn3.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
		this.btn3.Location = new System.Drawing.Point(159, 140);
		this.btn3.Name = "btn3";
		this.btn3.Padding = new System.Windows.Forms.Padding(6);
		this.btn3.Size = new System.Drawing.Size(160, 120);
		this.btn3.TabIndex = 3;
		this.btn3.Text = "WURPT Bash";
		this.btn3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
		this.btn3.UseVisualStyleBackColor = false;
		this.btn1.BackColor = System.Drawing.Color.Transparent;
		this.btn1.FlatAppearance.BorderSize = 0;
		this.btn1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(255, 128, 0);
		this.btn1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btn1.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn1.ForeColor = System.Drawing.Color.DarkBlue;
		this.btn1.Image = (System.Drawing.Image)resources.GetObject("btn1.Image");
		this.btn1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
		this.btn1.Location = new System.Drawing.Point(239, 20);
		this.btn1.Name = "btn1";
		this.btn1.Padding = new System.Windows.Forms.Padding(6);
		this.btn1.Size = new System.Drawing.Size(160, 120);
		this.btn1.TabIndex = 1;
		this.btn1.Text = "Convert Inventories for AutoUpdate";
		this.btn1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
		this.btn1.UseVisualStyleBackColor = false;
		this.btn0.BackColor = System.Drawing.Color.Transparent;
		this.btn0.FlatAppearance.BorderSize = 0;
		this.btn0.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(255, 128, 0);
		this.btn0.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btn0.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn0.ForeColor = System.Drawing.Color.DarkBlue;
		this.btn0.Image = (System.Drawing.Image)resources.GetObject("btn0.Image");
		this.btn0.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
		this.btn0.Location = new System.Drawing.Point(79, 20);
		this.btn0.Name = "btn0";
		this.btn0.Padding = new System.Windows.Forms.Padding(6);
		this.btn0.Size = new System.Drawing.Size(160, 120);
		this.btn0.TabIndex = 0;
		this.btn0.Text = "Open Local Inventory";
		this.btn0.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
		this.btn0.UseVisualStyleBackColor = false;
		this.btn2.BackColor = System.Drawing.Color.Transparent;
		this.btn2.FlatAppearance.BorderSize = 0;
		this.btn2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(255, 128, 0);
		this.btn2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btn2.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn2.ForeColor = System.Drawing.Color.DarkBlue;
		this.btn2.Image = (System.Drawing.Image)resources.GetObject("btn2.Image");
		this.btn2.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
		this.btn2.Location = new System.Drawing.Point(239, 20);
		this.btn2.Name = "btn2";
		this.btn2.Padding = new System.Windows.Forms.Padding(6);
		this.btn2.Size = new System.Drawing.Size(160, 120);
		this.btn2.TabIndex = 2;
		this.btn2.Text = "Connect to WUR Servers";
		this.btn2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
		this.btn2.UseVisualStyleBackColor = false;
		this.btn2.Visible = false;
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		base.ClientSize = new System.Drawing.Size(480, 390);
		base.Controls.Add(this.panelParent);
		this.DoubleBuffered = true;
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frmHome";
		this.Text = "Welcome";
		base.Paint += new System.Windows.Forms.PaintEventHandler(frmHome_Paint);
		base.Resize += new System.EventHandler(frmHome_Resize);
		this.panelParent.ResumeLayout(false);
		this.panelParent.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		this.panelBottom.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
