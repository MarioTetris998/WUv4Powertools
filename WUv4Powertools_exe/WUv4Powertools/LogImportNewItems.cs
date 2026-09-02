using System;
using System.Collections.Generic;
using System.Linq;

namespace WUv4Powertools;

// Building a record for an update the provider has never held, and correcting one it already has.
// Both need more of the log than a plain language fill-in does, so they live apart from the main
// import to keep that path readable.
public static class LogImportNewItems
{
	// Put in the detection field of an update the logs could not supply a rule for. The key it names
	// never exists, so the update is never offered to a machine until a real rule replaces it, and
	// the update list looks for exactly this text to mark the row as still needing work.
	public const string PlaceholderDetectionKey =
		"HKEY_LOCAL_MACHINE\\Software\\WUv4PowerTools\\NeedsDetection";

	public static readonly string PlaceholderDetection =
		"<detection><installed><expression><regKeyExists><key>" + PlaceholderDetectionKey +
		"</key></regKeyExists></expression></installed></detection>";

	// True for an update that came in from a log without a detection rule, which is what the list
	// uses to show it in bold.
	public static bool NeedsAttention(string itemsLine)
	{
		return !string.IsNullOrEmpty(itemsLine) &&
			itemsLine.IndexOf(PlaceholderDetectionKey, StringComparison.OrdinalIgnoreCase) >= 0;
	}

	// The five parts a brand new update needs. The identifier comes from the log itself, because
	// with nothing already in the provider under this code there is no local record to prefer.
	public static string BuildItemsLine(ImportCandidate c, string langGuid, string group,
		string fileGuid, string installation)
	{
		string critical = "4";
		long size = c.Size > 0 ? c.Size : 0;
		string stamp = c.HasPostedDate ? c.Timestamp : string.Empty;

		return string.Join("@|", new[]
		{
			fileGuid + "," + c.Code,
			"com_microsoft",
			langGuid,
			group,
			PlaceholderDetection,
			installation,
			"1",
			critical,
			size.ToString(),
			stamp,
			"0",
			"0",
			string.Empty,
			"-768"
		});
	}

	// A download block for an update whose file the log recorded. With no address at all the block
	// still has to be well formed, because the catalogue parses each file whole and one broken
	// record stops the entire provider loading.
	public static string BuildInstallation(ImportCandidate c)
	{
		string url = c.DownloadUrl ?? string.Empty;
		string name = LogImportParser.StripHash(c.FileName) ?? string.Empty;
		long size = c.Size > 0 ? c.Size : 0;

		return string.Format(
			"<installation order=\"0\" installerType=\"SOFTWARE\" exclusive=\"0\" needsReboot=\"0\">" +
			"<size>{0}</size>" +
			"<codeBase href=\"{1}\" crc=\"\" name=\"{2}\"><size>{0}</size></codeBase>" +
			"<command order=\"0\" commandType=\"EXE\">{2}<switches>/q:a /r:n</switches></command>" +
			"</installation>",
			size, url, name);
	}

	// Whether a title out of a log may be written over what the provider already holds.
	// English is the language this application translates everything else from, so its title is
	// the one thing an import must never overwrite: get that wrong and every other language
	// inherits the mistake the next time the strings are rebuilt. A title from an entry whose
	// language the client reported inconsistently is refused for the same reason.
	public static bool TitleUsable(ImportCandidate c, string locale)
	{
		if (c == null || string.IsNullOrEmpty(c.Title)) return false;
		if (string.Equals(locale, "en", StringComparison.OrdinalIgnoreCase)) return false;
		return !c.TitleConflict;
	}

	// What the log can put right about a record the provider already has. Nothing here touches the
	// operating system the update targets.
	public sealed class Correction
	{
		public bool Guid;
		public bool Title;
		public bool Version;
		public bool FileName;

		// The provider spells the update code with different capitals than the service did.
		public bool Capitalisation;

		// How the service spelled it, kept so the rename knows what to write.
		public string CodeAsPublished;

		// The log watched the install ask for a restart, and the provider claims otherwise.
		public bool Reboot;

		// What the log observed, as the 0 or 1 an installation block stores.
		public string RebootAsObserved;

		// The provider runs the update a different way than the log watched it run.
		public bool CommandType;

		public string CommandTypeAsObserved;

		public bool Any
		{
			get
			{
				return Guid || Title || Version || FileName || Capitalisation || Reboot || CommandType;
			}
		}

		public override string ToString()
		{
			List<string> parts = new List<string>();
			if (Guid) parts.Add("GUID");
			if (Title) parts.Add("title");
			if (Version) parts.Add("version");
			if (FileName) parts.Add("file name");
			if (Capitalisation) parts.Add("capitals");
			if (Reboot) parts.Add("restart flag");
			if (CommandType) parts.Add("installer type");
			return string.Join(", ", parts.ToArray());
		}
	}

	// The same code apart from its capitals, which is the only thing worth putting right here.
	// A different code altogether is a different update and is never touched.
	private static bool MisspelledCapitals(string stored, string published)
	{
		return !string.IsNullOrEmpty(stored) &&
			string.Equals(stored, published, StringComparison.OrdinalIgnoreCase) &&
			!string.Equals(stored, published, StringComparison.Ordinal);
	}

	// The update code as written in an items.txt row, which is the only file keeping its capitals.
	public static string CodeOf(string itemsLine)
	{
		if (string.IsNullOrEmpty(itemsLine)) return null;
		string head = itemsLine.Split(new[] { "@|" }, StringSplitOptions.None)[0];
		int comma = head.IndexOf(',');
		return comma < 0 ? null : head.Substring(comma + 1);
	}

	// The command type an installation block runs the update with.
	public static string CommandTypeOf(string itemsLine)
	{
		if (string.IsNullOrEmpty(itemsLine)) return null;
		int at = itemsLine.IndexOf("commandType=\"", StringComparison.Ordinal);
		if (at < 0) return null;
		at += "commandType=\"".Length;
		int end = itemsLine.IndexOf('\"', at);
		return end < 0 ? null : itemsLine.Substring(at, end - at);
	}

	// The restart flag on an installation block, as the 0 or 1 it stores.
	public static string RebootOf(string itemsLine)
	{
		if (string.IsNullOrEmpty(itemsLine)) return null;
		int at = itemsLine.IndexOf("<installation", StringComparison.Ordinal);
		if (at < 0) return null;
		int flag = itemsLine.IndexOf("needsReboot=\"", at, StringComparison.Ordinal);
		if (flag < 0) return null;
		flag += "needsReboot=\"".Length;
		int end = itemsLine.IndexOf('\"', flag);
		return end < 0 ? null : itemsLine.Substring(flag, end - flag);
	}

	// Compares what the log says against what the provider holds for the same update and language.
	public static Correction Compare(ImportCandidate c, string locale, ProviderIndex index,
		string existingItemsLine, string existingItemId, string existingTitle)
	{
		Correction correction = new Correction();

		// The provider and the log agree on the code apart from its capitals. The log carries the
		// service's own spelling, so it wins: 836528_All_OS_DoomCLn becomes 836528_All_OS_DoomCln.
		// The code is written in two places that can disagree with each other, the items row and
		// the identifier in itemsindex, so both are compared.
		if (!string.IsNullOrEmpty(c.Code))
		{
			string inRow = CodeOf(existingItemsLine);
			string inId = null;
			if (!string.IsNullOrEmpty(existingItemId))
			{
				string[] idParts = existingItemId.Split('.');
				if (idParts.Length >= 15) inId = idParts[13];
			}

			if (MisspelledCapitals(inRow, c.Code) || MisspelledCapitals(inId, c.Code))
			{
				correction.Capitalisation = true;
				correction.CodeAsPublished = c.Code;
			}
		}

		// The log watched the update install, so it knows how it was run. The install line never
		// names the item, so this came from pairing it with the download at the same position in
		// the session, which is the order the log writes them in.
		if (!string.IsNullOrEmpty(c.CommandType) && !string.IsNullOrEmpty(existingItemsLine))
		{
			string current = CommandTypeOf(existingItemsLine);
			if (!string.IsNullOrEmpty(current) &&
				!string.Equals(current, c.CommandType, StringComparison.OrdinalIgnoreCase))
			{
				correction.CommandType = true;
				correction.CommandTypeAsObserved = c.CommandType;
			}
		}

		// A log records what installing actually did on a real machine, which is better evidence
		// than whatever the provider currently claims.
		if (!string.IsNullOrEmpty(c.NeedsReboot) && !string.IsNullOrEmpty(existingItemsLine))
		{
			string current = RebootOf(existingItemsLine);
			if (current != null && current != c.NeedsReboot)
			{
				correction.Reboot = true;
				correction.RebootAsObserved = c.NeedsReboot;
			}
		}

		// Offered only when the published GUID is free. The same update sometimes served two
		// languages from one row, so the GUID a log gives for one language can already belong to
		// another. Moving onto it would leave two rows sharing an identity, so it is left alone
		// rather than offered every time and never applied.
		if (!string.IsNullOrEmpty(c.ItemGuid) && !string.IsNullOrEmpty(existingItemsLine))
		{
			int comma = existingItemsLine.IndexOf(',');
			string current = comma > 0 ? existingItemsLine.Substring(0, comma).Trim() : string.Empty;
			correction.Guid =
				!string.Equals(current, c.ItemGuid, StringComparison.OrdinalIgnoreCase) &&
				(index == null || !index.HasItemGuid(c.ItemGuid));
		}

		if (TitleUsable(c, locale) && existingTitle != null)
		{
			correction.Title = !string.Equals(existingTitle, c.Title, StringComparison.Ordinal);
		}

		if (!string.IsNullOrEmpty(c.Version) && !string.IsNullOrEmpty(existingItemId))
		{
			string[] parts = existingItemId.Split('.');
			if (parts.Length >= 15)
			{
				correction.Version = !string.Equals(parts[14], c.Version, StringComparison.OrdinalIgnoreCase);
			}
		}

		// A machine that failed to fetch the file only recorded what it tried, so its file name
		// is not good enough to overwrite one that is already there.
		if (!c.DownloadFailed && !string.IsNullOrEmpty(c.FileName) && !string.IsNullOrEmpty(existingItemsLine))
		{
			string leaf = LogImportEngine.LeafOfLine(existingItemsLine);

			// The address carries the hash the cabpool appended, while the log records the path the
			// file was saved to and so names it without one. They are the same file, so the hash comes
			// off both before they are weighed up. Skipping this offers a correction that never settles
			// and, worse, replaces a hashed address with a name that would not resolve.
			correction.FileName = leaf != null &&
				!string.Equals(LogImportParser.StripHash(leaf),
					LogImportParser.StripHash(c.FileName), StringComparison.OrdinalIgnoreCase);
		}

		return correction;
	}
}
