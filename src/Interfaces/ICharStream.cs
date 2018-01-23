namespace RobotParser.Interfaces
{
  
public interface ICharStream : IIntStream
{
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

	string Substring(int start, int length);

	int LT(int i);
}

}