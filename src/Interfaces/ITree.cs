namespace RobotParser.Interfaces
{
    using System.Collections.Generic;
  
public interface ITree
{
	int ChildCount
	{
		get;
	}

	ITree Parent
	{
		get;
		set;
	}

	int ChildIndex
	{
		get;
		set;
	}

	bool IsNil
	{
		get;
	}

	int TokenStartIndex
	{
		get;
		set;
	}

	int TokenStopIndex
	{
		get;
		set;
	}

	int Type
	{
		get;
	}

	string Text
	{
		get;
	}

	int Line
	{
		get;
	}

	int CharPositionInLine
	{
		get;
	}

	ITree GetChild(int i);

	bool HasAncestor(int ttype);

	ITree GetAncestor(int ttype);

	IList<ITree> GetAncestors();

	void FreshenParentAndChildIndexes();

	void AddChild(ITree t);

	void SetChild(int i, ITree t);

	object DeleteChild(int i);

	void ReplaceChildren(int startChildIndex, int stopChildIndex, object t);

	ITree DupNode();

	string ToStringTree();

	new string ToString();
}
}