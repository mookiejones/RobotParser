using System;
using System.Collections.Generic;
using RobotParser.Interfaces;
namespace RobotParser.Core
{
   [Serializable]
public class CoreStringStream : ICharStream, IIntStream
{
	protected char[] data;

	protected int n;

	protected int p;

	private int line = 1;

	private int charPositionInLine;

	protected int markDepth;

	protected IList<CharStreamState> markers;

	protected int lastMarker;

	public string name;

	public virtual int Index
	{
		get
		{
			return this.p;
		}
	}

	public virtual int Line
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

	public virtual int CharPositionInLine
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

	public virtual int Count
	{
		get
		{
			return this.n;
		}
	}

	public virtual string SourceName
	{
		get
		{
			return this.name;
		}
	}

	public CoreStringStream(string input)
		: this(input, null)
	{
	}

	public CoreStringStream(string input, string sourceName)
		: this(input.ToCharArray(), input.Length, sourceName)
	{
	}

	public CoreStringStream(char[] data, int numberOfActualCharsInArray)
		: this(data, numberOfActualCharsInArray, null)
	{
	}

	public CoreStringStream(char[] data, int numberOfActualCharsInArray, string sourceName)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (numberOfActualCharsInArray < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (numberOfActualCharsInArray > data.Length)
		{
			throw new ArgumentException();
		}
		this.data = data;
		this.n = numberOfActualCharsInArray;
		this.name = sourceName;
	}

	protected CoreStringStream()
	{
		this.data = new char[0];
	}

	public virtual void Reset()
	{
		this.p = 0;
		this.line = 1;
		this.charPositionInLine = 0;
		this.markDepth = 0;
	}

	public virtual void Consume()
	{
		if (this.p < this.n)
		{
			this.charPositionInLine++;
			if (this.data[this.p] == '\n')
			{
				this.line++;
				this.charPositionInLine = 0;
			}
			this.p++;
		}
	}

	public virtual int LA(int i)
	{
		if (i == 0)
		{
			return 0;
		}
		if (i < 0)
		{
			i++;
			if (this.p + i - 1 < 0)
			{
				return -1;
			}
		}
		if (this.p + i - 1 >= this.n)
		{
			return -1;
		}
		return this.data[this.p + i - 1];
	}

	public virtual int LT(int i)
	{
		return this.LA(i);
	}

	public virtual int Mark()
	{
		if (this.markers == null)
		{
			this.markers = new List<CharStreamState>();
			this.markers.Add(null);
		}
		this.markDepth++;
		CharStreamState charStreamState = null;
		if (this.markDepth >= this.markers.Count)
		{
			charStreamState = new CharStreamState();
			this.markers.Add(charStreamState);
		}
		else
		{
			charStreamState = this.markers[this.markDepth];
		}
		charStreamState.p = this.p;
		charStreamState.line = this.line;
		charStreamState.charPositionInLine = this.charPositionInLine;
		this.lastMarker = this.markDepth;
		return this.markDepth;
	}

	public virtual void Rewind(int m)
	{
		if (m < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		CharStreamState charStreamState = this.markers[m];
		this.Seek(charStreamState.p);
		this.line = charStreamState.line;
		this.charPositionInLine = charStreamState.charPositionInLine;
		this.Release(m);
	}

	public virtual void Rewind()
	{
		this.Rewind(this.lastMarker);
	}

	public virtual void Release(int marker)
	{
		this.markDepth = marker;
		this.markDepth--;
	}

	public virtual void Seek(int index)
	{
		if (index <= this.p)
		{
			this.p = index;
		}
		else
		{
			while (this.p < index)
			{
				this.Consume();
			}
		}
	}

	public virtual string Substring(int start, int length)
	{
		if (start < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (length < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (start + length > this.data.Length)
		{
			throw new ArgumentException();
		}
		if (length == 0)
		{
			return string.Empty;
		}
		return new string(this.data, start, length);
	}

	public override string ToString()
	{
		return new string(this.data);
	}
}

}