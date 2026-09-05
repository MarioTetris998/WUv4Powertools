using System;
using System.IO;

namespace WUv4Powertools;

// Where the copy of a file goes before it is written over.
//
// Every save keeps what it replaced. Those copies used to sit next to the originals, which put a
// .bak beside every file in the inventory, and the inventory is the thing being published. So the
// folder they go to can be chosen, anywhere at all, and only the choice is remembered.
//
// Nothing is chosen to begin with, which keeps the copy beside the original as it always was.
public static class Backups
{
	private const string Key = "backupFolder=";

	private static string folder;

	private static bool read;

	// Where the choice itself is kept. Not in the inventory, since that is the whole point.
	private static string SettingsFile
	{
		get
		{
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			return Path.Combine(Path.Combine(appData, "WUv4Powertools"), "settings.txt");
		}
	}

	// The folder chosen, or an empty string when the copies stay beside the originals.
	public static string Folder
	{
		get
		{
			if (!read)
			{
				read = true;
				folder = ReadSetting();
			}
			return folder ?? string.Empty;
		}
		set
		{
			folder = value ?? string.Empty;
			read = true;
			WriteSetting(folder);
		}
	}

	// Where the copy of this file belongs. The provider's own folder name is kept, because every
	// provider has an items.txt and they would otherwise write over one another.
	public static string PathFor(string target)
	{
		if (string.IsNullOrEmpty(target)) return target;
		if (Folder.Length == 0) return target + ".bak";

		string providerDir = Path.GetDirectoryName(target);
		string provider = string.IsNullOrEmpty(providerDir) ? string.Empty : Path.GetFileName(providerDir);
		string into = string.IsNullOrEmpty(provider) ? Folder : Path.Combine(Folder, provider);
		return Path.Combine(into, Path.GetFileName(target) + ".bak");
	}

	// Makes the folder the copy is about to be written into, and says whether it can be used.
	public static bool Ready(string backupPath)
	{
		try
		{
			string dir = Path.GetDirectoryName(backupPath);
			if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
			return true;
		}
		catch (Exception)
		{
			// An unwritable folder must not stop the save itself, which is the point of the exercise.
			return false;
		}
	}

	// Puts the copy where it belongs, replacing whatever copy was there before.
	public static void Keep(string target)
	{
		try
		{
			if (!File.Exists(target)) return;

			string backup = PathFor(target);
			if (!Ready(backup)) return;

			File.Copy(target, backup, true);
		}
		catch (Exception)
		{
			// Losing the copy is not worth losing the save over.
		}
	}

	private static string ReadSetting()
	{
		try
		{
			if (!File.Exists(SettingsFile)) return string.Empty;

			foreach (string raw in File.ReadAllLines(SettingsFile))
			{
				string line = raw.Trim();
				if (!line.StartsWith(Key, StringComparison.OrdinalIgnoreCase)) continue;

				return line.Substring(Key.Length).Trim();
			}
		}
		catch (Exception)
		{
			// No setting readable, so the copies stay beside the originals.
		}
		return string.Empty;
	}

	private static void WriteSetting(string value)
	{
		try
		{
			string dir = Path.GetDirectoryName(SettingsFile);
			if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
			File.WriteAllLines(SettingsFile, new string[] { Key + value });
		}
		catch (Exception)
		{
			// The choice will not survive a restart, which is better than refusing to save.
		}
	}
}
