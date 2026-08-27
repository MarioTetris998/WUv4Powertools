using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdvancedWizardControl.Enums;
using AdvancedWizardControl.EventArguments;
using AdvancedWizardControl.Wizard;
using AdvancedWizardControl.WizardPages;
using WUv4Powertools.Properties;

namespace WUv4Powertools;

public class frmAutoUpdateWiz : Form
{
	private IContainer components;

	private AdvancedWizard advancedWizard1;

	private AdvancedWizardPage advancedWizardPage4;

	private AdvancedWizardPage advancedWizardPage3;

	private AdvancedWizardPage advancedWizardPage2;

	private AdvancedWizardPage advancedWizardPage1;

	private FolderBrowserDialog folderConsumerDialog;

	private FolderBrowserDialog folderAutoUpdateDialog;

	private Label lblAutoUpdateDir;

	private Button btnBrowse1;

	private TextBox txtAutoUpdateDir;

	private Label lblConsumerDir;

	private Button btnBrowse0;

	private TextBox txtConsumerDir;

	private CheckedListBox chkProducts;

	private ProgressBar pbConversion;

	private Label lblWait;

	private ListBox lstLog;

	private Label lblFinished;

	public frmAutoUpdateWiz()
	{
		InitializeComponent();
	}

	private void frmAutoUpdateWiz_Load(object sender, EventArgs e)
	{
		if (Debugger.IsAttached)
		{
			txtConsumerDir.Text = "C:\\consumer";
			folderConsumerDialog.SelectedPath = txtConsumerDir.Text;
			txtAutoUpdateDir.Text = "C:\\autoupdate";
			folderAutoUpdateDialog.SelectedPath = txtAutoUpdateDir.Text;
		}
	}

	internal void loadInventories()
	{
		string[] array = File.ReadAllLines(txtAutoUpdateDir.Text + "\\providers.txt");
		string[] lines1 = File.ReadAllLines(txtAutoUpdateDir.Text + "\\providerstrings.txt");
		chkProducts.Items.Clear();
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string prov = array2[i].Split(',')[0];
			string[] array3 = lines1;
			foreach (string lin in array3)
			{
				if (lin.StartsWith(prov + ".en"))
				{
					chkProducts.Items.Add(lin.Split(',')[1].Split('@')[0], !Debugger.IsAttached);
				}
			}
		}
	}

	internal void startConversion()
	{
		advancedWizard1.ButtonsVisible = false;
		Task.Factory.StartNew(delegate
		{
			string[] array = File.ReadAllLines(txtAutoUpdateDir.Text + "\\providers.txt");
			for (int i = 0; i < array.Length; i++)
			{
				if (chkProducts.CheckedItems.IndexOf(chkProducts.Items[i]) > -1)
				{
					string text = array[i].Split(',')[0];
					List<string> list = new List<string>();
					List<string> list2 = new List<string>();
					List<string> list3 = new List<string>();
					List<string> list4 = new List<string>();
					writeLog("Starting conversion of " + chkProducts.Items[i]?.ToString() + "\n", 0);
					Encoding latin1 = Encoding.GetEncoding("ISO-8859-1");
					string[] array2 = File.ReadAllLines($"{txtConsumerDir.Text}\\{text}\\items.txt", latin1);
					string[] array3 = File.ReadAllLines($"{txtConsumerDir.Text}\\{text}\\itemsindex.txt", latin1);
					string[] array4 = File.ReadAllLines($"{txtConsumerDir.Text}\\{text}\\itemstrings.txt", Encoding.Unicode);
					string[] array5 = File.ReadAllLines($"{txtConsumerDir.Text}\\{text}\\itemstringsindex.txt", latin1);
					string[] array6 = File.ReadAllLines($"{txtConsumerDir.Text}\\{text}\\product2items.txt", latin1);
					writeLog(chkProducts.Items[i]?.ToString() + ": Converting items", 10);
					for (int j = 0; j < array2.Length; j++)
					{
						string[] array7 = array2[j].Split(new string[1] { "@|" }, StringSplitOptions.None);
						if (Convert.ToInt16(array7[7]) > 3)
						{
							if (!list2.Contains(array7[0].Split(',')[0]))
							{
								list2.Add(array7[0].Split(',')[0]);
							}
							if (!list3.Contains(array7[2]))
							{
								list3.Add(array7[2]);
							}
							array2[j] = null;
						}
					}
					writeLog(chkProducts.Items[i]?.ToString() + ": Converting itemsindex", 22);
					for (int k = 0; k < array3.Length; k++)
					{
						string item = array3[k].Split(new string[1] { "@|" }, StringSplitOptions.None)[0].Split(',')[1];
						if (list2.Contains(item))
						{
							if (!list.Contains(array3[k].Replace(text + ".", "").Split(',')[0]))
							{
								list.Add(array3[k].Replace(text + ".", "").Split(',')[0]);
							}
							array3[k] = null;
						}
					}
					writeLog(chkProducts.Items[i]?.ToString() + ": Converting product2items", 40);
					for (int l = 0; l < array6.Length; l++)
					{
						string[] array8 = array6[l].Split(',');
						for (int m = 0; m < array8.Length; m++)
						{
							if (list.Contains(array8[m]))
							{
								array8[m] = null;
							}
						}
						array8 = array8.Where((string x) => !string.IsNullOrEmpty(x)).ToArray();
						array6[l] = string.Join(",", array8);
					}
					writeLog(chkProducts.Items[i]?.ToString() + ": Converting itemstringsindex", 52);
					for (int num = 0; num < array5.Length; num++)
					{
						string item2 = array5[num].Split('.')[2].Split(',')[0];
						if (list3.Contains(item2))
						{
							string item3 = array5[num].Split(',')[1];
							list4.Add(item3);
							array5[num] = null;
						}
					}
					writeLog(chkProducts.Items[i]?.ToString() + ": Converting itemstrings", 64);
					for (int num2 = 0; num2 < array4.Length; num2++)
					{
						try
						{
							if (list4.Contains(array4[num2].Split(',')[0].Split('.')[1]))
							{
								array4[num2] = null;
							}
						}
						catch (Exception)
						{
							Console.WriteLine(num2 + " : " + array4.Length);
						}
					}
					writeLog(chkProducts.Items[i]?.ToString() + ": Optimizing files", 76);
					array2 = array2.Where((string x) => !string.IsNullOrEmpty(x)).ToArray();
					array3 = array3.Where((string x) => !string.IsNullOrEmpty(x)).ToArray();
					array5 = array5.Where((string x) => !string.IsNullOrEmpty(x)).ToArray();
					array4 = array4.Where((string x) => !string.IsNullOrEmpty(x)).ToArray();
					writeLog(chkProducts.Items[i]?.ToString() + ": Saving Files", 88);
					try
					{
						File.WriteAllLines($"{txtAutoUpdateDir.Text}\\{text}\\product2items.txt", array6, latin1);
						File.WriteAllLines($"{txtAutoUpdateDir.Text}\\{text}\\itemsindex.txt", array3, latin1);
						File.WriteAllLines($"{txtAutoUpdateDir.Text}\\{text}\\items.txt", array2, latin1);
						File.WriteAllLines($"{txtAutoUpdateDir.Text}\\{text}\\itemstringsindex.txt", array5, latin1);
						File.WriteAllLines($"{txtAutoUpdateDir.Text}\\{text}\\itemstrings.txt", array4, Encoding.Unicode);
						writeLog(chkProducts.Items[i]?.ToString() + ": The provider files are updated correctly!", 100);
					}
					catch (UnauthorizedAccessException)
					{
						writeLog(chkProducts.Items[i]?.ToString() + ": You don't have writing permissions to save on this files", 0);
					}
					catch (Exception ex3)
					{
						writeLog(chkProducts.Items[i]?.ToString() + ": " + ex3.Message, 0);
					}
				}
			}
			Invoke((Action)delegate
			{
				advancedWizard1.ButtonsVisible = true;
			});
		});
	}

	internal void writeLog(string msg, int progress)
	{
		Invoke((Action)delegate
		{
			pbConversion.Value = progress;
			lstLog.Items.Add(msg);
		});
	}

	private void btnBrowse0_Click(object sender, EventArgs e)
	{
		if (folderConsumerDialog.ShowDialog() == DialogResult.OK)
		{
			txtConsumerDir.Text = folderConsumerDialog.SelectedPath;
		}
	}

	private void btnBrowse1_Click(object sender, EventArgs e)
	{
		if (folderAutoUpdateDialog.ShowDialog() == DialogResult.OK)
		{
			txtAutoUpdateDir.Text = folderAutoUpdateDialog.SelectedPath;
		}
	}

	private void advancedWizard1_Next(object sender, WizardEventArgs e)
	{
		if (advancedWizard1.CurrentPage == advancedWizardPage1)
		{
			if (!Directory.Exists(txtConsumerDir.Text) || !Directory.Exists(txtAutoUpdateDir.Text))
			{
				e.AllowPageChange = false;
				MessageBox.Show("The directories are invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else
			{
				loadInventories();
			}
		}
		else if (advancedWizard1.CurrentPage == advancedWizardPage2)
		{
			if (chkProducts.CheckedItems.Count == 0)
			{
				e.AllowPageChange = false;
				MessageBox.Show("You need to check at least one product", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else
			{
				startConversion();
			}
		}
		else if (advancedWizard1.CurrentPage == advancedWizardPage3)
		{
			advancedWizard1.WizardPages.Insert(0, advancedWizardPage4);
			e.NextPageIndex = 0;
		}
	}

	private void advancedWizard1_Cancel(object sender, EventArgs e)
	{
		Close();
	}

	private void advancedWizardPage4_PageShow(object sender, WizardPageEventArgs e)
	{
		advancedWizard1.WizardPages.Remove(advancedWizardPage1);
		advancedWizard1.WizardPages.Remove(advancedWizardPage2);
		advancedWizard1.WizardPages.Remove(advancedWizardPage3);
		advancedWizard1.WizardPages.RemoveAt(1);
		advancedWizard1.FinishButton = true;
	}

	private void advancedWizard1_Finish(object sender, EventArgs e)
	{
		Close();
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
		this.advancedWizard1 = new AdvancedWizardControl.Wizard.AdvancedWizard();
		this.advancedWizardPage1 = new AdvancedWizardControl.WizardPages.AdvancedWizardPage();
		this.lblAutoUpdateDir = new System.Windows.Forms.Label();
		this.btnBrowse1 = new System.Windows.Forms.Button();
		this.txtAutoUpdateDir = new System.Windows.Forms.TextBox();
		this.lblConsumerDir = new System.Windows.Forms.Label();
		this.btnBrowse0 = new System.Windows.Forms.Button();
		this.txtConsumerDir = new System.Windows.Forms.TextBox();
		this.advancedWizardPage4 = new AdvancedWizardControl.WizardPages.AdvancedWizardPage();
		this.lblFinished = new System.Windows.Forms.Label();
		this.advancedWizardPage3 = new AdvancedWizardControl.WizardPages.AdvancedWizardPage();
		this.lstLog = new System.Windows.Forms.ListBox();
		this.pbConversion = new System.Windows.Forms.ProgressBar();
		this.lblWait = new System.Windows.Forms.Label();
		this.advancedWizardPage2 = new AdvancedWizardControl.WizardPages.AdvancedWizardPage();
		this.chkProducts = new System.Windows.Forms.CheckedListBox();
		this.folderConsumerDialog = new System.Windows.Forms.FolderBrowserDialog();
		this.folderAutoUpdateDialog = new System.Windows.Forms.FolderBrowserDialog();
		this.advancedWizard1.SuspendLayout();
		this.advancedWizardPage1.SuspendLayout();
		this.advancedWizardPage4.SuspendLayout();
		this.advancedWizardPage3.SuspendLayout();
		this.advancedWizardPage2.SuspendLayout();
		base.SuspendLayout();
		this.advancedWizard1.BackButtonEnabled = false;
		this.advancedWizard1.BackButtonText = "< Back";
		this.advancedWizard1.ButtonLayout = AdvancedWizardControl.Enums.ButtonLayoutKind.Office97;
		this.advancedWizard1.ButtonsVisible = true;
		this.advancedWizard1.CancelButtonText = "&Cancel";
		this.advancedWizard1.Controls.Add(this.advancedWizardPage1);
		this.advancedWizard1.Controls.Add(this.advancedWizardPage4);
		this.advancedWizard1.Controls.Add(this.advancedWizardPage3);
		this.advancedWizard1.Controls.Add(this.advancedWizardPage2);
		this.advancedWizard1.CurrentPageIsFinishPage = false;
		this.advancedWizard1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedWizard1.FinishButton = false;
		this.advancedWizard1.FinishButtonEnabled = true;
		this.advancedWizard1.FinishButtonText = "&Finish";
		this.advancedWizard1.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
		this.advancedWizard1.HelpButton = false;
		this.advancedWizard1.HelpButtonText = "&Help";
		this.advancedWizard1.Location = new System.Drawing.Point(0, 0);
		this.advancedWizard1.Name = "advancedWizard1";
		this.advancedWizard1.NextButtonEnabled = true;
		this.advancedWizard1.NextButtonText = "Next >";
		this.advancedWizard1.ProcessKeys = false;
		this.advancedWizard1.Size = new System.Drawing.Size(440, 321);
		this.advancedWizard1.TabIndex = 0;
		this.advancedWizard1.TouchScreen = false;
		this.advancedWizard1.WizardPages.Add(this.advancedWizardPage1);
		this.advancedWizard1.WizardPages.Add(this.advancedWizardPage2);
		this.advancedWizard1.WizardPages.Add(this.advancedWizardPage3);
		this.advancedWizard1.WizardPages.Add(this.advancedWizardPage4);
		this.advancedWizard1.Cancel += new System.EventHandler(advancedWizard1_Cancel);
		this.advancedWizard1.Next += new System.EventHandler<AdvancedWizardControl.EventArguments.WizardEventArgs>(advancedWizard1_Next);
		this.advancedWizard1.Finish += new System.EventHandler(advancedWizard1_Finish);
		this.advancedWizardPage1.Controls.Add(this.lblAutoUpdateDir);
		this.advancedWizardPage1.Controls.Add(this.btnBrowse1);
		this.advancedWizardPage1.Controls.Add(this.txtAutoUpdateDir);
		this.advancedWizardPage1.Controls.Add(this.lblConsumerDir);
		this.advancedWizardPage1.Controls.Add(this.btnBrowse0);
		this.advancedWizardPage1.Controls.Add(this.txtConsumerDir);
		this.advancedWizardPage1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedWizardPage1.Header = true;
		this.advancedWizardPage1.HeaderBackgroundColor = System.Drawing.Color.White;
		this.advancedWizardPage1.HeaderFont = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		this.advancedWizardPage1.HeaderImage = WUv4Powertools.Properties.Resources.Convert1;
		this.advancedWizardPage1.HeaderImageVisible = true;
		this.advancedWizardPage1.HeaderTitle = "Auto Update Inventory Converter";
		this.advancedWizardPage1.Location = new System.Drawing.Point(0, 0);
		this.advancedWizardPage1.Name = "advancedWizardPage1";
		this.advancedWizardPage1.PreviousPage = 0;
		this.advancedWizardPage1.Size = new System.Drawing.Size(440, 281);
		this.advancedWizardPage1.SubTitle = "Select the directories for the conversion";
		this.advancedWizardPage1.SubTitleFont = new System.Drawing.Font("Tahoma", 8f);
		this.advancedWizardPage1.TabIndex = 1;
		this.lblAutoUpdateDir.AutoSize = true;
		this.lblAutoUpdateDir.Location = new System.Drawing.Point(13, 144);
		this.lblAutoUpdateDir.Name = "lblAutoUpdateDir";
		this.lblAutoUpdateDir.Size = new System.Drawing.Size(171, 13);
		this.lblAutoUpdateDir.TabIndex = 6;
		this.lblAutoUpdateDir.Text = "Select the target autoupdate folder";
		this.btnBrowse1.Location = new System.Drawing.Point(352, 161);
		this.btnBrowse1.Name = "btnBrowse1";
		this.btnBrowse1.Size = new System.Drawing.Size(75, 23);
		this.btnBrowse1.TabIndex = 5;
		this.btnBrowse1.Text = "Browse";
		this.btnBrowse1.UseVisualStyleBackColor = true;
		this.btnBrowse1.Click += new System.EventHandler(btnBrowse1_Click);
		this.txtAutoUpdateDir.Location = new System.Drawing.Point(12, 163);
		this.txtAutoUpdateDir.Name = "txtAutoUpdateDir";
		this.txtAutoUpdateDir.Size = new System.Drawing.Size(334, 20);
		this.txtAutoUpdateDir.TabIndex = 4;
		this.lblConsumerDir.AutoSize = true;
		this.lblConsumerDir.Location = new System.Drawing.Point(13, 91);
		this.lblConsumerDir.Name = "lblConsumerDir";
		this.lblConsumerDir.Size = new System.Drawing.Size(168, 13);
		this.lblConsumerDir.TabIndex = 3;
		this.lblConsumerDir.Text = "Select the source consumer folder";
		this.btnBrowse0.Location = new System.Drawing.Point(352, 108);
		this.btnBrowse0.Name = "btnBrowse0";
		this.btnBrowse0.Size = new System.Drawing.Size(75, 23);
		this.btnBrowse0.TabIndex = 2;
		this.btnBrowse0.Text = "Browse";
		this.btnBrowse0.UseVisualStyleBackColor = true;
		this.btnBrowse0.Click += new System.EventHandler(btnBrowse0_Click);
		this.txtConsumerDir.Location = new System.Drawing.Point(12, 110);
		this.txtConsumerDir.Name = "txtConsumerDir";
		this.txtConsumerDir.Size = new System.Drawing.Size(334, 20);
		this.txtConsumerDir.TabIndex = 1;
		this.advancedWizardPage4.Controls.Add(this.lblFinished);
		this.advancedWizardPage4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedWizardPage4.Header = true;
		this.advancedWizardPage4.HeaderBackgroundColor = System.Drawing.Color.White;
		this.advancedWizardPage4.HeaderFont = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		this.advancedWizardPage4.HeaderImage = WUv4Powertools.Properties.Resources.Convert1;
		this.advancedWizardPage4.HeaderImageVisible = true;
		this.advancedWizardPage4.HeaderTitle = "Auto Update Inventory Converter";
		this.advancedWizardPage4.Location = new System.Drawing.Point(0, 0);
		this.advancedWizardPage4.Name = "advancedWizardPage4";
		this.advancedWizardPage4.PreviousPage = 2;
		this.advancedWizardPage4.Size = new System.Drawing.Size(440, 281);
		this.advancedWizardPage4.SubTitle = "Thanks for using the conversion tool";
		this.advancedWizardPage4.SubTitleFont = new System.Drawing.Font("Tahoma", 8f);
		this.advancedWizardPage4.TabIndex = 4;
		this.advancedWizardPage4.PageShow += new System.EventHandler<AdvancedWizardControl.EventArguments.WizardPageEventArgs>(advancedWizardPage4_PageShow);
		this.lblFinished.AutoSize = true;
		this.lblFinished.Location = new System.Drawing.Point(16, 91);
		this.lblFinished.Name = "lblFinished";
		this.lblFinished.Size = new System.Drawing.Size(235, 13);
		this.lblFinished.TabIndex = 1;
		this.lblFinished.Text = "The Autoupdate inventory Conversion is finished";
		this.advancedWizardPage3.Controls.Add(this.lstLog);
		this.advancedWizardPage3.Controls.Add(this.pbConversion);
		this.advancedWizardPage3.Controls.Add(this.lblWait);
		this.advancedWizardPage3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedWizardPage3.Header = true;
		this.advancedWizardPage3.HeaderBackgroundColor = System.Drawing.Color.White;
		this.advancedWizardPage3.HeaderFont = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		this.advancedWizardPage3.HeaderImage = WUv4Powertools.Properties.Resources.Convert1;
		this.advancedWizardPage3.HeaderImageVisible = true;
		this.advancedWizardPage3.HeaderTitle = "Auto Update Inventory Converter";
		this.advancedWizardPage3.Location = new System.Drawing.Point(0, 0);
		this.advancedWizardPage3.Name = "advancedWizardPage3";
		this.advancedWizardPage3.PreviousPage = 1;
		this.advancedWizardPage3.Size = new System.Drawing.Size(440, 281);
		this.advancedWizardPage3.SubTitle = "Please Wait for the conversion is ended";
		this.advancedWizardPage3.SubTitleFont = new System.Drawing.Font("Tahoma", 8f);
		this.advancedWizardPage3.TabIndex = 3;
		this.lstLog.FormattingEnabled = true;
		this.lstLog.Location = new System.Drawing.Point(16, 144);
		this.lstLog.Name = "lstLog";
		this.lstLog.Size = new System.Drawing.Size(411, 108);
		this.lstLog.TabIndex = 3;
		this.pbConversion.Location = new System.Drawing.Point(16, 110);
		this.pbConversion.Name = "pbConversion";
		this.pbConversion.Size = new System.Drawing.Size(411, 23);
		this.pbConversion.TabIndex = 2;
		this.lblWait.AutoSize = true;
		this.lblWait.Location = new System.Drawing.Point(13, 91);
		this.lblWait.Name = "lblWait";
		this.lblWait.Size = new System.Drawing.Size(198, 13);
		this.lblWait.TabIndex = 1;
		this.lblWait.Text = "Please Wait until the conversion is ready";
		this.advancedWizardPage2.Controls.Add(this.chkProducts);
		this.advancedWizardPage2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedWizardPage2.Header = true;
		this.advancedWizardPage2.HeaderBackgroundColor = System.Drawing.Color.White;
		this.advancedWizardPage2.HeaderFont = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		this.advancedWizardPage2.HeaderImage = WUv4Powertools.Properties.Resources.Convert1;
		this.advancedWizardPage2.HeaderImageVisible = true;
		this.advancedWizardPage2.HeaderTitle = "Auto Update Inventory Converter";
		this.advancedWizardPage2.Location = new System.Drawing.Point(0, 0);
		this.advancedWizardPage2.Name = "advancedWizardPage2";
		this.advancedWizardPage2.PreviousPage = 0;
		this.advancedWizardPage2.Size = new System.Drawing.Size(440, 281);
		this.advancedWizardPage2.SubTitle = "Select the products to convert";
		this.advancedWizardPage2.SubTitleFont = new System.Drawing.Font("Tahoma", 8f);
		this.advancedWizardPage2.TabIndex = 2;
		this.chkProducts.FormattingEnabled = true;
		this.chkProducts.Location = new System.Drawing.Point(12, 91);
		this.chkProducts.Name = "chkProducts";
		this.chkProducts.Size = new System.Drawing.Size(415, 169);
		this.chkProducts.TabIndex = 1;
		this.folderConsumerDialog.Description = "Select a consumer dictionary source folder for the conversion";
		this.folderAutoUpdateDialog.Description = "Select a autoupdate dictionary target folder for the conversion";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(440, 321);
		base.Controls.Add(this.advancedWizard1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmAutoUpdateWiz";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Auto Update Converter";
		base.Load += new System.EventHandler(frmAutoUpdateWiz_Load);
		this.advancedWizard1.ResumeLayout(false);
		this.advancedWizardPage1.ResumeLayout(false);
		this.advancedWizardPage1.PerformLayout();
		this.advancedWizardPage4.ResumeLayout(false);
		this.advancedWizardPage4.PerformLayout();
		this.advancedWizardPage3.ResumeLayout(false);
		this.advancedWizardPage3.PerformLayout();
		this.advancedWizardPage2.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
