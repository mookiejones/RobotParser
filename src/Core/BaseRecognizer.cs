using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using RobotParser.Interfaces;
using RobotParser.Exceptions;
namespace RobotParser.Core{
public abstract class BaseRecognizer
{
	public const int MemoRuleFailed = -2;

	public const int MemoRuleUnknown = -1;

	public const int InitialFollowStackSize = 100;

	public const int DefaultTokenChannel = 0;

	public const int Hidden = 99;

	public const string NextTokenRuleName = "nextToken";

	protected internal RecognizerSharedState state;

	public TextWriter TraceDestination
	{
		get;
		set;
	}

	public virtual int NumberOfSyntaxErrors
	{
		get
		{
			return this.state.syntaxErrors;
		}
	}

	public virtual int BacktrackingLevel
	{
		get
		{
			return this.state.backtracking;
		}
		set
		{
			this.state.backtracking = value;
		}
	}

	public virtual bool Failed
	{
		get
		{
			return this.state.failed;
		}
	}

	public virtual string[] TokenNames
	{
		get
		{
			return null;
		}
	}

	public virtual string GrammarFileName
	{
		get
		{
			return null;
		}
	}

	public abstract string SourceName
	{
		get;
	}

	public virtual IDebugEventListener DebugListener
	{
		get
		{
			return null;
		}
	}

	public BaseRecognizer()
		: this(new RecognizerSharedState())
	{
	}

	public BaseRecognizer(RecognizerSharedState state)
	{
		if (state == null)
		{
			state = new RecognizerSharedState();
		}
		this.state = state;
		this.InitDFAs();
	}

	protected virtual void InitDFAs()
	{
	}

	public virtual void Reset()
	{
		if (this.state != null)
		{
			this.state._fsp = -1;
			this.state.errorRecovery = false;
			this.state.lastErrorIndex = -1;
			this.state.failed = false;
			this.state.syntaxErrors = 0;
			this.state.backtracking = 0;
			int num = 0;
			while (this.state.ruleMemo != null && num < this.state.ruleMemo.Length)
			{
				this.state.ruleMemo[num] = null;
				num++;
			}
		}
	}

	public virtual object Match(IIntStream input, int ttype, BitSet follow)
	{
		object currentInputSymbol = this.GetCurrentInputSymbol(input);
		if (input.LA(1) == ttype)
		{
			input.Consume();
			this.state.errorRecovery = false;
			this.state.failed = false;
			return currentInputSymbol;
		}
		if (this.state.backtracking > 0)
		{
			this.state.failed = true;
			return currentInputSymbol;
		}
		return this.RecoverFromMismatchedToken(input, ttype, follow);
	}

	public virtual void MatchAny(IIntStream input)
	{
		this.state.errorRecovery = false;
		this.state.failed = false;
		input.Consume();
	}

	public virtual bool MismatchIsUnwantedToken(IIntStream input, int ttype)
	{
		return input.LA(2) == ttype;
	}

	public virtual bool MismatchIsMissingToken(IIntStream input, BitSet follow)
	{
		if (follow == null)
		{
			return false;
		}
		if (follow.Member(1))
		{
			BitSet a = this.ComputeContextSensitiveRuleFOLLOW();
			follow = follow.Or(a);
			if (this.state._fsp >= 0)
			{
				follow.Remove(1);
			}
		}
		if (!follow.Member(input.LA(1)) && !follow.Member(1))
		{
			return false;
		}
		return true;
	}

	public virtual void ReportError(RecognitionException e)
	{
		if (!this.state.errorRecovery)
		{
			this.state.syntaxErrors++;
			this.state.errorRecovery = true;
			this.DisplayRecognitionError(this.TokenNames, e);
		}
	}

	public virtual void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
	{
		string errorHeader = this.GetErrorHeader(e);
		string errorMessage = this.GetErrorMessage(e, tokenNames);
		this.EmitErrorMessage(errorHeader + " " + errorMessage);
	}

	public virtual string GetErrorMessage(RecognitionException e, string[] tokenNames)
	{
		string result = e.Message;
		if (e is UnwantedTokenException)
		{
			UnwantedTokenException ex = (UnwantedTokenException)e;
			string text = "<unknown>";
			text = ((ex.Expecting != -1) ? tokenNames[ex.Expecting] : "EndOfFile");
			result = "extraneous input " + this.GetTokenErrorDisplay(ex.UnexpectedToken) + " expecting " + text;
		}
		else if (e is MissingTokenException)
		{
			MissingTokenException ex2 = (MissingTokenException)e;
			string text2 = "<unknown>";
			text2 = ((ex2.Expecting != -1) ? tokenNames[ex2.Expecting] : "EndOfFile");
			result = "missing " + text2 + " at " + this.GetTokenErrorDisplay(e.Token);
		}
		else if (e is MismatchedTokenException)
		{
			MismatchedTokenException ex3 = (MismatchedTokenException)e;
			string text3 = "<unknown>";
			text3 = ((ex3.Expecting != -1) ? tokenNames[ex3.Expecting] : "EndOfFile");
			result = "mismatched input " + this.GetTokenErrorDisplay(e.Token) + " expecting " + text3;
		}
		else if (e is MismatchedTreeNodeException)
		{
			MismatchedTreeNodeException ex4 = (MismatchedTreeNodeException)e;
			string text4 = "<unknown>";
			text4 = ((ex4.Expecting != -1) ? tokenNames[ex4.Expecting] : "EndOfFile");
			string str = (ex4.Node != null) ? (ex4.Node.ToString() ?? string.Empty) : string.Empty;
			result = "mismatched tree node: " + str + " expecting " + text4;
		}
		else if (e is NoViableAltException)
		{
			result = "no viable alternative at input " + this.GetTokenErrorDisplay(e.Token);
		}
		else if (e is EarlyExitException)
		{
			result = "required (...)+ loop did not match anything at input " + this.GetTokenErrorDisplay(e.Token);
		}
		else if (e is MismatchedSetException)
		{
			MismatchedSetException ex5 = (MismatchedSetException)e;
			result = "mismatched input " + this.GetTokenErrorDisplay(e.Token) + " expecting set " + ex5.Expecting;
		}
		else if (e is MismatchedNotSetException)
		{
			MismatchedNotSetException ex6 = (MismatchedNotSetException)e;
			result = "mismatched input " + this.GetTokenErrorDisplay(e.Token) + " expecting set " + ex6.Expecting;
		}
		else if (e is FailedPredicateException)
		{
			FailedPredicateException ex7 = (FailedPredicateException)e;
			result = "rule " + ex7.RuleName + " failed predicate: {" + ex7.PredicateText + "}?";
		}
		return result;
	}

	public virtual string GetErrorHeader(RecognitionException e)
	{
		string text = this.SourceName ?? string.Empty;
		if (text.Length > 0)
		{
			text += ' ';
		}
		return string.Format("{0}line {1}:{2}", text, e.Line, e.CharPositionInLine + 1);
	}

	public virtual string GetTokenErrorDisplay(IToken t)
	{
		string text = t.Text;
		if (text == null)
		{
			text = ((t.Type != -1) ? ("<" + t.Type + ">") : "<EOF>");
		}
		text = Regex.Replace(text, "\n", "\\\\n");
		text = Regex.Replace(text, "\r", "\\\\r");
		text = Regex.Replace(text, "\t", "\\\\t");
		return "'" + text + "'";
	}

	public virtual void EmitErrorMessage(string msg)
	{
		if (this.TraceDestination != null)
		{
			this.TraceDestination.WriteLine(msg);
		}
	}

	public virtual void Recover(IIntStream input, RecognitionException re)
	{
		if (this.state.lastErrorIndex == input.Index)
		{
			input.Consume();
		}
		this.state.lastErrorIndex = input.Index;
		BitSet set = this.ComputeErrorRecoverySet();
		this.BeginResync();
		this.ConsumeUntil(input, set);
		this.EndResync();
	}

	public virtual void BeginResync()
	{
	}

	public virtual void EndResync()
	{
	}

	protected virtual BitSet ComputeErrorRecoverySet()
	{
		return this.CombineFollows(false);
	}

	protected virtual BitSet ComputeContextSensitiveRuleFOLLOW()
	{
		return this.CombineFollows(true);
	}

	protected virtual BitSet CombineFollows(bool exact)
	{
		int fsp = this.state._fsp;
		BitSet bitSet = new BitSet();
		for (int num = fsp; num >= 0; num--)
		{
			BitSet bitSet2 = this.state.following[num];
			bitSet.OrInPlace(bitSet2);
			if (exact)
			{
				if (!bitSet2.Member(1))
				{
					break;
				}
				if (num > 0)
				{
					bitSet.Remove(1);
				}
			}
		}
		return bitSet;
	}

	protected virtual object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
	{
		RecognitionException e = null;
		if (this.MismatchIsUnwantedToken(input, ttype))
		{
			e = new UnwantedTokenException(ttype, input, this.TokenNames);
			this.BeginResync();
			input.Consume();
			this.EndResync();
			this.ReportError(e);
			object currentInputSymbol = this.GetCurrentInputSymbol(input);
			input.Consume();
			return currentInputSymbol;
		}
		if (this.MismatchIsMissingToken(input, follow))
		{
			object missingSymbol = this.GetMissingSymbol(input, e, ttype, follow);
			e = new MissingTokenException(ttype, input, missingSymbol);
			this.ReportError(e);
			return missingSymbol;
		}
		e = new MismatchedTokenException(ttype, input, this.TokenNames);
		throw e;
	}

	public virtual object RecoverFromMismatchedSet(IIntStream input, RecognitionException e, BitSet follow)
	{
		if (this.MismatchIsMissingToken(input, follow))
		{
			this.ReportError(e);
			return this.GetMissingSymbol(input, e, 0, follow);
		}
		throw e;
	}

	protected virtual object GetCurrentInputSymbol(IIntStream input)
	{
		return null;
	}

	protected virtual object GetMissingSymbol(IIntStream input, RecognitionException e, int expectedTokenType, BitSet follow)
	{
		return null;
	}

	public virtual void ConsumeUntil(IIntStream input, int tokenType)
	{
		int num = input.LA(1);
		while (num != -1 && num != tokenType)
		{
			input.Consume();
			num = input.LA(1);
		}
	}

	public virtual void ConsumeUntil(IIntStream input, BitSet set)
	{
		int num = input.LA(1);
		while (num != -1 && !set.Member(num))
		{
			input.Consume();
			num = input.LA(1);
		}
	}

	protected void PushFollow(BitSet fset)
	{
		if (this.state._fsp + 1 >= this.state.following.Length)
		{
			Array.Resize<BitSet>(ref this.state.following, this.state.following.Length * 2);
		}
		this.state.following[++this.state._fsp] = fset;
	}

	protected void PopFollow()
	{
		this.state._fsp--;
	}

	public virtual IList<string> GetRuleInvocationStack()
	{
		return BaseRecognizer.GetRuleInvocationStack(new StackTrace(true));
	}

	public static IList<string> GetRuleInvocationStack(StackTrace trace)
	{
		if (trace == null)
		{
			throw new ArgumentNullException("trace");
		}
		List<string> list = new List<string>();
		StackFrame[] array = trace.GetFrames() ?? new StackFrame[0];
		for (int num = array.Length - 1; num >= 0; num--)
		{
			StackFrame stackFrame = array[num];
			MethodBase method = stackFrame.GetMethod();
			GrammarRuleAttribute[] array2 = (GrammarRuleAttribute[])method.GetCustomAttributes(typeof(GrammarRuleAttribute), true);
			if (array2 != null && array2.Length > 0)
			{
				list.Add(array2[0].Name);
			}
		}
		return list;
	}

	public virtual List<string> ToStrings(ICollection<IToken> tokens)
	{
		if (tokens == null)
		{
			return null;
		}
		List<string> list = new List<string>(tokens.Count);
		foreach (IToken token in tokens)
		{
			list.Add(token.Text);
		}
		return list;
	}

	public virtual int GetRuleMemoization(int ruleIndex, int ruleStartIndex)
	{
		if (this.state.ruleMemo[ruleIndex] == null)
		{
			this.state.ruleMemo[ruleIndex] = new Dictionary<int, int>();
		}
		int result = default(int);
		if (!this.state.ruleMemo[ruleIndex].TryGetValue(ruleStartIndex, out result))
		{
			return -1;
		}
		return result;
	}

	public virtual bool AlreadyParsedRule(IIntStream input, int ruleIndex)
	{
		int ruleMemoization = this.GetRuleMemoization(ruleIndex, input.Index);
		switch (ruleMemoization)
		{
		case -1:
			return false;
		case -2:
			this.state.failed = true;
			break;
		default:
			input.Seek(ruleMemoization + 1);
			break;
		}
		return true;
	}

	public virtual void Memoize(IIntStream input, int ruleIndex, int ruleStartIndex)
	{
		int value = this.state.failed ? (-2) : (input.Index - 1);
		if (this.state.ruleMemo == null && this.TraceDestination != null)
		{
			this.TraceDestination.WriteLine("!!!!!!!!! memo array is null for " + this.GrammarFileName);
		}
		if (ruleIndex >= this.state.ruleMemo.Length && this.TraceDestination != null)
		{
			this.TraceDestination.WriteLine("!!!!!!!!! memo size is " + this.state.ruleMemo.Length + ", but rule index is " + ruleIndex);
		}
		if (this.state.ruleMemo[ruleIndex] != null)
		{
			this.state.ruleMemo[ruleIndex][ruleStartIndex] = value;
		}
	}

	public virtual int GetRuleMemoizationCacheSize()
	{
		int num = 0;
		int num2 = 0;
		while (this.state.ruleMemo != null && num2 < this.state.ruleMemo.Length)
		{
			IDictionary<int, int> dictionary = this.state.ruleMemo[num2];
			if (dictionary != null)
			{
				num += dictionary.Count;
			}
			num2++;
		}
		return num;
	}

	[Conditional("ANTLR_TRACE")]
	public virtual void TraceIn(string ruleName, int ruleIndex, object inputSymbol)
	{
		if (this.TraceDestination != null)
		{
			this.TraceDestination.Write("enter " + ruleName + " " + inputSymbol);
			if (this.state.backtracking > 0)
			{
				this.TraceDestination.Write(" backtracking=" + this.state.backtracking);
			}
			this.TraceDestination.WriteLine();
		}
	}

	[Conditional("ANTLR_TRACE")]
	public virtual void TraceOut(string ruleName, int ruleIndex, object inputSymbol)
	{
		if (this.TraceDestination != null)
		{
			this.TraceDestination.Write("exit " + ruleName + " " + inputSymbol);
			if (this.state.backtracking > 0)
			{
				this.TraceDestination.Write(" backtracking=" + this.state.backtracking);
				if (this.state.failed)
				{
					this.TraceDestination.Write(" failed");
				}
				else
				{
					this.TraceDestination.Write(" succeeded");
				}
			}
			this.TraceDestination.WriteLine();
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugEnterRule(string grammarFileName, string ruleName)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.EnterRule(grammarFileName, ruleName);
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugExitRule(string grammarFileName, string ruleName)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.ExitRule(grammarFileName, ruleName);
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugEnterSubRule(int decisionNumber)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.EnterSubRule(decisionNumber);
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugExitSubRule(int decisionNumber)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.ExitSubRule(decisionNumber);
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugEnterAlt(int alt)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.EnterAlt(alt);
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugEnterDecision(int decisionNumber, bool couldBacktrack)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.EnterDecision(decisionNumber, couldBacktrack);
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugExitDecision(int decisionNumber)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.ExitDecision(decisionNumber);
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugLocation(int line, int charPositionInLine)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.Location(line, charPositionInLine);
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugSemanticPredicate(bool result, string predicate)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.SemanticPredicate(result, predicate);
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugBeginBacktrack(int level)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.BeginBacktrack(level);
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugEndBacktrack(int level, bool successful)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.EndBacktrack(level, successful);
		}
	}

	[Conditional("ANTLR_DEBUG")]
	protected virtual void DebugRecognitionException(RecognitionException ex)
	{
		IDebugEventListener debugListener = this.DebugListener;
		if (debugListener != null)
		{
			debugListener.RecognitionException(ex);
		}
	}
}


}