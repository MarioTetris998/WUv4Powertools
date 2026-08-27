using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WUv4Powertools;

public class frmStringEditor : Form
{
	private frmItemList frmItemList;

	private frmMain frmMain;

	private Update upd;

	private string langGuid;

	private int langLineId;

	private string langLine;

	private string[] baseLangs = new string[26]
	{
		"ar", "cs", "da", "de", "el", "en", "es", "fi", "fr", "he",
		"hu", "it", "ja", "ko", "nl", "no", "pl", "pt", "ptbr", "ru",
		"sk", "sl", "sv", "tr", "zhcn", "zhtw"
	};

	private IContainer components;

	private Label lblLang;

	private ComboBox cmbLang;

	private Label lblTitle;

	private TextBox txtTitle;

	private TextBox txtDescription;

	private Button btnCancel;

	private Button btnAdd;

	public frmStringEditor(frmItemList frmItemList, frmMain frmMain, Update upd)
	{
		this.upd = upd;
		this.frmMain = frmMain;
		this.frmItemList = frmItemList;
		InitializeComponent();
		
		// Hide description field for drivers
		if (frmItemList.isDriverProvider)
		{
			txtDescription.Visible = false;
			// Adjust form height if needed
			this.Height = this.Height - txtDescription.Height - 10;
		}
	}

	private void frmStringEditor_Load(object sender, EventArgs e)
	{
		string[] array = baseLangs;
		foreach (string lang in array)
		{
			cmbLang.Items.Add(lang);
		}
	}

	private void cmbLang_SelectedIndexChanged(object sender, EventArgs e)
	{
		// Validate that a language is selected
		if (cmbLang.SelectedItem == null || string.IsNullOrWhiteSpace(cmbLang.Text))
		{
			return;
		}
		
		// Validate required data
		if (frmItemList.l_itemstringsindex == null || frmItemList.l_itemstrings == null)
		{
			MessageBox.Show("Language data is not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		
		if (string.IsNullOrWhiteSpace(upd.langscode))
		{
			MessageBox.Show("Update language code is not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		
		langGuid = null;
		string[] l_itemstringsindex = frmItemList.l_itemstringsindex;
		
		// Find the language GUID
		foreach (string line in l_itemstringsindex)
		{
			if (string.IsNullOrWhiteSpace(line))
				continue;
				
			try
			{
				// Check if line contains the update's language code
				if (line.Contains(upd.langscode))
				{
					string[] dotSplit = line.Split('.');
					if (dotSplit.Length > 1 && dotSplit[1].ToLower() == cmbLang.Text.ToLower())
					{
						string[] commaSplit = line.Split(',');
						if (commaSplit.Length > 1)
						{
							langGuid = commaSplit[1].Trim();
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Skip malformed lines
				continue;
			}
		}
		
		// If no language GUID found, show error
		if (string.IsNullOrWhiteSpace(langGuid))
		{
			MessageBox.Show($"Language '{cmbLang.Text}' not found for this update.", "Language Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		
		// Find and parse the language string
		for (int j = 0; j < frmItemList.l_itemstrings.Length; j++)
		{
			string line2 = frmItemList.l_itemstrings[j];
			
			if (string.IsNullOrWhiteSpace(line2))
				continue;
				
			if (line2.Contains(langGuid))
			{
				try
				{
					string[] parts = line2.Split(new string[1] { "@|" }, StringSplitOptions.None);
					
					if (parts.Length == 0 || string.IsNullOrWhiteSpace(parts[0]))
					{
						MessageBox.Show("Invalid language string format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					
					// Parse title (always present)
					string[] titleParts = parts[0].Split(new char[1] { ',' }, 2);
					if (titleParts.Length < 2)
					{
						MessageBox.Show("Invalid title format in language string.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					
					string title = titleParts[1];
					txtTitle.Text = title ?? "";
					
					// Parse description (only if present - drivers don't have descriptions)
					string description = "";
					if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
					{
						description = parts[1];
						txtDescription.Text = description;
					}
					else
					{
						txtDescription.Text = "";
					}
					
					langLineId = j;
					
					// Create template for updating - handle drivers vs regular updates
					if (frmItemList.isDriverProvider || string.IsNullOrWhiteSpace(description))
					{
						// Driver format: title@|@|eula@|@|info
						langLine = line2.Replace(title, "{0}");
					}
					else
					{
						// Windows update format: title@|description@|eula@|@|info
						langLine = line2.Replace(title, "{0}").Replace(description, "{1}");
					}
					
					break;
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error parsing language data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
		}
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		_ = langLineId;
		if (langLine != null)
		{
			try
			{
				string newline;
				if (frmItemList.isDriverProvider)
				{
					// Driver format - only title, no description
					newline = string.Format(langLine, txtTitle.Text ?? "");
				}
				else
				{
					// Windows update format - title and description
					newline = string.Format(langLine, txtTitle.Text ?? "", txtDescription.Text ?? "");
				}
				
				frmItemList.l_itemstrings[langLineId] = newline;
				frmItemList.p_items = 0;
				frmItemList.u_items = null;
				frmItemList.lstItemCol = new List<ListViewItem>();
				frmItemList.lstItems.Items.Clear();
				
				// Check if BackgroundWorker is busy before starting it
				if (!frmItemList.bw.IsBusy)
				{
					frmItemList.bw.RunWorkerAsync();
				}
				
				// Set dialog result to OK (button already has this set, but ensure it's honored)
				this.DialogResult = DialogResult.OK;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error updating language strings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.DialogResult = DialogResult.None; // Keep dialog open on error
			}
		}
		else
		{
			MessageBox.Show("Please select a language first.", "No Language Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			this.DialogResult = DialogResult.None; // Keep dialog open
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
		this.lblLang = new System.Windows.Forms.Label();
		this.cmbLang = new System.Windows.Forms.ComboBox();
		this.lblTitle = new System.Windows.Forms.Label();
		this.txtTitle = new System.Windows.Forms.TextBox();
		this.txtDescription = new System.Windows.Forms.TextBox();
		this.btnCancel = new System.Windows.Forms.Button();
		this.btnAdd = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.lblLang.AutoSize = true;
		this.lblLang.Location = new System.Drawing.Point(13, 13);
		this.lblLang.Name = "lblLang";
		this.lblLang.Size = new System.Drawing.Size(58, 13);
		this.lblLang.TabIndex = 0;
		this.lblLang.Text = "Language:";
		this.cmbLang.FormattingEnabled = true;
		this.cmbLang.Location = new System.Drawing.Point(77, 10);
		this.cmbLang.Name = "cmbLang";
		this.cmbLang.Size = new System.Drawing.Size(205, 21);
		this.cmbLang.TabIndex = 1;
		this.cmbLang.SelectedIndexChanged += new System.EventHandler(cmbLang_SelectedIndexChanged);
		this.lblTitle.AutoSize = true;
		this.lblTitle.Location = new System.Drawing.Point(41, 40);
		this.lblTitle.Name = "lblTitle";
		this.lblTitle.Size = new System.Drawing.Size(30, 13);
		this.lblTitle.TabIndex = 2;
		this.lblTitle.Text = "Title:";
		this.txtTitle.Location = new System.Drawing.Point(77, 37);
		this.txtTitle.Name = "txtTitle";
		this.txtTitle.Size = new System.Drawing.Size(205, 20);
		this.txtTitle.TabIndex = 3;
		this.txtDescription.Location = new System.Drawing.Point(16, 63);
		this.txtDescription.Multiline = true;
		this.txtDescription.Name = "txtDescription";
		this.txtDescription.Size = new System.Drawing.Size(266, 169);
		this.txtDescription.TabIndex = 4;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Location = new System.Drawing.Point(207, 238);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 7;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.UseVisualStyleBackColor = true;
		this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.btnAdd.Location = new System.Drawing.Point(126, 238);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.Size = new System.Drawing.Size(75, 23);
		this.btnAdd.TabIndex = 6;
		this.btnAdd.Text = "OK";
		this.btnAdd.UseVisualStyleBackColor = true;
		this.btnAdd.Click += new System.EventHandler(btnAdd_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(294, 273);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnAdd);
		base.Controls.Add(this.txtDescription);
		base.Controls.Add(this.txtTitle);
		base.Controls.Add(this.lblTitle);
		base.Controls.Add(this.cmbLang);
		base.Controls.Add(this.lblLang);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmStringEditor";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "String Editor";
		this.AcceptButton = this.btnAdd;
		this.CancelButton = this.btnCancel;
		base.Load += new System.EventHandler(frmStringEditor_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
