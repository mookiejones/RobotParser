using System;
using System.Runtime.Serialization;
using RobotParser.Interfaces;
namespace RobotParser.Exceptions
{
 
[Serializable]
public class FailedPredicateException : RecognitionException
{
	private readonly string _ruleName;

	private readonly string _predicateText;

	public string RuleName
	{
		get
		{
			return this._ruleName;
		}
	}

	public string PredicateText
	{
		get
		{
			return this._predicateText;
		}
	}

	public FailedPredicateException()
	{
	}

	public FailedPredicateException(string message)
		: base(message)
	{
	}

	public FailedPredicateException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public FailedPredicateException(IIntStream input, string ruleName, string predicateText)
		: base(input)
	{
		this._ruleName = ruleName;
		this._predicateText = predicateText;
	}

	public FailedPredicateException(string message, IIntStream input, string ruleName, string predicateText)
		: base(message, input)
	{
		this._ruleName = ruleName;
		this._predicateText = predicateText;
	}

	public FailedPredicateException(string message, IIntStream input, string ruleName, string predicateText, Exception innerException)
		: base(message, input, innerException)
	{
		this._ruleName = ruleName;
		this._predicateText = predicateText;
	}

	protected FailedPredicateException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		this._ruleName = info.GetString("RuleName");
		this._predicateText = info.GetString("PredicateText");
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		base.GetObjectData(info, context);
		info.AddValue("RuleName", this._ruleName);
		info.AddValue("PredicateText", this._predicateText);
	}

	public override string ToString()
	{
		return "FailedPredicateException(" + this.RuleName + ",{" + this.PredicateText + "}?)";
	}
}

}