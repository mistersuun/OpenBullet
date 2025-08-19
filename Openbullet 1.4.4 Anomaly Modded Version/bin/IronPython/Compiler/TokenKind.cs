// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.TokenKind
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler;

public enum TokenKind
{
  EndOfFile = -1, // 0xFFFFFFFF
  Error = 0,
  NewLine = 1,
  Indent = 2,
  Dedent = 3,
  Comment = 4,
  Name = 8,
  Constant = 9,
  Dot = 31, // 0x0000001F
  Add = 32, // 0x00000020
  AddEqual = 33, // 0x00000021
  Subtract = 34, // 0x00000022
  SubtractEqual = 35, // 0x00000023
  Power = 36, // 0x00000024
  PowerEqual = 37, // 0x00000025
  Multiply = 38, // 0x00000026
  MultiplyEqual = 39, // 0x00000027
  FloorDivide = 40, // 0x00000028
  FloorDivideEqual = 41, // 0x00000029
  Divide = 42, // 0x0000002A
  DivideEqual = 43, // 0x0000002B
  Mod = 44, // 0x0000002C
  ModEqual = 45, // 0x0000002D
  LeftShift = 46, // 0x0000002E
  LeftShiftEqual = 47, // 0x0000002F
  RightShift = 48, // 0x00000030
  RightShiftEqual = 49, // 0x00000031
  BitwiseAnd = 50, // 0x00000032
  BitwiseAndEqual = 51, // 0x00000033
  BitwiseOr = 52, // 0x00000034
  BitwiseOrEqual = 53, // 0x00000035
  ExclusiveOr = 54, // 0x00000036
  ExclusiveOrEqual = 55, // 0x00000037
  LessThan = 56, // 0x00000038
  GreaterThan = 57, // 0x00000039
  LessThanOrEqual = 58, // 0x0000003A
  GreaterThanOrEqual = 59, // 0x0000003B
  Equals = 60, // 0x0000003C
  NotEquals = 61, // 0x0000003D
  LessThanGreaterThan = 62, // 0x0000003E
  LeftParenthesis = 63, // 0x0000003F
  RightParenthesis = 64, // 0x00000040
  LeftBracket = 65, // 0x00000041
  RightBracket = 66, // 0x00000042
  LeftBrace = 67, // 0x00000043
  RightBrace = 68, // 0x00000044
  Comma = 69, // 0x00000045
  Colon = 70, // 0x00000046
  BackQuote = 71, // 0x00000047
  Semicolon = 72, // 0x00000048
  Assign = 73, // 0x00000049
  Twiddle = 74, // 0x0000004A
  At = 75, // 0x0000004B
  FirstKeyword = 76, // 0x0000004C
  KeywordAnd = 76, // 0x0000004C
  KeywordAssert = 77, // 0x0000004D
  KeywordBreak = 78, // 0x0000004E
  KeywordClass = 79, // 0x0000004F
  KeywordContinue = 80, // 0x00000050
  KeywordDef = 81, // 0x00000051
  KeywordDel = 82, // 0x00000052
  KeywordElseIf = 83, // 0x00000053
  KeywordElse = 84, // 0x00000054
  KeywordExcept = 85, // 0x00000055
  KeywordExec = 86, // 0x00000056
  KeywordFinally = 87, // 0x00000057
  KeywordFor = 88, // 0x00000058
  KeywordFrom = 89, // 0x00000059
  KeywordGlobal = 90, // 0x0000005A
  KeywordIf = 91, // 0x0000005B
  KeywordImport = 92, // 0x0000005C
  KeywordIn = 93, // 0x0000005D
  KeywordIs = 94, // 0x0000005E
  KeywordLambda = 95, // 0x0000005F
  KeywordNot = 96, // 0x00000060
  KeywordOr = 97, // 0x00000061
  KeywordPass = 98, // 0x00000062
  KeywordPrint = 99, // 0x00000063
  KeywordRaise = 100, // 0x00000064
  KeywordReturn = 101, // 0x00000065
  KeywordTry = 102, // 0x00000066
  KeywordWhile = 103, // 0x00000067
  KeywordYield = 104, // 0x00000068
  KeywordAs = 105, // 0x00000069
  KeywordWith = 106, // 0x0000006A
  LastKeyword = 106, // 0x0000006A
  NLToken = 107, // 0x0000006B
}
