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
	// A language this update is already held in, preferring English. Used when the download names
	// no language of its own because one file serves them all.
	private static string FirstLanguageOf(ProviderIndex index, string code)
	{
		string first = null;
		foreach (string locale in index.LocalesFor(code))
		{
			if (string.Equals(locale, "en", StringComparison.OrdinalIgnoreCase)) return locale;

			if (first == null) first = locale;
		}

		return first;
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
					// A file for everybody is weighed against a language the update already has, so
					// the row is put right once. Offering it in the rest of the languages happens
					// after the import, where the whole update can be seen at once.
					string locale = download.Locale ?? FirstLanguageOf(index, code);
					if (locale == null) continue;

					string existing = index.ItemsLineFor(code, locale);
					bool present = existing != null;

					// The file the provider records for this language, if it has one at all.
					string currentLeaf = present ? LogImportEngine.LeafOfLine(existing) : null;
					// Compared without the cabpool hash, which the address carries and the saved path does
					// not, so a name that is already right is not offered again every time.
					bool sameFile = currentLeaf != null &&
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
						SourceFile = "Windows Update.log",
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
