using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WUv4Powertools;

// One provider inside a consumer dictionary folder, held as the same five arrays the item list uses.
// The importer works on these rather than on open tabs, so it can reach every operating system and
// Internet Explorer version in the folder whether or not any of them happen to be open.
public sealed class ProviderStore
{
	public string Name { get; private set; }

	public string Folder { get; private set; }

	public string[] Items;

	public string[] ItemsIndex;

	public string[] ItemStrings;

	public string[] ItemStringsIndex;

	public string[] Product2Items;

	// Set once anything has been added, so untouched providers are never rewritten.
	public bool Dirty;

	// The open tab showing this provider, when there is one. Its arrays are the ones edited, so
	// unsaved work in a tab is carried into the import rather than being overwritten by what is
	// still on disk.
	public frmItemList OpenTab;

	private ProviderIndex index;

	public ProviderIndex Index
	{
		get
		{
			if (index == null) index = new ProviderIndex(Name, Items, ItemsIndex, ItemStringsIndex, ItemStrings);
			return index;
		}
	}

	// Called after an import so the next question about this provider sees what was just added.
	public void InvalidateIndex()
	{
		index = null;
	}

	private static readonly Encoding Latin1 = Encoding.GetEncoding("ISO-8859-1");

	public static ProviderStore Load(string root, string name)
	{
		string folder = Path.Combine(root, name);
		ProviderStore store = new ProviderStore { Name = name, Folder = folder };
		store.Items = File.ReadAllLines(Path.Combine(folder, "items.txt"), Latin1);
		store.ItemsIndex = File.ReadAllLines(Path.Combine(folder, "itemsindex.txt"), Latin1);
		store.ItemStrings = File.ReadAllLines(Path.Combine(folder, "itemstrings.txt"), Encoding.Unicode);
		store.ItemStringsIndex = File.ReadAllLines(Path.Combine(folder, "itemstringsindex.txt"), Latin1);
		store.Product2Items = File.ReadAllLines(Path.Combine(folder, "product2items.txt"), Latin1);
		return store;
	}

	// Takes the arrays from the open tab instead of the files, so a provider being edited is
	// imported into as the user currently has it.
	public static ProviderStore FromTab(string root, frmItemList tab)
	{
		return new ProviderStore
		{
			Name = tab.provider,
			Folder = Path.Combine(root, tab.provider),
			Items = tab.l_items,
			ItemsIndex = tab.l_itemsindex,
			ItemStrings = tab.l_itemstrings,
			ItemStringsIndex = tab.l_itemstringsindex,
			Product2Items = tab.l_product2items,
			OpenTab = tab
		};
	}

	// An open provider keeps the app's usual promise that nothing reaches disk until the tab is
	// saved, so only closed providers are written here.
	public void Save()
	{
		frmMain.SaveProviderFiles(Folder, Product2Items, ItemsIndex, Items, ItemStringsIndex, ItemStrings);
	}

	// Pushes the imported arrays back into the tab and refreshes what it shows.
	public void PushToTab()
	{
		if (OpenTab == null) return;
		OpenTab.l_items = Items;
		OpenTab.l_itemsindex = ItemsIndex;
		OpenTab.l_itemstrings = ItemStrings;
		OpenTab.l_itemstringsindex = ItemStringsIndex;
		OpenTab.l_product2items = Product2Items;
		OpenTab.ReloadItems();
	}
}

// Every provider in one consumer dictionary folder.
public sealed class ConsumerDictionary
{
	public string Root { get; private set; }

	public readonly List<ProviderStore> Providers = new List<ProviderStore>();

	// Providers listed in providers.txt whose folder is missing or unreadable.
	public readonly List<string> Unavailable = new List<string>();

	private static readonly string[] Required =
	{
		"items.txt", "itemsindex.txt", "itemstrings.txt", "itemstringsindex.txt", "product2items.txt"
	};

	public ProviderStore Find(string name)
	{
		return Providers.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
	}

	// Reads providers.txt for the list of providers, then loads each one that is actually present.
	// A provider already open in a tab is taken from the tab rather than from disk.
	// Whether a provider was read from the folder now being loaded. An empty folder belongs to
	// a tab that did not come from one, and is accepted so nothing that used to work stops.
	private static bool SameFolder(string tabFolder, string root)
	{
		if (string.IsNullOrEmpty(tabFolder)) return true;

		char[] trailing = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
		return string.Equals(tabFolder.TrimEnd(trailing), (root ?? string.Empty).TrimEnd(trailing),
			StringComparison.OrdinalIgnoreCase);
	}

	public static ConsumerDictionary Load(string root, IEnumerable<frmItemList> openTabs)
	{
		ConsumerDictionary dictionary = new ConsumerDictionary { Root = root };

		string providersFile = Path.Combine(root, "providers.txt");
		if (!File.Exists(providersFile)) return dictionary;

		Dictionary<string, frmItemList> tabs = new Dictionary<string, frmItemList>(StringComparer.OrdinalIgnoreCase);
		foreach (frmItemList tab in openTabs ?? Enumerable.Empty<frmItemList>())
		{
			if (tab == null || string.IsNullOrEmpty(tab.provider)) continue;

			// Only a tab read from this same folder. Matching on the name alone would take a
			// provider open from another folder, read its rows as though they belonged here and
			// write them back into the wrong one.
			if (!SameFolder(tab.sourceFolder, root)) continue;

			tabs[tab.provider] = tab;
		}

		foreach (string line in File.ReadAllLines(providersFile))
		{
			if (string.IsNullOrWhiteSpace(line)) continue;
			string name = line.Split(',')[0].Trim();
			if (name.Length == 0) continue;

			string folder = Path.Combine(root, name);
			if (!Directory.Exists(folder) || Required.Any(f => !File.Exists(Path.Combine(folder, f))))
			{
				dictionary.Unavailable.Add(name);
				continue;
			}

			try
			{
				dictionary.Providers.Add(tabs.ContainsKey(name)
					? ProviderStore.FromTab(root, tabs[name])
					: ProviderStore.Load(root, name));
			}
			catch
			{
				dictionary.Unavailable.Add(name);
			}
		}

		return dictionary;
	}
}
