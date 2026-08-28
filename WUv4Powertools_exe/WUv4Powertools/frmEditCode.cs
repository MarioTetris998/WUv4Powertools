using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WUv4Powertools;

public class frmEditCode : Form
{
	private frmItemList frmItemList;

	private frmMain frmMain;

	private Update upd;

	private IContainer components;

	private Button btnCancel;

	private Button btnOK;

	private Label label1;

	private TextBox txtUpdCode;

	public frmEditCode(frmItemList frmItemList, frmMain frmMain, Update upd)
	{
		this.upd = upd;
		this.frmMain = frmMain;
		this.frmItemList = frmItemList;
		InitializeComponent();
	}

	private void frmEditCode_Load(object sender, EventArgs e)
	{
		txtUpdCode.Text = upd.code;
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		// Capture the dictionaries before changing them, so this can be taken back with Undo.
		frmItemList.PushUndoState();
		if (txtUpdCode.Text != null)
		{
			List<string> updateItems = new List<string>();
			List<string> updateGuids = new List<string>();
			for (int i = 0; i < frmItemList.l_items.Length; i++)
			{
				string[] itemLineSplit = frmItemList.l_items[i].Split(new string[1] { "@|" }, StringSplitOptions.None);
				if (itemLineSplit[0].Split(',')[1] == upd.code)
				{
					if (!updateGuids.Contains(itemLineSplit[0].Split(',')[0]))
					{
						updateGuids.Add(itemLineSplit[0].Split(',')[0]);
					}
					frmItemList.l_items[i] = frmItemList.l_items[i].Replace(upd.code, txtUpdCode.Text);
				}
			}
			for (int j = 0; j < frmItemList.l_itemsindex.Length; j++)
			{
				string indexGuid = frmItemList.l_itemsindex[j].Split(new string[1] { "@|" }, StringSplitOptions.None)[0].Split(',')[1];
				if (updateGuids.Contains(indexGuid))
				{
					if (!updateItems.Contains(frmItemList.l_itemsindex[j].Replace(frmItemList.provider + ".", "").Split(',')[0]))
					{
						updateItems.Add(frmItemList.l_itemsindex[j].Replace(frmItemList.provider + ".", "").Split(',')[0]);
					}
					frmItemList.l_itemsindex[j] = frmItemList.l_itemsindex[j].Replace(upd.code, txtUpdCode.Text);
				}
			}
			for (int k = 0; k < frmItemList.l_product2items.Length; k++)
			{
				string[] pline = frmItemList.l_product2items[k].Split(',');
				for (int l = 0; l < pline.Length; l++)
				{
					if (updateItems.Contains(pline[l]))
					{
						pline[l] = pline[l].Replace(upd.code, txtUpdCode.Text);
					}
				}
				frmItemList.l_product2items[k] = string.Join(",", pline);
			}
			frmItemList.p_items = 0;
			frmItemList.u_items = null;
			frmItemList.lstItemCol = new List<ListViewItem>();
			frmItemList.lstItems.Items.Clear();
			frmItemList.bw.RunWorkerAsync();
			MessageBox.Show("Update code changed Sucessfully", frmMain.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			Dispose();
		}
		else
		{
			MessageBox.Show("The Update code can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
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
		this.btnCancel = new System.Windows.Forms.Button();
		this.btnOK = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		this.txtUpdCode = new System.Windows.Forms.TextBox();
		base.SuspendLayout();
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Location = new System.Drawing.Point(197, 50);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 9;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.UseVisualStyleBackColor = true;
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.btnOK.Location = new System.Drawing.Point(116, 50);
		this.btnOK.Name = "btnOK";
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 8;
		this.btnOK.Text = "OK";
		this.btnOK.UseVisualStyleBackColor = true;
		this.btnOK.Click += new System.EventHandler(btnOK_Click);
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(13, 18);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(57, 13);
		this.label1.TabIndex = 11;
		this.label1.Text = "File Name:";
		this.txtUpdCode.Location = new System.Drawing.Point(76, 15);
		this.txtUpdCode.Name = "txtUpdCode";
		this.txtUpdCode.Size = new System.Drawing.Size(196, 20);
		this.txtUpdCode.TabIndex = 10;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 83);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.txtUpdCode);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOK);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmEditCode";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Edit Update Code";
		base.Load += new System.EventHandler(frmEditCode_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
