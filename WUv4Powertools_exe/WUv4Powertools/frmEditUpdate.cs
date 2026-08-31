using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using AdvancedWizardControl.Enums;
using AdvancedWizardControl.Wizard;
using AdvancedWizardControl.WizardPages;
using WUv4Powertools.Properties;

namespace WUv4Powertools;

public class frmEditUpdate : Form
{
	private frmItemList frmItemList;

	private frmMain frmMain;

	private bool isWindows;

	private bool is9x;

	private Update upd;

	private string[] line_split = new string[0];

	private int[] line_int = new int[0];

	private List<string> codeIndex = new List<string>();

	private Guid fileGuid = Guid.NewGuid();

	private Guid langGuid = Guid.NewGuid();

	private string[] baseLangs = new string[27]
	{
		"ar", "cs", "da", "de", "el", "en", "es", "fi", "fr", "he",
		"hu", "it", "ja", "ko", "nec", "nl", "no", "pl", "pt", "ptbr",
		"ru", "sk", "sl", "sv", "tr", "zhcn", "zhtw"
	};

	private string[] langGuids = new string[27]
	{
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper(),
		Guid.NewGuid().ToString().ToUpper()
	};

	private IContainer components;

	private AdvancedWizard advancedWizard1;

	private AdvancedWizardPage advancedWizardPage2;

	private CheckBox chkExclusive;

	private CheckBox chkCritical;

	private Label lblExtras;

	private Label lblGroup;

	private ComboBox cmbGroup;

	private Label lblHelp0;

	private CheckBox chkEULARequired;

	private AdvancedWizardPage advancedWizardPage3;

	private TextBox txtArguments;

	private Label lblArguments;

	private TextBox txtDetection;

	private Label lblDetection;

	private CheckBox chkRebootReq;

	private DateTimePicker cmbDate;

	private Label lblLanguage;

	private Label lblHelp1;

	// Prerequisite editing (feature: add/remove prerequisites on any update)
	private Label lblPrereqs;
	private ListBox lstPrereqs;
	private Button btnAddPrereq;
	private Button btnRemovePrereq;
	// Selected prerequisite update codes (provider-agnostic update codes, e.g. "811630_W98_5928").
	private List<string> prereqCodes = new List<string>();

	// IE OS targeting (edit which operating systems an IE update is displayed on).
	private bool isIEUpdate = false;
	private Label lblIEOS;
	private readonly Dictionary<string, CheckBox> ieOSChecks = new Dictionary<string, CheckBox>();
	// The OS names, in order, that each IE provider is allowed to target (static mapping, matches the Add wizard).
	private static readonly string[] AllIEOSes = { "Windows 98", "Windows ME", "Windows 2000", "Windows XP", "Windows Server 2003" };

	public frmEditUpdate(frmItemList frmItemList, frmMain frmMain, Update upd)
	{
		this.upd = upd;
		this.frmMain = frmMain;
		this.frmItemList = frmItemList;
		InitializeComponent();
	}

	private void advancedWizard1_Cancel(object sender, EventArgs e)
	{
		Dispose();
	}

	private void frmAddUpdate_Load(object sender, EventArgs e)
	{
		line_split = upd.itemlines[0].Split(new string[1] { "@|" }, StringSplitOptions.None);
		cmbGroup.Text = upd.group.ToString();
		chkCritical.Checked = upd.critical;
		chkExclusive.Checked = upd.exclusive;
		txtDetection.Text = line_split[4];
		XmlDocument installation = new XmlDocument();
		installation.Load(new MemoryStream(Encoding.UTF8.GetBytes(line_split[5])));
		DateTime myDate = DateTime.ParseExact(line_split[9].Split('T')[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
		cmbDate.Value = myDate;
		
		// Fix: Add null check for switches element
		var switchesElements = installation.GetElementsByTagName("switches");
		if (switchesElements != null && switchesElements.Count > 0)
		{
			txtArguments.Text = switchesElements[0].InnerXml;
		}
		else
		{
			txtArguments.Text = string.Empty;
		}
		
		// Fix: Add null check for installation element and needsReboot attribute
		var installationElements = installation.GetElementsByTagName("installation");
		if (installationElements != null && installationElements.Count > 0 && 
		    installationElements[0].Attributes["needsReboot"] != null)
		{
			chkRebootReq.Checked = installationElements[0].Attributes["needsReboot"].Value == "1";
		}
		else
		{
			chkRebootReq.Checked = false;
		}
		
		// Auto-detect if we're editing a driver
		if (frmItemList != null && frmItemList.isDriverProvider)
		{
			// Hide EULA checkbox for drivers (drivers don't have EULAs)
			if (chkEULARequired != null)
			{
				chkEULARequired.Visible = false;
			}
			
			// Adjust UI labels for driver updates
			lblDetection.Text = "Hardware IDs:";
			lblArguments.Text = "INF Command:";
			lblGroup.Text = "Category (Hardware):";
			
			// For drivers, ensure group is 90700 and make it read-only
			cmbGroup.Text = "90700";
			cmbGroup.Enabled = false;
		}
		else
		{
			// Show EULA checkbox for Windows updates
			if (chkEULARequired != null)
			{
				chkEULARequired.Visible = true;
			}
			
			// Standard labels for Windows updates
			lblDetection.Text = "Detection:";
			lblArguments.Text = "Arguments:";
			lblGroup.Text = "Group:";
			
			// For OS updates, enable group selection
			cmbGroup.Enabled = true;
		}
		
		osDetect(firstTime: true);
		LoadPrerequisites();

		// IE updates: let the user edit which operating systems the update is displayed on.
		if (frmItemList != null && (frmItemList.provider == "ie50x" || frmItemList.provider == "ie55x" || frmItemList.provider == "ie60x"))
		{
			isIEUpdate = true;
			InitializeIEOSEditControls();
		}
	}

	private List<string> AllowedIEOSes()
	{
		switch (frmItemList.provider)
		{
			case "ie50x": return new List<string> { "Windows 98", "Windows 2000" };
			case "ie55x": return new List<string> { "Windows 98", "Windows ME", "Windows 2000" };
			default: return new List<string>(AllIEOSes); // ie60x: all five
		}
	}

	// The platform substring that identifies each OS inside an itemID.
	private static string IEOSPlatformToken(string os)
	{
		switch (os)
		{
			case "Windows 98": return ".ver_platform_win32_windows.4.10.x86.";
			case "Windows ME": return ".ver_platform_win32_windows.4.90.x86.";
			case "Windows 2000": return ".ver_platform_win32_nt.5.0.x86.";
			case "Windows XP": return ".ver_platform_win32_nt.5.1.x86.";
			case "Windows Server 2003": return ".ver_platform_win32_nt.5.2.x86.";
			default: return null;
		}
	}

	// Does this update currently have any itemsindex line for the given OS?
	private bool IEOSIsCovered(string os)
	{
		string token = IEOSPlatformToken(os);
		string[] idx = frmItemList.l_itemsindex;
		if (idx == null || token == null) return false;
		foreach (string line in idx)
		{
			if (string.IsNullOrEmpty(line) || !LineBelongsToThisUpdate(line)) continue;
			if (ItemsIndexItemId(line).IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0) return true;
		}
		return false;
	}

	private void InitializeIEOSEditControls()
	{
		lblIEOS = new Label
		{
			AutoSize = true,
			Location = new System.Drawing.Point(23, 292),
			Text = "Display on Operating Systems:"
		};
		this.advancedWizardPage2.Controls.Add(lblIEOS);

		List<string> allowed = AllowedIEOSes();
		int x = 23;
		int y = 312;
		foreach (string os in allowed)
		{
			CheckBox chk = new CheckBox
			{
				AutoSize = true,
				Location = new System.Drawing.Point(x, y),
				Text = os,
				Checked = IEOSIsCovered(os),
				Tag = os
			};
			this.advancedWizardPage2.Controls.Add(chk);
			ieOSChecks[os] = chk;
			x += os.Length > 12 ? 130 : 95;
			if (x > 360) { x = 23; y += 24; }
		}
	}

	private void osDetect(bool firstTime)
	{
		if (firstTime)
		{
			switch (frmItemList.provider)
			{
			case "win98se":
				isWindows = true;
				is9x = true;
				break;
			case "winme":
				isWindows = true;
				is9x = true;
				break;
			case "win2k":
				isWindows = true;
				is9x = false;
				break;
			case "winxp":
				isWindows = true;
				is9x = false;
				break;
			case "netserver":
				isWindows = true;
				is9x = false;
				break;
			default:
				isWindows = false;
				break;
			}
		}
	}

	private void cmbOS_SelectedIndexChanged(object sender, EventArgs e)
	{
		osDetect(firstTime: false);
	}

	private async void advancedWizard1_Finish(object sender, EventArgs e)
	{
		// Capture the dictionaries before changing them, so this can be taken back with Undo.
		frmItemList.PushUndoState();
		if (txtDetection.Text != null)
		{
			// Capture all UI values on the UI thread before entering Task.Run
			string _cmbGroup = cmbGroup.Text;
			bool _chkCritical = chkCritical.Checked;
			bool _chkExclusive = chkExclusive.Checked;
			bool _chkRebootReq = chkRebootReq.Checked;
			DateTime _cmbDateValue = cmbDate.Value;
			string _txtArguments = txtArguments.Text;
			string _txtDetection = txtDetection.Text;

			// Capture IE OS checkbox state on the UI thread (can't read controls from the background thread).
			List<string> _desiredIEOSes = null;
			if (isIEUpdate)
			{
				_desiredIEOSes = new List<string>();
				foreach (var kv in ieOSChecks)
				{
					if (kv.Value.Checked) _desiredIEOSes.Add(kv.Key);
				}
			}

			await Task.Run(delegate
			{
				// Add/remove itemsindex + product2items entries so the IE update is shown on exactly the
				// checked operating systems. Done before prerequisites so new OS lines get their deps too.
				if (isIEUpdate) ApplyIEOSChanges(_desiredIEOSes);

				for (int i = 0; i < upd.itemlines.Length; i++)
				{
					string[] array = upd.itemlines[i].Split(new string[1] { "@|" }, StringSplitOptions.None);
					array[3] = _cmbGroup;
					array[7] = (_chkCritical ? "3" : "4");
					array[9] = _cmbDateValue.ToString("yyyy-MM-ddTHH:mm:ss.ffff");
					array[10] = (_chkExclusive ? "1" : "0");
					array[4] = _txtDetection;
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(array[5])));
					
					// Fix: Add null check for switches element
					var switchesElements = xmlDocument.GetElementsByTagName("switches");
					if (switchesElements != null && switchesElements.Count > 0)
					{
						switchesElements[0].InnerXml = _txtArguments;
					}
					
					// Fix: Add null check for installation element and needsReboot attribute
					var installationElements = xmlDocument.GetElementsByTagName("installation");
					if (installationElements != null && installationElements.Count > 0)
					{
						var needsRebootAttr = installationElements[0].Attributes["needsReboot"];
						if (needsRebootAttr != null)
						{
							needsRebootAttr.Value = (_chkRebootReq ? "1" : "0");
						}
						else
						{
							// Create the attribute if it doesn't exist
							var attr = xmlDocument.CreateAttribute("needsReboot");
							attr.Value = (_chkRebootReq ? "1" : "0");
							installationElements[0].Attributes.Append(attr);
						}
					}
					
					array[5] = xmlDocument.OuterXml;
					string text = string.Join("@|", array);
					frmItemList.l_items[upd.itemindexes[i]] = text;
				}

				// Persist prerequisite changes into itemsindex (per-locale, matched by platform prefix).
				// Runs on the background thread — itemsindex can have ~10k lines, so keep it off the UI thread.
				WritePrerequisites();
			});

			frmItemList.p_items = 0;
			frmItemList.u_items = null;
			frmItemList.lstItemCol = new List<ListViewItem>();
			frmItemList.lstItems.Items.Clear();
			
			// Check if BackgroundWorker is busy before starting it
			if (!frmItemList.bw.IsBusy)
			{
				frmItemList.bw.RunWorkerAsync();
			}
			
			MessageBox.Show("Update edited Sucessfully", frmMain.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			Dispose();
		}
		else
		{
			MessageBox.Show("You need to complete information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void chkCritical_CheckedChanged(object sender, EventArgs e)
	{
		bool ce = !chkCritical.Checked;
		cmbGroup.Enabled = ce;
		if (chkCritical.Checked)
		{
			cmbGroup.SelectedIndex = 0;
		}
	}

	// ================= Prerequisite editing =================
	// Prerequisites live in itemsindex.txt after the "@|" as a comma-separated list of prerequisite
	// itemIDs (minus the provider prefix). They must match the dependent's OS + locale, so we compute
	// them per dependent line by finding a prerequisite variant that shares the same platform/locale prefix.

	// itemsindex line = "<itemID>,<GUID>@|<dep1>,<dep2>,...". Returns the itemID (incl. provider prefix).
	private static string ItemsIndexItemId(string line)
	{
		string beforeAt = line.Split(new string[] { "@|" }, StringSplitOptions.None)[0];
		int lastComma = beforeAt.LastIndexOf(',');
		return (lastComma > 0) ? beforeAt.Substring(0, lastComma) : beforeAt;
	}

	// Returns the dependency field (after "@|"), or "" if none.
	private static string ItemsIndexDeps(string line)
	{
		string[] parts = line.Split(new string[] { "@|" }, StringSplitOptions.None);
		return (parts.Length > 1) ? parts[1] : "";
	}

	// Extract the update code from an itemID: the token after "com_microsoft." up to the next '.'.
	private static string CodeFromItemId(string itemId)
	{
		if (string.IsNullOrEmpty(itemId)) return null;
		int idx = itemId.IndexOf("com_microsoft.", StringComparison.Ordinal);
		if (idx < 0) return null;
		string tail = itemId.Substring(idx + "com_microsoft.".Length);
		int dot = tail.IndexOf('.');
		return (dot >= 0) ? tail.Substring(0, dot) : tail;
	}

	// True if this itemsindex line belongs to the update being edited.
	private bool LineBelongsToThisUpdate(string line)
	{
		string itemId = ItemsIndexItemId(line);
		// itemsindex stores the code lowercased; items.txt (upd.code) is mixed-case, so compare loosely.
		return itemId.IndexOf("com_microsoft." + upd.code + ".", StringComparison.OrdinalIgnoreCase) >= 0;
	}

	// Load the current prerequisite codes from this update's itemsindex lines.
	private void LoadPrerequisites()
	{
		prereqCodes.Clear();
		string[] idx = frmItemList.l_itemsindex;
		if (idx != null)
		{
			foreach (string line in idx)
			{
				if (string.IsNullOrEmpty(line) || !LineBelongsToThisUpdate(line)) continue;
				string deps = ItemsIndexDeps(line);
				if (string.IsNullOrWhiteSpace(deps)) continue;
				foreach (string dep in deps.Split(','))
				{
					if (string.IsNullOrWhiteSpace(dep)) continue;
					string c = CodeFromItemId(dep.Trim());
					if (!string.IsNullOrEmpty(c) && !prereqCodes.Contains(c)) prereqCodes.Add(c);
				}
			}
		}
		RefreshPrereqList();
	}

	private void RefreshPrereqList()
	{
		if (lstPrereqs == null) return;
		lstPrereqs.Items.Clear();
		foreach (string c in prereqCodes) lstPrereqs.Items.Add(TitleForCode(c));
		// One prerequisite is the limit, so there is nothing to add once one is chosen.
		if (btnAddPrereq != null) btnAddPrereq.Enabled = prereqCodes.Count == 0;
	}

	private void btnAddPrereq_Click(object sender, EventArgs e)
	{
		// Collect the distinct update codes in this provider (O(n) via a HashSet), excluding this update
		// and codes already chosen. HashSet keeps the "Add..." dialog responsive on large providers (~10k lines).
		var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var already = new HashSet<string>(prereqCodes, StringComparer.OrdinalIgnoreCase);
		List<string> available = new List<string>();
		string[] idx = frmItemList.l_itemsindex;
		if (idx != null)
		{
			foreach (string line in idx)
			{
				if (string.IsNullOrEmpty(line)) continue;
				string c = CodeFromItemId(ItemsIndexItemId(line));
				if (string.IsNullOrEmpty(c) || string.Equals(c, upd.code, StringComparison.OrdinalIgnoreCase) || already.Contains(c)) continue;
				if (seen.Add(c)) available.Add(c);
			}
		}
		available.Sort(StringComparer.OrdinalIgnoreCase);
		if (available.Count == 0)
		{
			MessageBox.Show("No other updates are available in this provider to use as prerequisites.", "Add Prerequisite", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}
		if (prereqCodes.Count > 0)
		{
			MessageBox.Show("An update can only have one prerequisite. Remove the current one first.",
				"Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}
		foreach (string c in ShowPrereqPicker(available))
		{
			if (!prereqCodes.Contains(c)) prereqCodes.Add(c);
			break;
		}
		RefreshPrereqList();
	}

	private void btnRemovePrereq_Click(object sender, EventArgs e)
	{
		int chosen = lstPrereqs.SelectedIndex;
		if (chosen < 0 || chosen >= prereqCodes.Count) return;
		prereqCodes.RemoveAt(chosen);
		RefreshPrereqList();
	}

	// Simple modal multi-select picker built in code (no separate designer file needed).
	// The update title as shown in the main list. Falls back to the code when the update is not
	// loaded, so something recognisable is always displayed.
	private string TitleForCode(string code)
	{
		if (string.IsNullOrEmpty(code)) return code;
		if (frmItemList != null && frmItemList.lstItemCol != null)
		{
			foreach (ListViewItem row in frmItemList.lstItemCol)
			{
				Update other = row.Tag as Update;
				if (other != null && string.Equals(other.code, code, StringComparison.OrdinalIgnoreCase))
				{
					return string.IsNullOrEmpty(row.Text) ? code : row.Text;
				}
			}
		}
		return code;
	}

	private List<string> ShowPrereqPicker(List<string> available)
	{
		List<string> result = new List<string>();
		using (Form dlg = new Form())
		{
			dlg.Text = "Select Prerequisite";
			dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
			dlg.StartPosition = FormStartPosition.CenterParent;
			dlg.MinimizeBox = false;
			dlg.MaximizeBox = false;
			dlg.ShowIcon = false;
			dlg.ShowInTaskbar = false;
			dlg.ClientSize = new System.Drawing.Size(360, 320);
			// A plain list, because only one prerequisite is allowed. Titles are shown, and the code
			// each one belongs to is kept alongside so the right value comes back.
			ListBox clb = new ListBox
			{
				Location = new System.Drawing.Point(10, 10),
				Size = new System.Drawing.Size(340, 260),
				// Update titles run long, so they can be scrolled sideways rather than cut off.
				HorizontalScrollbar = true
			};
			foreach (string c in available) clb.Items.Add(TitleForCode(c));
			dlg.Controls.Add(clb);
			Button ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new System.Drawing.Point(194, 282), Size = new System.Drawing.Size(75, 25) };
			Button cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(275, 282), Size = new System.Drawing.Size(75, 25) };
			dlg.Controls.Add(ok);
			dlg.Controls.Add(cancel);
			dlg.AcceptButton = ok;
			dlg.CancelButton = cancel;
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				if (clb.SelectedIndex >= 0 && clb.SelectedIndex < available.Count)
				{
					result.Add(available[clb.SelectedIndex]);
				}
			}
		}
		return result;
	}

	// The itemID prefix up to and including ".x86.<locale>." — identifies the platform + locale, ignoring
	// service pack / build, so a prerequisite for the "same OS + locale" matches even at a different SP.
	private static string LocalePrefix(string itemId)
	{
		int cmIdx = itemId.IndexOf("com_microsoft.", StringComparison.Ordinal);
		string prefix = (cmIdx >= 0) ? itemId.Substring(0, cmIdx) : itemId;
		int x86 = itemId.IndexOf(".x86.", StringComparison.OrdinalIgnoreCase);
		if (x86 >= 0)
		{
			int locStart = x86 + ".x86.".Length;
			int locEnd = itemId.IndexOf('.', locStart);
			if (locEnd > locStart) prefix = itemId.Substring(0, locEnd + 1);
		}
		return prefix;
	}

	// Rewrite the "@|" dependency field of every itemsindex line belonging to this update, using the
	// currently selected prerequisite codes (empty when none). O(n): builds a (localePrefix|code)->itemID
	// lookup once so it stays fast even on providers with ~10k itemsindex lines. Runs on a background thread.
	private void WritePrerequisites()
	{
		string[] idx = frmItemList.l_itemsindex;
		if (idx == null) return;
		string provPrefix = frmItemList.provider + ".";

		// Build the lookup once: for each itemsindex line, key = "<localePrefix lower><code lower>".
		var lookup = new Dictionary<string, string>();
		foreach (string line in idx)
		{
			if (string.IsNullOrEmpty(line)) continue;
			string itemId = ItemsIndexItemId(line);
			string code = CodeFromItemId(itemId);
			if (string.IsNullOrEmpty(code)) continue;
			string key = LocalePrefix(itemId).ToLowerInvariant() + "" + code.ToLowerInvariant();
			if (!lookup.ContainsKey(key)) lookup[key] = itemId; // first match per platform+locale+code
		}

		for (int i = 0; i < idx.Length; i++)
		{
			string line = idx[i];
			if (string.IsNullOrEmpty(line) || !LineBelongsToThisUpdate(line)) continue;
			string itemId = ItemsIndexItemId(line);
			string prefixLower = LocalePrefix(itemId).ToLowerInvariant();
			List<string> deps = new List<string>();
			foreach (string pc in prereqCodes)
			{
				if (!lookup.TryGetValue(prefixLower + "" + pc.ToLowerInvariant(), out string prereqItemId)) continue;
				string depNoPrefix = prereqItemId.StartsWith(provPrefix, StringComparison.OrdinalIgnoreCase)
					? prereqItemId.Substring(provPrefix.Length)
					: prereqItemId;
				if (!deps.Contains(depNoPrefix)) deps.Add(depNoPrefix);
			}
			string beforeAt = line.Split(new string[] { "@|" }, StringSplitOptions.None)[0];
			idx[i] = beforeAt + "@|" + string.Join(",", deps);
		}
	}

	// ================= Edit which OSes an IE update is displayed on =================

	// GUID of an itemsindex line: "<itemId>,<GUID>@|..." — the part after the last comma before "@|".
	private static string ItemsIndexGuid(string line)
	{
		string beforeAt = line.Split(new string[] { "@|" }, StringSplitOptions.None)[0];
		int c = beforeAt.LastIndexOf(',');
		return (c >= 0) ? beforeAt.Substring(c + 1) : "";
	}

	// The locale token of an itemID (the segment after ".x86.").
	private static string LocaleOf(string itemId)
	{
		int x = itemId.IndexOf(".x86.", StringComparison.OrdinalIgnoreCase);
		if (x < 0) return null;
		int s = x + ".x86.".Length;
		int e = itemId.IndexOf('.', s);
		return (e > s) ? itemId.Substring(s, e - s) : itemId.Substring(s);
	}

	// Build the itemID(s) (minus provider prefix) for an IE update on a given OS + locale. XP/2003 produce
	// a family-wildcard entry plus one per service pack, matching the formats the Catalog queries expect.
	private static List<string> BuildIEItemIds(string product, string os, string loc, string code, string ver)
	{
		var r = new List<string>();
		string tail = "com_microsoft." + code + "." + ver; // ver may be "" (update with no version)
		switch (os)
		{
			case "Windows 98":
				r.Add($"{product}.ver_platform_win32_windows.4.10.x86.{loc}......{tail}");
				break;
			case "Windows ME":
				r.Add($"{product}.ver_platform_win32_windows.4.90.x86.{loc}...3000...{tail}");
				break;
			case "Windows 2000":
				r.Add($"{product}.ver_platform_win32_nt.5.0.x86.{loc}...2195...{tail}");
				break;
			case "Windows XP":
				r.Add($"{product}.ver_platform_win32_nt.5.1.x86.{loc}.ver_nt_workstation..2600...{tail}");
				foreach (string sp in new[] { "0", "1", "2" })
					r.Add($"{product}.ver_platform_win32_nt.5.1.x86.{loc}.ver_nt_workstation..2600.{sp}.0.{tail}");
				break;
			case "Windows Server 2003":
				r.Add($"{product}.ver_platform_win32_nt.5.2.x86.{loc}.ver_nt_server..3790...{tail}");
				foreach (string sp in new[] { "0", "1" })
					r.Add($"{product}.ver_platform_win32_nt.5.2.x86.{loc}.ver_nt_server..3790.{sp}.0.{tail}");
				break;
		}
		return r;
	}

	// The product2items key for a value itemID = the itemID with provider prefix but without the trailing
	// ".com_microsoft.<code>.<ver>" — verified to match the real key formats for every OS.
	private static string Product2ItemsKey(string provider, string valueMinusProvider)
	{
		int cm = valueMinusProvider.IndexOf(".com_microsoft.", StringComparison.OrdinalIgnoreCase);
		string keyBody = (cm >= 0) ? valueMinusProvider.Substring(0, cm) : valueMinusProvider;
		return provider + "." + keyBody;
	}

	private static void AddToProduct2Items(List<string> p2i, string provider, string valueMinusProvider)
	{
		string key = Product2ItemsKey(provider, valueMinusProvider);
		for (int i = 0; i < p2i.Count; i++)
		{
			string line = p2i[i];
			if (string.IsNullOrEmpty(line)) continue;
			int firstComma = line.IndexOf(',');
			string lineKey = (firstComma >= 0) ? line.Substring(0, firstComma) : line;
			if (string.Equals(lineKey, key, StringComparison.OrdinalIgnoreCase))
			{
				if (line.IndexOf(valueMinusProvider, StringComparison.OrdinalIgnoreCase) < 0)
					p2i[i] = line + "," + valueMinusProvider;
				return;
			}
		}
		p2i.Add(key + "," + valueMinusProvider);
	}

	// Add/remove itemsindex + product2items entries so this update is displayed on exactly the chosen OSes.
	private void ApplyIEOSChanges(List<string> desiredOSes)
	{
		string provider = frmItemList.provider;
		string provPrefix = provider + ".";
		var idx = new List<string>(frmItemList.l_itemsindex ?? new string[0]);
		var p2i = new List<string>(frmItemList.l_product2items ?? new string[0]);

		// Parse the update's existing itemsindex lines: per-locale GUID (reused for new OS variants) + product/code/ver.
		var localeGuid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		string product = null, code = null, ver = null;
		foreach (string line in idx)
		{
			if (string.IsNullOrEmpty(line) || !LineBelongsToThisUpdate(line)) continue;
			string itemId = ItemsIndexItemId(line);
			string loc = LocaleOf(itemId);
			if (loc != null && !localeGuid.ContainsKey(loc)) localeGuid[loc] = ItemsIndexGuid(line);
			if (product == null)
			{
				string noProv = itemId.StartsWith(provPrefix, StringComparison.OrdinalIgnoreCase) ? itemId.Substring(provPrefix.Length) : itemId;
				int d = noProv.IndexOf('.');
				product = (d > 0) ? noProv.Substring(0, d) : noProv;
				int cm = itemId.IndexOf("com_microsoft.", StringComparison.Ordinal);
				if (cm >= 0)
				{
					string t = itemId.Substring(cm + "com_microsoft.".Length); // "<code>.<ver>"
					int dd = t.IndexOf('.');
					code = (dd >= 0) ? t.Substring(0, dd) : t;
					ver = (dd >= 0) ? t.Substring(dd + 1) : "";
				}
			}
		}
		if (product == null || code == null || localeGuid.Count == 0) return; // nothing to work from

		var current = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string os in AllIEOSes) if (IEOSIsCovered(os)) current.Add(os);
		var desired = new HashSet<string>(desiredOSes ?? new List<string>(), StringComparer.OrdinalIgnoreCase);
		var allowed = new HashSet<string>(AllowedIEOSes(), StringComparer.OrdinalIgnoreCase);

		foreach (string os in AllIEOSes)
		{
			string token = IEOSPlatformToken(os);

			// REMOVE: unchecked OS that is currently covered.
			if (current.Contains(os) && !desired.Contains(os))
			{
				idx.RemoveAll(line => !string.IsNullOrEmpty(line) && LineBelongsToThisUpdate(line)
					&& ItemsIndexItemId(line).IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0);
				var newP2i = new List<string>(p2i.Count);
				foreach (string line in p2i)
				{
					if (string.IsNullOrEmpty(line)) { newP2i.Add(line); continue; }
					string[] parts = line.Split(',');
					var kept = new List<string> { parts[0] };
					for (int j = 1; j < parts.Length; j++)
					{
						bool isThisUpdateOnThisOS = string.Equals(CodeFromItemId(parts[j]), code, StringComparison.OrdinalIgnoreCase)
							&& parts[j].IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
						if (!isThisUpdateOnThisOS) kept.Add(parts[j]);
					}
					if (kept.Count > 1) newP2i.Add(string.Join(",", kept)); // drop lines left with only a key
				}
				p2i = newP2i;
			}

			// ADD: checked+allowed OS that is not yet covered.
			if (desired.Contains(os) && allowed.Contains(os) && !current.Contains(os))
			{
				foreach (var kv in localeGuid)
				{
					foreach (string builtId in BuildIEItemIds(product, os, kv.Key, code, ver))
					{
						idx.Add(provPrefix + builtId + "," + kv.Value + "@|");
						AddToProduct2Items(p2i, provider, builtId);
					}
				}
			}
		}

		frmItemList.l_itemsindex = idx.ToArray();
		frmItemList.l_product2items = p2i.ToArray();
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
		this.advancedWizardPage3 = new AdvancedWizardControl.WizardPages.AdvancedWizardPage();
		this.txtDetection = new System.Windows.Forms.TextBox();
		this.lblDetection = new System.Windows.Forms.Label();
		this.txtArguments = new System.Windows.Forms.TextBox();
		this.lblArguments = new System.Windows.Forms.Label();
		this.advancedWizardPage2 = new AdvancedWizardControl.WizardPages.AdvancedWizardPage();
		this.cmbDate = new System.Windows.Forms.DateTimePicker();
		this.lblLanguage = new System.Windows.Forms.Label();
		this.chkRebootReq = new System.Windows.Forms.CheckBox();
		this.chkEULARequired = new System.Windows.Forms.CheckBox();
		this.lblHelp0 = new System.Windows.Forms.Label();
		this.cmbGroup = new System.Windows.Forms.ComboBox();
		this.lblGroup = new System.Windows.Forms.Label();
		this.lblExtras = new System.Windows.Forms.Label();
		this.chkExclusive = new System.Windows.Forms.CheckBox();
		this.chkCritical = new System.Windows.Forms.CheckBox();
		this.lblHelp1 = new System.Windows.Forms.Label();
		this.advancedWizard1.SuspendLayout();
		this.advancedWizardPage3.SuspendLayout();
		this.advancedWizardPage2.SuspendLayout();
		base.SuspendLayout();
		this.advancedWizard1.BackButtonEnabled = false;
		this.advancedWizard1.BackButtonText = "< Back";
		this.advancedWizard1.ButtonLayout = AdvancedWizardControl.Enums.ButtonLayoutKind.Office97;
		this.advancedWizard1.ButtonsVisible = true;
		this.advancedWizard1.CancelButtonText = "&Cancel";
		this.advancedWizard1.Controls.Add(this.advancedWizardPage2);
		this.advancedWizard1.Controls.Add(this.advancedWizardPage3);
		this.advancedWizard1.CurrentPageIsFinishPage = false;
		this.advancedWizard1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedWizard1.FinishButton = true;
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
		this.advancedWizard1.WizardPages.Add(this.advancedWizardPage2);
		this.advancedWizard1.WizardPages.Add(this.advancedWizardPage3);
		this.advancedWizard1.Cancel += new System.EventHandler(advancedWizard1_Cancel);
		this.advancedWizard1.Finish += new System.EventHandler(advancedWizard1_Finish);
		this.lblPrereqs = new System.Windows.Forms.Label();
		this.lstPrereqs = new System.Windows.Forms.ListBox();
		this.btnAddPrereq = new System.Windows.Forms.Button();
		this.btnRemovePrereq = new System.Windows.Forms.Button();
		this.advancedWizardPage3.Controls.Add(this.txtDetection);
		this.advancedWizardPage3.Controls.Add(this.lblDetection);
		this.advancedWizardPage3.Controls.Add(this.txtArguments);
		this.advancedWizardPage3.Controls.Add(this.lblArguments);
		this.advancedWizardPage3.Controls.Add(this.lblPrereqs);
		this.advancedWizardPage3.Controls.Add(this.lstPrereqs);
		this.advancedWizardPage3.Controls.Add(this.btnAddPrereq);
		this.advancedWizardPage3.Controls.Add(this.btnRemovePrereq);
		this.lblPrereqs.AutoSize = true;
		this.lblPrereqs.Location = new System.Drawing.Point(23, 219);
		this.lblPrereqs.Name = "lblPrereqs";
		this.lblPrereqs.Text = "Prerequisites:";
		this.lstPrereqs.FormattingEnabled = true;
		this.lstPrereqs.HorizontalScrollbar = true;
		this.lstPrereqs.Location = new System.Drawing.Point(142, 216);
		this.lstPrereqs.Name = "lstPrereqs";
		this.lstPrereqs.Size = new System.Drawing.Size(200, 56);
		this.lstPrereqs.TabIndex = 12;
		this.btnAddPrereq.Location = new System.Drawing.Point(348, 216);
		this.btnAddPrereq.Name = "btnAddPrereq";
		this.btnAddPrereq.Size = new System.Drawing.Size(80, 23);
		this.btnAddPrereq.TabIndex = 13;
		this.btnAddPrereq.Text = "Add...";
		this.btnAddPrereq.UseVisualStyleBackColor = true;
		this.btnAddPrereq.Click += new System.EventHandler(btnAddPrereq_Click);
		this.btnRemovePrereq.Location = new System.Drawing.Point(348, 245);
		this.btnRemovePrereq.Name = "btnRemovePrereq";
		this.btnRemovePrereq.Size = new System.Drawing.Size(80, 23);
		this.btnRemovePrereq.TabIndex = 14;
		this.btnRemovePrereq.Text = "Remove";
		this.btnRemovePrereq.UseVisualStyleBackColor = true;
		this.btnRemovePrereq.Click += new System.EventHandler(btnRemovePrereq_Click);
		this.advancedWizardPage3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedWizardPage3.Header = true;
		this.advancedWizardPage3.HeaderBackgroundColor = System.Drawing.Color.White;
		this.advancedWizardPage3.HeaderFont = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		this.advancedWizardPage3.HeaderImage = WUv4Powertools.Properties.Resources.EditUpdate;
		this.advancedWizardPage3.HeaderImageVisible = true;
		this.advancedWizardPage3.HeaderTitle = "Edit an Update";
		this.advancedWizardPage3.Location = new System.Drawing.Point(0, 0);
		this.advancedWizardPage3.Name = "advancedWizardPage3";
		this.advancedWizardPage3.PreviousPage = 0;
		this.advancedWizardPage3.Size = new System.Drawing.Size(440, 281);
		this.advancedWizardPage3.SubTitle = "Set download and installation options";
		this.advancedWizardPage3.SubTitleFont = new System.Drawing.Font("Tahoma", 8f);
		this.advancedWizardPage3.TabIndex = 3;
		this.txtDetection.Location = new System.Drawing.Point(142, 111);
		// Wraps so the text fills the box, with no scroll bars. A horizontal bar cannot show while
		// wrapping is on, since wrapped text never extends past the right edge. No length cap.
		this.txtDetection.Multiline = true;
		this.txtDetection.MaxLength = 0;
		this.txtDetection.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.txtDetection.WordWrap = true;
		this.txtDetection.Name = "txtDetection";
		this.txtDetection.Size = new System.Drawing.Size(286, 100);
		this.txtDetection.TabIndex = 11;
		this.lblDetection.AutoSize = true;
		this.lblDetection.Location = new System.Drawing.Point(23, 114);
		this.lblDetection.Name = "lblDetection";
		this.lblDetection.Size = new System.Drawing.Size(56, 13);
		this.lblDetection.TabIndex = 10;
		this.lblDetection.Text = "Detection:";
		this.txtArguments.Location = new System.Drawing.Point(142, 85);
		this.txtArguments.Name = "txtArguments";
		this.txtArguments.Size = new System.Drawing.Size(286, 20);
		this.txtArguments.TabIndex = 9;
		this.txtArguments.Text = "/q:a /r:n";
		this.lblArguments.AutoSize = true;
		this.lblArguments.Location = new System.Drawing.Point(23, 88);
		this.lblArguments.Name = "lblArguments";
		this.lblArguments.Size = new System.Drawing.Size(60, 13);
		this.lblArguments.TabIndex = 8;
		this.lblArguments.Text = "Arguments:";
		this.advancedWizardPage2.Controls.Add(this.lblHelp1);
		this.advancedWizardPage2.Controls.Add(this.cmbDate);
		this.advancedWizardPage2.Controls.Add(this.lblLanguage);
		this.advancedWizardPage2.Controls.Add(this.chkRebootReq);
		this.advancedWizardPage2.Controls.Add(this.chkEULARequired);
		this.advancedWizardPage2.Controls.Add(this.lblHelp0);
		this.advancedWizardPage2.Controls.Add(this.cmbGroup);
		this.advancedWizardPage2.Controls.Add(this.lblGroup);
		this.advancedWizardPage2.Controls.Add(this.lblExtras);
		this.advancedWizardPage2.Controls.Add(this.chkExclusive);
		this.advancedWizardPage2.Controls.Add(this.chkCritical);
		this.advancedWizardPage2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedWizardPage2.Header = true;
		this.advancedWizardPage2.HeaderBackgroundColor = System.Drawing.Color.White;
		this.advancedWizardPage2.HeaderFont = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		this.advancedWizardPage2.HeaderImage = WUv4Powertools.Properties.Resources.EditUpdate;
		this.advancedWizardPage2.HeaderImageVisible = true;
		this.advancedWizardPage2.HeaderTitle = "Edit an Update";
		this.advancedWizardPage2.Location = new System.Drawing.Point(0, 0);
		this.advancedWizardPage2.Name = "advancedWizardPage2";
		this.advancedWizardPage2.PreviousPage = 0;
		this.advancedWizardPage2.Size = new System.Drawing.Size(440, 281);
		this.advancedWizardPage2.SubTitle = "Configure the date, group and exclusiveness of update";
		this.advancedWizardPage2.SubTitleFont = new System.Drawing.Font("Tahoma", 8f);
		this.advancedWizardPage2.TabIndex = 2;
		this.cmbDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
		this.cmbDate.Location = new System.Drawing.Point(338, 163);
		this.cmbDate.Name = "cmbDate";
		this.cmbDate.Size = new System.Drawing.Size(90, 20);
		this.cmbDate.TabIndex = 17;
		this.lblLanguage.AutoSize = true;
		this.lblLanguage.Location = new System.Drawing.Point(23, 170);
		this.lblLanguage.Name = "lblLanguage";
		this.lblLanguage.Size = new System.Drawing.Size(33, 13);
		this.lblLanguage.TabIndex = 16;
		this.lblLanguage.Text = "Date:";
		this.chkRebootReq.AutoSize = true;
		this.chkRebootReq.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.chkRebootReq.Location = new System.Drawing.Point(207, 113);
		this.chkRebootReq.Name = "chkRebootReq";
		this.chkRebootReq.Size = new System.Drawing.Size(102, 17);
		this.chkRebootReq.TabIndex = 8;
		this.chkRebootReq.Text = "Reboot Needed";
		this.chkRebootReq.UseVisualStyleBackColor = true;
		this.chkEULARequired.AutoSize = true;
		this.chkEULARequired.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.chkEULARequired.Enabled = false;
		this.chkEULARequired.Location = new System.Drawing.Point(210, 90);
		this.chkEULARequired.Name = "chkEULARequired";
		this.chkEULARequired.Size = new System.Drawing.Size(99, 17);
		this.chkEULARequired.TabIndex = 7;
		this.chkEULARequired.Text = "Requires EULA";
		this.chkEULARequired.UseVisualStyleBackColor = true;
		this.lblHelp0.AutoSize = true;
		this.lblHelp0.Location = new System.Drawing.Point(23, 229);
		this.lblHelp0.Name = "lblHelp0";
		this.lblHelp0.Size = new System.Drawing.Size(202, 52);
		this.lblHelp0.TabIndex = 6;
		this.lblHelp0.Text = "90602 = Critical Updates\r\n90609 = Recomended Updates\r\n90943 = Internet and Multimedia Updates\r\n90945 = Multi-Language Features";
		this.cmbGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbGroup.FormattingEnabled = true;
		this.cmbGroup.Items.AddRange(new object[7] { "90602", "90609", "90943", "90944", "90945", "90949", "90952" });
		this.cmbGroup.Location = new System.Drawing.Point(307, 198);
		this.cmbGroup.Name = "cmbGroup";
		this.cmbGroup.Size = new System.Drawing.Size(121, 21);
		this.cmbGroup.TabIndex = 5;
		this.lblGroup.AutoSize = true;
		this.lblGroup.Location = new System.Drawing.Point(23, 201);
		this.lblGroup.Name = "lblGroup";
		this.lblGroup.Size = new System.Drawing.Size(39, 13);
		this.lblGroup.TabIndex = 4;
		this.lblGroup.Text = "Group:";
		this.lblExtras.AutoSize = true;
		this.lblExtras.Location = new System.Drawing.Point(23, 94);
		this.lblExtras.Name = "lblExtras";
		this.lblExtras.Size = new System.Drawing.Size(73, 13);
		this.lblExtras.TabIndex = 3;
		this.lblExtras.Text = "Extra Options:";
		this.chkExclusive.AutoSize = true;
		this.chkExclusive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.chkExclusive.Location = new System.Drawing.Point(319, 113);
		this.chkExclusive.Name = "chkExclusive";
		this.chkExclusive.Size = new System.Drawing.Size(109, 17);
		this.chkExclusive.TabIndex = 2;
		this.chkExclusive.Text = "Exclusive Update";
		this.chkExclusive.UseVisualStyleBackColor = true;
		this.chkCritical.AutoSize = true;
		this.chkCritical.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.chkCritical.Location = new System.Drawing.Point(333, 90);
		this.chkCritical.Name = "chkCritical";
		this.chkCritical.Size = new System.Drawing.Size(95, 17);
		this.chkCritical.TabIndex = 1;
		this.chkCritical.Text = "Critical Update";
		this.chkCritical.UseVisualStyleBackColor = true;
		this.chkCritical.CheckedChanged += new System.EventHandler(chkCritical_CheckedChanged);
		this.lblHelp1.AutoSize = true;
		this.lblHelp1.Location = new System.Drawing.Point(230, 229);
		this.lblHelp1.Name = "lblHelp1";
		this.lblHelp1.Size = new System.Drawing.Size(198, 39);
		this.lblHelp1.TabIndex = 18;
		this.lblHelp1.Text = "90944 = Additional Windows Downloads\r\n90949 = Windows Tools\r\n90952 = Advanced Security Updates";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.Control;
		base.ClientSize = new System.Drawing.Size(470, 480);
		base.Controls.Add(this.advancedWizard1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmEditUpdate";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Edit an Update";
		base.Load += new System.EventHandler(frmAddUpdate_Load);
		this.advancedWizard1.ResumeLayout(false);
		this.advancedWizardPage3.ResumeLayout(false);
		this.advancedWizardPage3.PerformLayout();
		this.advancedWizardPage2.ResumeLayout(false);
		this.advancedWizardPage2.PerformLayout();
		base.ResumeLayout(false);
	}
}
