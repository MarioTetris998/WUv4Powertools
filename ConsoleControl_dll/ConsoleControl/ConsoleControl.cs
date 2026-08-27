using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ConsoleControl;

public class ConsoleControl : UserControl
{
	public delegate void LineEnteredDelegate(object sender, string line);

	private const string RenderFontName = "Courier New";

	private const int ScreenSize = 2000;

	private readonly List<char> _keysBuffer;

	private readonly List<string> _commandBuffer;

	private int _commandBufferIndex;

	private bool _isCursorOn;

	private readonly Font _renderFont = new Font("Courier New", 10f, FontStyle.Regular);

	private Color _consoleBackgroundColor;

	private Color _consoleForegroundColor;

	private int _cursorX;

	private int _cursorY;

	private CursorTypes _cursorType;

	private readonly TextBlock[] _textBlockArray;

	private Timer _readLineTimer;

	public bool ShowCursor { get; set; }

	public bool AllowInput { get; set; }

	public bool EchoInput { get; set; }

	public Color CurrentForegroundColor { get; set; }

	public Color CurrentBackgroundColor { get; set; }

	public CursorTypes CursorType
	{
		get
		{
			return _cursorType;
		}
		set
		{
			_cursorType = value;
			Invalidate();
		}
	}

	public Color ConsoleBackgroundColor
	{
		get
		{
			return _consoleBackgroundColor;
		}
		set
		{
			_consoleBackgroundColor = value;
			BackColor = value;
			Invalidate();
		}
	}

	public Color ConsoleForegroundColor
	{
		get
		{
			return _consoleForegroundColor;
		}
		set
		{
			_consoleForegroundColor = value;
			ForeColor = value;
			Invalidate();
		}
	}

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams obj = base.CreateParams;
			obj.ExStyle |= 33554432;
			return obj;
		}
	}

	public event LineEnteredDelegate LineEntered;

	public ConsoleControl()
	{
		base.Width = 646;
		base.Height = 377;
		_cursorX = 0;
		_cursorY = 0;
		_isCursorOn = false;
		CursorType = CursorTypes.Underline;
		ConsoleBackgroundColor = Color.Black;
		ConsoleForegroundColor = Color.LightGray;
		CurrentForegroundColor = Color.LightGray;
		CurrentBackgroundColor = Color.Black;
		ShowCursor = true;
		AllowInput = true;
		EchoInput = true;
		_textBlockArray = new TextBlock[2000];
		for (int i = 0; i < 2000; i++)
		{
			_textBlockArray[i].BackgroundColor = ConsoleBackgroundColor;
			_textBlockArray[i].ForegroundColor = ConsoleForegroundColor;
			_textBlockArray[i].Character = '\0';
		}
		Timer timer = new Timer();
		timer.Enabled = true;
		timer.Interval = 500;
		timer.Tick += CursorFlashTimerTick;
		_keysBuffer = new List<char>();
		_commandBuffer = new List<string>();
		_commandBufferIndex = 0;
		base.KeyPress += ConsoleControlKeyPress;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Up:
		{
			if (_commandBufferIndex <= 0 || !AllowInput)
			{
				return true;
			}
			int len2 = _keysBuffer.Count;
			for (int k = 0; k < len2; k++)
			{
				ConsoleControlKeyPress(this, new KeyPressEventArgs('\b'));
			}
			_keysBuffer.Clear();
			string text = _commandBuffer[_commandBufferIndex - 1];
			foreach (char c2 in text)
			{
				_keysBuffer.Add(c2);
				if (EchoInput)
				{
					Write(c2);
				}
			}
			_commandBufferIndex--;
			return true;
		}
		case Keys.Down:
		{
			if (_commandBufferIndex + 1 >= _commandBuffer.Count || !AllowInput)
			{
				return true;
			}
			int len = _keysBuffer.Count;
			for (int i = 0; i < len; i++)
			{
				ConsoleControlKeyPress(this, new KeyPressEventArgs('\b'));
			}
			_keysBuffer.Clear();
			string text = _commandBuffer[_commandBufferIndex + 1];
			foreach (char c in text)
			{
				_keysBuffer.Add(c);
				if (EchoInput)
				{
					Write(c);
				}
			}
			_commandBufferIndex++;
			return true;
		}
		default:
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}

	private void ConsoleControlKeyPress(object sender, KeyPressEventArgs e)
	{
		if (!AllowInput)
		{
			return;
		}
		if (e.KeyChar == '\b')
		{
			if (_keysBuffer.Count == 0)
			{
				return;
			}
			if (EchoInput)
			{
				_textBlockArray[GetIndex()].Character = '\0';
				_cursorX--;
				if (_cursorX < 0)
				{
					_cursorY--;
					_cursorX = 79;
					if (_cursorY < 0)
					{
						_cursorY++;
						_cursorX = 0;
					}
				}
				_textBlockArray[GetIndex()].Character = '\0';
				Invalidate();
			}
			_keysBuffer.RemoveAt(_keysBuffer.Count - 1);
			return;
		}
		_keysBuffer.Add(e.KeyChar);
		if (EchoInput)
		{
			Write(e.KeyChar);
			if (e.KeyChar == '\r')
			{
				Write('\n');
			}
		}
		if (e.KeyChar == '\r')
		{
			if (Environment.NewLine.Length == 2)
			{
				_keysBuffer.Add('\n');
			}
			string s = _keysBuffer.Aggregate("", (string current, char c) => current + c);
			_keysBuffer.Clear();
			_commandBuffer.Add(s.Trim('\r', '\n'));
			_commandBufferIndex = _commandBuffer.Count;
			if (this.LineEntered != null)
			{
				this.LineEntered(this, s);
			}
		}
		Invalidate();
	}

	private void CursorFlashTimerTick(object sender, EventArgs e)
	{
		if (ShowCursor)
		{
			_isCursorOn = !_isCursorOn;
			char c = CursorType switch
			{
				CursorTypes.Block => '█', 
				CursorTypes.Invisible => ' ', 
				_ => '_', 
			};
			_textBlockArray[GetIndex()].Character = (_isCursorOn ? c : '\0');
			Invalidate();
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		int x = 0;
		int y = 0;
		int charWidth = 8;
		int charHeight = 15;
		using (Bitmap bitmap = new Bitmap(base.Width, base.Height))
		{
			using Graphics g = Graphics.FromImage(bitmap);
			for (int i = 0; i < 2000; i++)
			{
				Color color = ((_textBlockArray[i].Character == '\0') ? _consoleForegroundColor : _textBlockArray[i].ForegroundColor);
				Brush bgBrush = new SolidBrush((_textBlockArray[i].Character == '\0') ? _consoleBackgroundColor : _textBlockArray[i].BackgroundColor);
				Brush fgBrush = new SolidBrush(color);
				g.FillRectangle(bgBrush, new Rectangle(x + 2, y + 1, charWidth, charHeight));
				g.DrawString((_textBlockArray[i].Character == '\0') ? " " : _textBlockArray[i].Character.ToString(), _renderFont, fgBrush, new PointF(x, y));
				x += charWidth;
				if (x > 79 * charWidth)
				{
					y += charHeight;
					x = 0;
				}
			}
			e.Graphics.DrawImage(bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
		}
		base.OnPaint(e);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		base.Width = 646;
		base.Height = 377;
	}

	public void SetCursorPosition(int row, int column)
	{
		if (ShowCursor)
		{
			_textBlockArray[GetIndex()].Character = '\0';
		}
		_cursorX = column;
		_cursorY = row;
		Invalidate();
	}

	public void SetCursorPosition(Location location)
	{
		SetCursorPosition(location.Row, location.Column);
	}

	public Location GetCursorPosition()
	{
		return new Location
		{
			Column = _cursorX,
			Row = _cursorY
		};
	}

	public void Write()
	{
		Write(Environment.NewLine);
	}

	public void Write(char c)
	{
		Write(c, CurrentForegroundColor, CurrentBackgroundColor);
	}

	public void Write(char c, Color fgColor, Color bgColor)
	{
		switch (c)
		{
		case '\a':
			Console.Beep(1000, 500);
			break;
		case '\r':
			SetCursorPosition(GetCursorPosition().Row, 0);
			break;
		case '\n':
			if (Environment.NewLine.Length == 1)
			{
				SetCursorPosition(GetCursorPosition().Row, 0);
			}
			_cursorY++;
			if (_cursorY > 24)
			{
				ScrollUp();
				_cursorY = 24;
			}
			break;
		default:
			_textBlockArray[GetIndex()].Character = c;
			_textBlockArray[GetIndex()].BackgroundColor = bgColor;
			_textBlockArray[GetIndex()].ForegroundColor = fgColor;
			MoveCursorPosition();
			Invalidate();
			break;
		}
	}

	public void Write(string text)
	{
		Write(text, CurrentForegroundColor, CurrentBackgroundColor);
	}

	public void Write(string text, Color fgColor, Color bgColor)
	{
		foreach (char c in text)
		{
			Write(c, fgColor, bgColor);
		}
		Invalidate();
	}

	private void MoveCursorPosition()
	{
		_cursorX++;
		if (_cursorX > 79)
		{
			_cursorX = 0;
			_cursorY++;
		}
		if (_cursorY > 24)
		{
			ScrollUp();
			_cursorY = 24;
		}
	}

	private int GetIndex(int row, int col)
	{
		return 80 * row + col;
	}

	private int GetIndex()
	{
		return GetIndex(_cursorY, _cursorX);
	}

	public void ScrollUp(int lines)
	{
		while (lines > 0)
		{
			for (int i = 0; i < 1920; i++)
			{
				_textBlockArray[i] = _textBlockArray[i + 80];
			}
			for (int j = 1920; j < 2000; j++)
			{
				_textBlockArray[j].Character = '\0';
				_textBlockArray[j].BackgroundColor = ConsoleBackgroundColor;
				_textBlockArray[j].ForegroundColor = ConsoleForegroundColor;
			}
			lines--;
		}
		Invalidate();
	}

	public void ScrollUp()
	{
		ScrollUp(1);
	}

	public void Clear()
	{
		for (int i = 0; i < 2000; i++)
		{
			_textBlockArray[i].BackgroundColor = ConsoleBackgroundColor;
			_textBlockArray[i].ForegroundColor = ConsoleForegroundColor;
			_textBlockArray[i].Character = '\0';
		}
		_cursorX = 0;
		_cursorY = 0;
		Invalidate();
	}

	public void SetBackgroundColorAt(Color color, int row, int column)
	{
		_textBlockArray[GetIndex(row, column)].BackgroundColor = color;
		Invalidate();
	}

	public void SetForegroundColorAt(Color color, int row, int column)
	{
		_textBlockArray[GetIndex(row, column)].ForegroundColor = color;
		Invalidate();
	}

	public void SetCharacterAt(char character, int row, int column)
	{
		_textBlockArray[GetIndex(row, column)].Character = character;
		Invalidate();
	}
}
