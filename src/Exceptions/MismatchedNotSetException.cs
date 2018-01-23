using System;
using System.Runtime.Serialization;
using RobotParser.Interfaces;
namespace RobotParser.Exceptions
{
    using Core;
 [Serializable]
public class MismatchedNotSetException : MismatchedSetException
{
	public MismatchedNotSetException()
	{
	}

	public MismatchedNotSetException(string message)
		: base(message)
	{
	}

	public MismatchedNotSetException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public MismatchedNotSetException(BitSet expecting, IIntStream input)
		: base(expecting, input)
	{
	}

	public MismatchedNotSetException(string message, BitSet expecting, IIntStream input)
		: base(message, expecting, input)
	{
	}

	public MismatchedNotSetException(string message, BitSet expecting, IIntStream input, Exception innerException)
		: base(message, expecting, input, innerException)
	{
	}

	protected MismatchedNotSetException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override string ToString()
	{
		return "MismatchedNotSetException(" + this.UnexpectedType + "!=" + base.Expecting + ")";
	}
}

}