using System;
using RobotParser.Interfaces;
namespace RobotParser.Core
{
 [Serializable]
public class CommonTree : BaseTree
{
	[CLSCompliant(false)]
	public IToken token;

	protected int startIndex = -1;

	protected int stopIndex = -1;

	private CommonTree parent;

	private int childIndex = -1;

	public override int CharPositionInLine
	{
		get
		{
			if (this.token != null && this.token.CharPositionInLine != -1)
			{
				return this.token.CharPositionInLine;
			}
			if (this.ChildCount > 0)
			{
				return this.Children[0].CharPositionInLine;
			}
			return 0;
		}
		set
		{
			base.CharPositionInLine = value;
		}
	}

	public override int ChildIndex
	{
		get
		{
			return this.childIndex;
		}
		set
		{
			this.childIndex = value;
		}
	}

	public override bool IsNil
	{
		get
		{
			return this.token == null;
		}
	}

	public override int Line
	{
		get
		{
			if (this.token != null && this.token.Line != 0)
			{
				return this.token.Line;
			}
			if (this.ChildCount > 0)
			{
				return this.Children[0].Line;
			}
			return 0;
		}
		set
		{
			base.Line = value;
		}
	}

	public override ITree Parent
	{
		get
		{
			return this.parent;
		}
		set
		{
			this.parent = (CommonTree)value;
		}
	}

	public override string Text
	{
		get
		{
			if (this.token == null)
			{
				return null;
			}
			return this.token.Text;
		}
		set
		{
		}
	}

	public virtual IToken Token
	{
		get
		{
			return this.token;
		}
		set
		{
			this.token = value;
		}
	}

	public override int TokenStartIndex
	{
		get
		{
			if (this.startIndex == -1 && this.token != null)
			{
				return this.token.TokenIndex;
			}
			return this.startIndex;
		}
		set
		{
			this.startIndex = value;
		}
	}

	public override int TokenStopIndex
	{
		get
		{
			if (this.stopIndex == -1 && this.token != null)
			{
				return this.token.TokenIndex;
			}
			return this.stopIndex;
		}
		set
		{
			this.stopIndex = value;
		}
	}

	public override int Type
	{
		get
		{
			if (this.token == null)
			{
				return 0;
			}
			return this.token.Type;
		}
		set
		{
		}
	}

	public CommonTree()
	{
	}

	public CommonTree(CommonTree node)
		: base(node)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		this.token = node.token;
		this.startIndex = node.startIndex;
		this.stopIndex = node.stopIndex;
	}

	public CommonTree(IToken t)
	{
		this.token = t;
	}

	public override ITree DupNode()
	{
		return new CommonTree(this);
	}

	public virtual void SetUnknownTokenBoundaries()
	{
		if (this.Children == null)
		{
			if (this.startIndex >= 0 && this.stopIndex >= 0)
			{
				return;
			}
			this.startIndex = (this.stopIndex = this.token.TokenIndex);
		}
		else
		{
			for (int i = 0; i < this.Children.Count; i++)
			{
				((CommonTree)this.Children[i]).SetUnknownTokenBoundaries();
			}
			if (this.startIndex >= 0 && this.stopIndex >= 0)
			{
				return;
			}
			if (this.Children.Count > 0)
			{
				CommonTree commonTree = (CommonTree)this.Children[0];
				CommonTree commonTree2 = (CommonTree)this.Children[this.Children.Count - 1];
				this.startIndex = commonTree.TokenStartIndex;
				this.stopIndex = commonTree2.TokenStopIndex;
			}
		}
	}

	public override string ToString()
	{
		if (this.IsNil)
		{
			return "nil";
		}
		if (this.Type == 0)
		{
			return "<errornode>";
		}
		if (this.token == null)
		{
			return string.Empty;
		}
		return this.token.Text;
	}
}

}