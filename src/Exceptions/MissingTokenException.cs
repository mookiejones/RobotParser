namespace RobotParser.Exceptions
{
// Antlr.Runtime.MissingTokenException
 
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Interfaces;
[Serializable]
public class MissingTokenException : MismatchedTokenException
{
	private readonly object _inserted;

	public virtual int MissingType
	{
		get
		{
			return base.Expecting;
		}
	}

	public MissingTokenException()
	{
	}

	public MissingTokenException(string message)
		: base(message)
	{
	}

	public MissingTokenException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public MissingTokenException(int expecting, IIntStream input, object inserted)
		: this(expecting, input, inserted, null)
	{
	}

	public MissingTokenException(int expecting, IIntStream input, object inserted, IList<string> tokenNames)
		: base(expecting, input, tokenNames)
	{
		this._inserted = inserted;
	}

	public MissingTokenException(string message, int expecting, IIntStream input, object inserted, IList<string> tokenNames)
		: base(message, expecting, input, tokenNames)
	{
		this._inserted = inserted;
	}

	public MissingTokenException(string message, int expecting, IIntStream input, object inserted, IList<string> tokenNames, Exception innerException)
		: base(message, expecting, input, tokenNames, innerException)
	{
		this._inserted = inserted;
	}

	protected MissingTokenException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override string ToString()
	{
		if (this._inserted != null && base.Token != null)
		{
			return "MissingTokenException(inserted " + this._inserted + " at " + base.Token.Text + ")";
		}
		if (base.Token != null)
		{
			return "MissingTokenException(at " + base.Token.Text + ")";
		}
		return "MissingTokenException";
	}
}

}