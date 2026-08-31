using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace WUv4Powertools;

public class frmEditLanguage : Form
{
	private frmItemList frmItemList;

	private frmMain frmMain;

	private Update upd;

	private string[] baseLangs = new string[27]
	{
		"ar", "cs", "da", "de", "el", "en", "es", "fi", "fr", "he",
		"hu", "it", "ja", "ko", "nec", "nl", "no", "pl", "pt", "ptbr",
		"ru", "sk", "sl", "sv", "tr", "zhcn", "zhtw"
	};

	private string baseIndex;

	private string baseItem;

	private List<string> iLangs = new List<string>();

	private List<UpdEdit> uLangs = new List<UpdEdit>();

	private IContainer components;

	private ComboBox cmbLang;

	private Label lblLang;

	private TextBox txtDLink;

	private Label lblDLink;

	private Button btnAdd;

	private Button btnCancel;

	private Label label1;

	private TextBox txtFileName;

	private CheckBox chkAllLanguages;

	private Label lblSharedUrl;

	public frmEditLanguage(frmItemList frmItemList, frmMain frmMain, Update upd)
	{
		this.upd = upd;
		this.frmMain = frmMain;
		this.frmItemList = frmItemList;
		InitializeComponent();
	}

	private void frmAddLanguage_Load(object sender, EventArgs e)
	{
		string[] l_itemsindex = frmItemList.l_itemsindex;
		foreach (string line in l_itemsindex)
		{
			for (int j = 0; j < upd.itemlines.Length; j++)
			{
				string lin = upd.itemlines[j];
				string guid = lin.Split(',')[0];
				if (line.Contains(guid))
				{
					UpdEdit updEdit = new UpdEdit();
					string lang = (updEdit.updLang = line.Split('.')[6]);
					updEdit.updItem = lin;
					updEdit.updIndex = upd.itemindexes[j];
					iLangs.Add(lang);
					uLangs.Add(updEdit);
					if (baseIndex == null)
					{
						baseIndex = line.Replace(lang, "{0}").Replace(guid, "{1}");
					}
				}
			}
		}
		l_itemsindex = baseLangs;
		foreach (string lang2 in l_itemsindex)
		{
			if (iLangs.Contains(lang2))
			{
				cmbLang.Items.Add(lang2);
			}
		}
		ShowSharedUrlNotice();
	}

	// Says so when every language of this update already points at the same file, which is otherwise
	// invisible: the dialog shows one language at a time and they would all simply look the same.
	private void ShowSharedUrlNotice()
	{
		if (lblSharedUrl == null) return;
		lblSharedUrl.Text = string.Empty;
		if (uLangs == null || uLangs.Count < 2) return;

		string shared = null;
		int counted = 0;
		foreach (UpdEdit upe in uLangs)
		{
			string href = HrefOf(upe.updItem);
			if (href == null) continue;
			counted++;
			if (shared == null)
			{
				shared = href;
			}
			else if (!string.Equals(shared, href, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
		}
		if (counted < 2) return;
		lblSharedUrl.Text = "All " + counted + " languages use this one URL.";
	}

	// The codeBase href out of an items.txt line, or null when it cannot be read.
	private static string HrefOf(string itemLine)
	{
		if (string.IsNullOrEmpty(itemLine)) return null;
		string[] parts = itemLine.Split(new string[1] { "@|" }, StringSplitOptions.None);
		if (parts.Length < 6) return null;
		try
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(new MemoryStream(Encoding.UTF8.GetBytes(parts[5])));
			XmlNodeList nodes = doc.GetElementsByTagName("codeBase");
			if (nodes.Count == 0) return null;
			return nodes[0].Attributes["href"].Value;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		// A text box is never null, so the old check here passed on empty fields and the blank URL
		// then failed as an unreachable address, which read as the feature not working at all.
		if (string.IsNullOrWhiteSpace(txtDLink.Text) || string.IsNullOrWhiteSpace(txtFileName.Text))
		{
			MessageBox.Show("Choose a language first, then fill in the download link and file name.",
				"Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}

		bool applyToAll = chkAllLanguages != null && chkAllLanguages.Checked;
		if (applyToAll && MessageBox.Show("Point every language of this update at the same file?\n\nEach language will use this one URL and file name.", "Windows Update v4.0 PowerTools", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}

		// Capture the dictionaries before changing them, so this can be taken back with Undo.
		frmItemList.PushUndoState();

		long iSize;
		try
		{
			// One request regardless of how many languages are being pointed at it, since they all
			// end up on the same file.
			HttpWebResponse probe = (HttpWebResponse)((HttpWebRequest)WebRequest.Create(new Uri(txtDLink.Text))).GetResponse();
			iSize = probe.ContentLength;
			probe.Close();
		}
		catch (Exception ex)
		{
			MessageBox.Show("The download link could not be reached:\n\n" + ex.Message, "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}

		int changed = 0;
		foreach (UpdEdit upe in uLangs)
		{
			if (!applyToAll && cmbLang.Text != upe.updLang)
			{
				continue;
			}
			string[] line_split = upe.updItem.Split(new string[1] { "@|" }, StringSplitOptions.None);
			XmlDocument installation = new XmlDocument();
			installation.Load(new MemoryStream(Encoding.UTF8.GetBytes(line_split[5])));
			installation.GetElementsByTagName("codeBase")[0].Attributes["href"].Value = txtDLink.Text;
			installation.GetElementsByTagName("command")[0].InnerXml = installation.GetElementsByTagName("command")[0].InnerXml.Replace(installation.GetElementsByTagName("codeBase")[0].Attributes["name"].Value, txtFileName.Text);
			installation.GetElementsByTagName("codeBase")[0].Attributes["name"].Value = txtFileName.Text;
			installation.GetElementsByTagName("size")[0].InnerXml = iSize.ToString();
			installation.GetElementsByTagName("size")[1].InnerXml = iSize.ToString();
			line_split[5] = installation.OuterXml;
			line_split[8] = iSize.ToString();
			frmItemList.l_items[upe.updIndex] = string.Join("@|", line_split);
			changed++;
		}

		frmItemList.ReloadItems();
		MessageBox.Show(applyToAll
			? (changed + " languages now point at this file.")
			: "Language edited Sucessfully", frmMain.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		Dispose();
	}


	private void cmbLang_SelectedIndexChanged(object sender, EventArgs e)
	{
		foreach (UpdEdit upe in uLangs)
		{
			if (cmbLang.Text == upe.updLang)
			{
				string[] line_split = upe.updItem.Split(new string[1] { "@|" }, StringSplitOptions.None);
				Console.WriteLine(upe.updItem);
				XmlDocument installation = new XmlDocument();
				installation.Load(new MemoryStream(Encoding.UTF8.GetBytes(line_split[5])));
				txtDLink.Text = installation.GetElementsByTagName("codeBase")[0].Attributes["href"].Value;
				txtFileName.Text = installation.GetElementsByTagName("codeBase")[0].Attributes["name"].Value;
			}
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
		this.cmbLang = new System.Windows.Forms.ComboBox();
		this.lblLang = new System.Windows.Forms.Label();
		this.txtDLink = new System.Windows.Forms.TextBox();
		this.lblDLink = new System.Windows.Forms.Label();
		this.btnAdd = new System.Windows.Forms.Button();
		this.btnCancel = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		this.txtFileName = new System.Windows.Forms.TextBox();
		base.SuspendLayout();
		this.cmbLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbLang.FormattingEnabled = true;
		this.cmbLang.Location = new System.Drawing.Point(76, 15);
		this.cmbLang.Name = "cmbLang";
		this.cmbLang.Size = new System.Drawing.Size(196, 21);
		this.cmbLang.TabIndex = 0;
		this.cmbLang.SelectedIndexChanged += new System.EventHandler(cmbLang_SelectedIndexChanged);
		this.lblLang.AutoSize = true;
		this.lblLang.Location = new System.Drawing.Point(12, 18);
		this.lblLang.Name = "lblLang";
		this.lblLang.Size = new System.Drawing.Size(58, 13);
		this.lblLang.TabIndex = 1;
		this.lblLang.Text = "Language:";
		this.txtDLink.Location = new System.Drawing.Point(76, 43);
		this.txtDLink.Name = "txtDLink";
		this.txtDLink.Size = new System.Drawing.Size(196, 20);
		this.txtDLink.TabIndex = 2;
		this.lblDLink.AutoSize = true;
		this.lblDLink.Location = new System.Drawing.Point(26, 46);
		this.lblDLink.Name = "lblDLink";
		this.lblDLink.Size = new System.Drawing.Size(44, 13);
		this.lblDLink.TabIndex = 3;
		this.lblDLink.Text = "D. Link:";
		this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.btnAdd.Location = new System.Drawing.Point(116, 142);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.Size = new System.Drawing.Size(75, 23);
		this.btnAdd.TabIndex = 4;
		this.btnAdd.Text = "OK";
		this.btnAdd.UseVisualStyleBackColor = true;
		this.btnAdd.Click += new System.EventHandler(btnAdd_Click);
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Location = new System.Drawing.Point(197, 142);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 5;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.UseVisualStyleBackColor = true;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(13, 72);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(57, 13);
		this.label1.TabIndex = 7;
		this.label1.Text = "File Name:";
		this.txtFileName.Location = new System.Drawing.Point(76, 69);
		this.txtFileName.Name = "txtFileName";
		this.txtFileName.Size = new System.Drawing.Size(196, 20);
		this.txtFileName.TabIndex = 6;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.chkAllLanguages = new System.Windows.Forms.CheckBox();
		this.chkAllLanguages.AutoSize = true;
		this.chkAllLanguages.Location = new System.Drawing.Point(16, 96);
		this.chkAllLanguages.Name = "chkAllLanguages";
		this.chkAllLanguages.Size = new System.Drawing.Size(210, 17);
		this.chkAllLanguages.TabIndex = 8;
		this.chkAllLanguages.Text = "Use this one URL for all languages";
		this.chkAllLanguages.UseVisualStyleBackColor = true;
		base.ClientSize = new System.Drawing.Size(284, 175);
		this.lblSharedUrl = new System.Windows.Forms.Label();
		this.lblSharedUrl.AutoSize = false;
		this.lblSharedUrl.Location = new System.Drawing.Point(16, 118);
		this.lblSharedUrl.Size = new System.Drawing.Size(255, 15);
		this.lblSharedUrl.ForeColor = System.Drawing.Color.FromArgb(0, 102, 0);
		this.lblSharedUrl.Text = string.Empty;
		base.Controls.Add(this.lblSharedUrl);
		base.Controls.Add(this.chkAllLanguages);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.txtFileName);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnAdd);
		base.Controls.Add(this.lblDLink);
		base.Controls.Add(this.txtDLink);
		base.Controls.Add(this.lblLang);
		base.Controls.Add(this.cmbLang);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmEditLanguage";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Edit Language";
		base.Load += new System.EventHandler(frmAddLanguage_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
