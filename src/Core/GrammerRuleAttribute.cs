namespace RobotParser.Core
{
  using System;
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class GrammarRuleAttribute : Attribute
{
	private readonly string _name;

	public string Name
	{
		get
		{
			return this._name;
		}
	}

	public GrammarRuleAttribute(string name)
	{
		this._name = name;
	}
}
}