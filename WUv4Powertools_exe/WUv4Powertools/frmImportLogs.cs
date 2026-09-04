using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WUv4Powertools;

// One row in the import list. The same update usually turns up once per language, so they are
// shown together with the languages listed beside the code rather than repeated down the list.
internal sealed class CandidateGroup
{
	public string Provider;
	public string Code;
	public ImportKind Kind;
	public string Reason;
	public readonly List<ImportCandidate> Members = new List<ImportCandidate>();

	// English reads best for picking updates out of a list, so it wins when the logs have it.
	public string Display
	{
		get
		{
			ImportCandidate english = Members.FirstOrDefault(m =>
				string.Equals(m.Language, "en", StringComparison.OrdinalIgnoreCase) &&
				!string.IsNullOrEmpty(m.Title));
			if (english != null) return english.Title;
			ImportCandidate any = Members.FirstOrDefault(m => !string.IsNullOrEmpty(m.Title));
			return any != null ? any.Title : Code;
		}
	}

	public string Languages
	{
		get
		{
			return string.Join("/", Members
				.Select(m => m.Language)
				.Where(x => !string.IsNullOrEmpty(x))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToArray());
		}
	}

	public bool Selected
	{
		get { return Members.Any(m => m.Selected); }
		set { foreach (ImportCandidate m in Members) m.Selected = value; }
	}
}

// Imports updates recorded in a machine's iuhist.xml and Windows Update.log into a whole consumer
// dictionary folder. One history file usually covers several operating systems and Internet
// Explorer versions at once, so every provider in the folder is offered rather than only the
// open tab.
public class frmImportLogs : Form
{
	private readonly frmMain owner;

	private readonly string root;

	private readonly List<string> xmlFiles = new List<string>();

	private readonly List<string> logFiles = new List<string>();

	private LogImportResult result;

	private ConsumerDictionary dictionary;

	private SupersededList superseded;

	// Tracks which translated strings came from real logs, so a later repair leaves them alone.
	private readonly StringProvenance provenance;

	// Both notices are trimmed to fit, so the full text is kept on a tooltip.
	private readonly ToolTip tips = new ToolTip();

	private Label lblXml;
	private Label lblLog;
	private Button btnPickXml;
	private Button btnPickLog;

	private Button btnPickFolder;
	private Label lblLanguage;
	private ComboBox cmbLanguage;
	private Label lblBasis;
	private CheckBox chkOverride;
	private ListView lstCandidates;
	private CheckBox chkShowSkipped;
	private Label lblCounts;
	private Button btnImport;
	private Button btnCancel;
	private Button btnEditList;
	private Button btnSelectAll;
	private Button btnSelectNone;
	private Label lblLegend;
	private Label lblApplies;
	private Label lblRejected;

	public frmImportLogs(frmMain owner, string root)
	{
		this.owner = owner;
		this.root = root;
		superseded = SupersededList.Load();
		provenance = StringProvenance.Load(root);
		dictionary = ConsumerDictionary.Load(root, owner == null ? null : owner.OpenProviders());
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		this.lblXml = new Label();
		this.lblLog = new Label();
		this.btnPickXml = new Button();
		this.btnPickLog = new Button();
		this.btnPickFolder = new Button();
		this.lblLanguage = new Label();
		this.cmbLanguage = new ComboBox();
		this.lblBasis = new Label();
		this.chkOverride = new CheckBox();
		this.lstCandidates = new ListView();
		this.chkShowSkipped = new CheckBox();
		this.lblCounts = new Label();
		this.btnImport = new Button();
		this.btnCancel = new Button();
		this.btnEditList = new Button();
		this.btnSelectAll = new Button();
		this.btnSelectNone = new Button();
		this.lblLegend = new Label();
		this.lblApplies = new Label();
		this.lblRejected = new Label();
		this.SuspendLayout();

		this.btnPickXml.Location = new Point(12, 12);
		this.btnPickXml.Size = new Size(120, 25);
		this.btnPickXml.Text = "Choose iuhist.xml...";
		this.btnPickXml.UseVisualStyleBackColor = true;
		this.btnPickXml.Click += new EventHandler(this.btnPickXml_Click);

		this.lblXml.AutoSize = true;
		this.lblXml.Location = new Point(140, 18);
		this.lblXml.Text = "no files chosen";
		this.lblXml.ForeColor = SystemColors.GrayText;

		this.btnPickLog.Location = new Point(12, 43);
		this.btnPickLog.Size = new Size(120, 25);
		this.btnPickLog.Text = "Choose the log...";
		this.btnPickLog.UseVisualStyleBackColor = true;
		this.btnPickLog.Click += new EventHandler(this.btnPickLog_Click);

		// Everything in one folder at once, with each file read on its own terms. The language
		// box is left out of it: every file states which system, which browser version and
		// which language it came from, and that beats one choice made for the whole run.
		this.btnPickFolder.Location = new Point(266, 43);
		this.btnPickFolder.Size = new Size(190, 25);
		this.btnPickFolder.Text = "Import a whole folder...";
		this.btnPickFolder.UseVisualStyleBackColor = true;
		this.btnPickFolder.Click += new EventHandler(this.btnPickFolder_Click);

		this.lblLog.AutoSize = true;
		this.lblLog.Location = new Point(140, 49);
		this.lblLog.Text = "optional, supplies the download addresses";
		this.lblLog.ForeColor = SystemColors.GrayText;

		// The warning can run to a couple of hundred characters, so it wraps to two lines and trims
		// rather than running off the dialog.
		this.lblRejected.AutoSize = false;
		this.lblRejected.AutoEllipsis = true;
		this.lblRejected.Location = new Point(12, 74);
		this.lblRejected.Size = new Size(700, 30);
		this.lblRejected.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		this.lblRejected.ForeColor = Color.Firebrick;
		this.lblRejected.Text = string.Empty;

		this.lblLanguage.AutoSize = true;
		this.lblLanguage.Location = new Point(12, 112);
		this.lblLanguage.Text = "Language:";

		this.cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
		this.cmbLanguage.Location = new Point(78, 108);
		this.cmbLanguage.Size = new Size(90, 21);
		this.cmbLanguage.SelectedIndexChanged += new EventHandler(this.cmbLanguage_SelectedIndexChanged);

		this.chkOverride.AutoSize = true;
		this.chkOverride.Location = new Point(178, 110);
		this.chkOverride.Text = "That is wrong, put everything in this language";
		this.chkOverride.UseVisualStyleBackColor = true;
		this.chkOverride.CheckedChanged += new EventHandler(this.chkOverride_CheckedChanged);

		this.lblBasis.AutoSize = false;
		this.lblBasis.AutoEllipsis = true;
		this.lblBasis.Size = new Size(700, 15);
		this.lblBasis.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		this.lblBasis.Location = new Point(12, 137);
		this.lblBasis.ForeColor = SystemColors.GrayText;
		this.lblBasis.Text = string.Empty;

		this.lstCandidates.CheckBoxes = true;
		this.lstCandidates.FullRowSelect = true;
		this.lstCandidates.GridLines = true;
		this.lstCandidates.HideSelection = false;
		this.lstCandidates.Location = new Point(12, 157);
		this.lstCandidates.Size = new Size(700, 289);
		this.lstCandidates.View = View.Details;
		this.lstCandidates.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		this.lstCandidates.Columns.Add("Update", 215);
		this.lstCandidates.Columns.Add("Code", 120);
		this.lstCandidates.Columns.Add("Goes to", 70);
		this.lstCandidates.Columns.Add("Languages", 90);
		this.lstCandidates.Columns.Add("What it does", 190);
		this.lstCandidates.ItemChecked += new ItemCheckedEventHandler(this.lstCandidates_ItemChecked);
		this.lstCandidates.ColumnClick += new ColumnClickEventHandler(this.lstCandidates_ColumnClick);

		this.chkShowSkipped.AutoSize = true;
		this.chkShowSkipped.Location = new Point(12, 450);
		this.chkShowSkipped.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		this.chkShowSkipped.Text = "Show entries that will not be imported";
		this.chkShowSkipped.UseVisualStyleBackColor = true;
		this.chkShowSkipped.CheckedChanged += new EventHandler(this.chkShowSkipped_CheckedChanged);

		this.lblCounts.AutoSize = false;
		this.lblCounts.AutoEllipsis = true;
		this.lblCounts.Size = new Size(700, 15);
		this.lblCounts.Location = new Point(12, 505);
		this.lblCounts.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		this.lblCounts.Text = string.Empty;

		// Importing is not a draft. Saying so on the window itself matters more than saying it in
		// a prompt nobody reads twice.
		this.lblApplies.AutoSize = false;
		this.lblApplies.AutoEllipsis = true;
		this.lblApplies.Size = new Size(700, 15);
		this.lblApplies.Location = new Point(12, 471);
		this.lblApplies.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		this.lblApplies.ForeColor = Color.FromArgb(150, 60, 0);
		this.lblApplies.Text = "Importing applies to the inventory files straight away. It is not an edit you save afterwards. Replaced files are kept as .bak.";

		// What bold means, so nobody has to guess why some rows stand out.
		this.lblLegend.AutoSize = false;
		this.lblLegend.AutoEllipsis = true;
		this.lblLegend.Size = new Size(700, 15);
		this.lblLegend.Location = new Point(12, 487);
		this.lblLegend.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		this.lblLegend.ForeColor = SystemColors.GrayText;
		this.lblLegend.Text = "Bold means the update arrives with no detection rule, so it will not be offered to any machine until you add one. Grey rows are not imported.";

		this.btnSelectAll.Location = new Point(170, 527);
		this.btnSelectAll.Size = new Size(85, 25);
		this.btnSelectAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		this.btnSelectAll.Text = "Select all";
		this.btnSelectAll.UseVisualStyleBackColor = true;
		this.btnSelectAll.Click += new EventHandler(this.btnSelectAll_Click);

		this.btnSelectNone.Location = new Point(261, 527);
		this.btnSelectNone.Size = new Size(85, 25);
		this.btnSelectNone.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		this.btnSelectNone.Text = "Clear all";
		this.btnSelectNone.UseVisualStyleBackColor = true;
		this.btnSelectNone.Click += new EventHandler(this.btnSelectNone_Click);

		this.btnEditList.Location = new Point(12, 527);
		this.btnEditList.Size = new Size(150, 25);
		this.btnEditList.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		this.btnEditList.Text = "Superseded list...";
		this.btnEditList.UseVisualStyleBackColor = true;
		this.btnEditList.Click += new EventHandler(this.btnEditList_Click);

		this.btnImport.Location = new Point(556, 527);
		this.btnImport.Size = new Size(75, 25);
		this.btnImport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		this.btnImport.Text = "Import";
		this.btnImport.Enabled = false;
		this.btnImport.UseVisualStyleBackColor = true;
		this.btnImport.Click += new EventHandler(this.btnImport_Click);

		this.btnCancel.Location = new Point(637, 527);
		this.btnCancel.Size = new Size(75, 25);
		this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.DialogResult = DialogResult.Cancel;
		this.btnCancel.UseVisualStyleBackColor = true;

		this.ClientSize = new Size(724, 564);
		this.Controls.Add(this.btnPickXml);
		this.Controls.Add(this.lblXml);
		this.Controls.Add(this.btnPickLog);
		this.Controls.Add(this.btnPickFolder);
		this.Controls.Add(this.lblLog);
		this.Controls.Add(this.lblRejected);
		this.Controls.Add(this.lblLanguage);
		this.Controls.Add(this.cmbLanguage);
		this.Controls.Add(this.chkOverride);
		this.Controls.Add(this.lblBasis);
		this.Controls.Add(this.lstCandidates);
		this.Controls.Add(this.chkShowSkipped);
		this.Controls.Add(this.lblCounts);
		this.Controls.Add(this.lblApplies);
		this.Controls.Add(this.lblLegend);
		this.Controls.Add(this.btnSelectAll);
		this.Controls.Add(this.btnSelectNone);
		this.Controls.Add(this.btnEditList);
		this.Controls.Add(this.btnImport);
		this.Controls.Add(this.btnCancel);
		this.CancelButton = this.btnCancel;
		this.FormBorderStyle = FormBorderStyle.Sizable;
		this.MinimumSize = new Size(640, 480);
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.ShowIcon = false;
		this.ShowInTaskbar = false;
		this.StartPosition = FormStartPosition.CenterParent;
		this.Text = "Import updates from logs";
		this.Load += new EventHandler(this.frmImportLogs_Load);
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	private void frmImportLogs_Load(object sender, EventArgs e)
	{
		this.Text = "Import updates from logs into " + Path.GetFileName(root.TrimEnd('\\'));

		if (dictionary.Providers.Count == 0)
		{
			lblBasis.Text = "No providers could be read from this folder.";
			return;
		}

		lblBasis.Text = string.Format("{0} providers ready: {1}",
			dictionary.Providers.Count,
			string.Join(", ", dictionary.Providers.Select(p => p.Name).ToArray()));
		tips.SetToolTip(lblBasis, lblBasis.Text);
		tips.SetToolTip(lblApplies, lblApplies.Text);
		tips.SetToolTip(lblLegend, lblLegend.Text);

		if (dictionary.Unavailable.Count > 0)
		{
			lblRejected.Text = "Not readable in this folder, so their updates cannot be imported: " +
				string.Join(", ", dictionary.Unavailable.ToArray());
			tips.SetToolTip(lblRejected, lblRejected.Text);
		}
	}

	// Takes every history file and every log in one folder, and its subfolders, in a single go.
	// Each file says which system, which browser version and which language it belongs to, so
	// nothing has to be chosen by hand and the language box is ignored for the whole run.
	private void btnPickFolder_Click(object sender, EventArgs e)
	{
		using (FolderBrowserDialog dialog = new FolderBrowserDialog())
		{
			dialog.Description = "Choose a folder holding history files and logs. Everything in it, and in the folders inside it, is read at once.";
			if (dialog.ShowDialog(this) != DialogResult.OK) return;

			string[] found;
			try
			{
				found = Directory.GetFiles(dialog.SelectedPath, "*.*", SearchOption.AllDirectories);
			}
			catch (Exception ex)
			{
				MessageBox.Show("That folder could not be read.\n\n" + ex.Message,
					"Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}

			xmlFiles.Clear();
			logFiles.Clear();
			foreach (string file in found)
			{
				string extension = Path.GetExtension(file);
				if (string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase)) xmlFiles.Add(file);
				else if (string.Equals(extension, ".log", StringComparison.OrdinalIgnoreCase)) logFiles.Add(file);
			}

			if (xmlFiles.Count == 0 && logFiles.Count == 0)
			{
				MessageBox.Show("That folder holds no history files and no logs.",
					"Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			// One language chosen for the whole run is exactly what goes wrong here, so it is
			// turned off and each entry keeps the language its own file states.
			chkOverride.Checked = false;
		}
		Reparse();
	}

	private void btnPickXml_Click(object sender, EventArgs e)
	{
		using (OpenFileDialog dialog = new OpenFileDialog())
		{
			dialog.Title = "Choose one or more iuhist.xml files";
			dialog.Filter = "Update history (*.xml)|*.xml|All files (*.*)|*.*";
			dialog.Multiselect = true;
			if (dialog.ShowDialog(this) != DialogResult.OK) return;
			xmlFiles.Clear();
			xmlFiles.AddRange(dialog.FileNames);
		}
		Reparse();
	}

	private void btnPickLog_Click(object sender, EventArgs e)
	{
		using (OpenFileDialog dialog = new OpenFileDialog())
		{
			dialog.Title = "Choose one or more Windows Update.log files";
			dialog.Filter = "Update log (*.log)|*.log|All files (*.*)|*.*";
			dialog.Multiselect = true;
			if (dialog.ShowDialog(this) != DialogResult.OK) return;
			logFiles.Clear();
			logFiles.AddRange(dialog.FileNames);
		}
		Reparse();
	}

	private void Reparse()
	{
		lblXml.Text = xmlFiles.Count == 0 ? "no files chosen" : Describe(xmlFiles);
		lblLog.Text = logFiles.Count == 0 ? "optional, supplies the download addresses" : Describe(logFiles);

		if (xmlFiles.Count == 0 && logFiles.Count == 0) return;

		Cursor = Cursors.WaitCursor;
		try
		{
			result = LogImportParser.Parse(xmlFiles, logFiles);
		}
		finally
		{
			Cursor = Cursors.Default;
		}

		List<string> notices = new List<string>();

		// A file dated at or after the cutoff is not from the era these inventories cover, so it
		// is refused outright rather than partly read. This is said first, and the label it goes
		// in is already red, so it is the first thing seen when nothing appears to import.
		if (result.LateDatedFiles.Count > 0)
		{
			notices.Add(string.Format(
				"Cannot import {0} file{1}, because {2} from {3} or newer: {4}",
				result.LateDatedFiles.Count,
				result.LateDatedFiles.Count == 1 ? string.Empty : "s",
				result.LateDatedFiles.Count == 1 ? "it is" : "they are",
				LogImportParser.FirstRejectedYear,
				string.Join(", ", result.LateDatedFiles.ToArray())));
		}
		if (result.RejectedFiles.Count > 0)
		{
			notices.Add(string.Format("Skipped {0} file{1} that mention Windows Update Restored: {2}",
				result.RejectedFiles.Count, result.RejectedFiles.Count == 1 ? string.Empty : "s",
				string.Join(", ", result.RejectedFiles.ToArray())));
		}
		if (dictionary.Unavailable.Count > 0)
		{
			notices.Add("Not readable in this folder: " + string.Join(", ", dictionary.Unavailable.ToArray()));
		}
		notices.AddRange(result.Warnings);

		lblRejected.Text = notices.Count == 0 ? string.Empty : string.Join("  ", notices.ToArray());
		tips.SetToolTip(lblRejected, lblRejected.Text);

		// With no history file the log still names files it downloaded, which is enough to fill in
		// a missing language and put a wrong file name right for updates already in the folder.
		if (xmlFiles.Count == 0 && logFiles.Count > 0)
		{
			int fromLog = LogOnlyImport.AddCandidates(result, dictionary);
			if (fromLog > 0)
			{
				result.Warnings.Add(fromLog + " entries were worked out from the log alone, so they carry no title and no published date. Only a catalogue file has those.");
			}
		}

		populatingLanguages = true;
		cmbLanguage.Items.Clear();
		foreach (string language in result.LanguagesSeen.OrderBy(x => x))
		{
			cmbLanguage.Items.Add(language);
		}
		foreach (string language in new[] { "en", "de", "fr", "es", "it", "nl", "sv", "da", "fi", "no", "pl", "cs", "hu", "tr", "el", "ru", "ja", "ko", "zhcn", "zhtw", "ptbr", "pt", "ar", "he", "sk", "sl" })
		{
			if (!cmbLanguage.Items.Contains(language)) cmbLanguage.Items.Add(language);
		}

		if (!string.IsNullOrEmpty(result.DetectedLanguage) && cmbLanguage.Items.Contains(result.DetectedLanguage))
		{
			cmbLanguage.SelectedItem = result.DetectedLanguage;
		}
		else if (cmbLanguage.Items.Count > 0)
		{
			cmbLanguage.SelectedIndex = 0;
		}
		populatingLanguages = false;

		lblBasis.Text = "Language " + result.DetectionBasis + ". Each entry is imported into the language it states.";
		tips.SetToolTip(lblBasis, lblBasis.Text);
		Rebuild();
	}

	private static string Describe(List<string> files)
	{
		if (files.Count == 1) return Path.GetFileName(files[0]);
		return files.Count + " files from " + Path.GetFileName(Path.GetDirectoryName(files[0]));
	}

	private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
	{
		Rebuild();
	}

	private void chkOverride_CheckedChanged(object sender, EventArgs e)
	{
		Rebuild();
	}

	private void chkShowSkipped_CheckedChanged(object sender, EventArgs e)
	{
		Rebuild();
	}

	private bool suppressCheckEvents;

	private bool populatingLanguages;

	private readonly List<CandidateGroup> groups = new List<CandidateGroup>();

	private int sortColumn = -1;

	private bool sortAscending = true;

	private void Rebuild()
	{
		if (result == null || dictionary == null || populatingLanguages) return;

		string language = cmbLanguage.SelectedItem as string;
		LogImportEngine.Classify(result.Candidates, dictionary, superseded, language, chkOverride.Checked);

		// The same update arrives once per language, so they are collapsed into one row each.
		groups.Clear();
		Dictionary<string, CandidateGroup> byKey =
			new Dictionary<string, CandidateGroup>(StringComparer.OrdinalIgnoreCase);
		foreach (ImportCandidate c in result.Candidates)
		{
			string key = c.Provider + "|" + c.Code + "|" + (int)c.Kind;
			CandidateGroup group;
			if (!byKey.TryGetValue(key, out group))
			{
				group = new CandidateGroup { Provider = c.Provider, Code = c.Code, Kind = c.Kind };
				byKey[key] = group;
				groups.Add(group);
			}
			group.Members.Add(c);
			group.Reason = c.Reason;
		}

		Render();
	}

	// Clicking a heading sorts by it, and clicking the same one again turns the order around.
	private void lstCandidates_ColumnClick(object sender, ColumnClickEventArgs e)
	{
		if (e.Column == sortColumn) sortAscending = !sortAscending;
		else { sortColumn = e.Column; sortAscending = true; }
		Render();
	}

	// Draws the grouped rows in whatever order the headings ask for.
	private void Render()
	{
		IEnumerable<CandidateGroup> shown = groups.Where(g =>
			Importable(g.Kind) || chkShowSkipped.Checked);

		Func<CandidateGroup, string> key;
		switch (sortColumn)
		{
			case 1: key = g => g.Code; break;
			case 2: key = g => g.Provider; break;
			case 3: key = g => g.Languages; break;
			case 4: key = g => g.Reason ?? string.Empty; break;
			case 0: key = g => g.Display; break;
			default: key = null; break;
		}

		List<CandidateGroup> ordered;
		if (key == null)
		{
			// Until a heading is clicked, the rows worth acting on come first.
			ordered = shown
				.OrderBy(g => Rank(g.Kind))
				.ThenBy(g => g.Provider, StringComparer.OrdinalIgnoreCase)
				.ThenBy(g => g.Display, StringComparer.OrdinalIgnoreCase)
				.ToList();
		}
		else
		{
			ordered = sortAscending
				? shown.OrderBy(key, StringComparer.OrdinalIgnoreCase).ToList()
				: shown.OrderByDescending(key, StringComparer.OrdinalIgnoreCase).ToList();
		}

		suppressCheckEvents = true;
		lstCandidates.BeginUpdate();
		lstCandidates.Items.Clear();
		foreach (CandidateGroup g in ordered)
		{
			ListViewItem row = new ListViewItem(g.Display);
			row.SubItems.Add(g.Code);
			row.SubItems.Add(g.Provider);
			row.SubItems.Add(g.Languages);
			row.SubItems.Add(g.Reason);
			row.Tag = g;
			row.Checked = g.Selected;
			if (!Importable(g.Kind)) row.ForeColor = SystemColors.GrayText;
			else if (g.Kind == ImportKind.NewUpdate)
			{
				// Nothing in a log carries a detection rule, so an update the folder has never held
				// arrives without one. Bold here means the same as bold in the update list.
				row.ForeColor = Color.FromArgb(150, 60, 0);
				row.Font = new Font(lstCandidates.Font, FontStyle.Bold);
			}
			lstCandidates.Items.Add(row);
		}
		lstCandidates.EndUpdate();
		suppressCheckEvents = false;
		UpdateCounts();
	}

	private static bool Importable(ImportKind kind)
	{
		return kind == ImportKind.LanguageGap || kind == ImportKind.NewUpdate ||
			kind == ImportKind.Correction;
	}

	private static int Rank(ImportKind kind)
	{
		if (kind == ImportKind.LanguageGap) return 0;
		if (kind == ImportKind.NewUpdate) return 1;
		if (kind == ImportKind.Correction) return 2;
		return 3;
	}

	private void lstCandidates_ItemChecked(object sender, ItemCheckedEventArgs e)
	{
		if (suppressCheckEvents) return;
		CandidateGroup g = e.Item.Tag as CandidateGroup;
		if (g == null) return;

		if (e.Item.Checked && !Importable(g.Kind))
		{
			// Superseded, already present and other provider rows are shown for reference only.
			e.Item.Checked = false;
			return;
		}

		g.Selected = e.Item.Checked;
		UpdateCounts();
	}

	private void UpdateCounts()
	{
		if (result == null) return;

		// Counted as rows on screen, since each row is one update however many languages it covers.
		int gaps = groups.Count(g => g.Kind == ImportKind.LanguageGap);
		int fresh = groups.Count(g => g.Kind == ImportKind.NewUpdate);
		int fixable = groups.Count(g => g.Kind == ImportKind.Correction);
		int have = groups.Count(g => g.Kind == ImportKind.AlreadyPresent);
		int blocked = groups.Count(g => g.Kind == ImportKind.Superseded);
		int other = groups.Count(g => g.Kind == ImportKind.OtherProvider);
		int blank = groups.Count(g => g.Kind == ImportKind.NoDescription);
		int ticked = groups.Count(g => g.Selected);
		int entries = result.Candidates.Count(x => x.Selected);

		string spread = string.Join(", ", result.Candidates
			.Where(x => x.Selected)
			.GroupBy(x => x.Provider, StringComparer.OrdinalIgnoreCase)
			.OrderByDescending(g => g.Count())
			.Select(g => g.Key + " " + g.Count())
			.ToArray());

		lblCounts.Text = string.Format(
			"{0} updates ticked, {1} language entries{2}.  {3} missing language, {4} correctable, {5} already right, {6} no description, {7} superseded, {8} not in this inventory.",
			ticked, entries, spread.Length == 0 ? string.Empty : " (" + spread + ")",
			gaps, fixable, have, blank, blocked, other);
		tips.SetToolTip(lblCounts, lblCounts.Text);
		btnImport.Enabled = ticked > 0;
	}

	// Ticks everything on screen that can actually be imported. An update arriving without a
	// detection rule is deliberately left alone, because it needs work before it is any use.
	private void btnSelectAll_Click(object sender, EventArgs e)
	{
		SetAll(true);
	}

	private void btnSelectNone_Click(object sender, EventArgs e)
	{
		SetAll(false);
	}

	private void SetAll(bool ticked)
	{
		suppressCheckEvents = true;
		lstCandidates.BeginUpdate();
		foreach (ListViewItem row in lstCandidates.Items)
		{
			CandidateGroup g = row.Tag as CandidateGroup;
			if (g == null || !Importable(g.Kind)) continue;
			if (ticked && g.Kind == ImportKind.NewUpdate) continue;
			g.Selected = ticked;
			row.Checked = ticked;
		}
		lstCandidates.EndUpdate();
		suppressCheckEvents = false;
		UpdateCounts();
	}

	private void btnEditList_Click(object sender, EventArgs e)
	{
		try
		{
			SupersededList list = SupersededList.Load();
			Process.Start("notepad.exe", "\"" + list.Path + "\"");
			MessageBox.Show(this,
				"The list opens in Notepad. Save it, then press OK to apply the changes.",
				"Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
			superseded = SupersededList.Load();
			Rebuild();
		}
		catch (Exception ex)
		{
			MessageBox.Show(this, "The superseded list could not be opened.\n\n" + ex.Message,
				"Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
	}

	private void btnImport_Click(object sender, EventArgs e)
	{
		string language = cmbLanguage.SelectedItem as string;
		if (string.IsNullOrEmpty(language))
		{
			MessageBox.Show(this, "Choose the language these updates belong to first.",
				"Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}

		int fresh = groups.Count(g => g.Selected && g.Kind == ImportKind.NewUpdate);
		if (fresh > 0 && MessageBox.Show(this, string.Format(
			"{0} of the ticked updates are not in their provider in any language.\n\n" +
			"The logs record no detection rules, so those come in with a placeholder that matches " +
			"nothing. They will not be offered to any machine until you give them a real rule, and " +
			"they are shown in bold in the update list so you can find them.\n\nContinue?", fresh),
			"Windows Update v4.0 PowerTools", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
		{
			return;
		}

		int fixing = groups.Count(g => g.Selected && g.Kind == ImportKind.Correction);
		if (fixing > 0 && MessageBox.Show(this, string.Format(
			"{0} of the ticked updates are already in the folder and will be rewritten with what the " +
			"logs say: the GUID, title, version and file name.\n\nWhat the update targets is never " +
			"changed.\n\nGo ahead?", fixing),
			"Windows Update v4.0 PowerTools", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}

		// Everything is written now, so the last prompt spells out exactly which files change and
		// warns when a tab has work that has not been saved, since that goes to disk with it.
		string[] affected = result.Candidates
			.Where(x => x.Selected)
			.Select(x => x.Provider)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.Where(p => dictionary.Find(p) != null)
			.OrderBy(x => x)
			.ToArray();
		if (affected.Length == 0) return;

		string[] unsaved = affected
			.Where(p =>
			{
				ProviderStore s = dictionary.Find(p);
				return s != null && s.OpenTab != null && s.OpenTab.HasUnsavedChanges;
			})
			.ToArray();

		string caution = unsaved.Length == 0
			? string.Empty
			: "\n\n" + string.Join(", ", unsaved) +
			" has changes you have not saved. Those are written out with the import.";

		if (MessageBox.Show(this, string.Format(
			"This writes to {0} inventor{1} now:\n\n{2}\n\n" +
			"The changes are applied to the files themselves. This is not an edit you save afterwards " +
			"and there is no undo for it, though the files being replaced are kept as .bak.{3}\n\nGo ahead?",
			affected.Length, affected.Length == 1 ? "y" : "ies",
			string.Join(", ", affected), caution),
			"Windows Update v4.0 PowerTools", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
		{
			return;
		}

		ImportSummary summary;
		Cursor = Cursors.WaitCursor;
		try
		{
			summary = LogImportEngine.Apply(result.Candidates, dictionary, language, chkOverride.Checked, provenance);
		}
		finally
		{
			Cursor = Cursors.Default;
		}

		MessageBox.Show(this, Report(summary), "Windows Update v4.0 PowerTools",
			MessageBoxButtons.OK,
			summary.WriteErrors.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Asterisk);

		DialogResult = DialogResult.OK;
		Close();
	}

	private static string Report(ImportSummary summary)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.AppendLine(string.Format("Added {0} update{1} and corrected {2} existing record{3}.",
			summary.ItemsAdded, summary.ItemsAdded == 1 ? string.Empty : "s",
			summary.Corrected, summary.Corrected == 1 ? string.Empty : "s"));

		if (summary.ByProvider.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("Records touched, by provider:");
			foreach (var pair in summary.ByProvider.OrderByDescending(p => p.Value))
			{
				sb.AppendLine("   " + pair.Key + ": " + pair.Value);
			}
		}

		sb.AppendLine();
		sb.AppendLine(string.Format("{0} index entries, {1} new translated strings and {2} product links were written.",
			summary.IndexEntriesAdded, summary.StringsAdded, summary.ProductLinksAdded));
		if (summary.GuidsFromLog > 0)
		{
			sb.AppendLine(summary.GuidsFromLog + " of the added rows used the GUID the update was really published under.");
		}

		if (summary.Written.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("Written to the inventory files: " +
				string.Join(", ", summary.Written.ToArray()));
		}

		if (summary.LeftOpen.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("Also refreshed on screen: " +
				string.Join(", ", summary.LeftOpen.ToArray()));
		}

		if (summary.NewUpdatesAdded > 0 || summary.Corrected > 0)
		{
			sb.AppendLine();
			if (summary.NewUpdatesAdded > 0)
			{
				sb.AppendLine(summary.NewUpdatesAdded + " were new to the folder and came in with a placeholder detection rule. They show in bold until you give them a real one.");
			}
			if (summary.Corrected > 0)
			{
				sb.AppendLine(string.Format(
					"Corrections applied: {0} GUIDs, {1} titles, {2} versions, {3} file names, {4} dates, " +
					"{5} code spellings, {6} restart flags, {7} installer types, {8} links.",
					summary.GuidsCorrected, summary.TitlesCorrected, summary.VersionsCorrected,
					summary.FileNamesCorrected, summary.DatesCorrected, summary.CodesRecased,
					summary.RebootFlagsCorrected, summary.CommandTypesCorrected,
					summary.LinksCorrected));
			}
		}

		// Putting an update onto one row changes what the list shows for it, so it is said
		// plainly rather than left to be noticed.
		if (summary.RowsMerged > 0 || summary.LanguagesSharingAFile > 0)
		{
			sb.AppendLine(string.Format(
				"{0} rows held a download another row of the same update already had, so those updates now use one address for every language. {1} languages were given a row that was already there.",
				summary.RowsMerged, summary.LanguagesSharingAFile));
		}

		if (summary.WithoutPostedDate > 0)
		{
			sb.AppendLine();
			sb.AppendLine(summary.WithoutPostedDate + " had no published date, because only an iuhist_catalog.xml records one. A plain iuhist.xml only says when the machine downloaded the update.");
		}

		if (summary.GuessedNames > 0)
		{
			sb.AppendLine();
			sb.AppendLine(summary.GuessedNames + " had no download in the logs, so the file name was worked " +
				"out from how this update is named in the languages you already have. Check these:");
			foreach (string guess in summary.Guesses.Take(10))
			{
				sb.AppendLine("   " + guess);
			}
			if (summary.Guesses.Count > 10) sb.AppendLine("   and " + (summary.Guesses.Count - 10) + " more");
		}

		if (summary.Skipped > 0)
		{
			sb.AppendLine();
			sb.AppendLine(summary.Skipped + " were skipped:");
			foreach (string note in summary.Notes.Take(10))
			{
				sb.AppendLine("   " + note);
			}
			if (summary.Notes.Count > 10) sb.AppendLine("   and " + (summary.Notes.Count - 10) + " more");
		}

		if (summary.WriteErrors.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("These could not be written:");
			foreach (string error in summary.WriteErrors)
			{
				sb.AppendLine("   " + error);
			}
		}

		return sb.ToString();
	}
}
