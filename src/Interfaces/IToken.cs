namespace RobotParser.Interfaces
{
  public interface IToken
{
	string Text
	{
		get;
		set;
	}

	int Type
	{
		get;
		set;
	}

	int Line
	{
		get;
		set;
	}

	int CharPositionInLine
	{
		get;
		set;
	}

	int Channel
	{
		get;
		set;
	}

	int StartIndex
	{
		get;
		set;
	}

	int StopIndex
	{
		get;
		set;
	}

	int TokenIndex
	{
		get;
		set;
	}

	ICharStream InputStream
	{
		get;
		set;
	}
}

}