// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.TokenCategory
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

#nullable disable
namespace Microsoft.Scripting;

public enum TokenCategory
{
  None = 0,
  EndOfStream = 1,
  WhiteSpace = 2,
  Comment = 3,
  LineComment = 4,
  DocComment = 5,
  NumericLiteral = 6,
  CharacterLiteral = 7,
  StringLiteral = 8,
  RegularExpressionLiteral = 9,
  Keyword = 10, // 0x0000000A
  Directive = 11, // 0x0000000B
  Operator = 12, // 0x0000000C
  Delimiter = 13, // 0x0000000D
  Identifier = 14, // 0x0000000E
  Grouping = 15, // 0x0000000F
  Error = 16, // 0x00000010
  LanguageDefined = 256, // 0x00000100
}
