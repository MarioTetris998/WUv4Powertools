using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WUv4Powertools;

// Remembers which translated strings came out of a real Windows Update log and which were produced
// by the translate step when an update was added by hand. The dictionary format has no room for a
// field like this, so it is kept in a plain text file of its own. Losing the file only means the app
// stops knowing the difference, never that a string is damaged.
//
// It used to sit in the consumer folder, which is the folder being published, so it went out with
// the inventory. It now lives with the application's own settings, one file per inventory, and a
// copy left behind in an inventory is read once and then taken out of it.
public sealed class StringProvenance
{
	private const string FileName = "authentic-strings.txt";

	private readonly HashSet<string> authentic = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	private readonly string path;

	private bool dirty;

	public int Count
	{
		get { return authentic.Count; }
	}

	public string Path
	{
		get { return path; }
	}

	private StringProvenance(string path)
	{
		this.path = path;
	}

	public static StringProvenance Load(string consumerRoot)
	{
		StringProvenance store = new StringProvenance(PathFor(consumerRoot));
		Read(store, store.path);

		// A copy left in the inventory by an older version. What it knows is kept, and the file
		// itself is taken out, since the inventory is the thing being published.
		string inTheInventory = System.IO.Path.Combine(consumerRoot ?? string.Empty, FileName);
		if (File.Exists(inTheInventory))
		{
			Read(store, inTheInventory);
			store.dirty = true;
			store.Save();
			Remove(inTheInventory);
		}
		return store;
	}

	// Reads a list into a store, ignoring anything it cannot read.
	private static void Read(StringProvenance store, string from)
	{
		try
		{
			if (!File.Exists(from)) return;

			foreach (string raw in File.ReadAllLines(from))
			{
				string line = raw.Trim();
				if (line.Length == 0 || line.StartsWith("#")) continue;

				store.authentic.Add(line);
			}
		}
		catch
		{
			// An unreadable list only costs the app its knowledge of which strings are authentic.
		}
	}

	// Takes the old copy out of the inventory. Failing to is not worth an error.
	private static void Remove(string what)
	{
		try
		{
			File.Delete(what);
		}
		catch
		{
			// Read only, or in use. It is tried again the next time the inventory is opened.
		}
	}

	// One file per inventory, so two of them never share a list. The name is a digest of the folder
	// the inventory sits in, because a path is not a file name.
	private static string PathFor(string consumerRoot)
	{
		string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		string mine = System.IO.Path.Combine(
			System.IO.Path.Combine(appData, "WUv4Powertools"), "provenance");

		string full = consumerRoot ?? string.Empty;
		try
		{
			full = System.IO.Path.GetFullPath(full);
		}
		catch
		{
			// Take the path as it was given.
		}

		StringBuilder name = new StringBuilder();
		using (SHA1 sha = SHA1.Create())
		{
			byte[] digest = sha.ComputeHash(Encoding.UTF8.GetBytes(full.ToLowerInvariant()));
			foreach (byte b in digest) name.Append(b.ToString("x2"));
		}
		return System.IO.Path.Combine(mine, name.ToString() + ".txt");
	}

	// Keyed by provider and the string GUID, which is what itemstrings rows are addressed by.
	private static string Key(string provider, string stringGuid)
	{
		return provider + "." + stringGuid;
	}

	public void MarkAuthentic(string provider, string stringGuid)
	{
		if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(stringGuid)) return;
		if (authentic.Add(Key(provider, stringGuid))) dirty = true;
	}

	public bool IsAuthentic(string provider, string stringGuid)
	{
		if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(stringGuid)) return false;
		return authentic.Contains(Key(provider, stringGuid));
	}

	public void Save()
	{
		if (!dirty) return;
		// Whatever is on disk is read again and kept. This store may have been loaded while the
		// file was unreadable, and another session may have added to it since; writing only
		// what this run holds would drop those records. That is not a harmless loss, because
		// repairing strings would then treat a genuine string as translated and overwrite it.
		HashSet<string> merged = new HashSet<string>(authentic, StringComparer.OrdinalIgnoreCase);
		try
		{
			if (File.Exists(path))
			{
				foreach (string raw in File.ReadAllLines(path))
				{
					string line = raw.Trim();
					if (line.Length == 0 || line.StartsWith("#")) continue;
					merged.Add(line);
				}
			}
		}
		catch
		{
			// The file is there but cannot be read. Replacing it with the little this run knows
			// would throw away the rest, so it is left exactly as it is.
			return;
		}

		try
		{
			string dir = System.IO.Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("# Strings taken from real Windows Update logs rather than translated.");
			sb.AppendLine("# One provider and string GUID per line. Repairing strings leaves these alone.");
			sb.AppendLine("# Delete a line to let the repair treat that string as translated again.");
			sb.AppendLine();
			List<string> ordered = new List<string>(merged);
			ordered.Sort(StringComparer.OrdinalIgnoreCase);
			foreach (string entry in ordered)
			{
				sb.AppendLine(entry);
			}
			File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
			dirty = false;
		}
		catch
		{
			// Failing to record provenance must never lose an import that already succeeded.
		}
	}
}
