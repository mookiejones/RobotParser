namespace RobotParser.Interfaces
{
  public interface ITokenSource
{
	string SourceName
	{
		get;
	}

	string[] TokenNames
	{
		get;
	}

	IToken NextToken();
}

}