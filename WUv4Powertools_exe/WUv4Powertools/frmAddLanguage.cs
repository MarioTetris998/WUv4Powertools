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

public class frmAddLanguage : Form
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

	private List<string> baseIndexes = new List<string>();

	private string baseItem;

	private List<string> iLangs = new List<string>();

	private IContainer components;

	private ComboBox cmbLang;

	private Label lblLang;

	private TextBox txtDLink;

	private Label lblDLink;

	private Button btnAdd;

	private Button btnCancel;

	private Label label1;

	private TextBox txtFileName;

	public frmAddLanguage(frmItemList frmItemList, frmMain frmMain, Update upd)
	{
		this.upd = upd;
		this.frmMain = frmMain;
		this.frmItemList = frmItemList;
		InitializeComponent();
	}

	internal void addLang(HttpWebResponse response)
	{
		string[] _baseIndexes = baseIndexes.ToArray();
		string _cmbLang = cmbLang.Text;
		string _txtFileName = txtFileName.Text;
		string _txtDLink = txtDLink.Text;
		string ng = Guid.NewGuid().ToString().ToUpper();
		string iSize = response.ContentLength.ToString();
		for (int i = 0; i < _baseIndexes.Length; i++)
		{
			string indextemp = string.Format(_baseIndexes[i], _cmbLang, ng);
			Array.Resize(ref frmItemList.l_itemsindex, frmItemList.l_itemsindex.Length + 1);
			frmItemList.l_itemsindex[frmItemList.l_itemsindex.Length - 1] = indextemp;
			frmItemList.l_itemsindex = PWapi.DuoOptimize(frmItemList.l_itemsindex);
		}
		string itemtemp = baseItem.Replace("{0}", ng).Replace("{1}", _txtDLink).Replace("{2}", _txtFileName)
			.Replace("{3}", iSize);
		Array.Resize(ref frmItemList.l_items, frmItemList.l_items.Length + 1);
		frmItemList.l_items[frmItemList.l_items.Length - 1] = itemtemp;
		frmItemList.p_items = 0;
		frmItemList.u_items = null;
		frmItemList.lstItemCol = new List<ListViewItem>();
		frmItemList.lstItems.Items.Clear();
		frmItemList.bw.RunWorkerAsync();
		MessageBox.Show("Language added Sucessfully", frmMain.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		Dispose();
	}

	public bool isvalidurl(string url)
	{
		try
		{
			WebRequest webRequest = WebRequest.Create(url);
			webRequest.Timeout = 2000;
			webRequest.Method = "HEAD";
			using HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
			response.Close();
			return response.StatusCode == HttpStatusCode.OK;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public void autoFill()
	{
		cmbLang.Text = "sk";
		txtFileName.Text = "Windows-KB913433-x86-ENU.EXE";
		txtDLink.Text = "http://download.windowsupdate.com/msdownload/update/v3-19990518/cabpool/Windows-KB913433-x86-ENU_2dc93aa588101b091724fc0ba505bec.exe";
	}

	private void frmAddLanguage_Load(object sender, EventArgs e)
	{
		string[] l_itemsindex = frmItemList.l_itemsindex;
		foreach (string line in l_itemsindex)
		{
			string[] itemlines = upd.itemlines;
			foreach (string lin in itemlines)
			{
				string guid = lin.Split(',')[0];
				if (line.Contains(guid) && !baseIndexes.Contains(line))
				{
					string lang = line.Split('.')[6];
					iLangs.Add(lang);
					baseIndexes.Add(line.Replace("." + lang + ".", ".{0}.").Replace(guid, "{1}"));
					string[] line_split = lin.Replace(guid, "{0}").Split(new string[1] { "@|" }, StringSplitOptions.None);
					XmlDocument installation = new XmlDocument();
					installation.Load(new MemoryStream(Encoding.UTF8.GetBytes(line_split[5])));
					installation.GetElementsByTagName("codeBase")[0].Attributes["href"].Value = "{1}";
					installation.GetElementsByTagName("command")[0].InnerXml = installation.GetElementsByTagName("command")[0].InnerXml.Replace(installation.GetElementsByTagName("codeBase")[0].Attributes["name"].Value, "{2}");
					installation.GetElementsByTagName("codeBase")[0].Attributes["name"].Value = "{2}";
					installation.GetElementsByTagName("size")[0].InnerXml = "{3}";
					installation.GetElementsByTagName("size")[1].InnerXml = "{3}";
					line_split[5] = installation.OuterXml;
					line_split[8] = "{3}";
					baseItem = string.Join("@|", line_split);
				}
			}
		}
		l_itemsindex = baseLangs;
		foreach (string lang2 in l_itemsindex)
		{
			if (!iLangs.Contains(lang2))
			{
				cmbLang.Items.Add(lang2);
			}
		}
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		if (cmbLang.Text != null && txtDLink.Text != null && txtFileName.Text != null)
		{
			try
			{
				HttpWebResponse response = (HttpWebResponse)((HttpWebRequest)WebRequest.Create(new Uri(txtDLink.Text))).GetResponse();
				response.Close();
				addLang(response);
				return;
			}
			catch
			{
				MessageBox.Show("Unknown error", frmMain.Text, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
		}
		MessageBox.Show("You need to complete information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
		this.btnAdd.Location = new System.Drawing.Point(116, 95);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.Size = new System.Drawing.Size(75, 23);
		this.btnAdd.TabIndex = 4;
		this.btnAdd.Text = "OK";
		this.btnAdd.UseVisualStyleBackColor = true;
		this.btnAdd.Click += new System.EventHandler(btnAdd_Click);
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Location = new System.Drawing.Point(197, 95);
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
		base.ClientSize = new System.Drawing.Size(284, 128);
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
		base.Name = "frmAddLanguage";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Add Language";
		base.Load += new System.EventHandler(frmAddLanguage_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
