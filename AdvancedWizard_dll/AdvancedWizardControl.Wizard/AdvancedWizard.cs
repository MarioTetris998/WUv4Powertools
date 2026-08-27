using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using AdvancedWizardControl.Enums;
using AdvancedWizardControl.EventArguments;
using AdvancedWizardControl.Strategies;
using AdvancedWizardControl.WizardPages;

namespace AdvancedWizardControl.Wizard;

[ToolboxBitmap(typeof(Bitmap))]
[DefaultEvent("Finish")]
[Designer(typeof(AdvancedWizardDesigner))]
[DefaultProperty("Pages")]
public class AdvancedWizard : UserControl, IMessageFilter
{
	private const int VkEscape = 27;

	private const int VkReturn = 13;

	private const int WmKeydown = 256;

	private Button _btnBack;

	private Button _btnCancel;

	private Button _btnFinish;

	private Button _btnHelp;

	private Button _btnNext;

	private Panel _pnlButtons;

	private readonly AdvancedWizardPageCollection _pages;

	private bool _finishButton = true;

	private bool _helpButton = true;

	private AdvancedWizardPage _lastPage;

	internal bool NextButtonEnabledState;

	private bool _pageSetAsFinishPage;

	private int _selectedPage;

	private ISelectionService _selectionService;

	private readonly WizardStrategy _wizardStrategy;

	private string _backButtonText = "< Back";

	private ButtonLayoutKind _buttonLayoutKind;

	private bool _buttonsVisible = true;

	private string _cancelButtonText = "&Cancel";

	private string _finishButtonText = "&Finish";

	private string _helpButtonText = "&Help";

	private string _nextButtonText = "Next >";

	private string _tempNextText = "Next >";

	private bool _touchScreen;

	[Description("Add pages to the wizard.")]
	[Category("_wizardStrategy")]
	[Editor(typeof(AdvancedWizardCollectionEditor), typeof(UITypeEditor))]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public AdvancedWizardPageCollection WizardPages => _pages;

	[Browsable(false)]
	public AdvancedWizardPage CurrentPage => _pages[_selectedPage];

	[Description("Allows user to control the wizard through the Escape and Enter keys.")]
	[Category("_wizardStrategy")]
	public bool ProcessKeys { get; set; }

	[Description("Set the style of all the wizard buttons.")]
	[Browsable(true)]
	[Category("Behavior")]
	public FlatStyle FlatStyle
	{
		get
		{
			return _btnCancel.FlatStyle;
		}
		set
		{
			_btnCancel.FlatStyle = value;
			_btnBack.FlatStyle = value;
			_btnNext.FlatStyle = value;
			_btnFinish.FlatStyle = value;
			_btnHelp.FlatStyle = value;
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[Category("_wizardStrategy")]
	[Description("Alters the layout of the buttons.")]
	public ButtonLayoutKind ButtonLayout
	{
		get
		{
			return _buttonLayoutKind;
		}
		set
		{
			_buttonLayoutKind = value;
			_pnlButtons.SuspendLayout();
			try
			{
				ChangeSettingsBasedOnLayout(value);
				ShowHelpAndFinishButtons();
			}
			finally
			{
				_pnlButtons.ResumeLayout();
			}
		}
	}

	[Category("_wizardStrategy")]
	[Description("Show or hide the butons. You can still access the pages programmatically if you hide them.")]
	[Browsable(true)]
	public bool ButtonsVisible
	{
		get
		{
			return _buttonsVisible;
		}
		set
		{
			_buttonsVisible = value;
			ProcessButtonVisibleValue(value);
		}
	}

	[Category("_wizardStrategy")]
	[Browsable(true)]
	[Description("Increase the button size for easier use on a touchscreen")]
	public bool TouchScreen
	{
		get
		{
			return _touchScreen;
		}
		set
		{
			_touchScreen = value;
			ProcessTouchScreenValue(value);
		}
	}

	[Description("Allows a choice of a dedicated button to complete the wizard steps or to use the Next button.")]
	[Category("_wizardStrategy")]
	public bool FinishButton
	{
		get
		{
			return _finishButton;
		}
		set
		{
			_finishButton = value;
			if (_buttonLayoutKind == ButtonLayoutKind.Default)
			{
				ChangeDefaultLayout(_finishButton);
			}
			else
			{
				ChangeOfficeLayout(_finishButton);
			}
		}
	}

	[Category("_wizardStrategy")]
	[Description("Allows a choice of a dedicated button to complete the wizard steps or to use the Next button.")]
	public bool HelpButton
	{
		get
		{
			return _helpButton;
		}
		set
		{
			_helpButton = value;
			switch (_buttonLayoutKind)
			{
			case ButtonLayoutKind.Default:
				switch (_helpButton)
				{
				case true:
					_btnHelp.Visible = true;
					_btnHelp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
					break;
				case false:
					_btnHelp.Visible = false;
					_btnHelp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
					break;
				}
				break;
			case ButtonLayoutKind.Office97:
				switch (_helpButton)
				{
				case true:
					_btnHelp.Visible = true;
					_btnHelp.Left = _pnlButtons.Width - _btnHelp.Width - 12;
					_btnHelp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
					_btnCancel.Left = _btnHelp.Left - _btnCancel.Width - 5;
					_btnFinish.Left = _btnCancel.Left - _btnFinish.Width - 5;
					if (_btnFinish.Visible)
					{
						_btnNext.Left = _btnFinish.Left - _btnFinish.Width - 5;
					}
					else
					{
						_btnNext.Left = _btnFinish.Left;
					}
					_btnBack.Left = _btnNext.Left - _btnNext.Width;
					break;
				case false:
					_btnHelp.Visible = false;
					_btnHelp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
					_btnCancel.Left = _pnlButtons.Width - _btnCancel.Width - 12;
					_btnFinish.Left = _btnCancel.Left - _btnFinish.Width - 5;
					if (_btnFinish.Visible)
					{
						_btnNext.Left = _btnFinish.Left - _btnNext.Width - 5;
					}
					else
					{
						_btnNext.Left = _btnFinish.Left;
					}
					_btnBack.Left = _btnNext.Left - _btnBack.Width;
					break;
				}
				break;
			}
		}
	}

	[Localizable(true)]
	[Category("WizardText")]
	[Description("Customise the text of the Cancel button.")]
	public string HelpButtonText
	{
		get
		{
			return _btnHelp.Text;
		}
		set
		{
			_helpButtonText = value;
			_btnHelp.Text = value;
		}
	}

	[Localizable(true)]
	[Category("WizardText")]
	[Description("Customise the text of the Cancel button.")]
	public string CancelButtonText
	{
		get
		{
			return _btnCancel.Text;
		}
		set
		{
			_cancelButtonText = value;
			_btnCancel.Text = value;
		}
	}

	[Localizable(true)]
	[Category("WizardText")]
	[Description("Customise the text of the Finish button.")]
	public string FinishButtonText
	{
		get
		{
			return _btnFinish.Text;
		}
		set
		{
			_finishButtonText = value;
			_btnFinish.Text = value;
		}
	}

	[Category("WizardText")]
	[Localizable(true)]
	[Description("Customise the text of the Back button.")]
	public string BackButtonText
	{
		get
		{
			return _btnBack.Text;
		}
		set
		{
			_backButtonText = value;
			_btnBack.Text = value;
		}
	}

	[Localizable(true)]
	[Category("WizardText")]
	[Description("Customise the text of the Next button.")]
	public string NextButtonText
	{
		get
		{
			return _btnNext.Text;
		}
		set
		{
			_tempNextText = _nextButtonText;
			_nextButtonText = value;
			_btnNext.Text = value;
		}
	}

	[Browsable(false)]
	[Localizable(true)]
	[Category("_wizardStrategy")]
	[Description("Indicates whether the control is enabled.")]
	public bool BackButtonEnabled
	{
		get
		{
			return _btnBack.Enabled;
		}
		set
		{
			_btnBack.Enabled = value;
			_btnBack.Invalidate();
		}
	}

	[Browsable(false)]
	[Category("_wizardStrategy")]
	[Description("Indicates whether the control is enabled.")]
	[Localizable(true)]
	public bool NextButtonEnabled
	{
		get
		{
			return NextButtonEnabledState;
		}
		set
		{
			NextButtonEnabledState = value;
			_btnNext.Enabled = NextButtonEnabledState;
			_btnNext.Invalidate();
		}
	}

	[Localizable(true)]
	[Browsable(false)]
	[Category("_wizardStrategy")]
	[Description("Indicates whether the control is enabled.")]
	public bool FinishButtonEnabled
	{
		get
		{
			bool finishButton = _finishButton;
			if (finishButton)
			{
				return _btnFinish.Enabled;
			}
			return _btnNext.Enabled;
		}
		set
		{
			switch (_finishButton)
			{
			case true:
				_btnFinish.Enabled = value;
				break;
			case false:
				_btnNext.Enabled = value;
				NextButtonEnabledState = value;
				break;
			}
		}
	}

	[Browsable(false)]
	public bool CurrentPageIsFinishPage
	{
		get
		{
			if (_pageSetAsFinishPage)
			{
				return _lastPage == CurrentPage;
			}
			return false;
		}
		set
		{
			if (value)
			{
				_pageSetAsFinishPage = true;
				_lastPage = CurrentPage;
				if (HasExplicitFinishButton())
				{
					NextButtonEnabled = false;
				}
				else
				{
					_btnNext.Text = FinishButtonText;
				}
			}
			else
			{
				_pageSetAsFinishPage = false;
				_lastPage = null;
				if (HasExplicitFinishButton())
				{
					NextButtonEnabled = true;
				}
				else
				{
					_btnNext.Text = _tempNextText;
				}
			}
		}
	}

	[Category("WizardAction")]
	[Description("Fires when the Cancel button is clicked.")]
	public event EventHandler Cancel = delegate
	{
	};

	[Category("WizardAction")]
	[Description("Fires when the Next button is clicked.")]
	public event EventHandler<WizardEventArgs> Next = delegate
	{
	};

	[Description("Fires when the Back button is clicked.")]
	[Category("WizardAction")]
	public event EventHandler Back = delegate
	{
	};

	[Category("WizardAction")]
	[Description("Fires when the Finish button is clicked.")]
	public event EventHandler Finish = delegate
	{
	};

	[Description("Fires when the Help button is clicked.")]
	[Category("WizardAction")]
	public event EventHandler Help = delegate
	{
	};

	[Description("Fires when the page changes.")]
	[Category("WizardAction")]
	public event EventHandler<WizardPageChangedEventArgs> PageChanged = delegate
	{
	};

	[Category("WizardAction")]
	[Description("Fires when the last page is reached.")]
	public event EventHandler LastPage = delegate
	{
	};

	private void InitializeComponent()
	{
		this._pnlButtons = new System.Windows.Forms.Panel();
		this._btnHelp = new System.Windows.Forms.Button();
		this._btnCancel = new System.Windows.Forms.Button();
		this._btnBack = new System.Windows.Forms.Button();
		this._btnNext = new System.Windows.Forms.Button();
		this._btnFinish = new System.Windows.Forms.Button();
		this._pnlButtons.SuspendLayout();
		base.SuspendLayout();
		this._pnlButtons.BackColor = System.Drawing.SystemColors.Control;
		this._pnlButtons.Controls.Add(this._btnHelp);
		this._pnlButtons.Controls.Add(this._btnCancel);
		this._pnlButtons.Controls.Add(this._btnBack);
		this._pnlButtons.Controls.Add(this._btnNext);
		this._pnlButtons.Controls.Add(this._btnFinish);
		this._pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
		this._pnlButtons.Location = new System.Drawing.Point(0, 256);
		this._pnlButtons.Name = "_pnlButtons";
		this._pnlButtons.Size = new System.Drawing.Size(440, 40);
		this._pnlButtons.TabIndex = 0;
		this._btnHelp.Location = new System.Drawing.Point(8, 8);
		this._btnHelp.Name = "_btnHelp";
		this._btnHelp.TabIndex = 9;
		this._btnHelp.TabStop = false;
		this._btnHelp.Text = "&Help";
		this._btnHelp.Click += new System.EventHandler(BtnHelpClick);
		this._btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this._btnCancel.Location = new System.Drawing.Point(120, 8);
		this._btnCancel.Name = "_btnCancel";
		this._btnCancel.TabIndex = 8;
		this._btnCancel.TabStop = false;
		this._btnCancel.Text = "&Cancel";
		this._btnCancel.Click += new System.EventHandler(BtnCancelClick);
		this._btnBack.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this._btnBack.Location = new System.Drawing.Point(200, 8);
		this._btnBack.Name = "_btnBack";
		this._btnBack.TabIndex = 7;
		this._btnBack.TabStop = false;
		this._btnBack.Text = "< Back";
		this._btnBack.Click += new System.EventHandler(BtnBackClick);
		this._btnNext.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this._btnNext.Location = new System.Drawing.Point(280, 8);
		this._btnNext.Name = "_btnNext";
		this._btnNext.TabIndex = 6;
		this._btnNext.TabStop = false;
		this._btnNext.Text = "Next >";
		this._btnNext.Click += new System.EventHandler(BtnNextClick);
		this._btnFinish.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this._btnFinish.Location = new System.Drawing.Point(360, 8);
		this._btnFinish.Name = "_btnFinish";
		this._btnFinish.TabIndex = 5;
		this._btnFinish.TabStop = false;
		this._btnFinish.Text = "&Finish";
		this._btnFinish.Click += new System.EventHandler(BtnFinishClick);
		base.Controls.Add(this._pnlButtons);
		base.Name = "AdvancedWizard";
		base.Size = new System.Drawing.Size(440, 296);
		this._pnlButtons.ResumeLayout(false);
		base.ResumeLayout(false);
	}

	private void AllowKeyPressesToNavigateWizard()
	{
		Application.AddMessageFilter(this);
	}

	private void SetButtonLocationsForOfficeLayout()
	{
		_btnHelp.Left = _pnlButtons.Width - _btnHelp.Width - 12;
		_btnCancel.Left = _btnHelp.Left - _btnCancel.Width - 5;
		_btnFinish.Left = _btnCancel.Left - _btnFinish.Width - 5;
		if (_finishButton)
		{
			_btnNext.Left = _btnFinish.Left - _btnNext.Width - 5;
		}
		else
		{
			_btnNext.Left = _btnFinish.Left;
		}
		_btnBack.Left = _btnNext.Left - _btnBack.Width;
	}

	private void SetTabOrderForOfficeLayout()
	{
		_btnHelp.TabIndex = 4;
		_btnCancel.TabIndex = 3;
		_btnBack.TabIndex = 0;
		_btnNext.TabIndex = 1;
		_btnFinish.TabIndex = 2;
	}

	private void ChangeOfficeLayout(bool hasFinishButton)
	{
		if (hasFinishButton)
		{
			_btnFinish.Visible = true;
			if (_btnHelp.Visible)
			{
				_btnCancel.Left = _btnHelp.Left - _btnCancel.Width - 5;
			}
			else
			{
				_btnCancel.Left = _btnHelp.Left;
			}
			_btnNext.Left = _btnFinish.Left - _btnFinish.Width - 5;
			_btnBack.Left = _btnNext.Left - _btnNext.Width;
			if (IndexOfCurrentPage() == _pages.Count - 1)
			{
				_btnNext.Text = _nextButtonText;
			}
		}
		else
		{
			_btnFinish.Visible = false;
			if (_btnHelp.Visible)
			{
				_btnCancel.Left = _btnHelp.Left - _btnCancel.Width - 5;
			}
			else
			{
				_btnCancel.Left = _btnHelp.Left;
			}
			_btnNext.Left = _btnFinish.Left;
			_btnBack.Left = _btnNext.Left - _btnNext.Width;
			if (IndexOfCurrentPage() == _pages.Count - 1)
			{
				_btnNext.Text = _finishButtonText;
			}
		}
	}

	private void SetButtonLocationsForDefaultLayout()
	{
		_btnFinish.Left = _pnlButtons.Width - _btnFinish.Width - 12;
		_btnNext.Left = _btnFinish.Left - _btnNext.Width - 5;
		_btnBack.Left = _btnNext.Left - _btnBack.Width - 5;
		_btnCancel.Left = _btnBack.Left - _btnCancel.Width - 5;
		_btnHelp.Left = 12;
	}

	private void SetTabOrderForDefaultLayout()
	{
		_btnHelp.TabIndex = 0;
		_btnCancel.TabIndex = 1;
		_btnBack.TabIndex = 2;
		_btnNext.TabIndex = 3;
		_btnFinish.TabIndex = 4;
	}

	private void ChangeDefaultLayout(bool hasFinishButton)
	{
		if (hasFinishButton)
		{
			_btnFinish.Visible = true;
			_btnNext.Left = _btnFinish.Left - _btnFinish.Width - 5;
			_btnBack.Left = _btnNext.Left - _btnNext.Width;
			_btnCancel.Left = _btnBack.Left - _btnBack.Width - 5;
			if (IndexOfCurrentPage() == _pages.Count - 1)
			{
				_btnNext.Text = _nextButtonText;
			}
		}
		else
		{
			_btnFinish.Visible = false;
			_btnNext.Left = _btnFinish.Left;
			_btnBack.Left = _btnNext.Left - _btnNext.Width;
			_btnCancel.Left = _btnBack.Left - _btnBack.Width - 5;
			if (IndexOfCurrentPage() == _pages.Count - 1)
			{
				_btnNext.Text = _finishButtonText;
			}
		}
	}

	private void ShowHelpAndFinishButtons()
	{
		FinishButton = true;
		HelpButton = true;
	}

	private void ProcessButtonVisibleValue(bool val)
	{
		_pnlButtons.Visible = val;
	}

	private void ProcessTouchScreenValue(bool val)
	{
		if (val)
		{
			_pnlButtons.Height = 60;
			_btnHelp.Height = 46;
			_btnCancel.Height = 46;
			_btnBack.Height = 46;
			_btnNext.Height = 46;
			_btnFinish.Height = 46;
		}
		else
		{
			_pnlButtons.Height = 40;
			_btnHelp.Height = 23;
			_btnCancel.Height = 23;
			_btnBack.Height = 23;
			_btnNext.Height = 23;
			_btnFinish.Height = 23;
		}
		_btnHelp.Top = 8;
		_btnCancel.Top = 8;
		_btnBack.Top = 8;
		_btnNext.Top = 8;
		_btnFinish.Top = 8;
	}

	private void GetSelectionService()
	{
		_selectionService = (ISelectionService)GetService(typeof(ISelectionService));
	}

	public AdvancedWizard()
	{
		InitializeComponent();
		_pages = new AdvancedWizardPageCollection();
		_wizardStrategy = WizardStrategy.CreateWizard(base.DesignMode, this);
		FlatStyle = FlatStyle.Standard;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		GetSelectionService();
		_pnlButtons.SendToBack();
		_tempNextText = NextButtonText;
		_wizardStrategy.Loading();
		AllowKeyPressesToNavigateWizard();
	}

	internal void SetButtonStates()
	{
		_wizardStrategy.SetButtonStates();
	}

	internal void StoreIndexOfCurrentPage(int index)
	{
		_pages[_selectedPage].PreviousPage = index;
	}

	internal int ReadIndexOfPreviousPage()
	{
		return _pages[_selectedPage].PreviousPage;
	}

	internal bool HasExplicitFinishButton()
	{
		return _finishButton;
	}

	internal bool HasPages()
	{
		return _pages.Count > 0;
	}

	internal int IndexOfCurrentPage()
	{
		return _selectedPage;
	}

	internal int IndexOfNextPage()
	{
		return _selectedPage + 1;
	}

	internal bool HasOnePage()
	{
		return _pages.Count == 1;
	}

	internal bool MoreThanOnePageExists()
	{
		return _pages.Count > 1;
	}

	internal bool OnFirstPage()
	{
		return _selectedPage == 0;
	}

	internal bool OnLastPage()
	{
		if (_selectedPage != _pages.Count - 1)
		{
			if (_lastPage == CurrentPage)
			{
				return CurrentPageIsFinishPage;
			}
			return false;
		}
		return true;
	}

	internal bool OnAMiddlePage()
	{
		if (!OnFirstPage())
		{
			return !OnLastPage();
		}
		return false;
	}

	internal string ReadNextText()
	{
		return _tempNextText;
	}

	internal void SelectFirstPage()
	{
		_selectedPage = 0;
		AdvancedWizardPage advancedWizardPage = _pages[_selectedPage];
		advancedWizardPage.BringToFront();
		SetButtonStates();
	}

	internal void SelectWizardPage(int index)
	{
		if (index >= 0 && index <= _pages.Count)
		{
			_selectedPage = index;
			AdvancedWizardPage advancedWizardPage = _pages[index];
			advancedWizardPage.BringToFront();
			SetButtonStates();
		}
	}

	internal void SelectWizardPage(AdvancedWizardPage page)
	{
		if (_pages.Contains(page))
		{
			_selectedPage = _pages.IndexOf(page);
			page.BringToFront();
			SetButtonStates();
		}
	}

	internal void SelectPreviousPage()
	{
		if (_selectedPage > 0)
		{
			_selectedPage--;
			AdvancedWizardPage advancedWizardPage = _pages[_selectedPage];
			advancedWizardPage.BringToFront();
			SetButtonStates();
		}
	}

	internal void SelectNextPage()
	{
		if (_selectedPage < _pages.Count - 1)
		{
			_selectedPage++;
			AdvancedWizardPage advancedWizardPage = _pages[_selectedPage];
			advancedWizardPage.BringToFront();
			SetButtonStates();
		}
	}

	internal void SetButtonText(Button b, string text)
	{
		b.Text = text;
	}

	internal void SetButtonText(string buttonName, string text)
	{
		foreach (Control control in _pnlButtons.Controls)
		{
			if (control.Name == buttonName)
			{
				control.Text = text;
			}
		}
	}

	internal bool WizardHasNoPages()
	{
		return _pages.Count == 0;
	}

	internal bool UserClickedAButtonAtDesignTime(Point point)
	{
		Control childAtPoint = GetChildAtPoint(point);
		if (childAtPoint != null && childAtPoint.Name == "_pnlButtons")
		{
			Control childAtPoint2 = childAtPoint.GetChildAtPoint(childAtPoint.PointToClient(Cursor.Position));
			if (childAtPoint2 != null)
			{
				return WizardButtonWasClicked(childAtPoint2);
			}
			return false;
		}
		return false;
	}

	internal bool WizardButtonWasClicked(Control b)
	{
		if (b is Button)
		{
			return true;
		}
		return false;
	}

	internal bool PageChangedEventAssigned()
	{
		return this.PageChanged != null;
	}

	internal void FirePageChanged(int index)
	{
		WizardPageChangedEventArgs e = new WizardPageChangedEventArgs(_pages[index], index);
		this.PageChanged(this, e);
		CurrentPageIsFinishPage = e.SetAsFinishPage;
	}

	internal bool LastPageEventAssigned()
	{
		return this.LastPage != null;
	}

	internal void FireLastPage()
	{
		this.LastPage(this, EventArgs.Empty);
	}

	internal bool NextEventAssigned()
	{
		return this.Next != null;
	}

	internal WizardEventArgs FireNextEvent(int currentTabIndex)
	{
		WizardEventArgs e = new WizardEventArgs(currentTabIndex);
		this.Next(this, e);
		return e;
	}

	internal bool BackEventAssigned()
	{
		return this.Back != null;
	}

	internal WizardEventArgs FireBackEvent(int currentTabIndex)
	{
		WizardEventArgs e = new WizardEventArgs(currentTabIndex);
		this.Back(this, e);
		return e;
	}

	internal bool FinishEventAssigned()
	{
		return this.Finish != null;
	}

	internal void FireFinishEvent()
	{
		this.Finish(this, EventArgs.Empty);
	}

	internal bool HelpEventAssigned()
	{
		return this.Help != null;
	}

	internal void FireHelpEvent()
	{
		this.Help(this, EventArgs.Empty);
	}

	internal bool CancelEventAssigned()
	{
		return this.Cancel != null;
	}

	internal void FireCancelEvent()
	{
		this.Cancel(this, EventArgs.Empty);
	}

	internal void CheckForUserChangesToEventParameters(WizardEventArgs ev, out bool allowPageToChange, out int newTabIndex)
	{
		allowPageToChange = ev.AllowPageChange;
		newTabIndex = ev.NextPageIndex;
	}

	public void GoToPage(int pageIndex)
	{
		_wizardStrategy.GoToPage(pageIndex);
	}

	public void GoToPage(AdvancedWizardPage page)
	{
		_wizardStrategy.GoToPage(page);
	}

	public void ClickNext()
	{
		_wizardStrategy.Next(_selectionService);
	}

	public void ClickBack()
	{
		_wizardStrategy.Back(_selectionService);
	}

	public void ClickFinish()
	{
		_wizardStrategy.Finish();
	}

	public void ClickCancel()
	{
		_wizardStrategy.Cancel();
	}

	public void ClickHelp()
	{
		_wizardStrategy.Help();
	}

	internal void BtnNextClick(object sender, EventArgs e)
	{
		_wizardStrategy.Next(_selectionService);
	}

	internal void BtnBackClick(object sender, EventArgs e)
	{
		_wizardStrategy.Back(_selectionService);
	}

	internal void BtnFinishClick(object sender, EventArgs e)
	{
		_wizardStrategy.Finish();
	}

	internal void BtnCancelClick(object sender, EventArgs e)
	{
		_wizardStrategy.Cancel();
	}

	internal void BtnHelpClick(object sender, EventArgs e)
	{
		_wizardStrategy.Help();
	}

	public bool PreFilterMessage(ref Message msg)
	{
		if (msg.Msg == 256 && !base.DesignMode && ProcessKeys)
		{
			if ((int)msg.WParam == 27)
			{
				_wizardStrategy.Cancel();
			}
			else if ((int)msg.WParam == 13)
			{
				if (OnLastPage())
				{
					_wizardStrategy.Finish();
				}
				else if (NextButtonEnabled)
				{
					_wizardStrategy.Next(null);
				}
			}
		}
		return false;
	}

	private void ChangeSettingsBasedOnLayout(ButtonLayoutKind value)
	{
		if (value == ButtonLayoutKind.Default)
		{
			SetButtonLocationsForDefaultLayout();
			SetTabOrderForDefaultLayout();
		}
		else
		{
			SetButtonLocationsForOfficeLayout();
			SetTabOrderForOfficeLayout();
		}
	}
}
