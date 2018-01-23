using System;
using System.Runtime.Serialization;
namespace RobotParser.Exceptions
{
    using Interfaces;
    using Core;
  [Serializable]
public class NoViableAltException : RecognitionException
{
	private readonly string _grammarDecisionDescription;

	private readonly int _decisionNumber;

	private readonly int _stateNumber;

	public int DecisionNumber
	{
		get
		{
			return this._decisionNumber;
		}
	}

	public string GrammarDecisionDescription
	{
		get
		{
			return this._grammarDecisionDescription;
		}
	}

	public int StateNumber
	{
		get
		{
			return this._stateNumber;
		}
	}

	public NoViableAltException()
	{
	}

	public NoViableAltException(string grammarDecisionDescription)
	{
		this._grammarDecisionDescription = grammarDecisionDescription;
	}

	public NoViableAltException(string message, string grammarDecisionDescription)
		: base(message)
	{
		this._grammarDecisionDescription = grammarDecisionDescription;
	}

	public NoViableAltException(string message, string grammarDecisionDescription, Exception innerException)
		: base(message, innerException)
	{
		this._grammarDecisionDescription = grammarDecisionDescription;
	}

	public NoViableAltException(string grammarDecisionDescription, int decisionNumber, int stateNumber, IIntStream input)
		: base(input)
	{
		this._grammarDecisionDescription = grammarDecisionDescription;
		this._decisionNumber = decisionNumber;
		this._stateNumber = stateNumber;
	}

	public NoViableAltException(string message, string grammarDecisionDescription, int decisionNumber, int stateNumber, IIntStream input)
		: base(message, input)
	{
		this._grammarDecisionDescription = grammarDecisionDescription;
		this._decisionNumber = decisionNumber;
		this._stateNumber = stateNumber;
	}

	public NoViableAltException(string message, string grammarDecisionDescription, int decisionNumber, int stateNumber, IIntStream input, Exception innerException)
		: base(message, input, innerException)
	{
		this._grammarDecisionDescription = grammarDecisionDescription;
		this._decisionNumber = decisionNumber;
		this._stateNumber = stateNumber;
	}

	protected NoViableAltException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		this._grammarDecisionDescription = info.GetString("GrammarDecisionDescription");
		this._decisionNumber = info.GetInt32("DecisionNumber");
		this._stateNumber = info.GetInt32("StateNumber");
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		base.GetObjectData(info, context);
		info.AddValue("GrammarDecisionDescription", this._grammarDecisionDescription);
		info.AddValue("DecisionNumber", this._decisionNumber);
		info.AddValue("StateNumber", this._stateNumber);
	}

	public override string ToString()
	{
		if (base.Input is ICharStream)
		{
			return "NoViableAltException('" + (char)this.UnexpectedType + "'@[" + this.GrammarDecisionDescription + "])";
		}
		return "NoViableAltException(" + this.UnexpectedType + "@[" + this.GrammarDecisionDescription + "])";
	}
}

}