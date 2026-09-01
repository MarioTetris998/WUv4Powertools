using System;
using System.Collections.Generic;

namespace WUv4Powertools;

// Remembers which updates the last import touched, so the update list can pick them out. It lives
// only as long as the application is running: this is a "here is what just happened" marker, not a
// property of the dictionary, and it would be wrong to write it into the files.
public static class LogImportHighlight
{
	// provider -> the codes that import added or corrected.
	private static readonly Dictionary<string, HashSet<string>> touched =
		new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

	public static int Count
	{
		get
		{
			int total = 0;
			foreach (HashSet<string> codes in touched.Values) total += codes.Count;
			return total;
		}
	}

	// Called at the start of an import so the previous run's marks do not linger.
	public static void Clear()
	{
		touched.Clear();
	}

	public static void Add(string provider, string code)
	{
		if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(code)) return;
		if (!touched.ContainsKey(provider))
		{
			touched[provider] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}
		touched[provider].Add(code);
	}

	public static bool WasJustImported(string provider, string code)
	{
		if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(code)) return false;
		HashSet<string> codes;
		return touched.TryGetValue(provider, out codes) && codes.Contains(code);
	}

	public static bool HasAnyFor(string provider)
	{
		HashSet<string> codes;
		return !string.IsNullOrEmpty(provider) && touched.TryGetValue(provider, out codes) && codes.Count > 0;
	}
}
