using System;
using System.Drawing;
using System.Windows.Forms;

namespace WUv4Powertools;

public class PlaceHolderTextBox : ToolStripTextBox
{
	private bool isPlaceHolder = true;

	private string _placeHolderText;

	public string PlaceHolderText
	{
		get
		{
			return _placeHolderText;
		}
		set
		{
			_placeHolderText = value;
			setPlaceholder();
		}
	}

	public new string Text
	{
		get
		{
			if (!isPlaceHolder)
			{
				return base.Text;
			}
			return string.Empty;
		}
		set
		{
			// Clearing the box from code has to bring the placeholder back, otherwise it is left
			// simply blank. The control used to manage the placeholder on focus and blur alone, so
			// anything that assigned an empty string wiped it until the box was clicked into twice.
			if (string.IsNullOrEmpty(value) && !Focused)
			{
				isPlaceHolder = false;
				base.Text = string.Empty;
				setPlaceholder();
				return;
			}
			removePlaceHolder();
			base.Text = value;
		}
	}

	private void setPlaceholder()
	{
		if (string.IsNullOrEmpty(base.Text))
		{
			// Raise the flag first. Assigning the text fires TextChanged, and anything reading Text
			// from that handler would otherwise be handed the placeholder itself rather than an
			// empty string, which makes the search filter hide every row.
			isPlaceHolder = true;
			base.Text = PlaceHolderText;
			ForeColor = Color.Gray;
			Font = new Font(Font, FontStyle.Italic);
		}
	}

	private void removePlaceHolder()
	{
		if (isPlaceHolder)
		{
			// Same ordering concern in reverse: clear the flag before the text changes, so a reader
			// during TextChanged sees the real value.
			isPlaceHolder = false;
			base.Text = string.Empty;
			ForeColor = SystemColors.WindowText;
			Font = new Font(Font, FontStyle.Regular);
		}
	}

	public PlaceHolderTextBox()
	{
		base.GotFocus += removePlaceHolder;
		base.LostFocus += setPlaceholder;
	}

	private void setPlaceholder(object sender, EventArgs e)
	{
		setPlaceholder();
	}

	private void removePlaceHolder(object sender, EventArgs e)
	{
		removePlaceHolder();
	}
}
