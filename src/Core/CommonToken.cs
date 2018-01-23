using System;
using RobotParser.Interfaces;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace RobotParser.Core
{
  [Serializable]
public class CommonToken : IToken
{
	private int type;

	private int line;

	private int charPositionInLine = -1;

	private int channel;

	[NonSerialized]
	private ICharStream input;

	private string text;

	private int index = -1;

	private int start;

	private int stop;

	public string Text
	{
		get
		{
			if (this.text != null)
			{
				return this.text;
			}
			if (this.input == null)
			{
				return null;
			}
			if (this.start < this.input.Count && this.stop < this.input.Count)
			{
				this.text = this.input.Substring(this.start, this.stop - this.start + 1);
			}
			else
			{
				this.text = "<EOF>";
			}
			return this.text;
		}
		set
		{
			this.text = value;
		}
	}

	public int Type
	{
		get
		{
			return this.type;
		}
		set
		{
			this.type = value;
		}
	}

	public int Line
	{
		get
		{
			return this.line;
		}
		set
		{
			this.line = value;
		}
	}

	public int CharPositionInLine
	{
		get
		{
			return this.charPositionInLine;
		}
		set
		{
			this.charPositionInLine = value;
		}
	}

	public int Channel
	{
		get
		{
			return this.channel;
		}
		set
		{
			this.channel = value;
		}
	}

	public int StartIndex
	{
		get
		{
			return this.start;
		}
		set
		{
			this.start = value;
		}
	}

	public int StopIndex
	{
		get
		{
			return this.stop;
		}
		set
		{
			this.stop = value;
		}
	}

	public int TokenIndex
	{
		get
		{
			return this.index;
		}
		set
		{
			this.index = value;
		}
	}

	public ICharStream InputStream
	{
		get
		{
			return this.input;
		}
		set
		{
			this.input = value;
		}
	}

	public CommonToken()
	{
	}

	public CommonToken(int type)
	{
		this.type = type;
	}

	public CommonToken(ICharStream input, int type, int channel, int start, int stop)
	{
		this.input = input;
		this.type = type;
		this.channel = channel;
		this.start = start;
		this.stop = stop;
	}

	public CommonToken(int type, string text)
	{
		this.type = type;
		this.channel = 0;
		this.text = text;
	}

	public CommonToken(IToken oldToken)
	{
		this.text = oldToken.Text;
		this.type = oldToken.Type;
		this.line = oldToken.Line;
		this.index = oldToken.TokenIndex;
		this.charPositionInLine = oldToken.CharPositionInLine;
		this.channel = oldToken.Channel;
		this.input = oldToken.InputStream;
		if (oldToken is CommonToken)
		{
			this.start = ((CommonToken)oldToken).start;
			this.stop = ((CommonToken)oldToken).stop;
		}
	}

	public override string ToString()
	{
		string text = "";
		if (this.channel > 0)
		{
			text = ",channel=" + this.channel;
		}
		string text2 = this.Text;
		if (text2 != null)
		{
			text2 = Regex.Replace(text2, "\n", "\\\\n");
			text2 = Regex.Replace(text2, "\r", "\\\\r");
			text2 = Regex.Replace(text2, "\t", "\\\\t");
		}
		else
		{
			text2 = "<no text>";
		}
		return "[@" + this.TokenIndex + "," + this.start + ":" + this.stop + "='" + text2 + "',<" + this.type + ">" + text + "," + this.line + ":" + this.CharPositionInLine + "]";
	}

	[OnSerializing]
	internal void OnSerializing(StreamingContext context)
	{
		if (this.text == null)
		{
			this.text = this.Text;
		}
	}
}
}