using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WUv4Powertools;

// Copies updates from one provider to another, rewriting the operating system portion of every
// itemID so the copy targets the destination rather than the source.
//
// An itemID is a dotted path:
//   <provider>.<product>.<platform>.<major>.<minor>.<arch>.<locale>.<edition>.<suite>.<build>.<spMajor>.<spMinor>.<namespace>.<code>.
//
// Everything from <product> through <spMinor> describes the target and has to be replaced. The
// locale, namespace and code identify the update itself and are carried across unchanged.
public static class UpdateCopier
{
	// What each provider's itemIDs look like. Service packs are the trailing pair: an empty pair
	// means the entry applies to any service pack, which is the only form 98 and ME use.
	public sealed class ProviderTarget
	{
		public string Provider;
		public string Product;
		public string Platform;
		public string Major;
		public string Minor;
		public string Build;
		public string Edition;
		public string Suite;
		public int[] ServicePacks;
		public bool IsInternetExplorer;

		public string Describe()
		{
			return Product + " (" + Platform + " " + Major + "." + Minor + ")";
		}
	}

	private static readonly Dictionary<string, ProviderTarget> Targets =
		new Dictionary<string, ProviderTarget>(StringComparer.OrdinalIgnoreCase)
	{
		{ "win98se", new ProviderTarget { Provider = "win98se", Product = "windows98andwindows98secondedition",
			Platform = "ver_platform_win32_windows", Major = "4", Minor = "10", Build = "", Edition = "", Suite = "",
			ServicePacks = new int[0] } },
		{ "winme", new ProviderTarget { Provider = "winme", Product = "windowsmillenniumedition",
			Platform = "ver_platform_win32_windows", Major = "4", Minor = "90", Build = "3000", Edition = "", Suite = "",
			ServicePacks = new int[0] } },
		{ "win2k", new ProviderTarget { Provider = "win2k", Product = "windows2000",
			Platform = "ver_platform_win32_nt", Major = "5", Minor = "0", Build = "2195", Edition = "", Suite = "",
			ServicePacks = new int[] { 0, 1, 2, 3, 4 } } },
		{ "winxp", new ProviderTarget { Provider = "winxp", Product = "windowsxp",
			Platform = "ver_platform_win32_nt", Major = "5", Minor = "1", Build = "2600", Edition = "ver_nt_workstation",
			Suite = "", ServicePacks = new int[] { 0, 1, 2, 3 } } },
		{ "netserver", new ProviderTarget { Provider = "netserver", Product = "windowsnetserver2003family",
			Platform = "ver_platform_win32_nt", Major = "5", Minor = "2", Build = "3790", Edition = "ver_nt_server",
			Suite = "", ServicePacks = new int[] { 0, 1, 2 } } },
		{ "ie50x", new ProviderTarget { Provider = "ie50x", Product = "internetexplorer5x",
			Platform = "ver_platform_win32_windows", Major = "4", Minor = "10", Build = "", Edition = "", Suite = "",
			ServicePacks = new int[0], IsInternetExplorer = true } },
		{ "ie55x", new ProviderTarget { Provider = "ie55x", Product = "internetexplorer55x",
			Platform = "ver_platform_win32_windows", Major = "4", Minor = "10", Build = "", Edition = "", Suite = "",
			ServicePacks = new int[0], IsInternetExplorer = true } },
		{ "ie60x", new ProviderTarget { Provider = "ie60x", Product = "internetexplorer6x",
			Platform = "ver_platform_win32_windows", Major = "4", Minor = "10", Build = "", Edition = "", Suite = "",
			ServicePacks = new int[0], IsInternetExplorer = true } }
	};

	public static bool IsKnown(string provider)
	{
		return provider != null && Targets.ContainsKey(provider);
	}

	public static ProviderTarget TargetFor(string provider)
	{
		ProviderTarget target;
		return Targets.TryGetValue(provider ?? string.Empty, out target) ? target : null;
	}

	public static List<string> KnownProviders()
	{
		return new List<string>(Targets.Keys);
	}

	// True when the two providers are different Internet Explorer versions. The detection block of an
	// IE update tests the installed IE version, so a copy across versions will not match and the
	// update either never offers or offers against the wrong browser.
	public static bool IsCrossInternetExplorerVersion(string source, string destination)
	{
		ProviderTarget a = TargetFor(source);
		ProviderTarget b = TargetFor(destination);
		return a != null && b != null && a.IsInternetExplorer && b.IsInternetExplorer
			&& !string.Equals(a.Provider, b.Provider, StringComparison.OrdinalIgnoreCase);
	}

	// Splits an itemID that still carries its provider prefix.
	// Returns false when it does not have the expected shape.
	public static bool TrySplit(string itemId, out string[] parts)
	{
		parts = (itemId ?? string.Empty).Split('.');
		return parts.Length >= 14;
	}

	// Rebuilds an itemID for the destination, keeping the locale, namespace and code from the source
	// and replacing everything that describes the operating system. servicePack is null for an entry
	// that applies to any service pack.
	public static string Retarget(string sourceItemId, ProviderTarget destination, int? servicePack)
	{
		string[] parts;
		if (!TrySplit(sourceItemId, out parts))
		{
			return null;
		}

		// Everything from the namespace segment onwards identifies the update rather than the target,
		// so find it rather than counting back from the end. An update code containing a dot would
		// otherwise shift the count and corrupt the result.
		int tail = -1;
		for (int i = 7; i < parts.Length; i++)
		{
			if (parts[i].StartsWith("com_", StringComparison.OrdinalIgnoreCase))
			{
				tail = i;
				break;
			}
		}
		if (tail < 0)
		{
			return null;
		}
		string locale = parts[6];
		StringBuilder sb = new StringBuilder();
		sb.Append(destination.Provider).Append('.');
		sb.Append(destination.Product).Append('.');
		sb.Append(destination.Platform).Append('.');
		sb.Append(destination.Major).Append('.');
		sb.Append(destination.Minor).Append('.');
		sb.Append("x86").Append('.');
		sb.Append(locale).Append('.');
		sb.Append(destination.Edition).Append('.');
		sb.Append(destination.Suite).Append('.');
		sb.Append(destination.Build).Append('.');
		if (servicePack.HasValue)
		{
			sb.Append(servicePack.Value.ToString(CultureInfo.InvariantCulture)).Append('.').Append('0');
		}
		else
		{
			sb.Append('.');
		}
		sb.Append('.');
		for (int i = tail; i < parts.Length; i++)
		{
			sb.Append(parts[i]);
			if (i < parts.Length - 1)
			{
				sb.Append('.');
			}
		}
		return sb.ToString();
	}

	// The service pack values to generate for a destination. An empty service pack list yields a
	// single entry that applies to any service pack, which is what 98 and ME use.
	public static List<int?> ServicePackVariants(ProviderTarget destination, IList<int> chosen)
	{
		List<int?> result = new List<int?>();
		if (destination.ServicePacks.Length == 0)
		{
			result.Add(null);
			return result;
		}
		if (chosen == null || chosen.Count == 0)
		{
			foreach (int sp in destination.ServicePacks)
			{
				result.Add(sp);
			}
			return result;
		}
		foreach (int sp in chosen)
		{
			result.Add(sp);
		}
		return result;
	}
}

// The five dictionary files of one provider, held together so a copy can be applied as a unit.
public sealed class ProviderData
{
	public string Provider;
	public List<string> Items = new List<string>();
	public List<string> ItemsIndex = new List<string>();
	public List<string> ItemStrings = new List<string>();
	public List<string> ItemStringsIndex = new List<string>();
	public List<string> Product2Items = new List<string>();
}

public sealed class CopyOutcome
{
	public int UpdatesCopied;
	public int IndexEntriesAdded;
	public int LocalesCovered;
	public List<string> Skipped = new List<string>();
}

// Performs the copy itself. Every copied update gets fresh identifiers so it cannot collide with
// anything already in the destination, while one identifier is shared across a locale group exactly
// as the rest of the application expects.
public static class UpdateCopyEngine
{
	private static readonly string[] Sep = new string[] { "@|" };

	// The locale sits at position 6 of an itemID.
	private static string LocaleOf(string itemId)
	{
		string[] parts = itemId.Split('.');
		return (parts.Length > 6) ? parts[6] : null;
	}

	// The product2items key is the itemID with the trailing namespace and code removed.
	private static string ProductKeyOf(string itemId)
	{
		int cm = itemId.IndexOf(".com_", StringComparison.OrdinalIgnoreCase);
		return (cm > 0) ? itemId.Substring(0, cm + 1) : null;
	}

	public static CopyOutcome Copy(
		string sourceProvider,
		string[] srcItems,
		string[] srcItemsIndex,
		string[] srcItemStrings,
		string[] srcItemStringsIndex,
		IList<string> updateCodes,
		ProviderData destination,
		UpdateCopier.ProviderTarget destTarget,
		IList<int> chosenServicePacks)
	{
		CopyOutcome outcome = new CopyOutcome();

		Dictionary<string, string> itemByGuid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (string line in srcItems ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			int comma = line.IndexOf(',');
			if (comma > 0) itemByGuid[line.Substring(0, comma).Trim()] = line;
		}

		Dictionary<string, string> stringsBySetGuid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (string line in srcItemStrings ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			int comma = line.IndexOf(',');
			if (comma <= 0) continue;
			string key = line.Substring(0, comma);
			int dot = key.LastIndexOf('.');
			stringsBySetGuid[((dot >= 0) ? key.Substring(dot + 1) : key).Trim()] = line;
		}

		// locale + langGuid -> stringSet guid
		Dictionary<string, string> setGuidByLocaleLang = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (string line in srcItemStringsIndex ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			int comma = line.LastIndexOf(',');
			if (comma <= 0) continue;
			string[] keyParts = line.Substring(0, comma).Split('.');
			if (keyParts.Length < 3) continue;
			setGuidByLocaleLang[keyParts[1] + "|" + keyParts[2]] = line.Substring(comma + 1).Trim();
		}

		List<int?> spVariants = UpdateCopier.ServicePackVariants(destTarget, chosenServicePacks);

		HashSet<string> existingIndex = new HashSet<string>(destination.ItemsIndex, StringComparer.OrdinalIgnoreCase);
		Dictionary<string, int> productLine = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		for (int i = 0; i < destination.Product2Items.Count; i++)
		{
			string line = destination.Product2Items[i];
			if (string.IsNullOrEmpty(line)) continue;
			int comma = line.IndexOf(',');
			productLine[((comma > 0) ? line.Substring(0, comma) : line).Trim()] = i;
		}

		foreach (string code in updateCodes)
		{
			string token = "com_microsoft." + code + ".";
			// One shared identifier for the update's string set across every locale.
			string newLangGuid = Guid.NewGuid().ToString().ToUpperInvariant();
			// Keyed by the row it came from rather than by locale. An update whose languages all share
			// one download has a single row in the source, and minting one per locale would turn that
			// into a row for every language while every copy still pointed at the same file.
			Dictionary<string, string> newGuidBySourceRow = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			// Strings belong to a language, not to a file, so each locale still gets its own set even
			// when they all share one row.
			HashSet<string> localesDone = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			bool copiedAnything = false;

			foreach (string idxLine in srcItemsIndex ?? new string[0])
			{
				if (string.IsNullOrEmpty(idxLine)) continue;
				if (idxLine.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0) continue;

				string head = idxLine.Split(Sep, StringSplitOptions.None)[0];
				int comma = head.LastIndexOf(',');
				if (comma <= 0) continue;
				string sourceItemId = head.Substring(0, comma);
				string sourceGuid = head.Substring(comma + 1).Trim();
				string locale = LocaleOf(sourceItemId);
				if (locale == null) continue;

				string sourceItem;
				if (!itemByGuid.TryGetValue(sourceGuid, out sourceItem)) continue;

				// One new row for each row in the source, so an update sharing one download across every
				// language stays a single row here too.
				string newItemGuid;
				if (!newGuidBySourceRow.TryGetValue(sourceGuid, out newItemGuid))
				{
					newItemGuid = Guid.NewGuid().ToString().ToUpperInvariant();
					newGuidBySourceRow[sourceGuid] = newItemGuid;

					string[] fields = sourceItem.Split(Sep, StringSplitOptions.None);
					fields[0] = newItemGuid + "," + code;
					if (fields.Length > 2) fields[2] = newLangGuid;
					destination.Items.Add(string.Join("@|", fields));
				}

				// Carry the titles and descriptions over under fresh identifiers, once per language.
				if (localesDone.Add(locale))
				{
					string[] sourceFields = sourceItem.Split(Sep, StringSplitOptions.None);
					string sourceLangGuid = sourceFields.Length > 2 ? sourceFields[2].Trim() : null;
					string setGuid;
					if (sourceLangGuid != null && setGuidByLocaleLang.TryGetValue(locale + "|" + sourceLangGuid, out setGuid))
					{
						string newSetGuid = Guid.NewGuid().ToString().ToUpperInvariant();
						destination.ItemStringsIndex.Add(destination.Provider + "." + locale + "." + newLangGuid + "," + newSetGuid);
						string strings;
						if (stringsBySetGuid.TryGetValue(setGuid, out strings))
						{
							int c2 = strings.IndexOf(',');
							if (c2 > 0)
							{
								destination.ItemStrings.Add(destination.Provider + "." + newSetGuid + strings.Substring(c2));
							}
						}
					}
					outcome.LocalesCovered++;
				}

				foreach (int? sp in spVariants)
				{
					string newItemId = UpdateCopier.Retarget(sourceItemId, destTarget, sp);
					if (newItemId == null) continue;
					string newIndexLine = newItemId + "," + newItemGuid + "@|";
					if (!existingIndex.Add(newIndexLine)) continue;
					destination.ItemsIndex.Add(newIndexLine);
					outcome.IndexEntriesAdded++;
					copiedAnything = true;

					// Offer it under the matching product entry, creating that entry when absent.
					string productKey = ProductKeyOf(newItemId);
					if (productKey == null) continue;
					string valueId = newItemId.Substring(destination.Provider.Length + 1);
					int lineIndex;
					if (productLine.TryGetValue(productKey, out lineIndex))
					{
						if (destination.Product2Items[lineIndex].IndexOf(valueId, StringComparison.OrdinalIgnoreCase) < 0)
						{
							destination.Product2Items[lineIndex] = destination.Product2Items[lineIndex] + "," + valueId;
						}
					}
					else
					{
						destination.Product2Items.Add(productKey + "," + valueId);
						productLine[productKey] = destination.Product2Items.Count - 1;
					}
				}
			}

			if (copiedAnything) outcome.UpdatesCopied++;
			else outcome.Skipped.Add(code);
		}

		return outcome;
	}
}

// Holds what was copied until it is pasted into another provider. In application only: the updates
// are identified by code and carried with the source rows they need, so the source tab can be closed
// in between without losing the copy.
public static class UpdateClipboard
{
	public static string SourceProvider;
	public static List<string> Codes = new List<string>();
	public static string[] Items;
	public static string[] ItemsIndex;
	public static string[] ItemStrings;
	public static string[] ItemStringsIndex;

	public static bool HasContent
	{
		get { return Codes != null && Codes.Count > 0 && Items != null; }
	}

	public static void Set(string provider, List<string> codes,
		string[] items, string[] itemsIndex, string[] itemStrings, string[] itemStringsIndex)
	{
		SourceProvider = provider;
		Codes = codes;
		Items = items;
		ItemsIndex = itemsIndex;
		ItemStrings = itemStrings;
		ItemStringsIndex = itemStringsIndex;
	}

	public static void Clear()
	{
		SourceProvider = null;
		Codes = new List<string>();
		Items = null;
		ItemsIndex = null;
		ItemStrings = null;
		ItemStringsIndex = null;
	}
}
