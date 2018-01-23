namespace RobotParser.Exceptions
{
    using RobotParser.Core;
   // Antlr.Runtime.RecognitionException
 using RobotParser.Interfaces;
using System;
using System.Runtime.Serialization;

[Serializable]
public class RecognitionException : Exception
{
	private IIntStream _input;

	private int _index;

	private IToken _token;

	private object _node;

	private int _c;

	private int _line;

	private int _charPositionInLine;

	private bool _approximateLineInfo;

	public virtual int UnexpectedType
	{
		get
		{
			if (this._input is ITokenStream)
			{
				return this._token.Type;
			}
			ITreeNodeStream treeNodeStream = this._input as ITreeNodeStream;
			if (treeNodeStream != null)
			{
				ITreeAdaptor treeAdaptor = treeNodeStream.TreeAdaptor;
				return treeAdaptor.GetType(this._node);
			}
			return this._c;
		}
	}

	public bool ApproximateLineInfo
	{
		get
		{
			return this._approximateLineInfo;
		}
		protected set
		{
			this._approximateLineInfo = value;
		}
	}

	public IIntStream Input
	{
		get
		{
			return this._input;
		}
		protected set
		{
			this._input = value;
		}
	}

	public IToken Token
	{
		get
		{
			return this._token;
		}
		set
		{
			this._token = value;
		}
	}

	public object Node
	{
		get
		{
			return this._node;
		}
		protected set
		{
			this._node = value;
		}
	}

	public int Character
	{
		get
		{
			return this._c;
		}
		protected set
		{
			this._c = value;
		}
	}

	public int Index
	{
		get
		{
			return this._index;
		}
		protected set
		{
			this._index = value;
		}
	}

	public int Line
	{
		get
		{
			return this._line;
		}
		set
		{
			this._line = value;
		}
	}

	public int CharPositionInLine
	{
		get
		{
			return this._charPositionInLine;
		}
		set
		{
			this._charPositionInLine = value;
		}
	}

	public RecognitionException()
		: this("A recognition error occurred.", null, null)
	{
	}

	public RecognitionException(IIntStream input)
		: this("A recognition error occurred.", input, null)
	{
	}

	public RecognitionException(string message)
		: this(message, null, null)
	{
	}

	public RecognitionException(string message, IIntStream input)
		: this(message, input, null)
	{
	}

	public RecognitionException(string message, Exception innerException)
		: this(message, null, innerException)
	{
	}

	public RecognitionException(string message, IIntStream input, Exception innerException)
		: base(message, innerException)
	{
		this._input = input;
		if (input != null)
		{
			this._index = input.Index;
			if (input is ITokenStream)
			{
				this._token = ((ITokenStream)input).LT(1);
				this._line = this._token.Line;
				this._charPositionInLine = this._token.CharPositionInLine;
			}
			if (input is ITreeNodeStream)
			{
				this.ExtractInformationFromTreeNodeStream(input);
			}
			else if (input is ICharStream)
			{
				this._c = input.LA(1);
				this._line = ((ICharStream)input).Line;
				this._charPositionInLine = ((ICharStream)input).CharPositionInLine;
			}
			else
			{
				this._c = input.LA(1);
			}
		}
	}

	protected RecognitionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		this._index = info.GetInt32("Index");
		this._c = info.GetInt32("C");
		this._line = info.GetInt32("Line");
		this._charPositionInLine = info.GetInt32("CharPositionInLine");
		this._approximateLineInfo = info.GetBoolean("ApproximateLineInfo");
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		base.GetObjectData(info, context);
		info.AddValue("Index", this._index);
		info.AddValue("C", this._c);
		info.AddValue("Line", this._line);
		info.AddValue("CharPositionInLine", this._charPositionInLine);
		info.AddValue("ApproximateLineInfo", this._approximateLineInfo);
	}

	protected virtual void ExtractInformationFromTreeNodeStream(IIntStream input)
	{
		ITokenStreamInformation tokenStreamInformation = input as ITokenStreamInformation;
		if (tokenStreamInformation != null)
		{
			IToken lastToken = tokenStreamInformation.LastToken;
			IToken lastRealToken = tokenStreamInformation.LastRealToken;
			if (lastRealToken != null)
			{
				this._token = lastRealToken;
				this._line = lastRealToken.Line;
				this._charPositionInLine = lastRealToken.CharPositionInLine;
				this._approximateLineInfo = lastRealToken.Equals(lastToken);
			}
		}
		else
		{
			ITreeNodeStream treeNodeStream = (ITreeNodeStream)input;
			this._node = treeNodeStream.LT(1);
			ITreeAdaptor treeAdaptor = treeNodeStream.TreeAdaptor;
			IToken token = treeAdaptor.GetToken(this._node);
			if (token != null)
			{
				this._token = token;
				if (token.Line <= 0)
				{
					int num = -1;
					object obj = treeNodeStream.LT(num);
					IToken token2;
					while (true)
					{
						if (obj != null)
						{
							token2 = treeAdaptor.GetToken(obj);
							if (token2 != null && token2.Line > 0)
							{
								break;
							}
							num--;
							obj = treeNodeStream.LT(num);
							continue;
						}
						return;
					}
					this._line = token2.Line;
					this._charPositionInLine = token2.CharPositionInLine;
					this._approximateLineInfo = true;
				}
				else
				{
					this._line = token.Line;
					this._charPositionInLine = token.CharPositionInLine;
				}
			}
			else if (this._node is ITree)
			{
				this._line = ((ITree)this._node).Line;
				this._charPositionInLine = ((ITree)this._node).CharPositionInLine;
				if (this._node is CommonTree)
				{
					this._token = ((CommonTree)this._node).Token;
				}
			}
			else
			{
				int type = treeAdaptor.GetType(this._node);
				string text = treeAdaptor.GetText(this._node);
				this._token = new CommonToken(type, text);
			}
		}
	}
}

}