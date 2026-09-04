using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace WUv4Powertools;

// What the import dialog decided to do with one entry, which drives both the reason column and
// whether the row starts ticked.
public enum ImportKind
{
	// The provider already has this code in this language. Nothing to do.
	AlreadyPresent,

	// The provider has this code in another language but not this one. Everything except the
	// title and the localised download comes from the sibling entry, so these import cleanly.
	LanguageGap,

	// The provider has never seen this code. The logs carry no detection block, so these come in
	// as drafts that still need a detection expression before they will work.
	NewUpdate,

	// Listed in superseded.txt.
	Superseded,

	// The entry belongs to a different provider than the tab being imported into.
	OtherProvider,

	// The provider already has this update in this language, but the log disagrees with it
	// about something that can be put right without touching what it targets.
	Correction,

	// Importing this would leave the update with no description. Windows Update logs record a
	// title, a licence link and a details link, but never the description text itself, so an
	// entry with no description already in the provider has nowhere to get one.
	NoDescription
}

// One update recovered from an iuhist.xml entry, joined with anything the .log files add about it.
public sealed class ImportCandidate
{
	public string ItemId;

	public string Provider;

	// Original casing, taken from the name attribute rather than the lowercased itemID.
	public string Code;

	public string Language;

	public string Title;

	// Read for completeness but deliberately not written into a dictionary. Consumer providers
	// store the licence as <locale>/eula.htm and nothing else, while a corporate capture points
	// at corp_eula.htm. Copying that across would put a form in the consumer files that the
	// service never served.
	public string EulaHref;

	public string DetailsHref;

	// The date the update was published, in yyyy-MM-ddTHH:mm:ss.ffff. Only a catalogue entry
	// carries one. A plain iuhist.xml records when the machine downloaded the update, which is
	// a different thing entirely and is never written into a dictionary.
	public string Timestamp;

	// True when the entry came from a catalogue file and therefore has a real published date.
	public bool HasPostedDate;

	// The authentic item GUID this language was published under, from the identity element.
	// Using it instead of a fresh GUID makes an imported row match what the real service served.
	public string ItemGuid;

	// The last field of the itemID, as in com_microsoft.agent2_95.2_00_0_2202. It belongs to the
	// update rather than to the operating system, so it is safe to take from the log.
	public string Version;

	public long Size;

	public string DownloadUrl;

	public string FileName;

	// True when the download file name carries no language token, which is how the v4 catalogue
	// expressed "one file serves every language".
	public bool SharedAcrossLanguages;

	public string SourceFile;

	public ImportKind Kind;

	public string Reason;

	public bool Selected;

	// What the log can correct on a record the provider already holds.
	public LogImportNewItems.Correction Fix;

	// Set when the same update turned up under this language with more than one title, which
	// means the client reported the language wrongly somewhere. A title from an entry like that
	// cannot be trusted to be in the language it claims, so it is never written over anything.
	public bool TitleConflict;

	// Whether installing this update actually asked for a restart, as the log observed it on a
	// real machine. Only filled in when the install finished, since a failed one always reports
	// no restart needed and that says nothing about the update.
	public string NeedsReboot;

	// EXE, ADVANCED_INF or CABFILE, taken from the install the log recorded for this download.
	public string CommandType;

	public string InstallerType;

	// The file name the installer used, taken straight from the log rather than worked out by
	// stripping a hash off the address.
	public string CleanFileName;

	// True when the machine failed to download this update. What it recorded about the file is
	// then a record of an attempt rather than of the file it actually got.
	public bool DownloadFailed;

	public string Display
	{
		get { return string.IsNullOrEmpty(Title) ? Code : Title; }
	}
}

// Codes that must never be imported. Kept in a plain text file beside the executable so the list
// can be edited without rebuilding.
public sealed class SupersededList
{
	private readonly List<string> exact = new List<string>();

	private readonly List<string> prefixes = new List<string>();

	public string Path { get; private set; }

	public int Count
	{
		get { return exact.Count + prefixes.Count; }
	}

	public static string DefaultPath
	{
		get
		{
			return System.IO.Path.Combine(
				System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
				"superseded.txt");
		}
	}

	// Written out the first time the importer runs so there is always a documented file to edit,
	// even when the application was copied somewhere without it.
	private const string Template =
		"# Superseded updates that must never be imported from logs.\r\n" +
		"#\r\n" +
		"# One update code per line. Blank lines and lines starting with # are ignored.\r\n" +
		"# Matching is case insensitive and ignores surrounding whitespace.\r\n" +
		"#\r\n" +
		"# The code is the \"name\" attribute of an itemStatus entry in iuhist.xml, for example\r\n" +
		"#\r\n" +
		"#   811630_W98_5928\r\n" +
		"#\r\n" +
		"# A trailing * matches any code starting with the text before it:\r\n" +
		"#\r\n" +
		"#   811630_W98_*\r\n" +
		"#\r\n" +
		"# Prefix with a provider and a colon to exclude it in that provider only:\r\n" +
		"#\r\n" +
		"#   win98se:811630_W98_5928\r\n" +
		"#\r\n" +
		"# This file is read every time the import dialog opens, so edits take effect without\r\n" +
		"# rebuilding or restarting the application.\r\n\r\n";

	public static SupersededList Load()
	{
		return Load(DefaultPath);
	}

	public static SupersededList Load(string path)
	{
		SupersededList list = new SupersededList();
		list.Path = path;
		try
		{
			if (!File.Exists(path))
			{
				File.WriteAllText(path, Template, Encoding.UTF8);
				return list;
			}
			foreach (string raw in File.ReadAllLines(path))
			{
				string line = raw.Trim();
				if (line.Length == 0 || line.StartsWith("#")) continue;
				if (line.EndsWith("*")) list.prefixes.Add(line.Substring(0, line.Length - 1).ToLowerInvariant());
				else list.exact.Add(line.ToLowerInvariant());
			}
		}
		catch
		{
			// An unreadable or unwritable list must not stop an import. Nothing is excluded.
		}
		return list;
	}

	public bool Excludes(string provider, string code)
	{
		if (string.IsNullOrEmpty(code)) return false;
		string bare = code.ToLowerInvariant();
		string scoped = (provider ?? string.Empty).ToLowerInvariant() + ":" + bare;
		if (exact.Contains(bare) || exact.Contains(scoped)) return true;
		foreach (string p in prefixes)
		{
			// A prefix is weighed against the provider qualified form only when it names a provider
			// itself. Weighing a bare prefix against that form lets it match the provider name
			// rather than the code, so ie6* would drop every update in ie60x instead of the few
			// whose codes really do begin with ie6.
			bool qualified = p.IndexOf(':') >= 0;
			if (qualified ? scoped.StartsWith(p) : bare.StartsWith(p)) return true;
		}
		return false;
	}
}

// Everything one run of the file picker produced.
public sealed class LogImportResult
{
	public readonly List<ImportCandidate> Candidates = new List<ImportCandidate>();

	// Files skipped because they mention Windows Update Restored rather than the original service.
	public readonly List<string> RejectedFiles = new List<string>();

	// Files skipped because they contain a date in 2012 or later, which puts them past anything
	// the v4 service ever served.
	public readonly List<string> LateDatedFiles = new List<string>();

	public readonly List<string> Warnings = new List<string>();

	// Language the logs point at, and how that was worked out, shown so the user can overrule it.
	public string DetectedLanguage = string.Empty;

	public string DetectionBasis = string.Empty;

	public readonly List<string> LanguagesSeen = new List<string>();

	// Every download the .log files reported. On its own a log says nothing about which provider
	// an update belongs to, so these are matched against the dictionaries later.
	public readonly List<LogImportParser.LogEntry> Downloads = new List<LogImportParser.LogEntry>();
}

public static class LogImportParser
{
	// Any file naming the restored service is a re-hosted copy rather than an original capture, so
	// it is dropped before a single entry is read out of it.
	private static readonly string[] RestoredMarkers =
	{
		"windowsupdaterestored",
		"windows update restored",
		"windowsupdate-restored",
		"wurestored"
	};

	// Language tokens Microsoft put in v4 download file names. A name carrying none of these served
	// every language from the one file.
	private static readonly string[] LanguageTokens =
	{
		"ENU", "ENG", "USAM", "USA", "DEU", "GER", "FRA", "FRN", "ESN", "ESP", "ITA", "NLD", "SVE",
		"DAN", "FIN", "NOR", "PLK", "CSY", "RUS", "JPN", "CHS", "CHT", "KOR", "PTB", "PTG", "TRK",
		"ELL", "GRK", "HUN", "ARA", "HEB", "THA", "SLV", "SKY", "ROM", "BGR", "HRV", "ETI", "LVI",
		"LTH", "UKR", "CAT", "EUQ"
	};

	// Localised Program Files folder names, which are the clearest language signal a .log carries.
	private static readonly Dictionary<string, string> PathLanguages =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "program files", "en" },      { "programme", "de" },
		{ "ohjelmatiedostot", "fi" },   { "archivos de programa", "es" },
		{ "programfiler", "da" },       { "programfiler (x86)", "da" },
		{ "programmi", "it" },          { "arquivos de programas", "pt" },
		{ "program", "cs" },            { "programfájlok", "hu" },
		{ "fichiers communs", "fr" },   { "program files (x86)", "en" }
	};

	// Maps the three letter download token onto the two letter dictionary locale.
	internal static readonly Dictionary<string, string> TokenLocales =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "ENU", "en" }, { "ENG", "en" }, { "USAM", "en" }, { "USA", "en" },
		{ "DEU", "de" }, { "GER", "de" }, { "FRA", "fr" }, { "FRN", "fr" },
		{ "ESN", "es" }, { "ESP", "es" }, { "ITA", "it" }, { "NLD", "nl" },
		{ "SVE", "sv" }, { "DAN", "da" }, { "FIN", "fi" }, { "NOR", "no" },
		{ "PLK", "pl" }, { "CSY", "cs" }, { "RUS", "ru" }, { "JPN", "ja" },
		{ "CHS", "zhcn" }, { "CHT", "zhtw" }, { "KOR", "ko" }, { "PTB", "ptbr" },
		{ "PTG", "pt" }, { "TRK", "tr" }, { "ELL", "el" }, { "GRK", "el" },
		{ "HUN", "hu" }, { "ARA", "ar" }, { "HEB", "he" }
	};

	// Dates written inside the file, which is the only thing that counts. A file's modified time
	// says nothing about when its contents were recorded, so it is never consulted.
	// Only text shaped like a real date is read, because the fraction of a timestamp can look
	// like a year on its own: 2003-02-18T14:08:20.2020 ends in 2020 and is still 2003.
	private static readonly Regex DateShaped = new Regex(@"(?<![0-9])((?:19|20)[0-9]{2})-([01][0-9])-([0-3][0-9])");

	public const int FirstRejectedYear = 2012;

	// The latest year written inside the file, or zero when it holds no dates at all.
	public static int LatestYearInside(string path)
	{
		int latest = 0;
		try
		{
			foreach (Match m in DateShaped.Matches(File.ReadAllText(path, Encoding.UTF8)))
			{
				int year;
				int month;
				int day;
				if (!int.TryParse(m.Groups[1].Value, out year)) continue;
				if (!int.TryParse(m.Groups[2].Value, out month) || month < 1 || month > 12) continue;
				if (!int.TryParse(m.Groups[3].Value, out day) || day < 1 || day > 31) continue;
				if (year > latest) latest = year;
			}
		}
		catch
		{
			// An unreadable file is handled by the caller, which refuses to use it.
		}
		return latest;
	}

	// Whether a piece of text names the restored service. Used on an address as well as on a
	// whole file, so nothing carrying it is ever written into an inventory.
	public static bool NamesRestoredService(string text)
	{
		if (string.IsNullOrEmpty(text)) return false;

		string lower = text.ToLowerInvariant();
		foreach (string marker in RestoredMarkers)
		{
			if (lower.Contains(marker)) return true;
		}

		return false;
	}

	// Read as bytes and weighed in every encoding a log turns up in. Reading it as one encoding
	// and hoping was not good enough: a log written as UTF-16 read back as UTF-8 has a nul between
	// every letter, so the name never matched and the whole file was let through and used.
	public static bool MentionsRestored(string path)
	{
		try
		{
			byte[] bytes = File.ReadAllBytes(path);
			foreach (Encoding encoding in new Encoding[]
			{
				Encoding.UTF8, Encoding.GetEncoding(28591), Encoding.Unicode, Encoding.BigEndianUnicode
			})
			{
				string lower;
				try
				{
					lower = encoding.GetString(bytes).ToLowerInvariant();
				}
				catch
				{
					continue;
				}

				foreach (string marker in RestoredMarkers)
				{
					if (lower.Contains(marker)) return true;
				}
			}

			return false;
		}
		catch
		{
			// Unreadable files are treated as unusable rather than trusted.
			return true;
		}
	}

	public static LogImportResult Parse(IEnumerable<string> xmlPaths, IEnumerable<string> logPaths)
	{
		LogImportResult result = new LogImportResult();

		// The logs are read first because they supply the download URL that the XML lacks.
		List<LogEntry> downloads = new List<LogEntry>();
		List<string> pathLanguageVotes = new List<string>();
		List<string> tokenLanguageVotes = new List<string>();

		foreach (string path in logPaths ?? Enumerable.Empty<string>())
		{
			if (!Usable(path, result)) continue;

			// Weighed one file at a time. Pooling every log's votes together picked a single
			// language for the whole run, so importing a folder of logs from machines in
			// different languages filed most of them under whichever language won the count.
			int firstDownload = downloads.Count;
			List<string> pathsHere = new List<string>();
			List<string> tokensHere = new List<string>();
			ReadLog(path, downloads, pathsHere, tokensHere, result);

			string spoken = Commonest(pathsHere) ?? Commonest(tokensHere);
			for (int i = firstDownload; i < downloads.Count; i++)
			{
				downloads[i].LogLanguage = spoken;
				downloads[i].SourceFile = Path.GetFileName(path);
			}

			pathLanguageVotes.AddRange(pathsHere);
			tokenLanguageVotes.AddRange(tokensHere);
		}

		foreach (string path in xmlPaths ?? Enumerable.Empty<string>())
		{
			if (!Usable(path, result)) continue;
			ReadHistory(path, downloads, result);
		}

		result.Downloads.AddRange(downloads);
		Deduplicate(result);
		DecideLanguage(result, pathLanguageVotes, tokenLanguageVotes);
		return result;
	}

	// A file is only read when it came from the original service and holds nothing dated past the
	// end of what v4 ever served.
	private static bool Usable(string path, LogImportResult result)
	{
		if (MentionsRestored(path))
		{
			result.RejectedFiles.Add(Path.GetFileName(path));
			return false;
		}
		int latest = LatestYearInside(path);
		if (latest >= FirstRejectedYear)
		{
			result.LateDatedFiles.Add(Path.GetFileName(path) + " (" + latest + ")");
			return false;
		}
		return true;
	}

	// The same machine reports an update once per session, and several history files from the
	// same machine repeat it again, so the list is reduced to one row per update and language.
	// The row keeping a download address wins, since that is the part hardest to recover.
	private static void Deduplicate(LogImportResult result)
	{
		Dictionary<string, ImportCandidate> best =
			new Dictionary<string, ImportCandidate>(StringComparer.OrdinalIgnoreCase);
		List<ImportCandidate> order = new List<ImportCandidate>();
		List<string> conflicts = new List<string>();

		foreach (ImportCandidate c in result.Candidates)
		{
			string key = c.Provider + "|" + c.Code + "|" + c.Language;
			ImportCandidate existing;
			if (!best.TryGetValue(key, out existing))
			{
				best[key] = c;
				order.Add(c);
				continue;
			}
			if (!string.IsNullOrEmpty(existing.Title) && !string.IsNullOrEmpty(c.Title) &&
				!string.Equals(existing.Title, c.Title, StringComparison.Ordinal))
			{
				conflicts.Add(c.Code + " (" + c.Language + ")");
			existing.TitleConflict = true;
			c.TitleConflict = true;
			}
			if (string.IsNullOrEmpty(existing.DownloadUrl) && !string.IsNullOrEmpty(c.DownloadUrl))
			{
				order[order.IndexOf(existing)] = c;
				best[key] = c;
			}
		}

		if (conflicts.Count > 0)
		{
			result.Warnings.Add(string.Format(
				"{0} update{1} have more than one title under one language, so the reported language may be wrong. Check the titles below: {2}",
				conflicts.Count, conflicts.Count == 1 ? string.Empty : "s",
				string.Join(", ", conflicts.Distinct().Take(6).ToArray())));
		}

		result.Candidates.Clear();
		result.Candidates.AddRange(order);
	}

	public sealed class LogEntry
	{
		public string Url;
		public string FileName;
		public bool Shared;

		// The file name with the trailing content hash removed, which is what a code is compared against.
		public string Core;

		// Null when the file name carried no language tag, meaning it served every language.
		public string Locale;

		// The language of the machine whose log recorded this download. A bulk import covers
		// machines in many languages, so this belongs to the file rather than to the run.
		public string LogLanguage;

		public string SourceFile;

		// The six digit article number, which is the only key the two file formats share.
		public string Article;

		// How the update was installed, recovered by pairing this download with the install that
		// followed it in the same session.
		public string CommandType;

		public string InstallerType;

		// The update code this download belongs to, read from the local path the log writes on
		// the very next line. This is stated outright rather than inferred, so it beats every
		// other way of tying a download to an update.
		public string ItemCode;

		// The file name as the installer refers to it, already without the content hash.
		public string CleanName;
	}

	// v4 named the downloaded file after the article, while the history file names it after the
	// item code. Neither is derivable from the other, so a download is matched on the article
	// number once the obvious comparisons fail.
	private static string ArticleOf(string text)
	{
		if (string.IsNullOrEmpty(text)) return null;
		Match m = Regex.Match(text, @"(?:kb|q)?(\d{6})", RegexOptions.IgnoreCase);
		return m.Success ? m.Groups[1].Value : null;
	}

	// A download line looks like
	//   2003-08-14 12:01:44  Success  IUENGINE  Downloaded file http://download.windowsupdate.com/...
	// and the file name is the only place the update's language shows up.
	private static void ReadLog(string path, List<LogEntry> downloads,
		List<string> pathVotes, List<string> tokenVotes, LogImportResult result)
	{
		// A session lists everything it downloaded first, then the installs in the same order, so
		// the n-th install describes the n-th download. The log never names the item on an install
		// line, and this ordering is the only thing tying the two together.
		List<LogEntry> sessionDownloads = new List<LogEntry>();
		LogEntry lastDownload = null;
		List<string> sessionCommands = new List<string>();
		List<string> sessionInstallers = new List<string>();

		try
		{
			foreach (string line in File.ReadAllLines(path, Encoding.GetEncoding(28591)))
			{
				if (Regex.IsMatch(line, @"IUENGINE\s+Starting", RegexOptions.IgnoreCase))
				{
					PairSession(sessionDownloads, sessionCommands, sessionInstallers);
					sessionDownloads.Clear();
					sessionCommands.Clear();
					sessionInstallers.Clear();
				}

				// Every download is followed immediately by the local path it was written to, and that
				// path names the update it belongs to and the file the installer will run.
				Match local = LocalPath.Match(line);
				if (local.Success && lastDownload != null)
				{
					lastDownload.ItemCode = local.Groups[1].Value.Trim();
					lastDownload.CleanName = local.Groups[2].Value.Trim();
					lastDownload = null;
				}

				Match installing = Regex.Match(line, @"Installing\s+(\S+)\s+item from publisher", RegexOptions.IgnoreCase);
				if (installing.Success)
				{
					sessionInstallers.Add(installing.Groups[1].Value);
					sessionCommands.Add(null);
				}

				Match commandType = Regex.Match(line, @"Installer Command Type:\s*(\S+)", RegexOptions.IgnoreCase);
				if (commandType.Success && sessionCommands.Count > 0)
				{
					sessionCommands[sessionCommands.Count - 1] = commandType.Groups[1].Value;
				}

				int marker = line.IndexOf("WindowsUpdate\\V4", StringComparison.OrdinalIgnoreCase);
				if (marker > 0)
				{
					// The folder is the last part of the path, taken at the backslash. Cutting at the
					// last space first threw away every folder whose name contains one, so "Program
					// Files" came out as "Files" and matched nothing at all.
					string head = line.Substring(0, marker).TrimEnd('\\');
					string leaf = head.Contains("\\") ? head.Substring(head.LastIndexOf('\\') + 1) : head;
					foreach (KeyValuePair<string, string> pair in PathLanguages)
					{
						if (leaf.Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
						{
							pathVotes.Add(pair.Value);
							break;
						}
					}
				}

				foreach (Match m in Regex.Matches(line, @"https?://[^\s""]+", RegexOptions.IgnoreCase))
				{
					string url = m.Value;
					if (url.IndexOf("/cabpool/", StringComparison.OrdinalIgnoreCase) < 0) continue;

					string file = url.Substring(url.LastIndexOf('/') + 1);
					// v4 appended a content hash after the last underscore. The part before it is the
					// real file name and the only piece worth matching on.
					string core = file.Contains("_") ? file.Substring(0, file.LastIndexOf('_')) : file;
					string token = LanguageTokenOf(core);
					if (token != null && TokenLocales.ContainsKey(token)) tokenVotes.Add(TokenLocales[token]);

					LogEntry entry = new LogEntry
					{
						Url = url,
						FileName = file,
						Shared = token == null,
						Core = StripLanguageToken(core, token),
						Locale = token != null && TokenLocales.ContainsKey(token) ? TokenLocales[token] : null,
						Article = ArticleOf(core)
					};
					downloads.Add(entry);
					sessionDownloads.Add(entry);
					lastDownload = entry;
				}
			}
		}
		catch (Exception ex)
		{
			result.Warnings.Add(Path.GetFileName(path) + ": " + ex.Message);
		}

		PairSession(sessionDownloads, sessionCommands, sessionInstallers);
	}

	// Ties each download to the install that came at the same position in its session. A session
	// often downloads more than it goes on to install, so only as far as both lists reach.
	private static void PairSession(List<LogEntry> downloads, List<string> commands,
		List<string> installers)
	{
		int paired = Math.Min(downloads.Count, commands.Count);
		for (int i = 0; i < paired; i++)
		{
			if (commands[i] == null) continue;
			downloads[i].CommandType = commands[i];
			if (i < installers.Count) downloads[i].InstallerType = installers[i];
		}
	}

	// Local path c:\\WUTemp\\com_microsoft.<update code>\\<file the installer runs>
	private static readonly Regex LocalPath = new Regex(
		@"Local path\s+.*?com_microsoft\.([^\\\\/]+)[\\\\/]([^\s\\\\/]+)\s*$",
		RegexOptions.IgnoreCase);

	// The language tag inside a download file name, or null when the file served every language.
	internal static string LanguageTokenOf(string name)
	{
		if (string.IsNullOrEmpty(name)) return null;

		// A trailing extension hides a tag that runs to the end of the name, which is how the
		// 888113USA8.EXE family is written. Only a short tail counts as an extension, so a name
		// like IE6.0sp1-KB837009-x86-PLK keeps its own dots.
		int dot = name.LastIndexOf('.');
		string stem = dot > 0 && name.Length - dot <= 5 ? name.Substring(0, dot) : name;

		foreach (string token in LanguageTokens)
		{
			if (Regex.IsMatch(stem, "[_\\-.]" + token + "([_\\-.]|$)", RegexOptions.IgnoreCase)) return token;
			// Some 9x names ran the token straight onto the KB number, as in 823559FIN8.
			if (Regex.IsMatch(stem, "[0-9]" + token + "[0-9]?$", RegexOptions.IgnoreCase)) return token;
		}
		return null;
	}

	internal static string StripLanguageToken(string name, string token)
	{
		if (token == null) return name;
		string stripped = Regex.Replace(name, "[_\\-.]" + token + "([_\\-.]|$)", "$1", RegexOptions.IgnoreCase);
		return Regex.Replace(stripped, "([0-9])" + token + "([0-9]?)$", "$1$2", RegexOptions.IgnoreCase);
	}

	// v4 appended an underscore and a content hash to the file in the cabpool, while the name the
	// installer refers to has no hash. Importing the hashed form as the name is wrong, so it is
	// taken off whenever the tail looks like a hash rather than part of the name.
	internal static string StripHash(string fileName)
	{
		if (string.IsNullOrEmpty(fileName)) return fileName;
		int dot = fileName.LastIndexOf('.');
		string stem = dot < 0 ? fileName : fileName.Substring(0, dot);
		string ext = dot < 0 ? string.Empty : fileName.Substring(dot);
		Match m = Regex.Match(stem, @"^(.*)_[0-9a-fA-F]{8,}$");
		return m.Success ? m.Groups[1].Value + ext : fileName;
	}

	// Puts a different language's tag in place of the one a file name already carries, keeping the
	// surrounding punctuation and the case the original used. Returns null when the name carries no
	// tag, which means one file served every language and nothing should be changed.
	internal static string SwapLanguageToken(string fileName, string newToken)
	{
		if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(newToken)) return null;

		int dot = fileName.LastIndexOf('.');
		string stem = dot < 0 ? fileName : fileName.Substring(0, dot);
		string ext = dot < 0 ? string.Empty : fileName.Substring(dot);

		string token = LanguageTokenOf(stem);
		if (token == null) return null;

		// Match how the original was written, since these names run from Windows98-KB918547-ENU to
		// 873374_eng within the same folder.
		// The table entry is always upper case, so the case has to come from the text that is
		// actually in the name.
		int at = stem.IndexOf(token, StringComparison.OrdinalIgnoreCase);
		string asWritten = at >= 0 ? stem.Substring(at, token.Length) : token;
		string replacement = asWritten.ToUpperInvariant() == asWritten
			? newToken.ToUpperInvariant()
			: newToken.ToLowerInvariant();

		string swapped = Regex.Replace(stem, "([_\\-.])" + token + "([_\\-.]|$)",
			"${1}" + replacement + "$2", RegexOptions.IgnoreCase);
		if (swapped != stem) return swapped + ext;

		// The other shape runs the tag straight onto the article number, as in 888113USA8.
		swapped = Regex.Replace(stem, "([0-9])" + token + "([0-9]?)$",
			"${1}" + replacement + "$2", RegexOptions.IgnoreCase);
		return swapped == stem ? null : swapped + ext;
	}

	private static void ReadHistory(string path, List<LogEntry> downloads, LogImportResult result)
	{
		XmlDocument doc = new XmlDocument();
		doc.XmlResolver = null;
		try
		{
			doc.Load(path);
		}
		catch (Exception ex)
		{
			result.Warnings.Add(Path.GetFileName(path) + ": " + ex.Message);
			return;
		}

		XmlNodeList nodes = doc.GetElementsByTagName("itemStatus");
		foreach (XmlNode node in nodes)
		{
			XmlNode identity = Child(node, "identity");
			if (identity == null) continue;

			string itemId = Attr(identity, "itemID");
			string code = Attr(identity, "name");
			if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(code)) continue;

			string[] parts = itemId.Split('.');
			if (parts.Length < 15) continue;

			XmlNode description = Child(node, "description");
			XmlNode descriptionText = description == null ? null : Child(description, "descriptionText");

			ImportCandidate candidate = new ImportCandidate
			{
				ItemId = itemId,
				Provider = parts[0],
				Code = code,
				Language = Text(Child(identity, "language")),
				Title = descriptionText == null ? string.Empty : Text(Child(descriptionText, "title")),
				EulaHref = descriptionText == null ? string.Empty : Attr(Child(descriptionText, "eula"), "href"),
				DetailsHref = descriptionText == null ? string.Empty : Attr(Child(descriptionText, "details"), "href"),
				// Only the description carries a published date. The timestamp on itemStatus is when this
				// machine downloaded the update, so it is deliberately not read.
				Timestamp = NormaliseStamp(Attr(description, "timestamp")),
				ItemGuid = Text(Child(identity, "guid")).ToUpperInvariant(),
				Version = parts[14],
				SourceFile = Path.GetFileName(path)
			};
			candidate.HasPostedDate = !string.IsNullOrEmpty(candidate.Timestamp);
			// A restart flag is only evidence when the install actually finished. Of the failed ones in
			// a real capture, almost every single one says needsReboot=0 simply because nothing ran.
			XmlNode install = Child(node, "installStatus");
			if (string.Equals(Attr(install, "value"), "COMPLETE", StringComparison.OrdinalIgnoreCase))
			{
				candidate.NeedsReboot = Attr(install, "needsReboot");
			}
			candidate.DownloadFailed = string.Equals(
				Attr(Child(node, "downloadStatus"), "value"), "FAILED", StringComparison.OrdinalIgnoreCase);

			if (string.IsNullOrEmpty(candidate.Language)) candidate.Language = parts[6];

			long size;
			if (description != null && long.TryParse(Text(Child(description, "size")), out size)) candidate.Size = size;

			// The XML only records where the file landed locally, so the download is recovered by
			// matching that leaf name against what the .log files reported downloading.
			string leaf = Text(Child(node, "downloadPath"));
			if (!string.IsNullOrEmpty(leaf))
			{
				leaf = leaf.Contains("\\") ? leaf.Substring(leaf.LastIndexOf('\\') + 1) : leaf;
				if (leaf.StartsWith("com_microsoft.", StringComparison.OrdinalIgnoreCase))
				{
					leaf = leaf.Substring("com_microsoft.".Length);
				}
			}

			LogEntry hit = Lookup(downloads, candidate.Language, leaf, code);
			if (hit != null)
			{
				candidate.DownloadUrl = hit.Url;
				candidate.FileName = hit.FileName;
				candidate.SharedAcrossLanguages = hit.Shared;
				candidate.CommandType = hit.CommandType;
				candidate.CleanFileName = hit.CleanName;
				candidate.InstallerType = hit.InstallerType;
			}

			result.Candidates.Add(candidate);
			if (!string.IsNullOrEmpty(candidate.Language) && !result.LanguagesSeen.Contains(candidate.Language))
			{
				result.LanguagesSeen.Add(candidate.Language);
			}
		}
	}

	// Tried in order of how much the match can be trusted: the same name, then a name the code
	// merely starts with, then the same article number provided the languages agree.
	// Whether a download may be used for this language. A file whose name states a language is
	// that language's own and no other's, while a name with no language tag is the one file
	// every language downloads. Only the last of the searches below used to weigh this, so an
	// English file matched on its update code alone and ended up on the Czech row.
	private static bool Fits(LogEntry e, string language)
	{
		if (e == null) return false;
		if (e.Locale == null) return true;
		if (string.IsNullOrEmpty(language)) return false;

		return string.Equals(e.Locale, language, StringComparison.OrdinalIgnoreCase);
	}

	private static LogEntry Lookup(List<LogEntry> downloads, string language, params string[] keys)
	{
		// The log states which update a download belongs to, so that is tried before anything is
		// inferred from the file name. A third of these updates are named nothing like their file,
		// so without this they could not be matched at all.
		foreach (string key in keys)
		{
			if (string.IsNullOrEmpty(key)) continue;
			foreach (LogEntry e in downloads)
			{
				if (!Fits(e, language)) continue;
				if (string.Equals(e.ItemCode, key, StringComparison.OrdinalIgnoreCase)) return e;
			}
		}

		foreach (string key in keys)
		{
			if (string.IsNullOrEmpty(key)) continue;
			foreach (LogEntry e in downloads)
			{
				if (!Fits(e, language)) continue;
				if (string.Equals(e.Core, key, StringComparison.OrdinalIgnoreCase)) return e;
			}
		}

		foreach (string key in keys)
		{
			if (string.IsNullOrEmpty(key)) continue;
			foreach (LogEntry e in downloads)
			{
				if (!Fits(e, language)) continue;
				if (!string.IsNullOrEmpty(e.ItemCode) &&
					!string.Equals(e.ItemCode, key, StringComparison.OrdinalIgnoreCase)) continue;
				if (e.Core != null && e.Core.Length >= 6 &&
					key.StartsWith(e.Core, StringComparison.OrdinalIgnoreCase)) return e;
			}
		}

		foreach (string key in keys)
		{
			string article = ArticleOf(key);
			if (article == null) continue;
			foreach (LogEntry e in downloads)
			{
				if (e.Article != article) continue;

				// The same article number ships a separate file for each operating system, so a
				// download the log has already tied to a different update is never accepted here.
				if (!string.IsNullOrEmpty(e.ItemCode) &&
					!string.Equals(e.ItemCode, key, StringComparison.OrdinalIgnoreCase)) continue;

				if (!Fits(e, language)) continue;
				return e;
			}
		}

		return null;
	}

	// items.txt stores yyyy-MM-ddTHH:mm:ss.ffff, which is exactly what a catalogue description
	// carries. Nothing else in these files is a published date, so there is no fallback: an entry
	// without one simply has no date to offer.
	private static string NormaliseStamp(string posted)
	{
		foreach (string value in new[] { posted })
		{
			if (string.IsNullOrEmpty(value)) continue;
			DateTime parsed;
			if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
			{
				int dot = value.LastIndexOf('.');
				string fraction = dot > 0 && dot + 1 < value.Length ? value.Substring(dot + 1) : string.Empty;
				fraction = new string(fraction.TakeWhile(char.IsDigit).ToArray());
				if (fraction.Length == 0) fraction = "0000";
				else if (fraction.Length > 4) fraction = fraction.Substring(0, 4);
				else fraction = fraction.PadRight(4, '0');
				return parsed.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) + "." + fraction;
			}
		}
		return string.Empty;
	}

	// The value seen most often, or null when there is nothing to go on.
	private static string Commonest(List<string> votes)
	{
		if (votes == null || votes.Count == 0) return null;

		return votes.GroupBy(v => v).OrderByDescending(g => g.Count()).First().Key;
	}

	private static void DecideLanguage(LogImportResult result, List<string> pathVotes, List<string> tokenVotes)
	{
		// The history file states the language outright, so it wins whenever there is one. The
		// keyword heuristics only matter when importing from a .log with no matching XML.
		var fromXml = result.Candidates
			.Where(c => !string.IsNullOrEmpty(c.Language))
			.GroupBy(c => c.Language)
			.OrderByDescending(g => g.Count())
			.ToList();

		if (fromXml.Count > 0)
		{
			result.DetectedLanguage = fromXml[0].Key;
			result.DetectionBasis = string.Format(
				"stated in {0} of {1} history entries", fromXml[0].Count(), result.Candidates.Count);
			if (fromXml.Count > 1)
			{
				result.DetectionBasis += ", but the files also contain " +
					string.Join(", ", fromXml.Skip(1).Select(g => g.Key + " (" + g.Count() + ")").ToArray());
			}
			return;
		}

		if (pathVotes.Count > 0)
		{
			var top = pathVotes.GroupBy(v => v).OrderByDescending(g => g.Count()).First();
			result.DetectedLanguage = top.Key;
			result.DetectionBasis = "guessed from the localised Program Files folder in the log";
			return;
		}

		if (tokenVotes.Count > 0)
		{
			var top = tokenVotes.GroupBy(v => v).OrderByDescending(g => g.Count()).First();
			result.DetectedLanguage = top.Key;
			result.DetectionBasis = string.Format(
				"guessed from language tags in {0} download file names", tokenVotes.Count);
			return;
		}

		result.DetectionBasis = "could not be determined, please choose one";
	}

	private static XmlNode Child(XmlNode parent, string name)
	{
		if (parent == null) return null;
		foreach (XmlNode child in parent.ChildNodes)
		{
			if (string.Equals(child.LocalName, name, StringComparison.OrdinalIgnoreCase)) return child;
		}
		return null;
	}

	private static string Attr(XmlNode node, string name)
	{
		if (node == null || node.Attributes == null) return string.Empty;
		XmlAttribute a = node.Attributes[name];
		return a == null ? string.Empty : a.Value.Trim();
	}

	private static string Text(XmlNode node)
	{
		return node == null ? string.Empty : node.InnerText.Trim();
	}
}
