namespace WUv4Powertools;

public class Update
{
	public string[] itemlines;

	public int[] itemindexes;

	public LangTitleDesc[] lan;

	public int group;

	public bool critical;

	public bool exclusive;

	public int size;

	public string timesitamp;

	public string code;

	public string langscode;

	// New property to track custom order within group
	public int customOrder = -1;

	// Whether one file serves every language this update reaches, which is held as a single
	// row covering them all rather than a row for each language.
	public bool sharesOneFile;

	// Property to identify if this is a driver update
	public bool isDriver = false;

	public LangTitleDesc getLang(string l)
	{
		LangTitleDesc[] array = lan;
		foreach (LangTitleDesc lang in array)
		{
			if (lang.lang == l)
			{
				return lang;
			}
		}
		return null;
	}
}
