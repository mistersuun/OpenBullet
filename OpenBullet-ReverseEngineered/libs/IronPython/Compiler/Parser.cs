// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Parser
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

#nullable disable
namespace IronPython.Compiler;

public class Parser : IDisposable
{
  private readonly Tokenizer _tokenizer;
  private ErrorSink _errors;
  private ParserSink _sink;
  private SourceUnit _sourceUnit;
  private ModuleOptions _languageFeatures;
  private TokenWithSpan _token;
  private TokenWithSpan _lookahead;
  private Stack<FunctionDefinition> _functions;
  private bool _fromFutureAllowed;
  private string _privatePrefix;
  private bool _parsingStarted;
  private bool _allowIncomplete;
  private bool _inLoop;
  private bool _inFinally;
  private bool _inFinallyLoop;
  private bool _isGenerator;
  private bool _returnWithValue;
  private SourceCodeReader _sourceReader;
  private int _errorCode;
  private readonly CompilerContext _context;
  private PythonAst _globalParent;
  private static readonly char[] newLineChar = new char[1]
  {
    '\n'
  };
  private static readonly char[] whiteSpace = new char[2]
  {
    ' ',
    '\t'
  };

  private Parser(
    CompilerContext context,
    Tokenizer tokenizer,
    ErrorSink errorSink,
    ParserSink parserSink,
    ModuleOptions languageFeatures)
  {
    ContractUtils.RequiresNotNull((object) tokenizer, nameof (tokenizer));
    ContractUtils.RequiresNotNull((object) errorSink, nameof (errorSink));
    ContractUtils.RequiresNotNull((object) parserSink, nameof (parserSink));
    tokenizer.ErrorSink = (ErrorSink) new Parser.TokenizerErrorSink(this);
    this._tokenizer = tokenizer;
    this._errors = errorSink;
    if (parserSink != ParserSink.Null)
      this._sink = parserSink;
    this._context = context;
    this.Reset(tokenizer.SourceUnit, languageFeatures);
  }

  public static Parser CreateParser(CompilerContext context, PythonOptions options)
  {
    return Parser.CreateParserWorker(context, options, false);
  }

  [Obsolete("pass verbatim via PythonCompilerOptions in PythonOptions")]
  public static Parser CreateParser(CompilerContext context, PythonOptions options, bool verbatim)
  {
    return Parser.CreateParserWorker(context, options, verbatim);
  }

  private static Parser CreateParserWorker(
    CompilerContext context,
    PythonOptions options,
    bool verbatim)
  {
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    ContractUtils.RequiresNotNull((object) options, nameof (options));
    PythonCompilerOptions options1 = context.Options as PythonCompilerOptions;
    if (options == null)
      throw new ValueErrorException(Resources.PythonContextRequired);
    SourceCodeReader reader;
    try
    {
      reader = context.SourceUnit.GetReader();
      if (options1.SkipFirstLine)
        reader.ReadLine();
    }
    catch (IOException ex)
    {
      context.Errors.Add(context.SourceUnit, ex.Message, SourceSpan.Invalid, 0, Severity.Error);
      throw;
    }
    Tokenizer tokenizer = new Tokenizer(context.Errors, options1, verbatim);
    tokenizer.Initialize((object) null, (TextReader) reader, context.SourceUnit, SourceLocation.MinValue);
    tokenizer.IndentationInconsistencySeverity = options.IndentationInconsistencySeverity;
    return new Parser(context, tokenizer, context.Errors, context.ParserSink, options1.Module)
    {
      _sourceReader = reader
    };
  }

  public PythonAst ParseFile(bool makeModule) => this.ParseFile(makeModule, false);

  public PythonAst ParseFile(bool makeModule, bool returnValue)
  {
    try
    {
      return this.ParseFileWorker(makeModule, returnValue);
    }
    catch (BadSourceException ex)
    {
      throw this.BadSourceError(ex);
    }
  }

  public PythonAst ParseInteractiveCode(out ScriptCodeParseResult properties)
  {
    bool isEmptyStmt = false;
    properties = ScriptCodeParseResult.Complete;
    this._globalParent = new PythonAst(false, this._languageFeatures, true, this._context);
    this.StartParsing();
    bool parsingMultiLineCmpdStmt;
    Statement interactiveInput = this.InternalParseInteractiveInput(out parsingMultiLineCmpdStmt, out isEmptyStmt);
    if (this._errorCode == 0)
    {
      if (isEmptyStmt)
        properties = ScriptCodeParseResult.Empty;
      else if (parsingMultiLineCmpdStmt)
        properties = ScriptCodeParseResult.IncompleteStatement;
      return isEmptyStmt ? (PythonAst) null : this.FinishParsing(interactiveInput);
    }
    if ((this._errorCode & 15) != 0)
    {
      if ((this._errorCode & 2) != 0)
      {
        properties = ScriptCodeParseResult.IncompleteToken;
        return (PythonAst) null;
      }
      if ((this._errorCode & 1) != 0)
      {
        properties = !parsingMultiLineCmpdStmt ? ScriptCodeParseResult.IncompleteToken : ScriptCodeParseResult.IncompleteStatement;
        return (PythonAst) null;
      }
    }
    properties = ScriptCodeParseResult.Invalid;
    return (PythonAst) null;
  }

  private PythonAst FinishParsing(Statement ret)
  {
    PythonAst globalParent = this._globalParent;
    this._globalParent = (PythonAst) null;
    int[] lineLocations = this._tokenizer.GetLineLocations();
    if (this._sourceUnit.HasLineMapping)
    {
      List<int> intList = new List<int>();
      int num = 0;
      for (int index = 0; index < lineLocations.Length; ++index)
      {
        while (intList.Count < index)
          intList.Add(num);
        num = lineLocations[index] + 1;
        intList.Add(lineLocations[index]);
      }
      lineLocations = intList.ToArray();
    }
    globalParent.ParsingFinished(lineLocations, ret, this._languageFeatures);
    return globalParent;
  }

  public PythonAst ParseSingleStatement()
  {
    try
    {
      this._globalParent = new PythonAst(false, this._languageFeatures, true, this._context);
      this.StartParsing();
      this.MaybeEatNewLine();
      Statement stmt = this.ParseStmt();
      this.EatEndOfInput();
      return this.FinishParsing(stmt);
    }
    catch (BadSourceException ex)
    {
      throw this.BadSourceError(ex);
    }
  }

  public PythonAst ParseTopExpression()
  {
    try
    {
      this._globalParent = new PythonAst(false, this._languageFeatures, false, this._context);
      ReturnStatement ret = new ReturnStatement(this.ParseTestListAsExpression());
      ret.SetLoc(this._globalParent, 0, 0);
      return this.FinishParsing((Statement) ret);
    }
    catch (BadSourceException ex)
    {
      throw this.BadSourceError(ex);
    }
  }

  public static int GetNextAutoIndentSize(string text, int autoIndentTabWidth)
  {
    ContractUtils.RequiresNotNull((object) text, nameof (text));
    string[] strArray = text.Split(Parser.newLineChar);
    if (strArray.Length <= 1)
      return 0;
    string str = strArray[strArray.Length - 2];
    int index = 0;
    while (index < str.Length && str[index] == ' ')
      ++index;
    int nextAutoIndentSize = index;
    if (str.TrimEnd(Parser.whiteSpace).EndsWith(":"))
      nextAutoIndentSize += autoIndentTabWidth;
    return nextAutoIndentSize;
  }

  public ErrorSink ErrorSink
  {
    get => this._errors;
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      this._errors = value;
    }
  }

  public ParserSink ParserSink
  {
    get => this._sink;
    set
    {
      if (this._sink == ParserSink.Null)
        this._sink = (ParserSink) null;
      else
        this._sink = value;
    }
  }

  public int ErrorCode => this._errorCode;

  public void Reset(SourceUnit sourceUnit, ModuleOptions languageFeatures)
  {
    ContractUtils.RequiresNotNull((object) sourceUnit, nameof (sourceUnit));
    this._sourceUnit = sourceUnit;
    this._languageFeatures = languageFeatures;
    this._token = new TokenWithSpan();
    this._lookahead = new TokenWithSpan();
    this._fromFutureAllowed = true;
    this._functions = (Stack<FunctionDefinition>) null;
    this._privatePrefix = (string) null;
    this._parsingStarted = false;
    this._errorCode = 0;
  }

  public void Reset() => this.Reset(this._sourceUnit, this._languageFeatures);

  private void ReportSyntaxError(TokenWithSpan t) => this.ReportSyntaxError(t, 16 /*0x10*/);

  private void ReportSyntaxError(TokenWithSpan t, int errorCode)
  {
    this.ReportSyntaxError(t.Token, t.Span, errorCode, true);
  }

  private void ReportSyntaxError(Token t, IndexSpan span, int errorCode, bool allowIncomplete)
  {
    int start = span.Start;
    int end = span.End;
    if (allowIncomplete && (t.Kind == TokenKind.EndOfFile || this._tokenizer.IsEndOfFile && (t.Kind == TokenKind.Dedent || t.Kind == TokenKind.NLToken)))
      errorCode |= 1;
    string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Parser.GetErrorMessage(t, errorCode), (object) t.Image);
    this.ReportSyntaxError(start, end, message, errorCode);
  }

  private static string GetErrorMessage(Token t, int errorCode)
  {
    return (errorCode & -16) != 32 /*0x20*/ ? (t.Kind == TokenKind.EndOfFile ? "unexpected EOF while parsing" : Resources.UnexpectedToken) : Resources.ExpectedIndentation;
  }

  private void ReportSyntaxError(string message)
  {
    IndexSpan span = this._lookahead.Span;
    int start = span.Start;
    span = this._lookahead.Span;
    int end = span.End;
    string message1 = message;
    this.ReportSyntaxError(start, end, message1);
  }

  internal void ReportSyntaxError(int start, int end, string message)
  {
    this.ReportSyntaxError(start, end, message, 16 /*0x10*/);
  }

  internal void ReportSyntaxError(int start, int end, string message, int errorCode)
  {
    if (this._errorCode == 0)
      this._errorCode = errorCode;
    this._errors.Add(this._sourceUnit, message, new SourceSpan(this._tokenizer.IndexToLocation(start), this._tokenizer.IndexToLocation(end)), errorCode, Severity.FatalError);
  }

  private static bool IsPrivateName(string name) => name.StartsWith("__") && !name.EndsWith("__");

  private string FixName(string name)
  {
    if (this._privatePrefix != null && Parser.IsPrivateName(name))
      name = $"_{this._privatePrefix}{name}";
    return name;
  }

  private string ReadNameMaybeNone()
  {
    Token token = this.PeekToken();
    if (token == Tokens.NoneToken)
    {
      this.NextToken();
      return "None";
    }
    if (!(token is NameToken nameToken))
    {
      this.ReportSyntaxError("syntax error");
      return (string) null;
    }
    this.NextToken();
    return this.FixName(nameToken.Name);
  }

  private string ReadName()
  {
    if (!(this.PeekToken() is NameToken nameToken))
    {
      this.ReportSyntaxError(this._lookahead);
      return (string) null;
    }
    this.NextToken();
    return this.FixName(nameToken.Name);
  }

  private Statement ParseStmt()
  {
    switch (this.PeekToken().Kind)
    {
      case TokenKind.At:
        return this.ParseDecorated();
      case TokenKind.KeywordClass:
        return (Statement) this.ParseClassDef();
      case TokenKind.KeywordDef:
        return (Statement) this.ParseFuncDef();
      case TokenKind.KeywordFor:
        return (Statement) this.ParseForStmt();
      case TokenKind.KeywordIf:
        return (Statement) this.ParseIfStmt();
      case TokenKind.KeywordTry:
        return this.ParseTryStatement();
      case TokenKind.KeywordWhile:
        return (Statement) this.ParseWhileStmt();
      case TokenKind.LastKeyword:
        return (Statement) this.ParseWithStmt();
      default:
        return this.ParseSimpleStmt();
    }
  }

  private Statement ParseSimpleStmt()
  {
    Statement smallStmt = this.ParseSmallStmt();
    if (this.MaybeEat(TokenKind.Semicolon))
    {
      int startIndex = smallStmt.StartIndex;
      List<Statement> statementList = new List<Statement>();
      statementList.Add(smallStmt);
      while (!this.MaybeEatNewLine() && !this.MaybeEat(TokenKind.EndOfFile))
      {
        statementList.Add(this.ParseSmallStmt());
        if (!this.MaybeEat(TokenKind.EndOfFile))
        {
          if (!this.MaybeEat(TokenKind.Semicolon))
          {
            this.EatNewLine();
            break;
          }
        }
        else
          break;
      }
      Statement[] array = statementList.ToArray();
      SuiteStatement simpleStmt = new SuiteStatement(array);
      simpleStmt.SetLoc(this._globalParent, startIndex, array[array.Length - 1].EndIndex);
      return (Statement) simpleStmt;
    }
    if (!this.MaybeEat(TokenKind.EndOfFile) && !this.EatNewLine())
      this.NextToken();
    return smallStmt;
  }

  private Statement ParseSmallStmt()
  {
    switch (this.PeekToken().Kind)
    {
      case TokenKind.KeywordAssert:
        return (Statement) this.ParseAssertStmt();
      case TokenKind.KeywordBreak:
        if (!this._inLoop)
          this.ReportSyntaxError("'break' outside loop");
        return this.FinishSmallStmt((Statement) new BreakStatement());
      case TokenKind.KeywordContinue:
        if (!this._inLoop)
          this.ReportSyntaxError("'continue' not properly in loop");
        else if (this._inFinally && !this._inFinallyLoop)
          this.ReportSyntaxError("'continue' not supported inside 'finally' clause");
        return this.FinishSmallStmt((Statement) new ContinueStatement());
      case TokenKind.KeywordDel:
        return this.ParseDelStmt();
      case TokenKind.KeywordExec:
        return (Statement) this.ParseExecStmt();
      case TokenKind.KeywordFrom:
        return (Statement) this.ParseFromImportStmt();
      case TokenKind.KeywordGlobal:
        return (Statement) this.ParseGlobalStmt();
      case TokenKind.KeywordImport:
        return (Statement) this.ParseImportStmt();
      case TokenKind.KeywordPass:
        return this.FinishSmallStmt((Statement) new EmptyStatement());
      case TokenKind.KeywordPrint:
        return (Statement) this.ParsePrintStmt();
      case TokenKind.KeywordRaise:
        return (Statement) this.ParseRaiseStmt();
      case TokenKind.KeywordReturn:
        return this.ParseReturnStmt();
      case TokenKind.KeywordYield:
        return this.ParseYieldStmt();
      default:
        return this.ParseExprStmt();
    }
  }

  private Statement ParseDelStmt()
  {
    this.NextToken();
    int start = this.GetStart();
    List<Expression> exprList = this.ParseExprList();
    foreach (Expression expression in exprList)
    {
      string message = expression.CheckDelete();
      if (message != null)
        this.ReportSyntaxError(expression.StartIndex, expression.EndIndex, message, 16 /*0x10*/);
    }
    DelStatement delStmt = new DelStatement(exprList.ToArray());
    delStmt.SetLoc(this._globalParent, start, this.GetEnd());
    return (Statement) delStmt;
  }

  private Statement ParseReturnStmt()
  {
    if (this.CurrentFunction == null)
      this.ReportSyntaxError(Resources.MisplacedReturn);
    this.NextToken();
    Expression expression = (Expression) null;
    int start = this.GetStart();
    if (!Parser.NeverTestToken(this.PeekToken()))
      expression = this.ParseTestListAsExpr();
    if (expression != null)
    {
      this._returnWithValue = true;
      if (this._isGenerator)
        this.ReportSyntaxError("'return' with argument inside generator");
    }
    ReturnStatement returnStmt = new ReturnStatement(expression);
    returnStmt.SetLoc(this._globalParent, start, this.GetEnd());
    return (Statement) returnStmt;
  }

  private Statement FinishSmallStmt(Statement stmt)
  {
    this.NextToken();
    stmt.SetLoc(this._globalParent, this.GetStart(), this.GetEnd());
    return stmt;
  }

  private Statement ParseYieldStmt()
  {
    if (this.CurrentFunction == null)
      this.ReportSyntaxError(Resources.MisplacedYield);
    this._isGenerator = true;
    if (this._returnWithValue)
      this.ReportSyntaxError("'return' with argument inside generator");
    this.Eat(TokenKind.KeywordYield);
    Expression yieldExpression = this.ParseYieldExpression();
    ExpressionStatement yieldStmt = new ExpressionStatement(yieldExpression);
    yieldStmt.SetLoc(this._globalParent, yieldExpression.IndexSpan);
    return (Statement) yieldStmt;
  }

  private Expression ParseYieldExpression()
  {
    FunctionDefinition currentFunction = this.CurrentFunction;
    if (currentFunction != null)
      currentFunction.IsGenerator = true;
    int start = this.GetStart();
    bool trailingComma;
    List<Expression> expressionList = this.ParseExpressionList(out trailingComma);
    Expression expression;
    if (expressionList.Count == 0)
    {
      expression = (Expression) new ConstantExpression((object) null);
      expression.SetLoc(this._globalParent, start, this.GetEnd());
    }
    else
      expression = expressionList.Count == 1 ? expressionList[0] : this.MakeTupleOrExpr(expressionList, trailingComma);
    YieldExpression yieldExpression = new YieldExpression(expression);
    yieldExpression.SetLoc(this._globalParent, start, this.GetEnd());
    return (Expression) yieldExpression;
  }

  private Statement FinishAssignments(Expression right)
  {
    List<Expression> expressionList = (List<Expression>) null;
    Expression expression = (Expression) null;
    while (this.MaybeEat(TokenKind.Assign))
    {
      string message = right.CheckAssign();
      if (message != null)
        this.ReportSyntaxError(right.StartIndex, right.EndIndex, message, 80 /*0x50*/);
      if (expression == null)
      {
        expression = right;
      }
      else
      {
        if (expressionList == null)
        {
          expressionList = new List<Expression>();
          expressionList.Add(expression);
        }
        expressionList.Add(right);
      }
      right = !this.MaybeEat(TokenKind.KeywordYield) ? this.ParseTestListAsExpr() : this.ParseYieldExpression();
    }
    if (expressionList != null)
    {
      AssignmentStatement assignmentStatement = new AssignmentStatement(expressionList.ToArray(), right);
      assignmentStatement.SetLoc(this._globalParent, expressionList[0].StartIndex, right.EndIndex);
      return (Statement) assignmentStatement;
    }
    AssignmentStatement assignmentStatement1 = new AssignmentStatement(new Expression[1]
    {
      expression
    }, right);
    assignmentStatement1.SetLoc(this._globalParent, expression.StartIndex, right.EndIndex);
    return (Statement) assignmentStatement1;
  }

  private Statement ParseExprStmt()
  {
    Expression testListAsExpr = this.ParseTestListAsExpr();
    if (testListAsExpr is ErrorExpression)
      this.NextToken();
    if (this.PeekToken(TokenKind.Assign))
      return this.FinishAssignments(testListAsExpr);
    PythonOperator assignOperator = this.GetAssignOperator(this.PeekToken());
    if (assignOperator != PythonOperator.None)
    {
      this.NextToken();
      Expression right = !this.MaybeEat(TokenKind.KeywordYield) ? this.ParseTestListAsExpr() : this.ParseYieldExpression();
      string message = testListAsExpr.CheckAugmentedAssign();
      if (message != null)
        this.ReportSyntaxError(message);
      AugmentedAssignStatement exprStmt = new AugmentedAssignStatement(assignOperator, testListAsExpr, right);
      exprStmt.SetLoc(this._globalParent, testListAsExpr.StartIndex, this.GetEnd());
      return (Statement) exprStmt;
    }
    ExpressionStatement exprStmt1 = new ExpressionStatement(testListAsExpr);
    exprStmt1.SetLoc(this._globalParent, testListAsExpr.IndexSpan);
    return (Statement) exprStmt1;
  }

  private PythonOperator GetAssignOperator(Token t)
  {
    switch (t.Kind)
    {
      case TokenKind.AddEqual:
        return PythonOperator.Add;
      case TokenKind.SubtractEqual:
        return PythonOperator.Subtract;
      case TokenKind.PowerEqual:
        return PythonOperator.Power;
      case TokenKind.MultiplyEqual:
        return PythonOperator.Multiply;
      case TokenKind.FloorDivideEqual:
        return PythonOperator.FloorDivide;
      case TokenKind.DivideEqual:
        return !this.TrueDivision ? PythonOperator.Divide : PythonOperator.TrueDivide;
      case TokenKind.ModEqual:
        return PythonOperator.Mod;
      case TokenKind.LeftShiftEqual:
        return PythonOperator.LeftShift;
      case TokenKind.RightShiftEqual:
        return PythonOperator.RightShift;
      case TokenKind.BitwiseAndEqual:
        return PythonOperator.BitwiseAnd;
      case TokenKind.BitwiseOrEqual:
        return PythonOperator.BitwiseOr;
      case TokenKind.ExclusiveOrEqual:
        return PythonOperator.Xor;
      default:
        return PythonOperator.None;
    }
  }

  private PythonOperator GetBinaryOperator(OperatorToken token)
  {
    switch (token.Kind)
    {
      case TokenKind.Add:
        return PythonOperator.Add;
      case TokenKind.Subtract:
        return PythonOperator.Subtract;
      case TokenKind.Power:
        return PythonOperator.Power;
      case TokenKind.Multiply:
        return PythonOperator.Multiply;
      case TokenKind.FloorDivide:
        return PythonOperator.FloorDivide;
      case TokenKind.Divide:
        return !this.TrueDivision ? PythonOperator.Divide : PythonOperator.TrueDivide;
      case TokenKind.Mod:
        return PythonOperator.Mod;
      case TokenKind.LeftShift:
        return PythonOperator.LeftShift;
      case TokenKind.RightShift:
        return PythonOperator.RightShift;
      case TokenKind.BitwiseAnd:
        return PythonOperator.BitwiseAnd;
      case TokenKind.BitwiseOr:
        return PythonOperator.BitwiseOr;
      case TokenKind.ExclusiveOr:
        return PythonOperator.Xor;
      default:
        throw new ValueErrorException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.UnexpectedToken, (object) token.Kind));
    }
  }

  private ImportStatement ParseImportStmt()
  {
    this.Eat(TokenKind.KeywordImport);
    int start = this.GetStart();
    List<ModuleName> moduleNameList = new List<ModuleName>();
    List<string> stringList = new List<string>();
    moduleNameList.Add(this.ParseModuleName());
    stringList.Add(this.MaybeParseAsName());
    while (this.MaybeEat(TokenKind.Comma))
    {
      moduleNameList.Add(this.ParseModuleName());
      stringList.Add(this.MaybeParseAsName());
    }
    ImportStatement importStmt = new ImportStatement(moduleNameList.ToArray(), stringList.ToArray(), this.AbsoluteImports);
    importStmt.SetLoc(this._globalParent, start, this.GetEnd());
    return importStmt;
  }

  private ModuleName ParseModuleName()
  {
    int start = this.GetStart();
    ModuleName moduleName = new ModuleName(this.ReadNames());
    moduleName.SetLoc(this._globalParent, start, this.GetEnd());
    return moduleName;
  }

  private ModuleName ParseRelativeModuleName()
  {
    int start1 = this.GetStart();
    int dotCount = 0;
    while (this.MaybeEat(TokenKind.Dot))
      ++dotCount;
    string[] names = ArrayUtils.EmptyStrings;
    if (this.PeekToken() is NameToken)
      names = this.ReadNames();
    ModuleName relativeModuleName;
    if (dotCount > 0)
    {
      relativeModuleName = (ModuleName) new RelativeModuleName(names, dotCount);
    }
    else
    {
      if (names.Length == 0)
      {
        IndexSpan span = this._lookahead.Span;
        int start2 = span.Start;
        span = this._lookahead.Span;
        int end = span.End;
        this.ReportSyntaxError(start2, end, "invalid syntax");
      }
      relativeModuleName = new ModuleName(names);
    }
    relativeModuleName.SetLoc(this._globalParent, start1, this.GetEnd());
    return relativeModuleName;
  }

  private string[] ReadNames()
  {
    List<string> stringList = new List<string>();
    stringList.Add(this.ReadName());
    while (this.MaybeEat(TokenKind.Dot))
      stringList.Add(this.ReadName());
    return stringList.ToArray();
  }

  private FromImportStatement ParseFromImportStmt()
  {
    this.Eat(TokenKind.KeywordFrom);
    int start = this.GetStart();
    ModuleName relativeModuleName = this.ParseRelativeModuleName();
    this.Eat(TokenKind.KeywordImport);
    bool flag = this.MaybeEat(TokenKind.LeftParenthesis);
    bool fromFuture = false;
    string[] names;
    string[] asNames;
    if (this.MaybeEat(TokenKind.Multiply))
    {
      names = (string[]) FromImportStatement.Star;
      asNames = (string[]) null;
    }
    else
    {
      List<string> l = new List<string>();
      List<string> las = new List<string>();
      if (this.MaybeEat(TokenKind.LeftParenthesis))
      {
        this.ParseAsNameList(l, las);
        this.Eat(TokenKind.RightParenthesis);
      }
      else
        this.ParseAsNameList(l, las);
      names = l.ToArray();
      asNames = las.ToArray();
    }
    if (relativeModuleName.Names.Count == 1 && relativeModuleName.Names[0] == "__future__")
    {
      if (!this._fromFutureAllowed)
        this.ReportSyntaxError(Resources.MisplacedFuture);
      if (names == FromImportStatement.Star)
        this.ReportSyntaxError(Resources.NoFutureStar);
      fromFuture = true;
      foreach (string str1 in names)
      {
        switch (str1)
        {
          case "division":
            this._languageFeatures |= ModuleOptions.TrueDivision;
            continue;
          case "with_statement":
            this._languageFeatures |= ModuleOptions.WithStatement;
            continue;
          case "absolute_import":
            this._languageFeatures |= ModuleOptions.AbsoluteImports;
            continue;
          case "print_function":
            this._languageFeatures |= ModuleOptions.PrintFunction;
            this._tokenizer.PrintFunction = true;
            continue;
          case "unicode_literals":
            this._tokenizer.UnicodeLiterals = true;
            this._languageFeatures |= ModuleOptions.UnicodeLiterals;
            continue;
          case "nested_scopes":
          case "generators":
            continue;
          default:
            string str2 = str1;
            fromFuture = false;
            if (str2 != "braces")
            {
              this.ReportSyntaxError(Resources.UnknownFutureFeature + str2);
              continue;
            }
            this.ReportSyntaxError(Resources.NotAChance);
            continue;
        }
      }
    }
    if (flag)
      this.Eat(TokenKind.RightParenthesis);
    FromImportStatement fromImportStmt = new FromImportStatement(relativeModuleName, names, asNames, fromFuture, this.AbsoluteImports);
    fromImportStmt.SetLoc(this._globalParent, start, this.GetEnd());
    return fromImportStmt;
  }

  private void ParseAsNameList(List<string> l, List<string> las)
  {
    l.Add(this.ReadName());
    las.Add(this.MaybeParseAsName());
    while (this.MaybeEat(TokenKind.Comma) && !this.PeekToken(TokenKind.RightParenthesis))
    {
      l.Add(this.ReadName());
      las.Add(this.MaybeParseAsName());
    }
  }

  private string MaybeParseAsName()
  {
    return this.MaybeEat(TokenKind.KeywordAs) ? this.ReadName() : (string) null;
  }

  private ExecStatement ParseExecStmt()
  {
    this.Eat(TokenKind.KeywordExec);
    int start = this.GetStart();
    Expression locals = (Expression) null;
    Expression globals = (Expression) null;
    Expression expr = this.ParseExpr();
    if (this.MaybeEat(TokenKind.KeywordIn))
    {
      globals = this.ParseExpression();
      if (this.MaybeEat(TokenKind.Comma))
        locals = this.ParseExpression();
    }
    else if (expr is TupleExpression tupleExpression && (tupleExpression.Items.Count == 2 || tupleExpression.Items.Count == 3))
    {
      globals = tupleExpression.Items[1];
      if (tupleExpression.Items.Count == 3)
        locals = tupleExpression.Items[2];
      expr = tupleExpression.Items[0];
    }
    ExecStatement execStmt = new ExecStatement(expr, locals, globals);
    execStmt.SetLoc(this._globalParent, start, this.GetEnd());
    return execStmt;
  }

  private GlobalStatement ParseGlobalStmt()
  {
    this.Eat(TokenKind.KeywordGlobal);
    int start = this.GetStart();
    List<string> stringList = new List<string>();
    stringList.Add(this.ReadName());
    while (this.MaybeEat(TokenKind.Comma))
      stringList.Add(this.ReadName());
    GlobalStatement globalStmt = new GlobalStatement(stringList.ToArray());
    globalStmt.SetLoc(this._globalParent, start, this.GetEnd());
    return globalStmt;
  }

  private RaiseStatement ParseRaiseStmt()
  {
    this.Eat(TokenKind.KeywordRaise);
    int start = this.GetStart();
    Expression exceptionType = (Expression) null;
    Expression exceptionValue = (Expression) null;
    Expression traceBack = (Expression) null;
    if (!Parser.NeverTestToken(this.PeekToken()))
    {
      exceptionType = this.ParseExpression();
      if (this.MaybeEat(TokenKind.Comma))
      {
        exceptionValue = this.ParseExpression();
        if (this.MaybeEat(TokenKind.Comma))
          traceBack = this.ParseExpression();
      }
    }
    RaiseStatement raiseStmt = new RaiseStatement(exceptionType, exceptionValue, traceBack);
    raiseStmt.SetLoc(this._globalParent, start, this.GetEnd());
    return raiseStmt;
  }

  private AssertStatement ParseAssertStmt()
  {
    this.Eat(TokenKind.KeywordAssert);
    int start = this.GetStart();
    Expression expression1 = this.ParseExpression();
    Expression expression2 = (Expression) null;
    if (this.MaybeEat(TokenKind.Comma))
      expression2 = this.ParseExpression();
    Expression message = expression2;
    AssertStatement assertStmt = new AssertStatement(expression1, message);
    assertStmt.SetLoc(this._globalParent, start, this.GetEnd());
    return assertStmt;
  }

  private PrintStatement ParsePrintStmt()
  {
    this.Eat(TokenKind.KeywordPrint);
    int start = this.GetStart();
    Expression destination = (Expression) null;
    bool flag = false;
    if (this.MaybeEat(TokenKind.RightShift))
    {
      destination = this.ParseExpression();
      if (this.MaybeEat(TokenKind.Comma))
      {
        flag = true;
      }
      else
      {
        PrintStatement printStmt = new PrintStatement(destination, new Expression[0], false);
        printStmt.SetLoc(this._globalParent, start, this.GetEnd());
        return printStmt;
      }
    }
    bool trailingComma;
    List<Expression> expressionList = this.ParseExpressionList(out trailingComma);
    if (flag && expressionList.Count == 0)
      this.ReportSyntaxError(this._lookahead);
    Expression[] array = expressionList.ToArray();
    PrintStatement printStmt1 = new PrintStatement(destination, array, trailingComma);
    printStmt1.SetLoc(this._globalParent, start, this.GetEnd());
    return printStmt1;
  }

  private string SetPrivatePrefix(string name)
  {
    string privatePrefix = this._privatePrefix;
    this._privatePrefix = Parser.GetPrivatePrefix(name);
    return privatePrefix;
  }

  internal static string GetPrivatePrefix(string name)
  {
    if (name != null)
    {
      for (int index = 0; index < name.Length; ++index)
      {
        if (name[index] != '_')
          return name.Substring(index);
      }
    }
    return (string) null;
  }

  private ErrorExpression Error()
  {
    ErrorExpression errorExpression = new ErrorExpression();
    errorExpression.SetLoc(this._globalParent, this.GetStart(), this.GetEnd());
    return errorExpression;
  }

  private ExpressionStatement ErrorStmt() => new ExpressionStatement((Expression) this.Error());

  private ClassDefinition ParseClassDef()
  {
    this.Eat(TokenKind.KeywordClass);
    int start = this.GetStart();
    string name = this.ReadName();
    if (name == null)
      return new ClassDefinition((string) null, new Expression[0], (Statement) this.ErrorStmt());
    Expression[] bases = new Expression[0];
    if (this.MaybeEat(TokenKind.LeftParenthesis))
    {
      List<Expression> testList = this.ParseTestList();
      if (testList.Count == 1 && testList[0] is ErrorExpression)
        return new ClassDefinition(name, new Expression[0], (Statement) this.ErrorStmt());
      bases = testList.ToArray();
      this.Eat(TokenKind.RightParenthesis);
    }
    int end = this.GetEnd();
    string str = this.SetPrivatePrefix(name);
    Statement classOrFuncBody = this.ParseClassOrFuncBody();
    this._privatePrefix = str;
    ClassDefinition classDef = new ClassDefinition(name, bases, classOrFuncBody);
    classDef.HeaderIndex = end;
    classDef.SetLoc(this._globalParent, start, this.GetEnd());
    return classDef;
  }

  private List<Expression> ParseDecorators()
  {
    List<Expression> decorators = new List<Expression>();
    while (this.MaybeEat(TokenKind.At))
    {
      int start = this.GetStart();
      Expression target = (Expression) new NameExpression(this.ReadName());
      target.SetLoc(this._globalParent, start, this.GetEnd());
      while (this.MaybeEat(TokenKind.Dot))
      {
        string name = this.ReadNameMaybeNone();
        target = (Expression) new MemberExpression(target, name);
        target.SetLoc(this._globalParent, this.GetStart(), this.GetEnd());
      }
      target.SetLoc(this._globalParent, start, this.GetEnd());
      if (this.MaybeEat(TokenKind.LeftParenthesis))
      {
        if (this._sink != null)
          this._sink.StartParameters(this.GetSourceSpan());
        Arg[] objArray = this.FinishArgumentList((Arg) null);
        target = (Expression) this.FinishCallExpr(target, objArray);
      }
      target.SetLoc(this._globalParent, start, this.GetEnd());
      this.EatNewLine();
      decorators.Add(target);
    }
    return decorators;
  }

  private Statement ParseDecorated()
  {
    List<Expression> decorators = this.ParseDecorators();
    Statement decorated;
    if (this.PeekToken() == Tokens.KeywordDefToken)
    {
      FunctionDefinition funcDef = this.ParseFuncDef();
      funcDef.Decorators = (IList<Expression>) decorators.ToArray();
      decorated = (Statement) funcDef;
    }
    else if (this.PeekToken() == Tokens.KeywordClassToken)
    {
      ClassDefinition classDef = this.ParseClassDef();
      classDef.Decorators = (IList<Expression>) decorators.ToArray();
      decorated = (Statement) classDef;
    }
    else
    {
      decorated = (Statement) new EmptyStatement();
      this.ReportSyntaxError(this._lookahead);
    }
    return decorated;
  }

  private FunctionDefinition ParseFuncDef()
  {
    this.Eat(TokenKind.KeywordDef);
    int start1 = this.GetStart();
    string name = this.ReadName();
    this.Eat(TokenKind.LeftParenthesis);
    int start2 = this.GetStart();
    int end1 = this.GetEnd();
    int groupingLevel = this._tokenizer.GroupingLevel;
    Parameter[] varArgsList = this.ParseVarArgsList(TokenKind.RightParenthesis);
    if (varArgsList == null)
    {
      FunctionDefinition funcDef = new FunctionDefinition(name, new Parameter[0]);
      funcDef.SetLoc(this._globalParent, start1, end1);
      return funcDef;
    }
    int start3 = this.GetStart();
    int end2 = this.GetEnd();
    FunctionDefinition function = new FunctionDefinition(name, varArgsList);
    this.PushFunction(function);
    Statement classOrFuncBody = this.ParseClassOrFuncBody();
    this.PopFunction();
    function.Body = classOrFuncBody;
    function.HeaderIndex = end2;
    if (this._sink != null)
      this._sink.MatchPair(new SourceSpan(this._tokenizer.IndexToLocation(start2), this._tokenizer.IndexToLocation(end1)), new SourceSpan(this._tokenizer.IndexToLocation(start3), this._tokenizer.IndexToLocation(end2)), groupingLevel);
    function.SetLoc(this._globalParent, start1, classOrFuncBody.EndIndex);
    return function;
  }

  private Parameter ParseParameterName(HashSet<string> names, ParameterKind kind)
  {
    string name = this.ReadName();
    if (name == null)
      return (Parameter) null;
    this.CheckUniqueParameter(names, name);
    Parameter parameterName = new Parameter(name, kind);
    parameterName.SetLoc(this._globalParent, this.GetStart(), this.GetEnd());
    return parameterName;
  }

  private void CheckUniqueParameter(HashSet<string> names, string name)
  {
    if (names.Contains(name))
      this.ReportSyntaxError(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.DuplicateArgumentInFuncDef, (object) name));
    names.Add(name);
  }

  private Parameter[] ParseVarArgsList(TokenKind terminator)
  {
    List<Parameter> parameterList = new List<Parameter>();
    HashSet<string> names = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
    bool flag = false;
    int position = 0;
    while (!this.MaybeEat(terminator))
    {
      if (this.MaybeEat(TokenKind.Multiply))
      {
        Parameter parameterName1 = this.ParseParameterName(names, ParameterKind.List);
        if (parameterName1 == null)
          return (Parameter[]) null;
        parameterList.Add(parameterName1);
        if (this.MaybeEat(TokenKind.Comma))
        {
          this.Eat(TokenKind.Power);
          Parameter parameterName2 = this.ParseParameterName(names, ParameterKind.Dictionary);
          if (parameterName2 == null)
            return (Parameter[]) null;
          parameterList.Add(parameterName2);
        }
        this.Eat(terminator);
        break;
      }
      if (this.MaybeEat(TokenKind.Power))
      {
        Parameter parameterName = this.ParseParameterName(names, ParameterKind.Dictionary);
        if (parameterName == null)
          return (Parameter[]) null;
        parameterList.Add(parameterName);
        this.Eat(terminator);
        break;
      }
      Parameter parameter;
      if ((parameter = this.ParseParameter(position, names)) == null)
        return (Parameter[]) null;
      parameterList.Add(parameter);
      if (this.MaybeEat(TokenKind.Assign))
      {
        flag = true;
        parameter.DefaultValue = this.ParseExpression();
      }
      else if (flag)
        this.ReportSyntaxError(Resources.DefaultRequired);
      if (!this.MaybeEat(TokenKind.Comma))
      {
        this.Eat(terminator);
        break;
      }
      ++position;
    }
    return parameterList.ToArray();
  }

  private Parameter ParseParameter(int position, HashSet<string> names)
  {
    Token token = this.PeekToken();
    Parameter parameter = (Parameter) null;
    switch (token.Kind)
    {
      case TokenKind.Name:
        this.NextToken();
        string name = this.FixName((string) token.Value);
        parameter = new Parameter(name);
        this.CompleteParameterName((IronPython.Compiler.Ast.Node) parameter, name, names);
        break;
      case TokenKind.LeftParenthesis:
        this.NextToken();
        Expression sublist = this.ParseSublist(names);
        this.Eat(TokenKind.RightParenthesis);
        switch (sublist)
        {
          case TupleExpression tuple:
            parameter = (Parameter) new SublistParameter(position, tuple);
            break;
          case NameExpression nameExpression:
            parameter = new Parameter(nameExpression.Name);
            break;
          default:
            this.ReportSyntaxError(this._lookahead);
            break;
        }
        if (parameter != null)
        {
          parameter.SetLoc(this._globalParent, sublist.IndexSpan);
          break;
        }
        break;
      default:
        this.ReportSyntaxError(this._lookahead);
        break;
    }
    return parameter;
  }

  private void CompleteParameterName(IronPython.Compiler.Ast.Node node, string name, HashSet<string> names)
  {
    if (this._sink != null)
      this._sink.StartName(this.GetSourceSpan(), name);
    this.CheckUniqueParameter(names, name);
    node.SetLoc(this._globalParent, this.GetStart(), this.GetEnd());
  }

  private Expression ParseSublistParameter(HashSet<string> names)
  {
    Token token = this.NextToken();
    Expression sublistParameter1;
    switch (token.Kind)
    {
      case TokenKind.Name:
        string name = this.FixName((string) token.Value);
        NameExpression sublistParameter2 = new NameExpression(name);
        this.CompleteParameterName((IronPython.Compiler.Ast.Node) sublistParameter2, name, names);
        return (Expression) sublistParameter2;
      case TokenKind.LeftParenthesis:
        sublistParameter1 = this.ParseSublist(names);
        this.Eat(TokenKind.RightParenthesis);
        break;
      default:
        this.ReportSyntaxError(this._token);
        sublistParameter1 = (Expression) this.Error();
        break;
    }
    return sublistParameter1;
  }

  private Expression ParseSublist(HashSet<string> names)
  {
    List<Expression> l = new List<Expression>();
label_1:
    l.Add(this.ParseSublistParameter(names));
    bool trailingComma;
    if (this.MaybeEat(TokenKind.Comma))
    {
      trailingComma = true;
      switch (this.PeekToken().Kind)
      {
        case TokenKind.Name:
        case TokenKind.LeftParenthesis:
          goto label_1;
      }
    }
    else
      trailingComma = false;
    return this.MakeTupleOrExpr(l, trailingComma);
  }

  private Expression FinishOldLambdef()
  {
    return this.ParseLambdaHelperEnd(this.ParseLambdaHelperStart((string) null), this.ParseOldExpression());
  }

  private Expression FinishLambdef()
  {
    return this.ParseLambdaHelperEnd(this.ParseLambdaHelperStart((string) null), this.ParseExpression());
  }

  private FunctionDefinition ParseLambdaHelperStart(string name)
  {
    int start = this.GetStart();
    Parameter[] varArgsList = this.ParseVarArgsList(TokenKind.Colon);
    int end = this.GetEnd();
    FunctionDefinition function = new FunctionDefinition(name, varArgsList ?? new Parameter[0]);
    function.HeaderIndex = end;
    function.StartIndex = start;
    this.PushFunction(function);
    return function;
  }

  private Expression ParseLambdaHelperEnd(FunctionDefinition func, Expression expr)
  {
    Statement statement;
    if (func.IsGenerator)
    {
      YieldExpression yieldExpression = new YieldExpression(expr);
      yieldExpression.SetLoc(this._globalParent, expr.IndexSpan);
      statement = (Statement) new ExpressionStatement((Expression) yieldExpression);
    }
    else
      statement = (Statement) new ReturnStatement(expr);
    statement.SetLoc(this._globalParent, expr.StartIndex, expr.EndIndex);
    this.PopFunction();
    func.Body = statement;
    func.EndIndex = this.GetEnd();
    LambdaExpression lambdaHelperEnd = new LambdaExpression(func);
    func.SetLoc(this._globalParent, func.IndexSpan);
    lambdaHelperEnd.SetLoc(this._globalParent, func.IndexSpan);
    return (Expression) lambdaHelperEnd;
  }

  private WhileStatement ParseWhileStmt()
  {
    this.Eat(TokenKind.KeywordWhile);
    int start = this.GetStart();
    Expression expression = this.ParseExpression();
    int end = this.GetEnd();
    Statement loopSuite = this.ParseLoopSuite();
    Statement statement = (Statement) null;
    if (this.MaybeEat(TokenKind.KeywordElse))
      statement = this.ParseSuite();
    Statement body = loopSuite;
    Statement else_ = statement;
    WhileStatement whileStmt = new WhileStatement(expression, body, else_);
    whileStmt.SetLoc(this._globalParent, start, end, this.GetEnd());
    return whileStmt;
  }

  private WithStatement ParseWithStmt()
  {
    this.Eat(TokenKind.LastKeyword);
    Parser.WithItem withItem1 = this.ParseWithItem();
    List<Parser.WithItem> withItemList = (List<Parser.WithItem>) null;
    while (this.MaybeEat(TokenKind.Comma))
    {
      if (withItemList == null)
        withItemList = new List<Parser.WithItem>();
      withItemList.Add(this.ParseWithItem());
    }
    int end = this.GetEnd();
    Statement body = this.ParseSuite();
    if (withItemList != null)
    {
      for (int index = withItemList.Count - 1; index >= 0; --index)
      {
        Parser.WithItem withItem2 = withItemList[index];
        WithStatement withStatement = new WithStatement(withItem2.ContextManager, withItem2.Variable, body);
        withStatement.HeaderIndex = end;
        withStatement.SetLoc(this._globalParent, withItem1.Start, this.GetEnd());
        body = (Statement) withStatement;
        end = this.GetEnd();
      }
    }
    WithStatement withStmt = new WithStatement(withItem1.ContextManager, withItem1.Variable, body);
    withStmt.HeaderIndex = end;
    withStmt.SetLoc(this._globalParent, withItem1.Start, this.GetEnd());
    return withStmt;
  }

  private Parser.WithItem ParseWithItem()
  {
    int start = this.GetStart();
    Expression expression1 = this.ParseExpression();
    Expression expression2 = (Expression) null;
    if (this.MaybeEat(TokenKind.KeywordAs))
      expression2 = this.ParseExpression();
    Expression contextManager = expression1;
    Expression variable = expression2;
    return new Parser.WithItem(start, contextManager, variable);
  }

  private ForStatement ParseForStmt()
  {
    this.Eat(TokenKind.KeywordFor);
    int start = this.GetStart();
    bool trailingComma;
    Expression left = this.MakeTupleOrExpr(this.ParseTargetList(out trailingComma), trailingComma);
    this.Eat(TokenKind.KeywordIn);
    Expression testListAsExpr = this.ParseTestListAsExpr();
    int end = this.GetEnd();
    Statement loopSuite = this.ParseLoopSuite();
    Statement statement = (Statement) null;
    if (this.MaybeEat(TokenKind.KeywordElse))
      statement = this.ParseSuite();
    Expression list = testListAsExpr;
    Statement body = loopSuite;
    Statement else_ = statement;
    ForStatement forStmt = new ForStatement(left, list, body, else_);
    forStmt.HeaderIndex = end;
    forStmt.SetLoc(this._globalParent, start, this.GetEnd());
    return forStmt;
  }

  private Statement ParseLoopSuite()
  {
    bool inLoop = this._inLoop;
    bool inFinallyLoop = this._inFinallyLoop;
    try
    {
      this._inLoop = true;
      this._inFinallyLoop = this._inFinally;
      return this.ParseSuite();
    }
    finally
    {
      this._inLoop = inLoop;
      this._inFinallyLoop = inFinallyLoop;
    }
  }

  private Statement ParseClassOrFuncBody()
  {
    bool inLoop = this._inLoop;
    bool inFinally = this._inFinally;
    bool inFinallyLoop = this._inFinallyLoop;
    bool isGenerator = this._isGenerator;
    bool returnWithValue = this._returnWithValue;
    try
    {
      this._inLoop = false;
      this._inFinally = false;
      this._inFinallyLoop = false;
      this._isGenerator = false;
      this._returnWithValue = false;
      return this.ParseSuite();
    }
    finally
    {
      this._inLoop = inLoop;
      this._inFinally = inFinally;
      this._inFinallyLoop = inFinallyLoop;
      this._isGenerator = isGenerator;
      this._returnWithValue = returnWithValue;
    }
  }

  private IfStatement ParseIfStmt()
  {
    this.Eat(TokenKind.KeywordIf);
    int start = this.GetStart();
    List<IfStatementTest> ifStatementTestList = new List<IfStatementTest>();
    ifStatementTestList.Add(this.ParseIfStmtTest());
    while (this.MaybeEat(TokenKind.KeywordElseIf))
      ifStatementTestList.Add(this.ParseIfStmtTest());
    Statement else_ = (Statement) null;
    if (this.MaybeEat(TokenKind.KeywordElse))
      else_ = this.ParseSuite();
    IfStatement ifStmt = new IfStatement(ifStatementTestList.ToArray(), else_);
    ifStmt.SetLoc(this._globalParent, start, this.GetEnd());
    return ifStmt;
  }

  private IfStatementTest ParseIfStmtTest()
  {
    int start = this.GetStart();
    Expression expression = this.ParseExpression();
    int end = this.GetEnd();
    Statement suite = this.ParseSuite();
    Statement body = suite;
    IfStatementTest ifStmtTest = new IfStatementTest(expression, body);
    ifStmtTest.SetLoc(this._globalParent, start, suite.EndIndex);
    ifStmtTest.HeaderIndex = end;
    return ifStmtTest;
  }

  private Statement ParseTryStatement()
  {
    this.Eat(TokenKind.KeywordTry);
    int start = this.GetStart();
    int end = this.GetEnd();
    Statement suite = this.ParseSuite();
    Statement statement = (Statement) null;
    Statement else_ = (Statement) null;
    int endIndex;
    Statement tryStatement;
    if (this.MaybeEat(TokenKind.KeywordFinally))
    {
      Statement finallySuite = this.ParseFinallySuite(statement);
      endIndex = finallySuite.EndIndex;
      tryStatement = (Statement) new TryStatement(suite, (TryStatementHandler[]) null, else_, finallySuite)
      {
        HeaderIndex = end
      };
    }
    else
    {
      List<TryStatementHandler> statementHandlerList = new List<TryStatementHandler>();
      TryStatementHandler statementHandler = (TryStatementHandler) null;
      do
      {
        TryStatementHandler tryStmtHandler = this.ParseTryStmtHandler();
        endIndex = tryStmtHandler.EndIndex;
        statementHandlerList.Add(tryStmtHandler);
        if (statementHandler != null)
          this.ReportSyntaxError(statementHandler.StartIndex, statementHandler.EndIndex, "default 'except' must be last");
        if (tryStmtHandler.Test == null)
          statementHandler = tryStmtHandler;
      }
      while (this.PeekToken().Kind == TokenKind.KeywordExcept);
      if (this.MaybeEat(TokenKind.KeywordElse))
      {
        else_ = this.ParseSuite();
        endIndex = else_.EndIndex;
      }
      if (this.MaybeEat(TokenKind.KeywordFinally))
      {
        statement = this.ParseFinallySuite(statement);
        endIndex = statement.EndIndex;
      }
      tryStatement = (Statement) new TryStatement(suite, statementHandlerList.ToArray(), else_, statement)
      {
        HeaderIndex = end
      };
    }
    tryStatement.SetLoc(this._globalParent, start, endIndex);
    return tryStatement;
  }

  private Statement ParseFinallySuite(Statement finallySuite)
  {
    this.MarkFunctionContainsFinally();
    bool inFinally = this._inFinally;
    bool inFinallyLoop = this._inFinallyLoop;
    try
    {
      this._inFinally = true;
      this._inFinallyLoop = false;
      finallySuite = this.ParseSuite();
    }
    finally
    {
      this._inFinally = inFinally;
      this._inFinallyLoop = inFinallyLoop;
    }
    return finallySuite;
  }

  private void MarkFunctionContainsFinally()
  {
    FunctionDefinition currentFunction = this.CurrentFunction;
    if (currentFunction == null)
      return;
    currentFunction.ContainsTryFinally = true;
  }

  private TryStatementHandler ParseTryStmtHandler()
  {
    this.Eat(TokenKind.KeywordExcept);
    FunctionDefinition currentFunction = this.CurrentFunction;
    if (currentFunction != null)
      currentFunction.CanSetSysExcInfo = true;
    int start = this.GetStart();
    Expression test = (Expression) null;
    Expression target = (Expression) null;
    if (this.PeekToken().Kind != TokenKind.Colon)
    {
      test = this.ParseExpression();
      if (this.MaybeEat(TokenKind.Comma) || this.MaybeEat(TokenKind.KeywordAs))
        target = this.ParseExpression();
    }
    int end = this.GetEnd();
    Statement suite = this.ParseSuite();
    TryStatementHandler tryStmtHandler = new TryStatementHandler(test, target, suite);
    tryStmtHandler.HeaderIndex = end;
    tryStmtHandler.SetLoc(this._globalParent, start, suite.EndIndex);
    return tryStmtHandler;
  }

  private Statement ParseSuite()
  {
    if (!this.EatNoEof(TokenKind.Colon))
      return (Statement) this.ErrorStmt();
    TokenWithSpan lookahead1 = this._lookahead;
    List<Statement> statementList = new List<Statement>();
    if (!this.MaybeEat(TokenKind.NewLine))
      return this.ParseSimpleStmt();
    this.CheckSuiteEofError(lookahead1);
    TokenWithSpan lookahead2 = this._lookahead;
    while (this.PeekToken(TokenKind.NLToken))
    {
      lookahead2 = this._lookahead;
      this.NextToken();
    }
    if (!this.MaybeEat(TokenKind.Indent))
    {
      if (lookahead2.Token.Kind == TokenKind.Dedent)
        this.ReportSyntaxError(this._lookahead.Span.Start, this._lookahead.Span.End, Resources.ExpectedIndentation, 33);
      else
        this.ReportSyntaxError(lookahead2, 32 /*0x20*/);
      return (Statement) this.ErrorStmt();
    }
    do
    {
      Statement stmt = this.ParseStmt();
      statementList.Add(stmt);
      if (this.MaybeEat(TokenKind.Dedent))
        goto label_14;
    }
    while (this.PeekToken().Kind != TokenKind.EndOfFile);
    this.ReportSyntaxError("unexpected end of file");
label_14:
    Statement[] array = statementList.ToArray();
    SuiteStatement suite = new SuiteStatement(array);
    suite.SetLoc(this._globalParent, array[0].StartIndex, array[array.Length - 1].EndIndex);
    return (Statement) suite;
  }

  private void CheckSuiteEofError(TokenWithSpan cur)
  {
    if (!this.MaybeEat(TokenKind.EndOfFile))
      return;
    this.ReportSyntaxError(this._lookahead.Token, cur.Span, 16 /*0x10*/, true);
  }

  private Expression ParseOldExpression()
  {
    return this.MaybeEat(TokenKind.KeywordLambda) ? this.FinishOldLambdef() : this.ParseOrTest();
  }

  private Expression ParseExpression()
  {
    if (this.MaybeEat(TokenKind.KeywordLambda))
      return this.FinishLambdef();
    Expression trueExpr = this.ParseOrTest();
    if (this.MaybeEat(TokenKind.KeywordIf))
    {
      int startIndex = trueExpr.StartIndex;
      trueExpr = this.ParseConditionalTest(trueExpr);
      trueExpr.SetLoc(this._globalParent, startIndex, this.GetEnd());
    }
    return trueExpr;
  }

  private Expression ParseOrTest()
  {
    Expression left = this.ParseAndTest();
    while (this.MaybeEat(TokenKind.KeywordOr))
    {
      int startIndex = left.StartIndex;
      left = (Expression) new OrExpression(left, this.ParseAndTest());
      left.SetLoc(this._globalParent, startIndex, this.GetEnd());
    }
    return left;
  }

  private Expression ParseConditionalTest(Expression trueExpr)
  {
    Expression orTest = this.ParseOrTest();
    this.Eat(TokenKind.KeywordElse);
    Expression expression = this.ParseExpression();
    Expression trueExpression = trueExpr;
    Expression falseExpression = expression;
    return (Expression) new ConditionalExpression(orTest, trueExpression, falseExpression);
  }

  private Expression ParseAndTest()
  {
    Expression left = this.ParseNotTest();
    while (this.MaybeEat(TokenKind.FirstKeyword))
    {
      int startIndex = left.StartIndex;
      left = (Expression) new AndExpression(left, this.ParseAndTest());
      left.SetLoc(this._globalParent, startIndex, this.GetEnd());
    }
    return left;
  }

  private Expression ParseNotTest()
  {
    if (!this.MaybeEat(TokenKind.KeywordNot))
      return this.ParseComparison();
    int start = this.GetStart();
    UnaryExpression notTest = new UnaryExpression(PythonOperator.Not, this.ParseNotTest());
    notTest.SetLoc(this._globalParent, start, this.GetEnd());
    return (Expression) notTest;
  }

  private Expression ParseComparison()
  {
    Expression left = this.ParseExpr();
    while (true)
    {
      PythonOperator op;
      switch (this.PeekToken().Kind)
      {
        case TokenKind.LessThan:
          this.NextToken();
          op = PythonOperator.LessThan;
          break;
        case TokenKind.GreaterThan:
          this.NextToken();
          op = PythonOperator.GreaterThan;
          break;
        case TokenKind.LessThanOrEqual:
          this.NextToken();
          op = PythonOperator.LessThanOrEqual;
          break;
        case TokenKind.GreaterThanOrEqual:
          this.NextToken();
          op = PythonOperator.GreaterThanOrEqual;
          break;
        case TokenKind.Equals:
          this.NextToken();
          op = PythonOperator.Equal;
          break;
        case TokenKind.NotEquals:
          this.NextToken();
          op = PythonOperator.NotEqual;
          break;
        case TokenKind.LessThanGreaterThan:
          this.NextToken();
          op = PythonOperator.NotEqual;
          break;
        case TokenKind.KeywordIn:
          this.NextToken();
          op = PythonOperator.In;
          break;
        case TokenKind.KeywordIs:
          this.NextToken();
          op = !this.MaybeEat(TokenKind.KeywordNot) ? PythonOperator.Is : PythonOperator.IsNot;
          break;
        case TokenKind.KeywordNot:
          this.NextToken();
          this.Eat(TokenKind.KeywordIn);
          op = PythonOperator.NotIn;
          break;
        default:
          goto label_12;
      }
      Expression comparison = this.ParseComparison();
      BinaryExpression binaryExpression = new BinaryExpression(op, left, comparison);
      binaryExpression.SetLoc(this._globalParent, left.StartIndex, this.GetEnd());
      left = (Expression) binaryExpression;
    }
label_12:
    return left;
  }

  private Expression ParseExpr() => this.ParseExpr(0);

  private Expression ParseExpr(int precedence)
  {
    Expression left = this.ParseFactor();
    while (this.PeekToken() is OperatorToken token)
    {
      int precedence1 = token.Precedence;
      if (precedence1 < precedence)
        return left;
      this.NextToken();
      Expression expr = this.ParseExpr(precedence1 + 1);
      int startIndex = left.StartIndex;
      left = (Expression) new BinaryExpression(this.GetBinaryOperator(token), left, expr);
      left.SetLoc(this._globalParent, startIndex, this.GetEnd());
    }
    return left;
  }

  private Expression ParseFactor()
  {
    int start = this._lookahead.Span.Start;
    Expression factor;
    switch (this.PeekToken().Kind)
    {
      case TokenKind.Add:
        this.NextToken();
        factor = (Expression) new UnaryExpression(PythonOperator.Pos, this.ParseFactor());
        break;
      case TokenKind.Subtract:
        this.NextToken();
        factor = this.FinishUnaryNegate();
        break;
      case TokenKind.Twiddle:
        this.NextToken();
        factor = (Expression) new UnaryExpression(PythonOperator.Invert, this.ParseFactor());
        break;
      default:
        return this.ParsePower();
    }
    factor.SetLoc(this._globalParent, start, this.GetEnd());
    return factor;
  }

  private Expression FinishUnaryNegate()
  {
    if (this.PeekToken().Kind == TokenKind.Constant)
    {
      Token token = this.PeekToken();
      uint ret;
      if (token.Value is BigInteger && ((BigInteger) token.Value).AsUInt32(out ret) && ret == 2147483648U /*0x80000000*/)
      {
        string tokenString = this._tokenizer.GetTokenString();
        if (tokenString[tokenString.Length - 1] != 'L' && tokenString[tokenString.Length - 1] != 'l')
        {
          this.NextToken();
          return (Expression) new ConstantExpression((object) int.MinValue);
        }
      }
    }
    return (Expression) new UnaryExpression(PythonOperator.Negate, this.ParseFactor());
  }

  private Expression ParsePower()
  {
    Expression left = this.AddTrailers(this.ParsePrimary());
    if (this.MaybeEat(TokenKind.Power))
    {
      int startIndex = left.StartIndex;
      left = (Expression) new BinaryExpression(PythonOperator.Power, left, this.ParseFactor());
      left.SetLoc(this._globalParent, startIndex, this.GetEnd());
    }
    return left;
  }

  private Expression ParsePrimary()
  {
    Token token = this.PeekToken();
    switch (token.Kind)
    {
      case TokenKind.Name:
        this.NextToken();
        string name = (string) token.Value;
        if (this._sink != null)
          this._sink.StartName(this.GetSourceSpan(), name);
        Expression primary1 = (Expression) new NameExpression(this.FixName(name));
        primary1.SetLoc(this._globalParent, this.GetStart(), this.GetEnd());
        return primary1;
      case TokenKind.Constant:
        this.NextToken();
        int start = this.GetStart();
        object obj = token.Value;
        switch (obj)
        {
          case string s1:
            obj = (object) this.FinishStringPlus(s1);
            break;
          case Bytes s2:
            obj = (object) this.FinishBytesPlus(s2);
            break;
        }
        Expression primary2 = !(token is UnicodeStringToken) ? (Expression) new ConstantExpression(obj) : (Expression) ConstantExpression.MakeUnicode((string) obj);
        primary2.SetLoc(this._globalParent, start, this.GetEnd());
        return primary2;
      case TokenKind.LeftParenthesis:
        this.NextToken();
        return this.FinishTupleOrGenExp();
      case TokenKind.LeftBracket:
        this.NextToken();
        return this.FinishListValue();
      case TokenKind.LeftBrace:
        this.NextToken();
        return this.FinishDictOrSetValue();
      case TokenKind.BackQuote:
        this.NextToken();
        return this.FinishStringConversion();
      default:
        this.ReportSyntaxError(this._lookahead.Token, this._lookahead.Span, 16 /*0x10*/, this._allowIncomplete || this._tokenizer.EndContinues);
        Expression primary3 = (Expression) new ErrorExpression();
        primary3.SetLoc(this._globalParent, this._lookahead.Span.Start, this._lookahead.Span.End);
        return primary3;
    }
  }

  private string FinishStringPlus(string s)
  {
    for (Token token = this.PeekToken(); token is ConstantValueToken; token = this.PeekToken())
    {
      if (token.Value is string str)
      {
        s += str;
        this.NextToken();
      }
      else
      {
        this.ReportSyntaxError("invalid syntax");
        break;
      }
    }
    return s;
  }

  private Bytes FinishBytesPlus(Bytes s)
  {
    for (Token token = this.PeekToken(); token is ConstantValueToken; token = this.PeekToken())
    {
      if (token.Value is Bytes bytes)
      {
        s += bytes;
        this.NextToken();
      }
      else
      {
        this.ReportSyntaxError("invalid syntax");
        break;
      }
    }
    return s;
  }

  private Expression AddTrailers(Expression ret) => this.AddTrailers(ret, true);

  private Expression AddTrailers(Expression ret, bool allowGeneratorExpression)
  {
    bool allowIncomplete = this._allowIncomplete;
    try
    {
      this._allowIncomplete = true;
      while (true)
      {
        switch (this.PeekToken().Kind)
        {
          case TokenKind.Constant:
            goto label_8;
          case TokenKind.Dot:
            this.NextToken();
            string name = this.ReadNameMaybeNone();
            MemberExpression memberExpression = new MemberExpression(ret, name);
            memberExpression.SetLoc(this._globalParent, ret.StartIndex, this.GetEnd());
            ret = (Expression) memberExpression;
            continue;
          case TokenKind.LeftParenthesis:
            if (allowGeneratorExpression)
            {
              this.NextToken();
              Arg[] objArray = this.FinishArgListOrGenExpr();
              CallExpression callExpression = objArray == null ? new CallExpression(ret, new Arg[0]) : this.FinishCallExpr(ret, objArray);
              callExpression.SetLoc(this._globalParent, ret.StartIndex, this.GetEnd());
              ret = (Expression) callExpression;
              continue;
            }
            goto label_4;
          case TokenKind.LeftBracket:
            this.NextToken();
            Expression subscriptList = this.ParseSubscriptList();
            IndexExpression indexExpression = new IndexExpression(ret, subscriptList);
            indexExpression.SetLoc(this._globalParent, ret.StartIndex, this.GetEnd());
            ret = (Expression) indexExpression;
            continue;
          default:
            goto label_9;
        }
      }
label_4:
      return ret;
label_8:
      this.ReportSyntaxError("invalid syntax");
      return (Expression) this.Error();
label_9:
      return ret;
    }
    finally
    {
      this._allowIncomplete = allowIncomplete;
    }
  }

  private Expression ParseSubscriptList()
  {
    int start1 = this.GetStart();
    List<Expression> l = new List<Expression>();
    bool trailingComma;
    do
    {
      Expression e0;
      if (this.MaybeEat(TokenKind.Dot))
      {
        int start2 = this.GetStart();
        this.Eat(TokenKind.Dot);
        this.Eat(TokenKind.Dot);
        e0 = (Expression) new ConstantExpression((object) Ellipsis.Value);
        e0.SetLoc(this._globalParent, start2, this.GetEnd());
      }
      else if (this.MaybeEat(TokenKind.Colon))
      {
        e0 = this.FinishSlice((Expression) null, this.GetStart());
      }
      else
      {
        e0 = this.ParseExpression();
        if (this.MaybeEat(TokenKind.Colon))
          e0 = this.FinishSlice(e0, e0.StartIndex);
      }
      l.Add(e0);
      if (!this.MaybeEat(TokenKind.Comma))
      {
        this.Eat(TokenKind.RightBracket);
        trailingComma = false;
        break;
      }
      trailingComma = true;
    }
    while (!this.MaybeEat(TokenKind.RightBracket));
    Expression subscriptList = this.MakeTupleOrExpr(l, trailingComma, true);
    subscriptList.SetLoc(this._globalParent, start1, this.GetEnd());
    return subscriptList;
  }

  private Expression ParseSliceEnd()
  {
    Expression sliceEnd = (Expression) null;
    switch (this.PeekToken().Kind)
    {
      case TokenKind.RightBracket:
      case TokenKind.Comma:
        return sliceEnd;
      default:
        sliceEnd = this.ParseExpression();
        goto case TokenKind.RightBracket;
    }
  }

  private Expression FinishSlice(Expression e0, int start)
  {
    Expression stop = (Expression) null;
    Expression step = (Expression) null;
    bool stepProvided = false;
    switch (this.PeekToken().Kind)
    {
      case TokenKind.RightBracket:
      case TokenKind.Comma:
        SliceExpression sliceExpression = new SliceExpression(e0, stop, step, stepProvided);
        sliceExpression.SetLoc(this._globalParent, start, this.GetEnd());
        return (Expression) sliceExpression;
      case TokenKind.Colon:
        stepProvided = true;
        this.NextToken();
        step = this.ParseSliceEnd();
        goto case TokenKind.RightBracket;
      default:
        stop = this.ParseExpression();
        if (this.MaybeEat(TokenKind.Colon))
        {
          stepProvided = true;
          step = this.ParseSliceEnd();
          goto case TokenKind.RightBracket;
        }
        goto case TokenKind.RightBracket;
    }
  }

  private List<Expression> ParseExprList()
  {
    List<Expression> exprList = new List<Expression>();
    do
    {
      Expression expr = this.ParseExpr();
      exprList.Add(expr);
    }
    while (this.MaybeEat(TokenKind.Comma) && !Parser.NeverTestToken(this.PeekToken()));
    return exprList;
  }

  private Arg[] FinishArgListOrGenExpr()
  {
    Arg first = (Arg) null;
    if (this._sink != null)
      this._sink.StartParameters(this.GetSourceSpan());
    Token token = this.PeekToken();
    if (token.Kind != TokenKind.RightParenthesis && token.Kind != TokenKind.Multiply && token.Kind != TokenKind.Power)
    {
      int start = this.GetStart();
      Expression expression = this.ParseExpression();
      if (expression is ErrorExpression)
        return (Arg[]) null;
      if (this.MaybeEat(TokenKind.Assign))
      {
        first = this.FinishKeywordArgument(expression);
        if (first == null)
        {
          first = new Arg(expression);
          first.SetLoc(this._globalParent, expression.StartIndex, this.GetEnd());
        }
      }
      else
      {
        if (this.PeekToken(Tokens.KeywordForToken))
        {
          Arg obj = new Arg(this.ParseGeneratorExpression(expression));
          this.Eat(TokenKind.RightParenthesis);
          obj.SetLoc(this._globalParent, start, this.GetEnd());
          if (this._sink != null)
            this._sink.EndParameters(this.GetSourceSpan());
          return new Arg[1]{ obj };
        }
        first = new Arg(expression);
        first.SetLoc(this._globalParent, expression.StartIndex, expression.EndIndex);
      }
      if (this.MaybeEat(TokenKind.Comma))
      {
        if (this._sink != null)
          this._sink.NextParameter(this.GetSourceSpan());
      }
      else
      {
        this.Eat(TokenKind.RightParenthesis);
        first.SetLoc(this._globalParent, start, this.GetEnd());
        if (this._sink != null)
          this._sink.EndParameters(this.GetSourceSpan());
        return new Arg[1]{ first };
      }
    }
    return this.FinishArgumentList(first);
  }

  private Arg FinishKeywordArgument(Expression t)
  {
    if (!(t is NameExpression nameExpression))
    {
      this.ReportSyntaxError(Resources.ExpectedName);
      Arg obj = new Arg((string) null, t);
      obj.SetLoc(this._globalParent, t.StartIndex, t.EndIndex);
      return obj;
    }
    Expression expression = this.ParseExpression();
    Arg obj1 = new Arg(nameExpression.Name, expression);
    obj1.SetLoc(this._globalParent, nameExpression.StartIndex, expression.EndIndex);
    return obj1;
  }

  private void CheckUniqueArgument(List<Arg> names, Arg arg)
  {
    if (arg == null || arg.Name == null)
      return;
    for (int index = 0; index < names.Count; ++index)
    {
      if (names[index].Name == arg.Name)
        this.ReportSyntaxError(Resources.DuplicateKeywordArg);
    }
  }

  private Arg[] FinishArgumentList(Arg first)
  {
    List<Arg> names = new List<Arg>();
    if (first != null)
      names.Add(first);
    while (!this.MaybeEat(TokenKind.RightParenthesis))
    {
      int start = this.GetStart();
      Arg obj;
      if (this.MaybeEat(TokenKind.Multiply))
        obj = new Arg("*", this.ParseExpression());
      else if (this.MaybeEat(TokenKind.Power))
      {
        obj = new Arg("**", this.ParseExpression());
      }
      else
      {
        Expression expression = this.ParseExpression();
        if (this.MaybeEat(TokenKind.Assign))
        {
          obj = this.FinishKeywordArgument(expression);
          this.CheckUniqueArgument(names, obj);
        }
        else
          obj = new Arg(expression);
      }
      obj.SetLoc(this._globalParent, start, this.GetEnd());
      names.Add(obj);
      if (this.MaybeEat(TokenKind.Comma))
      {
        if (this._sink != null)
          this._sink.NextParameter(this.GetSourceSpan());
      }
      else
      {
        this.Eat(TokenKind.RightParenthesis);
        break;
      }
    }
    if (this._sink != null)
      this._sink.EndParameters(this.GetSourceSpan());
    return names.ToArray();
  }

  private List<Expression> ParseTestList() => this.ParseExpressionList(out bool _);

  private Expression ParseOldExpressionListAsExpr()
  {
    bool trailingComma;
    List<Expression> oldExpressionList = this.ParseOldExpressionList(out trailingComma);
    if (oldExpressionList.Count == 0 && !trailingComma)
      this.ReportSyntaxError("invalid syntax");
    return this.MakeTupleOrExpr(oldExpressionList, trailingComma);
  }

  private List<Expression> ParseOldExpressionList(out bool trailingComma)
  {
    List<Expression> oldExpressionList = new List<Expression>();
    trailingComma = false;
    while (!Parser.NeverTestToken(this.PeekToken()))
    {
      oldExpressionList.Add(this.ParseOldExpression());
      if (!this.MaybeEat(TokenKind.Comma))
      {
        trailingComma = false;
        break;
      }
      trailingComma = true;
    }
    return oldExpressionList;
  }

  private List<Expression> ParseTargetList(out bool trailingComma)
  {
    List<Expression> targetList = new List<Expression>();
    do
    {
      targetList.Add(this.ParseTarget());
      if (!this.MaybeEat(TokenKind.Comma))
      {
        trailingComma = false;
        break;
      }
      trailingComma = true;
    }
    while (!Parser.NeverTestToken(this.PeekToken()));
    return targetList;
  }

  private Expression ParseTarget()
  {
    Token token = this.PeekToken();
    switch (token.Kind)
    {
      case TokenKind.LeftParenthesis:
      case TokenKind.LeftBracket:
        this.Eat(token.Kind);
        bool trailingComma;
        Expression target = this.MakeTupleOrExpr(this.ParseTargetList(out trailingComma), trailingComma);
        if (token.Kind == TokenKind.LeftParenthesis)
        {
          this.Eat(TokenKind.RightParenthesis);
          return target;
        }
        this.Eat(TokenKind.RightBracket);
        return target;
      default:
        return this.AddTrailers(this.ParsePrimary(), false);
    }
  }

  private List<Expression> ParseExpressionList(out bool trailingComma)
  {
    List<Expression> expressionList = new List<Expression>();
    trailingComma = false;
    while (!Parser.NeverTestToken(this.PeekToken()))
    {
      expressionList.Add(this.ParseExpression());
      if (!this.MaybeEat(TokenKind.Comma))
      {
        trailingComma = false;
        break;
      }
      trailingComma = true;
    }
    return expressionList;
  }

  private Expression ParseTestListAsExpr()
  {
    if (Parser.NeverTestToken(this.PeekToken()))
      return this.ParseTestListAsExprError();
    Expression expression = this.ParseExpression();
    return !this.MaybeEat(TokenKind.Comma) ? expression : this.ParseTestListAsExpr(expression);
  }

  private Expression ParseTestListAsExpr(Expression expr)
  {
    List<Expression> l = new List<Expression>();
    l.Add(expr);
    bool trailingComma = true;
    while (!Parser.NeverTestToken(this.PeekToken()))
    {
      l.Add(this.ParseExpression());
      if (!this.MaybeEat(TokenKind.Comma))
      {
        trailingComma = false;
        break;
      }
    }
    return this.MakeTupleOrExpr(l, trailingComma);
  }

  private Expression ParseTestListAsExprError()
  {
    if (this.MaybeEat(TokenKind.Indent))
    {
      this.NextToken();
      this.ReportSyntaxError(this.GetStart(), this.GetEnd(), "unexpected indent", 32 /*0x20*/);
    }
    else
      this.ReportSyntaxError(this._lookahead);
    return (Expression) new ErrorExpression();
  }

  private Expression FinishExpressionListAsExpr(Expression expr)
  {
    int start = this.GetStart();
    bool trailingComma = true;
    List<Expression> l = new List<Expression>();
    l.Add(expr);
    while (!Parser.NeverTestToken(this.PeekToken()))
    {
      expr = this.ParseExpression();
      l.Add(expr);
      if (!this.MaybeEat(TokenKind.Comma))
      {
        trailingComma = false;
        break;
      }
      trailingComma = true;
    }
    Expression expression = this.MakeTupleOrExpr(l, trailingComma);
    expression.SetLoc(this._globalParent, start, this.GetEnd());
    return expression;
  }

  private Expression FinishTupleOrGenExp()
  {
    int start1 = this.GetStart();
    int end1 = this.GetEnd();
    int groupingLevel = this._tokenizer.GroupingLevel;
    Expression expression1;
    bool flag;
    if (this.MaybeEat(TokenKind.RightParenthesis))
    {
      expression1 = this.MakeTupleOrExpr(new List<Expression>(), false);
      flag = true;
    }
    else if (this.MaybeEat(TokenKind.KeywordYield))
    {
      expression1 = this.ParseYieldExpression();
      this.Eat(TokenKind.RightParenthesis);
      flag = true;
    }
    else
    {
      bool allowIncomplete = this._allowIncomplete;
      try
      {
        this._allowIncomplete = true;
        Expression expression2 = this.ParseExpression();
        expression1 = !this.MaybeEat(TokenKind.Comma) ? (!this.PeekToken(Tokens.KeywordForToken) ? (expression2 is ParenthesisExpression ? expression2 : (Expression) new ParenthesisExpression(expression2)) : this.ParseGeneratorExpression(expression2)) : this.FinishExpressionListAsExpr(expression2);
        flag = this.Eat(TokenKind.RightParenthesis);
      }
      finally
      {
        this._allowIncomplete = allowIncomplete;
      }
    }
    int start2 = this.GetStart();
    int end2 = this.GetEnd();
    if (flag && this._sink != null)
      this._sink.MatchPair(new SourceSpan(this._tokenizer.IndexToLocation(start1), this._tokenizer.IndexToLocation(end1)), new SourceSpan(this._tokenizer.IndexToLocation(start2), this._tokenizer.IndexToLocation(end2)), groupingLevel);
    expression1.SetLoc(this._globalParent, start1, end2);
    return expression1;
  }

  private Expression ParseGeneratorExpression(Expression expr)
  {
    ForStatement genExprFor = this.ParseGenExprFor();
    Statement current = (Statement) genExprFor;
    while (true)
    {
      while (!this.PeekToken(Tokens.KeywordForToken))
      {
        if (this.PeekToken(Tokens.KeywordIfToken))
        {
          current = Parser.NestGenExpr(current, (Statement) this.ParseGenExprIf());
        }
        else
        {
          ExpressionStatement nested = new ExpressionStatement((Expression) new YieldExpression(expr));
          nested.Expression.SetLoc(this._globalParent, expr.IndexSpan);
          nested.SetLoc(this._globalParent, expr.IndexSpan);
          Parser.NestGenExpr(current, (Statement) nested);
          FunctionDefinition function = new FunctionDefinition("<genexpr>", new Parameter[1]
          {
            new Parameter("__gen_$_parm__", ParameterKind.Normal)
          }, (Statement) genExprFor);
          function.IsGenerator = true;
          function.SetLoc(this._globalParent, genExprFor.StartIndex, this.GetEnd());
          function.HeaderIndex = genExprFor.EndIndex;
          Expression list = genExprFor.List;
          NameExpression nameExpression = new NameExpression("__gen_$_parm__");
          nameExpression.SetLoc(this._globalParent, list.IndexSpan);
          genExprFor.List = (Expression) nameExpression;
          GeneratorExpression generatorExpression = new GeneratorExpression(function, list);
          generatorExpression.SetLoc(this._globalParent, expr.StartIndex, this.GetEnd());
          return (Expression) generatorExpression;
        }
      }
      current = Parser.NestGenExpr(current, (Statement) this.ParseGenExprFor());
    }
  }

  private static Statement NestGenExpr(Statement current, Statement nested)
  {
    switch (current)
    {
      case ForStatement forStatement:
        forStatement.Body = nested;
        break;
      case IfStatement ifStatement:
        ifStatement.Tests[0].Body = nested;
        break;
    }
    return nested;
  }

  private ForStatement ParseGenExprFor()
  {
    int start = this.GetStart();
    this.Eat(TokenKind.KeywordFor);
    bool trailingComma;
    Expression left = this.MakeTupleOrExpr(this.ParseTargetList(out trailingComma), trailingComma);
    this.Eat(TokenKind.KeywordIn);
    Expression orTest = this.ParseOrTest();
    ForStatement genExprFor = new ForStatement(left, orTest, (Statement) null, (Statement) null);
    int end = this.GetEnd();
    genExprFor.SetLoc(this._globalParent, start, end);
    genExprFor.HeaderIndex = end;
    return genExprFor;
  }

  private IfStatement ParseGenExprIf()
  {
    int start = this.GetStart();
    this.Eat(TokenKind.KeywordIf);
    IfStatementTest ifStatementTest = new IfStatementTest(this.ParseOldExpression(), (Statement) null);
    int end = this.GetEnd();
    ifStatementTest.HeaderIndex = end;
    ifStatementTest.SetLoc(this._globalParent, start, end);
    IfStatement genExprIf = new IfStatement(new IfStatementTest[1]
    {
      ifStatementTest
    }, (Statement) null);
    genExprIf.SetLoc(this._globalParent, start, end);
    return genExprIf;
  }

  private Expression FinishDictOrSetValue()
  {
    int start1 = this.GetStart();
    int end1 = this.GetEnd();
    List<SliceExpression> sliceExpressionList = (List<SliceExpression>) null;
    List<Expression> expressionList = (List<Expression>) null;
    bool allowIncomplete = this._allowIncomplete;
    try
    {
      this._allowIncomplete = true;
      while (!this.MaybeEat(TokenKind.RightBrace))
      {
        bool flag = false;
        Expression expression1 = this.ParseExpression();
        if (this.MaybeEat(TokenKind.Colon))
        {
          if (expressionList != null)
            this.ReportSyntaxError("invalid syntax");
          else if (sliceExpressionList == null)
          {
            sliceExpressionList = new List<SliceExpression>();
            flag = true;
          }
          Expression expression2 = this.ParseExpression();
          if (this.PeekToken(Tokens.KeywordForToken))
          {
            if (!flag)
              this.ReportSyntaxError("invalid syntax");
            return (Expression) this.FinishDictComp(expression1, expression2, start1, end1);
          }
          SliceExpression sliceExpression = new SliceExpression(expression1, expression2, (Expression) null, false);
          sliceExpression.SetLoc(this._globalParent, expression1.StartIndex, expression2.EndIndex);
          sliceExpressionList.Add(sliceExpression);
        }
        else
        {
          if (sliceExpressionList != null)
            this.ReportSyntaxError("invalid syntax");
          else if (expressionList == null)
          {
            expressionList = new List<Expression>();
            flag = true;
          }
          if (this.PeekToken(Tokens.KeywordForToken))
          {
            if (!flag)
              this.ReportSyntaxError("invalid syntax");
            return (Expression) this.FinishSetComp(expression1, start1, end1);
          }
          expressionList?.Add(expression1);
        }
        if (!this.MaybeEat(TokenKind.Comma))
        {
          this.Eat(TokenKind.RightBrace);
          break;
        }
      }
    }
    finally
    {
      this._allowIncomplete = allowIncomplete;
    }
    int start2 = this.GetStart();
    int end2 = this.GetEnd();
    if (this._sink != null)
      this._sink.MatchPair(new SourceSpan(this._tokenizer.IndexToLocation(start1), this._tokenizer.IndexToLocation(end1)), new SourceSpan(this._tokenizer.IndexToLocation(start2), this._tokenizer.IndexToLocation(end2)), 1);
    if (sliceExpressionList != null || expressionList == null)
    {
      DictionaryExpression dictionaryExpression = new DictionaryExpression(sliceExpressionList == null ? new SliceExpression[0] : sliceExpressionList.ToArray());
      dictionaryExpression.SetLoc(this._globalParent, start1, end2);
      return (Expression) dictionaryExpression;
    }
    SetExpression setExpression = new SetExpression(expressionList.ToArray());
    setExpression.SetLoc(this._globalParent, start1, end2);
    return (Expression) setExpression;
  }

  private SetComprehension FinishSetComp(Expression item, int oStart, int oEnd)
  {
    ComprehensionIterator[] compIter = this.ParseCompIter();
    this.Eat(TokenKind.RightBrace);
    int start = this.GetStart();
    int end = this.GetEnd();
    if (this._sink != null)
      this._sink.MatchPair(new SourceSpan(this._tokenizer.IndexToLocation(oStart), this._tokenizer.IndexToLocation(oEnd)), new SourceSpan(this._tokenizer.IndexToLocation(start), this._tokenizer.IndexToLocation(end)), 1);
    SetComprehension setComprehension = new SetComprehension(item, compIter);
    setComprehension.SetLoc(this._globalParent, oStart, end);
    return setComprehension;
  }

  private DictionaryComprehension FinishDictComp(
    Expression key,
    Expression value,
    int oStart,
    int oEnd)
  {
    ComprehensionIterator[] compIter = this.ParseCompIter();
    this.Eat(TokenKind.RightBrace);
    int start = this.GetStart();
    int end = this.GetEnd();
    if (this._sink != null)
      this._sink.MatchPair(new SourceSpan(this._tokenizer.IndexToLocation(oStart), this._tokenizer.IndexToLocation(oEnd)), new SourceSpan(this._tokenizer.IndexToLocation(start), this._tokenizer.IndexToLocation(end)), 1);
    DictionaryComprehension dictionaryComprehension = new DictionaryComprehension(key, value, compIter);
    dictionaryComprehension.SetLoc(this._globalParent, oStart, end);
    return dictionaryComprehension;
  }

  private ComprehensionIterator[] ParseCompIter()
  {
    List<ComprehensionIterator> comprehensionIteratorList = new List<ComprehensionIterator>();
    comprehensionIteratorList.Add((ComprehensionIterator) this.ParseCompFor());
    while (true)
    {
      while (!this.PeekToken(Tokens.KeywordForToken))
      {
        if (!this.PeekToken(Tokens.KeywordIfToken))
          return comprehensionIteratorList.ToArray();
        comprehensionIteratorList.Add((ComprehensionIterator) this.ParseCompIf());
      }
      comprehensionIteratorList.Add((ComprehensionIterator) this.ParseCompFor());
    }
  }

  private ComprehensionFor ParseCompFor()
  {
    this.Eat(TokenKind.KeywordFor);
    int start = this.GetStart();
    bool trailingComma;
    Expression lhs = this.MakeTupleOrExpr(this.ParseTargetList(out trailingComma), trailingComma);
    this.Eat(TokenKind.KeywordIn);
    Expression orTest = this.ParseOrTest();
    ComprehensionFor compFor = new ComprehensionFor(lhs, orTest);
    compFor.SetLoc(this._globalParent, start, this.GetEnd());
    return compFor;
  }

  private Expression FinishListValue()
  {
    int start1 = this.GetStart();
    int end1 = this.GetEnd();
    int groupingLevel = this._tokenizer.GroupingLevel;
    Expression expression1;
    if (this.MaybeEat(TokenKind.RightBracket))
    {
      expression1 = (Expression) new ListExpression(new Expression[0]);
    }
    else
    {
      bool allowIncomplete = this._allowIncomplete;
      try
      {
        this._allowIncomplete = true;
        Expression expression2 = this.ParseExpression();
        if (this.MaybeEat(TokenKind.Comma))
        {
          List<Expression> testList = this.ParseTestList();
          this.Eat(TokenKind.RightBracket);
          testList.Insert(0, expression2);
          expression1 = (Expression) new ListExpression(testList.ToArray());
        }
        else if (this.PeekToken(Tokens.KeywordForToken))
        {
          expression1 = (Expression) this.FinishListComp(expression2);
        }
        else
        {
          this.Eat(TokenKind.RightBracket);
          expression1 = (Expression) new ListExpression(new Expression[1]
          {
            expression2
          });
        }
      }
      finally
      {
        this._allowIncomplete = allowIncomplete;
      }
    }
    int start2 = this.GetStart();
    int end2 = this.GetEnd();
    if (this._sink != null)
      this._sink.MatchPair(new SourceSpan(this._tokenizer.IndexToLocation(start1), this._tokenizer.IndexToLocation(end1)), new SourceSpan(this._tokenizer.IndexToLocation(start2), this._tokenizer.IndexToLocation(end2)), groupingLevel);
    expression1.SetLoc(this._globalParent, start1, end2);
    return expression1;
  }

  private ListComprehension FinishListComp(Expression item)
  {
    ComprehensionIterator[] listCompIter = this.ParseListCompIter();
    this.Eat(TokenKind.RightBracket);
    return new ListComprehension(item, listCompIter);
  }

  private ComprehensionIterator[] ParseListCompIter()
  {
    List<ComprehensionIterator> comprehensionIteratorList = new List<ComprehensionIterator>();
    comprehensionIteratorList.Add((ComprehensionIterator) this.ParseListCompFor());
    while (true)
    {
      while (!this.PeekToken(Tokens.KeywordForToken))
      {
        if (!this.PeekToken(Tokens.KeywordIfToken))
          return comprehensionIteratorList.ToArray();
        comprehensionIteratorList.Add((ComprehensionIterator) this.ParseCompIf());
      }
      comprehensionIteratorList.Add((ComprehensionIterator) this.ParseListCompFor());
    }
  }

  private ComprehensionFor ParseListCompFor()
  {
    this.Eat(TokenKind.KeywordFor);
    int start = this.GetStart();
    bool trailingComma;
    Expression lhs = this.MakeTupleOrExpr(this.ParseTargetList(out trailingComma), trailingComma);
    this.Eat(TokenKind.KeywordIn);
    Expression expressionListAsExpr = this.ParseOldExpressionListAsExpr();
    ComprehensionFor listCompFor = new ComprehensionFor(lhs, expressionListAsExpr);
    listCompFor.SetLoc(this._globalParent, start, this.GetEnd());
    return listCompFor;
  }

  private ComprehensionIf ParseCompIf()
  {
    this.Eat(TokenKind.KeywordIf);
    int start = this.GetStart();
    ComprehensionIf compIf = new ComprehensionIf(this.ParseOldExpression());
    compIf.SetLoc(this._globalParent, start, this.GetEnd());
    return compIf;
  }

  private Expression FinishStringConversion()
  {
    int start = this.GetStart();
    Expression testListAsExpr = this.ParseTestListAsExpr();
    this.Eat(TokenKind.BackQuote);
    BackQuoteExpression backQuoteExpression = new BackQuoteExpression(testListAsExpr);
    backQuoteExpression.SetLoc(this._globalParent, start, this.GetEnd());
    return (Expression) backQuoteExpression;
  }

  private Expression MakeTupleOrExpr(List<Expression> l, bool trailingComma)
  {
    return this.MakeTupleOrExpr(l, trailingComma, false);
  }

  private Expression MakeTupleOrExpr(List<Expression> l, bool trailingComma, bool expandable)
  {
    if (l.Count == 1 && !trailingComma)
      return l[0];
    Expression[] array = l.ToArray();
    TupleExpression tupleExpression = new TupleExpression(expandable && !trailingComma, array);
    if (array.Length != 0)
      tupleExpression.SetLoc(this._globalParent, array[0].StartIndex, array[array.Length - 1].EndIndex);
    return (Expression) tupleExpression;
  }

  private static bool NeverTestToken(Token t)
  {
    switch (t.Kind)
    {
      case TokenKind.EndOfFile:
      case TokenKind.NewLine:
      case TokenKind.Indent:
      case TokenKind.Dedent:
      case TokenKind.AddEqual:
      case TokenKind.SubtractEqual:
      case TokenKind.PowerEqual:
      case TokenKind.MultiplyEqual:
      case TokenKind.FloorDivideEqual:
      case TokenKind.DivideEqual:
      case TokenKind.ModEqual:
      case TokenKind.LeftShiftEqual:
      case TokenKind.RightShiftEqual:
      case TokenKind.BitwiseAndEqual:
      case TokenKind.BitwiseOrEqual:
      case TokenKind.ExclusiveOrEqual:
      case TokenKind.RightParenthesis:
      case TokenKind.RightBracket:
      case TokenKind.RightBrace:
      case TokenKind.Comma:
      case TokenKind.Semicolon:
      case TokenKind.Assign:
      case TokenKind.KeywordFor:
      case TokenKind.KeywordIf:
      case TokenKind.KeywordIn:
        return true;
      default:
        return false;
    }
  }

  private FunctionDefinition CurrentFunction
  {
    get
    {
      return this._functions != null && this._functions.Count > 0 ? this._functions.Peek() : (FunctionDefinition) null;
    }
  }

  private FunctionDefinition PopFunction()
  {
    return this._functions != null && this._functions.Count > 0 ? this._functions.Pop() : (FunctionDefinition) null;
  }

  private void PushFunction(FunctionDefinition function)
  {
    if (this._functions == null)
      this._functions = new Stack<FunctionDefinition>();
    this._functions.Push(function);
  }

  private CallExpression FinishCallExpr(Expression target, params Arg[] args)
  {
    bool flag1 = false;
    bool flag2 = false;
    int num1 = 0;
    int num2 = 0;
    foreach (Arg obj in args)
    {
      if (obj.Name == null)
      {
        if (flag1 | flag2 || num1 > 0)
          this.ReportSyntaxError(Resources.NonKeywordAfterKeywordArg);
      }
      else if (obj.Name == "*")
      {
        if (flag1 | flag2)
          this.ReportSyntaxError(Resources.OneListArgOnly);
        flag1 = true;
        ++num2;
      }
      else if (obj.Name == "**")
      {
        if (flag2)
          this.ReportSyntaxError(Resources.OneKeywordArgOnly);
        flag2 = true;
        ++num2;
      }
      else
      {
        if (flag2)
          this.ReportSyntaxError(Resources.KeywordOutOfSequence);
        ++num1;
      }
    }
    return new CallExpression(target, args);
  }

  public void Dispose()
  {
    if (this._sourceReader == null)
      return;
    this._sourceReader.Dispose();
  }

  private PythonAst ParseFileWorker(bool makeModule, bool returnValue)
  {
    this._globalParent = new PythonAst(makeModule, this._languageFeatures, false, this._context);
    this.StartParsing();
    List<Statement> statementList = new List<Statement>();
    this.MaybeEatNewLine();
    if (this.PeekToken(TokenKind.Constant))
    {
      Statement stmt = this.ParseStmt();
      statementList.Add(stmt);
      this._fromFutureAllowed = false;
      if (stmt is ExpressionStatement expressionStatement && expressionStatement.Expression is ConstantExpression expression && expression.Value is string)
        this._fromFutureAllowed = true;
    }
    this.MaybeEatNewLine();
    if (this._fromFutureAllowed)
    {
      while (this.PeekToken(Tokens.KeywordFromToken))
      {
        Statement stmt = this.ParseStmt();
        statementList.Add(stmt);
        if (stmt is FromImportStatement fromImportStatement && !fromImportStatement.IsFromFuture)
          break;
      }
    }
    this._fromFutureAllowed = false;
    while (!this.MaybeEat(TokenKind.EndOfFile))
    {
      if (!this.MaybeEatNewLine())
      {
        Statement stmt = this.ParseStmt();
        statementList.Add(stmt);
      }
    }
    Statement[] array = statementList.ToArray();
    if (returnValue && array.Length != 0 && array[array.Length - 1] is ExpressionStatement expressionStatement1)
    {
      ReturnStatement returnStatement = new ReturnStatement(expressionStatement1.Expression);
      array[array.Length - 1] = (Statement) returnStatement;
      returnStatement.SetLoc(this._globalParent, expressionStatement1.Expression.IndexSpan);
    }
    SuiteStatement ret = new SuiteStatement(array);
    ret.SetLoc(this._globalParent, 0, this.GetEnd());
    return this.FinishParsing((Statement) ret);
  }

  private Statement InternalParseInteractiveInput(
    out bool parsingMultiLineCmpdStmt,
    out bool isEmptyStmt)
  {
    try
    {
      isEmptyStmt = false;
      parsingMultiLineCmpdStmt = false;
      Statement interactiveInput;
      switch (this.PeekToken().Kind)
      {
        case TokenKind.NewLine:
          this.MaybeEatNewLine();
          this.Eat(TokenKind.EndOfFile);
          if (this._tokenizer.EndContinues)
          {
            parsingMultiLineCmpdStmt = true;
            this._errorCode = 1;
          }
          else
            isEmptyStmt = true;
          return (Statement) null;
        case TokenKind.At:
        case TokenKind.KeywordClass:
        case TokenKind.KeywordDef:
        case TokenKind.KeywordFor:
        case TokenKind.KeywordIf:
        case TokenKind.KeywordTry:
        case TokenKind.KeywordWhile:
        case TokenKind.LastKeyword:
          parsingMultiLineCmpdStmt = true;
          interactiveInput = this.ParseStmt();
          this.EatEndOfInput();
          break;
        default:
          interactiveInput = this.ParseSimpleStmt();
          this.MaybeEatNewLine();
          this.Eat(TokenKind.EndOfFile);
          break;
      }
      return interactiveInput;
    }
    catch (BadSourceException ex)
    {
      throw this.BadSourceError(ex);
    }
  }

  private Expression ParseTestListAsExpression()
  {
    this.StartParsing();
    Expression testListAsExpr = this.ParseTestListAsExpr();
    this.EatEndOfInput();
    return testListAsExpr;
  }

  private bool MaybeEatNewLine()
  {
    if (!this.MaybeEat(TokenKind.NewLine))
      return false;
    do
      ;
    while (this.MaybeEat(TokenKind.NLToken));
    return true;
  }

  private bool EatNewLine()
  {
    bool flag = this.Eat(TokenKind.NewLine);
    do
      ;
    while (this.MaybeEat(TokenKind.NLToken));
    return flag;
  }

  private Token EatEndOfInput()
  {
    do
      ;
    while (this.MaybeEatNewLine() || this.MaybeEat(TokenKind.Dedent));
    Token token = this.NextToken();
    if (token.Kind == TokenKind.EndOfFile)
      return token;
    this.ReportSyntaxError(this._token);
    return token;
  }

  private Exception BadSourceError(BadSourceException bse)
  {
    return this._sourceReader.BaseReader is StreamReader baseReader && baseReader.BaseStream.CanSeek ? PythonContext.ReportEncodingError(baseReader.BaseStream, this._sourceUnit.Path) : (Exception) PythonOps.BadSourceError(bse._badByte, new SourceSpan(this._tokenizer.CurrentPosition, this._tokenizer.CurrentPosition), this._sourceUnit.Path);
  }

  private bool TrueDivision
  {
    get => (this._languageFeatures & ModuleOptions.TrueDivision) == ModuleOptions.TrueDivision;
  }

  private bool AbsoluteImports
  {
    get
    {
      return (this._languageFeatures & ModuleOptions.AbsoluteImports) == ModuleOptions.AbsoluteImports;
    }
  }

  private void StartParsing()
  {
    this._parsingStarted = !this._parsingStarted ? true : throw new InvalidOperationException("Parsing already started. Use Restart to start again.");
    this.FetchLookahead();
  }

  private int GetEnd() => this._token.Span.End;

  private int GetStart() => this._token.Span.Start;

  private SourceSpan GetSourceSpan()
  {
    return new SourceSpan(this._tokenizer.IndexToLocation(this.GetStart()), this._tokenizer.IndexToLocation(this.GetEnd()));
  }

  private Token NextToken()
  {
    this._token = this._lookahead;
    this.FetchLookahead();
    return this._token.Token;
  }

  private Token PeekToken() => this._lookahead.Token;

  private void FetchLookahead()
  {
    this._lookahead = new TokenWithSpan(this._tokenizer.GetNextToken(), this._tokenizer.TokenSpan);
  }

  private bool PeekToken(TokenKind kind) => this.PeekToken().Kind == kind;

  private bool PeekToken(Token check) => this.PeekToken() == check;

  private bool Eat(TokenKind kind)
  {
    if (this.PeekToken().Kind != kind)
    {
      this.ReportSyntaxError(this._lookahead);
      return false;
    }
    this.NextToken();
    return true;
  }

  private bool EatNoEof(TokenKind kind)
  {
    if (this.PeekToken().Kind != kind)
    {
      this.ReportSyntaxError(this._lookahead.Token, this._lookahead.Span, 16 /*0x10*/, false);
      return false;
    }
    this.NextToken();
    return true;
  }

  private bool MaybeEat(TokenKind kind)
  {
    if (this.PeekToken().Kind != kind)
      return false;
    this.NextToken();
    return true;
  }

  private struct WithItem(int start, Expression contextManager, Expression variable)
  {
    public readonly int Start = start;
    public readonly Expression ContextManager = contextManager;
    public readonly Expression Variable = variable;
  }

  private class TokenizerErrorSink : ErrorSink
  {
    private readonly Parser _parser;

    public TokenizerErrorSink(Parser parser) => this._parser = parser;

    public override void Add(
      SourceUnit sourceUnit,
      string message,
      SourceSpan span,
      int errorCode,
      Severity severity)
    {
      if (this._parser._errorCode == 0 && (severity == Severity.Error || severity == Severity.FatalError))
        this._parser._errorCode = errorCode;
      this._parser.ErrorSink.Add(sourceUnit, message, span, errorCode, severity);
    }
  }
}
