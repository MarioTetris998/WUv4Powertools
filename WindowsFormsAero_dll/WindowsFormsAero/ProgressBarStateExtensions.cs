using WindowsFormsAero.Native;

namespace WindowsFormsAero;

internal static class ProgressBarStateExtensions
{
	/// <summary>
	/// Converts a <see cref="T:WindowsFormsAero.ProgressBarState" /> value into a native
	/// Win32 value, represented by <see cref="T:WindowsFormsAero.Native.ProgressBarState" />.
	/// </summary>
	public static WindowsFormsAero.Native.ProgressBarState ToNative(this ProgressBarState state)
	{
		return state switch
		{
			ProgressBarState.Error => WindowsFormsAero.Native.ProgressBarState.Error, 
			ProgressBarState.Paused => WindowsFormsAero.Native.ProgressBarState.Paused, 
			_ => WindowsFormsAero.Native.ProgressBarState.Normal, 
		};
	}
}
