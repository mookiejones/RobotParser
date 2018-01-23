using System;
using System.Collections.Generic;
using RobotParser.Interfaces;
namespace RobotParser.Core
{
    public class RecognizerSharedState
    {
        public BitSet[] following;

	[CLSCompliant(false)]
	public int _fsp;

	public bool errorRecovery;

	public int lastErrorIndex;

	public bool failed;

	public int syntaxErrors;

	public int backtracking;

	public IDictionary<int, int>[] ruleMemo;

	public IToken token;

	public int tokenStartCharIndex;

	public int tokenStartLine;

	public int tokenStartCharPositionInLine;

	public int channel;

	public int type;

	public string text;

	public RecognizerSharedState()
	{
		this.following = new BitSet[100];
		this._fsp = -1;
		this.lastErrorIndex = -1;
		this.tokenStartCharIndex = -1;
	}

	public RecognizerSharedState(RecognizerSharedState state)
	{
		if (state == null)
		{
			throw new ArgumentNullException("state");
		}
		this.following = (BitSet[])state.following.Clone();
		this._fsp = state._fsp;
		this.errorRecovery = state.errorRecovery;
		this.lastErrorIndex = state.lastErrorIndex;
		this.failed = state.failed;
		this.syntaxErrors = state.syntaxErrors;
		this.backtracking = state.backtracking;
		if (state.ruleMemo != null)
		{
			this.ruleMemo = (IDictionary<int, int>[])state.ruleMemo.Clone();
		}
		this.token = state.token;
		this.tokenStartCharIndex = state.tokenStartCharIndex;
		this.tokenStartCharPositionInLine = state.tokenStartCharPositionInLine;
		this.channel = state.channel;
		this.type = state.type;
		this.text = state.text;
	}
    }
}