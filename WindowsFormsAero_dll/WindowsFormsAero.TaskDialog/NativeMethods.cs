using System;
using System.Runtime.InteropServices;

namespace WindowsFormsAero.TaskDialog;

internal class NativeMethods
{
	internal delegate IntPtr TaskDialogCallback(IntPtr hwnd, uint msg, UIntPtr wParam, IntPtr lParam, IntPtr refData);

	/// <summary>The Task Dialog config structure.</summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
	internal struct TaskDialogConfig
	{
		public uint cbSize;

		public IntPtr hwndParent;

		public IntPtr hInstance;

		public TaskDialogFlags dwFlags;

		public CommonButton dwCommonButtons;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pszWindowTitle;

		public IntPtr hMainIcon;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pszMainInstruction;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pszContent;

		public uint cButtons;

		public IntPtr pButtons;

		public int nDefaultButton;

		public uint cRadioButtons;

		public IntPtr pRadioButtons;

		public int nDefaultRadioButton;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pszVerificationText;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pszExpandedInformation;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pszExpandedControlText;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pszCollapsedControlText;

		public IntPtr hFooterIcon;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pszFooter;

		public TaskDialogCallback pfCallback;

		public IntPtr lpCallbackData;

		public uint cxWidth;
	}

	/// <summary>Flags used in TaskDialogConfig struct.</summary>
	/// <remarks>From CommCtrl.h.</remarks>
	[Flags]
	internal enum TaskDialogFlags
	{
		TDF_ENABLE_HYPERLINKS = 1,
		TDF_USE_HICON_MAIN = 2,
		TDF_USE_HICON_FOOTER = 4,
		TDF_ALLOW_DIALOG_CANCELLATION = 8,
		TDF_USE_COMMAND_LINKS = 0x10,
		TDF_USE_COMMAND_LINKS_NO_ICON = 0x20,
		TDF_EXPAND_FOOTER_AREA = 0x40,
		TDF_EXPANDED_BY_DEFAULT = 0x80,
		TDF_VERIFICATION_FLAG_CHECKED = 0x100,
		TDF_SHOW_PROGRESS_BAR = 0x200,
		TDF_SHOW_MARQUEE_PROGRESS_BAR = 0x400,
		TDF_CALLBACK_TIMER = 0x800,
		TDF_POSITION_RELATIVE_TO_WINDOW = 0x1000,
		TDF_RTL_LAYOUT = 0x2000,
		TDF_NO_DEFAULT_RADIO_BUTTON = 0x4000,
		TDF_CAN_BE_MINIMIZED = 0x8000
	}

	/// <summary>Notifications returned by Task Dialogs to the callback.</summary>
	/// <remarks>From CommCtrl.h.</remarks>
	public enum TaskDialogNotification : uint
	{
		TDN_CREATED,
		TDN_NAVIGATED,
		TDN_BUTTON_CLICKED,
		TDN_HYPERLINK_CLICKED,
		TDN_TIMER,
		TDN_DESTROYED,
		TDN_RADIO_BUTTON_CLICKED,
		TDN_DIALOG_CONSTRUCTED,
		TDN_VERIFICATION_CLICKED,
		TDN_HELP,
		TDN_EXPANDO_BUTTON_CLICKED
	}

	/// <summary>Messages that can be sent to Task Dialogs.</summary>
	/// <remarks>From CommCtrl.h.</remarks>
	public enum TaskDialogMessages : uint
	{
		TDM_NAVIGATE_PAGE = 1125u,
		TDM_CLICK_BUTTON = 1126u,
		TDM_SET_MARQUEE_PROGRESS_BAR = 1127u,
		TDM_SET_PROGRESS_BAR_STATE = 1128u,
		TDM_SET_PROGRESS_BAR_RANGE = 1129u,
		TDM_SET_PROGRESS_BAR_POS = 1130u,
		TDM_SET_PROGRESS_BAR_MARQUEE = 1131u,
		TDM_SET_ELEMENT_TEXT = 1132u,
		TDM_CLICK_RADIO_BUTTON = 1134u,
		TDM_ENABLE_BUTTON = 1135u,
		TDM_ENABLE_RADIO_BUTTON = 1136u,
		TDM_CLICK_VERIFICATION = 1137u,
		TDM_UPDATE_ELEMENT_TEXT = 1138u,
		TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE = 1139u,
		TDM_UPDATE_ICON = 1140u
	}

	/// <summary>Direct Task Dialog call.</summary>
	[DllImport("comctl32.dll", CharSet = CharSet.Unicode)]
	public static extern int TaskDialog(IntPtr hWndParent, IntPtr hInstance, string pszWindowTitle, string pszMainInstruction, string pszContent, int dwCommonButtons, IntPtr pszIcon, out int pnButton);

	/// <summary>Indirect Task Dialog call. Allows complex dialogs with interaction logic (via callback).</summary>
	[DllImport("comctl32.dll", CharSet = CharSet.Unicode)]
	public static extern IntPtr TaskDialogIndirect(ref TaskDialogConfig pTaskConfig, out int pnButton, out int pnRadioButton, out bool pfVerificationFlagChecked);
}
