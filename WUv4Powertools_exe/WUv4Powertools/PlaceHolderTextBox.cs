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
			base.Text = value;
		}
	}

	private void setPlaceholder()
	{
		if (string.IsNullOrEmpty(base.Text))
		{
			base.Text = PlaceHolderText;
			ForeColor = Color.Gray;
			Font = new Font(Font, FontStyle.Italic);
			isPlaceHolder = true;
		}
	}

	private void removePlaceHolder()
	{
		if (isPlaceHolder)
		{
			base.Text = "";
			ForeColor = SystemColors.WindowText;
			Font = new Font(Font, FontStyle.Regular);
			isPlaceHolder = false;
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
