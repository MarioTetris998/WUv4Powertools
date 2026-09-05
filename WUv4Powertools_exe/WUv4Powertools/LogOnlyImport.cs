using System;
using System.Collections.Generic;
using System.Linq;

namespace WUv4Powertools;

// A Windows Update.log on its own names no providers and no update codes, only the files a machine
// downloaded. That is still worth something: the article number in a file name ties it to updates the
// dictionaries already hold, and the language tag in it says which language that file was for. From
// those two facts a log alone can fill in a missing language and put a wrong file name right.
public static class LogOnlyImport
{
	// The one language this update is held in, or null when it is held in none or in several.
	// A download whose name states no language can only be placed where there is a single
	// place for it to go.
	private static string OnlyLanguageOf(ProviderIndex index, string code)
	{
		string only = null;
		foreach (string locale in index.LocalesFor(code))
		{
			if (only != null) return null;

			only = locale;
		}

		return only;
	}

	// Builds candidates from the downloads a log recorded, for every provider in the folder that has
	// an update with the same article number. Only used when no history file was supplied, since a
	// history file states all of this outright and far more reliably.
	public static int AddCandidates(LogImportResult result, ConsumerDictionary dictionary)
	{
		if (result == null || dictionary == null) return 0;
		if (result.Downloads.Count == 0) return 0;

		int added = 0;

		// A machine downloads the same update in session after session, so the same file turns up
		// many times over. One row per update and language is enough.
		HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (ImportCandidate existingCandidate in result.Candidates)
		{
			seen.Add(existingCandidate.Provider + "|" + existingCandidate.Code +
				"|" + existingCandidate.Language);
		}
		// A machine downloads the same update over and over across the sessions a log covers, and
		// a later session may fetch the replacement for a file that has since been superseded:
		// Windows98-KB891711-FIN.EXE in one session and Windows98-KB891711-v2-FIN.EXE in a later
		// one. Entries are held in the order the machine wrote them, so the last download for an
		// update and language is the newest and the only one worth offering. Taking whichever
		// came first would offer to put the older file back, and then offer to undo that on the
		// next import, with neither pass ever settling.
		Dictionary<string, LogImportParser.LogEntry> newest =
			new Dictionary<string, LogImportParser.LogEntry>(StringComparer.OrdinalIgnoreCase);
		foreach (LogImportParser.LogEntry download in result.Downloads)
		{
			// The log names the update this download belongs to, and only that exact code is
			// accepted. Matching on the article number instead would tie a file to any update
			// sharing its KB number, and the same KB ships a separate binary per operating
			// system: a Windows ME download would overwrite the Windows Server 2003 one.
			if (string.IsNullOrEmpty(download.ItemCode)) continue;

			// Nothing carrying the restored service is ever taken from a log, address included.
			if (LogImportParser.NamesRestoredService(download.Url)) continue;

			// A file with no language tag in its name is the same download whatever language the
			// machine runs, so it belongs to the update as a whole rather than to one language.
			// Passing over it left an update that uses a single file untouched by an import that
			// had its download sitting right there in the log.
			newest[download.ItemCode + "|" + (download.Locale ?? string.Empty)] = download;
		}

		foreach (ProviderStore store in dictionary.Providers)
		{
			ProviderIndex index = store.Index;

			foreach (LogImportParser.LogEntry download in newest.Values)
			{
				foreach (string code in index.CodesMatching(download.ItemCode))
				{
					// Where a download goes has to be known rather than guessed. Its own name states the
					// language, or the update is held in one language alone and there is nowhere else it
					// could belong. What language the machine ran says nothing about a file whose name
					// states none, and a language the update happens to have is not evidence either, so a
					// download answering to neither is left out rather than filed somewhere it may not go.
					string locale = download.Locale ?? OnlyLanguageOf(index, code);
					if (locale == null) continue;

					string existing = index.ItemsLineFor(code, locale);
					bool present = existing != null;

					// The file the provider records for this language, if it has one at all.
					string currentLeaf = present ? LogImportEngine.LeafOfLine(existing) : null;
					// Where both name a file in the cabpool the hashes settle it, since one name covers
					// more than one build. Otherwise they are weighed without their hashes, which the
					// address carries and a saved path does not, so a name already right is not offered
					// again every time.
					string heldHash = currentLeaf == null ? null : LogImportParser.HashOf(currentLeaf);
					string statedHash = LogImportParser.HashOf(download.FileName);
					bool sameFile = heldHash != null && statedHash != null
						? string.Equals(heldHash, statedHash, StringComparison.OrdinalIgnoreCase)
						: currentLeaf != null &&
							string.Equals(LogImportParser.StripHash(currentLeaf),
								LogImportParser.StripHash(download.FileName), StringComparison.OrdinalIgnoreCase);
					if (present && sameFile) continue;

					ImportCandidate candidate = new ImportCandidate
					{
						Provider = store.Name,
						Code = code,
						Language = locale,
						DownloadUrl = download.Url,
						FileName = download.FileName,
						SharedAcrossLanguages = download.Shared,
						SourceFile = download.SourceFile ?? "Windows Update.log",
						// A log records a download, never a publication, so it can offer no date and
						// no title. Those only ever come from a catalogue file.
						Title = string.Empty,
						Timestamp = string.Empty,
						HasPostedDate = false,
						ItemId = index.ItemIdFor(code, locale) ?? index.SiblingItemId(code)
					};

					if (!seen.Add(store.Name + "|" + code + "|" + locale)) continue;

					result.Candidates.Add(candidate);
					added++;
				}
			}
		}

		return added;
	}
}
