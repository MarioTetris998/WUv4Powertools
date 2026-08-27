using System;
using System.Collections;
using AdvancedWizardControl.EventArguments;

namespace AdvancedWizardControl.WizardPages;

[Serializable]
public class AdvancedWizardPageCollection : IList, ICollection, IEnumerable
{
	private readonly ArrayList _items;

	public AdvancedWizardPage this[int index]
	{
		get
		{
			return (AdvancedWizardPage)_items[index];
		}
		set
		{
			_items[index] = value;
		}
	}

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			if (value is AdvancedWizardPage)
			{
				this[index] = (AdvancedWizardPage)value;
			}
		}
	}

	internal ArrayList Items => _items;

	public int Count => _items.Count;

	public bool IsSynchronized => _items.IsSynchronized;

	public object SyncRoot => _items.SyncRoot;

	public bool IsFixedSize => _items.IsFixedSize;

	public bool IsReadOnly => _items.IsReadOnly;

	public event EventHandler<WizardPageEventArgs> OnPageAdded;

	public AdvancedWizardPageCollection()
		: this(5)
	{
	}

	public AdvancedWizardPageCollection(int initialCount)
	{
		_items = new ArrayList(initialCount);
	}

	bool IList.Contains(object wizardPage)
	{
		return _items.Contains(wizardPage);
	}

	public bool Contains(AdvancedWizardPage page)
	{
		return _items.Contains(page);
	}

	public void CopyTo(Array array, int index)
	{
		if (array.Length - index >= Count)
		{
			for (int i = 0; i < Count; i++)
			{
				array.SetValue(_items[i], index + i);
			}
			return;
		}
		throw new ArgumentException("The Array to Copy To must have enough elements to copy all the items from this collection.", "Array");
	}

	public void CopyTo(AdvancedWizardPageCollection pages)
	{
		pages.Items.AddRange(Items);
	}

	int IList.Add(object @object)
	{
		if (@object is AdvancedWizardPage)
		{
			return Add((AdvancedWizardPage)@object);
		}
		return -1;
	}

	public int Add(AdvancedWizardPage page)
	{
		if (this.OnPageAdded != null)
		{
			this.OnPageAdded(this, new WizardPageEventArgs(page));
		}
		return _items.Add(page);
	}

	public void Clear()
	{
		_items.Clear();
	}

	int IList.IndexOf(object @object)
	{
		return _items.IndexOf(@object);
	}

	public int IndexOf(AdvancedWizardPage page)
	{
		return _items.IndexOf(page);
	}

	void IList.Insert(int index, object @object)
	{
		if (@object is AdvancedWizardPage)
		{
			_items.Insert(index, @object);
			return;
		}
		throw new ArgumentException("This collection can only contain WizardPage objects.", "object");
	}

	public void Insert(int index, AdvancedWizardPage page)
	{
		_items.Insert(index, page);
	}

	void IList.Remove(object @object)
	{
		if (@object is AdvancedWizardPage)
		{
			_items.Remove(@object);
		}
	}

	public void Remove(AdvancedWizardPage page)
	{
		_items.Remove(page);
	}

	public void RemoveAt(int index)
	{
		_items.RemoveAt(index);
	}

	public IEnumerator GetEnumerator()
	{
		return _items.GetEnumerator();
	}

	public IEnumerator GetEnumerator(int index, int count)
	{
		return _items.GetEnumerator(index, count);
	}
}
