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
		string wanted = txtUpdCode.Text == null ? string.Empty : txtUpdCode.Text.Trim();

		// A TextBox hands back an empty string rather than null, so the old check for null could
		// never fire: an empty box went straight through and took the code out of every row.
		if (wanted.Length == 0)
		{
			MessageBox.Show("The Update code can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}

		// The code is written into the identifier in itemsindex, which is split on dots, and into the
		// comma separated lists in product2items. A code carrying either would not survive the trip.
		if (wanted.IndexOf(',') >= 0 || wanted.IndexOf('.') >= 0 ||
			wanted.IndexOf("@|", StringComparison.Ordinal) >= 0)
		{
			MessageBox.Show("An update code cannot contain a comma, a full stop, or @|.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}

		if (string.Equals(wanted, upd.code, StringComparison.Ordinal))
		{
			Dispose();
			return;
		}

		// Renaming onto a code that is already here would merge two updates into one, and renaming
		// back afterwards would not separate them again.
		foreach (string existing in frmItemList.l_items)
		{
			if (string.IsNullOrEmpty(existing)) continue;

			string[] head = existing.Split(new string[1] { "@|" }, StringSplitOptions.None)[0].Split(',');
			if (head.Length > 1 && string.Equals(head[1].Trim(), wanted, StringComparison.OrdinalIgnoreCase))
			{
				MessageBox.Show("This inventory already has an update with that code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
		}

		// Capture the dictionaries before changing them, so this can be taken back with Undo.
		frmItemList.PushUndoState();

		List<string> updateGuids = new List<string>();
		for (int i = 0; i < frmItemList.l_items.Length; i++)
		{
			string line = frmItemList.l_items[i];
			if (string.IsNullOrEmpty(line)) continue;

			int at = line.IndexOf("@|", StringComparison.Ordinal);
			string head = at < 0 ? line : line.Substring(0, at);
			int comma = head.IndexOf(',');
			if (comma < 0 || head.Substring(comma + 1).Trim() != upd.code) continue;

			if (!updateGuids.Contains(head.Substring(0, comma)))
			{
				updateGuids.Add(head.Substring(0, comma));
			}

			// Only the code field is rewritten. Replacing the code everywhere in the row also
			// rewrote it inside the download address, which carries the code in a good many rows,
			// and left the update pointing at a file that was never published under that name.
			frmItemList.l_items[i] = head.Substring(0, comma + 1) + wanted +
				(at < 0 ? string.Empty : line.Substring(at));
		}

		// The identifiers as product2items writes them, before and after, so its references can be
		// moved across without matching on the code by eye.
		List<string> oldRefs = new List<string>();
		List<string> newRefs = new List<string>();
		for (int j = 0; j < frmItemList.l_itemsindex.Length; j++)
		{
			string line = frmItemList.l_itemsindex[j];
			if (string.IsNullOrEmpty(line)) continue;

			string entryHead = line.Split(new string[1] { "@|" }, StringSplitOptions.None)[0];
			int comma = entryHead.LastIndexOf(',');
			if (comma <= 0) continue;
			if (!updateGuids.Contains(entryHead.Substring(comma + 1).Trim())) continue;

			string oldId = entryHead.Substring(0, comma);
			string[] parts = oldId.Split('.');
			if (parts.Length < 15) continue;

			parts[13] = wanted;
			string newId = string.Join(".", parts);
			string oldRef = WithoutProvider(oldId);
			string newRef = WithoutProvider(newId);
			if (!oldRefs.Contains(oldRef))
			{
				oldRefs.Add(oldRef);
				newRefs.Add(newRef);
			}

			frmItemList.l_itemsindex[j] = newId + line.Substring(comma);
		}

		for (int k = 0; k < frmItemList.l_product2items.Length; k++)
		{
			string line = frmItemList.l_product2items[k];
			if (string.IsNullOrEmpty(line)) continue;

			string[] refs = line.Split(',');
			bool touched = false;

			// The first field is the operating system this line is for, never an update.
			for (int l = 1; l < refs.Length; l++)
			{
				int found = oldRefs.IndexOf(refs[l]);
				if (found < 0) continue;

				refs[l] = newRefs[found];
				touched = true;
			}
			if (touched) frmItemList.l_product2items[k] = string.Join(",", refs);
		}

		frmItemList.p_items = 0;
		frmItemList.u_items = null;
		frmItemList.lstItemCol = new List<ListViewItem>();
		frmItemList.lstItems.Items.Clear();
		frmItemList.bw.RunWorkerAsync();
		MessageBox.Show("Update code changed Sucessfully", frmMain.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		Dispose();
	}

	// An identifier as product2items writes it, which is the items index form with the provider that
	// heads it taken off. Only the leading provider is removed, never the name wherever else it falls.
	private string WithoutProvider(string itemId)
	{
		string prefix = frmItemList.provider + ".";
		return itemId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
			? itemId.Substring(prefix.Length)
			: itemId;
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
