using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Office2007Renderer;
using WindowsFormsAero;

namespace WUv4Powertools;

public class frmItemList : Form
{
	private frmMain frmMain;

	public List<ListViewItem> lstItemCol = new List<ListViewItem>();

	public BackgroundWorker bw = new BackgroundWorker();

	public int p_items;

	// Loop iterations finished, as opposed to p_items which only counts rows actually added.
	// Progress has to be measured against work done or the bar stops short of the end whenever
	// an update yields no row for the selected language.
	public int p_scanned;

	// Current search text. Empty shows everything. Applied when the list is rebuilt, so matches
	// keep their categories and everything else is left out rather than merely tinted.
	private string searchFilter = string.Empty;

	// One captured state of the five dictionaries. The arrays are copied but the strings inside are
	// shared, so a snapshot costs a few hundred kilobytes even on the largest provider.
	private sealed class DictionarySnapshot
	{
		public string[] Items;
		public string[] ItemsIndex;
		public string[] ItemStrings;
		public string[] ItemStringsIndex;
		public string[] Product2Items;
	}

	private readonly List<DictionarySnapshot> undoStates = new List<DictionarySnapshot>();

	private readonly List<DictionarySnapshot> redoStates = new List<DictionarySnapshot>();

	private const int MaxUndoDepth = 20;

	public bool CanUndo
	{
		get { return undoStates.Count > 0; }
	}

	public bool CanRedo
	{
		get { return redoStates.Count > 0; }
	}

	// Call before anything that changes the dictionaries. Taking a new step forward discards the
	// redo history, since redoing a change made before a different one no longer means anything.
	public void PushUndoState()
	{
		undoStates.Add(CaptureState());
		if (undoStates.Count > MaxUndoDepth)
		{
			undoStates.RemoveAt(0);
		}
		redoStates.Clear();
	}

	public bool Undo()
	{
		if (undoStates.Count == 0) return false;
		redoStates.Add(CaptureState());
		ApplyState(undoStates[undoStates.Count - 1]);
		undoStates.RemoveAt(undoStates.Count - 1);
		ReloadItems();
		return true;
	}

	public bool Redo()
	{
		if (redoStates.Count == 0) return false;
		undoStates.Add(CaptureState());
		ApplyState(redoStates[redoStates.Count - 1]);
		redoStates.RemoveAt(redoStates.Count - 1);
		ReloadItems();
		return true;
	}

	private DictionarySnapshot CaptureState()
	{
		DictionarySnapshot snapshot = new DictionarySnapshot();
		snapshot.Items = CopyOf(l_items);
		snapshot.ItemsIndex = CopyOf(l_itemsindex);
		snapshot.ItemStrings = CopyOf(l_itemstrings);
		snapshot.ItemStringsIndex = CopyOf(l_itemstringsindex);
		snapshot.Product2Items = CopyOf(l_product2items);
		return snapshot;
	}

	private void ApplyState(DictionarySnapshot snapshot)
	{
		l_items = snapshot.Items;
		l_itemsindex = snapshot.ItemsIndex;
		l_itemstrings = snapshot.ItemStrings;
		l_itemstringsindex = snapshot.ItemStringsIndex;
		l_product2items = snapshot.Product2Items;
	}

	private static string[] CopyOf(string[] source)
	{
		if (source == null) return null;
		string[] copy = new string[source.Length];
		Array.Copy(source, copy, source.Length);
		return copy;
	}

	// Rows actually on screen, which is the filtered count once a search is active.
	public int VisibleItemCount
	{
		get { return lstItems.Items.Count; }
	}

	public string CurrentSearchFilter
	{
		get { return searchFilter; }
	}

	public void SetSearchFilter(string term)
	{
		searchFilter = term ?? string.Empty;
		OrganizeIntoGroups();
	}

	// Matches on every column, ignoring case, so a code can be found whatever case it is typed in.
	public static bool ItemMatches(ListViewItem item, string term)
	{
		if (string.IsNullOrEmpty(term)) return true;
		foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
		{
			if (sub.Text != null && sub.Text.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	public string provider;

	public bool isDriverProvider = false;

	public string[] l_items;

	public string[] l_itemsindex;

	public string[] l_itemstrings;

	public string[] l_itemstringsindex;

	public string[] l_product2items;

	public string[] l_productgroupstrings;

	public string[] l_products;

	public string[] u_items;

	private IContainer components;

	private ColumnHeader colUpdName;

	private ColumnHeader colUpdCode;

	private ColumnHeader colLangCount;

	private ColumnHeader colUpdCritical;

	private ColumnHeader colUpdGroup;

	private ColumnHeader colUpdExclusive;

	public WindowsFormsAero.ListView lstItems;

	private Timer tmrLoad;

	private ImageList imgLst;

	private Panel panelRight;

	public System.Windows.Forms.TextBox lblUpdDescription;

	public Label lblTimeStamp;

	public LinkLabel lblEula;

	public Label lblUpdTitle;

	private Label lblRight;

	// New fields for drag-drop and grouping
	private ListViewItem draggedItem = null;
	private bool orderChanged = false;

	public frmItemList()
	{
		InitializeComponent();
		ToolStripManager.Renderer = new global::Office2007Renderer.Office2007Renderer();
		Font = SystemFonts.MenuFont;
		panelRight.BackColor = Office2007ColorTable._toolStripBegin;
		lblUpdDescription.BackColor = Office2007ColorTable._toolStripBegin;
		lblRight.BackColor = Office2007ColorTable._toolStripEnd;

		// Enable drag-drop
		lstItems.AllowDrop = true;
		lstItems.ItemDrag += LstItems_ItemDrag;
		lstItems.DragEnter += LstItems_DragEnter;
		lstItems.DragOver += LstItems_DragOver;
		lstItems.DragDrop += LstItems_DragDrop;

		// Enable groups
		lstItems.ShowGroups = true;
	}

	public void loadItems()
	{
		try
		{
			// Ensure lstItemCol is initialized
			if (lstItemCol == null)
			{
				lstItemCol = new List<ListViewItem>();
			}
			else
			{
				lstItemCol.Clear();
			}
			
			// A HashSet for the membership test. This was a List.Contains scan, which is linear per
			// item and so quadratic overall. win2k holds 3383 items lines, meaning millions of
			// string comparisons every time the list was rebuilt.
			List<string> u_items1 = new List<string>();
			HashSet<string> u_seen = new HashSet<string>(StringComparer.Ordinal);
			string[] array = l_items;
			for (int i = 0; i < array.Length; i++)
			{
				string code = array[i].Split('@')[0].Split(',')[1];
				if (u_seen.Add(code))
				{
					u_items1.Add(code);
				}
			}
			u_items = u_items1.ToArray();
			
			// Use a lock for thread-safe list access
			object lockObj = new object();
			
			Parallel.ForEach(u_items, new ParallelOptions
			{
				MaxDegreeOfParallelism = 8
			}, delegate(string _line)
			{
				try
				{
					List<string> list = new List<string>();
					List<int> list2 = new List<int>();
					string[] array2 = new string[0];
					int num = 0;
					string text = "";
					for (int j = 0; j < l_items.Length; j++)
					{
						string text2 = l_items[j];
						if (text2.Contains(_line))
						{
							num++;
							list.Add(text2);
							list2.Add(j);
							array2 = text2.Split(new string[1] { "@|" }, StringSplitOptions.None);
						}
					}
					Update update = new Update
					{
						itemindexes = list2.ToArray(),
						itemlines = list.ToArray()
					};
					
					// Validate array2 has minimum required elements
					if (array2.Length < 11)
					{
						// Skip this update if data is malformed
						return;
					}
			
			List<LangTitleDesc> list3 = new List<LangTitleDesc>();
			string value = array2[2]; // This is the GUID for languages
			string[] array3 = l_itemstringsindex;
			
			// First, find the string GUID that corresponds to our language GUID
			// Filter to only get English entries
			string stringGuid = null;
			foreach (string indexLine in array3)
			{
				if (string.IsNullOrEmpty(indexLine))
				{
					continue;
				}
				
				// Check if this line matches our GUID and is English
				// Format: "provider.en.GUID,stringGUID" or "devices.en.GUID,stringGUID"
				if (indexLine.Contains(value))
				{
					// Check if it's an English entry (contains .en. or .en,)
					if (indexLine.Contains(".en.") || indexLine.Contains(".en,"))
					{
						string[] indexParts = indexLine.Split(',');
						if (indexParts.Length >= 2)
						{
							stringGuid = indexParts[1].Trim();
							break;
						}
					}
				}
			}
			
			// If no English entry found, try without language filter as fallback
			if (string.IsNullOrEmpty(stringGuid))
			{
				foreach (string indexLine in array3)
				{
					if (string.IsNullOrEmpty(indexLine))
					{
						continue;
					}
					
					if (indexLine.Contains(value))
					{
						string[] indexParts = indexLine.Split(',');
						if (indexParts.Length >= 2)
						{
							stringGuid = indexParts[1].Trim();
							break;
						}
					}
				}
			}
			
			// Now parse the itemstrings with the string GUID
			if (!string.IsNullOrEmpty(stringGuid))
			{
				string[] array4 = l_itemstrings;
				foreach (string text4 in array4)
				{
					if (string.IsNullOrEmpty(text4))
					{
						continue;
					}
					
					// Check if this line contains our string GUID
					if (text4.Contains(stringGuid))
					{
						string[] text4Parts = text4.Split(new string[] { "@|" }, StringSplitOptions.None);
						if (text4Parts.Length >= 2)
						{
							// First part before @| contains "devices.GUID,Title"
							string[] titleParts = text4Parts[0].Split(new char[] { ',' }, 2);
							if (titleParts.Length >= 2)
							{
								LangTitleDesc langTitleDesc = new LangTitleDesc();
								text = titleParts[1].Trim();
								langTitleDesc.lang = "en"; // Default to English
								langTitleDesc.title = titleParts[1].Trim();
								langTitleDesc.description = text4Parts.Length > 1 ? text4Parts[1].Trim() : "";
								langTitleDesc.eulaUrl = text4Parts.Length > 4 ? text4Parts[4].Trim() : "";
								list3.Add(langTitleDesc);
								break; // Found it, no need to continue
							}
						}
					}
				}
			}
			
			// If we still don't have any language data, create a default entry with the code
			if (list3.Count == 0)
			{
				LangTitleDesc defaultLang = new LangTitleDesc
				{
					lang = "en",
					title = _line,
					description = "Driver Update",
					eulaUrl = ""
				};
				list3.Add(defaultLang);
				text = _line;
			}
			update.lan = list3.ToArray();
			
			// Driver format has different field positions than Windows updates
			// Windows: [priority at 7, timestamp at 9, exclusive at 10]
			// Driver: [priority at 7, timestamp at 9, exclusive at 11]
			
			// Parse fields with error handling for driver data which might have different formats
			try
			{
				int priorityIndex = isDriverProvider ? 7 : 7;
				update.critical = array2.Length > priorityIndex && !string.IsNullOrEmpty(array2[priorityIndex]) ? Convert.ToInt16(array2[priorityIndex]) <= 3 : false;
			}
			catch
			{
				update.critical = false;
			}
			
			update.timesitamp = array2.Length > 9 ? array2[9] : "";
			
			try
			{
				int exclusiveIndex = isDriverProvider ? 11 : 10;
				update.exclusive = array2.Length > exclusiveIndex && !string.IsNullOrEmpty(array2[exclusiveIndex]) ? Convert.ToInt16(array2[exclusiveIndex]) == 1 : false;
			}
			catch
			{
				update.exclusive = false;
			}
			
			try
			{
				update.group = array2.Length > 3 && !string.IsNullOrEmpty(array2[3]) ? Convert.ToInt32(array2[3]) : 0;
			}
			catch
			{
				update.group = 0;
			}
			
			update.langscode = array2.Length > 2 ? array2[2] : "";
			update.code = _line;
			update.isDriver = isDriverProvider; // Mark if this is a driver update
			
			// Use the title from language data if available, otherwise use code
			if (string.IsNullOrEmpty(text) && update.lan.Length > 0)
			{
				text = update.lan[0].title;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = _line; // Fallback to update code
			}
			
			ListViewItem listViewItem = new ListViewItem(text);
			if (!update.critical)
			{
				listViewItem.ImageIndex = 2;
			}
			else if (update.exclusive)
			{
				listViewItem.ImageIndex = 0;
			}
			else
			{
				listViewItem.ImageIndex = 1;
			}
			listViewItem.SubItems.Add(_line);
			listViewItem.SubItems.Add(num.ToString());
			listViewItem.SubItems.Add(update.critical.ToString());
			listViewItem.SubItems.Add(update.exclusive.ToString());
			listViewItem.SubItems.Add(update.group.ToString());
			listViewItem.Tag = update;
			
			// Thread-safe add to list
			lock (lockObj)
			{
				lstItemCol.Add(listViewItem);
				p_items++;
			}
		}
		catch (Exception innerEx)
		{
			// Log the error but continue processing other items
			System.Diagnostics.Debug.WriteLine($"Error processing item {_line}: {innerEx.Message}");
		}
		finally
		{
			System.Threading.Interlocked.Increment(ref p_scanned);
		}
	});
}
catch (Exception ex)
{
	// If there's a complete failure, at least log it
	System.Diagnostics.Debug.WriteLine($"Critical error in loadItems: {ex.Message}\n{ex.StackTrace}");
	throw; // Re-throw so the BackgroundWorker can catch it
}
}

	private void frmItemList_Load(object sender, EventArgs e)
	{
		frmMain = (frmMain)base.Tag;
	}

	private void tmrLoad_Tick(object sender, EventArgs e)
	{
		try
		{
			if (frmMain.mdiTabs.SelectedForm == this)
			{
				frmMain.lblItems.Visible = true;
				frmMain.pbBusy.Visible = true;
				frmMain.RefreshUndoRedoButtons();
				if (bw.IsBusy)
				{
					// Show real progress once the total is known. A marquee gave no idea whether a provider
					// with thousands of updates was halfway through or wedged.
					int total = (u_items != null) ? u_items.Length : 0;
					if (total > 0)
					{
						frmMain.pbBusy.Style = ProgressBarStyle.Continuous;
						frmMain.pbBusy.Maximum = total;
						frmMain.pbBusy.Value = Math.Min(p_scanned, total);
						frmMain.lblItems.Text = $"{p_scanned} of {total} items";
					}
					else
					{
						frmMain.pbBusy.Style = ProgressBarStyle.Marquee;
						frmMain.lblItems.Text = $"{p_items} items";
					}
				}
				else
				{
					// Clear the bar as well as switching style. The value set while loading otherwise stays
					// painted, leaving a part filled bar sitting there after the load has finished.
					frmMain.pbBusy.Style = ProgressBarStyle.Blocks;
					frmMain.pbBusy.Value = 0;
					frmMain.lblItems.Text = $"{VisibleItemCount} items";
				}
			}
		}
		catch (ObjectDisposedException)
		{
			// The form closed while the background load was still reporting progress.
		}
		catch (InvalidOperationException)
		{
			// Same race, seen as a handle that is no longer valid. Any other failure is a real bug
			// and is now allowed to surface rather than being swallowed silently.
		}
	}

	private void lstItems_SelectedIndexChanged(object sender, EventArgs e)
	{
		foreach (ListViewItem lvi in lstItems.SelectedItems)
		{
			if (lstItems.SelectedItems.Count == 1)
			{
				Update upd = (Update)lvi.Tag;
				lblUpdTitle.Text = upd.getLang("en").title;
				lblUpdDescription.Text = upd.getLang("en").description;
				lblTimeStamp.Text = upd.timesitamp;
				if (upd.getLang("en").eulaUrl.StartsWith("http://"))
				{
					lblEula.Tag = upd.getLang("en").eulaUrl;
				}
				else
				{
					lblEula.Tag = "http://www.download.windowsupdate.com/msdownload/update/v3/static/RTF/" + upd.getLang("en").eulaUrl;
				}
				lstItems.Tag = lvi.SubItems[1].Text;
			}
		}
	}

	private void frmItemList_Activated(object sender, EventArgs e)
	{
		frmMain.Tag = this;
	}

	private void frmItemList_FormClosed(object sender, FormClosedEventArgs e)
	{
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	// Drag-drop event handlers
	private void LstItems_ItemDrag(object sender, ItemDragEventArgs e)
	{
		draggedItem = (ListViewItem)e.Item;
		lstItems.DoDragDrop(draggedItem, DragDropEffects.Move);
	}

	private void LstItems_DragEnter(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(typeof(ListViewItem)))
		{
			e.Effect = DragDropEffects.Move;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void LstItems_DragOver(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(typeof(ListViewItem)))
		{
			e.Effect = DragDropEffects.Move;
		}
	}

	private void LstItems_DragDrop(object sender, DragEventArgs e)
	{
		if (draggedItem == null) return;

		Point cp = lstItems.PointToClient(new Point(e.X, e.Y));
		ListViewItem targetItem = lstItems.GetItemAt(cp.X, cp.Y);

		if (targetItem == null || targetItem == draggedItem) return;

		Update draggedUpdate = (Update)draggedItem.Tag;
		Update targetUpdate = (Update)targetItem.Tag;

		// Only allow reordering within the same group
		if (draggedUpdate.group != targetUpdate.group)
		{
			MessageBox.Show("You can only reorder updates within the same group!", 
				"Different Groups", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}

		// Ask for confirmation
		DialogResult result = MessageBox.Show(
			$"Are you sure you want to change the update order?\n\n" +
			$"Moving: {draggedUpdate.getLang("en")?.title ?? draggedUpdate.code}\n" +
			$"To position of: {targetUpdate.getLang("en")?.title ?? targetUpdate.code}",
			"Confirm Order Change",
			MessageBoxButtons.YesNo,
			MessageBoxIcon.Question);

		if (result != DialogResult.Yes) return;

		// Reorder within the ListView
		int draggedIndex = draggedItem.Index;
		int targetIndex = targetItem.Index;

		lstItems.Items.Remove(draggedItem);
		lstItems.Items.Insert(targetIndex, draggedItem);

		// Update custom order for all items in this group
		ReassignGroupOrders(draggedUpdate.group);

		orderChanged = true;
		draggedItem = null;
	}

	private void ReassignGroupOrders(int groupId)
	{
		int order = 0;
		foreach (ListViewItem item in lstItems.Items)
		{
			Update upd = (Update)item.Tag;
			if (upd.group == groupId)
			{
				upd.customOrder = order++;
			}
		}
	}

	// The link had no handler attached at all, so clicking it did nothing. The URL for the selected
	// update is put in the label's Tag when the selection changes, so that is what gets opened.
	private void lblEula_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		string url = lblEula.Tag as string;
		if (string.IsNullOrEmpty(url))
		{
			MessageBox.Show("This update has no EULA link.", "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}
		try
		{
			lblEula.LinkVisited = true;
			System.Diagnostics.Process.Start(url);
		}
		catch (Exception ex)
		{
			MessageBox.Show("The EULA could not be opened:\n\n" + url + "\n\n" + ex.Message, "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public void OrganizeIntoGroups()
	{
		lstItems.BeginUpdate();
		lstItems.Groups.Clear();
		
		// Define group priority order (critical/service packs first, recommended second, etc.)
		var groupPriority = new Dictionary<int, int>
		{
			{ 90602, 1 },  // Critical Updates and Service Packs
			{ 90609, 2 },  // Recommended Updates  
			{ 90943, 3 },  // Internet and Multimedia Updates
			{ 90945, 4 },  // Multi-Language Features
			{ 90944, 5 },  // Additional Windows Downloads
			{ 90949, 6 },  // Windows Tools
			{ 90700, 7 }   // Drivers (if any)
		};
		
		// Get all unique groups from items
		// Only what matches is considered, so a category with no match does not appear at all.
		List<ListViewItem> visible = new List<ListViewItem>();
		foreach (ListViewItem candidate in lstItemCol)
		{
			if (ItemMatches(candidate, searchFilter)) visible.Add(candidate);
		}

		var groups = new Dictionary<int, ListViewGroup>();
		foreach (ListViewItem item in visible)
		{
			Update upd = (Update)item.Tag;
			if (!groups.ContainsKey(upd.group))
			{
				string groupName = GetGroupName(upd.group);
				ListViewGroup lvg = new ListViewGroup(groupName, groupName);
				lvg.Tag = upd.group;
				
				groups.Add(upd.group, lvg);
			}
		}
		
		// Sort groups by priority before adding them to lstItems
		var sortedGroups = groups.OrderBy(g => groupPriority.ContainsKey(g.Key) ? groupPriority[g.Key] : 999);
		foreach (var group in sortedGroups)
		{
			lstItems.Groups.Add(group.Value);
		}

		// Sort items by group priority and custom order (DESCENDING - newest first)
		var sortedItems = visible.OrderBy(item => {
											var upd = (Update)item.Tag;
											return groupPriority.ContainsKey(upd.group) ? groupPriority[upd.group] : 999;
										 })
									 .ThenByDescending(item => {
										 Update upd = ((Update)item.Tag);
										 // If custom order is set, use it; otherwise use original file position
										 if (upd.customOrder != -1)
											 return upd.customOrder;
										 // Use the first itemindex as the original file position
										 return (upd.itemindexes != null && upd.itemindexes.Length > 0) 
											 ? upd.itemindexes[0] 
											 : int.MaxValue;
									 })
									 .ToList();

		lstItems.Items.Clear();
		foreach (ListViewItem item in sortedItems)
		{
			Update upd = (Update)item.Tag;
			item.Group = groups[upd.group];
			lstItems.Items.Add(item);
		}

		lstItems.EndUpdate();
	}

	private string GetGroupName(int groupId)
	{
		// Look up the actual group name from productgroupstrings
		if (l_productgroupstrings != null)
		{
			// Find the line that matches provider.groupId.en
			string searchKey = $".{groupId}.en,";
			foreach (string line in l_productgroupstrings)
			{
				if (line.Contains(searchKey))
				{
					// Extract the name after the comma and before @|
					// Format is: win2k.windows2000.90602.en,Critical Updates and Service Packs@|@|@|@|
					int commaPos = line.IndexOf(',');
					if (commaPos > 0)
					{
						int separatorPos = line.IndexOf("@|", commaPos);
						if (separatorPos > commaPos)
						{
							string groupName = line.Substring(commaPos + 1, separatorPos - commaPos - 1).Trim();
							return groupName;
						}
					}
				}
			}
		}
		
		// Hardcoded fallback with custom group names
		switch (groupId)
		{
			case 90602: return "Critical Updates";
			case 90609: return "Recommended Updates";
			case 90943: return "Internet and Multimedia Updates";
			case 90945: return "Multi-Language Features";
			case 90944: return "Additional Windows Downloads";
			case 90949: return "Windows Tools";
			case 90952: return "Advanced Security Updates";
		}
		
		// Final fallback if not found
		return $"Group {groupId}";
	}

	// ================= Persisting display order (keep all dictionary files in sync) =================
	// Main-site display order = itemID order within each product2items line. When the user drags to
	private bool groupingHandlerAttached;

	// Clears the list and reloads it. Every caller used to attach its own RunWorkerCompleted handler
	// right before starting the worker, so refreshing or switching language N times left N handlers
	// attached and ran the grouping pass N times per reload. The handler is attached once here.
	public void ReloadItems()
	{
		if (!groupingHandlerAttached)
		{
			bw.RunWorkerCompleted += delegate
			{
				OrganizeIntoGroups();
			};
			groupingHandlerAttached = true;
		}
		p_items = 0;
		p_scanned = 0;
		u_items = null;
		lstItemCol = new List<ListViewItem>();
		lstItems.Items.Clear();
		bw.RunWorkerAsync();
	}

	// Removes duplicate entries from every dictionary file. Repeated add and edit cycles used to
	// compound duplicate lines until a provider stopped loading at all, because the catalog parses
	// each file as a whole. Called on save. Returns the number of duplicate lines removed.
	public int SanitizeProvider()
	{
		int removed = 0;
		removed += DedupExact(ref l_items);
		removed += DedupExact(ref l_itemsindex);
		removed += DedupExact(ref l_itemstringsindex);
		removed += DedupExact(ref l_itemstrings);
		removed += MergeProduct2Items(ref l_product2items);
		return removed;
	}

	// Drops exact duplicate non empty lines, keeping the first occurrence and preserving order.
	private static int DedupExact(ref string[] arr)
	{
		if (arr == null || arr.Length == 0)
		{
			return 0;
		}
		HashSet<string> seen = new HashSet<string>(StringComparer.Ordinal);
		List<string> kept = new List<string>(arr.Length);
		int removed = 0;
		foreach (string line in arr)
		{
			if (string.IsNullOrEmpty(line))
			{
				kept.Add(line);
				continue;
			}
			if (seen.Add(line))
			{
				kept.Add(line);
			}
			else
			{
				removed++;
			}
		}
		if (removed > 0)
		{
			arr = kept.ToArray();
		}
		return removed;
	}

	// product2items must hold one line per key. Merges duplicate key lines and removes repeated
	// item ids inside each line.
	private static int MergeProduct2Items(ref string[] arr)
	{
		if (arr == null || arr.Length == 0)
		{
			return 0;
		}
		List<string> order = new List<string>();
		Dictionary<string, List<string>> vals = new Dictionary<string, List<string>>(StringComparer.Ordinal);
		Dictionary<string, HashSet<string>> seenVals = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
		foreach (string line in arr)
		{
			if (string.IsNullOrEmpty(line))
			{
				continue;
			}
			string[] parts = line.Split(',');
			string key = parts[0];
			if (!vals.ContainsKey(key))
			{
				vals[key] = new List<string>();
				seenVals[key] = new HashSet<string>(StringComparer.Ordinal);
				order.Add(key);
			}
			for (int j = 1; j < parts.Length; j++)
			{
				if (parts[j].Length == 0)
				{
					continue;
				}
				if (seenVals[key].Add(parts[j]))
				{
					vals[key].Add(parts[j]);
				}
			}
		}
		List<string> merged = new List<string>(order.Count);
		foreach (string key in order)
		{
			List<string> v = vals[key];
			merged.Add((v.Count > 0) ? (key + "," + string.Join(",", v)) : key);
		}
		int lineDelta = arr.Length - merged.Count;
		arr = merged.ToArray();
		return lineDelta;
	}

	// Every items.txt record opens with a GUID and a comma. Every record in the other four files
	// opens with the provider name and a dot.
	private static readonly System.Text.RegularExpressions.Regex ItemsRecordStart =
		new System.Text.RegularExpressions.Regex(@"^[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12},");

	// A stray newline inside a record splits one logical line into two physical ones. That cuts the
	// installation block in half, which is what makes the catalog fail to parse and blanks the whole
	// result list. A line that does not open a record is a continuation of the line above it.
	//
	// The opening token is taken from each file's OWN first line, never from the folder name. A copied
	// or renamed folder still holds records naming the original provider, so keying off the folder
	// would match nothing and collapse every line in the file into one.
	public int RepairSplitRecords()
	{
		int joins = 0;
		joins += RejoinRecords(ref l_items, null, true);
		joins += RejoinRecords(ref l_itemsindex, DeriveRecordPrefix(l_itemsindex));
		joins += RejoinRecords(ref l_itemstringsindex, DeriveRecordPrefix(l_itemstringsindex));
		joins += RejoinRecords(ref l_itemstrings, DeriveRecordPrefix(l_itemstrings));
		joins += RejoinRecords(ref l_product2items, DeriveRecordPrefix(l_product2items));
		return joins;
	}

	// The leading token of the file's first record, up to and including the first dot. Null when it
	// cannot be established, in which case that file is left alone rather than guessed at.
	private static string DeriveRecordPrefix(string[] arr)
	{
		if (arr == null) return null;
		foreach (string line in arr)
		{
			if (string.IsNullOrEmpty(line)) continue;
			// itemstrings.txt is UTF-16 with a byte order mark. If one survives into the first line
			// it would poison the token and make every following line look like a continuation.
			string first = line.TrimStart('\ufeff');
			int dot = first.IndexOf('.');
			if (dot <= 0) return null;
			return first.Substring(0, dot + 1);
		}
		return null;
	}

	// expectedPrefix null with an items.txt array means use the GUID rule. A null prefix for any
	// other file means the opening token could not be established, so nothing is touched.
	private static int RejoinRecords(ref string[] arr, string expectedPrefix, bool guidRule = false)
	{
		if (arr == null || arr.Length == 0) return 0;
		if (!guidRule && expectedPrefix == null) return 0;
		List<string> joined = new List<string>(arr.Length);
		int lastRecord = -1;
		int joins = 0;
		foreach (string line in arr)
		{
			if (string.IsNullOrEmpty(line))
			{
				joined.Add(line);
				continue;
			}
			string candidate = line.TrimStart('\ufeff');
			bool startsRecord = guidRule
				? ItemsRecordStart.IsMatch(candidate)
				: candidate.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase);
			if (startsRecord || lastRecord < 0)
			{
				joined.Add(line);
				lastRecord = joined.Count - 1;
			}
			else
			{
				joined[lastRecord] = joined[lastRecord] + line;
				joins++;
			}
		}
		// A genuine split affects a handful of records. Anything that would swallow most of the file
		// means the opening token is wrong for this data, so leave it untouched rather than destroy it.
		if (joins > arr.Length / 4)
		{
			return 0;
		}
		if (joins > 0)
		{
			arr = joined.ToArray();
		}
		return joins;
	}


	// Set by the background load when building the list throws. The failure is dealt with on the UI
	// thread by the completion handler, rather than a message box being raised from the worker.
	public Exception loadError;

	private bool repairOffered;

	// Offers to repair an inventory that would not load. Returns true when the caller should try the
	// load again. Only offered once per provider, so a repair that does not help cannot loop.
	public bool OfferCorruptionRepair()
	{
		Exception failure = loadError;
		loadError = null;
		if (failure == null)
		{
			return false;
		}

		if (repairOffered)
		{
			MessageBox.Show("This inventory still could not be loaded after being repaired.\n\n" + failure.Message + "\n\nThe remaining damage has to be corrected by hand.", "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
		repairOffered = true;

		if (MessageBox.Show("This inventory could not be loaded and appears to be corrupted.\n\n" + failure.Message + "\n\nFix corrupted inventory?", "Windows Update v4.0 PowerTools", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
		{
			return false;
		}

		// Capture the dictionaries before repairing them, so this can be taken back with Undo.
		PushUndoState();
		// Rejoin first: a split record is not a duplicate and is not malformed XML in its own right,
		// it is half a record, so the other passes cannot make sense of it until it is whole again.
		int rejoined = RepairSplitRecords();
		int duplicates = SanitizeProvider();
		int eulaFixed = RepairEulaEscaping();
		int orphanIndex = RepairOrphanedIndexEntries();
		int orphanStrings = RepairOrphanedStringIndex();
		List<string> stillBroken = FindCatalogBreakingRecords();

		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.AppendLine(rejoined + " split records were joined back together.");
		sb.AppendLine(duplicates + " duplicate lines were removed.");
		sb.AppendLine(eulaFixed + " EULA links were escaped.");
		sb.AppendLine(orphanIndex + " index entries pointing at a missing update were removed.");
		sb.AppendLine(orphanStrings + " string entries pointing at a missing row were removed.");
		if (stillBroken.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine(stillBroken.Count + " records are still malformed and have to be corrected by hand:");
			foreach (string problem in stillBroken.Take(5))
			{
				sb.AppendLine("    " + problem);
			}
			if (stillBroken.Count > 5)
			{
				sb.AppendLine("    and " + (stillBroken.Count - 5) + " more.");
			}
		}
		sb.AppendLine();
		sb.AppendLine("Nothing has been written yet. Save the provider to keep these repairs.");
		MessageBox.Show(sb.ToString(), "Windows Update v4.0 PowerTools", MessageBoxButtons.OK, MessageBoxIcon.Information);
		return true;
	}

	// items.txt field 5 is the <installation> block and itemstrings.txt field 4 is the EULA link.
	// The catalog page concatenates these raw into one XML document which the browser parses in a
	// single shot, and that parse is all or nothing: one malformed record blanks the ENTIRE result
	// list for every operating system and language whose search touches it. This app writes both
	// fields itself, so they are checked before every save rather than after the damage ships.
	private const int InstallationField = 5;

	private const int EulaField = 4;

	private static readonly System.Text.RegularExpressions.Regex BareAmpersand =
		new System.Text.RegularExpressions.Regex(@"&(?!(?:amp|lt|gt|quot|apos|#\d+|#x[0-9A-Fa-f]+);)");

	private static readonly string[] FieldSeparator = new string[1] { "@|" };

	// Returns one description per record that would break the catalog, with the line number and id
	// so it can actually be found. Empty when there is nothing wrong.
	public List<string> FindCatalogBreakingRecords()
	{
		List<string> problems = new List<string>();

		if (l_items != null)
		{
			for (int i = 0; i < l_items.Length; i++)
			{
				string line = l_items[i];
				if (string.IsNullOrEmpty(line)) continue;
				string[] parts = line.Split(FieldSeparator, StringSplitOptions.None);
				if (parts.Length <= InstallationField) continue;
				if (!IsWellFormedXml(parts[InstallationField]))
				{
					problems.Add("items.txt line " + (i + 1) + " (" + GuidOf(line) + "): the installation block is not well formed XML.");
				}
			}
		}

		if (l_itemstrings != null)
		{
			for (int i = 0; i < l_itemstrings.Length; i++)
			{
				string line = l_itemstrings[i];
				if (string.IsNullOrEmpty(line)) continue;
				string[] parts = line.Split(FieldSeparator, StringSplitOptions.None);
				if (parts.Length <= EulaField) continue;
				if (NeedsEscaping(parts[EulaField]))
				{
					problems.Add("itemstrings.txt line " + (i + 1) + ": the EULA link contains characters that break the XML attribute.");
				}
			}
		}
		return problems;
	}

	// Escapes the characters that break the XML attribute the EULA link sits in. This is mechanical
	// and safe. A malformed installation block is not, so that is only ever reported, never rewritten.
	public int RepairEulaEscaping()
	{
		if (l_itemstrings == null) return 0;
		int repaired = 0;
		for (int i = 0; i < l_itemstrings.Length; i++)
		{
			string line = l_itemstrings[i];
			if (string.IsNullOrEmpty(line)) continue;
			string[] parts = line.Split(FieldSeparator, StringSplitOptions.None);
			if (parts.Length <= EulaField) continue;
			string eula = parts[EulaField];
			if (!NeedsEscaping(eula)) continue;
			// Ampersands first, so the entities introduced below are not escaped a second time.
			parts[EulaField] = BareAmpersand.Replace(eula, "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
			l_itemstrings[i] = string.Join("@|", parts);
			repaired++;
		}
		return repaired;
	}

	private static bool IsWellFormedXml(string fragment)
	{
		if (string.IsNullOrEmpty(fragment)) return true;
		try
		{
			System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
			settings.DtdProcessing = System.Xml.DtdProcessing.Prohibit;
			settings.XmlResolver = null;
			using (System.IO.StringReader text = new System.IO.StringReader(fragment))
			using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(text, settings))
			{
				while (reader.Read())
				{
				}
			}
			return true;
		}
		catch (System.Xml.XmlException)
		{
			return false;
		}
	}

	private static bool NeedsEscaping(string value)
	{
		if (string.IsNullOrEmpty(value)) return false;
		return value.IndexOf('<') >= 0 || value.IndexOf('>') >= 0 || BareAmpersand.IsMatch(value);
	}

	private static string GuidOf(string itemsLine)
	{
		string head = itemsLine.Split(FieldSeparator, StringSplitOptions.None)[0];
		int comma = head.IndexOf(',');
		return (comma > 0) ? head.Substring(0, comma) : head;
	}

	// Cross checks the five dictionary files against each other. Returns null when nothing is broken,
	// otherwise a description of the damage. Code and id comparisons are case insensitive, because
	// items.txt is mixed case while itemsindex and product2items hold the same codes lowercased.
	//
	// coverageGaps is reported separately and is NOT damage. A product2items reference with no
	// itemsindex entry means that update is simply not present for that locale, which is the normal
	// state for any language whose files have not been obtained. The client offers nothing for it.
	// Only genuine breakage, where an entry points at a row that should exist but does not, is
	// treated as a problem worth blocking a save over.
	public string ValidateProvider(out int coverageGaps)
	{
		HashSet<string> itemGuids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string line in l_items ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			int comma = line.IndexOf(',');
			if (comma > 0) itemGuids.Add(line.Substring(0, comma).Trim());
		}

		HashSet<string> indexKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		int orphanIndexGuids = 0;
		foreach (string line in l_itemsindex ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			// The GUID is the last comma field BEFORE the first @|, because everything after that
			// separator is the dependencies field written by the prerequisites feature. Reading to
			// the last comma on the whole line glues the GUID onto a prerequisite reference and makes
			// every line that has prerequisites look like it points at a missing update.
			string key = IndexItemId(line);
			string guid = IndexGuid(line);
			if (key.Length > 0) indexKeys.Add(key.Trim());
			if (guid.Length > 0 && !itemGuids.Contains(guid)) orphanIndexGuids++;
		}

		int danglingRefs = 0;
		foreach (string line in l_product2items ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			string[] parts = line.Split(',');
			for (int j = 1; j < parts.Length; j++)
			{
				string re = parts[j].Trim();
				if (re.Length == 0) continue;
				if (!indexKeys.Contains(provider + "." + re)) danglingRefs++;
			}
		}

		HashSet<string> stringSetGuids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string line in l_itemstrings ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			int comma = line.IndexOf(',');
			if (comma <= 0) continue;
			string key = line.Substring(0, comma);
			int dot = key.LastIndexOf('.');
			stringSetGuids.Add(((dot >= 0) ? key.Substring(dot + 1) : key).Trim());
		}

		int missingStringRows = 0;
		foreach (string line in l_itemstringsindex ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			string head = line.Split(FieldSeparator, StringSplitOptions.None)[0];
			int comma = head.LastIndexOf(',');
			if (comma <= 0) continue;
			string target = head.Substring(comma + 1).Trim();
			if (target.Length > 0 && !stringSetGuids.Contains(target)) missingStringRows++;
		}

		coverageGaps = danglingRefs;
		List<string> catalogBreaking = FindCatalogBreakingRecords();
		if (orphanIndexGuids == 0 && missingStringRows == 0 && catalogBreaking.Count == 0)
		{
			return null;
		}
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		if (catalogBreaking.Count > 0)
		{
			sb.AppendLine(catalogBreaking.Count + " records would blank the whole catalog result list:");
			foreach (string problem in catalogBreaking.Take(5))
			{
				sb.AppendLine("    " + problem);
			}
			if (catalogBreaking.Count > 5)
			{
				sb.AppendLine("    and " + (catalogBreaking.Count - 5) + " more.");
			}
		}
		if (orphanIndexGuids > 0) sb.AppendLine(orphanIndexGuids + " itemsindex entries point at an update with no items.txt row, so the update resolves to nothing.");
		if (missingStringRows > 0) sb.AppendLine(missingStringRows + " itemstringsindex entries point at an itemstrings row that does not exist, so the title and description are missing.");
		return sb.ToString();
	}

	// Removes product2items references that resolve to nothing. These are what make a client offer
	// an update it cannot then download, which is most common on locales that were never built out.
	// Returns the number of references dropped.
	public int RepairDanglingReferences()
	{
		if (l_product2items == null || l_itemsindex == null) return 0;
		HashSet<string> indexKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string line in l_itemsindex)
		{
			if (string.IsNullOrEmpty(line)) continue;
			string key = IndexItemId(line);
			if (key.Length > 0) indexKeys.Add(key.Trim());
		}
		int dropped = 0;
		List<string> rebuilt = new List<string>(l_product2items.Length);
		foreach (string line in l_product2items)
		{
			if (string.IsNullOrEmpty(line)) { rebuilt.Add(line); continue; }
			string[] parts = line.Split(',');
			List<string> keep = new List<string>(parts.Length);
			for (int j = 1; j < parts.Length; j++)
			{
				string re = parts[j].Trim();
				if (re.Length == 0) continue;
				if (indexKeys.Contains(provider + "." + re)) keep.Add(parts[j]); else dropped++;
			}
			rebuilt.Add((keep.Count > 0) ? (parts[0] + "," + string.Join(",", keep)) : parts[0]);
		}
		if (dropped > 0) l_product2items = rebuilt.ToArray();
		return dropped;
	}

	// reorder, ApplyDisplayOrder() rewrites product2items to the on-screen order AND relocates every
	// affected update's lines in items/itemsindex/itemstringsindex/itemstrings so each update's entries
	// stay contiguous and in the same relative order across every file. Call on the UI thread (it reads
	// the ListView) before saving. No-op unless the user actually reordered.
	public bool orderWasChanged => orderChanged;

	// itemsindex line = "<itemID>,<GUID>@|<dependencies>". Returns the GUID only.
	private static string IndexGuid(string line)
	{
		string beforeAt = line.Split(FieldSeparator, StringSplitOptions.None)[0];
		int lastComma = beforeAt.LastIndexOf(',');
		return (lastComma > 0) ? beforeAt.Substring(lastComma + 1).Trim() : string.Empty;
	}

	// Drops itemsindex lines whose GUID has no items.txt row. Such a line offers an update that
	// resolves to nothing, so removing it is what makes the provider consistent again.
	public int RepairOrphanedIndexEntries()
	{
		if (l_itemsindex == null || l_items == null) return 0;
		HashSet<string> itemGuids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string line in l_items)
		{
			if (string.IsNullOrEmpty(line)) continue;
			int comma = line.IndexOf(',');
			if (comma > 0) itemGuids.Add(line.Substring(0, comma).Trim());
		}
		List<string> kept = new List<string>(l_itemsindex.Length);
		int dropped = 0;
		foreach (string line in l_itemsindex)
		{
			if (string.IsNullOrEmpty(line)) { kept.Add(line); continue; }
			string guid = IndexGuid(line);
			if (guid.Length > 0 && !itemGuids.Contains(guid)) { dropped++; continue; }
			kept.Add(line);
		}
		if (dropped > 0) l_itemsindex = kept.ToArray();
		return dropped;
	}

	// Drops itemstringsindex lines pointing at an itemstrings row that does not exist. Such a line
	// leaves an update with no title or description for that locale.
	public int RepairOrphanedStringIndex()
	{
		if (l_itemstringsindex == null || l_itemstrings == null) return 0;
		HashSet<string> stringSetGuids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string line in l_itemstrings)
		{
			if (string.IsNullOrEmpty(line)) continue;
			int comma = line.IndexOf(',');
			if (comma <= 0) continue;
			string k = line.Substring(0, comma);
			int dot = k.LastIndexOf('.');
			stringSetGuids.Add(((dot >= 0) ? k.Substring(dot + 1) : k).Trim());
		}
		List<string> kept = new List<string>(l_itemstringsindex.Length);
		int dropped = 0;
		foreach (string line in l_itemstringsindex)
		{
			if (string.IsNullOrEmpty(line)) { kept.Add(line); continue; }
			string head = line.Split(FieldSeparator, StringSplitOptions.None)[0];
			int comma = head.LastIndexOf(',');
			if (comma > 0)
			{
				string target = head.Substring(comma + 1).Trim();
				if (target.Length > 0 && !stringSetGuids.Contains(target)) { dropped++; continue; }
			}
			kept.Add(line);
		}
		if (dropped > 0) l_itemstringsindex = kept.ToArray();
		return dropped;
	}

	// itemsindex line = "<itemID>,<GUID>@|...". Returns the itemID (incl. provider prefix).
	private static string IndexItemId(string line)
	{
		string beforeAt = line.Split(new string[] { "@|" }, StringSplitOptions.None)[0];
		int lastComma = beforeAt.LastIndexOf(',');
		return (lastComma > 0) ? beforeAt.Substring(0, lastComma) : beforeAt;
	}

	// The update code embedded after "com_microsoft." in an itemID (up to the next '.'), or null.
	private static string CodeFromItemId(string itemId)
	{
		if (string.IsNullOrEmpty(itemId)) return null;
		int idx = itemId.IndexOf("com_microsoft.", StringComparison.Ordinal);
		if (idx < 0) return null;
		string tail = itemId.Substring(idx + "com_microsoft.".Length);
		int dot = tail.IndexOf('.');
		return (dot >= 0) ? tail.Substring(0, dot) : tail;
	}

	// The trailing GUID of a "provider.locale.GUID..." key, i.e. the segment after the last '.' before
	// the first comma.
	private static string TrailingGuidBeforeComma(string line)
	{
		int comma = line.IndexOf(',');
		string key = (comma >= 0) ? line.Substring(0, comma) : line;
		int dot = key.LastIndexOf('.');
		return (dot >= 0) ? key.Substring(dot + 1) : key;
	}

	// Re-emit 'lines' so every line mapping (via keyOf) to an update code is grouped contiguously in the
	// order given by 'order'. Lines that map to no listed code keep their original order at the end.
	private static string[] ReorderLinesByUpdate(string[] lines, List<string> order, HashSet<string> orderSet, Func<string, string> keyOf)
	{
		if (lines == null || lines.Length == 0) return lines;
		// Codes are compared case-insensitively: items.txt stores mixed-case codes while itemsindex /
		// product2items store the same code lowercased in the itemID.
		var buckets = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
		var orphans = new List<string>();
		foreach (string line in lines)
		{
			string k = string.IsNullOrEmpty(line) ? null : keyOf(line);
			if (k != null && orderSet.Contains(k))
			{
				if (!buckets.TryGetValue(k, out var b)) { b = new List<string>(); buckets[k] = b; }
				b.Add(line);
			}
			else
			{
				orphans.Add(line);
			}
		}
		var result = new List<string>(lines.Length);
		foreach (string code in order)
		{
			if (buckets.TryGetValue(code, out var b)) result.AddRange(b);
		}
		result.AddRange(orphans);
		return result.ToArray();
	}

	public bool ApplyDisplayOrder()
	{
		if (!orderChanged) return false;

		// 1. Ordered, distinct update codes as currently shown (top to bottom, grouped).
		// Code comparisons are case-insensitive (items.txt is mixed-case; itemsindex/product2items lowercase).
		var order = new List<string>();
		var orderSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var langGuidToCode = new Dictionary<string, string>();
		foreach (ListViewItem it in lstItems.Items)
		{
			if (!(it.Tag is Update u) || string.IsNullOrEmpty(u.code)) continue;
			if (orderSet.Add(u.code)) order.Add(u.code);
			if (!string.IsNullOrEmpty(u.langscode) && !langGuidToCode.ContainsKey(u.langscode))
				langGuidToCode[u.langscode] = u.code;
		}
		if (order.Count == 0) return false;

		var codeOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		for (int i = 0; i < order.Count; i++) codeOrder[order[i]] = i;

		// 2. product2items: reorder each line's item ids (keep the leading product key fixed).
		if (l_product2items != null)
		{
			for (int i = 0; i < l_product2items.Length; i++)
			{
				string line = l_product2items[i];
				if (string.IsNullOrEmpty(line)) continue;
				string[] parts = line.Split(',');
				if (parts.Length <= 2) continue; // key + at most one id: nothing to reorder
				string key = parts[0];
				var ids = new List<string>();
				for (int j = 1; j < parts.Length; j++) ids.Add(parts[j]);
				var sorted = ids.OrderBy(id =>
				{
					string c = CodeFromItemId(id);
					return (c != null && codeOrder.TryGetValue(c, out int p)) ? p : int.MaxValue;
				}).ToList(); // OrderBy is stable: unknown ids keep their relative order at the end
				l_product2items[i] = key + "," + string.Join(",", sorted);
			}
		}

		// 3. items / itemsindex: group each update's lines contiguously in the new order.
		l_items = ReorderLinesByUpdate(l_items, order, orderSet, line =>
		{
			string[] p = line.Split('@')[0].Split(',');
			return (p.Length > 1) ? p[1] : null; // items line = "<GUID>,<code>@|..."
		});
		l_itemsindex = ReorderLinesByUpdate(l_itemsindex, order, orderSet, line => CodeFromItemId(IndexItemId(line)));

		// 4. itemstringsindex: keyed by the update's string-set GUID (langscode). Build stringGuid->code
		//    along the way so itemstrings can be reordered too.
		var stringGuidToCode = new Dictionary<string, string>();
		if (l_itemstringsindex != null)
		{
			foreach (string line in l_itemstringsindex)
			{
				if (string.IsNullOrEmpty(line)) continue;
				string langGuid = TrailingGuidBeforeComma(line);
				if (!langGuidToCode.TryGetValue(langGuid, out string code)) continue;
				int comma = line.IndexOf(',');
				if (comma < 0 || comma + 1 >= line.Length) continue;
				string stringSetGuid = line.Substring(comma + 1).Trim();
				if (!string.IsNullOrEmpty(stringSetGuid) && !stringGuidToCode.ContainsKey(stringSetGuid))
					stringGuidToCode[stringSetGuid] = code;
			}
		}
		l_itemstringsindex = ReorderLinesByUpdate(l_itemstringsindex, order, orderSet, line =>
		{
			string langGuid = TrailingGuidBeforeComma(line);
			return langGuidToCode.TryGetValue(langGuid, out string c) ? c : null;
		});

		// 5. itemstrings: keyed by its string-set GUID.
		l_itemstrings = ReorderLinesByUpdate(l_itemstrings, order, orderSet, line =>
		{
			string guid = TrailingGuidBeforeComma(line);
			return stringGuidToCode.TryGetValue(guid, out string c) ? c : null;
		});

		orderChanged = false;
		return true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WUv4Powertools.frmItemList));
		this.lstItems = new WindowsFormsAero.ListView();
		this.colUpdName = new System.Windows.Forms.ColumnHeader();
		this.colUpdCode = new System.Windows.Forms.ColumnHeader();
		this.colLangCount = new System.Windows.Forms.ColumnHeader();
		this.colUpdCritical = new System.Windows.Forms.ColumnHeader();
		this.colUpdExclusive = new System.Windows.Forms.ColumnHeader();
		this.colUpdGroup = new System.Windows.Forms.ColumnHeader();
		this.imgLst = new System.Windows.Forms.ImageList(this.components);
		this.tmrLoad = new System.Windows.Forms.Timer(this.components);
		this.panelRight = new System.Windows.Forms.Panel();
		this.lblUpdDescription = new System.Windows.Forms.TextBox();
		this.lblTimeStamp = new System.Windows.Forms.Label();
		this.lblEula = new System.Windows.Forms.LinkLabel();
		this.lblUpdTitle = new System.Windows.Forms.Label();
		this.lblRight = new System.Windows.Forms.Label();
		this.panelRight.SuspendLayout();
		base.SuspendLayout();
		this.lstItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.lstItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[6] { this.colUpdName, this.colUpdCode, this.colLangCount, this.colUpdCritical, this.colUpdExclusive, this.colUpdGroup });
		this.lstItems.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lstItems.FullRowSelect = true;
		this.lstItems.HideSelection = false;
		this.lstItems.LabelEdit = true;
		this.lstItems.Location = new System.Drawing.Point(0, 0);
		this.lstItems.MultiSelect = false;
		this.lstItems.Name = "lstItems";
		this.lstItems.Size = new System.Drawing.Size(599, 450);
		this.lstItems.SmallImageList = this.imgLst;
		this.lstItems.Sorting = System.Windows.Forms.SortOrder.None;
		this.lstItems.TabIndex = 0;
		this.lstItems.UseCompatibleStateImageBehavior = false;
		this.lstItems.View = System.Windows.Forms.View.Details;
		this.lstItems.ShowGroups = true;
		this.lstItems.SelectedIndexChanged += new System.EventHandler(lstItems_SelectedIndexChanged);
		this.colUpdName.Text = "Update Name";
		this.colUpdName.Width = 250;
		this.colUpdCode.Text = "Update Code";
		this.colUpdCode.Width = 120;
		this.colLangCount.Text = "L. Count";
		this.colLangCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.colUpdCritical.Text = "Critical";
		this.colUpdCritical.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.colUpdExclusive.Text = "Exclusive";
		this.colUpdExclusive.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.colUpdGroup.Text = "Group";
		this.colUpdGroup.Width = 100;
		this.imgLst.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imgLst.ImageStream");
		this.imgLst.TransparentColor = System.Drawing.Color.Transparent;
		this.imgLst.Images.SetKeyName(0, "red_orb.ico");
		this.imgLst.Images.SetKeyName(1, "orange_orb.ico");
		this.imgLst.Images.SetKeyName(2, "green_orb.ico");
		this.tmrLoad.Enabled = true;
		this.tmrLoad.Tick += new System.EventHandler(tmrLoad_Tick);
		this.panelRight.BackColor = System.Drawing.SystemColors.Menu;
		this.panelRight.Controls.Add(this.lblUpdDescription);
		this.panelRight.Controls.Add(this.lblTimeStamp);
		this.panelRight.Controls.Add(this.lblEula);
		this.panelRight.Controls.Add(this.lblUpdTitle);
		this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.panelRight.Location = new System.Drawing.Point(600, 0);
		this.panelRight.Name = "panelRight";
		this.panelRight.Padding = new System.Windows.Forms.Padding(4);
		this.panelRight.Size = new System.Drawing.Size(200, 450);
		this.panelRight.TabIndex = 5;
		this.lblUpdDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.lblUpdDescription.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lblUpdDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblUpdDescription.Location = new System.Drawing.Point(4, 64);
		this.lblUpdDescription.Multiline = true;
		this.lblUpdDescription.Name = "lblUpdDescription";
		this.lblUpdDescription.ReadOnly = true;
		this.lblUpdDescription.Size = new System.Drawing.Size(192, 346);
		this.lblUpdDescription.TabIndex = 1;
		this.lblTimeStamp.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.lblTimeStamp.Location = new System.Drawing.Point(4, 410);
		this.lblTimeStamp.Name = "lblTimeStamp";
		this.lblTimeStamp.Padding = new System.Windows.Forms.Padding(2);
		this.lblTimeStamp.Size = new System.Drawing.Size(192, 18);
		this.lblTimeStamp.TabIndex = 3;
		this.lblEula.ActiveLinkColor = System.Drawing.SystemColors.HotTrack;
		this.lblEula.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(lblEula_LinkClicked);
		this.lblEula.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.lblEula.LinkColor = System.Drawing.SystemColors.Highlight;
		this.lblEula.Location = new System.Drawing.Point(4, 428);
		this.lblEula.Name = "lblEula";
		this.lblEula.Padding = new System.Windows.Forms.Padding(2);
		this.lblEula.Size = new System.Drawing.Size(192, 18);
		this.lblEula.TabIndex = 2;
		this.lblEula.TabStop = true;
		this.lblEula.Text = "Read EULA";
		this.lblUpdTitle.Dock = System.Windows.Forms.DockStyle.Top;
		this.lblUpdTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lblUpdTitle.Location = new System.Drawing.Point(4, 4);
		this.lblUpdTitle.Name = "lblUpdTitle";
		this.lblUpdTitle.Size = new System.Drawing.Size(192, 60);
		this.lblUpdTitle.TabIndex = 0;
		this.lblRight.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.lblRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.lblRight.Location = new System.Drawing.Point(599, 0);
		this.lblRight.Name = "lblRight";
		this.lblRight.Size = new System.Drawing.Size(1, 450);
		this.lblRight.TabIndex = 6;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.Control;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.lstItems);
		base.Controls.Add(this.lblRight);
		base.Controls.Add(this.panelRight);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frmItemList";
		this.Text = "Operating System";
		base.Activated += new System.EventHandler(frmItemList_Activated);
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(frmItemList_FormClosed);
		base.Load += new System.EventHandler(frmItemList_Load);
		this.panelRight.ResumeLayout(false);
		this.panelRight.PerformLayout();
		base.ResumeLayout(false);
	}
}
