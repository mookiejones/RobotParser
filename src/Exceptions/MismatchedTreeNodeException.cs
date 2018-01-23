using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using RobotParser.Interfaces;
namespace RobotParser.Exceptions
{
 [Serializable]
public class MismatchedTreeNodeException : RecognitionException
{
	private readonly int _expecting;

	public int Expecting
	{
		get
		{
			return this._expecting;
		}
	}

	public MismatchedTreeNodeException()
	{
	}

	public MismatchedTreeNodeException(string message)
		: base(message)
	{
	}

	public MismatchedTreeNodeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public MismatchedTreeNodeException(int expecting, ITreeNodeStream input)
		: base(input)
	{
		this._expecting = expecting;
	}

	public MismatchedTreeNodeException(string message, int expecting, ITreeNodeStream input)
		: base(message, input)
	{
		this._expecting = expecting;
	}

	public MismatchedTreeNodeException(string message, int expecting, ITreeNodeStream input, Exception innerException)
		: base(message, input, innerException)
	{
		this._expecting = expecting;
	}

	protected MismatchedTreeNodeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		this._expecting = info.GetInt32("Expecting");
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		base.GetObjectData(info, context);
		info.AddValue("Expecting", this._expecting);
	}

	public override string ToString()
	{
		return "MismatchedTreeNodeException(" + this.UnexpectedType + "!=" + this.Expecting + ")";
	}
}
}