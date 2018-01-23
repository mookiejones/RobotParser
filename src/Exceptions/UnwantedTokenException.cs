using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using RobotParser.Interfaces;
namespace RobotParser.Exceptions
{
  [Serializable]
public class UnwantedTokenException : MismatchedTokenException
{
	public virtual IToken UnexpectedToken
	{
		get
		{
			return base.Token;
		}
	}

	public UnwantedTokenException()
	{
	}

	public UnwantedTokenException(string message)
		: base(message)
	{
	}

	public UnwantedTokenException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public UnwantedTokenException(int expecting, IIntStream input)
		: base(expecting, input)
	{
	}

	public UnwantedTokenException(int expecting, IIntStream input, IList<string> tokenNames)
		: base(expecting, input, tokenNames)
	{
	}

	public UnwantedTokenException(string message, int expecting, IIntStream input, IList<string> tokenNames)
		: base(message, expecting, input, tokenNames)
	{
	}

	public UnwantedTokenException(string message, int expecting, IIntStream input, IList<string> tokenNames, Exception innerException)
		: base(message, expecting, input, tokenNames, innerException)
	{
	}

	protected UnwantedTokenException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override string ToString()
	{
		string str = (base.TokenNames != null && base.Expecting >= 0 && base.Expecting < base.TokenNames.Count) ? base.TokenNames[base.Expecting] : base.Expecting.ToString();
		string text = ", expected " + str;
		if (base.Expecting == 0)
		{
			text = "";
		}
		if (base.Token == null)
		{
			return "UnwantedTokenException(found=" + text + ")";
		}
		return "UnwantedTokenException(found=" + base.Token.Text + text + ")";
	}
}
}