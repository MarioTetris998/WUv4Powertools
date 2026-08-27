using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WUv4Powertools;

public class frmProvider : Form
{
	private int index;

	private IContainer components;

	public Button btnLoad;

	public ListBox lstProviders;

	public frmProvider()
	{
		InitializeComponent();
		Font = SystemFonts.MenuFont;
	}

	private void btnLoad_Click(object sender, EventArgs e)
	{
		try
		{
			frmMain frmMain2 = (frmMain)base.Tag;
			if (frmMain2 == null)
			{
				MessageBox.Show("Error: Main form reference is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			if (lstProviders.SelectedIndex < 0)
			{
				MessageBox.Show("Please select a provider from the list.", "No Provider Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			
			frmItemList frmItemList2 = new frmItemList();
			Invoke((MethodInvoker)delegate
			{
				try
				{
					// Check if we're loading from a consumerdrivers folder
					bool isDriverFolder = frmMain2.folderBrowserDialogSrc.Contains("consumerdrivers") || 
										  frmMain2.folderBrowserDialogSrc.Contains("drivers");
					frmItemList2.isDriverProvider = isDriverFolder;
					
					string[] array = File.ReadAllLines(frmMain2.folderBrowserDialogSrc + "\\providers.txt");
					for (int i = 0; i < array.Length; i++)
					{
						if (i == lstProviders.SelectedIndex)
						{
							frmItemList2.provider = array[i].Split(',')[0];
							
							// Validate all files exist before trying to load them
							string basePath = $"{frmMain2.folderBrowserDialogSrc}\\{frmItemList2.provider}";
							string[] requiredFiles = {
								"items.txt",
								"itemsindex.txt",
								"itemstrings.txt",
								"itemstringsindex.txt",
								"product2items.txt",
								"productgroupstrings.txt",
								"products.txt"
							};
							
							foreach (string file in requiredFiles)
							{
								string filePath = $"{basePath}\\{file}";
								if (!File.Exists(filePath))
								{
									MessageBox.Show($"Required file not found: {file}\n\nPath: {filePath}", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Error);
									return;
								}
							}
							
							// All files exist, proceed with loading
							// latin-1 (ISO-8859-1) for the index/body files so they round-trip losslessly with the save path.
							Encoding latin1 = Encoding.GetEncoding("ISO-8859-1");
							frmItemList2.l_items = File.ReadAllLines($"{basePath}\\items.txt", latin1);
							frmItemList2.l_itemsindex = File.ReadAllLines($"{basePath}\\itemsindex.txt", latin1);
							frmItemList2.l_itemstrings = File.ReadAllLines($"{basePath}\\itemstrings.txt", Encoding.Unicode);
							frmItemList2.l_itemstringsindex = File.ReadAllLines($"{basePath}\\itemstringsindex.txt", latin1);
							frmItemList2.l_product2items = File.ReadAllLines($"{basePath}\\product2items.txt", latin1);
							// productgroupstrings.txt is UTF-16LE with BOM, like itemstrings.txt.
							frmItemList2.l_productgroupstrings = File.ReadAllLines($"{basePath}\\productgroupstrings.txt", Encoding.Unicode);
							frmItemList2.l_products = File.ReadAllLines($"{basePath}\\products.txt", latin1);
						}
					}
					
					if (lstProviders.SelectedItem == null)
					{
						MessageBox.Show("Selected provider item is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					
					frmItemList2.Text = lstProviders.SelectedItem.ToString();
					frmItemList2.Tag = base.Tag;
					frmMain2.mdiTabs.TabPages.Add(frmItemList2);
					frmItemList2.bw.DoWork += delegate
					{
						try
						{
							frmItemList2.loadItems();
						}
						catch (Exception ex)
						{
							MessageBox.Show($"Error loading items: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					};
					frmItemList2.bw.RunWorkerCompleted += delegate
					{
						try
						{
							if (frmItemList2 == null)
							{
								return;
							}
							
							// Ensure we're on the UI thread
							if (frmItemList2.InvokeRequired)
							{
								frmItemList2.Invoke(new Action(() =>
								{
									AddItemsToList(frmItemList2);
								}));
							}
							else
							{
								AddItemsToList(frmItemList2);
							}
						}
						catch (Exception ex)
						{
							MessageBox.Show($"Error completing load: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					};
					frmItemList2.bw.RunWorkerAsync();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error loading provider: {ex.Message}\n\nStack: {ex.StackTrace}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			});
			Hide();
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error in btnLoad_Click: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	
	private void AddItemsToList(frmItemList frmItemList2)
	{
		try
		{
			if (frmItemList2 == null)
			{
				return;
			}
			
			if (frmItemList2.lstItems == null)
			{
				MessageBox.Show("lstItems control is null. The form may not have initialized properly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			if (frmItemList2.lstItemCol == null)
			{
				MessageBox.Show("lstItemCol collection is null. Items failed to load.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			if (frmItemList2.lstItemCol.Count == 0)
			{
				MessageBox.Show("No items were loaded. This provider may not have any English language updates, or the data format may be incompatible.", "No Items", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			
			frmItemList2.lstItems.Items.AddRange(frmItemList2.lstItemCol.ToArray());
			
			// Organize items into collapsible groups
			frmItemList2.OrganizeIntoGroups();
			
			if (frmItemList2.lstItems.Tag != null)
			{
				foreach (ListViewItem listViewItem in frmItemList2.lstItems.Items)
				{
					if (listViewItem.SubItems.Count > 1 && 
						listViewItem.SubItems[1].Text == frmItemList2.lstItems.Tag.ToString())
					{
						listViewItem.Selected = true;
						listViewItem.EnsureVisible();
						frmItemList2.lstItems.Select();
						break;
					}
				}
			}
			
			frmItemList2.Select();
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error adding items to list: {ex.Message}\n\nStack: {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
		this.btnLoad = new System.Windows.Forms.Button();
		this.lstProviders = new System.Windows.Forms.ListBox();
		base.SuspendLayout();
		this.btnLoad.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.btnLoad.Location = new System.Drawing.Point(147, 276);
		this.btnLoad.Name = "btnLoad";
		this.btnLoad.Size = new System.Drawing.Size(75, 23);
		this.btnLoad.TabIndex = 1;
		this.btnLoad.Text = "Load";
		this.btnLoad.UseVisualStyleBackColor = true;
		this.btnLoad.Click += new System.EventHandler(btnLoad_Click);
		this.lstProviders.FormattingEnabled = true;
		this.lstProviders.Location = new System.Drawing.Point(13, 13);
		this.lstProviders.Name = "lstProviders";
		this.lstProviders.Size = new System.Drawing.Size(209, 251);
		this.lstProviders.TabIndex = 3;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(234, 311);
		base.Controls.Add(this.lstProviders);
		base.Controls.Add(this.btnLoad);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
		base.Name = "frmProvider";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Load a Provider...";
		base.ResumeLayout(false);
	}
}
