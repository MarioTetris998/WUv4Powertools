using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AdvancedWizardControl.EventArguments;
using AdvancedWizardControl.Wizard;

namespace AdvancedWizardControl.WizardPages;

[ToolboxItem(false)]
[Designer(typeof(AdvancedWizardPageDesigner))]
public class AdvancedWizardPage : Panel
{
	internal Panel HeaderPanel;

	internal PictureBox WizardImage;

	internal Label WizardSubText;

	internal Label WizardText;

	private bool _headerVisible;

	private bool _imageVisible;

	[Description("A 48x48 image for the wizard header")]
	[Category("WizardPage")]
	[DefaultValue(null)]
	public Image HeaderImage
	{
		get
		{
			return WizardImage.Image;
		}
		set
		{
			WizardImage.Image = value;
		}
	}

	[Category("WizardPage")]
	[Description("Shows/Hides the image in the header")]
	public bool HeaderImageVisible
	{
		get
		{
			return _imageVisible;
		}
		set
		{
			_imageVisible = value;
			WizardImage.Visible = _imageVisible;
		}
	}

	[Category("WizardPage")]
	[Description("The background color for the header")]
	public Color HeaderBackgroundColor
	{
		get
		{
			return HeaderPanel.BackColor;
		}
		set
		{
			HeaderPanel.BackColor = value;
		}
	}

	[Description("Allows a title for the current page")]
	[Localizable(true)]
	[Category("WizardPage")]
	public string HeaderTitle
	{
		get
		{
			return WizardText.Text;
		}
		set
		{
			WizardText.Text = value;
		}
	}

	[Category("WizardPage")]
	[Description("The font for the header title")]
	public Font HeaderFont
	{
		get
		{
			return WizardText.Font;
		}
		set
		{
			WizardText.Font = value;
		}
	}

	[Localizable(true)]
	[Category("WizardPage")]
	[Description("Allows a subheading for the current page.")]
	public string SubTitle
	{
		get
		{
			return WizardSubText.Text;
		}
		set
		{
			WizardSubText.Text = value;
		}
	}

	[Category("WizardPage")]
	[Description("The font for the subtitle")]
	public Font SubTitleFont
	{
		get
		{
			return WizardSubText.Font;
		}
		set
		{
			WizardSubText.Font = value;
		}
	}

	[Description("The header gives you a head start in designing your pages. Turn it off for complete freedom of design.")]
	[Category("WizardPage")]
	public bool Header
	{
		get
		{
			return _headerVisible;
		}
		set
		{
			_headerVisible = value;
			HeaderPanel.Visible = _headerVisible;
		}
	}

	[Browsable(false)]
	public int PreviousPage { get; set; }

	[Description("Fires when the page is shown")]
	[Category("Wizard")]
	public event EventHandler<WizardPageEventArgs> PageShow = delegate
	{
	};

	public AdvancedWizardPage()
	{
		SetupHeader();
		SetupHeaderImage();
		SetupWizardText();
	}

	internal void FirePageShowEvent()
	{
		this.PageShow(this, new WizardPageEventArgs(this));
	}

	private void SetupHeader()
	{
		HeaderPanel = new Panel
		{
			Parent = this,
			Dock = DockStyle.Top,
			Height = 70,
			BackColor = Color.White
		};
		_headerVisible = true;
		_imageVisible = true;
	}

	private void SetupHeaderImage()
	{
		WizardImage = new PictureBox
		{
			Parent = HeaderPanel,
			Size = new Size(48, 48)
		};
		WizardImage.Left = base.Width - WizardImage.Width - 10;
		WizardImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
		WizardImage.Top = HeaderPanel.Height - WizardImage.Height - 10;
		WizardImage.BackColor = Color.Transparent;
		Stream manifestResourceStream = GetType().Assembly.GetManifestResourceStream("AdvancedWizardControl.Resources.wiz.gif");
		try
		{
			if (manifestResourceStream != null)
			{
				WizardImage.Image = Image.FromStream(manifestResourceStream);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void SetupWizardText()
	{
		WizardText = new Label
		{
			Font = new Font("tahoma", 10f, FontStyle.Bold),
			Left = HeaderPanel.Left + 20,
			Top = 20,
			AutoSize = true,
			Parent = HeaderPanel,
			Text = "Welcome to Advanced Wizard"
		};
		WizardSubText = new Label
		{
			Font = new Font("tahoma", 8f),
			Left = HeaderPanel.Left + 40,
			Top = 38,
			AutoSize = true,
			Parent = HeaderPanel,
			Text = "Your page description goes here"
		};
	}
}
