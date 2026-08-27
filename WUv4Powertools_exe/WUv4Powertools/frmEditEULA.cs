using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WUv4Powertools;

public class frmEditEULA : Form
{
	private frmItemList frmItemList;
	private frmMain frmMain;
	private Update upd;
	
	private IContainer components;
	private GroupBox grpEULAType;
	private RadioButton radOldType;
	private RadioButton radNewType;
	private RadioButton radCustom;
	private TextBox txtEULACode;
	private Label lblEULACode;
	private Label lblLanguage;
	private ComboBox cmbLanguages;
	private Button btnSave;
	private Button btnCancel;
	private Label lblCurrentEULA;
	private TextBox txtCurrentEULA;
	private CheckBox chkApplyToAll;
	
	public frmEditEULA(frmItemList frmItemList, frmMain frmMain, Update upd)
	{
		this.frmItemList = frmItemList;
		this.frmMain = frmMain;
		this.upd = upd;
		InitializeComponent();
		// Call LoadCurrentEULA after InitializeComponent to ensure all controls are initialized
		LoadCurrentEULA();
	}
	
	private void LoadCurrentEULA()
	{
		// Populate language dropdown by reading ALL languages from itemstringsindex
		// (upd.lan only contains English, so we need to read directly from itemstringsindex)
		cmbLanguages.Items.Clear();
		
		if (frmItemList.l_itemstringsindex != null && !string.IsNullOrWhiteSpace(upd.langscode))
		{
			HashSet<string> languages = new HashSet<string>();
			
			// Search itemstringsindex for all entries matching this update's langscode
			foreach (string line in frmItemList.l_itemstringsindex)
			{
				if (string.IsNullOrEmpty(line))
					continue;
				
				// Format: {langscode}.{lang},{guid}
				// Example: winxp.14B41767-321C-8A52-C524-8D75DCEADA8A.ar,810577
				if (line.Contains(upd.langscode) && line.Contains('.'))
				{
					string[] parts = line.Split('.');
					if (parts.Length >= 2)
					{
						// Extract the language code (between the second dot and the comma)
						string langPart = parts[1].Split(',')[0];
						if (!string.IsNullOrWhiteSpace(langPart))
						{
							languages.Add(langPart);
						}
					}
				}
			}
			
			// Add all found languages to dropdown (sorted alphabetically)
			List<string> sortedLangs = new List<string>(languages);
			sortedLangs.Sort();
			foreach (string lang in sortedLangs)
			{
				cmbLanguages.Items.Add(lang);
			}
		}
		
		// Select the first language by default
		if (cmbLanguages.Items.Count > 0)
		{
			cmbLanguages.SelectedIndex = 0;
		}
		else
		{
			MessageBox.Show("No language data found for this update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
	}
	
	private void cmbLanguages_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (cmbLanguages.SelectedIndex < 0)
			return;
		
		// Get the selected language code directly from the dropdown
		string langCode = cmbLanguages.SelectedItem?.ToString();
		if (string.IsNullOrWhiteSpace(langCode))
			return;
		
		// Find the EULA for this language using the same approach as String Editor
		// We need to find the language GUID first from itemstringsindex
		string langGuid = null;
		string[] l_itemstringsindex = frmItemList.l_itemstringsindex;
		
		foreach (string line in l_itemstringsindex)
		{
			if (string.IsNullOrEmpty(line))
				continue;
				
			// Look for lines that contain our langscode and match the selected language
			// Format: {langscode}.{lang},{guid}
			if (line.Contains(upd.langscode) && line.Split('.').Length > 1 && 
			    line.Split('.')[1].Split(',')[0].ToLower() == langCode.ToLower())
			{
				langGuid = line.Split(',')[1];
				break;
			}
		}
		
		// Now find the itemstring entry using the langGuid
		string eulaFromItemstring = null;
		if (!string.IsNullOrWhiteSpace(langGuid))
		{
			for (int j = 0; j < frmItemList.l_itemstrings.Length; j++)
			{
				string line = frmItemList.l_itemstrings[j];
				if (string.IsNullOrEmpty(line))
					continue;
					
				// Check if this line contains our langGuid
				if (line.Contains(langGuid))
				{
					// Split by @| to get the parts
					string[] parts = line.Split(new string[] { "@|" }, StringSplitOptions.None);
					
					// The EULA URL is at index 4 (based on frmItemList.cs line 150)
					if (parts.Length > 4)
					{
						eulaFromItemstring = parts[4];
						break;
					}
				}
			}
		}
		
		// Use the parsed EULA
		string currentEULA = eulaFromItemstring;
		
		// Handle null or empty EULA URL
		if (string.IsNullOrWhiteSpace(currentEULA))
		{
			txtCurrentEULA.Text = "(No EULA URL set)";
			radNewType.Checked = true;
			txtEULACode.Text = "";
		}
		else
		{
			// Check if it's an old-style relative path
			if (!currentEULA.StartsWith("http://") && !currentEULA.StartsWith("https://"))
			{
				// It's old style - convert to full URL for display
				txtCurrentEULA.Text = $"http://download.windowsupdate.com/msdownload/update/v3/static/RTF/{currentEULA}";
				radOldType.Checked = true;
				string[] pathParts = currentEULA.Split('/');
				if (pathParts.Length > 1)
				{
					txtEULACode.Text = pathParts[1].Replace(".htm", "");
				}
			}
			else if (currentEULA.Contains("support.microsoft.com/?kbid=") || currentEULA.Contains("support.microsoft.com/?id="))
			{
				// New style with kbid - THIS IS THE ONLY "NEW TYPE"
				txtCurrentEULA.Text = currentEULA;
				radNewType.Checked = true;
				if (currentEULA.Contains("kbid="))
				{
					txtEULACode.Text = currentEULA.Substring(currentEULA.IndexOf("kbid=") + 5);
				}
				else if (currentEULA.Contains("id="))
				{
					txtEULACode.Text = currentEULA.Substring(currentEULA.IndexOf("id=") + 3);
				}
			}
			else
			{
				// Everything else is Custom (including fwlink URLs)
				txtCurrentEULA.Text = currentEULA;
				radCustom.Checked = true;
				txtEULACode.Text = currentEULA;
			}
		}
	}
	
	private void btnSave_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(txtEULACode.Text))
		{
			MessageBox.Show("Please enter a EULA code or URL.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		
		if (!chkApplyToAll.Checked && cmbLanguages.SelectedIndex < 0)
		{
			MessageBox.Show("Please select a language or check 'Apply to all languages'.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		
		try
		{
			// Determine which languages to update
			List<string> languagesToUpdate = new List<string>();
			
			if (chkApplyToAll.Checked)
			{
				// Get all languages from the dropdown
				foreach (string lang in cmbLanguages.Items)
				{
					languagesToUpdate.Add(lang);
				}
				
				if (languagesToUpdate.Count == 0)
				{
					MessageBox.Show("No languages found to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				
				// Confirm with user
				DialogResult result = MessageBox.Show(
					$"This will update the EULA for ALL {languagesToUpdate.Count} languages. Are you sure?",
					"Confirm Apply to All",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button2);
				
				if (result != DialogResult.Yes)
				{
					return;
				}
			}
			else
			{
				// Just update the selected language
				string lang = cmbLanguages.SelectedItem?.ToString();
				if (string.IsNullOrWhiteSpace(lang))
				{
					MessageBox.Show("Invalid language selection.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				languagesToUpdate.Add(lang);
			}
			
			int successCount = 0;
			int failCount = 0;
			List<string> failedLanguages = new List<string>();
			
			// Update each language
			foreach (string lang in languagesToUpdate)
			{
				// Build the new EULA URL based on the selected type
				string newEULAForItemstring = "";
				
				if (radOldType.Checked)
				{
					// Old type: store as relative path "{lang}/{code}.htm"
					newEULAForItemstring = $"{lang}/{txtEULACode.Text}.htm";
				}
				else if (radNewType.Checked)
				{
					// New type: store full URL (same for all languages)
					newEULAForItemstring = $"http://support.microsoft.com/?kbid={txtEULACode.Text}";
				}
				else // Custom
				{
					// Custom: store as-is (same for all languages)
					newEULAForItemstring = txtEULACode.Text;
				}
				
				// Find the language GUID from itemstringsindex
				string langGuid = null;
				string[] l_itemstringsindex = frmItemList.l_itemstringsindex;
				
				foreach (string line in l_itemstringsindex)
				{
					if (string.IsNullOrEmpty(line))
						continue;
						
					// Look for lines that contain our langscode and match the selected language
					// Format: {langscode}.{lang},{guid}
					if (line.Contains(upd.langscode) && line.Split('.').Length > 1 && 
					    line.Split('.')[1].Split(',')[0].ToLower() == lang.ToLower())
					{
						langGuid = line.Split(',')[1];
						break;
					}
				}
				
				// Update the itemstring entry using the langGuid
				bool updated = false;
				if (!string.IsNullOrWhiteSpace(langGuid))
				{
					for (int i = 0; i < frmItemList.l_itemstrings.Length; i++)
					{
						string line = frmItemList.l_itemstrings[i];
						if (string.IsNullOrEmpty(line))
							continue;
							
						// Check if this line contains our langGuid
						if (line.Contains(langGuid))
						{
							// Split by @| to get the parts
							string[] parts = line.Split(new string[] { "@|" }, StringSplitOptions.None);
							
							// The EULA URL is at index 4 (based on frmItemList.cs line 150)
							if (parts.Length > 4)
							{
								parts[4] = newEULAForItemstring;
								frmItemList.l_itemstrings[i] = string.Join("@|", parts);
								
								// Update the Update object too (find the matching language)
								// Note: upd.lan may only contain English, so we might not find it
								if (upd.lan != null)
								{
									for (int k = 0; k < upd.lan.Length; k++)
									{
										if (upd.lan[k] != null && upd.lan[k].lang != null && 
										    upd.lan[k].lang.ToLower() == lang.ToLower())
										{
											upd.lan[k].eulaUrl = newEULAForItemstring;
											break;
										}
									}
								}
								
								updated = true;
								break;
							}
						}
					}
				}
				
				if (updated)
				{
					successCount++;
				}
				else
				{
					failCount++;
					failedLanguages.Add(lang);
				}
			}
			
			// Show results
			if (chkApplyToAll.Checked)
			{
				if (failCount == 0)
				{
					MessageBox.Show($"EULA updated successfully for all {successCount} languages!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					this.DialogResult = DialogResult.OK;
					this.Close();
				}
				else
				{
					MessageBox.Show($"EULA updated for {successCount} language(s).\n\nFailed to update {failCount} language(s): {string.Join(", ", failedLanguages)}", 
						"Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					// Don't close on partial success - let user review
				}
			}
			else
			{
				if (successCount > 0)
				{
					MessageBox.Show($"EULA updated successfully for {languagesToUpdate[0]}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					this.DialogResult = DialogResult.OK;
					this.Close();
				}
				else
				{
					MessageBox.Show($"Could not find itemstring for language {languagesToUpdate[0]}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			
			// Refresh the current display if a language is selected and window is still open
			if (!this.IsDisposed && cmbLanguages.SelectedIndex >= 0)
			{
				cmbLanguages_SelectedIndexChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error updating EULA: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	
	private void btnCancel_Click(object sender, EventArgs e)
	{
		this.DialogResult = DialogResult.Cancel;
		this.Close();
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
		this.grpEULAType = new GroupBox();
		this.radOldType = new RadioButton();
		this.radNewType = new RadioButton();
		this.radCustom = new RadioButton();
		this.txtEULACode = new TextBox();
		this.lblEULACode = new Label();
		this.lblLanguage = new Label();
		this.cmbLanguages = new ComboBox();
		this.btnSave = new Button();
		this.btnCancel = new Button();
		this.lblCurrentEULA = new Label();
		this.txtCurrentEULA = new TextBox();
		this.chkApplyToAll = new CheckBox();
		this.grpEULAType.SuspendLayout();
		this.SuspendLayout();
		
		// grpEULAType
		this.grpEULAType.Controls.Add(this.radOldType);
		this.grpEULAType.Controls.Add(this.radNewType);
		this.grpEULAType.Controls.Add(this.radCustom);
		this.grpEULAType.Location = new Point(12, 12);
		this.grpEULAType.Name = "grpEULAType";
		this.grpEULAType.Size = new Size(660, 100);
		this.grpEULAType.TabIndex = 0;
		this.grpEULAType.TabStop = false;
		this.grpEULAType.Text = "EULA Type";
		
		// radOldType
		this.radOldType.AutoSize = true;
		this.radOldType.Location = new Point(15, 25);
		this.radOldType.Name = "radOldType";
		this.radOldType.Size = new Size(600, 17);
		this.radOldType.TabIndex = 0;
		this.radOldType.Text = "Old Type (e.g., http://download.windowsupdate.com/msdownload/update/v3/static/RTF/{lang}/{code}.htm)";
		this.radOldType.UseVisualStyleBackColor = true;
		
		// radNewType
		this.radNewType.AutoSize = true;
		this.radNewType.Checked = true;
		this.radNewType.Location = new Point(15, 48);
		this.radNewType.Name = "radNewType";
		this.radNewType.Size = new Size(400, 17);
		this.radNewType.TabIndex = 1;
		this.radNewType.TabStop = true;
		this.radNewType.Text = "New Type (e.g., http://support.microsoft.com/?kbid={code})";
		this.radNewType.UseVisualStyleBackColor = true;
		
		// radCustom
		this.radCustom.AutoSize = true;
		this.radCustom.Location = new Point(15, 71);
		this.radCustom.Name = "radCustom";
		this.radCustom.Size = new Size(250, 17);
		this.radCustom.TabIndex = 2;
		this.radCustom.Text = "Custom URL (enter full URL below)";
		this.radCustom.UseVisualStyleBackColor = true;
		
		// lblEULACode
		this.lblEULACode.AutoSize = true;
		this.lblEULACode.Location = new Point(12, 120);
		this.lblEULACode.Name = "lblEULACode";
		this.lblEULACode.Size = new Size(120, 13);
		this.lblEULACode.TabIndex = 1;
		this.lblEULACode.Text = "EULA Code/URL:";
		
		// txtEULACode
		this.txtEULACode.Location = new Point(12, 136);
		this.txtEULACode.Name = "txtEULACode";
		this.txtEULACode.Size = new Size(660, 20);
		this.txtEULACode.TabIndex = 2;
		
		// lblCurrentEULA
		this.lblCurrentEULA.AutoSize = true;
		this.lblCurrentEULA.Location = new Point(12, 165);
		this.lblCurrentEULA.Name = "lblCurrentEULA";
		this.lblCurrentEULA.Size = new Size(100, 13);
		this.lblCurrentEULA.TabIndex = 3;
		this.lblCurrentEULA.Text = "Current EULA URL:";
		
		// txtCurrentEULA
		this.txtCurrentEULA.Location = new Point(12, 181);
		this.txtCurrentEULA.Name = "txtCurrentEULA";
		this.txtCurrentEULA.ReadOnly = true;
		this.txtCurrentEULA.Size = new Size(660, 20);
		this.txtCurrentEULA.TabIndex = 4;
		this.txtCurrentEULA.BackColor = SystemColors.Control;
		
		// lblLanguage
		this.lblLanguage.AutoSize = true;
		this.lblLanguage.Location = new Point(12, 210);
		this.lblLanguage.Name = "lblLanguage";
		this.lblLanguage.Size = new Size(100, 13);
		this.lblLanguage.TabIndex = 5;
		this.lblLanguage.Text = "Select Language:";
		
		// cmbLanguages
		this.cmbLanguages.DropDownStyle = ComboBoxStyle.DropDownList;
		this.cmbLanguages.FormattingEnabled = true;
		this.cmbLanguages.Location = new Point(12, 226);
		this.cmbLanguages.Name = "cmbLanguages";
		this.cmbLanguages.Size = new Size(660, 21);
		this.cmbLanguages.TabIndex = 6;
		this.cmbLanguages.SelectedIndexChanged += new EventHandler(this.cmbLanguages_SelectedIndexChanged);
		
		// chkApplyToAll
		this.chkApplyToAll.AutoSize = true;
		this.chkApplyToAll.Location = new Point(12, 253);
		this.chkApplyToAll.Name = "chkApplyToAll";
		this.chkApplyToAll.Size = new Size(300, 17);
		this.chkApplyToAll.TabIndex = 7;
		this.chkApplyToAll.Text = "Apply to all languages (will use the same EULA for all)";
		this.chkApplyToAll.UseVisualStyleBackColor = true;
		
		// btnSave
		this.btnSave.Location = new Point(516, 280);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new Size(75, 23);
		this.btnSave.TabIndex = 8;
		this.btnSave.Text = "Save";
		this.btnSave.UseVisualStyleBackColor = true;
		this.btnSave.Click += new EventHandler(this.btnSave_Click);
		
		// btnCancel
		this.btnCancel.Location = new Point(597, 280);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new Size(75, 23);
		this.btnCancel.TabIndex = 9;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.UseVisualStyleBackColor = true;
		this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
		
		// frmEditEULA
		this.AutoScaleDimensions = new SizeF(6F, 13F);
		this.AutoScaleMode = AutoScaleMode.Font;
		this.ClientSize = new Size(684, 315);
		this.Controls.Add(this.chkApplyToAll);
		this.Controls.Add(this.btnCancel);
		this.Controls.Add(this.btnSave);
		this.Controls.Add(this.cmbLanguages);
		this.Controls.Add(this.lblLanguage);
		this.Controls.Add(this.txtCurrentEULA);
		this.Controls.Add(this.lblCurrentEULA);
		this.Controls.Add(this.txtEULACode);
		this.Controls.Add(this.lblEULACode);
		this.Controls.Add(this.grpEULAType);
		this.FormBorderStyle = FormBorderStyle.FixedDialog;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "frmEditEULA";
		this.ShowIcon = false;
		this.ShowInTaskbar = false;
		this.StartPosition = FormStartPosition.CenterParent;
		this.Text = "Edit EULA Links";
		this.grpEULAType.ResumeLayout(false);
		this.grpEULAType.PerformLayout();
		this.ResumeLayout(false);
		this.PerformLayout();
	}
}
