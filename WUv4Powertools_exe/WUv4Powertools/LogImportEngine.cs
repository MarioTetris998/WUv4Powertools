using System;
using System.Collections.Generic;
using System.Linq;

namespace WUv4Powertools;

// Reads one provider's five dictionaries into the shape the importer needs to ask two questions:
// does this code already exist, and if so which languages does it already carry.
public sealed class ProviderIndex
{
	public readonly string Provider;

	// code -> locale -> the full itemID recorded in itemsindex.
	private readonly Dictionary<string, Dictionary<string, string>> byCode =
		new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

	// code -> locale -> the file GUID that itemsindex points at.
	private readonly Dictionary<string, Dictionary<string, string>> guidByCode =
		new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

	// file GUID -> the items.txt line it heads.
	private readonly Dictionary<string, string> itemsByGuid =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	// language GUID -> locale -> the string GUID holding that language's title.
	private readonly Dictionary<string, Dictionary<string, string>> stringLocales =
		new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

	// string GUID -> the title currently stored for it, so a log title can be compared with what
	// is already there before anything is rewritten.
	private readonly Dictionary<string, string> titleByStringGuid =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	// string GUID -> the description stored for it. An import is refused when this would end up
	// empty, because the logs cannot supply one.
	private readonly Dictionary<string, string> eulaByStringGuid =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, string> detailsByStringGuid =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, string> descriptionByStringGuid =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	// The category most of this provider's updates sit in, used for an update that arrives with
	// no local record to copy one from.
	private readonly Dictionary<string, int> groupTally = new Dictionary<string, int>();

	public ProviderIndex(string provider, string[] items, string[] itemsIndex, string[] itemStringsIndex,
		string[] itemStrings = null)
	{
		Provider = provider;

		foreach (string line in items ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			int comma = line.IndexOf(',');
			if (comma <= 0) continue;
			itemsByGuid[line.Substring(0, comma).Trim()] = line;

			string[] fields = line.Split(new[] { "@|" }, StringSplitOptions.None);
			if (fields.Length >= 4 && !string.IsNullOrEmpty(fields[3]))
			{
				if (!groupTally.ContainsKey(fields[3])) groupTally[fields[3]] = 0;
				groupTally[fields[3]]++;
			}
		}

		foreach (string line in itemStrings ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			int comma = line.IndexOf(',');
			if (comma <= 0) continue;
			string head = line.Substring(0, comma);
			int dot = head.LastIndexOf('.');
			if (dot < 0) continue;
			string stringGuid = head.Substring(dot + 1).Trim();
			string[] cells = line.Substring(comma + 1).Split(new[] { "@|" }, StringSplitOptions.None);
			titleByStringGuid[stringGuid] = cells[0];
			descriptionByStringGuid[stringGuid] = cells.Length > 1 ? cells[1] : string.Empty;
			// A row too short to hold these has no such field at all, which is not the same as
			// holding an empty one. Reporting it as empty offered a correction on a row that could
			// not take one, so it was counted and then never written.
			eulaByStringGuid[stringGuid] = cells.Length > 2 ? cells[2] : null;
			detailsByStringGuid[stringGuid] = cells.Length > 4 ? cells[4] : null;
		}

		foreach (string line in itemsIndex ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			string head = line.Split(new[] { "@|" }, StringSplitOptions.None)[0];
			int comma = head.LastIndexOf(',');
			if (comma <= 0) continue;
			string itemId = head.Substring(0, comma).Trim();
			string guid = head.Substring(comma + 1).Trim();
			string[] parts = itemId.Split('.');
			if (parts.Length < 15) continue;

			string code = parts[13];
			string locale = parts[6];
			if (!byCode.ContainsKey(code))
			{
				byCode[code] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				guidByCode[code] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			}
			byCode[code][locale] = itemId;
			guidByCode[code][locale] = guid;
		}

		foreach (string line in itemStringsIndex ?? new string[0])
		{
			if (string.IsNullOrEmpty(line)) continue;
			int comma = line.LastIndexOf(',');
			if (comma <= 0) continue;
			string[] key = line.Substring(0, comma).Split('.');
			if (key.Length < 3) continue;
			string locale = key[1];
			string langGuid = key[2];
			if (!stringLocales.ContainsKey(langGuid))
			{
				stringLocales[langGuid] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			}
			stringLocales[langGuid][locale] = line.Substring(comma + 1).Trim();
		}
	}

	public bool HasCode(string code)
	{
		return byCode.ContainsKey(code);
	}

	public bool HasLocale(string code, string locale)
	{
		return byCode.ContainsKey(code) && byCode[code].ContainsKey(locale);
	}

	public IEnumerable<string> LocalesFor(string code)
	{
		return byCode.ContainsKey(code) ? byCode[code].Keys : Enumerable.Empty<string>();
	}

	// The entry an import copies its detection, group and installer settings from. English is
	// preferred only because it is the one language every provider reliably carries.
	public string SiblingItemId(string code)
	{
		if (!byCode.ContainsKey(code)) return null;
		Dictionary<string, string> locales = byCode[code];
		foreach (string preferred in new[] { "en", "de", "fr" })
		{
			if (locales.ContainsKey(preferred)) return locales[preferred];
		}
		return locales.Values.FirstOrDefault();
	}

	public string SiblingItemLine(string code)
	{
		if (!guidByCode.ContainsKey(code)) return null;
		Dictionary<string, string> guids = guidByCode[code];
		foreach (string preferred in new[] { "en", "de", "fr" })
		{
			if (guids.ContainsKey(preferred) && itemsByGuid.ContainsKey(guids[preferred])) return itemsByGuid[guids[preferred]];
		}
		foreach (string guid in guids.Values)
		{
			if (itemsByGuid.ContainsKey(guid)) return itemsByGuid[guid];
		}
		return null;
	}

	// Which language tag each locale uses in its download file names, learned from the updates the
	// provider already holds. A fixed table would get this wrong, because the same folder runs
	// Windows98-KB918547-ENU next to 888113USA8, and those two families disagree about nearly every
	// language: ENU against USA, CSY against CZE, DEU against GER, ESN against SPA.
	private Dictionary<string, Dictionary<string, Dictionary<string, int>>> family;

	private Dictionary<string, Dictionary<string, int>> localeTally;

	private void BuildNaming()
	{
		family = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>(StringComparer.OrdinalIgnoreCase);
		localeTally = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);

		foreach (KeyValuePair<string, Dictionary<string, string>> code in guidByCode)
		{
			Dictionary<string, string> tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (KeyValuePair<string, string> pair in code.Value)
			{
				string line;
				if (!itemsByGuid.TryGetValue(pair.Value, out line)) continue;
				string leaf = LogImportEngine.LeafOfLine(line);
				if (leaf == null) continue;
				string token = LogImportParser.LanguageTokenOf(leaf);
				if (token == null) continue;
				tokens[pair.Key] = token;
				Bump(localeTally, pair.Key, token);
			}

			if (tokens.Count < 2) continue;
			foreach (KeyValuePair<string, string> from in tokens)
			{
				if (!family.ContainsKey(from.Value))
				{
					family[from.Value] = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);
				}
				foreach (KeyValuePair<string, string> to in tokens)
				{
					if (string.Equals(from.Key, to.Key, StringComparison.OrdinalIgnoreCase)) continue;
					Bump(family[from.Value], to.Key, to.Value);
				}
			}
		}
	}

	private static void Bump(Dictionary<string, Dictionary<string, int>> map, string key, string value)
	{
		if (!map.ContainsKey(key)) map[key] = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		if (!map[key].ContainsKey(value)) map[key][value] = 0;
		map[key][value]++;
	}

	private static string Best(Dictionary<string, int> counts)
	{
		if (counts == null || counts.Count == 0) return null;
		return counts.OrderByDescending(x => x.Value).ThenBy(x => x.Key).First().Key;
	}

	// The tag this locale would use in a file named like the one given. Preference goes to a family
	// that actually pairs the two, then to whatever this locale uses most often in this provider.
	public string TokenFor(string locale, string siblingToken)
	{
		if (family == null) BuildNaming();
		if (string.IsNullOrEmpty(locale)) return null;

		if (siblingToken != null && family.ContainsKey(siblingToken) && family[siblingToken].ContainsKey(locale))
		{
			return Best(family[siblingToken][locale]);
		}
		if (localeTally.ContainsKey(locale)) return Best(localeTally[locale]);
		return null;
	}

	// The category to file a brand new update under, taken from wherever most of this provider's
	// updates already sit.
	// This provider's own spelling of a code, when it holds it. Used to tie a log download to
	// an update by the code the log states, rather than by anything inferred.
	public IEnumerable<string> CodesMatching(string code)
	{
		if (string.IsNullOrEmpty(code)) yield break;
		foreach (string known in byCode.Keys)
		{
			if (string.Equals(known, code, StringComparison.OrdinalIgnoreCase)) yield return known;
		}
	}

	public string MostCommonGroup()
	{
		if (groupTally.Count == 0) return "0";
		return groupTally.OrderByDescending(x => x.Value).ThenBy(x => x.Key).First().Key;
	}

	public string ItemIdFor(string code, string locale)
	{
		Dictionary<string, string> locales;
		if (!byCode.TryGetValue(code ?? string.Empty, out locales)) return null;
		string id;
		return locales.TryGetValue(locale ?? string.Empty, out id) ? id : null;
	}

	public string ItemGuidFor(string code, string locale)
	{
		Dictionary<string, string> locales;
		if (!guidByCode.TryGetValue(code ?? string.Empty, out locales)) return null;
		string guid;
		return locales.TryGetValue(locale ?? string.Empty, out guid) ? guid : null;
	}

	// Whether some row already carries this GUID. Two rows sharing one identity would leave the
	// index unable to tell them apart, so a correction onto a taken GUID is never offered.
	public bool HasItemGuid(string guid)
	{
		return !string.IsNullOrEmpty(guid) && itemsByGuid.ContainsKey(guid);
	}

	// How many languages share the row this language points at. One file serving every language
	// is held as a single row, and anything written on it is written for all of them at once.
	public int LanguagesOnRowFor(string code, string locale)
	{
		string guid = ItemGuidFor(code, locale);
		if (string.IsNullOrEmpty(guid)) return 0;

		Dictionary<string, string> locales;
		if (!guidByCode.TryGetValue(code ?? string.Empty, out locales)) return 0;

		int sharing = 0;
		foreach (KeyValuePair<string, string> pair in locales)
		{
			if (string.Equals(pair.Value, guid, StringComparison.OrdinalIgnoreCase)) sharing++;
		}

		return sharing;
	}

	public string ItemsLineFor(string code, string locale)
	{
		string guid = ItemGuidFor(code, locale);
		if (guid == null) return null;
		string line;
		return itemsByGuid.TryGetValue(guid, out line) ? line : null;
	}

	// The language GUID an update's string set is keyed by, read from any of its items rows.
	public string LangGuidFor(string code)
	{
		string line = SiblingItemLine(code);
		if (line == null) return null;
		string[] fields = line.Split(new[] { "@|" }, StringSplitOptions.None);
		return fields.Length >= 3 ? fields[2] : null;
	}

	// True when this language already has description text, which is the only way an imported
	// entry can end up with one.
	public bool HasDescriptionFor(string code, string locale)
	{
		string langGuid = LangGuidFor(code);
		if (langGuid == null) return false;
		string stringGuid = StringGuidFor(langGuid, locale);
		if (stringGuid == null) return false;
		string description;
		return descriptionByStringGuid.TryGetValue(stringGuid, out description) &&
			!string.IsNullOrWhiteSpace(description);
	}

	// The licence address a string row holds, or null when there is no such row.
	public string EulaForStringGuid(string stringGuid)
	{
		string value;
		return eulaByStringGuid.TryGetValue(stringGuid ?? string.Empty, out value) ? value : null;
	}

	// The more information address a string row holds, or null when there is no such row.
	public string DetailsForStringGuid(string stringGuid)
	{
		string value;
		return detailsByStringGuid.TryGetValue(stringGuid ?? string.Empty, out value) ? value : null;
	}

	public string TitleForStringGuid(string stringGuid)
	{
		if (string.IsNullOrEmpty(stringGuid)) return null;
		string title;
		return titleByStringGuid.TryGetValue(stringGuid, out title) ? title : null;
	}

	public bool HasStringFor(string langGuid, string locale)
	{
		return StringGuidFor(langGuid, locale) != null;
	}

	// The string GUID this language's title lives under, or null when it has none yet.
	public string StringGuidFor(string langGuid, string locale)
	{
		if (string.IsNullOrEmpty(langGuid) || string.IsNullOrEmpty(locale)) return null;
		Dictionary<string, string> byLocale;
		if (!stringLocales.TryGetValue(langGuid, out byLocale)) return null;
		string guid;
		return byLocale.TryGetValue(locale, out guid) ? guid : null;
	}
}

public sealed class ImportSummary
{
	public int ItemsAdded;

	public int IndexEntriesAdded;

	// Languages given a row that was already there, because the file is the same one.
	public int LanguagesSharingAFile;

	// Rows removed because another row of the same update already held that file.
	public int RowsMerged;

	// Languages an update was offered in because one file serves all of them.
	public int LanguagesFilledIn;

	// Licence and more information addresses put right.
	public int LinksCorrected;

	public int StringsAdded;

	public int ProductLinksAdded;

	public int Skipped;

	// Entries whose download had to be worked out from how the provider names its other languages,
	// because no log recorded one.
	public int GuessedNames;

	public readonly List<string> Guesses = new List<string>();

	// Titles replaced with the authentic ones the service published, in place of whatever the
	// translate step had put there.
	public int TitlesCorrected;

	// Rows already in the provider whose item GUID was put back to the published one.
	public int GuidsCorrected;

	// Rows added using the GUID the update was really published under, rather than a fresh one.
	public int GuidsFromLog;

	public int VersionsCorrected;

	// Imported rows with no published date available, because only a catalogue file carries one.
	public int WithoutPostedDate;

	// Updates the provider had never held, brought in as drafts needing a detection rule.
	public int NewUpdatesAdded;

	// Records already present that the log put right.
	public int Corrected;

	public int FileNamesCorrected;

	public int DatesCorrected;

	// Updates whose code was respelled to the capitals the service published.
	public int CodesRecased;

	// Records whose restart flag was set to what the log watched happen.
	public int RebootFlagsCorrected;

	// Records now run the way the log watched them being installed.
	public int CommandTypesCorrected;

	public readonly List<string> Notes = new List<string>();

	// Provider name to the number of updates it gained, so the result can be reported per operating
	// system rather than as one total.
	public readonly Dictionary<string, int> ByProvider =
		new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

	// Providers written straight to disk because no tab had them open.
	public readonly List<string> Written = new List<string>();

	// Providers that were open in a tab and were refreshed on screen as well as written.
	public readonly List<string> LeftOpen = new List<string>();

	public readonly List<string> WriteErrors = new List<string>();

	public void Count(string provider)
	{
		if (!ByProvider.ContainsKey(provider)) ByProvider[provider] = 0;
		ByProvider[provider]++;
	}
}

public static class LogImportEngine
{
	private static readonly string[] Sep = { "@|" };

	// The language an entry imports into. The history file states one per entry, so a set of
	// files covering several languages still lands in the right place. The chosen language is
	// only used for entries that never stated one, unless the caller says detection was wrong.
	private static string LocaleFor(ImportCandidate c, string chosen, bool overrideLanguage)
	{
		if (overrideLanguage) return chosen;
		return string.IsNullOrEmpty(c.Language) ? chosen : c.Language;
	}

	// Works out what would happen to every candidate without changing anything, so the dialog can
	// show a reason for each row before the user commits. Each candidate is judged against the
	// provider it names, so one history file feeds every operating system and Internet Explorer
	// version in the folder at once.
	public static void Classify(List<ImportCandidate> candidates, ConsumerDictionary dictionary,
		SupersededList superseded, string chosenLanguage, bool overrideLanguage)
	{
		foreach (ImportCandidate c in candidates)
		{
			string locale = LocaleFor(c, chosenLanguage, overrideLanguage);
			ProviderStore store = dictionary.Find(c.Provider);

			if (store == null)
			{
				c.Kind = ImportKind.OtherProvider;
				c.Reason = c.Provider + " is not in this inventory";
				c.Selected = false;
				continue;
			}

			if (superseded != null && superseded.Excludes(c.Provider, c.Code))
			{
				c.Kind = ImportKind.Superseded;
				c.Reason = "listed in superseded.txt";
				c.Selected = false;
				continue;
			}

			ProviderIndex index = store.Index;

			if (index.HasLocale(c.Code, locale))
			{
				// The update is here, but the log may still know the real GUID, title, version or file
				// name for it. Those are offered as corrections rather than applied unasked, because
				// they rewrite a record that already works.
				string existingLine = index.ItemsLineFor(c.Code, locale);
				string existingId = index.ItemIdFor(c.Code, locale);
				string langGuid = index.LangGuidFor(c.Code);
				string stringGuid = langGuid == null ? null : index.StringGuidFor(langGuid, locale);
				string existingTitle = index.TitleForStringGuid(stringGuid);

				c.Fix = LogImportNewItems.Compare(c, locale, index, existingLine, existingId, existingTitle);

				// The licence and the more information address are stated by the source for this exact
				// language, and both are held per language, so a row carrying another language's is put
				// right. These used to be written only when the title happened to change as well.
				c.Fix.Eula = LogImportNewItems.LinkDiffers(StatedEula(c), index.EulaForStringGuid(stringGuid));
				c.Fix.Details = LogImportNewItems.LinkDiffers(c.DetailsHref, index.DetailsForStringGuid(stringGuid));

				// One file serving several languages is a single row, and a row can carry only one
				// identifier and one restart flag. The log states those for each language separately,
				// so offering them here would put right what one language says and leave the next
				// language asking for the opposite, over and over. They are only offered where the row
				// belongs to this language alone.
				if (index.LanguagesOnRowFor(c.Code, locale) > 1)
				{
					c.Fix.Guid = false;
					c.Fix.Reboot = false;
				}
				if (c.Fix.Any)
				{
					c.Kind = ImportKind.Correction;
					c.Reason = "in " + locale + ", can correct " + c.Fix;
				}
				else
				{
					c.Kind = ImportKind.AlreadyPresent;
					c.Reason = "already in " + locale;
				}
				// A correction only puts right what is already there, so it is offered ticked. Only an
				// update arriving without a detection rule is left for the user to choose deliberately.
				c.Selected = c.Kind == ImportKind.Correction;
				continue;
			}

			// An update with no description here and none in the logs would come in blank, so it is
			// left out rather than written half finished.
			if (!index.HasDescriptionFor(c.Code, locale))
			{
				c.Kind = ImportKind.NoDescription;
				c.Reason = index.HasCode(c.Code)
					? "no description for " + locale + " here, and logs carry none"
					: "new, and logs carry no description";
				c.Selected = false;
				continue;
			}

			if (index.HasCode(c.Code))
			{
				c.Kind = ImportKind.LanguageGap;
				c.Reason = "missing " + locale + ", have " +
					string.Join("/", index.LocalesFor(c.Code).OrderBy(x => x).Take(6).ToArray());
				c.Selected = true;
				continue;
			}

			c.Kind = ImportKind.NewUpdate;
			c.Reason = string.IsNullOrEmpty(c.DownloadUrl)
				? "new, comes in without a download or a detection rule"
				: "new, comes in without a detection rule";
			// Never ticked on the user's behalf. An update with no detection rule needs work before
			// it is any use, so bringing one in is always a deliberate choice.
			c.Selected = false;
		}
	}

	// Applies the ticked candidates across every provider they belong to. Providers open in a tab are
	// edited in memory and left for the user to save, the rest are written to disk here.
	public static ImportSummary Apply(List<ImportCandidate> candidates, ConsumerDictionary dictionary,
		string chosenLanguage, bool overrideLanguage, StringProvenance provenance)
	{
		ImportSummary summary = new ImportSummary();
		if (provenance == null) provenance = StringProvenance.Load(dictionary.Root);

		// Marks from an earlier run would only confuse which updates this one touched.
		LogImportHighlight.Clear();

		Dictionary<string, List<ImportCandidate>> byProvider = candidates
			.Where(x => x.Selected)
			.GroupBy(x => x.Provider, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

		foreach (KeyValuePair<string, List<ImportCandidate>> pair in byProvider)
		{
			if (dictionary.Find(pair.Key) != null) continue;

			summary.Skipped += pair.Value.Count;
			summary.Notes.Add(pair.Key + " is not in this inventory, so its updates were left out");
		}

		// Every provider in the folder, not only those a log had something to say about. Putting an
		// update that uses one file onto a single row is worth doing whether or not anything about it
		// needed correcting, and an update that is already correct produces no candidate at all, so
		// its provider was never even looked at.
		foreach (ProviderStore store in dictionary.Providers)
		{
			List<ImportCandidate> mine;
			if (!byProvider.TryGetValue(store.Name, out mine)) mine = new List<ImportCandidate>();

			// An open provider has to be able to take the whole import back in one step.
			if (store.OpenTab != null) store.OpenTab.PushUndoState();

			// Counted across both kinds of change. Watching only the number of rows added used to
			// drop a provider whose updates were all corrections, since correcting adds no rows: the
			// work was done and then never pushed to the tab or written to disk.
			// Counted across every kind of change. Watching only what was added or corrected used to
			// drop a provider whose only change was rows being put together, and that work was then
			// never written out.
			int before = summary.ItemsAdded + summary.Corrected + summary.RowsMerged +
				summary.LanguagesFilledIn;
			ApplyToProvider(mine, store, chosenLanguage, overrideLanguage, summary, provenance);
			if (summary.ItemsAdded + summary.Corrected + summary.RowsMerged +
				summary.LanguagesFilledIn == before) continue;

			store.Dirty = true;
			store.InvalidateIndex();

			// An import applies to the inventory itself rather than leaving an edit to be saved later,
			// so every provider it touches is written now. A provider open in a tab is refreshed from
			// the same arrays and marked saved, so the tab and the files on disk cannot drift apart.
			if (store.OpenTab != null)
			{
				store.PushToTab();
				summary.LeftOpen.Add(store.Name);
			}

			try
			{
				store.Save();
				summary.Written.Add(store.Name);
				if (store.OpenTab != null) store.OpenTab.MarkSaved();
			}
			catch (Exception ex)
			{
				summary.WriteErrors.Add(store.Name + ": " + ex.Message);
			}
		}

		provenance.Save();
		return summary;
	}

	private static void ApplyToProvider(List<ImportCandidate> candidates, ProviderStore store,
		string chosenLanguage, bool overrideLanguage, ImportSummary summary, StringProvenance provenance)
	{
		List<string> items = new List<string>(store.Items ?? new string[0]);
		List<string> itemsIndex = new List<string>(store.ItemsIndex ?? new string[0]);
		List<string> strings = new List<string>(store.ItemStrings ?? new string[0]);
		List<string> stringsIndex = new List<string>(store.ItemStringsIndex ?? new string[0]);
		List<string> product2Items = new List<string>(store.Product2Items ?? new string[0]);

		// Every update holding one address on more than one row is put onto a single row first, so
		// what follows reads and corrects the row that will still be there afterwards. This covers
		// the whole inventory rather than only the updates a log happened to mention: an update
		// that already uses one file for every language needs no correcting, so it was never
		// looked at, and its languages went on sitting in a row each holding the same download.
		summary.RowsMerged += MergeRowsSharingAFile(items, itemsIndex, null);

		ProviderIndex index = new ProviderIndex(store.Name, items.ToArray(), itemsIndex.ToArray(),
			stringsIndex.ToArray(), strings.ToArray());

		// Every language of the same new update has to share one string set, so the GUID minted for
		// the first language is reused by the rest.
		Dictionary<string, string> newLangGuids =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		// Identifiers the provider already carries, so a second row is never written for a language
		// that has one. Several entries can land on the same language: forcing an override collapses
		// every language of an update onto one, and a log can record the same download repeatedly.
		HashSet<string> takenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string entry in itemsIndex)
		{
			if (string.IsNullOrEmpty(entry)) continue;
			string entryHead = entry.Split(Sep, StringSplitOptions.None)[0];
			int entryComma = entryHead.LastIndexOf(',');
			if (entryComma > 0) takenIds.Add(entryHead.Substring(0, entryComma));
		}

		foreach (ImportCandidate c in candidates)
		{
			string locale = LocaleFor(c, chosenLanguage, overrideLanguage);
			if (string.IsNullOrEmpty(locale))
			{
				summary.Skipped++;
				summary.Notes.Add(c.Provider + ": " + c.Code +
					" skipped, the language it belongs to could not be worked out");
				continue;
			}

			if (c.Kind == ImportKind.Correction)
			{
				ApplyCorrection(c, locale, store, index, items, itemsIndex, strings, product2Items,
				summary, provenance);
				continue;
			}

			string sibling = index.SiblingItemLine(c.Code);
			string siblingId = index.SiblingItemId(c.Code);

			if (sibling == null || siblingId == null)
			{
				// The provider has never held this update, so there is nothing to copy a detection rule
				// from. It comes in with a placeholder rule that matches nothing, which keeps it out of
				// the way of real machines and marks it in the list as still needing work.
				AddNewUpdate(c, locale, store, index, items, itemsIndex, strings, stringsIndex,
					product2Items, newLangGuids, takenIds, summary, provenance);
				continue;
			}

			string[] fields = sibling.Split(Sep, StringSplitOptions.None);
			if (fields.Length < 14)
			{
				summary.Skipped++;
				summary.Notes.Add(store.Name + ": " + c.Code + " skipped, the entry it would copy is malformed");
				continue;
			}

			// The log records the GUID the update was actually published under for this language, so
			// it is used rather than a fresh one. That makes the row match what the service served.
			string newGuid = string.IsNullOrEmpty(c.ItemGuid)
				? Guid.NewGuid().ToString().ToUpper()
				: c.ItemGuid;
			if (!string.IsNullOrEmpty(c.ItemGuid)) summary.GuidsFromLog++;
			string langGuid = fields[2];

			// Only the locale is taken from the log. Every other part of the identifier comes from
			// the entry already in the provider, so an import can never retarget an update at a
			// different operating system by accident.
			// The version is the update's own, not the operating system's, so it comes from the log
			// when there is one. Everything else still comes from the sibling.
			string newItemId = ReplaceLocale(siblingId, locale);
			if (!string.IsNullOrEmpty(c.Version))
			{
				string withVersion = ReplaceVersion(newItemId, c.Version);
				if (withVersion != newItemId) summary.VersionsCorrected++;
				newItemId = withVersion;
			}

			// The installation block keeps the sibling's installer type, switches and reboot flag,
			// and only the download itself is swapped for the localised one.
			string installation = fields[5];
			if (!string.IsNullOrEmpty(c.DownloadUrl))
			{
				installation = SetDownload(installation, c.DownloadUrl, c.FileName, c.Size, c.CleanFileName);
			}
			else
			{
				// No log recorded this download, so the file is named the way the provider names the
				// same update in its other languages. A name with no language tag in it is one file
				// serving every language and is left exactly as it is.
				string siblingLeaf = LeafOfLine(sibling);
				string siblingToken = siblingLeaf == null ? null : LogImportParser.LanguageTokenOf(siblingLeaf);
				if (siblingToken != null)
				{
					string wanted = index.TokenFor(locale, siblingToken);
					string guessedLeaf = wanted == null ? null : LogImportParser.SwapLanguageToken(siblingLeaf, wanted);
					if (guessedLeaf != null)
					{
						installation = SetDownload(installation, null, guessedLeaf, 0);
						summary.GuessedNames++;
						if (summary.Guesses.Count < 40)
						{
							summary.Guesses.Add(store.Name + " " + locale + ": " + LogImportParser.StripHash(guessedLeaf));
						}
					}
				}
			}

			// Every language was published at its own moment, so a published date belongs to this
			// language's row alone. It is written into the copy being added, and the entry it was
			// copied from keeps its own, which is why the sibling is never edited here.
			// Only a catalogue entry carries a published date. A plain history file records when the
			// machine downloaded the update, which is not the same thing and is never written.
			string stamp = c.HasPostedDate ? c.Timestamp : fields[9];
			if (!c.HasPostedDate) summary.WithoutPostedDate++;

			string[] newFields = (string[])fields.Clone();
			newFields[0] = newGuid + "," + c.Code;
			newFields[5] = installation;
			newFields[9] = stamp;
			if (c.Size > 0)
			{
				newFields[8] = c.Size.ToString();
			}

			// One file serving several languages belongs in a single row that they all point at. Where
			// the file this language would use is one another row of the same update already carries,
			// that row answers for this language too and no second row holding the same download is
			// written. The name is weighed without its case and without the cabpool hash.
			string shared = RowWithSameFile(items, c.Code, string.Join("@|", newFields));
			if (shared != null)
			{
				string[] sharedFields = shared.Split(Sep, StringSplitOptions.None);
				int sharedComma = sharedFields[0].IndexOf(',');
				if (sharedComma > 0)
				{
					newGuid = sharedFields[0].Substring(0, sharedComma).Trim();
					if (sharedFields.Length > 2) langGuid = sharedFields[2];
				}
				else
				{
					shared = null;
				}
			}

			// Another entry has already given this language a row, so adding a second would leave two
			// records under one identifier and the catalogue unable to tell them apart.
			if (!takenIds.Add(newItemId))
			{
				summary.Skipped++;
				summary.Notes.Add(store.Name + ": " + c.Code + " (" + locale +
					") skipped, another entry has already given this language a row");
				continue;
			}

			if (shared == null)
			{
				items.Add(string.Join("@|", newFields));
				summary.ItemsAdded++;
			}
			else
			{
				summary.LanguagesSharingAFile++;
			}
			summary.Count(store.Name);
			LogImportHighlight.Add(store.Name, c.Code);

			itemsIndex.Add(newItemId + "," + newGuid + "@|");
			summary.IndexEntriesAdded++;

			// The version belongs to the update, so the languages already here are brought onto the
			// same one rather than being left behind by the language just added.
			if (!string.IsNullOrEmpty(c.Version))
			{
				SetVersionEverywhere(itemsIndex, product2Items, c.Code, c.Version);
			}

			// The title in the log is the one the service actually published, so it replaces whatever
			// is there now, which for most languages was produced by the translate step. The row is
			// recorded as authentic so a later repair knows not to touch it.
			if (LogImportNewItems.TitleUsable(c, locale))
			{
				string existingGuid = index.StringGuidFor(langGuid, locale);
				if (existingGuid != null)
				{
					if (ReplaceTitle(strings, store.Name, existingGuid, c.Title, c.DetailsHref))
					{
						summary.TitlesCorrected++;
					}
					provenance.MarkAuthentic(store.Name, existingGuid);
				}
				else
				{
					string stringGuid = Guid.NewGuid().ToString().ToUpper();
					stringsIndex.Add(store.Name + "." + locale + "." + langGuid + "," + stringGuid);
					strings.Add(string.Format("{0}.{1},{2}@|{3}@|{4}@|@|{5}",
						store.Name, stringGuid, c.Title, string.Empty, EulaFor(c, locale), c.DetailsHref));
					summary.StringsAdded++;
					provenance.MarkAuthentic(store.Name, stringGuid);
				}
			}

			if (AddProductLink(product2Items, store.Name, newItemId)) summary.ProductLinksAdded++;
		}

		// An update served by one file reaches every language, so it is offered in every language
		// this provider carries rather than only the few a log happened to mention.
		summary.LanguagesFilledIn += OfferInEveryLanguage(store, index, items, itemsIndex,
			product2Items);

		store.Items = items.ToArray();
		store.ItemsIndex = itemsIndex.ToArray();
		store.ItemStrings = strings.ToArray();
		store.ItemStringsIndex = stringsIndex.ToArray();
		store.Product2Items = product2Items.ToArray();
	}

	// itemstrings rows are <provider>.<guid>,<title>@|<description>@|<eula>@|@|<details>. Only the
	// title and the details link come from the log, so the rest of the row is left as it stands.
	// Writes the licence and more information addresses into a string row, leaving the title and
	// description as they are. Either may be null, meaning leave that one alone.
	private static bool ReplaceLinks(List<string> strings, string provider, string stringGuid,
		string eula, string details)
	{
		string key = provider + "." + stringGuid + ",";
		for (int i = 0; i < strings.Count; i++)
		{
			if (string.IsNullOrEmpty(strings[i])) continue;
			if (!strings[i].StartsWith(key, StringComparison.OrdinalIgnoreCase)) continue;

			string[] parts = strings[i].Substring(key.Length).Split(Sep, StringSplitOptions.None);
			if (parts.Length < 5) return false;

			bool moved = false;
			if (!string.IsNullOrEmpty(eula) && !string.Equals(parts[2], eula, StringComparison.Ordinal))
			{
				parts[2] = eula;
				moved = true;
			}
			if (!string.IsNullOrEmpty(details) && !string.Equals(parts[4], details, StringComparison.Ordinal))
			{
				parts[4] = details;
				moved = true;
			}
			if (!moved) return false;

			strings[i] = key + string.Join("@|", parts);
			return true;
		}

		return false;
	}

	private static bool ReplaceTitle(List<string> strings, string provider, string stringGuid,
		string title, string detailsHref)
	{
		string key = provider + "." + stringGuid + ",";
		for (int i = 0; i < strings.Count; i++)
		{
			if (string.IsNullOrEmpty(strings[i])) continue;
			if (!strings[i].StartsWith(key, StringComparison.OrdinalIgnoreCase)) continue;

			string[] parts = strings[i].Substring(key.Length).Split(Sep, StringSplitOptions.None);
			if (parts.Length == 0) return false;
			if (string.Equals(parts[0], title, StringComparison.Ordinal)) return false;

			parts[0] = title;
			if (parts.Length >= 5 && !string.IsNullOrEmpty(detailsHref)) parts[4] = detailsHref;
			strings[i] = key + string.Join("@|", parts);
			return true;
		}
		return false;
	}

	// Every language this provider carries, taken from the entries it already holds.
	private static HashSet<string> LanguagesServed(List<string> itemsIndex)
	{
		HashSet<string> locales = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		if (itemsIndex == null) return locales;

		foreach (string entry in itemsIndex)
		{
			if (string.IsNullOrEmpty(entry)) continue;

			string head = entry.Split(Sep, StringSplitOptions.None)[0];
			int comma = head.LastIndexOf(',');
			if (comma <= 0) continue;

			string[] parts = head.Substring(0, comma).Split('.');
			if (parts.Length >= 15) locales.Add(parts[6]);
		}

		return locales;
	}

	// An update whose file carries no language tag is the same download whatever language the
	// machine runs, so it belongs to all of them. Only the languages a log happened to record
	// were offered it, which left an update that covers everything looking like it covered four.
	// One row, and an entry for every language this provider carries and can name it in.
	private static int OfferInEveryLanguage(ProviderStore store, ProviderIndex index,
		List<string> items, List<string> itemsIndex, List<string> product2Items)
	{
		HashSet<string> served = LanguagesServed(itemsIndex);
		if (served.Count == 0) return 0;

		HashSet<string> codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string line in items)
		{
			string c = CodeOfRow(line);
			if (c != null) codes.Add(c);
		}

		int added = 0;
		foreach (string code in codes)
		{
			// Only where the whole update is one row. More than one means the languages really do
			// download different files, and nothing should be spread across them.
			string only = null;
			int rows = 0;
			foreach (string line in items)
			{
				if (!string.Equals(CodeOfRow(line), code, StringComparison.OrdinalIgnoreCase)) continue;

				rows++;
				only = line;
			}
			if (rows != 1 || only == null) continue;

			string leaf = LeafOfLine(only);
			if (leaf == null) continue;

			// A name carrying a language tag is that language's own file, not one for everybody.
			if (LogImportParser.LanguageTokenOf(leaf) != null) continue;

			string guid = GuidOfRow(only);
			string[] fields = only.Split(Sep, StringSplitOptions.None);
			string langGuid = fields.Length > 2 ? fields[2].Trim() : null;
			if (guid == null || langGuid == null) continue;

			// The entries it already has, and one to model the rest on.
			string template = null;
			HashSet<string> present = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (string entry in itemsIndex)
			{
				if (string.IsNullOrEmpty(entry)) continue;

				string head = entry.Split(Sep, StringSplitOptions.None)[0];
				int comma = head.LastIndexOf(',');
				if (comma <= 0) continue;

				string id = head.Substring(0, comma);
				string[] parts = id.Split('.');
				if (parts.Length < 15) continue;
				if (!string.Equals(parts[13], code, StringComparison.OrdinalIgnoreCase)) continue;

				present.Add(parts[6]);
				if (template == null) template = id;
			}
			if (template == null) continue;

			// At least two languages already downloading this one file is what says it serves them
			// all. Plenty of updates are for a single language and simply have no tag in the file
			// name, and widening one of those would offer it to machines it was never meant for.
			if (present.Count < 2) continue;

			foreach (string locale in served)
			{
				if (present.Contains(locale)) continue;

				// Only where the update can be named in that language. Without a title it would
				// appear as a blank line rather than an update.
				if (index.StringGuidFor(langGuid, locale) == null) continue;

				string newId = ReplaceLocale(template, locale);
				if (newId == null) continue;

				itemsIndex.Add(newId + "," + guid + "@|");
				AddProductLink(product2Items, store.Name, newId);
				added++;
			}
		}

		return added;
	}

	// The update code an items row carries, or null when the row is not one.
	private static string CodeOfRow(string line)
	{
		if (string.IsNullOrEmpty(line)) return null;

		int at = line.IndexOf("@|", StringComparison.Ordinal);
		string head = at < 0 ? line : line.Substring(0, at);
		int comma = head.IndexOf(',');
		return comma < 0 ? null : head.Substring(comma + 1).Trim();
	}

	// The identifier at the front of an items row.
	private static string GuidOfRow(string line)
	{
		if (string.IsNullOrEmpty(line)) return null;

		int comma = line.IndexOf(',');
		return comma <= 0 ? null : line.Substring(0, comma).Trim();
	}

	// The address a row downloads from, with the hash the cabpool appends taken off the file name.
	// The whole address matters, not just the file name: the language often sits in the path, as
	// in selfupd/x86/w98/de/cun.cab, so two languages can name the same file and still be two
	// different downloads.
	private static string DownloadKey(string itemLine)
	{
		string href = ValueOf(itemLine, "codeBase href=\"", "\"");
		if (string.IsNullOrEmpty(href)) return null;

		int slash = href.LastIndexOf('/');
		if (slash < 0) return LogImportParser.StripHash(href);

		return href.Substring(0, slash + 1) + LogImportParser.StripHash(href.Substring(slash + 1));
	}

	// A row of this update that downloads from the same address as the one given. Null when no
	// row does.
	private static string RowWithSameFile(List<string> items, string code, string itemLine)
	{
		string wanted = DownloadKey(itemLine);
		if (items == null || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(wanted)) return null;

		foreach (string line in items)
		{
			if (!string.Equals(CodeOfRow(line), code, StringComparison.OrdinalIgnoreCase)) continue;

			string other = DownloadKey(line);
			if (other == null) continue;
			if (string.Equals(other, wanted, StringComparison.OrdinalIgnoreCase)) return line;
		}

		return null;
	}

	// Puts every language of an update that uses one file onto a single row. Where two rows of
	// the same update hold the same download, the first is kept, every index entry naming the
	// others is pointed at it, and the others are taken out. Returns how many rows went.
	// A null set of codes means every update in the inventory.
	private static int MergeRowsSharingAFile(List<string> items, List<string> itemsIndex,
		HashSet<string> codes)
	{
		if (items == null || itemsIndex == null) return 0;

		// The row each duplicate should give way to, by the identifier it used to have.
		Dictionary<string, string> replacement =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		Dictionary<string, string> keptByFile =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		foreach (string line in items)
		{
			string code = CodeOfRow(line);
			if (code == null) continue;
			if (codes != null && !codes.Contains(code)) continue;

			string address = DownloadKey(line);
			string guid = GuidOfRow(line);
			if (address == null || guid == null) continue;

			string key = code + "|" + address;
			string keptGuid;
			if (!keptByFile.TryGetValue(key, out keptGuid))
			{
				keptByFile[key] = guid;
				continue;
			}
			if (!string.Equals(keptGuid, guid, StringComparison.OrdinalIgnoreCase))
			{
				replacement[guid] = keptGuid;
			}
		}
		if (replacement.Count == 0) return 0;

		// Every entry that named a row being taken out now names the one being kept.
		for (int i = 0; i < itemsIndex.Count; i++)
		{
			if (string.IsNullOrEmpty(itemsIndex[i])) continue;

			string head = itemsIndex[i].Split(Sep, StringSplitOptions.None)[0];
			int comma = head.LastIndexOf(',');
			if (comma <= 0) continue;

			string kept;
			if (!replacement.TryGetValue(head.Substring(comma + 1).Trim(), out kept)) continue;

			itemsIndex[i] = head.Substring(0, comma + 1) + kept + itemsIndex[i].Substring(head.Length);
		}

		int removed = 0;
		for (int i = items.Count - 1; i >= 0; i--)
		{
			string guid = GuidOfRow(items[i]);
			if (guid == null || !replacement.ContainsKey(guid)) continue;

			items.RemoveAt(i);
			removed++;
		}

		return removed;
	}

	// The row a language points at right now, read from the entries as they stand rather than from
	// the index built when the import began.
	private static string LiveGuidFor(List<string> itemsIndex, string code, string locale)
	{
		if (itemsIndex == null || string.IsNullOrEmpty(code)) return null;

		foreach (string entry in itemsIndex)
		{
			if (string.IsNullOrEmpty(entry)) continue;

			string head = entry.Split(Sep, StringSplitOptions.None)[0];
			int comma = head.LastIndexOf(',');
			if (comma <= 0) continue;

			string[] parts = head.Substring(0, comma).Split('.');
			if (parts.Length < 15) continue;
			if (!string.Equals(parts[13], code, StringComparison.OrdinalIgnoreCase)) continue;
			if (!string.Equals(parts[6], locale, StringComparison.OrdinalIgnoreCase)) continue;

			return head.Substring(comma + 1).Trim();
		}

		return null;
	}

	// The version is the final field of the identifier, as in com_microsoft.agent2_95.2_00_0_2202.
	private static string ReplaceVersion(string itemId, string version)
	{
		string[] parts = itemId.Split('.');
		if (parts.Length < 15) return itemId;
		parts[14] = version;
		return string.Join(".", parts);
	}

	// Writes a record for an update the provider has never held. The identifier comes from the log,
	// because with nothing already under this code there is no local record to prefer.
	private static void AddNewUpdate(ImportCandidate c, string locale, ProviderStore store,
		ProviderIndex index, List<string> items, List<string> itemsIndex, List<string> strings,
		List<string> stringsIndex, List<string> product2Items, Dictionary<string, string> newLangGuids,
		HashSet<string> takenIds, ImportSummary summary, StringProvenance provenance)
	{
		if (string.IsNullOrEmpty(c.ItemId))
		{
			summary.Skipped++;
			summary.Notes.Add(store.Name + ": " + c.Code + " skipped, the log gave no identifier for it");
			return;
		}

		string itemId = ReplaceLocale(c.ItemId, locale);
		if (!string.IsNullOrEmpty(c.Version)) itemId = ReplaceVersion(itemId, c.Version);

		// Two languages of the same new update share one string set.
		string langGuid;
		if (!newLangGuids.TryGetValue(c.Code, out langGuid))
		{
			langGuid = Guid.NewGuid().ToString().ToUpper();
			newLangGuids[c.Code] = langGuid;
		}

		string fileGuid = string.IsNullOrEmpty(c.ItemGuid)
			? Guid.NewGuid().ToString().ToUpper()
			: c.ItemGuid;
		if (!string.IsNullOrEmpty(c.ItemGuid)) summary.GuidsFromLog++;

		// A repeated run, or a second entry landing on the same language, must not write the same
		// record twice.
		string head = fileGuid + ",";
		if (!takenIds.Add(itemId) ||
			items.Any(x => !string.IsNullOrEmpty(x) && x.StartsWith(head, StringComparison.OrdinalIgnoreCase)))
		{
			summary.Skipped++;
			summary.Notes.Add(store.Name + ": " + c.Code + " (" + locale +
				") skipped, it is already written under this identifier");
			return;
		}

		string newRow = LogImportNewItems.BuildItemsLine(c, langGuid, index.MostCommonGroup(),
			fileGuid, LogImportNewItems.BuildInstallation(c));

		// An update new to this folder arrives once per language, and where those languages all
		// download the same file they belong on one row that they all point at. Writing a row per
		// language gave the same address two records and made an update that uses one file for
		// everything look as though it had a separate one for each.
		string sharedRow = RowWithSameFile(items, c.Code, newRow);
		if (sharedRow != null)
		{
			string sharedGuid = GuidOfRow(sharedRow);
			if (sharedGuid != null)
			{
				fileGuid = sharedGuid;
				summary.LanguagesSharingAFile++;
			}
			else
			{
				sharedRow = null;
			}
		}

		if (sharedRow == null)
		{
			items.Add(newRow);
			summary.ItemsAdded++;
			summary.NewUpdatesAdded++;
		}
		summary.Count(store.Name);
		LogImportHighlight.Add(store.Name, c.Code);
		if (!c.HasPostedDate) summary.WithoutPostedDate++;

		itemsIndex.Add(itemId + "," + fileGuid + "@|");
		summary.IndexEntriesAdded++;

		string stringGuid = Guid.NewGuid().ToString().ToUpper();
		stringsIndex.Add(store.Name + "." + locale + "." + langGuid + "," + stringGuid);
		strings.Add(string.Format("{0}.{1},{2}@|{3}@|{4}@|@|{5}",
			store.Name, stringGuid, c.Title, string.Empty, EulaFor(c, locale), c.DetailsHref));
		summary.StringsAdded++;
		provenance.MarkAuthentic(store.Name, stringGuid);

		if (AddProductLink(product2Items, store.Name, itemId)) summary.ProductLinksAdded++;
	}

	// Puts right what the log knows about a record the provider already has. Nothing here changes
	// which operating system the update targets.
	private static void ApplyCorrection(ImportCandidate c, string locale, ProviderStore store,
		ProviderIndex index, List<string> items, List<string> itemsIndex, List<string> strings,
		List<string> product2Items, ImportSummary summary, StringProvenance provenance)
	{
		if (c.Fix == null || !c.Fix.Any) return;

		// Set by each part that manages to change something. A correction that cannot be applied
		// must not be reported as done, or it comes back on the next import looking untouched.
		bool changed = false;

		// One code is shared by every language of an update, so respelling it covers the whole
		// update and is done before anything else is looked up by code.
		if (c.Fix.Capitalisation && !string.IsNullOrEmpty(c.Fix.CodeAsPublished))
		{
			// One code is shared by every language of an update, so the first language to be
			// corrected respells it for all of them. The rest find nothing left to do, and that
			// is the correction having worked rather than having failed.
			if (RenameCode(items, itemsIndex, product2Items, c.Fix.CodeAsPublished))
			{
				summary.CodesRecased++;
			}
			changed = true;
		}

		string oldGuid = index.ItemGuidFor(c.Code, locale);
		string itemId = index.ItemIdFor(c.Code, locale);
		if (oldGuid == null || itemId == null)
		{
			// Offered against a record that cannot be found again. Saying nothing here left the
			// correction on the list every time, with no sign of why it never took.
			summary.Skipped++;
			summary.Notes.Add(store.Name + ": " + c.Code + " (" + locale +
				") skipped, no record of it could be found for that language");
			return;
		}

		// The index was built before the rename above, so the identifier it hands back still
		// carries the old spelling. Writing that back would undo the rename on this one entry.
		if (c.Fix.Capitalisation && !string.IsNullOrEmpty(c.Fix.CodeAsPublished))
		{
			itemId = ReplaceCode(itemId, c.Fix.CodeAsPublished);
		}

		// items.txt row for this language.
		int itemAt = -1;
		for (int i = 0; i < items.Count; i++)
		{
			if (string.IsNullOrEmpty(items[i])) continue;
			if (items[i].StartsWith(oldGuid + ",", StringComparison.OrdinalIgnoreCase)) { itemAt = i; break; }
		}
		if (itemAt < 0)
		{
			// What the index hands back was read before this import began, and rows move while it
			// runs: putting an update onto one row leaves the languages that shared it pointing at
			// a row that has since gone. The entry for this language is read again as it stands now
			// rather than the correction being dropped.
			string live = LiveGuidFor(itemsIndex, c.Code, locale);
			if (live != null)
			{
				for (int i = 0; i < items.Count; i++)
				{
					if (string.IsNullOrEmpty(items[i])) continue;
					if (!items[i].StartsWith(live + ",", StringComparison.OrdinalIgnoreCase)) continue;

					itemAt = i;
					oldGuid = live;
					break;
				}
			}
		}
		if (itemAt < 0)
		{
			summary.Skipped++;
			summary.Notes.Add(store.Name + ": " + c.Code + " (" + locale +
				") skipped, the row it names is not there");
			return;
		}

		string[] fields = items[itemAt].Split(Sep, StringSplitOptions.None);
		if (fields.Length < 14) return;

		string newGuid = oldGuid;
		if (c.Fix.Guid && !string.IsNullOrEmpty(c.ItemGuid))
		{
			// Never move onto a GUID another record already uses, which would leave two rows sharing
			// one identity and the index unable to tell them apart.
			string taken = c.ItemGuid + ",";
			bool clash = false;
			for (int i = 0; i < items.Count; i++)
			{
				if (i == itemAt || string.IsNullOrEmpty(items[i])) continue;
				if (items[i].StartsWith(taken, StringComparison.OrdinalIgnoreCase)) { clash = true; break; }
			}
			if (!clash)
			{
				newGuid = c.ItemGuid;
				fields[0] = newGuid + "," + c.Code;
				summary.GuidsCorrected++;
				changed = true;
			}
		}

		if (c.Fix.FileName && !string.IsNullOrEmpty(c.DownloadUrl))
		{
			fields[5] = SetDownload(fields[5], c.DownloadUrl, c.FileName, c.Size, c.CleanFileName);
			summary.FileNamesCorrected++;
			changed = true;
		}

		if (c.Fix.CommandType && !string.IsNullOrEmpty(c.Fix.CommandTypeAsObserved))
		{
			string was = LogImportNewItems.CommandTypeOf(fields[5]);
			if (!string.IsNullOrEmpty(was))
			{
				fields[5] = fields[5].Replace(
					"commandType=\"" + was + "\"",
					"commandType=\"" + c.Fix.CommandTypeAsObserved + "\"");
				summary.CommandTypesCorrected++;
				changed = true;
			}
		}

		if (c.Fix.Reboot && !string.IsNullOrEmpty(c.Fix.RebootAsObserved))
		{
			string was = LogImportNewItems.RebootOf(fields[5]);
			if (was != null)
			{
				fields[5] = fields[5].Replace(
					"needsReboot=\"" + was + "\"",
					"needsReboot=\"" + c.Fix.RebootAsObserved + "\"");
				summary.RebootFlagsCorrected++;
				changed = true;
			}
		}

		// A published date belongs to one language. Where an update served every language from a
		// single row that row carries one date for all of them, and writing this language's date
		// into it would restamp every other language from one language's record. The catalogue
		// really does differ language by language, a second apart where a batch was published in
		// sequence, so the date is only written where the row belongs to this language alone.
		if (c.HasPostedDate && !SameInstant(fields[9], c.Timestamp) &&
			LocalesOnRow(itemsIndex, oldGuid) <= 1)
		{
			fields[9] = c.Timestamp;
			summary.DatesCorrected++;
			changed = true;
		}

		items[itemAt] = string.Join("@|", fields);

		// itemsindex points at the row by GUID, so both have to move together.
		string newItemId = itemId;
		if (c.Fix.Version && !string.IsNullOrEmpty(c.Version))
		{
			newItemId = ReplaceVersion(itemId, c.Version);
			summary.VersionsCorrected++;
			changed = true;
		}
		// An items row is pointed at by one index entry per operating system target it serves, so a
		// new GUID has to be followed through all of them. Only the entry naming this exact
		// identifier has its version rewritten.
		if (newItemId != itemId || newGuid != oldGuid)
		{
			for (int i = 0; i < itemsIndex.Count; i++)
			{
				if (string.IsNullOrEmpty(itemsIndex[i])) continue;
				string entryHead = itemsIndex[i].Split(Sep, StringSplitOptions.None)[0];
				int comma = entryHead.LastIndexOf(',');
				if (comma <= 0) continue;

				string entryId = entryHead.Substring(0, comma);
				string entryGuid = entryHead.Substring(comma + 1).Trim();
				if (!string.Equals(entryGuid, oldGuid, StringComparison.OrdinalIgnoreCase)) continue;

				string tail = itemsIndex[i].Substring(entryHead.Length);
				// Compared without case, since the rename may already have respelled this entry.
				bool exact = string.Equals(entryId, itemId, StringComparison.OrdinalIgnoreCase);
				itemsIndex[i] = (exact ? newItemId : entryId) + "," + newGuid + tail;
			}
		}

		// product2items names the same identifier with the provider taken off the front, so a
		// rewritten version has to be carried into it as well. Leaving it behind points the
		// product at an entry that no longer exists, and the update quietly stops being offered
		// for that operating system even though its row is still there.
		if (!string.Equals(newItemId, itemId, StringComparison.Ordinal))
		{
			RepointProductLinks(product2Items, itemId, newItemId);
		}

		// The version belongs to the update rather than to any one of its languages, so it goes
		// on every entry of that code. Writing it only on the entry for the language being
		// imported left every other language on the old version, and the identifier then did not
		// match the one the service published.
		if (c.Fix.Version && !string.IsNullOrEmpty(c.Version))
		{
			SetVersionEverywhere(itemsIndex, product2Items, c.Code, c.Version);
		}

		// Written on their own, so a link is put right even where the title is already correct.
		if (c.Fix.Eula || c.Fix.Details)
		{
			string langGuidForLinks = index.LangGuidFor(c.Code);
			string linkGuid = langGuidForLinks == null
				? null
				: index.StringGuidFor(langGuidForLinks, locale);
			if (linkGuid != null && ReplaceLinks(strings, store.Name, linkGuid,
				c.Fix.Eula ? StatedEula(c) : null, c.Fix.Details ? c.DetailsHref : null))
			{
				summary.LinksCorrected++;
				changed = true;
			}
		}

		if (c.Fix.Title && LogImportNewItems.TitleUsable(c, locale))
		{
			string langGuid = index.LangGuidFor(c.Code);
			string stringGuid = langGuid == null ? null : index.StringGuidFor(langGuid, locale);
			if (stringGuid != null && ReplaceTitle(strings, store.Name, stringGuid, c.Title, c.DetailsHref))
			{
				summary.TitlesCorrected++;
				changed = true;
				provenance.MarkAuthentic(store.Name, stringGuid);
			}
		}

		if (!changed)
		{
			summary.Skipped++;
			summary.Notes.Add(store.Name + ": " + c.Code + " (" + locale +
				") could not be corrected, nothing it offered could be applied");
			return;
		}

		summary.Corrected++;
		summary.Count(store.Name);
		LogImportHighlight.Add(store.Name, c.Code);
	}

	// Writes the published spelling of a code into all three files that carry it. Matching ignores
	// case, so only the capitals change and nothing is ever repointed at a different update.
	private static bool RenameCode(List<string> items, List<string> itemsIndex,
		List<string> product2Items, string published)
	{
		bool changed = false;

		for (int i = 0; i < items.Count; i++)
		{
			if (string.IsNullOrEmpty(items[i])) continue;
			int at = items[i].IndexOf("@|", StringComparison.Ordinal);
			if (at < 0) continue;
			string head = items[i].Substring(0, at);
			int comma = head.IndexOf(',');
			if (comma < 0) continue;
			string code = head.Substring(comma + 1);
			if (!string.Equals(code, published, StringComparison.OrdinalIgnoreCase)) continue;
			if (string.Equals(code, published, StringComparison.Ordinal)) continue;
			items[i] = head.Substring(0, comma + 1) + published + items[i].Substring(at);
			changed = true;
		}

		for (int i = 0; i < itemsIndex.Count; i++)
		{
			if (string.IsNullOrEmpty(itemsIndex[i])) continue;
			string head = itemsIndex[i].Split(Sep, StringSplitOptions.None)[0];
			int comma = head.LastIndexOf(',');
			if (comma <= 0) continue;
			string[] parts = head.Substring(0, comma).Split('.');
			if (parts.Length < 15) continue;
			if (!string.Equals(parts[13], published, StringComparison.OrdinalIgnoreCase)) continue;
			if (string.Equals(parts[13], published, StringComparison.Ordinal)) continue;
			parts[13] = published;
			itemsIndex[i] = string.Join(".", parts) + itemsIndex[i].Substring(comma);
			changed = true;
		}

		for (int i = 0; i < product2Items.Count; i++)
		{
			if (string.IsNullOrEmpty(product2Items[i])) continue;
			string[] refs = product2Items[i].Split(',');
			bool touched = false;
			for (int j = 1; j < refs.Length; j++)
			{
				string[] parts = refs[j].Split('.');
				if (parts.Length < 14) continue;
				if (!string.Equals(parts[12], published, StringComparison.OrdinalIgnoreCase)) continue;
				if (string.Equals(parts[12], published, StringComparison.Ordinal)) continue;
				parts[12] = published;
				refs[j] = string.Join(".", parts);
				touched = true;
			}
			if (touched)
			{
				product2Items[i] = string.Join(",", refs);
				changed = true;
			}
		}

		return changed;
	}

	// The code sits in the fourteenth field of an identifier.
	private static string ReplaceCode(string itemId, string code)
	{
		string[] parts = itemId.Split('.');
		if (parts.Length < 15) return itemId;
		parts[13] = code;
		return string.Join(".", parts);
	}

	private static string ReplaceLocale(string itemId, string locale)
	{
		string[] parts = itemId.Split('.');
		if (parts.Length < 15) return itemId;
		parts[6] = locale;
		return string.Join(".", parts);
	}

	// The file part of the codeBase address, which is what carries the language tag and the hash.
	internal static string LeafOfLine(string itemLineOrInstallation)
	{
		string href = ValueOf(itemLineOrInstallation, "codeBase href=\"", "\"");
		if (string.IsNullOrEmpty(href)) return null;
		int slash = href.LastIndexOf('/');
		return slash < 0 ? href : href.Substring(slash + 1);
	}

	// Points the installation at a different file. A full address replaces the old one outright,
	// while a null address keeps the folder the sibling used and only changes the file name.
	private static string SetDownload(string installation, string newHref, string newLeaf, long size,
		string cleanName = null)
	{
		// An address on the restored service is never written into an inventory, whatever it
		// was read from. Only the original service is a source for these.
		if (LogImportParser.NamesRestoredService(newHref)) return installation;

		string result = installation;
		string oldHref = ValueOf(installation, "codeBase href=\"", "\"");
		string oldLeaf = LeafOfLine(installation);

		string href = newHref;
		if (href == null && oldHref != null && oldLeaf != null && newLeaf != null)
		{
			href = oldHref.Substring(0, oldHref.Length - oldLeaf.Length) + newLeaf;
		}
		if (!string.IsNullOrEmpty(oldHref) && !string.IsNullOrEmpty(href))
		{
			result = result.Replace("codeBase href=\"" + oldHref + "\"",
				"codeBase href=\"" + href + "\"");
		}

		// The name attribute and the command text are the file the installer runs, which is the
		// download without its hash. They are only rewritten when they really are that file: plenty
		// of entries name a payload inside the package instead, such as system32\\inetcomm.dll, and
		// that has nothing to do with the language.
		string oldName = ValueOf(installation, "name=\"", "\"");
		string expected = LogImportParser.StripHash(oldLeaf);
		if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(expected) && newLeaf != null &&
			string.Equals(oldName, expected, StringComparison.OrdinalIgnoreCase))
		{
			// The log records the file the installer actually runs, so that is used in preference to
			// working it out by taking the hash off the address.
			string newName = string.IsNullOrEmpty(cleanName)
				? LogImportParser.StripHash(newLeaf)
				: cleanName;
			result = result.Replace("name=\"" + oldName + "\"", "name=\"" + newName + "\"");
			result = result.Replace(">" + oldName + "<", ">" + newName + "<");
		}

		if (size > 0)
		{
			string oldSize = ValueOf(installation, "<size>", "</size>");
			if (!string.IsNullOrEmpty(oldSize)) result = result.Replace("<size>" + oldSize + "</size>", "<size>" + size + "</size>");
		}

		return result;
	}

	private static string ValueOf(string text, string open, string close)
	{
		if (string.IsNullOrEmpty(text)) return null;

		int start = text.IndexOf(open, StringComparison.Ordinal);
		if (start < 0) return null;
		start += open.Length;
		int end = text.IndexOf(close, start, StringComparison.Ordinal);
		return end < 0 ? null : text.Substring(start, end - start);
	}

	// How many languages a single items row serves, counted from the index entries that point at
	// it. Most updates shipped one file per language, but some served every language from one
	// row and anything written there is written for all of them at once.
	private static int LocalesOnRow(List<string> itemsIndex, string guid)
	{
		if (itemsIndex == null || string.IsNullOrEmpty(guid)) return 0;

		HashSet<string> locales = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string entry in itemsIndex)
		{
			if (string.IsNullOrEmpty(entry)) continue;
			string head = entry.Split(Sep, StringSplitOptions.None)[0];
			int comma = head.LastIndexOf(',');
			if (comma <= 0) continue;
			if (!string.Equals(head.Substring(comma + 1).Trim(), guid,
				StringComparison.OrdinalIgnoreCase)) continue;

			string[] parts = head.Substring(0, comma).Split('.');
			if (parts.Length >= 15) locales.Add(parts[6]);
		}

		return locales.Count;
	}

	// Whether two published dates name the same moment. The catalogue writes the fraction of a
	// second as it pleases while the dictionaries pad it, so 17:26:05.055 and 17:26:05.0550 are
	// the same date spelled two ways and rewriting one as the other changes nothing.
	private static bool SameInstant(string left, string right)
	{
		if (string.Equals(left, right, StringComparison.Ordinal)) return true;
		if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) return false;

		return string.Equals(TrimFraction(left), TrimFraction(right), StringComparison.OrdinalIgnoreCase);
	}

	private static string TrimFraction(string stamp)
	{
		int dot = stamp.LastIndexOf('.');
		if (dot < 0) return stamp;

		string trimmed = stamp.Substring(dot + 1).TrimEnd('0');
		return trimmed.Length == 0 ? stamp.Substring(0, dot) : stamp.Substring(0, dot + 1) + trimmed;
	}

	// The licence address for a new row. The source states it outright, so that is what gets
	// written and an update keeps the licence it was really served with: the administrator
	// catalogue points at corp_eula.htm and consumer history at eula.htm. Only when the source
	// gave none is one built, and then it is the corporate form. What used to be written was
	// neither: a bare "cs/eula.htm" with no path in front of it, thrown away from the address
	// the file had already supplied.
	private const string EulaFolder = "/msdownload/update/v3/static/eula/";

	// The licence the source states, in the form the dictionaries hold, or null when it states
	// none. Correcting a row uses this rather than the one below: an update the source says
	// nothing about must keep the licence it has, not be given a made up one.
	private static string StatedEula(ImportCandidate c)
	{
		if (c == null || string.IsNullOrEmpty(c.EulaHref)) return null;

		int at = c.EulaHref.IndexOf(EulaFolder, StringComparison.OrdinalIgnoreCase);
		return at < 0 ? c.EulaHref : c.EulaHref.Substring(at + EulaFolder.Length);
	}

	private static string EulaFor(ImportCandidate c, string locale)
	{
		// The dictionaries hold this as a path relative to the licence folder, "cs/eula.htm"
		// rather than the whole address, which is how all eighteen thousand of the rows already
		// there are written. The source states the whole address, so the folder comes off it.
		if (c != null && !string.IsNullOrEmpty(c.EulaHref))
		{
			string href = c.EulaHref;
			int at = href.IndexOf(EulaFolder, StringComparison.OrdinalIgnoreCase);
			return at < 0 ? href : href.Substring(at + EulaFolder.Length);
		}

		return locale + "/corp_eula.htm";
	}

	// Writes the version onto every entry of an update, whatever language or operating system it
	// is for, and carries each rewritten identifier into product2items. Returns how many moved.
	private static int SetVersionEverywhere(List<string> itemsIndex, List<string> product2Items,
		string code, string version)
	{
		if (itemsIndex == null || string.IsNullOrEmpty(code) || version == null) return 0;

		int changed = 0;
		for (int i = 0; i < itemsIndex.Count; i++)
		{
			if (string.IsNullOrEmpty(itemsIndex[i])) continue;

			string head = itemsIndex[i].Split(Sep, StringSplitOptions.None)[0];
			int comma = head.LastIndexOf(',');
			if (comma <= 0) continue;

			string oldId = head.Substring(0, comma);
			string[] parts = oldId.Split('.');
			if (parts.Length < 15) continue;
			if (!string.Equals(parts[13], code, StringComparison.OrdinalIgnoreCase)) continue;
			if (string.Equals(parts[14], version, StringComparison.Ordinal)) continue;

			parts[14] = version;
			string newId = string.Join(".", parts);
			itemsIndex[i] = newId + itemsIndex[i].Substring(comma);
			RepointProductLinks(product2Items, oldId, newId);
			changed++;
		}

		return changed;
	}

	// Moves every product2items reference from one identifier to another, matched whole rather
	// than by substring so a code that is the start of a longer one is never caught.
	private static bool RepointProductLinks(List<string> product2Items, string oldItemId,
		string newItemId)
	{
		if (product2Items == null) return false;

		string oldRef = WithoutProvider(oldItemId);
		string newRef = WithoutProvider(newItemId);
		if (oldRef == null || newRef == null ||
			string.Equals(oldRef, newRef, StringComparison.Ordinal)) return false;

		bool changed = false;
		for (int i = 0; i < product2Items.Count; i++)
		{
			if (string.IsNullOrEmpty(product2Items[i])) continue;
			string[] refs = product2Items[i].Split(',');
			// A line can already carry the identifier being moved to, in which case the old
			// reference is dropped rather than rewritten: listing the same item twice under one
			// product is not something the catalogue expects.
			bool holdsNew = false;
			for (int j = 1; j < refs.Length; j++)
			{
				if (string.Equals(refs[j], newRef, StringComparison.OrdinalIgnoreCase)) holdsNew = true;
			}

			List<string> kept = new List<string> { refs[0] };
			bool touched = false;
			for (int j = 1; j < refs.Length; j++)
			{
				if (!string.Equals(refs[j], oldRef, StringComparison.OrdinalIgnoreCase))
				{
					kept.Add(refs[j]);
					continue;
				}

				touched = true;
				if (holdsNew) continue;

				kept.Add(newRef);
				holdsNew = true;
			}
			if (touched)
			{
				product2Items[i] = string.Join(",", kept.ToArray());
				changed = true;
			}
		}

		return changed;
	}

	// An identifier as product2items writes it, which is the items index form without the
	// provider that heads it.
	private static string WithoutProvider(string itemId)
	{
		if (string.IsNullOrEmpty(itemId)) return null;
		int dot = itemId.IndexOf('.');
		return dot < 0 ? null : itemId.Substring(dot + 1);
	}

	// product2items lists every item a given operating system target offers, one target per line.
	// The reference is appended to the matching line rather than replacing it.
	private static bool AddProductLink(List<string> product2Items, string provider, string itemId)
	{
		string withoutProvider = itemId.StartsWith(provider + ".", StringComparison.OrdinalIgnoreCase)
			? itemId.Substring(provider.Length + 1)
			: itemId;

		// The target key is the identifier with the namespace, code and version fields dropped.
		// Dropping the provider leaves fourteen fields, of which the first eleven make the key
		// that heads a product2items line.
		string[] parts = withoutProvider.Split('.');
		if (parts.Length < 14) return false;
		string targetKey = provider + "." + string.Join(".", parts.Take(11).ToArray());

		for (int i = 0; i < product2Items.Count; i++)
		{
			if (string.IsNullOrEmpty(product2Items[i])) continue;
			int comma = product2Items[i].IndexOf(',');
			string head = comma < 0 ? product2Items[i] : product2Items[i].Substring(0, comma);
			if (!string.Equals(head.TrimEnd('.'), targetKey.TrimEnd('.'), StringComparison.OrdinalIgnoreCase)) continue;
			if (product2Items[i].IndexOf(withoutProvider, StringComparison.OrdinalIgnoreCase) >= 0) return false;
			product2Items[i] = product2Items[i].TrimEnd(',') + "," + withoutProvider;
			return true;
		}

		product2Items.Add(targetKey + "," + withoutProvider);
		return true;
	}
}
