using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using AdvancedWizardControl.Enums;
using AdvancedWizardControl.Wizard;
using AdvancedWizardControl.WizardPages;
using WUv4Powertools.Properties;

namespace WUv4Powertools;

public class frmAddUpdate : Form
{
	private frmItemList frmItemList;

	private frmMain frmMain;

	private bool isWindows;

	private bool is9x;

	private List<string> codeIndex = new List<string>();

	private Guid fileGuid = Guid.NewGuid();

	private Guid langGuid = Guid.NewGuid();

	private string[] l_md_langs;

	private string[] l_md_filename;

	private string[] l_md_dlinks;

	private string[] l_md_guids;

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

	private AdvancedWizardPage advancedWizardPage1;

	private TextBox txtDescription;

	private TextBox txtTitle;

	private Label lblTitle;

	private Label lblDescription;

	private AdvancedWizardPage advancedWizardPage2;

	private Label lblLanguage;

	private CheckBox chkExclusive;

	private CheckBox chkCritical;

	private Label lblExtras;

	private Label lblGroup;

	private ComboBox cmbGroup;

	private Label lblHelp0;

	private CheckBox chkEULARequired;

	private AdvancedWizardPage advancedWizardPage3;

	private Label lblDownload;

	private TextBox txtDLink;

	private TextBox txtFileName;

	private Label lblFileName;

	private TextBox txtArguments;

	private Label lblArguments;

	private TextBox txtDetection;

	private Label lblDetection;

	private Label lblServicePack;

	private ComboBox cmbMaxSP;

	private ComboBox cmbMinSP;

	private AdvancedWizardPage advancedWizardPage4;

	private Label lblEULAType;

	private RadioButton radNewEULA;

	private RadioButton radOldEULA;

	private TextBox txtEULA;

	private Label lblEULACode;

	private ComboBox cmbOS;

	private TextBox txtUpdCode;

	private Label lblFileCode;

	private CheckBox chkRebootReq;

	private ComboBox cmbLang;

	private CheckBox chkMLFile;

	private DateTimePicker cmbDate;
	
	// Time controls for precise timestamp
	private NumericUpDown numHours;
	private NumericUpDown numMinutes;
	private NumericUpDown numSeconds;
	private NumericUpDown numMilliseconds;
	private Label lblTime;

	private RadioButton radCustomEULA;

	private GroupBox gbDLinks;

	private Button btnSelectFile;

	private RadioButton chkMultilanguage;

	private RadioButton chkSingleLang;

	private Label lblMFInfo;

	private OpenFileDialog openFileDialog1;

	private Label lblHelp1;

	// Feature 3: Update type and command editing controls
	private ComboBox cmbInstallerType;
	private Label lblInstallerType;
	private ComboBox cmbCommandType;
	private Label lblCommandType;
	private TextBox txtCommandFile;
	private Label lblCommandFile;
	
	// Driver update support - removed the selection page, auto-detect instead
	private bool isDriverUpdate = false;
	
	// Internet Explorer update support - OS selection checkboxes
	private bool isIEUpdate = false;
	private CheckBox chkIE_Win98;
	private CheckBox chkIE_WinMe;
	private CheckBox chkIE_Win2000;
	private CheckBox chkIE_WinXP;
	private CheckBox chkIE_WinServer2003;
	private Label lblIEOSSelection;
	private ComboBox cmbIEVersion;

	public frmAddUpdate(frmItemList frmItemList, frmMain frmMain)
	{
		this.frmMain = frmMain;
		this.frmItemList = frmItemList;
		InitializeComponent();
	}

	public void autoFill()
	{
		txtTitle.Text = "French Menus and Dialogs for Internet Explorer 6 SP1";
		txtDescription.Text = "This component allows you to display the Menus and Dialogs of Internet Explorer in French.";
		txtEULA.Text = "5737";
		txtFileName.Text = "ieuifr.EXE";
		txtDLink.Text = "http://download.windowsupdaterestored.com/updates/CabPool/ieuifr_B79C6D4192031BC831636CAD3AE22233AE948B43.EXE";
		txtUpdCode.Text = "PlugUIFR_W98_IE60_5737";
		chkRebootReq.Checked = true;
		chkExclusive.Checked = true;
		cmbGroup.Text = "90945";
		cmbDate.Text = "2003/1/1";
		txtDetection.Text = "<detection><installed><expression><regKeyExists><key>HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Active Setup\\Installed Components\\{AF202806-350E-11d2-B167-0060B03C1CA5}</key></regKeyExists></expression></installed><excluded><expression><regKeyVersion versionStatus=\"HIGHER\"><key>HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Internet Explorer</key><entry>Version</entry><version>6,00,0000,0000</version></regKeyVersion></expression></excluded></detection>";
	}

	private void advancedWizard1_Cancel(object sender, EventArgs e)
	{
		Dispose();
	}

	private void frmAddUpdate_Load(object sender, EventArgs e)
	{
		cmbLang.Text = "en";
		
		// Feature 3: Set default installer and command types
		if (cmbInstallerType != null)
		{
			cmbInstallerType.SelectedIndex = 0; // SOFTWARE
		}
		if (cmbCommandType != null)
		{
			cmbCommandType.SelectedIndex = 0; // EXE
		}
		
		// Auto-detect if we're working with a driver provider
		if (frmItemList != null && frmItemList.isDriverProvider)
		{
			isDriverUpdate = true;
			
			// Remove EULA page (page 4) for driver updates - drivers don't have EULAs
			if (this.advancedWizard1.WizardPages.Contains(this.advancedWizardPage4))
			{
				this.advancedWizard1.WizardPages.Remove(this.advancedWizardPage4);
			}
			
			// Hide multilanguage file checkbox for drivers
			if (chkMLFile != null)
			{
				chkMLFile.Visible = false;
			}
			
			// Hide multilanguage download options for drivers
			if (gbDLinks != null)
			{
				chkMultilanguage.Visible = false;
				chkSingleLang.Visible = false;
				btnSelectFile.Visible = false;
				lblMFInfo.Visible = false;
			}
			
			// Adjust UI for driver updates
			lblTitle.Text = "Driver Name:";
			lblDetection.Text = "Hardware IDs:";
			lblArguments.Text = "INF Command:";
			lblFileName.Text = "CAB File Name:";
			lblDownload.Text = "CAB Download Link:";
			
			// Hide description field for drivers (they don't have descriptions)
			lblDescription.Visible = false;
			txtDescription.Visible = false;
			
			// Hide OS and Service Pack dropdowns for drivers (not applicable)
			lblServicePack.Visible = false;
			cmbOS.Visible = false;
			cmbMinSP.Visible = false;
			cmbMaxSP.Visible = false;
			
			// For drivers, set group to 90700 (Hardware/Drivers category) and make it read-only
			cmbGroup.Text = "90700";
			lblGroup.Text = "Category (Hardware):";
			cmbGroup.Enabled = false; // Drivers always use 90700, no user selection needed
			
			// Change installer type to CDM (driver) by default
			if (cmbInstallerType != null && cmbInstallerType.Items.Count > 1)
			{
				cmbInstallerType.SelectedIndex = 1; // CDM for drivers
			}
			
			// For drivers, use ADVANCED_INF command type
			if (cmbCommandType != null && cmbCommandType.Items.Count > 2)
			{
				cmbCommandType.SelectedIndex = 2; // ADVANCED_INF
			}
		}
		else
		{
			isDriverUpdate = false;
			
			// Check if this is an Internet Explorer update
			if (frmItemList != null && (frmItemList.provider == "ie50x" || frmItemList.provider == "ie55x" || frmItemList.provider == "ie60x"))
			{
				isIEUpdate = true;
				
				// Hide OS and SP selection since IE updates use checkboxes instead
				lblServicePack.Visible = false;
				cmbOS.Visible = false;
				cmbMinSP.Visible = false;
				cmbMaxSP.Visible = false;
				
				// Initialize IE OS selection checkboxes
				InitializeIEOSControls();
			}
			else
			{
				isIEUpdate = false;
				
				// Show OS and Service Pack dropdowns for regular Windows updates
				lblServicePack.Visible = true;
				cmbOS.Visible = true;
				cmbMinSP.Visible = true;
				cmbMaxSP.Visible = true;
			}
			
			// Ensure EULA page is present for Windows updates
			if (!this.advancedWizard1.WizardPages.Contains(this.advancedWizardPage4))
			{
				this.advancedWizard1.WizardPages.Add(this.advancedWizardPage4);
			}
			
			// Show multilanguage file checkbox for Windows updates
			if (chkMLFile != null)
			{
				chkMLFile.Visible = true;
			}
			
			// Show multilanguage download options for Windows updates
			if (gbDLinks != null)
			{
				chkMultilanguage.Visible = true;
				chkSingleLang.Visible = true;
				btnSelectFile.Visible = true;
				lblMFInfo.Visible = true;
			}
			
			// Standard labels for Windows updates
			lblTitle.Text = "Title:";
			lblDescription.Text = "Description:";
			lblDetection.Text = "Detection:";
			lblArguments.Text = "Arguments:";
			lblFileName.Text = "File Name:";
			lblDownload.Text = "Download Link:";
			
			// Show description field for Windows updates
			lblDescription.Visible = true;
			txtDescription.Visible = true;
			
			// For OS updates, enable group selection and set default
			lblGroup.Text = "Group:";
			cmbGroup.Enabled = true;
			cmbGroup.SelectedIndex = 0;
		}
		
		osDetect(firstTime: true);
		autoFill();
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
				cmbOS.Enabled = false;
				cmbMinSP.Enabled = false;
				cmbMaxSP.Enabled = false;
				cmbOS.SelectedIndex = 0;
				break;
			case "winme":
				isWindows = true;
				is9x = true;
				cmbOS.Enabled = false;
				cmbMinSP.Enabled = false;
				cmbMaxSP.Enabled = false;
				cmbOS.SelectedIndex = 1;
				break;
			case "win2k":
				isWindows = true;
				is9x = false;
				cmbOS.Enabled = false;
				cmbMinSP.Enabled = true;
				cmbMaxSP.Enabled = true;
				cmbOS.SelectedIndex = 2;
				PopulateServicePacks(2195); // Windows 2000 build number
				break;
			case "winxp":
				isWindows = true;
				is9x = false;
				cmbOS.Enabled = false;
				cmbMinSP.Enabled = true;
				cmbMaxSP.Enabled = true;
				cmbOS.SelectedIndex = 3;
				PopulateServicePacks(2600); // Windows XP build number
				break;
			case "netserver":
				isWindows = true;
				is9x = false;
				cmbOS.Enabled = false;
				cmbMinSP.Enabled = true;
				cmbMaxSP.Enabled = true;
				cmbOS.SelectedIndex = 4;
				PopulateServicePacks(3790); // Windows Server 2003 build number
				break;
			case "ie50x":
				isWindows = true;
				is9x = false;
				isIEUpdate = true;
				cmbOS.Enabled = false;
				cmbMinSP.Enabled = false;
				cmbMaxSP.Enabled = false;
				break;
			case "ie55x":
				isWindows = true;
				is9x = false;
				isIEUpdate = true;
				cmbOS.Enabled = false;
				cmbMinSP.Enabled = false;
				cmbMaxSP.Enabled = false;
				break;
			case "ie60x":
				isWindows = true;
				is9x = false;
				isIEUpdate = true;
				cmbOS.Enabled = false;
				cmbMinSP.Enabled = false;
				cmbMaxSP.Enabled = false;
				break;
			default:
				isWindows = false;
				break;
			}
		}
		else
		{
			switch (cmbOS.SelectedIndex)
			{
			case 0: // Windows 98
				cmbMinSP.Enabled = false;
				cmbMaxSP.Enabled = false;
				cmbMinSP.Items.Clear();
				cmbMaxSP.Items.Clear();
				break;
			case 1: // Windows ME
				cmbMinSP.Enabled = false;
				cmbMaxSP.Enabled = false;
				cmbMinSP.Items.Clear();
				cmbMaxSP.Items.Clear();
				break;
			case 2: // Windows 2000
				cmbMinSP.Enabled = true;
				cmbMaxSP.Enabled = true;
				PopulateServicePacks(2195);
				break;
			case 3: // Windows XP
				cmbMinSP.Enabled = true;
				cmbMaxSP.Enabled = true;
				PopulateServicePacks(2600);
				break;
			case 4: // Windows Server 2003
				cmbMinSP.Enabled = true;
				cmbMaxSP.Enabled = true;
				PopulateServicePacks(3790);
				break;
			}
		}
	}

	private void PopulateServicePacks(int buildNumber)
	{
		cmbMinSP.Items.Clear();
		cmbMaxSP.Items.Clear();
		
		// Determine max SP based on build number
		int maxSP = 0;
		switch (buildNumber)
		{
			case 2195: // Windows 2000
				maxSP = 4;
				break;
			case 2600: // Windows XP
				maxSP = 3;
				break;
			case 3790: // Windows Server 2003
				maxSP = 2;
				break;
		}
		
		// Add service pack options (0 = RTM, 1-4 = SP1-SP4)
		for (int i = 0; i <= maxSP; i++)
		{
			cmbMinSP.Items.Add(i.ToString());
			cmbMaxSP.Items.Add(i.ToString());
		}
		
		// Set default selections
		if (cmbMinSP.Items.Count > 0)
			cmbMinSP.SelectedIndex = 0; // Default to RTM
		if (cmbMaxSP.Items.Count > 0)
			cmbMaxSP.SelectedIndex = cmbMaxSP.Items.Count - 1; // Default to highest SP
	}
	
	private void InitializeIEOSControls()
	{
		// No resize here. The form is built large enough for every flow, and resizing only when the
		// IE path ran was what left the ordinary add flow clipped at the smaller height.
		
		// Hide the OS and SP controls since they don't apply to IE updates
		if (lblServicePack != null) lblServicePack.Visible = false;
		if (cmbOS != null) cmbOS.Visible = false;
		if (cmbMinSP != null) cmbMinSP.Visible = false;
		if (cmbMaxSP != null) cmbMaxSP.Visible = false;
		
		// Create label for IE OS selection
		lblIEOSSelection = new Label();
		lblIEOSSelection.AutoSize = true;
		lblIEOSSelection.Location = new System.Drawing.Point(20, 230);
		lblIEOSSelection.Name = "lblIEOSSelection";
		lblIEOSSelection.Size = new System.Drawing.Size(200, 13);
		lblIEOSSelection.TabIndex = 100;
		lblIEOSSelection.Text = "Select Target Operating Systems:";
		this.advancedWizardPage1.Controls.Add(lblIEOSSelection);
		
		// Determine which IE version we're working with and create appropriate OS checkboxes
		int osStartY = 250;  // Start OS checkboxes at Y=250
		int checkboxWidth = 90;  // Width for OS names
		int startX = 25;
		
		// IE versions have different OS support:
		// IE 5.0x: Windows 98, Windows 2000
		// IE 5.5x: Windows 98, Windows ME, Windows 2000
		// IE 6.0x: Windows 98, Windows ME, Windows 2000, Windows XP, Windows Server 2003
		
		List<string> availableOSes = new List<string>();
		
		if (frmItemList.provider == "ie50x")
		{
			// IE 5.0x: Windows 98 and Windows 2000
			availableOSes.AddRange(new[] { "Windows 98", "Windows 2000" });
		}
		else if (frmItemList.provider == "ie55x")
		{
			// IE 5.5x: Windows 98, Windows ME, and Windows 2000
			availableOSes.AddRange(new[] { "Windows 98", "Windows ME", "Windows 2000" });
		}
		else if (frmItemList.provider == "ie60x")
		{
			// IE 6.0x: All Windows versions
			availableOSes.AddRange(new[] { "Windows 98", "Windows ME", "Windows 2000", "Windows XP", "Windows Server 2003" });
		}
		
		// Create a checkbox for each available OS
		for (int i = 0; i < availableOSes.Count; i++)
		{
			CheckBox chk = new CheckBox();
			chk.AutoSize = true;
			chk.Location = new System.Drawing.Point(startX + (i * checkboxWidth), osStartY);
			chk.Name = "chkIE_" + availableOSes[i].Replace(" ", "");
			chk.Size = new System.Drawing.Size(checkboxWidth, 17);
			chk.TabIndex = 101 + i;
			chk.Text = availableOSes[i];
			chk.UseVisualStyleBackColor = true;
			chk.Checked = true; // Default to all checked
			chk.Tag = availableOSes[i]; // Store the OS identifier
			this.advancedWizardPage1.Controls.Add(chk);
			
			// Store reference based on OS for later use
			switch (availableOSes[i])
			{
				case "Windows 98":
					chkIE_Win98 = chk;
					break;
				case "Windows ME":
					chkIE_WinMe = chk;
					break;
				case "Windows 2000":
					chkIE_Win2000 = chk;
					// Add event handler for Win2000 checkbox in ie50x to show/hide SP selection
					if (frmItemList.provider == "ie50x")
					{
						chk.CheckedChanged += ChkIE_Win2000_CheckedChanged_ie50x;
					}
					break;
				case "Windows XP":
					chkIE_WinXP = chk;
					break;
				case "Windows Server 2003":
					chkIE_WinServer2003 = chk;
					break;
			}
		}
		
		// For IE 5.0x, create Win2K SP selection checkboxes
		if (frmItemList.provider == "ie50x")
		{
			// Create label for Win2K SP selection
			Label lblIEW2KSPSelection = new Label();
			lblIEW2KSPSelection.AutoSize = true;
			lblIEW2KSPSelection.Location = new System.Drawing.Point(20, 285);
			lblIEW2KSPSelection.Name = "lblIEW2KSPSelection";
			lblIEW2KSPSelection.Size = new System.Drawing.Size(200, 13);
			lblIEW2KSPSelection.TabIndex = 120;
			lblIEW2KSPSelection.Text = "Windows 2000 Service Packs:";
			this.advancedWizardPage1.Controls.Add(lblIEW2KSPSelection);
			
			// Create SP checkboxes for Win2K (SP0-SP4)
			List<string> availableSPs = new List<string> { "SP0", "SP1", "SP2", "SP3", "SP4" };
			
			int spStartY = 305;
			int spCheckboxWidth = 60;
			
			// Create a checkbox for each available SP
			for (int i = 0; i < availableSPs.Count; i++)
			{
				CheckBox chk = new CheckBox();
				chk.AutoSize = true;
				chk.Location = new System.Drawing.Point(startX + (i * spCheckboxWidth), spStartY);
				chk.Name = "chkIESP_W2K_" + availableSPs[i];
				chk.Size = new System.Drawing.Size(spCheckboxWidth, 17);
				chk.TabIndex = 121 + i;
				chk.Text = availableSPs[i];
				chk.UseVisualStyleBackColor = true;
				chk.Checked = true; // Default to all checked
				chk.Tag = availableSPs[i]; // Store the SP identifier
				chk.Visible = chkIE_Win2000.Checked; // Only visible if Win2K is selected
				this.advancedWizardPage1.Controls.Add(chk);
			}
			
			// Add a helpful label explaining the IE 5.0x OS/SP system
			Label lblIEHelp = new Label();
			lblIEHelp.Location = new System.Drawing.Point(25, spStartY + 25);
			lblIEHelp.Name = "lblIEHelp";
			lblIEHelp.Size = new System.Drawing.Size(400, 50);
			lblIEHelp.TabIndex = 130;
			lblIEHelp.Text = "Select which operating systems this update applies to.\n" +
				"For Windows 2000, select specific service pack versions.\n" +
				"Windows 98 does not have service packs.";
			this.advancedWizardPage1.Controls.Add(lblIEHelp);
		}
		else
		{
			// For IE 5.5x and IE 6.0x, add explanation that no SP selection is needed
			Label lblIEHelp = new Label();
			lblIEHelp.Location = new System.Drawing.Point(25, 285);
			lblIEHelp.Name = "lblIEHelp";
			lblIEHelp.Size = new System.Drawing.Size(400, 50);
			lblIEHelp.TabIndex = 130;
			
			if (frmItemList.provider == "ie55x")
			{
				lblIEHelp.Text = "Select which operating systems this update applies to.\n" +
					"The update will be added to ALL service pack versions of Windows 2000 (SP0-SP4).\n" +
					"Windows 98 and Windows ME do not have service packs.";
			}
			else if (frmItemList.provider == "ie60x")
			{
				lblIEHelp.Text = "Select which operating systems this update applies to.\n" +
					"The update will be added to all major OS versions (no service pack differentiation).\n" +
					"Windows 98 and Windows ME do not have service packs.";
			}
			
			this.advancedWizardPage1.Controls.Add(lblIEHelp);
		}
	}
	
	// Event handler for Win2000 checkbox in ie50x to show/hide SP checkboxes
	private void ChkIE_Win2000_CheckedChanged_ie50x(object sender, EventArgs e)
	{
		if (frmItemList.provider == "ie50x")
		{
			bool isChecked = ((CheckBox)sender).Checked;
			
			// Show/hide Win2K SP checkboxes
			foreach (Control ctrl in this.advancedWizardPage1.Controls)
			{
				if (ctrl is CheckBox chk && chk.Name.StartsWith("chkIESP_W2K_"))
				{
					chk.Visible = isChecked;
				}
			}
		}
	}

	
	private void ChkIE_Win2000_CheckedChanged(object sender, EventArgs e)
	{
		// This method is no longer needed but keeping it for compatibility
	}

	private List<string> GetSelectedIEOSes()
	{
		List<string> selectedOSes = new List<string>();
		
		// Check OS checkboxes (they have names starting with "chkIE_Windows")
		foreach (Control ctrl in this.advancedWizardPage1.Controls)
		{
			if (ctrl is CheckBox chk && chk.Name.StartsWith("chkIE_Windows") && chk.Tag is string osName)
			{
				if (chk.Checked)
				{
					selectedOSes.Add(osName);
				}
			}
		}
		
		// If no specific OSes selected, return "ALL"
		if (selectedOSes.Count == 0)
		{
			selectedOSes.Add("ALL");
		}
		
		return selectedOSes;
	}
	
	private List<string> GetSelectedIESPs()
	{
		List<string> selectedSPs = new List<string>();
		
		// For IE50x, get selected Win2K SPs
		if (frmItemList.provider == "ie50x")
		{
			foreach (Control ctrl in this.advancedWizardPage1.Controls)
			{
				if (ctrl is CheckBox chk && chk.Name.StartsWith("chkIESP_W2K_") && chk.Tag is string spName)
				{
					if (chk.Checked)
					{
						selectedSPs.Add(spName);
					}
				}
			}
			
			// If no SPs selected, return empty list
			if (selectedSPs.Count == 0)
			{
				return selectedSPs;
			}
		}
		else
		{
			// For IE55x and IE60x, return "ALL" to indicate all SPs should be handled
			selectedSPs.Add("ALL");
		}
		
		return selectedSPs;
	}

	
	private void cmbOS_SelectedIndexChanged(object sender, EventArgs e)
	{
		osDetect(firstTime: false);
	}

	// Feature 3: Show/hide command file based on command type
	private void CmbCommandType_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (cmbCommandType == null || txtCommandFile == null || lblCommandFile == null) return;
		
		bool needsCommandFile = cmbCommandType.SelectedItem.ToString() != "EXE";
		lblCommandFile.Visible = needsCommandFile;
		txtCommandFile.Visible = needsCommandFile;
		
		if (needsCommandFile && cmbCommandType.SelectedItem.ToString() == "ADVANCED_INF")
		{
			txtCommandFile.Text = "setupapi.dll,InstallHinfSection DefaultInstall 128 ";
			lblCommandFile.Text = "INF Command:";
		}
		else if (needsCommandFile && cmbCommandType.SelectedItem.ToString() == "CABFILE")
		{
			txtCommandFile.Text = "";
			lblCommandFile.Text = "INF File in CAB:";
		}
	}

	private async void advancedWizard1_Finish(object sender, EventArgs e)
	{
		// Capture the dictionaries before changing them, so this can be taken back with Undo.
		frmItemList.PushUndoState();
		// Comprehensive input validation
		if (string.IsNullOrWhiteSpace(txtUpdCode.Text))
		{
			MessageBox.Show("Please enter an Update Code.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		
		if (string.IsNullOrWhiteSpace(txtDetection.Text))
		{
			MessageBox.Show("Please enter Detection code.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		
		if (string.IsNullOrWhiteSpace(txtEULA.Text))
		{
			MessageBox.Show("Please enter EULA information.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		
		if (string.IsNullOrWhiteSpace(txtTitle.Text))
		{
			MessageBox.Show("Please enter a Title.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		
		if (string.IsNullOrWhiteSpace(txtDescription.Text))
		{
			MessageBox.Show("Please enter a Description.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		
		// Validate download link requirements
		bool hasStandardLink = !string.IsNullOrWhiteSpace(txtDLink.Text) && !string.IsNullOrWhiteSpace(txtFileName.Text);
		bool hasMultiLanguageData = chkMultilanguage.Checked && 
									l_md_langs != null && 
									l_md_dlinks != null && 
									l_md_filename != null &&
									l_md_langs.Length > 0 &&
									l_md_dlinks.Length > 0 &&
									l_md_filename.Length > 0;
		
		if (!hasStandardLink && !hasMultiLanguageData)
		{
			MessageBox.Show("Please provide either a download link and filename, or multilanguage data.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		
		// Validate critical object references
		if (frmItemList == null)
		{
			MessageBox.Show("Internal error: frmItemList is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		
		if (frmMain == null)
		{
			MessageBox.Show("Internal error: frmMain is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		
		string _cmbGroup = cmbGroup.Text;
		string _cmbLang = cmbLang.Text;
		string _cmbMinSP = cmbMinSP.Text ?? "";
		string _cmbMaxSP = cmbMaxSP.Text ?? "";
		
		// Capture all UI values before Task.Run to avoid cross-thread access
		string _txtUpdCode = txtUpdCode.Text;
		string _txtDetection = txtDetection.Text;
		string _txtTitle = txtTitle.Text;
		string _txtDescription = txtDescription.Text;
		string _txtEULA = txtEULA.Text;
		string _txtDLink = txtDLink.Text;
		string _txtFileName = txtFileName.Text;
		string _txtArguments = txtArguments.Text;
		bool _chkExclusive = chkExclusive.Checked;
		bool _chkRebootReq = chkRebootReq.Checked;
		bool _chkCritical = chkCritical.Checked;
		bool _chkMultilanguage = chkMultilanguage.Checked;
		bool _chkMLFile = chkMLFile.Checked;
		bool _radOldEULA = radOldEULA.Checked;
		bool _radNewEULA = radNewEULA.Checked;
		bool _radCustomEULA = radCustomEULA.Checked;
		DateTime _cmbDateValue = cmbDate.Value;
		int _hours = (int)numHours.Value;
		int _minutes = (int)numMinutes.Value;
		int _seconds = (int)numSeconds.Value;
		int _milliseconds = (int)numMilliseconds.Value;
		
		// Capture IE update specific values
		bool _isIEUpdate = isIEUpdate;
		List<string> _selectedIEOSes = isIEUpdate ? GetSelectedIEOSes() : new List<string>();
		List<string> _selectedIESPs = isIEUpdate ? GetSelectedIESPs() : new List<string>();
		
		// Combine date and time
		DateTime _fullDateTime = new DateTime(
			_cmbDateValue.Year,
			_cmbDateValue.Month,
			_cmbDateValue.Day,
			_hours,
			_minutes,
			_seconds,
			_milliseconds
		);
		
		// Feature 3: Capture installer and command type values
		string _installerType = (cmbInstallerType != null && cmbInstallerType.SelectedItem != null) 
			? cmbInstallerType.SelectedItem.ToString() 
			: "SOFTWARE";
		string _commandType = (cmbCommandType != null && cmbCommandType.SelectedItem != null) 
			? cmbCommandType.SelectedItem.ToString() 
			: "EXE";
		string _commandFile = txtCommandFile != null ? txtCommandFile.Text : "";
		
		try
		{
			await Task.Run(delegate
			{
				// Initialize arrays if they're null
				if (frmItemList.l_items == null)
				{
					frmItemList.l_items = new string[0];
				}
				if (frmItemList.l_itemsindex == null)
				{
					frmItemList.l_itemsindex = new string[0];
				}
				if (frmItemList.l_itemstrings == null)
				{
					frmItemList.l_itemstrings = new string[0];
				}
				if (frmItemList.l_itemstringsindex == null)
				{
					frmItemList.l_itemstringsindex = new string[0];
				}
					// Collect all additional entries to add after the main loop
					// Collect all additional IE entries to add after the main loop completes
					List<string> allAdditionalEntries = new List<string>();
					
					for (int i = 0; i < frmItemList.l_product2items.Length; i++)
				{
					string text = frmItemList.l_product2items[i];
					if (string.IsNullOrEmpty(text))
					{
						continue; // Skip null or empty entries
					}
					
					string text2 = "";
					if (isWindows)
					{
						if (string.IsNullOrEmpty(frmItemList.provider))
						{
							throw new Exception("Provider is not set.");
						}
						
						string searchPattern = frmItemList.provider + ".";
						if (text.Contains(searchPattern))
						{
							string replaced = text.Replace(searchPattern, "");
							string[] parts = replaced.Split(',');
							if (parts.Length > 0)
							{
								text2 = parts[0];
							}
						}
					}
					if (is9x || (!is9x && _cmbMinSP.Length == 0 && _cmbMaxSP.Length == 0))
					{
						// For IE updates, we need special handling based on selected operating systems and service packs
						if (_isIEUpdate)
						{
							// Get which operating systems and SPs were selected
							List<string> selectedOSes = _selectedIEOSes;
							List<string> selectedSPs = _selectedIESPs;
							
							// Only process Windows 2000 entries from the dictionary
							// We'll generate Win98/ME/XP/2003 entries from these base entries
							if (text2.Contains("ver_platform_win32_nt.5.0"))
							{
								// This is a Windows 2000 entry - process based on IE version
								List<string> additionalEntries = new List<string>();
								bool addedWin2000 = false;
								
								// =================================================================
								// IE 5.0x PROCESSING
								// =================================================================
								if (frmItemList.provider == "ie50x")
								{
									// For IE 5.0x:
									// - Windows 98: Add with 5 dots (.....") - no build number, no SP
									// - Windows 2000: Add with selected SP versions only (SP0-SP4)
									
									// Process Windows 98 if selected
									if (selectedOSes.Contains("Windows 98"))
									{
										// Windows 98: ver_platform_win32_windows.4.10.x86.LANG.....
										string win98Text2 = text2.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_windows.4.10")
											.Replace("...2195..", ".....");
										
										// Only add if transformation was successful
										if (!win98Text2.Contains("2195"))
										{
											// Transform the complete entry (key + all items)
											string win98FullEntry = text.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_windows.4.10")
												.Replace("...2195..", ".....");
											// Append the new update item to the transformed entry
											additionalEntries.Add($"{win98FullEntry},{win98Text2}.com_microsoft.{_txtUpdCode}.");
											if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
											{
												codeIndex.Add($"{frmItemList.provider}.{win98Text2}.com_microsoft.{_txtUpdCode}.");
											}
										}
									}
									
									// Process Windows 2000 with selected SPs
									if (selectedOSes.Contains("Windows 2000") && selectedSPs.Count > 0)
									{
										// For each selected SP, create an entry
										foreach (string sp in selectedSPs)
										{
											string spPattern = "";
											switch (sp)
											{
												case "SP0": spPattern = "...2195.0.0"; break;
												case "SP1": spPattern = "...2195.1.0"; break;
												case "SP2": spPattern = "...2195.2.0"; break;
												case "SP3": spPattern = "...2195.3.0"; break;
												case "SP4": spPattern = "...2195.4.0"; break;
											}
											
											// Replace the wildcard ...2195.. with the specific SP pattern
											string win2kText2 = text2.Replace("...2195..", spPattern);
											
											additionalEntries.Add($"{text},{win2kText2}.com_microsoft.{_txtUpdCode}.");
											if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
											{
												codeIndex.Add($"{frmItemList.provider}.{win2kText2}.com_microsoft.{_txtUpdCode}.");
											}
										}
										addedWin2000 = true;
									}
								}
								
								// =================================================================
								// IE 5.5x PROCESSING
								// =================================================================
								else if (frmItemList.provider == "ie55x")
								{
									// For IE 5.5x:
									// - Windows 98: Add with 5 dots (.....") - no build number
									// - Windows ME: Add with ...3000.. - WinME build number
									// - Windows 2000: Add with ...2195.. - covers ALL SPs (wildcard)
									
									// Process Windows 98 if selected
									if (selectedOSes.Contains("Windows 98"))
									{
										// Windows 98: ver_platform_win32_windows.4.10.x86.LANG.....
										string win98Text2 = text2.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_windows.4.10")
											.Replace("...2195..", ".....");
										
										if (!win98Text2.Contains("2195"))
										{
											// Transform the complete entry (key + all items)
											string win98FullEntry = text.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_windows.4.10")
												.Replace("...2195..", ".....");
											// Append the new update item to the transformed entry
											additionalEntries.Add($"{win98FullEntry},{win98Text2}.com_microsoft.{_txtUpdCode}.");
											if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
											{
												codeIndex.Add($"{frmItemList.provider}.{win98Text2}.com_microsoft.{_txtUpdCode}.");
											}
										}
									}
									
									// Process Windows ME if selected
									if (selectedOSes.Contains("Windows ME"))
									{
										// Windows ME: ver_platform_win32_windows.4.90.x86.LANG...3000..
										string winMEText2 = text2.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_windows.4.90")
											.Replace("...2195..", "...3000..");
										
										if (!winMEText2.Contains("2195"))
										{
											// Transform the complete entry (key + all items)
											string winMEFullEntry = text.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_windows.4.90")
												.Replace("...2195..", "...3000..");
											// Append the new update item to the transformed entry
											additionalEntries.Add($"{winMEFullEntry},{winMEText2}.com_microsoft.{_txtUpdCode}.");
											if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
											{
												codeIndex.Add($"{frmItemList.provider}.{winMEText2}.com_microsoft.{_txtUpdCode}.");
											}
										}
									}
									
									// Process Windows 2000 if selected - keep the wildcard ...2195.. to cover all SPs
									if (selectedOSes.Contains("Windows 2000"))
									{
										// Keep the original Win2K entry with wildcard (covers ALL SPs SP0-SP4)
										frmItemList.l_product2items[i] = $"{text},{text2}.com_microsoft.{_txtUpdCode}.";
										addedWin2000 = true;
										if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
										{
											codeIndex.Add($"{frmItemList.provider}.{text2}.com_microsoft.{_txtUpdCode}.");
										}
									}
								}
								
								// =================================================================
								// IE 6.0x PROCESSING
								// =================================================================
								else if (frmItemList.provider == "ie60x")
								{
									// For IE 6.0x:
									// - Windows 98: Add with 5 dots (.....") - no build number
									// - Windows ME: Add with ...3000.. - WinME build number
									// - Windows 2000: Add with ...2195.. - covers all Win2K versions (wildcard)
									// - Windows XP: Add with ...2600.. - covers all WinXP versions (wildcard)
									// - Windows Server 2003: Add with ...3790.. - covers all Win2003 versions (wildcard)
									
									// Process Windows 98 if selected
									if (selectedOSes.Contains("Windows 98"))
									{
										// Windows 98: ver_platform_win32_windows.4.10.x86.LANG.....
										string win98Text2 = text2.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_windows.4.10")
											.Replace("...2195..", ".....");
										
										if (!win98Text2.Contains("2195"))
										{
											// Transform the complete entry (key + all items)
											string win98FullEntry = text.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_windows.4.10")
												.Replace("...2195..", ".....");
											// Append the new update item to the transformed entry
											additionalEntries.Add($"{win98FullEntry},{win98Text2}.com_microsoft.{_txtUpdCode}.");
											if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
											{
												codeIndex.Add($"{frmItemList.provider}.{win98Text2}.com_microsoft.{_txtUpdCode}.");
											}
										}
									}
									
									// Process Windows ME if selected
									if (selectedOSes.Contains("Windows ME"))
									{
										// Windows ME: ver_platform_win32_windows.4.90.x86.LANG...3000..
										string winMEText2 = text2.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_windows.4.90")
											.Replace("...2195..", "...3000..");
										
										if (!winMEText2.Contains("2195"))
										{
											// Transform the complete entry (key + all items)
											string winMEFullEntry = text.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_windows.4.90")
												.Replace("...2195..", "...3000..");
											// Append the new update item to the transformed entry
											additionalEntries.Add($"{winMEFullEntry},{winMEText2}.com_microsoft.{_txtUpdCode}.");
											if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
											{
												codeIndex.Add($"{frmItemList.provider}.{winMEText2}.com_microsoft.{_txtUpdCode}.");
											}
										}
									}
									
									// Process Windows 2000 if selected - use wildcard to cover all SPs
									if (selectedOSes.Contains("Windows 2000"))
									{
										// Keep the wildcard ...2195.. to cover all Win2K versions
										frmItemList.l_product2items[i] = $"{text},{text2}.com_microsoft.{_txtUpdCode}.";
										addedWin2000 = true;
										if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
										{
											codeIndex.Add($"{frmItemList.provider}.{text2}.com_microsoft.{_txtUpdCode}.");
										}
									}
									
									// Process Windows XP if selected. Real XP itemIDs are per-service-pack and include the
									// ver_nt_workstation product type, e.g. ...nt.5.1.x86.<loc>.ver_nt_workstation..2600.<sp>.0...
									// The catalog matches its XP OS query (which contains ver_nt_workstation and a specific SP)
									// as a substring of the itemID, so we must generate one correctly-tokenized entry per SP.
									if (selectedOSes.Contains("Windows XP"))
									{
										// Family wildcard + one entry per service pack (RTM/SP1/SP2).
										foreach (string xpReplace in new[] { ".ver_nt_workstation..2600..", ".ver_nt_workstation..2600.0.0", ".ver_nt_workstation..2600.1.0", ".ver_nt_workstation..2600.2.0" })
										{
											string winXPText2 = text2.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_nt.5.1")
												.Replace("...2195..", xpReplace);
											// Only the wildcard "...2195.." (Windows 2000 family) line transforms cleanly.
											if (winXPText2.Contains("2195")) continue;
											string winXPFullEntry = text.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_nt.5.1")
												.Replace("...2195..", xpReplace);
											additionalEntries.Add($"{winXPFullEntry},{winXPText2}.com_microsoft.{_txtUpdCode}.");
											if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
											{
												codeIndex.Add($"{frmItemList.provider}.{winXPText2}.com_microsoft.{_txtUpdCode}.");
											}
										}
									}
									
									// Process Windows Server 2003 if selected. Real 2003 itemIDs include the ver_nt_server
									// product type and are per-service-pack, e.g. ...nt.5.2.x86.<loc>.ver_nt_server..3790.<sp>.0...
									// Generate one correctly-tokenized entry per SP so the catalog's 2003 query matches.
									if (selectedOSes.Contains("Windows Server 2003"))
									{
										// Family wildcard + one entry per service pack (RTM/SP1).
										foreach (string srvReplace in new[] { ".ver_nt_server..3790..", ".ver_nt_server..3790.0.0", ".ver_nt_server..3790.1.0" })
										{
											string win2003Text2 = text2.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_nt.5.2")
												.Replace("...2195..", srvReplace);
											if (win2003Text2.Contains("2195")) continue;
											string win2003FullEntry = text.Replace("ver_platform_win32_nt.5.0", "ver_platform_win32_nt.5.2")
												.Replace("...2195..", srvReplace);
											additionalEntries.Add($"{win2003FullEntry},{win2003Text2}.com_microsoft.{_txtUpdCode}.");
											if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
											{
												codeIndex.Add($"{frmItemList.provider}.{win2003Text2}.com_microsoft.{_txtUpdCode}.");
											}
										}
									}
								}
								
								// If we processed this entry for IE, collect entries and skip normal processing
								if (addedWin2000 || additionalEntries.Count > 0)
								{
									// Add all entries from this iteration to the global collection
									allAdditionalEntries.AddRange(additionalEntries);
									continue; // Skip the normal processing below
								}
							}
							else
							{
								// Not a Win2000 entry - skip it for IE updates
								continue;
							}
						}
						
						// Regular Windows OS updates (Win98, WinME, or no SP specified)
						frmItemList.l_product2items[i] = $"{text},{text2}.com_microsoft.{_txtUpdCode}.";
						if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
						{
							codeIndex.Add($"{frmItemList.provider}.{text2}.com_microsoft.{_txtUpdCode}.");
						}
					}
					else
					{
						int num = 0;
						switch (frmItemList.provider)
						{
						case "win2k":
							num = 2195;
							break;
						case "winxp":
							num = 2600;
							break;
						case "netserver":
							num = 3790;
							break;
						}
						try
						{
							string[] splitResult = text2.Split(new string[1] { num.ToString() }, StringSplitOptions.None);
							if (splitResult.Length < 2)
							{
								continue; // Not enough parts, skip this entry
							}
							
							string[] dotParts = splitResult[1].Split('.');
							if (dotParts.Length < 2)
							{
								continue; // Not enough parts, skip this entry
							}
							
							if (!Int16.TryParse(dotParts[1], out short num2))
							{
								continue; // Can't parse SP number, skip this entry
							}
							
							// Check minimum service pack
							if (!string.IsNullOrEmpty(_cmbMinSP))
							{
								if (!Int16.TryParse(_cmbMinSP, out short minSP))
								{
									throw new Exception("Invalid minimum service pack value.");
								}
								
								if (num2 < minSP)
								{
									continue; // Below minimum SP, skip
								}
							}
							
							// Check maximum service pack
							if (!string.IsNullOrEmpty(_cmbMaxSP))
							{
								if (!Int16.TryParse(_cmbMaxSP, out short maxSP))
								{
									throw new Exception("Invalid maximum service pack value.");
								}
								
								if (num2 > maxSP)
								{
									continue; // Above maximum SP, skip
								}
							}
							
							frmItemList.l_product2items[i] = $"{text},{text2}.com_microsoft.{_txtUpdCode}.";
							if ((!_chkMultilanguage && ((text2.Contains(".en") && !_chkMLFile) || _chkMLFile)) || (_chkMultilanguage && l_md_dlinks != null))
							{
								codeIndex.Add($"{frmItemList.provider}.{text2}.com_microsoft.{_txtUpdCode}.");
							}
						}
						catch
						{
							// Silently skip entries that don't match expected format
						}
					}
				}

				// Now add all collected IE entries to the product2items array
				if (allAdditionalEntries.Count > 0)
				{
					int originalLength = frmItemList.l_product2items.Length;
					Array.Resize(ref frmItemList.l_product2items, originalLength + allAdditionalEntries.Count);
					for (int j = 0; j < allAdditionalEntries.Count; j++)
					{
						frmItemList.l_product2items[originalLength + j] = allAdditionalEntries[j];
					}
				}
				
				int num3 = frmItemList.l_itemsindex.Length;
				int newSize = frmItemList.l_itemsindex.Length + codeIndex.Count;
				Array.Resize(ref frmItemList.l_itemsindex, newSize);
				for (int j = 0; j < codeIndex.Count; j++)
				{
					int targetIndex = num3 + j;
					if (targetIndex >= frmItemList.l_itemsindex.Length)
					{
						throw new Exception($"Array index out of bounds: attempting to access index {targetIndex} in array of length {frmItemList.l_itemsindex.Length}");
					}
					if (j >= codeIndex.Count)
					{
						throw new Exception($"CodeIndex access out of bounds: attempting to access index {j} in collection of count {codeIndex.Count}");
					}
					frmItemList.l_itemsindex[targetIndex] = $"{codeIndex[j]},{fileGuid.ToString().ToUpper()}@|";
				}
				int num4 = (Convert.ToInt16(!_chkCritical) + 1) * 2;
				// Format timestamp with full precision: YYYY-MM-DDTHH:MM:SS.FFFF
				string text3 = _fullDateTime.ToString("yyyy-MM-ddTHH:mm:ss.ffff");
				
				// Validate download link before trying to use it
				if (string.IsNullOrWhiteSpace(_txtDLink))
				{
					throw new Exception("Please enter a download link URL.");
				}
				
				// Try to get the file size from the download link (with timeout)
				long contentLength = 0;
				try
				{
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(_txtDLink));
					request.Timeout = 5000; // 5 second timeout
					HttpWebResponse obj2 = (HttpWebResponse)request.GetResponse();
					contentLength = obj2.ContentLength;
					obj2.Close();
				}
				catch
				{
					// If we can't reach the URL, just use 0 for content length
					// This allows offline testing
					contentLength = 0;
				}
				// Feature 3: Build command XML based on command type
			string commandXml;
			if (_commandType == "EXE")
			{
				commandXml = string.Format("<command order=\"0\" commandType=\"EXE\">{0}<switches>{1}</switches></command>", 
					_txtFileName, _txtArguments);
			}
			else if (_commandType == "ADVANCED_INF")
			{
				commandXml = string.Format("<command order=\"0\" commandType=\"ADVANCED_INF\">{0}<switches>{1}</switches></command>", 
					_commandFile, _txtArguments);
			}
			else // CABFILE
			{
				commandXml = string.Format("<command order=\"0\" commandType=\"CABFILE\">{0}<infFile>{1}</infFile><switches>{2}</switches></command>", 
					_txtFileName, _commandFile, _txtArguments);
			}
			
			// Feature 2: Check if multilanguage and handle properly
			if (_chkMultilanguage && l_md_langs != null && l_md_langs.Length > 0)
			{
				// FEATURE 2 FIX: Create separate item for each language
				for (int langIdx = 0; langIdx < l_md_langs.Length; langIdx++)
				{
					if (langIdx >= l_md_filename.Length || langIdx >= l_md_dlinks.Length || langIdx >= l_md_guids.Length)
					{
						throw new Exception($"Array index mismatch at language index {langIdx}");
					}
					
					string langCode = l_md_langs[langIdx];
					string langFilename = l_md_filename[langIdx];
					string langDLink = l_md_dlinks[langIdx];
					string langFileGuid = l_md_guids[langIdx];
					
					long langContentLength = 0;
					try
					{
						HttpWebRequest langRequest = (HttpWebRequest)WebRequest.Create(new Uri(langDLink));
						langRequest.Timeout = 5000;
						HttpWebResponse langResponse = (HttpWebResponse)langRequest.GetResponse();
						langContentLength = langResponse.ContentLength;
						langResponse.Close();
					}
					catch { langContentLength = 0; }
					
					string langCommandXml;
					if (_commandType == "EXE")
					{
						langCommandXml = string.Format("<command order=\"0\" commandType=\"EXE\">{0}<switches>{1}</switches></command>", 
							langFilename, _txtArguments);
					}
					else if (_commandType == "ADVANCED_INF")
					{
						langCommandXml = string.Format("<command order=\"0\" commandType=\"ADVANCED_INF\">{0}<switches>{1}</switches></command>", 
							_commandFile, _txtArguments);
					}
					else
					{
						langCommandXml = string.Format("<command order=\"0\" commandType=\"CABFILE\">{0}<infFile>{1}</infFile><switches>{2}</switches></command>", 
							langFilename, _commandFile, _txtArguments);
					}
					
					string langItemEntry = string.Format(
						"{0},{1}_{2}@|com_microsoft@|{3}@|{4}@|{5}@|<installation order=\"0\" installerType=\"{6}\" exclusive=\"{7}\" needsReboot=\"{8}\"><size>{9}</size><codeBase href=\"{10}\" crc=\"40146758A8239579649EC1BBAF5AA83EEE998180\" name=\"{11}\"><size>{9}</size></codeBase>{12}</installation>@|1@|{13}@|{9}@|{14}@|0@|0@|@|-768",
						langFileGuid, _txtUpdCode, langCode, langGuid.ToString().ToUpper(), _cmbGroup, _txtDetection,
						_installerType, Convert.ToInt16(_chkExclusive), Convert.ToInt16(_chkRebootReq),
						langContentLength, langDLink, langFilename, langCommandXml, num4, text3);
					
					Array.Resize(ref frmItemList.l_items, frmItemList.l_items.Length + 1);
					frmItemList.l_items[frmItemList.l_items.Length - 1] = langItemEntry;
				}
			}
			else
			{
				// Single language with Feature 3 improvements
				string text4 = string.Format(
					"{0},{1}@|com_microsoft@|{2}@|{3}@|{4}@|<installation order=\"0\" installerType=\"{5}\" exclusive=\"{6}\" needsReboot=\"{7}\"><size>{8}</size><codeBase href=\"{9}\" crc=\"40146758A8239579649EC1BBAF5AA83EEE998180\" name=\"{10}\"><size>{8}</size></codeBase>{11}</installation>@|1@|{12}@|{8}@|{13}@|0@|0@|@|-768",
					fileGuid.ToString().ToUpper(), _txtUpdCode, langGuid.ToString().ToUpper(), _cmbGroup, _txtDetection,
					_installerType, Convert.ToInt16(_chkExclusive), Convert.ToInt16(_chkRebootReq),
					contentLength, _txtDLink, _txtFileName, commandXml, num4, text3);
				
				Array.Resize(ref frmItemList.l_items, frmItemList.l_items.Length + 1);
				frmItemList.l_items[frmItemList.l_items.Length - 1] = text4;
			}
				// Validate array lengths before processing
				if (baseLangs == null || langGuids == null)
				{
					throw new Exception("Language arrays are not initialized.");
				}
				
				if (baseLangs.Length != langGuids.Length)
				{
					throw new Exception($"Language array length mismatch: baseLangs has {baseLangs.Length} elements, langGuids has {langGuids.Length} elements.");
				}
				
				int languageCount = baseLangs.Length;
				
				// Process itemstringsindex array
				int num5 = frmItemList.l_itemstringsindex.Length;
				int newSize2 = frmItemList.l_itemstringsindex.Length + languageCount;
				Array.Resize(ref frmItemList.l_itemstringsindex, newSize2);
				
				// Use explicit bounds to prevent off-by-one errors
				for (int k = 0; k < languageCount; k++)
				{
					int targetIndex = num5 + k;
					if (targetIndex >= frmItemList.l_itemstringsindex.Length)
					{
						throw new Exception($"Array index out of bounds: attempting to access index {targetIndex} in array of length {frmItemList.l_itemstringsindex.Length}");
					}
					frmItemList.l_itemstringsindex[targetIndex] = $"{frmItemList.provider}.{baseLangs[k]}.{langGuid.ToString().ToUpper()},{langGuids[k]}";
				}
				
				// Process itemstrings array
				int num7 = frmItemList.l_itemstrings.Length;
				int newSize3 = frmItemList.l_itemstrings.Length + languageCount;
				Array.Resize(ref frmItemList.l_itemstrings, newSize3);

				// Translate the title and description into every language at once. This previously made two
				// sequential Google Translate calls per language inside the loop below, roughly 54 calls one
				// after another on a client with no timeout set, which is what made adding an update crawl.
				// The source language is never sent, and a failed or timed out call falls back to the original
				// text so an add still completes offline instead of hanging or throwing.
				string[] translatedTitles = new string[languageCount];
				string[] translatedDescriptions = new string[languageCount];
				Parallel.For(0, languageCount, new ParallelOptions { MaxDegreeOfParallelism = 12 }, li =>
				{
					string lang = baseLangs[li];
					bool sameAsSource = string.Equals(lang, _cmbLang, StringComparison.OrdinalIgnoreCase);
					try
					{
						translatedTitles[li] = sameAsSource ? _txtTitle : TranslateText(_txtTitle, _cmbLang, lang);
					}
					catch (Exception)
					{
						translatedTitles[li] = _txtTitle;
					}
					try
					{
						translatedDescriptions[li] = sameAsSource ? _txtDescription : TranslateText(_txtDescription, _cmbLang, lang);
					}
					catch (Exception)
					{
						translatedDescriptions[li] = _txtDescription;
					}
				});

				// Use explicit bounds to prevent off-by-one errors
				for (int l = 0; l < languageCount; l++)
				{
					int targetIndex = num7 + l;
					if (targetIndex >= frmItemList.l_itemstrings.Length)
					{
						throw new Exception($"Array index out of bounds: attempting to access index {targetIndex} in array of length {frmItemList.l_itemstrings.Length}");
					}
					
					string text5 = "";
					if (_radOldEULA)
					{
						text5 = $"http://www.download.windowsupdate.com/msdownload/update/v3/static/RTF/{baseLangs[l]}/{_txtEULA}.htm";
					}
					else if (_radNewEULA)
					{
						text5 = $"http://support.microsoft.com/?kbid={_txtEULA}";
					}
					else if (_radCustomEULA)
					{
						text5 = string.Format(_txtEULA);
					}
					string text6 = $"{frmItemList.provider}.{langGuids[l]},{translatedTitles[l]}@|{translatedDescriptions[l]}@|{baseLangs[l]}/eula.htm@|@|{text5}";
					Console.WriteLine(text6);
					frmItemList.l_itemstrings[targetIndex] = text6;
				}
			});
			frmItemList.p_items = 0;
			frmItemList.u_items = null;
			frmItemList.lstItemCol = new List<ListViewItem>();
			frmItemList.lstItems.Items.Clear();
			frmItemList.bw.RunWorkerAsync();
			
			// Use BeginInvoke to ensure the dialog closes properly after showing the message
			this.BeginInvoke(new Action(() =>
			{
				MessageBox.Show("Update added successfully!", frmMain.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				this.DialogResult = DialogResult.OK;
				this.Close();
			}));
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error adding update: {ex.Message}\n\nStack trace:\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	// WebClient defaults to a 100 second timeout, so a single unreachable translate call used to
	// stall an add for over a minute. This caps each request instead.
	private sealed class TimedWebClient : WebClient
	{
		private readonly int timeoutMs;

		public TimedWebClient(int timeoutMs)
		{
			this.timeoutMs = timeoutMs;
		}

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest request = base.GetWebRequest(address);
			if (request != null)
			{
				request.Timeout = timeoutMs;
			}
			return request;
		}
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
		using TimedWebClient webClient = new TimedWebClient(8000);
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

	private void chkCritical_CheckedChanged(object sender, EventArgs e)
	{
		bool ce = !chkCritical.Checked;
		cmbGroup.Enabled = ce;
		if (chkCritical.Checked)
		{
			cmbGroup.SelectedIndex = 0;
		}
	}

	private void advancedWizardPage4_Paint(object sender, PaintEventArgs e)
	{
	}

	private void chkSingleLang_CheckedChanged(object sender, EventArgs e)
	{
		txtFileName.Enabled = true;
		txtDLink.Enabled = true;
		btnSelectFile.Enabled = false;
		lblMFInfo.Enabled = false;
		// "Use one file for all languages" only applies to the single-file (single language) mode.
		if (chkMLFile != null) chkMLFile.Enabled = true;
	}

	private void chkMultilanguage_CheckedChanged(object sender, EventArgs e)
	{
		txtFileName.Enabled = false;
		txtDLink.Enabled = false;
		btnSelectFile.Enabled = true;
		lblMFInfo.Enabled = true;
		// Per-language files are supplied via the selected file; the shared-file option does not apply.
		if (chkMLFile != null)
		{
			chkMLFile.Checked = false;
			chkMLFile.Enabled = false;
		}
	}

	private void btnSelectFile_Click(object sender, EventArgs e)
	{
		if (openFileDialog1.ShowDialog() == DialogResult.OK)
		{
			try
			{
				// Feature 2: Clear previous data to prevent accumulation
				l_md_langs = null;
				l_md_filename = null;
				l_md_dlinks = null;
				l_md_guids = null;
				
				List<string> md_langs = new List<string>();
				List<string> md_filename = new List<string>();
				List<string> md_dlinks = new List<string>();
				List<string> md_guids = new List<string>();
				
				if (!File.Exists(openFileDialog1.FileName))
				{
					MessageBox.Show("The selected file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				
				string[] multilangfile = File.ReadAllLines(openFileDialog1.FileName);
				
				if (multilangfile == null || multilangfile.Length == 0)
				{
					MessageBox.Show("The selected file is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				
				int validLineCount = 0;
				string[] array = multilangfile;
				foreach (string s in array)
				{
					if (string.IsNullOrWhiteSpace(s))
					{
						continue; // Skip empty lines
					}
					
					string[] parts = s.Split(',');
					if (parts.Length < 3)
					{
						MessageBox.Show($"Invalid line format: '{s}'. Each line must have at least 3 comma-separated values (language, filename, download link).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						continue;
					}
					
					md_langs.Add(parts[0].Trim());
					md_filename.Add(parts[1].Trim());
					md_dlinks.Add(parts[2].Trim());
					md_guids.Add(Guid.NewGuid().ToString().ToUpper());
					validLineCount++;
				}
				
				if (validLineCount == 0)
				{
					MessageBox.Show("No valid lines found in the file. Each line must have format: language,filename,download_link", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				
				l_md_langs = md_langs.ToArray();
				l_md_filename = md_filename.ToArray();
				l_md_dlinks = md_dlinks.ToArray();
				l_md_guids = md_guids.ToArray();
				
				lblMFInfo.Text = $"{validLineCount} languages found";
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
		this.advancedWizard1 = new AdvancedWizardControl.Wizard.AdvancedWizard();
		this.advancedWizardPage4 = new AdvancedWizardControl.WizardPages.AdvancedWizardPage();
		this.radCustomEULA = new System.Windows.Forms.RadioButton();
		this.txtEULA = new System.Windows.Forms.TextBox();
		this.lblEULACode = new System.Windows.Forms.Label();
		this.radNewEULA = new System.Windows.Forms.RadioButton();
		this.radOldEULA = new System.Windows.Forms.RadioButton();
		this.lblEULAType = new System.Windows.Forms.Label();
		this.advancedWizardPage3 = new AdvancedWizardControl.WizardPages.AdvancedWizardPage();
		this.gbDLinks = new System.Windows.Forms.GroupBox();
		this.lblMFInfo = new System.Windows.Forms.Label();
		this.btnSelectFile = new System.Windows.Forms.Button();
		this.chkMultilanguage = new System.Windows.Forms.RadioButton();
		this.chkSingleLang = new System.Windows.Forms.RadioButton();
		this.lblDownload = new System.Windows.Forms.Label();
		this.txtDLink = new System.Windows.Forms.TextBox();
		this.lblFileName = new System.Windows.Forms.Label();
		this.txtFileName = new System.Windows.Forms.TextBox();
		this.txtDetection = new System.Windows.Forms.TextBox();
		this.lblDetection = new System.Windows.Forms.Label();
		this.txtArguments = new System.Windows.Forms.TextBox();
		this.lblArguments = new System.Windows.Forms.Label();
		this.advancedWizardPage2 = new AdvancedWizardControl.WizardPages.AdvancedWizardPage();
		this.chkMLFile = new System.Windows.Forms.CheckBox();
		this.chkRebootReq = new System.Windows.Forms.CheckBox();
		this.chkEULARequired = new System.Windows.Forms.CheckBox();
		this.lblHelp0 = new System.Windows.Forms.Label();
		this.cmbGroup = new System.Windows.Forms.ComboBox();
		this.lblGroup = new System.Windows.Forms.Label();
		this.lblExtras = new System.Windows.Forms.Label();
		this.chkExclusive = new System.Windows.Forms.CheckBox();
		this.chkCritical = new System.Windows.Forms.CheckBox();
		this.advancedWizardPage1 = new AdvancedWizardControl.WizardPages.AdvancedWizardPage();
		this.cmbDate = new System.Windows.Forms.DateTimePicker();
		this.numHours = new System.Windows.Forms.NumericUpDown();
		this.numMinutes = new System.Windows.Forms.NumericUpDown();
		this.numSeconds = new System.Windows.Forms.NumericUpDown();
		this.numMilliseconds = new System.Windows.Forms.NumericUpDown();
		this.lblTime = new System.Windows.Forms.Label();
		this.cmbLang = new System.Windows.Forms.ComboBox();
		this.txtUpdCode = new System.Windows.Forms.TextBox();
		this.lblFileCode = new System.Windows.Forms.Label();
		this.cmbOS = new System.Windows.Forms.ComboBox();
		this.cmbMaxSP = new System.Windows.Forms.ComboBox();
		this.cmbMinSP = new System.Windows.Forms.ComboBox();
		this.lblServicePack = new System.Windows.Forms.Label();
		this.lblLanguage = new System.Windows.Forms.Label();
		this.lblDescription = new System.Windows.Forms.Label();
		this.txtDescription = new System.Windows.Forms.TextBox();
		this.txtTitle = new System.Windows.Forms.TextBox();
		this.lblTitle = new System.Windows.Forms.Label();
		this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
		this.lblHelp1 = new System.Windows.Forms.Label();
		
		// Feature 3: Instantiate new controls
		this.cmbInstallerType = new System.Windows.Forms.ComboBox();
		this.lblInstallerType = new System.Windows.Forms.Label();
		this.cmbCommandType = new System.Windows.Forms.ComboBox();
		this.lblCommandType = new System.Windows.Forms.Label();
		this.txtCommandFile = new System.Windows.Forms.TextBox();
		this.lblCommandFile = new System.Windows.Forms.Label();
		
		this.advancedWizard1.SuspendLayout();
		this.advancedWizardPage4.SuspendLayout();
		this.advancedWizardPage3.SuspendLayout();
		this.gbDLinks.SuspendLayout();
		this.advancedWizardPage2.SuspendLayout();
		this.advancedWizardPage1.SuspendLayout();
		base.SuspendLayout();
		this.advancedWizard1.BackButtonEnabled = true;
		this.advancedWizard1.BackButtonText = "< Back";
		this.advancedWizard1.ButtonLayout = AdvancedWizardControl.Enums.ButtonLayoutKind.Office97;
		this.advancedWizard1.ButtonsVisible = true;
		this.advancedWizard1.CancelButtonText = "&Cancel";
		this.advancedWizard1.Controls.Add(this.advancedWizardPage3);
		this.advancedWizard1.Controls.Add(this.advancedWizardPage2);
		this.advancedWizard1.Controls.Add(this.advancedWizardPage1);
		this.advancedWizard1.Controls.Add(this.advancedWizardPage4);
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
		this.advancedWizard1.Size = new System.Drawing.Size(490, 490);
		this.advancedWizard1.TabIndex = 0;
		this.advancedWizard1.TouchScreen = false;
		this.advancedWizard1.WizardPages.Add(this.advancedWizardPage1);
		this.advancedWizard1.WizardPages.Add(this.advancedWizardPage2);
		this.advancedWizard1.WizardPages.Add(this.advancedWizardPage3);
		this.advancedWizard1.WizardPages.Add(this.advancedWizardPage4);
		this.advancedWizard1.Cancel += new System.EventHandler(advancedWizard1_Cancel);
		this.advancedWizard1.Finish += new System.EventHandler(advancedWizard1_Finish);
		this.advancedWizardPage4.Controls.Add(this.radCustomEULA);
		this.advancedWizardPage4.Controls.Add(this.txtEULA);
		this.advancedWizardPage4.Controls.Add(this.lblEULACode);
		this.advancedWizardPage4.Controls.Add(this.radNewEULA);
		this.advancedWizardPage4.Controls.Add(this.radOldEULA);
		this.advancedWizardPage4.Controls.Add(this.lblEULAType);
		this.advancedWizardPage4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedWizardPage4.Header = true;
		this.advancedWizardPage4.HeaderBackgroundColor = System.Drawing.Color.White;
		this.advancedWizardPage4.HeaderFont = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		this.advancedWizardPage4.HeaderImage = WUv4Powertools.Properties.Resources.AddUpdate;
		this.advancedWizardPage4.HeaderImageVisible = true;
		this.advancedWizardPage4.HeaderTitle = "Add an Update";
		this.advancedWizardPage4.Location = new System.Drawing.Point(0, 0);
		this.advancedWizardPage4.Name = "advancedWizardPage4";
		this.advancedWizardPage4.PreviousPage = 2;
		this.advancedWizardPage4.Size = new System.Drawing.Size(440, 281);
		this.advancedWizardPage4.SubTitle = "Configure EULA Options";
		this.advancedWizardPage4.SubTitleFont = new System.Drawing.Font("Tahoma", 8f);
		this.advancedWizardPage4.TabIndex = 4;
		this.advancedWizardPage4.Paint += new System.Windows.Forms.PaintEventHandler(advancedWizardPage4_Paint);
		this.radCustomEULA.AutoSize = true;
		this.radCustomEULA.Location = new System.Drawing.Point(158, 90);
		this.radCustomEULA.Name = "radCustomEULA";
		this.radCustomEULA.Size = new System.Drawing.Size(182, 30);
		this.radCustomEULA.TabIndex = 12;
		this.radCustomEULA.TabStop = true;
		this.radCustomEULA.Text = "Custom Type\r\nhttp://www.customexample.abc/";
		this.radCustomEULA.UseVisualStyleBackColor = true;
		this.txtEULA.Location = new System.Drawing.Point(142, 223);
		this.txtEULA.Name = "txtEULA";
		this.txtEULA.Size = new System.Drawing.Size(286, 20);
		this.txtEULA.TabIndex = 11;
		this.lblEULACode.AutoSize = true;
		this.lblEULACode.Location = new System.Drawing.Point(23, 230);
		this.lblEULACode.Name = "lblEULACode";
		this.lblEULACode.Size = new System.Drawing.Size(66, 13);
		this.lblEULACode.TabIndex = 10;
		this.lblEULACode.Text = "EULA Code:";
		this.radNewEULA.AutoSize = true;
		this.radNewEULA.Checked = true;
		this.radNewEULA.Location = new System.Drawing.Point(158, 170);
		this.radNewEULA.Name = "radNewEULA";
		this.radNewEULA.Size = new System.Drawing.Size(210, 30);
		this.radNewEULA.TabIndex = 8;
		this.radNewEULA.TabStop = true;
		this.radNewEULA.Text = "New Type\r\nhttp://support.microsoft.com/?kbid={0}";
		this.radNewEULA.UseVisualStyleBackColor = true;
		this.radOldEULA.AutoSize = true;
		this.radOldEULA.Location = new System.Drawing.Point(158, 130);
		this.radOldEULA.Name = "radOldEULA";
		this.radOldEULA.Size = new System.Drawing.Size(236, 30);
		this.radOldEULA.TabIndex = 7;
		this.radOldEULA.Text = "Old Type\r\n/msdownload/update/v3/static/RTF/da/{0}";
		this.radOldEULA.UseVisualStyleBackColor = true;
		this.lblEULAType.AutoSize = true;
		this.lblEULAType.Location = new System.Drawing.Point(23, 90);
		this.lblEULAType.Name = "lblEULAType";
		this.lblEULAType.Size = new System.Drawing.Size(65, 13);
		this.lblEULAType.TabIndex = 4;
		this.lblEULAType.Text = "EULA Type:";
		this.advancedWizardPage3.Controls.Add(this.gbDLinks);
		this.advancedWizardPage3.Controls.Add(this.txtDetection);
		this.advancedWizardPage3.Controls.Add(this.lblDetection);
		this.advancedWizardPage3.Controls.Add(this.txtArguments);
		this.advancedWizardPage3.Controls.Add(this.lblArguments);
		// Feature 3: Add new controls to page
		this.advancedWizardPage3.Controls.Add(this.lblInstallerType);
		this.advancedWizardPage3.Controls.Add(this.cmbInstallerType);
		this.advancedWizardPage3.Controls.Add(this.lblCommandType);
		this.advancedWizardPage3.Controls.Add(this.cmbCommandType);
		this.advancedWizardPage3.Controls.Add(this.lblCommandFile);
		this.advancedWizardPage3.Controls.Add(this.txtCommandFile);
		this.advancedWizardPage3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedWizardPage3.Header = true;
		this.advancedWizardPage3.HeaderBackgroundColor = System.Drawing.Color.White;
		this.advancedWizardPage3.HeaderFont = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		this.advancedWizardPage3.HeaderImage = WUv4Powertools.Properties.Resources.AddUpdate;
		this.advancedWizardPage3.HeaderImageVisible = true;
		this.advancedWizardPage3.HeaderTitle = "Add an Update";
		this.advancedWizardPage3.Location = new System.Drawing.Point(0, 0);
		this.advancedWizardPage3.Name = "advancedWizardPage3";
		this.advancedWizardPage3.PreviousPage = 1;
		this.advancedWizardPage3.Size = new System.Drawing.Size(440, 281);
		this.advancedWizardPage3.SubTitle = "Set download and installation options";
		this.advancedWizardPage3.SubTitleFont = new System.Drawing.Font("Tahoma", 8f);
		this.advancedWizardPage3.TabIndex = 3;
		this.gbDLinks.Controls.Add(this.lblMFInfo);
		this.gbDLinks.Controls.Add(this.btnSelectFile);
		this.gbDLinks.Controls.Add(this.chkMultilanguage);
		this.gbDLinks.Controls.Add(this.chkSingleLang);
		this.gbDLinks.Controls.Add(this.lblDownload);
		this.gbDLinks.Controls.Add(this.txtDLink);
		this.gbDLinks.Controls.Add(this.lblFileName);
		this.gbDLinks.Controls.Add(this.txtFileName);
		this.gbDLinks.Location = new System.Drawing.Point(189, 86);
		this.gbDLinks.Name = "gbDLinks";
		this.gbDLinks.Size = new System.Drawing.Size(239, 190);
		this.gbDLinks.TabIndex = 12;
		this.gbDLinks.TabStop = false;
		this.gbDLinks.Text = "Download Link Options";
		this.lblMFInfo.AutoSize = true;
		this.lblMFInfo.Enabled = false;
		this.lblMFInfo.Location = new System.Drawing.Point(109, 164);
		this.lblMFInfo.Name = "lblMFInfo";
		this.lblMFInfo.Size = new System.Drawing.Size(0, 13);
		this.lblMFInfo.TabIndex = 11;
		this.btnSelectFile.Enabled = false;
		this.btnSelectFile.Location = new System.Drawing.Point(18, 161);
		this.btnSelectFile.Name = "btnSelectFile";
		this.btnSelectFile.Size = new System.Drawing.Size(85, 23);
		this.btnSelectFile.TabIndex = 10;
		this.btnSelectFile.Text = "Select File";
		this.btnSelectFile.UseVisualStyleBackColor = true;
		this.btnSelectFile.Click += new System.EventHandler(btnSelectFile_Click);
		this.chkMultilanguage.AutoSize = true;
		this.chkMultilanguage.Enabled = false;
		this.chkMultilanguage.Location = new System.Drawing.Point(16, 138);
		this.chkMultilanguage.Name = "chkMultilanguage";
		this.chkMultilanguage.Size = new System.Drawing.Size(138, 17);
		this.chkMultilanguage.TabIndex = 9;
		this.chkMultilanguage.Text = "Add Various Languages";
		this.chkMultilanguage.UseVisualStyleBackColor = true;
		this.chkMultilanguage.CheckedChanged += new System.EventHandler(chkMultilanguage_CheckedChanged);
		this.chkSingleLang.AutoSize = true;
		this.chkSingleLang.Checked = true;
		this.chkSingleLang.Location = new System.Drawing.Point(16, 24);
		this.chkSingleLang.Name = "chkSingleLang";
		this.chkSingleLang.Size = new System.Drawing.Size(127, 17);
		this.chkSingleLang.TabIndex = 8;
		this.chkSingleLang.TabStop = true;
		this.chkSingleLang.Text = "Add Single Language";
		this.chkSingleLang.UseVisualStyleBackColor = true;
		this.chkSingleLang.CheckedChanged += new System.EventHandler(chkSingleLang_CheckedChanged);
		this.lblDownload.AutoSize = true;
		this.lblDownload.Location = new System.Drawing.Point(13, 57);
		this.lblDownload.Name = "lblDownload";
		this.lblDownload.Size = new System.Drawing.Size(113, 13);
		this.lblDownload.TabIndex = 4;
		this.lblDownload.Text = "Download Link (ENU):";
		this.txtDLink.Location = new System.Drawing.Point(16, 73);
		this.txtDLink.Name = "txtDLink";
		this.txtDLink.Size = new System.Drawing.Size(217, 20);
		this.txtDLink.TabIndex = 5;
		this.lblFileName.AutoSize = true;
		this.lblFileName.Location = new System.Drawing.Point(13, 96);
		this.lblFileName.Name = "lblFileName";
		this.lblFileName.Size = new System.Drawing.Size(57, 13);
		this.lblFileName.TabIndex = 6;
		this.lblFileName.Text = "File Name:";
		this.txtFileName.Location = new System.Drawing.Point(16, 112);
		this.txtFileName.Name = "txtFileName";
		this.txtFileName.Size = new System.Drawing.Size(217, 20);
		this.txtFileName.TabIndex = 7;
		// This holds a long XML detection fragment. It used to be eight pixels tall so its bottom
		// lined up with the download group box, which made it unreadable and unusable. It now runs
		// the full width below both columns, where there is free space.
		this.txtDetection.Location = new System.Drawing.Point(15, 304);
		this.txtDetection.Multiline = true;
		this.txtDetection.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.txtDetection.WordWrap = true;
		this.txtDetection.Name = "txtDetection";
		this.txtDetection.Size = new System.Drawing.Size(413, 100);
		this.txtDetection.TabIndex = 10;
		this.lblDetection.AutoSize = true;
		this.lblDetection.Location = new System.Drawing.Point(12, 288);
		this.lblDetection.Name = "lblDetection";
		this.lblDetection.Size = new System.Drawing.Size(56, 13);
		this.lblDetection.TabIndex = 10;
		this.lblDetection.Text = "Detection:";
		// Feature 3: Configure new controls - positioned at TOP of page
		this.lblInstallerType.AutoSize = true;
		this.lblInstallerType.Location = new System.Drawing.Point(12, 86);
		this.lblInstallerType.Name = "lblInstallerType";
		this.lblInstallerType.Size = new System.Drawing.Size(75, 13);
		this.lblInstallerType.TabIndex = 1;
		this.lblInstallerType.Text = "Installer Type:";
		this.cmbInstallerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbInstallerType.FormattingEnabled = true;
		this.cmbInstallerType.Items.AddRange(new object[] { "SOFTWARE", "ADVANCED_INF", "DRIVER" });
		this.cmbInstallerType.Location = new System.Drawing.Point(15, 102);
		this.cmbInstallerType.Name = "cmbInstallerType";
		this.cmbInstallerType.Size = new System.Drawing.Size(168, 21);
		this.cmbInstallerType.TabIndex = 2;
		this.cmbInstallerType.SelectedIndex = 0;
		this.lblCommandType.AutoSize = true;
		this.lblCommandType.Location = new System.Drawing.Point(12, 127);
		this.lblCommandType.Name = "lblCommandType";
		this.lblCommandType.Size = new System.Drawing.Size(85, 13);
		this.lblCommandType.TabIndex = 3;
		this.lblCommandType.Text = "Command Type:";
		this.cmbCommandType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbCommandType.FormattingEnabled = true;
		this.cmbCommandType.Items.AddRange(new object[] { "EXE", "ADVANCED_INF", "CABFILE" });
		this.cmbCommandType.Location = new System.Drawing.Point(15, 143);
		this.cmbCommandType.Name = "cmbCommandType";
		this.cmbCommandType.Size = new System.Drawing.Size(168, 21);
		this.cmbCommandType.TabIndex = 4;
		this.cmbCommandType.SelectedIndex = 0;
		this.cmbCommandType.SelectedIndexChanged += new System.EventHandler(this.CmbCommandType_SelectedIndexChanged);
		this.lblCommandFile.AutoSize = true;
		this.lblCommandFile.Location = new System.Drawing.Point(12, 168);
		this.lblCommandFile.Name = "lblCommandFile";
		this.lblCommandFile.Size = new System.Drawing.Size(110, 13);
		this.lblCommandFile.TabIndex = 5;
		this.lblCommandFile.Text = "Command/INF File:";
		this.lblCommandFile.Visible = false;
		this.txtCommandFile.Location = new System.Drawing.Point(15, 184);
		this.txtCommandFile.Name = "txtCommandFile";
		this.txtCommandFile.Size = new System.Drawing.Size(168, 20);
		this.txtCommandFile.TabIndex = 6;
		this.txtCommandFile.Visible = false;
		this.lblArguments.AutoSize = true;
		this.lblArguments.Location = new System.Drawing.Point(12, 210);
		this.lblArguments.Name = "lblArguments";
		this.lblArguments.Size = new System.Drawing.Size(60, 13);
		this.lblArguments.TabIndex = 7;
		this.lblArguments.Text = "Arguments:";
		this.txtArguments.Location = new System.Drawing.Point(15, 226);
		this.txtArguments.Name = "txtArguments";
		this.txtArguments.Size = new System.Drawing.Size(168, 20);
		this.txtArguments.TabIndex = 8;
		this.txtArguments.Text = "/q:a /r:n";
		this.lblDetection.AutoSize = true;
		this.lblDetection.Location = new System.Drawing.Point(12, 252);
		this.lblDetection.Name = "lblDetection";
		this.lblDetection.Size = new System.Drawing.Size(56, 13);
		this.lblDetection.TabIndex = 9;
		this.lblDetection.Text = "Detection:";
		// Note: txtDetection will be clipped at bottom - moved Arguments/Detection down
		this.advancedWizardPage2.Controls.Add(this.lblHelp1);
		this.advancedWizardPage2.Controls.Add(this.chkMLFile);
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
		this.advancedWizardPage2.HeaderImage = WUv4Powertools.Properties.Resources.AddUpdate;
		this.advancedWizardPage2.HeaderImageVisible = true;
		this.advancedWizardPage2.HeaderTitle = "Add an Update";
		this.advancedWizardPage2.Location = new System.Drawing.Point(0, 0);
		this.advancedWizardPage2.Name = "advancedWizardPage2";
		this.advancedWizardPage2.PreviousPage = 0;
		this.advancedWizardPage2.Size = new System.Drawing.Size(440, 281);
		this.advancedWizardPage2.SubTitle = "Configure the group and exclusiveness of update";
		this.advancedWizardPage2.SubTitleFont = new System.Drawing.Font("Tahoma", 8f);
		this.advancedWizardPage2.TabIndex = 2;
		this.chkMLFile.AutoSize = true;
		this.chkMLFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.chkMLFile.Location = new System.Drawing.Point(248, 136);
		this.chkMLFile.Name = "chkMLFile";
		this.chkMLFile.Size = new System.Drawing.Size(111, 17);
		this.chkMLFile.TabIndex = 9;
		this.chkMLFile.Text = "Use one file for all languages";
		this.chkMLFile.UseVisualStyleBackColor = true;
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
		this.advancedWizardPage1.Controls.Add(this.cmbDate);
		this.advancedWizardPage1.Controls.Add(this.lblTime);
		this.advancedWizardPage1.Controls.Add(this.numHours);
		this.advancedWizardPage1.Controls.Add(this.numMinutes);
		this.advancedWizardPage1.Controls.Add(this.numSeconds);
		this.advancedWizardPage1.Controls.Add(this.numMilliseconds);
		this.advancedWizardPage1.Controls.Add(this.cmbLang);
		this.advancedWizardPage1.Controls.Add(this.txtUpdCode);
		this.advancedWizardPage1.Controls.Add(this.lblFileCode);
		this.advancedWizardPage1.Controls.Add(this.cmbOS);
		this.advancedWizardPage1.Controls.Add(this.cmbMaxSP);
		this.advancedWizardPage1.Controls.Add(this.cmbMinSP);
		this.advancedWizardPage1.Controls.Add(this.lblServicePack);
		this.advancedWizardPage1.Controls.Add(this.lblLanguage);
		this.advancedWizardPage1.Controls.Add(this.lblDescription);
		this.advancedWizardPage1.Controls.Add(this.txtDescription);
		this.advancedWizardPage1.Controls.Add(this.txtTitle);
		this.advancedWizardPage1.Controls.Add(this.lblTitle);
		this.advancedWizardPage1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedWizardPage1.Header = true;
		this.advancedWizardPage1.HeaderBackgroundColor = System.Drawing.Color.White;
		this.advancedWizardPage1.HeaderFont = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		this.advancedWizardPage1.HeaderImage = WUv4Powertools.Properties.Resources.AddUpdate;
		this.advancedWizardPage1.HeaderImageVisible = true;
		this.advancedWizardPage1.HeaderTitle = "Add an Update";
		this.advancedWizardPage1.Location = new System.Drawing.Point(0, 0);
		this.advancedWizardPage1.Name = "advancedWizardPage1";
		this.advancedWizardPage1.PreviousPage = 0;
		this.advancedWizardPage1.Size = new System.Drawing.Size(440, 281);
		this.advancedWizardPage1.SubTitle = "Set initial features of your update";
		this.advancedWizardPage1.SubTitleFont = new System.Drawing.Font("Tahoma", 8f);
		this.advancedWizardPage1.TabIndex = 1;
		// Title/Name field
		this.lblTitle.AutoSize = true;
		this.lblTitle.Location = new System.Drawing.Point(20, 88);
		this.lblTitle.Name = "lblTitle";
		this.lblTitle.Size = new System.Drawing.Size(80, 13);
		this.lblTitle.TabIndex = 1;
		this.lblTitle.Text = "Title:";
		this.txtTitle.Location = new System.Drawing.Point(130, 85);
		this.txtTitle.Name = "txtTitle";
		this.txtTitle.Size = new System.Drawing.Size(295, 20);
		this.txtTitle.TabIndex = 1;
		// File Code
		this.lblFileCode.AutoSize = true;
		this.lblFileCode.Location = new System.Drawing.Point(20, 118);
		this.lblFileCode.Name = "lblFileCode";
		this.lblFileCode.Size = new System.Drawing.Size(58, 13);
		this.lblFileCode.TabIndex = 9;
		this.lblFileCode.Text = "File Code:";
		this.txtUpdCode.Location = new System.Drawing.Point(130, 115);
		this.txtUpdCode.Name = "txtUpdCode";
		this.txtUpdCode.Size = new System.Drawing.Size(295, 20);
		this.txtUpdCode.TabIndex = 2;
		// Description
		this.lblDescription.AutoSize = true;
		this.lblDescription.Location = new System.Drawing.Point(20, 148);
		this.lblDescription.Name = "lblDescription";
		this.lblDescription.Size = new System.Drawing.Size(66, 13);
		this.lblDescription.TabIndex = 4;
		this.lblDescription.Text = "Description:";
		this.txtDescription.Location = new System.Drawing.Point(130, 145);
		this.txtDescription.Multiline = true;
		this.txtDescription.Name = "txtDescription";
		this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.txtDescription.Size = new System.Drawing.Size(295, 45);
		this.txtDescription.TabIndex = 3;
		// Translation Language
		this.lblLanguage.AutoSize = true;
		this.lblLanguage.Location = new System.Drawing.Point(20, 200);
		this.lblLanguage.Name = "lblLanguage";
		this.lblLanguage.Size = new System.Drawing.Size(100, 13);
		this.lblLanguage.TabIndex = 13;
		this.lblLanguage.Text = "Translation Language:";
		this.cmbLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbLang.FormattingEnabled = true;
		this.cmbLang.Items.AddRange(new object[23]
		{
			"ar", "cs", "da", "de", "el", "en", "es", "fi", "fr", "he",
			"hu", "it", "ja", "ko", "nl", "no", "pl", "pt", "ru", "sv",
			"tr", "zh-cn", "zh-tw"
		});
		this.cmbLang.Location = new System.Drawing.Point(130, 197);
		this.cmbLang.Name = "cmbLang";
		this.cmbLang.Size = new System.Drawing.Size(80, 21);
		this.cmbLang.TabIndex = 4;
		// Date and Time - ALL ON ONE LINE
		this.cmbDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
		this.cmbDate.Location = new System.Drawing.Point(220, 197);
		this.cmbDate.Name = "cmbDate";
		this.cmbDate.Size = new System.Drawing.Size(90, 20);
		this.cmbDate.TabIndex = 5;
		// @ separator
		this.lblTime.AutoSize = false;
		this.lblTime.Location = new System.Drawing.Point(315, 197);
		this.lblTime.Name = "lblTime";
		this.lblTime.Size = new System.Drawing.Size(15, 20);
		this.lblTime.TabIndex = 20;
		this.lblTime.Text = "@";
		this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		// Hours
		((System.ComponentModel.ISupportInitialize)this.numHours).BeginInit();
		this.numHours.Location = new System.Drawing.Point(333, 197);
		this.numHours.Maximum = new decimal(new int[] { 23, 0, 0, 0 });
		this.numHours.Name = "numHours";
		this.numHours.Size = new System.Drawing.Size(28, 20);
		this.numHours.TabIndex = 6;
		this.numHours.Value = new decimal(new int[] { 0, 0, 0, 0 });
		((System.ComponentModel.ISupportInitialize)this.numHours).EndInit();
		// Colon 1
		Label lblColon1 = new Label();
		lblColon1.AutoSize = false;
		lblColon1.Location = new System.Drawing.Point(361, 197);
		lblColon1.Size = new System.Drawing.Size(6, 20);
		lblColon1.Text = ":";
		lblColon1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.advancedWizardPage1.Controls.Add(lblColon1);
		// Minutes
		((System.ComponentModel.ISupportInitialize)this.numMinutes).BeginInit();
		this.numMinutes.Location = new System.Drawing.Point(367, 197);
		this.numMinutes.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
		this.numMinutes.Name = "numMinutes";
		this.numMinutes.Size = new System.Drawing.Size(28, 20);
		this.numMinutes.TabIndex = 7;
		this.numMinutes.Value = new decimal(new int[] { 0, 0, 0, 0 });
		((System.ComponentModel.ISupportInitialize)this.numMinutes).EndInit();
		// Colon 2
		Label lblColon2 = new Label();
		lblColon2.AutoSize = false;
		lblColon2.Location = new System.Drawing.Point(395, 197);
		lblColon2.Size = new System.Drawing.Size(6, 20);
		lblColon2.Text = ":";
		lblColon2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.advancedWizardPage1.Controls.Add(lblColon2);
		// Seconds
		((System.ComponentModel.ISupportInitialize)this.numSeconds).BeginInit();
		this.numSeconds.Location = new System.Drawing.Point(401, 197);
		this.numSeconds.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
		this.numSeconds.Name = "numSeconds";
		this.numSeconds.Size = new System.Drawing.Size(28, 20);
		this.numSeconds.TabIndex = 8;
		this.numSeconds.Value = new decimal(new int[] { 0, 0, 0, 0 });
		((System.ComponentModel.ISupportInitialize)this.numSeconds).EndInit();
		// OS and Service Pack
		this.lblServicePack.AutoSize = true;
		this.lblServicePack.Location = new System.Drawing.Point(20, 230);
		this.lblServicePack.Name = "lblServicePack";
		this.lblServicePack.Size = new System.Drawing.Size(110, 13);
		this.lblServicePack.TabIndex = 8;
		this.lblServicePack.Text = "OS and SP (min-max):";
		this.cmbOS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbOS.FormattingEnabled = true;
		this.cmbOS.Items.AddRange(new object[5] { "Windows 98", "Windows ME", "Windows 2000", "Windows XP", "Windows Server 2003" });
		this.cmbOS.Location = new System.Drawing.Point(130, 227);
		this.cmbOS.Name = "cmbOS";
		this.cmbOS.Size = new System.Drawing.Size(140, 21);
		this.cmbOS.TabIndex = 9;
		this.cmbOS.SelectedIndexChanged += new System.EventHandler(cmbOS_SelectedIndexChanged);
		this.cmbMinSP.FormattingEnabled = true;
		this.cmbMinSP.Location = new System.Drawing.Point(280, 227);
		this.cmbMinSP.Name = "cmbMinSP";
		this.cmbMinSP.Size = new System.Drawing.Size(50, 21);
		this.cmbMinSP.TabIndex = 10;
		this.cmbMaxSP.FormattingEnabled = true;
		this.cmbMaxSP.Location = new System.Drawing.Point(340, 227);
		this.cmbMaxSP.Name = "cmbMaxSP";
		this.cmbMaxSP.Size = new System.Drawing.Size(50, 21);
		this.cmbMaxSP.TabIndex = 11;
		this.openFileDialog1.FileName = "openFileDialog1";
		this.openFileDialog1.Filter = "Text Files (*.txt)|*.txt";
		this.openFileDialog1.Title = "Select an File with links";
		this.lblHelp1.AutoSize = true;
		this.lblHelp1.Location = new System.Drawing.Point(230, 229);
		this.lblHelp1.Name = "lblHelp1";
		this.lblHelp1.Size = new System.Drawing.Size(198, 39);
		this.lblHelp1.TabIndex = 10;
		this.lblHelp1.Text = "90944 = Additional Windows Downloads\r\n90949 = Windows Tools\r\n90952 = Advanced Security Updates";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.Control;
		// The pages are all docked to fill, so per page sizes are ignored and only the form size
		// decides how much is visible. The deepest content is the IE 5.0x flow, whose help text ends
		// at 380px, and the widest is its row of five OS checkboxes ending at 475px. Add the 40px
		// button strip and this is what has to fit.
		base.ClientSize = new System.Drawing.Size(490, 490);
		base.Controls.Add(this.advancedWizard1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmAddUpdate";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Add an Update";
		base.Load += new System.EventHandler(frmAddUpdate_Load);
		this.advancedWizard1.ResumeLayout(false);
		this.advancedWizardPage4.ResumeLayout(false);
		this.advancedWizardPage4.PerformLayout();
		this.advancedWizardPage3.ResumeLayout(false);
		this.advancedWizardPage3.PerformLayout();
		this.gbDLinks.ResumeLayout(false);
		this.gbDLinks.PerformLayout();
		this.advancedWizardPage2.ResumeLayout(false);
		this.advancedWizardPage2.PerformLayout();
		this.advancedWizardPage1.ResumeLayout(false);
		this.advancedWizardPage1.PerformLayout();
		base.ResumeLayout(false);
	}
}
