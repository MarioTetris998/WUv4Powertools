using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace WUv4Powertools;

public class frmDeleteLanguage : Form
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

	private Label lblLang;

	private ComboBox cmbLang;

	private Button btnCancel;

	private Button btnDelete;

	public frmDeleteLanguage(frmItemList frmItemList, frmMain frmMain, Update upd)
	{
		this.upd = upd;
		this.frmMain = frmMain;
		this.frmItemList = frmItemList;
		InitializeComponent();
	}

	private void frmDeleteLanguage_Load(object sender, EventArgs e)
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
					if (lang != "en")
					{
						iLangs.Add(lang);
						uLangs.Add(updEdit);
					}
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
	}

	private void btnDelete_Click(object sender, EventArgs e)
	{
		if (cmbLang.Text != null)
		{
			foreach (UpdEdit upe in uLangs)
			{
				if (cmbLang.Text == upe.updLang)
				{
					frmItemList.l_items = frmItemList.l_items.Where((string val) => val != upe.updItem).ToArray();
				}
			}
			frmItemList.p_items = 0;
			frmItemList.u_items = null;
			frmItemList.lstItemCol = new List<ListViewItem>();
			frmItemList.lstItems.Items.Clear();
			frmItemList.bw.RunWorkerAsync();
			MessageBox.Show("Language deleted Sucessfully", frmMain.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			Dispose();
		}
		else
		{
			MessageBox.Show("You need to select a Language", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void cmbLang_SelectedIndexChanged(object sender, EventArgs e)
	{
		foreach (UpdEdit upe in uLangs)
		{
			if (cmbLang.Text == upe.updLang)
			{
				string[] line_split = upe.updItem.Split(new string[1] { "@|" }, StringSplitOptions.None);
				Console.WriteLine(upe.updItem);
				new XmlDocument().Load(new MemoryStream(Encoding.UTF8.GetBytes(line_split[5])));
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
		this.lblLang = new System.Windows.Forms.Label();
		this.cmbLang = new System.Windows.Forms.ComboBox();
		this.btnCancel = new System.Windows.Forms.Button();
		this.btnDelete = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.lblLang.AutoSize = true;
		this.lblLang.Location = new System.Drawing.Point(12, 18);
		this.lblLang.Name = "lblLang";
		this.lblLang.Size = new System.Drawing.Size(58, 13);
		this.lblLang.TabIndex = 3;
		this.lblLang.Text = "Language:";
		this.cmbLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbLang.FormattingEnabled = true;
		this.cmbLang.Location = new System.Drawing.Point(76, 15);
		this.cmbLang.Name = "cmbLang";
		this.cmbLang.Size = new System.Drawing.Size(196, 21);
		this.cmbLang.TabIndex = 2;
		this.cmbLang.SelectedIndexChanged += new System.EventHandler(cmbLang_SelectedIndexChanged);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Location = new System.Drawing.Point(197, 50);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 7;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.UseVisualStyleBackColor = true;
		this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.btnDelete.Location = new System.Drawing.Point(116, 50);
		this.btnDelete.Name = "btnDelete";
		this.btnDelete.Size = new System.Drawing.Size(75, 23);
		this.btnDelete.TabIndex = 6;
		this.btnDelete.Text = "OK";
		this.btnDelete.UseVisualStyleBackColor = true;
		this.btnDelete.Click += new System.EventHandler(btnDelete_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 83);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnDelete);
		base.Controls.Add(this.lblLang);
		base.Controls.Add(this.cmbLang);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmDeleteLanguage";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Delete Language";
		base.Load += new System.EventHandler(frmDeleteLanguage_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
