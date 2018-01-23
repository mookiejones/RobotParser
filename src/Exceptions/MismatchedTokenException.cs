using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using RobotParser.Interfaces;
namespace RobotParser.Exceptions
{
  [Serializable]
public class MismatchedTokenException : RecognitionException
{
	private readonly int _expecting;

	private readonly ReadOnlyCollection<string> _tokenNames;

	public int Expecting
	{
		get
		{
			return this._expecting;
		}
	}

	public ReadOnlyCollection<string> TokenNames
	{
		get
		{
			return this._tokenNames;
		}
	}

	public MismatchedTokenException()
	{
	}

	public MismatchedTokenException(string message)
		: base(message)
	{
	}

	public MismatchedTokenException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public MismatchedTokenException(int expecting, IIntStream input)
		: this(expecting, input, null)
	{
	}

	public MismatchedTokenException(int expecting, IIntStream input, IList<string> tokenNames)
		: base(input)
	{
		this._expecting = expecting;
		if (tokenNames != null)
		{
			this._tokenNames = tokenNames.ToList().AsReadOnly();
		}
	}

	public MismatchedTokenException(string message, int expecting, IIntStream input, IList<string> tokenNames)
		: base(message, input)
	{
		this._expecting = expecting;
		if (tokenNames != null)
		{
			this._tokenNames = tokenNames.ToList().AsReadOnly();
		}
	}

	public MismatchedTokenException(string message, int expecting, IIntStream input, IList<string> tokenNames, Exception innerException)
		: base(message, input, innerException)
	{
		this._expecting = expecting;
		if (tokenNames != null)
		{
			this._tokenNames = tokenNames.ToList().AsReadOnly();
		}
	}

	protected MismatchedTokenException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		this._expecting = info.GetInt32("Expecting");
		this._tokenNames = new ReadOnlyCollection<string>((string[])info.GetValue("TokenNames", typeof(string[])));
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		base.GetObjectData(info, context);
		info.AddValue("Expecting", this._expecting);
		info.AddValue("TokenNames", (this._tokenNames != null) ? this._tokenNames.ToArray() : null);
	}

	public override string ToString()
	{
		int unexpectedType = this.UnexpectedType;
		string text = (this.TokenNames != null && unexpectedType >= 0 && unexpectedType < this.TokenNames.Count) ? this.TokenNames[unexpectedType] : unexpectedType.ToString();
		string text2 = (this.TokenNames != null && this.Expecting >= 0 && this.Expecting < this.TokenNames.Count) ? this.TokenNames[this.Expecting] : this.Expecting.ToString();
		return "MismatchedTokenException(" + text + "!=" + text2 + ")";
	}
}
}