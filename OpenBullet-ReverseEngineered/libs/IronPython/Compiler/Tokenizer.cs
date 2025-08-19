// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Tokenizer
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;

#nullable disable
namespace IronPython.Compiler;

public sealed class Tokenizer : TokenizerService
{
  private Tokenizer.State _state;
  private readonly bool _verbatim;
  internal bool _dontImplyDedent;
  private bool _disableLineFeedLineSeparator;
  private SourceUnit _sourceUnit;
  private ErrorSink _errors;
  private Severity _indentationInconsistencySeverity;
  private bool _endContinues;
  private bool _printFunction;
  private bool _unicodeLiterals;
  private List<int> _newLineLocations;
  private SourceLocation _initialLocation;
  private TextReader _reader;
  private char[] _buffer;
  private bool _multiEolns;
  private int _position;
  private int _end;
  private int _tokenEnd;
  private int _start;
  private int _tokenStartIndex;
  private int _tokenEndIndex;
  private bool _bufferResized;
  private const int EOF = -1;
  private const int MaxIndent = 80 /*0x50*/;
  private const int DefaultBufferCapacity = 1024 /*0x0400*/;
  private Dictionary<object, NameToken> _names;
  private static object _currentName = new object();

  public Tokenizer()
  {
    this._errors = ErrorSink.Null;
    this._verbatim = true;
    this._state = new Tokenizer.State((object) null);
    this._names = new Dictionary<object, NameToken>(128 /*0x80*/, (IEqualityComparer<object>) new Tokenizer.TokenEqualityComparer(this));
  }

  public Tokenizer(ErrorSink errorSink)
  {
    this._errors = errorSink;
    this._state = new Tokenizer.State((object) null);
    this._names = new Dictionary<object, NameToken>(128 /*0x80*/, (IEqualityComparer<object>) new Tokenizer.TokenEqualityComparer(this));
  }

  public Tokenizer(ErrorSink errorSink, PythonCompilerOptions options)
  {
    ContractUtils.RequiresNotNull((object) errorSink, nameof (errorSink));
    ContractUtils.RequiresNotNull((object) options, nameof (options));
    this._errors = errorSink;
    this._verbatim = options.Verbatim;
    this._state = new Tokenizer.State((object) null);
    this._dontImplyDedent = options.DontImplyDedent;
    this._printFunction = options.PrintFunction;
    this._unicodeLiterals = options.UnicodeLiterals;
    this._names = new Dictionary<object, NameToken>(128 /*0x80*/, (IEqualityComparer<object>) new Tokenizer.TokenEqualityComparer(this));
  }

  internal Tokenizer(ErrorSink errorSink, PythonCompilerOptions options, bool verbatim)
    : this(errorSink, options)
  {
    this._verbatim = verbatim || options.Verbatim;
  }

  public override bool IsRestartable => true;

  public override object CurrentState => (object) this._state;

  public override SourceLocation CurrentPosition => this.IndexToLocation(this.CurrentIndex);

  public SourceLocation IndexToLocation(int index)
  {
    int index1 = this._newLineLocations.BinarySearch(index);
    if (index1 < 0)
    {
      if (index1 == -1)
        return new SourceLocation(index + this._initialLocation.Index, this._initialLocation.Line, checked (index + this._initialLocation.Column));
      index1 = ~index1 - 1;
    }
    return new SourceLocation(index + this._initialLocation.Index, this._sourceUnit.MapLine(index1 + 2) + this._initialLocation.Line - 1, index - this._newLineLocations[index1] + this._initialLocation.Column);
  }

  public SourceUnit SourceUnit => this._sourceUnit;

  public override ErrorSink ErrorSink
  {
    get => this._errors;
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      this._errors = value;
    }
  }

  public Severity IndentationInconsistencySeverity
  {
    get => this._indentationInconsistencySeverity;
    set
    {
      this._indentationInconsistencySeverity = value;
      if (value == Severity.Ignore || this._state.IndentFormat != null)
        return;
      this._state.IndentFormat = new StringBuilder[80 /*0x50*/];
    }
  }

  public bool IsEndOfFile => this.Peek() == -1;

  public IndexSpan TokenSpan
  {
    get => new IndexSpan(this._tokenStartIndex, this._tokenEndIndex - this._tokenStartIndex);
  }

  public void Initialize(SourceUnit sourceUnit)
  {
    ContractUtils.RequiresNotNull((object) sourceUnit, nameof (sourceUnit));
    this.Initialize((object) null, (TextReader) sourceUnit.GetReader(), sourceUnit, SourceLocation.MinValue, 1024 /*0x0400*/);
  }

  public override void Initialize(
    object state,
    TextReader reader,
    SourceUnit sourceUnit,
    SourceLocation initialLocation)
  {
    this.Initialize(state, reader, sourceUnit, initialLocation, 1024 /*0x0400*/);
  }

  public void Initialize(
    object state,
    TextReader reader,
    SourceUnit sourceUnit,
    SourceLocation initialLocation,
    int bufferCapacity)
  {
    this.Initialize(state, reader, sourceUnit, initialLocation, bufferCapacity, (PythonCompilerOptions) null);
  }

  public void Initialize(
    object state,
    TextReader reader,
    SourceUnit sourceUnit,
    SourceLocation initialLocation,
    int bufferCapacity,
    PythonCompilerOptions compilerOptions)
  {
    ContractUtils.RequiresNotNull((object) reader, nameof (reader));
    if (state != null)
      this._state = state is Tokenizer.State state1 ? new Tokenizer.State(state1) : throw new ValueErrorException("bad state provided");
    else
      this._state = new Tokenizer.State((object) null);
    if (compilerOptions != null && compilerOptions.InitialIndent != null)
      this._state.Indent = (int[]) compilerOptions.InitialIndent.Clone();
    this._sourceUnit = sourceUnit;
    this._disableLineFeedLineSeparator = reader is NoLineFeedSourceContentProvider.Reader;
    this._reader = reader;
    if (this._buffer == null || this._buffer.Length < bufferCapacity)
      this._buffer = new char[bufferCapacity];
    this._newLineLocations = new List<int>();
    this._tokenEnd = -1;
    this._multiEolns = !this._disableLineFeedLineSeparator;
    this._initialLocation = initialLocation;
    this._tokenEndIndex = -1;
    this._tokenStartIndex = 0;
    this._start = this._end = 0;
    this._position = 0;
  }

  public override TokenInfo ReadToken()
  {
    if (this._buffer == null)
      throw new InvalidOperationException("Uninitialized");
    Token nextToken = this.GetNextToken();
    SourceSpan span;
    ref SourceSpan local = ref span;
    IndexSpan tokenSpan = this.TokenSpan;
    SourceLocation location1 = this.IndexToLocation(tokenSpan.Start);
    tokenSpan = this.TokenSpan;
    SourceLocation location2 = this.IndexToLocation(tokenSpan.End);
    local = new SourceSpan(location1, location2);
    TokenTriggers trigger = TokenTriggers.None;
    TokenCategory category;
    switch (nextToken.Kind)
    {
      case TokenKind.EndOfFile:
        category = TokenCategory.EndOfStream;
        break;
      case TokenKind.Error:
        category = !(nextToken is IncompleteStringErrorToken) ? TokenCategory.Error : TokenCategory.StringLiteral;
        break;
      case TokenKind.NewLine:
        category = TokenCategory.WhiteSpace;
        break;
      case TokenKind.Comment:
        category = TokenCategory.Comment;
        break;
      case TokenKind.Name:
        category = TokenCategory.Identifier;
        break;
      case TokenKind.Constant:
        category = nextToken.Value is string ? TokenCategory.StringLiteral : TokenCategory.NumericLiteral;
        break;
      case TokenKind.Dot:
        category = TokenCategory.Operator;
        trigger = TokenTriggers.MemberSelect;
        break;
      case TokenKind.LeftParenthesis:
        category = TokenCategory.Grouping;
        trigger = TokenTriggers.MatchBraces | TokenTriggers.ParameterStart;
        break;
      case TokenKind.RightParenthesis:
        category = TokenCategory.Grouping;
        trigger = TokenTriggers.MatchBraces | TokenTriggers.ParameterEnd;
        break;
      case TokenKind.LeftBracket:
      case TokenKind.RightBracket:
      case TokenKind.LeftBrace:
      case TokenKind.RightBrace:
        category = TokenCategory.Grouping;
        trigger = TokenTriggers.MatchBraces;
        break;
      case TokenKind.Comma:
        category = TokenCategory.Delimiter;
        trigger = TokenTriggers.ParameterNext;
        break;
      case TokenKind.Colon:
        category = TokenCategory.Delimiter;
        break;
      case TokenKind.Semicolon:
        category = TokenCategory.Delimiter;
        break;
      default:
        category = nextToken.Kind < TokenKind.FirstKeyword || nextToken.Kind > TokenKind.LastKeyword ? TokenCategory.Operator : TokenCategory.Keyword;
        break;
    }
    return new TokenInfo(span, category, trigger);
  }

  internal bool TryGetTokenString(int len, out string tokenString)
  {
    if (len != this.TokenLength)
    {
      tokenString = (string) null;
      return false;
    }
    tokenString = this.GetTokenString();
    return true;
  }

  internal bool PrintFunction
  {
    get => this._printFunction;
    set => this._printFunction = value;
  }

  internal bool UnicodeLiterals
  {
    get => this._unicodeLiterals;
    set => this._unicodeLiterals = value;
  }

  public Token GetNextToken()
  {
    Token nextToken;
    if (this._state.PendingDedents != 0)
    {
      if (this._state.PendingDedents == -1)
      {
        this._state.PendingDedents = 0;
        nextToken = Tokens.IndentToken;
      }
      else
      {
        --this._state.PendingDedents;
        nextToken = Tokens.DedentToken;
      }
    }
    else
      nextToken = this.Next();
    return nextToken;
  }

  private Token Next()
  {
    bool atBeginning = this.AtBeginning;
    if (this._state.IncompleteString != (Tokenizer.IncompleteString) null && this.Peek() != -1)
    {
      Tokenizer.IncompleteString incompleteString = this._state.IncompleteString;
      this._state.IncompleteString = (Tokenizer.IncompleteString) null;
      return this.ContinueString(incompleteString.IsSingleTickQuote ? '\'' : '"', incompleteString.IsRaw, incompleteString.IsUnicode, false, incompleteString.IsTripleQuoted, 0);
    }
    this.DiscardToken();
    int num = this.NextChar();
    while (true)
    {
      while (num > 82)
      {
        if (num <= 95)
        {
          if (num != 85)
          {
            if (num != 92)
            {
              if (num == 95)
              {
                this._state.LastNewLine = false;
                return this.ReadName();
              }
              goto label_37;
            }
            if (this.ReadEolnOpt(this.NextChar()) > 0)
            {
              this._newLineLocations.Add(this.CurrentIndex);
              this.DiscardToken();
              num = this.NextChar();
              if (num == -1)
              {
                this._endContinues = true;
                continue;
              }
              continue;
            }
            this.BufferBack();
            goto label_37;
          }
        }
        else if (num != 98)
        {
          if (num != 114)
          {
            if (num != 117)
              goto label_37;
          }
          else
            goto label_30;
        }
        else
          goto label_31;
        this._state.LastNewLine = false;
        return this.ReadNameOrUnicodeString();
      }
      if (num <= 12)
      {
        if (num != -1)
        {
          if (num != 9)
          {
            if (num == 12)
            {
              this.DiscardToken();
              num = this.NextChar();
              continue;
            }
            goto label_37;
          }
        }
        else
          break;
      }
      else
      {
        switch (num - 32 /*0x20*/)
        {
          case 0:
            break;
          case 1:
          case 4:
          case 5:
          case 6:
          case 8:
          case 9:
          case 10:
          case 11:
          case 12:
          case 13:
          case 15:
            goto label_37;
          case 2:
          case 7:
            goto label_28;
          case 3:
            if (!this._verbatim)
            {
              num = this.SkipSingleLineComment();
              continue;
            }
            goto label_22;
          case 14:
            goto label_33;
          case 16 /*0x10*/:
          case 17:
          case 18:
          case 19:
          case 20:
          case 21:
          case 22:
          case 23:
          case 24:
          case 25:
            goto label_36;
          default:
            if (num != 66)
            {
              if (num == 82)
                goto label_30;
              goto label_37;
            }
            goto label_31;
        }
      }
      num = this.SkipWhiteSpace(atBeginning);
      continue;
label_37:
      if (this.ReadEolnOpt(num) > 0)
      {
        this._newLineLocations.Add(this.CurrentIndex);
        if (!this.ReadNewline())
        {
          this.DiscardToken();
          num = this.NextChar();
        }
        else
          goto label_39;
      }
      else
        goto label_43;
    }
    return this.ReadEof();
label_22:
    return this.ReadSingleLineComment();
label_28:
    this._state.LastNewLine = false;
    return this.ReadString((char) num, false, false, false);
label_30:
    this._state.LastNewLine = false;
    return this.ReadNameOrRawString();
label_31:
    this._state.LastNewLine = false;
    return this.ReadNameOrBytes();
label_33:
    this._state.LastNewLine = false;
    switch (this.Peek())
    {
      case 48 /*0x30*/:
      case 49:
      case 50:
      case 51:
      case 52:
      case 53:
      case 54:
      case 55:
      case 56:
      case 57:
        return this.ReadFraction();
      default:
        this.MarkTokenEnd();
        return Tokens.DotToken;
    }
label_36:
    this._state.LastNewLine = false;
    return this.ReadNumber(num);
label_39:
    if (this._state.LastNewLine)
      return Tokens.NLToken;
    this._state.LastNewLine = true;
    return Tokens.NewLineToken;
label_43:
    this._state.LastNewLine = false;
    Token token = this.NextOperator(num);
    if (token != null)
    {
      this.MarkTokenEnd();
      return token;
    }
    if (Tokenizer.IsNameStart(num))
      return this.ReadName();
    this.MarkTokenEnd();
    return (Token) Tokenizer.BadChar(num);
  }

  private int SkipWhiteSpace(bool atBeginning)
  {
    int current;
    do
    {
      current = this.NextChar();
    }
    while (current == 32 /*0x20*/ || current == 9);
    this.BufferBack();
    if (atBeginning && current != 35 && current != 12 && current != -1 && !this.IsEoln(current))
    {
      this.MarkTokenEnd();
      this.ReportSyntaxError(this.BufferTokenSpan, Resources.InvalidSyntax, 16 /*0x10*/);
    }
    this.DiscardToken();
    this.SeekRelative(1);
    return current;
  }

  private int SkipSingleLineComment()
  {
    int num = this.ReadLine();
    this.MarkTokenEnd();
    this.DiscardToken();
    this.SeekRelative(1);
    return num;
  }

  private Token ReadSingleLineComment()
  {
    this.ReadLine();
    this.MarkTokenEnd();
    return (Token) new CommentToken(this.GetTokenString());
  }

  private Token ReadNameOrUnicodeString()
  {
    if (this.NextChar(34))
      return this.ReadString('"', false, true, false);
    if (this.NextChar(39))
      return this.ReadString('\'', false, true, false);
    if (this.NextChar(114) || this.NextChar(82))
    {
      if (this.NextChar(34))
        return this.ReadString('"', true, true, false);
      if (this.NextChar(39))
        return this.ReadString('\'', true, true, false);
      this.BufferBack();
    }
    return this.ReadName();
  }

  private Token ReadNameOrBytes()
  {
    if (this.NextChar(34))
      return this.ReadString('"', false, false, true);
    if (this.NextChar(39))
      return this.ReadString('\'', false, false, true);
    if (this.NextChar(114) || this.NextChar(82))
    {
      if (this.NextChar(34))
        return this.ReadString('"', true, false, true);
      if (this.NextChar(39))
        return this.ReadString('\'', true, false, true);
      this.BufferBack();
    }
    return this.ReadName();
  }

  private Token ReadNameOrRawString()
  {
    if (this.NextChar(34))
      return this.ReadString('"', true, false, false);
    return this.NextChar(39) ? this.ReadString('\'', true, false, false) : this.ReadName();
  }

  private Token ReadEof()
  {
    this.MarkTokenEnd();
    if (this._dontImplyDedent || this._state.IndentLevel <= 0)
      return Tokens.EndOfFileToken;
    if (!this._state.LastNewLine)
    {
      this._state.LastNewLine = true;
      return Tokens.NewLineToken;
    }
    this.SetIndent(0, (StringBuilder) null);
    --this._state.PendingDedents;
    return Tokens.DedentToken;
  }

  private static ErrorToken BadChar(int ch)
  {
    return new ErrorToken(StringUtils.AddSlashes(((char) ch).ToString()));
  }

  private static bool IsNameStart(int ch) => char.IsLetter((char) ch) || ch == 95;

  private static bool IsNamePart(int ch) => char.IsLetterOrDigit((char) ch) || ch == 95;

  private Token ReadString(char quote, bool isRaw, bool isUni, bool isBytes)
  {
    int num = 0;
    bool isTriple = false;
    int startAdd;
    if (this.NextChar((int) quote))
    {
      if (this.NextChar((int) quote))
      {
        isTriple = true;
        startAdd = num + 3;
      }
      else
      {
        this.BufferBack();
        startAdd = num + 1;
      }
    }
    else
      startAdd = num + 1;
    if (isRaw)
      ++startAdd;
    if (isUni)
      ++startAdd;
    if (isBytes)
      ++startAdd;
    return this.ContinueString(quote, isRaw, isUni, isBytes, isTriple, startAdd);
  }

  private Token ContinueString(
    char quote,
    bool isRaw,
    bool isUnicode,
    bool isBytes,
    bool isTriple,
    int startAdd)
  {
    int num1 = 0;
    int num2;
    int num3;
    do
    {
      int current1;
      do
      {
        current1 = this.NextChar();
        if (current1 == -1)
        {
          this.BufferBack();
          if (isTriple)
          {
            this.MarkTokenEnd();
            SourceLocation sourceLocation1;
            ref SourceLocation local = ref sourceLocation1;
            SourceLocation sourceLocation2 = this.BufferTokenEnd;
            int index = sourceLocation2.Index - 1;
            sourceLocation2 = this.BufferTokenEnd;
            int line = sourceLocation2.Line;
            sourceLocation2 = this.IndexToLocation(this._tokenStartIndex);
            int column = sourceLocation2.Column + this._tokenEndIndex - this._tokenStartIndex - 1;
            local = new SourceLocation(index, line, column);
            this.ReportSyntaxError(new SourceSpan(sourceLocation1, sourceLocation1), Resources.EofInTripleQuotedString, 18);
          }
          else
            this.MarkTokenEnd();
          this.UnexpectedEndOfString(isTriple, isTriple);
          string tokenSubstring = this.GetTokenSubstring(startAdd, this.TokenLength - startAdd - num1);
          this._state.IncompleteString = new Tokenizer.IncompleteString(quote == '\'', isRaw, isUnicode, isTriple);
          return (Token) new IncompleteStringErrorToken(Resources.EofInString, tokenSubstring);
        }
        if (current1 == (int) quote)
        {
          if (isTriple)
          {
            if (this.NextChar((int) quote) && this.NextChar((int) quote))
            {
              num2 = num1 + 3;
              goto label_22;
            }
          }
          else
          {
            num2 = num1 + 1;
            goto label_22;
          }
        }
        else if (current1 == 92)
        {
          int current2 = this.NextChar();
          if (current2 == -1)
          {
            this.BufferBack();
            this.MarkTokenEnd();
            this.UnexpectedEndOfString(isTriple, isTriple);
            string tokenSubstring = this.GetTokenSubstring(startAdd, this.TokenLength - startAdd - num1 - 1);
            this._state.IncompleteString = new Tokenizer.IncompleteString(quote == '\'', isRaw, isUnicode, isTriple);
            return (Token) new IncompleteStringErrorToken(Resources.EofInString, tokenSubstring);
          }
          int num4;
          if ((num4 = this.ReadEolnOpt(current2)) > 0)
          {
            this._newLineLocations.Add(this.CurrentIndex);
            if (this.Peek() == -1)
            {
              this.SeekRelative(-num4);
              this.MarkTokenEnd();
              string tokenSubstring = this.GetTokenSubstring(startAdd, this.TokenLength - startAdd - num1 - 1);
              this.UnexpectedEndOfString(isTriple, true);
              return (Token) new IncompleteStringErrorToken(Resources.EofInString, tokenSubstring);
            }
          }
          else if (current2 != (int) quote && current2 != 92)
            this.BufferBack();
        }
      }
      while ((num3 = this.ReadEolnOpt(current1)) <= 0);
      this._newLineLocations.Add(this.CurrentIndex);
    }
    while (isTriple);
    this.SeekRelative(-num3);
    this.MarkTokenEnd();
    this.UnexpectedEndOfString(isTriple, false);
    string tokenSubstring1 = this.GetTokenSubstring(startAdd, this.TokenLength - startAdd - num1);
    return (Token) new IncompleteStringErrorToken(quote == '"' ? Resources.NewLineInDoubleQuotedString : Resources.NewLineInSingleQuotedString, tokenSubstring1);
label_22:
    this.MarkTokenEnd();
    return this.MakeStringToken(quote, isRaw, isUnicode, isBytes, isTriple, this._start + startAdd, this.TokenLength - startAdd - num2);
  }

  private Token MakeStringToken(
    char quote,
    bool isRaw,
    bool isUnicode,
    bool isBytes,
    bool isTriple,
    int start,
    int length)
  {
    if (!isBytes)
    {
      string str = LiteralParser.ParseString(this._buffer, start, length, isRaw, isUnicode || this.UnicodeLiterals, !this._disableLineFeedLineSeparator);
      return isUnicode ? (Token) new UnicodeStringToken((object) str) : (Token) new ConstantValueToken((object) str);
    }
    List<byte> bytes = LiteralParser.ParseBytes(this._buffer, start, length, isRaw, !this._disableLineFeedLineSeparator);
    return bytes.Count == 0 ? (Token) new ConstantValueToken((object) Bytes.Empty) : (Token) new ConstantValueToken((object) new Bytes((IList<byte>) bytes));
  }

  private void UnexpectedEndOfString(bool isTriple, bool isIncomplete)
  {
    this.ReportSyntaxError(this.BufferTokenSpan, isTriple ? Resources.EofInTripleQuotedString : Resources.EolInSingleQuotedString, isIncomplete ? 18 : 16 /*0x10*/);
  }

  private Token ReadNumber(int start)
  {
    int num1 = 10;
    if (start == 48 /*0x30*/)
    {
      if (this.NextChar(120) || this.NextChar(88))
        return this.ReadHexNumber();
      if (this.NextChar(98) || this.NextChar(66))
        return this.ReadBinaryNumber();
      if (this.NextChar(111) || this.NextChar(79))
        return this.ReadOctalNumber();
      num1 = 8;
    }
    int num2;
    do
    {
      num2 = this.NextChar();
      if (num2 <= 74)
      {
        if (num2 <= 57)
        {
          if (num2 == 46)
            goto label_19;
        }
        else
          goto label_12;
      }
      else
        goto label_14;
    }
    while ((uint) (num2 - 48 /*0x30*/) <= 9U);
    goto label_25;
label_12:
    if (num2 != 69)
    {
      if (num2 == 74)
        goto label_23;
      goto label_25;
    }
    goto label_20;
label_14:
    if (num2 <= 101)
    {
      if (num2 != 76)
      {
        if (num2 == 101)
          goto label_20;
        goto label_25;
      }
    }
    else if (num2 != 106)
    {
      if (num2 != 108)
        goto label_25;
    }
    else
      goto label_23;
    this.MarkTokenEnd();
    return (Token) new ConstantValueToken((object) LiteralParser.ParseBigInteger(this.GetTokenString(), num1));
label_19:
    return this.ReadFraction();
label_20:
    Token token = this.ReadExponent();
    if (token != null)
      return token;
    this.BufferBack();
    this.MarkTokenEnd();
    return (Token) new ConstantValueToken(this.ParseInteger(this.GetTokenString(), num1));
label_23:
    this.MarkTokenEnd();
    return (Token) new ConstantValueToken((object) LiteralParser.ParseImaginary(this.GetTokenString()));
label_25:
    this.BufferBack();
    this.MarkTokenEnd();
    return (Token) new ConstantValueToken(this.ParseInteger(this.GetTokenString(), num1));
  }

  private Token ReadBinaryNumber()
  {
    int num1 = 0;
    int num2 = 0;
    bool flag1 = false;
    BigInteger bigInteger = BigInteger.Zero;
    bool flag2 = true;
    while (true)
    {
      int num3 = this.NextChar();
      switch (num3)
      {
        case 48 /*0x30*/:
          if (num2 == 0)
            break;
          goto case 49;
        case 49:
          ++num1;
          if (num1 == 32 /*0x20*/)
          {
            flag1 = true;
            bigInteger = (BigInteger) num2;
          }
          if (num1 >= 32 /*0x20*/)
          {
            bigInteger = bigInteger << 1 | (BigInteger) (num3 - 48 /*0x30*/);
            break;
          }
          num2 = num2 << 1 | num3 - 48 /*0x30*/;
          break;
        case 76:
        case 108:
          goto label_8;
        default:
          goto label_9;
      }
      flag2 = false;
    }
label_8:
    this.MarkTokenEnd();
    return (Token) new ConstantValueToken((object) (flag1 ? bigInteger : (BigInteger) num2));
label_9:
    this.BufferBack();
    this.MarkTokenEnd();
    if (flag2)
      this.ReportSyntaxError(new SourceSpan(new SourceLocation(this._tokenEndIndex, this.IndexToLocation(this._tokenEndIndex).Line, this.IndexToLocation(this._tokenEndIndex).Column - 1), this.BufferTokenEnd), Resources.InvalidToken, 16 /*0x10*/);
    return (Token) new ConstantValueToken(flag1 ? (object) bigInteger : (object) num2);
  }

  private Token ReadOctalNumber()
  {
    bool flag = true;
    while (true)
    {
      switch (this.NextChar())
      {
        case 48 /*0x30*/:
        case 49:
        case 50:
        case 51:
        case 52:
        case 53:
        case 54:
        case 55:
          flag = false;
          continue;
        case 76:
        case 108:
          goto label_2;
        default:
          goto label_3;
      }
    }
label_2:
    this.MarkTokenEnd();
    return (Token) new ConstantValueToken((object) LiteralParser.ParseBigInteger(this.GetTokenSubstring(2, this.TokenLength - 2), 8));
label_3:
    this.BufferBack();
    this.MarkTokenEnd();
    if (flag)
      this.ReportSyntaxError(new SourceSpan(new SourceLocation(this._tokenEndIndex, this.IndexToLocation(this._tokenEndIndex).Line, this.IndexToLocation(this._tokenEndIndex).Column - 1), this.BufferTokenEnd), Resources.InvalidToken, 16 /*0x10*/);
    return (Token) new ConstantValueToken(this.ParseInteger(this.GetTokenSubstring(2), 8));
  }

  private Token ReadHexNumber()
  {
    bool flag = true;
    while (true)
    {
      switch (this.NextChar())
      {
        case 48 /*0x30*/:
        case 49:
        case 50:
        case 51:
        case 52:
        case 53:
        case 54:
        case 55:
        case 56:
        case 57:
        case 65:
        case 66:
        case 67:
        case 68:
        case 69:
        case 70:
        case 97:
        case 98:
        case 99:
        case 100:
        case 101:
        case 102:
          flag = false;
          continue;
        case 76:
        case 108:
          goto label_2;
        default:
          goto label_3;
      }
    }
label_2:
    this.MarkTokenEnd();
    return (Token) new ConstantValueToken((object) LiteralParser.ParseBigInteger(this.GetTokenSubstring(2, this.TokenLength - 3), 16 /*0x10*/));
label_3:
    this.BufferBack();
    this.MarkTokenEnd();
    if (flag)
    {
      int tokenEndIndex = this._tokenEndIndex;
      SourceLocation location = this.IndexToLocation(this._tokenEndIndex);
      int line = location.Line;
      location = this.IndexToLocation(this._tokenEndIndex);
      int column = location.Column - 1;
      this.ReportSyntaxError(new SourceSpan(new SourceLocation(tokenEndIndex, line, column), this.BufferTokenEnd), Resources.InvalidToken, 16 /*0x10*/);
    }
    return (Token) new ConstantValueToken(this.ParseInteger(this.GetTokenSubstring(2), 16 /*0x10*/));
  }

  private Token ReadFraction()
  {
    int num;
    do
    {
      num = this.NextChar();
      if (num > 69)
        goto label_3;
    }
    while ((uint) (num - 48 /*0x30*/) <= 9U);
    if (num == 69)
      goto label_6;
    goto label_10;
label_3:
    if (num != 74)
    {
      if (num != 101)
      {
        if (num != 106)
          goto label_10;
      }
      else
        goto label_6;
    }
    this.MarkTokenEnd();
    return (Token) new ConstantValueToken((object) LiteralParser.ParseImaginary(this.GetTokenString()));
label_6:
    Token token = this.ReadExponent();
    if (token != null)
      return token;
    this.BufferBack();
    this.MarkTokenEnd();
    return (Token) new ConstantValueToken(this.ParseFloat(this.GetTokenString()));
label_10:
    this.BufferBack();
    this.MarkTokenEnd();
    return (Token) new ConstantValueToken(this.ParseFloat(this.GetTokenString()));
  }

  private Token ReadExponent()
  {
    int currentIndex = this.CurrentIndex;
    int num = this.NextChar();
    if (num == 45 || num == 43)
      num = this.NextChar();
    while (true)
    {
      switch (num)
      {
        case 48 /*0x30*/:
        case 49:
        case 50:
        case 51:
        case 52:
        case 53:
        case 54:
        case 55:
        case 56:
        case 57:
          num = this.NextChar();
          continue;
        default:
          goto label_3;
      }
    }
label_3:
    if (num == 74 || num == 106)
    {
      this.MarkTokenEnd();
      return (Token) new ConstantValueToken((object) LiteralParser.ParseImaginary(this.GetTokenString()));
    }
    this.BufferBack();
    if (currentIndex == this.CurrentIndex)
      return (Token) null;
    this.MarkTokenEnd();
    return (Token) new ConstantValueToken(this.ParseFloat(this.GetTokenString()));
  }

  private Token ReadName()
  {
    this.BufferBack();
    switch (this.NextChar())
    {
      case 78:
        if (this.NextChar() == 111 && this.NextChar() == 110 && this.NextChar() == 101 && !Tokenizer.IsNamePart(this.Peek()))
        {
          this.MarkTokenEnd();
          return Tokens.NoneToken;
        }
        break;
      case 97:
        switch (this.NextChar())
        {
          case 110:
            if (this.NextChar() == 100 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordAndToken;
            }
            break;
          case 115:
            if (!Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordAsToken;
            }
            if (this.NextChar() == 115 && this.NextChar() == 101 && this.NextChar() == 114 && this.NextChar() == 116 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordAssertToken;
            }
            break;
        }
        break;
      case 98:
        if (this.NextChar() == 114 && this.NextChar() == 101 && this.NextChar() == 97 && this.NextChar() == 107 && !Tokenizer.IsNamePart(this.Peek()))
        {
          this.MarkTokenEnd();
          return Tokens.KeywordBreakToken;
        }
        break;
      case 99:
        switch (this.NextChar())
        {
          case 108:
            if (this.NextChar() == 97 && this.NextChar() == 115 && this.NextChar() == 115 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordClassToken;
            }
            break;
          case 111:
            if (this.NextChar() == 110 && this.NextChar() == 116 && this.NextChar() == 105 && this.NextChar() == 110 && this.NextChar() == 117 && this.NextChar() == 101 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordContinueToken;
            }
            break;
        }
        break;
      case 100:
        if (this.NextChar() == 101)
        {
          switch (this.NextChar())
          {
            case 102:
              if (!Tokenizer.IsNamePart(this.Peek()))
              {
                this.MarkTokenEnd();
                return Tokens.KeywordDefToken;
              }
              break;
            case 108:
              if (!Tokenizer.IsNamePart(this.Peek()))
              {
                this.MarkTokenEnd();
                return Tokens.KeywordDelToken;
              }
              break;
          }
        }
        else
          break;
        break;
      case 101:
        switch (this.NextChar())
        {
          case 108:
            switch (this.NextChar())
            {
              case 105:
                if (this.NextChar() == 102 && !Tokenizer.IsNamePart(this.Peek()))
                {
                  this.MarkTokenEnd();
                  return Tokens.KeywordElseIfToken;
                }
                break;
              case 115:
                if (this.NextChar() == 101 && !Tokenizer.IsNamePart(this.Peek()))
                {
                  this.MarkTokenEnd();
                  return Tokens.KeywordElseToken;
                }
                break;
            }
            break;
          case 120:
            switch (this.NextChar())
            {
              case 99:
                if (this.NextChar() == 101 && this.NextChar() == 112 /*0x70*/ && this.NextChar() == 116 && !Tokenizer.IsNamePart(this.Peek()))
                {
                  this.MarkTokenEnd();
                  return Tokens.KeywordExceptToken;
                }
                break;
              case 101:
                if (this.NextChar() == 99 && !Tokenizer.IsNamePart(this.Peek()))
                {
                  this.MarkTokenEnd();
                  return Tokens.KeywordExecToken;
                }
                break;
            }
            break;
        }
        break;
      case 102:
        switch (this.NextChar())
        {
          case 105:
            if (this.NextChar() == 110 && this.NextChar() == 97 && this.NextChar() == 108 && this.NextChar() == 108 && this.NextChar() == 121 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordFinallyToken;
            }
            break;
          case 111:
            if (this.NextChar() == 114 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordForToken;
            }
            break;
          case 114:
            if (this.NextChar() == 111 && this.NextChar() == 109 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordFromToken;
            }
            break;
        }
        break;
      case 103:
        if (this.NextChar() == 108 && this.NextChar() == 111 && this.NextChar() == 98 && this.NextChar() == 97 && this.NextChar() == 108 && !Tokenizer.IsNamePart(this.Peek()))
        {
          this.MarkTokenEnd();
          return Tokens.KeywordGlobalToken;
        }
        break;
      case 105:
        switch (this.NextChar())
        {
          case 102:
            if (!Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordIfToken;
            }
            break;
          case 109:
            if (this.NextChar() == 112 /*0x70*/ && this.NextChar() == 111 && this.NextChar() == 114 && this.NextChar() == 116 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordImportToken;
            }
            break;
          case 110:
            if (!Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordInToken;
            }
            break;
          case 115:
            if (!Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordIsToken;
            }
            break;
        }
        break;
      case 108:
        if (this.NextChar() == 97 && this.NextChar() == 109 && this.NextChar() == 98 && this.NextChar() == 100 && this.NextChar() == 97 && !Tokenizer.IsNamePart(this.Peek()))
        {
          this.MarkTokenEnd();
          return Tokens.KeywordLambdaToken;
        }
        break;
      case 110:
        if (this.NextChar() == 111 && this.NextChar() == 116 && !Tokenizer.IsNamePart(this.Peek()))
        {
          this.MarkTokenEnd();
          return Tokens.KeywordNotToken;
        }
        break;
      case 111:
        if (this.NextChar() == 114 && !Tokenizer.IsNamePart(this.Peek()))
        {
          this.MarkTokenEnd();
          return Tokens.KeywordOrToken;
        }
        break;
      case 112 /*0x70*/:
        switch (this.NextChar())
        {
          case 97:
            if (this.NextChar() == 115 && this.NextChar() == 115 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordPassToken;
            }
            break;
          case 114:
            if (this.NextChar() == 105 && this.NextChar() == 110 && this.NextChar() == 116 && !Tokenizer.IsNamePart(this.Peek()) && !this._printFunction)
            {
              this.MarkTokenEnd();
              return Tokens.KeywordPrintToken;
            }
            break;
        }
        break;
      case 114:
        switch (this.NextChar())
        {
          case 97:
            if (this.NextChar() == 105 && this.NextChar() == 115 && this.NextChar() == 101 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordRaiseToken;
            }
            break;
          case 101:
            if (this.NextChar() == 116 && this.NextChar() == 117 && this.NextChar() == 114 && this.NextChar() == 110 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordReturnToken;
            }
            break;
        }
        break;
      case 116:
        if (this.NextChar() == 114 && this.NextChar() == 121 && !Tokenizer.IsNamePart(this.Peek()))
        {
          this.MarkTokenEnd();
          return Tokens.KeywordTryToken;
        }
        break;
      case 119:
        switch (this.NextChar())
        {
          case 104:
            if (this.NextChar() == 105 && this.NextChar() == 108 && this.NextChar() == 101 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordWhileToken;
            }
            break;
          case 105:
            if (this.NextChar() == 116 && this.NextChar() == 104 && !Tokenizer.IsNamePart(this.Peek()))
            {
              this.MarkTokenEnd();
              return Tokens.KeywordWithToken;
            }
            break;
        }
        break;
      case 121:
        if (this.NextChar() == 105 && this.NextChar() == 101 && this.NextChar() == 108 && this.NextChar() == 100 && !Tokenizer.IsNamePart(this.Peek()))
        {
          this.MarkTokenEnd();
          return Tokens.KeywordYieldToken;
        }
        break;
    }
    this.BufferBack();
    int ch = this.NextChar();
    while (Tokenizer.IsNamePart(ch))
      ch = this.NextChar();
    this.BufferBack();
    this.MarkTokenEnd();
    NameToken nameToken;
    if (!this._names.TryGetValue(Tokenizer._currentName, out nameToken))
    {
      string tokenString = this.GetTokenString();
      nameToken = this._names[(object) tokenString] = new NameToken(tokenString);
    }
    return (Token) nameToken;
  }

  public int GroupingLevel
  {
    get => this._state.ParenLevel + this._state.BraceLevel + this._state.BracketLevel;
  }

  public bool EndContinues => this._endContinues;

  private bool ReadNewline()
  {
    if (this.IndentationInconsistencySeverity != Severity.Ignore)
      return this.ReadNewlineWithChecks();
    int spaces = 0;
    int num;
    while (true)
    {
      num = this.NextChar();
      switch (num)
      {
        case 9:
          spaces += 8 - spaces % 8;
          continue;
        case 12:
          spaces = 0;
          continue;
        case 32 /*0x20*/:
          ++spaces;
          continue;
        case 35:
          if (!this._verbatim)
          {
            this.ReadLine();
            continue;
          }
          goto label_8;
        default:
          goto label_10;
      }
    }
label_8:
    this.BufferBack();
    this.MarkTokenEnd();
    return true;
label_10:
    this.BufferBack();
    if (this.GroupingLevel > 0)
      return false;
    this.MarkTokenEnd();
    switch (num)
    {
      case -1:
        if (spaces < this._state.Indent[this._state.IndentLevel])
        {
          if (this._sourceUnit.Kind == SourceCodeKind.InteractiveCode || this._sourceUnit.Kind == SourceCodeKind.Statements)
          {
            this.SetIndent(spaces, (StringBuilder) null);
            goto case 10;
          }
          this.DoDedent(spaces, this._state.Indent[this._state.IndentLevel]);
          goto case 10;
        }
        goto case 10;
      case 10:
      case 13:
        return true;
      default:
        this.SetIndent(spaces, (StringBuilder) null);
        goto case 10;
    }
  }

  private bool ReadNewlineWithChecks()
  {
    StringBuilder stringBuilder = new StringBuilder(80 /*0x50*/);
    int spaces = 0;
    int current;
    while (true)
    {
      current = this.NextChar();
      switch (current)
      {
        case 9:
          spaces += 8 - spaces % 8;
          stringBuilder.Append('\t');
          continue;
        case 12:
          spaces = 0;
          stringBuilder.Append('\f');
          continue;
        case 32 /*0x20*/:
          ++spaces;
          stringBuilder.Append(' ');
          continue;
        case 35:
          if (!this._verbatim)
          {
            this.ReadLine();
            continue;
          }
          goto label_6;
        default:
          if (this.ReadEolnOpt(current) > 0)
          {
            this._newLineLocations.Add(this.CurrentIndex);
            spaces = 0;
            stringBuilder.Length = 0;
            continue;
          }
          goto label_10;
      }
    }
label_6:
    this.BufferBack();
    this.MarkTokenEnd();
    return true;
label_10:
    this.BufferBack();
    if (this.GroupingLevel > 0)
      return false;
    this.MarkTokenEnd();
    this.CheckIndent(stringBuilder);
    switch (current)
    {
      case -1:
        if (spaces < this._state.Indent[this._state.IndentLevel])
        {
          if (this._sourceUnit.Kind == SourceCodeKind.InteractiveCode || this._sourceUnit.Kind == SourceCodeKind.Statements)
          {
            this.SetIndent(spaces, stringBuilder);
            goto case 10;
          }
          this.DoDedent(spaces, this._state.Indent[this._state.IndentLevel]);
          goto case 10;
        }
        goto case 10;
      case 10:
      case 13:
        return true;
      default:
        this.SetIndent(spaces, stringBuilder);
        goto case 10;
    }
  }

  private void CheckIndent(StringBuilder sb)
  {
    if (this._state.Indent[this._state.IndentLevel] <= 0)
      return;
    StringBuilder stringBuilder = this._state.IndentFormat[this._state.IndentLevel];
    int num = stringBuilder.Length < sb.Length ? stringBuilder.Length : sb.Length;
    for (int index = 0; index < num; ++index)
    {
      if ((int) sb[index] != (int) stringBuilder[index])
      {
        SourceLocation bufferTokenEnd = this.BufferTokenEnd;
        this._errors.Add(this._sourceUnit, Resources.InconsistentWhitespace, new SourceSpan(bufferTokenEnd, bufferTokenEnd), 48 /*0x30*/, this._indentationInconsistencySeverity);
        this._indentationInconsistencySeverity = Severity.Ignore;
      }
    }
  }

  private void SetIndent(int spaces, StringBuilder chars)
  {
    int current = this._state.Indent[this._state.IndentLevel];
    if (spaces == current)
      return;
    if (spaces > current)
    {
      this._state.Indent[++this._state.IndentLevel] = spaces;
      if (this._state.IndentFormat != null)
        this._state.IndentFormat[this._state.IndentLevel] = chars;
      this._state.PendingDedents = -1;
    }
    else
    {
      int num = this.DoDedent(spaces, current);
      if (spaces == num)
        return;
      this.ReportSyntaxError(new SourceSpan(new SourceLocation(this._tokenEndIndex, this.IndexToLocation(this._tokenEndIndex).Line, this.IndexToLocation(this._tokenEndIndex).Column - 1), this.BufferTokenEnd), Resources.IndentationMismatch, 32 /*0x20*/);
    }
  }

  private int DoDedent(int spaces, int current)
  {
    for (; spaces < current; current = this._state.Indent[this._state.IndentLevel])
    {
      --this._state.IndentLevel;
      ++this._state.PendingDedents;
    }
    return current;
  }

  private object ParseInteger(string s, int radix)
  {
    try
    {
      return LiteralParser.ParseInteger(s, radix);
    }
    catch (ArgumentException ex)
    {
      this.ReportSyntaxError(this.BufferTokenSpan, ex.Message, 16 /*0x10*/);
    }
    return ScriptingRuntimeHelpers.Int32ToObject(0);
  }

  private object ParseFloat(string s)
  {
    try
    {
      return (object) LiteralParser.ParseFloat(s);
    }
    catch (Exception ex)
    {
      this.ReportSyntaxError(this.BufferTokenSpan, ex.Message, 16 /*0x10*/);
      return (object) 0.0;
    }
  }

  internal static bool TryGetEncoding(
    Encoding defaultEncoding,
    string line,
    ref Encoding enc,
    out string encName)
  {
    encName = (string) null;
    int index1 = 0;
    if (line.Length < 10)
      return false;
    while (index1 < line.Length && char.IsWhiteSpace(line[index1]))
      ++index1;
    int num1;
    if (index1 == line.Length || line[index1] != '#' || (num1 = line.IndexOf("coding")) == -1 || line.Length <= num1 + 6 || line[num1 + 6] != ':' && line[num1 + 6] != '=')
      return false;
    int num2 = num1 + 7;
    while (num2 < line.Length && char.IsWhiteSpace(line[num2]))
      ++num2;
    if (num2 == line.Length)
      return false;
    int index2 = num2;
    while (index2 < line.Length && (line[index2] == '-' || line[index2] == '.' || char.IsLetterOrDigit(line[index2])))
      ++index2;
    encName = line.Substring(num2, index2 - num2);
    if (!StringOps.TryGetEncoding(encName, out enc))
      return false;
    enc.DecoderFallback = (DecoderFallback) new NonStrictDecoderFallback();
    return true;
  }

  private void ReportSyntaxError(SourceSpan span, string message, int errorCode)
  {
    this._errors.Add(this._sourceUnit, message, span, errorCode, Severity.FatalError);
  }

  private void ReportSyntaxError(IndexSpan span, string message, int errorCode)
  {
    this._errors.Add(this._sourceUnit, message, new SourceSpan(this.IndexToLocation(span.Start), this.IndexToLocation(span.End)), errorCode, Severity.FatalError);
  }

  [Conditional("DUMP_TOKENS")]
  private void DumpBeginningOfUnit()
  {
    Console.WriteLine("--- Source unit: '{0}' ---", (object) this._sourceUnit.Path);
  }

  [Conditional("DUMP_TOKENS")]
  private static void DumpToken(Token token)
  {
    Console.WriteLine("{0} `{1}`", (object) token.Kind, (object) token.Image.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t"));
  }

  public int[] GetLineLocations() => this._newLineLocations.ToArray();

  private string GetTokenSubstring(int offset)
  {
    return this.GetTokenSubstring(offset, this._tokenEnd - this._start - offset);
  }

  private string GetTokenSubstring(int offset, int length)
  {
    return new string(this._buffer, this._start + offset, length);
  }

  [Conditional("DEBUG")]
  private void CheckInvariants()
  {
  }

  private int Peek()
  {
    if (this._position >= this._end)
    {
      this.RefillBuffer();
      if (this._position >= this._end)
        return -1;
    }
    return (int) this._buffer[this._position];
  }

  private int ReadLine()
  {
    int current;
    do
    {
      current = this.NextChar();
    }
    while (current != -1 && !this.IsEoln(current));
    this.BufferBack();
    return current;
  }

  private void MarkTokenEnd()
  {
    this._tokenEnd = Math.Min(this._position, this._end);
    this._tokenEndIndex = this._tokenStartIndex + (this._tokenEnd - this._start);
  }

  [Conditional("DUMP_TOKENS")]
  private void DumpToken()
  {
    Console.WriteLine("--> `{0}` {1}", (object) this.GetTokenString().Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t"), (object) this.TokenSpan);
  }

  private void BufferBack() => this.SeekRelative(-1);

  internal string GetTokenString()
  {
    return new string(this._buffer, this._start, this._tokenEnd - this._start);
  }

  private int TokenLength => this._tokenEnd - this._start;

  private void SeekRelative(int disp) => this._position += disp;

  private SourceLocation BufferTokenEnd => this.IndexToLocation(this._tokenEndIndex);

  private IndexSpan BufferTokenSpan
  {
    get => new IndexSpan(this._tokenStartIndex, this._tokenEndIndex - this._tokenStartIndex);
  }

  private bool NextChar(int ch)
  {
    if (this.Peek() != ch)
      return false;
    ++this._position;
    return true;
  }

  private int NextChar()
  {
    int num = this.Peek();
    ++this._position;
    return num;
  }

  private bool AtBeginning => this._position == 0 && !this._bufferResized;

  private int CurrentIndex
  {
    get => this._tokenStartIndex + Math.Min(this._position, this._end) - this._start;
  }

  private void DiscardToken()
  {
    if (this._tokenEnd == -1)
      this.MarkTokenEnd();
    this._start = this._tokenEnd;
    this._tokenStartIndex = this._tokenEndIndex;
    this._tokenEnd = -1;
  }

  private int ReadEolnOpt(int current)
  {
    switch (current)
    {
      case 10:
        return 1;
      case 13:
        if (this._multiEolns)
        {
          if (this.Peek() != 10)
            return 1;
          this.SeekRelative(1);
          return 2;
        }
        break;
    }
    return 0;
  }

  private bool IsEoln(int current)
  {
    switch (current)
    {
      case 10:
        return true;
      case 13:
        if (this._multiEolns)
        {
          this.Peek();
          return true;
        }
        break;
    }
    return false;
  }

  private void RefillBuffer()
  {
    if (this._end == this._buffer.Length)
    {
      Tokenizer.ResizeInternal(ref this._buffer, Math.Max(Math.Max((this._end - this._start) * 2, this._buffer.Length), this._position), this._start, this._end - this._start);
      this._end -= this._start;
      this._position -= this._start;
      this._start = 0;
      this._bufferResized = true;
    }
    this._end += this._reader.Read(this._buffer, this._end, this._buffer.Length - this._end);
  }

  private static void ResizeInternal(ref char[] array, int newSize, int start, int count)
  {
    char[] dst = newSize != array.Length ? new char[newSize] : array;
    Buffer.BlockCopy((Array) array, start * 2, (Array) dst, 0, count * 2);
    array = dst;
  }

  [Conditional("DEBUG")]
  private void ClearInvalidChars()
  {
    for (int index = 0; index < this._start; ++index)
      this._buffer[index] = char.MinValue;
    for (int end = this._end; end < this._buffer.Length; ++end)
      this._buffer[end] = char.MinValue;
  }

  private Token NextOperator(int ch)
  {
    switch (ch)
    {
      case 33:
        return this.NextChar(61) ? Tokens.NotEqualsToken : (Token) Tokenizer.BadChar(ch);
      case 37:
        return this.NextChar(61) ? Tokens.ModEqualToken : Tokens.ModToken;
      case 38:
        return this.NextChar(61) ? Tokens.BitwiseAndEqualToken : Tokens.BitwiseAndToken;
      case 40:
        ++this._state.ParenLevel;
        return Tokens.LeftParenthesisToken;
      case 41:
        --this._state.ParenLevel;
        return Tokens.RightParenthesisToken;
      case 42:
        return this.NextChar(42) ? (this.NextChar(61) ? Tokens.PowerEqualToken : Tokens.PowerToken) : (this.NextChar(61) ? Tokens.MultiplyEqualToken : Tokens.MultiplyToken);
      case 43:
        return this.NextChar(61) ? Tokens.AddEqualToken : Tokens.AddToken;
      case 44:
        return Tokens.CommaToken;
      case 45:
        return this.NextChar(61) ? Tokens.SubtractEqualToken : Tokens.SubtractToken;
      case 47:
        return this.NextChar(47) ? (this.NextChar(61) ? Tokens.FloorDivideEqualToken : Tokens.FloorDivideToken) : (this.NextChar(61) ? Tokens.DivideEqualToken : Tokens.DivideToken);
      case 58:
        return Tokens.ColonToken;
      case 59:
        return Tokens.SemicolonToken;
      case 60:
        if (this.NextChar(60))
          return this.NextChar(61) ? Tokens.LeftShiftEqualToken : Tokens.LeftShiftToken;
        if (this.NextChar(61))
          return Tokens.LessThanOrEqualToken;
        return this.NextChar(62) ? Tokens.LessThanGreaterThanToken : Tokens.LessThanToken;
      case 61:
        return this.NextChar(61) ? Tokens.EqualsToken : Tokens.AssignToken;
      case 62:
        return this.NextChar(62) ? (this.NextChar(61) ? Tokens.RightShiftEqualToken : Tokens.RightShiftToken) : (this.NextChar(61) ? Tokens.GreaterThanOrEqualToken : Tokens.GreaterThanToken);
      case 64 /*0x40*/:
        return Tokens.AtToken;
      case 91:
        ++this._state.BracketLevel;
        return Tokens.LeftBracketToken;
      case 93:
        --this._state.BracketLevel;
        return Tokens.RightBracketToken;
      case 94:
        return this.NextChar(61) ? Tokens.ExclusiveOrEqualToken : Tokens.ExclusiveOrToken;
      case 96 /*0x60*/:
        return Tokens.BackQuoteToken;
      case 123:
        ++this._state.BraceLevel;
        return Tokens.LeftBraceToken;
      case 124:
        return this.NextChar(61) ? Tokens.BitwiseOrEqualToken : Tokens.BitwiseOrToken;
      case 125:
        --this._state.BraceLevel;
        return Tokens.RightBraceToken;
      case 126:
        return Tokens.TwiddleToken;
      default:
        return (Token) null;
    }
  }

  private class TokenEqualityComparer : IEqualityComparer<object>
  {
    private readonly Tokenizer _tokenizer;

    public TokenEqualityComparer(Tokenizer tokenizer) => this._tokenizer = tokenizer;

    bool IEqualityComparer<object>.Equals(object x, object y)
    {
      return x == Tokenizer._currentName ? y == Tokenizer._currentName || this.Equals((string) y) : (y == Tokenizer._currentName ? this.Equals((string) x) : (string) x == (string) y);
    }

    public int GetHashCode(object obj)
    {
      int hashCode = 5381;
      if (obj == Tokenizer._currentName)
      {
        char[] buffer = this._tokenizer._buffer;
        int start = this._tokenizer._start;
        int tokenEnd = this._tokenizer._tokenEnd;
        for (int index = start; index < tokenEnd; ++index)
        {
          int num = (int) buffer[index];
          hashCode = (hashCode << 5) + hashCode ^ num;
        }
      }
      else
      {
        foreach (int num in (string) obj)
          hashCode = (hashCode << 5) + hashCode ^ num;
      }
      return hashCode;
    }

    private bool Equals(string value)
    {
      if (this._tokenizer._tokenEnd - this._tokenizer._start != value.Length)
        return false;
      char[] buffer = this._tokenizer._buffer;
      int index = 0;
      int start = this._tokenizer._start;
      while (index < value.Length)
      {
        if ((int) value[index] != (int) buffer[start])
          return false;
        ++index;
        ++start;
      }
      return true;
    }
  }

  [Serializable]
  private class IncompleteString : IEquatable<Tokenizer.IncompleteString>
  {
    public readonly bool IsRaw;
    public readonly bool IsUnicode;
    public readonly bool IsTripleQuoted;
    public readonly bool IsSingleTickQuote;

    public IncompleteString(bool isSingleTickQuote, bool isRaw, bool isUnicode, bool isTriple)
    {
      this.IsRaw = isRaw;
      this.IsUnicode = isUnicode;
      this.IsTripleQuoted = isTriple;
      this.IsSingleTickQuote = isSingleTickQuote;
    }

    public override bool Equals(object obj)
    {
      Tokenizer.IncompleteString other = obj as Tokenizer.IncompleteString;
      return other != (Tokenizer.IncompleteString) null && this.Equals(other);
    }

    public override int GetHashCode()
    {
      return (this.IsRaw ? 1 : 0) | (this.IsUnicode ? 2 : 0) | (this.IsTripleQuoted ? 4 : 0) | (this.IsSingleTickQuote ? 8 : 0);
    }

    public static bool operator ==(
      Tokenizer.IncompleteString left,
      Tokenizer.IncompleteString right)
    {
      return (object) left == null ? (object) right == null : left.Equals(right);
    }

    public static bool operator !=(
      Tokenizer.IncompleteString left,
      Tokenizer.IncompleteString right)
    {
      return !(left == right);
    }

    public bool Equals(Tokenizer.IncompleteString other)
    {
      return !(other == (Tokenizer.IncompleteString) null) && this.IsRaw == other.IsRaw && this.IsUnicode == other.IsUnicode && this.IsTripleQuoted == other.IsTripleQuoted && this.IsSingleTickQuote == other.IsSingleTickQuote;
    }
  }

  [Serializable]
  private struct State : IEquatable<Tokenizer.State>
  {
    public int[] Indent;
    public int IndentLevel;
    public int PendingDedents;
    public bool LastNewLine;
    public Tokenizer.IncompleteString IncompleteString;
    public StringBuilder[] IndentFormat;
    public int ParenLevel;
    public int BraceLevel;
    public int BracketLevel;

    public State(Tokenizer.State state)
    {
      this.Indent = (int[]) state.Indent.Clone();
      this.LastNewLine = state.LastNewLine;
      this.BracketLevel = state.BraceLevel;
      this.ParenLevel = state.ParenLevel;
      this.BraceLevel = state.BraceLevel;
      this.PendingDedents = state.PendingDedents;
      this.IndentLevel = state.IndentLevel;
      this.IndentFormat = state.IndentFormat != null ? (StringBuilder[]) state.IndentFormat.Clone() : (StringBuilder[]) null;
      this.IncompleteString = state.IncompleteString;
    }

    public State(object dummy)
    {
      this.Indent = new int[80 /*0x50*/];
      this.LastNewLine = false;
      this.BracketLevel = this.ParenLevel = this.BraceLevel = this.PendingDedents = this.IndentLevel = 0;
      this.IndentFormat = (StringBuilder[]) null;
      this.IncompleteString = (Tokenizer.IncompleteString) null;
    }

    public override bool Equals(object obj) => obj is Tokenizer.State state && state == this;

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Tokenizer.State left, Tokenizer.State right)
    {
      return left.BraceLevel == right.BraceLevel && left.BracketLevel == right.BracketLevel && left.IndentLevel == right.IndentLevel && left.ParenLevel == right.ParenLevel && left.PendingDedents == right.PendingDedents && left.LastNewLine == right.LastNewLine && left.IncompleteString == right.IncompleteString;
    }

    public static bool operator !=(Tokenizer.State left, Tokenizer.State right) => !(left == right);

    public bool Equals(Tokenizer.State other) => this.Equals(other);
  }
}
