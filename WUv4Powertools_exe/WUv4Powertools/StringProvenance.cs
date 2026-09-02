using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WUv4Powertools;

// Remembers which translated strings came out of a real Windows Update log and which were produced
// by the translate step when an update was added by hand. The dictionary format has no room for a
// field like this, so it is kept in a plain text file beside the consumer folder. Losing the file
// only means the app stops knowing the difference, never that a string is damaged.
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
		StringProvenance store = new StringProvenance(System.IO.Path.Combine(consumerRoot, FileName));
		try
		{
			if (File.Exists(store.path))
			{
				foreach (string raw in File.ReadAllLines(store.path))
				{
					string line = raw.Trim();
					if (line.Length == 0 || line.StartsWith("#")) continue;
					store.authentic.Add(line);
				}
			}
		}
		catch
		{
			// An unreadable list only costs the app its knowledge of which strings are authentic.
		}
		return store;
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
