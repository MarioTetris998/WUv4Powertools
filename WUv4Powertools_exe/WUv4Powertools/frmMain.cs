using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using MdiTabControl;
using Office2007Renderer;

namespace WUv4Powertools;

public class frmMain : Form
{
	public string folderBrowserDialogSrc;

	private Queue<string> MRUlist = new Queue<string>();

	private IContainer components;

	private FolderBrowserDialog folderBrowserDialog;

	private System.Windows.Forms.Timer tmrAgent;

	private ToolStripContainer tsContainer;

	private ToolStrip tbStandard;

	private ToolStripSplitButton openToolStripButton;

	private ToolStripButton saveToolStripButton;

	private ToolStripButton btnRefresh;

	private ToolStripSeparator toolStripSeparator6;

	private ToolStripButton cutToolStripButton;

	private ToolStripButton copyToolStripButton;

	private ToolStripButton pasteToolStripButton;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripButton btnNewUpdate;

	private ToolStripButton btnEditUpdate;

	private ToolStripButton btnEditEULA;

	private ToolStripButton btnAddUpdateLang;

	private ToolStripButton btnEditUpdateLang;

	private ToolStripButton btnStringsEditor;

	private ToolStripButton btnStringFix;

	private StatusStrip statusStandard;

	public ToolStripStatusLabel lblItems;

	public ToolStripProgressBar pbBusy;

	public MdiTabControl.TabControl mdiTabs;

	private ToolStrip tbSearch;

	private PlaceHolderTextBox txtSearch;

	private ToolStripButton btnFindNext;

	private ToolStripButton btnPreviousSearch;

	private MenuStrip menuStrip1;

	private ToolStripMenuItem fileToolStripMenuItem;

	private ToolStripMenuItem openToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator;

	private ToolStripMenuItem saveToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripMenuItem printToolStripMenuItem;

	private ToolStripMenuItem printPreviewToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator3;

	private ToolStripMenuItem exitToolStripMenuItem;

	private ToolStripMenuItem editToolStripMenuItem;

	private ToolStripMenuItem undoToolStripMenuItem;

	private ToolStripMenuItem redoToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator4;

	private ToolStripMenuItem cutToolStripMenuItem;

	private ToolStripMenuItem copyToolStripMenuItem;

	private ToolStripMenuItem pasteToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator5;

	private ToolStripMenuItem selectAllToolStripMenuItem;

	private ToolStripMenuItem toolsToolStripMenuItem;

	private ToolStripMenuItem restoreBackupToolStripMenuItem;

	private ToolStripMenuItem repairProviderToolStripMenuItem;

	private ToolStripMenuItem customizeToolStripMenuItem;

	private ToolStripMenuItem optionsToolStripMenuItem;

	private ToolStripMenuItem helpToolStripMenuItem;

	private ToolStripMenuItem contentsToolStripMenuItem;

	private ToolStripMenuItem indexToolStripMenuItem;

	private ToolStripMenuItem searchToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator8;

	private ToolStripMenuItem aboutToolStripMenuItem;

	private ToolStripButton btnDeleteUpdate;

	private ToolStripButton btnDeleteUpdateLang;

	private ToolStripButton btnChangeUpdateCode;

	public frmMain()
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		mdiTabs.TabPages.Add(new frmHome(this));
		mdiTabs.TabPages[0].CloseButtonVisible = false;
		ToolStripManager.Renderer = new global::Office2007Renderer.Office2007Renderer();
		statusStandard.Renderer = new global::Office2007Renderer.Office2007Renderer();
		mdiTabs.TabBackHighColor = Office2007ColorTable._toolStripBegin;
		mdiTabs.TabBackLowColor = Office2007ColorTable._toolStripEnd;
		mdiTabs.BorderColor = Color.FromArgb(21, 66, 139);
		mdiTabs.BackHighColor = Office2007ColorTable._toolStripBegin;
	}

	public void autoupdateConverter_Click(object sender, EventArgs e)
	{
		new frmAutoUpdateWiz().ShowDialog();
	}

	public void openWURServers_Click(object sender, EventArgs e)
	{
		try
		{
			folderBrowserDialogSrc = "\\\\51.178.29.117\\v4.windowsupdaterestored.com\\dictionaries\\consumer";
			openInventories(folderBrowserDialogSrc);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Windows Update v4.0", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	internal bool openInventories(string pathInv, bool addHistory = true)
	{
		if (string.IsNullOrWhiteSpace(pathInv))
		{
			MessageBox.Show("No inventory folder was specified.", "Windows Update v4.0", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
		
		string providersPath = pathInv + "\\providers.txt";
		string providerStringsPath = pathInv + "\\providerstrings.txt";
		
		// The folder can be gone entirely (a deleted or disconnected inventory) or still
		// be there but not be a dictionary, so check both before reading anything.
		if (!Directory.Exists(pathInv) || !File.Exists(providersPath) || !File.Exists(providerStringsPath))
		{
			MessageBox.Show("This inventory could not be opened. The folder no longer exists, or it is not a WUv4 dictionary:\n\n" + pathInv, "Windows Update v4.0", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			RemoveRecentFile(pathInv);
			return false;
		}
		
		string[] array;
		string[] lines1;
		try
		{
			array = File.ReadAllLines(providersPath);
			lines1 = File.ReadAllLines(providerStringsPath, Encoding.Unicode);
		}
		catch (Exception ex)
		{
			MessageBox.Show("This inventory could not be read:\n\n" + ex.Message, "Windows Update v4.0", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
		
		// Only record the path once it has actually loaded, so a folder that fails
		// never gets added to the recent list.
		if (addHistory)
		{
			SaveRecentFile(pathInv);
		}
		
		frmProvider frmProvider2 = new frmProvider();
		frmProvider2.Tag = this;
		frmProvider2.Show();
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string prov = array2[i].Split(',')[0];
			string[] array3 = lines1;
			foreach (string lin in array3)
			{
				if (string.IsNullOrEmpty(lin))
				{
					continue;
				}
				
				// Remove null characters and trim
				string cleanLine = lin.Replace("\0", "").Trim();
				
				if (cleanLine.Contains(prov + ".en,") || cleanLine.StartsWith(prov + ".en,"))
				{
					// Split by comma and get the provider name
					string[] parts = cleanLine.Split(',');
					if (parts.Length >= 2)
					{
						// Get everything between the comma and @| 
						string providerName = parts[1];
						int atPos = providerName.IndexOf("@|");
						if (atPos > 0)
						{
							providerName = providerName.Substring(0, atPos);
						}
						providerName = providerName.Trim();
						
						if (!string.IsNullOrEmpty(providerName))
						{
							frmProvider2.lstProviders.Items.Add(providerName);
						}
					}
				}
			}
		}
		
		return true;
	}

	private void tmrStatus_Tick(object sender, EventArgs e)
	{
	}

	private void frmMain_Load(object sender, EventArgs g)
	{
		LoadRecentList();
		RebuildRecentMenu();
	}

	private void btnNewUpdate_Click(object sender, EventArgs e)
	{
		_ = (frmItemList)base.Tag;
		new frmAddUpdate((frmItemList)base.Tag, this).ShowDialog();
	}

	private void lblEula_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
	}

	private void tmrAgent_Tick(object sender, EventArgs e)
	{
		if (base.Tag == null)
		{
			saveToolStripButton.Enabled = false;
			btnNewUpdate.Enabled = false;
			btnAddUpdateLang.Enabled = false;
			btnEditUpdate.Enabled = false;
			btnEditEULA.Enabled = false;
			btnEditUpdateLang.Enabled = false;
			btnDeleteUpdate.Enabled = false;
			btnDeleteUpdateLang.Enabled = false;
			btnRefresh.Enabled = false;
			btnStringsEditor.Enabled = false;
			btnStringFix.Enabled = false;
			btnChangeUpdateCode.Enabled = false;
			return;
		}
		saveToolStripButton.Enabled = true;
		btnNewUpdate.Enabled = true;
		btnRefresh.Enabled = true;
		if (((frmItemList)base.Tag).lstItems.SelectedItems.Count == 1)
		{
			btnAddUpdateLang.Enabled = true;
			btnEditUpdate.Enabled = true;
			btnEditEULA.Enabled = true;
			btnEditUpdateLang.Enabled = true;
			btnDeleteUpdate.Enabled = true;
			btnDeleteUpdateLang.Enabled = true;
			btnStringsEditor.Enabled = true;
			btnStringFix.Enabled = true;
			btnChangeUpdateCode.Enabled = true;
		}
		else
		{
			btnAddUpdateLang.Enabled = false;
			btnEditUpdate.Enabled = false;
			btnEditEULA.Enabled = false;
			btnEditUpdateLang.Enabled = false;
			btnDeleteUpdate.Enabled = false;
			btnDeleteUpdateLang.Enabled = false;
			btnStringsEditor.Enabled = false;
			btnStringFix.Enabled = false;
			btnChangeUpdateCode.Enabled = false;
		}
	}

	private void btnAddUpdateLang_Click(object sender, EventArgs e)
	{
		frmItemList frmItemList2 = (frmItemList)base.Tag;
		new frmAddLanguage((frmItemList)base.Tag, this, (Update)frmItemList2.lstItems.SelectedItems[0].Tag).ShowDialog();
	}

	private void btnEditUpdate_Click(object sender, EventArgs e)
	{
		frmItemList frmItemList2 = (frmItemList)base.Tag;
		new frmEditUpdate((frmItemList)base.Tag, this, (Update)frmItemList2.lstItems.SelectedItems[0].Tag).ShowDialog();
	}

	private void btnEditEULA_Click(object sender, EventArgs e)
	{
		frmItemList frmItemList2 = (frmItemList)base.Tag;
		if (frmItemList2.lstItems.SelectedItems.Count == 0)
		{
			MessageBox.Show("Please select an update first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		new frmEditEULA((frmItemList)base.Tag, this, (Update)frmItemList2.lstItems.SelectedItems[0].Tag).ShowDialog();
		// Refresh the list to show updated data
		frmItemList2.loadItems();
	}

	private void btnDeleteUpdate_Click(object sender, EventArgs e)
	{
		frmItemList frmItemList2 = (frmItemList)base.Tag;
		Update upd = (Update)frmItemList2.lstItems.SelectedItems[0].Tag;
		if (MessageBox.Show("Do you want to delete the update \"" + upd.getLang("en").title + "\"?", "Windows Update v4.0 PowerTools", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		List<string> updateItems = new List<string>();
		List<string> updateGuids = new List<string>();
		List<string> langGuids = new List<string>();
		List<string> uniLangGuids = new List<string>();
		for (int i = 0; i < frmItemList2.l_items.Length; i++)
		{
			string[] itemLineSplit = frmItemList2.l_items[i].Split(new string[1] { "@|" }, StringSplitOptions.None);
			if (itemLineSplit[0].Split(',')[1] == upd.code)
			{
				if (!updateGuids.Contains(itemLineSplit[0].Split(',')[0]))
				{
					updateGuids.Add(itemLineSplit[0].Split(',')[0]);
				}
				if (!langGuids.Contains(itemLineSplit[2]))
				{
					langGuids.Add(itemLineSplit[2]);
				}
				frmItemList2.l_items[i] = null;
			}
		}
		for (int j = 0; j < frmItemList2.l_itemsindex.Length; j++)
		{
			string indexGuid = frmItemList2.l_itemsindex[j].Split(new string[1] { "@|" }, StringSplitOptions.None)[0].Split(',')[1];
			if (updateGuids.Contains(indexGuid))
			{
				if (!updateItems.Contains(frmItemList2.l_itemsindex[j].Replace(frmItemList2.provider + ".", "").Split(',')[0]))
				{
					updateItems.Add(frmItemList2.l_itemsindex[j].Replace(frmItemList2.provider + ".", "").Split(',')[0]);
				}
				frmItemList2.l_itemsindex[j] = null;
			}
		}
		for (int k = 0; k < frmItemList2.l_product2items.Length; k++)
		{
			string[] pline = frmItemList2.l_product2items[k].Split(',');
			for (int l = 0; l < pline.Length; l++)
			{
				if (updateItems.Contains(pline[l]))
				{
					pline[l] = null;
				}
			}
			pline = pline.Where((string x) => !string.IsNullOrEmpty(x)).ToArray();
			frmItemList2.l_product2items[k] = string.Join(",", pline);
		}
		for (int i2 = 0; i2 < frmItemList2.l_itemstringsindex.Length; i2++)
		{
			string multiGuid = frmItemList2.l_itemstringsindex[i2].Split('.')[2].Split(',')[0];
			if (langGuids.Contains(multiGuid))
			{
				string uniGuid = frmItemList2.l_itemstringsindex[i2].Split(',')[1];
				uniLangGuids.Add(uniGuid);
				frmItemList2.l_itemstringsindex[i2] = null;
			}
		}
		for (int i3 = 0; i3 < frmItemList2.l_itemstrings.Length; i3++)
		{
			try
			{
				if (uniLangGuids.Contains(frmItemList2.l_itemstrings[i3].Split(',')[0].Split('.')[1]))
				{
					frmItemList2.l_itemstrings[i3] = null;
				}
			}
			catch (Exception)
			{
				Console.WriteLine(i3 + " : " + frmItemList2.l_itemstrings.Length);
			}
		}
		frmItemList2.l_items = frmItemList2.l_items.Where((string x) => !string.IsNullOrEmpty(x)).ToArray();
		frmItemList2.l_itemsindex = frmItemList2.l_itemsindex.Where((string x) => !string.IsNullOrEmpty(x)).ToArray();
		frmItemList2.l_itemstringsindex = frmItemList2.l_itemstringsindex.Where((string x) => !string.IsNullOrEmpty(x)).ToArray();
		frmItemList2.l_itemstrings = frmItemList2.l_itemstrings.Where((string x) => !string.IsNullOrEmpty(x)).ToArray();
		frmItemList2.ReloadItems();
		MessageBox.Show("Update deleted Sucessfully", Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	}

	private void btnDeleteUpdateLang_Click(object sender, EventArgs e)
	{
		frmItemList frmItemList2 = (frmItemList)base.Tag;
		new frmDeleteLanguage((frmItemList)base.Tag, this, (Update)frmItemList2.lstItems.SelectedItems[0].Tag).ShowDialog();
	}

	private void btnStringsEditor_Click(object sender, EventArgs e)
	{
		frmItemList frmItemList2 = (frmItemList)base.Tag;
		new frmStringEditor((frmItemList)base.Tag, this, (Update)frmItemList2.lstItems.SelectedItems[0].Tag).ShowDialog();
	}

	private void btnStringFix_Click(object sender, EventArgs e)
	{
		frmItemList frmItemList2 = (frmItemList)base.Tag;
		Update upd = (Update)frmItemList2.lstItems.SelectedItems[0].Tag;
		List<string> _baseGuids = new List<string>();
		List<string> __baseLangs = new List<string>();
		string[] _baseLangs = new string[26]
		{
			"ar", "cs", "da", "de", "el", "en", "es", "fi", "fr", "he",
			"hu", "it", "ja", "ko", "nl", "no", "pl", "pt", "ptbr", "ru",
			"sk", "sl", "sv", "tr", "zhcn", "zhtw"
		};
		string[] l_itemstringsindex = frmItemList2.l_itemstringsindex;
		foreach (string line in l_itemstringsindex)
		{
			string[] array = _baseLangs;
			foreach (string baseLang in array)
			{
				if (line.Contains(upd.langscode) && line.Split('.')[1].ToLower() == baseLang)
				{
					_baseGuids.Add(line.Split(',')[1]);
					__baseLangs.Add(baseLang);
				}
			}
		}
		string[] baseGuids = _baseGuids.ToArray();
		string[] baseLangs = __baseLangs.ToArray();
		for (int k = 0; k < frmItemList2.l_itemstrings.Length; k++)
		{
			string line2 = frmItemList2.l_itemstrings[k];
			for (int l = 0; l < baseGuids.Length; l++)
			{
				string baseGuid = baseGuids[l];
				if (baseLangs[l] != "en" && line2.Contains(baseGuid))
				{
					string title = line2.Split(new string[1] { "@|" }, StringSplitOptions.None)[0].Split(new char[1] { ',' }, 2)[1];
					string description = line2.Split(new string[1] { "@|" }, StringSplitOptions.None)[1];
					string newLangLine = string.Format(line2.Replace(title, "{0}").Replace(description, "{1}"), TranslateText(upd.getLang("en").title, "en", baseLangs[l]), TranslateText(upd.getLang("en").description, "en", baseLangs[l]));
					frmItemList2.l_itemstrings[k] = newLangLine;
				}
			}
		}
		frmItemList2.ReloadItems();
	}

	public string TranslateText(string input, string inLang, string outLang)
	{
		string _outlang = outLang;
		if (_outlang == "nec")
		{
			_outlang = "ja";
		}
		if (_outlang == "zhcn")
		{
			_outlang = "zh-cn";
		}
		if (_outlang == "zhtw")
		{
			_outlang = "zh-tw";
		}
		if (_outlang == "pt-br")
		{
			_outlang = "pt";
		}
		string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={inLang}&tl={_outlang}&dt=t&q={Uri.EscapeUriString(input)}";
		using WebClient webClient = new WebClient();
		webClient.Encoding = Encoding.UTF8;
		string result = webClient.DownloadString(url);
		dynamic translationItems = new JavaScriptSerializer().Deserialize<List<object>>(result)[0];
		string translation = "";
		foreach (object item in translationItems)
		{
			IEnumerator translationLineString = (item as IEnumerable).GetEnumerator();
			translationLineString.MoveNext();
			translation += $" {Convert.ToString(translationLineString.Current)}";
		}
		if (translation.Length > 1)
		{
			translation = translation.Substring(1);
		}
		return translation;
	}

	private void saveToolStripButton_Click(object sender, EventArgs e)
	{
		frmItemList frmItemList2 = (frmItemList)mdiTabs.SelectedForm;
		if (MessageBox.Show("Do you want to save the changes on this provider?", "Windows Update v4.0 PowerTools", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		// Persist any drag-reordering into the dictionaries (product2items order + keep every file in sync)
		// on the UI thread before the background write, since it reads the ListView.
		frmItemList2.ApplyDisplayOrder();

		// Strip duplicates before anything is written. Repeated add and edit cycles used to compound
		// duplicate lines until the provider stopped loading entirely.
		int duplicatesRemoved = frmItemList2.SanitizeProvider();

		// Coverage gaps are deliberate (an update simply is not held for that locale) so they are not
		// raised here. Only actual damage stops a save.
		int coverageGaps;
		string issues = frmItemList2.ValidateProvider(out coverageGaps);
		if (issues != null)
		{
			if (MessageBox.Show("This provider has damaged entries:\n\n" + issues + "\nSave anyway?", "Windows Update v4.0 PowerTools", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
			{
				return;
			}
		}

		string providerDir = folderBrowserDialogSrc + "\\" + frmItemList2.provider;
		pbBusy.Style = ProgressBarStyle.Marquee;
		new Thread((ThreadStart)delegate
		{
			try
			{
				SaveProviderFiles(providerDir, frmItemList2);
				Invoke((MethodInvoker)delegate
				{
					pbBusy.Style = ProgressBarStyle.Blocks;
					string note = (duplicatesRemoved > 0) ? ("\n\n" + duplicatesRemoved + " duplicate lines were removed.") : "";
					MessageBox.Show("The provider files are updated correctly!" + note, "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				});
			}
			catch (UnauthorizedAccessException)
			{
				Invoke((MethodInvoker)delegate
				{
					pbBusy.Style = ProgressBarStyle.Blocks;
					MessageBox.Show("You don't have writing permissions to save on this files", "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				});
			}
			catch (Exception saveError)
			{
				Exception shown = saveError;
				Invoke((MethodInvoker)delegate
				{
					pbBusy.Style = ProgressBarStyle.Blocks;
					MessageBox.Show("Nothing was written, the provider is unchanged.\n\n" + shown.Message, "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				});
			}
		}).Start();
	}

	// Reports what does not line up across the five dictionary files and offers to drop the
	// product2items references that resolve to nothing. Repairing only edits the in memory copy,
	// so nothing reaches disk until the provider is saved.
	private void repairProviderToolStripMenuItem_Click(object sender, EventArgs e)
	{
		frmItemList list = mdiTabs.SelectedForm as frmItemList;
		if (list == null)
		{
			MessageBox.Show("Open a provider first.", "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}

		int duplicates = list.SanitizeProvider();
		int coverageGaps;
		string issues = list.ValidateProvider(out coverageGaps);

		if (issues == null && duplicates == 0 && coverageGaps == 0)
		{
			MessageBox.Show("This provider is consistent. Nothing needed repairing.", "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}

		string report = "";
		if (duplicates > 0)
		{
			report += duplicates + " duplicate lines were removed.\n";
		}
		if (issues != null)
		{
			report += "\nDamaged entries:\n" + issues;
		}
		if (coverageGaps > 0)
		{
			// Not a fault. These are updates that are simply not held for a given locale.
			report += "\n" + coverageGaps + " product2items references have no matching update in this provider.\nThat is expected for any language whose update files you do not have, and those entries are simply not offered.\n";
		}

		if (coverageGaps == 0)
		{
			MessageBox.Show(report + "\nSave the provider to write this to disk.", "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}

		if (MessageBox.Show(report + "\nRemove those references anyway? Only do this if you never intend to add those updates.", "Windows Update v4.0 PowerTools", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}

		int dropped = list.RepairDanglingReferences();
		MessageBox.Show(dropped + " references were removed.\n\nSave the provider to write this to disk.", "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	}

	// Puts back the .bak files written by the last save. Saving keeps the previous contents of every
	// file alongside it, so this is a single step undo for a save that turned out to be wrong.
	private void restoreBackupToolStripMenuItem_Click(object sender, EventArgs e)
	{
		frmItemList list = mdiTabs.SelectedForm as frmItemList;
		if (list == null)
		{
			MessageBox.Show("Open a provider first.", "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}

		string providerDir = folderBrowserDialogSrc + "\\" + list.provider;
		DateTime newest = DateTime.MinValue;
		int found = 0;
		foreach (string name in DictionaryFileNames)
		{
			string backup = providerDir + "\\" + name + ".bak";
			if (!File.Exists(backup))
			{
				continue;
			}
			found++;
			DateTime stamp = File.GetLastWriteTime(backup);
			if (stamp > newest)
			{
				newest = stamp;
			}
		}

		if (found == 0)
		{
			MessageBox.Show("There is no backup for this provider yet. A backup is written every time you save.", "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}

		if (MessageBox.Show("Put back the files saved at " + newest.ToString() + "?\n\nThis discards everything saved since then.", "Windows Update v4.0 PowerTools", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
		{
			return;
		}

		try
		{
			foreach (string name in DictionaryFileNames)
			{
				string target = providerDir + "\\" + name;
				string backup = target + ".bak";
				if (File.Exists(backup))
				{
					File.Copy(backup, target, true);
				}
			}
		}
		catch (Exception restoreError)
		{
			MessageBox.Show("The backup could not be put back:\n\n" + restoreError.Message, "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}

		MessageBox.Show("The provider files were put back. Close this provider and open it again to load them.", "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	}

	// The five files that make up a provider, in the order they are written.
	private static readonly string[] DictionaryFileNames = new string[5] { "product2items.txt", "itemsindex.txt", "items.txt", "itemstringsindex.txt", "itemstrings.txt" };

	// Writes the five dictionary files as one unit. Every file goes to a temp file first and is only
	// swapped in once all five have been written, so a failure part way through leaves the provider
	// exactly as it was. The catalog parses each file whole, so a half written provider stops loading
	// altogether. The previous contents are kept alongside as .bak.
	private static void SaveProviderFiles(string providerDir, frmItemList list)
	{
		Encoding latin1 = Encoding.GetEncoding("ISO-8859-1");
		string[] names = DictionaryFileNames;
		string[][] payloads = new string[5][] { list.l_product2items, list.l_itemsindex, list.l_items, list.l_itemstringsindex, list.l_itemstrings };
		Encoding[] encodings = new Encoding[5] { latin1, latin1, latin1, latin1, Encoding.Unicode };

		string[] targets = new string[names.Length];
		string[] temps = new string[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			targets[i] = providerDir + "\\" + names[i];
			temps[i] = targets[i] + ".tmp";
		}

		// Phase one. If any of these throw, the real files have not been touched at all.
		try
		{
			for (int i = 0; i < names.Length; i++)
			{
				File.WriteAllLines(temps[i], payloads[i] ?? new string[0], encodings[i]);
			}
		}
		catch
		{
			DeleteTempFiles(temps);
			throw;
		}

		// Phase two. Swap each finished file in, keeping the old contents as .bak.
		for (int i = 0; i < names.Length; i++)
		{
			if (!File.Exists(targets[i]))
			{
				File.Move(temps[i], targets[i]);
				continue;
			}
			try
			{
				File.Replace(temps[i], targets[i], targets[i] + ".bak", true);
			}
			catch (PlatformNotSupportedException)
			{
				// File.Replace is not available on some network shares, and the WUR server is a UNC path.
				ReplaceByCopy(temps[i], targets[i]);
			}
			catch (IOException)
			{
				ReplaceByCopy(temps[i], targets[i]);
			}
		}
	}

	private static void ReplaceByCopy(string temp, string target)
	{
		File.Copy(target, target + ".bak", true);
		File.Copy(temp, target, true);
		File.Delete(temp);
	}

	private static void DeleteTempFiles(string[] temps)
	{
		foreach (string temp in temps)
		{
			try
			{
				if (File.Exists(temp))
				{
					File.Delete(temp);
				}
			}
			catch (IOException)
			{
				// A leftover temp file is harmless, it is overwritten on the next save.
			}
		}
	}

	private void btnEditUpdateLang_Click(object sender, EventArgs e)
	{
		frmItemList frmItemList2 = (frmItemList)base.Tag;
		new frmEditLanguage((frmItemList)base.Tag, this, (Update)frmItemList2.lstItems.SelectedItems[0].Tag).ShowDialog();
	}

	private void btnRefresh_Click(object sender, EventArgs e)
	{
		frmItemList obj = (frmItemList)mdiTabs.SelectedForm;
		obj.ReloadItems();
	}

	private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			frmItemList obj = (frmItemList)base.Tag;
			obj.ReloadItems();
		}
		catch
		{
		}
	}

	private void txtSearch_TextChanged(object sender, EventArgs e)
	{
		frmItemList list = base.Tag as frmItemList;
		if (list == null)
		{
			return;
		}
		string term = txtSearch.Text;
		foreach (ListViewItem item in list.lstItems.Items)
		{
			item.BackColor = (term.Length > 0 && ItemMatches(item, term)) ? Color.LightYellow : Color.Transparent;
		}
	}

	// Matches on every column rather than the title alone, and ignores case. Update codes are mixed
	// case and a provider can hold thousands of rows, so a case sensitive title only search missed
	// most of what people were looking for.
	private static bool ItemMatches(ListViewItem item, string term)
	{
		foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
		{
			if (sub.Text != null && sub.Text.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	private void btnFindNext_Click(object sender, EventArgs e)
	{
		try
		{
			frmItemList frmItemList2 = (frmItemList)base.Tag;
			for (int i = 0; i < frmItemList2.lstItems.Items.Count; i++)
			{
				ListViewItem lvi = frmItemList2.lstItems.Items[i];
				if (frmItemList2.lstItems.SelectedItems.Count > 0)
				{
					if (frmItemList2.lstItems.SelectedItems[0].Index < i && lvi.BackColor == Color.LightYellow)
					{
						lvi.Selected = true;
						lvi.EnsureVisible();
						i = frmItemList2.lstItems.Items.Count;
					}
				}
				else if (lvi.BackColor == Color.LightYellow)
				{
					lvi.Selected = true;
					lvi.EnsureVisible();
					i = frmItemList2.lstItems.Items.Count;
				}
			}
		}
		catch
		{
		}
	}

	private void btnPreviousSearch_Click(object sender, EventArgs e)
	{
		try
		{
			frmItemList frmItemList2 = (frmItemList)base.Tag;
			for (int i = frmItemList2.lstItems.Items.Count - 1; i >= 0; i--)
			{
				ListViewItem lvi = frmItemList2.lstItems.Items[i];
				if (frmItemList2.lstItems.SelectedItems.Count > 0)
				{
					if (frmItemList2.lstItems.SelectedItems[0].Index > i && lvi.BackColor == Color.LightYellow)
					{
						lvi.Selected = true;
						lvi.EnsureVisible();
						i = 0;
					}
				}
				else if (lvi.BackColor == Color.LightYellow)
				{
					lvi.Selected = true;
					lvi.EnsureVisible();
					i = 0;
				}
			}
		}
		catch
		{
		}
	}

	private void mdiTabs_SelectedTabChanged(object sender, EventArgs e)
	{
		try
		{
			base.Tag = (frmItemList)mdiTabs.SelectedForm;
		}
		catch
		{
			base.Tag = null;
		}
	}

	private void SaveRecentFile(string strPath)
	{
		LoadRecentList();
		if (!MRUlist.Contains(strPath))
		{
			MRUlist.Enqueue(strPath);
		}
		while (MRUlist.Count > 5)
		{
			MRUlist.Dequeue();
		}
		RebuildRecentMenu();
		WriteRecentList();
	}

	// Drops an inventory that can no longer be opened, so a folder that has been
	// deleted or moved stops reappearing in the recent list.
	private void RemoveRecentFile(string strPath)
	{
		LoadRecentList();
		if (!MRUlist.Contains(strPath))
		{
			return;
		}
		MRUlist = new Queue<string>(MRUlist.Where(item => item != strPath));
		RebuildRecentMenu();
		WriteRecentList();
	}

	private void RebuildRecentMenu()
	{
		openToolStripButton.DropDownItems.Clear();
		foreach (string strItem in MRUlist)
		{
			string itemPath = strItem;
			ToolStripMenuItem tsRecent = new ToolStripMenuItem(itemPath, null);
			tsRecent.Click += delegate
			{
				folderBrowserDialogSrc = itemPath;
				openInventories(folderBrowserDialogSrc, addHistory: false);
			};
			openToolStripButton.DropDownItems.Add(tsRecent);
		}
	}

	private void WriteRecentList()
	{
		try
		{
			StreamWriter stringToWrite = new StreamWriter(Environment.CurrentDirectory + "\\Recent.txt");
			foreach (string item in MRUlist)
			{
				stringToWrite.WriteLine(item);
			}
			stringToWrite.Flush();
			stringToWrite.Close();
		}
		catch (Exception)
		{
		}
	}

	private void LoadRecentList()
	{
		MRUlist.Clear();
		try
		{
			StreamReader srStream = new StreamReader(Environment.CurrentDirectory + "\\Recent.txt");
			string strLine = "";
			while (InlineAssignHelper(ref strLine, srStream.ReadLine()) != null)
			{
				MRUlist.Enqueue(strLine);
			}
			srStream.Close();
		}
		catch (Exception)
		{
		}
	}

	private static T InlineAssignHelper<T>(ref T target, T value)
	{
		target = value;
		return value;
	}

	private void helpToolStripButton_Click(object sender, EventArgs e)
	{
		new frmAbout().ShowDialog();
	}

	public void openToolStripButton_ButtonClick(object sender, EventArgs e)
	{
		try
		{
			if (Debugger.IsAttached)
			{
				folderBrowserDialog.SelectedPath = "C:\\consumer";
			}
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				folderBrowserDialogSrc = folderBrowserDialog.SelectedPath;
				openInventories(folderBrowserDialog.SelectedPath);
			}
		}
		catch (Exception)
		{
			MessageBox.Show("The folder is not a WUv4 dictionary.", "Windows Update v4.0", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void btnChangeUpdateCode_Click(object sender, EventArgs e)
	{
		frmItemList frmItemList2 = (frmItemList)base.Tag;
		new frmEditCode((frmItemList)base.Tag, this, (Update)frmItemList2.lstItems.SelectedItems[0].Tag).ShowDialog();
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WUv4Powertools.frmMain));
		this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
		this.tmrAgent = new System.Windows.Forms.Timer(this.components);
		this.tsContainer = new System.Windows.Forms.ToolStripContainer();
		this.mdiTabs = new MdiTabControl.TabControl();
		this.statusStandard = new System.Windows.Forms.StatusStrip();
		this.lblItems = new System.Windows.Forms.ToolStripStatusLabel();
		this.pbBusy = new System.Windows.Forms.ToolStripProgressBar();
		this.menuStrip1 = new System.Windows.Forms.MenuStrip();
		this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
		this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
		this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
		this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.restoreBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.repairProviderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
		this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.tbSearch = new System.Windows.Forms.ToolStrip();
		this.btnFindNext = new System.Windows.Forms.ToolStripButton();
		this.btnPreviousSearch = new System.Windows.Forms.ToolStripButton();
		this.txtSearch = new WUv4Powertools.PlaceHolderTextBox();
		this.tbStandard = new System.Windows.Forms.ToolStrip();
		this.openToolStripButton = new System.Windows.Forms.ToolStripSplitButton();
		this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
		this.btnRefresh = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
		this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
		this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
		this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.btnNewUpdate = new System.Windows.Forms.ToolStripButton();
		this.btnEditUpdate = new System.Windows.Forms.ToolStripButton();
		this.btnEditEULA = new System.Windows.Forms.ToolStripButton();
		this.btnDeleteUpdate = new System.Windows.Forms.ToolStripButton();
		this.btnAddUpdateLang = new System.Windows.Forms.ToolStripButton();
		this.btnEditUpdateLang = new System.Windows.Forms.ToolStripButton();
		this.btnDeleteUpdateLang = new System.Windows.Forms.ToolStripButton();
		this.btnStringsEditor = new System.Windows.Forms.ToolStripButton();
		this.btnStringFix = new System.Windows.Forms.ToolStripButton();
		this.btnChangeUpdateCode = new System.Windows.Forms.ToolStripButton();
		this.tsContainer.ContentPanel.SuspendLayout();
		this.tsContainer.TopToolStripPanel.SuspendLayout();
		this.tsContainer.SuspendLayout();
		this.statusStandard.SuspendLayout();
		this.menuStrip1.SuspendLayout();
		this.tbSearch.SuspendLayout();
		this.tbStandard.SuspendLayout();
		base.SuspendLayout();
		this.folderBrowserDialog.Description = "Select a consumer dictionary folder for manage updates";
		this.tmrAgent.Enabled = true;
		this.tmrAgent.Tick += new System.EventHandler(tmrAgent_Tick);
		this.tsContainer.BottomToolStripPanelVisible = false;
		this.tsContainer.ContentPanel.Controls.Add(this.mdiTabs);
		this.tsContainer.ContentPanel.Controls.Add(this.statusStandard);
		this.tsContainer.ContentPanel.Size = new System.Drawing.Size(800, 486);
		this.tsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tsContainer.LeftToolStripPanelVisible = false;
		this.tsContainer.Location = new System.Drawing.Point(0, 0);
		this.tsContainer.Name = "tsContainer";
		this.tsContainer.RightToolStripPanelVisible = false;
		this.tsContainer.Size = new System.Drawing.Size(800, 511);
		this.tsContainer.TabIndex = 15;
		this.tsContainer.Text = "toolStripContainer1";
		this.tsContainer.TopToolStripPanel.Controls.Add(this.menuStrip1);
		this.tsContainer.TopToolStripPanel.Controls.Add(this.tbSearch);
		this.tsContainer.TopToolStripPanel.Controls.Add(this.tbStandard);
		this.mdiTabs.AutoSize = true;
		this.mdiTabs.BackColor = System.Drawing.SystemColors.Control;
		this.mdiTabs.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mdiTabs.Location = new System.Drawing.Point(0, 0);
		this.mdiTabs.MenuRenderer = null;
		this.mdiTabs.Name = "mdiTabs";
		this.mdiTabs.Size = new System.Drawing.Size(800, 464);
		this.mdiTabs.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
		this.mdiTabs.TabBackHighColor = System.Drawing.Color.White;
		this.mdiTabs.TabBackLowColor = System.Drawing.SystemColors.GradientActiveCaption;
		this.mdiTabs.TabBackLowColorDisabled = System.Drawing.SystemColors.ActiveCaption;
		this.mdiTabs.TabBorderEnhanced = true;
		this.mdiTabs.TabCloseButtonImage = null;
		this.mdiTabs.TabCloseButtonImageDisabled = null;
		this.mdiTabs.TabCloseButtonImageHot = null;
		this.mdiTabs.TabCloseButtonSize = new System.Drawing.Size(10, 10);
		this.mdiTabs.TabGlassGradient = true;
		this.mdiTabs.TabIndex = 13;
		this.mdiTabs.SelectedTabChanged += new System.EventHandler(mdiTabs_SelectedTabChanged);
		this.statusStandard.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.lblItems, this.pbBusy });
		this.statusStandard.Location = new System.Drawing.Point(0, 464);
		this.statusStandard.Name = "statusStandard";
		this.statusStandard.Size = new System.Drawing.Size(800, 22);
		this.statusStandard.TabIndex = 12;
		this.statusStandard.Text = "statusStrip1";
		this.lblItems.Name = "lblItems";
		this.lblItems.Size = new System.Drawing.Size(45, 17);
		this.lblItems.Text = "0 items";
		this.pbBusy.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		this.pbBusy.Name = "pbBusy";
		this.pbBusy.Size = new System.Drawing.Size(100, 16);
		this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
		this.menuStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
		this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.fileToolStripMenuItem, this.editToolStripMenuItem, this.toolsToolStripMenuItem, this.helpToolStripMenuItem });
		this.menuStrip1.Location = new System.Drawing.Point(0, 0);
		this.menuStrip1.Name = "menuStrip1";
		this.menuStrip1.Size = new System.Drawing.Size(179, 24);
		this.menuStrip1.TabIndex = 17;
		this.menuStrip1.Text = "menuStrip1";
		this.menuStrip1.Visible = false;
		this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.openToolStripMenuItem, this.toolStripSeparator, this.saveToolStripMenuItem, this.toolStripSeparator1, this.printToolStripMenuItem, this.printPreviewToolStripMenuItem, this.toolStripSeparator3, this.exitToolStripMenuItem });
		this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
		this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
		this.fileToolStripMenuItem.Text = "&File";
		this.openToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("openToolStripMenuItem.Image");
		this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.openToolStripMenuItem.Name = "openToolStripMenuItem";
		this.openToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.O | System.Windows.Forms.Keys.Control;
		this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
		this.openToolStripMenuItem.Text = "&Open";
		this.openToolStripMenuItem.Click += new System.EventHandler(openToolStripButton_ButtonClick);
		this.toolStripSeparator.Name = "toolStripSeparator";
		this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
		this.saveToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("saveToolStripMenuItem.Image");
		this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
		this.saveToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.S | System.Windows.Forms.Keys.Control;
		this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
		this.saveToolStripMenuItem.Text = "&Save";
		this.saveToolStripMenuItem.Click += new System.EventHandler(saveToolStripButton_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
		this.printToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("printToolStripMenuItem.Image");
		this.printToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.printToolStripMenuItem.Name = "printToolStripMenuItem";
		this.printToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.P | System.Windows.Forms.Keys.Control;
		this.printToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
		this.printToolStripMenuItem.Text = "&Print";
		this.printPreviewToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("printPreviewToolStripMenuItem.Image");
		this.printPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
		this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
		this.printPreviewToolStripMenuItem.Text = "Print Pre&view";
		this.toolStripSeparator3.Name = "toolStripSeparator3";
		this.toolStripSeparator3.Size = new System.Drawing.Size(143, 6);
		this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
		this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
		this.exitToolStripMenuItem.Text = "E&xit";
		this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.undoToolStripMenuItem, this.redoToolStripMenuItem, this.toolStripSeparator4, this.cutToolStripMenuItem, this.copyToolStripMenuItem, this.pasteToolStripMenuItem, this.toolStripSeparator5, this.selectAllToolStripMenuItem });
		this.editToolStripMenuItem.Name = "editToolStripMenuItem";
		this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
		this.editToolStripMenuItem.Text = "&Edit";
		this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
		this.undoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Z | System.Windows.Forms.Keys.Control;
		this.undoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
		this.undoToolStripMenuItem.Text = "&Undo";
		this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
		this.redoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Y | System.Windows.Forms.Keys.Control;
		this.redoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
		this.redoToolStripMenuItem.Text = "&Redo";
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		this.toolStripSeparator4.Size = new System.Drawing.Size(141, 6);
		this.cutToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("cutToolStripMenuItem.Image");
		this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
		this.cutToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.X | System.Windows.Forms.Keys.Control;
		this.cutToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
		this.cutToolStripMenuItem.Text = "Cu&t";
		this.copyToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("copyToolStripMenuItem.Image");
		this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
		this.copyToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.C | System.Windows.Forms.Keys.Control;
		this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
		this.copyToolStripMenuItem.Text = "&Copy";
		this.pasteToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("pasteToolStripMenuItem.Image");
		this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
		this.pasteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.V | System.Windows.Forms.Keys.Control;
		this.pasteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
		this.pasteToolStripMenuItem.Text = "&Paste";
		this.toolStripSeparator5.Name = "toolStripSeparator5";
		this.toolStripSeparator5.Size = new System.Drawing.Size(141, 6);
		this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
		this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
		this.selectAllToolStripMenuItem.Text = "Select &All";
		this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.repairProviderToolStripMenuItem, this.restoreBackupToolStripMenuItem, this.customizeToolStripMenuItem, this.optionsToolStripMenuItem });
		this.restoreBackupToolStripMenuItem.Name = "restoreBackupToolStripMenuItem";
		this.restoreBackupToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
		this.restoreBackupToolStripMenuItem.Text = "&Undo Last Save";
		this.restoreBackupToolStripMenuItem.Click += new System.EventHandler(restoreBackupToolStripMenuItem_Click);
		this.repairProviderToolStripMenuItem.Name = "repairProviderToolStripMenuItem";
		this.repairProviderToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
		this.repairProviderToolStripMenuItem.Text = "&Validate and Repair Provider";
		this.repairProviderToolStripMenuItem.Click += new System.EventHandler(repairProviderToolStripMenuItem_Click);
		this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
		this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
		this.toolsToolStripMenuItem.Text = "&Tools";
		this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
		this.customizeToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.customizeToolStripMenuItem.Text = "&Customize";
		this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
		this.optionsToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.optionsToolStripMenuItem.Text = "&Options";
		this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.contentsToolStripMenuItem, this.indexToolStripMenuItem, this.searchToolStripMenuItem, this.toolStripSeparator8, this.aboutToolStripMenuItem });
		this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
		this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
		this.helpToolStripMenuItem.Text = "&Help";
		this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
		this.contentsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
		this.contentsToolStripMenuItem.Text = "&Contents";
		this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
		this.indexToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
		this.indexToolStripMenuItem.Text = "&Index";
		this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
		this.searchToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
		this.searchToolStripMenuItem.Text = "&Search";
		this.toolStripSeparator8.Name = "toolStripSeparator8";
		this.toolStripSeparator8.Size = new System.Drawing.Size(119, 6);
		this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
		this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
		this.aboutToolStripMenuItem.Text = "&About...";
		this.aboutToolStripMenuItem.Click += new System.EventHandler(helpToolStripButton_Click);
		this.tbSearch.Dock = System.Windows.Forms.DockStyle.None;
		this.tbSearch.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.btnFindNext, this.btnPreviousSearch, this.txtSearch });
		this.tbSearch.Location = new System.Drawing.Point(381, 0);
		this.tbSearch.Name = "tbSearch";
		this.tbSearch.Size = new System.Drawing.Size(210, 25);
		this.tbSearch.TabIndex = 16;
		this.btnFindNext.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		this.btnFindNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnFindNext.Image = (System.Drawing.Image)resources.GetObject("btnFindNext.Image");
		this.btnFindNext.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnFindNext.Name = "btnFindNext";
		this.btnFindNext.Size = new System.Drawing.Size(23, 22);
		this.btnFindNext.Text = "Find Next";
		this.btnFindNext.Click += new System.EventHandler(btnFindNext_Click);
		this.btnPreviousSearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		this.btnPreviousSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnPreviousSearch.Image = (System.Drawing.Image)resources.GetObject("btnPreviousSearch.Image");
		this.btnPreviousSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnPreviousSearch.Name = "btnPreviousSearch";
		this.btnPreviousSearch.Size = new System.Drawing.Size(23, 22);
		this.btnPreviousSearch.Text = "Previous Next";
		this.btnPreviousSearch.Click += new System.EventHandler(btnPreviousSearch_Click);
		this.txtSearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Italic);
		this.txtSearch.ForeColor = System.Drawing.Color.Gray;
		this.txtSearch.Name = "txtSearch";
		this.txtSearch.PlaceHolderText = "Search...";
		this.txtSearch.Size = new System.Drawing.Size(150, 25);
		this.txtSearch.TextChanged += new System.EventHandler(txtSearch_TextChanged);
		this.tbStandard.Dock = System.Windows.Forms.DockStyle.None;
		this.tbStandard.Items.AddRange(new System.Windows.Forms.ToolStripItem[18]
		{
			this.openToolStripButton, this.saveToolStripButton, this.btnRefresh, this.toolStripSeparator6, this.cutToolStripButton, this.copyToolStripButton, this.pasteToolStripButton, this.toolStripSeparator2, this.btnNewUpdate, this.btnEditUpdate,
			this.btnEditEULA, this.btnDeleteUpdate, this.btnAddUpdateLang, this.btnEditUpdateLang, this.btnDeleteUpdateLang, this.btnStringsEditor, this.btnStringFix, this.btnChangeUpdateCode
		});
		this.tbStandard.Location = new System.Drawing.Point(3, 0);
		this.tbStandard.Name = "tbStandard";
		this.tbStandard.Size = new System.Drawing.Size(378, 25);
		this.tbStandard.TabIndex = 15;
		this.tbStandard.Text = "toolStrip1";
		this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.openToolStripButton.Image = (System.Drawing.Image)resources.GetObject("openToolStripButton.Image");
		this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.openToolStripButton.Name = "openToolStripButton";
		this.openToolStripButton.Size = new System.Drawing.Size(32, 22);
		this.openToolStripButton.Text = "&Open";
		this.openToolStripButton.ButtonClick += new System.EventHandler(openToolStripButton_ButtonClick);
		this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.saveToolStripButton.Image = (System.Drawing.Image)resources.GetObject("saveToolStripButton.Image");
		this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.saveToolStripButton.Name = "saveToolStripButton";
		this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
		this.saveToolStripButton.Text = "&Save";
		this.saveToolStripButton.Click += new System.EventHandler(saveToolStripButton_Click);
		this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnRefresh.Image = (System.Drawing.Image)resources.GetObject("btnRefresh.Image");
		this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnRefresh.Name = "btnRefresh";
		this.btnRefresh.Size = new System.Drawing.Size(23, 22);
		this.btnRefresh.Text = "Reload";
		this.btnRefresh.Click += new System.EventHandler(btnRefresh_Click);
		this.toolStripSeparator6.Name = "toolStripSeparator6";
		this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
		this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.cutToolStripButton.Image = (System.Drawing.Image)resources.GetObject("cutToolStripButton.Image");
		this.cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.cutToolStripButton.Name = "cutToolStripButton";
		this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
		this.cutToolStripButton.Text = "C&ut";
		this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.copyToolStripButton.Image = (System.Drawing.Image)resources.GetObject("copyToolStripButton.Image");
		this.copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.copyToolStripButton.Name = "copyToolStripButton";
		this.copyToolStripButton.Size = new System.Drawing.Size(23, 22);
		this.copyToolStripButton.Text = "&Copy";
		this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.pasteToolStripButton.Image = (System.Drawing.Image)resources.GetObject("pasteToolStripButton.Image");
		this.pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.pasteToolStripButton.Name = "pasteToolStripButton";
		this.pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
		this.pasteToolStripButton.Text = "&Paste";
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
		this.btnNewUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnNewUpdate.Image = (System.Drawing.Image)resources.GetObject("btnNewUpdate.Image");
		this.btnNewUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnNewUpdate.Name = "btnNewUpdate";
		this.btnNewUpdate.Size = new System.Drawing.Size(23, 22);
		this.btnNewUpdate.Text = "Add New Update";
		this.btnNewUpdate.Click += new System.EventHandler(btnNewUpdate_Click);
		this.btnEditUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnEditUpdate.Image = (System.Drawing.Image)resources.GetObject("btnEditUpdate.Image");
		this.btnEditUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnEditUpdate.Name = "btnEditUpdate";
		this.btnEditUpdate.Size = new System.Drawing.Size(23, 22);
		this.btnEditUpdate.Text = "Edit Update";
		this.btnEditUpdate.Click += new System.EventHandler(btnEditUpdate_Click);
		this.btnEditEULA.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
		this.btnEditEULA.Image = (System.Drawing.Image)resources.GetObject("btnEditUpdate.Image");
		this.btnEditEULA.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnEditEULA.Name = "btnEditEULA";
		this.btnEditEULA.Size = new System.Drawing.Size(70, 22);
		this.btnEditEULA.Text = "Edit EULA";
		this.btnEditEULA.Click += new System.EventHandler(btnEditEULA_Click);
		this.btnDeleteUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnDeleteUpdate.Image = (System.Drawing.Image)resources.GetObject("btnDeleteUpdate.Image");
		this.btnDeleteUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnDeleteUpdate.Name = "btnDeleteUpdate";
		this.btnDeleteUpdate.Size = new System.Drawing.Size(23, 22);
		this.btnDeleteUpdate.Text = "Delete Update";
		this.btnDeleteUpdate.Click += new System.EventHandler(btnDeleteUpdate_Click);
		this.btnAddUpdateLang.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnAddUpdateLang.Image = (System.Drawing.Image)resources.GetObject("btnAddUpdateLang.Image");
		this.btnAddUpdateLang.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnAddUpdateLang.Name = "btnAddUpdateLang";
		this.btnAddUpdateLang.Size = new System.Drawing.Size(23, 22);
		this.btnAddUpdateLang.Text = "Add Language for Update";
		this.btnAddUpdateLang.Click += new System.EventHandler(btnAddUpdateLang_Click);
		this.btnEditUpdateLang.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnEditUpdateLang.Image = (System.Drawing.Image)resources.GetObject("btnEditUpdateLang.Image");
		this.btnEditUpdateLang.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnEditUpdateLang.Name = "btnEditUpdateLang";
		this.btnEditUpdateLang.Size = new System.Drawing.Size(23, 22);
		this.btnEditUpdateLang.Text = "Edit Language for Update";
		this.btnEditUpdateLang.Click += new System.EventHandler(btnEditUpdateLang_Click);
		this.btnDeleteUpdateLang.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnDeleteUpdateLang.Image = (System.Drawing.Image)resources.GetObject("btnDeleteUpdateLang.Image");
		this.btnDeleteUpdateLang.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnDeleteUpdateLang.Name = "btnDeleteUpdateLang";
		this.btnDeleteUpdateLang.Size = new System.Drawing.Size(23, 22);
		this.btnDeleteUpdateLang.Text = "Delete Language for Update";
		this.btnDeleteUpdateLang.Click += new System.EventHandler(btnDeleteUpdateLang_Click);
		this.btnStringsEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnStringsEditor.Image = (System.Drawing.Image)resources.GetObject("btnStringsEditor.Image");
		this.btnStringsEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnStringsEditor.Name = "btnStringsEditor";
		this.btnStringsEditor.Size = new System.Drawing.Size(23, 22);
		this.btnStringsEditor.Text = "String Editor";
		this.btnStringsEditor.Click += new System.EventHandler(btnStringsEditor_Click);
		this.btnStringFix.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnStringFix.Image = (System.Drawing.Image)resources.GetObject("btnStringFix.Image");
		this.btnStringFix.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnStringFix.Name = "btnStringFix";
		this.btnStringFix.Size = new System.Drawing.Size(23, 22);
		this.btnStringFix.Text = "Fix Strings";
		this.btnStringFix.Click += new System.EventHandler(btnStringFix_Click);
		this.btnChangeUpdateCode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.btnChangeUpdateCode.Image = (System.Drawing.Image)resources.GetObject("btnChangeUpdateCode.Image");
		this.btnChangeUpdateCode.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.btnChangeUpdateCode.Name = "btnChangeUpdateCode";
		this.btnChangeUpdateCode.Size = new System.Drawing.Size(23, 22);
		this.btnChangeUpdateCode.Text = "Change Update Code";
		this.btnChangeUpdateCode.Click += new System.EventHandler(btnChangeUpdateCode_Click);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		base.ClientSize = new System.Drawing.Size(800, 511);
		base.Controls.Add(this.tsContainer);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.KeyPreview = true;
		base.MainMenuStrip = this.menuStrip1;
		this.MinimumSize = new System.Drawing.Size(480, 540);
		base.Name = "frmMain";
		this.Text = "Windows Update v4.0 PowerTools";
		base.Load += new System.EventHandler(frmMain_Load);
		this.tsContainer.ContentPanel.ResumeLayout(false);
		this.tsContainer.ContentPanel.PerformLayout();
		this.tsContainer.TopToolStripPanel.ResumeLayout(false);
		this.tsContainer.TopToolStripPanel.PerformLayout();
		this.tsContainer.ResumeLayout(false);
		this.tsContainer.PerformLayout();
		this.statusStandard.ResumeLayout(false);
		this.statusStandard.PerformLayout();
		this.menuStrip1.ResumeLayout(false);
		this.menuStrip1.PerformLayout();
		this.tbSearch.ResumeLayout(false);
		this.tbSearch.PerformLayout();
		this.tbStandard.ResumeLayout(false);
		this.tbStandard.PerformLayout();
		base.ResumeLayout(false);
	}
}
