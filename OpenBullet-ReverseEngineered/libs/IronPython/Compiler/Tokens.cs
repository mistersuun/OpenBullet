// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Tokens
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler;

public static class Tokens
{
  public static readonly Token EndOfFileToken = (Token) new SymbolToken(TokenKind.EndOfFile, "<eof>");
  public static readonly Token NewLineToken = (Token) new SymbolToken(TokenKind.NewLine, "<newline>");
  public static readonly Token NLToken = (Token) new SymbolToken(TokenKind.NLToken, "<NL>");
  public static readonly Token IndentToken = (Token) new SymbolToken(TokenKind.Indent, "<indent>");
  public static readonly Token DedentToken = (Token) new SymbolToken(TokenKind.Dedent, "<dedent>");
  public static readonly Token CommentToken = (Token) new SymbolToken(TokenKind.Comment, "<comment>");
  public static readonly Token NoneToken = (Token) new ConstantValueToken((object) null);
  public static readonly Token DotToken = (Token) new SymbolToken(TokenKind.Dot, ".");

  public static Token AddToken { get; } = (Token) new OperatorToken(TokenKind.Add, "+", 4);

  public static Token AddEqualToken { get; } = (Token) new SymbolToken(TokenKind.AddEqual, "+=");

  public static Token SubtractToken { get; } = (Token) new OperatorToken(TokenKind.Subtract, "-", 4);

  public static Token SubtractEqualToken { get; } = (Token) new SymbolToken(TokenKind.SubtractEqual, "-=");

  public static Token PowerToken { get; } = (Token) new OperatorToken(TokenKind.Power, "**", 6);

  public static Token PowerEqualToken { get; } = (Token) new SymbolToken(TokenKind.PowerEqual, "**=");

  public static Token MultiplyToken { get; } = (Token) new OperatorToken(TokenKind.Multiply, "*", 5);

  public static Token MultiplyEqualToken { get; } = (Token) new SymbolToken(TokenKind.MultiplyEqual, "*=");

  public static Token FloorDivideToken { get; } = (Token) new OperatorToken(TokenKind.FloorDivide, "//", 5);

  public static Token FloorDivideEqualToken { get; } = (Token) new SymbolToken(TokenKind.FloorDivideEqual, "//=");

  public static Token DivideToken { get; } = (Token) new OperatorToken(TokenKind.Divide, "/", 5);

  public static Token DivideEqualToken { get; } = (Token) new SymbolToken(TokenKind.DivideEqual, "/=");

  public static Token ModToken { get; } = (Token) new OperatorToken(TokenKind.Mod, "%", 5);

  public static Token ModEqualToken { get; } = (Token) new SymbolToken(TokenKind.ModEqual, "%=");

  public static Token LeftShiftToken { get; } = (Token) new OperatorToken(TokenKind.LeftShift, "<<", 3);

  public static Token LeftShiftEqualToken { get; } = (Token) new SymbolToken(TokenKind.LeftShiftEqual, "<<=");

  public static Token RightShiftToken { get; } = (Token) new OperatorToken(TokenKind.RightShift, ">>", 3);

  public static Token RightShiftEqualToken { get; } = (Token) new SymbolToken(TokenKind.RightShiftEqual, ">>=");

  public static Token BitwiseAndToken { get; } = (Token) new OperatorToken(TokenKind.BitwiseAnd, "&", 2);

  public static Token BitwiseAndEqualToken { get; } = (Token) new SymbolToken(TokenKind.BitwiseAndEqual, "&=");

  public static Token BitwiseOrToken { get; } = (Token) new OperatorToken(TokenKind.BitwiseOr, "|", 0);

  public static Token BitwiseOrEqualToken { get; } = (Token) new SymbolToken(TokenKind.BitwiseOrEqual, "|=");

  public static Token ExclusiveOrToken { get; } = (Token) new OperatorToken(TokenKind.ExclusiveOr, "^", 1);

  public static Token ExclusiveOrEqualToken { get; } = (Token) new SymbolToken(TokenKind.ExclusiveOrEqual, "^=");

  public static Token LessThanToken { get; } = (Token) new OperatorToken(TokenKind.LessThan, "<", -1);

  public static Token GreaterThanToken { get; } = (Token) new OperatorToken(TokenKind.GreaterThan, ">", -1);

  public static Token LessThanOrEqualToken { get; } = (Token) new OperatorToken(TokenKind.LessThanOrEqual, "<=", -1);

  public static Token GreaterThanOrEqualToken { get; } = (Token) new OperatorToken(TokenKind.GreaterThanOrEqual, ">=", -1);

  public static Token EqualsToken { get; } = (Token) new OperatorToken(TokenKind.Equals, "==", -1);

  public static Token NotEqualsToken { get; } = (Token) new OperatorToken(TokenKind.NotEquals, "!=", -1);

  public static Token LessThanGreaterThanToken { get; } = (Token) new SymbolToken(TokenKind.LessThanGreaterThan, "<>");

  public static Token LeftParenthesisToken { get; } = (Token) new SymbolToken(TokenKind.LeftParenthesis, "(");

  public static Token RightParenthesisToken { get; } = (Token) new SymbolToken(TokenKind.RightParenthesis, ")");

  public static Token LeftBracketToken { get; } = (Token) new SymbolToken(TokenKind.LeftBracket, "[");

  public static Token RightBracketToken { get; } = (Token) new SymbolToken(TokenKind.RightBracket, "]");

  public static Token LeftBraceToken { get; } = (Token) new SymbolToken(TokenKind.LeftBrace, "{");

  public static Token RightBraceToken { get; } = (Token) new SymbolToken(TokenKind.RightBrace, "}");

  public static Token CommaToken { get; } = (Token) new SymbolToken(TokenKind.Comma, ",");

  public static Token ColonToken { get; } = (Token) new SymbolToken(TokenKind.Colon, ":");

  public static Token BackQuoteToken { get; } = (Token) new SymbolToken(TokenKind.BackQuote, "`");

  public static Token SemicolonToken { get; } = (Token) new SymbolToken(TokenKind.Semicolon, ";");

  public static Token AssignToken { get; } = (Token) new SymbolToken(TokenKind.Assign, "=");

  public static Token TwiddleToken { get; } = (Token) new SymbolToken(TokenKind.Twiddle, "~");

  public static Token AtToken { get; } = (Token) new SymbolToken(TokenKind.At, "@");

  public static Token KeywordAndToken { get; } = (Token) new SymbolToken(TokenKind.FirstKeyword, "and");

  public static Token KeywordAsToken { get; } = (Token) new SymbolToken(TokenKind.KeywordAs, "as");

  public static Token KeywordAssertToken { get; } = (Token) new SymbolToken(TokenKind.KeywordAssert, "assert");

  public static Token KeywordBreakToken { get; } = (Token) new SymbolToken(TokenKind.KeywordBreak, "break");

  public static Token KeywordClassToken { get; } = (Token) new SymbolToken(TokenKind.KeywordClass, "class");

  public static Token KeywordContinueToken { get; } = (Token) new SymbolToken(TokenKind.KeywordContinue, "continue");

  public static Token KeywordDefToken { get; } = (Token) new SymbolToken(TokenKind.KeywordDef, "def");

  public static Token KeywordDelToken { get; } = (Token) new SymbolToken(TokenKind.KeywordDel, "del");

  public static Token KeywordElseIfToken { get; } = (Token) new SymbolToken(TokenKind.KeywordElseIf, "elif");

  public static Token KeywordElseToken { get; } = (Token) new SymbolToken(TokenKind.KeywordElse, "else");

  public static Token KeywordExceptToken { get; } = (Token) new SymbolToken(TokenKind.KeywordExcept, "except");

  public static Token KeywordExecToken { get; } = (Token) new SymbolToken(TokenKind.KeywordExec, "exec");

  public static Token KeywordFinallyToken { get; } = (Token) new SymbolToken(TokenKind.KeywordFinally, "finally");

  public static Token KeywordForToken { get; } = (Token) new SymbolToken(TokenKind.KeywordFor, "for");

  public static Token KeywordFromToken { get; } = (Token) new SymbolToken(TokenKind.KeywordFrom, "from");

  public static Token KeywordGlobalToken { get; } = (Token) new SymbolToken(TokenKind.KeywordGlobal, "global");

  public static Token KeywordIfToken { get; } = (Token) new SymbolToken(TokenKind.KeywordIf, "if");

  public static Token KeywordImportToken { get; } = (Token) new SymbolToken(TokenKind.KeywordImport, "import");

  public static Token KeywordInToken { get; } = (Token) new SymbolToken(TokenKind.KeywordIn, "in");

  public static Token KeywordIsToken { get; } = (Token) new SymbolToken(TokenKind.KeywordIs, "is");

  public static Token KeywordLambdaToken { get; } = (Token) new SymbolToken(TokenKind.KeywordLambda, "lambda");

  public static Token KeywordNotToken { get; } = (Token) new SymbolToken(TokenKind.KeywordNot, "not");

  public static Token KeywordOrToken { get; } = (Token) new SymbolToken(TokenKind.KeywordOr, "or");

  public static Token KeywordPassToken { get; } = (Token) new SymbolToken(TokenKind.KeywordPass, "pass");

  public static Token KeywordPrintToken { get; } = (Token) new SymbolToken(TokenKind.KeywordPrint, "print");

  public static Token KeywordRaiseToken { get; } = (Token) new SymbolToken(TokenKind.KeywordRaise, "raise");

  public static Token KeywordReturnToken { get; } = (Token) new SymbolToken(TokenKind.KeywordReturn, "return");

  public static Token KeywordTryToken { get; } = (Token) new SymbolToken(TokenKind.KeywordTry, "try");

  public static Token KeywordWhileToken { get; } = (Token) new SymbolToken(TokenKind.KeywordWhile, "while");

  public static Token KeywordWithToken { get; } = (Token) new SymbolToken(TokenKind.LastKeyword, "with");

  public static Token KeywordYieldToken { get; } = (Token) new SymbolToken(TokenKind.KeywordYield, "yield");
}
