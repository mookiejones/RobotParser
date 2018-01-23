using System;
using RobotParser.Interfaces;
using System.Collections.Generic;
using System.Text;
namespace RobotParser.Core
{
   [Serializable]
public abstract class BaseTree : ITree
{
	private List<ITree> children;

	public virtual IList<ITree> Children
	{
		get
		{
			return this.children;
		}
	}

	public virtual int ChildCount
	{
		get
		{
			if (this.Children == null)
			{
				return 0;
			}
			return this.Children.Count;
		}
	}

	public virtual ITree Parent
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public virtual int ChildIndex
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public virtual bool IsNil
	{
		get
		{
			return false;
		}
	}

	public abstract int TokenStartIndex
	{
		get;
		set;
	}

	public abstract int TokenStopIndex
	{
		get;
		set;
	}

	public abstract int Type
	{
		get;
		set;
	}

	public abstract string Text
	{
		get;
		set;
	}

	public virtual int Line
	{
		get;
		set;
	}

	public virtual int CharPositionInLine
	{
		get;
		set;
	}

	public BaseTree()
	{
	}

	public BaseTree(ITree node)
	{
	}

	public virtual ITree GetChild(int i)
	{
		if (i < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (this.children != null && i < this.children.Count)
		{
			return this.children[i];
		}
		return null;
	}

	public virtual ITree GetFirstChildWithType(int type)
	{
		foreach (ITree child in this.children)
		{
			if (child.Type == type)
			{
				return child;
			}
		}
		return null;
	}

	public virtual void AddChild(ITree t)
	{
		if (t != null)
		{
			if (t.IsNil)
			{
				BaseTree baseTree = t as BaseTree;
				if (baseTree != null && this.children != null && this.children == baseTree.children)
				{
					throw new Exception("attempt to add child list to itself");
				}
				if (t.ChildCount > 0)
				{
					if (this.children != null || baseTree == null)
					{
						if (this.children == null)
						{
							this.children = this.CreateChildrenList();
						}
						int childCount = t.ChildCount;
						for (int i = 0; i < childCount; i++)
						{
							ITree child = t.GetChild(i);
							this.children.Add(child);
							child.Parent = this;
							child.ChildIndex = this.children.Count - 1;
						}
					}
					else
					{
						this.children = baseTree.children;
						this.FreshenParentAndChildIndexes();
					}
				}
			}
			else
			{
				if (this.children == null)
				{
					this.children = this.CreateChildrenList();
				}
				this.children.Add(t);
				t.Parent = this;
				t.ChildIndex = this.children.Count - 1;
			}
		}
	}

	public virtual void AddChildren(IEnumerable<ITree> kids)
	{
		if (kids == null)
		{
			throw new ArgumentNullException("kids");
		}
		foreach (ITree kid in kids)
		{
			this.AddChild(kid);
		}
	}

	public virtual void SetChild(int i, ITree t)
	{
		if (i < 0)
		{
			throw new ArgumentOutOfRangeException("i");
		}
		if (t != null)
		{
			if (t.IsNil)
			{
				throw new ArgumentException("Can't set single child to a list");
			}
			if (this.children == null)
			{
				this.children = this.CreateChildrenList();
			}
			this.children[i] = t;
			t.Parent = this;
			t.ChildIndex = i;
		}
	}

	public virtual object DeleteChild(int i)
	{
		if (i < 0)
		{
			throw new ArgumentOutOfRangeException("i");
		}
		if (i >= this.ChildCount)
		{
			throw new ArgumentException();
		}
		if (this.children == null)
		{
			return null;
		}
		ITree result = this.children[i];
		this.children.RemoveAt(i);
		this.FreshenParentAndChildIndexes(i);
		return result;
	}

	public virtual void ReplaceChildren(int startChildIndex, int stopChildIndex, object t)
	{
		if (startChildIndex < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (stopChildIndex < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (t == null)
		{
			throw new ArgumentNullException("t");
		}
		if (stopChildIndex < startChildIndex)
		{
			throw new ArgumentException();
		}
		if (this.children == null)
		{
			throw new ArgumentException("indexes invalid; no children in list");
		}
		int num = stopChildIndex - startChildIndex + 1;
		ITree tree = (ITree)t;
		List<ITree> list = null;
		if (tree.IsNil)
		{
			BaseTree baseTree = tree as BaseTree;
			if (baseTree != null && baseTree.children != null)
			{
				list = baseTree.children;
			}
			else
			{
				list = this.CreateChildrenList();
				int childCount = tree.ChildCount;
				for (int i = 0; i < childCount; i++)
				{
					list.Add(tree.GetChild(i));
				}
			}
		}
		else
		{
			list = new List<ITree>(1);
			list.Add(tree);
		}
		int count = list.Count;
		int count2 = list.Count;
		int num2 = num - count;
		if (num2 == 0)
		{
			int num3 = 0;
			for (int j = startChildIndex; j <= stopChildIndex; j++)
			{
				ITree tree2 = list[num3];
				this.children[j] = tree2;
				tree2.Parent = this;
				tree2.ChildIndex = j;
				num3++;
			}
		}
		else if (num2 > 0)
		{
			for (int k = 0; k < count2; k++)
			{
				this.children[startChildIndex + k] = list[k];
			}
			int num4 = startChildIndex + count2;
			for (int l = num4; l <= stopChildIndex; l++)
			{
				this.children.RemoveAt(num4);
			}
			this.FreshenParentAndChildIndexes(startChildIndex);
		}
		else
		{
			for (int m = 0; m < num; m++)
			{
				this.children[startChildIndex + m] = list[m];
			}
			for (int n = num; n < count; n++)
			{
				this.children.Insert(startChildIndex + n, list[n]);
			}
			this.FreshenParentAndChildIndexes(startChildIndex);
		}
	}

	protected virtual List<ITree> CreateChildrenList()
	{
		return new List<ITree>();
	}

	public virtual void FreshenParentAndChildIndexes()
	{
		this.FreshenParentAndChildIndexes(0);
	}

	public virtual void FreshenParentAndChildIndexes(int offset)
	{
		int childCount = this.ChildCount;
		for (int i = offset; i < childCount; i++)
		{
			ITree child = this.GetChild(i);
			child.ChildIndex = i;
			child.Parent = this;
		}
	}

	public virtual void SanityCheckParentAndChildIndexes()
	{
		this.SanityCheckParentAndChildIndexes(null, -1);
	}

	public virtual void SanityCheckParentAndChildIndexes(ITree parent, int i)
	{
		if (parent != this.Parent)
		{
			throw new InvalidOperationException("parents don't match; expected " + parent + " found " + this.Parent);
		}
		if (i != this.ChildIndex)
		{
			throw new InvalidOperationException("child indexes don't match; expected " + i + " found " + this.ChildIndex);
		}
		int childCount = this.ChildCount;
		for (int j = 0; j < childCount; j++)
		{
			BaseTree baseTree = (BaseTree)this.GetChild(j);
			baseTree.SanityCheckParentAndChildIndexes(this, j);
		}
	}

	public virtual bool HasAncestor(int ttype)
	{
		return this.GetAncestor(ttype) != null;
	}

	public virtual ITree GetAncestor(int ttype)
	{
		for (ITree parent = ((ITree)this).Parent; parent != null; parent = parent.Parent)
		{
			if (parent.Type == ttype)
			{
				return parent;
			}
		}
		return null;
	}

	public virtual IList<ITree> GetAncestors()
	{
		if (this.Parent == null)
		{
			return null;
		}
		List<ITree> list = new List<ITree>();
		for (ITree parent = ((ITree)this).Parent; parent != null; parent = parent.Parent)
		{
			list.Insert(0, parent);
		}
		return list;
	}

	public virtual string ToStringTree()
	{
		if (this.children != null && this.children.Count != 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!this.IsNil)
			{
				stringBuilder.Append("(");
				stringBuilder.Append(this.ToString());
				stringBuilder.Append(' ');
			}
			int num = 0;
			while (this.children != null && num < this.children.Count)
			{
				ITree tree = this.children[num];
				if (num > 0)
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append(tree.ToStringTree());
				num++;
			}
			if (!this.IsNil)
			{
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}
		return this.ToString();
	}

	public abstract override string ToString();

	public abstract ITree DupNode();
}
}