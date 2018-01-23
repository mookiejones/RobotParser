namespace RobotParser.Interfaces
{
    public interface ITokenStreamInformation
{
	IToken LastToken
	{
		get;
	}

	IToken LastRealToken
	{
		get;
	}

	int MaxLookBehind
	{
		get;
	}
}

}