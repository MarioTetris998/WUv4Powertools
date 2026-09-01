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
	// Builds candidates from the downloads a log recorded, for every provider in the folder that has
	// an update with the same article number. Only used when no history file was supplied, since a
	// history file states all of this outright and far more reliably.
	public static int AddCandidates(LogImportResult result, ConsumerDictionary dictionary)
	{
		if (result == null || dictionary == null) return 0;
		if (result.Downloads.Count == 0) return 0;

		int added = 0;
		foreach (ProviderStore store in dictionary.Providers)
		{
			ProviderIndex index = store.Index;

			foreach (LogImportParser.LogEntry download in result.Downloads)
			{
				if (download.Article == null) continue;

				// A file with no language tag served every language, so it says nothing about which
				// language is missing and is left for the history file to describe.
				if (download.Locale == null) continue;

				foreach (string code in index.CodesWithArticle(download.Article))
				{
					string existing = index.ItemsLineFor(code, download.Locale);
					bool present = existing != null;

					// The file the provider records for this language, if it has one at all.
					string currentLeaf = present ? LogImportEngine.LeafOfLine(existing) : null;
					bool sameFile = currentLeaf != null &&
						string.Equals(currentLeaf, download.FileName, StringComparison.OrdinalIgnoreCase);
					if (present && sameFile) continue;

					ImportCandidate candidate = new ImportCandidate
					{
						Provider = store.Name,
						Code = code,
						Language = download.Locale,
						DownloadUrl = download.Url,
						FileName = download.FileName,
						SharedAcrossLanguages = download.Shared,
						SourceFile = "Windows Update.log",
						// A log records a download, never a publication, so it can offer no date and
						// no title. Those only ever come from a catalogue file.
						Title = string.Empty,
						Timestamp = string.Empty,
						HasPostedDate = false,
						ItemId = index.ItemIdFor(code, download.Locale) ?? index.SiblingItemId(code)
					};

					result.Candidates.Add(candidate);
					added++;
				}
			}
		}

		return added;
	}
}
