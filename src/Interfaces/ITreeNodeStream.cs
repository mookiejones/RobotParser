namespace RobotParser.Interfaces
{
  
public interface ITreeNodeStream : IIntStream
{
	object this[int i]
	{
		get;
	}

	object TreeSource
	{
		get;
	}

	ITokenStream TokenStream
	{
		get;
	}

	ITreeAdaptor TreeAdaptor
	{
		get;
	}

	bool UniqueNavigationNodes
	{
		get;
		set;
	}

	object LT(int k);

	string ToString(object start, object stop);

	void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t);
}

}