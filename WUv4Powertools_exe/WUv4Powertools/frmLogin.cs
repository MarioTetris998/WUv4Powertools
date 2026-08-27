using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WUv4Powertools.Properties;

namespace WUv4Powertools;

public class frmLogin : Form
{
	private IContainer components;

	private PictureBox picIco;

	private Label lblUser;

	private Label lblPassword;

	private Button btnOK;

	private Button btnCancel;

	public TextBox txtUsername;

	public TextBox txtPassword;

	public frmLogin()
	{
		InitializeComponent();
		picIco.Image = new Icon(Resources.ICO100, 32, 32).ToBitmap();
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
		this.picIco = new System.Windows.Forms.PictureBox();
		this.lblUser = new System.Windows.Forms.Label();
		this.lblPassword = new System.Windows.Forms.Label();
		this.txtUsername = new System.Windows.Forms.TextBox();
		this.txtPassword = new System.Windows.Forms.TextBox();
		this.btnOK = new System.Windows.Forms.Button();
		this.btnCancel = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.picIco).BeginInit();
		base.SuspendLayout();
		this.picIco.Location = new System.Drawing.Point(12, 9);
		this.picIco.Name = "picIco";
		this.picIco.Size = new System.Drawing.Size(32, 32);
		this.picIco.TabIndex = 0;
		this.picIco.TabStop = false;
		this.lblUser.AutoSize = true;
		this.lblUser.Location = new System.Drawing.Point(51, 12);
		this.lblUser.Name = "lblUser";
		this.lblUser.Size = new System.Drawing.Size(58, 13);
		this.lblUser.TabIndex = 1;
		this.lblUser.Text = "Username:";
		this.lblPassword.AutoSize = true;
		this.lblPassword.Location = new System.Drawing.Point(53, 38);
		this.lblPassword.Name = "lblPassword";
		this.lblPassword.Size = new System.Drawing.Size(56, 13);
		this.lblPassword.TabIndex = 2;
		this.lblPassword.Text = "Password:";
		this.txtUsername.Location = new System.Drawing.Point(115, 9);
		this.txtUsername.Name = "txtUsername";
		this.txtUsername.Size = new System.Drawing.Size(167, 20);
		this.txtUsername.TabIndex = 3;
		this.txtPassword.Location = new System.Drawing.Point(115, 35);
		this.txtPassword.Name = "txtPassword";
		this.txtPassword.PasswordChar = '*';
		this.txtPassword.Size = new System.Drawing.Size(167, 20);
		this.txtPassword.TabIndex = 4;
		this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.btnOK.Location = new System.Drawing.Point(126, 61);
		this.btnOK.Name = "btnOK";
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 5;
		this.btnOK.Text = "OK";
		this.btnOK.UseVisualStyleBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Location = new System.Drawing.Point(207, 61);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 6;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(294, 93);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.txtPassword);
		base.Controls.Add(this.txtUsername);
		base.Controls.Add(this.lblPassword);
		base.Controls.Add(this.lblUser);
		base.Controls.Add(this.picIco);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmLogin";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Login to Connect";
		((System.ComponentModel.ISupportInitialize)this.picIco).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
